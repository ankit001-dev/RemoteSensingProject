using System;
using System.Data;
using static RemoteSensingProject.Models.LoginManager.main;
using Npgsql;

namespace RemoteSensingProject.Models.LoginManager
{
    public class LoginServices : DataFactory
    {  
        public Credentials Login(string username, string password)
        {
            try
            {
                Credentials cr = new Credentials();

                using (var cmd = new NpgsqlCommand("SELECT * FROM sp_manageloginmaster(@action, @userid, @username, @password)", con))
                {
                    cmd.Parameters.AddWithValue("@action", "loginUser");
                    cmd.Parameters.AddWithValue("@userid", 0);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    con.Open();

                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            rd.Read();
                            cr.userId = rd["userid"] != DBNull.Value ? Convert.ToInt32(rd["userid"]) : 0;
                            cr.username = rd["username"].ToString();
                            cr.password = rd["password"].ToString();
                            cr.Emp_Id = rd["emp_id"] != DBNull.Value ? Convert.ToInt32(rd["emp_id"]) : 0;
                            cr.Emp_Name = rd["name"] != DBNull.Value ? rd["name"].ToString() : "Admin";
                            cr.profilePath = rd["profile"] != DBNull.Value ? rd["profile"].ToString() : "/ProjectContent/Admin/Employee_Image/img.png";
                            cr.role = rd["userrole"].ToString();
                        }
                    }
                }

                return cr;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during login: " + ex.Message, ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

    }
}