using DocumentFormat.OpenXml.Office2010.Excel;
using RemoteSensingProject.Models.ProjectManager;
using System;
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
        public JsonResult UpdateTechnicalInternalProject(RemoteSensingProject.Models.Admin.main.TechnicalInternalMonthlyReport data)
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
        [HttpGet]
        public ActionResult GetInternalTechnicalProjectReport(int id, string type)
        {
            try
            {
                var data = type.Trim().Equals("editid") ? _managerServices.GetMonthlyTechnicalInternalProjectReport(id) : _managerServices.GetMonthlyTechnicalInternalProjectReport(projectid: id);
                return Json((object)new
                {
                    status = data.Count > 0 ? true : false,
                    data = data
                }, (JsonRequestBehavior)0);
            }
            catch (Exception ex)
            {
                return Json((object)new
                {
                    status = false,
                    message = "Error: " + ex.Message
                }, (JsonRequestBehavior)0);
            }
        }
        public ActionResult InternalProject_ProgressReportTechnical()
        {
            UserCredential userData = _managerServices.getManagerDetails(User.Identity.Name);
            ViewData["projectList"] = _managerServices.All_Project_List(userId: Convert.ToInt32(userData.userId), filterBy: "ProjectManager", projectTypeFilter: "Internal");
            ViewData["reportdata"] = _managerServices.GetMonthlyTechnicalInternalProjectReport();
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