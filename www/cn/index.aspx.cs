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
    public partial class index : System.Web.UI.Page
    {
        private DataUser myUser = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null)
            {
                Response.Redirect("login.aspx?url=./");
                return;
            }
            //if (Request.IsLocal)
            //{
            //    //ltInfo.Text = HelperSecret.DESDecrypt(HttpContext.Current.Request.Cookies["hkzx"].Value, "hkzx-807", "shhkzx-123");
            //    ltInfo.Text = HelperSecret.DESEncrypt("350=委员==11087=测试员=男=../upload/photo/2019/10086.jpg=2019-09-11 14:22:31=提案委员会=教科卫体委员会-科技小组=工会==北外滩街道活动组=群众", "hkzx-807", "shhkzx-123");
            //}
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.UserType = myUser.UserType;
            loadUser();
            plHome.Visible = true;
            new notice().ListData(myUser.TrueName, rpNotice);//信息发布
            cn.perform page = new cn.perform();
            page.MyPerform(myUser, rpPerform);//会议/活动通知
            page.LoadPerformFeed(myUser, ltUpPerformFeed);//取会议/活动更新信息
            new opinion().MyOpinion(myUser.TrueName, rpOpinion);//我的提案、需反馈的（第一、联名提交人）
            new opinion_pop().MyOpinion(myUser.TrueName, rpOpinionPop);//我的社情民意
            new report().MyReport(myUser.TrueName, rpReport);//我的调研报告
            //new survey().ListData(rpSurvey);//意见征询
        }
        //加载用户信息
        private void loadUser()
        {
            if (!string.IsNullOrEmpty(myUser.Photo))
            {
                ltUserPhoto.Text = string.Format("<img src='{0}' />", myUser.Photo);
            }
            ltTureName.Text = myUser.TrueName;
            WebUserScore webScore = new WebUserScore();
            int intYear = DateTime.Today.Year;
            decimal deScore = webScore.GetTotalScore(myUser.Id, intYear.ToString() + "-1-1", intYear.ToString() + "-12-31");
            if (deScore < 0)
            {
                lnkUserScore.ForeColor = System.Drawing.Color.Gray;
            }
            else if (!string.IsNullOrEmpty(myUser.OrderColor))
            {
                lnkUserScore.ForeColor = System.Drawing.ColorTranslator.FromHtml(myUser.OrderColor);
            }
            lnkUserScore.Text = deScore.ToString("n2");
            lnkUserScore.NavigateUrl += myUser.Id.ToString();
        }
        //
    }
}