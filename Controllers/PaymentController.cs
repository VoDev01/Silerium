using Microsoft.AspNetCore.Mvc;

namespace Silerium.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
