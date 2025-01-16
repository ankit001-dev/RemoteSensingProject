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
using System.IO;
using Microsoft.Ajax.Utilities;
using System.Web.ModelBinding;
using Microsoft.AspNetCore.Routing.Internal;
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

        #region add Employee
        public bool AddEmployees(Employee_model emp)
        {
            con.Open();
            SqlTransaction transaction = con.BeginTransaction();
            try
            {
                string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                Random rnd = new Random();
                var userName = emp.EmployeeName.Substring(0, 5) + "@" + emp.MobileNo.ToString().PadLeft(4, '0').Substring(emp.MobileNo.ToString().Length - 4);
                string userpassword = "";
                if (emp.Id == 0)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        userpassword += validChars[rnd.Next(validChars.Length)];
                    }
                }

                var ActionType = emp.Id != 0 ? "UpdateEmployees" : "InsertEmployees";
                cmd = new SqlCommand("sp_AdminEmployees", con, transaction);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", ActionType);
                cmd.Parameters.AddWithValue("@employeeCode", emp.EmployeeCode);
                cmd.Parameters.AddWithValue("@id", emp.Id);
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
                if (res > 0 && emp.Id == 0)
                {
                    string subject = "Login Credential";
                    string message = $"<p>Your user id : <b>{userName}</b></p><br><p>Password : <b>{userpassword}</b></p>";
                    _mail.SendMail(emp.EmployeeName, emp.Email, subject, message);
                    transaction.Commit();
                    return true;

                }
                else if(res>0)
                {
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

        public List<Employee_model> SelectEmployeeRecord()
        {
            try { 
            cmd = new SqlCommand("sp_AdminEmployees", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@action", "SelectEmployees");
            con.Open();
            var record = cmd.ExecuteReader();
            List<Employee_model> empModel = new List<Employee_model>();
            while (record.Read())
            {
                empModel.Add(new Employee_model
                {
                    Id = (int)record["id"],
                    EmployeeCode = record["employeeCode"].ToString(),
                    EmployeeName = record["name"].ToString(),
                    DevisionName = record["devisionName"].ToString(),
                    Email = record["email"].ToString(),
                    MobileNo = Convert.ToInt64(record["mobile"]),
                    EmployeeRole = record["role"].ToString(),
                    Division = (int)record["devision"],
                    DesignationName = record["designationName"].ToString(),
                    Status = (bool)record["status"],
                    ActiveStatus = (bool)record["activeStatus"],
                    CreationDate = Convert.ToDateTime(record["creationDate"]).ToString("dd-MM-yyyy"),
                    Image_url = record["profile"]!=DBNull.Value ? record["profile"].ToString():null
                });
            }
            return empModel;
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




        public Employee_model SelectEmployeeRecordById(int id)
        {
            try
            {
                cmd = new SqlCommand("sp_AdminEmployees", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "SelectEmployeesById");
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                Employee_model empModel=new Employee_model();
                var record = cmd.ExecuteReader();
                while (record.Read())
                {
                    empModel = new Employee_model
                    {
                        Id = (int)record["id"],
                        EmployeeCode = record["employeeCode"].ToString(),
                        Email = record["email"].ToString(),
                        Gender = record["gender"].ToString(),
                        MobileNo = (long)record["mobile"],
                        EmployeeName = record["name"].ToString(),
                        DevisionName = record["devisionName"].ToString(),
                        Division = (int)record["devision"],
                        Designation = (int)record["designation"],
                        EmployeeRole = record["role"].ToString(),
                        DesignationName = record["designationName"].ToString(),
                        Status = (bool)record["status"],
                        ActiveStatus = (bool)record["activeStatus"],
                        CreationDate = Convert.ToDateTime(record["creationDate"]).ToString("dd-MM-yyyy"),
                        Image_url = record["profile"] != null ? record["profile"].ToString() : null
                    };
                }
                return empModel;
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

        public bool ChangeActieStatus(int id)
        {
            try
            {
                cmd = new SqlCommand("sp_AdminEmployees", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "ChangeActiveStatus");
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
        #endregion



        #region Create Project
        public bool addProject(createProjectModel pm)
        {
            con.Open();
            SqlTransaction tran = con.BeginTransaction();
            try
            {
                cmd = new SqlCommand("sp_adminAddproject", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "insertProject");
                cmd.Parameters.AddWithValue("@title", pm.pm.ProjectTitle);
                cmd.Parameters.AddWithValue("@assignDate", pm.pm.AssignDate);
                cmd.Parameters.AddWithValue("@startDate", pm.pm.StartDate);
                cmd.Parameters.AddWithValue("@completionDate", pm.pm.CompletionDate);
                cmd.Parameters.AddWithValue("@projectmanager", pm.pm.ProjectManager);
                cmd.Parameters.AddWithValue("@budget", pm.pm.ProjectBudget);
                cmd.Parameters.AddWithValue("@description", pm.pm.ProjectDescription);
                cmd.Parameters.AddWithValue("@ProjectDocument", pm.pm.projectDocumentUrl);
                cmd.Parameters.AddWithValue("@projectType", pm.pm.ProjectType);
                cmd.Parameters.AddWithValue("@stage", pm.pm.ProjectStage);
                cmd.Parameters.AddWithValue("@createdBy", "admin");
                cmd.Parameters.Add("@project_Id", SqlDbType.Int);
                cmd.Parameters["@project_Id"].Direction = ParameterDirection.Output;
                int i = cmd.ExecuteNonQuery();
                int projectId = Convert.ToInt32(cmd.Parameters["@project_Id"].Value != DBNull.Value ? cmd.Parameters["@project_Id"].Value : 0);
                if(i > 0)
                {
                    if(pm.budgets != null && pm.budgets.Count > 0)
                    {
                        foreach(var item in pm.budgets)
                        {
                            cmd = new SqlCommand("sp_adminAddproject", con, tran);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@action", "insertProjectBudget");
                            cmd.Parameters.AddWithValue("@project_Id", projectId);
                            cmd.Parameters.AddWithValue("@heads", item.ProjectHeads);
                            cmd.Parameters.AddWithValue("@headsAmount", item.ProjectAmount);
                            cmd.Parameters.AddWithValue("@miscellaneous", item.Miscellaneous);
                            cmd.Parameters.AddWithValue("@miscAmount", item.Miscell_amt);
                            i += cmd.ExecuteNonQuery();
                        }
                    }
                    if(pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage.Equals("Yes"))
                    {
                        foreach(var item in pm.stages)
                        {
                            cmd = new SqlCommand("sp_adminAddproject", con, tran);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@action", "insertProjectStatge");
                            cmd.Parameters.AddWithValue("@project_Id", projectId);
                            cmd.Parameters.AddWithValue("@keyPoint", item.KeyPoint);
                            cmd.Parameters.AddWithValue("@completeDate", item.CompletionDate);
                            cmd.Parameters.AddWithValue("@stageDocument", item.Document_Url);
                            i += cmd.ExecuteNonQuery();
                        }
                    }

                    if (pm.pm.ProjectType.Equals("External") && projectId > 0)
                    {
                        cmd = new SqlCommand("sp_adminAddproject", con, tran);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@action", "insertExternalProject");
                        cmd.Parameters.AddWithValue("@project_Id", projectId);
                        cmd.Parameters.AddWithValue("@DepartmentName", pm.pm.ProjectDepartment);
                        cmd.Parameters.AddWithValue("@contactPerson", pm.pm.ContactPerson);
                        cmd.Parameters.AddWithValue("@address", pm.pm.Address);
                        i += cmd.ExecuteNonQuery();
                    }

                    if (pm.pm.SubOrdinate.Length > 0)
                    {
                        foreach(var item in pm.pm.SubOrdinate)
                        {
                            cmd = new SqlCommand("sp_adminAddproject", con, tran);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@action", "insertSubOrdinate");
                            cmd.Parameters.AddWithValue("@project_Id", projectId);
                            cmd.Parameters.AddWithValue("@id", item);
                            cmd.Parameters.AddWithValue("@projectmanager", pm.pm.ProjectManager);
                            i += cmd.ExecuteNonQuery();
                        }
                    }
                }
                tran.Commit();
                return i > 0;
            }catch(Exception ex)
            {
                tran.Rollback();
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        public List<Project_model> Project_List()
        {
            try
            {
                List<Project_model> list = new List<Project_model>();
                cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetAllProject");
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        list.Add(new Project_model
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            ProjectTitle = rd["title"].ToString(),
                            AssignDate = Convert.ToDateTime(rd["assignDate"]),
                            CompletionDate = Convert.ToDateTime(rd["completionDate"]),
                            StartDate = Convert.ToDateTime(rd["startDate"]),
                            ProjectManager = rd["name"].ToString(),
                            ProjectBudget = Convert.ToDecimal(rd["budget"]),
                            ProjectDescription = rd["description"].ToString(),
                            projectDocumentUrl = rd["ProjectDocument"].ToString(),
                            ProjectType = rd["projectType"].ToString(),
                            ProjectStage = Convert.ToBoolean(rd["stage"]),
                            CompletionDatestring = Convert.ToDateTime(rd["completionDate"]).ToString("dd-MM-yyyy"),
                            ProjectStatus = Convert.ToBoolean(rd["CompleteStatus"])
                        });
                    }
                }
                return list;
            }catch(Exception ex)
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


        public createProjectModel GetProjectById(int id)
        {
            try
            {
                createProjectModel cpm = new createProjectModel();
                cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetProjectById");
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                List<Project_Statge> stagesList = new List<Project_Statge>();
                List<Project_Budget> budgetList = new List<Project_Budget>();
                List<Project_Subordination> subList = new List<Project_Subordination>();
                Project_model pm = new Project_model();
                if (rd.HasRows)
                {
                    
                    while (rd.Read())
                    {
                        pm.Id = Convert.ToInt32(rd["id"]);
                        pm.ProjectTitle = rd["title"].ToString();
                        pm.AssignDate = Convert.ToDateTime(rd["assignDate"]);
                        pm.CompletionDate = Convert.ToDateTime(rd["completionDate"]);
                        pm.StartDate = Convert.ToDateTime(rd["startDate"]);
                        pm.ProjectManager = rd["ManagerName"].ToString();
                        pm.ProjectBudget = Convert.ToDecimal(rd["budget"]);
                        pm.ProjectDescription = rd["description"].ToString();
                        pm.projectDocumentUrl = rd["ProjectDocument"].ToString();
                        pm.ProjectType = rd["projectType"].ToString();
                        pm.ProjectStage = Convert.ToBoolean(rd["stage"]);
                        if (pm.ProjectType.Equals("External"))
                        {
                            pm.Address = rd["address"].ToString();
                            pm.ProjectDepartment = rd["DepartmentName"].ToString();
                            pm.ContactPerson = rd["contactPerson"].ToString();
                        }
                        if (pm.ProjectStage)
                        {
                            if(rd["StageId"] != DBNull.Value)
                            {

                            stagesList.Add(new Project_Statge
                            {
                                Id = Convert.ToInt32(rd["StageId"]),
                                KeyPoint = rd["keyPoint"].ToString(),
                                CompletionDate = Convert.ToDateTime(rd["completeDate"]),
                                Document_Url = rd["stageDocument"].ToString()
                            });
                            }
                        }
                        if (rd["SubordinateLinkId"] != DBNull.Value)
                        {
                            subList.Add(new Project_Subordination
                            {
                                Id = Convert.ToInt32(rd["SubordinateLinkId"]),
                                Name = rd["subName"].ToString(),
                                EmpCode = rd["subCode"].ToString()
                            });
                        }
                        if (pm.ProjectBudget > 0)
                        {
                            if (rd["budgetId"] != DBNull.Value)
                            {
                                budgetList.Add(new Project_Budget
                                {
                                    Id = Convert.ToInt32(rd["BudgetId"]),
                                    ProjectHeads = rd["heads"].ToString(),
                                    ProjectAmount = Convert.ToDecimal(rd["headsAmount"]),
                                    Miscellaneous = rd["miscellaneous"].ToString(),
                                    Miscell_amt = Convert.ToDecimal(rd["miscAmount"])
                                });
                            }
                        }
                       
                    }
                }
                cpm.pm = pm;
                cpm.SubOrdinate = subList;
                cpm.budgets = budgetList;
                cpm.stages = stagesList;
                return cpm;
            }
                    catch(Exception ex)
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

        #region Api Create Project
        public bool createApiProject(Project_model pm)
        {
            con.Open();
            SqlTransaction tran = con.BeginTransaction();
            try
            {
                cmd = new SqlCommand("sp_adminAddproject", con, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", pm.Id > 0 ? "updateProject" : "insertProject");
                cmd.Parameters.AddWithValue("@title", pm.ProjectTitle);
                cmd.Parameters.AddWithValue("@assignDate", pm.AssignDate);
                cmd.Parameters.AddWithValue("@startDate", pm.StartDate);
                cmd.Parameters.AddWithValue("@completionDate", pm.CompletionDate);
                cmd.Parameters.AddWithValue("@projectmanager", pm.ProjectManager);
                cmd.Parameters.AddWithValue("@budget", pm.ProjectBudget);
                cmd.Parameters.AddWithValue("@description", pm.ProjectDescription);
                cmd.Parameters.AddWithValue("@ProjectDocument", pm.projectDocumentUrl);
                cmd.Parameters.AddWithValue("@projectType", pm.ProjectType);
                cmd.Parameters.AddWithValue("@stage", pm.ProjectStage);
                cmd.Parameters.AddWithValue("@createdBy", "admin");
                cmd.Parameters.Add("@project_Id", SqlDbType.Int);
                cmd.Parameters["@project_Id"].Direction = ParameterDirection.Output;
                int i = cmd.ExecuteNonQuery();
                int projectId = Convert.ToInt32(cmd.Parameters["@project_Id"].Value != DBNull.Value ? cmd.Parameters["@project_Id"].Value : 0);
                if (pm.ProjectType.Equals("External") && (projectId > 0 || pm.Id > 0))
                {
                    cmd = new SqlCommand("sp_adminAddproject", con, tran);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@action", pm.Id > 0 ? "updateExternalProject" : "insertExternalProject");
                    cmd.Parameters.AddWithValue("@project_Id", projectId);
                    cmd.Parameters.AddWithValue("@DepartmentName", pm.ProjectDepartment);
                    cmd.Parameters.AddWithValue("@contactPerson", pm.ContactPerson);
                    cmd.Parameters.AddWithValue("@address", pm.Address);
                    i += cmd.ExecuteNonQuery();
                }
                if (pm.SubOrdinate.Length > 0)
                {
                    foreach (var item in pm.SubOrdinate)
                    {
                        cmd = new SqlCommand("sp_adminAddproject", con, tran);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@action", "insertSubOrdinate");
                        cmd.Parameters.AddWithValue("@project_Id", projectId);
                        cmd.Parameters.AddWithValue("@id", item);
                        cmd.Parameters.AddWithValue("@projectmanager", pm.ProjectManager);
                        i += cmd.ExecuteNonQuery();
                    }
                }
                tran.Commit();
                return i > 0;
            }catch(Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        public bool insertProjectStages(Project_Statge stg) {
            try
            {
                cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", stg.Id > 0 ? "updateProjectStage" : "insertProjectStatge");
                cmd.Parameters.AddWithValue("@project_Id", stg.Project_Id);
                cmd.Parameters.AddWithValue("@keyPoint", stg.KeyPoint);
                cmd.Parameters.AddWithValue("@completeDate", stg.CompletionDate);
                cmd.Parameters.AddWithValue("@stageDocument", stg.Document_Url);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        public bool insertProjectBudgets(Project_Budget bdg)
        {
            try
            {
                cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", bdg.Id > 0 ? "updateProjectBudget" : "insertProjectBudget");
                cmd.Parameters.AddWithValue("@project_Id", bdg.Project_Id);
                cmd.Parameters.AddWithValue("@heads", bdg.ProjectHeads);
                cmd.Parameters.AddWithValue("@headsAmount", bdg.ProjectAmount);
                cmd.Parameters.AddWithValue("@miscellaneous", bdg.Miscellaneous);
                cmd.Parameters.AddWithValue("@miscAmount", bdg.Miscell_amt);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        public List<Project_Budget> ProjectBudgetList(int Id)
        {
            try
            {
                List<Project_Budget> list = new List<Project_Budget>();
                cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetBudgetByProjectId");
                cmd.Parameters.AddWithValue("@id", Id);
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        list.Add(new Project_Budget
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            Project_Id = Convert.ToInt32(rd["project_id"]),
                            ProjectHeads = rd["heads"].ToString(),
                            ProjectAmount = Convert.ToDecimal(rd["headsAmount"] != DBNull.Value ? rd["headsAmount"] : 0),
                            Miscellaneous = rd["miscellaneous"].ToString(),
                            Miscell_amt = Convert.ToDecimal(rd["miscAmount"] != DBNull.Value ? rd["miscAmount"] : 0.00)
                        });
                    }
                }
                return list;
            }catch(Exception ex)
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

        public List<Project_Statge> ProjectStagesList(int Id)
        {
            try
            {
                List<Project_Statge> list = new List<Project_Statge>();
                cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetProjectStageByProjectId");
                cmd.Parameters.AddWithValue("@id", Id);
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        list.Add(new Project_Statge
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            Project_Id = Convert.ToInt32(rd["project_id"]),
                            KeyPoint = rd["keyPoint"].ToString(),
                            CompletionDate = Convert.ToDateTime(rd["completeDate"]),
                            Document_Url = rd["stageDocument"].ToString()
                        });
                    }
                }
                return list;
            }
            catch(Exception ex)
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
                while (sdr.Read())
                {
                    empObj = new Employee_model();
                    empObj.Id = Convert.ToInt32(sdr["id"]);
                    empObj.EmployeeName = sdr["name"].ToString();
                    empObj.EmployeeRole = sdr["role"].ToString();

                    empList.Add(empObj);
                }

                sdr.Close();

            }catch(Exception ex)
            {
                throw new Exception("An error accured",ex);
            }
            finally
            {
                con.Close();
            }
            return empList;
        }

        public bool insertMeeting(AddMeeting_Model obj)
        {
            con.Open();
            SqlTransaction transaction = con.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_ManageMeeting", con, transaction);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeetingType", obj.MeetingType);
                cmd.Parameters.AddWithValue("@meetingLink", obj.MeetingLink);
                cmd.Parameters.AddWithValue("@MeetingTitle", obj.MeetingTitle);
                cmd.Parameters.AddWithValue("@meetingTime", obj.MeetingTime);
                cmd.Parameters.AddWithValue("@meetingDocument", obj.Attachment_Url);
                cmd.Parameters.AddWithValue("@Id", obj.Id);
                if (obj.Id > 0)
                {
                    cmd.Parameters.AddWithValue("@action", "updateMeeting");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@action", "insertMeeting");
                }
                SqlParameter outputParam = new SqlParameter("@meetingId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);

                int i = cmd.ExecuteNonQuery();

                if (i > 0)
                {
                    int meetingId = (int)outputParam.Value;

                    if (obj.meetingMemberList != null)
                    {
                            foreach (var individualMember in obj.meetingMemberList)
                            {
                                 
                                if (individualMember != 0)
                                {
                                    cmd.Parameters.Clear();
                                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@action", "addMeetingMember");
                                    cmd.Parameters.AddWithValue("@employee", individualMember);
                                    cmd.Parameters.AddWithValue("@meeting", meetingId);
                                     i = cmd.ExecuteNonQuery();
                                }
                              
                            }
                        if (i <= 0)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }

                    if (obj.keyPointList != null)
                    {
                        foreach (var key in obj.keyPointList)
                        {
                            if (!string.IsNullOrEmpty(key))
                            {
                                cmd.Parameters.Clear();
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@action", "addMeetingKeyPoint");
                                cmd.Parameters.AddWithValue("@keyPoint", key);
                                cmd.Parameters.AddWithValue("@meeting", meetingId);
                                i = cmd.ExecuteNonQuery();
                            }
                                if (i <= 0)
                                {
                                    transaction.Rollback();
                                    return false;
                                }
                        
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                else
                {
                    transaction.Rollback();
                    return false;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("An error occurred while inserting the meeting", ex);
            }
            finally
            {
                con.Close();
            }
        }
        public List<Meeting_Model> getAllmeeting()
        {
            List<Meeting_Model> _list = new List<Meeting_Model>();
            Meeting_Model obj = null;
            try
            {

                SqlCommand cmd = new SqlCommand("sp_ManageMeeting", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getAllmeeting");
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
             
                while (sdr.Read())
                {
                    obj = new Meeting_Model();
                    obj.Id = Convert.ToInt32(sdr["id"]);
                    obj.CompleteStatus = Convert.ToInt32(sdr["completeStatus"]);
                    obj.MeetingType = sdr["meetingType"].ToString();
                    obj.MeetingLink = sdr["meetingLink"].ToString();
                    obj.MeetingTitle = sdr["MeetingTitle"].ToString();
                    obj.MeetingDate = Convert.ToDateTime(sdr["meetingTime"]).ToString("dd-MM-yyyy"); 
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

        public Meeting_Model getMeetingById(int id)
        {
            Meeting_Model obj = new Meeting_Model();

            try
            {
                con.Open();

                // Get meeting details
                using (SqlCommand cmd = new SqlCommand("sp_ManageMeeting", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@action", "getMeetingById");

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            obj.Id = Convert.ToInt32(sdr["id"]);
                            obj.MeetingType = sdr["meetingType"].ToString();
                            obj.MeetingLink = sdr["meetingLink"].ToString();
                            obj.MeetingTitle = sdr["meetingTitle"].ToString();
                            obj.MeetingTime = Convert.ToDateTime(sdr["meetingTime"]);
                            obj.empId = sdr["empId"].ToString();
                            obj.KeyPoint = sdr["meetingKey"].ToString();

                        }
                    }
                }
                if (!string.IsNullOrEmpty(obj.KeyPoint))
                {
                    if (obj.meetingKeyPoint == null)
                    {
                        obj.meetingKeyPoint = new List<string>();
                    }
                    foreach (var key in obj.KeyPoint.Split(','))
                    {
                        obj.meetingKeyPoint.Add(key);
                    }
                }

                if (!string.IsNullOrEmpty(obj.empId))
                {
                    if (obj.empName == null && obj.memberId == null)
                    {
                        obj.empName = new List<string>();
                        obj.memberId = new List<string>();
                    }
                    foreach (var emp in obj.empId.Split(','))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_ManageMeeting", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@id", emp);
                            cmd.Parameters.AddWithValue("@action", "getMeetingMemberById");

                            using (SqlDataReader sdr2 = cmd.ExecuteReader())
                            {
                                if (sdr2.Read())
                                {
                                    obj.MeetingMember = sdr2["name"].ToString();
                                    obj.empName.Add(obj.MeetingMember);
                                    obj.memberId.Add(emp);
                                }
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching meeting details", ex);
            }

            return obj;
        }

        #endregion End

        #region notice

        public bool InsertNotice(Generate_Notice gn)
        {
            try
            {
                cmd = new SqlCommand("sp_manageNotice", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", gn.Id>0?"UpdateNotice":"InsertNotice");
                cmd.Parameters.AddWithValue("@projectId",gn.ProjectId);
                cmd.Parameters.AddWithValue("@id",gn.Id);
                cmd.Parameters.AddWithValue("@noticeDocs", gn.Attachment_Url);
                cmd.Parameters.AddWithValue("@noticedesc", gn.Notice);
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

        public List<Generate_Notice> getNoticeList()
        {
            try
            {
                cmd = new SqlCommand("sp_manageNotice", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "SelectNotice");
                con.Open();
                List<Generate_Notice> noticeList = new List<Generate_Notice>();
                var  res=cmd.ExecuteReader() ;
                while (res.Read())
                {
                    noticeList.Add(new Generate_Notice
                    {
                        Id = (int)res["id"],
                        ProjectId = (int)res["project_id"],
                        ProjectManagerId = (int)res["empid"],
                        Attachment_Url = res["NoticeDocument"] == null ? null : res["NoticeDocument"].ToString(),
                        Notice = res["noticeDescription"].ToString(),
                        ProjectManagerImage = res["profile"].ToString(),
                        ProjectManager = res["name"].ToString(),
                        ProjectName = res["title"].ToString(),
                        noticeDate = Convert.ToDateTime(res["noticeDate"]).ToString("dd-MM-yyyy")

                    });
                }
                return noticeList;
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

    }
}