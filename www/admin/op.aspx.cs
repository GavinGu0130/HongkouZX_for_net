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
    public partial class op : System.Web.UI.Page
    {
        private DataAdmin myUser = null;
        private WebOpType webType = new WebOpType();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperAdmin.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
            {
                //Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            if (myUser.Grade < 9 || (myUser.Powers.IndexOf("alls") < 0 && myUser.Powers.IndexOf("system") < 0))
            {
                Response.Redirect("./");
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.Powers = myUser.Powers;
            WebOp webOp = new WebOp();
            if (!string.IsNullOrEmpty(Request.QueryString["tid"]))
            {
                Header.Title += " - 系统设置 - 分类选项";
                plOp.Visible = true;
                listOp(Convert.ToInt32(Request.QueryString["tid"]), webOp);
                if (!IsPostBack)
                {
                    hfBack.Value = PublicMod.GetBackUrl();
                    int Id = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                    loadOp(Id, webOp);
                }
            }
            else
            {
                Header.Title += " - 系统设置 - 分类";
                plType.Visible = true;
                listType(webOp);
                if (!IsPostBack)
                {
                    hfBack.Value = PublicMod.GetBackUrl();
                    int Id = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                    loadType(Id);
                }
            }
        }
        //
        #region 选项分类
        //加载列表
        private void listType(WebOp webOp)
        {
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            DataOpType[] data = webType.GetDatas(0, "", "Id,TypeName,Remark,Active", pageCur, pageSize, "", "total");
            if (data != null)
            {
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
                    DataOp[] opData = webOp.GetDatas(1, data[i].TypeName, "", "Id");//获取子选项数
                    if (opData != null)
                    {
                        data[i].OpNum = opData.Count();
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
        private void loadType(int Id)
        {
            if (Id <= 0)
            {
                txtTypeAddTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                txtTypeAddIp.Text = HelperMain.GetIp();
                txtTypeAddUser.Text = myUser.AdminName;
                return;
            }
            DataOpType[] data = webType.GetData(Id);
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
            DataOpType data = new DataOpType();
            data.Id = Convert.ToInt32(txtTypeId.Text);
            data.TypeName = HelperMain.SqlFilter(txtTypeName.Text.Trim(), 20);
            DataOpType[] ckData = webType.GetDatas(0, data.TypeName, "Id");//重复检查
            if (ckData != null && ckData[0].Id != data.Id)
            {
                ltInfo.Text = "<script>$(function(){ alert('“分类名称”重复，不能添加！'); window.history.back(-1); });</script>";
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
                string oldTypeName = "";
                DataOpType[] oldData = webType.GetData(data.Id, "TypeName");
                if (oldData != null)
                {
                    oldTypeName = oldData[0].TypeName;
                }
                data.UpTime = dtNow;
                data.UpIp = strIp;
                data.UpUser = strUser;
                if (webType.Update(data) <= 0)
                {
                    data.Id = -1;
                }
                else if (!string.IsNullOrEmpty(oldTypeName))
                {//更新子选项
                    WebOp webOp = new WebOp();
                    webOp.UpdateOpType(oldTypeName, data.TypeName);
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
        #region 选项名称
        //加载列表
        private void listOp(int tId, WebOp webOp)
        {
            if (tId <= 0)
            {
                Response.Redirect("op.aspx");
            }
            DataOpType[] opType = webType.GetData(tId, "TypeName");
            if (opType == null)
            {
                Response.Redirect("op.aspx");
            }
            txtOpType.Text = opType[0].TypeName;

            DataOp[] data = webOp.GetDatas(0, opType[0].TypeName, "", "Id,OpType,OpName,OpValue,OpValue2,Selected,Remark,Active");//加载选项名
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = i + 1;
                    if (data[i].Selected)
                    {
                        data[i].SelectedName = "是";
                    }
                    if (data[i].Active > 0)
                    {
                        data[i].ActiveName = "正常";
                    }
                    else
                    {
                        data[i].ActiveName = "取消";
                        data[i].rowClass = " class='cancel'";
                    }
                }
                rpOpList.DataSource = data;
                rpOpList.DataBind();
                ltOpNo.Visible = false;
                ltOpTotal.Text = data.Count().ToString();//data[0].total.ToString();
            }
        }
        //加载信息
        private void loadOp(int Id, WebOp webOp)
        {
            if (Id <= 0)
            {
                txtOpAddTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                txtOpAddIp.Text = HelperMain.GetIp();
                txtOpAddUser.Text = myUser.AdminName;
                return;
            }
            DataOp[] data = webOp.GetData(Id);
            if (data != null)
            {
                txtOpId.Text = data[0].Id.ToString();
                txtOpName.Text = data[0].OpName;
                txtOpValue.Text = data[0].OpValue;
                txtOpValue2.Text = data[0].OpValue2;
                HelperMain.SetRadioSelected(rblSelected, data[0].Selected.ToString());
                txtOpRemark.Text = data[0].Remark;
                txtOpActive.Text = data[0].Active.ToString();
                txtOpAddTime.Text = data[0].AddTime.ToString("yyyy-MM-dd HH:mm:ss");
                txtOpAddIp.Text = data[0].AddIp;
                txtOpAddUser.Text = data[0].AddUser;
                if (!string.IsNullOrEmpty(data[0].UpIp) || !string.IsNullOrEmpty(data[0].UpUser))
                {
                    txtOpUpTime.Text = data[0].UpTime.ToString("yyyy-MM-dd HH:mm:ss");
                    txtOpUpIp.Text = data[0].UpIp;
                    txtOpUpUser.Text = data[0].UpUser;
                }
                btnOpEdit.Text = "修改";
                ltOpTitle.Text = ltOpTitle.Text.Replace("新增", "修改");
                btnOpCancel.Visible = true;
            }
        }
        //编辑数据
        protected void btnOpEdit_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            WebOp webOp = new WebOp();
            DataOp data = new DataOp();
            data.Id = Convert.ToInt32(txtOpId.Text);
            data.OpType = HelperMain.SqlFilter(txtOpType.Text.Trim(), 20);
            data.OpName = HelperMain.SqlFilter(txtOpName.Text.Trim(), 50);
            DataOp[] ckData = webOp.GetDatas(0, data.OpType, data.OpName, "Id");//重复检查
            if (ckData != null && ckData[0].Id != data.Id)
            {
                ltInfo.Text = "<script>$(function(){ alert('“选项名称”重复，不能添加！'); window.history.back(-1); });</script>";
                return;
            }
            data.OpValue = HelperMain.SqlFilter(txtOpValue.Text.Trim(), 50);
            data.OpValue2 = HelperMain.SqlFilter(txtOpValue2.Text.Trim(), 50);
            data.Selected = Convert.ToBoolean(rblSelected.SelectedValue);
            data.Remark = HelperMain.SqlFilter(txtOpRemark.Text.Trim(), 500);
            data.Active = (!string.IsNullOrEmpty(txtOpActive.Text.Trim())) ? Convert.ToInt16(txtOpActive.Text.Trim()) : 1;
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(myUser.AdminName, 20);
            if (data.Id <= 0)
            {
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = strUser;
                data.Id = webOp.Insert(data);
            }
            else
            {
                data.UpTime = dtNow;
                data.UpIp = strIp;
                data.UpUser = strUser;
                if (webOp.Update(data) <= 0)
                {
                    data.Id = -1;
                }
            }
            if (data.Id > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltOpTitle.Text + "”成功！'); window.location.href='" + Request.Url.ToString() + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltOpTitle.Text + "”失败！'); window.history.back(-1); });</script>";
            }
        }
        //取消状态
        protected void btnOpCancel_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            int Id = Convert.ToInt32(txtOpId.Text);
            if (Id <= 0)
            {
                return;
            }
            WebOp webOp = new WebOp();
            if (webOp.UpdateActive(Id, -1) > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnOpCancel.Text + "成功！'); window.location.href='" + Request.Url.ToString() + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnOpCancel.Text + "失败！'); window.history.back(-1); });</script>";
            }
        }
        #endregion
        //
    }
}