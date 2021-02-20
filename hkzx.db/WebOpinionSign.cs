using System;
using System.Collections;
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
    public class DataOpinionSign : MSBaseData
    {
        public int Id { get; set; }//主键
        public string OpType { get; set; }//分类：提案，社情民意
        public int OpId { get; set; }//提案、社情民意Id
        public int UserId { get; set; }//用户Id
        public string SignUser { get; set; }//会签人
        public DateTime Overdue { get; set; }//过期时间：默认为当前时间
        public DateTime SignTime { get; set; }//会签时间
        public string SignIp { get; set; }//会签IP:端口号
        public string SignMark { get; set; }//时间标识：会间，会前，会后
        public string Body { get; set; }//签名意见
        public string Remark { get; set; }//备注
        public int Active { get; set; }//状态排序：0待签名，<=-400删除，1已签名
        public DateTime AddTime { get; set; }//添加时间：默认为当前时间
        public string AddIp { get; set; }//添加IP:端口号
        public string AddUser { get; set; }//添加人
        public DateTime UpTime { get; set; }//修改时间：默认为当前时间
        public string UpIp { get; set; }//修改IP:端口号
        public string UpUser { get; set; }//修改人
        //分页统计
        public string rowClass { get; set; }//行class属性
        public int num { get; set; }//序号
        public int total { get; set; }//统计数
        public string ActiveName { get; set; }//状态名称
        public string SignTimeText { get; set; }//会签时间文本
        public string OverdueText { get; set; }//过期时间文本
        //
        private static string[] columnList = new[] {
            "Id", "OpType", "OpId", "UserId", "SignUser", "Overdue", "SignTime", "SignIp", "SignMark", "Body", 
            "Remark", "Active", "AddTime", "AddIp", "AddUser", "UpTime", "UpIp", "UpUser"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] {
            SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.Int, SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.NText, 
            SqlDbType.NText, SqlDbType.Int, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar
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
    public class WebOpinionSign
    {
        private const string TableName = "tb_Opinion_Sign";
        SqlDAC sqlDac;
        public WebOpinionSign()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }
        //
        #region 查询
        public DataOpinionSign[] GetData(int intId, string strFields = "")
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
            DataOpinionSign[] result = (DataOpinionSign[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataOpinionSign));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataOpinionSign[] GetDatas(DataOpinionSign data, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strFromWhere = string.Format("FROM {0} WHERE ", TableName);
            if (!string.IsNullOrEmpty(data.ActiveName))
            {
                string strTmp = "";
                string[] arr = data.ActiveName.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        if (arr[i].IndexOf("<") >= 0 || arr[i].IndexOf(">") >= 0)
                        {
                            strTmp += "Active" + arr[i];
                        }
                        else
                        {
                            string tmp = "Active" + i.ToString();
                            list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                            strTmp += "Active=@" + tmp;
                        }
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += "(" + strTmp + ")";
                }
            }
            else
            {
                strFromWhere += "1=1";
            }
            if (!string.IsNullOrEmpty(data.OpType))
            {
                list.Add(SqlParamHelper.AddParameter("@OpType", SqlDbType.NVarChar, "OpType", data.OpType));
                strFromWhere += " AND OpType=@OpType";
            }
            if (data.OpId > 0)
            {
                list.Add(SqlParamHelper.AddParameter("@OpId", SqlDbType.Int, "OpId", data.OpId));
                strFromWhere += " AND OpId=@OpId";
            }
            if (data.UserId > 0)
            {
                list.Add(SqlParamHelper.AddParameter("@UserId", SqlDbType.Int, "UserId", data.UserId));
                strFromWhere += " AND UserId=@UserId";
            }
            if (data.SignUser != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SignUser", SqlDbType.NVarChar, "SignUser", data.SignUser));
                strFromWhere += " AND SignUser=@SignUser";
            }
            if (!string.IsNullOrEmpty(data.OverdueText) && data.OverdueText.IndexOf(",") >= 0)
            {
                string[] arr = data.OverdueText.Split(',');
                if (!string.IsNullOrEmpty(arr[0]))
                {
                    list.Add(SqlParamHelper.AddParameter("@Overdue1", SqlDbType.DateTime, "Overdue1", Convert.ToDateTime(arr[0])));
                    strFromWhere += " AND Overdue>=@Overdue1";
                }
                if (!string.IsNullOrEmpty(arr[1]))
                {
                    list.Add(SqlParamHelper.AddParameter("@Overdue2", SqlDbType.DateTime, "Overdue2", Convert.ToDateTime(arr[1])));
                    strFromWhere += " AND Overdue<=@Overdue2";
                }
            }
            string strOrder = " ORDER BY ";
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                strOrder += strOrderBy;
            }
            else
            {
                strOrder += "Overdue DESC, AddTime DESC";
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
            DataOpinionSign[] result = (DataOpinionSign[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataOpinionSign));
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
        #endregion
        //
        #region 修改
        //插入
        public int Insert(DataOpinionSign data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataOpinionSign data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataOpinionSign data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.OpType != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OpType", SqlDbType.NVarChar, "OpType", data.OpType));
            }
            if (data.OpId >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@OpId", SqlDbType.Int, "OpId", data.OpId));
            }
            if (data.UserId >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@UserId", SqlDbType.Int, "UserId", data.UserId));
            }
            if (data.SignUser != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SignUser", SqlDbType.NVarChar, "SignUser", data.SignUser));
            }
            if (data.Overdue > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@Overdue", SqlDbType.DateTime, "Overdue", data.Overdue));
            }
            if (data.SignTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@SignTime", SqlDbType.DateTime, "SignTime", data.SignTime));
            }
            else
            {
                list.Add(SqlParamHelper.AddParameter("@SignTime", SqlDbType.VarChar, "SignTime", null));
            }
            if (data.SignIp != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SignIp", SqlDbType.VarChar, "SignIp", data.SignIp));
            }
            if (data.SignMark != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SignMark", SqlDbType.NVarChar, "SignMark", data.SignMark));
            }
            if (data.Body != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Body", SqlDbType.NText, "Body", data.Body));
            }
            if (data.Remark != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Remark", SqlDbType.NText, "Remark", data.Remark));
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
            if (data.UpTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@UpTime", SqlDbType.DateTime, "UpTime", data.UpTime));
            }
            if (data.UpIp != null)
            {
                list.Add(SqlParamHelper.AddParameter("@UpIp", SqlDbType.VarChar, "UpIp", data.UpIp));
            }
            if (data.UpUser != null)
            {
                list.Add(SqlParamHelper.AddParameter("@UpUser", SqlDbType.NVarChar, "UpUser", data.UpUser));
            }
            return list.ToArray();
        }
        //更新签名状态
        public int UpdateActive(int intId, int Active)
        {
            string strSql = string.Format("UPDATE {0} SET Active=@Active WHERE Id=@Id", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
            list.Add(SqlParamHelper.AddParameter("@Active", SqlDbType.Int, "Active", Active));
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        //更新签名结束时间
        public int UpdateOverdue(int intId, DateTime Overdue, int Active = -1000)
        {
            string strSql = string.Format("UPDATE {0} SET Overdue=@Overdue", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
            list.Add(SqlParamHelper.AddParameter("@Overdue", SqlDbType.DateTime, "Overdue", Overdue));
            if (Active > -1000)
            {
                list.Add(SqlParamHelper.AddParameter("@Active", SqlDbType.Int, "Active", Active));
                strSql += ", Active=@Active";
            }
            strSql += " WHERE Id=@Id";
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        #endregion
    }
    //
}