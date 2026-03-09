using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

using static RemoteSensingProject.Models.CommonHelper;

namespace RemoteSensingProject.ApiServices
{
    [RoutePrefix("api/prashasan")]
    public class PrashasanController : ApiController
    {
        private readonly AdminServices _adminServices;
        private readonly ManagerService _managerServices;
        public PrashasanController()
        {
            _adminServices = new AdminServices();
            _managerServices = new ManagerService();
        }

        [Route("dashboard-count")]
        [HttpGet]
        public IHttpActionResult DashboardCount()
        {
            try
            {
                var data = _managerServices.GetPrashasanDashboardData();

                if (data != null)
                {
                    return Success(this, data);
                }
                else
                {
                    return NoData(this);
                }
            }
            catch (Exception ex)
            {
                return Error(this, ex.Message);
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
                bool res = _managerServices.insertOutSource(formData);
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
        [Route("OutsourceList")]
        [HttpGet]
        public IHttpActionResult GetOutsourceList(int?id= null, string searchTerm= null, int? page = null, int? limit = null) {
            try
            {
                var data = _managerServices.selectAllOutSOurceList(id: id, searchTerm: searchTerm, page:page, limit:limit);
                
                if (data != null && data.Any())
                {
                    string[] selectprop = new string[7] { "Id", "EmpName", "mobileNo", "email", "gender", "designationname", "designationid" };
                    dynamic newdata = SelectProperties(data, selectprop);
                    if (id.HasValue)
                    {
                        newdata = newdata[0];
                    }
                    return Success(this, newdata, pagination: data[0].Pagination);
                }
                else
                {
                    return NoData(this);
                }
            }
            catch (Exception ex)
            {
                return Error(this, ex.Message);
            }
        }
        [HttpPost]
        [Route("delete-outsource")]
        public IHttpActionResult DeleteOutSource(int Id)
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
                List<OuterSource> data = _managerServices.selectAllOutSOurceList(id:Id);
                if (data.Count <= 0)
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = "Invalid request id !"
                    });
                }
                bool res = _managerServices.DeleteOutSource(Id);
                return Ok(new
                {
                    status = res,
                    StatusCode = (res ? 200 : 500),
                    message = (res ? "Selected outsource removed successfully !" : "Some issue occred while processing your request.")
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

        [Route("manpower-request-division")]
        [HttpGet]
        public IHttpActionResult ManpowerRequestsInDivision(string searchTerm = null, int? page = null, int? limit = null)
        {
            try
            {
                var data = _managerServices.GetManpowerRequestsInDivision(searchTerm: searchTerm, page: page, limit: limit);

                if (data != null && data.Any())
                {
                    return Success(this, data, pagination: data[0].Pagination);
                }
                else
                {
                    return NoData(this);
                }
            }
            catch (Exception ex)
            {
                return Error(this, ex.Message);
            }
        }

        [Route("manpower-request-designation")]
        [HttpGet]
        public IHttpActionResult ManpowerRequestsInDesignation(int divisionid, string searchTerm = null, int? page = null, int? limit = null)
        {
            try
            {
                var data = _managerServices.GetManpowerRequestsInDesignation(id: divisionid, searchTerm: searchTerm, page: page, limit: limit);

                if (data != null && data.Any())
                {
                    return Success(this, data, pagination: data[0].Pagination);
                }
                else
                {
                    return NoData(this);
                }
            }
            catch (Exception ex)
            {
                return Error(this, ex.Message);
            }
        }

        [Route("getoutsource-notin-division")]
        [HttpGet]
        public IHttpActionResult OutsourceNotInDevision(int designationid)
        {
            try
            {
                var data = _managerServices.OutsourceNotInDivision(designationid);

                if (data != null && data.Any())
                {
                    return Success(this, data);
                }
                else
                {
                    return NoData(this);
                }
            }
            catch (Exception ex)
            {
                return Error(this, ex.Message);
            }
        }
        [Route("add-manpower")]
        [HttpPost]
        public IHttpActionResult AddManpower([FromBody] AddManPower ap)
        {
            try
            {
                List<string> errors = new List<string>();

                if (ap == null)
                    errors.Add("Request body is empty");

                if (ap.DivisionId <= 0)
                    errors.Add("Division id is not valid");

                if (ap.DesignationId <= 0)
                    errors.Add("Designation id is not valid");

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
                bool res = _managerServices.AddManpower(ap);

                return Ok(new
                {
                    status = res,
                    StatusCode = 200,
                    message = "Manpower added successfully!"
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
        private IHttpActionResult BadRequest(object value)
        {
            return Content<object>(HttpStatusCode.BadRequest, value);
        }
    }
}
