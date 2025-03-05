using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
        private readonly IWebHostEnvironment _env;
        private readonly SmtpSettings _smtpSettings;

        public Bookings_controller(HmzRtContext context, IConfiguration configuration ,IWebHostEnvironment env,
            IOptions<SmtpSettings> smtpOptions)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _tokenService = new TokenService(configuration);
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _smtpSettings = smtpOptions.Value ?? throw new ArgumentNullException(nameof(smtpOptions));
        }


        private async Task SendBookingConfirmation(Booking booking, string recipientEmail, Guest guestdata, Room roomdata, Payment paymentdata)
        {
            try
            {


            string templatePath = Path.Combine(_env.ContentRootPath, "Html", "BookingConfirmed.html");
            string emailTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

            

            


            // Replace placeholders with actual data
            emailTemplate = emailTemplate.Replace("{CustomerName}", $"{guestdata.FirstName} {guestdata.LastName}")
                                        .Replace("{BookingReference}", Convert.ToString(booking.BookingId))
                                        .Replace("{BookingDateTime}", booking.BookingDate.ToString())
                                        .Replace("{Duration}", Convert.ToString(booking.CheckInDate - booking.CheckOutDate))
                                        .Replace("{Location}", $"{Convert.ToString( roomdata.RoomNumber)}-es számú szoba")
                                        .Replace("{Subtotal}", (booking.TotalPrice / Convert.ToDecimal(1.73)).ToString())
                                        .Replace("{Tax}", (booking.TotalPrice /Convert.ToDecimal(1.27)).ToString())
                                        .Replace("{Total}", booking.TotalPrice.ToString())
                                        .Replace("{PaymentMethod}", paymentdata.PaymentMethod)
                                        .Replace("{PaymentStatus}", booking.PaymentStatus)
                                        .Replace("{SupportEmail}", "hmzrtkando@gmail.com")
                                        .Replace("{SupportPhone}", "+1 (555) 123-4567")
                                        .Replace("{ManageBookingUrl}", $"https://yourcompany.com/bookings/{Convert.ToString(booking.BookingId)}")
                                        .Replace("{CurrentYear}", DateTime.Now.Year.ToString())
                                        .Replace("{CompanyName}", "HMZ RT")
                                        .Replace("{CompanyAddress}", "Palóczy László utca 3, Miskolc, BAZ, 3531")
                                        .Replace("{PrivacyPolicyUrl}", "https://yourcompany.com/privacy")
                                        .Replace("{TermsUrl}", "https://yourcompany.com/terms");

            // Handle additional items if any
            //string additionalItemsHtml = "";
            //foreach (var item in booking.AdditionalItems)
            //{
            //    additionalItemsHtml += $@"
            //    <tr>
            //        <td width=""70%"" style=""font-size: 16px; padding-top: 10px;"">{item.Name}</td>
            //        <td width=""30%"" style=""text-align: right; font-size: 16px; padding-top: 10px;"">{item.Price.ToString("C")}</td>
            //    </tr>";
            //}
            //emailTemplate = emailTemplate.Replace("{AdditionalItems}", additionalItemsHtml);

            // Create the email message
            MailMessage message = new MailMessage
            {
                From = new MailAddress(_smtpSettings.FromEmail),
                Subject = $"Booking Confirmation - {Convert.ToString(booking.BookingId)}",
                Body = emailTemplate,
                IsBodyHtml = true
            };
            message.To.Add(recipientEmail);

            // Send the email
            using (SmtpClient client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                client.Send(message);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
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
                var guests = _context.Guests.FirstOrDefaultAsync(x => x.GuestId == booking.GuestId);
                if (guests == null)
                {
                    return StatusCode(404, "Vendég megadása szükséges foglalás előtt");
                }
                var user = _context.Useraccounts.FirstOrDefaultAsync(x => x.UserId == guests.Result.UserId);

                var payment = new Payment
                {
                    BookingId = booking.BookingId,
                    PaymentDate = DateTime.MinValue,
                    Amount = 15000,
                    PaymentMethod ="Fizetésnél frissül",
                    TransactionId ="0",
                    Status ="Fizetetlen",
                    Currency = "Huf",
                    PaymentNotes ="Fizetésre vár",
                    DateAdded = DateTime.Now
                };
                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();
                Guest guestdata = _context.Guests.FirstOrDefaultAsync(x => x.GuestId == booking.GuestId).Result;
                Room roomdata = _context.Rooms.FirstOrDefaultAsync(x => x.RoomId == booking.RoomId).Result;
                Payment paymentdata = _context.Payments.FirstOrDefaultAsync(x => x.BookingId == booking.BookingId).Result;
                if (guestdata == null || roomdata == null || paymentdata ==null)
                {
                    return StatusCode(404, "Nincs létrehozott guest a felhasználóhoz, vagy nem létező szobára hivatkoztál");
                }

                await SendBookingConfirmation(booking, user.Result.Email, guestdata,roomdata,paymentdata);






                return StatusCode(201, "Sikeres foglalás");
                
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
