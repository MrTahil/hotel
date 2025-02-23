using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMZ_rt.Controllers
{
    [Route("Maintenance")]
    [ApiController]
    public class RoomMaintenance_controller : Controller
    {


        private readonly HmzRtContext _context;
        public RoomMaintenance_controller(HmzRtContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpGet]
        public async Task<ActionResult<Roommaintenance>> GetMain()
        {
            try
            {


                return StatusCode(201, await _context.Roommaintenances.ToListAsync());
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { ex });
            }
        }

        [Authorize(Roles = "Base,Admin,System,Recept")]
        [HttpPost("MakeARequest")]
        public async Task<ActionResult<Roommaintenance>> Makeareq(kacsa dto)
        {
            try
            {
                var valami = new Roommaintenance
                {
                    MaintenanceDate = dto.MaintenanceDate,
                    Description = dto.Description,
                    Status = "Not resolved yet",
                    DateReported = DateTime.Now,
                    ResolutionDate = DateTime.MinValue,
                    Cost = 0,
                    Notes = dto.Notes,
                    RoomId = dto.RoomId

                };
                if (valami != null)
                {
                    await _context.Roommaintenances.AddAsync(valami);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Sikeres kérvény leadás!");
                }
                return StatusCode(404, "Ha ide eljutsz rég baj van igazából");


            }
            catch (Exception ex)
            {

                return StatusCode(500, new { ex });
            }
        }


        [Authorize(Roles = "Base,Admin,System,Recept")]
        [HttpDelete("DeleteRequest/{id}")]
        public async Task<ActionResult<Roommaintenance>> Deleteareq(int id)
        {
            try
            {
                var valami = await _context.Roommaintenances.FirstOrDefaultAsync(x => x.MaintenanceId == id);
                if (valami != null)
                {
                    _context.Roommaintenances.Remove(valami);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Sikeres törlés");
                }
                return StatusCode(404, "Nem található a kérelem");
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { ex });
            }
        }


        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPut("UpdateRequestByManagger/{id}")]
        public async Task<ActionResult<Roommaintenance>> Updatereq(int id)
        {
            try
            {
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex });
                throw;
            }
        }
    }
        
}
