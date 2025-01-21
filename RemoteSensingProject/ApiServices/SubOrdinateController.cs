using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.ProjectManager;
using RemoteSensingProject.Models.SubOrdinate;

namespace RemoteSensingProject.ApiServices
{
    public class SubOrdinateController : ApiController
    {

        private readonly AdminServices _adminServices;
        private readonly LoginServices _loginService;
        private readonly ManagerService _managerService;
        private readonly SubOrinateService _subOrdinate;
        public SubOrdinateController()
        {
            _subOrdinate = new SubOrinateService();
            _adminServices = new AdminServices();
            _loginService = new LoginServices();
            _managerService = new ManagerService();
        }


        #region subordiinate
        [HttpGet]
        [Route("api/subAssignedProject")]
        public IHttpActionResult assignedProject(int subId)
        {
            try
            {
                var data = _subOrdinate.getProjectBySubOrdinate(subId.ToString());
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = data.Any() ? 200 : 500,
                    message = data.Any() ? "Data found !" : "Data not found !",
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


        #endregion

        private IHttpActionResult BadRequest(object value)
        {
            return Content(HttpStatusCode.BadRequest, value);
        }
    }
}
