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
        [Route("api/delete-outsource")]
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
        private IHttpActionResult BadRequest(object value)
        {
            return Content<object>(HttpStatusCode.BadRequest, value);
        }
    }
}
