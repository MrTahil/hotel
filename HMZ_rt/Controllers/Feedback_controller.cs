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
                return StatusCode(400, "Ez biza üres");
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

                    if (feed != null)
                    {
                        _context.Feedbacks.Add(feed);
                       await _context.SaveChangesAsync();
                        return StatusCode(200, "Sikeres mentés");
                    }

                } return StatusCode(418, "Ha ide jutsz nagy baj van");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }
    }
}
