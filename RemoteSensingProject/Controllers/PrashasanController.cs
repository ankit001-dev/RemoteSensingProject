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


        public ActionResult ManageOutSource(string searchTerm = null)
        {
            ViewData["Designations"] = _adminServices.ListDesgination();
            ViewData["UserList"] = _managerServices.selectAllOutSOurceList(null, null, null, searchTerm);
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
        [HttpPost]
        public ActionResult DeleteOutsource(int id)
        {
            try 
            {
                bool res = _managerServices.DeleteOutSource(id);
                return Json((object)res,(JsonRequestBehavior)0);
            }
            catch(Exception ex)
            {
                throw ex;
            }
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