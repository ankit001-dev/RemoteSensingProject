using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RemoteSensingProject.Controllers
{
    public class CMController : Controller
    {
        // GET: CM
        public ActionResult Dashboard()
        {
            return View();
        }
    }
}