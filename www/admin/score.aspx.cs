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
    public partial class score : System.Web.UI.Page
    {
        private DataAdmin myUser = null;
        private WebScore webScore = new WebScore();
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
            if (!IsPostBack)
            {
                hfBack.Value = PublicMod.GetBackUrl();
                plScore.Visible = true;
                Header.Title += " - 积分设置";
                int Id = 0;
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    Id = Convert.ToInt32(Request.QueryString["id"]);
                }
                loadData(Id);
                listData();
            }
        }
        //加载列表
        private void listData()
        {
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            string strOrder = "Active DESC, ScoreType ASC, AddTime ASC";
            DataScore[] data = webScore.GetDatas(0, "", "", null, "", pageCur, pageSize, strOrder, "total");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    data[i].ScoreText = data[i].Score.ToString();
                    if (data[i].Score2 > 0)
                    {
                        data[i].ScoreText += "-" + data[i].Score2.ToString();
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
                rpList.DataSource = data;
                rpList.DataBind();
                ltNo.Visible = false;
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = Request.Url.ToString();
                lblNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
                ltTotal.Text = data[0].total.ToString();
            }
        }
        //加载信息
        private void loadData(int Id)
        {
            WebOp webOp = new WebOp();
            PublicMod.LoadDropDownList(ddlScoreType, webOp, "积分类别");
            if (Id <= 0)
            {
                txtAddTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                txtAddIp.Text = HelperMain.GetIp();
                txtAddUser.Text = myUser.AdminName;
                return;
            }
            DataScore[] data = webScore.GetData(Id);
            if (data != null)
            {
                txtId.Text = data[0].Id.ToString();
                HelperMain.SetDownSelected(ddlScoreType, data[0].ScoreType);
                txtTitle.Text = data[0].Title;
                txtScore.Text = data[0].Score.ToString();
                txtScore2.Text = data[0].Score2.ToString();
                txtUnit.Text = data[0].Unit;
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
                btnEdit.Text = "修改";
                ltTitle.Text = ltTitle.Text.Replace("新增", "修改");
                btnCancel.Visible = true;
            }
        }
        //编辑数据
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            DataScore data = new DataScore();
            data.Id = Convert.ToInt32(txtId.Text);
            data.ScoreType = HelperMain.SqlFilter(ddlScoreType.SelectedValue.Trim(), 20);
            data.Title = HelperMain.SqlFilter(txtTitle.Text.Trim(), 50);
            data.Score = Convert.ToDecimal(txtScore.Text);
            data.Score2 = Convert.ToDecimal(txtScore2.Text);
            data.Unit = HelperMain.SqlFilter(txtUnit.Text.Trim(), 4);
            data.Remark = HelperMain.SqlFilter(txtRemark.Text.Trim(), 200);
            data.Active = (!string.IsNullOrEmpty(txtActive.Text.Trim())) ? Convert.ToInt16(txtActive.Text.Trim()) : 1;
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(myUser.AdminName, 20);
            if (data.Id <= 0)
            {
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = strUser;
                data.Id = webScore.Insert(data);
            }
            else
            {
                data.UpTime = dtNow;
                data.UpIp = strIp;
                data.UpUser = strUser;
                if (webScore.Update(data) <= 0)
                {
                    data.Id = -1;
                }
            }
            if (data.Id > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltTitle.Text + "”成功！'); window.location.href='" + hfBack.Value + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltTitle.Text + "”失败！'); window.history.back(-1); });</script>";
            }
        }
        //取消状态
        protected void btnCancel_Click(object sender, EventArgs e)
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
            if (webScore.UpdateActive(Id, -1) > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnCancel.Text + "成功！'); window.location.href='" + hfBack.Value + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnCancel.Text + "失败！'); window.history.back(-1); });</script>";
            }
        }
        //
    }
}