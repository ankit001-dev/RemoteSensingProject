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


        public bool AddEmployees(Employee_model emp)
        {
            con.Open();
            SqlTransaction transaction = con.BeginTransaction();
            try
            {
                string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                Random rnd = new Random();
                var userName = emp.EmployeeName.Substring(0, 5) + "@" + emp.MobileNo.ToString().Substring(0, 5);
                string userpassword = "";
                for (int i = 0; i < 8; i++)
                {
                    userpassword += validChars[rnd.Next(validChars.Length)];
                }

                cmd = new SqlCommand("sp_AdminEmployees", con,transaction);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", emp.Id != 0 ? "UpdateEmployees" : "InsertEmployees");
                cmd.Parameters.AddWithValue("@employeeCode", emp.EmployeeCode);
                cmd.Parameters.AddWithValue("@name", emp.EmployeeName);
                cmd.Parameters.AddWithValue("@mobile", emp.MobileNo);
                cmd.Parameters.AddWithValue("@email", emp.Email);
                cmd.Parameters.AddWithValue("@gender", emp.Gender);
                cmd.Parameters.AddWithValue("@role", emp.EmployeeRole);
                cmd.Parameters.AddWithValue("@devision", emp.Division);
                cmd.Parameters.AddWithValue("@designation", emp.Designation);
                cmd.Parameters.AddWithValue("@profile", emp.Image_url);
                cmd.Parameters.AddWithValue("@username", userName);
                cmd.Parameters.AddWithValue("@password", userpassword);

                int res = cmd.ExecuteNonQuery();
                if (res > 0 && emp.Id==0)
                {
                        string subject = "Login Credential";
                        string message = $"<p>Your user id : <b>{userName}</b></p><br><p>Password : <b>{userpassword}</b></p>";
                        _mail.SendMail(emp.EmployeeName, emp.Email, subject, message);
                    transaction.Commit();
                    return true;
                   
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

        public bool RemoveEmployees(int id)
        {
            try
            {
                cmd = new SqlCommand("sp_AdminEmployees", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "DeleteEmployees");
                cmd.Parameters.AddWithValue("@id", id);

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
        {
            try
            {
                cmd = new SqlCommand("sp_ManageEmployeeCategory", con);
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
        #endregion




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

        #region Meeting 
        public List<Employee_model> BindEmployee()
        {
            List<Employee_model> empList = new List<Employee_model>();
            Employee_model empObj = null;
            try
            {
                SqlCommand cmd = new SqlCommand("sp_ManageMeeting", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "BindMeetingMember");
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.HasRows)
                {
                    if (sdr.Read())
                    {
                        empObj = new Employee_model();
                        empObj.Id = Convert.ToInt32(sdr["id"]);
                        empObj.EmployeeName = sdr["name"].ToString();
                    
                    }
                    empList.Add(empObj);
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
            return empList;
        }

        public bool insertMeeting(Meeting_Model obj)
        {
            SqlTransaction transaction = con.BeginTransaction();
          
            try
            {
                SqlCommand cmd = new SqlCommand("sp_ManageMeeting", con,transaction);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "insertMeeting");
                cmd.Parameters.AddWithValue("@MeetingType", obj.MeetingType);
                cmd.Parameters.AddWithValue("@meetingLink", obj.MeetingLink);
                cmd.Parameters.AddWithValue("@MeetingTitle", obj.MeetingTitle);
                cmd.Parameters.AddWithValue("@meetingTime", obj.MeetingTime);
                cmd.Parameters.AddWithValue("@meetingDocument", obj.Attachment);
                SqlParameter outputParam = new SqlParameter("@meetingId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                if (i > 0)
                {
                    cmd.Parameters.AddWithValue("@action", "addMeetingMember");
                    cmd.Parameters.AddWithValue("@employee",obj.EmployeeId);
                    cmd.Parameters.AddWithValue("@meeting",obj.MeetingId);
                    i = cmd.ExecuteNonQuery();

                    return true;
                }
                else
                {
                   
                    return false;
                }
            }catch(Exception ex)
            {
                transaction.Rollback();
                throw new Exception("An error accured", ex);
            }
            finally
            {
                con.Close();
            }
        }
        #endregion End
    }
}