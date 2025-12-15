using Antlr.Runtime.Tree;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Routing;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Http;
using static RemoteSensingProject.Models.Admin.main;

namespace RemoteSensingProject.ApiServices
{
    [JwtAuthorize(Roles = "projectManager")]
    public class ProjectManagerController : ApiController
    {
        private readonly AdminServices _adminServices;
        private readonly LoginServices _loginService;
        private readonly ManagerService _managerService;
        public ProjectManagerController()
        {
            _adminServices = new AdminServices();
            _loginService = new LoginServices();
            _managerService = new ManagerService();
        }


        #region Project substances
        [System.Web.Mvc.AllowAnonymous]
        [HttpGet]
        [Route("api/getProjectExpencesList")]
        public IHttpActionResult GetExpencesList(int projectId, int headId)
        {
            try
            {
                var data = _managerService.ExpencesList(headId, projectId);
                if (!data.Any())
                {
                    return Ok(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = "Data not found !"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = true,
                        StatusCode = 200,
                        data = data,
                        message = "Data found!"
                    });
                }
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

        [HttpGet]
        [Route("api/getsubordinate")]
        public IHttpActionResult GetSubordinateList()
        {
            try
            {
                var data = _adminServices.SelectEmployeeRecord().Where(d => d.EmployeeRole.Equals("subOrdinate")).ToList();
                var selectprop = new[] { "Id", "EmployeeName" };
                var newdata = CommonHelper.SelectProperties(data, selectprop);
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

        [HttpPost]
        [Route("api/addProjectExpenses")]
        public IHttpActionResult AddExpenses()
        {
            try
            {
                var request = HttpContext.Current.Request;
                List<string> validationErrors = new List<string>();
                if (string.IsNullOrWhiteSpace(request.Form.Get("projectId")))
                    validationErrors.Add("Project Id is required.");


                if (string.IsNullOrWhiteSpace(request.Form.Get("projectHeadId")))
                    validationErrors.Add("Project heads Id is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("title")))
                    validationErrors.Add("Title is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("date")))
                    validationErrors.Add("date is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("amount")))
                    validationErrors.Add("Amount is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("description")))
                    validationErrors.Add("Description is required.");


                var formData = new ProjectExpenses
                {
                    projectId = Convert.ToInt32(request.Form.Get("projectId")),
                    projectHeadId = Convert.ToInt32(request.Form.Get("projectHeadId")),
                    title = request.Form.Get("title"),
                    date = Convert.ToDateTime(request.Form.Get("date")),
                    amount = Convert.ToDecimal(request.Form.Get("amount")),
                    attatchment_url = request.Form.Get("attatchment_url"),
                    description = request.Form.Get("description")
                };

                var file = request.Files["Attatchment_file"];
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
                bool res = _managerService.insertExpences(formData);
                if (res)
                {
                    if (file != null && file.FileName != "")
                    {
                        file.SaveAs(HttpContext.Current.Server.MapPath(formData.attatchment_url));
                    }
                }
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Expenses added successfully !" : "Some issue occured !"
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
        [HttpGet]
        [Route("api/getFinancialReport")]
        public IHttpActionResult getFinancialReport(int projectId)
        {
            try
            {
                var data = _managerService.GetExtrnlFinancialReport(projectId);
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = data.Any() ? 200 : 500,
                    message = data.Any() ? "Data Found !" : "Data not found !",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }
        [System.Web.Mvc.AllowAnonymous]
        [HttpGet]
        [Route("api/getWeeklyUpdate")]
        public IHttpActionResult getWeeklyUpdate(int projectId)
        {
            try
            {
                var data = _managerService.MonthlyProjectUpdate(projectId);
                if (!data.Any())
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 404,
                        message = "Data not found !"
                    });
                }
                return Ok(new
                {
                    status = true,
                    StatusCode = 200,
                    message = "data found !",
                    data = data
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


        [HttpPost]
        [Route("api/insertWeeklyUpdate")]
        public IHttpActionResult insertWeeklyUpdate()
        {
            try
            {
                var request = HttpContext.Current.Request;
                List<string> validationErrors = new List<string>();
                if (string.IsNullOrWhiteSpace(request.Form.Get("ProjectId")))
                    validationErrors.Add("Project Id is required.");


                if (string.IsNullOrWhiteSpace(request.Form.Get("completionPerc")))
                    validationErrors.Add("Project completion in percentage is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("comments")))
                    validationErrors.Add("description is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("date")))
                    validationErrors.Add("date is required.");
                var formData = new Project_MonthlyUpdate
                {
                    ProjectId = Convert.ToInt32(request.Form.Get("ProjectId")),
                    completionPerc = Convert.ToInt32(request.Form.Get("completionPerc")),
                    comments = request.Form.Get("comments"),
                    date = Convert.ToDateTime(request.Form.Get("date"))
                };
                if (validationErrors.Any())
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = string.Join("\n", validationErrors)
                    });
                }
                bool res = _managerService.UpdateMonthlyStatus(formData);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Weekly Status updated successfuly !" : "Some issue occured !"
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


        [HttpPost]
        [Route("api/UpdateProjectStages")]
        public IHttpActionResult updateProjectStages()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var formData = new Project_Statge
                {
                    Stage_Id = Convert.ToInt32(request.Form.Get("Stage_Id")),
                    Comment = request.Form.Get("Comment"),
                    CompletionPrecentage = request.Form.Get("CompletionPrecentage"),
                    StageDocument_Url = request.Form.Get("StageDocument_Url"),
                    Status = request.Form.Get("Status")

                };

                var file = request.Files["StageDocument"];
                if (file != null && file.FileName != "")
                {
                    formData.StageDocument_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    formData.StageDocument_Url = Path.Combine("/ProjectContent/ProjectManager/ProjectDocs/", formData.StageDocument_Url);
                }

                bool res = _managerService.InsertStageStatus(formData);
                if (res)
                {
                    if (file != null && file.FileName != "")
                    {
                        file.SaveAs(HttpContext.Current.Server.MapPath(formData.StageDocument_Url));
                    }
                }
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "updateProjectStages updated successfully !" : "Some issue occred !"
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


        [HttpGet]
        [Route("api/ViewStagesComments")]
        public IHttpActionResult stagesList(int stageId)
        {
            try
            {

                var data = _managerService.ViewStagesComments(stageId.ToString());
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = data.Any() ? 200 : 500,
                    data = data
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

        #region dashboard
        [HttpGet]
        [Route("api/ManagerDashboard")]
        public IHttpActionResult Dashboard(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Ok(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = "Invalid userid !"
                    });
                }

                var data = _managerService.DashboardCount(Convert.ToInt32(userId));
                return Ok(new
                {
                    status = true,
                    message = "Data found !",
                    data = data
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

        #region Assigned PRoject
        [HttpGet]
        [Route("api/managerAssignedProject")]
        public IHttpActionResult AssignedPRoject(int userId, int? page, int? limit,string searchTerm = null,string statusFilter = null)
        {
            try
            {
                var data = _managerService.All_Project_List(userId: userId, limit:limit, page: page, "AssignedProject",searchTerm:searchTerm,statusFilter:statusFilter);
                var selectProperties = new[] { "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl", "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode" };
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
        #endregion

        #region Project
        [HttpGet]
        [Route("api/getManagerProject")]
        public IHttpActionResult GetProjectList(int userId, int? page =null, int? limit = null,string searchTerm = null,string statusFilter = null)
        {
            try
            {
                var data = _managerService.All_Project_List(userId, limit, page, "ManagerProject",searchTerm:searchTerm,statusFilter:statusFilter);
                var selectProperties = new[] { "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl", "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode" };
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


        [HttpGet]
        [Route("api/getManagerCompleteProject")]
        public IHttpActionResult GetCompleteProject(int userId, int page, int limit)
        {
            try
            {
                var data = _managerService.All_Project_List(userId, limit, page, "Complete");
                var selectProperties = new[] { "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl", "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode" };
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

        [HttpGet]
        [Route("api/getmanagerdelayProject")]
        public IHttpActionResult getmanagerDelay(int userId, int? limit, int? page)
        {
            try
            {
                var data = _managerService.All_Project_List(userId, limit, page, "delay");
                var selectProperties = new[] { "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl", "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode" };
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
        [HttpGet]
        [Route("api/getmanagerOngoingProject")]
        public IHttpActionResult onGoingProject(int userId, int? limit, int? page)
        {
            try
            {
                var data = _managerService.All_Project_List(userId, limit, page, "Ongoing");
                var selectProperties = new[] { "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl", "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode" };
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

        [HttpGet]
        [Route("api/getAllProjectList")]
        public IHttpActionResult getAllProjectList(int userId, int? limit = null, int? page = null,string searchTerm = null,string statusFilter = null)
        {
            try
            {
                var data = _managerService.All_Project_List(userId, limit, page, null,searchTerm:searchTerm,statusFilter:statusFilter);
                var selectProperties = new[] { "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl", "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode" };
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
        #endregion

        #region Notice
        [HttpGet]
        [Route("api/getManagerNotice")]
        public IHttpActionResult NoticeList(int managerId, int? page = null, int? limit = null,string searchTerm = null)
        {
            try
            {
                var data = _adminServices.getNoticeList(managerId: managerId, page: page, limit: limit,searchTerm:searchTerm);
                if (!data.Any())
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = "Data not found !"
                    });
                }
                return Ok(new
                {
                    status = true,
                    StatusCode = 200,
                    message = "data found !",
                    data = data
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

        #region Manager Meeting
        [HttpGet]
        [Route("api/managerMeetingList")]
        public IHttpActionResult managerMeeting(int managerId,int?page = null,int? limit = null, string searchTerm = null, string statusFilter = null)
        {
            try
            {
                var data = _managerService.getAllmeeting(managerId,limit,page, searchTerm: searchTerm, statusFilter: statusFilter);
                var selectprop = new[] { "Id", "CompleteStatus", "MeetingType", "MeetingLink", "MeetingTitle", "memberId", "CreaterId", "MeetingDate", "summary", "Attachment_Url" };
                var newData = CommonHelper.SelectProperties(data, selectprop);
                if (newData.Count > 0)
                {
                    return CommonHelper.Success(this, newData, "Data fetched successfully", 200, data[0].Pagination);
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

        [HttpGet]
        [Route("api/getConclusionForManagerMeeting")]
        public IHttpActionResult getConclusionForMeeting(int meetingId, int userId)
        {
            var res = _managerService.getConclusionForMeeting(meetingId, userId);
            return Ok(new { status = true, message = "data retrieved", data = res });
        }
        [HttpGet]
        [Route("api/getAllmeeting")]
        public IHttpActionResult getAllmeeting(int managerId, int? page, int? limit, string searchTerm = null, string statusFilter = null)
        {
            try
            {
                var res = _managerService.getAllmeeting(id: managerId, limit, page, searchTerm: searchTerm, statusFilter: statusFilter);

                var selectprop = new[] { "Id", "CompleteStatus", "MeetingType", "MeetingLink", "MeetingTitle", "AppStatus", "memberId", "CreaterId", "MeetingDate", "createdBy" };
                var data = CommonHelper.SelectProperties(res, selectprop);
                if (data.Count > 0)
                {
                    return CommonHelper.Success(this, data, "Data fetched successfully", 200, res[0].Pagination);
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
        [HttpPost]
        [Route("api/GetResponseFromMember")]
        public IHttpActionResult GetResponseFromMember()
        {
            var httpRequest = HttpContext.Current.Request;
            getMemberResponse mr = new getMemberResponse
            {
                ApprovedStatus = Convert.ToInt32(httpRequest.Form.Get("approveStatus")),
                reason = httpRequest.Form.Get("reason"),
                MeetingId = Convert.ToInt32(httpRequest.Form.Get("meetingId")),
                MemberId = Convert.ToInt32(httpRequest.Form.Get("memberId"))
            };
            var res = _managerService.GetResponseFromMember(mr);
            if (res)
            {
                return Ok(new { status = true, message = "Response Send Successfully", statusCode = 200 });

            }
            else
            {

                return Ok(new { status = true, message = "something went wrong", statusCode = 500 });
            }
        }
        [HttpGet]
        [Route("api/getProjectStatusForDashboard")]
        public IHttpActionResult getProjectstatus(string userId)
        {
            var res = _adminServices.getAllProjectCompletion().Where(e => e.ProjectManager == userId).ToList();

            return Ok(new { status = true, message = "data retrieved", data = res });
        }
        [HttpGet]
        [Route("api/getProblemListByManager")]
        public IHttpActionResult getProblemListByManager(int userId, int? page, int? limit,string searchTerm = null)
        {
            try
            {
                var res = _managerService.getAllSubOrdinateProblem(userId.ToString(), page, limit,searchTerm:searchTerm);
                if (res.Count > 0)
                {
                    return CommonHelper.Success(this, res, "Data fetched successfully", 200);
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


        #endregion


        #region outsource
        [HttpPost]
        [Route("api/CreateOutSource")]
        public IHttpActionResult CreateSource()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var form = request.Form;
                List<string> errors = new List<string>();
                // 🔹 Validate EmpName
                var empName = form["EmpName"];
                if (string.IsNullOrWhiteSpace(empName))
                    errors.Add("Employee Name is required.");

                // 🔹 Validate Mobile Number
                var mobile = form["mobileNo"];
                if (string.IsNullOrWhiteSpace(mobile))
                    errors.Add("Mobile Number is required.");
                else if (!System.Text.RegularExpressions.Regex.IsMatch(mobile, @"^\d{10}$"))
                    errors.Add("Mobile Number must be exactly 10 digits.");

                // 🔹 Validate Gender (only Male, Female, or Other allowed)
                var gender = form["gender"];
                if (string.IsNullOrWhiteSpace(gender))
                    errors.Add("Gender is required.");
                else
                {
                    var allowedGenders = new[] { "male", "female", "other" };
                    if (!allowedGenders.Contains(gender.Trim().ToLower()))
                        errors.Add("Gender must be either 'Male', 'Female', or 'Other'.");
                }

                // 🔹 Validate Email
                var email = form["email"];
                if (string.IsNullOrWhiteSpace(email))
                    errors.Add("Email is required.");
                else if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    errors.Add("Invalid email format.");

                // 🔹 Return validation errors if any
                if (errors.Count > 0)
                    return CommonHelper.Error(this, string.Join(", ", errors), 500);
                var formData = new OuterSource
                {
                    EmpId = Convert.ToInt32(request.Form.Get("EmpId")),
                    EmpName = request.Form.Get("EmpName"),
                    mobileNo = Convert.ToInt64(request.Form.Get("mobileNo")),
                    gender = request.Form.Get("gender"),
                    email = request.Form.Get("email")
                };
                bool res = _managerService.insertOutSource(formData);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Outsource created successfully !" : "Some issue occured"
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

        [HttpGet]
        [Route("api/GetOuterSourceList")]
        public IHttpActionResult GetOuterSourceListById(int userId, int? page, int? limit,string searchTerm = null)
        {
            try
            {
                var data = _managerService.selectAllOutSOurceList(userId, limit, page,searchTerm:searchTerm);
                var selectProperties = new[] { "Id", "EmpName", "mobileNo", "email", "joiningdate", "gender" };
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

        [HttpPost]
        [Route("api/createTask")]
        public IHttpActionResult createTask()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var form = request.Form;
                List<string> errors = new List<string>();

                // 🔹 Required field validations
                if (string.IsNullOrWhiteSpace(form["title"]))
                    errors.Add("Task title is required.");

                if (string.IsNullOrWhiteSpace(form["description"]))
                    errors.Add("Task description is required.");

                if (string.IsNullOrWhiteSpace(form["outSourceId"]))
                    errors.Add("At least one OutSource is required.");

                // 🔹 If any required field is missing
                if (errors.Count > 0)
                    return CommonHelper.Error(this, string.Join(", ", errors), 500);
                var formData = new OutSourceTask
                {
                    title = request.Form.Get("title"),
                    description = request.Form.Get("description"),
                    empId = Convert.ToInt32(request.Form.Get("empId"))
                };
                var outSourceList = request.Form["outSourceId"];
                if (outSourceList != null)
                {
                    formData.outSourceId = outSourceList.Split(',').Select(value => int.Parse(value.ToString())).ToArray();
                }
                bool res = _managerService.createTask(formData);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Task created successfully !" : "Some issue occured !"
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

        [HttpGet]
        [Route("api/getTaskList")]
        public IHttpActionResult getTaskList(int empId, int? page, int? limit,string searchTerm = null)
        {
            try
            {
                var data = _managerService.taskList(empId,searchTerm:searchTerm);
                var selectProperties = new[] { "Id", "title", "description", "completeStatus" };
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
        [System.Web.Mvc.AllowAnonymous]
        [HttpGet]
        [Route("api/ViewTaskId")]
        public IHttpActionResult ViewTaskList(int taskId)
        {
            try
            {
                var data = _managerService.ViewOutSourceList(taskId);
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = data.Any() ? 200 : 500,
                    data = data
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

        [HttpPut]
        [Route("api/UpdateTaskStatus")]
        public IHttpActionResult UpdateTAskStatus(int taskId)
        {
            try
            {
                bool res = _managerService.updateTaskStatus(taskId);
                return Ok(new
                {
                    status = res,
                    message = res ? "Task updated successfully !" : "Some issue occured !"
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


        #region Reimbursement
        [HttpPost]
        [Route("api/submitReinbursement")]
        public IHttpActionResult reimbursementSubmit()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var form = request.Form;
                List<string> errors = new List<string>();

                // 🔹 Validate type
                var type = form["type"];
                if (string.IsNullOrWhiteSpace(type))
                    errors.Add("Type is required.");

                // 🔹 Validate vrNo
                var vrNo = form["vrNo"];
                if (string.IsNullOrWhiteSpace(vrNo))
                    errors.Add("Voucher Number is required.");

                // 🔹 Validate date
                var dateStr = form["date"];
                DateTime date;
                if (string.IsNullOrWhiteSpace(dateStr) || !DateTime.TryParse(dateStr, out date))
                    errors.Add("Valid Date is required.");

                // 🔹 Validate particulars
                var particulars = form["particulars"];
                if (string.IsNullOrWhiteSpace(particulars))
                    errors.Add("Particulars are required.");

                // 🔹 Validate items
                var items = form["items"];
                if (string.IsNullOrWhiteSpace(items))
                    errors.Add("Items are required.");

                // 🔹 Validate purpose
                var purpose = form["purpose"];
                if (string.IsNullOrWhiteSpace(purpose))
                    errors.Add("Purpose is required.");

                // 🔹 Validate amount
                var amountStr = form["amount"];
                decimal amount;
                if (string.IsNullOrWhiteSpace(amountStr) || !decimal.TryParse(amountStr, out amount))
                    errors.Add("Valid Amount is required.");

                // 🔹 Return validation errors if any
                if (errors.Count > 0)
                    return CommonHelper.Error(this, string.Join(", ", errors), 500);
                var formData = new Reimbursement
                {
                    userId = Convert.ToInt32(request.Form.Get("userId")),
                    type = request.Form.Get("type"),
                    vrNo = request.Form.Get("vrNo"),
                    date = Convert.ToDateTime(request.Form.Get("date")),
                    particulars = request.Form.Get("particulars"),
                    items = request.Form.Get("items"),
                    purpose = request.Form.Get("purpose"),
                    amount = Convert.ToDecimal(request.Form.Get("amount"))
                };
                bool res = _managerService.insertReimbursement(formData);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Added Successfully!" : "Some error Occured"
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


        [HttpPost]
        [Route("api/finalSubmitReinbursement")]
        public IHttpActionResult finalSubmitReinbursement()
        {
            try
            {
                var request = HttpContext.Current.Request;
                string type = request.Form.Get("type");
                int id = Convert.ToInt32(request.Form.Get("id"));
                int userId = Convert.ToInt32(request.Form.Get("UserId"));
                bool res = _managerService.submitReinbursementForm(type, userId, id);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Reinbursement submitted successfully !" : "Some issue found while processing your request !"
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

        [HttpGet]
        [Route("api/getReimbursementByUserId")]
        public IHttpActionResult getReimbursement(int userId, int? page, int? limit,string typeFilter = null)
        {
            try
            {
                var data = _managerService.GetReimbursements(page, limit, null, userId, "getSpecificUserData",typeFilter:typeFilter);
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


        [System.Web.Mvc.AllowAnonymous]
        [HttpGet]
        [Route("api/ViewReinbursementBytype")]
        public IHttpActionResult viewReinbursement(int userId, string type, int id, int? page = null, int? limit = null)
        {
            try
            {
                var data = _managerService.GetSpecificUserReimbursements(userId, type, id, page, limit);
                var selectProperties = new[] { "id", "type", "vrNo", "date", "particulars", "items", "amount", "purpose", "status", "newRequest", "adminappr" };
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
        #endregion

        #region tourPurposal
        [HttpPost]
        [Route("api/submitTourProposal")]
        public IHttpActionResult toursubmit()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var form = request.Form;
                List<string> errors = new List<string>();

                // 🔹 Validate projectId
                if (string.IsNullOrWhiteSpace(form["projectId"]) || !int.TryParse(form["projectId"], out int projectId))
                    errors.Add("Valid Project ID is required.");

                // 🔹 Validate dateOfDept
                var dateOfDeptStr = form["dateOfDept"];
                DateTime dateOfDept;
                if (string.IsNullOrWhiteSpace(dateOfDeptStr) || !DateTime.TryParse(dateOfDeptStr, out dateOfDept))
                    errors.Add("Valid Date of Departure is required.");

                // 🔹 Validate place
                var place = form["place"];
                if (string.IsNullOrWhiteSpace(place))
                    errors.Add("Place is required.");

                // 🔹 Validate periodFrom
                var periodFromStr = form["periodFrom"];
                DateTime periodFrom;
                if (string.IsNullOrWhiteSpace(periodFromStr) || !DateTime.TryParse(periodFromStr, out periodFrom))
                    errors.Add("Valid Period From date is required.");

                // 🔹 Validate periodTo
                var periodToStr = form["periodTo"];
                DateTime periodTo;
                if (string.IsNullOrWhiteSpace(periodToStr) || !DateTime.TryParse(periodToStr, out periodTo))
                    errors.Add("Valid Period To date is required.");

                // 🔹 Validate returnDate
                var returnDateStr = form["returnDate"];
                DateTime returnDate;
                if (string.IsNullOrWhiteSpace(returnDateStr) || !DateTime.TryParse(returnDateStr, out returnDate))
                    errors.Add("Valid Return Date is required.");

                // 🔹 Validate purpose
                var purpose = form["purpose"];
                if (string.IsNullOrWhiteSpace(purpose))
                    errors.Add("Purpose is required.");

                // 🔹 Return validation errors if any
                if (errors.Count > 0)
                    return CommonHelper.Error(this, string.Join(", ", errors), 500);

                var formdata = new tourProposal
                {
                    userId = Convert.ToInt32(request.Form.Get("userId")),
                    projectId = Convert.ToInt32(request.Form.Get("projectId")),
                    dateOfDept = Convert.ToDateTime(request.Form.Get("dateOfDept")),
                    place = request.Form.Get("place"),
                    periodFrom = Convert.ToDateTime(request.Form.Get("periodFrom")),
                    periodTo = Convert.ToDateTime(request.Form.Get("periodTo")),
                    returnDate = Convert.ToDateTime(request.Form.Get("returnDate")),
                    purpose = request.Form.Get("purpose")
                };
                bool res = _managerService.insertTour(formdata);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Added successfully!" : "Error Occured"
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
        [HttpGet]
        [Route("api/GetTourForUserId")]
        public IHttpActionResult gettour(int userId, int? page, int? limit, int? projectFilter = null)
        {
            try
            {
                var data = _managerService.getTourList(userId: userId, page: page, limit: limit, type: "specificUser",projectFilter:projectFilter);
                var selectProperties = new[] { "id", "projectName", "dateOfDept", "place", "periodFrom", "periodTo", "returnDate", "purpose", "newRequest", "adminappr", "projectCode" };
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
        #endregion

        #region /*Get Hiring Vehicle*/
        [Route("api/addHiringRequest")]
        [HttpPost]
        public IHttpActionResult addHiringRequest()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var formdata = new HiringVehicle
                {
                    headId = Convert.ToInt32(request.Form.Get("headId")),
                    amount = Convert.ToDecimal(request.Form.Get("amount")),
                    userId = Convert.ToInt32(request.Form.Get("userId")),
                    projectId = Convert.ToInt32(request.Form.Get("projectId")),
                    dateFrom = Convert.ToDateTime(request.Form.Get("dateFrom")),
                    dateTo = Convert.ToDateTime(request.Form.Get("dateTo")),
                    proposedPlace = (string)request.Form.Get("proposedPlace"),
                    purposeOfVisit = (string)request.Form.Get("purposeOfVisit"),
                    totalDaysNight = Convert.ToString(request.Form.Get("totalDaysNight")),
                    totalPlainHills = Convert.ToString(request.Form.Get("totalPlanHills")),
                    taxi = (string)request.Form.Get("taxi"),
                    BookAgainstCentre = (string)request.Form.Get("BookAgainstCentre"),
                    availbilityOfFund = (string)request.Form.Get("availabilityOfFund"),
                    note = (string)request.Form.Get("note")
                };
                bool res = _managerService.insertHiring(formdata);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Added successfully!" : "Error Occured"
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
        [System.Web.Mvc.AllowAnonymous]
        [HttpGet]
        [Route("api/getHiringList")]
        public IHttpActionResult getHiringList(int id)
        {
            try
            {
                var data = _managerService.GetHiringVehicles(id: id, type: "GetById");
                return Ok(new
                {
                    status = data.Any(),
                    data = data
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
        [HttpGet]
        [Route("api/getAllHiringList")]
        public IHttpActionResult getAllHiringList(int userId,int?limit = null, int? page = null, int? projectFilter = null)
        {
            try
            {
                var data = _managerService.GetHiringVehicles(userId: userId, type: "projectManager",limit:limit,page:page,projectFilter:projectFilter);
                return Ok(new
                {
                    status = data.Any(),
                    data = data
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
        private IHttpActionResult BadRequest(object value)
        {
            return Content(HttpStatusCode.BadRequest, value);
        }

        #region Report
        [HttpGet]
        [Route("api/ProjectManagerReimbursementReport")]
        public IHttpActionResult ProjectManagerReimbursementReport(int userId)
        {
            try
            {
                var data = _managerService.reinbursementReport(userId);
                return Ok(new
                {
                    status = data.Any(),
                    StatuCode = data.Any() ? 200 : 500,
                    data = data
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

        [HttpGet]
        [Route("api/ProjectManagerHiringReportProjects")]
        public IHttpActionResult ProjectManagerHiringReportProjects(int userId)
        {
            try
            {
                var data = _managerService.ProjectManagerHiringReportProjects(userId);
                return Ok(new
                {
                    status = data.Any(),
                    StatuCode = data.Any() ? 200 : 500,
                    data = data
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

        [HttpGet]
        [Route("api/ProjectManagerHiringReportbyProjects")]
        public IHttpActionResult ProjectManagerHiringReportbyProjects(int userId, int projectId)
        {
            try
            {
                var data = _managerService.ProjectManagerHiringReportbyProjects(userId, projectId);
                return Ok(new
                {
                    status = data.Any(),
                    StatuCode = data.Any() ? 200 : 500,
                    data = data
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

        [HttpGet]
        [Route("api/ProjectManagerTourProposalReport")]
        public IHttpActionResult ProjectManagerTourProposalReport(int userId)
        {
            try
            {
                var data = _managerService.ProjectManagertourreportProjects(userId);
                return Ok(new
                {
                    status = data.Any(),
                    StatuCode = data.Any() ? 200 : 500,
                    data = data
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
        [HttpGet]
        [Route("api/ProjectManagertourreportByProjects")]
        public IHttpActionResult ProjectManagertourreportByProjects(int userId, int projectId)
        {
            try
            {
                var data = _managerService.ProjectManagertourreportByProjects(userId, projectId);
                return Ok(new
                {
                    status = data.Any(),
                    StatuCode = data.Any() ? 200 : 500,
                    data = data
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

        #region Raise Problem
        [HttpPost]
        [Route("api/raiseproblem")]
        public IHttpActionResult RaiseProblem()
        {
            try
            {
                var request = HttpContext.Current.Request;
                // === Required Field Validation ===
                if (string.IsNullOrWhiteSpace(request.Form.Get("title")))
                    return BadRequest(new { status = false, StatusCode = 400, message = "Title is required." });

                if (string.IsNullOrWhiteSpace(request.Form.Get("projectId")))
                    return BadRequest(new { status = false, StatusCode = 400, message = "Project ID is required." });

                if (string.IsNullOrWhiteSpace(request.Form.Get("description")))
                    return BadRequest(new { status = false, StatusCode = 400, message = "Description is required." });

                var formdata = new RaiseProblem
                {
                    title = request.Form.Get("title").ToString(),
                    projectId = Convert.ToInt32(request.Form.Get("projectId")),
                    description = request.Form.Get("description").ToString(),
                    id = Convert.ToInt32(request.Form.Get("userId"))
                };
                var file = request.Files["document"];
                if (file != null)
                    if (file.ContentLength > 0)
                    {
                        formdata.documentname = $"/ProjectContent/ProjectManager/raisedproblem{Guid.NewGuid()}{file.FileName}";
                    }
                bool res = _managerService.insertRaisedProblem(formdata);
                if (res)
                {
                    if (file != null)
                        if (file.ContentLength > 0)
                            file.SaveAs(HttpContext.Current.Server.MapPath(formdata.documentname));
                }
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Added successfully!" : "Error Occured"
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
        [HttpGet]
        [Route("api/getraisedproblem")]
        public IHttpActionResult getRaisedProblem(int userId, int? page, int? limit)
        {
            try
            {
                var data = _managerService.getProblems(userId, limit, page);
                if (data.Count > 0)
                {
                    return CommonHelper.Success(this, data, "Data fetched successfully", 200);
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
        [HttpGet]
        [Route("api/deleteraisedproblem")]
        public IHttpActionResult deleteraisedproblem(int id, int userId)
        {
            try
            {
                bool res = _managerService.deleteRaisedProblem(id, userId);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Deleted Successfully" : "Some issue occured"
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
        #region attendance
        [HttpPost]
        [Route("api/addAttendancefrompm")]
        public IHttpActionResult AddAttendace(AttendanceManage am)
        {
            try
            {
                var result = _managerService.InsertAttendance(am);
                if (result.success)
                {
                    if (result.skippedDates.Count > 0)
                    {
                        return Ok(new
                        {
                            success = true,
                            StatusCode = 200,
                            message = "Attendance submitted. Some dates were skipped as they already exist.",
                            skipped = result.skippedDates
                        });
                    }

                    return Ok(new { success = true, StatusCode = 200, message = "Attendance submitted successfully." });
                }
                else
                {
                    return Ok(new { success = false, message = "Error occurred: " + result.error });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 404,
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("api/getattendancebyIdofEmp")]
        public IHttpActionResult GetAttendanceByIdOfEmp(int projectManager, int EmpId)
        {
            try
            {
                var data = _managerService.GetAllAttendanceForProjectManager(projectManager, EmpId);
                if (data != null)
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        data = data,
                        message = "Data found!"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        data = data,
                        message = "Data not found!"
                    });
                }
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
        [HttpGet]
        [Route("api/ApproveAttendance")]
        public IHttpActionResult ApproveAttendance(int id, bool status, string remark)
        {
            try
            {
                bool res = _managerService.AttendanceApproval(id, status, remark);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = (res == true && status == true) ? "Approved Successfully" : res ? "Rejected  Successfully" : "Something went wrong"
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

        [HttpGet]
        [Route("api/getrepoattendance")]
        public IHttpActionResult getrepoattendance(int month, int year, int projectManager, int EmpId)
        {
            try
            {
                var data = _managerService.getReportAttendance(month, year, projectManager, EmpId);
                if (data != null)
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        data = data,
                        message = "Data found!"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        data = data,
                        message = "Data not found!"
                    });
                }
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
        #region Get All Report for App
        [HttpGet]
        [Route("api/getallreimbursementfilter")]
        public IHttpActionResult getallreimbursementforapp(int userId)
        {
            try
            {
                var data = _managerService.reinbursementReport(userId);
                return Ok(new
                {
                    status = data.Any(),
                    data = data
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
        [HttpGet]
        [Route("api/getalltourproposalfilter")]
        public IHttpActionResult getalltourproposalforapp(int userId)
        {
            try
            {
                var data = _managerService.getTourList(userId);
                return Ok(new
                {
                    status = data.Any(),
                    data = data
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
        [HttpGet]
        [Route("api/getallhiringfilter")]
        public IHttpActionResult getallhiringforapp(int userId)
        {
            try
            {
                var data = _managerService.GetHiringVehicles(userId: userId, type: "projectManager");
                return Ok(new
                {
                    status = data.Any(),
                    data = data
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

        #region Project Update 
        [HttpGet]
        [Route("api/getMonthlyIntUpdate")]
        public IHttpActionResult getMonthlyIntUpdate(int id)
        {
            try
            {
                var data = _managerService.MonthlyProjectUpdate(id);
                if (data != null)
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        data = data,
                        message = "Data Found!"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        StatusCode = 400,
                        message = "Data not Found!"
                    });
                }
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
        [HttpGet]
        [Route("api/getMonthlyExtUpdate")]
        public IHttpActionResult getMonthlyExtUpdate(int id)
        {
            try
            {
                var data = _managerService.GetExtrnlFinancialReport(id);
                if (data != null)
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        data = data,
                        message = "Data Found!"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        StatusCode = 400,
                        message = "Data not Found!"
                    });
                }
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
        [HttpPost]
        [Route("api/updateIntMonthly")]
        public IHttpActionResult MonthlyUpdateInt()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var formdata = new Project_MonthlyUpdate
                {
                    ProjectId = Convert.ToInt32(request.Form.Get("ProjectId")),
                    date = Convert.ToDateTime(request.Form.Get("date")),
                    comments = request.Form.Get("comments").ToString(),
                    unit = request.Form.Get("unit").ToString(),
                    annual = request.Form.Get("annual").ToString(),
                    reviewMonth = request.Form.Get("reviewMonth").ToString(),
                    MonthEndSequentially = request.Form.Get("MonthEndSequentially").ToString(),
                    StateBeneficiaries = request.Form.Get("StateBeneficiaries").ToString()
                };
                bool res = _managerService.UpdateMonthlyStatus(formdata);
                if (res)
                {
                    return Ok(new
                    {
                        status = res,
                        StatusCode = 200,
                        message = "Update Successfully"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = res,
                        StatusCode = 500,
                        message = "Some Issue Occured"
                    });
                }
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
        [HttpPost]
        [Route("api/updateExtMonthly")]
        public IHttpActionResult MonthlyUpdateExt()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var formdata = new FinancialMonthlyReport
                {
                    projectId = Convert.ToInt32(request.Form.Get("projectId")),
                    aim = request.Form.Get("aim").ToString(),
                    date = request.Form.Get("date").ToString(),
                    month_aim = request.Form.Get("month_aim").ToString(),
                    completeInMonth = request.Form.Get("completeInMonth").ToString(),
                    departBeneficiaries = request.Form.Get("departBeneficiaries").ToString()
                };
                bool res = _managerService.updateFinancialReportMonthly(formdata);
                if (res)
                {
                    return Ok(new
                    {
                        status = res,
                        StatusCode = 200,
                        message = "Update Successfully"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = res,
                        StatusCode = 500,
                        message = "Some Issue Occured"
                    });
                }
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

        #region Progress Report
        [HttpPost]
        [Route("api/addEmpMonthlyReport")]
        public IHttpActionResult AddEmpMonthlyReport()
        {
            try
            {
                var req = HttpContext.Current.Request;
                var form = req.Form;
                List<string> errors = new List<string>();

                // 🔹 Validate ProjectId
                if (string.IsNullOrWhiteSpace(form["ProjectId"]) || !int.TryParse(form["ProjectId"], out int projectId))
                    errors.Add("Valid Project ID is required.");

                // 🔹 Validate Unit
                var unit = form["Unit"];
                if (string.IsNullOrWhiteSpace(unit))
                    errors.Add("Unit is required.");

                // 🔹 Validate AnnualTarget
                if (string.IsNullOrWhiteSpace(form["AnnualTarget"]) || !int.TryParse(form["AnnualTarget"], out int annualTarget))
                    errors.Add("Valid Annual Target is required.");

                // 🔹 Validate TargetUptoReviewMonth
                if (string.IsNullOrWhiteSpace(form["TargetUptoReviewMonth"]) || !int.TryParse(form["TargetUptoReviewMonth"], out int targetUptoReviewMonth))
                    errors.Add("Valid Target Upto Review Month is required.");

                // 🔹 Validate AchievementDuringReviewMonth
                if (string.IsNullOrWhiteSpace(form["AchievementDuringReviewMonth"]) || !int.TryParse(form["AchievementDuringReviewMonth"], out int achievementDuringReviewMonth))
                    errors.Add("Valid Achievement During Review Month is required.");

                // 🔹 Validate CumulativeAchievement
                if (string.IsNullOrWhiteSpace(form["CumulativeAchievement"]) || !int.TryParse(form["CumulativeAchievement"], out int cumulativeAchievement))
                    errors.Add("Valid Cumulative Achievement is required.");

                // 🔹 Validate BenefitingDepartments
                var benefitingDepartments = form["BenefitingDepartments"];
                if (string.IsNullOrWhiteSpace(benefitingDepartments))
                    errors.Add("Benefiting Departments field is required.");

                // 🔹 Validate Remarks
                var remarks = form["Remarks"];
                if (string.IsNullOrWhiteSpace(remarks))
                    errors.Add("Remarks field is required.");

                // 🔹 If validation fails
                if (errors.Count > 0)
                    return CommonHelper.Error(this, string.Join(", ", errors), 500);
                EmpReportModel emp = new EmpReportModel
                {
                    PmId = Convert.ToInt32(req.Form.Get("PmId")),
                    ProjectId = Convert.ToInt32(req.Form.Get("ProjectId")),
                    Unit = req.Form.Get("Unit").ToString(),
                    AnnualTarget = Convert.ToInt32(req.Form.Get("AnnualTarget")),
                    TargetUptoReviewMonth = Convert.ToInt32(req.Form.Get("TargetUptoReviewMonth")),
                    AchievementDuringReviewMonth = Convert.ToInt32(req.Form.Get("AchievementDuringReviewMonth")),
                    CumulativeAchievement = Convert.ToInt32(req.Form.Get("CumulativeAchievement")),
                    BenefitingDepartments = req.Form.Get("BenefitingDepartments").ToString(),
                    Remarks = req.Form.Get("Remarks").ToString()
                };
                string message;
                bool res = _managerService.InsertEmpReport(emp,out message);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Added Successfully!" : message
                });
            }
            catch
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = "Some error Occured"
                });
            }
        }

        [HttpGet]
        [Route("api/getEmpMonthlyReport")]
        public IHttpActionResult getEmpMonthlyReport(int userId)
        {
            try
            {
                var data = _managerService.GetEmpReport(userId);
                if (data != null)
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        message = "Data Found!",
                        data = data
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        StatusCode = 400,
                        message = "Data not Found!"
                    });
                }
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
    }
}
