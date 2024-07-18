using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace CPUFramework
{
    public class SQLutility
    {
        public static string ConnectionString = "";
        public static DataTable GetDataTable(string sqlstatement)
        {
            DataTable dt = new();
            SqlConnection conn = new();
            conn.ConnectionString = ConnectionString;
            conn.Open();
            //DisplayMessage("connection status", conn.State.ToString());
            var cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sqlstatement;
            var dr = cmd.ExecuteReader();
            dt.Load(dr);
            AllowColumnNull(dt);

            return dt;
        }
        public static void ExecuteSQL(string sqlstatement)
        {
            GetDataTable(sqlstatement);
        }
        
        private static void AllowColumnNull(DataTable dt)
        {
            foreach(DataColumn c in dt.Columns)
            {
                c.AllowDBNull = true;
            }
        }
        public static void DebugPrintDataTable(DataTable dt)
        {
            foreach(DataRow r in dt.Rows)
            {
                foreach(DataColumn c in dt.Columns)
                {
                    Debug.Print(c.ColumnName + "=" + r[c.ColumnName].ToString());
                }
            }
        }
    }
}
