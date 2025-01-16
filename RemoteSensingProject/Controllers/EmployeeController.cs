using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
            if (pm.pm.projectDocument != null && pm.pm.projectDocument.FileName != "")
            {
                pm.pm.projectDocumentUrl = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(pm.pm.projectDocument.FileName);
                pm.pm.projectDocumentUrl = "/ProjectContent/ProjectManager/ProjectDocs/" + pm.pm.projectDocumentUrl;
            }

            if (pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage.Equals("Yes"))
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

            bool res = _adminServices.addProject(pm);
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
            return View();
        }
        #region /* Assign Project */
        public ActionResult Assigned_Project()
        {
           
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
        public ActionResult filterProject(string projectStatus)
        {
            var managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _managerServices.getManagerDetails(managerName);
            
            List<ProjectList> _list = new List<ProjectList>();
            if (Convert.ToInt32(projectStatus) == 1)
            {
                _list = _managerServices.getAllProjectByManager(userObj.userId).Where(e => e.CompleteionStatus == 0 && e.ApproveStatus == 1).ToList<ProjectList>();
            }else if (Convert.ToInt32(projectStatus) == 3)
            {
                _list=_managerServices.getAllProjectByManager(userObj.userId).Where(e=>e.CompleteionStatus==1 && e.ApproveStatus==1).ToList<ProjectList>();
            }else if(Convert.ToInt32(projectStatus) == 2)
            {
                _list = _managerServices.getAllProjectByManager(userObj.userId).Where(e => e.CompleteionStatus == 0 && e.ApproveStatus == 0).ToList<ProjectList>();
            }
            else
            {
                _list = _managerServices.getAllProjectByManager(userObj.userId);
            }
  
            return Json(new { _list=_list},JsonRequestBehavior.AllowGet);
        }
        #endregion /* End */
        public ActionResult Approved_Project()
        {
            return View();
        }
        public ActionResult Weekly_Update_Project()
        {
            return View();
        }

        public ActionResult Update_Project_Stage()
        {
            return View();
        }

        public ActionResult Add_Expenses()
        {
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

        public ActionResult Notice()
        {
            return View();
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