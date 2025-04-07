using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

#pragma warning disable

namespace HMZ_rt.Controllers
{
    [Route("Amenities")]
    [ApiController]
    public class AmenitiesController : ControllerBase
    {
        private readonly HmzRtContext _context;

        public AmenitiesController(HmzRtContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new amenity for a room
        /// </summary>
        /// <param name="upload">Amenity data to create</param>
        /// <returns>Create amenity response</returns>
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPost("NewAmenity")]
        public async Task<ActionResult<Amenity>> GenerateNewAmenity(UploadAmenitiesForNewRoomDto upload)
        {
            try
            {
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

                await _context.Amenities.AddAsync(amenity);
                await _context.SaveChangesAsync();

                return StatusCode(201, amenity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", ex.Message });
            }
        }

        /// <summary>
        /// Gets all amenities for a specific room
        /// </summary>
        /// <param name="id">Room identifier</param>
        /// <returns>Amenities for the room</returns>
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpGet("GetAmenitiesForRoom/{id}")]
        public async Task<ActionResult<IEnumerable<Amenity>>> GetAmenitiesForRoom(int id)
        {
            try
            {
                var data = await _context.Amenities
                    .Where(x => x.RoomId == id)
                    .ToListAsync();

                if (!data.Any())
                {
                    return NotFound(new { message = "Nincsenek szolgáltatások ezen a szobához!" });
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", ex.Message });
            }
        }

        /// <summary>
        /// Deletes an amenity by its identifier
        /// </summary>
        /// <param name="id">Amenity identifier</param>
        /// <returns>Deletion result</returns>
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpDelete("DeleteAmenity/{id}")]
        public async Task<ActionResult<Amenity>> DeleteAmenity(int id)
        {
            try
            {
                var data = await _context.Amenities
                    .FirstOrDefaultAsync(x => x.AmenityId == id);

                if (data != null)
                {
                    _context.Amenities.Remove(data);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, new { message = "Sikeres törlés" });
                }

                return StatusCode(404, new { message = "Az adat nem található!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing amenity
        /// </summary>
        /// <param name="id">Amenity identifier</param>
        /// <param name="udto">Update data transfer object</param>
        /// <returns>Update result</returns>
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPut("UpdateAmenity/{id}")]
        public async Task<ActionResult<Amenity>> UpdateAmenity(int id, UpdateAmenity udto)
        {
            try
            {
                var data = await _context.Amenities
                    .FirstOrDefaultAsync(x => x.AmenityId == id);

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

                    return StatusCode(200, new { message = "Sikeres frissítés" });
                }

                return StatusCode(404, new { message = "Az adat nem található!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", ex.Message });
            }
        }
    }
}
