using DocumentFormat.OpenXml.Spreadsheet;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using static RemoteSensingProject.Models.CommonHelper;

namespace RemoteSensingProject.ApiServices
{
    [JwtAuthorize(Roles = "projectManager,divisionHead")]
    public class ProjectManagerController : ApiController
    {
        private readonly AdminServices _adminServices;

        private readonly ManagerService _managerService;

        public ProjectManagerController()
        {
            _adminServices = new AdminServices();
            _managerService = new ManagerService();
        }

        [System.Web.Mvc.AllowAnonymous]
        [HttpGet]
        [Route("api/getProjectExpencesList")]
        public IHttpActionResult GetExpencesList(int projectId, int headId)
        {
            try
            {
                List<ProjectExpenses> data = _managerService.ExpencesList(headId, projectId);
                if (!data.Any())
                {
                    return Ok(new
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
                    data = data,
                    message = "Data found!"
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
        [Route("api/getsubordinate")]
        public IHttpActionResult GetSubordinateList()
        {
            try
            {
                List<RemoteSensingProject.Models.Admin.main.Employee_model> data = (from d in _adminServices.SelectEmployeeRecord()
                                                                                    where d.EmployeeRole.Equals("subOrdinate")
                                                                                    select d).ToList();
                string[] selectprop = new string[2] { "Id", "EmployeeName" };
                List<object> newdata = CommonHelper.SelectProperties(data, selectprop);
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
                bool res = _managerService.insertExpences(formData);
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

        [AllowAnonymous]
        [HttpGet]
        [Route("api/getFinancialReport")]
        public IHttpActionResult getFinancialReport(int projectId)
        {
            try
            {
                List<FinancialMonthlyReport> data = _managerService.GetExtrnlFinancialReport(projectId);
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = (data.Any() ? 200 : 500),
                    message = (data.Any() ? "Data Found !" : "Data not found !"),
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

        [AllowAnonymous]
        [HttpGet]
        [Route("api/getWeeklyUpdate")]
        public IHttpActionResult getWeeklyUpdate(int projectId)
        {
            try
            {
                List<RemoteSensingProject.Models.Admin.main.Project_MonthlyUpdate> data = _managerService.MonthlyProjectUpdate(projectId);
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
                HttpRequest request = HttpContext.Current.Request;
                List<string> validationErrors = new List<string>();
                if (string.IsNullOrWhiteSpace(request.Form.Get("ProjectId")))
                {
                    validationErrors.Add("Project Id is required.");
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("completionPerc")))
                {
                    validationErrors.Add("Project completion in percentage is required.");
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("comments")))
                {
                    validationErrors.Add("description is required.");
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("date")))
                {
                    validationErrors.Add("date is required.");
                }
                RemoteSensingProject.Models.Admin.main.Project_MonthlyUpdate formData = new RemoteSensingProject.Models.Admin.main.Project_MonthlyUpdate
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
                    StatusCode = (res ? 200 : 500),
                    message = (res ? "Weekly Status updated successfuly !" : "Some issue occured !")
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
                HttpRequest request = HttpContext.Current.Request;
                RemoteSensingProject.Models.Admin.main.Project_Statge formData = new RemoteSensingProject.Models.Admin.main.Project_Statge
                {
                    Stage_Id = Convert.ToInt32(request.Form.Get("Stage_Id")),
                    Comment = request.Form.Get("Comment"),
                    CompletionPrecentage = request.Form.Get("CompletionPrecentage"),
                    StageDocument_Url = request.Form.Get("StageDocument_Url"),
                    Status = request.Form.Get("Status")
                };
                HttpPostedFile file = request.Files["StageDocument"];
                if (file != null && file.FileName != "")
                {
                    formData.StageDocument_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    formData.StageDocument_Url = Path.Combine("/ProjectContent/ProjectManager/ProjectDocs/", formData.StageDocument_Url);
                }
                bool res = _managerService.InsertStageStatus(formData);
                if (res && file != null && file.FileName != "")
                {
                    file.SaveAs(HttpContext.Current.Server.MapPath(formData.StageDocument_Url));
                }
                return Ok(new
                {
                    status = res,
                    StatusCode = (res ? 200 : 500),
                    message = (res ? "updateProjectStages updated successfully !" : "Some issue occred !")
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
                List<RemoteSensingProject.Models.Admin.main.Project_Statge> data = _managerService.ViewStagesComments(stageId.ToString());
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = (data.Any() ? 200 : 500),
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
                DashboardCount data = _managerService.DashboardCount(Convert.ToInt32(userId));
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

        [HttpGet]
        [Route("api/managerAssignedProject")]
        public IHttpActionResult AssignedPRoject(int userId, int? page, int? limit, string searchTerm = null, string statusFilter = null)
        {
            try
            {
                List<RemoteSensingProject.Models.Admin.main.Project_model> data = _managerService.All_Project_List(userId, limit, page, "AssignedProject", null, searchTerm, statusFilter);
                string[] selectProperties = new string[20]
                {
                "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl",
                "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode"
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

        [HttpGet]
        [Route("api/getManagerProject")]
        public IHttpActionResult GetProjectList(int userId, int? page = null, int? limit = null, string searchTerm = null, string statusFilter = null)
        {
            try
            {
                List<RemoteSensingProject.Models.Admin.main.Project_model> data = _managerService.All_Project_List(userId, limit, page, "ManagerProject", null, searchTerm, statusFilter);
                string[] selectProperties = new string[20]
                {
                "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl",
                "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode"
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

        [HttpGet]
        [Route("api/getManagerCompleteProject")]
        public IHttpActionResult GetCompleteProject(int userId, int page, int limit)
        {
            try
            {
                List<RemoteSensingProject.Models.Admin.main.Project_model> data = _managerService.All_Project_List(userId, limit, page, "Complete");
                string[] selectProperties = new string[20]
                {
                "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl",
                "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode"
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

        [HttpGet]
        [Route("api/getmanagerdelayProject")]
        public IHttpActionResult getmanagerDelay(int userId, int? limit, int? page)
        {
            try
            {
                List<RemoteSensingProject.Models.Admin.main.Project_model> data = _managerService.All_Project_List(userId, limit, page, "delay");
                string[] selectProperties = new string[20]
                {
                "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl",
                "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode"
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

        [HttpGet]
        [Route("api/getmanagerOngoingProject")]
        public IHttpActionResult onGoingProject(int userId, int? limit, int? page)
        {
            try
            {
                List<RemoteSensingProject.Models.Admin.main.Project_model> data = _managerService.All_Project_List(userId, limit, page, "Ongoing");
                string[] selectProperties = new string[20]
                {
                "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl",
                "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode"
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

        [HttpGet]
        [Route("api/getAllProjectList")]
        public IHttpActionResult getAllProjectList(int userId, int? limit = null, int? page = null, string searchTerm = null, string statusFilter = null)
        {
            try
            {
                List<RemoteSensingProject.Models.Admin.main.Project_model> data = _managerService.All_Project_List(userId, limit, page, null, null, searchTerm, statusFilter);
                string[] selectProperties = new string[20]
                {
                "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl",
                "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode"
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

        [HttpGet]
        [Route("api/getManagerNotice")]
        public IHttpActionResult NoticeList(int managerId, int? page = null, int? limit = null, string searchTerm = null)
        {
            try
            {
                AdminServices adminServices = _adminServices;
                int? managerId2 = managerId;
                List<RemoteSensingProject.Models.Admin.main.Generate_Notice> data = adminServices.getNoticeList(limit, page, null, managerId2, searchTerm);
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

        [JwtAuthorize(Roles = "subOrdinate,projectManager")]
        [HttpGet]
        [Route("api/managerMeetingList")]
        public IHttpActionResult managerMeeting(int managerId, int? page = null, int? limit = null, string searchTerm = null, string statusFilter = null)
        {
            try
            {
                List<RemoteSensingProject.Models.Admin.main.Meeting_Model> data = _managerService.getAllmeeting(managerId, limit, page, searchTerm, statusFilter);
                string[] selectprop = new string[10] { "Id", "CompleteStatus", "MeetingType", "MeetingLink", "MeetingTitle", "memberId", "CreaterId", "MeetingDate", "summary", "Attachment_Url" };
                List<object> newData = CommonHelper.SelectProperties(data, selectprop);
                if (newData.Count > 0)
                {
                    return CommonHelper.Success((ApiController)(object)this, newData, "Data fetched successfully", 200, data[0].Pagination);
                }
                return CommonHelper.NoData((ApiController)(object)this);
            }
            catch (Exception ex)
            {
                return CommonHelper.Error((ApiController)(object)this, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/getConclusionForManagerMeeting")]
        public IHttpActionResult getConclusionForMeeting(int meetingId, int userId)
        {
            List<GetConclusion> res = _managerService.getConclusionForMeeting(meetingId, userId);
            return Ok(new
            {
                status = true,
                message = "data retrieved",
                data = res
            });
        }

        [HttpGet]
        [Route("api/getProjectStatusForDashboard")]
        public IHttpActionResult getProjectstatus(string userId)
        {
            List<RemoteSensingProject.Models.Admin.main.DashboardCount> res = (from e in _adminServices.getAllProjectCompletion()
                                                                               where e.ProjectManager == userId
                                                                               select e).ToList();
            return Ok(new
            {
                status = true,
                message = "data retrieved",
                data = res
            });
        }

        [HttpGet]
        [Route("api/getProblemListByManager")]
        public IHttpActionResult getProblemListByManager(int userId, int? page, int? limit, string searchTerm = null)
        {
            try
            {
                List<RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem> res = _managerService.getAllSubOrdinateProblem(userId.ToString(), page, limit, searchTerm);
                if (res.Count > 0)
                {
                    return CommonHelper.Success((ApiController)(object)this, res, "Data fetched successfully");
                }
                return CommonHelper.NoData((ApiController)(object)this);
            }
            catch (Exception ex)
            {
                return CommonHelper.Error((ApiController)(object)this, ex.Message);
            }
        }

        [HttpPost]
        [Route("api/CreateOutSource")]
        public IHttpActionResult CreateSource()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;
                NameValueCollection form = request.Form;
                List<string> errors = new List<string>();
                string empName = form["EmpName"];
                if (string.IsNullOrWhiteSpace(empName))
                {
                    errors.Add("Employee Name is required.");
                }
                string mobile = form["mobileNo"];
                if (string.IsNullOrWhiteSpace(mobile))
                {
                    errors.Add("Mobile Number is required.");
                }
                else if (!Regex.IsMatch(mobile, "^\\d{10}$"))
                {
                    errors.Add("Mobile Number must be exactly 10 digits.");
                }
                string gender = form["gender"];
                if (string.IsNullOrWhiteSpace(gender))
                {
                    errors.Add("Gender is required.");
                }
                else
                {
                    string[] allowedGenders = new string[3] { "male", "female", "other" };
                    if (!allowedGenders.Contains(gender.Trim().ToLower()))
                    {
                        errors.Add("Gender must be either 'Male', 'Female', or 'Other'.");
                    }
                }
                string email = form["email"];
                if (string.IsNullOrWhiteSpace(email))
                {
                    errors.Add("Email is required.");
                }
                else if (!Regex.IsMatch(email, "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$"))
                {
                    errors.Add("Invalid email format.");
                }
                if (errors.Count > 0)
                {
                    return CommonHelper.Error((ApiController)(object)this, string.Join(", ", errors));
                }
                OuterSource formData = new OuterSource
                {
                    EmpId = Convert.ToInt32(request.Form.Get("EmpId")),
                    EmpName = request.Form.Get("EmpName"),
                    mobileNo = Convert.ToInt64(request.Form.Get("mobileNo")),
                    gender = request.Form.Get("gender"),
                    email = request.Form.Get("email"),
                    designationid = Convert.ToInt32(request.Form.Get("designationId"))
                };
                bool res = _managerService.insertOutSource(formData);
                return Ok(new
                {
                    status = res,
                    StatusCode = (res ? 200 : 500),
                    message = (res ? "Outsource created successfully !" : "Some issue occured")
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
        public IHttpActionResult GetOuterSourceListById(int userId, int? page, int? limit, string searchTerm = null)
        {
            try
            {
                List<OuterSource> data = _managerService.GetAllocatedOutSOurceList(userId, limit, page, searchTerm);
                string[] selectProperties = new string[6] { "Id", "EmpName", "mobileNo", "email", "joiningdate", "gender" };
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

        #region Manage Task
        [HttpPost]
        [Route("api/createTask")]
        public IHttpActionResult createTask()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;
                NameValueCollection form = request.Form;
                List<string> errors = new List<string>();
                if (string.IsNullOrWhiteSpace(form["projectid"]))
                {
                    errors.Add("Project id is required");
                }
                if (string.IsNullOrWhiteSpace(form["title"]))
                {
                    errors.Add("Task title is required.");
                }
                if (string.IsNullOrWhiteSpace(form["description"]))
                {
                    errors.Add("Task description is required.");
                }
                if (string.IsNullOrWhiteSpace(form["outSourceId"]))
                {
                    errors.Add("At least one OutSource is required.");
                }
                if (errors.Count > 0)
                {
                    return CommonHelper.Error((ApiController)(object)this, string.Join(", ", errors));
                }
                OutSourceTask formData = new OutSourceTask
                {
                    projectId = Convert.ToInt32(request.Form.Get("projectId")),
                    title = request.Form.Get("title"),
                    description = request.Form.Get("description"),
                    empId = Convert.ToInt32(request.Form.Get("empId"))
                };
                string outSourceList = request.Form["outSourceId"];
                if (outSourceList != null)
                {
                    formData.outSourceId = (from value in outSourceList.Split(',')
                                            select int.Parse(value.ToString())).ToArray();
                }
                bool res = _managerService.createTask(formData);
                return Ok(new
                {
                    status = res,
                    StatusCode = (res ? 200 : 500),
                    message = (res ? "Task created successfully !" : "Some issue occured !")
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
        public IHttpActionResult getTaskList(int empId, int? page, int? limit, string searchTerm = null)
        {
            try
            {
                List<OutSourceTask> data = _managerService.taskList(empId, limit, page, searchTerm);
                string[] selectProperties = new string[4] { "Id", "title", "description", "completeStatus" };
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

        [System.Web.Mvc.AllowAnonymous]
        [HttpGet]
        [Route("api/ViewTaskId")]
        public IHttpActionResult ViewTaskList(int taskId)
        {
            try
            {
                List<OuterSource> data = _managerService.ViewOutSourceList(taskId);
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = (data.Any() ? 200 : 500),
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
        [Route("api/UpdateTaskStatus")]
        public IHttpActionResult UpdateTAskStatus(int taskId)
        {
            try
            {
                bool res = false;
                string message = "Some issue occured !";
                try
                {
                    res = _managerService.updateTaskStatus(taskId);
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
                return Ok(new
                {
                    status = res,
                    message = (res ? "Task updated successfully !" : message)
                });
            }
            catch (Exception ex2)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex2.Message
                });
            }
        }
        #endregion

        #region Manage Reimbursement
        [HttpPost]
        [Route("api/submitReinbursement")]
        public IHttpActionResult reimbursementSubmit()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;
                NameValueCollection form = request.Form;
                List<string> errors = new List<string>();
                string type = form["type"];
                if (string.IsNullOrWhiteSpace(type))
                {
                    errors.Add("Type is required.");
                }
                string vrNo = form["vrNo"];
                if (string.IsNullOrWhiteSpace(vrNo))
                {
                    errors.Add("Voucher Number is required.");
                }
                string dateStr = form["date"];
                if (string.IsNullOrWhiteSpace(dateStr) || !DateTime.TryParse(dateStr, out var _))
                {
                    errors.Add("Valid Date is required.");
                }
                string particulars = form["particulars"];
                if (string.IsNullOrWhiteSpace(particulars))
                {
                    errors.Add("Particulars are required.");
                }
                string items = form["items"];
                if (string.IsNullOrWhiteSpace(items))
                {
                    errors.Add("Items are required.");
                }
                string purpose = form["purpose"];
                if (string.IsNullOrWhiteSpace(purpose))
                {
                    errors.Add("Purpose is required.");
                }
                string amountStr = form["amount"];
                if (string.IsNullOrWhiteSpace(amountStr) || !decimal.TryParse(amountStr, out var _))
                {
                    errors.Add("Valid Amount is required.");
                }
                if (errors.Count > 0)
                {
                    return CommonHelper.Error((ApiController)(object)this, string.Join(", ", errors));
                }
                Reimbursement formData = new Reimbursement
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
                    StatusCode = (res ? 200 : 500),
                    message = (res ? "Added Successfully!" : "Some error Occured")
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
                HttpRequest request = HttpContext.Current.Request;
                string type = request.Form.Get("type");
                int id = Convert.ToInt32(request.Form.Get("id"));
                int userId = Convert.ToInt32(request.Form.Get("UserId"));
                bool res = _managerService.submitReinbursementForm(type, userId, id);
                return Ok(new
                {
                    status = res,
                    StatusCode = (res ? 200 : 500),
                    message = (res ? "Reinbursement submitted successfully !" : "Some issue found while processing your request !")
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
        public IHttpActionResult getReimbursement(int userId, int? page, int? limit, string typeFilter = null)
        {
            try
            {
                List<Reimbursement> data = _managerService.GetReimbursements(page, limit, null, userId, "getSpecificUserData", typeFilter);
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

        [AllowAnonymous]
        [HttpGet]
        [Route("api/ViewReinbursementBytype")]
        public IHttpActionResult viewReinbursement(int userId, string type, int id, int? page = null, int? limit = null)
        {
            try
            {
                List<Reimbursement> data = _managerService.GetSpecificUserReimbursements(userId, type, id, page, limit);
                string[] selectProperties = new string[11]
                {
                "id", "type", "vrNo", "date", "particulars", "items", "amount", "purpose", "status", "newRequest",
                "adminappr"
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
        #endregion

        private IHttpActionResult BadRequest(object value)
        {
            return Content<object>(HttpStatusCode.BadRequest, value);
        }

        [HttpGet]
        [Route("api/ProjectManagerReimbursementReport")]
        public IHttpActionResult ProjectManagerReimbursementReport(int userId)
        {
            try
            {
                List<Reimbursement> data = _managerService.reinbursementReport(userId);
                return Ok(new
                {
                    status = data.Any(),
                    StatuCode = (data.Any() ? 200 : 500),
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

        #region Manage Raise Problem
        [HttpPost]
        [Route("api/raiseproblem")]
        public IHttpActionResult RaiseProblem()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;
                if (string.IsNullOrWhiteSpace(request.Form.Get("title")))
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 400,
                        message = "Title is required."
                    });
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("projectId")))
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 400,
                        message = "Project ID is required."
                    });
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("description")))
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 400,
                        message = "Description is required."
                    });
                }
                RaiseProblem formdata = new RaiseProblem
                {
                    title = request.Form.Get("title").ToString(),
                    projectId = Convert.ToInt32(request.Form.Get("projectId")),
                    description = request.Form.Get("description").ToString(),
                    id = Convert.ToInt32(request.Form.Get("userId"))
                };
                HttpPostedFile file = request.Files["document"];
                if (file != null && file.ContentLength > 0)
                {
                    formdata.documentname = $"/ProjectContent/ProjectManager/raisedproblem{Guid.NewGuid()}{file.FileName}";
                }
                bool res = _managerService.insertRaisedProblem(formdata);
                if (res && file != null && file.ContentLength > 0)
                {
                    file.SaveAs(HttpContext.Current.Server.MapPath(formdata.documentname));
                }
                return Ok(new
                {
                    status = res,
                    StatusCode = (res ? 200 : 500),
                    message = (res ? "Added successfully!" : "Error Occured")
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
                List<RaiseProblem> data = _managerService.getProblems(userId, limit, page);
                if (data.Count > 0)
                {
                    return CommonHelper.Success((ApiController)(object)this, data, "Data fetched successfully");
                }
                return CommonHelper.NoData((ApiController)(object)this);
            }
            catch (Exception ex)
            {
                return CommonHelper.Error((ApiController)(object)this, ex.Message);
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
                    StatusCode = (res ? 200 : 500),
                    message = (res ? "Deleted Successfully" : "Some issue occured")
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

        #region Manage Attendance
        [HttpPost]
        [Route("api/addAttendancefrompm")]
        public IHttpActionResult AddAttendace(AttendanceManage am)
        {
            try
            {
                (bool, List<string>, string) result = _managerService.InsertAttendance(am);
                if (result.Item1)
                {
                    if (result.Item2.Count > 0)
                    {
                        return Ok(new
                        {
                            success = true,
                            StatusCode = 200,
                            message = "Attendance submitted. Some dates were skipped as they already exist.",
                            skipped = result.Item2
                        });
                    }
                    return Ok(new
                    {
                        success = true,
                        StatusCode = 200,
                        message = "Attendance submitted successfully."
                    });
                }
                return Ok(new
                {
                    success = false,
                    message = "Error occurred: " + result.Item3
                });
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
        [Route("api/ApproveAttendance")]
        public IHttpActionResult ApproveAttendance(int id, bool status, string remark)
        {
            try
            {
                bool res = _managerService.AttendanceApproval(id, status, remark);
                return Ok(new
                {
                    status = res,
                    StatusCode = (res ? 200 : 500),
                    message = ((res && status) ? "Approved Successfully" : (res ? "Rejected  Successfully" : "Something went wrong"))
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
                List<AttendanceManage> data = _managerService.getReportAttendance(month, year, projectManager, EmpId);
                if (data != null)
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        data = data,
                        message = "Data found!"
                    });
                }
                return Ok(new
                {
                    status = data.Any(),
                    data = data,
                    message = "Data not found!"
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

        #region Get Hiring Vehicle & Tour Proposal
        // All hiring vehicle
        [HttpGet]
        [Route("api/getProjectManagerHiringList")]
        public IHttpActionResult getAllHiringList(int userid, int? limit = null, int? page = null, int? projectFilter = null)
        {
            try
            {
                List<HiringVehicle> data = _managerService.GetHiringVehicles(type: "GETBYMANAGER", id: userid, projectFilter: projectFilter, page: page, limit: limit);
                if (data.Count > 0)
                {
                    return Success(this, data, "Data fetched successfully", 200, data[0].Pagination);
                }
                return NoData(this);
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
        [Route("api/getProjectManagerTourproposalList")]
        public IHttpActionResult GetAllTourProposal(int userid, int? limit = null, int? page = null, int? projectFilter = null)
        {
            try
            {
                var data = _managerService.GetTourList(type: "GETBYMANAGER", id: userid, projectFilter: projectFilter, page: page, limit: limit);
                if (data.Count > 0)
                {
                    return Success(this, data, "Data fetched successfully", 200, data[0].Pagination);
                }
                return NoData(this);
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

        [HttpGet]
        [Route("api/getallreimbursementfilter")]
        public IHttpActionResult getallreimbursementforapp(int userId)
        {
            try
            {
                List<Reimbursement> data = _managerService.reinbursementReport(userId);
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
        [Route("api/getMonthlyIntUpdate")]
        public IHttpActionResult getMonthlyIntUpdate(int id)
        {
            try
            {
                List<RemoteSensingProject.Models.Admin.main.Project_MonthlyUpdate> data = _managerService.MonthlyProjectUpdate(id);
                if (data != null)
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        data = data,
                        message = "Data Found!"
                    });
                }
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = 400,
                    message = "Data not Found!"
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
        [Route("api/getMonthlyExtUpdate")]
        public IHttpActionResult getMonthlyExtUpdate(int id)
        {
            try
            {
                List<FinancialMonthlyReport> data = _managerService.GetExtrnlFinancialReport(id);
                if (data != null)
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        data = data,
                        message = "Data Found!"
                    });
                }
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = 400,
                    message = "Data not Found!"
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

        [HttpPost]
        [Route("api/updateIntMonthly")]
        public IHttpActionResult MonthlyUpdateInt()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;
                RemoteSensingProject.Models.Admin.main.Project_MonthlyUpdate formdata = new RemoteSensingProject.Models.Admin.main.Project_MonthlyUpdate
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
                return Ok(new
                {
                    status = res,
                    StatusCode = 500,
                    message = "Some Issue Occured"
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

        [HttpPost]
        [Route("api/updateExtMonthly")]
        public IHttpActionResult MonthlyUpdateExt()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;
                FinancialMonthlyReport formdata = new FinancialMonthlyReport
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
                return Ok(new
                {
                    status = res,
                    StatusCode = 500,
                    message = "Some Issue Occured"
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

        [HttpPost]
        [Route("api/addEmpMonthlyReport")]
        public IHttpActionResult AddEmpMonthlyReport()
        {
            try
            {
                HttpRequest req = HttpContext.Current.Request;
                NameValueCollection form = req.Form;
                List<string> errors = new List<string>();
                if (string.IsNullOrWhiteSpace(form["ProjectId"]) || !int.TryParse(form["ProjectId"], out var _))
                {
                    errors.Add("Valid Project ID is required.");
                }
                string unit = form["Unit"];
                if (string.IsNullOrWhiteSpace(unit))
                {
                    errors.Add("Unit is required.");
                }
                if (string.IsNullOrWhiteSpace(form["AnnualTarget"]) || !int.TryParse(form["AnnualTarget"], out var _))
                {
                    errors.Add("Valid Annual Target is required.");
                }
                if (string.IsNullOrWhiteSpace(form["TargetUptoReviewMonth"]) || !int.TryParse(form["TargetUptoReviewMonth"], out var _))
                {
                    errors.Add("Valid Target Upto Review Month is required.");
                }
                if (string.IsNullOrWhiteSpace(form["AchievementDuringReviewMonth"]) || !int.TryParse(form["AchievementDuringReviewMonth"], out var _))
                {
                    errors.Add("Valid Achievement During Review Month is required.");
                }
                if (string.IsNullOrWhiteSpace(form["CumulativeAchievement"]) || !int.TryParse(form["CumulativeAchievement"], out var _))
                {
                    errors.Add("Valid Cumulative Achievement is required.");
                }
                string benefitingDepartments = form["BenefitingDepartments"];
                if (string.IsNullOrWhiteSpace(benefitingDepartments))
                {
                    errors.Add("Benefiting Departments field is required.");
                }
                string remarks = form["Remarks"];
                if (string.IsNullOrWhiteSpace(remarks))
                {
                    errors.Add("Remarks field is required.");
                }
                if (errors.Count > 0)
                {
                    return CommonHelper.Error((ApiController)(object)this, string.Join(", ", errors));
                }
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
                bool res = _managerService.InsertEmpReport(emp, out message);
                return Ok(new
                {
                    status = res,
                    StatusCode = (res ? 200 : 500),
                    message = (res ? "Added Successfully!" : message)
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
                List<EmpReportModel> data = _managerService.GetEmpReport(userId);
                if (data != null)
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        message = "Data Found!",
                        data = data
                    });
                }
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = 400,
                    message = "Data not Found!"
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

        [HttpPost]
        [Route("api/updateprojectstatus")]
        public IHttpActionResult UpdateProjectStatus(UpdateProjectStatus upd)
        {
            try
            {
                bool res = _managerService.InsertProjectStatus(upd);
                return Ok(new
                {
                    status = res,
                    message = (res ? "Project status updated successfully" : "Some error occured")
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

        [HttpGet]
        [Route("api/getlastprecentage")]
        public IHttpActionResult LastProjectPrecentage(int projectid)
        {
            try
            {
                List<UpdateProjectStatus> data = _managerService.LastProjectStatus(projectid);
                return Ok(new
                {
                    status = (data.Count > 0),
                    message = ((data.Count > 0) ? "data recived" : "data not found"),
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

        [HttpGet]
        [Route("api/deallocate-outsourece")]
        public IHttpActionResult DeAllocateOutsource(int outsourceid)
        {
            try
            {
                bool res = _managerService.DeAllocateManpower(outsourceid);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 400,
                    message = res ? "Deallocate Successfully" : "Something went wrong"
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

        [HttpPost]
        [Route("api/insert-outsource-tour")]
        public IHttpActionResult InsertOutsourceTour(OutsourceAtTour at)
        {
            try
            {
                List<string> error = new List<string>();
                if (at == null)
                {
                    error.Add("Invalid Data");
                }
                else if (at.outsourceid <= 0)
                    error.Add("outsource id is invalid");
                else if (at.fromDate == null)
                    error.Add("from date is invalid");
                else if (at.toDate == null)
                    error.Add("to date is invalid");

                if (error.Count > 0)
                {
                    return CommonHelper.Error((ApiController)(object)this, string.Join(", ", error));
                }
                bool res = _managerService.SendOutSourceAtTour(at);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 400,
                    message = res ? "Submitted Successfully" : "Something went wrong"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 400,
                    message = ex.Message
                });
            }
        }

        #region Manage Division Head
        [JwtAuthorize(Roles = "divisionHead")]
        [Route("api/manpower-request")]
        [HttpGet]
        public IHttpActionResult GetManpowerRequests(int userid, int? designationid = null, string searchTerm = null, int? page = null, int? limit = null)
        {
            try
            {
                int divisionid = _adminServices.SelectEmployeeRecordById(userid).Division;
                var data = _managerService.GetManpowerRequestsInDesignationPmWise(id: divisionid, designationid: designationid, searchTerm: searchTerm);
                return Ok(new
                {
                    status = data.Any(),
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    statuss = false,
                    StatuCode = 400,
                    message = ex.Message
                });
            }
        }
        [JwtAuthorize(Roles = "divisionHead")]
        [Route("api/manpower-request-bydivision")]
        [HttpGet]
        public IHttpActionResult GetManpowerRequestByDivision(int userid, string searchTerm = null)
        {
            try
            {
                int divisionid = _adminServices.SelectEmployeeRecordById(userid).Division;
                var data = _managerService.GetManpowerRequestsInDesignation(id: divisionid, searchTerm: searchTerm);
                return Ok(new
                {
                    status = data.Any(),
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    statuss = false,
                    StatuCode = 400,
                    message = ex.Message
                });
            }
        }
        [JwtAuthorize(Roles = "divisionHead")]
        [Route("api/allocate-manpower")]
        [HttpPost]
        public IHttpActionResult AddManpower([FromBody] AddManPower ap)
        {
            try
            {
                List<string> errors = new List<string>();

                if (ap == null)
                    errors.Add("Request body is empty");


                if (ap.PmId <= 0)
                    errors.Add("Project manager id is not valid");

                if (ap.Outsource == null || !ap.Outsource.Any())
                    errors.Add("At least one outsource is required");

                if (errors.Any())
                {
                    return Content(HttpStatusCode.BadRequest, new
                    {
                        status = false,
                        StatusCode = 400,
                        message = string.Join(", ", errors)
                    });
                }

                // Service throws exception on failure
                bool res = _managerService.AllocateManpower(ap);

                return Ok(new
                {
                    status = res,
                    StatusCode = 200,
                    message = "Manpower allocated successfully!"
                });
            }
            catch (Exception ex)
            {
                // Business / DB validation error
                return Content(HttpStatusCode.Conflict, new
                {
                    status = false,
                    StatusCode = 409,
                    message = ex.Message
                });
            }
        }
        [JwtAuthorize(Roles = "divisionHead")]
        [Route("api/outsource-of-division")]
        [HttpGet]
        public IHttpActionResult GetOutSourceOfDivision(int userid, int designationid)
        {
            try
            {
                int divisionid = _adminServices.SelectEmployeeRecordById(userid).Division;
                var data = _managerService.OutsourceOfDivision(divisionid, designationid);
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = data.Count > 0 ? 200 : 400,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 400,
                    message = ex.Message
                });
            }
        }
        #endregion
    }
}