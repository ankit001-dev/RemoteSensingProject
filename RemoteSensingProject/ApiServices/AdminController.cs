using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Razor.Generator;
using Microsoft.Ajax.Utilities;
using RemoteSensingProject.Controllers;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.ProjectManager;
using static RemoteSensingProject.Models.Admin.main;
using static RemoteSensingProject.Models.LoginManager.main;

namespace RemoteSensingProject.ApiServices
{
    public class AdminController : ApiController
    {
        private readonly AdminServices _adminServices;
        private readonly ManagerService _managerservice;
        private readonly LoginServices _loginService;
        public AdminController()
        {
            _adminServices = new AdminServices();
            _loginService = new LoginServices();
            _managerservice = new ManagerService();
        }
        [HttpPost]
        [Route("api/login")]
        public IHttpActionResult Login()
        {
            try
            {

                var request = HttpContext.Current.Request;
                string username = request.Form.Get("username");
                string password = request.Form.Get("password");
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 400,
                        Message = "Username and password are required."
                    });
                }
                Credentials data = _loginService.Login(username, password);

                if (username.Equals(data.username) && password.Equals(data.password))
                {
                    return Ok(new
                    {
                        status = true,
                        StatusCode = 200,
                        message = "User authorised successfully !",
                        data = data
                    });
                }
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 400,
                    message = "Invalid userid or password."
                });
            } catch (Exception ex) {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message,
                    data = ex
                });
            }


        }

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
            }catch(Exception ex)
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
        public IHttpActionResult All_EmpList()
        {
            try
            {
                var data = _adminServices.SelectEmployeeRecord();
                if (data != null && data.Count > 0)
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
                        StatusCode = 500,
                        message = "No data found!"
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

        [HttpPut]
        [Route("api/updateEmployeeData")]
        public IHttpActionResult Update_Employee()
        {
            try { 
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
                if(data != null && data.Id > 0)
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
            }catch(Exception ex)
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
                if (data.Id <=0)
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
            
            }catch(Exception ex)
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
            try { 
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

        [HttpGet]
        [Route("api/adminDelayProject")]
        public IHttpActionResult DelayProject()
        {
            try
            {
                var data = _adminServices.Project_List().Where(d => d.CompletionDate < DateTime.Now && !d.ProjectStatus).ToList();
                return Ok(new
                {
                    status = true,
                    message = "Delay project data !",
                    data = data,

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

        [HttpGet]
        [Route("api/adminOngoingProject")]
        public IHttpActionResult ongoingProject()
        {
            try
            {
                var data = _adminServices.Project_List().Where(d => d.CompletionDate > DateTime.Now && d.StartDate<DateTime.Now).ToList();
                return Ok(new
                {
                    status = true,
                    message = "Delay project data !",
                    data = data,

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
        [Route("api/adminCompleteProject")]
        public IHttpActionResult completeProject()
        {
            try
            {
                var data = _adminServices.Project_List().Where(d => d.ProjectStatus).ToList();
                return Ok(new
                {
                    status = true,
                    message = "Delay project data !",
                    data = data,

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

        #region Create Project
        [HttpGet]
        [Route("api/adminProjectList")]
        public IHttpActionResult getProjectList()
        {
            try
            {
                var data = _adminServices.Project_List();
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
                    message = ex.Message,
                    data = ex
                });
            }
        }

        [HttpPost]
        [Route("api/adminCreateProject")]
        public IHttpActionResult CreateProject()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var formData = new Project_model
                {
                    Id = Convert.ToInt32(request.Form.Get("Id")),
                    ProjectTitle = request.Form.Get("ProjectTitle"),
                    AssignDate = Convert.ToDateTime(request.Form.Get("AssignDate")),
                    StartDate = Convert.ToDateTime(request.Form.Get("StartDate")),
                    CompletionDate = Convert.ToDateTime(request.Form.Get("CompletionDate")),
                    ProjectManager = request.Form.Get("ProjectManager"),
                    ProjectBudget = Convert.ToDecimal(request.Form.Get("ProjectBudget")),
                    ProjectDescription = request.Form.Get("ProjectDescription"),
                    projectDocumentUrl = request.Form.Get("projectDocumentUrl"),
                    ProjectType = request.Form.Get("ProjectType"),
                    ProjectStage = Convert.ToBoolean(request.Form.Get("ProjectStage")),
                    createdBy = request.Form.Get("createdBy")
                };
                if (request.Form["SubOrdinate"] != null)
                {
                    formData.SubOrdinate = request.Form["SubOrdinate"].Split(',').Select(value => int.Parse(value.ToString())).ToArray();
                }
                if (formData.ProjectType.Equals("External"))
                {
                    formData.ContactPerson = request.Form.Get("ContactPerson");
                    formData.ProjectDepartment = request.Form.Get("ProjectDepartment");
                    formData.Address = request.Form.Get("Address");
                }
                var file = request.Files["projectDocument"];
                if (file != null && file.FileName != "")
                {
                    formData.projectDocumentUrl = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    formData.projectDocumentUrl = Path.Combine("/ProjectContent/Admin/ProjectDocs/", formData.projectDocumentUrl);
                }
                // Validations
                List<string> validationErrors = new List<string>();
                if (string.IsNullOrWhiteSpace(formData.ProjectTitle))
                    validationErrors.Add("Project Title is required.");

                if (formData.AssignDate == null)
                    validationErrors.Add("Assign Date is required.");

                if (formData.StartDate == null)
                    validationErrors.Add("Start Date is required.");

                if (formData.CompletionDate == null)
                    validationErrors.Add("Completion Date is required.");

                if (string.IsNullOrWhiteSpace(formData.ProjectManager))
                    validationErrors.Add("Project Manager is required.");

                if (string.IsNullOrWhiteSpace(formData.ProjectBudget.ToString()))
                    validationErrors.Add("Project Budget is required.");

                
                if (!decimal.TryParse(request.Form.Get("ProjectBudget"), out decimal projectBudget))
                    validationErrors.Add("Invalid Project Budget format.");

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
                    bool res = _adminServices.createApiProject(formData);
                    if (res)
                    {
                        if(file != null && file.FileName != "")
                        {
                            file.SaveAs(HttpContext.Current.Server.MapPath(formData.projectDocumentUrl));
                        }
                    }
                    return Ok(new
                    {
                        status = res,
                        StatusCode = res ? 200 : 500,
                        message = res ? "Project created successfully !" : "Some issue occured ! Please try after sometime."
                    });
                }
            }catch(Exception ex)
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
        [Route("api/adminAddBudgets")]
        public IHttpActionResult AddBudgets()
        {
            try{
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
            catch(Exception ex)
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


        [HttpGet]
        [Route("api/getProjectStages")]
        public IHttpActionResult GetProjectSatges(int projectId)
        {
            try
            {
                var data = _adminServices.ProjectStagesList(projectId);
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
        #endregion


        #region minute of meeting
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
                    Id = Convert.ToInt32(request.Form.Get("Id")),
                    MeetingType = request.Form.Get("MeetingType"),
                    MeetingLink = request.Form.Get("MeetingLink"),
                    MeetingTitle = request.Form.Get("MeetingTitle"),
                    MeetingAddress = request.Form.Get("MeetingAddress"),
                    MeetingTime = Convert.ToDateTime(request.Form.Get("MeetingTime")),
                    Attachment_Url = request.Form.Get("Attachment_Url"),
                    CreaterId = Convert.ToInt32(request.Form["CreaterId"] ?? "0")
                };
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
                    formData.meetingMemberList = request.Form["meetingMemberList"].Split(',').Select(value => int.Parse(value.ToString())).ToList();
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
                    if(file != null && file.FileName != "")
                    {
                        file.SaveAs(HttpContext.Current.Server.MapPath(formData.Attachment_Url));
                    }
                }
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Meeting created successfully !" : "Some issue occured while processing request."
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


        [HttpPut]
        [Route("api/updateadminMeeting")]
        public IHttpActionResult UpdateMeeting()
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
                    Id = Convert.ToInt32(request.Form.Get("Id")),
                    MeetingType = request.Form.Get("MeetingType"),
                    MeetingLink = request.Form.Get("MeetingLink"),
                    MeetingAddress = request.Form.Get("MeetingAddress"),
                    MeetingTitle = request.Form.Get("MeetingTitle"),
                    MeetingTime = Convert.ToDateTime(request.Form.Get("MeetingTime")),
                    Attachment_Url = request.Form.Get("Attachment_Url")
                };
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
                    formData.meetingMemberList = request.Form["meetingMemberList"].Split(',').Select(value => int.Parse(value.ToString())).ToList();
                }

                if (request.Form["keyPointList"] != null)
                {
                    formData.keyPointList = request.Form["keyPointList"].Split(',').ToList();
                }

                if (request.Form["KeypointId"] != null)
                {
                    formData.KeypointId = request.Form["KeypointId"].Split(',').ToList();
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
                bool res = _adminServices.UpdateMeeting(formData);
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
                    message = res ? "Meeting created successfully !" : "Some issue occured while processing request."
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


        [HttpGet]
        [Route("api/adminMeetingList")]
        public IHttpActionResult MeetingList()
        {
            try
            {
                var data = _adminServices.getAllmeeting();
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
                    StatusCode = data.Count > 0 ? 200 : 500,
                    message = "Data found !",
                    data = data
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


        [HttpGet]
        [Route("api/GetMeetingMemberListById")]
        public IHttpActionResult GetMeetingMemberList(int meetId)
        {
            try
            {
                var data = _adminServices.GetMeetingMemberList(meetId);
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
                    formData.MeetingMemberList = request.Form["MeetingMemberList"].Split(',').Select(e=>int.Parse(e)).ToList();
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
                    message = res ? "Meeting created successfully !" : "Some issue occured while processing request."
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
        public IHttpActionResult NoticeList()
        {
            try
            {
                var data = _adminServices.getNoticeList();
                if (!data.Any())
                {
                    return Ok(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = "Data Not found !"
                    });
                }
                return Ok(new
                {
                    status = true,
                    StatusCode = 200,
                    message = "Data found !",
                    data = data
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
        [Route("api/adminCreateNotice")]
        public IHttpActionResult CreateNotice()
        {
            var request = HttpContext.Current.Request;
            var formData = new Generate_Notice { 
                Id = Convert.ToInt32(request.Form.Get("Id")),
                ProjectId = Convert.ToInt32(request.Form.Get("ProjectId")),
                Attachment_Url = request.Form.Get("Attachment_Url"),
                Notice = request.Form.Get("Notice")
            };
            var file = request.Files["Attachment"];
            if(file != null && file.FileName != "")
            {
                formData.Attachment_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                formData.Attachment_Url = Path.Combine("/ProjectContent/Admin/NoticeDocs/", formData.Attachment_Url);
            }

            bool res = _adminServices.InsertNotice(formData);
            if (res)
            {
                if(file != null && file.FileName != "")
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
            try { 
            var data = _adminServices.ListDivison();
            if(data != null && data.Count > 0)
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
        public IHttpActionResult getAllProblemList()
        {
            var res =_managerservice.getSubOrdinateProblemforAdmin();


            return Ok(new { status = true, message = "data retrieved", data = res });
        }
        #endregion

        #region DesginationList
        [HttpGet]
        [Route("api/DesginationList")]
        public IHttpActionResult DesginationList()
        {
            try { 
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
        [Route("api/ViewReinbursementAdminView")]
        public IHttpActionResult viewReinbursement(int userId, string type)
        {
            try
            {
                var data = _adminServices.GetSpecificUserReimbursements(userId, type);
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
        [Route("api/ApproveReimbursement")]
        public IHttpActionResult ApproveReimbursement(int id, bool status,string type)
        {
            try
            {
                bool res = _adminServices.ReimbursementApproval(status,id,type);
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
        public IHttpActionResult ApproveHiring(int id,bool status)
        {
            try
            { 
                bool res = _adminServices.HiringApproval(id,status);
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
        [Route("api/ViewHiringAdminView")]
        public IHttpActionResult AllHiring()
        {
            try
            {
                var data = _adminServices.HiringList();
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

        #region Tour Proposal

        [HttpGet]
        [Route("api/ViewAlltourAdminView")]
        public IHttpActionResult AllTour()
        {
            try
            {
                var data = _adminServices.getAllTourList();
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
        [Route("api/ApproveTourProposal")]
        public IHttpActionResult ApproveTourProposal(int id, bool status)
        {
            try
            {
                bool res = _adminServices.Tourapproval(id, status);
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
            catch(Exception ex)
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
        private IHttpActionResult BadRequest(object value)
        {
            return Content(HttpStatusCode.BadRequest, value);
        }
    }
}
