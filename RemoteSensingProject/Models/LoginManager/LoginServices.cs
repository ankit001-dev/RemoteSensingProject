using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using static RemoteSensingProject.Models.LoginManager.main;
using System.Web.Security;

namespace RemoteSensingProject.Models.LoginManager
{
    public class LoginServices : DataFactory
    {  
        public Credentials Login(string username, string password)
        {
            try
            {
                Credentials cr = new Credentials();
                cmd = new SqlCommand("sp_manageLoginMaster", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "loginUser");
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    rd.Read();
                    cr.userId = Convert.ToInt32(rd["userid"] != DBNull.Value ? rd["userid"] : 0);
                        cr.username = rd["username"].ToString();
                        cr.password = rd["password"].ToString();
                    cr.role = rd["userRole"].ToString();        
                }

                return cr;

            }catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if(con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        public List<string> getUserRole(string username)
        {
            try
            {
                List<string> roles = new List<string>();
                cmd = new SqlCommand("sp_manageLoginMaster", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getUserRole");
                cmd.Parameters.AddWithValue("@username", username);
                if (con.State == ConnectionState.Closed)
                    con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        roles.Add(rd["userRole"].ToString());
                    }
                }
                return roles;
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
    }
}