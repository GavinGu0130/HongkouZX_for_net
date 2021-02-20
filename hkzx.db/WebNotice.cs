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
    public class DataNotice : MSBaseData
    {
        public int Id { get; set; }//主键
        public string SubType { get; set; }//类型：通知，公告
        public string ToMans { get; set; }//需通知的委员
        public DateTime OverTime { get; set; }//失效时间：默认为当前时间
        public string Title { get; set; }//标题
        public string Body { get; set; }//内容
        public string Files { get; set; }//附件：url(\n)url
        public string Remark { get; set; }//备注
        public int Active { get; set; }//状态排序：0暂存，-10取消，<=-400删除，1正常，10重要
        public int ReadNum { get; set; }//浏览数
        public DateTime ShowTime { get; set; }//显示时间：默认为当前时间，倒序
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
        public string OverTimeText { get; set; }//通知时间文本
        public string ShowTimeText { get; set; }//发布时间文本
        public int FeedNum { get; set; }//反馈数
        //
        private static string[] columnList = new[] {
            "Id", "SubType", "ToMans", "OverTime", "Title", "Body", "Files", "Remark", "Active", "ReadNum", 
            "ShowTime", "AddTime", "AddIp", "AddUser", "UpTime", "UpIp", "UpUser"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] {
            SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.DateTime, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.Text, SqlDbType.NText, SqlDbType.Int, SqlDbType.Int, 
            SqlDbType.DateTime, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar
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
    public class WebNotice
    {
        private const string TableName = "tb_Notice";
        SqlDAC sqlDac;
        public WebNotice()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }
        //
        #region 查询
        public DataNotice[] GetData(int intId, string strFields = "")
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
            DataNotice[] result = (DataNotice[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataNotice));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataNotice[] GetDatas(DataNotice data, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
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
                        string tmp = "SubType" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                        strTmp += "SubType=@" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(data.ToMans))
            {
                strFromWhere += " AND (SubType='公告'";
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
            if (!string.IsNullOrEmpty(data.OverTimeText) && data.OverTimeText.IndexOf(",") >= 0)
            {
                string[] arr = data.OverTimeText.Split(',');
                if (!string.IsNullOrEmpty(arr[0]))
                {
                    list.Add(SqlParamHelper.AddParameter("@OverTime1", SqlDbType.DateTime, "OverTime1", Convert.ToDateTime(arr[0])));
                    strFromWhere += " AND OverTime>=@OverTime1";
                }
                if (!string.IsNullOrEmpty(arr[1]))
                {
                    if (arr[1].IndexOf(":") < 0)
                    {
                        arr[1] += " 23:59:59";
                    }
                    list.Add(SqlParamHelper.AddParameter("@OverTime2", SqlDbType.DateTime, "OverTime2", Convert.ToDateTime(arr[1])));
                    strFromWhere += " AND OverTime<=@OverTime2";
                }
            }
            if (!string.IsNullOrEmpty(data.ShowTimeText) && data.ShowTimeText.IndexOf(",") >= 0)
            {
                string[] arr = data.ShowTimeText.Split(',');
                if (!string.IsNullOrEmpty(arr[0]))
                {
                    list.Add(SqlParamHelper.AddParameter("@ShowTime1", SqlDbType.DateTime, "ShowTime1", Convert.ToDateTime(arr[0])));
                    strFromWhere += " AND ShowTime>=@ShowTime1";
                }
                if (!string.IsNullOrEmpty(arr[1]))
                {
                    if (arr[1].IndexOf(":") < 0)
                    {
                        arr[1] += " 23:59:59";
                    }
                    list.Add(SqlParamHelper.AddParameter("@ShowTime2", SqlDbType.DateTime, "ShowTime2", Convert.ToDateTime(arr[1])));
                    strFromWhere += " AND ShowTime<=@ShowTime2";
                }
            }
            string strOrder = " ORDER BY ";
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                strOrder += strOrderBy;
            }
            else
            {
                strOrder += "Active DESC, OverTime ASC, ShowTime ASC, AddTime DESC";
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
                    pageSize = 10000;//考虑数据库性能，只读取前10,000条数据
                }
                strSql = "SELECT TOP " + pageSize.ToString() + strFields + strFromWhere + strOrder;
            }
            SqlParameter[] sqlParaArray = list.ToArray();
            DataNotice[] result = (DataNotice[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataNotice));
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
        public int Insert(DataNotice data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataNotice data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataNotice data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.SubType != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubType", SqlDbType.NVarChar, "SubType", data.SubType));
            }
            if (data.ToMans != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ToMans", SqlDbType.NText, "ToMans", data.ToMans));
            }
            if (data.OverTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@OverTime", SqlDbType.DateTime, "OverTime", data.OverTime));
            }
            if (data.Title != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Title", SqlDbType.NVarChar, "Title", data.Title));
            }
            if (data.Body != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Body", SqlDbType.NText, "Body", data.Body));
            }
            if (data.Files != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Files", SqlDbType.Text, "Files", data.Files));
            }
            if (data.Remark != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Remark", SqlDbType.NText, "Remark", data.Remark));
            }
            if (data.Active > -1000)
            {
                list.Add(SqlParamHelper.AddParameter("@Active", SqlDbType.Int, "Active", data.Active));
            }
            //if (data.ReadNum > -1000)
            //{
            //    list.Add(SqlParamHelper.AddParameter("@ReadNum", SqlDbType.Int, "ReadNum", data.ReadNum));
            //}
            if (data.ShowTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@ShowTime", SqlDbType.DateTime, "ShowTime", data.ShowTime));
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
        //增加浏览数
        public int AddReadNum(int intId)
        {
            string strSql = string.Format("UPDATE {0} SET ReadNum=ReadNum+1 WHERE Id=@Id", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
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
        public int UpdateActive(ArrayList arrList, int intActive, string VerifyInfo = "", string strIp = "", string strUser = "")
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
            //if (!string.IsNullOrEmpty(VerifyInfo))
            //{
            //    list.Add(SqlParamHelper.AddParameter("@VerifyInfo", SqlDbType.NText, "VerifyInfo", VerifyInfo));
            //    strSql += ", VerifyInfo=@VerifyInfo";
            //}
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
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        #endregion
        //
    }
}