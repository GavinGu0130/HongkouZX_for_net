<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="notice.aspx.cs" Inherits="hkzx.web.cn.notice" %><%--Tony维护--%>
<%@ Register src="ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="ucHeader.ascx" tagname="ucHeader" tagprefix="uc1" %>
<%@ Register src="ucFooter.ascx" tagname="ucFooter" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" />
    <title>虹口政协履职通</title>
</head>
<body>
<form id="form1" runat="server">
    <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    <asp:HiddenField ID="hfBack" runat="server" Value="./" />
    <uc1:ucHeader ID="header1" runat="server" />
    <div class="content">
        <div class="main">
            <asp:PlaceHolder ID="plList" runat="server" Visible="false">
<script>
    $(function () {
        if ($('.list>table>tbody>tr>td.no').text()) {
            $('.list>table>tbody>tr>td.no').attr('colspan', $('.list>table>thead>tr>th').length);
            $('.list>table>thead').hide();
        }
    });
</script>
                <div class="frm list hover">
                    <strong>信息发布
                        <span>符合条件的数据有：<b><asp:Literal ID="ltNoticeTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="state">类型</th>
                                <th>标题</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpNoticeList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><b><%#DataBinder.Eval(Container.DataItem, "Title")%></b>
                                            <p><%#DataBinder.Eval(Container.DataItem, "Body")%></p>
                                        </td>
                                        <td align="center"><a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltNoticeNo" runat="server">
                                <tr>
                                    <td class="no">暂无信息发布！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblNoticeNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plView" runat="server" Visible="false">
<script>
    $(function () {
        if ($('div.list>dl>dd').text()) {
            $('div.list>dl>dd').html(ubb2html($('div.list>dl>dd').text()));
        }
    });
</script>
                <asp:Repeater ID="rpView" runat="server">
                    <ItemTemplate>
                        <div class="frm list">
                            <dl>
                                <dt>
                                    <strong>
                                        [<%#DataBinder.Eval(Container.DataItem, "SubType")%>]
                                        <%#DataBinder.Eval(Container.DataItem, "Title")%>
                                    </strong>
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
            </asp:PlaceHolder>
        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
