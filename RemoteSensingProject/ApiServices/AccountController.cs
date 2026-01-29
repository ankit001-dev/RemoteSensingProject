// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.ApiServices.AccountController
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;

namespace RemoteSensingProject.ApiServices
{
	[JwtAuthorize(Roles = "account,admin")]
	public class AccountController : ApiController
	{
		private readonly AccountService _accountSerivce;

		private readonly ManagerService _mangerServices;

		public AccountController()
		{
			_accountSerivce = new AccountService();
			_mangerServices = new ManagerService();
		}

		[Route("api/getProjectList")]
		[HttpGet]
		public IHttpActionResult GetProjectList(int? page = null, int? limit = null, string searchTerm = null)
		{
			List<RemoteSensingProject.Models.Admin.main.Project_model> res = _mangerServices.All_Project_List(0, limit, page, "AccountPending", null, searchTerm);
			return Ok(new
			{
				status = true,
				data = res,
				message = "data retrieved"
			});
		}

		[Route("api/getProjectHistoryList")]
		[HttpGet]
		public IHttpActionResult GetProjectHistoryList(int? page = null, int? limit = null, string searchTerm = null)
		{
			List<RemoteSensingProject.Models.Admin.main.Project_model> res = _mangerServices.All_Project_List(0, limit, page, "AccountApproved", null, searchTerm);
			return Ok(new
			{
				status = true,
				data = res,
				message = "data retrieved"
			});
		}

		[Route("api/ProjectBudgetList")]
		[HttpGet]
		public IHttpActionResult ProjectBudgetList(int projectId)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.Project_Budget> res = _mangerServices.ProjectBudgetList(projectId);
				string[] selectprop = new string[6] { "Id", "Project_Id", "ProjectHeads", "ProjectAmount", "TotalAskAmount", "ApproveAmount" };
				List<object> newdata = CommonHelper.SelectProperties(res, selectprop);
				if (newdata.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, newdata, "Data fetched successfully");
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[Route("api/UpdateExpensesResponse")]
		[HttpPost]
		public IHttpActionResult UpdateExpensesResponse()
		{
			HttpRequest httpRequest = HttpContext.Current.Request;
			RemoteSensingProject.Models.Accounts.main.HeadExpenses he = new RemoteSensingProject.Models.Accounts.main.HeadExpenses
			{
				Reason = httpRequest.Form.Get("reason"),
				Amount = Convert.ToInt32(httpRequest.Form.Get("amount")),
				ProjectId = Convert.ToInt32(httpRequest.Form.Get("projectId")),
				HeadId = Convert.ToInt32(httpRequest.Form.Get("headId")),
				AppStatus = Convert.ToInt32(httpRequest.Form.Get("approveStatus")),
				Id = Convert.ToInt32(httpRequest.Form.Get("expensesId"))
			};
			if (_accountSerivce.UpdateExpensesResponse(he))
			{
				return Ok(new
				{
					status = true,
					statusCode = 200,
					message = "Response updated successfully"
				});
			}
			return Ok(new
			{
				status = false,
				statusCode = 500,
				message = "something went wrong"
			});
		}

		[Route("api/getFundReport")]
		[HttpGet]
		public IHttpActionResult getFundReport(int? page = null, int? limit = null)
		{
			List<RemoteSensingProject.Models.Admin.main.Project_model> res = _mangerServices.All_Project_List(0, limit, page, "ManagerProject");
			return Ok(new
			{
				status = true,
				data = res,
				message = "data retrieved"
			});
		}

		[Route("api/getAccountDashboards")]
		[HttpGet]
		public IHttpActionResult getAccountDashboards()
		{
			int completeCount = _mangerServices.All_Project_List(0, null, null, "ManagerProject").Count((RemoteSensingProject.Models.Admin.main.Project_model e) => e.ApproveStatus);
			int pendingCount = _mangerServices.All_Project_List(0, null, null, "ManagerProject").Count((RemoteSensingProject.Models.Admin.main.Project_model e) => !e.ApproveStatus);
			int totalcount = _mangerServices.All_Project_List(0, null, null, "ManagerProject").Count();
			return Ok(new
			{
				status = true,
				data = new
				{
					CompleteRequist = completeCount,
					PendingRequest = pendingCount,
					TotalRequest = totalcount
				},
				message = "data retrieved"
			});
		}

		[AllowAnonymous]
		[Route("api/getTourById")]
		[HttpGet]
		public IHttpActionResult TourById(int id)
		{
			try
			{
				ManagerService mangerServices = _mangerServices;
				int? id2 = id;
				List<tourProposal> data = mangerServices.getTourList(null, id2, "GetById");
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
				List<RemoteSensingProject.Models.Accounts.main.tourProposal> data = _accountSerivce.getTourList();
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

		[AllowAnonymous]
		[HttpPost]
		[Route("api/approveReinbursementAmtRequest")]
		public IHttpActionResult InsertReinbursementForm()
		{
			try
			{
				HttpRequest request = HttpContext.Current.Request;
				RemoteSensingProject.Models.Accounts.main.Reimbursement formData = new RemoteSensingProject.Models.Accounts.main.Reimbursement
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
					StatusCode = (res ? 200 : 500),
					message = (res ? "Amount approved successfully !" : "Some issue occured while processing reuqest..")
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

		[AllowAnonymous]
		[HttpPost]
		[Route("api/rejectReinbursementAmountRequest")]
		public IHttpActionResult RejectReinbursementForm()
		{
			try
			{
				HttpRequest request = HttpContext.Current.Request;
				string reason = request.Form.Get("reason");
				int sanctionId = Convert.ToInt32(request.Form.Get("sanctionId"));
				bool res = _accountSerivce.rejectReinbursementRequestAmt(sanctionId, reason);
				return Json(new
				{
					status = res,
					message = (res ? "Reinbursement rejected successfully !" : "Some issue occured while processing your request.")
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
			return Content<object>(HttpStatusCode.BadRequest, value);
		}

		[HttpGet]
		[Route("api/getDashboardCounts")]
		public IHttpActionResult DashboardCount()
		{
			try
			{
				RemoteSensingProject.Models.Accounts.main.DashboardCount data = _accountSerivce.DashboardCount();
				return Ok(new
				{
					status = true,
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

		[HttpGet]
		[Route("api/graphData")]
		public IHttpActionResult GraphData()
		{
			try
			{
				RemoteSensingProject.Models.Accounts.main.GraphData data = _accountSerivce.ExpencesListforgraph();
				return Ok(new
				{
					status = true,
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

		[HttpGet]
		[Route("api/budgetGraphData")]
		public IHttpActionResult BudgetGraphData()
		{
			try
			{
				List<RemoteSensingProject.Models.Accounts.main.GraphData> data = _accountSerivce.budgetdataforgraph();
				return Ok(new
				{
					status = true,
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

		[HttpGet]
		[Route("api/getAccountReinbursement")]
		public IHttpActionResult getAccountReinbursement(int? page = null, int? limit = null, int? projectMangerFilter = null)
		{
			try
			{
				List<Reimbursement> data = _mangerServices.GetReimbursements(page, limit, null, projectMangerFilter, "selectApprovedReinbursement");
				string[] selectProperties = new string[14]
				{
				"EmpName", "type", "id", "amount", "userId", "apprstatus", "subStatus", "adminappr", "status", "chequeNum",
				"accountNewRequest", "chequeDate", "newRequest", "approveAmount"
				};
				List<object> filterData = CommonHelper.SelectProperties(data, selectProperties);
				if (data.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, filterData, "Data fetched successfully", 200, data[0].Pagination);
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}
	}
}