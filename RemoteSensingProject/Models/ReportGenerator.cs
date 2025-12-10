using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
            ExcelPackage.License.SetNonCommercialPersonal("RemoteSensing");
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
            response.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/pdf");

            response.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = _fileName
                };

            return Task.FromResult(response);
        }
    }
    #endregion
}