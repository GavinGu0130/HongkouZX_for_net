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
    public partial class forum : System.Web.UI.Page
    {
        private DataUser myUser = null;
        private WebForumBoard webBoard = new WebForumBoard();
        private DataForumBoard boardData = null;
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
            if (!string.IsNullOrEmpty(Request.QueryString["bid"]))
            {
                int BoardId = Convert.ToInt32(Request.QueryString["bid"]);
                boardData = GetBoard(BoardId, myUser.TrueName, this);
                if (boardData == null)
                {
                    return;
                }
                Header.Title += " - " + boardData.BoardName;
                plNav.Visible = true;
                lnkBoard.NavigateUrl += BoardId.ToString();
                lnkBoard.Text = "[" + boardData.BoardName + "]";
                hfBoardId.Value = BoardId.ToString();//回复、编辑帖子

                if (!string.IsNullOrEmpty(Request.QueryString["ac"]) && !string.IsNullOrEmpty(Request.QueryString["id"]))
                {//处理帖子
                    UpdateActive(Convert.ToInt32(Request.QueryString["id"]), Request.QueryString["ac"], this, BoardId, boardData.IsMaster, ltInfo);
                }
                else if (!string.IsNullOrEmpty(Request.QueryString["tid"]))
                {//主题帖
                    plPost.Visible = true;
                    ltPostTitle.Text = ListPost(BoardId, Convert.ToInt32(Request.QueryString["tid"]), rpPostList, lblPostNav, this, boardData.IsPost, boardData.IsMaster, plPostCmd, lnkTopic, ltReadNum, ltReplyNum);
                    Header.Title += " - " + ltPostTitle.Text;
                }
                else if (!string.IsNullOrEmpty(Request.QueryString["rid"]))
                {//回复帖子
                    if (!IsPostBack)
                    {
                        hfBack.Value = PublicMod.GetBackUrl("?bid=" + BoardId.ToString());
                        plEdit.Visible = true;
                        Header.Title += " - " + LoadEdit(BoardId, 0, Convert.ToInt32(Request.QueryString["rid"]), boardData.IsMaster, myUser.Id, lnkTopic, lnkEdit, hfTopicId, hfId, ltTitle, txtTitle, txtBody, btnEdit);
                    }
                }
                else if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {//编辑帖子
                    if (!IsPostBack)
                    {
                        hfBack.Value = PublicMod.GetBackUrl();
                        plEdit.Visible = true;
                        Header.Title += " - " + LoadEdit(BoardId, Convert.ToInt32(Request.QueryString["id"]), 0, boardData.IsMaster, myUser.Id, lnkTopic, lnkEdit, hfTopicId, hfId, ltTitle, txtTitle, txtBody, btnEdit);
                    }
                }
                else
                {//主题列表
                    plTopic.Visible = true;
                    ltBoardRule.Text = boardData.BoardRule;
                    ltBoardMaster.Text = boardData.BoardMaster;
                    ListTopic(BoardId, rpTopicList, ltTopicNo, lblTopicNav, this, boardData.IsPost, boardData.IsMaster, plTopicCmd, lnkBoard);
                }
            }
            else
            {//版块
                plBoard.Visible = true;
                ListBoard(rpBoardList, ltBoardNo);
            }
        }
        //论坛首页加载版块列表
        public void ListBoard(Repeater rpList, Literal ltNo)
        {
            DataForumBoard[] data = webBoard.GetDatas(1, "", "Id,BoardName,BoardRule,BoardMaster");
            if (data != null)
            {
                WebForumBbs webBbs = new WebForumBbs();
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = i + 1;
                    DataForumBbs[] topic = webBbs.GetDatas(">0", data[i].Id, 0, "", "Title,AddTime,AddUser", 1, 1, "AddTime DESC,UpTime DESC,ReplyTime DESC");//加载版块最后发帖
                    if (topic != null)
                    {
                        data[i].TopicTitle = topic[0].Title;
                        data[i].TopicUser = topic[0].AddUser;
                        data[i].TopicTime = topic[0].AddTime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    DataForumBbs[] TodayTopicPost = webBbs.GetTodayTopicPost(1, data[i].Id);
                    if (TodayTopicPost != null)
                    {
                        data[i].TodayNum = TodayTopicPost[0].TodayNum;
                        data[i].TopicNum = TodayTopicPost[0].TopicNum;
                        data[i].PostNum = TodayTopicPost[0].PostNum;
                    }
                }
                rpList.DataSource = data;
                rpList.DataBind();
                ltNo.Visible = false;
            }
        }
        //加载论坛版块信息
        public DataForumBoard GetBoard(int BoardId, string strUser, Page page)
        {
            if (BoardId <= 0)
            {
                page.Response.Redirect("forum.aspx");
                return null;
            }
            DataForumBoard[] board = webBoard.GetData(BoardId, "BoardName,BoardRule,BoardMaster,PostActive,Active");
            if (board == null || board[0].Active <= 0)
            {
                page.Response.Redirect("forum.aspx");
                return null;
            }
            string strMaster = board[0].BoardMaster;
            if (strMaster != "" && ("|" + strMaster + "|").IndexOf("|" + strUser + "|") >= 0)
            {
                board[0].IsMaster = true;
            }
            WebForumSet webSet = new WebForumSet();
            DataForumSet[] setData = webSet.GetDatas(0, "forum");
            if (setData != null)
            {
                bool blIsPost = true;
                if (!string.IsNullOrEmpty(setData[0].NoPostDate))
                {
                    string[] arr = setData[0].NoPostDate.Split('\n');
                    for (int i = 0; i < arr.Count(); i++)
                    {
                        if (DateTime.Today == Convert.ToDateTime(arr[i]))
                        {
                            blIsPost = false;
                            break;
                        }
                    }
                }
                DateTime dtNow = DateTime.Now;
                int intWeek = Convert.ToInt16(dtNow.DayOfWeek);
                DateTime PostStart = Convert.ToDateTime(dtNow.ToString("yyyy-MM-dd") + " " + setData[0].PostStart);
                DateTime PostEnd = Convert.ToDateTime(dtNow.ToString("yyyy-MM-dd") + " " + setData[0].PostEnd);
                if (blIsPost && dtNow >= setData[0].StartDate && dtNow <= setData[0].EndDate && setData[0].PostWeek.IndexOf(intWeek.ToString()) >= 0 && dtNow > PostStart && dtNow < PostEnd)
                {
                    board[0].IsPost = true;
                }
                if (setData[0].PostActive <= 0)
                {
                    board[0].PostActive = setData[0].PostActive;
                }
            }
            return board[0];
        }
        //版块页面加载主题列表
        public void ListTopic(int BoardId, Repeater rpList, Literal ltNo, Label lblNav, Page page, bool isPost, bool isMaster, Panel plCmd, HyperLink hLnk)
        {
            if (isPost)
            {
                plCmd.Visible = true;
            }
            hLnk.CssClass = "cur";
            int pageCur = (!string.IsNullOrEmpty(page.Request.QueryString["page"])) ? Convert.ToInt32(page.Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            WebForumBbs webBbs = new WebForumBbs();
            string ActiveName = (isMaster) ? ">=0" : ">0";
            DataForumBbs[] data = webBbs.GetDatas(ActiveName, BoardId, 0, "", "Id,BoardId,TopicId,Title,Active,AddUser,ReadNum,ReplyNum,ReplyTime,ReplyUser", pageCur, pageSize, "BbsTop DESC, ReplyTime DESC, UpTime DESC, AddTime DESC", "total");//加载主题最后发帖
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    if (data[i].Active <= 0)
                    {
                        data[i].rowClass = " class='save' title='待审核'";
                    }
                }
                rpList.DataSource = data;
                rpList.DataBind();
                ltNo.Visible = false;
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = page.Request.Url.ToString();
                lblNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
            }
        }
        //主题帖加载帖子列表
        public string ListPost(int BoardId, int TopicId, Repeater rpList, Label lblNav, Page page, bool isPost, bool isMaster, Panel plCmd, HyperLink hLnk, Literal readNum, Literal replyNum)
        {
            if (isPost)
            {
                plCmd.Visible = true;
            }
            hLnk.NavigateUrl = string.Format(hLnk.NavigateUrl, BoardId, TopicId);
            hLnk.Text = hLnk.Text.Replace("返回", "");
            hLnk.CssClass = "cur";
            hLnk.Visible = true;
            if (TopicId <= 0)
            {
                page.Response.Redirect("forum.aspx?bid=" + BoardId.ToString());
                return "";
            }
            int pageCur = (!string.IsNullOrEmpty(page.Request.QueryString["page"])) ? Convert.ToInt32(page.Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            WebForumBbs webBbs = new WebForumBbs();
            DataForumBbs[] data = webBbs.GetDatas(">-400", BoardId, TopicId, "", "Id,BoardId,TopicId,Title,Body,Active,AddTime,AddUser,UserId,UpTime,UpUser,ReadNum,ReplyNum,ReplyTime,ReplyUser", pageCur, pageSize, "TopicId ASC, AddTime ASC", "total");//加载帖子列表
            if (data == null || data[0].Active < 0)
            {
                page.Response.Redirect("forum.aspx?bid=" + BoardId.ToString());
            }
            webBbs.UpdateReadNum(TopicId);//增加阅读数
            readNum.Text = (data[0].ReadNum + 1).ToString();
            replyNum.Text = data[0].ReplyNum.ToString();
            WebUser webUser = new WebUser();
            for (int i = 0; i < data.Count(); i++)
            {
                data[i].num = (pageCur - 1) * pageSize + i + 1;
                if (data[i].Active == 0)
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
                string strBtnEdit = "";
                //判断版主
                if (isMaster)
                {
                    if (isPost)
                    {
                        strBtnEdit += string.Format("<a href='?bid={0}&id={1}'>编辑</a>", data[i].BoardId, data[i].Id, TopicId);
                    }
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
                }
                else
                {
                    if (data[i].Active < 0)
                    {
                        data[i].Body = "===帖子已被锁定，内容不可见===";
                        data[i].Title = "";
                    }
                    else if (data[i].Active == 0)
                    {
                        data[i].Body = "===帖子待审核，内容不可见===";
                        data[i].Title = "";
                    }
                    else if (data[i].UserId == myUser.Id)
                    {
                        if (isPost)
                        {
                            strBtnEdit += string.Format("<a href='?bid={0}&id={1}'>编辑</a>", data[i].BoardId, data[i].Id, TopicId);
                        }
                        strBtnEdit += string.Format(" <a href='?bid={0}&id={1}&ac=del' class='cmd'><b>删除</b></a>", data[i].BoardId, data[i].Id, TopicId);
                    }
                }
                data[i].BtnEdit = strBtnEdit;
            }
            rpList.DataSource = data;
            rpList.DataBind();
            int pageCount = data[0].total;
            int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
            string lnk = page.Request.Url.ToString();
            lblNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
            return data[0].Title;
        }
         //版主处理帖子状态
        public void UpdateActive(int Id, string strAc, Page page, int BoardId, bool isMaster, Literal ltOut)
        {
            string strBack = PublicMod.GetBackUrl("?bid=" + BoardId.ToString());
            if (Id <= 0)
            {
                page.Response.Redirect(strBack);
                return;
            }
            WebForumBbs webBbs = new WebForumBbs();
            DataForumBbs[] data = webBbs.GetData(Id, "Active,UserId");
            if (data == null || data[0].Active <= -400)
            {
                page.Response.Redirect(strBack);
                return;
            }
            string strActive = "";
            int Active = 0;
            if (isMaster)
            {//判断版主
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
            }
            else if (data[0].UserId == myUser.Id && Request.QueryString["ac"] == "del")
            {//判断是否为发帖人
                strActive = "删除";
                Active = -400;//用户自行删除
            }
            if (strActive == "")
            {
                Response.Redirect(strBack);
                return;
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
        public string LoadEdit(int BoardId, int Id, int ReplyId, bool isMaster, int UserId, HyperLink lnkTopic2, HyperLink lnkEdit2, HiddenField hfTopicId2, HiddenField hfId2, Literal ltTitle2, TextBox txtTitle2, TextBox txtBody2, Button btnEdit2)
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
                if (data != null && data[0].Active >= 0 && data[0].BoardId == BoardId && (data[0].UserId == UserId || isMaster))
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
            ltInfo.Text = SubEdit(boardData.IsPost, hfBack.Value, this, myUser.Id, myUser.TrueName, Convert.ToInt32(hfBoardId.Value), Convert.ToInt32(hfTopicId.Value), Convert.ToInt32(hfId.Value), txtTitle.Text, txtBody.Text, boardData.PostActive, ltTitle.Text);
        }
        //处理编辑
        public string SubEdit(bool isPost, string strBack, Page page, int UserId, string TrueName, int BoardId, int TopicId, int Id, string Title, string Body, int Active, string Info)
        {
            string strOut = "";
            if (!isPost)
            {
                //page.Response.Redirect(strBack);
                return "<script>$(function(){ alert('现在的时间不能“发帖”'); window.location.href='" + strBack + "'; });</script>";
            }
            WebForumBbs webBbs = new WebForumBbs();
            DataForumBbs data = new DataForumBbs();
            data.Id = Id;
            data.BoardId = BoardId;
            if (data.BoardId <= 0)
            {
                page.Response.Redirect(strBack);
                return "<script>$(function(){ alert('“版块参数”错误！'); window.history.back(-1); });</script>";
            }
            data.TopicId = TopicId;
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(TrueName, 20);
            data.Title = HelperMain.SqlFilter(Title.Trim(), 50);
            DataForumBbs[] ckData = webBbs.GetDatas(">-400", data.BoardId, -100, data.Title, "Id,AddTime,UserId");//重复检查
            if (ckData != null && ckData[0].Id != data.Id && ckData[0].UserId != UserId && ckData[0].AddTime.AddSeconds(-30) > dtNow)
            {
                page.Response.Redirect(strBack);
                return "<script>$(function(){ alert('重复提交啦！'); });</script>";
            }
            data.Body = HelperMain.SqlFilter(Body.TrimEnd());
            data.Active = Active;
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
                data.UserId = -1000;
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
        //
        //获取未反馈数
        public int GetFeedNum(DataUser uData)
        {
            int intNum = 0;
            WebForumBbs webBbs = new WebForumBbs();
            DataForumBbs[] data = webBbs.GetTodayTopicPost(1, 0, "Today");
            if (data != null)
            {
                intNum = data[0].TodayNum;
            }
            return intNum;
        }
    }
}