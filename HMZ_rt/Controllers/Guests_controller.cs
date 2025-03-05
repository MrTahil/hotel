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
                        Useraccount test = _context.Useraccounts.FirstOrDefaultAsync(x => x.UserId == crtdto.UserId).Result;
                        if (_context.Useraccounts.Contains(test))
                        {
                            await _context.Guests.AddAsync(guestss);
                            await _context.SaveChangesAsync();
                            return StatusCode(201, "Sikeres vendég mentés!");
                        }return StatusCode(404, "Nem található felhasználó");
                        
                    }
                    return StatusCode(400, "Ez a vendég már hozzá van csatolva a profilhoz");
                }
                return BadRequest();
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
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

        [Authorize(Roles = "Base,Admin,Recept,System")]
        [HttpPut("UpdateGuest/{id}")]
        public async Task<ActionResult<Guest>> UpdateGuest(int id,UpdateGuest udto) {
            try
            {
            var someone= await _context.Guests.FirstOrDefaultAsync(x=> x.GuestId == id);
            if (someone != null) { 
                someone.Address = udto.Address;
                someone.Email = udto.Email;
                someone.PhoneNumber = udto.PhoneNumber;
                someone.LastName = udto.LastName;
                    someone.FirstName = udto.FirstName;
                    someone.City = udto.City;
                    someone.Country = udto.Country;
                    someone.DateOfBirth = udto.DateOfBirth;
                    someone.Gender = udto.Gender;
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Sikeres frissítés");
                }
                return StatusCode(404, "Nem található, vagy hibás json");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }

        }


        [Authorize(Roles = "Admin,System,Recept")]
        [HttpGet("GetcurrentGuests")]
        public async Task<ActionResult<Guest>> Getpplcurrentlyhere()
        {
            try
            {
                return BadRequest();
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }

    }
}
