using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RemoteSensingProject.Models.SubOrdinate
{
    public class main
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
        public class ProjectList
        {
            public int Id { get; set; } // Unique identifier for the project
            public DateTime AssignDate { get; set; }
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
            public string stage { get; set; }
            public string CreatedAt { get; set; }
            public string Upadtedat { get; set; }
            public string CreatedBy { get; set; }
            public int CompleteionStatus { get; set; }
            public int ApproveStatus { get; set; }
            public string CompletionDatestring { get; set; }
            public string AssignDateString { get; set; }
            public string StartDateString { get; set; }
        }
    }
}