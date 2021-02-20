using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mod.main;
using hkzx.db;
using hkzx.user;

namespace hkzx.web.admin
{
    public partial class perform : System.Web.UI.Page
    {
        private const string SIGNDESKEY = config.SIGNDEKEY;
        private const string SIGNDESIV = config.SIGNDESIV;
        private DataAdmin myUser = null;
        private WebPerform webPerform = new WebPerform();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperAdmin.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
            {
                //Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            if (myUser.Powers.IndexOf("alls") < 0 && myUser.Powers.IndexOf("perform") < 0)
            {
                Response.Redirect("./");
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.Powers = myUser.Powers;
            plNav.Visible = true;
            hfAdminName.Value = myUser.AdminName;
            if (!IsPostBack)
            {
                string strTitle = "";
                switch (Request.QueryString["ac"])
                {
                    case "roll":
                        strTitle = "会议/活动通知 - 名单";
                        plRoll.Visible = true;
                        listRoll(Convert.ToInt32(Request.QueryString["id"]));
                        break;
                    case "feed":
                        if (myUser.AdminName == "Tony")
                        {
                            plSms.Visible = true;
                        }
                        strTitle = "会议/活动通知 - 反馈情况";
                        plFeed.Visible = true;
                        listFeed(Convert.ToInt32(Request.QueryString["id"]));
                        break;
                    default:
                        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                        {
                            hfBack.Value = PublicMod.GetBackUrl();
                            plEdit.Visible = true;
                            loadData(Convert.ToInt32(Request.QueryString["id"]));
                            strTitle = ltEditTitle.Text;
                        }
                        else
                        {
                            strTitle = "会议/活动通知查询";
                            plQuery.Visible = true;
                            queryData();
                        }
                        break;
                }
                Header.Title += " - " + strTitle;
            }
        }
        //
        #region 查询
        //首页列表
        public void MyList(string ActiveName, Repeater rpList, Literal ltNo)
        {
            DataPerform qData = new DataPerform();
            qData.ActiveName = ActiveName;
            qData.StartTimeText = DateTime.Now.ToString("yyyy-MM-dd") + ",";
            listData("", qData, rpList, ltNo);
        }
        //查询列表
        protected void queryData()
        {
            WebOp webOp = new WebOp();
            PublicMod.LoadDropDownLists(hfOrg, ddlQOrgType, "OpName", null, webOp);
            PublicMod.LoadPerformType(ddlSubType, "参加会议,参加调研和视察,委员专题活动");
            PublicMod.LoadDropDownLists(hfSubType, ddlSubType, "OpName", null, webOp);
            PublicMod.LoadDropDownList(ddlQHaveBus, webOp, "履职活动用车", "_");

            DataPerform qData = new DataPerform();
            if (!string.IsNullOrEmpty(Request.QueryString["OrgName"]))
            {
                qData.OrgName = HelperMain.SqlFilter(Request.QueryString["OrgName"].Trim(), 20);
                txtQOrgName.Text = qData.OrgName;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["SubType"]))
            {
                qData.SubType = HelperMain.SqlFilter(Request.QueryString["SubType"].Trim(), 100);
                txtQSubType.Text = qData.SubType;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Title"]))
            {
                qData.Title = "%" + HelperMain.SqlFilter(Request.QueryString["Title"].Trim(), 50) + "%";
                txtQTitle.Text = qData.Title.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["PerformSite"]))
            {
                qData.PerformSite = "%" + HelperMain.SqlFilter(Request.QueryString["PerformSite"].Trim(), 100) + "%";
                txtQPerformSite.Text = qData.PerformSite.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Leaders"]))
            {
                qData.Leaders = "%" + HelperMain.SqlFilter(Request.QueryString["Leaders"].Trim(), 20) + "%";
                txtQLeaders.Text = qData.Leaders.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Attendees"]))
            {
                qData.Attendees = "%" + HelperMain.SqlFilter(Request.QueryString["Attendees"].Trim(), 20) + "%";
                txtQAttendees.Text = qData.Attendees.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["HaveBus"]))
            {
                qData.HaveBus = HelperMain.SqlFilter(Request.QueryString["HaveBus"].Trim(), 8);
                HelperMain.SetDownSelected(ddlQHaveBus, qData.HaveBus);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["IsMust"]))
            {
                qData.IsMust = HelperMain.SqlFilter(Request.QueryString["IsMust"].Trim(), 4);
                HelperMain.SetDownSelected(ddlQIsMust, qData.IsMust);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["StartTime"]) && Request.QueryString["StartTime"].IndexOf(",") >= 0)
            {
                string strTime = Request.QueryString["StartTime"].Trim();
                string strTime1 = strTime.Substring(0, strTime.IndexOf(","));
                string strTime2 = strTime.Substring(strTime.IndexOf(",") + 1);
                txtQStartTime1.Text = HelperMain.SqlFilter(strTime1.Trim(), 10);
                txtQStartTime2.Text = HelperMain.SqlFilter(strTime2.Trim(), 10);
                if (txtQStartTime1.Text != "" || txtQStartTime2.Text != "")
                {
                    qData.StartTimeText = txtQStartTime1.Text + "," + txtQStartTime2.Text;
                }
            }
            else if (Request.QueryString["ac"] != "query")
            {
                txtQStartTime1.Text = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
                qData.StartTimeText = txtQStartTime1.Text + ",";
            }
            if (!string.IsNullOrEmpty(Request.QueryString["EndTime"]) && Request.QueryString["EndTime"].IndexOf(",") >= 0)
            {
                string strTime = Request.QueryString["EndTime"].Trim();
                string strTime1 = strTime.Substring(0, strTime.IndexOf(","));
                string strTime2 = strTime.Substring(strTime.IndexOf(",") + 1);
                txtQEndTime1.Text = HelperMain.SqlFilter(strTime1.Trim(), 10);
                txtQEndTime2.Text = HelperMain.SqlFilter(strTime2.Trim(), 10);
                if (txtQEndTime1.Text != "" || txtQEndTime2.Text != "")
                {
                    qData.EndTimeText = txtQEndTime1.Text + "," + txtQEndTime2.Text;
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Active"]))
            {
                qData.ActiveName = HelperMain.SqlFilter(Request.QueryString["Active"].Trim(), 40);
            }
            else// if (Request.QueryString["ac"] != "query")
            {
                qData.ActiveName = "发布,履职关闭,提交申请,退回,导入";//,暂存,删除
            }
            HelperMain.SetCheckSelected(cblQActive, qData.ActiveName);
            listData(myUser.AdminName, qData, rpQueryList, ltQueryNo, lblQueryNav, ltQueryTotal);
        }
        //加载列表
        private void listData(string strUser, DataPerform qData, Repeater rpList, Literal ltNo, Label lblNav = null, Literal ltTotal = null)
        {
            int pageCur = 1;
            if (lblNav != null && !string.IsNullOrEmpty(Request.QueryString["page"]))
            {
                pageCur = Convert.ToInt32(Request.QueryString["page"]);
            }
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            string strOrder = "StartTime DESC, EndTime DESC, OverTime DESC";//"UpTime DESC, AddTime DESC"
            qData.AdminName = strUser;
            DataPerform[] data = webPerform.GetDatas(qData, "", pageCur, pageSize, strOrder, "total");
            if (data != null)
            {
                WebPerformFeed webFeed = new WebPerformFeed();
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
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
                    if (!string.IsNullOrEmpty(data[i].SubType) && data[i].SubType.IndexOf("-") > 0)
                    {
                        data[i].SubType = data[i].SubType.Replace(",", "<br/>");
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
                                data[i].rowClass = " class='save' title='提交申请'";
                            }
                            data[i].StateName = "查看";
                            break;
                        case "暂存":
                            data[i].rowClass = " class='save' title='暂存'";
                            data[i].StateName = "选取";
                            break;
                        case "退回":
                            string strBack = (!string.IsNullOrEmpty(data[i].VerifyInfo)) ? strBack = " [" + data[i].VerifyInfo + "]" : "";
                            data[i].rowClass = " class='save' title='退回" + strBack + "'";
                            data[i].StateName = "选取";
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
                            if (data[i].EndTime < DateTime.Now)
                            {
                                data[i].rowClass = " class='del' title='会议/活动已结束'";
                                data[i].ActiveName += "<br/>已结束";
                            }
                            else if (data[i].StartTime < DateTime.Now)
                            {
                                data[i].rowClass = " class='cancel' title='会议/活动进行中'";
                                data[i].ActiveName += "<br/>进行中";
                            }
                            else if (data[i].OverTime < DateTime.Now)
                            {
                                data[i].rowClass = " class='cancel' title='会议/活动已报名截止'";
                                data[i].ActiveName += "<br/>报名截止";
                            }
                            //else
                            //{
                            //    data[i].ActiveName = "正常";
                            //}
                            data[i].StateName = "查看";
                            break;
                    }
                    DataPerformFeed[] fData = webFeed.GetDatas("已签到,已出席", data[i].Id, 0, "", "Id");//已签到,已出席
                    if (fData != null)
                    {
                        data[i].FeedNum = fData.Count();
                    }
                    string strAttendees = data[i].Attendees.Trim(',');
                    if (!string.IsNullOrEmpty(strAttendees))
                    {
                        string[] arr = strAttendees.Split(',');
                        data[i].AttendeesNum = arr.Count();
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
                    string lnk = Request.Url.ToString();
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
        #region 编辑
        //加载信息
        private void loadData(int Id)
        {
            WebOp webOp = new WebOp();
            PublicMod.LoadDropDownLists(hfOrg, ddlOrgType, "OpName", null, webOp);
            PublicMod.LoadPerformType(ddlSubType, "参加会议,参加调研和视察,委员专题活动");
            PublicMod.LoadDropDownLists(hfSubType, ddlSubType, "OpName", null, webOp);
            PublicMod.LoadRadioButtonList(rblHaveBus, webOp, "履职活动用车");
            PublicMod.LoadCheckBoxList(cblHaveDinner, webOp, "履职活动其他");
            for (int i = 0; i < config.arrSignDesk.GetLength(0); i++)
            {
                //cblSignDesk.Items.Add((i + 1).ToString().PadLeft(2, '0'));
                ListItem item = new ListItem(config.arrSignDesk[i, 1], config.arrSignDesk[i, 0]);
                cblSignDesk.Items.Add(item);
            }

            if (Id <= 0)
            {
                txtAddTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                txtAddIp.Text = HelperMain.GetIp();
                txtAddUser.Text = myUser.AdminName;
                return;
            }
            DataPerform[] data = webPerform.GetData(Id);
            if (data != null)
            {
                txtId.Text = data[0].Id.ToString();
                string strOrgName = data[0].OrgName;
                if (!string.IsNullOrEmpty(strOrgName) && strOrgName.IndexOf(",") > 0)
                {
                    txtOrgName.Text = strOrgName.Substring(0, strOrgName.IndexOf(","));
                    txtOrgName2.Text = strOrgName.Substring(strOrgName.IndexOf(",") + 1);
                }
                else
                {
                    txtOrgName.Text = strOrgName;
                }
                string strSubType = data[0].SubType;
                if (!string.IsNullOrEmpty(strSubType) && strSubType.IndexOf(",") > 0)
                {
                    txtSubType.Text = strSubType.Substring(0, strSubType.IndexOf(","));
                    txtSubType2.Text = strSubType.Substring(strSubType.IndexOf(",") + 1);
                }
                else
                {
                    txtSubType.Text = strSubType;
                }
                txtIsMust.Text = data[0].IsMust;
                txtLinkman.Text = data[0].Linkman;
                txtLinkmanTel.Text = data[0].LinkmanTel;
                txtTitle.Text = data[0].Title;
                txtPerformSite.Text = data[0].PerformSite;
                txtOverTime.Text = data[0].OverTime.ToString("yyyy-MM-dd HH:mm");
                txtStartTime.Text = data[0].StartTime.ToString("yyyy-MM-dd HH:mm");
                txtEndTime.Text = data[0].EndTime.ToString("yyyy-MM-dd HH:mm");
                HelperMain.SetRadioSelected(rblHaveBus, data[0].HaveBus);
                HelperMain.SetCheckSelected(cblHaveDinner, data[0].HaveDinner);
                txtLeaders.Text = data[0].Leaders.Trim(',');
                txtAttendees.Text = data[0].Attendees.Trim(',');
                txtBody.Text = data[0].Body;
                if (!string.IsNullOrEmpty(data[0].Files))
                {
                    hfFiles.Value = data[0].Files;
                }
                txtRemark.Text = data[0].Remark;
                txtAddTime.Text = data[0].AddTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                txtAddIp.Text = data[0].AddIp;
                txtAddUser.Text = data[0].AddUser;
                if (!string.IsNullOrEmpty(data[0].UpIp) || !string.IsNullOrEmpty(data[0].UpUser))
                {
                    txtUpTime.Text = data[0].UpTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    txtUpIp.Text = data[0].UpIp;
                    txtUpUser.Text = data[0].UpUser;
                }
                HelperMain.SetCheckSelected(cblSignDesk, data[0].SignDesk.Trim(','));
                if (data[0].SignTime > DateTime.MinValue)
                {
                    txtSignTime.Text = data[0].SignTime.ToString("yyyy-MM-dd HH:mm");
                }
                ltConfirm.Visible = true;
                rblConfirm.Visible = true;
                btnEdit.Text = "更新";
                ltEditTitle.Text = ltEditTitle.Text.Replace("发布", "更新");
                btnDel.Visible = true;
            }
        }
        //提交数据
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            editData("发布");
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
            if (myUser == null)
            {
                return;
            }
            DataPerform data = new DataPerform();
            data.Id = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
            string strUser = (myUser != null) ? HelperMain.SqlFilter(myUser.AdminName, 20) : HelperMain.SqlFilter(hfAdminName.Value, 20);
            bool isUpdate = false;
            if (ActiveName == "删除")
            {
                if (data.Id > 0)
                {
                    delPerform(strUser, data.Id);//删除活动、取消积分
                    data.Id = webPerform.UpdateActive(data.Id, ActiveName);
                }
                else
                {
                    return;
                }
            }
            else
            {
                DateTime dtNow = DateTime.Now;
                //data.Id = Convert.ToInt32(txtId.Text);
                data.OrgName = HelperMain.SqlFilter(txtOrgName.Text.Trim());
                if (!string.IsNullOrEmpty(txtOrgName2.Text))
                {
                    data.OrgName += "," + HelperMain.SqlFilter(txtOrgName2.Text.Trim());
                }
                data.Linkman = HelperMain.SqlFilter(txtLinkman.Text.Trim(), 20);
                data.LinkmanTel = HelperMain.SqlFilter(txtLinkmanTel.Text.Trim(), 50);
                data.SubType = HelperMain.SqlFilter(txtSubType.Text.Trim());
                if (!string.IsNullOrEmpty(txtSubType2.Text))
                {
                    data.SubType += "," + HelperMain.SqlFilter(txtSubType2.Text.Trim());
                }
                //data.IsMust = HelperMain.SqlFilter(rblIsMust.SelectedValue.Trim(), 4);
                data.IsMust = HelperMain.SqlFilter(txtIsMust.Text.Trim(), 4);
                data.Title = HelperMain.SqlFilter(txtTitle.Text.Trim(), 50);
                data.StartTime = (!string.IsNullOrEmpty(txtStartTime.Text)) ? Convert.ToDateTime(txtStartTime.Text.Trim()) : dtNow;
                data.EndTime = (!string.IsNullOrEmpty(txtEndTime.Text)) ? Convert.ToDateTime(txtEndTime.Text.Trim()) : data.StartTime;
                data.OverTime = (!string.IsNullOrEmpty(txtOverTime.Text)) ? Convert.ToDateTime(txtOverTime.Text.Trim()) : data.StartTime;
                data.PerformSite = HelperMain.SqlFilter(txtPerformSite.Text.Trim(), 100);
                data.Body = HelperMain.SqlFilter(txtBody.Text.Trim());
                data.Files = HelperMain.SqlFilter(hfFiles.Value.Trim('|'));
                if (!string.IsNullOrEmpty(txtLeaders.Text.Trim()))
                {
                    data.Leaders = "," + HelperMain.SqlFilter(txtLeaders.Text.Trim()) + ",";
                }
                if (!string.IsNullOrEmpty(txtAttendees.Text))
                {
                    data.Attendees = "," + HelperMain.SqlFilter(txtAttendees.Text.Trim()) + ",";
                }
                data.HaveBus = HelperMain.SqlFilter(rblHaveBus.SelectedValue.Trim(), 8);
                data.HaveDinner = HelperMain.SqlFilter(HelperMain.GetCheckSelected(cblHaveDinner), 20);
                data.Remark = HelperMain.SqlFilter(txtRemark.Text.Trim());
                data.ActiveName = ActiveName;
                string strSignDesk = HelperMain.GetCheckSelected(cblSignDesk);
                if (!string.IsNullOrEmpty(strSignDesk))
                {
                    data.SignDesk = "," + strSignDesk + ",";
                }
                data.SignTime = (!string.IsNullOrEmpty(txtSignTime.Text)) ? Convert.ToDateTime(txtSignTime.Text.Trim()) : data.StartTime;
                string strIp = HelperMain.GetIpPort();
                if (data.Id <= 0)
                {
                    data.AddTime = dtNow;
                    data.AddIp = strIp;
                    data.AddUser = strUser;
                    data.Id = webPerform.Insert(data);
                }
                else
                {
                    if (!rblConfirm.Visible)
                    {
                        isUpdate = true;
                    }
                    else if (rblConfirm.SelectedValue == "是")
                    {
                        DataPerform[] pData = webPerform.GetData(data.Id, "StartTime,EndTime,PerformSite");
                        if (pData != null)
                        {//会议起止时间、地点改动，委员需要重新确认。
                            if (pData[0].StartTime != data.StartTime || pData[0].EndTime != data.EndTime || pData[0].PerformSite != data.PerformSite)
                            {
                                isUpdate = true;
                            }
                        }
                    }
                    data.UpTime = dtNow;
                    data.UpIp = strIp;
                    data.UpUser = strUser;
                    if (webPerform.Update(data) <= 0)
                    {
                        data.Id = -1;
                    }
                    ActiveName = (ActiveName == "发布") ? btnEdit.Text : btnSave.Text;
                }
            }
            if (data.Id > 0)
            {
                string strBack = hfBack.Value;
                ltInfo.Text = "<script>$(function(){ alert('“" + ActiveName + "通知”成功！'); window.location.href='" + strBack + "'; });</script>";
                addSignUser(data.Id, data.IsMust, data.Attendees, data.SubType, isUpdate);
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ActiveName + "通知”失败！'); window.history.back(-1); });</script>";
            }
        }
        #endregion
        //
        #region 操作
        //履职关闭
        protected void btnFinish_Click(object sender, EventArgs e)
        {
            updateActive("履职关闭");
        }
        //发布
        protected void btnPass_Click(object sender, EventArgs e)
        {
            updateActive("发布");
        }
        //退回
        protected void btnBack_Click(object sender, EventArgs e)
        {
            string VerifyInfo = (!string.IsNullOrEmpty(hfVerifyInfo.Value)) ? HelperMain.SqlFilter(hfVerifyInfo.Value.Trim()) : "";
            if (!string.IsNullOrEmpty(VerifyInfo))
            {
                updateActive("退回", VerifyInfo);
            }
        }
        //删除
        protected void btnDels_Click(object sender, EventArgs e)
        {
            string VerifyInfo = (!string.IsNullOrEmpty(hfVerifyInfo.Value)) ? HelperMain.SqlFilter(hfVerifyInfo.Value.Trim()) : "";
            if (!string.IsNullOrEmpty(VerifyInfo))
            {
                updateActive("删除", VerifyInfo);
            }
        }
        //处理操作
        private void updateActive(string ActiveName, string VerifyInfo = "")
        {
            if (myUser == null)
            {
                return;
            }
            ArrayList arrList = new ArrayList();
            for (int i = 0; i < rpQueryList.Items.Count; i++)
            {
                CheckBox ck = (CheckBox)rpQueryList.Items[i].FindControl("_ck");
                HiddenField hf = (HiddenField)rpQueryList.Items[i].FindControl("_id");
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
            string strUser = (myUser != null) ? HelperMain.SqlFilter(myUser.AdminName, 20) : HelperMain.SqlFilter(hfAdminName.Value, 20);
            int intId = 0;
            if (ActiveName == "履职关闭")
            {
                for (int i = 0; i < arrList.Count; i++)
                {
                    ClosePerform(strUser, Convert.ToInt32(arrList[i]));
                }
                intId = webPerform.UpdateActive(arrList, ActiveName, "", "", "", strIp, strUser);
            }
            else
            {
                if (ActiveName == "发布")
                {
                    for (int i = 0; i < arrList.Count; i++)
                    {
                        addSignUser(Convert.ToInt32(arrList[i]));
                    }
                }
                intId = webPerform.UpdateActive(arrList, ActiveName, VerifyInfo, strIp, strUser);
            }
            if (intId > 0)
            {
                string strBack = Request.Url.ToString();
                ltInfo.Text = "<script>$(function(){ alert('“" + ActiveName + "”操作成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ActiveName + "”操作失败！'); window.history.back(-1); });</script>";
            }
        }
        //生成签到名单表
        private void addSignUser(int PerformId, string IsMust = "", string Attendees = "", string PerformSubType = "", bool isUpdate = true)
        {
            if (string.IsNullOrEmpty(Attendees))
            {
                DataPerform[] pData = webPerform.GetData(PerformId, "IsMust,Attendees,SubType");
                if (pData != null)
                {
                    IsMust = pData[0].IsMust;
                    Attendees = pData[0].Attendees;
                    PerformSubType = pData[0].SubType;
                }
            }
            WebPerformFeed webFeed = new WebPerformFeed();
            DataPerformFeed[] delData = webFeed.GetDatas("", PerformId, 0, "", "Id,SignMan,ActiveName");
            if (delData != null)
            {
                ArrayList arrList = new ArrayList();
                for (int i = 0; i < delData.Count(); i++)
                {
                    if (Attendees.IndexOf("," + delData[i].SignMan + ",") < 0 && delData[i].ActiveName != "取消")
                    {
                        arrList.Add(delData[i].Id.ToString());
                    }
                }
                if (arrList.Count > 0)
                {
                    webFeed.UpdateActive(arrList, "取消");
                }
            }
            if (string.IsNullOrEmpty(Attendees))//string.IsNullOrEmpty(IsMust) || IsMust.IndexOf("必须") < 0 || 
            {
                return;
            }
            string[] arr = Attendees.Trim(',').Split(',');
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = (myUser != null) ? HelperMain.SqlFilter(myUser.AdminName, 20) : HelperMain.SqlFilter(hfAdminName.Value, 20);
            WebUser webUser = new WebUser();
            for (int i = 0; i < arr.Count(); i++)
            {
                if (!string.IsNullOrEmpty(arr[i]))
                {
                    DataPerformFeed data = new DataPerformFeed();
                    data.PerformId = PerformId;
                    data.SignMan = arr[i];
                    if (!string.IsNullOrEmpty(IsMust) && IsMust.IndexOf("必须") >= 0)
                    {
                        data.IsMust = "必须参加";
                    }
                    else
                    {
                        data.IsMust = "报名参加";
                    }
                    DataUser[] uData = webUser.GetDatas(config.PERIOD, data.SignMan, "Id,Role");
                    if (uData != null)
                    {
                        data.UserId = uData[0].Id;
                        if (PerformSubType == "常委会议" && uData[0].Role.IndexOf("常委") >= 0)
                        {
                            data.SignManType = "常委";
                        }
                    }
                    DataPerformFeed[] ckData = webFeed.GetDatas("", data.PerformId, 0, data.SignMan, "Id,ActiveName,Remark");
                    if (ckData != null)
                    {
                        data.Id = ckData[0].Id;
                        if (isUpdate || ckData[0].ActiveName == "取消")
                        {
                            if (ckData[0].ActiveName != "待确认")
                            {
                                data.ActiveName = "待确认";//"需重新确认";
                                if (!string.IsNullOrEmpty(ckData[0].Remark))
                                {
                                    data.Remark = ckData[0].Remark + " ";
                                }
                                data.Remark += ckData[0].ActiveName;
                            }
                        }
                        //else if (string.IsNullOrEmpty(ckData[0].ActiveName))
                        //{
                        //    data.ActiveName = "待确认";
                        //}
                        data.UpTime = dtNow;
                        data.UpIp = strIp;
                        data.UpUser = strUser;
                        data.Id = webFeed.Update(data);
                    }
                    else// if (!string.IsNullOrEmpty(IsMust) && IsMust.IndexOf("必须") >= 0)
                    {
                        //data.IsMust = "必须参加";
                        data.ActiveName = "待确认";
                        data.AddTime = dtNow;
                        data.AddIp = strIp;
                        data.AddUser = strUser;
                        data.Id = webFeed.Insert(data);
                    }
                }
            }
        }
        //履职关闭、结算扣分（人脸签到时，会直接计分）
        public void ClosePerform(string strUser, int PerformId)
        {
            DataPerform[] pData = webPerform.GetData(PerformId, "SubType,IsMust,EndTime,IsMust,ActiveName");
            if (pData == null || pData[0].ActiveName == "履职关闭")// || pData[0].EndTime < DateTime.Now
            {
                return;
            }
            DateTime dtTime = pData[0].EndTime;
            WebScore webScore = new WebScore();
            WebUserScore webUserScore = new WebUserScore();
            decimal deAddScore = 0;
            decimal deAddScore2 = 0;
            decimal deDeScore = 0;
            decimal deDeScore2 = 0;
            string PerformSubType = pData[0].SubType;
            if (!string.IsNullOrEmpty(PerformSubType))
            {
                if (PerformSubType.IndexOf("-") > 0)
                {
                    PerformSubType = PerformSubType.Substring(0, PerformSubType.IndexOf("-"));
                }
                DataScore[] sData = webScore.GetDatas(1, "", "", PerformSubType, "Score,Score2,Title");//取活动加分项
                if (sData != null)
                {
                    deAddScore = sData[0].Score;
                    deAddScore2 = sData[0].Score2;
                }
            }
            string strAddTitle = "出席-" + pData[0].SubType;
            string strDeTitle = (pData[0].IsMust.IndexOf("必须") >= 0) ? "必须参加的会议" : "其他会议及活动";
            DataScore[] sData2 = webScore.GetDatas(1, strDeTitle, "", "", "Score,Title");//取扣分项
            if (sData2 != null)
            {
                for (int i = 0; i < sData2.Count(); i++)
                {
                    if (sData2[i].Title.IndexOf("未提交") >= 0)
                    {
                        deDeScore2 = sData2[i].Score;//-2
                    }
                    else
                    {
                        deDeScore = sData2[i].Score;//-1
                    }
                }
            }
            string strTableName = "tb_Perform_Feed";
            string strIp = HelperMain.GetIpPort();
            //string strUser = myUser.AdminName;
            WebPerformFeed webFeed = new WebPerformFeed();
            DataPerformFeed[] data = webFeed.GetDatas("", PerformId, 0, "", "Id,UserId,SignManType,IsMust,ActiveName");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    if (data[i].ActiveName.IndexOf("已签到") >= 0 || data[i].ActiveName == "已出席")
                    {//系统建立活动状态：已签到，导入活动出席状态：已出席
                        if (data[i].SignTime > DateTime.MinValue)
                        {
                            dtTime = data[i].SignTime;
                        }
                        if (pData[0].SubType == "常委会议")
                        {
                            if (data[i].SignManType == "常委")
                            {
                                PublicMod.AddScore(data[i].UserId, strAddTitle + "-常委", deAddScore2, strTableName, data[i].Id, strIp, strUser, dtTime);
                            }
                            else
                            {
                                PublicMod.AddScore(data[i].UserId, strAddTitle + "-列席", deAddScore, strTableName, data[i].Id, strIp, strUser, dtTime);
                            }
                        }
                        //else if (pData[0].SubType.IndexOf("调研课题") > 0)
                        //{
                        //    if (data[i].SignManType == "执笔人")
                        //    {
                        //        PublicMod.AddScore(data[i].UserId, strAddTitle + "-执笔人", deAddScore2, strTableName, data[i].Id, strIp, strUser);
                        //    }
                        //    else
                        //    {
                        //        PublicMod.AddScore(data[i].UserId, strAddTitle + "-课题组", deAddScore, strTableName, data[i].Id, strIp, strUser);
                        //    }
                        //}
                        else if (pData[0].SubType.IndexOf("讲堂") > 0)
                        {
                            if (data[i].SignManType == "主讲人")
                            {
                                PublicMod.AddScore(data[i].UserId, strAddTitle + "-主讲人", deAddScore2, strTableName, data[i].Id, strIp, strUser, dtTime);
                            }
                            else
                            {
                                PublicMod.AddScore(data[i].UserId, strAddTitle + "-出席", deAddScore, strTableName, data[i].Id, strIp, strUser, dtTime);
                            }
                        }
                        else
                        {
                            if (data[i].IsMust != "必须参加" && data[i].ActiveName == "已签到(未报名)")
                            {
                                data[i].UserId = data[i].UserId / 2;//已签到(未报名)
                            }
                            PublicMod.AddScore(data[i].UserId, strAddTitle, deAddScore, strTableName, data[i].Id, strIp, strUser, dtTime);//签到积分
                        }
                    }
                    else if (data[i].ActiveName == "不参加" || data[i].ActiveName == "取消")
                    {
                        //public DataUserScore[] GetDatas(int Active, string UserIds, string Table, int TableId, string Title, string GetTimeText, string strFields, int intPage = 1, int pageSize = 0, string strOrderBy = "", string strFilter = "")
                        DataUserScore[] usData = webUserScore.GetDatas(1, data[i].UserId.ToString(), strTableName, data[i].Id, "", "", "Id");
                        if (usData != null)
                        {
                            ArrayList arrList = new ArrayList();
                            for (int j = 0; j < usData.Count(); j++)
                            {
                                arrList.Add(usData[j].Id);
                            }
                            if (arrList.Count > 0)
                            {
                                webUserScore.UpdateActive(arrList, -1);//取消积分
                            }
                        }
                    }
                    else if (data[i].ActiveName == "请假" || data[i].ActiveName == "缺席" || data[i].ActiveName == "未确认")// || data[i].ActiveName == "已出席"
                    {//导入活动出席状态：请假、缺席、未确认、已出席。王锦辰 Id=1804，为空（需再次确认）
                    }
                    else
                    {
                        if (data[i].IsMust == "必须参加")
                        {
                            if (data[i].ActiveName == "同意请假")
                            {//扣分：同意请假
                                PublicMod.AddScore(data[i].UserId, strDeTitle + "-同意请假", deDeScore, strTableName, data[i].Id, strIp, strUser, dtTime);
                            }
                            else
                            {//扣分：必须参加 而未参加、或请假未批准
                                PublicMod.AddScore(data[i].UserId, strDeTitle + "-未出席", deDeScore2, strTableName, data[i].Id, strIp, strUser, dtTime);
                            }
                        }
                        else if (data[i].ActiveName == "参加")
                        {//扣分：报名参加 而未参加，未取消的
                            PublicMod.AddScore(data[i].UserId, strDeTitle + "-未出席", deDeScore2, strTableName, data[i].Id, strIp, strUser, dtTime);
                        }
                        else if (data[i].ActiveName == "待确认")
                        {//清除待确认的扣分情况
                            PublicMod.AddScore(data[i].UserId, strDeTitle, 0, strTableName, data[i].Id, strIp, strUser, dtTime);
                        }
                    }
                }
                //webSign.UpdateActive(arrList, "关闭");
            }
            //Response.Write(deAddScore.ToString()+pData[0].ActiveName); Response.End();
        }
        //删除活动、取消积分
        private void delPerform(string strUser, int PerformId)
        {
            DataPerform[] pData = webPerform.GetData(PerformId, "ActiveName");
            if (pData == null || pData[0].ActiveName == "删除")
            {
                return;
            }
            string strTableName = "tb_Perform_Feed";
            WebPerformFeed webFeed = new WebPerformFeed();
            DataPerformFeed[] fData = webFeed.GetDatas("", PerformId, 0, "", "Id");
            if (fData != null)
            {
                string strIp = HelperMain.GetIpPort();
                WebUserScore webScore = new WebUserScore();
                for (int i = 0; i < fData.Count(); i++)
                {
                    DataUserScore[] sData = webScore.GetDatas(0, "", strTableName, fData[i].Id, "", "", "Id");
                    if (sData != null)
                    {
                        ArrayList arrList = new ArrayList();
                        for (int j = 0; j < sData.Count(); j++)
                        {
                            arrList.Add(sData[j].Id);
                        }
                        if (arrList.Count > 0)
                        {
                            webScore.UpdateActive(arrList, -1, strIp, strUser);//用户积分状态设为<0
                        }
                    }
                }
            }
        }
        //
        #endregion
        //
        #region 短信
        //短信测试
        protected void btnSmsTest_Click(object sender, EventArgs e)
        {
            //发送短信：{"code":200,"msg":"success","msgId":"160588699983403736321","contNum":2}
            //《政协-会议通知》 您有一个“报名参加”的会议 名称：街道活动cs 时间：2020年11月30日 13:00 地点：四川北路 请准时到会，不要迟到 详情：http://hkzx.quyou.net/m/perform.aspx?id=1066
            //{"username":"shzxhk","password":"8266598736aeee0fe6ba6046d00b3c05","tKey":1605942370,"ext":"","extend":"","signature":"【虹口政协】","tpId":26148,"records":[{\"mobile\":\"18918966030\",\"tpContent\":{\"valid_code\":\"386118\"}}],"msgid":null,"count":0}
            // { "code":200, "msg":"success", "tpId":"26148", "msgId":"160594261166703875841", "indList":[] }
            int tpId = 26148;
            string tpContent = "\"valid_code\":\"" + mod.main.HelperMain.GetRdString(6, "num") + "\"";
            string strOut = mod.main.HelperSms.SendSmsTp(config.SMSUSER, config.SMSPWD, "18918966030", "【虹口政协】", tpId, tpContent);//
            //string strOut = mod.main.HelperSms.Health(config.SMSUSER);//{"status":"UP","details":{"userInfo":{"status":"UP","details":{"balance":90909,"userStatus":0,"channel":78}}}}
            ltInfo.Text = "<script>$(function(){ alert('" + strOut + "'); window.history.back(-1); });</script>";
        }
        //平台监控
        protected void btnSmsHealth_Click(object sender, EventArgs e)
        {
            //发送短信：{"code":200,"msg":"success","msgId":"160588699983403736321","contNum":2}
            //string strOut = mod.main.HelperSms.BatchReport(config.SMSUSER, config.SMSPWD, 100);
            //string strOut = mod.main.HelperSms.Query(config.SMSUSER, config.SMSPWD, "160588699983403736321", "18918966030");//
            string strOut = mod.main.HelperSms.Health(config.SMSUSER);//{"status":"UP","details":{"userInfo":{"status":"UP","details":{"balance":90909,"userStatus":0,"channel":78}}}}
            ltInfo.Text = "<script>$(function(){ alert('" + strOut + "'); window.history.back(-1); });</script>";
        }
        //短信余额
        protected void btnSmsBalance_Click(object sender, EventArgs e)
        {
            //string strOut = mod.main.HelperSms.QueryBalance(config.SMSUSER, config.SMSPWD);//老接口测试
            string result = mod.main.HelperSms.Balance(config.SMSUSER, config.SMSPWD);
            mod.main.ZTResult json = mod.main.HelperSms.Json2ZTResult(result);
            string strOut = "";
            if (json.code == 200)
            {
                strOut = "剩余" + json.sumSms.ToString() + "条短信！";
            }
            else
            {
                strOut = json.code + "\\n" + json.msg;
            }
            ltInfo.Text = "<script>$(function(){ alert('" + strOut + "'); window.history.back(-1); });</script>";
        }
        #endregion
        //
        #region 反馈
        //加载反馈列表
        private void listFeed(int PerformId)
        {
            if (PerformId <= 0)
            {
                return;
            }
            DataPerform[] pData = webPerform.GetData(PerformId);//, "IsMust,SubType,Title,StartTime,EndTime,PerformSite,ActiveName"
            if (pData == null)
            {
                return;
            }
            txtPerformTitle.Text = pData[0].Title;//Regex.Replace("中文-123.abc", @"\d|[a-zA-Z]", "", RegexOptions.IgnoreCase);//
            txtPerformSubType.Text = pData[0].SubType;
            ltPerformTitle.Text = pData[0].Title;
            for (int i = ddlFeedActiveName.Items.Count - 1; i >= 0; i--)
            {
                if (pData[0].IsMust == "必须参加")
                {
                    if (ddlFeedActiveName.Items[i].Value == "不参加" || ddlFeedActiveName.Items[i].Value == "已签到(未报名)")
                    {
                        ddlFeedActiveName.Items.RemoveAt(i);
                    }
                }
                else
                {
                    if (ddlFeedActiveName.Items[i].Value == "请假申请" || ddlFeedActiveName.Items[i].Value == "同意请假" || ddlFeedActiveName.Items[i].Value == "不同意请假")
                    {
                        ddlFeedActiveName.Items.RemoveAt(i);
                    }
                }
            }
            for (int i = 0; i < ddlFeedActiveName.Items.Count; i++)
            {
                if (!string.IsNullOrEmpty(ddlFeedActiveName.Items[i].Value))
                {
                    ddlActiveName.Items.Add(ddlFeedActiveName.Items[i].Value);
                }
            }
            if (pData[0].SubType == "常委会议" || pData[0].SubType.IndexOf("讲堂") >= 0)
            {
                for (int i = ddlSignManType.Items.Count - 1; i >= 0; i--)
                {
                    switch (ddlSignManType.Items[i].Value)
                    {
                        case "常委":
                            if (pData[0].SubType != "常委会议")
                            {
                                ddlSignManType.Items.Remove(ddlSignManType.Items[i]);
                            }
                            break;
                        case "主讲人":
                            if (pData[0].SubType.IndexOf("讲堂") < 0)
                            {
                                ddlSignManType.Items.Remove(ddlSignManType.Items[i]);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                btnSignManTypes.Visible = false;
                ddlSignManType.Visible = false;
            }
            for (int i = ddlSignManSpeak.Items.Count - 1; i >= 0; i--)
            {
                if (ddlSignManSpeak.Items[i].Text.IndexOf("全会") >= 0 && pData[0].SubType != "政协全体会议")
                {
                    ddlSignManSpeak.Items.Remove(ddlSignManSpeak.Items[i]);
                }
            }
            DataPerformFeed qData = new DataPerformFeed();
            qData.PerformId = PerformId;
            if (!string.IsNullOrEmpty(Request.QueryString["SignMan"]))
            {
                qData.SignMan = HelperMain.SqlFilter(Request.QueryString["SignMan"].Trim());
                txtFeedSignMan.Text = qData.SignMan;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ActiveName"]))
            {
                qData.ActiveName = HelperMain.SqlFilter(Request.QueryString["ActiveName"].Trim(), 20);
                HelperMain.SetDownSelected(ddlFeedActiveName, qData.ActiveName);
            }
            WebPerformFeed webFeed = new WebPerformFeed();
            DataPerformFeed[] data = webFeed.GetDatas(qData, "", 1, 0, "SignTime ASC, AddTime ASC");
            if (data == null)
            {
                return;
            }
            if (pData[0].ActiveName == "发布")
            {
                plFeedCmd.Visible = true;
            }
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
                if (!string.IsNullOrEmpty(data[i].VerifyIp) || !string.IsNullOrEmpty(data[i].VerifyUser))
                {
                    data[i].VerifyTimeText = data[i].VerifyTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (data[i].SignTime > DateTime.MinValue)//!string.IsNullOrEmpty(data[i].SignIp)
                {
                    data[i].SignTimeText = data[i].SignTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (string.IsNullOrEmpty(Request.QueryString["down"]) && data[i].UserId > 0)
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
                                strMsg = mData[j].AddTime.ToString("yyyy-MM-dd") + " 已发送微信";
                                break;
                            }
                            else if (mData[j].Remark.IndexOf("\"code\":200,") > 0)
                            {
                                strMsg = mData[j].AddTime.ToString("yyyy-MM-dd") + " 已发送短信";
                                break;
                            }
                        }
                        if (string.IsNullOrEmpty(strMsg))
                        {
                            strMsg = "未发送成功";
                        }
                        data[i].SendMsg = string.Format("<a href='../cn/dialog.aspx?ac=msg&TableName=tb_Perform&TableId={1}&UserId={2}' target='_blank'>{0}</a>", strMsg, PerformId, data[i].UserId);
                    }
                }
            }
            switch (Request.QueryString["down"])
            {
                case "sign":
                    downSign(pData[0], data);
                    break;
                case "xls":
                    downXls(pData[0], data);
                    break;
                default:
                    rpFeedList.DataSource = data;
                    rpFeedList.DataBind();
                    ltFeedNo.Visible = false;
                    ltFeedTotal.Text = data.Count().ToString();
                    break;
            }
            txtSmsBody.ToolTip = config.SMSSIGN;//短信签名
            string strUrl = "http://" + config.HOSTNAME + "/m/perform.aspx?id=" + pData[0].Id.ToString();
            txtSmsBody.Text = string.Format("【虹口政协】《会议通知》您有一个“{0}”的会议，名称：{1}，时间：{2:yyyy-MM-dd HH:mm}，地点：{3}；请准时到会，不要迟到。详情：{4}", pData[0].IsMust, pData[0].Title, pData[0].StartTime, pData[0].PerformSite, strUrl);//【虹口政协】《会议通知》您有一个“{meet_type}”的会议，名称：{meet_name}，时间：{date}，地点：{address}；请准时到会，不要迟到。详情请登录履职通：http://hkzx.quyou.net/
        }
        //发送微信消息
        protected void btnWxMsg_Click(object sender, EventArgs e)
        {
            sendMsg("wx");
        }
        //发送模板短信
        protected void btnSmsTp_Click(object sender, EventArgs e)
        {
            sendMsg("smstp");
        }
        //发送短信
        protected void btnSmsMsg_Click(object sender, EventArgs e)
        {
            string strBody = txtSmsBody.Text.Trim();
            if (!string.IsNullOrEmpty(strBody))
            {
                strBody = strBody.Replace(config.SMSSIGN, "");
                string strTime = txtSmsTime.Text.Trim();
                sendMsg("sms", strBody, strTime);
            }
        }
        //发送消息
        private void sendMsg(string Label, string strBody = "", string sendTime = "")
        {
            if (myUser == null)
            {
                return;
            }
            int intId = Convert.ToInt32(Request.QueryString["id"]);
            if (intId <= 0)
            {
                return;
            }
            DataPerform[] pData = webPerform.GetData(intId);
            if (pData == null)
            {
                return;
            }
            string strUrl = "http://" + config.HOSTNAME + "/m/perform.aspx?id=" + pData[0].Id.ToString();
            //strUrl = Request.Url.Scheme + "://" + strUrl + "/m/perform.aspx?id=" + pData[0].Id.ToString();
            string strType = pData[0].IsMust;
            string strFirst = "您有一个“" + strType + "”的会议";
            string strRemark = "请准时到会，不要迟到";
            string strTitle = pData[0].Title.Replace("\"", "'");
            string strTime = pData[0].StartTime.ToString("yyyy年M月d日 HH:mm");
            string strSite = pData[0].PerformSite.Replace("\"", "'");
            string strIp = HelperMain.GetIpPort();
            string strUser = myUser.AdminName;
            string strMode = "政协-会议通知";
            if (string.IsNullOrEmpty(strBody))
            {
                strBody = "《" + strMode + "》" + strFirst + "，名称：" + strTitle + "，时间：" + strTime + "，地点：" + strSite + "，" + strRemark + "。详情：" + strUrl;
            }
            int okNum = 0;
            int errNum = 0;
            WebPerformFeed webFeed = new WebPerformFeed();
            WebUser webUser = new WebUser();
            WebUserWx webUserWx = new WebUserWx();
            WebSendMsg webSendMsg = new WebSendMsg();
            for (int i = 0; i < rpFeedList.Items.Count; i++)
            {
                CheckBox ck = (CheckBox)rpFeedList.Items[i].FindControl("_ck");
                HiddenField hf = (HiddenField)rpFeedList.Items[i].FindControl("_id");
                if (ck.Checked)
                {
                    DataPerformFeed[] data = webFeed.GetData(Convert.ToInt32(hf.Value), "UserId,ActiveName");
                    if (data != null && data[0].UserId > 0 && (data[0].ActiveName == "参加" || data[0].ActiveName == "待确认" || data[0].ActiveName == "未确认" || data[0].ActiveName == "请假" || data[0].ActiveName == "不同意请假"))
                    {
                        string result = "";
                        string strBody2 = "";
                        if (Label == "wx")
                        {//发送微信模板消息
                            DataUserWx[] wx = webUserWx.GetDatas(config.APPID, data[0].UserId, "", "WxOpenId");
                            if (wx != null && !string.IsNullOrEmpty(wx[0].WxOpenId))
                            {
                                result = PublicMod.SendTemplateMsg(wx[0].WxOpenId, strMode, strUrl, strFirst, strRemark, strTitle, strTime, strSite);
                                if (result.IndexOf("errmsg：ok") > 0)
                                {
                                    okNum++;
                                }
                                else
                                {
                                    errNum++;
                                }
                                strBody2 = strBody;
                            }
                        }
                        else
                        {//发送短信
                            DataUser[] uData = webUser.GetData(data[0].UserId, "Mobile");
                            if (uData != null && !string.IsNullOrEmpty(uData[0].Mobile))
                            {
                                if (!Regex.IsMatch(strTitle, "^[\u4e00-\u9fa5]{1,10}$"))
                                {
                                    Label = "sms";
                                }
                                if (Label == "smstp")
                                {//模板短信："date":"date","address":"others","name":"chinese","type":"chinese"
                                    //{"username":"shzxhk","password":"1bb2e021b0936ccb1733eb593e091287","tKey":1605946321,"ext":"","extend":"","signature":"【虹口政协】","tpId":26148,"records":[{"mobile":"18918966030","tpContent":{"valid_code":"358058"}}]}
                                    //{"username":"shzxhk","password":"0d719ff0cdeb341b9853448b83c4dbcf","tKey":1605944577,"mobile":null,"content":null,"time":null,"ext":"","extend":"","signature":"【虹口政协】","tpId":26152,"records":[{"mobile":"18918966030","tpContent":{"type":"报名参加", "name":"街道活动cs", "date":"2020-11-30 13:00", "address":"四川北路"}}],"msgid":null,"count":0}
                                    //{"username":"shzxhk","password":"bae4499dc819ae4a834156fcaf35d176","tKey":1605945467,"ext":"","extend":"","signature":"【虹口政协】","tpId":26152,"records":[{"mobile":"18918966030","tpContent":{"type":"报名参加","name":"街道活动cs","date":"2020-11-30 13:00","address":"四川北路"}}]}
                                    //{"code":4025,"msg":"template records null","tpId":"26152","msgId":"160594599264984628481","invalidList":null}
                                    int tpId = 26164;//"meet_type":"chinese","date":"date","address":"others","meet_name":"others"//【虹口政协】《会议通知》您有一个“{meet_type}”的会议，名称：{meet_name}，时间：{date}，地点：{address}；请准时到会，不要迟到。详情请登录履职通：http://hkzx.quyou.net/
                                    //int tpId = 26152;//"date":"date","address":"others","name":"chinese","type":"chinese"//【虹口政协】《会议通知》您有一个“{type}”的会议，名称：{name}，时间：{date}，地点：{address}；请准时到会，不要迟到。详情请登录履职通：http://hkzx.quyou.net/
                                    string strDate = strTime.Replace("年", "-").Replace("月", "-").Replace("日", "");
                                    if (strTitle.Length > 10)
                                    {
                                        strTitle = strTitle.Substring(0, 10);
                                    }
                                    string tpContent = "\"type\":\"" + strType + "\",\"name\":\"" + strTitle + "\",\"date\":\"" + strDate + "\",\"address\":\"" + strSite + "\"";
                                    strBody2 = config.SMSSIGN + "模板(" + tpId.ToString() + ")短信：" + tpContent;
                                    //ltInfo.Text = "<script>$(function(){ alert('" + tpContent + "'); window.history.back(-1); });</script>";
                                    //return;
                                    result = mod.main.HelperSms.SendSmsTp(config.SMSUSER, config.SMSPWD, uData[0].Mobile, config.SMSSIGN, tpId, tpContent);
                                }
                                else
                                {//自定义短信
                                    strBody2 = config.SMSSIGN + strBody.Replace("政协-会议通知", "会议通知");
                                    result = mod.main.HelperSms.SendSms(config.SMSUSER, config.SMSPWD, uData[0].Mobile, strBody2, sendTime);//自定义短信
                                    if (!string.IsNullOrEmpty(sendTime))
                                    {
                                        strBody2 = sendTime + "\n" + strBody2;
                                    }
                                }
                                strBody2 = uData[0].Mobile + "\n" + strBody2;
                                mod.main.ZTResult json = mod.main.HelperSms.Json2ZTResult(result);
                                string strOut = "";
                                if (json.code == 200)
                                {
                                    okNum++;
                                    strOut = "内容计费" + json.contNum.ToString() + "条！";
                                }
                                else
                                {
                                    errNum++;
                                    strOut = json.code + "\\n" + json.msg;
                                    ltInfo.Text = "<script>$(function(){ alert('" + result + "'); window.history.back(-1); });</script>";
                                    return;
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(result))
                        {
                            PublicMod.AddSendMsg(webSendMsg, "tb_Perform", pData[0].Id, data[0].UserId, strBody2, result, strIp, strUser, Label);
                        }
                    }
                }
            }
            if (okNum > 0 || errNum > 0)
            {
                string strOut = "";
                if (okNum > 0)
                {
                    strOut += "成功发送" + okNum.ToString() + "条消息！\\n";
                }
                if (errNum > 0)
                {
                    strOut += errNum.ToString() + "条消息发送失败！";
                }
                ltInfo.Text = "<script>$(function(){ alert('" + strOut + "'); window.location.replace('" + Request.Url.ToString() + "'); });</script>";
                //ltInfo.Text = "<script>$(function(){ alert('成功发送" + okNum.ToString() + "条消息！'); window.location.replace('" + Request.Url.ToString() + "'); });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('没有消息被发送！'); window.history.back(-1); });</script>";
            }
        }
        //操作：同意请假
        protected void btnLeave_Click(object sender, EventArgs e)
        {
            updateActiveFeed("同意请假");
        }
        //操作：不同意请假
        protected void btnDisLeave_Click(object sender, EventArgs e)
        {
            string strInfo = (!string.IsNullOrEmpty(hfVerifyInfoFeed.Value)) ? HelperMain.SqlFilter(hfVerifyInfoFeed.Value.Trim()) : "";
            if (!string.IsNullOrEmpty(strInfo))
            {
                updateActiveFeed("不同意请假", strInfo);
            }
        }
        //操作：取消
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            updateActiveFeed("取消");
        }
        //操作：人员类型
        protected void btnSignManTypes_Click(object sender, EventArgs e)
        {
            string strInfo = HelperMain.SqlFilter(ddlSignManType.SelectedValue.Trim(), 20);
            updateActiveFeed("人员类型", strInfo);
        }
        //操作：会议发言
        protected void btnSignManSpeaks_Click(object sender, EventArgs e)
        {
            string strInfo = HelperMain.SqlFilter(ddlSignManSpeak.SelectedValue.Trim(), 20);
            updateActiveFeed("会议发言", strInfo);
        }
        //操作：提供资源
        protected void btnSignManProvides_Click(object sender, EventArgs e)
        {
            string strInfo = HelperMain.SqlFilter(ddlSignManProvide.SelectedValue.Trim(), 20);
            if (!string.IsNullOrEmpty(strInfo))
            {
                strInfo = "提供资源" + strInfo;
            }
            updateActiveFeed("提供资源", strInfo);
        }
        //操作：签到状态
        protected void btnActiveNames_Click(object sender, EventArgs e)
        {
            string strInfo = HelperMain.SqlFilter(ddlActiveName.SelectedValue.Trim(), 20);
            if (!string.IsNullOrEmpty(strInfo))
            {
                string strInfo2 = (!string.IsNullOrEmpty(hfVerifyInfoFeed.Value)) ? HelperMain.SqlFilter(hfVerifyInfoFeed.Value.Trim()) : "";
                updateActiveFeed(strInfo, strInfo2, true);
            }
        }
        //处理操作
        private void updateActiveFeed(string ActiveName, string strInfo = "", bool bl = false)
        {
            if (myUser == null)
            {
                return;
            }
            int PerformId = Convert.ToInt32(Request.QueryString["id"]);
            if (PerformId <= 0)
            {
                return;
            }
            WebPerformFeed webFeed = new WebPerformFeed();
            ArrayList arrList = new ArrayList();
            ArrayList arrUser = new ArrayList();
            for (int i = 0; i < rpFeedList.Items.Count; i++)
            {
                CheckBox ck = (CheckBox)rpFeedList.Items[i].FindControl("_ck");
                HiddenField hf = (HiddenField)rpFeedList.Items[i].FindControl("_id");
                if (ck.Checked)
                {
                    DataPerformFeed[] data = webFeed.GetData(Convert.ToInt32(hf.Value), "UserId,ActiveName");
                    if (data != null)
                    {
                        //同意请假、不同意请假、取消时，必须是还未签到状态
                        if (ActiveName == "人员类型" || ActiveName == "会议发言" || ActiveName == "提供资源" || data[0].ActiveName.IndexOf("已签到") < 0 || bl)
                        {
                            arrList.Add(hf.Value);//状态不为“已签到”时，才能修改
                            arrUser.Add(data[0].UserId);
                        }
                        else
                        {
                            ck.Checked = false;
                        }
                    }
                }
            }
            if (arrList.Count <= 0)
            {
                return;
            }
            string strIp = HelperMain.GetIpPort();
            string strUser = myUser.AdminName;
            int intId = webFeed.UpdateActive(arrList, ActiveName, strInfo, strIp, strUser);
            if (intId > 0)
            {
                string strBack = Request.Url.ToString();
                ltInfo.Text = "<script>$(function(){ alert('“" + ActiveName + "”操作成功！'); window.location.href='" + strBack + "'; });</script>";

                DateTime dtTime = DateTime.Now;
                DataPerform[] pData = webPerform.GetData(PerformId, "StartTime,EndTime");
                if (pData != null)
                {
                    dtTime = pData[0].EndTime;
                }
                if (ActiveName == "会议发言" || ActiveName == "提供资源")
                {
                    string TableName = "tb_Perform_Feed";
                    if (!string.IsNullOrEmpty(strInfo))
                    {
                        decimal deScore = 0;
                        if (ActiveName == "会议发言")
                        {
                            WebScore webScore = new WebScore();
                            if (strInfo.IndexOf("大会发言") >= 0)
                            {
                                DataScore[] scoreData = webScore.GetDatas(0, "会议发言", "", "全会大会发言", "Score,Score2");//
                                if (scoreData != null)
                                {
                                    deScore = (strInfo.IndexOf("上台交流") >= 0) ? scoreData[0].Score2 : scoreData[0].Score;
                                }
                            }
                            else
                            {
                                string strTitle = (strInfo.IndexOf("专题会发言") >= 0) ? "全会专题会发言" : "其他会议发言";
                                DataScore[] scoreData = webScore.GetDatas(0, "会议发言", "", strTitle, "Score");//
                                if (scoreData != null)
                                {
                                    deScore = scoreData[0].Score;
                                }
                            }
                        }
                        else
                        {
                            string strScore = strInfo.Replace("提供资源", "").Replace("分", "");
                            deScore = Convert.ToDecimal(strScore);
                        }
                        for (int i = 0; i < arrList.Count; i++)
                        {
                            int TableId = Convert.ToInt32(arrList[i]);
                            int UserId = Convert.ToInt32(arrUser[i]);
                            PublicMod.AddScore(UserId, strInfo, deScore, TableName, TableId, strIp, strUser, dtTime);
                        }
                    }
                    else
                    {
                        WebUserScore webScore = new WebUserScore();
                        string Title = (ActiveName == "会议发言") ? "%发言%" : "%提供资源%";
                        for (int i = 0; i < arrList.Count; i++)
                        {
                            int TableId = Convert.ToInt32(arrList[i]);
                            string strUserId = arrUser[i].ToString();
                            DataUserScore[] sData = webScore.GetDatas(1, strUserId, TableName, TableId, Title, "", "Id");
                            if (sData != null)
                            {
                                for (int j = 0; j < sData.Count(); j++)
                                {
                                    webScore.UpdateActive(sData[j].Id, -1);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ActiveName + "”操作失败！'); window.history.back(-1); });</script>";
            }
        }
        //清除会议积分
        protected void btnClearScores_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            int intNum = 0;
            WebUserScore webScore = new WebUserScore();
            string strTableName = "tb_Perform_Feed";
            string strIp = HelperMain.GetIpPort();
            string strUser = myUser.AdminName;
            for (int i = 0; i < rpFeedList.Items.Count; i++)
            {
                CheckBox ck = (CheckBox)rpFeedList.Items[i].FindControl("_ck");
                HiddenField hf = (HiddenField)rpFeedList.Items[i].FindControl("_id");
                if (ck.Checked)
                {
                    DataUserScore[] sData = webScore.GetDatas(0, "", strTableName, Convert.ToInt32(hf.Value), "", "", "Id");
                    if (sData != null)
                    {
                        ArrayList arrList = new ArrayList();
                        for (int j = 0; j < sData.Count(); j++)
                        {
                            arrList.Add(sData[j].Id);
                        }
                        if (arrList.Count > 0)
                        {
                            intNum += arrList.Count;
                            webScore.UpdateActive(arrList, -1, strIp, strUser);//用户积分状态设为<0
                        }
                    }
                }
            }
            if (intNum > 0)
            {
                string strBack = Request.Url.ToString();
                ltInfo.Text = "<script>$(function(){ alert('“清除会议积分”操作成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('没有“会议积分”被清除！'); window.history.back(-1); });</script>";
            }
        }
        #endregion
        //
        #region 下载数据
        //下载数据包
        private void downSign(DataPerform pData, DataPerformFeed[] fData)
        {
            if (myUser == null)
            {
                return;
            }
            string strThead = "<SignUrl>签到Url</SignUrl><UserCode>委员编号</UserCode><TrueName>姓名</TrueName><Sex>性别</Sex><Photo>照片</Photo>";
            string strTbody = "";
            WebUser webUser = new WebUser();
            for (int i = 0; i < fData.Count(); i++)
            {
                //DataUser[] uData = webUser.GetData(fData[i].UserId, "Id,UserCode,TrueName,UserSex,Photo");
                DataUser[] uData = webUser.GetDatas(config.PERIOD, fData[i].SignMan, "Id,UserCode,TrueName,UserSex,Photo");
                if (uData != null)
                {
                    for (int j = 0; j < uData.Count(); j++)
                    {
                        //pid：PerformId（DES加密）
                        //token：UserId=TrueName=datetime（DES加密）
                        string pid = HelperSecret.DESEncrypt(pData.Id.ToString(), SIGNDESKEY, SIGNDESIV);
                        string token = string.Format("{0}={1}={2}", uData[j].Id, uData[j].TrueName, pData.EndTime);
                        token = HelperSecret.DESEncrypt(token, SIGNDESKEY, SIGNDESIV);
                        string strUrl = string.Format("pid={0}&token={1}&desk=", pid, token);
                        string strPhoto = uData[j].Photo;
                        if (!string.IsNullOrEmpty(strPhoto))
                        {
                            strPhoto = HelperImg.ImgToBase64(Server.MapPath(strPhoto));
                        }
                        strTbody += string.Format("<user><SignUrl><![CDATA[{0}]]></SignUrl><UserCode><![CDATA[{1}]]></UserCode><TrueName><![CDATA[{2}]]></TrueName><Sex><![CDATA[{3}]]></Sex><Photo><![CDATA[{4}]]></Photo></user>", strUrl, uData[j].UserCode, uData[j].TrueName, uData[j].UserSex, strPhoto);
                    }
                }
            }
            DateTime dtNow = DateTime.Now;
            string strFileName = string.Format("{0}_{1:yyyyMMddHHmmss}.xml", pData.Title, dtNow);
            string strStream = string.Format("\n<root>\n<file_info><type_name>{0}</type_name><down_time>{1:yyyy-MM-dd HH:mm:ss}</down_time><down_user>{2}</down_user></file_info>\n<user_info>\n{3}</user_info>\n<users>\n{4}</users></root>", pData.Title, dtNow, myUser.AdminName, strThead, strTbody);
            HelperFile.DownloadXml(strFileName, strStream);
        }
        //下载名单
        private void downXls(DataPerform pData, DataPerformFeed[] fData, List<DataUser> list = null)
        {
            if (myUser == null)
            {
                return;
            }
            DateTime dtNow = DateTime.Now;
            string virtualPath = string.Format("../download/{0:yyyy}/{0:MM}/", dtNow);
            string filepath = HttpContext.Current.Server.MapPath(virtualPath);
            if (!System.IO.Directory.Exists(filepath))
            {
                System.IO.Directory.CreateDirectory(filepath);
            }
            string strFileName = string.Format("{0}_{1:yyyyMMddHHmmss}.xls", pData.Title, dtNow);
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
            workSheet.Range[workSheet.Cells[3, 1], workSheet.Cells[3, 8]].MergeCells = true;//合并单元格
            workSheet.Cells[3, 1].Font.Size = 16;//设置字体大小
            PublicMod.SetCells(workSheet, 3, 1, pData.Title, "center,bold");
            PublicMod.SetCells(workSheet, 4, 1, string.Format("时间：{0:yyyy/MM/dd HH:mm} - {1:yyyy/MM/dd HH:mm}", pData.StartTime, pData.EndTime));
            PublicMod.SetCells(workSheet, 5, 1, "地点：" + pData.PerformSite);
            if (list == null)
            {//ac=feed
                PublicMod.SetCells(workSheet, 6, 1, "序号", "fit,center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 2, "委员编号", "fit,center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 3, "姓名", "center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 4, "性别", "fit,center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 5, "电话", "center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 6, "工作单位及职务", "fit,center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 7, "状态", "center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 8, "备注", "center,bold,border", "LightGray");
                WebUser webUser = new WebUser();
                int lenOrgName = "工作单位及职务".Length;
                int lenRemark = "备注".Length;
                for (int i = 0; i < fData.Count(); i++)
                {
                    DataUser[] uData = webUser.GetData(fData[i].UserId, "UserCode,TrueName,UserSex,Mobile,OrgName");
                    if (uData != null)
                    {
                        PublicMod.SetCells(workSheet, i + 7, 1, fData[i].num.ToString(), "txt,center,border");
                        PublicMod.SetCells(workSheet, i + 7, 2, uData[0].UserCode, "txt,center,border");
                        PublicMod.SetCells(workSheet, i + 7, 3, uData[0].TrueName, "txt,center,border");
                        PublicMod.SetCells(workSheet, i + 7, 4, uData[0].UserSex, "txt,center,border");
                        PublicMod.SetCells(workSheet, i + 7, 5, uData[0].Mobile, "fit,txt,center,border");
                        string attrOrgName = "txt,border";
                        if (uData[0].OrgName.Length > lenOrgName)
                        {
                            lenOrgName = uData[0].OrgName.Length;
                            attrOrgName += ",fit";
                        }
                        PublicMod.SetCells(workSheet, i + 7, 6, uData[0].OrgName, attrOrgName);
                        PublicMod.SetCells(workSheet, i + 7, 7, fData[i].ActiveName, "txt,center,border");
                        string attrRemark = "txt,border";
                        if (fData[i].LeaveReason.Length > lenRemark)
                        {
                            lenRemark = fData[i].LeaveReason.Length;
                            attrRemark += ",fit";
                        }
                        PublicMod.SetCells(workSheet, i + 7, 8, fData[i].LeaveReason, attrRemark);
                    }
                }
            }
            else
            {//ac=roll
                PublicMod.SetCells(workSheet, 6, 1, "序号", "center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 2, "状态", "center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 3, "委员编号", "fit,center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 4, "姓名", "center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 5, "性别", "fit,center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 6, "出生日期", "center,bold,border", "LightGray");
                workSheet.Cells[6, 6].ColumnWidth = 12;
                PublicMod.SetCells(workSheet, 6, 7, "政治面貌", "fit,center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 8, "界别", "center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 9, "是否特邀监督员", "fit,center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 10, "体制内外", "fit,center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 11, "单位性质", "fit,center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 12, "工作单位及职务", "fit,center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 13, "单位地址", "fit,center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 14, "家庭地址", "fit,center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 15, "联系方式", "center,bold,border", "LightGray");
                workSheet.Cells[6, 15].ColumnWidth = 12;
                PublicMod.SetCells(workSheet, 6, 16, "是否出席", "fit,center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 17, "是否发言", "fit,center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 6, 18, "是否提供资源", "fit,center,bold,border", "LightGray");
                if (list.Count() > 0)
                {
                    int lenOrgName = "工作单位及职务".Length;
                    int lenOrgAddress = "单位地址".Length;
                    int lenHomeAddress = "家庭地址".Length;
                    for (int i = 0; i < list.Count(); i++)
                    {
                        PublicMod.SetCells(workSheet, i + 7, 1, fData[i].num.ToString(), "txt,center,border");
                        PublicMod.SetCells(workSheet, i + 7, 2, fData[i].ActiveName, "txt,center,border");
                        PublicMod.SetCells(workSheet, i + 7, 3, list[i].UserCode, "txt,center,border");
                        PublicMod.SetCells(workSheet, i + 7, 4, list[i].TrueName, "txt,center,border");
                        PublicMod.SetCells(workSheet, i + 7, 5, list[i].UserSex, "txt,center,border");
                        PublicMod.SetCells(workSheet, i + 7, 6, list[i].BirthdayText, "txt,center,border");
                        PublicMod.SetCells(workSheet, i + 7, 7, list[i].Party, "txt,center,border");
                        PublicMod.SetCells(workSheet, i + 7, 8, list[i].Subsector, "txt,center,border");
                        PublicMod.SetCells(workSheet, i + 7, 9, list[i].IsInvited, "txt,center,border");
                        PublicMod.SetCells(workSheet, i + 7, 10, list[i].IsSystem, "txt,center,border");
                        PublicMod.SetCells(workSheet, i + 7, 11, list[i].OrgType, "txt,center,border");
                        string attrOrgName = "txt,border";
                        if (list[i].OrgName.Length > lenOrgName)
                        {
                            lenOrgName = list[i].OrgName.Length;
                            attrOrgName += ",fit";
                        }
                        PublicMod.SetCells(workSheet, i + 7, 12, list[i].OrgName, attrOrgName);
                        string attrOrgAddress = "txt,border";
                        if (list[i].OrgAddress.Length > lenOrgAddress)
                        {
                            lenOrgAddress = list[i].OrgAddress.Length;
                            attrOrgAddress += ",fit";
                        }
                        PublicMod.SetCells(workSheet, i + 7, 13, list[i].OrgAddress, attrOrgAddress);
                        string attrHomeAddress = "txt,border";
                        if (list[i].HomeAddress.Length > lenHomeAddress)
                        {
                            lenHomeAddress = list[i].HomeAddress.Length;
                            attrHomeAddress += ",fit";
                        }
                        PublicMod.SetCells(workSheet, i + 7, 14, list[i].HomeAddress, attrHomeAddress);
                        PublicMod.SetCells(workSheet, i + 7, 15, list[i].Mobile, "txt,center,border");
                        PublicMod.SetCells(workSheet, i + 7, 16, list[i].IsPresent, "txt,center,border");
                        PublicMod.SetCells(workSheet, i + 7, 17, list[i].IsSpeak, "txt,center,border");
                        PublicMod.SetCells(workSheet, i + 7, 18, list[i].IsProvide, "txt,center,border");

                    }
                }
            }

            //有两个选项可以设置，如下
            excelApp.Visible = false;//visable属性设置为true的话，excel程序会启动；false的话，excel只在后台运行
            excelApp.DisplayAlerts = false;//displayalert设置为true将会显示excel中的提示信息
            //保存文件，关闭workbook
            workBook.SaveAs(path, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            workBook.Close(false, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
            //退出并清理objects，回收内存
            excelApp.Quit();
            workSheet = null;
            workBook = null;
            excelApp = null;
            GC.Collect();
            Response.Redirect(fileName);
        }
        #endregion
        //
        #region 名单
        //履职名单
        private void listRoll(int PerformId)
        {
            if (PerformId <= 0)
            {
                return;
            }
            DataPerform[] pData = webPerform.GetData(PerformId, "Id,SubType,Title,StartTime,EndTime,PerformSite,Attendees,ActiveName");
            if (pData == null)
            {
                return;
            }
            ltRollType.Text = pData[0].SubType;
            ltRollTitle.Text = string.Format("{0}({1:yyyy年M月d日})", pData[0].Title, pData[0].StartTime);
            List<DataUser> list = new List<DataUser>();
            WebUser webUser = new WebUser();
            //string strMans = pData[0].Attendees;
            WebPerformFeed webFeed = new WebPerformFeed();
            DataPerformFeed[] fData = webFeed.GetDatas("<>'取消'", PerformId, 0, "");
            if (fData != null)
            {
                for (int i = 0; i < fData.Count(); i++)
                {
                    fData[i].num = i + 1;
                    //strMans = strMans.Replace("," + fData[i].SignMan + ",", ",");
                    DataUser data = new DataUser();
                    if (fData[i].UserId > 0)
                    {
                        DataUser[] uData = webUser.GetData(fData[i].UserId);
                        if (uData != null)
                        {
                            if (uData[0].Birthday > DateTime.MinValue)
                            {
                                uData[0].BirthdayText = uData[0].Birthday.ToString("yyyy-MM-dd");
                            }
                            if (uData[0].Role.IndexOf("特邀监督员") >= 0)
                            {
                                uData[0].IsInvited = "是";
                            }
                            data = uData[0];
                        }
                    }
                    else
                    {
                        data.Id = fData[i].UserId;
                        data.TrueName = fData[i].SignMan;
                    }
                    data.PerformFeedActive = fData[i].ActiveName;
                    if (fData[i].ActiveName.IndexOf("已签到") >= 0)
                    {
                        data.IsPresent = "是";
                    }
                    if (!string.IsNullOrEmpty(fData[i].SignManSpeak))
                    {
                        data.IsSpeak = "是";
                    }
                    if (!string.IsNullOrEmpty(fData[i].SignManProvide))
                    {
                        data.IsProvide = "是";
                    }
                    list.Add(data);
                }
            }
            //if (!string.IsNullOrEmpty(strMans))
            //{
            //    strMans = strMans.Trim(',');
            //}
            //if (!string.IsNullOrEmpty(strMans))
            //{
            //    string[] arr = strMans.Split(',');
            //    for (int i = 0; i < arr.Count(); i++)
            //    {
            //        if (!string.IsNullOrEmpty(arr[i]))
            //        {
            //            DataUser data = new DataUser();
            //            DataUser[] uData = webUser.GetDatas(arr[i]);
            //            if (uData != null)
            //            {
            //                if (uData[0].Birthday > DateTime.MinValue)
            //                {
            //                    uData[0].BirthdayText = uData[0].Birthday.ToString("yyyy-MM-dd");
            //                }
            //                if (uData[0].Role.IndexOf("特邀监督员") >= 0)
            //                {
            //                    uData[0].IsInvited = "是";
            //                }
            //                data = uData[0];
            //            }
            //            else
            //            {
            //                data.TrueName = arr[i];
            //            }
            //            list.Add(data);
            //        }
            //    }
            //}
            if (Request.QueryString["down"] == "xls")
            {
                downXls(pData[0], fData, list);
            }
            else if (list.Count > 0)
            {
                rpRollList.DataSource = list;
                rpRollList.DataBind();
                ltRollNo.Visible = false;
                ltRollTotal.Text = list.Count.ToString();
            }
        }
        #endregion
        //
    }
}