using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUFramework
{
    public class bizObject
    {
        string _tablename = "";
        string _getsproc = "";
        string _updatesproc = "";
        string _deletesproc = "";
        string _primarykeyname = "";
        string _primarykeyparamname = "";
        DataTable _datatable = new();
        public bizObject(string tablename)
        {
            _tablename = tablename;
            _getsproc = tablename + "Get";
            _updatesproc = tablename + "Update";
            _deletesproc = tablename + "Delete";
            _primarykeyname = tablename + "Id";
            _primarykeyparamname = "@" + _primarykeyname;
        }
        public DataTable LoadPres(int primarykeyvalue)
        {

            DataTable dt = new();
            SqlCommand cmd = SQLutility.GetSqlCommand(_getsproc);
            SQLutility.SetParamValue(cmd, _primarykeyparamname, primarykeyvalue);
            dt = SQLutility.GetDataTable(cmd);
            _datatable = dt;
            return dt;
        }
        public void Delete(DataTable dtpresident)
        {
            int id = (int)dtpresident.Rows[0][_primarykeyname];
            SqlCommand cmd = SQLutility.GetSqlCommand(_deletesproc);
            SQLutility.SetParamValue(cmd, _primarykeyparamname, id);
            SQLutility.ExecuteSQL(cmd);
        }

        public  void Save(DataTable datatable)
        {
            if (datatable.Rows.Count == 0)
            {
                throw new Exception($"cannot call {_tablename} save method, there are no rows in table");
            }
            DataRow r = datatable.Rows[0];
            SQLutility.SaveDataRow(r, _updatesproc);

        }
    }
}
