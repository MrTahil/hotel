using Microsoft.AspNetCore.Mvc;
#pragma warning disable
namespace HMZ_rt.Controllers
{
    public class Invoices_controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
