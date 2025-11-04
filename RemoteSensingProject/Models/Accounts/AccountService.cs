using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DocumentFormat.OpenXml.Math;
using Npgsql;
using static RemoteSensingProject.Models.Accounts.main;

namespace RemoteSensingProject.Models.Accounts
{
    public class AccountService:DataFactory
    {
        public List<Project_model> Project_List()
        {
            try
            {
                List<Project_model> list = new List<Project_model>();
                cmd = new NpgsqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getAllManagerProject");
                con.Open();
                NpgsqlDataReader rd = cmd.ExecuteReader();
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
                            ApproveStatus = (bool)rd["ApproveStatus"],
                            StartDate = Convert.ToDateTime(rd["startDate"]),
                            ProjectManager = rd["name"].ToString(),
                            ProjectBudget = Convert.ToDecimal(rd["budget"]),
                            ProjectDescription = rd["description"].ToString(),
                            projectDocumentUrl = rd["ProjectDocument"].ToString(),
                            ProjectType = rd["projectType"].ToString(),
                            ProjectStage = Convert.ToBoolean(rd["stage"]),
                            CompletionDatestring = Convert.ToDateTime(rd["completionDate"]).ToString("dd-MM-yyyy"),
                            ProjectStatus = Convert.ToBoolean(rd["CompleteStatus"]),
                            AssignDateString = Convert.ToDateTime(rd["assignDate"]).ToString("dd-MM-yyyy"),
                            StartDateString = Convert.ToDateTime(rd["startDate"]).ToString("dd-MM-yyyy"),
                            projectCode = rd["projectCode"] != DBNull.Value ? rd["projectCode"].ToString():"N/A"
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

        public bool UpdateExpensesResponse(HeadExpenses he)
        {
            try
            {
                List<Project_model> list = new List<Project_model>();
                cmd = new NpgsqlCommand("sp_ManageProjectSubstaces", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "updateProjectBudgetResponseFromAccounts");
                cmd.Parameters.AddWithValue("@reason",he.Reason);
                cmd.Parameters.AddWithValue("@amount", he.Amount);
                cmd.Parameters.AddWithValue("@id", he.Id);
                cmd.Parameters.AddWithValue("@projectId", he.ProjectId);
                cmd.Parameters.AddWithValue("@headId", he.HeadId);
                cmd.Parameters.AddWithValue("@appStatus", he.AppStatus);
                con.Open();

                int res = cmd.ExecuteNonQuery();

                return res>0;
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

        public List<Project_Budget> ProjectBudgetList(int Id)
        {
            try
            {
                List<Project_Budget> list = new List<Project_Budget>();
                cmd = new NpgsqlCommand("sp_ManageProjectSubstaces", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetBudgetByProjectId");
                cmd.Parameters.AddWithValue("@id", Id);
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


        public List<Reimbursement> GetReimbursements()
        {
            try
            {
                cmd = new NpgsqlCommand("sp_Reimbursement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectApprovedReinbursement");
                con.Open();
                List<Reimbursement> getlist = new List<Reimbursement>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                    {
                        while (res.Read())
                    {
                        getlist.Add(new Reimbursement
                        {
                            type = res["type"].ToString(),
                            EmpName = res["name"].ToString() + $"({res["employeeCode"].ToString()})",
                            amount = Convert.ToDecimal(res["amount"]),
                            userId = Convert.ToInt32(res["userId"]),
                            id = Convert.ToInt32(res["id"]),
                            appr_status = Convert.ToBoolean(res["Apprstatus"])
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

        public List<tourProposal> getTourList(int? limit = null,int? page = null)
        {
            try
            {
                con.Open();
                List<tourProposal> getlist = new List<tourProposal>();
                using (var tran = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand("fn_managetourproposal_cursor", con, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("v_action", "selectAlltourforAcc");
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
                                getlist.Add(new tourProposal
                                {
                                    id = Convert.ToInt32(res["id"]),
                                    projectManager = Convert.ToString(res["name"]),
                                    projectName = Convert.ToString(res["title"]),
                                    dateOfDept = Convert.ToDateTime(res["dateOfDept"]),
                                    place = Convert.ToString(res["place"]),
                                    periodFrom = Convert.ToDateTime(res["periodFrom"]),
                                    periodTo = Convert.ToDateTime(res["periodTo"]),
                                    returnDate = Convert.ToDateTime(res["returnDate"]),
                                    purpose = Convert.ToString(res["purpose"]),
                                    newRequest = Convert.ToBoolean(res["newRequest"]),
                                    adminappr = Convert.ToBoolean(res["adminappr"]),
                                    remark = res["remark"].ToString(),
                                    projectCode = res["projectCode"] != DBNull.Value ? res["projectCode"].ToString() : "N/A"
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
        public List<tourProposal> getTourOne(int? id)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_Tourproposal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selecttourOne");
                cmd.Parameters.AddWithValue("id", id);
                con.Open();
                List<tourProposal> getlist = new List<tourProposal>();
                var res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        getlist.Add(new tourProposal
                        {
                            id = Convert.ToInt32(res["id"]),
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


        public bool reinbursementRequestAmt(Reimbursement rs)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_Reimbursement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "approveReinburseAmt");
                cmd.Parameters.AddWithValue("@chequeNum", rs.chequeNumber);
                cmd.Parameters.AddWithValue("@chequeDate", rs.date);
                cmd.Parameters.AddWithValue("@sanctionAmt", rs.amount);
                cmd.Parameters.AddWithValue("@apprAmt", rs.apprAmt);
                cmd.Parameters.AddWithValue("@rejectAmt", rs.amount-rs.apprAmt);
                cmd.Parameters.AddWithValue("@id", rs.id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;

            }catch(Exception ex)
            {
                throw ex;
            }
        }

        public bool rejectReinbursementRequestAmt(int id, string reason)
        {
            try
            {
                cmd = new NpgsqlCommand("sp_Reimbursement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "rejectReinbursementAmtRequest");
                cmd.Parameters.AddWithValue("@rejectReason", reason);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
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

        public DashboardCount DashboardCount()
        {
            DashboardCount obj = null;
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand("sp_ManageDashboard", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "AccountDashboardCount");
                con.Open();
                NpgsqlDataReader sdr = cmd.ExecuteReader();

                if (sdr.Read())
                {
                    obj = new DashboardCount();
                    obj.TotalReinbursementReq = sdr["TotalReinbursementReq"].ToString();
                    obj.TotalTourProposalReq = sdr["TotalTourProposalReq"].ToString();
                    obj.totalVehicleHiringRequest = sdr["totalVehicleHiringRequest"].ToString();
                    obj.totalReinbursementPendingRequest = sdr["totalReinbursementPendingRequest"].ToString();
                    obj.totalReinbursementapprovedRequest = sdr["totalReinbursementapprovedRequest"].ToString();
                    obj.totalReinbursementRejectRequest = sdr["totalReinbursementRejectRequest"].ToString();
                    obj.totalTourProposalApprReque = sdr["totalTourProposalApprReque"].ToString();
                    obj.totalTourProposalRejectReque = sdr["totalTourProposalRejectReque"].ToString();
                    obj.totaTourProposalPendingReque = sdr["totaTourProposalPendingReque"].ToString();
                    obj.totalPendingHiringVehicle = sdr["totalPendingHiringVehicle"].ToString();
                    obj.totalApproveHiringVehicle = sdr["totalApproveHiringVehicle"].ToString();
                    obj.totalRejectHiringVehicle = sdr["totalRejectHiringVehicle"].ToString();
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
        public GraphData ExpencesListforgraph()
        {
            GraphData obj = null;
            try
            {
                cmd = new NpgsqlCommand("sp_ManageProjectSubstaces", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectExpensesforgraph");
                con.Open();
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        obj = new GraphData();

                        obj.ApprAmount = rd["appamount"] != DBNull.Value ? Convert.ToDecimal(rd["appamount"]) : 0m; // Default to 0 if null
                        obj.amount = rd["totalamount"] != DBNull.Value ? Convert.ToDecimal(rd["totalamount"]) : 0m;
                        //obj.month = rd["monthname"].ToString();
                    }
                }
                return obj;
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

        public List<GraphData> budgetdataforgraph()
        {
            try
            {
                List<GraphData> list = new List<GraphData>();
                cmd = new NpgsqlCommand("sp_ManageDashboard", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "graphdataofaccount");
                if (con.State == ConnectionState.Closed)
                    con.Open();
                NpgsqlDataReader rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    while (rd.Read())
                    {
                        list.Add(new GraphData
                        {
                            months = rd["months"].ToString(),
                            amount = Convert.ToDecimal(rd["amount"]),
                            ApprAmount = Convert.ToDecimal(rd["appramount"]),
                            pendingamount = Convert.ToDecimal(rd["pending"])
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
        public List<Reimbursement> getReimbursementrepo()
        {
            try
            {
                List<Reimbursement> list = new List<Reimbursement>();
                cmd = new NpgsqlCommand("sp_Reimbursement", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "accountrepo");
                con.Open();
                NpgsqlDataReader res = cmd.ExecuteReader();
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        list.Add(new Reimbursement
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
                            remark = res["remark"].ToString(),
                            apprstatus = Convert.ToBoolean(res["ApprStatus"]),
                            accountNewRequest = Convert.ToBoolean(res["accountNewRequest"]),
                            chequeNum = res["chequeNum"].ToString(),
                            chequeDate = res["chequeDate"] != DBNull.Value ? Convert.ToDateTime(res["chequeDate"]).ToString("dd/MM/yyyy") : "",
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
    }
}