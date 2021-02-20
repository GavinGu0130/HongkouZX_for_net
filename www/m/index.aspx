<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="hkzx.web.m.index" %><%--Tony维护--%>
<%@ Register src="../cn/ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="ucHeader.ascx" tagname="ucHeader" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" Client="wap" />
    <title>虹口政协履职通</title>
    <script>
        function mark(obj, str) {
            if (str) {
                if ($(obj + '>span').length > 0) {
                    $(obj + '>span').html(str);
                } else {
                    $(obj).append('<span>' + str + '</span>');
                }
            }
        }
        $(function () {
            <%--$('#lnkUserScore').click(function () {
                showDialog('我的积分明细', $(this).attr('href'), '', 320, 480, 'yes');
                return false;
            });--%>
            var url = 'index.aspx?ac=mark';
            $.ajax({
                type: 'GET', dataType: 'HTML', url: url + '&' + ~(-new Date() / 100),
                success: function (data) {
                    mark('.home>.photo', $(data).find('#upPerformFeed').html())
                    mark('a.notice', $(data).find('#lblNotice').html())
                    mark('a.perform', $(data).find('#lblPerform').html())
                    mark('a.opinion', $(data).find('#lblOpinion').html())
                    mark('a.datas', $(data).find('#lblDatas').html())
                    mark('a.forum', $(data).find('#lblForum').html())
                    mark('a.survey', $(data).find('#lblSurvey').html())
                }
            });
        });
    </script>
    <style>
        form { background:url(../inc/m/bg_top.jpg) center top no-repeat; width:100%; min-height:320px;}
.home>.ico>a.notice { left:70px; top:0;}
.home>.ico>a.perform { right:50px; top:0;}
.home>.ico>a.opinion { left:35px; top:205px;}
.home>.ico>a.pop { left:0; top:100px;}
.home>.ico>a.report { right:35px; top:205px;}
.home>.ico>a.datas { right:0; top:100px;}
.home>.ico>a.forum { right:133px; top:250px;}
.home>.ico>a.survey { display:none;}
    </style>
</head>
<body>
<form id="form1" runat="server">
    <uc1:ucHeader ID="header1" runat="server" Cur="home" />
    <div class="content home">
        <asp:PlaceHolder ID="plHome" runat="server" Visible="false">
            <div class="photo">
                <i><asp:Literal ID="ltUserPhoto" runat="server"></asp:Literal></i>
                <b><asp:Literal ID="ltUseName" runat="server"></asp:Literal></b>
                <u>积分：<asp:HyperLink ID="lnkUserScore" runat="server" NavigateUrl="../cn/dialog.aspx?ac=score&view=my&Titler=我的积分明细&UserId=">0</asp:HyperLink></u>
                <span></span>
            </div>
            <div class="ico">
                <div>
                    <i></i>
                    <u></u>
                </div>
                <a href="notice.aspx" class="notice"><i></i>信息发布</a>
                <a href="perform.aspx" class="perform"><i></i>会议/活动通知</a>
                <a href="opinion.aspx" class="opinion"><i></i>提案管理</a>
                <a href="opinion_pop.aspx?ac=query" class="pop"><i></i>社情民意</a>
                <a href="report.aspx?ac=query" class="report"><i></i>调研报告</a>
                <a href="datas.aspx" class="datas"><i></i>资料文档</a>
                <a href="forum.aspx" class="forum"><i></i>委员论坛</a>
                <a href="survey.aspx" class="survey"><i></i>问卷调查</a>
                <a href="user.aspx" class="user"><i></i>委员信息</a>
            </div>
            <%--<dl>
                <dd><a href="news.aspx?ac=news"><i class="news"></i>政协要闻<asp:Label ID="lblNews" runat="server" Visible="false" Text="99+"></asp:Label></a></dd>
                <dd><a href="news.aspx?ac=video"><i class="video"></i>视频新闻<asp:Label ID="lblVideo" runat="server" Visible="false" Text="99+"></asp:Label></a></dd>
                <dd><a href="user.aspx?ac=qrcode"><i class="qrcode"></i>签到二维码</a></dd>
            </dl>--%>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plMark" runat="server" Visible="false">
            <div id="upPerformFeed"><asp:Literal ID="ltUpPerformFeed" runat="server"></asp:Literal></div>
            <asp:Label ID="lblNotice" runat="server" Visible="false" Text="99+"></asp:Label>
            <asp:Label ID="lblPerform" runat="server" Visible="false" Text="99+"></asp:Label>
            <asp:Label ID="lblOpinion" runat="server" Visible="false" Text="99+"></asp:Label>
            <asp:Label ID="lblDatas" runat="server" Visible="false" Text="99+"></asp:Label>
            <asp:Label ID="lblForum" runat="server" Visible="false" Text="99+"></asp:Label>
            <asp:Label ID="lblSurvey" runat="server" Visible="false" Text="99+"></asp:Label>
        </asp:PlaceHolder>
    </div>
</form>
</body>
</html>
