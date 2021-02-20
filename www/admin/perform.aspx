<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="perform.aspx.cs" Inherits="hkzx.web.admin.perform" %><%--Tony维护--%>
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
            if ($('#plNav').text()) {
                var url = window.location.href;
                if (url.indexOf('ac=roll') > 0) {
                    $('#plNav>a#roll').show().addClass('cur');
                } else if (url.indexOf('ac=feed') > 0) {
                    $('#plNav>a#feed').show().addClass('cur');
                } else if (url.indexOf('id=0') > 0) {
                    $('#plNav>a[href*="id=0"]').addClass('cur').removeAttr('href');
                } else if (url.indexOf('id=') > 0) {
                    $('#plNav>a#update').show().addClass('cur');
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
    <asp:HiddenField ID="hfBack" runat="server" Value="./" /><asp:HiddenField ID="hfAdminName" runat="server" />
    <asp:HiddenField ID="hfOrg" runat="server" /><asp:HiddenField ID="hfSubType" runat="server" />
    <uc1:ucHeader ID="header1" runat="server" />
    <div class="content">
        <div class="main">
            <asp:Panel ID="plNav" runat="server" Visible="false" CssClass="btn">
                <a href="?ac=">检索会议/活动</a>
                <a href="?id=0">发布会议/活动</a>
                <a href="?ac=save" class="hide">暂存</a>
                <a id="update" class="hide">更新通知</a>
                <a id="feed" class="hide">反馈情况</a>
                <a id="roll" class="hide">报名统计</a>
            </asp:Panel>

            <asp:PlaceHolder ID="plQuery" runat="server" Visible="false">
<script>
    $(function () {
        loadSelMenu('#hfOrg', '#txtQOrgName', '#QOrgName', '');
        loadSelMenu('#hfSubType', '#txtQSubType', '#QSubType', '');
        $('#btnQAttendees').click(function () {
            var title = '选取需要通知的委员';
            var obj = 'txtQAttendees';
            showDialog(title, '../cn/dialog.aspx?ac=subman&obj=' + obj, '', 640, 400, 'no');
            return false;
        });
        $('#btnQuery').click(function () {
            try {
                var url = '';
                if ($('#txtQOrgName').val()) {
                    url += '&OrgName=' + $('#txtQOrgName').val();
                }
                if ($('#txtQSubType').val()) {
                    url += '&SubType=' + $('#txtQSubType').val();
                }
                if ($('#txtQTitle').val()) {
                    url += '&Title=' + $('#txtQTitle').val();
                }
                if ($('#txtQPerformSite').val()) {
                    url += '&PerformSite=' + $('#txtQPerformSite').val();
                }
                if ($('#txtQLeaders').val()) {
                    url += '&Leaders=' + $('#txtQLeaders').val();
                }
                if ($('#txtQAttendees').val()) {
                    var tmp2 = $('#txtQAttendees').val().replace(/，/g, ',');
                    $('#txtQAttendees').val(tmp2);
                    url += '&Attendees=' + $('#txtQAttendees').val();
                }
                if ($('#ddlQHaveBus').val()) {
                    url += '&HaveBus=' + $('#ddlQHaveBus').val();
                }
                if ($('#ddlQIsMust').val()) {
                    url += '&IsMust=' + $('#ddlQIsMust').val();
                }
                if ($('#txtQOverTime1').val() || $('#txtQOverTime2').val()) {
                    url += '&OverTime=' + $('#txtQOverTime1').val() + ',' + $('#txtQOverTime2').val();
                }
                if ($('#txtQStartTime1').val() || $('#txtQStartTime2').val()) {
                    url += '&StartTime=' + $('#txtQStartTime1').val() + ',' + $('#txtQStartTime2').val();
                }
                if ($('#txtQEndTime1').val() || $('#txtQEndTime2').val()) {
                    url += '&EndTime=' + $('#txtQEndTime1').val() + ',' + $('#txtQEndTime2').val();
                }
                var tmp = getChecked('#cblQActive', ',');
                if (tmp != '') {
                    url += '&Active=' + tmp;
                }
                //alert(encodeURI(url)); return false;
                if (url != '') {
                    window.location.href = '?ac=query' + encodeURI(url);
                } else {
                    window.location.href = '?ac=query';
                }
                return false;
            } catch (err) {
                alert("验证出错，请稍后重试！");
                return false;
            }
        });
        $('#btnPass, #btnFinish, #btnBack, #btnDels').click(function () {
            if (!$('.list>table>tbody>tr>td>input:checkbox').is(':checked')) {
                alert('请先选取要操作的数据！');
                return false;
            }
            if ($(this).attr('id') == 'btnBack') {
                var txt = prompt('请填写退回原因：');
                if (txt) {
                    $('#hfVerifyInfo').val(txt);
                } else {
                    return false;
                }
            } else if ($(this).attr('id') == 'btnDels') {
                var txt = prompt('请填写删除原因：');
                if (txt) {
                    $('#hfVerifyInfo').val(txt);
                } else {
                    return false;
                }
            } else if (!confirm('您确定要“' + $(this).val() + '”吗?')) {
                return false;
            }
        });
    });
</script>
                <div class="frm edit">
                    <strong>检索会议/活动</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>申请部门</th>
                                <td>
                                    <asp:DropDownList ID="ddlQOrgType" runat="server" Visible="false" ToolTip="部门">
                                        <asp:ListItem Text="办公室"></asp:ListItem>
                                        <asp:ListItem Text="专委办"></asp:ListItem>
                                        <asp:ListItem Text="专委会"></asp:ListItem>
                                        <asp:ListItem Text="界别"></asp:ListItem>
                                        <asp:ListItem Text="街道活动组"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:TextBox ID="txtQOrgName" runat="server" CssClass="readonly"></asp:TextBox>
                                    <div id="QOrgName" class="selmenu"></div>
                                </td>
                                <th>活动类型</th>
                                <td>
                                    <asp:TextBox ID="txtQSubType" runat="server" CssClass="readonly long" ToolTip="类型"></asp:TextBox>
                                    <div id="QSubType" class="selmenu"></div>
                                </td>
                            </tr>
                            <tr>
                                <th>活动主题</th>
                                <td><asp:TextBox ID="txtQTitle" runat="server" MaxLength="50" CssClass="long" ToolTip="活动主题"></asp:TextBox></td>
                                <th>地点</th>
                                <td><asp:TextBox ID="txtQPerformSite" runat="server" MaxLength="100" CssClass="long" ToolTip="地点"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>出席领导</th>
                                <td><asp:TextBox ID="txtQLeaders" runat="server" MaxLength="20" CssClass="long"></asp:TextBox></td>
                                <th>参加委员</th>
                                <td><asp:TextBox ID="txtQAttendees" runat="server" MaxLength="20" CssClass="long"></asp:TextBox>
                                    <a id="btnQAttendees" href="#" class="btn"><u>选取</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th>用车情况</th>
                                <td>
                                    <asp:DropDownList ID="ddlQHaveBus" runat="server" ToolTip="用车情况">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th>参加方式</th>
                                <td>
                                    <asp:DropDownList ID="ddlQIsMust" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        <asp:ListItem Text="必须参加"></asp:ListItem>
                                        <asp:ListItem Text="报名参加"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>会议开始时间</th>
                                <td><asp:TextBox ID="txtQStartTime1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox> - <asp:TextBox ID="txtQStartTime2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtQStartTime1\')}'})"></asp:TextBox></td>
                                <th>会议结束时间</th>
                                <td><asp:TextBox ID="txtQEndTime1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox> - <asp:TextBox ID="txtQEndTime2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtQEndTime1\')}'})"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>报名截止时间</th>
                                <td><asp:TextBox ID="txtQOverTime1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox> - <asp:TextBox ID="txtQOverTime2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtQOverTime1\')}'})"></asp:TextBox></td>
                                <th>状态</th>
                                <td>
                                    <asp:CheckBoxList ID="cblQActive" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="发布"></asp:ListItem>
                                        <asp:ListItem Text="履职关闭"></asp:ListItem>
                                        <asp:ListItem Text="提交申请"></asp:ListItem>
                                        <asp:ListItem Text="退回"></asp:ListItem>
                                        <asp:ListItem Text="暂存"></asp:ListItem>
                                        <asp:ListItem Text="删除"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <input id="btnQuery" type="button" value="查询" />
                        <input id="clean" type="reset" value="清空" />
                    </div>
                </div>
                <div class="cmd"><asp:HiddenField ID="hfVerifyInfo" runat="server" />
                    <asp:Button ID="btnPass" runat="server" Text="发布" OnClick="btnPass_Click" />
                    <asp:Button ID="btnFinish" runat="server" Text="履职关闭" OnClick="btnFinish_Click" />
                    <asp:Button ID="btnBack" runat="server" Text="退回" OnClick="btnBack_Click" />
                    <%--<asp:Button ID="btnDels" runat="server" Text="删除" OnClick="btnDels_Click" />--%>
                </div>
                <div class="list hover">
                    <strong>结果展现
                        <span>符合条件的数据有：<b><asp:Literal ID="ltQueryTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="state">参加方式</th>
                                <th>活动类型</th>
                                <th>申请部门(单位)</th>
                                <th>会议/活动主题</th>
                                <th class="time">起止时间</th>
                                <th>地点</th>
                                <th class="time">报名截止时间</th>
                                <th class="state">状态</th>
                                <th class="state">参与人数</th>
                                <th class="cmd"><input type="checkbox" title="全选" />操作</th>
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
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "StartTime", "{0:yyyy年M月d日 HH:mm}")%><br /><%#DataBinder.Eval(Container.DataItem, "EndTime", "{0:yyyy年M月d日 HH:mm}")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "PerformSite")%></td>
                                        <td align="center" title="<%#DataBinder.Eval(Container.DataItem, "OverTime", "{0:yyyy-MM-dd HH:mm}")%>"><%#DataBinder.Eval(Container.DataItem, "OverTimeText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "FeedNum")%>/<%#DataBinder.Eval(Container.DataItem, "AttendeesNum")%></td>
                                        <td>
                                            <asp:CheckBox ID="_ck" runat="server" /><asp:HiddenField ID="_id" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "Id") %>'/>
                                            <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>修改</u></a>
                                            <a href="?ac=feed&id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>参数设置</u></a>
                                            <a href="?ac=roll&id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>报名统计</u></a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltQueryNo" runat="server">
                                <tr>
                                    <td class="no">暂未查询到履职活动！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblQueryNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plEdit" runat="server" Visible="false">
<script>
    $(function () {
        loadSelMenu('#hfOrg', '#txtOrgName', '#OrgName', '');
        loadSelMenu('#hfOrg', '#txtOrgName2', '#OrgName2', '');
        loadSelMenu('#hfSubType', '#txtSubType', '#SubType', '');
        loadSelMenu('#hfSubType', '#txtSubType2', '#SubType2', '');
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
        upFile('#hfFiles', '#files', '#btnFiles', 'file');
        $('#btnAttendees').click(function () {
            var title = '选取参加委员';
            var obj = 'txtAttendees';
            showDialog(title, '../cn/dialog.aspx?ac=subman&obj=' + obj, '', 640, 400, 'no');
            return false;
        });
        $('form').submit(function () {
            try {
                var tmp2 = $('#txtAttendees').val().replace(/，/g, ',');
                $('#txtAttendees').val(tmp2);
                if (checkEmpty('#txtOrgName') || checkEmpty('#txtSubType') || checkEmpty('#txtLinkman') || checkEmpty('#txtLinkmanTel') || checkEmpty('#txtTitle') || checkEmpty('#txtPerformSite') || checkEmpty('#txtStartTime') || checkEmpty('#txtEndTime') || checkEmpty('#txtOverTime') || checkEmpty('#txtSignTime') || checkRadio('#rblHaveBus') || checkEmpty('#txtAttendees')) {
                    return false;
                }
                //var tmp = getChecked('#cblSignDesk', ',');
                //if (tmp) {
                //    var arr = tmp.split(',');
                //    if (arr.length < 2) {
                //        alert('请至少选取2台设备！');
                //        $('#cblSignDesk>input:checkbox:first').focus();
                //        return false;
                //    }
                //}
            } catch (err) {
                alert("验证出错，请稍后重试！");
                return false;
            }
        });
    });
</script>
                <div class="frm edit">
                    <strong><asp:Literal ID="ltEditTitle" runat="server" Text="发布通知"></asp:Literal></strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>自编号</th>
                                <td><asp:TextBox ID="txtId" runat="server" ReadOnly="true" CssClass="readonly" Text="0"></asp:TextBox></td>
                                <th>参加方式</th>
                                <td><asp:TextBox ID="txtIsMust" runat="server" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>部门</th>
                                <td>
                                    <asp:DropDownList ID="ddlOrgType" runat="server" Visible="false" ToolTip="部门">
                                        <asp:ListItem Text="办公室"></asp:ListItem>
                                        <asp:ListItem Text="专委办"></asp:ListItem>
                                        <asp:ListItem Text="专委会"></asp:ListItem>
                                        <asp:ListItem Text="界别活动组"></asp:ListItem>
                                        <asp:ListItem Text="街道活动组"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:TextBox ID="txtOrgName" runat="server" CssClass="readonly"></asp:TextBox>
                                    <div id="OrgName" class="selmenu"></div>
                                </td>
                                <th>部门2</th>
                                <td>
                                    <asp:TextBox ID="txtOrgName2" runat="server" CssClass="readonly"></asp:TextBox>
                                    <div id="OrgName2" class="selmenu"></div>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>类型</th>
                                <td>
                                    <asp:DropDownList ID="ddlSubType" runat="server" Visible="false" ToolTip="类型"></asp:DropDownList>
                                    <asp:TextBox ID="txtSubType" runat="server" CssClass="readonly long" ToolTip="类型"></asp:TextBox>
                                    <div id="SubType" class="selmenu"></div>
                                </td>
                                <th>类型2</th>
                                <td>
                                    <asp:TextBox ID="txtSubType2" runat="server" CssClass="readonly long" ToolTip="类型"></asp:TextBox>
                                    <div id="SubType2" class="selmenu"></div>
                                </td>
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
                                <th><asp:Literal ID="ltConfirm" runat="server" Visible="false">需要重新确认</asp:Literal></th>
                                <td>
                                    <asp:RadioButtonList ID="rblConfirm" runat="server" Visible="false" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="是" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="否"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>会议结束时间</th>
                                <td><asp:TextBox ID="txtEndTime" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm',minDate:'#F{$dp.$D(\'txtStartTime\')}'})" ToolTip="结束时间"></asp:TextBox></td>
                                <th rowspan="3">签到设备</th>
                                <td rowspan="3">
                                    <asp:CheckBoxList ID="cblSignDesk" runat="server" RepeatDirection="Horizontal" RepeatColumns="6"></asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>报名截止时间</th>
                                <td><asp:TextBox ID="txtOverTime" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm',maxDate:'#F{$dp.$D(\'txtStartTime\')}'})" ToolTip="报名截止时间"></asp:TextBox></td>
                            </tr>
                            <tr>
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
                            <tr>
                                <th>备注</th>
                                <td colspan="3"><asp:TextBox ID="txtRemark" runat="server" TextMode="MultiLine" Rows="3" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加时间</th>
                                <td><asp:TextBox ID="txtAddTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改时间</th>
                                <td><asp:TextBox ID="txtUpTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加IP</th>
                                <td><asp:TextBox ID="txtAddIp" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改IP</th>
                                <td><asp:TextBox ID="txtUpIp" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加人</th>
                                <td><asp:TextBox ID="txtAddUser" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改人</th>
                                <td><asp:TextBox ID="txtUpUser" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnEdit" runat="server" Text="发布" OnClick="btnEdit_Click" />
                        <asp:Button ID="btnSave" runat="server" Text="暂存" OnClick="btnSave_Click" />
                        <asp:Button ID="btnDel" runat="server" Text="删除" Visible="false" OnClick="btnDel_Click" />
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plFeed" runat="server" Visible="false">
<script>
    $(function () {
        $('#btnFromMan').click(function () {
            var title = '选取签到委员';
            var obj = 'txtFeedSignMan';
            showDialog(title, '../cn/dialog.aspx?ac=subman&obj=' + obj, '', 640, 400, 'no');
            return false;
        });
        $('#btnFeed').click(function () {
            try {
                var url = '';
                if ($('#txtFeedSignMan').val()) {
                    var tmp2 = $('#txtFeedSignMan').val().replace(/，/g, ',');
                    $('#txtFeedSignMan').val(tmp2);
                    url += '&SignMan=' + $('#txtFeedSignMan').val();
                }
                if ($('#ddlFeedActiveName').val()) {
                    url += '&ActiveName=' + $('#ddlFeedActiveName').val();
                }
                //alert(encodeURI(url)); return false;
                window.location.href = '?ac=feed&id=' + <%=Request.QueryString["id"]%> +encodeURI(url);
                return false;
            } catch (err) {
                alert("验证出错，请稍后重试！");
                return false;
            }
        });
        if ($('.list>table>tbody>tr>td').not('.list>table>tbody>tr>td.no').text()) {
            //$('#lnkDownSign').attr('href', window.location.href + '&down=sign');
            $('#lnkDownXls').attr('href', window.location.href + '&down=xls');
        } else {
            //$('#lnkDownSign').hide();
            $('#lnkDownXls').hide();
        }
        $('div.sms>div>.title>a').click(function () {
            $('div.sms').hide();
            return false;
        });
        $('#txtSmsBody').blur(function () {
            var sign = $(this).attr('title');
            var txt = $(this).val();
            if (txt) {
                var reg = new RegExp(sign, 'gi');///(【虹口政协】)+/
                txt = txt.replace(reg, '');
                txt = txt.replace(/\[|\]|{|}|【|】/g, '').replace(/^\s+|\s+$/g, '');;
            }
            txt = sign + txt;
            $(this).val(txt);
        }).blur();
        var strSms = $('#txtSmsBody').val();
        $('#btnReSms').click(function () {
            $('#txtSmsBody').val(strSms);
        });
        $('#btnSms, #btnSmsMsg, #btnSmsTp, #btnWxMsg, #btnLeave, #btnDisLeave, #btnCancel, #btnSignManTypes, #btnSignManSpeaks, #btnSignManProvides, #btnActiveNames, #btnClearScores').click(function () {
            if (!$('.list>table>tbody>tr>td>input:checkbox').is(':checked')) {
                alert('请先选取要操作的人员！');
                return false;
            }
            switch ($(this).attr('id')) {
                case 'btnSms':
                    $('div.sms').show();
                    return false;
                    break;
                case 'btnSmsMsg':
                    $('#txtSmsBody').blur();
                    var txt = $('#txtSmsBody').val();
                    if (txt == '' || txt == $('#txtSmsBody').attr('title')) {
                        alert('请填写自定义短信内容！');
                        $('#txtSmsBody').focus();
                        return false;
                    }
                    break;
                case 'btnDisLeave':
                    var txt = prompt('请填写原因：');
                    if (txt) {
                        $('#hfVerifyInfoFeed').val(txt);
                    } else {
                        return false;
                    }
                    break;
                case 'btnSignManTypes':
                    if (!confirm('您确定要“' + $(this).val() + $('#ddlSignManType').val() + '”吗?')) {
                        return false;
                    }
                    break;
                case 'btnSignManSpeaks':
                    if (!confirm('您确定要“' + $(this).val() + '《' + $('#ddlSignManSpeak').val() + '》”吗?')) {
                        return false;
                    }
                    break;
                case 'btnSignManProvides':
                    if (!confirm('您确定要“' + $(this).val() + $('#ddlSignManProvide').val() + '”吗?')) {
                        return false;
                    }
                    break;
                case 'btnActiveNames':
                    if ($('#ddlActiveName').val() == '' || !confirm('您确定要“' + $(this).val() + '《' + $('#ddlActiveName').val() + '》”吗?')) {
                        return false;
                    } else if ($('#ddlActiveName').val() == '不同意请假') {
                        var txt = prompt('请填写原因：');
                        if (txt) {
                            $('#hfVerifyInfoFeed').val(txt);
                        } else {
                            return false;
                        }
                    }
                    break;
                default:
                    if (!confirm('您确定要“' + $(this).val() + '”吗?')) {
                        return false;
                    }
                    break;
            }
        });
        $('.list>table>tbody>tr>td>a').each(function () {
            var url = $(this).attr('href');
            if (url.indexOf('/cn/') < 0) {
                url = '../cn/' + url;
            }
            $(this).click(function () {
                showDialog('已发送消息', url, '', 800, 400, 'yes');
                return false;
            });
        });
    });
</script>
                <div class="frm edit">
                    <strong>履职情况查询</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>活动主题</th>
                                <td><asp:TextBox ID="txtPerformTitle" runat="server" MaxLength="50" ReadOnly="true" CssClass="readonly long"></asp:TextBox></td>
                                <th>活动类型</th>
                                <td><asp:TextBox ID="txtPerformSubType" runat="server" MaxLength="20" ReadOnly="true" CssClass="readonly long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>履职委员</th>
                                <td><asp:TextBox ID="txtFeedSignMan" runat="server" CssClass="readonly long"></asp:TextBox>
                                    <a id="btnFromMan" href="#" class="btn"><u>选取</u></a>
                                </td>
                                <th>状态</th>
                                <td>
                                    <asp:DropDownList ID="ddlFeedActiveName" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        <asp:ListItem Text="已签到"></asp:ListItem>
                                        <asp:ListItem Text="已签到(未报名)"></asp:ListItem>
                                        <asp:ListItem Text="待确认"></asp:ListItem>
                                        <asp:ListItem Text="参加"></asp:ListItem>
                                        <asp:ListItem Text="不参加"></asp:ListItem>
                                        <asp:ListItem Text="请假申请"></asp:ListItem>
                                        <asp:ListItem Text="同意请假"></asp:ListItem>
                                        <asp:ListItem Text="不同意请假"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <input id="btnFeed" type="button" value="查询" />
                        <input type="reset" value="清空" />
                    </div>
                </div>
                <asp:PlaceHolder ID="plFeedCmd" runat="server" Visible="false">
                    <div class="cmd cmd2">
                        <asp:Button ID="btnSignManTypes" runat="server" Text="设置人员类型为" OnClick="btnSignManTypes_Click" />
                        <asp:DropDownList ID="ddlSignManType" runat="server">
                            <asp:ListItem></asp:ListItem>
                            <asp:ListItem Text="常委"></asp:ListItem>
                            <%--<asp:ListItem Text="调研课题执笔人" Value="执笔人"></asp:ListItem>--%>
                            <asp:ListItem Text="主讲人"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="btnSignManSpeaks" runat="server" Text="设置会议发言为" OnClick="btnSignManSpeaks_Click" />
                        <asp:DropDownList ID="ddlSignManSpeak" runat="server">
                            <asp:ListItem></asp:ListItem>
                            <asp:ListItem Text="全会大会发言-上台交流"></asp:ListItem>
                            <asp:ListItem Text="全会大会发言-书面交流"></asp:ListItem>
                            <asp:ListItem Text="全会专题会发言"></asp:ListItem>
                            <asp:ListItem Text="其他会议发言"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="btnSignManProvides" runat="server" Text="设置提供资源得" OnClick="btnSignManProvides_Click" />
                        <asp:DropDownList ID="ddlSignManProvide" runat="server">
                            <asp:ListItem></asp:ListItem>
                            <asp:ListItem Text="1分"></asp:ListItem>
                            <asp:ListItem Text="2分"></asp:ListItem>
                            <asp:ListItem Text="3分"></asp:ListItem>
                            <asp:ListItem Text="4分"></asp:ListItem>
                            <asp:ListItem Text="5分"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="btnActiveNames" runat="server" Text="设置状态为" OnClick="btnActiveNames_Click" />
                        <asp:DropDownList ID="ddlActiveName" runat="server">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="btnClearScores" runat="server" Text="清除会议积分" OnClick="btnClearScores_Click" />
                    </div>
                    <div id="dialog" class="sms">
                        <div style="width:640px;height:300px;margin-left:-320px;margin-top:-150px;">
                            <div class="title">发送自定义短信<a href="#">X</a></div>
                            <div class="body">
                                <asp:TextBox ID="txtSmsBody" runat="server" TextMode="MultiLine" Rows="8" ToolTip="【虹口政协】"></asp:TextBox>
                                <div class="cmd">
                                    <div>
                                        定时发送：<asp:TextBox ID="txtSmsTime" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss',minDate:'%y-%M-%d %H:{%m+30}:%s'})" ToolTip="定时发送"></asp:TextBox>
                                    </div>
                                    <asp:Button ID="btnSmsMsg" runat="server" Text="确定提交" OnClick="btnSmsMsg_Click" />
                                    <input id="btnReSms" type="button" value="重置" /><br />
                                    <b>*</b><span>自定义短信提交后，需要电信公司审核通过后发送，发送时间会有延迟！</span><br />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="cmd">
                        <asp:PlaceHolder ID="plSms" runat="server" Visible="false">
                            <asp:Button ID="btnSmsTest" runat="server" Text="发送短信测试" OnClick="btnSmsTest_Click" />
                            <asp:Button ID="btnSmsHealth" runat="server" Text="平台监控" OnClick="btnSmsHealth_Click" />
                            <asp:Button ID="btnSmsBalance" runat="server" Text="短信余额" OnClick="btnSmsBalance_Click" />
                        </asp:PlaceHolder>
                        <input ID="btnSms" type="button" value="自定义短信" />
                        <asp:Button ID="btnSmsTp" runat="server" Text="发送短信" OnClick="btnSmsTp_Click" />
                        <asp:Button ID="btnWxMsg" runat="server" Text="发送微信消息" OnClick="btnWxMsg_Click" />
                        <a id="lnkDownXls" href="#" target="_blank"><u>下载xls名单</u></a><asp:HiddenField ID="hfVerifyInfoFeed" runat="server" />
                        <%--<a id="lnkDownSign" href="#" target="_blank"><u>下载签到数据包</u></a>
                        <asp:Button ID="btnSignManType" runat="server" Text="设置主讲人" />
                        <asp:Button ID="btnSignManSpeak" runat="server" Text="设置会议发言" />
                        <asp:Button ID="btnLeave" runat="server" Text="同意请假" OnClick="btnLeave_Click" />
                        <asp:Button ID="btnDisLeave" runat="server" Text="不同意请假" OnClick="btnDisLeave_Click" />--%>
                        <asp:Button ID="btnCancel" runat="server" Text="取消" OnClick="btnCancel_Click" />
                    </div>
                </asp:PlaceHolder>
                <div class="list hover">
                    <strong>《<asp:Literal ID="ltPerformTitle" runat="server"></asp:Literal>》履职情况
                        <span>符合条件的数据有：<b><asp:Literal ID="ltFeedTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="state">状态</th>
                                <th class="state">姓名</th>
                                <th>工作单位及职务</th>
                                <th class="state">人员类型</th>
                                <th class="state">会议发言</th>
                                <th class="state">提供资源</th>
                                <th class="time">签到时间</th>
                                <th>请假原因/审批意见</th>
                                <th class="time">反馈时间</th>
                                <th class="time">审核时间</th>
                                <th class="state">已发消息</th>
                                <th class="state"><input type="checkbox" title="全选" />操作</th>
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
                                        <td>
                                            <%#DataBinder.Eval(Container.DataItem, "LeaveReason")%><br />
                                            <b><%#DataBinder.Eval(Container.DataItem, "VerifyInfo")%></b>
                                        </td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "FeedTimeText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "VerifyTimeText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SendMsg")%></td>
                                        <td align="center">
                                            <asp:CheckBox ID="_ck" runat="server" /><asp:HiddenField ID="_id" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "Id") %>'/>
                                        </td>
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
                    <asp:Label ID="lblFeedNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plRoll" runat="server" Visible="false">
<script>
    $(function () {
        if ($('.list>table>tbody>tr>td').not('.list>table>tbody>tr>td.no').text()) {
            $('#lnkDownXls2').attr('href', window.location.href + '&down=xls');
        } else {
            $('#lnkDownXls2').hide();
        }
    });
</script>
                <div class=" frm list hover">
                    <strong><b>活动类型：</b><asp:Literal ID="ltRollType" runat="server"></asp:Literal>
                        <span><a id="lnkDownXls2" href="#" target="_blank"><u>下载xls名单</u></a></span>
                    </strong>
                    <strong><b>活动名称：</b><asp:Literal ID="ltRollTitle" runat="server"></asp:Literal>
                        <span>共有<b><asp:Literal ID="ltRollTotal" runat="server" Text="0"></asp:Literal></b>人</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="state">委员编号</th>
                                <th class="state">反馈状态</th>
                                <th class="state">委员姓名</th>
                                <th class="num">性别</th>
                                <th class="date">出生日期</th>
                                <th class="state">政治面貌</th>
                                <th class="state">界别</th>
                                <th class="state">是否特邀<br />监督员</th>
                                <th class="state">体制内外</th>
                                <th class="state">单位性质</th>
                                <th>工作单位及职务</th>
                                <th>单位地址</th>
                                <th>家庭地址</th>
                                <th class="date">联系方式</th>
                                <th class="state">是否出席</th>
                                <th class="state">是否发言</th>
                                <th class="state">是否提供资源</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpRollList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th align="center"><%#DataBinder.Eval(Container.DataItem, "UserCode")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "PerformFeedActive")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "TrueName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "UserSex")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "BirthdayText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Party")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Subsector")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "IsInvited")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "IsSystem")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "OrgType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OrgName")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OrgAddress")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "HomeAddress")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Mobile")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "IsPresent") %></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "IsSpeak") %></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "IsProvide") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltRollNo" runat="server">
                                <tr>
                                    <td class="no">暂无委员参与！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblRollNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
