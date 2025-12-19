using Newtonsoft.Json;
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using static RemoteSensingProject.Models.Admin.main;

namespace RemoteSensingProject.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly AdminServices _adminServices;
        private readonly ManagerService _managerServices;
        private readonly AccountService _accountService;

        public AdminController()
        {
            _adminServices = new AdminServices();
            _managerServices = new ManagerService();
            _accountService = new AccountService();
        }
        // GET: Admin
        public ActionResult Dashboard()
        {
            var TotalCount = _adminServices.DashboardCount();
            DateTime twoYearsAgo = DateTime.Now.AddYears(-2);
            ViewData["physical"] = _adminServices.Project_List().Where(d => d.AssignDate >= twoYearsAgo).ToList();
            ViewData["budgetGraph"] = _adminServices.ViewProjectExpenditure();
            return View(TotalCount);
        }


        public ActionResult ViewExpenditureAmount(string req)
        {
            ViewData["ExpendedData"] = req == "expenditure" ? _adminServices.ViewProjectExpenditure().Where(d => d.expenditure > 0).ToList() : req == "remaining" ? _adminServices.ViewProjectExpenditure().Where(d => d.remaining > 0).ToList() : _adminServices.ViewProjectExpenditure();
            return View();
        }
        public ActionResult BindOverallCompletionPercentage()
        {
            var list = _adminServices.getAllProjectCompletion();
            return Json(new { list = list }, JsonRequestBehavior.AllowGet);

        }
        #region Employee Category
        public ActionResult Emp_Category()
        {
            return View();
        }

        public ActionResult getCategoryPartial(string partial)
        {
            List<CommonResponse> list = new List<CommonResponse>();
            if (partial.Equals("_desgination"))
            {
                list = _adminServices.ListDesgination();
            }
            else if (partial.Equals("_divison"))
            {
                list = _adminServices.ListDivison();
            }
            return PartialView(partial, list);
        }

        [HttpPost]
        public ActionResult InsertDesgination(CommonResponse cr)
        {
            bool res = _adminServices.InsertDesignation(cr);
            return Json(new
            {
                status = res,
                message = res ? "Desgination inserted successfully!" : "Some issue found while processing your request !"
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult InsertDivision(CommonResponse cr)
        {
            bool res = _adminServices.InsertDivison(cr);
            return Json(new
            {
                status = res,
                message = res ? "Divison inserted successfully!" : "Some issue found while processing your request !"
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpDelete]
        public ActionResult removeDivison(int id)
        {
            bool res = _adminServices.removeDivison(id);
            return Json(new
            {
                status = res,
                message = res ? "Divison removed successfully !" : "Some issue occred "
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpDelete]
        public ActionResult removeDesgination(int id)
        {
            bool res = _adminServices.removeDesgination(id);
            return Json(new
            {
                status = res,
                message = res ? "Divison removed successfully !" : "Some issue occred "
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult Employee_Registration(int? division = null, string searchTerm=null)
        {

            ViewBag.division = _adminServices.ListDivison();
            ViewBag.designation = _adminServices.ListDesgination();
            List<Employee_model> empList = new List<Employee_model>();
            
            empList = _adminServices.SelectEmployeeRecord(searchTerm: searchTerm, devision: division);
            ViewData["EmployeeList"] = empList;
            return View();
        }

        [HttpPost]
        public ActionResult Employee_Registration(Employee_model emp)
        {
            string filePage = Server.MapPath("~/ProjectContent/Admin/Employee_Images/");
            if (!Directory.Exists(filePage))
                Directory.CreateDirectory(filePage);
            string path = null;
            if (emp.EmployeeImages != null)
            {
                var fileName = Guid.NewGuid() + DateTime.Now.ToString("ddMMyyyyhhmm") + emp.EmployeeImages.FileName;
                path = Path.Combine("/ProjectContent/Admin/Employee_Images", fileName);
                emp.Image_url = path;
            }


            bool res = _adminServices.AddEmployees(emp,out string message);


            if (res && emp.EmployeeImages != null)
            {

                emp.EmployeeImages.SaveAs(Server.MapPath(path));

            }
            return Json(new
            {
                status = res,
                message = message
            });

        }
        [HttpGet]
        public ActionResult DeleteEmployees(int id)
        {
            var res = _adminServices.RemoveEmployees(id);

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ChangeActieStatus(int id)
        {
            var res = _adminServices.ChangeActieStatus(id);

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult SelectEmployeeRecordById(int id)
        {
            var res = _adminServices.SelectEmployeeRecordById(id);

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        #region add project
        public ActionResult Add_Project()
        {
            var TotalCount = _adminServices.DashboardCount();
            ViewBag.pendingBudget = TotalCount.PendingBudget;
            decimal b = Convert.ToDecimal(ViewBag.pendingBudget);
            var data = _adminServices.SelectEmployeeRecord();
            ViewBag.projectManager = data.Where(d => d.EmployeeRole.Equals("projectManager")).ToList();
            ViewBag.subOrdinateList = data.Where(d => d.EmployeeRole.Equals("subOrdinate")).ToList();

            return View();
        }

        public ActionResult GetProjecDatatById(int Id)
        {
            var data = _adminServices.GetProjectById(Id);
            return Json(new
            {
                status = true,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Project_Detail(int projectId)
        {
            return View();
        }

        [HttpPost]
        public ActionResult InsertProject(createProjectModel pm)
        {
            string filePage = Server.MapPath("~/ProjectContent/Admin/ProjectDocs/");
            if (!Directory.Exists(filePage))
                Directory.CreateDirectory(filePage);
            if (pm.pm.projectDocument != null && pm.pm.projectDocument.FileName != "")
            {
                pm.pm.projectDocumentUrl = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(pm.pm.projectDocument.FileName);
                pm.pm.projectDocumentUrl = Path.Combine("/ProjectContent/Admin/ProjectDocs/", pm.pm.projectDocumentUrl);
            }

            if (pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage)
            {
                foreach (var item in pm.stages)
                {
                    if (item.Stage_Document != null && item.Stage_Document.FileName != "")
                    {
                        item.Document_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(item.Stage_Document.FileName);
                        item.Document_Url = Path.Combine("/ProjectContent/Admin/ProjectDocs/", item.Document_Url);
                    }
                }
            }
            pm.pm.createdBy = "admin";
            bool res = _adminServices.addProject(pm);
            if (res)
            {
                if (pm.pm.projectDocument != null && pm.pm.projectDocument.FileName != "")
                {
                    pm.pm.projectDocument.SaveAs(Server.MapPath(pm.pm.projectDocumentUrl));
                }

                if (pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage)
                {
                    foreach (var item in pm.stages)
                    {
                        if (item.Stage_Document != null && item.Stage_Document.FileName != "")
                        {
                            item.Stage_Document.SaveAs(Server.MapPath(item.Document_Url));
                        }
                    }
                }
            }
            return Json(new
            {
                status = res
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Project_List(string searchTerm = null,string statusFilter=null)
        {
            ViewBag.ProjectList = _adminServices.Project_List(filterType:"ManagerProject",searchTerm:searchTerm,statusFilter:statusFilter);
            return View();
        }
        #endregion

        public ActionResult All_Projects(string searchTerm = null, string statusFilter = null,int? projectManagerFilter = null)
        {
            ViewBag.ManagerList = _adminServices.SelectEmployeeRecord().Where(d => d.EmployeeRole.Equals("projectManager")).ToList();
            
            ViewBag.ProjectList = _adminServices.Project_List(searchTerm:searchTerm,statusFilter:statusFilter,projectManager:projectManagerFilter);
            return View();
        }
        public ActionResult Project_Request()
        {
            return View();
        }
        #region /* Meeting */
        public ActionResult Min_Of_Meeting()
        {

            var empList = _adminServices.BindEmployee();
            return View(empList);
        }

        public ActionResult GetConclusions(int id)
        {
            var res = _adminServices.getConclusion(id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetConclusionOnly(int id, int meetingId)
        {
            var res = _adminServices.getConclusion(meetingId).Where(e => e.Id == id).Select(e => e.Conclusion).FirstOrDefault();
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddMeeting(AddMeeting_Model formData)
        {
            try
            {
                string filePage = Server.MapPath("~/ProjectContent/Admin/Meeting_Attachment/");
                if (!Directory.Exists(filePage))
                    Directory.CreateDirectory(filePage);
                string path = null;
                if (formData.Attachment != null && formData.Attachment.ContentLength > 0)
                {
                    var guid = Guid.NewGuid();
                    var FileExtension = Path.GetExtension(formData.Attachment.FileName);
                    var fileName = $"{guid}{FileExtension}";
                    path = Path.Combine("/ProjectContent/Admin/Meeting_Attachment/", fileName);

                    formData.Attachment_Url = path;
                }
                else
                {
                    return Json(new { success = false, message =
                        "Attachment is required"}, JsonRequestBehavior.AllowGet);
                }
                    bool status = _adminServices.insertMeeting(formData);
                if (status && formData.Attachment != null)
                {
                    formData.Attachment.SaveAs(Server.MapPath(path));
                }
                return Json(new { success = status,message = status? "Meeting add successfully": "Some error occured" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message,data=ex }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult UpdateMeeting(AddMeeting_Model formData)
        {
            string path = null;
            if (formData.Attachment != null && formData.Attachment.ContentLength > 0)
            {
                var guid = Guid.NewGuid();
                var FileExtension = Path.GetExtension(formData.Attachment.FileName);
                var fileName = $"{guid}{FileExtension}";
                path = Path.Combine("/ProjectContent/Admin/ProjectDocs/", fileName);

                formData.Attachment_Url = path;
            }

            bool status = _adminServices.UpdateMeeting(formData);
            if (status && formData.Attachment != null)
            {
                formData.Attachment.SaveAs(Server.MapPath(path));
            }
            return Json(new { success = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetMeetingById(int id)
        {
            var obj = _adminServices.getMeetingById(id);
            return Json(obj, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult GetMemberJoiningStatusByMeeting(int meetingId)
        {

            var res = _managerServices.getMemberJoiningStatus(meetingId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAllMeeting(string searchTerm = null, string statusFilter = null, string meetingMode = null)
        {
            List<Meeting_Model> empList = new List<Meeting_Model>();
            empList = _adminServices.getAllmeeting(searchTerm:searchTerm,statusFilter:statusFilter,meetingMode:meetingMode);
            return Json(new { empList = empList }, JsonRequestBehavior.AllowGet);
        }

        #endregion End Meeting

        public ActionResult MeetingConclusion(int meeting)
        {
            ViewBag.empList = _adminServices.BindEmployee();
            var obj = _adminServices.getMeetingById(meeting);
            ViewBag.getMember = _adminServices.GetMeetingMemberList(meeting);
            ViewBag.MeetingConclusion = _adminServices.getConclusion(meeting);
            return View(obj);
        }
        [HttpPost]
        public ActionResult AddMeetingConclusion(MeetingConclusion mc)
        {
            var res = _adminServices.AddMeetingResponse(mc);
            return Json(res);
        }
        public ActionResult getPresentMember(int id)
        {
            var res = _adminServices.getPresentMember(id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getKeypointResponse(int id)
        {
            var res = _adminServices.getKeypointResponse(id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult View_Weekly_Update(int Id)
        {
            ManagerService ms = new ManagerService();
            ViewData["weeklyUpdate"] = ms.MonthlyProjectUpdate(Id);
            return View();
        }

        public ActionResult View_Project_Stage(int Id)
        {
            ViewBag.ProjectStages = _adminServices.ProjectStagesList(Id);
            return View();
        }
        public ActionResult viewStagesReason(string stageId)
        {
            List<Project_Statge> stageList = new List<Project_Statge>();
            stageList = _managerServices.ViewStagesComments(stageId);
            return Json(new { stageList = stageList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult View_Expenses(int Id)
        {
            ViewBag.ProjectExpenses = _adminServices.ProjectBudgetList(Id);
            return View();
        }
        #region reports
        public ActionResult Project_Report()
        {
            return View();
        }

        public ActionResult Meeting_Report()
        {
            var empList = _adminServices.BindEmployee();

            return View(empList);
        }

        public ActionResult Member_Report(int? division = null, string searchTerm = null)
        {
            ViewBag.MemberList = _adminServices.SelectEmployeeRecord(searchTerm:searchTerm,devision:division);
            ViewBag.DivisonList = _adminServices.ListDivison();
            return View();
        }

        public ActionResult Pending_Project()
        {
            return View();
        }

        public ActionResult Completed_Project()
        {
            return View();
        }
        #endregion
        public ActionResult Generate_Notice()
        {
            ViewBag.ProjectList = _adminServices.Project_List();

            return View();
        }
        [HttpGet]
        public ActionResult GetProjectManagerByProjectId(int? id)
        {
            dynamic projectManager = null;
            if (id.HasValue)
            {

                projectManager = _adminServices.Project_List().Where(e => e.Id == id).FirstOrDefault();
            }

            return Json(projectManager, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult GetProjectById(int? id)
        {
            dynamic project = null;
            if (id.HasValue)
            {

                project = _adminServices.GetProjectById((int)id);

            }

            return Json(project, JsonRequestBehavior.AllowGet);

        }



        public ActionResult AddNotice(Generate_Notice gn)
        {
            string filePage = Server.MapPath("~/ProjectContent/Admin/NoticeDocs/");
            if (!Directory.Exists(filePage))
                Directory.CreateDirectory(filePage);
            string path = null;
            if (gn.Attachment != null)
            {
                var fileName = Guid.NewGuid() + DateTime.Now.ToString("ddMMyyyyhhmm") + gn.Attachment.FileName;
                path = Path.Combine("/ProjectContent/Admin/NoticeDocs", fileName);
                gn.Attachment_Url = path;
            }

            var res = _adminServices.InsertNotice(gn);
            if (res && gn.Attachment != null)
            {
                gn.Attachment.SaveAs(Server.MapPath(path));
            }
            return Json(res);
        }

        public ActionResult Notice_List(int? projectManager,string searchTerm = null)
        {
            dynamic noticeList = null;
            noticeList = _adminServices.getNoticeList(id: projectManager, searchTerm:searchTerm);
            ViewBag.ProjectList = _adminServices.Project_List();
            ViewBag.EmployeeList = _adminServices.SelectEmployeeRecord();
            ViewData["NoticeList"] = noticeList;

            return View();
        }

        public ActionResult View_Notice()
        {
            return View();
        }

        public ActionResult Expense_Report()
        {
            ViewBag.ProjectList = _adminServices.Project_List();
            return View();
        }
        public ActionResult getHeadsByProject(int id)
        {
            List<Project_model> _headList = new List<Project_model>();
            _headList = _adminServices.getHeadByProject(id);
            return Json(new { _headList = _headList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetExpenses(int hId, int pId)
        {
            List<ProjectExpenses> list = new List<ProjectExpenses>();
            list = _managerServices.ExpencesList(hId, pId);
            return Json(new { list = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SubOrdinateProblemList(string searchTerm = null)
        {
            ViewBag.ProjectProblemList = _managerServices.getSubOrdinateProblemforAdmin(searchTerm: searchTerm);
            return View();
        }
        [HttpGet]
        public ActionResult getsubproblembyid(int id)
        {
            var data = _managerServices.getSubOrdinateProblemforAdmin(id:id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReimbursementRequest(int? projectManagerFilter = null,string typeFilter = null)
        {
            ViewData["ReimBurseData"] = _managerServices.GetReimbursements(managerId:projectManagerFilter,typeFilter:typeFilter,type: "selectAll");
            ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
            return View();
        }
        [HttpPost]
        public ActionResult ReimbursementReque(bool status, int id, string type, string remark)
        {
            bool res = _adminServices.ReimbursementApproval(status, id, type, remark);
            if (res)
            {
                return Json(new
                {
                    status = res,
                    message = (res == true && status == true) ? "Approved Successfully" : "Rejected Successfully"
                });
            }
            else
            {
                return Json(new
                {
                    status = res,
                    message = "Some error Occured"
                });
            }
        }
        public ActionResult TravelRequest(int? managerFilter = null, int? projectFilter = null)
        {
            var res = _adminServices.getAllTourList(null, null, managerFilter: managerFilter, projectFilter: projectFilter);
            ViewData["allTourList"] = res;
            ViewData["projects"] = _adminServices.Project_List();
            ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
            return View();
        }

        [HttpPost]
        public ActionResult TravelRequest(bool status, int id, string remark)
        {
            bool res = _adminServices.Tourapproval(id, status, remark);
            if (res)
            {
                return Json(new
                {
                    status = res,
                    message = (res == true && status == true) ? "Approved Successfully" : "Rejected Successfully"
                });
            }
            else
            {
                return Json(new
                {
                    status = res,
                    message = "Some error Occured"
                });
            }
        }

        public ActionResult HiringRequest(int? managerFilter = null,int? projectFilter = null)
        {
            ViewData["hiringList"] = _adminServices.HiringList(null, null,managerFilter,projectFilter);
            ViewData["projects"] = _adminServices.Project_List();
            ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
            return View();
        }
        [HttpPost]
        public ActionResult HiringRequest(bool status, int id, string remark, string latitude, string longitude)
        {
            dynamic location = _adminServices.GetCityStateAsync(latitude, longitude).GetAwaiter().GetResult();
            //location = location.toString();x
            bool res = _adminServices.HiringApproval(id, status, remark, location);
            if (res)
            {
                return Json(new
                {
                    status = res,
                    message = (res == true && status == true) ? "Approved Successfully" : "Rejected Successfully"
                });
            }
            else
            {
                return Json(new
                {
                    status = res,
                    message = "Some error Occured"
                });
            }
        }
        public ActionResult Reimbursement_Report(int? projectManagerFilter = null, string typeFilter = null,string statusFilter = null)
        {
            ViewData["totalProjectManager"] = _adminServices.SelectEmployeeRecord().Where(d => d.EmployeeRole.Equals("projectManager")).ToList();
            List<Reimbursement> data = _managerServices.GetReimbursements(type: "selectReinbursementReport", managerId: projectManagerFilter, typeFilter: typeFilter, statusFilter:statusFilter);
            
            ViewData["totalReinursementReport"] = data;
            return View();
        }
        public ActionResult TourProposal_Report(int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
        {
            ViewData["allTourList"] = _accountService.getTourList(managerFilter:managerFilter,projectFilter:projectFilter,statusFilter:statusFilter);
            ViewData["projects"] = _adminServices.Project_List();
            ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
            return View();
        }
        public ActionResult Hiring_Report(int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
        {
            ViewData["hiringList"] = _adminServices.HiringReort(managerFilter:managerFilter,projectFilter:projectFilter,statusFilter:statusFilter);
            ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
            ViewData["projects"] = _adminServices.Project_List();
            return View();
        }

        public ActionResult RaisedProblem()
        {
            ViewData["problemList"] = _adminServices.getProblemList();
            return View();
        }

        public ActionResult Attendance()
        {
            ViewData["EmployeeList"] = _adminServices.SelectEmployeeRecord().Where(d => d.EmployeeRole == "projectManager").ToList();
            return View();
        }
        public ActionResult Attendance_Report()
        {
            ViewData["EmployeeList"] = _adminServices.SelectEmployeeRecord().Where(d => d.EmployeeRole == "projectManager").ToList();
            return View();
        }
        public ActionResult ExportAttendanceToExcel(int month, int year, int EmpId, int projectManager)
        {
            var data = _managerServices.ConvertExcelFile(month, year, projectManager, EmpId);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExcelReport.xlsx");
        }
        public ActionResult ShowAllAttendance(int year, int month, int projectManager)
        {
            var data = _managerServices.GetAllAttendanceForPmByMonth(year, month, projectManager);
            ViewData["showAllAtt"] = data;
            ViewData["AllAttendance"] = JsonConvert.SerializeObject(data); // 'data' is List<AllAttendance>
            ViewData["Year"] = year;
            ViewData["Month"] = month;
            return View();
        }
        public ActionResult ExportAllAtt(int year, int month, int projectManager)
        {
            var data = _managerServices.ConvertExcelFileOfAll(month, year, projectManager);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExcelReportOfAll.xlsx");
        }
        public ActionResult EmployeeMonthlyReport()
        {
            ViewBag.MemberList = _adminServices.SelectEmployeeRecord();
            return View();
        }
        [HttpGet]
        public ActionResult EmployeeReportMon(int id,int?month=null,int?year=null)
        {
            try
            {
                var data = _managerServices.GetEmpReport(id,month:month,year:year);
                return Json(new
                {
                    status = data != null ? true : false,
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
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