using DocumentFormat.OpenXml.Bibliography;
using OfficeOpenXml;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static RemoteSensingProject.Models.ApiCommon;

namespace RemoteSensingProject.Models
{
    public class ReportGenerator
    {

        #region Pdf Generator
        public static byte[] CreatePdf<T>(List<ColumnMapping> columns, IEnumerable<T> data, string title)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            int srNo = 1;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);

                    page.Header().AlignCenter().PaddingBottom(10).Text(title).FontSize(16).SemiBold();

                    page.Content().Table(table =>
                    {
                        // 1) Define columns: SR + dynamic columns
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(); // SR column

                            foreach (var col in columns)
                                c.RelativeColumn();
                        });

                        // 2) Header row
                        table.Header(h =>
                        {
                            h.Cell()
                                .Border(1)
                                .Background(Colors.Grey.Lighten3)
                                .Padding(5)
                                .Text("Sr")
                                .SemiBold();

                            foreach (var col in columns)
                            {
                                h.Cell()
                                .Border(1)
                                .Background(Colors.Grey.Lighten3)
                                .Padding(5)
                                .Text(col.Header)
                                .SemiBold();
                            }
                        });

                        // 3) Data Rows
                        foreach (var item in data)
                        {
                            // SR No Column
                            table.Cell()
                                .Border(1)
                                .Padding(5)
                                .Text(srNo.ToString());

                            srNo++;

                            // Other columns
                            foreach (var col in columns)
                            {
                                string value = GetValue(item, col.PropertyName);

                                table.Cell()
                                    .Border(1)
                                    .Padding(5)
                                    .Text(value);
                            }
                        }
                    });
                });
            });

            using (var ms = new MemoryStream())
            {
                document.GeneratePdf(ms);
                return ms.ToArray();
            }
        }

        private static string GetValue(object obj, string propertyName)
        {
            if (obj == null || string.IsNullOrEmpty(propertyName))
                return "";

            var prop = obj.GetType().GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (prop == null)
                return "";

            var value = prop.GetValue(obj);
            return value?.ToString() ?? "";
        }
        #endregion

        #region Excel Generator
        public static byte[] CreateExcel<T>(List<ColumnMapping> columns, IEnumerable<T> data, string sheetName)
        {
            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add(sheetName);

                int totalColumns = columns.Count + 1;  // +1 for Sr column

                // 1) Add the report title at the top, merged across all columns
                ws.Cells[1, 1, 1, totalColumns].Merge = true;
                ws.Cells[1, 1].Value = sheetName;  // or your report title
                ws.Cells[1, 1].Style.Font.Size = 16;
                ws.Cells[1, 1].Style.Font.Bold = true;
                ws.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells[1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws.Row(1).Height = 25;

                // 2) Add column headers in the second row now (row 2)
                int row = 2;
                int col = 1;

                ws.Cells[row, col].Value = "Sr";
                ws.Cells[row, col].Style.Font.Bold = true;
                ws.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                col++;

                foreach (var c in columns)
                {
                    ws.Cells[row, col].Value = c.Header;
                    ws.Cells[row, col].Style.Font.Bold = true;
                    ws.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    col++;
                }

                // 3) Add data starting from row 3
                row++;
                int sr = 1;

                foreach (var item in data)
                {
                    col = 1;
                    ws.Cells[row, col].Value = sr++;
                    col++;

                    foreach (var c in columns)
                    {
                        ws.Cells[row, col].Value = GetValue(item, c.PropertyName);
                        col++;
                    }

                    row++;
                }

                // 4) Auto-fit all columns
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
        #endregion

        #region Monthly Report Pdf Generator
        public static byte[] CreateMonthlyReviewPdf(List<EmpReportModel> data, string month)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            //QuestPDF.Settings. = true;

            // Register Hindi-capable font (update the path!)
            var fontPath = HttpContext.Current.Server.MapPath(
    "~/assets/NotoSansDevanagari-Regular.ttf"
);

            FontManager.RegisterFont(File.OpenRead(fontPath));

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);

                    page.DefaultTextStyle(t => t
                        .FontFamily("NotoSansDevanagari")
                        .FontSize(11)
                    );

                    // ---------------- Header ----------------
                    page.Header().Column(header =>
                    {
                        header.Item()
                            .AlignCenter()
                            .Text("मासिक समीक्षा: रूप पत्र-2")
                            .SemiBold().FontSize(16);

                        header.Item()
                            .AlignCenter()
                            .Text("शासन से गैर वेतन मद में प्राप्त धनराशि से संचालित योजना / कार्यक्रम की भौतिक उपलब्धियों का विवरण")
                            .FontSize(12);

                        header.Item()
                            .AlignRight()
                            .Text($"माह: {month}")
                            .SemiBold().FontSize(12);
                    });

                    // ---------------- Table ----------------
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(40);
                            c.RelativeColumn(2);
                            c.RelativeColumn(1);
                            c.RelativeColumn(1);
                            c.RelativeColumn(1);
                            c.RelativeColumn(1);
                            c.RelativeColumn(1);
                            c.RelativeColumn(2);
                            c.RelativeColumn(2);
                        });

                        string[] headers =
                        {
        "क्र.सं.",
        "मद / परियोजना का नाम",
        "इकाई",
        "वार्षिक",
        "आलोच्य मासांत तक",
        "आलोच्य माह में",
        "आलोच्य मासांत तक संचिति",
        "प्रदेश सरकार के लाभान्वित विभाग",
        "अनुभूति / टिप्पणी"
    };

                        // ✅ HEADER — defined ONCE
                        table.Header(header =>
                        {
                            foreach (var h in headers)
                            {
                                header.Cell()
                                    .Border(1)
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text(h)
                                    .SemiBold();
                            }
                        });

                        int sr = 1;

                        foreach (var item in data)
                        {
                            table.Cell().Border(1).Padding(5).AlignCenter().Text(sr.ToString());
                            table.Cell().Border(1).Padding(5).Text(item.ProjectName);
                            table.Cell().Border(1).Padding(5).AlignCenter().Text(item.Unit);
                            table.Cell().Border(1).Padding(5).AlignCenter().Text(item.AnnualTarget.ToString());
                            table.Cell().Border(1).Padding(5).AlignCenter().Text(item.TargetUptoReviewMonth.ToString());
                            table.Cell().Border(1).Padding(5).AlignCenter().Text(item.AchievementDuringReviewMonth.ToString());
                            table.Cell().Border(1).Padding(5).AlignCenter().Text(item.CumulativeAchievement.ToString());
                            table.Cell().Border(1).Padding(5).Text(item.BenefitingDepartments);
                            table.Cell().Border(1).Padding(5).Text(item.Remarks);

                            sr++;
                        }
                    });
                });
            });

            MemoryStream ms = new MemoryStream();
            document.GeneratePdf(ms);
            return ms.ToArray();
        }

        #endregion

        #region Manpower Monthly Report
        public static byte[] CreateManpowerMonthlyPdf(List<DivisionOutsourceReport> data, string month, int year)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            if (data == null || data.Count == 0)
                return Array.Empty<byte>();

            // Hindi font (same pattern as your other report)
            var fontPath = HttpContext.Current.Server.MapPath(
                "~/assets/NotoSansDevanagari-Regular.ttf"
            );
            FontManager.RegisterFont(File.OpenRead(fontPath));

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);

                    page.DefaultTextStyle(t => t
                        .FontFamily("NotoSansDevanagari")
                        .FontSize(11)
                    );

                    // ---------- HEADER ----------
                    page.Header().Row(row =>
                    {
                        row.RelativeItem()
                            .Text($"प्रभाग का नाम : {data[0].DivisionName}")
                            .SemiBold();

                        row.RelativeItem()
         .AlignRight()
         .Column(col =>
         {
             col.Item()
                 .AlignRight()
                 .Text("प्रारुप")
                 .SemiBold();

             col.Item()
                 .AlignRight()
                 .Text($"माह : {month} {year}")
                 .SemiBold();
         });
                    });

                    // ---------- TABLE ----------
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(40); // क्रम सं.
                            c.RelativeColumn(3);  // परियोजना
                            c.RelativeColumn(3);  // मानवशक्ति
                            c.RelativeColumn(2);  // पदनाम
                        });

                        // Table Header
                        table.Header(h =>
                        {
                            h.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).AlignCenter().Text("क्रम सं.").SemiBold();
                            h.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("परियोजना का नाम\n(बाह्य सहायक / गैर वेतन मद)").SemiBold();
                            h.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("परियोजना में आबद्ध मानवशक्ति का नाम").SemiBold();
                            h.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("पदनाम").SemiBold();
                        });

                        int srNo = 1;

                        var grouped = data.GroupBy(x => x.ProjectId).ToList();

                        foreach (var project in grouped)
                        {
                            bool firstRow = true;
                            int rowSpan = project.Count();

                            foreach (var item in project)
                            {
                                if (firstRow)
                                {
                                    table.Cell().RowSpan(Convert.ToUInt16(rowSpan))
                                        .Border(1).Padding(5)
                                        .AlignCenter()
                                        .Text(srNo.ToString())
                                        .SemiBold();

                                    table.Cell().RowSpan(Convert.ToUInt16(rowSpan))
                                        .Border(1).Padding(5)
                                        .Text(item.ProjectName)
                                        .SemiBold();
                                }

                                table.Cell()
                                    .Border(1).Padding(5)
                                    .Text(item.OutsourceName);

                                table.Cell()
                                    .Border(1).Padding(5)
                                    .Text(item.DesignationName);

                                firstRow = false;
                            }

                            srNo++;
                        }
                    });
                });
            });

            using (var ms = new MemoryStream())
            {
                document.GeneratePdf(ms);
                return ms.ToArray();
            }
        }
        #endregion
    }

    #region Returning File Function
    public class PdfResult : IHttpActionResult
    {
        private readonly byte[] _bytes;
        private readonly string _fileName;
        private readonly HttpRequestMessage _request;

        public PdfResult(byte[] bytes, string fileName, HttpRequestMessage request)
        {
            _bytes = bytes;
            _fileName = fileName;
            _request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken token)
        {
            var response = _request.CreateResponse(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(_bytes);

            string contentType;

            var extension = Path.GetExtension(_fileName)?.ToLower();

            switch (extension)
            {
                case ".pdf":
                    contentType = "application/pdf";
                    break;

                case ".xlsx":
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;

                case ".docx":   // ✅ ADD THIS
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;

                default:
                    contentType = "application/octet-stream"; // fallback
                    break;
            }

            response.Content.Headers.ContentType =
                new MediaTypeHeaderValue(contentType);

            response.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileNameStar = _fileName
                };

            return Task.FromResult(response);
        }

    }
    #endregion
}