using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mod.main;
using hkzx.db;
using hkzx.user;

namespace hkzx.web.cn
{
    public partial class datas : System.Web.UI.Page
    {
        private string strTableName = "tb_Datas";
        private DataUser myUser = null;
        private WebDatasType webType = new WebDatasType();
        private DataDatasType[] dataType = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null)
            {
                Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.UserType = myUser.UserType;
            string strTitle = "";
            int tId = ListType(rpType);
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                strTitle = "资料文档";
                strTitle += " - " + LoadDatas(Convert.ToInt32(Request.QueryString["id"]), rpView, myUser);
            }
            else if (Request.QueryString["ac"] == "query")
            {
                strTitle = "资料文档查询";
                plQuery.Visible = true;
                DataDatas qData = getQData();
                ListDatas(qData, rpQueryList, ltQueryNo, lblQueryNav, ltQueryTotal, this);
            }
            else
            {
                strTitle = "资料文档";
                plList.Visible = true;
                if (!string.IsNullOrEmpty(Request.QueryString["tid"]))
                {
                    tId = Convert.ToInt32(Request.QueryString["tid"]);
                }
                if (dataType != null)
                {
                    for (int i = 0; i < dataType.Count(); i++)
                    {
                        if (dataType[i].Id == tId)
                        {
                            strTitle += " - " + dataType[i].TypeName;
                            break;
                        }
                    }
                }
                lnkMore.NavigateUrl += tId.ToString();
                DataDatas qData = new DataDatas();
                qData.TypeId = tId;
                ListDatas(qData, rpDatasList, ltDatasNo, lblDatasNav, null, this);
            }
            Header.Title += " - " + strTitle;
        }
        //加载类型列表
        public int ListType(Repeater rpList)
        {
            int tId = 0;
            DataDatasType qData = new DataDatasType();
            qData.Active = 1;
            DataDatasType[] data = webType.GetDatas(qData, "Id,TypeName");
            if (data != null)
            {
                tId = data[0].Id;
                rpList.DataSource = data;
                rpList.DataBind();
                dataType = data;
            }
            return tId;
        }
        //加载查询
        private DataDatas getQData()
        {
            DataDatas data = new DataDatas();
            if (!string.IsNullOrEmpty(Request.QueryString["TypeId"]))
            {
                data.TypeId = Convert.ToInt32(Request.QueryString["TypeId"]);
            }
            if (dataType != null)
            {
                int intCur = 0;
                for (int i = 0; i < dataType.Count(); i++)
                {
                    if (dataType[i].Id == data.TypeId)
                    {
                        intCur = i;
                    }
                    ListItem item = new ListItem(dataType[i].TypeName, dataType[i].Id.ToString());
                    ddlQTypeId.Items.Add(item);
                }
                ddlQTypeId.SelectedIndex = intCur;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Title"]))
            {
                data.Title = "%" + HelperMain.SqlFilter(Request.QueryString["Title"].Trim(), 50) + "%";
                txtQTitle.Text = data.Title.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Body"]))
            {
                data.Title = "%" + HelperMain.SqlFilter(Request.QueryString["Body"].Trim(), 200) + "%";
                txtQBody.Text = data.Title.Trim('%');
            }
            return data;
        }
        //加载列表
        public void ListDatas(DataDatas qData, Repeater rpList, Literal ltNo, Label lblNav, Literal ltTotal = null, Page page = null)
        {
            if (qData.TypeId <= 0)
            {
                return;
            }
            int pageCur = (!string.IsNullOrEmpty(page.Request.QueryString["page"])) ? Convert.ToInt32(page.Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            qData.Active = 1;
            WebDatas webDatas = new WebDatas();
            DataDatas[] data = webDatas.GetDatas(qData, "Id,TypeId,Title,Files,UpTime", pageCur, pageSize, "", "total");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    data[i].Files = string.Format("<a href='{0}' target='_blank' class='btn btn2'><u>资料文档下载</u></a>", data[i].Files);
                }
                rpList.DataSource = data;
                rpList.DataBind();
                ltNo.Visible = false;
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = page.Request.Url.ToString();
                lblNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
                if (ltTotal != null)
                {
                    ltTotal.Text = data[0].total.ToString();
                }
            }
        }
        //加载资料文档
        public string LoadDatas(int Id, Repeater rpList, DataUser uData)
        {
            string strTitle = "";
            if (Id <= 0)
            {
                return "";
            }
            WebDatas webDatas = new WebDatas();
            DataDatas[] data = webDatas.GetData(Id);
            if (data != null)
            {
                webDatas.AddReadNum(Id);//增加浏览数
                if (dataType != null)
                {
                    for (int i = 0; i < dataType.Count(); i++)
                    {
                        if (dataType[i].Id == data[0].TypeId)
                        {
                            data[0].TypeName = dataType[i].TypeName;
                            break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(data[0].Files))
                {
                    data[0].Files = string.Format("<a href='{0}' target='_blank' class='btn'><u>资料文档下载</u></a>", data[0].Files);
                }
                rpList.DataSource = data;
                rpList.DataBind();
                strTitle = data[0].Title;
                PublicMod.AddFeed(strTableName, Id, uData);//增加浏览反馈
            }
            return strTitle;
        }
        //获取未反馈数
        public int GetFeedNum(DataUser uData)
        {
            int intNum = 0;
            DataDatas qData = new DataDatas();
            qData.ShowTimeText = string.Format("{0:yyyy-MM-dd},", DateTime.Today.AddMonths(-12));
            int pageCur = 1;
            int pageSize = 1000;//查询最近1000条
            WebDatas webDatas = new WebDatas();
            DataDatas[] data = webDatas.GetDatas(qData, "Id", pageCur, pageSize);
            if (data != null)
            {
                intNum = data.Count();
                WebFeedback webFeed = new WebFeedback();
                for (int i = 0; i < data.Count(); i++)
                {
                    DataFeedback[] fData = webFeed.GetDatas(">0", strTableName, data[i].Id, uData.Id, "Id");
                    if (fData != null)
                    {
                        intNum--;
                    }
                }
            }
            return intNum;
        }
        //
    }
}