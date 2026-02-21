using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RemoteSensingProject.Controllers
{
    public class TechnicalShellController : Controller
    {
        private readonly ManagerService _managerServices;
        public TechnicalShellController()
        {
            _managerServices = new ManagerService();
        }
        // GET: TechnicalShell
        #region Progress Report
        public JsonResult UpdateInternalProject(RemoteSensingProject.Models.Admin.main.TechnicalInternalMonthlyReport data)
        {
            try
            {
                UserCredential userObj = _managerServices.getManagerDetails(User.Identity.Name);
                string message = string.Empty;
                bool res = _managerServices.AddOrUpdateMonthlyInternalProgressReportTechnical(data, out message);
                return Json((object)new
                {
                    status = res,
                    message = (res ? "Monthly updated successfully !" : message)
                });
            }
            catch (Exception ex)
            {
                return Json((object)new
                {
                    status = false,
                    message = "Error: " + ex.Message
                });
            }
        }
        public ActionResult InternalProject_ProgressReportTechnical()
        {
            UserCredential userData = _managerServices.getManagerDetails(User.Identity.Name);
            ViewData["projectList"] = _managerServices.All_Project_List(userId: Convert.ToInt32(userData.userId), filterBy: "ProjectManager", projectTypeFilter: "Internal");
            ViewData["reportdata"] = _managerServices.GetMonthlyProjectReport();
            return View();
        }
        public ActionResult ExternalProject_ProgressReportTechnical()
        {
            UserCredential userData = _managerServices.getManagerDetails(User.Identity.Name);
            ViewData["projectList"] = _managerServices.All_Project_List(userId: Convert.ToInt32(userData.userId), filterBy: "ProjectManager", projectTypeFilter: "External");
            ViewData["reportdata"] = _managerServices.GetMonthlyExternalProjectReport();
            return View();
        }

        #endregion
    }
}