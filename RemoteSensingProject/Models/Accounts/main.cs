// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.Accounts.main
using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Web;

namespace RemoteSensingProject.Models.Accounts
{
	public class main
	{
		public class DashboardCount
		{
			public int TotalTourCount { get; set; }
			public int TotalInternalProjectCount { get; set; }
			public int TotalExternalProjectCount { get; set; }
			public int TotalInternalProjectFund { get; set; }
			public int TotalInternalCompletedProject { get; set; }
			public int TotalExternalProjectFund { get; set; }
			public int TotalInternalExpense { get; set; }
			public int TotalExternalExpense { get; set; }
			public int TotalExternalCompletedProject { get; set; }
			public int AdhisthanBudgetProvision { get; set; }
			public decimal AdhisthanExpenditure { get; set; }
			public decimal AdhisthanExpenditureInPerc { get; set; } 
		}

		public class Project_model
		{
			public string projectCode { get; set; }

			public int Id { get; set; }

			public decimal physicalcomplete { get; set; }

			public string ProjectTitle { get; set; }

			public DateTime CurrentDate => DateTime.Now;

			public DateTime AssignDate { get; set; }

			public DateTime StartDate { get; set; }

			public DateTime CompletionDate { get; set; }

			public string ProjectManager { get; set; }

			public string CompletionDatestring { get; set; }

			public string AssignDateString { get; set; }

			public string StartDateString { get; set; }

			public int[] SubOrdinate { get; set; }

			public HttpPostedFileBase projectDocument { get; set; }

			public string projectDocumentUrl { get; set; }

			public decimal ProjectBudget { get; set; }

			public string ProjectType { get; set; }

			public string ProjectDescription { get; set; }

			public bool ProjectStage { get; set; }

			public bool ProjectStatus { get; set; }

			public string ProjectDepartment { get; set; }

			public string ContactPerson { get; set; }

			public string Address { get; set; }

			public string createdBy { get; set; }

			public bool ApproveStatus { get; set; }
		}

		public class GraphData
		{
			public decimal TotalFund { get; set; }
			public string ProjectName { get; set; }
			public string ProjectCode { get; set; }

			public decimal TotalExpense { get; set; }

			public decimal TotalRemaining { get; set; }
			public string ProjectType { get; set; }
		}
        public class GraphGrouped
        {
            public List<GraphData> Internal { get; set; }
            public List<GraphData> External { get; set; }
        }
        public class HeadExpenses
		{
			public int Id { get; set; }

			public int ProjectId { get; set; }

			public int HeadId { get; set; }

			public int AppStatus { get; set; }

			public string Reason { get; set; }

			public float Amount { get; set; }
		}

		public class Project_Budget
		{
			public int Id { get; set; }

			public int Project_Id { get; set; }

			public int HeadId { get; set; }

			public string ProjectHeads { get; set; }

			public decimal ProjectAmount { get; set; }

			public string HeadsDescription { get; set; }

			public string CompletionDatestring { get; set; }

			public string TotalAskAmount { get; set; }

			public string ApproveAmount { get; set; }
		}

		public class tourProposal
		{
			public string statusLabel { get; set; }

			public ApiCommon.PaginationInfo Pagination { get; set; }

			public string projectCode { get; set; }

			public string remark { get; set; }

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

			public bool newRequest { get; set; }
		}

		public class AdhisthanModel
		{
			public int SchemeId { get; set; }
			public string SchemeName { get; set; }
			public int Id { get; set; }
			public string HeadName { get; set; }
			public decimal BudgetProvision { get; set; }
			public decimal ExpenditureAmount { get; set; }
			public decimal ExpenditurePercentage { get; set; }
			public decimal Committed { get; set; }
		}
		public class UpdateCommitted
		{
			public int HeadId { get; set; }
			public int ProjectId { get; set; }
			public int Id { get; set; }
			public string Title { get; set; }
			public decimal ExpenseCommitted { get; set; }
			public int AdhisthanId { get; set; }

        }
    }

}