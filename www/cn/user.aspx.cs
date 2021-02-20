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
    public partial class user : System.Web.UI.Page
    {
        DataUser myUser = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null || myUser.Id <= 0)
            {
                Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.UserType = myUser.UserType;
            string strTitle = "委员信息";
            if (!IsPostBack)
            {
                plUser.Visible = true;
                LoadData(myUser.Id, txtUserType, txtUserCode, txtUserScore, lnkUserScore, txtTrueName, rblUserSex, txtNative, ddlNation, ddlEducation, txtIdCard, hfPhoto, txtBirthday, txtPostDate, ddlParty, txtHkMacaoTw, txtRole, txtCommittee, txtSubsector, txtStreetTeam, txtOrgName, txtOrgPost, ddlOrgType, txtOrgAddress, txtOrgZip, txtOrgTel, txtSocietyDuty, txtHomeAddress, txtHomeZip, txtHomeTel, txtContactAddress, txtMobile, txtWeChat, txtCheckText);
                if (myUser.UserType != "委员")
                {
                    plCmd.Visible = false;
                }
            }
            Header.Title += " - " + strTitle;
        }
        //加载委员信息
        public void LoadData(int Id, TextBox _txtUserType, TextBox _txtUserCode, TextBox _txtUserScore, HyperLink _lnkUserScore, TextBox _txtTrueName, RadioButtonList _rblUserSex, TextBox _txtNative, DropDownList _ddlNation, DropDownList _ddlEducation, TextBox _txtIdCard, HiddenField _hfPhoto, TextBox _txtBirthday, TextBox _txtPostDate, DropDownList _ddlParty, TextBox _txtHkMacaoTw, TextBox _txtRole, TextBox _txtCommittee, TextBox _txtSubsector, TextBox _txtStreetTeam, TextBox _txtOrgName, TextBox _txtOrgPost, DropDownList _ddlOrgType, TextBox _txtOrgAddress, TextBox _txtOrgZip, TextBox _txtOrgTel, TextBox _txtSocietyDuty, TextBox _txtHomeAddress, TextBox _txtHomeZip, TextBox _txtHomeTel, TextBox _txtContactAddress, TextBox _txtMobile, TextBox _txtWeChat, TextBox _txtCheckText)
        {
            if (Id <= 0)
            {
                return;
            }
            PublicMod.LoadNation(_ddlNation);
            WebOp webOp = new WebOp();
            PublicMod.LoadDropDownList(_ddlEducation, webOp, "文化程度");
            PublicMod.LoadDropDownList(_ddlParty, webOp, "政治面貌");
            PublicMod.LoadDropDownList(_ddlOrgType, webOp, "单位性质");

            WebUser webUser = new WebUser();
            DataUser[] data = webUser.GetData(Id);
            if (data != null)
            {
                _txtUserType.Text = data[0].UserType;
                _txtUserCode.Text = data[0].UserCode;
                WebUserScore webScore = new WebUserScore();
                int intYear = DateTime.Today.Year;
                string strStart = intYear.ToString() + "-1-1";
                string strEnd = intYear.ToString() + "-12-31";
                decimal deScoreTotal = webScore.GetTotalScore(Id, strStart, strEnd);//累计积分
                _txtUserScore.Text = deScoreTotal.ToString("n2");
                _lnkUserScore.NavigateUrl += Id.ToString();
                _txtTrueName.Text = data[0].TrueName;
                HelperMain.SetRadioSelected(_rblUserSex, data[0].UserSex);
                _txtNative.Text = data[0].Native;
                HelperMain.SetDownSelected(_ddlNation, data[0].Nation);
                HelperMain.SetDownSelected(_ddlEducation, data[0].Education);
                if (!string.IsNullOrEmpty(data[0].IdCard))
                {
                    _txtIdCard.Text = HelperSecret.DESDecrypt(data[0].IdCard, config.IDDESKEY, config.IDDESIV);//des解密
                }
                _hfPhoto.Value = data[0].Photo;
                if (data[0].Birthday > DateTime.MinValue)
                {
                    _txtBirthday.Text = data[0].Birthday.ToString("yyyy-MM-dd");
                }
                if (data[0].PostDate > DateTime.MinValue)
                {
                    _txtPostDate.Text = data[0].PostDate.ToString("yyyy-MM-dd");
                }
                HelperMain.SetDownSelected(_ddlParty, data[0].Party);
                _txtHkMacaoTw.Text = data[0].HkMacaoTw;
                if (!string.IsNullOrEmpty(data[0].Role))
                {
                    string strRole = data[0].Role.Trim(',');
                    _txtRole.Text = strRole.Replace(",", "、");
                }
                string strCommittee = data[0].Committee;
                if (!string.IsNullOrEmpty(data[0].Committee2))
                {
                    if (!string.IsNullOrEmpty(strCommittee))
                    {
                        strCommittee += "、";
                    }
                    strCommittee += data[0].Committee2;
                }
                _txtCommittee.Text = strCommittee;
                _txtSubsector.Text = data[0].Subsector;
                _txtStreetTeam.Text = data[0].StreetTeam;
                _txtOrgName.Text = data[0].OrgName;
                _txtOrgPost.Text = data[0].OrgPost;
                HelperMain.SetDownSelected(_ddlOrgType, data[0].OrgType);
                _txtOrgAddress.Text = data[0].OrgAddress;
                _txtOrgZip.Text = data[0].OrgZip;
                _txtOrgTel.Text = data[0].OrgTel;
                _txtSocietyDuty.Text = data[0].SocietyDuty;
                _txtHomeAddress.Text = data[0].HomeAddress;
                _txtHomeZip.Text = data[0].HomeZip;
                _txtHomeTel.Text = data[0].HomeTel;
                _txtContactAddress.Text = data[0].ContactAddress;
                _txtMobile.Text = data[0].Mobile;
                _txtWeChat.Text = data[0].WeChat;
                _txtCheckText.Text = data[0].CheckText;
            }
        }
        //
        //编辑数据
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            EditUser(ltInfo, myUser.Id, txtTrueName, rblUserSex, txtNative, ddlNation, ddlEducation, txtIdCard, txtBirthday, ddlParty, txtHkMacaoTw, txtOrgName, txtOrgPost, ddlOrgType, txtOrgAddress, txtOrgZip, txtOrgTel, txtSocietyDuty, txtHomeAddress, txtHomeZip, txtHomeTel, txtContactAddress, txtMobile, txtWeChat, txtCheckText);
        }
        //处理数据
        public void EditUser(Literal _ltInfo, int Id, TextBox _txtTrueName, RadioButtonList _rblUserSex, TextBox _txtNative, DropDownList _ddlNation, DropDownList _ddlEducation, TextBox _txtIdCard, TextBox _txtBirthday, DropDownList _ddlParty, TextBox _txtHkMacaoTw, TextBox _txtOrgName, TextBox _txtOrgPost, DropDownList _ddlOrgType, TextBox _txtOrgAddress, TextBox _txtOrgZip, TextBox _txtOrgTel, TextBox _txtSocietyDuty, TextBox _txtHomeAddress, TextBox _txtHomeZip, TextBox _txtHomeTel, TextBox _txtContactAddress, TextBox _txtMobile, TextBox _txtWeChat, TextBox _txtCheckText)
        {
            WebUser webUser = new WebUser();
            DataUser[] ckData = webUser.GetData(Id);
            if (ckData != null && ckData[0].TrueName == _txtTrueName.Text)
            {
                DataUser data = new DataUser();
                data.UserSex = HelperMain.SqlFilter(_rblUserSex.SelectedValue, 2);
                data.Native = HelperMain.SqlFilter(_txtNative.Text.Trim(), 20);
                data.Nation = HelperMain.SqlFilter(_ddlNation.SelectedValue.Trim(), 8);
                if (data.Nation.IndexOf("族") > 0)
                {
                    data.Nation = data.Nation.Replace("族", "");
                }
                data.Education = HelperMain.SqlFilter(_ddlEducation.SelectedValue.Trim(), 20);
                string strIdCard = HelperMain.SqlFilter(_txtIdCard.Text.Replace(" ", ""), 18);
                if (!string.IsNullOrEmpty(strIdCard))
                {
                    data.IdCard = HelperSecret.DESEncrypt(strIdCard, config.IDDESKEY, config.IDDESIV);//des加密
                }
                else
                {
                    data.IdCard = "";
                }
                if (!string.IsNullOrEmpty(_txtBirthday.Text.Trim()))
                {
                    data.Birthday = Convert.ToDateTime(_txtBirthday.Text.Trim());
                }
                data.Party = HelperMain.SqlFilter(_ddlParty.SelectedValue.Trim(), 50);
                data.HkMacaoTw = HelperMain.SqlFilter(_txtHkMacaoTw.Text.Trim(), 20);
                data.OrgName = HelperMain.SqlFilter(_txtOrgName.Text.Trim(), 100);
                data.OrgPost = HelperMain.SqlFilter(_txtOrgPost.Text.Trim(), 20);
                data.OrgType = HelperMain.SqlFilter(_ddlOrgType.SelectedValue.Trim(), 8);
                data.OrgAddress = HelperMain.SqlFilter(_txtOrgAddress.Text.Trim(), 100);
                data.OrgZip = HelperMain.SqlFilter(_txtOrgZip.Text.Trim(), 6);
                data.OrgTel = HelperMain.SqlFilter(_txtOrgTel.Text.Trim(), 50);
                data.SocietyDuty = HelperMain.SqlFilter(_txtSocietyDuty.Text.Trim(), 50);
                data.HomeAddress = HelperMain.SqlFilter(_txtHomeAddress.Text.Trim(), 100);
                data.HomeZip = HelperMain.SqlFilter(_txtHomeZip.Text.Trim(), 6);
                data.HomeTel = HelperMain.SqlFilter(_txtHomeTel.Text.Trim(), 50);
                data.ContactAddress = HelperMain.SqlFilter(_txtContactAddress.Text.Trim(), 100);
                data.Mobile = HelperMain.SqlFilter(_txtMobile.Text.Replace(" ", ""), 50);
                data.Email = "";//HelperMain.SqlFilter(_txtEmail.Text.Trim(), 200);
                data.WeChat = HelperMain.SqlFilter(_txtWeChat.Text.Trim(), 50);

                string strCheckText = HelperMain.SqlFilter(_txtCheckText.Text.Trim());
                if (ckData[0].UserSex != data.UserSex)
                {
                    strCheckText += "\nUserSex=" + data.UserSex;
                }
                if (ckData[0].Native != data.Native)
                {
                    strCheckText += "\nNative=" + data.Native;
                }
                if (ckData[0].Nation != data.Nation)
                {
                    strCheckText += "\nNation=" + data.Nation;
                }
                if (ckData[0].Education != data.Education)
                {
                    strCheckText += "\nEducation=" + data.Education;
                }
                if (ckData[0].IdCard != data.IdCard)
                {
                    strCheckText += "\nIdCard=" + strIdCard;
                }
                if (ckData[0].Birthday != data.Birthday)
                {
                    if (data.Birthday > DateTime.MinValue)
                    {
                        strCheckText += "\nBirthday=" + data.Birthday.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        strCheckText += "\nBirthday=";
                    }
                }
                if (ckData[0].Party != data.Party)
                {
                    strCheckText += "\nParty=" + data.Party;
                }
                if (ckData[0].HkMacaoTw != data.HkMacaoTw)
                {
                    strCheckText += "\nHkMacaoTw=" + data.HkMacaoTw;
                }
                if (ckData[0].OrgName != data.OrgName)
                {
                    strCheckText += "\nOrgName=" + data.OrgName;
                }
                if (ckData[0].OrgPost != data.OrgPost)
                {
                    strCheckText += "\nOrgPost=" + data.OrgPost;
                }
                if (ckData[0].OrgType != data.OrgType)
                {
                    strCheckText += "\nOrgType=" + data.OrgType;
                }
                if (ckData[0].OrgAddress != data.OrgAddress)
                {
                    strCheckText += "\nOrgAddress=" + data.OrgAddress;
                }
                if (ckData[0].OrgZip != data.OrgZip)
                {
                    strCheckText += "\nOrgZip=" + data.OrgZip;
                }
                if (ckData[0].OrgTel != data.OrgTel)
                {
                    strCheckText += "\nOrgTel=" + data.OrgTel;
                }
                if (ckData[0].SocietyDuty != data.SocietyDuty)
                {
                    strCheckText += "\nSocietyDuty=" + data.SocietyDuty;
                }
                if (ckData[0].HomeAddress != data.HomeAddress)
                {
                    strCheckText += "\nHomeAddress=" + data.HomeAddress;
                }
                if (ckData[0].HomeZip != data.HomeZip)
                {
                    strCheckText += "\nHomeZip=" + data.HomeZip;
                }
                if (ckData[0].HomeTel != data.HomeTel)
                {
                    strCheckText += "\nHomeTel=" + data.HomeTel;
                }
                if (ckData[0].ContactAddress != data.ContactAddress)
                {
                    strCheckText += "\nContactAddress=" + data.ContactAddress;
                }
                if (ckData[0].Mobile != data.Mobile)
                {
                    strCheckText += "\nMobile=" + data.Mobile;
                }
                //if (ckData[0].Email != data.Email)
                //{
                //    strCheckText += "\nEmail=" + data.Email;
                //}
                if (ckData[0].WeChat != data.WeChat)
                {
                    strCheckText += "\nWeChat=" + data.WeChat;
                }
                
                if (!string.IsNullOrEmpty(strCheckText))
                {
                    strCheckText = strCheckText.Trim('\n');
                    if (webUser.UpdateCheck(Id, strCheckText) > 0)
                    {
                        string strBack = "user.aspx";
                        _ltInfo.Text = "<script>$(function(){ alert('“修改信息”成功！'); window.location.replace('" + strBack + "'); });</script>";
                    }
                    else
                    {
                        _ltInfo.Text = "<script>$(function(){ alert('“修改信息”失败！'); window.history.back(-1); });</script>";
                    }
                }
                else
                {
                    _ltInfo.Text = "<script>$(function(){ alert('信息未修改！'); window.history.back(-1); });</script>";
                }
            }
        }
        //
    }
}