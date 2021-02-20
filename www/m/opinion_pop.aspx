<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="opinion_pop.aspx.cs" Inherits="hkzx.web.m.opinion_pop" %><%--Tony维护--%>
<%@ Register src="../cn/ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="ucHeader.ascx" tagname="ucHeader" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" Client="wap" />
    <title>虹口政协履职通 - 社情民意</title>
    <script>
        $(function () {
            if ($('#plNav').text()) {
                var url = window.location.href;
                if (url.indexOf('ac=query') > 0) {
                    $('#plNav>div>a[href*="ac=query"]').addClass('cur');
                } else if (url.indexOf('ac=my') > 0) {
                    $('#plNav>div>a[href*="ac=my"]').addClass('cur');
                } else if (url.indexOf('ac=save') > 0) {
                    $('#plNav>div>a[href*="ac=save"]').addClass('cur');
                } else if (url.indexOf('id=') > 0 && !$('#btnSub').val()) {
                    $('#plNav>div>#view').show().addClass('cur');
                } else {
                    //$('#plNav>div>a:first').addClass('cur');
                    $('#plNav>div>a[href$="ac="]').addClass('cur');
                }
                $('#plNav>div>a>b').each(function () {
                    if ($(this).text() == '0') {
                        $(this).hide();
                    }
                });
            }
        });
    </script>
</head>
<body>
<form id="form1" runat="server">
    <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    <asp:HiddenField ID="hfBack" runat="server" /><asp:HiddenField ID="hfOrg" runat="server" />
    <uc1:ucHeader ID="header1" runat="server" Title="社情民意" />
    <div class="content main">
        <asp:Panel ID="plNav" runat="server" CssClass="btn">
            <div>
                <a href="?ac=">提交社情民意</a>
                <a href="?ac=my">我的社情民意</a>
                <a href="?ac=save">暂存<b><asp:Literal ID="ltSaveNum" runat="server">0</asp:Literal></b></a>
                <a href="?ac=query">检索社情民意</a>
                <a id="view" class="hide">查阅社情民意</a>
            </div>
        </asp:Panel>
        
        <asp:PlaceHolder ID="plSub" runat="server" Visible="false">
<script>
    $(function () {
        upFile('#hfFiles', '#files', '#btnFiles', 'doc');
        $('.edit>dl>dd.mans, .edit>dl>dd.team').hide();
        $('#hfSubManType').change(function () {
            switch ($(this).val()) {
                case '委员':
                    $('.edit>dl>dd.mans').show();
                    $('.edit>dl>dd.team').hide();
                    break;
                default:
                    $('.edit>dl>dd.mans').hide();
                    $('.edit>dl>dd.team').show();
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
        $('#btnSubMans').click(function () {
            var title = '';
            var url = '../cn/dialog.aspx?ac=subman';
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
            showDialog(title, url, '', 320, 480, 'no');
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
            <div class="edit">
                <dl><asp:HiddenField ID="hfSubManType" runat="server" Value="" />
                    <dt>提交社情民意</dt>
                    <dd class="mans">
                        <strong><b>*</b>反映人</strong>
                        <asp:TextBox ID="txtSubMan" runat="server" ReadOnly="true" CssClass="readonly" MaxLength="20" ToolTip="反映人"></asp:TextBox>
                    </dd>
                    <dd class="team">
                        <strong><b>*</b>反映单位</strong>
                        <asp:TextBox ID="txtSubOrg" runat="server" ReadOnly="true" CssClass="readonly" MaxLength="20" ToolTip="反映单位"></asp:TextBox>
                    </dd>
                    <dd class="team">
                        <strong><b>*</b>第一反映人</strong>
                        <asp:TextBox ID="txtLinkman" runat="server" MaxLength="20" ToolTip="第一反映人"></asp:TextBox>
                        <a id="btnLinkman" href="#" class="btn"><u>选取</u></a>
                    </dd>
                    <dd>
                        <strong><b>*</b>第一反映人身份</strong>
                        <asp:CheckBoxList ID="cblLinkmanInfo" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="第一反映人身份"></asp:CheckBoxList>
                    </dd>
                    <dd class="team">
                        <strong><b>*</b>政治面貌</strong>
                        <asp:CheckBoxList ID="cblLinkmanParty" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="政治面貌"></asp:CheckBoxList>
                    </dd>
                    <dd>
                        <strong><b>*</b>工作单位与职务</strong>
                        <asp:TextBox ID="txtLinkmanOrgName" runat="server" MaxLength="100" CssClass="long" ToolTip="工作单位与职务"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong><b>*</b>联系方式</strong>
                        <asp:TextBox ID="txtLinkmanTel" runat="server" MaxLength="50" ToolTip="联系方式"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong>联名反映人（委员）</strong>
                        <asp:TextBox ID="txtSubMans" runat="server" CssClass="readonly long" ToolTip="(委员)联名反映人"></asp:TextBox>
                        <a id="btnSubMans" href="#"><u>选取</u></a>
                    </dd>
                    <dd>
                        <strong>非委员联名人</strong>
                        <asp:TextBox ID="txtSubMan2" runat="server" CssClass="readonly long" ToolTip="非委员联名人"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong><b>*</b>是否同意公开</strong>
                        <asp:RadioButtonList ID="rblIsOpen" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="是否同意公开">
                            <asp:ListItem Text="是" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="否"></asp:ListItem>
                        </asp:RadioButtonList>
                        <asp:TextBox ID="txtOpenInfo" runat="server" MaxLength="20" ToolTip="原因"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong><b>*</b>信息分类</strong>
                        <asp:RadioButtonList ID="rblSubType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="类别"></asp:RadioButtonList>
                    </dd>
                    <dd>
                        <strong><b>*</b>信息标题</strong>
                        <asp:TextBox ID="txtSummary" runat="server" CssClass="long" MaxLength="100" ToolTip="标题"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong><b>*</b>情况反映</strong>
                        <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" CssClass="long" Rows="8"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong>意见建议</strong>
                        <asp:TextBox ID="txtAdvise" runat="server" TextMode="MultiLine" CssClass="long" Rows="8"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong>附件</strong>
                        <asp:HiddenField ID="hfFiles" runat="server" />
                        <a id="btnFiles" href="#" class="btn"><u>上传</u></a>
                        <div id="files"></div>
                    </dd>
                </dl>
                <div class="cmd">
                    <asp:Button ID="btnSub" runat="server" Text="提交" OnClick="btnSub_Click" />
                    <asp:Button ID="btnSave" runat="server" Text="暂存" OnClick="btnSave_Click" />
                    <asp:Button ID="btnDel" runat="server" Text="删除" Visible="false" OnClick="btnDel_Click" />
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plMy" runat="server" Visible="false">
            <div class="list table">
                <dl>
                    <dt>我的社情民意</dt>
                    <asp:Repeater ID="rpMyList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                <p>
                                    <b>信息分类：</b><%#DataBinder.Eval(Container.DataItem, "SubType")%><br />
                                    <b>信息标题：</b><%#DataBinder.Eval(Container.DataItem, "Summary")%><br />
                                    <b>提交日期：</b><%#DataBinder.Eval(Container.DataItem, "SubTime", "{0:yyyy-MM-dd}")%><br />
                                    <b>状态：</b><%#DataBinder.Eval(Container.DataItem, "ActiveName")%><br />
                                    <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u><%#DataBinder.Eval(Container.DataItem, "StateName")%></u></a>
                                </p>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltMyNo" runat="server">
                        <dd class="no">暂无提交的社情民意！</dd>
                    </asp:Literal>
                </dl>
                <asp:Label ID="lblMyNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plSave" runat="server" Visible="false">
            <div class="list table">
                <dl>
                    <dt>暂存的社情民意</dt>
                    <asp:Repeater ID="rpSaveList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                <p>
                                    <b>信息分类：</b><%#DataBinder.Eval(Container.DataItem, "SubType")%><br />
                                    <b>信息标题：</b><%#DataBinder.Eval(Container.DataItem, "Summary")%><br />
                                    <b>编辑时间：</b><%#DataBinder.Eval(Container.DataItem, "UpTime", "{0:yyyy-MM-dd HH:mm:ss}")%><br />
                                    <b>状态：</b><%#DataBinder.Eval(Container.DataItem, "ActiveName")%><br />
                                    <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>修改</u></a>
                                </p>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltSaveNo" runat="server">
                        <dd class="no">暂无暂存的社情民意！</dd>
                    </asp:Literal>
                </dl>
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
            showDialog(title, '../cn/dialog.aspx?ac=subman&type=all&obj=' + obj, '', 320, 480, 'no');
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
            <div class="edit query">
                <div class="label">查询条件</div>
                <dl>
                    <dd>
                        <strong>录用情况</strong>
                        <asp:DropDownList ID="ddlQActive" runat="server">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                            <asp:ListItem Text="已录用"></asp:ListItem>
                            <asp:ListItem Text="未录用"></asp:ListItem>
                            <asp:ListItem Text="留存"></asp:ListItem>
                        </asp:DropDownList>
                    </dd>
                    <dd>
                        <strong>社情民意类别</strong>
                        <asp:DropDownList ID="ddlQSubType" runat="server">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </dd>
                    <dd>
                        <strong>反映单位/反映人</strong>
                        <asp:TextBox ID="txtQSubMan" runat="server" MaxLength="20"></asp:TextBox>
                        <a id="btnQSubMan" href="#" class="btn"><u>选取</u></a>
                    </dd>
                    <dd>
                        <strong>专委会</strong>
                        <asp:DropDownList ID="ddlQCommittee" runat="server">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </dd>
                    <dd>
                        <strong>界别</strong>
                        <asp:DropDownList ID="ddlQSubsector" runat="server">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </dd>
                    <dd>
                        <strong>街道活动组</strong>
                        <asp:DropDownList ID="ddlQStreetTeam" runat="server">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </dd>
                    <dd>
                        <strong>政治面貌</strong>
                        <asp:DropDownList ID="ddlQParty" runat="server">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </dd>
                    <dd>
                        <strong>提交日期</strong>
                        <asp:TextBox ID="txtQSubTime1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox>&nbsp;<asp:TextBox ID="txtQSubTime2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong>信息标题</strong>
                        <asp:TextBox ID="txtQSummary" runat="server" CssClass="long"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong>情况反映</strong>
                        <asp:TextBox ID="txtQBody" runat="server" CssClass="long"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong>意见建议</strong>
                        <asp:TextBox ID="txtQAdvise" runat="server" CssClass="long"></asp:TextBox>
                    </dd>
                </dl>
                <div class="cmd">
                    <input id="btnQuery" type="button" value="查询" />
                    <input id="clean" type="reset" value="清空" />
                </div>
            </div>
            <div class="list table">
                <dl>
                    <dt class="total">符合条件的数据有：<b><asp:Literal ID="ltQueryTotal" runat="server" Text="0"></asp:Literal></b>条</dt>
                    <asp:Repeater ID="rpQueryList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                <p>
                                    <b>信息分类：</b><%#DataBinder.Eval(Container.DataItem, "SubType")%><br />
                                    <b>信息标题：</b><%#DataBinder.Eval(Container.DataItem, "Summary")%><br />
                                    <b>反映人：</b><%#DataBinder.Eval(Container.DataItem, "SubMan")%><br />
                                    <b>提交日期：</b><%#DataBinder.Eval(Container.DataItem, "SubTime", "{0:yyyy-MM-dd}")%><br />
                                    <b>录用情况：</b><%#DataBinder.Eval(Container.DataItem, "ActiveName")%><br />
                                    <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看</u></a>
                                </p>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltQueryNo" runat="server">
                        <dd class="no">暂无社情民意！</dd>
                    </asp:Literal>
                </dl>
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
            <div class="edit">
                <div class="label">查阅社情民意</div>
                <table>
                    <tbody>
                        <tr>
                            <th>录用情况</th>
                            <td><asp:Literal ID="ltActive" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>提交日期</th>
                            <td><asp:Literal ID="ltSubTime" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>信息分类</th>
                            <td><asp:Literal ID="ltSubType" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>是否同意公开</th>
                            <td><asp:Literal ID="ltIsOpen" runat="server"></asp:Literal></td>
                        </tr>
                        <tr class="man">
                            <th>反映人</th>
                            <td id="SubMan"><asp:Literal ID="ltSubMan" runat="server"></asp:Literal></td>
                        </tr>
                        <tr class="team">
                            <th>反映单位</th>
                            <td><asp:Literal ID="ltSubOrg" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>反映人</th>
                            <td><asp:Literal ID="ltLinkman" runat="server"></asp:Literal></td>
                        </tr>
                        <asp:PlaceHolder ID="plShowInfo" runat="server" Visible="false">
                            <tr>
                                <th>第一反映人身份</th>
                                <td><asp:Literal ID="ltLinkmanInfo" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>政治面貌</th>
                                <td><asp:Literal ID="ltLinkmanParty" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>工作单位与职务</th>
                                <td><asp:Literal ID="ltLinkmanOrgName" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>联系方式</th>
                                <td><asp:Literal ID="ltLinkmanTel" runat="server"></asp:Literal></td>
                            </tr>
                        </asp:PlaceHolder>
                        <tr>
                            <th>联名反映人</th>
                            <td><asp:Literal ID="ltSubMans" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>信息标题</th>
                            <td><asp:Literal ID="ltSummary" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>情况反映</th>
                            <td class="editor"><asp:Literal ID="ltBody" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>意见建议</th>
                            <td class="editor"><asp:Literal ID="ltAdvise" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>附件</th>
                            <td><asp:Literal ID="ltFiles" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>采用说明</th>
                            <td><asp:Literal ID="ltEmploy" runat="server"></asp:Literal></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </asp:PlaceHolder>
    </div>
</form>
</body>
</html>
