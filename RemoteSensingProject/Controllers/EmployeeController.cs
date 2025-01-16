using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RemoteSensingProject.Controllers
{
    [Authorize(Roles="projectManager")]
    public class EmployeeController : Controller
    {
        private readonly ManagerService _managerServices;
        // GET: Employee
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult Add_Project()
        {
            return View();
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
        public ActionResult GetAllProjectByManager(string userId)
        {

            List<ProjectList> _list = new List<ProjectList>();
            _list = _managerServices.getAllProjectByManager(userId);
            return Json(new {_list=_list},JsonRequestBehavior.AllowGet);
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