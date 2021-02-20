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
    public class DataPerform : MSBaseData
    {
        public int Id { get; set; }//主键
        public string OrgName { get; set; }//机构名称：政治面貌，专委会，界别活动组，街道活动组
        public string Linkman { get; set; }//联系人
        public string LinkmanTel { get; set; }//联系电话（手机）
        public string IsMust { get; set; }//参加方式：必须参加，报名参加
        public string SubType { get; set; }//类型：根据积分规则表中会议、活动
        public string Title { get; set; }//名称
        public DateTime OverTime { get; set; }//截止时间：默认为当前时间
        public DateTime StartTime { get; set; }//开始时间：默认为当前时间
        public DateTime EndTime { get; set; }//结束时间：默认为当前时间
        public string PerformSite { get; set; }//地点
        public string Body { get; set; }//内容说明
        public string Files { get; set; }//附件：url(\n)url
        public string Leaders { get; set; }//出席领导：,领导,领导,
        public string Attendees { get; set; }//参加委员：,委员,委员,
        public string HaveBus { get; set; }//用车情况：车队派车，外租车辆，否
        public string HaveDinner { get; set; }//其他：用餐，茶歇
        public string Remark { get; set; }//备注
        public string ActiveName { get; set; }//状态：删除，暂存，提交申请，退回，发布，履职关闭
        public string VerifyInfo { get; set; }//审核意见
        public DateTime AddTime { get; set; }//添加时间：默认为当前时间
        public string AddIp { get; set; }//添加IP:端口号
        public string AddUser { get; set; }//添加人
        public DateTime UpTime { get; set; }//修改时间：默认为当前时间
        public string UpIp { get; set; }//修改IP:端口号
        public string UpUser { get; set; }//修改人
        public DateTime FinishTime { get; set; }//履职关闭时间
        public string FinishIp { get; set; }//履职关闭IP:端口号
        public string FinishUser { get; set; }//履职关闭人
        public string SignDesk { get; set; }//签到设备：,01,02,03,
        public DateTime SignTime { get; set; }//签到开始时间
        //分页统计
        public string rowClass { get; set; }//行class属性
        public int num { get; set; }//序号
        public int total { get; set; }//统计数
        public string AdminName { get; set; }//管理员名
        public string StateName { get; set; }//操作名称
        public string OverTimeText { get; set; }//截止时间文本
        public string StartTimeText { get; set; }//开始时间文本
        public string EndTimeText { get; set; }//结束时间文本
        public string PerformTimeText { get; set; }//活动时间文本
        public string SignTimeText { get; set; }//签到开始时间文本
        public int FeedNum { get; set; }//反馈数
        public int AttendeesNum { get; set; }//参与人数
        public string FeedActiveName { get; set; }//出席情况
        //
        private static string[] columnList = new[] {
            "Id", "OrgName", "Linkman", "LinkmanTel", "IsMust", "SubType", "Title", "OverTime", "StartTime", "EndTime", 
            "PerformSite", "Body", "Files", "Leaders", "Attendees", "HaveBus", "HaveDinner", "Remark", "ActiveName", "VerifyInfo", 
            "AddTime", "AddIp", "AddUser", "UpTime", "UpIp", "UpUser", "FinishTime", "FinishIp", "FinishUser", "SignDesk", 
            "SignTime"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] { SqlDbType.Int, 
            SqlDbType.Int, SqlDbType.NText, SqlDbType.NVarChar, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.DateTime, SqlDbType.DateTime, 
            SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.Text, SqlDbType.NText, SqlDbType.NText, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.NVarChar, SqlDbType.NText, 
            SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.Text, 
            SqlDbType.DateTime
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
    public class WebPerform
    {
        private const string TableName = "tb_Perform";
        SqlDAC sqlDac;
        public WebPerform()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }
        //
        #region 查询
        public DataPerform[] GetData(int intId, string strFields = "")
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
            DataPerform[] result = (DataPerform[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataPerform));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataPerform[] GetDatas(DataPerform data, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
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
                            if (arr[i] == "暂存" && !string.IsNullOrEmpty(data.AdminName))
                            {
                                strTmp += "(ActiveName=@" + tmp + " AND AddUser='" + data.AdminName + "')";
                            }
                            else
                            {
                                strTmp += "ActiveName=@" + tmp;
                            }
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
            if (!string.IsNullOrEmpty(data.OrgName))
            {
                if (data.OrgName.IndexOf("-") > 0)
                {
                    string strType = data.OrgName.Substring(0, data.OrgName.IndexOf("-"));
                    string strName = data.OrgName.Substring(data.OrgName.IndexOf("-"));
                    list.Add(SqlParamHelper.AddParameter("@OrgType", SqlDbType.NVarChar, "OrgType", "%" + strType + "%"));
                    list.Add(SqlParamHelper.AddParameter("@OrgName", SqlDbType.NVarChar, "OrgName", "%" + strName + "%"));
                    strFromWhere += " AND OrgName LIKE @OrgType AND OrgName LIKE @OrgName";
                }
                else
                {
                    list.Add(SqlParamHelper.AddParameter("@OrgName", SqlDbType.NVarChar, "OrgName", "%" + data.OrgName + "%"));
                    strFromWhere += " AND OrgName LIKE @OrgName";
                }
            }
            if (!string.IsNullOrEmpty(data.SubType))
            {
                if (data.SubType.IndexOf("-") > 0)
                {
                    string strType = data.SubType.Substring(0, data.SubType.IndexOf("-"));
                    string strName = data.SubType.Substring(data.SubType.IndexOf("-"));
                    list.Add(SqlParamHelper.AddParameter("@SubType", SqlDbType.NVarChar, "SubType", "%" + strType + "%"));
                    list.Add(SqlParamHelper.AddParameter("@SubName", SqlDbType.NVarChar, "SubName", "%" + strName + "%"));
                    strFromWhere += " AND SubType LIKE @SubType AND SubType LIKE @SubName";
                }
                else
                {
                    list.Add(SqlParamHelper.AddParameter("@SubType", SqlDbType.NVarChar, "SubType", "%" + data.SubType + "%"));
                    strFromWhere += " AND SubType LIKE @SubType";
                }
            }
            if (!string.IsNullOrEmpty(data.IsMust))
            {
                list.Add(SqlParamHelper.AddParameter("@IsMust", SqlDbType.NVarChar, "IsMust", data.IsMust));
                strFromWhere += " AND IsMust=@IsMust";
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
            if (!string.IsNullOrEmpty(data.PerformSite))
            {
                list.Add(SqlParamHelper.AddParameter("@PerformSite", SqlDbType.NVarChar, "PerformSite", data.PerformSite));
                strFromWhere += " AND PerformSite LIKE @PerformSite";
            }
            if (!string.IsNullOrEmpty(data.Leaders))
            {
                string strTmp = "";
                string[] arr = data.Leaders.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "Leaders" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, "%," + arr[i] + ",%"));
                        strTmp += "Leaders LIKE @" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(data.Attendees))
            {
                string strTmp = "";
                string[] arr = data.Attendees.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "Attendees" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, "%," + arr[i] + ",%"));
                        strTmp += "Attendees LIKE @" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(data.SignDesk))
            {
                string strTmp = "";
                string[] arr = data.SignDesk.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "SignDesk" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, "%," + arr[i] + ",%"));
                        strTmp += "SignDesk LIKE @" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (data.AddTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@AddTime", SqlDbType.DateTime, "AddTime", data.AddTime));
                strFromWhere += " AND AddTime>=@AddTime";
            }
            if (data.AddUser != null)
            {
                list.Add(SqlParamHelper.AddParameter("@AddUser", SqlDbType.NVarChar, "AddUser", data.AddUser));
                strFromWhere += " AND AddUser=@AddUser";
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
            if (data.SignTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@SignTime", SqlDbType.DateTime, "SignTime", data.SignTime));
                strFromWhere += " AND SignTime>=@SignTime";
            }
            string strOrder = " ORDER BY ";
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                strOrder += strOrderBy;
            }
            else
            {
                strOrder += "StartTime ASC, EndTime ASC, OverTime ASC, UpTime DESC, AddTime DESC";
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
            DataPerform[] result = (DataPerform[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataPerform));
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
        public int Insert(DataPerform data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataPerform data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataPerform data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.OrgName != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OrgName", SqlDbType.NText, "OrgName", data.OrgName));
            }
            if (data.Linkman != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Linkman", SqlDbType.NVarChar, "Linkman", data.Linkman));
            }
            if (data.LinkmanTel != null)
            {
                list.Add(SqlParamHelper.AddParameter("@LinkmanTel", SqlDbType.VarChar, "LinkmanTel", data.LinkmanTel));
            }
            if (data.IsMust != null)
            {
                list.Add(SqlParamHelper.AddParameter("@IsMust", SqlDbType.NVarChar, "IsMust", data.IsMust));
            }
            if (data.SubType != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubType", SqlDbType.NText, "SubType", data.SubType));
            }
            if (data.Title != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Title", SqlDbType.NVarChar, "Title", data.Title));
            }
            if (data.OverTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@OverTime", SqlDbType.DateTime, "OverTime", data.OverTime));
            }
            if (data.StartTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@StartTime", SqlDbType.DateTime, "StartTime", data.StartTime));
            }
            if (data.EndTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@EndTime", SqlDbType.DateTime, "EndTime", data.EndTime));
            }
            if (data.PerformSite != null)
            {
                list.Add(SqlParamHelper.AddParameter("@PerformSite", SqlDbType.NVarChar, "PerformSite", data.PerformSite));
            }
            if (data.Body != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Body", SqlDbType.NText, "Body", data.Body));
            }
            if (data.Files != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Files", SqlDbType.Text, "Files", data.Files));
            }
            if (data.Leaders != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Leaders", SqlDbType.NText, "Leaders", data.Leaders));
            }
            if (data.Attendees != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Attendees", SqlDbType.NText, "Attendees", data.Attendees));
            }
            if (data.HaveBus != null)
            {
                list.Add(SqlParamHelper.AddParameter("@HaveBus", SqlDbType.NVarChar, "HaveBus", data.HaveBus));
            }
            if (data.HaveDinner != null)
            {
                list.Add(SqlParamHelper.AddParameter("@HaveDinner", SqlDbType.NVarChar, "HaveDinner", data.HaveDinner));
            }
            if (data.Remark != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Remark", SqlDbType.NText, "Remark", data.Remark));
            }
            if (data.ActiveName != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ActiveName", SqlDbType.NVarChar, "ActiveName", data.ActiveName));
            }
            if (data.VerifyInfo != null)
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyInfo", SqlDbType.NText, "VerifyInfo", data.VerifyInfo));
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
            if (data.FinishTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@FinishTime", SqlDbType.DateTime, "FinishTime", data.FinishTime));
            }
            if (data.FinishIp != null)
            {
                list.Add(SqlParamHelper.AddParameter("@FinishIp", SqlDbType.VarChar, "FinishIp", data.FinishIp));
            }
            if (data.FinishUser != null)
            {
                list.Add(SqlParamHelper.AddParameter("@FinishUser", SqlDbType.NVarChar, "FinishUser", data.FinishUser));
            }
            if (data.SignDesk != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SignDesk", SqlDbType.NVarChar, "SignDesk", data.SignDesk));
            }
            if (data.SignTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@SignTime", SqlDbType.DateTime, "SignTime", data.SignTime));
            }
            else
            {
                list.Add(SqlParamHelper.AddParameter("@SignTime", SqlDbType.VarChar, "SignTime", null));
            }
            return list.ToArray();
        }
        //修改状态
        public int UpdateActive(int intId, string ActiveName)
        {
            string strSql = string.Format("UPDATE {0} SET ActiveName=@ActiveName WHERE Id=@Id", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
            list.Add(SqlParamHelper.AddParameter("@ActiveName", SqlDbType.NVarChar, "ActiveName", ActiveName));
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        //public int UpdateActive(ArrayList arrList, int intActive, string VerifyInfo = "", string strIp = "", string strUser = "")
        public int UpdateActive(ArrayList arrList, string ActiveName, string VerifyInfo = "", string strIp = "", string strUser = "", string strFinishIp = "", string strFinishUser = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strSql = string.Format("UPDATE {0} SET ActiveName=@ActiveName", TableName);
            list.Add(SqlParamHelper.AddParameter("@ActiveName", SqlDbType.NVarChar, "ActiveName", ActiveName));
            if (!string.IsNullOrEmpty(strIp) || !string.IsNullOrEmpty(strUser))
            {
                list.Add(SqlParamHelper.AddParameter("@UpTime", SqlDbType.DateTime, "UpTime", DateTime.Now));
                list.Add(SqlParamHelper.AddParameter("@UpIp", SqlDbType.VarChar, "UpIp", strIp));
                list.Add(SqlParamHelper.AddParameter("@UpUser", SqlDbType.NVarChar, "UpUser", strUser));
                strSql += ", UpTime=@UpTime, UpIp=@UpIp, UpUser=@UpUser";
            }
            else if (!string.IsNullOrEmpty(strFinishIp) || !string.IsNullOrEmpty(strFinishUser))
            {
                list.Add(SqlParamHelper.AddParameter("@FinishTime", SqlDbType.DateTime, "FinishTime", DateTime.Now));
                list.Add(SqlParamHelper.AddParameter("@FinishIp", SqlDbType.VarChar, "FinishIp", strFinishIp));
                list.Add(SqlParamHelper.AddParameter("@FinishUser", SqlDbType.NVarChar, "FinishUser", strFinishUser));
                strSql += ", FinishTime=@FinishTime, FinishIp=@FinishIp, FinishUser=@FinishUser";
            }
            if (!string.IsNullOrEmpty(VerifyInfo))
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyInfo", SqlDbType.NText, "VerifyInfo", VerifyInfo));
                strSql += ", VerifyInfo=@VerifyInfo";
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
    }
    //
}