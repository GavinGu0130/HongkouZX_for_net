<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="news.aspx.cs" Inherits="hkzx.web.cn.news" %><%--Tony维护--%>
<%@ Register src="ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="ucHeader.ascx" tagname="ucHeader" tagprefix="uc1" %>
<%@ Register src="ucFooter.ascx" tagname="ucFooter" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" />
    <title>虹口政协履职通</title>
    <script>
        $(function () {
            if ($('.list>table>tbody>tr>td.no').text()) {
                $('.list>table>tbody>tr>td.no').attr('colspan', $('.list>table>thead>tr>th').size());
                $('.list>table>thead').hide();
            }
        });
    </script>
</head>
<body>
<form id="form1" runat="server">
    <uc1:ucHeader ID="header1" runat="server" />
    <div class="content">
        <div class="main">
            <asp:PlaceHolder ID="plNews" runat="server" Visible="false">
                <div class="frm list hover">
                    <strong>政协要闻
                        <span>符合条件的数据有：<b><asp:Literal ID="ltNewsTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <tbody>
                            <asp:Repeater ID="rpNewsList" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <div class="date"><%#DataBinder.Eval(Container.DataItem, "ShowTimeText")%></div>
                                            <div class="pic"><%#DataBinder.Eval(Container.DataItem, "PicUrl")%></div>
                                            <div class="body">
                                                <b><a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><%#DataBinder.Eval(Container.DataItem, "Title")%></a></b>
                                                <p><%#DataBinder.Eval(Container.DataItem, "Body")%></p>
                                            </div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltNewsNo" runat="server">
                                <tr>
                                    <td class="no">暂无政协要闻！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblNewsNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plVideo" runat="server" Visible="false">
                <div class="frm list">
                    <strong>视频新闻</strong>
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
                    <div class="frm list">
                        <dl>
                            <dt>
                                <strong><%#DataBinder.Eval(Container.DataItem, "Title")%></strong>
                                <span>
                                    发布日期：<%#DataBinder.Eval(Container.DataItem, "ShowTime", "{0:yyyy-MM-dd}")%>
                                    浏览次数：<%#DataBinder.Eval(Container.DataItem, "ReadNum")%>
                                </span>
                            </dt>
                            <dd><%#DataBinder.Eval(Container.DataItem, "Body")%></dd>
                        </dl>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
