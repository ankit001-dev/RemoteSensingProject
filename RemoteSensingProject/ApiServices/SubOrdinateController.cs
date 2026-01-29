// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.ApiServices.SubOrdinateController
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.ProjectManager;
using RemoteSensingProject.Models.SubOrdinate;

namespace RemoteSensingProject.ApiServices
{
	[JwtAuthorize(Roles = "subOrdinate")]
	public class SubOrdinateController : ApiController
	{
		private readonly AdminServices _adminServices;

		private readonly LoginServices _loginService;

		private readonly ManagerService _managerService;

		private readonly SubOrinateService _subOrdinate;

		public SubOrdinateController()
		{
			_subOrdinate = new SubOrinateService();
			_adminServices = new AdminServices();
			_loginService = new LoginServices();
			_managerService = new ManagerService();
		}

		[HttpGet]
		[Route("api/subAssignedProject")]
		public IHttpActionResult assignedProject(int subId, int? page = null, int? limit = null, string searchTerm = null, string statusFilter = null)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.Project_model> data = _managerService.All_Project_List(0, limit, page, "SubordinateProject", subId, searchTerm, statusFilter);
				return Ok(new
				{
					status = data.Any(),
					StatusCode = (data.Any() ? 200 : 500),
					message = (data.Any() ? "Data found !" : "Data not found !"),
					data = data
				});
			}
			catch (Exception ex)
			{
				return BadRequest(new
				{
					status = false,
					StatusCode = 500,
					message = ex.Message
				});
			}
		}

		[HttpPost]
		[Route("api/SubOrdinateRaiseProblem")]
		public IHttpActionResult Raise_Problem()
		{
			try
			{
				HttpRequest request = HttpContext.Current.Request;
				RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem data = new RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem
				{
					Project_Id = Convert.ToInt32(request.Form.Get("Project_Id")),
					Title = request.Form.Get("title"),
					Description = request.Form.Get("Description")
				};
				HttpPostedFile file = request.Files["Attachment"];
				if (file != null && file.FileName != "")
				{
					data.Attchment_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					data.Attchment_Url = Path.Combine("/ProjectContent/SubOrdinate/ProblemDocs", data.Attchment_Url);
					bool status = _subOrdinate.InsertSubOrdinateProblem(data);
					if (status && file != null && file.FileName != "")
					{
						string fullPath = HttpContext.Current.Server.MapPath(data.Attchment_Url);
						string directoryPath = Path.GetDirectoryName(fullPath);
						if (!Directory.Exists(directoryPath))
						{
							Directory.CreateDirectory(directoryPath);
						}
						file.SaveAs(fullPath);
					}
					if (status)
					{
						return Ok(new
						{
							status = true,
							message = "Data Added successfully !"
						});
					}
					return Ok(new
					{
						status = false,
						message = "Failed To added Data!"
					});
				}
				return BadRequest(new
				{
					status = false,
					StatusCode = 404,
					message = "Employee image is not found. Try with employee image profile."
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
			return Content<object>(HttpStatusCode.BadRequest, value);
		}

		[HttpGet]
		[Route("api/getDashboardCount")]
		public IHttpActionResult DashbaordCount(int subId)
		{
			try
			{
				RemoteSensingProject.Models.SubOrdinate.main.DashboardCount data = _subOrdinate.GetDashboardCounts(subId);
				return Ok(new
				{
					status = true,
					data = data
				});
			}
			catch (Exception ex)
			{
				return BadRequest(new
				{
					status = false,
					StatusCode = 500,
					message = ex.Message
				});
			}
		}
	}
}