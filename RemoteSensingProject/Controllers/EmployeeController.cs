using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Services.Description;
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
            ViewBag.TotalAssignProject = TotalCount.TotalAssignProject;
            ViewBag.TotaCompleteProject = TotalCount.TotaCompleteProject;
            ViewBag.TotalDelayProject = TotalCount.TotalDelayProject;
            ViewBag.TotalOngoingProject = TotalCount.TotalOngoingProject;
            ViewBag.TotalNotice = TotalCount.TotalNotice;
            ViewBag.TotalMeeting = TotalCount.TotalMeeting;
            return View();
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
        public ActionResult Add_Project()
        {
            var data = _adminServices.SelectEmployeeRecord();
            ViewBag.subOrdinateList = data.Where(d => d.EmployeeRole.Equals("subOrdinate")).ToList();

            return View();
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

                if (pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage.Equals("Yes"))
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
            ViewBag.WeeklyUpdateList = _managerServices.WeeklyUpdateList(Id);
            return View();
        }

        public JsonResult UpdateWeekly(Project_WeeklyUpdate pwu)
        {
            bool res = _managerServices.updateWeeklyStatus(pwu);
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
            if (status)
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
        public ActionResult Meetings()
        {
            var userId = _managerServices.getManagerDetails(User.Identity.Name);
            var res = _managerServices.getAllmeeting(int.Parse(userId.userId));
            return View(res);
        }


        public ActionResult GetMemberResponse(getMemberResponse mr)
        {
            var userId = _managerServices.getManagerDetails(User.Identity.Name);
            mr.MemberId = int.Parse(userId.userId);
            var res = _managerServices.GetResponseFromMember(mr);
            return Json(res);
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
        public ActionResult All_Project_Report()
        {
            var userObj = _managerServices.getManagerDetails(User.Identity.Name).userId;
            ViewData["ProjectList"] = _managerServices.All_Project_List(userObj);
            return View();
        }

        public ActionResult Pending_Project_Report()
        {
            return View();
        }

        public ActionResult Complete_Project_Report()
        {
            return View();
        }

       

    }
}