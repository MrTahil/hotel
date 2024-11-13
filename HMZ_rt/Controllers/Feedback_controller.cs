using Microsoft.AspNetCore.Mvc;

namespace HMZ_rt.Controllers
{
    public class Feedback_controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
