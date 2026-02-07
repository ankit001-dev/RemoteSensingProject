using RemoteSensingProject.Models.Admin;
using System;
using System.Linq;
using System.Security.Policy;
using System.Web.Mvc;

namespace RemoteSensingProject.Controllers
{
    public class CGPController : Controller
    {
        public readonly AdminServices _adminServices;
        public CGPController()
        {
            _adminServices = new AdminServices();
        }
        // GET: CGP
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
        public ActionResult AllProject(string searchTerm = null, string statusFilter = null, int? projectManager = null)
        {
            ((dynamic)((ControllerBase)this).ViewBag).ManagerList = (from d in _adminServices.SelectEmployeeRecord()
                                                                     where d.EmployeeRole.Contains("projectManager")
                                                                     select d).ToList();
            object viewBag = ((ControllerBase)this).ViewBag;
            AdminServices adminServices = _adminServices;
            ((dynamic)viewBag).ProjectList = adminServices.Project_List(null, null, null, searchTerm, statusFilter, projectManager);
            ViewBag.pageTitle = "All Project";
            return View();
        }
        public ActionResult AllInternalProject(string searchTerm=null,string statusFilter = null,int? projectManager = null)
        {
            ((dynamic)((ControllerBase)this).ViewBag).ManagerList = (from d in _adminServices.SelectEmployeeRecord()
                                                                     where d.EmployeeRole.Contains("projectManager")
                                                                     select d).ToList();
            object viewBag = ((ControllerBase)this).ViewBag;
            AdminServices adminServices = _adminServices;
            ((dynamic)viewBag).ProjectList = adminServices.Project_List(null, null, "Internal", searchTerm, statusFilter, projectManager);
            ViewBag.pageTitle = "Internal Project";
            return View("AllProject");
        }
        public ActionResult AllExternalProject(string searchTerm = null, string statusFilter = null, int? projectManager = null)
        {
            ((dynamic)((ControllerBase)this).ViewBag).ManagerList = (from d in _adminServices.SelectEmployeeRecord()
                                                                     where d.EmployeeRole.Contains("projectManager")
                                                                     select d).ToList();
            object viewBag = ((ControllerBase)this).ViewBag;
            AdminServices adminServices = _adminServices;
            ((dynamic)viewBag).ProjectList = adminServices.Project_List(null, null, "External", searchTerm, statusFilter, projectManager);
            ViewBag.pageTitle = "External Project";
            return View("AllProject");
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
    }
}