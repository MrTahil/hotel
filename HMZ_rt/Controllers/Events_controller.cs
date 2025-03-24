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
        public async Task<ActionResult<Event>> Createevent([FromForm] CreateEvent crtdto)
        {
            try
            {
                var existing = await _context.Events.FirstOrDefaultAsync(x => x.EventName == crtdto.EventName);
                if (existing == null)
                {
                    string imagePath = "";

                    if (crtdto.ImageFile != null && crtdto.ImageFile.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(crtdto.ImageFile.FileName);

                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "events");

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string filePath = Path.Combine(uploadsFolder, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await crtdto.ImageFile.CopyToAsync(fileStream);
                        }

                        imagePath = "/images/events/" + fileName;
                    }

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

                    if (news != null)
                    {
                        _context.Events.Add(news);
                        await _context.SaveChangesAsync();
                        return StatusCode(201, "Sikeres mentés");
                    }
                    return StatusCode(404, "Valami üres");
                }
                return StatusCode(201);
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
                    if (data.Price < 0)
                    {
                        return BadRequest("Nem lehet kevesebb mint 0 az ár");
                    }
                    if (data.EventDate > DateTime.Now.AddDays(1))
                    {
                        return BadRequest("Nem lehet 1 napnál hamarabbi eseményt hozzáadni");
                    }
                    if (data.Capacity < 0)
                    {
                        return BadRequest("A kapacitás nem lehet kevesebb mint 1");
                    }

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


            var dat = await _context.Events.Include(x => x.Eventbookings).Where(x=> x.EventDate > DateTime.Now.AddDays(-1) ).ToListAsync();
                return StatusCode(201, dat);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }
        [Authorize(Roles ="Admin,System,Recept")]
        [HttpDelete("DeleteEvenet/{id}")]
        public async Task<ActionResult<Event>> DeleteEvent(int id)
        {
            try
            {
                var dat = await _context.Events.FirstOrDefaultAsync(x => x.EventId == id);
                if (dat != null) { 
                _context.Events.Remove(dat);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Sikeres törlés");
                }
                return StatusCode(404, "Nincs találat");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }
    }
}







//public async Task<IActionResult> UploadProfilePicture(IFormFile file, [FromForm] string email)
//{
//    if (file == null || file.Length == 0)
//    {
//        return BadRequest("Nincs kiválasztott fájl.");
//    }

//    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_pictures");
//    if (!Directory.Exists(uploadsFolder))
//    {
//        Directory.CreateDirectory(uploadsFolder);
//    }

//    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
//    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

//    using (var stream = new FileStream(filePath, FileMode.Create))
//    {
//        await file.CopyToAsync(stream);
//    }

//    var imageUrl = $"/profile_pictures/{uniqueFileName}";


//    user.ProfilePictureUrl = imageUrl;
//    _context.users.Update(user);
//    await _context.SaveChangesAsync();

//    return Ok(new { imageUrl });
//}