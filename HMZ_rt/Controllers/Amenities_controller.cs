using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
#pragma warning disable
namespace HMZ_rt.Controllers
{
    [Route("Amenities")]
    [ApiController]
    public class Amenities_controller : Controller
    {
        private readonly HmzRtContext _context;
        public Amenities_controller(HmzRtContext context)
        {
            _context = context;
        }


        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPost("New aminiti")]
        public async Task<ActionResult<Amenity>> GenerateNewAmenitie( UploadAmenitiesForNewRoomDto upload)
        {
            try
            {


                //mentés
                var amenity = new Amenity
                {
                    AmenityName = upload.AmenityName,
                    Description = upload.Descript,
                    Availability = upload.Availability,
                    RoomId = upload.RoomId,
                    Status = upload.Status,
                    Icon = upload.Icon,
                    Category= upload.Categ,
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

        //[Authorize(Roles = "Admin,System,Recept")]
        [HttpGet("GetAmenitiesForRoom/{Id}")]

        public async Task<ActionResult<IEnumerable<Amenity>>> GetResultsForRunner(int Id)
        {
            try
            {


            var data = await _context.Amenities.Where(x => x.RoomId == Id).ToListAsync();

            if (!data.Any())
            {
                return NotFound(new { message = "Elfelejtetted feltölteni ezt a szekciót!" });
            }
            return Ok(data);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }

        [Authorize(Roles = "Admin,System,Recept")]
        [HttpDelete("DeleteAmenity/{id}")]
        public async Task<ActionResult<Amenity>> Deleteamenity(int id)
        {
            try
            {
                var data = await _context.Amenities.FirstOrDefaultAsync(x => x.AmenityId == id);
                if (data != null) { 
                _context.Amenities.Remove(data);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Sikeres törlés");
                } return StatusCode(404, "Ez az adat nem található az adatbázisban!");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }

        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPut("UpdateAmenity/{id}")]
        public async Task<ActionResult<Amenity>> UpdateAmenity(int id, UpdateAmenity udto)
        {
            try
            {
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
                    return StatusCode(201, "Sikeres frissítés");
                    
                }
                return StatusCode(404, "Nem található");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }

    }
}
