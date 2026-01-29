// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.ProjectManager.AttendanceManage
using System;
using System.Collections.Generic;

public class AttendanceManage
{
	public int total { get; set; }

	public int present { get; set; }

	public int absent { get; set; }

	public string remark { get; set; }

	public int id { get; set; }

	public int projectManager { get; set; }

	public string projectManagerName { get; set; }

	public int EmpId { get; set; }

	public string EmpName { get; set; }

	public string address { get; set; }

	public string longitude { get; set; }

	public string latitude { get; set; }

	public string attendanceStatus { get; set; }

	public DateTime attendanceDate { get; set; }

	public bool projectManagerAppr { get; set; }

	public DateTime createdAt { get; set; }

	public bool status { get; set; }

	public bool newRequest { get; set; }

	public Dictionary<string, string> Attendance { get; set; }
}
