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
    public class DataPerformFeed : MSBaseData
    {
        public int Id { get; set; }//主键
        public int PerformId { get; set; }//履职活动Id
        public int UserId { get; set; }//用户Id
        public string SignMan { get; set; }//履职人员
        public string SignManType { get; set; }//人员类型：常委，调研执笔人，讲堂主讲人
        public string SignManSpeak { get; set; }//大会发言：上台交流，书面交流
        public string SignManProvide { get; set; }//是否提供资源：提供资源
        public DateTime SignTime { get; set; }//签到时间
        public string SignIp { get; set; }//签到IP:端口号
        public string SignDesk { get; set; }//签到人/设备
        public int LeaveNum { get; set; }//请假次数：最多申请2次
        public string LeaveReason { get; set; }//请假事由
        public DateTime LeaveTime { get; set; }//请假时间
        public string LeaveIp { get; set; }//请假IP:端口号
        public string VerifyInfo { get; set; }//审批意见：同意，不同意
        public string Remark { get; set; }//备注
        public string IsMust { get; set; }//参加方式：必须参加
        public string ActiveName { get; set; }//状态：参加，请假申请，不同意请假（必须参加扣2，其他扣1），同意请假（必须参加扣1），不参加（不扣分），已签到，已出席（加分）
        public DateTime AddTime { get; set; }//添加时间：默认为当前时间
        public string AddIp { get; set; }//添加IP:端口号
        public string AddUser { get; set; }//添加人
        public DateTime UpTime { get; set; }//修改时间：默认为当前时间
        public string UpIp { get; set; }//修改IP:端口号
        public string UpUser { get; set; }//修改人
        public DateTime VerifyTime { get; set; }//审核时间
        public string VerifyIp { get; set; }//审核IP:端口号
        public string VerifyUser { get; set; }//审核人
        public string SendMsg { get; set; }//发送的消息
        //聚合
        public int PlatformNum { get; set; }//上台交流次数
        public int WriteNum { get; set; }//书面交流次数
        public int SpeakNum { get; set; }//其他会议发言
        //分页统计
        public string rowClass { get; set; }//行class属性
        public int num { get; set; }//序号
        public int total { get; set; }//统计数
        public string SignTimeText { get; set; }//签到时间文本
        public string FeedTimeText { get; set; }//反馈时间文本
        public string VerifyTimeText { get; set; }//审核时间文本
        public string PerformSubType { get; set; }//会议/活动类型
        public string PerformTitle { get; set; }//会议/活动名称
        public string SignManCode { get; set; }//履职人员编号
        public string SignManOrg { get; set; }//履职人员单位
        //
        private static string[] columnList = new[] {
            "Id", "PerformId", "UserId", "SignMan", "SignManType", "SignManSpeak", "SignManProvide", "SignTime", "SignIp", "SignDesk", 
            "LeaveNum", "LeaveReason", "LeaveTime", "LeaveIp", "VerifyInfo", "Remark", "IsMust", "ActiveName", "AddTime", "AddIp", 
            "AddUser", "UpTime", "UpIp", "UpUser", "VerifyTime", "VerifyIp", "VerifyUser"
            , "PlatformNum", "WriteNum", "SpeakNum"
            , "Committee", "Committee2"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] { SqlDbType.Int, 
            SqlDbType.Int, SqlDbType.Int, SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, 
            SqlDbType.Int, SqlDbType.NText, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, 
            SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar
            , SqlDbType.Int, SqlDbType.Int, SqlDbType.Int
            , SqlDbType.NVarChar, SqlDbType.NVarChar
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
    public class WebPerformFeed
    {
        private const string TableName = "tb_Perform_Feed";
        SqlDAC sqlDac;
        public WebPerformFeed()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }
        //
        #region 查询
        public DataPerformFeed[] GetData(int intId, string strFields = "")
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
            DataPerformFeed[] result = (DataPerformFeed[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataPerformFeed));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataPerformFeed[] GetDatas(string ActiveName, int PerformId, int UserId, string SignMan, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
        {
            DataPerformFeed data = new DataPerformFeed();
            data.ActiveName = ActiveName;
            data.PerformId = PerformId;
            data.UserId = UserId;
            data.SignMan = SignMan;
            return GetDatas(data, strFields, intPage, pageSize, strOrderBy, strFilter);
        }
        public DataPerformFeed[] GetDatas(DataPerformFeed data, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
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
                        if (arr[i].IndexOf("<>") >= 0)
                        {
                            strTmp += "ActiveName" + arr[i];
                        }
                        else
                        {
                            string tmp = "ActiveName" + i.ToString();
                            list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                            strTmp += "ActiveName=@" + tmp;
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
            if (data.PerformId > 0)
            {
                list.Add(SqlParamHelper.AddParameter("@PerformId", SqlDbType.Int, "PerformId", data.PerformId));
                strFromWhere += " AND PerformId=@PerformId";
            }
            if (data.UserId > 0)
            {
                list.Add(SqlParamHelper.AddParameter("@UserId", SqlDbType.Int, "UserId", data.UserId));
                strFromWhere += " AND UserId=@UserId";
            }
            if (!string.IsNullOrEmpty(data.SignMan))
            {
                string strTmp = "";
                string[] arr = data.SignMan.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "SignMan" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                        strTmp += "SignMan=@" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(data.SignTimeText) && data.SignTimeText.IndexOf(",") >= 0)
            {
                string[] arr = data.SignTimeText.Split(',');
                if (!string.IsNullOrEmpty(arr[0]))
                {
                    list.Add(SqlParamHelper.AddParameter("@SignTime1", SqlDbType.DateTime, "SignTime1", Convert.ToDateTime(arr[0])));
                    strFromWhere += " AND SignTime>=@SignTime1";
                }
                if (!string.IsNullOrEmpty(arr[1]))
                {
                    list.Add(SqlParamHelper.AddParameter("@SignTime2", SqlDbType.DateTime, "SignTime2", Convert.ToDateTime(arr[1])));
                    strFromWhere += " AND SignTime<=@SignTime2";
                }
            }
            string strOrder = " ORDER BY ";
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                strOrder += strOrderBy;
            }
            else
            {
                strOrder += "SignTime DESC, UpTime DESC, AddTime DESC";
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
            DataPerformFeed[] result = (DataPerformFeed[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataPerformFeed));
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
        //获取大会发言
        public DataPerformFeed[] GetSpeaks(DataPerformFeed data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strWhere = "";
            if (!string.IsNullOrEmpty(data.ActiveName))
            {
                list.Add(SqlParamHelper.AddParameter("@ActiveName", SqlDbType.NVarChar, "ActiveName", data.ActiveName));
                strWhere += "AND ActiveName=@ActiveName";
            }
            if (!string.IsNullOrEmpty(data.SignTimeText) && data.SignTimeText.IndexOf(",") >= 0)
            {
                string[] arr = data.SignTimeText.Split(',');
                if (!string.IsNullOrEmpty(arr[0]))
                {
                    list.Add(SqlParamHelper.AddParameter("@SignTime1", SqlDbType.DateTime, "SignTime1", Convert.ToDateTime(arr[0])));
                    strWhere += " AND SignTime>=@SignTime1";
                }
                if (!string.IsNullOrEmpty(arr[1]))
                {
                    list.Add(SqlParamHelper.AddParameter("@SignTime2", SqlDbType.DateTime, "SignTime2", Convert.ToDateTime(arr[1])));
                    strWhere += " AND SignTime<=@SignTime2";
                }
            }
            SqlParameter[] sqlParaArray = list.ToArray();
            string strSql = string.Format("SELECT UserId, SignMan, (SELECT COUNT(Id) FROM {0} AS s WHERE s.UserId=f.UserId AND SignManSpeak LIKE '%上台交流%') AS PlatformNum, (SELECT COUNT(Id) FROM {0} AS s WHERE s.UserId=f.UserId AND SignManSpeak LIKE '%书面交流%') AS WriteNum, (SELECT COUNT(Id) FROM {0} AS s WHERE s.UserId=f.UserId AND SignManSpeak LIKE '其他会议发言') AS SpeakNum FROM {0} AS f WHERE SignManSpeak LIKE '%发言%' {1} GROUP BY SignMan, UserId ORDER BY SignMan ASC", TableName, strWhere);
            DataPerformFeed[] result = (DataPerformFeed[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataPerformFeed));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataPerformFeed[] GetSpeaks(string Committee, string Subsector, string SignTimeText, string strFields = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            if (string.IsNullOrEmpty(strFields))
            {
                strFields = "f.PerformId, f.UserId, f.SignMan, f.SignManSpeak";//f.UserId
            }
            string strSql = string.Format("SELECT {1} FROM {0} AS f LEFT JOIN [tb_User] AS u ON (u.Id=f.UserId) WHERE f.SignManSpeak LIKE '%发言%' AND (f.ActiveName='已签到' OR f.ActiveName='已出席')", TableName, strFields);
            if (!string.IsNullOrEmpty(Committee))
            {
                list.Add(SqlParamHelper.AddParameter("@Committee", SqlDbType.NVarChar, "Committee", Committee));
                strSql += " AND (u.Committee=@Committee OR u.Committee2=@Committee)";
            }
            if (!string.IsNullOrEmpty(SignTimeText) && SignTimeText.IndexOf(",") >= 0)
            {
                string[] arr = SignTimeText.Split(',');
                if (!string.IsNullOrEmpty(arr[0]))
                {
                    list.Add(SqlParamHelper.AddParameter("@SignTime1", SqlDbType.DateTime, "SignTime1", Convert.ToDateTime(arr[0])));
                    strSql += " AND f.SignTime>=@SignTime1";
                }
                if (!string.IsNullOrEmpty(arr[1]))
                {
                    list.Add(SqlParamHelper.AddParameter("@SignTime2", SqlDbType.DateTime, "SignTime2", Convert.ToDateTime(arr[1])));
                    strSql += " AND f.SignTime<=@SignTime2";
                }
            }
            if (!string.IsNullOrEmpty(Subsector))
            {
                string strTmp = "";
                string[] arr = Subsector.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "Subsector" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                        strTmp += "u.Subsector=@" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strSql += " AND (" + strTmp + ")";
                }
                //list.Add(SqlParamHelper.AddParameter("@Subsector", SqlDbType.NVarChar, "Subsector", Subsector));
                //strSql += " AND (u.Subsector=@Subsector OR u.Subsector2=@Subsector)";
            }
            //HttpContext.Current.Response.Write(strSql); HttpContext.Current.Response.End();
            SqlParameter[] sqlParaArray = list.ToArray();
            DataPerformFeed[] result = (DataPerformFeed[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataPerformFeed));
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
        public int Insert(DataPerformFeed data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataPerformFeed data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataPerformFeed data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.PerformId >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@PerformId", SqlDbType.Int, "PerformId", data.PerformId));
            }
            if (data.UserId >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@UserId", SqlDbType.Int, "UserId", data.UserId));
            }
            if (data.SignMan != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SignMan", SqlDbType.NVarChar, "SignMan", data.SignMan));
            }
            if (data.SignManType != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SignManType", SqlDbType.NVarChar, "SignManType", data.SignManType));
            }
            if (data.SignManSpeak != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SignManSpeak", SqlDbType.NVarChar, "SignManSpeak", data.SignManSpeak));
            }
            if (data.SignManProvide != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SignManProvide", SqlDbType.NVarChar, "SignManProvide", data.SignManProvide));
            }
            if (data.SignTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@SignTime", SqlDbType.DateTime, "SignTime", data.SignTime));
            }
            if (data.SignIp != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SignIp", SqlDbType.VarChar, "SignIp", data.SignIp));
            }
            if (data.SignDesk != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SignDesk", SqlDbType.NVarChar, "SignDesk", data.SignDesk));
            }
            if (data.LeaveNum >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@LeaveNum", SqlDbType.Int, "LeaveNum", data.LeaveNum));
            }
            if (data.LeaveReason != null)
            {
                list.Add(SqlParamHelper.AddParameter("@LeaveReason", SqlDbType.NText, "LeaveReason", data.LeaveReason));
            }
            if (data.LeaveTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@LeaveTime", SqlDbType.DateTime, "LeaveTime", data.LeaveTime));
            }
            if (data.LeaveIp != null)
            {
                list.Add(SqlParamHelper.AddParameter("@LeaveIp", SqlDbType.VarChar, "LeaveIp", data.LeaveIp));
            }
            if (data.VerifyInfo != null)
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyInfo", SqlDbType.NText, "VerifyInfo", data.VerifyInfo));
            }
            if (data.Remark != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Remark", SqlDbType.NText, "Remark", data.Remark));
            }
            if (data.IsMust != null)
            {
                list.Add(SqlParamHelper.AddParameter("@IsMust", SqlDbType.NVarChar, "IsMust", data.IsMust));
            }
            if (data.ActiveName != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ActiveName", SqlDbType.NVarChar, "ActiveName", data.ActiveName));
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
            if (data.VerifyTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyTime", SqlDbType.DateTime, "VerifyTime", data.VerifyTime));
            }
            //else
            //{
            //    list.Add(SqlParamHelper.AddParameter("@VerifyTime", SqlDbType.VarChar, "VerifyTime", null));
            //}
            if (data.VerifyIp != null)
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyIp", SqlDbType.VarChar, "VerifyIp", data.VerifyIp));
            }
            if (data.VerifyUser != null)
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyUser", SqlDbType.NVarChar, "VerifyUser", data.VerifyUser));
            }
            return list.ToArray();
        }
        //修改状态
        public int UpdateSign(int intId, string ActiveName, DateTime dtSignTime, string strIp = "", string strDesk = "")
        {
            string strSql = string.Format("UPDATE {0} SET ActiveName=@ActiveName", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@ActiveName", SqlDbType.NVarChar, "ActiveName", ActiveName));
            if (!string.IsNullOrEmpty(strIp) || !string.IsNullOrEmpty(strDesk))
            {
                if (dtSignTime <= DateTime.MinValue)
                {
                    dtSignTime = DateTime.Now;
                }
                list.Add(SqlParamHelper.AddParameter("@SignTime", SqlDbType.DateTime, "SignTime", dtSignTime));
                list.Add(SqlParamHelper.AddParameter("@SignIp", SqlDbType.VarChar, "SignIp", strIp));
                list.Add(SqlParamHelper.AddParameter("@SignDesk", SqlDbType.NVarChar, "SignDesk", strDesk));
                strSql += ", SignTime=@SignTime, SignIp=@SignIp, SignDesk=@SignDesk";
            }
            strSql += " WHERE Id=@Id";
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }

        public int UpdateActive(ArrayList arrList, string ActiveName, string VerifyInfo = "", string strIp = "", string strUser = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strSql = string.Format("UPDATE {0} SET ", TableName);
            switch (ActiveName)
            {
                case "人员类型":
                    list.Add(SqlParamHelper.AddParameter("@SignManType", SqlDbType.NVarChar, "SignManType", VerifyInfo));
                    strSql += "SignManType=@SignManType";
                    break;
                case "会议发言":
                    list.Add(SqlParamHelper.AddParameter("@SignManSpeak", SqlDbType.NVarChar, "SignManSpeak", VerifyInfo));
                    strSql += "SignManSpeak=@SignManSpeak";
                    break;
                case "提供资源":
                    list.Add(SqlParamHelper.AddParameter("@SignManProvide", SqlDbType.NVarChar, "SignManProvide", VerifyInfo));
                    strSql += "SignManProvide=@SignManProvide";
                    break;
                default:
                    list.Add(SqlParamHelper.AddParameter("@ActiveName", SqlDbType.NVarChar, "ActiveName", ActiveName));
                    strSql += "ActiveName=@ActiveName";
                    if (ActiveName == "已签到")
                    {
                        list.Add(SqlParamHelper.AddParameter("@SignTime", SqlDbType.DateTime, "SignTime", DateTime.Now));
                        strSql += ", SignTime=@SignTime";
                    }
                    if (!string.IsNullOrEmpty(VerifyInfo))
                    {
                        list.Add(SqlParamHelper.AddParameter("@VerifyInfo", SqlDbType.NText, "VerifyInfo", VerifyInfo));
                        strSql += ", VerifyInfo=@VerifyInfo";
                    }
                    break;
            }
            if (!string.IsNullOrEmpty(strIp) || !string.IsNullOrEmpty(strUser))
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyTime", SqlDbType.DateTime, "VerifyTime", DateTime.Now));
                list.Add(SqlParamHelper.AddParameter("@VerifyIp", SqlDbType.VarChar, "VerifyIp", strIp));
                list.Add(SqlParamHelper.AddParameter("@VerifyUser", SqlDbType.NVarChar, "VerifyUser", strUser));
                strSql += ", VerifyTime=@VerifyTime, VerifyIp=@VerifyIp, VerifyUser=@VerifyUser";
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
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        #endregion
    }
    //
}