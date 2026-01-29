// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Controllers.HomeController
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RemoteSensingProject.Controllers;

namespace RemoteSensingProject.Controllers
{
	public class HomeController : Controller
	{
		public class usersModel
		{
			public string name { get; set; }

			public string type { get; set; }

			public string email { get; set; }

			public string passsword { get; set; }

			public string mobileno { get; set; }

			public string active { get; set; }
		}

		public class common_response
		{
			public string message { get; set; }

			public string parameter { get; set; }

			public bool success { get; set; }

			public string username { get; set; }

			public string useremail { get; set; }

			public string usertype { get; set; }

			public string employee_branch { get; set; }

			public string employeeId { get; set; }

			public string id { get; set; }
		}

		public ActionResult Login()
		{
			return View();
		}

		public ActionResult ContactUs()
		{
			return View();
		}

		public ActionResult Privacy_Policy()
		{
			Response.Cache.SetCacheability(HttpCacheability.Public);
			Response.Cache.SetMaxAge(TimeSpan.FromDays(1.0));
			return View();
		}

		[Authorize]
		public ActionResult Help_menu()
		{
			return View();
		}

		[Authorize]
		public ActionResult Support()
		{
			return View();
		}

		[HttpPost]
		public JsonResult admin_Login(string username, string password, string url)
		{
			common_response Response = new common_response();
			if (username == null || username == "")
			{
				Response.message = "Invalid Username.";
				return Json((object)Response);
			}
			if (password == null || password == "")
			{
				Response.message = "Invalid password (must include atleast 8 charaters,uppercase and lowercase alphabhet, one number and one special charater).";
				return Json((object)Response);
			}
			List<usersModel> userList = usersdata();
			List<usersModel> filteredList = userList.Where((usersModel usersModel2) => usersModel2.email == username && usersModel2.passsword == password).ToList();
			if (filteredList.Count < 1)
			{
				Response.message = "Invalid username or password!.";
				return Json((object)Response);
			}
			if (filteredList.Count > 0)
			{
				Response.success = true;
				Response.parameter = username;
				foreach (usersModel user in filteredList)
				{
					Response.usertype = user.type;
					Response.username = user.name;
					Response.useremail = user.email;
				}
			}
			if (Response.success)
			{
				if (url != null && url.ToString() != "")
				{
					Response.message = HttpUtility.HtmlDecode(url);
				}
				else
				{
					Session["usertype"] = Response.usertype.ToString();
					Session["username"] = Response.username.ToString();
					if (Response.usertype == "Admin")
					{
						Response.message = "/Admin/Dashboard";
					}
					else if (Response.usertype == "ProjectManager")
					{
						Response.message = "/Employee/Dashboard";
					}
					else if (Response.usertype == "SubCoordinate")
					{
						Response.message = "/SubCoordinate/Dashboard";
					}
				}
				Session["adminname"] = Response.parameter.ToString();
			}
			return Json((object)Response);
		}

		public List<usersModel> usersdata()
		{
			List<usersModel> userlist = new List<usersModel>();
			usersModel admin = new usersModel
			{
				name = "Admin",
				type = "Admin",
				email = "Admin",
				passsword = "123",
				mobileno = "123",
				active = "1"
			};
			usersModel ProjectManager = new usersModel
			{
				name = "Project Manager",
				type = "ProjectManager",
				email = "ProjectManager",
				passsword = "123",
				mobileno = "123",
				active = "1"
			};
			usersModel SubCoordinate = new usersModel
			{
				name = "Sub Co-ordinate",
				type = "SubCoordinate",
				email = "Employee",
				passsword = "123",
				mobileno = "123",
				active = "1"
			};
			userlist.Add(admin);
			userlist.Add(ProjectManager);
			userlist.Add(SubCoordinate);
			return userlist;
		}
	}
}