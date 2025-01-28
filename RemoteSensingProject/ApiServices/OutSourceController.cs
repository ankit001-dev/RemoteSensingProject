using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.ProjectManager;
using RemoteSensingProject.Models.SubOrdinate;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static RemoteSensingProject.Models.SubOrdinate.main;

namespace RemoteSensingProject.ApiServices
{
    public class OutSourceController : ApiController
    {
        private readonly AdminServices _adminServices;
        private readonly ManagerService _managerservice;
        private readonly LoginServices _loginService;
        private readonly SubOrinateService _subordinate;
        public OutSourceController()
        {
            _adminServices = new AdminServices();
            _loginService = new LoginServices();
            _managerservice = new ManagerService();
            _subordinate = new SubOrinateService();
        }
        [HttpGet]
        [Route("api/getOutSourceTask")]
        public IHttpActionResult getOutSourceAssignTask(int id)
        {
            try
            {
                var taskList = _subordinate.getOutSourceTask(id);

                if (taskList != null)
                {
                    return Ok(new
                    {
                        status = true,
                        data = taskList,
                        message = "Data found !"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = false,
                        data = taskList,
                        message = "Data not found !"
                    });
                }
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
        [Route("api/submitOutSourceTask")]
        public IHttpActionResult submitOutSourceTask()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                OutSource_Task task = new OutSource_Task
                {
                    Reason = httpRequest.Form.Get("reason"),
                    EmpId = Convert.ToInt32(httpRequest.Form.Get("EmpId")),
                    id = Convert.ToInt32(httpRequest.Form.Get("id"))
                };
                bool status = _subordinate.AddOutSourceTask(task);
                if (status)
                {
                    return Ok(new
                    {
                        status = true,
                        message = "Task added successfully !"
                    }); ;
                }
                else
                {
                    return Ok(new
                    {
                        status = false,
                        message = "Some issue found while processing request. Please try after sometime."
                    });
                }
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

        private IHttpActionResult BadRequest(object value)
        {
            throw new NotImplementedException();
        }
    }
}
