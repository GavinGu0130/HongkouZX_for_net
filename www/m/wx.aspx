<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="wx.aspx.cs" Inherits="hkzx.web.m.wx" %><%--Tony维护--%>
<%@ Register src="../cn/ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="ucHeader.ascx" tagname="ucHeader" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" Client="wap" />
    <title>虹口政协履职通</title>
</head>
<body>
<form id="form1" runat="server">
    <uc1:ucHeader ID="header1" runat="server" Title="微信信息" Cur="login" />
    <div class="content wx">
        <asp:Panel ID="plSubscribe" runat="server" Visible="false">
            <br /><br />
            <span>请“长按”并识别二维码，关注本微信号！</span>
            <img src="../image/qrcode2.jpg" width="240px" /><br/>
            <a href="https://mp.weixin.qq.com/mp/profile_ext?action=home&__biz=MzU4MDgzNTE4Mw==&scene=124#wechat_redirect"><b>虹口政协履职通</b></a><br/>
            <%--微信号：shbstravel--%>
        </asp:Panel>
        <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    </div>
</form>
</body>
</html>
