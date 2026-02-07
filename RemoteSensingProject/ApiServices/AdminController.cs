// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.ApiServices.AdminController
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using Antlr.Runtime.Tree;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.ProjectManager;
using RemoteSensingProject.Models.SubOrdinate;

namespace RemoteSensingProject.ApiServices
{
	[JwtAuthorize(Roles = "admin,accounts")]
	public class AdminController : ApiController
	{
		private readonly AdminServices _adminServices;

		private readonly ManagerService _managerservice;

		private readonly LoginServices _loginService;

		private readonly AccountService _accountService;

		public AdminController()
		{
			_adminServices = new AdminServices();
			_loginService = new LoginServices();
			_managerservice = new ManagerService();
			_accountService = new AccountService();
		}

		private string getRole()
		{
			ClaimsIdentity identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
			string role = identity.Claims.FirstOrDefault((Claim c) => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" || c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
			return string.IsNullOrEmpty(role) ? "Role claim not found in token" : role;
		}

		[HttpGet]
		[Route("api/ViewExpenditureAmtData")]
		public IHttpActionResult ViewExpendedAmt(int? limit = null, int? page = null)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.ProjectExpenditure> data = _adminServices.ViewProjectExpenditure(limit, page);
				if (data.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, data, "Data fetched successfully", 200, data[0].Pagination);
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		#region Manage Division
		[HttpPost]
		[Route("api/add-division")]
		public IHttpActionResult AddDivision([FromBody] Models.Admin.main.CommonResponse cr)
		{
			try
			{
				if (string.IsNullOrEmpty(cr.name))
					return Content(HttpStatusCode.BadRequest, new
					{
						status = false,
						StatusCode = 400,
						message = "Division name is required"
					});
				bool res = _adminServices.InsertDivison(cr);
				return Ok(new
				{
					status = res,
					StatusCode = res ? 201 : 400,
					message = res ? cr.id>0?"Updated Successfully": "Added Successfully" : "Some error occured"
				});
			}
			catch(Exception ex)
			{
				return BadRequest(new
				{
					status = false,
					StatusCode = 400,
					message = ex.Message
				});
			}
		}

		[HttpGet]
		[Route("api/get-division")]
		public IHttpActionResult GetAllDivision()
		{
			try
			{
				var data = _adminServices.ListDivison();
				return Ok(new
				{
					status = data.Any(),
					data = data
				});
			}
			catch(Exception ex)
			{
				return BadRequest(new
				{
					status = false,
					StatusCode = 400,
					message = ex.Message
				});
			}
		}
		[HttpGet]
		[Route("api/remove-division")]
		public IHttpActionResult RemoveDivision(int id)
		{
			try
			{
				bool res = _adminServices.removeDivison(id);
				return Ok(new
				{
					status = res,
					StatusCode = res ? 200 : 400,
					message = res ? "Deleted Successfully" : "Something went wrong"
				});
			}
			catch(Exception ex)
			{
				return BadRequest(new
				{
					status = false,
					StatusCode = 400,
					message = ex.Message
				});
			}
		}
        #endregion

        [HttpPost]
		[Route("api/EmployeeRegistration")]
		public IHttpActionResult Emp_Register()
		{
			try
			{
				HttpRequest request = HttpContext.Current.Request;
				RemoteSensingProject.Models.Admin.main.Employee_model empData = new RemoteSensingProject.Models.Admin.main.Employee_model
				{
					Id = Convert.ToInt32(request.Form.Get("Id")),
					EmployeeCode = request.Form.Get("EmployeeCode"),
					EmployeeName = request.Form.Get("EmployeeName"),
					MobileNo = Convert.ToInt64(request.Form.Get("MobileNo")),
					Email = request.Form.Get("Email"),
					Division = Convert.ToInt32(request.Form.Get("Division")),
					Designation = Convert.ToInt32(request.Form.Get("Designation")),
					Gender = request.Form.Get("Gender"),
					Image_url = request.Form.Get("Image_url")
				};
				string[] empRole = request.Form.GetValues("EmployeeRole") ?? Array.Empty<string>();
				HttpPostedFile file = request.Files["EmployeeImages"];
				if (file != null && file.FileName != "")
				{
					empData.Image_url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					empData.Image_url = Path.Combine("/ProjectContent/Admin/Employee_Images/", empData.Image_url);
				}
				else if (string.IsNullOrEmpty(empData.Image_url))
				{
					return BadRequest(new
					{
						status = false,
						StatusCode = 404,
						message = "Employee image is not found. Try with employee image profile."
					});
				}
				List<string> validationErrors = new List<string>();
				if (string.IsNullOrWhiteSpace(empData.EmployeeCode))
				{
					validationErrors.Add("Employee Code is required.");
				}
				if (string.IsNullOrWhiteSpace(empData.EmployeeName))
				{
					validationErrors.Add("Employee Name is required.");
				}
				if (empData.MobileNo == 0L || empData.MobileNo.ToString().Length != 10)
				{
					validationErrors.Add("A valid 10-digit Mobile Number is required.");
				}
				if (string.IsNullOrWhiteSpace(empData.Email) || !empData.Email.Contains("@"))
				{
					validationErrors.Add("A valid Email address is required.");
				}
				if (empData.EmployeeRole == null||empData.EmployeeRole.Length == 0)
				{
					validationErrors.Add("Employee Role is required.");
				}
				if (empData.Division <= 0)
				{
					validationErrors.Add("Division must be selected.");
				}
				if (empData.Designation <= 0)
				{
					validationErrors.Add("Designation must be selected.");
				}
				if (string.IsNullOrWhiteSpace(empData.Gender) || (!empData.Gender.Equals("Male", StringComparison.OrdinalIgnoreCase) && !empData.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase) && !empData.Gender.Equals("Other", StringComparison.OrdinalIgnoreCase)))
				{
					validationErrors.Add("Gender must be Male, Female, or Other.");
				}
				if (validationErrors.Any())
				{
					return BadRequest(new
					{
						status = false,
						StatusCode = 500,
						message = string.Join("\n", validationErrors)
					});
				}
				string mess = null;
				bool res = _adminServices.AddEmployees(empData, out mess);
				if (res && file != null && file.FileName != "")
				{
					file.SaveAs(HttpContext.Current.Server.MapPath(empData.Image_url));
				}
				return Ok(new
				{
					status = res,
					StatusCode = (res ? 200 : 500),
					message = (res ? "Employee registration completed successfully !" : mess)
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

		[HttpGet]
		[Route("api/allEmployeeList")]
		public IHttpActionResult All_EmpList(int? page = null, int? limit = null, string searchTerm = null, int? devision = null)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.Employee_model> data = _adminServices.SelectEmployeeRecord(page, limit, searchTerm, devision);
				string[] selectProperties = new string[13]
				{
				"Id", "EmployeeCode", "EmployeeName", "DevisionName", "Email", "MobileNo", "EmployeeRole", "Division", "DesignationName", "Status",
				"ActiveStatus", "CreationDate", "Image_url"
				};
				List<object> filtered = CommonHelper.SelectProperties(data, selectProperties);
				if (data != null && data.Count > 0)
				{
					ApiCommon.PaginationInfo pagination = data[0].Pagination;
					return CommonHelper.Success((ApiController)(object)this, filtered, "Data fetched successfully.", 200, pagination);
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[HttpDelete]
		[Route("api/removeEmployee")]
		public IHttpActionResult RemoveEmployee(int Id)
		{
			try
			{
				if (Id <= 0)
				{
					return BadRequest(new
					{
						status = false,
						StatusCode = 500,
						message = "Invalid request id !"
					});
				}
				RemoteSensingProject.Models.Admin.main.Employee_model data = _adminServices.SelectEmployeeRecordById(Id);
				if (data.Id <= 0)
				{
					return BadRequest(new
					{
						status = false,
						StatusCode = 500,
						message = "Invalid request id !"
					});
				}
				bool res = _adminServices.RemoveEmployees(Id);
				return Ok(new
				{
					status = res,
					StatusCode = (res ? 200 : 500),
					message = (res ? "Selected employee removed successfully !" : "Some issue occred while processing your request.")
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

		[HttpGet]
		[Route("api/adminDashboard")]
		public IHttpActionResult adminDashboard()
		{
			try
			{
				RemoteSensingProject.Models.Admin.main.DashboardCount data = _adminServices.DashboardCount();
				if (data != null)
				{
					return Ok(new
					{
						status = true,
						StatusCode = 200,
						message = "Data Found !",
						data = data
					});
				}
				return Ok(new
				{
					status = false,
					StatusCode = 404,
					message = "Admin dashboard not found !"
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

		[HttpGet]
		[Route("api/adminProjectGraph")]
		public IHttpActionResult adminProjectGraph()
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.DashboardCount> data = _adminServices.getAllProjectCompletion();
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

		[HttpGet]
		[Route("api/adminDelayProject")]
		public IHttpActionResult DelayProject(int? page, int? limit)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.Project_model> data = _managerservice.All_Project_List(0, limit, page, "delay");
				string[] selectProperties = new string[23]
				{
				"Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl",
				"ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode",
				"ProjectDepartment", "ContactPerson", "Address"
				};
				List<object> newData = CommonHelper.SelectProperties(data, selectProperties);
				if (data.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, newData, "Data fetched successfully", 200, data[0].Pagination);
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[HttpGet]
		[Route("api/adminOngoingProject")]
		public IHttpActionResult ongoingProject(int? page, int? limit)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.Project_model> data = _managerservice.All_Project_List(0, limit, page, "Ongoing");
				string[] selectProperties = new string[23]
				{
				"Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl",
				"ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode",
				"ProjectDepartment", "ContactPerson", "Address"
				};
				List<object> newData = CommonHelper.SelectProperties(data, selectProperties);
				if (newData.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, newData, "Data fetched successfully", 200, data[0].Pagination);
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[HttpGet]
		[Route("api/adminCompleteProject")]
		public IHttpActionResult completeProject(int? page, int? limit)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.Project_model> data = _managerservice.All_Project_List(0, limit, page, "Complete");
				string[] selectProperties = new string[23]
				{
				"Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl",
				"ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode",
				"ProjectDepartment", "ContactPerson", "Address"
				};
				List<object> newData = CommonHelper.SelectProperties(data, selectProperties);
				if (newData.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, newData, "Data fetched successfully", 200, data[0].Pagination);
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[HttpGet]
		[Route("api/ViewProjectDetailById")]
		public IHttpActionResult ViewProjectDetailById(int projectId)
		{
			try
			{
				return Ok();
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

		[HttpGet]
		[Route("api/adminProjectList")]
		public IHttpActionResult getProjectList(int? page, int? limit, string searchTerm = null, string statusFilter = null, int? projectManagerFilter = null)
		{
			try
			{
				string[] selectProperties = new string[23]
				{
				"Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl",
				"ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode",
				"ProjectDepartment", "ContactPerson", "Address"
				};
				List<RemoteSensingProject.Models.Admin.main.Project_model> data = _managerservice.All_Project_List(projectManagerFilter, limit, page, null, null, searchTerm, statusFilter);
				List<object> filterData = CommonHelper.SelectProperties(data, selectProperties);
				if (data.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, filterData, "Data fetched successfully", 200, data[0].Pagination);
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[HttpPost]
		[Route("api/adminAddBudgets")]
		public IHttpActionResult AddBudgets()
		{
			try
			{
				HttpRequest request = HttpContext.Current.Request;
				List<string> validationErrors = new List<string>();
				if (string.IsNullOrWhiteSpace(request.Form.Get("Project_Id")))
				{
					validationErrors.Add("Project ID is required.");
				}
				if (string.IsNullOrWhiteSpace(request.Form.Get("ProjectHeads")))
				{
					validationErrors.Add("Project Heads is required.");
				}
				if (string.IsNullOrWhiteSpace(request.Form.Get("ProjectAmount")))
				{
					validationErrors.Add("Project Amount is required.");
				}
				if (!int.TryParse(request.Form.Get("Project_Id"), out var _))
				{
					validationErrors.Add("Invalid Project ID format.");
				}
				if (!decimal.TryParse(request.Form.Get("ProjectAmount"), out var _))
				{
					validationErrors.Add("Invalid Project Amount format.");
				}
				RemoteSensingProject.Models.Admin.main.Project_Budget formData = new RemoteSensingProject.Models.Admin.main.Project_Budget
				{
					Id = Convert.ToInt32(request.Form.Get("Id")),
					Project_Id = Convert.ToInt32(request.Form.Get("Project_Id")),
					ProjectHeads = request.Form.Get("ProjectHeads"),
					ProjectAmount = Convert.ToDecimal(request.Form.Get("ProjectAmount"))
				};
				if (validationErrors.Any())
				{
					return BadRequest(new
					{
						status = false,
						StatusCode = 500,
						message = string.Join("\n", validationErrors)
					});
				}
				decimal ProjectBudget = _adminServices.GetProjectById(formData.Project_Id).pm.ProjectBudget;
				decimal totalBudgets = _adminServices.ProjectBudgetList(formData.Project_Id).Sum((RemoteSensingProject.Models.Admin.main.Project_Budget x) => x.ProjectAmount);
				if (totalBudgets + formData.ProjectAmount <= ProjectBudget)
				{
					bool res = _adminServices.insertProjectBudgets(formData);
					return Ok(new
					{
						status = res,
						StatusCode = (res ? 200 : 500),
						message = (res ? "Project budget added successfully !" : "Some issue occured !")
					});
				}
				return BadRequest(new
				{
					status = false,
					StatusCode = 404,
					message = "Maximum amount reached !"
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

		[HttpPost]
		[Route("api/adminAddStages")]
		public IHttpActionResult AddStages()
		{
			try
			{
				HttpRequest request = HttpContext.Current.Request;
				List<string> validationErrors = new List<string>();
				if (string.IsNullOrWhiteSpace(request.Form.Get("Project_Id")))
				{
					validationErrors.Add("Project ID is required.");
				}
				if (string.IsNullOrWhiteSpace(request.Form.Get("KeyPoint")))
				{
					validationErrors.Add("Stages keys is required.");
				}
				if (string.IsNullOrWhiteSpace(request.Form.Get("CompletionDate")))
				{
					validationErrors.Add("Completion date  is required.");
				}
				if (validationErrors.Any())
				{
					return BadRequest(new
					{
						status = false,
						StatusCode = 500,
						message = string.Join("\n", validationErrors)
					});
				}
				RemoteSensingProject.Models.Admin.main.Project_Statge formData = new RemoteSensingProject.Models.Admin.main.Project_Statge
				{
					Id = Convert.ToInt32(request.Form.Get("Id")),
					Project_Id = Convert.ToInt32(request.Form.Get("Project_Id")),
					KeyPoint = request.Form.Get("KeyPoint"),
					Document_Url = request.Form.Get("Document_Url"),
					CompletionDate = Convert.ToDateTime(request.Form.Get("CompletionDate"))
				};
				HttpPostedFile file = request.Files["Stage_Document"];
				if (file != null && file.FileName != "")
				{
					formData.Document_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					formData.Document_Url = Path.Combine("/ProjectContent/Admin/ProjectDocs/", formData.Document_Url);
				}
				bool res = _adminServices.insertProjectStages(formData);
				if (res && file != null && file.FileName != "")
				{
					file.SaveAs(HttpContext.Current.Server.MapPath(formData.Document_Url));
				}
				return Ok(new
				{
					status = res,
					StatusCode = (res ? 200 : 500),
					message = (res ? "Project stages added successfully !" : "Some issue occured while processing request ! Please try after sometome.")
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

		[System.Web.Mvc.AllowAnonymous]
		[HttpGet]
		[Route("api/GetProjectBudgets")]
		public IHttpActionResult GetProjetBudgets(int projectId)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.Project_Budget> data = _adminServices.ProjectBudgetList(projectId);
				if (!data.Any())
				{
					return Ok(new
					{
						status = false,
						StatusCode = 404,
						message = "Data not found !"
					});
				}
				return Ok(new
				{
					status = true,
					StatusCode = 200,
					message = "Data found !",
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

		[HttpGet]
		[Route("api/getProjectStages")]
		public IHttpActionResult GetProjectSatges(int projectId)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.Project_Statge> data = _adminServices.ProjectStagesList(projectId);
				string[] selectedprop = new string[8] { "Id", "Project_Id", "KeyPoint", "CompletionDate", "CompletionDatestring", "Status", "Document_Url", "completionStatus" };
				List<object> newdata = CommonHelper.SelectProperties(data, selectedprop);
				if (!newdata.Any())
				{
					return BadRequest(new
					{
						status = false,
						StatusCode = 404,
						message = "Data not found !"
					});
				}
				return Ok(new
				{
					status = true,
					StatusCode = 200,
					message = "Data found !",
					data = newdata
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

		[HttpGet]
		[Route("api/adminMeetingList")]
		public IHttpActionResult MeetingList(int? limit = null, int? page = null, string searchTerm = null, string statusFilter = null, string meetingMode = null)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.Meeting_Model> data = _adminServices.getAllmeeting(limit, page, searchTerm, statusFilter, meetingMode);
				string[] selectprop = new string[10] { "Id", "CompleteStatus", "MeetingType", "MeetingLink", "MeetingTitle", "memberId", "CreaterId", "MeetingDate", "summary", "Attachment_Url" };
				List<object> newData = CommonHelper.SelectProperties(data, selectprop);
				if (newData.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, newData, "Data fetched successfully", 200, data[0].Pagination);
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[HttpGet]
		[Route("api/GetMeetingMemberListById")]
		public IHttpActionResult GetMeetingMemberList(int meetId)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.Employee_model> data = _adminServices.GetMeetingMemberList(meetId);
				string[] selectProperties = new string[7] { "Id", "EmployeeCode", "EmployeeName", "EmployeeRole", "MobileNo", "Email", "meetingId" };
				List<object> newData = CommonHelper.SelectProperties(data, selectProperties);
				if (newData.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, newData, "Data fetched successfully");
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[HttpGet]
		[Route("api/getMeetingKeyResponse")]
		public IHttpActionResult GetMeetingKeyResponse(int id)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.KeyPointResponse> data = _adminServices.getKeypointResponse(id);
				return Ok(new
				{
					status = true,
					StatusCode = 200,
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

		[HttpGet]
		[Route("api/getMeetingPresentMember")]
		public IHttpActionResult GetMeetingPresentMember(int MeetId)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.Employee_model> data = _adminServices.getPresentMember(MeetId);
				string[] selectedprop = new string[4] { "EmployeeName", "Image_url", "EmployeeRole", "PresentStatus" };
				List<object> newData = CommonHelper.SelectProperties(data, selectedprop);
				return Ok(new
				{
					status = true,
					StatusCode = 200,
					data = newData
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

		[HttpGet]
		[Route("api/getMeetingConclusion")]
		public IHttpActionResult GetMeetingCoclusion(int MeetId)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.MeetingConclusion> data = _adminServices.getConclusion(MeetId);
				return Ok(new
				{
					status = true,
					StatusCode = 200,
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
		[Route("api/UpdateMeetingConclusion")]
		public IHttpActionResult UpdateMeetingConclusion()
		{
			try
			{
				HttpRequest request = HttpContext.Current.Request;
				List<string> validationErrors = new List<string>();
				if (string.IsNullOrWhiteSpace(request.Form.Get("Meeting")))
				{
					validationErrors.Add("Meeting Id is required.");
				}
				if (string.IsNullOrWhiteSpace(request.Form.Get("Conclusion")))
				{
					validationErrors.Add("Meeting conclusion is required.");
				}
				if (string.IsNullOrWhiteSpace(request.Form.Get("FollowUpStatus")))
				{
					validationErrors.Add("Follow up status is required.");
				}
				RemoteSensingProject.Models.Admin.main.MeetingConclusion formData = new RemoteSensingProject.Models.Admin.main.MeetingConclusion
				{
					Meeting = Convert.ToInt32(request.Form.Get("Meeting")),
					Conclusion = request.Form.Get("Conclusion"),
					FollowUpStatus = Convert.ToBoolean(request.Form.Get("FollowUpStatus")),
					NextFollowUpDate = (string.IsNullOrWhiteSpace(request.Form["NextFollowUpDate"]) ? ((DateTime?)null) : new DateTime?(DateTime.Parse(request.Form["NextFollowUpDate"]))),
					summary = request.Form.Get("summary")
				};
				if (request.Form["MeetingMemberList"] != null)
				{
					formData.MeetingMemberList = (from e in request.Form["MeetingMemberList"].Split(',')
												  select int.Parse(e)).ToList();
				}
				else
				{
					validationErrors.Add("Meeting member list is required !");
				}
				if (request.Form["MemberId"] != null)
				{
					formData.MemberId = request.Form["MemberId"].Split(',').ToList();
				}
				else
				{
					validationErrors.Add("Member Id is required !");
				}
				if (request.Form["KeyResponse"] != null)
				{
					formData.KeyResponse = request.Form["KeyResponse"].Split(',').ToList();
				}
				else
				{
					validationErrors.Add("Key responses is required !");
				}
				if (request.Form["KeyPointId"] != null)
				{
					formData.KeyPointId = request.Form["KeyPointId"].Split(',').ToList();
				}
				else
				{
					validationErrors.Add("Key Id is required !");
				}
				if (validationErrors.Any())
				{
					return BadRequest(new
					{
						status = false,
						StatusCode = 500,
						message = string.Join("\n", validationErrors)
					});
				}
				bool res = _adminServices.AddMeetingResponse(formData);
				return Ok(new
				{
					status = res,
					StatusCode = (res ? 200 : 500),
					message = (res ? "Meeting conclusion updated successfully !" : "Some issue occured while processing request.")
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

		[HttpGet]
		[Route("api/getMemberJoiningStatus")]
		public IHttpActionResult GetMemberJoiningStatus(int meetingId)
		{
			List<RemoteSensingProject.Models.Admin.main.Employee_model> res = _managerservice.getMemberJoiningStatus(meetingId);
			return Ok(new
			{
				status = true,
				message = "data retrieved",
				data = res
			});
		}

		[HttpGet]
		[Route("api/getallNoticeList")]
		public IHttpActionResult NoticeList(int? limit = null, int? page = null, int? projectId = null, string searchTerm = null)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.Generate_Notice> data = _adminServices.getNoticeList(limit, page, projectId, null, searchTerm);
				string[] selectprop = new string[9] { "Id", "ProjectId", "ProjectManagerId", "Attachment_Url", "Notice", "ProjectManagerImage", "ProjectManager", "ProjectName", "noticeDate" };
				List<object> newData = CommonHelper.SelectProperties(data, selectprop);
				if (data.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, newData, "Data fetched successfully", 200, data[0].Pagination);
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[HttpPost]
		[Route("api/adminCreateNotice")]
		public IHttpActionResult CreateNotice()
		{
			HttpRequest request = HttpContext.Current.Request;
			RemoteSensingProject.Models.Admin.main.Generate_Notice formData = new RemoteSensingProject.Models.Admin.main.Generate_Notice
			{
				Id = Convert.ToInt32(request.Form.Get("Id")),
				ProjectId = Convert.ToInt32(request.Form.Get("ProjectId")),
				Attachment_Url = request.Form.Get("Attachment_Url"),
				Notice = request.Form.Get("Notice")
			};
			HttpPostedFile file = request.Files["Attachment"];
			if (file != null && file.FileName != "")
			{
				formData.Attachment_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
				formData.Attachment_Url = Path.Combine("/ProjectContent/Admin/NoticeDocs/", formData.Attachment_Url);
			}
			bool res = _adminServices.InsertNotice(formData);
			if (res && file != null && file.FileName != "")
			{
				file.SaveAs(HttpContext.Current.Server.MapPath(formData.Attachment_Url));
			}
			return Ok(new
			{
				status = res,
				StatusCode = (res ? 200 : 500),
				message = (res ? "Notice created !" : "Some issue occured !")
			});
		}

		[HttpGet]
		[Route("api/DivisonList")]
		public IHttpActionResult DivisonList()
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.CommonResponse> data = _adminServices.ListDivison();
				if (data != null && data.Count > 0)
				{
					return Ok(new
					{
						status = true,
						StatusCode = 200,
						message = "All divison fetched !",
						data = data
					});
				}
				return BadRequest(new
				{
					StatusCode = HttpStatusCode.BadRequest,
					message = "Some issue found while processing request."
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

		[HttpGet]
		[Route("api/getAllProblemList")]
		public IHttpActionResult getAllProblemList(int? limit = null, int? page = null, string searchTerm = null)
		{
			try
			{
				List<RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem> res = _managerservice.getSubOrdinateProblemforAdmin(limit, page, searchTerm);
				string[] selectProp = new string[8] { "ProblemId", "ProjectName", "Title", "Description", "Attchment_Url", "CreatedDate", "newRequest", "projectCode" };
				List<object> newdata = CommonHelper.SelectProperties(res, selectProp);
				if (res.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, newdata, "Data fetched successfully", 200, res[0].Pagination);
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[HttpGet]
		[Route("api/GetReimbursementList")]
		public IHttpActionResult GetReimbursementList(int? page = null, int? limit = null, int? managerId = null, string typeFilter = null, string statusFilter = null)
		{
			try
			{
				List<Reimbursement> data = _managerservice.GetReimbursements(page, limit, null, managerId, "selectAll", typeFilter, statusFilter);
				if (data.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, data, "Data fetched successfully", 200, data[0].Pagination);
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[HttpGet]
		[Route("api/ApproveReimbursement")]
		public IHttpActionResult ApproveReimbursement(int id, bool status, string type, string remark)
		{
			try
			{
				bool res = _adminServices.ReimbursementApproval(status, id, type, remark);
				return Ok(new
				{
					status = res,
					StatusCode = (res ? 200 : 500),
					message = ((res && status) ? "Approved Successfully" : (res ? "Rejected  Successfully" : "Something went wrong"))
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

		[HttpGet]
		[Route("api/ApprovalHiring")]
		public IHttpActionResult ApproveHiring(int id, bool status, string remark, string location)
		{
			try
			{
				bool res = _adminServices.HiringApproval(id, status, remark, location);
				return Ok(new
				{
					status = res,
					StatusCode = (res ? 200 : 500),
					message = ((res && status) ? "Approved Successfully" : (res ? "Rejected  Successfully" : "Something went wrong"))
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

		[HttpGet]
		[Route("api/GetAllHiring")]
		public IHttpActionResult AllHiring(int? page = null, int? limit = null, int? managerFilter = null, int? projectFilter = null)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.HiringVehicle1> data = _adminServices.HiringList(page, limit, managerFilter, projectFilter).ToList();
				if (data.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, data, "Data fetched successfully", 200, data[0].Pagination);
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[HttpGet]
		[Route("api/ViewAlltourAdminView")]
		public IHttpActionResult AllTour(int? page = null, int? limit = null, int? managerFilter = null, int? projectFilter = null)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.tourProposalAll> data = _adminServices.getAllTourList(page, limit, null, null, managerFilter, projectFilter).ToList();
				if (data.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, data, "Data fetched successfully", 200, data[0].Pagination);
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[HttpGet]
		[Route("api/ApproveTourProposal")]
		public IHttpActionResult ApproveTourProposal(int id, bool status, string remark)
		{
			try
			{
				bool res = _adminServices.Tourapproval(id, status, remark);
				return Ok(new
				{
					status = res,
					StatusCode = (res ? 200 : 500),
					message = ((res && status) ? "Approved Successfully" : (res ? "Rejected  Successfully" : "Something went wrong"))
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

		[Route("api/budgetforgraph")]
		[HttpGet]
		public IHttpActionResult GetBudgetForGraph()
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.BudgetForGraph> data = _adminServices.BudgetForGraph();
				return Ok(new
				{
					status = data.Any(),
					data = data
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					status = false,
					StatusCode = 500,
					Message = ex.Message
				});
			}
		}

		[HttpGet]
		[Route("api/ReimbursementReport")]
		public IHttpActionResult ReimbursementReport(int? limit = null, int? page = null, int? projectManagerFilter = null, string typeFilter = null, string statusFilter = null)
		{
			try
			{
				List<Reimbursement> data = _managerservice.GetReimbursements(page, limit, null, projectManagerFilter, "selectReinbursementReport", typeFilter, statusFilter);
				return Ok(new
				{
					status = data.Any(),
					StatuCode = (data.Any() ? 200 : 500),
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

		[HttpGet]
		[Route("api/HiringReportProjects")]
		public IHttpActionResult HiringReportProjects()
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.HiringVehicle1> data = _adminServices.hiringreportprojects();
				return Ok(new
				{
					status = data.Any(),
					StatuCode = (data.Any() ? 200 : 500),
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

		[HttpGet]
		[Route("api/HiringReportByProject")]
		public IHttpActionResult HiringReportByProject(int projectId)
		{
			try
			{
				ManagerService managerservice = _managerservice;
				int? id = projectId;
				List<HiringVehicle> data = managerservice.GetHiringVehicles(projectId);
				return Ok(new
				{
					status = data.Any(),
					StatuCode = (data.Any() ? 200 : 500),
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

		[HttpGet]
		[Route("api/tourproposalproject")]
		public IHttpActionResult tourproposalproject()
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.tourProposalrepo> data = _adminServices.TourReportProject();
				return Ok(new
				{
					status = data.Any(),
					StatuCode = (data.Any() ? 200 : 500),
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

		[HttpGet]
		[Route("api/tourproposalbyproject")]
		public IHttpActionResult tourproposalbyproject(int projectId)
		{
			try
			{
				AdminServices adminServices = _adminServices;
				int? id = projectId;
				List<RemoteSensingProject.Models.Admin.main.tourProposalAll> data = adminServices.getAllTourList(null, null, "GetById", id);
				return Ok(new
				{
					status = data.Any(),
					StatuCode = (data.Any() ? 200 : 500),
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

		[HttpGet]
		[Route("api/getraisedproblemforadmin")]
		public IHttpActionResult getRaisedProblem(int? limit = null, int? page = null, string searchTerm = null)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.RaisedProblem> data = _adminServices.getProblemList(page, limit, null, null, searchTerm);
				if (data.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, data, "Data fetched successfully");
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[System.Web.Mvc.AllowAnonymous]
		[HttpGet]
		[Route("api/getRaisedProblemById")]
		public IHttpActionResult getRaisedProblemById(int id, int? managerId = 0)
		{
			try
			{
				string role = getRole();
				if (role.Equals("projectManager"))
				{
					ClaimsIdentity identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
					int userId = int.Parse(identity.Claims.FirstOrDefault((Claim c) => c.Type == "userId")?.Value);
					managerId = userId;
				}
				AdminServices adminServices = _adminServices;
				int? managerId2 = managerId;
				int? id2 = id;
				List<RemoteSensingProject.Models.Admin.main.RaisedProblem> data = adminServices.getProblemList(null, null, id2, managerId2);
				return Ok(new
				{
					status = data.Any(),
					data = data
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					status = false,
					StatusCode = 500,
					message = ex.Message
				});
			}
		}

		[HttpGet]
		[Route("api/viewExpenses")]
		public IHttpActionResult viewExpenses(int id)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.Project_Budget> data = _adminServices.ProjectBudgetList(id);
				return Ok(new
				{
					status = data.Any(),
					data = data
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					status = false,
					StatusCode = 500,
					message = ex.Message
				});
			}
		}

		[HttpGet]
		[Route("api/getmeetingforadmin")]
		public IHttpActionResult getmeetingadmin(int? limit = null, int? page = null)
		{
			try
			{
				IEnumerable<RemoteSensingProject.Models.Admin.main.Meeting_Model> data = from d in _adminServices.getAllmeeting(limit, page)
																						 where d.createdBy == "admin"
																						 select d;
				return Ok(new
				{
					status = data.Any(),
					data = data
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					status = false,
					StatusCode = 500,
					message = ex.Message
				});
			}
		}

		[HttpGet]
		[Route("api/getmeetingforprojectmanager")]
		public IHttpActionResult getmeetingprojectmanager(int? limit = null, int? page = null)
		{
			try
			{
				IEnumerable<RemoteSensingProject.Models.Admin.main.Meeting_Model> data = from d in _adminServices.getAllmeeting(limit, page)
																						 where d.createdBy == "projectmanager"
																						 select d;
				return Ok(new
				{
					status = data.Any(),
					data = data
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					status = false,
					StatusCode = 500,
					message = ex.Message
				});
			}
		}

		[JwtAuthorize(Roles = "admin,accounts")]
		[HttpGet]
		[Route("api/allhiringreport")]
		public IHttpActionResult AllHiringReport(int? limit = null, int? page = null, int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.HiringVehicle1> data = _adminServices.HiringReort(limit, page, managerFilter, projectFilter, statusFilter);
				if (data.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, data, "Data fetched successfully");
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[JwtAuthorize(Roles = "admin,accounts")]
		[HttpGet]
		[Route("api/alltourproposalreport")]
		public IHttpActionResult AllTourproposalReport(int? limit = null, int? page = null, int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
		{
			try
			{
				List<RemoteSensingProject.Models.Accounts.main.tourProposal> data = _accountService.getTourList(limit, page, managerFilter, projectFilter, statusFilter);
				if (data.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, data, "Data fetched successfully");
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[HttpGet]
		[Route("api/getOutsource")]
		public IHttpActionResult GetOutsource(int userId)
		{
			try
			{
				List<OuterSource> data = _managerservice.selectAllOutSOurceList(userId);
				return Ok(new
				{
					status = data.Any(),
					StatuCode = (data.Any() ? 200 : 500),
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

		[HttpGet]
		[Route("api/getphyfinGraphData")]
		public IHttpActionResult getgraphData(int? page, int? limit)
		{
			try
			{
				DateTime twoYearsAgo = DateTime.Now.AddYears(-2);
				List<RemoteSensingProject.Models.Admin.main.Project_model> data = (from d in _adminServices.Project_List(page, limit)
																				   where d.AssignDate >= twoYearsAgo
																				   select d).ToList();
				string[] selectProperties = new string[20]
				{
				"Id", "ProjectTitle", "AssignDate", "CompletionDate", "StartDate", "ProjectManager", "Percentage", "ProjectBudget", "ProjectDescription", "projectDocumentUrl",
				"ProjectType", "physicalcomplete", "overallPercentage", "ProjectStage", "CompletionDatestring", "ProjectStatus", "AssignDateString", "StartDateString", "createdBy", "projectCode"
				};
				List<object> newData = CommonHelper.SelectProperties(data, selectProperties);
				if (data.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, newData, "Data fetched successfully", 200, data[0].Pagination);
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[System.Web.Mvc.AllowAnonymous]
		[HttpGet]
		[Route("api/getOutsourceByPm")]
		public IHttpActionResult getOutsourceByProjectManager(int projectManager, string searchTerm = null)
		{
			try
			{
				List<AttendanceManage> data = _managerservice.getAttendanceCount(projectManager, searchTerm);
				string[] selectprop = new string[4] { "EmpId", "EmpName", "present", "absent" };
				List<object> newdata = CommonHelper.SelectProperties(data, selectprop);
				return Ok(new
				{
					status = data.Any(),
					data = newdata,
					StatusCode = 200
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					status = false,
					StatusCode = 500,
					message = ex.Message
				});
			}
		}

		[System.Web.Mvc.AllowAnonymous]
		[HttpGet]
		[Route("api/getAttendanceListByEmp")]
		public IHttpActionResult getAttendanceListByEmp(int projectManager, int EmpId)
		{
			try
			{
				List<AttendanceManage> data = _managerservice.GetAllAttendanceForProjectManager(projectManager, EmpId);
				return Ok(new
				{
					status = data.Any(),
					data = data,
					StatusCode = 200
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					status = false,
					StatusCode = 500,
					message = ex.Message
				});
			}
		}

		[HttpGet]
		[Route("api/MonthFilter")]
		public IHttpActionResult monthFilter(int month, int year, int projectManager, int EmpId)
		{
			try
			{
				List<AttendanceManage> data = _managerservice.getReportAttendance(month, year, projectManager, EmpId);
				return Ok(new
				{
					status = data.Any(),
					data = data,
					StatusCode = 200
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					status = false,
					StatusCode = 500,
					message = ex.Message
				});
			}
		}

		[System.Web.Mvc.AllowAnonymous]
		[HttpGet]
		[Route("api/getAttendanceByEmp")]
		public IHttpActionResult getAttendanceByEmp(int projectManager, int EmpId)
		{
			try
			{
				List<AttendanceManage> data = _managerservice.GetAllAttendanceForProjectManager(projectManager, EmpId);
				return Ok(new
				{
					status = data.Any(),
					data = data,
					StatusCode = 200
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					status = false,
					StatusCode = 500,
					message = ex.Message
				});
			}
		}

		[System.Web.Mvc.AllowAnonymous]
		[HttpGet]
		[Route("api/getAttendanceRepo")]
		public IHttpActionResult getAttendanceRepo(int month, int year, int projectManager, int? EmpId = null)
		{
			try
			{
				List<AttendanceManage> data = _managerservice.getReportAttendance(month, year, projectManager, EmpId.HasValue ? Convert.ToInt32(EmpId) : 0);
				return Ok(new
				{
					status = data.Any(),
					data = data,
					StatusCode = 200
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					status = false,
					StatusCode = 500,
					message = ex.Message
				});
			}
		}

		[HttpGet]
		[Route("api/getEmployeeMonthlyReportForAdmin")]
		public IHttpActionResult EmployeeMonthlyReport(int empId)
		{
			try
			{
				List<EmpReportModel> data = _managerservice.GetEmpReport(empId);
				return Ok(new
				{
					status = data.Any(),
					data = data,
					StatusCode = 200,
					message = (data.Any() ? "Data found !" : "Data not found !")
				});
			}
			catch
			{
				return BadRequest(new
				{
					status = false,
					StatusCode = 500,
					message = "Some issue occured while processing request."
				});
			}
		}

		[System.Web.Mvc.AllowAnonymous]
		[HttpGet]
		[Route("api/getbudgetheads")]
		public IHttpActionResult GetBudgetHeads()
		{
			try
			{
				List<RemoteSensingProject.Models.Admin.main.BudgetHeadModel> data = _adminServices.GetBudgetHeads();
				return Ok(new
				{
					status = true,
					message = "Budget heads fetched successfully",
					data = data
				});
			}
			catch (Exception ex)
			{
				return Content(HttpStatusCode.InternalServerError, new
				{
					status = false,
					message = "Something went wrong",
					error = ex.Message
				});
			}
		}

		[System.Web.Mvc.AllowAnonymous]
		[HttpGet]
		[Route("api/GetCMDashboard")]
		public IHttpActionResult GetCMDashboard(int? id = null, bool? status = true, int? startYear = null, string statusFilter = null)
		{
			List<RemoteSensingProject.Models.Admin.main.CMDashboardData> data = _adminServices.GetCmDashboardList(id, status, startYear, statusFilter);
			int totalProject = data.Count();
			int totalOngoing = data.Where((RemoteSensingProject.Models.Admin.main.CMDashboardData d) => d.projectStatus.Equals("Ongoing", StringComparison.OrdinalIgnoreCase)).Count();
			int totalComplete = data.Where((RemoteSensingProject.Models.Admin.main.CMDashboardData d) => d.projectStatus.Equals("Complete", StringComparison.OrdinalIgnoreCase)).Count();
			return Ok(new
			{
				status = (data.Count > 0),
				totalProject = totalProject,
				totalOngoing = totalOngoing,
				totalComplete = totalComplete,
				data = data
			});
		}

		private IHttpActionResult BadRequest(object value)
		{
			return Content<object>(HttpStatusCode.BadRequest, value);
		}
	}
}