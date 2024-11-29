using HMZ_rt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMZ_rt.Controllers
{
    [Route("Rooms")]
    [ApiController]
    public class Rooms_controller : Controller
    {
        private readonly HmzRtContext _context;

        [HttpGet("SzobaInfokFőképhez")]
        public async Task<ActionResult<Rooms_controller>> BasesiteofRooms()
        {
            return Ok( _context.Rooms.ToListAsync());
        }
    }
}
