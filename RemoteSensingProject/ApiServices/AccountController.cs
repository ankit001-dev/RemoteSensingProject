  using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        [Route("api/getFundReport")]
        [HttpGet]
        public IHttpActionResult getFundReport()
        {
           var res = _accountSerivce.Project_List();
            return Ok(new { status = true, data = res, message = "data retrieved" });
        }
        
        [Route("api/getAccountDashboards")]
        [HttpGet]
        public IHttpActionResult getAccountDashboards()
        {
           int completeCount = _accountSerivce.Project_List().Count(e => e.ApproveStatus);
            int pendingCount = _accountSerivce.Project_List().Count(e => !e.ApproveStatus);
            int totalcount = _accountSerivce.Project_List().Count();
            return Ok(new { status = true, data = new {CompleteRequist= completeCount ,PendingRequest= pendingCount ,TotalRequest= totalcount }, message = "data retrieved" });
        }

        [Route("api/getTourById")]
        [HttpGet]
        public IHttpActionResult TourById(int id)
        {
            try
            {
                var data = _accountSerivce.getTourOne(id);
                return Ok(new
                {
                    status = data.Any(),
                    data = data
                });
            }
            catch
            {
                return Ok(new
                {
                    status=false,
                    StatusCode = 500,
                    message="Data not found"
                });
            }
        }

        [Route("api/getAllTour")]
        [HttpGet]
        public IHttpActionResult getAllTour()
        {
            try
            {
                var data = _accountSerivce.getTourList();
                return Ok(new
                {
                    status = data.Any(),
                    data = data
                });
            }
            catch
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = "Data not found"
                });
            }
        }

        [HttpPost]
        [Route("api/approveReinbursementAmtRequest")]
        public IHttpActionResult InsertReinbursementForm()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var formData = new Reimbursement
                {
                  chequeNumber = request.Form.Get("chequeNum"),
                 date = Convert.ToDateTime(request.Form.Get("chequeDate")),
                 amount = Convert.ToDecimal(request.Form.Get("sanctionAmt")),
                apprAmt= Convert.ToDecimal(request.Form.Get("apprAmount")),
                id = Convert.ToInt32(request.Form.Get("id"))
                };

                bool res = _accountSerivce.reinbursementRequestAmt(formData);

                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Amount approved successfully !" : "Some issue occured while processing reuqest.."
                });
            }catch(Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }


        [HttpPost]
        [Route("api/rejectReinbursementAmountRequest")]
        public IHttpActionResult RejectReinbursementForm()
        {
            try
            {
                var request = HttpContext.Current.Request;
                string reason = request.Form.Get("reason");
                int sanctionId = Convert.ToInt32(request.Form.Get("sanctionId"));
                bool res = _accountSerivce.rejectReinbursementRequestAmt(sanctionId, reason);
                return Json(new
                {
                    status = res,
                    message = res ? "Reinbursement rejected successfully !" : "Some issue occured while processing your request."
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }

        private IHttpActionResult BadRequest(object value)
        {
            return Content(HttpStatusCode.BadRequest, value);
        }
    }
}