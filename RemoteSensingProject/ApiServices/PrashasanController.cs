using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
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
