using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RemoteSensingProject.Models.Admin;
using static RemoteSensingProject.Models.Admin.main;

namespace RemoteSensingProject.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        private readonly AdminServices _adminServices;
        public AdminController()
        {
            _adminServices = new AdminServices();   
        }
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

        public ActionResult InsertDesgination(CommonResponse cr)
        {
            bool res = _adminServices.InsertDesgination(cr);
            return Json(new
            {
                status = res,
                message = res ? "Desgination inserted successfully!"  : "Some issue found while processing your request !"
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InsertDivision(CommonResponse cr)
        {
            bool res = _adminServices.InsertDivison(cr);
            return Json(new
            {
                status = res,
                message = res ? "Divison inserted successfully!"  : "Some issue found while processing your request !"
            }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        public ActionResult Employee_Registration()
        {

            
            ViewBag.division = _adminServices.ListDivison();
            ViewBag.designation = _adminServices.ListDesgination();

            return View();
        }

        [HttpPost]
        public ActionResult Employee_Registration(Employee_model emp)
        {
            bool res = false;
            string path = null;
            if (emp.EmployeeImages != null)
            {
                var fileName = Guid.NewGuid() + DateTime.Now.ToString("ddMMyyyyhhmm") + emp.EmployeeImages.FileName;
                  path = Path.Combine("/ProjectContent/Admin/Employee_Images", fileName);
                emp.Image_url = path;
            }
            if (emp.Id != 0)
            {

             res = _adminServices.AddEmployees(emp);

            }
            else
            {
                res = _adminServices.AddEmployees(emp);

            }
            if (res)
            {

                emp.EmployeeImages.SaveAs(Server.MapPath(path));

            }
            return Json(res);
            
        }
        [HttpGet]
        public ActionResult DeleteEmployees(int id)
        {
            var res = _adminServices.RemoveEmployees(id);
           
                return Json(res,JsonRequestBehavior.AllowGet);
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