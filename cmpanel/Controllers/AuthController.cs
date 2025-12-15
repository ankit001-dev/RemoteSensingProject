using Microsoft.AspNetCore.Mvc;

namespace cmpanel.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
