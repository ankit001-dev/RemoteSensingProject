// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Controllers.SubOrdinateController
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using RemoteSensingProject.Models.SubOrdinate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace RemoteSensingProject.Controllers
{
	[Authorize(Roles = "outSource")]
	public class SubOrdinateController : Controller
	{
		private readonly SubOrinateService _subOrdinate;
		private readonly AdminServices _adminServices;
		private readonly ManagerService _managerServices;
		public SubOrdinateController()
		{
			_subOrdinate = new SubOrinateService();
			_adminServices = new AdminServices();
			_managerServices = new ManagerService();
		}

		public ActionResult Dashboard()
		{
			string managerName = ((Controller)this).User.Identity.Name;
			int userId = Convert.ToInt32(_managerServices.getManagerDetails(managerName).userId);
			RemoteSensingProject.Models.SubOrdinate.main.DashboardCount dcount = _subOrdinate.GetDashboardCounts(Convert.ToInt32(userId));
			List<RemoteSensingProject.Models.SubOrdinate.main.ProjectList> _list = new List<RemoteSensingProject.Models.SubOrdinate.main.ProjectList>();
			ViewDataDictionary viewData = ((ControllerBase)this).ViewData;
			ManagerService managerServices = _managerServices;
			int? userId2 = 0;
			int? id = userId;
			viewData["AssignedProjectList"] = managerServices.All_Project_List(userId2, null, null, "SubordinateProject", id);
			return View((object)dcount);
		}

		public ActionResult Assigned_Project(string searchTerm = null, string statusFilter = null, string filterType = null)
		{
			string managerName = ((Controller)this).User.Identity.Name;
			int userId = Convert.ToInt32(_managerServices.getManagerDetails(managerName).userId);
			List<RemoteSensingProject.Models.SubOrdinate.main.ProjectList> _list = new List<RemoteSensingProject.Models.SubOrdinate.main.ProjectList>();
			List<RemoteSensingProject.Models.Admin.main.Project_model> data = _managerServices.All_Project_List(0, null, null, "SubordinateProject", userId, searchTerm, statusFilter);
			if (!string.IsNullOrEmpty(filterType))
			{
				data = data.Where((RemoteSensingProject.Models.Admin.main.Project_model d) => d.ProjectType.ToLower() == filterType.ToLower()).ToList();
			}
			((ControllerBase)this).ViewData["AssignedProjectList"] = data;
			return View();
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

		public ActionResult AddRaiseProblem(RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem formData)
		{
			string path = null;
			if (formData.Attachment != null && formData.Attachment.ContentLength > 0)
			{
				string filePage = ((Controller)this).Server.MapPath("~/ProjectContent/SubOrdinate/ProblemDocs");
				if (!Directory.Exists(filePage))
				{
					Directory.CreateDirectory(filePage);
				}
				Guid guid = Guid.NewGuid();
				string FileExtension = Path.GetExtension(formData.Attachment.FileName);
				string fileName = $"{guid}{FileExtension}";
				path = (formData.Attchment_Url = Path.Combine("/ProjectContent/SubOrdinate/ProblemDocs", fileName));
			}
			bool status = _subOrdinate.InsertSubOrdinateProblem(formData);
			if (status)
			{
				formData.Attachment.SaveAs(((Controller)this).Server.MapPath(path));
			}
			return Json((object)new { status }, (JsonRequestBehavior)0);
		}

		public ActionResult Meeting_List(string searchTerm = null, string statusFilter = null)
		{
			var userId = _subOrdinate.GetOutSourceId(User.Identity.Name);
			List<RemoteSensingProject.Models.Admin.main.Meeting_Model> res = _subOrdinate.getAllSubordinatemeeting(int.Parse(userId.userId), null, null, searchTerm, statusFilter);
			return View((object)res);
		}

		public ActionResult GetConclusions(int id)
		{
			UserCredential userId = _managerServices.getManagerDetails(((Controller)this).User.Identity.Name);
			List<GetConclusion> res = _managerServices.getConclusionForMeeting(id, int.Parse(userId.userId));
			return Json((object)res, (JsonRequestBehavior)0);
		}

		public ActionResult getPresentMember(int id)
		{
			List<RemoteSensingProject.Models.Admin.main.Employee_model> res = _adminServices.getPresentMember(id);
			return Json((object)res, (JsonRequestBehavior)0);
		}

		public ActionResult getKeypointResponse(int id)
		{
			List<RemoteSensingProject.Models.Admin.main.KeyPointResponse> res = _adminServices.getKeypointResponse(id);
			return Json((object)res, (JsonRequestBehavior)0);
		}

		public ActionResult GetMemberResponse(getMemberResponse mr)
		{
            var userData = _subOrdinate.GetOutSourceId(User.Identity.Name);
            mr.MemberId = int.Parse(userData.userId);
			bool res = _managerServices.GetResponseFromMember(mr);
			return Json((object)res);
		}

		public ActionResult ProjectAllTaskList()
		{
			var userData = _subOrdinate.GetOutSourceId(User.Identity.Name);

            ViewData["TaskList"] = _subOrdinate.getOutSourceTask(Convert.ToInt32(userData.userId));

            return View();
		}
		public ActionResult ProjectPendingTaskList()
		{
            var userData = _subOrdinate.GetOutSourceId(User.Identity.Name);

            ViewData["TaskList"] = _subOrdinate.getOutSourceTask(Convert.ToInt32(userData.userId), statusFilter: "Pending");

            return View();
		}
		public ActionResult ProjectCompleteTaskList()
		{
            var userData = _subOrdinate.GetOutSourceId(User.Identity.Name);

            ViewData["TaskList"] = _subOrdinate.getOutSourceTask(Convert.ToInt32(userData.userId), statusFilter: "Complete");

            return View();
		}

		[HttpPost]
		public JsonResult CompleteTask(Models.SubOrdinate.main.OutSource_Task task)
		{
            var userData = _subOrdinate.GetOutSourceId(User.Identity.Name);
			task.EmpId = Convert.ToInt32(userData.userId);
			bool result = _subOrdinate.AddOutSourceTask(task);
			return Json(new
			{
				status = result,
				message = result ? "Task Completed successfully. Please wait for admin approval." : "some issue found while updating task status"
			}, JsonRequestBehavior.AllowGet);
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