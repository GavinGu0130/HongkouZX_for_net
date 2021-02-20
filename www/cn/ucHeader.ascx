<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucHeader.ascx.cs" Inherits="hkzx.web.cn.ucHeader" %><%--Tony维护--%>
<div class="header">
<script>
    $(function () {
        <% if (string.IsNullOrEmpty(UserName) && Cur != "login") { %>
        showDialog('', 'login.aspx', '', 400, 280, 'no');
        <% } %>
        var txt = $('.header>.hello').text();
        if (txt && txt.indexOf(' ，') >= 0) {
            $('.header>.hello').hide();
        }
        window.setInterval(function () {
            var now = new Date();//获取系统当前时间
            var hh = now.getHours().toString().replace(/\b\d{1}\b/g, '0$&');
            var mm = now.getMinutes().toString().replace(/\b\d{1}\b/g, '0$&');
            var ss = now.getSeconds().toString().replace(/\b\d{1}\b/g, '0$&');
            $('#time').text(hh + ':' + mm + ':' + ss);
        }, 1000);
    });
</script>
    <div class="logo"><i></i><img src="../image/txt.png" /><span>履职通</span></div>
    <div class="hello">
        <asp:Literal ID="ltUser" runat="server"></asp:Literal>，<b><asp:Literal ID="ltHello" runat="server"></asp:Literal></b> 登录时间：<asp:Literal ID="ltLoginTime" runat="server"></asp:Literal>
    </div>
    <div class="user">
        现在时间：<span id="time"></span>
        <asp:PlaceHolder ID="plUser" runat="server" Visible="false">
            <a href="./">首页</a>
            <a href="user.aspx">我的信息</a>
            <a href="login.aspx?ac=pwd">修改密码</a>
            <a href="login.aspx?ac=logout">退出</a>
        </asp:PlaceHolder>
    </div>
</div>
<asp:Panel ID="plMenu" runat="server" Visible="false" CssClass="menu">
    <div>
        <span>政协动态</span>
        <ul>
            <%--<li><a href="news.aspx?ac=news">政协要闻</a></li>
            <li><a href="news.aspx?ac=video">视频新闻</a></li>--%>
            <li><a href="notice.aspx">信息发布</a></li>
            <li><a href="perform.aspx">会议/活动通知</a></li>
        </ul>
    </div>
    <div>
        <span>网上提交</span>
        <ul>
            <li><a href="opinion.aspx">提交提案</a></li>
            <li><a href="opinion_pop.aspx">提交社情民意</a></li>
            <li><a href="report.aspx">提交调研报告</a></li>
            <asp:Literal ID="ltPerform" runat="server" Visible="false">
                <li><a href="perform.aspx?ac=sub">申请会议/活动</a></li>
            </asp:Literal>
            <li style="display:none;"><a href="survey.aspx">问卷调查<asp:Literal ID="ltSurvey" runat="server" Visible="false"><b>(99+)</b></asp:Literal></a></li>
        </ul>
    </div>
    <div>
        <span>信息查询</span>
        <ul>
            <li><a href="opinion.aspx?ac=query">检索提案</a></li>
            <li><a href="opinion_pop.aspx?ac=query">检索社情民意</a></li>
            <li><a href="report.aspx?ac=query">检索调研报告</a></li>
            <li><a href="datas.aspx">资料文档</a></li>
        </ul>
    </div>
    <div>
        <span><a href="forum.aspx">委员论坛</a></span>
    </div>
</asp:Panel>
