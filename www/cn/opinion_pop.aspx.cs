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
    public partial class opinion_pop : System.Web.UI.Page
    {
        private DataUser myUser = null;
        WebOpinionPop webPop = new WebOpinionPop();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null)
            {
                Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.UserType = myUser.UserType;
            LoadNav(myUser.TrueName, plNav, ltSaveNum);
            string strTitle = "";
            switch (Request.QueryString["ac"])
            {
                case "query":
                    strTitle = "检索社情民意";
                    plQuery.Visible = true;
                    QueryData(myUser.TrueName, rpQueryList, ltQueryNo, lblQueryNav, ltQueryTotal, this, ddlQActive, ddlQSubType, txtQSubMan, ddlQCommittee, ddlQSubsector, ddlQStreetTeam, ddlQParty, txtQSubTime1, txtQSubTime2, txtQSummary, txtQBody, txtQAdvise);
                    break;
                case "my":
                    strTitle = "我的社情民意";
                    plMy.Visible = true;
                    MyList("<>'删除'", myUser.TrueName, rpMyList, ltMyNo, lblMyNav, ltMyTotal, this);//我的
                    break;
                case "save":
                    strTitle = "暂存的社情民意";
                    plSave.Visible = true;
                    MyList("暂存,退回", myUser.TrueName, rpSaveList, ltSaveNo, lblSaveNav, null, this);//暂存
                    break;
                default:
                    if (!IsPostBack)
                    {
                        int Id = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                        strTitle = LoadData(Id, myUser, plSub, hfSubManType, rblSubType, txtSubMan, txtSubOrg, txtLinkman, cblLinkmanInfo, cblLinkmanParty, txtLinkmanOrgName, txtLinkmanTel, this, rblIsOpen, txtOpenInfo, txtSubMans, txtSubMan2, txtSummary, txtBody, txtAdvise, hfFiles, btnDel, plView, ltActive, ltSubTime, ltSubType, ltIsOpen, ltSubMan, ltSubOrg, ltLinkman, ltLinkmanInfo, ltLinkmanParty, ltLinkmanOrgName, ltLinkmanTel, ltSubMans, ltSummary, ltBody, ltAdvise, ltFiles, ltEmploy, plShowInfo);
                    }
                    break;
            }
            Header.Title += " - " + strTitle;
        }
        //页面nav
        public void LoadNav(string strUser, Panel plNav2, Literal ltSaveNum2)
        {
            plNav2.Visible = true;
            DataOpinionPop qData = new DataOpinionPop();
            qData.ActiveName = "暂存,退回";
            qData.AddUser = strUser;
            DataOpinionPop[] data = webPop.GetDatas(qData, "Id");
            if (data != null)
            {
                ltSaveNum2.Text = (data.Count() > 99) ? "99+" : data.Count().ToString();
            }
        }
        //
        #region 提交
        //初始化提交表
        private void initSub(DataUser uData, PlaceHolder plSub2, HiddenField hfSubManType2, RadioButtonList rblSubType2, TextBox txtSubMan2, TextBox txtSubOrg2, TextBox txtLinkman2, CheckBoxList cblLinkmanInfo2, CheckBoxList cblLinkmanParty2, TextBox txtLinkmanOrgName2, TextBox txtLinkmanTel2)
        {
            plSub2.Visible = true;
            WebOp webOp = new WebOp();
            PublicMod.LoadRadioButtonList(rblSubType2, webOp, "社情民意类别");
            PublicMod.LoadCheckBoxList(cblLinkmanInfo2, webOp, "反映人身份");
            PublicMod.LoadCheckBoxList(cblLinkmanParty2, webOp, "政治面貌");
            hfSubManType2.Value = uData.UserType;
            if (hfSubManType2.Value == "委员")
            {
                HelperMain.SetCheckSelected(cblLinkmanInfo2, "区政协委员");
                HelperMain.SetCheckSelected(cblLinkmanParty2, uData.Party);
                WebUser webUser = new WebUser();
                DataUser[] data = webUser.GetData(uData.Id, "Mobile,OrgName");
                if (data != null)
                {
                    txtLinkmanOrgName2.Text = data[0].OrgName;
                    txtLinkmanTel2.Text = data[0].Mobile;
                }
            }
            txtSubMan2.Text = uData.TrueName;
            txtSubOrg2.Text = uData.TrueName;
        }
        //信息
        public string LoadData(int Id, DataUser uData, PlaceHolder plSub2, HiddenField hfSubManType2, RadioButtonList rblSubType2, TextBox txtSubMan2, TextBox txtSubOrg2, TextBox txtLinkman2, CheckBoxList cblLinkmanInfo2, CheckBoxList cblLinkmanParty2, TextBox txtLinkmanOrgName2, TextBox txtLinkmanTel2, Page page, RadioButtonList rblIsOpen2, TextBox txtOpenInfo2, TextBox _txtSubMans, TextBox _txtSubMan2, TextBox txtSummary2, TextBox txtBody2, TextBox txtAdvise2, HiddenField hfFiles2, Button btnDel2, PlaceHolder plView2, Literal ltActive2, Literal ltSubTime2, Literal ltSubType2, Literal ltIsOpen2, Literal ltSubMan2, Literal ltSubOrg2, Literal ltLinkman2, Literal ltLinkmanInfo2, Literal ltLinkmanParty2, Literal ltLinkmanOrgName2, Literal ltLinkmanTel2, Literal _ltSubMans, Literal ltSummary2, Literal ltBody2, Literal ltAdvise2, Literal ltFiles2, Literal ltEmploy2, PlaceHolder _plShowInfo)
        {
            string strTitle = "提交社情民意";
            if (Id <= 0)
            {
                initSub(uData, plSub2, hfSubManType2, rblSubType2, txtSubMan2, txtSubOrg2, txtLinkman2, cblLinkmanInfo2, cblLinkmanParty2, txtLinkmanOrgName2, txtLinkmanTel2);
                return strTitle;
            }
            DataOpinionPop[] data = webPop.GetData(Id);
            if (data == null)
            {
                page.Response.Redirect("opinion_pop.aspx");
                return "";
            }
            if (data[0].ActiveName == "暂存" || data[0].ActiveName == "退回")
            {
                if (data[0].AddUser == uData.TrueName)
                {
                    initSub(uData, plSub2, hfSubManType2, rblSubType2, txtSubMan2, txtSubOrg2, txtLinkman2, cblLinkmanInfo2, cblLinkmanParty2, txtLinkmanOrgName2, txtLinkmanTel2);
                    HelperMain.SetRadioSelected(rblSubType2, data[0].SubType);
                    HelperMain.SetRadioSelected(rblIsOpen2, data[0].IsOpen);
                    txtOpenInfo2.Text = data[0].OpenInfo;
                    txtSubMan2.Text = data[0].SubMan;
                    txtSubOrg2.Text = data[0].SubMan;
                    txtLinkman2.Text = data[0].Linkman;
                    HelperMain.SetCheckSelected(cblLinkmanInfo2, data[0].LinkmanInfo);
                    HelperMain.SetCheckSelected(cblLinkmanParty2, data[0].LinkmanParty);
                    if (!string.IsNullOrEmpty(data[0].LinkmanOrgName))
                    {
                        txtLinkmanOrgName2.Text = data[0].LinkmanOrgName;
                    }
                    if (!string.IsNullOrEmpty(data[0].LinkmanTel))
                    {
                        txtLinkmanTel2.Text = data[0].LinkmanTel;
                    }
                    _txtSubMans.Text = data[0].SubMans.Trim(',');
                    _txtSubMan2.Text = data[0].SubMan2.Trim(',');
                    txtSummary2.Text = data[0].Summary;
                    txtBody2.Text = data[0].Body;
                    txtAdvise2.Text = data[0].Advise;
                    hfFiles2.Value = data[0].Files;
                    btnDel2.Visible = true;
                }
            }
            else if (data[0].ActiveName == "已录用" || data[0].ActiveName == "提交" || data[0].ActiveName == "未录用" || data[0].ActiveName == "留存" || data[0].ActiveName == "待审核")// && data[0].AddUser == uData.TrueName
            {//二种情况：查看、反馈
                strTitle = "查阅社情民意";
                plView2.Visible = true;
                ltActive2.Text = data[0].ActiveName;
                ltSubTime2.Text = data[0].SubTime.ToString("yyyy-MM-dd HH:mm:ss");
                ltSubType2.Text = data[0].SubType;
                ltIsOpen2.Text = data[0].IsOpen;
                if (!string.IsNullOrEmpty(data[0].OpenInfo))
                {
                    ltIsOpen2.Text += "，" + data[0].OpenInfo;
                }
                if (data[0].SubManType == "委员")
                {
                    ltSubMan2.Text = data[0].SubMan;
                }
                else
                {
                    ltSubOrg2.Text = data[0].SubMan;
                    ltLinkman2.Text = data[0].Linkman;
                    //_plShowInfo.Visible = true;
                    ltLinkmanInfo2.Text = data[0].LinkmanInfo;
                    ltLinkmanParty2.Text = data[0].LinkmanParty;
                    ltLinkmanOrgName2.Text = data[0].LinkmanOrgName;
                    ltLinkmanTel2.Text = data[0].LinkmanTel;
                }
                if (!string.IsNullOrEmpty(data[0].SubMans))
                {
                    _ltSubMans.Text = data[0].SubMans.Trim(',');
                }
                if (!string.IsNullOrEmpty(data[0].SubMan2))
                {
                    string strSubMans = "";
                    if (!string.IsNullOrEmpty(_ltSubMans.Text))
                    {
                        strSubMans = _ltSubMans.Text + ",";
                    }
                    strSubMans += data[0].SubMan2.Trim(',');
                    _ltSubMans.Text = strSubMans;
                }
                ltSummary2.Text = data[0].Summary;
                ltBody2.Text = data[0].Body;
                ltAdvise2.Text = data[0].Advise;
                if (!string.IsNullOrEmpty(data[0].Files))
                {
                    string strFiles = "";
                    string[] arr = data[0].Files.Split('|');
                    for (int i = 0; i < arr.Count(); i++)
                    {
                        if (!string.IsNullOrEmpty(arr[i]))
                        {
                            strFiles += string.Format("<a href='{0}' target='_blank'>{0}</a><br/>", arr[i]);
                        }
                    }
                    ltFiles2.Text = strFiles;
                }
                ltEmploy2.Text = PublicMod.GetPopFeed(data[0]);
            }
            else
            {
                page.Response.Redirect("opinion_pop.aspx");
            }
            return strTitle;
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
            ltInfo.Text = EditData(ActiveName, myUser, this, rblSubType, rblIsOpen, txtOpenInfo, txtSubMan, txtLinkman, cblLinkmanInfo, cblLinkmanParty, txtLinkmanOrgName, txtLinkmanTel, txtSubMans, txtSubMan2, txtSummary, txtBody, txtAdvise, hfFiles);
        }
        public string EditData(string ActiveName, DataUser uData, Page page, RadioButtonList rblSubType2, RadioButtonList rblIsOpen2, TextBox txtOpenInfo2, TextBox txtSubMan2, TextBox txtLinkman2, CheckBoxList cblLinkmanInfo2, CheckBoxList cblLinkmanParty2, TextBox txtLinkmanOrgName2, TextBox txtLinkmanTel2, TextBox _txtSubMans, TextBox _txtSubMan2, TextBox txtSummary2, TextBox txtBody2, TextBox txtAdvise2, HiddenField hfFiles2)
        {
            if (uData == null)
            {
                return "<script>$(function(){ alert('请重新登录！'); });</script>";
            }
            string strOut = "";
            string strBack = PublicMod.GetBackUrl();
            DataOpinionPop data = new DataOpinionPop();
            data.Id = (!string.IsNullOrEmpty(page.Request.QueryString["id"])) ? Convert.ToInt32(page.Request.QueryString["id"]) : 0;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(uData.TrueName, 20);
            if (ActiveName == "删除")
            {
                if (data.Id > 0)
                {
                    data.Id = webPop.UpdateActive(data.Id, ActiveName);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                data.SubType = HelperMain.SqlFilter(rblSubType2.SelectedValue, 20);
                data.IsOpen = HelperMain.SqlFilter(rblIsOpen2.SelectedValue, 2);
                if (data.IsOpen == "否")
                {
                    data.OpenInfo = HelperMain.SqlFilter(txtOpenInfo2.Text.Trim(), 20);
                }
                data.SubManType = uData.UserType;
                data.SubMan = HelperMain.SqlFilter(txtSubMan2.Text.Trim(), 20);
                data.Linkman = HelperMain.SqlFilter(txtLinkman2.Text.Trim(), 20);
                data.LinkmanInfo = HelperMain.SqlFilter(HelperMain.GetCheckSelected(cblLinkmanInfo2));
                data.LinkmanParty = HelperMain.SqlFilter(HelperMain.GetCheckSelected(cblLinkmanParty2), 20);
                data.LinkmanOrgName = HelperMain.SqlFilter(txtLinkmanOrgName2.Text.Trim(), 100);
                data.LinkmanTel = HelperMain.SqlFilter(txtLinkmanTel2.Text.Trim(), 50);
                if (!string.IsNullOrEmpty(_txtSubMans.Text))
                {
                    _txtSubMans.Text = _txtSubMans.Text.Replace("、", ",").Replace("，", ",").Replace("\n", ",").Replace("\r", "");
                    data.SubMans = "," + HelperMain.SqlFilter(_txtSubMans.Text.Trim()) + ",";
                }
                if (!string.IsNullOrEmpty(_txtSubMan2.Text))
                {
                    _txtSubMan2.Text = _txtSubMan2.Text.Replace("、", ",").Replace("，", ",").Replace("\n", ",").Replace("\r", "");
                    data.SubMan2 = "," + HelperMain.SqlFilter(_txtSubMan2.Text.Trim()) + ",";
                }
                data.Summary = HelperMain.SqlFilter(txtSummary2.Text.Trim(), 100);
                data.Body = HelperMain.SqlFilter(txtBody2.Text.TrimEnd());
                data.Advise = HelperMain.SqlFilter(txtAdvise2.Text.TrimEnd());
                data.Files = HelperMain.SqlFilter(hfFiles2.Value.Trim('|'));
                //data.Remark
                DateTime dtNow = DateTime.Now;
                if (ActiveName == "反映")
                {
                    data.ActiveName = "待审核";
                    data.SubTime = dtNow;
                    data.SubIp = strIp;
                    string strSubMans = data.SubMan;
                    if (!string.IsNullOrEmpty(data.SubMans))
                    {
                        strSubMans += "," + data.SubMans.Trim(',');
                    }
                    string strParty = "";
                    string strCommittee = "";
                    string strSubsector = "";
                    string strStreetTeam = "";
                    string[] arr = strSubMans.Split(',');
                    WebUser webUser = new WebUser();
                    for (int i = 0; i < arr.Count(); i++)
                    {
                        if (!string.IsNullOrEmpty(arr[i]))
                        {
                            DataUser[] uData2 = webUser.GetDatas(config.PERIOD, arr[i], "Id,Party,Committee,Committee2,Subsector,StreetTeam");
                            if (uData2 != null)
                            {
                                if (!string.IsNullOrEmpty(uData2[0].Party) && (strParty + ",").IndexOf(uData2[0].Party) < 0)
                                {
                                    strParty += "," + uData2[0].Party;
                                }
                                if (!string.IsNullOrEmpty(uData2[0].Committee) && (strCommittee + ",").IndexOf(uData2[0].Committee) < 0)
                                {
                                    strCommittee += "," + uData2[0].Committee;
                                }
                                if (!string.IsNullOrEmpty(uData2[0].Committee2) && (strCommittee + ",").IndexOf(uData2[0].Committee2) < 0)
                                {
                                    strCommittee += "," + uData2[0].Committee2;
                                }
                                if (!string.IsNullOrEmpty(uData2[0].Subsector) && (strSubsector + ",").IndexOf(uData2[0].Subsector) < 0)
                                {
                                    strSubsector += "," + uData2[0].Subsector;
                                }
                                if (!string.IsNullOrEmpty(uData2[0].StreetTeam) && (strStreetTeam + ",").IndexOf(uData2[0].StreetTeam) < 0)
                                {
                                    strStreetTeam += "," + uData2[0].StreetTeam;
                                }
                            }
                        }
                    }
                    data.Party = strParty.Trim(',');
                    data.Committee = strCommittee.Trim(',');
                    data.Subsector = strSubsector.Trim(',');
                    data.StreetTeam = strStreetTeam.Trim(',');
                }
                else
                {
                    data.ActiveName = "暂存";
                }
                DataOpinionPop qData = new DataOpinionPop();
                qData.Summary = data.Summary;
                qData.SubTimeText = string.Format("{0:yyyy-MM-dd},{0:yyyy-MM-dd 23:59:59}", DateTime.Today);
                qData.AddUser = strUser;
                DataOpinionPop[] ckData = webPop.GetDatas(qData, "Id", 0, 1);//重复检查
                if (ckData != null)
                {
                    //ltInfo.Text = "<script>$(function(){ alert('“标题”重复，不能添加！'); window.history.back(-1); });</script>";
                    strOut = "<script>$(function(){ alert('重复提交！'); window.history.back(-1); });</script>";
                    //page.Response.Redirect(strBack);
                    return strOut;
                }
                if (data.Id <= 0)
                {
                    //DataOpinionPop qData = new DataOpinionPop();
                    //qData.Summary = data.Summary;
                    //qData.AddTime = DateTime.Today;
                    //qData.AddUser = strUser;
                    //DataOpinionPop[] ckData = webPop.GetDatas(qData, "Id", 0, 1);//重复检查
                    //if (ckData != null)
                    //{
                    //    //ltInfo.Text = "<script>$(function(){ alert('“标题”重复，不能添加！'); window.history.back(-1); });</script>";
                    //    strOut = "<script>$(function(){ alert('重复提交！'); window.history.back(-1); });</script>";
                    //    //page.Response.Redirect(strBack);
                    //    return strOut;
                    //}
                    data.UserId = uData.Id;
                    data.AddTime = dtNow;
                    data.AddIp = strIp;
                    data.AddUser = strUser;
                    data.Id = webPop.Insert(data);
                }
                else
                {
                    data.UserId = -1;
                    data.UpTime = dtNow;
                    data.UpIp = strIp;
                    data.UpUser = strUser;
                    if (webPop.Update(data) <= 0)
                    {
                        data.Id = -1;
                    }
                }
            }
            if (data.Id > 0)
            {
                strOut = "<script>$(function(){ alert('“" + ActiveName + "社情民意”成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                strOut = "<script>$(function(){ alert('“" + ActiveName + "社情民意”失败！'); window.history.back(-1); });</script>";
            }
            return strOut;
        }
        #endregion
        //
        #region 社情民意查询
        //首页我的社情民意
        public void MyOpinion(string TrueName, Repeater rpList)
        {
            DataOpinionPop data = new DataOpinionPop();
            data.ActiveName = "提交,已录用,未录用";
            data.SubMan = TrueName;
            listData(data, rpList);
        }
        //加载我的列表：我的、暂存
        public void MyList(string ActiveName, string strUser, Repeater rpList, Literal ltNo, Label lblNav, Literal ltTotal, Page page)
        {
            DataOpinionPop data = new DataOpinionPop();
            data.ActiveName = ActiveName;
            data.AddUser = strUser;
            listData(data, rpList, "", ltNo, lblNav, ltTotal, page);
        }
        //初始化查询
        public void QueryData(string strUser, Repeater rpList, Literal ltNo, Label lblNav, Literal ltTotal, Page page, DropDownList ddlQActive2, DropDownList ddlQSubType2, TextBox txtQSubMan2, DropDownList ddlQCommittee2, DropDownList ddlQSubsector2, DropDownList ddlQStreetTeam2, DropDownList ddlQParty2, TextBox txtQSubTime01, TextBox txtQSubTime02, TextBox txtQSummary2, TextBox txtQBody2, TextBox txtQAdvise2)
        {
            WebOp webOp = new WebOp();
            PublicMod.LoadDropDownList(ddlQSubType2, webOp, "社情民意类别");
            PublicMod.LoadDropDownList(ddlQCommittee2, webOp, "专委会");
            PublicMod.LoadDropDownList(ddlQSubsector2, webOp, "界别");
            PublicMod.LoadDropDownList(ddlQStreetTeam2, webOp, "街道活动组");
            PublicMod.LoadDropDownList(ddlQParty2, webOp, "政治面貌");
            DataOpinionPop qData = new DataOpinionPop();
            if (!string.IsNullOrEmpty(page.Request.QueryString["SubMan"]))
            {
                qData.SubMan = HelperMain.SqlFilter(page.Request.QueryString["SubMan"].Trim());
                txtQSubMan2.Text = qData.SubMan;
            }
            //else
            //{
            //    qData.SubMan = HelperMain.SqlFilter(strUser, 20);
            //}
            if (!string.IsNullOrEmpty(page.Request.QueryString["Active"]))
            {
                qData.ActiveName = HelperMain.SqlFilter(page.Request.QueryString["Active"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQActive2, qData.ActiveName);
            }
            else
            {
                qData.ActiveName = "提交,已录用,未录用,留存";
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["SubType"]))
            {
                qData.SubType = HelperMain.SqlFilter(page.Request.QueryString["SubType"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQSubType2, qData.SubType);
            }
            //if (!string.IsNullOrEmpty(page.Request.QueryString["LinkmanInfo"]))
            //{
            //    qData.LinkmanInfo = HelperMain.SqlFilter(page.Request.QueryString["LinkmanInfo"].Trim(), 100);
            //    HelperMain.SetCheckSelected(cblQLinkmanInfo2, qData.LinkmanInfo);
            //}
            //if (!string.IsNullOrEmpty(page.Request.QueryString["LinkmanParty"]))
            //{
            //    qData.LinkmanParty = HelperMain.SqlFilter(page.Request.QueryString["LinkmanParty"].Trim(), 20);
            //    HelperMain.SetDownSelected(ddlQLinkmanParty2, qData.LinkmanParty);
            //}
            if (!string.IsNullOrEmpty(page.Request.QueryString["Committee"]))
            {
                qData.Committee = HelperMain.SqlFilter(page.Request.QueryString["Committee"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQCommittee2, qData.Committee);
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["Subsector"]))
            {
                qData.Subsector = HelperMain.SqlFilter(page.Request.QueryString["Subsector"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQSubsector2, qData.Subsector);
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["StreetTeam"]))
            {
                qData.StreetTeam = HelperMain.SqlFilter(page.Request.QueryString["StreetTeam"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQStreetTeam2, qData.StreetTeam);
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["Party"]))
            {
                qData.Party = HelperMain.SqlFilter(page.Request.QueryString["Party"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQParty2, qData.Party);
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["SubTime"]) && page.Request.QueryString["SubTime"].IndexOf(",") >= 0)
            {
                string strTime = page.Request.QueryString["SubTime"].Trim();
                string strTime1 = strTime.Substring(0, strTime.IndexOf(","));
                string strTime2 = strTime.Substring(strTime.IndexOf(",") + 1);
                txtQSubTime01.Text = HelperMain.SqlFilter(strTime1.Trim(), 10);
                txtQSubTime02.Text = HelperMain.SqlFilter(strTime2.Trim(), 10);
                if (txtQSubTime01.Text != "" || txtQSubTime02.Text != "")
                {
                    qData.SubTimeText = txtQSubTime01.Text + "," + txtQSubTime02.Text;
                }
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["Summary"]))
            {
                qData.Summary = "%" + HelperMain.SqlFilter(page.Request.QueryString["Summary"].Trim(), 100) + "%";
                txtQSummary2.Text = qData.Summary.Trim('%');
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["Body"]))
            {
                qData.Body = "%" + HelperMain.SqlFilter(page.Request.QueryString["Body"].Trim()) + "%";
                txtQBody2.Text = qData.Body.Trim('%');
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["Advise"]))
            {
                qData.Advise = "%" + HelperMain.SqlFilter(page.Request.QueryString["Advise"].Trim(), 100) + "%";
                txtQAdvise2.Text = qData.Advise.Trim('%');
            }
            listData(qData, rpList, "", ltNo, lblNav, ltTotal, page);
        }
        private void listData(DataOpinionPop qData, Repeater rpList, string strOrder = "", Literal ltNo = null, Label lblNav = null, Literal ltTotal = null, Page page = null)
        {
            int pageCur = 1;
            if (lblNav != null && !string.IsNullOrEmpty(page.Request.QueryString["page"]))
            {
                pageCur = Convert.ToInt32(page.Request.QueryString["page"]);
            }
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            DataOpinionPop[] data = webPop.GetDatas(qData, "Id,SubType,Summary,SubMan,SubTime,ActiveName,UpTime", pageCur, pageSize, strOrder, "total");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    //if (!string.IsNullOrEmpty(data[i].OrgName))
                    //{
                    //    data[i].OrgType += "-" + data[i].OrgName;
                    //}
                    switch (data[i].ActiveName)
                    {
                        case "暂存":
                            data[i].rowClass = " class='save' title='暂存'";
                            data[i].StateName = "修改";
                            break;
                        case "留存"://未录用
                            data[i].rowClass = " class='cancel' title='留存'";
                            data[i].StateName = "查看";
                            if (data[i].SubTime > DateTime.MinValue)
                            {
                                data[i].SubTimeText = data[i].SubTime.ToString("yyyy-MM-dd");
                            }
                            break;
                        case "已录用":
                            data[i].StateName = "查看";
                            if (data[i].SubTime > DateTime.MinValue)
                            {
                                data[i].SubTimeText = data[i].SubTime.ToString("yyyy-MM-dd");
                            }
                            break;
                        default://提交
                            data[i].StateName = "查看";
                            if (data[i].SubTime > DateTime.MinValue)
                            {
                                data[i].SubTimeText = data[i].SubTime.ToString("yyyy-MM-dd");
                            }
                            break;
                    }
                }
                rpList.DataSource = data;
                rpList.DataBind();
                if (ltNo != null)
                {
                    ltNo.Visible = false;
                }
                if (lblNav != null)
                {
                    int pageCount = data[0].total;
                    int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                    string lnk = page.Request.Url.ToString();
                    lblNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
                }
                if (ltTotal != null)
                {
                    ltTotal.Text = data[0].total.ToString();
                }
            }
        }
        #endregion
        //
    }
}