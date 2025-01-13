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
            public int EmployeeCode { get; set; }
            public string EmployeeName { get; set; }
            public string MobileNo { get; set; }
            public string Email { get; set; }
            public string EmployeeRole { get; set; }
            public string Division { get; set; }
            public string Designation { get; set; }
            public string Gender { get; set; }
            public HttpPostedFileBase EmployeeImages { get; set; }
            public string Image_url { get; set; }

        }
        public class Project_model
        {
            public int Id { get; set; }
            public string ProjectTitle { get; set; }
            public DateTime AssignDate { get; set; }
            public DateTime StartDate { get; set; }
            public string CompletionDate { get; set; }
            public string ProjectManager { get; set; }
            public string SubOrdinate { get; set; }
            public float ProjectBudget { get; set; }
            public string ProjectType { get; set; }
            public string ProjectDescription { get; set; }

        }
        public class Project_Budget
        {
            public int Id { get; set; }
            public string ProjectHeads { get; set; }
            public string ProjectAmount { get; set; }
            public string Miscellaneous { get; set; }
            public string HeadsDescription { get; set; }

        }

        public class Project_Statge
        {
            public int Id { get; set; } 
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
            public DateTime MeetingTime { get; set; }
            public HttpPostedFileBase Attachment { get; set; }
            public string Attachment_Url { get; set;}
            public string KeyPoint { get; set; }

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
            public HttpPostedFileBase Attachment { get; set; }
            public string Attachment_Url { get; set; }
            public string Notice { get; set; }

        }
   
    }
}