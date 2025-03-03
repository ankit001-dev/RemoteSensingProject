using Antlr.Runtime.Tree;
using Microsoft.AspNetCore.Routing;
using RemoteSensingProject.Models.MailService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using static RemoteSensingProject.Models.Admin.main;
using static RemoteSensingProject.Models.SubOrdinate.main;

namespace RemoteSensingProject.Models.ProjectManager
{
    public class ManagerService : DataFactory
    {
        #region /* Dashboard Count */
        public DashboardCount DashboardCount(string userId)
        {
            DashboardCount obj = null;
            try
            {
                SqlCommand cmd = new SqlCommand("sp_ManageDashboard", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "ManagerDashboardCount");
                cmd.Parameters.AddWithValue("@projectManager", userId);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    obj = new DashboardCount();
                    obj.TotalAssignProject = sdr["TotalAssignProject"].ToString();
                    obj.TotaCompleteProject = sdr["TotaCompleteProject"].ToString();
                    obj.TotalDelayProject = sdr["TotalDelayproject"].ToString();
                    obj.TotalNotice = sdr["TotalNotice"].ToString();
                    obj.TotalOngoingProject = sdr["TotalOngoingProject"].ToString();
                    obj.TotalMeeting = sdr["totalMeetings"].ToString();
                }

                sdr.Close();

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
                SqlCommand cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetAllProjectByManager");
                cmd.Parameters.AddWithValue("@projectManager", userId);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
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
                    obj.overAllPercent = Convert.ToDecimal(sdr["overallPercentage"]);
                    obj.Percentage = (sdr["financialStatusPercentage"] != DBNull.Value ? Convert.ToDecimal(sdr["financialStatusPercentage"]) : (decimal)0.00);
                    _list.Add(obj);
                }
                sdr.Close();
            } catch (Exception ex)
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
                SqlCommand cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getManagerDetails");
                cmd.Parameters.AddWithValue("@username", managerName);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    _details = new UserCredential();
                    _details.username = sdr["username"].ToString();
                    _details.userId = sdr["userid"].ToString();
                    _details.userRole = sdr["userRole"].ToString();

                }
                sdr.Close();
            } catch (Exception ex)
            {
                throw new Exception("An error accured", ex);

            }
            finally
            {
                con.Close();
            }
            return _details;
        }
        #endregion

        #region /* Add Project */
        public bool addManagerProject(createProjectModel pm)
        {
            con.Open();
            SqlTransaction tran = con.BeginTransaction();
            try
            {
                cmd = new SqlCommand("sp_adminAddproject", con, tran);
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
                cmd.Parameters.AddWithValue("@ApproveStatus", 1);
                cmd.Parameters.Add("@project_Id", SqlDbType.Int);
                cmd.Parameters["@project_Id"].Direction = ParameterDirection.Output;
                int i = cmd.ExecuteNonQuery();
                int projectId = Convert.ToInt32(cmd.Parameters["@project_Id"].Value != DBNull.Value ? cmd.Parameters["@project_Id"].Value : 0);
                if (i > 0)
                {
                    if (pm.budgets != null && pm.budgets.Count > 0)
                    {
                        foreach (var item in pm.budgets)
                        {
                            cmd = new SqlCommand("sp_adminAddproject", con, tran);
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
                            cmd = new SqlCommand("sp_adminAddproject", con, tran);
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
                        cmd = new SqlCommand("sp_adminAddproject", con, tran);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@action", "insertExternalProject");
                        cmd.Parameters.AddWithValue("@project_Id", projectId);
                        cmd.Parameters.AddWithValue("@DepartmentName", pm.pm.ProjectDepartment);
                        cmd.Parameters.AddWithValue("@contactPerson", pm.pm.ContactPerson);
                        cmd.Parameters.AddWithValue("@address", pm.pm.Address);
                        i += cmd.ExecuteNonQuery();
                    }

                    if (pm.pm.SubOrdinate.Length > 0)
                    {
                        foreach (var item in pm.pm.SubOrdinate)
                        {
                            cmd = new SqlCommand("sp_adminAddproject", con, tran);
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
                cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getManagerProject");
                cmd.Parameters.AddWithValue("@projectManager", userId);
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
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
                            StartDateString = Convert.ToDateTime(rd["startDate"]).ToString("dd-MM-yyyy")

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

        public List<Project_model> All_Project_List(string userId)
        {
            try
            {
                List<Project_model> list = new List<Project_model>();
                cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getProjectManagerTotalProject");
                cmd.Parameters.AddWithValue("@projectManager", userId);
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
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
                            overallPercentage = Convert.ToDecimal(rd["overallPercentage"])
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
                cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getNotStartedProjectByManager");
                cmd.Parameters.AddWithValue("@projectManager", userId);
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
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
                            createdBy = rd["createdBy"].ToString()
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
                cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getCompleteprojectByManager");
                cmd.Parameters.AddWithValue("@projectManager", userId);
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
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
                            createdBy = rd["createdBy"].ToString()
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
                cmd = new SqlCommand("sp_manageNotice", con);
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
                SqlCommand cmd = new SqlCommand("sp_ManageSubordinateProjectProblem", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getAllProblemListByManager");
                cmd.Parameters.AddWithValue("@projectManager", projectManager);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        obj = new Raise_Problem();
                        obj.ProjectName = sdr["ProjectName"].ToString();
                        obj.Title = sdr["Title"].ToString();
                        obj.Description = sdr["Description"].ToString();
                        obj.Attchment_Url = sdr["Attachment"].ToString();
                        obj.CreatedDate = Convert.ToDateTime(sdr["CreatedDate"]).ToString("dd-MM-yyyy");
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
        public List<Raise_Problem> getSubOrdinateProblemforAdmin()
        {
            try
            {
                List<Raise_Problem> problemList = new List<Raise_Problem>();
                Raise_Problem obj = null;
                SqlCommand cmd = new SqlCommand("sp_ManageSubordinateProjectProblem", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getAllSubOrdinateProblem");
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        obj = new Raise_Problem();
                        obj.ProjectName = sdr["ProjectName"].ToString();
                        obj.Title = sdr["Title"].ToString();
                        obj.Description = sdr["Description"].ToString();
                        obj.Attchment_Url = sdr["Attachment"].ToString();
                        obj.CreatedDate = Convert.ToDateTime(sdr["CreatedDate"]).ToString("dd-MM-yyyy");
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
        #endregion

        #region update weekly update
        public bool updateWeeklyStatus(Project_WeeklyUpdate pwu)
        {
            try
            {
                cmd = new SqlCommand("sp_ManageProjectSubstaces", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "insertUpdate");
                cmd.Parameters.AddWithValue("@project_Id", pwu.ProjectId);
                cmd.Parameters.AddWithValue("@w_date", pwu.date);
                cmd.Parameters.AddWithValue("@comment", pwu.comments);
                cmd.Parameters.AddWithValue("@completion", pwu.completionPerc);
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

        public List<Project_WeeklyUpdate> WeeklyUpdateList(int projectId)
        {
            try
            {
                List<Project_WeeklyUpdate> list = new List<Project_WeeklyUpdate>();
                cmd = new SqlCommand("sp_ManageProjectSubstaces", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectAllByProject");
                cmd.Parameters.AddWithValue("@project_Id", projectId);
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                List<Project_Subordination> subList = new List<Project_Subordination>();
                Project_model pm = new Project_model();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        list.Add(new Project_WeeklyUpdate
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            date = Convert.ToDateTime(rd["w_date"]),
                            comments = rd["comment"].ToString(),
                            completionPerc = Convert.ToInt32(rd["completion"])
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


        #region Project Expences
        public bool insertExpences(ProjectExpenses exp)
        {
            try
            {
                cmd = new SqlCommand("sp_ManageProjectSubstaces", con);
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
            } catch (Exception ex)
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

        public List<ProjectExpenses> ExpencesList(int headId, int projectId)
        {
            try
            {
                List<ProjectExpenses> list = new List<ProjectExpenses>();
                cmd = new SqlCommand("sp_ManageProjectSubstaces", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectExpenses");
                cmd.Parameters.AddWithValue("@project_id", projectId);
                cmd.Parameters.AddWithValue("@id", headId);
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
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
                return list;
            } catch (Exception ex)
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
                cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetBudgetByProjectId");
                cmd.Parameters.AddWithValue("@id", Id);
                if (con.State == ConnectionState.Closed)
                    con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
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
                cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetProjectStageByProjectId");
                cmd.Parameters.AddWithValue("@id", Id);
                if (con.State == ConnectionState.Closed)
                    con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
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
                using (SqlCommand cmd = new SqlCommand("sp_ManageStageStatus", con))
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
                        using (SqlCommand updateCmd = new SqlCommand("sp_ManageStageStatus", con))
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
                SqlCommand cmd = new SqlCommand("sp_ManageStageStatus", con);
                cmd.Parameters.AddWithValue("@action", "viewDealyReason");
                cmd.Parameters.AddWithValue("@stageId", stageId);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
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
                cmd = new SqlCommand("sp_manageOutSource", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "createOutSource");
                cmd.Parameters.AddWithValue("@EmpId", os.EmpId);
                cmd.Parameters.AddWithValue("@outCode", userName);
                cmd.Parameters.AddWithValue("@emp_name", os.EmpName);
                cmd.Parameters.AddWithValue("@emp_mobile", os.mobileNo);
                cmd.Parameters.AddWithValue("@emp_email", os.email);
                cmd.Parameters.AddWithValue("@emp_gender", os.gender);
                cmd.Parameters.AddWithValue("@password", userpassword);
                con.Open();
                int j = cmd.ExecuteNonQuery();
                if (j > 0)
                {
                    mail _mail = new mail();
                    string subject = "Login Credential";
                    string message = $"<p>Your user id : <b>{userName}</b></p><br><p>Password : <b>{userpassword}</b></p>";
                    _mail.SendMail(os.EmpName, os.email, subject, message);
                }
                return j > 0;
            } catch (Exception ex)
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

        public List<OuterSource> selectAllOutSOurceList(int userId)
        {
            try
            {
                List<OuterSource> list = new List<OuterSource>();
                cmd = new SqlCommand("sp_manageOutSource", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectAll");
                cmd.Parameters.AddWithValue("@empId", userId);
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
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
                            gender = rd["emp_gender"].ToString()
                        });
                    }
                }
                return list;
            } catch (Exception ex)
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
                cmd = new SqlCommand("sp_manageMemberResponseForMeeting", con);
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
                SqlCommand cmd = new SqlCommand("sp_ManageMeeting", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectMeetingForProjectManager");
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();

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
        public List<GetConclusion> getConclusionForMeeting(int meetingId, int userId)
        {
            try
            {
                List<GetConclusion> _list = new List<GetConclusion>();

                SqlCommand cmd = new SqlCommand("sp_meetingConslusion", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectConclusionForProjectManager");
                cmd.Parameters.AddWithValue("@memberId", userId);
                cmd.Parameters.AddWithValue("@meeting", meetingId);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();

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

                    sdr.Close();
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

        public List<Employee_model> getMemberJoiningStatus(int meetingId) {
            try
            {

                cmd.Parameters.Clear();
                cmd = new SqlCommand("sp_manageMemberResponseForMeeting", con);
                cmd.Parameters.AddWithValue("@action", "selectMemberJoiningStatus");
                cmd.Parameters.AddWithValue("@meeting", meetingId);
                cmd.CommandType = CommandType.StoredProcedure;
                List<Employee_model> meetingc = new List<Employee_model>();

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
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
            SqlTransaction tran = con.BeginTransaction();
            try
            {
                cmd = new SqlCommand("sp_manageOutSourceTask", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "createTask");
                cmd.Parameters.AddWithValue("@empId", ost.empId);
                cmd.Parameters.AddWithValue("@title", ost.title);
                cmd.Parameters.AddWithValue("@description", ost.description);
                cmd.Parameters.Add("@taskId", SqlDbType.Int);
                cmd.Parameters["@taskId"].Direction = ParameterDirection.Output;
                int i = cmd.ExecuteNonQuery();
                int taskId = Convert.ToInt32(cmd.Parameters["@taskId"].Value != DBNull.Value ? cmd.Parameters["@taskId"].Value : 0);
                if (i > 0 && taskId > 0 && ost.outSourceId.Length > 0)
                {
                    foreach (var item in ost.outSourceId)
                    {
                        cmd.Dispose();
                        cmd = new SqlCommand("sp_manageOutSourceTask", con, tran);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@action", "assignTask");
                        cmd.Parameters.AddWithValue("@empId", item);
                        cmd.Parameters.AddWithValue("@id", taskId);
                        i += cmd.ExecuteNonQuery();
                    }
                }

                tran.Commit();
                return i > 0;
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

        public List<OutSourceTask> taskList(int empId)
        {
            try
            {
                List<OutSourceTask> list = new List<OutSourceTask>();
                cmd = new SqlCommand("sp_manageOutSourceTask", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectAllTask");
                cmd.Parameters.AddWithValue("@empId", empId);
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
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
                cmd = new SqlCommand("sp_manageOutSourceTask", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "ViewTaskEmpStatus");
                cmd.Parameters.AddWithValue("@id", taskId);
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
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
                return list;
            } catch (Exception ex)
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
                cmd = new SqlCommand("sp_manageOutSourceTask", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "updateTaskStatus");
                cmd.Parameters.AddWithValue("@id", taskId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }catch(Exception ex)
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
                cmd = new SqlCommand("sp_Reimbursement", con);
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

        public bool submitReinbursementForm(string type, int userId)
        {
            try
            {
                cmd = new SqlCommand("sp_Reimbursement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "submitReinbursementForm");
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@type", type);
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
                cmd = new SqlCommand("sp_Reimbursement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectAll");
                con.Open();
                List<Reimbursement> getlist = new List<Reimbursement>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while(res.Read())
                    {
                        getlist.Add(new Reimbursement
                        {
                            EmpName = res["name"].ToString() + $"({res["employeeCode"].ToString()})",
                            type = (string)res["type"],
                            amount = Convert.ToDecimal(res["amount"]),
                            userId = Convert.ToInt32(res["userId"])
                        });
                    }
                }
                return getlist;
            }
            catch(Exception ex)
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

        public List<Reimbursement> GetSpecificUserReimbursements(int id, string type)
        {
            try
            {
                cmd = new SqlCommand("sp_Reimbursement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetSpecificTypeData");
                cmd.Parameters.AddWithValue("@userId", id);
                cmd.Parameters.AddWithValue("@type", type);
                con.Open();
                List<Reimbursement> getlist = new List<Reimbursement>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while(res.Read())
                    {
                        getlist.Add(new Reimbursement
                        {
                            id = (int)res["id"],
                            type = (string)res["type"],
                            vrNo = (string)res["vrNo"],
                            date = Convert.ToDateTime(res["date"]),
                            particulars = (string)res["particulars"],
                            items = (string)res["items"],
                            amount = Convert.ToDecimal(res["amount"]),
                            purpose = (string)res["purpose"],
                            status = Convert.ToBoolean(res["status"]),
                            newRequest = (bool)res["newRequest"],
                            adminappr = (bool)res["admin_appr"]
                        });
                    }
                }
                return getlist;
            }
            catch(Exception ex)
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
                cmd = new SqlCommand("sp_Reimbursement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getSpecificUserData");
                cmd.Parameters.AddWithValue("@userId", userId);
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        list.Add(new Reimbursement
                        {
                            type = rd["type"].ToString(),
                            amount = Convert.ToDecimal(rd["amount"]),
                            subStatus = Convert.ToBoolean(rd["SubmitStatus"])
                        });
                    }
                }
                return list;
            }catch(Exception ex)
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
                cmd = new SqlCommand("sp_Tourproposal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userId", data.userId);
                cmd.Parameters.AddWithValue("@projectId", data.projectId);
                cmd.Parameters.AddWithValue("@dateOfDept", data.dateOfDept);
                cmd.Parameters.AddWithValue("@place", data.place);
                cmd.Parameters.AddWithValue("@periodFrom", data.periodFrom);
                cmd.Parameters.AddWithValue("@periodTo", data.periodTo);
                cmd.Parameters.AddWithValue("@returnDate", data.returnDate);
                cmd.Parameters.AddWithValue("@purpose", data.purpose);
                cmd.Parameters.AddWithValue("@action","insert");
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                if(con.State == ConnectionState.Open)
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
                cmd = new SqlCommand("sp_Tourproposal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectAll");
                cmd.Parameters.AddWithValue("@userId", userId);
                con.Open();
                List<tourProposal> getlist = new List<tourProposal>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while(res.Read())
                    {
                        getlist.Add(new tourProposal
                        {
                            id = (int)res["id"],
                            projectName = (string)res["title"],
                            dateOfDept = Convert.ToDateTime(res["dateOfDept"]),
                            place = (string)res["place"],
                            periodFrom = Convert.ToDateTime(res["periodFrom"]),
                            periodTo = Convert.ToDateTime(res["periodTo"]),
                            returnDate = Convert.ToDateTime(res["returnDate"]),
                            purpose = (string)res["purpose"],
                        });
                    }
                }
                return getlist;
            }
            catch(Exception ex)
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
                cmd = new SqlCommand("sp_HiringVehicle", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectProject");
                cmd.Parameters.AddWithValue("@userId", userId);
                con.Open();
                List<HiringVehicle> projectList = new List<HiringVehicle>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while(res.Read())
                    {
                        projectList.Add(new HiringVehicle
                        {
                            projectId = (int)res["id"],
                            projectName = (string)res["title"]
                        });
                    }
                }
                return projectList;
            }
            catch(Exception ex)
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
                cmd = new SqlCommand("sp_HiringVehicle", con);
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
                cmd.Parameters.AddWithValue("@taxiReportTo", data.taxiReportTo);
                cmd.Parameters.AddWithValue("@taxiReportAt", data.taxiReportAt);
                cmd.Parameters.AddWithValue("@taxiReportPlace", data.taxiReportPlace);
                cmd.Parameters.AddWithValue("@taxiReportOn", data.taxiReportOn);
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
                cmd = new SqlCommand("sp_HiringVehicle", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectAll");
                cmd.Parameters.AddWithValue("@userId", userId);
                con.Open();
                List<HiringVehicle> hiringList = new List<HiringVehicle>();
                var res = cmd.ExecuteReader();
                if(res.HasRows)
                {
                    while(res.Read())
                    {
                        hiringList.Add(new HiringVehicle
                        {
                            id = (int)res["id"],
                            projectName = Convert.ToString(res["title"]),
                            headName = Convert.ToString(res["heads"]),
                            amount = Convert.ToDecimal(res["amount"]),
                            dateFrom = Convert.ToDateTime(res["dateFrom"]),
                            dateTo = Convert.ToDateTime(res["dateTo"]),
                            proposedPlace = (string)res["proposedPlace"],
                            purposeOfVisit = (string)res["purposeOfVisit"],
                            totalDaysNight = (string)res["totalDaysNight"],
                            totalPlainHills = (string)res["totalPlainHills"],
                            taxi = (string)res["taxi"],
                            BookAgainstCentre = (string)res["BookAgainstCentre"],
                            availbilityOfFund = (string)res["availbilityOfFund"],
                            taxiReportTo = (string)res["taxiReportTo"],
                            taxiReportAt = (TimeSpan)res["taxiReportAt"],
                            taxiReportPlace = (string)res["taxiReportPlace"],
                            taxiReportOn = Convert.ToDateTime(res["taxiReportOn"])
                        });
                    }
                }
                return hiringList;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if(con.State==ConnectionState.Open)
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
                cmd = new SqlCommand("sp_HiringVehicle", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectOne");
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                List<HiringVehicle> hiringList = new List<HiringVehicle>();
                var res = cmd.ExecuteReader();
                if(res.HasRows)
                {
                    while(res.Read())
                    {
                        hiringList.Add(new HiringVehicle
                        {
                            id = (int)res["id"],
                            projectName = Convert.ToString(res["title"]),
                            headName = Convert.ToString(res["heads"]),
                            amount = Convert.ToDecimal(res["amount"]),
                            dateFrom = Convert.ToDateTime(res["dateFrom"]),
                            dateTo = Convert.ToDateTime(res["dateTo"]),
                            proposedPlace = (string)res["proposedPlace"],
                            purposeOfVisit = (string)res["purposeOfVisit"],
                            totalDaysNight = (string)res["totalDaysNight"],
                            totalPlainHills = (string)res["totalPlainHills"],
                            taxi = (string)res["taxi"],
                            BookAgainstCentre = (string)res["BookAgainstCentre"],
                            availbilityOfFund = (string)res["availbilityOfFund"],
                            taxiReportTo = (string)res["taxiReportTo"],
                            taxiReportAt = (TimeSpan)res["taxiReportAt"],
                            taxiReportPlace = (string)res["taxiReportPlace"],
                            taxiReportOn = Convert.ToDateTime(res["taxiReportOn"])
                        });
                    }
                }
                return hiringList;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if(con.State==ConnectionState.Open)
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
                cmd = new SqlCommand("sp_HiringVehicle", con);
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
    }
}