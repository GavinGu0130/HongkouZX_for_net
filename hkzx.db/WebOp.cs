﻿using System;
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
    public class DataOp : MSBaseData
    {
        public int Id { get; set; }//主键
        public string OpType { get; set; }//分类名称
        public string OpName { get; set; }//选项名称
        public string OpValue { get; set; }//选项值
        public string OpValue2 { get; set; }//选项值2
        public bool Selected { get; set; }//默认是否选中
        public string Remark { get; set; }//备注
        public int Active { get; set; }//状态：默认0为前台不显示
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
        public string SelectedName { get; set; }//选中状态名称
        //
        private static string[] columnList = new[] {
            "Id", "OpType", "OpName", "OpValue", "OpValue2", "Selected", "Remark", "Active", 
            "AddTime", "AddIp", "AddUser", "UpTime", "UpIp", "UpUser"
        };
        public override string[] GetColumnName()
        {
            return columnList;
        }
        private static SqlDbType[] columnTypeList = new[] { SqlDbType.Int, 
            SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.Bit, SqlDbType.NText, SqlDbType.Int, 
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
    public class WebOp
    {
        private const string TableName = "tb_Op";
        SqlDAC sqlDac;
        public WebOp()
        {
            sqlDac = new SqlDAC(Config.ConnString);
        }
        //
        #region 查询
        public DataOp[] GetData(int intId, string strFields = "")
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
            DataOp[] result = (DataOp[])sqlDac.GetDataByAnyCondition(strSql, sqlParameters, typeof(DataOp));
            if (result != null && result.Length > 0)
            {
                return result;
            }
            return null;
        }
        public DataOp[] GetDatas(int Active, string OpType, string OpName, string strFields)
        {
            DataOp data = new DataOp();
            data.Active = Active;
            data.OpType = OpType;
            data.OpName = OpName;
            return GetDatas(data, strFields);
        }
        public DataOp[] GetDatas(DataOp data, string strFields = "", int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
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
            if (!string.IsNullOrEmpty(data.OpName))
            {
                list.Add(SqlParamHelper.AddParameter("@OpName", SqlDbType.NVarChar, "OpName", data.OpName));
                strFromWhere += " AND OpName=@OpName";
            }
            if (!string.IsNullOrEmpty(data.OpValue2))
            {
                list.Add(SqlParamHelper.AddParameter("@OpValue2", SqlDbType.NVarChar, "OpValue2", data.OpValue2));
                if (data.OpValue2.IndexOf("%") >= 0)
                {
                    strFromWhere += " AND OpValue2 LIKE @OpValue2";
                }
                else
                {
                    strFromWhere += " AND OpValue2=@OpValue2";
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
            DataOp[] result = (DataOp[])sqlDac.GetDataByAnyCondition(strSql, sqlParaArray, typeof(DataOp));
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
        public int Insert(DataOp data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            return sqlDac.InsertQuery(TableName, sqlParaArray);
        }
        //修改
        public int Update(DataOp data)
        {
            SqlParameter[] sqlParaArray = getParaArray(data);
            SqlParameter[] sqlParaArrayWhere = new[]{
                SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id)
            };
            return sqlDac.UpdateQuery(TableName, sqlParaArray, sqlParaArrayWhere);
        }
        private SqlParameter[] getParaArray(DataOp data)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            //list.Add(SqlParamHelper.AddParameter("@Id", SqlDbType.Int, "Id", data.Id));
            if (data.OpType != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OpType", SqlDbType.NVarChar, "OpType", data.OpType));
            }
            if (data.OpName != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OpName", SqlDbType.NVarChar, "OpName", data.OpName));
            }
            if (data.OpValue != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OpValue", SqlDbType.NVarChar, "OpValue", data.OpValue));
            }
            if (data.OpValue2 != null)
            {
                list.Add(SqlParamHelper.AddParameter("@OpValue2", SqlDbType.NVarChar, "OpValue2", data.OpValue2));
            }
            list.Add(SqlParamHelper.AddParameter("@Selected", SqlDbType.Bit, "Selected", data.Selected));
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
        //修改分类名称
        public int UpdateOpType(string oldOpType, string newOpType)
        {
            string strSql = string.Format("UPDATE {0} SET OpType=@newOpType WHERE OpType=@oldOpType", TableName);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(SqlParamHelper.AddParameter("@oldOpType", SqlDbType.NVarChar, "oldOpType", oldOpType));
            list.Add(SqlParamHelper.AddParameter("@newOpType", SqlDbType.NVarChar, "newOpType", newOpType));
            SqlParameter[] sqlParaArray = list.ToArray();
            return sqlDac.ExecProcedure(strSql, sqlParaArray, false);
        }
        #endregion
    }
    //
}