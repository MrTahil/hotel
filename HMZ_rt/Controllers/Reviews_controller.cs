using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMZ_rt.Controllers
{
    [Route("Reviews")]
    [ApiController]
    public class Reviews_controller : Controller
    {
        private readonly HmzRtContext _context;
        public Reviews_controller(HmzRtContext context)
        {
            _context = context;
        }

        [HttpGet("GetReviewsForRoom/{id}")]
        public async Task<ActionResult<Review>> GetReviForRoom(int id)
        {
            try
            {
                var data = await _context.Reviews.Where(x => x.RoomId == id).ToListAsync();
                if (data != null)
                {
                    return StatusCode(200, data);
                } return StatusCode(200, "Ehhez a szobához még nincsenek visszajelzések.");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }
        [Authorize(Roles ="Admin,Recept,Base,System")]
        [HttpPost("NemComment/{roomid}")]
        public async Task<ActionResult<Review>> NewRevi(int roomid, NewRevi crtdto)
        {
            try
            {
                var data = _context.Rooms.FirstOrDefaultAsync(x => x.RoomId == roomid);
                if (data!= null)
                {
                    var newdata = new Review
                    {
                        ReviewDate = DateTime.Now,
                        GuestId = crtdto.GuestId,
                        RoomId = roomid,
                        Rating = crtdto.Rating,
                        Comment = crtdto.Comment,
                        Status = "OK",
                        Response = "",
                        ResponseDate = DateTime.MinValue,
                        DateAdded = DateTime.Now
                    };
                    if (newdata != null)
                    {
                       await _context.Reviews.AddAsync(newdata);
                        await _context.SaveChangesAsync();
                        return StatusCode(201, "Sikeres kommentelés");
                    } return StatusCode(418, "Valami biza nem jó");

                } return StatusCode(404, "Nem található szoba ezzel az id-val");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }
    }
}
