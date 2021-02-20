<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucHeader.ascx.cs" Inherits="hkzx.web.m.ucHeader" %><%--Tony维护--%>
<script>
    $(function () {
        <% if (string.IsNullOrEmpty(UserName) && Cur != "login") {%>
        showDialog('', '../cn/login.aspx', '', 320, 280, 'no');
        <% } %>
        if (document.referrer != '') {// && document.referrer.indexOf(window.location.host) >= 0
            if (window.location.href.indexOf('/m/') + 3 == window.location.href.length) {
                $('#header>a.back').hide();
            } else {
                $('#header>a.back').click(function () {
                    window.history.back(-1);
                    return false;
                });
            }
        } else {
            $('#header>a.back').hide();
        }
        $('#header>dl.more>dt').click(function () {
            var $obj = $(this).parent().find('dd');
            $obj.show();
            setTimeout(function () {
                $obj.hide();
            }, 5000);
        });
        $('.header dl.more>dd>a.wx').click(function () {
            if ($(this).attr('title') == '绑定微信' && !confirm('您确定要“' + $(this).attr('title') + '”吗？')) {
                $(this).parent().hide();
                return false;
            }
        });
    });
</script>
<div class="header">
    <div id="header">
        <asp:HyperLink ID="lnkBack" runat="server" CssClass="back">&lt;</asp:HyperLink>
        <asp:Label ID="lblTitle" runat="server"><img src="../inc/m/title.png" /></asp:Label>
        <asp:PlaceHolder ID="plUser" runat="server" Visible="false">
            <dl class="more">
                <dt><i></i></dt>
                <dd>
                    <b><asp:Literal ID="ltUser" runat="server"></asp:Literal></b>
                    <asp:HyperLink ID="lnkWxBind" runat="server" Visible="false" NavigateUrl="wx.aspx?ac=bind" CssClass="wx" ToolTip="绑定微信"><i></i>绑定微信</asp:HyperLink>
                    <asp:HyperLink ID="lnkUnBind" runat="server" Visible="false" NavigateUrl="wx.aspx?ac=unbind" CssClass="wx" ToolTip="微信信息"><i></i>微信信息</asp:HyperLink>
                    <a href="./"><i></i>首页</a>
                    <a href="../cn/login.aspx?ac=pwd"><i></i>修改密码</a>
                    <a href="../cn/login.aspx?ac=logout"><i></i><b>退出</b></a>
                </dd>
            </dl>
        </asp:PlaceHolder>
    </div>
</div>
