using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.Ajax.Utilities;
using RemoteSensingProject.Controllers;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using static RemoteSensingProject.Models.Admin.main;
using static RemoteSensingProject.Models.LoginManager.main;

namespace RemoteSensingProject.ApiServices
{
    public class AdminController : ApiController
    {
        private readonly AdminServices _adminServices;
        private readonly LoginServices _loginService;
        public AdminController()
        {
            _adminServices = new AdminServices();
            _loginService = new LoginServices();    
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
        #endregion

        #region Add Project
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
                    ProjectTitle = request.Form.Get("ProjectTitle"),
                    AssignDate = Convert.ToDateTime(request.Form.Get("AssignDate")),
                    StartDate = Convert.ToDateTime(request.Form.Get("StartDate")),
                    CompletionDate = Convert.ToDateTime(request.Form.Get("CompletionDate")),
                    ProjectManager = request.Form.Get("ProjectManager"),
                    ProjectBudget = Convert.ToDecimal(request.Form.Get("ProjectManager")),
                    ProjectDescription = request.Form.Get("ProjectDescription"),
                    projectDocumentUrl = request.Form.Get("projectDocumentUrl"),
                    ProjectType = request.Form.Get("ProjectType"),
                    ProjectStage = Convert.ToBoolean(request.Form.Get("ProjectStage"))
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
                var file = request.Files["ProjectStage"];
                if (file != null && file.FileName != "")
                {
                    formData.projectDocumentUrl = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    formData.projectDocumentUrl = "/ProjectContent/Admin/PrjectDocs/" + formData.projectDocumentUrl;
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

                if (string.IsNullOrWhiteSpace(request.Form.Get("Miscell_amt")))
                    validationErrors.Add("Miscellaneous Amount is required.");

                if (!int.TryParse(request.Form.Get("Project_Id"), out int projectId))
                    validationErrors.Add("Invalid Project ID format.");

                if (!decimal.TryParse(request.Form.Get("ProjectAmount"), out decimal projectAmount))
                    validationErrors.Add("Invalid Project Amount format.");

                if (!decimal.TryParse(request.Form.Get("Miscell_amt"), out decimal miscellAmt))
                    validationErrors.Add("Invalid Miscellaneous Amount format.");

                var formData = new Project_Budget
                {
                    Project_Id = Convert.ToInt32(request.Form.Get("Project_Id")),
                    ProjectHeads = request.Form.Get("ProjectHeads"),
                    ProjectAmount = Convert.ToDecimal(request.Form.Get("ProjectAmount")),
                    Miscellaneous = request.Form.Get("Miscellaneous"),
                    Miscell_amt = Convert.ToDecimal("Miscell_amt")
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
                bool res = _adminServices.insertProjectBudgets(formData);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Project budget added successfully !" : "Some issue occured !"
                });
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
        [Route("api/")]
        #endregion

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
        private IHttpActionResult BadRequest(object value)
        {
            return Content(HttpStatusCode.BadRequest, value);
        }
    }
}
