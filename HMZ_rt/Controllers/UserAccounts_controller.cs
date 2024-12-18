using HMZ_rt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;

namespace HMZ_rt.Controllers
{
    [Route("UserAccounts")]
    [ApiController]
    public class UserAccounts_controller : Controller
    {
        private readonly HmzRtContext _context;
        public UserAccounts_controller(HmzRtContext context)
        {
            _context = context;
        }



        [HttpGet("listoutallusers")]
        public async Task<ActionResult<Useraccount>> Userslist()
        {
            return Ok(await _context.Useraccounts.ToListAsync());

        }





        [HttpPost("NewUserGenerating")]
        public async Task<ActionResult<Useraccount>> NewAccount(CreateUserDto newuser) {


            //ellenőrzés
            var existingUserByUsername = await _context.Useraccounts
            .FirstOrDefaultAsync(u => u.Username == newuser.UserName);

            if (existingUserByUsername != null)
            {
                return BadRequest(new { message = "Ez a felhasználónév már használatban van!" });
            }


            var existingUserByEmail = await _context.Useraccounts
    .FirstOrDefaultAsync(u => u.Email == newuser.Email);

            if (existingUserByEmail != null)
            {
                return BadRequest(new { message = "Ez az E-mail már használatban van!" });
            }

            //mentés
            var user = new Useraccount
            {
                Username = newuser.UserName,
                Password = newuser.Password,
                Email = newuser.Email,
                Role = "Base",
                Status = newuser.Status,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                LastLogin = DateTime.Now,
                Notes = newuser.Notes,
            };
            if (user != null) {
                await _context.Useraccounts.AddAsync(user);
                await _context.SaveChangesAsync();
                return StatusCode(201, user);
            }
            return BadRequest();
        }


        [HttpDelete("DeleteUserById{InUserId}")]
        public async Task<ActionResult<Useraccount>> DeleteAccount(int InUserId)
        {
            var os = await _context.Useraccounts.FirstOrDefaultAsync(x => x.UserId == InUserId);
            if (os != null)
            {
                _context.Useraccounts.Remove(os);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Sikeresen törölve!" });
            }
            return NotFound();
        }


        [HttpDelete("DeleteUserByUsername{Username}")]
        public async Task<ActionResult<Useraccount>> DeleteAccountByName(string Username)
        {
            var os = await _context.Useraccounts.FirstOrDefaultAsync(x => x.Username == Username);
            if (os != null)
            {
                _context.Useraccounts.Remove(os);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Sikeresen törölve!" });
            }
            return NotFound();
        }


        [HttpGet("UsersWithNotificationsFull{UserIdd}")]
        public async Task<ActionResult<Useraccount>> GetAllNotification(int UserIdd)
        {
            var alldat = await _context.Useraccounts.Include(x => x.Notifications).Where(x=> x.UserId == UserIdd).ToListAsync();
            return Ok(alldat);
        }


    }
}
