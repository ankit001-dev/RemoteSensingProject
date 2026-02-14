// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.ApiServices.AccountController
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using static RemoteSensingProject.Models.CommonHelper;

namespace RemoteSensingProject.ApiServices
{
    [JwtAuthorize(Roles = "accounts,admin")]
    public class AccountController : ApiController
    {
        private readonly AccountService _accountSerivce;

        private readonly ManagerService _mangerServices;

        public AccountController()
        {
            _accountSerivce = new AccountService();
            _mangerServices = new ManagerService();
        }
        #region Manage Project
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
        #endregion

        #region Manage Budget & Expense
        [Route("api/ProjectBudgetList")]
        [HttpGet]
        public IHttpActionResult ProjectBudgetList(int projectId)
        {
            try
            {
                List<RemoteSensingProject.Models.Admin.main.Project_Budget> res = _mangerServices.ProjectBudgetList(projectId);
                string[] selectprop = new string[6] { "Id", "Project_Id", "ProjectHeads", "ProjectAmount", "TotalAskAmount", "ApproveAmount" };
                List<object> newdata = SelectProperties(res, selectprop);
                if (newdata.Count > 0)
                {
                    return Success(this, newdata, "Data fetched successfully");
                }
                return NoData(this);
            }
            catch (Exception ex)
            {
                return Error(this, ex.Message);
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
        #endregion

        #region Manage Dashboard
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
        [Route("api/budgetGraphData")]
        public IHttpActionResult BudgetGraphData()
        {
            try
            {
                var data = _accountSerivce.budgetdataforgraph();
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
        #endregion

        #region Add Tour Propoal

        // Add TourProposal
        [HttpPost]
        [Route("api/submitTourProposal")]
        public IHttpActionResult toursubmit()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;
                NameValueCollection form = request.Form;
                List<string> errors = new List<string>();
                if (string.IsNullOrWhiteSpace(form["projectId"]) || !int.TryParse(form["projectId"], out var _))
                {
                    errors.Add("Valid Project ID is required.");
                }
                string dateOfDeptStr = form["dateOfDept"];
                if (string.IsNullOrWhiteSpace(dateOfDeptStr) || !DateTime.TryParse(dateOfDeptStr, out var _))
                {
                    errors.Add("Valid Date of Departure is required.");
                }
                string place = form["place"];
                if (string.IsNullOrWhiteSpace(place))
                {
                    errors.Add("Place is required.");
                }
                string periodFromStr = form["periodFrom"];
                if (string.IsNullOrWhiteSpace(periodFromStr) || !DateTime.TryParse(periodFromStr, out var _))
                {
                    errors.Add("Valid Period From date is required.");
                }
                string periodToStr = form["periodTo"];
                if (string.IsNullOrWhiteSpace(periodToStr) || !DateTime.TryParse(periodToStr, out var _))
                {
                    errors.Add("Valid Period To date is required.");
                }
                string returnDateStr = form["returnDate"];
                if (string.IsNullOrWhiteSpace(returnDateStr) || !DateTime.TryParse(returnDateStr, out var _))
                {
                    errors.Add("Valid Return Date is required.");
                }
                string purpose = form["purpose"];
                if (string.IsNullOrWhiteSpace(purpose))
                {
                    errors.Add("Purpose is required.");
                }
                if (errors.Count > 0)
                {
                    return Error((ApiController)(object)this, string.Join(", ", errors));
                }
                tourProposal formdata = new tourProposal
                {
                    projectId = Convert.ToInt32(request.Form.Get("projectId")),
                    dateOfDept = Convert.ToDateTime(request.Form.Get("dateOfDept")),
                    place = request.Form.Get("place"),
                    periodFrom = Convert.ToDateTime(request.Form.Get("periodFrom")),
                    periodTo = Convert.ToDateTime(request.Form.Get("periodTo")),
                    returnDate = Convert.ToDateTime(request.Form.Get("returnDate")),
                    purpose = request.Form.Get("purpose"),
                    id = !string.IsNullOrEmpty(request.Form.Get("id")) ? Convert.ToInt32(request.Form.Get("id")) : 0
                };
                bool res = _mangerServices.insertTour(formdata);
                return Ok(new
                {
                    status = res,
                    StatusCode = (res ? 200 : 500),
                    message = res ? (formdata.id > 0 ? "Updated Successfully" : "Added successfully!") : "Error Occured"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region Add Expense
        [HttpPost]
        [Route("api/addProjectExpenses")]
        public IHttpActionResult AddExpenses()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;
                List<string> validationErrors = new List<string>();
                if (string.IsNullOrWhiteSpace(request.Form.Get("projectId")))
                {
                    validationErrors.Add("Project Id is required.");
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("projectHeadId")))
                {
                    validationErrors.Add("Project heads Id is required.");
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("title")))
                {
                    validationErrors.Add("Title is required.");
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("date")))
                {
                    validationErrors.Add("date is required.");
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("amount")))
                {
                    validationErrors.Add("Amount is required.");
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("description")))
                {
                    validationErrors.Add("Description is required.");
                }
                ProjectExpenses formData = new ProjectExpenses
                {
                    projectId = Convert.ToInt32(request.Form.Get("projectId")),
                    projectHeadId = Convert.ToInt32(request.Form.Get("projectHeadId")),
                    title = request.Form.Get("title"),
                    date = Convert.ToDateTime(request.Form.Get("date")),
                    amount = Convert.ToDecimal(request.Form.Get("amount")),
                    attatchment_url = request.Form.Get("attatchment_url"),
                    description = request.Form.Get("description")
                };
                HttpPostedFile file = request.Files["Attatchment_file"];
                if (file != null && file.FileName != "")
                {
                    formData.attatchment_url = DateTime.Now.ToString("ddMMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    formData.attatchment_url = Path.Combine("/ProjectContent/ProjectManager/HeadsSlip/", formData.attatchment_url);
                }
                if (validationErrors.Any())
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = string.Join("\n", validationErrors)
                    });
                }
                bool res = _mangerServices.insertExpences(formData);
                if (res && file != null && file.FileName != "")
                {
                    file.SaveAs(HttpContext.Current.Server.MapPath(formData.attatchment_url));
                }
                return Ok(new
                {
                    status = res,
                    StatusCode = (res ? 200 : 500),
                    message = (res ? "Expenses added successfully !" : "Some issue occured !")
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
        #endregion
        private IHttpActionResult BadRequest(object value)
        {
            return Content<object>(HttpStatusCode.BadRequest, value);
        }
    }
}