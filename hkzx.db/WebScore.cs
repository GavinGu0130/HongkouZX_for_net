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
    public class DataScore : MSBaseData
    {
        public int Id { get; set; }//主键
        public string ScoreType { get; set; }//积分类别
        public string ScoreType2 { get; set; }//二级分类
        public string Title { get; set; }//标题
        public decimal Score { get; set; }//分值
        public decimal Score2 { get; set; }//最多分值
        public string Unit { get; set; }//计量单位：半天，次，件，篇，项，年
        public string Remark { get; set; }//备注
        public int Active { get; set; }//状态：默认0为前台不显示
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
        public string ScoreText { get; set; }//分值文本
        public string ActiveName { get; set; }//状态名称
        //
        private static string[] columnList = new[] {
            "Id", "ScoreType", "ScoreType2", "Title", "Score", "Score2", "Unit", "Remark", "Active", "AddTime", 
            "AddIp", "AddUser", "UpTime", "UpIp", "UpUser"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] {
            SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.Decimal, SqlDbType.Decimal, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.Int, SqlDbType.DateTime, 
            SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar
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
    public class WebScore
    {
        private const string TableName = "tb_Score";
        SqlDAC sqlDac;
        public WebScore()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }
        //
        #region 查询
        public DataScore[] GetData(int intId, string strFields = "")
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
            DataScore[] result = (DataScore[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataScore));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataScore[] GetDatas(int Active, string ScoreType, string ScoreType2, string Title, string strFields, int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strFromWhere = string.Format("FROM {0} WHERE ", TableName);
            if (Active > 0)
            {
                strFromWhere += "Active>0";
            }
            else if (Active < 0)
            {
                strFromWhere += "Active<0";
            }
            else
            {
                strFromWhere += "1=1";
            }
            if (!string.IsNullOrEmpty(ScoreType))
            {
                string strTmp = "";
                string[] arr = ScoreType.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "ScoreType" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                        strTmp += "ScoreType=@" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(ScoreType2))
            {
                list.Add(SqlParamHelper.AddParameter("@ScoreType2", SqlDbType.NVarChar, "ScoreType2", ScoreType2));
                strFromWhere += " AND ScoreType2=@ScoreType2";
            }
            if (!string.IsNullOrEmpty(Title))
            {
                list.Add(SqlParamHelper.AddParameter("@Title", SqlDbType.NVarChar, "Title", Title));
                if (Title.IndexOf("%") >= 0)
                {
                    strFromWhere += " AND Title LIKE @Title";
                }
                else
                {
                    strFromWhere += " AND Title=@Title";
                }
            }
            string strOrder = " ORDER BY ";
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                strOrder += strOrderBy;
            }
            else
            {
                strOrder += "Active DESC, ScoreType ASC, AddTime DESC";//, UpTime DESC
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
            DataScore[] result = (DataScore[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataScore));
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
        public int Insert(DataScore data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataScore data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataScore data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.ScoreType != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ScoreType", SqlDbType.NVarChar, "ScoreType", data.ScoreType));
            }
            if (data.ScoreType2 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ScoreType2", SqlDbType.NVarChar, "ScoreType2", data.ScoreType2));
            }
            if (data.Title != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Title", SqlDbType.NVarChar, "Title", data.Title));
            }
            if (data.Score > -1000)
            {
                list.Add(SqlParamHelper.AddParameter("@Score", SqlDbType.Decimal, "Score", data.Score));
            }
            if (data.Score2 > -1000)
            {
                list.Add(SqlParamHelper.AddParameter("@Score2", SqlDbType.Decimal, "Score2", data.Score2));
            }
            if (data.Unit != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Unit", SqlDbType.NVarChar, "Unit", data.Unit));
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
        //
    }
}