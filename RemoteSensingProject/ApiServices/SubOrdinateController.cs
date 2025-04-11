using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.AspNetCore.Hosting.Server;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.ProjectManager;
using RemoteSensingProject.Models.SubOrdinate;
using static RemoteSensingProject.Models.SubOrdinate.main;
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

        #region Raise Problem
        [HttpPost]
        [Route("api/SubOrdinateRaiseProblem")]
         public IHttpActionResult Raise_Problem( )
         {
            try
            {
                var request = HttpContext.Current.Request;
                var data = new Raise_Problem
                {
                    Project_Id = Convert.ToInt32(request.Form.Get("Project_Id")),
                    Title = request.Form.Get("title"),
                    Description = request.Form.Get("Description"),
                    Attchment_Url = request.Form.Get("Attchment_Url")
                };
                var file = request.Files["Attachment"];
                if (file != null && file.FileName != "")
                {
                    data.Attchment_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    data.Attchment_Url = Path.Combine("/ProjectContent/SubOrdinate/ProblemDocs", data.Attchment_Url);
                }
                else if (string.IsNullOrEmpty(data.Attchment_Url))
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 404,
                        message = "Employee image is not found. Try with employee image profile."
                    });
                }

                bool status = _subOrdinate.InsertSubOrdinateProblem(data);
                if (status)
                {
                    if (file != null && file.FileName != "")
                    {
                        file.SaveAs(HttpContext.Current.Server.MapPath(data.Attchment_Url));
                    }
                }
                if (status)
                {
                    return Ok(new
                    {
                        status = true,
                        message = "Data Added successfully !"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = false,
                        message = "Failed To added Data!"
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
        #endregion 


        private IHttpActionResult BadRequest(object value)
        {
            return Content(HttpStatusCode.BadRequest, value);
        }
        [HttpGet]
        [Route("api/getDashboardCount")]
        public IHttpActionResult DashbaordCount(int subId)
        {
            try
            {
                var data = _subOrdinate.GetDashboardCounts(subId.ToString());
                    return Ok(new
                    {
                        status = true,
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

    }
}
