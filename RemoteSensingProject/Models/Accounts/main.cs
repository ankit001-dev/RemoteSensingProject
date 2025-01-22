using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RemoteSensingProject.Models.Accounts
{
    public class main
    {

        public class Project_model
        {
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
    }
}