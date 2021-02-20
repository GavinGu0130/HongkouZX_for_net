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
    public partial class perform : System.Web.UI.Page
    {
        private DataUser myUser = null;
        private cn.perform page = new cn.perform();
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
            if (myUser.UserType != "" && myUser.UserType != "委员")
            {
                page.LoadNav(myUser.TrueName, plNav, ltSaveNum);
            }
            string strTitle = "";
            switch (Request.QueryString["ac"])
            {
                case "sub":
                    strTitle = "申请会议/活动";
                    plSub.Visible = true;
                    int Id = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                    if (!IsPostBack)
                    {
                        page.SubData(Id, myUser, txtActiveName, hfOrg, ddlOrgType, hfSubType, ddlSubType, rblHaveBus, cblHaveDinner, txtOrgName, txtSubType, txtIsMust, txtLinkman, txtLinkmanTel, txtTitle, txtPerformSite, txtStartTime, txtEndTime, txtOverTime, txtSignTime, txtLeaders, txtAttendees, txtBody, hfFiles, plVerify, ltVerifyInfo, btnDel);
                    }
                    break;
                case "my":
                    strTitle = "我的会议/活动申请";
                    plMy.Visible = true;
                    page.MyList("<>'删除'", myUser, rpMyList, ltMyNo, lblMyNav, null, this);//我的
                    break;
                case "save":
                    strTitle = "暂存的会议/活动申请";
                    plSave.Visible = true;
                    page.MyList("暂存,退回", myUser, rpSaveList, ltSaveNo, lblSaveNav, null, this);//暂存
                    break;
                default:
                    strTitle = "会议/活动通知";
                    if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                    {
                        plView.Visible = true;
                        strTitle += " 详情";
                        page.LoadData(Convert.ToInt32(Request.QueryString["id"]), myUser, this, ltOrgName, ltLinkman, ltLinkmanTel, ltIsMust, ltSubType, ltTitle, ltStartTime, ltEndTime, ltOverTime, ltSignTime, ltPerformSite, ltHaveBus, ltHaveDinner, ltLeaders, ltAttendees, ltBody, ltFiles, ltActive, plFeed, ltFeed, btnAttend, btnNonAttend, btnLeave, plPlayFeed, rpFeedList, ltFeedNo, ltFeedTotal);
                    }
                    else
                    {
                        if (myUser.UserType != "" && myUser.UserType != "委员")
                        {
                            Response.Redirect("perform.aspx?ac=my");
                        }
                        plList.Visible = true;
                        page.MyPerform(myUser, rpQueryList, ltQueryNo, lblQueryNav, ltQueryTotal, this);
                    }
                    break;
            }
            Header.Title += " - " + strTitle;
            header1.Title = strTitle;
        }
        //
        //参加
        protected void btnAttend_Click(object sender, EventArgs e)
        {
            editFeed("参加");
        }
        protected void btnNonAttend_Click(object sender, EventArgs e)
        {
            editFeed("不参加");
        }
        //请假
        protected void btnLeave_Click(object sender, EventArgs e)
        {
            editFeed("请假申请");
        }
        //编辑反馈
        private void editFeed(string ActiveName)
        {
            ltInfo.Text = new cn.perform().EditFeed(ActiveName, myUser, this, txtReply);
        }
        //
        //提交数据
        protected void btnSub_Click(object sender, EventArgs e)
        {
            editData("提交申请");
        }
        //暂存数据
        protected void btnSave_Click(object sender, EventArgs e)
        {
            editData("暂存");
        }
        //删除数据
        protected void btnDel_Click(object sender, EventArgs e)
        {
            editData("删除");
        }
        //编辑数据
        private void editData(string ActiveName)
        {
            ltInfo.Text = page.EditData(ActiveName, myUser, this, txtOrgName, txtLinkman, txtLinkmanTel, txtSubType, txtIsMust, txtTitle, txtStartTime, txtEndTime, txtOverTime, txtSignTime, txtPerformSite, txtBody, hfFiles, txtLeaders, txtAttendees, rblHaveBus, cblHaveDinner);
        }
        //
    }
}