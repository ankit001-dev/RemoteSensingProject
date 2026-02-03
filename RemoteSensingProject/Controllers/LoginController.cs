// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Controllers.LoginController
using System;
using System.Runtime.Caching;
using System.Web.Mvc;
using System.Web.Security;
using RemoteSensingProject.Controllers;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.MailService;

namespace RemoteSensingProject.Controllers
{
	public class LoginController : Controller
	{
		public class PdfRequest
		{
			public string HtmlText { get; set; }
		}

		private readonly LoginServices _loginServices;

		private ObjectCache _cache = MemoryCache.Default;

		private mail _mail;

		public LoginController()
		{
			_loginServices = new LoginServices();
			_mail = new mail();
		}

		public ActionResult Login()
		{
			return View();
		}

		public ActionResult AuthoriseLogin(string username, string password)
		{
			main.Credentials cr = _loginServices.Login(username, password);
			if (!string.IsNullOrEmpty(cr.username) && !string.IsNullOrEmpty(cr.password) && cr.username.Equals(username) && cr.password.Equals(password))
			{
				string url = "";
				string role = cr.role;
				string role2 = role;
				if (role2 != null)
				{
					if (role2.Equals("admin"))
					{
						url = "/admin/Dashboard";
					}
					else
					{
						string role3 = role2;
						if (role3.Equals("projectManager"))
						{
							url = "/employee/dashboard";
						}
						else
						{
							string role4 = role2;
							if (role4.Equals("accounts"))
							{
								url = "/accounts/dashboard";
							}
							else
							{
								string role5 = role2;
								if (role5.Equals("subOrdinate"))
								{
									url = "/SubOrdinate/dashboard";
								}
							}
						}
					}
				}
				FormsAuthentication.SetAuthCookie(username, createPersistentCookie: false);
				return Json((object)new
				{
					status = true,
					message = "Authorised successfully !",
					url = url
				}, (JsonRequestBehavior)0);
			}
			return Json((object)new
			{
				status = false,
				message = "Invalid user !"
			}, (JsonRequestBehavior)0);
		}

		public ActionResult Logout()
		{
			FormsAuthentication.SignOut();
			return RedirectToAction("login");
		}

		public ActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		public ActionResult SendOtp(main.Credentials userData)
		{
			try
			{
				string otp = new Random().Next(100000, 999999).ToString();
				main.Credentials emp = _loginServices.ValidateUserFromEmail(userData.Email);
				if (emp.userId <= 0)
				{
					return Json((object)new
					{
						status = false,
						message = "User does not exist."
					});
				}
				if (!_mail.SendOtpMail(userData.Email, otp))
				{
					return Json((object)new
					{
						status = false,
						message = "Error occured while sending mail. Try again later!"
					});
				}
				string cacheKey = "otp_" + userData.Email;
				CacheItemPolicy policy = new CacheItemPolicy
				{
					AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10.0)
				};
				_cache.Set(cacheKey, otp, policy);
				return Json((object)new
				{
					status = true,
					message = "OTP sent successfully !",
					otp = otp
				}, (JsonRequestBehavior)0);
			}
			catch
			{
				return Json((object)new
				{
					status = false,
					message = "Server error occured"
				});
			}
		}

		[HttpPost]
		public ActionResult VerifyOtp(string email, string otp)
		{
			try
			{
				string cacheKey = "otp_" + email;
				if (!(_cache.Get(cacheKey) is string cacheOtp))
				{
					return Json((object)new
					{
						status = false,
						message = "OTP expired or not found"
					});
				}
				if (cacheOtp == otp)
				{
					string verifiedKey = "otp_verified_" + email;
					_cache.Set(verifiedKey, true, new CacheItemPolicy
					{
						AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10.0)
					});
					return Json((object)new
					{
						status = true,
						message = "OTP verified successfully"
					});
				}
				return Json((object)new
				{
					status = false,
					message = "Invalid OTP"
				});
			}
			catch
			{
				return Json((object)new
				{
					status = false,
					message = "Server error occured"
				});
			}
		}

		public ActionResult ChangePassword()
		{
			return View();
		}

		[HttpPut]
		public ActionResult ResetPassword(main.Credentials userdata)
		{
			try
			{
				if (!userdata.newPassword.Equals(userdata.confirmPassword))
				{
					return Json((object)new
					{
						status = false,
						message = "Password not matched"
					});
				}
				string verifiedKey = "otp_verified_" + userdata.Email;
				if (_cache.Get(verifiedKey) as bool? != true)
				{
					return Json((object)new
					{
						status = false,
						message = "OTP not verified or expired"
					});
				}
				bool res = _loginServices.ChangePassword(userdata);
				if (res)
				{
					if (!_mail.SendPasswordChangedMail(userdata.Email, userdata.newPassword))
					{
						return Json((object)new
						{
							status = false,
							message = "Error occurred while sending mail. Try again later!"
						});
					}
					_cache.Remove("otp_" + userdata.Email);
					_cache.Remove(verifiedKey);
				}
				return Json((object)new
				{
					status = res,
					message = "Password changed successfully."
				});
			}
			catch
			{
				return Json((object)new
				{
					status = false,
					message = "Server error occured"
				});
			}
		}

		[HttpPut]
		public ActionResult ChangePassword(main.Credentials userdata)
		{
			try
			{
				userdata.Email = ((Controller)this).User.Identity.Name;
				if (userdata.newPassword.Equals(userdata.confirmPassword))
				{
					bool ch = _loginServices.ValidateUserFromEmailPassword(userdata.Email, userdata.oldPassword);
					bool res = _loginServices.ChangePassword(userdata);
					if (res && !_mail.SendPasswordChangedMail(userdata.Email, userdata.newPassword))
					{
						return Json((object)new
						{
							status = false,
							message = "Error occured while sending mail. Try again later!"
						});
					}
					return Json((object)new
					{
						status = res,
						message = "Password changed successfully."
					});
				}
				return Json((object)new
				{
					status = false,
					message = "Password not matched"
				});
			}
			catch
			{
				return Json((object)new
				{
					status = false,
					message = "Server error occured"
				});
			}
		}

		[HttpPost]
		public ActionResult ExportHtmlToPdf(PdfRequest request)
		{
			DataFactory df = new DataFactory();
			byte[] pdfFile = df.ExportPdfData(request.HtmlText);
			string fileName = $"{DateTime.Now:ddMMyyyy}{Guid.NewGuid()}.pdf";
			((Controller)this).Response.Headers["Content-Disposition"] = "attachment; filename=" + fileName;
			return File(pdfFile, "application/pdf", fileName);
		}
	}
}