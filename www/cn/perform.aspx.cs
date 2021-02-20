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
    public partial class perform : System.Web.UI.Page
    {
        private DataUser myUser = null;
        private WebPerform webPerform = new WebPerform();
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
            if (myUser.UserType != "" && myUser.UserType != "委员")
            {
                LoadNav(myUser.TrueName, plNav, ltSaveNum);
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
                        SubData(Id, myUser, txtActiveName, hfOrg, ddlOrgType, hfSubType, ddlSubType, rblHaveBus, cblHaveDinner, txtOrgName, txtSubType, txtIsMust, txtLinkman, txtLinkmanTel, txtTitle, txtPerformSite, txtStartTime, txtEndTime, txtOverTime, txtSignTime, txtLeaders, txtAttendees, txtBody, hfFiles, plVerify, ltVerifyInfo, btnDel);
                    }
                    break;
                case "my":
                    strTitle = "我的会议/活动申请";
                    plMy.Visible = true;
                    MyList("<>'删除'", myUser, rpMyList, ltMyNo, lblMyNav, ltMyTotal, this);//我的
                    break;
                case "save":
                    strTitle = "暂存的会议/活动申请";
                    plSave.Visible = true;
                    MyList("暂存,退回", myUser, rpSaveList, ltSaveNo, lblSaveNav, null, this);//暂存
                    break;
                default:
                    strTitle = "会议/活动通知";
                    if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                    {
                        plView.Visible = true;
                        strTitle += " 详情";
                        LoadData(Convert.ToInt32(Request.QueryString["id"]), myUser, this, ltOrgName, ltLinkman, ltLinkmanTel, ltIsMust, ltSubType, ltTitle, ltStartTime, ltEndTime, ltOverTime, ltSignTime, ltPerformSite, ltHaveBus, ltHaveDinner, ltLeaders, ltAttendees, ltBody, ltFiles, ltActive, plFeed, ltFeed, btnAttend, btnNonAttend, btnLeave, plPlayFeed, rpFeedList, ltFeedNo, ltFeedTotal);
                    }
                    else
                    {
                        plList.Visible = true;
                        MyPerform(myUser, rpQueryList, ltQueryNo, lblQueryNav, ltQueryTotal, this);
                    }
                    break;
            }
            Header.Title += " - " + strTitle;
        }
        //页面nav
        public void LoadNav(string strUser, Panel plNav2, Literal ltSaveNum2)
        {
            plNav2.Visible = true;
            DataPerform qData = new DataPerform();
            qData.ActiveName = "暂存,退回";
            qData.AddUser = strUser;
            DataPerform[] data = webPerform.GetDatas(qData, "Id");
            if (data != null)
            {
                ltSaveNum2.Text = (data.Count() > 99) ? "99+" : data.Count().ToString();
            }
        }
        //
        #region 查询
        //我的履职
        public void MyPerform(DataUser uData, Repeater rpList, Literal ltNo = null, Label lblNav = null, Literal ltTotal = null, Page page = null)
        {
            DataPerform qData = new DataPerform();
            qData.ActiveName = "发布,履职关闭";
            qData.Attendees = uData.TrueName;
            qData.EndTimeText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",";
            listData(uData, qData, rpList, ltNo, lblNav, ltTotal, page);
        }
        //加载我的列表
        public void MyList(string ActiveName, DataUser uData, Repeater rpList, Literal ltNo = null, Label lblNav = null, Literal ltTotal = null, Page page = null)
        {
            DataPerform qData = new DataPerform();
            qData.ActiveName = ActiveName;
            qData.AddUser = uData.TrueName;
            string strOrder = "StartTime DESC, EndTime DESC, OverTime DESC";
            listData(uData, qData, rpList, ltNo, lblNav, ltTotal, page, strOrder);
        }
        //加载列表
        private void listData(DataUser uData, DataPerform qData, Repeater rpList, Literal ltNo = null, Label lblNav = null, Literal ltTotal = null, Page page = null, string strOrder = "")
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
            DataPerform[] data = webPerform.GetDatas(qData, "Id,OrgName,IsMust,SubType,Title,OverTime,StartTime,EndTime,PerformSite,ActiveName,VerifyInfo", pageCur, pageSize, strOrder, "total");
            if (data != null)
            {
                WebPerformFeed webFeed = new WebPerformFeed();
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    if (!string.IsNullOrEmpty(data[i].SubType))
                    {
                        data[i].SubType = data[i].SubType.Replace(",", "<br/>");
                    }
                    if (!string.IsNullOrEmpty(data[i].OrgName))
                    {
                        string[] arr = data[i].OrgName.Split(',');
                        for (int j = 0; j < arr.Count(); j++)
                        {
                            if (arr[j].IndexOf("-") > 0)
                            {
                                arr[j] = arr[j].Substring(arr[j].IndexOf("-") + 1);
                            }
                        }
                        data[i].OrgName = string.Join("<br/>", arr);
                    }
                    if (data[i].OverTime > DateTime.MinValue)
                    {
                        data[i].OverTimeText = data[i].OverTime.ToString("yyyy-MM-dd");
                    }
                    if (!string.IsNullOrEmpty(data[i].Files))
                    {
                        data[i].Files = string.Format("<a href='{0}' target='_blank'><u>附件下载</u></a>", data[i].Files);
                    }
                    else
                    {
                        data[i].Files = "<br/>";
                    }
                    data[i].PerformTimeText = PublicMod.GetTimeText(data[i].StartTime, data[i].EndTime);
                    switch (data[i].ActiveName)
                    {
                        case "提交申请":
                            if (data[i].EndTime < DateTime.Now)
                            {
                                data[i].rowClass = " class='del' title='申请已过期'";
                                data[i].ActiveName += "<br/>已过期";
                            }
                            else
                            {
                                data[i].rowClass = " class='wait' title='提交申请'";
                            }
                            data[i].StateName = "查看";
                            break;
                        case "暂存":
                            data[i].rowClass = " class='save' title='暂存'";
                            data[i].StateName = "修改";
                            break;
                        case "退回":
                            string strBack = (!string.IsNullOrEmpty(data[i].VerifyInfo)) ? strBack = " [" + data[i].VerifyInfo + "]" : "";
                            data[i].rowClass = " class='wait' title='退回" + strBack + "'";
                            data[i].StateName = "修改";
                            break;
                        case "删除":
                            data[i].rowClass = " class='del' title='删除'";
                            data[i].StateName = "查看";
                            break;
                        case "履职关闭":
                            data[i].rowClass = " class='cancel' title='履职关闭'";
                            data[i].StateName = "查看";
                            break;
                        default:
                            string strActiveName = "";
                            if (uData.UserType == "委员")
                            {
                                DataPerformFeed qFdata = new DataPerformFeed();
                                qFdata.PerformId = data[i].Id;
                                qFdata.UserId = uData.Id;
                                DataPerformFeed[] fData = webFeed.GetDatas(qFdata, "IsMust,ActiveName");
                                if (fData != null)
                                {
                                    //if (!string.IsNullOrEmpty(fData[0].IsMust))
                                    //{
                                    //    strActiveName = fData[0].IsMust + "<br/>";
                                    //}
                                    strActiveName += fData[0].ActiveName;
                                }
                            }
                            else
                            {
                                strActiveName = data[i].ActiveName;
                            }
                            if (data[i].EndTime < DateTime.Now)
                            {
                                data[i].rowClass = " class='del' title='会议/活动已结束'";
                                data[i].ActiveName = "已结束<br/>" + strActiveName;
                            }
                            else if (data[i].StartTime < DateTime.Now)
                            {
                                data[i].rowClass = " class='cancel' title='会议/活动进行中'";
                                data[i].ActiveName = "进行中<br/>" + strActiveName;
                            }
                            else if (data[i].OverTime < DateTime.Now)
                            {
                                data[i].rowClass = " class='cancel' title='会议/活动已报名截止'";
                                data[i].ActiveName = "报名截止<br/>" + strActiveName;
                            }
                            else
                            {
                                data[i].ActiveName = strActiveName;
                                //data[i].ActiveName = "正常";
                            }
                            data[i].StateName = "查看";
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
        //加载信息
        public void LoadData(int Id, DataUser uData, Page page, Literal ltOrgName2, Literal ltLinkman2, Literal ltLinkmanTel2, Literal ltIsMust2, Literal ltSubType2, Literal ltTitle2, Literal ltStartTime2, Literal ltEndTime2, Literal ltOverTime2, Literal ltSignTime2, Literal ltPerformSite2, Literal ltHaveBus2, Literal ltHaveDinner2, Literal ltLeaders2, Literal ltAttendees2, Literal ltBody2, Literal ltFiles2, Literal ltActive2, PlaceHolder plFeed2, Literal ltFeed2, Button btnAttend2, Button btnNonAttend2, Button btnLeave2, Panel _plPlayFeed = null, Repeater _rpFeedList = null, Literal _ltFeedNo = null, Literal _ltFeedTotal = null)
        {
            if (Id <= 0)
            {
                page.Response.Redirect("perform.aspx");
                return;
            }
            DataPerform[] data = webPerform.GetData(Id);
            if (data == null)
            {
                page.Response.Redirect("perform.aspx");
                return;
            }
            else if (((data[0].ActiveName == "发布" || data[0].ActiveName == "履职关闭") && data[0].Attendees.IndexOf(uData.TrueName) >= 0) || ((data[0].ActiveName == "提交申请" || data[0].ActiveName == "发布" || data[0].ActiveName == "履职关闭") && data[0].AddUser == uData.TrueName))
            {
                ltOrgName2.Text = data[0].OrgName;
                ltLinkman2.Text = data[0].Linkman;
                ltLinkmanTel2.Text = data[0].LinkmanTel;
                ltIsMust2.Text = data[0].IsMust;
                ltSubType2.Text = data[0].SubType;
                ltTitle2.Text = data[0].Title;
                ltStartTime2.Text = data[0].StartTime.ToString("yyyy年M月d日 HH:mm");
                ltEndTime2.Text = data[0].EndTime.ToString("yyyy年M月d日 HH:mm");
                ltOverTime2.Text = (data[0].OverTime > DateTime.MinValue) ? data[0].OverTime.ToString("yyyy年M月d日 HH:mm") : ltStartTime2.Text;
                ltSignTime2.Text = (data[0].SignTime > DateTime.MinValue) ? data[0].SignTime.ToString("yyyy年M月d日 HH:mm") : ltStartTime2.Text;
                ltPerformSite2.Text = data[0].PerformSite;
                ltHaveBus2.Text = data[0].HaveBus;
                if (!string.IsNullOrEmpty(data[0].HaveDinner))
                {
                    ltHaveDinner2.Text = data[0].HaveDinner.Replace(",", "、");
                }
                ltLeaders2.Text = data[0].Leaders.Trim(',');
                ltAttendees2.Text = data[0].Attendees.Trim(',');
                ltBody2.Text = data[0].Body.Replace("\n", "<br/>");
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
                if (data[0].ActiveName == "发布")
                {
                    if (data[0].Attendees.IndexOf("," + uData.TrueName + ",") >= 0)
                    {
                        plFeed2.Visible = true;
                        int LeaveNum = 0;
                        bool blCmd = true;
                        WebPerformFeed webFeed = new WebPerformFeed();
                        DataPerformFeed[] fData = webFeed.GetDatas("", data[0].Id, uData.Id, uData.TrueName, "LeaveNum,VerifyInfo,IsMust,ActiveName", 0, 1);
                        if (fData != null)
                        {
                            LeaveNum = fData[0].LeaveNum;
                            if (!string.IsNullOrEmpty(fData[0].IsMust))
                            {
                                ltFeed2.Text = "(" + fData[0].IsMust + ") ";
                            }
                            ltFeed2.Text += fData[0].ActiveName;
                            if (fData[0].ActiveName.IndexOf("已签到") >= 0 || fData[0].ActiveName == "已出席")
                            {
                                blCmd = false;
                            }
                            else if (!string.IsNullOrEmpty(fData[0].VerifyInfo))
                            {
                                ltFeed2.Text += " [" + fData[0].VerifyInfo + "]";
                            }
                        }
                        if (blCmd && data[0].OverTime > DateTime.Now)
                        {
                            btnAttend2.Visible = true;
                            if (data[0].IsMust.IndexOf("必须") >= 0)
                            {
                                if (LeaveNum < 2 && ltFeed2.Text.IndexOf("请假申请") < 0)
                                {
                                    btnLeave2.Visible = true;
                                }
                            }
                            else
                            {
                                btnNonAttend2.Visible = true;
                            }
                        }
                    }
                    if (data[0].EndTime < DateTime.Now)
                    {
                        ltActive2.Text = "已结束";
                    }
                    else if (data[0].StartTime < DateTime.Now)
                    {
                        ltActive2.Text = "进行中";
                    }
                    else if (data[0].OverTime < DateTime.Now)
                    {
                        ltActive2.Text = "报名截止";
                    }
                    else
                    {
                        ltActive2.Text = data[0].ActiveName;
                    }
                }
                else
                {
                    ltActive2.Text = data[0].ActiveName;//"履职关闭";
                }
                if (_plPlayFeed != null && data[0].AddUser == uData.TrueName && data[0].ActiveName != "提交申请")
                {
                    listFeed(Id, _plPlayFeed, _rpFeedList, _ltFeedNo, _ltFeedTotal);
                }
            }
            else
            {
                page.Response.Redirect("perform.aspx");
            }
        }
        //加载反馈列表
        private void listFeed(int PerformId, Panel _plPlayFeed, Repeater _rpFeedList, Literal _ltFeedNo, Literal _ltFeedTotal)
        {
            DataPerformFeed qData = new DataPerformFeed();
            qData.PerformId = PerformId;
            WebPerformFeed webFeed = new WebPerformFeed();
            DataPerformFeed[] data = webFeed.GetDatas(qData, "", 1, 0, "SignTime ASC, AddTime ASC");
            if (data != null)
            {
                _plPlayFeed.Visible = true;
                WebUser webUser = new WebUser();
                WebUserWx webUserWx = new WebUserWx();
                WebSendMsg webSendMsg = new WebSendMsg();
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = i + 1;
                    switch (data[i].ActiveName)
                    {
                        case "取消":
                            data[i].rowClass = " class='del' title='取消'";
                            break;
                        case "不参加":
                            data[i].rowClass = " class='cancel' title='不参加'";
                            break;
                        case "不同意请假":
                        case "同意请假":
                        case "请假":
                            data[i].rowClass = " class='save' title='" + data[i].ActiveName + "'";
                            break;
                        default:
                            break;
                    }
                    DataUser[] uData = webUser.GetData(data[i].UserId, "UserCode,OrgName,Role");
                    if (uData != null)
                    {
                        data[i].SignManCode = uData[0].UserCode;
                        data[i].SignManOrg = uData[0].OrgName;
                        if (!string.IsNullOrEmpty(uData[0].Role) && uData[0].Role.IndexOf("常委") >= 0)
                        {
                            data[i].SignManType = "常委";
                        }
                    }
                    if (!string.IsNullOrEmpty(data[i].IsMust))
                    {
                        data[i].IsMust += "<br/>";
                    }
                    if (string.IsNullOrEmpty(data[i].ActiveName) || data[i].ActiveName == "待确认")
                    {

                    }
                    else if (!string.IsNullOrEmpty(data[i].UpIp) || !string.IsNullOrEmpty(data[i].UpUser))
                    {
                        data[i].FeedTimeText = data[i].UpTime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    //else
                    //{
                    //    data[i].FeedTimeText = data[i].AddTime.ToString("yyyy-MM-dd HH:mm:ss");
                    //}
                    //if (!string.IsNullOrEmpty(data[i].VerifyIp) || !string.IsNullOrEmpty(data[i].VerifyUser))
                    //{
                    //    data[i].VerifyTimeText = data[i].VerifyTime.ToString("yyyy-MM-dd HH:mm:ss");
                    //}
                    if (data[i].SignTime > DateTime.MinValue)//!string.IsNullOrEmpty(data[i].SignIp)
                    {
                        data[i].SignTimeText = data[i].SignTime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (data[i].UserId > 0)
                    {
                        DataUserWx[] wxData = webUserWx.GetDatas(config.APPID, data[i].UserId, "", "WxOpenId");
                        if (wxData != null)
                        {
                            data[i].SignMan = "<i class='wx' title='" + wxData[0].WxOpenId + "'></i>" + data[i].SignMan;
                        }
                        //是否发送微信消息
                        DataSendMsg[] mData = webSendMsg.GetDatas(">0", "tb_Perform", PerformId, data[i].UserId, "", "Remark,AddTime");
                        if (mData != null)
                        {
                            string strMsg = "";
                            for (int j = 0; j < mData.Count(); j++)
                            {
                                if (mData[j].Remark.IndexOf("errmsg：ok") > 0)
                                {
                                    strMsg = mData[j].AddTime.ToString("yyyy-MM-dd") + " 已发送成功";
                                    break;
                                }
                            }
                            if (string.IsNullOrEmpty(strMsg))
                            {
                                strMsg = "未发送成功";
                            }
                            data[i].SendMsg = strMsg;
                        }
                    }
                }
                _rpFeedList.DataSource = data;
                _rpFeedList.DataBind();
                _ltFeedNo.Visible = false;
                _ltFeedTotal.Text = data.Count().ToString();
            }
        }
        #endregion
        //
        #region 反馈
        //获取会议/活动更改信息、未反馈数
        public void LoadPerformFeed(DataUser uData, Literal ltPerformFeed, Label lblPerformFeedNum = null)
        {
            if (uData == null)
            {
                return;
            }
            DataPerform qData = new DataPerform();
            qData.ActiveName = "发布";
            qData.Attendees = uData.TrueName;
            qData.EndTimeText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",";
            int pageCur = 1;
            int pageSize = 100;//查询最近100条
            WebPerform webPerform = new WebPerform();
            DataPerform[] data = webPerform.GetDatas(qData, "Id,Title", pageCur, pageSize);
            if (data != null)
            {
                string strInfo = "";
                int intNum = data.Count();
                WebPerformFeed webFeed = new WebPerformFeed();
                for (int i = 0; i < data.Count(); i++)
                {
                    DataPerformFeed[] fData = webFeed.GetDatas("待确认,同意请假,已签到,关闭", data[i].Id, uData.Id, "", "Id,ActiveName");
                    if (fData != null)
                    {
                        if (fData[0].ActiveName == "待确认") {
                            strInfo += string.Format("<a href='perform.aspx?id={0}'>{1} [有更新，需重新确认]</a> ", data[i].Id, data[i].Title);
                        }
                        else
                        {
                            intNum--;
                        }
                    }
                }
                if (ltPerformFeed != null && !string.IsNullOrEmpty(strInfo))
                {
                    ltPerformFeed.Text = strInfo;
                }
                if (lblPerformFeedNum != null)
                {
                    if (intNum > 0)
                    {
                        lblPerformFeedNum.Text = (intNum > 99) ? "99+" : intNum.ToString();
                        lblPerformFeedNum.Visible = true;
                    }
                }
            }
        }
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
            ltInfo.Text = EditFeed(ActiveName, myUser, this, txtReply);
        }
        public string EditFeed(string ActiveName, DataUser uData, Page page, TextBox txtReply2)
        {
            if (uData == null)
            {
                return "<script>$(function(){ alert('请重新登录！'); });</script>";
            }
            string strOut = "";
            string strBack = PublicMod.GetBackUrl();
            int PerformId = (!string.IsNullOrEmpty(page.Request.QueryString["id"])) ? Convert.ToInt32(page.Request.QueryString["id"]) : 0;
            if (PerformId <= 0)
            {
                return "";
            }
            DataPerform[] pData = webPerform.GetData(PerformId, "OverTime,EndTime,ActiveName");
            if (pData == null || pData[0].ActiveName == "履职关闭" || pData[0].OverTime < DateTime.Now)
            {
                return "<script>$(function(){ alert('[履职活动]已关闭或报名已经截止！'); window.location.href='" + strBack + "'; });</script>";
            }
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(uData.TrueName, 20);
            DataPerformFeed data = new DataPerformFeed();
            data.PerformId = PerformId;
            data.UserId = uData.Id;
            data.SignMan = strUser;
            //data.SignTime
            //data.SignIp
            //data.SignAdmin
            //data.Files
            if (ActiveName == "请假申请")
            {
                if (!string.IsNullOrEmpty(txtReply2.Text))
                {
                    data.LeaveNum = 1;
                    data.LeaveReason = HelperMain.SqlFilter(txtReply2.Text.Trim());
                    data.LeaveTime = dtNow;
                    data.LeaveIp = strIp;
                }
                else
                {
                    return "<script>$(function(){ alert('请假，\\n请填写[原因]'); window.history.back(-1); });</script>";
                }
            }
            data.ActiveName = ActiveName;
            WebPerformFeed webFeed = new WebPerformFeed();
            DataPerformFeed[] ckData = webFeed.GetDatas("", data.PerformId, 0, data.SignMan, "Id,LeaveNum,ActiveName");
            if (ckData != null)
            {
                if (ckData[0].ActiveName.IndexOf("已签到") < 0)
                {
                    data.LeaveNum += ckData[0].LeaveNum;
                    data.Id = ckData[0].Id;
                    data.UpTime = dtNow;
                    data.UpIp = strIp;
                    data.UpUser = strUser;
                    data.Id = webFeed.Update(data);
                }
                else
                {
                    return "<script>$(function(){ alert('[签到状态]不能修改！'); window.location.href='" + strBack + "'; });</script>";
                }
            }
            else
            {
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = strUser;
                data.Id = webFeed.Insert(data);
            }
            if (data.Id > 0)
            {
                strOut = "<script>$(function(){ alert('“" + ActiveName + "”已提交成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                strOut = "<script>$(function(){ alert('“" + ActiveName + "”提交失败！'); window.history.back(-1); });</script>";
            }
            return strOut;
        }
        #endregion
        //
        #region 提交
        //加载信息
        public void SubData(int Id, DataUser uData, TextBox txtAcitveName2, HiddenField hfOrg2, DropDownList ddlOrgType2, HiddenField hfSubType2, DropDownList ddlSubType2, RadioButtonList rblHaveBus2, CheckBoxList cblHaveDinner2, TextBox txtOrgName2, TextBox txtSubType2, TextBox txtIsMust2, TextBox txtLinkman2, TextBox txtLinkmanTel2, TextBox txtTitle2, TextBox txtPerformSite2, TextBox txtStartTime2, TextBox txtEndTime2, TextBox txtOverTime2, TextBox txtSignTime2, TextBox txtLeaders2, TextBox txtAttendees2, TextBox txtBody2, HiddenField hfFiles2, PlaceHolder plVerify2, Literal ltVerifyInfo2, Button btnDel2)
        {
            WebOp webOp = new WebOp();
            PublicMod.LoadDropDownLists(hfOrg2, ddlOrgType2, "OpName", uData, webOp);
            PublicMod.LoadPerformType(ddlSubType2, "参加会议,参加调研和视察,委员专题活动");
            PublicMod.LoadDropDownLists(hfSubType2, ddlSubType2, "OpName", null, webOp);
            PublicMod.LoadRadioButtonList(rblHaveBus2, webOp, "履职活动用车");
            PublicMod.LoadCheckBoxList(cblHaveDinner2, webOp, "履职活动其他");
            if (Id <= 0)
            {
                return;
            }
            DataPerform[] data = webPerform.GetData(Id);
            if (data != null)
            {
                txtAcitveName2.Text = data[0].ActiveName;
                txtOrgName2.Text = data[0].OrgName;
                txtSubType2.Text = data[0].SubType;
                txtIsMust2.Text = data[0].IsMust;
                txtLinkman2.Text = data[0].Linkman;
                txtLinkmanTel2.Text = data[0].LinkmanTel;
                txtTitle2.Text = data[0].Title;
                txtPerformSite2.Text = data[0].PerformSite;
                txtStartTime2.Text = data[0].StartTime.ToString("yyyy-MM-dd HH:mm");
                txtEndTime2.Text = data[0].EndTime.ToString("yyyy-MM-dd HH:mm");
                txtOverTime2.Text = (data[0].OverTime > DateTime.MinValue) ? data[0].OverTime.ToString("yyyy-MM-dd HH:mm") : txtStartTime2.Text;
                txtSignTime2.Text = (data[0].SignTime > DateTime.MinValue) ? data[0].SignTime.ToString("yyyy-MM-dd HH:mm") : txtStartTime2.Text;
                HelperMain.SetRadioSelected(rblHaveBus2, data[0].HaveBus);
                HelperMain.SetCheckSelected(cblHaveDinner2, data[0].HaveDinner);
                txtLeaders2.Text = data[0].Leaders.Trim(',');
                txtAttendees2.Text = data[0].Attendees.Trim(',');
                txtBody2.Text = data[0].Body;
                if (!string.IsNullOrEmpty(data[0].Files))
                {
                    hfFiles2.Value = data[0].Files;
                }
                if (!string.IsNullOrEmpty(data[0].VerifyInfo))
                {
                    plVerify2.Visible = true;
                    ltVerifyInfo2.Text = data[0].VerifyInfo;
                }
                btnDel2.Visible = true;
            }
        }
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
            ltInfo.Text = EditData(ActiveName, myUser, this, txtOrgName, txtLinkman, txtLinkmanTel, txtSubType, txtIsMust, txtTitle, txtStartTime, txtEndTime, txtOverTime, txtSignTime, txtPerformSite, txtBody, hfFiles, txtLeaders, txtAttendees, rblHaveBus, cblHaveDinner);
        }
        public string EditData(string ActiveName, DataUser uData, Page page, TextBox txtOrgName2, TextBox txtLinkman2, TextBox txtLinkmanTel2, TextBox txtSubType2, TextBox txtIsMust2, TextBox txtTitle2, TextBox txtStartTime2, TextBox txtEndTime2, TextBox txtOverTime2, TextBox txtSignTime2, TextBox txtPerformSite2, TextBox txtBody2, HiddenField hfFiles2, TextBox txtLeaders2, TextBox txtAttendees2, RadioButtonList rblHaveBus2, CheckBoxList cblHaveDinner2)
        {
            if (uData == null)
            {
                return "<script>$(function(){ alert('请重新登录！'); });</script>";
            }
            string strOut = "";
            DataPerform data = new DataPerform();
            data.Id = (!string.IsNullOrEmpty(page.Request.QueryString["id"])) ? Convert.ToInt32(page.Request.QueryString["id"]) : 0;
            if (ActiveName == "删除")
            {
                if (data.Id > 0)
                {
                    data.Id = webPerform.UpdateActive(data.Id, ActiveName);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                DateTime dtNow = DateTime.Now;
                //data.Id = Convert.ToInt32(txtId.Text);
                data.OrgName = HelperMain.SqlFilter(txtOrgName2.Text.Trim());
                data.Linkman = HelperMain.SqlFilter(txtLinkman2.Text.Trim(), 20);
                data.LinkmanTel = HelperMain.SqlFilter(txtLinkmanTel2.Text.Trim(), 50);
                data.SubType = HelperMain.SqlFilter(txtSubType2.Text.Trim());
                //data.IsMust = HelperMain.SqlFilter(rblIsMust2.SelectedValue.Trim(), 4);
                data.IsMust = HelperMain.SqlFilter(txtIsMust2.Text.Trim(), 4);
                data.Title = HelperMain.SqlFilter(txtTitle2.Text.Trim(), 50);
                data.StartTime = (!string.IsNullOrEmpty(txtStartTime2.Text)) ? Convert.ToDateTime(txtStartTime2.Text.Trim()) : dtNow;
                data.EndTime = (!string.IsNullOrEmpty(txtEndTime2.Text)) ? Convert.ToDateTime(txtEndTime2.Text.Trim()) : data.StartTime;
                data.OverTime = (!string.IsNullOrEmpty(txtOverTime2.Text)) ? Convert.ToDateTime(txtOverTime2.Text.Trim()) : data.StartTime;
                data.SignTime = (!string.IsNullOrEmpty(txtSignTime2.Text)) ? Convert.ToDateTime(txtSignTime2.Text.Trim()) : data.StartTime;
                data.PerformSite = HelperMain.SqlFilter(txtPerformSite2.Text.Trim(), 100);
                data.Body = HelperMain.SqlFilter(txtBody2.Text.Trim());
                data.Files = HelperMain.SqlFilter(hfFiles2.Value.Trim('|'));
                if (!string.IsNullOrEmpty(txtLeaders2.Text.Trim()))
                {
                    data.Leaders = "," + HelperMain.SqlFilter(txtLeaders2.Text.Trim()) + ",";
                }
                if (!string.IsNullOrEmpty(txtAttendees2.Text))
                {
                    data.Attendees = "," + HelperMain.SqlFilter(txtAttendees2.Text.Trim()) + ",";
                }
                data.HaveBus = HelperMain.SqlFilter(rblHaveBus2.SelectedValue.Trim(), 8);
                data.HaveDinner = HelperMain.SqlFilter(HelperMain.GetCheckSelected(cblHaveDinner2), 20);
                data.ActiveName = ActiveName;
                string strIp = HelperMain.GetIpPort();
                string strUser = HelperMain.SqlFilter(uData.TrueName, 20);
                if (data.Id <= 0)
                {
                    data.AddTime = dtNow;
                    data.AddIp = strIp;
                    data.AddUser = strUser;
                    string strSignDesk = "";
                    for (int i = 0; i < config.arrSignDesk.GetLength(0); i++)
                    {
                        if (strUser.IndexOf(config.arrSignDesk[i, 1]) >= 0)
                        {
                            strSignDesk = "," + config.arrSignDesk[i, 0] + ",";
                            break;
                        }
                    }
                    data.SignDesk = strSignDesk;

                    data.Id = webPerform.Insert(data);
                }
                else
                {
                    data.UpTime = dtNow;
                    data.UpIp = strIp;
                    data.UpUser = strUser;
                    if (webPerform.Update(data) <= 0)
                    {
                        data.Id = -1;
                    }
                }
            }
            if (data.Id > 0)
            {
                string strBack = "perform.aspx?ac=my";//PublicMod.GetBackUrl();
                strOut = "<script>$(function(){ alert('“" + ActiveName + "”成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                strOut = "<script>$(function(){ alert('“" + ActiveName + "”失败！'); window.history.back(-1); });</script>";
            }
            return strOut;
        }
        #endregion
        //
    }
}