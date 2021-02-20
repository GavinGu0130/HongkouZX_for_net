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
    public class DataOpinionFeed : MSBaseData
    {
        public int Id { get; set; }//主键
        public string OpType { get; set; }//分类：提案，社情民意
        public int OpId { get; set; }//提案、社情民意Id
        public int OrgId { get; set; }//承办单位Id
        public string Interview { get; set; }//走访情况：已走访，委员本人提出不需要走访，未走访
        public string Attitude { get; set; }//办理态度：满意，理解，不满意
        public string TakeWay { get; set; }//听取意见方式：走访，电话，其他，未联系
        public string Pertinence { get; set; }//答复是否针对提案：针对，基本针对，未针对
        public string LeaderReply { get; set; }//（团体提案）是否分管领导答复：是，否
        public string Result { get; set; }//办理结果：同意，理解，保留，不同意
        public string Body { get; set; }//对提案办理的意见或建议
        public string Remark { get; set; }//备注
        public int Active { get; set; }//状态：默认0为前台不显示
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
        public string ActiveName { get; set; }//状态名称
        public string AddTimeText { get; set; }//添加时间文本
        //
        private static string[] columnList = new[] {
            "Id", "OpType", "OpId", "OrgId", "Interview", "Attitude", "TakeWay", "Pertinence", "LeaderReply", "Result", 
            "Body", "Remark", "Active", "UserId", "AddTime", "AddIp", "AddUser", "UpTime", "UpIp", "UpUser"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] {
            SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.Int, SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, 
            SqlDbType.NText, SqlDbType.NText, SqlDbType.Int, SqlDbType.Int, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar, SqlDbType.DateTime, SqlDbType.VarChar, SqlDbType.NVarChar
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
    public class WebOpinionFeed
    {
        private const string TableName = "tb_Opinion_Feed";
        SqlDAC sqlDac;
        public WebOpinionFeed()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }
        //
        #region 查询
        public DataOpinionFeed[] GetData(int intId, string strFields = "")
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
            DataOpinionFeed[] result = (DataOpinionFeed[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataOpinionFeed));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataOpinionFeed[] GetDatas(int Active, string OpType, int OpId, int UserId = 0, string strFields = "")
        {
            DataOpinionFeed data = new DataOpinionFeed();
            data.Active = Active;
            data.OpType = OpType;
            data.OpId = OpId;
            data.UserId = UserId;
            return GetDatas(data, strFields);
        }
        public DataOpinionFeed[] GetDatas(DataOpinionFeed data, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
        {
            List<SqlParameter> list = new List<SqlParameter>();
            string strFromWhere = string.Format("FROM {0} WHERE ", TableName);
            if (data.Active > 0)
            {
                strFromWhere += "Active>0";
            }
            else if (data.Active < 0)
            {
                strFromWhere += "Active<0";
            }
            else
            {
                strFromWhere += "1=1";
            }
            if (!string.IsNullOrEmpty(data.OpType))
            {
                list.Add(SqlParamHelper.AddParameter("@OpType", SqlDbType.NVarChar, "OpType", data.OpType));
                strFromWhere += " AND OpType=@OpType";
            }
            if (data.OpId > 0)
            {
                list.Add(SqlParamHelper.AddParameter("@OpId", SqlDbType.Int, "OpId", data.OpId));
                strFromWhere += " AND OpId=@OpId";
            }
            if (data.UserId > 0)
            {
                list.Add(SqlParamHelper.AddParameter("@UserId", SqlDbType.Int, "UserId", data.UserId));
                strFromWhere += " AND UserId=@UserId";
            }
            //if (data.OrgId > 0)
            //{
            //    list.Add(SqlParamHelper.AddParameter("@OrgId", SqlDbType.Int, "OrgId", data.OrgId));
            //    strFromWhere += " AND OrgId=@OrgId";
            //}
            if (!string.IsNullOrEmpty(data.AddTimeText) && data.AddTimeText.IndexOf(",") >= 0)
            {
                string[] arr = data.AddTimeText.Split(',');
                if (!string.IsNullOrEmpty(arr[0]))
                {
                    list.Add(SqlParamHelper.AddParameter("@AddTime1", SqlDbType.DateTime, "AddTime1", Convert.ToDateTime(arr[0])));
                    strFromWhere += " AND AddTime>=@AddTime1";
                }
                if (!string.IsNullOrEmpty(arr[1]))
                {
                    list.Add(SqlParamHelper.AddParameter("@AddTime2", SqlDbType.DateTime, "AddTime2", Convert.ToDateTime(arr[1])));
                    strFromWhere += " AND AddTime<=@AddTime2";
                }
            }
            string strOrder = " ORDER BY ";
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                strOrder += strOrderBy;
            }
            else
            {
                strOrder += "Active DESC";
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
            DataOpinionFeed[] result = (DataOpinionFeed[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataOpinionFeed));
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
        public int Insert(DataOpinionFeed data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataOpinionFeed data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataOpinionFeed data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.OpType != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OpType", SqlDbType.NVarChar, "OpType", data.OpType));
            }
            if (data.OpId >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@OpId", SqlDbType.Int, "OpId", data.OpId));
            }
            if (data.OrgId >= 0)
            {
                list.Add(SqlParamHelper.AddParameter("@OrgId", SqlDbType.Int, "OrgId", data.OrgId));
            }
            if (data.Interview != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Interview", SqlDbType.NVarChar, "Interview", data.Interview));
            }
            if (data.Attitude != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Attitude", SqlDbType.NVarChar, "Attitude", data.Attitude));
            }
            if (data.TakeWay != null)
            {
                list.Add(SqlParamHelper.AddParameter("@TakeWay", SqlDbType.NVarChar, "TakeWay", data.TakeWay));
            }
            if (data.Pertinence != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Pertinence", SqlDbType.NVarChar, "Pertinence", data.Pertinence));
            }
            if (data.LeaderReply != null)
            {
                list.Add(SqlParamHelper.AddParameter("@LeaderReply", SqlDbType.NVarChar, "LeaderReply", data.LeaderReply));
            }
            if (data.Result != null)
            {
                list.Add(SqlParamHelper.AddParameter("@Result", SqlDbType.NVarChar, "Result", data.Result));
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
        #endregion
    }
    //
}