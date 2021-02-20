<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="datas.aspx.cs" Inherits="hkzx.web.m.datas" %><%--Tony维护--%>
<%@ Register src="../cn/ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="ucHeader.ascx" tagname="ucHeader" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" Client="wap" />
    <title>虹口政协履职通 - 资料文档</title>
    <script>
        $(function () {
            var url = window.location.href;
            if (url.indexOf('tid=') < 0) {
                $('#nav>div>a:first').addClass('cur');
                url += $('#nav>div>a:first').attr('href');
            } else {
                var tid = url.substring(url.indexOf('tid='));
                if (tid.indexOf('&') > 0) {
                    tid = tid.substring(0, tid.indexOf('&'));
                }
                $('#nav>div>a[href*="' + tid + '"]').addClass('cur');
            }
        });
    </script>
</head>
<body>
<form id="form1" runat="server">
    <uc1:ucHeader ID="header1" runat="server" Title="资料文档" />
    <div class="content main">
        <div id="nav" class="btn">
            <div>
                <asp:Repeater ID="rpType" runat="server">
                    <ItemTemplate>
                        <a href="?tid=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><%#DataBinder.Eval(Container.DataItem, "TypeName")%></a>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <asp:PlaceHolder ID="plList" runat="server" Visible="false">
            <div class="list">
                <ul>
                    <asp:Repeater ID="rpList" runat="server">
                        <ItemTemplate>
                            <li>
                                <a href="?tid=<%#DataBinder.Eval(Container.DataItem, "TypeId")%>&id=<%#DataBinder.Eval(Container.DataItem, "Id")%>">
                                    <span><%#DataBinder.Eval(Container.DataItem, "Title")%></span>
                                </a><%#DataBinder.Eval(Container.DataItem, "Files")%>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltNo" runat="server">
                        <li class="no">暂时没有查询到资料文档！</li>
                    </asp:Literal>
                </ul>
                <asp:Label ID="lblNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:Repeater ID="rpView" runat="server">
            <ItemTemplate>
                <div class="view">
                    <strong>[<%#DataBinder.Eval(Container.DataItem, "TypeName")%>] <%#DataBinder.Eval(Container.DataItem, "Title")%></strong>
                    <span>
                        发布日期：<%#DataBinder.Eval(Container.DataItem, "UpTime", "{0:yyyy-MM-dd}")%>
                        浏览次数：<%#DataBinder.Eval(Container.DataItem, "ReadNum")%>
                    </span>
                    <div>
                        <%#DataBinder.Eval(Container.DataItem, "Files")%>
                        <p><%#DataBinder.Eval(Container.DataItem, "Body")%></p>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</form>
</body>
</html>
