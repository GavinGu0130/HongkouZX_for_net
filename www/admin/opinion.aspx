<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="opinion.aspx.cs" Inherits="hkzx.web.admin.opinion" %>

<%--Tony维护--%>
<%@ Register Src="../cn/ucMeta.ascx" TagName="ucMeta" TagPrefix="uc1" %>
<%@ Register Src="ucHeader.ascx" TagName="ucHeader" TagPrefix="uc1" %>
<%@ Register Src="ucFooter.ascx" TagName="ucFooter" TagPrefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" Client="admin" />
    <title>虹口政协履职通 - 管理系统</title>
    <script>
        $(function () {
            if ($('#plNav').text()) {
                var url = window.location.href;
                if (url.indexOf('id=') > 0) {
                    $('#plNav>a#view').show().addClass('cur');
                } else if ($('#btnMerge').val() == '归并') {
                    $('#plNav>a#merge').show().addClass('cur');
                } else if (url.indexOf('ac=feed') > 0) {
                    $('#plNav>a[href*="ac=feed"]').addClass('cur').removeAttr('href');
                } else if (url.indexOf('ac=sub') > 0) {
                    $('#plNav>a[href*="ac=sub"]').addClass('cur').removeAttr('href');
                } else {
                    $('#plNav>a:first').addClass('cur').removeAttr('href');
                }
            }
            if ($('.list>table>tbody>tr>td.no').text()) {
                $('.list>table>tbody>tr>td.no').attr('colspan', $('.list>table>thead>tr>th').length);
                $('.list>table>thead').hide();
            } else {
                $('.list>table>thead>tr>th>input:checkbox').click(function () {
                    $('.list>table>tbody>tr>td>input:checkbox').prop('checked', $(this).prop('checked'));
                });
            }
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
        <asp:HiddenField ID="hfBack" runat="server" Value="./" />
        <asp:HiddenField ID="hfOrg" runat="server" />
        <asp:HiddenField ID="hfActive" runat="server" />
        <uc1:ucHeader ID="header1" runat="server" />
        <div class="content">
            <div class="main">
                <asp:Panel ID="plNav" runat="server" Visible="false" CssClass="btn">
                    <a href="?ac=">检索提案</a>
                    <a href="?ac=feed">征询意见统计</a>
                    <a id="view" class="hide">审核提案</a>
                    <a id="merge" class="hide">归并提案</a>
                </asp:Panel>

                <asp:PlaceHolder ID="plQuery" runat="server" Visible="false">
                    <script>
                        $(function () {
                            $('#btnQSubMan').click(function () {
                                var title = '选取提案人';
                                var obj = 'txtQSubMan';
                                showDialog(title, '../cn/dialog.aspx?ac=subman&type=all&obj=' + obj, '', 640, 400, 'no');
                                return false;
                            });
                            //$('#txtQSubMan').focus(function () {
                            //    $('#btnQSubMan').click();
                            //});
                            var intQuery = 1;
                            $('form').submit(function () {
                                if (intQuery != 0) {
                                    try {
                                        var url = '';
                                        if ($('#ddlQPeriod').val()) {
                                            url += '&Period=' + $('#ddlQPeriod').val();
                                        }
                                        if ($('#ddlQTimes').val()) {
                                            url += '&Times=' + $('#ddlQTimes').val();
                                        }
                                        if ($('#txtQOpNo').val()) {
                                            url += '&OpNo=' + $('#txtQOpNo').val();
                                        }
                                        if ($('#ddlQApplyState').val()) {
                                            url += '&ApplyState=' + $('#ddlQApplyState').val();
                                        }
                                        if ($('#ddlQTimeMark').val()) {
                                            url += '&TimeMark=' + $('#ddlQTimeMark').val();
                                        }
                                        if ($('#ddlQActiveName').val()) {
                                            url += '&ActiveName=' + $('#ddlQActiveName').val();
                                        }
                                        if ($('#ddlQSubType').val()) {
                                            url += '&SubType=' + $('#ddlQSubType').val();
                                        }
                                        if ($('#ddlQIsPoint').val()) {
                                            url += '&IsPoint=' + $('#ddlQIsPoint').val();
                                        }
                                        if ($('#ddlQIsOpen').val()) {
                                            url += '&IsOpen=' + $('#ddlQIsOpen').val();
                                        }
                                        if ($('#ddlQSubsector').val()) {
                                            url += '&Subsector=' + $('#ddlQSubsector').val();
                                        }
                                        if ($('#ddlQCommittee').val()) {
                                            url += '&Committee=' + $('#ddlQCommittee').val();
                                        }
                                        if ($('#ddlQStreetTeam').val()) {
                                            url += '&StreetTeam=' + $('#ddlQStreetTeam').val();
                                        }
                                        if ($('#ddlQParty').val()) {
                                            url += '&Party=' + $('#ddlQParty').val();
                                        }
                                        if ($('#ddlQSubManType').val()) {
                                            url += '&SubManType=' + $('#ddlQSubManType').val();
                                        }
                                        if ($('#txtQSubMan').val()) {
                                            url += '&SubMan=' + $('#txtQSubMan').val();
                                        }
                                        var tmp = getChecked('#cblQIsSubMan1', ',');
                                        if (tmp) {
                                            url += '&IsSubMan1=' + tmp;
                                        }
                                        if ($('#ddlQUserSex').val()) {
                                            url += '&UserSex=' + $('#ddlQUserSex').val();
                                        }
                                        if ($('#ddlQExamOrgType').val()) {
                                            url += '&ExamOrgType=' + $('#ddlQExamOrgType').val();
                                        }
                                        if ($('#ddlQExamHostOrg').val()) {
                                            url += '&ExamHostOrg=' + $('#ddlQExamHostOrg').val();
                                        }
                                        if ($('#ddlQExamHelpOrg').val()) {
                                            url += '&ExamHelpOrg=' + $('#ddlQExamHelpOrg').val();
                                        }
                                        if ($('#ddlQFeedInterview').val()) {
                                            url += '&FeedInterview=' + $('#ddlQFeedInterview').val();
                                        }
                                        if ($('#ddlQFeedAttitude').val()) {
                                            url += '&FeedAttitude=' + $('#ddlQFeedAttitude').val();
                                        }
                                        if ($('#ddlQFeedResult').val()) {
                                            url += '&FeedResult=' + $('#ddlQFeedResult').val();
                                        }
                                        if ($('#ddlQFeedTakeWay').val()) {
                                            url += '&FeedTakeWay=' + $('#ddlQFeedTakeWay').val();
                                        }
                                        if ($('#ddlQFeedPertinence').val()) {
                                            url += '&FeedPertinence=' + $('#ddlQFeedPertinence').val();
                                        }
                                        if ($('#ddlQFeedLeaderReply').val()) {
                                            url += '&FeedLeaderReply=' + $('#ddlQFeedLeaderReply').val();
                                        }
                                        if ($('#ddlQResultInfo').val()) {
                                            url += '&ResultInfo1=' + $('#ddlQResultInfo').val();
                                        }
                                        if ($('#ddlQResultInfo2').val()) {
                                            url += '&ResultInfo2=' + $('#ddlQResultInfo2').val();
                                        }
                                        if ($('#ddlQFeedActive').val()) {
                                            url += '&FeedActive=' + $('#ddlQFeedActive').val();
                                        }
                                        if ($('#txtQSummary').val()) {
                                            url += '&Summary=' + $('#txtQSummary').val();
                                        }
                                        if ($('#txtQBody').val()) {
                                            url += '&Body=' + $('#txtQBody').val();
                                        }
                                        if ($('#ddlQReApply').val()) {
                                            url += '&ReApply=' + $('#ddlQReApply').val();
                                        }
                                        url += '&Order=' + $('#ddlQOrderBy').val() + '&By=' + $('#rblQOrderBy>input:radio:checked').val();
                                        var tmp = getChecked('#cblQFields', ',');
                                        if (tmp != '') {
                                            url += '&Fields=' + tmp;
                                        } else {
                                            alert('请选择[查询信息]项');
                                            $('#cblQFields>input:checkbox:first').focus();
                                            return false;
                                        }
                                        if ($('#txtQUserSubNum').val()) {
                                            url += '&UserSubNum=' + $('#txtQUserSubNum').val();
                                        }
                                        if ($('#txtQUserSubMin').val()) {
                                            url += '&UserSubMin=' + $('#txtQUserSubMin').val();
                                        }
                                        //alert(encodeURI(url)); return false;
                                        if (url != '') {
                                            url = encodeURI(url);
                                        }
                                        var ac = (intQuery < 0) ? 'user' : 'query';
                                        window.location.href = '?ac=' + ac + url;
                                        return false;
                                    } catch (err) {
                                        alert("验证出错，请稍后重试！");
                                        return false;
                                    }
                                }
                            });
                            $('#btnUser').click(function () {
                                intQuery = -1;
                            });
                            $('#btnQuery').click(function () {
                                intQuery = 1;
                            });
                            $('#btnIsFeed, #btnNoFeed').click(function () {
                                if (checkSelect('#ddlQPeriod') || checkSelect('#ddlQTimes')) {
                                    return false;
                                }
                                intQuery = 0;
                            });
                            $('#btnDels, #btnIsReg, #btnNoReg, #btnSub, #btnMerges').click(function () {
                                var num = $('.list>table>tbody>tr>td>input:checkbox:checked').length;
                                if (num <= 0) {
                                    alert('请先选取要操作的数据！');
                                    return false;
                                } else if ($(this).val() == '归并' && num < 2) {
                                    alert('归并操作至少要选取2件或2件以上提案！');
                                    return false;
                                }
                                if (!confirm('您确定要“' + $(this).val() + '”吗?')) {
                                    return false;
                                }
                                intQuery = 0;
                            });
                        });
                    </script>
                    <div class="frm edit">
                        <strong>检索提案</strong>
                        <table>
                            <tbody>
                                <tr>
                                    <td colspan="6" align="center">虹口区政协
                                    <asp:DropDownList ID="ddlQPeriod" runat="server" Width="60" ToolTip="届次">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>届
                                    <asp:DropDownList ID="ddlQTimes" runat="server" Width="60" ToolTip="届次">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>次
                                    会议提案信息
                                    </td>
                                </tr>
                                <tr>
                                    <th>提案序号</th>
                                    <td>
                                        <asp:TextBox ID="txtQOpNo" runat="server"></asp:TextBox></td>
                                    <th>提案性质</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQActiveName" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>办理状态</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQApplyState" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <th>提案类别</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQSubType" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>是否重点提案</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQIsPoint" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                            <asp:ListItem Text="是"></asp:ListItem>
                                            <asp:ListItem Text="否"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>时间标识</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQTimeMark" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                            <asp:ListItem Text="会间"></asp:ListItem>
                                            <asp:ListItem Text="会后"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <th>提案者专委会</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQCommittee" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>提案者界别</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQSubsector" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>街道活动组</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQStreetTeam" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <th>是否同意公开</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQIsOpen" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                            <asp:ListItem Text="是"></asp:ListItem>
                                            <asp:ListItem Text="否"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>政治面貌</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQParty" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>提案人性别</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQUserSex" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                            <asp:ListItem Text="男"></asp:ListItem>
                                            <asp:ListItem Text="女"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <th>提案者性质</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQSubManType" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>提案人</th>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtQSubMan" runat="server" CssClass="readonly"></asp:TextBox>
                                        <a id="btnQSubMan" href="#" class="btn"><u>选取</u></a>
                                    </td>
                                    <td>
                                        <asp:CheckBoxList ID="cblQIsSubMan1" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                            <asp:ListItem Text="只判断第一提案人" Value="1"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <th>单位所属</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQExamOrgType" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                            <asp:ListItem Text="区政府部门"></asp:ListItem>
                                            <asp:ListItem Text="区党群部门"></asp:ListItem>
                                            <asp:ListItem Text="其他"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>主办单位</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQExamHostOrg" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>会办单位</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQExamHelpOrg" runat="server"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <th>走访情况</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQFeedInterview" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>办理(走访)<br />
                                        人员态度</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQFeedAttitude" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>是否同意<br />
                                        办理结果</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQFeedResult" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr style="display: none;">
                                    <th>答复前听取<br />
                                        意见方式</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQFeedTakeWay" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>答复是否<br />
                                        针对提案</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQFeedPertinence" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>(团体)<br />
                                        分管领导答复</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQFeedLeaderReply" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                            <asp:ListItem Text="是"></asp:ListItem>
                                            <asp:ListItem Text="否"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <th>办理结果</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQResultInfo" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>跟踪办理结果</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQResultInfo2" runat="server"></asp:DropDownList>
                                    </td>
                                    <th>是否已征询意见</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQFeedActive" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                            <asp:ListItem Text="是"></asp:ListItem>
                                            <asp:ListItem Text="否"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <th>案由</th>
                                    <td colspan="5">
                                        <asp:TextBox ID="txtQSummary" runat="server" CssClass="long"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <th>提案内容</th>
                                    <td colspan="5">
                                        <asp:TextBox ID="txtQBody" runat="server" CssClass="long"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <th>再办理</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQReApply" runat="server">
                                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                            <asp:ListItem Text="是"></asp:ListItem>
                                            <asp:ListItem Text="否"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>排序字段</th>
                                    <td>
                                        <asp:DropDownList ID="ddlQOrderBy" runat="server"></asp:DropDownList><br />
                                        <asp:RadioButtonList ID="rblQOrderBy" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                            <asp:ListItem Text="升序" Value="ASC" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="降序" Value="DESC"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                    <th>提案数量</th>
                                    <td>
                                        大于等于<asp:TextBox ID="txtQUserSubNum" runat="server" CssClass="num"></asp:TextBox>
                                        小于<asp:TextBox ID="txtQUserSubMin" runat="server" CssClass="num"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>

                                </tr>
                                <tr>
                                    <th>选择查询信息</th>
                                    <td colspan="5">
                                        <asp:CheckBoxList ID="cblQFields" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <div class="cmd">
                            <input id="btnQuery" type="submit" value="查询" />
                            <%--<input id="clean" type="reset" value="清空" />--%>
                            <asp:HyperLink ID="lnkDownXls" runat="server" Target="_blank">下载提案</asp:HyperLink>
                            <input id="btnUser" type="submit" value="按委员查询" />
                        </div>
                    </div>
                    <div class="cmd cmd2">
                        <asp:Button ID="btnDels" runat="server" Visible="false" Text="删除" OnClick="btnDels_Click" />
                        <%--<asp:Button ID="btnIsReg" runat="server" Text="立案" OnClick="btnIsReg_Click" />
                        <asp:Button ID="btnNoReg" runat="server" Text="不立案" OnClick="btnNoReg_Click" />
                        <asp:Button ID="btnSub" runat="server" Text="待立案" OnClick="btnSub_Click" />
                        <asp:Button ID="btnSave" runat="server" Text="暂存" OnClick="btnSave_Click" />
                        <asp:Button ID="btnBack" runat="server" Text="退回" OnClick="btnBack_Click" /><asp:HiddenField ID="hfVerifyInfo" runat="server" />--%>
                    </div>
                    <div class="cmd">
                        <asp:Button ID="btnMerges" runat="server" Text="归并" OnClick="btnMerges_Click" />
                        <asp:Button ID="btnIsFeed" runat="server" Text="开启征询意见" OnClick="btnIsFeed_Click" />
                        <asp:Button ID="btnNoFeed" runat="server" Text="关闭征询意见" OnClick="btnNoFeed_Click" />
                    </div>
                    <div class="list hover">
                        <strong>结果展现
                        <span>符合条件的数据有：<b><asp:Literal ID="ltQueryTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                        </strong>
                        <asp:PlaceHolder ID="plQueryOpinion" runat="server" Visible="false">
                            <table>
                                <thead>
                                    <tr>
                                        <asp:Literal ID="ltQueryThead" runat="server">
                                    <th class="num">序号</th>
                                    <th class="state">提案号</th>
                                    <th class="time">类别</th>
                                    <th>案由</th>
                                    <th class="state">时间标识</th>
                                    <th class="date">提交日期</th>
                                    <th class="state">办理情况</th>
                                        </asp:Literal>
                                        <th class="cmd">
                                            <input type="checkbox" title="全选" />操作</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rpQueryTbody" runat="server">
                                        <ItemTemplate>
                                            <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                            <%#DataBinder.Eval(Container.DataItem, "tbody")%>
                                            <td>
                                                <asp:CheckBox ID="_ck" runat="server" /><asp:HiddenField ID="_id" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "Id") %>'/>
                                                <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>选取</u></a>
                                            </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <asp:Repeater ID="rpQueryList" runat="server">
                                        <ItemTemplate>
                                            <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                            <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                            <td><%#DataBinder.Eval(Container.DataItem, "OpNo")%></td>
                                            <td><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                            <td><%#DataBinder.Eval(Container.DataItem, "Summary")%></td>
                                            <td align="center"><%#DataBinder.Eval(Container.DataItem, "TimeMark")%></td>
                                            <td align="center"><%#DataBinder.Eval(Container.DataItem, "SubTime", "{0:yyyy-MM-dd}")%></td>
                                            <td align="center"><%#DataBinder.Eval(Container.DataItem, "ApplyState")%></td>
                                            <td>
                                                <asp:CheckBox ID="_ck" runat="server" /><asp:HiddenField ID="_id" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "Id") %>'/>
                                                <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>选取</u></a>
                                            </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <asp:Literal ID="ltQueryNo" runat="server">
                                    <tr>
                                        <td class="no">暂无提案！</td>
                                    </tr>
                                    </asp:Literal>
                                </tbody>
                            </table>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plQueryUser" runat="server" Visible="false">
                            <table>
                                <thead>
                                    <tr>
                                        <th>委员编号</th>
                                        <th>姓名</th>
                                        <th>性别</th>
                                        <th>政治面貌</th>
                                        <th>手机</th>
                                        <th>专委会</th>
                                        <th>界别</th>
                                        <th>街道活动组</th>
                                        <th>工作单位</th>
                                        <th>提案数</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rpUser" runat="server">
                                        <ItemTemplate>
                                            <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                            <td><%#DataBinder.Eval(Container.DataItem, "UserCode")%></td>
                                            <td><%#DataBinder.Eval(Container.DataItem, "TrueName")%></td>
                                            <td><%#DataBinder.Eval(Container.DataItem, "UserSex")%></td>
                                            <td><%#DataBinder.Eval(Container.DataItem, "Party")%></td>
                                            <td><%#DataBinder.Eval(Container.DataItem, "Mobile")%></td>
                                            <td><%#DataBinder.Eval(Container.DataItem, "Committee")%></td>
                                            <td><%#DataBinder.Eval(Container.DataItem, "Subsector")%></td>
                                            <td><%#DataBinder.Eval(Container.DataItem, "StreetTeam")%></td>
                                            <td><%#DataBinder.Eval(Container.DataItem, "OrgName")%></td>
                                            <td><%#DataBinder.Eval(Container.DataItem, "UserSubNum")%></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <asp:Literal ID="ltUserNo" runat="server">
                                    <tr>
                                        <td class="no">未查询到委员！</td>
                                    </tr>
                                    </asp:Literal>
                                </tbody>
                            </table>
                        </asp:PlaceHolder>
                        <asp:Label ID="lblQueryNav" runat="server" CssClass="nav"></asp:Label>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="plEdit" runat="server" Visible="false">
                    <script>
                        $(function () {
                            loadEditor('#txtBody', '#editorBody', true);
                            upFile('#hfFiles', '#files', '#btnFiles', 'file');
                            loadSelMenu('#hfOrg', '#txtExamHelpOrg', '#ExamHelpOrg', '', 'add');
                            $('#rblSubManType>input:radio').change(function () {
                                if ($(this).is(':checked')) {
                                    switch ($(this).val()) {
                                        case '委员':
                                            $('.edit>table>tbody>tr.mans').show();
                                            $('.edit>table>tbody>tr.team').hide();
                                            break;
                                        default:
                                            $('.edit>table>tbody>tr.mans').hide();
                                            $('.edit>table>tbody>tr.team').show();
                                            break;
                                    }
                                }
                            }).change();
                            $('#rblIsOpen>input:radio').change(function () {
                                if ($(this).is(':checked')) {
                                    if ($(this).val() == '否') {
                                        $('#txtMOpenInfo').show();
                                    } else {
                                        $('#txtMOpenInfo').hide();
                                    }
                                }
                            }).change();
                            $('#btnSubMan, #btnSubMan2, #btnSubMans').click(function () {
                                var title = '';
                                var obj = '';
                                switch ($(this).attr('id')) {
                                    case 'btnSubMan':
                                        title = '选取第一提案人';
                                        obj = 'txtSubMan';
                                        break;
                                        //case 'btnSubMan2':
                                        //    title = '选取第二提案人';
                                        //    obj = 'txtSubMan2';
                                        //    break;
                                    default:
                                        title = '选取联名提案人';
                                        obj = 'txtSubMans';
                                        break;
                                }
                                showDialog(title, '../cn/dialog.aspx?ac=subman&type=all&obj=' + obj, '', 640, 400, 'no');
                                return false;
                            });
                            $('#txtSubMan, #txtSubMan2, #txtSubMans').focus(function () {
                                //$('#' + $(this).attr('id').replace('txt', 'btn')).click();
                            }).change(function () {
                                switch ($(this).attr('id')) {
                                    case 'txtSubMan2':
                                        var arr = $(this).val().split(',');
                                        var str = '';
                                        for (i = 0; i < arr.length; i++) {
                                            if (arr[i] != $('#txtSubMan').val()) {
                                                if (str) {
                                                    str += ',';
                                                }
                                                str += arr[i];
                                            }
                                        }
                                        $(this).val(str);
                                        break;
                                    case 'txtSubMans':
                                        var arr = $(this).val().split(',');
                                        var str = '';
                                        for (i = 0; i < arr.length; i++) {
                                            if (!(arr[i] == $('#txtSubMan').val() || arr[i] == $('#txtSubMan2').val())) {
                                                if (str) {
                                                    str += ',';
                                                }
                                                str += arr[i];
                                            }
                                        }
                                        $(this).val(str);
                                        break;
                                    default:
                                        break;
                                }
                            });
                            $('#ddlActiveName').change(function () {
                                if ($(this).val() == '不立案') {
                                    $('#ddlVerifyInfo').show();
                                } else {
                                    $('#ddlVerifyInfo').hide();
                                }
                            }).change();
                            $('#btnDel').click(function () {
                                if (!confirm('您确定要“' + $(this).val() + '”吗?')) {
                                    return false;
                                }
                            });
                            $('#btnPop').click(function () {
                                if (!confirm('您确定要“' + $(this).val() + '”吗?')) {
                                    return false;
                                }
                                if ($('#ddlActiveName').val() != '不立案') {
                                    $('#ddlActiveName').val('不立案').change();
                                    alert('请选择“不立案”的理由！');
                                    $('#ddlVerifyInfo').focus();
                                    return false;
                                }
                            });
                            $('form').submit(function () {
                                try {
                                    if ($('#rblSubManType>input:radio:checked').val() == '委员') {
                                        if (checkEmpty('#txtSubMan')) {
                                            return false;
                                        }
                                    } else {
                                        if (checkEmpty('#txtSubOrg') || checkEmpty('#txtLinkman') || checkEmpty('#txtLinkmanTel')) {
                                            return false;
                                        }
                                    }
                                    if ($('#rblIsOpen>input:radio:checked').val() == '否' && checkEmpty('#txtOpenInfo')) {
                                        return false;
                                    }
                                    if (checkRadio('#rblIsOpen') || checkEmpty('#txtSubTime') || checkEmpty('#txtSummary')) {
                                        return false;
                                    }
                                    if ($('#txtBody').val() == '' && $('#hfFiles').val() == '') {
                                        alert('请填写[内容]或上传[附件]');
                                        return false;
                                    }
                                    if ($('#ddlActiveName').val() == "立案" && (checkRadio('#rblSubType') || checkSelect('#ddlApplyState') || checkSelect('#ddlExamHostOrg') || checkSelect('#ddlExamHelpOrg'))) {//checkEmpty('#txtOpNo')
                                        return false;
                                    }
                                } catch (err) {
                                    alert("验证出错，请稍后重试！");
                                    return false;
                                }
                            });
                        });
                    </script>
                    <div class="frm edit">
                        <strong class="center">虹口区政协
                        <asp:DropDownList ID="ddlPeriod" runat="server"></asp:DropDownList>届
                        <asp:DropDownList ID="ddlTimes" runat="server"></asp:DropDownList>次会议
                        第<asp:TextBox ID="txtTeamNum" runat="server" MaxLength="2"></asp:TextBox>组
                        提案信息
                        </strong>
                        <table>
                            <tbody>
                                <tr>
                                    <th>提案流水号</th>
                                    <td>
                                        <asp:TextBox ID="txtId" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                    <th>内部流水号</th>
                                    <td>
                                        <asp:TextBox ID="txtOpNum" runat="server" CssClass="readonly"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <th>提案类别</th>
                                    <td>
                                        <asp:RadioButtonList ID="rblSubType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="提案类别"></asp:RadioButtonList>
                                    </td>
                                    <th><b>*</b>提交日期</th>
                                    <td>
                                        <asp:TextBox ID="txtSubTime" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})" ToolTip="提交日期"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <th><b>*</b>提案者性质</th>
                                    <td>
                                        <asp:RadioButtonList ID="rblSubManType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
                                    </td>
                                    <th><b>*</b>是否同意公开</th>
                                    <td>
                                        <asp:RadioButtonList ID="rblIsOpen" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="是否同意公开">
                                            <asp:ListItem Text="是" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="否"></asp:ListItem>
                                        </asp:RadioButtonList>
                                        <asp:TextBox ID="txtOpenInfo" runat="server" MaxLength="20" ToolTip="原因"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr class="mans">
                                    <th><b>*</b>第一提案人</th>
                                    <td>
                                        <asp:TextBox ID="txtSubMan" runat="server" ReadOnly="true" CssClass="readonly" ToolTip="第一提案人"></asp:TextBox><br />
                                        <a id="btnSubMan" href="#" class="btn"><u>选取</u></a>
                                    </td>
                                    <th>联名提案人</th>
                                    <td>
                                        <asp:TextBox ID="txtSubMans" runat="server" TextMode="MultiLine" Rows="2" CssClass="readonly long"></asp:TextBox>
                                        <a id="btnSubMans" href="#" class="btn"><u>选取</u></a>
                                    </td>
                                </tr>
                                <tr class="team">
                                    <th><b>*</b>组织名称</th>
                                    <td>
                                        <asp:TextBox ID="txtSubOrg" runat="server" ReadOnly="true" CssClass="readonly long" ToolTip="组织名称"></asp:TextBox></td>
                                    <th></th>
                                    <td></td>
                                </tr>
                                <tr class="team">
                                    <th><b>*</b>联系人</th>
                                    <td>
                                        <asp:TextBox ID="txtLinkman" runat="server" MaxLength="20" ToolTip="联系人"></asp:TextBox></td>
                                    <th><b>*</b>联系电话</th>
                                    <td>
                                        <asp:TextBox ID="txtLinkmanTel" runat="server" MaxLength="50" ToolTip="联系电话"></asp:TextBox></td>
                                </tr>
                                <tr class="team">
                                    <th>通讯地址</th>
                                    <td>
                                        <asp:TextBox ID="txtLinkmanAddress" runat="server" MaxLength="100" CssClass="long"></asp:TextBox></td>
                                    <th>邮政编码</th>
                                    <td>
                                        <asp:TextBox ID="txtLinkmanZip" runat="server" MaxLength="6"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <th><b>*</b>案由</th>
                                    <td colspan="3">
                                        <asp:TextBox ID="txtSummary" runat="server" TextMode="MultiLine" CssClass="long" Rows="3" MaxLength="100" ToolTip="案由"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <th>提案者意向主办单位</th>
                                    <td>
                                        <asp:DropDownList ID="ddlAdviseHostOrg" runat="server"></asp:DropDownList>
                                    </td>
                                    <th>提案者意向会办单位</th>
                                    <td>
                                        <asp:DropDownList ID="ddlAdviseHelpOrg" runat="server"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <th>提案内容</th>
                                    <td colspan="3">
                                        <div id="editorBody" class="editor"><%--<p>欢迎使用 <b>wangEditor</b> 富文本编辑器</p>--%></div>
                                        <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" CssClass="long" Rows="8" ToolTip="提案内容"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <th>附件</th>
                                    <td colspan="3">
                                        <asp:HiddenField ID="hfFiles" runat="server" />
                                        <a id="btnFiles" href="#" class="btn"><u>上传</u></a>
                                        <div id="files"></div>
                                    </td>
                                </tr>
                                <tr>
                                    <th>提案序号</th>
                                    <td>
                                        <asp:TextBox ID="txtOpNo" runat="server" MaxLength="20" ToolTip="提案序号"></asp:TextBox>
                                    </td>
                                    <th><b>*</b>提案性质</th>
                                    <td>
                                        <asp:DropDownList ID="ddlActiveName" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlVerifyInfo" runat="server"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <th>时间标识</th>
                                    <td>
                                        <asp:DropDownList ID="ddlTimeMark" runat="server">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem Text="会间" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="会后"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>是否会签</th>
                                    <td>
                                        <asp:DropDownList ID="ddlIsSign" runat="server">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem Text="是"></asp:ListItem>
                                            <asp:ListItem Text="否" Selected="True"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <th>办理状态</th>
                                    <td>
                                        <asp:DropDownList ID="ddlApplyState" runat="server" ToolTip="办理状态">
                                            <asp:ListItem></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>计划办结日期</th>
                                    <td>
                                        <asp:TextBox ID="txtPlannedDate" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <th>是否重点提案</th>
                                    <td>
                                        <asp:DropDownList ID="ddlIsPoint" runat="server">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem Text="是"></asp:ListItem>
                                            <asp:ListItem Text="否"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>是否优秀提案</th>
                                    <td>
                                        <asp:DropDownList ID="ddlIsGood" runat="server">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem Text="是"></asp:ListItem>
                                            <asp:ListItem Text="否"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <th>再办理</th>
                                    <td>
                                        <asp:Literal ID="ltReApply" runat="server"></asp:Literal>
                                        <asp:DropDownList ID="ddlReApply" runat="server">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem Text="是"></asp:ListItem>
                                            <asp:ListItem Text="否"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>是否需要征询意见</th>
                                    <td>
                                        <asp:DropDownList ID="ddlIsFeed" runat="server">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem Text="是"></asp:ListItem>
                                            <asp:ListItem Text="否"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <th>审查意向主办单位</th>
                                    <td>
                                        <asp:DropDownList ID="ddlExamHostOrg" runat="server" ToolTip="主办单位">
                                            <asp:ListItem></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <%--<th>审查意向会办单位</th>
                                <td>
                                    <asp:DropDownList ID="ddlExamHelpOrg" runat="server" ToolTip="会办单位">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th>审查意向会办单位2</th>
                                <td>
                                    <asp:DropDownList ID="ddlExamHelpOrg2" runat="server" ToolTip="会办单位">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                </td>--%>
                                    <th>审查意向会办单位</th>
                                    <td>
                                        <asp:TextBox ID="txtExamHelpOrg" runat="server" CssClass="readonly long" ToolTip="会办单位"></asp:TextBox>
                                        <div id="ExamHelpOrg" class="selmenu"></div>
                                    </td>
                                </tr>
                                <tr>
                                    <th>立案时间</th>
                                    <td>
                                        <asp:TextBox ID="txtRegTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                    <th>办复时间</th>
                                    <td>
                                        <asp:TextBox ID="txtResultTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <th>办理结果</th>
                                    <td>
                                        <asp:DropDownList ID="ddlResultInfo" runat="server">
                                            <asp:ListItem></asp:ListItem>
                                        </asp:DropDownList><br />
                                        <asp:TextBox ID="txtResultBody" runat="server" TextMode="MultiLine" Rows="3" CssClass="long"></asp:TextBox>
                                    </td>
                                    <th>跟踪办理结果</th>
                                    <td>
                                        <asp:DropDownList ID="ddlResultInfo2" runat="server">
                                            <asp:ListItem></asp:ListItem>
                                        </asp:DropDownList><br />
                                        <asp:TextBox ID="txtResultBody2" runat="server" TextMode="MultiLine" Rows="3" CssClass="long"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <th>备注</th>
                                    <td colspan="3">
                                        <asp:TextBox ID="txtRemark" runat="server" TextMode="MultiLine" Rows="5" CssClass="long"></asp:TextBox></td>
                                </tr>
                            </tbody>
                        </table>
                        <div class="cmd">
                            <asp:Button ID="btnEdit" runat="server" Text="审核" OnClick="btnEdit_Click" />
                            <input type="button" value="返回" onclick="window.history.back(-1);" />
                            <asp:Button ID="btnDel" runat="server" Visible="false" Text="删除" OnClick="btnDel_Click" />
                            <asp:Button ID="btnPop" runat="server" Visible="false" Text="转社情民意" OnClick="btnPop_Click" />
                        </div>
                    </div>
                    <asp:Panel ID="plSign" runat="server" Visible="false" CssClass="list hover">
                        <script>
                            $(function () {
                                $('#plSign>table>tbody>tr>td>a').click(function () {
                                    var url = '../cn/dialog.aspx' + $(this).attr('href');
                                    showDialog('提案会签', url, '', 600, 320, 'no');
                                    return false;
                                });
                            });
                        </script>
                        <strong>联名提案会签情况表</strong>
                        <table>
                            <thead>
                                <tr>
                                    <th class="state">联名委员</th>
                                    <th class="state">时间标识</th>
                                    <th class="time">签名时间</th>
                                    <th>签名意见</th>
                                    <th class="time">签名结束时间</th>
                                    <th class="state">状态</th>
                                    <th class="cmd">操作</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rpSignList" runat="server">
                                    <ItemTemplate>
                                        <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SignUser")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SignMark")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SignTimeText")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Body")%><p style="color:#F00;"><%#DataBinder.Eval(Container.DataItem, "Remark")%></p></td>
                                        <td id="s<%#DataBinder.Eval(Container.DataItem, "Id")%>"><%#DataBinder.Eval(Container.DataItem, "Overdue", "{0:yyyy-MM-dd HH:mm:ss}")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td><a href="?ac=sign&id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>修改</u></a></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </asp:Panel>
                    <div class="edit">
                        <strong>提案办理情况征询意见表</strong>
                        <asp:Repeater ID="rpFeedList" runat="server">
                            <ItemTemplate>
                                <table>
                                    <thead>
                                        <tr>
                                            <th>反馈时间</th>
                                            <td><%#DataBinder.Eval(Container.DataItem, "AddTime", "{0:yyyy-MM-dd HH:mm:ss}")%></td>
                                            <th>反馈流水号</th>
                                            <td><%#DataBinder.Eval(Container.DataItem, "Id")%></td>
                                            <th>反馈人</th>
                                            <td><%#DataBinder.Eval(Container.DataItem, "AddUser")%></td>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <th>走访情况</th>
                                            <td><%#DataBinder.Eval(Container.DataItem, "Interview")%></td>
                                            <th>办理(走访)<br />
                                                人员态度</th>
                                            <td><%#DataBinder.Eval(Container.DataItem, "Attitude")%></td>
                                            <th>是否同意<br />
                                                办理结果</th>
                                            <td><%#DataBinder.Eval(Container.DataItem, "Result")%></td>
                                        </tr>
                                        <tr style="display: none;">
                                            <th>答复前听取<br />
                                                意见方式</th>
                                            <td><%#DataBinder.Eval(Container.DataItem, "TakeWay")%></td>
                                            <th>答复是否<br />
                                                针对提案</th>
                                            <td><%#DataBinder.Eval(Container.DataItem, "Pertinence")%></td>
                                            <th>(团体)分管领导答复</span></th>
                                            <td><%#DataBinder.Eval(Container.DataItem, "LeaderReply")%></td>
                                        </tr>
                                        <tr>
                                            <th>对提案办理的<br />
                                                意见或建议</th>
                                            <td colspan="5"><%#DataBinder.Eval(Container.DataItem, "Body")%></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="plMerge" runat="server" Visible="false">
                    <script>
                        $(function () {
                            loadEditor('#txtMBody', '#editorMBody', true);
                            upFile('#hfMFiles', '#Mfiles', '#btnMFiles', 'file');
                            $('#rblMSubManType>input:radio').change(function () {
                                if ($(this).is(':checked')) {
                                    switch ($(this).val()) {
                                        case '委员':
                                            $('.edit>table>tbody>tr.mans').show();
                                            $('.edit>table>tbody>tr.team').hide();
                                            break;
                                        default:
                                            $('.edit>table>tbody>tr.mans').hide();
                                            $('.edit>table>tbody>tr.team').show();
                                            break;
                                    }
                                }
                            }).change();
                            $('#rblMIsOpen>input:radio').change(function () {
                                if ($(this).is(':checked')) {
                                    if ($(this).val() == '否') {
                                        $('#txtMOpenInfo').show();
                                    } else {
                                        $('#txtMOpenInfo').hide();
                                    }
                                }
                            }).change();
                            $('#btnMSubMan, #btnMSubMan2, #btnMSubMans').click(function () {
                                var title = '';
                                var obj = '';
                                switch ($(this).attr('id')) {
                                    case 'btnMSubMan':
                                        title = '选取第一提案人';
                                        obj = 'txtMSubMan';
                                        break;
                                        //case 'btnMSubMan2':
                                        //    title = '选取第二提案人';
                                        //    obj = 'txtMSubMan2';
                                        //    break;
                                    default:
                                        title = '选取联名提案人';
                                        obj = 'txtMSubMans';
                                        break;
                                }
                                showDialog(title, '../cn/dialog.aspx?ac=subman&obj=' + obj, '', 640, 400, 'no');
                                return false;
                            });
                            $('#txtMSubMan, #txtMSubMan2, #txtMSubMans').focus(function () {
                                $('#' + $(this).attr('id').replace('txt', 'btn')).click();
                            }).change(function () {
                                switch ($(this).attr('id')) {
                                    case 'txtMSubMan2':
                                        var arr = $(this).val().split(',');
                                        var str = '';
                                        for (i = 0; i < arr.length; i++) {
                                            if (arr[i] != $('#txtMSubMan').val()) {
                                                if (str) {
                                                    str += ',';
                                                }
                                                str += arr[i];
                                            }
                                        }
                                        $(this).val(str);
                                        break;
                                    case 'txtMSubMans':
                                        var arr = $(this).val().split(',');
                                        var str = '';
                                        for (i = 0; i < arr.length; i++) {
                                            if (!(arr[i] == $('#txtMSubMan').val() || arr[i] == $('#txtMSubMan2').val())) {
                                                if (str) {
                                                    str += ',';
                                                }
                                                str += arr[i];
                                            }
                                        }
                                        $(this).val(str);
                                        break;
                                    default:
                                        break;
                                }
                            });
                            $('form').submit(function () {
                                try {
                                    if (!confirm('您确定要“' + $('#btnMerge').val() + '”吗?')) {
                                        return false;
                                    }
                                    if ($('#hfMSubManType').val() == '委员') {
                                        if (checkEmpty('#txtMSubMan')) {
                                            return false;
                                        }
                                    } else if (checkEmpty('#txtMSubOrg') || checkEmpty('#txtMLinkman') || checkEmpty('#txtMLinkmanTel')) {
                                        return false;
                                    }
                                    if ($('#rblMIsOpen>input:radio:checked').val() == '否' && checkEmpty('#txtMOpenInfo')) {
                                        return false;
                                    }
                                    if (checkRadio('#rblMIsOpen') || checkEmpty('#txtMSubTime') || checkRadio('#rblMSubType') || checkEmpty('#txtMSummary')) {
                                        return false;
                                    }
                                    if ($('#txtMBody').val() == '' && $('#hfMFiles').val() == '') {
                                        alert('请填写[内容]或上传[附件]');
                                        return false;
                                    }
                                } catch (err) {
                                    alert("验证出错，请稍后重试！");
                                    return false;
                                }
                            });
                            $('#MergeTitle').change(function () {
                                var id = $(this).val();
                                $('#merges>.merge').each(function () {
                                    if ($(this).find('.id').text() == id) {
                                        $('#txtMSummary').val($(this).find('.title').text());
                                        $('#txtMBody').val($(this).find('.body').text()).change();
                                        $('#hfMFiles').val($(this).find('.files').text()).change();
                                        var subtype = $(this).find('.subtype').text();
                                        $('#rblMSubType>input:radio').each(function () {
                                            if ($(this).val() == subtype) {
                                                $(this).click();
                                            }
                                        });
                                        var submantype = $(this).find('.submantype').text();
                                        $('#rblMSubManType>input:radio').each(function () {
                                            if ($(this).val() == submantype) {
                                                $(this).click();
                                            }
                                        });
                                    }
                                });
                            });
                            if ($('#merges').text()) {
                                var str = '';
                                var subman = '';
                                var subman2 = '';
                                var submans = '';
                                $('#merges>.merge').each(function () {
                                    str += '<option value="' + $(this).find('.id').text() + '">' + $(this).find('.title').text() + '</option>';
                                    var man = $(this).find('.subman').text().replace(/(^[\s,]*)|([\s,]*$)/g, '');
                                    if (man) {
                                        if (subman) {
                                            subman += ',';
                                        }
                                        subman += man;
                                    }
                                    //var man2 = $(this).find('.subman2').text().replace(/(^[\s,]*)|([\s,]*$)/g, '');
                                    //if (man2) {
                                    //    if (subman2) {
                                    //        subman2 += ',';
                                    //    }
                                    //    subman2 += man2;
                                    //}
                                    var mans = $(this).find('.submans').text().replace(/(^[\s,]*)|([\s,]*$)/g, '');
                                    if (mans) {
                                        if (submans) {
                                            submans += ',';
                                        }
                                        submans += mans;
                                    }
                                });
                                $('#MergeTitle').html(str).change();
                                $('#txtMSubMan').val(subman);
                                //$('#txtMSubMan2').val(subman2);
                                $('#txtMSubMans').val(submans);
                                $('#txtMSubOrg').val(subman);
                            }
                        });
                    </script>
                    <div class="frm edit">
                        <strong class="center">虹口区政协
                        <asp:DropDownList ID="ddlMPeriod" runat="server"></asp:DropDownList>届
                        <asp:DropDownList ID="ddlMTimes" runat="server"></asp:DropDownList>次会议
                        提案信息
                        </strong>
                        <div id="merges" style="display: none;">
                            <asp:Repeater ID="rpMergeList" runat="server">
                                <ItemTemplate>
                                    <div class="merge">
                                        <div class="id"><%#DataBinder.Eval(Container.DataItem, "Id")%></div>
                                        <div class="subtype"><%#DataBinder.Eval(Container.DataItem, "SubType")%></div>
                                        <div class="title"><%#DataBinder.Eval(Container.DataItem, "Summary")%></div>
                                        <div class="submantype"><%#DataBinder.Eval(Container.DataItem, "SubManType")%></div>
                                        <div class="subman"><%#DataBinder.Eval(Container.DataItem, "SubMan")%></div>
                                        <div class="submans"><%#DataBinder.Eval(Container.DataItem, "SubMans")%></div>
                                        <div class="body"><%#DataBinder.Eval(Container.DataItem, "Body")%></div>
                                        <div class="file"><%#DataBinder.Eval(Container.DataItem, "Files")%></div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <table>
                            <tbody>
                                <tr>
                                    <th>提案号</th>
                                    <td>
                                        <asp:TextBox ID="txtMOpNo" runat="server"></asp:TextBox></td>
                                    <th><b>*</b>主提案</th>
                                    <td>
                                        <asp:HiddenField ID="hfMergeId" runat="server" />
                                        <select id="MergeTitle" class="long"></select>
                                    </td>
                                </tr>
                                <tr>
                                    <th><b>*</b>提案类别</th>
                                    <td>
                                        <asp:RadioButtonList ID="rblMSubType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="提案类别"></asp:RadioButtonList>
                                    </td>
                                    <th><b>*</b>提交日期</th>
                                    <td>
                                        <asp:TextBox ID="txtMSubTime" runat="server" MaxLength="10" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" ToolTip="提交日期"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <th><b>*</b>提案者性质</th>
                                    <td>
                                        <asp:RadioButtonList ID="rblMSubManType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="提案者性质"></asp:RadioButtonList>
                                    </td>
                                    <th><b>*</b>是否同意公开</th>
                                    <td>
                                        <asp:RadioButtonList ID="rblMIsOpen" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="是否同意公开">
                                            <asp:ListItem Text="是" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="否"></asp:ListItem>
                                        </asp:RadioButtonList>
                                        <asp:TextBox ID="txtMOpenInfo" runat="server" MaxLength="20" ToolTip="原因"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <th><b>*</b>案由</th>
                                    <td colspan="3">
                                        <asp:TextBox ID="txtMSummary" runat="server" TextMode="MultiLine" CssClass="long" Rows="3" MaxLength="100" ToolTip="案由"></asp:TextBox></td>
                                </tr>
                                <tr class="mans">
                                    <th><b>*</b>第一提案人</th>
                                    <td>
                                        <asp:TextBox ID="txtMSubMan" runat="server" ReadOnly="true" CssClass="readonly long" ToolTip="第一提案人"></asp:TextBox><br />
                                        <a id="btnMSubMan" href="#" class="btn"><u>选取</u></a>
                                    </td>
                                    <th>联名提案人</th>
                                    <td>
                                        <asp:TextBox ID="txtMSubMans" runat="server" TextMode="MultiLine" Rows="2" CssClass="readonly long"></asp:TextBox>
                                        <a id="btnMSubMans" href="#" class="btn"><u>选取</u></a>
                                    </td>
                                </tr>
                                <tr class="team">
                                    <th><b>*</b>组织名称</th>
                                    <td>
                                        <asp:TextBox ID="txtMSubOrg" runat="server" ReadOnly="true" CssClass="readonly long" ToolTip="组织名称"></asp:TextBox></td>
                                    <th></th>
                                    <td></td>
                                </tr>
                                <tr class="team">
                                    <th><b>*</b>联系人</th>
                                    <td>
                                        <asp:TextBox ID="txtMLinkman" runat="server" MaxLength="20" ToolTip="联系人"></asp:TextBox></td>
                                    <th><b>*</b>联系电话</th>
                                    <td>
                                        <asp:TextBox ID="txtMLinkmanTel" runat="server" MaxLength="50" ToolTip="联系电话"></asp:TextBox></td>
                                </tr>
                                <tr class="team">
                                    <th>通讯地址</th>
                                    <td>
                                        <asp:TextBox ID="txtMLinkmanAddress" runat="server" MaxLength="100" CssClass="long"></asp:TextBox></td>
                                    <th>邮政编码</th>
                                    <td>
                                        <asp:TextBox ID="txtMLinkmanZip" runat="server" MaxLength="6"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <th>提案内容</th>
                                    <td colspan="3">
                                        <div id="editorMBody" class="editor"><%--<p>欢迎使用 <b>wangEditor</b> 富文本编辑器</p>--%></div>
                                        <asp:TextBox ID="txtMBody" runat="server" TextMode="MultiLine" CssClass="long" Rows="8" ToolTip="提案内容"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <th>附件</th>
                                    <td colspan="3">
                                        <asp:HiddenField ID="hfMFiles" runat="server" />
                                        <a id="btnMFiles" href="#" class="btn"><u>上传</u></a>
                                        <div id="Mfiles"></div>
                                    </td>
                                </tr>
                                <tr>
                                    <th><b>*</b>提案性质</th>
                                    <td>
                                        <asp:DropDownList ID="ddlMActiveName" runat="server">
                                            <asp:ListItem Text="待立案"></asp:ListItem>
                                            <asp:ListItem Text="立案"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>是否重点提案</th>
                                    <td>
                                        <asp:DropDownList ID="ddlMIsPoint" runat="server">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem Text="是"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <th>时间标识</th>
                                    <td>
                                        <asp:DropDownList ID="ddlMTimeMark" runat="server">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem Text="会间"></asp:ListItem>
                                            <asp:ListItem Text="会后"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <th>是否会签</th>
                                    <td>
                                        <asp:DropDownList ID="ddlMIsSign" runat="server">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem Text="是"></asp:ListItem>
                                            <asp:ListItem Text="否"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <div class="cmd">
                            <asp:Button ID="btnMerge" runat="server" Text="归并" OnClick="btnMerge_Click" />
                        </div>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="plFeed" runat="server" Visible="false">
                    <script>
                        $(function () {
                            $('#btnFeed').click(function () {
                                try {
                                    var url = '';
                                    if ($('#ddlFPeriod').val()) {
                                        url += '&Period=' + $('#ddlFPeriod').val();
                                    }
                                    if ($('#ddlFTimes').val()) {
                                        url += '&Times=' + $('#ddlFTimes').val();
                                    }
                                    if (url != '') {
                                        url = encodeURI(url);
                                    }
                                    window.location.href = '?ac=feed' + url;
                                    return false;
                                } catch (err) {
                                    alert("验证出错，请稍后重试！");
                                    return false;
                                }
                            });
                            $('.list>table>tbody>tr>td>i').each(function () {
                                var w = ($(this).text()) ? $(this).text() : '0';
                                $(this).parent().find('span').width(w);
                            });
                        });
                    </script>
                    <div class="frm edit">
                        <strong>征询意见统计</strong>
                        <table>
                            <tbody>
                                <tr>
                                    <td colspan="6" align="center">虹口区政协
                                    <asp:DropDownList ID="ddlFPeriod" runat="server" Width="60">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>届
                                    <asp:DropDownList ID="ddlFTimes" runat="server" Width="60">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>次
                                    会议提案信息
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <div class="cmd">
                            <input id="btnFeed" type="button" value="查询" />
                        </div>
                    </div>
                    <div class="list percent">
                        <table>
                            <tbody>
                                <tr>
                                    <th width="130px">提案 (<asp:Literal ID="ltCount1" runat="server"></asp:Literal>)</th>
                                    <td>
                                        <span></span>
                                        征询意见 (<asp:Literal ID="ltCount2" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltCount20" runat="server"></asp:Literal></i>
                                    </td>
                                </tr>
                                <tr>
                                    <th rowspan="3">走访情况</th>
                                    <td>
                                        <span></span>
                                        已走访 (<asp:Literal ID="ltInterview1" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltInterview10" runat="server" /></i>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span></span>
                                        委员本人提出不需要走访 (<asp:Literal ID="ltInterview2" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltInterview20" runat="server" /></i>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span></span>
                                        未走访 (<asp:Literal ID="ltInterview3" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltInterview30" runat="server" /></i>
                                    </td>
                                </tr>
                                <tr>
                                    <th rowspan="3">办理态度</th>
                                    <td>
                                        <span></span>
                                        满意 (<asp:Literal ID="ltAttitude1" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltAttitude10" runat="server" /></i>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span></span>
                                        理解 (<asp:Literal ID="ltAttitude2" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltAttitude20" runat="server" /></i>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span></span>
                                        不满意 (<asp:Literal ID="ltAttitude3" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltAttitude30" runat="server" /></i>
                                    </td>
                                </tr>
                                <tr style="display: none;">
                                    <th rowspan="4">听取意见方式</th>
                                    <td>
                                        <span></span>
                                        走访 (<asp:Literal ID="ltTakeWay1" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltTakeWay10" runat="server" /></i>
                                    </td>
                                </tr>
                                <tr style="display: none;">
                                    <td>
                                        <span></span>
                                        电话 (<asp:Literal ID="ltTakeWay2" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltTakeWay20" runat="server" /></i>
                                    </td>
                                </tr>
                                <tr style="display: none;">
                                    <td>
                                        <span></span>
                                        其他 (<asp:Literal ID="ltTakeWay3" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltTakeWay30" runat="server" /></i>
                                    </td>
                                </tr>
                                <tr style="display: none;">
                                    <td>
                                        <span></span>
                                        未联系 (<asp:Literal ID="ltTakeWay4" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltTakeWay40" runat="server" /></i>
                                    </td>
                                </tr>
                                <tr style="display: none;">
                                    <th rowspan="3">答复是否针对提案</th>
                                    <td>
                                        <span></span>
                                        针对 (<asp:Literal ID="ltPertinence1" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltPertinence10" runat="server" /></i>
                                    </td>
                                </tr>
                                <tr style="display: none;">
                                    <td>
                                        <span></span>
                                        基本针对 (<asp:Literal ID="ltPertinence2" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltPertinence20" runat="server" /></i>
                                    </td>
                                </tr>
                                <tr style="display: none;">
                                    <td>
                                        <span></span>
                                        未针对 (<asp:Literal ID="ltPertinence3" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltPertinence30" runat="server" /></i>
                                    </td>
                                </tr>
                                <tr>
                                    <th rowspan="4">办理结果</th>
                                    <td>
                                        <span></span>
                                        同意 (<asp:Literal ID="ltResult1" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltResult10" runat="server" /></i>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span></span>
                                        理解 (<asp:Literal ID="ltResult2" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltResult20" runat="server" /></i>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span></span>
                                        保留 (<asp:Literal ID="ltResult3" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltResult30" runat="server" /></i>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span></span>
                                        不同意 (<asp:Literal ID="ltResult4" runat="server"></asp:Literal>) <i>
                                            <asp:Literal ID="ltResult40" runat="server" /></i>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>
        <uc1:ucFooter ID="footer1" runat="server" />
    </form>
</body>
</html>
