<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="hkzx.web.admin.index" %><%--Tony维护--%>
<%@ Register src="../cn/ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="ucHeader.ascx" tagname="ucHeader" tagprefix="uc1" %>
<%@ Register src="ucFooter.ascx" tagname="ucFooter" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" Client="admin" />
    <title>虹口政协履职通 - 管理系统</title>
    <script>
        $(function () {
            $('.block').each(function () {
                var $no = $(this).find('table>tbody>tr>td.no');
                if ($no.text()) {
                    $no.attr('colspan', $(this).find('table>thead>tr>th').length);
                    //$(this).find('table>thead').hide();
                }
            })
        });
    </script>
</head>
<body>
<form id="form1" runat="server">
    <uc1:ucHeader ID="header1" runat="server" />
    <div class="content">
        <asp:Panel ID="plHome" runat="server" Visible="false" CssClass="home">
            <div class="place"><i></i>欢迎您登入政协委员履职管理系统</div>
            <div class="block">
                <div>
                    <strong>信息发布
                        <a href="notice.aspx">更多&gt;&gt;</a>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="num">类型</th>
                                <th>标题</th>
                                <th class="time">失效时间</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpNoticeList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "OverTime", "{0:yyyy-MM-dd HH:mm:ss}")%></td>
                                        <td align="center"><a href="notice.aspx?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><u>查看</u></a></td>
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
                </div>
            </div>
            <%--<div class="block">
                <div>
                    <strong>政协动态
                        <a href="news.aspx">更多&gt;&gt;</a>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="state">类型</th>
                                <th>标题</th>
                                <th class="time">发布时间</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpNewsList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ShowTime", "{0:yyyy-MM-dd HH:mm:ss}")%></td>
                                        <td align="center"><a href="news.aspx?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><u>查看</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltNewsNo" runat="server">
                                <tr>
                                    <td class="no">暂无政协动态！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row"></div>--%>
            <div class="block">
                <div>
                    <strong>会议/活动通知
                        <a href="perform.aspx">更多&gt;&gt;</a>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>活动主题</th>
                                <th class="time">时间</th>
                                <th>地点</th>
                                <th class="date">状态</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpPerformListNew" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubType")%><br /><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "StartTime", "{0:yyyy年M月d日 HH:mm}")%><br /><%#DataBinder.Eval(Container.DataItem, "EndTime", "{0:yyyy年M月d日 HH:mm}")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "PerformSite")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td><a href="perform.aspx?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><u>查看</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltPerformNoNew" runat="server">
                                <tr>
                                    <td class="no">近期暂无会议/活动通知！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row"></div>
            <div class="block block2">
                <div>
                    <strong>待审核的提案
                        <a href="opinion.aspx">更多&gt;&gt;</a>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="time">类别</th>
                                <th>案由</th>
                                <th>建议主办单位</th>
                                <th>建议会办单位</th>
                                <th class="time">提交时间</th>
                                <th class="state">状态</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpOpinionList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Summary")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "AdviseHostOrg")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "AdviseHelpOrg")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SubTime", "{0:yyyy-MM-dd HH:mm:ss}")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td><a href="opinion.aspx?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><u>查看</u></a>
                                            <%#DataBinder.Eval(Container.DataItem, "other")%>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltOpinionNo" runat="server">
                                <tr>
                                    <td class="no">暂无待审核的提案！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row"></div>
            <div class="block block2">
                <div>
                    <strong>待审核的社情民意
                        <a href="opinion_pop.aspx">更多&gt;&gt;</a>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="time">类别</th>
                                <th>标题</th>
                                <th class="time">提交时间</th>
                                <th class="state">状态</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpPopList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Summary")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SubTime", "{0:yyyy-MM-dd HH:mm:ss}")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td><a href="opinion_pop.aspx?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><u>查看</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltPopNo" runat="server">
                                <tr>
                                    <td class="no">暂无待审核的社情民意！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="block block2">
                <div>
                    <strong>待审核的调研报告
                        <a href="report.aspx">更多&gt;&gt;</a>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>提交部门(单位)</th>
                                <th>标题</th>
                                <th class="date">状态</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpReportList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OrgName")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td><a href="report.aspx?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><u><%#DataBinder.Eval(Container.DataItem, "StateName")%></u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltReportNo" runat="server">
                                <tr>
                                    <td class="no">暂无待审核的调研报告！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row"></div>
        </asp:Panel>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
