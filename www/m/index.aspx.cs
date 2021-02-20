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
    public partial class index : System.Web.UI.Page
    {
        private DataUser myUser = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.TrueName))
            {
                Response.Redirect("../cn/login.aspx?url=./");//../cn/login.aspx?url=../m/
                return;
            }
            if (Request.QueryString["ac"] == "mark")
            {
                plMark.Visible = true;
                //loadNews();
                loadNotice();
                loadPerform();
                loadOpinion();
                loadDatas();
                loadForum();
                loadSurvey();
            }
            else
            {
                header1.UserName = myUser.TrueName;
                loadUser();
                plHome.Visible = true;
            }
        }
        //加载用户信息
        private void loadUser()
        {
            ltUseName.Text = myUser.TrueName;
            if (!string.IsNullOrEmpty(myUser.Photo))
            {
                ltUserPhoto.Text = string.Format("<img src='{0}' />", myUser.Photo);
            }
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
            string strScore = deScore.ToString("n2");
            if (strScore.IndexOf(".00") > 0)
            {
                strScore = strScore.Substring(0, strScore.IndexOf(".00"));
            }
            lnkUserScore.Text = strScore;
            lnkUserScore.NavigateUrl += myUser.Id.ToString();
        }
        //查询政协要闻、视频新闻
        //private void loadNews()
        //{
        //    cn.news page = new cn.news();
        //    int intNum = page.GetFeedNum("政协要闻", myUser);
        //    if (intNum > 0)
        //    {
        //        lblNews.Text = (intNum > 99) ? "99+" : intNum.ToString();
        //        lblNews.Visible = true;
        //    }
        //    intNum = page.GetFeedNum("视频新闻", myUser);
        //    if (intNum > 0)
        //    {
        //        lblVideo.Text = (intNum > 99) ? "99+" : intNum.ToString();
        //        lblVideo.Visible = true;
        //    }
        //}
        //查询信息发布
        private void loadNotice()
        {
            int intNum = new cn.notice().GetFeedNum(myUser);
            if (intNum > 0)
            {
                lblNotice.Text = (intNum > 99) ? "99+" : intNum.ToString();
                lblNotice.Visible = true;
            }
        }
        //查询 会议/活动通知
        private void loadPerform()
        {
            cn.perform page = new cn.perform();
            //page.LoadPerformFeed(myUser, ltUpPerformFeed);
            page.LoadPerformFeed(myUser, ltUpPerformFeed, lblPerform);
        }
        //待会签提案
        private void loadOpinion()
        {
            cn.opinion page = new cn.opinion();
            int intNum = page.GetSignFeed(myUser.Id);
            if (intNum > 0)
            {
                lblOpinion.Text = (intNum > 99) ? "99+" : intNum.ToString();
                lblOpinion.Visible = true;
            }
        }
        //查询资料文档
        private void loadDatas()
        {
            int intNum = new cn.datas().GetFeedNum(myUser);
            if (intNum > 0)
            {
                lblDatas.Text = (intNum > 99) ? "99+" : intNum.ToString();
                lblDatas.Visible = true;
            }
        }
        //查询论坛
        private void loadForum()
        {
            int intNum = new cn.forum().GetFeedNum(myUser);
            if (intNum > 0)
            {
                lblForum.Text = (intNum > 99) ? "99+" : intNum.ToString();
                lblForum.Visible = true;
            }
        }
        //查询问卷调查
        private void loadSurvey()
        {
            int intNum = new cn.survey().GetFeedNum(myUser);
            if (intNum > 0)
            {
                lblSurvey.Text = (intNum > 99) ? "99+" : intNum.ToString();
                lblSurvey.Visible = true;
            }
        }
        //
    }
}