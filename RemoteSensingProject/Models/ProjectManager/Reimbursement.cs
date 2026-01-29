// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.ProjectManager.Reimbursement
using System;
using RemoteSensingProject.Models;

public class Reimbursement
{
	public string statusLabel { get; set; }

	public bool apprstatus { get; set; }

	public bool submitstatus { get; set; }

	public string remark { get; set; }

	public bool status { get; set; }

	public string chequeNum { get; set; }

	public string chequeDate { get; set; }

	public bool adminappr { get; set; }

	public bool newRequest { get; set; }

	public bool accountNewRequest { get; set; }

	public int id { get; set; }

	public int userId { get; set; }

	public string EmpName { get; set; }

	public string type { get; set; }

	public string vrNo { get; set; }

	public DateTime date { get; set; }

	public string particulars { get; set; }

	public string items { get; set; }

	public string purpose { get; set; }

	public decimal amount { get; set; }

	public bool subStatus { get; set; }

	public decimal approveAmount { get; set; }

	public ApiCommon.PaginationInfo Pagination { get; set; }
}
