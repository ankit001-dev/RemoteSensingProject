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
using System.Web;
using System.Web.Mvc;
using static RemoteSensingProject.Models.Accounts.main;

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
			ViewData["dashboardCount"] = _accountSerivce.DashboardCount();
			((ControllerBase)this).ViewData["budgetdataforgraph"] = _accountSerivce.budgetdataforgraph();
			return View();
		}

        #region Manage Projects
		public ActionResult InternalProject(string searchTerm = null)
		{
            ViewBag.ProjectList = _managerServices.All_Project_List(searchTerm: searchTerm, projectTypeFilter: "Internal");
            return View();
		}
        public ActionResult ExternalProject(string searchTerm = null)
        {
            ViewBag.ProjectList = _managerServices.All_Project_List(searchTerm: searchTerm, projectTypeFilter: "External");
            return View();
        }
        #endregion

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

		public ActionResult Meeting_List()
		{
			return View();
		}

        #region Manage TourProposal
        public ActionResult TourProposalRequest(int? projectFilter = null)
		{
			((ControllerBase)this).ViewData["projectList"] = _managerServices.All_Project_List();
			ViewData["tourproposal"] = _managerServices.GetTourList(type:"ALLDATA",projectFilter:projectFilter);
			return View();
		}
        
        #endregion

        public ActionResult FundReport(string statusFilter = null)
		{
			List<Models.Admin.main.Project_model> data = _managerServices.All_Project_List(statusFilter:statusFilter, projectTypeFilter: "Internal");
			((dynamic)((ControllerBase)this).ViewBag).ProjectList = data;
			return View();
		}

        #region Manage Budget Heads
        public ActionResult Budget_Head()
        {
            ViewData["headlist"] = _adminServices.GetBudgetHeads();
            return View();
        }
        [HttpPost]
        public ActionResult InsertBudgetHead(RemoteSensingProject.Models.Admin.main.CommonResponse cr)
        {
            bool res = _adminServices.InsertBudgetHead(cr);
            return Json((object)new
            {
                status = res,
                message = (res ? (cr.id > 0 ? "Head updated successfully" : "Head inserted successfully!") : "Some issue found while processing your request !")
            }, (JsonRequestBehavior)0);
        }
        [HttpDelete]
        public ActionResult RemoveBudgetHead(int id)
        {
            bool res = _adminServices.DeleteBudgetHead(id);
            return Json((object)new
            {
                status = res,
                message = (res ? "Head removed successfully !" : "Some issue occred ")
            }, (JsonRequestBehavior)0);
        }
        #endregion

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

        #region New Module Adhisthan 
        public ActionResult Adhisthan()
        {
            ViewData["adhisthanList"] = _accountSerivce.GetAdhisthanList();
            return View();
        }
        [HttpPost]
        public ActionResult InsertAdhisthan(AdhisthanModel model)
        {
            try
            {
                bool res = _accountSerivce.InsertAdhisthan(model);
                return Json((object)new
                {
                    status = res,
                    message = (res ?(model.Id>0?"Data updated successfully": "Data added successfully !") : "Some issue occured !")
                });
            }
            catch (Exception ex)
            {
                return Json((object)new
                {
                    status = false,
                    message = "Server is busy !"
                });
            }
        }
        [HttpPost]
        public ActionResult InsertExpenditure(AdhisthanModel model)
        {
            try
            {
                bool res = _accountSerivce.InsertExpenditure(model);
                return Json((object)new
                {
                    status = res,
                    message = (res ?(model.Id>0?"Data updated successfully": "Data added successfully !") : "Some issue occured !")
                });
            }
            catch (Exception ex)
            {
                return Json((object)new
                {
                    status = false,
                    message = "Server is busy !"
                });
            }
        }
        #endregion

        #region Manage Update Committed
        [HttpPost]
        public ActionResult UpdateCommitted(UpdateCommitted model)
        {
            try
            {
                bool res = _accountSerivce.UpdateExpenseCommitted(model);
                return Json((object)new
                {
                    status = res,
                    message = (res ? "Data updated successfully"  : "Some issue occured !")
                });
            }
            catch (Exception ex)
            {
                return Json((object)new
                {
                    status = false,
                    message = "Server is busy !"
                });
            }
        }
        #endregion
    }
}