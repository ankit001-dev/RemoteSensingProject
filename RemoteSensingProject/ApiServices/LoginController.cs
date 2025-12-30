using Newtonsoft.Json;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.MailService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Runtime.Caching;
using System.Web.Http;
using static RemoteSensingProject.Models.Admin.main;
using static RemoteSensingProject.Models.LoginManager.main;

namespace RemoteSensingProject.ApiServices
{
    [JwtAuthorize]
    public class LoginController : ApiController
    {
        private readonly LoginServices _loginService;
        private readonly AdminServices _adminServices;
        private readonly ObjectCache _cache = MemoryCache.Default;
        private readonly mail _mail;
        public LoginController()
        {
            _loginService = new LoginServices();
            _adminServices = new AdminServices();
            _mail = new mail();
        }
        #region Login Api
        [System.Web.Mvc.AllowAnonymous]
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
                        role = data.role,
                        userId = data.userId
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
        #endregion

        [RoleAuthorize("admin,projectManager")]
        [HttpGet]
        [Route("api/getresult")]
        public IHttpActionResult GetResult()
        {
            try
            {
                return Ok(new
                {
                    status = true,
                    StatusCode = 200,
                    Message = "Welcome to Api"
                });
            }
            catch
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    Message = "baba ji ka thullu"
                });
            }
        }

        #region Forgot Password
        // ================= SEND OTP =================
        [System.Web.Mvc.AllowAnonymous]
        [HttpPost]
        [Route("api/send-otp")]
        public IHttpActionResult SendOtp([FromBody] Credentials userData)
        {
            try
            {
                var otp = new Random().Next(100000, 999999).ToString();

                var emp = _loginService.ValidateUserFromEmail(userData.Email);
                if (emp.userId <= 0)
                {
                    return BadRequest("User does not exist.");
                }

                bool flag = _mail.SendOtpMail(userData.Email, otp);
                if (!flag)
                {
                    return InternalServerError(new Exception("Error occurred while sending mail."));
                }

                string cacheKey = "otp_" + userData.Email;
                CacheItemPolicy policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10)
                };
                _cache.Set(cacheKey, otp, policy);

                return Ok(new
                {
                    status = true,
                    message = "OTP sent successfully!"
                    // ❌ Do NOT return OTP in real applications
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ================= VERIFY OTP =================
        [System.Web.Mvc.AllowAnonymous]
        [HttpPost]
        [Route("api/verify-otp")]
        public IHttpActionResult VerifyOtp([FromBody] VerifyOtpRequest model)
        {
            try
            {
                string cacheKey = "otp_" + model.Email;
                var cacheOtp = MemoryCache.Default.Get(cacheKey) as string;

                if (cacheOtp == null)
                {
                    return BadRequest("OTP expired or not found.");
                }

                if (cacheOtp != model.Otp)
                {
                    return BadRequest("Invalid OTP.");
                }

                MemoryCache.Default.Set(
                    "otp_verified_" + model.Email,
                    true,
                    DateTimeOffset.Now.AddMinutes(10)
                );

                return Ok(new
                {
                    status = true,
                    message = "OTP verified successfully"
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ================= RESET PASSWORD =================
        [System.Web.Mvc.AllowAnonymous]
        [HttpPost]
        [Route("api/reset-password")]
        public IHttpActionResult ResetPassword([FromBody] Credentials userdata)
        {
            try
            {
                if (!userdata.newPassword.Equals(userdata.confirmPassword))
                {
                    return BadRequest("Password not matched.");
                }
                string cacheKey = "otp_" + userdata.Email;

                // 3️⃣ Get OTP from cache
                var cachedOtp = MemoryCache.Default.Get(cacheKey) as string;

                if (cachedOtp == null)
                {
                    return BadRequest("OTP expired or not found.");
                }
                if (cachedOtp != userdata.otp)
                {
                    return BadRequest("Invalid OTP.");
                }
                MemoryCache.Default.Remove(cacheKey);
                var res = _loginService.ChangePassword(userdata);
                if (res)
                {
                    bool flag = _mail.SendPasswordChangedMail(userdata.Email, userdata.newPassword);
                    if (!flag)
                    {
                        return InternalServerError(new Exception("Error occurred while sending mail."));
                    }
                }

                return Ok(new
                {
                    status = res,
                    message = "Password changed successfully."
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // ================= CHANGE PASSWORD =================
        [System.Web.Mvc.AllowAnonymous]
        [HttpPost]
        [Route("api/change-password")]
        public IHttpActionResult ChangePassword([FromBody] Credentials userdata)
        {
            try
            {
                userdata.Email = User.Identity.Name;

                if (!userdata.newPassword.Equals(userdata.confirmPassword))
                {
                    return BadRequest("Password not matched.");
                }

                bool validUser = _loginService
                    .ValidateUserFromEmailPassword(userdata.Email, userdata.oldPassword);

                if (!validUser)
                {
                    return BadRequest("Invalid old password.");
                }

                var res = _loginService.ChangePassword(userdata);
                if (res)
                {
                    bool flag = _mail.SendPasswordChangedMail(userdata.Email, userdata.newPassword);
                    if (!flag)
                    {
                        return InternalServerError(new Exception("Error occurred while sending mail."));
                    }
                }

                return Ok(new
                {
                    status = res,
                    message = "Password changed successfully."
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion
    }
}
