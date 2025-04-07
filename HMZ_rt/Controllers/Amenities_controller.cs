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

        private async Task<ActionResult> HandleError(Exception ex)
        {
            return StatusCode(500, new { message = "Belső szerver hiba", ex.Message });
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
                if (upload.RoomId <= 0)
                {
                    return BadRequest(new { message = "Érvénytelen szoba azonosító!" });
                }

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
                return await HandleError(ex);
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
                return await HandleError(ex);
            }
        }

        /// <summary>
        /// Deletes an amenity by its identifier
        /// </summary>
        /// <param name="id">Amenity identifier</param>
        /// <returns>Deletion result</returns>
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpDelete("DeleteAmenity/{id}")]
        public async Task<ActionResult> DeleteAmenity(int id)
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
                return await HandleError(ex);
            }
        }

        /// <summary>
        /// Updates an existing amenity
        /// </summary>
        /// <param name="id">Amenity identifier</param>
        /// <param name="updateDto">Update data transfer object</param>
        /// <returns>Update result</returns>
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPut("UpdateAmenity/{id}")]
        public async Task<ActionResult> UpdateAmenity(int id, UpdateAmenity updateDto)
        {
            try
            {
                var data = await _context.Amenities
                    .FirstOrDefaultAsync(x => x.AmenityId == id);

                if (data == null)
                {
                    return StatusCode(404, new { message = "Az adat nem található!" });
                }

                data.AmenityName = updateDto.AmenityName;
                data.Description = updateDto.Descript;
                data.Availability = updateDto.Availability;
                data.Status = updateDto.Status;
                data.Icon = updateDto.Icon;
                data.Category = updateDto.Category;
                data.Priority = updateDto.Priority;

                _context.Amenities.Update(data);
                await _context.SaveChangesAsync();

                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return await HandleError(ex);
            }
        }
    }
}
