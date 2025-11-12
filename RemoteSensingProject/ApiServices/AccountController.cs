using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.UI;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Spreadsheet;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using static RemoteSensingProject.Models.Accounts.main;

namespace RemoteSensingProject.ApiServices
{
    [JwtAuthorize]
    public class AccountController : ApiController
    {
        // GET: Account
        private readonly AccountService _accountSerivce;
        private readonly ManagerService _mangerServices;
        public AccountController()
        {
            _accountSerivce = new AccountService();
            _mangerServices = new ManagerService();
        }

        [Route("api/getProjectList")]
        [HttpGet]
        public IHttpActionResult GetProjectList(int? page = null, int? limit = null)
        {
            var res = _mangerServices.All_Project_List(userId: 0, limit: limit, page: page, filterType: "AccountPending");
            return Ok(new { status = true, data = res, message = "data retrieved" });
        }
        [Route("api/getProjectHistoryList")]
        [HttpGet]
        public IHttpActionResult GetProjectHistoryList(int? page = null, int? limit = null)
        {
            var res = _mangerServices.All_Project_List(userId: 0, limit: limit, page: page, filterType: "ManagerProject").Where(e => e.ApproveStatus == true).ToList();
            return Ok(new { status = true, data = res, message = "data retrieved" });
        }
        [Route("api/ProjectBudgetList")]
        [HttpGet]
        public IHttpActionResult ProjectBudgetList(int projectId)
        {
            try
            {
                var res = _mangerServices.ProjectBudgetList(projectId);
                var selectprop = new[] { "Id", "Project_Id", "ProjectHeads", "ProjectAmount", "TotalAskAmount", "ApproveAmount" };
                var newdata = CommonHelper.SelectProperties(res, selectprop);
                if (newdata.Count > 0)
                {
                    return CommonHelper.Success(this, newdata, "Data fetched successfully", 200);
                }
                else
                {
                    return CommonHelper.NoData(this);
                }
            }
            catch (Exception ex)
            {
                return CommonHelper.Error(this, ex.Message);
            }

        }
        [Route("api/UpdateExpensesResponse")]
        [HttpPost]
        public IHttpActionResult UpdateExpensesResponse()
        {
            var httpRequest = HttpContext.Current.Request;
            HeadExpenses he = new HeadExpenses
            {
                Reason = httpRequest.Form.Get("reason"),
                Amount = Convert.ToInt32(httpRequest.Form.Get("amount")),
                ProjectId = Convert.ToInt32(httpRequest.Form.Get("projectId")),
                HeadId = Convert.ToInt32(httpRequest.Form.Get("headId")),
                AppStatus = Convert.ToInt32(httpRequest.Form.Get("approveStatus")),
                Id = Convert.ToInt32(httpRequest.Form.Get("expensesId")),
            };

            var res = _accountSerivce.UpdateExpensesResponse(he);
            if (res)
            {
                return Ok(new { status = true, statusCode = 200, message = "Response updated successfully" });
            }
            else
            {
                return Ok(new { status = false, statusCode = 500, message = "something went wrong" });

            }
        }

        [Route("api/getFundReport")]
        [HttpGet]
        public IHttpActionResult getFundReport(int?page = null,int?limit = null)
        {
            var res = _mangerServices.All_Project_List(userId: 0, limit: limit, page: page, filterType: "ManagerProject");
            return Ok(new { status = true, data = res, message = "data retrieved" });
        }

        [Route("api/getAccountDashboards")]
        [HttpGet]
        public IHttpActionResult getAccountDashboards()
        {
            int completeCount = _mangerServices.All_Project_List(userId: 0, limit: null, page: null, filterType: "ManagerProject").Count(e => e.ApproveStatus);
            int pendingCount = _mangerServices.All_Project_List(userId: 0, limit: null, page: null, filterType: "ManagerProject").Count(e => !e.ApproveStatus);
            int totalcount = _mangerServices.All_Project_List(userId: 0, limit: null, page: null, filterType: "ManagerProject").Count();
            return Ok(new { status = true, data = new { CompleteRequist = completeCount, PendingRequest = pendingCount, TotalRequest = totalcount }, message = "data retrieved" });
        }

        [System.Web.Mvc.AllowAnonymous]
        [Route("api/getTourById")]
        [HttpGet]
        public IHttpActionResult TourById(int id)
        {
            try
            {
                var data = _mangerServices.getTourList(id: id, type: "GetById");
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
        [System.Web.Mvc.AllowAnonymous]
        [HttpPost]
        [Route("api/approveReinbursementAmtRequest")]
        public IHttpActionResult InsertReinbursementForm()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var formData = new Models.Accounts.main.Reimbursement
                {
                    chequeNumber = request.Form.Get("chequeNum"),
                    date = Convert.ToDateTime(request.Form.Get("chequeDate")),
                    amount = Convert.ToDecimal(request.Form.Get("sanctionAmt")),
                    apprAmt = Convert.ToDecimal(request.Form.Get("apprAmount")),
                    id = Convert.ToInt32(request.Form.Get("id"))
                };

                bool res = _accountSerivce.reinbursementRequestAmt(formData);

                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Amount approved successfully !" : "Some issue occured while processing reuqest.."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }

        [System.Web.Mvc.AllowAnonymous]
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
            catch (Exception ex)
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

        [HttpGet]
        [Route("api/getDashboardCounts")]
        public IHttpActionResult DashboardCount()
        {
            try
            {
                var data = _accountSerivce.DashboardCount();
                return Ok(new
                {
                    status = true,
                    data = data,
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
        [HttpGet]
        [Route("api/graphData")]
        public IHttpActionResult GraphData()
        {
            try
            {
                var data = _accountSerivce.ExpencesListforgraph();
                return Ok(new
                {
                    status = true,
                    data = data,
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
        [HttpGet]
        [Route("api/budgetGraphData")]
        public IHttpActionResult BudgetGraphData()
        {
            try
            {
                var data = _accountSerivce.budgetdataforgraph();
                return Ok(new
                {
                    status = true,
                    data = data,
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
        [HttpGet]
        [Route("api/getAccountReinbursement")]
        public IHttpActionResult getAccountReinbursement()
        {
            try
            {
                var data = _mangerServices.GetReimbursements(type: "selectApprovedReinbursement");
                var selectProperties = new[] { "EmpName", "type", "id", "amount", "userId", "apprstatus", "subStatus", "adminappr", "status", "chequeNum", "accountNewRequest", "chequeDate", "newRequest", "approveAmount" };
                var filterData = CommonHelper.SelectProperties(data, selectProperties);
                if (data.Count > 0)
                {
                    return CommonHelper.Success(this, filterData, "Data fetched successfully", 200, data[0].Pagination);
                }
                else
                {
                    return CommonHelper.NoData(this);
                }
            }
            catch (Exception ex)
            {
                return CommonHelper.Error(this, ex.Message);
            }
        }
    }
}