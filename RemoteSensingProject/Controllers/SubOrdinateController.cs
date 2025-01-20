using RemoteSensingProject.Models.Admin;
using static RemoteSensingProject.Models.SubOrdinate.main;
using RemoteSensingProject.Models.SubOrdinate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RemoteSensingProject.Controllers
{
    [Authorize(Roles = "subOrdinate")]
    public class SubOrdinateController : Controller
    {
        private readonly SubOrinateService _subOrdinate;
        private readonly AdminServices _adminServices;
        public SubOrdinateController()
        {
            _subOrdinate =new  SubOrinateService();
            _adminServices = new AdminServices();
            

        }
        // GET: SubOrdinate
        public ActionResult Dashboard()
        {
            return View();
        }
        #region Assigned Project
        public ActionResult Assigned_Project()
        {
            var managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _subOrdinate.getManagerDetails(managerName);

            List<ProjectList> _list = new List<ProjectList>();
            ViewData["AssignedProjectList"] = _subOrdinate.getProjectBySubOrdinate(userObj.userId);

            return View();
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

        #endregion End Assigned
        public ActionResult Meeting_List()
        {
            return View();
        }
        public ActionResult RejectList()
        {
            return View();
        }
        public ActionResult FundReport()
        {
            return View();
        }
    }
}