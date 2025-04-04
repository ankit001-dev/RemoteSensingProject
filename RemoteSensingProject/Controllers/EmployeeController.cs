using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Xml.Linq;
using static RemoteSensingProject.Models.Admin.main;

namespace RemoteSensingProject.Controllers
{
    [Authorize(Roles="projectManager")]
    public class EmployeeController : Controller
    {
        private readonly ManagerService _managerServices;
        private readonly AdminServices _adminServices;
        public EmployeeController()
        {
            _managerServices = new ManagerService();
            _adminServices = new AdminServices();
        }
    
       
        // GET: Employee
        public ActionResult Dashboard()
        {
            var managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);
            var TotalCount = _managerServices.DashboardCount(userObj.userId);
            ViewData["emplist"] = _managerServices.All_Project_List(userObj.userId);
            return View(TotalCount);
        }
        public ActionResult BindOverallCompletionPercentage()
        {
               var managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);
           var list = _adminServices.getAllProjectCompletion().Where(e => e.ProjectManager == userObj.userId).ToList();
            return Json(new { list = list }, JsonRequestBehavior.AllowGet);

        }
        #region OutSource
        public ActionResult OutSource()
        {
            int userObj = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            ViewData["UserList"] = _managerServices.selectAllOutSOurceList(userObj);
            return View();
        }

        [HttpPost]
        public ActionResult CreateOutSource(OuterSource os)
        {
            var userObj = _managerServices.getManagerDetails(User.Identity.Name).userId;
            os.EmpId = Convert.ToInt32(userObj);
            bool res = _managerServices.insertOutSource(os);
            return Json(new
            {
                status = res,
                message = res ? "Outsource created succesfully !" : "Some issue occured !"
            });
        }
        #endregion

        #region  Task Assign
        public ActionResult CreateTask(string req)
        {
            int userObj = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            ViewData["OutSourceList"] = _managerServices.selectAllOutSOurceList(userObj);
            ViewData["TaskList"] = req=="completed"?_managerServices.taskList(userObj).Where(d=>d.completeStatus).ToList():req=="pending"? _managerServices.taskList(userObj).Where(d=>!d.completeStatus).ToList(): _managerServices.taskList(userObj);
            return View();
        }
        [HttpPost]
        public ActionResult CreateTaskJson(OutSourceTask ost)
        {
            ost.empId= Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            bool res = _managerServices.createTask(ost);
            return Json(new
            {
                status = res,
                message = res ? "Task created successfully !" : "Some issue occured !"
            });
        }
        #endregion
        public ActionResult Add_Project()
        {
            var data = _adminServices.SelectEmployeeRecord();
            ViewBag.subOrdinateList = data.Where(d => d.EmployeeRole.Equals("subOrdinate")).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult UpdateTaskStatus(int taskId)
        {
            bool res = _managerServices.updateTaskStatus(taskId);
            return Json(new
            {
                status = res,
                message = res ? "Task updated successfully !" : "Some issue occured !"
            });
        }
        public ActionResult InsertProject(createProjectModel pm)
        {
            var managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);
            if(userObj != null)
            {
                pm.pm.ProjectManager = userObj.userId;
            }
            if (pm.pm.projectDocument != null && pm.pm.projectDocument.FileName != "")
            {
                pm.pm.projectDocumentUrl = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(pm.pm.projectDocument.FileName);
                pm.pm.projectDocumentUrl = "/ProjectContent/ProjectManager/ProjectDocs/" + pm.pm.projectDocumentUrl;
            }

            if (pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage)
            {
                foreach (var item in pm.stages)
                {
                    if (item.Stage_Document != null && item.Stage_Document.FileName != "")
                    {
                        item.Document_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(item.Stage_Document.FileName);
                        item.Document_Url = "/ProjectContent/ProjectManager/ProjectDocs/" + item.Document_Url;
                    }
                }
            }

            bool res = _managerServices.addManagerProject(pm);

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
        public ActionResult Project_List()
        {
            var managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);
            ViewBag.ProjectList = _managerServices.Project_List(userObj.userId);
            return View();
        }
     
        public ActionResult All_Project_List()
        {
            var userObj = _managerServices.getManagerDetails(User.Identity.Name).userId;
            ViewData["ProjectList"] = _managerServices.All_Project_List(userObj);
            return View();
        }

        #region /* Assign Project */
        public ActionResult Assigned_Project()
        {
            var managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);

            List<ProjectList> _list = new List<ProjectList>();
            ViewData["AssignedProjectList"] = _managerServices.getAllProjectByManager(userObj.userId);
            return View();
        }
        public ActionResult GetAllProjectByManager()
        {
            var managerName=User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);
           
            List<ProjectList> _list = new List<ProjectList>();
            _list = _managerServices.getAllProjectByManager(userObj.userId);
            return Json(new {_list=_list},JsonRequestBehavior.AllowGet);
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

        #endregion /* End */


  
        public ActionResult Approved_Project()
        {
            return View();
        }
        #region weekly update
        public ActionResult Weekly_Update_Project(int Id)
        {
            ViewBag.WeeklyUpdateList = _managerServices.MonthlyProjectUpdate(Id);
            return View();
        }

        public ActionResult monthly_extr_ProjectStatue(int Id)
        {
            ViewData["totalData"] = _managerServices.GetExtrnlFinancialReport(Id);
            return View();
        }

        public JsonResult insertMonthlyExtrPrj(FinancialMonthlyReport fr)
        {
            bool res = _managerServices.updateFinancialReportMonthly(fr);
            return Json(new
            {
                status = res,
                message = res ? "Updated successfully !" : "Some issue occured while processing..."
            });
        }

        public JsonResult UpdateWeekly(Project_MonthlyUpdate pwu)
        {
            bool res = _managerServices.UpdateMonthlyStatus(pwu);
            return Json(new
            {
                status = res,
                message = res ? "Weekly updated successfully !" : "Some issue conflicted !"
            }); 
        }
        #endregion

    
        public ActionResult Update_Project_Stage(int Id)
        {

            ViewBag.ProjectStages = _managerServices.ProjectStagesList(Id);
            return View();
        }
        [HttpPost]
        public ActionResult AddStageStatus(Project_Statge formData)
        {
            if ((formData.StageDocument != null && formData.StageDocument.ContentLength > 0) ||
                     (formData.DelayedStageDocument != null && formData.DelayedStageDocument.ContentLength > 0))
            {
                string fileName = formData.StageDocument?.FileName ?? formData.DelayedStageDocument?.FileName;
                formData.StageDocument_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(fileName);
                formData.StageDocument_Url = Path.Combine("/ProjectContent/ProjectManager/ProjectDocs/", formData.StageDocument_Url);
            }

            string message = "";
            bool status = _managerServices.InsertStageStatus(formData);
            if (status)
            {
                if (formData.StageDocument != null && formData.StageDocument.FileName != "")
                {
                    formData.StageDocument.SaveAs(Server.MapPath(formData.StageDocument_Url));
                }
            }
            if (status)
            {

                message = "Satges Updated Successfully !";
            }
            else
            {
                message = "Failed To Update Stages";
            }
            return Json(new { message = message, status = status }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult viewStagesReason(string stageId)
        {
            List<Project_Statge> stageList = new List<Project_Statge>();
            stageList = _managerServices.ViewStagesComments(stageId);
            return Json(new { stageList = stageList }, JsonRequestBehavior.AllowGet);
        }
        #region Project expenses
        public ActionResult Add_Expenses(int Id)
        {
            ViewData["ProjectStages"] = _adminServices.ProjectBudgetList(Id);
            return View();
        }

        public ActionResult InsertExpenses(List<ProjectExpenses> list)
        {
            if(list.Count > 0)
            {
                bool res = false;
                foreach(var item in list)
                {
                    var file = item.Attatchment_file;
                    if(file != null && file.FileName != "")
                    {
                        item.attatchment_url = DateTime.Now.ToString("ddMMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        item.attatchment_url = Path.Combine("/ProjectContent/ProjectManager/HeadsSlip/", item.attatchment_url);
                    }
                   res = _managerServices.insertExpences(item);

                    if (res)
                    {
                        if(file != null && file.FileName != "")
                        {
                            file.SaveAs(Server.MapPath(item.attatchment_url));
                        }
                    }
                }

                return Json(new
                {
                    status = res,
                    message = res ? "Project created successfully !" : "Some issue occured !"
                });
            }
            else
            {
                return Json(new
                {
                    status = false,
                    message  = "Server is busy !"
                });
            }
        }
        #endregion
        public ActionResult Min_Of_Meeting()
        {

            var empList = _adminServices.BindEmployee().Where(e=>e.EmployeeRole=="subOrdinate");
            return View(empList);
        }

        public ActionResult GetConclusions(int id)
        {
            var userId = _managerServices.getManagerDetails(User.Identity.Name);
            var res = _managerServices.getConclusionForMeeting(id, int.Parse(userId.userId));
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddMeeting(AddMeeting_Model formData)
        {
            string path = null;
            if (formData.Attachment != null && formData.Attachment.ContentLength > 0)
            {
                var guid = Guid.NewGuid();
                var FileExtension = Path.GetExtension(formData.Attachment.FileName);
                var fileName = $"{guid}{FileExtension}";
                path = Path.Combine("/ProjectContent/ProjectManager/Meeting_Attachment", fileName);

                formData.Attachment_Url = path;
            }
            var userId = _managerServices.getManagerDetails(User.Identity.Name);
            formData.CreaterId = int.Parse(userId.userId);
            bool status = _adminServices.insertMeeting(formData);
            if (status && formData.Attachment != null)
            {
                formData.Attachment.SaveAs(Server.MapPath(path));
            }
            return Json(new { success = status }, JsonRequestBehavior.AllowGet);
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
                path = Path.Combine("/ProjectContent/Admin/Meeting_Attachment", fileName);

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
        public ActionResult GetAllMeeting()
        {
            var id = _managerServices.getManagerDetails(User.Identity.Name).userId;
            List<Meeting_Model> empList = new List<Meeting_Model>();
            empList = _adminServices.getAllmeeting().Where(e=>e.CreaterId.ToString()==id).ToList();
            return Json(new { empList = empList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Meeting_Conclusion(int meeting)
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
        public ActionResult Meetings(string req)
        {
            var userId = _managerServices.getManagerDetails(User.Identity.Name);
            var res = req=="admin"? _managerServices.getAllmeeting(int.Parse(userId.userId)).Where(d => d.createdBy == "admin").ToList(): req=="projectmanager"? _managerServices.getAllmeeting(int.Parse(userId.userId)).Where(d=>d.createdBy == "projectmanager").ToList(): _managerServices.getAllmeeting(int.Parse(userId.userId));
            return View(res);
        }

        public ActionResult GetMemberResponse(getMemberResponse mr)
        {
            var userId = _managerServices.getManagerDetails(User.Identity.Name);
            mr.MemberId = int.Parse(userId.userId);
            var res = _managerServices.GetResponseFromMember(mr);
            return Json(res);
        }

        [HttpGet]
        public ActionResult GetMemberJoiningStatusByMeeting(int meetingId)
        {

            var res = _managerServices.getMemberJoiningStatus(meetingId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyProfile()
        {
            return View();
        }

        public ActionResult Notice(int? projectId)
        {
            dynamic noticeList = null;
            var managerName = User.Identity.Name;
            UserCredential userObj = _managerServices.getManagerDetails(managerName);
            if (projectId.HasValue)
            {
                noticeList = _adminServices.getNoticeList().Where(e => e.ProjectManagerId == projectId).ToList();
            }
            else
            {
                noticeList = _managerServices.getNoticeList(userObj.userId);


            }
            ViewBag.ProjectList = _adminServices.Project_List();

            ViewData["NoticeList"] = noticeList;

            return View();
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
        public ActionResult SubOrdinateProblemList()
        {
            var managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);
            ViewBag.ProjectProblemList = _managerServices.getAllSubOrdinateProblem(userObj.userId);
            return View();
        }
        [HttpGet]
        public ActionResult SubordinateProblemListById(int id)
        {
            var managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);
            var data = _managerServices.getAllSubOrdinateProblemById(userObj.userId, id);
            return Json(data,JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult updateProjectProblemStatus(int problemId)
        {
            bool res = _managerServices.CompleteSelectedProblem(problemId);
            return Json(new
            {
                status = res,
                message = res ? "Problem solved successfully !" : "Some issue occured.  Try after sometime."
            });
        }
        public ActionResult All_Project_Report(string req)
        {
            var userObj = _managerServices.getManagerDetails(User.Identity.Name).userId;
            ViewData["ProjectList"] = req == "completed" ?  _managerServices.All_Project_List(userObj).Where(d => d.CompletionDate < DateTime.Now && d.StartDate<DateTime.Now && d.completestatus == true).ToList() : req == "delay" ?  _managerServices.All_Project_List(userObj).Where(d => d.completestatus == false && d.CompletionDate <= DateTime.Now).ToList() : req == "ongoing" ?  _managerServices.All_Project_List(userObj).Where(d => d.StartDate < DateTime.Now && d.CompletionDate > DateTime.Now).ToList() : _managerServices.All_Project_List(userObj);
            return View();
        }

        public ActionResult Pending_Project_Report()
        {
            var userObj = _managerServices.getManagerDetails(User.Identity.Name).userId;
            ViewData["NotStartedProject"] = _managerServices.GetNotStartedProject_List(userObj);
            return View();
        }

        public ActionResult Complete_Project_Report()
        {
            var userObj = _managerServices.getManagerDetails(User.Identity.Name).userId;
            ViewData["CompleteProjectList"] = _managerServices.GetCompleteProject_List(userObj);
            return View();
        }

        public ActionResult Expense_Report()
        {
            var userObj = _managerServices.getManagerDetails(User.Identity.Name).userId;
            ViewBag.ProjectList = _managerServices.All_Project_List(userObj);
            return View();
        }

        public ActionResult Reimbursement_Form()
        {
            int id = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            var res = _managerServices.GetReinbursementDatabyType(id);
            ViewData["reimlist"] = res;
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
                    foreach (var item in data)
                    {
                        item.userId = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
                        bool res = _managerServices.insertReimbursement(item);
                        if (res)
                            i++;
                    }
                }
                return Json(new
                {
                    status = i == data.Count,
                    message = i == data.Count ? "Added Successfully" : "Some issue occured while processing some data ."
                });
            }
            catch(Exception ex)
            {
                return Json(new {status = false, message = ex.Message});
            }
           
        }

        public ActionResult ViewReinbursementListByType(string type, int id)
        {
            int userId = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            var data = _managerServices.GetSpecificUserReimbursements(userId, type, id);
            return Json(new
            {
                status = data.Any(),
                data = data,

            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SubmitReinbursementFormType(string type, int Id)
        {
            int userId = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);

            bool res = _managerServices.submitReinbursementForm(type, userId, Id);

            return Json(new
            {
                status = res,
                message = res ? "Submitted successfully !" : "Some issue occured !"
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Hiring_Vehicle()
        {
            int userid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            var res = _managerServices.getProjectList(userid);
            ViewData["projectlist"] = res;
            var res1 = _managerServices.GetHiringVehicles(userid);
            ViewData["hiringList"] = res1;
            ViewData["projects"] = _adminServices.Project_List();
            return View();
        }
        [HttpPost]
        public ActionResult Hiring_Vehicle(HiringVehicle data)
        {
            int userid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            data.userId = userid;
            bool res = _managerServices.insertHiring(data);
            if (res)
            {
                return Json(new
                {
                    status = res,
                    message = "Added Successfully"
                });
            }
            else
            {
                return Json(new
                {
                    status = res,
                    message = "Something went wrong"
                });
            }
        }
        public ActionResult Tour_Proposal()
        {
            int userid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            var res = _managerServices.getTourList(userid);
            var res1 = _managerServices.getProjectList(userid);
            ViewData["projectList"] = res1;
            ViewData["tourList"] = res;
            ViewData["projects"] = _adminServices.Project_List();
            return View();
        }
        [HttpPost]
        public ActionResult Tour_Proposal(tourProposal data)
        {
            data.userId = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            bool res = _managerServices.insertTour(data);
                if (res)
                {
                    return Json(new
                    {
                        status = res,
                        message = "Added Successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        status = res,
                        message = "Something went wrong"
                    });
                }
        }

        public ActionResult Reimbursement_Report(string req)
        {
            int userId = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            ViewData["totalReinursementReport"] = req=="approved"?_managerServices.reinbursementReport(userId).Where(d => d.newRequest== false && d.adminappr == true).ToList():req == "rejected"? _managerServices.reinbursementReport(userId).Where(d=>d.newRequest==false&&d.adminappr==false).ToList(): _managerServices.reinbursementReport(userId);
            return View();
        }
        public ActionResult TourProposal_Report(string req)
        {
            int userid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            ViewData["tourList"] = req=="approved"? _managerServices.getTourList(userid).Where(d=>d.newRequest == false && d.adminappr == true).ToList():req=="rejected"? _managerServices.getTourList(userid).Where(d=>d.newRequest&&d.adminappr ==false).ToList(): req=="pending"? _managerServices.getTourList(userid).Where(d => d.newRequest == true && d.adminappr == false).ToList(): _managerServices.getTourList(userid);
            ViewData["projects"] = _adminServices.Project_List();
            ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
            return View();
        }
        public ActionResult Hiring_Report(string req)
        {
            int userid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            var res = _managerServices.getProjectList(userid);
            ViewData["projectlist"] = res;
            var res1 = req == "approved" ? _managerServices.GetHiringVehicles(userid).Where(d => d.newRequest == false && d.adminappr == true).ToList() : req =="rejected"? _managerServices.GetHiringVehicles(userid).Where(d=>d.newRequest==false && d.adminappr == false).ToList(): req=="pending"? _managerServices.GetHiringVehicles(userid).Where(d=>d.newRequest == true&&d.adminappr == false).ToList(): _managerServices.GetHiringVehicles(userid);
            ViewData["hiringList"] = res1;
            ViewData["projects"] = _adminServices.Project_List();
            return View();
        }
        public ActionResult RaiseProblem()
        {
            int userid = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            ViewData["projectList"] = _managerServices.All_Project_List(userid.ToString()); 
            ViewData["ProblemList"] = _managerServices.getProblems(userid);
            return View();
        }
        [HttpPost]
        public  ActionResult RaiseProblem(RaiseProblem dt)
        {
            dt.id = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            if (dt.document.ContentLength > 0)
            {
                dt.documentname = $"/ProjectContent/ProjectManager/raisedproblem{Guid.NewGuid()}{dt.document.FileName}";
            }
            bool res = _managerServices.insertRaisedProblem(dt);
            if (res)
            {
                    if(dt.document.ContentLength>0)
                    dt.document.SaveAs(Server.MapPath(dt.documentname));
                return Json(new
                {
                    status = res,
                    message = "Problem raised successfully !"
                });
            }
            return Json(new
            {
                status = res,
                message = "Some issue occured !"
            });
        }
        [HttpGet]
        public ActionResult DeleteRaiseProblem(int id)
        {
            int userId = Convert.ToInt32(_managerServices.getManagerDetails(User.Identity.Name).userId);
            bool res = _managerServices.deleteRaisedProblem(id,userId);
            if (res)
            {
                return Json(new
                {
                    status = res,
                    message = "Deleted Successfully"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    status = res,
                    message = "Something wend wrong.",
                },
                    JsonRequestBehavior.AllowGet
                );
            }
        }
    }
}