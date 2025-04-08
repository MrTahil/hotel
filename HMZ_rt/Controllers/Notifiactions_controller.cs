using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
#pragma warning disable
namespace HMZ_rt.Controllers
{
    [Route("UserAccounts")]
    [ApiController]
    public class Notifiactions_controller : Controller
    {
        private readonly HmzRtContext _context;





        public Notifiactions_controller(HmzRtContext context) { _context = context; }
        [Authorize(Roles = "System,Admin,Recept")]
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


        [HttpPut("UpdateSentTime/{NotiId}")]
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
            return NotFound( "Nincs ilyen id-val rendelkező adat az adatbázisban." );
        }
        
        [HttpPut("UpdateOpenedTime/{NotiId}")]
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
            return NotFound( "Nincs ilyen id-val rendelkező adat az adatbázisban." );
        }

        [Authorize(Roles = "Admin,System,Recept")]
        [HttpDelete("DeleteNoti/{id}")]
        public async Task<ActionResult<Notification>> DeleteNoti(int id)
        {
            try
            {
                var adat = await _context.Notifications.FirstOrDefaultAsync(x => x.NotificationId == id);
                if (adat != null)
                {
                    _context.Notifications.Remove(adat);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, "Sikeres törlés");
                } return StatusCode(404, "Nincs ilyen id-val adat az adatbázisban");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
            
        }

        [Authorize(Roles = "Admin,System,Recept,Base")]
        [HttpPut("UpdateStatusofNoti/{id}")]
        public async Task<ActionResult<Notification>> UpdateNotificationStatus(int id, UpdateNotiStatus udto)
        {
            try
            {
                var adat = await _context.Notifications.FirstOrDefaultAsync(x => x.NotificationId == id);
                if (adat != null)
                {
                    adat.Status = udto.Status;
                    _context.Notifications.Update(adat);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Sikeres változtatás");
                }
                return StatusCode(404, "Nem található");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
    }
}
