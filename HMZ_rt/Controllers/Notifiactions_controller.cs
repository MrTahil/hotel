using HMZ_rt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HMZ_rt.Controllers
{
    [Route("UserAccounts")]
    [ApiController]
    public class Notifiactions_controller : Controller
    {
        private readonly HmzRtContext _context;





        public Notifiactions_controller(HmzRtContext context) { _context = context; }
        [HttpPost("Sent Messages")]
        public async Task<ActionResult<Notification>> NewMessage(CreateNotifiactionDto notidto)
        {
            var isis = new Notification {
                Message = notidto.Message,
                Status = notidto.Status,
                Type = notidto.Type,
                Priority = notidto.Priority,
                Notes = notidto.Notes,
                UserId = notidto.UserId,
                Category = notidto.Category,
                DateSent = DateTime.MinValue,
                DateRead = DateTime.MinValue,
            };
            if (isis != null)
            {
                await _context.Notifications.AddAsync(isis);
                await _context.SaveChangesAsync();
                return StatusCode(201, isis);
            }
            return BadRequest();
        }


        [HttpPut("UpdateSentTime{NotiId}")]
        public async Task<ActionResult<Notification>> UpdateOnSent(int NotiId)
        {
            var oke=await _context.Notifications.FirstOrDefaultAsync(x => x.NotificationId == NotiId);
            if (oke != null)
            {
                oke.DateSent = DateTime.Now;
                _context.Notifications.Update(oke);
                await _context.SaveChangesAsync();
                return Ok(oke);
            }
            return NotFound(new { message = "Nincs ilyen id-val rendelkező adat az adatbázisban." });
        }

        [HttpPut("UpdateOpenedTime{NotiId}")]
        public async Task<ActionResult<Notification>> UpdateOnOpened(int NotiId)
        {
            var oke = await _context.Notifications.FirstOrDefaultAsync(x => x.NotificationId == NotiId);
            if (oke != null)
            {
                oke.DateRead = DateTime.Now;
                _context.Notifications.Update(oke);
                await _context.SaveChangesAsync();
                return Ok(oke);
            }
            return NotFound(new { message = "Nincs ilyen id-val rendelkező adat az adatbázisban." });
        }
    }
}
