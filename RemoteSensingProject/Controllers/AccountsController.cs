using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RemoteSensingProject.Controllers
{
    [Authorize(Roles= "accounts")]
    public class AccountsController : Controller
    {
        // GET: Accounts
        public ActionResult Dashboard()
        {
            return View();
        }

       public ActionResult Requests()
        {
            return View();
        } 
        public ActionResult ApprovedList()
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