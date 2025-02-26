﻿using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace CPUFramework
{
    public class bizObject<T> : INotifyPropertyChanged where T:bizObject<T>, new()
    {
        string _typename = "";
        string _tablename = "";
        string _getsproc = "";
        string _updatesproc = "";
        string _deletesproc = "";
        string _primarykeyname = "";
        string _primarykeyparamname = "";
        DataTable _datatable = new();
        List<PropertyInfo> _properties = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public bizObject()
        {
            Type t = this.GetType();
            _typename = t.Name;
            _tablename = _typename;
            if (_tablename.ToLower().StartsWith("biz")) { _tablename = _tablename.Substring(3); }
            _getsproc = _tablename + "Get";
            _updatesproc = _tablename + "Update";
            _deletesproc = _tablename + "Delete";
            _primarykeyname = _tablename + "Id";
            _primarykeyparamname = "@" + _primarykeyname;
            _properties = t.GetProperties().ToList<PropertyInfo>();
        }
        public DataTable Load(int primarykeyvalue)
        {

            DataTable dt = new();
            SqlCommand cmd = SQLutility.GetSqlCommand(_getsproc);
            SQLutility.SetParamValue(cmd, _primarykeyparamname, primarykeyvalue);
            dt = SQLutility.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                LoadProps(dt.Rows[0]);
            }
            _datatable = dt;
            return dt;
        }
        public List<T> GetList(bool includeblank = false, int userid = 0)
        {
            List<T> lst = new();
            SqlCommand cmd = SQLutility.GetSqlCommand(_getsproc);
            if (cmd.Parameters.Contains("@All"))
            {
                SQLutility.SetParamValue(cmd, "@All", 1);
            }
            
            if (cmd.Parameters.Contains("@IncludeBlank"))
            {
                SQLutility.SetParamValue(cmd, "@IncludeBlank", includeblank);
            }
            if (cmd.Parameters.Contains("@UserId"))
            {
                SQLutility.SetParamValue(cmd, "@UserId", userid);
            }
            var dt = SQLutility.GetDataTable(cmd);
            return GetListFromDatatable(dt);
        }
       
        protected List<T> GetListFromDatatable(DataTable dt)
        {
            List<T> lst = new();
            foreach (DataRow dr in dt.Rows)
            {
                T obj = new T();
                obj.LoadProps(dr);
                lst.Add(obj);
            }
            return lst;
        }

        protected void LoadProps(DataRow dr)
        {
            foreach(DataColumn col in dr.Table.Columns)
            {
                SetProp(col.ColumnName, dr[col.ColumnName]);
            }
        }

        public void Delete(int id, int userid = 0)
        {
            this.ErrorMessage = "";
            SqlCommand cmd = SQLutility.GetSqlCommand(_deletesproc);
            SQLutility.SetParamValue(cmd, _primarykeyparamname, id);
            if (cmd.Parameters.Contains("@UserId"))
            {
                SQLutility.SetParamValue(cmd, "@UserId", userid);
            }
            SQLutility.ExecuteSQL(cmd);
            
            foreach (SqlParameter param in cmd.Parameters)
            {
                Debug.WriteLine($"Param: {param.ParameterName}, Direction: {param.Direction}");
                
                if (param.Direction == ParameterDirection.InputOutput)
                {
                    SetProp(param.ParameterName, param.Value);
                }
            }
        }

        public void Delete()
        {
            this.ErrorMessage = "";
            PropertyInfo? prop = GetProp(_primarykeyname, true, false);
            if(prop != null)
            {
                object? id = prop.GetValue(this);
                if(id != null)
                {
                  this.Delete((int)id);
                }  
            } 
        }

        public void Delete(DataTable dtpresident)
        {
            int id = (int)dtpresident.Rows[0][_primarykeyname];
            this.Delete(id);
        }

        public void Save()
        {
            this.ErrorMessage = "";
            SqlCommand cmd = SQLutility.GetSqlCommand(_updatesproc);
            foreach(SqlParameter param in cmd.Parameters)
            {
                var prop = GetProp(param.ParameterName, true, false);
                if (prop != null)
                {
                    object? val = prop.GetValue(this);
                    if(val == null)
                    {
                        val = DBNull.Value;
                    }
                   param.Value = val;
                }
            }
            SQLutility.ExecuteSQL(cmd);
            foreach(SqlParameter param in cmd.Parameters)
            {
                if(param.Direction == ParameterDirection.InputOutput)
                {
                    SetProp(param.ParameterName, param.Value);
                }
            }
        }
        public  void Save(DataTable datatable)
        {
            this.ErrorMessage = "";
            if (datatable.Rows.Count == 0)
            {
                throw new Exception($"cannot call {_tablename} save method, there are no rows in table");
            }
            DataRow r = datatable.Rows[0];
            SQLutility.SaveDataRow(r, _updatesproc);

        }

        private PropertyInfo? GetProp(string propname, bool forread, bool forwrite)
        {
            propname = propname.ToLower();
            if (propname.StartsWith("@"))
            {
                propname = propname.Substring(1);
            }
            PropertyInfo? prop = _properties.FirstOrDefault(p =>
            p.Name.ToLower() == propname
            && (forread == false || p.CanRead == true)
            && (forwrite == false || p.CanWrite == true)
            );
            return prop;
        }
        private void SetProp(string propname, object? value)
        {
            var prop = GetProp(propname, false, true);
            if (prop != null)
            {
                if(value == DBNull.Value)
                {
                    value = null;
                }
                try
                {
                    prop.SetValue(this, value);
                }
                catch(Exception ex)
                {
                    string msg = $"{_typename}.{prop.Name} is being set to{value?.ToString()} and that is the wrong data type. {ex.Message}";
                    throw new CPUDevException(msg, ex);
                }
            }
        }
        protected string GetSprocName { get => _getsproc; }
        protected void InvokePropertyChanged([CallerMemberName] string propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
        public string ErrorMessage { get; set; } = "";
    }
}
