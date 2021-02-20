using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mod.main;

namespace hkzx.web.cn
{
    public partial class ucHeader : System.Web.UI.UserControl
    {
        public string Cur = "";
        public string UserName = "";
        public string LastTime = "";
        public string UserType = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Cur != "login")
            {
                string client = HelperMain.GetClient("no");
                if (client == "wx" || client == "mobile" || client == "wap")
                {
                    Response.Redirect("../m/");
                }
            }
            DateTime lastTime = (!string.IsNullOrEmpty(LastTime)) ? Convert.ToDateTime(LastTime) : DateTime.Now;
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(LastTime))
            {
                ltHello.Text = HelperMain.SayHello();
                ltUser.Text = UserName;
                lastTime = Convert.ToDateTime(LastTime);
                plUser.Visible = true;
                plMenu.Visible = true;
                if (UserType != "" && UserType != "委员")
                {
                    ltPerform.Visible = true;
                }
                hkzx.db.DataUser myUser = hkzx.user.HelperUser.GetUser();
                if (myUser != null)
                {
                    int intNum = new cn.survey().GetFeedNum(myUser);
                    if (intNum > 0)
                    {
                        if (intNum < 100)
                        {
                            ltSurvey.Text = ltSurvey.Text.Replace("99+", intNum.ToString());
                        }
                        ltSurvey.Visible = true;
                    }
                }
            }
            ltLoginTime.Text = string.Format("{0:yyyy年MM月dd日} {1} {0:HH:mm}", lastTime, HelperMain.GetWeek(Convert.ToInt16(lastTime.DayOfWeek)));
        }
        //
    }
}