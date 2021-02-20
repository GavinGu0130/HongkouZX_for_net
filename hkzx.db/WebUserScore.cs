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
    public class DataUserScore : MSBaseData
    {
        public int Id { get; set; }//主键
        public int UserId { get; set; }//用户Id
        public string Title { get; set; }//标题
        public decimal Score { get; set; }//积分
        public string TableName { get; set; }//添加的表名
        public int TableId { get; set; }//添加的表Id
        public string Remark { get; set; }//备注
        public int Active { get; set; }//状态：默认0为前台不显示
        public DateTime AddTime { get; set; }//添加时间：默认为当前时间
        public string AddIp { get; set; }//添加IP:端口号
        public string AddUser { get; set; }//添加人
        public DateTime UpTime { get; set; }//修改时间：默认为当前时间
        public string UpIp { get; set; }//修改IP:端口号
        public string UpUser { get; set; }//修改人
        public DateTime GetTime { get; set; }//得分时间：默认为当前时间
        //分页统计
        public string rowClass { get; set; }//行class属性
        public int num { get; set; }//序号
        public int total { get; set; }//统计数
        public string other { get; set; }//其他说明
        public string BtnEdit { get; set; }//编辑按钮文本
        public string ScoreText { get; set; }//积分文本
        //public string ActiveName { get; set; }//状态名称
        //public string AddTimeText { get; set; }//新增时间文本
        //
        private static string[] columnList = new[] {
            "Id", "UserId", "Title", "Score", "TableName", "TableId", "Remark", "Active", "AddTime", "AddIp", 
            "AddUser", "UpTime", "UpIp", "UpUser", "GetTime"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] {
            SqlDbType.Int, SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.Decimal, SqlDbType.VarChar, SqlDbType.Int, SqlDbType.NText, SqlDbType.Int, SqlDbType.DateTime, SqlDbType.VarChar, 
            SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime
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
    public class WebUserScore
    {
        private const string TableName = "tb_User_Score";
        SqlDAC sqlDac;
        public WebUserScore()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }
        //
        #region 查询
        public DataUserScore[] GetData(int intId, string strFields = "")
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
            DataUserScore[] result = (DataUserScore[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataUserScore));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataUserScore[] GetDatas(int Active, string UserIds, string Table, int TableId, string Title, string GetTimeText, string strFields, int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
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
            if (!string.IsNullOrEmpty(UserIds))
            {
                string strTmp = "";
                string[] arr = UserIds.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "UserId" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.Int, tmp, Convert.ToInt32(arr[i])));
                        strTmp += "UserId=@" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(Table))
            {
                list.Add(SqlParamHelper.AddParameter("@TableName", SqlDbType.VarChar, "TableName", Table));
                strFromWhere += " AND TableName=@TableName";
            }
            if (TableId > 0)
            {
                list.Add(SqlParamHelper.AddParameter("@TableId", SqlDbType.Int, "TableId", TableId));
                strFromWhere += " AND TableId=@TableId";
            }
            if (!string.IsNullOrEmpty(Title))
            {
                string strTmp = "";
                string[] arr = Title.Split('|');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (strTmp != "")
                    {
                        strTmp += " OR ";
                    }
                    string tmp = "Title" + i.ToString();
                    list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                    if (arr[i].IndexOf("%") >= 0)
                    {
                        strTmp += "Title LIKE @" + tmp;
                    }
                    else
                    {
                        strTmp += "Title=@" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(GetTimeText) && GetTimeText != "," && GetTimeText.IndexOf(",") >= 0)
            {
                string[] arr = GetTimeText.Split(',');
                if (!string.IsNullOrEmpty(arr[0]))
                {
                    list.Add(SqlParamHelper.AddParameter("@GetTime1", SqlDbType.DateTime, "GetTime1", Convert.ToDateTime(arr[0])));
                    strFromWhere += " AND GetTime>=@GetTime1";
                }
                if (!string.IsNullOrEmpty(arr[1]))
                {
                    if (arr[1].IndexOf(":") < 0)
                    {
                        arr[1] += " 23:59:59";
                    }
                    list.Add(SqlParamHelper.AddParameter("@GetTime2", SqlDbType.DateTime, "GetTime2", Convert.ToDateTime(arr[1])));
                    strFromWhere += " AND GetTime<=@GetTime2";
                }
            }
            string strOrder = " ORDER BY ";
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                strOrder += strOrderBy;
            }
            else
            {
                strOrder += "UpTime DESC, AddTime DESC";
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
            DataUserScore[] result = (DataUserScore[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataUserScore));
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
        public decimal GetTotalScore(int UserId, string strStart, string strEnd, string strTableName = "")
        {
            decimal deScore = 0;
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@UserId", SqlDbType.Int, "UserId", UserId));
            string strSql = string.Format("SELECT SUM(Score) FROM {0} WHERE UserId=@UserId AND Active>0", TableName);
            if (!string.IsNullOrEmpty(strStart))
            {
                list.Add(SqlParamHelper.AddParameter("@dtStart", SqlDbType.DateTime, "dtStart", Convert.ToDateTime(strStart)));
                strSql += " AND GetTime>=@dtStart";
            }
            if (!string.IsNullOrEmpty(strEnd))
            {
                if (strEnd.IndexOf(":") < 0)
                {
                    strEnd += " 23:59:59";
                }
                list.Add(SqlParamHelper.AddParameter("@dtEnd", SqlDbType.DateTime, "dtEnd", Convert.ToDateTime(strEnd)));
                strSql += " AND GetTime<=@dtEnd";
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
            SqlParameter[] sqlParaArray = list.ToArray();
            string strValue = sqlDac.GetSpecValue(strSql, sqlParaArray);//获取查询总数
            if (!string.IsNullOrEmpty(strValue))
            {
                deScore = Convert.ToDecimal(strValue);
            }
            return deScore;
        }
        #endregion
        //
        #region 修改
        //插入
        public int Insert(DataUserScore data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataUserScore data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataUserScore data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.UserId >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@UserId", SqlDbType.Int, "UserId", data.UserId));
            }
            if (data.Title != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Title", SqlDbType.NVarChar, "Title", data.Title));
            }
            if (data.Score > -1000)
            {
                list.Add(SqlParamHelper.AddParameter("@Score", SqlDbType.Decimal, "Score", data.Score));
            }
            if (data.TableName != null)
            {
                list.Add(SqlParamHelper.AddParameter("@TableName", SqlDbType.VarChar, "TableName", data.TableName));
            }
            if (data.TableId >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@TableId", SqlDbType.Int, "TableId", data.TableId));
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
            if (data.GetTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@GetTime", SqlDbType.DateTime, "GetTime", data.GetTime));
            }
            return list.ToArray();
        }
        //修改得分、标题
        public int UpdateScore(int intId, decimal deScore, string strTitle)
        {
            string strSql = string.Format("UPDATE {0} SET Active=1, Score=@Score, Title=@Title  WHERE Id=@Id", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
            list.Add(SqlParamHelper.AddParameter("@Score", SqlDbType.Decimal, "Score", deScore));
            list.Add(SqlParamHelper.AddParameter("@Title", SqlDbType.NVarChar, "Title", strTitle));
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
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
        public int UpdateActive(ArrayList arrList, int intActive, string strIp = "", string strUser = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strSql = string.Format("UPDATE {0} SET Active=@Active", TableName);
            list.Add(SqlParamHelper.AddParameter("@Active", SqlDbType.Int, "Active", intActive));
            if (!string.IsNullOrEmpty(strIp) || !string.IsNullOrEmpty(strUser))
            {
                list.Add(SqlParamHelper.AddParameter("@UpTime", SqlDbType.DateTime, "UpTime", DateTime.Now));
                list.Add(SqlParamHelper.AddParameter("@UpIp", SqlDbType.VarChar, "UpIp", strIp));
                list.Add(SqlParamHelper.AddParameter("@UpUser", SqlDbType.NVarChar, "UpUser", strUser));
                strSql += ", UpTime=@UpTime, UpIp=@UpIp, UpUser=@UpUser";
            }
            if (arrList.Count > 0)
            {
                for (int i = 0; i < arrList.Count; i++)
                {
                    list.Add(SqlParamHelper.AddParameter("@Id_" + i, SqlDbType.Int, "Id_" + i, Convert.ToInt32(arrList[i])));
                    if (i == 0)
                    {
                        strSql += " WHERE Id = @Id_" + i;
                    }
                    else
                    {
                        strSql += " OR Id = @Id_" + i;
                    }
                }
            }
            else
            {
                return -1;
            }
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        #endregion
        //
    }
}