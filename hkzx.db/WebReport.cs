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
    public class DataReport : MSBaseData
    {
        public int Id { get; set; }//主键
        public string OrgName { get; set; }//机构名称：政治面貌，专委会，界别活动组，街道活动组
        public string IsPoint { get; set; }//是否建议案：
        public string SubMan { get; set; }//执笔人
        public string SubMans { get; set; }//课题组成员：,委员,委员,
        public string Title { get; set; }//标题
        public string Body { get; set; }//内容
        public string Files { get; set; }//附件：url(\n)url
        public string VerifyInfo { get; set; }//（审核）原因
        public string Remark { get; set; }//备注
        public string ActiveName { get; set; }//状态：删除，暂存，已提交，退回，审核通过
        public int UserId { get; set; }//用户Id
        public DateTime AddTime { get; set; }//添加时间：默认为当前时间
        public string AddIp { get; set; }//添加IP:端口号
        public string AddUser { get; set; }//添加人
        public DateTime UpTime { get; set; }//修改时间：默认为当前时间
        public string UpIp { get; set; }//修改IP:端口号
        public string UpUser { get; set; }//修改人
        public DateTime SubTime { get; set; }//提交时间
        public string SubIp { get; set; }//提交IP:端口号
        public DateTime VerifyTime { get; set; }//审核时间
        public string VerifyIp { get; set; }//审核IP:端口号
        public string VerifyUser { get; set; }//审核人
        //分页统计
        public string rowClass { get; set; }//行class属性
        public int num { get; set; }//序号
        public int total { get; set; }//统计数
        public string SubTimeText { get; set; }//提交时间文本
        public string VerifyTimeText { get; set; }//审核时间文本
        public string StateName { get; set; }//操作名称
        //
        private static string[] columnList = new[] {
            "Id", "OrgName", "IsPoint", "SubMan", "SubMans", "Title", "Body", "Files", "VerifyInfo", "Remark", 
            "ActiveName", "UserId", "AddTime", "AddIp", "AddUser", "UpTime", "UpIp", "UpUser", "SubTime", "SubIp", 
            "VerifyTime", "VerifyIp", "VerifyUser"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] { SqlDbType.Int, 
            SqlDbType.Int, SqlDbType.NText, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.Text, SqlDbType.NText, SqlDbType.NText, 
            SqlDbType.NVarChar, SqlDbType.Int, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, 
            SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar
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
    public class WebReport
    {
        private const string TableName = "tb_Report";
        SqlDAC sqlDac;
        public WebReport()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }
        //
        #region 查询
        public DataReport[] GetData(int intId, string strFields = "")
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
            DataReport[] result = (DataReport[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataReport));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataReport[] GetDatas(DataReport data, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
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
            if (!string.IsNullOrEmpty(data.IsPoint))
            {
                list.Add(SqlParamHelper.AddParameter("@IsPoint", SqlDbType.NVarChar, "IsPoint", data.IsPoint));
                strFromWhere += " AND IsPoint=@IsPoint";
            }
            if (!string.IsNullOrEmpty(data.SubMan))
            {
                list.Add(SqlParamHelper.AddParameter("@SubMan", SqlDbType.NVarChar, "SubMan", data.SubMan));
                strFromWhere += " AND SubMan=@SubMan";
            }
            if (!string.IsNullOrEmpty(data.SubMans))
            {
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
                        string tmp = "SubMans" + i.ToString();
                        list.Add(SqlParamHelper.AddParameter("@" + tmp, SqlDbType.NVarChar, tmp, "%," + arr[i] + ",%"));
                        strTmp += "SubMans LIKE @" + tmp;
                    }
                }
                if (strTmp != "")
                {
                    strFromWhere += "(" + strTmp + ")";
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
            if (!string.IsNullOrEmpty(data.SubTimeText) && data.SubTimeText.IndexOf(",") >= 0)
            {
                string[] arr = data.SubTimeText.Split(',');
                if (!string.IsNullOrEmpty(arr[0]))
                {
                    list.Add(SqlParamHelper.AddParameter("@SubTime1", SqlDbType.DateTime, "SubTime1", Convert.ToDateTime(arr[0])));
                    strFromWhere += " AND SubTime>=@SubTime1";
                }
                if (!string.IsNullOrEmpty(arr[1]))
                {
                    if (arr[1].IndexOf(":") < 0)
                    {
                        arr[1] += " 23:59:59";
                    }
                    list.Add(SqlParamHelper.AddParameter("@SubTime2", SqlDbType.DateTime, "SubTime2", Convert.ToDateTime(arr[1])));
                    strFromWhere += " AND SubTime<=@SubTime2";
                }
            }
            string strOrder = " ORDER BY ";
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                strOrder += strOrderBy;
            }
            else
            {
                strOrder += "SubTime DESC, UpTime DESC, AddTime DESC";
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
                    pageSize = 1000000;//考虑数据库性能，只读取前1000,000条数据
                }
                strSql = "SELECT TOP " + pageSize.ToString() + strFields + strFromWhere + strOrder;
            }
            SqlParameter[] sqlParaArray = list.ToArray();
            DataReport[] result = (DataReport[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataReport));
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
        public int Insert(DataReport data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataReport data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataReport data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.OrgName != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OrgName", SqlDbType.NText, "OrgName", data.OrgName));
            }
            if (data.IsPoint != null)
            {
                list.Add(SqlParamHelper.AddParameter("@IsPoint", SqlDbType.NVarChar, "IsPoint", data.IsPoint));
            }
            if (data.SubMan != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubMan", SqlDbType.NVarChar, "SubMan", data.SubMan));
            }
            if (data.SubMans != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubMans", SqlDbType.NText, "SubMans", data.SubMans));
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
            if (data.VerifyInfo != null)
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyInfo", SqlDbType.NText, "VerifyInfo", data.VerifyInfo));
            }
            if (data.Remark != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Remark", SqlDbType.NText, "Remark", data.Remark));
            }
            if (data.ActiveName != null)
            {
                list.Add(SqlParamHelper.AddParameter("@ActiveName", SqlDbType.NVarChar, "ActiveName", data.ActiveName));
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
            if (data.SubTime > DateTime.MinValue)
            {
                list.Add(SqlParamHelper.AddParameter("@SubTime", SqlDbType.DateTime, "SubTime", data.SubTime));
            }
            if (data.SubIp != null)
            {
                list.Add(SqlParamHelper.AddParameter("@SubIp", SqlDbType.VarChar, "SubIp", data.SubIp));
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
        public int UpdateActive(int intId, string ActiveName)
        {
            string strSql = string.Format("UPDATE {0} SET ActiveName=@ActiveName WHERE Id=@Id", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", intId));
            list.Add(SqlParamHelper.AddParameter("@ActiveName", SqlDbType.NVarChar, "ActiveName", ActiveName));
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        public int UpdateActive(ArrayList arrList, string ActiveName, string VerifyInfo = "", string strIp = "", string strUser = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strSql = string.Format("UPDATE {0} SET ActiveName=@ActiveName", TableName);
            list.Add(SqlParamHelper.AddParameter("@ActiveName", SqlDbType.NVarChar, "ActiveName", ActiveName));
            if (!string.IsNullOrEmpty(strIp) || !string.IsNullOrEmpty(strUser))
            {
                list.Add(SqlParamHelper.AddParameter("@VerifyTime", SqlDbType.DateTime, "VerifyTime", DateTime.Now));
                list.Add(SqlParamHelper.AddParameter("@VerifyIp", SqlDbType.VarChar, "VerifyIp", strIp));
                list.Add(SqlParamHelper.AddParameter("@VerifyUser", SqlDbType.NVarChar, "VerifyUser", strUser));
                strSql += ", VerifyTime=@VerifyTime, VerifyIp=@VerifyIp, VerifyUser=@VerifyUser";
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
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        #endregion
    }
    //
}