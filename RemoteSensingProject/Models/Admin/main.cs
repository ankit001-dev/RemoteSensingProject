using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Web;

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
            public bool ActiveStatus { get; set; }

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
            public string ProjectTitle { get; set; }
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
            public string ProjectDepartment { get; set; }
            public string ContactPerson { get; set; }
            public string Address { get; set; }
           
        }
        public class Project_Budget
        {
            public int Id { get; set; }
            public int Project_Id { get; set; }
            public string ProjectHeads { get; set; }
            public decimal ProjectAmount { get; set; }
            public string Miscellaneous { get; set; }
            public decimal Miscell_amt { get; set; }
            public string HeadsDescription { get; set; }

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
            public string KeyPoint { get; set; }
            public HttpPostedFileBase Stage_Document { get; set; }
            public string Document_Url { get; set; }
            public DateTime CompletionDate { get; set; }

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
            public HttpPostedFileBase Attachment { get; set; }
            public string Attachment_Url { get; set; }
            public string KeyPoint { get; set; }
            public int status { get; set; }
            public int EmployeeId { get; set; }
            public int MeetingId { get; set; }
            public string[] meetingMemberList { get; set; }
            public string[] keyPointList { get; set; }
            public int CompleteStatus { get; set; }
            public string empId { get; set; }
            public string meetingId { get; set; }
            public List<string> empName { get; set; }
            public List<string> meetingKeyPoint { get; set; }
            public List<string> memberId { get; set; }
        }

        public class AddMeeting_Model
        {
            public int Id { get; set; }
            public string MeetingType { get; set; }
            public string MeetingMember { get; set; }
            public string MeetingLink { get; set; }
            public string MeetingTitle { get; set; }
            public string MeetingDate { get; set; }
            public DateTime MeetingTime { get; set; }
            public HttpPostedFileBase Attachment { get; set; }
            public string Attachment_Url { get; set; }
            public int status { get; set; }
            public List<int> meetingMemberList { get; set; }
            public List<string> keyPointList { get; set; }
            public int CompleteStatus { get; set; }
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
            public string Notice { get; set; }
            public string noticeDate { get; set; }

        }
        public class DashboardCount
        {
            public string TotalEmployee { get; set; }
            public string TotalProject { get; set; }
            public string TotalDelayproject { get; set; }
            public string TotalCompleteProject { get; set; }
            public string TotalOngoingProject { get; set; }
            public string TotalMeetings { get; set; }

        }

    }
}