using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using RemoteSensingProject.Models.TechnicalCell;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using static RemoteSensingProject.Models.TechnicalCell.main;

namespace RemoteSensingProject.Controllers
{
    [Authorize(Roles = "divisionHead")]
    public class DivisionHeadController : Controller
    {
        private readonly ManagerService _managerServices;
        private readonly AdminServices _adminServices;
        private readonly TechnicalCellServices _technicalCellServices;
        public DivisionHeadController()
        {
            _adminServices = new AdminServices();
            _managerServices = new ManagerService();
            _technicalCellServices = new TechnicalCellServices();
        }

        public ActionResult Common()
        {
            ViewData["dynamicformats"] = _managerServices.GetDynamicTablesFormat();
            return View();
        }

        public ActionResult DynamicFormat(int id)
        {
            Models.TechnicalCell.main.DynamicFormate data = _managerServices.GetDynamicTableColumn(id);
            ViewData["data"] = data;
            var userObj = _managerServices.getManagerDetails(User.Identity.Name);
            var dynamicData = _technicalCellServices.GetDynamicData(data.TableName, data.ColumnName, "divisionHead", Convert.ToInt32(userObj.divisionId));
            ViewData["DynamicData"] = dynamicData;
            return View();
        }
        [HttpPost]
        public JsonResult UpdateDynamicReportData(DynamicInsertData data)
        {
            try
            {
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(data.TableRawValue);
                data.Data = dictionary;
                UserCredential userObj = _managerServices.getManagerDetails(User.Identity.Name);
                int divisionid = userObj.divisionId;
                string message = string.Empty;
                bool res = _technicalCellServices.InsertDynamicReportData(data, int.Parse(userObj.userId), userObj.userRole,divisionid);
                return Json((object)new
                {
                    status = res,
                    message = (res ? "Data saved successfully !" : "Some issue occured while processing...")
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

        #region Final Submit
        [HttpGet]
        public ActionResult InternalReportFinalSubmit()
        {
            try
            {
                int userid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
                bool res = _managerServices.FinalSubmitInternalReportDivision(userid);
                return Json(new
                {
                    status = res,
                    message = res ? "Submitted Successfully" : "Something went wrong"
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
        [HttpGet]
        public ActionResult ExternalReportFinalSubmit()
        {
            try
            {
                int userid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
                bool res = _managerServices.FinalSubmitExternalReportDivision(userid);
                return Json(new
                {
                    status = res,
                    message = res ? "Submitted Successfully" : "Something went wrong"
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
        #endregion

        #region Progress Report
        public ActionResult InternalProject_ProgressReportDivision(int? year = null, int? month = null, int? projectManagerId = null)
        {
            UserCredential userData = _managerServices.getManagerDetails(User.Identity.Name);
            ViewData["projectList"] = _managerServices.All_Project_List(userId: Convert.ToInt32(userData.divisionId), filterBy: "DivisionHead", projectTypeFilter: "Internal", searchTerm: null, statusFilter: null);
            ViewData["reportdata"] = _managerServices.GetMonthlyTechnicalInternalProjectReport(divisionid: userData.divisionId, year: year, month: month, projectmanager: projectManagerId, filterby: "divisionhead");
            ViewData["projectmanagerlist"] = _adminServices.BindEmployee().Where(n => n.EmployeeRole != null && n.EmployeeRole.Contains("projectManager")).ToList();
            ViewBag.divisionId = Convert.ToInt32(userData.divisionId);
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
        public ActionResult ExternalProject_ProgressReportDivision(int? year = null, int? month = null, int? projectManagerId = null)
        {
            UserCredential userData = _managerServices.getManagerDetails(User.Identity.Name);
            ViewData["projectList"] = _managerServices.All_Project_List(userId: Convert.ToInt32(userData.divisionId), filterBy: "DivisionHead", projectTypeFilter: "External", searchTerm: null, statusFilter: null);
            ViewData["reportdata"] = _managerServices.GetMonthlyExternalProjectReport(divisionid: userData.divisionId, year: year, month: month, projectmanager: projectManagerId, filterby: "divisionhead");
            ViewBag.divisionId = Convert.ToInt32(userData.divisionId);
            ViewData["projectmanagerlist"] = _adminServices.BindEmployee().Where(n => n.EmployeeRole != null && n.EmployeeRole.Contains("projectManager")).ToList();
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
    }
}