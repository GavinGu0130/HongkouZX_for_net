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
    public class DataNews : MSBaseData
    {
        public int Id { get; set; }//主键
        public string SubType { get; set; }//类型：政协要闻，视频新闻
        public string Client { get; set; }//显示终端：PC，手机
        public string Title { get; set; }//标题
        public string LnkUrl { get; set; }//链接地址
        public string PicUrl { get; set; }//图片地址
        public string Video { get; set; }//视频地址
        public string Body { get; set; }//内容提要
        public string Remark { get; set; }//备注
        public int Active { get; set; }//状态：默认0为前台不显示
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
        public string ShowTimeText { get; set; }//发布时间文本
        //
        private static string[] columnList = new[] {
            "Id", "SubType", "Client", "Title", "LnkUrl", "PicUrl", "Video", "Body", "Remark", "Active", 
            "ReadNum", "ShowTime", "AddTime", "AddIp", "AddUser", "UpTime", "UpIp", "UpUser"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] {
            SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.Text, SqlDbType.Text, SqlDbType.Text, SqlDbType.NText, SqlDbType.NText, SqlDbType.Int, 
            SqlDbType.Int, SqlDbType.DateTime, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar
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
    public class WebNews
    {
        private const string TableName = "tb_News";
        SqlDAC sqlDac;
        public WebNews()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }
        //
        #region 查询
        public DataNews[] GetData(int intId, string strFields = "")
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
            DataNews[] result = (DataNews[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataNews));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataNews[] GetDatas(DataNews data, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
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
            if (!string.IsNullOrEmpty(data.Client))
            {
                string strTmp = "";
                string[] arr = data.Client.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "Client" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                        strTmp += "Client=@" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
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
                strOrder += "ShowTime DESC, Active DESC, AddTime DESC";
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
            DataNews[] result = (DataNews[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataNews));
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
        public int Insert(DataNews data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataNews data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataNews data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.SubType != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubType", SqlDbType.NVarChar, "SubType", data.SubType));
            }
            if (data.Client != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Client", SqlDbType.NVarChar, "Client", data.Client));
            }
            if (data.Title != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Title", SqlDbType.NVarChar, "Title", data.Title));
            }
            if (data.LnkUrl != null)
            {
                list.Add(SqlParamHelper.AddParameter("@LnkUrl", SqlDbType.Text, "LnkUrl", data.LnkUrl));
            }
            if (data.PicUrl != null)
            {
                list.Add(SqlParamHelper.AddParameter("@PicUrl", SqlDbType.Text, "PicUrl", data.PicUrl));
            }
            if (data.Video != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Video", SqlDbType.Text, "Video", data.Video));
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
        //增加浏览数
        public int AddReadNum(int intId)
        {
            string strSql = string.Format("UPDATE {0} SET ReadNum=ReadNum+1 WHERE Id=@Id", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        #endregion
        //
    }
}