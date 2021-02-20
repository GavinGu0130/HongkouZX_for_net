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
    public partial class notice : System.Web.UI.Page
    {
        private string strTableName = "tb_Notice";
        private DataUser myUser = null;
        private WebNotice webNotice = new WebNotice();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.TrueName))
            {
                Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.UserType = myUser.UserType;
            string strTitle = "";
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    strTitle = LoadData(Convert.ToInt32(Request.QueryString["id"]), plView, rpView, myUser);
                }
                else
                {
                    plList.Visible = true;
                    ListData(myUser.TrueName, rpNoticeList, ltNoticeNo, lblNoticeNav, ltNoticeTotal, this);
                }
            }
            Header.Title += " - 信息发布" + strTitle;
        }
        //加载列表
        public void ListData(string strUser, Repeater rpList, Literal ltNo = null, Label lblNav = null, Literal ltTotal = null, Page page = null)
        {
            DataNotice qData = new DataNotice();
            qData.ActiveName = ">0";
            qData.ToMans = strUser;
            qData.OverTimeText = DateTime.Now.ToString("yyyy-MM-dd HH:mm,");
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
            DataNotice[] data = webNotice.GetDatas(qData, "Id,SubType,Title,Body,Files,Active", pageCur, pageSize, "", "total");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    if (data[i].Active >= 10)
                    {
                        //data[i].DegreeText = "重要";
                        data[i].rowClass = " class='cancel' title='重要'";
                    }
                    if (!string.IsNullOrEmpty(data[i].Body))
                    {
                        data[i].Body = data[i].Body.Replace("\n", "<br/>");
                    }
                    if (!string.IsNullOrEmpty(data[i].Files))
                    {
                        data[i].Files = string.Format("<a href='{0}' target='_blank'><u>附件下载</u></a>", data[i].Files);
                    }
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
        public string LoadData(int Id, PlaceHolder _plView, Repeater rpList, DataUser uData)
        {
            string strTitle = "";
            if (Id > 0)
            {
                DataNotice[] data = webNotice.GetData(Id, "Id,SubType,ToMans,OverTime,Title,Body,Files,Active,ReadNum,ShowTime");
                if (data != null && data[0].Active > 0)
                {
                    _plView.Visible = true;
                    webNotice.AddReadNum(Id);//增加浏览数
                    data[0].ReadNum += 1;
                    data[0].Body = data[0].Body.Replace("\n", "<br/>");
                    rpList.DataSource = data;
                    rpList.DataBind();
                    //addFeed(data[0].Id, uData);
                    PublicMod.AddFeed(strTableName, Id, uData);//增加浏览反馈
                    strTitle = " - " + data[0].Title;
                }
            }
            return "";
        }
        //获取未反馈数
        public int GetFeedNum(DataUser uData)
        {
            int intNum = 0;
            DataNotice qData = new DataNotice();
            qData.ActiveName = ">0";
            qData.ToMans = uData.TrueName;
            qData.OverTimeText = DateTime.Now.ToString("yyyy-MM-dd HH:mm,");
            int pageCur = 1;
            int pageSize = 1000;//查询最近1000条
            DataNotice[] data = webNotice.GetDatas(qData, "Id", pageCur, pageSize);
            if (data != null)
            {
                intNum = data.Count();
                //WebNoticeFeedback webFeed = new WebNoticeFeedback();
                //for (int i = 0; i < data.Count(); i++)
                //{
                //    DataNoticeFeedback[] fData = webFeed.GetDatas(">0", data[i].Id, uData.Id, "", "Id");
                //    if (fData != null)
                //    {
                //        intNum--;
                //    }
                //}
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