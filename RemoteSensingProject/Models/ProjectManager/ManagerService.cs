using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using static RemoteSensingProject.Models.Admin.main;

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
                    obj.CompletionDatestring = Convert.ToDateTime(sdr["CompletionDate"]).ToString("dd-MM-yyyy");
                    obj.Status = sdr["status"].ToString();
                    obj.CompleteionStatus = Convert.ToInt32(sdr["CompleteStatus"]);
                    obj.ApproveStatus = Convert.ToInt32(sdr["ApproveStatus"]);
                    _list.Add(obj);
                }
                sdr.Close();
            }catch(Exception ex)
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
            }catch(Exception ex)
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
                cmd.Parameters.AddWithValue("@action", "GetAllProjectByManager");
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
            catch(Exception ex)
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
            catch(Exception ex)
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
                cmd.Parameters.AddWithValue("@w_date",exp.date);
                cmd.Parameters.AddWithValue("@amount", exp.amount);
                cmd.Parameters.AddWithValue("@attatchment", exp.attatchment_url);
                cmd.Parameters.AddWithValue("@comment", exp.description);
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

        public List<ProjectExpenses> ExpencesList(int projectId, int headId)
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
                            title = rd["title"].ToString(),
                            date = Convert.ToDateTime(rd["insertDate"]),
                            amount = Convert.ToDecimal(rd["amount"]),
                            attatchment_url = rd["attatchment"].ToString(),
                            description = rd["description"].ToString()
                        });
                    }
                }
                return list;
            }catch(Exception ex)
                            pm.Address = rd["address"].ToString();
                            pm.ProjectDepartment = rd["DepartmentName"].ToString();
                            pm.ContactPerson = rd["contactPerson"].ToString();
                        }
                        if (rd["SubordinateLinkId"] != DBNull.Value)
                        {
                            subList.Add(new Project_Subordination
                            {
                                Id = Convert.ToInt32(rd["SubordinateLinkId"]),
                                Name = rd["subName"].ToString(),
                                EmpCode = rd["subCode"].ToString()
                            });
                        }
                    }

                    rd.Close();
                }
                cpm.pm = pm;
                cpm.SubOrdinate = subList;
                cpm.budgets = ProjectBudgetList(id);
                cpm.stages = ProjectStagesList(id);
                return cpm;
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
                            ProjectAmount = Convert.ToDecimal(rd["headsAmount"] != DBNull.Value ? rd["headsAmount"] : 0),
                            Miscellaneous = rd["miscellaneous"].ToString(),
                            Miscell_amt = Convert.ToDecimal(rd["miscAmount"] != DBNull.Value ? rd["miscAmount"] : 0.00)
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
                            Status = rd["status"].ToString(),
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
        public bool insertStageStatus(Project_Statge obj)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("sp_ManageStageStatus", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "insertStageStatus");
                cmd.Parameters.AddWithValue("@stageId", obj.Project_Id);
                cmd.Parameters.AddWithValue("@Comment", obj.Comment);
                cmd.Parameters.AddWithValue("@CompletionPrecentage", obj.CompletionPrecentage);
                cmd.Parameters.AddWithValue("@StageDocument", obj.StageDocument_Url);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                if (i > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }catch(Exception ex)
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
    }
}