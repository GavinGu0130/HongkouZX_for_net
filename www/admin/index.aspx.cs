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
    public partial class index : System.Web.UI.Page
    {
        private DataAdmin myUser = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperAdmin.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
            {
                Response.Redirect("login.aspx?url=./");
                return;
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.Powers = myUser.Powers;

            plHome.Visible = true;
            //new news().MyList(rpNewsList, ltNewsNo);//政协动态
            new notice().MyList(rpNoticeList, ltNoticeNo);//信息发布
            new perform().MyList("发布,提交申请", rpPerformListNew, ltPerformNoNew);//会议/活动通知
            new opinion().MyList(rpOpinionList, ltOpinionNo);//待审核的提案
            new opinion_pop().MyList(rpPopList, ltPopNo);//待审核的社情民意
            new report().MyList(rpReportList, ltReportNo);//待审核的调研报告
        }
        //
    }
}