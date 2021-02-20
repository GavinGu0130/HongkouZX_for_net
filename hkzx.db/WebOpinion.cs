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
    public class DataOpinion : MSBaseData
    {
        public int Id { get; set; }//主键
        public string OpNum { get; set; }//流水号
        public string OpNo { get; set; }//提案号
        public string Period { get; set; }//_届
        public string Times { get; set; }//_次会议
        public int TeamNum { get; set; }//第n组
        public string SubType { get; set; }//提案分类：经济，城市建设与管理，教科文卫体，其他
        public string IsSign { get; set; }//是否会签：是，否
        public string TimeMark { get; set; }//时间标识：会间，会后
        public string IsOpen { get; set; }//是否同意公开：是，否
        public string OpenInfo { get; set; }//不公开原由
        public string IsPoint { get; set; }//是否重点提案：是
        public string IsGood { get; set; }//是否优秀提案：是
        public string IsFeed { get; set; }//需意见反馈：是
        public string ReApply { get; set; }//再办理：是
        public string SubManType { get; set; }//提交者性质：委员(个人、联名)，团体(政治面貌、专委会、界别、街道活动组)
        public string Party { get; set; }//政治面貌
        public string Committee { get; set; }//专委会
        public string Subsector { get; set; }//界别
        public string StreetTeam { get; set; }//街道活动组
        public string SubMan { get; set; }//（委员提案）第一提案人，（团体提案）组织名称。（归并后）,委员,委员,
        public string SubMan2 { get; set; }//（委员提案）第二提案人：（归并后）,委员,委员,
        public string SubMans { get; set; }//（委员提案）联名人：,委员,委员,
        public string Linkman { get; set; }//（团体提案）联系人
        public string LinkmanAddress { get; set; }//（团体提案）通讯地址
        public string LinkmanZip { get; set; }//（团体提案）通讯邮编
        public string LinkmanTel { get; set; }//（团体提案）联系电话
        public string Summary { get; set; }//案由
        public string Body { get; set; }//提案内容
        public string Files { get; set; }//附件：url|url
        public string Remark { get; set; }//备注
        public string AdviseHostOrg { get; set; }//（建议意向）主办单位|区党群部门
        public string AdviseHelpOrg { get; set; }//（建议意向）会办单位|区党群部门
        public string ExamHostOrg { get; set; }//（审查意向）主办单位|区政府部门
        public string ExamHelpOrg { get; set; }//（审查意向）会办单位|区政府部门
        public string MergeId { get; set; }//归并Id：,Id,Id,
        public DateTime SubTime { get; set; }//提交时间
        public string SubIp { get; set; }//提交IP:端口号
        public DateTime PlannedDate { get; set; }//计划办结日期
        public string ResultInfo { get; set; }//办理结果：处理中，解决或采纳，列入计划拟解决，留作参考
        public string ResultBody { get; set; }//办理结果说明
        public string ResultInfo2 { get; set; }//跟踪办理结果：处理中，解决或采纳，列入计划拟解决，留作参考
        public string ResultBody2 { get; set; }//跟踪办理结果说明
        public string ApplyState { get; set; }//办理状态：提案立案，提案分理，部门处理，督办处理，已办复
        public string ActiveName { get; set; }//提案性质：删除，暂存，待立案，立案，不立案，退回
        public string VerifyInfo { get; set; }//审核意见：12种不立案的情况
        public int UserId { get; set; }//用户Id
        public DateTime AddTime { get; set; }//添加时间：默认为当前时间
        public string AddIp { get; set; }//添加IP:端口号
        public string AddUser { get; set; }//添加人
        public DateTime UpTime { get; set; }//修改时间：默认为当前时间
        public string UpIp { get; set; }//修改IP:端口号
        public string UpUser { get; set; }//修改人
        public DateTime RegTime { get; set; }//立案时间：
        public string RegIp { get; set; }//立案IP:端口号
        public string RegUser { get; set; }//立案人
        public DateTime VerifyTime { get; set; }//审核时间
        public string VerifyIp { get; set; }//审核IP:端口号
        public string VerifyUser { get; set; }//审核人
        public DateTime ResultTime { get; set; }//办复时间：
        public string ResultIp { get; set; }//办复IP:端口号
        public string ResultUser { get; set; }//办复人
        //分页统计
        public string rowClass { get; set; }//行class属性
        public int num { get; set; }//序号
        public int total { get; set; }//统计数
        public string other { get; set; }//按钮等
        public string tbody { get; set; }//自定义行
        public string SubMan3 { get; set; }//会签人
        public string StateName { get; set; }//操作名称
        public string SubTimeText { get; set; }//提交时间文本
        public string ExamOrgType { get; set; }//单位所属：区政府部门，区党群部门，其他
        public string QueryFields { get; set; }//查询字符
        //INNER JOIN
        public string JoinFields { get; set; }//查询字段
        public string JoinText { get; set; }//查询字符串
        public int UserSubNum { get; set; }//提案数
        public int UserSubMin { get; set; }//提案数小于
        public bool IsSubMan1 { get; set; }//只判断第一提案人
        public string UserCode { get; set; }//委员编号
        public string UserSex { get; set; }//提案人性别
        public string UserParty { get; set; }//提案人政治面貌
        public string UserCommittee { get; set; }//提案人专委会
        public string UserSubsector { get; set; }//提案人界别
        public string UserStreetTeam { get; set; }//提案人街道活动组
        public int FeedId { get; set; }//反馈表Id
        public string FeedTakeWay { get; set; }//答复前听取意见方式
        public string FeedAttitude { get; set; }//办理(走访)人员态度
        public string FeedResult { get; set; }//是否同意办理结果
        public string FeedInterview { get; set; }//走访情况
        public string FeedPertinence { get; set; }//答复是否针对提案
        public string FeedLeaderReply { get; set; }//(团体)分管领导答复
        public int FeedActive { get; set; }//反馈状态
        public string FeedActiveName { get; set; }//反馈状态文本
        //
        private static string[] columnList = new[] {
            "Id", "OpNum", "OpNo", "Period", "Times", "TeamNum", "SubType", "IsSign", "TimeMark", "IsOpen", 
            "OpenInfo", "IsPoint", "IsGood", "IsFeed", "ReApply", "SubManType", "Party", "Committee", "Subsector", "StreetTeam", 
            "SubMan", "SubMan2", "SubMans", "Linkman", "LinkmanAddress", "LinkmanZip", "LinkmanTel", "Summary", "Body", "Files", 
            "Remark", "AdviseHostOrg", "AdviseHelpOrg", "ExamHostOrg", "ExamHelpOrg", "MergeId", "SubTime", "SubIp", "PlannedDate", "ResultInfo", 
            "ResultBody", "ResultInfo2", "ResultBody2", "ApplyState", "ActiveName", "VerifyInfo", "UserId", "AddTime", "AddIp", "AddUser", 
            "UpTime", "UpIp", "UpUser", "RegTime", "RegIp", "RegUser", "VerifyTime", "VerifyIp", "VerifyUser", "ResultTime", 
            "ResultIp", "ResultUser"
            , "UserSubNum", "UserParty", "UserSex"
            , "FeedId", "FeedInterview", "FeedAttitude", "FeedResult", "FeedTakeWay", "FeedPertinence", "FeedLeaderReply", "FeedActive"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] {
            SqlDbType.Int, SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, 
            SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, 
            SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.Text, 
            SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, SqlDbType.Text, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.DateTime, SqlDbType.NVarChar, 
            SqlDbType.NText, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.Int, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, 
            SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, 
            SqlDbType.VarChar, SqlDbType.NVarChar
            , SqlDbType.Int, SqlDbType.NText, SqlDbType.NVarChar
            , SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.Int
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
    public class WebOpinion
    {
        private const string TableName = "tb_Opinion";
        SqlDAC sqlDac;
        public WebOpinion()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }
        //
        #region 查询
        public DataOpinion[] GetData(int intId, string strFields = "")
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
            DataOpinion[] result = (DataOpinion[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataOpinion));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataOpinion[] GetDatas(string strOpNo, string strOpNum = "", int intId = 0)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strSql = string.Format("SELECT * FROM {0} WHERE 1=0", TableName);
            if (!string.IsNullOrEmpty(strOpNo))
            {
                list.Add(SqlParamHelper.AddParameter("@OpNo", SqlDbType.VarChar, "OpNo", strOpNo));
                strSql += " OR OpNo=@OpNo";
            }
            if (!string.IsNullOrEmpty(strOpNum))
            {
                list.Add(SqlParamHelper.AddParameter("@OpNum", SqlDbType.VarChar, "OpNum", strOpNum));
                strSql += " OR OpNum=@OpNum";
            }
            if (intId > 0)
            {
                list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
                strSql += " OR Id=@Id";
            }
            strSql += " ORDER BY Id ASC";
            SqlParameter[] sqlParaArray = list.ToArray();
            DataOpinion[] result = (DataOpinion[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataOpinion));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataOpinion[] GetDatas(DataOpinion data, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "", string strJoin = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strFromWhere = string.Format("FROM {0} AS o{1} WHERE ", TableName, strJoin);
            if (!string.IsNullOrEmpty(data.MergeId))
            {
                string strTmp = "";
                string[] arr = data.MergeId.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "Id" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                        strTmp += "o.Id=@" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += "(" + strTmp + ")";
                }
            }
            else if (!string.IsNullOrEmpty(data.ActiveName))
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
                        if (arr[i].IndexOf("<>") >= 0)
                        {
                            strTmp += "o.ActiveName" + arr[i];
                        }
                        else
                        {
                            string tmp = "ActiveName" + i.ToString();
                            list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                            strTmp += "o.ActiveName=@" + tmp;
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
            if (data.OpNo != null)
            {
                if (data.OpNo.IndexOf("<>") >= 0)
                {
                    strFromWhere += " AND o.OpNo" + data.OpNo;
                }
                else
                {
                    list.Add(SqlParamHelper.AddParameter("@OpNo", SqlDbType.NVarChar, "OpNo", data.OpNo));
                    strFromWhere += " AND o.OpNo=@OpNo";
                }
            }
            if (!string.IsNullOrEmpty(data.Period))
            {
                list.Add(SqlParamHelper.AddParameter("@Period", SqlDbType.NVarChar, "Period", data.Period));
                strFromWhere += " AND o.Period=@Period";
            }
            if (!string.IsNullOrEmpty(data.Times))
            {
                list.Add(SqlParamHelper.AddParameter("@Times", SqlDbType.NVarChar, "Times", data.Times));
                strFromWhere += " AND o.Times=@Times";
            }
            if (data.TeamNum > 0)
            {
                list.Add(SqlParamHelper.AddParameter("@TeamNum", SqlDbType.Int, "TeamNum", data.TeamNum));
                strFromWhere += " AND o.TeamNum=@TeamNum";
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
                        strTmp += "o.SubType=@" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(data.ApplyState))
            {
                string strTmp = "";
                string[] arr = data.ApplyState.Split(',');
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
                            strTmp += "o.ApplyState" + arr[i];
                        }
                        else
                        {
                            string tmp = "ApplyState" + i.ToString();
                            list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                            strTmp += "o.ApplyState=@" + tmp;
                        }
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(data.IsSign))
            {
                list.Add(SqlParamHelper.AddParameter("@IsSign", SqlDbType.NVarChar, "IsSign", data.IsSign));
                strFromWhere += " AND o.IsSign=@IsSign";
            }
            if (!string.IsNullOrEmpty(data.TimeMark))
            {
                list.Add(SqlParamHelper.AddParameter("@TimeMark", SqlDbType.NVarChar, "TimeMark", data.TimeMark));
                strFromWhere += " AND o.TimeMark=@TimeMark";
            }
            if (!string.IsNullOrEmpty(data.IsOpen))
            {
                if (data.IsOpen.IndexOf("<>") >= 0)
                {
                    strFromWhere += " AND o.IsOpen" + data.IsOpen;
                }
                else
                {
                    list.Add(SqlParamHelper.AddParameter("@IsOpen", SqlDbType.NVarChar, "IsOpen", data.IsOpen));
                    strFromWhere += " AND o.IsOpen=@IsOpen";
                }
            }
            if (!string.IsNullOrEmpty(data.IsPoint))
            {
                list.Add(SqlParamHelper.AddParameter("@IsPoint", SqlDbType.NVarChar, "IsPoint", data.IsPoint));
                strFromWhere += " AND o.IsPoint=@IsPoint";
            }
            if (!string.IsNullOrEmpty(data.IsGood))
            {
                list.Add(SqlParamHelper.AddParameter("@IsGood", SqlDbType.NVarChar, "IsGood", data.IsGood));
                strFromWhere += " AND o.IsGood=@IsGood";
            }
            if (!string.IsNullOrEmpty(data.IsFeed))
            {
                list.Add(SqlParamHelper.AddParameter("@IsFeed", SqlDbType.NVarChar, "IsFeed", data.IsFeed));
                strFromWhere += " AND o.IsFeed=@IsFeed";
            }
            if (data.ReApply != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ReApply", SqlDbType.NVarChar, "ReApply", data.ReApply));
                strFromWhere += " AND o.ReApply=@ReApply";
            }
            if (data.SubManType != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubManType", SqlDbType.NVarChar, "SubManType", data.SubManType));
                strFromWhere += " AND o.SubManType=@SubManType";
            }
            if (data.Party != null)
            {
                string strTmp = "";
                string[] arr = data.Party.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "Party" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, "%" + arr[i] + "%"));
                        strTmp += "o.Party LIKE @" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (data.Committee != null)
            {
                string strTmp = "";
                string[] arr = data.Committee.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "Committee" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, "%" + arr[i] + "%"));
                        strTmp += "o.Committee LIKE @" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (data.Subsector != null)
            {
                string strTmp = "";
                string[] arr = data.Subsector.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "Subsector" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, "%" + arr[i] + "%"));
                        strTmp += "o.Subsector LIKE @" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (data.StreetTeam != null)
            {
                string strTmp = "";
                string[] arr = data.StreetTeam.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "StreetTeam" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, "%" + arr[i] + "%"));
                        strTmp += "o.StreetTeam LIKE @" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(data.Summary))
            {
                string strTmp = "";
                string[] arr = data.Summary.Split('+');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "Summary" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                        if (arr[i].IndexOf("%") >= 0)
                        {
                            strTmp += "o.Summary LIKE @" + tmp;
                        }
                        else
                        {
                            strTmp += "o.Summary=@" + tmp;
                        }
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(data.Body))
            {
                string strTmp = "";
                string[] arr = data.Body.Split('+');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "Body" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                        strTmp += "o.Body LIKE @" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(data.SubMan))
            {
                string strTmp = "";
                if (data.IsSubMan1)
                {
                    //第一提交人
                    //list.Add(SqlParamHelper.AddParameter("@SubMan1", SqlDbType.NVarChar, "SubMan1", "%," + data.SubMan1 + ",%"));
                    //strFromWhere += " AND o.SubMan LIKE @SubMan1";
                    string[] arr = data.SubMan.Split(',');
                    for (int i = 0; i < arr.Count(); i++)
                    {
                        if (arr[i] != "")
                        {
                            if (strTmp != "")
                            {
                                strTmp += " OR ";
                            }
                            string tmp = "SubMan1_" + i.ToString();
                            list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, "%," + arr[i] + ",%"));
                            strTmp += string.Format("o.SubMan LIKE @{0}", tmp);
                        }
                    }
                }
                else
                {
                    //我的提案：第一、二、三提案人
                    //list.Add(SqlParamHelper.AddParameter("@SubMans", SqlDbType.NVarChar, "SubMans", "%," + data.SubMan + ",%"));
                    //strFromWhere += " AND (o.SubMan LIKE @SubMans OR o.SubMan2 LIKE @SubMans OR o.SubMans LIKE @SubMans)";
                    string[] arr = data.SubMan.Split(',');
                    for (int i = 0; i < arr.Count(); i++)
                    {
                        if (arr[i] != "")
                        {
                            if (strTmp != "")
                            {
                                strTmp += " OR ";
                            }
                            string tmp = "SubMans_" + i.ToString();
                            list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, "%," + arr[i] + ",%"));
                            strTmp += string.Format("o.SubMan LIKE @{0} OR o.SubMans LIKE @{0}", tmp);// OR o.SubMan2 LIKE @{0}
                        }
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(data.SubMan3))
                {//联名提交人(检查会签)
                    list.Add(SqlParamHelper.AddParameter("@SubMan3", SqlDbType.NVarChar, "SubMan3", data.SubMan3));
                    strFromWhere += " AND (s.Overdue>GETDATE() OR s.Active>0)";
                }
                if (!string.IsNullOrEmpty(data.SubMans))
                {//联名提交人
                    string strTmp = "";
                    string[] arr = data.SubMans.Split(',');
                    for (int i = 0; i < arr.Count(); i++)
                    {
                        if (arr[i] != "")
                        {
                            if (strTmp != "")
                            {
                                strTmp += " OR ";
                            }
                            string tmps = "SubMans_" + i.ToString();
                            list.Add(SqlParamHelper.AddParameter("@" + tmps, SqlDbType.NVarChar, tmps, "%," + arr[i] + ",%"));
                            strTmp += string.Format("o.SubMans LIKE @{0}", tmps);// OR o.SubMan2 LIKE @{0}
                        }
                    }
                    if (strTmp != "")
                    {
                        strFromWhere += " AND (" + strTmp + ")";
                    }
                }
            }
            if (data.ExamHostOrg != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ExamHostOrg", SqlDbType.NText, "ExamHostOrg", data.ExamHostOrg));
                strFromWhere += " AND o.ExamHostOrg LIKE @ExamHostOrg";
            }
            if (data.ExamHelpOrg != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ExamHelpOrg", SqlDbType.NText, "ExamHelpOrg", data.ExamHelpOrg));
                strFromWhere += " AND o.ExamHelpOrg LIKE @ExamHelpOrg";
            }
            if (data.ExamOrgType != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ExamOrgType", SqlDbType.NVarChar, "ExamOrgType", "%|" + data.ExamOrgType));
                strFromWhere += " AND (o.ExamHostOrg LIKE @ExamOrgType OR o.ExamHelpOrg LIKE @ExamOrgType)";
            }
            if (data.ResultInfo != null)
            {
                if (data.ResultInfo.IndexOf("<>") >= 0)
                {
                    strFromWhere += " AND o.ResultInfo" + data.ResultInfo;
                }
                else
                {
                    list.Add(SqlParamHelper.AddParameter("@ResultInfo", SqlDbType.NVarChar, "ResultInfo", data.ResultInfo));
                    strFromWhere += " AND o.ResultInfo=@ResultInfo";
                }
            }
            if (data.ResultInfo2 != null)
            {
                if (data.ResultInfo2.IndexOf("<>") >= 0)
                {
                    strFromWhere += " AND o.ResultInfo2" + data.ResultInfo2;
                }
                else
                {
                    list.Add(SqlParamHelper.AddParameter("@ResultInfo2", SqlDbType.NVarChar, "ResultInfo2", data.ResultInfo2));
                    strFromWhere += " AND o.ResultInfo2=@ResultInfo2";
                }
            }
            if (!string.IsNullOrEmpty(data.SubTimeText) && data.SubTimeText.IndexOf(",") >= 0)
            {
                string[] arr = data.SubTimeText.Split(',');
                if (!string.IsNullOrEmpty(arr[0]))
                {
                    list.Add(SqlParamHelper.AddParameter("@SubTime1", SqlDbType.DateTime, "SubTime1", Convert.ToDateTime(arr[0])));
                    strFromWhere += " AND o.SubTime>=@SubTime1";
                }
                if (!string.IsNullOrEmpty(arr[1]))
                {
                    if (arr[1].IndexOf(":") < 0)
                    {
                        arr[1] += " 23:59:59";
                    }
                    list.Add(SqlParamHelper.AddParameter("@SubTime2", SqlDbType.DateTime, "SubTime2", Convert.ToDateTime(arr[1])));
                    strFromWhere += " AND o.SubTime<=@SubTime2";
                }
            }
            if (data.AddTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@AddTime", SqlDbType.DateTime, "AddTime", data.AddTime));
                strFromWhere += " AND o.AddTime>=@AddTime";
            }
            if (data.AddUser != null)
            {
                list.Add(SqlParamHelper.AddParameter("@AddUser", SqlDbType.NVarChar, "AddUser", data.AddUser));
                strFromWhere += " AND o.AddUser=@AddUser";
            }
            //外联查询
            if (data.UserSex != null)
            {
                list.Add(SqlParamHelper.AddParameter("@UserSex", SqlDbType.NVarChar, "UserSex", data.UserSex));
                strFromWhere += " AND u.UserSex=@UserSex";
            }
            if (data.UserParty != null)
            {
                list.Add(SqlParamHelper.AddParameter("@UserParty", SqlDbType.NVarChar, "UserParty", data.UserParty));
                strFromWhere += " AND u.Party LIKE @UserParty";
            }
            if (data.UserCommittee != null)
            {
                list.Add(SqlParamHelper.AddParameter("@UserCommittee", SqlDbType.NVarChar, "UserCommittee", data.UserCommittee));
                strFromWhere += " AND (u.Committee LIKE @UserCommittee OR u.Committee2 LIKE @UserCommittee)";
            }
            if (data.UserSubsector != null)
            {
                list.Add(SqlParamHelper.AddParameter("@UserSubsector", SqlDbType.NVarChar, "UserSubsector", data.UserSubsector));
                strFromWhere += " AND u.Subsector LIKE @UserSubsector";
            }
            if (data.UserStreetTeam != null)
            {
                list.Add(SqlParamHelper.AddParameter("@UserStreetTeam", SqlDbType.NVarChar, "UserStreetTeam", data.UserStreetTeam));
                strFromWhere += " AND u.StreetTeam LIKE @UserStreetTeam";
            }
            if (data.FeedInterview != null)
            {
                list.Add(SqlParamHelper.AddParameter("@FeedInterview", SqlDbType.NVarChar, "FeedInterview", data.FeedInterview));
                strFromWhere += " AND f.Interview=@FeedInterview";
            }
            if (data.FeedAttitude != null)
            {
                list.Add(SqlParamHelper.AddParameter("@FeedAttitude", SqlDbType.NVarChar, "FeedAttitude", data.FeedAttitude));
                strFromWhere += " AND f.Attitude=@FeedAttitude";
            }
            if (data.FeedResult != null)
            {
                list.Add(SqlParamHelper.AddParameter("@FeedResult", SqlDbType.NVarChar, "FeedResult", data.FeedResult));
                strFromWhere += " AND f.Result=@FeedResult";
            }
            if (data.FeedTakeWay != null)
            {
                list.Add(SqlParamHelper.AddParameter("@FeedTakeWay", SqlDbType.NVarChar, "FeedTakeWay", data.FeedTakeWay));
                strFromWhere += " AND f.TakeWay=@FeedTakeWay";
            }
            if (data.FeedPertinence != null)
            {
                list.Add(SqlParamHelper.AddParameter("@FeedPertinence", SqlDbType.NVarChar, "FeedPertinence", data.FeedPertinence));
                strFromWhere += " AND f.Pertinence=@FeedPertinence";
            }
            if (data.FeedLeaderReply != null)
            {
                list.Add(SqlParamHelper.AddParameter("@FeedLeaderReply", SqlDbType.NVarChar, "FeedLeaderReply", data.FeedLeaderReply));
                strFromWhere += " AND f.LeaderReply=@FeedLeaderReply";
            }
            if (data.FeedActiveName != null)
            {
                if (data.FeedActiveName == "是")
                {
                    strFromWhere += " AND f.Active>0";
                }
                else
                {
                    strFromWhere += " AND o.IsFeed='是' AND (f.Active<=0 OR f.Active IS NULL)";
                }
            }
            //外联结束
            //if (data.UserSubNum > 0)
            //{
            //    strFromWhere += " AND " + data.UserSubNum.ToString() + "<=ALL (SELECT COUNT(t.Id) FROM " + TableName + " AS t WHERE t.SubMan LIKE o.SubMan)";
            //}
            string strOrder = " ORDER BY ";
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                strOrder += strOrderBy;
            }
            else
            {
                strOrder += "o.OpNo DESC, o.SubTime DESC, o.UpTime DESC, o.AddTime DESC";
            }
            if (strOrderBy.IndexOf("Id ASC") < 0 && strOrderBy.IndexOf("Id DESC") < 0)
            {
                strOrder += ", o.Id ASC";
            }
            if (strFields == "")
            {
                strFields = " o.* ";
            }
            else
            {
                strFields = " " + strFields + " ";
            }
            string strSql = "";
            if (pageSize > 0 && intPage > 1)
            {
                //分页查询语句：SELECT TOP 页大小 * FROM table WHERE id NOT IN ( SELECT TOP 页大小*(页数-1) id FROM table ORDER BY id ) ORDER BY id
                strSql = "SELECT TOP " + pageSize.ToString() + strFields + strFromWhere + " AND o.Id NOT IN ( SELECT TOP " + (pageSize * (intPage - 1)).ToString() + " o.Id " + strFromWhere + strOrder + " )" + strOrder;
            }
            else
            {
                if (strFields.IndexOf("distinct") >= 0)
                {
                    strSql = "SELECT DISTINCT Id FROM (SELECT " + strFields + strFromWhere + ") AS newTable";
                }
                else
                {
                    if (pageSize <= 0)
                    {
                        pageSize = 100000;//考虑数据库性能，只读取前100,000条数据
                    }
                    strSql = "SELECT TOP " + pageSize.ToString() + strFields + strFromWhere + strOrder;
                }
            }
            //HttpContext.Current.Response.Write(strSql); HttpContext.Current.Response.End();
            SqlParameter[] sqlParaArray = list.ToArray();
            DataOpinion[] result = (DataOpinion[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataOpinion));
            if (result != null && result.Length > 0)
            {
                if (strFilter.IndexOf("total") >= 0)
                {
                    strSql = "SELECT COUNT(o.Id) " + strFromWhere;
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
        public int Insert(DataOpinion data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataOpinion data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataOpinion data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.OpNum != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OpNum", SqlDbType.VarChar, "OpNum", data.OpNum));
            }
            if (data.OpNo != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OpNo", SqlDbType.VarChar, "OpNo", data.OpNo));
            }
            if (data.Period != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Period", SqlDbType.NVarChar, "Period", data.Period));
            }
            if (data.Times != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Times", SqlDbType.NVarChar, "Times", data.Times));
            }
            if (data.TeamNum >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@TeamNum", SqlDbType.Int, "TeamNum", data.TeamNum));
            }
            if (data.SubType != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubType", SqlDbType.NVarChar, "SubType", data.SubType));
            }
            if (data.IsSign != null)
            {
                list.Add(SqlParamHelper.AddParameter("@IsSign", SqlDbType.NVarChar, "IsSign", data.IsSign));
            }
            if (data.TimeMark != null)
            {
                list.Add(SqlParamHelper.AddParameter("@TimeMark", SqlDbType.NVarChar, "TimeMark", data.TimeMark));
            }
            if (data.IsOpen != null)
            {
                list.Add(SqlParamHelper.AddParameter("@IsOpen", SqlDbType.NVarChar, "IsOpen", data.IsOpen));
            }
            if (data.OpenInfo != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OpenInfo", SqlDbType.NVarChar, "OpenInfo", data.OpenInfo));
            }
            if (data.IsPoint != null)
            {
                list.Add(SqlParamHelper.AddParameter("@IsPoint", SqlDbType.NVarChar, "IsPoint", data.IsPoint));
            }
            if (data.IsGood != null)
            {
                list.Add(SqlParamHelper.AddParameter("@IsGood", SqlDbType.NVarChar, "IsGood", data.IsGood));
            }
            if (data.IsFeed != null)
            {
                list.Add(SqlParamHelper.AddParameter("@IsFeed", SqlDbType.NVarChar, "IsFeed", data.IsFeed));
            }
            if (data.ReApply != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ReApply", SqlDbType.NVarChar, "ReApply", data.ReApply));
            }
            if (data.SubManType != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubManType", SqlDbType.NVarChar, "SubManType", data.SubManType));
            }
            if (data.Party != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Party", SqlDbType.NText, "Party", data.Party));
            }
            if (data.Committee != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Committee", SqlDbType.NText, "Committee", data.Committee));
            }
            if (data.Subsector != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Subsector", SqlDbType.NText, "Subsector", data.Subsector));
            }
            if (data.StreetTeam != null)
            {
                list.Add(SqlParamHelper.AddParameter("@StreetTeam", SqlDbType.NText, "StreetTeam", data.StreetTeam));
            }
            if (data.SubMan != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubMan", SqlDbType.NVarChar, "SubMan", data.SubMan));
            }
            if (data.SubMan2 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubMan2", SqlDbType.NVarChar, "SubMan2", data.SubMan2));
            }
            if (data.SubMans != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubMans", SqlDbType.NText, "SubMans", data.SubMans));
            }
            if (data.Linkman != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Linkman", SqlDbType.NVarChar, "Linkman", data.Linkman));
            }
            if (data.LinkmanAddress != null)
            {
                list.Add(SqlParamHelper.AddParameter("@LinkmanAddress", SqlDbType.NText, "LinkmanAddress", data.LinkmanAddress));
            }
            if (data.LinkmanZip != null)
            {
                list.Add(SqlParamHelper.AddParameter("@LinkmanZip", SqlDbType.VarChar, "LinkmanZip", data.LinkmanZip));
            }
            if (data.LinkmanTel != null)
            {
                list.Add(SqlParamHelper.AddParameter("@LinkmanTel", SqlDbType.VarChar, "LinkmanTel", data.LinkmanTel));
            }
            if (data.Summary != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Summary", SqlDbType.NVarChar, "Summary", data.Summary));
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
            if (data.AdviseHostOrg != null)
            {
                list.Add(SqlParamHelper.AddParameter("@AdviseHostOrg", SqlDbType.NText, "AdviseHostOrg", data.AdviseHostOrg));
            }
            if (data.AdviseHelpOrg != null)
            {
                list.Add(SqlParamHelper.AddParameter("@AdviseHelpOrg", SqlDbType.NText, "AdviseHelpOrg", data.AdviseHelpOrg));
            }
            if (data.ExamHostOrg != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ExamHostOrg", SqlDbType.NText, "ExamHostOrg", data.ExamHostOrg));
            }
            if (data.ExamHelpOrg != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ExamHelpOrg", SqlDbType.NText, "ExamHelpOrg", data.ExamHelpOrg));
            }
            if (data.MergeId != null)
            {
                list.Add(SqlParamHelper.AddParameter("@MergeId", SqlDbType.Text, "MergeId", data.MergeId));
            }
            if (data.SubTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@SubTime", SqlDbType.DateTime, "SubTime", data.SubTime));
            }
            if (data.SubIp != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubIp", SqlDbType.VarChar, "SubIp", data.SubIp));
            }
            if (data.PlannedDate > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@PlannedDate", SqlDbType.DateTime, "PlannedDate", data.PlannedDate));
            }
            if (data.ResultInfo != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ResultInfo", SqlDbType.NVarChar, "ResultInfo", data.ResultInfo));
            }
            if (data.ResultBody != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ResultBody", SqlDbType.NText, "ResultBody", data.ResultBody));
            }
            if (data.ResultInfo2 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ResultInfo2", SqlDbType.NVarChar, "ResultInfo2", data.ResultInfo2));
            }
            if (data.ResultBody2 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ResultBody2", SqlDbType.NText, "ResultBody2", data.ResultBody2));
            }
            if (data.ApplyState != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ApplyState", SqlDbType.NVarChar, "ApplyState", data.ApplyState));
            }
            if (data.ActiveName != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ActiveName", SqlDbType.NVarChar, "ActiveName", data.ActiveName));
            }
            if (data.VerifyInfo != null)
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyInfo", SqlDbType.NText, "VerifyInfo", data.VerifyInfo));
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
            if (data.RegTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@RegTime", SqlDbType.DateTime, "RegTime", data.RegTime));
            }
            if (data.RegIp != null)
            {
                list.Add(SqlParamHelper.AddParameter("@RegIp", SqlDbType.VarChar, "RegIp", data.RegIp));
            }
            if (data.RegUser != null)
            {
                list.Add(SqlParamHelper.AddParameter("@RegUser", SqlDbType.NVarChar, "RegUser", data.RegUser));
            }
            if (data.VerifyTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyTime", SqlDbType.DateTime, "VerifyTime", data.VerifyTime));
            }
            else
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyTime", SqlDbType.VarChar, "VerifyTime", null));
            }
            if (data.VerifyIp != null)
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyIp", SqlDbType.VarChar, "VerifyIp", data.VerifyIp));
            }
            if (data.VerifyUser != null)
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyUser", SqlDbType.NVarChar, "VerifyUser", data.VerifyUser));
            }
            if (data.ResultTime >= DateTime.MaxValue)
            {
                list.Add(SqlParamHelper.AddParameter("@ResultTime", SqlDbType.VarChar, "ResultTime", null));
            }
            else if (data.ResultTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@ResultTime", SqlDbType.DateTime, "ResultTime", data.ResultTime));
            }
            //else
            //{
            //    list.Add(SqlParamHelper.AddParameter("@ResultTime", SqlDbType.VarChar, "ResultTime", null));
            //}
            if (data.ResultIp != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ResultIp", SqlDbType.VarChar, "ResultIp", data.ResultIp));
            }
            if (data.ResultUser != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ResultUser", SqlDbType.NVarChar, "ResultUser", data.ResultUser));
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
        public int UpdateActive(ArrayList arrList, string ActiveName, string strIp = "", string strUser = "", string HostOrg = "", string HelpOrg = "", string Result = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strSql = string.Format("UPDATE {0} SET ", TableName);
            switch (ActiveName)
            {
                case "已承办":
                    list.Add(SqlParamHelper.AddParameter("@ActiveName", SqlDbType.NVarChar, "ActiveName", ActiveName));
                    list.Add(SqlParamHelper.AddParameter("@ExamHostOrg", SqlDbType.NVarChar, "ExamHostOrg", HostOrg));
                    list.Add(SqlParamHelper.AddParameter("@ExamHelpOrg", SqlDbType.NVarChar, "ExamHelpOrg", HelpOrg));
                    strSql += "ActiveName=@ActiveName, ExamHostOrg=@ExamHostOrg, ExamHelpOrg=@ExamHelpOrg";
                    break;
                case "已办复":
                    list.Add(SqlParamHelper.AddParameter("@ActiveName", SqlDbType.NVarChar, "ActiveName", ActiveName));
                    list.Add(SqlParamHelper.AddParameter("@Result", SqlDbType.NVarChar, "Result", Result));
                    strSql += "ActiveName=@ActiveName, Result=@Result";
                    break;
                case "立案":
                case "不立案":
                    break;
                //case "归并":
                //case "删除":
                default:
                    list.Add(SqlParamHelper.AddParameter("@ActiveName", SqlDbType.NVarChar, "ActiveName", ActiveName));
                    strSql += "ActiveName=@ActiveName";
                    break;
            }
            if (!string.IsNullOrEmpty(strIp) || !string.IsNullOrEmpty(strUser))
            {
                if (ActiveName == "立案" || ActiveName == "不立案")
                {
                    list.Add(SqlParamHelper.AddParameter("@RegTime", SqlDbType.DateTime, "RegTime", DateTime.Now));
                    list.Add(SqlParamHelper.AddParameter("@RegIp", SqlDbType.VarChar, "RegIp", strIp));
                    list.Add(SqlParamHelper.AddParameter("@RegUser", SqlDbType.NVarChar, "RegUser", strUser));
                    strSql += "Property='" + ActiveName + "', RegTime=@RegTime, RegIp=@RegIp, RegUser=@RegUser";
                }
                else if (ActiveName == "已办复")
                {
                    list.Add(SqlParamHelper.AddParameter("@ResultTime", SqlDbType.DateTime, "ResultTime", DateTime.Now));
                    list.Add(SqlParamHelper.AddParameter("@ResultIp", SqlDbType.VarChar, "ResultIp", strIp));
                    list.Add(SqlParamHelper.AddParameter("@ResultUser", SqlDbType.NVarChar, "ResultUser", strUser));
                    strSql += ", ResultTime=@ResultTime, ResultIp=@ResultIp, ResultUser=@ResultUser";
                }
                else
                {
                    list.Add(SqlParamHelper.AddParameter("@VerifyTime", SqlDbType.DateTime, "VerifyTime", DateTime.Now));
                    list.Add(SqlParamHelper.AddParameter("@VerifyIp", SqlDbType.VarChar, "VerifyIp", strIp));
                    list.Add(SqlParamHelper.AddParameter("@VerifyUser", SqlDbType.NVarChar, "VerifyUser", strUser));
                    strSql += ", VerifyTime=@VerifyTime, VerifyIp=@VerifyIp, VerifyUser=@VerifyUser";
                }
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
        //更新联名人
        public int UpdateSubMans(int intId, string SubMans, string SubMan2, string IsSign)
        {
            string strSql = string.Format("UPDATE {0} SET SubMan2=@SubMan2, SubMans=@SubMans WHERE Id=@Id", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
            list.Add(SqlParamHelper.AddParameter("@SubMans", SqlDbType.NText, "SubMans", SubMans));
            list.Add(SqlParamHelper.AddParameter("@SubMan2", SqlDbType.NText, "SubMan2", SubMan2));
            list.Add(SqlParamHelper.AddParameter("@IsSign", SqlDbType.NVarChar, "IsSign", IsSign));
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        //更新冗余（党派、专委会、界别、街道活动组）
        public int UpdateUser(int intId, string Party, string Committee, string Subsector, string StreetTeam)
        {
            string strSql = string.Format("UPDATE {0} SET Party=@Party, Committee=@Committee, Subsector=@Subsector, StreetTeam=@StreetTeam WHERE Id=@Id", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
            list.Add(SqlParamHelper.AddParameter("@Party", SqlDbType.NText, "Party", Party));
            list.Add(SqlParamHelper.AddParameter("@Committee", SqlDbType.NText, "Committee", Committee));
            list.Add(SqlParamHelper.AddParameter("@Subsector", SqlDbType.NText, "Subsector", Subsector));
            list.Add(SqlParamHelper.AddParameter("@StreetTeam", SqlDbType.NText, "StreetTeam", StreetTeam));
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        //修改是否需要反馈
        public int UpdateFeed(string Period, string Times, string IsFeed, string strIp = "", string strUser = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strSql = string.Format("UPDATE {0} SET IsFeed=@IsFeed", TableName);
            list.Add(SqlParamHelper.AddParameter("@IsFeed", SqlDbType.NVarChar, "IsFeed", IsFeed));
            if (!string.IsNullOrEmpty(strIp) || !string.IsNullOrEmpty(strUser))
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyTime", SqlDbType.DateTime, "VerifyTime", DateTime.Now));
                list.Add(SqlParamHelper.AddParameter("@VerifyIp", SqlDbType.VarChar, "VerifyIp", strIp));
                list.Add(SqlParamHelper.AddParameter("@VerifyUser", SqlDbType.NVarChar, "VerifyUser", strUser));
                strSql += ", VerifyTime=@VerifyTime, VerifyIp=@VerifyIp, VerifyUser=@VerifyUser";
            }
            strSql += " WHERE Period=@Period AND Times=@Times";
            if (IsFeed == "是")
            {
                strSql += " AND OpNo<>''";
            }
            list.Add(SqlParamHelper.AddParameter("@Period", SqlDbType.NVarChar, "Period", Period));
            list.Add(SqlParamHelper.AddParameter("@Times", SqlDbType.NVarChar, "Times", Times));
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        public int UpdateFeed(ArrayList arrList, string IsFeed, string strIp = "", string strUser = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strSql = string.Format("UPDATE {0} SET IsFeed=@IsFeed", TableName);
            list.Add(SqlParamHelper.AddParameter("@IsFeed", SqlDbType.NVarChar, "IsFeed", IsFeed));
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