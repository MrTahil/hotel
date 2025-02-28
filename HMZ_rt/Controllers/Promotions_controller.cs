using HMZ_rt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMZ_rt.Controllers
{
    [Route("Promotions")]
    [ApiController]
    public class Promotions_controller : Controller
    {
        private readonly HmzRtContext _context;
        public Promotions_controller(HmzRtContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<Promotion>> getpromotions()
        {
            try
            {
                var promotions = await _context.Promotions.ToListAsync();
                return StatusCode(201, promotions);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }
    }
}
