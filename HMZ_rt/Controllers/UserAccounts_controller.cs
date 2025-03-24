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
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        /// Constants for user account management
        private const int TwoFactorCodeExpiryDays = 1;
        private const string UnactivatedRole = "unactivated";
        private const string BaseRole = "Base";
        private const string ActivatedStatus = "activated";

        /// Constructor for initializing dependencies
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

        /// Utility class for password hashing and verification
        public static class PasswordHasher
        {
            private const int SaltSize = 16;
            private const int HashSize = 32;
            private const int Iterations = 100000;

            /// Hashes a password with a random salt
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

            /// Verifies a password against a stored hash
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

        /// Service for JWT token generation and management
        public class TokenService
        {
            private readonly IConfiguration _configuration;

            public TokenService(IConfiguration configuration)
            {
                _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            }

            /// Generates a JWT token with user claims
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

            /// Generates a random refresh token
            public string GenerateRefreshToken()
            {
                var randomNumber = new byte[32];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// Generates a random 6-digit code for two-factor authentication
        private string Generate2FACode()
        {
            return new Random().Next(100000, 999999).ToString("D6");
        }

        /// Sends a two-factor authentication code via email
        private async Task Send2FACode(string userName, string recipientEmail, string verificationCode)
        {
            try
            {
                string templatePath = Path.Combine(_env.ContentRootPath, "Html", "2fa.html");
                string emailTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

                // Replace template placeholders with actual values
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
                    Subject = "Security Code - Your Authentication Code",
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

        /// Retrieves all users with their notifications (admin access only)
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
                return StatusCode(500, new { message = "Server error occurred" });
            }
        }

        /// Registers a new user with email verification
        [HttpPost("Register")]
        public async Task<ActionResult<Useraccount>> NewAccount(CreateUserDto newuser)
        {
            try
            {
                if (newuser == null)
                    return BadRequest(new { message = "Invalid user data" });

                // Check if username or email already exists
                if (await UserExists(newuser.UserName, newuser.Email))
                    return BadRequest(new { message = "Username or Email is already in use" });

                // Generate 2FA code and create new user
                var twoFactorCode = Generate2FACode();
                var user = await CreateNewUser(newuser, twoFactorCode);

                await _context.Useraccounts.AddAsync(user);
                await _context.SaveChangesAsync();
                await Send2FACode(user.Username, user.Email, twoFactorCode);

                return StatusCode(201, new { message = "Registration successful, activation code sent to email" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// Checks if a username or email is already in use
        private async Task<bool> UserExists(string username, string email)
        {
            return await _context.Useraccounts.AnyAsync(u =>
                u.Username == username || u.Email == email);
        }

        /// Creates a new user entity with initial settings
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

        /// Verifies a two-factor authentication code to activate a user account
        [HttpPost("Verify2FA")]
        public async Task<ActionResult> Verify2FA(fa2 dto)
        {
            try
            {
                var user = await _context.Useraccounts
                    .FirstOrDefaultAsync(u => u.Email == dto.Email);

                if (user == null)
                    return NotFound(new { message = "User not found" });

                // Validate the 2FA code
                if (!IsValidTwoFactorCode(user, dto.Code))
                    return BadRequest(new { message = "Invalid or expired authentication code" });

                await ActivateUser(user);
                return Ok(new { message = "Activation successful, you can now log in" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error during activation" });
            }
        }

        /// Validates if a 2FA code is correct and not expired
        private bool IsValidTwoFactorCode(Useraccount user, string code)
        {
            return user.Authenticationcode == code &&
                   user.Authenticationexpire > DateTime.Now;
        }

        /// Activates a user account after successful verification
        private async Task ActivateUser(Useraccount user)
        {
            user.Authenticationcode = ActivatedStatus;
            user.Role = BaseRole;
            user.Authenticationexpire = null;
            await _context.SaveChangesAsync();
        }

        /// Deletes a user account with password verification
        [HttpDelete("DeleteUserByUsername/{Username}")]
        public async Task<ActionResult<Useraccount>> DeleteAccountByName(string Username, DeleteAccount ddto)
        {
            try
            {
                var user = await _context.Useraccounts
                    .FirstOrDefaultAsync(x => x.Username == Username);

                if (user == null)
                    return NotFound(new { message = "User not found" });

                // Note: There's a logic error here - the condition should check if ddto is null
                if (ddto == null)
                {
                    return StatusCode(404, "Password cannot be empty");
                }

                // Verify password before deletion
                if (PasswordHasher.VerifyPassword(ddto.Password, user.Password))
                {
                    _context.Useraccounts.Remove(user);
                    await _context.SaveChangesAsync();
                    return Ok("Successfully deleted");
                }

                return BadRequest("Incorrect password");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Deletion failed due to server error" });
            }
        }

        /// Retrieves notifications for a specific user (admin access only)
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
                    return NotFound(new { message = "User not found" });

                return Ok(userData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve data" });
            }
        }

        /// Authenticates a user and issues JWT tokens
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (loginDto == null)
                    return BadRequest(new { message = "Invalid login data" });

                // Validate user credentials
                var user = await ValidateUser(loginDto);
                if (user == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                // Generate tokens and update user's refresh token
                var (accessToken, refreshToken) = GenerateTokens(user);
                await UpdateUserRefreshToken(user, refreshToken);

                // Return role information for admin users
                if (user.Role == "System" || user.Role == "Recept" || user.Role == "Admin")
                {
                    return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken, user.Role });
                }

                return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Login failed due to server error" });
            }
        }

        /// Validates user credentials during login
        private async Task<Useraccount> ValidateUser(LoginDto loginDto)
        {
            var user = await _context.Useraccounts
                .FirstOrDefaultAsync(u => u.Username.Trim() == loginDto.UserName.Trim());

            if (user == null || !PasswordHasher.VerifyPassword(loginDto.Password, user.Password))
                return null;

            return user;
        }

        /// Generates access and refresh tokens for a user
        private (string accessToken, string refreshToken) GenerateTokens(Useraccount user)
        {
            var accessToken = _tokenService.GenerateJwtToken(user.UserId.ToString(), user.Role);
            var refreshToken = _tokenService.GenerateRefreshToken();
            return (accessToken, refreshToken);
        }

        /// Updates a user's refresh token in the database
        private async Task UpdateUserRefreshToken(Useraccount user, string refreshToken)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"]));

            _context.Useraccounts.Update(user);
            await _context.SaveChangesAsync();
        }

        /// Issues new tokens using a valid refresh token
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken))
                    return BadRequest(new { message = "Invalid token" });

                var user = await _context.Useraccounts
                    .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

                // Validate refresh token
                if (!IsValidRefreshToken(user))
                    return Unauthorized(new { message = "Invalid or expired token" });

                // Generate new tokens
                var (accessToken, newRefreshToken) = GenerateTokens(user);
                await UpdateUserRefreshToken(user, newRefreshToken);

                return Ok(new { AccessToken = accessToken, RefreshToken = newRefreshToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Token refresh failed due to server error" });
            }
        }

        /// Initiates password reset by sending a verification code
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
                        return NotFound(new { message = "User not found!" });

                    // Set authentication code and expiry
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



        /// Verifies the code for forgotten password reset
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

        /// Sets a new password after forgotten password verification
        [HttpPut("SetNewPassword")]
        public async Task<ActionResult<Useraccount>> setnewpass(Forgotpass1 frgdto)
        {
            var user = await _context.Useraccounts.FirstOrDefaultAsync(x => x.Email == frgdto.Email);

            if (user != null && user.Authenticationcode == "confirmed")
            {
                if (PasswordHasher.VerifyPassword(frgdto.Password, user.Password))
                {
                    return StatusCode(400, "The new password cannot be the same as the old one.");
                }
                user.Password = PasswordHasher.HashPassword(frgdto.Password);
                user.Authenticationcode = "activated";
                _context.Useraccounts.Update(user);
                await _context.SaveChangesAsync();
                return StatusCode(201, new { message = "Password changed successfully!" });
            }
            return StatusCode(500, new { message = "Something went wrong" });
        }

        /// Checks if the refresh token is still valid
        private bool IsValidRefreshToken(Useraccount user)
        {
            return user != null && user.RefreshTokenExpiryTime > DateTime.UtcNow;
        }

        /// Retrieves user ID by email (admin access only)
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

        /// Retrieves user data by username
        [Authorize(Roles = "System,Admin,Recept,Base")]
        [HttpGet("GetOneUserData/{username}")]
        public async Task<ActionResult<Useraccount>> GetOneUserDataByUsername(string username)
        {
            try
            {
                var data = await _context.Useraccounts.FirstOrDefaultAsync(x => x.Username == username);
                if (data != null)
                {
                    return StatusCode(200, data);
                }
                return StatusCode(404, "User not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Sets a new password when the old password is known
        [Authorize(Roles = "System,Admin,Recept,Base")]
        [HttpPut("Newpasswithknownpass/{username}")]
        public async Task<ActionResult<Useraccount>> SetNewPassOnKnown(string username, SetNewPass udto)
        {
            try
            {
                var data = await _context.Useraccounts.FirstOrDefaultAsync(x => x.Username == username);
                if (data != null)
                {
                    if (PasswordHasher.VerifyPassword(udto.OldPassword, data.Password))
                    {
                        if (PasswordHasher.VerifyPassword(udto.Password, data.Password))
                        {
                            return StatusCode(400, "The new password cannot be the same as the old one.");
                        }
                        data.Password = PasswordHasher.HashPassword(udto.Password);
                        _context.Useraccounts.Update(data);
                        await _context.SaveChangesAsync();
                        return StatusCode(201, "Password changed successfully");
                    }
                    return StatusCode(400, "Incorrect password");
                }
                return StatusCode(404, "User not found (frontend error)");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}