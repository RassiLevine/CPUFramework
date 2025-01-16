using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUFramework
{
    public class bizUser:bizObject<bizUser>
    {

        public void Login()
        {
            SqlCommand cmd = SQLutility.GetSqlCommand("UserLogin");
            SQLutility.SetParamValue(cmd, "@username", UserName);
            SQLutility.SetParamValue(cmd, "@password", Password);
            this.Password = "";
            DataTable dt = SQLutility.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                this.LoadProps(dt.Rows[0]);
            }
        }
        public void Logout()
        {
            SqlCommand cmd = SQLutility.GetSqlCommand("UserLogout");
            SQLutility.SetParamValue(cmd, "@username", UserName);
            DataTable dt = SQLutility.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                this.LoadProps(dt.Rows[0]);
            }
        }

        public void LoadBySessionKey(string SessionKey)
        {
            SqlCommand cmd = SQLutility.GetSqlCommand("UserGet");
            SQLutility.SetParamValue(cmd, "@sessionkey", SessionKey);
            DataTable dt = SQLutility.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                this.LoadProps(dt.Rows[0]);
            }
        }

        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public string SessionKey { get; set; } = "";
        public string RoleName { get; set; } = "";
        public int RoleRank { get; set; } 


    }
}
