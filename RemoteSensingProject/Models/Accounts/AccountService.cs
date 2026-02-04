// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.Accounts.AccountService
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using Npgsql;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Accounts;

namespace RemoteSensingProject.Models.Accounts
{
	public class AccountService : DataFactory
	{
		public List<main.Project_model> Project_List()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			try
			{
				List<main.Project_model> list = new List<main.Project_model>();
				cmd = new NpgsqlCommand("sp_adminAddproject", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"getAllManagerProject");
				((DbConnection)(object)con).Open();
				NpgsqlDataReader rd = cmd.ExecuteReader();
				if (((DbDataReader)(object)rd).HasRows)
				{
					while (((DbDataReader)(object)rd).Read())
					{
						list.Add(new main.Project_model
						{
							Id = Convert.ToInt32(((DbDataReader)(object)rd)["id"]),
							ProjectTitle = ((DbDataReader)(object)rd)["title"].ToString(),
							AssignDate = Convert.ToDateTime(((DbDataReader)(object)rd)["assignDate"]),
							CompletionDate = Convert.ToDateTime(((DbDataReader)(object)rd)["completionDate"]),
							ApproveStatus = (bool)((DbDataReader)(object)rd)["ApproveStatus"],
							StartDate = Convert.ToDateTime(((DbDataReader)(object)rd)["startDate"]),
							ProjectManager = ((DbDataReader)(object)rd)["name"].ToString(),
							ProjectBudget = Convert.ToDecimal(((DbDataReader)(object)rd)["budget"]),
							ProjectDescription = ((DbDataReader)(object)rd)["description"].ToString(),
							projectDocumentUrl = ((DbDataReader)(object)rd)["ProjectDocument"].ToString(),
							ProjectType = ((DbDataReader)(object)rd)["projectType"].ToString(),
							ProjectStage = Convert.ToBoolean(((DbDataReader)(object)rd)["stage"]),
							CompletionDatestring = Convert.ToDateTime(((DbDataReader)(object)rd)["completionDate"]).ToString("dd-MM-yyyy"),
							ProjectStatus = Convert.ToBoolean(((DbDataReader)(object)rd)["CompleteStatus"]),
							AssignDateString = Convert.ToDateTime(((DbDataReader)(object)rd)["assignDate"]).ToString("dd-MM-yyyy"),
							StartDateString = Convert.ToDateTime(((DbDataReader)(object)rd)["startDate"]).ToString("dd-MM-yyyy"),
							projectCode = ((((DbDataReader)(object)rd)["projectCode"] != DBNull.Value) ? ((DbDataReader)(object)rd)["projectCode"].ToString() : "N/A")
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

		public bool UpdateExpensesResponse(main.HeadExpenses he)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			try
			{
				List<main.Project_model> list = new List<main.Project_model>();
				cmd = new NpgsqlCommand("CALL sp_ManageProjectSubstaces(v_action=>@action, v_reason=>@reason, v_amount=>@amount, v_id=>@id, v_projectId=>@projectId, V_appStatus=>@appStatus )", con);
				cmd.Parameters.AddWithValue("@action", (object)"updateProjectBudgetResponseFromAccounts");
				if (string.IsNullOrWhiteSpace(he.Reason))
				{
					cmd.Parameters.AddWithValue("@reason", (object)DBNull.Value);
				}
				else
				{
					cmd.Parameters.AddWithValue("@reason", (object)he.Reason);
				}
				cmd.Parameters.AddWithValue("@amount", (object)Convert.ToDecimal(he.Amount));
				cmd.Parameters.AddWithValue("@id", (object)he.Id);
				cmd.Parameters.AddWithValue("@projectId", (object)he.ProjectId);
				cmd.Parameters.AddWithValue("@appStatus", (object)he.AppStatus);
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

		public List<main.Project_Budget> ProjectBudgetList(int Id)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			try
			{
				List<main.Project_Budget> list = new List<main.Project_Budget>();
				cmd = new NpgsqlCommand("sp_ManageProjectSubstaces", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"GetBudgetByProjectId");
				cmd.Parameters.AddWithValue("@id", (object)Id);
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

		public List<main.Reimbursement> GetReimbursements()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_Reimbursement", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"selectApprovedReinbursement");
				((DbConnection)(object)con).Open();
				List<main.Reimbursement> getlist = new List<main.Reimbursement>();
				NpgsqlDataReader res = cmd.ExecuteReader();
				if (((DbDataReader)(object)res).HasRows)
				{
					while (((DbDataReader)(object)res).Read())
					{
						getlist.Add(new main.Reimbursement
						{
							type = ((DbDataReader)(object)res)["type"].ToString(),
							EmpName = ((DbDataReader)(object)res)["name"].ToString() + "(" + ((DbDataReader)(object)res)["employeeCode"].ToString() + ")",
							amount = Convert.ToDecimal(((DbDataReader)(object)res)["amount"]),
							userId = Convert.ToInt32(((DbDataReader)(object)res)["userId"]),
							id = Convert.ToInt32(((DbDataReader)(object)res)["id"]),
							appr_status = Convert.ToBoolean(((DbDataReader)(object)res)["Apprstatus"])
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

		public List<main.tourProposal> getTourList(int? limit = null, int? page = null, int? managerFilter = null, int? projectFilter = null, string statusFilter = null)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Expected O, but got Unknown
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Expected O, but got Unknown
			try
			{
				((DbConnection)(object)con).Open();
				List<main.tourProposal> getlist = new List<main.tourProposal>();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_managetourproposal_cursor", con, tran);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"selectAlltourforAcc");
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
										getlist.Add(new main.tourProposal
										{
											id = Convert.ToInt32(((DbDataReader)(object)res)["id"]),
											projectId = Convert.ToInt32(((DbDataReader)(object)res)["projectId"]),
											projectManager = Convert.ToString(((DbDataReader)(object)res)["name"]),
											projectName = Convert.ToString(((DbDataReader)(object)res)["title"]),
											dateOfDept = Convert.ToDateTime(((DbDataReader)(object)res)["dateOfDept"]),
											place = Convert.ToString(((DbDataReader)(object)res)["place"]),
											periodFrom = Convert.ToDateTime(((DbDataReader)(object)res)["periodFrom"]),
											periodTo = Convert.ToDateTime(((DbDataReader)(object)res)["periodTo"]),
											returnDate = Convert.ToDateTime(((DbDataReader)(object)res)["returnDate"]),
											purpose = Convert.ToString(((DbDataReader)(object)res)["purpose"]),
											newRequest = Convert.ToBoolean(((DbDataReader)(object)res)["newRequest"]),
											adminappr = Convert.ToBoolean(((DbDataReader)(object)res)["adminappr"]),
											remark = ((DbDataReader)(object)res)["remark"].ToString(),
											projectCode = ((((DbDataReader)(object)res)["projectCode"] != DBNull.Value) ? ((DbDataReader)(object)res)["projectCode"].ToString() : "N/A"),
											statusLabel = ((Convert.ToBoolean(((DbDataReader)(object)res)["newRequest"]) && !Convert.ToBoolean(((DbDataReader)(object)res)["adminappr"])) ? "Pending" : ((!Convert.ToBoolean(((DbDataReader)(object)res)["newRequest"]) && Convert.ToBoolean(((DbDataReader)(object)res)["adminappr"])) ? "Approved" : "Rejected"))
										});
										if (firstRow)
										{
											getlist[0].Pagination = new ApiCommon.PaginationInfo
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

		public List<main.tourProposal> getTourOne(int? id)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("sp_Tourproposal", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"selecttourOne");
				cmd.Parameters.AddWithValue("id", (object)id);
				((DbConnection)(object)con).Open();
				List<main.tourProposal> getlist = new List<main.tourProposal>();
				NpgsqlDataReader res = cmd.ExecuteReader();
				if (((DbDataReader)(object)res).HasRows)
				{
					while (((DbDataReader)(object)res).Read())
					{
						getlist.Add(new main.tourProposal
						{
							id = Convert.ToInt32(((DbDataReader)(object)res)["id"]),
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

		public bool reinbursementRequestAmt(main.Reimbursement rs)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			try
			{
				NpgsqlCommand cmd = new NpgsqlCommand("call sp_Reimbursement(p_action=>@p_action,p_chequenum=>@p_chequenum,p_chequedate=>@p_chequedate,p_sanctionamt=>@p_sanctionamt,p_appramt=>@p_appramt,p_rejectamt=>@p_rejectamt,p_id=>@p_id)", con);
				cmd.Parameters.AddWithValue("@p_action", (object)"approveReinburseAmt");
				cmd.Parameters.AddWithValue("@p_chequenum", (object)rs.chequeNumber);
				cmd.Parameters.AddWithValue("@p_chequedate", (object)Convert.ToDateTime(rs.date));
				cmd.Parameters.AddWithValue("@p_sanctionamt", (object)rs.amount);
				cmd.Parameters.AddWithValue("@p_appramt", (object)rs.apprAmt);
				cmd.Parameters.AddWithValue("@p_rejectamt", (object)(rs.amount - rs.apprAmt));
				cmd.Parameters.AddWithValue("@p_id", (object)rs.id);
				((DbConnection)(object)con).Open();
				((DbCommand)(object)cmd).ExecuteNonQuery();
				return true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public bool rejectReinbursementRequestAmt(int id, string reason)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			try
			{
				cmd = new NpgsqlCommand("CALL sp_Reimbursement(p_rejectreason => @rejectReason, p_action => @action, p_id => @id)", con);
				cmd.Parameters.AddWithValue("@action", (object)"rejectReinbursementAmtRequest");
				cmd.Parameters.AddWithValue("@rejectReason", (object)reason);
				cmd.Parameters.AddWithValue("@id", (object)id);
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

		public main.DashboardCount DashboardCount()
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
						cmd.Parameters.AddWithValue("v_action", (object)"AccountDashboardCount");
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
									obj = new main.DashboardCount();
									obj.TotalReinbursementReq = ((DbDataReader)(object)sdr)["TotalReinbursementReq"].ToString();
									obj.TotalTourProposalReq = ((DbDataReader)(object)sdr)["TotalTourProposalReq"].ToString();
									obj.totalVehicleHiringRequest = ((DbDataReader)(object)sdr)["totalVehicleHiringRequest"].ToString();
									obj.totalReinbursementPendingRequest = ((DbDataReader)(object)sdr)["totalReinbursementPendingRequest"].ToString();
									obj.totalReinbursementapprovedRequest = ((DbDataReader)(object)sdr)["totalReinbursementapprovedRequest"].ToString();
									obj.totalReinbursementRejectRequest = ((DbDataReader)(object)sdr)["totalReinbursementRejectRequest"].ToString();
									obj.totalTourProposalApprReque = ((DbDataReader)(object)sdr)["totalTourProposalApprReque"].ToString();
									obj.totalTourProposalRejectReque = ((DbDataReader)(object)sdr)["totalTourProposalRejectReque"].ToString();
									obj.totaTourProposalPendingReque = ((DbDataReader)(object)sdr)["totaTourProposalPendingReque"].ToString();
									obj.totalPendingHiringVehicle = ((DbDataReader)(object)sdr)["totalPendingHiringVehicle"].ToString();
									obj.totalApproveHiringVehicle = ((DbDataReader)(object)sdr)["totalApproveHiringVehicle"].ToString();
									obj.totalRejectHiringVehicle = ((DbDataReader)(object)sdr)["totalRejectHiringVehicle"].ToString();
								}
								((DbDataReader)(object)sdr).Close();
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

		public main.GraphData ExpencesListforgraph()
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Expected O, but got Unknown
			main.GraphData obj = null;
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
						cmd.Parameters.AddWithValue("v_action", (object)"selectExpensesforgraph");
						cmd.Parameters.AddWithValue("v_projectmanager", (object)0);
						cmd.Parameters.AddWithValue("v_sid", (object)0);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("FETCH ALL FROM \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader rd = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)rd).HasRows)
								{
									while (((DbDataReader)(object)rd).Read())
									{
										obj = new main.GraphData();
										obj.ApprAmount = ((((DbDataReader)(object)rd)["appamount"] != DBNull.Value) ? Convert.ToDecimal(((DbDataReader)(object)rd)["appamount"]) : 0m);
										obj.amount = ((((DbDataReader)(object)rd)["totalamount"] != DBNull.Value) ? Convert.ToDecimal(((DbDataReader)(object)rd)["totalamount"]) : 0m);
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
						return obj;
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

		public List<main.GraphData> budgetdataforgraph()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Expected O, but got Unknown
			try
			{
				List<main.GraphData> list = new List<main.GraphData>();
				((DbConnection)(object)con).Open();
				NpgsqlTransaction tran = con.BeginTransaction();
				try
				{
					NpgsqlCommand cmd = new NpgsqlCommand("fn_managedashboard_cursor", con);
					try
					{
						((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("v_action", (object)"graphdataofaccount");
						cmd.Parameters.AddWithValue("v_projectmanager", (object)0);
						cmd.Parameters.AddWithValue("v_sid", (object)0);
						string cursorName = (string)((DbCommand)(object)cmd).ExecuteScalar();
						NpgsqlCommand fetchCmd = new NpgsqlCommand("FETCH ALL FROM \"" + cursorName + "\";", con, tran);
						try
						{
							NpgsqlDataReader rd = fetchCmd.ExecuteReader();
							try
							{
								if (((DbDataReader)(object)rd).HasRows)
								{
									while (((DbDataReader)(object)rd).Read())
									{
										list.Add(new main.GraphData
										{
											months = ((DbDataReader)(object)rd)["months"].ToString(),
											amount = Convert.ToDecimal(((DbDataReader)(object)rd)["amount"]),
											ApprAmount = Convert.ToDecimal(((DbDataReader)(object)rd)["appramount"]),
											pendingamount = Convert.ToDecimal(((DbDataReader)(object)rd)["pending"])
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
						return list;
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

		public List<main.Reimbursement> getReimbursementrepo()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			try
			{
				List<main.Reimbursement> list = new List<main.Reimbursement>();
				cmd = new NpgsqlCommand("sp_Reimbursement", con);
				((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@action", (object)"accountrepo");
				((DbConnection)(object)con).Open();
				NpgsqlDataReader res = cmd.ExecuteReader();
				if (((DbDataReader)(object)res).HasRows)
				{
					while (((DbDataReader)(object)res).Read())
					{
						list.Add(new main.Reimbursement
						{
							type = ((DbDataReader)(object)res)["type"].ToString(),
							EmpName = ((DbDataReader)(object)res)["name"].ToString() + "(" + ((DbDataReader)(object)res)["employeeCode"].ToString() + ")",
							amount = Convert.ToDecimal(((DbDataReader)(object)res)["amount"]),
							approveAmount = Convert.ToDecimal((((DbDataReader)(object)res)["apprAmt"] != DBNull.Value) ? ((DbDataReader)(object)res)["apprAmt"] : ((object)0)),
							userId = Convert.ToInt32(((DbDataReader)(object)res)["userId"]),
							id = Convert.ToInt32(((DbDataReader)(object)res)["id"]),
							appr_status = Convert.ToBoolean(((DbDataReader)(object)res)["Apprstatus"]),
							newRequest = Convert.ToBoolean(((DbDataReader)(object)res)["newStatus"]),
							status = Convert.ToBoolean((((DbDataReader)(object)res)["apprAmountStatus"] != DBNull.Value) ? ((DbDataReader)(object)res)["apprAmountStatus"] : ((object)false)),
							remark = ((DbDataReader)(object)res)["remark"].ToString(),
							apprstatus = Convert.ToBoolean(((DbDataReader)(object)res)["ApprStatus"]),
							accountNewRequest = Convert.ToBoolean(((DbDataReader)(object)res)["accountNewRequest"]),
							chequeNum = ((DbDataReader)(object)res)["chequeNum"].ToString(),
							chequeDate = ((((DbDataReader)(object)res)["chequeDate"] != DBNull.Value) ? Convert.ToDateTime(((DbDataReader)(object)res)["chequeDate"]).ToString("dd/MM/yyyy") : "")
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

        #region Manage Hiring Vehicle

        #endregion
    }
}
