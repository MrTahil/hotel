using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Admin,System,Recept")]
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

        [Authorize(Roles = "Admin,System,Recept")]
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

        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPut("UpdateAmenity/{id}")]
        public async Task<ActionResult<Amenity>> UpdateAmenity(int id, UpdateAmenity udto)
        {
            try
            {
                var adat = await _context.Amenities.FirstOrDefaultAsync(x => x.AmenityId == id);
                if (adat != null)
                {
                    adat.AmenityName = udto.AmenityName;
                    adat.Description = udto.Descript;
                    adat.Availability = udto.Availability;
                    adat.Status = udto.Status;
                    adat.Icon = udto.Icon;
                    adat.Category = udto.Category;
                    adat.Priority = udto.Priority;
                    _context.Amenities.Update(adat);
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
