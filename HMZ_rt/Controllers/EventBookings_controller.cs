﻿using HMZ_rt.Models;
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
                var param = await _context.Events
                    .Where(x => x.EventDate > DateTime.Now.AddDays(-1))
                    .Select(x => x.EventId)
                    .ToListAsync();

                var paramHashSet = new HashSet<int>(param);

                var data = await _context.Eventbookings
                    .Where(x => paramHashSet.Contains(x.EventId))
                    .ToListAsync(); 
                if (data != null)
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
                
                var eventdata =  _context.Events.FirstOrDefault(x => x.EventId == eventid && x.EventDate > DateTime.Now.AddDays(-1));
                if (eventdata == null) {
                    return StatusCode(404, "Ez az esemény nem létezik vagy már véget ért.");
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



