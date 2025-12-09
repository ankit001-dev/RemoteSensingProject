using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Grpc.Core;
using Newtonsoft.Json;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Web;
using System.Web.Http;
using static RemoteSensingProject.Models.Admin.main;
using static RemoteSensingProject.Models.ApiCommon;

namespace RemoteSensingProject.ApiServices
{
    [JwtAuthorize]
    public class AdminController : ApiController
    {
        private readonly AdminServices _adminServices;
        private readonly ManagerService _managerservice;
        private readonly LoginServices _loginService;
        private readonly AccountService _accountService;
        public AdminController()
        {
            _adminServices = new AdminServices();
            _loginService = new LoginServices();
            _managerservice = new ManagerService();
            _accountService = new AccountService();
        }

        #region ExpenditureAmt
        [HttpGet]
        [Route("api/ViewExpenditureAmtData")]
        public IHttpActionResult ViewExpendedAmt(int?limit=null,int?page=null)
        {
            try
            {
                var data = _adminServices.ViewProjectExpenditure(limit,page);
                if (data.Count > 0)
                {
                    return CommonHelper.Success(this, data, "Data fetched successfully", 200, data[0].Pagination);
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

        #region Employee Registration
        [HttpPost]
        [Route("api/EmployeeRegistration")]
        public IHttpActionResult Emp_Register()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var empData = new Employee_model
                {
                    Id = Convert.ToInt32(request.Form.Get("Id")),
                    EmployeeCode = request.Form.Get("EmployeeCode"),
                    EmployeeName = request.Form.Get("EmployeeName"),
                    MobileNo = Convert.ToInt64(request.Form.Get("MobileNo")),
                    Email = request.Form.Get("Email"),
                    EmployeeRole = request.Form.Get("EmployeeRole"),
                    Division = Convert.ToInt32(request.Form.Get("Division")),
                    Designation = Convert.ToInt32(request.Form.Get("Designation")),
                    Gender = request.Form.Get("Gender"),
                    Image_url = request.Form.Get("Image_url")
                };
                var file = request.Files["EmployeeImages"];
                if (file != null && file.FileName != "")
                {
                    empData.Image_url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    empData.Image_url = Path.Combine("/ProjectContent/Admin/Employee_Images/", empData.Image_url);
                }
                else if (string.IsNullOrEmpty(empData.Image_url))
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 404,
                        message = "Employee image is not found. Try with employee image profile."
                    });
                }

                List<string> validationErrors = new List<string>();

                if (string.IsNullOrWhiteSpace(empData.EmployeeCode))
                    validationErrors.Add("Employee Code is required.");

                if (string.IsNullOrWhiteSpace(empData.EmployeeName))
                    validationErrors.Add("Employee Name is required.");

                if (empData.MobileNo == 0 || empData.MobileNo.ToString().Length != 10)
                    validationErrors.Add("A valid 10-digit Mobile Number is required.");

                if (string.IsNullOrWhiteSpace(empData.Email) || !empData.Email.Contains("@"))
                    validationErrors.Add("A valid Email address is required.");

                if (string.IsNullOrWhiteSpace(empData.EmployeeRole))
                    validationErrors.Add("Employee Role is required.");

                if (empData.Division <= 0)
                    validationErrors.Add("Division must be selected.");

                if (empData.Designation <= 0)
                    validationErrors.Add("Designation must be selected.");

                if (string.IsNullOrWhiteSpace(empData.Gender) ||
                    !(empData.Gender.Equals("Male", StringComparison.OrdinalIgnoreCase) ||
                      empData.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase) ||
                      empData.Gender.Equals("Other", StringComparison.OrdinalIgnoreCase)))
                    validationErrors.Add("Gender must be Male, Female, or Other.");



                if (validationErrors.Any())
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = string.Join("\n", validationErrors)
                    });

                }
                else
                {
                    bool res = _adminServices.AddEmployees(empData);
                    if (res)
                    {
                        if (file != null && file.FileName != "")
                        {
                            file.SaveAs(HttpContext.Current.Server.MapPath(empData.Image_url));
                        }
                    }
                    return Ok(new
                    {
                        status = res,
                        StatusCode = res ? 200 : 500,
                        message = res ? "Employee registration completed successfully !" : "Some issue occured while processing with your request. Please try after sometime."
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message,
                    data = ex
                });
            }
        }


        [HttpGet]
        [Route("api/allEmployeeList")]
        public IHttpActionResult All_EmpList(int? page=null, int? limit = null, string searchTerm = null,int?devision = null)
        {
            try
            {
                var data = _adminServices.SelectEmployeeRecord(page, limit,searchTerm,devision);
                var selectProperties = new[] { "Id", "EmployeeCode", "EmployeeName", "DevisionName", "Email", "MobileNo", "EmployeeRole", "Division", "DesignationName", "Status", "ActiveStatus", "CreationDate", "Image_url" };
                var filtered = CommonHelper.SelectProperties(data, selectProperties);

                if (data != null && data.Count > 0)
                {
                    var pagination = data[0].Pagination;
                    return CommonHelper.Success(this, filtered, "Data fetched successfully.", 200, pagination);
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

        [HttpPut]
        [Route("api/updateEmployeeData")]
        public IHttpActionResult Update_Employee()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var empData = new Employee_model
                {
                    Id = Convert.ToInt32(request.Form.Get("Id")),
                    EmployeeCode = request.Form.Get("EmployeeCode"),
                    EmployeeName = request.Form.Get("EmployeeName"),
                    MobileNo = Convert.ToInt64(request.Form.Get("MobileNo")),
                    Email = request.Form.Get("Email"),
                    EmployeeRole = request.Form.Get("EmployeeRole"),
                    Division = Convert.ToInt32(request.Form.Get("Division")),
                    Designation = Convert.ToInt32(request.Form.Get("Designation")),
                    Gender = request.Form.Get("Gender"),
                    Image_url = request.Form.Get("Image_url")
                };
                var file = request.Files["EmployeeImages"];
                if (file != null && file.FileName != "")
                {
                    empData.Image_url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    empData.Image_url = "/ProjectContent/Admin/Employee_Images/" + empData.Image_url;
                }
                else if (string.IsNullOrEmpty(empData.Image_url))
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 404,
                        message = "Employee image is not found. Try with employee image profile."
                    });
                }

                List<string> validationErrors = new List<string>();

                if (empData.Id <= 0)
                    validationErrors.Add("Invalid request.");

                if (string.IsNullOrWhiteSpace(empData.EmployeeCode))
                    validationErrors.Add("Employee Code is required.");

                if (string.IsNullOrWhiteSpace(empData.EmployeeName))
                    validationErrors.Add("Employee Name is required.");

                if (empData.MobileNo == 0 || empData.MobileNo.ToString().Length != 10)
                    validationErrors.Add("A valid 10-digit Mobile Number is required.");

                if (string.IsNullOrWhiteSpace(empData.Email) || !empData.Email.Contains("@"))
                    validationErrors.Add("A valid Email address is required.");

                if (string.IsNullOrWhiteSpace(empData.EmployeeRole))
                    validationErrors.Add("Employee Role is required.");

                if (empData.Division <= 0)
                    validationErrors.Add("Division must be selected.");

                if (empData.Designation <= 0)
                    validationErrors.Add("Designation must be selected.");

                if (string.IsNullOrWhiteSpace(empData.Gender) ||
                    !(empData.Gender.Equals("Male", StringComparison.OrdinalIgnoreCase) ||
                      empData.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase) ||
                      empData.Gender.Equals("Other", StringComparison.OrdinalIgnoreCase)))
                    validationErrors.Add("Gender must be Male, Female, or Other.");

                if (string.IsNullOrWhiteSpace(empData.Image_url))
                    validationErrors.Add("Employee Image not found !");

                if (validationErrors.Any())
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = string.Join("\n", validationErrors)
                    });

                }
                else
                {
                    bool res = _adminServices.AddEmployees(empData);
                    if (res)
                    {
                        if (file != null && file.FileName != "")
                        {
                            file.SaveAs(HttpContext.Current.Server.MapPath(empData.Image_url));
                        }
                    }
                    return Ok(new
                    {
                        status = res,
                        StatusCode = res ? 200 : 500,
                        message = res ? "Employee profile updation completed successfully !" : "Some issue occured while processing with your request. Please try after sometime."
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message,
                    data = ex
                });
            }
        }

        [HttpGet]
        [Route("api/getEmployeeById")]
        public IHttpActionResult Get_EmployeeById(int Id)
        {
            try
            {
                var data = _adminServices.SelectEmployeeRecordById(Id);
                if (data != null && data.Id > 0)
                {
                    return Ok(new
                    {
                        status = true,
                        StatusCode = 200,
                        message = "Data found !",
                        data = data
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 404,
                        message = "Data not found "
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message,
                    data = ex
                });
            }
        }

        [HttpDelete]
        [Route("api/removeEmployee")]
        public IHttpActionResult RemoveEmployee(int Id)
        {
            try
            {
                if (Id <= 0)
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = "Invalid request id !"
                    });
                }
                var data = _adminServices.SelectEmployeeRecordById(Id);
                if (data.Id <= 0)
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = "Invalid request id !"
                    });
                }
                bool res = _adminServices.RemoveEmployees(Id);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Selected employee removed successfully !" : "Some issue occred while processing your request."
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message,
                    data = ex
                });
            }
        }
        #endregion

        #region Admin Dashboard
        [HttpGet]
        [Route("api/adminDashboard")]
        public IHttpActionResult adminDashboard()
        {
            try
            {
                var data = _adminServices.DashboardCount();
                if (data != null)
                {
                    return Ok(new
                    {
                        status = true,
                        StatusCode = 200,
                        message = "Data Found !",
                        data = data
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = false,
                        StatusCode = 404,
                        message = "Admin dashboard not found !"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message,
                    data = ex
                });
            }
        }

        [HttpGet]
        [Route("api/adminProjectGraph")]
        public IHttpActionResult adminProjectGraph()
        {
            try
            {
                var data = _adminServices.getAllProjectCompletion();
                return Ok(new
                {
                    status = true,
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
        [Route("api/adminDelayProject")]
        public IHttpActionResult DelayProject(int? page, int? limit)
        {
            try
            {
                var data = _managerservice.All_Project_List(0, limit, page, "delay", null);
                var selectProperties = new[] { "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl", "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode", "ProjectDepartment", "ContactPerson", "Address" };
                var newData = CommonHelper.SelectProperties(data, selectProperties);

                if (data.Count > 0)
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
        [Route("api/adminOngoingProject")]
        public IHttpActionResult ongoingProject(int? page, int? limit)
        {
            try
            {
                var data = _managerservice.All_Project_List(0, limit, page, "Ongoing", null);
                var selectProperties = new[] { "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl", "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode", "ProjectDepartment", "ContactPerson", "Address" };
                var newData = CommonHelper.SelectProperties(data, selectProperties);
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
        [Route("api/adminCompleteProject")]
        public IHttpActionResult completeProject(int? page, int? limit)
        {
            try
            {
                var data = _managerservice.All_Project_List(0, limit, page, "Complete", null);
                var selectProperties = new[] { "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl", "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode", "ProjectDepartment", "ContactPerson", "Address" };
                var newData = CommonHelper.SelectProperties(data, selectProperties);
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


        #endregion

        #region Create Project

        [HttpGet]
        [Route("api/ViewProjectDetailById")]
        public IHttpActionResult ViewProjectDetailById(int projectId)
        {
            try
            {
                return Ok();
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
        [Route("api/adminProjectList")]
        public IHttpActionResult getProjectList(int? page, int? limit, string searchTerm = null, string statusFilter = null, int? projectManagerFilter = null)
        {
            try
            {
                var selectProperties = new[] { "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl", "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode", "ProjectDepartment", "ContactPerson", "Address" };
                var data = _managerservice.All_Project_List(userId:projectManagerFilter,page:page, limit:limit, filterType:null,searchTerm:searchTerm,statusFilter:statusFilter);
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
        [Route("api/adminAddBudgets")]
        public IHttpActionResult AddBudgets()
        {
            try
            {
                var request = HttpContext.Current.Request;
                // Validations
                List<string> validationErrors = new List<string>();
                if (string.IsNullOrWhiteSpace(request.Form.Get("Project_Id")))
                    validationErrors.Add("Project ID is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("ProjectHeads")))
                    validationErrors.Add("Project Heads is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("ProjectAmount")))
                    validationErrors.Add("Project Amount is required.");


                if (!int.TryParse(request.Form.Get("Project_Id"), out int projectId))
                    validationErrors.Add("Invalid Project ID format.");

                if (!decimal.TryParse(request.Form.Get("ProjectAmount"), out decimal projectAmount))
                    validationErrors.Add("Invalid Project Amount format.");



                var formData = new Project_Budget
                {
                    Id = Convert.ToInt32(request.Form.Get("Id")),
                    Project_Id = Convert.ToInt32(request.Form.Get("Project_Id")),
                    ProjectHeads = request.Form.Get("ProjectHeads"),
                    ProjectAmount = Convert.ToDecimal(request.Form.Get("ProjectAmount"))
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

                decimal ProjectBudget = _adminServices.GetProjectById(formData.Project_Id).pm.ProjectBudget;
                decimal totalBudgets = _adminServices.ProjectBudgetList(formData.Project_Id).Sum(x => x.ProjectAmount);
                if ((totalBudgets + formData.ProjectAmount) <= ProjectBudget)
                {
                    bool res = _adminServices.insertProjectBudgets(formData);
                    return Ok(new
                    {
                        status = res,
                        StatusCode = res ? 200 : 500,
                        message = res ? "Project budget added successfully !" : "Some issue occured !"
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 404,
                        message = "Maximum amount reached !"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message,
                    data = ex
                });
            }
        }

        [HttpPost]
        [Route("api/adminAddStages")]
        public IHttpActionResult AddStages()
        {
            try
            {
                var request = HttpContext.Current.Request;
                List<string> validationErrors = new List<string>();
                if (string.IsNullOrWhiteSpace(request.Form.Get("Project_Id")))
                    validationErrors.Add("Project ID is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("KeyPoint")))
                    validationErrors.Add("Stages keys is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("CompletionDate")))
                    validationErrors.Add("Completion date  is required.");
                if (validationErrors.Any())
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = string.Join("\n", validationErrors)
                    });
                }
                var formData = new Project_Statge
                {
                    Id = Convert.ToInt32(request.Form.Get("Id")),
                    Project_Id = Convert.ToInt32(request.Form.Get("Project_Id")),
                    KeyPoint = request.Form.Get("KeyPoint"),
                    Document_Url = request.Form.Get("Document_Url"),
                    CompletionDate = Convert.ToDateTime(request.Form.Get("CompletionDate"))
                };

                var file = request.Files["Stage_Document"];
                if (file != null && file.FileName != "")
                {
                    formData.Document_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    formData.Document_Url = Path.Combine("/ProjectContent/Admin/ProjectDocs/", formData.Document_Url);
                }

                bool res = _adminServices.insertProjectStages(formData);
                if (res && file != null && file.FileName != "")
                {
                    file.SaveAs(HttpContext.Current.Server.MapPath(formData.Document_Url));
                }
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Project stages added successfully !" : "Some issue occured while processing request ! Please try after sometome."
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

        //[HttpGet]
        //[Route("api/GetadminProjectDetailById")]
        //public IHttpActionResult GetProjectById(int Id)
        //{
        //    try
        //    {
        //        var data = _adminServices.GetProjectById(Id);
        //        return Ok(new
        //        {
        //            status = true,
        //            data = data
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            status = false,
        //            StatusCode = 500,
        //            message = ex.Message
        //        });
        //    }
        //}

        [System.Web.Mvc.AllowAnonymous]
        [HttpGet]
        [Route("api/GetProjectBudgets")]
        public IHttpActionResult GetProjetBudgets(int projectId)
        {
            try
            {
                var data = _adminServices.ProjectBudgetList(projectId);
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
        [Route("api/getProjectStages")]
        public IHttpActionResult GetProjectSatges(int projectId)
        {
            try
            {
                var data = _adminServices.ProjectStagesList(projectId);
                var selectedprop = new[] { "Id", "Project_Id", "KeyPoint", "CompletionDate", "CompletionDatestring", "Status", "Document_Url", "completionStatus" };
                var newdata = CommonHelper.SelectProperties(data, selectedprop);
                if (!newdata.Any())
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
                    message = "Data found !",
                    data = newdata
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

        #region minute of meeting
        [HttpGet]
        [Route("api/adminMeetingList")]
        public IHttpActionResult MeetingList(int? limit = null, int? page = null, string searchTerm = null, string statusFilter = null, string meetingMode = null)
        {
            try
            {
                var data = _adminServices.getAllmeeting(limit, page,searchTerm:searchTerm,statusFilter:statusFilter,meetingMode:meetingMode);
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
        [Route("api/GetMeetingMemberListById")]
        public IHttpActionResult GetMeetingMemberList(int meetId)
        {
            try
            {
                var data = _adminServices.GetMeetingMemberList(meetId);
                var selectProperties = new[] { "Id", "EmployeeCode", "EmployeeName", "EmployeeRole", "MobileNo", "Email", "meetingId" };
                var newData = CommonHelper.SelectProperties(data, selectProperties);
                if (newData.Count > 0)
                {
                    return CommonHelper.Success(this, newData, "Data fetched successfully", 200);
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
        [Route("api/getMeetingKeyResponse")]
        public IHttpActionResult GetMeetingKeyResponse(int id)
        {
            try
            {
                var data = _adminServices.getKeypointResponse(id);
                return Ok(new
                {
                    status = true,
                    StatusCode = 200,
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
        [Route("api/getMeetingPresentMember")]
        public IHttpActionResult GetMeetingPresentMember(int MeetId)
        {
            try
            {
                var data = _adminServices.getPresentMember(MeetId);
                var selectedprop = new[] { "EmployeeName", "Image_url", "EmployeeRole", "PresentStatus" };
                var newData = CommonHelper.SelectProperties(data, selectedprop);
                return Ok(new
                {
                    status = true,
                    StatusCode = 200,
                    data = newData
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
        [Route("api/getMeetingConclusion")]
        public IHttpActionResult GetMeetingCoclusion(int MeetId)
        {
            try
            {
                var data = _adminServices.getConclusion(MeetId);
                return Ok(new
                {
                    status = true,
                    StatusCode = 200,
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
        [Route("api/UpdateMeetingConclusion")]
        public IHttpActionResult UpdateMeetingConclusion()
        {
            try
            {

                var request = HttpContext.Current.Request;
                List<string> validationErrors = new List<string>();
                if (string.IsNullOrWhiteSpace(request.Form.Get("Meeting")))
                    validationErrors.Add("Meeting Id is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("Conclusion")))
                    validationErrors.Add("Meeting conclusion is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("FollowUpStatus")))
                    validationErrors.Add("Follow up status is required.");

                var formData = new MeetingConclusion
                {
                    Meeting = Convert.ToInt32(request.Form.Get("Meeting")),
                    Conclusion = request.Form.Get("Conclusion"),
                    FollowUpStatus = Convert.ToBoolean(request.Form.Get("FollowUpStatus")),
                    NextFollowUpDate = string.IsNullOrWhiteSpace(request.Form["NextFollowUpDate"])
    ? (DateTime?)null
    : DateTime.Parse(request.Form["NextFollowUpDate"]),
                    summary = request.Form.Get("summary")
                };
                if (request.Form["MeetingMemberList"] != null)
                {
                    formData.MeetingMemberList = request.Form["MeetingMemberList"].Split(',').Select(e => int.Parse(e)).ToList();
                }
                else
                {
                    validationErrors.Add("Meeting member list is required !");
                }
                if (request.Form["MemberId"] != null)
                {
                    formData.MemberId = request.Form["MemberId"].Split(',').ToList();
                }
                else
                {
                    validationErrors.Add("Member Id is required !");
                }
                if (request.Form["KeyResponse"] != null)
                {
                    formData.KeyResponse = request.Form["KeyResponse"].Split(',').ToList();
                }
                else
                {
                    validationErrors.Add("Key responses is required !");
                }
                if (request.Form["KeyPointId"] != null)
                {
                    formData.KeyPointId = request.Form["KeyPointId"].Split(',').ToList();
                }
                else
                {
                    validationErrors.Add("Key Id is required !");
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
                bool res = _adminServices.AddMeetingResponse(formData);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Meeting conclusion updated successfully !" : "Some issue occured while processing request."
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
        [Route("api/getMemberJoiningStatus")]
        public IHttpActionResult GetMemberJoiningStatus(int meetingId)
        {
            var res = _managerservice.getMemberJoiningStatus(meetingId);
            return Ok(new { status = true, message = "data retrieved", data = res });
        }

        #endregion

        #region Admin Generate Notice
        [HttpGet]
        [Route("api/getallNoticeList")]
        public IHttpActionResult NoticeList(int? limit = null, int? page = null, int? projectId=null, string searchTerm = null)
        {
            try
            {
                var data = _adminServices.getNoticeList(limit:limit,page:page,id:projectId,searchTerm:searchTerm);
                var selectprop = new[] { "Id", "ProjectId", "ProjectManagerId", "Attachment_Url", "Notice", "ProjectManagerImage", "ProjectManager", "ProjectName", "noticeDate" };
                var newData = CommonHelper.SelectProperties(data, selectprop);
                if (data.Count > 0)
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

        [HttpPost]
        [Route("api/adminCreateNotice")]
        public IHttpActionResult CreateNotice()
        {
            var request = HttpContext.Current.Request;
            var formData = new Generate_Notice
            {
                Id = Convert.ToInt32(request.Form.Get("Id")),
                ProjectId = Convert.ToInt32(request.Form.Get("ProjectId")),
                Attachment_Url = request.Form.Get("Attachment_Url"),
                Notice = request.Form.Get("Notice")
            };
            var file = request.Files["Attachment"];
            if (file != null && file.FileName != "")
            {
                formData.Attachment_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                formData.Attachment_Url = Path.Combine("/ProjectContent/Admin/NoticeDocs/", formData.Attachment_Url);
            }

            bool res = _adminServices.InsertNotice(formData);
            if (res)
            {
                if (file != null && file.FileName != "")
                {
                    file.SaveAs(HttpContext.Current.Server.MapPath(formData.Attachment_Url));
                }
            }

            return Ok(new
            {
                status = res,
                StatusCode = res ? 200 : 500,
                message = res ? "Notice created !" : "Some issue occured !"
            });
        }
        #endregion

        #region divisionlist
        [HttpGet]
        [Route("api/DivisonList")]
        public IHttpActionResult DivisonList()
        {
            try
            {
                var data = _adminServices.ListDivison();
                if (data != null && data.Count > 0)
                {
                    return Ok(new
                    {
                        status = true,
                        StatusCode = 200,
                        message = "All divison fetched !",
                        data = data
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        message = "Some issue found while processing request."
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message,
                    data = ex
                });
            }
        }
        #endregion

        #region All ProblemList
        [HttpGet]
        [Route("api/getAllProblemList")]
        public IHttpActionResult getAllProblemList(int? limit = null, int? page = null, string searchTerm = null)
        {
            try
            {
                var res = _managerservice.getSubOrdinateProblemforAdmin(limit, page,searchTerm);
                var selectProp = new[] { "ProblemId", "ProjectName", "Title", "Description", "Attchment_Url", "CreatedDate", "newRequest", "projectCode" };
                var newdata = CommonHelper.SelectProperties(res, selectProp);
                if (res.Count > 0)
                {
                    return CommonHelper.Success(this, newdata, "Data fetched successfully", 200, res[0].Pagination);
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

        #region DesginationList
        [HttpGet]
        [Route("api/DesginationList")]
        public IHttpActionResult DesginationList()
        {
            try
            {
                var data = _adminServices.ListDesgination();
                if (data != null && data.Count > 0)
                {
                    return Ok(new
                    {
                        status = true,
                        StatusCode = 200,
                        message = "All desgination fetched !",
                        data = data
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        message = "Some issue found while processing request."
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message,
                    data = ex
                });
            }
        }
        #endregion

        #region Reimbursement
        [HttpGet]
        [Route("api/GetReimbursementList")]
        public IHttpActionResult GetReimbursementList(int? page=null, int? limit=null, int? managerId = null, string typeFilter = null, string statusFilter = null)
        {
            try
            {
                var data = _managerservice.GetReimbursements(page, limit,null,managerId:managerId, "selectAll",typeFilter:typeFilter,statusFilter:statusFilter);
                if (data.Count > 0)
                {
                    return CommonHelper.Success(this, data, "Data fetched successfully", 200, data[0].Pagination);
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
        [Route("api/ApproveReimbursement")]
        public IHttpActionResult ApproveReimbursement(int id, bool status, string type, string remark)
        {
            try
            {
                bool res = _adminServices.ReimbursementApproval(status, id, type, remark);
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
        #endregion

        #region Hiring
        [HttpGet]
        [Route("api/ApprovalHiring")]
        public IHttpActionResult ApproveHiring(int id, bool status, string remark, string location)
        {
            try
            {
                bool res = _adminServices.HiringApproval(id, status, remark, location);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = (res && status) ? "Approved Successfully" : res ? "Rejected  Successfully" : "Something went wrong"
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
        [Route("api/GetAllHiring")]
        public IHttpActionResult AllHiring(int? page=null, int? limit=null, int? managerFilter = null, int? projectFilter = null)
        {
            try
            {
                var data = _adminServices.HiringList(page, limit,managerFilter:managerFilter,projectFilter:projectFilter).ToList();
                if (data.Count > 0)
                {
                    return CommonHelper.Success(this, data, "Data fetched successfully", 200, data[0].Pagination);
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

        #region Tour Proposal

        [HttpGet]
        [Route("api/ViewAlltourAdminView")]
        public IHttpActionResult AllTour(int? page=null, int? limit=null, int? managerFilter = null, int? projectFilter = null)
        {
            try
            {
                var data = _adminServices.getAllTourList(page, limit,managerFilter:managerFilter,projectFilter:projectFilter).ToList();

                if (data.Count > 0)
                {
                    return CommonHelper.Success(this, data, "Data fetched successfully", 200, data[0].Pagination);
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
        [Route("api/ApproveTourProposal")]
        public IHttpActionResult ApproveTourProposal(int id, bool status, string remark)
        {
            try
            {
                bool res = _adminServices.Tourapproval(id, status, remark);
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

        #endregion

        #region BudgetFor Graph
        [Route("api/budgetforgraph")]
        [HttpGet]
        public IHttpActionResult GetBudgetForGraph()
        {
            try
            {
                var data = _adminServices.BudgetForGraph();
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
                    Message = ex.Message
                });
            }
        }
        #endregion
        
        #region Report
        [HttpGet]
        [Route("api/ReimbursementReport")]
        public IHttpActionResult ReimbursementReport(int? limit=null,int?page=null, int? projectManagerFilter = null, string typeFilter = null, string statusFilter = null)
        {
            try
            {
                var data = _managerservice.GetReimbursements(limit:limit,page:page,type: "selectReinbursementReport", managerId: projectManagerFilter, typeFilter: typeFilter, statusFilter: statusFilter);
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
        [Route("api/HiringReportProjects")]
        public IHttpActionResult HiringReportProjects()
        {
            try
            {
                var data = _adminServices.hiringreportprojects();
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
        [Route("api/HiringReportByProject")]
        public IHttpActionResult HiringReportByProject(int projectId)
        {
            try
            {
                var data = _managerservice.GetHiringVehicles(id:projectId, type: "GetById");
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
        [Route("api/tourproposalproject")]
        public IHttpActionResult tourproposalproject()
        {
            try
            {
                var data = _adminServices.TourReportProject();
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
        [Route("api/tourproposalbyproject")]
        public IHttpActionResult tourproposalbyproject(int projectId)
        {
            try
            {
                var data = _adminServices.getAllTourList(id:projectId,type: "GetById");
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

        #region RaisedProblem
        [HttpGet]
        [Route("api/getraisedproblemforadmin")]
        public IHttpActionResult getRaisedProblem(int?limit=null,int?page=null,string searchTerm = null)
        {
            try
            {
                var data = _adminServices.getProblemList(limit:limit,page:page,searchTerm:searchTerm);
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
        [System.Web.Mvc.AllowAnonymous]
        [HttpGet]
        [Route("api/getRaisedProblemById")]
        public IHttpActionResult getRaisedProblemById(int id)
        {
            try
            {
                var data = _adminServices.getProblemList(null, null, id, null);
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

        #region expenses
        [HttpGet]
        [Route("api/viewExpenses")]
        public IHttpActionResult viewExpenses(int id)
        {
            try
            {
                var data = _adminServices.ProjectBudgetList(id);
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

        #region meeting by admin
        [HttpGet]
        [Route("api/getmeetingforadmin")]
        public IHttpActionResult getmeetingadmin(int? limit=null,int?page=null)
        {
            try
            {
                var data = _adminServices.getAllmeeting(limit,page).Where(d=>d.createdBy=="admin");
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
        [Route("api/getmeetingforprojectmanager")]
        public IHttpActionResult getmeetingprojectmanager(int? limit=null,int?page = null)
        {
            try
            {
                var data = _adminServices.getAllmeeting(limit,page).Where(d=>d.createdBy=="projectmanager");
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

        #region Report for mobile app
        [HttpGet]
        [Route("api/allhiringreport")]
        public IHttpActionResult AllHiringReport(int?limit=null,int?page=null, int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
        {
            try
            {
                var data = _adminServices.HiringReort(limit,page, managerFilter: managerFilter, projectFilter: projectFilter, statusFilter: statusFilter);
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
        [Route("api/alltourproposalreport")]
        public IHttpActionResult AllTourproposalReport(int?limit=null,int? page=null, int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
        {
            try
            {
                var data = _accountService.getTourList(limit,page, managerFilter: managerFilter, projectFilter: projectFilter, statusFilter: statusFilter);
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
        [Route("api/getOutsource")]
        public IHttpActionResult GetOutsource(int userId)
        {
            try
            {
                var data = _managerservice.selectAllOutSOurceList(userId);
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

        #region Graph
        [HttpGet]
        [Route("api/getphyfinGraphData")]
        public IHttpActionResult getgraphData(int? page, int? limit)
        {
            try
            {
                DateTime twoYearsAgo = DateTime.Now.AddYears(-2);
                var data = _adminServices.Project_List(page, limit).Where(d => d.AssignDate >= twoYearsAgo).ToList();
                var selectProperties = new[] { "Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl", "ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode" };
                var newData = CommonHelper.SelectProperties(data, selectProperties);
                if (data.Count > 0)
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
        #endregion

        #region
        [System.Web.Mvc.AllowAnonymous]
        [HttpGet]
        [Route("api/getOutsourceByPm")]
        public IHttpActionResult getOutsourceByProjectManager(int projectManager,string searchTerm = null)
        {
            try
            {
                var data = _managerservice.getAttendanceCount(projectManager,searchTerm);
                var selectprop = new[] { "EmpId", "EmpName", "present", "absent" };
                var newdata = CommonHelper.SelectProperties(data, selectprop);
                return Ok(new
                {
                    status = data.Any(),
                    data = newdata,
                    StatusCode = 200
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
        [Route("api/getAttendanceListByEmp")]
        public IHttpActionResult getAttendanceListByEmp(int projectManager, int EmpId)
        {
            try
            {
                var data = _managerservice.GetAllAttendanceForProjectManager(projectManager, EmpId);
                return Ok(new
                {
                    status = data.Any(),
                    data = data,
                    StatusCode = 200
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
        [Route("api/MonthFilter")]
        public IHttpActionResult monthFilter(int month, int year, int projectManager, int EmpId)
        {
            try
            {
                var data = _managerservice.getReportAttendance(month, year, projectManager, EmpId);
                return Ok(new
                {
                    status = data.Any(),
                    data = data,
                    StatusCode = 200
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
        [Route("api/getAttendanceByEmp")]
        public IHttpActionResult getAttendanceByEmp(int projectManager, int EmpId)
        {
            try
            {
                var data = _managerservice.GetAllAttendanceForProjectManager(projectManager, EmpId);
                return Ok(new
                {
                    status = data.Any(),
                    data = data,
                    StatusCode = 200
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
        [Route("api/getAttendanceRepo")]
        public IHttpActionResult getAttendanceRepo(int month, int year, int projectManager, int? EmpId = null)
        {
            try
            {
                var data = _managerservice.getReportAttendance(month, year, projectManager, EmpId.HasValue ? Convert.ToInt32(EmpId) : 0);
                return Ok(new
                {
                    status = data.Any(),
                    data = data,
                    StatusCode = 200
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

        #region Employee Monthly Report
        [HttpGet]
        [Route("api/getEmployeeMonthlyReportForAdmin")]
        public IHttpActionResult EmployeeMonthlyReport(int empId)
        {
            try
            {
                var data = _managerservice.GetEmpReport(empId);
                return Ok(new
                {
                    status = data.Any(),
                    data = data,
                    StatusCode = 200,
                    message = data.Any() ? "Data found !" : "Data not found !"
                });
            }
            catch
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = "Some issue occured while processing request."
                });
            }
        }
        #endregion
        private IHttpActionResult BadRequest(object value)
        {
            return Content(HttpStatusCode.BadRequest, value);
        }
    }
}
