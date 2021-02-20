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
    public class DataOpinionPop : MSBaseData
    {
        public int Id { get; set; }//主键
        public string OpNum { get; set; }//流水号=提案号
        public string OrgName { get; set; }//机构名称：政治面貌，专委会，界别活动组，街道活动组
        public string SubType { get; set; }//提交分类：经济，城市建设与管理，教科文卫体，人力资源和社会保障，统战外事，其他
        public string SubType2 { get; set; }//提交分类2
        public string IsGood { get; set; }//是否优秀社情民意：是
        public string IsOpen { get; set; }//是否同意公开：是，否
        public string OpenInfo { get; set; }//不公开原由
        public string SubManType { get; set; }//提交者性质：委员提案(个人、联名)，团体提案(政治面貌、专委会、界别、街道活动组)
        public string Party { get; set; }//政治面貌
        public string Committee { get; set; }//专委会
        public string Subsector { get; set; }//界别
        public string StreetTeam { get; set; }//街道活动组
        public string SubMan { get; set; }//（委员提交）反映人，（团体提交）组织名称
        public string SubMans { get; set; }//（委员提交）联名人：,委员,委员,
        public string SubMan2 { get; set; }//非委员反映人：
        public string Linkman { get; set; }//（团体提交）联系人
        public string LinkmanInfo { get; set; }//（团体提交）第一反映人身份：全国政协委员，市政协委员，区政协委员，民主党派和工商联，其他
        public string LinkmanParty { get; set; }//（团体提交）第一反映人所属党派
        public string LinkmanOrgName { get; set; }//（团体提交）第一反映人工作单位与职务
        public string LinkmanTel { get; set; }//（团体提交）第一反映人联系方式
        public string Summary { get; set; }//标题
        public string Body { get; set; }//情况反映
        public string Advise { get; set; }//意见建议
        public string Files { get; set; }//附件：url(\n)url
        public string Remark { get; set; }//备注
        public string Adopt1 { get; set; }//（区级）采用：单篇；综合，原始篇目： (1)、 (2)
        public string Give1 { get; set; }//（区级）主送：市政协综合信息处；区委、区政府、区政协领导，区委办、区政府办及有关部门；区___委/办/局
        public string Employ1 { get; set; }//（区级）得到区有关部门采用；得区领导批示
        public string Reply1 { get; set; }//（区级）领导批示
        public string Adopt2 { get; set; }//（市级）采用：单篇；综合
        public string Send2 { get; set; }//（市级）报送市政协
        public string Give2 { get; set; }//（市级）转送市有关部门；报送市领导；报送全国政协
        public string Employ2 { get; set; }//（市级）得到市有关部门采用；得到市领导批示
        public string Reply2 { get; set; }//（市级）领导批示
        public string Send3 { get; set; }//（国家）报送全国政协
        public string Give3 { get; set; }//（国家）全国政协单篇采用；全国政协综合采用；全国政协转送国家有关部门
        public string Employ3 { get; set; }//（国家）单篇得到中央领导批示；综合得到中央领导批示；得到国家有关部门采用
        public string Reply3 { get; set; }//（国家）中央领导批示
        public string VerifyTitle { get; set; }//（核稿）标题：关于_____的建议
        public string VerifyBody { get; set; }//（核稿）反映的问题（分析）
        public string VerifyAdvise { get; set; }//（核稿）建议
        public DateTime VerifyTime { get; set; }//（核稿）日期
        public string VerifyIp { get; set; }//（核稿）IP:端口号
        public string VerifyUser { get; set; }//（核稿人）
        public DateTime EditTime { get; set; }//（归档）编辑时间
        public string EditIp { get; set; }//（归档）编辑IP:端口号
        public string EditUser { get; set; }//（归档）编辑人
        public string SignMan { get; set; }//签发人
        public DateTime SubTime { get; set; }//提交时间
        public string SubIp { get; set; }//提交IP:端口号
        public string ActiveName { get; set; }//录用情况：删除，暂存，提交，已录用，留存/未录用
        public string VerifyInfo { get; set; }//审核意见：12种不立案的情况
        public int UserId { get; set; }//用户Id
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
        public string tbody { get; set; }//自定义行
        public string SubMan1 { get; set; }//第一提交人
        public string StateName { get; set; }//操作名称
        public string SubTimeText { get; set; }//提交时间文本
        //INNER JOIN
        public string UserCode { get; set; }//委员证号
        //
        private static string[] columnList = new[] {
            "Id", "OpNum", "OrgName", "SubType", "SubType2", "IsGood", "IsOpen", "OpenInfo", "SubManType", "Party", 
            "Committee", "Subsector", "StreetTeam", "SubMan", "SubMans", "SubMan2", "Linkman", "LinkmanInfo", "LinkmanParty", "LinkmanOrgName", 
            "LinkmanTel", "Summary", "Body", "Advise", "Files", "Remark", "Adopt1", "Give1", "Employ1", "Reply1", 
            "Adopt2", "Send2", "Give2", "Employ2", "Reply2", "Send3", "Give3", "Employ3", "Reply3", "VerifyTitle", 
            "VerifyBody", "VerifyAdvise", "VerifyTime", "VerifyIp", "VerifyUser", "EditTime", "EditIp", "EditUser", "SignMan", "SubTime", 
            "SubIp", "ActiveName", "VerifyInfo", "UserId", "AddTime", "AddIp", "AddUser", "UpTime", "UpIp", "UpUser"
            , "UserCode", "FeedId", "FeedTakeWay", "FeedAttitude", "FeedResult"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] {
            SqlDbType.Int, SqlDbType.VarChar, SqlDbType.NText, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NText, 
            SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.NText, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, 
            SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.NText, SqlDbType.Text, SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, 
            SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, SqlDbType.NVarChar, 
            SqlDbType.NText, SqlDbType.NText, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.DateTime, 
            SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.Int, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar
            , SqlDbType.VarChar, SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar
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
    public class WebOpinionPop
    {
        private const string TableName = "tb_Opinion_Pop";
        SqlDAC sqlDac;
        public WebOpinionPop()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }
        //
        #region 查询
        public DataOpinionPop[] GetData(int intId, string strFields = "")
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
            DataOpinionPop[] result = (DataOpinionPop[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataOpinionPop));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataOpinionPop[] GetDatas(string OpNum, string strFields = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@OpNum", SqlDbType.NVarChar, "OpNum", OpNum));
            SqlParameter[] sqlParaArray = list.ToArray();
            if (string.IsNullOrEmpty(strFields))
            {
                strFields = "*";
            }
            string strSql = string.Format("SELECT {1} FROM {0} WHERE OpNum=@OpNum", TableName, strFields);
            DataOpinionPop[] result = (DataOpinionPop[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataOpinionPop));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataOpinionPop[] GetDatas(DataOpinionPop data, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "", string strJoin = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strFromWhere = string.Format("FROM {0} AS o{1} WHERE ", TableName, strJoin);
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
            if (data.OpNum != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OpNum", SqlDbType.NVarChar, "OpNum", data.OpNum));
                strFromWhere += " AND o.OpNum=@OpNum";
            }
            if (!string.IsNullOrEmpty(data.OrgName))
            {
                list.Add(SqlParamHelper.AddParameter("@OrgName", SqlDbType.NVarChar, "OrgName", "%" + data.OrgName + "%"));
                strFromWhere += " AND OrgName LIKE @OrgName";
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
            if (data.IsOpen != null)
            {
                list.Add(SqlParamHelper.AddParameter("@IsOpen", SqlDbType.NVarChar, "IsOpen", data.IsOpen));
                strFromWhere += " AND o.IsOpen=@IsOpen";
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
                        strTmp += "(o.Party LIKE @" + tmp + " OR o.LinkmanParty LIKE @" + tmp + ")";
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
            if (!string.IsNullOrEmpty(data.SubMan))
            {//反映人
                //list.Add(SqlParamHelper.AddParameter("@SubMan", SqlDbType.NVarChar, "SubMan", data.SubMan));
                //list.Add(SqlParamHelper.AddParameter("@SubMans", SqlDbType.NVarChar, "SubMans", "%," + data.SubMan + ",%"));
                //strFromWhere += " AND (o.SubMan LIKE @SubMan OR o.SubMans LIKE @SubMans)";
                string strTmp = "";
                string[] arr = data.SubMan.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "SubMan_" + i.ToString();
                        string tmps = "SubMans_" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                        list.Add(SqlParamHelper.AddParameter("@" + tmps, SqlDbType.NVarChar, tmps, "%," + arr[i] + ",%"));
                        strTmp += string.Format("o.SubMan LIKE @{0} OR o.SubMans LIKE @{1} OR o.SubMan2 LIKE @{1}", tmp, tmps);
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(data.SubMan1))
                {//第一反映人
                    //list.Add(SqlParamHelper.AddParameter("@SubMan1", SqlDbType.NVarChar, "SubMan1", data.SubMan1));
                    //strFromWhere += " AND (o.SubMan=@SubMan1 OR o.Linkman=@SubMan1)";
                    string strTmp = "";
                    string[] arr = data.SubMan1.Split(',');
                    for (int i = 0; i < arr.Count(); i++)
                    {
                        if (arr[i] != "")
                        {
                            if (strTmp != "")
                            {
                                strTmp += " OR ";
                            }
                            string tmp = "SubMan1_" + i.ToString();
                            list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                            strTmp += string.Format("o.SubMan=@{0} OR o.Linkman=@{0}", tmp);
                        }
                    }
                    if (strTmp != "")
                    {
                        strFromWhere += " AND (" + strTmp + ")";
                    }
                }
                if (!string.IsNullOrEmpty(data.SubMans))
                {//联名人
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
                            strTmp += string.Format("o.SubMans LIKE @{0} OR o.SubMan2 LIKE @{1}", tmps);
                        }
                    }
                    if (strTmp != "")
                    {
                        strFromWhere += " AND (" + strTmp + ")";
                    }
                }
            }
            if (!string.IsNullOrEmpty(data.LinkmanInfo))
            {
                string strTmp = "";
                string[] arr = data.LinkmanInfo.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "LinkmanInfo" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, "%" + arr[i] + "%"));
                        strTmp += "o.LinkmanInfo LIKE @" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(data.LinkmanParty))
            {
                string strTmp = "";
                string[] arr = data.LinkmanParty.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (arr[i] != "")
                    {
                        if (strTmp != "")
                        {
                            strTmp += " OR ";
                        }
                        string tmp = "LinkmanParty" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, "%," + arr[i] + ",%"));
                        strTmp += "(o.Party LIKE @" + tmp + " OR o.LinkmanParty LIKE @" + tmp + ")";
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(data.LinkmanOrgName))
            {
                list.Add(SqlParamHelper.AddParameter("@LinkmanOrgName", SqlDbType.NVarChar, "LinkmanOrgName", data.LinkmanOrgName));
                strFromWhere += " AND o.LinkmanOrgName LIKE @LinkmanOrgName";
            }
            if (!string.IsNullOrEmpty(data.LinkmanTel))
            {
                list.Add(SqlParamHelper.AddParameter("@LinkmanTel", SqlDbType.VarChar, "LinkmanTel", data.LinkmanTel));
                strFromWhere += " AND o.LinkmanTel LIKE @LinkmanTel";
            }
            if (!string.IsNullOrEmpty(data.Summary))
            {
                list.Add(SqlParamHelper.AddParameter("@Summary", SqlDbType.NVarChar, "Summary", data.Summary));
                if (data.Summary.IndexOf("%") >= 0)
                {
                    strFromWhere += " AND o.Summary LIKE @Summary";
                }
                else
                {
                    strFromWhere += " AND o.Summary=@Summary";
                }
            }
            if (!string.IsNullOrEmpty(data.Body))
            {
                list.Add(SqlParamHelper.AddParameter("@Body", SqlDbType.NText, "Body", data.Body));
                strFromWhere += " AND o.Body LIKE @Body";
            }
            if (!string.IsNullOrEmpty(data.Advise))
            {
                list.Add(SqlParamHelper.AddParameter("@Advise", SqlDbType.NText, "Advise", data.Advise));
                strFromWhere += " AND o.Advise LIKE @Advise";
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
            string strOrder = " ORDER BY ";
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                strOrder += strOrderBy;
            }
            else
            {
                strOrder += "o.SubTime DESC, o.UpTime DESC, o.AddTime DESC";//o.OpNum DESC, 
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
                if (strFields.IndexOf("|") >= 0)
                {
                    strFields = strFields.Replace("|", ",o.");
                }
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
                if (pageSize <= 0)
                {
                    pageSize = 10000;//考虑数据库性能，只读取前10,000条数据
                }
                strSql = "SELECT TOP " + pageSize.ToString() + strFields + strFromWhere + strOrder;
            }
            SqlParameter[] sqlParaArray = list.ToArray();
            DataOpinionPop[] result = (DataOpinionPop[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataOpinionPop));
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
        public int Insert(DataOpinionPop data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataOpinionPop data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataOpinionPop data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.OpNum != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OpNum", SqlDbType.VarChar, "OpNum", data.OpNum));
            }
            if (data.OrgName != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OrgName", SqlDbType.NText, "OrgName", data.OrgName));
            }
            if (data.SubType != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubType", SqlDbType.NVarChar, "SubType", data.SubType));
            }
            if (data.SubType2 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubType2", SqlDbType.NVarChar, "SubType2", data.SubType2));
            }
            if (data.IsGood != null)
            {
                list.Add(SqlParamHelper.AddParameter("@IsGood", SqlDbType.NVarChar, "IsGood", data.IsGood));
            }
            if (data.IsOpen != null)
            {
                list.Add(SqlParamHelper.AddParameter("@IsOpen", SqlDbType.NVarChar, "IsOpen", data.IsOpen));
            }
            if (data.OpenInfo != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OpenInfo", SqlDbType.NVarChar, "OpenInfo", data.OpenInfo));
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
            if (data.SubMans != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubMans", SqlDbType.NText, "SubMans", data.SubMans));
            }
            if (data.SubMan2 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubMan2", SqlDbType.NText, "SubMan2", data.SubMan2));
            }
            if (data.Linkman != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Linkman", SqlDbType.NVarChar, "Linkman", data.Linkman));
            }
            if (data.LinkmanInfo != null)
            {
                list.Add(SqlParamHelper.AddParameter("@LinkmanInfo", SqlDbType.NText, "LinkmanInfo", data.LinkmanInfo));
            }
            if (data.LinkmanParty != null)
            {
                list.Add(SqlParamHelper.AddParameter("@LinkmanParty", SqlDbType.NText, "LinkmanParty", data.LinkmanParty));
            }
            if (data.LinkmanOrgName != null)
            {
                list.Add(SqlParamHelper.AddParameter("@LinkmanOrgName", SqlDbType.NText, "LinkmanOrgName", data.LinkmanOrgName));
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
            if (data.Advise != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Advise", SqlDbType.NText, "Advise", data.Advise));
            }
            if (data.Files != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Files", SqlDbType.Text, "Files", data.Files));
            }
            if (data.Remark != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Remark", SqlDbType.NText, "Remark", data.Remark));
            }
            if (data.Adopt1 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Adopt1", SqlDbType.NText, "Adopt1", data.Adopt1));
            }
            if (data.Give1 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Give1", SqlDbType.NText, "Give1", data.Give1));
            }
            if (data.Employ1 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Employ1", SqlDbType.NText, "Employ1", data.Employ1));
            }
            if (data.Reply1 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Reply1", SqlDbType.NText, "Reply1", data.Reply1));
            }
            if (data.Adopt2 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Adopt2", SqlDbType.NText, "Adopt2", data.Adopt2));
            }
            if (data.Send2 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Send2", SqlDbType.NText, "Send2", data.Send2));
            }
            if (data.Give2 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Give2", SqlDbType.NText, "Give2", data.Give2));
            }
            if (data.Employ2 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Employ2", SqlDbType.NText, "Employ2", data.Employ2));
            }
            if (data.Reply2 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Reply2", SqlDbType.NText, "Reply2", data.Reply2));
            }
            if (data.Send3 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Send3", SqlDbType.NText, "Send3", data.Send3));
            }
            if (data.Give3 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Give3", SqlDbType.NText, "Give3", data.Give3));
            }
            if (data.Employ3 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Employ3", SqlDbType.NText, "Employ3", data.Employ3));
            }
            if (data.Reply3 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Reply3", SqlDbType.NText, "Reply3", data.Reply3));
            }
            if (data.VerifyTitle != null)
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyTitle", SqlDbType.NVarChar, "VerifyTitle", data.VerifyTitle));
            }
            if (data.VerifyBody != null)
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyBody", SqlDbType.NText, "VerifyBody", data.VerifyBody));
            }
            if (data.VerifyAdvise != null)
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyAdvise", SqlDbType.NText, "VerifyAdvise", data.VerifyAdvise));
            }
            if (data.VerifyTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyTime", SqlDbType.DateTime, "VerifyTime", data.VerifyTime));
            }
            if (data.VerifyIp != null)
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyIp", SqlDbType.VarChar, "VerifyIp", data.VerifyIp));
            }
            if (data.VerifyUser != null)
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyUser", SqlDbType.NVarChar, "VerifyUser", data.VerifyUser));
            }
            if (data.EditTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@EditTime", SqlDbType.DateTime, "EditTime", data.EditTime));
            }
            if (data.EditIp != null)
            {
                list.Add(SqlParamHelper.AddParameter("@EditIp", SqlDbType.VarChar, "EditIp", data.EditIp));
            }
            if (data.EditUser != null)
            {
                list.Add(SqlParamHelper.AddParameter("@EditUser", SqlDbType.NVarChar, "EditUser", data.EditUser));
            }
            if (data.SignMan != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SignMan", SqlDbType.NVarChar, "SignMan", data.SignMan));
            }
            if (data.SubTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@SubTime", SqlDbType.DateTime, "SubTime", data.SubTime));
            }
            if (data.SubIp != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubIp", SqlDbType.VarChar, "SubIp", data.SubIp));
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
            return list.ToArray();
        }
        //修改状态
        public int UpdateActive(int intId, string ActiveName, string strIp = "", string strUser = "")
        {
            string strSql = string.Format("UPDATE {0} SET ActiveName=@ActiveName", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
            list.Add(SqlParamHelper.AddParameter("@ActiveName", SqlDbType.NVarChar, "ActiveName", ActiveName));
            if (!string.IsNullOrEmpty(strUser))
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyTime", SqlDbType.DateTime, "VerifyTime", DateTime.Now));
                list.Add(SqlParamHelper.AddParameter("@VerifyIp", SqlDbType.VarChar, "VerifyIp", strIp));
                list.Add(SqlParamHelper.AddParameter("@VerifyUser", SqlDbType.NVarChar, "VerifyUser", strUser));
                strSql += ", VerifyTime=@VerifyTime, VerifyIp=@VerifyIp, VerifyUser=@VerifyUser";
            }
            strSql += " WHERE Id=@Id";
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        #endregion
    }
    //
}