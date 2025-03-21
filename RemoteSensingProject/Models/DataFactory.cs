using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using Grpc.Core;
using Ninject.Activation;
using SelectPdf;
namespace RemoteSensingProject.Models
{
    public class DataFactory
    {
        public SqlConnection con;
        public SqlCommand cmd;
        public DataFactory() { 
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
            cmd = new SqlCommand();
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