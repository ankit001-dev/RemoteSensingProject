using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static RemoteSensingProject.Models.LoginManager.main;
using RemoteSensingProject.Models.LoginManager;
using System.Web.Security;

namespace RemoteSensingProject.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginServices _loginServices;
        public LoginController()
        {
            _loginServices = new LoginServices();   
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
    }
}