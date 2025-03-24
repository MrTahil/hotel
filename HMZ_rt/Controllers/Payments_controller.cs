using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace HMZ_rt.Controllers
{
    [Route("Payments")]
    [ApiController]
    public class Payments_controller : Controller
    {
        private readonly StripeClient _stripeClient;
        private readonly HmzRtContext _context;

        /// Initializes the Payments controller with database context and Stripe configuration.
        /// <param name="context">The database context.</param>
        /// <param name="config">The application configuration containing Stripe API keys.</param>
        public Payments_controller(HmzRtContext context, IConfiguration config)
        {
            _context = context;
            StripeConfiguration.ApiKey = config["Stripe:sk_test_51R27jdFtGHCiSxUKjs6qi5njSOSiCPwsUU8U1nsrmPnp6vMrE3TXdz58vjsq5FpzzMsbY0ZZG9UWO9CAcm5pXSHs00g2xPr3kI"];
            _stripeClient = new StripeClient(config["Stripe:sk_test_51R27jdFtGHCiSxUKjs6qi5njSOSiCPwsUU8U1nsrmPnp6vMrE3TXdz58vjsq5FpzzMsbY0ZZG9UWO9CAcm5pXSHs00g2xPr3kI"]);
        }

        // The following methods are commented out in the original code

        //[HttpPost("create-intent")]
        //public async Task<IActionResult> CreatePaymentIntent([FromBody] PaymentIntentCreateRequest request)
        //{
        //    try
        //    {
        //        var options = new PaymentIntentCreateOptions
        //        {
        //            Amount = request.Amount,
        //            Currency = "usd",
        //            PaymentMethodTypes = new List<string> { "card" }
        //        };

        //        var service = new PaymentIntentService();
        //        var paymentIntent = await service.CreateAsync(options);

        //        return Ok(new { clientSecret = paymentIntent.ClientSecret });
        //    }
        //    catch (StripeException e)
        //    {
        //        return BadRequest(new { error = e.StripeError.Message });
        //    }
        //}

        //[HttpPost("webhook")]
        //public async Task<IActionResult> HandleWebhook()
        //{
        //    var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        //    var stripeEvent = EventUtility.ConstructEvent(
        //        json,
        //        Request.Headers["Stripe-Signature"],
        //        "YOUR_WEBHOOK_SECRET"
        //    );

        //    if (stripeEvent.Type == "payment_intent.succeeded")
        //    {
        //        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        //        // Implementáld a sikeres fizetés logikáját
        //        return Ok();
        //    }

        //    return BadRequest();
        //}


        /// Updates the payment status and method for a specific booking.
        /// <param name="bookid">The ID of the booking to update payment for.</param>
        /// <param name="udto">The updated payment information.</param>
        /// <returns>Success message or error if update fails.</returns>
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPut("UpdatePaymentStatusByBookingId/{bookid}")]
        public async Task<ActionResult<Payment>> UpdateStatus(int bookid, UpdatePaymentInfo udto)
        {
            try
            {
                // Find payment and booking records
                var payment = await _context.Payments.FirstOrDefaultAsync(x => x.BookingId == bookid);
                var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == bookid);

                if (payment != null)
                {
                    // Check if payment is already marked as paid
                    if (payment.Status.Trim().ToLower() == "fizetve")
                    {
                        return StatusCode(401, "You cannot modify this as it's already paid!");
                    }

                    // Update payment and booking status
                    payment.Status = udto.Status;
                    payment.PaymentMethod = udto.PaymentMethod;
                    booking.PaymentStatus = udto.Status;

                    _context.Payments.Update(payment);
                    await _context.SaveChangesAsync();

                    _context.Bookings.Update(booking);
                    await _context.SaveChangesAsync();

                    return StatusCode(201, "Success");
                }

                return StatusCode(404, "ID not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }

    // Commented out class in original code
    //public class PaymentIntentCreateRequest
    //{
    //    public long Amount { get; set; }
    //}
}
