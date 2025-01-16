using HMZ_rt.Models;
using Microsoft.AspNetCore.Mvc;
using HMZ_rt.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HMZ_rt.Controllers
{
    [Route("Rooms")]
    [ApiController]
    public class Rooms_controller : Controller
    {
        private readonly HmzRtContext _context;
        public Rooms_controller(HmzRtContext context)
        {
            _context = context;
        }


        [HttpPost("CreateRoom")]
        public async Task<ActionResult<Room>> CreateRoom(CreateRoom crtm)
        {
            var existingRoomByNumber = await _context.Rooms
            .FirstOrDefaultAsync(u => u.RoomNumber == crtm.RoomNumber);

            if (existingRoomByNumber != null)
            {
                return BadRequest(new { message = "Ez a szobaszám már használatban van!" });
            }


            var room = new Room
            {
                RoomType = crtm.RoomType,
                RoomNumber = crtm.RoomNumber,
                Capacity = crtm.Capacity,
                PricePerNight = crtm.PricePerNight,
                Status = crtm.Status,
                Description = crtm.Description,
                FloorNumber = crtm.FLoorNumber,
                Images = crtm.Images,
                DateAdded = DateTime.Now
            };
            if (room != null) {
                await _context.Rooms.AddAsync(room);
                await _context.SaveChangesAsync();
                return StatusCode(201, room);
            }
            return BadRequest();
        }

        [HttpGet("GetRoomWithoutAmenities")]
        public async Task<ActionResult<Room>> GetRooms()
        {
            return Ok(await _context.Rooms.ToListAsync());
        }



        [HttpDelete("DeleteRoomById{Id}")]
        public async Task<ActionResult<Room>> DeleteRoomById(int Id)
        {


            var os = await _context.Rooms.FirstOrDefaultAsync(x => x.RoomId== Id);
            if (os != null)
            {
                _context.Rooms.Remove(os);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Sikeresen törölve!" });
            }
            return NotFound();
        }

        
    }
}
