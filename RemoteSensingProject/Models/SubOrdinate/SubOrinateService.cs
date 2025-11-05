using System;
using System.Collections.Generic;
using static RemoteSensingProject.Models.SubOrdinate.main;
using Npgsql;
using System.Data;

namespace RemoteSensingProject.Models.SubOrdinate
{
    public class SubOrinateService : DataFactory
    {
        public UserCredential getManagerDetails(string managerName)
        {
            UserCredential _details = new UserCredential();
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("sp_adminAddproject", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getManagerDetails");
                cmd.Parameters.AddWithValue("@username", managerName);
                con.Open();
                NpgsqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    _details = new UserCredential();
                    _details.username = sdr["username"].ToString();
                    _details.userId = sdr["userid"].ToString();
                    _details.userRole = sdr["userRole"].ToString();

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
            return _details;
        }
        public List<ProjectList> getProjectBySubOrdinate(string userId)
        {
            List<ProjectList> _list = new List<ProjectList>();
            ProjectList obj = null;
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("sp_adminAddproject", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetProjectBySubOrdinate");
                cmd.Parameters.AddWithValue("@subOrdinate", userId);
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
                    obj.CompleteionStatus = Convert.ToBoolean(sdr["CompleteStatus"]);
                    obj.ApproveStatus = Convert.ToInt32(sdr["ApproveStatus"]);
                    obj.CreatedBy = sdr["name"].ToString();
                    obj.projectType = sdr["projectType"].ToString();
                    obj.projectStatus = Convert.ToSingle(sdr["completionPercentage"]);
                    obj.projectCode = sdr["projectCode"] != DBNull.Value ? sdr["projectCode"].ToString() : "N/A";
                    _list.Add(obj);
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

        #region Rasie Problem
        public bool InsertSubOrdinateProblem(Raise_Problem raise)
        {
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageSubordinateProjectProblem", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "insertProblem");
                cmd.Parameters.AddWithValue("@Project_Id", raise.Project_Id);
                cmd.Parameters.AddWithValue("@Title", raise.Title);
                cmd.Parameters.AddWithValue("@Description", raise.Description);
                cmd.Parameters.AddWithValue("@Attachment", raise.Attchment_Url);
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
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }

        }

        #endregion Problem End 

        #region Out Source Start
        public List<OutSource_Task> getOutSourceTask(int id)
        {
            try
            {
                List<OutSource_Task> taskList = new List<OutSource_Task>();
                OutSource_Task task = null;
                NpgsqlCommand cmd = new NpgsqlCommand("sp_manageOutSourceTask", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getTaskByOutSource");
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                NpgsqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        task = new OutSource_Task();
                        task.id = Convert.ToInt32(sdr["id"]);
                        task.Title = sdr["title"].ToString();
                        task.Description = sdr["description"].ToString();
                        task.CompleteStatus = Convert.ToInt32(sdr["completeStatus"]);
                        task.Status = sdr["Status"].ToString();
                        taskList.Add(task);
                    }
                }
                return taskList;
            }
            catch (Exception ex)
            {
                throw new Exception("An error accure", ex);
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        public bool AddOutSourceTask(OutSource_Task task)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("sp_manageOutSourceTask",con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "insertOutsource");
            cmd.Parameters.AddWithValue("@response", task.Reason);
            cmd.Parameters.AddWithValue("@id", task.id);
            cmd.Parameters.AddWithValue("@empId", task.EmpId);
            con.Open();
            int i=cmd.ExecuteNonQuery();
            if (i > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        #endregion Out Source End

        #region DashboardCount
        public DashboardCount GetDashboardCounts(int userId)
        {
            
            DashboardCount obj = null;
            try
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_managedashboard_cursor", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "managesubordinatedashboard");
                    cmd.Parameters.AddWithValue("v_projectmanager", 0);
                    cmd.Parameters.AddWithValue("v_sid", userId);
                    string cursorName = (string)cmd.ExecuteScalar();

                    // Now fetch the data from the cursor
                    using (var fetchCmd = new NpgsqlCommand($"FETCH ALL FROM \"{cursorName}\";", con, tran))
                    using (var sdr = fetchCmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                obj = new DashboardCount();
                                obj.TotalAssignProject = Convert.ToInt32(sdr["TotalProject"]);
                                obj.InternalProject = Convert.ToInt32(sdr["InternalProject"]);
                                obj.ExternalProject = Convert.ToInt32(sdr["ExternalProject"]);
                                obj.CompletedProject = Convert.ToInt32(sdr["CompletedProject"]);
                                obj.PendingProject = Convert.ToInt32(sdr["PendingProject"]);
                                obj.OngoingProject = Convert.ToInt32(sdr["OngoingProject"]);
                                obj.TotalMeetings = Convert.ToInt32(sdr["TotalMeetings"]);
                                obj.AdminMeetings = Convert.ToInt32(sdr["AdminMeetings"]);
                                obj.ProjectManagerMeetings = Convert.ToInt32(sdr["ProjectManagerMeetings"]);
                            }
                            sdr.Close();
                        }
                        return obj;
                    }
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
    }
}