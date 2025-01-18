using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using static RemoteSensingProject.Models.Admin.main;

namespace RemoteSensingProject.Controllers
{
    [Authorize(Roles="projectManager")]
    public class EmployeeController : Controller
    {
        private readonly ManagerService _managerServices;
        public EmployeeController()
        {
            _managerServices = new ManagerService();
            _adminServices = new AdminServices();
        }
        private readonly AdminServices _adminServices;
       
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

            return View();
        }
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
        //public ActionResult filterProject(string projectStatus)
        //{
        //    var managerName = User.Identity.Name;
        //    UserCredential userObj = new UserCredential();
        //    userObj = _managerServices.getManagerDetails(managerName);

        //    List<ProjectList> _list = new List<ProjectList>();
        //    if (Convert.ToInt32(projectStatus) == 1)
        //    {
        //        _list = _managerServices.getAllProjectByManager(userObj.userId).Where(e => e.CompleteionStatus == 0 && e.ApproveStatus == 1).ToList<ProjectList>();
        //    }else if (Convert.ToInt32(projectStatus) == 3)
        //    {
        //        _list=_managerServices.getAllProjectByManager(userObj.userId).Where(e=>e.CompleteionStatus==1 && e.ApproveStatus==1).ToList<ProjectList>();
        //    }else if(Convert.ToInt32(projectStatus) == 2)
        //    {
        //        _list = _managerServices.getAllProjectByManager(userObj.userId).Where(e => e.CompleteionStatus == 0 && e.ApproveStatus == 0).ToList<ProjectList>();
        //    }
        //    else
        //    {
        //        _list = _managerServices.getAllProjectByManager(userObj.userId);
        //    }

        //    return Json(new { _list=_list},JsonRequestBehavior.AllowGet);
        //}
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
            if(formData.StageDocument !=null && formData.StageDocument.ContentLength > 0)
            {
                formData.StageDocument_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(formData.StageDocument.FileName);
                formData.StageDocument_Url = Path.Combine("/ProjectContent/ProjectManager/ProjectDocs/", formData.StageDocument_Url);
            }
            string message = "";
            bool status = _managerServices.insertStageStatus(formData);
            if (status)
            {
                if(formData.StageDocument !=null && formData.StageDocument.FileName != "")
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
            return Json(new {message=message,status=status },JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add_Expenses(int Id)
        {
            ViewData["ProjectStages"] = _adminServices.ProjectBudgetList(Id);
            return View();
        }
        public ActionResult Min_Of_Meeting()
        {
            return View();
        }

        public ActionResult Meeting_Conclusion()
        {
            return View();
        }
        public ActionResult Meetings()
        {
            return View();
        }

        public ActionResult MyProfile()
        {
            return View();
        }

        public ActionResult Notice(int? projectId)
        {
            dynamic noticeList = null;
            var managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);
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
        public ActionResult All_Project_Report()
        {
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