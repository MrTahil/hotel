using HMZ_rt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;

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

        // A collection to keep track of already generated IDs
        private static HashSet<string> generatedIds = new HashSet<string>();



        // Method to generate a unique numeric ID
        public static string GenerateUniqueId()
        {
            string uniqueId;
            do
            {

                uniqueId = GenerateNumericId();
            }
            while (generatedIds.Contains(uniqueId)); // Check if the ID already exists


            generatedIds.Add(uniqueId);
            return uniqueId;
        }

        // Method to generate a random numeric ID
        public static string GenerateNumericId()
        {
            // Combine current timestamp (e.g., the number of seconds since Unix epoch) and random digits
            string timestamp = DateTime.UtcNow.Ticks.ToString().Substring(10); // Take the last 10 digits of the timestamp (for 11 digit ID)
            string randomPart = GenerateRandomNumericString(1); // Add 1 random digit to make the ID 11 characters long


            string id = timestamp + randomPart;

            return id.Length > 11 ? id.Substring(0, 11) : id;
        }

        // Method to generate a random numeric string of a specified length
        private static string GenerateRandomNumericString(int length)
        {
            Random random = new Random();
            string result = string.Empty;
            for (int i = 0; i < length; i++)
            {
                result += random.Next(0, 10);
            }
            return result;
        }








        [HttpGet("listoutallusers")]
        public async Task<ActionResult<Useraccount>> Userslist()
        {
            return Ok(await _context.Useraccounts.ToListAsync());

        }



        [HttpGet("idsfortheids")]
        public async Task<ActionResult<HashSet<string>>> Usersids()
        {
            var ids = await _context.Useraccounts
                                    .Select(u => u.UserId.ToString())
                                    .ToListAsync();

            lock (generatedIds)
            {
                foreach (var id in ids)
                {
                    generatedIds.Add(id);
                }
            }


            return Ok(generatedIds);
        }




        [HttpPost("NewUserGenerating")]
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
            var user = new Useraccount
            {
                Username = newuser.UserName,
                UserId = Convert.ToInt32(GenerateUniqueId()),
                Password = newuser.Password,
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


    }
}
