// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Controllers.HomeController
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using RemoteSensingProject.Controllers;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;

namespace RemoteSensingProject.Controllers
{
	public class HomeController : Controller
	{
		private readonly AdminServices _adminServices;
		private readonly ManagerService _managerServices;

        public HomeController()
        {
			_adminServices = new AdminServices();
			_managerServices = new ManagerService();
        }

      

		public ActionResult ContactUs()
		{
			return View();
		}

		public ActionResult Privacy_Policy()
		{
			Response.Cache.SetCacheability(HttpCacheability.Public);
			Response.Cache.SetMaxAge(TimeSpan.FromDays(1.0));
			return View();
		}

		[Authorize]
		public ActionResult Help_menu()
		{
			return View();
		}

		[Authorize]
		public ActionResult Support()
		{
			return View();
		}


		#region Project Details & Tour Detail View
		[Authorize(Roles = "admin,employee,accounts,cgp,divisionHead,projectManager")]
		public ActionResult Project_Details(int id)
		{
			ViewBag.projectId = id;
            var data = _adminServices.GetProjectById(id);
			return View(data);
		}
        [Authorize(Roles = "admin,employee,accounts,cgp,divisionHead,projectManager")]
        public ActionResult TourDetails(int id)
		{
			var data = _managerServices.GetTourDetails(id);
            return PartialView("_tourDetail",data);
		}
        #endregion
    }
}