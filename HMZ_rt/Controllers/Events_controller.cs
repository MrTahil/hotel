using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMZ_rt.Controllers
{
    [Route("Events")]
    [ApiController]
    public class Events_controller : Controller
    {
        private readonly HmzRtContext _context;

        public Events_controller(HmzRtContext context)
        {
            _context = context;
        }

        /// Retrieves all events with their bookings for administrative purposes.
        /// <returns>A list of all events with their bookings.</returns>
        [Authorize(Roles = "Admin, System,Recept")]
        [HttpGet("Geteventsadmin")]
        public async Task<ActionResult<Event>> Getallevent()
        {
            try
            {
                var events = await _context.Events.Include(x => x.Eventbookings).ToListAsync();
                if (events != null && events.Any())
                {
                    return StatusCode(201, events);
                }
                return StatusCode(404, "Empty");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Creates a new event with optional image upload.
        /// <param name="crtdto">The event details including an optional image file.</param>
        /// <returns>Success message or error if creation fails.</returns>
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPost("CreateEvent")]
        public async Task<ActionResult<Event>> Createevent([FromForm] CreateEvent crtdto)
        {
            try
            {
                // Check if an event with the same name already exists
                var existing = await _context.Events.FirstOrDefaultAsync(x => x.EventName == crtdto.EventName);
                if (existing == null)
                {
                    string imagePath = "";

                    // Process image upload if provided
                    if (crtdto.ImageFile != null && crtdto.ImageFile.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(crtdto.ImageFile.FileName);
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "events");

                        // Create directory if it doesn't exist
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string filePath = Path.Combine(uploadsFolder, fileName);

                        // Save the file
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await crtdto.ImageFile.CopyToAsync(fileStream);
                        }

                        imagePath = "/images/events/" + fileName;
                    }

                    // Create new event object
                    var news = new Event
                    {
                        EventName = crtdto.EventName,
                        Capacity = crtdto.Capacity,
                        Status = crtdto.Status,
                        EventDate = crtdto.EventDate,
                        Location = crtdto.Location,
                        Description = crtdto.Description,
                        OrganizerName = crtdto.OrganizerName,
                        ContactInfo = crtdto.ContactInfo,
                        DateAdded = DateTime.Now,
                        Price = crtdto.Price,
                        Images = imagePath
                    };

                    _context.Events.Add(news);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Successfully saved");
                }
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


        /// Updates an existing event by ID.
        /// <param name="id">The ID of the event to update.</param>
        /// <param name="udto">The updated event details.</param>
        /// <returns>Success message or error if update fails.</returns>
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPut("UpdateEvent/{id}")]
        public async Task<ActionResult<Event>> Updateevent(int id, UpdateEvent udto)
        {
            try
            {
                var data = await _context.Events.FirstOrDefaultAsync(x => x.EventId == id);

                if (data != null)
                {
                    data.Status = udto.Status;
                    data.Capacity = udto.Capacity;
                    data.EventName = udto.EventName;
                    data.EventDate = udto.EventDate;
                    data.Location = udto.Location;
                    data.Description = udto.Description;
                    data.OrganizerName = udto.OrganizerName;
                    data.ContactInfo = udto.ContactInfo;
                    data.Price = udto.Price;

                    // Validate price
                    if (data.Price < 0)
                    {
                        return BadRequest("Price cannot be less than 0");
                    }

                    // Validate event date
                    if (data.EventDate > DateTime.Now.AddDays(1))
                    {
                        return BadRequest("Cannot add an event less than 1 day from now");
                    }

                    // Validate capacity
                    if (data.Capacity < 0)
                    {
                        return BadRequest("Capacity cannot be less than 1");
                    }

                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Successfully saved");
                }
                return StatusCode(404, "Something is not right");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Retrieves all upcoming events (events that haven't ended yet).
        /// <returns>A list of upcoming events with their bookings.</returns>
        [HttpGet("Geteents")]
        public async Task<ActionResult<Event>> getevents()
        {
            try
            {
                // Get events that haven't ended yet (after yesterday)
                var dat = await _context.Events
                    .Include(x => x.Eventbookings)
                    .Where(x => x.EventDate > DateTime.Now.AddDays(-1))
                    .ToListAsync();

                return StatusCode(201, dat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


        /// Deletes an event by ID.
        /// <param name="id">The ID of the event to delete.</param>
        /// <returns>Success message or error if deletion fails.</returns>
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpDelete("DeleteEvenet/{id}")]
        public async Task<ActionResult<Event>> DeleteEvent(int id)
        {
            try
            {
                var dat = await _context.Events.FirstOrDefaultAsync(x => x.EventId == id);
                if (dat != null)
                {
                    _context.Events.Remove(dat);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Successfully deleted");
                }
                return StatusCode(404, "No match found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
