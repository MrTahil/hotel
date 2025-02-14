using HMZ_rt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
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
        private readonly TokenService _tokenService;
        private const int TwoFactorCodeExpiryDays = 1;
        private const string UnactivatedRole = "unactivated";
        private const string BaseRole = "Base";
        private const string ActivatedStatus = "activated";

        public UserAccounts_controller(HmzRtContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _tokenService = new TokenService(configuration);
        }

        public static class PasswordHasher
        {
            private const int SaltSize = 16;
            private const int HashSize = 32;
            private const int Iterations = 100000;

            public static string HashPassword(string password)
            {
                if (string.IsNullOrEmpty(password))
                    throw new ArgumentNullException(nameof(password));

                byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
                byte[] hash = pbkdf2.GetBytes(HashSize);

                byte[] hashBytes = new byte[salt.Length + hash.Length];
                Buffer.BlockCopy(salt, 0, hashBytes, 0, salt.Length);
                Buffer.BlockCopy(hash, 0, hashBytes, salt.Length, hash.Length);

                return Convert.ToBase64String(hashBytes);
            }

            public static bool VerifyPassword(string password, string storedHash)
            {
                if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
                    return false;

                try
                {
                    byte[] hashBytes = Convert.FromBase64String(storedHash);
                    byte[] salt = new byte[SaltSize];
                    Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);
                    byte[] storedHashBytes = new byte[HashSize];
                    Buffer.BlockCopy(hashBytes, SaltSize, storedHashBytes, 0, HashSize);

                    using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
                    byte[] newHash = pbkdf2.GetBytes(HashSize);

                    return newHash.SequenceEqual(storedHashBytes);
                }
                catch
                {
                    return false;
                }
            }
        }

        public class TokenService
        {
            private readonly IConfiguration _configuration;

            public TokenService(IConfiguration configuration)
            {
                _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            }

            public string GenerateJwtToken(string userId, string role)
            {
                var secretKey = _configuration["JwtSettings:SecretKey"] ??
                    throw new InvalidOperationException("JWT secret key is not configured");
                var issuer = _configuration["JwtSettings:Issuer"];
                var audience = _configuration["JwtSettings:Audience"];
                var expirationMinutes = int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"]);
                var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);

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
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private string Generate2FACode()
        {
            return new Random().Next(100000, 999999).ToString("D6");
        }

        private async Task Send2FACodeByEmail(string email, string code)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
                throw new ArgumentException("Email and code must not be null or empty");

            var smtpSettings = new SmtpSettings
            {
                Server = "smtp.gmail.com",
                Port = 587,
                Username = "hmzrtkando@gmail.com",
                Password = "kcrv dzii jrum sabt",
                FromEmail = "hmzservices@hmz.hu"
            };

            var subject = "HMZ regisztráció megerősítése";
            var body = GenerateEmailBody(code);

            using var client = new SmtpClient(smtpSettings.Server, smtpSettings.Port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings.FromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send email: {ex.Message}", ex);
            }
        }

        private string GenerateEmailBody(string code)
        {
            // Email template moved to a separate method for clarity
            return $@"<!DOCTYPE html>\r\n<html lang=\""hu\"">\r\n<head>\r\n    <meta charset=\""UTF-8\"">\r\n    <meta name=\""viewport\"" content=\""width=device-width, initial-scale=1.0\"">\r\n    <title>Biztonsági Kód</title>\r\n    <style>\r\n        body {{{{\r\n            font-family: 'Arial', sans-serif;\r\n            line-height: 1.6;\r\n            margin: 0;\r\n            padding: 0;\r\n            background-color: #f0f8ff;\r\n        }}}}\r\n        .container {{{{\r\n            max-width: 600px;\r\n            margin: 20px auto;\r\n            padding: 30px;\r\n            background-color: #ffffff;\r\n            border-radius: 10px;\r\n            box-shadow: 0 2px 5px rgba(0,0,0,0.1);\r\n        }}}}\r\n        .header {{{{\r\n            text-align: center;\r\n            color: #1a73e8;\r\n            margin-bottom: 25px;\r\n        }}}}\r\n        .code {{{{\r\n            font-size: 32px;\r\n            font-weight: bold;\r\n            color: #1565c0;\r\n            text-align: center;\r\n            margin: 30px 0;\r\n            padding: 15px;\r\n            background-color: #e3f2fd;\r\n            border-radius: 5px;\r\n            letter-spacing: 3px;\r\n        }}}}\r\n        .footer {{{{\r\n            text-align: center;\r\n            margin-top: 30px;\r\n            color: #666666;\r\n            font-size: 12px;\r\n        }}}}\r\n        .note {{{{\r\n            color: #666;\r\n            font-size: 14px;\r\n            text-align: center;\r\n            margin: 20px 0;\r\n            font-style: italic;\r\n        }}}}\r\n        .warning {{{{\r\n            text-align: center;\r\n            color: #666;\r\n            font-size: 14px;\r\n            margin: 20px 0;\r\n        }}}}\r\n        @media screen and (max-width: 600px) {{{{\r\n            .container {{{{\r\n                margin: 10px;\r\n                padding: 20px;\r\n            }}}}\r\n        }}}}\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\""container\"">\r\n        <div class=\""header\"">\r\n            <h1>Biztonsági Kód</h1>\r\n        </div>\r\n        \r\n        <p>Kedves Felhasználónk!</p>\r\n        \r\n        <p>A bejelentkezéshez szükséges kétlépcsős azonosítás kódja:</p>\r\n        \r\n        <div class=\""code\"">{{code}}</div>\r\n        \r\n        <p class=\""note\"">Ez egy automatikusan generált üzenet, kérjük ne válaszoljon rá.</p>\r\n        \r\n        <p class=\""warning\"">Ha nem Ön kezdeményezte ezt a kérést, kérjük azonnal változtassa meg jelszavát!</p>\r\n        \r\n        <div class=\""footer\"">\r\n            <p>© 2025 [hmzrt.com] Minden jog fenntartva</p>\r\n            <p>Ügyfélszolgálat: [hmzrtkando@gmail.com] | [+36 70 1231212]</p>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>";
        }

        [Authorize(Roles = "System,Admin")]
        [HttpGet("Allusers")]
        public async Task<IActionResult> GetUsersWithNotifications()
        {
            try
            {
                var usersWithNotifications = await _context.Useraccounts
                    .Include(u => u.Notifications)
                    .ToListAsync();

                return Ok(usersWithNotifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error occurred while fetching users." });
            }
        }

        [HttpPost("Register")]
        public async Task<ActionResult<Useraccount>> NewAccount(CreateUserDto newuser)
        {
            try
            {
                if (newuser == null)
                    return BadRequest(new { message = "Invalid user data" });

                if (await UserExists(newuser.UserName, newuser.Email))
                    return BadRequest(new { message = "Username or email already exists" });

                var twoFactorCode = Generate2FACode();
                var user = await CreateNewUser(newuser, twoFactorCode);

                await _context.Useraccounts.AddAsync(user);
                await _context.SaveChangesAsync();
                await Send2FACodeByEmail(user.Email, twoFactorCode);

                return StatusCode(201, new { message = "Registration successful! Please verify your account with the 2FA code." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Registration failed due to an internal error." });
            }
        }

        private async Task<bool> UserExists(string username, string email)
        {
            return await _context.Useraccounts.AnyAsync(u =>
                u.Username == username || u.Email == email);
        }

        private async Task<Useraccount> CreateNewUser(CreateUserDto newuser, string twoFactorCode)
        {
            return new Useraccount
            {
                Username = newuser.UserName,
                Password = PasswordHasher.HashPassword(newuser.Password),
                Email = newuser.Email,
                Role = UnactivatedRole,
                Status = newuser.Status,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                LastLogin = DateTime.Now,
                Notes = newuser.Notes,
                Authenticationcode = twoFactorCode,
                Authenticationexpire = DateTime.Now.AddDays(TwoFactorCodeExpiryDays)
            };
        }

        [HttpPost("Verify2FA")]
        public async Task<ActionResult> Verify2FA(string email, string code)
        {
            try
            {
                var user = await _context.Useraccounts
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                    return NotFound(new { message = "User not found" });

                if (!IsValidTwoFactorCode(user, code))
                    return BadRequest(new { message = "Invalid or expired 2FA code" });

                await ActivateUser(user);
                return Ok(new { message = "2FA verification successful! Account activated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Verification failed due to an internal error." });
            }
        }

        private bool IsValidTwoFactorCode(Useraccount user, string code)
        {
            return user.Authenticationcode == code &&
                   user.Authenticationexpire > DateTime.Now;
        }

        private async Task ActivateUser(Useraccount user)
        {
            user.Authenticationcode = ActivatedStatus;
            user.Role = BaseRole;
            user.Authenticationexpire = null;
            await _context.SaveChangesAsync();
        }

        [HttpDelete("DeleteUserById{InUserId}")]
        public async Task<ActionResult<Useraccount>> DeleteAccount(int InUserId)
        {
            try
            {
                var user = await _context.Useraccounts
                    .FirstOrDefaultAsync(x => x.UserId == InUserId);

                if (user == null)
                    return NotFound(new { message = "User not found" });

                _context.Useraccounts.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Successfully deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Deletion failed due to an internal error." });
            }
        }

        [HttpDelete("DeleteUserByUsername{Username}")]
        public async Task<ActionResult<Useraccount>> DeleteAccountByName(string Username)
        {
            try
            {
                var user = await _context.Useraccounts
                    .FirstOrDefaultAsync(x => x.Username == Username);

                if (user == null)
                    return NotFound(new { message = "User not found" });

                _context.Useraccounts.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Successfully deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Deletion failed due to an internal error." });
            }
        }

        [Authorize(Roles = "System")]
        [HttpGet("UsersWithNotifications{UserIdd}")]
        public async Task<ActionResult<Useraccount>> GetAllNotification(int UserIdd)
        {
            try
            {
                var userData = await _context.Useraccounts
                    .Include(x => x.Notifications)
                    .Where(x => x.UserId == UserIdd)
                    .ToListAsync();

                if (!userData.Any())
                    return NotFound(new { message = "User not found" });

                return Ok(userData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve notifications." });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (loginDto == null)
                    return BadRequest(new { message = "Invalid login data" });

                var user = await ValidateUser(loginDto);
                if (user == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                var (accessToken, refreshToken) = GenerateTokens(user);
                await UpdateUserRefreshToken(user, refreshToken);

                return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Login failed due to an internal error." });
            }
        }

        private async Task<Useraccount> ValidateUser(LoginDto loginDto)
        {
            var user = await _context.Useraccounts
                .FirstOrDefaultAsync(u => u.Username.Trim() == loginDto.UserName.Trim());

            if (user == null || !PasswordHasher.VerifyPassword(loginDto.Password, user.Password))
                return null;

            return user;
        }

        private (string accessToken, string refreshToken) GenerateTokens(Useraccount user)
        {
            var accessToken = _tokenService.GenerateJwtToken(user.UserId.ToString(), user.Role);
            var refreshToken = _tokenService.GenerateRefreshToken();
            return (accessToken, refreshToken);
        }

        private async Task UpdateUserRefreshToken(Useraccount user, string refreshToken)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"]));

            _context.Useraccounts.Update(user);
            await _context.SaveChangesAsync();
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken))
                    return BadRequest(new { message = "Invalid refresh token" });

                var user = await _context.Useraccounts
                    .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

                if (!IsValidRefreshToken(user))
                    return Unauthorized(new { message = "Invalid or expired token" });

                var (accessToken, newRefreshToken) = GenerateTokens(user);
                await UpdateUserRefreshToken(user, newRefreshToken);

                return Ok(new { AccessToken = accessToken, RefreshToken = newRefreshToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Token refresh failed due to an internal error." });
            }
        }

        private bool IsValidRefreshToken(Useraccount user)
        {
            return user != null && user.RefreshTokenExpiryTime > DateTime.UtcNow;
        }

        private class SmtpSettings
        {
            public string Server { get; set; }
            public int Port { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string FromEmail { get; set; }
        }
    }
}
