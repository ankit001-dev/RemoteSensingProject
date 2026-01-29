// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.ProjectManager.OuterSource
using RemoteSensingProject.Models;

public class OuterSource
{
	public string joiningdate { get; set; }

	public int Id { get; set; }

	public int EmpId { get; set; }

	public int designationid { get; set; }

	public string designationname { get; set; }

	public string EmpName { get; set; }

	public long mobileNo { get; set; }

	public string gender { get; set; }

	public string email { get; set; }

	public bool completeStatus { get; set; }

	public string message { get; set; }

	public ApiCommon.PaginationInfo Pagination { get; set; }
}
