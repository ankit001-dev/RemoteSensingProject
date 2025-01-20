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
        //public List<ProjectList> getAllSubOrdinateProblem()
        //{
        //    List<ProjectList> problemList = new List<ProjectList>();
        //    ProjectList obj = null;
        //    SqlCommand cmd = new SqlCommand();

        //    return 
        //}
     
        #endregion Problem End 
    }
}