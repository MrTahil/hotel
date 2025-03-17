using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMZ_rt.Controllers
{
    [Route("Feedback")]
    [ApiController]
    public class EventBookings_controller : Controller
    {
        private readonly HmzRtContext _context;
        public EventBookings_controller(HmzRtContext context)
        {
            _context = context;
        }


        [Authorize(Roles = "System,Admin,Recept")]
        [HttpGet("GetEventBookings")]
        public async Task<ActionResult<Eventbooking>> Getalleventbooking()
        {
            try
            {
                var adat = await _context.Eventbookings.ToListAsync();
                if (adat != null)
                {
                    return StatusCode(200, adat);
                }
                return StatusCode(404, "Nincs adat a táblában");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex); 
            }
        }

        [Authorize(Roles = "Base,System,Admin,Recept")]
        [HttpDelete("DeleteBookingById/{id}")]
        public async Task<ActionResult<Eventbooking>> DeleteBooking(int id)
        {
            try
            {
                
                var data = await _context.Eventbookings.FirstOrDefaultAsync(x => x.EventBookingId == id);
                if (data != null) { 
                _context.Eventbookings.Remove(data);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, "Sikeres törlés");
                }
                return StatusCode(404, "Nem található ezen az Id-n adat");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }


        [Authorize(Roles = "Base,System,Admin,Recept")]
        [HttpPost("NewEvenBooking/{eventid}")]
        public async Task<ActionResult<Eventbooking>> CreateBooking(CreateEventBooking crtdto, int eventid)
        {
            try
            {
                var eventdata = _context.Events.FirstOrDefault(x => x.EventId == eventid);
                if (eventdata == null) {
                    return StatusCode(404, "Ez az event Id nem létezik");
                }
                var guestdata = _context.Guests.FirstOrDefaultAsync(x => x.GuestId == crtdto.GuestId);
                if (guestdata == null) {
                    return StatusCode(404, "Ez a vendég Id nem létezik");
                }

                var data = new Eventbooking
                {
                    EventId = eventid,
                    GuestId = crtdto.GuestId,
                    BookingDate = DateTime.Now,
                    NumberOfTickets = crtdto.NumberOfTickets,
                    TotalPrice = eventdata.Price * crtdto.NumberOfTickets,
                    Status = crtdto.Status,
                    PaymentStatus = crtdto.PaymentStatus,
                    DateAdded = DateTime.Now,
                    Notes = crtdto.Notes
                };
                if (data !=null)
                {
                    _context.Eventbookings.Add(data);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Sikeres Foglalás");
                } return StatusCode(404, "Something went wrong");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }

        }

    }
}
