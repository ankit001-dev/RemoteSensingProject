using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using static RemoteSensingProject.Models.Accounts.main;

namespace RemoteSensingProject.ApiServices
{
    public class AccountController : ApiController
    {
        // GET: Account
        private readonly AccountService _accountSerivce;
        private readonly AdminServices _adminServices;
        public AccountController()
        {
            _accountSerivce = new AccountService();
            _adminServices = new AdminServices();
        }

        [Route("api/getProjectList")]
        [HttpGet]
        public IHttpActionResult GetProjectList()
        {
            var res = _accountSerivce.Project_List().Where(e => e.ApproveStatus == false).ToList();
            return Ok(new { status = true, data = res, message = "data retrieved" });
        }
        [Route("api/getProjectHistoryList")]
        [HttpGet]

        public IHttpActionResult GetProjectHistoryList()
        {
            var res = _accountSerivce.Project_List().Where(e => e.ApproveStatus == true).ToList();
            return Ok(new { status = true, data = res, message = "data retrieved" });
        }
          [Route("api/ProjectBudgetList")]
        [HttpGet]
        public IHttpActionResult ProjectBudgetList(int projectId)
        {
            var res=_accountSerivce.ProjectBudgetList(projectId);
            return Ok(new { status = true, data = res, message = "data retrieved" });
        }
        [Route("api/UpdateExpensesResponse")]
        [HttpPost]
        public IHttpActionResult UpdateExpensesResponse()
        {
            var httpRequest = HttpContext.Current.Request;
            HeadExpenses he = new HeadExpenses
            {
                Reason = httpRequest.Form.Get("reason"),
                Amount=Convert.ToInt32(httpRequest.Form.Get("amount")),
                ProjectId =Convert.ToInt32(httpRequest.Form.Get("projectId")),
                HeadId =Convert.ToInt32(httpRequest.Form.Get("headId")),
                AppStatus =Convert.ToInt32(httpRequest.Form.Get("approveStatus")),
                Id =Convert.ToInt32(httpRequest.Form.Get("expensesId")),
            };

            var res=_accountSerivce.UpdateExpensesResponse(he);
            if (res)
            {
                return Ok(new { status = true,statusCode=200, message = "Response updated successfully" });
            }
            else
            {
                return Ok(new { status = false,statusCode=500, message = "something went wrong" });

            }
        }


    }
}