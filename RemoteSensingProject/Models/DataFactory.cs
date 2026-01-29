// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.DataFactory
using System.Configuration;
using System.IO;
using Npgsql;
using SelectPdf;

public class DataFactory
{
	public NpgsqlConnection con;

	public NpgsqlCommand cmd;

	public DataFactory()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		con = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
		cmd = new NpgsqlCommand();
	}

	public byte[] ExportPdfData(string htmlContent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		HtmlToPdf converter = new HtmlToPdf();
		PdfDocument doc = converter.ConvertHtmlString(htmlContent);
		byte[] pdfBytes;
		using (MemoryStream ms = new MemoryStream())
		{
			doc.Save((Stream)ms);
			pdfBytes = ms.ToArray();
		}
		doc.Close();
		return pdfBytes;
	}
}
