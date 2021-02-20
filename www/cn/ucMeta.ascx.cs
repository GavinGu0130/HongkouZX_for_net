using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace hkzx.web.cn
{
    public partial class ucMeta : System.Web.UI.UserControl
    {
        public string Client = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            string strHost = Request.Url.Host;
            if (!(strHost == config.HOSTIP || (strHost == config.HOSTNAME && Request.Url.ToString().IndexOf("//" + config.HOSTNAME + "/admin/") < 0) || (Request.IsLocal && strHost == "localhost") || strHost.IndexOf("192.168.") == 0 ||strHost == "hkzx2.quyou.net"))
            {
                string strUrl = "http://" + config.HOSTIP + "/";
                if (Client == "admin")
                {
                    strUrl += "admin/";
                }
                Response.Redirect(strUrl);
                return;
            }
            string strUri = Request.Url.ToString();
            if (strUri.StartsWith("http://" + config.HOSTIP + ""))
            {//跳转到https协议
                Response.Redirect(strUri.Replace("http://", "https://"));
                return;
            }
            //if (strUri.StartsWith("https://" + config.HOSTIP + ""))
            //{//漏扫//跳转到http协议
            //    Response.Redirect(strUri.Replace("https://", "http://"));
            //    return;
            //}
            switch (Client)
            {
                case "admin":
                    plAdmin.Visible = true;
                    break;
                case "wx":
                case "mobile":
                case "wap":
                    plM.Visible = true;
                    break;
                default:
                    plPc.Visible = true;
                    break;
            }
        }
        //
    }
}