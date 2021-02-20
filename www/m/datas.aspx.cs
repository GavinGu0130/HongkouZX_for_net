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
    public partial class datas : System.Web.UI.Page
    {
        private DataUser myUser = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.TrueName))
            {
                Response.Redirect("../cn/login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            header1.UserName = myUser.TrueName;
            cn.datas page = new cn.datas();
            int tId = page.ListType(rpType);
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                page.LoadDatas(Convert.ToInt32(Request.QueryString["id"]), rpView, myUser);
            }
            else
            {
                plList.Visible = true;
                if (!string.IsNullOrEmpty(Request.QueryString["tid"]))
                {
                    tId = Convert.ToInt32(Request.QueryString["tid"]);
                }
                DataDatas qData = new DataDatas();
                qData.TypeId = tId;
                page.ListDatas(qData, rpList, ltNo, lblNav, null, this);
            }
        }
        //
    }
}