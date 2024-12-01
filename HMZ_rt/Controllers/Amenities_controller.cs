using HMZ_rt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMZ_rt.Controllers
{
    [Route("Amenities")]
    [ApiController]
    public class Amenities_controller : Controller
    {
        private readonly HmzRtContext _context;
        public Amenities_controller(HmzRtContext context) { _context = context; }


        // A collection to keep track of already generated IDs
        private static HashSet<string> generatedIds = new HashSet<string>();
        // Method to generate a unique numeric ID
        public string GenerateUniqueId()
        {
            if (generatedIds == null)
            {
                var ids = _context.Amenities
                       .Select(u => u.AmenityId.ToString())
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




        //TODO !!!!!
        [HttpPost("New aminiti{Amenity}")]
        public async Task<ActionResult<Amenity>> GenerateNewAmenitie(string Amenity)
        {
            string[] tipes = Amenity.Split(';');
            foreach (var s in tipes) {
                switch (s)
                {
                    
                    case "klima":
                        var ne = new Amenity {
                        AmenityName = s,

                        };
                        return Ok(ne);
                    default:return BadRequest();
                }
               
            } 
            return BadRequest();
        }


    }
}
