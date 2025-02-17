using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static HMZ_rt.Controllers.UserAccounts_controller;

namespace HMZ_rt.Controllers
{
    [Route("Bookings")]
    [ApiController]
    public class Bookings_controller : Controller
    {
        private readonly HmzRtContext _context;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;

        public Bookings_controller(HmzRtContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _tokenService = new TokenService(configuration);
        }



        [Authorize(Roles ="Base,Admin,System")]
        [HttpPost("New_Booking")]
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

        [HttpGet("BookingsByUserId")]
        public async Task<ActionResult<Booking>> GetUserBookings(int UserIdd)
        {
            var igen =await  _context.Guests.FirstOrDefaultAsync(x => x.UserId == UserIdd);
            var foglalasok =  _context.Bookings.Where(x => x.GuestId == igen.GuestId).Include(x => x.Payments);
            if (igen != null) {
                return StatusCode(201, foglalasok);
            }
            return BadRequest();


        }



    }
}
