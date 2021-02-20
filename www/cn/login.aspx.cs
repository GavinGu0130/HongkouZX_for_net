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
    public partial class login : System.Web.UI.Page
    {
        DataUser myUser = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["ac"] == "logout")
                {
                    HelperUser.Logout();//退出
                    string strBack = (!string.IsNullOrEmpty(Request.QueryString["url"])) ? Request.QueryString["url"] : "./";
                    Response.Redirect(strBack);
                }
                else if (Request.QueryString["ac"] == "pwd")
                {
                    myUser = HelperUser.GetUser();
                    if (myUser == null || myUser.Id <= 0)
                    {
                        Response.Redirect("./");
                        return;
                    }
                }
                bool isDialog = true;
                if (!string.IsNullOrEmpty(Request.QueryString["url"]))
                {
                    isDialog = false;
                }
                string strTitle = "";
                if (Request.QueryString["ac"] == "pwd")
                {
                    strTitle = "修改密码";
                    plPwd.Visible = true;
                    if (!IsPostBack)
                    {
                        hfBack.Value = PublicMod.GetBackUrl("./");
                        txtName.Text = myUser.TrueName;
                    }
                    isDialog = false;
                }
                else
                {
                    strTitle = "登录";
                    plLogin.Visible = true;
                    if (HelperMain.GetIsWx(5))
                    {
                        string strUrl = (!string.IsNullOrEmpty(Request.QueryString["url"])) ? Request.QueryString["url"] : "../m/";
                        lnkWx.NavigateUrl += "&url=" + HttpUtility.UrlEncode(strUrl);
                        lnkWx.Visible = true;
                    }
                    //if (!string.IsNullOrEmpty(Request.Form["txtUser"]) && !string.IsNullOrEmpty(Request.Form["txtPwd"]))
                    //{
                    //    modLogin(Request.Form["txtUser"], Request.Form["txtPwd"]);
                    //}
                }
                string client = HelperMain.GetClient("no");
                meta1.Client = client;
                if (!isDialog)
                {
                    if (client == "wx" || client == "mobile" || client == "wap")
                    {
                        header2.Visible = true;
                    }
                    else
                    {
                        header1.Visible = true;
                        footer1.Visible = true;
                        if (myUser != null)
                        {
                            header1.UserName = myUser.TrueName;
                            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        Header.Title += " - " + strTitle;
                    }
                    if (Request.QueryString["ac"] == "pwd")
                    {
                        plPwd.CssClass += " login2";
                    }
                    else
                    {
                        plLogin.CssClass += " login2";
                    }
                }
            }
        }
        //登录
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string strUser = txtUser.Text.Trim();
            string strPwd = txtPwd.Text.Trim();
            if (!string.IsNullOrEmpty(strUser) && !string.IsNullOrEmpty(strPwd))
            {
                modLogin(strUser, strPwd);
            }
        }
        private void modLogin(string strUser, string strPwd)
        {
            string strCode = txtCode.Text.Trim();
            if (string.IsNullOrEmpty(strCode))
            {
                ltInfo.Text = "<script>$(function(){ alert('“验证码”不能为空！'); });</script>";
                return;
            }
            string strCookie = code.strCookie;
            if (Request.Cookies[strCookie].Value != strCode.ToUpper())
            {
                ltInfo.Text = "<script>$(function(){ alert('“验证码”输入错误！'); });</script>";
                return;
            }
            if (string.IsNullOrEmpty(strUser) || string.IsNullOrEmpty(strPwd))
            {
                ltInfo.Text = "<script>$(function(){ alert('“用户名、密码”不能为空！'); });</script>";
                return;
            }
            string strIp = HelperMain.GetIpPort();
            DateTime dtNow = DateTime.Now;
            string strPwdMd5 = strPwd;//HelperSecret.MD5Encrypt(strPwd);//
            WebUser webUser = new WebUser();
            DataUser[] data = webUser.Login(strIp, dtNow, strPwdMd5, "", strUser, config.PERIOD);
            if (data == null)
            {
                ltInfo.Text = "<script>$(function(){ alert('“用户名、密码”不正确！'); });</script>";
            }
            else if (data[0].ErrNum > 10 || data[0].Active <= 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“密码”错误次数过多，\\n或“帐户”已被锁定！'); });</script>";
            }
            else
            {
                PublicMod.ReOrderScore(data[0]);//按积分重新排序
                HelperUser.SetUser(data[0]);//设置用户cookie
                string strLog = HelperMain.GetClient("no") + "登录";
                PublicMod.WriteLog("user_Login", data[0].Id, strLog, "u_" + data[0].Id.ToString());//登录日志
                if (!string.IsNullOrEmpty(Request.QueryString["url"]))
                {
                    ltInfo.Text = "登录成功！<script>top.window.location.replace('" + Request.QueryString["url"] + "');</script>";
                    //Response.Redirect(Request.QueryString["url"]);
                }
                else
                {
                    ltInfo.Text = "登录成功！<script>top.window.location.reload();</script>";
                }
            }
        }
        //修改密码
        protected void btnPwd_Click(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null || myUser.TrueName != txtName.Text)
            {
                Response.Redirect("login.aspx");
            }
            string strOld = txtOld.Text.Trim();
            string strNew = txtNew.Text.Trim();
            if (string.IsNullOrEmpty(strOld) || string.IsNullOrEmpty(strNew))
            {
                ltInfo.Text = "<script>$(function(){ alert('“新密码”和“旧密码”都不能为空！'); });</script>";
                return;
            }
            else if (strOld == strNew)
            {
                ltInfo.Text = "<script>$(function(){ alert('“新密码”和“旧密码”不能一样！'); });</script>";
                return;
            }
            string strErr = HelperMain.ChkPwd(strNew, 6, 20, 2);
            if (!string.IsNullOrEmpty(strErr))
            {
                ltInfo.Text = "<script>$(function(){ alert('" + strErr + "'); });</script>";
                return;
            }
            string strOldMd5 = strOld;//HelperSecret.MD5Encrypt(strOld);
            string strNewMd5 = HelperSecret.MD5Encrypt(strNew);
            WebUser webUser = new WebUser();
            if (webUser.SetUserPwd(myUser.Id, strNewMd5, strOldMd5) > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“密码”修改成功！'); window.location.replace('" + hfBack.Value + "'); });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“旧密码”错误！'); });</script>";
            }
        }
        //
    }
}