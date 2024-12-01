using HMZ_rt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HMZ_rt.Controllers
{
    [Route("UserAccounts")]
    [ApiController]
    public class Notifiactions_controller : Controller
    {
        private readonly HmzRtContext _context;




        // A collection to keep track of already generated IDs
        private static HashSet<string> generatedIds = new HashSet<string>();
        // Method to generate a unique numeric ID
        public string GenerateUniqueId()
        {
            if (generatedIds == null)
            {
                var ids = _context.Notifications
                       .Select(u => u.NotificationId.ToString())
                       .ToList();

                lock (generatedIds)
                {
                    foreach (var id in ids)
                    {
                        generatedIds.Add(id);
                    }
                }
            }
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















        public Notifiactions_controller(HmzRtContext context) { _context = context; }
        [HttpPost("Sent Messages")]
        public async Task<ActionResult<Notification>> NewMessage(CreateNotifiactionDto notidto)
        {
            var isis = new Notification {
                NotificationId = Convert.ToInt32(GenerateNumericId()),
                Message = notidto.Message,
                Status = notidto.Status,
                Type = notidto.Type,
                Priority = notidto.Priority,
                Notes = notidto.Notes,
                UserId = notidto.UserId,
                Category = notidto.Category,
                DateSent = DateTime.MinValue,
                DateRead = DateTime.MinValue,
            };
            if (isis != null)
            {
                await _context.Notifications.AddAsync(isis);
                await _context.SaveChangesAsync();
                return StatusCode(201, isis);
            }
            return BadRequest();
        }


        [HttpPut("UpdateSentTime{NotiId}")]
        public async Task<ActionResult<Notification>> UpdateOnSent(int NotiId)
        {
            var oke=await _context.Notifications.FirstOrDefaultAsync(x => x.NotificationId == NotiId);
            if (oke != null)
            {
                oke.DateSent = DateTime.Now;
                _context.Notifications.Update(oke);
                await _context.SaveChangesAsync();
                return Ok(oke);
            }
            return NotFound(new { message = "Nincs ilyen id-val rendelkező adat az adatbázisban." });
        }

        [HttpPut("UpdateOpenedTime{NotiId}")]
        public async Task<ActionResult<Notification>> UpdateOnOpened(int NotiId)
        {
            var oke = await _context.Notifications.FirstOrDefaultAsync(x => x.NotificationId == NotiId);
            if (oke != null)
            {
                oke.DateRead = DateTime.Now;
                _context.Notifications.Update(oke);
                await _context.SaveChangesAsync();
                return Ok(oke);
            }
            return NotFound(new { message = "Nincs ilyen id-val rendelkező adat az adatbázisban." });
        }
    }
}
