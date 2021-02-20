<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="perform.aspx.cs" Inherits="hkzx.web.cn.perform" %><%--Tony维护--%>
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
            if ($('#plNav').text()) {
                var url = window.location.href;
                if (url.indexOf('ac=query') > 0) {
                    $('#plNav>a[href*="ac=query"]').addClass('cur');
                } else if (url.indexOf('ac=my') > 0) {
                    $('#plNav>a[href*="ac=my"]').addClass('cur');
                } else if (url.indexOf('ac=save') > 0) {
                    $('#plNav>a[href*="ac=save"]').addClass('cur');
                } else {
                    //$('#plNav>a:first').addClass('cur');
                    $('#plNav>a[href$="ac=sub"]').addClass('cur');
                }
            }
            if ($('.list>table>tbody>tr>td.no').text()) {
                $('.list>table>tbody>tr>td.no').attr('colspan', $('.list>table>thead>tr>th').length);
                $('.list>table>thead').hide();
            }
        });
    </script>
</head>
<body>
<form id="form1" runat="server">
    <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    <asp:HiddenField ID="hfBack" runat="server" />
    <asp:HiddenField ID="hfOrg" runat="server" /><asp:HiddenField ID="hfSubType" runat="server" />
    <uc1:ucHeader ID="header1" runat="server" />
    <div class="content">
        <div class="main">
            <asp:Panel ID="plNav" runat="server" Visible="false" CssClass="btn">
                <a href="?ac=sub">申请会议/活动</a>
                <a href="?ac=my">我申请的会议/活动</a>
                <a href="?ac=save">暂存<b>(<asp:Literal ID="ltSaveNum" runat="server">0</asp:Literal>)</b></a>
            </asp:Panel>

            <asp:PlaceHolder ID="plList" runat="server" Visible="false">
                <div class="frm list">
                    <strong>会议/活动通知
                        <span>符合条件的数据有：<b><asp:Literal ID="ltQueryTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="state">参加方式</th>
                                <th>类型</th>
                                <th>部门</th>
                                <th>主题</th>
                                <th class="time">起止时间</th>
                                <th>地点</th>
                                <th class="time">报名截止时间</th>
                                <th class="state">状态</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpQueryList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "IsMust")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OrgName")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td align="center">
                                            <%#DataBinder.Eval(Container.DataItem, "StartTime", "{0:yyyy年M月d日 HH:mm}")%><br />
                                            <%#DataBinder.Eval(Container.DataItem, "EndTime", "{0:yyyy年M月d日 HH:mm}")%>
                                        </td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "PerformSite")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "OverTime", "{0:yyyy年M月d日 HH:mm}")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltQueryNo" runat="server">
                                <tr>
                                    <td class="no">暂无履职活动！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblQueryNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plView" runat="server" Visible="false">
                <div class="frm edit">
                    <strong>会议/活动详情</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>活动状态</th>
                                <td><asp:Literal ID="ltActive" runat="server"></asp:Literal></td>
                                <th>参加方式</th>
                                <td><asp:Literal ID="ltIsMust" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>类型</th>
                                <td><asp:Literal ID="ltSubType" runat="server"></asp:Literal></td>
                                <th>申请部门</th>
                                <td><asp:Literal ID="ltOrgName" runat="server"></asp:Literal></td>
                            </tr>
                        <asp:PlaceHolder ID="plSubMan" runat="server" Visible="false">
                            <tr>
                                <th>联系人</th>
                                <td><asp:Literal ID="ltLinkman" runat="server"></asp:Literal></td>
                                <th>联系电话</th>
                                <td><asp:Literal ID="ltLinkmanTel" runat="server"></asp:Literal></td>
                            </tr>
                        </asp:PlaceHolder>
                            <tr>
                                <th>主题</th>
                                <td colspan="3"><asp:Literal ID="ltTitle" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>起止时间</th>
                                <td><asp:Literal ID="ltStartTime" runat="server"></asp:Literal> - <asp:Literal ID="ltEndTime" runat="server"></asp:Literal></td>
                                <th>地点</th>
                                <td><asp:Literal ID="ltPerformSite" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>报名截止时间</th>
                                <td><asp:Literal ID="ltOverTime" runat="server"></asp:Literal></td>
                                <th>签到开始时间</th>
                                <td><asp:Literal ID="ltSignTime" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>用车情况</th>
                                <td><asp:Literal ID="ltHaveBus" runat="server"></asp:Literal></td>
                                <th>其他</th>
                                <td><asp:Literal ID="ltHaveDinner" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>出席领导</th>
                                <td colspan="3"><asp:Literal ID="ltLeaders" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>参加委员</th>
                                <td colspan="3"><asp:Literal ID="ltAttendees" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>活动内容</th>
                                <td colspan="3"><asp:Literal ID="ltBody" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>附件</th>
                                <td colspan="3"><asp:Literal ID="ltFiles" runat="server"></asp:Literal></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <asp:Panel ID="plPlayFeed" runat="server" Visible="false" CssClass="list hover">
                    <strong>履职情况
                        <span>符合条件的数据有：<b><asp:Literal ID="ltFeedTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="state">状态</th>
                                <th class="state">姓名</th>
                                <th>委员单位</th>
                                <th class="state">人员类型</th>
                                <th class="state">会议发言</th>
                                <th class="state">提供资源</th>
                                <th class="time">签到时间</th>
                                <th class="time">反馈时间</th>
                                <th class="state">微信消息</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpFeedList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "IsMust")%><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center" title="<%#DataBinder.Eval(Container.DataItem, "SignManCode")%>"><%#DataBinder.Eval(Container.DataItem, "SignMan")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SignManOrg")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SignManType")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SignManSpeak")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SignManProvide")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SignTimeText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "FeedTimeText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SendMsg")%></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltFeedNo" runat="server">
                                <tr>
                                    <td class="no">暂无履职情况！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                </asp:Panel>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plFeed" runat="server" Visible="false">
<script>
    $(function () {
        $('#leave').hide();
        $('#btnLeave').click(function () {
            try {
                if ($('#txtReply').val() == '') {
                    $('#leave').slideDown(200);
                    window.setTimeout(function () {
                        alert('请填写[' + $('#txtReply').attr('title') + ']');
                        $('#txtReply').focus();
                    }, 300);
                    return false;
                }
                return true;
            } catch (err) {
                //alert("验证出错，请稍后重试！");
                return false;
            }
        });
    });
</script>
                <div class="edit">
                    <strong>您的履职情况</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>履职状态</th>
                                <td><asp:Literal ID="ltFeed" runat="server"></asp:Literal></td>
                            </tr>
                            <tr id="leave">
                                <th>请假原因</th>
                                <td><asp:TextBox ID="txtReply" runat="server" TextMode="MultiLine" Rows="3" CssClass="long" ToolTip="请假原因"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnAttend" runat="server" Visible="false" Text="参加" OnClick="btnAttend_Click" />
                        <asp:Button ID="btnNonAttend" runat="server" Visible="false" Text="不参加" OnClick="btnNonAttend_Click" />
                        <asp:Button ID="btnLeave" runat="server" Visible="false" Text="请假" OnClick="btnLeave_Click" />
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plSub" runat="server" Visible="false">
<script>
    $(function () {
        loadSelMenu('#hfOrg', '#txtOrgName', '#OrgName', '');
        loadSelMenu('#hfSubType', '#txtSubType', '#SubType', '');
        $('#txtSubType').change(function () {
            var txt = $(this).val();
            if (txt) {
                if (txt == '政协全体会议' || txt == '全体委员学习会' || txt == '常委会议') {
                    $('#txtIsMust').val('必须参加');
                } else {
                    $('#txtIsMust').val('报名参加');
                }
            }
        }).change();
        //upFile('#txtFiles', '', '#btnFiles');
        upFile('#hfFiles', '#files', '#btnFiles', 'doc');
        $('#btnAttendees').click(function () {
            var title = '选取参加委员';
            var obj = 'txtAttendees';
            showDialog(title, '../cn/dialog.aspx?ac=subman&obj=' + obj, '', 640, 400, 'no');
            return false;
        });
        $('#txtAttendees').focus(function () {
            $('#' + $(this).attr('id').replace('txt', 'btn')).click();
        });
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtOrgName') || checkEmpty('#txtSubType') || checkEmpty('#txtLinkman') || checkEmpty('#txtLinkmanTel') || checkEmpty('#txtTitle') || checkEmpty('#txtPerformSite') || checkEmpty('#txtStartTime') || checkEmpty('#txtEndTime') || checkEmpty('#txtOverTime') || checkEmpty('#txtSignTime') || checkRadio('#rblHaveBus') || checkEmpty('#txtAttendees')) {
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
                    <strong>申请会议/活动</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th><b>*</b>申请部门(单位)</th>
                                <td>
                                    <asp:DropDownList ID="ddlOrgType" runat="server" Visible="false" ToolTip="部门">
                                        <asp:ListItem Text="办公室"></asp:ListItem>
                                        <asp:ListItem Text="专委办"></asp:ListItem>
                                        <asp:ListItem Text="专委会"></asp:ListItem>
                                        <asp:ListItem Text="界别"></asp:ListItem>
                                        <asp:ListItem Text="街道活动组"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:TextBox ID="txtOrgName" runat="server" CssClass="readonly long"></asp:TextBox>
                                    <div id="OrgName" class="selmenu"></div>
                                </td>
                                <th>活动状态</th>
                                <td><asp:TextBox ID="txtActiveName" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>类型</th>
                                <td>
                                    <asp:DropDownList ID="ddlSubType" runat="server" Visible="false" ToolTip="类型"></asp:DropDownList>
                                    <asp:TextBox ID="txtSubType" runat="server" CssClass="readonly long" ToolTip="类型"></asp:TextBox>
                                    <div id="SubType" class="selmenu"></div>
                                </td>
                                <th>参加方式</th>
                                <td><asp:TextBox ID="txtIsMust" runat="server" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>联系人</th>
                                <td><asp:TextBox ID="txtLinkman" runat="server" MaxLength="20" ToolTip="联系人"></asp:TextBox></td>
                                <th><b>*</b>联系电话</th>
                                <td><asp:TextBox ID="txtLinkmanTel" runat="server" MaxLength="50" ToolTip="联系电话"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>活动主题</th>
                                <td><asp:TextBox ID="txtTitle" runat="server" MaxLength="50" CssClass="long" ToolTip="活动主题"></asp:TextBox></td>
                                <th><b>*</b>地点</th>
                                <td><asp:TextBox ID="txtPerformSite" runat="server" MaxLength="100" CssClass="long" ToolTip="地点"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>会议开始时间</th>
                                <td><asp:TextBox ID="txtStartTime" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" ToolTip="开始时间"></asp:TextBox></td>
                                <th><b>*</b>会议结束时间</th>
                                <td><asp:TextBox ID="txtEndTime" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm',minDate:'#F{$dp.$D(\'txtStartTime\')}'})" ToolTip="结束时间"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>报名截止时间</th>
                                <td><asp:TextBox ID="txtOverTime" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm',maxDate:'#F{$dp.$D(\'txtStartTime\')}'})" ToolTip="报名截止时间"></asp:TextBox></td>
                                <th><b>*</b>签到开始时间</th>
                                <td><asp:TextBox ID="txtSignTime" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm',maxDate:'#F{$dp.$D(\'txtStartTime\')}'})" ToolTip="签到开始时间"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>用车情况</th>
                                <td>
                                    <asp:RadioButtonList ID="rblHaveBus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="用车情况"></asp:RadioButtonList>
                                </td>
                                <th>其他</th>
                                <td>
                                    <asp:CheckBoxList ID="cblHaveDinner" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th>出席领导</th>
                                <td colspan="3"><asp:TextBox ID="txtLeaders" runat="server" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>参加委员</th>
                                <td colspan="3">
                                    <asp:TextBox ID="txtAttendees" runat="server" TextMode="MultiLine" Rows="3" CssClass="readonly long" ToolTip="参加委员"></asp:TextBox>
                                    <a id="btnAttendees" href="#" class="btn"><u>选取</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th>活动内容</th>
                                <td colspan="3"><asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" Rows="9" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>附件</th>
                                <td colspan="3">
                                    <asp:HiddenField ID="hfFiles" runat="server" />
                                    <a id="btnFiles" href="#" class="btn"><u>上传</u></a>
                                    <div id="files"></div>
                                </td>
                            </tr>
                            <asp:PlaceHolder ID="plVerify" runat="server" Visible="false">
                                <tr>
                                    <th>退回原因</th>
                                    <td colspan="3"><asp:Literal ID="ltVerifyInfo" runat="server"></asp:Literal></td>
                                </tr>
                            </asp:PlaceHolder>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnSub" runat="server" Text="申请" OnClick="btnSub_Click" />
                        <asp:Button ID="btnSave" runat="server" Text="暂存" OnClick="btnSave_Click" />
                        <asp:Button ID="btnDel" runat="server" Text="删除" Visible="false" OnClick="btnDel_Click" />
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plMy" runat="server" Visible="false">
<script>
    $(function () {
        $('.list>table>tbody>tr>td>a').each(function () {
            if ($(this).text() == '修改') {
                var url = $(this).attr('href') + '&ac=sub';
                $(this).attr('href', url);
            }
        })
    });
</script>
                <div class="frm list hover">
                    <strong>我的会议/活动申请
                        <span>符合条件的数据有：<b><asp:Literal ID="ltMyTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="state">参加方式</th>
                                <th>类型</th>
                                <th>部门</th>
                                <th>主题</th>
                                <th class="time">起止时间</th>
                                <th>地点</th>
                                <th class="time">报名截止时间</th>
                                <th class="state">状态</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpMyList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "IsMust")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OrgName")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td align="center">
                                            <%#DataBinder.Eval(Container.DataItem, "StartTime", "{0:yyyy年M月d日 HH:mm}")%><br />
                                            <%#DataBinder.Eval(Container.DataItem, "EndTime", "{0:yyyy年M月d日 HH:mm}")%>
                                        </td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "PerformSite")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "OverTime", "{0:yyyy年M月d日 HH:mm}")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u><%#DataBinder.Eval(Container.DataItem, "StateName")%></u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltMyNo" runat="server">
                                <tr>
                                    <td class="no">暂无调研报告！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblMyNav" runat="server" CssClass="nav"></asp:Label>
                </div>

            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plSave" runat="server" Visible="false">
                <div class="frm list hover">
                    <strong>暂存的会议/活动申请</strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="state">参加方式</th>
                                <th>类型</th>
                                <th>部门</th>
                                <th>主题</th>
                                <th class="time">起止时间</th>
                                <th>地点</th>
                                <th class="time">报名截止时间</th>
                                <th class="state">状态</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpSaveList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "IsMust")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OrgName")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td align="center">
                                            <%#DataBinder.Eval(Container.DataItem, "StartTime", "{0:yyyy年M月d日 HH:mm}")%><br />
                                            <%#DataBinder.Eval(Container.DataItem, "EndTime", "{0:yyyy年M月d日 HH:mm}")%>
                                        </td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "PerformSite")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "OverTime", "{0:yyyy年M月d日 HH:mm}")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><a href="?ac=sub&id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>修改</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltSaveNo" runat="server">
                                <tr>
                                    <td class="no">暂无调研报告！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblSaveNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
