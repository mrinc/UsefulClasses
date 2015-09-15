using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace Useful_Classes
{
    public delegate void ErrorEventHandler(Exception e);

    public class DatabaseConnection : IDisposable
    {
        private string host = "";
        private string username = "";
        private string password = "";
        private string database = "";
        private DatabaseType dbType = DatabaseType.NONE;

        private string MSSQLServerStr = "Server=%ADDRESS%; Database=%DATABASE%; User Id=%USERNAME%; Password=%PASSWORD%; MultipleActiveResultSets=True";
        private string MySQLServerStr = "Server=%ADDRESS%; Database=%DATABASE%; User Id=%USERNAME%; Password=%PASSWORD%;";
        private string MSDB2003ServerStr = "Provider=Microsoft.JET.OLEDB.4.0;Data Source=%DATABASE%;User Id=admin;";//Jet OLEDB:Database Password=%PASSWORD%;
        private string MSDB2007ServerStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=%DATABASE%;";//Persist Security Info=False;

        private SqlConnection MSSQLConn;
        private OleDbConnection MSDBSQLConn;
        private MySqlConnection MySQLConn;

        public event ErrorEventHandler errorCaught;

        protected virtual void OnError(Exception e)
        {
            if (errorCaught != null)
                errorCaught(e);
        }

        // The class constructor.
        public DatabaseConnection()
        {
        }

        // Track whether Dispose has been called.
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (isConnected)
                    {
                        closeConnection();
                    }
                }
            }
            disposed = true;
        }

        ~DatabaseConnection()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        public bool isConnected
        {
            get
            {
                /*if (!Licencing.validate())
                    return false;*/

                if (dbType == DatabaseType.MSSQL)
                {
                    if (MSSQLConn != null)
                        return (MSSQLConn.State == ConnectionState.Open) ? true : false;
                    else
                        return false;
                }
                else if (dbType == DatabaseType.MYSQL)
                {
                    if (MySQLConn != null)
                        return (MySQLConn.State == ConnectionState.Open) ? true : false;
                    else
                        return false;
                }
                else if (dbType == DatabaseType.MSACCESS2003 | dbType == DatabaseType.MSACCESS2007)
                {
                    if (MSDBSQLConn != null)
                        return (MSDBSQLConn.State == ConnectionState.Open) ? true : false;
                    else
                        return false;
                }
                else
                    return false;
            }
        }

        private bool isConnectedDo
        {
            get
            {
                /*if (!Licencing.validate())
                    return false;*/

                if (!isConnected)
                {
                    openConnection();
                    return false;
                }
                else
                    return true;
            }
        }

        public void closeConnection()
        {
            /*if (!Licencing.validate())
                return;*/

            try
            {
                if (dbType == DatabaseType.MSSQL)
                    MSSQLConn.Close();
                else if (dbType == DatabaseType.MYSQL)
                    MySQLConn.Close();
                else if (dbType == DatabaseType.MSACCESS2003 | dbType == DatabaseType.MSACCESS2007)
                    MSDBSQLConn.Close();
            }
            catch (Exception ee)
            {
                OnError(ee);
            }
        }

        public bool openConnection()
        {
            /*if (!Licencing.validate())
                return false;*/

            try
            {
                createConnection(true);
                return true;
            }
            catch (Exception ee)
            {
                OnError(ee);
            }
            return false;
        }

        private String createString()
        {
            /*if (!Licencing.validate())
                return null;*/

            if (dbType == DatabaseType.MSSQL)
                return MSSQLServerStr.Replace("%ADDRESS%", host).Replace("%DATABASE%", database)
                                .Replace("%USERNAME%", username).Replace("%PASSWORD%", password);
            else if (dbType == DatabaseType.MYSQL)
                return MySQLServerStr.Replace("%ADDRESS%", host).Replace("%DATABASE%", database)
                                .Replace("%USERNAME%", username).Replace("%PASSWORD%", password);
            else if (dbType == DatabaseType.MSACCESS2003)
            {
                if (password.Length == 0)
                    return (MSDB2003ServerStr.Replace("%DATABASE%", database) + "Persist Security Info=False;");
                else
                    return (MSDB2003ServerStr.Replace("%DATABASE%", database) + "Jet OLEDB:Database Password=%PASSWORD%;").Replace("%PASSWORD%", password);
            }
            else if (dbType == DatabaseType.MSACCESS2007)
            {
                if (password.Length == 0)
                    return (MSDB2007ServerStr.Replace("%DATABASE%", database) + "Persist Security Info=False;");
                else
                    return (MSDB2007ServerStr.Replace("%DATABASE%", database) + "Jet OLEDB:Database Password=%PASSWORD%;").Replace("%PASSWORD%", password);
            }
            else
                return "";
        }

        public void createConnection(string HOST, string USERNAME, string PASSWORD, string DATABASE, DatabaseType TYPE)
        {
            /*if (!Licencing.validate())
                return;*/

            host = HOST;
            username = USERNAME;
            password = PASSWORD;
            database = DATABASE;
            dbType = TYPE;

            createConnection(true);
        }

        public void createConnection(string HOST, string USERNAME, string PASSWORD, string DATABASE, DatabaseType TYPE, bool open)
        {
            /*if (!Licencing.validate())
                return;*/

            host = HOST;
            username = USERNAME;
            password = PASSWORD;
            database = DATABASE;
            dbType = TYPE;

            createConnection(open);
        }

        private void createConnection(bool open)
        {
            /*if (!Licencing.validate())
                return;*/

            try
            {
                String connStr = createString();
                if (!connStr.Equals(""))
                {
                    if (dbType == DatabaseType.MSSQL)
                    {
                        MSSQLConn = new SqlConnection(connStr);
                        if (open)
                            MSSQLConn.Open();
                    }
                    else if (dbType == DatabaseType.MYSQL)
                    {
                        MySQLConn = new MySqlConnection(connStr);
                        if (open) 
                            MySQLConn.Open();
                    }
                    else if (dbType == DatabaseType.MSACCESS2003 | dbType == DatabaseType.MSACCESS2007)
                    {
                        MSDBSQLConn = new OleDbConnection(connStr);
                        if (open) 
                            MSDBSQLConn.Open();
                    }
                }
            }
            catch (Exception ee)
            {
                OnError(ee);
            }
        }

        public enum SqlTypes
        {
            SELECT,
            UPDATE,
            INSERT,
            INSERT_CUSTOM,
            INSERT_RETURN_ID,
            SELECT_DATATABLE,
            STORED_PROCEDURE,
            STORED_PROCEDURE_DATATABLE,
            STORE_PROCEDURE_VALUE,
            AUTO
        };

        public enum SqlResult
        {
            SUCCESS,
            FAIL
        };

        public enum SqlHasReturn
        {
            YES,
            NO
        };

        public DataTable DLookupT(string table, string fieldName)
        {
            return DLookupT(table, new string[] { fieldName }, "1=1", new string[0], false);
        }

        public DataTable DLookupT(string table, string fieldName, string clauses)
        {
            return DLookupT(table, new string[] { fieldName }, clauses, new string[0], false);
        }

        public DataTable DLookupT(string table, string fieldName, string clauses, string orderByFieldName, bool orderByAsc)
        {
            return DLookupT(table, new string[] { fieldName }, clauses, new string[] { orderByFieldName }, false);
        }

        public DataTable DLookupT(string table, string[] fieldNames)
        {
            return DLookupT(table, fieldNames, "1=1", new string[0], false);
        }

        public DataTable DLookupT(string table, string[] fieldNames, string clauses)
        {
            return DLookupT(table, fieldNames, clauses, new string[0], false);
        }

        public DataTable DLookupT(string table, string[] fieldNames, string clauses, string orderByFieldName, bool orderByAsc)
        {
            return DLookupT(table, fieldNames, clauses, new string[] { orderByFieldName }, false);
        }

        public DataTable DLookupT(string table, string[] fieldNames, string clauses, string[] orderByFieldNames, bool orderByAsc)
        {
            DataTable data = null;
            bool connectedP = isConnectedDo;
            try
            {
                string fieldName = fieldNames[0];

                for (int i = 1; i < fieldNames.Length; i++)
                    fieldName += ", " + fieldNames[i];

                string query = "SELECT " + fieldName + " FROM " + table + " WHERE (" + clauses + ")" + ((orderByFieldNames.Length > 0) ? " ORDER BY " + getOrderByData(orderByFieldNames) + " " + ((orderByAsc) ? "ASC" : "DESC") : "");

                if (dbType == DatabaseType.MSSQL)
                {
                    SqlDataAdapter a = new SqlDataAdapter(query, MSSQLConn);
                    data = new DataTable();
                    a.Fill(data);
                    a.Dispose();
                }
                else if (dbType == DatabaseType.MYSQL)
                {
                    MySqlDataAdapter a = new MySqlDataAdapter(query, MySQLConn);
                    data = new DataTable();
                    a.Fill(data);
                    a.Dispose();
                }
                else if (dbType == DatabaseType.MSACCESS2003 | dbType == DatabaseType.MSACCESS2007)
                {
                    OleDbDataAdapter a = new OleDbDataAdapter(query, MSDBSQLConn);
                    data = new DataTable();
                    a.Fill(data);
                    a.Dispose();
                }
            }
            catch (Exception eee)
            {
                OnError(eee);
                data = new DataTable();
                data.Columns.AddRange(new DataColumn[] { new DataColumn("Error"), new DataColumn("InnerError") });
                data.Rows.Add(eee.Message, eee.ToString());
            }
            finally
            {
                if (!connectedP)
                    closeConnection();
            }
            return data;
        }

        public DataTable DLookupT(string query)
        {
            DataTable data = null;
            bool connectedP = isConnectedDo;
            try
            {
                if (dbType == DatabaseType.MSSQL)
                {
                    SqlDataAdapter a = new SqlDataAdapter(query, MSSQLConn);
                    data = new DataTable();
                    a.Fill(data);
                    a.Dispose();
                }
                else if (dbType == DatabaseType.MYSQL)
                {
                    MySqlDataAdapter a = new MySqlDataAdapter(query, MySQLConn);
                    data = new DataTable();
                    a.Fill(data);
                    a.Dispose();
                }
                else if (dbType == DatabaseType.MSACCESS2003 | dbType == DatabaseType.MSACCESS2007)
                {
                    OleDbDataAdapter a = new OleDbDataAdapter(query, MSDBSQLConn);
                    data = new DataTable();
                    a.Fill(data);
                    a.Dispose();
                }
            }
            catch (Exception eee)
            {
                OnError(eee);
                data = new DataTable();
                data.Columns.AddRange(new DataColumn[] { new DataColumn("Error"), new DataColumn("InnerError") });
                data.Rows.Add(eee.Message, eee.ToString());
            }
            finally
            {
                if (!connectedP)
                    closeConnection();
            }
            return data;
        }

        public int DTSQLCMD(string query)
        {
            int rowsUpdated = -1;
            bool connectedP = isConnectedDo;
            try
            {
                if (dbType == DatabaseType.MSSQL)
                {
                    rowsUpdated = new SqlCommand(query, MSSQLConn).ExecuteNonQuery();
                }
                else if (dbType == DatabaseType.MYSQL)
                {
                    rowsUpdated = new MySqlCommand(query, MySQLConn).ExecuteNonQuery();
                }
                else if (dbType == DatabaseType.MSACCESS2003 | dbType == DatabaseType.MSACCESS2007)
                {
                    rowsUpdated = new OleDbCommand(query, MSDBSQLConn).ExecuteNonQuery();
                }
            }
            catch (Exception eee)
            {
                OnError(eee);
            }
            finally
            {
                if (!connectedP)
                    closeConnection();
            }
            return rowsUpdated;
        }

        public object DLookup(string table, string fieldName)
        {
            return DLookup(table, fieldName, "1=1", new string[0], false);
        }

        public object DLookup(string table, string fieldName, string clauses)
        {
            return DLookup(table, fieldName, clauses, new string[0], false);
        }

        public object DLookup(string table, string fieldName, string clauses, string orderByFieldName, bool orderByAsc)
        {
            return DLookup(table, fieldName, clauses, new string[] { orderByFieldName }, false);
        }

        public object DLookup(string table, string fieldName, string clauses, string[] orderByFieldNames, bool orderByAsc)
        {
            object data = null;
            bool connectedP = isConnectedDo;
            try
            {
                string query = "SELECT TOP 1 " + fieldName + " FROM " + table + " WHERE (" + clauses + ")" + ((orderByFieldNames.Length > 0) ? " ORDER BY " + getOrderByData(orderByFieldNames) + " " + ((orderByAsc) ? "ASC" : "DESC") : "");
                    
                if (dbType == DatabaseType.MSSQL)
                {
                    getQueryDbChanger(query, SqlTypes.SELECT);
                    SqlDataReader Dr = new SqlCommand(query, MSSQLConn).ExecuteReader();
                    if (Dr.Read())
                    {
                        data = Dr[0];
                    }
                    Dr.Close();
                }
                else if (dbType == DatabaseType.MYSQL)
                {
                    MySqlDataReader Dr = new MySqlCommand(query, MySQLConn).ExecuteReader();
                    if (Dr.Read())
                    {
                        data = Dr[0];
                    }
                    Dr.Close();
                }
                else if (dbType == DatabaseType.MSACCESS2003 | dbType == DatabaseType.MSACCESS2007)
                {
                    OleDbDataReader Dr = new OleDbCommand(query, MSDBSQLConn).ExecuteReader();
                    if (Dr.Read())
                    {
                        data = Dr[0];
                    }
                    Dr.Close();
                }
            }
            catch (Exception eee)
            {
                OnError(eee);
                data = eee.Message;
            }
            finally
            {
                if (!connectedP)
                    closeConnection();
            }
            return data;
        }

        public object DLookup(string sqlQuery)
        {
            object data = null;
            bool connectedP = isConnectedDo;
            try
            {
                if (dbType == DatabaseType.MSSQL)
                {
                    getQueryDbChanger(sqlQuery, SqlTypes.SELECT);
                    SqlDataReader Dr = new SqlCommand(sqlQuery, MSSQLConn).ExecuteReader();
                    if (Dr.Read())
                    {
                        data = Dr[0];
                    }
                    Dr.Close();
                }
                else if (dbType == DatabaseType.MYSQL)
                {
                    MySqlDataReader Dr = new MySqlCommand(sqlQuery, MySQLConn).ExecuteReader();
                    if (Dr.Read())
                    {
                        data = Dr[0];
                    }
                    Dr.Close();
                }
                else if (dbType == DatabaseType.MSACCESS2003 | dbType == DatabaseType.MSACCESS2007)
                {
                    OleDbDataReader Dr = new OleDbCommand(sqlQuery, MSDBSQLConn).ExecuteReader();
                    if (Dr.Read())
                    {
                        data = Dr[0];
                    }
                    Dr.Close();
                }
            }
            catch (Exception eee)
            {
                OnError(eee);
                data = eee.Message;
            }
            finally
            {
                if (!connectedP)
                    closeConnection();
            }
            return data;
        }

        public int DInsertRID(string sqlQuery)
        {
            int data = -1;
            bool connectedP = isConnectedDo;
            try
            {
                if (dbType == DatabaseType.MSSQL)
                {
                    getQueryDbChanger(sqlQuery, SqlTypes.SELECT);
                    using (SqlDataReader Dr = new SqlCommand(sqlQuery + ";SELECT CAST(SCOPE_IDENTITY() as int)", MSSQLConn).ExecuteReader())
                    {
                        if (Dr.Read())
                        {
                            data = Int32.Parse(Dr[0].ToString());
                        }
                    }
                }
            }
            catch (Exception eee)
            {
                OnError(eee);
            }
            finally
            {
                if (!connectedP)
                    closeConnection();
            }
            return data;
        }

        public object[] DLookupA(string table, string fieldName)
        {
            return DLookupA(table, fieldName, "1=1", new string[0], false);
        }

        public object[] DLookupA(string table, string fieldName, string clauses)
        {
            return DLookupA(table, fieldName, clauses, new string[0], false);
        }

        public object[] DLookupA(string table, string fieldName, string clauses, string orderByFieldName, bool orderByAsc)
        {
            return DLookupA(table, fieldName, clauses, new string[] { orderByFieldName }, false);
        }

        public object[] DLookupA(string table, string fieldName, string clauses, string[] orderByFieldNames, bool orderByAsc)
        {
            List<object> data = new List<object>();
            bool connectedP = isConnectedDo;
            string query = "";
            try
            {
                query = "SELECT " + fieldName + " FROM " + table + " WHERE (" + clauses + ")" + ((orderByFieldNames.Length > 0) ? " ORDER BY " + getOrderByData(orderByFieldNames) + " " + ((orderByAsc) ? "ASC" : "DESC") : "");

                if (dbType == DatabaseType.MSSQL)
                {
                    getQueryDbChanger(query, SqlTypes.SELECT);
                    SqlDataReader Dr = new SqlCommand(query, MSSQLConn).ExecuteReader();
                    while (Dr.Read())
                    {
                        data.Add(Dr[0]);
                    }
                    Dr.Close();
                }
                else if (dbType == DatabaseType.MYSQL)
                {
                    MySqlDataReader Dr = new MySqlCommand(query, MySQLConn).ExecuteReader();
                    while (Dr.Read())
                    {
                        data.Add(Dr[0]);
                    }
                    Dr.Close();
                }
                else if (dbType == DatabaseType.MSACCESS2003 | dbType == DatabaseType.MSACCESS2007)
                {
                    OleDbDataReader Dr = new OleDbCommand(query, MSDBSQLConn).ExecuteReader();
                    while (Dr.Read())
                    {
                        data.Add(Dr[0]);
                    }
                    Dr.Close();
                }
            }
            catch (Exception eee)
            {
                OnError(eee);
                data.Clear();
                data.Add("ERROR: " + eee.Message);
                data.Add("ERRORI: " + table);
                data.Add("ERRORI: " + fieldName);
                data.Add("ERRORI: " + clauses);
                data.Add("ERRORI: " + query);
            }
            finally
            {
                if (!connectedP)
                    closeConnection();
            }
            return data.ToArray();
        }

        private string getOrderByData(string[] clauses)
        {
            string retS = "";
            for (int i = 0; i < clauses.Length; i++)
            {
                if (i > 0)
                    retS += ", ";
                retS += clauses[i];
            }
            return retS;
        }

        public System.Drawing.Bitmap DLookupIMG(string table, string fieldName, string clauses)
        {
            return DLookupIMG(table, fieldName, clauses, new string[0] {}, false);
        }

        public System.Drawing.Bitmap DLookupIMG(string table, string fieldName, string clauses, string orderByFieldNames, bool orderByAsc)
        {
            return DLookupIMG(table, fieldName, clauses, new string[] { orderByFieldNames }, orderByAsc);
        }

        public System.Drawing.Bitmap DLookupIMG(string table, string fieldName, string clauses, string[] orderByFieldNames, bool orderByAsc)
        {
            System.Drawing.Bitmap bmp = null;
            bool connectedP = isConnectedDo;
            try
            {
                string query = "SELECT TOP 1 " + fieldName + " FROM " + table + " WHERE (" + clauses + ")" + ((orderByFieldNames.Length > 0) ? " ORDER BY " + getOrderByData(orderByFieldNames) + " " + ((orderByAsc) ? "ASC" : "DESC") : "");

                if (dbType == DatabaseType.MSSQL)
                {
                    getQueryDbChanger(query, SqlTypes.SELECT);
                    object img = new SqlCommand(query, MSSQLConn).ExecuteScalar();
                    MemoryStream memImg = new MemoryStream((byte[])img);
                    bmp = new System.Drawing.Bitmap(memImg);
                }
                else if (dbType == DatabaseType.MYSQL)
                {
                    /*MySqlDataReader Dr = new MySqlCommand(query, MySQLConn).ExecuteReader();
                    if (Dr.Read())
                    {
                        data = Dr[0];
                    }
                    Dr.Close();*/
                }
                else if (dbType == DatabaseType.MSACCESS2003 | dbType == DatabaseType.MSACCESS2007)
                {
                    /*OleDbDataReader Dr = new OleDbCommand(query, MSDBSQLConn).ExecuteReader();
                    if (Dr.Read())
                    {
                        data = Dr[0];
                    }
                    Dr.Close();*/
                }
            }
            catch (Exception ee)
            {
                OnError(ee);
            }
            finally
            {
                if (!connectedP)
                    closeConnection();
            }
            return bmp;
        }

        /*public object DLookup(string table, string field, DLookupData[] clauses)
        {
            object data = null;
            try
            {
            }
            catch (Exception ee)
            {
                OnError(ee);
            }
            return data;
        }*/

        public object[] executeSql(string query, SqlTypes type, SqlParameter[] parameters)
        {
            /*if (!Licencing.validate())
                return null;*/

            object[] returnObj = new object[5];
            returnObj[1] = SqlHasReturn.NO;
            //if (key.Equals(SecurityKey))
            {
                bool connectedP = isConnectedDo;
                try
                {

                    if (type == SqlTypes.AUTO)
                    {
                        if (query.ToUpper().StartsWith("INSERT"))
                            type = SqlTypes.INSERT;
                        else if (query.ToUpper().StartsWith("SELECT"))
                            type = SqlTypes.SELECT;
                        else if (query.ToUpper().StartsWith("UPDATE"))
                            type = SqlTypes.UPDATE;
                        else
                            throw new Exception("");

                        if (type == SqlTypes.INSERT & parameters != null)
                            type = SqlTypes.INSERT_CUSTOM;

                    }

                    if (dbType == DatabaseType.MSSQL)
                        query = getQueryDbChanger(query, type);

                    if (type == SqlTypes.STORED_PROCEDURE)
                    {
                        if (dbType == DatabaseType.MSSQL)
                        {
                            SqlCommand command = new SqlCommand();
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(parameters);
                            command.Connection = MSSQLConn;
                            command.CommandText = query;
                            DataTable dt = new DataTable();
                            SqlDataAdapter da = new SqlDataAdapter(command);
                            da.Fill(dt);
                            da.Dispose();

                            if (dt.Rows.Count > 0)
                            {
                                returnObj[1] = SqlHasReturn.YES;
                                returnObj[2] = Int32.Parse(dt.Rows[0].ItemArray[0].ToString());
                            }
                        }
                    }
                    else if (type == SqlTypes.STORE_PROCEDURE_VALUE)
                    {
                        if (dbType == DatabaseType.MSSQL)
                        {
                            Object returnValue;
                            SqlCommand command = new SqlCommand();

                            foreach (SqlParameter obj in parameters)
                                command.Parameters.Add(obj);

                            var returnParameter = command.Parameters.Add("@ReturnVal", SqlDbType.Int);
                            returnParameter.Direction = ParameterDirection.ReturnValue;

                            command.CommandType = CommandType.StoredProcedure;
                            command.Connection = MSSQLConn;
                            command.CommandText = query;
                            command.ExecuteNonQuery();

                            returnValue = returnParameter;
                            returnObj[1] = SqlHasReturn.YES;
                            returnObj[2] = returnValue;
                        }
                    }
                    else if (type == SqlTypes.STORED_PROCEDURE_DATATABLE)
                    {
                        if (dbType == DatabaseType.MSSQL)
                        {
                            SqlCommand command = new SqlCommand();
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(parameters);
                            command.Connection = MSSQLConn;
                            command.CommandText = query;
                            DataTable dt = new DataTable();
                            SqlDataAdapter da = new SqlDataAdapter(command);
                            da.Fill(dt);
                            da.Dispose();
                            returnObj[1] = SqlHasReturn.YES;
                            returnObj[2] = dt;
                        }
                    }
                    else if (type == SqlTypes.INSERT_RETURN_ID)
                    {
                        if (dbType == DatabaseType.MSSQL)
                        {
                            using (SqlDataReader Dr = new SqlCommand(query + ";SELECT CAST(SCOPE_IDENTITY() as int)", MSSQLConn).ExecuteReader())
                            {
                                if (Dr.Read())
                                {
                                    returnObj[1] = SqlHasReturn.YES;
                                    returnObj[2] = Int32.Parse(Dr[0].ToString());
                                }
                            }
                        }
                    }
                    else if (type == SqlTypes.INSERT)
                    {
                        if (dbType == DatabaseType.MSSQL)
                        {
                            using (SqlCommand commando = new SqlCommand(query, MSSQLConn))
                            {
                                int rws = commando.ExecuteNonQuery();

                                if (rws > 0)
                                    returnObj[1] = SqlHasReturn.YES;

                                returnObj[2] = rws;
                            }
                        }
                        else if (dbType == DatabaseType.MSSQL)
                        {
                            using (MySqlCommand commando = new MySqlCommand(query, MySQLConn))
                            {
                                int rws = commando.ExecuteNonQuery();

                                if (rws > 0)
                                    returnObj[1] = SqlHasReturn.YES;

                                returnObj[2] = rws;
                            }
                        }
                        else if (dbType == DatabaseType.MSACCESS2003 | dbType == DatabaseType.MSACCESS2007)
                        {
                            using (OleDbCommand commando = new OleDbCommand(query, MSDBSQLConn))
                            {
                                int rws = commando.ExecuteNonQuery();

                                if (rws > 0)
                                    returnObj[1] = SqlHasReturn.YES;

                                returnObj[2] = rws;
                            }
                        }
                    }
                    else if (type == SqlTypes.SELECT)
                    {
                        if (dbType == DatabaseType.MSSQL)
                        {
                            SqlDataReader Dr = new SqlCommand(query, MSSQLConn).ExecuteReader();
                            returnObj[1] = SqlHasReturn.YES;
                            returnObj[2] = Dr;
                        }
                        else if (dbType == DatabaseType.MYSQL)
                        {
                            MySqlDataReader Dr = new MySqlCommand(query, MySQLConn).ExecuteReader();
                            returnObj[1] = SqlHasReturn.YES;
                            returnObj[2] = Dr;
                        }
                        else if (dbType == DatabaseType.MSACCESS2003 | dbType == DatabaseType.MSACCESS2007)
                        {
                            OleDbDataReader Dr = new OleDbCommand(query, MSDBSQLConn).ExecuteReader();
                            returnObj[1] = SqlHasReturn.YES;
                            returnObj[2] = Dr;
                        }
                    }
                    else if (type == SqlTypes.SELECT_DATATABLE)
                    {
                        if (dbType == DatabaseType.MSSQL)
                        {
                            SqlDataAdapter a = new SqlDataAdapter(query, MSSQLConn);
                            DataTable dt = new DataTable();
                            a.Fill(dt);
                            returnObj[1] = SqlHasReturn.YES;
                            returnObj[2] = dt;
                            a.Dispose();
                        }
                        else if (dbType == DatabaseType.MYSQL)
                        {
                            MySqlDataAdapter a = new MySqlDataAdapter(query, MySQLConn);
                            DataTable dt = new DataTable();
                            a.Fill(dt);
                            returnObj[1] = SqlHasReturn.YES;
                            returnObj[2] = dt;
                            a.Dispose();
                        }
                        else if (dbType == DatabaseType.MSACCESS2003 | dbType == DatabaseType.MSACCESS2007)
                        {
                            OleDbDataAdapter a = new OleDbDataAdapter(query, MSDBSQLConn);
                            DataTable dt = new DataTable();
                            a.Fill(dt);
                            returnObj[1] = SqlHasReturn.YES;
                            returnObj[2] = dt;
                            a.Dispose();
                        }
                    }
                    else if (type == SqlTypes.UPDATE)
                    {
                        if (dbType == DatabaseType.MSSQL)
                        {
                            SqlDataReader Dr = new SqlCommand(query, MSSQLConn).ExecuteReader();
                            returnObj[1] = SqlHasReturn.YES;
                            returnObj[2] = Dr;
                        }
                        else if (dbType == DatabaseType.MYSQL)
                        {
                            MySqlDataReader Dr = new MySqlCommand(query, MySQLConn).ExecuteReader();
                            returnObj[1] = SqlHasReturn.YES;
                            returnObj[2] = Dr;
                        }
                        else if (dbType == DatabaseType.MSACCESS2003 | dbType == DatabaseType.MSACCESS2007)
                        {
                            OleDbDataReader Dr = new OleDbCommand(query, MSDBSQLConn).ExecuteReader();
                            returnObj[1] = SqlHasReturn.YES;
                            returnObj[2] = Dr;
                        }
                    }
                    else if (type == SqlTypes.INSERT_CUSTOM & parameters != null)
                    {
                        if (dbType == DatabaseType.MSSQL)
                        {
                            using (SqlCommand commanda = new SqlCommand(query, MSSQLConn))
                            {
                                commanda.Parameters.AddRange(parameters);

                                int rws = commanda.ExecuteNonQuery();

                                if (rws > 0)
                                    returnObj[1] = SqlHasReturn.YES;

                                returnObj[2] = rws;
                            }
                        }
                        else if (dbType == DatabaseType.MYSQL)
                        {
                            using (MySqlCommand commanda = new MySqlCommand(query, MySQLConn))
                            {
                                commanda.Parameters.AddRange(parameters);

                                int rws = commanda.ExecuteNonQuery();

                                if (rws > 0)
                                    returnObj[1] = SqlHasReturn.YES;

                                returnObj[2] = rws;
                            }
                        }
                        else if (dbType == DatabaseType.MSACCESS2003 | dbType == DatabaseType.MSACCESS2007)
                        {
                            using (OleDbCommand commanda = new OleDbCommand(query, MSDBSQLConn))
                            {
                                commanda.Parameters.AddRange(parameters);

                                int rws = commanda.ExecuteNonQuery();

                                if (rws > 0)
                                    returnObj[1] = SqlHasReturn.YES;

                                returnObj[2] = rws;
                            }
                        }
                    }
                }
                catch (Exception eee)
                {
                    OnError(eee);
                    returnObj[1] = SqlHasReturn.NO;
                    returnObj[4] = eee;
                }
                finally
                {
                    if (!connectedP & type != SqlTypes.SELECT)
                        closeConnection();
                }
                returnObj[0] = SqlResult.SUCCESS;
                return returnObj;
            }
            /*else
                return new object[] { SqlResult.FAIL, false };*/
        }

        private string getQueryDbChanger(string query, SqlTypes type)
        {
            try
            {
                string nQuery = query;
                if (type == SqlTypes.INSERT | type == SqlTypes.INSERT_CUSTOM | type == SqlTypes.INSERT_RETURN_ID)
                {
                    if (query.ToUpper().StartsWith("INSERT INTO "))
                    {
                        nQuery = query.Replace(query.Split(' ')[2], "[" + database + "].[dbo].[" + query.Split(' ')[2] + "]");
                        //while (nQuery.Contains("~" + query.Split(' ')[2]))
                        //nQuery = nQuery.Replace("~" + query.Split(' ')[2], "[" + database + "].[dbo].[" + query.Split(' ')[2] + "]");
                    }
                }
                else if (type == SqlTypes.SELECT)
                {
                    if (query.ToUpper().StartsWith("SELECT "))
                    {
                        int fromStartIndex = query.ToUpper().IndexOf("FROM ");
                        nQuery = query.Insert(fromStartIndex + 5, "[" + database + "].[dbo].[");
                        int endOfTableName = -1;
                        for (int i = fromStartIndex + 5 + ("[" + database + "].[dbo].[").Length; i < nQuery.Length; i++)
                        {
                            if (nQuery.ToCharArray()[i] == ' ')
                                endOfTableName = i;

                            if (endOfTableName != -1)
                                i = nQuery.Length;
                        }
                        if (endOfTableName == -1)
                            nQuery = nQuery + "]";
                        else
                        {
                            nQuery = nQuery.Insert(endOfTableName, "]");
                        }
                    }
                }
                else if (type == SqlTypes.UPDATE)
                {
                    if (query.ToUpper().StartsWith("UPDATE "))
                    {
                        nQuery = query.Replace(query.Split(' ')[1], "[" + database + "].[dbo].[" + query.Split(' ')[1] + "]");
                    }
                }
                return nQuery;
            }
            catch (Exception ee)
            {
                OnError(ee);
            }
            return query;
        }
    }

    public enum DatabaseType
    {
        MSSQL,
        MYSQL,
        MSACCESS2003,
        MSACCESS2007,
        NONE
    }

    public class DLookupData
    {
        string field;
        object clause;
        DLookupData[] innerLookups = new DLookupData[0];

        public DLookupData(string FieldName, object ClauseData)
        {
            field = FieldName;
            clause = ClauseData;
        }

        public DLookupData(DLookupData[] clauses)
        {
            innerLookups = clauses;
        }

        public string FieldName
        {
            get
            {
                return field;
            }
        }

        public object ClauseData
        {
            get
            {
                return clause;
            }
        }

        public Type ClauseDataType
        {
            get
            {
                return clause.GetType();
            }
        }
    }
}
