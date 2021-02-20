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
    public class DataAdmin : MSBaseData
    {
        public int Id { get; set; }//主键
        public string AdminName { get; set; }//管理员名
        public string AdminPwd { get; set; }//密码（MD5加密）
        public string TrueName { get; set; }//真实姓名
        public string UserSex { get; set; }//性别
        public string IdCard { get; set; }//身份证号（des加密保存）
        public string Photo { get; set; }//照片
        public int Grade { get; set; }//级别状态（-1锁定，0待批准，1普通管理员，9超级管理员）
        public string Powers { get; set; }//管理权限
        public string DepPost { get; set; }//部门职(岗)位
        public string OfficeTel { get; set; }//办公室电话
        public string Mobile { get; set; }//手机
        public string Email { get; set; }//邮箱
        public string WeChat { get; set; }//微信号
        public string Remark { get; set; }//备注
        public int Active { get; set; }//状态：默认0为前台不显示
        public DateTime AddTime { get; set; }//添加时间：默认为当前时间
        public string AddIp { get; set; }//添加IP:端口号
        public string AddUser { get; set; }//添加人
        public DateTime UpTime { get; set; }//修改时间：默认为当前时间
        public string UpIp { get; set; }//修改IP:端口号
        public string UpUser { get; set; }//修改人
        public DateTime LastTime { get; set; }//最后登录时间
        public string LastIp { get; set; }//最后登录IP:端口号
        public int ErrNum { get; set; }//密码错误次数
        //分页统计
        public string rowClass { get; set; }//行class属性
        public int num { get; set; }//序号
        public int total { get; set; }//统计数
        public string ActiveName { get; set; }//状态名称
        public string LastTimeText { get; set; }//登录时间文本
        //
        private static string[] columnList = new[] {
            "Id", "AdminName", "AdminPwd", "TrueName", "UserSex", "IdCard", "Photo", "Grade", "Powers", "DepPost", 
            "OfficeTel", "Mobile", "Email", "WeChat", "Remark", "Active", "LastTime", "LastIp", "ErrNum", 
            "AddTime", "AddIp", "AddUser", "UpTime", "UpIp", "UpUser"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] {
            SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.VarChar, SqlDbType.Text, SqlDbType.Int, SqlDbType.Text, SqlDbType.NVarChar, 
            SqlDbType.VarChar, SqlDbType.VarChar, SqlDbType.Text, SqlDbType.VarChar, SqlDbType.NText, SqlDbType.Int, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.Int, 
            SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar
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
    public class WebAdmin
    {
        private const string TableName = "tb_Admin";
        SqlDAC sqlDac;
        public WebAdmin()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }

        #region 用户登录
        public DataAdmin[] Login(string LastIp, DateTime LastTime, string UserPwd, string UserName)
        {
            string strSql = string.Format("SELECT * FROM {0} WHERE AdminPwd=@AdminPwd AND AdminName=@AdminName", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@AdminPwd", SqlDbType.VarChar, "AdminPwd", UserPwd));
            list.Add(SqlParamHelper.AddParameter("@AdminName", SqlDbType.NVarChar, "AdminName", UserName));
            SqlParameter[] sqlParameters = list.ToArray();
            DataAdmin[] result = (DataAdmin[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataAdmin));
            if (result != null && result.Length > 0)
            {
                if (result[0].ErrNum > 10)
                {
                    updateErrNum(result[0].Id);//增加错误次数
                }
                else if (result[0].Active > 0)
                {
                    updateLogin(result[0].Id, LastIp, LastTime);//更新登录信息
                }
                return result;
            }
            else
            {
                updateErrNum(0, UserName);//增加错误次数
            }
            return null;
        }
        //更新登录信息
        private int updateLogin(int Id, string LastIp, DateTime LastTime)
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
        private void updateErrNum(int Id, string UserName = "")
        {
            string strSql = string.Format("UPDATE {0} SET ErrNum=ErrNum+1 WHERE ", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            if (Id > 0)
            {
                list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", Id));
                strSql += "Id=@Id";
            }
            else
            {
                list.Add(SqlParamHelper.AddParameter("@AdminName", SqlDbType.NVarChar, "AdminName", UserName));
                strSql += "AdminName=@AdminName";
            }
            SqlParameter[] sqlParameters = list.ToArray();
            sqlDac.UpdateQuery(strSql, sqlParameters);
        }
        #endregion

        #region 查询
        public DataAdmin[] GetData(int intId, string strFields = "")
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
            DataAdmin[] result = (DataAdmin[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataAdmin));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataAdmin[] GetDatas(string AdminName, string strFields = "")
        {
            SqlParameter[] sqlParameters = new[] 
            { 
               new SqlParameter("@AdminName", SqlDbType.NVarChar)
            };
            sqlParameters[0].Value = AdminName;
            if (string.IsNullOrEmpty(strFields))
            {
                strFields = "*";
            }
            string strSql = string.Format("SELECT {1} FROM {0} WHERE AdminName=@AdminName", TableName, strFields);
            DataAdmin[] result = (DataAdmin[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataAdmin));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataAdmin[] GetDatas(DataAdmin data, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strFromWhere = string.Format("FROM {0} WHERE ", TableName);
            if (data.Active > 0)
            {
                strFromWhere += "Active>0";
            }
            else
            {
                strFromWhere += "1=1";
            }
            if (!string.IsNullOrEmpty(data.AdminName))
            {
                list.Add(SqlParamHelper.AddParameter("@AdminName", SqlDbType.NVarChar, "AdminName", data.AdminName));
                strFromWhere += " AND AdminName LIKE @AdminName";
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
            if (!string.IsNullOrEmpty(data.DepPost))
            {
                list.Add(SqlParamHelper.AddParameter("@DepPost", SqlDbType.NVarChar, "DepPost", data.DepPost));
                strFromWhere += " AND OrgName LIKE @OrgName";
            }
            if (!string.IsNullOrEmpty(data.Mobile))
            {
                list.Add(SqlParamHelper.AddParameter("@Mobile", SqlDbType.VarChar, "Mobile", data.Mobile));
                strFromWhere += " AND (Mobile LIKE @Mobile OR OfficeTel LIKE @Mobile)";
            }
            if (!string.IsNullOrEmpty(data.Email))
            {
                list.Add(SqlParamHelper.AddParameter("@Email", SqlDbType.VarChar, "Email", data.Email));
                strFromWhere += " AND Email LIKE @Email";
            }
            string strOrder = " ORDER BY ";
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                strOrder += strOrderBy;
            }
            else
            {
                strOrder += "AdminName ASC";
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
            DataAdmin[] result = (DataAdmin[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataAdmin));
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

        #region 修改
        //修改用户密码
        public int SetUserPwd(int Id, string NewPwd, string OldPwd = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", Id));
            list.Add(SqlParamHelper.AddParameter("@NewPwd", SqlDbType.VarChar, "NewPwd", NewPwd));
            string strSql = string.Format("UPDATE {0} SET AdminPwd=@NewPwd WHERE Id=@Id", TableName);
            if (!string.IsNullOrEmpty(OldPwd))
            {
                list.Add(SqlParamHelper.AddParameter("@OldPwd", SqlDbType.VarChar, "OldPwd", OldPwd));
                strSql += " AND AdminPwd=@OldPwd";
            }
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        //插入
        public int Insert(DataAdmin data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataAdmin data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataAdmin data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.AdminName != null)
            {
                list.Add(SqlParamHelper.AddParameter("@AdminName", SqlDbType.NVarChar, "AdminName", data.AdminName));
            }
            if (data.AdminPwd != null)
            {
                list.Add(SqlParamHelper.AddParameter("@AdminPwd", SqlDbType.VarChar, "AdminPwd", data.AdminPwd));
            }
            if (data.TrueName != null)
            {
                list.Add(SqlParamHelper.AddParameter("@TrueName", SqlDbType.NVarChar, "TrueName", data.TrueName));
            }
            if (data.UserSex != null)
            {
                list.Add(SqlParamHelper.AddParameter("@UserSex", SqlDbType.NVarChar, "UserSex", data.UserSex));
            }
            if (data.IdCard != null)
            {
                list.Add(SqlParamHelper.AddParameter("@IdCard", SqlDbType.VarChar, "IdCard", data.IdCard));
            }
            if (data.Photo != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Photo", SqlDbType.Text, "Photo", data.Photo));
            }
            if (data.Grade >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@Grade", SqlDbType.Int, "Grade", data.Grade));
            }
            if (data.Powers != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Powers", SqlDbType.Text, "Powers", data.Powers));
            }
            if (data.DepPost != null)
            {
                list.Add(SqlParamHelper.AddParameter("@DepPost", SqlDbType.NVarChar, "DepPost", data.DepPost));
            }
            if (data.OfficeTel != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OfficeTel", SqlDbType.Text, "OfficeTel", data.OfficeTel));
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
            //else
            //{
            //    list.Add(SqlParamHelper.AddParameter("@LastTime", SqlDbType.VarChar, "LastTime", null));
            //}
            //if (data.LastIp != null)
            //{
            //    list.Add(SqlParamHelper.AddParameter("@LastIp", SqlDbType.VarChar, "LastIp", data.LastIp));
            //}
            if (data.ErrNum >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@ErrNum", SqlDbType.Int, "ErrNum", data.ErrNum));
            }
            return list.ToArray();
        }
        #endregion
    }
    //
}