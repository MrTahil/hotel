using HMZ_rt.Models;
using Microsoft.AspNetCore.Mvc;
using HMZ_rt.Controllers;

namespace HMZ_rt.Controllers
{
    public class Rooms_controller : Controller
    {
        [HttpGet]
        public async Task<ActionResult<Room>> CreateNewRoom()
        {
            return Ok();
        }
    }
}
