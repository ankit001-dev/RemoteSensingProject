using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Web;
using System.Windows.Documents;
using static RemoteSensingProject.Models.ApiCommon;

namespace RemoteSensingProject.Models.ProjectManager
{
   
    public class DashboardCount
    {
        public string ProjectProblem { get; set; }
        public string OutSource { get; set; }
        public string CompletedTask { get; set; }
        public string TotalTask { get; set; }
        public string ReimbursementPendingRequest { get; set; }
        public string ReimbursementApprovedRequest { get; set; }
        public string ReimbursementRejectedRequest { get; set; }
        public string HiringPendingRequest { get; set; }
        public string HiringApprovedRequest { get; set; }
        public string HiringRejectedRequest { get; set; }
        public string TourPendingRequest { get; set; }
        public string TourApprovedRequest { get; set; }
        public string TourRejectedRequest { get; set; }
        public string EmpMeeting { get; set; }
        public string TotalReimbursement { get; set; }
        public string TotalTourProposal { get; set; }
        public string TotalHiring { get; set; }
        public string AdminMeeting { get; set; }
        public string SelfCreatedProject { get; set; }
        public string TotalAssignProject { get; set; }
        public string TotaCompleteProject { get; set; }
        public string TotalDelayProject { get; set; }
        public string TotalMeeting { get; set; }
        public string TotalOngoingProject { get; set; }
        public string TotalNotice { get; set; }

    }

    public class AssignedProject
    {
        public int Id { get; set; } // Unique identifier for the project
        public DateTime AssignDate { get; set; }
        public string ProjectName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public string ProjectStatus { get; set; }
    }
    public class UserCredential
    {
        public string userRole { get; set; }
        public string username { get; set; }
        public string userId { get; set; }
    }

    public class GetConclusion
    {
        public int Id { get; set; }
        public string Conclusion { get; set; }
        public string FollowDate { get; set; }
        
    }

    public class ProjectList
    {
        public string projectCode { get; set; }
        public int Id { get; set; } // Unique identifier for the project
        public DateTime AssignDate { get; set; }
        public DateTime CurrentDate => DateTime.Now;
        public string ProjectName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public string ProjectStatus { get; set; } // e.g., Running, Completed, Delayed
        public string Status { get; set; } // Additional status field (define its purpose)
        public string Title { get; set; }
        public int managerId { get; set; }
        public float budget { get; set; }
        public string Description { set; get; }
        public string ProjectDocument { get; set; }
        public string projectType { get; set; }
        public bool stage { get; set; }
        public string CreatedAt { get; set; }
        public string Upadtedat { get; set; }
        public string CreatedBy { get; set;}
        public int CompleteionStatus { get; set;}
        public int ApproveStatus { get; set;}
        public string CompletionDatestring { get; set; }
        public string AssignDateString { get; set; }
        public string StartDateString { get; set; }
        public decimal Percentage { get; set; }
        public decimal physicalPercent { get; set; }
        public decimal overAllPercent { get; set; }

    }


    public class FinancialMonthlyReport
    {
        public int id { get; set;}
        public int projectId { get; set; }
        public string aim { get; set; }
        public string date { get; set; }
        public string month_aim { get; set; }
        public string completeInMonth { get; set; }
        public string departBeneficiaries { get; set; }
        public string projectName { get; set; }
        public string totalBudget { get; set; }
        public string department { get; set; }
        public string description { get; set; }
    }

    public class ApprovedProject
    {
        public int Id { get; set; } // Unique identifier
        public DateTime AssignDate { get; set; }
        public string ProjectName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public string ProjectStatus { get; set; } // e.g., Running, Completed, Delayed
    }

    public class Meetings
    {
        public int Id { get; set; } // Unique identifier
        public DateTime Date { get; set; } // Meeting date
        public string Title { get; set; } // Meeting title
        public string MeetMode { get; set; } // e.g., Online, In-Person
        public string MeetAddress { get; set; } // Meeting location or link
        public string MeetStatus { get; set; } // e.g., Scheduled, Completed, Canceled
        public string Status { get; set; } // Additional status field (define its purpose)
    }

    public class getMemberResponse
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int MeetingId { get; set; }
        public int? ConclusionId { get; set; }
        public int ApprovedStatus { get; set; }
        public string reason { get; set; }
    }

    public class AllProjectReport
    {
        public int Id { get; set; } // Unique identifier
        public DateTime AssignDate { get; set; } // Date when the project was assigned
        public string ProjectName { get; set; } // Name of the project
        public DateTime StartDate { get; set; } // Project start date
        public DateTime CompletionDate { get; set; } // Expected or actual completion date
    }

    public class PendingProjectReport
    {
        public int Id { get; set; } // Unique identifier for the project
        public DateTime AssignDate { get; set; } // Date when the project was assigned
        public string ProjectName { get; set; } // Name of the project
        public DateTime StartDate { get; set; } // Start date of the project
        public DateTime CompletionDate { get; set; } // Expected or actual completion date
    }
    public class CompleteProjectReport
    {
        public int Id { get; set; } // Unique identifier for the project
        public DateTime AssignDate { get; set; } // The date when the project was assigned
        public string ProjectName { get; set; } // The name of the project
        public DateTime StartDate { get; set; } // The start date of the project
        public DateTime CompletionDate { get; set; } // The completion date of the project
    }

    public class ProjectExpenses
    {
        public int Id { get; set; }
        public int projectId { get; set; }
        public int projectHeadId { get; set; }
        public int AppStatus { get; set; }
        public float AppAmount { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime date { get; set; }
        public decimal amount { get; set; }
        public HttpPostedFileBase Attatchment_file { get; set; }
        public string attatchment_url { get; set; }
        public string DateString { get; set; }
        public string reason { get; set; }
    }

    public class OuterSource
    {
        public string joiningdate { get; set; }
        public int Id { get; set; }
        public int EmpId { get; set; }
        public int designationid { get; set; }
        public string designationname { get; set; }
        public string EmpName { get; set; }
        public long mobileNo { get; set; } 
        public string gender { get; set; }
        public string email { get; set; }
        public bool completeStatus { get; set; }
        public string message { get; set; }
        public PaginationInfo Pagination { get; set; }
    }

    public class OutSourceTask { 
        public int Id { get; set; }
        public int empId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int[] outSourceId { get; set; }
        public bool completeStatus { get; set; }
        public PaginationInfo Pagination { get; set; }
    }
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
        public PaginationInfo Pagination { get; set; }
    }
    public class tourProposal
    {
        public string projectCode { get; set; }
        public string remark { get; set; }
        public bool newRequest { get; set; }
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
        public PaginationInfo Pagination { get; set; }
    }
    public class HiringVehicle
    {
        public string remark { get; set; }
        public string projectCode { get; set; }
        public string projectManager { get; set; }
        public bool adminappr { get; set; }
        public bool newRequest { get; set; }
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
    public class RaiseProblem
    {
        public PaginationInfo Pagination { get; set; }
        public string projectCode { get; set; }
        public DateTime createdAt { get; set; }
        public string projectManager {get; set; }  
        public int userId { get; set; }
        public bool adminappr { get; set; }
        public bool newRequest { get; set; }
        public int id { get; set; }
        public string projectname { get; set; }
        public int projectId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public HttpPostedFileBase document{ get; set; }
        public string documentname{ get; set; }
    }
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
    public class EmpReportModel
    {
        // Project & Unit Section
        public int PmId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Unit { get; set; }
        public int AnnualTarget { get; set; }

        // Target Section
        public int TargetUptoReviewMonth { get; set; }
        public int AchievementDuringReviewMonth { get; set; }
        public int CumulativeAchievement { get; set; }

        // Departments Benefiting & Remarks
        public string BenefitingDepartments { get; set; }
        public string Remarks { get; set; }
        public string CreatedAt { get; set; }
    }
    public class FeedbackModel
    {
        public string Title { get; set; }
        public string FeedbackType { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
    }
    public class UpdateProjectStatus
    {
        public bool projectStatus { get; set; }
        public string remark { get; set; }
        public string CompletionPrecentage { get; set; }
        public int projectId { get; set; }
    }
}