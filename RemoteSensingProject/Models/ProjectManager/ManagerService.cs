using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IdentityModel;
using System.IO;
using System.Linq;
using System.Web.Services.Description;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2010.Excel;
using Npgsql;
using NpgsqlTypes;
using RemoteSensingProject.Models.MailService;
using static RemoteSensingProject.Models.Admin.main;
using static RemoteSensingProject.Models.SubOrdinate.main;

namespace RemoteSensingProject.Models.ProjectManager
{
    public class ManagerService : DataFactory
    {

        #region /* Dashboard Count */
        public DashboardCount DashboardCount(int userId)
        {
            DashboardCount obj = null;
            try
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_managedashboard_cursor", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "ManagerDashboardCount");
                    cmd.Parameters.AddWithValue("v_projectmanager", userId);
                    cmd.Parameters.AddWithValue("v_sid", 0);
                    string cursorName = (string)cmd.ExecuteScalar();

                    // Now fetch the data from the cursor
                    using (var fetchCmd = new NpgsqlCommand($"FETCH ALL FROM \"{cursorName}\";", con, tran))
                    using (var sdr = fetchCmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            sdr.Read();
                            obj = new DashboardCount();
                            obj.TotalAssignProject = sdr["TotalAssignProject"].ToString();
                            obj.TotaCompleteProject = sdr["TotalCompleteProject"].ToString();
                            obj.TotalDelayProject = sdr["TotalDelayproject"].ToString();
                            obj.TotalNotice = sdr["TotalNotice"].ToString();
                            obj.TotalOngoingProject = sdr["TotalOngoingProject"].ToString();
                            obj.TotalMeeting = sdr["totalMeetings"].ToString();
                            obj.SelfCreatedProject = sdr["SelfCreatedProject"].ToString();
                            obj.EmpMeeting = sdr["EmpMeeting"].ToString();
                            obj.AdminMeeting = sdr["AdminMeeting"].ToString();
                            obj.TotalReimbursement = sdr["TotalReimbursement"].ToString();
                            obj.TotalTourProposal = sdr["TotalTourProposal"].ToString();
                            obj.TotalHiring = sdr["TotalHiring"].ToString();
                            obj.ReimbursementPendingRequest = sdr["ReimbursementPendingRequest"].ToString();
                            obj.ReimbursementApprovedRequest = sdr["ReimbursementApprovedRequest"].ToString();
                            obj.ReimbursementRejectedRequest = sdr["ReimbursementRejectedRequest"].ToString();
                            obj.TourPendingRequest = sdr["TourPendingRequest"].ToString();
                            obj.TourApprovedRequest = sdr["TourApprovedRequest"].ToString();
                            obj.TourRejectedRequest = sdr["TourRejectedRequest"].ToString();
                            obj.HiringPendingRequest = sdr["HiringPendingRequest"].ToString();
                            obj.HiringApprovedRequest = sdr["HiringApprovedRequest"].ToString();
                            obj.HiringRejectedRequest = sdr["HiringRejectedRequest"].ToString();
                            obj.TotalTask = sdr["TotalTask"].ToString();
                            obj.CompletedTask = sdr["CompletedTask"].ToString();
                            obj.OutSource = sdr["OutSource"].ToString();
                            obj.ProjectProblem = sdr["ProjectProblem"].ToString();
                        }
                    }
                }
                return obj;

            }
            catch (Exception ex)
            {
                throw new Exception("An error accured", ex);
            }
            finally
            {
                con.Close();

            }
        }

        #endregion 

        #region /* Assign Project */

        public UserCredential getManagerDetails(string managerName)
        {
            UserCredential _details = new UserCredential();
            try
            {
                con.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM sp_manageloginmaster(@action, @userid, @username)", con))
                {
                    cmd.Parameters.AddWithValue("@action", "getUserRole");
                    cmd.Parameters.AddWithValue("@username", managerName);
                    cmd.Parameters.AddWithValue("@userid", 0);
                    using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            sdr.Read();
                            _details = new UserCredential();
                            _details.username = sdr["username"].ToString();
                            _details.userId = sdr["userid"].ToString();
                            _details.userRole = sdr["userRole"].ToString();
                        }
                    }
                    return _details;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error accured", ex);

            }
            finally
            {
                con.Close();
            }

        }
        #endregion

        #region /* Add Project */
        public bool addManagerProject(createProjectModel pm)
        {
            con.Open();
            NpgsqlTransaction tran = con.BeginTransaction();
            try
            {
                Random rand = new Random();
                string projectCode = $"{rand.Next(1000, 9999).ToString()}{DateTime.Now.Day}{DateTime.Now.Year.ToString().Substring(2, 2)}";
                cmd = new NpgsqlCommand("sp_adminAddproject", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "insertProject");
                cmd.Parameters.AddWithValue("@title", pm.pm.ProjectTitle);
                cmd.Parameters.AddWithValue("@assignDate", pm.pm.AssignDate);
                cmd.Parameters.AddWithValue("@startDate", pm.pm.StartDate);
                cmd.Parameters.AddWithValue("@completionDate", pm.pm.CompletionDate);
                cmd.Parameters.AddWithValue("@projectmanager", pm.pm.ProjectManager);
                cmd.Parameters.AddWithValue("@budget", pm.pm.ProjectBudget);
                cmd.Parameters.AddWithValue("@description", pm.pm.ProjectDescription);
                cmd.Parameters.AddWithValue("@ProjectDocument", pm.pm.projectDocumentUrl);
                cmd.Parameters.AddWithValue("@projectType", pm.pm.ProjectType);
                cmd.Parameters.AddWithValue("@stage", pm.pm.ProjectStage);
                cmd.Parameters.AddWithValue("@createdBy", "projectManager");
                cmd.Parameters.AddWithValue("@projectCode", projectCode);
                cmd.Parameters.AddWithValue("@ApproveStatus", 1);
                cmd.Parameters.Add("@project_Id", NpgsqlDbType.Integer);
                cmd.Parameters["@project_Id"].Direction = ParameterDirection.Output;
                int i = cmd.ExecuteNonQuery();
                int projectId = Convert.ToInt32(cmd.Parameters["@project_Id"].Value != DBNull.Value ? cmd.Parameters["@project_Id"].Value : 0);
                if (i > 0)
                {
                    if (pm.budgets != null && pm.budgets.Count > 0)
                    {
                        foreach (var item in pm.budgets)
                        {
                            cmd = new NpgsqlCommand("sp_adminAddproject", con, tran);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@action", "insertProjectBudget");
                            cmd.Parameters.AddWithValue("@project_Id", projectId);
                            cmd.Parameters.AddWithValue("@heads", item.ProjectHeads);
                            cmd.Parameters.AddWithValue("@headsAmount", item.ProjectAmount);
                            i += cmd.ExecuteNonQuery();
                        }
                    }
                    if (pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage)
                    {
                        foreach (var item in pm.stages)
                        {
                            cmd = new NpgsqlCommand("sp_adminAddproject", con, tran);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@action", "insertProjectStatge");
                            cmd.Parameters.AddWithValue("@project_Id", projectId);
                            cmd.Parameters.AddWithValue("@keyPoint", item.KeyPoint);
                            cmd.Parameters.AddWithValue("@completeDate", item.CompletionDate);
                            cmd.Parameters.AddWithValue("@stageDocument", item.Document_Url);
                            i += cmd.ExecuteNonQuery();
                        }
                    }

                    if (pm.pm.ProjectType.Equals("External") && projectId > 0)
                    {
                        cmd = new NpgsqlCommand("sp_adminAddproject", con, tran);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@action", "insertExternalProject");
                        cmd.Parameters.AddWithValue("@project_Id", projectId);
                        cmd.Parameters.AddWithValue("@DepartmentName", pm.pm.ProjectDepartment);
                        cmd.Parameters.AddWithValue("@contactPerson", pm.pm.ContactPerson);
                        cmd.Parameters.AddWithValue("@address", pm.pm.Address);
                        i += cmd.ExecuteNonQuery();
                    }

                    if (pm.pm.SubOrdinate != null && pm.pm.SubOrdinate.Length > 0)
                    {
                        foreach (var item in pm.pm.SubOrdinate)
                        {
                            cmd = new NpgsqlCommand("sp_adminAddproject", con, tran);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@action", "insertSubOrdinate");
                            cmd.Parameters.AddWithValue("@project_Id", projectId);
                            cmd.Parameters.AddWithValue("@id", item);
                            cmd.Parameters.AddWithValue("@projectmanager", pm.pm.ProjectManager);
                            i += cmd.ExecuteNonQuery();
                        }
                    }
                }
                tran.Commit();
                return i > 0;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        public List<Project_model> All_Project_List(int? userId, int? limit=null, int? page=null, string filterType=null, int? id = null,string searchTerm = null,string statusFilter = null)
        {
            try
            {
                con.Open();
                List<Project_model> list = new List<Project_model>();
                cmd = new NpgsqlCommand("SELECT * FROM fn_get_all_projects(@action,@v_id,@v_projectManager,@v_filterType,@v_limit,@v_page,@v_searchTerm,@v_statusFilter)", con);
                cmd.Parameters.AddWithValue("@action", "GetAllProject");
                cmd.Parameters.AddWithValue("@v_projectManager", userId.HasValue?(object)userId:0);
                if (string.IsNullOrWhiteSpace(filterType))
                    cmd.Parameters.AddWithValue("@v_filterType", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@v_filterType", filterType);
                cmd.Parameters.AddWithValue("@v_id", id.HasValue ? id : 0);
                cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@v_searchTerm", string.IsNullOrEmpty(searchTerm) ? DBNull.Value : (object)searchTerm);
                cmd.Parameters.AddWithValue("@v_statusFilter", string.IsNullOrEmpty(statusFilter) ? DBNull.Value : (object)statusFilter);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        bool firstRow = true;
                        list.Add(new Project_model
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            ProjectTitle = rd["title"].ToString(),
                            AssignDate = Convert.ToDateTime(rd["assignDate"]),
                            CompletionDate = Convert.ToDateTime(rd["completionDate"]),
                            StartDate = Convert.ToDateTime(rd["startDate"]),
                            ApproveStatus = (bool)rd["ApproveStatus"],
                            ProjectManager = rd["name"].ToString(),
                            ProjectBudget = Convert.ToDecimal(rd["budget"]),
                            ProjectDescription = rd["description"].ToString(),
                            projectDocumentUrl = rd["ProjectDocument"].ToString(),
                            ProjectType = rd["projectType"].ToString(),
                            ProjectStage = Convert.ToBoolean(rd["stage"]),
                            CompletionDatestring = Convert.ToDateTime(rd["completionDate"]).ToString("dd-MM-yyyy"),
                            ProjectStatus = Convert.ToBoolean(rd["CompleteStatus"]),
                            AssignDateString = Convert.ToDateTime(rd["assignDate"]).ToString("dd-MM-yyyy"),
                            StartDateString = Convert.ToDateTime(rd["startDate"]).ToString("dd-MM-yyyy"),
                            createdBy = rd["createdBy"].ToString(),
                            Percentage = rd["financialStatusPercentage"].ToString(),
                            physicalcomplete = Convert.ToDecimal(rd["completionPercentage"]),
                            overallPercentage = Convert.ToDecimal(rd["overallPercentage"]),
                            completestatus = Convert.ToBoolean(rd["CompleteStatus"]),
                            projectCode = rd["projectCode"] != DBNull.Value ? rd["projectCode"].ToString() : "N/A",
                            // ✅ These three only when projectType == "external"
                            ProjectDepartment = rd["projectType"].ToString().ToLower() == "external"
        ? rd["departmentname"].ToString()
        : string.Empty,

                            ContactPerson = rd["projectType"].ToString().ToLower() == "external"
        ? rd["contactperson"].ToString()
        : string.Empty,

                            Address = rd["projectType"].ToString().ToLower() == "external"
        ? rd["address"].ToString()
        : string.Empty
                        });
                        if (firstRow)
                        {
                            list[0].Pagination = new ApiCommon.PaginationInfo
                            {
                                PageNumber = page ?? 0,
                                TotalPages = Convert.ToInt32(rd["totalpages"] != DBNull.Value ? rd["totalpages"] : 0),
                                TotalRecords = Convert.ToInt32(rd["totalrecords"] != DBNull.Value ? rd["totalrecords"] : 0),
                                PageSize = limit ?? 0
                            };
                            firstRow = false; // Optional: ensure pagination is only assigned once
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }


        #endregion /* End */ 

        #region Notice
        public List<Generate_Notice> getNoticeList(string userId)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_manageNotice", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getNoticeByManager");
                cmd.Parameters.AddWithValue("@projectManager", userId);
                con.Open();
                List<Generate_Notice> noticeList = new List<Generate_Notice>();
                var res = cmd.ExecuteReader();
                while (res.Read())
                {
                    noticeList.Add(new Generate_Notice
                    {
                        Id = (int)res["id"],
                        ProjectId = (int)res["project_id"],
                        ProjectManagerId = (int)res["empid"],
                        Attachment_Url = res["NoticeDocument"] == null ? null : res["NoticeDocument"].ToString(),
                        Notice = res["noticeDescription"].ToString(),
                        ProjectManagerImage = res["profile"].ToString(),
                        ProjectManager = res["name"].ToString(),
                        ProjectName = res["title"].ToString(),
                        noticeDate = Convert.ToDateTime(res["noticeDate"]).ToString("dd-MM-yyyy")

                    });
                }
                return noticeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        public List<Raise_Problem> getAllSubOrdinateProblem(string projectManager,int?page= null,int? limit = null, string searchTerm = null)
        {
            try
            {
                con.Open();
                List<Raise_Problem> problemList = new List<Raise_Problem>();
                Raise_Problem obj = null;
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageproblems_cursor", con, tran))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@v_action", "getAllProblemListByManager");
                    cmd.Parameters.AddWithValue("@v_projectmanager", Convert.ToInt32(projectManager));
                    cmd.Parameters.AddWithValue("@v_id", 0);
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("v_searchterm", string.IsNullOrEmpty(searchTerm) ? (object)DBNull.Value : searchTerm);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))

                    using (var sdr = fetchCmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            bool firstRow = true;
                            while (sdr.Read())
                            {
                                obj = new Raise_Problem();
                                obj.ProblemId = Convert.ToInt32(sdr["problemId"]);
                                obj.ProjectName = sdr["ProjectName"].ToString();
                                obj.Title = sdr["Title"].ToString();
                                obj.Description = sdr["Description"].ToString();
                                obj.Attchment_Url = sdr["Attachment"].ToString();
                                obj.CreatedDate = Convert.ToDateTime(sdr["CreatedDate"]).ToString("dd-MM-yyyy");
                                obj.newRequest = Convert.ToBoolean(sdr["newRequest"]);
                                obj.projectCode = sdr["projectCode"] != DBNull.Value ? sdr["projectCode"].ToString() : "N/A";
                                problemList.Add(obj);

                                if (firstRow)
                                {
                                    obj.Pagination = new ApiCommon.PaginationInfo
                                    {
                                        PageNumber = page ?? 0,
                                        TotalPages = Convert.ToInt32(sdr["totalpages"] != DBNull.Value ? sdr["totalpages"] : 0),
                                        TotalRecords = Convert.ToInt32(sdr["totalrecords"] != DBNull.Value ? sdr["totalrecords"] : 0),
                                        PageSize = limit ?? 0
                                    };
                                    firstRow = false; // Optional: ensure pagination is only assigned once
                                }
                            }
                        }
                    }
                    using(var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                return problemList;

            }
            catch (Exception ex)
            {
                throw new Exception("An error accured", ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }

        }
        public List<Raise_Problem> getAllSubOrdinateProblemById(string projectManager, int id)
        {
            try
            {
                con.Open();
                List<Raise_Problem> problemList = new List<Raise_Problem>();
                Raise_Problem obj = null;
                var cmd = new NpgsqlCommand("fn_manageproblems_cursor", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@v_action", "getAllProblemListByManagerById");
                cmd.Parameters.AddWithValue("@v_projectmanager", projectManager);
                cmd.Parameters.AddWithValue("@v_id", id);
                string cursorName = (string)cmd.ExecuteScalar();
                using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con))

                using (var sdr = fetchCmd.ExecuteReader())
                {
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            obj = new Raise_Problem();
                            obj.ProblemId = Convert.ToInt32(sdr["problemId"]);
                            obj.ProjectName = sdr["ProjectName"].ToString();
                            obj.Title = sdr["Title"].ToString();
                            obj.Description = sdr["Description"].ToString();
                            obj.Attchment_Url = sdr["Attachment"].ToString();
                            obj.CreatedDate = Convert.ToDateTime(sdr["CreatedDate"]).ToString("dd-MM-yyyy");
                            obj.newRequest = Convert.ToBoolean(sdr["newRequest"]);
                            problemList.Add(obj);
                        }
                    }
                }

                return problemList;

            }
            catch (Exception ex)
            {
                throw new Exception("An error accured", ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }

        }
        public bool CompleteSelectedProblem(int probId)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_ManageSubordinateProjectProblem", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "completeproblem");
                cmd.Parameters.AddWithValue("@id", probId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        public List<Raise_Problem> getSubOrdinateProblemforAdmin(int? limit = null, int? page = null,string searchTerm = null,int?id= null)
        {
            try
            {
                con.Open();
                List<Raise_Problem> problemList = new List<Raise_Problem>();
                Raise_Problem obj = null;
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageproblems_cursor", con, tran))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("v_action", "getAllSubOrdinateProblem");
                    cmd.Parameters.AddWithValue("v_projectmanager", 0);  // default
                    cmd.Parameters.AddWithValue("v_id", id??0);              // default
                    cmd.Parameters.AddWithValue("v_limit", limit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("v_page", page ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("v_searchterm", string.IsNullOrEmpty(searchTerm) ? (object)DBNull.Value : searchTerm);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var sdr = fetchCmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            bool firstRow = true;
                            while (sdr.Read())
                            {
                                obj = new Raise_Problem();
                                obj.ProblemId = Convert.ToInt32(sdr["problemId"]);
                                obj.ProjectName = sdr["ProjectName"].ToString();
                                obj.Title = sdr["Title"].ToString();
                                obj.Description = sdr["Description"].ToString();
                                obj.Attchment_Url = sdr["Attachment"].ToString();
                                obj.CreatedDate = Convert.ToDateTime(sdr["CreatedDate"]).ToString("dd-MM-yyyy");
                                obj.newRequest = Convert.ToBoolean(sdr["newRequest"]);
                                obj.projectCode = sdr["projectCode"] != DBNull.Value ? sdr["projectCode"].ToString() : "N/A";
                                problemList.Add(obj);

                                if (firstRow)
                                {
                                    obj.Pagination = new ApiCommon.PaginationInfo
                                    {
                                        PageNumber = page ?? 0,
                                        TotalPages = Convert.ToInt32(sdr["totalpages"] != DBNull.Value ? sdr["totalpages"] : 0),
                                        TotalRecords = Convert.ToInt32(sdr["totalrecords"] != DBNull.Value ? sdr["totalrecords"] : 0),
                                        PageSize = limit ?? 0
                                    };
                                    firstRow = false; // Optional: ensure pagination is only assigned once
                                }
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                return problemList;

            }
            catch (Exception ex)
            {
                throw new Exception("An error accured", ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }

        }
        #endregion

        #region update monthly update
        public bool UpdateMonthlyStatus(Project_MonthlyUpdate pwu)
        {
            try
            {
                using (var cmd = new NpgsqlCommand("CALL sp_manageprojectsubstaces(v_action=>@v_action, v_project_Id => @project_id, v_w_date=>@w_date, v_comment=>@comment, v_unit=>@unit, v_annual=>@annual, v_monthEnd=>@monthend, v_reviewMonth=>@reviewmonth, v_MonthEndSequentially=>@monthendseq, v_StateBeneficiaries=>@statebeneficiaries)", con))
                {
                    cmd.Parameters.Add("@v_action", NpgsqlDbType.Varchar).Value = "insertUpdate";
                    cmd.Parameters.Add("@project_id", NpgsqlDbType.Integer).Value = pwu.ProjectId;
                    cmd.Parameters.Add("@w_date", NpgsqlDbType.Date).Value = pwu.date;
                    cmd.Parameters.Add("@comment", NpgsqlDbType.Text).Value = (object)pwu.comments ?? DBNull.Value;
                    cmd.Parameters.Add("@unit", NpgsqlDbType.Varchar).Value = (object)pwu.unit ?? DBNull.Value;
                    cmd.Parameters.Add("@annual", NpgsqlDbType.Varchar).Value = (object)pwu.annual ?? DBNull.Value;
                    cmd.Parameters.Add("@monthend", NpgsqlDbType.Text).Value = (object)pwu.monthEnd ?? DBNull.Value;
                    cmd.Parameters.Add("@reviewmonth", NpgsqlDbType.Text).Value = (object)pwu.reviewMonth ?? DBNull.Value;
                    cmd.Parameters.Add("@monthendseq", NpgsqlDbType.Text).Value = (object)pwu.MonthEndSequentially ?? DBNull.Value;
                    cmd.Parameters.Add("@statebeneficiaries", NpgsqlDbType.Varchar).Value = (object)pwu.StateBeneficiaries ?? DBNull.Value;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        public List<Project_MonthlyUpdate> MonthlyProjectUpdate(int projectId)
        {
            try
            {
                con.Open();
                List<Project_Subordination> subList = new List<Project_Subordination>();
                List<Project_MonthlyUpdate> list = new List<Project_MonthlyUpdate>();
                Project_model pm = new Project_model();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageprojectsubstances_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "selectAllByProject");
                    cmd.Parameters.AddWithValue("v_project_id", projectId);
                    cmd.Parameters.AddWithValue("v_id", 0);
                    cmd.Parameters.AddWithValue("v_limit", 0);
                    cmd.Parameters.AddWithValue("v_page", 0);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var rd = fetchCmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                list.Add(new Project_MonthlyUpdate
                                {
                                    Id = Convert.ToInt32(rd["id"]),
                                    date = Convert.ToDateTime(rd["w_date"]),
                                    comments = rd["comment"].ToString(),
                                    unit = rd["unit"].ToString(),
                                    annual = rd["annual"].ToString(),
                                    monthEnd = rd["monthEnd"].ToString(),
                                    reviewMonth = rd["reviewMonth"].ToString(),
                                    MonthEndSequentially = rd["MonthEndSequentially"].ToString(),
                                    StateBeneficiaries = rd["StateBeneficiaries"].ToString(),
                                    projectName = rd["title"].ToString()
                                });
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        #endregion

        #region update monthly ExternalProject
        public bool updateFinancialReportMonthly(FinancialMonthlyReport fr)
        {
            try
            {
                cmd = new NpgsqlCommand(@"CALL public.sp_manageprojectsubstaces(
                    @action,          -- v_action
                    0,                -- v_id
                    @project_id,      -- v_project_id
                    @w_date,          -- v_w_date
                    NULL,             -- v_title
                    NULL,             -- v_comment
                    NULL,             -- v_reason
                    0,                -- v_completion
                    NULL,             -- v_attatchment
                    0,                -- v_amount
                    NULL,             -- v_unit
                    NULL,             -- v_annual
                    NULL,             -- v_monthend
                    NULL,             -- v_reviewmonth
                    NULL,             -- v_monthendsequentially
                    NULL,             -- v_statebeneficiaries
                    NULL,             -- v_createdat
                    0,                -- v_projectid
                    0,                -- v_headid
                    NULL,             -- v_updateat
                    false,            -- v_status
                    @aim,             -- v_aim
                    @month_aim,       -- v_month_aim
                    @completeinmonth, -- v_completeinmonth
                    @departbeneficiaries, -- v_departbeneficiaries
                    0,                -- v_appstatus
                    NULL              -- v_rc
                );", con);

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@action", "insertfinanceUpdate");
                cmd.Parameters.AddWithValue("@project_id", fr.projectId);
                cmd.Parameters.AddWithValue("@aim", fr.aim);
                //cmd.Parameters.AddWithValue("@v_w_date", fr.date);
                // 👇 Convert string to DateTime safely
                DateTime workDate;
                if (DateTime.TryParse(fr.date, out workDate))
                    cmd.Parameters.AddWithValue("@w_date", workDate);
                else
                    throw new Exception("Invalid date format in fr.date");
                cmd.Parameters.AddWithValue("@month_aim", fr.month_aim);
                cmd.Parameters.AddWithValue("@completeinmonth", fr.completeInMonth);
                cmd.Parameters.AddWithValue("@departbeneficiaries", fr.departBeneficiaries);
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        public List<FinancialMonthlyReport> GetExtrnlFinancialReport(int projectId)
        {
            try
            {
                List<FinancialMonthlyReport> list = new List<FinancialMonthlyReport>();
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageprojectsubstances_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "selectFinancilaReportByProjectId");
                    cmd.Parameters.AddWithValue("v_project_id", projectId);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var rd = fetchCmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                list.Add(new FinancialMonthlyReport
                                {
                                    id = Convert.ToInt32(rd["id"]),
                                    projectId = Convert.ToInt32(rd["project_Id"]),
                                    aim = rd["aim"].ToString(),
                                    date = Convert.ToDateTime(rd["f_date"]).ToString("dd/MM/yyyy"),
                                    month_aim = rd["month_aim"].ToString(),
                                    completeInMonth = rd["completeInMonth"].ToString(),
                                    departBeneficiaries = rd["departBeneficiaries"].ToString(),
                                    projectName = rd["title"].ToString(),
                                    totalBudget = rd["budget"].ToString(),
                                    description = rd["description"].ToString(),
                                    department = rd["DepartmentName"].ToString()
                                });
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        #endregion

        #region Project Expences
        public bool insertExpences(ProjectExpenses exp)
        {
            try
            {
                cmd = new NpgsqlCommand("CALL sp_ManageProjectSubstaces(v_action=>@action, v_project_id=>@project_id, v_id=>@id, v_title=>@title, v_w_date=>@w_date, v_amount=>@amount, v_attatchment=>@attatchment, v_comment =>@comment )", con);
                cmd.Parameters.AddWithValue("@action", "insertExpences");
                cmd.Parameters.AddWithValue("@project_id", exp.projectId);
                cmd.Parameters.AddWithValue("@id", exp.projectHeadId);
                cmd.Parameters.AddWithValue("@title", exp.title);
                cmd.Parameters.AddWithValue("@w_date", exp.date);
                cmd.Parameters.AddWithValue("@amount", exp.amount);
                if (string.IsNullOrEmpty(exp.attatchment_url))
                {
                    cmd.Parameters.AddWithValue("@attatchment", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@attatchment", exp.attatchment_url);
                }

                cmd.Parameters.AddWithValue("@comment", exp.description);
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        public List<ProjectExpenses> ExpencesList(int headId, int projectId, int? limit = null, int? page = null)
        {
            try
            {
                con.Open();
                List<ProjectExpenses> list = new List<ProjectExpenses>();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageprojectsubstances_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "selectExpenses");
                    cmd.Parameters.AddWithValue("v_project_id", projectId);
                    cmd.Parameters.AddWithValue("v_id", headId);
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var rd = fetchCmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                list.Add(new ProjectExpenses
                                {
                                    Id = Convert.ToInt32(rd["id"]),
                                    projectHeadId = Convert.ToInt32(rd["budgetHeadId"]),
                                    AppStatus = rd["AppStatus"] != DBNull.Value ? Convert.ToInt32(rd["AppStatus"]) : 0,
                                    AppAmount = rd["AppAmount"] != DBNull.Value ? float.Parse(rd["AppAmount"].ToString()) : 0,
                                    title = rd["title"].ToString(),
                                    date = Convert.ToDateTime(rd["insertDate"]),
                                    DateString = Convert.ToDateTime(rd["insertDate"]).ToString("dd-MM-yyyy"),
                                    amount = Convert.ToDecimal(rd["amount"]),
                                    attatchment_url = rd["attatchment"].ToString(),
                                    description = rd["description"].ToString(),
                                    reason = rd["reason"] != DBNull.Value ? rd["reason"].ToString() : "N/A"
                                });
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        public List<Project_Budget> ProjectBudgetList(int Id, int? page=null, int? limit=null)
        {

            List<Project_Budget> list = new List<Project_Budget>();
            try
            {
                con.Open();
                using (var cmd = new NpgsqlCommand("fn_getprojectstagesandbudget", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "GetBudgetByProjectId");
                    cmd.Parameters.AddWithValue("v_id", Id);
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                list.Add(new Project_Budget
                                {
                                    Id = Convert.ToInt32(rd["id"]),
                                    Project_Id = Convert.ToInt32(rd["project_id"]),
                                    ProjectHeads = rd["heads"].ToString(),
                                    ProjectAmount = Convert.ToDecimal(rd["headsAmount"] != DBNull.Value ? rd["headsAmount"] : 0),
                                    TotalAskAmount = rd["totalAskAmount"].ToString(),
                                    ApproveAmount = rd["approveAmount"].ToString(),
                                });
                            }
                        }
                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        #endregion

        #region Satges
        public bool InsertStageStatus(Project_Statge obj)
        {
            try
            {
                using (var cmd = new NpgsqlCommand(@"CALL public.sp_managestagestatus(
            @v_id,
            @v_stageid,
            @v_comment,
            @v_completionprecentage,
            @v_stagedocument,
            @v_delayreason,
            @v_updatestatus,
            @v_status,
            @v_project_id,
            @v_completionstatus,
            @v_action,
            NULL
        );", con))
                {
                    cmd.CommandType = CommandType.Text;

                    // 🔹 Explicitly specify types to avoid "unknown"
                    cmd.Parameters.Add("@v_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = 0;
                    cmd.Parameters.Add("@v_stageid", NpgsqlTypes.NpgsqlDbType.Integer).Value = obj.Stage_Id;
                    cmd.Parameters.Add("@v_comment", NpgsqlTypes.NpgsqlDbType.Text).Value = (object)obj.Comment ?? DBNull.Value;
                    cmd.Parameters.Add("@v_completionprecentage", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object)obj.CompletionPrecentage ?? DBNull.Value;
                    cmd.Parameters.Add("@v_stagedocument", NpgsqlTypes.NpgsqlDbType.Text).Value = (object)obj.StageDocument_Url ?? DBNull.Value;
                    cmd.Parameters.Add("@v_delayreason", NpgsqlTypes.NpgsqlDbType.Text).Value = DBNull.Value;
                    cmd.Parameters.Add("@v_updatestatus", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (object)obj.Status ?? DBNull.Value;
                    cmd.Parameters.Add("@v_status", NpgsqlTypes.NpgsqlDbType.Smallint).Value = 1;
                    cmd.Parameters.Add("@v_project_id", NpgsqlTypes.NpgsqlDbType.Integer).Value = obj.Project_Id;
                    cmd.Parameters.Add("@v_completionstatus", NpgsqlTypes.NpgsqlDbType.Integer).Value = 0;
                    cmd.Parameters.Add("@v_action", NpgsqlTypes.NpgsqlDbType.Varchar).Value = "insertStageStatus";

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                // ✅ If completed, update completion status
                if (obj.Status?.ToLower() == "completed")
                {
                    using (var cmd = new NpgsqlCommand(@"CALL public.sp_managestagestatus(
                0, @stageId, NULL, NULL, NULL, NULL, NULL, 1, @projectId, @completionStatus, @action, NULL
            );", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@stageId", NpgsqlTypes.NpgsqlDbType.Integer).Value = obj.Stage_Id;
                        cmd.Parameters.Add("@projectId", NpgsqlTypes.NpgsqlDbType.Integer).Value = obj.Project_Id;
                        cmd.Parameters.Add("@completionStatus", NpgsqlTypes.NpgsqlDbType.Integer).Value = 1;
                        cmd.Parameters.Add("@action", NpgsqlTypes.NpgsqlDbType.Varchar).Value = "updateStageCompetionStatus";

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting stage status", ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        public List<Project_Statge> ViewStagesComments(string stageId, int? page = null,int? limit = null)
        {
            try
            {
                List<Project_Statge> stageList = new List<Project_Statge>();
                Project_Statge stage = null;
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageprojectsubstances_cursor", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@v_action", "viewDealyReason");
                    cmd.Parameters.AddWithValue("@v_project_id", 0);
                    cmd.Parameters.AddWithValue("@v_id", Convert.ToInt32(stageId));
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var sdr = fetchCmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                stage = new Project_Statge();
                                stage.StageDocument_Url = sdr["StageDocument"].ToString();
                                stage.Comment = sdr["Comment"].ToString();
                                stage.Status = sdr["updateStatus"].ToString();
                                stage.CreatedDate = Convert.ToDateTime(sdr["CreatedDate"]).ToString("dd-MM-yyyy");
                                stageList.Add(stage);
                            }
                        }
                    }
                    using(var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }

                return stageList;
            }
            catch (Exception ex)
            {
                throw new Exception("An error accured", ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }

        }
        #endregion Create OutSource

        #region OutSource
        public bool insertOutSource(OuterSource os)
        {
            try
            {
                string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                Random rnd = new Random();
                // var userName = os.EmpName.Substring(0, 5) + "@" + os.mobileNo.ToString().PadLeft(4, '0').Substring(os.mobileNo.ToString().Length - 4);
                string userpassword = "";
                if (os.Id == 0)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        userpassword += validChars[rnd.Next(validChars.Length)];
                    }
                }
                cmd = new NpgsqlCommand("CALL sp_manageoutsource(@p_action, NULL::int, @p_empid, @p_outcode, @p_emp_name, @p_emp_mobile, @p_emp_email, @p_joiningdate, @p_emp_gender, @p_password,NULL::boolean)", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@p_action", NpgsqlTypes.NpgsqlDbType.Varchar).Value = "createOutSource";
                cmd.Parameters.Add("@p_empid", NpgsqlTypes.NpgsqlDbType.Integer).Value = os.EmpId;
                cmd.Parameters.Add("@p_outcode", NpgsqlTypes.NpgsqlDbType.Varchar).Value = os.email;
                cmd.Parameters.Add("@p_emp_name", NpgsqlTypes.NpgsqlDbType.Varchar).Value = os.EmpName;
                cmd.Parameters.Add("@p_emp_mobile", NpgsqlTypes.NpgsqlDbType.Bigint).Value = Convert.ToInt64(os.mobileNo);
                cmd.Parameters.Add("@p_emp_email", NpgsqlTypes.NpgsqlDbType.Varchar).Value = os.email;
                cmd.Parameters.Add("@p_joiningdate", NpgsqlTypes.NpgsqlDbType.Timestamp)
              .Value = Convert.ToDateTime(os.joiningdate);
                cmd.Parameters.Add("@p_emp_gender", NpgsqlTypes.NpgsqlDbType.Varchar).Value = os.gender;
                cmd.Parameters.Add("@p_password", NpgsqlTypes.NpgsqlDbType.Varchar).Value = userpassword;
                con.Open();
                cmd.ExecuteNonQuery();
                mail _mail = new mail();
                string subject = "Login Credential";
                string message = $"<p>Your user id : <b>{os.email}</b></p><br><p>Password : <b>{userpassword}</b></p>";
                _mail.SendMail(os.EmpName, os.email, subject, message);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        public List<OuterSource> selectAllOutSOurceList(int userId, int? limit = null, int? page = null,string searchTerm = null)
        {
            try
            {
                List<OuterSource> list = new List<OuterSource>();
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageOutsource_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@v_action", "selectAll");
                    cmd.Parameters.AddWithValue("@v_id", userId);
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_searchterm", string.IsNullOrEmpty(searchTerm) ? (object)DBNull.Value : (object)searchTerm);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var rd = fetchCmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            bool firstRow = true;
                            while (rd.Read())
                            {
                                var data = new OuterSource
                                {
                                    Id = Convert.ToInt32(rd["id"]),
                                    EmpName = rd["emp_name"].ToString(),
                                    mobileNo = Convert.ToInt64(rd["emp_mobile"]),
                                    email = rd["emp_email"].ToString(),
                                    joiningdate = rd["joiningdate"].ToString(),
                                    gender = rd["emp_gender"].ToString()
                                };
                                if (firstRow)
                                {
                                    data.Pagination = new ApiCommon.PaginationInfo
                                    {
                                        TotalPages = Convert.ToInt32(rd["totalpages"]),
                                        TotalRecords = Convert.ToInt32(rd["totalrecords"]),
                                        PageNumber = page ?? 0,
                                        PageSize = limit ?? 0
                                    };
                                    firstRow = false;
                                }
                                list.Add(data);
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\"", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        #endregion

        #region meetings

        public bool GetResponseFromMember(getMemberResponse mr)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd = new NpgsqlCommand("call sp_managememberresponseformeeting(v_action=>@v_action,v_appstatus=>@v_appstatus,v_reason=>@v_reason,v_meeting=>@v_meeting,v_employee=>@v_employee)", con);
                //cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@v_action", "getResponseFromMemberForMeeting");
                cmd.Parameters.AddWithValue("@v_appstatus", mr.ApprovedStatus);
                cmd.Parameters.AddWithValue("@v_reason", mr.reason??"");
                cmd.Parameters.AddWithValue("@v_meeting", mr.MeetingId);
                cmd.Parameters.AddWithValue("@v_employee", mr.MemberId);
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        public List<Meeting_Model> getAllmeeting(int id, int? limit = null, int? page = null,string searchTerm = null,string statusFilter = null)
        {
            try
            {
                List<Meeting_Model> _list = new List<Meeting_Model>();
                Meeting_Model obj = null;
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand(@"select * from fn_get_meetings(@p_action,@p_id,@v_limit,@v_page,@v_type,@v_searchTerm,@v_statusFilter);", con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@p_action", "selectMeetingForProjectManager");
                    cmd.Parameters.AddWithValue("@p_id", id);
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_type", (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_searchTerm", string.IsNullOrEmpty(searchTerm) ? DBNull.Value : (object)searchTerm);
                    cmd.Parameters.AddWithValue("@v_statusFilter", string.IsNullOrEmpty(statusFilter) ? DBNull.Value : (object)statusFilter);
                    var sdr = cmd.ExecuteReader();
                    bool firstRow = true;
                    while (sdr.Read())
                    {
                        obj = new Meeting_Model();
                        obj.Id = Convert.ToInt32(sdr["id"]);
                        obj.CompleteStatus = Convert.ToInt32(sdr["completeStatus"]);
                        obj.MeetingType = sdr["meetingType"].ToString();
                        obj.MeetingLink = sdr["meetingLink"].ToString();
                        obj.MeetingTitle = sdr["MeetingTitle"].ToString();
                        obj.AppStatus = sdr["appStatus"] != DBNull.Value ? (int)sdr["appStatus"] : 0;
                        obj.memberId = sdr["memberId"] != DBNull.Value ? sdr["memberId"].ToString().Split(',').ToList() : new List<string>();
                        obj.CreaterId = sdr["createrId"] != DBNull.Value ? Convert.ToInt32(sdr["createrId"]) : 0;
                        obj.MeetingDate = Convert.ToDateTime(sdr["meetingTime"]).ToString("dd-MM-yyyy hh:mm tt");
                        obj.createdBy = sdr["createdBy"].ToString();
                        _list.Add(obj);

                        if (firstRow)
                        {
                            obj.Pagination = new ApiCommon.PaginationInfo
                            {
                                PageNumber = page ?? 0,
                                TotalPages = Convert.ToInt32(sdr["totalpages"] != DBNull.Value ? sdr["totalpages"] : 0),
                                TotalRecords = Convert.ToInt32(sdr["totalrecords"] != DBNull.Value ? sdr["totalrecords"] : 0),
                                PageSize = limit ?? 0
                            };
                            firstRow = false; // Optional: ensure pagination is only assigned once
                        }
                    }
                }
                return _list;
            }
            catch (Exception ex)
            {
                throw new Exception("An error accured", ex);
            }
            finally
            {
                con.Close();
            }
        }
        public List<GetConclusion> getConclusionForMeeting(int meetingId, int userId, int? limit = null, int? page = null)
        {
            try
            {
                List<GetConclusion> _list = new List<GetConclusion>();
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_managemeetingconclusion_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@v_action", "selectConclusionForProjectManager");
                    cmd.Parameters.AddWithValue("@v_id", 0);
                    cmd.Parameters.AddWithValue("@v_memberid", userId);
                    cmd.Parameters.AddWithValue("@v_meeting", meetingId);
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : 0);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : 0);

                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\"; ", con, tran))
                    using (var sdr = fetchCmd.ExecuteReader())
                    {

                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                _list.Add(new GetConclusion
                                {
                                    Id = Convert.ToInt32(sdr["id"]),
                                    Conclusion = sdr["conclusion"].ToString(),
                                    FollowDate = sdr["nextFollow"] != DBNull.Value ? Convert.ToDateTime(sdr["nextFollow"]).ToString("dd-MM-yyyy") : "N/A"
                                });


                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                return _list;
            }
            catch (Exception ex)
            {
                throw new Exception("An error accured", ex);
            }
            finally
            {
                con.Close();
            }
        }

        public List<Employee_model> getMemberJoiningStatus(int meetingId)
        {
            try
            {

                List<Employee_model> meetingc = new List<Employee_model>();
                con.Open();

                using (var cmd1 = new NpgsqlCommand("SELECT * from fn_get_meetings(@p_action,@p_id);", con))
                {
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.AddWithValue("@p_action", "selectMemberJoiningStatus");
                    cmd1.Parameters.AddWithValue("@p_id", meetingId);
                    using (var rdr = cmd1.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                meetingc.Add(new Employee_model
                                {
                                    EmployeeName = rdr["name"].ToString(),
                                    Image_url = rdr["profile"].ToString(),
                                    EmployeeRole = rdr["role"].ToString(),
                                    AppStatus = rdr["appstatus"] != DBNull.Value ? (int)rdr["appstatus"] : 0,
                                    Reason = rdr["reason"] != DBNull.Value ? rdr["reason"].ToString() : "N/A",
                                });
                            }
                        }
                    }
                }

                return meetingc;
            }
            catch (Exception ex)
            {
                throw new Exception("An error accured", ex);
            }
            finally
            {
                con.Close();
                cmd.Dispose();
            }
        }

        #endregion

        #region create task
        public bool createTask(OutSourceTask ost)
        {
            con.Open();
            NpgsqlTransaction tran = con.BeginTransaction();
            try
            {
                cmd = new NpgsqlCommand("CALL sp_manageoutsourcetask(@v_action, null::int, @v_empid, @v_title, @v_description, null::text, null::smallint, @v_taskid, NULL, NULL)", con, tran);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@v_action", "createTask");
                cmd.Parameters.AddWithValue("@v_empid", ost.empId);
                cmd.Parameters.AddWithValue("@v_title", ost.title);
                cmd.Parameters.AddWithValue("@v_description", ost.description);
                var p_taskid = new NpgsqlParameter("v_taskid", NpgsqlTypes.NpgsqlDbType.Integer)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = 0 // must be set!
                };
                cmd.Parameters.Add(p_taskid);
                cmd.ExecuteNonQuery();
                int taskId = Convert.ToInt32(cmd.Parameters["@v_taskid"].Value != DBNull.Value ? cmd.Parameters["@v_taskid"].Value : 0);
                if (taskId > 0 && ost.outSourceId.Length > 0)
                {
                    foreach (var item in ost.outSourceId)
                    {
                        cmd.Dispose();
                        cmd = new NpgsqlCommand("CALL sp_manageoutsourcetask(@v_action, @v_id, @v_empid, NULL, NULL, null::text, null::smallint, NULL, NULL, NULL)", con, tran);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@v_action", "assignTask");
                        cmd.Parameters.AddWithValue("@v_empid", item);
                        cmd.Parameters.AddWithValue("@v_id", taskId);
                        cmd.ExecuteNonQuery();
                    }
                }

                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        public List<OutSourceTask> taskList(int empId, int? limit = null, int? page = null,string searchTerm = null)
        {
            try
            {
                List<OutSourceTask> list = new List<OutSourceTask>();
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageoutsource_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@v_action", "selectAllTask");
                    cmd.Parameters.AddWithValue("@v_id", empId);
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("v_searchterm", string.IsNullOrEmpty(searchTerm) ? DBNull.Value : (object)searchTerm);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var rd = fetchCmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            bool firstRow = true;
                            while (rd.Read())
                            {
                                var data = new OutSourceTask
                                {
                                    Id = Convert.ToInt32(rd["id"]),
                                    title = rd["title"].ToString(),
                                    description = rd["description"].ToString(),
                                    completeStatus = Convert.ToBoolean(rd["completeStatus"])
                                };

                                if (firstRow)
                                {
                                    data.Pagination = new ApiCommon.PaginationInfo
                                    {
                                        TotalPages = Convert.ToInt32(rd["totalpages"]),
                                        TotalRecords = Convert.ToInt32(rd["totalrecords"]),
                                        PageNumber = page ?? 0,
                                        PageSize = limit ?? 0
                                    };
                                    firstRow = false;
                                }
                                list.Add(data);
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        public List<OuterSource> ViewOutSourceList(int taskId)
        {
            try
            {
                List<OuterSource> list = new List<OuterSource>();
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageoutsource_cursor", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@v_action", "ViewTaskEmpStatus");
                    cmd.Parameters.AddWithValue("@v_id", taskId);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var rd = fetchCmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                list.Add(new OuterSource
                                {
                                    Id = Convert.ToInt32(rd["id"]),
                                    EmpName = rd["emp_name"].ToString(),
                                    mobileNo = Convert.ToInt64(rd["emp_mobile"]),
                                    email = rd["emp_email"].ToString(),
                                    gender = rd["emp_gender"].ToString(),
                                    completeStatus = Convert.ToBoolean(rd["completeStatus"]),
                                    message = rd["response"].ToString()
                                });
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\"", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        public bool updateTaskStatus(int taskId)
        {
            try
            {
                cmd = new NpgsqlCommand("CALL sp_manageoutsourcetask(@v_action, @v_id, NULL, NULL, NULL, NULL::text, NULL::smallint, NULL, NULL, NULL)", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@v_action", "updateTaskStatus");
                cmd.Parameters.AddWithValue("@v_id", taskId);
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        #endregion

        #region insertReimbursement

        public bool insertReimbursement(Reimbursement data)
        {
            try
            {
                cmd = new NpgsqlCommand("CALL sp_Reimbursement(p_vrno => @vrNo, p_date => @date, p_particulars => @particulars, p_items => @items, p_userid => @userId, p_amount => @amount, p_purpose => @purpose, p_type => @type, p_action => @action)", con);
                cmd.Parameters.AddWithValue("@vrNo", data.vrNo);
                cmd.Parameters.AddWithValue("@date", data.date);
                cmd.Parameters.AddWithValue("@particulars", data.particulars);
                cmd.Parameters.AddWithValue("@items", data.items);
                cmd.Parameters.AddWithValue("@userId", data.userId);
                cmd.Parameters.AddWithValue("@amount", data.amount);
                cmd.Parameters.AddWithValue("@purpose", data.purpose);
                cmd.Parameters.AddWithValue("@type", data.type);
                cmd.Parameters.AddWithValue("@action", "insert");
                con.Open();
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }

        public bool submitReinbursementForm(string type, int userId, int Id)
        {
            try
            {
                cmd = new NpgsqlCommand("CALL sp_Reimbursement(p_action => @action, p_userid => @userId, p_type => @type, p_id => @id)", con);
                cmd.Parameters.AddWithValue("@action", "submitReinbursementForm");
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@id", Id);
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }
        public List<Reimbursement> GetReimbursements(int? page = null, int? limit = null, int? id = null, int? managerId = null, string type = null,string typeFilter = null,string statusFilter = null)
        {
            try
            {
                con.Open();
                List<Reimbursement> getlist = new List<Reimbursement>();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_managereimbursement_cursor", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "selectAll");
                    cmd.Parameters.AddWithValue("v_projectmanager", managerId.HasValue ? managerId : 0);
                    cmd.Parameters.AddWithValue("v_id", id.HasValue ? id : 0);
                    if (string.IsNullOrWhiteSpace(type))
                    {
                        cmd.Parameters.AddWithValue("v_type", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("v_type", type);
                    }
                    cmd.Parameters.AddWithValue("v_limit", limit.HasValue ? limit : 0);
                    cmd.Parameters.AddWithValue("v_page", page.HasValue ? page : 0);
                    cmd.Parameters.AddWithValue("v_typefilter", string.IsNullOrEmpty(typeFilter) ? DBNull.Value : (object)typeFilter);
                    cmd.Parameters.AddWithValue("v_statusfilter", string.IsNullOrEmpty(statusFilter) ? DBNull.Value: (object)statusFilter);

                    string cursorName = (string)cmd.ExecuteScalar();

                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var res = fetchCmd.ExecuteReader())
                    {
                        if (res.HasRows)
                        {
                            bool firstRow = true;
                            while (res.Read())
                            {
                                var data = new Reimbursement
                                {
                                    EmpName = res["name"].ToString() + $"({res["employeeCode"].ToString()})",
                                    type = res["type"].ToString(),
                                    id = Convert.ToInt32(res["id"]),
                                    amount = Convert.ToDecimal(res["amount"]),
                                    userId = Convert.ToInt32(res["userId"]),
                                    apprstatus = Convert.ToBoolean(res["Apprstatus"]),
                                    subStatus = Convert.ToBoolean(res["SaveStatus"]),
                                    adminappr = Convert.ToBoolean(res["Apprstatus"]),
                                    status = Convert.ToBoolean(res["apprAmountStatus"] != DBNull.Value ? res["apprAmountStatus"] : false),
                                    chequeNum = res["chequeNum"].ToString(),
                                    accountNewRequest = Convert.ToBoolean(res["accountNewRequest"]),
                                    chequeDate = res["chequeDate"] != DBNull.Value ? Convert.ToDateTime(res["chequeDate"]).ToString("dd/MM/yyyy") : "",
                                    newRequest = Convert.ToBoolean(res["newStatus"]),
                                    approveAmount = Convert.ToDecimal(res["apprAmt"] != DBNull.Value ? res["apprAmt"] : 0),
                                    remark = res["remark"].ToString(),
                                    statusLabel = !Convert.ToBoolean(res["newStatus"]) && Convert.ToBoolean(res["Apprstatus"]) ? "Approved": !Convert.ToBoolean(res["newStatus"]) && !Convert.ToBoolean(res["Apprstatus"])? "Rejected" : !Convert.ToBoolean(res["newStatus"]) && !Convert.ToBoolean(res["Apprstatus"]) ? "Pending":""
                                };

                                if (firstRow)
                                {
                                    data.Pagination = new ApiCommon.PaginationInfo
                                    {
                                        TotalPages = Convert.ToInt32(res["totalpages"]),
                                        TotalRecords = Convert.ToInt32(res["totalrecords"]),
                                        PageSize = limit ?? 0,
                                        PageNumber = page ?? 0
                                    };
                                    firstRow = false;
                                }
                                getlist.Add(data);
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                return getlist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }

        public List<Reimbursement> GetSpecificUserReimbursements(int userid, string type, int id, int? page, int? limit)
        {
            try
            {
                con.Open();
                List<Reimbursement> getlist = new List<Reimbursement>();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageReimbursement_cursor", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "GetSpecificTypeData");
                    cmd.Parameters.AddWithValue("v_projectmanager", userid);
                    cmd.Parameters.AddWithValue("v_id", id);
                    if (string.IsNullOrWhiteSpace(type))
                    {
                        cmd.Parameters.AddWithValue("v_type", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("v_type", type);
                    }
                    cmd.Parameters.AddWithValue("v_limit", limit.HasValue ? limit : 0);
                    cmd.Parameters.AddWithValue("v_page", page.HasValue ? page : 0);

                    string cursorName = (string)cmd.ExecuteScalar();

                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var res = fetchCmd.ExecuteReader())
                    {
                        if (res.HasRows)
                        {
                            bool firstRow = true;
                            while (res.Read())
                            {
                                var data = new Reimbursement
                                {
                                    id = Convert.ToInt32(res["id"]),
                                    type = res["type"].ToString(),
                                    vrNo = res["vrNo"].ToString(),
                                    date = Convert.ToDateTime(res["date"]),
                                    particulars = res["particulars"].ToString(),
                                    items = res["items"].ToString(),
                                    amount = Convert.ToDecimal(res["amount"]),
                                    purpose = res["purpose"].ToString(),
                                    status = Convert.ToBoolean(res["status"]),
                                    newRequest = Convert.ToBoolean(res["newRequest"]),
                                    adminappr = Convert.ToBoolean(res["admin_appr"])
                                };

                                if (firstRow)
                                {
                                    data.Pagination = new ApiCommon.PaginationInfo
                                    {
                                        TotalPages = Convert.ToInt32(res["totalpages"]),
                                        TotalRecords = Convert.ToInt32(res["totalrecords"]),
                                        PageNumber = page ?? 0,
                                        PageSize = limit ?? 0
                                    };
                                    firstRow = false;
                                }
                                getlist.Add(data);
                            }
                        }
                    }

                    return getlist;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }

        #endregion

        #region tourproposal

        public bool insertTour(tourProposal data)
        {
            try
            {
                cmd = new NpgsqlCommand("CALL sp_Tourproposal(v_userid => @userId, v_projectid => @projectId, v_dateofdept => @dateOfDept, v_place => @place, v_periodfrom => @periodFrom, v_periodto => @periodTo, v_returndate => @returnDate, v_purpose => @purpose, v_action => @action)", con);
                cmd.Parameters.AddWithValue("@userId", data.userId);
                cmd.Parameters.AddWithValue("@projectId", data.projectId);
                cmd.Parameters.AddWithValue("@dateOfDept", data.dateOfDept);
                cmd.Parameters.AddWithValue("@place", data.place);
                cmd.Parameters.AddWithValue("@periodFrom", data.periodFrom);
                cmd.Parameters.AddWithValue("@periodTo", data.periodTo);
                cmd.Parameters.AddWithValue("@returnDate", data.returnDate);
                cmd.Parameters.AddWithValue("@purpose", data.purpose);
                cmd.Parameters.AddWithValue("@action", "insert");
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }

        public List<tourProposal> getTourList(int? userId = null, int? id = null, string type = null, int? page = null, int? limit = null, int? projectFilter = null,string statusFilter = null)
        {
            try
            {
                con.Open();
                List<tourProposal> getlist = new List<tourProposal>();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_managetourproposal_cursor", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "selectAlltour");
                    cmd.Parameters.AddWithValue("v_projectmanager", userId.HasValue ? userId : 0);
                    cmd.Parameters.AddWithValue("v_id", id.HasValue ? id  : 0);
                    cmd.Parameters.AddWithValue("v_projectid", projectFilter.HasValue ? projectFilter : 0);
                    cmd.Parameters.AddWithValue("v_statusfilter", string.IsNullOrEmpty(statusFilter) ? DBNull.Value : (object)statusFilter);
                    if (string.IsNullOrWhiteSpace(type))
                    {
                        cmd.Parameters.AddWithValue("v_type", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("v_type", type);
                    }
                    cmd.Parameters.AddWithValue("v_limit", limit.HasValue ? limit : 0);
                    cmd.Parameters.AddWithValue("v_page", page.HasValue ? page : 0);

                    string cursorName = (string)cmd.ExecuteScalar();

                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var res = fetchCmd.ExecuteReader())
                    {
                        if (res.HasRows)
                        {
                            bool firstRow = true;
                            while (res.Read())
                            {
                                var data = new tourProposal
                                {
                                    id = Convert.ToInt32(res["id"]),
                                    projectName = Convert.ToString(res["title"]),
                                    dateOfDept = Convert.ToDateTime(res["dateOfDept"]),
                                    place = Convert.ToString(res["place"]),
                                    periodFrom = Convert.ToDateTime(res["periodFrom"]),
                                    periodTo = Convert.ToDateTime(res["periodTo"]),
                                    returnDate = Convert.ToDateTime(res["returnDate"]),
                                    purpose = Convert.ToString(res["purpose"]),
                                    newRequest = Convert.ToBoolean(res["newRequest"]),
                                    adminappr = Convert.ToBoolean(res["adminappr"]),
                                    projectCode = res["projectCode"] != DBNull.Value ? res["projectCode"].ToString() : "N/A"
                                };
                                if (firstRow)
                                {
                                    data.Pagination = new ApiCommon.PaginationInfo
                                    {
                                        TotalPages = Convert.ToInt32(res["totalpages"]),
                                        TotalRecords = Convert.ToInt32(res["totalrecords"]),
                                        PageSize = limit ?? 0,
                                        PageNumber = page ?? 0
                                    };
                                    firstRow = false;
                                }
                                getlist.Add(data);
                            }
                        }
                        return getlist;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }
        #endregion

        #region Hiring vehicle

        public List<HiringVehicle> getProjectList(int userId)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_HiringVehicle", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectProject");
                cmd.Parameters.AddWithValue("@userId", userId);
                con.Open();
                List<HiringVehicle> projectList = new List<HiringVehicle>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        projectList.Add(new HiringVehicle
                        {
                            projectId = res["id"] != null ? Convert.ToInt32(res["id"]) : 0,
                            projectName = res["title"] != DBNull.Value ? res["title"].ToString() : ""
                        });
                    }
                }
                return projectList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }

        public bool insertHiring(HiringVehicle data)
        {
            try
            {
                cmd = new NpgsqlCommand("CALL sp_HiringVehicle(v_hid => @hid, v_amount => @amount, v_userid => @userId, v_projectid => @projectId, v_datefrom => @dateFrom, v_dateto => @dateTo, v_proposedplace=> @proposedPlace, v_purposeofvisit=>@purposeOfVisit, v_totaldaysnight=>@totalDaysNight, v_totalplainhills=>@totalPlainHills,v_taxi=>@taxi,v_bookagainstcentre=>@BookAgainstCentre,v_note =>@note, v_action=>@action )", con);
                cmd.Parameters.AddWithValue("@hid", data.headId);
                cmd.Parameters.AddWithValue("@amount", data.amount);
                cmd.Parameters.AddWithValue("@userId", data.userId);
                cmd.Parameters.AddWithValue("@projectId", data.projectId);
                cmd.Parameters.AddWithValue("@dateFrom", data.dateFrom);
                cmd.Parameters.AddWithValue("@dateTo", data.dateTo);
                cmd.Parameters.AddWithValue("@proposedPlace", data.proposedPlace);
                cmd.Parameters.AddWithValue("@purposeOfVisit", data.purposeOfVisit);
                cmd.Parameters.AddWithValue("@totalDaysNight", data.totalDaysNight);
                cmd.Parameters.AddWithValue("@totalPlainHills", data.totalPlainHills);
                cmd.Parameters.AddWithValue("@taxi", data.taxi);
                cmd.Parameters.AddWithValue("@BookAgainstCentre", data.BookAgainstCentre);
                cmd.Parameters.AddWithValue("@note", data.note);
                cmd.Parameters.AddWithValue("@action", "insert");
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }

        public List<HiringVehicle> GetHiringVehicles(int? userId = null, int? id = null, string type = null, int? page = null, int? limit = null, int? projectFilter = null,string statusFilter = null)
        {
            try
            {
                con.Open();
                List<HiringVehicle> hiringList = new List<HiringVehicle>();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_managehiringvehicle_cursor", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "selectAllHiring");
                    cmd.Parameters.AddWithValue("v_projectmanager", userId.HasValue ? userId : 0);
                    cmd.Parameters.AddWithValue("v_projectid", projectFilter.HasValue ? projectFilter : 0);
                    cmd.Parameters.AddWithValue("v_id", id.HasValue ? id : 0);
                    if (string.IsNullOrWhiteSpace(type))
                    {
                        cmd.Parameters.AddWithValue("v_type", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("v_type", type);
                    }
                    cmd.Parameters.AddWithValue("v_limit", limit.HasValue ? limit : 0);
                    cmd.Parameters.AddWithValue("v_page", page.HasValue ? page : 0);
                    cmd.Parameters.AddWithValue("v_statusfilter", string.IsNullOrEmpty(statusFilter) ? DBNull.Value : (object)statusFilter);
                    string cursorName = (string)cmd.ExecuteScalar();

                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var res = fetchCmd.ExecuteReader())
                    {

                        if (res.HasRows)
                        {
                            while (res.Read())
                            {
                                hiringList.Add(new HiringVehicle
                                {
                                    id = (int)res["id"],
                                    projectName = Convert.ToString(res["title"]),
                                    headName = Convert.ToString(res["heads"]),
                                    amount = Convert.ToDecimal(res["amount"]),
                                    dateFrom = Convert.ToDateTime(res["dateFrom"]),
                                    dateTo = Convert.ToDateTime(res["dateTo"]),
                                    proposedPlace = res["proposedPlace"].ToString(),
                                    purposeOfVisit = res["purposeOfVisit"].ToString(),
                                    totalDaysNight = res["totalDaysNight"].ToString(),
                                    totalPlainHills = res["totalPlainHills"].ToString(),
                                    taxi = res["taxi"].ToString(),
                                    BookAgainstCentre = res["BookAgainstCentre"].ToString(),
                                    availbilityOfFund = res["availbilityOfFund"].ToString(),
                                    note = res["note"].ToString(),
                                    newRequest = Convert.ToBoolean(res["newRequest"]),
                                    adminappr = Convert.ToBoolean(res["adminappr"]),
                                    projectCode = res["projectCode"] != DBNull.Value ? res["projectCode"].ToString() : "N/A"
                                });
                            }
                        }
                        return hiringList;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }
        public List<HiringVehicle> GetHiringList(int id)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_HiringVehicle", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectOne");
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                List<HiringVehicle> hiringList = new List<HiringVehicle>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        hiringList.Add(new HiringVehicle
                        {
                            id = Convert.ToInt32(res["id"]),
                            projectName = Convert.ToString(res["title"]),
                            headName = Convert.ToString(res["heads"]),
                            amount = Convert.ToDecimal(res["amount"]),
                            dateFrom = Convert.ToDateTime(res["dateFrom"]),
                            dateTo = Convert.ToDateTime(res["dateTo"]),
                            proposedPlace = (string)res["proposedPlace"],
                            purposeOfVisit = (string)res["purposeOfVisit"],
                            totalDaysNight = (string)res["totalDaysNight"],
                            totalPlainHills = Convert.ToString(res["totalPlainHills"]),
                            taxi = (string)res["taxi"],
                            BookAgainstCentre = (string)res["BookAgainstCentre"],
                            note = res["note"].ToString()
                        });
                    }
                }
                return hiringList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }

        #endregion

        #region /* Get Heads */
        public List<HiringVehicle> getHead(int id)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_HiringVehicle", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectHead");
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                List<HiringVehicle> getList = new List<HiringVehicle>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        getList.Add(new HiringVehicle
                        {
                            headId = (int)res["hid"],
                            headName = (string)res["headName"]
                        });
                    }
                }
                return getList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }
        #endregion

        public List<Reimbursement> reinbursementReport(int userId)
        {
            try
            {
                List<Reimbursement> list = new List<Reimbursement>();
                cmd = new NpgsqlCommand("sp_Reimbursement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectReinbursementforUSerReport");
                cmd.Parameters.AddWithValue("@userid", userId);
                con.Open();
                NpgsqlDataReader res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        list.Add(new Reimbursement
                        {
                            id = Convert.ToInt32(res["id"]),
                            type = res["type"].ToString(),
                            amount = Convert.ToDecimal(res["amount"]),
                            subStatus = Convert.ToBoolean(res["SaveStatus"]),
                            adminappr = Convert.ToBoolean(res["Apprstatus"]),
                            userId = Convert.ToInt32(res["userId"]),
                            chequeNum = res["chequeNum"].ToString(),
                            chequeDate = res["chequeDate"] != DBNull.Value ? Convert.ToDateTime(res["chequeDate"]).ToString("dd/MM/yyyy") : "",
                            newRequest = Convert.ToBoolean(res["newStatus"]),
                            approveAmount = Convert.ToDecimal(res["apprAmt"] != DBNull.Value ? res["apprAmt"] : 0)
                        });
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        #region Report for App
        public List<HiringVehicle> ProjectManagerHiringReportProjects(int userId)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_HiringVehicle", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectProjectManagerHiringReportProjects");
                cmd.Parameters.AddWithValue("@userId", userId);
                con.Open();
                List<HiringVehicle> projectList = new List<HiringVehicle>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        projectList.Add(new HiringVehicle
                        {
                            projectId = (int)res["projectId"],
                            projectName = (string)res["title"]
                        });
                    }
                }
                return projectList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }
        public List<HiringVehicle> ProjectManagerHiringReportbyProjects(int userId, int projectId)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_HiringVehicle", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectProjectManagerHiringReportbyProjects");
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@projectId", projectId);
                con.Open();
                List<HiringVehicle> projectList = new List<HiringVehicle>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        projectList.Add(new HiringVehicle
                        {
                            id = (int)res["id"],
                            projectName = Convert.ToString(res["title"]),
                            headName = Convert.ToString(res["heads"]),
                            amount = Convert.ToDecimal(res["amount"]),
                            dateFrom = Convert.ToDateTime(res["dateFrom"]),
                            dateTo = Convert.ToDateTime(res["dateTo"]),
                            proposedPlace = res["proposedPlace"].ToString(),
                            purposeOfVisit = res["purposeOfVisit"].ToString(),
                            totalDaysNight = res["totalDaysNight"].ToString(),
                            totalPlainHills = res["totalPlainHills"].ToString(),
                            taxi = res["taxi"].ToString(),
                            BookAgainstCentre = res["BookAgainstCentre"].ToString(),
                            availbilityOfFund = res["availbilityOfFund"].ToString(),
                            note = res["note"].ToString(),
                            newRequest = Convert.ToBoolean(res["newRequest"]),
                            adminappr = Convert.ToBoolean(res["adminappr"])
                        });
                    }
                }
                return projectList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }

        public List<tourProposal> ProjectManagertourreportProjects(int userId)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_Tourproposal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "ProjectManagertourreportProjects");
                cmd.Parameters.AddWithValue("@userId", userId);
                con.Open();
                List<tourProposal> projectList = new List<tourProposal>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        projectList.Add(new tourProposal
                        {
                            projectId = (int)res["projectId"],
                            projectName = (string)res["title"]
                        });
                    }
                }
                return projectList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }
        public List<tourProposal> ProjectManagertourreportByProjects(int userId, int projectId)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_Tourproposal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectProjectManagertourreportByProjects");
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@projectId", projectId);
                con.Open();
                List<tourProposal> getlist = new List<tourProposal>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        getlist.Add(new tourProposal
                        {
                            id = Convert.ToInt32(res["id"]),
                            projectName = Convert.ToString(res["title"]),
                            dateOfDept = Convert.ToDateTime(res["dateOfDept"]),
                            place = Convert.ToString(res["place"]),
                            periodFrom = Convert.ToDateTime(res["periodFrom"]),
                            periodTo = Convert.ToDateTime(res["periodTo"]),
                            returnDate = Convert.ToDateTime(res["returnDate"]),
                            purpose = Convert.ToString(res["purpose"]),
                            newRequest = Convert.ToBoolean(res["newRequest"]),
                            adminappr = Convert.ToBoolean(res["adminappr"])
                        });
                    }
                }
                return getlist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }
        #endregion

        #region Raise Problem Full Crud
        public List<RaiseProblem> getProjectListForProblem(int userId)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_raiseProblem", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectproject");
                cmd.Parameters.AddWithValue("@userId", userId);
                con.Open();
                List<RaiseProblem> projectList = new List<RaiseProblem>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        projectList.Add(new RaiseProblem
                        {
                            projectId = (int)res["id"],
                            projectname = (string)res["title"]
                        });
                    }
                }
                return projectList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }
        public bool insertRaisedProblem(RaiseProblem rp)
        {
            try
            {
                cmd = new NpgsqlCommand(@"
                CALL public.sp_raiseproblem(
                    @v_action,
                    NULL,              -- v_id
                    @v_projectid,
                    @v_title,
                    @v_description,
                    @v_document,
                    NULL,              -- v_createdat
                    NULL,              -- v_updatedat
                    NULL,              -- v_adminappr
                    NULL,              -- v_newrequest
                    NULL,              -- v_status
                    NULL,              -- v_projectname
                    @v_userid,
                    NULL               -- v_rc
                )", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@v_action", "insert");
                cmd.Parameters.AddWithValue("@v_title", rp.title);
                cmd.Parameters.AddWithValue("@v_projectid", rp.projectId);
                cmd.Parameters.AddWithValue("@v_description", rp.description);
                cmd.Parameters.AddWithValue("@v_document", rp.documentname??"");
                cmd.Parameters.AddWithValue("@v_userid", rp.id);
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        public List<RaiseProblem> getProblems(int userId, int? limit = null, int? page = null)
        {
            try
            {
                List<RaiseProblem> list = new List<RaiseProblem>();
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageproblems_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "selectProblemsforAdmin");
                    cmd.Parameters.AddWithValue("v_projectmanager", userId);
                    cmd.Parameters.AddWithValue("v_id", 0);
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var res = fetchCmd.ExecuteReader())
                    {
                        if (res.HasRows)
                        {
                            bool firstRow = true;
                            while (res.Read())
                            {
                                list.Add(new RaiseProblem
                                {
                                    id = Convert.ToInt32(res["id"]),
                                    title = res["title"].ToString(),
                                    description = res["description"].ToString(),
                                    adminappr = Convert.ToBoolean(res["adminappr"]),
                                    newRequest = Convert.ToBoolean(res["newRequest"]),
                                    documentname = res["document"].ToString(),
                                    projectname = res["projectName"].ToString(),
                                    createdAt = Convert.ToDateTime(res["createdAt"]),
                                    projectCode = res["projectCode"] != DBNull.Value ? res["projectCode"].ToString() : "N/A"
                                });
                                if (firstRow)
                                {
                                    list[0].Pagination = new ApiCommon.PaginationInfo
                                    {
                                        PageNumber = page ?? 0,
                                        TotalPages = Convert.ToInt32(res["totalpages"] != DBNull.Value ? res["totalpages"] : 0),
                                        TotalRecords = Convert.ToInt32(res["totalrecords"] != DBNull.Value ? res["totalrecords"] : 0),
                                        PageSize = limit ?? 0
                                    };
                                    firstRow = false; // Optional: ensure pagination is only assigned once
                                }
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        public bool deleteRaisedProblem(int id, int userId)
        {
            try
            {
                cmd = new NpgsqlCommand(@"
                CALL public.sp_raiseproblem(
                    @v_action,
                    @v_id,
                    NULL,
                    NULL,
                    NULL,
                    NULL,
                    NULL,              -- v_createdat
                    NULL,              -- v_updatedat
                    NULL,              -- v_adminappr
                    NULL,              -- v_newrequest
                    NULL,              -- v_status
                    NULL,              -- v_projectname
                    @v_userid,
                    NULL               -- v_rc
                )", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@v_action", "delete");
                cmd.Parameters.AddWithValue("@v_id", id);
                cmd.Parameters.AddWithValue("@v_userid", userId);
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        #endregion

        #region Attendance Manage
        public (bool success, string error) insertAttendance(AttendanceManage am)
        {
            try
            {
                con.Open();
                // Check if already exists
                NpgsqlCommand checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM ManageAttendance WHERE EmpId = @EmpId AND attendancedate = @Date", con);
                checkCmd.Parameters.AddWithValue("@EmpId", am.EmpId);
                checkCmd.Parameters.AddWithValue("@Date", am.attendanceDate);
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    return (true, null);
                }
                cmd = new NpgsqlCommand("sp_ManageAttendance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "insertOutsource");
                cmd.Parameters.AddWithValue("@EmpId", am.EmpId);
                cmd.Parameters.AddWithValue("@address", am.address);
                cmd.Parameters.AddWithValue("@longitude", am.longitude);
                cmd.Parameters.AddWithValue("@latitude", am.latitude);
                cmd.Parameters.AddWithValue("@attendancestatus", am.attendanceStatus);
                cmd.Parameters.AddWithValue("@attendancedate", am.attendanceDate);
                cmd.Parameters.AddWithValue("@projectManager", am.projectManager);

                int res = cmd.ExecuteNonQuery();
                if (res > 0)
                {
                    return (true, "Added Successfully");
                }
                else
                {
                    return (false, "Server Error");
                }
            }
            catch
            {
                return (false, "Error Occured");
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        public (bool success, List<string> skippedDates, string error) InsertAttendance(AttendanceManage model)
        {
            List<string> skippedDates = new List<string>();
            try
            {

                con.Open();
                foreach (var item in model.Attendance)
                {
                    using (var tran = con.BeginTransaction())
                    {
                        int count = 0;
                        // Check if already exists
                        using (var checkCmd = new NpgsqlCommand("select fn_manageattendance_cursor(@v_action,NULL::int,@v_id,NULL::int,NULL::int,NULL::int,NULL::int,@v_date)", con, tran))
                        {
                            checkCmd.Parameters.AddWithValue("@v_action", "checkattendance");
                            checkCmd.Parameters.AddWithValue("@v_id", model.EmpId);
                            checkCmd.Parameters.AddWithValue("@v_date", DateTime.Parse(item.Key));
                            string cursorName = (string)checkCmd.ExecuteScalar();

                            if (string.IsNullOrEmpty(cursorName))
                            {
                                // If no cursor returned, move to the next record
                                continue;
                            }

                            // Fetch the results from the cursor
                            using (var fetchCmd = new NpgsqlCommand($"FETCH ALL FROM \"{cursorName}\"", con, tran))
                            {
                                count = Convert.ToInt32(fetchCmd.ExecuteScalar());
                            }
                        }


                        if (count > 0)
                        {
                            skippedDates.Add(item.Key); // Already exists
                            continue;
                        }

                        // Insert only if not exists
                        NpgsqlCommand cmd = new NpgsqlCommand(@"
                            call sp_manageattendance(
                                0,
                                @v_projectmanager,
                                @v_empid,
                                NULL::varchar,                     -- address
                                NULL::varchar,                     -- longitude
                                NULL::varchar,                     -- latitude
                                @v_attendancestatus,
                                @v_attendancedate,
                                NULL::boolean,                     -- v_projectmanagerappr
                                NULL::timestamp without time zone, -- v_createdat
                                NULL::timestamp without time zone, -- v_updatedat
                                NULL::boolean,                     -- v_newrequest
                                NULL::boolean,                     -- v_status
                                NULL::varchar,                     -- v_remark
                                NULL::integer,                     -- v_month
                                NULL::integer,                     -- v_year
                                @v_action
                            );", con, tran);

                        cmd.CommandType = System.Data.CommandType.Text;

                        // ✅ Add parameters with explicit PostgreSQL types
                        cmd.Parameters.Add("@v_action", NpgsqlDbType.Varchar).Value = "InsertAttendance";
                        cmd.Parameters.Add("@v_projectmanager", NpgsqlDbType.Integer).Value = (int)model.projectManager;
                        cmd.Parameters.Add("@v_empid", NpgsqlDbType.Integer).Value = (int)model.EmpId;
                        cmd.Parameters.Add("@v_attendancedate", NpgsqlDbType.Timestamp).Value = Convert.ToDateTime(item.Key);  // DateTime or DateOnly
                        cmd.Parameters.Add("@v_attendancestatus", NpgsqlDbType.Varchar).Value = item.Value;
                        cmd.ExecuteNonQuery();

                        tran.Commit();
                    }
                }

                return (true, skippedDates, null);
            }
            catch (Exception ex)
            {
                return (false, skippedDates, ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        public List<AttendanceManage> GetAllAttendanceForOutsource(int EmpId)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_ManageAttendance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", EmpId);
                cmd.Parameters.AddWithValue("@action", "getallforoutsorce");
                List<AttendanceManage> list = new List<AttendanceManage>();
                con.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        list.Add(new AttendanceManage
                        {
                            id = Convert.ToInt32(rd["id"]),
                            EmpId = Convert.ToInt32(rd["EmpId"]),
                            projectManager = Convert.ToInt32(rd["projectManager"]),
                            address = rd["address"].ToString(),
                            longitude = rd["longitude"].ToString(),
                            latitude = rd["latitude"].ToString(),
                            createdAt = Convert.ToDateTime(rd["createdAt"]),
                            attendanceDate = Convert.ToDateTime(rd["attendancedate"]),
                            attendanceStatus = rd["attendancestatus"].ToString(),
                            newRequest = Convert.ToBoolean(rd["newRequest"]),
                            remark = rd["remark"].ToString(),
                            projectManagerAppr = Convert.ToBoolean(rd["projectManagerAppr"])
                        });
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        public List<AttendanceManage> GetAllAttendanceForProjectManager(int projectManager, int EmpId)
        {
            try
            {
                List<AttendanceManage> list = new List<AttendanceManage>();
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageattendance_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_id", EmpId);
                    cmd.Parameters.AddWithValue("v_projectmanager", projectManager);
                    cmd.Parameters.AddWithValue("v_action", "getallforprojectmanager");
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var rd = fetchCmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                list.Add(new AttendanceManage
                                {
                                    id = Convert.ToInt32(rd["id"]),
                                    EmpId = Convert.ToInt32(rd["EmpId"]),
                                    projectManager = Convert.ToInt32(rd["projectManager"]),
                                    createdAt = Convert.ToDateTime(rd["createdAt"]),
                                    attendanceDate = Convert.ToDateTime(rd["attendancedate"]),
                                    attendanceStatus = rd["attendancestatus"].ToString(),
                                    newRequest = Convert.ToBoolean(rd["newRequest"]),
                                    projectManagerName = rd["projectManagerName"].ToString(),
                                    EmpName = rd["EmpName"].ToString(),
                                    remark = rd["remark"].ToString(),
                                    absent = Convert.ToInt32(rd["TotalAbsent"]),
                                    present = Convert.ToInt32(rd["TotalPresent"]),
                                    projectManagerAppr = Convert.ToBoolean(rd["projectManagerAppr"])
                                });
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        public List<AttendanceManage> GetAllAttendanceForProjectManager(int projectManager, int? limit = null, int? page = null)
        {
            try
            {
                List<AttendanceManage> list = new List<AttendanceManage>();
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageattendance_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@v_action", "getallforprojectmanager");
                    cmd.Parameters.AddWithValue("@v_projectmanager", projectManager);
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);

                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var rd = fetchCmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                list.Add(new AttendanceManage
                                {
                                    id = Convert.ToInt32(rd["id"]),
                                    EmpId = Convert.ToInt32(rd["EmpId"]),
                                    projectManager = Convert.ToInt32(rd["projectManager"]),
                                    createdAt = Convert.ToDateTime(rd["createdAt"]),
                                    attendanceDate = Convert.ToDateTime(rd["attendancedate"]),
                                    attendanceStatus = rd["attendancestatus"].ToString(),
                                    newRequest = Convert.ToBoolean(rd["newRequest"]),
                                    projectManagerName = rd["projectManagerName"].ToString(),
                                    EmpName = rd["EmpName"].ToString(),
                                    remark = rd["remark"].ToString(),
                                    //absent = Convert.ToInt32(rd["TotalAbsent"]),
                                    //present = Convert.ToInt32(rd["TotalPresent"]),
                                    projectManagerAppr = Convert.ToBoolean(rd["projectManagerAppr"])
                                });
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        public List<AllAttendance> GetAllAttendanceForPmByMonth(int year, int month, int projectManager, int? limit = null, int? page = null)
        {
            try
            {
                List<AllAttendance> list = new List<AllAttendance>();
                Dictionary<int, AllAttendance> attendanceDict = new Dictionary<int, AllAttendance>();
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageattendance_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@v_action", "showAllAttendance");
                    cmd.Parameters.AddWithValue("@v_projectmanager", projectManager);
                    cmd.Parameters.AddWithValue("@v_id", 0);
                    cmd.Parameters.AddWithValue("@v_year", year);
                    cmd.Parameters.AddWithValue("@v_month", month);
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);

                    string curcorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{curcorName}\";", con))
                    using (var rd = fetchCmd.ExecuteReader())
                    {

                        while (rd.Read())
                        {
                            int empId = Convert.ToInt32(rd["EmpId"]);
                            string empName = rd["EmpName"].ToString();
                            int present = Convert.ToInt32(rd["TotalPresent"]);
                            int absent = Convert.ToInt32(rd["TotalAbsent"]);

                            AttendanceManage att = new AttendanceManage
                            {
                                EmpId = empId,
                                EmpName = empName,
                                present = present,
                                absent = absent,
                                attendanceDate = rd["attendanceDate"] != DBNull.Value ? Convert.ToDateTime(rd["attendanceDate"]) : DateTime.MinValue,
                                attendanceStatus = rd["attendanceStatus"]?.ToString(),
                            };

                            if (!attendanceDict.ContainsKey(empId))
                            {
                                attendanceDict[empId] = new AllAttendance
                                {
                                    EmpId = empId,
                                    EmpName = empName,
                                    month = month,
                                    present = present,
                                    absent = absent,
                                    showAll = new List<AttendanceManage> { att }
                                };
                            }
                            else
                            {
                                attendanceDict[empId].showAll.Add(att);
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{curcorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                }

                return attendanceDict.Values.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();

            }
        }

        public bool AttendanceApproval(int id, bool status, string remark)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_ManageAttendance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "approval");
                cmd.Parameters.AddWithValue("@projectManagerappr", status);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@remark", status ? null : remark);
                con.Open();
                int res = cmd.ExecuteNonQuery();
                return res > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }
        public List<AttendanceManage> getAttendanceCount(int projectManager,string searchTerm = null)
        {
            try
            {
                con.Open();
                List<AttendanceManage> list = new List<AttendanceManage>();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageAttendance_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "Totalcountpa");
                    cmd.Parameters.AddWithValue("v_projectmanager", projectManager);
                    cmd.Parameters.AddWithValue("v_id", DBNull.Value);
                    cmd.Parameters.AddWithValue("v_year", DateTime.Now.Year);
                    cmd.Parameters.AddWithValue("v_month", DateTime.Now.Month - 1);
                    cmd.Parameters.AddWithValue("v_limit", DBNull.Value);
                    cmd.Parameters.AddWithValue("v_page", DBNull.Value);
                    cmd.Parameters.AddWithValue("v_date", DBNull.Value);
                    cmd.Parameters.AddWithValue("v_searchterm", string.IsNullOrEmpty(searchTerm) ? DBNull.Value : (object)searchTerm);

                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var res = fetchCmd.ExecuteReader())
                    {
                        if (res.HasRows)
                        {
                            while (res.Read())
                            {
                                list.Add(new AttendanceManage
                                {
                                    EmpId = Convert.ToInt32(res["EmpId"]),
                                    EmpName = res["Emp_Name"].ToString(),
                                    present = Convert.ToInt32(res["presentcount"]),
                                    absent = Convert.ToInt32(res["absentcount"])
                                });
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        public List<AttendanceManage> getAttendanceCountByMonth(int year, int month, int projectManager, int? limit = null, int? page = null)
        {
            try
            {
                con.Open();
                List<AttendanceManage> list = new List<AttendanceManage>();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageattendance_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@v_action", "Totalcountpa");
                    cmd.Parameters.AddWithValue("@v_projectmanager", projectManager);
                    cmd.Parameters.AddWithValue("@v_id", 0);
                    cmd.Parameters.AddWithValue("@v_year", year);
                    cmd.Parameters.AddWithValue("@v_month", month);
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_date", NpgsqlTypes.NpgsqlDbType.Timestamp, DBNull.Value);

                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var res = fetchCmd.ExecuteReader())
                    {
                        if (res.HasRows)
                        {
                            while (res.Read())
                            {
                                list.Add(new AttendanceManage
                                {
                                    EmpId = Convert.ToInt32(res["EmpId"]),
                                    EmpName = res["Emp_Name"].ToString(),
                                    present = Convert.ToInt32(res["presentcount"]),
                                    absent = Convert.ToInt32(res["absentcount"])
                                });
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        public List<AttendanceManage> getReportAttendance(int month, int year, int projectManager, int EmpId)
        {
            try
            {
                List<AttendanceManage> list = new List<AttendanceManage>();
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageattendance_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "getRepo");
                    cmd.Parameters.AddWithValue("v_month", month);
                    cmd.Parameters.AddWithValue("v_year", year);
                    cmd.Parameters.AddWithValue("v_projectmanager", projectManager);
                    cmd.Parameters.AddWithValue("v_id", EmpId);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var rd = fetchCmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                list.Add(new AttendanceManage
                                {
                                    id = Convert.ToInt32(rd["id"]),
                                    EmpId = Convert.ToInt32(rd["EmpId"]),
                                    projectManager = Convert.ToInt32(rd["projectManager"]),
                                    createdAt = Convert.ToDateTime(rd["createdAt"]),
                                    attendanceDate = Convert.ToDateTime(rd["attendancedate"]),
                                    attendanceStatus = rd["attendancestatus"].ToString(),
                                    newRequest = Convert.ToBoolean(rd["newRequest"]),
                                    projectManagerName = rd["projectManagerName"].ToString(),
                                    EmpName = rd["EmpName"].ToString(),
                                    absent = Convert.ToInt32(rd["TotalAbsent"]),
                                    present = Convert.ToInt32(rd["TotalPresent"]),
                                    remark = rd["remark"].ToString(),
                                    projectManagerAppr = Convert.ToBoolean(rd["projectManagerAppr"])
                                });
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand(cursorName, con, tran))
                    {
                        closeCmd.CommandText = $"close \"{cursorName}\";";
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        public bool toKnowToday(int EmpId, DateTime attendanceDate)
        {
            try
            {
                con.Open();
                // Check if already exists
                cmd = new NpgsqlCommand("SELECT COUNT(*) FROM ManageAttendance WHERE EmpId = @EmpId AND attendancedate = @Date", con);
                cmd.Parameters.AddWithValue("@EmpId", EmpId);
                cmd.Parameters.AddWithValue("@Date", attendanceDate);
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        public List<AttendanceManage> chardata(int EmpId)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_ManageAttendance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "chardata");
                cmd.Parameters.AddWithValue("@EmpId", EmpId);
                List<AttendanceManage> list = new List<AttendanceManage>();
                con.Open();
                var data = cmd.ExecuteReader();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        list.Add(new AttendanceManage
                        {
                            present = Convert.ToInt32(data["PresentCount"]),
                            absent = Convert.ToInt32(data["AbsentCount"]),
                            total = Convert.ToInt32(data["total"])
                        });
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        #endregion

        public byte[] ConvertExcelFile(int month, int year, int userObj, int EmpId)
        {
            var data = getReportAttendance(month, year, userObj, EmpId);

            if (data.Any())
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Attendance Report");
                    worksheet.Cell(1, 1).Value = $"Name : {data[0].EmpName}";
                    worksheet.Cell(1, 5).Value = $"Total Present : {data[0].present}";
                    worksheet.Cell(1, 9).Value = $"Total Absent : {data[0].absent}";
                    worksheet.Cell(1, 13).Value = $"Month : {data[0].attendanceDate.ToString("MMMM")}";

                    // Define ranges (each 4 columns wide)
                    var range = worksheet.Range("A1:D1");
                    var range1 = worksheet.Range("E1:H1");
                    var range2 = worksheet.Range("I1:L1");
                    var range3 = worksheet.Range("M1:P1");

                    // Merge all ranges
                    range.Merge();
                    range1.Merge();
                    range2.Merge();
                    range3.Merge();

                    // Center text horizontally and vertically
                    range.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    range.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    range1.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    range1.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    range2.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    range2.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    range3.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    range3.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);


                    int daysInMonth = DateTime.DaysInMonth(year, month);
                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        DateTime currentDate = new DateTime(year, month, day);
                        worksheet.Cell(2, day).Value = currentDate.ToString("dd");
                    }

                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        DateTime currentDate = new DateTime(year, month, day);
                        var status = data.FirstOrDefault(x => x.attendanceDate.Date == currentDate.Date)?.attendanceStatus ?? "-";
                        worksheet.Cell(3, day).Value = status;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return stream.ToArray();
                    }
                }
            }
            else
            {
                return null;
            }
        }
        public byte[] ConvertExcelFileOfAll(int month, int year, int userObj)
        {
            var allData = GetAllAttendanceForPmByMonth(year, month, userObj); // ye list<AllAttendance> return karta hai

            if (allData.Any())
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Attendance Report");
                    int currentRow = 1;

                    // ✅ First Row
                    // Left side: "(-) means not available"
                    worksheet.Range(currentRow, 1, currentRow, 5).Merge();
                    worksheet.Cell(currentRow, 1).Value = "(-) means not available";
                    worksheet.Range(currentRow, 1, currentRow, 5).Style.Font.Bold = true;
                    worksheet.Range(currentRow, 1, currentRow, 5).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Range(currentRow, 1, currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                    // Right side: "Publish Date & Time"
                    worksheet.Range(currentRow, 12, currentRow, 16).Merge();
                    worksheet.Cell(currentRow, 12).Value = "Publish Date & Time : " + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt");
                    worksheet.Range(currentRow, 12, currentRow, 16).Style.Font.Bold = true;
                    worksheet.Range(currentRow, 12, currentRow, 16).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Range(currentRow, 12, currentRow, 16).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                    currentRow++; // Move to next row

                    foreach (var emp in allData)
                    {
                        // Header Row
                        worksheet.Cell(currentRow, 1).Value = $"Name : {emp.EmpName}";
                        worksheet.Cell(currentRow, 5).Value = $"Total Present : {emp.present}";
                        worksheet.Cell(currentRow, 9).Value = $"Total Absent : {emp.absent}";
                        worksheet.Cell(currentRow, 13).Value = $"Month : {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}";

                        // Merge and Style
                        worksheet.Range(currentRow, 1, currentRow, 4).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Range(currentRow, 5, currentRow, 8).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Range(currentRow, 9, currentRow, 12).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Range(currentRow, 13, currentRow, 16).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        currentRow++;

                        int daysInMonth = DateTime.DaysInMonth(year, month);

                        // Date Row
                        for (int day = 1; day <= daysInMonth; day++)
                        {
                            worksheet.Cell(currentRow, day).Value = day.ToString("00");
                        }

                        currentRow++;

                        // Status Row
                        for (int day = 1; day <= daysInMonth; day++)
                        {
                            DateTime currentDate = new DateTime(year, month, day);
                            var status = emp.showAll.FirstOrDefault(x => x.attendanceDate.Date == currentDate.Date)?.attendanceStatus ?? "-";
                            worksheet.Cell(currentRow, day).Value = status;
                        }

                        currentRow += 2; // Space between employees
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return stream.ToArray();
                    }
                }
            }

            return null;
        }

        #region Employee Monthly Report
        public bool InsertEmpReport(EmpReportModel model,out string message)
        {
            message = string.Empty;
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(@"call sp_ManageEmpReport(@v_action,@v_pmid,Null,@v_ProjectId,@v_unit,@v_annualtarget,@v_targetuptoreviewmonth,@v_achievementduringreviewmonth,@v_cumulativeachievement,@v_benefitingdepartments,@v_remarks,Null);", con))
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@v_action", "insert");

                    // Add all model properties
                    cmd.Parameters.AddWithValue("@v_ProjectId", model.ProjectId);
                    cmd.Parameters.AddWithValue("@v_pmid", model.PmId);
                    cmd.Parameters.AddWithValue("@v_unit", model.Unit);
                    cmd.Parameters.AddWithValue("@v_annualtarget", model.AnnualTarget);
                    cmd.Parameters.AddWithValue("@v_targetuptoreviewmonth", model.TargetUptoReviewMonth);
                    cmd.Parameters.AddWithValue("@v_achievementduringreviewmonth", model.AchievementDuringReviewMonth);
                    cmd.Parameters.AddWithValue("@v_cumulativeachievement", model.CumulativeAchievement);
                    cmd.Parameters.AddWithValue("@v_benefitingdepartments", model.BenefitingDepartments);
                    cmd.Parameters.AddWithValue("@v_remarks", model.Remarks ?? string.Empty);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch(SqlException sqlex)
            {
                message = sqlex.Message;
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }
        public List<EmpReportModel> GetEmpReport(int userid, int? limit = null, int? page = null, int? id = null,int?month=null,int?year=null)
        {
            try
            {
                List<EmpReportModel> list = new List<EmpReportModel>();
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageempreport_cursor", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@v_action", "get");
                    cmd.Parameters.AddWithValue("@v_projectmanager", userid);
                    cmd.Parameters.AddWithValue("@v_id", id.HasValue ? (object)id.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_month", month.HasValue ? (object)month.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_year", year.HasValue ? (object)year.Value : DBNull.Value);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (NpgsqlDataReader res = fetchCmd.ExecuteReader())
                    {
                        if (res.HasRows)
                        {
                            while (res.Read())
                            {
                                list.Add(new EmpReportModel
                                {
                                    ProjectId = res["ProjectId"] != DBNull.Value ? Convert.ToInt32(res["ProjectId"]) : 0,
                                    ProjectName = res["title"] != DBNull.Value ? res["title"].ToString() : "",
                                    Unit = res["Unit"] != DBNull.Value ? res["Unit"].ToString() : "",
                                    AnnualTarget = res["AnnualTarget"] != DBNull.Value ? Convert.ToInt32(res["AnnualTarget"]) : 0,
                                    TargetUptoReviewMonth = res["TargetUptoReviewMonth"] != DBNull.Value ? Convert.ToInt32(res["TargetUptoReviewMonth"]) : 0,
                                    AchievementDuringReviewMonth = res["AchievementDuringReviewMonth"] != DBNull.Value ? Convert.ToInt32(res["AchievementDuringReviewMonth"]) : 0,
                                    CumulativeAchievement = res["CumulativeAchievement"] != DBNull.Value ? Convert.ToInt32(res["CumulativeAchievement"]) : 0,
                                    BenefitingDepartments = res["BenefitingDepartments"] != DBNull.Value ? res["BenefitingDepartments"].ToString() : "",
                                    Remarks = res["Remarks"] != DBNull.Value ? res["Remarks"].ToString() : "",
                                    CreatedAt = Convert.ToDateTime(res["CreatedAt"]).ToString("dd-mm-yyyy")
                                });
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\"", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        #endregion


        #region Manage Feedback
        public bool InsertFeedback(FeedbackModel model)
        {
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("CALL sp_manage_feedback(@p_title, @p_feedback_type, @p_description, @p_userid,null::boolean,@p_action);", con))
                {
                    cmd.CommandType = CommandType.Text;

                    // Add parameters from the model
                    cmd.Parameters.AddWithValue("@p_title", model.Title??"");
                    cmd.Parameters.AddWithValue("@p_feedback_type", model.FeedbackType);
                    cmd.Parameters.AddWithValue("@p_description", model.Description);
                    cmd.Parameters.AddWithValue("@p_userid", model.UserId);
                    cmd.Parameters.AddWithValue("@p_action", "insertfeedback");

                    con.Open();
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }
        public List<FeedbackModel> GetFeedbacks(int userid)
        {
            try
            {
                List<FeedbackModel> list = new List<FeedbackModel>();
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_getfeedbacks_cursor", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@v_action", "getfeedbacks");
                    cmd.Parameters.AddWithValue("@v_projectmanager", userid);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (NpgsqlDataReader res = fetchCmd.ExecuteReader())
                    {
                        if (res.HasRows)
                        {
                            while (res.Read())
                            {
                                list.Add(new FeedbackModel
                                {
                                    Title = res["title"] != DBNull.Value ? res["title"].ToString() : "",
                                    FeedbackType = res["feedback_type"] != DBNull.Value ? res["feedback_type"].ToString() : "",
                                    Description = res["description"] != DBNull.Value ? res["description"].ToString() : "",
                                });
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\"", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }
        #endregion
    }

}