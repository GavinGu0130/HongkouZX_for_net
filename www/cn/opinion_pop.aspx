<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="opinion_pop.aspx.cs" Inherits="hkzx.web.cn.opinion_pop" %><%--Tony维护--%>
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
                } else if (url.indexOf('id=') > 0 && !$('#btnSub').val()) {
                    $('#plNav>#view').show().addClass('cur');
                } else {
                    //$('#plNav>a:first').addClass('cur');
                    $('#plNav>a[href$="ac="]').addClass('cur');
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
    <asp:HiddenField ID="hfBack" runat="server" /><asp:HiddenField ID="hfOrg" runat="server" />
    <uc1:ucHeader ID="header1" runat="server" />
    <div class="content">
        <div class="main">
            <asp:Panel ID="plNav" runat="server" Visible="false" CssClass="btn">
                <a href="?ac=">提交社情民意</a>
                <a href="?ac=my">我的社情民意</a>
                <%--<a href="?ac=feed">需反馈提案<b>(<asp:Literal ID="ltFeedNum" runat="server">0</asp:Literal>)</b></a>--%>
                <a href="?ac=save">暂存<b>(<asp:Literal ID="ltSaveNum" runat="server">0</asp:Literal>)</b></a>
                <a href="?ac=query">检索社情民意</a>
                <a id="view" class="hide">查阅社情民意</a>
            </asp:Panel>

            <asp:PlaceHolder ID="plSub" runat="server" Visible="false">
<script>
    $(function () {
        upFile('#hfFiles', '#files', '#btnFiles', 'doc');
        $('.edit>table>tbody>tr.mans, .edit>table>tbody>tr.team').hide();
        $('#hfSubManType').change(function () {
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
        }).change();
        function showOpenInfo() {
            if ($('#rblIsOpen>input:radio:checked').val() == '否') {
                $('#txtOpenInfo').show();
            } else {
                $('#txtOpenInfo').hide();
            }
        }
        showOpenInfo();
        $('#rblIsOpen>input:radio').click(function () {
            if ($(this).is(':checked')) {
                showOpenInfo()
            }
        });
        $('#btnLinkman, #btnSubMans').click(function () {
            var title = '';
            var url = 'dialog.aspx?ac=subman';
            switch ($(this).attr('id')) {
                case 'btnLinkman':
                    title = '选取第一反映人';
                    url += '&type=all&obj=txtLinkman';
                    break;
                default:
                    title = '邀请联名反映人';
                    url += '&obj=txtSubMans';
                    break;
            }
            showDialog(title, url, '', 640, 400, 'no');
            return false;
        });
        $('#txtSubMans').focus(function () {
            $('#' + $(this).attr('id').replace('txt', 'btn')).click();
        }).change(function () {
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
        });
        $('form').submit(function () {
            try {
                //alert('暂未开放'); return false;
                if ($('#hfSubManType').val() == '委员') {
                    if (checkEmpty('#txtSubMan')) {
                        return false;
                    }
                } else if (checkEmpty('#txtSubOrg') || checkEmpty('#txtLinkman') || checkCheck('#cblLinkmanInfo') || checkCheck('#cblLinkmanParty') || checkEmpty('#txtLinkmanOrgName') || checkEmpty('#txtLinkmanTel')) {
                    return false;
                }
                if ($('#rblIsOpen>input:radio:checked').val() == '否' && checkEmpty('#txtOpenInfo')) {
                    return false;
                }
                if (checkRadio('#rblIsOpen') || checkRadio('#rblSubType') || checkEmpty('#txtSummary') || checkEmpty('#txtBody')) {
                    return false;
                }
            } catch (err) {
                //alert("验证出错，请稍后重试！");
                return false;
            }
        });
    });
</script>
                <div class="frm edit">
                    <strong>提交社情民意</strong>
                    <table>
                        <tbody>
                            <asp:HiddenField ID="hfSubManType" runat="server" Value="" />
                            <tr class="mans">
                                <th><b>*</b>反映人</th>
                                <td><asp:TextBox ID="txtSubMan" runat="server" ReadOnly="true" CssClass="readonly" MaxLength="20" ToolTip="反映人"></asp:TextBox></td>
                                <th></th>
                                <td></td>
                            </tr>
                            <tr class="team">
                                <th><b>*</b>反映单位</th>
                                <td><asp:TextBox ID="txtSubOrg" runat="server" ReadOnly="true" CssClass="readonly long" MaxLength="20" ToolTip="反映单位"></asp:TextBox></td>
                                <th rowspan="2"><b>*</b>政治面貌</th>
                                <td rowspan="2">
                                    <asp:CheckBoxList ID="cblLinkmanParty" runat="server" RepeatDirection="Horizontal" RepeatColumns="4" Width="100%" ToolTip="政治面貌"></asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr class="team">
                                <th><b>*</b>第一反映人</th>
                                <td><asp:TextBox ID="txtLinkman" runat="server" MaxLength="20" ToolTip="第一反映人"></asp:TextBox>
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
                                <th><b>*</b>工作单位与职务</th>
                                <td><asp:TextBox ID="txtLinkmanOrgName" runat="server" MaxLength="100" CssClass="long" ToolTip="工作单位与职务"></asp:TextBox></td>
                                <th><b>*</b>联系方式</th>
                                <td><asp:TextBox ID="txtLinkmanTel" runat="server" MaxLength="50" ToolTip="联系方式"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>是否同意公开</th>
                                <td>
                                    <asp:RadioButtonList ID="rblIsOpen" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="是否同意公开">
                                        <asp:ListItem Text="是" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="否"></asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:TextBox ID="txtOpenInfo" runat="server" MaxLength="20" ToolTip="原因"></asp:TextBox>
                                </td>
                                <th rowspan="2">联名反映人<br />（委员）</th>
                                <td rowspan="2"><asp:TextBox ID="txtSubMans" runat="server" TextMode="MultiLine" Rows="2" CssClass="readonly long" ToolTip="(委员)联名反映人"></asp:TextBox>
                                    <a id="btnSubMans" href="#" class="btn"><u>选取</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th>非委员联名人</th>
                                <td><asp:TextBox ID="txtSubMan2" runat="server" CssClass="long" ToolTip="非委员联名人"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>信息分类</th>
                                <td colspan="3">
                                    <asp:RadioButtonList ID="rblSubType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="类别"></asp:RadioButtonList>
                                </td>
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
                                <td colspan="3"><asp:TextBox ID="txtAdvise" runat="server" TextMode="MultiLine" CssClass="long" Rows="8"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>附件</th>
                                <td colspan="3">
                                    <asp:HiddenField ID="hfFiles" runat="server" />
                                    <a id="btnFiles" href="#" class="btn"><u>上传</u></a>
                                    <div id="files"></div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnSub" runat="server" Text="提交" OnClick="btnSub_Click" />
                        <asp:Button ID="btnSave" runat="server" Text="暂存" OnClick="btnSave_Click" />
                        <asp:Button ID="btnDel" runat="server" Text="删除" Visible="false" OnClick="btnDel_Click" />
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plMy" runat="server" Visible="false">
                <div class="frm list">
                    <strong>我的社情民意
                        <span>符合条件的数据有：<b><asp:Literal ID="ltMyTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>信息分类</th>
                                <th>信息标题</th>
                                <th class="date">提交日期</th>
                                <th class="state">录用情况</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpMyList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Summary")%></td>
                                        <td align="center" title="<%#DataBinder.Eval(Container.DataItem, "SubTime", "{0:yyyy-MM-dd HH:mm:ss}")%>"><%#DataBinder.Eval(Container.DataItem, "SubTimeText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td><a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u><%#DataBinder.Eval(Container.DataItem, "StateName")%></u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltMyNo" runat="server">
                                <tr>
                                    <td class="no">暂无提交的社情民意！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblMyNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plSave" runat="server" Visible="false">
                <div class="frm list">
                    <strong>暂存的社情民意</strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="time">信息分类</th>
                                <th>信息标题</th>
                                <th class="time">编辑时间</th>
                                <th class="state">状态</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpSaveList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Summary")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "UpTime", "{0:yyyy-MM-dd HH:mm:ss}")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>选取</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltSaveNo" runat="server">
                                <tr>
                                    <td class="no">暂无暂存的社情民意！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblSaveNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plQuery" runat="server" Visible="false">
<script>
    $(function () {
        loadSelect('#hfOrg', '#ddlQOrgType', '#QOrgName', '全部', '#hfQOrgName');
        $('#btnQSubMan').click(function () {
            var title = '选取反映人';
            var obj = 'txtQSubMan';
            showDialog(title, 'dialog.aspx?ac=subman&type=all&obj=' + obj, '', 640, 400, 'no');
            return false;
        });
        $('#btnQuery').click(function () {
            try {
                var url = '';
                if ($('#ddlQActive').val()) {
                    url += '&Active=' + $('#ddlQActive').val();
                }
                if ($('#ddlQSubType').val()) {
                    url += '&SubType=' + $('#ddlQSubType').val();
                }
                if ($('#txtQSubMan').val()) {
                    url += '&SubMan=' + $('#txtQSubMan').val();
                }
                var tmp = getChecked('#cblQLinkmanInfo', ',');
                if (tmp) {
                    url += '&LinkmanInfo=' + tmp;
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
                //alert(encodeURI(url)); return false;
                if (url != '') {
                    window.location.href = '?ac=query' + encodeURI(url);
                } else {
                    window.location.href = '?ac=query';
                }
            } catch (err) {
                //alert("验证出错，请稍后重试！");
                return false;
            }
        });
    });
</script>
                <div class="frm edit">
                    <strong>查询条件</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>录用情况</th>
                                <td>
                                    <asp:DropDownList ID="ddlQActive" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        <asp:ListItem Text="已录用"></asp:ListItem>
                                        <asp:ListItem Text="留存"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th>社情民意类别</th>
                                <td>
                                    <asp:DropDownList ID="ddlQSubType" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>反映单位/反映人</th>
                                <td><asp:TextBox ID="txtQSubMan" runat="server" MaxLength="20" CssClass="long"></asp:TextBox>
                                    <a id="btnQSubMan" href="#" class="btn"><u>选取</u></a>
                                </td>
                                <th>提交日期</th>
                                <td><asp:TextBox ID="txtQSubTime1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox> - <asp:TextBox ID="txtQSubTime2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox></td>
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
                        </tbody>
                    </table>
                    <div class="cmd">
                        <input id="btnQuery" type="button" value="查询" />
                        <input id="clean" type="reset" value="清空" />
                    </div>
                </div>
                <div class="list">
                    <strong>结果展现
                        <span>符合条件的数据有：<b><asp:Literal ID="ltQueryTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>信息分类</th>
                                <th>信息标题</th>
                                <th class="state">反映人</th>
                                <th class="date">提交日期</th>
                                <th class="state">录用情况</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpQueryList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Summary")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SubMan")%></td>
                                        <td align="center" title="<%#DataBinder.Eval(Container.DataItem, "SubTime", "{0:yyyy-MM-dd HH:mm:ss}")%>"><%#DataBinder.Eval(Container.DataItem, "SubTimeText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看</u></a></td>
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

            <asp:PlaceHolder ID="plView" runat="server" Visible="false">
<script>
    $(function () {
        $('div.edit>table>tbody>tr>td.editor').each(function () {
            if ($(this).text()) {
                $(this).html(ubb2html($(this).text()));
            }
        });
        if ($('#SubMan').text()) {
            $('.edit>table>tbody>tr.man').show();
            $('.edit>table>tbody>tr.team').hide();
        } else {
            $('.edit>table>tbody>tr.man').hide();
            $('.edit>table>tbody>tr.team').show();
        }
    });
</script>
                <div class="frm edit">
                    <strong>查阅社情民意</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>录用情况</th>
                                <td><asp:Literal ID="ltActive" runat="server"></asp:Literal></td>
                                <th>提交日期</th>
                                <td><asp:Literal ID="ltSubTime" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>信息分类</th>
                                <td><asp:Literal ID="ltSubType" runat="server"></asp:Literal></td>
                                <th>是否同意公开</th>
                                <td><asp:Literal ID="ltIsOpen" runat="server"></asp:Literal></td>
                            </tr>
                            <tr class="man">
                                <th>反映人</th>
                                <td id="SubMan"><asp:Literal ID="ltSubMan" runat="server"></asp:Literal></td>
                                <th></th>
                                <td></td>
                            </tr>
                            <tr class="team">
                                <th>反映单位</th>
                                <td><asp:Literal ID="ltSubOrg" runat="server"></asp:Literal></td>
                                <th>反映人</th>
                                <td><asp:Literal ID="ltLinkman" runat="server"></asp:Literal></td>
                            </tr>
                            <asp:PlaceHolder ID="plShowInfo" runat="server" Visible="false">
                                <tr>
                                    <th>第一反映人身份</th>
                                    <td><asp:Literal ID="ltLinkmanInfo" runat="server"></asp:Literal></td>
                                    <th>政治面貌</th>
                                    <td><asp:Literal ID="ltLinkmanParty" runat="server"></asp:Literal></td>
                                </tr>
                                <tr>
                                    <th>工作单位与职务</th>
                                    <td><asp:Literal ID="ltLinkmanOrgName" runat="server"></asp:Literal></td>
                                    <th>联系方式</th>
                                    <td><asp:Literal ID="ltLinkmanTel" runat="server"></asp:Literal></td>
                                </tr>
                            </asp:PlaceHolder>
                            <tr>
                                <th>联名反映人</th>
                                <td colspan="3"><asp:Literal ID="ltSubMans" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>信息标题</th>
                                <td colspan="3"><asp:Literal ID="ltSummary" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>情况反映</th>
                                <td colspan="3" class="editor"><asp:Literal ID="ltBody" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>意见建议</th>
                                <td colspan="3" class="editor"><asp:Literal ID="ltAdvise" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>附件</th>
                                <td colspan="3"><asp:Literal ID="ltFiles" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>采用说明</th>
                                <td colspan="3"><asp:Literal ID="ltEmploy" runat="server"></asp:Literal></td>
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
