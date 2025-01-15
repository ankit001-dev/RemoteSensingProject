using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RemoteSensingProject.Models.LoginManager
{
    public class main
    {
        public class Credentials
        {
            public string username { get; set; }
            public string password { get; set; }
            public string Emp_Name { get; set; }
            public string role { get; set; }
            public string profilePath { get; set; }
            public int userId { get; set; }     
        }
    }   
}