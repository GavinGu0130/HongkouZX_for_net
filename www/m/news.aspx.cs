using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mod.main;
using hkzx.db;
using hkzx.user;

namespace hkzx.web.m
{
    public partial class news : System.Web.UI.Page
    {
        private DataUser myUser = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.TrueName))
            {
                Response.Redirect("../cn/login.aspx?url=?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            header1.UserName = myUser.TrueName;
            cn.news page = new cn.news();
            string strTitle = "";
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                strTitle = page.LoadData(Convert.ToInt32(Request.QueryString["id"]), rpView, this, myUser);
                if (strTitle.IndexOf("-") >= 0)
                {
                    strTitle = strTitle.Substring(strTitle.IndexOf("-") + 1);
                }
            }
            else
            {
                switch (Request.QueryString["ac"])
                {
                    case "news":
                        strTitle = "政协要闻";
                        plNews.Visible = true;
                        page.ListData("政协要闻", rpNewsList, ltNewsNo, lblNewsNav, null, this);
                        break;
                    case "video":
                        strTitle = "视频新闻";
                        plVideo.Visible = true;
                        page.ListData("视频新闻", rpVideoList, null, lblVideoNav, null, this);
                        break;
                    default:
                        break;
                }
            }
            Header.Title += " - " + strTitle;
            header1.Title = strTitle;
        }
        //
    }
}