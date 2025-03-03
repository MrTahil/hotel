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
        public Amenities_controller(HmzRtContext context)
        {
            _context = context;
        }



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


        [HttpGet("GetAmenitiesForRoom/{Id}")]

        public async Task<ActionResult<IEnumerable<Amenity>>> GetResultsForRunner(int Id)
        {
            try
            {


            var results = await _context.Amenities.Where(x => x.RoomId == Id).ToListAsync();

            if (!results.Any())
            {
                return NotFound(new { message = "Elfelejtetted feltölteni ezt a szekciót!" });
            }
            return Ok(results);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }
        [HttpDelete("DeleteAmenity/{id}")]
        public async Task<ActionResult<Amenity>> Deleteamenity(int id)
        {
            try
            {
                var deletee = await _context.Amenities.FirstOrDefaultAsync(x => x.AmenityId == id);
                if (deletee != null) { 
                _context.Amenities.Remove(deletee);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Sikeres törlés");
                } return StatusCode(404, "Ez az adat nem található az adatbázisban!");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }
    }
}
