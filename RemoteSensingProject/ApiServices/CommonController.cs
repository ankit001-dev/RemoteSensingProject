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
        private IHttpActionResult BadRequest(object value)
        {
            return Content(HttpStatusCode.BadRequest, value);
        }
    }
}
