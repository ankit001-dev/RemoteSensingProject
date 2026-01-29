// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.ProjectManager.OutSourceTask
using RemoteSensingProject.Models;

public class OutSourceTask
{
	public int Id { get; set; }

	public int empId { get; set; }

	public string title { get; set; }

	public string description { get; set; }

	public int[] outSourceId { get; set; }

	public bool completeStatus { get; set; }

	public ApiCommon.PaginationInfo Pagination { get; set; }
}
