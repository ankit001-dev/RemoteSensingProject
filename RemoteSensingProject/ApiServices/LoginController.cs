using System;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.MailService;

namespace RemoteSensingProject.ApiServices
{
	[JwtAuthorize]
	public class LoginController : ApiController
	{
		private readonly LoginServices _loginService;

		private readonly AdminServices _adminServices;

		private readonly ObjectCache _cache = MemoryCache.Default;

		private readonly mail _mail;

		private readonly JwtAuthorizeAttribute authgaurd;

		public LoginController()
		{
			_loginService = new LoginServices();
			_adminServices = new AdminServices();
			_mail = new mail();
			authgaurd = new JwtAuthorizeAttribute();
		}

		[System.Web.Mvc.AllowAnonymous]
		[HttpPost]
		[Route("api/login")]
		public IHttpActionResult Login()
		{
			try
			{
				HttpRequest request = HttpContext.Current.Request;
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
				RemoteSensingProject.Models.LoginManager.main.Credentials data = _loginService.Login(username, password);
				if (username.Equals(data.username) && password.Equals(data.password))
				{
					string token = _loginService.GenerateToken(data);
					var userData = new { data.username, data.Emp_Id, data.Emp_Name, data.profilePath, data.role, data.userId };
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

		[System.Web.Mvc.AllowAnonymous]
		[HttpGet]
		[Route("api/refresh-token")]
		public IHttpActionResult RefreshToken(string token)
		{
			ClaimsPrincipal principal;
			switch (authgaurd.ValidateTokenIgnoreExpiry(token, out principal))
			{
				case 0:
					return Ok(new
					{
						status = "invalid"
					});
				case 2:
					return Ok(new
					{
						status = "stillvalid"
					});
				default:
					{
                        ClaimsIdentity identity = principal.Identity as ClaimsIdentity;

                        RemoteSensingProject.Models.LoginManager.main.Credentials cred =
                            new RemoteSensingProject.Models.LoginManager.main.Credentials
                            {
                                role = identity.Claims
                                           .Where(c => c.Type == ClaimTypes.Role)
                                           .Select(c => c.Value)
                                           .ToArray(),
                                username = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value,

                                userId = int.Parse(identity.FindFirst("userId")?.Value ?? "0")
                            };
                        string newToken = _loginService.GenerateToken(cred);
						return Ok(new
						{
							status = "newtoken",
							token = newToken
						});
					}
			}
		}

		private IHttpActionResult BadRequest(object value)
		{
			return Content<object>(HttpStatusCode.BadRequest, value);
		}

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

		[System.Web.Mvc.AllowAnonymous]
		[HttpPost]
		[Route("api/send-otp")]
		public IHttpActionResult SendOtp([FromBody] RemoteSensingProject.Models.LoginManager.main.Credentials userData)
		{
			try
			{
				string otp = new Random().Next(100000, 999999).ToString();
				RemoteSensingProject.Models.LoginManager.main.Credentials emp = _loginService.ValidateUserFromEmail(userData.Email);
				if (emp.userId <= 0)
				{
					return BadRequest("User does not exist.");
				}
				if (!_mail.SendOtpMail(userData.Email, otp))
				{
					return InternalServerError(new Exception("Error occurred while sending mail."));
				}
				string cacheKey = "otp_" + userData.Email;
				CacheItemPolicy policy = new CacheItemPolicy
				{
					AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10.0)
				};
				_cache.Set(cacheKey, otp, policy);
				return Ok(new
				{
					status = true,
					message = "OTP sent successfully!"
				});
			}
			catch (Exception ex)
			{
				return InternalServerError(ex);
			}
		}

		[System.Web.Mvc.AllowAnonymous]
		[HttpPost]
		[Route("api/verify-otp")]
		public IHttpActionResult VerifyOtp([FromBody] RemoteSensingProject.Models.LoginManager.main.VerifyOtpRequest model)
		{
			try
			{
				string cacheKey = "otp_" + model.Email;
				if (!(MemoryCache.Default.Get(cacheKey) is string cacheOtp))
				{
					return BadRequest("OTP expired or not found.");
				}
				if (cacheOtp != model.Otp)
				{
					return BadRequest("Invalid OTP.");
				}
				MemoryCache.Default.Set("otp_verified_" + model.Email, true, DateTimeOffset.Now.AddMinutes(10.0));
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

		[System.Web.Mvc.AllowAnonymous]
		[HttpPost]
		[Route("api/reset-password")]
		public IHttpActionResult ResetPassword([FromBody] RemoteSensingProject.Models.LoginManager.main.Credentials userdata)
		{
			try
			{
				if (!userdata.newPassword.Equals(userdata.confirmPassword))
				{
					return BadRequest("Password not matched.");
				}
				string cacheKey = "otp_" + userdata.Email;
				if (!(MemoryCache.Default.Get(cacheKey) is string cachedOtp))
				{
					return BadRequest("OTP expired or not found.");
				}
				if (cachedOtp != userdata.otp)
				{
					return BadRequest("Invalid OTP.");
				}
				MemoryCache.Default.Remove(cacheKey);
				bool res = _loginService.ChangePassword(userdata);
				if (res && !_mail.SendPasswordChangedMail(userdata.Email, userdata.newPassword))
				{
					return InternalServerError(new Exception("Error occurred while sending mail."));
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

		[RoleAuthorize("projectManager,subOrdinate,outSource,account")]
		[HttpPost]
		[Route("api/change-password")]
		public IHttpActionResult ChangePassword([FromBody] RemoteSensingProject.Models.LoginManager.main.Credentials userdata)
		{
			try
			{
				userdata.Email = userdata.Email;
				if (!userdata.newPassword.Equals(userdata.confirmPassword))
				{
					return BadRequest("Password not matched.");
				}
				if (!_loginService.ValidateUserFromEmailPassword(userdata.Email, userdata.oldPassword))
				{
					return BadRequest("Invalid old password.");
				}
				bool res = _loginService.ChangePassword(userdata);
				if (res && !_mail.SendPasswordChangedMail(userdata.Email, userdata.newPassword))
				{
					return InternalServerError(new Exception("Error occurred while sending mail."));
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
	}
}