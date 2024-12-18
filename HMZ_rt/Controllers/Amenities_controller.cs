using HMZ_rt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMZ_rt.Controllers
{
    [Route("Amenities")]
    [ApiController]
    public class Amenities_controller : Controller
    {

        //TODO !!!!!
        [HttpPost("New aminiti{Amenity}")]
        public async Task<ActionResult<Amenity>> GenerateNewAmenitie(string Amenity)
        {
            string[] tipes = Amenity.Split(';');
            foreach (var s in tipes) {
                switch (s)
                {
                    
                    case "klima":
                        var ne = new Amenity {
                        AmenityName = s,

                        };
                        return Ok(ne);
                    default:return BadRequest();
                }
               
            } 
            return BadRequest();
        }


    }
}
