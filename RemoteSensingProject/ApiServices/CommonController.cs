// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.ApiServices.CommonController
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.ProjectManager;
using static RemoteSensingProject.Models.CommonHelper;

[JwtAuthorize(Roles = "admin,account,projectManager,subOrdinate,outSource,prashasan")]
public class CommonController : ApiController
{
	private readonly AdminServices _adminServices;

	private readonly ManagerService _managerservice;

	private readonly LoginServices _loginService;

	private readonly AccountService _accountService;

	public CommonController()
	{
		_adminServices = new AdminServices();
		_loginService = new LoginServices();
		_managerservice = new ManagerService();
		_accountService = new AccountService();
	}
    #region Manage Project
    [RoleAuthorize("admin,projectManager")]
	[HttpPost]
	[Route("api/adminCreateProject")]
	public IHttpActionResult CreateProject()
	{
		List<string> savedFiles = new List<string>();
		try
		{
			HttpRequest request = HttpContext.Current.Request;
			NameValueCollection form = request.Form;
			HttpFileCollection files = request.Files;
			RemoteSensingProject.Models.Admin.main.createProjectModel model = new RemoteSensingProject.Models.Admin.main.createProjectModel();
			model.pm = new RemoteSensingProject.Models.Admin.main.Project_model();
			model.pm.Id = Convert.ToInt32(form["Id"] ?? "0");
			model.pm.ProjectTitle = form["ProjectTitle"];
			model.pm.ProjectManager = form["ProjectManager"];
			model.pm.ProjectDescription = form["ProjectDescription"];
			model.pm.ProjectType = form["ProjectType"];
			model.pm.letterNo = form["letterNo"] ?? "0";
			if (string.IsNullOrEmpty(form["createdBy"]))
			{
				return CommonHelper.Error((ApiController)(object)this, "Created By is required");
			}
			model.pm.createdBy = form["createdBy"].ToString();
			if (!bool.TryParse(form["projectStage"], out var projectStages))
			{
				return CommonHelper.Error((ApiController)(object)this, "Project Stages is required");
			}
			model.pm.ProjectStage = projectStages;
			if (!DateTime.TryParse(form["AssignDate"], out var assignDate))
			{
				return CommonHelper.Error((ApiController)(object)this, "Assign Date is required.");
			}
			model.pm.AssignDate = assignDate;
			if (!DateTime.TryParse(form["StartDate"], out var startDate))
			{
				return CommonHelper.Error((ApiController)(object)this, "Start Date is required.");
			}
			model.pm.StartDate = startDate;
			if (!DateTime.TryParse(form["CompletionDate"], out var compDate))
			{
				return CommonHelper.Error((ApiController)(object)this, "Completion Date is required.");
			}
			model.pm.CompletionDate = compDate;
			if (!decimal.TryParse(form["ProjectBudget"], out var budget))
			{
				return CommonHelper.Error((ApiController)(object)this, "Invalid Project Budget");
			}
			model.pm.ProjectBudget = budget;
			if (!int.TryParse(form["hrcount"], out var hr))
			{
				return CommonHelper.Error((ApiController)(object)this, "Invalid Human resources count");
			}
			model.pm.hrCount = hr;
			if (model.pm.ProjectType?.ToLower() == "external")
			{
				model.pm.ContactPerson = form["ContactPerson"];
				model.pm.ProjectDepartment = form["ProjectDepartment"];
				model.pm.Address = form["Address"];
			}
			if (!string.IsNullOrEmpty(form["SubOrdinate"]))
			{
				model.pm.SubOrdinate = form["SubOrdinate"].Split(',').Select(int.Parse).ToArray();
			}
			if (!string.IsNullOrEmpty(form["budgets"]))
			{
				model.budgets = JsonConvert.DeserializeObject<List<RemoteSensingProject.Models.Admin.main.Project_Budget>>(form["budgets"]);
				if (model.budgets != null && model.budgets.Any())
				{
					decimal totalBudgetAmount = model.budgets.Sum((RemoteSensingProject.Models.Admin.main.Project_Budget b) => b.ProjectAmount);
					if (totalBudgetAmount > model.pm.ProjectBudget)
					{
						return CommonHelper.Error((ApiController)(object)this, $"Total of all ProjectAmounts ({totalBudgetAmount}) cannot exceed the main ProjectBudget ({model.pm.ProjectBudget}).", 400);
					}
				}
			}
			if (!string.IsNullOrEmpty(form["hrcount"]))
			{
				model.hr = JsonConvert.DeserializeObject<List<RemoteSensingProject.Models.Admin.main.HumanResources>>(form["hr"]);
				if (model.hr != null && model.hr.Any())
				{
					decimal totalhrCount = model.hr.Sum((RemoteSensingProject.Models.Admin.main.HumanResources b) => b.designationCount);
					if (totalhrCount > (decimal)model.pm.hrCount)
					{
						return CommonHelper.Error((ApiController)(object)this, $"Total of all Human Resources ({totalhrCount}) cannot exceed the Human Resources count ({model.pm.hrCount}).", 400);
					}
				}
			}
			if (!string.IsNullOrEmpty(form["stages"]) && model.pm.ProjectStage)
			{
				model.stages = JsonConvert.DeserializeObject<List<RemoteSensingProject.Models.Admin.main.Project_Statge>>(form["stages"]);
			}
			string filePage = HttpContext.Current.Server.MapPath("~/ProjectContent/Admin/ProjectDocs/");
			if (!Directory.Exists(filePage))
			{
				Directory.CreateDirectory(filePage);
			}
			HttpPostedFile projectFile = files["projectDocument"];
			if (projectFile != null && projectFile.ContentLength > 0)
			{
				string fileName = DateTime.Now.ToString("ddMMyyyy") + "_" + Guid.NewGuid().ToString() + Path.GetExtension(projectFile.FileName);
				string filePath = "/ProjectContent/Admin/ProjectDocs/" + fileName;
				model.pm.projectDocumentUrl = filePath;
				projectFile.SaveAs(HttpContext.Current.Server.MapPath(filePath));
				savedFiles.Add(filePath);
			}
			if (model.stages != null && model.pm.ProjectStage)
			{
				for (int i = 0; i < model.stages.Count; i++)
				{
					HttpPostedFile stageFile = files["StageDocument_" + i];
					if (stageFile != null && stageFile.ContentLength > 0)
					{
						string fileName2 = "STAGE_" + i + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + Guid.NewGuid().ToString() + Path.GetExtension(stageFile.FileName);
						string filePath2 = "/ProjectContent/Admin/ProjectDocs/" + fileName2;
						stageFile.SaveAs(HttpContext.Current.Server.MapPath(filePath2));
						model.stages[i].StageDocument_Url = filePath2;
						savedFiles.Add(filePath2);
					}
				}
			}
			if (_adminServices.addProject(model))
			{
				return CommonHelper.Success((ApiController)(object)this, (model.pm.Id <= 0) ? "Project created successfully!" : "Project updated successfully!");
			}
			return CommonHelper.Error((ApiController)(object)this, "Something went wrong.");
		}
		catch (Exception ex)
		{
			foreach (string f in savedFiles)
			{
				if (File.Exists(f))
				{
					File.Delete(f);
				}
			}
			return CommonHelper.Error((ApiController)(object)this, ex.Message);
		}
	}
	[System.Web.Mvc.AllowAnonymous]
	[RoleAuthorize("admin,projectManager")]
	[HttpGet]
	[Route("api/GetadminProjectDetailById")]
	public IHttpActionResult GetProjectById(int Id)
	{
		try
		{
			RemoteSensingProject.Models.Admin.main.createProjectModel data = _adminServices.GetProjectById(Id);
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

    #endregion

    #region Manage Meetings
    [RoleAuthorize("admin,projectManager")]
	[HttpPost]
	[Route("api/adminCreateMeeting")]
	public IHttpActionResult CreateMeeting()
	{
		try
		{
			HttpRequest request = HttpContext.Current.Request;
			List<string> validationErrors = new List<string>();
			if (string.IsNullOrWhiteSpace(request.Form.Get("MeetingType")))
			{
				validationErrors.Add("Meeting Type is required.");
			}
			if (string.IsNullOrWhiteSpace(request.Form.Get("MeetingTitle")))
			{
				validationErrors.Add("Meeting title is required.");
			}
			if (string.IsNullOrWhiteSpace(request.Form.Get("MeetingTime")))
			{
				validationErrors.Add("Meeting time is required.");
			}
			if (string.IsNullOrWhiteSpace(request.Form.Get("meetingMemberList")))
			{
				validationErrors.Add("Meeting member is required.");
			}
			if (string.IsNullOrWhiteSpace(request.Form.Get("keyPointList")))
			{
				validationErrors.Add("Key points is required.");
			}
			int id;
			DateTime meetingTime;
			int createrId;
			RemoteSensingProject.Models.Admin.main.AddMeeting_Model formData = new RemoteSensingProject.Models.Admin.main.AddMeeting_Model
			{
				Id = (int.TryParse(request.Form.Get("Id"), out id) ? id : 0),
				MeetingType = (request.Form.Get("MeetingType") ?? string.Empty),
				MeetingLink = (request.Form.Get("MeetingLink") ?? string.Empty),
				MeetingTitle = (request.Form.Get("MeetingTitle") ?? string.Empty),
				MeetingAddress = (request.Form.Get("MeetingAddress") ?? string.Empty),
				MeetingTime = (DateTime.TryParse(request.Form.Get("MeetingTime"), out meetingTime) ? meetingTime : DateTime.MinValue),
				Attachment_Url = (request.Form.Get("Attachment_Url") ?? string.Empty),
				CreaterId = (int.TryParse(request.Form.Get("CreaterId"), out createrId) ? createrId : 0)
			};
			string filePage = HttpContext.Current.Server.MapPath("~/ProjectContent/Admin/Meeting_Attachment/");
			if (!Directory.Exists(filePage))
			{
				Directory.CreateDirectory(filePage);
			}
			HttpPostedFile file = request.Files["Attachment"];
			if (file != null && file.FileName != "")
			{
				formData.Attachment_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
				formData.Attachment_Url = Path.Combine("/ProjectContent/Admin/Meeting_Attachment/", formData.Attachment_Url);
			}
			else if (string.IsNullOrWhiteSpace(request.Form.Get("Attachment_Url")))
			{
				validationErrors.Add("Meeting attachment is required.");
			}
			if (request.Form["meetingMemberList"] != null)
			{
				formData.meetingMemberList = ((request.Form["meetingMemberList"] != null) ? (from value in request.Form["meetingMemberList"].Split(',')
					select int.Parse(value.ToString())).ToList() : new List<int>());
			}
			if (request.Form["keyPointList"] != null)
			{
				formData.keyPointList = request.Form["keyPointList"].Split(',').ToList();
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
			bool res = _adminServices.insertMeeting(formData);
			if (res && file != null && file.FileName != "")
			{
				file.SaveAs(HttpContext.Current.Server.MapPath(formData.Attachment_Url));
			}
			return Ok(new
			{
				status = res,
				StatusCode = ((!res) ? 500 : ((formData.Id > 0) ? 200 : 201)),
				message = ((!res) ? "Some issue occured while processing request." : ((formData.Id > 0) ? "Meeting updated successfully" : "Meeting created successfully !"))
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

	[RoleAuthorize("admin,projectManager")]
	[HttpGet]
	[Route("api/GetMeetingById")]
	public IHttpActionResult GetMeetingById(int Id)
	{
		try
		{
			RemoteSensingProject.Models.Admin.main.Meeting_Model data = _adminServices.getMeetingById(Id);
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

	[RoleAuthorize("projectManager,subOrdinate")]
	[HttpPost]
	[Route("api/GetResponseFromMember")]
	public IHttpActionResult GetResponseFromMember()
	{
		HttpRequest httpRequest = HttpContext.Current.Request;
		getMemberResponse mr = new getMemberResponse
		{
			ApprovedStatus = Convert.ToInt32(httpRequest.Form.Get("approveStatus")),
			reason = httpRequest.Form.Get("reason"),
			MeetingId = Convert.ToInt32(httpRequest.Form.Get("meetingId")),
			MemberId = Convert.ToInt32(httpRequest.Form.Get("memberId"))
		};
		if (_managerservice.GetResponseFromMember(mr))
		{
			return Ok(new
			{
				status = true,
				message = "Response Send Successfully",
				statusCode = 200
			});
		}
		return Ok(new
		{
			status = true,
			message = "something went wrong",
			statusCode = 500
		});
	}

    [RoleAuthorize("projectManager,subOrdinate")]
    [HttpGet]
    [Route("api/getAllmeeting")]
    public IHttpActionResult getAllmeeting(int managerId, int? page, int? limit, string searchTerm = null, string statusFilter = null)
    {
        try
        {
            List<RemoteSensingProject.Models.Admin.main.Meeting_Model> res = _managerservice.getAllmeeting(managerId, limit, page, searchTerm, statusFilter);
            string[] selectprop = new string[10] { "Id", "CompleteStatus", "MeetingType", "MeetingLink", "MeetingTitle", "AppStatus", "memberId", "CreaterId", "MeetingDate", "createdBy" };
            List<object> data = CommonHelper.SelectProperties(res, selectprop);
            if (data.Count > 0)
            {
                return CommonHelper.Success((ApiController)(object)this, data, "Data fetched successfully", 200, res[0].Pagination);
            }
            return CommonHelper.NoData((ApiController)(object)this);
        }
        catch (Exception ex)
        {
            return CommonHelper.Error((ApiController)(object)this, ex.Message);
        }
    }
    #endregion

    #region Manage Employee
    [HttpPost]
	[Route("api/updateEmployeeData")]
	public IHttpActionResult Update_Employee()
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
			HttpPostedFile file = request.Files["EmployeeImages"];
			if (file != null && file.FileName != "")
			{
				empData.Image_url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
				empData.Image_url = "/ProjectContent/Admin/Employee_Images/" + empData.Image_url;
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
			if (empData.Id <= 0)
			{
				validationErrors.Add("Invalid request.");
			}
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
			if (string.IsNullOrWhiteSpace(empData.Image_url))
			{
				validationErrors.Add("Employee Image not found !");
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
				message = (res ? "Employee profile updation completed successfully !" : mess)
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
	[Route("api/getEmployeeById")]
	public IHttpActionResult Get_EmployeeById(int Id)
	{
		try
		{
			RemoteSensingProject.Models.Admin.main.Employee_model data = _adminServices.SelectEmployeeRecordById(Id);
			if (data != null && data.Id > 0)
			{
				return Ok(new
				{
					status = true,
					StatusCode = 200,
					message = "Data found !",
					data = data
				});
			}
			return BadRequest(new
			{
				status = false,
				StatusCode = 404,
				message = "Data not found "
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

    #endregion

    #region Manage Designation
    [RoleAuthorize("admin,prashasan")]
    [HttpPost]
    [Route("api/add-designation")]
    public IHttpActionResult AddDesignation([FromBody] RemoteSensingProject.Models.Admin.main.CommonResponse cr)
    {
        try
        {
            if (string.IsNullOrEmpty(cr.name))
                return Content(HttpStatusCode.BadRequest, new
                {
                    status = false,
                    StatusCode = 400,
                    message = "Designation name is required"
                });
            bool res = _adminServices.InsertDesignation(cr);
            return Ok(new
            {
                status = res,
                StatusCode = res ? 201 : 400,
                message = res ? cr.id > 0 ? "Updated Successfully" : "Added Successfully" : "Some error occured"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                status = false,
                StatusCode = 400,
                message = ex.Message
            });
        }
    }
    [RoleAuthorize("admin,projectManager,prashasan")]
	[HttpGet]
	[Route("api/DesginationList")]
	public IHttpActionResult DesginationList()
	{
		try
		{
			List<RemoteSensingProject.Models.Admin.main.CommonResponse> data = _adminServices.ListDesgination();
			if (data != null && data.Count > 0)
			{
				return Ok(new
				{
					status = true,
					StatusCode = 200,
					message = "All desgination fetched !",
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
    [Route("api/remove-designation")]
    public IHttpActionResult RemoveDesignation(int id)
    {
        try
        {
            bool res = _adminServices.removeDesgination(id);
            return Ok(new
            {
                status = res,
                StatusCode = res ? 200 : 400,
                message = res ? "Deleted Successfully" : "Something went wrong"
            });
        }
        catch (Exception ex)
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

    #region Manage Attendance
    [RoleAuthorize("admin,projectManager")]
	[HttpGet]
	[Route("api/getattendancebyIdofEmp")]
	public IHttpActionResult GetAttendanceByIdOfEmp(int projectManager, int EmpId)
	{
		try
		{
			List<AttendanceManage> data = _managerservice.GetAllAttendanceForProjectManager(projectManager, EmpId);
			if (data != null)
			{
				return Ok(new
				{
					status = data.Any(),
					data = data,
					message = "Data found!"
				});
			}
			return Ok(new
			{
				status = data.Any(),
				data = data,
				message = "Data not found!"
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

    #endregion

    #region Manage Hiring & Tour Proposal
    // Hiring By id
    // Get Hiring BY ID
    [System.Web.Mvc.AllowAnonymous]
    [HttpGet]
    [Route("api/getHiringById")]
    public IHttpActionResult GetHiringById(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Id is not valid"
                });
            }
            List<HiringVehicle> data = _managerservice.GetHiringVehicles(id: id, type: "GETBYID");
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

    //All Hirings
    [HttpGet]
    [Route("api/getAllHiringList")]
    public IHttpActionResult getAllHiringList(int? limit = null, int? page = null, int? projectFilter = null)
    {
        try
        {
            List<HiringVehicle> data = _managerservice.GetHiringVehicles(type: "ALLDATA", projectFilter: projectFilter, limit: limit, page: page);
            if (data.Count > 0)
            {
                return Success(this, data, "Data fetched successfully", 200, data[0].Pagination);
            }
            return NoData(this);
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

    // Tourproposal By Id
    [Route("api/getTourById")]
    [HttpGet]
    public IHttpActionResult TourById(int id)
    {
        try
        {
			if (id <= 0)
			{
				return BadRequest(new
				{
					status = false,
					message = "Id is not valid"
				});
			}
            List<tourProposal> data = _managerservice.GetTourList(type: "GETBYID", id: id);
            return Ok(new
            {
                status = data.Any(),
                data = data
            });
        }
        catch
        {
            return Ok(new
            {
                status = false,
                StatusCode = 500,
                message = "Data not found"
            });
        }
    }
    // All TourProposal
    [HttpGet]
    [Route("api/getAllTourproposalList")]
    public IHttpActionResult GetAllTourProposal(int? limit = null, int? page = null, int? projectFilter = null)
    {
        try
        {
            var data = _managerservice.GetTourList(type: "ALLDATA", projectFilter: projectFilter, limit: limit, page: page);
            if (data.Count > 0)
            {
                return Success(this, data, "Data fetched successfully", 200, data[0].Pagination);
            }
            return NoData(this);
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
    #endregion

    private IHttpActionResult BadRequest(object value)
	{
		return Content<object>(HttpStatusCode.BadRequest, value);
	}
}
