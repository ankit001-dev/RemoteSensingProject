using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.ProjectManager;

namespace RemoteSensingProject.ApiServices
{
    public class ProjectManagerController : ApiController
    {
        private readonly AdminServices _adminServices;
        private readonly LoginServices _loginService;
        private readonly ManagerService _managerService;
        public ProjectManagerController()
        {
            _adminServices = new AdminServices();
            _loginService = new LoginServices();
            _managerService = new ManagerService();
        }


        #region Project substances
        [HttpGet]
        [Route("api/getProjectExpencesList")]
        public IHttpActionResult GetExpencesList(int projectId, int headId)
        {
            try
            {
                var data = _managerService.ExpencesList(projectId, headId);
                if (!data.Any())
                {
                    return Ok(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = "Data not found !"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = true,
                        StatusCode = 200,
                        data = data,
                        message = "Data found!"
                    });
                }
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

        #region dashboard
        [HttpGet]
        [Route("api/ManagerDashboard")]
        public IHttpActionResult Dashboard(int userId)
        {
            try
            {
                return Ok();
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

        #region Assigned PRoject
        [HttpGet]
        [Route("api/managerAssignedProject")]
        public IHttpActionResult AssignedPRoject(int userId)
        {
            try
            {
                return Ok();
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
