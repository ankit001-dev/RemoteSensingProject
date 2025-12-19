using DocumentFormat.OpenXml.Math;
using Google.Protobuf.WellKnownTypes;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using static RemoteSensingProject.Models.SubOrdinate.main;

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
                NpgsqlCommand cmd = new NpgsqlCommand("call sp_managesubordinateprojectproblem(v_action=>@v_action,v_project_id=>@v_project_id,v_title=>@v_title,v_description=>@v_description,v_attachment=>@v_attachment)", con);
                //cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@v_action", "insertProblem");
                cmd.Parameters.AddWithValue("@v_project_id", raise.Project_Id);
                cmd.Parameters.AddWithValue("@v_project_id", raise.Project_Id);
                cmd.Parameters.AddWithValue("@v_title", raise.Title);
                cmd.Parameters.AddWithValue("@v_description", raise.Description);
                cmd.Parameters.AddWithValue("@v_attachment", raise.Attchment_Url??"");
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
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

        #region Outsource Start
        public List<OutSource_Task> getOutSourceTask(int id,int? limit = null ,int? page = null, string searchTerm = null)
        {
            try
            {
                List<OutSource_Task> taskList = new List<OutSource_Task>();
                OutSource_Task task = null;
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageoutsource_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@v_action", "getTaskByOutSource");
                    cmd.Parameters.AddWithValue("@v_id", id);
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("v_searchterm", string.IsNullOrEmpty(searchTerm) ? DBNull.Value : (object)searchTerm);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var sdr = fetchCmd.ExecuteReader())
                    {
                        bool firstRow = true;
                        while (sdr.Read())
                        {
                            task = new OutSource_Task();
                            task.id = Convert.ToInt32(sdr["id"]);
                            task.Title = sdr["title"].ToString(); 
                            task.Description = sdr["description"].ToString();
                            task.CompleteStatus = Convert.ToInt32(sdr["completeStatus"]);
                            task.Status = sdr["Status"].ToString();
                            taskList.Add(task);
                        if (firstRow)
                        {
                            task.Pagination = new ApiCommon.PaginationInfo
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
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
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
            NpgsqlCommand cmd = new NpgsqlCommand("CALL sp_manageoutsourcetask(@v_action, @v_id, @v_empid, NULL, NULL,@v_response , null::smallint, NULL, NULL, NULL)", con);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.AddWithValue("@v_action", "insertOutsource");
            cmd.Parameters.AddWithValue("@v_response", task.Reason);
            cmd.Parameters.AddWithValue("@v_id", task.id);
            cmd.Parameters.AddWithValue("@v_empId", task.EmpId);
            con.Open();
            cmd.ExecuteNonQuery();
            return true;
        }

        #endregion Outsource End

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