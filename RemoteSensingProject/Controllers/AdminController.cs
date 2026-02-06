// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Controllers.AdminController
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using RemoteSensingProject.Models.SubOrdinate;

namespace RemoteSensingProject.Controllers
{

    [Authorize(Roles = "admin,prashasan")]
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

        public ActionResult Dashboard()
        {
            RemoteSensingProject.Models.Admin.main.DashboardCount TotalCount = _adminServices.DashboardCount();
            DateTime twoYearsAgo = DateTime.Now.AddYears(-2);
            ((ControllerBase)this).ViewData["physical"] = (from d in _adminServices.Project_List()
                                                           where d.AssignDate >= twoYearsAgo
                                                           select d).ToList();
            ((ControllerBase)this).ViewData["budgetGraph"] = _adminServices.ViewProjectExpenditure();
            return View((object)TotalCount);
        }

        public ActionResult ViewExpenditureAmount(string req)
        {
            ((ControllerBase)this).ViewData["ExpendedData"] = ((req == "expenditure") ? (from d in _adminServices.ViewProjectExpenditure()
                                                                                         where d.expenditure > 0m
                                                                                         select d).ToList() : ((req == "remaining") ? (from d in _adminServices.ViewProjectExpenditure()
                                                                                                                                       where d.remaining > 0m
                                                                                                                                       select d).ToList() : _adminServices.ViewProjectExpenditure()));
            return View();
        }

        public ActionResult BindOverallCompletionPercentage()
        {
            List<RemoteSensingProject.Models.Admin.main.DashboardCount> list = _adminServices.getAllProjectCompletion();
            return Json((object)new { list }, (JsonRequestBehavior)0);
        }

        public ActionResult Emp_Category()
        {
            return View();
        }

        public ActionResult getCategoryPartial(string partial)
        {
            List<RemoteSensingProject.Models.Admin.main.CommonResponse> list = new List<RemoteSensingProject.Models.Admin.main.CommonResponse>();
            if (partial.Equals("_desgination"))
            {
                list = _adminServices.ListDesgination();
            }
            else if (partial.Equals("_divison"))
            {
                list = _adminServices.ListDivison();
            }
            return PartialView(partial, (object)list);
        }

        [HttpPost]
        public ActionResult InsertDesgination(RemoteSensingProject.Models.Admin.main.CommonResponse cr)
        {
            bool res = _adminServices.InsertDesignation(cr);
            return Json((object)new
            {
                status = res,
                message = (res ? "Desgination inserted successfully!" : "Some issue found while processing your request !")
            }, (JsonRequestBehavior)0);
        }

        [HttpPost]
        public ActionResult InsertDivision(RemoteSensingProject.Models.Admin.main.CommonResponse cr)
        {
            bool res = _adminServices.InsertDivison(cr);
            return Json((object)new
            {
                status = res,
                message = (res ? "Divison inserted successfully!" : "Some issue found while processing your request !")
            }, (JsonRequestBehavior)0);
        }

        [HttpDelete]
        public ActionResult removeDivison(int id)
        {
            bool res = _adminServices.removeDivison(id);
            return Json((object)new
            {
                status = res,
                message = (res ? "Divison removed successfully !" : "Some issue occred ")
            }, (JsonRequestBehavior)0);
        }

        [HttpDelete]
        public ActionResult removeDesgination(int id)
        {
            bool res = _adminServices.removeDesgination(id);
            return Json((object)new
            {
                status = res,
                message = (res ? "Divison removed successfully !" : "Some issue occred ")
            }, (JsonRequestBehavior)0);
        }

        public ActionResult Employee_Registration(int? division = null, string searchTerm = null)
        {
            ((dynamic)((ControllerBase)this).ViewBag).division = _adminServices.ListDivison();
            ((dynamic)((ControllerBase)this).ViewBag).designation = _adminServices.ListDesgination();
            List<RemoteSensingProject.Models.Admin.main.Employee_model> empList = new List<RemoteSensingProject.Models.Admin.main.Employee_model>();
            empList = _adminServices.SelectEmployeeRecord(null, null, searchTerm, division);
            ((ControllerBase)this).ViewData["EmployeeList"] = empList;
            return View();
        }

        [HttpPost]
        public ActionResult Employee_Registration(RemoteSensingProject.Models.Admin.main.Employee_model emp)
        {
            string filePage = ((Controller)this).Server.MapPath("~/ProjectContent/Admin/Employee_Images/");
            if (!Directory.Exists(filePage))
            {
                Directory.CreateDirectory(filePage);
            }
            string path = null;
            if (emp.EmployeeImages != null)
            {
                string fileName = Guid.NewGuid().ToString() + DateTime.Now.ToString("ddMMyyyyhhmm") + emp.EmployeeImages.FileName;
                path = (emp.Image_url = Path.Combine("/ProjectContent/Admin/Employee_Images", fileName));
            }
            string message;
            bool res = _adminServices.AddEmployees(emp, out message);
            if (res && emp.EmployeeImages != null)
            {
                emp.EmployeeImages.SaveAs(((Controller)this).Server.MapPath(path));
            }
            return Json((object)new
            {
                status = res,
                message = message
            });
        }

        [HttpGet]
        public ActionResult DeleteEmployees(int id)
        {
            bool res = _adminServices.RemoveEmployees(id);
            return Json((object)res, (JsonRequestBehavior)0);
        }

        [HttpGet]
        public ActionResult ChangeActieStatus(int id)
        {
            bool res = _adminServices.ChangeActieStatus(id);
            return Json((object)res, (JsonRequestBehavior)0);
        }

        [HttpGet]
        public ActionResult SelectEmployeeRecordById(int id)
        {
            RemoteSensingProject.Models.Admin.main.Employee_model res = _adminServices.SelectEmployeeRecordById(id);
            return Json((object)res, (JsonRequestBehavior)0);
        }

        public ActionResult Add_Project()
        {
            RemoteSensingProject.Models.Admin.main.DashboardCount TotalCount = _adminServices.DashboardCount();
            ((dynamic)((ControllerBase)this).ViewBag).pendingBudget = TotalCount.PendingBudget;
            decimal b = Convert.ToDecimal(((dynamic)((ControllerBase)this).ViewBag).pendingBudget);
            List<RemoteSensingProject.Models.Admin.main.Employee_model> data = _adminServices.SelectEmployeeRecord();
            ((dynamic)((ControllerBase)this).ViewBag).projectManager = data.Where((RemoteSensingProject.Models.Admin.main.Employee_model d) => d.EmployeeRole.Equals("projectManager")).ToList();
            ((dynamic)((ControllerBase)this).ViewBag).subOrdinateList = data.Where((RemoteSensingProject.Models.Admin.main.Employee_model d) => d.EmployeeRole.Equals("subOrdinate")).ToList();
            List<RemoteSensingProject.Models.Admin.main.BudgetHeadModel> budgetHeads = _adminServices.GetBudgetHeads();
            ((ControllerBase)this).ViewData["Designations"] = _adminServices.ListDesgination();
            ((ControllerBase)this).ViewData["BudgetHeads"] = budgetHeads;
            return View();
        }

        public ActionResult GetProjecDatatById(int Id)
        {
            RemoteSensingProject.Models.Admin.main.createProjectModel data = _adminServices.GetProjectById(Id);
            return Json((object)new
            {
                status = true,
                data = data
            }, (JsonRequestBehavior)0);
        }

        public ActionResult Project_Detail(int projectId)
        {
            return View();
        }

        [HttpPost]
        public ActionResult InsertProject(RemoteSensingProject.Models.Admin.main.createProjectModel pm)
        {
            string filePage = ((Controller)this).Server.MapPath("~/ProjectContent/Admin/ProjectDocs/");
            if (!Directory.Exists(filePage))
            {
                Directory.CreateDirectory(filePage);
            }
            if (pm.pm.projectDocument != null && pm.pm.projectDocument.FileName != "")
            {
                pm.pm.projectDocumentUrl = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(pm.pm.projectDocument.FileName);
                pm.pm.projectDocumentUrl = Path.Combine("/ProjectContent/Admin/ProjectDocs/", pm.pm.projectDocumentUrl);
            }
            if (pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage)
            {
                foreach (RemoteSensingProject.Models.Admin.main.Project_Statge item in pm.stages)
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
                    pm.pm.projectDocument.SaveAs(((Controller)this).Server.MapPath(pm.pm.projectDocumentUrl));
                }
                if (pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage)
                {
                    foreach (RemoteSensingProject.Models.Admin.main.Project_Statge item2 in pm.stages)
                    {
                        if (item2.Stage_Document != null && item2.Stage_Document.FileName != "")
                        {
                            item2.Stage_Document.SaveAs(((Controller)this).Server.MapPath(item2.Document_Url));
                        }
                    }
                }
            }
            return Json((object)new
            {
                status = res
            }, (JsonRequestBehavior)0);
        }

        public ActionResult Project_List(string searchTerm = null, string statusFilter = null)
        {
            ((dynamic)((ControllerBase)this).ViewBag).ProjectList = _adminServices.Project_List(null, null, "ManagerProject", searchTerm, statusFilter);
            return View();
        }

        public ActionResult All_Projects(string searchTerm = null, string statusFilter = null, int? projectManagerFilter = null)
        {
            ((dynamic)((ControllerBase)this).ViewBag).ManagerList = (from d in _adminServices.SelectEmployeeRecord()
                                                                     where d.EmployeeRole.Equals("projectManager")
                                                                     select d).ToList();
            object viewBag = ((ControllerBase)this).ViewBag;
            AdminServices adminServices = _adminServices;
            int? projectManager = projectManagerFilter;
            ((dynamic)viewBag).ProjectList = adminServices.Project_List(null, null, null, searchTerm, statusFilter, projectManager);
            return View();
        }

        public ActionResult Project_Request()
        {
            return View();
        }

        public ActionResult Min_Of_Meeting()
        {
            List<RemoteSensingProject.Models.Admin.main.Employee_model> empList = _adminServices.BindEmployee();
            return View((object)empList);
        }

        public ActionResult GetConclusions(int id)
        {
            List<RemoteSensingProject.Models.Admin.main.MeetingConclusion> res = _adminServices.getConclusion(id);
            return Json((object)res, (JsonRequestBehavior)0);
        }

        public ActionResult GetConclusionOnly(int id, int meetingId)
        {
            string res = (from e in _adminServices.getConclusion(meetingId)
                          where e.Id == id
                          select e.Conclusion).FirstOrDefault();
            return Json((object)res, (JsonRequestBehavior)0);
        }

        [HttpPost]
        public ActionResult AddMeeting(RemoteSensingProject.Models.Admin.main.AddMeeting_Model formData)
        {
            try
            {
                string filePage = ((Controller)this).Server.MapPath("~/ProjectContent/Admin/Meeting_Attachment/");
                if (!Directory.Exists(filePage))
                {
                    Directory.CreateDirectory(filePage);
                }
                string path = null;
                if (formData.Attachment != null && formData.Attachment.ContentLength > 0)
                {
                    Guid guid = Guid.NewGuid();
                    string FileExtension = Path.GetExtension(formData.Attachment.FileName);
                    string fileName = $"{guid}{FileExtension}";
                    path = (formData.Attachment_Url = Path.Combine("/ProjectContent/Admin/Meeting_Attachment/", fileName));
                    bool status = _adminServices.insertMeeting(formData);
                    if (status && formData.Attachment != null)
                    {
                        formData.Attachment.SaveAs(((Controller)this).Server.MapPath(path));
                    }
                    return Json((object)new
                    {
                        success = status,
                        message = (status ? "Meeting add successfully" : "Some error occured")
                    }, (JsonRequestBehavior)0);
                }
                return Json((object)new
                {
                    success = false,
                    message = "Attachment is required"
                }, (JsonRequestBehavior)0);
            }
            catch (Exception ex)
            {
                return Json((object)new
                {
                    success = false,
                    message = ex.Message,
                    data = ex
                }, (JsonRequestBehavior)0);
            }
        }

        [HttpPost]
        public ActionResult UpdateMeeting(RemoteSensingProject.Models.Admin.main.AddMeeting_Model formData)
        {
            string path = null;
            if (formData.Attachment != null && formData.Attachment.ContentLength > 0)
            {
                Guid guid = Guid.NewGuid();
                string FileExtension = Path.GetExtension(formData.Attachment.FileName);
                string fileName = $"{guid}{FileExtension}";
                path = (formData.Attachment_Url = Path.Combine("/ProjectContent/Admin/ProjectDocs/", fileName));
            }
            bool status = _adminServices.UpdateMeeting(formData);
            if (status && formData.Attachment != null)
            {
                formData.Attachment.SaveAs(((Controller)this).Server.MapPath(path));
            }
            return Json((object)new
            {
                success = status
            }, (JsonRequestBehavior)0);
        }

        [HttpGet]
        public ActionResult GetMeetingById(int id)
        {
            RemoteSensingProject.Models.Admin.main.Meeting_Model obj = _adminServices.getMeetingById(id);
            return Json((object)obj, (JsonRequestBehavior)0);
        }

        [HttpGet]
        public ActionResult GetMemberJoiningStatusByMeeting(int meetingId)
        {
            List<RemoteSensingProject.Models.Admin.main.Employee_model> res = _managerServices.getMemberJoiningStatus(meetingId);
            return Json((object)res, (JsonRequestBehavior)0);
        }

        [HttpGet]
        public ActionResult GetAllMeeting(string searchTerm = null, string statusFilter = null, string meetingMode = null)
        {
            List<RemoteSensingProject.Models.Admin.main.Meeting_Model> empList = new List<RemoteSensingProject.Models.Admin.main.Meeting_Model>();
            empList = _adminServices.getAllmeeting(null, null, searchTerm, statusFilter, meetingMode);
            return Json((object)new { empList }, (JsonRequestBehavior)0);
        }

        public ActionResult MeetingConclusion(int meeting)
        {
            ((dynamic)((ControllerBase)this).ViewBag).empList = _adminServices.BindEmployee();
            RemoteSensingProject.Models.Admin.main.Meeting_Model obj = _adminServices.getMeetingById(meeting);
            ((dynamic)((ControllerBase)this).ViewBag).getMember = _adminServices.GetMeetingMemberList(meeting);
            ((dynamic)((ControllerBase)this).ViewBag).MeetingConclusion = _adminServices.getConclusion(meeting);
            return View((object)obj);
        }

        [HttpPost]
        public ActionResult AddMeetingConclusion(RemoteSensingProject.Models.Admin.main.MeetingConclusion mc)
        {
            bool res = _adminServices.AddMeetingResponse(mc);
            return Json((object)res);
        }

        public ActionResult getPresentMember(int id)
        {
            List<RemoteSensingProject.Models.Admin.main.Employee_model> res = _adminServices.getPresentMember(id);
            return Json((object)res, (JsonRequestBehavior)0);
        }

        public ActionResult getKeypointResponse(int id)
        {
            List<RemoteSensingProject.Models.Admin.main.KeyPointResponse> res = _adminServices.getKeypointResponse(id);
            return Json((object)res, (JsonRequestBehavior)0);
        }

        public ActionResult View_Weekly_Update(int Id)
        {
            ManagerService ms = new ManagerService();
            ((ControllerBase)this).ViewData["weeklyUpdate"] = ms.MonthlyProjectUpdate(Id);
            return View();
        }

        public ActionResult View_Project_Stage(int Id)
        {
            ((dynamic)((ControllerBase)this).ViewBag).ProjectStages = _adminServices.ProjectStagesList(Id);
            return View();
        }

        public ActionResult viewStagesReason(string stageId)
        {
            List<RemoteSensingProject.Models.Admin.main.Project_Statge> stageList = new List<RemoteSensingProject.Models.Admin.main.Project_Statge>();
            stageList = _managerServices.ViewStagesComments(stageId);
            return Json((object)new { stageList }, (JsonRequestBehavior)0);
        }

        public ActionResult View_Expenses(int Id)
        {
            ((dynamic)((ControllerBase)this).ViewBag).ProjectExpenses = _adminServices.ProjectBudgetList(Id);
            return View();
        }

        public ActionResult Project_Report()
        {
            return View();
        }

        public ActionResult Meeting_Report()
        {
            List<RemoteSensingProject.Models.Admin.main.Employee_model> empList = _adminServices.BindEmployee();
            return View((object)empList);
        }

        public ActionResult Member_Report(int? division = null, string searchTerm = null)
        {
            ((dynamic)((ControllerBase)this).ViewBag).MemberList = _adminServices.SelectEmployeeRecord(null, null, searchTerm, division);
            ((dynamic)((ControllerBase)this).ViewBag).DivisonList = _adminServices.ListDivison();
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

        public ActionResult Generate_Notice()
        {
            ((dynamic)((ControllerBase)this).ViewBag).ProjectList = _adminServices.Project_List();
            return View();
        }

        [HttpGet]
        public ActionResult GetProjectManagerByProjectId(int? id)
        {
            dynamic projectManager = null;
            if (id.HasValue)
            {
                projectManager = (from e in _adminServices.Project_List()
                                  where e.Id == id
                                  select e).FirstOrDefault();
            }
            return (ActionResult)Json(projectManager, (JsonRequestBehavior)0);
        }

        [HttpGet]
        public ActionResult GetProjectById(int? id)
        {
            dynamic project = null;
            if (id.HasValue)
            {
                project = _adminServices.GetProjectById(id.Value);
            }
            return (ActionResult)Json(project, (JsonRequestBehavior)0);
        }

        public ActionResult AddNotice(RemoteSensingProject.Models.Admin.main.Generate_Notice gn)
        {
            string filePage = ((Controller)this).Server.MapPath("~/ProjectContent/Admin/NoticeDocs/");
            if (!Directory.Exists(filePage))
            {
                Directory.CreateDirectory(filePage);
            }
            string path = null;
            if (gn.Attachment != null)
            {
                string fileName = Guid.NewGuid().ToString() + DateTime.Now.ToString("ddMMyyyyhhmm") + gn.Attachment.FileName;
                path = (gn.Attachment_Url = Path.Combine("/ProjectContent/Admin/NoticeDocs", fileName));
            }
            bool res = _adminServices.InsertNotice(gn);
            if (res && gn.Attachment != null)
            {
                gn.Attachment.SaveAs(((Controller)this).Server.MapPath(path));
            }
            return Json((object)res);
        }

        public ActionResult Notice_List(int? projectManager, string searchTerm = null)
        {
            object noticeList = null;
            AdminServices adminServices = _adminServices;
            int? id = projectManager;
            noticeList = adminServices.getNoticeList(null, null, id, null, searchTerm);
            ((dynamic)((ControllerBase)this).ViewBag).ProjectList = _adminServices.Project_List();
            ((dynamic)((ControllerBase)this).ViewBag).EmployeeList = _adminServices.SelectEmployeeRecord();
            ((ControllerBase)this).ViewData["NoticeList"] = noticeList;
            return View();
        }

        public ActionResult View_Notice()
        {
            return View();
        }

        public ActionResult Expense_Report()
        {
            ((dynamic)((ControllerBase)this).ViewBag).ProjectList = _adminServices.Project_List();
            return View();
        }

        public ActionResult getHeadsByProject(int id)
        {
            List<RemoteSensingProject.Models.Admin.main.Project_model> _headList = new List<RemoteSensingProject.Models.Admin.main.Project_model>();
            _headList = _adminServices.getHeadByProject(id);
            return Json((object)new { _headList }, (JsonRequestBehavior)0);
        }

        public ActionResult GetExpenses(int hId, int pId)
        {
            List<ProjectExpenses> list = new List<ProjectExpenses>();
            list = _managerServices.ExpencesList(hId, pId);
            return Json((object)new { list }, (JsonRequestBehavior)0);
        }

        public ActionResult SubOrdinateProblemList(string searchTerm = null)
        {
            ((dynamic)((ControllerBase)this).ViewBag).ProjectProblemList = _managerServices.getSubOrdinateProblemforAdmin(null, null, searchTerm);
            return View();
        }

        [HttpGet]
        public ActionResult getsubproblembyid(int id)
        {
            ManagerService managerServices = _managerServices;
            int? id2 = id;
            List<RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem> data = managerServices.getSubOrdinateProblemforAdmin(null, null, null, id2);
            return Json((object)data, (JsonRequestBehavior)0);
        }

        public ActionResult ReimbursementRequest(int? projectManagerFilter = null, string typeFilter = null)
        {
            ViewDataDictionary viewData = ((ControllerBase)this).ViewData;
            ManagerService managerServices = _managerServices;
            int? managerId = projectManagerFilter;
            viewData["ReimBurseData"] = managerServices.GetReimbursements(null, null, null, managerId, "selectAll", typeFilter);
            ((ControllerBase)this).ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
            return View();
        }

        [HttpPost]
        public ActionResult ReimbursementReque(bool status, int id, string type, string remark)
        {
            bool res = _adminServices.ReimbursementApproval(status, id, type, remark);
            if (res)
            {
                return Json((object)new
                {
                    status = res,
                    message = ((res && status) ? "Approved Successfully" : "Rejected Successfully")
                });
            }
            return Json((object)new
            {
                status = res,
                message = "Some error Occured"
            });
        }

        public ActionResult TravelRequest(int? projectFilter = null)
        {
            AdminServices adminServices = _adminServices;
            ((ControllerBase)this).ViewData["allTourList"] = _managerServices.GetTourList(type: "ALLDATA", projectFilter: projectFilter);
            ((ControllerBase)this).ViewData["projects"] = _adminServices.Project_List();
            return View();
        }

        public ActionResult HiringRequest(int? projectFilter = null)
        {
            ((ControllerBase)this).ViewData["hiringList"] = _managerServices.GetHiringVehicles(type: "ALLDATA", projectFilter: projectFilter);
            ((ControllerBase)this).ViewData["projects"] = _adminServices.Project_List();
            return View();
        }

        public ActionResult Reimbursement_Report(int? projectManagerFilter = null, string typeFilter = null, string statusFilter = null)
        {
            ((ControllerBase)this).ViewData["totalProjectManager"] = (from d in _adminServices.SelectEmployeeRecord()
                                                                      where d.EmployeeRole.Equals("projectManager")
                                                                      select d).ToList();
            ManagerService managerServices = _managerServices;
            int? managerId = projectManagerFilter;
            List<Reimbursement> data = managerServices.GetReimbursements(null, null, null, managerId, "selectReinbursementReport", typeFilter, statusFilter);
            ((ControllerBase)this).ViewData["totalReinursementReport"] = data;
            return View();
        }

        public ActionResult TourProposal_Report(int? projectFilter = null)
        {
            ViewDataDictionary viewData = ((ControllerBase)this).ViewData;
            AccountService accountService = _accountService;
            viewData["allTourList"] = _managerServices.GetTourList(type: "ALLDATA", projectFilter: projectFilter);
            ((ControllerBase)this).ViewData["projects"] = _adminServices.Project_List();
            return View();
        }

        public ActionResult Hiring_Report(int? projectFilter = null)
        {
            ViewDataDictionary viewData = ((ControllerBase)this).ViewData;
            AdminServices adminServices = _adminServices;
            viewData["hiringList"] = _managerServices.GetHiringVehicles(type: "ALLDATA", projectFilter: projectFilter);
            ((ControllerBase)this).ViewData["projects"] = _adminServices.Project_List();
            return View();
        }

        public ActionResult RaisedProblem()
        {
            ((ControllerBase)this).ViewData["problemList"] = _adminServices.getProblemList();
            return View();
        }

        public ActionResult Attendance()
        {
            ((ControllerBase)this).ViewData["EmployeeList"] = (from d in _adminServices.SelectEmployeeRecord()
                                                               where d.EmployeeRole.Contains("projectManager")
                                                               select d).ToList();
            return View();
        }

        public ActionResult Attendance_Report()
        {
            ((ControllerBase)this).ViewData["EmployeeList"] = (from d in _adminServices.SelectEmployeeRecord()
                                                               where d.EmployeeRole.Contains("projectManager")
                                                               select d).ToList();
            return View();
        }

        public ActionResult ExportAttendanceToExcel(int month, int year, int EmpId, int projectManager)
        {
            byte[] data = _managerServices.ConvertExcelFile(month, year, projectManager, EmpId);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExcelReport.xlsx");
        }

        public ActionResult ShowAllAttendance(int year, int month, int projectManager)
        {
            List<AllAttendance> data = _managerServices.GetAllAttendanceForPmByMonth(year, month, projectManager);
            ((ControllerBase)this).ViewData["showAllAtt"] = data;
            ((ControllerBase)this).ViewData["AllAttendance"] = JsonConvert.SerializeObject((object)data);
            ((ControllerBase)this).ViewData["Year"] = year;
            ((ControllerBase)this).ViewData["Month"] = month;
            return View();
        }

        public ActionResult ExportAllAtt(int year, int month, int projectManager)
        {
            byte[] data = _managerServices.ConvertExcelFileOfAll(month, year, projectManager);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExcelReportOfAll.xlsx");
        }

        public ActionResult EmployeeMonthlyReport()
        {
            ((dynamic)((ControllerBase)this).ViewBag).MemberList = _adminServices.SelectEmployeeRecord();
            return View();
        }

        [HttpGet]
        public ActionResult EmployeeReportMon(int id, int? month = null, int? year = null)
        {
            try
            {
                List<EmpReportModel> data = _managerServices.GetEmpReport(id, null, null, null, month, year);
                return Json((object)new
                {
                    status = (data != null),
                    data = data
                }, (JsonRequestBehavior)0);
            }
            catch (Exception ex)
            {
                return Json((object)new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }

        public ActionResult CMDashboardData()
        {
            ((ControllerBase)this).ViewData["ProjectList"] = _adminServices.GetCmDashboardList(null, true);
            return View();
        }

        [HttpPost]
        public ActionResult SubmitDashboardData(RemoteSensingProject.Models.Admin.main.CMDashboardData cm)
        {
            if (!((Controller)this).ModelState.IsValid)
            {
                return Json((object)new
                {
                    status = false,
                    message = "Invalid data submitted"
                });
            }
            cm.db_action = ((cm.Id > 0) ? "UPDATE" : "INSERT");
            bool result = _adminServices.InsertCmDashboardProject(cm);
            return Json((object)new
            {
                status = result,
                message = ((!result) ? "Some issue found while submitting." : ((cm.Id > 0) ? "Project updated successfuflly !" : "Project inserted successfully !"))
            }, (JsonRequestBehavior)0);
        }

        public ActionResult DeleteDashboardData(int projectId)
        {
            RemoteSensingProject.Models.Admin.main.CMDashboardData cm = new RemoteSensingProject.Models.Admin.main.CMDashboardData();
            cm.Id = projectId;
            cm.db_action = "DELETE";
            bool result = _adminServices.InsertCmDashboardProject(cm);
            return Json((object)new
            {
                status = result,
                message = (result ? "Project deleted successfully!" : "Failed to delete project.")
            }, (JsonRequestBehavior)0);
        }

        public ActionResult GetCmPRojectById(int projectId)
        {
            RemoteSensingProject.Models.Admin.main.CMDashboardData data = _adminServices.GetCmDashboardList(projectId, true).FirstOrDefault();
            return Json((object)new { data }, (JsonRequestBehavior)0);
        }

        public ActionResult ProjectReport(string type, string searchTerm = null, string statusFilter = null, int? projectManagerFilter = null)
        {
            //IL_0271: Unknown result type (might be due to invalid IL or missing references)
            //IL_0277: Expected O, but got Unknown
            //IL_0017: Unknown result type (might be due to invalid IL or missing references)
            //IL_001d: Expected O, but got Unknown
            //IL_025c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0262: Expected O, but got Unknown
            try
            {
                if (string.IsNullOrWhiteSpace(type))
                {
                    return (ActionResult)new HttpStatusCodeResult(400, "Report type is required.");
                }
                List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
            {
                new ApiCommon.ColumnMapping
                {
                    Header = "Project Code",
                    PropertyName = "projectCode"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Project Name",
                    PropertyName = "ProjectTitle"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Project Manager",
                    PropertyName = "ProjectManager"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Division",
                    PropertyName = "devisionName"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Assign Date",
                    PropertyName = "AssignDateString"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Start Date",
                    PropertyName = "StartDateString"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Completion Date",
                    PropertyName = "CompletionDatestring"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Physical in %",
                    PropertyName = "physicalcomplete"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Financial in %",
                    PropertyName = "Percentage"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Overall in %",
                    PropertyName = "overallPercentage"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Project Status",
                    PropertyName = "projectStatusLabel"
                }
            };
                IEnumerable<RemoteSensingProject.Models.Admin.main.Project_model> data = _adminServices.Project_List(null, null, null, searchTerm, statusFilter, projectManagerFilter);
                if (type.Equals("Excel", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Project Report");
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Project_Report.xlsx");
                }
                if (type.Equals("pdf", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Project Report");
                    return File(pdfBytes, "application/pdf", "Project_Report.pdf");
                }
                return (ActionResult)new HttpStatusCodeResult(400, "Invalid report type. Use Excel or Pdf.");
            }
            catch (Exception)
            {
                return (ActionResult)new HttpStatusCodeResult(500, "Error while generating project report.");
            }
        }

        public ActionResult DownloadEmployeeReport(string type, string searchTerm = null, int? devision = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(type))
                {
                    return (ActionResult)new HttpStatusCodeResult(400, "Report type is required.");
                }
                List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
            {
                new ApiCommon.ColumnMapping
                {
                    Header = "Employee Code",
                    PropertyName = "EmployeeCode"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Employee Name",
                    PropertyName = "EmployeeName"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Division Name",
                    PropertyName = "DevisionName"
                }
            };
                List<RemoteSensingProject.Models.Admin.main.Employee_model> data = _adminServices.SelectEmployeeRecord(null, null, searchTerm, devision);
                if (type.Equals("pdf", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Employee Report");
                    return File(pdfBytes, "application/pdf", "Employee_Report.pdf");
                }
                if (type.Equals("excel", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Employee Report");
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee_Report.xlsx");
                }
                return new HttpStatusCodeResult(400, "Invalid report type. Use Excel or Pdf.");
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500, "Error while generating employee report.");
            }
        }

        public ActionResult DownloadMeetingReport(string type, string searchTerm = null, string statusFilter = null, string meetingMode = null)
        {
            //IL_0173: Unknown result type (might be due to invalid IL or missing references)
            //IL_0179: Expected O, but got Unknown
            //IL_0017: Unknown result type (might be due to invalid IL or missing references)
            //IL_001d: Expected O, but got Unknown
            //IL_015e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0164: Expected O, but got Unknown
            try
            {
                if (string.IsNullOrWhiteSpace(type))
                {
                    return (ActionResult)new HttpStatusCodeResult(500, "Report type is required");
                }
                List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
            {
                new ApiCommon.ColumnMapping
                {
                    Header = "Meeting Name",
                    PropertyName = "MeetingTitle"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Meeting Mode",
                    PropertyName = "MeetingType"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Meeting Date",
                    PropertyName = "MeetingDate"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Status",
                    PropertyName = "statusLabel"
                }
            };
                List<RemoteSensingProject.Models.Admin.main.Meeting_Model> data = _adminServices.getAllmeeting(null, null, searchTerm, statusFilter, meetingMode);
                if (type.Equals("pdf", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Meeting Report");
                    return File(pdfBytes, "application/pdf", "Meeting_Report.pdf");
                }
                if (type.Equals("excel", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Meeting Report");
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Metting_Report.xlsx");
                }
                return (ActionResult)new HttpStatusCodeResult(400, "Invalid report type. Use Excel or Pdf.");
            }
            catch (Exception)
            {
                return (ActionResult)new HttpStatusCodeResult(500, "Error while generating metting report.");
            }
        }

        public ActionResult DownloadExpencesReport(string type, int projectId, int headId)
        {
            //IL_0165: Unknown result type (might be due to invalid IL or missing references)
            //IL_016b: Expected O, but got Unknown
            //IL_0017: Unknown result type (might be due to invalid IL or missing references)
            //IL_001d: Expected O, but got Unknown
            //IL_0150: Unknown result type (might be due to invalid IL or missing references)
            //IL_0156: Expected O, but got Unknown
            try
            {
                if (string.IsNullOrWhiteSpace(type))
                {
                    return (ActionResult)new HttpStatusCodeResult(500, "Report type is required");
                }
                List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
            {
                new ApiCommon.ColumnMapping
                {
                    Header = "Title",
                    PropertyName = "title"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Date",
                    PropertyName = "DateString"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Description",
                    PropertyName = "description"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Amount",
                    PropertyName = "amount"
                }
            };
                List<ProjectExpenses> data = _managerServices.ExpencesList(headId, projectId);
                if (type.Equals("pdf", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Expense Report");
                    return File(pdfBytes, "application/pdf", "Project_Expense_report.pdf");
                }
                if (type.Equals("excel", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Expense Report");
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Project_Expense_Report.xlsx");
                }
                return (ActionResult)new HttpStatusCodeResult(400, "Invalid report type. Use Excel or Pdf.");
            }
            catch (Exception)
            {
                return (ActionResult)new HttpStatusCodeResult(500, "Error while generating project expenses report.");
            }
        }

        public ActionResult DownloadReimbursementReport(string type, int? projectManagerFilter = null, string typeFilter = null, string statusFilter = null)
        {
            //IL_01ca: Unknown result type (might be due to invalid IL or missing references)
            //IL_01d0: Expected O, but got Unknown
            //IL_0017: Unknown result type (might be due to invalid IL or missing references)
            //IL_001d: Expected O, but got Unknown
            //IL_01b5: Unknown result type (might be due to invalid IL or missing references)
            //IL_01bb: Expected O, but got Unknown
            try
            {
                if (string.IsNullOrWhiteSpace(type))
                {
                    return (ActionResult)new HttpStatusCodeResult(500, "Report type is required");
                }
                List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
            {
                new ApiCommon.ColumnMapping
                {
                    Header = "Project Manager",
                    PropertyName = "EmpName"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Type",
                    PropertyName = "type"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Amount",
                    PropertyName = "amount"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Approval Amt",
                    PropertyName = "approveAmount"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Remark",
                    PropertyName = "remark"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Status",
                    PropertyName = "statusLabel"
                }
            };
                List<Reimbursement> data = _managerServices.GetReimbursements(null, null, null, projectManagerFilter, "selectReinbursementReport", typeFilter, statusFilter);
                if (type.Equals("pdf", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Reimbursement Report");
                    return File(pdfBytes, "application/pdf", "Reimbursement_report.pdf");
                }
                if (type.Equals("excel", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Reimbursement Report");
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reimbursement_report.xlsx");
                }
                return (ActionResult)new HttpStatusCodeResult(400, "Invalid report type. Use Excel or Pdf.");
            }
            catch (Exception)
            {
                return (ActionResult)new HttpStatusCodeResult(500, "Error while generating project expenses report.");
            }
        }

        public ActionResult DownloadTourProposalReport(string type, int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
        {
            //IL_026f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0275: Expected O, but got Unknown
            //IL_0017: Unknown result type (might be due to invalid IL or missing references)
            //IL_001d: Expected O, but got Unknown
            //IL_025a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0260: Expected O, but got Unknown
            try
            {
                if (string.IsNullOrWhiteSpace(type))
                {
                    return (ActionResult)new HttpStatusCodeResult(500, "Report type is required");
                }
                List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
            {
                new ApiCommon.ColumnMapping
                {
                    Header = "Project Manager",
                    PropertyName = "projectManager"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Project Code",
                    PropertyName = "projectCode"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Project",
                    PropertyName = "projectName"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Purpose",
                    PropertyName = "purpose"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Date of Depart",
                    PropertyName = "dateOfDept"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Date of Return",
                    PropertyName = "returnDate"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Place Visit",
                    PropertyName = "place"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Period From",
                    PropertyName = "periodFrom"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Period To",
                    PropertyName = "periodTo"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Remark",
                    PropertyName = "remark"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Status",
                    PropertyName = "statusLabel"
                }
            };
                List<RemoteSensingProject.Models.Accounts.main.tourProposal> data = _accountService.getTourList(null, null, managerFilter, projectFilter, statusFilter);
                if (type.Equals("pdf", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Tourproposal Report");
                    return File(pdfBytes, "application/pdf", "Tour_Proposal_Report.pdf");
                }
                if (type.Equals("excel", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Tourproposal Report");
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Tour_Proposal_Report.xlsx");
                }
                return (ActionResult)new HttpStatusCodeResult(400, "Invalid report type. Use Excel or Pdf.");
            }
            catch (Exception)
            {
                return (ActionResult)new HttpStatusCodeResult(500, "Error while generating project expenses report.");
            }
        }

        public ActionResult DownloadHiringVehicleReport(string type, int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
        {
            //IL_0293: Unknown result type (might be due to invalid IL or missing references)
            //IL_0299: Expected O, but got Unknown
            //IL_0017: Unknown result type (might be due to invalid IL or missing references)
            //IL_001d: Expected O, but got Unknown
            //IL_027e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0284: Expected O, but got Unknown
            try
            {
                if (string.IsNullOrWhiteSpace(type))
                {
                    return (ActionResult)new HttpStatusCodeResult(500, "Report type is required");
                }
                List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
            {
                new ApiCommon.ColumnMapping
                {
                    Header = "Project Manager",
                    PropertyName = "projectManager"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Project Code",
                    PropertyName = "projectCode"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Project",
                    PropertyName = "projectName"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Head Name",
                    PropertyName = "headName"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Date From",
                    PropertyName = "dateFrom"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Date To",
                    PropertyName = "dateTo"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Proposed Place",
                    PropertyName = "proposedPlace"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Purpose of Visit",
                    PropertyName = "purposeOfVisit"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Total Day & Night",
                    PropertyName = "totalDaysNight"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Availability of Fund",
                    PropertyName = "availbilityOfFund"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Remark",
                    PropertyName = "remark"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Status",
                    PropertyName = "statusLabel"
                }
            };
                List<RemoteSensingProject.Models.Admin.main.HiringVehicle1> data = _adminServices.HiringReort(null, null, managerFilter, projectFilter, statusFilter);
                if (type.Equals("pdf", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Hiring Report");
                    return File(pdfBytes, "application/pdf", "Hiring_Report.pdf");
                }
                if (type.Equals("excel", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Hiring Report");
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Hiring_Report.xlsx");
                }
                return (ActionResult)new HttpStatusCodeResult(400, "Invalid report type. Use Excel or Pdf.");
            }
            catch (Exception)
            {
                return (ActionResult)new HttpStatusCodeResult(500, "Error while generating project expenses report.");
            }
        }
    }
}