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
    public partial class ucHeader : System.Web.UI.UserControl
    {
        public string Title = "";
        public string Cur = "";
        public string UserName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Cur != "login")
            {
                string client = HelperMain.GetClient("no");
                if (client == "wx" || client == "mobile" || client == "wap")
                {
                }
                else
                {
                    Response.Redirect("../cn/");
                }
                if (Cur == "home")
                {
                    lnkBack.Visible = false;
                }
            }
            if (!string.IsNullOrEmpty(Title))
            {
                lblTitle.Text = Title;
            }
            if (!string.IsNullOrEmpty(UserName))
            {
                ltUser.Text = UserName;
                plUser.Visible = true;
                if (HelperMain.GetIsWx(5) || Request.IsLocal)
                {
                    string UserWx = "";
                    DataUser myUser = HelperUser.GetUser();
                    if (myUser != null && myUser.Id > 0 && myUser.TrueName == UserName)
                    {
                        WebUserWx webUserWx = new WebUserWx();
                        DataUserWx[] data = webUserWx.GetDatas(config.APPID, myUser.Id, "", "WxNickName");
                        if (data != null)
                        {
                            UserWx = data[0].WxNickName;
                        }
                    }
                    if (!string.IsNullOrEmpty(UserWx))
                    {
                        lnkUnBind.Visible = true;
                        lnkUnBind.NavigateUrl += "&url=" + HttpUtility.UrlEncode(Request.Url.ToString());
                        lnkUnBind.Text = lnkUnBind.Text.Replace("微信信息", UserWx);
                    }
                    else
                    {
                        lnkWxBind.Visible = true;
                        lnkWxBind.NavigateUrl += "&url=" + HttpUtility.UrlEncode(Request.Url.ToString());
                    }
                }
            }
        }
        //
    }
}