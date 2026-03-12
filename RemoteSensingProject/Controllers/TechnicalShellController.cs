using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using RemoteSensingProject.Models.TechnicalCell;
using System;
using System.Globalization;
using System.Web.Mvc;
using static RemoteSensingProject.Models.TechnicalCell.main;

namespace RemoteSensingProject.Controllers
{
    public class TechnicalShellController : Controller
    {
        private readonly ManagerService _managerServices;
        private readonly AdminServices _adminServices;
        private readonly TechnicalCellServices _technicalCellServices;
        public TechnicalShellController()
        {
            _managerServices = new ManagerService();
            _adminServices = new AdminServices();
            _technicalCellServices = new TechnicalCellServices();
        }

        public ActionResult Common()
        {
            return View();
        }

        public ActionResult DynamicFormat()
        {
            return View();
        }
        // GET: TechnicalShell
        #region Progress Report
        public JsonResult UpdateTechnicalInternalProject(RemoteSensingProject.Models.Admin.main.TechnicalInternalMonthlyReport data)
        {
            try
            {
                UserCredential userObj = _managerServices.getManagerDetails(User.Identity.Name);
                data.CreaterId = userObj.userId;
                data.CreaterRole = userObj.userRole;
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
        public ActionResult InternalProject_ProgressReportTechnical(int?year = null,int? month = null,int? division = null)
        {
            UserCredential userData = _managerServices.getManagerDetails(User.Identity.Name);
            ViewData["projectList"] = _managerServices.All_Project_List(userId: Convert.ToInt32(userData.userId), filterBy: "ProjectManager", projectTypeFilter: "Internal");
            ViewData["reportdata"] = _managerServices.GetMonthlyTechnicalInternalProjectReportSchemeWise(month:month,year:year,divisionid:division,filterby: "technicalcell");
            ViewData["DivisionList"] = _adminServices.ListDivison();
            if (month.HasValue && month >= 1 && month <= 12)
            {
                ViewBag.Month = new CultureInfo("hi-IN")
                    .DateTimeFormat
                    .GetMonthName(month.Value);
            }
            else
            {
                ViewBag.Month = new CultureInfo("hi-IN").DateTimeFormat.GetMonthName(DateTime.Now.Month); ViewBag.Year = year;
            }
            if (year.HasValue)
            {
                ViewBag.Year = year;
            }
            else
            {
                ViewBag.Year = DateTime.Now.Year;
            }
            return View();
        }
        public ActionResult ExternalProject_ProgressReportTechnical(int? year = null, int? month = null, int? division = null)
        {
            UserCredential userData = _managerServices.getManagerDetails(User.Identity.Name);
            ViewData["projectList"] = _managerServices.All_Project_List(userId: Convert.ToInt32(userData.userId), filterBy: "ProjectManager", projectTypeFilter: "External");
            ViewData["reportdata"] = _managerServices.GetMonthlyExternalProjectReport(month:month,year:year,divisionid:division,filterby: "technicalcell");
            ViewData["DivisionList"] = _adminServices.ListDivison();
            if (month.HasValue && month >= 1 && month <= 12)
            {
                ViewBag.Month = new CultureInfo("hi-IN")
                    .DateTimeFormat
                    .GetMonthName(month.Value);
            }
            else
            {
                ViewBag.Month = new CultureInfo("hi-IN").DateTimeFormat.GetMonthName(DateTime.Now.Month); ViewBag.Year = year;
            }
            if (year.HasValue)
            {
                ViewBag.Year = year;
            }
            else
            {
                ViewBag.Year = DateTime.Now.Year;
            }
            return View();
        }

        #endregion

        public ActionResult Generate_formate()
        {
            return View();
        }

        public ActionResult SavedynamicFormate(DynamicFormate data)
        {
            try
            {
                data.TableName = "tbl_dynamicFormate_" + Guid.NewGuid().ToString("N");
                bool result = _technicalCellServices.CreateDynamicReport(data);
                return Json(new
                {
                    status = result,
                    message = result ? "Dynamic report created successfully !" : "Some issue occured while creating dynamic report."
                });
            }catch(Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = ex.Message
                });
            }
                
        }
    }
}