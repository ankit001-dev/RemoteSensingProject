// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.ApiServices.ReportApiController
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;

[JwtAuthorize(Roles = "admin,projectManager,accounts")]
public class ReportApiController : ApiController
{
	private readonly AdminServices _adminServices;

	private readonly ManagerService _managerservice;

	private readonly AccountService _accountService;

	public ReportApiController()
	{
		_adminServices = new AdminServices();
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
	[Route("api/report/employeereportpdf")]
	public IHttpActionResult GetEmployeeReportPdf(string searchTerm = null, int? devision = null)
	{
		List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
		{
			new ApiCommon.ColumnMapping
			{
				Header = "Employee Code",
				PropertyName = "EmployeeCode"
			},
			new ApiCommon.ColumnMapping
			{
				Header = "Employee Name",
				PropertyName = "EmployeeName"
			},
			new ApiCommon.ColumnMapping
			{
				Header = "Division Name",
				PropertyName = "DevisionName"
			}
		};
		List<RemoteSensingProject.Models.Admin.main.Employee_model> data = _adminServices.SelectEmployeeRecord(null, null, searchTerm, devision);
		byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Employee Report");
		return (IHttpActionResult)(object)new PdfResult(pdfBytes, "EmployeeReport.pdf", ((ApiController)this).Request);
	}

	[HttpGet]
	[Route("api/report/employeereportexcel")]
	public IHttpActionResult GetEmployeeReportExcel(string searchTerm = null, int? devision = null)
	{
		List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
		{
			new ApiCommon.ColumnMapping
			{
				Header = "Employee Code",
				PropertyName = "EmployeeCode"
			},
			new ApiCommon.ColumnMapping
			{
				Header = "Employee Name",
				PropertyName = "EmployeeName"
			},
			new ApiCommon.ColumnMapping
			{
				Header = "Division Name",
				PropertyName = "DevisionName"
			}
		};
		List<RemoteSensingProject.Models.Admin.main.Employee_model> data = _adminServices.SelectEmployeeRecord(null, null, searchTerm, devision);
		byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Employee Report");
		return (IHttpActionResult)(object)new PdfResult(excelBytes, "EmployeeReport.xlsx", ((ApiController)this).Request);
	}

	[HttpGet]
	[Route("api/report/projectreportpdf")]
	public IHttpActionResult AllProjectReportPdf(string searchTerm = null, string statusFilter = null, int? projectManagerFilter = null)
	{
		try
		{
			List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
			{
				new ApiCommon.ColumnMapping
				{
					Header = "Project Code",
					PropertyName = "projectCode"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project Name",
					PropertyName = "ProjectTitle"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project Manager",
					PropertyName = "ProjectManager"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Division",
					PropertyName = "devisionName"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Assign Date",
					PropertyName = "AssignDateString"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Start Date",
					PropertyName = "StartDateString"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Completion Date",
					PropertyName = "CompletionDatestring"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Physical in %",
					PropertyName = "physicalcomplete"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Financial in %",
					PropertyName = "Percentage"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Overall in %",
					PropertyName = "overallPercentage"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project Status",
					PropertyName = "projectStatusLabel"
				}
			};
			IEnumerable<RemoteSensingProject.Models.Admin.main.Project_model> data = null;
			string role = getRole();
			if (role.Equals("admin"))
			{
				AdminServices adminServices = _adminServices;
				int? projectManager = projectManagerFilter;
				data = adminServices.Project_List(null, null, null, searchTerm, statusFilter, projectManager);
			}
			else
			{
				if (!role.Equals("projectManager"))
				{
					return Content<string>(HttpStatusCode.Forbidden, role + "You do not have permission to access this resource.");
				}
				ClaimsIdentity identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
				int userId = int.Parse(identity.Claims.FirstOrDefault((Claim c) => c.Type == "userId")?.Value);
				data = _managerservice.All_Project_List(userId, null, null, null, null, searchTerm, statusFilter);
			}
			byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Project Report");
			return (IHttpActionResult)(object)new PdfResult(pdfBytes, "ProjectsReport.pdf", ((ApiController)this).Request);
		}
		catch (Exception ex)
		{
			return InternalServerError(new Exception("An error occurred while generating the project report: " + ex.Message));
		}
	}

	[HttpGet]
	[Route("api/report/projectreportexcel")]
	public IHttpActionResult AllProjectReportExcel(string searchTerm = null, string statusFilter = null, int? projectManagerFilter = null)
	{
		try
		{
			List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
			{
				new ApiCommon.ColumnMapping
				{
					Header = "Project Code",
					PropertyName = "projectCode"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project Name",
					PropertyName = "ProjectTitle"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project Manager",
					PropertyName = "ProjectManager"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Division",
					PropertyName = "devisionName"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Assign Date",
					PropertyName = "AssignDateString"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Start Date",
					PropertyName = "StartDateString"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Completion Date",
					PropertyName = "CompletionDatestring"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Physical in %",
					PropertyName = "physicalcomplete"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Financial in %",
					PropertyName = "Percentage"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Overall in %",
					PropertyName = "overallPercentage"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project Status",
					PropertyName = "projectStatusLabel"
				}
			};
			IEnumerable<RemoteSensingProject.Models.Admin.main.Project_model> data = null;
			string role = getRole();
			if (role.Equals("admin"))
			{
				AdminServices adminServices = _adminServices;
				int? projectManager = projectManagerFilter;
				data = adminServices.Project_List(null, null, null, searchTerm, statusFilter, projectManager);
			}
			else
			{
				if (!role.Equals("projectManager"))
				{
					return Content<string>(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
				}
				ClaimsIdentity identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
				int userId = int.Parse(identity.Claims.FirstOrDefault((Claim c) => c.Type == "userId")?.Value);
				data = _managerservice.All_Project_List(userId, null, null, null, null, searchTerm, statusFilter);
			}
			byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Project Report");
			return (IHttpActionResult)(object)new PdfResult(excelBytes, "ProjectsReport.xlsx", ((ApiController)this).Request);
		}
		catch (Exception ex)
		{
			return InternalServerError(new Exception("An error occurred while generating the report: " + ex.Message));
		}
	}

	[HttpGet]
	[Route("api/report/meetingreportpdf")]
	public IHttpActionResult MeetingReportPdf(string searchTerm = null, string statusFilter = null, string meetingMode = null)
	{
		List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
		{
			new ApiCommon.ColumnMapping
			{
				Header = "Meeting Name",
				PropertyName = "MeetingTitle"
			},
			new ApiCommon.ColumnMapping
			{
				Header = "Meeting Mode",
				PropertyName = "MeetingType"
			},
			new ApiCommon.ColumnMapping
			{
				Header = "Meeting Date",
				PropertyName = "MeetingDate"
			},
			new ApiCommon.ColumnMapping
			{
				Header = "Status",
				PropertyName = "statusLabel"
			}
		};
		List<RemoteSensingProject.Models.Admin.main.Meeting_Model> data = _adminServices.getAllmeeting(null, null, searchTerm, statusFilter, meetingMode);
		byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Meeting Report");
		return (IHttpActionResult)(object)new PdfResult(pdfBytes, "MeetingReport.pdf", ((ApiController)this).Request);
	}

	[HttpGet]
	[Route("api/report/meetingreportexcel")]
	public IHttpActionResult MeetingReportExcel(string searchTerm = null, string statusFilter = null, string meetingMode = null)
	{
		List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
		{
			new ApiCommon.ColumnMapping
			{
				Header = "Meeting Name",
				PropertyName = "MeetingTitle"
			},
			new ApiCommon.ColumnMapping
			{
				Header = "Meeting Mode",
				PropertyName = "MeetingType"
			},
			new ApiCommon.ColumnMapping
			{
				Header = "Meeting Date",
				PropertyName = "MeetingDate"
			},
			new ApiCommon.ColumnMapping
			{
				Header = "Status",
				PropertyName = "statusLabel"
			}
		};
		List<RemoteSensingProject.Models.Admin.main.Meeting_Model> data = _adminServices.getAllmeeting(null, null, searchTerm, statusFilter, meetingMode);
		byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Meeting Report");
		return (IHttpActionResult)(object)new PdfResult(excelBytes, "MeetingReport.xlsx", ((ApiController)this).Request);
	}

	[HttpGet]
	[Route("api/report/Expensereportpdf")]
	public IHttpActionResult ExpenseReportPdf(int projectId, int headId)
	{
		List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
		{
			new ApiCommon.ColumnMapping
			{
				Header = "Title",
				PropertyName = "title"
			},
			new ApiCommon.ColumnMapping
			{
				Header = "Date",
				PropertyName = "DateString"
			},
			new ApiCommon.ColumnMapping
			{
				Header = "Description",
				PropertyName = "description"
			},
			new ApiCommon.ColumnMapping
			{
				Header = "Amount",
				PropertyName = "amount"
			}
		};
		List<ProjectExpenses> data = _managerservice.ExpencesList(headId, projectId);
		byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Expense Report");
		return (IHttpActionResult)(object)new PdfResult(pdfBytes, "ExpenseReport.pdf", ((ApiController)this).Request);
	}

	[HttpGet]
	[Route("api/report/Expensereportexcel")]
	public IHttpActionResult ExpenseReportExcel(int projectId, int headId)
	{
		List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
		{
			new ApiCommon.ColumnMapping
			{
				Header = "Title",
				PropertyName = "title"
			},
			new ApiCommon.ColumnMapping
			{
				Header = "Date",
				PropertyName = "DateString"
			},
			new ApiCommon.ColumnMapping
			{
				Header = "Description",
				PropertyName = "description"
			},
			new ApiCommon.ColumnMapping
			{
				Header = "Amount",
				PropertyName = "amount"
			}
		};
		List<ProjectExpenses> data = _managerservice.ExpencesList(headId, projectId);
		byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Expense Report");
		return (IHttpActionResult)(object)new PdfResult(excelBytes, "ExpenseReport.xlsx", ((ApiController)this).Request);
	}

	[HttpGet]
	[Route("api/report/Reimbursementreportpdf")]
	public IHttpActionResult ReimbursementReportPdf(int? projectManagerFilter = null, string typeFilter = null, string statusFilter = null)
	{
		try
		{
			List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
			{
				new ApiCommon.ColumnMapping
				{
					Header = "Project Manager",
					PropertyName = "EmpName"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Type",
					PropertyName = "type"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Amount",
					PropertyName = "amount"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Approval Amt",
					PropertyName = "approveAmount"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Remark",
					PropertyName = "remark"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Status",
					PropertyName = "statusLabel"
				}
			};
			IEnumerable<Reimbursement> data = null;
			string role = getRole();
			if (role.Equals("admin"))
			{
				ManagerService managerservice = _managerservice;
				int? managerId = projectManagerFilter;
				string typeFilter2 = typeFilter;
				string statusFilter2 = statusFilter;
				data = managerservice.GetReimbursements(null, null, null, managerId, "selectReinbursementReport", typeFilter2, statusFilter2);
			}
			else if (role.Equals("projectManager"))
			{
				ClaimsIdentity identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
				int userId = int.Parse(identity.Claims.FirstOrDefault((Claim c) => c.Type == "userId")?.Value);
				ManagerService managerservice2 = _managerservice;
				int? managerId = userId;
				string statusFilter2 = statusFilter;
				string typeFilter2 = typeFilter;
				data = managerservice2.GetReimbursements(null, null, null, managerId, "selectReinbursementforUserReport", typeFilter2, statusFilter2);
			}
			else
			{
				if (!role.Equals("accounts"))
				{
					return Content<string>(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
				}
				ManagerService managerservice3 = _managerservice;
				int? managerId = projectManagerFilter;
				string typeFilter2 = typeFilter;
				string statusFilter2 = statusFilter;
				data = managerservice3.GetReimbursements(null, null, null, managerId, "accountrepo", typeFilter2, statusFilter2);
			}
			byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Reimbursement Report");
			return (IHttpActionResult)(object)new PdfResult(pdfBytes, "ReimbursementReport.pdf", ((ApiController)this).Request);
		}
		catch (Exception ex)
		{
			return InternalServerError(new Exception("An error occurred while generating the report: " + ex.Message));
		}
	}

	[HttpGet]
	[Route("api/report/Reimbursementreportexcel")]
	public IHttpActionResult ReimbursementReportExcel(int? projectManagerFilter = null, string typeFilter = null, string statusFilter = null)
	{
		try
		{
			List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
			{
				new ApiCommon.ColumnMapping
				{
					Header = "Project Manager",
					PropertyName = "EmpName"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Type",
					PropertyName = "type"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Amount",
					PropertyName = "amount"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Approval Amt",
					PropertyName = "approveAmount"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Remark",
					PropertyName = "remark"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Status",
					PropertyName = "statusLabel"
				}
			};
			IEnumerable<Reimbursement> data = null;
			string role = getRole();
			if (role.Equals("admin"))
			{
				ManagerService managerservice = _managerservice;
				int? managerId = projectManagerFilter;
				string typeFilter2 = typeFilter;
				string statusFilter2 = statusFilter;
				data = managerservice.GetReimbursements(null, null, null, managerId, "selectReinbursementReport", typeFilter2, statusFilter2);
			}
			else if (role.Equals("projectManager"))
			{
				ClaimsIdentity identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
				int userId = int.Parse(identity.Claims.FirstOrDefault((Claim c) => c.Type == "userId")?.Value);
				ManagerService managerservice2 = _managerservice;
				int? managerId = userId;
				string statusFilter2 = statusFilter;
				string typeFilter2 = typeFilter;
				data = managerservice2.GetReimbursements(null, null, null, managerId, "selectReinbursementforUserReport", typeFilter2, statusFilter2);
			}
			else
			{
				if (!role.Equals("accounts"))
				{
					return Content<string>(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
				}
				ManagerService managerservice3 = _managerservice;
				int? managerId = projectManagerFilter;
				string typeFilter2 = typeFilter;
				string statusFilter2 = statusFilter;
				data = managerservice3.GetReimbursements(null, null, null, managerId, "accountrepo", typeFilter2, statusFilter2);
			}
			byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Reimbursement Report");
			return (IHttpActionResult)(object)new PdfResult(pdfBytes, "ReimbursementReport.pdf", ((ApiController)this).Request);
		}
		catch (Exception ex)
		{
			return InternalServerError(new Exception("An error occurred while generating the report: " + ex.Message));
		}
	}

	[HttpGet]
	[Route("api/report/Tourproposalreportpdf")]
	public IHttpActionResult TourproposalReportPdf(int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
	{
		try
		{
			List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
			{
				new ApiCommon.ColumnMapping
				{
					Header = "Project Manager",
					PropertyName = "projectManager"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project Code",
					PropertyName = "projectCode"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project",
					PropertyName = "projectName"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Purpose",
					PropertyName = "purpose"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Date of Depart",
					PropertyName = "dateOfDept"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Date of Return",
					PropertyName = "returnDate"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Place Visit",
					PropertyName = "place"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Period From",
					PropertyName = "periodFrom"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Period To",
					PropertyName = "periodTo"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Remark",
					PropertyName = "remark"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Status",
					PropertyName = "statusLabel"
				}
			};
			dynamic data = null;
			string role = getRole();
			if (role.Equals("admin") || role.Equals("accounts"))
			{
				AccountService accountService = _accountService;
				int? managerFilter2 = managerFilter;
				int? projectFilter2 = projectFilter;
				string statusFilter2 = statusFilter;
				data = accountService.getTourList(null, null, managerFilter2, projectFilter2, statusFilter2);
			}
			else
			{
				if (!role.Equals("projectManager"))
				{
					return Content<string>(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
				}
				ClaimsIdentity identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
				int userId = int.Parse(identity.Claims.FirstOrDefault((Claim c) => c.Type == "userId")?.Value);
				ManagerService managerservice = _managerservice;
				data = managerservice.GetTourList(type:"ALLDATA",projectFilter:projectFilter);
			}
			byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Tourproposal Report");
			return (IHttpActionResult)(object)new PdfResult(pdfBytes, "TourproposalReport.pdf", ((ApiController)this).Request);
		}
		catch (Exception ex)
		{
			return InternalServerError(new Exception("An error occurred while generating the report: " + ex.Message));
		}
	}

	[HttpGet]
	[Route("api/report/Tourproposalreportexcel")]
	public IHttpActionResult TourproposalReportExcel(int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
	{
		try
		{
			List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
			{
				new ApiCommon.ColumnMapping
				{
					Header = "Project Manager",
					PropertyName = "projectManager"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project Code",
					PropertyName = "projectCode"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project",
					PropertyName = "projectName"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Purpose",
					PropertyName = "purpose"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Date of Depart",
					PropertyName = "dateOfDept"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Date of Return",
					PropertyName = "returnDate"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Place Visit",
					PropertyName = "place"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Period From",
					PropertyName = "periodFrom"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Period To",
					PropertyName = "periodTo"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Remark",
					PropertyName = "remark"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Status",
					PropertyName = "statusLabel"
				}
			};
			dynamic data = null;
			string role = getRole();
			if (role.Equals("admin") || role.Equals("accounts"))
			{
				AccountService accountService = _accountService;
				int? managerFilter2 = managerFilter;
				int? projectFilter2 = projectFilter;
				string statusFilter2 = statusFilter;
				data = accountService.getTourList(null, null, managerFilter2, projectFilter2, statusFilter2);
			}
			else
			{
				if (!role.Equals("projectManager"))
				{
					return Content<string>(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
				}
				ClaimsIdentity identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
				int userId = int.Parse(identity.Claims.FirstOrDefault((Claim c) => c.Type == "userId")?.Value);
				ManagerService managerservice = _managerservice;
				data = managerservice.GetTourList(type:"ALLDATA",projectFilter:projectFilter);
			}
			byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Tourproposal Report");
			return (IHttpActionResult)(object)new PdfResult(excelBytes, "TourproposalReport.xlsx", ((ApiController)this).Request);
		}
		catch (Exception ex)
		{
			return InternalServerError(new Exception("An error occurred while generating the report: " + ex.Message));
		}
	}

	[HttpGet]
	[Route("api/report/Hiringreportpdf")]
	public IHttpActionResult HiringReportPdf(int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
	{
		try
		{
			List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
			{
				new ApiCommon.ColumnMapping
				{
					Header = "Project Manager",
					PropertyName = "projectManager"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project Code",
					PropertyName = "projectCode"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project",
					PropertyName = "projectName"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Head Name",
					PropertyName = "headName"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Date From",
					PropertyName = "dateFrom"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Date To",
					PropertyName = "dateTo"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Proposed Place",
					PropertyName = "proposedPlace"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Purpose of Visit",
					PropertyName = "purposeOfVisit"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Total Day & Night",
					PropertyName = "totalDaysNight"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Availability of Fund",
					PropertyName = "availbilityOfFund"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Remark",
					PropertyName = "remark"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Status",
					PropertyName = "statusLabel"
				}
			};
			dynamic data = null;
			string role = getRole();
			if (role.Equals("admin") || role.Equals("accounts"))
			{
				AdminServices adminServices = _adminServices;
				int? managerFilter2 = managerFilter;
				int? projectFilter2 = projectFilter;
				string statusFilter2 = statusFilter;
				data = adminServices.HiringReort(null, null, managerFilter2, projectFilter2, statusFilter2);
			}
			else
			{
				if (!role.Equals("projectManager"))
				{
					return Content<string>(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
				}
				ClaimsIdentity identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
				int userId = int.Parse(identity.Claims.FirstOrDefault((Claim c) => c.Type == "userId")?.Value);
				ManagerService managerservice = _managerservice;
				data = managerservice.GetHiringVehicles(type:"GETALLDATA",projectFilter:projectFilter);
			}
			byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Hiring Report");
			return (IHttpActionResult)(object)new PdfResult(pdfBytes, "HiringReport.pdf", ((ApiController)this).Request);
		}
		catch (Exception ex)
		{
			return InternalServerError(new Exception("An error occurred while generating the report: " + ex.Message));
		}
	}

	[HttpGet]
	[Route("api/report/Hiringreportexcel")]
	public IHttpActionResult HiringReportExcel(int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
	{
		try
		{
			List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
			{
				new ApiCommon.ColumnMapping
				{
					Header = "Project Manager",
					PropertyName = "projectManager"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project Code",
					PropertyName = "projectCode"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project",
					PropertyName = "projectName"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Head Name",
					PropertyName = "headName"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Date From",
					PropertyName = "dateFrom"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Date To",
					PropertyName = "dateTo"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Proposed Place",
					PropertyName = "proposedPlace"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Purpose of Visit",
					PropertyName = "purposeOfVisit"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Total Day & Night",
					PropertyName = "totalDaysNight"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Availability of Fund",
					PropertyName = "availbilityOfFund"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Remark",
					PropertyName = "remark"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Status",
					PropertyName = "statusLabel"
				}
			};
			dynamic data = null;
			string role = getRole();
			if (role.Equals("admin"))
			{
				AdminServices adminServices = _adminServices;
				int? managerFilter2 = managerFilter;
				int? projectFilter2 = projectFilter;
				string statusFilter2 = statusFilter;
				data = adminServices.HiringReort(null, null, managerFilter2, projectFilter2, statusFilter2);
			}
			else
			{
				if (!role.Equals("projectManager"))
				{
					return Content<string>(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
				}
				ClaimsIdentity identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
				int userId = int.Parse(identity.Claims.FirstOrDefault((Claim c) => c.Type == "userId")?.Value);
				ManagerService managerservice = _managerservice;
				data = managerservice.GetHiringVehicles(type:"GETALLDATA",projectFilter:projectFilter);
			}
			byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Hiring Report");
			return (IHttpActionResult)(object)new PdfResult(excelBytes, "HiringReport.xlsx", ((ApiController)this).Request);
		}
		catch (Exception ex)
		{
			return InternalServerError(new Exception("An error occurred while generating the report: " + ex.Message));
		}
	}

	[HttpGet]
	[Route("api/report/EmployeeMonthlyreportpdf")]
	public IHttpActionResult EmployeeMonthlyReportPdf(int id, int month, int year)
	{
		ManagerService managerservice = _managerservice;
		int? month2 = month;
		int? year2 = year;
		List<EmpReportModel> data = managerservice.GetEmpReport(id, null, null, null, month2, year2);
		byte[] pdfBytes = ReportGenerator.CreateMonthlyReviewPdf(data, "EmployeeMonthly Report");
		return (IHttpActionResult)(object)new PdfResult(pdfBytes, "EmployeeMonthlyReport.pdf", ((ApiController)this).Request);
	}

	[HttpGet]
	[Route("api/report/Fundreportpdf")]
	public IHttpActionResult FundReportPdf(string statusFilter = null)
	{
		try
		{
			List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
			{
				new ApiCommon.ColumnMapping
				{
					Header = "Project Manager",
					PropertyName = "projectManager"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project Code",
					PropertyName = "projectCode"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project",
					PropertyName = "projectName"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Head Name",
					PropertyName = "headName"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Date From",
					PropertyName = "dateFrom"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Date To",
					PropertyName = "dateTo"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Proposed Place",
					PropertyName = "proposedPlace"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Purpose of Visit",
					PropertyName = "purposeOfVisit"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Total Day & Night",
					PropertyName = "totalDaysNight"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Availability of Fund",
					PropertyName = "availbilityOfFund"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Remark",
					PropertyName = "remark"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Status",
					PropertyName = "statusLabel"
				}
			};
			dynamic data = null;
			string role = getRole();
			if (role.Equals("admin"))
			{
				data = _managerservice.All_Project_List(0);
				if (!string.IsNullOrWhiteSpace(statusFilter))
				{
					if (statusFilter.ToLower().Equals("complete"))
					{
						data = _managerservice.All_Project_List(0, null, null, "AccountApproved");
					}
					else if (statusFilter.ToLower().Equals("pending"))
					{
						data = _managerservice.All_Project_List(0, null, null, "AccountPending");
					}
				}
			}
			else
			{
				if (!role.Equals("projectManager"))
				{
					return Content<string>(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
				}
				ClaimsIdentity identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
				int userId = int.Parse(identity.Claims.FirstOrDefault((Claim c) => c.Type == "userId")?.Value);
				data = _managerservice.All_Project_List(0);
				if (!string.IsNullOrWhiteSpace(statusFilter))
				{
					if (statusFilter.ToLower().Equals("complete"))
					{
						data = _managerservice.All_Project_List(0, null, null, "AccountApproved");
					}
					else if (statusFilter.ToLower().Equals("pending"))
					{
						data = _managerservice.All_Project_List(0, null, null, "AccountPending");
					}
				}
			}
			byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Fund Report");
			return (IHttpActionResult)(object)new PdfResult(pdfBytes, "FundReport.pdf", ((ApiController)this).Request);
		}
		catch (Exception ex)
		{
			return InternalServerError(new Exception("An error occurred while generating the report: " + ex.Message));
		}
	}

	[HttpGet]
	[Route("api/report/Fundreportexcel")]
	public IHttpActionResult FundReportExcel(int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
	{
		try
		{
			List<ApiCommon.ColumnMapping> columnMappings = new List<ApiCommon.ColumnMapping>
			{
				new ApiCommon.ColumnMapping
				{
					Header = "Project Manager",
					PropertyName = "projectManager"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project Code",
					PropertyName = "projectCode"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Project",
					PropertyName = "projectName"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Head Name",
					PropertyName = "headName"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Date From",
					PropertyName = "dateFrom"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Date To",
					PropertyName = "dateTo"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Proposed Place",
					PropertyName = "proposedPlace"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Purpose of Visit",
					PropertyName = "purposeOfVisit"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Total Day & Night",
					PropertyName = "totalDaysNight"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Availability of Fund",
					PropertyName = "availbilityOfFund"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Remark",
					PropertyName = "remark"
				},
				new ApiCommon.ColumnMapping
				{
					Header = "Status",
					PropertyName = "statusLabel"
				}
			};
			dynamic data = null;
			string role = getRole();
			if (role.Equals("admin"))
			{
				data = _managerservice.All_Project_List(0);
				if (!string.IsNullOrWhiteSpace(statusFilter))
				{
					if (statusFilter.ToLower().Equals("complete"))
					{
						data = _managerservice.All_Project_List(0, null, null, "AccountApproved");
					}
					else if (statusFilter.ToLower().Equals("pending"))
					{
						data = _managerservice.All_Project_List(0, null, null, "AccountPending");
					}
				}
			}
			else
			{
				if (!role.Equals("projectManager"))
				{
					return Content<string>(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
				}
				ClaimsIdentity identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
				int userId = int.Parse(identity.Claims.FirstOrDefault((Claim c) => c.Type == "userId")?.Value);
				data = _managerservice.All_Project_List(0);
				if (!string.IsNullOrWhiteSpace(statusFilter))
				{
					if (statusFilter.ToLower().Equals("complete"))
					{
						data = _managerservice.All_Project_List(0, null, null, "AccountApproved");
					}
					else if (statusFilter.ToLower().Equals("pending"))
					{
						data = _managerservice.All_Project_List(0, null, null, "AccountPending");
					}
				}
			}
			byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Fund Report");
			return (IHttpActionResult)(object)new PdfResult(excelBytes, "FundReport.xlsx", ((ApiController)this).Request);
		}
		catch (Exception ex)
		{
			return InternalServerError(new Exception("An error occurred while generating the report: " + ex.Message));
		}
	}

	[HttpGet]
	[Route("api/report/attendancereportexcel")]
	public IHttpActionResult AttendanceReportExcel(int month, int year, int projectManager, int? EmpId = null)
	{
		try
		{
			dynamic data = null;
			data = ((!EmpId.HasValue) ? _managerservice.ConvertExcelFileOfAll(month, year, projectManager) : _managerservice.ConvertExcelFile(month, year, projectManager, Convert.ToInt32(EmpId)));
			return (IHttpActionResult)(object)new PdfResult(data, "AttendanceReport.xlsx", ((ApiController)this).Request);
		}
		catch (Exception ex)
		{
			return InternalServerError(new Exception("An error occurred while generating the report: " + ex.Message));
		}
	}
}
