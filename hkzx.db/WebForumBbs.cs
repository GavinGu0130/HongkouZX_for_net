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
    public class DataForumBbs : MSBaseData
    {
        public int Id { get; set; }//主键
        public int BoardId { get; set; }//版块Id
        public int BbsTop { get; set; }//置顶状态：0为正常，-1沉底，1区域置顶，10总固顶
        public int TopicId { get; set; }//主题Id：0为主题帖，<0为精华帖
        public string Title { get; set; }//标题
        public string Body { get; set; }//内容
        public string Remark { get; set; }//备注
        public int Active { get; set; }//状态排序：倒序，0发帖需审核，-1锁定，<=-400删除，1发帖直接显示，10审核通过显示
        public int UserId { get; set; }//发帖人Id
        public DateTime AddTime { get; set; }//添加时间：默认为当前时间
        public string AddIp { get; set; }//添加IP:端口号
        public string AddUser { get; set; }//添加人
        public DateTime UpTime { get; set; }//修改时间
        public string UpIp { get; set; }//修改IP:端口号
        public string UpUser { get; set; }//修改人：默认为当前时间
        public int ReadNum { get; set; }//浏览数
        public int ReplyNum { get; set; }//回复数（冗余）
        public DateTime ReplyTime { get; set; }//最后回复时间
        public string ReplyIp { get; set; }//最后回复Ip
        public string ReplyUser { get; set; }//最后回复人
        //分页统计
        public string rowClass { get; set; }//行class属性
        public int num { get; set; }//序号
        public int total { get; set; }//统计数
        public string ActiveName { get; set; }//状态名称
        public int TodayNum { get; set; }//今日帖数
        public int TopicNum { get; set; }//主题帖数
        public int PostNum { get; set; }//总帖数
        public string BtnEdit { get; set; }//编辑按钮文本
        public string UpTimeText { get; set; }//最后修改时间文本
        public string UserPhoto { get; set; }//发帖人头像
        public string UserSex { get; set; }//发帖人性别
        //
        private static string[] columnList = new[] {
            "Id", "BoardId", "BbsTop", "TopicId", "Title", "Body", "Remark", "Active", "UserId", "AddTime", 
            "AddIp", "AddUser", "UpTime", "UpIp", "UpUser", "ReadNum", "ReplyNum", "ReplyTime", "ReplyIp", "ReplyUser", 
            "TodayNum", "TopicNum", "PostNum"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] { SqlDbType.Int, 
            SqlDbType.Int, SqlDbType.Int, SqlDbType.Int, SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.NText, SqlDbType.Int, SqlDbType.Int, SqlDbType.DateTime, 
            SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.Int, SqlDbType.Int, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, 
            SqlDbType.Int, SqlDbType.Int, SqlDbType.Int
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
    public class WebForumBbs
    {
        private const string TableName = "tb_Forum_Bbs";
        SqlDAC sqlDac;
        public WebForumBbs()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }
        //
        #region 查询
        public DataForumBbs[] GetData(int intId, string strFields = "")
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
            DataForumBbs[] result = (DataForumBbs[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataForumBbs));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataForumBbs[] GetDatas(string ActiveName, int BoardId, int TopicId, string Title, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strFromWhere = string.Format("FROM {0} WHERE ", TableName);
            if (!string.IsNullOrEmpty(ActiveName))
            {
                string strTmp = "";
                string[] arr = ActiveName.Split(',');
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
            if (BoardId > 0)
            {
                list.Add(SqlParamHelper.AddParameter("@BoardId", SqlDbType.Int, "BoardId", BoardId));
                strFromWhere += " AND BoardId=@BoardId";
            }
            if (TopicId > 0)
            {//帖子列表
                list.Add(SqlParamHelper.AddParameter("@TopicId", SqlDbType.Int, "TopicId", TopicId));
                strFromWhere += " AND (Id=@TopicId OR TopicId=@TopicId)";
            }
            else
            {//主题帖列表
                strFromWhere += " AND TopicId<=0";
            }
            if (!string.IsNullOrEmpty(Title))
            {
                list.Add(SqlParamHelper.AddParameter("@Title", SqlDbType.NVarChar, "Title", Title));
                strFromWhere += " AND Title=@Title";
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
            DataForumBbs[] result = (DataForumBbs[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataForumBbs));
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
        public DataForumBbs[] GetTodayTopicPost(int Active, int BoardId, string strFields = "")
        {
            if (string.IsNullOrEmpty(strFields))
            {
                strFields = "Today,Topic,Post";
            }
            string strActive = (Active > 0) ? "Active>0" : "1=1";
            string strBoard = (BoardId > 0) ? "AND BoardId=" + BoardId.ToString() + "" : "";
            string strSelect = "";
            if (strFields.IndexOf("Today") >= 0)
            {
                string strToday = string.Format("'{0:yyyy-MM-dd}'", DateTime.Today);//"DATENAME(year,GetDate())+'-'+DATENAME(month,GetDate())+'-'+DATENAME(day,GetDate())";
                strSelect += string.Format(",(SELECT count(Id) FROM {0} WHERE {1} {2} AND AddTime>={3}) AS TodayNum", TableName, strActive, strBoard, strToday);
            }
            if (strFields.IndexOf("Topic") >= 0)
            {
                strSelect += string.Format(",(SELECT count(Id) FROM {0} WHERE {1} {2} AND TopicId<=0) AS TopicNum", TableName, strActive, strBoard);
            }
            if (strFields.IndexOf("Post") >= 0)
            {
                strSelect += string.Format(",(SELECT count(Id) FROM {0} WHERE {1} {2}) AS PostNum", TableName, strActive, strBoard);
            }
            string strSql = string.Format("SELECT {1} FROM {0}", TableName, strSelect.Trim(','));
            DataForumBbs[] result = (DataForumBbs[])sqlDac.GetDataByAnyCondition(strSql, null, typeof(DataForumBbs));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        #endregion
        //
        #region 修改
        //插入
        public int Insert(DataForumBbs data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataForumBbs data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataForumBbs data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.BoardId >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@BoardId", SqlDbType.Int, "BoardId", data.BoardId));
            }
            //if (data.BbsTop > -1000)
            //{
            //    list.Add(SqlParamHelper.AddParameter("@BbsTop", SqlDbType.Int, "BbsTop", data.BbsTop));
            //}
            if (data.TopicId > -1000)
            {
                list.Add(SqlParamHelper.AddParameter("@TopicId", SqlDbType.Int, "TopicId", data.TopicId));
            }
            if (data.Title != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Title", SqlDbType.NVarChar, "Title", data.Title));
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
            if (data.UserId >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@UserId", SqlDbType.Int, "UserId", data.UserId));
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
            //if (data.ReadNum >= 0)
            //{
            //    list.Add(SqlParamHelper.AddParameter("@ReadNum", SqlDbType.Int, "ReadNum", data.ReadNum));
            //}
            //if (data.ReplyNum >= 0)
            //{
            //    list.Add(SqlParamHelper.AddParameter("@ReplyNum", SqlDbType.Int, "ReplyNum", data.ReplyNum));
            //}
            if (data.ReplyTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@ReplyTime", SqlDbType.DateTime, "ReplyTime", data.ReplyTime));
            }
            if (data.ReplyIp != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ReplyIp", SqlDbType.VarChar, "ReplyIp", data.ReplyIp));
            }
            if (data.ReplyUser != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ReplyIp", SqlDbType.NVarChar, "ReplyIp", data.ReplyIp));
            }
            return list.ToArray();
        }
        //增加阅读数
        public int UpdateReadNum(int intId)
        {
            string strSql = string.Format("UPDATE {0} SET ReadNum = ReadNum+1 WHERE Id={1}", TableName, intId);
            return sqlDac.ExecProcedure(strSql, null, false);
        }
        //更新主题的最后回复
        public int UpdateReply(int intId, string strReplyUser, string strReplyIp, DateTime dtReplyTime)
        {
            string strSql = string.Format("UPDATE {0} SET ReplyNum=ReplyNum+1, ReplyTime=@ReplyTime, ReplyIp=@ReplyIp, ReplyUser=@ReplyUser WHERE Id=@Id", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
            list.Add(SqlParamHelper.AddParameter("@ReplyTime", SqlDbType.DateTime, "ReplyTime", dtReplyTime));
            list.Add(SqlParamHelper.AddParameter("@ReplyIp", SqlDbType.VarChar, "ReplyIp", strReplyIp));
            list.Add(SqlParamHelper.AddParameter("@ReplyUser", SqlDbType.NVarChar, "ReplyUser", strReplyUser));
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        //修改状态
        public int UpdateActive(int intId, int intActive, string strActive = "Active")
        {
            string strSql = string.Format("UPDATE {0} SET {1}=@{1} WHERE Id=@Id", TableName, strActive);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
            list.Add(SqlParamHelper.AddParameter("@" + strActive, SqlDbType.Int, strActive, intActive));
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        #endregion
    }
    //
}