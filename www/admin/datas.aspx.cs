using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mod.main;
using hkzx.db;
using hkzx.user;

namespace hkzx.web.admin
{
    public partial class datas : System.Web.UI.Page
    {
        private DataAdmin myUser = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperAdmin.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
            {
                //Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            if (myUser.Powers.IndexOf("alls") < 0 && myUser.Powers.IndexOf("datas") < 0)
            {
                Response.Redirect("./");
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.Powers = myUser.Powers;
            if (!IsPostBack)
            {
                WebDatasType webType = new WebDatasType();
                hfBack.Value = PublicMod.GetBackUrl();
                if (!string.IsNullOrEmpty(Request.QueryString["tid"]))
                {
                    plDatas.Visible = true;
                    WebDatas webDatas = new WebDatas();
                    listDatas(Convert.ToInt32(Request.QueryString["tid"]), webType, webDatas);
                    int Id = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                    loadDatas(Id, webDatas);
                }
                else
                {
                    plType.Visible = true;
                    listType(webType);
                    int Id = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                    loadType(Id, webType);
                }
            }
        }
        //
        #region 资料文档类型
        //加载列表
        private void listType(WebDatasType webType)
        {
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            DataDatasType[] data = webType.GetDatas(new DataDatasType(), "Id,TypeName,Remark,Active", pageCur, pageSize, "", "total");
            if (data != null)
            {
                WebDatas webDatas = new WebDatas();
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    if (data[i].Active > 0)
                    {
                        data[i].ActiveName = "正常";
                    }
                    else
                    {
                        data[i].ActiveName = "取消";
                        data[i].rowClass = " class='cancel'";
                    }
                    DataDatas qData2 = new DataDatas();
                    qData2.Active = 1;
                    qData2.TypeId = data[i].Id;
                    DataDatas[] data2 = webDatas.GetDatas(qData2, "Id");
                    if (data2 != null)
                    {
                        data[i].DatasNum = data2.Count();
                    }
                }
                rpTypeList.DataSource = data;
                rpTypeList.DataBind();
                ltTypeNo.Visible = false;
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = Request.Url.ToString();
                lblTypeNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
                ltTypeTotal.Text = data[0].total.ToString();
            }
        }
        //加载信息
        private void loadType(int Id, WebDatasType webType)
        {
            if (Id <= 0)
            {
                txtTypeAddTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                txtTypeAddIp.Text = HelperMain.GetIp();
                txtTypeAddUser.Text = myUser.AdminName;
                return;
            }
            DataDatasType[] data = webType.GetData(Id);
            if (data != null)
            {
                txtTypeId.Text = data[0].Id.ToString();
                txtTypeName.Text = data[0].TypeName;
                txtTypeRemark.Text = data[0].Remark;
                txtTypeActive.Text = data[0].Active.ToString();
                txtTypeAddTime.Text = data[0].AddTime.ToString("yyyy-MM-dd HH:mm:ss");
                txtTypeAddIp.Text = data[0].AddIp;
                txtTypeAddUser.Text = data[0].AddUser;
                if (!string.IsNullOrEmpty(data[0].UpIp) || !string.IsNullOrEmpty(data[0].UpUser))
                {
                    txtTypeUpTime.Text = data[0].UpTime.ToString("yyyy-MM-dd HH:mm:ss");
                    txtTypeUpIp.Text = data[0].UpIp;
                    txtTypeUpUser.Text = data[0].UpUser;
                }
                btnTypeEdit.Text = "修改";
                ltTypeTitle.Text = ltTypeTitle.Text.Replace("新增", "修改");
                btnTypeCancel.Visible = true;
            }
        }
        //编辑数据
        protected void btnTypeEdit_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            string strBack = hfBack.Value;
            WebDatasType webType = new WebDatasType();
            DataDatasType data = new DataDatasType();
            data.TypeName = HelperMain.SqlFilter(txtTypeName.Text.Trim(), 20);
            DataDatasType[] ckData = webType.GetDatas(data, "Id");//重复检查
            data.Id = Convert.ToInt32(txtTypeId.Text);
            if (ckData != null && ckData[0].Id != data.Id)
            {
                ltInfo.Text = "<script>$(function(){ alert('“类型名称”重复，不能添加！'); window.history.back(-1); });</script>";
                return;
            }
            data.Remark = HelperMain.SqlFilter(txtTypeRemark.Text.Trim(), 100);
            data.Active = (!string.IsNullOrEmpty(txtTypeActive.Text.Trim())) ? Convert.ToInt16(txtTypeActive.Text.Trim()) : 1;
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(myUser.AdminName, 20);
            if (data.Id <= 0)
            {
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = strUser;
                data.Id = webType.Insert(data);
            }
            else
            {
                data.UpTime = dtNow;
                data.UpIp = strIp;
                data.UpUser = strUser;
                if (webType.Update(data) <= 0)
                {
                    data.Id = -1;
                }
            }
            if (data.Id > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltTypeTitle.Text + "”成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltTypeTitle.Text + "”失败！'); window.history.back(-1); });</script>";
            }
        }
        //取消状态
        protected void btnTypeCancel_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            int Id = Convert.ToInt32(txtTypeId.Text);
            if (Id <= 0)
            {
                return;
            }
            WebDatasType webType = new WebDatasType();
            if (webType.UpdateActive(Id, -1) > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnTypeCancel.Text + "成功！'); window.location.href='" + hfBack.Value + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnTypeCancel.Text + "失败！'); window.history.back(-1); });</script>";
            }
        }
        #endregion
        //
        #region 资料文档信息
        //加载列表
        private void listDatas(int tId, WebDatasType webType, WebDatas webDatas)
        {
            if (tId <= 0)
            {
                Response.Redirect("op.aspx");
            }
            DataDatasType[] dType = webType.GetData(tId, "TypeName");
            if (dType == null)
            {
                Response.Redirect("data.aspx");
            }
            hfTypeId.Value = tId.ToString();
            txtType.Text = dType[0].TypeName;
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            DataDatas qData = new DataDatas();
            qData.TypeId = tId;
            DataDatas[] data = webDatas.GetDatas(qData, "Id,TypeId,Title,Body,Files,Remark,Active", pageCur, pageSize, "", "total");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    data[i].TypeName = dType[0].TypeName;
                    if (data[i].Active > 0)
                    {
                        data[i].ActiveName = "正常";
                    }
                    else
                    {
                        data[i].ActiveName = "取消";
                        data[i].rowClass = " class='cancel'";
                    }
                    if (data[i].Body.Length > 50)
                    {
                        data[i].Body = data[i].Body.Substring(0, 50) + "…";
                    }
                    if (!string.IsNullOrEmpty(data[i].Files))
                    {
                        data[i].Files = string.Format("<a href='{0}' target='_blank' class='btn'><u>附件下载</u></a>", data[i].Files);
                    }
                }
                rpDatasList.DataSource = data;
                rpDatasList.DataBind();
                ltDatasNo.Visible = false;
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = Request.Url.ToString();
                lblDatasNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
                ltDatasTotal.Text = data[0].total.ToString();
            }
        }
        //加载信息
        private void loadDatas(int Id, WebDatas webDatas)
        {
            if (Id <= 0)
            {
                txtAddTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                txtAddIp.Text = HelperMain.GetIp();
                txtAddUser.Text = myUser.AdminName;
                return;
            }
            DataDatas[] data = webDatas.GetData(Id);
            if (data != null)
            {
                txtId.Text = data[0].Id.ToString();
                hfTypeId.Value = data[0].TypeId.ToString();
                txtTitle.Text = data[0].Title;
                txtBody.Text = data[0].Body;
                txtFiles.Text = data[0].Files;
                txtRemark.Text = data[0].Remark;
                txtActive.Text = data[0].Active.ToString();
                txtAddTime.Text = data[0].AddTime.ToString("yyyy-MM-dd HH:mm:ss");
                txtAddIp.Text = data[0].AddIp;
                txtAddUser.Text = data[0].AddUser;
                if (!string.IsNullOrEmpty(data[0].UpIp) || !string.IsNullOrEmpty(data[0].UpUser))
                {
                    txtUpTime.Text = data[0].UpTime.ToString("yyyy-MM-dd HH:mm:ss");
                    txtUpIp.Text = data[0].UpIp;
                    txtUpUser.Text = data[0].UpUser;
                }
                btnDatasEdit.Text = "修改";
                ltDatasTitle.Text = ltDatasTitle.Text.Replace("新增", "修改");
                btnDatasCancel.Visible = true;
            }
        }
        //编辑数据
        protected void btnDatasEdit_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            WebDatas webDatas = new WebDatas();
            DataDatas data = new DataDatas();
            data.Id = Convert.ToInt32(txtId.Text);
            data.TypeId = Convert.ToInt32(hfTypeId.Value);
            data.Title = HelperMain.SqlFilter(txtTitle.Text.Trim(), 50);
            DataDatas[] ckData = webDatas.GetDatas(data, "Id");//重复检查
            if (ckData != null && ckData[0].Id != data.Id)
            {
                ltInfo.Text = "<script>$(function(){ alert('“选项名称”重复，不能添加！'); window.history.back(-1); });</script>";
                return;
            }
            data.Body = HelperMain.SqlFilter(txtBody.Text.Trim());
            data.Files = HelperMain.SqlFilter(txtFiles.Text.Trim());
            data.Remark = HelperMain.SqlFilter(txtRemark.Text.Trim());
            data.Active = (!string.IsNullOrEmpty(txtActive.Text.Trim())) ? Convert.ToInt16(txtActive.Text.Trim()) : 1;
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(myUser.AdminName, 20);
            if (data.Id <= 0)
            {
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = strUser;
                data.Id = webDatas.Insert(data);
            }
            else
            {
                data.UpTime = dtNow;
                data.UpIp = strIp;
                data.UpUser = strUser;
                if (webDatas.Update(data) <= 0)
                {
                    data.Id = -1;
                }
            }
            if (data.Id > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltDatasTitle.Text + "”成功！'); window.location.href='" + hfBack.Value + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltDatasTitle.Text + "”失败！'); window.history.back(-1); });</script>";
            }
        }
        //取消状态
        protected void btnDatasCancel_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            int Id = Convert.ToInt32(txtId.Text);
            if (Id <= 0)
            {
                return;
            }
            WebDatas WebDatas = new WebDatas();
            if (WebDatas.UpdateActive(Id, -1) > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnDatasCancel.Text + "成功！'); window.location.href='" + hfBack.Value + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnDatasCancel.Text + "失败！'); window.history.back(-1); });</script>";
            }
        }
        #endregion
        //
    }
}