using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace HMZ_rt.Controllers
{
    [Route("UserAccounts")]
    [ApiController]
    public class UserAccounts_controller : ControllerBase
    {
        private readonly HmzRtContext _context;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;
        private readonly IWebHostEnvironment _env;
        private readonly SmtpSettings _smtpSettings;

        private const int TwoFactorCodeExpiryDays = 1;
        private const string UnactivatedRole = "unactivated";
        private const string BaseRole = "Base";
        private const string ActivatedStatus = "activated";

        public UserAccounts_controller(
            HmzRtContext context,
            IConfiguration configuration,
            IWebHostEnvironment env,
            IOptions<SmtpSettings> smtpOptions,
            TokenService tokenService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _smtpSettings = smtpOptions.Value ?? throw new ArgumentNullException(nameof(smtpOptions));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
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




        private async Task Send2FACode(string userName, string recipientEmail, string verificationCode)
        {
            try
            {
                string templatePath = Path.Combine(_env.ContentRootPath, "Html", "2fa.html");
                string emailTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

                emailTemplate = emailTemplate
                    .Replace("{UserName}", userName)
                    .Replace("{VerificationCode}", verificationCode)
                    .Replace("{ExpirationTime}", "15")
                    .Replace("{CurrentYear}", DateTime.Now.Year.ToString())
                    .Replace("{CompanyName}", "HMZ RT")
                    .Replace("{CompanyAddress}", "Palóczy László utca 3, Miskolc, BAZ, 3531")
                    .Replace("{PrivacyPolicyUrl}", "https://yourcompany.com/privacy")
                    .Replace("{TermsUrl}", "https://yourcompany.com/terms");

                using MailMessage message = new()
                {
                    From = new MailAddress(_smtpSettings.FromEmail),
                    Subject = "Biztonsági Kód - Az Ön hitelesítési kódja",
                    Body = emailTemplate,
                    IsBodyHtml = true,
                    Priority = MailPriority.High
                };
                message.To.Add(recipientEmail);

                using SmtpClient client = new(_smtpSettings.Server, _smtpSettings.Port)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password)
                };

                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send 2FA email", ex);
            }
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
                return StatusCode(500, new { message = "Szerverhiba lépett fel" });
            }
        }

        [HttpPost("Register")]
        public async Task<ActionResult<Useraccount>> NewAccount(CreateUserDto newuser)
        {
            try
            {
                if (newuser == null)
                    return BadRequest(new { message = "Hibás felhasználói adatok" });

                if (await UserExists(newuser.UserName, newuser.Email))
                    return BadRequest(new { message = "Név vagy Email már használatban van" });

                var twoFactorCode = Generate2FACode();
                var user = await CreateNewUser(newuser, twoFactorCode);

                await _context.Useraccounts.AddAsync(user);
                await _context.SaveChangesAsync();
                await Send2FACode(user.Username, user.Email, twoFactorCode);

                return StatusCode(201, new { message = "Sikeres regisztráció, emailben elküldtük az aktiváló kódot" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
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
        public async Task<ActionResult> Verify2FA(fa2 dto)
        {
            try
            {
                var user = await _context.Useraccounts
                    .FirstOrDefaultAsync(u => u.Email == dto.Email);

                if (user == null)
                    return NotFound(new { message = "Felhasználható nem található" });

                if (!IsValidTwoFactorCode(user, dto.Code))
                    return BadRequest(new { message = "Hibás vagy lejárt hitelesítő kód" });

                await ActivateUser(user);
                return Ok(new { message = "Sikeres aktiválás, mostmár bejelentkezhetsz" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Szerver miatti sikertelen aktiváls, ha a hiba továbbra is fenn áll kérlek kérd a munkatársaink segítéségét" });
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

        [HttpDelete("DeleteUserById/{InUserId}")]
        public async Task<ActionResult<Useraccount>> DeleteAccount(int InUserId)
        {
            try
            {
                var user = await _context.Useraccounts
                    .FirstOrDefaultAsync(x => x.UserId == InUserId);

                if (user == null)
                    return NotFound(new { message = "Felhasználó nem található" });

                _context.Useraccounts.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Sikeres törlés" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Szerverhiba miatt sikertelen törlés" });
            }
        }

        [HttpDelete("DeleteUserByUsername/{Username}")]
        public async Task<ActionResult<Useraccount>> DeleteAccountByName(string Username)
        {
            try
            {
                var user = await _context.Useraccounts
                    .FirstOrDefaultAsync(x => x.Username == Username);

                if (user == null)
                    return NotFound(new { message = "Felhasználó nem található" });

                _context.Useraccounts.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Sikeres törlés" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Szerverhiba miatt sikertelen törlés" });
            }
        }

        [Authorize(Roles = "System,Admin")]
        [HttpGet("UsersWithNotifications/{UserIdd}")]
        public async Task<ActionResult<Useraccount>> GetAllNotification(int UserIdd)
        {

            try
            {
                var userData = await _context.Useraccounts
                    .Include(x => x.Notifications)
                    .Where(x => x.UserId == UserIdd)
                    .ToListAsync();

                if (!userData.Any())
                    return NotFound(new { message = "Felhasználó nem található" });

                return Ok(userData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Az adatok lekérése sikertelen" });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (loginDto == null)
                    return BadRequest(new { message = "Hibás bejelentési adatok" });

                var user = await ValidateUser(loginDto);
                if (user == null)
                    return Unauthorized(new { message = "Hibás adatok" });

                var (accessToken, refreshToken) = GenerateTokens(user);
                await UpdateUserRefreshToken(user, refreshToken);
                if (user.Role == "System" || user.Role == "Recept" || user.Role == "Admin")
                {


                    return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken, user.Role });
                }
                return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Sikertelen bejelentkezés szerverhiba miatt" });
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
                    return BadRequest(new { message = "Hibás token" });

                var user = await _context.Useraccounts
                    .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

                if (!IsValidRefreshToken(user))
                    return Unauthorized(new { message = "Hibás vagy lejárt token" });

                var (accessToken, newRefreshToken) = GenerateTokens(user);
                await UpdateUserRefreshToken(user, newRefreshToken);

                return Ok(new { AccessToken = accessToken, RefreshToken = newRefreshToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Szerverhiba miatt sikertelen tokenrefresh" });
            }
        }


        [HttpPost("ForgotPasswordsendemail/{email}")]
        public async Task<ActionResult<Useraccount>> Forgotpass(string email)
        {
            try
            {
                if (await _context.Useraccounts.AnyAsync(x => x.Email == email))
                {
                    string code = Generate2FACode();

                    var user = await _context.Useraccounts
                        .FirstOrDefaultAsync(x => x.Email == email);

                    if (user == null)
                        return NotFound(new { message = "Felhasználó nem található!" });
                    user.Authenticationcode = code;
                    user.Authenticationexpire = DateTime.Now.AddDays(TwoFactorCodeExpiryDays);
                    await Send2FACode(user.Username, email, code);
                    _context.Useraccounts.Update(user);
                    await _context.SaveChangesAsync();
                    return StatusCode(200);
                }
                return StatusCode(418);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }




        [HttpPut("VerifyTheforgotpass")]
        public async Task<ActionResult<Useraccount>> forgotpasspart2(Forgotpass frgdto)
        {
            var user = await _context.Useraccounts.FirstOrDefaultAsync(x => x.Email == frgdto.Email);
            if (user != null)
            {

                if (user.Authenticationcode == frgdto.Code)
                {
                    user.Authenticationcode = "confirmed";
                    _context.Useraccounts.Update(user);
                    await _context.SaveChangesAsync();
                    return StatusCode(201);
                }
            }
            return StatusCode(202);
        }
        [HttpPut("SetNewPassword")]
        public async Task<ActionResult<Useraccount>> setnewpass(Forgotpass1 frgdto)
        {
            var user = await _context.Useraccounts.FirstOrDefaultAsync(x => x.Email == frgdto.Email);

            if (user != null)
            {
                { if (user.Authenticationcode == "confirmed")
                    {
                        user.Password = PasswordHasher.HashPassword(frgdto.Password);
                        user.Authenticationcode = "000000";
                        _context.Useraccounts.Update(user);
                        await _context.SaveChangesAsync();
                        return StatusCode(201, new { message = "Sikeres jelszóváltoztatás!" });
                    } }
            }
            return StatusCode(500, new { message = "Valami nem jó biza" });
        }

        private bool IsValidRefreshToken(Useraccount user)
        {
            return user != null && user.RefreshTokenExpiryTime > DateTime.UtcNow;
        }


        [Authorize(Roles = "System,Admin,Recept")]
        [HttpGet("GetId/{email}")]
        public async Task<ActionResult<Useraccount>> GetUserIdByEmail(string email)
        {
            try
            {
                var dat = await _context.Useraccounts.FirstOrDefaultAsync(x => x.Email == email);
                return StatusCode(200, dat.UserId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "System,Admin,Recept,Base")]
        [HttpGet("GetOneUserData/{username}")]
        public async Task<ActionResult<Useraccount>> GetOneUserDataByUsername(string username)
        {
            try
            {
                var data =await _context.Useraccounts.FirstOrDefaultAsync(x => x.Username == username);
                if (data != null)
                {
                    return StatusCode(200, data);
                } return StatusCode(404, "Nem található felhasználó");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }

        [Authorize(Roles = "System,Admin,Recept,Base")]
        [HttpPut("Newpasswithknownpass/{username}")]
        public async Task<ActionResult<Useraccount>> SetNewPassOnKnown(string username, SetNewPass udto)
        {
            try
            {
                var data =await _context.Useraccounts.FirstOrDefaultAsync(x => x.Username == username);
                if (data != null)
                {
                    if (PasswordHasher.VerifyPassword(udto.OldPassword, data.Password))
                    {
                        data.Password = PasswordHasher.HashPassword(udto.Password);
                        _context.Useraccounts.Update(data);
                        await _context.SaveChangesAsync();
                        return StatusCode(201, "Sikres jelszó változtatás");

                    }
                    return StatusCode(400, "Hibás jelszó");
                    
                }
                return StatusCode(404, "Nem található a felhasználó(elrontotta a frontend)");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
            
        }
    } 
}
