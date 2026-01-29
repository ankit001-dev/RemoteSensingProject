// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.ProjectManager.tourProposal
using System;
using RemoteSensingProject.Models;

public class tourProposal
{
	public string projectCode { get; set; }

	public string remark { get; set; }

	public bool newRequest { get; set; }

	public bool adminappr { get; set; }

	public string projectName { get; set; }

	public int projectId { get; set; }

	public string projectManager { get; set; }

	public int userId { get; set; }

	public int id { get; set; }

	public DateTime dateOfDept { get; set; }

	public string place { get; set; }

	public DateTime periodFrom { get; set; }

	public DateTime periodTo { get; set; }

	public DateTime returnDate { get; set; }

	public string purpose { get; set; }

	public ApiCommon.PaginationInfo Pagination { get; set; }
}
