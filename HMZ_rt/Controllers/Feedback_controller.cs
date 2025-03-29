using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMZ_rt.Controllers
{
    [Route("Feedback")]
    [ApiController]
    public class Feedback_controller : Controller
    {
        private readonly HmzRtContext _context;
        public Feedback_controller(HmzRtContext context)
        {
            _context = context;
        }



        [Authorize(Roles = "System,Admin,Recept")]
        [HttpGet("GetallFeedback")]
        public async Task<ActionResult<Feedback>> Getallfeedback()
        {
            try
            {

            var data =await _context.Feedbacks.ToListAsync();
                if (data != null) {
                    return StatusCode(200, data);
                }
                return StatusCode(404, "Nem található");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }


        [Authorize(Roles ="Admin,System,Recept")]
        [HttpPost("UploadFeedback")]
        public async Task<ActionResult<Feedback>> Uploadnewfeedback(CreateFeedback crtdto) {

            try
            {
                if (crtdto != null)
                {
                    var feed = new Feedback
                    {
                        FeedbackDate = DateTime.Now,
                        Comments = "",
                        Category = crtdto.Category,
                        Rating = crtdto.Rating,
                        Status = crtdto.Status,
                        Response = crtdto.Response,
                        ResponseDate = DateTime.Now,
                        DateAdded = DateTime.Now,
                        GuestId = crtdto.GuestId
                    };
                    var guest = await _context.Guests.FirstOrDefaultAsync(x => x.GuestId == crtdto.GuestId);
                    if (guest == null) {
                        return StatusCode(404, "Nem található ezzel az Id-val vendég");
                    }
                    if (feed != null)
                    {
                        _context.Feedbacks.Add(feed);
                       await _context.SaveChangesAsync();
                        return StatusCode(201, "Sikeres mentés");
                    }

                } return StatusCode(418, "Ha ide jutsz nagy baj van");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }


        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPut("UpdateForFeedback/{id}")]
        public async Task<ActionResult<Feedback>> UpdateFeedback(UpdateFeedback udto, int id)
        {
            try
            {
                var adat = await _context.Feedbacks.FirstOrDefaultAsync(x => x.FeedbackId == id);
                if (adat != null)
                {
                    adat.Comments = udto.Comment;
                    adat.Status = udto.Status;
                    _context.Feedbacks.Update(adat);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Sikeres mentés");
                } return StatusCode(404, "Nem található id");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }

        [Authorize(Roles = "Admin,System,Recept")]
        [HttpDelete("DeleteFeedback/{id}")]
        public async Task<ActionResult<Feedback>> DeleteFeedback(int id)
        {
            try
            {
                var data = await _context.Feedbacks.FirstOrDefaultAsync(x => x.FeedbackId == id);
                if (data != null)
                {
                    _context.Feedbacks.Remove(data);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Sikeres törlés");
                } return StatusCode(404, "Nem található adat");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }
    }
}
