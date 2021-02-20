using System;
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
    public partial class survey : System.Web.UI.Page
    {
        private DataAdmin myUser = null;
        private WebSurvey webSurvey = new WebSurvey();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperAdmin.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
            {
                //Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            if (myUser.Powers.IndexOf("alls") < 0 && myUser.Powers.IndexOf("survey") < 0)
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
                hfBack.Value = PublicMod.GetBackUrl();
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    int Id = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                    if (!string.IsNullOrEmpty(Request.QueryString["oid"]))
                    {
                        strTitle = " - 选项";
                        plOp.Visible = true;
                        WebSurveyOp webOp = new WebSurveyOp();
                        listOp(Id, webOp);
                        int intId = (!string.IsNullOrEmpty(Request.QueryString["oid"])) ? Convert.ToInt32(Request.QueryString["oid"]) : 0;
                        loadOp(intId, webOp);
                    }
                    else if (!string.IsNullOrEmpty(Request.QueryString["rid"]))
                    {
                        strTitle = " - 结果";
                        plResult.Visible = true;
                        WebSurveyResult webResult = new WebSurveyResult();
                        listResult(Id, webResult);
                        int intId = (!string.IsNullOrEmpty(Request.QueryString["rid"])) ? Convert.ToInt32(Request.QueryString["rid"]) : 0;
                        if (intId > 0)
                        {
                            loadResult(intId, webResult);
                        }
                    }
                    else
                    {
                        plSurveyEdit.Visible = true;
                        loadSurvey(Id);
                        strTitle = ltSurveyTitle.Text;
                    }
                }
                else
                {
                    plSurvey.Visible = true;
                    queryData();
                }
                Header.Title += " - 问卷调查" + strTitle;
            }
        }
        //
        #region 标题
        //查询列表
        private void queryData()
        {
            DataSurvey qData = new DataSurvey();
            if (!string.IsNullOrEmpty(Request.QueryString["SubType"]))
            {
                qData.SubType = HelperMain.SqlFilter(Request.QueryString["SubType"].Trim(), 20);
                HelperMain.SetCheckSelected(cblQSubType, qData.SubType);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Active"]))
            {
                qData.ActiveName = HelperMain.SqlFilter(Request.QueryString["Active"].Trim(), 20);
                HelperMain.SetCheckSelected(cblQActive, qData.ActiveName);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Title"]))
            {
                string strTitle = HelperMain.SqlFilter(Request.QueryString["Title"].Trim(), 20);
                if (!string.IsNullOrEmpty(strTitle))
                {
                    qData.Title = "%" + strTitle + "%";
                    txtQTitle.Text = strTitle;
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ToMans"]))
            {
                qData.ToMans = HelperMain.SqlFilter(Request.QueryString["ToMans"].Trim());
                txtQToMans.Text = qData.ToMans;
            }
            listSurvey(qData, rpSurveyList, ltSurveyNo, lblSurveyNav, ltSurveyTotal);
        }
        //加载列表
        private void listSurvey(DataSurvey qData, Repeater rpList, Literal ltNo, Label lblNav = null, Literal ltTotal = null)
        {
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            DataSurvey[] data = webSurvey.GetDatas(qData, "", pageCur, pageSize, "Active DESC, EndTime DESC, StartTime DESC, AddTime DESC", "total");
            if (data != null)
            {
                WebSurveyOp webOp = new WebSurveyOp();
                WebSurveyResult webResult = new WebSurveyResult();
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    if (data[i].Active > 0)
                    {
                        data[i].ActiveName = "正常";
                    }
                    else if (data[i].Active < 0)
                    {
                        data[i].ActiveName = "取消";
                        data[i].rowClass = " class='cancel' title='取消'";
                    }
                    else
                    {
                        data[i].ActiveName = "暂停";
                        data[i].rowClass = " class='save' title='暂停'";
                    }
                    DataSurveyOp[] opData = webOp.GetDatas(1, data[i].Id, "", "Id");
                    if (opData != null)
                    {
                        data[i].OpNum = opData.Count();
                    }
                    DataSurveyResult[] resData = webResult.GetDatas(1, data[i].Id, 0, "Id");
                    if (resData != null)
                    {
                        data[i].ResultNum = resData.Count();
                    }
                }
                rpList.DataSource = data;
                rpList.DataBind();
                ltNo.Visible = false;
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = Request.Url.ToString();
                lblNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
                ltTotal.Text = data[0].total.ToString();
            }
        }
        //加载信息
        private void loadSurvey(int Id)
        {
            if (Id <= 0)
            {
                txtAddTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                txtAddIp.Text = HelperMain.GetIp();
                txtAddUser.Text = myUser.AdminName;
                return;
            }
            DataSurvey[] data = webSurvey.GetData(Id);
            if (data != null)
            {
                txtId.Text = data[0].Id.ToString();
                txtActive.Text = data[0].Active.ToString();
                txtTitle.Text = data[0].Title;
                HelperMain.SetRadioSelected(rblSubType, data[0].SubType);
                txtToMans.Text = data[0].ToMans.Trim(',');
                txtSurveyNum.Text = data[0].SurveyNum.ToString();
                if (data[0].StartTime > DateTime.MinValue)
                {
                    txtStartTime.Text = data[0].StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (data[0].EndTime > DateTime.MinValue)
                {
                    txtEndTime.Text = data[0].EndTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                txtMaxNum.Text = data[0].MaxNum.ToString();
                txtMinNum.Text = data[0].MinNum.ToString();
                txtBody.Text = data[0].Body;
                txtRemark.Text = data[0].Remark;
                txtAddTime.Text = data[0].AddTime.ToString("yyyy-MM-dd HH:mm:ss");
                txtAddIp.Text = data[0].AddIp;
                txtAddUser.Text = data[0].AddUser;
                if (!string.IsNullOrEmpty(data[0].UpIp) || !string.IsNullOrEmpty(data[0].UpUser))
                {
                    txtUpTime.Text = data[0].UpTime.ToString("yyyy-MM-dd HH:mm:ss");
                    txtUpIp.Text = data[0].UpIp;
                    txtUpUser.Text = data[0].UpUser;
                }
                btnSurveyEdit.Text = "修改";
                ltSurveyTitle.Text = ltSurveyTitle.Text.Replace("新增", "修改");
                btnSurveyStop.Visible = true;
                btnSurveyCancel.Visible = true;
            }
        }
        //编辑数据
        protected void btnSurveyEdit_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            string strBack = hfBack.Value;
            WebSurvey webSurvey = new WebSurvey();
            DataSurvey data = new DataSurvey();
            data.Id = Convert.ToInt32(txtId.Text);
            data.SubType = HelperMain.SqlFilter(rblSubType.SelectedValue, 20);
            string strToMans = HelperMain.SqlFilter(txtToMans.Text.Trim());
            if (!string.IsNullOrEmpty(strToMans))
            {
                strToMans = "," + strToMans.Trim(',') + ",";
            }
            data.ToMans = strToMans;
            data.Title = HelperMain.SqlFilter(txtTitle.Text.Trim(), 50);
            DataSurvey[] ckData = webSurvey.GetDatas("", "", data.Title, DateTime.Now, "Id");//重复检查
            if (ckData != null && ckData[0].Id != data.Id)
            {
                ltInfo.Text = "<script>$(function(){ alert('“标题”重复，不能添加！'); window.history.back(-1); });</script>";
                return;
            }
            data.SurveyNum = (!string.IsNullOrEmpty(txtSurveyNum.Text)) ? Convert.ToInt16(txtSurveyNum.Text) : 0;
            data.StartTime = (!string.IsNullOrEmpty(txtStartTime.Text)) ? Convert.ToDateTime(txtStartTime.Text.Trim()) : DateTime.Now;
            data.EndTime = (!string.IsNullOrEmpty(txtEndTime.Text)) ? Convert.ToDateTime(txtEndTime.Text.Trim()) : DateTime.Now;
            data.MaxNum = (!string.IsNullOrEmpty(txtMaxNum.Text)) ? Convert.ToInt16(txtMaxNum.Text.Trim()) : 0;
            data.MinNum = (!string.IsNullOrEmpty(txtMinNum.Text)) ? Convert.ToInt16(txtMinNum.Text.Trim()) : 0;
            data.Body = HelperMain.SqlFilter(txtBody.Text.Trim());
            data.Remark = HelperMain.SqlFilter(txtRemark.Text.Trim());
            data.Remark = HelperMain.SqlFilter(txtRemark.Text.Trim(), 100);
            data.Active = (!string.IsNullOrEmpty(txtActive.Text.Trim())) ? Convert.ToInt16(txtActive.Text.Trim()) : 1;
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(myUser.AdminName, 20);
            if (data.Id <= 0)
            {
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = strUser;
                data.Id = webSurvey.Insert(data);
            }
            else
            {
                data.UpTime = dtNow;
                data.UpIp = strIp;
                data.UpUser = strUser;
                if (webSurvey.Update(data) <= 0)
                {
                    data.Id = -1;
                }
            }
            if (data.Id > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltSurveyTitle.Text + "”成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltSurveyTitle.Text + "”失败！'); window.history.back(-1); });</script>";
            }
        }
        //暂停状态
        protected void btnSurveyStop_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            int Id = Convert.ToInt32(txtId.Text);
            if (Id <= 0)
            {
                return;
            }
            WebSurvey webSurvey = new WebSurvey();
            if (webSurvey.UpdateActive(Id, 0) > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnSurveyStop.Text + "成功！'); window.location.href='" + hfBack.Value + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnSurveyStop.Text + "失败！'); window.history.back(-1); });</script>";
            }
        }
        //取消状态
        protected void btnSurveyCancel_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            int Id = Convert.ToInt32(txtId.Text);
            if (Id <= 0)
            {
                return;
            }
            WebSurvey webSurvey = new WebSurvey();
            if (webSurvey.UpdateActive(Id, -1) > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnSurveyCancel.Text + "成功！'); window.location.href='" + hfBack.Value + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnSurveyCancel.Text + "失败！'); window.history.back(-1); });</script>";
            }
        }
        #endregion
        //
        #region 选项
        //加载列表
        private void listOp(int sId, WebSurveyOp webOp)
        {
            if (sId <= 0)
            {
                Response.Redirect("survey.aspx");
                return;
            }
            DataSurvey[] opSurvey = webSurvey.GetData(sId, "Id,SubType,Title");
            if (opSurvey == null)
            {
                Response.Redirect("survey.aspx");
                return;
            }
            ltOpSurveySubType.Text = opSurvey[0].SubType;
            ltOpSurveyTitle.Text = opSurvey[0].Title;
            txtSurveyTitle.Text = opSurvey[0].Title;

            DataSurveyOp[] data = webOp.GetDatas(0, opSurvey[0].Id, "", "Id,SurveyId,Title,PicUrl,VoteNum,Method,Answer,Score,Body,Active");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = i + 1;
                    if (!string.IsNullOrEmpty(data[i].PicUrl))
                    {
                        data[i].PicUrl = string.Format("<img src='{0}'/>", data[i].PicUrl);
                    }
                    data[i].Body = data[i].Body.Replace("\n", "<br/>");
                    if (data[i].Active > 0)
                    {
                        data[i].ActiveName = "正常";
                    }
                    else if (data[i].Active < 0)
                    {
                        data[i].ActiveName = "取消";
                        data[i].rowClass = " class='cancel' title='取消'";
                    }
                    else
                    {
                        data[i].ActiveName = "暂停";
                        data[i].rowClass = " class='save' title='暂停'";
                    }
                }
                rpOpList.DataSource = data;
                rpOpList.DataBind();
                ltOpNo.Visible = false;
                ltOpTotal.Text = data.Count().ToString();//data[0].total.ToString();
            }
        }
        //加载信息
        private void loadOp(int Id, WebSurveyOp webOp)
        {
            if (Id <= 0)
            {
                txtOpAddTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                txtOpAddIp.Text = HelperMain.GetIp();
                txtOpAddUser.Text = myUser.AdminName;
                return;
            }
            DataSurveyOp[] data = webOp.GetData(Id);
            if (data != null)
            {
                txtOpId.Text = data[0].Id.ToString();
                txtOpTitle.Text = data[0].Title;
                txtOpPicUrl.Text = data[0].PicUrl;
                HelperMain.SetRadioSelected(rblOpMethod, data[0].Method);
                txtOpBody.Text = data[0].Body;
                txtOpAnswer.Text = data[0].Answer;
                txtOpScore.Text = data[0].Score.ToString();
                txtOpRemark.Text = data[0].Remark;
                txtOpActive.Text = data[0].Active.ToString();
                txtOpAddTime.Text = data[0].AddTime.ToString("yyyy-MM-dd HH:mm:ss");
                txtOpAddIp.Text = data[0].AddIp;
                txtOpAddUser.Text = data[0].AddUser;
                if (!string.IsNullOrEmpty(data[0].UpIp) || !string.IsNullOrEmpty(data[0].UpUser))
                {
                    txtOpUpTime.Text = data[0].UpTime.ToString("yyyy-MM-dd HH:mm:ss");
                    txtOpUpIp.Text = data[0].UpIp;
                    txtOpUpUser.Text = data[0].UpUser;
                }
                btnOpEdit.Text = "修改";
                ltOpTitle.Text = ltOpTitle.Text.Replace("新增", "修改");
                btnOpStop.Visible = true;
                btnOpCancel.Visible = true;
            }
        }
        //编辑数据
        protected void btnOpEdit_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            WebSurveyOp webOp = new WebSurveyOp();
            DataSurveyOp data = new DataSurveyOp();
            data.Id = Convert.ToInt32(txtOpId.Text);
            data.SurveyId = Convert.ToInt32(Request.QueryString["id"]);
            data.Title = HelperMain.SqlFilter(txtOpTitle.Text.Trim(), 100);
            DataSurveyOp[] ckData = webOp.GetDatas(0, data.SurveyId, data.Title, "Id");//重复检查
            if (ckData != null && ckData[0].Id != data.Id)
            {
                ltInfo.Text = "<script>$(function(){ alert('“选项名称”重复，不能添加！'); window.history.back(-1); });</script>";
                return;
            }
            data.PicUrl = HelperMain.SqlFilter(txtOpPicUrl.Text.Trim());
            data.Method = HelperMain.SqlFilter(rblOpMethod.SelectedValue, 10);
            data.Body = HelperMain.SqlFilter(txtOpBody.Text.Trim());
            data.Answer = HelperMain.SqlFilter(txtOpAnswer.Text.Trim(), 50);
            data.Score=(!string.IsNullOrEmpty(txtOpScore.Text)) ? Convert.ToInt16(txtOpScore.Text.Trim()) : 0;
            data.Remark = HelperMain.SqlFilter(txtOpRemark.Text.Trim(), 500);
            data.Active = (!string.IsNullOrEmpty(txtOpActive.Text.Trim())) ? Convert.ToInt16(txtOpActive.Text.Trim()) : 1;
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(myUser.AdminName, 20);
            if (data.Id <= 0)
            {
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = strUser;
                data.Id = webOp.Insert(data);
            }
            else
            {
                data.UpTime = dtNow;
                data.UpIp = strIp;
                data.UpUser = strUser;
                if (webOp.Update(data) <= 0)
                {
                    data.Id = -1;
                }
            }
            if (data.Id > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltOpTitle.Text + "”成功！'); window.location.href='" + hfBack.Value + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltOpTitle.Text + "”失败！'); window.history.back(-1); });</script>";
            }
        }
        //暂停状态
        protected void btnOpStop_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            int Id = Convert.ToInt32(txtId.Text);
            if (Id <= 0)
            {
                return;
            }
            WebSurveyOp webOp = new WebSurveyOp();
            if (webOp.UpdateActive(Id, 0) > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnOpStop.Text + "成功！'); window.location.href='" + hfBack.Value + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnOpStop.Text + "失败！'); window.history.back(-1); });</script>";
            }
        }
        //取消状态
        protected void btnOpCancel_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            int Id = Convert.ToInt32(txtId.Text);
            if (Id <= 0)
            {
                return;
            }
            WebSurveyOp webOp = new WebSurveyOp();
            if (webOp.UpdateActive(Id, -1) > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnOpCancel.Text + "成功！'); window.location.href='" + hfBack.Value + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnOpCancel.Text + "失败！'); window.history.back(-1); });</script>";
            }
        }
        #endregion
        //
        #region 结果
        //加载列表
        private void listResult(int sId, WebSurveyResult webResult)
        {
            if (sId <= 0)
            {
                Response.Redirect("survey.aspx");
                return;
            }
            DataSurvey[] surData = webSurvey.GetData(sId, "SubType,Title");
            if (surData == null)
            {
                Response.Redirect("survey.aspx");
                return;
            }
            if (Request.QueryString["down"] == "xls")
            {
                downXls(sId, webResult, surData[0].SubType, surData[0].Title);//下载
                return;
            }
            ltResultSubType.Text = surData[0].SubType;
            ltResultTitle.Text = surData[0].Title;
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            DataSurveyResult[] data = webResult.GetDatas(0, sId, 0, "", pageCur, pageSize, "", "total");
            if (data != null)
            {
                lnkDown.Visible = true;
                lnkDown.NavigateUrl = Request.Url.ToString() + "&down=xls";
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    if (!string.IsNullOrEmpty(data[i].Body))
                    {
                        string[] arrTitles = data[i].Titles.Split('\n');
                        string[] arrBody = data[i].Body.Split('\n');
                        for (int j = 0; j < arrBody.Count(); j++)
                        {
                            arrBody[j] = "<li><b>" + arrTitles[j] + "</b><br/>[ " + arrBody[j] +  " ]</li>";
                        }
                        data[i].Body = "<ol>" + string.Join("", arrBody) + "</ol>";
                    }
                    if (data[i].Active > 0)
                    {
                        data[i].ActiveName = "正常";
                    }
                    else if (data[i].Active < 0)
                    {
                        data[i].ActiveName = "取消";
                        data[i].rowClass = " class='cancel' title='取消'";
                    }
                    else
                    {
                        data[i].ActiveName = "暂停";
                        data[i].rowClass = " class='save' title='暂停'";
                    }
                    data[i].ScoreText = (ltResultSubType.Text == "答题") ? data[i].Score.ToString() : "/";
                    //data[i].SubTimeText = (data[i].AddTime > DateTime.Today) ? data[i].AddTime.ToString("HH:mm:ss") : data[i].AddTime.ToString("yyyy-MM-dd");
                }
                rpResultList.DataSource = data;
                rpResultList.DataBind();
                ltResultNo.Visible = false;
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = Request.Url.ToString();
                lblResultNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
                ltResultTotal.Text = data[0].total.ToString();
            }
        }
        //加载信息
        private void loadResult(int Id, WebSurveyResult webResult)
        {
            DataSurveyResult[] data = webResult.GetData(Id);
            if (data == null)
            {
                return;
            }
            plResultInfo.Visible = true;
            ltResultId.Text = data[0].Id.ToString();
            if (data[0].UserId > 0)
            {
                WebUser webUser = new WebUser();
                DataUser[] uData = webUser.GetData(data[0].UserId, "TrueName");
                if (uData != null)
                {
                    ltResultUser.Text = uData[0].TrueName;
                }
            }
            ltResultScore.Text = (ltResultSubType.Text == "答题") ? data[0].Score.ToString() : "/";
            if (data[0].Active > 0)
            {
                ltResultActive.Text = "正常";
            }
            else if (data[0].Active < 0)
            {
                ltResultActive.Text = "取消";
            }
            else
            {
                ltResultActive.Text = "暂停";
            }
            ltResultAddTime.Text = data[0].AddTime.ToString("yyyy-MM-dd HH:mm:ss");
            ltResultAddIp.Text = data[0].AddIp;
            ltResultAddUser.Text = data[0].AddUser;
            if (!string.IsNullOrEmpty(data[0].Body))
            {
                string[] arrTitles = data[0].Titles.Split('\n');
                string[] arrBody = data[0].Body.Split('\n');
                for (int j = 0; j < arrBody.Count(); j++)
                {
                    arrBody[j] = "<li><b>" + arrTitles[j] + "</b><br/>[ " + arrBody[j] + " ]</li>";
                }
                ltResultBody.Text = "<ol>" + string.Join("", arrBody) + "</ol>";
            }
        }
        //下载数据
        private void downXls(int sId, WebSurveyResult webResult, string strSubType, string strTitle)
        {
            if (myUser == null)
            {
                return;
            }
            string strOrderBy = "AddTime ASC";
            DataSurveyResult[] data = webResult.GetDatas(0, sId, 0, "", 1, 0, strOrderBy);
            if (data != null)
            {
                DateTime dtNow = DateTime.Now;
                string virtualPath = string.Format("../download/{0:yyyy}/{0:MM}/", dtNow);
                string filepath = HttpContext.Current.Server.MapPath(virtualPath);
                if (!System.IO.Directory.Exists(filepath))
                {
                    System.IO.Directory.CreateDirectory(filepath);
                }
                strTitle = strSubType + "《" + strTitle + "》";
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
                workSheet.Rows["3:4"].RowHeight = 20;
                int intCol = 6;
                workSheet.Range[workSheet.Cells[3, 1], workSheet.Cells[3, intCol]].MergeCells = true;//合并单元格
                PublicMod.SetCells(workSheet, 3, 1, strTitle, "center,bold");
                PublicMod.SetCells(workSheet, 4, 1, "序号", "center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 4, 2, "反馈委员", "fit,center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 4, 3, "反馈内容", "center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 4, 4, "得分", "center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 4, 5, "提交时间", "center,bold,border", "LightGray");
                PublicMod.SetCells(workSheet, 4, 6, "状态", "center,bold,border", "LightGray");
                WebUser webUser = new WebUser();
                for (int i = 0; i < data.Count(); i++)
                {
                    int intRow = i + 5;
                    PublicMod.SetCells(workSheet, intRow, 1, (i + 1).ToString(), "center,border");
                    string strUser = "";
                    if (data[i].UserId > 0)
                    {
                        DataUser[] uData = webUser.GetData(data[i].UserId, "TrueName");
                        if (uData != null)
                        {
                           strUser = uData[0].TrueName;
                        }
                    }
                    PublicMod.SetCells(workSheet, intRow, 2, strUser, "txt,center,border");
                    string strBody = "";
                    if (!string.IsNullOrEmpty(data[i].Body))
                    {
                        string[] arrTitles = data[i].Titles.Split('\n');
                        string[] arrBody = data[i].Body.Split('\n');
                        for (int j = 0; j < arrBody.Count(); j++)
                        {
                            arrBody[j] = (j + 1).ToString() + "、" + arrTitles[j] + "\n[ " + arrBody[j] + " ]";
                        }
                        strBody = string.Join("\n", arrBody);
                    }
                    workSheet.Cells[intRow, 3].ColumnWidth = 50;
                    PublicMod.SetCells(workSheet, intRow, 3, strBody, "txt,border");
                    string strScore = (ltResultSubType.Text == "答题") ? data[i].Score.ToString() : "/";
                    PublicMod.SetCells(workSheet, intRow, 4, strScore, "txt,center,border");
                    workSheet.Cells[intRow, 5].ColumnWidth = 12;
                    PublicMod.SetCells(workSheet, intRow, 5, data[i].AddTime.ToString("yyyy-MM-dd HH:mm:ss"), "date,center,border");
                    string strActive = "";
                    if (data[i].Active > 0)
                    {
                        strActive = "正常";
                    }
                    else if (data[i].Active < 0)
                    {
                        strActive = "取消";
                    }
                    else
                    {
                        strActive = "暂停";
                    }
                    PublicMod.SetCells(workSheet, intRow, 6, strActive, "txt,center,border");
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
        #endregion
        //
    }
}