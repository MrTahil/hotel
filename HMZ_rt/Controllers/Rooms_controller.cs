using HMZ_rt.Models;
using Microsoft.AspNetCore.Mvc;
using HMZ_rt.Controllers;

namespace HMZ_rt.Controllers
{
    [Route("Rooms")]
    [ApiController]
    public class Rooms_controller : Controller
    {
        [HttpGet]
        public async Task<ActionResult<Room>> CreateNewRoom()
        {
            return Ok();
        }
    }
}
