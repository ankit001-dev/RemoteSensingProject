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
            var TotalCount = _adminServices.DashboardCount();
            ViewBag.totalEmployee = TotalCount.TotalEmployee;
            ViewBag.totalProject = TotalCount.TotalProject;
            ViewBag.totalDelayProject = TotalCount.TotalDelayproject;
            ViewBag.totalCompleteProject = TotalCount.TotalCompleteProject;
            ViewBag.totalOngoingProject = TotalCount.TotalOngoingProject;
            ViewBag.totalMeetings = TotalCount.TotalMeetings;

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

        [HttpPost]
        public ActionResult InsertDesgination(CommonResponse cr)
        {
            bool res = _adminServices.InsertDesgination(cr);
            return Json(new
            {
                status = res,
                message = res ? "Desgination inserted successfully!"  : "Some issue found while processing your request !"
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult InsertDivision(CommonResponse cr)
        {
            bool res = _adminServices.InsertDivison(cr);
            return Json(new
            {
                status = res,
                message = res ? "Divison inserted successfully!"  : "Some issue found while processing your request !"
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

        public ActionResult Employee_Registration()
        {

            ViewBag.division = _adminServices.ListDivison();
            ViewBag.designation = _adminServices.ListDesgination();

            ViewData["EmployeeList"] = _adminServices.SelectEmployeeRecord();
            return View();
        }

        [HttpPost]
        public ActionResult Employee_Registration(Employee_model emp)
        {
            string path = null;
            if (emp.EmployeeImages != null)
            {
                var fileName = Guid.NewGuid() + DateTime.Now.ToString("ddMMyyyyhhmm") + emp.EmployeeImages.FileName;
                  path = Path.Combine("/ProjectContent/Admin/Employee_Images", fileName);
                emp.Image_url = path;
            }


            bool res = _adminServices.AddEmployees(emp);

            
            if (res && emp.EmployeeImages!=null)
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
        [HttpGet]
        public ActionResult ChangeActieStatus(int id)
        {
            var res = _adminServices.ChangeActieStatus(id);
           
                return Json(res,JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult SelectEmployeeRecordById(int id)
        {
            var res = _adminServices.SelectEmployeeRecordById(id);
           
                return Json(res,JsonRequestBehavior.AllowGet);
        }

        #region add project
        public ActionResult Add_Project()
        {

            return View();
        }

        public ActionResult InsertProject(createProjectModel pm)
        {
            bool res = _adminServices.addProject(pm);
            return Json(new
            {
                status = res
            });
        }
        #endregion
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
        #region /* Meeting */
        public ActionResult Min_Of_Meeting()
        {
            List<Employee_model> empList = new List<Employee_model>();
            empList = _adminServices.BindEmployee();
            ViewBag.Employee = empList;

            return View();
        }

        #endregion End Meeting

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