using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HMZ_rt.Controllers
{
    [Route("Notifications")]
    [ApiController]
    public class Notifiactions_controller : Controller
    {
        private readonly HmzRtContext _context;

        public Notifiactions_controller(HmzRtContext context)
        {
            _context = context;
        }

        /// Creates a new notification message in the system.
        /// <param name="notidto">The notification details to create.</param>
        /// <returns>The created notification or error if creation fails.</returns>
        [Authorize(Roles = "System,Admin,Recept")]
        [HttpPost("Sent Messages")]
        public async Task<ActionResult<Notification>> NewMessage(CreateNotifiactionDto notidto)
        {
            // Create new notification object
            var isis = new Notification
            {
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

        /// Updates the sent timestamp of a notification.
        /// <param name="NotiId">The ID of the notification to update.</param>
        /// <returns>The updated notification or error if update fails.</returns>
        [HttpPut("UpdateSentTime/{NotiId}")]
        public async Task<ActionResult<Notification>> UpdateOnSent(int NotiId)
        {
            var oke = await _context.Notifications.FirstOrDefaultAsync(x => x.NotificationId == NotiId);
            if (oke != null)
            {
                oke.DateSent = DateTime.Now;
                _context.Notifications.Update(oke);
                await _context.SaveChangesAsync();
                return Ok(oke);
            }
            return NotFound(new { message = "No data with this ID exists in the database." });
        }

        /// Updates the read timestamp of a notification when it's opened.
        /// <param name="NotiId">The ID of the notification to update.</param>
        /// <returns>The updated notification or error if update fails.</returns>
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
            return NotFound(new { message = "No data with this ID exists in the database." });
        }

        /// Deletes a notification by ID.
        /// <param name="id">The ID of the notification to delete.</param>
        /// <returns>Success message or error if deletion fails.</returns>
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
                    return StatusCode(200, "Successfully deleted");
                }
                return StatusCode(404, "No data with this ID in the database");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Updates the status of a notification.
        /// <param name="id">The ID of the notification to update.</param>
        /// <param name="udto">The updated status information.</param>
        /// <returns>Success message or error if update fails.</returns>
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
                    return StatusCode(201, "Successfully changed");
                }
                return StatusCode(404, "Not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
