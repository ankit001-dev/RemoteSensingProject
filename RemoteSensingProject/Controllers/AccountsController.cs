using Microsoft.AspNetCore.Mvc.Formatters.Internal;
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
            return View();
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
            ViewData["ProjectStages"] = _adminServices.ProjectBudgetList(Id);

            return View();
        
        }

        public ActionResult UpdateExpensesResponse(HeadExpenses he)
        {
            var res = _accountSerivce.UpdateExpensesResponse(he);
           return  Json(res);
        }

        public ActionResult RequestHistory()
        {
            ViewBag.ProjectList = _accountSerivce.Project_List().ToList();
            return View();
        } 
        public ActionResult FundReport()
        {
            return View();
        }    
       
    }
}