using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Spreadsheet;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.ProjectManager;
using System.Collections.Generic;
using System.Web.Http;
using static RemoteSensingProject.Models.ApiCommon;

namespace RemoteSensingProject.ApiServices
{
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
    }
}
