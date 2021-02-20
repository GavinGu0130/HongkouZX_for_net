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
    public partial class news : System.Web.UI.Page
    {
        private string strTableName = "tb_News";
        private DataUser myUser = null;
        private WebNews webNews = new WebNews();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.TrueName))
            {
                //Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            string strTitle = "";
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                strTitle = LoadData(Convert.ToInt32(Request.QueryString["id"]), rpView, this, myUser);
            }
            else
            {
                switch (Request.QueryString["ac"])
                {
                    case "news":
                        strTitle = "政协要闻";
                        plNews.Visible = true;
                        ListData("政协要闻", rpNewsList, ltNewsNo, lblNewsNav, ltNewsTotal, this);
                        break;
                    case "video":
                        strTitle = "视频新闻";
                        plVideo.Visible = true;
                        ListData("视频新闻", rpVideoList, null, lblVideoNav, null, this);
                        break;
                    default:
                        break;
                }
            }
            Header.Title += " - " + strTitle;
        }
        //加载列表
        public void ListData(string SubType, Repeater rpList, Literal ltNo = null, Label lblNav = null, Literal ltTotal = null, Page page = null)
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
            DataNews qData = new DataNews();
            qData.ActiveName = ">0";
            qData.SubType = SubType;
            DataNews[] data = webNews.GetDatas(qData, "Id,SubType,Title,PicUrl,Body,ReadNum,ShowTime", pageCur, pageSize, "", "total");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    if (!string.IsNullOrEmpty(data[i].PicUrl))
                    {
                        data[i].PicUrl = string.Format("<img src='{0}' />", data[i].PicUrl);
                    }
                    data[i].ShowTimeText = (data[i].ShowTime < DateTime.Today) ? data[i].ShowTime.ToString("yyyy-MM-dd") : data[i].ShowTime.ToString("[HH:mm]");
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
        public string LoadData(int Id, Repeater rpList, Page page, DataUser uData)
        {
            string strTitle = "";
            if (Id <= 0)
            {
                return "";
            }
            DataNews[] data = webNews.GetData(Id, "SubType,Title,LnkUrl,PicUrl,Video,Body,ReadNum,ShowTime");
            if (data != null)
            {
                webNews.AddReadNum(Id);//增加浏览数
                if (!string.IsNullOrEmpty(data[0].LnkUrl))
                {
                    page.Response.Redirect(data[0].LnkUrl);//直接跳转到链接
                    return "";
                }

                data[0].ReadNum += 1;
                if (data[0].SubType == "视频新闻")
                {
                    //ltViewBody.Text = string.Format("<embed type='application/x-mplayer2' src='{0}' enablecontextmenu='false' autostart='false' />", data[0].Video);
                    //data[0].Body = string.Format("<embed type='application/x-mplayer2' src='{0}' enablecontextmenu='false' autostart='false' />", data[0].Video);
                    data[0].Body = string.Format("<iframe src='{0}' autostart='false' width='100%' height='100%'  loop='false'></iframe>", data[0].Video);
                    strTitle = "视频新闻";
                }
                else
                {
                    data[0].Body = data[0].Body.Replace("\n", "<br/>");
                    strTitle = "政协要闻";
                }
                rpList.DataSource = data;
                rpList.DataBind();
                //strTitle += "-" + data[0].Title;
                PublicMod.AddFeed(strTableName, Id, uData);//增加浏览反馈
            }
            return strTitle;
        }
        //获取未反馈数
        public int GetFeedNum(string SubType, DataUser uData)
        {
            int intNum = 0;
            int pageCur = 1;
            int pageSize = 1000;//查询最近1000条
            DataNews qData = new DataNews();
            qData.ActiveName = ">0";
            qData.SubType = SubType;
            qData.ShowTimeText = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd") + ",";
            DataNews[] data = webNews.GetDatas(qData, "Id", pageCur, pageSize);
            if (data != null)
            {
                intNum = data.Count();
                WebFeedback webFeed = new WebFeedback();
                for (int i = 0; i < data.Count(); i++)
                {
                    DataFeedback[] fData = webFeed.GetDatas(">0", strTableName, data[i].Id, uData.Id, "", "Id");
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