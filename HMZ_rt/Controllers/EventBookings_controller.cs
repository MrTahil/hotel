using Microsoft.AspNetCore.Mvc;

namespace HMZ_rt.Controllers
{
    public class EventBookings_controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
