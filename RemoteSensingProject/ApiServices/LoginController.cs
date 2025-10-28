using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using RemoteSensingProject.Models.LoginManager;
using static RemoteSensingProject.Models.LoginManager.main;

namespace RemoteSensingProject.ApiServices
{
    public class LoginController : ApiController
    {
        private readonly LoginServices _loginService;
        public LoginController()
        {
            _loginService = new LoginServices();
        }
        [HttpPost]
        [Route("api/login")]
        public IHttpActionResult Login()
        {
            try
            {

                var request = HttpContext.Current.Request;
                string username = request.Form.Get("username");
                string password = request.Form.Get("password");
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 400,
                        Message = "Username and password are required."
                    });
                }
                Credentials data = _loginService.Login(username, password);

                if (username.Equals(data.username) && password.Equals(data.password))
                {
                    string token = _loginService.GenerateToken(data);
                    var userData = new
                    {
                        username = data.username,
                        Emp_Id = data.Emp_Id,
                        Emp_Name = data.Emp_Name,
                        profilePath = data.profilePath,
                    };
                    return Ok(new
                    {
                        status = true,
                        StatusCode = 200,
                        data = userData,
                        token = token
                    });
                }
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 400,
                    message = "Invalid userid or password."
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
            return Content(HttpStatusCode.BadRequest, value);
        }
    }
}
