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
        [Authorize(Roles = "System,Admin,Recept")]
        [HttpDelete("Deletepromotion/{id}")]
        public async Task<ActionResult<Promotion>> DeletePromotion(int id)
        {
            try
            {
                var promotion =await _context.Promotions.FirstOrDefaultAsync(x => x.PromotionId == id);
                if (promotion != null)
                {
                     _context.Promotions.Remove(promotion);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Sikeres törlés");
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }

        [Authorize(Roles = "System,Admin,Recept")]
        [HttpPut("UpdatePromotion/{id}")]
        public async Task<ActionResult<Promotion>> UpdateRoom(int id, promotionupdate udto)
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
                    return StatusCode(201, "Sikeres mentés");
                }
                return StatusCode(404);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
                throw;
            }
        }
    }
}
