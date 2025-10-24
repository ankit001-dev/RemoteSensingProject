using Npgsql;
using SelectPdf;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
namespace RemoteSensingProject.Models
{
    public class DataFactory
    {
        public NpgsqlConnection con;
        public NpgsqlCommand cmd;
        public DataFactory() {
            con = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
            cmd = new NpgsqlCommand();
        }



        public byte[] ExportPdfData(string htmlContent) 
        {
            HtmlToPdf converter = new HtmlToPdf();
            PdfDocument doc = converter.ConvertHtmlString(htmlContent);
            byte[] pdfBytes;
            using (MemoryStream ms = new MemoryStream())
             {
                doc.Save(ms);
                pdfBytes = ms.ToArray();
            }

            doc.Close();
                return pdfBytes;
        }
    }
}