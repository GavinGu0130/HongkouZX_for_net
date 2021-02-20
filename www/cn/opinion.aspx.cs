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
    public partial class opinion : System.Web.UI.Page
    {
        private DataUser myUser = null;
        private WebOpinion webOpinion = new WebOpinion();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.TrueName))
            {
                //Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.UserType = myUser.UserType;
            LoadNav(myUser, plNav, ltSaveNum, ltSignNum, ltFeedNum);//, ltResultNum
            string strTitle = "";
            switch (Request.QueryString["ac"])
            {
                case "query":
                    strTitle = "检索提案";
                    plQuery.Visible = true;
                    QueryData(myUser.TrueName, rpQueryList, ltQueryNo, lblQueryNav, ltQueryTotal, this, ddlQPeriod, ddlQTimes, ddlQSubType, ddlQCommittee, ddlQSubsector, ddlQStreetTeam, ddlQParty, txtQOpNo, txtQSubMan, ddlQTimeMark, ddlQActiveName, ddlQIsGood, ddlQIsPoint, txtQSummary, txtQBody);
                    break;
                case "my":
                    strTitle = "我的提案";
                    plMy.Visible = true;
                    MyList("归并,待立案,立案,不立案", myUser.TrueName, rpMyList, ltMyNo, lblMyNav, ltMyTotal, this);//我的<>'删除'
                    break;
                case "save":
                    strTitle = "暂存的提案";
                    plSave.Visible = true;
                    MyList("暂存,退回", myUser.TrueName, rpSaveList, ltSaveNo, lblSaveNav, null, this);//暂存
                    break;
                case "sign":
                    if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                    {
                        strTitle = "会签提案";
                        if (!IsPostBack)
                        {
                            LoadSign(myUser, Convert.ToInt32(Request.QueryString["id"]), plView, ltId, ltOpNo, ltPeriod, ltTimes, ltTeamNum, plTeamNum, ltSubTime, ltApplyState, ltActiveName, ltSubType, ltIsPoint, ltIsOpen, ltSubManType, ltSubOrg, ltLinkman, ltLinkmanTel, ltLinkmanAddress, ltLinkmanZip, ltSubMan1, ltSubMans, ltSubMan, ltSummary, ltBody, ltFiles, ltAdviseHostOrg, ltAdviseHelpOrg, plViewResult, ltExamHostOrg, ltExamHelpOrg, ltResult, this, plSignBody, hfSignId, txtSignBody, plSignEdit, ltSignBody);
                        }
                    }
                    else
                    {
                        strTitle = "需会签提案";
                        plSign.Visible = true;
                        SignList(myUser.TrueName, rpSignList, ltSignNo, lblSignNav, this);//需反馈提案
                    }
                    break;
                case "feed":
                    if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                    {
                        strTitle = "反馈提案";
                        LoadFeed(Convert.ToInt32(Request.QueryString["id"]), myUser, plFeedEdit, ltFeedOpNo, ltFeedOpOrg, ltFeedOpResult, ltFeedSummary, ltFeedInterview, ltFeedAttitude, ltFeedTakeWay, ltFeedPertinence, ltFeedLeaderReply, ltFeedResult, ltFeedBody, rblFeedInterview, rblFeedAttitude, rblFeedTakeWay, rblFeedPertinence, rblFeedLeaderReply, rblFeedResult, txtFeedBody, plFeedCmd);
                    }
                    else
                    {
                        strTitle = "需反馈提案";
                        plFeed.Visible = true;
                        FeedList(myUser.TrueName, rpFeedList, ltFeedNo, lblFeedNav, this);//需反馈提案
                    }
                    break;
                case "result":
                    strTitle = "跟踪办理情况";
                    plResult.Visible = true;
                    ResultList(myUser.TrueName, rpResultList, ltResultNo, lblResultNav, this);//跟踪办理情况
                    break;
                default:
                    if (!IsPostBack)
                    {
                        int Id = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                        strTitle = LoadData(Id, myUser, plSub, ddlPeriod, ddlTimes, ddlAdviseHostOrg, ddlAdviseHelpOrg, hfSubManType, txtSubMan, txtSubOrg, txtSubTime, rblIsOpen, txtOpenInfo, txtSummary, txtSubMans, txtLinkman, txtLinkmanTel, txtLinkmanAddress, txtLinkmanZip, txtBody, hfFiles, btnDel, this, hfUserId, plView, ltId, ltOpNo, ltPeriod, ltTimes, ltTeamNum, plTeamNum, ltSubTime, ltApplyState, ltActiveName, ltSubType, ltIsPoint, ltIsOpen, ltSubManType, ltSubOrg, ltLinkman, ltLinkmanTel, ltLinkmanAddress, ltLinkmanZip, ltSubMan1, ltSubMans, ltSubMan, ltSummary, ltBody, ltFiles, ltAdviseHostOrg, ltAdviseHelpOrg, plViewResult, ltExamHostOrg, ltExamHelpOrg, ltResult);
                    }
                    break;
            }
            Header.Title += " - " + strTitle;
        }
        //页面nav
        public void LoadNav(DataUser uData, Panel _plNav, Literal _ltSaveNum, Literal _ltSignNum = null, Literal _ltFeedNum = null, Literal _ltFlowNum = null)
        {
            _plNav.Visible = true;
            DataOpinion qData = new DataOpinion();
            DataOpinion[] data = null;
            qData.ActiveName = "暂存,退回";//
            qData.AddUser = uData.TrueName;
            data = webOpinion.GetDatas(qData, "Id");
            if (data != null)
            {
                _ltSaveNum.Text = (data.Count() > 99) ? "99+" : data.Count().ToString();
            }

            qData.AddUser = null;
            if (_ltFeedNum != null)
            {
                qData.ActiveName = "归并,立案";
                qData.IsFeed = "是";
                qData.OpNo = "<>''";
                //qData.ActiveName = "已办复";//需反馈
                qData.IsSubMan1 = true;
                qData.SubMan = uData.TrueName;
                data = webOpinion.GetDatas(qData, "Id");
                if (data != null)
                {
                    _ltFeedNum.Text = (data.Count() > 99) ? "99+" : data.Count().ToString();
                }
            }

            if (_ltFlowNum != null)
            {
                qData.ActiveName = null;
                qData.IsSubMan1 = true;
                qData.SubMan = uData.TrueName;
                //qData.SubTimeText = null;
                qData.ResultInfo2 = "<>''";//跟踪办理
                data = webOpinion.GetDatas(qData, "Id");
                if (data != null)
                {
                    _ltFlowNum.Text = (data.Count() > 99) ? "99+" : data.Count().ToString();
                }
            }
            //qData.ResultInfo2 = null;

            if (_ltSignNum != null)
            {
                int intNum = GetSignFeed(uData.Id);
                if (intNum > 0)
                {
                    _ltSignNum.Text = (intNum > 99) ? "99+" : intNum.ToString();
                }
            }
        }
        //
        #region 提交
        //初始化提交表
        private void initSub(DataUser uData, PlaceHolder _plSub, DropDownList _ddlPeriod, DropDownList _ddlTimes, DropDownList _ddlAdviseHostOrg, DropDownList _ddlAdviseHelpOrg, HiddenField _hfSubManType, TextBox _txtSubMan, TextBox _txtSubOrg, TextBox _txtSubTime)
        {
            _plSub.Visible = true;
            WebOp webOp = new WebOp();
            PublicMod.LoadDropDownList(_ddlPeriod, webOp, "_届", config.PERIOD);
            PublicMod.LoadDropDownList(_ddlTimes, webOp, "_次会议", config.TIMES);
            //PublicMod.LoadRadioButtonList(_rblSubType, webOp, "提案类别");
            PublicMod.LoadDropDownList(_ddlAdviseHostOrg, webOp, "承办单位");
            PublicMod.LoadDropDownList(_ddlAdviseHelpOrg, webOp, "承办单位");
            _hfSubManType.Value = uData.UserType;
            _txtSubMan.Text = uData.TrueName;
            _txtSubOrg.Text = uData.TrueName;
            _txtSubTime.Text = DateTime.Today.ToString("yyyy-MM-dd");
        }
        //信息
        public string LoadData(int Id, DataUser uData, PlaceHolder _plSub, DropDownList _ddlPeriod, DropDownList _ddlTimes, DropDownList _ddlAdviseHostOrg, DropDownList _ddlAdviseHelpOrg, HiddenField _hfSubManType, TextBox _txtSubMan, TextBox _txtSubOrg, TextBox _txtSubTime, RadioButtonList _rblIsOpen, TextBox _txtOpenInfo, TextBox _txtSummary, TextBox _txtSubMans, TextBox _txtLinkman, TextBox _txtLinkmanTel, TextBox _txtLinkmanAddress, TextBox _txtLinkmanZip, TextBox _txtBody, HiddenField _hfFiles, Button _btnDel, Page page, HiddenField _hfUserId, PlaceHolder _plView, Literal _ltId, Literal _ltOpNo, Literal _ltPeriod, Literal _ltTimes, Literal _ltTeamNum, PlaceHolder _plTeamNum, Literal _ltSubTime, Literal _ltApplyState, Literal _ltActiveName, Literal _ltSubType, Literal _ltIsPoint, Literal _ltIsOpen, Literal _ltSubManType, Literal _ltSubOrg, Literal _ltLinkman, Literal _ltLinkmanTel, Literal _ltLinkmanAddress, Literal _ltLinkmanZip, Literal _ltSubMan1, Literal _ltSubMans, Literal _ltSubMan, Literal _ltSummary, Literal _ltBody, Literal _ltFiles, Literal _ltAdviseHostOrg, Literal _ltAdviseHelpOrg, PlaceHolder _plResult, Literal _ltExamHostOrg, Literal _ltExamHelpOrg, Literal _ltResult)
        {
            string strTitle = "提交提案";
            if (Id <= 0)
            {
                initSub(uData, _plSub, _ddlPeriod, _ddlTimes, _ddlAdviseHostOrg, _ddlAdviseHelpOrg, _hfSubManType, _txtSubMan, _txtSubOrg, _txtSubTime);
                return strTitle;
            }
            DataOpinion[] data = webOpinion.GetData(Id);
            if (data == null)
            {
                page.Response.Redirect("opinion.aspx");
                return "";
            }
            if (data[0].ActiveName == "暂存" || data[0].ActiveName == "退回")
            {
                if (data[0].AddUser == uData.TrueName)
                {
                    initSub(uData, _plSub, _ddlPeriod, _ddlTimes, _ddlAdviseHostOrg, _ddlAdviseHelpOrg, _hfSubManType, _txtSubMan, _txtSubOrg, _txtSubTime);
                    HelperMain.SetDownSelected(_ddlPeriod, data[0].Period);
                    HelperMain.SetDownSelected(_ddlTimes, data[0].Times);
                    if (data[0].SubTime > DateTime.MinValue)
                    {
                        _txtSubTime.Text = data[0].SubTime.ToString("yyyy-MM-dd");
                    }
                    //HelperMain.SetRadioSelected(_rblSubType, data[0].SubType);
                    HelperMain.SetRadioSelected(_rblIsOpen, data[0].IsOpen);
                    _txtOpenInfo.Text = data[0].OpenInfo;
                    _txtSummary.Text = data[0].Summary;
                    _txtSubMan.Text = data[0].SubMan.Trim(',');
                    //string strSubMans = data[0].SubMan2.Trim(',') + "," + data[0].SubMans.Trim(',');
                    _txtSubMans.Text = data[0].SubMans.Trim(',');
                    _txtLinkman.Text = data[0].Linkman;
                    _txtLinkmanTel.Text = data[0].LinkmanTel;
                    _txtLinkmanAddress.Text = data[0].LinkmanAddress;
                    _txtLinkmanZip.Text = data[0].LinkmanZip;
                    HelperMain.SetDownSelected(_ddlAdviseHostOrg, data[0].AdviseHostOrg);
                    HelperMain.SetDownSelected(_ddlAdviseHelpOrg, data[0].AdviseHelpOrg);
                    _txtBody.Text = data[0].Body;
                    _hfFiles.Value = data[0].Files;
                    _btnDel.Visible = true;
                }
            }
            else if (data[0].ActiveName == "归并" || data[0].ActiveName == "立案" || data[0].ActiveName == "待立案" || data[0].ActiveName == "不立案")// || (data[0].ActiveName.IndexOf("删除") < 0 && data[0].AddUser == uData.TrueName)
            {//二种情况：查看、会签
                strTitle = "查阅提案";
                ViewData(data[0], _plView, _ltId, _ltOpNo, _ltPeriod, _ltTimes, _ltTeamNum, _plTeamNum, _ltSubTime, _ltApplyState, _ltActiveName, _ltSubType, _ltIsPoint, _ltIsOpen, _ltSubManType, _ltSubOrg, _ltLinkman, _ltLinkmanTel, _ltLinkmanAddress, _ltLinkmanZip, _ltSubMan1, _ltSubMans, _ltSubMan, _ltSummary, _ltBody, _ltFiles, _ltAdviseHostOrg, _ltAdviseHelpOrg, _plResult, _ltExamHostOrg, _ltExamHelpOrg, _ltResult, uData, _hfUserId);
            }
            else
            {
                page.Response.Redirect("opinion.aspx");
            }
            return strTitle;
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
            ltInfo.Text = EditData(ActiveName, myUser, this, txtSubTime, ddlPeriod, ddlTimes, rblIsOpen, txtOpenInfo, txtSubMan, txtSubMans, txtLinkman, txtLinkmanAddress, txtLinkmanZip, txtLinkmanTel, txtSummary, txtBody, hfFiles, ddlAdviseHostOrg, ddlAdviseHelpOrg);
        }
        public string EditData(string ActiveName, DataUser uData, Page page, TextBox _txtSubTime, DropDownList _ddlPeriod, DropDownList _ddlTimes, RadioButtonList _rblIsOpen, TextBox _txtOpenInfo, TextBox _txtSubMan, TextBox _txtSubMans, TextBox _txtLinkman, TextBox _txtLinkmanAddress, TextBox _txtLinkmanZip, TextBox _txtLinkmanTel, TextBox _txtSummary, TextBox _txtBody, HiddenField _hfFiles, DropDownList _ddlAdviseHostOrg, DropDownList _ddlAdviseHelpOrg)
        {
            if (uData == null)
            {
                return "<script>$(function(){ alert('请重新登录！'); });</script>";
            }
            string strOut = "";
            string strBack = PublicMod.GetBackUrl();
            DataOpinion data = new DataOpinion();
            data.Id = (!string.IsNullOrEmpty(page.Request.QueryString["id"])) ? Convert.ToInt32(page.Request.QueryString["id"]) : 0;
            if (ActiveName == "删除")
            {
                if (data.Id > 0)
                {
                    data.Id = webOpinion.UpdateActive(data.Id, ActiveName);
                }
                else
                {
                    return strOut;
                }
            }
            else
            {
                data.Period = HelperMain.SqlFilter(_ddlPeriod.SelectedValue, 4);
                data.Times = HelperMain.SqlFilter(_ddlTimes.SelectedValue, 4);
                //data.TeamNum = (!string.IsNullOrEmpty(_txtTeamNum.Text)) ? Convert.ToInt16(_txtTeamNum.Text.Trim()) : 0;
                //data.SubType = HelperMain.SqlFilter(_rblSubType.SelectedValue, 20);
                //data.ReApply
                //data.IsPoint
                data.IsOpen = HelperMain.SqlFilter(_rblIsOpen.SelectedValue.Trim(), 2);
                if (data.IsOpen == "否")
                {
                    data.OpenInfo = HelperMain.SqlFilter(_txtOpenInfo.Text.Trim(), 20);
                }
                data.SubMan = "," + HelperMain.SqlFilter(_txtSubMan.Text.Trim(), 20) + ",";
                //data.SubManType = HelperMain.SqlFilter(hfSubManType.Value.Trim(), 8);
                data.SubManType = uData.UserType;
                if (data.SubManType == "委员")
                {
                    if (!string.IsNullOrEmpty(_txtSubMans.Text))
                    {
                        string strTmp = HelperMain.SqlFilter(_txtSubMans.Text.Trim());
                        string[] arr = strTmp.Trim(',').Split(',');
                        string strSubMan = data.SubMan.Trim(',');
                        strTmp = "";
                        for (int i = 0; i < arr.Count(); i++)
                        {
                            if (!string.IsNullOrEmpty(arr[i]) && arr[i] != strSubMan)
                            {
                                strTmp += "," + arr[i];
                            }
                        }
                        if (!string.IsNullOrEmpty(strTmp))
                        {
                            data.SubMans = "," + strTmp.Trim(',') + ",";
                        }
                    }
                }
                else
                {
                    data.Linkman = HelperMain.SqlFilter(_txtLinkman.Text.Trim(), 20);
                    data.LinkmanAddress = HelperMain.SqlFilter(_txtLinkmanAddress.Text.Trim());
                    data.LinkmanZip = HelperMain.SqlFilter(_txtLinkmanZip.Text.Trim(), 6);
                    data.LinkmanTel = HelperMain.SqlFilter(_txtLinkmanTel.Text.Trim(), 50);
                }
                data.IsSign = (!string.IsNullOrEmpty(data.SubMans)) ? "是" : "";
                data.Summary = HelperMain.SqlFilter(_txtSummary.Text.Trim(), 100);
                string strBody = HelperMain.SqlFilter(_txtBody.Text.TrimEnd());
                if (!string.IsNullOrEmpty(strBody))
                {
                    strBody = HttpUtility.UrlDecode(strBody);
                    strBody = HelperMain.DelUbbFont(strBody);
                }
                data.Body = strBody;
                data.Files = HelperMain.SqlFilter(_hfFiles.Value.Trim('|'));
                //data.Remark
                data.AdviseHostOrg = HelperMain.SqlFilter(_ddlAdviseHostOrg.SelectedValue.Trim());
                data.AdviseHelpOrg = HelperMain.SqlFilter(_ddlAdviseHelpOrg.SelectedValue.Trim());
                //data.ExamHostOrg
                //data.ExamHelpOrg
                //data.PlannedDate
                //data.ResultInfo
                //data.ResultBody
                //data.ResultInfo2
                //data.ResultBody2
                DateTime dtNow = DateTime.Now;
                string strIp = HelperMain.GetIpPort();
                string strUser = HelperMain.SqlFilter(uData.TrueName, 20);
                if (ActiveName == "提交")
                {
                    data.ActiveName = "待立案";
                    WebForumSet webSet = new WebForumSet();
                    DataForumSet[] mData = webSet.GetDatas(0, "meeting");
                    if (mData != null)
                    {
                        //会签：会间3小时内，会后1天内
                        if (dtNow >= mData[0].StartDate && dtNow <= mData[0].EndDate)
                        {
                            data.TimeMark = "会间";
                        }
                        else if (dtNow > mData[0].EndDate)
                        {
                            data.TimeMark = "会后";
                        }
                        else
                        {
                            data.TimeMark = "";
                        }
                    }
                    else
                    {
                        data.TimeMark = "会间";
                    }
                    //string strSubTime = HelperMain.SqlFilter(_txtSubTime.Text.Trim());
                    //if (strSubTime.IndexOf(":") < 0)
                    //{
                    //    strSubTime += " " + dtNow.ToString("HH:mm:ss");
                    //}
                    //data.SubTime = (!string.IsNullOrEmpty(strSubTime)) ? Convert.ToDateTime(strSubTime) : dtNow;
                    data.SubTime = dtNow;
                    data.SubIp = strIp;
                    string strParty = uData.Party;
                    string strCommittee = uData.Committee;
                    string strSubsector = uData.Subsector;
                    string strStreetTeam = uData.StreetTeam;
                    //提案中联名委员的党派、专委会、界别、街道活动组
                    //if (!string.IsNullOrEmpty(data.SubMans))
                    //{
                    //    string strSubMans = data.SubMans.Trim(',');
                    //    string[] arr = strSubMans.Split(',');
                    //    WebUser webUser = new WebUser();
                    //    for (int i = 0; i < arr.Count(); i++)
                    //    {
                    //        if (!string.IsNullOrEmpty(arr[i]))
                    //        {
                    //            DataUser[] uData2 = webUser.GetDatas(config.PERIOD, arr[i], "Id,Party,Committee,Committee2,Subsector,StreetTeam");
                    //            if (uData2 != null)
                    //            {
                    //                if (!string.IsNullOrEmpty(uData2[0].Party) && (strParty + ",").IndexOf(uData2[0].Party) < 0)
                    //                {
                    //                    strParty += "," + uData2[0].Party;
                    //                }
                    //                if (!string.IsNullOrEmpty(uData2[0].Committee) && (strCommittee + ",").IndexOf(uData2[0].Committee) < 0)
                    //                {
                    //                    strCommittee += "," + uData2[0].Committee;
                    //                }
                    //                if (!string.IsNullOrEmpty(uData2[0].Committee2) && (strCommittee + ",").IndexOf(uData2[0].Committee2) < 0)
                    //                {
                    //                    strCommittee += "," + uData2[0].Committee2;
                    //                }
                    //                if (!string.IsNullOrEmpty(uData2[0].Subsector) && (strSubsector + ",").IndexOf(uData2[0].Subsector) < 0)
                    //                {
                    //                    strSubsector += "," + uData2[0].Subsector;
                    //                }
                    //                if (!string.IsNullOrEmpty(uData2[0].StreetTeam) && (strStreetTeam + ",").IndexOf(uData2[0].StreetTeam) < 0)
                    //                {
                    //                    strStreetTeam += "," + uData2[0].StreetTeam;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    data.Party = strParty.Trim(',');
                    data.Committee = strCommittee.Trim(',');
                    data.Subsector = strSubsector.Trim(',');
                    data.StreetTeam = strStreetTeam.Trim(',');
                }
                else
                {
                    data.ActiveName = "暂存";
                }
                if (data.Id <= 0)
                {
                    DataOpinion qData = new DataOpinion();
                    qData.Period = config.PERIOD;
                    qData.Times = data.Times;
                    qData.Summary = data.Summary;
                    //qData.AddTime = DateTime.Today;
                    qData.AddUser = strUser;
                    qData.ActiveName = "<>'删除'";
                    DataOpinion[] ckData = webOpinion.GetDatas(qData, "Id", 0, 1);//重复检查
                    if (ckData != null)
                    {
                        strOut = "<script>$(function(){ alert('“案由”重复，不能提交！\\n如需修改请联系：提案科 " + config.TelTeam + "'); window.history.back(-1); });</script>";
                        //strOut = "<script>$(function(){ alert('重复提交！'); });</script>";
                        return strOut;
                        //page.Response.Redirect(strBack);
                    }
                    data.UserId = uData.Id;
                    data.AddTime = dtNow;
                    data.AddIp = strIp;
                    data.AddUser = strUser;
                    data.Id = webOpinion.Insert(data);
                }
                else
                {
                    data.UserId = -1;
                    data.UpTime = dtNow;
                    data.UpIp = strIp;
                    data.UpUser = strUser;
                    if (webOpinion.Update(data) <= 0)
                    {
                        data.Id = -1;
                    }
                }
            }
            if (data.Id > 0)
            {
                strOut = "<script>$(function(){ alert('“" + ActiveName + "提案”成功！'); window.location.href='" + strBack + "'; });</script>";
                if (ActiveName == "提交")
                {
                    AddSign(uData.TrueName, data.Id, data.SubMans, data.TimeMark);//增加会签人
                }
            }
            else
            {
                strOut = "<script>$(function(){ alert('“" + ActiveName + "提案”失败！'); window.history.back(-1); });</script>";
            }
            return strOut;
        }
        #endregion
        //
        #region 查阅提案
        //首页我的提案（第一、联名提交人）
        public void MyOpinion(string strUser, Repeater rpList)
        {
            DataOpinion data = new DataOpinion();
            data.ActiveName = "归并,待立案,立案,不立案";
            data.SubMan = strUser;
            //data.SubMan1 = strUser;
            listData(strUser, data, rpList);
        }
        //加载我的列表：已提交的、暂存/退回
        public void MyList(string ActiveName, string strUser, Repeater rpList, Literal ltNo, Label lblNav, Literal ltTotal, Page page)
        {
            DataOpinion data = new DataOpinion();
            data.ActiveName = ActiveName;
            data.IsSubMan1 = true;
            data.SubMan = strUser;
            //data.AddUser = strUser;
            listData(strUser, data, rpList, ltNo, lblNav, ltTotal, page);
        }
        //需会签提案
        public void SignList(string strUser, Repeater rpList, Literal ltNo, Label lblNav, Page page)
        {
            DataOpinion data = new DataOpinion();
            data.ActiveName = "待立案";
            //data.SubMans = strUser;
            data.SubMan3 = strUser;
            data.Period = config.PERIOD;
            data.Times = config.TIMES;
            string strJoin = " LEFT JOIN tb_Opinion_Sign AS s ON (s.OpId=o.Id AND s.SignUser=@SubMan3)";
            listData(strUser, data, rpList, ltNo, lblNav, null, page, strJoin);
        }
        //需反馈的（第一提交人）
        public void FeedList(string strUser, Repeater rpList, Literal ltNo, Label lblNav, Page page)
        {
            DataOpinion data = new DataOpinion();
            data.ActiveName = "归并,立案";
            data.IsFeed = "是";
            data.OpNo = "<>''";
            //data.ApplyState = "已办复";
            data.IsSubMan1 = true;
            data.SubMan = strUser;
            listData(strUser, data, rpList, ltNo, lblNav, null, page);
        }
        //跟踪办理列表
        public void ResultList(string strUser, Repeater rpList, Literal ltNo, Label lblNav, Page page)
        {
            DataOpinion data = new DataOpinion();
            data.ResultInfo2 = "<>''";
            data.SubMan = strUser;
            //data.AddUser = strUser;
            listData(strUser, data, rpList, ltNo, lblNav, null, page);
        }
        //提案查询
        public void QueryData(string strUser, Repeater rpList, Literal ltNo, Label lblNav, Literal ltTotal, Page page, DropDownList _ddlQPeriod, DropDownList _ddlQTimes, DropDownList _ddlQSubType, DropDownList _ddlQCommittee, DropDownList _ddlQSubsector, DropDownList _ddlQStreetTeam, DropDownList _ddlQParty, TextBox _txtQOpNo, TextBox _txtQSubMan, DropDownList _ddlQTimeMark, DropDownList _ddlQActiveName, DropDownList _ddlQIsGood, DropDownList _ddlQIsPoint, TextBox _txtQSummary, TextBox _txtQBody)
        {
            WebOp webOp = new WebOp();
            PublicMod.LoadDropDownList(_ddlQPeriod, webOp, "_届", config.PERIOD);
            PublicMod.LoadDropDownList(_ddlQTimes, webOp, "_次会议", config.TIMES);
            //_ddlQTimes.SelectedIndex = 0;
            PublicMod.LoadDropDownList(_ddlQSubType, webOp, "提案类别");
            PublicMod.LoadDropDownList(_ddlQCommittee, webOp, "专委会");
            PublicMod.LoadDropDownList(_ddlQSubsector, webOp, "界别");
            PublicMod.LoadDropDownList(_ddlQStreetTeam, webOp, "街道活动组");
            PublicMod.LoadDropDownList(_ddlQParty, webOp, "政治面貌");
            DataOpinion data = new DataOpinion();
            data.OpNo = "<>''";
            data.IsOpen = "<>'否'";//是
            data.ActiveName = "归并,待立案,立案,不立案";
            if (!string.IsNullOrEmpty(page.Request.QueryString["SubMan"]))
            {
                data.SubMan = HelperMain.SqlFilter(page.Request.QueryString["SubMan"].Trim(), 20);
                _txtQSubMan.Text = data.SubMan;
            }
            //else
            //{
            //    data.SubMan = HelperMain.SqlFilter(strUser, 20);
            //}
            if (!string.IsNullOrEmpty(page.Request.QueryString["Period"]))
            {
                data.Period = HelperMain.SqlFilter(page.Request.QueryString["Period"].Trim(), 4);
                HelperMain.SetDownSelected(_ddlQPeriod, data.Period);
            }
            else if (page.Request.QueryString.ToString() == "ac=query" || page.Request.QueryString.ToString().IndexOf("ac=query&page=") >= 0)
            {
                data.Period = HelperMain.SqlFilter(_ddlQPeriod.SelectedValue, 4);
            }
            else
            {
                _ddlQPeriod.SelectedIndex = -1;
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["Times"]))
            {
                data.Times = HelperMain.SqlFilter(page.Request.QueryString["Times"].Trim(), 4);
                HelperMain.SetDownSelected(_ddlQTimes, data.Times);
            }
            else if (page.Request.QueryString.ToString() == "ac=query" || page.Request.QueryString.ToString().IndexOf("ac=query&page=") >= 0)
            {
                data.Times = HelperMain.SqlFilter(_ddlQTimes.SelectedValue, 4);
            }
            else
            {
                _ddlQTimes.SelectedIndex = -1;
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["OpNo"]))
            {
                data.OpNo = HelperMain.SqlFilter(page.Request.QueryString["OpNo"].Trim(), 20);
                _txtQOpNo.Text = data.OpNo;
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["TimeMark"]))
            {
                data.TimeMark = HelperMain.SqlFilter(page.Request.QueryString["TimeMark"].Trim(), 4);
                HelperMain.SetDownSelected(_ddlQTimeMark, data.TimeMark);
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["ActiveName"]))
            {
                data.ActiveName = HelperMain.SqlFilter(page.Request.QueryString["ActiveName"].Trim(), 10);
                HelperMain.SetDownSelected(_ddlQActiveName, data.ActiveName);
                //data.ActiveName = data.ActiveName.Replace("|", ",");
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["IsGood"]))
            {
                data.IsGood = HelperMain.SqlFilter(page.Request.QueryString["IsGood"].Trim(), 4);
                HelperMain.SetDownSelected(_ddlQIsGood, data.IsGood);
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["IsPoint"]))
            {
                data.IsPoint = HelperMain.SqlFilter(page.Request.QueryString["IsPoint"].Trim(), 4);
                HelperMain.SetDownSelected(_ddlQIsPoint, data.IsPoint);
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["SubType"]))
            {
                data.SubType = HelperMain.SqlFilter(page.Request.QueryString["SubType"].Trim(), 20);
                HelperMain.SetDownSelected(_ddlQSubType, data.SubType);
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["Committee"]))
            {
                string strCommittee = HelperMain.SqlFilter(page.Request.QueryString["Committee"].Trim(), 20);
                HelperMain.SetDownSelected(_ddlQCommittee, strCommittee);
                data.UserCommittee = strCommittee;
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["Subsector"]))
            {
                string strSubsector = HelperMain.SqlFilter(page.Request.QueryString["Subsector"].Trim(), 20);
                HelperMain.SetDownSelected(_ddlQSubsector, strSubsector);
                data.UserSubsector = strSubsector;
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["StreetTeam"]))
            {
                string strStreetTeam = HelperMain.SqlFilter(page.Request.QueryString["StreetTeam"].Trim(), 20);
                HelperMain.SetDownSelected(_ddlQStreetTeam, strStreetTeam);
                data.UserStreetTeam = strStreetTeam;
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["Party"]))
            {
                string strParty = HelperMain.SqlFilter(page.Request.QueryString["Party"].Trim(), 20);
                HelperMain.SetDownSelected(_ddlQParty, strParty);
                data.UserParty = strParty;
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["Summary"]))
            {
                data.Summary = "%" + HelperMain.SqlFilter(page.Request.QueryString["Summary"].Trim(), 100) + "%";
                _txtQSummary.Text = data.Summary.Trim('%');
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["Body"]))
            {
                data.Body = "%" + HelperMain.SqlFilter(page.Request.QueryString["Body"].Trim()) + "%";
                _txtQBody.Text = data.Body.Trim('%');
            }
            //联合查询开始
            string strJoin = "";
            if (!string.IsNullOrEmpty(data.UserSex) || !string.IsNullOrEmpty(data.UserParty) || !string.IsNullOrEmpty(data.UserCommittee) || !string.IsNullOrEmpty(data.UserSubsector) || !string.IsNullOrEmpty(data.UserStreetTeam))
            {
                strJoin += " INNER JOIN tb_User AS u ON (o.SubMan LIKE '%' + u.TrueName + '%' OR o.SubMans LIKE '%' + u.TrueName + '%')";
            }
            if (!string.IsNullOrEmpty(strJoin))
            {//联合查询，先执行一次去重复查询，再根据Id查询结果
                DataOpinion[] data2 = webOpinion.GetDatas(data, "o.Id", 1, 0, "", "distinct", strJoin);
                if (data2 == null)
                {
                    //ltInfo.Text = "<script>$(function(){ alert('未查询到“提案”'); window.history.back(-1); });</script>";
                    return;
                }
                DataOpinion qData2 = new DataOpinion();
                string strId = "";
                for (int i = 0; i < data2.Count(); i++)
                {
                    if (i > 0)
                    {
                        strId += ",";
                    }
                    strId += data2[i].Id.ToString();
                }
                qData2.MergeId = strId;
                data = qData2;
            }
            //联合查询结束
            listData(strUser, data, rpList, ltNo, lblNav, ltTotal, page);
        }
        private void listData(string strUser, DataOpinion qData, Repeater rpList, Literal ltNo = null, Label lblNav = null, Literal ltTotal = null, Page page = null, string strJoin = "")
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
            string strOrder = "OpNo ASC"; 
            DataOpinion[] data = webOpinion.GetDatas(qData, "", pageCur, pageSize, strOrder, "total", strJoin);
            if (data != null)
            {
                WebOpinionSign webSign = new WebOpinionSign();
                WebOpinionFeed webFeed = new WebOpinionFeed();
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    if (data[i].SubTime > DateTime.MinValue)
                    {
                        data[i].SubTimeText = data[i].SubTime.ToString("yyyy-MM-dd");
                    }
                    switch (data[i].ActiveName)
                    {
                        case "删除":
                            data[i].rowClass = " class='del' title='删除'";
                            break;
                        case "暂存":
                            data[i].rowClass = " class='save' title='暂存'";
                            data[i].StateName = "修改";
                            //data[i].ApplyState = data[i].ActiveName;
                            break;
                        case "退回":
                            data[i].rowClass = " class='cancel' title='退回'";
                            data[i].StateName = "修改";
                            //data[i].ApplyState = data[i].ActiveName;
                            break;
                        case "待立案":
                            data[i].rowClass = " class='wait' title='待立案'";//
                            //data[i].ApplyState = data[i].ActiveName;
                            data[i].StateName = "查看";
                            break;
                        default:
                            data[i].StateName = "查看";
                            break;
                    }
                    if (data[i].IsGood == "是")
                    {
                        data[i].Summary = "<i class='flag'>优秀</i>" + data[i].Summary;
                    }
                    if (data[i].IsPoint == "是")
                    {
                        data[i].Summary = "<i class='flag'>重点</i>" + data[i].Summary;
                    }
                    if (DateTime.Today < data[i].SubTime.AddMonths(6))
                    {//提案6个月内，暂不显示主办、会办单位
                        data[i].ExamHostOrg = "";
                        data[i].ExamHelpOrg = "";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(data[i].ExamHostOrg))
                        {
                            string[] arr = data[i].ExamHostOrg.Split(',');
                            for (int j = 0; j < arr.Count(); j++)
                            {
                                if (arr[j].IndexOf("|") > 0)
                                {
                                    arr[j] = arr[j].Substring(0, arr[j].IndexOf("|"));
                                }
                            }
                            data[i].ExamHostOrg = string.Join("<br/>", arr);
                        }
                        if (!string.IsNullOrEmpty(data[i].ExamHelpOrg))
                        {
                            string[] arr = data[i].ExamHelpOrg.Split(',');
                            for (int j = 0; j < arr.Count(); j++)
                            {
                                if (arr[j].IndexOf("|") > 0)
                                {
                                    arr[j] = arr[j].Substring(0, arr[j].IndexOf("|"));
                                }
                            }
                            data[i].ExamHelpOrg = string.Join("<br/>", arr);
                        }
                    }
                    if (data[i].ApplyState == "已办复")
                    {
                        if (!string.IsNullOrEmpty(data[i].ResultInfo))
                        {
                            data[i].ApplyState = data[i].ResultInfo;
                        }
                        else if (!string.IsNullOrEmpty(data[i].ResultInfo2))
                        {
                            data[i].ApplyState = data[i].ResultInfo2;
                        }
                    }
                    else if (data[i].ApplyState == "部门处理")
                    {
                        data[i].ApplyState = "";
                    }
                    if (isFeed(data[i], strUser))
                    {//需反馈数据表
                        string strOpType = "提案";
                        DataOpinionFeed[] fData = webFeed.GetDatas(0, strOpType, data[i].Id, data[i].UserId, "Active");
                        if (fData != null && fData[0].Active > 0)
                        {
                            if (data[i].ApplyState == "已办复")
                            {
                                data[i].ApplyState += "<br/>已征询";
                            }
                            else
                            {
                                data[i].ApplyState = "已征询";
                            }
                        }
                        data[i].other = string.Format("<a href='opinion.aspx?ac=feed&id={0}' class='btn'><u>意见征询</u></a>", data[i].Id);
                    }
                    else if ((qData.ActiveName == "待立案" || data[i].ActiveName == "待立案") && data[i].StateName == "查看" && data[i].SubMans.IndexOf("," + strUser + ",") >= 0)
                    {//需会签数据表
                        DataOpinionSign qSign = new DataOpinionSign();
                        qSign.OpType = "提案";
                        qSign.OpId = data[i].Id;
                        qSign.SignUser = strUser;
                        //qSign.UserId = myUser.Id;
                        //qSign.OverdueText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",";
                        qSign.ActiveName = ">=-1";
                        DataOpinionSign[] sData = webSign.GetDatas(qSign, "Overdue,SignMark,Active");
                        if (sData != null)
                        {
                            if (sData[0].Active > 0)
                            {
                                //data[i].ActiveName = sData[0].SignMark + "已签";
                                string strTxt = (qData.ActiveName == "待立案") ? "查看" : "会签";
                                data[i].other = string.Format("<a href='opinion.aspx?ac=sign&id={0}' class='btn'><u>{1}</u></a>", data[i].Id, strTxt);
                            }
                            else if (sData[0].Overdue < DateTime.Now)
                            {
                                data[i].ActiveName = "会签结束";
                                if (qData.ActiveName == "待立案")
                                {
                                    //data[i].ActiveName = "未签";
                                    data[i].other = string.Format("<a href='opinion.aspx?id={0}' class='btn'><u>查看</u></a>", data[i].Id);
                                }
                            }
                            else if (sData[0].Active == 0)
                            {
                                data[i].ActiveName = "待会签";
                                data[i].other = string.Format("<a href='opinion.aspx?ac=sign&id={0}' class='btn'><u>会签</u></a>", data[i].Id);
                            }
                            else if (sData[0].Active == -1)
                            {
                                data[i].ActiveName = "谢绝会签";
                            }
                        }
                    }
                    else
                    {
                        //data[i].other = data[i].ResultInfo;
                    }
                    //if (data[i].ActiveName == data[i].ApplyState)
                    //{
                    //    data[i].ApplyState = "";
                    //}
                    string strSubMans = data[i].SubMans;//联名提案人data[i].SubMan2 + "," + 
                    strSubMans = strSubMans.Trim(',');
                    if (!string.IsNullOrEmpty(strSubMans))
                    {
                        strSubMans = data[i].SubMan.Trim(',') + "<br/>" + strSubMans;
                    }
                    else
                    {
                        strSubMans = data[i].SubMan.Trim(',');
                    }
                    //data[i].IsSubMan1 = true;
                    data[i].SubMan = strSubMans;
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
        //查看
        private void ViewData(DataOpinion data, PlaceHolder _plView, Literal _ltId, Literal _ltOpNo, Literal _ltPeriod, Literal _ltTimes, Literal _ltTeamNum, PlaceHolder _plTeamNum, Literal _ltSubTime, Literal _ltApplyState, Literal _ltActiveName, Literal _ltSubType, Literal _ltIsPoint, Literal _ltIsOpen, Literal _ltSubManType, Literal _ltSubOrg, Literal _ltLinkman, Literal _ltLinkmanTel, Literal _ltLinkmanAddress, Literal _ltLinkmanZip, Literal _ltSubMan1, Literal _ltSubMans, Literal _ltSubMan, Literal _ltSummary, Literal _ltBody, Literal _ltFiles, Literal _ltAdviseHostOrg, Literal _ltAdviseHelpOrg, PlaceHolder _plResult, Literal _ltExamHostOrg, Literal _ltExamHelpOrg, Literal _ltResult, DataUser uData = null, HiddenField _hfUserId = null)
        {
            bool isMy = false;
            _plView.Visible = true;
            if (uData != null && _hfUserId != null && data.UserId == uData.Id)
            {
                isMy = true;
                _hfUserId.Value = data.UserId.ToString();
            }
            _ltId.Text = data.Id.ToString();
            _ltOpNo.Text = data.OpNo;
            _ltPeriod.Text = data.Period;
            _ltTimes.Text = data.Times;
            if (data.TeamNum > 0)
            {
                _ltTeamNum.Text = data.TeamNum.ToString();
                _plTeamNum.Visible = true;
            }
            _ltSubType.Text = data.SubType;
            if (isMy)
            {
                _ltSubTime.Text = data.SubTime.ToString("yyyy-MM-dd");// HH:mm:ss
                _ltApplyState.Text = data.ApplyState;
                _ltActiveName.Text = data.ActiveName;
                _ltIsPoint.Text = data.IsPoint;
                _ltIsOpen.Text = data.IsOpen;
                if (!string.IsNullOrEmpty(data.OpenInfo))
                {
                    _ltIsOpen.Text += "，" + data.OpenInfo;
                }
            }
            _ltSubManType.Text = data.SubManType;
            if (data.SubManType == "委员")
            {
                if (!string.IsNullOrEmpty(data.SubMans))//!string.IsNullOrEmpty(data.SubMan2) || 
                {//联名
                    _ltSubManType.Text = "联名";
                    _ltSubMan1.Text = data.SubMan.Trim(',');
                    string strSubMans = data.SubMans.Trim(',');
                    _ltSubMans.Text = PublicMod.GetSubMans(data.Id, strSubMans);
                }
                else
                {//个人
                    _ltSubMan.Text = data.SubMan.Trim(',');
                }
            }
            else
            {
                _ltSubOrg.Text = data.SubMan.Trim(',');
                if (isMy)
                {
                    _ltLinkmanAddress.Text = data.LinkmanAddress;
                    _ltLinkmanZip.Text = data.LinkmanZip;
                    _ltLinkman.Text = data.Linkman;
                    _ltLinkmanTel.Text = data.LinkmanTel;
                }
            }
            _ltSummary.Text = data.Summary;
            if (data.IsGood == "是")
            {
                _ltSummary.Text = "<i class='flag'>优秀</i>" + _ltSummary.Text;
            }
            if (data.IsPoint == "是")
            {
                _ltSummary.Text = "<i class='flag'>重点</i>" + _ltSummary.Text;
            }
            _ltBody.Text = data.Body;
            if (!string.IsNullOrEmpty(data.Files))
            {
                string strFiles = "";
                string[] arr = data.Files.Split('|');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(arr[i]))
                    {
                        strFiles += string.Format("<a href='{0}' target='_blank'>{0}</a><br/>", arr[i]);
                    }
                }
                _ltFiles.Text = strFiles;
            }
            if (isMy)
            {
                _ltAdviseHostOrg.Text = PublicMod.GetOrgText(data.AdviseHostOrg);
                _ltAdviseHelpOrg.Text = PublicMod.GetOrgText(data.AdviseHelpOrg);
            }
            if (!string.IsNullOrEmpty(data.ResultInfo))
            {
                _plResult.Visible = true;
                if (isMy)
                {
                    if (!string.IsNullOrEmpty(data.ExamHostOrg))
                    {
                        string[] arr = data.ExamHostOrg.Split(',');
                        for (int i = 0; i < arr.Count(); i++)
                        {
                            if (arr[i].IndexOf("|") > 0)
                            {
                                arr[i] = arr[i].Substring(0, arr[i].IndexOf("|"));
                            }
                        }
                        _ltExamHostOrg.Text = string.Join(",", arr);
                    }
                    if (!string.IsNullOrEmpty(data.ExamHelpOrg))
                    {
                        string[] arr = data.ExamHelpOrg.Split(',');
                        for (int i = 0; i < arr.Count(); i++)
                        {
                            if (arr[i].IndexOf("|") > 0)
                            {
                                arr[i] = arr[i].Substring(0, arr[i].IndexOf("|"));
                            }
                        }
                        _ltExamHelpOrg.Text = string.Join(",", arr);
                    }
                }
                string strResult = "<b>" + data.ResultInfo + "</b>";
                if (!string.IsNullOrEmpty(data.ResultBody))
                {
                    strResult += "<hr/>" + data.ResultBody.Replace("\n", "<br/>");
                }
                if (!string.IsNullOrEmpty(data.ResultInfo2))
                {
                    strResult += "<hr/><b>" + data.ResultInfo2 + "</b>";
                    if (!string.IsNullOrEmpty(data.ResultBody2))
                    {
                        strResult += "<hr/>" + data.ResultBody2.Replace("\n", "<br/>");
                    }
                }
                _ltResult.Text = strResult;
            }
        }
        #endregion
        //
        #region 会签提案
        //待会签数
        public int GetSignFeed(int UserId)
        {
            int intNum = 0;
            //qData.ActiveName = "待立案";
            //qData.SubMans = strUser;
            //data = webOpinion.GetDatas(qData, "Id");
            DataOpinionSign qSign = new DataOpinionSign();
            qSign.OpType = "提案";
            qSign.UserId = UserId;
            qSign.OverdueText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",";
            qSign.ActiveName = "0";
            WebOpinionSign webSign = new WebOpinionSign();
            DataOpinionSign[] sData = webSign.GetDatas(qSign, "OpId");
            if (sData != null)
            {
                intNum = sData.Count();
            }
            return intNum;
        }
        //增加会签人
        public void AddSign(string strUser, int OpId, string SubMans, string TimeMark)
        {
            if (string.IsNullOrEmpty(SubMans))
            {
                return;
            }
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            WebUser webUser = new WebUser();
            string[] arr = SubMans.Split(',');
            for (int i = 0; i < arr.Count(); i++)
            {
                if (!string.IsNullOrEmpty(arr[i]))
                {
                    DataUser[] uData = webUser.GetDatas(config.PERIOD, arr[i], "Id,TrueName");
                    if (uData != null)
                    {
                        DateTime dtOverdue = (TimeMark == "会间") ? dtNow.AddHours(3) : dtNow.AddDays(1);//会签：会间3小时内，会后1天内
                        DataOpinionSign qSign = new DataOpinionSign();
                        qSign.OpType = "提案";
                        qSign.OpId = OpId;
                        qSign.UserId = uData[0].Id;
                        WebOpinionSign webSign = new WebOpinionSign();
                        DataOpinionSign[] sData = webSign.GetDatas(qSign, "Id,Overdue,Active");
                        if (sData != null)
                        {
                            if (sData[0].Active == 0)
                            {
                                webSign.UpdateOverdue(sData[0].Id, dtOverdue);//更新签名结束时间
                            }
                        }
                        else
                        {
                            DataOpinionSign data = new DataOpinionSign();
                            data.OpType = "提案";
                            data.OpId = OpId;
                            data.UserId = uData[0].Id;
                            data.SignUser = uData[0].TrueName;
                            data.Overdue = dtOverdue;
                            //data.SignTime
                            //data.SignIp
                            data.SignMark = TimeMark;
                            //data.Body
                            //data.Remark
                            data.Active = 0;
                            data.AddTime = dtNow;
                            data.AddIp = strIp;
                            data.AddUser = strUser;
                            webSign.Insert(data);

                        }
                    }
                }
            }
        }
        //加载会签
        public void LoadSign(DataUser uData, int Id, PlaceHolder _plView, Literal _ltId, Literal _ltOpNo, Literal _ltPeriod, Literal _ltTimes, Literal _ltTeamNum, PlaceHolder _plTeamNum, Literal _ltSubTime, Literal _ltApplyState, Literal _ltActiveName, Literal _ltSubType, Literal _ltIsPoint, Literal _ltIsOpen, Literal _ltSubManType, Literal _ltSubOrg, Literal _ltLinkman, Literal _ltLinkmanTel, Literal _ltLinkmanAddress, Literal _ltLinkmanZip, Literal _ltSubMan1, Literal _ltSubMans, Literal _ltSubMan, Literal _ltSummary, Literal _ltBody, Literal _ltFiles, Literal _ltAdviseHostOrg, Literal _ltAdviseHelpOrg, PlaceHolder _plResult, Literal _ltExamHostOrg, Literal _ltExamHelpOrg, Literal _ltResult, Page page, PlaceHolder _plSignBody, HiddenField _hfSignId, TextBox _txtSignBody, Panel _plSignEdit, Literal _ltSignBody)
        {
            DataOpinion[] data = webOpinion.GetData(Id);
            if (data != null)
            {
                ViewData(data[0], _plView, _ltId, _ltOpNo, _ltPeriod, _ltTimes, _ltTeamNum, _plTeamNum, _ltSubTime, _ltApplyState, _ltActiveName, _ltSubType, _ltIsPoint, _ltIsOpen, _ltSubManType, _ltSubOrg, _ltLinkman, _ltLinkmanTel, _ltLinkmanAddress, _ltLinkmanZip, _ltSubMan1, _ltSubMans, _ltSubMan, _ltSummary, _ltBody, _ltFiles, _ltAdviseHostOrg, _ltAdviseHelpOrg, _plResult, _ltExamHostOrg, _ltExamHelpOrg, _ltResult);
                if (data[0].SubMans.IndexOf("," + uData.TrueName + ",") >= 0)
                {
                    DataOpinionSign qSign = new DataOpinionSign();
                    qSign.OpType = "提案";
                    qSign.OpId = Id;
                    qSign.UserId = uData.Id;
                    //qSign.ActiveName = "<>''";
                    WebOpinionSign webSign = new WebOpinionSign();
                    DataOpinionSign[] sData = webSign.GetDatas(qSign, "Id,Overdue,Body,Active");
                    if (sData != null)
                    {
                        _plSignBody.Visible = true;
                        if (sData[0].Active > 0)
                        {//已会签
                            _ltSignBody.Text = sData[0].Body;
                            return;
                        }
                        else if (sData[0].Overdue < DateTime.Now || sData[0].Active < 0)
                        {//会签结束、或取消
                            _plSignBody.Visible = false;
                            return;
                        }
                        else if (sData[0].Active == 0)
                        {//待会签
                            _hfSignId.Value = sData[0].Id.ToString();
                            _txtSignBody.Visible = true;
                            _plSignEdit.Visible = true;
                        }
                    }
                }
            }
            else
            {
                page.Response.Redirect("opinion.aspx");
            }
        }
        //提交会签
        protected void btnSign_Click(object sender, EventArgs e)
        {
            ltInfo.Text = EditSign(myUser, this, hfSignId, txtSignBody);
        }
        public string EditSign(DataUser uData, Page page, HiddenField _hfSignId, TextBox _txtSignBody)
        {
            if (uData == null)
            {
                return "<script>$(function(){ alert('请重新登录！'); });</script>";
            }
            string strOut = "";
            string strBack = PublicMod.GetBackUrl();
            int OpId = (!string.IsNullOrEmpty(page.Request.QueryString["id"])) ? Convert.ToInt32(page.Request.QueryString["id"]) : 0;
            if (OpId <= 0)
            {
                return "<script>$(function(){ alert('错误！'); });</script>";
            }
            DataOpinion[] oData = webOpinion.GetData(OpId, "Id,Party,Committee,Subsector,StreetTeam,ActiveName");
            if (oData == null || oData[0].ActiveName != "待立案")
            {
                return "<script>$(function(){ alert('[提案会签]错误！'); window.location.href='" + strBack + "'; });</script>";
            }
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(uData.TrueName, 20);
            WebOpinionSign webSign = new WebOpinionSign();
            DataOpinionSign data = new DataOpinionSign();
            data.Id = (!string.IsNullOrEmpty(_hfSignId.Value)) ? Convert.ToInt32(_hfSignId.Value) : 0;
            data.OpType = "提案";
            data.OpId = OpId;
            data.UserId = uData.Id;
            data.SignUser = strUser;
            //data.Overdue
            data.SignTime = dtNow;
            data.SignIp = strIp;
            //data.SignMark
            data.Body = HelperMain.SqlFilter(_txtSignBody.Text.Trim(), 100);
            //data.Remark
            data.Active = 1;
            if (data.Id <= 0)
            {
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = strUser;
                data.Id = webSign.Insert(data);
            }
            else
            {
                DataOpinionSign[] sData = webSign.GetData(data.Id, "Overdue");
                if (sData == null || sData[0].Overdue < DateTime.Now)
                {
                    return "<script>$(function(){ alert('[提案会签]已结束！'); window.location.href='" + strBack + "'; });</script>";
                }
                data.UpTime = dtNow;
                data.UpIp = strIp;
                data.UpUser = strUser;
                if (webSign.Update(data) <= 0)
                {
                    data.Id = -1;
                }
            }
            string strParty = oData[0].Party;
            string strCommittee = oData[0].Committee;
            string strSubsector = oData[0].Subsector;
            string strStreetTeam = oData[0].StreetTeam;
            if (!string.IsNullOrEmpty(uData.Party) && strParty.IndexOf(uData.Party) < 0)
            {
                strParty += "," + uData.Party;
            }
            if (!string.IsNullOrEmpty(uData.Committee) && strCommittee.IndexOf(uData.Committee) < 0)
            {
                strCommittee += "," + uData.Committee;
            }
            if (!string.IsNullOrEmpty(uData.Committee2) && strCommittee.IndexOf(uData.Committee2) < 0)
            {
                strCommittee += "," + uData.Committee2;
            }
            if (!string.IsNullOrEmpty(uData.Subsector) && strSubsector.IndexOf(uData.Subsector) < 0)
            {
                strSubsector += "," + uData.Subsector;
            }
            if (!string.IsNullOrEmpty(uData.StreetTeam) && strStreetTeam.IndexOf(uData.StreetTeam) < 0)
            {
                strStreetTeam += "," + uData.StreetTeam;
            }
            strParty = strParty.Trim(',');
            strCommittee = strCommittee.Trim(',');
            strSubsector = strSubsector.Trim(',');
            strStreetTeam = strStreetTeam.Trim(',');
            webOpinion.UpdateUser(oData[0].Id, strParty, strCommittee, strSubsector, strStreetTeam);//更新提案中联名委员的党派、专委会、界别、街道活动组
            if (data.Id > 0)
            {
                strOut = "<script>$(function(){ alert('“同意会签”已提交成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                strOut = "<script>$(function(){ alert('“同意会签”提交失败！'); window.history.back(-1); });</script>";
            }
            return strOut;
        }
        //谢绝会签
        protected void btnNoSign_Click(object sender, EventArgs e)
        {
            ltInfo.Text = NoSign(myUser, this, hfSignId);
        }
        public string NoSign(DataUser uData, Page page, HiddenField _hfSignId)
        {
            if (uData == null)
            {
                return "<script>$(function(){ alert('请重新登录！'); });</script>";
            }
            string strOut = "";
            string strBack = PublicMod.GetBackUrl();
            int intId = (!string.IsNullOrEmpty(_hfSignId.Value)) ? Convert.ToInt32(_hfSignId.Value) : 0;
            WebOpinionSign webSign = new WebOpinionSign();
            if (webSign.UpdateActive(intId, -1) <= 0)
            {
                intId = -1;
            }
            if (intId > 0)
            {
                strOut = "<script>$(function(){ alert('“谢绝会签”已提交成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                strOut = "<script>$(function(){ alert('“谢绝会签”提交失败！'); window.history.back(-1); });</script>";
            }
            return strOut;
        }
        #endregion
        //
        #region 需反馈提案
        //反馈判断
        private bool isFeed(DataOpinion data, string strUser)
        {
            if (!string.IsNullOrEmpty(data.OpNo) && (data.ActiveName == "归并" || data.ActiveName == "立案") && (data.ResultInfo == "解决或采纳" || data.ResultInfo == "列入计划拟解决" || data.ResultInfo == "留作参考" || data.ApplyState == "已办复") && data.IsFeed == "是" && data.SubMan.IndexOf("," + strUser + ",") >= 0)
            {
                return true;
            }
            return false;
        }
        //加载反馈表
        public void LoadFeed(int Id, DataUser uData, PlaceHolder _plFeedEdit, Literal _ltFeedOpNo, Literal _ltFeedOpOrg, Literal _ltFeedOpResult, Literal _ltFeedSummary, Literal _ltFeedInterview, Literal _ltFeedAttitude, Literal _ltFeedTakeWay, Literal _ltFeedPertinence, Literal _ltFeedLeaderReply, Literal _ltFeedResult, Literal _ltFeedBody, RadioButtonList _rblFeedInterview, RadioButtonList _rblFeedAttitude, RadioButtonList _rblFeedTakeWay, RadioButtonList _rblFeedPertinence, RadioButtonList _rblFeedLeaderReply, RadioButtonList _rblFeedResult, TextBox _txtFeedBody, Panel _plFeedCmd)
        {
            if(Id <= 0)
            {
                return;
            }
            DataOpinion[] oData = webOpinion.GetData(Id, "OpNo,Summary,ExamHostOrg,ExamHelpOrg,ResultInfo,ResultBody,SubMan, OpNo,ActiveName,ApplyState,IsFeed");
            if (oData != null && isFeed(oData[0], uData.TrueName))
            {
                _plFeedEdit.Visible = true;
                _ltFeedOpNo.Text = oData[0].OpNo;
                string strFeedOpOrg = PublicMod.GetOrgText(oData[0].ExamHostOrg);
                if (!string.IsNullOrEmpty(oData[0].ExamHelpOrg))
                {
                    strFeedOpOrg += "，" + PublicMod.GetOrgText(oData[0].ExamHelpOrg).Replace(",", "，");
                }
                _ltFeedOpOrg.Text = strFeedOpOrg;
                string strFeedOpResult = "<b>" + oData[0].ResultInfo + "</b>";
                if (!string.IsNullOrEmpty(oData[0].ResultBody))
                {
                    strFeedOpResult += "<hr/>" + oData[0].ResultBody.Replace("\n", "<br/>");
                }
                _ltFeedOpResult.Text = strFeedOpResult;
                _ltFeedSummary.Text = oData[0].Summary;
                WebOpinionFeed webFeed = new WebOpinionFeed();
                DataOpinionFeed[] data = webFeed.GetDatas(1, "提案", Id, uData.Id);
                if (data != null)
                {
                    _ltFeedInterview.Text = data[0].Interview;
                    _ltFeedAttitude.Text = data[0].Attitude;
                    _ltFeedResult.Text = data[0].Result;
                    _ltFeedTakeWay.Text = data[0].TakeWay;
                    _ltFeedPertinence.Text = data[0].Pertinence;
                    _rblFeedLeaderReply.Text = data[0].LeaderReply;
                    _ltFeedBody.Text = (!string.IsNullOrEmpty(data[0].Body)) ? data[0].Body : "无";
                }
                else
                {
                    WebOp webOp = new WebOp();
                    _rblFeedInterview.Visible = true;
                    PublicMod.LoadRadioButtonList(_rblFeedInterview, webOp, "走访情况");
                    _rblFeedAttitude.Visible = true;
                    PublicMod.LoadRadioButtonList(_rblFeedAttitude, webOp, "办理人员态度");
                    _rblFeedResult.Visible = true;
                    PublicMod.LoadRadioButtonList(_rblFeedResult, webOp, "是否同意办理结果");
                    _rblFeedTakeWay.Visible = true;
                    PublicMod.LoadRadioButtonList(_rblFeedTakeWay, webOp, "听取意见方式");
                    _rblFeedPertinence.Visible = true;
                    PublicMod.LoadRadioButtonList(_rblFeedPertinence, webOp, "答复是否针对提案");
                    _rblFeedLeaderReply.Visible = true;
                    //PublicMod.LoadRadioButtonList(_rblFeedLeaderReply, webOp, "分管领导答复");
                    _txtFeedBody.Visible = true;
                    _plFeedCmd.Visible = true;
                }
            }
            else
            {
                Response.Redirect("opinion.aspx?ac=my");
            }
        }
        //提交反馈
        protected void btnFeed_Click(object sender, EventArgs e)
        {
            ltInfo.Text = EditFeed(myUser, this, rblFeedInterview, rblFeedAttitude, rblFeedTakeWay, rblFeedPertinence, rblFeedResult, txtFeedBody);
        }
        public string EditFeed(DataUser uData, Page page, RadioButtonList _rblFeedInterview, RadioButtonList _rblFeedAttitude, RadioButtonList _rblFeedTakeWay, RadioButtonList _rblFeedPertinence, RadioButtonList _rblFeedResult, TextBox _txtFeedBody)
        {
            if (uData == null)
            {
                return "<script>$(function(){ alert('请重新登录！'); });</script>";
            }
            string strOut = "";
            string strBack = PublicMod.GetBackUrl();
            strBack = strBack.Replace("ac=feed", "ac=my");
            int OpId = (!string.IsNullOrEmpty(page.Request.QueryString["id"])) ? Convert.ToInt32(page.Request.QueryString["id"]) : 0;
            if (OpId <= 0)
            {
                return "<script>$(function(){ alert('错误！'); });</script>";
            }
            string strUser = HelperMain.SqlFilter(uData.TrueName, 20);
            DataOpinion[] oData = webOpinion.GetData(OpId, "IsFeed,SubMan,ApplyState,ResultInfo");//,ActiveName
            if (oData == null || !(oData[0].ApplyState == "已办复" || oData[0].ResultInfo == "解决或采纳" || oData[0].ResultInfo == "列入计划拟解决" || oData[0].ResultInfo == "留作参考") || oData[0].IsFeed != "是" || oData[0].SubMan.IndexOf("," + strUser + ",") < 0)
            {
                return "<script>$(function(){ alert('[提案反馈]错误！'); window.location.href='" + strBack + "'; });</script>";
            }
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            WebOpinionFeed webFeed = new WebOpinionFeed();
            DataOpinionFeed data = new DataOpinionFeed();
            //data.Id = 0;
            data.OpType = "提案";
            data.OpId = OpId;
            data.OrgId = 0;
            data.Interview = HelperMain.SqlFilter(_rblFeedInterview.SelectedValue.Trim(), 20);
            data.Attitude = HelperMain.SqlFilter(_rblFeedAttitude.SelectedValue.Trim(), 8);
            data.TakeWay = HelperMain.SqlFilter(_rblFeedTakeWay.SelectedValue.Trim(), 8);
            data.Pertinence = HelperMain.SqlFilter(_rblFeedPertinence.SelectedValue.Trim(), 8);
            //data.LeaderReply
            data.Result = HelperMain.SqlFilter(_rblFeedResult.SelectedValue.Trim(), 8);
            data.Body = HelperMain.SqlFilter(_txtFeedBody.Text.Trim());
            //data.Remark
            data.Active = 1;
            data.UserId = uData.Id;
            DataOpinionFeed[] ckData = webFeed.GetDatas(0, data.OpType, data.OpId, data.UserId, "Id");
            if (ckData != null)
            {
                data.Id = ckData[0].Id;
            }
            if (data.Id <= 0)
            {
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = strUser;
                data.Id = webFeed.Insert(data);
            }
            else
            {
                data.UpTime = dtNow;
                data.UpIp = strIp;
                data.UpUser = strUser;
            }
            if (data.Id > 0)
            {
                strOut = "<script>$(function(){ alert('“征询意见”已提交成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                strOut = "<script>$(function(){ alert('“征询意见”提交失败！'); window.history.back(-1); });</script>";
            }
            return strOut;
        }
        #endregion
        //
    }
}