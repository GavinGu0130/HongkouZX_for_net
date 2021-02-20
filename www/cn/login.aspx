<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="hkzx.web.cn.login" %><%--Tony维护--%>
<%@ Register src="ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="ucHeader.ascx" tagname="ucHeader" tagprefix="uc1" %>
<%@ Register src="ucFooter.ascx" tagname="ucFooter" tagprefix="uc1" %>
<%@ Register src="../m/ucHeader.ascx" tagname="ucHeaderM" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" />
    <title>虹口政协履职通</title>
    <style>
        body { min-width:auto;}
    </style>
    <script src="../inc/md5.js"></script>
</head>
<body>
<form id="form1" runat="server">
    <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    <asp:HiddenField ID="hfBack" runat="server" />
    <uc1:ucHeader ID="header1" runat="server" Visible="false" Cur="login" />
    <uc1:ucHeaderM ID="header2" runat="server" Visible="false" Cur="login" />
    <asp:Panel ID="plLogin" runat="server" Visible="false" CssClass="login">
<script>
    $(function () {
        if ($('#txtCode').attr('title') == '验证码') {
            var num = 0;
            var coder;
            function go() {
                if (num++ < 5) {
                    clearTimeout(coder);
                    $('#code').attr('src', 'code.aspx?' + ~(new Date() / 100));
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
        }
        $('#txtUser').focus();
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtUser') || checkEmpty('#txtPwd')) {
                    return false;
                }
                if ($('#txtCode').attr('title') == '验证码' && checkEmpty('#txtCode')) {
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
        <dl>
            <dt><asp:Literal ID="ltTitle" runat="server">系统登录</asp:Literal></dt>
            <dd class="txt"><b>姓　　名：</b><asp:TextBox ID="txtUser" runat="server" MaxLength="20" ToolTip="姓名" AutoPostBack="false"></asp:TextBox></dd>
            <dd class="txt"><b>密　　码：</b><asp:TextBox ID="txtPwd" runat="server" MaxLength="20" TextMode="Password" ToolTip="密码" AutoPostBack="false"></asp:TextBox></dd>
            <dd class="code"><b>验 证 码：</b><asp:TextBox ID="txtCode" runat="server" MaxLength="5" ToolTip="验证码" AutoPostBack="false" TextMode="Number"></asp:TextBox><img id="code" src="code.aspx" title="请点击" /></dd>
            <dd class="btn">
                <asp:Button ID="btnLogin" runat="server" Text="登录" OnClick="btnLogin_Click" />
                <asp:HyperLink ID="lnkWx" runat="server" Visible="false" NavigateUrl="../m/wx.aspx?ac=login"><i class="wx"></i>微信登录</asp:HyperLink>
            </dd>
        </dl>
    </asp:Panel>

    <asp:Panel ID="plPwd" runat="server" Visible="false" CssClass="login">
<script>
    $(function () {
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtOld')) {
                    return false;
                }
                if (checkEmpty('#txtNew', 6, 20)) {
                    return false;
                }
                if ($('#txtNew').val() != $('#txtNew2').val()) {
                    alert('两次输入的[密码]不一致');
                    $('#txtNew2').focus();
                    return false;
                }
                $('#txtOld').val(CryptoJS.MD5(encodeURIComponent($('#txtOld').val())));
                return true;
            } catch (err) {
                //alert("验证出错，请稍后重试！");
                return false;
            }
        });
    });
</script>
        <dl>
            <dt>修改密码</dt>
            <dd class="txt"><b>用 户 名：</b><asp:TextBox ID="txtName" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></dd>
            <dd class="txt"><b>旧 密 码：</b><asp:TextBox ID="txtOld" runat="server" MaxLength="20" TextMode="Password" ToolTip="旧密码" AutoPostBack="false"></asp:TextBox></dd>
            <dd class="txt"><b>新 密 码：</b><asp:TextBox ID="txtNew" runat="server" MaxLength="20" TextMode="Password" ToolTip="新密码" AutoPostBack="false"></asp:TextBox></dd>
            <dd class="txt"><b>再输一遍：</b><asp:TextBox ID="txtNew2" runat="server" MaxLength="20" TextMode="Password" ToolTip="再输一遍" AutoPostBack="false"></asp:TextBox></dd>
            <dd class="btn"><asp:Button ID="btnPwd" runat="server" Text="修改" OnClick="btnPwd_Click" /><input type="button" value="返回" onclick="history.back(-1);" /></dd>
        </dl>
    </asp:Panel>
    <uc1:ucFooter ID="footer1" runat="server" Visible="false" />
</form>
</body>
</html>
