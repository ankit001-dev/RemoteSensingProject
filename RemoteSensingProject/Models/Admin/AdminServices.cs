// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.Admin.AdminServices
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Npgsql;
using NpgsqlTypes;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.MailService;

namespace RemoteSensingProject.Models.Admin
{
	public class AdminServices : DataFactory
	{
		private mail _mail = new mail();

		public bool InsertDesignation(main.CommonResponse cr)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Expected O, but got Unknown
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Expected O, but got Unknown
			//IL_00e5: Expected O, but got Unknown
			try
			{
				NpgsqlCommand cmd = new NpgsqlCommand("CALL sp_manageemployeecategory(:v_id, :v_designationname, :v_status, :v_devisionname, :v_action, :v_rc)", con);
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("v_id", (cr.id == 0) ? DBNull.Value : ((object)cr.id));
					cmd.Parameters.AddWithValue("v_designationname", ((object)cr.name) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("v_status", (object)true);
					cmd.Parameters.AddWithValue("v_devisionname", (object)DBNull.Value);
					cmd.Parameters.AddWithValue("v_action", (object)((cr.id > 0) ? "UpdateDesignation" : "InsertDesignation"));
					NpgsqlParameterCollection parameters = cmd.Parameters;
					NpgsqlParameter val = new NpgsqlParameter("v_rc", (NpgsqlDbType)23);
					((DbParameter)val).Direction = ParameterDirection.InputOutput;
					((DbParameter)val).Value = DBNull.Value;
					parameters.Add(val);
					((DbConnection)(object)con).Open();
					((DbCommand)(object)cmd).ExecuteNonQuery();
					return true;
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
			}
		}

		public bool InsertDivison(main.CommonResponse cr)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Expected O, but got Unknown
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Expected O, but got Unknown
			//IL_00e5: Expected O, but got Unknown
			try
			{
				NpgsqlCommand cmd = new NpgsqlCommand("CALL sp_manageemployeecategory(:v_id, :v_designationname, :v_status, :v_devisionname, :v_action, :v_rc)", con);
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("v_id", (cr.id == 0) ? DBNull.Value : ((object)cr.id));
					cmd.Parameters.AddWithValue("v_designationname", (object)DBNull.Value);
					cmd.Parameters.AddWithValue("v_status", (object)true);
					cmd.Parameters.AddWithValue("v_devisionname", ((object)cr.name) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("v_action", (object)((cr.id > 0) ? "UpdateDevision" : "InsertDevision"));
					NpgsqlParameterCollection parameters = cmd.Parameters;
					NpgsqlParameter val = new NpgsqlParameter("v_rc", (NpgsqlDbType)23);
					((DbParameter)val).Direction = ParameterDirection.InputOutput;
					((DbParameter)val).Value = DBNull.Value;
					parameters.Add(val);
					((DbConnection)(object)con).Open();
					((DbCommand)(object)cmd).ExecuteNonQuery();
					return true;
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)base.cmd).Dispose();
			}
		}

		public List<main.CommonResponse> ListDivison()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			try
			{
				List<main.CommonResponse> list = new List<main.CommonResponse>();
				NpgsqlCommand cmd = new NpgsqlCommand("select * FROM fn_get_employee_category(@action)", con);
				try
				{
					cmd.Parameters.AddWithValue("@action", (object)"GetAllDevision");
					((DbConnection)(object)con).Open();
					NpgsqlDataReader rd = cmd.ExecuteReader();
					if (((DbDataReader)(object)rd).HasRows)
					{
						while (((DbDataReader)(object)rd).Read())
						{
							list.Add(new main.CommonResponse
							{
								id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
								name = ((DbDataReader)(object)rd)["name"].ToString()
							});
						}
						((DbDataReader)(object)rd).Close();
					}
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
				return list;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)base.cmd).Dispose();
			}
		}

		public List<main.CommonResponse> ListDesgination()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			try
			{
				List<main.CommonResponse> list = new List<main.CommonResponse>();
				cmd = new NpgsqlCommand("select * FROM fn_get_employee_category(@action)", con);
				cmd.Parameters.AddWithValue("@action", (object)"GetAllDesignation");
				((DbConnection)(object)con).Open();
				NpgsqlDataReader rd = cmd.ExecuteReader();
				if (((DbDataReader)(object)rd).HasRows)
				{
					while (((DbDataReader)(object)rd).Read())
					{
						list.Add(new main.CommonResponse
						{
							id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
							name = ((DbDataReader)(object)rd)["name"].ToString()
						});
					}
					((DbDataReader)(object)rd).Close();
				}
				return list;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public bool removeDivison(int Id)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Expected O, but got Unknown
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Expected O, but got Unknown
			//IL_00c1: Expected O, but got Unknown
			try
			{
				NpgsqlCommand cmd = new NpgsqlCommand("CALL sp_manageemployeecategory(:v_id, :v_designationname, :v_status, :v_devisionname, :v_action, :v_rc)", con);
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("v_id", (Id == 0) ? DBNull.Value : ((object)Id));
					cmd.Parameters.AddWithValue("v_designationname", (object)DBNull.Value);
					cmd.Parameters.AddWithValue("v_status", (object)false);
					cmd.Parameters.AddWithValue("v_devisionname", (object)DBNull.Value);
					cmd.Parameters.AddWithValue("v_action", (object)"deleteDevision");
					NpgsqlParameterCollection parameters = cmd.Parameters;
					NpgsqlParameter val = new NpgsqlParameter("v_rc", (NpgsqlDbType)23);
					((DbParameter)val).Direction = ParameterDirection.InputOutput;
					((DbParameter)val).Value = DBNull.Value;
					parameters.Add(val);
					((DbConnection)(object)con).Open();
					((DbCommand)(object)cmd).ExecuteNonQuery();
					return true;
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)base.cmd).Dispose();
			}
		}

		public bool removeDesgination(int Id)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Expected O, but got Unknown
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Expected O, but got Unknown
			//IL_00c1: Expected O, but got Unknown
			try
			{
				NpgsqlCommand cmd = new NpgsqlCommand("CALL sp_manageemployeecategory(:v_id, :v_designationname, :v_status, :v_devisionname, :v_action, :v_rc)", con);
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("v_id", (Id == 0) ? DBNull.Value : ((object)Id));
					cmd.Parameters.AddWithValue("v_designationname", (object)DBNull.Value);
					cmd.Parameters.AddWithValue("v_status", (object)false);
					cmd.Parameters.AddWithValue("v_devisionname", (object)DBNull.Value);
					cmd.Parameters.AddWithValue("v_action", (object)"deleteDesignation");
					NpgsqlParameterCollection parameters = cmd.Parameters;
					NpgsqlParameter val = new NpgsqlParameter("v_rc", (NpgsqlDbType)23);
					((DbParameter)val).Direction = ParameterDirection.InputOutput;
					((DbParameter)val).Value = DBNull.Value;
					parameters.Add(val);
					((DbConnection)(object)con).Open();
					((DbCommand)(object)cmd).ExecuteNonQuery();
					return true;
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)base.cmd).Dispose();
			}
		}

		public bool AddEmployees(main.Employee_model emp, out string mess)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_001e: Expected O, but got Unknown
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Expected O, but got Unknown
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Expected O, but got Unknown
			//IL_0257: Expected O, but got Unknown
			mess = "";
			NpgsqlCommand cmd = null;
			try
			{
				NpgsqlCommand val = new NpgsqlCommand("CALL sp_adminemployees(:p_id, :p_employeecode, :p_name, :p_mobile, :p_email, :p_gender, :p_role, :p_username, :p_password, :p_devision, :p_designation, :p_profile, :p_action, :p_rc)", con);
				cmd = val;
				NpgsqlCommand val2 = val;
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
					Random rnd = new Random();
					string userPassword = "";
					if (emp.Id == 0)
					{
						for (int i = 0; i < 8; i++)
						{
							userPassword += validChars[rnd.Next(validChars.Length)];
						}
					}
					string actionType = ((emp.Id > 0) ? "UpdateEmployees" : "InsertEmployees");
					cmd.Parameters.AddWithValue("p_id", (emp.Id == 0) ? DBNull.Value : ((object)emp.Id));
					cmd.Parameters.AddWithValue("p_employeecode", ((object)emp.EmployeeCode) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("p_name", ((object)emp.EmployeeName) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("p_mobile", (object)emp.MobileNo);
					cmd.Parameters.AddWithValue("p_email", ((object)emp.Email) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("p_gender", ((object)emp.Gender) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("p_role", ((object)emp.EmployeeRole) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("p_username", ((object)emp.Email) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("p_password", ((object)userPassword) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("p_devision", (object)emp.Division);
					cmd.Parameters.AddWithValue("p_designation", (object)emp.Designation);
					cmd.Parameters.AddWithValue("p_profile", ((object)emp.Image_url) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("p_action", (object)actionType);
					NpgsqlParameterCollection parameters = cmd.Parameters;
					NpgsqlParameter val3 = new NpgsqlParameter("p_rc", (NpgsqlDbType)23);
					((DbParameter)val3).Direction = ParameterDirection.InputOutput;
					((DbParameter)val3).Value = DBNull.Value;
					parameters.Add(val3);
					((DbConnection)(object)con).Open();
					((DbCommand)(object)cmd).ExecuteNonQuery();
					if (emp.Id == 0)
					{
						string subject = "Login Credential";
						string message = "<p>Your user id: <b>" + emp.Email + "</b></p><br><p>Password: <b>" + userPassword + "</b></p>";
						_mail.SendMail(emp.EmployeeName, emp.Email, subject, message);
					}
					mess = "Employee Added Successfully";
					return true;
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				mess = ex.Message;
				return false;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd)?.Dispose();
			}
		}

		public bool RemoveEmployees(int id)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Expected O, but got Unknown
			NpgsqlCommand cmd = null;
			try
			{
				cmd = new NpgsqlCommand();
				cmd.Connection = con;
				((DbCommand)(object)cmd).CommandType = CommandType.Text;
				((DbCommand)(object)cmd).CommandText = "CALL sp_adminemployees(p_id => @p_id, p_action => @p_action)";
				cmd.Parameters.AddWithValue("p_action", (object)"DeleteEmployees");
				cmd.Parameters.AddWithValue("p_id", (object)id);
				((DbConnection)(object)con).Open();
				((DbCommand)(object)cmd).ExecuteNonQuery();
				return true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public List<main.Employee_model> SelectEmployeeRecord(int? page = null, int? limit = null, string searchTerm = null, int? devision = null)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			List<main.Employee_model> empModel = new List<main.Employee_model>();
			try
			{
				((DbConnection)(object)con).Open();
				NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM fn_get_employees(@v_action,@v_id,@v_limit,@v_page,@v_searchTerm,@v_division);", con);
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("@v_action", (object)"SelectEmployees");
					cmd.Parameters.AddWithValue("@v_id", (object)0);
					cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
					cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
					cmd.Parameters.AddWithValue("@v_searchTerm", (object)(string.IsNullOrEmpty(searchTerm) ? ((IConvertible)DBNull.Value) : ((IConvertible)searchTerm)));
					cmd.Parameters.AddWithValue("@v_division", devision.HasValue ? ((object)devision.Value) : DBNull.Value);
					NpgsqlDataReader record = cmd.ExecuteReader();
					try
					{
						bool firstRow = true;
						while (((DbDataReader)(object)record).Read())
						{
							var roleValue = ((DbDataReader)(object)record)["role"];

                            main.Employee_model employee = new main.Employee_model
							{
								Id = Convert.ToInt32(((DbDataReader)(object)record)["id"]),
								EmployeeCode = ((DbDataReader)(object)record)["employeecode"]?.ToString(),
								EmployeeName = ((DbDataReader)(object)record)["name"]?.ToString(),
								DevisionName = ((DbDataReader)(object)record)["devisionname"]?.ToString(),
								Email = ((DbDataReader)(object)record)["email"]?.ToString(),
								MobileNo = ((((DbDataReader)(object)record)["mobile"] != DBNull.Value) ? Convert.ToInt64(((DbDataReader)(object)record)["mobile"]) : 0),
                                EmployeeRole = roleValue != DBNull.Value
    ? roleValue.ToString()
        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(r => r.Trim())
        .ToArray()
    : Array.Empty<string>(),
                                Division = ((((DbDataReader)(object)record)["devision"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)record)["devision"]) : 0),
								DesignationName = ((DbDataReader)(object)record)["designationname"]?.ToString(),
								Status = (((DbDataReader)(object)record)["status"] != DBNull.Value && Convert.ToBoolean(((DbDataReader)(object)record)["status"])),
								Image_url = ((((DbDataReader)(object)record)["profile"] != DBNull.Value) ? ((DbDataReader)(object)record)["profile"].ToString() : null),
								ActiveStatus = Convert.ToBoolean(((DbDataReader)(object)record)["activestatus"])
							};
							if (firstRow)
							{
								employee.Pagination = new ApiCommon.PaginationInfo
								{
									PageNumber = page.GetValueOrDefault(),
									TotalPages = Convert.ToInt32((((DbDataReader)(object)record)["totalpages"] != DBNull.Value) ? ((DbDataReader)(object)record)["totalpages"] : ((object)0)),
									TotalRecords = Convert.ToInt32((((DbDataReader)(object)record)["totalrecords"] != DBNull.Value) ? ((DbDataReader)(object)record)["totalrecords"] : ((object)0)),
									PageSize = limit.GetValueOrDefault()
								};
								firstRow = false;
							}
							empModel.Add(employee);
						}
					}
					finally
					{
						((IDisposable)record)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
				return empModel;
			}
			catch (Exception innerException)
			{
				throw new Exception("Error while fetching employee records", innerException);
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
			}
		}

		public main.Employee_model SelectEmployeeRecordById(int id)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			try
			{
				main.Employee_model empModel = new main.Employee_model();
				((DbConnection)(object)con).Open();
				NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM fn_get_employees(@v_action,@v_id);", con);
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("@v_action", (object)"SelectEmployeesById");
					cmd.Parameters.AddWithValue("@v_id", (object)id);
					NpgsqlDataReader record = cmd.ExecuteReader();
					try
					{
						while (((DbDataReader)(object)record).Read())
						{
							var roleValue = record["role"];
							empModel = new main.Employee_model
							{
								Id = Convert.ToInt32(((DbDataReader)(object)record)["id"]),
								EmployeeCode = ((DbDataReader)(object)record)["employeecode"]?.ToString(),
								EmployeeName = ((DbDataReader)(object)record)["name"]?.ToString(),
								DevisionName = ((DbDataReader)(object)record)["devisionname"]?.ToString(),
								Email = ((DbDataReader)(object)record)["email"]?.ToString(),
								MobileNo = ((((DbDataReader)(object)record)["mobile"] != DBNull.Value) ? Convert.ToInt64(((DbDataReader)(object)record)["mobile"]) : 0),
                                EmployeeRole = roleValue != DBNull.Value
    ? roleValue.ToString()
        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(r => r.Trim())
        .ToArray()
    : Array.Empty<string>(),

                                Division = ((((DbDataReader)(object)record)["devision"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)record)["devision"]) : 0),
								Designation = ((((DbDataReader)(object)record)["designation"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)record)["designation"]) : 0),
								DesignationName = ((DbDataReader)(object)record)["designationname"]?.ToString(),
								Gender = ((DbDataReader)(object)record)["gender"]?.ToString(),
								Status = (((DbDataReader)(object)record)["status"] != DBNull.Value && Convert.ToBoolean(((DbDataReader)(object)record)["status"])),
								Image_url = ((((DbDataReader)(object)record)["profile"] != DBNull.Value) ? ((DbDataReader)(object)record)["profile"].ToString() : null),
								ActiveStatus = Convert.ToBoolean(((DbDataReader)(object)record)["activeStatus"]),
								updationStatus = Convert.ToBoolean(((DbDataReader)(object)record)["updatestatus"])
							};
						}
					}
					finally
					{
						((IDisposable)record)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
				return empModel;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)base.cmd).Dispose();
			}
		}

		public bool ChangeActieStatus(int id)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand();
				cmd.Connection = con;
				((DbCommand)(object)cmd).CommandType = CommandType.Text;
				((DbCommand)(object)cmd).CommandText = "CALL sp_adminemployees(p_id => @p_id, p_action => @p_action)";
				cmd.Parameters.AddWithValue("p_action", (object)"ChangeActiveStatus");
				cmd.Parameters.AddWithValue("p_id", (object)id);
				((DbConnection)(object)con).Open();
				((DbCommand)(object)cmd).ExecuteNonQuery();
				return true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public bool addProject(main.createProjectModel pm)
		{
			NpgsqlTransaction tran = null;
			NpgsqlCommand cmd = null;
			try
			{
				((DbConnection)(object)con).Open();
				tran = con.BeginTransaction();
				Random rand = new Random();
				if (pm.pm.Id > 0)
				{
					pm.projectCode = $"{rand.Next(1000, 9999)}{DateTime.Now.Day}{DateTime.Now.Year.ToString().Substring(2, 2)}";
				}
				int ProjectManager;
				Dictionary<string, object> projectParams = new Dictionary<string, object>
				{
					["p_action"] = ((pm.pm.Id <= 0) ? "insertProject" : "updateProject"),
					["p_letterno"] = Convert.ToInt32(pm.pm.letterNo),
					["p_title"] = pm.pm.ProjectTitle,
					["p_assigndate"] = pm.pm.AssignDate,
					["p_startdate"] = pm.pm.StartDate,
					["p_completiondate"] = pm.pm.CompletionDate,
					["p_projectmanager"] = (int.TryParse(pm.pm.ProjectManager, out ProjectManager) ? ProjectManager : 0),
					["p_budget"] = Convert.ToDouble(pm.pm.ProjectBudget),
					["p_description"] = pm.pm.ProjectDescription ?? string.Empty,
					["p_projectdocument"] = pm.pm.projectDocumentUrl ?? string.Empty,
					["p_projecttype"] = pm.pm.ProjectType ?? string.Empty,
					["p_stage"] = pm.pm.ProjectStage,
					["p_createdby"] = pm.pm.createdBy ?? string.Empty,
					["p_status"] = true,
					["p_approvestatus"] = true,
					["p_projectcode"] = pm.projectCode,
					["p_id"] = pm.pm.Id,
					["p_hrcount"] = pm.pm.hrCount
				};
				int projectId = ExecuteProjectAction(projectParams, tran);
				if (pm.budgets != null && pm.budgets.Count > 0)
				{
					foreach (main.Project_Budget item in pm.budgets)
					{
						if (item.HeadId > 0)
						{
							Dictionary<string, object> budgetParams = new Dictionary<string, object>
							{
								["p_action"] = ((item.Id <= 0) ? "insertProjectBudget" : "updateprojectBudget"),
								["p_project_id"] = ((pm.pm.Id > 0) ? pm.pm.Id : projectId),
								["p_heads"] = item.HeadId.ToString(),
								["p_headsamount"] = item.ProjectAmount,
								["p_projectmanager"] = item.Id
							};
							ExecuteProjectAction(budgetParams, tran);
						}
					}
				}
				if (pm.hr != null && pm.hr.Count > 0 && pm.pm.hrCount > 0)
				{
					foreach (main.HumanResources item2 in pm.hr)
					{
						Dictionary<string, object> hrparams = new Dictionary<string, object>
						{
							["p_action"] = ((item2.id <= 0) ? "insertHumanResources" : "updateHumanResources"),
							["p_project_id"] = ((pm.pm.Id > 0) ? pm.pm.Id : projectId),
							["p_id"] = item2.designationId,
							["p_hrcount"] = item2.designationCount,
							["p_projectmanager"] = item2.id
						};
						ExecuteProjectAction(hrparams, tran);
					}
				}
				if (pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage)
				{
					foreach (main.Project_Statge item3 in pm.stages)
					{
						Dictionary<string, object> stageParams = new Dictionary<string, object>
						{
							["p_action"] = ((item3.Id <= 0) ? "insertProjectStage" : "updateprojectStage"),
							["p_project_id"] = ((pm.pm.Id > 0) ? pm.pm.Id : projectId),
							["p_id"] = item3.Id,
							["p_keypoint"] = item3.KeyPoint,
							["p_completiondate"] = item3.CompletionDate,
							["p_stagedocument"] = item3.Document_Url
						};
						ExecuteProjectAction(stageParams, tran);
					}
				}
				if (pm.pm.SubOrdinate != null && pm.pm.SubOrdinate.Length != 0)
				{
					int[] subOrdinate = pm.pm.SubOrdinate;
					foreach (int subId in subOrdinate)
					{
						int SubProjectManager;
						Dictionary<string, object> subParams = new Dictionary<string, object>
						{
							["p_action"] = ((pm.pm.Id <= 0) ? "insertSubOrdinate" : "updateSubOrdinate"),
							["p_project_id"] = ((pm.pm.Id > 0) ? pm.pm.Id : projectId),
							["p_id"] = subId,
							["p_projectmanager"] = (int.TryParse(pm.pm.ProjectManager, out SubProjectManager) ? SubProjectManager : 0)
						};
						ExecuteProjectAction(subParams, tran);
					}
				}
				if (pm.pm.ProjectType.Equals("External") && (projectId > 0 || pm.pm.Id > 0))
				{
					Dictionary<string, object> extParams = new Dictionary<string, object>
					{
						["p_action"] = ((pm.pm.Id <= 0) ? "insertExternalProject" : "updateExternalProject"),
						["p_project_id"] = ((pm.pm.Id > 0) ? pm.pm.Id : projectId),
						["p_departmentname"] = pm.pm.ProjectDepartment,
						["p_contactperson"] = pm.pm.ContactPerson,
						["p_address"] = pm.pm.Address
					};
					ExecuteProjectAction(extParams, tran);
				}
				((DbTransaction)(object)tran).Commit();
				return true;
			}
			catch (Exception ex)
			{
				((DbTransaction)(object)tran)?.Rollback();
				Console.WriteLine("❌ Error: " + ex.Message);
				return false;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd)?.Dispose();
			}
		}

		private int ExecuteProjectAction(Dictionary<string, object> parameters, NpgsqlTransaction tran)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			NpgsqlCommand cmd = new NpgsqlCommand("CALL sp_adminaddproject(\r\n    @p_action,\r\n    @p_letterno,\r\n    @p_id,\r\n    @p_title,\r\n    @p_assigndate,\r\n    @p_startdate,\r\n    @p_completiondate,\r\n    @p_projectmanager,\r\n    @p_subordinate,\r\n    @p_budget,\r\n    @p_description,\r\n    @p_projectdocument,\r\n    @p_projecttype,\r\n    @p_stage,\r\n    @p_projectcode,\r\n    @p_approvestatus,\r\n    @p_createdby,\r\n    @p_status,\r\n    @p_heads,\r\n    @p_headsamount,\r\n    @p_keypoint,\r\n    @p_stagedocument,\r\n    @p_departmentname,\r\n    @p_contactperson,\r\n    @p_address,\r\n    @p_hrcount,\r\n    @p_project_id\r\n)", con, tran);
			try
			{
				((DbCommand)(object)cmd).CommandType = CommandType.Text;
				List<string> allParams = new List<string>
			{
				"p_action", "p_letterno", "p_id", "p_title", "p_assigndate", "p_startdate", "p_completiondate", "p_projectmanager", "p_subordinate", "p_budget",
				"p_description", "p_projectdocument", "p_projecttype", "p_stage", "p_projectcode", "p_approvestatus", "p_createdby", "p_status", "p_heads", "p_headsamount",
				"p_keypoint", "p_stagedocument", "p_departmentname", "p_contactperson", "p_address", "p_hrcount", "p_project_id"
			};
				foreach (string paramName in allParams)
				{
					if (parameters.ContainsKey(paramName))
					{
						cmd.Parameters.AddWithValue(paramName, parameters[paramName] ?? DBNull.Value);
					}
					else if (paramName == "p_project_id" || paramName == "ref")
					{
						((DbParameter)(object)cmd.Parameters.AddWithValue(paramName, (object)0)).Direction = ParameterDirection.InputOutput;
					}
					else
					{
						cmd.Parameters.AddWithValue(paramName, (object)DBNull.Value);
					}
				}
				((DbCommand)(object)cmd).ExecuteNonQuery();
				if (((DbParameterCollection)(object)cmd.Parameters).Contains("p_project_id") && ((DbParameter)(object)cmd.Parameters["p_project_id"]).Value != DBNull.Value)
				{
					return Convert.ToInt32(((DbParameter)(object)cmd.Parameters["p_project_id"]).Value);
				}
				return 0;
			}
			finally
			{
				((IDisposable)cmd)?.Dispose();
			}
		}

		private DateTime? GetDateSafe(IDataReader rd, string column)
		{
			object value = rd[column];
			if (value == null || value == DBNull.Value)
			{
				return null;
			}
			if (value is DateTimeOffset dto)
			{
				return dto.DateTime;
			}
			if (value is DateTime dt)
			{
				return dt;
			}
			if (double.TryParse(value.ToString(), out var num))
			{
				return DateTime.FromOADate(num);
			}
			if (DateTime.TryParse(value.ToString(), out var parsed))
			{
				return parsed;
			}
			return null;
		}

		public List<main.Project_model> Project_List(int? page = null, int? limit = null, string filterType = null, string searchTerm = null, string statusFilter = null, int? projectManager = null)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			try
			{
				List<main.Project_model> list = new List<main.Project_model>();
				cmd = new NpgsqlCommand("SELECT * FROM fn_get_all_projects(@action,@v_id,@v_projectManager,@v_filterType,@v_limit,@v_page,@v_searchTerm,@v_statusFilter)", con);
				cmd.Parameters.AddWithValue("@action", (object)"GetAllProject");
				cmd.Parameters.AddWithValue("@v_id", (object)DBNull.Value);
				cmd.Parameters.AddWithValue("@v_projectManager", projectManager.HasValue ? ((object)projectManager) : DBNull.Value);
				cmd.Parameters.AddWithValue("@v_filterType", (object)(string.IsNullOrEmpty(filterType) ? ((IConvertible)DBNull.Value) : ((IConvertible)filterType)));
				cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
				cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
				cmd.Parameters.AddWithValue("@v_searchTerm", (object)(string.IsNullOrEmpty(searchTerm) ? ((IConvertible)DBNull.Value) : ((IConvertible)searchTerm)));
				cmd.Parameters.AddWithValue("@v_statusFilter", (object)(string.IsNullOrEmpty(statusFilter) ? ((IConvertible)DBNull.Value) : ((IConvertible)statusFilter)));
				((DbConnection)(object)con).Open();
				NpgsqlDataReader rd = cmd.ExecuteReader();
				if (((DbDataReader)(object)rd).HasRows)
				{
					bool firstRow = true;
					while (((DbDataReader)(object)rd).Read())
					{
						main.Project_model project = new main.Project_model
						{
							Id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
							ProjectTitle = ((DbDataReader)(object)rd)["title"].ToString(),
							AssignDate = GetDateSafe((IDataReader)rd, "assignDate"),
							CompletionDate = GetDateSafe((IDataReader)rd, "completionDate"),
							StartDate = GetDateSafe((IDataReader)rd, "startDate"),
							ProjectManager = ((DbDataReader)(object)rd)["name"].ToString(),
							Percentage = ((((DbDataReader)(object)rd)["financialStatusPercentage"] != DBNull.Value) ? ((DbDataReader)(object)rd)["financialStatusPercentage"].ToString() : ""),
							ProjectBudget = Convert.ToDecimal((((DbDataReader)(object)rd)["budget"] != DBNull.Value) ? ((DbDataReader)(object)rd)["budget"] : ((object)0)),
							ProjectDescription = ((DbDataReader)(object)rd)["description"].ToString(),
							projectDocumentUrl = ((DbDataReader)(object)rd)["ProjectDocument"].ToString(),
							ProjectType = ((DbDataReader)(object)rd)["projectType"].ToString(),
							physicalcomplete = Math.Round(Convert.ToDecimal(((DbDataReader)(object)rd)["completionPercentage"]), 2),
							overallPercentage = Convert.ToDecimal(((DbDataReader)(object)rd)["overallPercentage"]),
							ProjectStage = Convert.ToBoolean(((DbDataReader)(object)rd)["stage"]),
							ProjectStatus = Convert.ToBoolean(((DbDataReader)(object)rd)["CompleteStatus"]),
							createdBy = ((DbDataReader)(object)rd)["createdBy"].ToString(),
							projectCode = ((((DbDataReader)(object)rd)["projectCode"] != DBNull.Value) ? ((DbDataReader)(object)rd)["projectCode"].ToString() : "N/A"),
							devisionName = ((DbDataReader)(object)rd)["devisionname"].ToString()
						};
						project.CompletionDatestring = project.CompletionDate?.ToString("dd-MM-yyyy") ?? "N/A";
						project.AssignDateString = project.AssignDate?.ToString("dd-MM-yyyy") ?? "N/A";
						project.StartDateString = project.StartDate?.ToString("dd-MM-yyyy") ?? "N/A";
						if (project.ProjectStatus || project.physicalcomplete == 100m)
						{
							project.projectStatusLabel = "Completed";
						}
						else if (project.CompletionDate < DateTime.Now)
						{
							project.projectStatusLabel = "Delay";
						}
						else if (project.StartDate < DateTime.Now)
						{
							project.projectStatusLabel = "Ongoing";
						}
						else
						{
							project.projectStatusLabel = "Upcoming";
						}
						if (firstRow)
						{
							project.Pagination = new ApiCommon.PaginationInfo
							{
								PageNumber = page.GetValueOrDefault(),
								TotalPages = Convert.ToInt32((((DbDataReader)(object)rd)["totalpages"] != DBNull.Value) ? ((DbDataReader)(object)rd)["totalpages"] : ((object)0)),
								TotalRecords = Convert.ToInt32((((DbDataReader)(object)rd)["totalrecords"] != DBNull.Value) ? ((DbDataReader)(object)rd)["totalrecords"] : ((object)0)),
								PageSize = limit.GetValueOrDefault()
							};
							firstRow = false;
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
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public List<main.Project_model> getHeadByProject(int projectId, int? page = null, int? limit = null)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			try
			{
				List<main.Project_model> _headList = new List<main.Project_model>();
				((DbConnection)(object)con).Open();
				main.Project_model obj = null;
				NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM fn_get_all_projects(@v_action,@v_id,@v_limit ,@v_page)", con);
				cmd.Parameters.AddWithValue("@v_action", (object)"getHeadByProject");
				cmd.Parameters.AddWithValue("@v_id", (object)projectId);
				cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
				cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
				NpgsqlDataReader sdr = cmd.ExecuteReader();
				if (((DbDataReader)(object)sdr).HasRows)
				{
					while (((DbDataReader)(object)sdr).Read())
					{
						obj = new main.Project_model();
						obj.Id = Convert.ToInt32(((DbDataReader)(object)sdr)["id"]);
						obj.heads = ((DbDataReader)(object)sdr)["title"].ToString();
						_headList.Add(obj);
					}
				}
				return _headList;
			}
			catch (Exception innerException)
			{
				throw new Exception("An error accured", innerException);
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)base.cmd).Dispose();
			}
		}

		public main.createProjectModel GetProjectById(int id)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			try
			{
				main.createProjectModel cpm = new main.createProjectModel();
				cmd = new NpgsqlCommand("SELECT * FROM fn_get_all_projects(@action,@v_id,@v_limit,@v_page)", con);
				cmd.Parameters.AddWithValue("@action", (object)"GetProjectById");
				cmd.Parameters.AddWithValue("@v_id", (object)id);
				cmd.Parameters.AddWithValue("@v_limit", (object)DBNull.Value);
				cmd.Parameters.AddWithValue("@v_page", (object)DBNull.Value);
				((DbConnection)(object)con).Open();
				NpgsqlDataReader rd = cmd.ExecuteReader();
				List<main.Project_Subordination> subList = new List<main.Project_Subordination>();
				main.Project_model pm = new main.Project_model();
				if (((DbDataReader)(object)rd).HasRows)
				{
					while (((DbDataReader)(object)rd).Read())
					{
						pm.Id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]);
						pm.hrCount = ((((DbDataReader)(object)rd)["hrcount"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)rd)["hrcount"]) : 0);
						pm.ProjectTitle = ((DbDataReader)(object)rd)["title"].ToString();
						pm.AssignDate = GetDateSafe((IDataReader)rd, "assignDate");
						pm.CompletionDate = GetDateSafe((IDataReader)rd, "completionDate");
						pm.StartDate = GetDateSafe((IDataReader)rd, "startDate");
						pm.ProjectManager = ((DbDataReader)(object)rd)["name"].ToString();
						pm.ProjectBudget = Convert.ToDecimal(((DbDataReader)(object)rd)["budget"]);
						pm.ProjectDescription = ((DbDataReader)(object)rd)["description"].ToString();
						pm.projectDocumentUrl = ((DbDataReader)(object)rd)["ProjectDocument"].ToString();
						pm.ProjectType = ((DbDataReader)(object)rd)["projectType"].ToString();
						pm.ProjectStage = Convert.ToBoolean(((DbDataReader)(object)rd)["stage"]);
						pm.ProjectStatus = Convert.ToBoolean(((DbDataReader)(object)rd)["CompleteStatus"]);
						pm.projectCode = ((((DbDataReader)(object)rd)["projectCode"] != DBNull.Value) ? ((DbDataReader)(object)rd)["projectCode"].ToString() : "N/A");
						pm.devisionName = ((DbDataReader)(object)rd)["devisionname"].ToString();
						if (pm.ProjectType.Equals("External"))
						{
							pm.Address = ((DbDataReader)(object)rd)["address"].ToString();
							pm.ProjectDepartment = ((DbDataReader)(object)rd)["departmentname"].ToString();
							pm.ContactPerson = ((DbDataReader)(object)rd)["contactperson"].ToString();
						}
						if (((DbDataReader)(object)rd)["SubordinateLinkId"] != DBNull.Value)
						{
							subList.Add(new main.Project_Subordination
							{
								Id = Convert.ToInt32(((DbDataReader)(object)rd)["SubordinateLinkId"]),
								Name = ((DbDataReader)(object)rd)["subName"].ToString(),
								EmpCode = ((DbDataReader)(object)rd)["subCode"].ToString()
							});
						}
						pm.CompletionDatestring = pm.CompletionDate?.ToString("dd-MM-yyyy") ?? "N/A";
						pm.AssignDateString = pm.AssignDate?.ToString("dd-MM-yyyy") ?? "N/A";
						pm.StartDateString = pm.StartDate?.ToString("dd-MM-yyyy") ?? "N/A";
					}
					((DbDataReader)(object)rd).Close();
				}
				cpm.pm = pm;
				cpm.SubOrdinate = subList;
				cpm.budgets = ProjectBudgetList(id);
				cpm.stages = ProjectStagesList(id);
				cpm.hr = ProjectHumanResourcesList(id);
				return cpm;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public bool createApiProject(main.Project_model pm)
		{
			NpgsqlTransaction tran = null;
			NpgsqlCommand cmd = null;
			try
			{
				((DbConnection)(object)con).Open();
				tran = con.BeginTransaction();
				Random rand = new Random();
				pm.projectCode = $"{rand.Next(1000, 9999)}{DateTime.Now.Day}{DateTime.Now.Year.ToString().Substring(2, 2)}";
				int letterNo;
				int ProjectManager;
				Dictionary<string, object> projectParams = new Dictionary<string, object>
				{
					["p_action"] = ((pm.Id > 0) ? "updateProject" : "insertProject"),
					["p_letterno"] = (int.TryParse(pm.letterNo, out letterNo) ? letterNo : 0),
					["p_title"] = pm.ProjectTitle,
					["p_assigndate"] = pm.AssignDate,
					["p_startdate"] = pm.StartDate,
					["p_completiondate"] = pm.CompletionDate,
					["p_projectmanager"] = (int.TryParse(pm.ProjectManager, out ProjectManager) ? ProjectManager : 0),
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
				if (pm.SubOrdinate != null && pm.SubOrdinate.Length != 0)
				{
					int[] subOrdinate = pm.SubOrdinate;
					foreach (int subId in subOrdinate)
					{
						int SubProjectManager;
						Dictionary<string, object> subParams = new Dictionary<string, object>
						{
							["p_action"] = "insertSubOrdinate",
							["p_project_id"] = projectId,
							["p_id"] = subId,
							["p_projectmanager"] = (int.TryParse(pm.ProjectManager, out SubProjectManager) ? SubProjectManager : 0)
						};
						ExecuteProjectAction(subParams, tran);
					}
				}
				if (pm.ProjectType.Equals("External") && projectId > 0)
				{
					Dictionary<string, object> extParams = new Dictionary<string, object>
					{
						["p_action"] = ((pm.Id > 0) ? "updateExternalProject" : "insertExternalProject"),
						["p_project_id"] = projectId,
						["p_departmentname"] = pm.ProjectDepartment,
						["p_contactperson"] = pm.ContactPerson,
						["p_address"] = pm.Address
					};
					ExecuteProjectAction(extParams, tran);
				}
				((DbTransaction)(object)tran).Commit();
				return true;
			}
			catch (Exception)
			{
				((DbTransaction)(object)tran).Rollback();
				return false;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public bool insertProjectStages(main.Project_Statge stg)
		{
			NpgsqlTransaction tran = null;
			NpgsqlCommand cmd = null;
			try
			{
				((DbConnection)(object)con).Open();
				tran = con.BeginTransaction();
				Dictionary<string, object> stageParams = new Dictionary<string, object>
				{
					["p_action"] = "insertProjectStage",
					["p_project_id"] = stg.Project_Id,
					["p_keypoint"] = stg.KeyPoint,
					["p_completiondate"] = stg.CompletionDate,
					["p_stagedocument"] = stg.Document_Url
				};
				ExecuteProjectAction(stageParams, tran);
				((DbTransaction)(object)tran).Commit();
				return true;
			}
			catch (Exception)
			{
				((DbTransaction)(object)tran).Rollback();
				return false;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public bool insertProjectBudgets(main.Project_Budget bdg)
		{
			NpgsqlTransaction tran = null;
			NpgsqlCommand cmd = null;
			try
			{
				((DbConnection)(object)con).Open();
				tran = con.BeginTransaction();
				Dictionary<string, object> budgetParams = new Dictionary<string, object>
				{
					["p_action"] = ((bdg.Id > 0) ? "updateProjectBudget" : "insertProjectBudget"),
					["p_project_id"] = bdg.Project_Id,
					["p_heads"] = bdg.ProjectHeads,
					["p_headsamount"] = bdg.ProjectAmount
				};
				ExecuteProjectAction(budgetParams, tran);
				((DbTransaction)(object)tran).Commit();
				return true;
			}
			catch (Exception)
			{
				((DbTransaction)(object)tran).Rollback();
				return false;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public List<main.Project_Budget> ProjectBudgetList(int Id)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			try
			{
				List<main.Project_Budget> list = new List<main.Project_Budget>();
				cmd = new NpgsqlCommand("SELECT * FROM fn_getProjectStagesAndBudget(@action,@v_id,@v_limit,@v_page)", con);
				cmd.Parameters.AddWithValue("@action", (object)"GetBudgetByProjectId");
				cmd.Parameters.AddWithValue("@v_id", (object)Id);
				cmd.Parameters.AddWithValue("@v_limit", (object)DBNull.Value);
				cmd.Parameters.AddWithValue("@v_page", (object)DBNull.Value);
				if (((DbConnection)(object)con).State == ConnectionState.Closed)
				{
					((DbConnection)(object)con).Open();
				}
				NpgsqlDataReader rd = cmd.ExecuteReader();
				if (((DbDataReader)(object)rd).HasRows)
				{
					while (((DbDataReader)(object)rd).Read())
					{
						list.Add(new main.Project_Budget
						{
							Id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
							Project_Id = Convert.ToInt32(((DbDataReader)(object)rd)["project_id"]),
							ProjectHeads = ((DbDataReader)(object)rd)["heads"].ToString(),
							HeadId = Convert.ToInt32((((DbDataReader)(object)rd)["budgethead"] != DBNull.Value) ? ((DbDataReader)(object)rd)["budgethead"] : ((object)0)),
							TotalAskAmount = ((DbDataReader)(object)rd)["totalAskAmount"].ToString(),
							ApproveAmount = ((DbDataReader)(object)rd)["approveAmount"].ToString(),
							ProjectAmount = Convert.ToDecimal((((DbDataReader)(object)rd)["headsAmount"] != DBNull.Value) ? ((DbDataReader)(object)rd)["headsAmount"] : ((object)0))
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
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public List<main.Project_Statge> ProjectStagesList(int Id)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			try
			{
				List<main.Project_Statge> list = new List<main.Project_Statge>();
				cmd = new NpgsqlCommand("SELECT * FROM fn_getProjectStagesAndBudget(@action,@v_id,@v_limit,@v_page)", con);
				cmd.Parameters.AddWithValue("@action", (object)"GetProjectStageByProjectId");
				cmd.Parameters.AddWithValue("@v_id", (object)Id);
				cmd.Parameters.AddWithValue("@v_limit", (object)DBNull.Value);
				cmd.Parameters.AddWithValue("@v_page", (object)DBNull.Value);
				if (((DbConnection)(object)con).State == ConnectionState.Closed)
				{
					((DbConnection)(object)con).Open();
				}
				NpgsqlDataReader rd = cmd.ExecuteReader();
				if (((DbDataReader)(object)rd).HasRows)
				{
					while (((DbDataReader)(object)rd).Read())
					{
						list.Add(new main.Project_Statge
						{
							Id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
							Project_Id = Convert.ToInt32(((DbDataReader)(object)rd)["project_id"]),
							KeyPoint = ((DbDataReader)(object)rd)["keyPoint"].ToString(),
							CompletionDate = Convert.ToDateTime(((DbDataReader)(object)rd)["completeDate"]),
							CompletionDatestring = Convert.ToDateTime(((DbDataReader)(object)rd)["completeDate"]).ToString("dd-MM-yyyy"),
							Status = ((DbDataReader)(object)rd)["StagesStatus"].ToString(),
							Document_Url = ((DbDataReader)(object)rd)["stageDocument"].ToString(),
							completionStatus = Convert.ToInt32(((DbDataReader)(object)rd)["completionStatus"])
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
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public List<main.HumanResources> ProjectHumanResourcesList(int Id)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			try
			{
				List<main.HumanResources> list = new List<main.HumanResources>();
				cmd = new NpgsqlCommand("SELECT * FROM fn_getProjectStagesAndBudget(@action,@v_id,@v_limit,@v_page)", con);
				cmd.Parameters.AddWithValue("@action", (object)"GetProjectHumanResources");
				cmd.Parameters.AddWithValue("@v_id", (object)Id);
				cmd.Parameters.AddWithValue("@v_limit", (object)DBNull.Value);
				cmd.Parameters.AddWithValue("@v_page", (object)DBNull.Value);
				if (((DbConnection)(object)con).State == ConnectionState.Closed)
				{
					((DbConnection)(object)con).Open();
				}
				NpgsqlDataReader rd = cmd.ExecuteReader();
				if (((DbDataReader)(object)rd).HasRows)
				{
					while (((DbDataReader)(object)rd).Read())
					{
						list.Add(new main.HumanResources
						{
							id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
							designationId = Convert.ToInt32(((DbDataReader)(object)rd)["project_id"]),
							designationCount = Convert.ToInt32(((DbDataReader)(object)rd)["approveAmount"]),
							designationName = ((DbDataReader)(object)rd)["stageDocument"].ToString()
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
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public main.DashboardCount DashboardCount()
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Expected O, but got Unknown
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Expected O, but got Unknown
			main.DashboardCount obj = null;
			try
			{
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_managedashboard_cursor", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"AdminDashboardCount");
						cmd.Parameters.AddWithValue("v_projectmanager", (object)0);
						cmd.Parameters.AddWithValue("v_sid", (object)0);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("FETCH ALL FROM \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader sdr = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)sdr).HasRows)
								{
									((DbDataReader)(object)sdr).Read();
									obj = new main.DashboardCount
									{
										TotalEmployee = ((DbDataReader)(object)sdr)["TotalEmployee"].ToString(),
										TotalProjectManager = ((DbDataReader)(object)sdr)["TotalProjectManager"].ToString(),
										TotalSubOrdinate = ((DbDataReader)(object)sdr)["TotalSubOrdinate"].ToString(),
										TotalAccounts = ((DbDataReader)(object)sdr)["TotalAccounts"].ToString(),
										TotalProject = ((DbDataReader)(object)sdr)["TotalProject"].ToString(),
										TotalInternalProject = ((DbDataReader)(object)sdr)["TotalInternalProject"].ToString(),
										TotalExternalProject = ((DbDataReader)(object)sdr)["TotalExternalProject"].ToString(),
										TotalDelayproject = ((DbDataReader)(object)sdr)["TotalDelayproject"].ToString(),
										TotalCompleteProject = ((DbDataReader)(object)sdr)["TotalCompleteProject"].ToString(),
										TotalOngoingProject = ((DbDataReader)(object)sdr)["TotalOngoingProject"].ToString(),
										TotalMeetings = ((DbDataReader)(object)sdr)["TotalMeetings"].ToString(),
										TotalAdminMeetings = ((DbDataReader)(object)sdr)["TotalAdminMeetings"].ToString(),
										TotalProjectManagerMeetings = ((DbDataReader)(object)sdr)["TotalProjectManagerMeetings"].ToString(),
										TotalReinbursementReq = ((DbDataReader)(object)sdr)["TotalReinbursementReq"].ToString(),
										TotalTourProposalReq = ((DbDataReader)(object)sdr)["TotalTourProposalReq"].ToString(),
										totalVehicleHiringRequest = ((DbDataReader)(object)sdr)["totalVehicleHiringRequest"].ToString(),
										totalReinbursementPendingRequest = ((DbDataReader)(object)sdr)["totalReinbursementPendingRequest"].ToString(),
										totalReinbursementapprovedRequest = ((DbDataReader)(object)sdr)["totalReinbursementapprovedRequest"].ToString(),
										totalReinbursementRejectRequest = ((DbDataReader)(object)sdr)["totalReinbursementRejectRequest"].ToString(),
										totalTourProposalApprReque = ((DbDataReader)(object)sdr)["totalTourProposalApprReque"].ToString(),
										totalTourProposalRejectReque = ((DbDataReader)(object)sdr)["totalTourProposalRejectReque"].ToString(),
										totaTourProposalPendingReque = ((DbDataReader)(object)sdr)["totalTourProposalRejectReque"].ToString(),
										totalPendingHiringVehicle = ((DbDataReader)(object)sdr)["totalPendingHiringVehicle"].ToString(),
										totalApproveHiringVehicle = ((DbDataReader)(object)sdr)["totalApproveHiringVehicle"].ToString(),
										totalRejectHiringVehicle = ((DbDataReader)(object)sdr)["totalRejectHiringVehicle"].ToString(),
										TotalBudget = Convert.ToDecimal((((DbDataReader)(object)sdr)["totalBudgets"] != DBNull.Value) ? ((DbDataReader)(object)sdr)["totalBudgets"] : ((object)0)),
										PendingBudget = Convert.ToDecimal((((DbDataReader)(object)sdr)["pendingBudget"] != DBNull.Value) ? ((DbDataReader)(object)sdr)["pendingBudget"] : ((object)0))
									};
								}
							}
							finally
							{
								((IDisposable)sdr)?.Dispose();
							}
						}
						finally
						{
							((IDisposable)fetchCmd)?.Dispose();
						}
						NpgsqlCommand closeCmd = new NpgsqlCommand("CLOSE \"" + cursorName + "\";", con, tran);
						try
						{
							((DbCommand)(object)closeCmd).ExecuteNonQuery();
						}
						finally
						{
							((IDisposable)closeCmd)?.Dispose();
						}
						((DbTransaction)(object)tran).Commit();
					}
					finally
					{
						((IDisposable)cmd)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)tran)?.Dispose();
				}
				return obj;
			}
			catch (Exception innerException)
			{
				throw new Exception("Error while fetching dashboard data", innerException);
			}
			finally
			{
				((DbConnection)(object)con).Close();
			}
		}

		public List<main.ProjectExpenditure> ViewProjectExpenditure(int? limit = null, int? page = null)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Expected O, but got Unknown
			try
			{
				List<main.ProjectExpenditure> list = new List<main.ProjectExpenditure>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_managedashboard_cursor", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"viewProjectExpenditure");
						cmd.Parameters.AddWithValue("v_projectmanager", (object)DBNull.Value);
						cmd.Parameters.AddWithValue("v_sid", (object)0);
						cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("FETCH ALL FROM \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader rd = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)rd).HasRows)
								{
									bool firstRow = true;
									while (((DbDataReader)(object)rd).Read())
									{
										list.Add(new main.ProjectExpenditure
										{
											id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
											ProjectName = ((DbDataReader)(object)rd)["title"].ToString(),
											projectmanager = ((DbDataReader)(object)rd)["name"].ToString(),
											ProjectBudget = Convert.ToDecimal(((DbDataReader)(object)rd)["budget"]),
											expenditure = ((((DbDataReader)(object)rd)["ExpendedAmt"] != DBNull.Value) ? Convert.ToDecimal(((DbDataReader)(object)rd)["ExpendedAmt"]) : 0m),
											remaining = ((((DbDataReader)(object)rd)["remainingAmt"] != DBNull.Value) ? Convert.ToDecimal(((DbDataReader)(object)rd)["remainingAmt"]) : 0m)
										});
										if (firstRow)
										{
											list[0].Pagination = new ApiCommon.PaginationInfo
											{
												PageNumber = page.GetValueOrDefault(),
												TotalPages = Convert.ToInt32((((DbDataReader)(object)rd)["totalpages"] != DBNull.Value) ? ((DbDataReader)(object)rd)["totalpages"] : ((object)0)),
												TotalRecords = Convert.ToInt32((((DbDataReader)(object)rd)["totalrecords"] != DBNull.Value) ? ((DbDataReader)(object)rd)["totalrecords"] : ((object)0)),
												PageSize = limit.GetValueOrDefault()
											};
											firstRow = false;
										}
									}
								}
								return list;
							}
							finally
							{
								((IDisposable)rd)?.Dispose();
							}
						}
						finally
						{
							((IDisposable)fetchCmd)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)cmd)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)tran)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)base.cmd).Dispose();
			}
		}

		public List<main.DashboardCount> getAllProjectCompletion()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			try
			{
				List<main.DashboardCount> list = new List<main.DashboardCount>();
				main.DashboardCount obj = null;
				NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageDashboard", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"getOverallProjectCompletion");
				((DbConnection)(object)con).Open();
				NpgsqlDataReader sdr = cmd.ExecuteReader();
				if (((DbDataReader)(object)sdr).HasRows)
				{
					while (((DbDataReader)(object)sdr).Read())
					{
						obj = new main.DashboardCount();
						obj.Title = ((DbDataReader)(object)sdr)["title"].ToString();
						obj.OverallCompletionPercentage = ((DbDataReader)(object)sdr)["overallCompletion"].ToString();
						obj.ProjectManager = ((DbDataReader)(object)sdr)["ProjectManager"].ToString();
						list.Add(obj);
					}
				}
				return list;
			}
			catch (Exception innerException)
			{
				throw new Exception("An error accured", innerException);
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)base.cmd).Dispose();
			}
		}

		public List<main.Employee_model> BindEmployee()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			List<main.Employee_model> empList = new List<main.Employee_model>();
			main.Employee_model empObj = null;
			try
			{
				((DbConnection)(object)con).Open();
				NpgsqlCommand cmd = new NpgsqlCommand("SELECT * from fn_get_employees(@v_action, @v_id);", con);
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("@v_action", (object)"SelectEmployees");
					cmd.Parameters.AddWithValue("@v_id", (object)DBNull.Value);
					NpgsqlDataReader reader = cmd.ExecuteReader();
					try
					{
						while (((DbDataReader)(object)reader).Read())
						{
                            var roleValue = reader["role"];
                            empObj = new main.Employee_model();
							empObj.Id = Convert.ToInt32(((DbDataReader)(object)reader)["id"]);
							empObj.EmployeeName = ((DbDataReader)(object)reader)["name"].ToString();
							empObj.EmployeeRole = roleValue != DBNull.Value
    ? roleValue.ToString()
        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(r => r.Trim())
        .ToArray()
    : Array.Empty<string>();
                            empList.Add(empObj);
						}
					}
					finally
					{
						((IDisposable)reader)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error fetching meeting members: " + ex.Message, ex);
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
			}
			return empList;
		}

		public bool insertMeeting(main.AddMeeting_Model obj)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected O, but got Unknown
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Expected O, but got Unknown
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Expected O, but got Unknown
			//IL_01f9: Expected O, but got Unknown
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Expected O, but got Unknown
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Expected O, but got Unknown
			((DbConnection)(object)con).Open();
			NpgsqlTransaction transaction = con.BeginTransaction();
			try
			{
				int meetingId = 0;
				NpgsqlCommand cmd = new NpgsqlCommand("CALL public.sp_managemeeting(@p_id, @p_meetingtype, @p_meetinglink, @p_meetingtitle, @p_meetingtime, @p_meetingdocument, @p_createdby, NULL, @p_createrid, NULL, 0, 0, 0, NULL, NULL, @p_meetingid, 0, @p_action)", con, transaction);
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("p_id", (object)obj.Id);
					cmd.Parameters.AddWithValue("p_meetingtype", ((object)obj.MeetingType) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("p_meetinglink", (obj.MeetingType?.ToLower() == "offline") ? (((object)obj.MeetingAddress) ?? ((object)DBNull.Value)) : (((object)obj.MeetingLink) ?? ((object)DBNull.Value)));
					cmd.Parameters.AddWithValue("p_meetingtitle", ((object)obj.MeetingTitle) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("p_meetingtime", (object)obj.MeetingTime);
					cmd.Parameters.AddWithValue("p_meetingdocument", ((object)obj.Attachment_Url) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("p_createdby", (object)((obj.CreaterId > 0) ? "projectmanager" : "admin"));
					cmd.Parameters.AddWithValue("p_createrid", (object)((obj.CreaterId > 0) ? obj.CreaterId : new int?(0)));
					cmd.Parameters.AddWithValue("p_action", (object)((obj.Id > 0) ? "updateMeeting" : "insertMeeting"));
					NpgsqlParameter val = new NpgsqlParameter("p_meetingid", (NpgsqlDbType)9);
					((DbParameter)val).Direction = ParameterDirection.InputOutput;
					((DbParameter)val).Value = ((obj.Id > 0) ? obj.Id : 0);
					NpgsqlParameter meetingIdParam = val;
					cmd.Parameters.Add(meetingIdParam);
					((DbCommand)(object)cmd).ExecuteNonQuery();
					meetingId = Convert.ToInt32(((DbParameter)(object)meetingIdParam).Value);
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
				if (obj.meetingMemberList != null && obj.meetingMemberList.Any())
				{
					foreach (int member in obj.meetingMemberList)
					{
						if (member != 0)
						{
							NpgsqlCommand cmd2 = new NpgsqlCommand("CALL public.sp_managemeeting(p_employee => @p_employee, p_meeting => @p_meeting,p_appstatus => @p_appstatus, p_action => @p_action)", con, transaction);
							try
							{
								((DbCommand)(object)cmd2).CommandType = CommandType.Text;
								cmd2.Parameters.AddWithValue("@p_employee", (NpgsqlDbType)9, (object)member);
								cmd2.Parameters.AddWithValue("@p_meeting", (NpgsqlDbType)9, (object)meetingId);
								cmd2.Parameters.AddWithValue("@p_appstatus", (NpgsqlDbType)9, (object)((obj.CreaterId > 0) ? 1 : 0));
								cmd2.Parameters.AddWithValue("@p_action", (NpgsqlDbType)22, (object)"addMeetingMember");
								((DbCommand)(object)cmd2).ExecuteNonQuery();
							}
							finally
							{
								((IDisposable)cmd2)?.Dispose();
							}
						}
					}
				}
				if (obj.keyPointList != null && obj.keyPointList.Any() && obj.keyPointList != null && obj.keyPointList.Count > 0)
				{
					foreach (string key in obj.keyPointList)
					{
						if (!string.IsNullOrWhiteSpace(key))
						{
							NpgsqlCommand cmd3 = new NpgsqlCommand("CALL public.sp_managemeeting(p_keypoint => @p_keypoint, p_meeting => @p_meeting, p_action => @p_action)", con, transaction);
							try
							{
								((DbCommand)(object)cmd3).CommandType = CommandType.Text;
								cmd3.Parameters.AddWithValue("@p_keypoint", (object)key);
								cmd3.Parameters.AddWithValue("@p_meeting", (object)meetingId);
								cmd3.Parameters.AddWithValue("@p_action", (object)"AddMeetingKeyPoint");
								((DbCommand)(object)cmd3).ExecuteNonQuery();
							}
							finally
							{
								((IDisposable)cmd3)?.Dispose();
							}
						}
					}
				}
				((DbTransaction)(object)transaction).Commit();
				return true;
			}
			catch (Exception innerException)
			{
				((DbTransaction)(object)transaction).Rollback();
				throw new Exception("Error while inserting/updating meeting", innerException);
			}
			finally
			{
				((IDisposable)transaction)?.Dispose();
			}
		}

		public bool UpdateMeeting(main.AddMeeting_Model obj)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Expected O, but got Unknown
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Expected O, but got Unknown
			((DbConnection)(object)con).Open();
			NpgsqlTransaction transaction = con.BeginTransaction();
			try
			{
				NpgsqlCommand cmd = new NpgsqlCommand("\r\n                    CALL public.sp_managemeeting(\r\n                        p_id => @p_id,\r\n                        p_meetingtype => @p_meetingtype,\r\n                        p_meetinglink => @p_meetinglink,\r\n                        p_meetingtitle => @p_meetingtitle,\r\n                        p_meetingtime => @p_meetingtime,\r\n                        p_meetingdocument => @p_meetingdocument,\r\n                        p_createdby => NULL,\r\n                        p_updateddate => NULL,\r\n                        p_createrid => NULL,\r\n                        p_conclusionid => NULL,\r\n                        p_employee => 0,\r\n                        p_meeting => 0,\r\n                        p_appstatus => 0,\r\n                        p_keypoint => NULL,\r\n                        p_reason => NULL,\r\n                        p_meetingid => NULL,\r\n                        p_status => 0,\r\n                        p_action => @p_action\r\n                    );", con, transaction);
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("@p_id", (NpgsqlDbType)9, (object)obj.Id);
					cmd.Parameters.AddWithValue("@p_meetingtype", (NpgsqlDbType)22, ((object)obj.MeetingType) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("@p_meetinglink", (NpgsqlDbType)19, (object)((obj.MeetingType?.ToLower() == "offline") ? (obj.MeetingAddress ?? "") : (obj.MeetingLink ?? "")));
					cmd.Parameters.AddWithValue("@p_meetingtitle", (NpgsqlDbType)22, ((object)obj.MeetingTitle) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("@p_meetingtime", (NpgsqlDbType)22, ((object)obj.MeetingTime.ToString()) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("@p_meetingdocument", (NpgsqlDbType)19, ((object)obj.Attachment_Url) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("@p_action", (NpgsqlDbType)22, (object)"updateMeeting");
					((DbCommand)(object)cmd).ExecuteNonQuery();
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
				if (obj.meetingMemberList != null && obj.meetingMemberList.Count > 0)
				{
					foreach (int memberId in obj.meetingMemberList)
					{
						if (memberId != 0)
						{
							NpgsqlCommand cmdMember = new NpgsqlCommand("\r\n                                CALL public.sp_managemeeting(\r\n                                    p_employee => @p_employee,\r\n                                    p_meeting => @p_meeting,\r\n                                    p_id => 0,\r\n                                    p_meetingtype => NULL,\r\n                                    p_meetinglink => NULL,\r\n                                    p_meetingtitle => NULL,\r\n                                    p_meetingtime => NULL,\r\n                                    p_meetingdocument => NULL,\r\n                                    p_createdby => NULL,\r\n                                    p_updateddate => NULL,\r\n                                    p_createrid => NULL,\r\n                                    p_conclusionid => NULL,\r\n                                    p_appstatus => 0,\r\n                                    p_keypoint => NULL,\r\n                                    p_reason => NULL,\r\n                                    p_meetingid => 0,\r\n                                    p_status => 0,\r\n                                    p_action => @p_action\r\n                                );", con, transaction);
							try
							{
								((DbCommand)(object)cmdMember).CommandType = CommandType.Text;
								cmdMember.Parameters.AddWithValue("@p_employee", (NpgsqlDbType)9, (object)memberId);
								cmdMember.Parameters.AddWithValue("@p_meeting", (NpgsqlDbType)9, (object)obj.Id);
								cmdMember.Parameters.AddWithValue("@p_action", (NpgsqlDbType)22, (object)"updateMember");
								((DbCommand)(object)cmdMember).ExecuteNonQuery();
							}
							finally
							{
								((IDisposable)cmdMember)?.Dispose();
							}
						}
					}
				}
				if (obj.KeypointId != null && obj.keyPointList != null)
				{
					for (int j = 0; j < obj.KeypointId.Count; j++)
					{
						if (!string.IsNullOrWhiteSpace(obj.KeypointId[j]?.ToString()) && !string.IsNullOrWhiteSpace(obj.keyPointList[j]?.ToString()))
						{
							NpgsqlCommand cmdKey = new NpgsqlCommand("\r\n                                CALL public.sp_managemeeting(\r\n                                    p_meeting => @p_meeting,\r\n                                    p_keypoint => @p_keypoint,\r\n                                    p_action => @p_action,\r\n                                    p_id => @p_id\r\n                                );", con, transaction);
							try
							{
								((DbCommand)(object)cmdKey).CommandType = CommandType.Text;
								cmdKey.Parameters.AddWithValue("@p_meeting", (NpgsqlDbType)9, (object)obj.Id);
								cmdKey.Parameters.AddWithValue("@p_id", (NpgsqlDbType)9, (object)Convert.ToInt32(obj.KeypointId[j]));
								cmdKey.Parameters.AddWithValue("@p_keypoint", (NpgsqlDbType)22, (object)obj.keyPointList[j]);
								cmdKey.Parameters.AddWithValue("@p_action", (NpgsqlDbType)22, (object)"updateKeyPoint");
								((DbCommand)(object)cmdKey).ExecuteNonQuery();
							}
							finally
							{
								((IDisposable)cmdKey)?.Dispose();
							}
						}
					}
				}
				((DbTransaction)(object)transaction).Commit();
				return true;
			}
			catch (Exception innerException)
			{
				((DbTransaction)(object)transaction).Rollback();
				throw new Exception("An error occurred while updating the meeting.", innerException);
			}
			finally
			{
				((IDisposable)transaction)?.Dispose();
			}
		}

		public List<main.Meeting_Model> getAllmeeting(int? limit = null, int? page = null, string searchTerm = null, string statusFilter = null, string meetingMode = null)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<main.Meeting_Model> _list = new List<main.Meeting_Model>();
				main.Meeting_Model obj = null;
				NpgsqlCommand cmd = new NpgsqlCommand("SELECT * from fn_get_meetings(@p_action,@p_id,@v_limit,@v_page,@v_type,@v_searchTerm,@v_statusFilter,@v_meetingMode);", con);
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("@p_action", (object)"getAllmeeting");
					cmd.Parameters.AddWithValue("@p_id", (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
					cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
					cmd.Parameters.AddWithValue("@v_type", (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@v_searchTerm", (object)(string.IsNullOrEmpty(searchTerm) ? ((IConvertible)DBNull.Value) : ((IConvertible)searchTerm)));
					cmd.Parameters.AddWithValue("@v_statusFilter", (object)(string.IsNullOrEmpty(statusFilter) ? ((IConvertible)DBNull.Value) : ((IConvertible)statusFilter)));
					cmd.Parameters.AddWithValue("@v_meetingMode", (object)(string.IsNullOrEmpty(meetingMode) ? ((IConvertible)DBNull.Value) : ((IConvertible)meetingMode)));
					NpgsqlDataReader sdr = cmd.ExecuteReader();
					try
					{
						bool firstRow = true;
						while (((DbDataReader)(object)sdr).Read())
						{
							obj = new main.Meeting_Model();
							obj.Id = Convert.ToInt32(((DbDataReader)(object)sdr)["id"]);
							obj.CompleteStatus = Convert.ToInt32(((DbDataReader)(object)sdr)["completeStatus"]);
							obj.MeetingType = ((DbDataReader)(object)sdr)["meetingType"].ToString();
							obj.MeetingLink = ((DbDataReader)(object)sdr)["meetingLink"].ToString();
							obj.MeetingTitle = ((DbDataReader)(object)sdr)["MeetingTitle"].ToString();
							obj.memberId = ((((DbDataReader)(object)sdr)["memberId"] != DBNull.Value) ? ((DbDataReader)(object)sdr)["memberId"].ToString().Split(',').ToList() : new List<string>());
							obj.CreaterId = ((((DbDataReader)(object)sdr)["createrId"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)sdr)["createrId"]) : 0);
							obj.MeetingDate = ((((DbDataReader)(object)sdr)["meetingTime"] != DBNull.Value && DateTime.TryParse(((DbDataReader)(object)sdr)["meetingTime"].ToString(), out var mt)) ? mt.ToString("dd-MM-yyyy") : string.Empty);
							obj.summary = ((DbDataReader)(object)sdr)["meetSummary"].ToString();
							obj.Attachment_Url = ((((DbDataReader)(object)sdr)["reason"] != null) ? ((DbDataReader)(object)sdr)["reason"].ToString() : "");
							obj.createdBy = ((DbDataReader)(object)sdr)["createdBy"].ToString();
							obj.statusLabel = ((Convert.ToInt32(((DbDataReader)(object)sdr)["completeStatus"]) == 0) ? "Pending" : "Completed");
							_list.Add(obj);
							if (firstRow)
							{
								obj.Pagination = new ApiCommon.PaginationInfo
								{
									PageNumber = page.GetValueOrDefault(),
									TotalPages = Convert.ToInt32((((DbDataReader)(object)sdr)["totalpages"] != DBNull.Value) ? ((DbDataReader)(object)sdr)["totalpages"] : ((object)0)),
									TotalRecords = Convert.ToInt32((((DbDataReader)(object)sdr)["totalrecords"] != DBNull.Value) ? ((DbDataReader)(object)sdr)["totalrecords"] : ((object)0)),
									PageSize = limit.GetValueOrDefault()
								};
								firstRow = false;
							}
						}
					}
					finally
					{
						((IDisposable)sdr)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
				return _list;
			}
			catch (Exception innerException)
			{
				throw new Exception("An error accured", innerException);
			}
			finally
			{
				((DbConnection)(object)con).Close();
			}
		}

		public main.Meeting_Model getMeetingById(int id)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			main.Meeting_Model obj = new main.Meeting_Model();
			try
			{
				((DbConnection)(object)con).Open();
				NpgsqlCommand cmd = new NpgsqlCommand("SELECT * from fn_get_meetings(@p_action,@p_id);", con);
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("@p_action", (object)"getMeetingById");
					cmd.Parameters.AddWithValue("@p_id", (object)id);
					NpgsqlDataReader sdr = cmd.ExecuteReader();
					try
					{
						if (((DbDataReader)(object)sdr).Read())
						{
							obj.Id = Convert.ToInt32(((DbDataReader)(object)sdr)["id"]);
							obj.MeetingType = ((DbDataReader)(object)sdr)["meetingType"].ToString();
							obj.MeetingLink = ((DbDataReader)(object)sdr)["meetingLink"].ToString();
							obj.MeetingTitle = ((DbDataReader)(object)sdr)["meetingTitle"].ToString();
							obj.MeetingTime = Convert.ToDateTime(((DbDataReader)(object)sdr)["meetingTime"]);
							obj.MeetingTimeString = Convert.ToDateTime(((DbDataReader)(object)sdr)["meetingTime"]).ToString("yyyy-MM-ddTHH:mm");
							obj.memberId = ((((DbDataReader)(object)sdr)["empId"] != DBNull.Value) ? ((DbDataReader)(object)sdr)["empId"].ToString().Split(',').ToList() : new List<string>());
							obj.Attachment_Url = ((((DbDataReader)(object)sdr)["reason"] != null) ? ((DbDataReader)(object)sdr)["reason"].ToString() : "");
						}
						if (((DbDataReader)(object)sdr)["meetingKey"] != null)
						{
							List<main.KeyPoint> keyDict = new List<main.KeyPoint>();
							string[] array = ((DbDataReader)(object)sdr)["meetingKey"].ToString().Split(',');
							foreach (string key in array)
							{
								if (!string.IsNullOrEmpty(key))
								{
									keyDict.Add(new main.KeyPoint
									{
										Id = int.Parse(key.Split(':')[0]),
										keyPoint = key.Split(':')[1]
									});
								}
							}
							obj.MeetingKeyPointDict = keyDict;
						}
					}
					finally
					{
						((IDisposable)sdr)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
			}
			catch (Exception innerException)
			{
				throw new Exception("An error occurred while fetching meeting details", innerException);
			}
			finally
			{
				((DbConnection)(object)con).Close();
				((Component)(object)base.cmd).Dispose();
			}
			return obj;
		}

		public List<main.Employee_model> GetMeetingMemberList(int id)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<main.Employee_model> empModel = new List<main.Employee_model>();
				((DbParameterCollection)(object)base.cmd.Parameters).Clear();
				NpgsqlCommand cmd = new NpgsqlCommand("SELECT * from fn_get_meetings(@p_action,@p_id);", con);
				try
				{
					cmd.Parameters.AddWithValue("@p_id", (object)id);
					cmd.Parameters.AddWithValue("@p_action", (object)"getMeetingMemberById");
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					NpgsqlDataReader res = cmd.ExecuteReader();
					try
					{
						if (((DbDataReader)(object)res).HasRows)
						{
							while (((DbDataReader)(object)res).Read())
							{
                                var roleValue = res["role"];
                                empModel.Add(new main.Employee_model
								{
									Id = (int)((DbDataReader)(object)res)["id"],
									EmployeeCode = ((DbDataReader)(object)res)["employeeCode"].ToString(),
									EmployeeName = ((DbDataReader)(object)res)["name"].ToString(),
									EmployeeRole = roleValue != DBNull.Value
    ? roleValue.ToString()
        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(r => r.Trim())
        .ToArray()
    : Array.Empty<string>(),
                                    MobileNo = Convert.ToInt64((((DbDataReader)(object)res)["mobile"] != DBNull.Value) ? ((DbDataReader)(object)res)["mobile"] : ((object)0)),
									Email = ((DbDataReader)(object)res)["email"].ToString(),
									meetingId = Convert.ToInt32((((DbDataReader)(object)res)["meetingId"] != DBNull.Value) ? ((DbDataReader)(object)res)["meetingId"] : ((object)0))
								});
							}
						}
					}
					finally
					{
						((IDisposable)res)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
				return empModel;
			}
			catch (Exception innerException)
			{
				throw new Exception("An error accured", innerException);
			}
			finally
			{
				((DbConnection)(object)con).Close();
				((Component)(object)base.cmd).Dispose();
			}
		}

		public bool AddMeetingResponse(main.MeetingConclusion mc)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Expected O, but got Unknown
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Expected O, but got Unknown
			//IL_015b: Expected O, but got Unknown
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Expected O, but got Unknown
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Expected O, but got Unknown
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Expected O, but got Unknown
			((DbConnection)(object)con).Open();
			NpgsqlTransaction transaction = con.BeginTransaction();
			try
			{
				int conclusionId = 0;
				NpgsqlCommand cmd = new NpgsqlCommand("\r\n            CALL public.sp_meetingconslusion(\r\n                @v_id::integer,\r\n                @v_meeting::integer,\r\n                @v_conclusion::text,\r\n                @v_nextfollow::varchar,\r\n                @v_memberid::integer,\r\n                @v_response::text,\r\n                @v_followupstatus::boolean,\r\n                @v_keyid::integer,\r\n                @v_conclusionid::integer,\r\n                @v_summary::text,\r\n                @v_action::varchar,\r\n                @v_rc::refcursor\r\n            )", con, transaction);
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("v_id", (object)0);
					cmd.Parameters.AddWithValue("v_meeting", (object)Convert.ToInt32(mc.Meeting));
					cmd.Parameters.AddWithValue("v_conclusion", ((object)mc.Conclusion) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("v_nextfollow", ((object)mc.NextFollowUpDate?.ToString("yyyy-MM-dd")) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("v_memberid", (object)0);
					cmd.Parameters.AddWithValue("v_response", (object)DBNull.Value);
					cmd.Parameters.AddWithValue("v_followupstatus", (NpgsqlDbType)2, (object)(mc.FollowUpStatus ? true : false));
					cmd.Parameters.AddWithValue("v_keyid", (object)0);
					NpgsqlParameter val = new NpgsqlParameter("v_conclusionid", (NpgsqlDbType)9);
					((DbParameter)val).Direction = ParameterDirection.InputOutput;
					((DbParameter)val).Value = conclusionId;
					NpgsqlParameter conclusionParam = val;
					cmd.Parameters.Add(conclusionParam);
					cmd.Parameters.AddWithValue("v_summary", ((object)mc.summary) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("v_action", (object)"insertConclusion");
					cmd.Parameters.AddWithValue("v_rc", (object)DBNull.Value);
					((DbCommand)(object)cmd).ExecuteNonQuery();
					conclusionId = (int)((DbParameter)(object)conclusionParam).Value;
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
				if (mc.MemberId != null && mc.MemberId.Count > 0)
				{
					foreach (string memberId in mc.MemberId)
					{
						if (!string.IsNullOrEmpty(memberId))
						{
							NpgsqlCommand memberCmd = new NpgsqlCommand("\r\n                        CALL public.sp_meetingconslusion(\r\n                            @v_id::integer,\r\n                            0::integer,\r\n                            NULL::text,\r\n                            NULL::varchar,\r\n                            @v_memberid::integer,\r\n                            NULL::text,\r\n                            false::boolean,\r\n                            0::integer,\r\n                            @v_conclusionid::integer,\r\n                            NULL::text,\r\n                            @v_action::varchar,\r\n                            NULL::refcursor\r\n                        )", con, transaction);
							((DbCommand)(object)memberCmd).CommandType = CommandType.Text;
							memberCmd.Parameters.AddWithValue("v_id", (object)conclusionId);
							memberCmd.Parameters.AddWithValue("v_memberid", (object)Convert.ToInt32(memberId));
							memberCmd.Parameters.AddWithValue("v_conclusionid", (object)conclusionId);
							memberCmd.Parameters.AddWithValue("v_action", (object)"updateMeetingMemberPresence");
							((DbCommand)(object)memberCmd).ExecuteNonQuery();
						}
					}
				}
				if (mc.KeyPointId != null && mc.KeyPointId.Count > 0)
				{
					for (int i = 0; i < mc.KeyPointId.Count; i++)
					{
						NpgsqlCommand keyCmd = new NpgsqlCommand("\r\n                    CALL public.sp_meetingconslusion(\r\n                        @v_id::integer,\r\n                        0::integer,\r\n                        NULL::text,\r\n                        NULL::varchar,\r\n                        0::integer,\r\n                        @v_response::text,\r\n                        false::boolean,\r\n                        @v_keyid::integer,\r\n                        @v_conclusionid::integer,\r\n                        NULL::text,\r\n                        @v_action::varchar,\r\n                        NULL::refcursor\r\n                    )", con, transaction);
						((DbCommand)(object)keyCmd).CommandType = CommandType.Text;
						keyCmd.Parameters.AddWithValue("v_id", (object)conclusionId);
						keyCmd.Parameters.AddWithValue("v_keyid", (object)mc.KeyPointId[i]);
						keyCmd.Parameters.AddWithValue("v_response", ((object)mc.KeyResponse[i]) ?? ((object)DBNull.Value));
						keyCmd.Parameters.AddWithValue("v_conclusionid", (object)conclusionId);
						keyCmd.Parameters.AddWithValue("v_action", (object)"isertKeypointResponse");
						((DbCommand)(object)keyCmd).ExecuteNonQuery();
					}
				}
				if (mc.MeetingMemberList != null && mc.FollowUpStatus)
				{
					foreach (int individualMember in mc.MeetingMemberList)
					{
						if (individualMember != 0)
						{
							NpgsqlCommand memberCmd2 = new NpgsqlCommand("\r\n                        CALL sp_ManageMeeting(\r\n                           p_action=> @v_action::varchar,\r\n                           p_employee=> @v_employee::integer,\r\n                           p_meeting=> @v_meeting::integer\r\n                        )", con, transaction);
							((DbCommand)(object)memberCmd2).CommandType = CommandType.Text;
							memberCmd2.Parameters.AddWithValue("@v_action", (object)"addMeetingMember");
							memberCmd2.Parameters.AddWithValue("@v_employee", (object)individualMember);
							memberCmd2.Parameters.AddWithValue("@v_meeting", (object)mc.Meeting);
							((DbCommand)(object)memberCmd2).ExecuteNonQuery();
						}
					}
				}
				((DbTransaction)(object)transaction).Commit();
				return true;
			}
			catch (Exception innerException)
			{
				((DbTransaction)(object)transaction).Rollback();
				throw new Exception("An error occurred while adding meeting response.", innerException);
			}
			finally
			{
				((DbConnection)(object)con).Close();
			}
		}

		public List<main.MeetingConclusion> getConclusion(int id)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			try
			{
				List<main.MeetingConclusion> meetingc = new List<main.MeetingConclusion>();
				((DbConnection)(object)con).Open();
				((DbParameterCollection)(object)base.cmd.Parameters).Clear();
				NpgsqlCommand cmd = new NpgsqlCommand("select * from fn_get_meetings(@p_action,@p_id);", con);
				try
				{
					cmd.Parameters.AddWithValue("@p_action", (object)"selectConclusion");
					cmd.Parameters.AddWithValue("@p_id", (object)id);
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					NpgsqlDataReader rdr = cmd.ExecuteReader();
					try
					{
						if (((DbDataReader)(object)rdr).HasRows)
						{
							while (((DbDataReader)(object)rdr).Read())
							{
								meetingc.Add(new main.MeetingConclusion
								{
									Id = Convert.ToInt32(((DbDataReader)(object)rdr)["id"]),
									Meeting = id,
									MeetingDate = ((((DbDataReader)(object)rdr)["meetingTime"] != DBNull.Value) ? Convert.ToDateTime(((DbDataReader)(object)rdr)["meetingTime"]).ToString("dd-MM-yyyy hh:mm tt") : "N/A"),
									Conclusion = ((DbDataReader)(object)rdr)["conclusion"].ToString(),
									NextFollow = ((((DbDataReader)(object)rdr)["nextFollow"] != DBNull.Value) ? Convert.ToDateTime(((DbDataReader)(object)rdr)["nextFollow"]).ToString("dd-MM-yyyy") : "N/A"),
									mode = ((DbDataReader)(object)rdr)["meetingType"].ToString(),
									address = ((DbDataReader)(object)rdr)["meetingLink"].ToString()
								});
							}
						}
					}
					finally
					{
						((IDisposable)rdr)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
				return meetingc;
			}
			catch (Exception innerException)
			{
				throw new Exception("An error accured", innerException);
			}
			finally
			{
				((DbConnection)(object)con).Close();
				((Component)(object)base.cmd).Dispose();
			}
		}

		public List<main.Employee_model> getPresentMember(int id)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<main.Employee_model> meetingc = new List<main.Employee_model>();
				((DbParameterCollection)(object)base.cmd.Parameters).Clear();
				NpgsqlCommand cmd = new NpgsqlCommand("select * from fn_get_meetings(@p_action,@p_id);", con);
				try
				{
					cmd.Parameters.AddWithValue("@p_action", (object)"selectPresentMember");
					cmd.Parameters.AddWithValue("@p_id", (object)id);
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					NpgsqlDataReader rdr = cmd.ExecuteReader();
					try
					{
						if (((DbDataReader)(object)rdr).HasRows)
						{
							while (((DbDataReader)(object)rdr).Read())
							{
								var roleValue = rdr["role"];

                                meetingc.Add(new main.Employee_model
								{
									EmployeeName = ((DbDataReader)(object)rdr)["name"].ToString(),
									Image_url = ((DbDataReader)(object)rdr)["profile"].ToString(),
									EmployeeRole = roleValue != DBNull.Value
    ? roleValue.ToString()
        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(r => r.Trim())
        .ToArray()
    : Array.Empty<string>(),
                                    PresentStatus = (bool)((DbDataReader)(object)rdr)["completeStatus"]
								});
							}
						}
					}
					finally
					{
						((IDisposable)rdr)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
				return meetingc;
			}
			catch (Exception innerException)
			{
				throw new Exception("An error accured", innerException);
			}
			finally
			{
				((DbConnection)(object)con).Close();
				((Component)(object)base.cmd).Dispose();
			}
		}

		public List<main.KeyPointResponse> getKeypointResponse(int id)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			try
			{
				((DbParameterCollection)(object)base.cmd.Parameters).Clear();
				List<main.KeyPointResponse> md = new List<main.KeyPointResponse>();
				((DbConnection)(object)con).Open();
				NpgsqlCommand cmd = new NpgsqlCommand("select * from fn_get_meetings(@p_action,@p_id)", con);
				try
				{
					cmd.Parameters.AddWithValue("@p_action", (object)"KeyPointResponse");
					cmd.Parameters.AddWithValue("@p_id", (object)id);
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					NpgsqlDataReader rdr = cmd.ExecuteReader();
					try
					{
						if (((DbDataReader)(object)rdr).HasRows)
						{
							while (((DbDataReader)(object)rdr).Read())
							{
								md.Add(new main.KeyPointResponse
								{
									KeyPoint = ((DbDataReader)(object)rdr)["keypoint"].ToString(),
									Response = ((DbDataReader)(object)rdr)["response"].ToString()
								});
							}
						}
					}
					finally
					{
						((IDisposable)rdr)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
				return md;
			}
			catch (Exception innerException)
			{
				throw new Exception("An error accured", innerException);
			}
			finally
			{
				((DbConnection)(object)con).Close();
				((Component)(object)base.cmd).Dispose();
			}
		}

		public bool InsertNotice(main.Generate_Notice gn)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("CALL sp_managenotice(@v_id, @v_projectid, @v_noticedocs, @v_noticedesc, NULL, @v_action)", con);
				((DbCommand)(object)cmd).CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@v_action", (object)((gn.Id > 0) ? "UpdateNotice" : "InsertNotice"));
				cmd.Parameters.AddWithValue("@v_projectId", (object)gn.ProjectId);
				cmd.Parameters.AddWithValue("v_id", (object)gn.Id);
				cmd.Parameters.AddWithValue("v_noticeDocs", ((object)gn.Attachment_Url) ?? ((object)DBNull.Value));
				cmd.Parameters.AddWithValue("v_noticedesc", (object)gn.Notice);
				((DbConnection)(object)con).Open();
				((DbCommand)(object)cmd).ExecuteNonQuery();
				return true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public List<main.Generate_Notice> getNoticeList(int? limit = null, int? page = null, int? id = null, int? managerId = null, string searchTerm = null)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Expected O, but got Unknown
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<main.Generate_Notice> noticeList = new List<main.Generate_Notice>();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageNotice_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"SelectNotice");
						cmd.Parameters.AddWithValue("v_projectmanager", managerId.HasValue ? ((object)managerId.Value) : ((object)0));
						cmd.Parameters.AddWithValue("v_id", id.HasValue ? ((object)id.Value) : ((object)0));
						cmd.Parameters.AddWithValue("v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("v_searchterm", (object)(string.IsNullOrEmpty(searchTerm) ? ((IConvertible)DBNull.Value) : ((IConvertible)searchTerm)));
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader res = fetchCmd.ExecuteReader();
							try
							{
								while (((DbDataReader)(object)res).Read())
								{
									bool firstRow = true;
									noticeList.Add(new main.Generate_Notice
									{
										Id = (int)((DbDataReader)(object)res)["id"],
										ProjectId = (int)((DbDataReader)(object)res)["project_id"],
										ProjectManagerId = (int)((DbDataReader)(object)res)["empid"],
										Attachment_Url = ((((DbDataReader)(object)res)["NoticeDocument"] == null) ? null : ((DbDataReader)(object)res)["NoticeDocument"].ToString()),
										Notice = ((DbDataReader)(object)res)["noticeDescription"].ToString(),
										ProjectManagerImage = ((DbDataReader)(object)res)["profile"].ToString(),
										ProjectManager = ((DbDataReader)(object)res)["name"].ToString(),
										ProjectName = ((DbDataReader)(object)res)["title"].ToString(),
										noticeDate = Convert.ToDateTime(((DbDataReader)(object)res)["noticeDate"]).ToString("dd-MM-yyyy")
									});
									if (firstRow)
									{
										noticeList[0].Pagination = new ApiCommon.PaginationInfo
										{
											PageNumber = page.GetValueOrDefault(),
											TotalPages = Convert.ToInt32((((DbDataReader)(object)res)["totalpages"] != DBNull.Value) ? ((DbDataReader)(object)res)["totalpages"] : ((object)0)),
											TotalRecords = Convert.ToInt32((((DbDataReader)(object)res)["totalrecords"] != DBNull.Value) ? ((DbDataReader)(object)res)["totalrecords"] : ((object)0)),
											PageSize = limit.GetValueOrDefault()
										};
										firstRow = false;
									}
								}
							}
							finally
							{
								((IDisposable)res)?.Dispose();
							}
						}
						finally
						{
							((IDisposable)fetchCmd)?.Dispose();
						}
						NpgsqlCommand closeCmd = new NpgsqlCommand("close \"" + cursorName + "\";", con, tran);
						try
						{
							((DbCommand)(object)closeCmd).ExecuteNonQuery();
						}
						finally
						{
							((IDisposable)closeCmd)?.Dispose();
						}
						((DbTransaction)(object)tran).Commit();
					}
					finally
					{
						((IDisposable)cmd)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)tran)?.Dispose();
				}
				return noticeList;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)base.cmd).Dispose();
			}
		}

		public List<main.tourProposalAll> getAllTourList(int? page = null, int? limit = null, string type = null, int? id = null, int? managerFilter = null, int? projectFilter = null)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Expected O, but got Unknown
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Expected O, but got Unknown
			try
			{
				List<main.tourProposalAll> getlist = new List<main.tourProposalAll>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageTourProposal_cursor", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"selectAlltour");
						cmd.Parameters.AddWithValue("v_projectmanager", (object)(managerFilter ?? new int?(0)));
						cmd.Parameters.AddWithValue("v_id", (object)(id ?? new int?(0)));
						cmd.Parameters.AddWithValue("v_type", (object)(string.IsNullOrEmpty(type) ? "AllData" : type));
						cmd.Parameters.AddWithValue("v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("v_projectid", (object)(projectFilter ?? new int?(0)));
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("FETCH ALL FROM \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader res = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)res).HasRows)
								{
									bool firstRow = true;
									while (((DbDataReader)(object)res).Read())
									{
										main.tourProposalAll tourData = new main.tourProposalAll
										{
											id = Convert.ToInt32(((DbDataReader)(object)res)["id"]),
											projectName = ((DbDataReader)(object)res)["title"].ToString(),
											projectManager = ((DbDataReader)(object)res)["name"].ToString(),
											dateOfDept = Convert.ToDateTime(((DbDataReader)(object)res)["dateOfDept"]),
											place = ((DbDataReader)(object)res)["place"].ToString(),
											periodFrom = Convert.ToDateTime(((DbDataReader)(object)res)["periodFrom"]),
											periodTo = Convert.ToDateTime(((DbDataReader)(object)res)["periodTo"]),
											returnDate = Convert.ToDateTime(((DbDataReader)(object)res)["returnDate"]),
											purpose = ((DbDataReader)(object)res)["purpose"].ToString(),
											projectCode = ((((DbDataReader)(object)res)["projectCode"] != DBNull.Value) ? ((DbDataReader)(object)res)["projectCode"].ToString() : "N/A")
										};
										if (firstRow)
										{
											tourData.Pagination = new ApiCommon.PaginationInfo
											{
												TotalPages = Convert.ToInt32(((DbDataReader)(object)res)["totalpages"]),
												TotalRecords = Convert.ToInt32(((DbDataReader)(object)res)["totalrecords"]),
												PageNumber = page.GetValueOrDefault(),
												PageSize = limit.GetValueOrDefault()
											};
											firstRow = false;
										}
										getlist.Add(tourData);
									}
								}
							}
							finally
							{
								((IDisposable)res)?.Dispose();
							}
						}
						finally
						{
							((IDisposable)fetchCmd)?.Dispose();
						}
						NpgsqlCommand closeCmd = new NpgsqlCommand("CLOSE \"" + cursorName + "\";", con, tran);
						try
						{
							((DbCommand)(object)closeCmd).ExecuteNonQuery();
						}
						finally
						{
							((IDisposable)closeCmd)?.Dispose();
						}
						((DbTransaction)(object)tran).Commit();
					}
					finally
					{
						((IDisposable)cmd)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)tran)?.Dispose();
				}
				return getlist;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)base.cmd).Dispose();
			}
		}

		public bool Tourapproval(int id, bool status, string remark)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			try
			{
				NpgsqlCommand cmd = new NpgsqlCommand("CALL sp_Tourproposal(v_remark=>@v_remark,v_id=>@v_id,v_adminappr=>@v_adminappr, v_action=>@v_action)", con);
				cmd.Parameters.AddWithValue("@v_action", (object)"approval");
				cmd.Parameters.AddWithValue("@v_adminappr", (object)status);
				cmd.Parameters.AddWithValue("@v_id", (object)id);
				cmd.Parameters.AddWithValue("@v_remark", (object)(status ? "" : remark));
				((DbConnection)(object)con).Open();
				((DbCommand)(object)cmd).ExecuteNonQuery();
				return true;
			}
			catch
			{
				return false;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)base.cmd).Dispose();
			}
		}

		public bool ReimbursementApproval(bool status, int id, string type, string remark)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("call sp_reimbursement(p_action=>@p_action,p_admin_appr=>@p_admin_appr,p_id=>@p_id,p_type=>@p_type,p_remark=>@p_remark)", con);
				cmd.Parameters.AddWithValue("@p_action", (object)"approval");
				cmd.Parameters.AddWithValue("@p_admin_appr", (object)status);
				cmd.Parameters.AddWithValue("@p_id", (object)id);
				cmd.Parameters.AddWithValue("@p_type", (object)type);
				cmd.Parameters.AddWithValue("@p_remark", (object)(status ? "" : remark));
				((DbConnection)(object)con).Open();
				((DbCommand)(object)cmd).ExecuteNonQuery();
				return true;
			}
			catch
			{
				return false;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public List<main.AdminReimbursement> GetSpecificUserReimbursements(int id, string type)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_Reimbursement", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"GetAllSubmittedData");
				cmd.Parameters.AddWithValue("@userId", (object)id);
				cmd.Parameters.AddWithValue("@type", (object)type);
				((DbConnection)(object)con).Open();
				List<main.AdminReimbursement> getlist = new List<main.AdminReimbursement>();
				NpgsqlDataReader res = cmd.ExecuteReader();
				if (((DbDataReader)(object)res).HasRows)
				{
					while (((DbDataReader)(object)res).Read())
					{
						getlist.Add(new main.AdminReimbursement
						{
							id = (int)((DbDataReader)(object)res)["id"],
							type = (string)((DbDataReader)(object)res)["type"],
							vrNo = (string)((DbDataReader)(object)res)["vrNo"],
							date = Convert.ToDateTime(((DbDataReader)(object)res)["date"]),
							particulars = (string)((DbDataReader)(object)res)["particulars"],
							items = (string)((DbDataReader)(object)res)["items"],
							amount = Convert.ToDecimal(((DbDataReader)(object)res)["amount"]),
							purpose = (string)((DbDataReader)(object)res)["purpose"],
							status = Convert.ToBoolean(((DbDataReader)(object)res)["status"]),
							newRequest = (bool)((DbDataReader)(object)res)["newRequest"],
							adminappr = (bool)((DbDataReader)(object)res)["admin_appr"]
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
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public List<main.HiringVehicle1> HiringList(int? page = null, int? limit = null, int? managerFilter = null, int? projectFilter = null)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Expected O, but got Unknown
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<main.HiringVehicle1> list = new List<main.HiringVehicle1>();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageHiringVehicle_cursor", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"selectAllHiring");
						cmd.Parameters.AddWithValue("v_projectmanager", (object)(managerFilter ?? new int?(0)));
						cmd.Parameters.AddWithValue("v_projectid", (object)(projectFilter ?? new int?(0)));
						cmd.Parameters.AddWithValue("v_type", (object)"AllData");
						cmd.Parameters.AddWithValue("v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("FETCH ALL FROM \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader res = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)res).HasRows)
								{
									bool firstRow = true;
									while (((DbDataReader)(object)res).Read())
									{
										main.HiringVehicle1 hiringVehicle = new main.HiringVehicle1
										{
											id = (int)((DbDataReader)(object)res)["id"],
											projectName = Convert.ToString(((DbDataReader)(object)res)["title"]),
											projectManager = Convert.ToString(((DbDataReader)(object)res)["name"]),
											headName = Convert.ToString(((DbDataReader)(object)res)["heads"]),
											amount = Convert.ToDecimal(((DbDataReader)(object)res)["amount"]),
											dateFrom = Convert.ToDateTime(((DbDataReader)(object)res)["dateFrom"]),
											dateTo = Convert.ToDateTime(((DbDataReader)(object)res)["dateTo"]),
											proposedPlace = ((DbDataReader)(object)res)["proposedPlace"].ToString(),
											purposeOfVisit = ((DbDataReader)(object)res)["purposeOfVisit"].ToString(),
											totalDaysNight = ((DbDataReader)(object)res)["totalDaysNight"].ToString(),
											totalPlainHills = ((DbDataReader)(object)res)["totalPlainHills"].ToString(),
											taxi = ((DbDataReader)(object)res)["taxi"].ToString(),
											BookAgainstCentre = ((DbDataReader)(object)res)["BookAgainstCentre"].ToString(),
											availbilityOfFund = ((DbDataReader)(object)res)["availbilityOfFund"].ToString(),
											note = ((DbDataReader)(object)res)["note"].ToString(),
											newRequest = Convert.ToBoolean(((DbDataReader)(object)res)["newRequest"]),
											adminappr = Convert.ToBoolean(((DbDataReader)(object)res)["adminappr"]),
											projectCode = ((((DbDataReader)(object)res)["projectCode"] != DBNull.Value) ? ((DbDataReader)(object)res)["projectCode"].ToString() : "N/A")
										};
										if (firstRow)
										{
											hiringVehicle.Pagination = new ApiCommon.PaginationInfo
											{
												TotalPages = Convert.ToInt32(((DbDataReader)(object)res)["totalpages"]),
												TotalRecords = Convert.ToInt32(((DbDataReader)(object)res)["totalrecords"]),
												PageNumber = page.GetValueOrDefault(),
												PageSize = limit.GetValueOrDefault()
											};
											firstRow = false;
										}
										list.Add(hiringVehicle);
									}
								}
							}
							finally
							{
								((IDisposable)res)?.Dispose();
							}
						}
						finally
						{
							((IDisposable)fetchCmd)?.Dispose();
						}
						NpgsqlCommand closeCmd = new NpgsqlCommand("CLOSE \"" + cursorName + "\";", con, tran);
						try
						{
							((DbCommand)(object)closeCmd).ExecuteNonQuery();
						}
						finally
						{
							((IDisposable)closeCmd)?.Dispose();
						}
						((DbTransaction)(object)tran).Commit();
					}
					finally
					{
						((IDisposable)cmd)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)tran)?.Dispose();
				}
				return list;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)base.cmd).Dispose();
			}
		}

		public async Task<string> GetCityStateAsync(string latitude, string longitude)
		{
			try
			{
				string url = "https://nominatim.openstreetmap.org/reverse?lat=" + latitude + "&lon=" + longitude + "&format=json";
				HttpClient client = new HttpClient();
				try
				{
					client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
					((HttpHeaders)client.DefaultRequestHeaders).Add("Accept-Language", "en");
					JObject json = JObject.Parse(await client.GetStringAsync(url).ConfigureAwait(continueOnCapturedContext: false));
					JToken obj = json["address"];
					object obj2 = ((obj == null) ? null : ((object)obj[(object)"city"])?.ToString());
					if (obj2 == null)
					{
						JToken obj3 = json["address"];
						obj2 = ((obj3 == null) ? null : ((object)obj3[(object)"town"])?.ToString());
						if (obj2 == null)
						{
							JToken obj4 = json["address"];
							obj2 = ((obj4 == null) ? null : ((object)obj4[(object)"village"])?.ToString());
							if (obj2 == null)
							{
								JToken obj5 = json["address"];
								obj2 = ((obj5 == null) ? null : ((object)obj5[(object)"hamlet"])?.ToString()) ?? "";
							}
						}
					}
					string city = (string)obj2;
					JToken obj6 = json["address"];
					string state = ((obj6 == null) ? null : ((object)obj6[(object)"state"])?.ToString()) ?? "";
					return (city + ", " + state).Trim(',', ' ');
				}
				finally
				{
					((IDisposable)client)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				throw ex2;
			}
		}

		public bool HiringApproval(int id, bool status, string remark, string location)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("CALL sp_HiringVehicle(v_id=>@v_id,v_adminappr=>@v_adminappr,v_remark => @v_remark, v_address=>@v_address,v_action=>@v_action )", con);
				cmd.Parameters.AddWithValue("@v_action", (object)"approval");
				cmd.Parameters.AddWithValue("@v_adminappr", (object)status);
				cmd.Parameters.AddWithValue("@v_id", (object)id);
				cmd.Parameters.AddWithValue("@v_remark", (object)(status ? "" : remark));
				cmd.Parameters.AddWithValue("@v_address", (object)location);
				((DbConnection)(object)con).Open();
				((DbCommand)(object)cmd).ExecuteNonQuery();
				return true;
			}
			catch
			{
				return false;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public List<main.BudgetForGraph> BudgetForGraph()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_ManageDashboard", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"BudgetGraph");
				List<main.BudgetForGraph> list = new List<main.BudgetForGraph>();
				((DbConnection)(object)con).Open();
				NpgsqlDataReader res = cmd.ExecuteReader();
				if (((DbDataReader)(object)res).HasRows)
				{
					while (((DbDataReader)(object)res).Read())
					{
						list.Add(new main.BudgetForGraph
						{
							budget = Convert.ToDecimal(((DbDataReader)(object)res)["budget"]),
							month = Convert.ToString(((DbDataReader)(object)res)["months"])
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
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public List<main.HiringVehicle1> HiringReort(int? limit = null, int? page = null, int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Expected O, but got Unknown
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<main.HiringVehicle1> list = new List<main.HiringVehicle1>();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_managehiringvehicle_cursor", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@v_action", (object)"selectAllHiringReport");
						cmd.Parameters.AddWithValue("v_projectmanager", (object)(managerFilter ?? new int?(0)));
						cmd.Parameters.AddWithValue("v_id", (object)(projectFilter ?? new int?(0)));
						cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("v_statusfilter", (object)(string.IsNullOrEmpty(statusFilter) ? ((IConvertible)DBNull.Value) : ((IConvertible)statusFilter)));
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader res = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)res).HasRows)
								{
									bool firstRow = true;
									while (((DbDataReader)(object)res).Read())
									{
										list.Add(new main.HiringVehicle1
										{
											id = (int)((DbDataReader)(object)res)["id"],
											projectId = (int)((DbDataReader)(object)res)["projectId"],
											projectName = Convert.ToString(((DbDataReader)(object)res)["title"]),
											headName = Convert.ToString(((DbDataReader)(object)res)["heads"]),
											amount = Convert.ToDecimal(((DbDataReader)(object)res)["amount"]),
											dateFrom = Convert.ToDateTime(((DbDataReader)(object)res)["dateFrom"]),
											dateTo = Convert.ToDateTime(((DbDataReader)(object)res)["dateTo"]),
											proposedPlace = ((DbDataReader)(object)res)["proposedPlace"].ToString(),
											purposeOfVisit = ((DbDataReader)(object)res)["purposeOfVisit"].ToString(),
											totalDaysNight = ((DbDataReader)(object)res)["totalDaysNight"].ToString(),
											totalPlainHills = ((DbDataReader)(object)res)["totalPlainHills"].ToString(),
											taxi = ((DbDataReader)(object)res)["taxi"].ToString(),
											BookAgainstCentre = ((DbDataReader)(object)res)["BookAgainstCentre"].ToString(),
											availbilityOfFund = ((DbDataReader)(object)res)["availbilityOfFund"].ToString(),
											note = ((DbDataReader)(object)res)["note"].ToString(),
											remark = ((DbDataReader)(object)res)["remark"].ToString(),
											projectCode = ((((DbDataReader)(object)res)["projectCode"] != DBNull.Value) ? ((DbDataReader)(object)res)["projectCode"].ToString() : "N/A")
										});
										if (firstRow)
										{
											list[0].Pagination = new ApiCommon.PaginationInfo
											{
												PageNumber = page.GetValueOrDefault(),
												TotalPages = Convert.ToInt32((((DbDataReader)(object)res)["totalpages"] != DBNull.Value) ? ((DbDataReader)(object)res)["totalpages"] : ((object)0)),
												TotalRecords = Convert.ToInt32((((DbDataReader)(object)res)["totalrecords"] != DBNull.Value) ? ((DbDataReader)(object)res)["totalrecords"] : ((object)0)),
												PageSize = limit.GetValueOrDefault()
											};
											firstRow = false;
										}
									}
								}
							}
							finally
							{
								((IDisposable)res)?.Dispose();
							}
						}
						finally
						{
							((IDisposable)fetchCmd)?.Dispose();
						}
						NpgsqlCommand closeCmd = new NpgsqlCommand("close \"" + cursorName + "\";", con, tran);
						try
						{
							((DbCommand)(object)closeCmd).ExecuteNonQuery();
						}
						finally
						{
							((IDisposable)closeCmd)?.Dispose();
						}
						((DbTransaction)(object)tran).Commit();
					}
					finally
					{
						((IDisposable)cmd)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)tran)?.Dispose();
				}
				return list;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)base.cmd).Dispose();
			}
		}

		public List<main.HiringVehicle1> hiringreportprojects()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_HiringVehicle", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"selecthiringreportprojects");
				((DbConnection)(object)con).Open();
				List<main.HiringVehicle1> list = new List<main.HiringVehicle1>();
				NpgsqlDataReader res = cmd.ExecuteReader();
				if (((DbDataReader)(object)res).HasRows)
				{
					while (((DbDataReader)(object)res).Read())
					{
						list.Add(new main.HiringVehicle1
						{
							id = Convert.ToInt32(((DbDataReader)(object)res)["projectId"]),
							projectName = Convert.ToString(((DbDataReader)(object)res)["title"]),
							employeecode = ((DbDataReader)(object)res)["empcode"].ToString(),
							projectManager = ((DbDataReader)(object)res)["empname"].ToString()
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
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public List<main.HiringVehicle1> hiringreportbyproject(int projectid)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_HiringVehicle", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"selecthiringreportbyproject");
				cmd.Parameters.AddWithValue("@projectId", (object)projectid);
				((DbConnection)(object)con).Open();
				List<main.HiringVehicle1> list = new List<main.HiringVehicle1>();
				NpgsqlDataReader res = cmd.ExecuteReader();
				if (((DbDataReader)(object)res).HasRows)
				{
					while (((DbDataReader)(object)res).Read())
					{
						list.Add(new main.HiringVehicle1
						{
							id = (int)((DbDataReader)(object)res)["id"],
							projectName = Convert.ToString(((DbDataReader)(object)res)["title"]),
							projectManager = Convert.ToString(((DbDataReader)(object)res)["name"]),
							headName = Convert.ToString(((DbDataReader)(object)res)["heads"]),
							amount = Convert.ToDecimal(((DbDataReader)(object)res)["amount"]),
							dateFrom = Convert.ToDateTime(((DbDataReader)(object)res)["dateFrom"]),
							dateTo = Convert.ToDateTime(((DbDataReader)(object)res)["dateTo"]),
							proposedPlace = ((DbDataReader)(object)res)["proposedPlace"].ToString(),
							purposeOfVisit = ((DbDataReader)(object)res)["purposeOfVisit"].ToString(),
							totalDaysNight = ((DbDataReader)(object)res)["totalDaysNight"].ToString(),
							totalPlainHills = ((DbDataReader)(object)res)["totalPlainHills"].ToString(),
							taxi = ((DbDataReader)(object)res)["taxi"].ToString(),
							BookAgainstCentre = ((DbDataReader)(object)res)["BookAgainstCentre"].ToString(),
							availbilityOfFund = ((DbDataReader)(object)res)["availbilityOfFund"].ToString(),
							note = ((DbDataReader)(object)res)["note"].ToString(),
							newRequest = Convert.ToBoolean(((DbDataReader)(object)res)["newRequest"]),
							adminappr = Convert.ToBoolean(((DbDataReader)(object)res)["adminappr"])
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
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public List<main.tourProposalrepo> TourReportProject()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_Tourproposal", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"selecttourproject");
				((DbConnection)(object)con).Open();
				List<main.tourProposalrepo> getlist = new List<main.tourProposalrepo>();
				NpgsqlDataReader res = cmd.ExecuteReader();
				if (((DbDataReader)(object)res).HasRows)
				{
					while (((DbDataReader)(object)res).Read())
					{
						getlist.Add(new main.tourProposalrepo
						{
							id = Convert.ToInt32(((DbDataReader)(object)res)["projectId"]),
							projectName = Convert.ToString(((DbDataReader)(object)res)["title"]),
							employeecode = ((DbDataReader)(object)res)["empcode"].ToString(),
							projectManager = ((DbDataReader)(object)res)["empname"].ToString()
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
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public List<main.tourProposalrepo> tourproposalbyproject(int projectid)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_Tourproposal", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"selecttourproposalbyproject");
				cmd.Parameters.AddWithValue("@projectId", (object)projectid);
				((DbConnection)(object)con).Open();
				List<main.tourProposalrepo> list = new List<main.tourProposalrepo>();
				NpgsqlDataReader res = cmd.ExecuteReader();
				if (((DbDataReader)(object)res).HasRows)
				{
					while (((DbDataReader)(object)res).Read())
					{
						list.Add(new main.tourProposalrepo
						{
							id = Convert.ToInt32(((DbDataReader)(object)res)["projectId"]),
							projectManager = Convert.ToString(((DbDataReader)(object)res)["name"]),
							projectName = Convert.ToString(((DbDataReader)(object)res)["title"]),
							dateOfDept = Convert.ToDateTime(((DbDataReader)(object)res)["dateOfDept"]),
							place = Convert.ToString(((DbDataReader)(object)res)["place"]),
							periodFrom = Convert.ToDateTime(((DbDataReader)(object)res)["periodFrom"]),
							periodTo = Convert.ToDateTime(((DbDataReader)(object)res)["periodTo"]),
							returnDate = Convert.ToDateTime(((DbDataReader)(object)res)["returnDate"]),
							purpose = Convert.ToString(((DbDataReader)(object)res)["purpose"]),
							newRequest = Convert.ToBoolean(((DbDataReader)(object)res)["newRequest"]),
							adminappr = Convert.ToBoolean(((DbDataReader)(object)res)["adminappr"])
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
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)cmd).Dispose();
			}
		}

		public List<main.RaisedProblem> getProblemList(int? page = null, int? limit = null, int? id = null, int? managerId = null, string searchTerm = null)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Expected O, but got Unknown
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Expected O, but got Unknown
			try
			{
				List<main.RaisedProblem> list = new List<main.RaisedProblem>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageproblems_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"selectProblemsforAdmin");
						cmd.Parameters.AddWithValue("v_projectmanager", (object)(managerId ?? new int?(0)));
						cmd.Parameters.AddWithValue("v_id", (object)(id ?? new int?(0)));
						cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_searchterm", (object)(string.IsNullOrEmpty(searchTerm) ? ((IConvertible)DBNull.Value) : ((IConvertible)searchTerm)));
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\"", con, tran);
						try
						{
							NpgsqlDataReader res = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)res).HasRows)
								{
									bool firstRow = true;
									while (((DbDataReader)(object)res).Read())
									{
										list.Add(new main.RaisedProblem
										{
											id = Convert.ToInt32(((DbDataReader)(object)res)["id"]),
											title = ((DbDataReader)(object)res)["title"].ToString(),
											description = ((DbDataReader)(object)res)["description"].ToString(),
											adminappr = Convert.ToBoolean(((DbDataReader)(object)res)["adminappr"]),
											newRequest = Convert.ToBoolean(((DbDataReader)(object)res)["newRequest"]),
											documentname = ((DbDataReader)(object)res)["document"].ToString(),
											projectname = ((DbDataReader)(object)res)["projectName"].ToString(),
											projectManager = ((DbDataReader)(object)res)["projectManager"].ToString(),
											createdAt = Convert.ToDateTime(((DbDataReader)(object)res)["createdAt"]),
											projectCode = ((((DbDataReader)(object)res)["projectCode"] != DBNull.Value) ? ((DbDataReader)(object)res)["projectCode"].ToString() : "N/A")
										});
										if (firstRow)
										{
											list[0].Pagination = new ApiCommon.PaginationInfo
											{
												PageNumber = page.GetValueOrDefault(),
												TotalPages = Convert.ToInt32((((DbDataReader)(object)res)["totalpages"] != DBNull.Value) ? ((DbDataReader)(object)res)["totalpages"] : ((object)0)),
												TotalRecords = Convert.ToInt32((((DbDataReader)(object)res)["totalrecords"] != DBNull.Value) ? ((DbDataReader)(object)res)["totalrecords"] : ((object)0)),
												PageSize = limit.GetValueOrDefault()
											};
											firstRow = false;
										}
									}
								}
							}
							finally
							{
								((IDisposable)res)?.Dispose();
							}
						}
						finally
						{
							((IDisposable)fetchCmd)?.Dispose();
						}
						NpgsqlCommand closeCmd = new NpgsqlCommand("close \"" + cursorName + "\";", con, tran);
						try
						{
							((DbCommand)(object)closeCmd).ExecuteNonQuery();
						}
						finally
						{
							((IDisposable)closeCmd)?.Dispose();
						}
						((DbTransaction)(object)tran).Commit();
					}
					finally
					{
						((IDisposable)cmd)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)tran)?.Dispose();
				}
				return list;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)base.cmd).Dispose();
			}
		}

		public List<main.RaisedProblem> getproblembyId(int id)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Expected O, but got Unknown
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Expected O, but got Unknown
			try
			{
				List<main.RaisedProblem> list = new List<main.RaisedProblem>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageproblems_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@v_action", (object)"selectProblemsforAdminById");
						cmd.Parameters.AddWithValue("@v_projectmanager", (object)0);
						cmd.Parameters.AddWithValue("@v_id", (object)id);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\"", con, tran);
						try
						{
							NpgsqlDataReader res = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)res).HasRows)
								{
									while (((DbDataReader)(object)res).Read())
									{
										list.Add(new main.RaisedProblem
										{
											id = Convert.ToInt32(((DbDataReader)(object)res)["id"]),
											title = ((DbDataReader)(object)res)["title"].ToString(),
											description = ((DbDataReader)(object)res)["description"].ToString(),
											adminappr = Convert.ToBoolean(((DbDataReader)(object)res)["adminappr"]),
											newRequest = Convert.ToBoolean(((DbDataReader)(object)res)["newRequest"]),
											documentname = ((DbDataReader)(object)res)["document"].ToString(),
											projectname = ((DbDataReader)(object)res)["projectName"].ToString(),
											projectManager = ((DbDataReader)(object)res)["projectManager"].ToString()
										});
									}
								}
							}
							finally
							{
								((IDisposable)res)?.Dispose();
							}
						}
						finally
						{
							((IDisposable)fetchCmd)?.Dispose();
						}
						NpgsqlCommand closeCmd = new NpgsqlCommand("close \"" + cursorName + "\"", con, tran);
						try
						{
							((DbCommand)(object)closeCmd).ExecuteNonQuery();
						}
						finally
						{
							((IDisposable)closeCmd)?.Dispose();
						}
						((DbTransaction)(object)tran).Commit();
					}
					finally
					{
						((IDisposable)cmd)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)tran)?.Dispose();
				}
				return list;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)base.cmd).Dispose();
			}
		}

		public List<main.Meeting_Model> getAllmeetingforAdmin()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			try
			{
				List<main.Meeting_Model> _list = new List<main.Meeting_Model>();
				main.Meeting_Model obj = null;
				NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageMeeting", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"getAllmeetingofadmin");
				((DbConnection)(object)con).Open();
				NpgsqlDataReader sdr = cmd.ExecuteReader();
				while (((DbDataReader)(object)sdr).Read())
				{
					obj = new main.Meeting_Model();
					obj.Id = Convert.ToInt32(((DbDataReader)(object)sdr)["id"]);
					obj.CompleteStatus = Convert.ToInt32(((DbDataReader)(object)sdr)["completeStatus"]);
					obj.MeetingType = ((DbDataReader)(object)sdr)["meetingType"].ToString();
					obj.MeetingLink = ((DbDataReader)(object)sdr)["meetingLink"].ToString();
					obj.MeetingTitle = ((DbDataReader)(object)sdr)["MeetingTitle"].ToString();
					obj.memberId = ((((DbDataReader)(object)sdr)["memberId"] != DBNull.Value) ? ((DbDataReader)(object)sdr)["memberId"].ToString().Split(',').ToList() : new List<string>());
					obj.CreaterId = ((((DbDataReader)(object)sdr)["createrId"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)sdr)["createrId"]) : 0);
					obj.MeetingDate = Convert.ToDateTime(((DbDataReader)(object)sdr)["meetingTime"]).ToString("dd-MM-yyyy");
					obj.summary = ((DbDataReader)(object)sdr)["meetSummary"].ToString();
					_list.Add(obj);
					obj.createdBy = ((DbDataReader)(object)sdr)["createdBy"].ToString();
				}
				((DbDataReader)(object)sdr).Close();
				return _list;
			}
			catch (Exception innerException)
			{
				throw new Exception("An error accured", innerException);
			}
			finally
			{
				((DbConnection)(object)con).Close();
			}
		}

		public List<main.Meeting_Model> getAllmeetingforprojectmanager()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			try
			{
				List<main.Meeting_Model> _list = new List<main.Meeting_Model>();
				main.Meeting_Model obj = null;
				NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageMeeting", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"getAllmeetingofprojectmanager");
				((DbConnection)(object)con).Open();
				NpgsqlDataReader sdr = cmd.ExecuteReader();
				while (((DbDataReader)(object)sdr).Read())
				{
					obj = new main.Meeting_Model();
					obj.Id = Convert.ToInt32(((DbDataReader)(object)sdr)["id"]);
					obj.CompleteStatus = Convert.ToInt32(((DbDataReader)(object)sdr)["completeStatus"]);
					obj.MeetingType = ((DbDataReader)(object)sdr)["meetingType"].ToString();
					obj.MeetingLink = ((DbDataReader)(object)sdr)["meetingLink"].ToString();
					obj.MeetingTitle = ((DbDataReader)(object)sdr)["MeetingTitle"].ToString();
					obj.memberId = ((((DbDataReader)(object)sdr)["memberId"] != DBNull.Value) ? ((DbDataReader)(object)sdr)["memberId"].ToString().Split(',').ToList() : new List<string>());
					obj.CreaterId = ((((DbDataReader)(object)sdr)["createrId"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)sdr)["createrId"]) : 0);
					obj.MeetingDate = Convert.ToDateTime(((DbDataReader)(object)sdr)["meetingTime"]).ToString("dd-MM-yyyy");
					obj.summary = ((DbDataReader)(object)sdr)["meetSummary"].ToString();
					_list.Add(obj);
					obj.createdBy = ((DbDataReader)(object)sdr)["createdBy"].ToString();
				}
				((DbDataReader)(object)sdr).Close();
				return _list;
			}
			catch (Exception innerException)
			{
				throw new Exception("An error accured", innerException);
			}
			finally
			{
				((DbConnection)(object)con).Close();
			}
		}

		public List<main.SubProblem> getAllSubOrdinateProblemByIdforadmin(int id)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			try
			{
				List<main.SubProblem> problemList = new List<main.SubProblem>();
				main.SubProblem obj = null;
				NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageSubordinateProjectProblem", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"getallproblembyidforadmin");
				cmd.Parameters.AddWithValue("@id", (object)id);
				((DbConnection)(object)con).Open();
				NpgsqlDataReader sdr = cmd.ExecuteReader();
				if (((DbDataReader)(object)sdr).HasRows)
				{
					while (((DbDataReader)(object)sdr).Read())
					{
						obj = new main.SubProblem();
						obj.ProblemId = Convert.ToInt32(((DbDataReader)(object)sdr)["problemId"]);
						obj.ProjectName = ((DbDataReader)(object)sdr)["ProjectName"].ToString();
						obj.Title = ((DbDataReader)(object)sdr)["Title"].ToString();
						obj.Description = ((DbDataReader)(object)sdr)["Description"].ToString();
						obj.Attchment_Url = ((DbDataReader)(object)sdr)["Attachment"].ToString();
						obj.CreatedDate = Convert.ToDateTime(((DbDataReader)(object)sdr)["CreatedDate"]).ToString("dd-MM-yyyy");
						obj.newRequest = Convert.ToBoolean(((DbDataReader)(object)sdr)["newRequest"]);
						problemList.Add(obj);
					}
				}
				return problemList;
			}
			catch (Exception innerException)
			{
				throw new Exception("An error accured", innerException);
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				((Component)(object)base.cmd).Dispose();
			}
		}

		public List<main.BudgetHeadModel> GetBudgetHeads()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Expected O, but got Unknown
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Expected O, but got Unknown
			try
			{
				List<main.BudgetHeadModel> budgetlist = new List<main.BudgetHeadModel>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageempreport_cursor", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"getbudgetHeads");
						cmd.Parameters.AddWithValue("v_projectmanager", (object)0);
						cmd.Parameters.AddWithValue("v_id", (object)0);
						cmd.Parameters.AddWithValue("v_limit", (object)DBNull.Value);
						cmd.Parameters.AddWithValue("v_page", (object)DBNull.Value);
						cmd.Parameters.AddWithValue("v_year", (object)DBNull.Value);
						cmd.Parameters.AddWithValue("v_month", (object)DBNull.Value);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader res = fetchCmd.ExecuteReader();
							try
							{
								while (((DbDataReader)(object)res).Read())
								{
									budgetlist.Add(new main.BudgetHeadModel
									{
										Id = ((((DbDataReader)(object)res)["id"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)res)["id"]) : 0),
										BudgetHead = ((((DbDataReader)(object)res)["budgethead"] == DBNull.Value) ? null : ((DbDataReader)(object)res)["budgethead"].ToString())
									});
								}
							}
							finally
							{
								((IDisposable)res)?.Dispose();
							}
						}
						finally
						{
							((IDisposable)fetchCmd)?.Dispose();
						}
						NpgsqlCommand closeCmd = new NpgsqlCommand("close \"" + cursorName + "\"", con, tran);
						try
						{
							((DbCommand)(object)closeCmd).ExecuteNonQuery();
						}
						finally
						{
							((IDisposable)closeCmd)?.Dispose();
						}
						((DbTransaction)(object)tran).Commit();
					}
					finally
					{
						((IDisposable)cmd)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)tran)?.Dispose();
				}
				return budgetlist;
			}
			catch (Exception innerException)
			{
				throw new Exception("An error occurred", innerException);
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
				if (base.cmd != null)
				{
					((Component)(object)base.cmd).Dispose();
				}
			}
		}

		public bool InsertCmDashboardProject(main.CMDashboardData cm)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			try
			{
				NpgsqlCommand cmd = new NpgsqlCommand("call sp_manage_cm_dashboard(@p_action, @p_id, @p_project_name, @p_start_year, @p_end_date, @p_remark, @p_budget, @p_status, @p_projectStatus)", con);
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("p_action", (object)cm.db_action);
					NpgsqlParameterCollection parameters = cmd.Parameters;
					int? id = cm.Id;
					parameters.AddWithValue("p_id", id.HasValue ? ((object)id.GetValueOrDefault()) : DBNull.Value);
					cmd.Parameters.AddWithValue("p_project_name", ((object)cm.ProjectTitile) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("p_start_year", (object)cm.StartYear);
					NpgsqlParameterCollection parameters2 = cmd.Parameters;
					DateTime? endDate = cm.EndDate;
					parameters2.AddWithValue("p_end_date", endDate.HasValue ? ((object)endDate.GetValueOrDefault()) : DBNull.Value);
					cmd.Parameters.AddWithValue("p_remark", ((object)cm.Remark) ?? ((object)DBNull.Value));
					NpgsqlParameterCollection parameters3 = cmd.Parameters;
					decimal? budget = cm.Budget;
					parameters3.AddWithValue("p_budget", budget.HasValue ? ((object)budget.GetValueOrDefault()) : DBNull.Value);
					cmd.Parameters.AddWithValue("p_projectStatus", ((object)cm.projectStatus) ?? ((object)DBNull.Value));
					cmd.Parameters.AddWithValue("p_status", (object)true);
					((DbConnection)(object)con).Open();
					((DbCommand)(object)cmd).ExecuteNonQuery();
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		public List<main.CMDashboardData> GetCmDashboardList(int? id = null, bool? status = true, int? startYear = null, string statusFilter = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			List<main.CMDashboardData> list = new List<main.CMDashboardData>();
			try
			{
				NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM fn_get_cm_dashboard(@p_id, @p_status, @p_start_year, @p_project_status)", con);
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					NpgsqlParameterCollection parameters = cmd.Parameters;
					int? num = id;
					parameters.AddWithValue("@p_id", num.HasValue ? ((object)num.GetValueOrDefault()) : DBNull.Value);
					NpgsqlParameterCollection parameters2 = cmd.Parameters;
					bool? flag = status;
					parameters2.AddWithValue("@p_status", flag.HasValue ? ((object)(flag == true)) : DBNull.Value);
					NpgsqlParameterCollection parameters3 = cmd.Parameters;
					num = startYear;
					parameters3.AddWithValue("@p_start_year", num.HasValue ? ((object)num.GetValueOrDefault()) : DBNull.Value);
					cmd.Parameters.AddWithValue("@p_project_status", ((object)statusFilter) ?? ((object)DBNull.Value));
					((DbConnection)(object)con).Open();
					NpgsqlDataReader rd = cmd.ExecuteReader();
					try
					{
						while (((DbDataReader)(object)rd).Read())
						{
							list.Add(new main.CMDashboardData
							{
								Id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
								ProjectTitile = ((DbDataReader)(object)rd)["project_name"]?.ToString(),
								StartYear = Convert.ToInt32(((DbDataReader)(object)rd)["start_year"]),
								EndDate = ((((DbDataReader)(object)rd)["end_date"] == DBNull.Value) ? ((DateTime?)null) : new DateTime?(Convert.ToDateTime(((DbDataReader)(object)rd)["end_date"]))),
								Remark = ((DbDataReader)(object)rd)["remark"]?.ToString(),
								Budget = ((((DbDataReader)(object)rd)["budget"] == DBNull.Value) ? ((decimal?)null) : new decimal?(Convert.ToDecimal(((DbDataReader)(object)rd)["budget"]))),
								projectStatus = ((DbDataReader)(object)rd)["projectStatus"].ToString()
							});
						}
					}
					finally
					{
						((IDisposable)rd)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
			}
			catch
			{
			}
			return list;
		}
	}

}