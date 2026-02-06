// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Controllers.EmployeeController
using Newtonsoft.Json;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace RemoteSensingProject.Controllers
{
    [Authorize(Roles = "projectManager")]
    public class EmployeeController : Controller
    {
        private readonly ManagerService _managerServices;

        private readonly AdminServices _adminServices;

        private readonly AccountService _accountService;

        public EmployeeController()
        {
            _managerServices = new ManagerService();
            _adminServices = new AdminServices();
            _accountService = new AccountService();
        }

        public ActionResult Dashboard()
        {
            string managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);
            DashboardCount TotalCount = _managerServices.DashboardCount(Convert.ToInt32(userObj.userId));
            DateTime twoYearsAgo = DateTime.Now.AddYears(-2);
            ((ControllerBase)this).ViewData["emplist"] = (from d in _managerServices.All_Project_List(Convert.ToInt32(userObj.userId))
                                                          where d.AssignDate >= twoYearsAgo
                                                          select d).ToList();
            return View((object)TotalCount);
        }

        public ActionResult BindOverallCompletionPercentage()
        {
            string managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);
            List<RemoteSensingProject.Models.Admin.main.DashboardCount> list = (from e in _adminServices.getAllProjectCompletion()
                                                                                where e.ProjectManager == userObj.userId
                                                                                select e).ToList();
            return Json((object)new { list }, (JsonRequestBehavior)0);
        }

        public ActionResult OutSource(string searchTerm = null)
        {
            int userObj = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            ((ControllerBase)this).ViewData["UserList"] = _managerServices.GetAllocatedOutSOurceList(userObj, null, null, searchTerm);
            return View();
        }


        public ActionResult CreateTask(string req)
        {
            int userObj = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            ViewData["projectlist"] = _managerServices.All_Project_List(Convert.ToInt32(userObj), null, null, "AssignedProject");
            ((ControllerBase)this).ViewData["OutSourceList"] = _managerServices.GetAllocatedOutSOurceList(userObj);
            ((ControllerBase)this).ViewData["TaskList"] = ((req == "completed") ? (from d in _managerServices.taskList(userObj)
                                                                                   where d.completeStatus
                                                                                   select d).ToList() : ((req == "pending") ? (from d in _managerServices.taskList(userObj)
                                                                                                                               where !d.completeStatus
                                                                                                                               select d).ToList() : _managerServices.taskList(userObj)));
            return View();
        }

        [HttpPost]
        public ActionResult InsertOutSourceTour(OutsourceAtTour at)
        {
            try
            {
                bool res = _managerServices.SendOutSourceAtTour(at);
                return Json(new
                {
                    status = res,
                    message = res ? "Submitted successfully" : "Something went wrong"
                });
            }
            catch(Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult CreateTaskJson(OutSourceTask ost)
        {
            ost.empId = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            bool res = _managerServices.createTask(ost);
            return Json((object)new
            {
                status = res,
                message = (res ? "Task created successfully !" : "Some issue occured !")
            });
        }

        public ActionResult Add_Project()
        {
            List<RemoteSensingProject.Models.Admin.main.Employee_model> data = _adminServices.SelectEmployeeRecord();
            ((dynamic)((ControllerBase)this).ViewBag).subOrdinateList = data.Where((RemoteSensingProject.Models.Admin.main.Employee_model d) => d.EmployeeRole.Equals("subOrdinate")).ToList();
            ((ControllerBase)this).ViewData["BudgetHeads"] = _adminServices.GetBudgetHeads();
            ((ControllerBase)this).ViewData["Designations"] = _adminServices.ListDesgination();
            return View();
        }

        [HttpPost]
        public ActionResult UpdateTaskStatus(int taskId)
        {
            bool res = false;
            string message = "Some issue occured !";
            try
            {
                res = _managerServices.updateTaskStatus(taskId);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json((object)new
            {
                status = res,
                message = (res ? "Task updated successfully !" : message)
            });
        }

        public ActionResult InsertProject(RemoteSensingProject.Models.Admin.main.createProjectModel pm)
        {
            string filePage = Server.MapPath("~/ProjectContent/ProjectManager/ProjectDocs/");
            if (!Directory.Exists(filePage))
            {
                Directory.CreateDirectory(filePage);
            }
            string managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);
            if (userObj != null)
            {
                pm.pm.ProjectManager = userObj.userId;
                pm.pm.createdBy = "projectManager";
                if (pm.pm.projectDocument != null && pm.pm.projectDocument.FileName != "")
                {
                    pm.pm.projectDocumentUrl = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(pm.pm.projectDocument.FileName);
                    pm.pm.projectDocumentUrl = "/ProjectContent/ProjectManager/ProjectDocs/" + pm.pm.projectDocumentUrl;
                }
                if (pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage)
                {
                    foreach (RemoteSensingProject.Models.Admin.main.Project_Statge item in pm.stages)
                    {
                        if (item.Stage_Document != null && item.Stage_Document.FileName != "")
                        {
                            item.Document_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(item.Stage_Document.FileName);
                            item.Document_Url = "/ProjectContent/ProjectManager/ProjectDocs/" + item.Document_Url;
                        }
                    }
                }
                bool res = _adminServices.addProject(pm);
                if (res)
                {
                    if (pm.pm.projectDocument != null && pm.pm.projectDocument.FileName != "")
                    {
                        pm.pm.projectDocument.SaveAs(Server.MapPath(pm.pm.projectDocumentUrl));
                    }
                    if (pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage)
                    {
                        foreach (RemoteSensingProject.Models.Admin.main.Project_Statge item2 in pm.stages)
                        {
                            if (item2.Stage_Document != null && item2.Stage_Document.FileName != "")
                            {
                                item2.Stage_Document.SaveAs(Server.MapPath(item2.Document_Url));
                            }
                        }
                    }
                }
                return Json((object)new
                {
                    status = res,
                    message = ((pm.pm.Id > 0) ? "Project Updated Successfully" : "Project Created Successfully")
                }, (JsonRequestBehavior)0);
            }
            return RedirectToAction("login", "login");
        }

        public ActionResult Project_List(string searchTerm = null, string statusFilter = null)
        {
            string managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);
            ((dynamic)((ControllerBase)this).ViewBag).ProjectList = _managerServices.All_Project_List(Convert.ToInt32(userObj.userId), null, null, "ManagerProject", null, searchTerm, statusFilter);
            return View();
        }

        public ActionResult All_Project_List(string searchTerm = null, string statusFilter = null)
        {
            UserCredential userObj = _managerServices.getManagerDetails(User.Identity.Name);
            ((ControllerBase)this).ViewData["ProjectList"] = _managerServices.All_Project_List(Convert.ToInt32(userObj.userId), null, null, null, null, searchTerm, statusFilter);
            return View();
        }

        public ActionResult Assigned_Project(string searchTerm = null, string statusFilter = null)
        {
            string managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);
            List<ProjectList> _list = new List<ProjectList>();
            ((ControllerBase)this).ViewData["AssignedProjectList"] = _managerServices.All_Project_List(Convert.ToInt32(userObj.userId), null, null, "AssignedProject", null, searchTerm, statusFilter);
            return View();
        }

        public ActionResult GetAllProjectByManager()
        {
            string managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);
            List<RemoteSensingProject.Models.Admin.main.Project_model> _list = new List<RemoteSensingProject.Models.Admin.main.Project_model>();
            _list = _managerServices.All_Project_List(Convert.ToInt32(userObj.userId), null, null, "AssignedProject");
            return Json((object)new { _list }, (JsonRequestBehavior)0);
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

        public ActionResult Approved_Project()
        {
            return View();
        }

        public ActionResult Weekly_Update_Project(int Id)
        {
            ((dynamic)((ControllerBase)this).ViewBag).WeeklyUpdateList = _managerServices.MonthlyProjectUpdate(Id);
            return View();
        }

        public ActionResult monthly_extr_ProjectStatue(int Id)
        {
            ((ControllerBase)this).ViewData["totalData"] = _managerServices.GetExtrnlFinancialReport(Id);
            return View();
        }

        public JsonResult insertMonthlyExtrPrj(FinancialMonthlyReport fr)
        {
            bool res = _managerServices.updateFinancialReportMonthly(fr);
            return Json((object)new
            {
                status = res,
                message = (res ? "Updated successfully !" : "Some issue occured while processing...")
            });
        }

        public JsonResult UpdateWeekly(RemoteSensingProject.Models.Admin.main.Project_MonthlyUpdate pwu)
        {
            bool res = _managerServices.UpdateMonthlyStatus(pwu);
            return Json((object)new
            {
                status = res,
                message = (res ? "Monthly updated successfully !" : "Some issue conflicted !")
            });
        }

        public ActionResult Update_Project_Stage(int Id)
        {
            ((dynamic)((ControllerBase)this).ViewBag).ProjectStages = _adminServices.ProjectStagesList(Id);
            return View();
        }

        [HttpPost]
        public ActionResult AddStageStatus(RemoteSensingProject.Models.Admin.main.Project_Statge formData)
        {
            if ((formData.StageDocument != null && formData.StageDocument.ContentLength > 0) || (formData.DelayedStageDocument != null && formData.DelayedStageDocument.ContentLength > 0))
            {
                string fileName = formData.StageDocument?.FileName ?? formData.DelayedStageDocument?.FileName;
                formData.StageDocument_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(fileName);
                formData.StageDocument_Url = Path.Combine("/ProjectContent/ProjectManager/ProjectDocs/", formData.StageDocument_Url);
            }
            string message = "";
            bool status = _managerServices.InsertStageStatus(formData);
            if (status && formData.StageDocument != null && formData.StageDocument.FileName != "")
            {
                formData.StageDocument.SaveAs(Server.MapPath(formData.StageDocument_Url));
            }
            message = ((!status) ? "Failed To Update Stages" : "Satges Updated Successfully !");
            return Json((object)new { message, status }, (JsonRequestBehavior)0);
        }

        public ActionResult viewStagesReason(string stageId)
        {
            List<RemoteSensingProject.Models.Admin.main.Project_Statge> stageList = new List<RemoteSensingProject.Models.Admin.main.Project_Statge>();
            stageList = _managerServices.ViewStagesComments(stageId);
            return Json((object)new { stageList }, (JsonRequestBehavior)0);
        }

        public ActionResult Add_Expenses(int Id)
        {
            ((ControllerBase)this).ViewData["ProjectStages"] = _adminServices.ProjectBudgetList(Id);
            return View();
        }

        public ActionResult Min_Of_Meeting()
        {
            int userid =Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            var empList = _managerServices.GetAllocatedOutSOurceList(userid);
            return View((object)empList);
        }

        public ActionResult GetConclusions(int id)
        {
            UserCredential userId = _managerServices.getManagerDetails(User.Identity.Name);
            List<GetConclusion> res = _managerServices.getConclusionForMeeting(id, int.Parse(userId.userId));
            return Json((object)res, (JsonRequestBehavior)0);
        }

        [HttpPost]
        public ActionResult AddMeeting(RemoteSensingProject.Models.Admin.main.AddMeeting_Model formData)
        {
            string filePage = Server.MapPath("~/ProjectContent/ProjectManager/Meeting_Attachment/");
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
                path = (formData.Attachment_Url = Path.Combine("/ProjectContent/ProjectManager/Meeting_Attachment", fileName));
            }
            UserCredential userId = _managerServices.getManagerDetails(User.Identity.Name);
            formData.CreaterId = int.Parse(userId.userId);
            bool status = _adminServices.insertMeeting(formData);
            if (status && formData.Attachment != null)
            {
                formData.Attachment.SaveAs(Server.MapPath(path));
            }
            return Json((object)new
            {
                success = status
            }, (JsonRequestBehavior)0);
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
                path = (formData.Attachment_Url = Path.Combine("/ProjectContent/ProjectManager/Meeting_Attachment", fileName));
            }
            bool status = _adminServices.UpdateMeeting(formData);
            if (status && formData.Attachment != null)
            {
                formData.Attachment.SaveAs(Server.MapPath(path));
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
        public ActionResult GetAllMeeting()
        {
            string id = _managerServices.getManagerDetails(User.Identity.Name).userId;
            List<RemoteSensingProject.Models.Admin.main.Meeting_Model> empList = new List<RemoteSensingProject.Models.Admin.main.Meeting_Model>();
            empList = (from e in _adminServices.getAllmeeting()
                       where e.CreaterId.ToString() == id
                       select e).ToList();
            return Json((object)new { empList }, (JsonRequestBehavior)0);
        }

        public ActionResult Meeting_Conclusion(int meeting)
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

        public ActionResult Meetings(string searchTerm = null, string statusFilter = null)
        {
            UserCredential userId = _managerServices.getManagerDetails(User.Identity.Name);
            List<RemoteSensingProject.Models.Admin.main.Meeting_Model> res = _managerServices.getAllmeeting(int.Parse(userId.userId), null, null, searchTerm, statusFilter);
            return View((object)res);
        }

        public ActionResult GetMemberResponse(getMemberResponse mr)
        {
            UserCredential userId = _managerServices.getManagerDetails(User.Identity.Name);
            mr.MemberId = int.Parse(userId.userId);
            bool res = _managerServices.GetResponseFromMember(mr);
            return Json((object)res);
        }

        [HttpGet]
        public ActionResult GetMemberJoiningStatusByMeeting(int meetingId)
        {
            List<RemoteSensingProject.Models.Admin.main.Employee_model> res = _managerServices.getMemberJoiningStatus(meetingId);
            return Json((object)res, (JsonRequestBehavior)0);
        }

        public ActionResult MyProfile()
        {
            return View();
        }

        public ActionResult Notice(int? projectId, string searchTerm = null)
        {
            object noticeList = null;
            string managerName = User.Identity.Name;
            UserCredential userObj = _managerServices.getManagerDetails(managerName);
            noticeList = _adminServices.getNoticeList(null, null, projectId, Convert.ToInt32(userObj.userId), searchTerm);
            ((dynamic)((ControllerBase)this).ViewBag).ProjectList = _adminServices.Project_List();
            ((ControllerBase)this).ViewData["NoticeList"] = noticeList;
            return View();
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

        public ActionResult SubOrdinateProblemList(string searchTerm = null)
        {
            string managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);
            ((dynamic)((ControllerBase)this).ViewBag).ProjectProblemList = _managerServices.getAllSubOrdinateProblem(userObj.userId, null, null, searchTerm);
            return View();
        }

        [HttpGet]
        public ActionResult SubordinateProblemListById(int id)
        {
            string managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);
            List<RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem> data = _managerServices.getAllSubOrdinateProblemById(userObj.userId, id);
            return Json((object)data, (JsonRequestBehavior)0);
        }

        [HttpPost]
        public ActionResult updateProjectProblemStatus(int problemId)
        {
            bool res = _managerServices.CompleteSelectedProblem(problemId);
            return Json((object)new
            {
                status = res,
                message = (res ? "Problem solved successfully !" : "Some issue occured.  Try after sometime.")
            });
        }

        public ActionResult All_Project_Report(string searchTerm = null, string statusFilter = null)
        {
            UserCredential userObj = _managerServices.getManagerDetails(User.Identity.Name);
            ((ControllerBase)this).ViewData["ProjectList"] = _managerServices.All_Project_List(Convert.ToInt32(userObj.userId), null, null, null, null, searchTerm, statusFilter);
            return View();
        }

        public ActionResult Pending_Project_Report()
        {
            string userObj = _managerServices.getManagerDetails(User.Identity.Name).userId;
            ((ControllerBase)this).ViewData["NotStartedProject"] = _managerServices.All_Project_List(Convert.ToInt32(userObj), null, null, "Upcoming");
            return View();
        }

        public ActionResult Complete_Project_Report()
        {
            string userObj = _managerServices.getManagerDetails(User.Identity.Name).userId;
            ((ControllerBase)this).ViewData["CompleteProjectList"] = _managerServices.All_Project_List(Convert.ToInt32(userObj), null, null, "Complete");
            return View();
        }

        public ActionResult Expense_Report()
        {
            UserCredential userObj = _managerServices.getManagerDetails(User.Identity.Name);
            ((dynamic)((ControllerBase)this).ViewBag).ProjectList = _managerServices.All_Project_List(Convert.ToInt32(userObj.userId));
            return View();
        }

        public ActionResult Reimbursement_Form(string typeFilter = null)
        {
            int id = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            List<Reimbursement> res = _managerServices.GetReimbursements(null, null, null, id, "getSpecificUserData", typeFilter);
            ((ControllerBase)this).ViewData["reimlist"] = res;
            return View();
        }

        [HttpPost]
        public ActionResult Reimbursement_Form(List<Reimbursement> data)
        {
            try
            {
                int i = 0;
                if (data.Any())
                {
                    foreach (Reimbursement item in data)
                    {
                        item.userId = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
                        if (_managerServices.insertReimbursement(item))
                        {
                            i++;
                        }
                    }
                }
                return Json((object)new
                {
                    status = (i == data.Count),
                    message = ((i == data.Count) ? "Added Successfully" : "Some issue occured while processing some data .")
                });
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

        public ActionResult ViewReinbursementListByType(string type, int id)
        {
            int userId = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            List<Reimbursement> data = _managerServices.GetReimbursements(null, null, id, userId, type);
            return Json((object)new
            {
                status = data.Any(),
                data = data
            }, (JsonRequestBehavior)0);
        }

        public ActionResult SubmitReinbursementFormType(string type, int Id)
        {
            int userId = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            bool res = _managerServices.submitReinbursementForm(type, userId, Id);
            return Json((object)new
            {
                status = res,
                message = (res ? "Submitted successfully !" : "Some issue occured !")
            }, (JsonRequestBehavior)0);
        }

        public ActionResult Hiring_Vehicle(int? projectFilter = null)
        {
            int userid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            List<RemoteSensingProject.Models.Admin.main.Project_model> res = _managerServices.All_Project_List(userid);
            ((ControllerBase)this).ViewData["projectlist"] = res;
            ManagerService managerServices = _managerServices;
            List<HiringVehicle> res2 = managerServices.GetHiringVehicles(type: "GETBYMANAGER", id: userid, projectFilter: projectFilter);
            ((ControllerBase)this).ViewData["hiringList"] = res2;
            return View();
        }

        public ActionResult Tour_Proposal(int? projectFilter = null)
        {
            int userid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            ManagerService managerServices = _managerServices;
            List<tourProposal> res = managerServices.GetTourList(type: "GETBYMANAGER", id: userid, projectFilter: projectFilter);
            List<RemoteSensingProject.Models.Admin.main.Project_model> res2 = _managerServices.All_Project_List(userid);
            ((ControllerBase)this).ViewData["projectList"] = res2;
            ((ControllerBase)this).ViewData["tourList"] = res;
            return View();
        }

        public ActionResult Reimbursement_Report(string typeFilter = null, string statusFilter = null)
        {
            int userId = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            ViewDataDictionary viewData = ((ControllerBase)this).ViewData;
            ManagerService managerServices = _managerServices;
            int? managerId = userId;
            viewData["totalReinursementReport"] = managerServices.GetReimbursements(null, null, null, managerId, "selectReinbursementforUserReport", typeFilter, statusFilter);
            return View();
        }

        //public ActionResult TourProposal_Report(int? projectFilter = null, string statusFilter = null)
        //{
        //	int userid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
        //	ViewDataDictionary viewData = ((ControllerBase)this).ViewData;
        //	ManagerService managerServices = _managerServices;
        //	int? userId = userid;
        //	int? projectFilter2 = projectFilter;
        //	viewData["tourList"] = managerServices.getTourList(userId, null, "specificUser", null, null, projectFilter2, statusFilter);
        //	((ControllerBase)this).ViewData["projects"] = _adminServices.Project_List();
        //	((ControllerBase)this).ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
        //	return View();
        //}

        //public ActionResult Hiring_Report(int? projectFilter = null, string statusFilter = null)
        //{
        //	int userid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
        //	List<RemoteSensingProject.Models.Admin.main.Project_model> res = _managerServices.All_Project_List(userid);
        //	((ControllerBase)this).ViewData["projectlist"] = res;
        //	ManagerService managerServices = _managerServices;
        //	int? userId = userid;
        //	int? projectFilter2 = projectFilter;
        //	List<HiringVehicle> res2 = managerServices.GetHiringVehicles(userId, null, "projectManager", null, null, projectFilter2, statusFilter);
        //	((ControllerBase)this).ViewData["hiringList"] = res2;
        //	((ControllerBase)this).ViewData["projects"] = _adminServices.Project_List();
        //	return View();
        //}

        public ActionResult RaiseProblem()
        {
            int userid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            ((ControllerBase)this).ViewData["projectList"] = _managerServices.All_Project_List(userid);
            ((ControllerBase)this).ViewData["ProblemList"] = _managerServices.getProblems(userid);
            return View();
        }

        [HttpPost]
        public ActionResult RaiseProblem(RaiseProblem dt)
        {
            dt.id = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            if (dt.document.ContentLength > 0)
            {
                dt.documentname = $"/ProjectContent/ProjectManager/raisedproblem/{Guid.NewGuid()}{dt.document.FileName}";
            }
            bool res = _managerServices.insertRaisedProblem(dt);
            if (res)
            {
                if (dt.document != null && dt.document.ContentLength > 0)
                {
                    string fullPath = Server.MapPath(dt.documentname);
                    string dir = Path.GetDirectoryName(fullPath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    dt.document.SaveAs(fullPath);
                }
                return Json((object)new
                {
                    status = res,
                    message = "Problem raised successfully !"
                });
            }
            return Json((object)new
            {
                status = res,
                message = "Some issue occured !"
            });
        }

        [HttpGet]
        public ActionResult GetRaiseProblemById(int id)
        {
            int managerId = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            AdminServices adminServices = _adminServices;
            int? managerId2 = managerId;
            int? id2 = id;
            List<RemoteSensingProject.Models.Admin.main.RaisedProblem> data = adminServices.getProblemList(null, null, id2, managerId2);
            return Json((object)new
            {
                status = true,
                data = data
            }, (JsonRequestBehavior)0);
        }

        [HttpGet]
        public ActionResult DeleteRaiseProblem(int id)
        {
            int userId = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            bool res = _managerServices.deleteRaisedProblem(id, userId);
            if (res)
            {
                return Json((object)new
                {
                    status = res,
                    message = "Deleted Successfully"
                }, (JsonRequestBehavior)0);
            }
            return Json((object)new
            {
                status = res,
                message = "Something wend wrong."
            }, (JsonRequestBehavior)0);
        }

        public ActionResult ManageAttendance()
        {
            int userObj = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            ((ControllerBase)this).ViewData["UserList"] = _managerServices.selectAllOutSOurceList(userObj);
            ((ControllerBase)this).ViewData["attendanceCount"] = _managerServices.getAttendanceCount(userObj);
            return View();
        }

        [HttpGet]
        public JsonResult GetAttendanceListByEmpId(int id)
        {
            int userObj = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            List<AttendanceManage> data = _managerServices.GetAllAttendanceForProjectManager(userObj, id);
            return Json((object)data, (JsonRequestBehavior)0);
        }

        [HttpPost]
        public JsonResult AddAttendance(AttendanceManage model)
        {
            int userObj = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            model.projectManager = Convert.ToInt32(userObj);
            (bool, List<string>, string) result = _managerServices.InsertAttendance(model);
            if (result.Item1)
            {
                if (result.Item2.Count > 0)
                {
                    return Json((object)new
                    {
                        success = true,
                        message = "Attendance submitted. Some dates were skipped as they already exist.",
                        skipped = result.Item2
                    });
                }
                return Json((object)new
                {
                    success = true,
                    message = "Attendance submitted successfully."
                });
            }
            return Json((object)new
            {
                success = false,
                message = "Error occurred: " + result.Item3
            });
        }

        public ActionResult AttendanceRequest()
        {
            int userObj = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            ((ControllerBase)this).ViewData["newattendancelist"] = _managerServices.GetAllAttendanceForProjectManager(userObj);
            return View();
        }

        [HttpPost]
        public ActionResult AttendanceRequest(bool status, int id, string remark)
        {
            bool res = _managerServices.AttendanceApproval(id, status, remark);
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

        [HttpGet]
        public ActionResult getAttendanceRepo(int month, int year, int EmpId)
        {
            int userObj = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            List<AttendanceManage> data = _managerServices.getReportAttendance(month, year, userObj, EmpId);
            if (data.Any())
            {
                return Json((object)new
                {
                    status = data.Any(),
                    data = data
                }, (JsonRequestBehavior)0);
            }
            return Json((object)new
            {
                status = data.Any(),
                message = "Data not found"
            }, (JsonRequestBehavior)0);
        }

        [HttpGet]
        public ActionResult Attendance_Report()
        {
            int userObj = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            ((ControllerBase)this).ViewData["UserList"] = _managerServices.selectAllOutSOurceList(userObj);
            ((ControllerBase)this).ViewData["attendanceCount"] = _managerServices.getAttendanceCount(userObj);
            return View();
        }

        [HttpGet]
        public JsonResult Attendance_ReportByMonth(int year, int month)
        {
            int userObj = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            List<AttendanceManage> data = _managerServices.getAttendanceCountByMonth(year, month, userObj);
            return Json((object)data, (JsonRequestBehavior)0);
        }

        public ActionResult ExportAttendanceToExcel(int month, int year, int EmpId)
        {
            int userObj = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            byte[] data = _managerServices.ConvertExcelFile(month, year, userObj, EmpId);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExcelReport.xlsx");
        }

        public ActionResult ShowAllAttendance(int year, int month)
        {
            int userObj = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            List<AllAttendance> data = _managerServices.GetAllAttendanceForPmByMonth(year, month, userObj);
            ((ControllerBase)this).ViewData["showAllAtt"] = data;
            ((ControllerBase)this).ViewData["AllAttendance"] = JsonConvert.SerializeObject((object)data);
            ((ControllerBase)this).ViewData["Year"] = year;
            ((ControllerBase)this).ViewData["Month"] = month;
            return View();
        }

        public ActionResult ExportAllAtt(int year, int month)
        {
            int userObj = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            byte[] data = _managerServices.ConvertExcelFileOfAll(month, year, userObj);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExcelReportOfAll.xlsx");
        }

        public ActionResult EmpMonthlyReport(int? month = null, int? year = null)
        {
            int userid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            ((ControllerBase)this).ViewData["projectList"] = _managerServices.All_Project_List(userid);
            ViewDataDictionary viewData = ((ControllerBase)this).ViewData;
            ManagerService managerServices = _managerServices;
            int? month2 = month;
            viewData["ReportList"] = managerServices.GetEmpReport(userid, null, null, null, month2, year);
            return View();
        }

        [HttpPost]
        public ActionResult InsertEmpReport(EmpReportModel model)
        {
            try
            {
                string message = "";
                int userid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
                model.PmId = userid;
                bool res = _managerServices.InsertEmpReport(model, out message);
                if (res)
                {
                    return Json((object)new
                    {
                        status = res,
                        message = "Report inserted successfully!"
                    });
                }
                return Json((object)new
                {
                    status = res,
                    message = message
                });
            }
            catch (Exception ex)
            {
                return Json((object)new
                {
                    status = false,
                    message = "Server error occurred: " + ex.Message
                });
            }
        }

        public ActionResult Feedback()
        {
            int userid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            ((ControllerBase)this).ViewData["feedbacklist"] = _managerServices.GetFeedbacks(userid);
            return View();
        }

        [HttpPost]
        public ActionResult InsertFeedback(FeedbackModel feed)
        {
            try
            {
                feed.UserId = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
                bool res = _managerServices.InsertFeedback(feed);
                return Json((object)new
                {
                    status = res,
                    message = "Feedback added successfully"
                });
            }
            catch
            {
                return Json((object)new
                {
                    status = false,
                    message = "Server error occurred"
                });
            }
        }

        [HttpPost]
        public JsonResult UpdateProjectStatus(UpdateProjectStatus upd)
        {
            try
            {
                bool res = _managerServices.InsertProjectStatus(upd);
                return Json((object)new
                {
                    status = res,
                    message = (res ? "Project status updated successfully" : "Some error occured")
                });
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

        [HttpGet]
        public JsonResult LastProjectStatusPrencentage(int projectid)
        {
            try
            {
                List<UpdateProjectStatus> data = _managerServices.LastProjectStatus(projectid);
                return Json((object)new
                {
                    status = (data.Count > 0),
                    message = ((data.Count > 0) ? "data recived" : "data not found"),
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

        [HttpPut]
        public ActionResult DeAllocateOutsource(int outsourceid)
        {
            try
            {
                bool res = _managerServices.DeAllocateManpower(outsourceid);
                return Json(new
                {
                    status = true,
                    message = "Deallocated Successfully"
                });
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

        #region reports Start
        public ActionResult ProjectReport(string type, string searchTerm = null, string statusFilter = null)
        {
            //IL_027f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0286: Expected O, but got Unknown
            //IL_0041: Unknown result type (might be due to invalid IL or missing references)
            //IL_0048: Expected O, but got Unknown
            //IL_0269: Unknown result type (might be due to invalid IL or missing references)
            //IL_0270: Expected O, but got Unknown
            UserCredential userData = _managerServices.getManagerDetails(User.Identity.Name);
            int managerId = Convert.ToInt32(userData.userId);
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
                AdminServices adminServices = _adminServices;
                int? projectManager = managerId;
                IEnumerable<RemoteSensingProject.Models.Admin.main.Project_model> data = adminServices.Project_List(null, null, null, searchTerm, statusFilter, projectManager);
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

        public ActionResult DownloadReimbursementReport(string type, string typeFilter = null, string statusFilter = null)
        {
            //IL_01b4: Unknown result type (might be due to invalid IL or missing references)
            //IL_01bb: Expected O, but got Unknown
            //IL_0041: Unknown result type (might be due to invalid IL or missing references)
            //IL_0048: Expected O, but got Unknown
            //IL_019e: Unknown result type (might be due to invalid IL or missing references)
            //IL_01a5: Expected O, but got Unknown
            try
            {
                UserCredential userData = _managerServices.getManagerDetails(User.Identity.Name);
                int projectManagerFilter = Convert.ToInt32(userData.userId);
                if (string.IsNullOrWhiteSpace(type))
                {
                    return (ActionResult)new HttpStatusCodeResult(500, "Report type is required");
                }
                List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
            {
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
                    Header = "Remark",
                    PropertyName = "remark"
                },
                new ApiCommon.ColumnMapping
                {
                    Header = "Status",
                    PropertyName = "statusLabel"
                }
            };
                ManagerService managerServices = _managerServices;
                int? managerId = projectManagerFilter;
                List<Reimbursement> data = managerServices.GetReimbursements(null, null, null, managerId, "selectReinbursementReport", typeFilter, statusFilter);
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

        public ActionResult DownloadTourProposalReport(string type, int? projectFilter = null, string statusFilter = null)
        {
            //IL_027d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0284: Expected O, but got Unknown
            //IL_0041: Unknown result type (might be due to invalid IL or missing references)
            //IL_0048: Expected O, but got Unknown
            //IL_0267: Unknown result type (might be due to invalid IL or missing references)
            //IL_026e: Expected O, but got Unknown
            try
            {
                UserCredential userData = _managerServices.getManagerDetails(User.Identity.Name);
                int managerFilter = Convert.ToInt32(userData.userId);
                if (string.IsNullOrWhiteSpace(type))
                {
                    return (ActionResult)new HttpStatusCodeResult(500, "Report type is required");
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
                AccountService accountService = _accountService;
                int? managerFilter2 = managerFilter;
                List<RemoteSensingProject.Models.Accounts.main.tourProposal> data = accountService.getTourList(null, null, managerFilter2, projectFilter, statusFilter);
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

        public ActionResult DownloadHiringVehicleReport(string type, int? projectFilter = null, string statusFilter = null)
        {
            //IL_02a1: Unknown result type (might be due to invalid IL or missing references)
            //IL_02a8: Expected O, but got Unknown
            //IL_0041: Unknown result type (might be due to invalid IL or missing references)
            //IL_0048: Expected O, but got Unknown
            //IL_028b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0292: Expected O, but got Unknown
            try
            {
                UserCredential userData = _managerServices.getManagerDetails(User.Identity.Name);
                int managerFilter = Convert.ToInt32(userData.userId);
                if (string.IsNullOrWhiteSpace(type))
                {
                    return (ActionResult)new HttpStatusCodeResult(500, "Report type is required");
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
                AdminServices adminServices = _adminServices;
                int? managerFilter2 = managerFilter;
                List<RemoteSensingProject.Models.Admin.main.HiringVehicle1> data = adminServices.HiringReort(null, null, managerFilter2, projectFilter, statusFilter);
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

        #endregion Reports End

        #region Division Head

        [Authorize(Roles = "divisionHead")]
        public ActionResult DivisionHead(string searchTerm)
        {
            int divisionid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).divisionId);
            ViewData["data"] = _managerServices.GetManpowerRequestsInDesignationPmWise(id: divisionid, searchTerm: searchTerm);
            return View();
        }

        [Authorize(Roles = "divisionHead")]
        public ActionResult ManPower(string searchTerm = null)
        {
            int divisionid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).divisionId);
            ViewData["manpowerrequestsindesignation"] = _managerServices.GetManpowerRequestsInDesignation(id: divisionid, searchTerm: searchTerm);
            return View();
        }
        [Authorize(Roles = "divisionHead")]
        public ActionResult ManageManpower(int id, string searchTerm = null)
        {
            int divisionid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).divisionId);
            ViewData["data"] = _managerServices.GetManpowerRequestsInDesignationPmWise(id: divisionid, designationid: id, searchTerm: searchTerm);
            return View();
        }
        [Authorize(Roles = "divisionHead")]
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
        [Authorize(Roles = "divisionHead")]
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
        #endregion

        #region Manage CM Dashboard

        [Authorize(Roles = "cmDashboard")]
        public ActionResult CMDashboardData()
        {
            ViewData["ProjectList"] = _adminServices.GetCmDashboardList(null, true);
            return View();
        }
        [Authorize(Roles = "cmDashboard")]
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
        [Authorize(Roles = "cmDashboard")]
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
        [Authorize(Roles = "cmDashboard")]
        public ActionResult GetCmPRojectById(int projectId)
        {
            RemoteSensingProject.Models.Admin.main.CMDashboardData data = _adminServices.GetCmDashboardList(projectId, true).FirstOrDefault();
            return Json((object)new { data }, (JsonRequestBehavior)0);
        }
        #endregion
    }
}