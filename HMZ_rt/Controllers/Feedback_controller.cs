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

        /// Retrieves all feedback entries from the database.
        /// <returns>A list of all feedback or 404 if none found.</returns>
        [Authorize(Roles = "System,Admin,Recept")]
        [HttpGet("GetallFeedback")]
        public async Task<ActionResult<Feedback>> Getallfeedback()
        {
            try
            {
                var data = await _context.Feedbacks.ToListAsync();
                if (data != null && data.Any())
                {
                    return StatusCode(200, data);
                }
                return StatusCode(404, "Not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Creates a new feedback entry in the database.
        /// <param name="crtdto">The feedback details to create.</param>
        /// <returns>Success message or error if creation fails.</returns>
        [Authorize(Roles = "Admin,System,Recept")]
        [HttpPost("UploadFeedback")]
        public async Task<ActionResult<Feedback>> Uploadnewfeedback(CreateFeedback crtdto)
        {
            try
            {
                if (crtdto != null)
                {
                    // Verify the guest exists before creating feedback
                    var guest = await _context.Guests.FirstOrDefaultAsync(x => x.GuestId == crtdto.GuestId);
                    if (guest == null)
                    {
                        return StatusCode(404, "No guest found with this ID");
                    }

                    // Create new feedback object
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

                    _context.Feedbacks.Add(feed);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, "Successfully saved");
                }

                return StatusCode(418, "If you reach this point, there's a big problem");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Updates an existing feedback entry by ID.
        /// <param name="udto">The updated feedback details.</param>
        /// <param name="id">The ID of the feedback to update.</param>
        /// <returns>Success message or error if update fails.</returns>
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
                    return StatusCode(201, "Successfully saved");
                }

                return StatusCode(404, "ID not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Deletes a feedback entry by ID.
        /// <param name="id">The ID of the feedback to delete.</param>
        /// <returns>Success message or error if deletion fails.</returns>
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
                    return StatusCode(201, "Successfully deleted");
                }

                return StatusCode(404, "Data not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
