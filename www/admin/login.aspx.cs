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
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["ac"] == "logout")
                {
                    HelperAdmin.Logout();//退出
                    string strBack = (!string.IsNullOrEmpty(Request.QueryString["url"])) ? Request.QueryString["url"] : "./";
                    Response.Redirect(strBack);
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["url"]))
                    {
                        hfBack.Value = Request.QueryString["url"];
                        plLogin.CssClass += " login2";
                    }
                    else
                    {
                        header1.Visible = false;
                        footer1.Visible = false;
                    }
                }
            }
        }
        //登录
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string strCookie = code.strCookie;
            string strUser = txtUser.Text.Trim();
            string strPwd = txtPwd.Text.Trim();
            string strCode = txtCode.Text.Trim();
            if (string.IsNullOrEmpty(strUser) || string.IsNullOrEmpty(strPwd) || string.IsNullOrEmpty(strCode))
            {
                ltInfo.Text = "<script>$(function(){ alert('“用户名、密码、验证码”都不能为空！'); });</script>";
                return;
            }
            if (Request.Cookies[strCookie].Value != strCode.ToUpper())
            {
                ltInfo.Text = "<script>$(function(){ alert('“验证码”输入错误！'); });</script>";
                return;
            }
            string strIp = HelperMain.GetIpPort();
            DateTime dtNow = DateTime.Now;
            string strPwdMd5 = strPwd;//HelperSecret.MD5Encrypt(strPwd);
            WebAdmin webAdmin = new WebAdmin();
            DataAdmin[] data = webAdmin.Login(strIp, dtNow, strPwdMd5, strUser);
            if (data == null)
            {
                ltInfo.Text = "<script>$(function(){ alert('“用户名、密码”不正确！'); });</script>";
                return;
            }
            else if (data[0].ErrNum > 10 || data[0].Active <= 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“密码”错误次数过多，\\n或“帐户”已被锁定！'); });</script>";
                return;
            }
            else
            {
                data[0].LastTime = dtNow;
                HelperAdmin.SetUser(data[0]);//设置cookie
                string strLog = HelperMain.GetClient("no") + "后台登录";
                PublicMod.WriteLog("ad_Login", data[0].Id, strLog, "u_" + data[0].Id.ToString());//登录日志
                if (!string.IsNullOrEmpty(Request.QueryString["url"]))
                {
                    Response.Redirect(hfBack.Value);
                }
                else
                {
                    ltInfo.Text = "登录成功！<script>top.window.location.reload();</script>";
                }
            }
        }
        //
    }
}