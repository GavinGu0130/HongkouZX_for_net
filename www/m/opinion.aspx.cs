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
    public partial class opinion : System.Web.UI.Page
    {
        private DataUser myUser = null;
        private cn.opinion page = new cn.opinion();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.TrueName))
            {
                //Response.Redirect("../cn/login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            header1.UserName = myUser.TrueName;
            page.LoadNav(myUser, plNav, ltSaveNum, ltSignNum, ltFeedNum);//, ltResultNum
            string strTitle = "";
            switch (Request.QueryString["ac"])
            {
                case "query":
                    strTitle = "检索提案";
                    plQuery.Visible = true;
                    page.QueryData(myUser.TrueName, rpQueryList, ltQueryNo, lblQueryNav, ltQueryTotal, this, ddlQPeriod, ddlQTimes, ddlQSubType, ddlQCommittee, ddlQSubsector, ddlQStreetTeam, ddlQParty, txtQOpNo, txtQSubMan, ddlQTimeMark, ddlQActiveName, ddlQIsGood, ddlQIsPoint, txtQSummary, txtQBody);
                    break;
                case "my":
                    strTitle = "我的提案";
                    plMy.Visible = true;
                    page.MyList("归并,待立案,立案,不立案", myUser.TrueName, rpSubList, ltSubNo, lblSubNav, null, this);//我的<>'删除'
                    break;
                case "sign":
                    if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                    {
                        strTitle = "会签提案";
                        page.LoadSign(myUser, Convert.ToInt32(Request.QueryString["id"]), plView, ltId, ltOpNo, ltPeriod, ltTimes, ltTeamNum, plTeamNum, ltSubTime, ltApplyState, ltActiveName, ltSubType, ltIsPoint, ltIsOpen, ltSubManType, ltSubOrg, ltLinkman, ltLinkmanTel, ltLinkmanAddress, ltLinkmanZip, ltSubMan1, ltSubMans, ltSubMan, ltSummary, ltBody, ltFiles, ltAdviseHostOrg, ltAdviseHelpOrg, plViewResult, ltExamHostOrg, ltExamHelpOrg, ltResult, this, plSignBody, hfSignId, txtSignBody, plSignEdit, ltSignBody);
                    }
                    else
                    {
                        strTitle = "需会签提案";
                        plSign.Visible = true;
                        page.SignList(myUser.TrueName, rpSignList, ltSignNo, lblSignNav, this);//需反馈提案
                    }
                    break;
                case "feed":
                    if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                    {
                        strTitle = "反馈提案";
                        if (!IsPostBack)
                        {
                            page.LoadFeed(Convert.ToInt32(Request.QueryString["id"]), myUser, plFeedEdit, ltFeedOpNo, ltFeedOpOrg, ltFeedOpResult, ltFeedSummary, ltFeedInterview, ltFeedAttitude, ltFeedTakeWay, ltFeedPertinence, ltFeedLeaderReply, ltFeedResult, ltFeedBody, rblFeedInterview, rblFeedAttitude, rblFeedTakeWay, rblFeedPertinence, rblFeedLeaderReply, rblFeedResult, txtFeedBody, plFeedCmd);
                        }
                    }
                    else
                    {
                        strTitle = "需反馈提案";
                        plFeed.Visible = true;
                        page.FeedList(myUser.TrueName, rpFeedList, ltFeedNo, lblFeedNav, this);//需反馈提案
                    }
                    break;
                case "save":
                    strTitle = "暂存的提案";
                    plSave.Visible = true;
                    page.MyList("暂存,退回", myUser.TrueName, rpSaveList, ltSaveNo, lblSaveNav, null, this);//暂存
                    break;
                case "result":
                    strTitle = "跟踪办理情况";
                    plResult.Visible = true;
                    page.ResultList(myUser.TrueName, rpResultList, ltResultNo, lblResultNav, this);//跟踪办理情况
                    break;
                default:
                    if (!IsPostBack)
                    {
                        int Id = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                        strTitle = page.LoadData(Id, myUser, plSub, ddlPeriod, ddlTimes, ddlAdviseHostOrg, ddlAdviseHelpOrg, hfSubManType, txtSubMan, txtSubOrg, txtSubTime, rblIsOpen, txtOpenInfo, txtSummary, txtSubMans, txtLinkman, txtLinkmanTel, txtLinkmanAddress, txtLinkmanZip, txtBody, hfFiles, btnDel, this, hfUserId, plView, ltId, ltOpNo, ltPeriod, ltTimes, ltTeamNum, plTeamNum, ltSubTime, ltApplyState, ltActiveName, ltSubType, ltIsPoint, ltIsOpen, ltSubManType, ltSubOrg, ltLinkman, ltLinkmanTel, ltLinkmanAddress, ltLinkmanZip, ltSubMan1, ltSubMans, ltSubMan, ltSummary, ltBody, ltFiles, ltAdviseHostOrg, ltAdviseHelpOrg, plViewResult, ltExamHostOrg, ltExamHelpOrg, ltResult);
                    }
                    break;
            }
            Header.Title += " - " + strTitle;
            header1.Title = strTitle;
        }
        //提交数据
        protected void btnSub_Click(object sender, EventArgs e)
        {
            editData("提交");
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
            ltInfo.Text = page.EditData(ActiveName, myUser, this, txtSubTime, ddlPeriod, ddlTimes, rblIsOpen, txtOpenInfo, txtSubMan, txtSubMans, txtLinkman, txtLinkmanAddress, txtLinkmanZip, txtLinkmanTel, txtSummary, txtBody, hfFiles, ddlAdviseHostOrg, ddlAdviseHelpOrg);
        }
        //提交会签
        protected void btnSign_Click(object sender, EventArgs e)
        {
            ltInfo.Text = page.EditSign(myUser, this, hfSignId, txtSignBody);
        }
        //谢绝会签
        protected void btnNoSign_Click(object sender, EventArgs e)
        {
            ltInfo.Text = page.NoSign(myUser, this, hfSignId);
        }
        //提交反馈
        protected void btnFeed_Click(object sender, EventArgs e)
        {
            ltInfo.Text = page.EditFeed(myUser, this, rblFeedInterview, rblFeedAttitude, rblFeedTakeWay, rblFeedPertinence, rblFeedResult, txtFeedBody);
        }
        //
    }
}