using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json.Linq;
using Npgsql;
using NpgsqlTypes;
using RemoteSensingProject.Models.MailService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using static RemoteSensingProject.Models.Admin.main;


namespace RemoteSensingProject.Models.Admin
{
    public class AdminServices : DataFactory
    {

        #region Employee Category
        mail _mail = new mail();


        public bool InsertDesignation(CommonResponse cr)
        {
            try
            {
                using (var cmd = new NpgsqlCommand(
                    "CALL sp_manageemployeecategory(:v_id, :v_designationname, :v_status, :v_devisionname, :v_action, :v_rc)", con))
                {
                    cmd.CommandType = CommandType.Text; // must be Text for CALL

                    cmd.Parameters.AddWithValue("v_id", cr.id == 0 ? (object)DBNull.Value : cr.id);
                    cmd.Parameters.AddWithValue("v_designationname", cr.name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("v_status", true);
                    cmd.Parameters.AddWithValue("v_devisionname", DBNull.Value);
                    cmd.Parameters.AddWithValue("v_action", cr.id > 0 ? "UpdateDesignation" : "InsertDesignation");
                    cmd.Parameters.Add(new NpgsqlParameter("v_rc", NpgsqlDbType.Refcursor)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = DBNull.Value
                    });

                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }



        public bool InsertDivison(CommonResponse cr)
        {
            try
            {
                using (var cmd = new NpgsqlCommand(
                    "CALL sp_manageemployeecategory(:v_id, :v_designationname, :v_status, :v_devisionname, :v_action, :v_rc)", con))
                {
                    cmd.CommandType = CommandType.Text; // must be Text for CALL

                    cmd.Parameters.AddWithValue("v_id", cr.id == 0 ? (object)DBNull.Value : cr.id);
                    cmd.Parameters.AddWithValue("v_designationname", DBNull.Value);
                    cmd.Parameters.AddWithValue("v_status", true);
                    cmd.Parameters.AddWithValue("v_devisionname", cr.name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("v_action", cr.id > 0 ? "UpdateDevision" : "InsertDevision");
                    cmd.Parameters.Add(new NpgsqlParameter("v_rc", NpgsqlDbType.Refcursor)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = DBNull.Value
                    });

                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
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

        public List<CommonResponse> ListDivison()
        {
            try
            {
                List<CommonResponse> list = new List<CommonResponse>();
                using (var cmd = new NpgsqlCommand("select * FROM fn_get_employee_category(@action)", con))
                {
                    cmd.Parameters.AddWithValue("@action", "GetAllDevision");
                    con.Open();
                    NpgsqlDataReader rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            list.Add(new CommonResponse
                            {
                                id = Convert.ToInt32(rd["id"]),
                                name = rd["name"].ToString()
                            });
                        }
                        rd.Close();
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
                cmd = new NpgsqlCommand("select * FROM fn_get_employee_category(@action)", con);
                cmd.Parameters.AddWithValue("@action", "GetAllDesignation");
                con.Open();
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        list.Add(new CommonResponse
                        {
                            id = Convert.ToInt32(rd["id"]),
                            name = rd["name"].ToString()
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
                using (var cmd = new NpgsqlCommand(
                    "CALL sp_manageemployeecategory(:v_id, :v_designationname, :v_status, :v_devisionname, :v_action, :v_rc)", con))
                {
                    cmd.CommandType = CommandType.Text; // must be Text for CALL

                    cmd.Parameters.AddWithValue("v_id", Id == 0 ? (object)DBNull.Value : Id);
                    cmd.Parameters.AddWithValue("v_designationname", DBNull.Value);
                    cmd.Parameters.AddWithValue("v_status", false);
                    cmd.Parameters.AddWithValue("v_devisionname", DBNull.Value);
                    cmd.Parameters.AddWithValue("v_action", "deleteDevision");
                    cmd.Parameters.Add(new NpgsqlParameter("v_rc", NpgsqlDbType.Refcursor)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = DBNull.Value
                    });

                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
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
        public bool removeDesgination(int Id)
        {

            try
            {
                using (var cmd = new NpgsqlCommand(
                    "CALL sp_manageemployeecategory(:v_id, :v_designationname, :v_status, :v_devisionname, :v_action, :v_rc)", con))
                {
                    cmd.CommandType = CommandType.Text; // must be Text for CALL

                    cmd.Parameters.AddWithValue("v_id", Id == 0 ? (object)DBNull.Value : Id);
                    cmd.Parameters.AddWithValue("v_designationname", DBNull.Value);
                    cmd.Parameters.AddWithValue("v_status", false);
                    cmd.Parameters.AddWithValue("v_devisionname", DBNull.Value);
                    cmd.Parameters.AddWithValue("v_action", "deleteDesignation");
                    cmd.Parameters.Add(new NpgsqlParameter("v_rc", NpgsqlDbType.Refcursor)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = DBNull.Value
                    });

                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
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

        #region add Employee
        public bool AddEmployees(Employee_model emp)
        {
            NpgsqlCommand cmd = null;
            try
            {
                using (cmd = new NpgsqlCommand(
                    "CALL sp_adminemployees(:p_id, :p_employeecode, :p_name, :p_mobile, :p_email, :p_gender, :p_role, :p_username, :p_password, :p_devision, :p_designation, :p_profile, :p_action, :p_rc)",
                    con))
                {
                    cmd.CommandType = CommandType.Text; // must be Text for CALL

                    // Generate username
                    string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    Random rnd = new Random();
                    string userName = emp.EmployeeName.Substring(0, Math.Min(5, emp.EmployeeName.Length))
                                        + "@" + emp.MobileNo.ToString().PadLeft(4, '0')
                                        .Substring(emp.MobileNo.ToString().Length - 4);

                    // Generate password only for new employees
                    string userPassword = "";
                    if (emp.Id == 0)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            userPassword += validChars[rnd.Next(validChars.Length)];
                        }
                    }

                    // Determine the procedure action
                    string actionType = emp.Id > 0 ? "UpdateEmployees" : "InsertEmployees";

                    // Add parameters
                    cmd.Parameters.AddWithValue("p_id", emp.Id == 0 ? (object)DBNull.Value : emp.Id);
                    cmd.Parameters.AddWithValue("p_employeecode", emp.EmployeeCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_name", emp.EmployeeName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_mobile", emp.MobileNo);
                    cmd.Parameters.AddWithValue("p_email", emp.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_gender", emp.Gender ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_role", emp.EmployeeRole ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_username", userName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_password", userPassword ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_devision", emp.Division);
                    cmd.Parameters.AddWithValue("p_designation", emp.Designation);
                    cmd.Parameters.AddWithValue("p_profile", emp.Image_url ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_action", actionType);

                    cmd.Parameters.Add(new NpgsqlParameter("p_rc", NpgsqlDbType.Refcursor)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = DBNull.Value
                    });

                    con.Open();
                    cmd.ExecuteNonQuery();

                    // Send mail only for insert
                    if (emp.Id == 0)
                    {
                        string subject = "Login Credential";
                        string message = $"<p>Your user id: <b>{userName}</b></p><br><p>Password: <b>{userPassword}</b></p>";
                        _mail.SendMail(emp.EmployeeName, emp.Email, subject, message);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding/updating employee: " + ex.Message, ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public bool RemoveEmployees(int id)
        {
            NpgsqlCommand cmd = null;
            try
            {
                cmd = new NpgsqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "CALL sp_adminemployees(p_id => @p_id, p_action => @p_action)";

                // Set procedure parameters
                cmd.Parameters.AddWithValue("p_action", "DeleteEmployees");
                cmd.Parameters.AddWithValue("p_id", id);

                con.Open();
                cmd.ExecuteNonQuery();
                return true; // if no exception, assume success
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

        public List<Employee_model> SelectEmployeeRecord(int? page = null, int? limit = null)
        {
            List<Employee_model> empModel = new List<Employee_model>();

            try
            {
                con.Open();

                using (var cmd = new NpgsqlCommand("SELECT * FROM fn_get_employees(@v_action,@v_id,@v_limit,@v_page);", con))
                {
                    cmd.CommandType = CommandType.Text; // Use text since function returns rows
                    cmd.Parameters.AddWithValue("@v_action", "SelectEmployees");
                    cmd.Parameters.AddWithValue("@v_id", 0);
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);

                    using (var record = cmd.ExecuteReader())
                    {
                        bool firstRow = true;
                        while (record.Read())
                        {
                            var employee = new Employee_model
                            {
                                Id = Convert.ToInt32(record["id"]),
                                EmployeeCode = record["employeecode"]?.ToString(),
                                EmployeeName = record["name"]?.ToString(),
                                DevisionName = record["devisionname"]?.ToString(),
                                Email = record["email"]?.ToString(),
                                MobileNo = record["mobile"] != DBNull.Value ? Convert.ToInt64(record["mobile"]) : 0,
                                EmployeeRole = record["role"] != DBNull.Value ? record["role"].ToString().Trim() : "",
                                Division = record["devision"] != DBNull.Value ? Convert.ToInt32(record["devision"]) : 0,
                                DesignationName = record["designationname"]?.ToString(),
                                Status = record["status"] != DBNull.Value && Convert.ToBoolean(record["status"]),
                                Image_url = record["profile"] != DBNull.Value ? record["profile"].ToString() : null,
                                ActiveStatus = Convert.ToBoolean(record["activestatus"])
                            };

                            // Assign pagination info only for the first row
                            if (firstRow)
                            {
                                employee.Pagination = new ApiCommon.PaginationInfo
                                {
                                    PageNumber = page ?? 0,
                                    TotalPages = Convert.ToInt32(record["totalpages"] != DBNull.Value ? record["totalpages"] : 0),
                                    TotalRecords = Convert.ToInt32(record["totalrecords"] != DBNull.Value ? record["totalrecords"] : 0),
                                    PageSize = limit ?? 0
                                };
                                firstRow = false; // Optional: ensure pagination is only assigned once
                            }

                            empModel.Add(employee);

                        }
                    }
                }

                return empModel;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching employee records", ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }


        public Employee_model SelectEmployeeRecordById(int id)
        {
            try
            {
                Employee_model empModel = new Employee_model();
                con.Open();

                using (var cmd = new NpgsqlCommand("SELECT * FROM fn_get_employees(@v_action,@v_id);", con))
                {
                    cmd.CommandType = CommandType.Text; // Use text since function returns rows
                    cmd.Parameters.AddWithValue("@v_action", "SelectEmployeesById");
                    cmd.Parameters.AddWithValue("@v_id", id);

                    using (var record = cmd.ExecuteReader())
                    {
                        while (record.Read())
                        {
                            empModel = new Employee_model
                            {
                                Id = Convert.ToInt32(record["id"]),
                                EmployeeCode = record["employeecode"]?.ToString(),
                                EmployeeName = record["name"]?.ToString(),
                                DevisionName = record["devisionname"]?.ToString(),
                                Email = record["email"]?.ToString(),
                                MobileNo = record["mobile"] != DBNull.Value ? Convert.ToInt64(record["mobile"]) : 0,
                                EmployeeRole = record["role"] != DBNull.Value ? record["role"].ToString().Trim() : "",
                                Division = record["devision"] != DBNull.Value ? Convert.ToInt32(record["devision"]) : 0,
                                Designation = record["designation"] != DBNull.Value ? Convert.ToInt32(record["devision"]) : 0,
                                DesignationName = record["designationname"]?.ToString(),
                                Gender = record["gender"]?.ToString(),
                                Status = record["status"] != DBNull.Value && Convert.ToBoolean(record["status"]),
                                Image_url = record["profile"] != DBNull.Value ? record["profile"].ToString() : null,
                                ActiveStatus = Convert.ToBoolean(record["activeStatus"])
                            };
                        }
                    }
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
                cmd = new NpgsqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "CALL sp_adminemployees(p_id => @p_id, p_action => @p_action)";

                // Set procedure parameters
                cmd.Parameters.AddWithValue("p_action", "ChangeActiveStatus");
                cmd.Parameters.AddWithValue("p_id", id);

                con.Open();
                cmd.ExecuteNonQuery();
                return true; // if no exception, assume success
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
            NpgsqlTransaction tran = null;
            NpgsqlCommand cmd = null;

            try
            {
                con.Open();
                tran = con.BeginTransaction();

                // Generate Project Code
                Random rand = new Random();
                pm.projectCode = $"{rand.Next(1000, 9999)}{DateTime.Now.Day}{DateTime.Now.Year.ToString().Substring(2, 2)}";

                // 1️⃣ Insert main project
                var projectParams = new Dictionary<string, object>
                {
                    ["p_action"] = "insertProject",
                    ["p_letterno"] = int.TryParse(pm.pm.letterNo, out int letterNo) ? letterNo : 0,
                    ["p_title"] = pm.pm.ProjectTitle,
                    ["p_assigndate"] = pm.pm.AssignDate,
                    ["p_startdate"] = pm.pm.StartDate,
                    ["p_completiondate"] = pm.pm.CompletionDate,
                    ["p_projectmanager"] = int.TryParse(pm.pm.ProjectManager, out int ProjectManager) ? ProjectManager : 0,
                    ["p_budget"] = pm.pm.ProjectBudget,
                    ["p_description"] = pm.pm.ProjectDescription,
                    ["p_projectdocument"] = pm.pm.projectDocumentUrl,
                    ["p_projecttype"] = pm.pm.ProjectType,
                    ["p_stage"] = pm.pm.ProjectStage,
                    ["p_createdby"] = pm.pm.createdBy,
                    ["p_status"] = true,
                    ["p_approvestatus"] = true,
                    ["p_projectcode"] = pm.projectCode
                };

                int projectId = ExecuteProjectAction(projectParams, tran);

                // 2️⃣ Insert budgets
                if (pm.budgets != null && pm.budgets.Count > 0)
                {
                    foreach (var item in pm.budgets)
                    {
                        var budgetParams = new Dictionary<string, object>
                        {
                            ["p_action"] = "insertProjectBudget",
                            ["p_project_id"] = projectId,
                            ["p_heads"] = item.ProjectHeads,
                            ["p_headsamount"] = item.ProjectAmount
                        };
                        ExecuteProjectAction(budgetParams, tran);
                    }
                }

                // 3️⃣ Insert stages
                if (pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage)
                {
                    foreach (var item in pm.stages)
                    {
                        var stageParams = new Dictionary<string, object>
                        {
                            ["p_action"] = "insertProjectStage",
                            ["p_project_id"] = projectId,
                            ["p_keypoint"] = item.KeyPoint,
                            ["p_completiondate"] = item.CompletionDate,
                            ["p_stagedocument"] = item.Document_Url
                        };
                        ExecuteProjectAction(stageParams, tran);
                    }
                }

                // 4️⃣ Insert subordinates
                if (pm.pm.SubOrdinate != null && pm.pm.SubOrdinate.Length > 0)
                {
                    foreach (var subId in pm.pm.SubOrdinate)
                    {
                        var subParams = new Dictionary<string, object>
                        {
                            ["p_action"] = "insertSubOrdinate",
                            ["p_project_id"] = projectId,
                            ["p_id"] = subId,
                            ["p_projectmanager"] = int.TryParse(pm.pm.ProjectManager, out int SubProjectManager) ? SubProjectManager : 0
                        };
                        ExecuteProjectAction(subParams, tran);
                    }
                }

                // 5️⃣ Insert External project details
                if (pm.pm.ProjectType.Equals("External") && projectId > 0)
                {
                    var extParams = new Dictionary<string, object>
                    {
                        ["p_action"] = "insertExternalProject",
                        ["p_project_id"] = projectId,
                        ["p_departmentname"] = pm.pm.ProjectDepartment,
                        ["p_contactperson"] = pm.pm.ContactPerson,
                        ["p_address"] = pm.pm.Address
                    };
                    ExecuteProjectAction(extParams, tran);
                }

                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran?.Rollback();
                Console.WriteLine("❌ Error: " + ex.Message);
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd?.Dispose();
            }
        }


        private int ExecuteProjectAction(Dictionary<string, object> parameters, NpgsqlTransaction tran)
        {
            using (var cmd = new NpgsqlCommand(
                "CALL sp_adminaddproject(:p_action, :p_letterno, :p_id, :p_title, :p_assigndate, :p_startdate, :p_completiondate, :p_projectmanager, :p_subordinate, :p_budget, :p_description, :p_projectdocument, :p_projecttype, :p_stage, :p_projectcode, :p_approvestatus, :p_createdby, :p_status, :p_heads, :p_headsamount, :p_keypoint, :p_stagedocument, :p_departmentname, :p_contactperson, :p_address , :p_project_id)", con, tran))
            {
                cmd.CommandType = CommandType.Text;

                // Define all parameters with defaults
                var allParams = new List<string>
        {
            "p_action","p_letterno","p_id","p_title","p_assigndate","p_startdate","p_completiondate","p_projectmanager","p_subordinate","p_budget",
            "p_description","p_projectdocument","p_projecttype","p_stage","p_projectcode","p_approvestatus","p_createdby","p_status", "p_heads", "p_headsamount", "p_keypoint","p_stagedocument", "p_departmentname", "p_contactperson", "p_address", "p_project_id"
        };

                // Assign provided params or default DBNull
                foreach (var paramName in allParams)
                {
                    if (parameters.ContainsKey(paramName))
                        cmd.Parameters.AddWithValue(paramName, parameters[paramName] ?? (object)DBNull.Value);
                    else if (paramName == "p_project_id" || paramName == "ref")
                        cmd.Parameters.AddWithValue(paramName, 0).Direction = ParameterDirection.InputOutput;
                    else
                        cmd.Parameters.AddWithValue(paramName, DBNull.Value);
                }

                cmd.ExecuteNonQuery();

                // Return project id if available
                if (cmd.Parameters.Contains("p_project_id") && cmd.Parameters["p_project_id"].Value != DBNull.Value)
                    return Convert.ToInt32(cmd.Parameters["p_project_id"].Value);
                else
                    return 0;
            }
        }


        public List<Project_model> Project_List(int? page = null, int? limit = null)
        {
            try
            {
                List<Project_model> list = new List<Project_model>();
                cmd = new NpgsqlCommand("SELECT * FROM fn_get_all_projects(@action,@v_id,@v_projectManager,@v_filterType,@v_limit,@v_page)", con);
                cmd.Parameters.AddWithValue("@action", "GetAllProject");
                cmd.Parameters.AddWithValue("@v_projectManager", DBNull.Value);
                cmd.Parameters.AddWithValue("@v_filterType", DBNull.Value);
                cmd.Parameters.AddWithValue("@v_id", DBNull.Value);
                cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                con.Open();
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    bool firstRow = true;
                    while (rd.Read())
                    {
                        var project = new Project_model
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
                            StartDateString = Convert.ToDateTime(rd["startDate"]).ToString("dd-MM-yyyy"),
                            createdBy = rd["createdBy"].ToString(),
                            projectCode = rd["projectCode"] != DBNull.Value ? rd["projectCode"].ToString() : "N/A"
                        };
                        if (firstRow)
                        {
                            project.Pagination = new ApiCommon.PaginationInfo
                            {
                                PageNumber = page ?? 0,
                                TotalPages = Convert.ToInt32(rd["totalpages"] != DBNull.Value ? rd["totalpages"] : 0),
                                TotalRecords = Convert.ToInt32(rd["totalrecords"] != DBNull.Value ? rd["totalrecords"] : 0),
                                PageSize = limit ?? 0
                            };
                            firstRow = false; // Optional: ensure pagination is only assigned once
                        }

                        list.Add(project);
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
        public List<Project_model> getHeadByProject(int projectId,int? page = null,int? limit = null)
        {
            try
            {
                List<Project_model> _headList = new List<Project_model>();
                con.Open();
                Project_model obj = null;
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM fn_get_all_projects(@v_action,@v_id,@v_limit ,@v_page)", con);
                cmd.Parameters.AddWithValue("@v_action", "getHeadByProject");
                cmd.Parameters.AddWithValue("@v_id", projectId);
                cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                NpgsqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        obj = new Project_model();
                        obj.Id = Convert.ToInt32(sdr["id"]);
                        obj.heads = sdr["title"].ToString();
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

                cmd = new NpgsqlCommand("SELECT * FROM fn_get_all_projects(@action,@v_id,@v_limit,@v_page)", con);
                cmd.Parameters.AddWithValue("@action", "GetProjectById");
                cmd.Parameters.AddWithValue("@v_id", id);
                cmd.Parameters.AddWithValue("@v_limit",  DBNull.Value);
                cmd.Parameters.AddWithValue("@v_page",  DBNull.Value);
                con.Open();
                NpgsqlDataReader rd = cmd.ExecuteReader();
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
                        pm.ProjectManager = rd["name"].ToString();
                        pm.ProjectBudget = Convert.ToDecimal(rd["budget"]);
                        pm.ProjectDescription = rd["description"].ToString();
                        pm.projectDocumentUrl = rd["ProjectDocument"].ToString();
                        pm.ProjectType = rd["projectType"].ToString();
                        pm.ProjectStage = Convert.ToBoolean(rd["stage"]);
                        pm.ProjectStatus = Convert.ToBoolean(rd["CompleteStatus"]);
                        pm.projectCode = rd["projectCode"] != DBNull.Value ? rd["projectCode"].ToString() : "N/A";
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
            NpgsqlTransaction tran = null;
            NpgsqlCommand cmd = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();

                // Generate Project Code
                Random rand = new Random();
                pm.projectCode = $"{rand.Next(1000, 9999)}{DateTime.Now.Day}{DateTime.Now.Year.ToString().Substring(2, 2)}";

                // 1️⃣ Insert main project
                var projectParams = new Dictionary<string, object>
                {
                    ["p_action"] = pm.Id > 0 ? "updateProject" : "insertProject",
                    ["p_letterno"] = int.TryParse(pm.letterNo, out int letterNo) ? letterNo : 0,
                    ["p_title"] = pm.ProjectTitle,
                    ["p_assigndate"] = pm.AssignDate,
                    ["p_startdate"] = pm.StartDate,
                    ["p_completiondate"] = pm.CompletionDate,
                    ["p_projectmanager"] = int.TryParse(pm.ProjectManager, out int ProjectManager) ? ProjectManager : 0,
                    ["p_budget"] = pm.ProjectBudget,
                    ["p_description"] = pm.ProjectDescription,
                    ["p_projectdocument"] = pm.projectDocumentUrl,
                    ["p_projecttype"] = pm.ProjectType,
                    ["p_stage"] = pm.ProjectStage,
                    ["p_createdby"] = "admin",
                    ["p_status"] = true,
                    ["p_approvestatus"] = true,
                    ["p_projectcode"] = pm.projectCode
                };

                int projectId = ExecuteProjectAction(projectParams, tran);

                if (pm.SubOrdinate != null && pm.SubOrdinate.Length > 0)
                {
                    foreach (var subId in pm.SubOrdinate)
                    {
                        var subParams = new Dictionary<string, object>
                        {
                            ["p_action"] =  "insertSubOrdinate",
                            ["p_project_id"] = projectId,
                            ["p_id"] = subId,
                            ["p_projectmanager"] = int.TryParse(pm.ProjectManager, out int SubProjectManager) ? SubProjectManager : 0
                        };
                        ExecuteProjectAction(subParams, tran);
                    }
                }

                // 5️⃣ Insert External project details
                if (pm.ProjectType.Equals("External") && projectId > 0)
                {
                    var extParams = new Dictionary<string, object>
                    {
                        ["p_action"] = pm.Id > 0 ? "updateExternalProject" : "insertExternalProject",
                        ["p_project_id"] = projectId,
                        ["p_departmentname"] = pm.ProjectDepartment,
                        ["p_contactperson"] = pm.ContactPerson,
                        ["p_address"] = pm.Address
                    };
                    ExecuteProjectAction(extParams, tran);
                }
                tran.Commit();
                return true;
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

        public bool insertProjectStages(Project_Statge stg)
        {
            NpgsqlTransaction tran = null;
            NpgsqlCommand cmd = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                        var stageParams = new Dictionary<string, object>
                        {
                            ["p_action"] = "insertProjectStage",
                            ["p_project_id"] = stg.Project_Id,
                            ["p_keypoint"] = stg.KeyPoint,
                            ["p_completiondate"] = stg.CompletionDate,
                            ["p_stagedocument"] = stg.Document_Url
                        };
                        ExecuteProjectAction(stageParams, tran);
                tran.Commit();
                return true;

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

        public bool insertProjectBudgets(Project_Budget bdg)
        {
            NpgsqlTransaction tran = null;
            NpgsqlCommand cmd = null;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                var budgetParams = new Dictionary<string, object>
                {
                    ["p_action"] = bdg.Id > 0 ? "updateProjectBudget" : "insertProjectBudget",
                    ["p_project_id"] = bdg.Project_Id,
                    ["p_heads"] = bdg.ProjectHeads,
                    ["p_headsamount"] = bdg.ProjectAmount
                };
                ExecuteProjectAction(budgetParams, tran);
                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                cmd.Dispose();
            }
        }

        public List<Project_Budget> ProjectBudgetList(int Id)
        {
            try
            {
                List<Project_Budget> list = new List<Project_Budget>();
                cmd = new NpgsqlCommand("SELECT * FROM fn_getProjectStagesAndBudget(@action,@v_id,@v_limit,@v_page)", con);
                cmd.Parameters.AddWithValue("@action", "GetBudgetByProjectId");
                cmd.Parameters.AddWithValue("@v_id", Id);
                cmd.Parameters.AddWithValue("@v_limit", DBNull.Value);
                cmd.Parameters.AddWithValue("@v_page", DBNull.Value);
                if (con.State == ConnectionState.Closed)
                    con.Open();
                NpgsqlDataReader rd = cmd.ExecuteReader();
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

        public List<Project_Statge> ProjectStagesList(int Id)
        {
            try
            {
                List<Project_Statge> list = new List<Project_Statge>();
                cmd = new NpgsqlCommand("SELECT * FROM fn_getProjectStagesAndBudget(@action,@v_id,@v_limit,@v_page)", con);
                cmd.Parameters.AddWithValue("@action", "GetProjectStageByProjectId");
                cmd.Parameters.AddWithValue("@v_id", Id);
                cmd.Parameters.AddWithValue("@v_limit", DBNull.Value);
                cmd.Parameters.AddWithValue("@v_page", DBNull.Value);
                if (con.State == ConnectionState.Closed)
                    con.Open();
                NpgsqlDataReader rd = cmd.ExecuteReader();
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
        #endregion

        #region /* Admin Dashboard Count */
        public DashboardCount DashboardCount()
        {
            DashboardCount obj = null;
            try
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_managedashboard_cursor", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "AdminDashboardCount");
                    cmd.Parameters.AddWithValue("v_projectmanager", 0);
                    cmd.Parameters.AddWithValue("v_sid", 0);
                    // Execute the function — it returns the cursor name
                    string cursorName = (string)cmd.ExecuteScalar();
                    // Now fetch the data from the cursor
                    using (var fetchCmd = new NpgsqlCommand($"FETCH ALL FROM \"{cursorName}\";", con, tran))
                    using (var sdr = fetchCmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            sdr.Read();
                            obj = new DashboardCount
                            {
                                TotalEmployee = sdr["TotalEmployee"].ToString(),
                                TotalProjectManager = sdr["TotalProjectManager"].ToString(),
                                TotalSubOrdinate = sdr["TotalSubOrdinate"].ToString(),
                                TotalAccounts = sdr["TotalAccounts"].ToString(),
                                TotalProject = sdr["TotalProject"].ToString(),
                                TotalInternalProject = sdr["TotalInternalProject"].ToString(),
                                TotalExternalProject = sdr["TotalExternalProject"].ToString(),
                                TotalDelayproject = sdr["TotalDelayproject"].ToString(),
                                TotalCompleteProject = sdr["TotalCompleteProject"].ToString(),
                                TotalOngoingProject = sdr["TotalOngoingProject"].ToString(),
                                TotalMeetings = sdr["TotalMeetings"].ToString(),
                                TotalAdminMeetings = sdr["TotalAdminMeetings"].ToString(),
                                TotalProjectManagerMeetings = sdr["TotalProjectManagerMeetings"].ToString(),
                                TotalReinbursementReq = sdr["TotalReinbursementReq"].ToString(),
                                TotalTourProposalReq = sdr["TotalTourProposalReq"].ToString(),
                                totalVehicleHiringRequest = sdr["totalVehicleHiringRequest"].ToString(),
                                totalReinbursementPendingRequest = sdr["totalReinbursementPendingRequest"].ToString(),
                                totalReinbursementapprovedRequest = sdr["totalReinbursementapprovedRequest"].ToString(),
                                totalReinbursementRejectRequest = sdr["totalReinbursementRejectRequest"].ToString(),
                                totalTourProposalApprReque = sdr["totalTourProposalApprReque"].ToString(),
                                totalTourProposalRejectReque = sdr["totalTourProposalRejectReque"].ToString(),
                                totaTourProposalPendingReque = sdr["totalTourProposalRejectReque"].ToString(),
                                totalPendingHiringVehicle = sdr["totalPendingHiringVehicle"].ToString(),
                                totalApproveHiringVehicle = sdr["totalApproveHiringVehicle"].ToString(),
                                totalRejectHiringVehicle = sdr["totalRejectHiringVehicle"].ToString(),
                                TotalBudget = Convert.ToDecimal(sdr["totalBudgets"] != DBNull.Value ? sdr["totalBudgets"] : 0),
                                PendingBudget = Convert.ToDecimal(sdr["pendingBudget"] != DBNull.Value ? sdr["pendingBudget"] : 0),
                                //expenditure = Convert.ToDecimal(sdr["AppStatus"] != DBNull.Value ? sdr["AppStatus"] : 0)
                            };
                        }
                    }

                    // Close the cursor explicitly
                    using (var closeCmd = new NpgsqlCommand($"CLOSE \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }

                    tran.Commit();
                }

                return obj;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching dashboard data", ex);
            }
            finally
            {
                con.Close();
            }
        }


        public List<ProjectExpenditure> ViewProjectExpenditure()
        {
            try
            {
                List<ProjectExpenditure> list = new List<ProjectExpenditure>();
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_managedashboard_cursor", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "viewProjectExpenditure");
                    cmd.Parameters.AddWithValue("v_projectmanager", DBNull.Value);
                    cmd.Parameters.AddWithValue("v_sid", 0);

                    string cursorName = (string)cmd.ExecuteScalar();

                    // Now fetch the data from the cursor
                    using (var fetchCmd = new NpgsqlCommand($"FETCH ALL FROM \"{cursorName}\";", con, tran))
                    using (var rd = fetchCmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                list.Add(new ProjectExpenditure
                                {
                                    id = Convert.ToInt32(rd["id"]),
                                    ProjectName = rd["title"].ToString(),
                                    projectmanager = rd["name"].ToString(),
                                    ProjectBudget = Convert.ToDecimal(rd["budget"]),
                                    expenditure = rd["ExpendedAmt"] != DBNull.Value ? Convert.ToDecimal(rd["ExpendedAmt"]) : 0,
                                    remaining = rd["remainingAmt"] != DBNull.Value ? Convert.ToDecimal(rd["remainingAmt"]) : 0
                                });
                            }
                        }
                        return list;
                    }

                    // Close the cursor explicitly
                    using (var closeCmd = new NpgsqlCommand($"CLOSE \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }

                    tran.Commit();
                }
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
        public List<DashboardCount> getAllProjectCompletion()
        {
            try
            {
                List<DashboardCount> list = new List<DashboardCount>();
                DashboardCount obj = null;
                NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageDashboard", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getOverallProjectCompletion");
                con.Open();
                NpgsqlDataReader sdr = cmd.ExecuteReader();
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
                con.Open();
                using (var cmd = new NpgsqlCommand("SELECT * from fn_get_employees(@v_action, @v_id);", con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@v_action", "SelectEmployees");
                    cmd.Parameters.AddWithValue("@v_id", DBNull.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            empObj = new Employee_model();
                            empObj.Id = Convert.ToInt32(reader["id"]);
                            empObj.EmployeeName = reader["name"].ToString();
                            empObj.EmployeeRole = reader["role"].ToString();

                            empList.Add(empObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching meeting members: " + ex.Message, ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return empList;
        }

        public bool insertMeeting(AddMeeting_Model obj)
        {
            con.Open();
            using (var transaction = con.BeginTransaction())
            {
                try
                {
                    int meetingId = 0;

                    using (var cmd = new NpgsqlCommand("CALL public.sp_managemeeting(@p_id, @p_meetingtype, @p_meetinglink, @p_meetingtitle, @p_meetingtime, @p_meetingdocument, @p_createdby, NULL, @p_createrid, NULL, 0, 0, 0, NULL, NULL, @p_meetingid, 0, @p_action)", con, transaction))
                    {
                        cmd.CommandType = CommandType.Text;

                        // --- Insert or Update Meeting ---
                        cmd.Parameters.AddWithValue("p_id", obj.Id);
                        cmd.Parameters.AddWithValue("p_meetingtype", obj.MeetingType ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("p_meetinglink",
                            obj.MeetingType?.ToLower() == "offline" ?
                            (object)obj.MeetingAddress ?? DBNull.Value :
                            (object)obj.MeetingLink ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("p_meetingtitle", obj.MeetingTitle ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("p_meetingtime", obj.MeetingTime.ToString() ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("p_meetingdocument", obj.Attachment_Url ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("p_createdby", obj.CreaterId > 0 ? "projectmanager" : "admin");
                        cmd.Parameters.AddWithValue("p_createrid", obj.CreaterId > 0 ? obj.CreaterId : 0);
                        cmd.Parameters.AddWithValue("p_action", obj.Id > 0 ? "updateMeeting" : "insertMeeting");

                        // INOUT parameter for meeting id
                        var meetingIdParam = new NpgsqlParameter("p_meetingid", NpgsqlTypes.NpgsqlDbType.Integer)
                        {
                            Direction = ParameterDirection.InputOutput,
                            Value = obj.Id > 0 ? obj.Id : 0
                        };
                        cmd.Parameters.Add(meetingIdParam);

                        cmd.ExecuteNonQuery();
                        meetingId = Convert.ToInt32(meetingIdParam.Value);
                    }

                    // --- Add Members ---
                    if (obj.meetingMemberList != null && obj.meetingMemberList.Any())
                    {
                        foreach (var member in obj.meetingMemberList)
                        {
                            if (member == 0) continue;

                            using (var cmd = new NpgsqlCommand("CALL public.sp_managemeeting(p_employee => @p_employee, p_meeting => @p_meeting, p_action => @p_action)", con, transaction))
                            {
                                cmd.CommandType = CommandType.Text;

                                cmd.Parameters.AddWithValue("@p_employee", NpgsqlTypes.NpgsqlDbType.Integer, member);
                                cmd.Parameters.AddWithValue("@p_meeting", NpgsqlTypes.NpgsqlDbType.Integer, meetingId);
                                cmd.Parameters.AddWithValue("@p_action", NpgsqlTypes.NpgsqlDbType.Varchar, "addMeetingMember");

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    // --- Add Key Points ---
                    if (obj.keyPointList != null && obj.keyPointList.Any())
                    if (obj.keyPointList != null && obj.keyPointList.Count > 0)      
                    {
                        foreach (var key in obj.keyPointList)
                        {
                            if (string.IsNullOrWhiteSpace(key)) continue;

                            using (var cmd = new NpgsqlCommand("CALL public.sp_managemeeting(p_keypoint => @p_keypoint, p_meeting => @p_meeting, p_action => @p_action)", con, transaction))
                            {
                                cmd.CommandType = CommandType.Text;

                                cmd.Parameters.AddWithValue("@p_keypoint", key);
                                cmd.Parameters.AddWithValue("@p_meeting", meetingId);
                                cmd.Parameters.AddWithValue("@p_action", "AddMeetingKeyPoint");

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error while inserting/updating meeting", ex);
                }
            }
        }

        public bool UpdateMeeting(AddMeeting_Model obj)
        {
            con.Open();
            using (var transaction = con.BeginTransaction())
            {
                try
                {
                    // === Update Main Meeting ===
                    using (var cmd = new NpgsqlCommand(@"
                    CALL public.sp_managemeeting(
                        p_id => @p_id,
                        p_meetingtype => @p_meetingtype,
                        p_meetinglink => @p_meetinglink,
                        p_meetingtitle => @p_meetingtitle,
                        p_meetingtime => @p_meetingtime,
                        p_meetingdocument => @p_meetingdocument,
                        p_createdby => NULL,
                        p_updateddate => NULL,
                        p_createrid => NULL,
                        p_conclusionid => NULL,
                        p_employee => 0,
                        p_meeting => 0,
                        p_appstatus => 0,
                        p_keypoint => NULL,
                        p_reason => NULL,
                        p_meetingid => NULL,
                        p_status => 0,
                        p_action => @p_action
                    );", con, transaction))
                    {
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.AddWithValue("@p_id", NpgsqlTypes.NpgsqlDbType.Integer, obj.Id);
                        cmd.Parameters.AddWithValue("@p_meetingtype", NpgsqlTypes.NpgsqlDbType.Varchar, obj.MeetingType ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_meetinglink", NpgsqlTypes.NpgsqlDbType.Text,
                            obj.MeetingType?.ToLower() == "offline" ? obj.MeetingAddress ?? "" : obj.MeetingLink ?? "");
                        cmd.Parameters.AddWithValue("@p_meetingtitle", NpgsqlTypes.NpgsqlDbType.Varchar, obj.MeetingTitle ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_meetingtime", NpgsqlTypes.NpgsqlDbType.Varchar, obj.MeetingTime.ToString() ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_meetingdocument", NpgsqlTypes.NpgsqlDbType.Text, (object)obj.Attachment_Url ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@p_action", NpgsqlTypes.NpgsqlDbType.Varchar, "updateMeeting");

                        cmd.ExecuteNonQuery();
                    }

                    // === Update Members ===
                    if (obj.meetingMemberList != null && obj.meetingMemberList.Count > 0)
                    {
                        foreach (var memberId in obj.meetingMemberList)
                        {
                            if (memberId != 0)
                            {
                                using (var cmdMember = new NpgsqlCommand(@"
                                CALL public.sp_managemeeting(
                                    p_employee => @p_employee,
                                    p_meeting => @p_meeting,
                                    p_id => 0,
                                    p_meetingtype => NULL,
                                    p_meetinglink => NULL,
                                    p_meetingtitle => NULL,
                                    p_meetingtime => NULL,
                                    p_meetingdocument => NULL,
                                    p_createdby => NULL,
                                    p_updateddate => NULL,
                                    p_createrid => NULL,
                                    p_conclusionid => NULL,
                                    p_appstatus => 0,
                                    p_keypoint => NULL,
                                    p_reason => NULL,
                                    p_meetingid => 0,
                                    p_status => 0,
                                    p_action => @p_action
                                );", con, transaction))
                                {
                                    cmdMember.CommandType = CommandType.Text;
                                    cmdMember.Parameters.AddWithValue("@p_employee", NpgsqlTypes.NpgsqlDbType.Integer, memberId);
                                    cmdMember.Parameters.AddWithValue("@p_meeting", NpgsqlTypes.NpgsqlDbType.Integer, obj.Id);
                                    cmdMember.Parameters.AddWithValue("@p_action", NpgsqlTypes.NpgsqlDbType.Varchar, "updateMember");
                                    cmdMember.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    // === Update Keypoints ===
                    if (obj.KeypointId != null && obj.keyPointList != null)
                    {
                        for (int j = 0; j < obj.KeypointId.Count; j++)
                        {
                            if (!string.IsNullOrWhiteSpace(obj.KeypointId[j]?.ToString()) &&
                                !string.IsNullOrWhiteSpace(obj.keyPointList[j]?.ToString()))
                            {
                                using (var cmdKey = new NpgsqlCommand(@"
                                CALL public.sp_managemeeting(
                                    p_meeting => @p_meeting,
                                    p_keypoint => @p_keypoint,
                                    p_action => @p_action,
                                    p_id => @p_id
                                );", con, transaction))
                                {
                                    cmdKey.CommandType = CommandType.Text;
                                    cmdKey.Parameters.AddWithValue("@p_meeting", NpgsqlTypes.NpgsqlDbType.Integer, obj.Id);
                                    cmdKey.Parameters.AddWithValue("@p_id", NpgsqlTypes.NpgsqlDbType.Integer, Convert.ToInt32(obj.KeypointId[j]));
                                    cmdKey.Parameters.AddWithValue("@p_keypoint", NpgsqlTypes.NpgsqlDbType.Varchar, obj.keyPointList[j]);
                                    cmdKey.Parameters.AddWithValue("@p_action", NpgsqlTypes.NpgsqlDbType.Varchar, "updateKeyPoint");
                                    cmdKey.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("An error occurred while updating the meeting.", ex);
                }
            }
        }

        public List<Meeting_Model> getAllmeeting()
        {
            try
            {
                con.Open();
                List<Meeting_Model> _list = new List<Meeting_Model>();
                Meeting_Model obj = null;
                using (var cmd = new NpgsqlCommand("SELECT * from fn_get_meetings(@p_action);", con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@p_action", "getAllmeeting");

                    using (var sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            obj = new Meeting_Model();
                            obj.Id = Convert.ToInt32(sdr["id"]);
                            obj.CompleteStatus = Convert.ToInt32(sdr["completeStatus"]);
                            obj.MeetingType = sdr["meetingType"].ToString();
                            obj.MeetingLink = sdr["meetingLink"].ToString();
                            obj.MeetingTitle = sdr["MeetingTitle"].ToString();
                            obj.memberId = sdr["memberId"] != DBNull.Value ? sdr["memberId"].ToString().Split(',').ToList() : new List<string>();
                            obj.CreaterId = sdr["createrId"] != DBNull.Value ? Convert.ToInt32(sdr["createrId"]) : 0;
                            obj.MeetingDate = Convert.ToDateTime(sdr["meetingTime"]).ToString("dd-MM-yyyy");
                            obj.summary = sdr["meetSummary"].ToString();
                            _list.Add(obj);
                        }
                    }
                }
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
                using (var cmd = new NpgsqlCommand("SELECT * from fn_get_meetings(@p_action,@p_id);", con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@p_action", "getMeetingById");
                    cmd.Parameters.AddWithValue("@p_id", id);

                    using (var sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            obj.Id = Convert.ToInt32(sdr["id"]);
                            obj.MeetingType = sdr["meetingType"].ToString();
                            obj.MeetingLink = sdr["meetingLink"].ToString();
                            obj.MeetingTitle = sdr["meetingTitle"].ToString();
                            obj.MeetingTime = Convert.ToDateTime(sdr["meetingTime"]);
                            obj.MeetingTimeString = Convert.ToDateTime(sdr["meetingTime"]).ToString("yyyy-MM-ddTHH:mm");
                            obj.memberId = sdr["empId"] != DBNull.Value ? sdr["empId"].ToString().Split(',').ToList() : new List<string>();

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
            try
            {

                con.Open();
                List<Employee_model> empModel = new List<Employee_model>();
                cmd.Parameters.Clear();
                using (var cmd = new NpgsqlCommand("SELECT * from fn_get_meetings(@p_action,@p_id);", con)) { 
                    cmd.Parameters.AddWithValue("@p_id", id);
                    cmd.Parameters.AddWithValue("@p_action", "getMeetingMemberById");
                    cmd.CommandType = CommandType.Text;
                    using(var res = cmd.ExecuteReader())
                    {
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
            var transaction = con.BeginTransaction();
            try
            {
                // Step 1: Insert Conclusion
                int conclusionId = 0; // INOUT parameter

                using (var cmd = new NpgsqlCommand(@"
            CALL public.sp_meetingconslusion(
                @v_id::integer,
                @v_meeting::integer,
                @v_conclusion::text,
                @v_nextfollow::varchar,
                @v_memberid::integer,
                @v_response::text,
                @v_followupstatus::boolean,
                @v_keyid::integer,
                @v_conclusionid::integer,
                @v_summary::text,
                @v_action::varchar,
                @v_rc::refcursor
            )", con, transaction))
                {
                    cmd.CommandType = CommandType.Text;

                    // Procedure parameters
                    cmd.Parameters.AddWithValue("v_id", 0);
                    cmd.Parameters.AddWithValue("v_meeting", Convert.ToInt32(mc.Meeting));
                    cmd.Parameters.AddWithValue("v_conclusion", mc.Conclusion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("v_nextfollow", mc.NextFollowUpDate?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("v_memberid", 0);
                    cmd.Parameters.AddWithValue("v_response", DBNull.Value);
                    cmd.Parameters.AddWithValue("v_followupstatus", NpgsqlTypes.NpgsqlDbType.Boolean,mc.FollowUpStatus ? true : false);
                    cmd.Parameters.AddWithValue("v_keyid", 0);

                    var conclusionParam = new NpgsqlParameter("v_conclusionid", NpgsqlTypes.NpgsqlDbType.Integer)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = conclusionId
                    };
                    cmd.Parameters.Add(conclusionParam);

                    cmd.Parameters.AddWithValue("v_summary", mc.summary ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("v_action", "insertConclusion");
                    cmd.Parameters.AddWithValue("v_rc", DBNull.Value);

                    cmd.ExecuteNonQuery();

                    conclusionId = (int)conclusionParam.Value;
                }

                // Step 2: Update Member Presence
                if (mc.MemberId != null && mc.MemberId.Count > 0)
                {
                    foreach (var memberId in mc.MemberId)
                    {
                        if (!string.IsNullOrEmpty(memberId))
                        {
                            var memberCmd = new NpgsqlCommand(@"
                        CALL public.sp_meetingconslusion(
                            @v_id::integer,
                            0::integer,
                            NULL::text,
                            NULL::varchar,
                            @v_memberid::integer,
                            NULL::text,
                            false::boolean,
                            0::integer,
                            @v_conclusionid::integer,
                            NULL::text,
                            @v_action::varchar,
                            NULL::refcursor
                        )", con, transaction);
                            memberCmd.CommandType = CommandType.Text;

                            memberCmd.Parameters.AddWithValue("v_id", conclusionId);
                            memberCmd.Parameters.AddWithValue("v_memberid", Convert.ToInt32(memberId));
                            memberCmd.Parameters.AddWithValue("v_conclusionid", conclusionId);
                            memberCmd.Parameters.AddWithValue("v_action", "updateMeetingMemberPresence");

                            memberCmd.ExecuteNonQuery();
                        }
                    }
                }

                // Step 3: Insert Keypoint Responses
                if (mc.KeyPointId != null && mc.KeyPointId.Count > 0)
                {
                    for (int i = 0; i < mc.KeyPointId.Count; i++)
                    {
                        var keyCmd = new NpgsqlCommand(@"
                    CALL public.sp_meetingconslusion(
                        @v_id::integer,
                        0::integer,
                        NULL::text,
                        NULL::varchar,
                        0::integer,
                        @v_response::text,
                        false::boolean,
                        @v_keyid::integer,
                        @v_conclusionid::integer,
                        NULL::text,
                        @v_action::varchar,
                        NULL::refcursor
                    )", con, transaction);
                        keyCmd.CommandType = CommandType.Text;

                        keyCmd.Parameters.AddWithValue("v_id", conclusionId);
                        keyCmd.Parameters.AddWithValue("v_keyid", mc.KeyPointId[i]);
                        keyCmd.Parameters.AddWithValue("v_response", mc.KeyResponse[i] ?? (object)DBNull.Value);
                        keyCmd.Parameters.AddWithValue("v_conclusionid", conclusionId);
                        keyCmd.Parameters.AddWithValue("v_action", "isertKeypointResponse");

                        keyCmd.ExecuteNonQuery();
                    }
                }

                // Step 4: Add Meeting Members if FollowUpStatus is true
                if (mc.MeetingMemberList != null && mc.FollowUpStatus)
                {
                    foreach (var individualMember in mc.MeetingMemberList)
                    {
                        if (individualMember != 0)
                        {
                            var memberCmd = new NpgsqlCommand(@"
                        CALL sp_ManageMeeting(
                            @v_action::varchar,
                            @v_employee::integer,
                            @v_meeting::integer
                        )", con, transaction);
                            memberCmd.CommandType = CommandType.Text;

                            memberCmd.Parameters.AddWithValue("v_action", "addMeetingMember");
                            memberCmd.Parameters.AddWithValue("v_employee", individualMember);
                            memberCmd.Parameters.AddWithValue("v_meeting", mc.Meeting);

                            memberCmd.ExecuteNonQuery();
                        }
                    }
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("An error occurred while adding meeting response.", ex);
            }
            finally
            {
                con.Close();
            }
        }
        public List<MeetingConclusion> getConclusion(int id)
        {
            try
            {
                List<MeetingConclusion> meetingc = new List<MeetingConclusion>();
                con.Open();
                cmd.Parameters.Clear();
                using(var cmd= new NpgsqlCommand("select * from fn_get_meetings(@p_action,@p_id);", con))
                {
                    cmd.Parameters.AddWithValue("@p_action", "selectConclusion");
                    cmd.Parameters.AddWithValue("@p_id", id);
                    cmd.CommandType = CommandType.Text;
                    using(var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                meetingc.Add(new MeetingConclusion
                                {
                                    Id = Convert.ToInt32(rdr["id"]),
                                    Meeting = id,
                                    MeetingDate = rdr["meetingTime"] != DBNull.Value ? Convert.ToDateTime(rdr["meetingTime"]).ToString("dd-MM-yyyy hh:mm tt") : "N/A",
                                    Conclusion = rdr["conclusion"].ToString(),
                                    NextFollow = rdr["nextFollow"] != DBNull.Value ? Convert.ToDateTime(rdr["nextFollow"]).ToString("dd-MM-yyyy") : "N/A",
                                    mode = rdr["meetingType"].ToString(),
                                    address = rdr["meetingLink"].ToString()
                                });
                            }
                        }
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
                con.Open();
                List<Employee_model> meetingc = new List<Employee_model>();
                cmd.Parameters.Clear();
                using (var cmd = new NpgsqlCommand(@"select * from fn_get_meetings(@p_action,@p_id);", con))
                {
                    cmd.Parameters.AddWithValue("@p_action", "selectPresentMember");
                    cmd.Parameters.AddWithValue("@p_id", id);
                    cmd.CommandType = CommandType.Text;

                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                meetingc.Add(new Employee_model
                                {
                                    EmployeeName = rdr["name"].ToString(),
                                    Image_url = rdr["profile"].ToString(),
                                    EmployeeRole = rdr["role"].ToString(),
                                    PresentStatus = (bool)rdr["completeStatus"]

                                });
                            }
                        }
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
                List<KeyPointResponse> md = new List<KeyPointResponse>();
                con.Open();
                using (var cmd = new NpgsqlCommand(@"select * from fn_get_meetings(@p_action,@p_id)", con))
                {
                    cmd.Parameters.AddWithValue("@p_action", "KeyPointResponse");
                    cmd.Parameters.AddWithValue("@p_id", id);
                    cmd.CommandType = CommandType.Text;
                    using (var rdr = cmd.ExecuteReader())
                    {
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
                cmd = new NpgsqlCommand("CALL sp_managenotice(@v_id, @v_projectid, @v_noticedocs, @v_noticedesc, NULL, @v_action)", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@v_action", gn.Id > 0 ? "UpdateNotice" : "InsertNotice");
                cmd.Parameters.AddWithValue("@v_projectId", gn.ProjectId);
                cmd.Parameters.AddWithValue("v_id", gn.Id);
                cmd.Parameters.AddWithValue("v_noticeDocs", gn.Attachment_Url ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("v_noticedesc", gn.Notice);
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
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

        public List<Generate_Notice> getNoticeList(int? limit = null,int? page = null, int?id = null, int?managerId = null)
        {
            try
            {
                con.Open();
                List<Generate_Notice> noticeList = new List<Generate_Notice>();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageNotice_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@v_action", "SelectNotice");
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_projectmanager", managerId.HasValue ? (object)managerId.Value : 0);
                    cmd.Parameters.AddWithValue("@v_id", id.HasValue ? (object)id.Value : DBNull.Value);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var res = fetchCmd.ExecuteReader())
                    {
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
                    }
                    using(var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
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
        //        cmd = new NpgsqlCommand("sp_manageBudgets", con);
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
        //        cmd = new NpgsqlCommand("sp_manageBudgets", con);
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
        public List<tourProposalAll> getAllTourList(int?page, int?limit)
        {
            try
            {
                List<tourProposalAll> getlist = new List<tourProposalAll>();
                con.Open();
                using(var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageTourProposal_cursor", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "selectAlltour");
                    cmd.Parameters.AddWithValue("v_projectmanager", DBNull.Value);
                    cmd.Parameters.AddWithValue("v_id", 0);
                    cmd.Parameters.AddWithValue("v_limit", page);
                    cmd.Parameters.AddWithValue("v_page", limit);
                    // Execute the function — it returns the cursor name
                    string cursorName = (string)cmd.ExecuteScalar();

                    // Now fetch the data from the cursor
                    using (var fetchCmd = new NpgsqlCommand($"FETCH ALL FROM \"{cursorName}\";", con, tran))
                    using (var res = fetchCmd.ExecuteReader())
                    {
                        if (res.HasRows)
                        {
                            bool firstRow = true;
                            while (res.Read())
                            {
                                var tourData = new tourProposalAll
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
                                    projectCode = res["projectCode"] != DBNull.Value ? res["projectCode"].ToString() : "N/A"
                                };
                                if (firstRow)
                                {
                                    tourData.Pagination = new ApiCommon.PaginationInfo
                                    {
                                        TotalPages = Convert.ToInt32(res["totalpages"]),
                                        TotalRecords = Convert.ToInt32(res["totalrecords"]),
                                        PageNumber = page ?? 0,
                                        PageSize = limit ?? 0
                                    };
                                    firstRow = false;
                                }
                            }
                        }
                    }
                    // Close the cursor explicitly
                    using (var closeCmd = new NpgsqlCommand($"CLOSE \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
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

        public bool Tourapproval(int id, bool status, string remark)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_Tourproposal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "approval");
                cmd.Parameters.AddWithValue("@adminappr", status);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@remark", status ? "" : remark);
                con.Open();
                int res = cmd.ExecuteNonQuery();
                return res > 0;
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
        public bool ReimbursementApproval(bool status, int id, string type, string remark)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_Reimbursement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "approval");
                cmd.Parameters.AddWithValue("@admin_appr", status);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@remark", status ? "" : remark);
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
                cmd = new NpgsqlCommand("sp_Reimbursement", con);
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
        public List<HiringVehicle1> HiringList(int?page, int?limit)
        {
            try
            {
                con.Open();
                List<HiringVehicle1> list = new List<HiringVehicle1>();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageHiringVehicle_cursor", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "selectAllHiring");
                    cmd.Parameters.AddWithValue("v_projectmanager", DBNull.Value);
                    cmd.Parameters.AddWithValue("v_id", 0);
                    cmd.Parameters.AddWithValue("v_limit", page);
                    cmd.Parameters.AddWithValue("v_page", limit);

                    string cursorName = (string)cmd.ExecuteScalar();

                    using (var fetchCmd = new NpgsqlCommand($"FETCH ALL FROM \"{cursorName}\";", con, tran))

                    using (var res = fetchCmd.ExecuteReader())
                    {
                        if (res.HasRows)
                        {
                            bool firstRow = true;
                            while (res.Read())
                            {
                                var hiringVehicle = new HiringVehicle1
                                {
                                    id = (int)res["id"],
                                    projectName = Convert.ToString(res["title"]),
                                    projectManager = Convert.ToString(res["name"]),
                                    headName = Convert.ToString(res["heads"]),
                                    amount = Convert.ToDecimal(res["amount"]),
                                    dateFrom = Convert.ToDateTime(res["dateFrom"]),
                                    dateTo = Convert.ToDateTime(res["dateTo"]),
                                    proposedPlace = res["proposedPlace"].ToString(),
                                    purposeOfVisit = res["purposeOfVisit"].ToString(),
                                    totalDaysNight = res["totalDaysNight"].ToString(),
                                    totalPlainHills = res["totalPlainHills"].ToString(),
                                    taxi = res["taxi"].ToString(),
                                    BookAgainstCentre = res["BookAgainstCentre"].ToString(),
                                    availbilityOfFund = res["availbilityOfFund"].ToString(),
                                    note = res["note"].ToString(),
                                    newRequest = Convert.ToBoolean(res["newRequest"]),
                                    adminappr = Convert.ToBoolean(res["adminappr"]),
                                    projectCode = res["projectCode"] != DBNull.Value ? res["projectCode"].ToString() : "N/A"
                                };
                                if (firstRow)
                                {
                                    hiringVehicle.Pagination = new ApiCommon.PaginationInfo
                                    {
                                        TotalPages = Convert.ToInt32(res["totalpages"]),
                                        TotalRecords = Convert.ToInt32(res["totalrecords"]),
                                        PageNumber = page ?? 0,
                                        PageSize = limit ?? 0
                                    };
                                    firstRow = false;
                                }

                                list.Add(hiringVehicle);
                            }
                        }
                    }
                    // Close the cursor explicitly
                    using (var closeCmd = new NpgsqlCommand($"CLOSE \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
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
        //public async Task<(string latitude, string longitude)> GetLatLongAsync()
        //{
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            string url = "https://ipinfo.io/json";
        //            HttpResponseMessage response = await client.GetAsync(url);
        //            response.EnsureSuccessStatusCode();

        //            string responseBody = await response.Content.ReadAsStringAsync();
        //            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);

        //            string loc = jsonData.loc; 
        //            string[] coordinates = loc.Split(',');

        //            string latitude = coordinates[0];
        //            string longitude = coordinates[1];

        //            return (latitude, longitude);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public async Task<string> GetCityStateAsync(string latitude, string longitude)
        {
            try
            {
                string url = $"https://nominatim.openstreetmap.org/reverse?lat={latitude}&lon={longitude}&format=json";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
                    client.DefaultRequestHeaders.Add("Accept-Language", "en");

                    var response = await client.GetStringAsync(url).ConfigureAwait(false);
                    JObject json = JObject.Parse(response);

                    string city = json["address"]?["city"]?.ToString()
                                ?? json["address"]?["town"]?.ToString()
                                ?? json["address"]?["village"]?.ToString()
                                ?? json["address"]?["hamlet"]?.ToString()
                                ?? "";

                    string state = json["address"]?["state"]?.ToString() ?? "";

                    return $"{city}, {state}".Trim(new char[] { ',', ' ' });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public bool HiringApproval(int id, bool status, string remark, dynamic location)
        {
            try
            {
                //var location = getlocationasync();
                cmd = new NpgsqlCommand("sp_HiringVehicle", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "approval");
                cmd.Parameters.AddWithValue("@adminappr", status);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@remark", status ? "" : remark);
                cmd.Parameters.AddWithValue("@location", location);
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

        #region Graph Data
        public List<BudgetForGraph> BudgetForGraph()
        {
            try
            {
                cmd = new NpgsqlCommand("sp_ManageDashboard", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "BudgetGraph");
                List<BudgetForGraph> list = new List<BudgetForGraph>();
                con.Open();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        list.Add(new BudgetForGraph
                        {
                            budget = Convert.ToDecimal(res["budget"]),
                            month = Convert.ToString(res["months"])
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

        //public List<BudgetForGraph> PhysicalForGraph()
        //{
        //    try
        //    {
        //        cmd = new NpgsqlCommand("sp_ManageDashboard", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@action", "BudgetGraph");
        //        List<BudgetForGraph> list = new List<BudgetForGraph>();
        //        con.Open();
        //        var res = cmd.ExecuteReader();
        //        if (res.HasRows)
        //        {
        //            while (res.Read())
        //            {
        //                list.Add(new BudgetForGraph
        //                {
        //                    budget = Convert.ToDecimal(res["budget"]),
        //                    month = Convert.ToString(res["months"])
        //                });
        //            }
        //        }
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (con.State == ConnectionState.Open)
        //        {
        //            con.Close();
        //        }
        //        cmd.Dispose();
        //    }
        //}
        #endregion

        #region All Reports
        public List<HiringVehicle1> HiringReort(int? limit = null, int? page = null)
        {
            try
            {
                con.Open();
                List<HiringVehicle1> list = new List<HiringVehicle1>();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_managehiringvehicle_cursor", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@v_action", "selectAllHiringReport");
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var res = fetchCmd.ExecuteReader())
                    {
                        if (res.HasRows)
                        {
                            while (res.Read())
                            {
                                list.Add(new HiringVehicle1
                                {
                                    id = (int)res["id"],
                                    projectName = Convert.ToString(res["title"]),
                                    projectManager = Convert.ToString(res["name"]),
                                    headName = Convert.ToString(res["heads"]),
                                    amount = Convert.ToDecimal(res["amount"]),
                                    dateFrom = Convert.ToDateTime(res["dateFrom"]),
                                    dateTo = Convert.ToDateTime(res["dateTo"]),
                                    proposedPlace = res["proposedPlace"].ToString(),
                                    purposeOfVisit = res["purposeOfVisit"].ToString(),
                                    totalDaysNight = res["totalDaysNight"].ToString(),
                                    totalPlainHills = res["totalPlainHills"].ToString(),
                                    taxi = res["taxi"].ToString(),
                                    BookAgainstCentre = res["BookAgainstCentre"].ToString(),
                                    availbilityOfFund = res["availbilityOfFund"].ToString(),
                                    note = res["note"].ToString(),
                                    newRequest = Convert.ToBoolean(res["newRequest"]),
                                    adminappr = Convert.ToBoolean(res["adminappr"]),
                                    remark = res["remark"].ToString(),
                                    projectCode = res["projectCode"] != DBNull.Value ? res["projectCode"].ToString() : "N/A",
                                });
                            }
                        }
                    }
                    using(var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
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

        #region /* Hiring Report for App */
        public List<HiringVehicle1> hiringreportprojects()
        {
            try
            {
                cmd = new NpgsqlCommand("sp_HiringVehicle", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selecthiringreportprojects");
                con.Open();
                List<HiringVehicle1> list = new List<HiringVehicle1>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        list.Add(new HiringVehicle1
                        {
                            id = Convert.ToInt32(res["projectId"]),
                            projectName = Convert.ToString(res["title"]),
                            employeecode = res["empcode"].ToString(),
                            projectManager = res["empname"].ToString()
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

        public List<HiringVehicle1> hiringreportbyproject(int projectid)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_HiringVehicle", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selecthiringreportbyproject");
                cmd.Parameters.AddWithValue("@projectId", projectid);
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
                            projectManager = Convert.ToString(res["name"]),
                            headName = Convert.ToString(res["heads"]),
                            amount = Convert.ToDecimal(res["amount"]),
                            dateFrom = Convert.ToDateTime(res["dateFrom"]),
                            dateTo = Convert.ToDateTime(res["dateTo"]),
                            proposedPlace = res["proposedPlace"].ToString(),
                            purposeOfVisit = res["purposeOfVisit"].ToString(),
                            totalDaysNight = res["totalDaysNight"].ToString(),
                            totalPlainHills = res["totalPlainHills"].ToString(),
                            taxi = res["taxi"].ToString(),
                            BookAgainstCentre = res["BookAgainstCentre"].ToString(),
                            availbilityOfFund = res["availbilityOfFund"].ToString(),
                            note = res["note"].ToString(),
                            newRequest = Convert.ToBoolean(res["newRequest"]),
                            adminappr = Convert.ToBoolean(res["adminappr"]),
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
        #endregion

        #region /*Tour Report for App */
        public List<tourProposalrepo> TourReportProject()
        {
            try
            {
                cmd = new NpgsqlCommand("sp_Tourproposal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selecttourproject");
                con.Open();
                List<tourProposalrepo> getlist = new List<tourProposalrepo>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        getlist.Add(new tourProposalrepo
                        {
                            id = Convert.ToInt32(res["projectId"]),
                            projectName = Convert.ToString(res["title"]),
                            employeecode = res["empcode"].ToString(),
                            projectManager = res["empname"].ToString()
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

        public List<tourProposalrepo> tourproposalbyproject(int projectid)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_Tourproposal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selecttourproposalbyproject");
                cmd.Parameters.AddWithValue("@projectId", projectid);
                con.Open();
                List<tourProposalrepo> list = new List<tourProposalrepo>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        list.Add(new tourProposalrepo
                        {
                            id = Convert.ToInt32(res["projectId"]),
                            projectManager = Convert.ToString(res["name"]),
                            projectName = Convert.ToString(res["title"]),
                            dateOfDept = Convert.ToDateTime(res["dateOfDept"]),
                            place = Convert.ToString(res["place"]),
                            periodFrom = Convert.ToDateTime(res["periodFrom"]),
                            periodTo = Convert.ToDateTime(res["periodTo"]),
                            returnDate = Convert.ToDateTime(res["returnDate"]),
                            purpose = Convert.ToString(res["purpose"]),
                            newRequest = Convert.ToBoolean(res["newRequest"]),
                            adminappr = Convert.ToBoolean(res["adminappr"])
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
        #endregion

        public List<AdminReimbursement> ReinbursementReport(int? limit = null,int? page = null)
        {
            try
            {
                con.Open();
                List<AdminReimbursement> list = new List<AdminReimbursement>();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_managereimbursement_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "selectReinbursementReport");
                    cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? (object)limit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@v_page", page.HasValue ? (object)page.Value : DBNull.Value);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\";", con, tran))
                    using (var res = fetchCmd.ExecuteReader())
                    {
                        if (res.HasRows)
                        {
                            while (res.Read())
                            {
                                list.Add(new AdminReimbursement
                                {
                                    type = res["type"].ToString(),
                                    EmpName = res["name"].ToString() + $"({res["employeeCode"].ToString()})",
                                    amount = Convert.ToDecimal(res["amount"]),
                                    approveAmount = Convert.ToDecimal(res["apprAmt"] != DBNull.Value ? res["apprAmt"] : 0),
                                    userId = Convert.ToInt32(res["userId"]),
                                    id = Convert.ToInt32(res["id"]),
                                    appr_status = Convert.ToBoolean(res["Apprstatus"]),
                                    newRequest = Convert.ToBoolean(res["newStatus"]),
                                    status = Convert.ToBoolean(res["apprAmountStatus"] != DBNull.Value ? res["apprAmountStatus"] : false),
                                    chequeNum = res["chequeNum"].ToString(),
                                    chequeDate = res["chequeDate"] != DBNull.Value ? Convert.ToDateTime(res["chequeDate"]).ToString("dd/MM/yyyy") : "",
                                    remark = res["remark"].ToString()
                                });
                            }
                        }
                    }
                    using(var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
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
        #endregion

        #region Raised Problem
        public List<RaisedProblem> getProblemList(int? page = null, int?limit = null, int?id=null, int?managerId=null)
        {
            try
            {
                List<RaisedProblem> list = new List<RaisedProblem>();
                con.Open();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_manageproblems_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "selectProblemsforAdmin");
                    cmd.Parameters.AddWithValue("v_projectmanager", managerId);
                    cmd.Parameters.AddWithValue("v_id", id.HasValue ? id : 0);
                    cmd.Parameters.AddWithValue("v_page", page.HasValue ? page : 0);
                    cmd.Parameters.AddWithValue("v_limit", limit.HasValue ? limit : 0);
                    string cursorName = (string)cmd.ExecuteScalar();
                    using (var fetchCmd = new NpgsqlCommand($"fetch all from \"{cursorName}\"", con, tran))
                    using (var res = fetchCmd.ExecuteReader())
                    {
                        if (res.HasRows)
                        {
                            while (res.Read())
                            {
                                list.Add(new RaisedProblem
                                {
                                    id = Convert.ToInt32(res["id"]),
                                    title = res["title"].ToString(),
                                    description = res["description"].ToString(),
                                    adminappr = Convert.ToBoolean(res["adminappr"]),
                                    newRequest = Convert.ToBoolean(res["newRequest"]),
                                    documentname = res["document"].ToString(),
                                    projectname = res["projectName"].ToString(),
                                    projectManager = res["projectManager"].ToString(),
                                    createdAt = Convert.ToDateTime(res["createdAt"]),
                                    projectCode = res["projectCode"] != DBNull.Value ? res["projectCode"].ToString() : "N/A"
                                });
                            }
                        }
                    }
                    using (var closeCmd = new NpgsqlCommand($"close \"{cursorName}\";", con, tran))
                    {
                        closeCmd.ExecuteNonQuery();
                    }
                    tran.Commit();
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
        //public bool approveRaisedProblem(int id, bool status, string remark)
        //{
        //    try
        //    {
        //        cmd = new NpgsqlCommand("sp_raiseProblem", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@action", "approval");
        //        cmd.Parameters.AddWithValue("@adminappr", status);
        //        cmd.Parameters.AddWithValue("@id", id);
        //        con.Open();
        //        int res = cmd.ExecuteNonQuery();
        //        return res > 0;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        if (con.State == ConnectionState.Open)
        //        {
        //            con.Close();
        //        }
        //        cmd.Dispose();
        //    }
        //}

        public List<RaisedProblem> getproblembyId(int id)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_raiseProblem", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectProblemsforAdminById");
                cmd.Parameters.AddWithValue("@id", id);
                List<RaisedProblem> list = new List<RaisedProblem>();
                con.Open();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        list.Add(new RaisedProblem
                        {
                            id = Convert.ToInt32(res["id"]),
                            title = res["title"].ToString(),
                            description = res["description"].ToString(),
                            adminappr = Convert.ToBoolean(res["adminappr"]),
                            newRequest = Convert.ToBoolean(res["newRequest"]),
                            documentname = res["document"].ToString(),
                            projectname = res["projectName"].ToString(),
                            projectManager = res["projectManager"].ToString()
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

        #endregion

        #region get meeting list
        public List<Meeting_Model> getAllmeetingforAdmin()
        {
            try
            {
                List<Meeting_Model> _list = new List<Meeting_Model>();
                Meeting_Model obj = null;
                NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageMeeting", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getAllmeetingofadmin");
                con.Open();
                NpgsqlDataReader sdr = cmd.ExecuteReader();

                while (sdr.Read())
                {
                    obj = new Meeting_Model();
                    obj.Id = Convert.ToInt32(sdr["id"]);
                    obj.CompleteStatus = Convert.ToInt32(sdr["completeStatus"]);
                    obj.MeetingType = sdr["meetingType"].ToString();
                    obj.MeetingLink = sdr["meetingLink"].ToString();
                    obj.MeetingTitle = sdr["MeetingTitle"].ToString();
                    obj.memberId = sdr["memberId"] != DBNull.Value ? sdr["memberId"].ToString().Split(',').ToList() : new List<string>();
                    obj.CreaterId = sdr["createrId"] != DBNull.Value ? Convert.ToInt32(sdr["createrId"]) : 0;
                    obj.MeetingDate = Convert.ToDateTime(sdr["meetingTime"]).ToString("dd-MM-yyyy");
                    obj.summary = sdr["meetSummary"].ToString();
                    _list.Add(obj);
                    obj.createdBy = sdr["createdBy"].ToString();
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
        public List<Meeting_Model> getAllmeetingforprojectmanager()
        {
            try
            {
                List<Meeting_Model> _list = new List<Meeting_Model>();
                Meeting_Model obj = null;
                NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageMeeting", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getAllmeetingofprojectmanager");
                con.Open();
                NpgsqlDataReader sdr = cmd.ExecuteReader();

                while (sdr.Read())
                {
                    obj = new Meeting_Model();
                    obj.Id = Convert.ToInt32(sdr["id"]);
                    obj.CompleteStatus = Convert.ToInt32(sdr["completeStatus"]);
                    obj.MeetingType = sdr["meetingType"].ToString();
                    obj.MeetingLink = sdr["meetingLink"].ToString();
                    obj.MeetingTitle = sdr["MeetingTitle"].ToString();
                    obj.memberId = sdr["memberId"] != DBNull.Value ? sdr["memberId"].ToString().Split(',').ToList() : new List<string>();
                    obj.CreaterId = sdr["createrId"] != DBNull.Value ? Convert.ToInt32(sdr["createrId"]) : 0;
                    obj.MeetingDate = Convert.ToDateTime(sdr["meetingTime"]).ToString("dd-MM-yyyy");
                    obj.summary = sdr["meetSummary"].ToString();
                    _list.Add(obj);
                    obj.createdBy = sdr["createdBy"].ToString();
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
        #endregion
        public List<SubProblem> getAllSubOrdinateProblemByIdforadmin(int id)
        {
            try
            {
                List<SubProblem> problemList = new List<SubProblem>();
                SubProblem obj = null;
                NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageSubordinateProjectProblem", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getallproblembyidforadmin");
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                NpgsqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        obj = new SubProblem();
                        obj.ProblemId = Convert.ToInt32(sdr["problemId"]);
                        obj.ProjectName = sdr["ProjectName"].ToString();
                        obj.Title = sdr["Title"].ToString();
                        obj.Description = sdr["Description"].ToString();
                        obj.Attchment_Url = sdr["Attachment"].ToString();
                        obj.CreatedDate = Convert.ToDateTime(sdr["CreatedDate"]).ToString("dd-MM-yyyy");
                        obj.newRequest = Convert.ToBoolean(sdr["newRequest"]);
                        problemList.Add(obj);
                    }
                }
                return problemList;

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
    }
}