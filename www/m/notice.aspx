<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="notice.aspx.cs" Inherits="hkzx.web.m.notice" %><%--Tony维护--%>
<%@ Register src="../cn/ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="ucHeader.ascx" tagname="ucHeader" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" Client="wap" />
    <title>虹口政协履职通 - 信息发布</title>
</head>
<body>
<form id="form1" runat="server">
    <uc1:ucHeader ID="header1" runat="server" Title="信息发布" />
    <div class="content main">
        <asp:PlaceHolder ID="plList" runat="server" Visible="false">
            <div class="list">
                <ul>
                    <asp:Repeater ID="rpList" runat="server">
                        <ItemTemplate>
                            <li<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>">
                                <span>
                                    <b>[<%#DataBinder.Eval(Container.DataItem, "SubType")%>]</b>
                                    <%#DataBinder.Eval(Container.DataItem, "Title")%>
                                </span>
                                <%--<p><%#DataBinder.Eval(Container.DataItem, "Body")%></p>--%>
                            </a>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltNo" runat="server">
                        <li class="no">暂无信息发布！</li>
                    </asp:Literal>
                </ul>
                <asp:Label ID="lblNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plView" runat="server" Visible="false">
<script>
    $(function () {
        if ($('div.view>div').text()) {
            $('div.view>div').html(ubb2html($('div.view>div').text()));
        }
    });
</script>
            <asp:Repeater ID="rpView" runat="server">
                <ItemTemplate>
                    <div class="view">
                        <strong><%#DataBinder.Eval(Container.DataItem, "Title")%></strong>
                        <span>
                            发布日期：<%#DataBinder.Eval(Container.DataItem, "ShowTime", "{0:yyyy-MM-dd}")%>
                            浏览次数：<%#DataBinder.Eval(Container.DataItem, "ReadNum")%>
                        </span>
                        <div><%#DataBinder.Eval(Container.DataItem, "Body")%></div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </asp:PlaceHolder>
    </div>
</form>
</body>
</html>
