using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMZ_rt.Controllers
{
    [Route("Guests")]
    [ApiController]
    public class Guests_controller : ControllerBase
    {
        private readonly HmzRtContext _context;
        public Guests_controller(HmzRtContext context)
        {
            _context = context;
        }


        [Authorize(Roles = "Base,Admin,Recept,System")]
        [HttpPost("Addnewguest")]
        public async Task<ActionResult<Guest>> Createguest(CreateGuest crtdto)
        {
            try
            {
                var guestss = new Guest
                {
                    FirstName = crtdto.FirstName,
                    LastName = crtdto.LastName,
                    Email = crtdto.Email,
                    PhoneNumber = crtdto.PhoneNumber,
                    Address = crtdto.Address,
                    City = crtdto.City,
                    Country = crtdto.Country,
                    DateOfBirth = crtdto.DateOfBirth,
                    Gender = crtdto.Gender,
                    UserId = crtdto.UserId
                };
                if (guestss != null)
                {
                    if (!_context.Guests.Contains(guestss))
                    {
                        await _context.Guests.AddAsync(guestss);
                        await _context.SaveChangesAsync();
                        return StatusCode(201, "Sikeres vendég mentés!");
                    }
                    return StatusCode(400, "Ez a vendég már hozzá van csatolva a profilhoz");
                }
                return BadRequest();
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }


        [Authorize(Roles = "Admin,Recept,System")]
        [HttpGet("GetAllGuestever")]
        public async Task<ActionResult<Guest>> Getallguestsinsystem()
        {
            try
            {
            var all =await _context.Guests.Include(x => x.Bookings).Include(a => a.Feedbacks).Include(z => z.Reviews).ToListAsync();
            return StatusCode(201, all);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }
    }
}
