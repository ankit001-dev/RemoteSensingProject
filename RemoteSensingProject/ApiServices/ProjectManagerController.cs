using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;

namespace RemoteSensingProject.ApiServices
{
    public class ProjectManagerController : ApiController
    {
        private readonly AdminServices _adminServices;
        private readonly LoginServices _loginService;
        public ProjectManagerController()
        {
            _adminServices = new AdminServices();
            _loginService = new LoginServices();
        }
    }
}
