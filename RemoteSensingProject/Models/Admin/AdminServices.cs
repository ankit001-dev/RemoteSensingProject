using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static RemoteSensingProject.Models.Admin.main;
using static RemoteSensingProject.Models.LoginManager.LoginServices;
using System.Data.SqlClient;
using System.Data;
using System.Security.Policy;
using System.Runtime.InteropServices;
using System.Web.UI.WebControls;
using System.Web.Razor.Generator;
using RemoteSensingProject.Models.MailService;
using System.Web.Services.Description;
namespace RemoteSensingProject.Models.Admin
{
    public class AdminServices : DataFactory
    {
        #region Employee Category
        mail _mail = new mail();

        public bool InsertDesgination(CommonResponse cr)
        {
            try
            {
                cmd = new SqlCommand("sp_ManageEmployeeCategory", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action" , cr.id > 0 ? "UpdateDesignation" : "InsertDesignation");
                cmd.Parameters.AddWithValue("@designationName", cr.name);
                cmd.Parameters.AddWithValue("@id", cr.id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }catch(Exception ex)
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

        public bool InsertDivison(CommonResponse cr)
        {
            try
            {
                cmd = new SqlCommand("sp_ManageEmployeeCategory", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action" , cr.id > 0 ? "UpdateDevision" : "InsertDevision");
                cmd.Parameters.AddWithValue("@devisionName", cr.name);
                cmd.Parameters.AddWithValue("@id", cr.id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }catch(Exception ex)
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

        public List<CommonResponse> ListDivison()
        {
            try
            {
                List<CommonResponse> list = new List<CommonResponse>();
                cmd = new SqlCommand("sp_ManageEmployeeCategory", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetAllDevision");
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        list.Add(new CommonResponse
                        {
                            id = Convert.ToInt32(rd["id"]),
                            name = rd["devisionName"].ToString()
                        });
                    }
                    rd.Close();
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
        public List<CommonResponse> ListDesgination()
        {
            try
            {
                List<CommonResponse> list = new List<CommonResponse>();
                cmd = new SqlCommand("sp_ManageEmployeeCategory", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetAllDesignation");
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        list.Add(new CommonResponse
                        {
                            id = Convert.ToInt32(rd["id"]),
                            name = rd["designationName"].ToString()
                        });
                    }
                    rd.Close();
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

        public bool removeDesgination(int Id)

        public bool AddUserIdPassword(int userId, string username, string password, string userRole)
        {
            try
            {
                cmd = new SqlCommand("sp_manageLoginMaster", con);
                cmd.Parameters.AddWithValue("@action", "InsertLoginData");
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@userName", username);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@userRole", userRole);
                con.Open();
                int res = cmd.ExecuteNonQuery();
               
                return res > 0 ? true : false;
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
        public bool AddEmployees(Employee_model emp)
        {
                con.Open();
            SqlTransaction transaction = con.BeginTransaction();
            try
            {
                cmd = new SqlCommand("sp_ManageEmployeeCategory", con);

                cmd = new SqlCommand("sp_ManageEmployeeCategory", con,transaction);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "deleteDesignation");
                cmd.Parameters.AddWithValue("@id", Id);
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
        public bool removeDivison(int Id)
        {
            try
            {
                cmd = new SqlCommand("sp_ManageEmployeeCategory", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "deleteDevision");
                cmd.Parameters.AddWithValue("@id", Id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;

            }
                cmd.Parameters.AddWithValue("@action", emp.Id!=0? "UpdateEmployees" : "InsertEmployees");
                cmd.Parameters.AddWithValue("@id", emp.Id);
                cmd.Parameters.AddWithValue("@employeeCode", emp.EmployeeCode);
                cmd.Parameters.AddWithValue("@name", emp.EmployeeName);
                cmd.Parameters.AddWithValue("@mobile", emp.MobileNo);
                cmd.Parameters.AddWithValue("@email", emp.Email);
                cmd.Parameters.AddWithValue("@gender", emp.Gender);
                cmd.Parameters.AddWithValue("@role", emp.EmployeeRole);
                cmd.Parameters.AddWithValue("@devision", emp.Division);
                cmd.Parameters.AddWithValue("@designation", emp.Designation);
                cmd.Parameters.AddWithValue("@profile", emp.Image_url);
                SqlParameter outputParam = new SqlParameter("@empId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);
                int res = cmd.ExecuteNonQuery();
                if (res > 0)
                {
                        string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                        Random rnd=new Random();
                        var empId = (int)outputParam.Value;
                        var userName = emp.EmployeeName.Substring(0, 5) + "@" + emp.MobileNo.Substring(0, 5);
                        string userpassword= "";
                        for (int i = 0; i < 8; i++)
                        {
                            userpassword+= validChars[rnd.Next(validChars.Length)];
                        }
                    var loginRes = AddUserIdPassword(empId, userName, userpassword, emp.EmployeeRole);
                    if (loginRes)
                    {
                        string subject = "Login Credential";
                        string message = $"<p>Your user id : <b>{userName}</b></p><br><p>Password : <b>{userpassword}</b></p>";
                        _mail.SendMail(emp.EmployeeName,emp.Email,subject,message);
                    }
                    transaction.Commit();

                    return loginRes;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
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
        public bool AddEmployees(Employee_model emp)


        public bool RemoveEmployees(int? id)
        {
            try
            {
                cmd = new SqlCommand("sp_ManageEmployeeCategory", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "InsertEmployees");
                cmd.Parameters.AddWithValue("@employeeCode", emp.EmployeeCode);
                cmd.Parameters.AddWithValue("@name", emp.EmployeeCode);
                cmd.Parameters.AddWithValue("@mobile", emp.EmployeeCode);
                cmd.Parameters.AddWithValue("@email", emp.EmployeeCode);
                cmd.Parameters.AddWithValue("@gender", emp.EmployeeCode);
                cmd.Parameters.AddWithValue("@role", emp.EmployeeCode);
                cmd.Parameters.AddWithValue("@devision", emp.EmployeeCode);
                cmd.Parameters.AddWithValue("@designation", emp.EmployeeCode);
                cmd.Parameters.AddWithValue("@profile", emp.EmployeeCode);

                con.Open();
                int res = cmd.ExecuteNonQuery();
                if (res > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
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

        #region Create PRoject
        #endregion
        #region /* Admin Dashboard Count */
        public DashboardCount DashboardCount()
        {
            DashboardCount obj = null;
            try
            {

                SqlCommand cmd = new SqlCommand("sp_ManageDashboard", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "AdminDashboardCount");
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.Read())
                {
                    obj = new DashboardCount();
                    obj.TotalEmployee = sdr["TotalEmployee"].ToString();
                    obj.TotalProject = sdr["TotalProject"].ToString();
                    obj.TotalDelayproject = sdr["TotalDelayproject"].ToString();
                    obj.TotalCompleteProject = sdr["TotalCompleteProject"].ToString();
                    obj.TotalOngoingProject = sdr["TotalOngoingProject"].ToString();
                    obj.TotalMeetings = sdr["TotalMeetings"].ToString();
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
        #endregion /* End */    


    }
}