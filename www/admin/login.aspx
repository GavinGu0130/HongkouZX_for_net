<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="hkzx.web.admin.login" %><%--Tony维护--%>
<%@ Register src="../cn/ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="ucHeader.ascx" tagname="ucHeader" tagprefix="uc1" %>
<%@ Register src="ucFooter.ascx" tagname="ucFooter" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" Client="admin" />
    <title>虹口政协履职通 - 管理系统</title>
    <style>
        body { min-width:auto;}
    </style>
    <script src="../inc/md5.js"></script>
    <script>
        $(function () {
            var num = 0;
            var coder;
            function go() {
                if (num++ < 5) {
                    clearTimeout(coder);
                    $('#code').attr('src', '../cn/code.aspx?' + ~(new Date() / 100));
                    coder = setTimeout(function () { go(); }, 60000);//定时器
                } else {
                    $('#code').attr('src', '../image/empty.gif');
                }
            }
            go();
            $('#code').click(function () {
                num = 0;
                go();
                $('#txtCode').val('').focus();
            });
            $('#txtUser').focus();
            $('form').submit(function () {
                try {
                    if (checkEmpty('#txtUser') || checkEmpty('#txtPwd') || checkEmpty('#txtCode')) {
                        return false;
                    }
                    $('#txtPwd').val(CryptoJS.MD5(encodeURIComponent($('#txtPwd').val())));
                    return true;
                } catch (err) {
                    //alert("验证出错，请稍后重试！");
                    return false;
                }
            });
        });
    </script>
</head>
<body>
<form id="form1" runat="server">
    <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    <asp:HiddenField ID="hfBack" runat="server" Value="./" />
    <uc1:ucHeader ID="header1" runat="server" Cur="login" />
    <asp:Panel ID="plLogin" runat="server" CssClass="login">
        <dl>
            <dt>管理登录</dt>
            <dd class="txt"><b>用 户 名：</b><asp:TextBox ID="txtUser" runat="server" MaxLength="20" ToolTip="用户名" AutoPostBack="false"></asp:TextBox></dd>
            <dd class="txt"><b>密　　码：</b><asp:TextBox ID="txtPwd" runat="server" MaxLength="20" TextMode="Password" ToolTip="密码" AutoPostBack="false"></asp:TextBox></dd>
            <dd class="code"><b>验 证 码：</b><asp:TextBox ID="txtCode" runat="server" MaxLength="5" ToolTip="验证码" AutoPostBack="false"></asp:TextBox><img id="code" src="../cn/code.aspx" title="请点击" /></dd>
            <dd class="btn"><asp:Button ID="btnLogin" runat="server" Text="登录" OnClick="btnLogin_Click" /></dd>
        </dl>
    </asp:Panel>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
