using System;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using NLog;

namespace QnapBackupper
{
    internal class SQL
    {
        public static string GetLaps(string sucursal, Logger log)
        {
            String result = "VACIO";
            String sucParameter = "rex" + sucursal + "a";

            if (sucursal.Equals("00"))
            {
                sucParameter = "rex_ts";
            }

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SISTEMAS"].ConnectionString))
                {
                    using (SqlCommand sqlComm = new SqlCommand("GetLaps", sqlConn))
                    {
                        sqlComm.CommandType = CommandType.StoredProcedure;
                        sqlComm.Parameters.AddWithValue("@suc", sucParameter);
                        sqlComm.CommandTimeout = 600;
                        DataTable dt = new DataTable();
                        using (SqlDataAdapter sqlDA = new SqlDataAdapter(sqlComm))
                        {
                            sqlDA.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                result = dt.Rows[0][0].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Falló la obtención de LAPS: {ex}.");
            }
            return result;
        }
    }
}
