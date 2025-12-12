using Antlr.Runtime.Tree;
using DocumentFormat.OpenXml.Spreadsheet;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;
using static RemoteSensingProject.Models.Admin.main;
using static RemoteSensingProject.Models.ApiCommon;

namespace RemoteSensingProject.ApiServices
{
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
            var identity = (ClaimsIdentity)System.Web.HttpContext.Current.User.Identity;
            var role = identity.Claims
                .FirstOrDefault(c =>
                    c.Type == ClaimTypes.Role ||
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                ?.Value;

            return string.IsNullOrEmpty(role)
                ? "Role claim not found in token"
                : role;
        }
        #region Admin Reports
        #region Employee Reports
        [HttpGet]
        [Route("api/report/employeereportpdf")]
        public IHttpActionResult GetEmployeeReportPdf(string searchTerm = null, int? devision = null)
        {
            var columnMappings = new List<ColumnMapping>
            {
                new ColumnMapping { Header = "Employee Code", PropertyName = "EmployeeCode" },
                new ColumnMapping { Header = "Employee Name", PropertyName = "EmployeeName" },
                new ColumnMapping { Header = "Division Name", PropertyName = "DevisionName" }
            };

            var data = _adminServices.SelectEmployeeRecord(searchTerm: searchTerm, devision: devision);
            byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Employee Report");

            return new PdfResult(pdfBytes, "EmployeeReport.pdf", Request);
        }
        [HttpGet]
        [Route("api/report/employeereportexcel")]
        public IHttpActionResult GetEmployeeReportExcel(string searchTerm = null, int? devision = null)
        {
            var columnMappings = new List<ColumnMapping>
            {
                new ColumnMapping { Header = "Employee Code", PropertyName = "EmployeeCode" },
                new ColumnMapping { Header = "Employee Name", PropertyName = "EmployeeName" },
                new ColumnMapping { Header = "Division Name", PropertyName = "DevisionName" }
            };

            var data = _adminServices.SelectEmployeeRecord(searchTerm: searchTerm, devision: devision);
            byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Employee Report");

            return new PdfResult(excelBytes, "EmployeeReport.xlsx", Request);
        }
        #endregion

        #region Project Reports
        [HttpGet]
        [Route("api/report/projectreportpdf")]
        public IHttpActionResult AllProjectReportPdf(string searchTerm = null, string statusFilter = null, int? projectManagerFilter = null)
        {
            try
            {
                var columnMappings = new List<ColumnMapping>
            {
                new ColumnMapping { Header = "Project Code", PropertyName = "projectCode" },
                new ColumnMapping { Header = "Project Name", PropertyName = "ProjectTitle" },
                new ColumnMapping { Header = "Project Manager", PropertyName = "ProjectManager" },
                new ColumnMapping { Header = "Assign Date", PropertyName = "AssignDateString" },
                new ColumnMapping { Header = "Start Date", PropertyName = "StartDateString" },
                new ColumnMapping { Header = "Completion Date", PropertyName = "CompletionDatestring" },
                new ColumnMapping { Header = "Physical in %", PropertyName = "physicalcomplete" },
                new ColumnMapping { Header = "Financial in %", PropertyName = "Percentage" },
                new ColumnMapping { Header = "Overall in %", PropertyName = "overallPercentage" },
                new ColumnMapping { Header = "Project Status", PropertyName = "projectStatusLabel" }
            };
                IEnumerable<Project_model> data = null;
                string role = getRole();
                if (role.Equals("admin"))
                {
                    data = _adminServices.Project_List(searchTerm: searchTerm, statusFilter: statusFilter, projectManager: projectManagerFilter);
                }
                else if (role.Equals("projectManager"))
                {
                    var identity = (ClaimsIdentity)System.Web.HttpContext.Current.User.Identity;
                    int userId = int.Parse(identity.Claims
                .FirstOrDefault(c =>
                    c.Type == "userId")
                ?.Value);
                    data = _managerservice.All_Project_List(userId, null, null, null, null, searchTerm: searchTerm, statusFilter: statusFilter);
                }
                else
                {
                    return Content(HttpStatusCode.Forbidden, $"{role}You do not have permission to access this resource.");
                }
                byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Project Report");

                return new PdfResult(pdfBytes, "ProjectsReport.pdf", Request);
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
                var columnMappings = new List<ColumnMapping>
            {
                new ColumnMapping { Header = "Project Code", PropertyName = "projectCode" },
                new ColumnMapping { Header = "Project Name", PropertyName = "ProjectTitle" },
                new ColumnMapping { Header = "Project Manager", PropertyName = "ProjectManager" },
                new ColumnMapping { Header = "Assign Date", PropertyName = "AssignDateString" },
                new ColumnMapping { Header = "Start Date", PropertyName = "StartDateString" },
                new ColumnMapping { Header = "Completion Date", PropertyName = "CompletionDatestring" },
                new ColumnMapping { Header = "Physical in %", PropertyName = "physicalcomplete" },
                new ColumnMapping { Header = "Financial in %", PropertyName = "Percentage" },
                new ColumnMapping { Header = "Overall in %", PropertyName = "overallPercentage" },
                new ColumnMapping { Header = "Project Status", PropertyName = "projectStatusLabel" }
            };

                IEnumerable<Project_model> data = null;
                string role = getRole();
                if (role.Equals("admin"))
                {
                    data = _adminServices.Project_List(searchTerm: searchTerm, statusFilter: statusFilter, projectManager: projectManagerFilter);
                }
                else if (role.Equals("projectManager"))
                {
                    var identity = (ClaimsIdentity)System.Web.HttpContext.Current.User.Identity;
                    int userId = int.Parse(identity.Claims
                .FirstOrDefault(c =>
                    c.Type == "userId")
                ?.Value);
                    data = _managerservice.All_Project_List(userId, null, null, null, null, searchTerm: searchTerm, statusFilter: statusFilter);
                }
                else
                {
                    return Content(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
                }
                byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Project Report");

                return new PdfResult(excelBytes, "ProjectsReport.xlsx", Request);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("An error occurred while generating the report: " + ex.Message));
            }
        }
        #endregion

        #region Meeting Reports
        [HttpGet]
        [Route("api/report/meetingreportpdf")]
        public IHttpActionResult MeetingReportPdf(string searchTerm = null, string statusFilter = null, string meetingMode = null)
        {
            var columnMappings = new List<ColumnMapping>
            {
                new ColumnMapping { Header = "Meeting Name", PropertyName = "MeetingTitle" },
                new ColumnMapping { Header = "Meeting Mode", PropertyName = "MeetingType" },
                new ColumnMapping { Header = "Meeting Date", PropertyName = "MeetingDate" },
                new ColumnMapping { Header = "Status", PropertyName = "statusLabel" },
            };

            var data = _adminServices.getAllmeeting(searchTerm: searchTerm, statusFilter: statusFilter, meetingMode: meetingMode);
            byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Meeting Report");

            return new PdfResult(pdfBytes, "MeetingReport.pdf", Request);
        }
        [HttpGet]
        [Route("api/report/meetingreportexcel")]
        public IHttpActionResult MeetingReportExcel(string searchTerm = null, string statusFilter = null, string meetingMode = null)
        {
            var columnMappings = new List<ColumnMapping>
            {
                new ColumnMapping { Header = "Meeting Name", PropertyName = "MeetingTitle" },
                new ColumnMapping { Header = "Meeting Mode", PropertyName = "MeetingType" },
                new ColumnMapping { Header = "Meeting Date", PropertyName = "MeetingDate" },
                new ColumnMapping { Header = "Status", PropertyName = "statusLabel" },
            };

            var data = _adminServices.getAllmeeting(searchTerm: searchTerm, statusFilter: statusFilter, meetingMode: meetingMode);
            byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Meeting Report");

            return new PdfResult(excelBytes, "MeetingReport.xlsx", Request);
        }
        #endregion

        #region Expense Reports
        [HttpGet]
        [Route("api/report/Expensereportpdf")]
        public IHttpActionResult ExpenseReportPdf(int projectId, int headId)
        {
            var columnMappings = new List<ColumnMapping>
            {
                new ColumnMapping { Header = "Title", PropertyName = "title" },
                new ColumnMapping { Header = "Date", PropertyName = "DateString" },
                new ColumnMapping { Header = "Description", PropertyName = "description" },
                new ColumnMapping { Header = "Amount", PropertyName = "amount" },
            };

            var data = _managerservice.ExpencesList(headId, projectId);
            byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Expense Report");

            return new PdfResult(pdfBytes, "ExpenseReport.pdf", Request);
        }
        [HttpGet]
        [Route("api/report/Expensereportexcel")]
        public IHttpActionResult ExpenseReportExcel(int projectId, int headId)
        {
            var columnMappings = new List<ColumnMapping>
            {
                new ColumnMapping { Header = "Title", PropertyName = "title" },
                new ColumnMapping { Header = "Date", PropertyName = "DateString" },
                new ColumnMapping { Header = "Description", PropertyName = "description" },
                new ColumnMapping { Header = "Amount", PropertyName = "amount" },
            };

            var data = _managerservice.ExpencesList(headId, projectId);
            byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Expense Report");

            return new PdfResult(excelBytes, "ExpenseReport.xlsx", Request);
        }
        #endregion

        #region Reimbursement Reports
        [HttpGet]
        [Route("api/report/Reimbursementreportpdf")]
        public IHttpActionResult ReimbursementReportPdf(int? projectManagerFilter = null, string typeFilter = null, string statusFilter = null)
        {
            try
            {
                var columnMappings = new List<ColumnMapping>
                {
                    new ColumnMapping { Header = "Project Manager", PropertyName = "EmpName" },
                    new ColumnMapping { Header = "Type", PropertyName = "type" },
                    new ColumnMapping { Header = "Amount", PropertyName = "amount" },
                    new ColumnMapping { Header = "Approval Amt", PropertyName = "approveAmount" },
                    new ColumnMapping { Header = "Remark", PropertyName = "remark" },
                    new ColumnMapping { Header = "Status", PropertyName = "statusLabel" },
                };
                IEnumerable<Reimbursement> data = null;
                string role = getRole();
                if (role.Equals("admin"))
                {
                    data = _managerservice.GetReimbursements(type: "selectReinbursementReport", managerId: projectManagerFilter, typeFilter: typeFilter, statusFilter: statusFilter);
                }
                else if (role.Equals("projectManager"))
                {
                    var identity = (ClaimsIdentity)System.Web.HttpContext.Current.User.Identity;
                    int userId = int.Parse(identity.Claims
                .FirstOrDefault(c =>
                    c.Type == "userId")
                ?.Value);
                    data = _managerservice.GetReimbursements(managerId: userId, type: "selectReinbursementforUserReport", statusFilter: statusFilter, typeFilter: typeFilter);
                }
                else if (role.Equals("accounts"))
                {
                    data = _managerservice.GetReimbursements(type: "accountrepo", managerId: projectManagerFilter, typeFilter: typeFilter, statusFilter: statusFilter);
                }
                else
                {
                    return Content(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
                }
                byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Reimbursement Report");

                return new PdfResult(pdfBytes, "ReimbursementReport.pdf", Request);
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
                var columnMappings = new List<ColumnMapping>
                {
                    new ColumnMapping { Header = "Project Manager", PropertyName = "EmpName" },
                    new ColumnMapping { Header = "Type", PropertyName = "type" },
                    new ColumnMapping { Header = "Amount", PropertyName = "amount" },
                    new ColumnMapping { Header = "Approval Amt", PropertyName = "approveAmount" },
                    new ColumnMapping { Header = "Remark", PropertyName = "remark" },
                    new ColumnMapping { Header = "Status", PropertyName = "statusLabel" },
                };

                IEnumerable<Reimbursement> data = null;
                string role = getRole();
                if (role.Equals("admin"))
                {
                    data = _managerservice.GetReimbursements(type: "selectReinbursementReport", managerId: projectManagerFilter, typeFilter: typeFilter, statusFilter: statusFilter);
                }
                else if (role.Equals("projectManager"))
                {
                    var identity = (ClaimsIdentity)System.Web.HttpContext.Current.User.Identity;
                    int userId = int.Parse(identity.Claims
                .FirstOrDefault(c =>
                    c.Type == "userId")
                ?.Value);
                    data = _managerservice.GetReimbursements(managerId: userId, type: "selectReinbursementforUserReport", statusFilter: statusFilter, typeFilter: typeFilter);
                }
                else if (role.Equals("accounts"))
                {
                    data = _managerservice.GetReimbursements(type: "accountrepo", managerId: projectManagerFilter, typeFilter: typeFilter, statusFilter: statusFilter);
                }
                else
                {
                    return Content(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
                }
                byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Reimbursement Report");

                return new PdfResult(pdfBytes, "ReimbursementReport.pdf", Request);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("An error occurred while generating the report: " + ex.Message));
            }
        }
        #endregion

        #region Tourproposal Reports
        [HttpGet]
        [Route("api/report/Tourproposalreportpdf")]
        public IHttpActionResult TourproposalReportPdf(int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
        {
            try
            {
                var columnMappings = new List<ColumnMapping>
                {
                    new ColumnMapping { Header = "Project Manager", PropertyName = "projectManager" },
                    new ColumnMapping { Header = "Project Code", PropertyName = "projectCode" },
                    new ColumnMapping { Header = "Project", PropertyName = "projectName" },
                    new ColumnMapping { Header = "Purpose", PropertyName = "purpose" },
                    new ColumnMapping { Header = "Date of Depart", PropertyName = "dateOfDept" },
                    new ColumnMapping { Header = "Date of Return", PropertyName = "returnDate" },
                    new ColumnMapping { Header = "Place Visit", PropertyName = "place" },
                    new ColumnMapping { Header = "Period From", PropertyName = "periodFrom" },
                    new ColumnMapping { Header = "Period To", PropertyName = "periodTo" },
                    new ColumnMapping { Header = "Remark", PropertyName = "remark" },
                    new ColumnMapping { Header = "Status", PropertyName = "statusLabel" },
                };
                dynamic data = null;
                string role = getRole();
                if (role.Equals("admin") || role.Equals("accounts"))
                {
                    data = _accountService.getTourList(managerFilter: managerFilter, projectFilter: projectFilter, statusFilter: statusFilter);
                }
                else if (role.Equals("projectManager"))
                {
                    var identity = (ClaimsIdentity)System.Web.HttpContext.Current.User.Identity;
                    int userId = int.Parse(identity.Claims
                .FirstOrDefault(c =>
                    c.Type == "userId")
                ?.Value);
                    data = _managerservice.getTourList(userId: userId, type: "specificUser", projectFilter: projectFilter, statusFilter: statusFilter);
                }
                else
                {
                    return Content(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
                }
                byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Tourproposal Report");

                return new PdfResult(pdfBytes, "TourproposalReport.pdf", Request);
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
                var columnMappings = new List<ColumnMapping>
                {
                    new ColumnMapping { Header = "Project Manager", PropertyName = "projectManager" },
                    new ColumnMapping { Header = "Project Code", PropertyName = "projectCode" },
                    new ColumnMapping { Header = "Project", PropertyName = "projectName" },
                    new ColumnMapping { Header = "Purpose", PropertyName = "purpose" },
                    new ColumnMapping { Header = "Date of Depart", PropertyName = "dateOfDept" },
                    new ColumnMapping { Header = "Date of Return", PropertyName = "returnDate" },
                    new ColumnMapping { Header = "Place Visit", PropertyName = "place" },
                    new ColumnMapping { Header = "Period From", PropertyName = "periodFrom" },
                    new ColumnMapping { Header = "Period To", PropertyName = "periodTo" },
                    new ColumnMapping { Header = "Remark", PropertyName = "remark" },
                    new ColumnMapping { Header = "Status", PropertyName = "statusLabel" },
                };

                dynamic data = null;
                string role = getRole();
                if (role.Equals("admin") || role.Equals("accounts"))
                {
                    data = _accountService.getTourList(managerFilter: managerFilter, projectFilter: projectFilter, statusFilter: statusFilter);
                }
                else if (role.Equals("projectManager"))
                {
                    var identity = (ClaimsIdentity)System.Web.HttpContext.Current.User.Identity;
                    int userId = int.Parse(identity.Claims
                .FirstOrDefault(c =>
                    c.Type == "userId")
                ?.Value);
                    data = _managerservice.getTourList(userId: userId, type: "specificUser", projectFilter: projectFilter, statusFilter: statusFilter);
                }
                else
                {
                    return Content(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
                }
                byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Tourproposal Report");

                return new PdfResult(excelBytes, "TourproposalReport.xlsx", Request);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("An error occurred while generating the report: " + ex.Message));
            }
        }
        #endregion

        #region Hiring Reports
        [HttpGet]
        [Route("api/report/Hiringreportpdf")]
        public IHttpActionResult HiringReportPdf(int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
        {
            try
            {
                var columnMappings = new List<ColumnMapping>
                {
                    new ColumnMapping { Header = "Project Manager", PropertyName = "projectManager" },
                    new ColumnMapping { Header = "Project Code", PropertyName = "projectCode" },
                    new ColumnMapping { Header = "Project", PropertyName = "projectName" },
                    new ColumnMapping { Header = "Head Name", PropertyName = "headName" },
                    new ColumnMapping { Header = "Date From", PropertyName = "dateFrom" },
                    new ColumnMapping { Header = "Date To", PropertyName = "dateTo" },
                    new ColumnMapping { Header = "Proposed Place", PropertyName = "proposedPlace" },
                    new ColumnMapping { Header = "Purpose of Visit", PropertyName = "purposeOfVisit" },
                    new ColumnMapping { Header = "Total Day & Night", PropertyName = "totalDaysNight" },
                    new ColumnMapping { Header = "Availability of Fund", PropertyName = "availbilityOfFund" },
                    new ColumnMapping { Header = "Remark", PropertyName = "remark" },
                    new ColumnMapping { Header = "Status", PropertyName = "statusLabel" },
                };

                dynamic data = null;
                string role = getRole();
                if (role.Equals("admin") || role.Equals("accounts"))
                {
                    data = _adminServices.HiringReort(managerFilter: managerFilter, projectFilter: projectFilter, statusFilter: statusFilter);
                }
                else if (role.Equals("projectManager"))
                {
                    var identity = (ClaimsIdentity)System.Web.HttpContext.Current.User.Identity;
                    int userId = int.Parse(identity.Claims
                .FirstOrDefault(c =>
                    c.Type == "userId")
                ?.Value);
                    data = _managerservice.GetHiringVehicles(userId: userId, type: "projectManager", projectFilter: projectFilter, statusFilter: statusFilter);
                }
                else
                {
                    return Content(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
                }
                byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Hiring Report");

                return new PdfResult(pdfBytes, "HiringReport.pdf", Request);
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
                var columnMappings = new List<ColumnMapping>
                {
                    new ColumnMapping { Header = "Project Manager", PropertyName = "projectManager" },
                    new ColumnMapping { Header = "Project Code", PropertyName = "projectCode" },
                    new ColumnMapping { Header = "Project", PropertyName = "projectName" },
                    new ColumnMapping { Header = "Head Name", PropertyName = "headName" },
                    new ColumnMapping { Header = "Date From", PropertyName = "dateFrom" },
                    new ColumnMapping { Header = "Date To", PropertyName = "dateTo" },
                    new ColumnMapping { Header = "Proposed Place", PropertyName = "proposedPlace" },
                    new ColumnMapping { Header = "Purpose of Visit", PropertyName = "purposeOfVisit" },
                    new ColumnMapping { Header = "Total Day & Night", PropertyName = "totalDaysNight" },
                    new ColumnMapping { Header = "Availability of Fund", PropertyName = "availbilityOfFund" },
                    new ColumnMapping { Header = "Remark", PropertyName = "remark" },
                    new ColumnMapping { Header = "Status", PropertyName = "statusLabel" },
                };

                dynamic data = null;
                string role = getRole();
                if (role.Equals("admin"))
                {
                    data = _adminServices.HiringReort(managerFilter: managerFilter, projectFilter: projectFilter, statusFilter: statusFilter);
                }
                else if (role.Equals("projectManager"))
                {
                    var identity = (ClaimsIdentity)System.Web.HttpContext.Current.User.Identity;
                    int userId = int.Parse(identity.Claims
                .FirstOrDefault(c =>
                    c.Type == "userId")
                ?.Value);
                    data = _managerservice.GetHiringVehicles(userId: userId, type: "projectManager", projectFilter: projectFilter, statusFilter: statusFilter);
                }
                else
                {
                    return Content(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
                }
                byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Hiring Report");

                return new PdfResult(excelBytes, "HiringReport.xlsx", Request);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("An error occurred while generating the report: " + ex.Message));
            }
        }
        #endregion

        #region Employee Monthly Reports
        [HttpGet]
        [Route("api/report/EmployeeMonthlyreportpdf")]
        public IHttpActionResult EmployeeMonthlyReportPdf(int id)
        {
            var data = _managerservice.GetEmpReport(id);
            byte[] pdfBytes = ReportGenerator.CreateMonthlyReviewPdf(data, "EmployeeMonthly Report");

            return new PdfResult(pdfBytes, "EmployeeMonthlyReport.pdf", Request);
        }
        #endregion

        #region Fund Reports
        [HttpGet]
        [Route("api/report/Fundreportpdf")]
        public IHttpActionResult FundReportPdf(string statusFilter = null)
        {
            try
            {
                var columnMappings = new List<ColumnMapping>
                {
                    new ColumnMapping { Header = "Project Manager", PropertyName = "projectManager" },
                    new ColumnMapping { Header = "Project Code", PropertyName = "projectCode" },
                    new ColumnMapping { Header = "Project", PropertyName = "projectName" },
                    new ColumnMapping { Header = "Head Name", PropertyName = "headName" },
                    new ColumnMapping { Header = "Date From", PropertyName = "dateFrom" },
                    new ColumnMapping { Header = "Date To", PropertyName = "dateTo" },
                    new ColumnMapping { Header = "Proposed Place", PropertyName = "proposedPlace" },
                    new ColumnMapping { Header = "Purpose of Visit", PropertyName = "purposeOfVisit" },
                    new ColumnMapping { Header = "Total Day & Night", PropertyName = "totalDaysNight" },
                    new ColumnMapping { Header = "Availability of Fund", PropertyName = "availbilityOfFund" },
                    new ColumnMapping { Header = "Remark", PropertyName = "remark" },
                    new ColumnMapping { Header = "Status", PropertyName = "statusLabel" },
                };

                dynamic data = null;
                string role = getRole();
                if (role.Equals("admin"))
                {
                    data = _managerservice.All_Project_List(0, null, null, null);
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
                else if (role.Equals("projectManager"))
                {
                    var identity = (ClaimsIdentity)System.Web.HttpContext.Current.User.Identity;
                    int userId = int.Parse(identity.Claims
                .FirstOrDefault(c =>
                    c.Type == "userId")
                ?.Value);
                    data = _managerservice.All_Project_List(0, null, null, null);
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
                    return Content(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
                }
                byte[] pdfBytes = ReportGenerator.CreatePdf(columnMappings, data, "Fund Report");

                return new PdfResult(pdfBytes, "FundReport.pdf", Request);
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
                var columnMappings = new List<ColumnMapping>
                {
                    new ColumnMapping { Header = "Project Manager", PropertyName = "projectManager" },
                    new ColumnMapping { Header = "Project Code", PropertyName = "projectCode" },
                    new ColumnMapping { Header = "Project", PropertyName = "projectName" },
                    new ColumnMapping { Header = "Head Name", PropertyName = "headName" },
                    new ColumnMapping { Header = "Date From", PropertyName = "dateFrom" },
                    new ColumnMapping { Header = "Date To", PropertyName = "dateTo" },
                    new ColumnMapping { Header = "Proposed Place", PropertyName = "proposedPlace" },
                    new ColumnMapping { Header = "Purpose of Visit", PropertyName = "purposeOfVisit" },
                    new ColumnMapping { Header = "Total Day & Night", PropertyName = "totalDaysNight" },
                    new ColumnMapping { Header = "Availability of Fund", PropertyName = "availbilityOfFund" },
                    new ColumnMapping { Header = "Remark", PropertyName = "remark" },
                    new ColumnMapping { Header = "Status", PropertyName = "statusLabel" },
                };

                dynamic data = null;
                string role = getRole();
                if (role.Equals("admin"))
                {
                    data = _managerservice.All_Project_List(0, null, null, null);
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
                else if (role.Equals("projectManager"))
                {
                    var identity = (ClaimsIdentity)System.Web.HttpContext.Current.User.Identity;
                    int userId = int.Parse(identity.Claims
                .FirstOrDefault(c =>
                    c.Type == "userId")
                ?.Value);
                    data = _managerservice.All_Project_List(0, null, null, null);
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
                    return Content(HttpStatusCode.Forbidden, "You do not have permission to access this resource.");
                }
                byte[] excelBytes = ReportGenerator.CreateExcel(columnMappings, data, "Fund Report");

                return new PdfResult(excelBytes, "FundReport.xlsx", Request);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("An error occurred while generating the report: " + ex.Message));
            }
        }
        #endregion

        #region Attendance Report
        [HttpGet]
        [Route("api/report/attendancereportexcel")]
        public IHttpActionResult AttendanceReportExcel(int month, int year, int projectManager, int? EmpId = null)
        {
            try
            {
                dynamic data = null;
                if (EmpId.HasValue)
                {
                    data = _managerservice.ConvertExcelFile(month, year, projectManager, Convert.ToInt32(EmpId));
                }
                else
                {
                    data = _managerservice.ConvertExcelFileOfAll(month, year, projectManager);
                }
                    return new PdfResult(data, "AttendanceReport.xlsx", Request);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("An error occurred while generating the report: " + ex.Message));
            }
        }
        #endregion

        #endregion

    }
}
