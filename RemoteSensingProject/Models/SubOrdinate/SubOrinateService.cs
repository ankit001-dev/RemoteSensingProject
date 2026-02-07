// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// SubOrinateService
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web.UI;
using DocumentFormat.OpenXml.Office.Word;
using Npgsql;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.SubOrdinate;

namespace RemoteSensingProject.Models.SubOrdinate
{
	public class SubOrinateService : DataFactory
	{
		public main.UserCredential getManagerDetails(string managerName)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			main.UserCredential _details = new main.UserCredential();
			try
			{
				NpgsqlCommand cmd = new NpgsqlCommand("sp_adminAddproject", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"getManagerDetails");
				cmd.Parameters.AddWithValue("@username", (object)managerName);
				((DbConnection)(object)con).Open();
				NpgsqlDataReader sdr = cmd.ExecuteReader();
				while (((DbDataReader)(object)sdr).Read())
				{
					_details = new main.UserCredential();
					_details.username = ((DbDataReader)(object)sdr)["username"].ToString();
					_details.userId = ((DbDataReader)(object)sdr)["userid"].ToString();
					_details.userRole = ((DbDataReader)(object)sdr)["userRole"].ToString();
				}
				((DbDataReader)(object)sdr).Close();
			}
			catch (Exception innerException)
			{
				throw new Exception("An error accured", innerException);
			}
			finally
			{
				((DbConnection)(object)con).Close();
			}
			return _details;
		}

		public List<main.ProjectList> getProjectBySubOrdinate(string userId)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			List<main.ProjectList> _list = new List<main.ProjectList>();
			main.ProjectList obj = null;
			try
			{
				NpgsqlCommand cmd = new NpgsqlCommand("sp_adminAddproject", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"GetProjectBySubOrdinate");
				cmd.Parameters.AddWithValue("@subOrdinate", (object)userId);
				((DbConnection)(object)con).Open();
				NpgsqlDataReader sdr = cmd.ExecuteReader();
				while (((DbDataReader)(object)sdr).Read())
				{
					obj = new main.ProjectList();
					obj.Id = Convert.ToInt32(((DbDataReader)(object)sdr)["id"]);
					obj.Title = ((DbDataReader)(object)sdr)["title"].ToString();
					obj.AssignDateString = Convert.ToDateTime(((DbDataReader)(object)sdr)["AssignDate"]).ToString("dd-MM-yyyy");
					obj.StartDateString = Convert.ToDateTime(((DbDataReader)(object)sdr)["StartDate"]).ToString("dd-MM-yyyy");
					obj.StartDate = Convert.ToDateTime(((DbDataReader)(object)sdr)["StartDate"]);
					obj.CompletionDate = Convert.ToDateTime(((DbDataReader)(object)sdr)["CompletionDate"]);
					obj.CompletionDatestring = Convert.ToDateTime(((DbDataReader)(object)sdr)["CompletionDate"]).ToString("dd-MM-yyyy");
					obj.Status = ((DbDataReader)(object)sdr)["status"].ToString();
					obj.CompleteionStatus = Convert.ToBoolean(((DbDataReader)(object)sdr)["CompleteStatus"]);
					obj.ApproveStatus = Convert.ToInt32(((DbDataReader)(object)sdr)["ApproveStatus"]);
					obj.CreatedBy = ((DbDataReader)(object)sdr)["name"].ToString();
					obj.projectType = ((DbDataReader)(object)sdr)["projectType"].ToString();
					obj.projectStatus = Convert.ToSingle(((DbDataReader)(object)sdr)["completionPercentage"]);
					obj.projectCode = ((((DbDataReader)(object)sdr)["projectCode"] != DBNull.Value) ? ((DbDataReader)(object)sdr)["projectCode"].ToString() : "N/A");
					_list.Add(obj);
				}
				((DbDataReader)(object)sdr).Close();
			}
			catch (Exception innerException)
			{
				throw new Exception("An error accured", innerException);
			}
			finally
			{
				((DbConnection)(object)con).Close();
			}
			return _list;
		}

		public bool InsertSubOrdinateProblem(main.Raise_Problem raise)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			try
			{
				NpgsqlCommand cmd = new NpgsqlCommand("call sp_managesubordinateprojectproblem(v_action=>@v_action,v_project_id=>@v_project_id,v_title=>@v_title,v_description=>@v_description,v_attachment=>@v_attachment)", con);
				cmd.Parameters.AddWithValue("@v_action", (object)"insertProblem");
				cmd.Parameters.AddWithValue("@v_project_id", (object)raise.Project_Id);
				cmd.Parameters.AddWithValue("@v_project_id", (object)raise.Project_Id);
				cmd.Parameters.AddWithValue("@v_title", (object)raise.Title);
				cmd.Parameters.AddWithValue("@v_description", (object)raise.Description);
				cmd.Parameters.AddWithValue("@v_attachment", (object)(raise.Attchment_Url ?? ""));
				((DbConnection)(object)con).Open();
				((DbCommand)(object)cmd).ExecuteNonQuery();
				return true;
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
		public main.UserCredential GetOutSourceId(string username)
		{
			try
			{
				main.UserCredential userData = new main.UserCredential();
				con.Open();
                using (NpgsqlTransaction tran = con.BeginTransaction())
				{
					using (NpgsqlCommand cmd = new NpgsqlCommand("fn_manageoutsource_cursor", con, tran))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@v_action", "getOutSourceById");
						cmd.Parameters.AddWithValue("@v_searchterm", username);
						string cursorName = (string)cmd.ExecuteScalar();
						using (NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran))
						{
							using (NpgsqlDataReader sdr = fetchCmd.ExecuteReader())
							{
								if (sdr.HasRows)
								{
									sdr.Read();
									userData.userId = sdr["id"].ToString();
									userData.username = sdr["emp_name"].ToString();
								}
							}
                        }
					}
					return userData;
				}
			}catch(Exception ex)
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

        public List<main.OutSource_Task> getOutSourceTask(int id, int? limit = null, int? page = null, string searchTerm = null, string statusFilter = null)
		{
			try
			{
				List<main.OutSource_Task> taskList = new List<main.OutSource_Task>();
				main.OutSource_Task task = null;
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_manageoutsource_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@v_action", NpgsqlTypes.NpgsqlDbType.Varchar).Value = "getTaskByOutSource";

                        cmd.Parameters.Add("@v_statusfilter", NpgsqlTypes.NpgsqlDbType.Varchar)
                            .Value = (object)statusFilter ?? DBNull.Value;

                        cmd.Parameters.Add("@v_id", NpgsqlTypes.NpgsqlDbType.Integer)
                            .Value = id;

                        cmd.Parameters.Add("@v_limit", NpgsqlTypes.NpgsqlDbType.Integer)
                            .Value = (object)limit ?? DBNull.Value;

                        cmd.Parameters.Add("@v_page", NpgsqlTypes.NpgsqlDbType.Integer)
                            .Value = (object)page ?? DBNull.Value;

                        cmd.Parameters.Add("@v_searchterm", NpgsqlTypes.NpgsqlDbType.Varchar)
                            .Value = (object)searchTerm ?? DBNull.Value;
                        string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("fetch all from \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader sdr = fetchCmd.ExecuteReader();
							try
							{
								bool firstRow = true;
								while (((DbDataReader)(object)sdr).Read())
								{
									task = new main.OutSource_Task();
									task.id = Convert.ToInt32(((DbDataReader)(object)sdr)["id"]);
									task.Title = ((DbDataReader)(object)sdr)["title"].ToString();
									task.Description = ((DbDataReader)(object)sdr)["description"].ToString();
									task.CompleteStatus = Convert.ToBoolean(((DbDataReader)(object)sdr)["completeStatus"]);
									task.Status = ((DbDataReader)(object)sdr)["Status"].ToString();
									task.projectId = Convert.ToInt32(sdr["projectid"]);
									task.projectName = sdr["ProjectTitle"].ToString();
									task.AssignTaskId = Convert.ToInt32(sdr["AssignTaskId"]);
									task.ApprovalStatus = Convert.ToBoolean(sdr["OutSourceCompleteStatus"]);
									taskList.Add(task);
									if (firstRow)
									{
										task.Pagination = new ApiCommon.PaginationInfo
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
				return taskList;
			}
			catch (Exception innerException)
			{
				throw new Exception("An error accure", innerException);
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

		public bool AddOutSourceTask(main.OutSource_Task task)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			NpgsqlCommand cmd = new NpgsqlCommand("CALL sp_manageoutsourcetask(@v_action, @v_id, @v_empid, NULL, NULL,@v_response , null::smallint, NULL, NULL, NULL)", con);
			((DbCommand)(object)cmd).CommandType = CommandType.Text;
			cmd.Parameters.AddWithValue("@v_action", (object)"insertOutsource");
			cmd.Parameters.AddWithValue("@v_response", (object)task.Reason);
			cmd.Parameters.AddWithValue("@v_id", (object)task.id);
			cmd.Parameters.AddWithValue("@v_empId", (object)task.EmpId);
			((DbConnection)(object)con).Open();
			((DbCommand)(object)cmd).ExecuteNonQuery();
			return true;
		}

		public main.DashboardCount GetDashboardCounts(int userId)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Expected O, but got Unknown
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
						cmd.Parameters.AddWithValue("v_action", (object)"managesubordinatedashboard");
						cmd.Parameters.AddWithValue("v_projectmanager", (object)0);
						cmd.Parameters.AddWithValue("v_sid", (object)userId);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("FETCH ALL FROM \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader sdr = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)sdr).HasRows)
								{
									while (((DbDataReader)(object)sdr).Read())
									{
										obj = new main.DashboardCount();
										obj.TotalAssignProject = Convert.ToInt32(((DbDataReader)(object)sdr)["TotalProject"]);
										obj.InternalProject = Convert.ToInt32(((DbDataReader)(object)sdr)["InternalProject"]);
										obj.ExternalProject = Convert.ToInt32(((DbDataReader)(object)sdr)["ExternalProject"]);
										obj.CompletedProject = Convert.ToInt32(((DbDataReader)(object)sdr)["CompletedProject"]);
										obj.PendingProject = Convert.ToInt32(((DbDataReader)(object)sdr)["PendingProject"]);
										obj.OngoingProject = Convert.ToInt32(((DbDataReader)(object)sdr)["OngoingProject"]);
										obj.TotalMeetings = Convert.ToInt32(((DbDataReader)(object)sdr)["TotalMeetings"]);
										obj.AdminMeetings = Convert.ToInt32(((DbDataReader)(object)sdr)["AdminMeetings"]);
										obj.ProjectManagerMeetings = Convert.ToInt32(((DbDataReader)(object)sdr)["ProjectManagerMeetings"]);
									}
									((DbDataReader)(object)sdr).Close();
								}
								return obj;
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

		public List<RemoteSensingProject.Models.Admin.main.Meeting_Model> getAllSubordinatemeeting(int id, int? limit = null, int? page = null, string searchTerm = null, string statusFilter = null)
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
						cmd.Parameters.AddWithValue("@p_action", (object)"selectsubordinatemeetings");
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
	}
}