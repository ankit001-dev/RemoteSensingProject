using RemoteSensingProject.Models.Admin;
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
            return View();
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