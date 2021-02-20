using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MS.Lib.Data;

namespace hkzx.db
{
    [Serializable]
    public class DataLog : MSBaseData
    {
        public int Id { get; set; }//主键
        public string TableName { get; set; }//表格名称
        public int TableId { get; set; }//表格Id
        public string Body { get; set; }//内容
        public int Active { get; set; }//状态：默认0为前台不显示
        public DateTime AddTime { get; set; }//添加时间：默认为当前时间
        public string AddIp { get; set; }//添加IP:端口号
        public string AddUser { get; set; }//添加人
        //分页统计
        public string rowClass { get; set; }//行class属性
        public int num { get; set; }//序号
        public int total { get; set; }//统计数
        public string ActiveName { get; set; }//状态名称
        public string AddTimeText { get; set; }//添加时间文本
        //
        private static string[] columnList = new[] {
            "Id", "TableName", "TableId", "Body", "Active", "AddTime", "AddIp", "AddUser"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] { SqlDbType.Int, 
            SqlDbType.Int, SqlDbType.VarChar, SqlDbType.Int, SqlDbType.NText, SqlDbType.Int, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar
        };
        public override SqlDbType[] GetColumnType()
        {
            return columnTypeList;
        }
        private static string[] primaryKeyList = new[] { "Id" };
        public override string[] GetPrimaryKey()
        {
            return primaryKeyList;
        }
        private static string[] nullableList = { };
        public override string[] GetNullableColumn()
        {
            return nullableList;
        }
    }
    //
    public class WebLog
    {
        private const string TableName = "tb_Log";
        SqlDAC sqlDac;
        public WebLog()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }
        //
        #region 查询
        public DataLog[] GetData(int intId, string strFields = "")
        {
            SqlParameter[] sqlParameters = new[] 
            { 
               new SqlParameter("@Id", SqlDbType.Int, 4)
            };
            sqlParameters[0].Value = intId;
            if (string.IsNullOrEmpty(strFields))
            {
                strFields = "*";
            }
            string strSql = string.Format("SELECT {1} FROM {0} WHERE Id=@Id", TableName, strFields);
            DataLog[] result = (DataLog[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataLog));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataLog[] GetDatas(DataLog data, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strFromWhere = string.Format("FROM {0} WHERE ", TableName);
            if (data.Active > 0)
            {
                strFromWhere += "Active>0";
            }
            else if (data.Active < 0)
            {
                strFromWhere += "Active<0";
            }
            else
            {
                strFromWhere += "1=1";
            }
            if (!string.IsNullOrEmpty(data.TableName))
            {
                list.Add(SqlParamHelper.AddParameter("@TableName", SqlDbType.VarChar, "TableName", data.TableName));
            }
            if (data.TableId > 0)
            {
                list.Add(SqlParamHelper.AddParameter("@TableId", SqlDbType.Int, "TableId", data.TableId));
                strFromWhere += " AND TableId=@TableId";
            }
            if (!string.IsNullOrEmpty(data.Body))
            {
                list.Add(SqlParamHelper.AddParameter("@Body", SqlDbType.NText, "Body", data.Body));
                strFromWhere += " AND Body LIKE @Body";
            }
            if (!string.IsNullOrEmpty(data.AddTimeText) && data.AddTimeText.IndexOf(",") >= 0)
            {
                string[] arr = data.AddTimeText.Split(',');
                if (!string.IsNullOrEmpty(arr[0]))
                {
                    list.Add(SqlParamHelper.AddParameter("@AddTime1", SqlDbType.DateTime, "AddTime1", Convert.ToDateTime(arr[0])));
                    strFromWhere += " AND AddTime>=@AddTime1";
                }
                if (!string.IsNullOrEmpty(arr[1]))
                {
                    list.Add(SqlParamHelper.AddParameter("@AddTime2", SqlDbType.DateTime, "AddTime2", Convert.ToDateTime(arr[1])));
                    strFromWhere += " AND AddTime<=@AddTime2";
                }
            }
            string strOrder = " ORDER BY ";
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                strOrder += strOrderBy;
            }
            else
            {
                strOrder += "Active DESC, AddTime DESC";
            }
            strOrder += ", Id ASC";
            if (strFields == "")
            {
                strFields = " * ";
            }
            else
            {
                strFields = " " + strFields + " ";
            }
            string strSql = "";
            if (pageSize > 0 && intPage > 1)
            {
                //分页查询语句：SELECT TOP 页大小 * FROM table WHERE id NOT IN ( SELECT TOP 页大小*(页数-1) id FROM table ORDER BY id ) ORDER BY id
                strSql = "SELECT TOP " + pageSize.ToString() + strFields + strFromWhere + " AND Id NOT IN ( SELECT TOP " + (pageSize * (intPage - 1)).ToString() + " Id " + strFromWhere + strOrder + " )" + strOrder;
            }
            else
            {
                if (pageSize <= 0)
                {
                    pageSize = 10000;//考虑数据库性能，只读取前10000条数据
                }
                strSql = "SELECT TOP " + pageSize.ToString() + strFields + strFromWhere + strOrder;
            }
            SqlParameter[] sqlParaArray = list.ToArray();
            DataLog[] result = (DataLog[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataLog));
            if (result != null && result.Length > 0)
            {
                if (strFilter.IndexOf("total") >= 0)
                {
                    strSql = "SELECT COUNT(Id) " + strFromWhere;
                    string strValue = sqlDac.GetSpecValue(strSql, sqlParaArray);//获取查询总数
                    if (!string.IsNullOrEmpty(strValue))
                    {
                        result[0].total = Convert.ToInt32(strValue);
                    }
                }
                return result;
            }
            return null;
        }
        public int GetCount(int Active, string strTableName, string AddTimeText)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strSql = string.Format("SELECT COUNT(Id) FROM {0} WHERE ", TableName);
            if (Active > 0)
            {
                strSql += "Active>0";
            }
            else if (Active < 0)
            {
                strSql += "Active<0";
            }
            else
            {
                strSql += "1=1";
            }
            if (!string.IsNullOrEmpty(strTableName))
            {
                string strTmp = "";
                string[] arr = strTableName.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "TableName" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.VarChar, tmp, arr[i]));
                        strTmp += "TableName=@" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strSql += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(AddTimeText) && AddTimeText.IndexOf(",") >= 0)
            {
                string[] arr = AddTimeText.Split(',');
                if (!string.IsNullOrEmpty(arr[0]))
                {
                    list.Add(SqlParamHelper.AddParameter("@AddTime1", SqlDbType.DateTime, "AddTime1", Convert.ToDateTime(arr[0])));
                    strSql += " AND AddTime>=@AddTime1";
                }
                if (!string.IsNullOrEmpty(arr[1]))
                {
                    if (arr[1].IndexOf(":") < 0)
                    {
                        arr[1] += " 23:59:59";
                    }
                    list.Add(SqlParamHelper.AddParameter("@AddTime2", SqlDbType.DateTime, "AddTime2", Convert.ToDateTime(arr[1])));
                    strSql += " AND AddTime<=@AddTime2";
                }
            }
            SqlParameter[] sqlParaArray = list.ToArray();
            string strValue = sqlDac.GetSpecValue(strSql, sqlParaArray);//获取查询总数
            if (!string.IsNullOrEmpty(strValue))
            {
                return Convert.ToInt32(strValue);
            }
            return -1;
        }
        #endregion
        //
        #region 修改
        //插入
        public int Insert(DataLog data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataLog data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataLog data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.TableName != null)
            {
                list.Add(SqlParamHelper.AddParameter("@TableName", SqlDbType.VarChar, "TableName", data.TableName));
            }
            if (data.TableId >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@TableId", SqlDbType.Int, "TableId", data.TableId));
            }
            if (data.Body != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Body", SqlDbType.NText, "Body", data.Body));
            }
            if (data.Active > -1000)
            {
                list.Add(SqlParamHelper.AddParameter("@Active", SqlDbType.Int, "Active", data.Active));
            }
            if (data.AddTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@AddTime", SqlDbType.DateTime, "AddTime", data.AddTime));
            }
            if (data.AddIp != null)
            {
                list.Add(SqlParamHelper.AddParameter("@AddIp", SqlDbType.VarChar, "AddIp", data.AddIp));
            }
            if (data.AddUser != null)
            {
                list.Add(SqlParamHelper.AddParameter("@AddUser", SqlDbType.NVarChar, "AddUser", data.AddUser));
            }
            return list.ToArray();
        }
        //修改状态
        public int UpdateActive(int intId, int intActive)
        {
            string strSql = string.Format("UPDATE {0} SET Active=@Active WHERE Id=@Id", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
            list.Add(SqlParamHelper.AddParameter("@Active", SqlDbType.Int, "Active", intActive));
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        #endregion
    }
    //
}