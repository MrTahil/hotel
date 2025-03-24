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


        /// Retrieves all maintenance records from the database.
        /// <returns>A list of all maintenance records.</returns>
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpGet("Getmaintance")]
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

        /// Creates a new maintenance request.
        /// <param name="dto">The maintenance request details.</param>
        /// <returns>Success message or error if creation fails.</returns>
        [Authorize(Roles = "Base,Admin,System,Recept")]
        [HttpPost("MakeARequest")]
        public async Task<ActionResult<Roommaintenance>> Makeareq(kacsa dto)
        {
            try
            {
                var data = new Roommaintenance
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

                // Validate maintenance date is not in the past
                if (data.MaintenanceDate < DateTime.Now.AddDays(1))
                {
                    return BadRequest("Date cannot be earlier than today");
                }

                await _context.Roommaintenances.AddAsync(data);
                await _context.SaveChangesAsync();
                return StatusCode(201, "Request submitted successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex });
            }
        }

        /// Deletes a maintenance request by ID.
        /// <param name="id">The ID of the maintenance request to delete.</param>
        /// <returns>Success message or error if deletion fails.</returns>
        [Authorize(Roles = "Base,Admin,System,Recept")]
        [HttpDelete("DeleteRequest/{id}")]
        public async Task<ActionResult<Roommaintenance>> Deleteareq(int id)
        {
            try
            {
                var data = await _context.Roommaintenances.FirstOrDefaultAsync(x => x.MaintenanceId == id);
                if (data != null)
                {
                    _context.Roommaintenances.Remove(data);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Successfully deleted");
                }
                return StatusCode(404, "Request not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex });
            }
        }

        /// Updates a maintenance request with resolution details.
        /// <param name="id">The ID of the maintenance request to update.</param>
        /// <param name="udto">The updated maintenance details including resolution information.</param>
        /// <returns>Success message or error if update fails.</returns>
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPut("UpdateRequestByManagger/{id}")]
        public async Task<ActionResult<Roommaintenance>> Updatereq(int id, MaintanceUpdate udto)
        {
            try
            {
                var data = _context.Roommaintenances.FirstOrDefault(x => x.MaintenanceId == id);
                if (data != null)
                {
                    data.ResolutionDate = udto.ResolutionDate;
                    data.Status = udto.Status;
                    data.Cost = udto.Cost;
                    data.Notes = udto.Notes;
                    data.StaffId = udto.StaffId;

                    _context.Roommaintenances.Update(data);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Successfully updated");
                }

                return StatusCode(404, "No service request found with this ID");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex });
            }
        }
    }
}
