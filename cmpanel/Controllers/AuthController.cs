using Microsoft.AspNetCore.Mvc;

namespace cmpanel.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult forgotPassword()
        {
            return View();
        }
        public IActionResult changePassword()
        {
            return View();
        }
    }
}
