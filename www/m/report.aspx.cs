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
    public partial class report : System.Web.UI.Page
    {
        private DataUser myUser = null;
        private cn.report page = new cn.report();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.TrueName))
            {
                //Response.Redirect("../cn/login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            header1.UserName = myUser.TrueName;
            page.LoadNav(myUser.TrueName, plNav, ltSaveNum);
            string strTitle = "调研报告";
            switch (Request.QueryString["ac"])
            {
                case "query":
                    strTitle = "检索调研报告";
                    plQuery.Visible = true;
                    page.QueryData(myUser, rpQueryList, ltQueryNo, lblQueryNav, ltQueryTotal, this, hfOrg, ddlQOrgType, txtQOrgName, ddlQIsPoint, txtQSubMan, txtQSubMans, txtQTitle, txtQSubMan, txtQSubTime1, txtQSubTime2);
                    break;
                case "save":
                    strTitle = "暂存的调研报告";
                    plSave.Visible = true;
                    page.MyList("暂存,退回", myUser.TrueName, rpSaveList, ltSaveNo, lblSaveNav, null, this);//暂存
                    break;
                case "my":
                    strTitle = "我的调研报告";
                    plMy.Visible = true;
                    page.MyList("<>'删除'", myUser.TrueName, rpMyList, ltMyNo, lblMyNav, null, this);
                    break;
                default:
                    if (!IsPostBack)
                    {
                        int Id = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                        strTitle = page.LoadData(Id, myUser, this, plSub, hfOrg, ddlOrgType, txtOrgName, rblIsPoint, txtSubMan, txtSubMan, txtTitle, txtBody, hfFiles, txtSubMan, btnDel, plView, ltOrgName, ltIsPoint, ltSubMan, ltSubMans, ltTitle, ltBody, ltFiles, ltSubMan, ltSubTime);
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
            ltInfo.Text = page.EditData(ActiveName, this, myUser, txtOrgName, rblIsPoint, txtSubMan, txtSubMan, txtTitle, txtBody, hfFiles, txtSubMan);
        }
        //
    }
}