<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucHeader.ascx.cs" Inherits="hkzx.web.admin.ucHeader" %>
<div class="header">
<script>
    $(function () {
        <% if (string.IsNullOrEmpty(UserName) && Cur != "login") { %>
        showDialog('', 'login.aspx', '', 320, 280, 'no');
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
    <div class="logo"><i></i><img src="../image/txt.png" /><span>履职通</span><u>管理系统</u></div>
    <div class="hello">
        <asp:Literal ID="ltUser" runat="server"></asp:Literal>，<b><asp:Literal ID="ltHello" runat="server"></asp:Literal></b> 登录时间：<asp:Literal ID="ltLoginTime" runat="server"></asp:Literal>
    </div>
    <div class="user">
        现在时间：<span id="time"></span>
        <asp:PlaceHolder ID="plUser" runat="server" Visible="false">
            <a href="./">首页</a>
            <a href="ads.aspx">我的信息</a>
            <a href="ads.aspx?ac=pwd">修改密码</a>
            <a href="login.aspx?ac=logout">退出</a>
        </asp:PlaceHolder>
    </div>
</div>
<asp:Panel ID="plMenu" runat="server" Visible="false" CssClass="menu">
    <div>
        <span>数据管理</span>
        <ul>
            <asp:Literal ID="ltUsers" runat="server" Visible="false"><li><a href="user.aspx">委员信息</a><br /><a href="user.aspx?ac=check">待审核委员</a></li></asp:Literal>
            <%--<asp:Literal ID="ltNews" runat="server" Visible="false"><li><a href="news.aspx">政协动态</a></li></asp:Literal>
            <asp:Literal ID="ltView" runat="server" Visible="false"><li><a href="interview.aspx">委员走访</a></li></asp:Literal>--%>
            <asp:Literal ID="ltNotice" runat="server" Visible="false"><li><a href="notice.aspx">信息发布</a></li></asp:Literal>
            <asp:Literal ID="ltPerform" runat="server" Visible="false"><li><a href="perform.aspx">会议/活动通知</a></li></asp:Literal>
            <asp:Literal ID="ltOpin" runat="server" Visible="false"><li><a href="opinion.aspx">提案管理</a></li></asp:Literal>
            <asp:Literal ID="ltPop" runat="server" Visible="false"><li><a href="opinion_pop.aspx">社情民意</a></li></asp:Literal>
            <asp:Literal ID="ltReport" runat="server" Visible="false"><li><a href="report.aspx">调研报告</a></li></asp:Literal>
            <asp:Literal ID="ltSurvey" runat="server" Visible="false"><li style="display:none;"><a href="survey.aspx">问卷调查</a></li></asp:Literal>
            <asp:Literal ID="ltDatas" runat="server" Visible="false"><li><a href="datas.aspx">资料文档</a></li></asp:Literal>
            <asp:Literal ID="ltForum" runat="server" Visible="false"><li><a href="forum.aspx">委员论坛</a></li></asp:Literal>
        </ul>
    </div>
    <asp:Panel ID="plCount" runat="server" Visible="false">
        <span>统计分析</span>
        <ul>
            <li><a href="count.aspx">委员/团体</a></li>
            <li><a href="count.aspx?ac=committee">专委会</a></li>
            <li><a href="count.aspx?ac=subsector">界别</a></li>
            <li><a href="count.aspx?ac=street">街道活动组</a></li>
            <li><a href="count.aspx?ac=speak">会议发言</a></li>
            <li><a href="count.aspx?ac=invited">特邀监督员工作</a></li>
            <li><a href="count.aspx?ac=appraise"><b>遴选评优</b></a></li>
        </ul>
    </asp:Panel>
    <asp:Panel ID="plAdmin" runat="server" Visible="false">
        <span>系统设置</span>
        <ul>
            <li><a href="output.aspx">提案交互数据</a></li>
            <li><a href="input.aspx">导入数据</a></li>
            <li><a href="op.aspx">选项管理</a></li>
            <li><a href="score.aspx">积分管理</a></li>
            <li><a href="forum.aspx?ac=meeting">全会时间设置</a></li>
            <li><a href="ads.aspx?ac=manage">后台用户管理</a></li>
        </ul>
    </asp:Panel>
</asp:Panel>
