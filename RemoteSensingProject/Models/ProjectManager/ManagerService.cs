using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RemoteSensingProject.Models.ProjectManager
{
    public class ManagerService : DataFactory
    {
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
                    obj.AssignDate = Convert.ToDateTime(sdr["AssignDate"]);
                    obj.Title = sdr["title"].ToString();
                    obj.StartDate = Convert.ToDateTime(sdr["StartDate"]);
                    obj.CompletionDate = Convert.ToDateTime(sdr["CompletionDate"]);
                    obj.Status = sdr["status"].ToString();
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
        public UserCredential getManagerDetails(string userId)
        {
            UserCredential _details = new UserCredential();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getManagerDetails");
                cmd.Parameters.AddWithValue("@action", userId);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    _details = new UserCredential();
                    _details.username = sdr["username"].ToString();
                    _details.userId = Convert.ToInt32(sdr["userId"]);
                    _details.role = sdr["role"].ToString();

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
    }
}