// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.ProjectManager.ManagerService
using ClosedXML.Excel;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;

namespace RemoteSensingProject.Models.ProjectManager
{
	public class ManagerService : DataFactory
	{
		public DashboardCount DashboardCount(int userId)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Expected O, but got Unknown
			DashboardCount obj = null;
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
						cmd.Parameters.AddWithValue("v_action", (object)"ManagerDashboardCount");
						cmd.Parameters.AddWithValue("v_projectmanager", (object)userId);
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
									obj = new DashboardCount();
									obj.TotalAssignProject = ((DbDataReader)(object)sdr)["TotalAssignProject"].ToString();
									obj.TotaCompleteProject = ((DbDataReader)(object)sdr)["TotalCompleteProject"].ToString();
									obj.TotalDelayProject = ((DbDataReader)(object)sdr)["TotalDelayproject"].ToString();
									obj.TotalNotice = ((DbDataReader)(object)sdr)["TotalNotice"].ToString();
									obj.TotalOngoingProject = ((DbDataReader)(object)sdr)["TotalOngoingProject"].ToString();
									obj.TotalMeeting = ((DbDataReader)(object)sdr)["totalMeetings"].ToString();
									obj.SelfCreatedProject = ((DbDataReader)(object)sdr)["SelfCreatedProject"].ToString();
									obj.EmpMeeting = ((DbDataReader)(object)sdr)["EmpMeeting"].ToString();
									obj.AdminMeeting = ((DbDataReader)(object)sdr)["AdminMeeting"].ToString();
									obj.TotalReimbursement = ((DbDataReader)(object)sdr)["TotalReimbursement"].ToString();
									//obj.TotalTourProposal = ((DbDataReader)(object)sdr)["TotalTourProposal"].ToString();
									//obj.TotalHiring = ((DbDataReader)(object)sdr)["TotalHiring"].ToString();
									obj.ReimbursementPendingRequest = ((DbDataReader)(object)sdr)["ReimbursementPendingRequest"].ToString();
									obj.ReimbursementApprovedRequest = ((DbDataReader)(object)sdr)["ReimbursementApprovedRequest"].ToString();
									obj.ReimbursementRejectedRequest = ((DbDataReader)(object)sdr)["ReimbursementRejectedRequest"].ToString();
									//obj.TourPendingRequest = ((DbDataReader)(object)sdr)["TourPendingRequest"].ToString();
									//obj.TourApprovedRequest = ((DbDataReader)(object)sdr)["TourApprovedRequest"].ToString();
									//obj.TourRejectedRequest = ((DbDataReader)(object)sdr)["TourRejectedRequest"].ToString();
									//obj.HiringPendingRequest = ((DbDataReader)(object)sdr)["HiringPendingRequest"].ToString();
									//obj.HiringApprovedRequest = ((DbDataReader)(object)sdr)["HiringApprovedRequest"].ToString();
									//obj.HiringRejectedRequest = ((DbDataReader)(object)sdr)["HiringRejectedRequest"].ToString();
									obj.TotalTask = ((DbDataReader)(object)sdr)["TotalTask"].ToString();
									obj.CompletedTask = ((DbDataReader)(object)sdr)["CompletedTask"].ToString();
									obj.OutSource = ((DbDataReader)(object)sdr)["OutSource"].ToString();
									obj.ProjectProblem = ((DbDataReader)(object)sdr)["ProjectProblem"].ToString();
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
				throw new Exception("An error accured", innerException);
			}
			finally
			{
				((DbConnection)(object)con).Close();
			}
		}

		public UserCredential getManagerDetails(string managerName)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			UserCredential _details = new UserCredential();
			try
			{
				((DbConnection)(object)con).Open();
				NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM sp_manageloginmaster(@action, @userid, @username)", con);
				try
				{
					cmd.Parameters.AddWithValue("@action", (object)"selectprofile");
					cmd.Parameters.AddWithValue("@username", (object)managerName);
					cmd.Parameters.AddWithValue("@userid", (object)0);
					NpgsqlDataReader sdr = cmd.ExecuteReader();
					try
					{
						if (((DbDataReader)(object)sdr).HasRows)
						{
							((DbDataReader)(object)sdr).Read();
							_details = new UserCredential();
							_details.username = ((DbDataReader)(object)sdr)["username"].ToString();
							_details.userId = ((DbDataReader)(object)sdr)["userid"].ToString();
							_details.userRole = ((DbDataReader)(object)sdr)["userRole"].ToString();
							_details.divisionId = Convert.ToInt32(sdr["emp_id"]);
						}
					}
					finally
					{
						((IDisposable)sdr)?.Dispose();
					}
					return _details;
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
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

		public bool addManagerProject(RemoteSensingProject.Models.Admin.main.createProjectModel pm)
		{
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Expected O, but got Unknown
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Expected O, but got Unknown
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Expected O, but got Unknown
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Expected O, but got Unknown
			//IL_0680: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Expected O, but got Unknown
			((DbConnection)(object)con).Open();
			NpgsqlTransaction tran = con.BeginTransaction();
			try
			{
				Random rand = new Random();
				string projectCode = $"{rand.Next(1000, 9999).ToString()}{DateTime.Now.Day}{DateTime.Now.Year.ToString().Substring(2, 2)}";
				cmd = new NpgsqlCommand("sp_adminAddproject", con, tran);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"insertProject");
				cmd.Parameters.AddWithValue("@title", (object)pm.pm.ProjectTitle);
				cmd.Parameters.AddWithValue("@assignDate", (object)pm.pm.AssignDate);
				cmd.Parameters.AddWithValue("@startDate", (object)pm.pm.StartDate);
				cmd.Parameters.AddWithValue("@completionDate", (object)pm.pm.CompletionDate);
				cmd.Parameters.AddWithValue("@projectmanager", (object)pm.pm.ProjectManager);
				cmd.Parameters.AddWithValue("@budget", (object)pm.pm.ProjectBudget);
				cmd.Parameters.AddWithValue("@description", (object)pm.pm.ProjectDescription);
				cmd.Parameters.AddWithValue("@ProjectDocument", (object)pm.pm.projectDocumentUrl);
				cmd.Parameters.AddWithValue("@projectType", (object)pm.pm.ProjectType);
				cmd.Parameters.AddWithValue("@stage", (object)pm.pm.ProjectStage);
				cmd.Parameters.AddWithValue("@createdBy", (object)"projectManager");
				cmd.Parameters.AddWithValue("@projectCode", (object)projectCode);
				cmd.Parameters.AddWithValue("@ApproveStatus", (object)1);
				cmd.Parameters.Add("@project_Id", (NpgsqlDbType)9);
				((DbParameter)(object)cmd.Parameters["@project_Id"]).Direction = ParameterDirection.Output;
				int i = ((DbCommand)(object)cmd).ExecuteNonQuery();
				int projectId = Convert.ToInt32((((DbParameter)(object)cmd.Parameters["@project_Id"]).Value != DBNull.Value) ? ((DbParameter)(object)cmd.Parameters["@project_Id"]).Value : ((object)0));
				if (i > 0)
				{
					if (pm.budgets != null && pm.budgets.Count > 0)
					{
						foreach (RemoteSensingProject.Models.Admin.main.Project_Budget item in pm.budgets)
						{
							cmd = new NpgsqlCommand("sp_adminAddproject", con, tran);
							((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
							cmd.Parameters.AddWithValue("@action", (object)"insertProjectBudget");
							cmd.Parameters.AddWithValue("@project_Id", (object)projectId);
							cmd.Parameters.AddWithValue("@heads", (object)item.ProjectHeads);
							cmd.Parameters.AddWithValue("@headsAmount", (object)item.ProjectAmount);
							i += ((DbCommand)(object)cmd).ExecuteNonQuery();
						}
					}
					if (pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage)
					{
						foreach (RemoteSensingProject.Models.Admin.main.Project_Statge item2 in pm.stages)
						{
							cmd = new NpgsqlCommand("sp_adminAddproject", con, tran);
							((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
							cmd.Parameters.AddWithValue("@action", (object)"insertProjectStatge");
							cmd.Parameters.AddWithValue("@project_Id", (object)projectId);
							cmd.Parameters.AddWithValue("@keyPoint", (object)item2.KeyPoint);
							cmd.Parameters.AddWithValue("@completeDate", (object)item2.CompletionDate);
							cmd.Parameters.AddWithValue("@stageDocument", (object)item2.Document_Url);
							i += ((DbCommand)(object)cmd).ExecuteNonQuery();
						}
					}
					if (pm.pm.ProjectType.Equals("External") && projectId > 0)
					{
						cmd = new NpgsqlCommand("sp_adminAddproject", con, tran);
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@action", (object)"insertExternalProject");
						cmd.Parameters.AddWithValue("@project_Id", (object)projectId);
						cmd.Parameters.AddWithValue("@DepartmentName", (object)pm.pm.ProjectDepartment);
						cmd.Parameters.AddWithValue("@contactPerson", (object)pm.pm.ContactPerson);
						cmd.Parameters.AddWithValue("@address", (object)pm.pm.Address);
						i += ((DbCommand)(object)cmd).ExecuteNonQuery();
					}
					if (pm.pm.SubOrdinate != null && pm.pm.SubOrdinate.Length != 0)
					{
						int[] subOrdinate = pm.pm.SubOrdinate;
						foreach (int item3 in subOrdinate)
						{
							cmd = new NpgsqlCommand("sp_adminAddproject", con, tran);
							((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
							cmd.Parameters.AddWithValue("@action", (object)"insertSubOrdinate");
							cmd.Parameters.AddWithValue("@project_Id", (object)projectId);
							cmd.Parameters.AddWithValue("@id", (object)item3);
							cmd.Parameters.AddWithValue("@projectmanager", (object)pm.pm.ProjectManager);
							i += ((DbCommand)(object)cmd).ExecuteNonQuery();
						}
					}
				}
				((DbTransaction)(object)tran).Commit();
				return i > 0;
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

		public List<RemoteSensingProject.Models.Admin.main.Project_model> All_Project_List(int? userId, int? limit = null, int? page = null, string filterType = null, int? id = null, string searchTerm = null, string statusFilter = null)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<RemoteSensingProject.Models.Admin.main.Project_model> list = new List<RemoteSensingProject.Models.Admin.main.Project_model>();
				cmd = new NpgsqlCommand("SELECT * FROM fn_get_all_projects(@action,@v_id,@v_projectManager,@v_filterType,@v_limit,@v_page,@v_searchTerm,@v_statusFilter)", con);
				cmd.Parameters.AddWithValue("@action", (object)"GetAllProject");
				cmd.Parameters.AddWithValue("@v_projectManager", userId.HasValue ? ((object)userId) : ((object)0));
				if (string.IsNullOrWhiteSpace(filterType))
				{
					cmd.Parameters.AddWithValue("@v_filterType", (object)DBNull.Value);
				}
				else
				{
					cmd.Parameters.AddWithValue("@v_filterType", (object)filterType);
				}
				cmd.Parameters.AddWithValue("@v_id", (object)(id ?? new int?(0)));
				cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
				cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
				cmd.Parameters.AddWithValue("@v_searchTerm", (object)(string.IsNullOrEmpty(searchTerm) ? ((IConvertible)DBNull.Value) : ((IConvertible)searchTerm)));
				cmd.Parameters.AddWithValue("@v_statusFilter", (object)(string.IsNullOrEmpty(statusFilter) ? ((IConvertible)DBNull.Value) : ((IConvertible)statusFilter)));
				NpgsqlDataReader rd = cmd.ExecuteReader();
				if (((DbDataReader)(object)rd).HasRows)
				{
					while (((DbDataReader)(object)rd).Read())
					{
						bool firstRow = true;
						list.Add(new RemoteSensingProject.Models.Admin.main.Project_model
						{
							Id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
							ProjectTitle = ((DbDataReader)(object)rd)["title"].ToString(),
							AssignDate = (((DbDataReader)(object)rd)["assignDate"] as DateTime?),
							CompletionDate = (((DbDataReader)(object)rd)["completionDate"] as DateTime?),
							devisionName = ((DbDataReader)(object)rd)["devisionname"].ToString(),
							StartDate = (((DbDataReader)(object)rd)["startDate"] as DateTime?),
							ApproveStatus = (bool)((DbDataReader)(object)rd)["ApproveStatus"],
							ProjectManager = ((DbDataReader)(object)rd)["name"].ToString(),
							ProjectBudget = Convert.ToDecimal(((DbDataReader)(object)rd)["budget"]),
							ProjectDescription = ((DbDataReader)(object)rd)["description"].ToString(),
							projectDocumentUrl = ((DbDataReader)(object)rd)["ProjectDocument"].ToString(),
							ProjectType = ((DbDataReader)(object)rd)["projectType"].ToString(),
							ProjectStage = Convert.ToBoolean(((DbDataReader)(object)rd)["stage"]),
							CompletionDatestring = ((((DbDataReader)(object)rd)["completionDate"] != DBNull.Value) ? Convert.ToDateTime(((DbDataReader)(object)rd)["completionDate"]).ToString("dd-MM-yyyy") : "N/A"),
							ProjectStatus = Convert.ToBoolean(((DbDataReader)(object)rd)["CompleteStatus"]),
							AssignDateString = ((((DbDataReader)(object)rd)["assignDate"] != DBNull.Value) ? Convert.ToDateTime(((DbDataReader)(object)rd)["assignDate"]).ToString("dd-MM-yyyy") : "N/A"),
							StartDateString = ((((DbDataReader)(object)rd)["startDate"] != DBNull.Value) ? Convert.ToDateTime(((DbDataReader)(object)rd)["startDate"]).ToString("dd-MM-yyyy") : "N/A"),
							createdBy = ((DbDataReader)(object)rd)["createdBy"].ToString(),
							Percentage = ((DbDataReader)(object)rd)["financialStatusPercentage"].ToString(),
							physicalcomplete = Convert.ToDecimal(((DbDataReader)(object)rd)["completionPercentage"]),
							overallPercentage = Convert.ToDecimal(((DbDataReader)(object)rd)["overallPercentage"]),
							completestatus = Convert.ToBoolean(((DbDataReader)(object)rd)["CompleteStatus"]),
							projectCode = ((((DbDataReader)(object)rd)["projectCode"] != DBNull.Value) ? ((DbDataReader)(object)rd)["projectCode"].ToString() : "N/A"),
							ProjectDepartment = ((((DbDataReader)(object)rd)["projectType"].ToString().ToLower() == "external") ? ((DbDataReader)(object)rd)["departmentname"].ToString() : string.Empty),
							ContactPerson = ((((DbDataReader)(object)rd)["projectType"].ToString().ToLower() == "external") ? ((DbDataReader)(object)rd)["contactperson"].ToString() : string.Empty),
							Address = ((((DbDataReader)(object)rd)["projectType"].ToString().ToLower() == "external") ? ((DbDataReader)(object)rd)["address"].ToString() : string.Empty)
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

		public List<RemoteSensingProject.Models.Admin.main.Generate_Notice> getNoticeList(string userId)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_manageNotice", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"getNoticeByManager");
				cmd.Parameters.AddWithValue("@projectManager", (object)userId);
				((DbConnection)(object)con).Open();
				List<RemoteSensingProject.Models.Admin.main.Generate_Notice> noticeList = new List<RemoteSensingProject.Models.Admin.main.Generate_Notice>();
				NpgsqlDataReader res = cmd.ExecuteReader();
				while (((DbDataReader)(object)res).Read())
				{
					noticeList.Add(new RemoteSensingProject.Models.Admin.main.Generate_Notice
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
				((Component)(object)cmd).Dispose();
			}
		}

		public List<RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem> getAllSubOrdinateProblem(string projectManager, int? page = null, int? limit = null, string searchTerm = null)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Expected O, but got Unknown
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem> problemList = new List<RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem>();
				RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem obj = null;
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageproblems_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@v_action", (object)"getAllProblemListByManager");
						cmd.Parameters.AddWithValue("@v_projectmanager", (object)Convert.ToInt32(projectManager));
						cmd.Parameters.AddWithValue("@v_id", (object)0);
						cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("v_searchterm", (object)(string.IsNullOrEmpty(searchTerm) ? ((IConvertible)DBNull.Value) : ((IConvertible)searchTerm)));
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader sdr = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)sdr).HasRows)
								{
									bool firstRow = true;
									while (((DbDataReader)(object)sdr).Read())
									{
										obj = new RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem();
										obj.ProblemId = Convert.ToInt32(((DbDataReader)(object)sdr)["problemId"]);
										obj.ProjectName = ((DbDataReader)(object)sdr)["ProjectName"].ToString();
										obj.Title = ((DbDataReader)(object)sdr)["Title"].ToString();
										obj.Description = ((DbDataReader)(object)sdr)["Description"].ToString();
										obj.Attchment_Url = ((DbDataReader)(object)sdr)["Attachment"].ToString();
										obj.CreatedDate = Convert.ToDateTime(((DbDataReader)(object)sdr)["CreatedDate"]).ToString("dd-MM-yyyy");
										obj.newRequest = Convert.ToBoolean(((DbDataReader)(object)sdr)["newRequest"]);
										obj.projectCode = ((((DbDataReader)(object)sdr)["projectCode"] != DBNull.Value) ? ((DbDataReader)(object)sdr)["projectCode"].ToString() : "N/A");
										problemList.Add(obj);
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

		public List<RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem> getAllSubOrdinateProblemById(string projectManager, int id)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Expected O, but got Unknown
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Expected O, but got Unknown
			List<RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem> problemList = new List<RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem>();
			((DbConnection)(object)con).Open();
			try
			{
				NpgsqlTransaction transaction = con.BeginTransaction();
				NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM  fn_manageproblems_cursor(@v_action,@v_projectmanager,@v_id);", con, transaction);
				try
				{
					cmd.Parameters.AddWithValue("v_action", (object)"getAllProblemListByManagerById");
					cmd.Parameters.AddWithValue("v_projectmanager", (object)Convert.ToInt32(projectManager));
					cmd.Parameters.AddWithValue("v_id", (object)id);
					string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
					NpgsqlCommand fetchCmd = new NpgsqlCommand("FETCH ALL FROM \"" + cursorName + "\";", con, transaction);
					try
					{
						NpgsqlDataReader sdr = fetchCmd.ExecuteReader();
						try
						{
							while (((DbDataReader)(object)sdr).Read())
							{
								problemList.Add(new RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem
								{
									ProblemId = Convert.ToInt32(((DbDataReader)(object)sdr)["problemId"]),
									ProjectName = ((DbDataReader)(object)sdr)["ProjectName"].ToString(),
									Title = ((DbDataReader)(object)sdr)["Title"].ToString(),
									Description = ((DbDataReader)(object)sdr)["Description"].ToString(),
									Attchment_Url = ((DbDataReader)(object)sdr)["Attachment"].ToString(),
									CreatedDate = Convert.ToDateTime(((DbDataReader)(object)sdr)["CreatedDate"]).ToString("dd-MM-yyyy"),
									newRequest = Convert.ToBoolean(((DbDataReader)(object)sdr)["newRequest"])
								});
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
					NpgsqlCommand closeCmd = new NpgsqlCommand("close \"" + cursorName + "\"", con, transaction);
					try
					{
						((DbCommand)(object)closeCmd).ExecuteNonQuery();
					}
					finally
					{
						((IDisposable)closeCmd)?.Dispose();
					}
					((DbTransaction)(object)transaction).Commit();
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
				return problemList;
			}
			catch (Exception innerException)
			{
				throw new Exception("An error occurred", innerException);
			}
			finally
			{
				((DbConnection)(object)con).Close();
			}
		}

		public bool CompleteSelectedProblem(int probId)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_ManageSubordinateProjectProblem", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"completeproblem");
				cmd.Parameters.AddWithValue("@id", (object)probId);
				((DbConnection)(object)con).Open();
				return ((DbCommand)(object)cmd).ExecuteNonQuery() > 0;
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

		public List<RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem> getSubOrdinateProblemforAdmin(int? limit = null, int? page = null, string searchTerm = null, int? id = null)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Expected O, but got Unknown
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem> problemList = new List<RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem>();
				RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem obj = null;
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageproblems_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"getAllSubOrdinateProblem");
						cmd.Parameters.AddWithValue("v_projectmanager", (object)0);
						cmd.Parameters.AddWithValue("v_id", (object)id.GetValueOrDefault());
						NpgsqlParameterCollection parameters = cmd.Parameters;
						int? num = limit;
						parameters.AddWithValue("v_limit", num.HasValue ? ((object)num.GetValueOrDefault()) : DBNull.Value);
						NpgsqlParameterCollection parameters2 = cmd.Parameters;
						num = page;
						parameters2.AddWithValue("v_page", num.HasValue ? ((object)num.GetValueOrDefault()) : DBNull.Value);
						cmd.Parameters.AddWithValue("v_searchterm", (object)(string.IsNullOrEmpty(searchTerm) ? ((IConvertible)DBNull.Value) : ((IConvertible)searchTerm)));
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader sdr = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)sdr).HasRows)
								{
									bool firstRow = true;
									while (((DbDataReader)(object)sdr).Read())
									{
										obj = new RemoteSensingProject.Models.SubOrdinate.main.Raise_Problem();
										obj.ProblemId = Convert.ToInt32(((DbDataReader)(object)sdr)["problemId"]);
										obj.ProjectName = ((DbDataReader)(object)sdr)["ProjectName"].ToString();
										obj.Title = ((DbDataReader)(object)sdr)["Title"].ToString();
										obj.Description = ((DbDataReader)(object)sdr)["Description"].ToString();
										obj.Attchment_Url = ((DbDataReader)(object)sdr)["Attachment"].ToString();
										obj.CreatedDate = Convert.ToDateTime(((DbDataReader)(object)sdr)["CreatedDate"]).ToString("dd-MM-yyyy");
										obj.newRequest = Convert.ToBoolean(((DbDataReader)(object)sdr)["newRequest"]);
										obj.projectCode = ((((DbDataReader)(object)sdr)["projectCode"] != DBNull.Value) ? ((DbDataReader)(object)sdr)["projectCode"].ToString() : "N/A");
										problemList.Add(obj);
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

		public bool UpdateMonthlyStatus(RemoteSensingProject.Models.Admin.main.Project_MonthlyUpdate pwu)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			try
			{
				NpgsqlCommand cmd = new NpgsqlCommand("CALL sp_manageprojectsubstaces(v_action=>@v_action, v_project_Id => @project_id, v_w_date=>@w_date, v_comment=>@comment, v_unit=>@unit, v_annual=>@annual, v_monthEnd=>@monthend, v_reviewMonth=>@reviewmonth, v_MonthEndSequentially=>@monthendseq, v_StateBeneficiaries=>@statebeneficiaries)", con);
				try
				{
					((DbParameter)(object)cmd.Parameters.Add("@v_action", (NpgsqlDbType)22)).Value = "insertUpdate";
					((DbParameter)(object)cmd.Parameters.Add("@project_id", (NpgsqlDbType)9)).Value = pwu.ProjectId;
					((DbParameter)(object)cmd.Parameters.Add("@w_date", (NpgsqlDbType)7)).Value = pwu.date;
					((DbParameter)(object)cmd.Parameters.Add("@comment", (NpgsqlDbType)19)).Value = ((object)pwu.comments) ?? ((object)DBNull.Value);
					((DbParameter)(object)cmd.Parameters.Add("@unit", (NpgsqlDbType)22)).Value = ((object)pwu.unit) ?? ((object)DBNull.Value);
					((DbParameter)(object)cmd.Parameters.Add("@annual", (NpgsqlDbType)22)).Value = ((object)pwu.annual) ?? ((object)DBNull.Value);
					((DbParameter)(object)cmd.Parameters.Add("@monthend", (NpgsqlDbType)19)).Value = ((object)pwu.monthEnd) ?? ((object)DBNull.Value);
					((DbParameter)(object)cmd.Parameters.Add("@reviewmonth", (NpgsqlDbType)19)).Value = ((object)pwu.reviewMonth) ?? ((object)DBNull.Value);
					((DbParameter)(object)cmd.Parameters.Add("@monthendseq", (NpgsqlDbType)19)).Value = ((object)pwu.MonthEndSequentially) ?? ((object)DBNull.Value);
					((DbParameter)(object)cmd.Parameters.Add("@statebeneficiaries", (NpgsqlDbType)22)).Value = ((object)pwu.StateBeneficiaries) ?? ((object)DBNull.Value);
					((DbConnection)(object)con).Open();
					((DbCommand)(object)cmd).ExecuteNonQuery();
				}
				finally
				{
					((IDisposable)cmd)?.Dispose();
				}
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
				((Component)(object)base.cmd).Dispose();
			}
		}

		public List<RemoteSensingProject.Models.Admin.main.Project_MonthlyUpdate> MonthlyProjectUpdate(int projectId)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Expected O, but got Unknown
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<RemoteSensingProject.Models.Admin.main.Project_Subordination> subList = new List<RemoteSensingProject.Models.Admin.main.Project_Subordination>();
				List<RemoteSensingProject.Models.Admin.main.Project_MonthlyUpdate> list = new List<RemoteSensingProject.Models.Admin.main.Project_MonthlyUpdate>();
				RemoteSensingProject.Models.Admin.main.Project_model pm = new RemoteSensingProject.Models.Admin.main.Project_model();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageprojectsubstances_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"selectAllByProject");
						cmd.Parameters.AddWithValue("v_project_id", (object)projectId);
						cmd.Parameters.AddWithValue("v_id", (object)0);
						cmd.Parameters.AddWithValue("v_limit", (object)0);
						cmd.Parameters.AddWithValue("v_page", (object)0);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader rd = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)rd).HasRows)
								{
									while (((DbDataReader)(object)rd).Read())
									{
										list.Add(new RemoteSensingProject.Models.Admin.main.Project_MonthlyUpdate
										{
											Id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
											date = Convert.ToDateTime(((DbDataReader)(object)rd)["w_date"]),
											comments = ((DbDataReader)(object)rd)["comment"].ToString(),
											unit = ((DbDataReader)(object)rd)["unit"].ToString(),
											annual = ((DbDataReader)(object)rd)["annual"].ToString(),
											monthEnd = ((DbDataReader)(object)rd)["monthEnd"].ToString(),
											reviewMonth = ((DbDataReader)(object)rd)["reviewMonth"].ToString(),
											MonthEndSequentially = ((DbDataReader)(object)rd)["MonthEndSequentially"].ToString(),
											StateBeneficiaries = ((DbDataReader)(object)rd)["StateBeneficiaries"].ToString(),
											projectName = ((DbDataReader)(object)rd)["title"].ToString()
										});
									}
								}
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

		public bool updateFinancialReportMonthly(FinancialMonthlyReport fr)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("CALL public.sp_manageprojectsubstaces(\r\n                    @action,          -- v_action\r\n                    0,                -- v_id\r\n                    @project_id,      -- v_project_id\r\n                    @w_date,          -- v_w_date\r\n                    NULL,             -- v_title\r\n                    NULL,             -- v_comment\r\n                    NULL,             -- v_reason\r\n                    0,                -- v_completion\r\n                    NULL,             -- v_attatchment\r\n                    0,                -- v_amount\r\n                    NULL,             -- v_unit\r\n                    NULL,             -- v_annual\r\n                    NULL,             -- v_monthend\r\n                    NULL,             -- v_reviewmonth\r\n                    NULL,             -- v_monthendsequentially\r\n                    NULL,             -- v_statebeneficiaries\r\n                    NULL,             -- v_createdat\r\n                    0,                -- v_projectid\r\n                    0,                -- v_headid\r\n                    NULL,             -- v_updateat\r\n                    false,            -- v_status\r\n                    @aim,             -- v_aim\r\n                    @month_aim,       -- v_month_aim\r\n                    @completeinmonth, -- v_completeinmonth\r\n                    @departbeneficiaries, -- v_departbeneficiaries\r\n                    0,                -- v_appstatus\r\n                    NULL              -- v_rc\r\n                );", con);
				((DbCommand)(object)cmd).CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@action", (object)"insertfinanceUpdate");
				cmd.Parameters.AddWithValue("@project_id", (object)fr.projectId);
				cmd.Parameters.AddWithValue("@aim", (object)fr.aim);
				if (DateTime.TryParse(fr.date, out var workDate))
				{
					cmd.Parameters.AddWithValue("@w_date", (object)workDate);
					cmd.Parameters.AddWithValue("@month_aim", (object)fr.month_aim);
					cmd.Parameters.AddWithValue("@completeinmonth", (object)fr.completeInMonth);
					cmd.Parameters.AddWithValue("@departbeneficiaries", (object)fr.departBeneficiaries);
					((DbConnection)(object)con).Open();
					((DbCommand)(object)cmd).ExecuteNonQuery();
					return true;
				}
				throw new Exception("Invalid date format in fr.date");
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

		public List<FinancialMonthlyReport> GetExtrnlFinancialReport(int projectId)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Expected O, but got Unknown
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Expected O, but got Unknown
			try
			{
				List<FinancialMonthlyReport> list = new List<FinancialMonthlyReport>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageprojectsubstances_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"selectFinancilaReportByProjectId");
						cmd.Parameters.AddWithValue("v_project_id", (object)projectId);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader rd = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)rd).HasRows)
								{
									while (((DbDataReader)(object)rd).Read())
									{
										list.Add(new FinancialMonthlyReport
										{
											id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
											projectId = Convert.ToInt32(((DbDataReader)(object)rd)["project_Id"]),
											aim = ((DbDataReader)(object)rd)["aim"].ToString(),
											date = Convert.ToDateTime(((DbDataReader)(object)rd)["f_date"]).ToString("dd/MM/yyyy"),
											month_aim = ((DbDataReader)(object)rd)["month_aim"].ToString(),
											completeInMonth = ((DbDataReader)(object)rd)["completeInMonth"].ToString(),
											departBeneficiaries = ((DbDataReader)(object)rd)["departBeneficiaries"].ToString(),
											projectName = ((DbDataReader)(object)rd)["title"].ToString(),
											totalBudget = ((DbDataReader)(object)rd)["budget"].ToString(),
											description = ((DbDataReader)(object)rd)["description"].ToString(),
											department = ((DbDataReader)(object)rd)["DepartmentName"].ToString()
										});
									}
								}
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

		public bool insertExpences(ProjectExpenses exp)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("CALL sp_ManageProjectSubstaces(v_action=>@action, v_project_id=>@project_id, v_id=>@id, v_title=>@title, v_w_date=>@w_date, v_amount=>@amount, v_attatchment=>@attatchment, v_comment =>@comment )", con);
				cmd.Parameters.AddWithValue("@action", (object)"insertExpences");
				cmd.Parameters.AddWithValue("@project_id", (object)exp.projectId);
				cmd.Parameters.AddWithValue("@id", (object)exp.projectHeadId);
				cmd.Parameters.AddWithValue("@title", (object)exp.title);
				cmd.Parameters.AddWithValue("@w_date", (object)exp.date);
				cmd.Parameters.AddWithValue("@amount", (object)exp.amount);
				if (string.IsNullOrEmpty(exp.attatchment_url))
				{
					cmd.Parameters.AddWithValue("@attatchment", (object)DBNull.Value);
				}
				else
				{
					cmd.Parameters.AddWithValue("@attatchment", (object)exp.attatchment_url);
				}
				cmd.Parameters.AddWithValue("@comment", (object)exp.description);
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

		public List<ProjectExpenses> ExpencesList(int headId, int projectId, int? limit = null, int? page = null)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Expected O, but got Unknown
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<ProjectExpenses> list = new List<ProjectExpenses>();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageprojectsubstances_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"selectExpenses");
						cmd.Parameters.AddWithValue("v_project_id", (object)projectId);
						cmd.Parameters.AddWithValue("v_id", (object)headId);
						cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader rd = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)rd).HasRows)
								{
									while (((DbDataReader)(object)rd).Read())
									{
										list.Add(new ProjectExpenses
										{
											Id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
											projectHeadId = Convert.ToInt32(((DbDataReader)(object)rd)["budgetHeadId"]),
											AppStatus = ((((DbDataReader)(object)rd)["AppStatus"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)rd)["AppStatus"]) : 0),
											AppAmount = ((((DbDataReader)(object)rd)["AppAmount"] != DBNull.Value) ? float.Parse(((DbDataReader)(object)rd)["AppAmount"].ToString()) : 0f),
											title = ((DbDataReader)(object)rd)["title"].ToString(),
											date = Convert.ToDateTime(((DbDataReader)(object)rd)["insertDate"]),
											DateString = Convert.ToDateTime(((DbDataReader)(object)rd)["insertDate"]).ToString("dd-MM-yyyy"),
											amount = Convert.ToDecimal(((DbDataReader)(object)rd)["amount"]),
											attatchment_url = ((DbDataReader)(object)rd)["attatchment"].ToString(),
											description = ((DbDataReader)(object)rd)["description"].ToString(),
											reason = ((((DbDataReader)(object)rd)["reason"] != DBNull.Value) ? ((DbDataReader)(object)rd)["reason"].ToString() : "N/A")
										});
									}
								}
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

		public List<RemoteSensingProject.Models.Admin.main.Project_Budget> ProjectBudgetList(int Id, int? page = null, int? limit = null)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			List<RemoteSensingProject.Models.Admin.main.Project_Budget> list = new List<RemoteSensingProject.Models.Admin.main.Project_Budget>();
			try
			{
				((DbConnection)(object)con).Open();
				NpgsqlCommand cmd = new NpgsqlCommand("fn_getprojectstagesandbudget", con);
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("v_action", (object)"GetBudgetByProjectId");
					cmd.Parameters.AddWithValue("v_id", (object)Id);
					cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
					cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
					NpgsqlDataReader rd = cmd.ExecuteReader();
					try
					{
						if (((DbDataReader)(object)rd).HasRows)
						{
							while (((DbDataReader)(object)rd).Read())
							{
								list.Add(new RemoteSensingProject.Models.Admin.main.Project_Budget
								{
									Id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
									Project_Id = Convert.ToInt32(((DbDataReader)(object)rd)["project_id"]),
									ProjectHeads = ((DbDataReader)(object)rd)["heads"].ToString(),
									ProjectAmount = Convert.ToDecimal((((DbDataReader)(object)rd)["headsAmount"] != DBNull.Value) ? ((DbDataReader)(object)rd)["headsAmount"] : ((object)0)),
									TotalAskAmount = ((DbDataReader)(object)rd)["totalAskAmount"].ToString(),
									ApproveAmount = ((DbDataReader)(object)rd)["approveAmount"].ToString()
								});
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

		public bool InsertStageStatus(RemoteSensingProject.Models.Admin.main.Project_Statge obj)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			try
			{
				NpgsqlConnection val = con;
				try
				{
					((DbConnection)(object)con).Open();
					NpgsqlCommand cmd = new NpgsqlCommand("CALL public.sp_managestagestatus(\r\n            @v_id,\r\n            @v_stageid,\r\n            @v_comment,\r\n            @v_completionprecentage,\r\n            @v_stagedocument,\r\n            @v_delayreason,\r\n            @v_updatestatus,\r\n            @v_status,\r\n            @v_project_id,\r\n            @v_completionstatus,\r\n            @v_action,\r\n            NULL\r\n        );", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.Text;
						((DbParameter)(object)cmd.Parameters.Add("@v_id", (NpgsqlDbType)9)).Value = 0;
						((DbParameter)(object)cmd.Parameters.Add("@v_stageid", (NpgsqlDbType)9)).Value = obj.Stage_Id;
						((DbParameter)(object)cmd.Parameters.Add("@v_comment", (NpgsqlDbType)19)).Value = ((object)obj.Comment) ?? ((object)DBNull.Value);
						((DbParameter)(object)cmd.Parameters.Add("@v_completionprecentage", (NpgsqlDbType)22)).Value = ((object)obj.CompletionPrecentage) ?? ((object)DBNull.Value);
						((DbParameter)(object)cmd.Parameters.Add("@v_stagedocument", (NpgsqlDbType)19)).Value = ((object)obj.StageDocument_Url) ?? ((object)DBNull.Value);
						((DbParameter)(object)cmd.Parameters.Add("@v_delayreason", (NpgsqlDbType)19)).Value = DBNull.Value;
						((DbParameter)(object)cmd.Parameters.Add("@v_updatestatus", (NpgsqlDbType)22)).Value = ((object)obj.Status) ?? ((object)DBNull.Value);
						((DbParameter)(object)cmd.Parameters.Add("@v_status", (NpgsqlDbType)18)).Value = 1;
						((DbParameter)(object)cmd.Parameters.Add("@v_project_id", (NpgsqlDbType)9)).Value = obj.Project_Id;
						((DbParameter)(object)cmd.Parameters.Add("@v_completionstatus", (NpgsqlDbType)2)).Value = false;
						((DbParameter)(object)cmd.Parameters.Add("@v_action", (NpgsqlDbType)22)).Value = "insertStageStatus";
						((DbCommand)(object)cmd).ExecuteNonQuery();
					}
					finally
					{
						((IDisposable)cmd)?.Dispose();
					}
					return true;
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			catch (Exception innerException)
			{
				throw new Exception("Error inserting stage status", innerException);
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
			}
		}

		public List<RemoteSensingProject.Models.Admin.main.Project_Statge> ViewStagesComments(string stageId, int? page = null, int? limit = null)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Expected O, but got Unknown
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Expected O, but got Unknown
			try
			{
				List<RemoteSensingProject.Models.Admin.main.Project_Statge> stageList = new List<RemoteSensingProject.Models.Admin.main.Project_Statge>();
				RemoteSensingProject.Models.Admin.main.Project_Statge stage = null;
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageprojectsubstances_cursor", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@v_action", (object)"viewDealyReason");
						cmd.Parameters.AddWithValue("@v_project_id", (object)0);
						cmd.Parameters.AddWithValue("@v_id", (object)Convert.ToInt32(stageId));
						cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader sdr = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)sdr).HasRows)
								{
									while (((DbDataReader)(object)sdr).Read())
									{
										stage = new RemoteSensingProject.Models.Admin.main.Project_Statge();
										stage.StageDocument_Url = ((DbDataReader)(object)sdr)["StageDocument"].ToString();
										stage.Comment = ((DbDataReader)(object)sdr)["Comment"].ToString();
										stage.Status = ((DbDataReader)(object)sdr)["updateStatus"].ToString();
										stage.CreatedDate = Convert.ToDateTime(((DbDataReader)(object)sdr)["CreatedDate"]).ToString("dd-MM-yyyy");
										stageList.Add(stage);
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
				return stageList;
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

		public bool insertOutSource(OuterSource os)
		{
			try
			{
				string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
				Random rnd = new Random();
				string userpassword = "";
				if (os.Id == 0)
				{
					for (int i = 0; i < 8; i++)
					{
						userpassword += validChars[rnd.Next(validChars.Length)];
					}
				}
				cmd = new NpgsqlCommand("CALL sp_manageoutsource(p_aciton => @p_action,p_id=>@p_id,p_designationid=> @p_designationid,p_emp_name=> @p_emp_name, p_emp_mobile=>@p_emp_mobile, p_emp_email=>@p_emp_email, p_emp_gender=>@p_emp_gender, p_password=>@p_password)", con);
				((DbCommand)(object)cmd).CommandType = CommandType.Text;
				((DbParameter)(object)cmd.Parameters.Add("@p_action", (NpgsqlDbType)22)).Value = os.Id > 0 ? "updateOutSource" : "createOutSource";
				((DbParameter)(object)cmd.Parameters.Add("@p_id", (NpgsqlDbType)9)).Value = os.Id;
				((DbParameter)(object)cmd.Parameters.Add("@p_designationid", (NpgsqlDbType)9)).Value = os.designationid;
				((DbParameter)(object)cmd.Parameters.Add("@p_emp_name", (NpgsqlDbType)22)).Value = os.EmpName;
				((DbParameter)(object)cmd.Parameters.Add("@p_emp_mobile", (NpgsqlDbType)1)).Value = Convert.ToInt64(os.mobileNo);
				((DbParameter)(object)cmd.Parameters.Add("@p_emp_email", (NpgsqlDbType)22)).Value = os.email;
				((DbParameter)(object)cmd.Parameters.Add("@p_emp_gender", (NpgsqlDbType)22)).Value = os.gender;
				((DbParameter)(object)cmd.Parameters.Add("@p_password", (NpgsqlDbType)22)).Value = userpassword;
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

        public bool SendOutSourceAtTour(OutsourceAtTour os)
        {
            try
            {
                cmd = new NpgsqlCommand("CALL sp_manageoutsource(p_action => @p_action,p_outsourceid=>@p_outsourceid,p_fromdate=> @p_fromdate,p_todate=> @p_todate, p_description=>@p_description)", con);
                ((DbCommand)(object)cmd).CommandType = CommandType.Text;

                cmd.Parameters.Add("@p_action", NpgsqlDbType.Varchar).Value = "sendoutsourceontour";
                cmd.Parameters.Add("@p_outsourceid", NpgsqlDbType.Integer).Value = os.outsourceid;
                cmd.Parameters.Add("@p_fromdate", NpgsqlDbType.Timestamp).Value = os.fromDate;
                cmd.Parameters.Add("@p_todate", NpgsqlDbType.Timestamp).Value = os.toDate;
                cmd.Parameters.Add("@p_description", NpgsqlDbType.Text).Value = os.description;

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

        public List<OuterSource> selectAllOutSOurceList(int? id, int? limit = null, int? page = null, string searchTerm = null)
		{
			try
			{
				List<OuterSource> list = new List<OuterSource>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageOutsource_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@v_action", (object)"selectAll");
						cmd.Parameters.AddWithValue("@v_id", id.HasValue ? ((object)id.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_searchterm", (object)(string.IsNullOrEmpty(searchTerm) ? ((IConvertible)DBNull.Value) : ((IConvertible)searchTerm)));
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
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
										OuterSource data = new OuterSource
										{
											Id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
											EmpName = ((DbDataReader)(object)rd)["emp_name"].ToString(),
											mobileNo = Convert.ToInt64(((DbDataReader)(object)rd)["emp_mobile"]),
											email = ((DbDataReader)(object)rd)["emp_email"].ToString(),
											gender = ((DbDataReader)(object)rd)["emp_gender"].ToString(),
											designationname = ((DbDataReader)(object)rd)["designationname"].ToString(),
											designationid = Convert.ToInt32(rd["designationid"])
										};
										if (firstRow)
										{
											data.Pagination = new ApiCommon.PaginationInfo
											{
												TotalPages = Convert.ToInt32(((DbDataReader)(object)rd)["totalpages"]),
												TotalRecords = Convert.ToInt32(((DbDataReader)(object)rd)["totalrecords"]),
												PageNumber = page.GetValueOrDefault(),
												PageSize = limit.GetValueOrDefault()
											};
											firstRow = false;
										}
										list.Add(data);
									}
								}
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

        public List<OuterSource> GetAllocatedOutSOurceList(int? id, int? limit = null, int? page = null, string searchTerm = null)
        {
            try
            {
                List<OuterSource> list = new List<OuterSource>();
                ((DbConnection)(object)con).Open();
                NpgsqlTransaction tran = con.BeginTransaction();
                try
                {
                    NpgsqlCommand cmd = new NpgsqlCommand("fn_manageOutsource_cursor", con, tran);
                    try
                    {
                        ((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@v_action", (object)"selectallocatedoutsource");
                        cmd.Parameters.AddWithValue("@v_id", id.HasValue ? ((object)id.Value) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@v_searchterm", (object)(string.IsNullOrEmpty(searchTerm) ? ((IConvertible)DBNull.Value) : ((IConvertible)searchTerm)));
                        string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
                        NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
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
                                        OuterSource data = new OuterSource
                                        {
                                            Id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
                                            EmpName = ((DbDataReader)(object)rd)["emp_name"].ToString(),
                                            mobileNo = Convert.ToInt64(((DbDataReader)(object)rd)["emp_mobile"]),
                                            email = ((DbDataReader)(object)rd)["emp_email"].ToString(),
                                            gender = ((DbDataReader)(object)rd)["emp_gender"].ToString(),
                                            designationname = ((DbDataReader)(object)rd)["designationname"].ToString(),
                                            designationid = Convert.ToInt32(rd["designationid"])
                                        };
                                        if (firstRow)
                                        {
                                            data.Pagination = new ApiCommon.PaginationInfo
                                            {
                                                TotalPages = Convert.ToInt32(((DbDataReader)(object)rd)["totalpages"]),
                                                TotalRecords = Convert.ToInt32(((DbDataReader)(object)rd)["totalrecords"]),
                                                PageNumber = page.GetValueOrDefault(),
                                                PageSize = limit.GetValueOrDefault()
                                            };
                                            firstRow = false;
                                        }
                                        list.Add(data);
                                    }
                                }
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

        public bool DeleteOutSource(int id)
		{
			try
			{
                cmd = new NpgsqlCommand();
				cmd.Connection = con;
                ((DbCommand)(object)cmd).CommandType = CommandType.Text;
				((DbCommand)(object)cmd).CommandText = "CALL sp_manageoutsource(p_action=>@p_action,p_id=>@p_id)";
				cmd.Parameters.AddWithValue("p_action", (object)"deleteOutsource");
				cmd.Parameters.AddWithValue("p_id", (object)id);
                ((DbConnection)(object)con).Open();
                ((DbCommand)(object)cmd).ExecuteNonQuery();
                return true;
            }
			catch(Exception ex)
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

		public bool GetResponseFromMember(getMemberResponse mr)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			try
			{
				((DbParameterCollection)(object)cmd.Parameters).Clear();
				cmd = new NpgsqlCommand("call sp_managememberresponseformeeting(v_action=>@v_action,v_appstatus=>@v_appstatus,v_reason=>@v_reason,v_meeting=>@v_meeting,v_employee=>@v_employee)", con);
				cmd.Parameters.AddWithValue("@v_action", (object)"getResponseFromMemberForMeeting");
				cmd.Parameters.AddWithValue("@v_appstatus", (object)mr.ApprovedStatus);
				cmd.Parameters.AddWithValue("@v_reason", (object)(mr.reason ?? ""));
				cmd.Parameters.AddWithValue("@v_meeting", (object)mr.MeetingId);
				cmd.Parameters.AddWithValue("@v_employee", (object)mr.MemberId);
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

		public List<RemoteSensingProject.Models.Admin.main.Meeting_Model> getAllmeeting(int id, int? limit = null, int? page = null, string searchTerm = null, string statusFilter = null)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			try
			{
				List<RemoteSensingProject.Models.Admin.main.Meeting_Model> _list = new List<RemoteSensingProject.Models.Admin.main.Meeting_Model>();
				RemoteSensingProject.Models.Admin.main.Meeting_Model obj = null;
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("select * from fn_get_meetings(@p_action,@p_id,@v_limit,@v_page,@v_type,@v_searchTerm,@v_statusFilter);", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.Text;
						cmd.Parameters.AddWithValue("@p_action", (object)"selectMeetingForProjectManager");
						cmd.Parameters.AddWithValue("@p_id", (object)id);
						cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_type", (object)DBNull.Value);
						cmd.Parameters.AddWithValue("@v_searchTerm", (object)(string.IsNullOrEmpty(searchTerm) ? ((IConvertible)DBNull.Value) : ((IConvertible)searchTerm)));
						cmd.Parameters.AddWithValue("@v_statusFilter", (object)(string.IsNullOrEmpty(statusFilter) ? ((IConvertible)DBNull.Value) : ((IConvertible)statusFilter)));
						NpgsqlDataReader sdr = cmd.ExecuteReader();
						bool firstRow = true;
						while (((DbDataReader)(object)sdr).Read())
						{
							obj = new RemoteSensingProject.Models.Admin.main.Meeting_Model();
							obj.Id = Convert.ToInt32(((DbDataReader)(object)sdr)["id"]);
							obj.CompleteStatus = Convert.ToInt32(((DbDataReader)(object)sdr)["completeStatus"]);
							obj.MeetingType = ((DbDataReader)(object)sdr)["meetingType"].ToString();
							obj.MeetingLink = ((DbDataReader)(object)sdr)["meetingLink"].ToString();
							obj.MeetingTitle = ((DbDataReader)(object)sdr)["MeetingTitle"].ToString();
							obj.AppStatus = ((((DbDataReader)(object)sdr)["appStatus"] != DBNull.Value) ? ((int)((DbDataReader)(object)sdr)["appStatus"]) : 0);
							obj.memberId = ((((DbDataReader)(object)sdr)["memberId"] != DBNull.Value) ? ((DbDataReader)(object)sdr)["memberId"].ToString().Split(',').ToList() : new List<string>());
							obj.CreaterId = ((((DbDataReader)(object)sdr)["createrId"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)sdr)["createrId"]) : 0);
							obj.MeetingDate = Convert.ToDateTime(((DbDataReader)(object)sdr)["meetingTime"]).ToString("dd-MM-yyyy hh:mm tt");
							obj.createdBy = ((DbDataReader)(object)sdr)["createdBy"].ToString();
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
						((IDisposable)cmd)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)tran)?.Dispose();
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

		public List<GetConclusion> getConclusionForMeeting(int meetingId, int userId, int? limit = null, int? page = null)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Expected O, but got Unknown
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Expected O, but got Unknown
			try
			{
				List<GetConclusion> _list = new List<GetConclusion>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_managemeetingconclusion_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@v_action", (object)"selectConclusionForProjectManager");
						cmd.Parameters.AddWithValue("@v_id", (object)0);
						cmd.Parameters.AddWithValue("@v_memberid", (object)userId);
						cmd.Parameters.AddWithValue("@v_meeting", (object)meetingId);
						cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : ((object)0));
						cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : ((object)0));
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\"; ", con, tran);
						try
						{
							NpgsqlDataReader sdr = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)sdr).HasRows)
								{
									while (((DbDataReader)(object)sdr).Read())
									{
										_list.Add(new GetConclusion
										{
											Id = Convert.ToInt32(((DbDataReader)(object)sdr)["id"]),
											Conclusion = ((DbDataReader)(object)sdr)["conclusion"].ToString(),
											FollowDate = ((((DbDataReader)(object)sdr)["nextFollow"] != DBNull.Value) ? Convert.ToDateTime(((DbDataReader)(object)sdr)["nextFollow"]).ToString("dd-MM-yyyy") : "N/A")
										});
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

		public List<RemoteSensingProject.Models.Admin.main.Employee_model> getMemberJoiningStatus(int meetingId)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			try
			{
				List<RemoteSensingProject.Models.Admin.main.Employee_model> meetingc = new List<RemoteSensingProject.Models.Admin.main.Employee_model>();
				((DbConnection)(object)con).Open();
				NpgsqlCommand cmd1 = new NpgsqlCommand("SELECT * from fn_get_meetings(@p_action,@p_id);", con);
				try
				{
					((DbCommand)(object)cmd1).CommandType = CommandType.Text;
					cmd1.Parameters.AddWithValue("@p_action", (object)"selectMemberJoiningStatus");
					cmd1.Parameters.AddWithValue("@p_id", (object)meetingId);
					NpgsqlDataReader rdr = cmd1.ExecuteReader();
					try
					{
						if (((DbDataReader)(object)rdr).HasRows)
						{
							while (((DbDataReader)(object)rdr).Read())
							{
								var roleValue = rdr["role"];

                                meetingc.Add(new RemoteSensingProject.Models.Admin.main.Employee_model
								{
									EmployeeName = ((DbDataReader)(object)rdr)["name"].ToString(),
									Image_url = ((DbDataReader)(object)rdr)["profile"].ToString(),
									EmployeeRole = roleValue != DBNull.Value
    ? roleValue.ToString()
        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(r => r.Trim())
        .ToArray()
    : Array.Empty<string>(),
                                    AppStatus = ((((DbDataReader)(object)rdr)["appstatus"] != DBNull.Value) ? ((int)((DbDataReader)(object)rdr)["appstatus"]) : 0),
									Reason = ((((DbDataReader)(object)rdr)["reason"] != DBNull.Value) ? ((DbDataReader)(object)rdr)["reason"].ToString() : "N/A")
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
					((IDisposable)cmd1)?.Dispose();
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
				((Component)(object)cmd).Dispose();
			}
		}

		public bool createTask(OutSourceTask ost)
		{
			((DbConnection)(object)con).Open();
			NpgsqlTransaction tran = con.BeginTransaction();
			try
			{
				cmd = new NpgsqlCommand("CALL sp_manageoutsourcetask(v_action=>@v_action,v_id=> @v_id,v_empid=> @v_empid,v_title=>@v_title, v_description=>@v_description,v_taskid=> @v_taskid)", con, tran);
				((DbCommand)(object)cmd).CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@v_action", (object)"createTask");
				cmd.Parameters.AddWithValue("@v_id", ost.projectId);
				cmd.Parameters.AddWithValue("@v_empid", (object)ost.empId);
				cmd.Parameters.AddWithValue("@v_title", (object)ost.title);
				cmd.Parameters.AddWithValue("@v_description", (object)ost.description);
				NpgsqlParameter val = new NpgsqlParameter("@v_taskid", (NpgsqlDbType)9);
				((DbParameter)val).Direction = ParameterDirection.InputOutput;
				((DbParameter)val).Value = 0;
				NpgsqlParameter p_taskid = val;
				cmd.Parameters.Add(p_taskid);
				((DbCommand)(object)cmd).ExecuteNonQuery();
				int taskId = Convert.ToInt32((((DbParameter)(object)cmd.Parameters["@v_taskid"]).Value != DBNull.Value) ? ((DbParameter)(object)cmd.Parameters["@v_taskid"]).Value : ((object)0));
				if (taskId > 0 && ost.outSourceId.Length != 0)
				{
					int[] outSourceId = ost.outSourceId;
					foreach (int item in outSourceId)
					{
						((Component)(object)cmd).Dispose();
						cmd = new NpgsqlCommand("CALL sp_manageoutsourcetask(v_action=>@v_action, v_id=>@v_id, v_empid=>@v_empid)", con, tran);
						((DbCommand)(object)cmd).CommandType = CommandType.Text;
						cmd.Parameters.AddWithValue("@v_action", (object)"assignTask");
						cmd.Parameters.AddWithValue("@v_empid", (object)item);
						cmd.Parameters.AddWithValue("@v_id", (object)taskId);
						((DbCommand)(object)cmd).ExecuteNonQuery();
					}
				}
				((DbTransaction)(object)tran).Commit();
				return true;
			}
			catch (Exception ex)
			{
				((DbTransaction)(object)tran).Rollback();
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

		public List<OutSourceTask> taskList(int empId, int? limit = null, int? page = null, string searchTerm = null)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Expected O, but got Unknown
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Expected O, but got Unknown
			try
			{
				List<OutSourceTask> list = new List<OutSourceTask>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageoutsource_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@v_action", (object)"selectAllTask");
						cmd.Parameters.AddWithValue("@v_id", (object)empId);
						cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("v_searchterm", (object)(string.IsNullOrEmpty(searchTerm) ? ((IConvertible)DBNull.Value) : ((IConvertible)searchTerm)));
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
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
										OutSourceTask data = new OutSourceTask
										{
											Id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
											title = ((DbDataReader)(object)rd)["title"].ToString(),
											description = ((DbDataReader)(object)rd)["description"].ToString(),
											completeStatus = Convert.ToBoolean(((DbDataReader)(object)rd)["completeStatus"]),
											projectName = rd["projectName"].ToString()
										};
										if (firstRow)
										{
											data.Pagination = new ApiCommon.PaginationInfo
											{
												TotalPages = Convert.ToInt32(((DbDataReader)(object)rd)["totalpages"]),
												TotalRecords = Convert.ToInt32(((DbDataReader)(object)rd)["totalrecords"]),
												PageNumber = page.GetValueOrDefault(),
												PageSize = limit.GetValueOrDefault()
											};
											firstRow = false;
										}
										list.Add(data);
									}
								}
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

		public List<OuterSource> ViewOutSourceList(int taskId)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Expected O, but got Unknown
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Expected O, but got Unknown
			try
			{
				List<OuterSource> list = new List<OuterSource>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageoutsource_cursor", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@v_action", (object)"ViewTaskEmpStatus");
						cmd.Parameters.AddWithValue("@v_id", (object)taskId);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader rd = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)rd).HasRows)
								{
									while (((DbDataReader)(object)rd).Read())
									{
										list.Add(new OuterSource
										{
											Id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
											EmpName = ((DbDataReader)(object)rd)["emp_name"].ToString(),
											mobileNo = Convert.ToInt64(((DbDataReader)(object)rd)["emp_mobile"]),
											email = ((DbDataReader)(object)rd)["emp_email"].ToString(),
											gender = ((DbDataReader)(object)rd)["emp_gender"].ToString(),
											completeStatus = Convert.ToBoolean(((DbDataReader)(object)rd)["completeStatus"]),
											message = ((DbDataReader)(object)rd)["response"].ToString()
										});
									}
								}
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

		public bool updateTaskStatus(int taskId)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("CALL sp_manageoutsourcetask(@v_action, @v_id, NULL,null::int, NULL, NULL, NULL::text, NULL::smallint, NULL, NULL, NULL)", con);
				((DbCommand)(object)cmd).CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@v_action", (object)"updateTaskStatus");
				cmd.Parameters.AddWithValue("@v_id", (object)taskId);
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

		public bool insertReimbursement(Reimbursement data)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("CALL sp_Reimbursement(p_vrno => @vrNo, p_date => @date, p_particulars => @particulars, p_items => @items, p_userid => @userId, p_amount => @amount, p_purpose => @purpose, p_type => @type, p_action => @action)", con);
				cmd.Parameters.AddWithValue("@vrNo", (object)data.vrNo);
				cmd.Parameters.AddWithValue("@date", (object)data.date);
				cmd.Parameters.AddWithValue("@particulars", (object)data.particulars);
				cmd.Parameters.AddWithValue("@items", (object)data.items);
				cmd.Parameters.AddWithValue("@userId", (object)data.userId);
				cmd.Parameters.AddWithValue("@amount", (object)data.amount);
				cmd.Parameters.AddWithValue("@purpose", (object)data.purpose);
				cmd.Parameters.AddWithValue("@type", (object)data.type);
				cmd.Parameters.AddWithValue("@action", (object)"insert");
				((DbConnection)(object)con).Open();
				((DbCommand)(object)cmd).ExecuteNonQuery();
				return true;
			}
			catch (Exception)
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

		public bool submitReinbursementForm(string type, int userId, int Id)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("CALL sp_Reimbursement(p_action => @action, p_userid => @userId, p_type => @type, p_id => @id)", con);
				cmd.Parameters.AddWithValue("@action", (object)"submitReinbursementForm");
				cmd.Parameters.AddWithValue("@userId", (object)userId);
				cmd.Parameters.AddWithValue("@type", (object)type);
				cmd.Parameters.AddWithValue("@id", (object)Id);
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

		public List<Reimbursement> GetReimbursements(int? page = null, int? limit = null, int? id = null, int? managerId = null, string type = null, string typeFilter = null, string statusFilter = null)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Expected O, but got Unknown
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<Reimbursement> getlist = new List<Reimbursement>();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_managereimbursement_cursor", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"selectAll");
						cmd.Parameters.AddWithValue("v_projectmanager", (object)(managerId ?? new int?(0)));
						cmd.Parameters.AddWithValue("v_id", (object)(id ?? new int?(0)));
						if (string.IsNullOrWhiteSpace(type))
						{
							cmd.Parameters.AddWithValue("v_type", (object)DBNull.Value);
						}
						else
						{
							cmd.Parameters.AddWithValue("v_type", (object)type);
						}
						cmd.Parameters.AddWithValue("v_limit", (object)(limit ?? new int?(0)));
						cmd.Parameters.AddWithValue("v_page", (object)(page ?? new int?(0)));
						cmd.Parameters.AddWithValue("v_typefilter", (object)(string.IsNullOrEmpty(typeFilter) ? ((IConvertible)DBNull.Value) : ((IConvertible)typeFilter)));
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
										Reimbursement data = new Reimbursement
										{
											EmpName = ((DbDataReader)(object)res)["name"].ToString() + "(" + ((DbDataReader)(object)res)["employeeCode"].ToString() + ")",
											type = ((DbDataReader)(object)res)["type"].ToString(),
											id = Convert.ToInt32(((DbDataReader)(object)res)["id"]),
											amount = Convert.ToDecimal(((DbDataReader)(object)res)["amount"]),
											userId = Convert.ToInt32(((DbDataReader)(object)res)["userId"]),
											apprstatus = Convert.ToBoolean(((DbDataReader)(object)res)["Apprstatus"]),
											subStatus = Convert.ToBoolean(((DbDataReader)(object)res)["SaveStatus"]),
											adminappr = Convert.ToBoolean(((DbDataReader)(object)res)["Apprstatus"]),
											status = Convert.ToBoolean((((DbDataReader)(object)res)["apprAmountStatus"] != DBNull.Value) ? ((DbDataReader)(object)res)["apprAmountStatus"] : ((object)false)),
											chequeNum = ((DbDataReader)(object)res)["chequeNum"].ToString(),
											accountNewRequest = Convert.ToBoolean(((DbDataReader)(object)res)["accountNewRequest"]),
											chequeDate = ((((DbDataReader)(object)res)["chequeDate"] != DBNull.Value) ? Convert.ToDateTime(((DbDataReader)(object)res)["chequeDate"]).ToString("dd/MM/yyyy") : ""),
											newRequest = Convert.ToBoolean(((DbDataReader)(object)res)["newStatus"]),
											approveAmount = Convert.ToDecimal((((DbDataReader)(object)res)["apprAmt"] != DBNull.Value) ? ((DbDataReader)(object)res)["apprAmt"] : ((object)0)),
											remark = ((DbDataReader)(object)res)["remark"].ToString(),
											statusLabel = ((!Convert.ToBoolean(((DbDataReader)(object)res)["newStatus"]) && Convert.ToBoolean(((DbDataReader)(object)res)["Apprstatus"])) ? "Approved" : ((!Convert.ToBoolean(((DbDataReader)(object)res)["newStatus"]) && !Convert.ToBoolean(((DbDataReader)(object)res)["Apprstatus"])) ? "Rejected" : ((!Convert.ToBoolean(((DbDataReader)(object)res)["newStatus"]) && !Convert.ToBoolean(((DbDataReader)(object)res)["Apprstatus"])) ? "Pending" : "")))
										};
										if (firstRow)
										{
											data.Pagination = new ApiCommon.PaginationInfo
											{
												TotalPages = Convert.ToInt32(((DbDataReader)(object)res)["totalpages"]),
												TotalRecords = Convert.ToInt32(((DbDataReader)(object)res)["totalrecords"]),
												PageSize = limit.GetValueOrDefault(),
												PageNumber = page.GetValueOrDefault()
											};
											firstRow = false;
										}
										getlist.Add(data);
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

		public List<Reimbursement> GetSpecificUserReimbursements(int userid, string type, int id, int? page, int? limit)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<Reimbursement> getlist = new List<Reimbursement>();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageReimbursement_cursor", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"GetSpecificTypeData");
						cmd.Parameters.AddWithValue("v_projectmanager", (object)userid);
						cmd.Parameters.AddWithValue("v_id", (object)id);
						if (string.IsNullOrWhiteSpace(type))
						{
							cmd.Parameters.AddWithValue("v_type", (object)DBNull.Value);
						}
						else
						{
							cmd.Parameters.AddWithValue("v_type", (object)type);
						}
						cmd.Parameters.AddWithValue("v_limit", (object)(limit ?? new int?(0)));
						cmd.Parameters.AddWithValue("v_page", (object)(page ?? new int?(0)));
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
										Reimbursement data = new Reimbursement
										{
											id = Convert.ToInt32(((DbDataReader)(object)res)["id"]),
											type = ((DbDataReader)(object)res)["type"].ToString(),
											vrNo = ((DbDataReader)(object)res)["vrNo"].ToString(),
											date = Convert.ToDateTime(((DbDataReader)(object)res)["date"]),
											particulars = ((DbDataReader)(object)res)["particulars"].ToString(),
											items = ((DbDataReader)(object)res)["items"].ToString(),
											amount = Convert.ToDecimal(((DbDataReader)(object)res)["amount"]),
											purpose = ((DbDataReader)(object)res)["purpose"].ToString(),
											status = Convert.ToBoolean(((DbDataReader)(object)res)["status"]),
											newRequest = Convert.ToBoolean(((DbDataReader)(object)res)["newRequest"]),
											adminappr = Convert.ToBoolean(((DbDataReader)(object)res)["admin_appr"])
										};
										if (firstRow)
										{
											data.Pagination = new ApiCommon.PaginationInfo
											{
												TotalPages = Convert.ToInt32(((DbDataReader)(object)res)["totalpages"]),
												TotalRecords = Convert.ToInt32(((DbDataReader)(object)res)["totalrecords"]),
												PageNumber = page.GetValueOrDefault(),
												PageSize = limit.GetValueOrDefault()
											};
											firstRow = false;
										}
										getlist.Add(data);
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
						return getlist;
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

        #region Manage Tour Proposal
        public bool insertTour(tourProposal data)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("CALL sp_Tourproposal(v_id=> @v_id,v_userid => null::INT, v_projectid => @projectId, v_dateofdept => @dateOfDept, v_place => @place, v_periodfrom => @periodFrom, v_periodto => @periodTo, v_returndate => @returnDate, v_purpose => @purpose, v_action => @action)", con);
				cmd.Parameters.AddWithValue("@projectId", (object)data.projectId);
				cmd.Parameters.AddWithValue("@dateOfDept", (object)data.dateOfDept);
				cmd.Parameters.AddWithValue("@place", (object)data.place);
				cmd.Parameters.AddWithValue("@periodFrom", (object)data.periodFrom);
				cmd.Parameters.AddWithValue("@periodTo", (object)data.periodTo);
				cmd.Parameters.AddWithValue("@returnDate", (object)data.returnDate);
				cmd.Parameters.AddWithValue("@purpose", (object)data.purpose);
				cmd.Parameters.AddWithValue("@action", data.id > 0 ? "update" : (object)"insert");
				cmd.Parameters.AddWithValue("@v_id", data.id);
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

		public List<tourProposal> GetTourList(int? id = null, string type = null, int? page = null, int? limit = null, int? projectFilter = null)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<tourProposal> getlist = new List<tourProposal>();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_managetourproposal_cursor", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"selectAlltour");
						cmd.Parameters.AddWithValue("v_id", (object)(id ?? new int?(0)));
						cmd.Parameters.AddWithValue("v_projectid", (object)(projectFilter ?? new int?(0)));
						cmd.Parameters.AddWithValue("v_type", (object)type);
						cmd.Parameters.AddWithValue("v_limit", (object)(limit ?? new int?(0)));
						cmd.Parameters.AddWithValue("v_page", (object)(page ?? new int?(0)));
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
										tourProposal data = new tourProposal
										{
											id = Convert.ToInt32(((DbDataReader)(object)res)["id"]),
											projectName = Convert.ToString(((DbDataReader)(object)res)["title"]),
											projectId = Convert.ToInt32(res["projectId"]),
											dateOfDept = Convert.ToDateTime(((DbDataReader)(object)res)["dateOfDept"]),
											place = Convert.ToString(((DbDataReader)(object)res)["place"]),
											periodFrom = Convert.ToDateTime(((DbDataReader)(object)res)["periodFrom"]),
											periodTo = Convert.ToDateTime(((DbDataReader)(object)res)["periodTo"]),
											returnDate = Convert.ToDateTime(((DbDataReader)(object)res)["returnDate"]),
											purpose = Convert.ToString(((DbDataReader)(object)res)["purpose"]),
											projectCode = ((((DbDataReader)(object)res)["projectCode"] != DBNull.Value) ? ((DbDataReader)(object)res)["projectCode"].ToString() : "N/A")
										};
										if (firstRow)
										{
											data.Pagination = new ApiCommon.PaginationInfo
											{
												TotalPages = Convert.ToInt32(((DbDataReader)(object)res)["totalpages"]),
												TotalRecords = Convert.ToInt32(((DbDataReader)(object)res)["totalrecords"]),
												PageSize = limit.GetValueOrDefault(),
												PageNumber = page.GetValueOrDefault()
											};
											firstRow = false;
										}
										getlist.Add(data);
									}
								}
								return getlist;
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

        #endregion

        #region Manage Hiring Vehicle
        public bool insertHiring(HiringVehicle data)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("CALL sp_HiringVehicle(v_id=>@v_id,v_hid => @hid, v_amount => @amount, v_userid => null::int, v_projectid => @projectId, v_datefrom => @dateFrom, v_dateto => @dateTo, v_proposedplace=> @proposedPlace, v_purposeofvisit=>@purposeOfVisit, v_totaldaysnight=>@totalDaysNight, v_totalplainhills=>@totalPlainHills,v_taxi=>@taxi,v_bookagainstcentre=>@BookAgainstCentre,v_note =>@note, v_action=>@action )", con);
				cmd.Parameters.AddWithValue("@hid", (object)data.headId);
				cmd.Parameters.AddWithValue("@amount", (object)data.amount);
				cmd.Parameters.AddWithValue("@projectId", (object)data.projectId);
				cmd.Parameters.AddWithValue("@dateFrom", (object)data.dateFrom);
				cmd.Parameters.AddWithValue("@dateTo", (object)data.dateTo);
				cmd.Parameters.AddWithValue("@proposedPlace", (object)data.proposedPlace);
				cmd.Parameters.AddWithValue("@purposeOfVisit", (object)data.purposeOfVisit);
				cmd.Parameters.AddWithValue("@totalDaysNight", (object)data.totalDaysNight);
				cmd.Parameters.AddWithValue("@totalPlainHills", (object)data.totalPlainHills);
				cmd.Parameters.AddWithValue("@taxi", (object)data.taxi);
				cmd.Parameters.AddWithValue("@BookAgainstCentre", (object)data.BookAgainstCentre);
				cmd.Parameters.AddWithValue("@note", (object)data.note);
				cmd.Parameters.AddWithValue("@action",data.id > 0 ? "update" : (object)"insert");
				cmd.Parameters.AddWithValue("@v_id", data.id);
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

		public List<HiringVehicle> GetHiringVehicles(int? id = null, string type = null, int? page = null, int? limit = null, int? projectFilter = null)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<HiringVehicle> hiringList = new List<HiringVehicle>();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_managehiringvehicle_cursor", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"selectAllHiring");
						cmd.Parameters.AddWithValue("v_projectid", (object)(projectFilter ?? new int?(0)));
						cmd.Parameters.AddWithValue("v_id", (object)(id ?? new int?(0)));
						cmd.Parameters.AddWithValue("v_type", (object)type);
						cmd.Parameters.AddWithValue("v_limit", (object)(limit ?? new int?(0)));
						cmd.Parameters.AddWithValue("v_page", (object)(page ?? new int?(0)));
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader res = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)res).HasRows)
								{
									while (((DbDataReader)(object)res).Read())
									{
										bool firstRow = true;
										hiringList.Add(new HiringVehicle
										{
											id = (int)((DbDataReader)(object)res)["id"],
											projectName = Convert.ToString(((DbDataReader)(object)res)["title"]),
											projectId = Convert.ToInt32(res["projectId"]),
											headName = Convert.ToString(((DbDataReader)(object)res)["heads"]),
											headId = Convert.ToInt32(res["hid"]),
											amount = Convert.ToDecimal(((DbDataReader)(object)res)["amount"]),
											dateFrom = Convert.ToDateTime(((DbDataReader)(object)res)["dateFrom"]),
											dateTo = Convert.ToDateTime(((DbDataReader)(object)res)["dateTo"]),
											proposedPlace = ((DbDataReader)(object)res)["proposedPlace"].ToString(),
											purposeOfVisit = ((DbDataReader)(object)res)["purposeOfVisit"].ToString(),
											totalDaysNight = ((DbDataReader)(object)res)["totalDaysNight"].ToString(),
											totalPlainHills = ((DbDataReader)(object)res)["totalPlainHills"].ToString(),
											taxi = ((DbDataReader)(object)res)["taxi"].ToString(),
											BookAgainstCentre = ((DbDataReader)(object)res)["BookAgainstCentre"].ToString(),
											note = ((DbDataReader)(object)res)["note"].ToString(),
											projectCode = ((((DbDataReader)(object)res)["projectCode"] != DBNull.Value) ? ((DbDataReader)(object)res)["projectCode"].ToString() : "N/A")
										});
										if (firstRow)
										{
											hiringList[0].Pagination = new ApiCommon.PaginationInfo
                                            {
                                                PageNumber = page ?? 0,
                                                TotalPages = Convert.ToInt32(res["totalpages"] != DBNull.Value ? res["totalpages"] : 0),
                                                TotalRecords = Convert.ToInt32(res["totalrecords"] != DBNull.Value ? res["totalrecords"] : 0),
                                                PageSize = limit ?? 0
                                            };
                                            firstRow = false; // Optional: ensure pagination is only assigned once
                                        }
									}
								}
								return hiringList;
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
        
		#endregion

		public List<HiringVehicle> getHead(int id)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_HiringVehicle", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"selectHead");
				cmd.Parameters.AddWithValue("@id", (object)id);
				((DbConnection)(object)con).Open();
				List<HiringVehicle> getList = new List<HiringVehicle>();
				NpgsqlDataReader res = cmd.ExecuteReader();
				if (((DbDataReader)(object)res).HasRows)
				{
					while (((DbDataReader)(object)res).Read())
					{
						getList.Add(new HiringVehicle
						{
							headId = (int)((DbDataReader)(object)res)["hid"],
							headName = (string)((DbDataReader)(object)res)["headName"]
						});
					}
				}
				return getList;
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

		public List<Reimbursement> reinbursementReport(int userId)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			try
			{
				List<Reimbursement> list = new List<Reimbursement>();
				cmd = new NpgsqlCommand("sp_Reimbursement", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"selectReinbursementforUSerReport");
				cmd.Parameters.AddWithValue("@userid", (object)userId);
				((DbConnection)(object)con).Open();
				NpgsqlDataReader res = cmd.ExecuteReader();
				if (((DbDataReader)(object)res).HasRows)
				{
					while (((DbDataReader)(object)res).Read())
					{
						list.Add(new Reimbursement
						{
							id = Convert.ToInt32(((DbDataReader)(object)res)["id"]),
							type = ((DbDataReader)(object)res)["type"].ToString(),
							amount = Convert.ToDecimal(((DbDataReader)(object)res)["amount"]),
							subStatus = Convert.ToBoolean(((DbDataReader)(object)res)["SaveStatus"]),
							adminappr = Convert.ToBoolean(((DbDataReader)(object)res)["Apprstatus"]),
							userId = Convert.ToInt32(((DbDataReader)(object)res)["userId"]),
							chequeNum = ((DbDataReader)(object)res)["chequeNum"].ToString(),
							chequeDate = ((((DbDataReader)(object)res)["chequeDate"] != DBNull.Value) ? Convert.ToDateTime(((DbDataReader)(object)res)["chequeDate"]).ToString("dd/MM/yyyy") : ""),
							newRequest = Convert.ToBoolean(((DbDataReader)(object)res)["newStatus"]),
							approveAmount = Convert.ToDecimal((((DbDataReader)(object)res)["apprAmt"] != DBNull.Value) ? ((DbDataReader)(object)res)["apprAmt"] : ((object)0))
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

		public List<HiringVehicle> ProjectManagerHiringReportProjects(int userId)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_HiringVehicle", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"selectProjectManagerHiringReportProjects");
				cmd.Parameters.AddWithValue("@userId", (object)userId);
				((DbConnection)(object)con).Open();
				List<HiringVehicle> projectList = new List<HiringVehicle>();
				NpgsqlDataReader res = cmd.ExecuteReader();
				if (((DbDataReader)(object)res).HasRows)
				{
					while (((DbDataReader)(object)res).Read())
					{
						projectList.Add(new HiringVehicle
						{
							projectId = (int)((DbDataReader)(object)res)["projectId"],
							projectName = (string)((DbDataReader)(object)res)["title"]
						});
					}
				}
				return projectList;
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

		public List<HiringVehicle> ProjectManagerHiringReportbyProjects(int userId, int projectId)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_HiringVehicle", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"selectProjectManagerHiringReportbyProjects");
				cmd.Parameters.AddWithValue("@userId", (object)userId);
				cmd.Parameters.AddWithValue("@projectId", (object)projectId);
				((DbConnection)(object)con).Open();
				List<HiringVehicle> projectList = new List<HiringVehicle>();
				NpgsqlDataReader res = cmd.ExecuteReader();
				if (((DbDataReader)(object)res).HasRows)
				{
					while (((DbDataReader)(object)res).Read())
					{
						projectList.Add(new HiringVehicle
						{
							id = (int)((DbDataReader)(object)res)["id"],
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
							newRequest = Convert.ToBoolean(((DbDataReader)(object)res)["newRequest"]),
							adminappr = Convert.ToBoolean(((DbDataReader)(object)res)["adminappr"])
						});
					}
				}
				return projectList;
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

		public List<tourProposal> ProjectManagertourreportProjects(int userId)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_Tourproposal", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"ProjectManagertourreportProjects");
				cmd.Parameters.AddWithValue("@userId", (object)userId);
				((DbConnection)(object)con).Open();
				List<tourProposal> projectList = new List<tourProposal>();
				NpgsqlDataReader res = cmd.ExecuteReader();
				if (((DbDataReader)(object)res).HasRows)
				{
					while (((DbDataReader)(object)res).Read())
					{
						projectList.Add(new tourProposal
						{
							projectId = (int)((DbDataReader)(object)res)["projectId"],
							projectName = (string)((DbDataReader)(object)res)["title"]
						});
					}
				}
				return projectList;
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

		public List<tourProposal> ProjectManagertourreportByProjects(int userId, int projectId)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_Tourproposal", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"selectProjectManagertourreportByProjects");
				cmd.Parameters.AddWithValue("@userId", (object)userId);
				cmd.Parameters.AddWithValue("@projectId", (object)projectId);
				((DbConnection)(object)con).Open();
				List<tourProposal> getlist = new List<tourProposal>();
				NpgsqlDataReader res = cmd.ExecuteReader();
				if (((DbDataReader)(object)res).HasRows)
				{
					while (((DbDataReader)(object)res).Read())
					{
						getlist.Add(new tourProposal
						{
							id = Convert.ToInt32(((DbDataReader)(object)res)["id"]),
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

		public bool insertRaisedProblem(RaiseProblem rp)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("\r\n                CALL public.sp_raiseproblem(\r\n                    @v_action,\r\n                    NULL,              -- v_id\r\n                    @v_projectid,\r\n                    @v_title,\r\n                    @v_description,\r\n                    @v_document,\r\n                    NULL,              -- v_createdat\r\n                    NULL,              -- v_updatedat\r\n                    NULL,              -- v_adminappr\r\n                    NULL,              -- v_newrequest\r\n                    NULL,              -- v_status\r\n                    NULL,              -- v_projectname\r\n                    @v_userid,\r\n                    NULL               -- v_rc\r\n                )", con);
				((DbCommand)(object)cmd).CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@v_action", (object)"insert");
				cmd.Parameters.AddWithValue("@v_title", (object)rp.title);
				cmd.Parameters.AddWithValue("@v_projectid", (object)rp.projectId);
				cmd.Parameters.AddWithValue("@v_description", (object)rp.description);
				cmd.Parameters.AddWithValue("@v_document", (object)(rp.documentname ?? ""));
				cmd.Parameters.AddWithValue("@v_userid", (object)rp.id);
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

		public List<RaiseProblem> getProblems(int userId, int? limit = null, int? page = null)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Expected O, but got Unknown
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Expected O, but got Unknown
			try
			{
				List<RaiseProblem> list = new List<RaiseProblem>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageproblems_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"selectProblemsforAdmin");
						cmd.Parameters.AddWithValue("v_projectmanager", (object)userId);
						cmd.Parameters.AddWithValue("v_id", (object)0);
						cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
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
										list.Add(new RaiseProblem
										{
											id = Convert.ToInt32(((DbDataReader)(object)res)["id"]),
											title = ((DbDataReader)(object)res)["title"].ToString(),
											description = ((DbDataReader)(object)res)["description"].ToString(),
											adminappr = Convert.ToBoolean(((DbDataReader)(object)res)["adminappr"]),
											newRequest = Convert.ToBoolean(((DbDataReader)(object)res)["newRequest"]),
											documentname = ((DbDataReader)(object)res)["document"].ToString(),
											projectname = ((DbDataReader)(object)res)["projectName"].ToString(),
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

		public bool deleteRaisedProblem(int id, int userId)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("\r\n                CALL public.sp_raiseproblem(\r\n                    @v_action,\r\n                    @v_id,\r\n                    NULL,\r\n                    NULL,\r\n                    NULL,\r\n                    NULL,\r\n                    NULL,              -- v_createdat\r\n                    NULL,              -- v_updatedat\r\n                    NULL,              -- v_adminappr\r\n                    NULL,              -- v_newrequest\r\n                    NULL,              -- v_status\r\n                    NULL,              -- v_projectname\r\n                    @v_userid,\r\n                    NULL               -- v_rc\r\n                )", con);
				((DbCommand)(object)cmd).CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@v_action", (object)"delete");
				cmd.Parameters.AddWithValue("@v_id", (object)id);
				cmd.Parameters.AddWithValue("@v_userid", (object)userId);
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

		public (bool success, string error) insertAttendance(AttendanceManage am)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				NpgsqlCommand checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM ManageAttendance WHERE EmpId = @EmpId AND attendancedate = @Date", con);
				checkCmd.Parameters.AddWithValue("@EmpId", (object)am.EmpId);
				checkCmd.Parameters.AddWithValue("@Date", (object)am.attendanceDate);
				int count = (int)((DbCommand)(object)checkCmd).ExecuteScalar();
				if (count > 0)
				{
					return (success: true, error: null);
				}
				cmd = new NpgsqlCommand("sp_ManageAttendance", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"insertOutsource");
				cmd.Parameters.AddWithValue("@EmpId", (object)am.EmpId);
				cmd.Parameters.AddWithValue("@address", (object)am.address);
				cmd.Parameters.AddWithValue("@longitude", (object)am.longitude);
				cmd.Parameters.AddWithValue("@latitude", (object)am.latitude);
				cmd.Parameters.AddWithValue("@attendancestatus", (object)am.attendanceStatus);
				cmd.Parameters.AddWithValue("@attendancedate", (object)am.attendanceDate);
				cmd.Parameters.AddWithValue("@projectManager", (object)am.projectManager);
				int res = ((DbCommand)(object)cmd).ExecuteNonQuery();
				if (res > 0)
				{
					return (success: true, error: "Added Successfully");
				}
				return (success: false, error: "Server Error");
			}
			catch
			{
				return (success: false, error: "Error Occured");
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

		public (bool success, List<string> skippedDates, string error) InsertAttendance(AttendanceManage model)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Expected O, but got Unknown
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Expected O, but got Unknown
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Expected O, but got Unknown
			List<string> skippedDates = new List<string>();
			try
			{
				((DbConnection)(object)con).Open();
				foreach (KeyValuePair<string, string> item in model.Attendance)
				{
					NpgsqlTransaction tran = con.BeginTransaction();
					try
					{
						int count = 0;
						NpgsqlCommand checkCmd = new NpgsqlCommand("select fn_manageattendance_cursor(@v_action,NULL::int,@v_id,NULL::int,NULL::int,NULL::int,NULL::int,@v_date)", con, tran);
						try
						{
							checkCmd.Parameters.AddWithValue("@v_action", (object)"checkattendance");
							checkCmd.Parameters.AddWithValue("@v_id", (object)model.EmpId);
							checkCmd.Parameters.AddWithValue("@v_date", (object)DateTime.Parse(item.Key));
							string cursorName = (string)((DbCommand)(object)checkCmd).ExecuteScalar();
							if (string.IsNullOrEmpty(cursorName))
							{
								continue;
							}
							NpgsqlCommand fetchCmd = new NpgsqlCommand("FETCH ALL FROM \"" + cursorName + "\"", con, tran);
							try
							{
								count = Convert.ToInt32(((DbCommand)(object)fetchCmd).ExecuteScalar());
							}
							finally
							{
								((IDisposable)fetchCmd)?.Dispose();
							}
							goto IL_0119;
						}
						finally
						{
							((IDisposable)checkCmd)?.Dispose();
						}
					IL_0119:
						if (count > 0)
						{
							skippedDates.Add(item.Key);
							continue;
						}
						NpgsqlCommand cmd = new NpgsqlCommand("\r\n                            call sp_manageattendance(\r\n                                0,\r\n                                @v_projectmanager,\r\n                                @v_empid,\r\n                                NULL::varchar,                     -- address\r\n                                NULL::varchar,                     -- longitude\r\n                                NULL::varchar,                     -- latitude\r\n                                @v_attendancestatus,\r\n                                @v_attendancedate,\r\n                                NULL::boolean,                     -- v_projectmanagerappr\r\n                                NULL::timestamp without time zone, -- v_createdat\r\n                                NULL::timestamp without time zone, -- v_updatedat\r\n                                NULL::boolean,                     -- v_newrequest\r\n                                NULL::boolean,                     -- v_status\r\n                                NULL::varchar,                     -- v_remark\r\n                                NULL::integer,                     -- v_month\r\n                                NULL::integer,                     -- v_year\r\n                                @v_action\r\n                            );", con, tran);
						((DbCommand)(object)cmd).CommandType = CommandType.Text;
						((DbParameter)(object)cmd.Parameters.Add("@v_action", (NpgsqlDbType)22)).Value = "InsertAttendance";
						((DbParameter)(object)cmd.Parameters.Add("@v_projectmanager", (NpgsqlDbType)9)).Value = model.projectManager;
						((DbParameter)(object)cmd.Parameters.Add("@v_empid", (NpgsqlDbType)9)).Value = model.EmpId;
						((DbParameter)(object)cmd.Parameters.Add("@v_attendancedate", (NpgsqlDbType)21)).Value = Convert.ToDateTime(item.Key);
						((DbParameter)(object)cmd.Parameters.Add("@v_attendancestatus", (NpgsqlDbType)22)).Value = item.Value;
						((DbCommand)(object)cmd).ExecuteNonQuery();
						((DbTransaction)(object)tran).Commit();
					}
					finally
					{
						((IDisposable)tran)?.Dispose();
					}
				}
				return (success: true, skippedDates: skippedDates, error: null);
			}
			catch (Exception ex)
			{
				return (success: false, skippedDates: skippedDates, error: ex.Message);
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

		public List<AttendanceManage> GetAllAttendanceForOutsource(int EmpId)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_ManageAttendance", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@EmpId", (object)EmpId);
				cmd.Parameters.AddWithValue("@action", (object)"getallforoutsorce");
				List<AttendanceManage> list = new List<AttendanceManage>();
				((DbConnection)(object)con).Open();
				NpgsqlDataReader rd = cmd.ExecuteReader();
				if (((DbDataReader)(object)rd).HasRows)
				{
					while (((DbDataReader)(object)rd).Read())
					{
						list.Add(new AttendanceManage
						{
							id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
							EmpId = Convert.ToInt32(((DbDataReader)(object)rd)["EmpId"]),
							projectManager = Convert.ToInt32(((DbDataReader)(object)rd)["projectManager"]),
							address = ((DbDataReader)(object)rd)["address"].ToString(),
							longitude = ((DbDataReader)(object)rd)["longitude"].ToString(),
							latitude = ((DbDataReader)(object)rd)["latitude"].ToString(),
							createdAt = Convert.ToDateTime(((DbDataReader)(object)rd)["createdAt"]),
							attendanceDate = Convert.ToDateTime(((DbDataReader)(object)rd)["attendancedate"]),
							attendanceStatus = ((DbDataReader)(object)rd)["attendancestatus"].ToString(),
							newRequest = Convert.ToBoolean(((DbDataReader)(object)rd)["newRequest"]),
							remark = ((DbDataReader)(object)rd)["remark"].ToString(),
							projectManagerAppr = Convert.ToBoolean(((DbDataReader)(object)rd)["projectManagerAppr"])
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

		public List<AttendanceManage> GetAllAttendanceForProjectManager(int projectManager, int EmpId)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Expected O, but got Unknown
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Expected O, but got Unknown
			try
			{
				List<AttendanceManage> list = new List<AttendanceManage>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageattendance_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_id", (object)EmpId);
						cmd.Parameters.AddWithValue("v_projectmanager", (object)projectManager);
						cmd.Parameters.AddWithValue("v_action", (object)"getallforprojectmanager");
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader rd = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)rd).HasRows)
								{
									while (((DbDataReader)(object)rd).Read())
									{
										list.Add(new AttendanceManage
										{
											id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
											EmpId = Convert.ToInt32(((DbDataReader)(object)rd)["EmpId"]),
											projectManager = Convert.ToInt32(((DbDataReader)(object)rd)["projectManager"]),
											createdAt = Convert.ToDateTime(((DbDataReader)(object)rd)["createdAt"]),
											attendanceDate = Convert.ToDateTime(((DbDataReader)(object)rd)["attendancedate"]),
											attendanceStatus = ((DbDataReader)(object)rd)["attendancestatus"].ToString(),
											newRequest = Convert.ToBoolean(((DbDataReader)(object)rd)["newRequest"]),
											projectManagerName = ((DbDataReader)(object)rd)["projectManagerName"].ToString(),
											EmpName = ((DbDataReader)(object)rd)["EmpName"].ToString(),
											remark = ((DbDataReader)(object)rd)["remark"].ToString(),
											absent = Convert.ToInt32(((DbDataReader)(object)rd)["TotalAbsent"]),
											present = Convert.ToInt32(((DbDataReader)(object)rd)["TotalPresent"]),
											projectManagerAppr = Convert.ToBoolean(((DbDataReader)(object)rd)["projectManagerAppr"])
										});
									}
								}
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

		public List<AttendanceManage> GetAllAttendanceForProjectManager(int projectManager, int? limit = null, int? page = null)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Expected O, but got Unknown
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Expected O, but got Unknown
			try
			{
				List<AttendanceManage> list = new List<AttendanceManage>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageattendance_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@v_action", (object)"getallforprojectmanager");
						cmd.Parameters.AddWithValue("@v_projectmanager", (object)projectManager);
						cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader rd = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)rd).HasRows)
								{
									while (((DbDataReader)(object)rd).Read())
									{
										list.Add(new AttendanceManage
										{
											id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
											EmpId = Convert.ToInt32(((DbDataReader)(object)rd)["EmpId"]),
											projectManager = Convert.ToInt32(((DbDataReader)(object)rd)["projectManager"]),
											createdAt = Convert.ToDateTime(((DbDataReader)(object)rd)["createdAt"]),
											attendanceDate = Convert.ToDateTime(((DbDataReader)(object)rd)["attendancedate"]),
											attendanceStatus = ((DbDataReader)(object)rd)["attendancestatus"].ToString(),
											newRequest = Convert.ToBoolean(((DbDataReader)(object)rd)["newRequest"]),
											projectManagerName = ((DbDataReader)(object)rd)["projectManagerName"].ToString(),
											EmpName = ((DbDataReader)(object)rd)["EmpName"].ToString(),
											remark = ((DbDataReader)(object)rd)["remark"].ToString(),
											projectManagerAppr = Convert.ToBoolean(((DbDataReader)(object)rd)["projectManagerAppr"])
										});
									}
								}
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

		public List<AllAttendance> GetAllAttendanceForPmByMonth(int year, int month, int projectManager, int? limit = null, int? page = null)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Expected O, but got Unknown
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Expected O, but got Unknown
			try
			{
				List<AllAttendance> list = new List<AllAttendance>();
				Dictionary<int, AllAttendance> attendanceDict = new Dictionary<int, AllAttendance>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageattendance_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@v_action", (object)"showAllAttendance");
						cmd.Parameters.AddWithValue("@v_projectmanager", (object)projectManager);
						cmd.Parameters.AddWithValue("@v_id", (object)0);
						cmd.Parameters.AddWithValue("@v_year", (object)year);
						cmd.Parameters.AddWithValue("@v_month", (object)month);
						cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
						string curcorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + curcorName + "\";", con);
						try
						{
							NpgsqlDataReader rd = fetchCmd.ExecuteReader();
							try
							{
								while (((DbDataReader)(object)rd).Read())
								{
									int empId = Convert.ToInt32(((DbDataReader)(object)rd)["EmpId"]);
									string empName = ((DbDataReader)(object)rd)["EmpName"].ToString();
									int present = Convert.ToInt32(((DbDataReader)(object)rd)["TotalPresent"]);
									int absent = Convert.ToInt32(((DbDataReader)(object)rd)["TotalAbsent"]);
									AttendanceManage att = new AttendanceManage
									{
										EmpId = empId,
										EmpName = empName,
										present = present,
										absent = absent,
										attendanceDate = ((((DbDataReader)(object)rd)["attendanceDate"] != DBNull.Value) ? Convert.ToDateTime(((DbDataReader)(object)rd)["attendanceDate"]) : DateTime.MinValue),
										attendanceStatus = ((DbDataReader)(object)rd)["attendanceStatus"]?.ToString()
									};
									if (!attendanceDict.ContainsKey(empId))
									{
										attendanceDict[empId] = new AllAttendance
										{
											EmpId = empId,
											EmpName = empName,
											month = month,
											present = present,
											absent = absent,
											showAll = new List<AttendanceManage> { att }
										};
									}
									else
									{
										attendanceDict[empId].showAll.Add(att);
									}
								}
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
						NpgsqlCommand closeCmd = new NpgsqlCommand("close \"" + curcorName + "\";", con, tran);
						try
						{
							((DbCommand)(object)closeCmd).ExecuteNonQuery();
						}
						finally
						{
							((IDisposable)closeCmd)?.Dispose();
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
				return attendanceDict.Values.ToList();
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

		public bool AttendanceApproval(int id, bool status, string remark)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_ManageAttendance", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"approval");
				cmd.Parameters.AddWithValue("@projectManagerappr", (object)status);
				cmd.Parameters.AddWithValue("@id", (object)id);
				cmd.Parameters.AddWithValue("@remark", (object)(status ? null : remark));
				((DbConnection)(object)con).Open();
				int res = ((DbCommand)(object)cmd).ExecuteNonQuery();
				return res > 0;
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

		public List<AttendanceManage> getAttendanceCount(int projectManager, string searchTerm = null)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Expected O, but got Unknown
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<AttendanceManage> list = new List<AttendanceManage>();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageAttendance_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"Totalcountpa");
						cmd.Parameters.AddWithValue("v_projectmanager", (object)projectManager);
						cmd.Parameters.AddWithValue("v_id", (object)DBNull.Value);
						cmd.Parameters.AddWithValue("v_year", (object)DateTime.Now.Year);
						cmd.Parameters.AddWithValue("v_month", (object)(DateTime.Now.Month - 1));
						cmd.Parameters.AddWithValue("v_limit", (object)DBNull.Value);
						cmd.Parameters.AddWithValue("v_page", (object)DBNull.Value);
						cmd.Parameters.AddWithValue("v_date", (object)DBNull.Value);
						cmd.Parameters.AddWithValue("v_searchterm", (object)(string.IsNullOrEmpty(searchTerm) ? ((IConvertible)DBNull.Value) : ((IConvertible)searchTerm)));
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader res = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)res).HasRows)
								{
									while (((DbDataReader)(object)res).Read())
									{
										list.Add(new AttendanceManage
										{
											EmpId = Convert.ToInt32(((DbDataReader)(object)res)["EmpId"]),
											EmpName = ((DbDataReader)(object)res)["Emp_Name"].ToString(),
											present = Convert.ToInt32(((DbDataReader)(object)res)["presentcount"]),
											absent = Convert.ToInt32(((DbDataReader)(object)res)["absentcount"])
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

		public List<AttendanceManage> getAttendanceCountByMonth(int year, int month, int projectManager, int? limit = null, int? page = null)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Expected O, but got Unknown
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<AttendanceManage> list = new List<AttendanceManage>();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageattendance_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@v_action", (object)"Totalcountpa");
						cmd.Parameters.AddWithValue("@v_projectmanager", (object)projectManager);
						cmd.Parameters.AddWithValue("@v_id", (object)0);
						cmd.Parameters.AddWithValue("@v_year", (object)year);
						cmd.Parameters.AddWithValue("@v_month", (object)month);
						cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_date", (NpgsqlDbType)21, (object)DBNull.Value);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader res = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)res).HasRows)
								{
									while (((DbDataReader)(object)res).Read())
									{
										list.Add(new AttendanceManage
										{
											EmpId = Convert.ToInt32(((DbDataReader)(object)res)["EmpId"]),
											EmpName = ((DbDataReader)(object)res)["Emp_Name"].ToString(),
											present = Convert.ToInt32(((DbDataReader)(object)res)["presentcount"]),
											absent = Convert.ToInt32(((DbDataReader)(object)res)["absentcount"])
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

		public List<AttendanceManage> getReportAttendance(int month, int year, int projectManager, int EmpId)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Expected O, but got Unknown
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Expected O, but got Unknown
			try
			{
				List<AttendanceManage> list = new List<AttendanceManage>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageattendance_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"getRepo");
						cmd.Parameters.AddWithValue("v_month", (object)month);
						cmd.Parameters.AddWithValue("v_year", (object)year);
						cmd.Parameters.AddWithValue("v_projectmanager", (object)projectManager);
						cmd.Parameters.AddWithValue("v_id", (object)EmpId);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader rd = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)rd).HasRows)
								{
									while (((DbDataReader)(object)rd).Read())
									{
										list.Add(new AttendanceManage
										{
											id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
											EmpId = Convert.ToInt32(((DbDataReader)(object)rd)["EmpId"]),
											projectManager = Convert.ToInt32(((DbDataReader)(object)rd)["projectManager"]),
											createdAt = Convert.ToDateTime(((DbDataReader)(object)rd)["createdAt"]),
											attendanceDate = Convert.ToDateTime(((DbDataReader)(object)rd)["attendancedate"]),
											attendanceStatus = ((DbDataReader)(object)rd)["attendancestatus"].ToString(),
											newRequest = Convert.ToBoolean(((DbDataReader)(object)rd)["newRequest"]),
											projectManagerName = ((DbDataReader)(object)rd)["projectManagerName"].ToString(),
											EmpName = ((DbDataReader)(object)rd)["EmpName"].ToString(),
											absent = Convert.ToInt32(((DbDataReader)(object)rd)["TotalAbsent"]),
											present = Convert.ToInt32(((DbDataReader)(object)rd)["TotalPresent"]),
											remark = ((DbDataReader)(object)rd)["remark"].ToString(),
											projectManagerAppr = Convert.ToBoolean(((DbDataReader)(object)rd)["projectManagerAppr"])
										});
									}
								}
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
						NpgsqlCommand closeCmd = new NpgsqlCommand(cursorName, con, tran);
						try
						{
							((DbCommand)(object)closeCmd).CommandText = "close \"" + cursorName + "\";";
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

		public bool toKnowToday(int EmpId, DateTime attendanceDate)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				cmd = new NpgsqlCommand("SELECT COUNT(*) FROM ManageAttendance WHERE EmpId = @EmpId AND attendancedate = @Date", con);
				cmd.Parameters.AddWithValue("@EmpId", (object)EmpId);
				cmd.Parameters.AddWithValue("@Date", (object)attendanceDate);
				int count = (int)((DbCommand)(object)cmd).ExecuteScalar();
				if (count > 0)
				{
					return true;
				}
				return false;
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

		public List<AttendanceManage> chardata(int EmpId)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_ManageAttendance", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"chardata");
				cmd.Parameters.AddWithValue("@EmpId", (object)EmpId);
				List<AttendanceManage> list = new List<AttendanceManage>();
				((DbConnection)(object)con).Open();
				NpgsqlDataReader data = cmd.ExecuteReader();
				if (((DbDataReader)(object)data).HasRows)
				{
					while (((DbDataReader)(object)data).Read())
					{
						list.Add(new AttendanceManage
						{
							present = Convert.ToInt32(((DbDataReader)(object)data)["PresentCount"]),
							absent = Convert.ToInt32(((DbDataReader)(object)data)["AbsentCount"]),
							total = Convert.ToInt32(((DbDataReader)(object)data)["total"])
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

		public byte[] ConvertExcelFile(int month, int year, int userObj, int EmpId)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			List<AttendanceManage> data = getReportAttendance(month, year, userObj, EmpId);
			if (data.Any())
			{
				XLWorkbook workbook = new XLWorkbook();
				try
				{     
					IXLWorksheet worksheet = workbook.Worksheets.Add("Attendance Report");
					worksheet.Cell(1, 1).Value = "Name : " + data[0].EmpName;
					worksheet.Cell(1, 5).Value = $"Total Present : {data[0].present}";
					worksheet.Cell(1, 9).Value = $"Total Absent : {data[0].absent}";
					worksheet.Cell(1, 13).Value = "Month : " + data[0].attendanceDate.ToString("MMMM");
					IXLRange range = worksheet.Range("A1:D1");
					IXLRange range2 = worksheet.Range("E1:H1");
					IXLRange range3 = worksheet.Range("I1:L1");
					IXLRange range4 = worksheet.Range("M1:P1");
					((IXLRangeBase)range).Merge();
					((IXLRangeBase)range2).Merge();
					((IXLRangeBase)range3).Merge();
					((IXLRangeBase)range4).Merge();
					((IXLRangeBase)range).Style.Alignment.SetHorizontal((XLAlignmentHorizontalValues)0);
					((IXLRangeBase)range).Style.Alignment.SetVertical((XLAlignmentVerticalValues)1);
					((IXLRangeBase)range2).Style.Alignment.SetHorizontal((XLAlignmentHorizontalValues)0);
					((IXLRangeBase)range2).Style.Alignment.SetVertical((XLAlignmentVerticalValues)1);
					((IXLRangeBase)range3).Style.Alignment.SetHorizontal((XLAlignmentHorizontalValues)0);
					((IXLRangeBase)range3).Style.Alignment.SetVertical((XLAlignmentVerticalValues)1);
					((IXLRangeBase)range4).Style.Alignment.SetHorizontal((XLAlignmentHorizontalValues)0);
					((IXLRangeBase)range4).Style.Alignment.SetVertical((XLAlignmentVerticalValues)1);
					int daysInMonth = DateTime.DaysInMonth(year, month);
					for (int day = 1; day <= daysInMonth; day++)
					{
						DateTime currentDate = new DateTime(year, month, day);
						worksheet.Cell(2, day).Value = currentDate.ToString("dd");
					}
					for (int i = 1; i <= daysInMonth; i++)
					{
						DateTime currentDate2 = new DateTime(year, month, i);
						string status = data.FirstOrDefault((AttendanceManage x) => x.attendanceDate.Date == currentDate2.Date)?.attendanceStatus ?? "-";
						worksheet.Cell(3, i).Value = status;
					}
					 MemoryStream stream = new MemoryStream();
					workbook.SaveAs((Stream)stream);
					return stream.ToArray();
				}
				finally
				{
					((IDisposable)workbook)?.Dispose();
				}
			}
			return null;
		}

		public byte[] ConvertExcelFileOfAll(int month, int year, int userObj)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			List<AllAttendance> allData = GetAllAttendanceForPmByMonth(year, month, userObj);
			if (allData.Any())
			{
				XLWorkbook workbook = new XLWorkbook();
				try
				{
					IXLWorksheet worksheet = workbook.Worksheets.Add("Attendance Report");
					int currentRow = 1;
					((IXLRangeBase)worksheet.Range(currentRow, 1, currentRow, 5)).Merge();
					worksheet.Cell(currentRow, 1).Value = "(-) means not available";
					((IXLFontBase)((IXLRangeBase)worksheet.Range(currentRow, 1, currentRow, 5)).Style.Font).Bold = true;
					((IXLRangeBase)worksheet.Range(currentRow, 1, currentRow, 5)).Style.Fill.BackgroundColor = XLColor.LightGray;
					((IXLRangeBase)worksheet.Range(currentRow, 1, currentRow, 5)).Style.Alignment.SetHorizontal((XLAlignmentHorizontalValues)6);
					((IXLRangeBase)worksheet.Range(currentRow, 12, currentRow, 16)).Merge();
					worksheet.Cell(currentRow, 12).Value = "Publish Date & Time : " + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt");
					((IXLFontBase)((IXLRangeBase)worksheet.Range(currentRow, 12, currentRow, 16)).Style.Font).Bold = true;
					((IXLRangeBase)worksheet.Range(currentRow, 12, currentRow, 16)).Style.Fill.BackgroundColor = XLColor.LightGray;
					((IXLRangeBase)worksheet.Range(currentRow, 12, currentRow, 16)).Style.Alignment.SetHorizontal((XLAlignmentHorizontalValues)7);
					currentRow++;
					foreach (AllAttendance emp in allData)
					{
						worksheet.Cell(currentRow, 1).Value = "Name : " + emp.EmpName;
						worksheet.Cell(currentRow, 5).Value = $"Total Present : {emp.present}";
						worksheet.Cell(currentRow, 9).Value = $"Total Absent : {emp.absent}";
						worksheet.Cell(currentRow, 13).Value = "Month : " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
						((IXLRangeBase)((IXLRangeBase)worksheet.Range(currentRow, 1, currentRow, 4)).Merge()).Style.Alignment.SetHorizontal((XLAlignmentHorizontalValues)0);
						((IXLRangeBase)((IXLRangeBase)worksheet.Range(currentRow, 5, currentRow, 8)).Merge()).Style.Alignment.SetHorizontal((XLAlignmentHorizontalValues)0);
						((IXLRangeBase)((IXLRangeBase)worksheet.Range(currentRow, 9, currentRow, 12)).Merge()).Style.Alignment.SetHorizontal((XLAlignmentHorizontalValues)0);
						((IXLRangeBase)((IXLRangeBase)worksheet.Range(currentRow, 13, currentRow, 16)).Merge()).Style.Alignment.SetHorizontal((XLAlignmentHorizontalValues)0);
						currentRow++;
						int daysInMonth = DateTime.DaysInMonth(year, month);
						for (int day = 1; day <= daysInMonth; day++)
						{
							worksheet.Cell(currentRow, day).Value = day.ToString("00");
						}
						currentRow++;
						for (int i = 1; i <= daysInMonth; i++)
						{
							DateTime currentDate = new DateTime(year, month, i);
							string status = emp.showAll.FirstOrDefault((AttendanceManage x) => x.attendanceDate.Date == currentDate.Date)?.attendanceStatus ?? "-";
							worksheet.Cell(currentRow, i).Value = status;
						}
						currentRow += 2;
					}
					 MemoryStream stream = new MemoryStream();
					workbook.SaveAs((Stream)stream);
					return stream.ToArray();
				}
				finally
				{
					((IDisposable)workbook)?.Dispose();
				}
			}
			return null;
		}

		public bool InsertEmpReport(EmpReportModel model, out string message)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			message = string.Empty;
			try
			{
				NpgsqlCommand cmd = new NpgsqlCommand("call sp_ManageEmpReport(@v_action,@v_pmid,Null,@v_ProjectId,@v_unit,@v_annualtarget,@v_targetuptoreviewmonth,@v_achievementduringreviewmonth,@v_cumulativeachievement,@v_benefitingdepartments,@v_remarks,Null);", con);
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("@v_action", (object)"insert");
					cmd.Parameters.AddWithValue("@v_ProjectId", (object)model.ProjectId);
					cmd.Parameters.AddWithValue("@v_pmid", (object)model.PmId);
					cmd.Parameters.AddWithValue("@v_unit", (object)model.Unit);
					cmd.Parameters.AddWithValue("@v_annualtarget", (object)model.AnnualTarget);
					cmd.Parameters.AddWithValue("@v_targetuptoreviewmonth", (object)model.TargetUptoReviewMonth);
					cmd.Parameters.AddWithValue("@v_achievementduringreviewmonth", (object)model.AchievementDuringReviewMonth);
					cmd.Parameters.AddWithValue("@v_cumulativeachievement", (object)model.CumulativeAchievement);
					cmd.Parameters.AddWithValue("@v_benefitingdepartments", (object)model.BenefitingDepartments);
					cmd.Parameters.AddWithValue("@v_remarks", (object)(model.Remarks ?? string.Empty));
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
			}
		}

		public List<EmpReportModel> GetEmpReport(int userid, int? limit = null, int? page = null, int? id = null, int? month = null, int? year = null)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Expected O, but got Unknown
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Expected O, but got Unknown
			try
			{
				List<EmpReportModel> list = new List<EmpReportModel>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageempreport_cursor", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@v_action", (object)"get");
						cmd.Parameters.AddWithValue("@v_projectmanager", (object)userid);
						cmd.Parameters.AddWithValue("@v_id", id.HasValue ? ((object)id.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_month", month.HasValue ? ((object)month.Value) : DBNull.Value);
						cmd.Parameters.AddWithValue("@v_year", year.HasValue ? ((object)year.Value) : DBNull.Value);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader res = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)res).HasRows)
								{
									while (((DbDataReader)(object)res).Read())
									{
										list.Add(new EmpReportModel
										{
											ProjectId = ((((DbDataReader)(object)res)["ProjectId"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)res)["ProjectId"]) : 0),
											ProjectName = ((((DbDataReader)(object)res)["title"] != DBNull.Value) ? ((DbDataReader)(object)res)["title"].ToString() : ""),
											Unit = ((((DbDataReader)(object)res)["Unit"] != DBNull.Value) ? ((DbDataReader)(object)res)["Unit"].ToString() : ""),
											AnnualTarget = ((((DbDataReader)(object)res)["AnnualTarget"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)res)["AnnualTarget"]) : 0),
											TargetUptoReviewMonth = ((((DbDataReader)(object)res)["TargetUptoReviewMonth"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)res)["TargetUptoReviewMonth"]) : 0),
											AchievementDuringReviewMonth = ((((DbDataReader)(object)res)["AchievementDuringReviewMonth"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)res)["AchievementDuringReviewMonth"]) : 0),
											CumulativeAchievement = ((((DbDataReader)(object)res)["CumulativeAchievement"] != DBNull.Value) ? Convert.ToInt32(((DbDataReader)(object)res)["CumulativeAchievement"]) : 0),
											BenefitingDepartments = ((((DbDataReader)(object)res)["BenefitingDepartments"] != DBNull.Value) ? ((DbDataReader)(object)res)["BenefitingDepartments"].ToString() : ""),
											Remarks = ((((DbDataReader)(object)res)["Remarks"] != DBNull.Value) ? ((DbDataReader)(object)res)["Remarks"].ToString() : ""),
											CreatedAt = Convert.ToDateTime(((DbDataReader)(object)res)["CreatedAt"]).ToString("dd-mm-yyyy")
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
			}
		}

		public bool InsertFeedback(FeedbackModel model)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			try
			{
				NpgsqlCommand cmd = new NpgsqlCommand("CALL sp_manage_feedback(@p_title, @p_feedback_type, @p_description, @p_userid,null::boolean,@p_action);", con);
				try
				{
					((DbCommand)(object)cmd).CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("@p_title", (object)(model.Title ?? ""));
					cmd.Parameters.AddWithValue("@p_feedback_type", (object)model.FeedbackType);
					cmd.Parameters.AddWithValue("@p_description", (object)model.Description);
					cmd.Parameters.AddWithValue("@p_userid", (object)model.UserId);
					cmd.Parameters.AddWithValue("@p_action", (object)"insertfeedback");
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
			}
		}

		public List<FeedbackModel> GetFeedbacks(int userid)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Expected O, but got Unknown
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Expected O, but got Unknown
			try
			{
				List<FeedbackModel> list = new List<FeedbackModel>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_getfeedbacks_cursor", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@v_action", (object)"getfeedbacks");
						cmd.Parameters.AddWithValue("@v_projectmanager", (object)userid);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader res = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)res).HasRows)
								{
									while (((DbDataReader)(object)res).Read())
									{
										list.Add(new FeedbackModel
										{
											Title = ((((DbDataReader)(object)res)["title"] != DBNull.Value) ? ((DbDataReader)(object)res)["title"].ToString() : ""),
											FeedbackType = ((((DbDataReader)(object)res)["feedback_type"] != DBNull.Value) ? ((DbDataReader)(object)res)["feedback_type"].ToString() : ""),
											Description = ((((DbDataReader)(object)res)["description"] != DBNull.Value) ? ((DbDataReader)(object)res)["description"].ToString() : "")
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
			}
		}

		public bool InsertProjectStatus(UpdateProjectStatus obj)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			try
			{
				NpgsqlConnection val = con;
				try
				{
					((DbConnection)(object)con).Open();
					NpgsqlCommand cmd = new NpgsqlCommand("CALL public.sp_managestagestatus(\r\n            NULL,\r\n            NULL,\r\n            @v_comment,\r\n            @v_completionprecentage,\r\n            NULL,\r\n            NULL,\r\n            NULL,\r\n            NULL,\r\n            @v_project_id,\r\n            @v_completionstatus,\r\n            @v_action,\r\n            NULL\r\n        );", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.Text;
						((DbParameter)(object)cmd.Parameters.Add("@v_project_id", (NpgsqlDbType)9)).Value = obj.projectId;
						((DbParameter)(object)cmd.Parameters.Add("@v_completionstatus", (NpgsqlDbType)2)).Value = obj.projectStatus;
						((DbParameter)(object)cmd.Parameters.Add("@v_completionprecentage", (NpgsqlDbType)22)).Value = ((object)obj.CompletionPrecentage) ?? ((object)DBNull.Value);
						((DbParameter)(object)cmd.Parameters.Add("@v_comment", (NpgsqlDbType)19)).Value = ((object)obj.remark) ?? ((object)DBNull.Value);
						((DbParameter)(object)cmd.Parameters.Add("@v_action", (NpgsqlDbType)22)).Value = "updateProjectStatus";
						((DbCommand)(object)cmd).ExecuteNonQuery();
					}
					finally
					{
						((IDisposable)cmd)?.Dispose();
					}
					return true;
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			catch (Exception innerException)
			{
				throw new Exception("Error inserting stage status", innerException);
			}
			finally
			{
				if (((DbConnection)(object)con).State == ConnectionState.Open)
				{
					((DbConnection)(object)con).Close();
				}
			}
		}

		public List<UpdateProjectStatus> LastProjectStatus(int projectId)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Expected O, but got Unknown
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<UpdateProjectStatus> list = new List<UpdateProjectStatus>();
				RemoteSensingProject.Models.Admin.main.Project_model pm = new RemoteSensingProject.Models.Admin.main.Project_model();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageprojectsubstances_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"selectlaststatusofproject");
						cmd.Parameters.AddWithValue("v_project_id", (object)projectId);
						cmd.Parameters.AddWithValue("v_id", (object)0);
						cmd.Parameters.AddWithValue("v_limit", (object)0);
						cmd.Parameters.AddWithValue("v_page", (object)0);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader rd = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)rd).HasRows)
								{
									while (((DbDataReader)(object)rd).Read())
									{
										list.Add(new UpdateProjectStatus
										{
											CompletionPrecentage = ((DbDataReader)(object)rd)["completepercentage"].ToString()
										});
									}
								}
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

        #region Manpower Requests
        public List<ManpowerRequests> GetManpowerRequestsInDivision(int? limit = null, int? page = null,string searchTerm=null)
        {
            try
            {
                ((DbConnection)(object)con).Open();
                List<ManpowerRequests> list = new List<ManpowerRequests>();
                RemoteSensingProject.Models.Admin.main.Project_model pm = new RemoteSensingProject.Models.Admin.main.Project_model();
                NpgsqlTransaction tran = con.BeginTransaction();
                try
                {
                    NpgsqlCommand cmd = new NpgsqlCommand("fn_manageprshasanpanel", con, tran);
                    try
                    {
                        ((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("v_action", (object)"selectmanpowerrequests");
                        cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@v_searchterm", (object)(string.IsNullOrEmpty(searchTerm) ? ((IConvertible)DBNull.Value) : ((IConvertible)searchTerm)));
                        string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
                        NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
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
                                        ManpowerRequests data = new ManpowerRequests
                                        {
                                            id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
											divisionName = ((DbDataReader)(object)rd)["devisionName"].ToString(),
                                            manpowerrequests = rd["manpower"] != DBNull.Value? Convert.ToInt32(rd["manpower"]) : 0,
                                            manpowerremaining = rd["remaining_outsource"] != DBNull.Value? Convert.ToInt32(rd["remaining_outsource"]) : 0,
                                            manpoweradded = rd["added_outsource"] != DBNull.Value? Convert.ToInt32(rd["added_outsource"]) : 0
                                        };
                                        if (firstRow)
                                        {
                                            data.Pagination = new ApiCommon.PaginationInfo
                                            {
                                                TotalPages = Convert.ToInt32(((DbDataReader)(object)rd)["totalpages"]),
                                                TotalRecords = Convert.ToInt32(((DbDataReader)(object)rd)["totalrecords"]),
                                                PageNumber = page.GetValueOrDefault(),
                                                PageSize = limit.GetValueOrDefault()
                                            };
                                            firstRow = false;
                                        }
                                        list.Add(data);
                                    }
                                }
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
        public bool AddManpower(AddManPower os)
        {
            ((DbConnection)(object)con).Open();
            using (var transaction = con.BeginTransaction())
            {
                try
                {
                    foreach (var outsourceId in os.Outsource)
                    {
                        using (var cmd = new NpgsqlCommand(
                            "CALL sp_managemanpower(p_action=>@p_action, p_id=>@p_id, p_designationid=>@p_designationid, p_outsourceid=>@p_outsourceid)",
                            con, transaction))
                        {
                            cmd.CommandType = CommandType.Text;

                            cmd.Parameters.Add("@p_action", NpgsqlDbType.Varchar).Value = "addmanpower";
                            cmd.Parameters.Add("@p_id", NpgsqlDbType.Integer).Value = os.DivisionId;
                            cmd.Parameters.Add("@p_designationid", NpgsqlDbType.Integer).Value = os.DesignationId;
                            cmd.Parameters.Add("@p_outsourceid", NpgsqlDbType.Integer).Value = outsourceId;

                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (PostgresException pgEx)
                {
                    transaction.Rollback();

                    // Stored procedure ka exact error message
                    throw new Exception(pgEx.MessageText);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
        }

        public bool AllocateManpower(AddManPower os)
        {
            ((DbConnection)(object)con).Open();
            using (var transaction = con.BeginTransaction())
            {
                try
                {
                    foreach (var outsourceId in os.Outsource)
                    {
                        using (var cmd = new NpgsqlCommand(
                            "CALL sp_managemanpower(p_action=>@p_action, p_pmid=>@p_pmid, p_outsourceid=>@p_outsourceid)",
                            con, transaction))
                        {
                            cmd.CommandType = CommandType.Text;

                            cmd.Parameters.Add("@p_action", NpgsqlDbType.Varchar).Value = "allocatemanpower";
                            cmd.Parameters.Add("@p_pmid", NpgsqlDbType.Integer).Value = os.PmId;
                            cmd.Parameters.Add("@p_outsourceid", NpgsqlDbType.Integer).Value = outsourceId;

                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (PostgresException pgEx)
                {
                    transaction.Rollback();

                    // Stored procedure ka exact error message
                    throw new Exception(pgEx.MessageText);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
        }

        public bool DeAllocateManpower(int outsourceid)
        {
            ((DbConnection)(object)con).Open();
            using (var transaction = con.BeginTransaction())
            {
                try
                {
                    using (var cmd = new NpgsqlCommand(
                        "CALL sp_managemanpower(p_action=>@p_action, p_outsourceid=>@p_outsourceid)",
                        con, transaction))
                    {
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Add("@p_action", NpgsqlDbType.Varchar).Value = "deallocatemanpower";
                        cmd.Parameters.Add("@p_outsourceid", NpgsqlDbType.Integer).Value = outsourceid;

                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
        }

        public PrashasanDashboard GetPrashasanDashboardData()
        {
            try
            {
                ((DbConnection)(object)con).Open();
                PrashasanDashboard data = new PrashasanDashboard();
                NpgsqlTransaction tran = con.BeginTransaction();
                try
                {
                    NpgsqlCommand cmd = new NpgsqlCommand("fn_manageprshasanpanel", con, tran);
                    try
                    {
                        ((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("v_action", (object)"prashasandashboarddata");
                        cmd.Parameters.AddWithValue("@v_limit", DBNull.Value);
                        cmd.Parameters.AddWithValue("@v_page", DBNull.Value);
						cmd.Parameters.AddWithValue("@v_searchterm", DBNull.Value);
                        string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
                        NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
                        try
                        {
                            NpgsqlDataReader rd = fetchCmd.ExecuteReader();
                            try
                            {
                                if (((DbDataReader)(object)rd).HasRows)
                                {
                                    while (((DbDataReader)(object)rd).Read())
                                    {
                                        data = new PrashasanDashboard
                                        {
                                            totalDesignation = rd["totalDesignation"] != DBNull.Value ? Convert.ToInt32(rd["totalDesignation"]) : 0,
                                            totalOutsource = rd["totalOutsource"] != DBNull.Value ? Convert.ToInt32(rd["totaloutsource"]) : 0,
                                            totalRemaining = rd["totalRemaining"] != DBNull.Value ? Convert.ToInt32(rd["totalremaining"]) : 0,
                                            totalRequest = rd["totalRequest"] != DBNull.Value ? Convert.ToInt32(rd["totalRequest"]) : 0
                                        };
										//list = data;
                                    }
                                }
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
                return data;
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

        public List<OuterSource> OutsourceNotInDivision(int id)
        {
            try
            {
                List<OuterSource> list = new List<OuterSource>();
                ((DbConnection)(object)con).Open();
                NpgsqlTransaction tran = con.BeginTransaction();
                try
                {
                    NpgsqlCommand cmd = new NpgsqlCommand("fn_manageprshasanpanel", con, tran);
                    try
                    {
                        ((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@v_action", (object)"selectoutsourcenotindivision");
                        cmd.Parameters.AddWithValue("@v_id", (object)id);
                        string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
                        NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
                        try
                        {
                            NpgsqlDataReader rd = fetchCmd.ExecuteReader();
                            try
                            {
                                if (((DbDataReader)(object)rd).HasRows)
                                {
                                    //bool firstRow = true;
                                    while (((DbDataReader)(object)rd).Read())
                                    {
                                        OuterSource data = new OuterSource
                                        {
                                            Id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
                                            EmpName = ((DbDataReader)(object)rd)["emp_name"].ToString(),
                                            mobileNo = Convert.ToInt64(((DbDataReader)(object)rd)["emp_mobile"]),
                                            email = ((DbDataReader)(object)rd)["emp_email"].ToString(),
                                            gender = ((DbDataReader)(object)rd)["emp_gender"].ToString(),
                                            designationname = ((DbDataReader)(object)rd)["designationname"].ToString(),
                                            designationid = Convert.ToInt32(rd["designationid"])
                                        };
                                        list.Add(data);
                                    }
                                }
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

        public List<OuterSource> OutsourceOfDivision(int divisionid,int designationid)
        {
            try
            {
                List<OuterSource> list = new List<OuterSource>();
                ((DbConnection)(object)con).Open();
                NpgsqlTransaction tran = con.BeginTransaction();
                try
                {
                    NpgsqlCommand cmd = new NpgsqlCommand("fn_manageprshasanpanel", con, tran);
                    try
                    {
                        ((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@v_action", (object)"selectoutsourceofdivision");
                        cmd.Parameters.AddWithValue("@v_id", (object)divisionid);
                        cmd.Parameters.AddWithValue("@v_designationid", (object)designationid);
                        string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
                        NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
                        try
                        {
                            NpgsqlDataReader rd = fetchCmd.ExecuteReader();
                            try
                            {
                                if (((DbDataReader)(object)rd).HasRows)
                                {
                                    //bool firstRow = true;
                                    while (((DbDataReader)(object)rd).Read())
                                    {
                                        OuterSource data = new OuterSource
                                        {
                                            Id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
                                            EmpName = ((DbDataReader)(object)rd)["emp_name"].ToString(),
                                            mobileNo = Convert.ToInt64(((DbDataReader)(object)rd)["emp_mobile"]),
                                            email = ((DbDataReader)(object)rd)["emp_email"].ToString(),
                                            gender = ((DbDataReader)(object)rd)["emp_gender"].ToString(),
                                            designationname = ((DbDataReader)(object)rd)["designationname"].ToString(),
                                            designationid = Convert.ToInt32(rd["designationid"])
                                        };
                                        list.Add(data);
                                    }
                                }
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

        public List<ManpowerRequests> GetManpowerRequestsInDesignation(int id, int? limit = null, int? page = null, string searchTerm = null)
        {
            try
            {
                ((DbConnection)(object)con).Open();
                List<ManpowerRequests> list = new List<ManpowerRequests>();
                RemoteSensingProject.Models.Admin.main.Project_model pm = new RemoteSensingProject.Models.Admin.main.Project_model();
                NpgsqlTransaction tran = con.BeginTransaction();
                try
                {
                    NpgsqlCommand cmd = new NpgsqlCommand("fn_manageprshasanpanel", con, tran);
                    try
                    {
                        ((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("v_action", (object)"selectmanpowerwithdesignation");
                        cmd.Parameters.AddWithValue("@v_id", (object)id);
                        cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@v_searchterm", (object)(string.IsNullOrEmpty(searchTerm) ? ((IConvertible)DBNull.Value) : ((IConvertible)searchTerm)));
                        string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
                        NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
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
                                        ManpowerRequests data = new ManpowerRequests
                                        {
                                            id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
                                            divisionName = ((DbDataReader)(object)rd)["designationName"].ToString(),
                                            manpowerrequests = rd["manpower"] != DBNull.Value ? Convert.ToInt32(rd["manpower"]) : 0,
                                            manpowerremaining = rd["remaining_outsource"] != DBNull.Value ? Convert.ToInt32(rd["remaining_outsource"]) : 0,
                                            manpoweradded = rd["added_outsource"] != DBNull.Value ? Convert.ToInt32(rd["added_outsource"]) : 0
                                        };
                                        if (firstRow)
                                        {
                                            data.Pagination = new ApiCommon.PaginationInfo
                                            {
                                                TotalPages = Convert.ToInt32(((DbDataReader)(object)rd)["totalpages"]),
                                                TotalRecords = Convert.ToInt32(((DbDataReader)(object)rd)["totalrecords"]),
                                                PageNumber = page.GetValueOrDefault(),
                                                PageSize = limit.GetValueOrDefault()
                                            };
                                            firstRow = false;
                                        }
                                        list.Add(data);
                                    }
                                }
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

        public List<ManpowerRequests> GetManpowerRequestsInDesignationPmWise(int id,int? designationid = null, int? limit = null, int? page = null, string searchTerm = null)
        {
            try
            {
                ((DbConnection)(object)con).Open();
                List<ManpowerRequests> list = new List<ManpowerRequests>();
                Admin.main.Project_model pm = new Admin.main.Project_model();
                NpgsqlTransaction tran = con.BeginTransaction();
                try
                {
                    NpgsqlCommand cmd = new NpgsqlCommand("fn_manageprshasanpanel", con, tran);
                    try
                    {
                        ((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("v_action", (object)"selectmanpowerwithdesignationpmwise");
                        cmd.Parameters.AddWithValue("@v_id", (object)id);
                        cmd.Parameters.AddWithValue("@v_designationid", designationid.HasValue? (object)designationid : DBNull.Value);
                        cmd.Parameters.AddWithValue("@v_limit", limit.HasValue ? ((object)limit.Value) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@v_page", page.HasValue ? ((object)page.Value) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@v_searchterm", (object)(string.IsNullOrEmpty(searchTerm) ? ((IConvertible)DBNull.Value) : ((IConvertible)searchTerm)));
                        string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
                        NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
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
                                        ManpowerRequests data = new ManpowerRequests
                                        {
                                            id = Convert.ToInt32(((DbDataReader)(object)rd)["desgid"]),
                                            projectManager = ((DbDataReader)(object)rd)["name"].ToString(),
											pmid = Convert.ToInt32(((DbDataReader)(object)rd)["empid"]),
                                            manpowerrequests = rd["manpower"] != DBNull.Value ? Convert.ToInt32(rd["manpower"]) : 0,
                                            manpowerremaining = rd["remaining_outsource"] != DBNull.Value ? Convert.ToInt32(rd["remaining_outsource"]) : 0,
                                            manpoweradded = rd["added_outsource"] != DBNull.Value ? Convert.ToInt32(rd["added_outsource"]) : 0
                                        };
                                        if (firstRow)
                                        {
                                            data.Pagination = new ApiCommon.PaginationInfo
                                            {
                                                TotalPages = Convert.ToInt32(((DbDataReader)(object)rd)["totalpages"]),
                                                TotalRecords = Convert.ToInt32(((DbDataReader)(object)rd)["totalrecords"]),
                                                PageNumber = page.GetValueOrDefault(),
                                                PageSize = limit.GetValueOrDefault()
                                            };
                                            firstRow = false;
                                        }
                                        list.Add(data);
                                    }
                                }
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
        #endregion
    }
}