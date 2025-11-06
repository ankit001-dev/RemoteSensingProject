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

        public ActionResult RequestHistory()
        {
            ViewBag.ProjectList = _managerServices.All_Project_List(0, null, null, "AccountApproved");
            return View();
        }
        public ActionResult Meeting_List()
        {
            return View();
        }

        public ActionResult TourProposalRequest()
        {
            ViewData["projects"] = _adminServices.Project_List();
            ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
            ViewData["tourproposal"] = _accountSerivce.getTourList();
            return View();
        }
        public ActionResult ReinbursementRequest()
        {
            ViewData["ReimBurseData"] = _managerServices.GetReimbursements(type: "selectApprovedReinbursement");
            ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
            return View();
        }
        public ActionResult HiringRequest()
        {
            ViewData["hiringList"] = _adminServices.HiringReort();
            ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
            ViewData["projects"] = _adminServices.Project_List();
            return View();
        }
        public ActionResult FundReport(string req)
        {
            var data = _managerServices.All_Project_List(0, null, null, null);
            if (!string.IsNullOrWhiteSpace(req))
            {
                if (req.Equals("complete"))
                {
                    data = _managerServices.All_Project_List(0, null, null, "AccountApproved");
                }
                else if (req.Equals("pending"))
                {
                    data = _managerServices.All_Project_List(0, null, null, "AccountPending");
                }
            }
            ViewBag.ProjectList = data;
            return View();
        }

        public ActionResult Reimbursement_Report(string req)
        {
            ViewData["totalProjectManager"] = _adminServices.SelectEmployeeRecord().Where(d => d.EmployeeRole.Equals("projectManager")).ToList();
            var data = _managerServices.GetReimbursements(type: "accountrepo").ToList();
            if (!string.IsNullOrWhiteSpace(req))
            {
                if (req.Equals("approved"))
                {
                    data = data.Where(d => d.accountNewRequest == false && d.apprstatus == true && d.status == true).ToList();
                }else if (req.Equals("rejected"))
                {
                    data = data.Where(d => d.accountNewRequest == false && (d.apprstatus == false || d.status == false)).ToList();
                }
            }
            ViewData["totalReinursementReport"] = data;
            return View();
        }
        public ActionResult TourProposal_Report(string req)
        {
           if(req == "pending")
            {
                ViewData["allTourList"] = _accountSerivce.getTourList().Where(d => d.newRequest && !d.adminappr).ToList();
            } 
            else if (req == "approved")
            {
                ViewData["allTourList"] = _accountSerivce.getTourList().Where(d => !d.newRequest && d.adminappr).ToList();
            }
            else if (req == "rejected")
            {
                ViewData["allTourList"] = _accountSerivce.getTourList().Where(d => !d.newRequest && !d.adminappr).ToList();
            }
            ViewData["projects"] = _adminServices.Project_List();
            ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
            return View();
        }
        public ActionResult Hiring_Report(string req)
        {
            if (req == "pending")
            {
                ViewData["hiringList"] = _adminServices.HiringReort().Where(d => d.newRequest && !d.adminappr).ToList();
            }
            else if (req == "approved")
            {
                ViewData["hiringList"] = _adminServices.HiringReort().Where(d => !d.newRequest && d.adminappr).ToList();
            }
            else if (req == "rejected")
            {
                ViewData["hiringList"] = _adminServices.HiringReort().Where(d => !d.newRequest && !d.adminappr).ToList();
            }
            else
            {
                ViewData["hiringList"] = _adminServices.HiringReort();
            }
            ViewData["projectMangaer"] = _adminServices.SelectEmployeeRecord();
            ViewData["projects"] = _adminServices.Project_List();
            return View();
        }
    }
}