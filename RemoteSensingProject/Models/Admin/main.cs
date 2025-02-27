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
            public string TotalProject { get; set; }
            public string TotalDelayproject { get; set; }
            public string TotalCompleteProject { get; set; }
            public string TotalOngoingProject { get; set; }
            public string TotalMeetings { get; set; }
            public string ProjectManager { get; set; }
            public decimal TotalBudget { get; set; }
            public decimal PendingBudget { get; set; }
            public decimal ExpenditureBudget { get; set; }

        }
        public class BudgetForm
        {
            public int sn { get; set; }
            public string addedDate { get; set; }
            public string financial { get; set; }
            public decimal budget { get; set; }
        }
    }
}