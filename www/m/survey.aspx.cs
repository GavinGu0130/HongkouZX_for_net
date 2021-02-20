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
    public partial class survey : System.Web.UI.Page
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
            cn.survey page = new cn.survey();
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                if (!IsPostBack)
                {
                    strTitle = page.LoadData(myUser, plEdit, Convert.ToInt32(Request.QueryString["id"]), ltInfo, ltTitle, hfMaxNum, hfMinNum, rpOpList, ltOpNo, plSub);
                }
            }
            else
            {
                page.ListData(plList, myUser.TrueName, rpList, ltNo, lblNav, ltTotal, this);
            }
            Header.Title += " - 问卷调查" + strTitle;
        }
        //处理提交数据
        protected void btnSub_Click(object sender, EventArgs e)
        {
            new cn.survey().SubData(myUser, ltInfo, rpOpList, this);
        }
        //
    }
}