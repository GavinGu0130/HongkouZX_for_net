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
    public partial class user : System.Web.UI.Page
    {
        private DataUser myUser = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.TrueName))
            {
                Response.Redirect("../cn/login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            header1.UserName = myUser.TrueName;
            string strTitle = "委员信息";
            if (!IsPostBack)
            {
                switch (Request.QueryString["ac"])
                {
                    case "qrcode":
                        strTitle = "签到二维码";
                        plQrcode.Visible = true;
                        lblUser.Text = myUser.TrueName;
                        break;
                    case "token":
                        Response.Write(getToken());
                        Response.End();
                        break;
                    default:
                        strTitle = "委员信息";
                        plUser.Visible = true;
                        cn.user page = new cn.user();
                        page.LoadData(myUser.Id, txtUserType, txtUserCode, txtUserScore, lnkUserScore, txtTrueName, rblUserSex, txtNative, ddlNation, ddlEducation, txtIdCard, hfPhoto, txtBirthday, txtPostDate, ddlParty, txtHkMacaoTw, txtRole, txtCommittee, txtSubsector, txtStreetTeam, txtOrgName, txtOrgPost, ddlOrgType, txtOrgAddress, txtOrgZip, txtOrgTel, txtSocietyDuty, txtHomeAddress, txtHomeZip, txtHomeTel, txtContactAddress, txtMobile, txtWeChat, txtCheckText);
                        if (myUser.UserType != "委员")
                        {
                            plCmd.Visible = false;
                        }
                        break;
                }
            }
            header1.Title = strTitle;
            Header.Title = strTitle;
        }
        //加密签到字符串
        private string getToken()
        {
            string strQrcode = string.Format("{0}={1}={2}", myUser.Id, myUser.TrueName, DateTime.Now.AddMinutes(5));
            return HelperSecret.DESEncrypt(strQrcode, config.SIGNDEKEY, config.SIGNDESIV);
        }
        //编辑数据
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            cn.user page = new cn.user();
            page.EditUser(ltInfo, myUser.Id, txtTrueName, rblUserSex, txtNative, ddlNation, ddlEducation, txtIdCard, txtBirthday, ddlParty, txtHkMacaoTw, txtOrgName, txtOrgPost, ddlOrgType, txtOrgAddress, txtOrgZip, txtOrgTel, txtSocietyDuty, txtHomeAddress, txtHomeZip, txtHomeTel, txtContactAddress, txtMobile, txtWeChat, txtCheckText);
        }
        //
    }
}