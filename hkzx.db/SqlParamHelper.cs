using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace hkzx.db
{
    public class SqlParamHelper
    {
        public static SqlParameter AddParameter(string strParameterName, SqlDbType sqlDbType, string strSourceColumn, bool decValue)
        {
            SqlParameter sqlParam = new SqlParameter();
            sqlParam.ParameterName = strParameterName;
            sqlParam.SqlDbType = sqlDbType;
            sqlParam.SourceColumn = strSourceColumn;
            sqlParam.Value = decValue;
            return sqlParam;
        }

        public static SqlParameter AddParameter(string strParameterName, SqlDbType sqlDbType, string strSourceColumn, int intValue)
        {
            SqlParameter sqlParam = new SqlParameter();
            sqlParam.ParameterName = strParameterName;
            sqlParam.SqlDbType = sqlDbType;
            sqlParam.SourceColumn = strSourceColumn;
            sqlParam.Value = intValue;
            return sqlParam;
        }

        public static SqlParameter AddParameter(string strParameterName, SqlDbType sqlDbType, string strSourceColumn, float decValue)
        {
            SqlParameter sqlParam = new SqlParameter();
            sqlParam.ParameterName = strParameterName;
            sqlParam.SqlDbType = sqlDbType;
            sqlParam.SourceColumn = strSourceColumn;
            sqlParam.Value = decValue;
            return sqlParam;
        }

        public static SqlParameter AddParameter(string strParameterName, SqlDbType sqlDbType, string strSourceColumn, decimal decValue)
        {
            SqlParameter sqlParam = new SqlParameter();
            sqlParam.ParameterName = strParameterName;
            sqlParam.SqlDbType = sqlDbType;
            sqlParam.SourceColumn = strSourceColumn;
            sqlParam.Value = decValue;
            return sqlParam;
        }

        public static SqlParameter AddParameter(string strParameterName, SqlDbType sqlDbType, string strSourceColumn, string strValue)
        {
            SqlParameter sqlParam = new SqlParameter();
            sqlParam.ParameterName = strParameterName;
            sqlParam.SqlDbType = sqlDbType;
            sqlParam.SourceColumn = strSourceColumn;
            sqlParam.Value = strValue;
            return sqlParam;
        }

        public static SqlParameter AddParameter(string strParameterName, SqlDbType sqlDbType, string strSourceColumn, DateTime dateValue)
        {
            SqlParameter sqlParam = new SqlParameter();
            sqlParam.ParameterName = strParameterName;
            sqlParam.SqlDbType = sqlDbType;
            sqlParam.SourceColumn = strSourceColumn;
            sqlParam.Value = dateValue;
            return sqlParam;
        }
        //
    }
}