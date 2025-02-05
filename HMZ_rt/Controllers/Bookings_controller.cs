using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMZ_rt.Controllers
{
    [Route("Bookings")]
    [ApiController]
    public class Bookings_controller : Controller
    {
        private readonly HmzRtContext _context;
        public Bookings_controller(HmzRtContext context)
        {
            _context = context;
        }

        [Authorize(Roles ="Base")]
        [HttpPost("New_Booking{RoomId}")]
        public async Task<ActionResult<Booking>> Booking(int roomid, CreateBookingDto crtbooking)
        {
            var booking = new Booking
            {
                CheckInDate = crtbooking.CheckInDate,
                CheckOutDate = crtbooking.CheckOutDate,
                GuestId = crtbooking.GuestId,
                RoomId = roomid,
                TotalPrice = crtbooking.TotalPrice,
                BookingDate = DateTime.Now,
                PaymentStatus = crtbooking.PaymentStatus,
                NumberOfGuests = crtbooking.NumberOfGuests
            };
            if (crtbooking != null) { 
            await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();
                return StatusCode(201, booking);
                
            }
            return BadRequest();
        }
        




    }
}
