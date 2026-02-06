using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RemoteSensingProject.Controllers
{
    [Authorize(Roles = "prashasan")]
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
            PrashasanDashboard data = _managerServices.GetPrashasanDashboardData();
            return View(data);
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
        public ActionResult ManageManPowerRequest(string searchTerm = null)
        {
            ViewData["manpowerrequestsindivision"] = _managerServices.GetManpowerRequestsInDivision(searchTerm: searchTerm);
            return View();
        }

        public ActionResult Monthly_ManPower_Allocation_Report()
        {
            return View();
        }
        public ActionResult AddManpower(int id, string searchTerm = null)
        {
            ViewData["manpowerrequestsindesignation"] = _managerServices.GetManpowerRequestsInDesignation(id:id,searchTerm: searchTerm);
            ViewData["designationList"] = _adminServices.ListDesgination();
            return View();
        }
        [HttpGet]
        public JsonResult OutSourceListByDesignation(int id)
        {
            try
            {
                var data = _managerServices.OutsourceNotInDivision(id);
                return Json(new { status = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { status = false, message=ex.Message });
            }
        }
        [HttpPost]
        public ActionResult AddManPower(AddManPower model)
        {
            try
            {
                // Basic validation
                if (model.DivisionId == 0 ||
                    model.DesignationId == 0 ||
                    model.Outsource == null ||
                    !model.Outsource.Any())
                {
                    return Json(new { status = false, message = "Invalid data" });
                }

                _managerServices.AddManpower(model);

                return Json(new
                {
                    status = true,
                    message = "Manpower added successfully"
                });
            }
            catch (Exception ex)
            {
                // DB validation / business rule message
                return Json(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }
    }
}