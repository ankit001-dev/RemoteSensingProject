using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Math;
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
        public List<ProjectList> getAllProjectByManager(string userId)
        {
            List<ProjectList> _list = new List<ProjectList>();
            ProjectList obj = null;
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("sp_adminAddproject", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetAllProjectByManager");
                cmd.Parameters.AddWithValue("@projectManager", userId);
                con.Open();
                NpgsqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    obj = new ProjectList();
                    obj.Id = Convert.ToInt32(sdr["id"]);
                    obj.Title = sdr["title"].ToString();
                    obj.AssignDateString = Convert.ToDateTime(sdr["AssignDate"]).ToString("dd-MM-yyyy");
                    obj.StartDateString = Convert.ToDateTime(sdr["StartDate"]).ToString("dd-MM-yyyy");
                    obj.StartDate = Convert.ToDateTime(sdr["StartDate"]);
                    obj.CompletionDate = Convert.ToDateTime(sdr["CompletionDate"]);
                    obj.CompletionDatestring = Convert.ToDateTime(sdr["CompletionDate"]).ToString("dd-MM-yyyy");
                    obj.Status = sdr["status"].ToString();
                    obj.CompleteionStatus = Convert.ToInt32(sdr["CompleteStatus"]);
                    obj.ApproveStatus = Convert.ToInt32(sdr["ApproveStatus"]);
                    obj.budget = (float)Convert.ToDecimal(sdr["budget"]);
                    obj.stage = Convert.ToBoolean(sdr["stage"].ToString());
                    obj.physicalPercent = Convert.ToDecimal(sdr["completionPercentage"]);
                    obj.projectType = sdr["projectType"].ToString();
                    obj.overAllPercent = Convert.ToDecimal(sdr["overallPercentage"]);
                    obj.Percentage = (sdr["financialStatusPercentage"] != DBNull.Value ? Convert.ToDecimal(sdr["financialStatusPercentage"]) : (decimal)0.00);
                    _list.Add(obj);
                    obj.projectCode = sdr["projectCode"] != DBNull.Value ? sdr["projectCode"].ToString() : "N/A";
                }
                sdr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("An error accured", ex);
            }
            finally
            {
                con.Close();
            }
            return _list;
        }

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
            } catch (Exception ex)
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

        public List<Project_model> Project_List(string userId)
        {
            try
            {
                List<Project_model> list = new List<Project_model>();
                cmd = new NpgsqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getManagerProject");
                cmd.Parameters.AddWithValue("@projectManager", userId);
                con.Open();
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        list.Add(new Project_model
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            ProjectTitle = rd["title"].ToString(),
                            AssignDate = Convert.ToDateTime(rd["assignDate"]),
                            CompletionDate = Convert.ToDateTime(rd["completionDate"]),
                            StartDate = Convert.ToDateTime(rd["startDate"]),
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
                            projectCode = rd["projectCode"] != DBNull.Value ? rd["projectCode"].ToString() : "N/A"
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

        public List<Project_model> All_Project_List( int userId, int?limit, int?page)
        {
            try
            {
                con.Open();
                List<Project_model> list = new List<Project_model>();
                cmd = new NpgsqlCommand("SELECT * FROM fn_get_all_projects(@action,@v_id,@v_projectManager,@v_limit,@v_page)", con);
                cmd.Parameters.AddWithValue("@action", "GetAllProject");
                cmd.Parameters.AddWithValue("@v_projectManager", userId);
                cmd.Parameters.AddWithValue("@v_id", DBNull.Value);
                cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        list.Add(new Project_model
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            ProjectTitle = rd["title"].ToString(),
                            AssignDate = Convert.ToDateTime(rd["assignDate"]),
                            CompletionDate = Convert.ToDateTime(rd["completionDate"]),
                            StartDate = Convert.ToDateTime(rd["startDate"]),
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
                            projectCode = rd["projectCode"] != DBNull.Value ? rd["projectCode"].ToString() : "N/A"
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

        public List<Project_model> GetNotStartedProject_List(string userId)
        {
            try
            {
                List<Project_model> list = new List<Project_model>();
                cmd = new NpgsqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getNotStartedProjectByManager");
                cmd.Parameters.AddWithValue("@projectManager", userId);
                con.Open();
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        list.Add(new Project_model
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            ProjectTitle = rd["title"].ToString(),
                            AssignDate = Convert.ToDateTime(rd["assignDate"]),
                            CompletionDate = Convert.ToDateTime(rd["completionDate"]),
                            StartDate = Convert.ToDateTime(rd["startDate"]),
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
                            projectCode = rd["projectCode"] != DBNull.Value ? rd["projectCode"].ToString() : "N/A"
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
        public List<Project_model> GetCompleteProject_List(string userId)
        {
            try
            {
                List<Project_model> list = new List<Project_model>();
                cmd = new NpgsqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getCompleteprojectByManager");
                cmd.Parameters.AddWithValue("@projectManager", userId);
                con.Open();
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        list.Add(new Project_model
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            ProjectTitle = rd["title"].ToString(),
                            AssignDate = Convert.ToDateTime(rd["assignDate"]),
                            CompletionDate = Convert.ToDateTime(rd["completionDate"]),
                            StartDate = Convert.ToDateTime(rd["startDate"]),
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
                            projectCode = rd["projectCode"] != DBNull.Value ? rd["projectCode"].ToString() : "N/A"
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

        public List<Raise_Problem> getAllSubOrdinateProblem(string projectManager)
        {
            try
            {
                List<Raise_Problem> problemList = new List<Raise_Problem>();
                Raise_Problem obj = null;
                NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageSubordinateProjectProblem", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getAllProblemListByManager");
                cmd.Parameters.AddWithValue("@projectManager", projectManager);
                con.Open();
                NpgsqlDataReader sdr = cmd.ExecuteReader();
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
                        obj.projectCode = sdr["projectCode"] != DBNull.Value ? sdr["projectCode"].ToString() : "N/A";
                        problemList.Add(obj);
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
        public List<Raise_Problem> getAllSubOrdinateProblemById(string projectManager, int id)
        {
            try
            {
                List<Raise_Problem> problemList = new List<Raise_Problem>();
                Raise_Problem obj = null;
                NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageSubordinateProjectProblem", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getAllProblemListByManagerById");
                cmd.Parameters.AddWithValue("@projectManager", projectManager);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                NpgsqlDataReader sdr = cmd.ExecuteReader();
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
        public List<Raise_Problem> getSubOrdinateProblemforAdmin()
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
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
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
                                obj.projectCode = sdr["projectCode"] != DBNull.Value ? sdr["projectCode"].ToString() : "N/A";
                                problemList.Add(obj);
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
                cmd = new NpgsqlCommand("sp_ManageProjectSubstaces", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "insertUpdate");
                cmd.Parameters.AddWithValue("@project_Id", pwu.ProjectId);
                cmd.Parameters.AddWithValue("@w_date", pwu.date);
                cmd.Parameters.AddWithValue("@comment", pwu.comments);
                cmd.Parameters.AddWithValue("@unit", pwu.unit);
                cmd.Parameters.AddWithValue("@annual", pwu.annual);
                cmd.Parameters.AddWithValue("@monthEnd", pwu.monthEnd);
                cmd.Parameters.AddWithValue("@reviewMonth", pwu.reviewMonth);
                cmd.Parameters.AddWithValue("@MonthEndSequentially", pwu.MonthEndSequentially);
                cmd.Parameters.AddWithValue("@StateBeneficiaries", pwu.StateBeneficiaries);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
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
                using (var cmd = new NpgsqlCommand("fn_manageProjectSubstances_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "selectAllByProject");
                    cmd.Parameters.AddWithValue("v_project_Id", projectId);
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
                cmd = new NpgsqlCommand("sp_ManageProjectSubstaces", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "insertfinanceUpdate");
                cmd.Parameters.AddWithValue("@project_Id", fr.projectId);
                cmd.Parameters.AddWithValue("@aim", fr.aim);
                cmd.Parameters.AddWithValue("@w_date", fr.date);
                cmd.Parameters.AddWithValue("@month_aim", fr.month_aim);
                cmd.Parameters.AddWithValue("@completeInMonth", fr.completeInMonth);
                cmd.Parameters.AddWithValue("@departBeneficiaries", fr.departBeneficiaries);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
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
                    cmd.Parameters.AddWithValue("v_project_Id", projectId);
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
                cmd = new NpgsqlCommand("sp_ManageProjectSubstaces", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "insertExpences");
                cmd.Parameters.AddWithValue("@project_id", exp.projectId);
                cmd.Parameters.AddWithValue("@id", exp.projectHeadId);
                cmd.Parameters.AddWithValue("@title", exp.title);
                cmd.Parameters.AddWithValue("@w_date", exp.date);
                cmd.Parameters.AddWithValue("@amount", exp.amount);
                cmd.Parameters.AddWithValue("@attatchment", exp.attatchment_url);
                cmd.Parameters.AddWithValue("@comment", exp.description);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
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

        public List<ProjectExpenses> ExpencesList(int headId, int projectId,int? limit = null,int? page = null)
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
                    using(var closeCmd =  new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
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
        public List<Project_Budget> ProjectBudgetList(int Id)
        {
            try
            {
                List<Project_Budget> list = new List<Project_Budget>();
                cmd = new NpgsqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetBudgetByProjectId");
                cmd.Parameters.AddWithValue("@id", Id);
                if (con.State == ConnectionState.Closed)
                    con.Open();
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        list.Add(new Project_Budget
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            Project_Id = Convert.ToInt32(rd["project_id"]),
                            ProjectHeads = rd["heads"].ToString(),
                            ProjectAmount = Convert.ToDecimal(rd["headsAmount"] != DBNull.Value ? rd["headsAmount"] : 0)
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

        #region Satges
        public List<Project_Statge> ProjectStagesList(int Id)
        {
            try
            {
                List<Project_Statge> list = new List<Project_Statge>();
                cmd = new NpgsqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetProjectStageByProjectId");
                cmd.Parameters.AddWithValue("@id", Id);
                if (con.State == ConnectionState.Closed)
                    con.Open();
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        list.Add(new Project_Statge
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            Project_Id = Convert.ToInt32(rd["project_id"]),
                            KeyPoint = rd["keyPoint"].ToString(),
                            CompletionDate = Convert.ToDateTime(rd["completeDate"]),
                            CompletionDatestring = Convert.ToDateTime(rd["completeDate"]).ToString("dd-MM-yyyy"),
                            Document_Url = rd["stageDocument"].ToString(),
                            Status = rd["StagesStatus"] != DBNull.Value ? rd["StagesStatus"].ToString() : "Pending",
                            completionStatus = Convert.ToInt32(rd["completionStatus"])
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
                    if (con.State == ConnectionState.Open)
                        con.Close();
                cmd.Dispose();
            }
        }
        public bool InsertStageStatus(Project_Statge obj)
        {
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageStageStatus", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "insertStageStatus");
                    cmd.Parameters.AddWithValue("@stageId", obj.Stage_Id);
                    cmd.Parameters.AddWithValue("@Comment", obj.Comment);
                    cmd.Parameters.AddWithValue("@CompletionPrecentage", obj.CompletionPrecentage);
                    cmd.Parameters.AddWithValue("@StageDocument", obj.StageDocument_Url ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@updateStatus", obj.Status);
                    con.Open();
                    int i = cmd.ExecuteNonQuery();

                    if (obj.Status == "completed")
                    {
                        using (NpgsqlCommand updateCmd = new NpgsqlCommand("sp_ManageStageStatus", con))
                        {
                            updateCmd.CommandType = CommandType.StoredProcedure;
                            updateCmd.Parameters.AddWithValue("@action", "updateStageCompetionStatus");
                            updateCmd.Parameters.AddWithValue("@completionStatus", 1);
                            updateCmd.Parameters.AddWithValue("@project_Id", obj.Project_Id);
                            updateCmd.Parameters.AddWithValue("@stageId", obj.Stage_Id);

                            updateCmd.ExecuteNonQuery();
                        }
                    }

                    if (i > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while inserting stage status.", ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        public List<Project_Statge> ViewStagesComments(string stageId)
        {
            try
            {
                List<Project_Statge> stageList = new List<Project_Statge>();
                Project_Statge stage = null;
                NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageStageStatus", con);
                cmd.Parameters.AddWithValue("@action", "viewDealyReason");
                cmd.Parameters.AddWithValue("@stageId", stageId);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                NpgsqlDataReader sdr = cmd.ExecuteReader();
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
                var userName = os.EmpName.Substring(0, 5) + "@" + os.mobileNo.ToString().PadLeft(4, '0').Substring(os.mobileNo.ToString().Length - 4);
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
                cmd.Parameters.Add("@p_outcode", NpgsqlTypes.NpgsqlDbType.Varchar).Value = userName;
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
                string message = $"<p>Your user id : <b>{userName}</b></p><br><p>Password : <b>{userpassword}</b></p>";
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

        public List<OuterSource> selectAllOutSOurceList(int userId,int? limit = null,int? page = null)
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
                                    joiningdate = rd["joiningdate"].ToString(),
                                    gender = rd["emp_gender"].ToString()
                                });
                            }
                        }
                    }
                    using(var closeCmd = new NpgsqlCommand($"close \"{cursorName}\"", con, tran))
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
                cmd = new NpgsqlCommand("sp_manageMemberResponseForMeeting", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getResponseFromMemberForMeeting");
                cmd.Parameters.AddWithValue("@appStatus", mr.ApprovedStatus);
                cmd.Parameters.AddWithValue("@reason", mr.reason);
                cmd.Parameters.AddWithValue("@meeting", mr.MeetingId);
                cmd.Parameters.AddWithValue("@employee", mr.MemberId);
                con.Open();
                int res = cmd.ExecuteNonQuery();
                return res > 0;
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

        public List<Meeting_Model> getAllmeeting(int id)
        {
            try
            {
                List<Meeting_Model> _list = new List<Meeting_Model>();
                Meeting_Model obj = null;
                NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageMeeting", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectMeetingForProjectManager");
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                NpgsqlDataReader sdr = cmd.ExecuteReader();

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
                    _list.Add(obj);
                    obj.createdBy = sdr["createdBy"].ToString();
                }

                sdr.Close();
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
        public List<GetConclusion> getConclusionForMeeting(int meetingId, int userId,int? limit = null,int? page = null)
        {
            try
            {
                List<GetConclusion> _list = new List<GetConclusion>();
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_managemeetingconclusion_cursor", con,tran))
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
                    using(var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
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

        public List<OutSourceTask> taskList(int empId,int? limit = null,int? page = null)
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
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var rd = fetchCmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                list.Add(new OutSourceTask
                                {
                                    Id = Convert.ToInt32(rd["id"]),
                                    title = rd["title"].ToString(),
                                    description = rd["description"].ToString(),
                                    completeStatus = Convert.ToBoolean(rd["completeStatus"])
                                });
                            }
                        }
                    }
                    using(var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
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
                    using(var closeCmd = new NpgsqlCommand($"close \"{cursorName}\"", con, tran))
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
                cmd = new NpgsqlCommand("sp_Reimbursement", con);
                cmd.CommandType = CommandType.StoredProcedure;
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
                return cmd.ExecuteNonQuery() > 0;
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

        public bool submitReinbursementForm(string type, int userId, int Id)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_Reimbursement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "submitReinbursementForm");
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@id", Id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
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
        public List<Reimbursement> GetReimbursements()
        {
            try
            {
                con.Open();
                List<Reimbursement> getlist = new List<Reimbursement>();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageReimbursement_cursor", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "selectAll");

                    string cursorName = (string)cmd.ExecuteScalar();

                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var res = fetchCmd.ExecuteReader())
                    {
                        if (res.HasRows)
                        {
                            while (res.Read())
                            {
                                getlist.Add(new Reimbursement
                                {
                                    EmpName = res["name"].ToString() + $"({res["employeeCode"].ToString()})",
                                    type = res["type"].ToString(),
                                    id = Convert.ToInt32(res["id"]),
                                    amount = Convert.ToDecimal(res["amount"]),
                                    userId = Convert.ToInt32(res["userId"]),
                                    chequeNum = res["chequeNum"].ToString(),
                                    chequeDate = res["chequeDate"] != DBNull.Value ? Convert.ToDateTime(res["chequeDate"]).ToString("dd/MM/yyyy") : ""
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

        public List<Reimbursement> GetSpecificUserReimbursements(int userid, string type, int id)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_Reimbursement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetSpecificTypeData");
                cmd.Parameters.AddWithValue("@userId", userid);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                List<Reimbursement> getlist = new List<Reimbursement>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        getlist.Add(new Reimbursement
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

        public List<Reimbursement> GetReinbursementDatabyType(int userId)
        {
            try
            {
                List<Reimbursement> list = new List<Reimbursement>();
                cmd = new NpgsqlCommand("sp_Reimbursement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getSpecificUserData");
                cmd.Parameters.AddWithValue("@userId", userId);
                con.Open();
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        list.Add(new Reimbursement
                        {
                            id = Convert.ToInt32(rd["id"]),
                            type = rd["type"].ToString(),
                            amount = Convert.ToDecimal(rd["amount"]),
                            subStatus = Convert.ToBoolean(rd["SaveStatus"]),
                            userId = Convert.ToInt32(rd["userId"]),
                            chequeNum = rd["chequeNum"].ToString(),
                            chequeDate = rd["chequeDate"] != DBNull.Value ? Convert.ToDateTime(rd["chequeDate"]).ToString("dd/MM/yyyy") : "",
                            newRequest = Convert.ToBoolean(rd["newStatus"]),
                            approveAmount = Convert.ToDecimal(rd["apprAmt"] != DBNull.Value ? rd["apprAmt"] : 0),
                            //adminappr = Convert.ToBoolean("admin_appr"),
                            //apprstatus = Convert.ToBoolean("Apprstatus"),
                            //submitstatus = Convert.ToBoolean("SubmitStatus")
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

        #region tourproposal

        public bool insertTour(tourProposal data)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_Tourproposal", con);
                cmd.CommandType = CommandType.StoredProcedure;
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
                return cmd.ExecuteNonQuery() > 0;
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

        public List<tourProposal> getTourList(int userId)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_Tourproposal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectAll");
                cmd.Parameters.AddWithValue("@userId", userId);
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
                            adminappr = Convert.ToBoolean(res["adminappr"]),
                            projectCode = res["projectCode"] != DBNull.Value ? res["projectCode"].ToString() : "N/A"
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
                cmd = new NpgsqlCommand("sp_HiringVehicle", con);
                cmd.CommandType = CommandType.StoredProcedure;
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
                cmd.Parameters.AddWithValue("@availbilityOfFund", data.availbilityOfFund);
                cmd.Parameters.AddWithValue("@note", data.note);
                cmd.Parameters.AddWithValue("@action", "insert");
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
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

        public List<HiringVehicle> GetHiringVehicles(int userId)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_HiringVehicle", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectAll");
                cmd.Parameters.AddWithValue("@userId", userId);
                con.Open();
                List<HiringVehicle> hiringList = new List<HiringVehicle>();
                var res = cmd.ExecuteReader();
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
                cmd = new NpgsqlCommand("sp_raiseProblem", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "insert");
                cmd.Parameters.AddWithValue("@title", rp.title);
                cmd.Parameters.AddWithValue("@projectId", rp.projectId);
                cmd.Parameters.AddWithValue("@description", rp.description);
                cmd.Parameters.AddWithValue("@document", rp.documentname);
                cmd.Parameters.AddWithValue("@userId", rp.id);
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
                    con.Close();
                cmd.Dispose();
            }
        }
        public List<RaiseProblem> getProblems(int userId)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_raiseProblem", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectProblems");
                cmd.Parameters.AddWithValue("@userId", userId);
                List<RaiseProblem> list = new List<RaiseProblem>();
                con.Open();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
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
        public bool deleteRaisedProblem(int id, int userId)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_raiseProblem", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "delete");
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@userId", userId);
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
                    // Check if already exists
                    NpgsqlCommand checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM ManageAttendance WHERE EmpId = @EmpId AND attendancedate = @Date", con);
                    checkCmd.Parameters.AddWithValue("@EmpId", model.EmpId);
                    checkCmd.Parameters.AddWithValue("@Date", item.Key);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        skippedDates.Add(item.Key); // Already exists
                        continue;
                    }

                    // Insert only if not exists
                    NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageAttendance", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "InsertAttendance");
                    cmd.Parameters.AddWithValue("@projectManager", model.projectManager);
                    cmd.Parameters.AddWithValue("@EmpId", model.EmpId);
                    cmd.Parameters.AddWithValue("@attendancedate", item.Key);
                    cmd.Parameters.AddWithValue("@attendancestatus", item.Value);
                    cmd.ExecuteNonQuery();
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
                    using(var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
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
        public List<AttendanceManage> GetAllAttendanceForProjectManager(int projectManager)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_ManageAttendance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@projectManager", projectManager);
                cmd.Parameters.AddWithValue("@action", "getallforprojectmanager");
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
        public List<AllAttendance> GetAllAttendanceForPmByMonth(int year, int month, int projectManager)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_ManageAttendance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@projectManager", projectManager);
                cmd.Parameters.AddWithValue("@action", "showAllAttendance");
                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@month", month);
                List<AllAttendance> list = new List<AllAttendance>();
                con.Open();
                var rd = cmd.ExecuteReader();

                Dictionary<int, AllAttendance> attendanceDict = new Dictionary<int, AllAttendance>();

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
        public List<AttendanceManage> getAttendanceCount(int projectManager)
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
                    cmd.Parameters.AddWithValue("v_id", DBNull.Value);
                    cmd.Parameters.AddWithValue("v_year", 0);
                    cmd.Parameters.AddWithValue("v_month", 0);
                    cmd.Parameters.AddWithValue("v_projectmanager", projectManager);
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
        public List<AttendanceManage> getAttendanceCountByMonth(int year, int month, int projectManager)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_ManageAttendance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@projectManager", projectManager);
                cmd.Parameters.AddWithValue("@action", "Totalcountpa");
                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@month", month);
                con.Open();
                List<AttendanceManage> list = new List<AttendanceManage>();
                var res = cmd.ExecuteReader();
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
                    using(var closeCmd = new NpgsqlCommand(cursorName, con, tran))
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
        public bool InsertEmpReport(EmpReportModel model)
        {
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageEmpReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@action", "insert");

                    // Add all model properties
                    cmd.Parameters.AddWithValue("@ProjectId", model.ProjectId);
                    cmd.Parameters.AddWithValue("@PmId", model.PmId);
                    cmd.Parameters.AddWithValue("@Unit", model.Unit);
                    cmd.Parameters.AddWithValue("@AnnualTarget", model.AnnualTarget);
                    cmd.Parameters.AddWithValue("@TargetUptoReviewMonth", model.TargetUptoReviewMonth);
                    cmd.Parameters.AddWithValue("@AchievementDuringReviewMonth", model.AchievementDuringReviewMonth);
                    cmd.Parameters.AddWithValue("@CumulativeAchievement", model.CumulativeAchievement);
                    cmd.Parameters.AddWithValue("@BenefitingDepartments", model.BenefitingDepartments);
                    cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? string.Empty);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
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
        public List<EmpReportModel> GetEmpReport(int userid)
        {
            try
            {
                List<EmpReportModel> list = new List<EmpReportModel>();

                using (NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageEmpReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "get");
                    cmd.Parameters.AddWithValue("@PmId", userid);
                    //cmd.Parameters.AddWithValue("@Id", id.HasValue ? (object)id.Value : DBNull.Value);

                    con.Open();
                    using (NpgsqlDataReader res = cmd.ExecuteReader())
                    {
                        if (res.HasRows)
                        {
                            while (res.Read())
                            {
                                list.Add(new EmpReportModel
                                {
                                    ProjectId = Convert.ToInt32(res["ProjectId"]),
                                    ProjectName = res["title"].ToString(),
                                    Unit = res["Unit"].ToString(),
                                    AnnualTarget = Convert.ToInt32(res["AnnualTarget"]),
                                    TargetUptoReviewMonth = Convert.ToInt32(res["TargetUptoReviewMonth"]),
                                    AchievementDuringReviewMonth = Convert.ToInt32(res["AchievementDuringReviewMonth"]),
                                    CumulativeAchievement = Convert.ToInt32(res["CumulativeAchievement"]),
                                    BenefitingDepartments = res["BenefitingDepartments"].ToString(),
                                    Remarks = res["Remarks"] != DBNull.Value ? res["Remarks"].ToString() : ""
                                });
                            }
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
            }
        }
        public EmpReportModel GetEmpReportDataById(int id)
        {
            try
            {
                EmpReportModel model = null;

                using (NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageEmpReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", "get");
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    using (NpgsqlDataReader res = cmd.ExecuteReader())
                    {
                        if (res.HasRows)
                        {
                            if (res.Read())
                            {
                                model = new EmpReportModel
                                {
                                    ProjectId = Convert.ToInt32(res["ProjectId"]),
                                    Unit = res["Unit"].ToString(),
                                    AnnualTarget = Convert.ToInt32(res["AnnualTarget"]),
                                    TargetUptoReviewMonth = Convert.ToInt32(res["TargetUptoReviewMonth"]),
                                    AchievementDuringReviewMonth = Convert.ToInt32(res["AchievementDuringReviewMonth"]),
                                    CumulativeAchievement = Convert.ToInt32(res["CumulativeAchievement"]),
                                    BenefitingDepartments = res["BenefitingDepartments"].ToString(),
                                    Remarks = res["Remarks"] != DBNull.Value ? res["Remarks"].ToString() : ""
                                };
                            }
                        }
                    }
                }

                return model;
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

        public bool DeleteTargetAchievement(int id)
        {
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageEmpReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Pass action and Id
                    cmd.Parameters.AddWithValue("@action", "delete");
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
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

        #endregion
    }

}