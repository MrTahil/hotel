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
        // A collection to keep track of already generated IDs (in a real scenario, use a database)
        private static HashSet<string> generatedIds = new HashSet<string>();


        // Method to generate a unique numeric ID
        public static string GenerateUniqueId()
        {
            string uniqueId;
            do
            {
                // Generate the ID by combining a timestamp and random digits
                uniqueId = GenerateNumericId();
            }
            while (generatedIds.Contains(uniqueId)); // Check if the ID already exists

            // Add the generated ID to the collection (in practice, store it in a database)
            generatedIds.Add(uniqueId);
            return uniqueId;
        }

        // Method to generate a random numeric ID
        public static string GenerateNumericId()
        {
            // Combine current timestamp (e.g., the number of seconds since Unix epoch) and random digits
            string timestamp = DateTime.UtcNow.Ticks.ToString().Substring(10); // Take the last 10 digits of the timestamp (for 11 digit ID)
            string randomPart = GenerateRandomNumericString(1); // Add 1 random digit to make the ID 11 characters long

            // Combine both parts to create an 11-digit ID
            string id = timestamp + randomPart;

            return id.Length > 11 ? id.Substring(0, 11) : id; // Ensure the ID is exactly 11 characters long
        }

        // Method to generate a random numeric string of a specified length
        private static string GenerateRandomNumericString(int length)
        {
            Random random = new Random();
            string result = string.Empty;
            for (int i = 0; i < length; i++)
            {
                result += random.Next(0, 10); // Random digit between 0 and 9
            }
            return result;
        }








        [HttpGet]
        public async Task<ActionResult<Useraccount>> Userslist()
        {
            return Ok(await _context.Useraccounts.ToListAsync());
            
        }



        [HttpGet]
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




        [HttpPost]
        public async Task<ActionResult<Useraccount>> NewAccount(CreateUserDto newuser) {

            var user = new Useraccount
            {
                Username = newuser.Username,
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
    }
}
