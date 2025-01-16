using HMZ_rt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HMZ_rt.Controllers
{
    [Route("UserAccounts")]
    [ApiController]
    public class UserAccounts_controller : Controller
    {
        private readonly HmzRtContext _context;
        public UserAccounts_controller(HmzRtContext context)
        {
            _context = context;
        }

        public static class PasswordHasher
        {
            public static string HashPassword(string password)
            {
                // Só generálása
                byte[] salt = RandomNumberGenerator.GetBytes(16);

                // Jelszó hashelése PBKDF2-vel
                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
                byte[] hash = pbkdf2.GetBytes(32);

                byte[] hashBytes = new byte[salt.Length + hash.Length];
                Buffer.BlockCopy(salt, 0, hashBytes, 0, salt.Length);
                Buffer.BlockCopy(hash, 0, hashBytes, salt.Length, hash.Length);

                // Visszaadás Base64 formátumban
                return Convert.ToBase64String(hashBytes);
            }


            public static bool VerifyPassword(string password, string storedHash)
            {
                // Tárolt hash Base64 dekódolása
                byte[] hashBytes = Convert.FromBase64String(storedHash);

                // Só és hash szétválasztása
                byte[] salt = new byte[16];
                Buffer.BlockCopy(hashBytes, 0, salt, 0, 16);
                byte[] storedHashBytes = new byte[32];
                Buffer.BlockCopy(hashBytes, 16, storedHashBytes, 0, 32);

                // Új hash generálása 
                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
                byte[] newHash = pbkdf2.GetBytes(32);

                // Hash-ek összehasonlítása
                return newHash.SequenceEqual(storedHashBytes);
            }
        }























            //Végpontok
        [HttpGet("listoutallusers")]
        public async Task<ActionResult<Useraccount>> Userslist()
        {
            return Ok(await _context.Useraccounts.ToListAsync());

        }





        [HttpPost("Register")]
        public async Task<ActionResult<Useraccount>> NewAccount(CreateUserDto newuser) {


            //ellenőrzés
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

            //mentés
            string hashedPassword = PasswordHasher.HashPassword(newuser.Password);

            var user = new Useraccount
            {
                Username = newuser.UserName,
                Password = hashedPassword,
                Email = newuser.Email,
                Role = "Base",
                Status = newuser.Status,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                LastLogin = DateTime.Now,
                Notes = newuser.Notes,
            };
            if (user != null) {
                await _context.Useraccounts.AddAsync(user);
                await _context.SaveChangesAsync();
                return StatusCode(201, user);
            }
            return BadRequest();
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


        [HttpGet("UsersWithNotificationsFull{UserIdd}")]
        public async Task<ActionResult<Useraccount>> GetAllNotification(int UserIdd)
        {
            var alldat = await _context.Useraccounts.Include(x => x.Notifications).Where(x=> x.UserId == UserIdd).ToListAsync();
            return Ok(alldat);
        }




        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            var user = await _context.Useraccounts
                .FirstOrDefaultAsync(u => u.Username == loginDto.UserName || u.Email == loginDto.UserName);

            if (user == null)
            {
                return Unauthorized(new { message = "Helytelen felhasználónév vagy jelszó!" });
            }

            // Jelszó ellenőrzése
            bool isPasswordValid = PasswordHasher.VerifyPassword(loginDto.Password, user.Password);

            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Helytelen felhasználónév vagy jelszó!" });
            }

            user.LastLogin = DateTime.Now;
            _context.Useraccounts.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sikeres bejelentkezés!", user });
        }



    }
}
