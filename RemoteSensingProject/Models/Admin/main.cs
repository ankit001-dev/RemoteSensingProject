using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Web;
using System.Web.Mvc;

namespace RemoteSensingProject.Models.Admin
{
    public class main
    {
        public class CommonResponse
        {
            public int id { get; set; }
            public string name { get; set; }
        }
        public class Employee_model
        {
            public int Id { get; set; }
            public string EmployeeCode { get; set; }
            public string EmployeeName { get; set; }
            public string CreationDate { get; set; }
            public long MobileNo { get; set; }
            public string Email { get; set; }
            public string EmployeeRole { get; set; }
            public int Division { get; set; }
            public int Designation { get; set; }
            public string Gender { get; set; }
            public HttpPostedFileBase EmployeeImages { get; set; }
            public string Image_url { get; set; }
            public string DevisionName { get; set; }
            public string DesignationName { get; set; }
            public bool Status { get; set; }
            public int meetingId { get; set; }
            public bool ActiveStatus { get; set; }
            public bool PresentStatus { get; set; }
            public int AppStatus { get; set; }
            public string Reason { get; set; }
        }

        public class MeetingConclusion
        {
            public int Id { get; set; }
            public int Meeting { get; set; }
            public List<string> KeyPointId { get; set; }
            public List<string> MemberId { get; set; }
            public List<string> KeyResponse { get; set; }
            public string Conclusion { get; set; }
            public DateTime? NextFollowUpDate { get; set; }
            public string NextFollow { get; set; }
            public bool FollowUpStatus { get; set; }
            public string MeetingDate { get; set; }
            public string mode { get; set; }
            public string address { get; set; }
            public List<int> MeetingMemberList { get; set; }
            public string summary { get; set; }
        }

        public class KeyPointResponse
        {
            public string KeyPoint { get; set; }
            public string Response { get; set; }
        }

        public class createProjectModel
        {
            public Project_model pm { get; set; }
            public List<Project_Budget> budgets { get; set; }
            public List<Project_Statge> stages { get; set; }
            public List<Project_Subordination> SubOrdinate { get; set; }
        }
        public class Project_model
        {        
            public int Id { get; set; }
            public string letterNo { get; set; }
            public decimal physicalcomplete { get; set; }
            public decimal overallPercentage { get; set; }
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
            public int ProjectId { get; set; }
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
            public string heads { get; set; }
            public string Percentage { get; set; }
           
        }

        public class Project_WeeklyUpdate
        {
            public int Id { get; set; }
            public string comments { get; set; }
            public int ProjectId { get; set; }
            public int completionPerc { get; set; }
            public DateTime date { get; set; } 
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
        public class Project_Subordination
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string EmpCode { get; set; }
        }
        public class Project_Statge
        {
            public int Id { get; set; }
            public int Project_Id { get; set; }
            public int Stage_Id { get; set; }
            public string KeyPoint { get; set; }
            public string Reason { get; set; }
            public HttpPostedFileBase Stage_Document { get; set; }
            public string Document_Url { get; set; }
            public string Status { get; set; }
            public int completionStatus { get; set; }
            public DateTime CompletionDate { get; set; }
            public string CompletionDatestring { get; set; }
            public string Comment { get; set; }
            public string CompletionPrecentage { get; set; }
            public HttpPostedFileBase StageDocument { get; set; }
            public HttpPostedFileBase DelayedStageDocument { get; set; }
            public string StageDocument_Url { get; set; }
            public string DelayReason { get; set; }
            public string CreatedDate { get; set; }
        }
        public class Meeting_Model
        {
            public int Id { get; set; }
            public string MeetingType { get; set; }
            public string MeetingMember { get; set; }
            public string MeetingLink { get; set; }
            public string MeetingTitle { get; set; }
            public string MeetingDate { get; set; }
            public DateTime MeetingTime { get; set; }
            public string MeetingTimeString { get; set; }
            public HttpPostedFileBase Attachment { get; set; }
            public string Attachment_Url { get; set; }
            public string KeyPoint { get; set; }
            public int status { get; set; }
            public int EmployeeId { get; set; }
            public int MeetingId { get; set; }
            public int CreaterId { get; set; }
            public string[] meetingMemberList { get; set; }
            public string[] keyPointList { get; set; }
            public int CompleteStatus { get; set; }
            public int AppStatus { get; set; }
            public string empId { get; set; }
            public string meetingId { get; set; }
            public List<string> empName { get; set; }
            public List<string> meetingKeyPoint { get; set; }
            public List<string> memberId { get; set; }
            public List<KeyPoint> MeetingKeyPointDict { get; set; }
            public string summary { get; set; }

        }
        public class KeyPoint
        {
            public int Id { get; set; }
            public string keyPoint { get; set; }
        }
        public class AddMeeting_Model
        {
            public int Id { get; set; }
            public string MeetingType { get; set; }
            public string MeetingMember { get; set; }
            public string MeetingLink { get; set; }
            public string MeetingAddress { get; set; }
            public string MeetingTitle { get; set; }
            public string MeetingDate { get; set; }
            public DateTime MeetingTime { get; set; }
            public HttpPostedFileBase Attachment { get; set; }
            public string Attachment_Url { get; set; }
            public int status { get; set; }
            public List<int> meetingMemberList { get; set; }
            public List<string> keyPointList { get; set; }
            public List<string> KeypointId { get; set; }
            public int CompleteStatus { get; set; }
            public int? CreaterId { get; set; }
        
        }

        public class Meeting_Conclusion
        {
            public int Id { get; set; }
            public string Response { get; set; }
            public string Conclusion { get; set; }
            public DateTime NextDate { get; set; }
        }

        public class Generate_Notice
        {
            public int Id { get; set; }
            public string ProjectName { get; set; }
            public string ProjectManager { get; set; }
            public string ProjectManagerImage { get; set; }
            public int ProjectManagerId { get; set; }
            public int ProjectId { get; set; }
            public HttpPostedFileBase Attachment { get; set; }
            public string Attachment_Url { get; set; }
            [AllowHtml]
            public string Notice { get; set; }
            public string noticeDate { get; set; }

        }
        public class DashboardCount
        {
            public decimal expenditure { get; set; }
            public string Title { get; set; }
            public string OverallCompletionPercentage { get; set; }
            public string TotalEmployee { get; set; }
            public string TotalProjectManager { get; set; }
            public string TotalAccounts { get; set; }
            public string TotalSubOrdinate { get; set; }
            public string TotalProject { get; set; }
            public string TotalInternalProject { get; set; }
            public string TotalExternalProject { get; set; }
            public string TotalDelayproject { get; set; }
            public string TotalCompleteProject { get; set; }
            public string TotalOngoingProject { get; set; }
            public string TotalMeetings { get; set; }
            public string TotalAdminMeetings { get; set; }
            public string TotalProjectManagerMeetings { get; set; }
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
            public string ProjectManager { get; set; }
            public decimal TotalBudget { get; set; }
            public decimal PendingBudget { get; set; }
            public decimal ExpenditureBudget { get; set; }

        }
        public class ProjectExpenditure
        {
            public int id { get; set; }
            public string ProjectName  { get; set; }
            public decimal ProjectBudget  { get; set; }
            public decimal expenditure  { get; set; }
            public decimal remaining  { get; set; }
        }
     
        public class BudgetForGraph
        {
            public decimal budget { get; set; }
            public string month { get; set; }
        }
        public class tourProposalAll
        {
            public bool newRequest { get; set; }
            public bool adminappr { get; set; }
            public string projectName { get; set; }
            public string projectManager { get; set; }
            public int userId { get; set; }
            public int id { get; set; }
            public DateTime dateOfDept { get; set; }
            public string place { get; set; }
            public DateTime periodFrom { get; set; }
            public DateTime periodTo { get; set; }
            public DateTime returnDate { get; set; }
            public string purpose { get; set; }
        }


        public class AdminReimbursement
        {
            public bool status { get; set; }
            public bool adminappr { get; set; }
            public bool newRequest { get; set; }
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
            public decimal approveAmount { get; set; }
            public bool subStatus { get; set; }
            public bool appr_status { get; set; }
            public string chequeNum { get; set; }
            public string chequeDate { get; set; }
        }
        public class HiringVehicle1
        {
            public bool newRequest { get; set; }
            public bool adminappr { get; set; }
            public string projectManager { get; set; }
            public bool status { get; set; }
            public decimal amount { get; set; }
            public int headId { get; set; }
            public string headName { get; set; }
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
            public string note { get; set; }
            public string proposedPlace { get; set; }
        }
    }
}