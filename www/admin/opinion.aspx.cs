using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mod.main;
using hkzx.db;
using hkzx.user;

namespace hkzx.web.admin
{
    public partial class opinion : System.Web.UI.Page
    {
        private DataAdmin myUser = null;
        WebOpinion webOpinion = new WebOpinion();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperAdmin.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
            {
                //Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            if (myUser.Powers.IndexOf("alls") < 0 && myUser.Powers.IndexOf("opin") < 0)
            {
                Response.Redirect("./");
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.Powers = myUser.Powers;
            plNav.Visible = true;
            if (!IsPostBack)
            {
                string strTitle = "";
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    strTitle = "查阅提案情况";
                    plEdit.Visible = true;
                    loadData(Convert.ToInt32(Request.QueryString["id"]));
                }
                else if (Request.QueryString["down"] == "xls")
                {
                    strTitle = "下载提案";
                    downXls();
                }
                else if (Request.QueryString["ac"] == "feed")
                {
                    strTitle = "征询意见统计";
                    plFeed.Visible = true;
                    countFeed();
                }
                else if (Request.QueryString["ac"] == "user")
                {
                    strTitle = "提案查询(按委员)";
                    plQuery.Visible = true;
                    plQueryUser.Visible = true;
                    queryUser();
                }
                else
                {
                    if (myUser.Grade >= 9 && myUser.Powers.IndexOf("alls") >= 0)
                    {
                        btnDels.Visible = true;
                    }
                    strTitle = "提案查询";
                    plQuery.Visible = true;
                    plQueryOpinion.Visible = true;
                    queryData();
                }
                Header.Title += " - " + strTitle;
            }
        }
        //
        #region 查询
        //首页列表
        public void MyList(Repeater rpList, Literal ltNo)
        {
            DataOpinion qData = new DataOpinion();
            qData.ActiveName = "待立案";
            //qData.SubTimeText = DateTime.Now.ToString("yyyy-MM-dd") + ",";
            string strOrder = "o.SubTime ASC, o.UpTime DESC, o.AddTime DESC";
            listData(qData, strOrder, rpList, ltNo);
        }
        //加载列表
        private void listData(DataOpinion qData, string strOrder, Repeater rpList, Literal ltNo, Label lblNav = null, Literal ltTotal = null, Page page = null)
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
            DataOpinion[] data = webOpinion.GetDatas(qData, "", pageCur, pageSize, strOrder, "total");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    if (!string.IsNullOrEmpty(data[i].AdviseHostOrg) && data[i].AdviseHostOrg.IndexOf("|") > 0)
                    {
                        data[i].AdviseHostOrg = data[i].AdviseHostOrg.Substring(0, data[i].AdviseHostOrg.IndexOf("|"));
                    }
                    if (!string.IsNullOrEmpty(data[i].AdviseHelpOrg) && data[i].AdviseHelpOrg.IndexOf("|") > 0)
                    {
                        data[i].AdviseHelpOrg = data[i].AdviseHelpOrg.Substring(0, data[i].AdviseHelpOrg.IndexOf("|"));
                    }
                    data[i].SubMan = data[i].SubMan.Trim(',');
                    switch (data[i].ActiveName)
                    {
                        case "删除":
                            data[i].rowClass = " class='del' title='删除'";
                            break;
                        case "暂存":
                            data[i].rowClass = " class='save' title='暂存'";
                            data[i].StateName = "选取";
                            break;
                        case "退回":
                            data[i].rowClass = " class='cancel' title='退回'";
                            data[i].StateName = "选取";
                            break;
                        case "待立案":
                            data[i].rowClass = " class='wait' title='待立案'";
                            data[i].StateName = "查看";
                            break;
                        default:
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
        //
        //初始化查询
        private void initQuery()
        {
            string strUrl = Request.Url.ToString();
            if (strUrl.IndexOf("&UserSubNum=") >= 0)
            {
                strUrl = strUrl.Substring(0, strUrl.IndexOf("&UserSubNum="));
            }
            if (strUrl.IndexOf("?") > 0)
            {
                strUrl += "&down=xls";
            }
            else
            {
                strUrl += "?down=xls";
            }
            lnkDownXls.NavigateUrl = strUrl;
            WebOp webOp = new WebOp();
            PublicMod.LoadDropDownList(ddlQPeriod, webOp, "_届", config.PERIOD);
            PublicMod.LoadDropDownList(ddlQTimes, webOp, "_次会议", config.TIMES);
            PublicMod.LoadDropDownList(ddlQSubType, webOp, "提案类别");
            PublicMod.LoadDropDownList(ddlQSubManType, webOp, "用户类别");
            PublicMod.LoadDropDownList(ddlQCommittee, webOp, "专委会");
            PublicMod.LoadDropDownList(ddlQSubsector, webOp, "界别");
            PublicMod.LoadDropDownList(ddlQStreetTeam, webOp, "街道活动组");
            PublicMod.LoadDropDownList(ddlQParty, webOp, "政治面貌");
            PublicMod.LoadDropDownList(ddlQExamHostOrg, webOp, "承办单位");
            for (int i = 0; i < ddlQExamHostOrg.Items.Count; i++)
            {
                ListItem item = new ListItem(ddlQExamHostOrg.Items[i].Text, ddlQExamHostOrg.Items[i].Value);
                ddlQExamHelpOrg.Items.Add(item);
            }
            PublicMod.LoadDropDownList(ddlQResultInfo, webOp, "办理结果");
            for (int i = 0; i < ddlQResultInfo.Items.Count; i++)
            {
                ListItem item = new ListItem(ddlQResultInfo.Items[i].Text, ddlQResultInfo.Items[i].Value);
                ddlQResultInfo2.Items.Add(item);
            }
            PublicMod.LoadDropDownList(ddlQFeedInterview, webOp, "走访情况");
            PublicMod.LoadDropDownList(ddlQFeedTakeWay, webOp, "听取意见方式");
            PublicMod.LoadDropDownList(ddlQFeedAttitude, webOp, "办理人员态度");
            PublicMod.LoadDropDownList(ddlQFeedResult, webOp, "是否同意办理结果");
            PublicMod.LoadDropDownList(ddlQFeedPertinence, webOp, "答复是否针对提案");
            PublicMod.LoadDropDownList(ddlQApplyState, webOp, "提案办理状态");
            PublicMod.LoadDropDownList(ddlQActiveName, webOp, "提案性质");
            //PublicMod.LoadDropDownList(ddlQOrderBy, webOp, "提案查询信息");
            PublicMod.LoadCheckBoxList(cblQFields, webOp, "提案查询信息");
            for (int i = 0; i < cblQFields.Items.Count; i++)
            {
                ListItem item = new ListItem(cblQFields.Items[i].Text, cblQFields.Items[i].Value);
                ddlQOrderBy.Items.Add(item);
            }
            HelperMain.SetDownSelected(ddlQOrderBy, "OpNo");//提案序号
        }
        //查询列表
        private void queryData()
        {
            initQuery();//初始化查询
            DataOpinion qData = getData();
            //int intYear = DateTime.Today.Year;
            DateTime dtStart = DateTime.MinValue;//new DateTime(intYear - 1, 12, 1);
            DateTime dtEnd = DateTime.MinValue;//new DateTime(intYear, 11, 30, 23, 59, 59);
            qData = queryOpinion(qData, dtStart, dtEnd);
            if (qData == null)
            {
                return;
            }
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            string strOrderBy = ddlQOrderBy.SelectedValue + " " + rblQOrderBy.SelectedValue;
            DataOpinion[] data = webOpinion.GetDatas(qData, qData.JoinFields, pageCur, pageSize, strOrderBy, "total", qData.JoinText);
            if (data != null)
            {
                string[] arrFields = qData.QueryFields.Split(',');
                string strThead = "";
                for (int i = 0; i < cblQFields.Items.Count; i++)
                {
                    for (int j = 0; j < arrFields.Count(); j++)
                    {
                        if (cblQFields.Items[i].Value == arrFields[j])
                        {
                            if (cblQFields.Items[i].Value == "Feedback")
                            {
                                strThead += "<th>走访情况</th><th>办理态度</th><th>办理结果</th>";//<th>听取意见方式</th><th>是否针对提案</th><th>分管领导答复</th>
                            }
                            else
                            {
                                strThead += "<th>" + cblQFields.Items[i].Text + "</th>";
                            }
                            break;
                        }
                    }
                }
                ltQueryThead.Text = strThead;
                WebOpinionFeed webFeed = new WebOpinionFeed();
                for (int i = 0; i < data.Count(); i++)
                {
                    switch (data[i].ActiveName)
                    {
                        case "待立案":
                            data[i].rowClass = " class='save' title='待立案'";
                            break;
                        case "暂存":
                            data[i].rowClass = " class='cancel' title='暂存'";
                            break;
                        case "退回":
                            data[i].rowClass = " class='cancel' title='退回'";
                            break;
                        case "删除":
                            data[i].rowClass = " class='del' title='删除'";
                            break;
                        default:
                            break;
                    }
                    string strSubMans = "";
                    string strTr = "";
                    for (int j = 0; j < arrFields.Count(); j++)
                    {
                        string strTd = loadFields(data[i], arrFields[j], webFeed);
                        string strAlt = "";
                        switch (arrFields[j])
                        {
                            case "Id":
                                if (!string.IsNullOrEmpty(data[i].OpNum))
                                {
                                    strAlt = string.Format(" title='内部流水号：{0}'", data[i].OpNum);
                                }
                                break;
                            case "SubTime":
                                strAlt = string.Format(" title='{0:yyyy-MM-dd HH:mm:ss}'", data[i].SubTime);
                                break;
                            case "SubMan":
                                strSubMans = strTd;
                                break;
                            case "ActiveName":
                                if (strTd == "待立案" && strSubMans.IndexOf("*") > 0)
                                {
                                    strTd = "待会签";
                                }
                                else if (strTd == "不立案" && !string.IsNullOrEmpty(data[i].VerifyInfo))
                                {
                                    strAlt = string.Format(" title='原因：{0}'", data[i].VerifyInfo);
                                }
                                break;
                            default:
                                break;
                        }
                        strTr += "<td" + strAlt + ">" + strTd + "</td>";
                    }
                    data[i].tbody = strTr;
                }
                rpQueryTbody.DataSource = data;
                rpQueryTbody.DataBind();
                ltQueryNo.Visible = false;
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = Request.Url.ToString();
                lblQueryNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
                ltQueryTotal.Text = data[0].total.ToString();
            }
            //listData(data, rpQueryList, ltQueryNo, lblQueryNav, "", ltQueryTotal);
        }
        //加载查询内容
        private string loadFields(DataOpinion data, string strField, WebOpinionFeed webFeed = null)
        {//Id,OpNo,SubMan,Summary,SubType,ActiveName,ExamHostOrg,ExamHelpOrg,Result,Feedback,ReApply,ApplyState,Result2,SubTime,PlannedDate
            string strOut = "";
            switch (strField)
            {
                case "Id":
                    strOut = data.Id.ToString();
                    break;
                case "OpNo":
                    strOut = data.OpNo;
                    break;
                case "SubMan":
                    strOut = data.SubMan.Trim(',');
                    string strSubMans = "";
                    switch (data.SubManType)
                    {
                        case "委员":
                            strSubMans = data.SubMans.Trim(',');//data.SubMan2.Trim(',') + "," + 
                            int intMode = (myUser.Grade >= 9) ? 0 : 1;
                            strSubMans = PublicMod.GetSubMans(data.Id, strSubMans, intMode);
                            break;
                        default:
                            if (!string.IsNullOrEmpty(data.Linkman))
                            {
                                strSubMans += "联系人：" + data.Linkman;
                            }
                            break;
                    }
                    if (strSubMans != "")
                    {
                        if (!string.IsNullOrEmpty(strOut))
                        {
                            strOut += "<br/>" + strSubMans;
                        }
                        else
                        {
                            strOut = strSubMans;
                        }
                    }
                    break;
                case "ExamHostOrg":
                    strOut = PublicMod.GetOrgText(data.ExamHostOrg);
                    break;
                case "ExamHelpOrg":
                    strOut = PublicMod.GetOrgText(data.ExamHelpOrg);
                    break;
                case "ResultInfo":
                    strOut = data.ResultInfo;
                    if (!string.IsNullOrEmpty(data.ResultInfo2))
                    {
                        strOut += "<br/>" + data.ResultInfo2;
                    }
                    break;
                case "ResultInfo1":
                    strOut = data.ResultInfo;
                    break;
                case "Feedback":
                    strOut = data.FeedInterview + "</td><td>" + data.FeedAttitude + "</td><td>" + data.FeedResult;// + "</td><td>" + data.FeedTakeWay + "</td><td>" + data.FeedPertinence + "</td><td>" + data.FeedLeaderReply
                    break;
                case "IsFeed":
                    strOut = (data.IsFeed == "是" && data.FeedActive > 0) ? "已反馈" : data.IsFeed;
                    break;
                case "Summary":
                    strOut = data.Summary;
                    if (data.IsGood == "是")
                    {
                        strOut = "<i class='flag'>优秀</i>" + strOut;
                    }
                    if (data.IsPoint == "是")
                    {
                        strOut = "<i class='flag'>重点</i>" + strOut;
                    }
                    break;
                case "ActiveName":
                    strOut = data.ActiveName;
                    break;
                case "ReApply":
                    strOut = data.ReApply;
                    break;
                case "ApplyState":
                    strOut = data.ApplyState;
                    break;
                case "ResultInfo2":
                    strOut = data.ResultInfo2;
                    break;
                case "SubType":
                    strOut = data.SubType;
                    break;
                case "SubTime":
                    strOut = (data.SubTime > DateTime.MinValue) ? data.SubTime.ToString("yyyy-MM-dd") : "";
                    break;
                case "PlannedDate":
                    strOut = (data.PlannedDate > DateTime.MinValue) ? data.PlannedDate.ToString("yyyy-MM-dd") : "";
                    break;
                default:
                    break;
            }
            if (strOut == "")
            {
                strOut = "&nbsp;";
            }
            return strOut;
        }
        //获取查询条件
        private DataOpinion getData()
        {
            DataOpinion qData = new DataOpinion();
            string strPeriod = "";
            if (!string.IsNullOrEmpty(Request.QueryString["Period"]))
            {
                strPeriod = HelperMain.SqlFilter(Request.QueryString["Period"].Trim(), 4);
                HelperMain.SetDownSelected(ddlQPeriod, strPeriod);
            }
            else if (string.IsNullOrEmpty(Request.QueryString["ac"]))
            {
                strPeriod = HelperMain.SqlFilter(ddlQPeriod.SelectedValue, 4);
            }
            else
            {
                ddlQPeriod.SelectedIndex = -1;
            }
            qData.Period = strPeriod;
            string strTimes = "";
            if (!string.IsNullOrEmpty(Request.QueryString["Times"]))
            {
                strTimes = HelperMain.SqlFilter(Request.QueryString["Times"].Trim(), 4);
                HelperMain.SetDownSelected(ddlQTimes, strTimes);
            }
            else if (string.IsNullOrEmpty(Request.QueryString["ac"]))
            {
                strTimes = HelperMain.SqlFilter(ddlQTimes.SelectedValue, 4);
            }
            else
            {
                ddlQTimes.SelectedIndex = -1;
            }
            qData.Times = strTimes;
            if (!string.IsNullOrEmpty(Request.QueryString["OpNo"]))
            {
                qData.OpNo = HelperMain.SqlFilter(Request.QueryString["OpNo"].Trim(), 20);
                txtQOpNo.Text = qData.OpNo;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ApplyState"]))
            {
                qData.ApplyState = HelperMain.SqlFilter(Request.QueryString["ApplyState"].Trim(), 8);
                HelperMain.SetDownSelected(ddlQApplyState, qData.ApplyState);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["TimeMark"]))
            {
                qData.TimeMark = HelperMain.SqlFilter(Request.QueryString["TimeMark"].Trim(), 4);
                HelperMain.SetDownSelected(ddlQTimeMark, qData.TimeMark);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ActiveName"]))
            {
                qData.ActiveName = HelperMain.SqlFilter(Request.QueryString["ActiveName"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQActiveName, qData.ActiveName);
            }
            else
            {
                qData.ActiveName = "归并,待立案,立案,不立案,退回";//"<>'暂存'"
            }
            if (!string.IsNullOrEmpty(Request.QueryString["SubType"]))
            {
                qData.SubType = HelperMain.SqlFilter(Request.QueryString["SubType"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQSubType, qData.SubType);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["IsPoint"]))
            {
                qData.IsPoint = HelperMain.SqlFilter(Request.QueryString["IsPoint"].Trim(), 2);
                HelperMain.SetDownSelected(ddlQIsPoint, qData.IsPoint);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["IsOpen"]))
            {
                qData.IsOpen = HelperMain.SqlFilter(Request.QueryString["IsOpen"].Trim(), 2);
                HelperMain.SetDownSelected(ddlQIsOpen, qData.IsOpen);
            }
            if (Request.QueryString["IsSubMan1"] == "1")
            {
                HelperMain.SetCheckSelected(cblQIsSubMan1, "1");
                qData.IsSubMan1 = true;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["SubMan"]))
            {
                qData.SubMan = HelperMain.SqlFilter(Request.QueryString["SubMan"].Trim());
                txtQSubMan.Text = qData.SubMan;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["SubManType"]))
            {
                qData.SubManType = HelperMain.SqlFilter(Request.QueryString["SubManType"].Trim(), 8);
                HelperMain.SetDownSelected(ddlQSubManType, qData.SubManType);
                if (Request.QueryString["ac"] == "user" && qData.SubManType == "委员")
                {
                    qData.UserCode = "14%";
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ExamHostOrg"]))
            {
                qData.ExamHostOrg = HelperMain.SqlFilter(Request.QueryString["ExamHostOrg"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQExamHostOrg, qData.ExamHostOrg);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ExamHelpOrg"]))
            {
                qData.ExamHelpOrg = HelperMain.SqlFilter(Request.QueryString["ExamHelpOrg"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQExamHelpOrg, qData.ExamHelpOrg);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ExamOrgType"]))
            {
                qData.ExamOrgType = HelperMain.SqlFilter(Request.QueryString["ExamOrgType"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQExamOrgType, qData.ExamOrgType);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ResultInfo1"]))
            {
                qData.ResultInfo = HelperMain.SqlFilter(Request.QueryString["ResultInfo1"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQResultInfo, qData.ResultInfo);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ResultInfo2"]))
            {
                qData.ResultInfo2 = HelperMain.SqlFilter(Request.QueryString["ResultInfo2"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQResultInfo2, qData.ResultInfo2);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Summary"]))
            {
                string strTmp = HelperMain.SqlFilter(Request.QueryString["Summary"].Trim());
                string[] arr = strTmp.Split(' ');
                for (int i = 0; i < arr.Count(); i++)
                {
                    arr[i] = "%" + arr[i] + "%";
                }
                qData.Summary = string.Join("+", arr);
                txtQSummary.Text = strTmp;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Body"]))
            {
                string strTmp = HelperMain.SqlFilter(Request.QueryString["Body"].Trim());
                string[] arr = strTmp.Split(' ');
                for (int i = 0; i < arr.Count(); i++)
                {
                    arr[i] = "%" + arr[i] + "%";
                }
                qData.Body = string.Join("+", arr);
                txtQBody.Text = strTmp;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ReApply"]))
            {
                qData.ReApply = HelperMain.SqlFilter(Request.QueryString["ReApply"].Trim(), 2);
                HelperMain.SetDownSelected(ddlQReApply, qData.ReApply);
            }
            //外联查询
            if (!string.IsNullOrEmpty(Request.QueryString["UserSex"]))
            {
                qData.UserSex = HelperMain.SqlFilter(Request.QueryString["UserSex"].Trim(), 2);
                HelperMain.SetDownSelected(ddlQUserSex, qData.UserSex);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Party"]))
            {
                qData.UserParty = HelperMain.SqlFilter(Request.QueryString["Party"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQParty, qData.UserParty);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Committee"]))
            {
                qData.UserCommittee = HelperMain.SqlFilter(Request.QueryString["Committee"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQCommittee, qData.UserCommittee);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Subsector"]))
            {
                qData.UserSubsector = HelperMain.SqlFilter(Request.QueryString["Subsector"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQSubsector, qData.UserSubsector);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["StreetTeam"]))
            {
                qData.UserStreetTeam = HelperMain.SqlFilter(Request.QueryString["StreetTeam"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQStreetTeam, qData.UserStreetTeam);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["FeedInterview"]))
            {
                qData.FeedInterview = HelperMain.SqlFilter(Request.QueryString["FeedInterview"].Trim(), 8);
                HelperMain.SetDownSelected(ddlQFeedInterview, qData.FeedInterview);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["FeedAttitude"]))
            {
                qData.FeedAttitude = HelperMain.SqlFilter(Request.QueryString["FeedAttitude"].Trim(), 8);
                HelperMain.SetDownSelected(ddlQFeedAttitude, qData.FeedAttitude);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["FeedResult"]))
            {
                qData.FeedResult = HelperMain.SqlFilter(Request.QueryString["FeedResult"].Trim(), 8);
                HelperMain.SetDownSelected(ddlQFeedResult, qData.FeedResult);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["FeedTakeWay"]))
            {
                qData.FeedTakeWay = HelperMain.SqlFilter(Request.QueryString["FeedTakeWay"].Trim(), 8);
                HelperMain.SetDownSelected(ddlQFeedTakeWay, qData.FeedTakeWay);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["FeedPertinence"]))
            {
                qData.FeedPertinence = HelperMain.SqlFilter(Request.QueryString["FeedPertinence"].Trim(), 8);
                HelperMain.SetDownSelected(ddlQFeedPertinence, qData.FeedPertinence);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["FeedLeaderReply"]))
            {
                qData.FeedLeaderReply = HelperMain.SqlFilter(Request.QueryString["FeedLeaderReply"].Trim(), 8);
                HelperMain.SetDownSelected(ddlQFeedLeaderReply, qData.FeedLeaderReply);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["FeedActive"]))
            {
                qData.FeedActiveName = HelperMain.SqlFilter(Request.QueryString["FeedActive"].Trim(), 8);
                HelperMain.SetDownSelected(ddlQFeedActive, qData.FeedActiveName);
            }
            //查询排序
            if (!string.IsNullOrEmpty(Request.QueryString["Order"]) && !string.IsNullOrEmpty(Request.QueryString["By"]))
            {
                HelperMain.SetDownSelected(ddlQOrderBy, Request.QueryString["Order"]);
                HelperMain.SetRadioSelected(rblQOrderBy, Request.QueryString["By"]);
            }
            //按委员查询
            if (!string.IsNullOrEmpty(Request.QueryString["UserSubNum"]))
            {
                qData.UserSubNum = Convert.ToInt32(Request.QueryString["UserSubNum"]);
                txtQUserSubNum.Text = qData.UserSubNum.ToString();
            }
            if (!string.IsNullOrEmpty(Request.QueryString["UserSubMin"]))
            {
                qData.UserSubMin = Convert.ToInt32(Request.QueryString["UserSubMin"]);
                txtQUserSubMin.Text = qData.UserSubMin.ToString();
            }
            return qData;
        }
        //下载数据
        private void downXls()
        {
            WebOp webOp = new WebOp();
            PublicMod.LoadDropDownList(ddlQPeriod, webOp, "_届", config.PERIOD);
            PublicMod.LoadDropDownList(ddlQTimes, webOp, "_次会议", config.TIMES);
            //plQuery.Visible = true;
            //queryData();
            DataOpinion qData = getData();
            int intYear = DateTime.Today.Year;
            DateTime dtStart = DateTime.MinValue;
            DateTime dtEnd = DateTime.MinValue;
            if (DateTime.Today > new DateTime(intYear, 12, 1))
            {
                dtStart = new DateTime(intYear, 1, 1);
                dtEnd = new DateTime(intYear, 12, 31, 23, 59, 59);
            }
            else
            {
                dtStart = new DateTime(intYear - 1, 12, 1);
                dtEnd = new DateTime(intYear, 11, 30, 23, 59, 59);
            }
            qData = queryOpinion(qData, dtStart, dtEnd, "*");
            if (qData == null)
            {
                ltInfo.Text = "<script>$(function(){ alert('未查询到“提案”'); window.history.back(-1); });</script>";
                return;
            }
            string strOrderBy = "OpNo ASC";
            DataOpinion[] data = webOpinion.GetDatas(qData, qData.JoinFields, 1, 0, strOrderBy, "", qData.JoinText);
            if (data != null)
            {
                DateTime dtNow = DateTime.Now;
                string virtualPath = string.Format("../download/{0:yyyy}/{0:MM}/", dtNow);
                string filepath = HttpContext.Current.Server.MapPath(virtualPath);
                if (!System.IO.Directory.Exists(filepath))
                {
                    System.IO.Directory.CreateDirectory(filepath);
                }
                string strTitle = qData.ActiveName + "提案";
                string strFileName = string.Format("{0}_{1:yyyyMMddHHmmss}.xlsx", strTitle, dtNow);
                string fileName = virtualPath + "/" + strFileName;
                string path = Server.MapPath(fileName);
                //首先初始化excel object
                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                //在创建excel workbook之前，检查系统是否安装excel
                if (excelApp == null)
                {
                    // if equal null means EXCEL is not installed.  
                    //MessageBox.Show("Excel is not properly installed!");
                    return;
                }
                //判断文件是否存在，如果存在就打开workbook，如果不存在就新建一个
                Microsoft.Office.Interop.Excel.Workbook workBook;
                if (System.IO.File.Exists(path))
                {
                    workBook = excelApp.Application.Workbooks.Open(path, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    //workBook = excelApp.Application.Workbooks.Add(true);
                }
                else
                {
                    workBook = excelApp.Application.Workbooks.Add(true);
                }
                //在创建完workbook之后，下一步就是新建worksheet并写入数据
                Microsoft.Office.Interop.Excel.Worksheet workSheet = workBook.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;
                workSheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.Worksheets.get_Item(1);//获得第i个sheet，准备写入
                //workSheet.Name = strFileName.Replace(".xls", "");//第1个表";
                workSheet.Cells[1, 1] = string.Format("下载时间：{0:yyyy-MM-dd HH:mm:ss}，下载人：{1}", dtNow, myUser.AdminName);
                workSheet.Rows["3:6"].RowHeight = 20;
                if (dtStart > DateTime.MinValue && dtEnd > DateTime.MinValue)
                {
                    PublicMod.SetCells(workSheet, 3, 1, string.Format("提案时间：{0:yyyy/MM/dd} - {1:yyyy/MM/dd}", dtStart, dtEnd));
                }
                string strPeriodTimes = "";
                if (!string.IsNullOrEmpty(qData.Period))
                {
                    strPeriodTimes += qData.Period + "届";
                }
                if (!string.IsNullOrEmpty(qData.Times))
                {
                    strPeriodTimes += qData.Times + "次";
                }
                if (!string.IsNullOrEmpty(strPeriodTimes))
                {
                    strTitle = strPeriodTimes + " " + strTitle;
                }
                workSheet.Cells[5, 1].Font.Size = 16;//设置字体大小
                PublicMod.SetCells(workSheet, 5, 1, strTitle, "center,bold");
                string strFields = (!string.IsNullOrEmpty(Request.QueryString["Fields"])) ? Request.QueryString["Fields"] : "";
                int intRow = 6;
                int num = 0;
                if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("Id") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "流水号", "center,bold,border", "LightGray");
                }
                if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("OpNo") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "提案序号", "center,bold,border", "LightGray");
                }
                if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("SubType") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 10;//列宽
                    PublicMod.SetCells(workSheet, intRow, num, "提案分类", "center,bold,border", "LightGray");
                }
                if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("SubMan") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "第一提案人", "fit,center,bold,border", "LightGray");
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 20;
                    PublicMod.SetCells(workSheet, intRow, num, "联名提案人", "center,bold,border", "LightGray");
                }
                if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("Summary") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 30;
                    PublicMod.SetCells(workSheet, intRow, num, "案由", "center,bold,border", "LightGray");
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 30;
                    PublicMod.SetCells(workSheet, intRow, num, "提案内容", "center,bold,border", "LightGray");
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 30;
                    PublicMod.SetCells(workSheet, intRow, num, "附件", "center,bold,border", "LightGray");
                }
                if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("ActiveName") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "提案性质", "center,bold,border", "LightGray");
                }
                if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("ExamHostOrg") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 15;
                    PublicMod.SetCells(workSheet, intRow, num, "主办单位", "center,bold,border", "LightGray");
                }
                if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("ExamHelpOrg") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 15;
                    PublicMod.SetCells(workSheet, intRow, num, "会办单位", "center,bold,border", "LightGray");
                }
                if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("ResultInfo1") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 15;
                    PublicMod.SetCells(workSheet, intRow, num, "办理结果", "center,bold,border", "LightGray");
                }
                if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("SubTime") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 20;
                    PublicMod.SetCells(workSheet, intRow, num, "提案时间", "center,bold,border", "LightGray");
                }
                int intCol = num;
                //workSheet.Range[workSheet.Cells[3, 1], workSheet.Cells[3, intCol]].MergeCells = true;//合并单元格
                workSheet.Range[workSheet.Cells[5, 1], workSheet.Cells[5, intCol]].MergeCells = true;//合并单元格
                bool blFeedback = false;
                if (strFields.IndexOf("Feedback") >= 0)
                {
                    blFeedback = true;
                    intCol += 3;
                    num++;
                    PublicMod.SetCells(workSheet, (intRow - 1), num, "意见反馈", "center,bold", "LightGray");
                    workSheet.Range[workSheet.Cells[(intRow - 1), num], workSheet.Cells[(intRow - 1), intCol]].MergeCells = true;//合并单元格
                    PublicMod.SetCells(workSheet, intRow, num, "走访情况", "fit,center,bold,border", "LightGray");
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "办理态度", "fit,center,bold,border", "LightGray");
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "办理结果", "fit,center,bold,border", "LightGray");
                    //num++;
                    //PublicMod.SetCells(workSheet, intRow, num, "听取意见方式", "fit,center,bold,border", "LightGray");
                    //num++;
                    //PublicMod.SetCells(workSheet, intRow, num, "是否针对提案", "fit,center,bold,border", "LightGray");
                    //num++;
                    //PublicMod.SetCells(workSheet, intRow, num, "分管领导答复", "fit,center,bold,border", "LightGray");
                }
                if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("IsFeed") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "征询意见", "fit,center,bold,border", "LightGray");
                }
                if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("ReApply") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "再办理", "center,bold,border", "LightGray");
                }
                if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("ApplyState") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "跟踪办理情况", "fit,center,bold,border", "LightGray");
                }
                if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("ResultInfo2") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "跟踪办理结果", "fit,center,bold,border", "LightGray");
                }
                if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("PlannedDate") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 15;
                    PublicMod.SetCells(workSheet, intRow, num, "计划办结日期", "center,bold,border", "LightGray");
                }
                string strUrl = (Request.IsLocal) ? "http://" + config.HOSTDEBUG + "/" : "http://" + config.HOSTIP + "/";
                intRow = 7;
                for (int i = 0; i < data.Count(); i++)
                {
                    num = 0;
                    if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("Id") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].Id.ToString(), "center,border");//流水号
                    }
                    if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("OpNo") >= 0)
                    {
                        num++;
                        //workSheet.Cells[intRow, num].RowHeight = 20;//行高
                        PublicMod.SetCells(workSheet, intRow, num, data[i].OpNo, "txt,center,border");//提案号
                    }
                    if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("SubType") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].SubType, "txt,center,border");//提案分类
                    }
                    if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("SubMan") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].SubMan.Trim(','), "txt,border");//第一提案人
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].SubMans.Trim(','), "txt,wrap,border");//联名提案人
                    }
                    int intFilesNum = 0;
                    if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("Summary") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].Summary, "txt,wrap,border");//案由
                        string strBodyUrl = "";
                        string strBody = HelperMain.DelUbb(data[i].Body);
                        if (strBody.Length >= 50)
                        {
                            strBody = strBody.Substring(0, 50) + "……";
                            strBodyUrl = strUrl + "admin/opinion.aspx?id=" + data[i].Id.ToString();
                        }
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, strBody, "txt,wrap,border", "", strBodyUrl);//提案内容
                        if (!string.IsNullOrEmpty(data[i].Files))
                        {
                            string[] arrFiles = data[i].Files.Split('|');
                            for (int j = 0; j < arrFiles.Count(); j++)
                            {
                                if (!string.IsNullOrEmpty(arrFiles[j]))
                                {
                                    string strFilesUrl = arrFiles[j].Replace("../", strUrl);
                                    num++;
                                    workSheet.Cells[intRow + intFilesNum, num].ColumnWidth = 30;
                                    PublicMod.SetCells(workSheet, intRow + intFilesNum, num, strFilesUrl, "txt,border", "", strFilesUrl);//附件
                                    PublicMod.SetCells(workSheet, intRow + intFilesNum, num + 1, "", "txt,border");//空//提案性质
                                    intFilesNum++;
                                }
                            }
                        }
                        else
                        {
                            num++;
                            PublicMod.SetCells(workSheet, intRow, num, "", "txt,border");//空//附件
                        }
                    }
                    if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("ActiveName") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].ActiveName, "txt,center,border");//提案性质
                    }
                    if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("ExamHostOrg") >= 0)
                    {
                        num++;
                        string strExamHostOrg = PublicMod.GetOrgText(data[i].ExamHostOrg);
                        PublicMod.SetCells(workSheet, intRow, num, strExamHostOrg, "txt,center,wrap,border");//主办单位
                    }
                    if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("ExamHelpOrg") >= 0)
                    {
                        num++;
                        string strExamHelpOrg = PublicMod.GetOrgText(data[i].ExamHelpOrg);
                        PublicMod.SetCells(workSheet, intRow, num, strExamHelpOrg, "txt,center,wrap,border");//会办单位
                    }
                    if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("ResultInfo1") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].ResultInfo, "txt,center,wrap,border");//办理结果
                    }
                    if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("SubTime") >= 0)
                    {
                        num++;
                        string strSubTime = (data[i].SubTime > DateTime.MinValue) ? data[i].SubTime.ToString("yyyy-MM-dd HH:mm:ss") : "";
                        strSubTime = strSubTime.Replace(" 00:00:00", "");
                        PublicMod.SetCells(workSheet, intRow, num, strSubTime, "date,center,wrap,border");//提案时间
                    }
                    if (blFeedback)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].FeedInterview, "txt,center,border");//走访情况
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].FeedAttitude, "txt,center,border");//办理态度
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].FeedResult, "txt,center,border");//办理结果
                        //num++;
                        //PublicMod.SetCells(workSheet, intRow, num, data[i].FeedTakeWay, "txt,center,border");//听取意见方式
                        //num++;
                        //PublicMod.SetCells(workSheet, intRow, num, data[i].FeedPertinence, "txt,center,border");//是否针对提案
                        //num++;
                        //PublicMod.SetCells(workSheet, intRow, num, data[i].FeedLeaderReply, "txt,center,border");//(团体)分管领导答复
                    }
                    if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("IsFeed") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].IsFeed, "txt,center,border");//征询意见
                    }
                    if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("ReApply") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].ReApply, "txt,center,border");//再办理
                    }
                    if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("ApplyState") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].ApplyState, "txt,center,border");//跟踪办理情况
                    }
                    if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("ResultInfo2") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].ResultInfo2, "txt,center,wrap,border");//跟踪办理结果
                    }
                    if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("PlannedDate") >= 0)
                    {
                        num++;
                        string strPlannedDate = (data[i].PlannedDate > DateTime.MinValue) ? data[i].PlannedDate.ToString("yyyy-MM-dd") : "";
                        PublicMod.SetCells(workSheet, intRow, num, strPlannedDate, "txt,center,border");//计划办结日期
                    }
                    if (intFilesNum > 0)
                    {
                        intRow += intFilesNum;
                    }
                    else
                    {
                        intRow++;
                    }
                }
                //有两个选项可以设置，如下
                excelApp.Visible = false;//visable属性设置为true的话，excel程序会启动；false的话，excel只在后台运行
                excelApp.DisplayAlerts = false;//displayalert设置为true将会显示excel中的提示信息
                //保存文件，关闭workbook
                //workBook.SaveAs(path, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
                //workBook.SaveAs(path, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                workBook.SaveCopyAs(path);//DCOM配置excel权限，最后将”SaveAs”方法改成“SaveCopyAs”解决。
                workBook.Close(false, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                //退出并清理objects，回收内存
                excelApp.Quit();
                workSheet = null;
                workBook = null;
                excelApp = null;
                GC.Collect();
                Response.Redirect(fileName);
            }
        }
        //按提案查询
        private DataOpinion queryOpinion(DataOpinion qData, DateTime dtStart, DateTime dtEnd, string strFields = "")
        {
            string strJoin = "";
            //if (!string.IsNullOrEmpty(Request.QueryString["UserSex"]) || !string.IsNullOrEmpty(Request.QueryString["Party"]) || !string.IsNullOrEmpty(Request.QueryString["Committee"]) || !string.IsNullOrEmpty(Request.QueryString["Subsector"]) || !string.IsNullOrEmpty(Request.QueryString["StreetTeam"]))
            if (!string.IsNullOrEmpty(qData.UserSex) || !string.IsNullOrEmpty(qData.UserParty) || !string.IsNullOrEmpty(qData.UserCommittee) || !string.IsNullOrEmpty(qData.UserSubsector) || !string.IsNullOrEmpty(qData.UserStreetTeam))
            {
                if (Request.QueryString["IsSubMan1"] == "1")
                {
                    strJoin += " INNER JOIN tb_User AS u ON (o.SubMan LIKE '%' + u.TrueName + '%')";//" INNER JOIN tb_User AS u ON (u.Id=o.UserId)";
                }
                else
                {
                    strJoin += " INNER JOIN tb_User AS u ON (o.SubMan LIKE '%' + u.TrueName + '%' OR o.SubMans LIKE '%' + u.TrueName + '%')";
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["FeedActive"]) || !string.IsNullOrEmpty(Request.QueryString["FeedInterview"]) || !string.IsNullOrEmpty(Request.QueryString["FeedAttitude"]) || !string.IsNullOrEmpty(Request.QueryString["FeedResult"]))// || !string.IsNullOrEmpty(Request.QueryString["FeedTakeWay"]) || !string.IsNullOrEmpty(Request.QueryString["FeedPertinence"]) || !string.IsNullOrEmpty(Request.QueryString["FeedLeaderReply"])
            {
                strJoin += " LEFT JOIN tb_Opinion_Feed AS f ON (f.OpId=o.Id AND f.OpType='提案')";
            }
            if (!string.IsNullOrEmpty(strFields))
            {

            }
            else if (!string.IsNullOrEmpty(Request.QueryString["Fields"]))
            {
                strFields = Request.QueryString["Fields"].Trim();
            }
            else
            {
                strFields = HelperMain.GetCheckSelected(cblQFields);
            }
            if (string.IsNullOrEmpty(strFields))
            {
                for (int i = 0; i < cblQFields.Items.Count; i++)
                {
                    strFields += "," + cblQFields.Items[i].Value;
                }
                strFields = strFields.Trim(',');
            }
            HelperMain.SetCheckSelected(cblQFields, strFields);
            string[] arrFields = strFields.Split(',');
            string strJoinShow = "";
            string[] tmpFields = new string[arrFields.Count()];
            for (int i = 0; i < arrFields.Count(); i++)
            {
                if (!string.IsNullOrEmpty(arrFields[i]))
                {
                    if (arrFields[i] == "*")
                    {
                        tmpFields[i] = "o.*, f1.Id AS FeedId, f1.Interview AS FeedInterview, f1.Attitude AS FeedAttitude, f1.Result AS FeedResult, f1.TakeWay AS FeedTakeWay, f1.Pertinence AS FeedPertinence, f1.LeaderReply AS FeedLeaderReply, f1.Active AS FeedActive";
                        strJoinShow = " LEFT JOIN tb_Opinion_Feed AS f1 ON (f1.OpId=o.Id AND f1.OpType='提案')";
                    }
                    else if (arrFields[i] == "Feedback")
                    {
                        tmpFields[i] = "f1.Id AS FeedId, f1.Interview AS FeedInterview, f1.Attitude AS FeedAttitude, f1.Result AS FeedResult, f1.TakeWay AS FeedTakeWay, f1.Pertinence AS FeedPertinence, f1.LeaderReply AS FeedLeaderReply, f1.Active AS FeedActive";
                        strJoinShow = " LEFT JOIN tb_Opinion_Feed AS f1 ON (f1.OpId=o.Id AND f1.OpType='提案')";
                    }
                    else if (arrFields[i] == "Id")
                    {
                        tmpFields[i] = "o.Id,o.OpNum";
                    }
                    else if (arrFields[i] == "SubMan")
                    {
                        tmpFields[i] = "o.SubManType,o.SubMan,o.SubMan2,o.SubMans";
                    }
                    else if (arrFields[i] == "Summary")
                    {
                        tmpFields[i] = "o.Summary,o.IsGood,o.IsPoint";
                    }
                    else if (arrFields[i] == "ActiveName")
                    {
                        tmpFields[i] = "o.ActiveName,o.VerifyInfo";
                    }
                    else
                    {
                        tmpFields[i] = "o." + arrFields[i];
                    }
                }
            }
            string sqlFields = string.Join(",", tmpFields);
            if (!string.IsNullOrEmpty(strJoinShow))
            {
                strJoin += strJoinShow;
            }
            if (dtStart > DateTime.MinValue && dtEnd > DateTime.MinValue)
            {
                qData.SubTimeText = string.Format("{0:yyyy-MM-dd},{1:yyyy-MM-dd HH:mm:ss}", dtStart, dtEnd);
            }
            //HttpContext.Current.Response.Write(strJoin); HttpContext.Current.Response.End();
            if (!string.IsNullOrEmpty(strJoin))
            {//联合查询，先执行一次去重复查询，再根据Id查询结果
                DataOpinion[] data2 = webOpinion.GetDatas(qData, "o.Id", 1, 0, "", "distinct", strJoin);
                if (data2 == null)
                {
                    //ltInfo.Text = "<script>$(function(){ alert('未查询到“提案”'); window.history.back(-1); });</script>";
                    return null;
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
                qData2.Period = qData.Period;
                qData2.Times = qData.Times;
                qData = qData2;
            }
            //qData.Period = strPeriod;
            //qData.Times = strTimes;
            qData.QueryFields = strFields;
            qData.JoinFields = sqlFields;
            qData.JoinText = strJoinShow;
            //提案数量
            return qData;
        }
        //按委员查询
        private void queryUser()
        {
            initQuery();//初始化查询
            DataOpinion qData = getData();
            WebUser webUser = new WebUser();
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            string strOrderBy = "u2.UserSubNum " + rblQOrderBy.SelectedValue + ", u.UserCode ASC";
            DataUser[] data = webUser.GetDatas(qData, "", pageCur, pageSize, strOrderBy, "total");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    string strCommittee = data[i].Committee;
                    if (!string.IsNullOrEmpty(data[i].Committee2))
                    {
                        strCommittee += "<br/>" + data[i].Committee2;
                    }
                    data[i].Committee = strCommittee;
                }
                rpUser.DataSource = data;
                rpUser.DataBind();
                ltUserNo.Visible = false;
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = Request.Url.ToString();
                lblQueryNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
                ltQueryTotal.Text = data[0].total.ToString();
            }
        }
        #endregion
        //
        #region 编辑
        //加载信息
        private void loadData(int Id)
        {
            hfBack.Value = PublicMod.GetBackUrl();
            if (Id > 0 || (Id == 0 && myUser.Grade >= 9))
            {
                WebOp webOp = new WebOp();
                PublicMod.LoadDropDownList(ddlPeriod, webOp, "_届", config.PERIOD);
                PublicMod.LoadDropDownList(ddlTimes, webOp, "_次会议", config.TIMES);
                PublicMod.LoadRadioButtonList(rblSubType, webOp, "提案类别");
                PublicMod.LoadRadioButtonList(rblSubManType, webOp, "用户类别");

                PublicMod.LoadDropDownList(ddlApplyState, webOp, "提案办理状态");
                PublicMod.LoadDropDownList(ddlExamHostOrg, webOp, "承办单位");
                for (int i = 0; i < ddlExamHostOrg.Items.Count; i++)
                {
                    ListItem item = new ListItem(ddlExamHostOrg.Items[i].Text, ddlExamHostOrg.Items[i].Value);
                    ddlAdviseHostOrg.Items.Add(item);
                    ListItem item2 = new ListItem(ddlExamHostOrg.Items[i].Text, ddlExamHostOrg.Items[i].Value);
                    ddlAdviseHelpOrg.Items.Add(item2);
                }
                PublicMod.LoadDropDownLists(hfOrg, ddlExamHostOrg, "OpName", null, webOp);//,OpValue
                PublicMod.LoadDropDownList(ddlResultInfo, webOp, "办理结果");
                //PublicMod.LoadDropDownList(ddlResultInfo2, webOp, "办理结果");
                for (int i = 0; i < ddlResultInfo.Items.Count; i++)
                {
                    ddlResultInfo2.Items.Add(ddlResultInfo.Items[i].Value);
                }
                PublicMod.LoadDropDownList(ddlActiveName, webOp, "提案性质");
                PublicMod.LoadDropDownList(ddlVerifyInfo, webOp, "不立案原因");
                if (Id == 0)
                {
                    txtSubMan.ReadOnly = false;
                    txtSubOrg.ReadOnly = false;
                    btnEdit.Text = "新增";
                }
            }
            if (Id <= 0)
            {
                return;
            }
            DataOpinion[] data = webOpinion.GetData(Id);
            if (data != null)
            {
                HelperMain.SetDownSelected(ddlActiveName, data[0].ActiveName);
                if (data[0].ActiveName == "不立案")
                {
                    HelperMain.SetDownSelected(ddlVerifyInfo, data[0].VerifyInfo);
                }
                txtId.Text = data[0].Id.ToString();
                if (data[0].SubTime > DateTime.MinValue)
                {
                    txtSubTime.Text = data[0].SubTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                txtOpNum.Text = data[0].OpNum;//内部流水号（对接数据使用）
                txtOpNo.Text = data[0].OpNo;
                HelperMain.SetDownSelected(ddlPeriod, data[0].Period);
                HelperMain.SetDownSelected(ddlTimes, data[0].Times);
                if (data[0].TeamNum > 0)
                {
                    txtTeamNum.Text = data[0].TeamNum.ToString();
                }
                HelperMain.SetRadioSelected(rblSubType, data[0].SubType);
                HelperMain.SetRadioSelected(rblIsOpen, data[0].IsOpen);
                txtOpenInfo.Text = data[0].OpenInfo;
                txtSummary.Text = data[0].Summary;
                HelperMain.SetRadioSelected(rblSubManType, data[0].SubManType);
                if (data[0].SubManType == "委员")
                {
                    if (myUser.TrueName == "Tony")
                    {
                        txtSubMan.ReadOnly = false;
                    }
                    txtSubMan.Text = data[0].SubMan.Trim(',');
                    //string strSubMans = data[0].SubMan2.Trim(',') + "," + data[0].SubMans.Trim(',');
                    txtSubMans.Text = PublicMod.GetSubMans(data[0].Id, data[0].SubMans.Trim(','), -1);
                }
                else
                {
                    if (myUser.TrueName == "Tony")
                    {
                        txtSubOrg.ReadOnly = false;
                    }
                    txtSubOrg.Text = data[0].SubMan.Trim(',');
                    txtLinkman.Text = data[0].Linkman;
                    txtLinkmanTel.Text = data[0].LinkmanTel;
                    txtLinkmanAddress.Text = data[0].LinkmanAddress;
                    txtLinkmanZip.Text = data[0].LinkmanZip;
                }
                HelperMain.SetDownSelected(ddlAdviseHostOrg, data[0].AdviseHostOrg);
                HelperMain.SetDownSelected(ddlAdviseHelpOrg, data[0].AdviseHelpOrg);
                txtBody.Text = data[0].Body;
                hfFiles.Value = data[0].Files;

                HelperMain.SetDownSelected(ddlApplyState, data[0].ApplyState);
                HelperMain.SetDownSelected(ddlIsSign, data[0].IsSign);
                HelperMain.SetDownSelected(ddlTimeMark, data[0].TimeMark);
                HelperMain.SetDownSelected(ddlIsPoint, data[0].IsPoint);
                HelperMain.SetDownSelected(ddlIsGood, data[0].IsGood);
                HelperMain.SetDownSelected(ddlIsFeed, data[0].IsFeed);
                HelperMain.SetDownSelected(ddlReApply, data[0].ReApply);
                txtRemark.Text = data[0].Remark;
                HelperMain.SetDownSelected(ddlExamHostOrg, data[0].ExamHostOrg);
                if (!string.IsNullOrEmpty(data[0].ExamHelpOrg))
                {
                    string[] arr = data[0].ExamHelpOrg.Split(',');
                    for (int i = 0; i < arr.Count(); i++)
                    {
                        if (arr[i].IndexOf("|") > 0)
                        {
                            arr[i] = arr[i].Substring(0, arr[i].IndexOf("|"));
                        }
                    }
                    txtExamHelpOrg.Text = string.Join(",", arr);
                }
                if (data[0].PlannedDate > DateTime.MinValue)
                {
                    txtPlannedDate.Text = data[0].PlannedDate.ToString("yyyy-MM-dd");
                }
                HelperMain.SetDownSelected(ddlResultInfo, data[0].ResultInfo);
                txtResultBody.Text = data[0].ResultBody;
                HelperMain.SetDownSelected(ddlResultInfo2, data[0].ResultInfo2);
                txtResultBody2.Text = data[0].ResultBody2;
                if (data[0].RegTime > DateTime.MinValue)
                {
                    txtRegTime.Text = data[0].RegTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (data[0].ResultTime > DateTime.MinValue)
                {
                    txtResultTime.Text = data[0].ResultTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                //联名提案会签情况
                if (!string.IsNullOrEmpty(txtSubMans.Text))
                {
                    plSign.Visible = true;
                    DataOpinionSign qSign = new DataOpinionSign();
                    qSign.OpType = "提案";
                    qSign.OpId = data[0].Id;
                    WebOpinionSign webSign = new WebOpinionSign();
                    DataOpinionSign[] sData = webSign.GetDatas(qSign);
                    if (sData != null)
                    {
                        for (int i = 0; i < sData.Count(); i++)
                        {
                            if (sData[i].Active < 0)
                            {
                                sData[i].rowClass = " class='cancel' title='取消'";
                            }
                            if (sData[i].SignTime > DateTime.MinValue)
                            {
                                sData[i].SignTimeText = sData[i].SignTime.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            if (sData[i].Active > 0)
                            {
                                sData[i].ActiveName = "已签";
                            }
                            else if (sData[i].Active <= -100)
                            {
                                sData[i].ActiveName = "取消";
                            }
                            else if (sData[i].Active < 0)
                            {
                                sData[i].ActiveName = "谢绝会签";
                            }
                            else
                            {
                                if (sData[i].Overdue < DateTime.Now)
                                {
                                    sData[i].ActiveName = "会签过期";
                                    sData[i].rowClass = " class='cancel' title='会签过期'";
                                }
                                else
                                {
                                    sData[i].ActiveName = "待会签";
                                }
                            }
                        }
                        rpSignList.DataSource = sData;
                        rpSignList.DataBind();
                    }
                }
                //提案办理情况征询意见
                WebOpinionFeed webFeed = new WebOpinionFeed();
                DataOpinionFeed[] fData = webFeed.GetDatas(0, "提案", data[0].Id);
                if (fData != null)
                {
                    rpFeedList.DataSource = fData;
                    rpFeedList.DataBind();
                }
                //btnEdit
                btnDel.Visible = true;
                btnPop.Visible = true;
            }
        }
        //删除提案到回收站
        protected void btnDel_Click(object sender, EventArgs e)
        {
            int intId = (!string.IsNullOrEmpty(txtId.Text)) ? Convert.ToInt32(txtId.Text.Trim()) : 0;
            if (intId <= 0)
            {
                return;
            }
            if (webOpinion.UpdateActive(intId, "删除") > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“删除提案”成功！'); window.location.href='" + hfBack.Value + "'; });</script>";
                //更新会签状态
                WebOpinionSign webSign = new WebOpinionSign();
                DataOpinionSign qData = new DataOpinionSign();
                qData.ActiveName = "0";
                qData.OpId = intId;
                DataOpinionSign[] data = webSign.GetDatas(qData, "Id");
                if (data != null)
                {
                    for (int i = 0; i < data.Count(); i++)
                    {
                        webSign.UpdateActive(data[i].Id, -100);//取消会签
                    }
                }
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“删除提案”失败！'); window.history.back(-1); });</script>";
            }
        }
        //编辑审核
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            string strBack = hfBack.Value;
            DataOpinion data = new DataOpinion();
            data.Id = (!string.IsNullOrEmpty(txtId.Text)) ? Convert.ToInt32(txtId.Text.Trim()) : 0;
            if (data.Id < 0 || (data.Id == 0 && myUser.Grade < 9))
            {
                return;
            }
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(myUser.AdminName, 20);
            data.OpNum = HelperMain.SqlFilter(txtOpNum.Text.Trim(), 20);//内部流水号（对接数据使用）
            data.OpNo = HelperMain.SqlFilter(txtOpNo.Text.Trim(), 20);
            //data.Period = null;
            //data.Times = null;
            data.TeamNum = -1;
            data.ApplyState = HelperMain.SqlFilter(ddlApplyState.SelectedValue, 8);
            data.ActiveName = HelperMain.SqlFilter(ddlActiveName.SelectedValue.Trim(), 20);
            if (data.ActiveName == "不立案")
            {
                data.VerifyInfo = HelperMain.SqlFilter(ddlVerifyInfo.SelectedValue.Trim(), 50);
            }
            if (data.ActiveName == "立案")
            {
                data.RegTime = dtNow;
                data.RegIp = strIp;
                data.RegUser = strUser;
            }
            data.SubType = HelperMain.SqlFilter(rblSubType.SelectedValue, 20);
            data.IsSign = HelperMain.SqlFilter(ddlIsSign.SelectedValue.Trim(), 2);
            data.TimeMark = HelperMain.SqlFilter(ddlTimeMark.SelectedValue.Trim(), 4);
            data.IsOpen = HelperMain.SqlFilter(rblIsOpen.SelectedValue.Trim(), 2);
            if (data.IsOpen == "否")
            {
                data.OpenInfo = HelperMain.SqlFilter(txtOpenInfo.Text.Trim(), 20);
            }
            data.IsPoint = HelperMain.SqlFilter(ddlIsPoint.SelectedValue, 2);
            data.IsGood = HelperMain.SqlFilter(ddlIsGood.SelectedValue, 2);
            data.IsFeed = HelperMain.SqlFilter(ddlIsFeed.SelectedValue, 2);
            data.ReApply = HelperMain.SqlFilter(ddlReApply.SelectedValue, 2);
            data.SubManType = HelperMain.SqlFilter(rblSubManType.SelectedValue, 8);
            if (data.SubManType == "委员")
            {
                if (!string.IsNullOrEmpty(txtSubMan.Text))
                {
                    string strTmp = HelperMain.SqlFilter(txtSubMan.Text.Trim());
                    data.SubMan = "," + strTmp.Trim(',') + ",";
                }
                //if (!string.IsNullOrEmpty(txtSubMan2.Text))
                //{
                //    string strTmp = HelperMain.SqlFilter(txtSubMan2.Text.Trim());
                //    data.SubMan2 = "," + strTmp.Trim(',') + ",";
                //}
                if (!string.IsNullOrEmpty(txtSubMans.Text))
                {
                    string strTmp = HelperMain.SqlFilter(txtSubMans.Text.Trim());
                    data.SubMans = "," + strTmp.Trim(',') + ",";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(txtSubOrg.Text))
                {
                    string strTmp = HelperMain.SqlFilter(txtSubOrg.Text.Trim());
                    data.SubMan = "," + strTmp.Trim(',') + ",";
                }
                data.Linkman = HelperMain.SqlFilter(txtLinkman.Text.Trim(), 20);
                data.LinkmanAddress = HelperMain.SqlFilter(txtLinkmanAddress.Text.Trim());
                data.LinkmanZip = HelperMain.SqlFilter(txtLinkmanZip.Text.Trim(), 6);
                data.LinkmanTel = HelperMain.SqlFilter(txtLinkmanTel.Text.Trim(), 50);
            }
            data.Summary = HelperMain.SqlFilter(txtSummary.Text.Trim(), 100);
            string strBody = HelperMain.SqlFilter(txtBody.Text.TrimEnd());
            if (!string.IsNullOrEmpty(strBody))
            {
                strBody = HttpUtility.UrlDecode(strBody);
            }
            data.Body = strBody;
            data.Files = HelperMain.SqlFilter(hfFiles.Value.Trim('|'));
            data.Remark = HelperMain.SqlFilter(txtRemark.Text.Trim());
            data.AdviseHostOrg = HelperMain.SqlFilter(ddlAdviseHostOrg.SelectedValue.Trim());
            data.AdviseHelpOrg = HelperMain.SqlFilter(ddlAdviseHelpOrg.SelectedValue.Trim());
            data.ExamHostOrg = HelperMain.SqlFilter(ddlExamHostOrg.SelectedValue, 20);
            string strExamHelpOrg = HelperMain.SqlFilter(txtExamHelpOrg.Text.Trim());
            if (!string.IsNullOrEmpty(strExamHelpOrg))
            {
                WebOp webOp = new WebOp();
                string[] arr = strExamHelpOrg.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    DataOp[] opData = webOp.GetDatas(0, "承办单位", arr[i], "OpValue");
                    if (opData != null)
                    {
                        arr[i] = opData[0].OpValue;
                    }
                }
                data.ExamHelpOrg = string.Join(",", arr);
            }
            //data.MergeId
            if (!string.IsNullOrEmpty(txtSubTime.Text))
            {
                data.SubTime = Convert.ToDateTime(txtSubTime.Text);
            }
            //data.SubIp
            if (!string.IsNullOrEmpty(txtPlannedDate.Text))
            {
                data.PlannedDate = Convert.ToDateTime(txtPlannedDate.Text.Trim());
            }
            ////data.FilingTime
            data.ResultInfo = HelperMain.SqlFilter(ddlResultInfo.SelectedValue, 20);
            data.ResultBody = HelperMain.SqlFilter(txtResultBody.Text.Trim());
            data.ResultInfo2 = HelperMain.SqlFilter(ddlResultInfo2.SelectedValue, 20);
            data.ResultBody2 = HelperMain.SqlFilter(txtResultBody2.Text.Trim());
            if ((!string.IsNullOrEmpty(data.ResultInfo) || !string.IsNullOrEmpty(data.ResultInfo2) && data.ActiveName != "归并"))
            {
                data.ResultTime = dtNow;
                data.ResultIp = strIp;
                data.ResultUser = strUser;
            }
            data.UserId = -1;
            //data.AddTime
            //data.AddIp
            //data.AddUser
            //data.UpTime
            //data.UpIp
            //data.UpUser
            data.VerifyTime = dtNow;
            data.VerifyIp = strIp;
            data.VerifyUser = strUser;
            data.Period = HelperMain.SqlFilter(ddlPeriod.SelectedValue, 8);
            data.Times = HelperMain.SqlFilter(ddlTimes.SelectedValue, 8);
            if (data.Id == 0)
            {
                data.Id = webOpinion.Insert(data);
                if (data.Id > 0)
                {
                    ltInfo.Text = "<script>$(function(){ alert('“新增提案”成功！'); window.location.href='" + strBack + "'; });</script>";
                }
                else
                {
                    ltInfo.Text = "<script>$(function(){ alert('“新增提案”失败！'); window.history.back(-1); });</script>";
                }
            }
            else if (webOpinion.Update(data) > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + btnEdit.Text + "提案”成功！'); window.location.href='" + strBack + "'; });</script>";
                switch (data.ActiveName)
                {
                    case "立案":
                        data.SubMans = upadateSubMans(data.Id);//验证联名人是否会签，并更新
                        addScore(data);//增加积分
                        break;
                    case "待立案":
                        if (string.IsNullOrEmpty(data.OpNum) && string.IsNullOrEmpty(data.OpNo))
                        {//新增、修改流水号，新增、修改提案号，不修改会签人信息
                            new hkzx.web.cn.opinion().AddSign(myUser.AdminName, data.Id, data.SubMans, data.TimeMark);//增加会签人
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + btnEdit.Text + "提案”失败！'); window.history.back(-1); });</script>";
            }
        }
        //验证联名人是否会签，并更新
        private string upadateSubMans(int intId)
        {
            DataOpinion[] data = webOpinion.GetData(intId, "SubMan2,SubMans");
            if (data == null || string.IsNullOrEmpty(data[0].SubMans))
            {
                return "";
            }
            string strSubMan2 = data[0].SubMan2;
            string strSubMans = "";
            WebOpinionSign webSign = new WebOpinionSign();
            DataOpinionSign qSign = new DataOpinionSign();
            qSign.OpType = "提案";
            qSign.OpId = intId;
            string[] arr = data[0].SubMans.Trim(',').Split(',');
            for (int i = 0; i < arr.Count(); i++)
            {
                if (!string.IsNullOrEmpty(arr[i]))
                {
                    qSign.SignUser = arr[i];
                    DataOpinionSign[] sData = webSign.GetDatas(qSign, "Active");
                    string strTmp = "," + arr[i] + ",";
                    if (sData != null && sData[0].Active > 0)
                    {
                        strSubMans += "," + arr[i];
                        if (strSubMan2 == strTmp)
                        {
                            strSubMan2 = "";
                        }
                        else
                        {
                            strSubMan2 = strSubMan2.Replace(strTmp, ",");
                        }
                    }
                    else if (strSubMan2.IndexOf(strTmp) < 0)
                    {
                        if (!string.IsNullOrEmpty(strSubMan2))
                        {
                            strSubMan2 += ",";
                        }
                        strSubMan2 += arr[i] + ",";
                    }
                }
            }
            if (!string.IsNullOrEmpty(strSubMans))
            {
                strSubMans += ",";
            }
            if (strSubMans != data[0].SubMans || strSubMan2 != data[0].SubMan2)
            {
                string strIsSign = (!string.IsNullOrEmpty(strSubMans)) ? "是" : "";
                webOpinion.UpdateSubMans(intId, strSubMans, strSubMan2, strIsSign);
            }
            return strSubMans;
        }
        //增加积分
        private void addScore(DataOpinion data)
        {
            string TitleProperty = "";
            string TitlePoint = "";
            string TitleGood = "";
            decimal ScoreProperty = 0;
            decimal ScoreProperty2 = 0;
            decimal ScorePoint = 0;
            decimal ScoreGood = 0;
            WebScore webScore = new WebScore();
            if (data.ActiveName == "立案")
            {
                TitleProperty = "立案提案";
                DataScore[] sData = webScore.GetDatas(1, "提交提案", "", TitleProperty + "%", "score,score2");
                if (sData != null)
                {
                    ScoreProperty = sData[0].Score;
                    if (ScoreProperty == 1)
                    {
                        ScoreProperty = ScoreProperty / 2;
                    }
                    ScoreProperty2 = sData[0].Score2;
                }
            }
            if (data.IsPoint == "是")
            {
                TitlePoint = "重点专题提案";
                DataScore[] sData = webScore.GetDatas(1, "提交提案", "", "%" + TitlePoint, "score");
                if (sData != null)
                {
                    ScorePoint = sData[0].Score;
                }
            }
            if (data.IsGood == "是")
            {
                TitleGood = "优秀提案";
                DataScore[] sData = webScore.GetDatas(1, "提交提案", "", TitleGood, "score");
                if (sData != null)
                {
                    ScoreGood = sData[0].Score;
                }
            }
            string TableName = "tb_Opinion";
            DateTime dtTime = (data.SubTime > DateTime.MinValue) ? data.SubTime : DateTime.Now;
            WebUser webUser = new WebUser();
            if (!string.IsNullOrEmpty(txtSubMan.Text))
            {
                string[] arr = data.SubMan.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(arr[i]))
                    {
                        DataUser[] uData2 = webUser.GetDatas(config.PERIOD, arr[i], "Id");
                        if (uData2 != null)
                        {
                            if (!string.IsNullOrEmpty(TitleProperty))
                            {//立案提案（第一提案人）
                                PublicMod.AddScore(uData2[0].Id, TitleProperty + "（第一提案人）", ScoreProperty2, TableName, data.Id, data.VerifyIp, data.VerifyUser, dtTime);//立案
                            }
                            if (!string.IsNullOrEmpty(TitlePoint))
                            {//重点专题提案
                                PublicMod.AddScore(uData2[0].Id, TitlePoint, ScorePoint, TableName, data.Id, data.VerifyIp, data.VerifyUser, dtTime);
                            }
                            if (!string.IsNullOrEmpty(TitleGood))
                            {//优秀提案
                                PublicMod.AddScore(uData2[0].Id, TitleGood, ScoreGood, TableName, data.Id, data.VerifyIp, data.VerifyUser, dtTime);
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(txtSubMans.Text))
            {
                string[] arr = data.SubMans.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(arr[i]))
                    {
                        DataUser[] uData2 = webUser.GetDatas(config.PERIOD, arr[i], "Id");
                        if (uData2 != null)
                        {
                            if (!string.IsNullOrEmpty(TitleProperty))
                            {//立案提案（联名）
                                PublicMod.AddScore(uData2[0].Id, TitleProperty + "（联名）", ScoreProperty, TableName, data.Id, data.VerifyIp, data.VerifyUser, dtTime);//立案
                            }
                        }
                    }
                }
            }
            //if (!string.IsNullOrEmpty(txtSubMan2.Text))
            //{
            //    strSubMans += "," + HelperMain.SqlFilter(txtSubMan2.Text.Trim(), 20);
            //}
        }
        //转社情民意
        protected void btnPop_Click(object sender, EventArgs e)
        {
            int intId = (!string.IsNullOrEmpty(txtId.Text)) ? Convert.ToInt32(txtId.Text.Trim()) : 0;
            if (intId <= 0)
            {
                return;
            }
            DataOpinion[] oData = webOpinion.GetData(intId);
            if (oData == null)
            {
                return;
            }
            oData[0].ActiveName = "不立案";
            oData[0].VerifyInfo = "内容不属于本区工作范围的";
            if (!string.IsNullOrEmpty(oData[0].Remark))
            {
                if (oData[0].Remark.IndexOf("转社情民意") < 0)
                {
                    oData[0].Remark += "\n转社情民意。";
                }
            }
            else
            {
                oData[0].Remark = "转社情民意。";
            }
            DataOpinionPop data = new DataOpinionPop();
            data.OpNum = "o_" + oData[0].Id.ToString();
            WebOpinionPop webPop = new WebOpinionPop();
            if (!string.IsNullOrEmpty(data.OpNum))
            {
                DataOpinionPop[] pData = webPop.GetDatas(data.OpNum, "Id,ActiveName");
                if (pData != null)
                {
                    data.Id = pData[0].Id;
                    data.ActiveName = pData[0].ActiveName;
                }
            }
            //data.OrgName
            data.SubType = oData[0].SubType;
            //data.SubType2
            //data.IsGood
            data.IsOpen = oData[0].IsOpen;
            data.OpenInfo = oData[0].OpenInfo;
            data.SubManType = oData[0].SubManType;
            data.Party = oData[0].Party;
            data.Committee = oData[0].Committee;
            data.Subsector = oData[0].Subsector;
            data.StreetTeam = oData[0].StreetTeam;
            if (!string.IsNullOrEmpty(oData[0].SubMan))
            {
                data.SubMan = oData[0].SubMan.Trim(',');
            }
            string strSubMans = PublicMod.GetSubMans(intId, oData[0].SubMans, 1);
            if (!string.IsNullOrEmpty(strSubMans))
            {
                data.SubMans = "," + strSubMans + ",";
            }
            if (data.SubManType == "委员")
            {
                //data.Linkman = oData[0].Linkman;
                data.LinkmanInfo = "区政协委员";
                WebUser webUser = new WebUser();
                DataUser[] uData = webUser.GetData(oData[0].UserId, "Party,OrgName,Mobile");
                if (uData != null)
                {
                    data.LinkmanParty = uData[0].Party;
                    data.LinkmanOrgName = uData[0].OrgName;
                    data.LinkmanTel = uData[0].Mobile;
                }
            }
            else
            {
                data.Linkman = oData[0].Linkman;
                //data.LinkmanInfo
                string strAddress = oData[0].LinkmanAddress;
                if (!string.IsNullOrEmpty(oData[0].LinkmanZip))
                {
                    strAddress += "(" + oData[0].LinkmanZip + ")";
                }
                data.LinkmanOrgName = strAddress;
                data.LinkmanTel = oData[0].LinkmanTel;
            }
            data.Summary = oData[0].Summary;
            string strBody = oData[0].Body;
            strBody = strBody.Replace("[p]", "").Replace("[/p]", "|||");
            strBody = HelperMain.DelUbb(strBody);
            strBody = strBody.Replace("|||", "\n");
            data.Body = strBody;
            data.Files = oData[0].Files;
            data.SubTime = oData[0].SubTime;
            data.SubIp = oData[0].SubIp;
            if (string.IsNullOrEmpty(data.ActiveName))
            {
                data.ActiveName = "待审核";
            }
            //DataOpinionPop qData = new DataOpinionPop();
            //qData.Summary = data.Summary;
            //qData.AddTime = data.SubTime;
            //qData.AddUser = data.SubMan;
            //DataOpinionPop[] ckData = webPop.GetDatas(qData, "Id", 0, 1);//重复检查
            //if (ckData != null)
            //{
            //    //ltInfo.Text = "<script>$(function(){ alert('“标题”重复，不能添加！'); window.history.back(-1); });</script>";
            //    ltInfo.Text = "<script>$(function(){ alert('重复提交！'); window.history.back(-1); });</script>";
            //    return;
            //}
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(myUser.AdminName, 20);
            if (data.Id <= 0)
            {
                data.UserId = oData[0].UserId;
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = strUser;
                data.Id = webPop.Insert(data);
            }
            else
            {
                data.UpTime = dtNow;
                data.UpIp = strIp;
                data.UpUser = strUser;
                if (webPop.Update(data) <= 0)
                {
                    data.Id = 0;
                }
            }
            if (data.Id > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“提案”转“社情民意”成功！'); window.location.href='" + hfBack.Value + "'; });</script>";
                webOpinion.Update(oData[0]);//更新提案信息
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“提案”转“社情民意”失败！'); window.history.back(-1); });</script>";
            }
        }
        #endregion
        //
        #region 操作
        //删除提案到回收站（批量）
        protected void btnDels_Click(object sender, EventArgs e)
        {
            updateActive("删除");
        }
        //protected void btnIsReg_Click(object sender, EventArgs e)
        //{
        //    updateActive("立案");
        //}
        //protected void btnNoReg_Click(object sender, EventArgs e)
        //{
        //    updateActive("不立案");
        //}
        //protected void btnSub_Click(object sender, EventArgs e)
        //{
        //    updateActive("待立案");
        //}
        //protected void btnProcess_Click(object sender, EventArgs e)
        //{
        //    string strHostOrg = (!string.IsNullOrEmpty(ddlSubHostOrg.SelectedValue)) ? HelperMain.SqlFilter(ddlSubHostOrg.SelectedValue.Trim(), 20) : "";
        //    string strHelpOrg = (!string.IsNullOrEmpty(ddlSubHelpOrg.SelectedValue)) ? HelperMain.SqlFilter(ddlSubHelpOrg.SelectedValue.Trim(), 20) : "";
        //    if (!string.IsNullOrEmpty(strHostOrg) && !string.IsNullOrEmpty(strHelpOrg))
        //    {
        //        updateActive("已分理", strHostOrg, strHelpOrg);
        //    }
        //}
        //protected void btnUndertake_Click(object sender, EventArgs e)
        //{
        //    string strHostOrg = (!string.IsNullOrEmpty(ddlSubHostOrg.SelectedValue)) ? HelperMain.SqlFilter(ddlSubHostOrg.SelectedValue.Trim(), 20) : "";
        //    string strHelpOrg = (!string.IsNullOrEmpty(ddlSubHelpOrg.SelectedValue)) ? HelperMain.SqlFilter(ddlSubHelpOrg.SelectedValue.Trim(), 20) : "";
        //    if (!string.IsNullOrEmpty(strHostOrg) && !string.IsNullOrEmpty(strHelpOrg))
        //    {
        //        updateActive("已承办", strHostOrg, strHelpOrg);
        //    }
        //}
        //protected void btnResult_Click(object sender, EventArgs e)
        //{
        //    string strResult = (!string.IsNullOrEmpty(ddlSubResult.SelectedValue)) ? HelperMain.SqlFilter(ddlSubResult.SelectedValue.Trim(), 20) : "";
        //    if (!string.IsNullOrEmpty(strResult))
        //    {
        //        updateActive("已办复", "", "", strResult);
        //    }
        //}
        //处理操作
        private void updateActive(string State, string HostOrg = "", string HelpOrg = "", string Result = "")
        {
            if (myUser == null)
            {
                return;
            }
            ArrayList arrList = new ArrayList();
            for (int i = 0; i < rpQueryTbody.Items.Count; i++)
            {
                CheckBox ck = (CheckBox)rpQueryTbody.Items[i].FindControl("_ck");
                HiddenField hf = (HiddenField)rpQueryTbody.Items[i].FindControl("_id");
                if (ck.Checked)
                {
                    arrList.Add(hf.Value);
                }
            }
            if (arrList.Count <= 0)
            {
                return;
            }
            string strIp = HelperMain.GetIpPort();
            string strUser = myUser.AdminName;
            if (webOpinion.UpdateActive(arrList, State, strIp, strUser, HostOrg, HelpOrg, Result) > 0)
            {
                string strBack = Request.Url.ToString();
                ltInfo.Text = "<script>$(function(){ alert('“" + State + "”操作成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + State + "”操作失败！'); window.history.back(-1); });</script>";
            }
        }
        #endregion
        //
        #region 归并
        //获取归并提案
        protected void btnMerges_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            string strMergeId = "";
            for (int i = 0; i < rpQueryTbody.Items.Count; i++)
            {
                CheckBox ck = (CheckBox)rpQueryTbody.Items[i].FindControl("_ck");
                HiddenField hf = (HiddenField)rpQueryTbody.Items[i].FindControl("_id");
                if (ck.Checked)
                {
                    strMergeId += "," + hf.Value;
                }
            }
            if (string.IsNullOrEmpty(strMergeId))
            {
                return;
            }
            plQuery.Visible = false;
            plMerge.Visible = true;
            WebOp webOp = new WebOp();
            PublicMod.LoadDropDownList(ddlMPeriod, webOp, "_届", config.PERIOD);
            PublicMod.LoadDropDownList(ddlMTimes, webOp, "_次会议", config.TIMES);
            PublicMod.LoadRadioButtonList(rblMSubType, webOp, "提案类别");
            PublicMod.LoadRadioButtonList(rblMSubManType, webOp, "用户类别");
            txtMSubTime.Text = DateTime.Today.ToString("yyyy-MM-dd");
            hfMergeId.Value = strMergeId.Trim(',');
            DataOpinion qData = new DataOpinion();
            qData.MergeId = hfMergeId.Value;
            DataOpinion[] data = webOpinion.GetDatas(qData, "Id,SubType,Summary,SubManType,SubMan,SubMan2,SubMans,Body,Files");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    //string strSubMans = data[i].SubMan2.Trim(',') + "," + data[i].SubMans.Trim(',');//
                    data[i].SubMan = data[i].SubMan.Trim(',');
                    data[i].SubMans = data[i].SubMans.Trim(',');
                }
                rpMergeList.DataSource = data;
                rpMergeList.DataBind();
            }
        }
        //归并提案
        protected void btnMerge_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            DataOpinion data = new DataOpinion();
            data.Period = HelperMain.SqlFilter(ddlMPeriod.SelectedValue, 4);
            data.Times = HelperMain.SqlFilter(ddlMTimes.SelectedValue, 4);
            data.TeamNum = 0;
            data.IsPoint = HelperMain.SqlFilter(ddlMIsPoint.SelectedValue.Trim(), 2);
            data.IsSign = HelperMain.SqlFilter(ddlMIsSign.SelectedValue.Trim(), 2);
            data.TimeMark = HelperMain.SqlFilter(ddlMTimeMark.SelectedValue.Trim(), 4);
            data.IsOpen = HelperMain.SqlFilter(rblMIsOpen.SelectedValue.Trim(), 2);
            if (data.IsOpen == "否")
            {
                data.OpenInfo = HelperMain.SqlFilter(txtMOpenInfo.Text.Trim(), 20);
            }
            data.SubManType = HelperMain.SqlFilter(rblMSubManType.SelectedValue.Trim(), 8);
            if (data.SubManType == "委员")
            {
                //if (!string.IsNullOrEmpty(txtMSubMan2.Text))
                //{
                //    string strTmp = HelperMain.SqlFilter(txtMSubMan2.Text.Trim());
                //    data.SubMan2 = "," + strTmp.Trim(',') + ",";
                //}
                if (!string.IsNullOrEmpty(txtMSubMans.Text))
                {
                    string strTmp = HelperMain.SqlFilter(txtMSubMans.Text.Trim());
                    data.SubMans = "," + strTmp.Trim(',') + ",";
                }
            }
            else
            {
                data.Linkman = HelperMain.SqlFilter(txtMLinkman.Text.Trim(), 20);
                data.LinkmanAddress = HelperMain.SqlFilter(txtMLinkmanAddress.Text.Trim());
                data.LinkmanZip = HelperMain.SqlFilter(txtMLinkmanZip.Text.Trim(), 6);
                data.LinkmanTel = HelperMain.SqlFilter(txtMLinkmanTel.Text.Trim(), 50);
            }
            data.Summary = HelperMain.SqlFilter(txtMSummary.Text.Trim(), 100);
            data.Body = HelperMain.SqlFilter(txtMBody.Text.TrimEnd());
            data.Files = HelperMain.SqlFilter(hfMFiles.Value.Trim('|'));
            data.ActiveName = HelperMain.SqlFilter(ddlMActiveName.SelectedValue.Trim(), 20);
            data.MergeId = HelperMain.SqlFilter(hfMergeId.Value.Trim());
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(myUser.AdminName, 20);
            data.AddTime = dtNow;
            data.AddIp = strIp;
            data.AddUser = strUser;
            if (data.ActiveName == "立案")
            {
                data.RegTime = dtNow;
                data.RegIp = strIp;
                data.RegUser = strUser;
                data.VerifyTime = dtNow;
                data.VerifyIp = strIp;
                data.VerifyUser = strUser;
            }
            data.Id = webOpinion.Insert(data);
            if (data.Id > 0)
            {
                string[] arr = data.MergeId.Split(',');
                ArrayList arrList = new ArrayList();
                for (int i = 0; i < arr.Count(); i++)
                {
                    arrList.Add(arr[i]);
                }
                if (arrList.Count <= 0)
                {
                    return;
                }
                webOpinion.UpdateActive(arrList, "归并", strIp, strUser);
                string strBack = PublicMod.GetBackUrl();
                ltInfo.Text = "<script>$(function(){ alert('“" + btnMerge.Text + "”操作成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + btnMerge.Text + "”操作失败！'); window.history.back(-1); });</script>";
            }
        }
        #endregion
        //
        #region 征询意见
        protected void btnIsFeed_Click(object sender, EventArgs e)
        {
            updateFeed(btnIsFeed.Text);//"开启"
        }
        protected void btnNoFeed_Click(object sender, EventArgs e)
        {
            updateFeed(btnNoFeed.Text);//"关闭"
        }
        //开启/关闭征询意见
        private void updateFeed(string State)
        {
            string strPeriod = ddlQPeriod.SelectedValue;
            string strTimes = ddlQTimes.SelectedValue;
            if (string.IsNullOrEmpty(strPeriod) || string.IsNullOrEmpty(strTimes))
            {
                return;
            }
            string strFeed = "";
            if (State.StartsWith("开启"))
            {
                strFeed = "是";
            }
            else if (State.StartsWith("关闭"))
            {
                strFeed = "否";
            }
            else
            {
                return;
            }
            string strIp = HelperMain.GetIpPort();
            string strUser = myUser.AdminName;
            if (webOpinion.UpdateFeed(strPeriod, strTimes, strFeed) > 0)
            {
                string strBack = Request.Url.ToString();
                ltInfo.Text = "<script>$(function(){ alert('“" + State + "”操作成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + State + "”操作失败！'); window.history.back(-1); });</script>";
            }
        }
        //暂未使用
        private void updateFeed2(string State)
        {
            if (myUser == null)
            {
                return;
            }
            string IsFeed = "";
            if (State.StartsWith("需要"))
            {
                IsFeed = "是";
            }
            else if (State.StartsWith("不需要"))
            {
                IsFeed = "否";
            }
            else
            {
                return;
            }
            ArrayList arrList = new ArrayList();
            for (int i = 0; i < rpQueryTbody.Items.Count; i++)
            {
                CheckBox ck = (CheckBox)rpQueryTbody.Items[i].FindControl("_ck");
                HiddenField hf = (HiddenField)rpQueryTbody.Items[i].FindControl("_id");
                if (ck.Checked)
                {
                    arrList.Add(hf.Value);
                }
            }
            if (arrList.Count <= 0)
            {
                return;
            }
            string strIp = HelperMain.GetIpPort();
            string strUser = myUser.AdminName;
            if (webOpinion.UpdateFeed(arrList, IsFeed, strIp, strUser) > 0)
            {
                string strBack = Request.Url.ToString();
                ltInfo.Text = "<script>$(function(){ alert('“" + State + "”操作成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + State + "”操作失败！'); window.history.back(-1); });</script>";
            }
        }
        //统计
        private void countFeed()
        {
            WebOp webOp = new WebOp();
            PublicMod.LoadDropDownList(ddlFPeriod, webOp, "_届");
            PublicMod.LoadDropDownList(ddlFTimes, webOp, "_次会议");
            DataOpinion qData = new DataOpinion();
            //qData.IsFeed = "是";
            string strPeriod = "";
            if (!string.IsNullOrEmpty(Request.QueryString["Period"]))
            {
                strPeriod = HelperMain.SqlFilter(Request.QueryString["Period"].Trim(), 4);
                HelperMain.SetDownSelected(ddlFPeriod, strPeriod);
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["ac"]))
            {
                strPeriod = HelperMain.SqlFilter(ddlFPeriod.SelectedValue, 4);
            }
            else
            {
                ddlFPeriod.SelectedIndex = -1;
            }
            qData.Period = strPeriod;
            string strTimes = "";
            if (!string.IsNullOrEmpty(Request.QueryString["Times"]))
            {
                strTimes = HelperMain.SqlFilter(Request.QueryString["Times"].Trim(), 4);
                HelperMain.SetDownSelected(ddlFTimes, strTimes);
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["ac"]))
            {
                strTimes = HelperMain.SqlFilter(ddlFTimes.SelectedValue, 4);
            }
            else
            {
                ddlFTimes.SelectedIndex = -1;
            }
            qData.Times = strTimes;
            DataOpinion[] oData = webOpinion.GetDatas(qData, "Id");
            if (oData != null)
            {
                decimal deTotal = oData.Count();
                decimal deNum = 0;
                decimal deInterview1 = 0;
                decimal deInterview2 = 0;
                decimal deInterview3 = 0;
                decimal deAttitude1 = 0;
                decimal deAttitude2 = 0;
                decimal deAttitude3 = 0;
                decimal deTakeWay1 = 0;
                decimal deTakeWay2 = 0;
                decimal deTakeWay3 = 0;
                decimal deTakeWay4 = 0;
                decimal dePertinence1 = 0;
                decimal dePertinence2 = 0;
                decimal dePertinence3 = 0;
                decimal deResult1 = 0;
                decimal deResult2 = 0;
                decimal deResult3 = 0;
                decimal deResult4 = 0;
                WebOpinionFeed webFeed = new WebOpinionFeed();
                for (int i = 0; i < oData.Count(); i++)
                {
                    DataOpinionFeed qFeed = new DataOpinionFeed();
                    qFeed.OpType = "提案";
                    qFeed.OpId = oData[i].Id;
                    qFeed.Active = 1;
                    DataOpinionFeed[] data = webFeed.GetDatas(qFeed, "");
                    if (data != null)
                    {
                        deNum += data.Count();
                        for (int j = 0; j < data.Count(); j++)
                        {
                            switch (data[j].Interview)
                            {
                                case "已走访":
                                    deInterview1++;
                                    break;
                                case "委员本人提出不需要走访":
                                    deInterview2++;
                                    break;
                                case "未走访":
                                    deInterview3++;
                                    break;
                                default:
                                    break;
                            }
                            switch (data[j].Attitude)
                            {
                                case "满意":
                                    deAttitude1++;
                                    break;
                                case "理解":
                                    deAttitude2++;
                                    break;
                                case "不满意":
                                    deAttitude3++;
                                    break;
                                default:
                                    break;
                            }
                            switch (data[j].TakeWay)
                            {
                                case "走访":
                                    deTakeWay1++;
                                    break;
                                case "电话":
                                    deTakeWay2++;
                                    break;
                                case "其他":
                                    deTakeWay3++;
                                    break;
                                case "未联系":
                                    deTakeWay4++;
                                    break;
                                default:
                                    break;
                            }
                            switch (data[j].Pertinence)
                            {
                                case "针对":
                                    dePertinence1++;
                                    break;
                                case "基本针对":
                                    dePertinence2++;
                                    break;
                                case "未针对":
                                    dePertinence3++;
                                    break;
                                default:
                                    break;
                            }
                            switch (data[j].Result)
                            {
                                case "同意":
                                    deResult1++;
                                    break;
                                case "理解":
                                    deResult2++;
                                    break;
                                case "保留":
                                    deResult3++;
                                    break;
                                case "不同意":
                                    deResult4++;
                                    break;
                                default:
                                    break;
                            }
                        }
                        ltCount1.Text = deTotal.ToString();
                        ltCount2.Text = deNum.ToString();
                        if (deNum > 0)
                        {
                            ltCount20.Text = (deNum / deTotal * 100).ToString("n2") + "%";
                            ltInterview10.Text = (deInterview1 / deNum * 100).ToString("n2") + "%";
                            ltInterview1.Text = deInterview1.ToString();
                            ltInterview20.Text = (deInterview2 / deNum * 100).ToString("n2") + "%";
                            ltInterview2.Text = deInterview2.ToString();
                            ltInterview30.Text = (deInterview3 / deNum * 100).ToString("n2") + "%";
                            ltInterview3.Text = deInterview3.ToString();
                            ltAttitude10.Text = (deAttitude1 / deNum * 100).ToString("n2") + "%";
                            ltAttitude1.Text = deAttitude1.ToString();
                            ltAttitude20.Text = (deAttitude2 / deNum * 100).ToString("n2") + "%";
                            ltAttitude2.Text = deAttitude2.ToString();
                            ltAttitude30.Text = (deAttitude3 / deNum * 100).ToString("n2") + "%";
                            ltAttitude3.Text = deAttitude3.ToString();
                            ltTakeWay10.Text = (deTakeWay1 / deNum * 100).ToString("n2") + "%";
                            ltTakeWay1.Text = deTakeWay1.ToString();
                            ltTakeWay20.Text = (deTakeWay2 / deNum * 100).ToString("n2") + "%";
                            ltTakeWay2.Text = deTakeWay2.ToString();
                            ltTakeWay30.Text = (deTakeWay3 / deNum * 100).ToString("n2") + "%";
                            ltTakeWay3.Text = deTakeWay3.ToString();
                            ltTakeWay40.Text = (deTakeWay4 / deNum * 100).ToString("n2") + "%";
                            ltTakeWay4.Text = deTakeWay4.ToString();
                            ltPertinence10.Text = (dePertinence1 / deNum * 100).ToString("n2") + "%";
                            ltPertinence1.Text = dePertinence1.ToString();
                            ltPertinence20.Text = (dePertinence2 / deNum * 100).ToString("n2") + "%";
                            ltPertinence2.Text = dePertinence2.ToString();
                            ltPertinence30.Text = (dePertinence3 / deNum * 100).ToString("n2") + "%";
                            ltPertinence3.Text = dePertinence3.ToString();
                            ltResult10.Text = (deResult1 / deNum * 100).ToString("n2") + "%";
                            ltResult1.Text = deResult1.ToString();
                            ltResult20.Text = (deResult2 / deNum * 100).ToString("n2") + "%";
                            ltResult2.Text = deResult2.ToString();
                            ltResult30.Text = (deResult3 / deNum * 100).ToString("n2") + "%";
                            ltResult3.Text = deResult3.ToString();
                            ltResult40.Text = (deResult4 / deNum * 100).ToString("n2") + "%";
                            ltResult4.Text = deResult4.ToString();
                        }
                    }
                }
            }
        }
        #endregion
        //
    }
}