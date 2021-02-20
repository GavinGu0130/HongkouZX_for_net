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
    public partial class forum : System.Web.UI.Page
    {
        private DataAdmin myUser = null;
        private WebForumBoard webBoard = new WebForumBoard();
        private DataForumBoard boardData = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperAdmin.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
            {
                //Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            if (myUser.Powers.IndexOf("alls") < 0 && myUser.Powers.IndexOf("forum") < 0)
            {
                Response.Redirect("./");
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.Powers = myUser.Powers;

            string strTitle = "委员论坛";
            if (!string.IsNullOrEmpty(Request.QueryString["bid"]))
            {//帖子管理
                int BoardId = Convert.ToInt32(Request.QueryString["bid"]);
                boardData = getBoard(BoardId);
                if (boardData == null)
                {
                    return;
                }
                strTitle += " - " + boardData.BoardName;
                plNav2.Visible = true;
                lnkBoard.NavigateUrl += BoardId.ToString();
                lnkBoard.Text = "[" + boardData.BoardName + "]";
                hfBoardId.Value = BoardId.ToString();//回复、编辑帖子

                if (!string.IsNullOrEmpty(Request.QueryString["ac"]) && !string.IsNullOrEmpty(Request.QueryString["id"]))
                {//处理帖子
                    updateActive(Convert.ToInt32(Request.QueryString["id"]), Request.QueryString["ac"], BoardId, ltInfo);
                }
                else if (!string.IsNullOrEmpty(Request.QueryString["tid"]))
                {//主题帖
                    plPost.Visible = true;
                    ltPostTitle.Text = listPost(BoardId, Convert.ToInt32(Request.QueryString["tid"]), rpPostList, lblPostNav, lnkTopic, ltReadNum, ltReplyNum);
                    strTitle += " - " + ltPostTitle.Text;
                }
                else if (!string.IsNullOrEmpty(Request.QueryString["rid"]))
                {//回复帖子
                    if (!IsPostBack)
                    {
                        hfBack.Value = PublicMod.GetBackUrl("?bid=" + BoardId.ToString());
                        plEdit.Visible = true;
                        strTitle += " - " + loadEdit(BoardId, 0, Convert.ToInt32(Request.QueryString["rid"]), lnkTopic, lnkEdit, hfTopicId, hfId, ltTitle, txtTitle, txtBody, btnEdit);
                    }
                }
                else if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {//编辑帖子
                    if (!IsPostBack)
                    {
                        hfBack.Value = PublicMod.GetBackUrl();
                        plEdit.Visible = true;
                        strTitle += " - " + loadEdit(BoardId, Convert.ToInt32(Request.QueryString["id"]), 0, lnkTopic, lnkEdit, hfTopicId, hfId, ltTitle, txtTitle, txtBody, btnEdit);
                    }
                }
                else
                {//主题列表
                    plTopic.Visible = true;
                    ltBoardRule.Text = boardData.BoardRule;
                    ltBoardMaster.Text = boardData.BoardMaster;
                    listTopic(BoardId, rpTopicList, ltTopicNo, lblTopicNav, lnkBoard);
                }
            }
            else
            {//论坛管理、版块设置
                if (!IsPostBack)
                {
                    hfBack.Value = PublicMod.GetBackUrl();
                    if (Request.QueryString["ac"] == "meeting")
                    {
                        if (myUser.Grade < 9 || (myUser.Powers.IndexOf("alls") < 0 && myUser.Powers.IndexOf("system") < 0))
                        {
                            Response.Redirect("./");
                        }
                        strTitle = " - 全会时间设置";
                        plMeeting.Visible = true;
                        loadMeeting();
                    }
                    else
                    {
                        plNav.Visible = true;
                        if (Request.QueryString["ac"] == "set")
                        {
                            strTitle += " - 设置";
                            plSet.Visible = true;
                            loadSet();
                        }
                        else if (!string.IsNullOrEmpty(Request.QueryString["tid"]))
                        {
                            //strTitle += " - ";
                            plBbs.Visible = true;
                            //WebDatas webDatas = new WebDatas();
                            //listDatas(Convert.ToInt32(Request.QueryString["tid"]), webType, webDatas);
                            //int Id = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                            //loadDatas(Id, webDatas);
                        }
                        else
                        {
                            strTitle += " - 版块";
                            plBoard.Visible = true;
                            listBoard();
                            int Id = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                            loadBoard(Id);
                        }
                    }
                }
            }
            Header.Title += " - " + strTitle;
        }
        //
        #region 全会时间设置
        private void loadMeeting()
        {
            WebForumSet webSet = new WebForumSet();
            DataForumSet[] data = webSet.GetDatas(0, "meeting");
            if (data != null)
            {
                txtMId.Text = data[0].Id.ToString();
                txtMStartDate.Text = data[0].StartDate.ToString("yyyy-MM-dd HH:mm");
                txtMEndDate.Text = data[0].EndDate.ToString("yyyy-MM-dd HH:mm");
                txtMRemark.Text = data[0].Remark;
                txtMAddTime.Text = data[0].AddTime.ToString("yyyy-MM-dd HH:mm:ss");
                txtMAddIp.Text = data[0].AddIp;
                txtMAddUser.Text = data[0].AddUser;
                if (!string.IsNullOrEmpty(data[0].UpIp) || !string.IsNullOrEmpty(data[0].UpUser))
                {
                    txtMUpTime.Text = data[0].UpTime.ToString("yyyy-MM-dd HH:mm:ss");
                    txtMUpIp.Text = data[0].UpIp;
                    txtMUpUser.Text = data[0].UpUser;
                }
                txtMLabel.Text = data[0].Label;
                btnMeeting.Text = "修改";
            }
            else
            {
                txtMAddTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                txtMAddIp.Text = HelperMain.GetIp();
                txtMAddUser.Text = myUser.AdminName;
            }
        }
        //编辑数据
        protected void btnMeeting_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            string strBack = hfBack.Value;
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(myUser.AdminName, 20);
            WebForumSet webSet = new WebForumSet();
            DataForumSet data = new DataForumSet();
            data.Id = Convert.ToInt32(txtMId.Text);
            data.StartDate = (!string.IsNullOrEmpty(txtMStartDate.Text)) ? Convert.ToDateTime(txtMStartDate.Text) : dtNow;
            data.EndDate = (!string.IsNullOrEmpty(txtMEndDate.Text)) ? Convert.ToDateTime(txtMEndDate.Text) : dtNow;
            data.Remark = HelperMain.SqlFilter(txtMRemark.Text.Trim());
            data.Active = 1;
            data.Label = HelperMain.SqlFilter(txtMLabel.Text.Trim(), 20);
            if (data.Id <= 0)
            {
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = strUser;
                data.Id = webSet.Insert(data);
            }
            else
            {
                data.UpTime = dtNow;
                data.UpIp = strIp;
                data.UpUser = strUser;
                if (webSet.Update(data) <= 0)
                {
                    data.Id = -1;
                }
            }
            if (data.Id > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltMTitle.Text + "”成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltMTitle.Text + "”失败！'); window.history.back(-1); });</script>";
            }
        }
        #endregion
        //
        #region 论坛设置
        private void loadSet()
        {
            WebForumSet webSet = new WebForumSet();
            DataForumSet[] data = webSet.GetDatas(0, "forum");//webSet.GetData(Id);
            if (data != null)
            {
                txtSetId.Text = data[0].Id.ToString();
                txtSetStartDate.Text = data[0].StartDate.ToString("yyyy-MM-dd");
                txtSetEndDate.Text = data[0].EndDate.ToString("yyyy-MM-dd");
                txtSetNoPostDate.Text = data[0].NoPostDate;
                HelperMain.SetCheckSelected(cblSetPostWeek, data[0].PostWeek);
                txtSetPostStart.Text = data[0].PostStart;
                txtSetPostEnd.Text = data[0].PostEnd;
                HelperMain.SetRadioSelected(rblSetPostActive, data[0].PostActive.ToString());
                txtSetRemark.Text = data[0].Remark;
                //txtSetActive.Text = data[0].Active.ToString();
                txtSetAddTime.Text = data[0].AddTime.ToString("yyyy-MM-dd HH:mm:ss");
                txtSetAddIp.Text = data[0].AddIp;
                txtSetAddUser.Text = data[0].AddUser;
                if (!string.IsNullOrEmpty(data[0].UpIp) || !string.IsNullOrEmpty(data[0].UpUser))
                {
                    txtSetUpTime.Text = data[0].UpTime.ToString("yyyy-MM-dd HH:mm:ss");
                    txtSetUpIp.Text = data[0].UpIp;
                    txtSetUpUser.Text = data[0].UpUser;
                }
                txtSetLabel.Text = data[0].Label;
                btnSetEdit.Text = "修改";
            }
            else
            {
                txtSetStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtSetAddTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                txtSetAddIp.Text = HelperMain.GetIp();
                txtSetAddUser.Text = myUser.AdminName;
            }
        }
        //编辑数据
        protected void btnSetEdit_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            string strBack = hfBack.Value;
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(myUser.AdminName, 20);
            WebForumSet webSet = new WebForumSet();
            DataForumSet data = new DataForumSet();
            data.Id = Convert.ToInt32(txtSetId.Text);
            data.StartDate = (!string.IsNullOrEmpty(txtSetStartDate.Text)) ? Convert.ToDateTime(txtSetStartDate.Text) : dtNow;
            data.EndDate = (!string.IsNullOrEmpty(txtSetEndDate.Text)) ? Convert.ToDateTime(txtSetEndDate.Text) : dtNow;
            data.NoPostDate = HelperMain.SqlFilter(txtSetNoPostDate.Text.Trim());
            data.PostWeek = HelperMain.GetCheckSelected(cblSetPostWeek);
            data.PostStart = HelperMain.SqlFilter(txtSetPostStart.Text.Trim(), 8);
            data.PostEnd = HelperMain.SqlFilter(txtSetPostEnd.Text.Trim(), 8);
            data.PostActive = Convert.ToInt16(rblSetPostActive.SelectedValue);
            data.Remark = HelperMain.SqlFilter(txtSetRemark.Text.Trim());
            data.Active = 1;
            data.Label = HelperMain.SqlFilter(txtSetLabel.Text.Trim(), 20);
            if (data.Id <= 0)
            {
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = strUser;
                data.Id = webSet.Insert(data);
            }
            else
            {
                data.UpTime = dtNow;
                data.UpIp = strIp;
                data.UpUser = strUser;
                if (webSet.Update(data) <= 0)
                {
                    data.Id = -1;
                }
            }
            if (data.Id > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltSetTitle.Text + "”成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltSetTitle.Text + "”失败！'); window.history.back(-1); });</script>";
            }
        }
        #endregion
        //
        #region 版块设置
        //加载列表
        private void listBoard()
        {
            DataForumBoard[] data = webBoard.GetDatas(0, "", "Id,BoardName,BoardRule,BoardMaster,Remark,Active");
            if (data != null)
            {
                WebForumBbs webBbs = new WebForumBbs();
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = i + 1;
                    if (data[i].Active > 0)
                    {
                        data[i].ActiveName = "正常";
                    }
                    else
                    {
                        data[i].ActiveName = "取消";
                        data[i].rowClass = " class='cancel'";
                    }
                    DataForumBbs[] topic = webBbs.GetDatas(">0", data[i].Id, 0, "", "Title,AddTime,AddUser", 1, 1, "AddTime DESC,UpTime DESC,ReplyTime DESC");
                    if (topic != null)
                    {
                        data[i].TopicTitle = topic[0].Title;
                        data[i].TopicUser = topic[0].AddUser;
                        data[i].TopicTime = topic[0].AddTime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    DataForumBbs[] ttp = webBbs.GetTodayTopicPost(0, data[i].Id);
                    if (ttp != null)
                    {
                        data[i].TodayNum = ttp[0].TodayNum;
                        data[i].TopicNum = ttp[0].TopicNum;
                        data[i].PostNum = ttp[0].PostNum;
                    }
                }
                rpBoardList.DataSource = data;
                rpBoardList.DataBind();
                ltBoardNo.Visible = false;
            }
        }
        //加载信息
        private void loadBoard(int Id)
        {
            if (Id <= 0)
            {
                txtBoardAddTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                txtBoardAddIp.Text = HelperMain.GetIp();
                txtBoardAddUser.Text = myUser.AdminName;
                return;
            }
            DataForumBoard[] data = webBoard.GetData(Id);
            if (data != null)
            {
                txtBoardId.Text = data[0].Id.ToString();
                txtBoardName.Text = data[0].BoardName;
                txtBoardRule.Text = data[0].BoardRule;
                txtBoardMaster.Text = data[0].BoardMaster;
                HelperMain.SetRadioSelected(rblBoardPostActive, data[0].PostActive.ToString());
                txtBoardRemark.Text = data[0].Remark;
                txtBoardActive.Text = data[0].Active.ToString();
                txtBoardAddTime.Text = data[0].AddTime.ToString("yyyy-MM-dd HH:mm:ss");
                txtBoardAddIp.Text = data[0].AddIp;
                txtBoardAddUser.Text = data[0].AddUser;
                if (!string.IsNullOrEmpty(data[0].UpIp) || !string.IsNullOrEmpty(data[0].UpUser))
                {
                    txtBoardUpTime.Text = data[0].UpTime.ToString("yyyy-MM-dd HH:mm:ss");
                    txtBoardUpIp.Text = data[0].UpIp;
                    txtBoardUpUser.Text = data[0].UpUser;
                }
                btnBoardEdit.Text = "修改";
                ltBoardTitle.Text = ltBoardTitle.Text.Replace("新增", "修改");
                btnBoardCancel.Visible = true;
            }
        }
        //编辑数据
        protected void btnBoardEdit_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            string strBack = hfBack.Value;
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(myUser.AdminName, 20);
            DataForumBoard data = new DataForumBoard();
            data.Id = Convert.ToInt32(txtBoardId.Text);
            data.BoardName = HelperMain.SqlFilter(txtBoardName.Text.Trim(), 20);
            DataForumBoard[] ckData = webBoard.GetDatas(0, data.BoardName, "Id");//重复检查
            if (ckData != null && ckData[0].Id != data.Id)
            {
                ltInfo.Text = "<script>$(function(){ alert('“版块名称”重复，不能添加！'); window.history.back(-1); });</script>";
                return;
            }
            data.BoardRule = HelperMain.SqlFilter(txtBoardRule.Text.Trim());
            data.BoardMaster = HelperMain.SqlFilter(txtBoardMaster.Text.Trim());
            //data.PostStart = (!string.IsNullOrEmpty(txtBoardPostStart.Text)) ? Convert.ToDateTime(txtBoardPostStart.Text) : dtNow;
            //data.PostEnd = (!string.IsNullOrEmpty(txtBoardPostEnd.Text)) ? Convert.ToDateTime(txtBoardPostEnd.Text) : dtNow;
            data.PostActive = Convert.ToInt16(rblBoardPostActive.SelectedValue);
            data.Remark = HelperMain.SqlFilter(txtBoardRemark.Text.Trim());
            data.Active = (!string.IsNullOrEmpty(txtBoardActive.Text.Trim())) ? Convert.ToInt16(txtBoardActive.Text.Trim()) : 1;
            if (data.Id <= 0)
            {
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = strUser;
                data.Id = webBoard.Insert(data);
            }
            else
            {
                data.UpTime = dtNow;
                data.UpIp = strIp;
                data.UpUser = strUser;
                if (webBoard.Update(data) <= 0)
                {
                    data.Id = -1;
                }
            }
            if (data.Id > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltBoardTitle.Text + "”成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltBoardTitle.Text + "”失败！'); window.history.back(-1); });</script>";
            }
        }
        //取消状态
        protected void btnBoardCancel_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            int Id = Convert.ToInt32(txtBoardId.Text);
            if (Id <= 0)
            {
                return;
            }
            if (webBoard.UpdateActive(Id, -1) > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnBoardCancel.Text + "成功！'); window.location.href='" + hfBack.Value + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('" + btnBoardCancel.Text + "失败！'); window.history.back(-1); });</script>";
            }
        }
        #endregion
        //
        #region 论坛帖子管理
        //加载论坛版块信息
        private DataForumBoard getBoard(int BoardId)
        {
            if (BoardId <= 0)
            {
                Response.Redirect("forum.aspx");
                return null;
            }
            DataForumBoard[] board = webBoard.GetData(BoardId, "BoardName,BoardRule,BoardMaster,Active");
            if (board == null)
            {
                Response.Redirect("forum.aspx");
                return null;
            }
            //string strMaster = board[0].BoardMaster;
            //if (strMaster != "" && ("|" + strMaster + "|").IndexOf("|" + strUser + "|") >= 0)
            //{
            //    board[0].IsMaster = true;
            //}
            //WebForumSet webSet = new WebForumSet();
            //DataForumSet[] setData = webSet.GetDatas(0);
            //if (setData != null)
            //{
            //    bool blIsPost = true;
            //    if (!string.IsNullOrEmpty(setData[0].NoPostDate))
            //    {
            //        string[] arr = setData[0].NoPostDate.Split('\n');
            //        for (int i = 0; i < arr.Count(); i++)
            //        {
            //            if (DateTime.Today == Convert.ToDateTime(arr[i]))
            //            {
            //                blIsPost = false;
            //                break;
            //            }
            //        }
            //    }
            //    DateTime dtNow = DateTime.Now;
            //    int intWeek = Convert.ToInt16(dtNow.DayOfWeek);
            //    DateTime PostStart = Convert.ToDateTime(dtNow.ToString("yyyy-MM-dd") + " " + setData[0].PostStart);
            //    DateTime PostEnd = Convert.ToDateTime(dtNow.ToString("yyyy-MM-dd") + " " + setData[0].PostEnd);
            //    if (blIsPost && dtNow >= setData[0].StartDate && dtNow <= setData[0].EndDate && setData[0].PostWeek.IndexOf(intWeek.ToString()) >= 0 && dtNow > PostStart && dtNow < PostEnd)
            //    {
            //        board[0].IsPost = true;
            //    }
            //    board[0].PostActive = setData[0].PostActive;
            //}
            return board[0];
        }
        //版块页面加载主题列表
        private void listTopic(int BoardId, Repeater rpList, Literal ltNo, Label lblNav, HyperLink hLnk)
        {
            hLnk.CssClass = "cur";
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            WebForumBbs webBbs = new WebForumBbs();
            string ActiveName = ">-400";
            DataForumBbs[] data = webBbs.GetDatas(ActiveName, BoardId, 0, "", "Id,BoardId,TopicId,Title,Active,AddUser,ReadNum,ReplyNum,ReplyTime,ReplyUser", pageCur, pageSize, "BbsTop DESC, ReplyTime DESC, UpTime DESC, AddTime DESC", "total");//加载主题最后发帖
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    if (data[i].Active <= -444)
                    {
                        data[i].rowClass = " class='del' title='删除'";
                    }
                    else if (data[i].Active < 0)
                    {
                        data[i].rowClass = " class='cancel' title='锁定'";
                    }
                    else if (data[i].Active <= 0)
                    {
                        data[i].rowClass = " class='save' title='待审核'";
                    }
                    else if (data[i].Active < 10)
                    {
                        data[i].rowClass = " class='wait'";
                    }
                }
                rpList.DataSource = data;
                rpList.DataBind();
                ltNo.Visible = false;
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = Request.Url.ToString();
                lblNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
            }
        }
        //主题帖加载帖子列表
        private string listPost(int BoardId, int TopicId, Repeater rpList, Label lblNav, HyperLink hLnk, Literal readNum, Literal replyNum)
        {
            hLnk.NavigateUrl = string.Format(hLnk.NavigateUrl, BoardId, TopicId);
            hLnk.Text = hLnk.Text.Replace("返回", "");
            hLnk.CssClass = "cur";
            hLnk.Visible = true;
            if (TopicId <= 0)
            {
                Response.Redirect("forum.aspx?bid=" + BoardId.ToString());
                return "";
            }
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            WebForumBbs webBbs = new WebForumBbs();
            DataForumBbs[] data = webBbs.GetDatas(">-400", BoardId, TopicId, "", "Id,BoardId,TopicId,Title,Body,Active,AddTime,AddUser,UserId,UpTime,UpUser,ReadNum,ReplyNum,ReplyTime,ReplyUser", pageCur, pageSize, "TopicId ASC, AddTime ASC", "total");//加载帖子列表
            if (data == null)// || data[0].Active < 0
            {
                Response.Redirect("forum.aspx?bid=" + BoardId.ToString());
            }
            webBbs.UpdateReadNum(TopicId);//增加阅读数
            readNum.Text = (data[0].ReadNum + 1).ToString();
            replyNum.Text = data[0].ReplyNum.ToString();
            WebUser webUser = new WebUser();
            for (int i = 0; i < data.Count(); i++)
            {
                data[i].num = (pageCur - 1) * pageSize + i + 1;
                if (data[i].Active > 0 && data[i].Active < 10)
                {
                    data[i].rowClass = " class='wait'";
                }
                else if (data[i].Active == 0)
                {
                    data[i].rowClass = " class='save' title='待审核'";
                }
                else if (data[i].Active <= -400)
                {
                    data[i].rowClass = " class='del' title='删除'";
                }
                else if (data[i].Active < 0)
                {
                    data[i].rowClass = " class='cancel' title='锁定'";
                }
                DataUser[] uData = webUser.GetData(data[i].UserId, "UserSex,Photo");
                if (uData != null)
                {
                    data[i].UserSex = uData[0].UserSex;
                    if (!string.IsNullOrEmpty(uData[0].Photo))
                    {
                        data[i].UserPhoto = string.Format("<img src='{0}' >", uData[0].Photo);
                    }
                }
                if (!string.IsNullOrEmpty(data[i].UpIp) || !string.IsNullOrEmpty(data[i].UpUser))
                {
                    data[i].UpTimeText = "[" + data[i].UpTime.ToString("yyyy-MM-dd HH:mm:ss") + "]";
                }
                string strBtnEdit = string.Format("<a href='?bid={0}&id={1}'>编辑</a>", data[i].BoardId, data[i].Id, TopicId);
                if (data[i].Active > 0)
                {
                    if (data[i].Active < 10)
                    {
                        strBtnEdit += string.Format(" <a href='?bid={0}&id={1}&ac=pass' class='cmd'>审核通过</a>", data[i].BoardId, data[i].Id, TopicId);
                    }
                    else
                    {
                        //strBtnEdit += string.Format(" <a href='?bid={0}&id={1}&ac=save' class='cmd'>重新审核</a>", data[i].BoardId, data[i].Id, TopicId);
                    }
                    strBtnEdit += string.Format(" <a href='?bid={0}&id={1}&ac=lock' class='cmd'>锁定</a>", data[i].BoardId, data[i].Id, TopicId);
                }
                else if (data[i].Active < 0)
                {
                    strBtnEdit += string.Format(" <a href='?bid={0}&id={1}&ac=unlock' class='cmd'>解锁</a>", data[i].BoardId, data[i].Id, TopicId);
                    data[i].Body = "===帖子已被锁定，以下内容不可见===[quote]" + data[i].Body + "[/quote]";
                }
                else
                {
                    strBtnEdit += string.Format(" <a href='?bid={0}&id={1}&ac=pass' class='cmd'>审核通过</a>", data[i].BoardId, data[i].Id, TopicId);
                    data[i].Body = "===帖子待审核，以下内容不可见===[quote]" + data[i].Body + "[/quote]";
                }
                strBtnEdit += string.Format(" <a href='?bid={0}&id={1}&ac=del' class='cmd'><b>删除</b></a>", data[i].BoardId, data[i].Id, TopicId);
                data[i].BtnEdit = strBtnEdit;
            }
            rpList.DataSource = data;
            rpList.DataBind();
            int pageCount = data[0].total;
            int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
            string lnk = Request.Url.ToString();
            lblNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
            return data[0].Title;
        }
        //版主处理帖子状态
        private void updateActive(int Id, string strAc, int BoardId, Literal ltOut)
        {
            string strBack = PublicMod.GetBackUrl("?bid=" + BoardId.ToString());
            if (Id <= 0)
            {
                Response.Redirect(strBack);
                return;
            }
            WebForumBbs webBbs = new WebForumBbs();
            DataForumBbs[] data = webBbs.GetData(Id, "Active,UserId");
            if (data == null || data[0].Active <= -400)
            {
                Response.Redirect(strBack);
                return;
            }
            string strActive = "";
            int Active = 0;
            switch (strAc)
            {
                case "pass":
                    strActive = "审核";
                    Active = 10;//管理审核通过
                    break;
                case "save":
                    strActive = "重新审核";
                    Active = 10;//管理重新审核
                    break;
                case "del":
                    strActive = "删除";
                    Active = -444;//管理删除
                    break;
                case "lock":
                    strActive = "锁定";
                    Active = -1;//管理锁定
                    break;
                case "unlock":
                    strActive = "解锁";
                    Active = 10;//管理解锁
                    break;
                default:
                    break;
            }
            if (webBbs.UpdateActive(Id, Active) > 0)
            {
                ltOut.Text = "<script>$(function(){ alert('“" + strActive + "帖子”成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                ltOut.Text = "<script>$(function(){ alert('“" + strActive + "帖子”失败！'); window.history.back(-1); });</script>";
            }
        }
        //加载帖子数据：ReplyID>0为回复，Id>0为修改，Id<=0为发表
        private string loadEdit(int BoardId, int Id, int ReplyId, HyperLink lnkTopic2, HyperLink lnkEdit2, HiddenField hfTopicId2, HiddenField hfId2, Literal ltTitle2, TextBox txtTitle2, TextBox txtBody2, Button btnEdit2)
        {
            string strTitle = "";
            lnkEdit2.CssClass = "cur";
            lnkEdit2.Visible = true;
            if (ReplyId > 0)
            {
                WebForumBbs webBbs = new WebForumBbs();
                DataForumBbs[] data = webBbs.GetData(ReplyId, "Id,BoardId,Title,Active");
                if (data != null && data[0].Active >= 0 && data[0].BoardId == BoardId)
                {
                    strTitle = " [回复帖子]";
                    lnkTopic2.NavigateUrl = string.Format(lnkTopic2.NavigateUrl, BoardId, data[0].Id);
                    lnkTopic2.Visible = true;
                    lnkEdit2.NavigateUrl = "?bid=" + BoardId.ToString() + "&rid=" + ReplyId.ToString();
                    lnkEdit2.Text = "回复帖子";
                    if (ltTitle2 != null)
                    {
                        ltTitle2.Text = ltTitle2.Text.Replace("发表", "回复");
                    }

                    hfTopicId2.Value = data[0].Id.ToString();
                    txtTitle2.Text = data[0].Title;
                    txtTitle2.ReadOnly = true;
                    txtTitle2.CssClass += " readonly";
                }
            }
            else if (Id > 0)
            {
                WebForumBbs webBbs = new WebForumBbs();
                DataForumBbs[] data = webBbs.GetData(Id, "Id,BoardId,TopicId,Title,Body,Active,UserId");
                if (data != null && data[0].Active >= 0 && data[0].BoardId == BoardId)
                {
                    strTitle = " [修改帖子]";
                    int TopicId = (data[0].TopicId > 0) ? data[0].TopicId : data[0].Id;
                    lnkTopic2.NavigateUrl = string.Format(lnkTopic2.NavigateUrl, BoardId, TopicId);
                    lnkTopic2.Visible = true;
                    lnkEdit2.NavigateUrl = string.Format(lnkEdit2.NavigateUrl, BoardId, Id);
                    lnkEdit2.Text = "修改帖子";
                    if (ltTitle2 != null)
                    {
                        ltTitle2.Text = ltTitle2.Text.Replace("发表", "修改");
                    }
                    btnEdit2.Text = "修改";

                    hfId2.Value = data[0].Id.ToString();
                    hfTopicId2.Value = data[0].TopicId.ToString();
                    txtTitle2.Text = data[0].Title;
                    txtBody2.Text = data[0].Body;
                }
            }
            else
            {
                strTitle = " [发表帖子]";
                lnkEdit2.NavigateUrl = string.Format(lnkEdit2.NavigateUrl, BoardId, Id);
                lnkEdit2.Text = "发表帖子";
            }
            return strTitle;
        }
        //编辑数据
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            ltInfo.Text = subEdit(hfBack.Value, -1, myUser.TrueName, Convert.ToInt32(hfBoardId.Value), Convert.ToInt32(hfTopicId.Value), Convert.ToInt32(hfId.Value), txtTitle.Text, txtBody.Text, ltTitle.Text);
        }
        //处理编辑
        private string subEdit(string strBack, int UserId, string TrueName, int BoardId, int TopicId, int Id, string Title, string Body, string Info)
        {
            string strOut = "";
            WebForumBbs webBbs = new WebForumBbs();
            DataForumBbs data = new DataForumBbs();
            data.Id = Id;
            data.BoardId = BoardId;
            if (data.BoardId <= 0)
            {
                //Response.Redirect(strBack);
                return "<script>$(function(){ alert('“版块参数”错误！'); window.history.back(-1); });</script>";
            }
            data.TopicId = TopicId;
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(TrueName, 20);
            data.Title = HelperMain.SqlFilter(Title.Trim(), 50);
            DataForumBbs[] ckData = webBbs.GetDatas(">-400", data.BoardId, -100, data.Title, "Id,AddTime,UserId");//重复检查
            if (ckData != null && ckData[0].Id != data.Id && ckData[0].UserId != UserId && ckData[0].AddTime.AddSeconds(-30) > dtNow)//
            {
                //Response.Redirect(strBack);
                return "<script>$(function(){ alert('重复提交啦！'); });</script>";
            }
            data.Body = HelperMain.SqlFilter(Body.TrimEnd());
            data.Active = 10;//boardData.PostActive;
            if (data.Id <= 0)
            {
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = strUser;
                data.UserId = UserId;
                data.Id = webBbs.Insert(data);
                if (data.TopicId > 0)
                {//处理回复帖子
                    webBbs.UpdateReply(data.TopicId, data.AddUser, data.AddIp, data.AddTime);//更新主题的最后回复
                }
            }
            else
            {
                data.UserId = -1;
                data.UpTime = dtNow;
                data.UpIp = strIp;
                data.UpUser = strUser;
                data.UserId = -10000;
                if (webBbs.Update(data) <= 0)
                {
                    data.Id = -1;
                }
            }
            if (data.Id > 0)
            {
                strOut = "<script>$(function(){ alert('“" + Info + "”成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                strOut = "<script>$(function(){ alert('“" + Info + "”失败！'); window.history.back(-1); });</script>";
            }
            return strOut;
        }
        #endregion
        //
    }
}