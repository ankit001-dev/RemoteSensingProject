// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.ProjectManager.RaiseProblem
using System;
using System.Web;
using RemoteSensingProject.Models;

public class RaiseProblem
{
	public ApiCommon.PaginationInfo Pagination { get; set; }

	public string projectCode { get; set; }

	public DateTime createdAt { get; set; }

	public string projectManager { get; set; }

	public int userId { get; set; }

	public bool adminappr { get; set; }

	public bool newRequest { get; set; }

	public int id { get; set; }

	public string projectname { get; set; }

	public int projectId { get; set; }

	public string title { get; set; }

	public string description { get; set; }

	public HttpPostedFileBase document { get; set; }

	public string documentname { get; set; }
}
