using DocumentFormat.OpenXml.EMMA;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}