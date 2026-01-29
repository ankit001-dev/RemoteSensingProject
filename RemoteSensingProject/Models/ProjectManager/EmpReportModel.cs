// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.ProjectManager.EmpReportModel
public class EmpReportModel
{
	public int PmId { get; set; }

	public int ProjectId { get; set; }

	public string ProjectName { get; set; }

	public string Unit { get; set; }

	public int AnnualTarget { get; set; }

	public int TargetUptoReviewMonth { get; set; }

	public int AchievementDuringReviewMonth { get; set; }

	public int CumulativeAchievement { get; set; }

	public string BenefitingDepartments { get; set; }

	public string Remarks { get; set; }

	public string CreatedAt { get; set; }
}
