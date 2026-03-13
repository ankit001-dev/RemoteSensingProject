using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.ExtendedProperties;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using static RemoteSensingProject.Models.TechnicalCell.main;

namespace RemoteSensingProject.Models.TechnicalCell
{
    public class TechnicalCellServices : DataFactory
    {
        public bool CreateDynamicReport(DynamicFormate model)
        {
            try
            {
                using (var cmd = new NpgsqlCommand("CALL sp_create_dynamic_table(@format_name,@table_name,@columns)", con))
                {
                    cmd.Parameters.AddWithValue("format_name", model.FormatName);
                    cmd.Parameters.AddWithValue("table_name", model.TableName);
                    cmd.Parameters.AddWithValue("columns", model.ColumnName.ToArray());
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool InsertDynamicReportData(DynamicInsertData data, int userId, string createdBy,int divisionid)
        {
            if (!Regex.IsMatch(data.TableName, @"^[a-zA-Z0-9_]+$"))
                throw new Exception("Invalid table name");

            var columns = new List<string>();   
            var parameters = new List<string>();

            int i = 0;

            foreach (var item in data.Data)
            {
                columns.Add($"\"{item.Key}\"");
                parameters.Add($"@p{i}");
                i++;
            }

            string columnList = string.Join(",", columns);
            string parameterList = string.Join(",", parameters);

            string query = $@"
        INSERT INTO ""{data.TableName}""
        (managerid, createdby,divisionid, {columnList})
        VALUES
        (@managerid, @createdby,@divisionid, {parameterList})
    ";

            using (var cmd = new NpgsqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("managerid", userId);
                cmd.Parameters.AddWithValue("createdby", createdBy);
                cmd.Parameters.AddWithValue("divisionid", createdBy);

                i = 0;

                foreach (var item in data.Data)
                {
                    cmd.Parameters.AddWithValue($"@p{i}", item.Value);
                    i++;
                }
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public DataTable GetDynamicData(string tableName, List<string> columns, string userrole, int userId)
        {
            DataTable dt = new DataTable();

            string columnList = string.Join(",", columns.Select(c => $"\"{c}\""));

            string query;

            if (userrole == "technicalShell")
            {
                query = $@"SELECT {columnList}
                   FROM ""{tableName}""
                   WHERE status = true
                   ORDER BY id DESC";
            }
            else
            {
                query = $@"SELECT {columnList}
                   FROM ""{tableName}""
                   WHERE status = true
                   AND managerid = @userId
                   ORDER BY id DESC";
            }

            using (var cmd = new NpgsqlCommand(query, con))
            {
                if (userrole == "projectManager")
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                }

                using (var adapter = new NpgsqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }

            return dt;
        }
    }
}