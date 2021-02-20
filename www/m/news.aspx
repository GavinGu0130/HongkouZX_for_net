<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="news.aspx.cs" Inherits="hkzx.web.m.news" %><%--Tony维护--%>
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
    <uc1:ucHeader ID="header1" runat="server" Title="政协动态" />
    <div class="content main">
        <asp:PlaceHolder ID="plNews" runat="server" Visible="false">
            <div class="list news">
                <ul>
                    <asp:Repeater ID="rpNewsList" runat="server">
                        <ItemTemplate>
                            <li>
                                <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>">
                                    <div class="pic"><%#DataBinder.Eval(Container.DataItem, "PicUrl")%></div>
                                    <div class="body">
                                        <b><%#DataBinder.Eval(Container.DataItem, "Title")%></b>
                                        <p><%#DataBinder.Eval(Container.DataItem, "Body")%></p>
                                    </div>
                                </a>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltNewsNo" runat="server">
                        <li class="no">暂无政协要闻！</li>
                    </asp:Literal>
                </ul>
                <asp:Label ID="lblNewsNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plVideo" runat="server" Visible="false">
            <div class="list video">
                <ul>
                    <asp:Repeater ID="rpVideoList" runat="server">
                        <ItemTemplate>
                            <li>
                                <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>">
                                    <%#DataBinder.Eval(Container.DataItem, "PicUrl")%>
                                    <span><%#DataBinder.Eval(Container.DataItem, "Title")%></span>
                                </a>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
                <asp:Label ID="lblVideoNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

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
    </div>
</form>
</body>
</html>
