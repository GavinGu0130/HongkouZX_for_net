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
    public partial class notice : System.Web.UI.Page
    {
        private DataUser myUser = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.TrueName))
            {
                if (HelperMain.GetIsWx(5))
                {
                    Response.Redirect("../cn/login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                }
                return;
            }
            header1.UserName = myUser.TrueName;
            string strTitle = "";
            cn.notice page = new cn.notice();
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                strTitle = page.LoadData(Convert.ToInt32(Request.QueryString["id"]), plView, rpView, myUser);
            }
            else
            {
                plList.Visible = true;
                page.ListData(myUser.TrueName, rpList, ltNo, lblNav, null, this);
            }
            Header.Title += " - 信息发布" + strTitle;
        }
        //
    }
}