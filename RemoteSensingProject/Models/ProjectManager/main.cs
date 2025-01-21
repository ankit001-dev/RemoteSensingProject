using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Web;

namespace RemoteSensingProject.Models.ProjectManager
{
    public class DashboardCount
    {
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
        public float Percentage { get; set; }

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
    }

    public class OuterSource
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public long mobileNo { get; set; } 
        public string gender { get; set; }
        public string email { get; set; }
    }
}