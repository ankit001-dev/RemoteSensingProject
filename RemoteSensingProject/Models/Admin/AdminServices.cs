using System;
using System.Collections.Generic;
using static RemoteSensingProject.Models.Admin.main;
using System.Data.SqlClient;
using System.Data;
using RemoteSensingProject.Models.MailService;
using System.Linq;
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
                cmd.Parameters.AddWithValue("@action", cr.id > 0 ? "UpdateDesignation" : "InsertDesignation");
                cmd.Parameters.AddWithValue("@designationName", cr.name);
                cmd.Parameters.AddWithValue("@id", cr.id);
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

        public bool InsertDivison(CommonResponse cr)
        {
            try
            {
                cmd = new SqlCommand("sp_ManageEmployeeCategory", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", cr.id > 0 ? "UpdateDevision" : "InsertDevision");
                cmd.Parameters.AddWithValue("@devisionName", cr.name);
                cmd.Parameters.AddWithValue("@id", cr.id);
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
                else if (res > 0)
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
            try
            {
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
                        EmployeeRole = record["role"].ToString().Trim(),
                        Division = (int)record["devision"],
                        DesignationName = record["designationName"].ToString(),
                        Status = (bool)record["status"],
                        ActiveStatus = (bool)record["activeStatus"],
                        CreationDate = Convert.ToDateTime(record["creationDate"]).ToString("dd-MM-yyyy"),
                        Image_url = record["profile"] != DBNull.Value ? record["profile"].ToString() : null
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
                Employee_model empModel = new Employee_model();
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
                cmd.Parameters.AddWithValue("@letterNo", pm.pm.letterNo);
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
                cmd.Parameters.AddWithValue("@ApproveStatus", 1);
                cmd.Parameters.Add("@project_Id", SqlDbType.Int);
                cmd.Parameters["@project_Id"].Direction = ParameterDirection.Output;
                int i = cmd.ExecuteNonQuery();
                int projectId = Convert.ToInt32(cmd.Parameters["@project_Id"].Value != DBNull.Value ? cmd.Parameters["@project_Id"].Value : 0);
                if (i > 0)
                {
                    if (pm.budgets != null && pm.budgets.Count > 0)
                    {
                        foreach (var item in pm.budgets)
                        {
                            cmd = new SqlCommand("sp_adminAddproject", con, tran);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@action", "insertProjectBudget");
                            cmd.Parameters.AddWithValue("@project_Id", projectId);
                            cmd.Parameters.AddWithValue("@heads", item.ProjectHeads);
                            cmd.Parameters.AddWithValue("@headsAmount", item.ProjectAmount);
                            i += cmd.ExecuteNonQuery();
                        }
                    }
                    if (pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage)
                    {
                        foreach (var item in pm.stages)
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
                        foreach (var item in pm.pm.SubOrdinate)
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
            }
            catch (Exception ex)
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
                            Percentage = rd["financialStatusPercentage"] != DBNull.Value ? rd["financialStatusPercentage"].ToString() : "",
                            ProjectBudget = Convert.ToDecimal(rd["budget"]),
                            ProjectDescription = rd["description"].ToString(),
                            projectDocumentUrl = rd["ProjectDocument"].ToString(),
                            ProjectType = rd["projectType"].ToString(),
                            physicalcomplete = Convert.ToDecimal(rd["completionPercentage"]),
                            overallPercentage = Convert.ToDecimal(rd["overallPercentage"]),
                            ProjectStage = Convert.ToBoolean(rd["stage"]),
                            CompletionDatestring = Convert.ToDateTime(rd["completionDate"]).ToString("dd-MM-yyyy"),
                            ProjectStatus = Convert.ToBoolean(rd["CompleteStatus"]),
                            AssignDateString = Convert.ToDateTime(rd["assignDate"]).ToString("dd-MM-yyyy"),
                            StartDateString = Convert.ToDateTime(rd["startDate"]).ToString("dd-MM-yyyy")

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
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }
        public List<Project_model> getHeadByProject(int projectId)
        {
            try
            {
                List<Project_model> _headList = new List<Project_model>();
                Project_model obj = null;
                SqlCommand cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getHeadByProject");
                cmd.Parameters.AddWithValue("@id", projectId);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        obj = new Project_model();
                        obj.Id = Convert.ToInt32(sdr["id"]);
                        obj.heads = sdr["heads"].ToString();
                        _headList.Add(obj);
                    }
                }
                return _headList;
            }
            catch (Exception ex)
            {
                throw new Exception("An error accured", ex);
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
                List<Project_Subordination> subList = new List<Project_Subordination>();
                Project_model pm = new Project_model();
                if (rd.HasRows)
                {

                    while (rd.Read())
                    {
                        pm.Id = Convert.ToInt32(rd["id"]);
                        pm.ProjectTitle = rd["title"].ToString();
                        pm.AssignDate = Convert.ToDateTime(rd["assignDate"]);
                        pm.AssignDateString = Convert.ToDateTime(rd["assignDate"]).ToString("dd-MM-yyyy");
                        pm.CompletionDate = Convert.ToDateTime(rd["completionDate"]);
                        pm.CompletionDatestring = Convert.ToDateTime(rd["completionDate"]).ToString("dd-MM-yyyy");
                        pm.StartDate = Convert.ToDateTime(rd["startDate"]);
                        pm.StartDateString = Convert.ToDateTime(rd["startDate"]).ToString("dd-MM-yyyy");
                        pm.ProjectManager = rd["ManagerName"].ToString();
                        pm.ProjectBudget = Convert.ToDecimal(rd["budget"]);
                        pm.ProjectDescription = rd["description"].ToString();
                        pm.projectDocumentUrl = rd["ProjectDocument"].ToString();
                        pm.ProjectType = rd["projectType"].ToString();
                        pm.ProjectStage = Convert.ToBoolean(rd["stage"]);
                        pm.ProjectStatus = Convert.ToBoolean(rd["CompleteStatus"]);
                        if (pm.ProjectType.Equals("External"))
                        {
                            pm.Address = rd["address"].ToString();
                            pm.ProjectDepartment = rd["DepartmentName"].ToString();
                            pm.ContactPerson = rd["contactPerson"].ToString();
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
                    }

                    rd.Close();
                }
                cpm.pm = pm;
                cpm.SubOrdinate = subList;
                cpm.budgets = ProjectBudgetList(id);
                cpm.stages = ProjectStagesList(id);
                return cpm;
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
                cmd.Parameters.AddWithValue("@createdBy", pm.createdBy);
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
            }
            catch (Exception ex)
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

        public bool insertProjectStages(Project_Statge stg)
        {
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
                if(con.State == ConnectionState.Closed)
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
                            TotalAskAmount = rd["totalAskAmount"].ToString(),
                            ApproveAmount = rd["approveAmount"].ToString(),
                            ProjectAmount = Convert.ToDecimal(rd["headsAmount"] != DBNull.Value ? rd["headsAmount"] : 0)
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
                if (con.State == ConnectionState.Closed)
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
                            CompletionDatestring = Convert.ToDateTime(rd["completeDate"]).ToString("dd-MM-yyyy"),
                            Status = rd["StagesStatus"].ToString(),
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
                    obj.TotalProjectManager = sdr["TotalProjectManager"].ToString();
                    obj.TotalSubOrdinate = sdr["TotalSubOrdinate"].ToString();
                    obj.TotalAccounts = sdr["TotalAccounts"].ToString();
                    obj.TotalProject = sdr["TotalProject"].ToString();
                    obj.TotalInternalProject = sdr["TotalInternalProject"].ToString();
                    obj.TotalExternalProject = sdr["TotalExternalProject"].ToString();
                    obj.TotalDelayproject = sdr["TotalDelayproject"].ToString();
                    obj.TotalCompleteProject = sdr["TotalCompleteProject"].ToString();
                    obj.TotalOngoingProject = sdr["TotalOngoingProject"].ToString();
                    obj.TotalMeetings = sdr["TotalMeetings"].ToString();
                    obj.TotalAdminMeetings = sdr["TotalAdminMeetings"].ToString();
                    obj.TotalProjectManagerMeetings = sdr["TotalProjectManagerMeetings"].ToString();
                    obj.TotalBudget = Convert.ToDecimal(sdr["totalBudgets"]);
                    obj.PendingBudget = Convert.ToDecimal(sdr["pendingBudget"]);
                    obj.expenditure = Convert.ToDecimal(sdr["expenditure"]);
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
        public List<DashboardCount> getAllProjectCompletion()
        {
            try
            {
                List<DashboardCount> list = new List<DashboardCount>();
                DashboardCount obj = null;
                SqlCommand cmd = new SqlCommand("sp_ManageDashboard", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getOverallProjectCompletion");
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        obj = new DashboardCount();
                        obj.Title = sdr["title"].ToString();
                        obj.OverallCompletionPercentage = sdr["overallCompletion"].ToString();
                        obj.ProjectManager = sdr["ProjectManager"].ToString();
                        list.Add(obj);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("An error accured", ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
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

            }
            catch (Exception ex)
            {
                throw new Exception("An error accured", ex);
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
                cmd.Parameters.AddWithValue("@meetingLink", obj.MeetingType.ToLower() == "offline" ? obj.MeetingAddress : obj.MeetingLink);
                cmd.Parameters.AddWithValue("@MeetingTitle", obj.MeetingTitle);
                cmd.Parameters.AddWithValue("@createrId", obj.CreaterId>0?obj.CreaterId:null);
                cmd.Parameters.AddWithValue("@createdBy", obj.CreaterId!=null && obj.CreaterId > 0?"projectManager":"admin");
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

                    if (obj.keyPointList != null && obj.keyPointList.Count>0)
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

        public bool UpdateMeeting(AddMeeting_Model obj)
        {
            con.Open();
            SqlTransaction transaction = con.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_ManageMeeting", con, transaction);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MeetingType", obj.MeetingType);
                cmd.Parameters.AddWithValue("@meetingLink", obj.MeetingType.ToLower()=="offline"?obj.MeetingAddress: obj.MeetingLink);
                cmd.Parameters.AddWithValue("@MeetingTitle", obj.MeetingTitle);
                cmd.Parameters.AddWithValue("@meetingTime", obj.MeetingTime);
                cmd.Parameters.AddWithValue("@meetingDocument", obj.Attachment_Url);
                cmd.Parameters.AddWithValue("@Id", obj.Id);
                cmd.Parameters.AddWithValue("@action", "updateMeeting");

                int i = cmd.ExecuteNonQuery();

                if (i > 0)
                {
                    if (obj.meetingMemberList != null)
                    {
                        foreach (var individualMember in obj.meetingMemberList)
                        {
                            if (individualMember != 0)
                            {
                                cmd.Parameters.Clear();
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@action", "updateMember");
                                cmd.Parameters.AddWithValue("@employee", individualMember);
                                cmd.Parameters.AddWithValue("@meeting", obj.Id);
                                i = cmd.ExecuteNonQuery();
                            }

                        }
                        if (i <= 0)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }

                    for (var j = 0; j < obj.KeypointId.Count; j++)
                    {
                        if (!string.IsNullOrEmpty(obj.KeypointId[j].ToString()) && !string.IsNullOrEmpty(obj.keyPointList[j].ToString()))
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@action", "updateKeyPoint");
                            cmd.Parameters.AddWithValue("@keyPoint", obj.keyPointList[j].ToString());
                            cmd.Parameters.AddWithValue("@id", obj.KeypointId[j]);
                            cmd.Parameters.AddWithValue("@meeting", obj.Id);
                            i = cmd.ExecuteNonQuery();
                        }
                    }
                    if (i <= 0)
                    {
                        transaction.Rollback();
                        return false;
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
            try
            {
            List<Meeting_Model> _list = new List<Meeting_Model>();
            Meeting_Model obj = null;
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
                    obj.memberId = sdr["memberId"]!=DBNull.Value?sdr["memberId"].ToString().Split(',').ToList():new List<string>();
                    obj.CreaterId = sdr["createrId"] != DBNull.Value ? Convert.ToInt32(sdr["createrId"]):0;
                    obj.MeetingDate = Convert.ToDateTime(sdr["meetingTime"]).ToString("dd-MM-yyyy");
                    obj.summary = sdr["meetSummary"].ToString();
                    _list.Add(obj);
                }

                sdr.Close();
            return _list;
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
                            obj.MeetingTimeString = Convert.ToDateTime(sdr["meetingTime"]).ToString("yyyy-MM-ddTHH:mm");
                            obj.memberId = sdr["empId"]!=DBNull.Value? sdr["empId"].ToString().Split(',').ToList() :new List<string>();

                        }
                        if (sdr["meetingKey"] != null)
                        {
                            List<KeyPoint> keyDict = new List<KeyPoint>();
                            foreach (var key in sdr["meetingKey"].ToString().Split(','))
                            {
                                if (!string.IsNullOrEmpty(key))
                                {

                                keyDict.Add(new KeyPoint { Id = int.Parse(key.Split(':')[0]), keyPoint = key.Split(':')[1] });
                                }
                            }
                            obj.MeetingKeyPointDict = keyDict;
                        }
                    }
                }


                //if (!string.IsNullOrEmpty(obj.empId))
                //{
                //    if (obj.empName == null && obj.memberId == null)
                //    {
                //        obj.empName = new List<string>();
                //        obj.memberId = new List<string>();
                //    }
                //    if (obj.empId != null) { 
                //    foreach (var emp in obj.empId.Split(','))
                //    {
                //        using (SqlCommand cmd = new SqlCommand("sp_ManageMeeting", con))
                //        {
                //            cmd.CommandType = CommandType.StoredProcedure;
                //            cmd.Parameters.AddWithValue("@id", emp);
                //            cmd.Parameters.AddWithValue("@action", "getMeetingMemberById");

                //            using (SqlDataReader sdr2 = cmd.ExecuteReader())
                //            {
                //                if (sdr2.Read())
                //                {
                //                    obj.MeetingMember = sdr2["name"].ToString();
                //                    obj.empName.Add(obj.MeetingMember);
                //                    obj.memberId.Add(emp);
                //                }
                //            }
                //        }
                //    }
                //    }
                //}


            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching meeting details", ex);
            }
            finally
            {
                con.Close();
                cmd.Dispose();
            }
            return obj;
        }

        public List<Employee_model> GetMeetingMemberList(int id)
        {
            try {

                con.Open();
                cmd.Parameters.Clear();
                cmd = new SqlCommand("sp_ManageMeeting", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@action", "getMeetingMemberById");
                cmd.CommandType = CommandType.StoredProcedure;
                List<Employee_model> empModel = new List<Employee_model>();
               
                SqlDataReader res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        empModel.Add(new Employee_model
                        {
                            Id = (int)res["id"],
                            EmployeeCode = res["employeeCode"].ToString(),
                            EmployeeName = res["name"].ToString(),
                            EmployeeRole = res["role"].ToString(),
                            MobileNo = (long)res["mobile"],
                            Email = res["email"].ToString(),
                            meetingId = (int)res["meetingId"]
                        });

                    }
                }
                return empModel;
            }
            catch (Exception ex)
            {
                throw new Exception("An error accured", ex);
    }
            finally
            {
                con.Close();
                cmd.Dispose();
            }
        }

        public bool AddMeetingResponse(MeetingConclusion mc)
        {
            con.Open();
            SqlTransaction transaction = con.BeginTransaction();
            try
            {
                cmd.Parameters.Clear();
                cmd = new SqlCommand("sp_meetingConslusion", con, transaction);
                cmd.Parameters.AddWithValue("@action", "insertConclusion");
                cmd.Parameters.AddWithValue("@meeting", mc.Meeting);
                cmd.Parameters.AddWithValue("@conclusion", mc.Conclusion);
                cmd.Parameters.AddWithValue("@nextFollow", mc.NextFollowUpDate);
                cmd.Parameters.AddWithValue("@followUpStatus", mc.FollowUpStatus);
                cmd.Parameters.AddWithValue("@summary", mc.summary);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter outputParam = new SqlParameter("@conclusionId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);

                int res = cmd.ExecuteNonQuery();
                if (res > 0)
                {
                    int cId = (int)outputParam.Value;
                    if (mc.MemberId.Count > 0)
                    {
                        foreach(var item in mc.MemberId) {
                            if (!string.IsNullOrEmpty(item)){
                                cmd.Parameters.Clear();
                                cmd = new SqlCommand("sp_meetingConslusion", con, transaction);
                                cmd.Parameters.AddWithValue("@action", "updateMeetingMemberPresence");
                                cmd.Parameters.AddWithValue("@id", cId);
                                cmd.Parameters.AddWithValue("@memberId", item);
                                cmd.CommandType = CommandType.StoredProcedure;
                                res = cmd.ExecuteNonQuery();
                                if (res <= 0)
                                {
                                    transaction.Rollback();
                                    return false;
                                }
                    }
                            }
                    }
                    if (res > 0)
                    {
                        if (mc.KeyPointId.Count > 0)
                        {
                            for(var i = 0; i < mc.KeyPointId.Count; i++) 
                            {
                                cmd.Parameters.Clear();
                                cmd = new SqlCommand("sp_meetingConslusion", con, transaction);
                                cmd.Parameters.AddWithValue("@action", "isertKeypointResponse");
                                cmd.Parameters.AddWithValue("@keyId", mc.KeyPointId[i]);
                                cmd.Parameters.AddWithValue("@response", mc.KeyResponse[i]);
                                cmd.Parameters.AddWithValue("@id", cId);
                                cmd.CommandType = CommandType.StoredProcedure;
                                res = cmd.ExecuteNonQuery();
                                if (res <= 0)
                                {
                                    transaction.Rollback();
                                    return false;
                                }
                            }
                        }
                    }

                    if (mc.MeetingMemberList != null && mc.FollowUpStatus)
                    {
                        foreach (var individualMember in mc.MeetingMemberList)
                        {

                            if (individualMember!=0)
                            {
                                cmd.Parameters.Clear();
                                cmd = new SqlCommand("sp_ManageMeeting", con, transaction);
                                cmd.Parameters.AddWithValue("@action", "addMeetingMember");
                                cmd.Parameters.AddWithValue("@employee", individualMember);
                                cmd.Parameters.AddWithValue("@meeting", mc.Meeting);
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                res = cmd.ExecuteNonQuery();
                            }

                        }

                        if (res <= 0)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }

                    transaction.Commit();
                    return true;

                }else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("An error accured", ex);
            }
            finally
            {
                con.Close();
                cmd.Dispose();
            }
        }
        public List<MeetingConclusion> getConclusion(int id)
        {
            try
            {

                cmd.Parameters.Clear();
                cmd = new SqlCommand("sp_meetingConslusion", con);
                cmd.Parameters.AddWithValue("@action", "selectConclusion");
                cmd.Parameters.AddWithValue("@meeting", id);
                cmd.CommandType = CommandType.StoredProcedure;
                List<MeetingConclusion> meetingc = new List<MeetingConclusion>();

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        meetingc.Add(new MeetingConclusion
                        {
                            Id = Convert.ToInt32(rdr["id"]),
                            Meeting = id,
                            MeetingDate = rdr["meetingTime"]!=DBNull.Value?Convert.ToDateTime(rdr["meetingTime"]).ToString("dd-MM-yyyy hh:mm tt"):"N/A",
                            Conclusion = rdr["conclusion"].ToString(),
                            NextFollow = rdr["nextFollow"]!=DBNull.Value?Convert.ToDateTime(rdr["nextFollow"]).ToString("dd-MM-yyyy"):"N/A",
                            mode = rdr["meetingType"].ToString(),
                            address = rdr["meetingLink"].ToString()
                        });
                    }
                }
                return meetingc;
            }
            catch (Exception ex)
            {
                throw new Exception("An error accured", ex);
            }
            finally
            {
                con.Close();
                cmd.Dispose();
            }
        }

        public List<Employee_model> getPresentMember(int id)
        {
            try
            {

                cmd.Parameters.Clear();
                cmd = new SqlCommand("sp_meetingConslusion", con);
                cmd.Parameters.AddWithValue("@action", "selectPresentMember");
                cmd.Parameters.AddWithValue("@id", id);
                cmd.CommandType = CommandType.StoredProcedure;
                List<Employee_model> meetingc = new List<Employee_model>();

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        meetingc.Add(new Employee_model
                        {
                            EmployeeName = rdr["name"].ToString(),
                            Image_url = rdr["profile"].ToString(),
                            EmployeeRole = rdr["role"].ToString(),
                            PresentStatus = (bool)rdr["presentStatus"]
                           
                        });
                    }
                }
                return meetingc;
            }
            catch (Exception ex)
            {
                throw new Exception("An error accured", ex);
            }
            finally
            {
                con.Close();
                cmd.Dispose();
            }
        }

        public List<KeyPointResponse> getKeypointResponse(int id)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd = new SqlCommand("sp_meetingConslusion", con);
                cmd.Parameters.AddWithValue("@action", "KeyPointResponse");
                cmd.Parameters.AddWithValue("@id", id);
                cmd.CommandType = CommandType.StoredProcedure;
                List<KeyPointResponse> md = new List<KeyPointResponse>();
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        md.Add(new KeyPointResponse
                        {
                            KeyPoint = rdr["keypoint"].ToString(),
                            Response = rdr["response"].ToString()

                        });
                    }
                }
                return md;
            }
            catch (Exception ex)
            {
                throw new Exception("An error accured", ex);
            }
            finally
            {
                con.Close();
                cmd.Dispose();
            }
        }

        #endregion End

        #region notice

        public bool InsertNotice(Generate_Notice gn)
        {
            try
            {
                cmd = new SqlCommand("sp_manageNotice", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", gn.Id > 0 ? "UpdateNotice" : "InsertNotice");
                cmd.Parameters.AddWithValue("@projectId", gn.ProjectId);
                cmd.Parameters.AddWithValue("@id", gn.Id);
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
                var res = cmd.ExecuteReader();
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

        //#region Budget

        //public bool InsertBuget(BudgetForm data)
        //{
        //    try
        //    {
        //        cmd = new SqlCommand("sp_manageBudgets", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@budget", data.budget);
        //        cmd.Parameters.AddWithValue("@action", "insert");
        //        con.Open();
        //       return cmd.ExecuteNonQuery()> 0;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        if(con.State == ConnectionState.Open)
        //        con.Close();
        //        cmd.Dispose();
        //    }
        //}

        //public List<BudgetForm> getBudgetList()
        //{
        //    try
        //    {
        //        cmd = new SqlCommand("sp_manageBudgets", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@action", "selectAll");
        //        con.Open();
        //        List<BudgetForm> budgetList = new List<BudgetForm>();
        //        var res = cmd.ExecuteReader();
        //        if (res.HasRows)
        //        {
        //            while (res.Read())
        //            {
        //                budgetList.Add(new BudgetForm
        //                {
        //                    sn = (int)res["id"],
        //                    addedDate = Convert.ToString(res["createdAt"]),
        //                    budget = (decimal)res["budget"]
        //                });
        //            }
        //        }
        //        return budgetList;
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (con.State == ConnectionState.Open)
        //            con.Close();
        //        cmd.Dispose();
        //    }
        //}

        //#endregion

        #region /*tour*/
        public List<tourProposalAll> getAllTourList()
        {
            try
            {
                cmd = new SqlCommand("sp_Tourproposal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectAlltour");
                con.Open();
                List<tourProposalAll> getlist = new List<tourProposalAll>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        getlist.Add(new tourProposalAll
                        {
                            id = Convert.ToInt32(res["id"]),
                            projectName = res["title"].ToString(),
                            projectManager = res["name"].ToString(),
                            dateOfDept = Convert.ToDateTime(res["dateOfDept"]),
                            place = res["place"].ToString(),
                            periodFrom = Convert.ToDateTime(res["periodFrom"]),
                            periodTo = Convert.ToDateTime(res["periodTo"]),
                            returnDate = Convert.ToDateTime(res["returnDate"]),
                            purpose = res["purpose"].ToString(),
                        });
                    }
                }
                return getlist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }

        public bool Tourapproval(int id,bool status)
        {
            try
            {
                cmd = new SqlCommand("sp_Tourproposal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "approval");
                    cmd.Parameters.AddWithValue("@adminappr", status);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }
        #endregion

        #region /*Reimbursement request approval*/
        public bool ReimbursementApproval(bool status, int userId, string type)
        {
            try
            {
                cmd = new SqlCommand("sp_Reimbursement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "approval");
                cmd.Parameters.AddWithValue("@admin_appr", status);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@type", type);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }

        public List<AdminReimbursement> GetSpecificUserReimbursements(int id, string type)
        {
            try
            {
                cmd = new SqlCommand("sp_Reimbursement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetAllSubmittedData");
                cmd.Parameters.AddWithValue("@userId", id);
                cmd.Parameters.AddWithValue("@type", type);
                con.Open();
                List<AdminReimbursement> getlist = new List<AdminReimbursement>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        getlist.Add(new AdminReimbursement
                        {
                            id = (int)res["id"],
                            type = (string)res["type"],
                            vrNo = (string)res["vrNo"],
                            date = Convert.ToDateTime(res["date"]),
                            particulars = (string)res["particulars"],
                            items = (string)res["items"],
                            amount = Convert.ToDecimal(res["amount"]),
                            purpose = (string)res["purpose"],
                            status = Convert.ToBoolean(res["status"]),
                            newRequest = (bool)res["newRequest"],
                            adminappr = (bool)res["admin_appr"]
                        });
                    }
                }
                return getlist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }

        #endregion

        #region /* Hiring*/
        public List<HiringVehicle1> HiringList()
        {
            try
            {
                cmd = new SqlCommand("sp_HiringVehicle", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectAllHiring");
                con.Open();
                List<HiringVehicle1> list = new List<HiringVehicle1>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        list.Add(new HiringVehicle1
                        {
                            id = (int)res["id"],
                            projectName = Convert.ToString(res["title"]),
                            headName = Convert.ToString(res["heads"]),
                            amount = Convert.ToDecimal(res["amount"]),
                            dateFrom = Convert.ToDateTime(res["dateFrom"]),
                            dateTo = Convert.ToDateTime(res["dateTo"]),
                            proposedPlace =res["proposedPlace"].ToString(),
                            purposeOfVisit =res["purposeOfVisit"].ToString(),
                            totalDaysNight =res["totalDaysNight"].ToString(),
                            totalPlainHills =res["totalPlainHills"].ToString(),
                            taxi = res["taxi"].ToString(),
                            BookAgainstCentre = res["BookAgainstCentre"].ToString(),
                            availbilityOfFund = res["availbilityOfFund"].ToString(),
                            taxiReportTo = res["taxiReportTo"].ToString(),
                            taxiReportAt = (TimeSpan)res["taxiReportAt"],
                            taxiReportPlace = res["taxiReportPlace"].ToString(),
                            taxiReportOn = Convert.ToDateTime(res["taxiReportOn"])
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
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }

        public bool HiringApproval(int id, bool status)
        {
            try
            {
                cmd = new SqlCommand("sp_HiringVehicle", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "approval");
                cmd.Parameters.AddWithValue("@adminappr", status);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Dispose();
            }
        }
        #endregion
    }
}