using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMZ_rt.Controllers
{
    [Route("Staff")]
    [ApiController]
    public class Staff_controller : Controller
    {
        private readonly HmzRtContext _context;
        public Staff_controller(HmzRtContext context)
        {
            _context = context;
        }



        [Authorize(Roles = "System,Admin")]
        [HttpPost]
        public async Task<ActionResult<Staff>> CreateStaff(NewStaffDto nwstdto)
        {
            var dolgozo = new Staff
            {
                FirstName = nwstdto.FirstName,
                LastName = nwstdto.LastName,
                Email = nwstdto.Email,
                PhoneNumber = nwstdto.PhoneNumber,
                Position = nwstdto.Position,
                Salary = nwstdto.Salary,
                DateHired = DateTime.Now,
                Status = nwstdto.Status,
                Department = nwstdto.Departmen
            };
            if (nwstdto != null)
            {
                await _context.Staff.AddAsync(dolgozo);
                await _context.SaveChangesAsync();
                return StatusCode(201, dolgozo);
            }
            return BadRequest();
        }
    }
}
