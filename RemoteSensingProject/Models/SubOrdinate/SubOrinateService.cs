using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
                SqlCommand cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetProjectBySubOrdinate");
                cmd.Parameters.AddWithValue("@subOrdinate", userId);
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
                    obj.CreatedBy = sdr["name"].ToString();
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
                SqlCommand cmd = new SqlCommand("sp_ManageSubordinateProjectProblem", con);
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
                SqlCommand cmd = new SqlCommand("sp_manageOutSourceTask", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getTaskByOutSource");
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
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
            SqlCommand cmd = new SqlCommand("sp_manageOutSourceTask",con);
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
    }
}