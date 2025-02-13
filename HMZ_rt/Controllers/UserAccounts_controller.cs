using HMZ_rt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using System.Net;

namespace HMZ_rt.Controllers
{
    [Route("UserAccounts")]
    [ApiController]
    public class UserAccounts_controller : Controller
    {
        private readonly HmzRtContext _context;
        private readonly IConfiguration _configuration;

        public UserAccounts_controller(HmzRtContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public static class PasswordHasher
        {
            public static string HashPassword(string password)
            {
                byte[] salt = RandomNumberGenerator.GetBytes(16);
                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
                byte[] hash = pbkdf2.GetBytes(32);

                byte[] hashBytes = new byte[salt.Length + hash.Length];
                Buffer.BlockCopy(salt, 0, hashBytes, 0, salt.Length);
                Buffer.BlockCopy(hash, 0, hashBytes, salt.Length, hash.Length);

                return Convert.ToBase64String(hashBytes);
            }

            public static bool VerifyPassword(string password, string storedHash)
            {
                byte[] hashBytes = Convert.FromBase64String(storedHash);
                byte[] salt = new byte[16];
                Buffer.BlockCopy(hashBytes, 0, salt, 0, 16);
                byte[] storedHashBytes = new byte[32];
                Buffer.BlockCopy(hashBytes, 16, storedHashBytes, 0, 32);

                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
                byte[] newHash = pbkdf2.GetBytes(32);

                return newHash.SequenceEqual(storedHashBytes);
            }
        }

        public class TokenService
        {
            private readonly IConfiguration _configuration;

            public TokenService(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public string GenerateJwtToken(string userId, string role)
            {
                var secretKey = _configuration["JwtSettings:SecretKey"];
                var issuer = _configuration["JwtSettings:Issuer"];
                var audience = _configuration["JwtSettings:Audience"];
                var expiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"]));

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userId),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: expiration,
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            public string GenerateRefreshToken()
            {
                var randomNumber = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomNumber);
                    return Convert.ToBase64String(randomNumber);
                }
            }
        }
        private string Generate2FACode()
        {
            // 6 számjegyű véletlenszerű kód generálása
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }


        private async Task Send2FACodeByEmail(string email, string code)
        {
            var smtpSettings = new SmtpSettings
            {
                Server = "smtp.gmail.com",
                Port = 587,
                Username = "hmzrtkando@gmail.com",
                Password = "kcrv dzii jrum sabt",
                FromEmail = "hmzservices@hmz.hu"
            };

            var subject = "HMZ regisztráció megerősítése";
            var body = $"<h1>Kétfaktoros hitelesítés</h1><p>Az Ön hitelesítési kódja: <strong>{code}</strong></p>";

            using (var client = new SmtpClient(smtpSettings.Server, smtpSettings.Port))
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpSettings.FromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
        }































        //Végpontok
        [Authorize(Roles = "System,Admin")]
        [HttpGet("Allusers")]
        public async Task<IActionResult> GetUsersWithNotifications()
        {
            var usersWithNotifications = await _context.Useraccounts
                .Include(u => u.Notifications) // Kapcsolódó Notifications betöltése
                .ToListAsync();

            return Ok(usersWithNotifications);
        }





        [HttpPost("Register")]
        public async Task<ActionResult<Useraccount>> NewAccount(CreateUserDto newuser)
        {
            // Ellenőrzés
            var existingUserByUsername = await _context.Useraccounts
                .FirstOrDefaultAsync(u => u.Username == newuser.UserName);

            if (existingUserByUsername != null)
            {
                return BadRequest(new { message = "Ez a felhasználónév már használatban van!" });
            }

            var existingUserByEmail = await _context.Useraccounts
                .FirstOrDefaultAsync(u => u.Email == newuser.Email);

            if (existingUserByEmail != null)
            {
                return BadRequest(new { message = "Ez az E-mail már használatban van!" });
            }

            // 2FA kód generálása
            var twoFactorCode = Generate2FACode();
            var twoFactorCodeExpiry = DateTime.Now.AddDays(1); // 10 percig érvényes

            // Mentés
            string hashedPassword = PasswordHasher.HashPassword(newuser.Password);

            var user = new Useraccount
            {
                Username = newuser.UserName,
                Password = hashedPassword,
                Email = newuser.Email,
                Role = "unactivated",
                Status = newuser.Status,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                LastLogin = DateTime.Now,
                Notes = newuser.Notes,

                // 2FA mezők
                Authenticationcode = twoFactorCode,
                Authenticationexpire = twoFactorCodeExpiry
            };
            
            if (user != null)
            {
                await _context.Useraccounts.AddAsync(user);
                await _context.SaveChangesAsync();

                // 2FA kód küldése e-mailben
                await Send2FACodeByEmail(user.Email, twoFactorCode);

                return StatusCode(201, new { message = "Regisztráció sikeres! Kérjük, erősítse meg a fiókját a kapott 2FA kóddal." });
            }
            return StatusCode(418);

        }

        [HttpPost("Verify2FA")]
        public async Task<ActionResult> Verify2FA(string email, string code)
        {
            var user = await _context.Useraccounts
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound(new { message = "Felhasználó nem található!" });
            }

            // Ellenőrzés, hogy a kód érvényes-e
            if (user.Authenticationcode == code && user.Authenticationexpire > DateTime.Now)
            {
                // 2FA sikeres, kód törlése
                user.Authenticationcode = "activated";
                user.Role = "Base";
                user.Authenticationexpire = null;
                await _context.SaveChangesAsync();

                return Ok(new { message = "2FA hitelesítés sikeres! Fiókja aktiválva." });
            }

            return BadRequest(new { message = "Érvénytelen vagy lejárt 2FA kód!" });
        }

























        [HttpDelete("DeleteUserById{InUserId}")]
        public async Task<ActionResult<Useraccount>> DeleteAccount(int InUserId)
        {
            var os = await _context.Useraccounts.FirstOrDefaultAsync(x => x.UserId == InUserId);
            if (os != null)
            {
                _context.Useraccounts.Remove(os);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Sikeresen törölve!" });
            }
            return NotFound();
        }


        [HttpDelete("DeleteUserByUsername{Username}")]
        public async Task<ActionResult<Useraccount>> DeleteAccountByName(string Username)
        {
            var os = await _context.Useraccounts.FirstOrDefaultAsync(x => x.Username == Username);
            if (os != null)
            {
                _context.Useraccounts.Remove(os);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Sikeresen törölve!" });
            }
            return NotFound();
        }


        [Authorize(Roles = "System")]
        [HttpGet("UsersWithNotifications{UserIdd}")]
        public async Task<ActionResult<Useraccount>> GetAllNotification(int UserIdd)
        {
            var alldat = await _context.Useraccounts.Include(x => x.Notifications).Where(x=> x.UserId == UserIdd).ToListAsync();
            return Ok(alldat);
        }




        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Useraccounts
                .FirstOrDefaultAsync(u => u.Username.Trim() == loginDto.UserName.Trim());


            if (user == null || !PasswordHasher.VerifyPassword(loginDto.Password, user.Password))
                return Unauthorized(new { message = "Invalid credentials" });

            var tokenService = new TokenService(_configuration);

            var jwtToken = tokenService.GenerateJwtToken(user.UserId.ToString(), user.Role);
            var refreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"]));

            _context.Useraccounts.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = jwtToken,
                RefreshToken = refreshToken
            });
        }


        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var user = await _context.Useraccounts.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return Unauthorized(new { message = "Invalid vagy lejárt token" });

            var tokenService = new TokenService(_configuration);
            var newJwtToken = tokenService.GenerateJwtToken(user.UserId.ToString(), user.Role);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"]));

            _context.Useraccounts.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = newJwtToken,
                RefreshToken = newRefreshToken
            });
        }

            








    }
}
