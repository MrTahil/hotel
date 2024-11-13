using Microsoft.AspNetCore.Mvc;

namespace HMZ_rt.Controllers
{
    public class Rooms_controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
