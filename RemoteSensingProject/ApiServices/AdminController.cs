using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using RemoteSensingProject.Controllers;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
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
            var request = HttpContext.Current.Request;
            string username = request.Form.Get("username");
            string password = request.Form.Get("password");
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 400,
                    Message = "Username and password are required."
                });
            }
            Credentials data = _loginService.Login(username, password);

            if(username.Equals(data.username) && password.Equals(data.password))
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


        }

        private IHttpActionResult BadRequest(object value)
        {
            return Content(HttpStatusCode.BadRequest, value);
        }
    }
}
