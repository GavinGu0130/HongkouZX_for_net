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
    public class DataUser : MSBaseData
    {
        public int Id { get; set; }//主键
        public string UserType { get; set; }//用户类别：委员，党派团体，专委会，界别，街道活动组，小组、联组，其他
        public string UserName { get; set; }//用户名
        public string UserPwd { get; set; }//密码（MD5加密）
        public string Period { get; set; }//_届：十四、十三
        public string UserCode { get; set; }//委员编号
        public string TrueName { get; set; }//委员姓名
        public string UserSex { get; set; }//性别
        public string Native { get; set; }//籍贯
        public string Nation { get; set; }//民族：
        public string Education { get; set; }//学历（文化程度）
        public string IdCard { get; set; }//身份证号（des加密保存）
        public string Photo { get; set; }//照片
        public DateTime Birthday { get; set; }//出生日期
        public DateTime PostDate { get; set; }//委员任职日期
        public int PostNum { get; set; }//连任届数
        public string Party { get; set; }//政治面貌：中共党员，共青团员，民革党员，民盟盟员，民建会员，民进会员，农工党党员，致公党党员，九三学社社员，台盟盟员，无党派人士，群众
        public string HkMacaoTw { get; set; }//港澳台委员：香港委员，澳门委员，台湾委员
        public string Role { get; set; }//政协职务：主席，秘书长（副秘书长），常委，专委会主任，界别召集人，街道活动组组长，特邀监督员
        public string Committee { get; set; }//专委会
        public string CommitteeDuty { get; set; }//专委会职务：主任
        public string Committee2 { get; set; }//专委会2
        public string Committee2Duty { get; set; }//专委会2职务：主任
        public string Subsector { get; set; }//界别：26个
        public string SubsectorDuty { get; set; }//界别职务
        public string Subsector2 { get; set; }//界别活动组：22个
        public string StreetTeam { get; set; }//街道活动组
        public string StreetTeamDuty { get; set; }//街道活动组职务
        public string OrgName { get; set; }//工作单位
        public string OrgDuty { get; set; }//单位职务
        public string OrgPost { get; set; }//职称
        public string OrgType { get; set; }//单位性质：机关单位，事业单位，民非组织，国有企业，国有控股企业，外资企业，合资企业，私营企业，自由职业者
        public string IsSystem { get; set; }//单位体制内：体制内，体制外
        public string OrgAddress { get; set; }//单位地址
        public string OrgZip { get; set; }//单位邮编
        public string OrgTel { get; set; }//单位电话
        public string SocietyDuty { get; set; }//社会职务
        public string HomeAddress { get; set; }//家庭地址
        public string HomeZip { get; set; }//家庭邮编
        public string HomeTel { get; set; }//家庭电话
        public string ContactAddress { get; set; }//通讯地址
        public string ContactZip { get; set; }//通讯邮编
        public string Mobile { get; set; }//手机
        public string Email { get; set; }//邮箱
        public string WeChat { get; set; }//微信号
        public string Religion { get; set; }//宗教信仰
        public string Honor { get; set; }//荣誉称号
        public string CheckText { get; set; }//待审核信息：字段名:修改后文本\n字段名:修改后文本
        public string Remark { get; set; }//备注
        public int Active { get; set; }//状态：大于0时生效
        public DateTime AddTime { get; set; }//添加时间：默认为当前时间
        public string AddIp { get; set; }//添加IP:端口号
        public string AddUser { get; set; }//添加人
        public DateTime UpTime { get; set; }//修改时间：默认为当前时间
        public string UpIp { get; set; }//修改IP:端口号
        public string UpUser { get; set; }//修改人
        public DateTime LastTime { get; set; }//最后登录时间
        public string LastIp { get; set; }//最后登录IP:端口号
        public int ErrNum { get; set; }//密码错误次数
        public int OrderScore { get; set; }//积分排名：-1为不排名(不记分人员)
        public DateTime OrderTime { get; set; }//排名时间
        public string OrderColor { get; set; }//排名颜色
        public int OldId { get; set; }//瑞饶表Id
        //INNER JOIN
        public int UserSubNum { get; set; }//用户提交数：提案
        //分页统计
        public string rowClass { get; set; }//行class属性
        public int num { get; set; }//序号
        public int total { get; set; }//统计数
        public string other { get; set; }//其它说明
        public string tbody { get; set; }//自定义行
        public string WxOpenId { get; set; }//微信openid
        public string ActiveName { get; set; }//状态名称
        public string LastTimeText { get; set; }//登录时间文本
        public string BirthdayText { get; set; }//出生日期文本
        public string PostDateText { get; set; }//任职日期文本
        public string ContactTel { get; set; }//联系电话
        public string PerformFeedActive { get; set; }//履职反馈状态
        public int PerformNum { get; set; }//履职次数
        public string IsInvited { get; set; }//是否特邀监督员
        public string IsPresent { get; set; }//是否出席
        public string IsSpeak { get; set; }//是否发言
        public string IsProvide { get; set; }//是否提供资源
        public int PlatformNum { get; set; }//上台交流次数
        public int WriteNum { get; set; }//书面交流次数
        public int SpeakNum { get; set; }//其他会议发言
        public decimal UserScore { get; set; }//委员总积分
        public decimal UserScore1 { get; set; }//会议活动得分
        public decimal UserScore2 { get; set; }//建言得分
        public string UserType2 { get; set; }//用户类别：在册委员
        //
        private static string[] columnList = new[] {
            "Id", "UserType", "UserName", "UserPwd", "Period", "UserCode", "TrueName", "UserSex", "Native", "Nation", 
            "Education", "IdCard", "Photo", "Birthday", "PostDate", "PostNum", "Party", "HkMacaoTw", "Role", "Committee", 
            "CommitteeDuty", "Committee2", "Committee2Duty", "Subsector", "SubsectorDuty", "Subsector2", "StreetTeam", "StreetTeamDuty", "OrgName", "OrgDuty", 
            "OrgPost", "OrgType", "IsSystem", "OrgAddress", "OrgZip", "OrgTel", "SocietyDuty", "HomeAddress", "HomeZip", "HomeTel", 
            "ContactAddress", "ContactZip", "Mobile", "Email", "WeChat", "Religion", "Honor", "CheckText", "Remark", "Active", 
            "AddTime", "AddIp", "AddUser", "UpTime", "UpIp", "UpUser", "LastTime", "LastIp", "ErrNum", "OrderScore", 
            "OrderTime", "OrderColor", "OldId"
            , "UserSubNum"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] {
            SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, 
            SqlDbType.NVarChar, SqlDbType.VarChar, SqlDbType.Text, SqlDbType.DateTime, SqlDbType.DateTime, SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, 
            SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.NVarChar, 
            SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.VarChar, SqlDbType.VarChar, 
            SqlDbType.NText, SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.Text, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.NText, SqlDbType.NText, SqlDbType.Int, 
            SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.Int, SqlDbType.Int, 
            SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.Int
            , SqlDbType.Int
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
    public class WebUser
    {
        private const string TableName = "tb_User";
        SqlDAC sqlDac;
        public WebUser()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }

        #region 用户登录
        public DataUser[] Login(string LastIp, DateTime LastTime, string UserPwd, string UserName, string TrueName = "", string strPeriod = "")
        {
            string strSql = string.Format("SELECT * FROM {0} WHERE UserPwd=@UserPwd", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@UserPwd", SqlDbType.VarChar, "UserPwd", UserPwd));
            if (!string.IsNullOrEmpty(TrueName))
            {
                list.Add(SqlParamHelper.AddParameter("@TrueName", SqlDbType.NVarChar, "TrueName", TrueName));
                strSql += " AND TrueName=@TrueName";
            }
            else
            {
                list.Add(SqlParamHelper.AddParameter("@UserName", SqlDbType.NVarChar, "UserName", UserName));
                strSql += " AND UserName=@UserName";
            }
            if (!string.IsNullOrEmpty(strPeriod))
            {
                list.Add(SqlParamHelper.AddParameter("@Period", SqlDbType.NVarChar, "Period", strPeriod));
                strSql += " AND Period=@Period";
            }
            SqlParameter[] sqlParameters = list.ToArray();
            DataUser[] result = (DataUser[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataUser));
            if (result != null && result.Length > 0)
            {
                if (result[0].ErrNum > 10)
                {
                    updateErrNum(result[0].Id);//增加错误次数
                }
                else if (result[0].Active > 0)
                {
                    UpdateLogin(result[0].Id, LastIp, LastTime);//更新登录信息
                    result[0].LastTime = LastTime;
                }
                return result;
            }
            else
            {
                updateErrNum(0, UserName, TrueName);//增加错误次数
            }
            return null;
        }
        //更新登录信息
        public int UpdateLogin(int Id, string LastIp, DateTime LastTime)
        {
            string strSql = string.Format("UPDATE {0} SET ErrNum=0, LastTime=@LastTime, LastIp=@LastIp WHERE Id=@Id", TableName);
            SqlParameter[] sqlParaArray = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", Id),
                SqlParamHelper.AddParameter("@LastTime", SqlDbType.DateTime, "LastTime", LastTime),
                SqlParamHelper.AddParameter("@LastIp", SqlDbType.VarChar, "LastIp", LastIp),
            };
            return sqlDac.UpdateQuery(strSql, sqlParaArray);
        }
        //更新登录错误次数
        private void updateErrNum(int Id, string UserName = "", string TrueName = "")
        {
            string strSql = string.Format("UPDATE {0} SET ErrNum=ErrNum+1 WHERE ", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            if (Id > 0)
            {
                list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", Id));
                strSql += "Id=@Id";
            }
            else if (!string.IsNullOrEmpty(TrueName))
            {
                list.Add(SqlParamHelper.AddParameter("@TrueName", SqlDbType.NVarChar, "TrueName", TrueName));
                strSql += "TrueName=@TrueName";
            }
            else
            {
                list.Add(SqlParamHelper.AddParameter("@UserName", SqlDbType.NVarChar, "UserName", UserName));
                strSql += "UserName=@UserName";
            }
            SqlParameter[] sqlParameters = list.ToArray();
            sqlDac.UpdateQuery(strSql, sqlParameters);
        }
        #endregion

        #region 查询
        public DataUser[] GetData(int intId, string strFields = "")
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
            DataUser[] result = (DataUser[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataUser));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataUser[] GetData(string UserCode, string strFields = "")
        {
            SqlParameter[] sqlParameters = new[] 
            { 
               new SqlParameter("@UserCode", SqlDbType.VarChar)
            };
            sqlParameters[0].Value = UserCode;
            if (string.IsNullOrEmpty(strFields))
            {
                strFields = "*";
            }
            string strSql = string.Format("SELECT {1} FROM {0} WHERE UserCode=@UserCode", TableName, strFields);
            DataUser[] result = (DataUser[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataUser));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataUser[] GetDatas(string Period, string TrueName, string strFields = "")
        {
            SqlParameter[] sqlParameters = new[] 
            { 
               new SqlParameter("@Period", SqlDbType.NVarChar),
               new SqlParameter("@TrueName", SqlDbType.NVarChar)
            };
            sqlParameters[0].Value = Period;
            sqlParameters[1].Value = TrueName;
            if (string.IsNullOrEmpty(strFields))
            {
                strFields = "*";
            }
            string strSql = string.Format("SELECT {1} FROM {0} WHERE Period=@Period AND TrueName=@TrueName", TableName, strFields);
            DataUser[] result = (DataUser[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataUser));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataUser[] GetDatas(DataUser data, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strFromWhere = string.Format("FROM {0} WHERE ", TableName);
            if (!string.IsNullOrEmpty(data.ActiveName))
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
                        if (arr[i].IndexOf("<") >= 0 || arr[i].IndexOf(">") >= 0)
                        {
                            strTmp += "Active" + arr[i];
                        }
                        else
                        {
                            string tmp = "Active" + i.ToString();
                            list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.Int, tmp, Convert.ToInt16(arr[i])));
                            strTmp += "Active=@" + tmp;
                        }
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += "(" + strTmp + ")";
                }
                else
                {
                    strFromWhere += "1=1";
                }
            }
            else
            {
                strFromWhere += "1=1";
            }
            if (!string.IsNullOrEmpty(data.UserType))
            {
                list.Add(SqlParamHelper.AddParameter("@UserType", SqlDbType.NVarChar, "UserType", data.UserType));
                strFromWhere += " AND UserType=@UserType";
            }
            if (!string.IsNullOrEmpty(data.Period))
            {
                string strTmp = "";
                string[] arr = data.Period.Split(',');
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
                            strTmp += "Period NOT LIKE " + arr[i].Replace("<>", "");
                        }
                        else
                        {
                            string tmp = "Period" + i.ToString();
                            list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                            strTmp += "Period=@" + tmp;
                        }
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(data.UserCode))
            {
                list.Add(SqlParamHelper.AddParameter("@UserCode", SqlDbType.VarChar, "UserCode", data.UserCode));
                strFromWhere += " AND UserCode LIKE @UserCode";
            }
            if (!string.IsNullOrEmpty(data.TrueName))
            {
                list.Add(SqlParamHelper.AddParameter("@TrueName", SqlDbType.NVarChar, "TrueName", data.TrueName));
                strFromWhere += " AND TrueName LIKE @TrueName";
            }
            if (!string.IsNullOrEmpty(data.UserSex))
            {
                list.Add(SqlParamHelper.AddParameter("@UserSex", SqlDbType.NVarChar, "UserSex", data.UserSex));
                strFromWhere += " AND UserSex=@UserSex";
            }
            if (!string.IsNullOrEmpty(data.Native))
            {
                list.Add(SqlParamHelper.AddParameter("@Native", SqlDbType.NVarChar, "Native", data.Native));
                strFromWhere += " AND Native LIKE @Native";
            }
            if (!string.IsNullOrEmpty(data.Nation))
            {
                list.Add(SqlParamHelper.AddParameter("@Nation", SqlDbType.NVarChar, "Nation", data.Nation));
                strFromWhere += " AND Nation LIKE @Nation";
            }
            if (!string.IsNullOrEmpty(data.BirthdayText) && data.BirthdayText.IndexOf(",") >= 0)
            {
                string[] arr = data.BirthdayText.Split(',');
                if (!string.IsNullOrEmpty(arr[0]))
                {
                    list.Add(SqlParamHelper.AddParameter("@Birthday1", SqlDbType.DateTime, "Birthday1", Convert.ToDateTime(arr[0])));
                    strFromWhere += " AND Birthday>=@Birthday1";
                }
                if (!string.IsNullOrEmpty(arr[1]))
                {
                    if (arr[1].IndexOf(":") < 0)
                    {
                        arr[1] += " 23:59:59";
                    }
                    list.Add(SqlParamHelper.AddParameter("@Birthday2", SqlDbType.DateTime, "Birthday2", Convert.ToDateTime(arr[1])));
                    strFromWhere += " AND Birthday<=@Birthday2";
                }
            }
            if (!string.IsNullOrEmpty(data.PostDateText) && data.PostDateText.IndexOf(",") >= 0)
            {
                string[] arr = data.PostDateText.Split(',');
                if (!string.IsNullOrEmpty(arr[0]))
                {
                    list.Add(SqlParamHelper.AddParameter("@PostDate1", SqlDbType.DateTime, "PostDate1", Convert.ToDateTime(arr[0])));
                    strFromWhere += " AND PostDate>=@PostDate1";
                }
                if (!string.IsNullOrEmpty(arr[1]))
                {
                    if (arr[1].IndexOf(":") < 0)
                    {
                        arr[1] += " 23:59:59";
                    }
                    list.Add(SqlParamHelper.AddParameter("@PostDate2", SqlDbType.DateTime, "PostDate2", Convert.ToDateTime(arr[1])));
                    strFromWhere += " AND PostDate<=@PostDate2";
                }
            }
            if (!string.IsNullOrEmpty(data.Education))
            {
                list.Add(SqlParamHelper.AddParameter("@Education", SqlDbType.NVarChar, "Education", data.Education));
                strFromWhere += " AND Education LIKE @Education";
            }
            if (!string.IsNullOrEmpty(data.Party))
            {
                list.Add(SqlParamHelper.AddParameter("@Party", SqlDbType.NVarChar, "Party", data.Party));
                strFromWhere += " AND Party LIKE @Party";
            }
            if (!string.IsNullOrEmpty(data.Role))
            {
                string strTmp = "";
                if (data.Role.IndexOf("|") >= 0)
                {
                    data.Role = data.Role.Replace("|", ",");
                }
                string[] arr = data.Role.Split(',');
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
                            strTmp += "Role" + arr[i];
                        }
                        else
                        {
                            string tmp = "Role" + i.ToString();
                            list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, "%," + arr[i] + ",%"));
                            strTmp += "Role LIKE @" + tmp;
                        }
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
            }
            if (!string.IsNullOrEmpty(data.Committee))
            {
                list.Add(SqlParamHelper.AddParameter("@Committee", SqlDbType.NVarChar, "Committee", data.Committee));
                if (data.Committee.IndexOf("%") >= 0)
                {
                    strFromWhere += " AND (Committee LIKE @Committee OR Committee2 LIKE @Committee)";
                }
                else
                {
                    strFromWhere += " AND (Committee=@Committee OR Committee2=@Committee)";
                }
            }
            if (!string.IsNullOrEmpty(data.Subsector))
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
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, arr[i]));
                        strTmp += "Subsector=@" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += " AND (" + strTmp + ")";
                }
                //list.Add(SqlParamHelper.AddParameter("@Subsector", SqlDbType.NVarChar, "Subsector", data.Subsector));
                //strFromWhere += " AND Subsector=@Subsector";
            }
            //if (!string.IsNullOrEmpty(data.Subsector2))
            //{
            //    list.Add(SqlParamHelper.AddParameter("@Subsector2", SqlDbType.NVarChar, "Subsector2", data.Subsector2));
            //    strFromWhere += " AND Subsector2=@Subsector2";
            //}
            if (!string.IsNullOrEmpty(data.StreetTeam))
            {
                list.Add(SqlParamHelper.AddParameter("@StreetTeam", SqlDbType.NVarChar, "StreetTeam", data.StreetTeam));
                strFromWhere += " AND StreetTeam=@StreetTeam";
            }
            if (!string.IsNullOrEmpty(data.IsSystem))
            {
                list.Add(SqlParamHelper.AddParameter("@IsSystem", SqlDbType.NVarChar, "IsSystem", data.IsSystem));
                strFromWhere += " AND IsSystem=@IsSystem";
            }
            if (!string.IsNullOrEmpty(data.OrgType))
            {
                list.Add(SqlParamHelper.AddParameter("@OrgType", SqlDbType.NVarChar, "OrgType", data.OrgType));
                strFromWhere += " AND OrgType=@OrgType";
            }
            if (!string.IsNullOrEmpty(data.OrgName))
            {
                list.Add(SqlParamHelper.AddParameter("@OrgName", SqlDbType.NVarChar, "OrgName", data.OrgName));
                strFromWhere += " AND OrgName LIKE @OrgName";
            }
            if (!string.IsNullOrEmpty(data.ContactTel))
            {
                list.Add(SqlParamHelper.AddParameter("@ContactTel", SqlDbType.VarChar, "ContactTel", data.ContactTel));
                strFromWhere += " AND (Mobile LIKE @ContactTel OR OrgTel LIKE @ContactTel OR HomeTel LIKE @ContactTel)";
            }
            if (!string.IsNullOrEmpty(data.Email))
            {
                list.Add(SqlParamHelper.AddParameter("@Email", SqlDbType.VarChar, "Email", data.Email));
                strFromWhere += " AND Email LIKE @Email";
            }
            if (data.OrderScore > 0)
            {
                strFromWhere += " AND OrderScore>=0";
            }
            if (!string.IsNullOrEmpty(data.CheckText))
            {
                strFromWhere += " AND CheckText NOT LIKE ''";
            }
            string strOrder = " ORDER BY ";
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                strOrder += strOrderBy;
            }
            else
            {
                strOrder += "UserCode ASC";
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
            DataUser[] result = (DataUser[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataUser));
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
        public DataUser[] GetDatas(DataOpinion data, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strFromWhere = string.Format("FROM {0} AS u0 WHERE u0.Active>0", TableName);
            if (!string.IsNullOrEmpty(data.SubManType))
            {
                list.Add(SqlParamHelper.AddParameter("@UserType", SqlDbType.NVarChar, "UserType", data.SubManType));
                strFromWhere += " AND u0.UserType=@UserType";
            }
            if (!string.IsNullOrEmpty(data.UserCode))
            {
                list.Add(SqlParamHelper.AddParameter("@UserCode", SqlDbType.NVarChar, "UserCode", data.UserCode));
                strFromWhere += " AND u0.UserCode LIKE @UserCode";
            }
            if (!string.IsNullOrEmpty(data.Period))
            {
                list.Add(SqlParamHelper.AddParameter("@UserPeriod", SqlDbType.NVarChar, "UserPeriod", data.Period));
                strFromWhere += " AND u0.Period=@UserPeriod";
            }
            if (!string.IsNullOrEmpty(data.SubMan))
            {
                list.Add(SqlParamHelper.AddParameter("@TrueName", SqlDbType.NVarChar, "TrueName", data.SubMan));
                strFromWhere += " AND u0.TrueName=@TrueName";
            }
            if (!string.IsNullOrEmpty(data.UserSex))
            {
                list.Add(SqlParamHelper.AddParameter("@UserSex", SqlDbType.NVarChar, "UserSex", data.UserSex));
                strFromWhere += " AND u0.UserSex=@UserSex";
            }
            if (!string.IsNullOrEmpty(data.UserParty))
            {
                list.Add(SqlParamHelper.AddParameter("@Party", SqlDbType.NVarChar, "Party", data.UserParty));
                strFromWhere += " AND u0.Party=@Party";
            }
            if (!string.IsNullOrEmpty(data.UserCommittee))
            {
                list.Add(SqlParamHelper.AddParameter("@Committee", SqlDbType.NVarChar, "Committee", data.UserCommittee));
                strFromWhere += " AND (u0.Committee=@Committee OR u0.Committee2=@Committee)";
            }
            if (!string.IsNullOrEmpty(data.UserSubsector))
            {
                list.Add(SqlParamHelper.AddParameter("@Subsector", SqlDbType.NVarChar, "Subsector", data.UserSubsector));
                strFromWhere += " AND u0.Subsector=@Subsector";
            }
            if (!string.IsNullOrEmpty(data.UserStreetTeam))
            {
                list.Add(SqlParamHelper.AddParameter("@StreetTeam", SqlDbType.NVarChar, "StreetTeam", data.UserStreetTeam));
                strFromWhere += " AND u0.StreetTeam=@StreetTeam";
            }
            string strOrder = " ORDER BY ";
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                strOrder += strOrderBy;
            }
            else
            {
                strOrder += "u2.UserSubNum DESC, u.UserCode ASC";//, u.UpTime DESC, u.AddTime ASC
            }
            if (strOrderBy.IndexOf("Id ASC") < 0 && strOrderBy.IndexOf("Id DESC") < 0)
            {
                strOrder += ", u.Id ASC";
            }
            if (strFields == "")
            {
                strFields = " u.* ";
            }
            else
            {
                strFields = " " + strFields + " ";
            }
            string strOpinion = "(o.ActiveName='归并' OR o.ActiveName='立案')";
            if (!string.IsNullOrEmpty(data.Period))
            {
                list.Add(SqlParamHelper.AddParameter("@Period", SqlDbType.NVarChar, "Period", data.Period));
                strOpinion += " AND o.Period=@Period";
            }
            if (!string.IsNullOrEmpty(data.Times))
            {
                list.Add(SqlParamHelper.AddParameter("@Times", SqlDbType.NVarChar, "Times", data.Times));
                strOpinion += " AND o.Times=@Times";
            }
            if (data.IsSubMan1)
            {
                strOpinion += " AND o.SubMan LIKE '%,' + u0.TrueName + ',%'";
            }
            else
            {
                strOpinion += " AND (o.SubMan LIKE '%,' + u0.TrueName + ',%' OR o.SubMans LIKE '%,' + u0.TrueName + ',%')";
            }
            string strUserSubNum = "";
            if (data.UserSubNum >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@UserSubNum", SqlDbType.Int, "UserSubNum", data.UserSubNum));
                strUserSubNum += " AND u2.UserSubNum>=@UserSubNum";
            }
            if (data.UserSubMin > 0)
            {
                list.Add(SqlParamHelper.AddParameter("@UserSubMin", SqlDbType.Int, "UserSubMin", data.UserSubMin));
                strUserSubNum += " AND u2.UserSubNum<@UserSubMin";
            }
            string strSql = "";
            if (pageSize > 0 && intPage > 0)
            {
                //分页查询语句：SELECT TOP 页大小 * FROM table WHERE id NOT IN ( SELECT TOP 页大小*(页数-1) id FROM table ORDER BY id ) ORDER BY id
                //strSql = "SELECT TOP " + pageSize.ToString() + strFields + strFromWhere + " AND u.Id NOT IN ( SELECT TOP " + (pageSize * (intPage - 1)).ToString() + " u.Id " + strFromWhere + strOrder + " )" + strOrder;
                //strSql = "SELECT TOP 10 u2.UserSubNum, u.* FROM tb_User AS u, ( SELECT row_number() over(ORDER BY u2.UserSubNum DESC, u.UserCode ASC, u.UpTime DESC, u.AddTime ASC, u.Id ASC) as rownumber, u2.UserSubNum, u.Id FROM tb_User AS u, ( SELECT (SELECT COUNT(o.Id) FROM tb_Opinion AS o WHERE o.SubMan LIKE '%,' + u0.TrueName + ',%') AS UserSubNum, u0.Id FROM tb_User AS u0 WHERE u0.UserType='委员' AND u0.Active>0) AS u2 WHERE u.Id=u2.Id AND u2.UserSubNum>0 ) AS u2 WHERE u.Id=u2.Id AND u2.UserSubNum>0 AND rownumber>0 ORDER BY u2.UserSubNum DESC, u.UserCode ASC, u.UpTime DESC, u.AddTime ASC, u.Id ASC";
                strSql = string.Format("SELECT TOP " + pageSize.ToString() + " u2.UserSubNum, u.* FROM tb_User AS u, ("
                    + "SELECT row_number() over(" + strOrder + ") as rownumber, u2.UserSubNum, u.Id FROM tb_User AS u, ("
                    + "SELECT (SELECT COUNT(o.Id) FROM tb_Opinion AS o WHERE " + strOpinion + ") AS UserSubNum, u0.Id "
                    + strFromWhere + ") AS u2 WHERE u.Id=u2.Id" + strUserSubNum
                    + ") AS u2 WHERE u.Id=u2.Id" + strUserSubNum + " AND rownumber>" + (pageSize * (intPage - 1)).ToString() + strOrder, TableName);
            }
            else
            {
                return null;
                //if (pageSize <= 0)
                //{
                //    pageSize = 100000;//考虑数据库性能，只读取前100,000条数据
                //}
                //strSql = "SELECT TOP " + pageSize.ToString() + strFields + strFromWhere + strOrder;
            }
            //HttpContext.Current.Response.Write(strSql); HttpContext.Current.Response.End();
            SqlParameter[] sqlParaArray = list.ToArray();
            DataUser[] result = (DataUser[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataUser));
            if (result != null && result.Length > 0)
            {
                if (strFilter.IndexOf("total") >= 0)
                {
                    strSql = "SELECT COUNT(u.Id) FROM tb_User AS u, (SELECT (SELECT COUNT(o.Id) FROM tb_Opinion AS o WHERE " + strOpinion + ") AS UserSubNum, u0.Id " + strFromWhere + ") AS u2 WHERE u.Id=u2.Id" + strUserSubNum;
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

        #region 修改
        //修改用户密码
        public int SetUserPwd(int Id, string NewPwd, string OldPwd = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", Id));
            list.Add(SqlParamHelper.AddParameter("@NewPwd", SqlDbType.VarChar, "NewPwd", NewPwd));
            string strSql = string.Format("UPDATE {0} SET UserPwd=@NewPwd WHERE Id=@Id", TableName);
            if (!string.IsNullOrEmpty(OldPwd))
            {
                list.Add(SqlParamHelper.AddParameter("@OldPwd", SqlDbType.VarChar, "OldPwd", OldPwd));
                strSql += " AND UserPwd=@OldPwd";
            }
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        //插入
        public int Insert(DataUser data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataUser data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataUser data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.UserType != null)
            {
                list.Add(SqlParamHelper.AddParameter("@UserType", SqlDbType.NVarChar, "UserType", data.UserType));
            }
            if (data.UserName != null)
            {
                list.Add(SqlParamHelper.AddParameter("@UserName", SqlDbType.NVarChar, "UserName", data.UserName));
            }
            if (data.UserPwd != null)
            {
                list.Add(SqlParamHelper.AddParameter("@UserPwd", SqlDbType.VarChar, "UserPwd", data.UserPwd));
            }
            if (data.Period != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Period", SqlDbType.NVarChar, "Period", data.Period));
            }
            if (data.UserCode != null)
            {
                list.Add(SqlParamHelper.AddParameter("@UserCode", SqlDbType.VarChar, "UserCode", data.UserCode));
            }
            if (data.TrueName != null)
            {
                list.Add(SqlParamHelper.AddParameter("@TrueName", SqlDbType.NVarChar, "TrueName", data.TrueName));
            }
            if (data.UserSex != null)
            {
                list.Add(SqlParamHelper.AddParameter("@UserSex", SqlDbType.NVarChar, "UserSex", data.UserSex));
            }
            if (data.Native != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Native", SqlDbType.NVarChar, "Native", data.Native));
            }
            if (data.Nation != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Nation", SqlDbType.NVarChar, "Nation", data.Nation));
            }
            if (data.Education != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Education", SqlDbType.NVarChar, "Education", data.Education));
            }
            if (data.IdCard != null)
            {
                list.Add(SqlParamHelper.AddParameter("@IdCard", SqlDbType.VarChar, "IdCard", data.IdCard));
            }
            if (data.Photo != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Photo", SqlDbType.Text, "Photo", data.Photo));
            }
            if (data.Birthday > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@Birthday", SqlDbType.DateTime, "Birthday", data.Birthday));
            }
            else
            {
                list.Add(SqlParamHelper.AddParameter("@Birthday", SqlDbType.VarChar, "Birthday", null));
            }
            if (data.PostDate > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@PostDate", SqlDbType.DateTime, "PostDate", data.PostDate));
            }
            else
            {
                list.Add(SqlParamHelper.AddParameter("@PostDate", SqlDbType.VarChar, "PostDate", null));
            }
            if (data.PostNum >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@PostNum", SqlDbType.Int, "PostNum", data.PostNum));
            }
            if (data.Party != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Party", SqlDbType.NVarChar, "Party", data.Party));
            }
            if (data.HkMacaoTw != null)
            {
                list.Add(SqlParamHelper.AddParameter("@HkMacaoTw", SqlDbType.NVarChar, "HkMacaoTw", data.HkMacaoTw));
            }
            if (data.Role != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Role", SqlDbType.NText, "Role", data.Role));
            }
            if (data.Committee != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Committee", SqlDbType.NVarChar, "Committee", data.Committee));
            }
            if (data.CommitteeDuty != null)
            {
                list.Add(SqlParamHelper.AddParameter("@CommitteeDuty", SqlDbType.NVarChar, "CommitteeDuty", data.CommitteeDuty));
            }
            if (data.Committee2 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Committee2", SqlDbType.NVarChar, "Committee2", data.Committee2));
            }
            if (data.Committee2Duty != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Committee2Duty", SqlDbType.NVarChar, "Committee2Duty", data.Committee2Duty));
            }
            if (data.Subsector != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Subsector", SqlDbType.NVarChar, "Subsector", data.Subsector));
            }
            if (data.SubsectorDuty != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubsectorDuty", SqlDbType.NVarChar, "SubsectorDuty", data.SubsectorDuty));
            }
            if (data.Subsector2 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Subsector2", SqlDbType.NVarChar, "Subsector2", data.Subsector2));
            }
            if (data.StreetTeam != null)
            {
                list.Add(SqlParamHelper.AddParameter("@StreetTeam", SqlDbType.NVarChar, "StreetTeam", data.StreetTeam));
            }
            if (data.StreetTeamDuty != null)
            {
                list.Add(SqlParamHelper.AddParameter("@StreetTeamDuty", SqlDbType.NVarChar, "StreetTeamDuty", data.StreetTeamDuty));
            }
            if (data.OrgName != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OrgName", SqlDbType.NText, "OrgName", data.OrgName));
            }
            if (data.OrgDuty != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OrgDuty", SqlDbType.NVarChar, "OrgDuty", data.OrgDuty));
            }
            if (data.OrgPost != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OrgPost", SqlDbType.NVarChar, "OrgPost", data.OrgPost));
            }
            if (data.OrgType != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OrgType", SqlDbType.NVarChar, "OrgType", data.OrgType));
            }
            if (data.IsSystem != null)
            {
                list.Add(SqlParamHelper.AddParameter("@IsSystem", SqlDbType.NVarChar, "IsSystem", data.IsSystem));
            }
            if (data.OrgAddress != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OrgAddress", SqlDbType.NText, "OrgAddress", data.OrgAddress));
            }
            if (data.OrgZip != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OrgZip", SqlDbType.VarChar, "OrgZip", data.OrgZip));
            }
            if (data.OrgTel != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OrgTel", SqlDbType.VarChar, "OrgTel", data.OrgTel));
            }
            if (data.SocietyDuty != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SocietyDuty", SqlDbType.NVarChar, "SocietyDuty", data.SocietyDuty));
            }
            if (data.HomeAddress != null)
            {
                list.Add(SqlParamHelper.AddParameter("@HomeAddress", SqlDbType.NText, "HomeAddress", data.HomeAddress));
            }
            if (data.HomeZip != null)
            {
                list.Add(SqlParamHelper.AddParameter("@HomeZip", SqlDbType.VarChar, "HomeZip", data.HomeZip));
            }
            if (data.HomeTel != null)
            {
                list.Add(SqlParamHelper.AddParameter("@HomeTel", SqlDbType.VarChar, "HomeTel", data.HomeTel));
            }
            if (data.ContactAddress != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ContactAddress", SqlDbType.NText, "ContactAddress", data.ContactAddress));
            }
            if (data.ContactZip != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ContactZip", SqlDbType.VarChar, "ContactZip", data.ContactZip));
            }
            if (data.Mobile != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Mobile", SqlDbType.VarChar, "Mobile", data.Mobile));
            }
            if (data.Email != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Email", SqlDbType.Text, "Email", data.Email));
            }
            if (data.WeChat != null)
            {
                list.Add(SqlParamHelper.AddParameter("@WeChat", SqlDbType.VarChar, "WeChat", data.WeChat));
            }
            if (data.Religion != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Religion", SqlDbType.NVarChar, "Religion", data.Religion));
            }
            if (data.Honor != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Honor", SqlDbType.NText, "Honor", data.Honor));
            }
            if (data.CheckText != null)
            {
                list.Add(SqlParamHelper.AddParameter("@CheckText", SqlDbType.NText, "CheckText", data.CheckText));
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
            //if (data.LastTime > DateTime.MinValue)
            //{
            //    list.Add(SqlParamHelper.AddParameter("@LastTime", SqlDbType.DateTime, "LastTime", data.LastTime));
            //}
            //if (data.LastIp != null)
            //{
            //    list.Add(SqlParamHelper.AddParameter("@LastIp", SqlDbType.VarChar, "LastIp", data.LastIp));
            //}
            if (data.ErrNum >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@ErrNum", SqlDbType.Int, "ErrNum", data.ErrNum));
            }
            if (data.OrderScore > -1000)
            {
                list.Add(SqlParamHelper.AddParameter("@OrderScore", SqlDbType.Int, "OrderScore", data.OrderScore));
            }
            if (data.OrderTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@OrderTime", SqlDbType.DateTime, "OrderTime", data.OrderTime));
            }
            if (data.OrderColor != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OrderColor", SqlDbType.VarChar, "OrderColor", data.OrderColor));
            }
            if (data.OldId >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@OldId", SqlDbType.Int, "OldId", data.OldId));
            }
            return list.ToArray();
        }
        //修改状态
        public int UpdateActive(int intId, int Active)
        {
            string strSql = string.Format("UPDATE {0} SET Active=@Active WHERE Id=@Id", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
            list.Add(SqlParamHelper.AddParameter("@Active", SqlDbType.Int, "Active", Active));
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        //申请修改用户信息
        public int UpdateCheck(int intId, string CheckText)
        {
            string strSql = string.Format("UPDATE {0} SET CheckText=@CheckText WHERE Id=@Id", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
            list.Add(SqlParamHelper.AddParameter("@CheckText", SqlDbType.NText, "CheckText", CheckText));
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        //积分排序
        public int UpdateOrder(int intId, int OrderScore, string OrderColor)
        {
            string strSql = string.Format("UPDATE {0} SET OrderScore=@OrderScore, OrderTime=@OrderTime, OrderColor=@OrderColor WHERE Id=@Id", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
            list.Add(SqlParamHelper.AddParameter("@OrderScore", SqlDbType.Int, "OrderScore", OrderScore));
            list.Add(SqlParamHelper.AddParameter("@OrderTime", SqlDbType.DateTime, "OrderTime", DateTime.Now));
            list.Add(SqlParamHelper.AddParameter("@OrderColor", SqlDbType.VarChar, "OrderColor", OrderColor));
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        #endregion
    }
    //
}