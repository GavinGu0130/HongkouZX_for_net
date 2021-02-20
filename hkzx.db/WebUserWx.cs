using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MS.Lib.Data;
using System.Data.SqlClient;

namespace hkzx.db
{
    [Serializable]
    public class DataUserWx : MSBaseData
    {
        public int Id { get; set; }//主键
        public int UserId { get; set; }//用户ID
        public string WxAppId { get; set; }//公众号ID
        public int WxSubscribe { get; set; }//关注公众号：1为是，0为否
        public string WxOpenId { get; set; }//微信用户的标识，对当前公众号唯一
        public string WxNickName { get; set; }//微信昵称
        public int WxSex { get; set; }//微信性别：默认1为男，2为女，0为未知
        public string WxLanguage { get; set; }//微信语言：zh_CN
        public string WxProvince { get; set; }//微信省/市
        public string WxCity { get; set; }//微信城市
        public string WxCountry { get; set; }//微信国家
        public string WxHeadImgUrl { get; set; }//微信用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空。若用户更换头像，原有头像URL将失效
        public int WxSubscribe_time { get; set; }//关注时间戳，标准北京时间，时区为东八区，自1970年1月1日 0点0分0秒以来的秒数。注意：部分系统取到的值为毫秒级，需要转换成秒(10位数字)
        public string WxPrivilege { get; set; }//微信用户特权
        public string WxUnionid { get; set; }//微信开放平台Id
        public string WxRemark { get; set; }//公众号运营者对粉丝的备注，公众号运营者可在微信公众平台用户管理界面对粉丝添加备注
        public int WxGroupid { get; set; }//用户所在的分组ID（兼容旧的用户分组接口）
        public string WxTagid_list { get; set; }//用户被打上的标签ID列表
        public string WxAccess_token { get; set; }//通过code换取的是一个特殊的网页授权access_token,与基础支持中的access_token（该access_token用于调用其他接口）不同。
        public DateTime WxAccess_expires_in { get; set; }//网页授权access_token过期时间
        public string WxRefresh_token { get; set; }//由于网页授权access_token拥有较短的有效期，当access_token超时后，可以使用refresh_token进行刷新，refresh_token有效期为30天，当refresh_token失效之后，需要用户重新授权。
        public DateTime WxRefresh_expires_in { get; set; }//网页授权refresh_token过期时间
        public int Active { get; set; }//状态排序，默认0为前台不显示
        public DateTime AddTime { get; set; }//注册时间，非空，默认为当前时间
        public string AddIp { get; set; }//注册IP:端口号
        public DateTime UpTime { get; set; }//最后登录时间，非空，默认为当前时间
        public string UpIp { get; set; }//最后登录IP:端口号
        //分页统计
        public string rowClass { get; set; }//行class属性
        public int num { get; set; }//序号
        public int total { get; set; }//统计数
        public string ActiveName { get; set; }//状态名称

        private static string[] columnList = new[] {
            "Id", "UserId", "WxAppId", "WxSubscribe", "WxOpenId", "WxNickName", "WxSex", "WxLanguage", "WxProvince", "WxCity", 
            "WxCountry", "WxHeadImgUrl", "WxSubscribe_time", "WxPrivilege", "WxUnionid", "WxRemark", "WxGroupid", "WxTagid_list", "WxAccess_token", "WxAccess_expires_in", 
            "WxRefresh_token", "WxRefresh_expires_in", "Active", "AddTime", "AddIp", "UpTime", "UpIp"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] {
            SqlDbType.Int, SqlDbType.Int, SqlDbType.VarChar, SqlDbType.Int, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.Int, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, 
            SqlDbType.NVarChar, SqlDbType.VarChar, SqlDbType.Int, SqlDbType.Text, SqlDbType.VarChar, SqlDbType.NText, SqlDbType.Int, SqlDbType.Text, SqlDbType.Text, SqlDbType.DateTime, 
            SqlDbType.Text, SqlDbType.DateTime, SqlDbType.Int, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.DateTime, SqlDbType.VarChar
        };
        public override SqlDbType[] GetColumnType()
        {
            return columnTypeList;
        }
        private static string[] premaryKeyList = new[] { "Id" };
        public override string[] GetPrimaryKey()
        {
            return premaryKeyList;
        }
        private static string[] nullableList = { };
        public override string[] GetNullableColumn()
        {
            return nullableList;
        }
    }
    //
    public class WebUserWx
    {
        private const string TableName = "tb_User_Wx";
        SqlDAC sqlDac;
        public WebUserWx()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }
        //
        #region 查询
        //获取用户信息
        public DataUserWx[] GetData(int intId)
        {
            string strSql = string.Format("SELECT * FROM {0} WHERE Id=@Id", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.VarChar, "Id", intId));
            SqlParameter[] sqlParaArray = list.ToArray();
            DataUserWx[] result = (DataUserWx[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataUserWx));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataUserWx[] GetDatas(string WxAppId, int UserId = 0, string WxOpenId = "", string strFields = "", int intPage = 0, int pageSize = 0, string strOrderBy = "", string strFilter = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strFromWhere = string.Format("FROM {0} WHERE WxAppId=@WxAppId", TableName);
            list.Add(SqlParamHelper.AddParameter("@WxAppId", SqlDbType.VarChar, "WxAppId", WxAppId));
            if (UserId > 0)
            {
                strFromWhere += " AND UserId=@UserId";
                list.Add(SqlParamHelper.AddParameter("@UserId", SqlDbType.Int, "UserId", UserId));
            }
            if (!string.IsNullOrEmpty(WxOpenId))
            {
                strFromWhere += " AND WxOpenId=@WxOpenId";
                list.Add(SqlParamHelper.AddParameter("@WxOpenId", SqlDbType.VarChar, "WxOpenId", WxOpenId));
            }
            string strOrder = " ORDER BY ";
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                strOrder += strOrderBy;
            }
            else
            {
                strOrder += "AddTime ASC";
            }
            strOrder += ", Id ASC";
            string strSql = "";
            if (pageSize > 0 && intPage > 1)
            {
                //分页查询语句：SELECT TOP 页大小 * FROM table WHERE id NOT IN ( SELECT TOP 页大小*(页数-1) id FROM table ORDER BY id ) ORDER BY id
                strSql = "SELECT TOP " + pageSize.ToString() + " * " + strFromWhere + " AND Id NOT IN ( SELECT TOP " + (pageSize * (intPage - 1)).ToString() + " Id " + strFromWhere + strOrder + " )" + strOrder;
            }
            else
            {
                if (pageSize <= 0)
                {
                    pageSize = 1000;//考虑数据库性能，只读取前1000条数据
                }
                strSql = "SELECT TOP " + pageSize.ToString() + " * " + strFromWhere + strOrder;
            }
            SqlParameter[] sqlParaArray = list.ToArray();
            DataUserWx[] result = (DataUserWx[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataUserWx));
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
        #region 编辑
        //插入
        public int Insert(DataUserWx data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataUserWx data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataUserWx data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            if (data.UserId >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@UserId", SqlDbType.Int, "UserId", data.UserId));
            }
            if (data.WxAppId != null)
            {
                list.Add(SqlParamHelper.AddParameter("@WxAppId", SqlDbType.VarChar, "WxAppId", data.WxAppId));
            }
            if (data.WxSubscribe >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@WxSubscribe", SqlDbType.Int, "WxSubscribe", data.WxSubscribe));
            }
            if (data.WxOpenId != null)
            {
                list.Add(SqlParamHelper.AddParameter("@WxOpenId", SqlDbType.VarChar, "WxOpenId", data.WxOpenId));
            }
            if (data.WxNickName != null)
            {
                list.Add(SqlParamHelper.AddParameter("@WxNickName", SqlDbType.NVarChar, "WxNickName", data.WxNickName));
            }
            if (data.WxLanguage != null)
            {
                list.Add(SqlParamHelper.AddParameter("@WxLanguage", SqlDbType.VarChar, "WxLanguage", data.WxLanguage));
            }
            if (data.WxSex >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@WxSex", SqlDbType.Int, "WxSex", data.WxSex));
            }
            if (data.WxProvince != null)
            {
                list.Add(SqlParamHelper.AddParameter("@WxProvince", SqlDbType.NVarChar, "WxProvince", data.WxProvince));
            }
            if (data.WxCity != null)
            {
                list.Add(SqlParamHelper.AddParameter("@WxCity", SqlDbType.NVarChar, "WxCity", data.WxCity));
            }
            if (data.WxCountry != null)
            {
                list.Add(SqlParamHelper.AddParameter("@WxCountry", SqlDbType.NVarChar, "WxCountry", data.WxCountry));
            }
            if (data.WxHeadImgUrl != null)
            {
                list.Add(SqlParamHelper.AddParameter("@WxHeadImgUrl", SqlDbType.Text, "WxHeadImgUrl", data.WxHeadImgUrl));
            }
            if (data.WxSubscribe_time >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@WxSubscribe_time", SqlDbType.Int, "WxSubscribe_time", data.WxSubscribe_time));
            }
            if (data.WxPrivilege != null)
            {
                list.Add(SqlParamHelper.AddParameter("@WxPrivilege", SqlDbType.Text, "WxPrivilege", data.WxPrivilege));
            }
            if (data.WxUnionid != null)
            {
                list.Add(SqlParamHelper.AddParameter("@WxUnionid", SqlDbType.VarChar, "WxUnionid", data.WxUnionid));
            }
            if (data.WxRemark != null)
            {
                list.Add(SqlParamHelper.AddParameter("@WxRemark", SqlDbType.NText, "WxRemark", data.WxRemark));
            }
            if (data.WxGroupid >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@WxGroupid", SqlDbType.Int, "WxGroupid", data.WxGroupid));
            }
            if (data.WxTagid_list != null)
            {
                list.Add(SqlParamHelper.AddParameter("@WxTagid_list", SqlDbType.Text, "WxTagid_list", data.WxTagid_list));
            }
            if (data.WxAccess_token != null)
            {
                list.Add(SqlParamHelper.AddParameter("@WxAccess_token", SqlDbType.Text, "WxAccess_token", data.WxAccess_token));
            }
            if (data.WxAccess_expires_in > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@WxAccess_expires_in", SqlDbType.DateTime, "WxAccess_expires_in", data.WxAccess_expires_in));
            }
            else
            {
                list.Add(SqlParamHelper.AddParameter("@WxAccess_expires_in", SqlDbType.VarChar, "WxAccess_expires_in", null));
            }
            if (data.WxRefresh_token != null)
            {
                list.Add(SqlParamHelper.AddParameter("@WxRefresh_token", SqlDbType.Text, "WxRefresh_token", data.WxRefresh_token));
            }
            if (data.WxRefresh_expires_in > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@WxRefresh_expires_in", SqlDbType.DateTime, "WxRefresh_expires_in", data.WxRefresh_expires_in));
            }
            else
            {
                list.Add(SqlParamHelper.AddParameter("@WxRefresh_expires_in", SqlDbType.VarChar, "WxRefresh_expires_in", null));
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
            if (data.UpTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@UpTime", SqlDbType.DateTime, "UpTime", data.UpTime));
            }
            if (data.UpIp != null)
            {
                list.Add(SqlParamHelper.AddParameter("@UpIp", SqlDbType.VarChar, "UpIp", data.UpIp));
            }
            return list.ToArray();
        }
        //更新用户Id
        public int UpdateUserId(int UserId, string WxAppId, string WxOpenId)
        {
            string strSql = string.Format("UPDATE {0} SET UserId=@UserId WHERE WxAppId=@WxAppId AND WxOpenId=@WxOpenId", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@UserId", SqlDbType.Int, "UserId", UserId));
            list.Add(SqlParamHelper.AddParameter("@WxAppId", SqlDbType.VarChar, "WxId", WxAppId));
            list.Add(SqlParamHelper.AddParameter("@WxOpenId", SqlDbType.VarChar, "WxOpenId", WxOpenId));
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.UpdateQuery(strSql, sqlParaArray);
        }
        #endregion
        //
    }
    //
}