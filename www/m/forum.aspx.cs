using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mod.main;
using hkzx.db;
using hkzx.user;

namespace hkzx.web.m
{
    public partial class forum : System.Web.UI.Page
    {
        private DataUser myUser = null;
        private DataForumBoard boardData = null;
        private cn.forum page = new cn.forum();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.TrueName))
            {
                //Response.Redirect("../cn/login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            header1.UserName = myUser.TrueName;
            if (!string.IsNullOrEmpty(Request.QueryString["bid"]))
            {
                int BoardId = Convert.ToInt32(Request.QueryString["bid"]);
                boardData = page.GetBoard(BoardId, myUser.TrueName, this);
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
                    page.UpdateActive(Convert.ToInt32(Request.QueryString["id"]), Request.QueryString["ac"], this, BoardId, boardData.IsMaster, ltInfo);
                }
                else if (!string.IsNullOrEmpty(Request.QueryString["tid"]))
                {//主题帖
                    plPost.Visible = true;
                    ltPostTitle.Text = page.ListPost(BoardId, Convert.ToInt32(Request.QueryString["tid"]), rpPostList, lblPostNav, this, boardData.IsPost, boardData.IsMaster, plPostCmd, lnkTopic, ltReadNum, ltReplyNum);
                    Header.Title += " - " + ltPostTitle.Text;
                }
                else if (!string.IsNullOrEmpty(Request.QueryString["rid"]))
                {//回复帖子
                    if (!IsPostBack)
                    {
                        hfBack.Value = PublicMod.GetBackUrl("?bid=" + BoardId.ToString());
                        plEdit.Visible = true;
                        Header.Title += " - " + page.LoadEdit(BoardId, 0, Convert.ToInt32(Request.QueryString["rid"]), boardData.IsMaster, myUser.Id, lnkTopic, lnkEdit, hfTopicId, hfId, ltTitle, txtTitle, txtBody, btnEdit);
                    }
                }
                else if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {//编辑帖子
                    if (!IsPostBack)
                    {
                        hfBack.Value = PublicMod.GetBackUrl();
                        plEdit.Visible = true;
                        Header.Title += " - " + page.LoadEdit(BoardId, Convert.ToInt32(Request.QueryString["id"]), 0, boardData.IsMaster, myUser.Id, lnkTopic, lnkEdit, hfTopicId, hfId, ltTitle, txtTitle, txtBody, btnEdit);
                    }
                }
                else
                {//主题列表
                    ltBoardRule.Text = boardData.BoardRule;
                    ltBoardMaster.Text = boardData.BoardMaster;
                    plTopic.Visible = true;
                    page.ListTopic(BoardId, rpTopicList, ltTopicNo, lblTopicNav, this, boardData.IsPost, boardData.IsMaster, plTopicCmd, lnkBoard);
                }
            }
            else
            {//版块
                plBoard.Visible = true;
                page.ListBoard(rpBoardList, ltBoardNo);
            }
        }
        //编辑数据
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            ltInfo.Text = page.SubEdit(boardData.IsPost, hfBack.Value, this, myUser.Id, myUser.TrueName, Convert.ToInt32(hfBoardId.Value), Convert.ToInt32(hfTopicId.Value), Convert.ToInt32(hfId.Value), txtTitle.Text, txtBody.Text, boardData.PostActive, ltTitle.Text);
        }
        //
    }
}