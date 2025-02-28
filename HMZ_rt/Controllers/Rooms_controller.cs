using HMZ_rt.Models;
using Microsoft.AspNetCore.Mvc;
using HMZ_rt.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography.X509Certificates;

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


        [Authorize(Roles = "System,Admin,Recept")]
        [HttpPost("CreateRoom")]
        public async Task<ActionResult<Room>> CreateRoom(CreateRoom crtm)
        {
            try
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
                if (room != null)
                {
                    await _context.Rooms.AddAsync(room);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, room);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }




        [Authorize(Roles = "System,Admin,Recept")]
        [HttpGet("Admin_Get_Data")]
        public async Task<ActionResult<List<Room>>> GetRoomsAdmin()
        {
            try
            {


                var rooms = await _context.Rooms
                    .Include(r => r.AmenitiesNavigation)
                    .Include(a => a.Promotions)
                    .Include(a => a.Reviews)
                    .Include(a => a.Roominventories)
                    .Include(a => a.Roommaintenances)
                    .Include(a => a.Bookings)
                    .ToListAsync();

                return Ok(rooms);

            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }
        [HttpGet("GetRoomWith")]
        public async Task<ActionResult<List<Room>>> GetRooms()
        {
            try
            {


            var roomsWithAmenities = await _context.Rooms
                .Include(r => r.AmenitiesNavigation)
                .Include(a => a.Promotions)
                .Include(a => a.Reviews)
                .Include(a => a.Roominventories)
                .ToListAsync();

            return Ok(roomsWithAmenities);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }

        [Authorize(Roles = "System,Admin,Recept")]
        [HttpDelete("DeleteRoomById{Id}")]
        public async Task<ActionResult<Room>> DeleteRoomById(int Id)
        {

            try
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
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }
        }

        [Authorize(Roles = "System,Admin,Recept")]
        [HttpPut("SzobaUpdate")]
        public async Task<ActionResult<Room>> UpdateRoomById(int Id, UpdateRoomDto udto)
        {
            try
            {


                var os = await _context.Rooms.FirstOrDefaultAsync(x => x.RoomId == Id);
                if (os != null)
                {

                    os.Status = udto.Status;
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                return BadRequest();
            }

            catch (Exception ex)
            {

                return StatusCode(500,ex);
            }
        } 
            

        
    }
}
