using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static RemoteSensingProject.Models.Admin.main;
using System.Data.SqlClient;
using System.Data;
using System.Security.Policy;
using System.Runtime.InteropServices;
namespace RemoteSensingProject.Models.Admin
{
    public class AdminServices : DataFactory
    {
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
                cmd.Parameters.AddWithValue("@action" , cr.id > 0 ? "InsertDevision" : "UpdateDevision");
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

    }
}