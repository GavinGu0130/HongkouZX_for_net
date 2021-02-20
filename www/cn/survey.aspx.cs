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

namespace hkzx.web.cn
{
    public partial class survey : System.Web.UI.Page
    {
        private DataUser myUser = null;
        private WebSurvey webSurvey = new WebSurvey();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.TrueName))
            {
                Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.UserType = myUser.UserType;
            string strTitle = "";
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                if (!IsPostBack)
                {
                    strTitle = LoadData(myUser, plEdit, Convert.ToInt32(Request.QueryString["id"]), ltInfo, ltTitle, hfMaxNum, hfMinNum, rpOpList, ltOpNo, plSub);
                }
            }
            else
            {
                ListData(plList, myUser.TrueName, rpList, ltNo, lblNav, ltTotal, this);
            }
            Header.Title += " - 问卷调查" + strTitle;
        }
        //获取未反馈数
        public int GetFeedNum(DataUser uData)
        {
            int intNum = 0;
            DateTime dtNow = DateTime.Now;
            DataSurvey qData = new DataSurvey();
            qData.ActiveName = ">0";
            qData.ToMans = uData.TrueName;
            qData.StartTimeText = "," + dtNow.ToString("yyyy-MM-dd HH:mm:ss");
            qData.EndTimeText = dtNow.ToString("yyyy-MM-dd HH:mm:ss") + ",";
            DataSurvey[] data = webSurvey.GetDatas(qData, "Id");
            if (data != null)
            {
                intNum = data.Count();
                WebSurveyResult webResult = new WebSurveyResult();
                for (int i = 0; i < data.Count(); i++)
                {
                    DataSurveyResult[] resData = webResult.GetDatas(1, data[i].Id, uData.Id, "Id");
                    if (resData != null)
                    {
                        intNum--;
                    }
                }
            }
            return intNum;
        }
        //加载列表
        public void ListData(PlaceHolder _plList, string strUser, Repeater _rpList, Literal _ltNo = null, Label _lblNav = null, Literal _ltTotal = null, Page page = null)
        {
            _plList.Visible = true;
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
            DateTime dtNow = DateTime.Now;
            DataSurvey qData = new DataSurvey();
            qData.ActiveName = ">0";
            qData.ToMans = strUser;
            qData.StartTimeText = "," + dtNow.ToString("yyyy-MM-dd HH:mm:ss");
            qData.EndTimeText = dtNow.ToString("yyyy-MM-dd HH:mm:ss") + ",";
            DataSurvey[] data = webSurvey.GetDatas(qData, "Id,SubType,Title,SurveyNum,StartTime,EndTime,Active", pageCur, pageSize, "", "total");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    data[i].EndTimeText = data[i].EndTime.ToString("yyyy-MM-dd HH:mm");
                    if (data[i].SurveyNum == 0)
                    {
                        data[i].SurveyNumText = "1次";
                    }
                    else if (data[i].SurveyNum > 0)
                    {
                        data[i].SurveyNumText = data[i].SurveyNum.ToString() + "次/天";
                    }
                    else
                    {
                        data[i].SurveyNumText = "*";
                    }
                }
                _rpList.DataSource = data;
                _rpList.DataBind();
                if (_ltNo != null)
                {
                    _ltNo.Visible = false;
                }
                if (_lblNav != null)
                {
                    int pageCount = data[0].total;
                    int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                    string lnk = page.Request.Url.ToString();
                    _lblNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
                }
                if (_ltTotal != null)
                {
                    _ltTotal.Text = data[0].total.ToString();
                }
            }
        }
        //加载信息
        public string LoadData(DataUser uData, PlaceHolder _plEdit, int Id, Literal _ltInfo, Literal _ltTitle, HiddenField _hfMaxNum, HiddenField _hfMinNum, Repeater _rpList, Literal _ltNo, Panel _plSub)
        {
            _plEdit.Visible = true;
            string strTitle = "";
            if (Id > 0)
            {
                WebSurveyResult webResult = new WebSurveyResult();
                DataSurvey surData = getSurvey(uData, Id, webResult, _ltInfo);
                if (surData != null)
                {
                    strTitle = " - " + surData.Title;
                    _ltTitle.Text = surData.Title;
                    WebSurveyOp webOp = new WebSurveyOp();
                    DataSurveyOp[] data = webOp.GetDatas(1, Id, "");
                    if (data != null)
                    {
                        for (int i = 0; i < data.Count(); i++)
                        {
                            data[i].num = i + 1;
                            if (surData.SubType == "投票")
                            {
                                _hfMaxNum.Value = surData.MaxNum.ToString();
                                _hfMinNum.Value = surData.MinNum.ToString();
                                data[i].Title = string.Format("<img src='{0}' /><input type='checkbox' name='_rd{1}' value='{2}'/> {3}", data[i].PicUrl, i, data[i].Id, data[i].Title);
                                data[i].Body = "";
                            }
                            else
                            {
                                string strBody = "";
                                switch (data[i].Method)
                                {
                                    case "单选":
                                        strBody += getRadio(data[i].Body, i);
                                        break;
                                    case "多选":
                                        strBody += getCheckbox(data[i].Body, i);
                                        break;
                                    default:
                                        break;
                                }
                                data[i].Body = strBody;
                            }
                        }
                        _rpList.DataSource = data;
                        _rpList.DataBind();
                        _ltNo.Visible = false;
                        _plSub.Visible = true;
                    }
                }
            }
            return strTitle;
        }
        //处理单选
        private string getRadio(string strBody, int intNo)
        {
            if (!string.IsNullOrEmpty(strBody))
            {
                string[] arr = strBody.Split('\n');
                strBody = "";
                for (int i = 0; i < arr.Count(); i++)
                {
                    strBody += string.Format("<input type='radio' name='_rd{0}' value='{1}'/>{1} ", intNo, arr[i]);
                }
            }
            return strBody;
        }
        //处理多选
        private string getCheckbox(string strBody, int intNo)
        {
            if (!string.IsNullOrEmpty(strBody))
            {
                string[] arr = strBody.Split('\n');
                strBody = "";
                for (int i = 0; i < arr.Count(); i++)
                {
                    strBody += string.Format("<input type='checkbox' name='_ck{0}' value='{1}'/>{1} ", intNo, arr[i]);
                }
            }
            return strBody;
        }

        //获取调查标题对象
        private DataSurvey getSurvey(DataUser uData, int Id, WebSurveyResult webResult, Literal _ltInfo)
        {
            if (Id <= 0)
            {
                return null;
            }
            DataSurvey[] data = webSurvey.GetData(Id, "Id,SubType,Title,SurveyNum,StartTime,EndTime,MaxNum,MinNum,Active");
            if (data == null)
            {
                return null;
            }
            if (data[0].Active <= 0 || data[0].EndTime <= DateTime.Now || data[0].StartTime >= DateTime.Now)
            {
                _ltInfo.Text = "<script>$(function(){ alert('“问卷调查”已结束或未开始！'); window.history.back(-1); });</script>";
                return null;
            }
            if (data[0].SurveyNum >= 0)
            {
                DataSurveyResult[] resData = webResult.GetDatas(1, Id, uData.Id, "Id,AddTime");
                if (resData != null)
                {
                    if (data[0].SurveyNum > 0)
                    {
                        int intNum = 0;
                        for (int i = 0; i < resData.Count(); i++)
                        {
                            if (resData[i].AddTime >= DateTime.Today)
                            {
                                intNum++;
                            }
                        }
                        if (intNum >= data[0].SurveyNum)
                        {
                            string strTxt = "您今天参与次数已满了，\\n请明天再参与，";
                            if (DateTime.Today.AddDays(1) > data[0].EndTime)
                            {
                                strTxt += "\\n请明天再参与，";
                            }
                            strTxt += "谢谢！";
                            _ltInfo.Text = "<script>$(function(){ alert('" + strTxt + "'); window.history.back(-1); });</script>";
                            return null;
                        }
                    }
                    else
                    {
                        _ltInfo.Text = "<script>$(function(){ alert('您已参与过“调查”了，谢谢！'); window.history.back(-1); });</script>";
                        return null;
                    }
                }
            }
            return data[0];
        }

        //处理提交数据
        protected void btnSub_Click(object sender, EventArgs e)
        {
            SubData(myUser, ltInfo, rpOpList, this);
        }
        public void SubData(DataUser uData, Literal _ltInfo, Repeater _rpList, Page page)
        {
            if (uData == null)
            {
                return;
            }
            WebSurveyResult webResult = new WebSurveyResult();
            int SurveyId = (!string.IsNullOrEmpty(page.Request.QueryString["id"])) ? Convert.ToInt32(page.Request.QueryString["id"]) : 0;
            DataSurvey surData = getSurvey(uData, SurveyId, webResult, _ltInfo);
            if (surData == null)
            {
                return;
            }
            string strTitles = "";
            string strBody = "";
            int intScore = 0;
            WebSurveyOp webOp = (surData.SubType == "投票" || surData.SubType == "答题") ? new WebSurveyOp() : null;
            ArrayList arrList = new ArrayList();
            for (int i = 0; i < _rpList.Items.Count; i++)
            {
                HiddenField title = (HiddenField)_rpList.Items[i].FindControl("_title");
                TextBox txt = (TextBox)_rpList.Items[i].FindControl("_txt");
                if (!string.IsNullOrEmpty(txt.Text))
                {
                    strTitles += title.Value + "\n";//问卷、答题的题目，投票的选项名
                    strBody += txt.Text + "\n";//回答的答案，投票选项的Id
                    arrList.Add(txt.Text);
                }
                else if (surData.SubType != "投票")
                {//问卷、答题 必答判断
                    return;
                }
                if (surData.SubType == "答题")
                {//是否计分
                    HiddenField id = (HiddenField)rpOpList.Items[i].FindControl("_id");
                    DataSurveyOp[] opData = webOp.GetData(Convert.ToInt32(id.Value), "Answer,Score,Active");
                    if (opData != null && opData[0].Active > 0)
                    {
                        if (txt.Text == opData[0].Answer)
                        {
                            intScore += opData[0].Score;
                        }
                    }
                }
            }
            if (surData.SubType == "投票" && arrList.Count > 0)
            {//如果是投票，并且 <=最大投票数，>=最小投票数，则计票
                if (arrList.Count >= surData.MinNum && arrList.Count <= surData.MaxNum)
                {
                    if (webOp.AddVoteNum(arrList) > 0)
                    {
                        //增加计票
                    }
                    else
                    {
                        _ltInfo.Text = "<script>$(function(){ alert('“计票”失败，请稍后重试！'); window.history.back(-1); });</script>";
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            if (!string.IsNullOrEmpty(strTitles))
            {
                strTitles = strTitles.Trim('\n');
            }
            if (!string.IsNullOrEmpty(strBody))
            {
                strBody = strBody.Trim('\n');
            }
            DataSurveyResult data = new DataSurveyResult();
            data.SurveyId = SurveyId;
            data.UserId = uData.Id;
            data.Titles = strTitles;
            data.Body = strBody;
            data.Score = intScore;
            //data.Remark
            data.Active = 1;
            data.AddTime = DateTime.Now;
            data.AddIp = HelperMain.GetIpPort();
            data.AddUser = uData.TrueName;
            int intId = webResult.Insert(data);
            if (intId > 0)
            {
                string strBack = PublicMod.GetBackUrl();
                _ltInfo.Text = "<script>$(function(){ alert('“提交”成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                _ltInfo.Text = "<script>$(function(){ alert('“提交”失败！'); window.history.back(-1); });</script>";
            }
        }
        //
    }
}