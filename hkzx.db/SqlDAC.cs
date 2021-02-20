using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using MS.Lib.Data;

namespace hkzx.db
{
    /// <summary>
    ///SqlDAC 的摘要说明
    /// </summary>
    public class SqlDAC //: IDisposable
    {
        private SqlConnection _sqlConn;    //数据库连接
        public SqlDAC(string ConnString)
        {
            try
            {
                //获取数据库连接字符串
                //string ConnString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnMain"].ConnectionString;
                _sqlConn = new SqlConnection(ConnString);//获取并打开数据库连接
                _sqlConn.Open();
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
        }

        #region 清理资源
        ~SqlDAC()
        {
            try
            {
                //if (_sqlConn != null && _sqlConn.State != ConnectionState.Closed)
                //{
                //    _sqlConn.Close();
                //}
                //_sqlConn.Dispose();
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
        }

        //private bool IsDisposed = false;
        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}
        //private void Dispose(bool Disposing)
        //{
        //    if (!IsDisposed)
        //    {
        //        if (Disposing)
        //        {
        //            //清理托管资源
        //        }
        //        //清理非托管资源。比如关闭数据库的连接，关闭文件句柄等
        //        try
        //        {
        //            //_sqlConn.Dispose();
        //        }
        //        catch
        //        {
        //        }
        //    }
        //    IsDisposed = true;
        //    GC.Collect();
        //}
        #endregion

        #region 插入
        /// <summary>
        /// 插入记录，返回插入Id
        /// </summary>
        /// <returns></returns>
        public int InsertQuery(string strTableName, SqlParameter[] sqlParaArray)
        {
            //SqlCmd 初始化 Parameters <字段列表> 和 <Values>
            if (sqlParaArray != null)
            {
                string strInsert = "";                  //Insert语句字段列表
                string strValues = "";                  //Insert语句Values列表
                for (int i = 0; i < sqlParaArray.Length; i++)
                {
                    if (i > 0)
                    {
                        strInsert += ", ";
                        strValues += ", ";
                    }
                    strInsert += sqlParaArray[i].SourceColumn;
                    if (sqlParaArray[i].Value == null)
                    {
                        strValues += "null";
                    }
                    else
                    {
                        strValues += sqlParaArray[i].ParameterName;
                    }
                }
                string strSQL = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", strTableName, strInsert, strValues);
                return ExecProcedure(strSQL, sqlParaArray, true);
            }
            return 0;
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改记录
        /// </summary>
        /// <returns></returns>
        public int UpdateQuery(string strTableName, SqlParameter[] sqlParaArray, SqlParameter[] sqlParaArrayWhere)
        {
            string strUpdate = "";      //Update语句Set子句
            string strWhere = "";       //Update语句Where子句
            // 初始化 Set子句
            if (sqlParaArray != null)
            {
                for (int i = 0; i < sqlParaArray.Length; i++)
                {
                    if (i > 0)
                    {
                        strUpdate += ", ";
                    }
                    if (sqlParaArray[i].Value == null)
                    {
                        strUpdate += sqlParaArray[i].SourceColumn + "=null";
                    }
                    else
                    {
                        strUpdate += string.Format("{0}={1}", sqlParaArray[i].SourceColumn, sqlParaArray[i].ParameterName);
                    }
                }
            }
            // 初始化 Where子句
            if (sqlParaArrayWhere != null)
            {
                for (int i = 0; i < sqlParaArrayWhere.Length; i++)
                {
                    if (i > 0)
                    {
                        strUpdate += " AND ";
                    }
                    if (sqlParaArrayWhere[i].Value == null)
                    {
                        strWhere += sqlParaArrayWhere[i].SourceColumn += "=null";
                    }
                    else
                    {
                        strWhere += string.Format("{0}={1}", sqlParaArrayWhere[i].SourceColumn, sqlParaArrayWhere[i].ParameterName);
                    }
                }
            }
            if (strUpdate != "")
            {
                string strSQL = string.Format("UPDATE {0} SET {1} WHERE {2}", strTableName, strUpdate, strWhere);
                List<SqlParameter> list = new List<SqlParameter>();
                list.AddRange(sqlParaArray);
                list.AddRange(sqlParaArrayWhere);
                SqlParameter[] sqlParameters = list.ToArray();
                return ExecProcedure(strSQL, sqlParameters, false);
            }
            return 0;
        }
        /// <summary>
        /// 修改记录
        /// </summary>
        /// <returns></returns>
        public int UpdateQuery(string strSQL, SqlParameter[] sqlParameters)
        {
            return ExecProcedure(strSQL, sqlParameters, false);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="strTableName"></param>
        /// <param name="sqlParaArrayWhere"></param>
        /// <returns></returns>
        public int DeleteQuery(string strTableName, SqlParameter[] sqlParaArrayWhere)
        {
            string strWhere = "";
            if (sqlParaArrayWhere != null)
            {
                for (int i = 0; i < sqlParaArrayWhere.Length; i++)
                {
                    if (i > 0)
                    {
                        strWhere += " AND ";
                    }
                    if (sqlParaArrayWhere[i].Value == null)
                    {
                        strWhere += sqlParaArrayWhere[i].SourceColumn += "=null";
                    }
                    else
                    {
                        strWhere += string.Format("{0}={1}", sqlParaArrayWhere[i].SourceColumn, sqlParaArrayWhere[i].ParameterName);
                    }
                }
                string strSQL = string.Format("DELETE FROM {0} WHERE {1}", strTableName, strWhere);
                return ExecProcedure(strSQL, sqlParaArrayWhere, false);
            }
            return 0;
        }
        public int DeleteQuerySql(string strSQL, SqlParameter[] sqlParaArrayWhere)
        {
            return ExecProcedure(strSQL, sqlParaArrayWhere, false);
        }
        #endregion

        #region 存储过程
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <returns></returns>
        public int ExecProcedure(string strSQL, SqlParameter[] sqlParaArray, bool bMode)
        {
            SqlCommand sqlCmd = new SqlCommand();   //定义SqlCommand
            int intResult = 0;              //SqlCommand执行后返回的影响记录数，bMode为true时，返回插入主键Id值
            sqlCmd.CommandText = strSQL;    //SqlCmd 初始化 CommandText
            //SqlCmd 初始化 Parameters
            if (sqlParaArray != null)
            {
                for (int i = 0; i < sqlParaArray.Length; i++)
                {
                    if (sqlParaArray[i].Value != null)
                    {
                        sqlCmd.Parameters.Add(sqlParaArray[i]);
                    }
                }
            }
            //SqlCmd 初始化数据库连接
            if (_sqlConn.State.ToString().CompareTo("Closed") == 0)
            {
                _sqlConn.Open();
            }
            SqlTransaction sqlTran = _sqlConn.BeginTransaction();
            try
            {
                sqlCmd.Connection = _sqlConn;
                sqlCmd.Transaction = sqlTran;
                //执行SqlCmd
                intResult = sqlCmd.ExecuteNonQuery();
                sqlTran.Commit();
                if (bMode)
                {
                    sqlCmd.CommandText = "SELECT @@IDENTITY";
                    intResult = Convert.ToInt32(sqlCmd.ExecuteScalar());
                }
            }
            catch (SqlException e)
            {
                sqlTran.Rollback();
                throw new Exception(e.Message);
            }
            finally
            {
                _sqlConn.Close();
            }
            return intResult;
        }
        #endregion

        #region 查询
        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <returns></returns>
        public Array GetDataByAnyCondition(string strSQL, SqlParameter[] sqlParaArray, Type dataType)
        {
            ArrayList dataList = new ArrayList();
            SqlDataReader dr = null;
            try
            {
                SqlCommand sqlCmd = new SqlCommand();   //定义SqlCommand
                sqlCmd.CommandText = strSQL;    //SqlCmd 初始化 CommandText
                sqlCmd.Parameters.Clear();
                //SqlCmd 初始化 Parameters
                if (sqlParaArray != null)
                {
                    for (int i = 0; i < sqlParaArray.Length; i++)
                    {
                        SqlParameter pa = (SqlParameter)((ICloneable)sqlParaArray[i]).Clone();
                        sqlCmd.Parameters.Add(pa);
                    }
                }
                //SqlCmd 初始化数据库连接
                if (_sqlConn.State.ToString().CompareTo("Closed") == 0)
                {
                    _sqlConn.Open();
                }
                sqlCmd.Connection = _sqlConn;
                dr = sqlCmd.ExecuteReader();    //执行SqlCmd
                MSBaseData data;
                while (dr.Read())
                {
                    data = (MSBaseData)Activator.CreateInstance(dataType);
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        object readData = dr[i];
                        if (readData == DBNull.Value)
                        {
                            readData = null;
                        }
                        data.SetData(dr.GetName(i), readData);
                    }
                    dataList.Add(data);
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
                _sqlConn.Close();
            }
            return dataList.ToArray(dataType);
        }
        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <returns></returns>
        public Array SelelctQuery(string strTableName, SqlParameter[] sqlParaArray, SqlParameter[] sqlParaArrayOrderBy, Type dataType)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.AppendFormat("SELECT * FROM {0}", strTableName);
            if (sqlParaArray != null)
            {
                for (int i = 0; i < sqlParaArray.Length; i++)
                {
                    string strAppend = "";
                    if (i == 0)
                    {
                        strAppend += " WHERE ";
                    }
                    else
                    {
                        strAppend += " AND ";
                    }
                    if (sqlParaArray[i].Value == null)
                    {
                        strAppend += sqlParaArray[i].SourceColumn += "=null";
                    }
                    else
                    {
                        strAppend += string.Format("{0}={1}", sqlParaArray[i].SourceColumn, sqlParaArray[i].ParameterName);
                    }
                    strSQL.Append(strAppend);
                }
            }
            if (sqlParaArrayOrderBy != null)
            {
                for (int i = 0; i < sqlParaArrayOrderBy.Length; i++)
                {
                    if (i == 0)
                    {
                        strSQL.Append(string.Format(" ORDER BY {0} {1}", sqlParaArrayOrderBy[i].SourceColumn, sqlParaArrayOrderBy[i].Value));
                    }
                    else
                    {
                        strSQL.Append(string.Format(", {0} {1}", sqlParaArrayOrderBy[i].SourceColumn, sqlParaArrayOrderBy[i].Value));
                    }
                }
            }
            List<SqlParameter> list = new List<SqlParameter>();
            list.AddRange(sqlParaArray);
            list.AddRange(sqlParaArrayOrderBy);
            SqlParameter[] sqlParameters = list.ToArray();
            return GetDataByAnyCondition(strSQL.ToString(), sqlParameters, dataType);
        }
        #endregion

        #region 获取指定要求的数值
        /// <summary>
        /// 获取指定要求的数值
        /// </summar
        /// <returns></returns>
        public string GetSpecValue(string strSQL)
        {
            return GetSpecValue(strSQL, null);
        }
        /// <summary>
        /// 获取指定要求的数值，带有where条件
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <param name="sqlParaArrayWhere">where条件参数</param>
        /// <returns></returns>
        public string GetSpecValue(string strSQL, SqlParameter[] sqlParaArrayWhere)
        {
            string strValue = "";//指定要求的数值
            try
            {
                SqlCommand sqlCmd = new SqlCommand();   //定义SqlCommand
                sqlCmd.CommandText = strSQL;    //SqlCmd 初始化 CommandText
                //SqlCmd 初始化 Parameters
                if (sqlParaArrayWhere != null)
                {
                    for (int i = 0; i < sqlParaArrayWhere.Length; i++)
                    {
                        sqlCmd.Parameters.Add(sqlParaArrayWhere[i]);
                    }
                }
                //SqlCmd 初始化数据库连接
                if (_sqlConn.State.ToString().CompareTo("Closed") == 0)
                {
                    _sqlConn.Open();
                }
                sqlCmd.Connection = _sqlConn;
                //执行SqlCmd
                object obj = sqlCmd.ExecuteScalar();//SqlCmd执行返回结果
                //返回值
                if (obj != null && obj != DBNull.Value)
                {
                    strValue = obj.ToString();
                }
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                _sqlConn.Close();
            }
            return strValue;
        }
        #endregion
        //
    }
}