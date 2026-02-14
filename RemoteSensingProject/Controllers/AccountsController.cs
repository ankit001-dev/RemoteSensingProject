// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Controllers.AccountsController
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
			//RemoteSensingProject.Models.Accounts.main.DashboardCount TotalCount = _accountSerivce.DashboardCount();
			((ControllerBase)this).ViewData["projectlist"] = _managerServices.All_Project_List(0, 1, 5, "AccountApproved").Take(5).ToList();
			((ControllerBase)this).ViewData["graphdata"] = _accountSerivce.ExpencesListforgraph();
			((ControllerBase)this).ViewData["budgetdataforgraph"] = _accountSerivce.budgetdataforgraph();
			return View();
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
			((dynamic)((ControllerBase)this).ViewBag).ProjectList = _managerServices.All_Project_List(userId: null, searchTerm:searchTerm, filterType: "External");
            return View();
		}

		public ActionResult Meeting_List()
		{
			return View();
		}

        #region Manage TourProposal
        public ActionResult TourProposalRequest(int? projectFilter = null)
		{
			((ControllerBase)this).ViewData["projectList"] = _adminServices.Project_List();
			ViewData["tourproposal"] = _managerServices.GetTourList(type:"ALLDATA",projectFilter:projectFilter);
			return View();
		}
        
		public ActionResult TourProposal_Report(int? projectFilter = null)
		{
			ViewDataDictionary viewData = ((ControllerBase)this).ViewData;
			viewData["allTourList"] = _managerServices.GetTourList(type: "ALLDATA", projectFilter:projectFilter);
			((ControllerBase)this).ViewData["projects"] = _adminServices.Project_List();
			return View();
		}

        #endregion

        public ActionResult FundReport(string statusFilter = null)
		{
			List<Models.Admin.main.Project_model> data = _managerServices.All_Project_List(userId:null, statusFilter:statusFilter,filterType:"Internal");
			((dynamic)((ControllerBase)this).ViewBag).ProjectList = data;
			return View();
		}

		#region New Expense Changes
		[HttpPost]
        public ActionResult InsertExpenses(List<ProjectExpenses> list)
        {
            string filePage = Server.MapPath("~/ProjectContent/ProjectManager/HeadsSlip/");
            if (!Directory.Exists(filePage))
            {
                Directory.CreateDirectory(filePage);
            }
            if (list.Count > 0)
            {
                bool res = false;
                foreach (ProjectExpenses item in list)
                {
                    HttpPostedFileBase file = item.Attatchment_file;
                    if (file != null && file.FileName != "")
                    {
                        item.attatchment_url = DateTime.Now.ToString("ddMMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        item.attatchment_url = Path.Combine("/ProjectContent/ProjectManager/HeadsSlip/", item.attatchment_url);
                    }
                    res = _managerServices.insertExpences(item);
                    if (res && file != null && file.FileName != "")
                    {
                        file.SaveAs(Server.MapPath(item.attatchment_url));
                    }
                }
                return Json((object)new
                {
                    status = res,
                    message = (res ? "Project created successfully !" : "Some issue occured !")
                });
            }
            return Json((object)new
            {
                status = false,
                message = "Server is busy !"
            });
        }
        #endregion
    }
}