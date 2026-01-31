using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    }
}
