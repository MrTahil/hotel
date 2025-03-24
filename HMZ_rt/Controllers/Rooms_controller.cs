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

        /// Creates a new room in the system with validation for duplicate room numbers.
        /// <param name="crtm">The room details to create.</param>
        /// <returns>The created room or error if creation fails.</returns>
        [Authorize(Roles = "System,Admin,Recept")]
        [HttpPost("CreateRoom")]
        public async Task<ActionResult<Room>> CreateRoom(CreateRoom crtm)
        {
            try
            {
                // Check if room number already exists
                var existingRoomByNumber = await _context.Rooms
                    .FirstOrDefaultAsync(u => u.RoomNumber == crtm.RoomNumber);

                if (existingRoomByNumber != null)
                {
                    return BadRequest(new { message = "This room number is already in use!" });
                }

                // Create new room object
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

                await _context.Rooms.AddAsync(room);
                await _context.SaveChangesAsync();
                return StatusCode(201, room);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Retrieves all rooms with their related data for administrative purposes.
        /// <returns>A list of all rooms with comprehensive related data.</returns>
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


        /// Retrieves all rooms with basic related data for general viewing.
        /// <returns>A list of all rooms with basic related data.</returns>
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

        /// Deletes a room by ID.
        /// <param name="Id">The ID of the room to delete.</param>
        /// <returns>Success message or error if deletion fails.</returns>
        [Authorize(Roles = "System,Admin,Recept")]
        [HttpDelete("DeleteRoomById/{Id}")]
        public async Task<ActionResult<Room>> DeleteRoomById(int Id)
        {
            try
            {
                var os = await _context.Rooms.FirstOrDefaultAsync(x => x.RoomId == Id);
                if (os != null)
                {
                    _context.Rooms.Remove(os);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Successfully deleted!" });
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        /// Updates a room's status by ID.
        /// <param name="Id">The ID of the room to update.</param>
        /// <param name="udto">The updated room details.</param>
        /// <returns>Success message or error if update fails.</returns>
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
                return StatusCode(500, ex);
            }
        }

        /// Searches for available rooms based on check-in/check-out dates and guest count.
        /// <param name="gtdto">The search parameters including dates and guest number.</param>
        /// <returns>A list of available rooms matching the criteria.</returns>
        [HttpPut("Searchwithparams")]
        public async Task<ActionResult<Room>> GetroomsWithparams(Getrooms gtdto)
        {
            // Find rooms that don't have bookings overlapping with the requested dates
            var availableRooms = _context.Rooms
                .Include(x => x.Bookings)
                .Where(room => !room.Bookings.Any(b =>
                    ((gtdto.CheckInDate >= b.CheckInDate && gtdto.CheckInDate < b.CheckOutDate) ||
                     (gtdto.CheckOutDate > b.CheckInDate && gtdto.CheckOutDate <= b.CheckOutDate) ||
                     (gtdto.CheckInDate <= b.CheckInDate && gtdto.CheckOutDate >= b.CheckOutDate))));

            // Filter by capacity
            return StatusCode(200, availableRooms.Where(x => x.Capacity >= gtdto.GuestNumber));
        }
    }
}
