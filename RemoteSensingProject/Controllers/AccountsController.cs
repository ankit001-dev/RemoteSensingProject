// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Controllers.AccountsController
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;

namespace RemoteSensingProject.Controllers
{
	[Authorize(Roles = "accounts")]
	public class AccountsController : Controller
	{
		private readonly AccountService _accountSerivce;

		private readonly AdminServices _adminServices;

		private readonly ManagerService _managerServices;

		public AccountsController()
		{
			_accountSerivce = new AccountService();
			_adminServices = new AdminServices();
			_managerServices = new ManagerService();
		}

		public ActionResult Dashboard()
		{
			((dynamic)((ControllerBase)this).ViewBag).CompleteRequest = _managerServices.All_Project_List(0, null, null, "AccountApproved").Count();
			((dynamic)((ControllerBase)this).ViewBag).PendingStatus = _managerServices.All_Project_List(0, null, null, "AccountPending").Count();
			((dynamic)((ControllerBase)this).ViewBag).TotalFunRequest = _managerServices.All_Project_List(0).Count();
			RemoteSensingProject.Models.Accounts.main.DashboardCount TotalCount = _accountSerivce.DashboardCount();
			((ControllerBase)this).ViewData["projectlist"] = _managerServices.All_Project_List(0, 1, 5, "AccountApproved").Take(5).ToList();
			((ControllerBase)this).ViewData["graphdata"] = _accountSerivce.ExpencesListforgraph();
			((ControllerBase)this).ViewData["budgetdataforgraph"] = _accountSerivce.budgetdataforgraph();
			return View((object)TotalCount);
		}

		public ActionResult Requests()
		{
			((dynamic)((ControllerBase)this).ViewBag).ProjectList = _managerServices.All_Project_List(0, null, null, "AccountApproved");
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

		public ActionResult Expenses(int Id)
		{
			((ControllerBase)this).ViewData["ProjectStages"] = _managerServices.ProjectBudgetList(Id);
			return View();
		}

		public ActionResult UpdateExpensesResponse(RemoteSensingProject.Models.Accounts.main.HeadExpenses he)
		{
			bool res = _accountSerivce.UpdateExpensesResponse(he);
			return Json((object)res);
		}

		public ActionResult RequestHistory(string searchTerm = null)
		{
			((dynamic)((ControllerBase)this).ViewBag).ProjectList = _managerServices.All_Project_List(0, null, null, "AccountApproved", null, searchTerm);
			return View();
		}

		public ActionResult Meeting_List()
		{
			return View();
		}

		public ActionResult TourProposalRequest(int? managerFilter = null, int? projectFilter = null)
		{
			((ControllerBase)this).ViewData["projects"] = _adminServices.Project_List();
			((ControllerBase)this).ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
			ViewDataDictionary viewData = ((ControllerBase)this).ViewData;
			AccountService accountSerivce = _accountSerivce;
			int? managerFilter2 = managerFilter;
			viewData["tourproposal"] = accountSerivce.getTourList(null, null, managerFilter2, projectFilter);
			return View();
		}

		public ActionResult ReinbursementRequest()
		{
			((ControllerBase)this).ViewData["ReimBurseData"] = _managerServices.GetReimbursements(null, null, null, null, "selectApprovedReinbursement");
			((ControllerBase)this).ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
			return View();
		}

		public ActionResult HiringRequest(int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
		{
			ViewDataDictionary viewData = ((ControllerBase)this).ViewData;
			AdminServices adminServices = _adminServices;
			int? projectFilter2 = projectFilter;
			viewData["hiringList"] = adminServices.HiringReort(null, null, managerFilter, projectFilter2, statusFilter);
			((ControllerBase)this).ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
			((ControllerBase)this).ViewData["projects"] = _adminServices.Project_List();
			return View();
		}

		public ActionResult FundReport(string statusFilter = null)
		{
			List<RemoteSensingProject.Models.Admin.main.Project_model> data = _managerServices.All_Project_List(0);
			if (!string.IsNullOrWhiteSpace(statusFilter))
			{
				if (statusFilter.ToLower().Equals("complete"))
				{
					data = _managerServices.All_Project_List(0, null, null, "AccountApproved");
				}
				else if (statusFilter.ToLower().Equals("pending"))
				{
					data = _managerServices.All_Project_List(0, null, null, "AccountPending");
				}
			}
			((dynamic)((ControllerBase)this).ViewBag).ProjectList = data;
			return View();
		}

		public ActionResult Reimbursement_Report(int? projectManagerFilter = null, string typeFilter = null, string statusFilter = null)
		{
			((ControllerBase)this).ViewData["totalProjectManager"] = (from d in _adminServices.SelectEmployeeRecord()
																	  where d.EmployeeRole.Equals("projectManager")
																	  select d).ToList();
			ManagerService managerServices = _managerServices;
			int? managerId = projectManagerFilter;
			List<Reimbursement> data = managerServices.GetReimbursements(null, null, null, managerId, "accountrepo", typeFilter, statusFilter);
			((ControllerBase)this).ViewData["totalReinursementReport"] = data;
			return View();
		}

		public ActionResult TourProposal_Report(int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
		{
			ViewDataDictionary viewData = ((ControllerBase)this).ViewData;
			AccountService accountSerivce = _accountSerivce;
			int? projectFilter2 = projectFilter;
			viewData["allTourList"] = accountSerivce.getTourList(null, null, managerFilter, projectFilter2, statusFilter);
			((ControllerBase)this).ViewData["projects"] = _adminServices.Project_List();
			((ControllerBase)this).ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
			return View();
		}

		public ActionResult Hiring_Report(int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
		{
			ViewDataDictionary viewData = ((ControllerBase)this).ViewData;
			AdminServices adminServices = _adminServices;
			int? projectFilter2 = projectFilter;
			viewData["hiringList"] = adminServices.HiringReort(null, null, managerFilter, projectFilter2, statusFilter);
			((ControllerBase)this).ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
			((ControllerBase)this).ViewData["projects"] = _adminServices.Project_List();
			return View();
		}
	}
}