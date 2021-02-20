<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="hkzx.web.cn.index" %><%--Tony维护--%>
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
    <uc1:ucHeader ID="header1" runat="server" />
    <div class="content">
        <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
        <asp:Panel ID="plHome" runat="server" Visible="false" CssClass="home">
<script>
    $(function () {
        $('#lnkUserScore').click(function () {
            showDialog('我的积分明细', $(this).attr('href'), '', 640, 480, 'yes');
            return false;
        });
    });
</script>
            <div class="place"><i><asp:Literal ID="ltUserPhoto" runat="server"></asp:Literal></i><b><asp:Literal ID="ltTureName" runat="server"></asp:Literal></b><%--欢迎您登入政协委员履职系统！--%>　积分：<asp:HyperLink ID="lnkUserScore" runat="server" NavigateUrl="../cn/dialog.aspx?ac=score&view=my&UserId=">0</asp:HyperLink></div>
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
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpNotice" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td align="center"><a href="notice.aspx?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><u>查看</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="block">
                <div>
                    <strong>会议/活动通知
                        <a href="perform.aspx">更多&gt;&gt;</a>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="state">报名方式</th>
                                <th>会议/活动主题</th>
                                <th class="time">起止时间</th>
                                <th>地点</th>
                                <th class="state">状态</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpPerform" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "IsMust")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubType")%><br /><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td align="center">
                                            <%#DataBinder.Eval(Container.DataItem, "StartTime", "{0:yyyy年M月d日 HH:mm}")%><br />
                                            <%#DataBinder.Eval(Container.DataItem, "EndTime", "{0:yyyy年M月d日 HH:mm}")%>
                                        </td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "PerformSite")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><a href="perform.aspx?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><u>查看</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <p><asp:Literal ID="ltUpPerformFeed" runat="server"></asp:Literal></p>
                </div>
            </div>
            <div class="block block2">
                <div>
                    <strong>我的提案
                        <a href="opinion.aspx?ac=my">更多&gt;&gt;</a>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="state">提案号</th>
                                <th>类别</th>
                                <th>案由</th>
                                <th>主办单位</th>
                                <th>会办单位</th>
                                <th class="state">所处流程</th>
                                <th class="cmd2">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpOpinion" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OpNo")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Summary")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "ExamHostOrg")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "ExamHelpOrg")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%><br /><%#DataBinder.Eval(Container.DataItem, "ApplyState")%></td>
                                        <td><a href="opinion.aspx?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><u>查看</u></a>
                                            <%#DataBinder.Eval(Container.DataItem, "other")%>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="block block2">
                <div>
                    <strong>我的社情民意
                        <a href="opinion_pop.aspx?ac=query">更多&gt;&gt;</a>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>类别</th>
                                <th>标题</th>
                                <th class="state">录用情况</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpOpinionPop" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Summary")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><a href="opinion_pop.aspx?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><u>查看</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="block block2">
                <div>
                    <strong>我的调研报告
                        <a href="report.aspx?ac=query">更多&gt;&gt;</a>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>提交部门(单位)</th>
                                <th>标题</th>
                                <th class="state">状态</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpReport" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OrgName")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><a href="report.aspx?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><u><%#DataBinder.Eval(Container.DataItem, "StateName")%></u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row"></div>
            <%--<div class="block block2">
                <div>
                    <strong>意见征询
                        <a href="survey.aspx">更多&gt;&gt;</a>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="num">类型</th>
                                <th>标题</th>
                                <th class="date">截止时间</th>
                                <th class="cmd">可参与</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpSurvey" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "EndTimeText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SurveyNumText")%></td>
                                        <td><a href="survey.aspx?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><u>选取</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="row"></div>--%>
        </asp:Panel>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
