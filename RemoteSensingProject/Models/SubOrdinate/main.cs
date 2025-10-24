using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RemoteSensingProject.Models.SubOrdinate
{
    public class main
    {

        public class DashboardCount
        {
            public int TotalAssignProject { get; set; }
            public int InternalProject { get; set; }
            public int ExternalProject { get; set; }
            public int CompletedProject { get; set; }
            public int PendingProject { get; set; } 
            public int OngoingProject { get; set; }
            public int TotalMeetings { get; set; }
            public int AdminMeetings { get; set; }
            public int ProjectManagerMeetings { get; set; } 

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
            public string projectCode { get; set; }
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
            public bool CompleteionStatus { get; set; }
            public int ApproveStatus { get; set; }
            public string CompletionDatestring { get; set; }
            public string AssignDateString { get; set; }
            public string StartDateString { get; set; }
            public float projectStatus { get; set; }
        }
        public class Raise_Problem
        {
            public string projectCode { get; set; }
            public bool newRequest { get; set; }
            public int ProblemId { get; set; }
            public int Project_Id { get; set; }
            public string ProjectName { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public HttpPostedFileBase Attachment { get; set; }
            public string Attchment_Url { get; set; }
            public string CreatedDate { get; set; }
          
        }
        public class OutSource_Task
        {
            public int id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public int CompleteStatus { get; set; }
            public string Status { get; set; }
            public int EmpId { get; set; }
            public string Reason { get; set; }

        }
    }
}