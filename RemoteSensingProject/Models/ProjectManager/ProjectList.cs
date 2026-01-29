// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.ProjectManager.ProjectList
using System;

public class ProjectList
{
	public string projectCode { get; set; }

	public int Id { get; set; }

	public DateTime AssignDate { get; set; }

	public DateTime CurrentDate => DateTime.Now;

	public string ProjectName { get; set; }

	public DateTime StartDate { get; set; }

	public DateTime CompletionDate { get; set; }

	public string ProjectStatus { get; set; }

	public string Status { get; set; }

	public string Title { get; set; }

	public int managerId { get; set; }

	public float budget { get; set; }

	public string Description { get; set; }

	public string ProjectDocument { get; set; }

	public string projectType { get; set; }

	public bool stage { get; set; }

	public string CreatedAt { get; set; }

	public string Upadtedat { get; set; }

	public string CreatedBy { get; set; }

	public int CompleteionStatus { get; set; }

	public int ApproveStatus { get; set; }

	public string CompletionDatestring { get; set; }

	public string AssignDateString { get; set; }

	public string StartDateString { get; set; }

	public decimal Percentage { get; set; }

	public decimal physicalPercent { get; set; }

	public decimal overAllPercent { get; set; }
}
