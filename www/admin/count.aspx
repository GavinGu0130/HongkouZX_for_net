<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="count.aspx.cs" Inherits="hkzx.web.admin.count" %><%--Tony维护--%>
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
                if (url.indexOf('ac=') >= 0) {
                    url = url.substring(url.indexOf('ac='));
                    if (url.indexOf('&') > 0) {
                        url = url.substring(0, url.indexOf('&'));
                    }
                    $('#plNav>a[href*="' + url + '"]').addClass('cur');
                } else {
                    $('#plNav>a:first').addClass('cur');
                }
            }
            if ($('.list>table>tbody>tr>td.no').text()) {
                $('.list>table>tbody>tr>td.no').attr('colspan', $('.list>table>thead>tr>th').length);
                $('.list>table>thead').hide();
            }
            $('.count>table>tbody>tr>td>a').each(function () {
                var url = $(this).attr('href');
                if (url.indexOf('/cn/') < 0) {
                    url = '../cn/' + url;
                }
                var title = '';
                if (url.indexOf('ac=score') > 0) {
                    title = '积分记录';
                } else if (url.indexOf('ac=perform') > 0) {
                    title = $(this).attr('title') + '记录';
                } else if (url.indexOf('ac=speak') > 0) {
                    title = '会议发言记录';
                } else if (url.indexOf('ac=opinion') > 0) {
                    title = '提案记录';
                } else if (url.indexOf('ac=pop') > 0) {
                    title = '社情民意记录';
                }
                $(this).click(function () {
                    showDialog(title, url, '', 640, 400, 'yes');
                    return false;
                });
            });
        });
    </script>
</head>
<body>
<form id="form1" runat="server">
    <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    <asp:HiddenField ID="hfBack" runat="server" Value="./" /><asp:HiddenField ID="hfOrg" runat="server" />
    <uc1:ucHeader ID="header1" runat="server" />
    <div class="content">
        <div class="main">
            <asp:Panel ID="plNav" runat="server" Visible="false" CssClass="btn">
                <a href="?ac=user">委员/团体</a>
                <a href="?ac=committee">专委会</a>
                <a href="?ac=subsector">界别</a>
                <a href="?ac=street">街道活动组</a>
                <a href="?ac=speak">会议发言</a>
                <a href="?ac=invited">特邀监督员工作</a>
                <a href="?ac=appraise">遴选评优</a>
                <asp:HyperLink ID="lnkOther" runat="server" Visible="false" NavigateUrl="?ac=other">其它</asp:HyperLink>
            </asp:Panel>

            <asp:PlaceHolder ID="plUser" runat="server" Visible="false">
<script>
    $(function () {
        //$('.main>.line').hide();
        $('#allPlay, #allSpeak, #allReport').click(function () {
            var obj = $(this).attr('id').replace('all', 'cblF');
            $('#' + obj + '>input[type=checkbox]').prop('checked', $(this).prop('checked'));
        });
        $('#cblFPlay>input[type=checkbox], #cblFSpeak>input[type=checkbox], #cblFReport>input[type=checkbox]').click(function () {
            var $parent = $(this).parent();
            var obj = $parent.attr("id").replace('cblF', '#all');
            if ($parent.find('>input[type=checkbox]:checked').length == $parent.find('>input[type=checkbox]').length) {
                $(obj).prop('checked', 'checked');
            } else {
                $(obj).prop('checked', '');
            }
        });
        $('#rblQUserType').change(function () {
            if ($(this).find('input:radio:checked').val() == '委员') {
                $('.edit>table>tbody>tr.man').show();
            } else {
                $('.edit>table>tbody>tr.man').hide();
            }
        }).change();
        $('form').submit(function () {
            try {
                var url = '';
                var type = $('#rblQUserType>input:radio:checked').val();
                url += '&UserType=' + type;
                if (type == '委员') {
                    if ($('#txtQUserCode').val()) {
                        url += '&UserCode=' + $('#txtQUserCode').val();
                    }
                    if ($('#txtQTrueName').val()) {
                        url += '&TrueName=' + $('#txtQTrueName').val();
                    }
                    if ($('#ddlQUserSex').val()) {
                        url += '&UserSex=' + $('#ddlQUserSex').val();
                    }
                    if ($('#txtQNative').val()) {
                        url += '&Native=' + $('#txtQNative').val();
                    }
                    if ($('#ddlQNation').val()) {
                        url += '&Nation=' + $('#ddlQNation').val();
                    }
                    if ($('#txtQBirthday1').val() || $('#txtQBirthday2').val()) {
                        url += '&Birthday=' + $('#txtQBirthday1').val() + ',' + $('#txtQBirthday2').val();
                    }
                    if ($('#txtQPostDate1').val() || $('#txtQPostDate2').val()) {
                        url += '&PostDate=' + $('#txtQPostDate1').val() + ',' + $('#txtQPostDate2').val();
                    }
                    if ($('#txtQEducation').val()) {
                        url += '&Education=' + $('#txtQEducation').val();
                    }
                    if ($('#ddlQParty').val()) {
                        url += '&Party=' + $('#ddlQParty').val();
                    }
                    if ($('#ddlQCommittee').val()) {
                        url += '&Committee=' + $('#ddlQCommittee').val();
                    }
                    if ($('#ddlQSubsector').val()) {
                        url += '&Subsector=' + $('#ddlQSubsector').val();
                    }
                    if ($('#ddlQStreetTeam').val()) {
                        url += '&StreetTeam=' + $('#ddlQStreetTeam').val();
                    }
                    if ($('#ddlQOrgType').val()) {
                        url += '&OrgType=' + $('#ddlQOrgType').val();
                    }
                    if ($('#txtQOrgName').val()) {
                        url += '&OrgName=' + $('#txtQOrgName').val();
                    }
                    if ($('#txtQContactTel').val()) {
                        url += '&ContactTel=' + $('#txtQContactTel').val();
                    }
                    var tmp = getChecked('#cblQRole', ',');
                    if (tmp != '') {
                        url += '&Role=' + tmp;
                    }
                } else {

                }
                if ($('#txtQCountDate1').val() || $('#txtQCountDate2').val()) {
                    url += '&CountDate=' + $('#txtQCountDate1').val() + ',' + $('#txtQCountDate2').val();
                }
                url += '&CountType=' + $('#rblCountType>input:radio:checked').val();
                var sel = '';
                tmp = getChecked('#cblFPlay', ',');
                if (tmp != '') {
                    if (sel != '') {
                        set += ',';
                    }
                    sel += tmp;
                }
                tmp = getChecked('#cblFSpeak', ',');
                if (tmp != '') {
                    if (sel != '') {
                        sel += ',';
                    }
                    sel += tmp;
                }
                tmp = getChecked('#cblFReport', ',');
                if (tmp != '') {
                    if (sel != '') {
                        sel += ',';
                    }
                    sel += tmp;
                }
                tmp = getChecked('#cblFOther', ',');
                if (tmp != '') {
                    if (sel != '') {
                        sel += ',';
                    }
                    sel += tmp;
                }
                if (sel != '') {
                    url += '&Fields=' + sel;
                }
                //alert(encodeURI(url)); return false;
                window.location.href = '?ac=user' + encodeURI(url);
                return false;
            } catch (err) {
                alert("验证出错，请稍后重试！"+err);
                return false;
            }
        });
    });
</script>
                <div class="frm edit">
                    <strong>委员/团体查询</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>统计类型</th>
                                <td colspan="3">
                                    <asp:RadioButtonList ID="rblQUserType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="委员" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="专委会"></asp:ListItem>
                                        <asp:ListItem Text="界别"></asp:ListItem>
                                        <asp:ListItem Text="街道活动组"></asp:ListItem>
                                        <asp:ListItem Text="党派团体"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr class="man">
                                <th>委员编号</th>
                                <td><asp:TextBox ID="txtQUserCode" runat="server"></asp:TextBox></td>
                                <th></th>
                                <td></td>
                            </tr>
                            <tr class="man">
                                <th>委员姓名</th>
                                <td><asp:TextBox ID="txtQTrueName" runat="server"></asp:TextBox></td>
                                <th>性别</th>
                                <td>
                                    <asp:DropDownList ID="ddlQUserSex" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        <asp:ListItem Text="男"></asp:ListItem>
                                        <asp:ListItem Text="女"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr class="man">
                                <th>籍贯</th>
                                <td><asp:TextBox ID="txtQNative" runat="server"></asp:TextBox></td>
                                <th>民族</th>
                                <td>
                                    <asp:DropDownList ID="ddlQNation" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        <asp:ListItem Text="汉族"></asp:ListItem>
                                        <asp:ListItem Text="阿昌族"></asp:ListItem>
                                        <asp:ListItem Text="白族"></asp:ListItem>
                                        <asp:ListItem Text="保安族"></asp:ListItem>
                                        <asp:ListItem Text="布朗族"></asp:ListItem>
                                        <asp:ListItem Text="布依族"></asp:ListItem>
                                        <asp:ListItem Text="藏族"></asp:ListItem>
                                        <asp:ListItem Text="朝鲜族"></asp:ListItem>
                                        <asp:ListItem Text="达斡尔族"></asp:ListItem>
                                        <asp:ListItem Text="傣族"></asp:ListItem>
                                        <asp:ListItem Text="德昂族"></asp:ListItem>
                                        <asp:ListItem Text="东乡族"></asp:ListItem>
                                        <asp:ListItem Text="侗族"></asp:ListItem>
                                        <asp:ListItem Text="独龙族"></asp:ListItem>
                                        <asp:ListItem Text="俄罗斯族"></asp:ListItem>
                                        <asp:ListItem Text="鄂伦春族"></asp:ListItem>
                                        <asp:ListItem Text="鄂温克族"></asp:ListItem>
                                        <asp:ListItem Text="高山族"></asp:ListItem>
                                        <asp:ListItem Text="哈尼族"></asp:ListItem>
                                        <asp:ListItem Text="哈萨克族"></asp:ListItem>
                                        <asp:ListItem Text="赫哲族"></asp:ListItem>
                                        <asp:ListItem Text="回族"></asp:ListItem>
                                        <asp:ListItem Text="基诺族"></asp:ListItem>
                                        <asp:ListItem Text="京族"></asp:ListItem>
                                        <asp:ListItem Text="景颇族"></asp:ListItem>
                                        <asp:ListItem Text="柯尔克孜族"></asp:ListItem>
                                        <asp:ListItem Text="拉祜族"></asp:ListItem>
                                        <asp:ListItem Text="黎族"></asp:ListItem>
                                        <asp:ListItem Text="珞巴族"></asp:ListItem>
                                        <asp:ListItem Text="满族"></asp:ListItem>
                                        <asp:ListItem Text="毛南族"></asp:ListItem>
                                        <asp:ListItem Text="门巴族"></asp:ListItem>
                                        <asp:ListItem Text="蒙古族"></asp:ListItem>
                                        <asp:ListItem Text="苗族"></asp:ListItem>
                                        <asp:ListItem Text="仫佬族"></asp:ListItem>
                                        <asp:ListItem Text="纳西族"></asp:ListItem>
                                        <asp:ListItem Text="怒族"></asp:ListItem>
                                        <asp:ListItem Text="普米族"></asp:ListItem>
                                        <asp:ListItem Text="羌族"></asp:ListItem>
                                        <asp:ListItem Text="撒拉族"></asp:ListItem>
                                        <asp:ListItem Text="畲族"></asp:ListItem>
                                        <asp:ListItem Text="水族"></asp:ListItem>
                                        <asp:ListItem Text="僳僳族"></asp:ListItem>
                                        <asp:ListItem Text="塔吉克族"></asp:ListItem>
                                        <asp:ListItem Text="塔塔尔族"></asp:ListItem>
                                        <asp:ListItem Text="土家族"></asp:ListItem>
                                        <asp:ListItem Text="土族"></asp:ListItem>
                                        <asp:ListItem Text="佤族"></asp:ListItem>
                                        <asp:ListItem Text="维吾尔族"></asp:ListItem>
                                        <asp:ListItem Text="乌孜别克族"></asp:ListItem>
                                        <asp:ListItem Text="锡伯族"></asp:ListItem>
                                        <asp:ListItem Text="瑶族"></asp:ListItem>
                                        <asp:ListItem Text="彝族"></asp:ListItem>
                                        <asp:ListItem Text="仡佬族"></asp:ListItem>
                                        <asp:ListItem Text="裕固族"></asp:ListItem>
                                        <asp:ListItem Text="壮族"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr class="man">
                                <th>出生日期</th>
                                <td><asp:TextBox ID="txtQBirthday1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox> - <asp:TextBox ID="txtQBirthday2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox></td>
                                <th>任职日期</th>
                                <td><asp:TextBox ID="txtQPostDate1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox> - <asp:TextBox ID="txtQPostDate2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox></td>
                            </tr>
                            <tr class="man">
                                <th>政治面貌</th>
                                <td>
                                    <asp:DropDownList ID="ddlQParty" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th>文化程度</th>
                                <td><asp:TextBox ID="txtQEducation" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr class="man">
                                <th>专委会</th>
                                <td>
                                    <asp:DropDownList ID="ddlQCommittee" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th>联系电话</th>
                                <td><asp:TextBox ID="txtQContactTel" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr class="man">
                                <th>界别</th>
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
                            <tr class="man">
                                <th>工作单位</th>
                                <td><asp:TextBox ID="txtQOrgName" runat="server" CssClass="long"></asp:TextBox></td>
                                <th>单位性质</th>
                                <td>
                                    <asp:DropDownList ID="ddlQOrgType" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr class="man">
                                <th>政协职务</th>
                                <td colspan="3">
                                    <asp:CheckBoxList ID="cblQRole" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th>统计起止时间</th>
                                <td><asp:TextBox ID="txtQCountDate1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox> - <asp:TextBox ID="txtQCountDate2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtQCountDate1\')}'})"></asp:TextBox></td>
                                <th>统计方式</th>
                                <td>
                                    <asp:RadioButtonList ID="rblCountType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="分数" Value="score" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="次数" Value="num"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <th>选择查询信息<br />
                                    <asp:CheckBox ID="allPlay" runat="server" />全部会议<br />
                                    <asp:CheckBox ID="allSpeak" runat="server" />全部发言<br />
                                    <asp:CheckBox ID="allReport" runat="server" />全部调研<br />
                                </th>
                                <td colspan="3">
                                    <asp:CheckBoxList ID="cblFPlay" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList><br />
                                    <asp:CheckBoxList ID="cblFSpeak" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList><br />
                                    <asp:CheckBoxList ID="cblFReport" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList><br />
                                    <asp:CheckBoxList ID="cblFOther" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <input id="btnQuery" type="submit" value="查询" />
                        <asp:HyperLink ID="lnkDownXls" runat="server" Target="_blank">导出</asp:HyperLink>
                    </div>
                </div>
                <div class="list hover count">
                    <strong>结果展现
                        <span>符合条件的数据有：<b><asp:Literal ID="ltUserTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <asp:Literal ID="ltUserTable" runat="server"></asp:Literal>
                    <asp:Label ID="lblUserNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plCount" runat="server" Visible="false">
<script>
    $(function () {
        $('#ddlYear').change(function () {
            var url = window.location.href;
            if (url.indexOf('&year=') > 0) {
                url = url.substr(0, url.indexOf('&year='));
            }
            url += '&year=' + $(this).val();
            window.location.href = url;
        });
    });
</script>
                <div class="line"></div>
                <div class="list hover count">
                    <strong class="center">
                        <asp:DropDownList ID="ddlYear" runat="server"></asp:DropDownList>
                        <asp:Literal ID="ltCountTitle" runat="server"></asp:Literal>
                    </strong>
                    <asp:Literal ID="ltCountTable" runat="server"></asp:Literal>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plSpeak" runat="server" Visible="false">
                <div class="line"></div>
                <div class="list hover">
                    <strong>会议发言
                        <span>共有<b><asp:Literal ID="ltSpeakTotal" runat="server" Text="0"></asp:Literal></b>人</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="state">编号</th>
                                <th class="state">委员姓名</th>
                                <th class="num">性别</th>
                                <th class="date">出生日期</th>
                                <th class="state">政治面貌</th>
                                <th class="state">界别</th>
                                <th class="state">特邀监督员</th>
                                <th class="state">体制内外</th>
                                <th class="state">单位性质</th>
                                <th>单位地址</th>
                                <th>家庭地址</th>
                                <th class="date">联系方式</th>
                                <th class="state">上台发言次数</th>
                                <th class="state">书面发言次数</th>
                                <th class="state">其他会议发言</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpSpeakList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th align="center"><%#DataBinder.Eval(Container.DataItem, "UserCode")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "TrueName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "UserSex")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "BirthdayText")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Party")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Subsector")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "IsInvited")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "IsSystem")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "OrgType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OrgAddress")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "HomeAddress")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Mobile")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "PlatformNum") %></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "WriteNum") %></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SpeakNum") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltSpeakNo" runat="server">
                                <tr>
                                    <td class="no">暂无委员发言！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblSpeakNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plInvited" runat="server" Visible="false">
                <div class="line"></div>
                <div class="list hover">
                    <strong>特邀监督员工作
                        <span>共有<b><asp:Literal ID="ltInvitedTotal" runat="server" Text="0"></asp:Literal></b>人</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="state">编号</th>
                                <th class="state">委员姓名</th>
                                <th class="num">性别</th>
                                <th class="date">出生日期</th>
                                <th class="state">政治面貌</th>
                                <th class="state">界别</th>
                                <th class="state">体制内外</th>
                                <th class="state">单位性质</th>
                                <th>单位地址</th>
                                <th>家庭地址</th>
                                <th class="date">联系方式</th>
                                <th class="state">出席活动次数</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpInvitedList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th align="center"><%#DataBinder.Eval(Container.DataItem, "UserCode")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "TrueName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "UserSex")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "BirthdayText")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Party")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Subsector")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "IsSystem")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "OrgType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OrgAddress")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "HomeAddress")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Mobile")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "PerformNum") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltInvitedNo" runat="server">
                                <tr>
                                    <td class="no">暂无特邀监督员信息！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblInvitedNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plAppraise" runat="server" Visible="false">
                <div class="frm edit">
                    <strong>遴选评优</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>评选类型</th>
                                <td>
                                    <asp:RadioButtonList ID="rblAppUserType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="委员" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="专委会"></asp:ListItem>
                                        <asp:ListItem Text="界别"></asp:ListItem>
                                        <asp:ListItem Text="街道活动组"></asp:ListItem>
                                        <asp:ListItem Text="党派"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                                <th>排序方式</th>
                                <td>
                                    <asp:RadioButtonList ID="rblAppOrder" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="降序" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="升序"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <th>起止时间</th>
                                <td><asp:TextBox ID="txtAppDate1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox> - <asp:TextBox ID="txtAppDate2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox></td>
                                <th>评选方式</th>
                                <td>
                                    <asp:RadioButtonList ID="rblAppCountType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="分数" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="次数"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <th>评选项目</th>
                                <td colspan="3">
                                    <asp:RadioButtonList ID="rblAppType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="总分/总次数" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="会议/活动"></asp:ListItem>
                                        <asp:ListItem Text="会议发言"></asp:ListItem>
                                        <asp:ListItem Text="调研报告"></asp:ListItem>
                                        <asp:ListItem Text="提供资源"></asp:ListItem>
                                        <asp:ListItem Text="提案"></asp:ListItem>
                                        <asp:ListItem Text="社情民意"></asp:ListItem>
                                        <asp:ListItem Text="扣分项"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnApp" runat="server" Text="评选" OnClick="btnApp_Click" />
                    </div>
                </div>
                <asp:Panel ID="plApp" runat="server" Visible="false" CssClass="list hover count">
                    <strong>结果展现
                        <span>符合条件的数据有：<b><asp:Literal ID="ltAppTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th>序号</th>
                                <th><asp:Literal ID="ltAppUserName" runat="server">姓名</asp:Literal></th>
                                <th><asp:Literal ID="ltAppUserCode" runat="server">委员编号</asp:Literal></th>
                                <th>会议/活动</th>
                                <th>会议发言</th>
                                <th>调研报告</th>
                                <th>提供资源</th>
                                <th>提案</th>
                                <th>社情民意</th>
                                <th>扣分项</th>
                                <th><asp:Literal ID="ltAppScore" runat="server">总分</asp:Literal></th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpApp" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "UserName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "UserCode")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ScorePlayText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ScoreSpeakText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ScoreReportText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ScoreResourceText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ScoreOpinionText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ScorePopText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ScoreDeText")%></td>
                                        <th><%#DataBinder.Eval(Container.DataItem, "ScoreTotalText") %></th>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </asp:Panel>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plOther" runat="server" Visible="false">
                <div class="frm">
                    <asp:Button ID="btnPop" runat="server" Text="社情民意统计" OnClick="btnPop_Click" />
                </div>
                <asp:Label ID="lblResult" runat="server"></asp:Label>
            </asp:PlaceHolder>

        </div>
    </div>
</form>
</body>
</html>
