using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using static RemoteSensingProject.Models.ApiCommon;

namespace RemoteSensingProject.Models.Accounts
{
    public class main
    {
        public class DashboardCount
        {
            public string TotalReinbursementReq { get; set; }
            public string TotalTourProposalReq { get; set; }
            public string totalVehicleHiringRequest { get; set; }
            public string totalReinbursementPendingRequest { get; set; }
            public string totalReinbursementapprovedRequest { get; set; }
            public string totalReinbursementRejectRequest { get; set; }
            public string totalTourProposalApprReque { get; set; }
            public string totalTourProposalRejectReque { get; set; }
            public string totaTourProposalPendingReque { get; set; }
            public string totalPendingHiringVehicle { get; set; }
            public string totalApproveHiringVehicle { get; set; }
            public string totalRejectHiringVehicle { get; set; }
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
            public decimal ApprAmount { get; set; }
            public string title { get; set; }
            public decimal amount { get; set; }
            public string months { get; set; }
            public decimal pendingamount { get; set; }
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


        public class Reimbursement
        {
            public string chequeNum { get; set; }
            public string chequeDate { get; set; }
            public bool accountNewRequest { get; set; }
            public bool apprstatus { get; set; }
            public decimal approveAmount { get; set; }
            public bool newRequest { get; set; }
            public bool status { get; set; }
            public string remark { get; set; }
            public DateTime date { get; set; }
            public int id { get; set; }
            public int userId { get; set; }
            public string EmpName { get; set; }
            public string type { get; set; }
            public string chequeNumber { get; set; }
            public string vrNo { get; set; }
            public string particulars { get; set; }
            public string items { get; set; }
            public string purpose { get; set; }
            public decimal amount { get; set; }
            public decimal apprAmt { get; set; }
            public bool appr_status { get; set; }
        }
        public class tourProposal
        {
            public string statusLabel { get; set; }
            public PaginationInfo Pagination { get; set; }
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
        public class HiringVehicle
        {
            public int projectId { get; set; }
            public int id { get; set; }
            public int userId { get; set; }
            public string projectName { get; set; }
            public DateTime dateFrom { get; set; }
            public DateTime dateTo { get; set; }
            public string purposeOfVisit { get; set; }
            public string totalDaysNight { get; set; }
            public string totalPlainHills { get; set; }
            public string taxi { get; set; }
            public string BookAgainstCentre { get; set; }
            public string availbilityOfFund { get; set; }
            public string taxiReportTo { get; set; }
            public TimeSpan taxiReportAt { get; set; }
            public string taxiReportPlace { get; set; }
            public DateTime taxiReportOn { get; set; }
            public string proposedPlace { get; set; }
        }
    }
}