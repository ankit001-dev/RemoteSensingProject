using Microsoft.AspNetCore.Mvc;

namespace cmpanel.Controllers
{
    public class CmController : Controller
    {
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult ProjectList()
        {
            return View();
        }
        public ActionResult projectDetails()
        {
            return View();
        }
    }
}
