using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RemoteSensingProject.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Dashboard()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        #region Employee Category
        public ActionResult Emp_Category()
        {
            return View();
        }

        public ActionResult getCategoryPartial(string partial)
        {
            return PartialView(partial);
        }
        #endregion

        public ActionResult Employee_Registration()
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

        public ActionResult All_Projects()
        {
            return View();
        }
        public ActionResult Project_Request()
        {
            return View();
        }
        public ActionResult Min_Of_Meeting()
        {
            return View();
        }

        public ActionResult MeetingConclusion()
        {
            return View();
        }
        public ActionResult Project_Detail()
        {
            return View();
        }
        public ActionResult View_Weekly_Update()
        {
            return View();
        }

        public ActionResult View_Project_Stage()
        {
            return View();
        }

        public ActionResult View_Expenses()
        {
            return View();
        }

        public ActionResult Project_Report()
        {
            return View();
        }

        public ActionResult Meeting_Report()
        {
            return View();
        }

        public ActionResult Member_Report()
        {
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
            return View();
        }
        public ActionResult Notice_List()
        {
            return View();
        }

        public ActionResult View_Notice()
        {
            return View();
        }

        public ActionResult Expense_Report()
        {
            return View();
        }

    }
}