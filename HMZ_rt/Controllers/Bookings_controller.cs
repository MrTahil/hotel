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
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Net.Mime;

namespace HMZ_rt.Controllers
{
    /// Controller for managing hotel room bookings
    [Route("Bookings")]
    [ApiController]
    public class Bookings_controller : Controller
    {
        private readonly HmzRtContext _context;               // Database context
        private readonly IConfiguration _configuration;        // App configuration
        private readonly TokenService _tokenService;           // For JWT token handling
        private readonly IWebHostEnvironment _env;             // Environment info for file paths
        private readonly SmtpSettings _smtpSettings;           // Email settings

        public Bookings_controller(HmzRtContext context, IConfiguration configuration, IWebHostEnvironment env,
            IOptions<SmtpSettings> smtpOptions)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _tokenService = new TokenService(configuration);
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _smtpSettings = smtpOptions.Value ?? throw new ArgumentNullException(nameof(smtpOptions));
        }

        /// Sends booking confirmation email with invoice PDF attachment
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
                                             .Replace("{Subtotal}", Math.Round(Convert.ToDecimal(booking.TotalPrice) * Convert.ToDecimal(0.73)).ToString())
                                             .Replace("{Tax}", Math.Round(Convert.ToDecimal(booking.TotalPrice) * Convert.ToDecimal(0.27)).ToString())
                                             .Replace("{Total}", Math.Round(Convert.ToDecimal(booking.TotalPrice)).ToString())
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

                byte[] pdfBytes = GenerateInvoicePdf(booking, guestdata, roomdata, paymentdata);

                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress(_smtpSettings.FromEmail);
                    message.Subject = $"Foglalás megerősítése - {booking.BookingId}";
                    message.Body = emailTemplate;
                    message.IsBodyHtml = true;
                    message.To.Add(recipientEmail);


                    using (var pdfStream = new MemoryStream(pdfBytes))
                    {
                        var attachment = new Attachment(pdfStream, "foglalas_szamla.pdf", MediaTypeNames.Application.Pdf);
                        message.Attachments.Add(attachment);

                        using (SmtpClient client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
                        {
                            client.EnableSsl = true;
                            client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                            await client.SendMailAsync(message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Hiba e-mail küldéskor: {ex.Message}");
            }
        }


        /// Generates a PDF invoice for booking confirmation
        private byte[] GenerateInvoicePdf(Booking booking, Guest guest, Room room, Payment payment)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));
                    page.Background().Border(1).BorderColor(Colors.Grey.Lighten2);


                    page.Header().Padding(1, Unit.Centimetre).Column(column =>
                    {
                        column.Item().Row(row =>
                        {

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("HMZ RT").FontSize(22).Bold().FontColor(Colors.Indigo.Medium);
                                c.Item().Text("Palóczy László utca 3, Miskolc, BAZ, 3531").FontSize(9).FontColor(Colors.Grey.Medium);
                                c.Item().Text("Tel: +36 (70) 123-4567 | Email: hmzrtkando@gmail.com").FontSize(9).FontColor(Colors.Grey.Medium);
                            });


                            row.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().Text("FOGLALÁSI VISSZAIGAZOLÁS").FontSize(16).Bold().FontColor(Colors.Indigo.Medium);
                                c.Item().Text($"Foglalás száma: #{booking.BookingId}").FontSize(12).FontColor(Colors.Grey.Medium);
                                c.Item().Text($"Dátum: {booking.BookingDate:yyyy.MM.dd}").FontSize(10).FontColor(Colors.Grey.Medium);
                            });
                        });
                    });


                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                    {

                        column.Item().PaddingBottom(10).Component(new TableComponent("VENDÉG ADATOK", new List<(string, string)>
         {
             ("Név", $"{guest.FirstName} {guest.LastName}"),
             ("Email", guest.Email ?? "N/A"),
             ("Telefon", guest.PhoneNumber ?? "N/A"),
             ("Cím", guest.Address ?? "N/A")
         }));


                        column.Item().PaddingTop(10).PaddingBottom(10).Component(new TableComponent("FOGLALÁS RÉSZLETEI", new List<(string, string)>
         {
             ("Szoba", $"{room.RoomNumber}-es számú szoba"),
             ("Érkezés", booking.CheckInDate?.ToString("yyyy.MM.dd") ?? "N/A"),
             ("Távozás", booking.CheckOutDate?.ToString("yyyy.MM.dd") ?? "N/A"),
             ("Időtartam", booking.CheckOutDate.HasValue && booking.CheckInDate.HasValue
                 ? $"{(booking.CheckOutDate.Value - booking.CheckInDate.Value).TotalDays} nap"
                 : "N/A")
         }));


                        column.Item().PaddingTop(10).PaddingBottom(10).Component(new TableComponent("FIZETÉSI INFORMÁCIÓK", new List<(string, string)>
         {
             ("Fizetési mód", payment.PaymentMethod),
             ("Fizetés állapota", booking.PaymentStatus)
         }));


                        column.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });


                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Indigo.Medium).Padding(5).Text("Tétel").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Indigo.Medium).Padding(5).Text("Részletek").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Indigo.Medium).Padding(5).AlignRight().Text("Összeg").FontColor(Colors.White).Bold();
                            });


                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Szobafoglalás");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"{room.RoomNumber}-es szoba, {(booking.CheckOutDate.Value - booking.CheckInDate.Value).TotalDays} nap");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"{Math.Round(Convert.ToDecimal(booking.TotalPrice))} HUF");




                            decimal subtotal = Math.Round(Convert.ToDecimal(booking.TotalPrice) * Convert.ToDecimal(0.73));
                            table.Cell().Padding(5).Text("");
                            table.Cell().Padding(5).AlignRight().Text("Részösszeg:").Bold();
                            table.Cell().Padding(5).AlignRight().Text($"{subtotal} HUF");


                            decimal tax = Math.Round(Convert.ToDecimal(booking.TotalPrice) * Convert.ToDecimal(0.27));
                            table.Cell().Padding(5).Text("");
                            table.Cell().Padding(5).AlignRight().Text("ÁFA (27%):").Bold();
                            table.Cell().Padding(5).AlignRight().Text($"{tax} HUF");


                            decimal total = Math.Round(Convert.ToDecimal(booking.TotalPrice));
                            table.Cell().Padding(5).Text("");
                            table.Cell().Padding(5).AlignRight().Text("Végösszeg:").Bold();
                            table.Cell().Padding(5).AlignRight().Text($"{total} HUF").FontColor(Colors.Indigo.Medium).Bold();
                        });


                        column.Item().PaddingTop(20).Background(Colors.Grey.Lighten4).Padding(10).Column(c =>
                        {
                            c.Item().Text("Köszönjük a foglalását!").FontSize(14).Bold().FontColor(Colors.Indigo.Medium);
                            c.Item().Text("Reméljük, hogy kellemes időt tölt nálunk. Ha bármilyen kérdése vagy kérése van, kérjük, vegye fel velünk a kapcsolatot.").FontSize(10);
                        });
                    });


                    page.Footer().Padding(1, Unit.Centimetre).Column(column =>
                    {
                        column.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"© {DateTime.Now.Year} HMZ RT. Minden jog fenntartva.").FontSize(9).FontColor(Colors.Grey.Medium);
                            });

                            row.RelativeItem().AlignRight().Text(text =>
                            {
                                text.Span("Oldal ").FontSize(9).FontColor(Colors.Grey.Medium);
                                text.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Medium);
                                text.Span(" / ").FontSize(9).FontColor(Colors.Grey.Medium);
                                text.TotalPages().FontSize(9).FontColor(Colors.Grey.Medium);
                            });
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }

        /// Helper component for creating info tables in the PDF
        public class TableComponent : IComponent
        {
            private readonly string _title;
            private readonly List<(string Label, string Value)> _items;

            public TableComponent(string title, List<(string, string)> items)
            {
                _title = title;
                _items = items;
            }

            public void Compose(IContainer container)
            {
                // Create styled table with title and content rows
            }
        }

        /// Creates a new booking for a room
        [Authorize(Roles = "Base,Admin,System")]
        [HttpPost("New_Booking/{roomid}")]
        public async Task<ActionResult<Booking>> Booking(int roomid, CreateBookingDto crtbooking)
        {
            // Validate input parameters
            if (crtbooking == null)
            {
                return BadRequest();
            }

            if (!crtbooking.CheckInDate.HasValue || !crtbooking.CheckOutDate.HasValue)
            {
                return BadRequest("A kezdő és végdátum megadása kötelező.");
            }

            var checkInDate = crtbooking.CheckInDate.Value;
            var checkOutDate = crtbooking.CheckOutDate.Value;

            // Check if dates are valid
            if (checkOutDate <= checkInDate)
            {
                return BadRequest("A foglalás végdátuma a kezdet után kell legyen.");
            }

            var bookingDuration = (checkOutDate - checkInDate).TotalDays;
            if (bookingDuration < 2)
            {
                return StatusCode(400, "Minimum 2 napra kell foglalni");
            }

            // Check if room exists
            var roomData = await _context.Rooms.FirstOrDefaultAsync(x => x.RoomId == roomid);
            if (roomData == null)
            {
                return StatusCode(404, "Nem létező szobára hivatkoztál");
            }

            // Check if guest exists
            var guestData = await _context.Guests.FirstOrDefaultAsync(x => x.GuestId == crtbooking.GuestId);
            if (guestData == null)
            {
                return StatusCode(404, "Vendég megadása szükséges foglalás előtt");
            }

            // Check if user account exists
            var user = await _context.Useraccounts.FirstOrDefaultAsync(x => x.UserId == guestData.UserId);
            if (user == null)
            {
                return StatusCode(404, "Nem található felhasználó a vendéghez");
            }

            // Check for conflicting bookings in the same period
            var existingBooking = await _context.Bookings
                .AnyAsync(b => b.RoomId == roomid &&
                              ((crtbooking.CheckInDate >= b.CheckInDate && crtbooking.CheckInDate < b.CheckOutDate) ||
                               (crtbooking.CheckOutDate > b.CheckInDate && crtbooking.CheckOutDate <= b.CheckOutDate) ||
                               (crtbooking.CheckInDate <= b.CheckInDate && crtbooking.CheckOutDate >= b.CheckOutDate)));

            if (existingBooking)
            {
                return StatusCode(409, "A szoba foglalt a kiválasztott időszakra");
            }

            // Check room capacity
            if (roomData.Capacity < crtbooking.NumberOfGuests)
            {
                return StatusCode(400, "Túl sok fővel próbáltál foglalni");
            }

            // Create new booking
            var booking = new Booking
            {
                CheckInDate = crtbooking.CheckInDate,
                CheckOutDate = crtbooking.CheckOutDate,
                GuestId = crtbooking.GuestId,
                RoomId = roomid,
                TotalPrice = roomData.PricePerNight * crtbooking.NumberOfGuests,
                BookingDate = DateTime.Now,
                PaymentStatus = "Fizetésre vár",
                NumberOfGuests = crtbooking.NumberOfGuests,
                Status = "Jóváhagyva"
            };

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            // Create payment record
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

            // Send confirmation email
            await SendBookingConfirmation(booking, user.Email, guestData, roomData, payment);

            return StatusCode(201, "Sikeres foglalás");
        }

        /// Gets all bookings for a specific user
        [Authorize(Roles = "Base,Admin,System,Recept")]
        [HttpGet("BookingsByUserId")]
        public async Task<ActionResult<Booking>> GetUserBookings(int UserIdd)
        {
            var data = await _context.Guests.FirstOrDefaultAsync(x => x.UserId == UserIdd);
            var reservation = _context.Bookings.Where(x => x.GuestId == data.GuestId).Include(x => x.Payments);
            if (data != null)
            {
                return StatusCode(201, reservation);
            }
            return BadRequest();
        }


        /// Deletes a booking by ID
        [Authorize(Roles = "Base,Admin,System,Recept")]
        [HttpDelete("DeleteBooking/{id}")]
        public async Task<ActionResult<Booking>> DeleteBooking(int id)
        {
            try
            {
                var books = await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == id);
                if (books != null)
                {
                    _context.Bookings.Remove(books);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Sikeres törlés");
                }
                return StatusCode(404, "Nem található a foglalás");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Updates booking details by ID - MARKED AS DO NOT USE
        //DO NOT USE
        [Authorize(Roles = "Base,Admin,System,Recept")]
        [HttpPut("UpdateBooking/{id}")]
        public async Task<ActionResult<Booking>> UpdateBookingByid(int id, UpdateBooking udto)
        {
            try
            {
                var updatedbook = await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == id);
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

        /// Gets all bookings from the past month to one year in the future
        [Authorize(Roles = "Base,Admin,System,Recept")]
        [HttpGet("Getalldat")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetallData()
        {
            try
            {
                // Get bookings from last month to next year
                var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);
                var oneYearAway = DateTime.UtcNow.AddYears(1);

                var data = await _context.Bookings
                    .Where(b => b.CheckOutDate >= oneMonthAgo && b.CheckInDate <= oneYearAway)
                    .ToListAsync();

                if (data != null && data.Any())
                {
                    return Ok(data);
                }
                return NotFound("Nincsenek foglalások az elmúlt 1 hónapban vagy a közeljövőben");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Updates booking status, with validation for payment status
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPut("UpdateBookingStatus/{id}")]
        public async Task<ActionResult<Booking>> UpdateBookingStatusByid(int id, UpdateBookingStatus udto)
        {
            try
            {
                var updatedbook = await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == id);
                if (updatedbook != null)
                {
                    // Can only mark as finished if already paid
                    if (updatedbook.PaymentStatus.Trim().ToLower() == "fizetve" && udto.Status.Trim().ToLower() == "finished")
                    {
                        updatedbook.Status = udto.Status;
                    }
                    if (updatedbook.PaymentStatus.Trim().ToLower() != "fizetve" && udto.Status.Trim().ToLower() == "finished")
                    {
                        return StatusCode(400, "Még nincs kifizetve, addig nem lezárható");
                    }

                    // Can update status if not already finished
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

        /// Gets all booked dates for a specific room
        [Authorize(Roles = "Admin,Base,Recept,System")]
        [HttpGet("GetBookedDates/{roomid}")]
        public async Task<ActionResult<Booking>> GetBookeddates(int roomid)
        {
            try
            {
                var booksdata = _context.Bookings.Where(x => x.RoomId == roomid);
                if (booksdata != null)
                {
                    // Return just the check-in and check-out dates
                    var data = await booksdata.Select(b => new { b.CheckInDate, b.CheckOutDate }).ToListAsync();
                    return Ok(data);
                }
                return StatusCode(404, "Nem található szoba");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}