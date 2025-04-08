using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
#pragma warning disable
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
                if (!crtdto.DateOfBirth.HasValue)
                {
                    return StatusCode(400, "A születési dátum megadása kötelező.");
                }

                var today = DateTime.Today;
                var birthDate = crtdto.DateOfBirth.Value;
                var age = today.Year - birthDate.Year;


                if (birthDate > today.AddYears(-age))
                {
                    age--;
                }

                if (age < 17)
                {
                    return StatusCode(400, "17 évnél fiatalabb vendéget nem lehet hozzáadni!");
                }


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
                        Useraccount test =await _context.Useraccounts.FirstOrDefaultAsync(x => x.UserId == crtdto.UserId);
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
            var data =await _context.Guests.Include(x => x.Bookings).Include(a => a.Feedbacks).Include(z => z.Reviews).ToListAsync();
                if (data != null)
                {
                    return StatusCode(200, data);
                } return StatusCode(404, "Üres tábla");
            
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
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

                return StatusCode(500, ex.Message);
            }

        }


        [Authorize(Roles = "Admin,System,Recept")]
        [HttpGet("GetcurrentGuests")]
        public async Task<ActionResult<Guest>> Getpplcurrentlyhere()
        {
            try
            {
                var ppl = _context.Guests.Include(x => x.Bookings.Where(x => x.CheckInDate < DateTime.Now.AddDays(-1) && x.CheckOutDate > DateTime.Now.AddDays(1)));
                
                if (ppl != null)
                {
                    return StatusCode(200, ppl);
                }
                return StatusCode(400, "Én se tudom mi a baja engedd el");




            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin,System,Recept,Base")]
        [HttpDelete("DeleteGuest/{id}")]
        public async Task<ActionResult<Guest>> DeleteGuest(int id)
        {
            try
            {
                var dat = await _context.Guests.FirstOrDefaultAsync(x => x.GuestId == id);
                if (dat != null) { 
                _context.Guests.Remove(dat);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Sikeres törlés!");
                }
                return StatusCode(404, "Nem található!");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
            

        }

        [Authorize(Roles = "Admin,System,Recept,Base")]
        [HttpGet("GetGuestData/{useranem}")]
        public async Task<ActionResult<Guest>> GetOneUsersGuests(string useranem)
        {
            try
            {
                var userid = await _context.Useraccounts.FirstOrDefaultAsync(x => x.Username == useranem);
                if (userid != null)
                {


                    var data = await _context.Guests.Where(x => x.UserId == userid.UserId).ToListAsync();
                    if (data != null)
                    {
                        return StatusCode(200, data);
                    }
                    return StatusCode(404, "Nincs mentett vendég");
                } return StatusCode(404, "nem található felhasználó");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin,System,Recept")]
        [HttpGet("GetByEmail/{email}")]
        public async Task<ActionResult<Guest>> GetOneUserGuestEmail(string email)
        {
            try
            {
                var data = await _context.Guests.FirstOrDefaultAsync(x => x.Email == email);
                if (data != null)
                {
                    return StatusCode(200, data.GuestId);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
    }
}
