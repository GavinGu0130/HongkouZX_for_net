using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mod.main;

namespace hkzx.web.admin
{
    public partial class ucHeader : System.Web.UI.UserControl
    {
        public string Cur = "";
        public string UserName = "";
        public string LastTime = "";
        public string Powers = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime loginTime = DateTime.Now;
            if (!string.IsNullOrEmpty(LastTime))
            {
                ltHello.Text = HelperMain.SayHello();
                ltUser.Text = UserName;
                loginTime = Convert.ToDateTime(LastTime);
                plUser.Visible = true;
                plMenu.Visible = true;
            }
            ltLoginTime.Text = string.Format("{0:yyyy年MM月dd日} {1} {0:HH:mm}", loginTime, HelperMain.GetWeek(Convert.ToInt16(loginTime.DayOfWeek)));
            if (Powers.IndexOf("alls") >= 0 || Powers.IndexOf("system") >= 0)
            {
                plAdmin.Visible = true;
            }
            if (Powers.IndexOf("alls") >= 0 || Powers.IndexOf("count") >= 0)
            {
                plCount.Visible = true;
            }
            if (Powers.IndexOf("alls") >= 0 || Powers.IndexOf("user") >= 0)
            {
                ltUsers.Visible = true;
            }
            //if (Powers.IndexOf("alls") >= 0 || Powers.IndexOf("news") >= 0)
            //{
            //    ltNews.Visible = true;
            //}
            //if (Powers.IndexOf("alls") >= 0 || Powers.IndexOf("view") >= 0)
            //{
            //    ltView.Visible = true;
            //}
            if (Powers.IndexOf("alls") >= 0 || Powers.IndexOf("notice") >= 0)
            {
                ltNotice.Visible = true;
            }
            if (Powers.IndexOf("alls") >= 0 || Powers.IndexOf("perform") >= 0)
            {
                ltPerform.Visible = true;
            }
            if (Powers.IndexOf("alls") >= 0 || Powers.IndexOf("opin") >= 0)
            {
                ltOpin.Visible = true;
            }
            if (Powers.IndexOf("alls") >= 0 || Powers.IndexOf("pop") >= 0)
            {
                ltPop.Visible = true;
            }
            if (Powers.IndexOf("alls") >= 0 || Powers.IndexOf("report") >= 0)
            {
                ltReport.Visible = true;
            }
            if (Powers.IndexOf("alls") >= 0 || Powers.IndexOf("survey") >= 0)
            {
                ltSurvey.Visible = true;
            }
            if (Powers.IndexOf("alls") >= 0 || Powers.IndexOf("datas") >= 0)
            {
                ltDatas.Visible = true;
            }
            if (Powers.IndexOf("alls") >= 0 || Powers.IndexOf("forum") >= 0)
            {
                ltForum.Visible = true;
            }
        }
        //
    }
}