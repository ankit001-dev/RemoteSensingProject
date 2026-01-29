// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.ProjectManager.AllAttendance
using System.Collections.Generic;
using RemoteSensingProject.Models.ProjectManager;

public class AllAttendance
{
	public string EmpName { get; set; }

	public int EmpId { get; set; }

	public int month { get; set; }

	public string projectManager { get; set; }

	public int present { get; set; }

	public int absent { get; set; }

	public List<AttendanceManage> showAll { get; set; }
}
