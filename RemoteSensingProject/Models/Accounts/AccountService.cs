using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using static RemoteSensingProject.Models.Accounts.main;
using System.Net;

namespace RemoteSensingProject.Models.Accounts
{
    public class AccountService:DataFactory
    {
        public List<Project_model> Project_List()
        {
            try
            {
                List<Project_model> list = new List<Project_model>();
                cmd = new SqlCommand("sp_adminAddproject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "getAllManagerProject");
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

        public bool UpdateExpensesResponse(HeadExpenses he)
        {
            try
            {
                List<Project_model> list = new List<Project_model>();
                cmd = new SqlCommand("sp_ManageProjectSubstaces", con);
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
                cmd = new SqlCommand("sp_ManageProjectSubstaces", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetBudgetByProjectId");
                cmd.Parameters.AddWithValue("@id", Id);
                if (con.State == ConnectionState.Closed)
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
                cmd = new SqlCommand("sp_Reimbursement", con);
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

        public List<tourProposal> getTourList()
        {
            try
            {
                cmd = new SqlCommand("sp_Tourproposal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "selectAlltourforAcc");
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
                            adminappr = Convert.ToBoolean(res["adminappr"]),
                            remark = res["remark"].ToString()
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
        public List<tourProposal> getTourOne(int? id)
        {
            try
            {
                cmd = new SqlCommand("sp_Tourproposal", con);
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
                cmd = new SqlCommand("sp_Reimbursement", con);
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
                cmd = new SqlCommand("sp_Reimbursement", con);
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
    }
}