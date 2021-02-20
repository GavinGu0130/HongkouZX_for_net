<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="opinion_pop.aspx.cs" Inherits="hkzx.web.admin.opinion_pop" %><%--Tony维护--%>
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
                if ($('#plNav>a#lnkEdit').text()) {
                    $('#plNav>a#lnkEdit').show();
                }
                var url = window.location.href;
                if (url.indexOf('id=0') > 0) {
                    $('#plNav>a#lnkEdit').addClass('cur');
                } else if (url.indexOf('id=') > 0) {
                    $('#plNav>a#edit').show().addClass('cur');
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
            //$('#btnOutput').click(function () {
            //    alert('建设中。。。');
            //    return false;
            //});
        });
    </script>
</head>
<body>
<form id="form1" runat="server">
    <asp:Panel ID="plOutput" runat="server" Visible="false" CssClass="dialog">
        <script>
            $(function () {
                var ac = $('form').attr('action');
                $('form').attr('action', unescape(ac));
            });
        </script>
        <div class="out">
            <asp:CheckBoxList ID="cblOutput" runat="server" RepeatDirection="Horizontal" RepeatColumns="6">
                <asp:ListItem Text="流水号" Value="Id" Selected="True"></asp:ListItem>
                <asp:ListItem Text="反映人" Value="SubMan" Selected="True"></asp:ListItem>
                <asp:ListItem Text="信息类别" Value="SubType" Selected="True"></asp:ListItem>
                <asp:ListItem Text="标题" Value="Summary" Selected="True"></asp:ListItem>
                <asp:ListItem Text="提交时间" Value="SubTime" Selected="True"></asp:ListItem>
                <asp:ListItem Text="工作单位与职位" Value="LinkmanOrgName"></asp:ListItem>
                <asp:ListItem Text="联系方式" Value="LinkmanTel"></asp:ListItem>
                <asp:ListItem Text="委员类型" Value="LinkmanInfo"></asp:ListItem>
                <asp:ListItem Text="委员证号" Value="UserCode"></asp:ListItem>
                <asp:ListItem Text="来稿单位" Value="OrgName"></asp:ListItem>
                <asp:ListItem Text="签发人" Value="VerifyUser"></asp:ListItem>
                <asp:ListItem Text="编辑" Value="UpTime"></asp:ListItem>
                <asp:ListItem Text="联名委员" Value="SubMans"></asp:ListItem>
                <asp:ListItem Text="是否公开" Value="IsOpen"></asp:ListItem>
                <asp:ListItem Text="所属党派" Value="LinkmanParty"></asp:ListItem>
                <asp:ListItem Text="界别" Value="Subsector"></asp:ListItem>
                <asp:ListItem Text="专委会" Value="Committee"></asp:ListItem>
                <asp:ListItem Text="内容" Value="Body"></asp:ListItem>
                <asp:ListItem Text="反馈结果" Value="ActiveName"></asp:ListItem>
                <asp:ListItem Text="备注" Value="Remark"></asp:ListItem>
            </asp:CheckBoxList>
            <div class="cmd"><asp:Button ID="btnOutput" runat="server" Text="导出" OnClick="btnOutput_Click" /></div>
        </div>
    </asp:Panel>
    <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    <asp:HiddenField ID="hfBack" runat="server" Value="./" /><asp:HiddenField ID="hfOrg" runat="server" />
    <uc1:ucHeader ID="header1" runat="server" />
    <asp:Panel ID="plMain" runat="server" CssClass="content">
        <div class="main">
            <asp:Panel ID="plNav" runat="server" Visible="false" CssClass="btn">
                <a href="?ac=">检索社情民意</a>
                <asp:HyperLink ID="lnkEdit" runat="server" Visible="true" NavigateUrl="?id=0" CssClass="hide">新增社情民意</asp:HyperLink>
                <a id="edit" class="hide">审核社情民意</a>
            </asp:Panel>

            <asp:PlaceHolder ID="plQuery" runat="server" Visible="false">
<script>
    $(function () {
        $('#btnQSubMan, #btnQSubMans').click(function () {
            var title = '';
            var obj = '';
            switch ($(this).attr('id')) {
                case 'btnQSubMan':
                    title = '选取反映人';
                    obj = 'txtQSubMan';
                    break;
                default:
                    title = '选取联名人';
                    obj = 'txtQSubMans';
                    break;
            }
            showDialog(title, '../cn/dialog.aspx?ac=subman&type=all&obj=' + obj, '', 640, 400, 'no');
            return false;
        });
        $('#txtQSubMans').focus(function () {
            $('#' + $(this).attr('id').replace('txt', 'btn')).click();
        });
        function getQuery() {
            var url = '';
            var tmp = getChecked('#cblQActiveName', ',');
            if (tmp) {
                url += '&ActiveName=' + tmp;
            }
            if ($('#ddlQSubType').val()) {
                url += '&SubType=' + $('#ddlQSubType').val();
            }
            if ($('#txtQSubMan').val()) {
                url += '&SubMan=' + $('#txtQSubMan').val();
            }
            if ($('#txtQSubMans').val()) {
                url += '&SubMans=' + $('#txtQSubMans').val();
            }
            var tmp = getChecked('#cblQLinkmanInfo', ',');
            if (tmp) {
                url += '&LinkmanInfo=' + tmp;
            }
            if ($('#txtQLinkmanOrgName').val()) {
                url += '&LinkmanOrgName=' + $('#txtQLinkmanOrgName').val();
            }
            if ($('#txtQLinkmanTel').val()) {
                url += '&LinkmanTel=' + $('#txtQLinkmanTel').val();
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
            if ($('#txtQSubTime1').val() || $('#txtQSubTime2').val()) {
                url += '&SubTime=' + $('#txtQSubTime1').val() + ',' + $('#txtQSubTime2').val();
            }
            if ($('#txtQSummary').val()) {
                url += '&Summary=' + $('#txtQSummary').val();
            }
            if ($('#txtQBody').val()) {
                url += '&Body=' + $('#txtQBody').val();
            }
            if ($('#txtQAdvise').val()) {
                url += '&Advise=' + $('#txtQAdvise').val();
            }
            if ($('#ddlQIsOpen').val()) {
                url += '&IsOpen=' + $('#ddlQIsOpen').val();
            }
            url += '&Order=' + $('#ddlQOrderBy').val() + '&By=' + $('#rblQOrderBy>input:radio:checked').val();
            return url;
        }
        $('#btnOut').click(function () {
            var url = window.location.href;
            if (url.indexOf('?') > 0) {
                url = url.substring(0, url.indexOf('?'));
            }
            url += '?ac=output' + getQuery();
            showDialog('社情民意 导出选项', url, '', 640, 300, 'no');
        });
        $('form').submit(function () {
            try {
                var tmp = getChecked('#cblQFields', ',');
                if (!tmp) {
                    alert('请选择[查询信息]项');
                    $('#cblQFields>input:checkbox:first').focus();
                    return false;
                }
                var url = getQuery();
                url += '&Fields=' + tmp;
                //alert(encodeURI(url)); return false;
                window.location.href = '?ac=query' + encodeURI(url);
            } catch (err) {
                //alert("验证出错，请稍后重试！");
            }
            return false;
        });
    });
</script>
                <div class="frm edit">
                    <strong>检索社情民意</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>反映单位/反映人</th>
                                <td><asp:TextBox ID="txtQSubMan" runat="server" CssClass="long"></asp:TextBox>
                                    <a id="btnQSubMan" href="#" class="btn"><u>选取</u></a>
                                </td>
                                <th>录用情况</th>
                                <td colspan="3">
                                    <asp:CheckBoxList ID="cblQActiveName" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th>第一反映人身份</th>
                                <td>
                                    <asp:CheckBoxList ID="cblQLinkmanInfo" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
                                </td>
                                <th>联名反映人</th>
                                <td><asp:TextBox ID="txtQSubMans" runat="server" CssClass="readonly long"></asp:TextBox>
                                    <a id="btnQSubMans" href="#" class="btn"><u>选取</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th>工作单位与职务</th>
                                <td><asp:TextBox ID="txtQLinkmanOrgName" runat="server" CssClass="long"></asp:TextBox></td>
                                <th>联系方式</th>
                                <td><asp:TextBox ID="txtQLinkmanTel" runat="server" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>专委会</th>
                                <td>
                                    <asp:DropDownList ID="ddlQCommittee" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th>界别</th>
                                <td>
                                    <asp:DropDownList ID="ddlQSubsector" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>街道活动组</th>
                                <td>
                                    <asp:DropDownList ID="ddlQStreetTeam" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th>政治面貌</th>
                                <td>
                                    <asp:DropDownList ID="ddlQParty" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>社情民意分类</th>
                                <td>
                                    <asp:DropDownList ID="ddlQSubType" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th>提交日期</th>
                                <td><asp:TextBox ID="txtQSubTime1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox> - <asp:TextBox ID="txtQSubTime2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtQSubTime1\')}'})"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>信息标题</th>
                                <td colspan="3"><asp:TextBox ID="txtQSummary" runat="server" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>情况反映</th>
                                <td colspan="3"><asp:TextBox ID="txtQBody" runat="server" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>意见建议</th>
                                <td colspan="3"><asp:TextBox ID="txtQAdvise" runat="server" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>信息是否公开</th>
                                <td>
                                    <asp:DropDownList ID="ddlQIsOpen" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        <asp:ListItem Text="是"></asp:ListItem>
                                        <asp:ListItem Text="否"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th>排序字段</th>
                                <td>
                                    <asp:DropDownList ID="ddlQOrderBy" runat="server"></asp:DropDownList>
                                    <asp:RadioButtonList ID="rblQOrderBy" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="升序" Value="ASC"></asp:ListItem>
                                        <asp:ListItem Text="降序" Value="DESC" Selected="True"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <th>选择查询信息</th>
                                <td colspan="3">
                                    <asp:CheckBoxList ID="cblQFields" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <input id="btnQuery" type="submit" value="查询" />
                        <%--<input id="clean" type="reset" value="清空" />--%>
                        <input id="btnOut" type="button" value="导出" />
                    </div>
                </div>
                <div class="list">
                    <strong>结果展现
                        <span>符合条件的数据有：<b><asp:Literal ID="ltQueryTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                            <asp:Literal ID="ltQueryThead" runat="server">
                                <th class="num">序号</th>
                                <th class="state">类别</th>
                                <th>标题</th>
                                <th class="state">录用情况</th>
                            </asp:Literal>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpQueryTbody" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <%#DataBinder.Eval(Container.DataItem, "tbody")%>
                                        <td>
                                            <%--<asp:CheckBox ID="_ck" runat="server" /><asp:HiddenField ID="_id" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "Id") %>'/>--%>
                                            <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>选取</u></a>
                                            <%--<a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>&ac=verify" class="btn" target="_blank"><u>核稿单</u></a>--%>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Repeater ID="rpQueryList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Summary")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td><a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>选取</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltQueryNo" runat="server">
                                <tr>
                                    <td class="no">暂无社情民意！</td>
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
        if ($('#txtSubTime').attr('readonly')) {
            $('#txtSubTime').removeAttr('onfocus');
        }
        upFile('#hfFiles', '#files', '#btnFiles', 'file');
        //$('#ddlActive').change(function () {
        //    if ($(this).val() == '已录用') {
        //        $('.employ1').show();
        //        $('.verify').show();
        //    } else {
        //        $('.employ1').hide();
        //        $('.verify').hide();
        //    }
        //}).change();
        //$('#rblGive1').click(function () {
        //    if ($(this).find('input:radio:checked').val() && $(this).find('input:radio:checked').val().indexOf('市政协') >= 0) {
        //        $('.employ2').show();
        //    } else {
        //        $('.employ2').hide();
        //    }
        //}).click();
        //$('#cblSend2').click(function () {
        //    if ($(this).find('input:checkbox:checked').val()) {
        //        $('.employ3').show();
        //    } else {
        //        $('.employ3').hide();
        //    }
        //}).click();
        $('#rblSubManType>input:radio').change(function () {
            if ($(this).is(':checked')) {
                switch ($(this).val()) {
                    case '委员':
                        $('.edit>table>tbody>tr.man').show();
                        $('.edit>table>tbody>tr.team').hide();
                        break;
                    default:
                        $('.edit>table>tbody>tr.man').hide();
                        $('.edit>table>tbody>tr.team').show();
                        break;
                }
            }
        }).change();
        $('#rblIsOpen>input:radio').change(function () {
            if ($(this).is(':checked')) {
                if ($(this).val() == '否') {
                    $('#txtOpenInfo').show();
                } else {
                    $('#txtOpenInfo').hide();
                }
            }
        }).change();
        $('#btnSubMan, #btnSubMans, #btnLinkman').click(function () {
            var title = '';
            var obj = '';
            switch ($(this).attr('id')) {
                case 'btnSubMan':
                    title = '选取反映人';
                    obj = 'txtSubMan';
                    break;
                case 'btnLinkman':
                    title = '选取反映人';
                    obj = 'txtLinkman';
                    break;
                default:
                    title = '选取联名人';
                    obj = 'txtSubMans';
                    break;
            }
            showDialog(title, '../cn/dialog.aspx?ac=subman&type=all&obj=' + obj, '', 640, 400, 'no');
            return false;
        });
        $('#txtSubMan, #txtSubMans').focus(function () {
            $('#' + $(this).attr('id').replace('txt', 'btn')).click();
        }).change(function () {
            switch ($(this).attr('id')) {
                case 'txtSubMans':
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
                default:
                    break;
            }
        });
        $('#btnBack').click(function () {
            if (!confirm('您确定要“退回”操作吗?')) {
                return false;
            }
            $('#ddlActive').val('退回')
        });
        $('form').submit(function () {
            try {
                if ($('#ddlActive').val() != '退回') {
                    if ($('#rblSubManType>input:radio:checked').val() == '委员') {
                        if (checkEmpty('#txtSubMan')) {
                            return false;
                        }
                    } else {
                        if (checkEmpty('#txtSubOrg') || checkEmpty('#txtLinkman')) {
                            return false;
                        }
                        if ($('#txtId').val() != '0' && (checkCheck('#cblLinkmanInfo') || checkCheck('#cblLinkmanParty') || checkEmpty('#txtLinkmanOrgName') || checkEmpty('#txtLinkmanTel'))) {
                            return false;
                        }
                    }
                    if (checkRadio('#rblSubType') || checkEmpty('#txtSummary') || checkEmpty('#txtBody') || checkRadio('#rblIsOpen') || ($('#rblIsOpen>input:radio:checked').val() == '否' && checkEmpty('#txtOpenInfo'))) {
                        return false;
                    }
                    if ($('#ddlActive').val() == '已录用') {
                        if (checkRadio('#rblAdopt1') || ($('#rblAdopt1>input:radio:checked').val() != '单篇采用' && checkEmpty('#txtAdopt1')) || checkCheck('#cblGive1') || ($('#cblGive1>input:checkbox:last').is(':checked') && checkEmpty('#txtGive1'))) {
                            return false;
                        }
                        //if ($('#txtId').val() != '0' && (checkEmpty('#txtVerifyTitle') || checkEmpty('#txtVerifyBody') || checkEmpty('#txtVerifyAdvise'))) {
                        //    return false;
                        //}
                    }
                }
            } catch (err) {
                //alert("验证出错，请稍后重试！");
                return false;
            }
        });
    });
</script>
                <div class="frm edit">
                    <strong>社情民意信息</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>自编号</th>
                                <td><asp:TextBox ID="txtId" runat="server" ReadOnly="true" CssClass="readonly" Text="0"></asp:TextBox>
                                    <asp:TextBox ID="txtOpNum" runat="server" CssClass="readonly" Text=""></asp:TextBox>
                                </td>
                                <th>提交时间</th>
                                <td><asp:TextBox ID="txtSubTime" runat="server" CssClass="readonly Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>信息分类</th>
                                <td colspan="3">
                                    <asp:RadioButtonList ID="rblSubType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="类别"></asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>提交者性质</th>
                                <td>
                                    <asp:RadioButtonList ID="rblSubManType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="委员" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="党派团体"></asp:ListItem>
                                        <asp:ListItem Text="专委会"></asp:ListItem>
                                        <asp:ListItem Text="界别"></asp:ListItem>
                                        <asp:ListItem Text="其他"></asp:ListItem>
                                    </asp:RadioButtonList>
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
                            <tr class="man">
                                <th><b>*</b>反映人</th>
                                <td id="SubMan" colspan="3"><asp:TextBox ID="txtSubMan" runat="server" MaxLength="20" ToolTip="反映人" CssClass="readonly"></asp:TextBox>
                                    <a id="btnSubMan" href="#" class="btn"><u>选取</u></a>
                                </td>
                            </tr>
                            <tr class="team">
                                <th><b>*</b>反映单位</th>
                                <td><asp:TextBox ID="txtSubOrg" runat="server" MaxLength="20" ToolTip="反映单位" CssClass="long"></asp:TextBox></td>
                                <th><b>*</b>反映人</th>
                                <td><asp:TextBox ID="txtLinkman" runat="server" MaxLength="20" ToolTip="反映人"></asp:TextBox>
                                    <a id="btnLinkman" href="#" class="btn"><u>选取</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>第一反映人身份</th>
                                <td colspan="3">
                                    <asp:CheckBoxList ID="cblLinkmanInfo" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="第一反映人身份"></asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>政治面貌</th>
                                <td colspan="3">
                                    <asp:CheckBoxList ID="cblLinkmanParty" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="政治面貌"></asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>工作单位与职务</th>
                                <td><asp:TextBox ID="txtLinkmanOrgName" runat="server" MaxLength="100" CssClass="long" ToolTip="工作单位与职务"></asp:TextBox></td>
                                <th><b>*</b>联系方式</th>
                                <td><asp:TextBox ID="txtLinkmanTel" runat="server" MaxLength="50" ToolTip="联系方式"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>联名反映人</th>
                                <td colspan="3"><asp:TextBox ID="txtSubMans" runat="server" CssClass="readonly long"></asp:TextBox>
                                    <a id="btnSubMans" href="#" class="btn"><u>选取</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th>非委员反映人</th>
                                <td colspan="3"><asp:TextBox ID="txtSubMan2" runat="server" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>信息标题</th>
                                <td colspan="3"><asp:TextBox ID="txtSummary" runat="server" CssClass="long" MaxLength="100" ToolTip="信息标题"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>情况反映</th>
                                <td colspan="3"><asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" CssClass="long" Rows="8" ToolTip="情况反映"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>意见建议</th>
                                <td colspan="3"><asp:TextBox ID="txtAdvise" runat="server" TextMode="MultiLine" Rows="8" CssClass="long"></asp:TextBox></td>
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
                                <th><b>*</b>录用情况</th>
                                <td>
                                    <asp:DropDownList ID="ddlActive" runat="server" ToolTip="录用情况"></asp:DropDownList>
                                </td>
                                <th>是否优秀社情民意</th>
                                <td>
                                    <asp:CheckBoxList ID="cblIsGood" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="是"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr class="employ1">
                                <th>采用</th>
                                <td colspan="3">
                                    <asp:RadioButtonList ID="rblAdopt1" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" ToolTip="采用">
                                        <asp:ListItem Text="单篇" Value="单篇采用"></asp:ListItem>
                                        <asp:ListItem Text="综合" Value="综合采用"></asp:ListItem>
                                    </asp:RadioButtonList>，原始篇目：
                                    (1)<asp:TextBox ID="txtAdopt1" runat="server" MaxLength="100" ToolTip="采用"></asp:TextBox>
                                    (2)<asp:TextBox ID="txtAdopt1_2" runat="server" MaxLength="100"></asp:TextBox>
                                </td>
                            </tr>
                            <tr class="employ1">
                                <th>主送</th>
                                <td>
                                    <asp:CheckBoxList ID="cblGive1" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" ToolTip="主送">
                                        <asp:ListItem Text="市政协综合信息处" Value="报送市政协"></asp:ListItem>
                                        <asp:ListItem Text="区委、区政府、区政协领导，区委办、区政府办及有关部门" Value="报送区领导及有关部门"></asp:ListItem>
                                        <asp:ListItem Text="区"></asp:ListItem>
                                    </asp:CheckBoxList>
                                    <asp:TextBox ID="txtGive1" runat="server" ToolTip="主送"></asp:TextBox> (委/办/局)
                                </td>
                                <th rowspan="2">区领导批示</th>
                                <td rowspan="2"><asp:TextBox ID="txtReply1" runat="server" TextMode="MultiLine" Rows="6" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr class="employ1">
                                <th>区采用情况</th>
                                <td>
                                    <asp:CheckBoxList ID="cblEmploy1" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" ToolTip="区采用情况">
                                        <asp:ListItem Text="得到区有关部门采用"></asp:ListItem>
                                        <asp:ListItem Text="得到区领导批示"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr class="employ2">
                                <th>市政协</th>
                                <td>
                                    <asp:RadioButtonList ID="rblAdopt2" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="采用">
                                        <asp:ListItem Text="单篇"></asp:ListItem>
                                        <asp:ListItem Text="综合"></asp:ListItem>
                                    </asp:RadioButtonList><br />
                                    <asp:CheckBoxList ID="cblGive2" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" ToolTip="报送">
                                        <asp:ListItem Text="转送市有关部门"></asp:ListItem>
                                        <asp:ListItem Text="报送市领导"></asp:ListItem>
                                        <asp:ListItem Text="报送全国政协"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                                <th rowspan="2">市领导批示</th>
                                <td rowspan="2"><asp:TextBox ID="txtReply2" runat="server" TextMode="MultiLine" Rows="5" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr class="employ2">
                                <th>市政协采用情况</th>
                                <td>
                                    <asp:CheckBoxList ID="cblEmploy2" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" ToolTip="采用">
                                        <asp:ListItem Text="得到市有关部门采用"></asp:ListItem>
                                        <asp:ListItem Text="得到市领导批示"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr class="employ3">
                                <th>全国政协</th>
                                <td>
                                    <asp:CheckBoxList ID="cblGive3" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" ToolTip="报送">
                                        <asp:ListItem Text="全国政协单篇采用"></asp:ListItem>
                                        <asp:ListItem Text="全国政协综合采用"></asp:ListItem>
                                        <asp:ListItem Text="全国政协转送国家有关部门"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                                <th rowspan="2">中央领导批示</th>
                                <td rowspan="2"><asp:TextBox ID="txtReply3" runat="server" TextMode="MultiLine" Rows="5" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr class="employ3">
                                <th>全国政协采用情况</th>
                                <td>
                                    <asp:CheckBoxList ID="cblEmploy3" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" ToolTip="采用">
                                        <asp:ListItem Text="全国政协单篇采用得到中央领导批示"></asp:ListItem>
                                        <asp:ListItem Text="全国政协综合采用得到中央领导批示"></asp:ListItem>
                                        <asp:ListItem Text="得到国家有关部门采用"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr class="verify">
                                <th>(核稿)标题</th>
                                <td colspan="3">关于<asp:TextBox ID="txtVerifyTitle" runat="server" CssClass="long" ToolTip="标题"></asp:TextBox>的建议</td>
                            </tr>
                            <tr class="verify">
                                <th>(核稿)反映的问题<br />(分析)</th>
                                <td colspan="3"><asp:TextBox ID="txtVerifyBody" runat="server" TextMode="MultiLine" MaxLength="1500" Rows="8" CssClass="long" ToolTip="反映的问题(分析)"></asp:TextBox></td>
                            </tr>
                            <tr class="verify">
                                <th>(核稿)建议</th>
                                <td colspan="3"><asp:TextBox ID="txtVerifyAdvise" runat="server" TextMode="MultiLine" MaxLength="1500" Rows="8" CssClass="long" ToolTip="建议"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>备注</th>
                                <td colspan="3"><asp:TextBox ID="txtRemark" runat="server" TextMode="MultiLine" Rows="3" CssClass="long"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnEdit" runat="server" Text="确定" OnClick="btnEdit_Click" />
                        <asp:Button ID="btnDel" runat="server" Text="删除" Visible="false" OnClick="btnDel_Click" />
                        <%--<asp:Button ID="btnBack" runat="server" Text="退回" OnClick="btnBack_Click" />--%>
                        <input type="button" value="返回" onclick="window.history.back(-1);" />
                    </div>
                </div>
            </asp:PlaceHolder>

        </div>
    </asp:Panel>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
