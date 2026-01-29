// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.ProjectManager.ProjectExpenses
using System;
using System.Web;

public class ProjectExpenses
{
	public int Id { get; set; }

	public int projectId { get; set; }

	public int projectHeadId { get; set; }

	public int AppStatus { get; set; }

	public float AppAmount { get; set; }

	public string title { get; set; }

	public string description { get; set; }

	public DateTime date { get; set; }

	public decimal amount { get; set; }

	public HttpPostedFileBase Attatchment_file { get; set; }

	public string attatchment_url { get; set; }

	public string DateString { get; set; }

	public string reason { get; set; }
}
