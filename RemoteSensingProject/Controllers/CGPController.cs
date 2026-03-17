using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Web.Mvc;

namespace RemoteSensingProject.Controllers
{
    public class CGPController : Controller
    {
        public readonly AdminServices _adminServices;
        private readonly ManagerService _manager;
        public CGPController()
        {
            _adminServices = new AdminServices();
            _manager = new ManagerService();
        }
        // GET: CGP
        public ActionResult Dashboard()
        {
            RemoteSensingProject.Models.Admin.main.DashboardCount TotalCount = _adminServices.DashboardCount();
            DateTime twoYearsAgo = DateTime.Now.AddYears(-2);
            ((ControllerBase)this).ViewData["physical"] = (from d in _manager.All_Project_List()
                                                           where d.AssignDate >= twoYearsAgo
                                                           select d).ToList();
            ((ControllerBase)this).ViewData["budgetGraph"] = _adminServices.ViewProjectExpenditure();
            return View((object)TotalCount);
        }
        public ActionResult AllProject(string searchTerm = null, string statusFilter = null, int? projectManager = null,string financialyear = null)
        {
            ((dynamic)((ControllerBase)this).ViewBag).ManagerList = (from d in _adminServices.SelectEmployeeRecord()
                                                                     where d.EmployeeRole.Contains("projectManager")
                                                                     select d).ToList();
            object viewBag = ((ControllerBase)this).ViewBag;
            AdminServices adminServices = _adminServices;
            ((dynamic)viewBag).ProjectList = _manager.All_Project_List(searchTerm:searchTerm, statusFilter:statusFilter, userId:projectManager, filterBy:(projectManager.HasValue ? "ProjectManager" : ""),financialyear:financialyear);
            ViewBag.pageTitle = "All Project";
            ViewData["financialyears"] = _manager.GetAllFinancialYears();
            return View();
        }
        public ActionResult AllInternalProject(string searchTerm=null,string statusFilter = null,int? projectManager = null,string financialyear = null)
        {
            ((dynamic)((ControllerBase)this).ViewBag).ManagerList = (from d in _adminServices.SelectEmployeeRecord()
                                                                     where d.EmployeeRole.Contains("projectManager")
                                                                     select d).ToList();
            object viewBag = ((ControllerBase)this).ViewBag;
            AdminServices adminServices = _adminServices;
            ((dynamic)viewBag).ProjectList = _manager.All_Project_List(projectTypeFilter:"Internal", searchTerm:searchTerm, statusFilter:statusFilter, userId: projectManager, filterBy:(projectManager.HasValue ? "ProjectManager" : ""),financialyear:financialyear);
            ViewBag.pageTitle = "Internal Project";
            ViewData["financialyears"] = _manager.GetAllFinancialYears();
            return View("AllProject");
        }
        public ActionResult AllExternalProject(string searchTerm = null, string statusFilter = null, int? projectManager = null, string financialyear = null)
        {
            ((dynamic)((ControllerBase)this).ViewBag).ManagerList = (from d in _adminServices.SelectEmployeeRecord()
                                                                     where d.EmployeeRole.Contains("projectManager")
                                                                     select d).ToList();
            object viewBag = ((ControllerBase)this).ViewBag;
            AdminServices adminServices = _adminServices;
            ((dynamic)viewBag).ProjectList = _manager.All_Project_List(projectTypeFilter: "External", searchTerm: searchTerm, statusFilter: statusFilter, userId: projectManager, filterBy: (projectManager.HasValue ? "ProjectManager" : ""), financialyear: financialyear);
            ViewBag.pageTitle = "External Project";
            ViewData["financialyears"] = _manager.GetAllFinancialYears();
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

        public ActionResult ViewProjectExpenditure(string req)
        {
            ViewData["ExpendedData"] = ((req == "expenditure") ? (from d in _adminServices.ViewProjectExpenditure()
                                                                  where d.expenditure > 0m
                                                                  select d).ToList() : ((req == "remaining") ? (from d in _adminServices.ViewProjectExpenditure()
                                                                                                                where d.remaining > 0m
                                                                                                                select d).ToList() : _adminServices.ViewProjectExpenditure()));
            return View();
        }
    }
}