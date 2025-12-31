using Newtonsoft.Json;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using static RemoteSensingProject.Models.Admin.main;

namespace RemoteSensingProject.ApiServices
{
    [JwtAuthorize(Roles = "admin,account,projectManager,subOrdinate,outSource")]
    public class CommonController : ApiController
    {
        private readonly AdminServices _adminServices;
        private readonly ManagerService _managerservice;
        private readonly LoginServices _loginService;
        private readonly AccountService _accountService;
        public CommonController()
        {
            _adminServices = new AdminServices();
            _loginService = new LoginServices();
            _managerservice = new ManagerService();
            _accountService = new AccountService();
        }
        #region Add Project
        [RoleAuthorize("admin,projectManager")]
        [HttpPost]
        [Route("api/adminCreateProject")]
        public IHttpActionResult CreateProject()
        {
            List<string> savedFiles = new List<string>();
            try
            {
                var request = HttpContext.Current.Request;
                var form = request.Form;
                var files = request.Files;

                // ✅ Initialize main model
                var model = new createProjectModel();
                model.pm = new Project_model();

                model.pm.Id = Convert.ToInt32(form["Id"] ?? "0");
                model.pm.ProjectTitle = form["ProjectTitle"];
                model.pm.ProjectManager = form["ProjectManager"];
                model.pm.ProjectDescription = form["ProjectDescription"];
                model.pm.ProjectType = form["ProjectType"];
                model.pm.letterNo = form["letterNo"] ?? "0";
                if (string.IsNullOrEmpty((form["createdBy"])))
                    return CommonHelper.Error(this, "Created By is required", 500);
                model.pm.createdBy = form["createdBy"].ToString();

                // Project Stages
                if (!Boolean.TryParse(form["projectStage"], out var projectStages))
                    return CommonHelper.Error(this, "Project Stages is required", 500);
                model.pm.ProjectStage = projectStages;

                // ✅ Dates
                if (!DateTime.TryParse(form["AssignDate"], out var assignDate))
                    return CommonHelper.Error(this, "Assign Date is required.", 500);
                model.pm.AssignDate = assignDate;

                if (!DateTime.TryParse(form["StartDate"], out var startDate))
                    return CommonHelper.Error(this, "Start Date is required.", 500);
                model.pm.StartDate = startDate;

                if (!DateTime.TryParse(form["CompletionDate"], out var compDate))
                    return CommonHelper.Error(this, "Completion Date is required.", 500);
                model.pm.CompletionDate = compDate;

                // ✅ Budget
                if (!decimal.TryParse(form["ProjectBudget"], out var budget))
                    return CommonHelper.Error(this, "Invalid Project Budget", 500);
                model.pm.ProjectBudget = budget;
                // ✅ HRcount
                if (!int.TryParse(form["hrcount"], out var hr))
                    return CommonHelper.Error(this, "Invalid Human resources count", 500);
                model.pm.hrCount = hr;

                // ✅ External Project Extra Fields
                if (model.pm.ProjectType?.ToLower() == "external")
                {
                    model.pm.ContactPerson = form["ContactPerson"];
                    model.pm.ProjectDepartment = form["ProjectDepartment"];
                    model.pm.Address = form["Address"];
                }

                // ✅ Subordinates (ID list)
                if (!string.IsNullOrEmpty(form["SubOrdinate"]))
                {
                    model.pm.SubOrdinate =
                        form["SubOrdinate"]
                        .Split(',')
                        .Select(int.Parse)
                        .ToArray();
                }

                // ✅ Parse budgets JSON
                if (!string.IsNullOrEmpty(form["budgets"]))
                {
                    model.budgets = JsonConvert.DeserializeObject<List<Project_Budget>>(form["budgets"]);

                    // ✅ Validate sum of ProjectAmount ≤ ProjectBudget
                    if (model.budgets != null && model.budgets.Any())
                    {
                        decimal totalBudgetAmount = model.budgets.Sum(b => b.ProjectAmount);
                        if (totalBudgetAmount > model.pm.ProjectBudget)
                        {
                            return CommonHelper.Error(this,
                                $"Total of all ProjectAmounts ({totalBudgetAmount}) cannot exceed the main ProjectBudget ({model.pm.ProjectBudget}).",
                                400);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(form["hrcount"]))
                {
                    model.hr = JsonConvert.DeserializeObject<List<HumanResources>>(form["hr"]);

                    // ✅ Validate sum of ProjectAmount ≤ ProjectBudget
                    if (model.hr != null && model.hr.Any())
                    {
                        decimal totalhrCount = model.hr.Sum(b => b.designationCount);
                        if (totalhrCount > model.pm.hrCount)
                        {
                            return CommonHelper.Error(this,
                                $"Total of all Human Resources ({totalhrCount}) cannot exceed the Human Resources count ({model.pm.hrCount}).",
                                400);
                        }
                    }
                }

                // ✅ Parse stages JSON
                if (!string.IsNullOrEmpty(form["stages"]) && model.pm.ProjectStage)
                {
                    model.stages = JsonConvert.DeserializeObject<List<Project_Statge>>(form["stages"]);
                }

                // ✅ Upload Project Document (single)
                string filePage = HttpContext.Current.Server.MapPath("~/ProjectContent/Admin/ProjectDocs/");
                if (!Directory.Exists(filePage))
                    Directory.CreateDirectory(filePage);
                var projectFile = files["projectDocument"];
                if (projectFile != null && projectFile.ContentLength > 0)
                {
                    string fileName = DateTime.Now.ToString("ddMMyyyy") + "_" + Guid.NewGuid()
                                      + Path.GetExtension(projectFile.FileName);
                    string filePath = "/ProjectContent/Admin/ProjectDocs/" + fileName;

                    model.pm.projectDocumentUrl = filePath;
                    projectFile.SaveAs(HttpContext.Current.Server.MapPath(filePath));
                    savedFiles.Add(filePath);
                }

                // ✅ Upload Stage Documents (multiple)
                if (model.stages != null && model.pm.ProjectStage)
                {
                    for (int i = 0; i < model.stages.Count; i++)
                    {
                        var stageFile = files["StageDocument_" + i];
                        if (stageFile != null && stageFile.ContentLength > 0)
                        {
                            string fileName = "STAGE_" + i + "_" + DateTime.Now.ToString("ddMMyyyy")
                                              + "_" + Guid.NewGuid() + Path.GetExtension(stageFile.FileName);

                            string filePath = "/ProjectContent/Admin/ProjectDocs/" + fileName;

                            // ✅ Save file
                            stageFile.SaveAs(HttpContext.Current.Server.MapPath(filePath));

                            // ✅ Store URL in model
                            model.stages[i].StageDocument_Url = filePath;
                            savedFiles.Add(filePath);
                        }
                    }
                }

                // ✅ Save project in DB
                bool result = _adminServices.addProject(model);

                if (result)
                {
                    return CommonHelper.Success(this,model.pm.Id<=0? "Project created successfully!": "Project updated successfully!");
                }
                else
                {
                    return CommonHelper.Error(this, "Something went wrong.", 500);
                }
            }
            catch (Exception ex)
            {
                foreach (var f in savedFiles)
                {
                    if (File.Exists(f))
                        File.Delete(f);
                }
                return CommonHelper.Error(this, ex.Message, 500);

            }
        }

        [RoleAuthorize("admin,projectManager")]
        [HttpGet]
        [Route("api/GetadminProjectDetailById")]
        public IHttpActionResult GetProjectById(int Id)
        {
            try
            {
                var data = _adminServices.GetProjectById(Id);
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

        #endregion

        #region Add Meeting
        [RoleAuthorize("admin,projectManager")]
        [HttpPost]
        [Route("api/adminCreateMeeting")]
        public IHttpActionResult CreateMeeting()
        {
            try
            {
                var request = HttpContext.Current.Request;
                List<string> validationErrors = new List<string>();
                if (string.IsNullOrWhiteSpace(request.Form.Get("MeetingType")))
                    validationErrors.Add("Meeting Type is required.");


                if (string.IsNullOrWhiteSpace(request.Form.Get("MeetingTitle")))
                    validationErrors.Add("Meeting title is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("MeetingTime")))
                    validationErrors.Add("Meeting time is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("meetingMemberList")))
                    validationErrors.Add("Meeting member is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("keyPointList")))
                    validationErrors.Add("Key points is required.");

                var formData = new AddMeeting_Model
                {
                    // For Id, use TryParse to avoid invalid format error
                    Id = int.TryParse(request.Form.Get("Id"), out var id) ? id : 0,

                    // For MeetingType, MeetingLink, MeetingTitle, MeetingAddress, use null coalescing operator for string defaults
                    MeetingType = request.Form.Get("MeetingType") ?? string.Empty,
                    MeetingLink = request.Form.Get("MeetingLink") ?? string.Empty,
                    MeetingTitle = request.Form.Get("MeetingTitle") ?? string.Empty,
                    MeetingAddress = request.Form.Get("MeetingAddress") ?? string.Empty,

                    // For MeetingTime, use DateTime.TryParse to avoid invalid format error
                    MeetingTime = DateTime.TryParse(request.Form.Get("MeetingTime"), out var meetingTime) ? meetingTime : DateTime.MinValue,

                    // For Attachment_Url, use null coalescing operator to default to empty string if null
                    Attachment_Url = request.Form.Get("Attachment_Url") ?? string.Empty,

                    // For CreaterId, use TryParse to handle invalid formats
                    CreaterId = int.TryParse(request.Form.Get("CreaterId"), out var createrId) ? createrId : 0
                };
                string filePage = HttpContext.Current.Server.MapPath("~/ProjectContent/Admin/Meeting_Attachment/");
                if (!Directory.Exists(filePage))
                    Directory.CreateDirectory(filePage);
                var file = request.Files["Attachment"];
                if (file != null && file.FileName != "")
                {
                    formData.Attachment_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    formData.Attachment_Url = Path.Combine("/ProjectContent/Admin/Meeting_Attachment/", formData.Attachment_Url);
                }
                else if (string.IsNullOrWhiteSpace(request.Form.Get("Attachment_Url")))
                    validationErrors.Add("Meeting attachment is required.");

                if (request.Form["meetingMemberList"] != null)
                {
                    formData.meetingMemberList = request.Form["meetingMemberList"] != null ? request.Form["meetingMemberList"].Split(',').Select(value => int.Parse(value.ToString())).ToList() : new List<int>();
                }

                if (request.Form["keyPointList"] != null)
                {
                    formData.keyPointList = request.Form["keyPointList"].Split(',').ToList();
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
                bool res = _adminServices.insertMeeting(formData);
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
                    StatusCode = res ? formData.Id > 0 ? 200 : 201 : 500,
                    message = res ? formData.Id > 0 ? "Meeting updated successfully" : "Meeting created successfully !" : "Some issue occured while processing request."
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

        [RoleAuthorize("admin,projectManager")]
        [HttpGet]
        [Route("api/GetMeetingById")]
        public IHttpActionResult GetMeetingById(int Id)
        {
            try
            {
                var data = _adminServices.getMeetingById(Id);
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
        #endregion
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
                    string mess = null;
                    bool res = _adminServices.AddEmployees(empData, out mess);
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
                        message = res ? "Employee profile updation completed successfully !" : mess
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
        #region DesginationList
        [RoleAuthorize("admin,projectManager")]
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

        #region Attendance
        [RoleAuthorize("admin,projectManager")]
        [HttpGet]
        [Route("api/getattendancebyIdofEmp")]
        public IHttpActionResult GetAttendanceByIdOfEmp(int projectManager, int EmpId)
        {
            try
            {
                var data = _managerservice.GetAllAttendanceForProjectManager(projectManager, EmpId);
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
        private IHttpActionResult BadRequest(object value)
        {
            return Content(HttpStatusCode.BadRequest, value);
        }
    }
}
