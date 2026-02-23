using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Linq;
using System.Web.Mvc;

namespace RemoteSensingProject.Controllers
{
    [Authorize(Roles = "divisionHead")]
    public class DivisionHeadController : Controller
    {
        private readonly ManagerService _managerServices;
        public DivisionHeadController()
        {
            _managerServices = new ManagerService();
        }
        public ActionResult DivisionHead(string searchTerm)
        {
            int divisionid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).divisionId);
            ViewData["data"] = _managerServices.GetManpowerRequestsInDesignationPmWise(id: divisionid, searchTerm: searchTerm);
            return View();
        }

        public ActionResult ManPower(string searchTerm = null)
        {
            int divisionid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).divisionId);
            ViewData["manpowerrequestsindesignation"] = _managerServices.GetManpowerRequestsInDesignation(id: divisionid, searchTerm: searchTerm);
            return View();
        }
        public ActionResult ManageManpower(int id, string searchTerm = null)
        {
            int divisionid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).divisionId);
            ViewData["data"] = _managerServices.GetManpowerRequestsInDesignationPmWise(id: divisionid, designationid: id, searchTerm: searchTerm);
            return View();
        }

        public ActionResult GetOutsouceOfDivision(int designationid)
        {
            try
            {
                int divisionid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).divisionId);
                var data = _managerServices.OutsourceOfDivision(divisionid, designationid);
                return Json(new
                {
                    status = data.Any(),
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AllocateManPower(AddManPower model)
        {
            try
            {
                // Basic validation
                if (
                    model.PmId == 0 ||
                    model.Outsource == null ||
                    !model.Outsource.Any())
                {
                    return Json(new { status = false, message = "Invalid data" });
                }

                _managerServices.AllocateManpower(model);

                return Json(new
                {
                    status = true,
                    message = "Manpower allocated successfully"
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

        public ActionResult DivisionHeadAllProjectList(string searchTerm = null, string statusFilter = null)
        {
            UserCredential userObj = _managerServices.getManagerDetails(User.Identity.Name);
            ((ControllerBase)this).ViewData["ProjectList"] = _managerServices.All_Project_List(userId: Convert.ToInt32(userObj.divisionId), filterBy: "DivisionHead", searchTerm: searchTerm, statusFilter: statusFilter);
            return View();
        }

        #region Progress Report
        public ActionResult InternalProject_ProgressReportDivision()
        {
            UserCredential userData = _managerServices.getManagerDetails(User.Identity.Name);
            ViewData["projectList"] = _managerServices.All_Project_List(userId: Convert.ToInt32(userData.divisionId), filterBy: "DivisionHead",projectTypeFilter:"Internal", searchTerm: null, statusFilter: null);
            ViewData["reportdata"] = _managerServices.GetMonthlyProjectReport(divisionid:userData.divisionId);
            ViewBag.divisionId = Convert.ToInt32(userData.divisionId);
            return View();
        }
        public ActionResult ExternalProject_ProgressReportDivision()
        {
            UserCredential userData = _managerServices.getManagerDetails(User.Identity.Name);
            ViewData["projectList"] = _managerServices.All_Project_List(userId: Convert.ToInt32(userData.divisionId), filterBy: "DivisionHead",projectTypeFilter:"External", searchTerm: null, statusFilter: null);
            ViewData["reportdata"] = _managerServices.GetMonthlyExternalProjectReport(divisionid: userData.divisionId);
            ViewBag.divisionId = Convert.ToInt32(userData.divisionId);
            return View();
        }

        #endregion
    }
}