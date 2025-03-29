using HMZ_rt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HMZ_rt.Controllers
{
    [Route("Staff")]
    [ApiController]
    public class Staff_controller : Controller
    {
        private readonly HmzRtContext _context;
        public Staff_controller(HmzRtContext context)
        {
            _context = context;
        }



        [Authorize(Roles = "System,Admin")]
        [HttpPost("CreateStaff")]
        public async Task<ActionResult<Staff>> CreateStaff(NewStaffDto nwstdto)
        {
            try
            {


            var dolgozo = new Staff
            {
                FirstName = nwstdto.FirstName,
                LastName = nwstdto.LastName,
                Email = nwstdto.Email,
                PhoneNumber = nwstdto.PhoneNumber,
                Position = nwstdto.Position,
                Salary = nwstdto.Salary,
                DateHired = DateTime.Now,
                Status = nwstdto.Status,
                Department = nwstdto.Departmen
            };
            if (nwstdto != null)
            {
                await _context.Staff.AddAsync(dolgozo);
                await _context.SaveChangesAsync();
                return StatusCode(201, dolgozo);
            }
            return BadRequest();
        }
            catch (Exception ex)
            {

                return StatusCode(500, new { ex
    });
            }


            
        }
        [Authorize(Roles = "System,Admin,Recept")]
        [HttpGet("ListStaff")]
        public async Task<ActionResult<Staff>> Getstaff()
        {
            try
            {

            
            return StatusCode(200, await _context.Staff.ToListAsync());
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { ex });
            }
        }

        [Authorize(Roles ="System,Admin,Recept")]
        [HttpPut("UpdateStaff")]
        public async Task<ActionResult<Staff>> UpdateStaff(UpdateStaffDto crtdto, int id)
        {
            try
            {
                var os = await _context.Staff.FirstOrDefaultAsync(x => x.StaffId == id);
                if (os != null)
                {
                    os.Salary = crtdto.Salary;
                    os.PhoneNumber = crtdto.PhoneNumber;
                    os.FirstName = crtdto.FirstName;
                    os.LastName = crtdto.LastName;
                    os.Email = crtdto.Email;
                    os.Department = crtdto.Department;
                    os.Position = crtdto.Position;
                    os.Status = crtdto.Status;

                    await _context.SaveChangesAsync();
                    return Ok();
                }
                return StatusCode(404, "Nem található a felhasználó");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex });
            }
        }
        [Authorize(Roles ="Admin,Recept,System")]
        [HttpDelete("DeleteStaff/{id}")]
        public async Task<ActionResult<Staff>> DeleteStafff(int id)
        {
            try
            {
                var user = await _context.Staff.FirstOrDefaultAsync(x => x.StaffId == id);
                if (user == null)
                    return NotFound(new { message = "Felhasználó nem található" });

                _context.Staff.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Sikeres törlés" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex });
            }
        }


    }
}
