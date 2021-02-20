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
    public partial class opinion_pop : System.Web.UI.Page
    {
        private DataUser myUser = null;
        private cn.opinion_pop page = new cn.opinion_pop();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.TrueName))
            {
                //Response.Redirect("../cn/login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            header1.UserName = myUser.TrueName;
            page.LoadNav(myUser.TrueName, plNav, ltSaveNum);//, ltMyNum
            string strTitle = "";
            switch (Request.QueryString["ac"])
            {
                case "query":
                    strTitle = "检索社情民意";
                    plQuery.Visible = true;
                    page.QueryData(myUser.TrueName, rpQueryList, ltQueryNo, lblQueryNav, ltQueryTotal, this, ddlQActive, ddlQSubType, txtQSubMan, ddlQCommittee, ddlQSubsector, ddlQStreetTeam, ddlQParty, txtQSubTime1, txtQSubTime2, txtQSummary, txtQBody, txtQAdvise);
                    break;
                case "my":
                    strTitle = "我的社情民意";
                    plMy.Visible = true;
                    page.MyList("<>'删除'", myUser.TrueName, rpMyList, ltMyNo, lblMyNav, null, this);//我的
                    break;
                case "save":
                    strTitle = "暂存的社情民意";
                    plSave.Visible = true;
                    page.MyList("暂存,退回", myUser.TrueName, rpSaveList, ltSaveNo, lblSaveNav, null, this);//暂存
                    break;
                default:
                    if (!IsPostBack)
                    {
                        int Id = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                        strTitle = page.LoadData(Id, myUser, plSub, hfSubManType, rblSubType, txtSubMan, txtSubOrg, txtLinkman, cblLinkmanInfo, cblLinkmanParty, txtLinkmanOrgName, txtLinkmanTel, this, rblIsOpen, txtOpenInfo, txtSubMans, txtSubMan2, txtSummary, txtBody, txtAdvise, hfFiles, btnDel, plView, ltActive, ltSubTime, ltSubType, ltIsOpen, ltSubMan, ltSubOrg, ltLinkman, ltLinkmanInfo, ltLinkmanParty, ltLinkmanOrgName, ltLinkmanTel, ltSubMans, ltSummary, ltBody, ltAdvise, ltFiles, ltEmploy, plShowInfo);
                    }
                    break;
            }
            Header.Title += " - " + strTitle;
        }
        //提交数据
        protected void btnSub_Click(object sender, EventArgs e)
        {
            editData("反映");
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
            ltInfo.Text = page.EditData(ActiveName, myUser, this, rblSubType, rblIsOpen, txtOpenInfo, txtSubMan, txtLinkman, cblLinkmanInfo, cblLinkmanParty, txtLinkmanOrgName, txtLinkmanTel, txtSubMans, txtSubMan2, txtSummary, txtBody, txtAdvise, hfFiles);
        }
        //
    }
}