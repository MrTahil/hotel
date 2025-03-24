using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMZ_rt.Controllers
{
    /// Controller for managing hotel room amenities
    [Route("Amenities")]
    [ApiController]
    public class Amenities_controller : Controller
    {
        private readonly HmzRtContext _context;  // Database context for DB operations

        public Amenities_controller(HmzRtContext context)
        {
            _context = context;
        }

        /// Creates a new amenity for a room
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPost("New aminiti")]
        public async Task<ActionResult<Amenity>> GenerateNewAmenitie(UploadAmenitiesForNewRoomDto upload)
        {
            try
            {
                // Create new amenity from DTO data
                var amenity = new Amenity
                {
                    AmenityName = upload.AmenityName,
                    Description = upload.Descript,
                    Availability = upload.Availability,
                    RoomId = upload.RoomId,
                    Status = upload.Status,
                    Icon = upload.Icon,
                    Category = upload.Categ,
                    Priority = upload.Priority
                };

                if (amenity != null)
                {
                    await _context.Amenities.AddAsync(amenity);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, amenity);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Gets all amenities for a specific room
        //[Authorize(Roles = "Admin,System,Recept")]  // Authorization commented out
        [HttpGet("GetAmenitiesForRoom/{Id}")]
        public async Task<ActionResult<IEnumerable<Amenity>>> GetResultsForRunner(int Id)
        {
            try
            {
                // Query amenities for the specified room
                var data = await _context.Amenities.Where(x => x.RoomId == Id).ToListAsync();

                if (!data.Any())
                {
                    return NotFound(new { message = "Elfelejtetted feltölteni ezt a szekciót!" });  // "You forgot to upload this section!"
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Deletes an amenity by its ID
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpDelete("DeleteAmenity/{id}")]
        public async Task<ActionResult<Amenity>> Deleteamenity(int id)
        {
            try
            {
                // Find and remove the amenity
                var data = await _context.Amenities.FirstOrDefaultAsync(x => x.AmenityId == id);
                if (data != null)
                {
                    _context.Amenities.Remove(data);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Sikeres törlés");  // "Successful deletion"
                }
                return StatusCode(404, "Ez az adat nem található az adatbázisban!");  // "This data is not found in the database!"
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Updates an existing amenity
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPut("UpdateAmenity/{id}")]
        public async Task<ActionResult<Amenity>> UpdateAmenity(int id, UpdateAmenity udto)
        {
            try
            {
                // Find and update the amenity with new values
                var data = await _context.Amenities.FirstOrDefaultAsync(x => x.AmenityId == id);
                if (data != null)
                {
                    data.AmenityName = udto.AmenityName;
                    data.Description = udto.Descript;
                    data.Availability = udto.Availability;
                    data.Status = udto.Status;
                    data.Icon = udto.Icon;
                    data.Category = udto.Category;
                    data.Priority = udto.Priority;

                    _context.Amenities.Update(data);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Sikeres frissítés");  // "Successful update"
                }
                return StatusCode(404, "Nem található");  // "Not found"
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}