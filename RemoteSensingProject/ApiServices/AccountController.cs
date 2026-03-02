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
            List<RemoteSensingProject.Models.Admin.main.Project_model> res = _mangerServices.All_Project_List(limit:limit,page:page, filterBy:"AccountPending", searchTerm:searchTerm);
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
            List<RemoteSensingProject.Models.Admin.main.Project_model> res = _mangerServices.All_Project_List(limit:limit, page:page, filterBy:"AccountApproved", searchTerm: searchTerm);
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

        #region Manage Adhisthan
        [HttpPost]
        [Route("api/add-adhisthan")]
        public IHttpActionResult AddAdhisthan(main.AdhisthanModel ad)
        {
            try
            {
                bool res = _accountSerivce.InsertAdhisthan(ad);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 201 : 400,
                    message = res ? (ad.Id > 0 ? "Data updated successfully" : "Data added successfully !") : "Some issue occured !"
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("api/get-adhisthan")]
        public IHttpActionResult GetAdhisthan()
        {
            try
            {
                var data = _accountSerivce.GetAdhisthanList();
                return Ok(new
                {
                    status = data.Any(),
                    data = data,
                    message = data.Any() ? "Data found" : "Data not found"
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }
        [HttpPost]
        [Route("api/add-expenditure")]
        public IHttpActionResult AddExpenditure(main.AdhisthanModel ad)
        {
            try
            {
                bool res = _accountSerivce.InsertExpenditure(ad);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 201 : 400,
                    message = res ?  "Data added successfully !" : "Some issue occured !"
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }
        [HttpPost]
        [Route("api/add-committed")]
        public IHttpActionResult AddCommitted(main.UpdateCommitted ad)
        {
            try
            {
                bool res = _accountSerivce.UpdateExpenseCommitted(ad);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 201 : 400,
                    message = res ?  "Data updated successfully !" : "Some issue occured !"
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }
        [HttpPost]
        [Route("api/add-committed-heads")]
        public IHttpActionResult AddCommittedInHeads(main.UpdateCommitted ad)
        {
            try
            {
                bool res = _accountSerivce.UpdateExpenseCommittedInHeads(ad);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 201 : 400,
                    message = res ?  "Data updated successfully !" : "Some issue occured !"
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }

        #endregion

        #region Manage Project List 
        [HttpGet]
        [Route("api/get-internal-projects")]
        public IHttpActionResult GetInternalProjectList(string searchTerm = null,int? limit = null,int? page = null,string statusFilter = null)
        {
            string[] selectProperties = new string[23]
           {
                "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl",
                "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode",
                "ProjectDepartment", "ContactPerson", "Address"
           };
            var data = _mangerServices.All_Project_List(searchTerm: searchTerm, projectTypeFilter: "Internal",limit:limit,page:page,statusFilter:statusFilter);
            List<object> filterData = CommonHelper.SelectProperties(data, selectProperties);
            if (data.Count > 0)
            {
                return CommonHelper.Success((ApiController)(object)this, filterData, "Data fetched successfully", 200, data[0].Pagination);
            }
            return CommonHelper.NoData((ApiController)(object)this);
        }
        [HttpGet]
        [Route("api/get-external-projects")]
        public IHttpActionResult GetExternalProjectList(string searchTerm = null,int? limit = null,int? page = null,string statusFilter = null)
        {
            string[] selectProperties = new string[23]
           {
                "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl",
                "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode",
                "ProjectDepartment", "ContactPerson", "Address"
           };
            var data = _mangerServices.All_Project_List(searchTerm: searchTerm, projectTypeFilter: "External",limit:limit,page:page,statusFilter:statusFilter);
            List<object> filterData = CommonHelper.SelectProperties(data, selectProperties);
            if (data.Count > 0)
            {
                return CommonHelper.Success((ApiController)(object)this, filterData, "Data fetched successfully", 200, data[0].Pagination);
            }
            return CommonHelper.NoData((ApiController)(object)this);
        }
        #endregion
        private IHttpActionResult BadRequest(object value)
        {
            return Content<object>(HttpStatusCode.BadRequest, value);
        }
    }
}