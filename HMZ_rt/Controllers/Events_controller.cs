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

        [Authorize(Roles = "Admin, System,Recept")]
        [HttpGet("Geteventsadmin")]
        public async Task<ActionResult<Event>> Getallevent()
        {
            try
            {
                var events = await _context.Events.Include(x => x.Eventbookings).ToListAsync();
                if (events != null) {
                    return StatusCode(201, events);

                }
                return StatusCode(404, "üres");

            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }

        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPost("CreateEvent")]
        public async Task<ActionResult<Event>> Createevent(CreateEvent crtdto)
        {
            try
            {
                var existing = await _context.Events.FirstOrDefaultAsync(x => x.EventName == crtdto.EventName);
                if (existing != null)
                {
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
                        DateAdded = DateTime.Now
                    };
                    if (news != null) { 
                        await _context.Events.AddAsync(news);
                        await _context.SaveChangesAsync();
                        return StatusCode(201, "Sikeres mentés");
                    } return StatusCode(404, "Valami üres");
                } return StatusCode(201);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }



        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPut("UpdateEvent/{id}")]
        public async Task<ActionResult<Event>> Updateevent(int id, UpdateEvent udto) {
            try
            {
                var egyevent = await _context.Events.FirstOrDefaultAsync(x => x.EventId == id); 
                if (egyevent != null)
                {
                    egyevent.Status = udto.Status;
                    egyevent.Capacity = udto.Capacity;
                    egyevent.EventName = udto.EventName;
                    egyevent.EventDate = udto.EventDate;
                    egyevent.Location = udto.Location;
                    egyevent.Description = udto.Description;
                    egyevent.OrganizerName = udto.OrganizerName;
                    egyevent.ContactInfo = udto.ContactInfo;
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Sikeres mentés");
                }return StatusCode(404, "Valami nem jó");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }
        [HttpGet("Geteents")]
        public async Task<ActionResult<Event>> getevents()
        {
            try
            {


            var dat = await _context.Events.Include(x => x.Eventbookings).Where(x=> x.EventDate >= DateTime.Now ).ToListAsync();
                return StatusCode(201, dat);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }
    }
}
