using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RemoteSensingProject.Controllers
{
    public class PrashasanController : Controller
    {
        private readonly AdminServices _adminServices;
        private readonly ManagerService _managerServices;
        public PrashasanController()
        {
            _adminServices = new AdminServices();
            _managerServices = new ManagerService();
        }
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
            ViewData["Designations"] = _adminServices.ListDesgination();
            return View();
        }

        [HttpPost]
        public ActionResult CreateOutSource(OuterSource os)
        {
            bool res = false;
            string message = "Some issue occured !";
            try
            {
                res = _managerServices.insertOutSource(os);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json((object)new
            {
                status = res,
                message = (res ? "Outsource created succesfully !" : message)
            });
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