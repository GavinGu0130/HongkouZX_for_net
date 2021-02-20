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
    public class DataForumSet : MSBaseData
    {
        public int Id { get; set; }//主键
        public DateTime StartDate { get; set; }//发帖开始日期：默认为当前时间
        public DateTime EndDate { get; set; }//发帖结束日期：默认为当前时间
        public string NoPostDate { get; set; }//禁止发帖日期：2018-12-30(\n)2018-12-31
        public string PostWeek { get; set; }//发帖星期时间：0周日，1周一，2周二，3周三，4周四，5周五，6周六
        public string PostStart { get; set; }//当天开始时间：09:00
        public string PostEnd { get; set; }//当天结束时间：17:00
        public int PostActive { get; set; }//发的帖子默认状态值
        public string Remark { get; set; }//备注
        public int Active { get; set; }//状态：默认0为前台不显示
        public DateTime AddTime { get; set; }//添加时间：默认为当前时间
        public string AddIp { get; set; }//添加IP:端口号
        public string AddUser { get; set; }//添加人
        public DateTime UpTime { get; set; }//修改时间：默认为当前时间
        public string UpIp { get; set; }//修改IP:端口号
        public string UpUser { get; set; }//修改人
        public string Label { get; set; }//标签
        //分页统计
        public string rowClass { get; set; }//行class属性
        public int num { get; set; }//序号
        public int total { get; set; }//统计数
        public string ActiveName { get; set; }//状态名称
        public string PostActiveName { get; set; }//发帖默认状态名称
        //
        private static string[] columnList = new[] {
            "Id", "StartDate", "EndDate", "NoPostDate", "PostWeek", "PostStart", "PostEnd", "PostActive", "Remark", "Active", 
            "AddTime", "AddIp", "AddUser", "UpTime", "UpIp", "UpUser", "Label"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] { SqlDbType.Int, 
            SqlDbType.Int, SqlDbType.DateTime, SqlDbType.DateTime, SqlDbType.Text, SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.Int, SqlDbType.NText, SqlDbType.Int, 
            SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.VarChar
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
    public class WebForumSet
    {
        private const string TableName = "tb_Forum_Set";
        SqlDAC sqlDac;
        public WebForumSet()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }
        //
        #region 查询
        public DataForumSet[] GetData(int intId, string strFields = "")
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
            DataForumSet[] result = (DataForumSet[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataForumSet));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataForumSet[] GetDatas(int Active, string Label, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strFromWhere = string.Format("FROM {0} WHERE ", TableName);
            if (Active > 0)
            {
                strFromWhere += "Active>0";
            }
            else if (Active < 0)
            {
                strFromWhere += "Active<=0";
            }
            else
            {
                strFromWhere += "1=1";
            }
            if (!string.IsNullOrEmpty(Label))
            {
                list.Add(SqlParamHelper.AddParameter("@Label", SqlDbType.VarChar, "Label", Label));
                strFromWhere += " AND Label=@Label";
            }
            string strOrder = " ORDER BY ";
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                strOrder += strOrderBy;
            }
            else
            {
                strOrder += "Active DESC";
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
            DataForumSet[] result = (DataForumSet[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataForumSet));
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
        public int Insert(DataForumSet data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataForumSet data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataForumSet data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.StartDate > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@StartDate", SqlDbType.DateTime, "StartDate", data.StartDate));
            }
            if (data.EndDate > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@EndDate", SqlDbType.DateTime, "EndDate", data.EndDate));
            }
            if (data.NoPostDate != null)
            {
                list.Add(SqlParamHelper.AddParameter("@NoPostDate", SqlDbType.Text, "NoPostDate", data.NoPostDate));
            }
            if (data.PostWeek != null)
            {
                list.Add(SqlParamHelper.AddParameter("@PostWeek", SqlDbType.VarChar, "PostWeek", data.PostWeek));
            }
            if (data.PostStart != null)
            {
                list.Add(SqlParamHelper.AddParameter("@PostStart", SqlDbType.VarChar, "PostStart", data.PostStart));
            }
            if (data.PostEnd != null)
            {
                list.Add(SqlParamHelper.AddParameter("@PostEnd", SqlDbType.VarChar, "PostEnd", data.PostEnd));
            }
            if (data.PostActive > -1000)
            {
                list.Add(SqlParamHelper.AddParameter("@PostActive", SqlDbType.Int, "PostActive", data.PostActive));
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
            if (data.Label != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Label", SqlDbType.VarChar, "Label", data.Label));
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