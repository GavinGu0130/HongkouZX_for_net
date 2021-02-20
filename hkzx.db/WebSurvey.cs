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
    public class DataSurvey : MSBaseData
    {
        public int Id { get; set; }//主键
        public string SubType { get; set; }//调查类型：问卷，投票，答题（计分）
        public string ToMans { get; set; }//需参与的委员
        public string Title { get; set; }//标题
        public int SurveyNum { get; set; }//参与次数：-1为不限次数，0为只能参与一次，n为每天参与n次
        public DateTime StartTime { get; set; }//开始时间：默认为当前时间
        public DateTime EndTime { get; set; }//结束时间：默认为当前时间
        public int MaxNum { get; set; }//投票选项上限
        public int MinNum { get; set; }//投票选项下限
        public string Body { get; set; }//规则说明
        public string Remark { get; set; }//备注
        public int Active { get; set; }//状态排序：倒序，0暂存，<0不显示
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
        public int OpNum { get; set; }//子选项数
        public int ResultNum { get; set; }//结果数
        public string StartTimeText { get; set; }//开始时间文本
        public string EndTimeText { get; set; }//结束时间文本
        public string SurveyNumText { get; set; }//可参与次数
        //
        private static string[] columnList = new[] {
            "Id", "SubType", "ToMans", "Title", "SurveyNum", "StartTime", "EndTime", "MaxNum", "MinNum", "Body", 
            "Remark", "Active", "AddTime", "AddIp", "AddUser", "UpTime", "UpIp", "UpUser"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] { SqlDbType.Int, 
            SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.NVarChar, SqlDbType.Int, SqlDbType.DateTime, SqlDbType.DateTime, SqlDbType.Int, SqlDbType.Int, SqlDbType.NText, 
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
    public class WebSurvey
    {
        private const string TableName = "tb_Survey";
        SqlDAC sqlDac;
        public WebSurvey()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }
        //
        #region 查询
        public DataSurvey[] GetData(int intId, string strFields = "")
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
            DataSurvey[] result = (DataSurvey[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataSurvey));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataSurvey[] GetDatas(string ActiveName, string ToMans, string Title, DateTime EndTime, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
        {
            DataSurvey data = new DataSurvey();
            data.ActiveName = ActiveName;
            data.ToMans = ToMans;
            data.Title = Title;
            data.EndTimeText = EndTime.ToString("yyyy-MM-dd hh:mm:ss") + ",";
            return GetDatas(data, strFields, intPage, pageSize, strOrderBy, strFilter);
        }
        public DataSurvey[] GetDatas(DataSurvey data, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strFromWhere = string.Format("FROM {0} WHERE ", TableName);
            if (!string.IsNullOrEmpty(data.ActiveName))
            {
                string strTmp = "";
                if (data.ActiveName.IndexOf("|") >= 0)
                {
                    data.ActiveName = data.ActiveName.Replace("|", ",");
                }
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
                            strTmp += "Active=" + arr[i];
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
            if (!string.IsNullOrEmpty(data.SubType))
            {
                string strTmp = "";
                string[] arr = data.SubType.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        if (arr[i].IndexOf("<>") >= 0)
                        {
                            strTmp += "SubType" + arr[i];
                        }
                        else
                        {
                            string tmp = "SubType" + i.ToString();
                            list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                            strTmp += "SubType=@" + tmp;
                        }
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(data.ToMans))
            {
                strFromWhere += " AND (ToMans LIKE ''";
                string[] arr = data.ToMans.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        string tmp = "ToMans" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, "%," + arr[i] + ",%"));
                        strFromWhere += " OR ToMans LIKE @" + tmp;
                    }
                }
                strFromWhere += ")";
            }
            if (!string.IsNullOrEmpty(data.Title))
            {
                list.Add(SqlParamHelper.AddParameter("@Title", SqlDbType.NVarChar, "Title", data.Title));
                if (data.Title.IndexOf("%") >= 0)
                {
                    strFromWhere += " AND Title LIKE @Title";
                }
                else
                {
                    strFromWhere += " AND Title=@Title";
                }
            }
            if (!string.IsNullOrEmpty(data.StartTimeText) && data.StartTimeText.IndexOf(",") >= 0)
            {
                string[] arr = data.StartTimeText.Split(',');
                if (!string.IsNullOrEmpty(arr[0]))
                {
                    list.Add(SqlParamHelper.AddParameter("@StartTime1", SqlDbType.DateTime, "StartTime1", Convert.ToDateTime(arr[0])));
                    strFromWhere += " AND StartTime>=@StartTime1";
                }
                if (!string.IsNullOrEmpty(arr[1]))
                {
                    if (arr[1].IndexOf(":") < 0)
                    {
                        arr[1] += " 23:59:59";
                    }
                    list.Add(SqlParamHelper.AddParameter("@StartTime2", SqlDbType.DateTime, "StartTime2", Convert.ToDateTime(arr[1])));
                    strFromWhere += " AND StartTime<=@StartTime2";
                }
            }
            if (!string.IsNullOrEmpty(data.EndTimeText) && data.EndTimeText.IndexOf(",") >= 0)
            {
                string[] arr = data.EndTimeText.Split(',');
                if (!string.IsNullOrEmpty(arr[0]))
                {
                    list.Add(SqlParamHelper.AddParameter("@EndTime1", SqlDbType.DateTime, "EndTime1", Convert.ToDateTime(arr[0])));
                    strFromWhere += " AND EndTime>=@EndTime1";
                }
                if (!string.IsNullOrEmpty(arr[1]))
                {
                    if (arr[1].IndexOf(":") < 0)
                    {
                        arr[1] += " 23:59:59";
                    }
                    list.Add(SqlParamHelper.AddParameter("@EndTime2", SqlDbType.DateTime, "EndTime2", Convert.ToDateTime(arr[1])));
                    strFromWhere += " AND EndTime<=@EndTime2";
                }
            }
            string strOrder = " ORDER BY ";
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                strOrder += strOrderBy;
            }
            else
            {
                strOrder += "Active DESC, AddTime ASC";
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
            DataSurvey[] result = (DataSurvey[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataSurvey));
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
        public int Insert(DataSurvey data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataSurvey data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataSurvey data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.SubType != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubType", SqlDbType.NVarChar, "SubType", data.SubType));
            }
            if (data.Title != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Title", SqlDbType.NVarChar, "Title", data.Title));
            }
            if (data.ToMans != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ToMans", SqlDbType.NText, "ToMans", data.ToMans));
            }
            if (data.SurveyNum > -1000)
            {
                list.Add(SqlParamHelper.AddParameter("@SurveyNum", SqlDbType.Int, "SurveyNum", data.SurveyNum));
            }
            if (data.StartTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@StartTime", SqlDbType.DateTime, "StartTime", data.StartTime));
            }
            if (data.EndTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@EndTime", SqlDbType.DateTime, "EndTime", data.EndTime));
            }
            if (data.MaxNum >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@MaxNum", SqlDbType.Int, "MaxNum", data.MaxNum));
            }
            if (data.MinNum >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@MinNum", SqlDbType.Int, "MinNum", data.MinNum));
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