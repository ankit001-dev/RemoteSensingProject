using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RemoteSensingProject.Controllers
{
    public class PrashasanController : Controller
    {
        // GET: Prashasan
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult ManageDivision()
        {
            return View();
        }


        public ActionResult ManageOutSource()
        {
            return View();
        }

        public ActionResult ManageManPowerRequest()
        {
            return View();
        }

        public ActionResult Monthly_ManPower_Allocation_Report()
        {
            return View();
        }
    }
}