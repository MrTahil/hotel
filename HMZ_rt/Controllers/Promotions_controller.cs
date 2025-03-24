using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMZ_rt.Controllers
{
    [Route("Promotions")]
    [ApiController]
    public class Promotions_controller : Controller
    {
        private readonly HmzRtContext _context;

        public Promotions_controller(HmzRtContext context)
        {
            _context = context;
        }

        /// Retrieves all promotions from the database.
        /// <returns>A list of all promotions.</returns>
        [HttpGet("Getpromotions")]
        public async Task<ActionResult<Promotion>> getpromotions()
        {
            try
            {
                var promotions = await _context.Promotions.ToListAsync();
                return StatusCode(201, promotions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Deletes a promotion by ID.
        /// <param name="id">The ID of the promotion to delete.</param>
        /// <returns>Success message or error if deletion fails.</returns>
        [Authorize(Roles = "System,Admin,Recept")]
        [HttpDelete("Deletepromotion/{id}")]
        public async Task<ActionResult<Promotion>> DeletePromotion(int id)
        {
            try
            {
                var promotion = await _context.Promotions.FirstOrDefaultAsync(x => x.PromotionId == id);
                if (promotion != null)
                {
                    _context.Promotions.Remove(promotion);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Successfully deleted");
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Updates an existing promotion by ID.
        /// <param name="id">The ID of the promotion to update.</param>
        /// <param name="udto">The updated promotion details.</param>
        /// <returns>Success message or error if update fails.</returns>
        [Authorize(Roles = "System,Admin,Recept")]
        [HttpPut("UpdatePromotion/{id}")]
        public async Task<ActionResult<Promotion>> UpdatePromotion(int id, promotionupdate udto)
        {
            try
            {
                var os = await _context.Promotions.FirstOrDefaultAsync(x => x.PromotionId == id);
                if (os != null)
                {
                    os.PromotionName = udto.Name;
                    os.Description = udto.Description;
                    os.StartDate = udto.StartDate;
                    os.EndDate = udto.EndDate;
                    os.DiscountPercentage = udto.DiscountPercentage;
                    os.TermsConditions = udto.TermsConditions;
                    os.Status = udto.Status;

                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Successfully saved");
                }
                return StatusCode(404);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Creates a new promotion in the system.
        /// <param name="crtdto">The promotion details to create.</param>
        /// <returns>Success message or error if creation fails.</returns>
        [Authorize(Roles = "System,Admin,Recept")]
        [HttpPost("CreatePromotion")]
        public async Task<ActionResult<Promotion>> CreatePromotion(promotioncreate crtdto)
        {
            try
            {
                var promotion = new Promotion
                {
                    PromotionName = crtdto.Name,
                    Description = crtdto.Description,
                    StartDate = crtdto.StartDate,
                    TermsConditions = crtdto.TermsConditions,
                    EndDate = crtdto.EndDate,
                    DiscountPercentage = crtdto.DiscountPercentage,
                    RoomId = crtdto.RoomId,
                    Status = crtdto.Status,
                    DateAdded = DateTime.Now
                };

                await _context.Promotions.AddAsync(promotion);
                await _context.SaveChangesAsync();
                return StatusCode(201, "Successfully saved!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Updates only the status of a promotion.
        /// <param name="id">The ID of the promotion to update.</param>
        /// <param name="udto">The updated status information.</param>
        /// <returns>Success message or error if update fails.</returns>
        [Authorize(Roles = "System,Admin,Recept")]
        [HttpPut("UpdateStatusofpromotion/{id}")]
        public async Task<ActionResult<Promotion>> UpdateStatusofpromotion(int id, PromotionStatus udto)
        {
            try
            {
                var os = await _context.Promotions.FirstOrDefaultAsync(x => x.PromotionId == id);
                if (os != null)
                {
                    os.Status = udto.Status;
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Successfully saved");
                }
                return StatusCode(404);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
