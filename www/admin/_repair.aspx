<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="_repair.aspx.cs" Inherits="hkzx.web.admin._repair" %><%--Tony维护--%>
<!DOCTYPE html>
<html>
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>修复</title>
</head>
<body>
<form id="form1" runat="server">
    <asp:Button ID="btnUserScore" runat="server" Visible="false" Text="修正用户积分时间" OnClick="btnUserScore_Click" />
    <asp:Label ID="lblInfo" runat="server"></asp:Label>
</form>
</body>
</html>
