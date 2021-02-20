using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mod.main;
using hkzx.db;
using hkzx.user;
using System.Collections;

namespace hkzx.web.admin
{
    public partial class notice : System.Web.UI.Page
    {
        private string strTableName = "tb_Notice";
        private DataAdmin myUser = null;
        private WebNotice webNotice = new WebNotice();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperAdmin.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
            {
                //Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            if (myUser.Powers.IndexOf("alls") < 0 && myUser.Powers.IndexOf("notice") < 0)
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
                    hfBack.Value = PublicMod.GetBackUrl();
                    plEdit.Visible = true;
                    loadData(Convert.ToInt32(Request.QueryString["id"]));
                    strTitle = ltEditTitle.Text;
                }
                else if (!string.IsNullOrEmpty(Request.QueryString["nid"]))
                {
                    strTitle = "信息发布 - 反馈情况";
                    plFeed.Visible = true;
                    listFeed(Convert.ToInt32(Request.QueryString["nid"]));
                }
                else
                {
                    strTitle = "信息发布查询";
                    plList.Visible = true;
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
            DataNotice qData = new DataNotice();
            qData.ActiveName = ">0";
            qData.OverTimeText = DateTime.Now.ToString("yyyy-MM-dd") + ",";
            listData(qData, rpList, ltNo);
        }
        //查询列表
        private void queryData()
        {
            DataNotice qData = new DataNotice();
            if (!string.IsNullOrEmpty(Request.QueryString["Active"]))
            {
                qData.ActiveName = HelperMain.SqlFilter(Request.QueryString["Active"].Trim(), 20);
            }
            //else if (Request.QueryString["ac"] != "query")
            //{
            //    data.ActiveName = "1";
            //}
            HelperMain.SetCheckSelected(cblQActive, qData.ActiveName);
            if (!string.IsNullOrEmpty(Request.QueryString["SubType"]))
            {
                qData.SubType = HelperMain.SqlFilter(Request.QueryString["SubType"], 20);
                HelperMain.SetCheckSelected(cblQSubType, qData.SubType);
            }
            //if (!string.IsNullOrEmpty(Request.QueryString["Client"]))
            //{
            //    data.Client = HelperMain.SqlFilter(Request.QueryString["Client"], 20);
            //}
            if (!string.IsNullOrEmpty(Request.QueryString["Title"]))
            {
                qData.Title = "%" + HelperMain.SqlFilter(Request.QueryString["Title"], 50) + "%";
                txtQTitle.Text = qData.Title.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ToMans"]))
            {
                qData.ToMans = HelperMain.SqlFilter(Request.QueryString["ToMans"]);
                txtQToMans.Text = qData.ToMans;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["OverTime"]) && Request.QueryString["OverTime"].IndexOf(",") >= 0)
            {
                string strTime = Request.QueryString["OverTime"];
                string strOverTime1 = strTime.Substring(0, strTime.IndexOf(","));
                string strOverTime2 = strTime.Substring(strTime.IndexOf(",") + 1);
                txtQOverTime1.Text = HelperMain.SqlFilter(strOverTime1.Trim(), 10);
                txtQOverTime2.Text = HelperMain.SqlFilter(strOverTime2.Trim(), 10);
                if (txtQOverTime1.Text != "" || txtQOverTime2.Text != "")
                {
                    qData.OverTimeText = txtQOverTime1.Text + "," + txtQOverTime2.Text;
                }
            }
            else if (Request.QueryString["ac"] != "query")
            {
                txtQOverTime1.Text = DateTime.Now.ToString("yyyy-MM-dd");
                qData.OverTimeText = txtQOverTime1.Text + ",";
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ShowTime"]) && Request.QueryString["ShowTime"].IndexOf(",") >= 0)
            {
                string strShowTime1 = Request.QueryString["ShowTime"].Substring(0, Request.QueryString["ShowTime"].IndexOf(","));
                string strShowTime2 = Request.QueryString["ShowTime"].Substring(Request.QueryString["ShowTime"].IndexOf(",") + 1);
                txtQShowTime1.Text = HelperMain.SqlFilter(strShowTime1.Trim(), 10);
                txtQShowTime2.Text = HelperMain.SqlFilter(strShowTime2.Trim(), 10);
                if (txtQShowTime1.Text != "" || txtQShowTime2.Text != "")
                {
                    qData.ShowTimeText = txtQShowTime1.Text + "," + txtQShowTime2.Text;
                }
            }
            listData(qData, rpQueryList, ltQueryNo, lblQueryNav, ltQueryTotal);
        }
        //加载列表
        private void listData(DataNotice qData, Repeater rpList, Literal ltNo, Label lblNav = null, Literal ltTotal = null)
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
            string strOrder = "OverTime ASC, Active DESC, ShowTime ASC, AddTime DESC";
            DataNotice[] data = webNotice.GetDatas(qData, "Id,SubType,ToMans,OverTime,Title,Body,Files,Active,ReadNum", pageCur, pageSize, strOrder, "total");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    if (!string.IsNullOrEmpty(data[i].Body))
                    {
                        data[i].Body = HelperMain.DelUbb(data[i].Body);//data[i].Body.Replace("\n", "<br/>");
                        if (data[i].Body.Length > 100)
                        {
                            data[i].Body = data[i].Body.Substring(0, 100) + "……";
                        }
                    }
                    if (!string.IsNullOrEmpty(data[i].Files))
                    {
                        data[i].Files = string.Format("<a href='{0}' target='_blank'><u>附件下载</u></a>", data[i].Files);
                    }
                    if (data[i].Active == 0)
                    {
                        data[i].rowClass = " class='save' title='暂存'";
                        data[i].ActiveName = "暂存";
                    }
                    else if (data[i].Active > 0)
                    {
                        if (data[i].OverTime < DateTime.Now)
                        {
                            data[i].rowClass = " class='cancel' title='过期'";
                        }
                        data[i].ActiveName = (data[i].Active >= 10) ? "重要" : "正常";
                    }
                    else if (data[i].Active <= -400)
                    {
                        data[i].rowClass = " class='del' title='删除'";
                        data[i].ActiveName = "删除";
                    }
                    else if (data[i].Active < 0)
                    {
                        data[i].rowClass = " class='cancel' title='取消'";
                        data[i].ActiveName = "取消";
                    }
                    //data[i].OverTimeText = data[i].OverTime.ToString("yyyy-MM-dd HH:mm");
                    WebFeedback webFeed = new WebFeedback();
                    DataFeedback[] fData = webFeed.GetDatas(">0", strTableName, data[i].Id, 0, "Id");
                    if (fData != null)
                    {
                        data[i].FeedNum = fData.Count();
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
        private void loadData(int Id, DataNotice[] data = null)
        {
            for (int i = 0; i < cblQSubType.Items.Count; i++)
            {
                rblSubType.Items.Add(cblQSubType.Items[i]);
            }
            if (data == null)
            {
                if (Id <= 0)
                {
                    txtAddTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    txtAddIp.Text = HelperMain.GetIp();
                    txtAddUser.Text = myUser.AdminName;
                    txtShowTime.Text = txtAddTime.Text;
                    return;
                }
                data = webNotice.GetData(Id);
            }
            if (data != null)
            {
                if (data[0].Active >= 10)
                {
                    cblDegree.SelectedIndex = 0;
                }
                txtId.Text = data[0].Id.ToString();
                HelperMain.SetRadioSelected(rblSubType, data[0].SubType);
                //data[0].Client
                //data[0].SendMsg
                txtTitle.Text = data[0].Title;
                txtBody.Text = data[0].Body;
                //txtFiles.Text = data[0].Files;
                txtToMans.Text = data[0].ToMans.Trim(',');
                txtOverTime.Text = data[0].OverTime.ToString("yyyy-MM-dd HH:mm");
                txtRemark.Text = data[0].Remark;
                //HelperMain.SetRadioSelected(rblActive, data[0].Active.ToString());
                txtReadNum.Text = data[0].ReadNum.ToString();
                txtShowTime.Text = data[0].ShowTime.ToString("yyyy-MM-dd HH:mm:ss");
                txtAddTime.Text = data[0].AddTime.ToString("yyyy-MM-dd HH:mm:ss");
                txtAddIp.Text = data[0].AddIp;
                txtAddUser.Text = data[0].AddUser;
                if (!string.IsNullOrEmpty(data[0].UpIp) || !string.IsNullOrEmpty(data[0].UpUser))
                {
                    txtUpTime.Text = data[0].UpTime.ToString("yyyy-MM-dd HH:mm:ss");
                    txtUpIp.Text = data[0].UpIp;
                    txtUpUser.Text = data[0].UpUser;
                }
                btnEdit.Text = "更新";
                ltEditTitle.Text = ltEditTitle.Text.Replace("发布", "更新");
                btnDel.Visible = true;
            }
        }

        //提交数据
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            editData(1);
        }
        //暂存数据
        protected void btnSave_Click(object sender, EventArgs e)
        {
            editData(0);
        }
        //删除数据
        protected void btnDel_Click(object sender, EventArgs e)
        {
            editData(-400);
        }
        //编辑数据
        private void editData(int Active)
        {
            if (myUser == null)
            {
                return;
            }
            string strOut = "";
            string strBack = hfBack.Value;
            DataNotice data = new DataNotice();
            data.Id = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
            if (Active < 0)
            {
                if (data.Id > 0)
                {
                    strOut = "删除";
                    data.Id = webNotice.UpdateActive(data.Id, Active);
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
                data.SubType = HelperMain.SqlFilter(rblSubType.SelectedValue, 8);
                data.Title = HelperMain.SqlFilter(txtTitle.Text.Trim(), 50);
                data.OverTime = (!string.IsNullOrEmpty(txtOverTime.Text)) ? Convert.ToDateTime(txtOverTime.Text.Trim()) : dtNow;
                DataNotice[] ckData = webNotice.GetDatas(data, "Id");//重复检查
                if (ckData != null && ckData[0].Id != data.Id)
                {
                    ltInfo.Text = "<script>$(function(){ alert('“通知标题”重复，不能添加！'); window.history.back(-1); });</script>";
                    return;
                }
                data.Body = HelperMain.SqlFilter(txtBody.Text.Trim());
                //data.Files = HelperMain.SqlFilter(txtFiles.Text.Trim());
                if (!string.IsNullOrEmpty(txtToMans.Text))
                {
                    data.ToMans = "," + HelperMain.SqlFilter(txtToMans.Text.Trim()) + ",";
                }
                data.Remark = HelperMain.SqlFilter(txtRemark.Text.Trim(), 500);
                data.Active = Active;
                if (Active > 0)
                {
                    if (cblDegree.SelectedValue == "重要")
                    {
                        data.Active = 10;
                    }
                    strOut = btnEdit.Text;
                }
                else
                {
                    strOut = "暂存";
                }
                //data.ReadNum = (!string.IsNullOrEmpty(txtReadNum.Text)) ? Convert.ToInt64(txtReadNum.Text.Trim()) : 0;
                string strIp = HelperMain.GetIpPort();
                string strUser = HelperMain.SqlFilter(myUser.AdminName, 20);
                data.ShowTime = (!string.IsNullOrEmpty(txtShowTime.Text)) ? Convert.ToDateTime(txtShowTime.Text.Trim()) : dtNow;
                if (data.Id <= 0)
                {
                    data.AddTime = dtNow;
                    data.AddIp = strIp;
                    data.AddUser = strUser;
                    data.Id = webNotice.Insert(data);
                }
                else
                {
                    data.UpTime = dtNow;
                    data.UpIp = strIp;
                    data.UpUser = strUser;
                    if (webNotice.Update(data) <= 0)
                    {
                        data.Id = -1;
                    }
                }

                if (data.Id > 0)
                {
                    ltInfo.Text = "<script>$(function(){ alert('“" + strOut + "信息”成功！'); window.location.href='" + strBack + "'; });</script>";
                    string strToMans = (data.SubType == "公告") ? "" : data.ToMans;
                    addFeedUser(data.Id, strToMans, strIp, strUser);
                }
                else
                {
                    ltInfo.Text = "<script>$(function(){ alert('“" + strOut + "信息”失败！'); window.history.back(-1); });</script>";
                }
            }
        }
        //生成反馈名单表
        private void addFeedUser(int TableId, string ToMans, string strIp, string strUser)
        {
            string strTableName = "tb_Notice";
            WebFeedback webFeed = new WebFeedback();
            webFeed.UpdateActive(strTableName, TableId, -1);//反馈信息=0时，先取消反馈信息

            WebUser webUser = new WebUser();
            if (!string.IsNullOrEmpty(ToMans))
            {//通知
                string[] arr = ToMans.Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(arr[i]))
                    {
                        DataUser[] uData = webUser.GetDatas(config.PERIOD, arr[i], "Id,TrueName");
                        if (uData != null)
                        {
                            addFeedUser(webFeed, strTableName, TableId, uData[0].Id, uData[0].TrueName, strIp);//通知
                        }
                    }
                }
            }
            else
            {//公告
                DataUser qUser = new DataUser();
                qUser.Period = config.PERIOD;
                qUser.UserType = "委员";
                qUser.ActiveName = ">0";
                DataUser[] uData = webUser.GetDatas(qUser, "Id,TrueName");
                for (int i = 0; i < uData.Count(); i++)
                {
                    addFeedUser(webFeed, strTableName, TableId, uData[i].Id, uData[i].TrueName, strIp);//公告
                }
            }
        }
        private void addFeedUser(WebFeedback webFeed, string TableName, int TableId, int UserId, string strUser, string strIp)
        {
            DataFeedback[] fData = webFeed.GetDatas("<0", TableName, TableId, UserId, "Id");
            if (fData != null)
            {
                webFeed.UpdateActive(fData[0].Id, 0, strUser);//更新反馈状态
            }
            else
            {
                DataFeedback data = new DataFeedback();
                data.TableName = TableName;
                data.TableId = TableId;
                data.UserId = UserId;
                //data.Remark
                data.Active = 0;
                data.AddTime = DateTime.Now;
                data.AddIp = strIp;
                data.AddUser = strUser;
                webFeed.Insert(data);//新增反馈
            }
        }
        #endregion
        //
        #region 操作
        protected void btnImportant_Click(object sender, EventArgs e)
        {
            updateActive(10, "重要");
        }
        protected void btnPass_Click(object sender, EventArgs e)
        {
            updateActive(1, "正常");
        }
        protected void btnDels_Click(object sender, EventArgs e)
        {
            string VerifyInfo = (!string.IsNullOrEmpty(hfVerifyInfo.Value)) ? HelperMain.SqlFilter(hfVerifyInfo.Value.Trim()) : "";
            if (!string.IsNullOrEmpty(VerifyInfo))
            {
                updateActive(-444, "删除", VerifyInfo);
            }
        }
        //处理操作
        private void updateActive(int Active, string State, string VerifyInfo = "")
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
            string strUser = myUser.AdminName;
            if (webNotice.UpdateActive(arrList, Active, VerifyInfo, strIp, strUser) > 0)
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
        #region 反馈
        //加载反馈列表
        private void listFeed(int NoticeId)
        {
            if (NoticeId <= 0)
            {
                Response.Redirect("notice.aspx");
                return;
            }
            DataNotice[] nData = webNotice.GetData(NoticeId, "Title");
            if (nData == null)
            {
                Response.Redirect("notice.aspx");
                return;
            }
            txtNoticeTitle.Text = nData[0].Title;
            ltNoticeTitle.Text = nData[0].Title;
            int UserId = 0;
            if (!string.IsNullOrEmpty(Request.QueryString["FromMan"]))
            {
                string strFromMan = HelperMain.SqlFilter(Request.QueryString["FromMan"].Trim(), 20);
                txtFromMan.Text = strFromMan;
                WebUser webUser = new WebUser();
                DataUser[] uData = webUser.GetDatas(config.PERIOD, strFromMan, "Id");
                if (uData != null)
                {
                    UserId = uData[0].Id;
                }
            }
            string ActiveName = "";
            if (!string.IsNullOrEmpty(Request.QueryString["ActiveName"]))
            {
                ActiveName = HelperMain.SqlFilter(Request.QueryString["ActiveName"].Trim(), 20);
                HelperMain.SetDownSelected(ddlFeedActive, ActiveName);
            }
            else
            {
                ActiveName = ">=0";
            }
            WebFeedback webFeed = new WebFeedback();
            DataFeedback[] data = webFeed.GetDatas(ActiveName, strTableName, NoticeId, UserId);
            if (data != null)
            {
                //WebUser webUser = new WebUser();
                WebUserWx webUserWx = new WebUserWx();
                WebSendMsg webSendMsg = new WebSendMsg();
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = i + 1;
                    if (data[i].Active > 0)
                    {
                        data[i].ActiveName = "已阅";
                        if (data[i].UpTime > DateTime.MinValue)
                        {
                            data[i].FeedTime = data[i].UpTime.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                    else if (data[i].Active < 0)
                    {
                        data[i].ActiveName = "取消";
                    }
                    else
                    {

                    }
                    //DataUser[] uData = webUser.GetData(data[i].UserId, "TrueName");
                    //if (uData != null)
                    //{

                    //}
                    DataUserWx[] wxData = webUserWx.GetDatas(config.APPID, data[i].UserId, "", "WxOpenId");
                    if (wxData != null)
                    {
                        data[i].FeedUser = "<i class='wx' title='" + wxData[0].WxOpenId + "'></i>" + data[i].AddUser;
                    }
                    else
                    {
                        data[i].FeedUser = data[i].AddUser;
                    }
                    //是否发送微信消息
                    DataSendMsg[] mData = webSendMsg.GetDatas(">0", "tb_Notice", NoticeId, data[i].UserId, "", "Remark,AddTime");
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
                        data[i].SendMsg = string.Format("<a href='../cn/dialog.aspx?ac=msg&TableName=tb_Notice&TableId={1}&UserId={2}' target='_blank'>{0}</a>", strMsg, NoticeId, data[i].UserId);
                    }
                }
                rpFeedList.DataSource = data;
                rpFeedList.DataBind();
                ltFeedNo.Visible = false;
                ltFeedTotal.Text = data.Count().ToString();
            }
        }
        //发送微信消息
        protected void btnWxMsg_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            int intId = Convert.ToInt32(Request.QueryString["nid"]);
            if (intId <= 0)
            {
                return;
            }
            DataNotice[] nData = webNotice.GetData(intId);
            if (nData == null)
            {
                return;
            }
            string strUrl = "http://" + config.HOSTNAME + "/m/notice.aspx?id=" + nData[0].Id.ToString();
            //strUrl = Request.Url.Scheme + "://" + strUrl + "/m/notice.aspx?id=" + nData[0].Id.ToString();
            string strFirst = "您有一个“" + nData[0].SubType + "”待查阅";
            string strRemark = nData[0].Body.Replace("\"", "'");
            if (!string.IsNullOrEmpty(strRemark))
            {
                strRemark = HelperMain.DelUbb(strRemark);
            }
            string strTitle = nData[0].Title.Replace("\"", "'");
            string strTime = nData[0].UpTime.ToString("yyyy年M月d日"); //nData[0].OverTime
            string strIp = HelperMain.GetIpPort();
            string strUser = myUser.AdminName;
            string strMode = "信息发布";
            string strBody = "《" + strMode + "-" + nData[0].SubType + "》\n" + strFirst + "\n名称：" + strTitle + "\n" + strRemark + "\n详情：" + strUrl;
            int okNum = 0;
            int errNum = 0;
            WebFeedback webFeed = new WebFeedback();
            WebUserWx webUserWx = new WebUserWx();
            WebSendMsg webSendMsg = new WebSendMsg();
            for (int i = 0; i < rpFeedList.Items.Count; i++)
            {
                CheckBox ck = (CheckBox)rpFeedList.Items[i].FindControl("_ck");
                HiddenField hf = (HiddenField)rpFeedList.Items[i].FindControl("_id");
                if (ck.Checked)
                {
                    DataFeedback[] data = webFeed.GetData(Convert.ToInt32(hf.Value), "UserId,Active");
                    if (data != null && data[0].UserId > 0)// && data[0].Active == 0
                    {
                        DataUserWx[] wx = webUserWx.GetDatas(config.APPID, data[0].UserId, "", "WxOpenId");
                        if (wx != null && !string.IsNullOrEmpty(wx[0].WxOpenId))
                        {
                            string strMsg = PublicMod.SendTemplateMsg(wx[0].WxOpenId, strMode, strUrl, strFirst, strRemark, strTitle, strTime);
                            PublicMod.AddSendMsg(webSendMsg, "tb_Notice", nData[0].Id, data[0].UserId, strBody, strMsg, strIp, strUser, "wx");
                            if (strMsg.IndexOf("errmsg：ok") > 0)
                            {
                                okNum++;
                            }
                            else
                            {
                                errNum++;
                            }
                        }
                    }
                }
            }
            if (okNum > 0 || errNum > 0)
            {
                string strOut = "";
                if (okNum > 0)
                {
                    strOut += "成功发送" + okNum.ToString() + "条微信消息！\\n";
                }
                if (errNum > 0)
                {
                    strOut += errNum.ToString() + "条消息发送失败！";
                }
                ltInfo.Text = "<script>$(function(){ alert('" + strOut + "'); window.location.replace('" + Request.Url.ToString() + "'); });</script>";
                //ltInfo.Text = "<script>$(function(){ alert('成功发送" + okNum.ToString() + "条微信消息！'); window.location.replace('" + Request.Url.ToString() + "'); });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('没有微信消息被发送！'); });</script>";
            }
        }
        #endregion
        //
    }
}