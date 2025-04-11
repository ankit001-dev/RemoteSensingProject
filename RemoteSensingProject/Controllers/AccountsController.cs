using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.SqlServer.Server;
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
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
        public AccountsController()
        {
            _accountSerivce = new AccountService();
            _adminServices = new AdminServices();
        }


        // GET: Accounts
        public ActionResult Dashboard()
        {
            ViewBag.CompleteRequest = _accountSerivce.Project_List().Count(e => e.ApproveStatus);
            ViewBag.PendingStatus = _accountSerivce.Project_List().Count(e => !e.ApproveStatus);
            ViewBag.TotalFunRequest = _accountSerivce.Project_List().Count();
            var TotalCount = _accountSerivce.DashboardCount();
            ViewData["projectlist"] = _accountSerivce.Project_List().Take(5).ToList();
            ViewData["graphdata"] = _accountSerivce.ExpencesListforgraph();
            ViewData["budgetdataforgraph"] = _accountSerivce.budgetdataforgraph();

            return View(TotalCount);
        }

        public ActionResult Requests()
        {
            ViewBag.ProjectList = _accountSerivce.Project_List().Where(e=>e.ApproveStatus==false).ToList();
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
            ViewData["ProjectStages"] = _accountSerivce.ProjectBudgetList(Id);
            return View();
        
        }

        public ActionResult UpdateExpensesResponse(HeadExpenses he)
        {
            var res = _accountSerivce.UpdateExpensesResponse(he);
           return  Json(res);
        }

        public ActionResult RequestHistory()
        {
            ViewBag.ProjectList = _accountSerivce.Project_List().Where(e => e.ApproveStatus == true).ToList();
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
            ViewData["ReimBurseData"] = _accountSerivce.GetReimbursements();
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
            ViewBag.ProjectList = req=="complete"? _accountSerivce.Project_List().Where(d => d.ApproveStatus == true).ToList():req=="pending"? _accountSerivce.Project_List().Where(d => d.ApproveStatus == false).ToList(): _accountSerivce.Project_List();
            return View();
        }

        public ActionResult Reimbursement_Report(string req)
        {
            ViewData["totalProjectManager"] = _adminServices.SelectEmployeeRecord().Where(d => d.EmployeeRole.Equals("projectManager")).ToList();
            ViewData["totalReinursementReport"] = req == "approved" ? _adminServices.ReinbursementReport().Where(d => d.newRequest == false && d.appr_status == true && d.status == true).ToList() : req == "rejected" ? _adminServices.ReinbursementReport().Where(d => d.newRequest == false && (d.appr_status == false || d.status == false)).ToList() : _adminServices.ReinbursementReport();
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