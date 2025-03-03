using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMZ_rt.Controllers
{
    [Route("Events")]
    [ApiController]
    public class Events_controller : Controller
    {
        private readonly HmzRtContext _context;
        public Events_controller(HmzRtContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin, System,Admin")]
        [HttpGet("Geteventsadmin")]
        public async Task<ActionResult<Event>> Getallevent()
        {
            try
            {
                var events = await _context.Events.Include(x => x.Eventbookings).ToListAsync();
                if (events != null) {
                    return StatusCode(201, events);

                }
                return StatusCode(404, "üres");

            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }
    }
}
