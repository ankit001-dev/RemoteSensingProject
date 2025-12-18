using RemoteSensingProject.Models;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.MailService;
using System;
using System.Runtime.Caching;
using System.Web.Mvc;
using System.Web.Security;
using static RemoteSensingProject.Models.LoginManager.main;

namespace RemoteSensingProject.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginServices _loginServices;
        private ObjectCache _cache = MemoryCache.Default;
        private mail _mail; 
        public LoginController()
        {
            _loginServices = new LoginServices();
            _mail = new mail();
        }
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult AuthoriseLogin(string username, string password)
        {
            Credentials cr = _loginServices.Login(username, password);

            if(!string.IsNullOrEmpty(cr.username)&& !string.IsNullOrEmpty(cr.password) &&cr.username.Equals(username) && cr.password.Equals(password))
            {
                string url = "";
                switch (cr.role)
                {
                    case string role when role.Equals("admin"):
                        url = "/admin/Dashboard";
                        break;
                    case string role when role.Equals("projectManager"):
                        url = "/employee/dashboard";
                        break; 
                    case string role when role.Equals("accounts"):
                        url = "/accounts/dashboard";
                        break;
                    case string role when role.Equals("subOrdinate"):
                        url = "/SubOrdinate/dashboard";
                        break;
                }

                FormsAuthentication.SetAuthCookie(username, false);
                return Json(new
                {
                    status = true,
                    message = "Authorised successfully !",
                    url = url
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    status = false,
                    message = "Invalid user !"
                }, JsonRequestBehavior.AllowGet);
            }
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
        public ActionResult SendOtp(Credentials userData)
        {
            try {
                var otp = new Random().Next(100000, 999999).ToString();

                var emp = _loginServices.ValidateUserFromEmail(userData.Email);

                if (emp.userId <= 0)
                {
                    return Json(new
                    {
                        status = false,
                        message = "User does not exist."
                    });
                }
                bool flag = _mail.SendOtpMail(userData.Email, otp);

                if (!flag)
                {
                    return Json(new
                    {
                        status = false,
                        message = "Error occured while sending mail. Try again later!"
                    });
                }

                string cacheKey = "otp_" + userData.Email;
                CacheItemPolicy policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10)
                };
                _cache.Set(cacheKey, otp, policy);
                return Json(new
                {
                    status = true,
                    message = "OTP sent successfully !",
                    otp = otp
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new
                {
                    status = false,
                    message = "Server error occured"
                });
            }
        }

        [HttpPost]
        public ActionResult VerifyOtp(string email, string otp)
        {
            try { 
                string cacheKey = "otp_" + email;
                var cacheOtp = _cache.Get(cacheKey) as string;
                if (cacheOtp == null)
                {
                    return Json(new
                    {
                        status = false,
                        message = "OTP expired or not found"
                    });
                }
                if (cacheOtp == otp)
                {
                    _cache.Remove(cacheKey);
                    return Json(new
                    {
                        status = true,
                        message = "OTP verified successfully"
                    });
                }
                return Json(new
                {
                    status = false,
                    message = "Invalid OTP"
                });
            }
            catch
            {
                return Json(new
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
        public ActionResult ResetPassword(Credentials userdata)
        {
            try
            {
                //userdata.Email = User.Identity.Name;
                if (userdata.newPassword.Equals(userdata.confirmPassword))
                {
                    var res = _loginServices.ChangePassword(userdata);
                    if (res)
                    {
                        bool flag = _mail.SendPasswordChangedMail(userdata.Email, userdata.newPassword);

                        if (!flag)
                        {
                            return Json(new
                            {
                                status = false,
                                message = "Error occured while sending mail. Try again later!"
                            });
                        }
                    }
                    return Json(new
                    {
                        status = res,
                        message = "Password changed successfully."
                    });
                }
                else
                {
                    return Json(new
                    {
                        status = false,
                        message = "Password not matched"
                    });
                }
            }
            catch
            {
                return Json(new
                {
                    status = false,
                    message = "Server error occured"
                });
            }
        }
        [HttpPut]
        public ActionResult ChangePassword(Credentials userdata)
        {
            try
            {
                userdata.Email = User.Identity.Name;
                if (userdata.newPassword.Equals(userdata.confirmPassword))
                {
                    bool ch = _loginServices.ValidateUserFromEmailPassword(userdata.Email, userdata.oldPassword);
                    var res = _loginServices.ChangePassword(userdata);
                    if (res)
                    {
                        bool flag = _mail.SendPasswordChangedMail(userdata.Email, userdata.newPassword);

                        if (!flag)
                        {
                            return Json(new
                            {
                                status = false,
                                message = "Error occured while sending mail. Try again later!"
                            });
                        }
                    }
                    return Json(new
                    {
                        status = res,
                        message = "Password changed successfully."
                    });
                }
                else
                {
                    return Json(new
                    {
                        status = false,
                        message = "Password not matched"
                    });
                }
            }
            catch
            {
                return Json(new
                {
                    status = false,
                    message = "Server error occured"
                });
            }
        }

        #region exportHtmlToPdf

        [HttpPost]
        public ActionResult ExportHtmlToPdf([System.Web.Http.FromBody] PdfRequest request)
        {
            DataFactory df = new DataFactory();
            byte[] pdfFile = df.ExportPdfData(request.HtmlText);

            string fileName = $"{DateTime.Now:ddMMyyyy}{Guid.NewGuid()}.pdf";

            Response.Headers["Content-Disposition"] = $"attachment; filename={fileName}";

            return File(pdfFile, "application/pdf", fileName);
        }


        public class PdfRequest
        {
            public string HtmlText { get; set; }
        }
        #endregion
    }
}