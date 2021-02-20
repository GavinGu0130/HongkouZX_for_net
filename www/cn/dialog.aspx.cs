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
    public partial class dialog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            meta1.Client = HelperMain.GetClient("no");
            if (!IsPostBack)
            {
                hfObj.Value = Request.QueryString["obj"];
                switch (Request.QueryString["ac"])
                {
                    case "subman":
                        plSelUser.Visible = true;
                        listSelUser();
                        break;
                    case "score":
                        plScore.Visible = true;
                        listScore();
                        break;
                    case "speak":
                        plSpeak.Visible = true;
                        listSpeak();
                        break;
                    case "perform":
                        plPerform.Visible = true;
                        listPerform();
                        break;
                    case "opinion":
                        plOpinion.Visible = true;
                        listOpinion();
                        break;
                    case "pop":
                        plPop.Visible = true;
                        listPop();
                        break;
                    case "sign":
                        plOpSign.Visible = true;
                        loadOpSign();
                        break;
                    case "msg":
                        plMsg.Visible = true;
                        listMsg();
                        break;
                    default:
                        break;
                }
            }
        }
        //
        #region 选择委员
        //初始化
        private void listSelUser()
        {
            WebOp webOp = new WebOp();
            if (Request.QueryString["type"] == "all")
            {
                PublicMod.LoadDropDownList(ddlQSelUserType, webOp, "用户类别");
            }
            else
            {
                //ddlQSelUserType.Visible = false;
            }
            //PublicMod.LoadDropDownList(ddlQSelPeriod, webOp, "_届");
            PublicMod.LoadDropDownList(ddlQSelCommittee, webOp, "专委会");
            PublicMod.LoadDropDownList(ddlQSelSubsector, webOp, "界别");
            PublicMod.LoadDropDownList(ddlQSelStreetTeam, webOp, "街道活动组");
            PublicMod.LoadDropDownList(ddlQSelParty, webOp, "政治面貌");
            PublicMod.LoadCheckBoxList(cblQSelRole, webOp, "政协职务");
            querySelUser(new DataUser());
        }
        //查询委员
        protected void btnQSelUser_Click(object sender, EventArgs e)
        {
            DataUser data = new DataUser();
            if (!string.IsNullOrEmpty(txtQSelUser.Text))
            {
                data.TrueName = "%" + HelperMain.SqlFilter(txtQSelUser.Text.Trim(), 20) + "%";
            }
            //data.Period = HelperMain.SqlFilter(ddlQSelPeriod.SelectedValue.Trim(), 4);
            data.Committee = HelperMain.SqlFilter(ddlQSelCommittee.SelectedValue.Trim(), 20);
            if (!string.IsNullOrEmpty(data.Committee))
            {
                data.Committee += "%";
            }
            data.Subsector = HelperMain.SqlFilter(ddlQSelSubsector.SelectedValue.Trim(), 20);
            data.StreetTeam = HelperMain.SqlFilter(ddlQSelStreetTeam.SelectedValue.Trim(), 20);
            data.Party = "%" + HelperMain.SqlFilter(ddlQSelParty.SelectedValue.Trim(), 10) + "%";
            data.Role = HelperMain.SqlFilter(HelperMain.GetCheckSelected(cblQSelRole));
            querySelUser(data);
        }
        private void querySelUser(DataUser qUser)
        {
            qUser.ActiveName = ">0";
            qUser.Period = config.PERIOD;
            qUser.UserType = (ddlQSelUserType.Visible) ? HelperMain.SqlFilter(ddlQSelUserType.SelectedValue.Trim(), 20) : "在册委员";
            if (qUser.UserType == "在册委员")
            {
                qUser.UserType = "委员";
                qUser.ActiveName = ">0";
                qUser.UserCode = "14%";
            }
            WebUser webUser = new WebUser();
            DataUser[] data = webUser.GetDatas(qUser, "UserCode,TrueName", 1, 0);
            if (data != null)
            {
                rpListUser.DataSource = data;
                rpListUser.DataBind();
            }
        }
        #endregion
        //
        #region 记录
        //积分记录
        private void listScore()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["Titler"]))
            {
                string Title = Request.QueryString["Titler"];
                Header.Title = Title;
                header2.Title = Title;
                header2.Visible = true;
            }
            string strTableName = Request.QueryString["TableName"];
            string strTitle = "";
            switch (strTableName)
            {
                case "tb_Perform_Feed":
                    //strTableName = "tb_Perform_Feed";
                    strTitle = "出席-" + Request.QueryString["Title"] + "%";
                    if (strTitle.IndexOf("|") > 0)
                    {
                        strTitle = strTitle.Replace("|", "、");
                    }
                    break;
                case "tb_Perform_Speak":
                    strTableName = "tb_Perform_Feed";
                    strTitle = (!string.IsNullOrEmpty(Request.QueryString["Title"])) ? Request.QueryString["Title"] : "%发言%";
                    if (strTitle.IndexOf("|") > 0)
                    {
                        strTitle = strTitle.Replace("|", "、");
                    }
                    break;
                case "tb_Perform_Res":
                    strTableName = "tb_Perform_Feed";
                    strTitle = "提供资源%";
                    break;
                case "tb_Perform_De":
                    strTableName = "tb_Perform_Feed";
                    strTitle = "%未出席|必须参加的会议-同意请假";
                    break;
                default:
                    break;
            }
            WebUserScore webScore = new WebUserScore();
            WebUser webUser = new WebUser();
            string strUserId = Request.QueryString["UserId"];
            if (!string.IsNullOrEmpty(Request.QueryString["UserType"]))
            {
                DataUser qUser = new DataUser();
                qUser.ActiveName = ">0";
                qUser.Period = config.PERIOD;
                switch (Request.QueryString["UserType"])
                {
                    case "专委会":
                        qUser.Committee = strUserId;
                        break;
                    case "界别":
                        qUser.Subsector = strUserId;
                        break;
                    case "街道活动组":
                        qUser.StreetTeam = strUserId;
                        break;
                    case "党派团体":
                        if (strUserId == "工商联" || strUserId == "侨联")
                        {
                            qUser.Subsector = strUserId;
                        }
                        else
                        {
                            qUser.Party = strUserId;
                        }
                        break;
                    default:
                        return;
                }
                DataUser[] userData = webUser.GetDatas(qUser, "Id");
                if (userData != null)
                {
                    strUserId = "";
                    for (int j = 0; j < userData.Count(); j++)
                    {
                        if (!string.IsNullOrEmpty(strUserId))
                        {
                            strUserId += ",";
                        }
                        strUserId += userData[j].Id.ToString();
                    }
                }
            }
            if (string.IsNullOrEmpty(strUserId))
            {
                return;
            }

            DateTime dtNow = DateTime.Now;
            string strGetTimeText = "";
            if (!string.IsNullOrEmpty(Request.QueryString["time"]))
            {
                strGetTimeText = Request.QueryString["time"];
            }
            else
            {
                strGetTimeText = string.Format("{0:yyyy}-01-01,{0:yyyy}-12-31", dtNow);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["view"]))
            {
                plYear.Visible = true;
                int UserId = 0;
                int nowYear = dtNow.Year;
                int addYear = 0;
                if (strUserId.IndexOf(",") < 0)
                {
                    UserId = Convert.ToInt32(strUserId);
                    DataUser[] userData = webUser.GetData(UserId, "AddTime");
                    if (userData != null)
                    {
                        addYear = userData[0].AddTime.Year;
                    }
                }
                for (int i = addYear; i <= nowYear; i++)
                {
                    ListItem item = new ListItem(string.Format("{0}年度", i), string.Format("{0}-01-01,{0}-12-31", i));
                    ddlYear.Items.Add(item);
                }
                HelperMain.SetDownSelected(ddlYear, strGetTimeText);
                string strStart = strGetTimeText.Substring(0, strGetTimeText.IndexOf(","));
                string strEnd = strGetTimeText.Substring(strGetTimeText.IndexOf(",") + 1);
                decimal deScoreTotal = webScore.GetTotalScore(UserId, strStart, strEnd);//累计积分
                decimal deScore2 = webScore.GetTotalScore(UserId, strStart, strEnd, "tb_Opinion,tb_Opinion_Pop");//建言得分
                ltScore.Text = deScoreTotal.ToString("n2");
                ltScore1.Text = (deScoreTotal - deScore2).ToString("n2");
                ltScore2.Text = deScore2.ToString("n2");
            }
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            string strOrder = "GetTime DESC";
            DataUserScore[] data = webScore.GetDatas(1, strUserId, strTableName, 0, strTitle, strGetTimeText, "Id,UserId,Title,Score,TableId,TableName,Remark", pageCur, pageSize, strOrder, "total");//查询委员活动项积分列表
            if (data != null)
            {
                WebOpinion webOpin = new WebOpinion();
                WebOpinionPop webPop = new WebOpinionPop();
                WebReport webReport = new WebReport();
                WebPerform webPerform = new WebPerform();
                WebPerformFeed webFeed = new WebPerformFeed();
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    data[i].other = PublicMod.GetScoreOther(data[i], webOpin, webPop, webReport, webPerform, webFeed);
                    if (!string.IsNullOrEmpty(data[i].Remark))
                    {
                        if (!string.IsNullOrEmpty(data[i].other))
                        {
                            data[i].other += "<br/>";
                        }
                        data[i].other += data[i].Remark;
                    }
                    data[i].ScoreText = data[i].Score.ToString("n2");
                    if (!string.IsNullOrEmpty(Request.QueryString["UserType"]))
                    {
                        DataUser[] userData = webUser.GetData(data[i].UserId, "TrueName");
                        if (userData != null)
                        {
                            data[i].Title = "(" + userData[0].TrueName + ") " + data[i].Title;
                        }
                    }
                }
                rpScoreList.DataSource = data;
                rpScoreList.DataBind();
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = Request.Url.ToString();
                lblScoreNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
            }
        }
        //发言记录
        private void listSpeak()
        {
            WebPerformFeed webFeed = new WebPerformFeed();
            string Committee = HelperMain.SqlFilter(Request.QueryString["Committee"].Trim(), 20);
            string Subsector = HelperMain.SqlFilter(Request.QueryString["Subsector"].Trim(), 20);
            string strSignTimeText = (!string.IsNullOrEmpty(Request.QueryString["time"])) ? HelperMain.SqlFilter(Request.QueryString["time"].Trim(), 21) : string.Format("{0:yyyy}-01-01,{0:yyyy}-12-31", DateTime.Today);
            DataPerformFeed[] data = webFeed.GetSpeaks(Committee, Subsector, strSignTimeText, "f.PerformId, f.SignMan, f.SignManSpeak");
            if (data != null)
            {
                WebPerform webPerform = new WebPerform();
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = i + 1;
                    DataPerform[] pData = webPerform.GetData(data[i].PerformId, "SubType,Title");
                    if (pData != null)
                    {
                        data[i].PerformSubType = pData[0].SubType;
                        data[i].PerformTitle = pData[0].Title;
                    }
                }
                rpSpeakList.DataSource = data;
                rpSpeakList.DataBind();
            }
        }
        //会议/活动记录
        private void listPerform()
        {
            WebPerform webPerform = new WebPerform();
            DataPerform qData = new DataPerform();
            if (!string.IsNullOrEmpty(Request.QueryString["ActiveName"]))
            {
                qData.ActiveName = HelperMain.SqlFilter(Request.QueryString["ActiveName"].Trim(), 20);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["OrgName"]))
            {
                qData.OrgName = HelperMain.SqlFilter(Request.QueryString["OrgName"].Trim(), 20);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["SubType"]))
            {
                qData.SubType = HelperMain.SqlFilter(Request.QueryString["SubType"].Trim(), 100);
            }
            qData.StartTimeText = (!string.IsNullOrEmpty(Request.QueryString["time"])) ? HelperMain.SqlFilter(Request.QueryString["time"].Trim(), 21) : string.Format("{0:yyyy}-01-01,{0:yyyy}-12-31", DateTime.Today);
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            string strOrder = "";
            DataPerform[] data = webPerform.GetDatas(qData, "", pageCur, pageSize, strOrder, "total");
            if (data != null)
            {
                WebPerformFeed webFeed = new WebPerformFeed();
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    DataPerformFeed[] fData = webFeed.GetDatas("", data[i].Id, 0, "", "Id");
                    if (fData != null)
                    {
                        data[i].FeedNum = fData.Count();
                    }
                }
                rpPerformList.DataSource = data;
                rpPerformList.DataBind();
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = Request.Url.ToString();
                lblPerformNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
            }
        }
        //提案记录
        private void listOpinion()
        {
            WebOpinion webOpinion = new WebOpinion();
            DataOpinion qData = new DataOpinion();
            if (!string.IsNullOrEmpty(Request.QueryString["ActiveName"]))
            {
                qData.ActiveName = HelperMain.SqlFilter(Request.QueryString["ActiveName"].Trim(), 20);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Party"]))
            {
                qData.Party = HelperMain.SqlFilter(Request.QueryString["Party"].Trim(), 20);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Committee"]))
            {
                qData.Committee = HelperMain.SqlFilter(Request.QueryString["Committee"].Trim(), 20);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Subsector"]))
            {
                qData.Subsector = HelperMain.SqlFilter(Request.QueryString["Subsector"].Trim(), 20);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["StreetTeam"]))
            {
                qData.StreetTeam = HelperMain.SqlFilter(Request.QueryString["StreetTeam"].Trim(), 20);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["SubMan"]))
            {
                qData.SubMan = HelperMain.SqlFilter(Request.QueryString["SubMan"].Trim());
                qData.IsSubMan1 = true;
            }
            qData.SubTimeText = (!string.IsNullOrEmpty(Request.QueryString["time"])) ? HelperMain.SqlFilter(Request.QueryString["time"].Trim(), 21) : string.Format("{0}-12-01,{1:yyyy}-11-30", DateTime.Today.Year - 1, DateTime.Today);
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            string strOrder = "";
            DataOpinion[] data = webOpinion.GetDatas(qData, "", pageCur, pageSize, strOrder, "total");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    //if (data[i].SubManType == "委员")
                    //{

                    //}
                    //else
                    //{

                    //}
                    data[i].SubMan = data[i].SubMan.Trim(',');
                }
                rpOpinionList.DataSource = data;
                rpOpinionList.DataBind();
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = Request.Url.ToString();
                lblOpinionNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
            }
        }
        //社情民意记录
        private void listPop()
        {
            WebOpinionPop webPop = new WebOpinionPop();
            DataOpinionPop qData = new DataOpinionPop();
            if (!string.IsNullOrEmpty(Request.QueryString["ActiveName"]))
            {
                qData.ActiveName = HelperMain.SqlFilter(Request.QueryString["ActiveName"].Trim(), 20);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Party"]))
            {
                qData.Party = HelperMain.SqlFilter(Request.QueryString["Party"].Trim(), 20);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Committee"]))
            {
                qData.Committee = HelperMain.SqlFilter(Request.QueryString["Committee"].Trim(), 20);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Subsector"]))
            {
                qData.Subsector = HelperMain.SqlFilter(Request.QueryString["Subsector"].Trim(), 20);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["StreetTeam"]))
            {
                qData.StreetTeam = HelperMain.SqlFilter(Request.QueryString["StreetTeam"].Trim(), 20);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["SubMan1"]))
            {
                qData.SubMan1 = HelperMain.SqlFilter(Request.QueryString["SubMan1"].Trim());
            }
            qData.SubTimeText = (!string.IsNullOrEmpty(Request.QueryString["time"])) ? HelperMain.SqlFilter(Request.QueryString["time"].Trim(), 21) : string.Format("{0:yyyy}-01-01,{0:yyyy}-12-31", DateTime.Today);
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            string strOrder = "";
            DataOpinionPop[] data = webPop.GetDatas(qData, "", pageCur, pageSize, strOrder, "total");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    if (data[i].SubManType == "委员")
                    {

                    }
                    else
                    {
                        data[i].SubMan = data[i].Linkman;
                    }
                }
                rpPopList.DataSource = data;
                rpPopList.DataBind();
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = Request.Url.ToString();
                lblPopNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
            }
        }
        #endregion
        //
        #region 会签
        //加载信息
        private void loadOpSign()
        {
            int intId = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
            if (intId > 0)
            {
                WebOpinionSign webSign = new WebOpinionSign();
                DataOpinionSign[] data = webSign.GetData(intId);
                if (data != null)
                {
                    txtSignId.Text = data[0].Id.ToString();
                    hfSignOpId.Value = data[0].OpId.ToString();
                    hfSignUserId.Value = data[0].UserId.ToString();
                    txtSignUser.Text = data[0].SignUser;
                    txtSignOverdue.Text = data[0].Overdue.ToString("yyyy-MM-dd HH:mm");
                    if (data[0].SignTime > DateTime.MinValue)
                    {
                        txtSignTime.Text = data[0].SignTime.ToString("yyyy-MM-dd HH:mm");
                    }
                    txtSignIp.Text = data[0].SignIp;
                    txtSignBody.Text = data[0].Body;
                    HelperMain.SetRadioSelected(rblSignActive, data[0].Active.ToString());
                }
                else
                {
                    btnSign.Visible = false;
                }
            }
        }
        //修改会签
        protected void btnSign_Click(object sender, EventArgs e)
        {
            DataAdmin myUser = HelperAdmin.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
            {
                //ltInfo.Text = "<script>$(function(){ $('#dialog', window.parent.document).remove(); });</script>";
                ltInfo.Text = "<script>$(function(){ alert('请重新登录！');parent.location.reload(); });</script>";
                return;
            }
            int intId = (!string.IsNullOrEmpty(txtSignId.Text)) ? Convert.ToInt32(txtSignId.Text) : 0;
            if (intId <= 0 || string.IsNullOrEmpty(txtSignOverdue.Text))
            {
                ltInfo.Text = "<script>$(function(){ alert('修改出错了！'); window.history.back(-1); });</script>";
                return;
            }
            DateTime dtOverdue = Convert.ToDateTime(txtSignOverdue.Text);
            int intActive = Convert.ToInt16(rblSignActive.SelectedValue);
            WebOpinionSign webSign = new WebOpinionSign();
            if (webSign.UpdateOverdue(intId, dtOverdue, intActive) > 0)
            {
                ltInfo.Text = "<script>$(function(){ $('#s" + intId.ToString() + "', window.parent.document).text('" + txtSignOverdue.Text + "'); alert('“修改会签”成功！'); $('#dialog', window.parent.document).remove(); });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“修改会签”失败！'); window.history.back(-1); });</script>";
            }
        }
        #endregion
        //
        #region 发送的消息
        private void listMsg()
        {
            string TableName = mod.main.HelperMain.SqlFilter(Request.QueryString["TableName"], 20);
            int TableId = (!string.IsNullOrEmpty(Request.QueryString["TableId"])) ? Convert.ToInt32(Request.QueryString["TableId"]) : 0;
            int UserId = (!string.IsNullOrEmpty(Request.QueryString["UserId"])) ? Convert.ToInt32(Request.QueryString["UserId"]) : 0;
            if (TableId <= 0 || UserId <= 0)
            {
                return;
            }
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            string strOrder = "AddTime DESC";
            WebSendMsg webMsg = new WebSendMsg();
            DataSendMsg[] data = webMsg.GetDatas("", TableName, TableId, UserId, "", "", pageCur, pageSize, strOrder, "total");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    string strRemark = data[i].Remark;
                    if ((data[i].Label == "sms" || data[i].Label == "smstp") && !string.IsNullOrEmpty(strRemark) && strRemark.IndexOf("\"code\":200") > 0 && data[i].AddTime.AddDays(30) > DateTime.Now)
                    {
                        if (strRemark.IndexOf("\"mobile\":\"") > 0)
                        {
                            string strQuery = strRemark.Substring(strRemark.IndexOf("\n")).Trim();
                            if (strQuery.IndexOf("\"smsStatus\":1,") > 0)
                            {
                                data[i].other = "发送成功！";
                            }
                            else if (strQuery.IndexOf("\"smsStatus\":2,") > 0)
                            {
                                data[i].other = "发送失败！";
                            }
                            else if (strQuery.IndexOf("\"smsStatus\":3,") > 0)
                            {
                                data[i].other = "待返回！";
                            }
                            data[i].other += "<p>" + strQuery + "</p>";
                            data[i].Remark = strRemark.Substring(0, strRemark.IndexOf("\n"));
                        }
                        else
                        {
                            string strMobile = data[i].Body;
                            if (strMobile.IndexOf("\n") > 0)
                            {
                                strMobile = strMobile.Substring(0, strMobile.IndexOf("\n")).Trim();
                            }
                            if (strMobile.Length == 11 && strMobile.StartsWith("1"))
                            {
                                string strMsgId = strRemark;
                                strMsgId = strMsgId.Substring(strMsgId.IndexOf("\"msgId\":\"") + 9);
                                strMsgId = strMsgId.Substring(0, strMsgId.IndexOf("\""));
                                string strQuery = mod.main.HelperSms.Query(config.SMSUSER, config.SMSPWD, strMsgId, strMobile);//strMsgId;//
                                if (strQuery.IndexOf("\"smsStatus\":1,") > 0)
                                {
                                    data[i].other = "发送成功！";
                                }
                                else if (strQuery.IndexOf("\"smsStatus\":2,") > 0)
                                {
                                    data[i].other = "发送失败！";
                                }
                                else if (strQuery.IndexOf("\"smsStatus\":3,") > 0)
                                {
                                    data[i].other = "待返回！";
                                }
                                data[i].other += "<p>" + strQuery + "</p>";
                                webMsg.UpdateRemark(data[i].Id, strRemark + "\n" + strQuery);
                            }
                            //else
                            //{
                            //    data[i].other = strMobile;
                            //}
                        }
                    }
                }
                rpMsgList.DataSource = data;
                rpMsgList.DataBind();
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = Request.Url.ToString();
                lblMsgNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
            }
        }
        #endregion
        //
    }
}