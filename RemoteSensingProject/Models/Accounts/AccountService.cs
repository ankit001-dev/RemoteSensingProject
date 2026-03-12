// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.Models.Accounts.AccountService
using Npgsql;
using NpgsqlTypes;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Accounts;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using static RemoteSensingProject.Models.Accounts.main;

namespace RemoteSensingProject.Models.Accounts
{
	public class AccountService : DataFactory
	{
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

        #region Manage Dashboard Data
        public main.DashboardCount DashboardCount()
		{
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
									obj.TotalTourCount = sdr["TotalTourCount"] != DBNull.Value ? Convert.ToInt32(sdr["TotalTourCount"]) : 0;
									obj.TotalInternalProjectCount = sdr["TotalInternalProjectCount"] != DBNull.Value ? Convert.ToInt32(sdr["TotalInternalProjectCount"]) : 0;
									obj.TotalInternalProjectFund = sdr["TotalInternalProjectFund"] != DBNull.Value ? Convert.ToInt32(sdr["TotalInternalProjectFund"]) : 0;
									obj.TotalInternalExpense = sdr["TotalInternalExpense"] != DBNull.Value ? Convert.ToInt32(sdr["TotalInternalExpense"]) : 0;
									obj.TotalInternalCompletedProject = sdr["TotalInternalCompletedProject"] != DBNull.Value ? Convert.ToInt32(sdr["TotalInternalCompletedProject"]) : 0;
									obj.TotalExternalProjectCount = sdr["TotalExternalProjectCount"] != DBNull.Value ? Convert.ToInt32(sdr["TotalExternalProjectCount"]) : 0;
									obj.TotalExternalProjectFund = sdr["TotalExternalProjectFund"] != DBNull.Value ? Convert.ToInt32(sdr["TotalExternalProjectFund"]) : 0;
									obj.TotalExternalExpense = sdr["TotalExternalExpense"] != DBNull.Value ? Convert.ToInt32(sdr["TotalExternalExpense"]) : 0;
									obj.TotalExternalCompletedProject = sdr["TotalExternalCompletedProject"] != DBNull.Value ? Convert.ToInt32(sdr["TotalExternalCompletedProject"]) : 0;
									obj.AdhisthanBudgetProvision = sdr["AdhisthanBudgetProvision"] != DBNull.Value ? Convert.ToInt32(sdr["AdhisthanBudgetProvision"]) : 0;
									obj.AdhisthanExpenditure = sdr["AdhisthanExpenditure"] != DBNull.Value ? Convert.ToDecimal(sdr["AdhisthanExpenditure"]) : 0;
									obj.AdhisthanExpenditureInPerc = sdr["AdhisthanExpenditureInPerc"] != DBNull.Value ? Convert.ToDecimal(sdr["AdhisthanExpenditureInPerc"]) : 0;
                                    

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

		public GraphGrouped budgetdataforgraph()
		{
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
											ProjectCode = ((DbDataReader)(object)rd)["projectCode"].ToString(),
											ProjectName = rd["title"].ToString(),
											TotalFund = Convert.ToDecimal(((DbDataReader)(object)rd)["totalfund"]),
											TotalExpense = Convert.ToDecimal(((DbDataReader)(object)rd)["totalexpense"]),
											TotalRemaining = Convert.ToDecimal(((DbDataReader)(object)rd)["totalremaining"]),
                                            ProjectType = rd["projectType"].ToString(),
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
                        // RESTRUCTURE HERE
						GraphGrouped result = new GraphGrouped
						{
							Internal = list
								.Where(x => x.ProjectType == "Internal")
								.ToList(),

							External = list
								.Where(x => x.ProjectType == "External")
								.ToList()
						};
                        return result;
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

        #region Manage Adhisthan
		//Add Adhisthan
        public bool InsertAdhisthan(AdhisthanModel ad)
		{
            ((DbConnection)(object)con).Open();
            using (var transaction = con.BeginTransaction())
            {
                try
                {
                    using (var cmd = new NpgsqlCommand(
                        "CALL sp_manageadhisthan(p_action=>@p_action,p_id=>@p_id, p_headname=>@p_headname, p_budgetprovision=>@p_budgetprovision,p_committed=>@p_committed)",
                        con, transaction))
                    {
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Add("@p_action", NpgsqlDbType.Varchar).Value = ad.Id>0? "updateadhisthan" : "insertadhisthan";
                        cmd.Parameters.Add("@p_id", NpgsqlDbType.Integer).Value = ad.Id;
                        cmd.Parameters.Add("@p_headname", NpgsqlDbType.Varchar).Value = ad.HeadName;
                        //cmd.Parameters.Add("@p_w_date", NpgsqlDbType.Date).Value = DateTime.Now.Date;
                        cmd.Parameters.Add("@p_budgetprovision", NpgsqlDbType.Numeric).Value = ad.BudgetProvision;
                        cmd.Parameters.Add("@p_committed", NpgsqlDbType.Numeric).Value = ad.Committed;

                        cmd.ExecuteNonQuery();
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

        //Add Expenditure
        public bool InsertExpenditure(AdhisthanModel ad)
        {
            ((DbConnection)(object)con).Open();
            using (var transaction = con.BeginTransaction())
            {
                try
                {
                    using (var cmd = new NpgsqlCommand(
                        "CALL sp_manageadhisthan(p_action=>@p_action, p_id=>@p_id, p_budgetprovision=>@p_budgetprovision)",
                        con, transaction))
                    {
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Add("@p_action", NpgsqlDbType.Varchar).Value = "addExpenditure";
                        cmd.Parameters.Add("@p_id", NpgsqlDbType.Integer).Value = ad.Id;
                        cmd.Parameters.Add("@p_budgetprovision", NpgsqlDbType.Numeric).Value = ad.ExpenditureAmount;

                        cmd.ExecuteNonQuery();
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

        //Update Expense Committed In Adhisthan
        public bool UpdateExpenseCommitted(UpdateCommitted ad)
        {
            ((DbConnection)(object)con).Open();
            using (var transaction = con.BeginTransaction())
            {
                try
                {
                    using (var cmd = new NpgsqlCommand(
                        "CALL sp_manageadhisthan(p_action=>@p_action, p_id=>@p_id,p_headname=>@p_headname, p_committed=>@p_committed)",
                        con, transaction))
                    {
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Add("@p_action", NpgsqlDbType.Varchar).Value = "updateexpensecommitted";
                        cmd.Parameters.Add("@p_id", NpgsqlDbType.Integer).Value = ad.AdhisthanId;
                        cmd.Parameters.Add("@p_headname", NpgsqlDbType.Varchar).Value = ad.Title;
                        cmd.Parameters.Add("@p_committed", NpgsqlDbType.Numeric).Value = ad.ExpenseCommitted;

                        cmd.ExecuteNonQuery();
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
        //Update Expense Committed In Project Heads
        public bool UpdateExpenseCommittedInHeads(UpdateCommitted ad)
        {
            ((DbConnection)(object)con).Open();
            using (var transaction = con.BeginTransaction())
            {
                try
                {
                    using (var cmd = new NpgsqlCommand(
                        "CALL sp_manageadhisthan(p_action=>@p_action, p_id=>@p_id,p_projectid=>@p_projectid,p_headname=>@p_headname, p_committed=>@p_committed)",
                        con, transaction))
                    {
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Add("@p_action", NpgsqlDbType.Varchar).Value = "updateheadcommitt";
                        cmd.Parameters.Add("@p_id", NpgsqlDbType.Integer).Value = ad.HeadId;
                        cmd.Parameters.Add("@p_projectid", NpgsqlDbType.Integer).Value = ad.ProjectId;
                        cmd.Parameters.Add("@p_headname", NpgsqlDbType.Varchar).Value = ad.Title;
                        cmd.Parameters.Add("@p_committed", NpgsqlDbType.Numeric).Value = ad.ExpenseCommitted;

                        cmd.ExecuteNonQuery();
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

        public List<AdhisthanModel> GetAdhisthanList(int? id = null,int? limit = null,int? page = null,string searchTerm = null)
		{
            try
            {
                ((DbConnection)(object)con).Open();
                List<AdhisthanModel> data = new List<AdhisthanModel>();
                NpgsqlTransaction tran = con.BeginTransaction();
                try
                {
                    NpgsqlCommand cmd = new NpgsqlCommand("fn_manageadhisthan", con, tran);
                    try
                    {
                        ((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("v_action", (object)"selectadhisthan");
                        cmd.Parameters.AddWithValue("@v_id",id.HasValue? (object)id:DBNull.Value);
                        cmd.Parameters.AddWithValue("@v_limit",limit.HasValue? (object)limit:DBNull.Value);
                        cmd.Parameters.AddWithValue("@v_page", page.HasValue? (object)page:DBNull.Value);
                        cmd.Parameters.AddWithValue("@v_searchterm", !string.IsNullOrEmpty(searchTerm) ?(object)searchTerm:DBNull.Value);
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
										data.Add(new AdhisthanModel {
											Id = Convert.ToInt32(rd["id"]),
											HeadName = rd["headname"].ToString(),
											BudgetProvision = rd["budgetprovision"] != null ? Convert.ToDecimal(rd["budgetprovision"]) : 0,
											Committed = rd["committed"] != null ? Convert.ToDecimal(rd["committed"]) : 0,
											ExpenditureAmount = rd["expenditure"] != null ? Convert.ToDecimal(rd["expenditure"]) : 0,
											ExpenditurePercentage = Convert.ToDecimal(rd["expense_percentage"] ?? 0)
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
        public List<AdhisthanModel> GetInternalProjectExpenses(int? id = null,int? limit = null,int? page = null,string searchTerm = null)
		{
            try
            {
                ((DbConnection)(object)con).Open();
                List<AdhisthanModel> data = new List<AdhisthanModel>();
                NpgsqlTransaction tran = con.BeginTransaction();
                try
                {
                    NpgsqlCommand cmd = new NpgsqlCommand("fn_manageadhisthan", con, tran);
                    try
                    {
                        ((DbCommand)(object)cmd).CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("v_action", (object)"SelectInternalHeadExpensesReport");
                        cmd.Parameters.AddWithValue("@v_id",id.HasValue? (object)id:DBNull.Value);
                        cmd.Parameters.AddWithValue("@v_limit",limit.HasValue? (object)limit:DBNull.Value);
                        cmd.Parameters.AddWithValue("@v_page", page.HasValue? (object)page:DBNull.Value);
                        cmd.Parameters.AddWithValue("@v_searchterm", !string.IsNullOrEmpty(searchTerm) ?(object)searchTerm:DBNull.Value);
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
										data.Add(new AdhisthanModel {
											Id = Convert.ToInt32(rd["id"]),
											HeadName = rd["title"].ToString(),
											BudgetProvision = rd["budget"] != null ? Convert.ToDecimal(rd["budget"]) : 0,
											ExpenditureAmount = rd["expenditure"] != null ? Convert.ToDecimal(rd["expenditure"]) : 0,
											ExpenditurePercentage = Convert.ToDecimal(rd["expense_percentage"] ?? 0),
                                            SchemeName = rd["projectscheme"].ToString(),
                                            SchemeId = rd["projectschemeid"]!=DBNull.Value?Convert.ToInt32(rd["projectschemeid"]):0 
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
        #endregion
    }
}
