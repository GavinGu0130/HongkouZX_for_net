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
    public partial class news : System.Web.UI.Page
    {
        private DataAdmin myUser = null;
        private WebNews webNews = new WebNews();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperAdmin.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
            {
                //Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            if (myUser.Powers.IndexOf("alls") < 0 && myUser.Powers.IndexOf("news") < 0)
            {
                Response.Redirect("./");
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.Powers = myUser.Powers;
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    hfBack.Value = PublicMod.GetBackUrl();
                    plEdit.Visible = true;
                    loadData(Convert.ToInt32(Request.QueryString["id"]));
                    Header.Title += " - " + ltEditTitle.Text;
                }
                else
                {
                    Header.Title += " - 查询新闻";
                    plList.Visible = true;
                    queryData();
                }
            }
        }
        //首页列表
        public void MyList(Repeater rpList, Literal ltNo)
        {
            DataNews qData = new DataNews();
            qData.ActiveName = ">0";
            qData.ShowTimeText = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd") + ",";
            listData(qData, rpList, ltNo);
        }
        //查询列表
        private void queryData()
        {
            DataNews qData = new DataNews();
            if (!string.IsNullOrEmpty(Request.QueryString["SubType"]))
            {
                qData.SubType = HelperMain.SqlFilter(Request.QueryString["SubType"], 20);
                HelperMain.SetCheckSelected(cblQSubType, qData.SubType);
            }
            //if (!string.IsNullOrEmpty(Request.QueryString["Client"]))
            //{
            //    qData.Client = HelperMain.SqlFilter(Request.QueryString["Client"], 20);
            //}
            if (!string.IsNullOrEmpty(Request.QueryString["Title"]))
            {
                qData.Title = "%" + HelperMain.SqlFilter(Request.QueryString["Title"], 50) + "%";
                txtQTitle.Text = qData.Title.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ShowTime"]) && Request.QueryString["ShowTime"].IndexOf(",") >= 0)
            {
                string strShowTime1 = Request.QueryString["ShowTime"].Substring(0, Request.QueryString["ShowTime"].IndexOf(","));
                string strShowTime2 = Request.QueryString["ShowTime"].Substring(Request.QueryString["ShowTime"].IndexOf(",") + 1);
                txtQShowTime1.Text = HelperMain.SqlFilter(strShowTime1.Trim(), 10);
                txtQShowTime2.Text = HelperMain.SqlFilter(strShowTime2.Trim(), 10);
                if (txtQShowTime1.Text != "" || txtQShowTime2.Text != "")
                {
                    qData.ShowTimeText = txtQShowTime1.Text + "," + txtQShowTime2.Text;
                }
            }
            else if (Request.QueryString["ac"] != "query")
            {
                txtQShowTime1.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                qData.ShowTimeText = txtQShowTime1.Text + ",";
            }
            listData(qData, rpNewsList, ltNewsNo, lblNewsNav, ltNewsTotal, this);
        }
        //加载列表
        private void listData(DataNews qData, Repeater rpList, Literal ltNo, Label lblNav = null, Literal ltTotal = null, Page page = null)
        {
            int pageCur = 1;
            if (lblNav != null && !string.IsNullOrEmpty(page.Request.QueryString["page"]))
            {
                pageCur = Convert.ToInt32(page.Request.QueryString["page"]);
            }
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            string strOrder = "";
            DataNews[] data = webNews.GetDatas(qData, "", pageCur, pageSize, strOrder, "total");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    if (!string.IsNullOrEmpty(data[i].PicUrl))
                    {
                        data[i].PicUrl = string.Format("<img src='{0}'/>", data[i].PicUrl);
                    }
                    if (!string.IsNullOrEmpty(data[i].Body))
                    {
                        data[i].Body = data[i].Body.Replace("\n", "<br/>");
                    }
                    if (data[i].Active > 0)
                    {
                        data[i].ActiveName = "正常";
                    }
                    else if (data[i].Active < 0)
                    {
                        data[i].ActiveName = "取消";
                        data[i].rowClass = " class='cancel' title='取消'";
                    }
                    else
                    {
                        data[i].ActiveName = "暂停";
                        data[i].rowClass = " class='save' title='暂停'";
                    }
                    //data[i].ShowTimeText = (data[i].ShowTime < DateTime.Today) ? data[i].ShowTime.ToString("yyyy-MM-dd") : data[i].ShowTime.ToString("HH:mm:ss");
                    //if (data[i].ShowTimeText == "00:00:00")
                    //{
                    //    data[i].ShowTimeText = data[i].ShowTime.ToString("yyyy-MM-dd");
                    //}
                }
                rpList.DataSource = data;
                rpList.DataBind();
                if (ltNo != null)
                {
                    ltNo.Visible = false;
                }
                if (lblNav != null)
                {
                    int pageCount = data[0].total;
                    int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                    string lnk = page.Request.Url.ToString();
                    lblNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
                }
                if (ltTotal != null)
                {
                    ltTotal.Text = data[0].total.ToString();
                }
            }
        }
        //加载信息
        private void loadData(int Id)
        {
            for (int i = 0; i < cblQSubType.Items.Count; i++)
            {
                rblSubType.Items.Add(cblQSubType.Items[i]);
            }
            if (Id <= 0)
            {
                txtAddTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                txtAddIp.Text = HelperMain.GetIp();
                txtAddUser.Text = myUser.AdminName;
                txtShowTime.Text = txtAddTime.Text;
                return;
            }
            DataNews[] data = webNews.GetData(Id);
            if (data != null)
            {
                txtId.Text = data[0].Id.ToString();
                for (int i = 0; i < rblSubType.Items.Count; i++)
                {
                    if (rblSubType.Items[i].Text == data[0].SubType)
                    {
                        rblSubType.SelectedIndex = i;
                        break;
                    }
                }
                //data[0].Client
                txtTitle.Text = data[0].Title;
                txtLnkUrl.Text = data[0].LnkUrl;
                txtPicUrl.Text = data[0].PicUrl;
                txtVideo.Text = data[0].Video;
                txtBody.Text = data[0].Body;
                txtRemark.Text = data[0].Remark;
                HelperMain.SetRadioSelected(rblActive, data[0].Active.ToString());
                txtReadNum.Text = data[0].ReadNum.ToString();
                txtShowTime.Text = data[0].ShowTime.ToString("yyyy-MM-dd HH:mm:ss");
                txtAddTime.Text = data[0].AddTime.ToString("yyyy-MM-dd HH:mm:ss");
                txtAddIp.Text = data[0].AddIp;
                txtAddUser.Text = data[0].AddUser;
                if (!string.IsNullOrEmpty(data[0].UpIp) || !string.IsNullOrEmpty(data[0].UpUser))
                {
                    txtUpTime.Text = data[0].UpTime.ToString("yyyy-MM-dd HH:mm:ss");
                    txtUpIp.Text = data[0].UpIp;
                    txtUpUser.Text = data[0].UpUser;
                }
                btnEdit.Text = "更新";
                ltEditTitle.Text = ltEditTitle.Text.Replace("发布", "更新");
            }
        }
        //编辑数据
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            DataNews data = new DataNews();
            data.Id = Convert.ToInt32(txtId.Text);
            data.SubType = HelperMain.SqlFilter(rblSubType.SelectedValue, 8);
            data.Title = HelperMain.SqlFilter(txtTitle.Text.Trim(), 50);
            DataNews[] ckData = webNews.GetDatas(data, "Id");//重复检查
            if (ckData != null && ckData[0].Id != data.Id)
            {
                ltInfo.Text = "<script>$(function(){ alert('“新闻标题”重复，不能添加！'); window.history.back(-1); });</script>";
                return;
            }
            //data.Client
            data.LnkUrl = HelperMain.SqlFilter(txtLnkUrl.Text.Trim());
            data.PicUrl = HelperMain.SqlFilter(txtPicUrl.Text.Trim());
            data.Video = HelperMain.SqlFilter(txtVideo.Text.Trim());
            data.Body = HelperMain.SqlFilter(txtBody.Text.Trim());
            data.Remark = HelperMain.SqlFilter(txtRemark.Text.Trim(), 500);
            data.Active = Convert.ToInt16(rblActive.SelectedValue);
            //data.ReadNum = (!string.IsNullOrEmpty(txtReadNum.Text)) ? Convert.ToInt64(txtReadNum.Text.Trim()) : 0;
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(myUser.AdminName, 20);
            data.ShowTime = (!string.IsNullOrEmpty(txtShowTime.Text)) ? Convert.ToDateTime(txtShowTime.Text.Trim()) : dtNow;
            if (data.Id <= 0)
            {
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = strUser;
                data.Id = webNews.Insert(data);
            }
            else
            {
                data.UpTime = dtNow;
                data.UpIp = strIp;
                data.UpUser = strUser;
                if (webNews.Update(data) <= 0)
                {
                    data.Id = -1;
                }
            }
            if (data.Id > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltEditTitle.Text + "”成功！'); window.location.href='" + hfBack.Value + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltEditTitle.Text + "”失败！'); window.history.back(-1); });</script>";
            }
        }
        //
    }
}