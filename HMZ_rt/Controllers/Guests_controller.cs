using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

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

        /// Creates a new guest in the system with age validation.
        /// <param name="crtdto">The guest details to create.</param>
        /// <returns>Success message or error if creation fails.</returns>
        [Authorize(Roles = "Base,Admin,Recept,System")]
        [HttpPost("Addnewguest")]
        public async Task<ActionResult<Guest>> Createguest(CreateGuest crtdto)
        {
            try
            {
                // Validate birth date is provided
                if (!crtdto.DateOfBirth.HasValue)
                {
                    return StatusCode(400, "Date of birth is required.");
                }

                // Calculate age and validate minimum age requirement
                var today = DateTime.Today;
                var birthDate = crtdto.DateOfBirth.Value;
                var age = today.Year - birthDate.Year;

                if (birthDate > today.AddYears(-age))
                {
                    age--;
                }

                if (age < 17)
                {
                    return StatusCode(400, "Guests under 17 years old cannot be added!");
                }

                // Create new guest object
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

                if (!_context.Guests.Contains(guestss))
                {
                    // Verify user account exists
                    Useraccount test = await _context.Useraccounts.FirstOrDefaultAsync(x => x.UserId == crtdto.UserId);
                    if (test != null)
                    {
                        await _context.Guests.AddAsync(guestss);
                        await _context.SaveChangesAsync();
                        return StatusCode(201, "Guest successfully saved!");
                    }
                    return StatusCode(404, "User not found");
                }
                return StatusCode(400, "This guest is already attached to the profile");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// Retrieves all guests with their related data (bookings, feedback, reviews).
        /// <returns>List of all guests with related data or error if retrieval fails.</returns>
        [Authorize(Roles = "Admin,Recept,System")]
        [HttpGet("GetAllGuestever")]
        public async Task<ActionResult<Guest>> Getallguestsinsystem()
        {
            try
            {
                var data = await _context.Guests
                    .Include(x => x.Bookings)
                    .Include(a => a.Feedbacks)
                    .Include(z => z.Reviews)
                    .ToListAsync();

                if (data != null && data.Any())
                {
                    return StatusCode(201, data);
                }
                return StatusCode(404, "Empty table");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Updates guest information by ID.
        /// <param name="id">The ID of the guest to update.</param>
        /// <param name="udto">The updated guest details.</param>
        /// <returns>Success message or error if update fails.</returns>
        [Authorize(Roles = "Base,Admin,Recept,System")]
        [HttpPut("UpdateGuest/{id}")]
        public async Task<ActionResult<Guest>> UpdateGuest(int id, UpdateGuest udto)
        {
            try
            {
                var someone = await _context.Guests.FirstOrDefaultAsync(x => x.GuestId == id);
                if (someone != null)
                {
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
                    return StatusCode(201, "Successfully updated");
                }
                return StatusCode(404, "Not found or invalid JSON");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Retrieves guests currently staying at the hotel.
        /// <returns>List of current guests or error if retrieval fails.</returns>
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpGet("GetcurrentGuests")]
        public async Task<ActionResult<Guest>> Getpplcurrentlyhere()
        {
            try
            {
                // Get guests with active bookings (checked in before yesterday and checking out after tomorrow)
                var ppl = _context.Guests
                    .Include(x => x.Bookings.Where(x =>
                        x.CheckInDate < DateTime.Now.AddDays(-1) &&
                        x.CheckOutDate > DateTime.Now.AddDays(1)));

                if (ppl != null)
                {
                    return StatusCode(201, ppl);
                }
                return StatusCode(400, "Unknown error, please ignore");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Deletes a guest by ID.
        /// <param name="id">The ID of the guest to delete.</param>
        /// <returns>Success message or error if deletion fails.</returns>
        [Authorize(Roles = "Admin,System,Recept,Base")]
        [HttpDelete("DeleteGuest/{id}")]
        public async Task<ActionResult<Guest>> DeleteGuest(int id)
        {
            try
            {
                var dat = await _context.Guests.FirstOrDefaultAsync(x => x.GuestId == id);
                if (dat != null)
                {
                    _context.Guests.Remove(dat);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Successfully deleted!");
                }
                return StatusCode(404, "Not found!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Retrieves guest data associated with a specific username.
        /// <param name="useranem">The username to look up.</param>
        /// <returns>Guest data or error if retrieval fails.</returns>
        [Authorize(Roles = "Admin,System,Recept,Base")]
        [HttpGet("GetGuestData/{useranem}")]
        public async Task<ActionResult<List<Guest>>> GetOneUsersGuests(string useranem)
        {
            try
            {
                var userid = await _context.Useraccounts.FirstOrDefaultAsync(x => x.Username == useranem);
                if (userid != null)
                {
                    var data = await _context.Guests.Where(x => x.UserId == userid.UserId).ToListAsync();
                    if (data.Any())
                    {
                        return StatusCode(200, data);
                    }
                    return StatusCode(404, "Nincs mentett vendég");
                }
                return StatusCode(404, "Nem található felhasználó");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
