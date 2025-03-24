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

        /// Retrieves all event bookings for events that haven't ended yet.
        /// <returns>A list of event bookings or a 404 if no data is found.</returns>
        [Authorize(Roles = "System,Admin,Recept")]
        [HttpGet("GetEventBookings")]
        public async Task<ActionResult<Eventbooking>> Getalleventbooking()
        {
            try
            {
                // Get IDs of events that haven't ended (after yesterday)
                var param = await _context.Events
                    .Where(x => x.EventDate > DateTime.Now.AddDays(-1))
                    .Select(x => x.EventId)
                    .ToListAsync();

                var paramHashSet = new HashSet<int>(param);

                // Retrieve bookings for active events
                var data = await _context.Eventbookings
                    .Where(x => paramHashSet.Contains(x.EventId))
                    .ToListAsync();

                if (data != null && data.Any())
                {
                    return StatusCode(200, data);
                }

                return StatusCode(404, "No data found in the table");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Deletes a booking by its ID.
        /// <param name="id">The ID of the booking to delete.</param>
        /// <returns>A success message or a 404 if the booking is not found.</returns>
        [Authorize(Roles = "Base,System,Admin,Recept")]
        [HttpDelete("DeleteBookingById/{id}")]
        public async Task<ActionResult<Eventbooking>> DeleteBooking(int id)
        {
            try
            {
                var data = await _context.Eventbookings.FirstOrDefaultAsync(x => x.EventBookingId == id);
                if (data != null)
                {
                    _context.Eventbookings.Remove(data);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, "Successful deletion");
                }

                return StatusCode(404, "No data found for this ID");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Creates a new event booking.
        /// <param name="crtdto">The booking details.</param>
        /// <param name="eventid">The ID of the event to book.</param>
        /// <returns>A success message or an error if the booking cannot be created.</returns>
        [Authorize(Roles = "Base,System,Admin,Recept")]
        [HttpPost("NewEvenBooking/{eventid}")]
        public async Task<ActionResult<Eventbooking>> CreateBooking(CreateEventBooking crtdto, int eventid)
        {
            try
            {
                // Check if the event exists and hasn't ended
                var eventdata = _context.Events.FirstOrDefault(x => x.EventId == eventid && x.EventDate > DateTime.Now.AddDays(-1));
                if (eventdata == null)
                {
                    return StatusCode(404, "This event doesn't exist or has already ended.");
                }

                // Verify the guest exists
                var guestdata = await _context.Guests.FirstOrDefaultAsync(x => x.GuestId == crtdto.GuestId);
                if (guestdata == null)
                {
                    return StatusCode(404, "This guest ID doesn't exist");
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

                _context.Eventbookings.Add(data);
                await _context.SaveChangesAsync();
                return StatusCode(201, "Booking successful");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
