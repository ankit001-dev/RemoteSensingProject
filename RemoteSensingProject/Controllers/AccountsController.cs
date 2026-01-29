using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.SqlServer.Server;
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static RemoteSensingProject.Models.Accounts.main;

namespace RemoteSensingProject.Controllers
{
    [Authorize(Roles= "accounts")]
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


        // GET: Accounts
        public ActionResult Dashboard()
        {
            ViewBag.CompleteRequest = _managerServices.All_Project_List(0, null, null, "AccountApproved").Count();
            ViewBag.PendingStatus = _managerServices.All_Project_List(0, null, null, "AccountPending").Count();
            ViewBag.TotalFunRequest = _managerServices.All_Project_List(0, null, null, null).Count();
            var TotalCount = _accountSerivce.DashboardCount();
            ViewData["projectlist"] = _managerServices.All_Project_List(0, 1, 5, "AccountApproved").Take(5).ToList();
            ViewData["graphdata"] = _accountSerivce.ExpencesListforgraph();
            ViewData["budgetdataforgraph"] = _accountSerivce.budgetdataforgraph();

            return View(TotalCount);
        }

        public ActionResult Requests()
        {
            ViewBag.ProjectList = _managerServices.All_Project_List(0, null, null, "AccountApproved");
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
        public ActionResult Expenses(int Id)
        {
            ViewData["ProjectStages"] = _managerServices.ProjectBudgetList(Id, null, null);
            return View();
        
        }

        public ActionResult UpdateExpensesResponse(HeadExpenses he)
        {
            var res = _accountSerivce.UpdateExpensesResponse(he);
           return  Json(res);
        }

        public ActionResult RequestHistory(string searchTerm = null)
        {
            ViewBag.ProjectList = _managerServices.All_Project_List(0, null, null, "AccountApproved",searchTerm:searchTerm);
            return View();
        }
        public ActionResult Meeting_List()
        {
            return View();
        }

        public ActionResult TourProposalRequest(int? managerFilter = null, int? projectFilter = null)
        {
            ViewData["projects"] = _adminServices.Project_List();
            ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
            ViewData["tourproposal"] = _accountSerivce.getTourList(managerFilter: managerFilter, projectFilter: projectFilter);
            return View();
        }
        public ActionResult ReinbursementRequest()
        {
            ViewData["ReimBurseData"] = _managerServices.GetReimbursements(type: "selectApprovedReinbursement");
            ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
            return View();
        }
        public ActionResult HiringRequest(int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
        {
            ViewData["hiringList"] = _adminServices.HiringReort(managerFilter: managerFilter, projectFilter: projectFilter, statusFilter: statusFilter);
            ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
            ViewData["projects"] = _adminServices.Project_List();
            return View();
        }
        public ActionResult FundReport(string statusFilter=null)
        {
            var data = _managerServices.All_Project_List(0, null, null, null);
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
            ViewBag.ProjectList = data;
            return View();
        }

        public ActionResult Reimbursement_Report(int? projectManagerFilter = null, string typeFilter = null, string statusFilter = null)
        {
            ViewData["totalProjectManager"] = _adminServices.SelectEmployeeRecord().Where(d => d.EmployeeRole.Equals("projectManager")).ToList();
            var data = _managerServices.GetReimbursements(type: "accountrepo", managerId: projectManagerFilter, typeFilter: typeFilter, statusFilter: statusFilter);
           
            ViewData["totalReinursementReport"] = data;
            return View();
        }
        public ActionResult TourProposal_Report(int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
        {
            ViewData["allTourList"] = _accountSerivce.getTourList(managerFilter: managerFilter, projectFilter: projectFilter, statusFilter: statusFilter);
            ViewData["projects"] = _adminServices.Project_List();
            ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
            return View();
        }
        public ActionResult Hiring_Report(int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
        {
            ViewData["hiringList"] = _adminServices.HiringReort(managerFilter: managerFilter, projectFilter: projectFilter, statusFilter: statusFilter);
            ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
            ViewData["projects"] = _adminServices.Project_List();
            return View();
        }
    }
}