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

                TimeSpan duration = TimeSpan.Zero;
                if (booking.CheckOutDate.HasValue && booking.CheckInDate.HasValue)
                {
                    duration = booking.CheckOutDate.Value - booking.CheckInDate.Value;
                }


                emailTemplate = emailTemplate.Replace("{CustomerName}", $"{guestdata.FirstName} {guestdata.LastName}")
                                             .Replace("{BookingReference}", Convert.ToString(booking.BookingId))
                                             .Replace("{BookingDateTime}", booking.BookingDate.ToString())
                                             .Replace("{ServiceName}", "Szobafoglalás")
                                             .Replace("{Duration}", $"{duration.TotalDays.ToString()} nap")
                                             .Replace("{Location}", $"{Convert.ToString(roomdata.RoomNumber)}-es számú szoba")
                                             .Replace("{Subtotal}",Math.Round( Convert.ToDecimal(booking.TotalPrice) * Convert.ToDecimal(0.73)).ToString())
                                             .Replace("{Tax}",  Math.Round(Convert.ToDecimal( booking.TotalPrice) * Convert.ToDecimal(0.27)).ToString())
                                             .Replace("{Total}",Math.Round (Convert.ToDecimal( booking.TotalPrice)).ToString())
                                             .Replace("{PaymentMethod}", paymentdata.PaymentMethod)
                                             .Replace("{PaymentStatus}", booking.PaymentStatus)
                                             .Replace("{SupportEmail}", "hmzrtkando@gmail.com")
                                             .Replace("{SupportPhone}", "+36 (70) 123-4567")
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

                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress(_smtpSettings.FromEmail);
                    message.Subject = $"Foglalás megerősítése - {Convert.ToString(booking.BookingId)}";
                    message.Body = emailTemplate;
                    message.IsBodyHtml = true;
                    message.To.Add(recipientEmail);

                    
                    using (SmtpClient client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
                    {
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                        await client.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine(ex);
            }
        }

















        [Authorize(Roles = "Base,Admin,System")]
        [HttpPost("New_Booking/{roomid}")]
        public async Task<ActionResult<Booking>> Booking(int roomid, CreateBookingDto crtbooking)
        {
            if (crtbooking == null)
            {
                return BadRequest();
            }
            var roomData = await _context.Rooms.FirstOrDefaultAsync(x => x.RoomId == roomid);
            if (roomData == null)
            {
                return StatusCode(404, "Nem létező szobára hivatkoztál");
            }
            var guestData = await _context.Guests.FirstOrDefaultAsync(x => x.GuestId == crtbooking.GuestId);
            if (guestData == null)
            {
                return StatusCode(404, "Vendég megadása szükséges foglalás előtt");
            }
            var user = await _context.Useraccounts.FirstOrDefaultAsync(x => x.UserId == guestData.UserId);
            if (user == null)
            {
                return StatusCode(404, "Nem található felhasználó a vendéghez");
            }
                
            var existingBooking = await _context.Bookings
                .AnyAsync(b => b.RoomId == roomid &&
                              ((crtbooking.CheckInDate >= b.CheckInDate && crtbooking.CheckInDate < b.CheckOutDate) ||
                               (crtbooking.CheckOutDate > b.CheckInDate && crtbooking.CheckOutDate <= b.CheckOutDate) ||
                               (crtbooking.CheckInDate <= b.CheckInDate && crtbooking.CheckOutDate >= b.CheckOutDate)));

            if (existingBooking)
            {
                return StatusCode(409, "A szoba foglalt a kiválasztott időszakra");
            }


            var booking = new Booking
            {
                CheckInDate = crtbooking.CheckInDate,
                CheckOutDate = crtbooking.CheckOutDate,
                GuestId = crtbooking.GuestId,
                RoomId = roomid,
                TotalPrice = roomData.PricePerNight * crtbooking.NumberOfGuests, //without coc
                BookingDate = DateTime.Now,
                PaymentStatus = "Fizetésre vár",
                NumberOfGuests = crtbooking.NumberOfGuests,
                Status = "Jóváhagyva"
            };

            if (roomData.Capacity < crtbooking.NumberOfGuests)
            {
                return StatusCode(400, "Túl sok fővel próbáltál foglalni");
            }
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            var payment = new Payment
            {
                BookingId = booking.BookingId,
                PaymentDate = DateTime.MinValue,
                Amount = booking.TotalPrice,
                PaymentMethod = crtbooking.PaymentMethod,
                TransactionId = "0",
                Status = "Fizetésre vár",
                Currency = "Huf",
                PaymentNotes = "",
                DateAdded = DateTime.Now
            };
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            await SendBookingConfirmation(booking, user.Email, guestData, roomData, payment);

            return StatusCode(201, "Sikeres foglalás");
        }





        [Authorize(Roles ="Base,Admin,System,Recept")]
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
        [Authorize(Roles = "Base,Admin,System,Recept")]
        [HttpDelete("DeleteBooking/{id}")]
        public async Task<ActionResult<Booking>> DeleteBooking(int id)
        {
            try
            {
                var books =await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == id);
                if (books != null)
                {
                    _context.Bookings.Remove(books);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Sikeres törlés");
                } return StatusCode(404, "Nem található a foglalás");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }
        [Authorize(Roles = "Base,Admin,System,Recept")]
        [HttpPut("UpdateBooking/{id}")]
        public async Task<ActionResult<Booking>> UpdateBookingByid(int id, UpdateBooking udto)
        {
            try
            {
                var updatedbook =await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == id);
                if (updatedbook != null)
                {
                    updatedbook.CheckInDate = udto.CheckInDate;
                    updatedbook.CheckOutDate = udto.CheckOutDate;
                    updatedbook.NumberOfGuests = udto.NumberOfGuests;
                }
                else
                {
                    return BadRequest();
                }
                 _context.Bookings.Update(updatedbook);
                await _context.SaveChangesAsync();
                return StatusCode(201, "Sikeres mentés");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }

        [Authorize(Roles = "Base,Admin,System,Recept")]
        [HttpGet("Getalldat")]
        public async Task<ActionResult<Booking>> GetallData()
        {
            try
            {


            var data = await _context.Bookings.ToListAsync();
            if (data!= null)
            {
                return StatusCode(200, data);
            }
                return StatusCode(404, "Nincs adat a táblában");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }





        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPut("UpdateBookingStatus/{id}")]
        public async Task<ActionResult<Booking>> UpdateBookingStatusByid(int id, UpdateBookingStatus udto)
        {
            try
            {
                var updatedbook = await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == id);
                if (updatedbook != null)
                {
                    if (updatedbook.PaymentStatus.Trim().ToLower() == "fizetve" && udto.Status.Trim().ToLower() == "finished")
                    {
                        updatedbook.Status = udto.Status;
                    }
                    if (updatedbook.PaymentStatus.Trim().ToLower() != "fizetve" && udto.Status.Trim().ToLower() == "finished")
                    {
                        return StatusCode(400, "Még nincs kifizetve, addig nem lezárható");
                    }

                    if (updatedbook.Status.Trim().ToLower() != "finished")
                    {
                        updatedbook.Status = udto.Status;
                    }
                }
                else
                {
                    return BadRequest();
                }
                _context.Bookings.Update(updatedbook);
                await _context.SaveChangesAsync();
                return StatusCode(201, "Sikeres mentés");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }

    }
}
