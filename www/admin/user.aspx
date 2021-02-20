<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="user.aspx.cs" Inherits="hkzx.web.admin.user" %><%--Tony维护--%>
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
            var url = window.location.href;
            if (url.indexOf('ac=scoreadd') > 0) {
                $('#plNav>a#lnkScore').addClass('cur');
            } else if (url.indexOf('ac=score') > 0) {
                $('#plNav>a#score').addClass('cur').show();
            } else if (url.indexOf('id=') < 0) {
                $('#plNav>a:first').addClass('cur').removeAttr('href');
            } else if (url.indexOf('id=0') > 0) {
                $('#plNav>a[href*="id=0"]').addClass('cur').removeAttr('href');
            } else {
                $('#plNav>a#update').addClass('cur').show().text($('div.edit>strong').text());
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
    <asp:Panel ID="plOutput" runat="server" Visible="false" CssClass="dialog">
<script>
    $(function () {
        $('#btnOutput').click(function () {
            var tmp = getChecked('#cblOutput', ',');
            if (!tmp) {
                alert('请选择[下载]项');
                $('#cblQFields>input:checkbox:first').focus();
                return false;
            }
        });
    });
</script>
        <div class="out">
            <asp:CheckBoxList ID="cblOutput" runat="server" RepeatDirection="Horizontal" RepeatColumns="5">
                <asp:ListItem Text="用户类别" Value="UserType"></asp:ListItem>
                <asp:ListItem Text="委员编号" Value="UserCode" Selected="True"></asp:ListItem>
                <asp:ListItem Text="姓名" Value="TrueName" Selected="True"></asp:ListItem>
                <asp:ListItem Text="性别" Value="UserSex" Selected="True"></asp:ListItem>
                <asp:ListItem Text="政治面貌" Value="Party"></asp:ListItem>
                <asp:ListItem Text="身份证号" Value="IdCard"></asp:ListItem>
                <asp:ListItem Text="出生日期" Value="Birthday"></asp:ListItem>
                <asp:ListItem Text="籍贯" Value="Native"></asp:ListItem>
                <asp:ListItem Text="民族" Value="Nation"></asp:ListItem>
                <asp:ListItem Text="文化程度" Value="Education"></asp:ListItem>
                <asp:ListItem Text="手机号码" Value="Mobile" Selected="True"></asp:ListItem>
                <asp:ListItem Text="微信号码" Value="WeChat"></asp:ListItem>
                <asp:ListItem Text="任职日期" Value="PostDate"></asp:ListItem>
                <asp:ListItem Text="界别" Value="Subsector" Selected="True"></asp:ListItem>
                <asp:ListItem Text="专委会" Value="Committee" Selected="True"></asp:ListItem>
                <asp:ListItem Text="街道活动组" Value="StreetTeam" Selected="True"></asp:ListItem>
                <asp:ListItem Text="政协职务" Value="Role"></asp:ListItem>
                <asp:ListItem Text="港澳台委员" Value="HkMacaoTw"></asp:ListItem>
                <asp:ListItem Text="工作单位及职务" Value="OrgName"></asp:ListItem>
                <asp:ListItem Text="职称" Value="OrgPost"></asp:ListItem>
                <asp:ListItem Text="单位性质" Value="OrgType"></asp:ListItem>
                <asp:ListItem Text="单位地址" Value="OrgAddress"></asp:ListItem>
                <asp:ListItem Text="单位邮编" Value="OrgZip"></asp:ListItem>
                <asp:ListItem Text="单位电话" Value="OrgTel"></asp:ListItem>
                <asp:ListItem Text="社会职务" Value="SocietyDuty"></asp:ListItem>
                <asp:ListItem Text="家庭地址" Value="HomeAddress"></asp:ListItem>
                <asp:ListItem Text="家庭邮编" Value="HomeZip"></asp:ListItem>
                <asp:ListItem Text="家庭电话" Value="HomeTel"></asp:ListItem>
                <asp:ListItem Text="通讯地址及邮编" Value="ContactAddress"></asp:ListItem>
                <asp:ListItem Text="积分" Value="UserScore"></asp:ListItem>
                <asp:ListItem Text="状态" Value="Active"></asp:ListItem>
                <asp:ListItem Text="备注" Value="Remark"></asp:ListItem>
            </asp:CheckBoxList>
            <div class="cmd"><asp:Button ID="btnOutput" runat="server" Text="导出" OnClick="btnOutput_Click" /></div>
        </div>
    </asp:Panel>
    <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    <asp:HiddenField ID="hfBack" runat="server" Value="./" />
    <asp:HiddenField ID="hfCommittee" runat="server" /><asp:HiddenField ID="hfEducation" runat="server" />
    <uc1:ucHeader ID="header1" runat="server" />
    <asp:Panel ID="plMain" runat="server" CssClass="content">
        <div class="main">
            <asp:Panel ID="plNav" runat="server" Visible="false" CssClass="btn">
                <a href="?ac=query">检索委员信息</a>
                <a href="?id=0">新增委员信息</a>
                <a id="update" class="hide">编辑委员信息</a>
                <a id="score" class="hide">委员积分情况</a>
                <asp:HyperLink ID="lnkScore" runat="server" Visible="false" NavigateUrl="?ac=scoreadd">新增委员积分</asp:HyperLink>
            </asp:Panel>

            <asp:PlaceHolder ID="plQuery" runat="server" Visible="false">
<script>
    $(function () {
        function getQuery() {
            var url = '';
            if ($('#ddlQUserType').val()) {
                url += '&UserType=' + $('#ddlQUserType').val();
            }
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
            if ($('#ddlQEducation').val()) {
                url += '&Education=' + $('#ddlQEducation').val();
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
            //alert(encodeURI(url)); return false;
            return url;
        }
        $('#btnOut').click(function () {
            var url = window.location.href;
            if (url.indexOf('?') > 0) {
                url = url.substring(0, url.indexOf('?'));
            }
            url += '?ac=output' + getQuery();
            showDialog('委员信息 导出选项', url, '', 640, 350, 'no');
        });
        $('form').submit(function () {
            try {
                var url = getQuery();
                window.location.href = '?ac=query' + encodeURI(url);
            } catch (err) {
                //alert("验证出错，请稍后重试！");
            }
            return false;
        });
    });
</script>
                <div class="frm edit">
                    <strong>委员信息查询</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>委员编号</th>
                                <td><asp:TextBox ID="txtQUserCode" runat="server"></asp:TextBox></td>
                                <th>用户类型</th>
                                <td>
                                    <asp:DropDownList ID="ddlQUserType" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        <asp:ListItem Text="=在册委员=" Value="在册委员"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
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
                            <tr>
                                <th>籍贯</th>
                                <td><asp:TextBox ID="txtQNative" runat="server"></asp:TextBox></td>
                                <th>民族</th>
                                <td>
                                    <asp:DropDownList ID="ddlQNation" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>出生日期</th>
                                <td><asp:TextBox ID="txtQBirthday1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox> - <asp:TextBox ID="txtQBirthday2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtQBirthday1\')}'})"></asp:TextBox></td>
                                <th>任职日期</th>
                                <td><asp:TextBox ID="txtQPostDate1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox> - <asp:TextBox ID="txtQPostDate2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtQPostDate1\')}'})"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>政治面貌</th>
                                <td>
                                    <asp:DropDownList ID="ddlQParty" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th>文化程度</th>
                                <td>
                                    <asp:DropDownList ID="ddlQEducation" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>专委会</th>
                                <td>
                                    <asp:DropDownList ID="ddlQCommittee" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th>联系电话</th>
                                <td><asp:TextBox ID="txtQContactTel" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
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
                            <tr>
                                <th>工作单位及职务</th>
                                <td><asp:TextBox ID="txtQOrgName" runat="server" CssClass="long"></asp:TextBox></td>
                                <th>单位性质</th>
                                <td>
                                    <asp:DropDownList ID="ddlQOrgType" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>政协职务</th>
                                <td colspan="3">
                                    <asp:CheckBoxList ID="cblQRole" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
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
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plCheck" runat="server" Visible="false">
                <div class="btn">
                    <a class="cur">待审核委员信息</a>
                </div>
                <div class="line"></div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plList" runat="server" Visible="false">
                <div class="list hover">
                    <strong>结果展现
                        <span>符合条件的数据有：<b><asp:Literal ID="ltTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>委员编号</th>
                                <th>姓名</th>
                                <th>性别</th>
                                <th>籍贯</th>
                                <th>民族</th>
                                <th>政治面貌</th>
                                <th>手机</th>
                                <th>专委会</th>
                                <th>界别</th>
                                <th>街道活动组</th>
                                <th class="date">出生日期</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "UserCode")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "WxOpenId")%><%#DataBinder.Eval(Container.DataItem, "TrueName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "UserSex")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Native")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Nation")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Party")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Mobile")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Committee")%><br /><%#DataBinder.Eval(Container.DataItem, "Committee2")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Subsector")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "StreetTeam")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "BirthdayText")%></td>
                                        <td>
                                            <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看</u></a>
                                            <a href="?ac=edit&id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>修改</u></a>
                                            <a href="?ac=score&id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>积分情况</u></a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltNo" runat="server">
                                <tr><td class="no">暂时没有查询到信息！</td></tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plEdit" runat="server" Visible="false">
<script>
    function verOk(obj) {
        var $obj = $(obj).parent();
        var txt = $obj.find('span').html();
        switch ($obj.attr('id')) {
            case 'verUserSex':
                $obj.parent().find('input:radio').each(function () {
                    if ($(this).val() == txt) {
                        $(this).attr('checked', 'checked');
                    } else {
                        $(this).removeAttr('checked');
                    }
                });
                break;
            case 'verParty':
                $obj.parent().find('input:checkbox').each(function () {
                    if (txt.indexOf($(this).val()) >= 0) {
                        $(this).attr('checked', 'checked');
                    } else {
                        $(this).removeAttr('checked');
                    }
                });
                break;
            case 'verNation':
                $obj.parent().find('select').val(txt + '族');
                break;
            //case 'verEducation':
            case 'verHkMacaoTw':
            case 'verOrgType':
                $obj.parent().find('select').val(txt);
                break;
            case 'verRemark':
                $obj.parent().find('textarea').text(txt);
                break;
            default:
                $obj.parent().find('input').val(txt);
                break;
        }
        $obj.text('');
    }
    function verDel(obj) {
        $(obj).parent().text('');
    }
    $(function () {
        loadSelMenu('#hfEducation', '#txtEducation', '#Education', '');
        loadSelMenu('#hfCommittee', '#txtCommittee', '#Committee', '');
        loadSelMenu('#hfCommittee', '#txtCommittee2', '#Committee2', '');
        upFile('#txtPhoto', '#showPhoto', '#btnPhoto', 'photo');
        $('#txtIdCode').bind('keyup', function () {
            formatCard(this);
        }).blur(function () {
            formatCard(this);
        }).change(function () {
            checkCardCode(this, '#txtBirthday', 'rblUserSex');
        }).blur();
        $('#txtMobile').bind('keyup', function () {
            formatMobile(this);
        }).blur(function () {
            //formatMobile(this);
        }).change(function () {
            checkMobile(this);
        }).blur();
        $('#txtEmail').change(function () {
            if ($(this).val()) {
                checkEmail(this);
            }
        });
        $('#txtCheckText').focus(function () {
            $(this).blur();
        });
        if ($('#btnEdit').val()) {
            //$('#back').hide();
        } else {
            $('#reset').hide();
        }
        $('#back').click(function () {
            window.history.back(-1);
        });
        $('div.cmd>input:reset').click(function () {
            //$('#txtCheckText').text('UserSex=女\nParty=中共,民革\nMobile=18918966030\nNation=汉\nHkMacaoTw=香港委员\nOrgType=私营企业');
            var str = $('#txtCheckText').text();
            if (str) {
                var arr = str.split('\n');
                for (i = 0; i < arr.length; i++) {
                    if (arr[i] && arr[i].indexOf('=') > 0) {
                        var txt = arr[i].substring(0, arr[i].indexOf('='));
                        var val = arr[i].substring(arr[i].indexOf('=') + 1);
                        $('#ver' + txt).html('<b>待验证：</b><span>' + val + '</span><a onclick="verOk(this)">确认</a><a onclick="verDel(this)">删除</a>');
                    }
                }
            }
        }).click();
        $('#btnDel').click(function () {
            if (!confirm('您确定要“删除信息”吗?')) {
                return false;
            }
        });
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtTrueName') || checkRadio('#rblUserType') || ($('#rblUserType input:radio:checked').val() == '委员' && checkRadio('#rblUserSex')) || checkRadio('#rblActive')) {// || checkEmpty('#txtMobile')
                    return false;
                }
                var txt = '';
                $('div.check').each(function () {
                    var t = $(this).find('span').text();
                    if (t) {
                        t = t.replace(/\n/g, '[br]');
                        var id = $(this).attr('id');
                        id = id.substring(3);
                        if (txt) {
                            txt += '\n';
                        }
                        txt += id + '=' + t;
                    }
                });
                $('#txtCheckText').text(txt);
                //alert('pass'); return false;
                return true;
            } catch (err) {
                alert("验证出错，请稍后重试！");
                return false;
            }
        });
    });
</script>
                <div class="frm edit">
                    <strong><asp:Literal ID="ltTitle" runat="server" Text="新增委员信息"></asp:Literal></strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>自编号</th>
                                <td><asp:TextBox ID="txtId" runat="server" ReadOnly="True" CssClass="readonly" Text="0"></asp:TextBox>
                                    <asp:TextBox ID="txtOldId" runat="server" ReadOnly="True" CssClass="readonly" Text="0"></asp:TextBox>
                                </td>
                                <th>本年度积分</th>
                                <td><asp:TextBox ID="txtUserScore" runat="server" ReadOnly="true" CssClass="readonly" Text="0" ToolTip="本年度积分"></asp:TextBox>
                                    <asp:CheckBoxList ID="cblNoScore" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="不统计积分" Value="-1"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th>登录次数</th>
                                <td><asp:TextBox ID="txtLoginNum" runat="server" ReadOnly="true" CssClass="readonly" Text="0" ToolTip="登录次数"></asp:TextBox></td>
                                <th>会议活动得分</th>
                                <td><asp:TextBox ID="txtUserScore1" runat="server" ReadOnly="true" CssClass="readonly" Text="0"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>用户类别</th>
                                <td>
                                    <asp:RadioButtonList ID="rblUserType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="用户类型">
                                    </asp:RadioButtonList><%--第_届
                                    <asp:CheckBoxList ID="cblPeriod" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>--%>
                                </td>
                                <th>建言得分</th>
                                <td><asp:TextBox ID="txtUserScore2" runat="server" ReadOnly="true" CssClass="readonly" Text="0"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>委员编号</th>
                                <td><asp:TextBox ID="txtUserCode" runat="server" MaxLength="8" ToolTip="委员编号"></asp:TextBox></td>
                                <th>密码</th>
                                <td><asp:TextBox ID="txtUserPwd" runat="server" TextMode="Password" MaxLength="20" ToolTip="密码"></asp:TextBox>
                                    <i>(不修改时请留空)</i>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>姓名</th>
                                <td><asp:TextBox ID="txtTrueName" runat="server" MaxLength="20" ToolTip="姓名"></asp:TextBox>
                                    <i><asp:Literal ID="ltUserName" runat="server"></asp:Literal></i>
                                </td>
                                <th rowspan="8">照片</th>
                                <td>
                                    <asp:TextBox ID="txtPhoto" runat="server" ToolTip="照片" CssClass="long"></asp:TextBox>
                                    <a id="btnPhoto" href="#" class="btn"><u>上传</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th>性别</th>
                                <td>
                                    <asp:RadioButtonList ID="rblUserSex" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="性别">
                                        <asp:ListItem Text="男"></asp:ListItem>
                                        <asp:ListItem Text="女"></asp:ListItem>
                                    </asp:RadioButtonList>
                                    <div id="verUserSex" class="check"></div>
                                </td>
                                <td rowspan="7" id="showPhoto" class="pic"></td>
                            </tr>
                            <tr>
                                <th>身份证号</th>
                                <td><asp:TextBox ID="txtIdCard" runat="server" MaxLength="25" ToolTip="身份证"></asp:TextBox>
                                    <div id="verIdCard" class="check"></div>
                                </td>
                            </tr>
                            <tr>
                                <th>出生日期</th>
                                <td><asp:TextBox ID="txtBirthday" runat="server" MaxLength="10" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox>
                                    <div id="verBirthday" class="check"></div>
                                </td>
                            </tr>
                            <tr>
                                <th>籍贯</th>
                                <td><asp:TextBox ID="txtNative" runat="server" MaxLength="20" ToolTip="籍贯"></asp:TextBox>
                                    <div id="verNative" class="check"></div>
                                </td>
                            </tr>
                            <tr>
                                <th>民族</th>
                                <td>
                                    <asp:DropDownList ID="ddlNation" runat="server" ToolTip="民族">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                    <div id="verNation" class="check"></div>
                                </td>
                            </tr>
                            <tr>
                                <th>文化程度</th>
                                <td>
                                    <asp:DropDownList ID="ddlEducation" runat="server" Visible="false" ToolTip="文化程度">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:TextBox ID="txtEducation" runat="server" ToolTip="文化程度"></asp:TextBox>
                                    <div id="Education" class="selmenu"></div>
                                    <div id="verEducation" class="check"></div>
                                </td>
                            </tr>
                            <tr>
                                <th>手机号码</th>
                                <td><asp:TextBox ID="txtMobile" runat="server" MaxLength="50" ToolTip="手机号码"></asp:TextBox>
                                    <div id="verMobile" class="check"></div>
                                </td>
                            </tr>
                            <tr>
                                <th>微信号码</th>
                                <td><asp:TextBox ID="txtWeChat" runat="server" MaxLength="50" ToolTip="微信号码"></asp:TextBox>
                                    <div id="verWeChat" class="check"></div>
                                </td>
                                <th>委员任职日期</th>
                                <td><asp:TextBox ID="txtPostDate" runat="server" MaxLength="10" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})" ToolTip="委员任职日期"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>政治面貌</th>
                                <td>
                                    <asp:CheckBoxList ID="cblParty" runat="server" RepeatDirection="Horizontal" RepeatColumns="4" Width="100%" ToolTip="政治面貌"></asp:CheckBoxList>
                                    <div id="verParty" class="check"></div>
                                </td>
                                <th>政协职务</th>
                                <td>
                                    <asp:CheckBoxList ID="cblRole" runat="server" RepeatDirection="Horizontal" RepeatColumns="4" Width="100%" ToolTip="政协职务"></asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th>专委会</th>
                                <td>
                                    <asp:DropDownList ID="ddlCommittee" runat="server" Visible="false" ToolTip="专委会">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:TextBox ID="txtCommittee" runat="server" ToolTip="专委会"></asp:TextBox>
                                    <div id="Committee" class="selmenu"></div>
                                </td>
                                <th>专委会2</th>
                                <td>
                                    <asp:DropDownList ID="ddlCommittee2" runat="server" Visible="false" ToolTip="专委会2"></asp:DropDownList>
                                    <asp:TextBox ID="txtCommittee2" runat="server" ToolTip="专委会2"></asp:TextBox>
                                    <div id="Committee2" class="selmenu"></div>
                                </td>
                            </tr>
                            <tr>
                                <th>界别</th>
                                <td>
                                    <asp:DropDownList ID="ddlSubsector" runat="server" ToolTip="界别">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th>街道活动组</th>
                                <td>
                                    <asp:DropDownList ID="ddlStreetTeam" runat="server" ToolTip="街道活动组">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>港澳台委员</th>
                                <td>
                                    <asp:DropDownList ID="ddlHkMacaoTw" runat="server" ToolTip="港澳台委员">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                    <div id="verHkMacaoTw" class="check"></div>
                                </td>
                                <th>职称</th>
                                <td><asp:TextBox ID="txtOrgPost" runat="server" MaxLength="20" ToolTip="职称"></asp:TextBox>
                                    <div id="verOrgPost" class="check"></div>
                                </td>
                            </tr>
                            <tr>
                                <th>工作单位及职务</th>
                                <td><asp:TextBox ID="txtOrgName" runat="server" MaxLength="100" ToolTip="工作单位及职务" CssClass="long"></asp:TextBox>
                                    <div id="verOrgName" class="check"></div>
                                </td>
                                <th>单位性质</th>
                                <td>
                                    <asp:DropDownList ID="ddlOrgType" runat="server" ToolTip="单位性质">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                    <div id="verOrgType" class="check"></div>
                                </td>
                            </tr>
                            <tr>
                                <th>单位地址</th>
                                <td><asp:TextBox ID="txtOrgAddress" runat="server" MaxLength="100" ToolTip="单位地址" CssClass="long"></asp:TextBox>
                                    <div id="verOrgAddress" class="check"></div>
                                </td>
                                <th>单位邮编</th>
                                <td><asp:TextBox ID="txtOrgZip" runat="server" MaxLength="6" ToolTip="单位邮编"></asp:TextBox>
                                    <div id="verOrgZip" class="check"></div>
                                </td>
                            </tr>
                            <tr>
                                <th>单位电话</th>
                                <td><asp:TextBox ID="txtOrgTel" runat="server" MaxLength="50" ToolTip="单位电话"></asp:TextBox>
                                    <div id="verOrgTel" class="check"></div>
                                </td>
                                <th>社会职务</th>
                                <td><asp:TextBox ID="txtSocietyDuty" runat="server" MaxLength="50" ToolTip="社会职务" CssClass="long"></asp:TextBox>
                                    <div id="verSocietyDuty" class="check"></div>
                                </td>
                            </tr>
                            <tr>
                                <th>家庭地址</th>
                                <td><asp:TextBox ID="txtHomeAddress" runat="server" MaxLength="100" ToolTip="家庭地址" CssClass="long"></asp:TextBox>
                                    <div id="verHomeAddress" class="check"></div>
                                </td>
                                <th>家庭邮编</th>
                                <td><asp:TextBox ID="txtHomeZip" runat="server" MaxLength="6" ToolTip="家庭邮编"></asp:TextBox>
                                    <div id="verHomeZip" class="check"></div>
                                </td>
                            </tr>
                            <tr>
                                <th>家庭电话</th>
                                <td><asp:TextBox ID="txtHomeTel" runat="server" MaxLength="50" ToolTip="家庭电话"></asp:TextBox>
                                    <div id="verHomeTel" class="check"></div>
                                </td>
                                <th>通讯地址及邮编</th>
                                <td><asp:TextBox ID="txtContactAddress" runat="server" MaxLength="100" ToolTip="通讯地址及邮编" CssClass="long"></asp:TextBox>
                                    <div id="verContactAddress" class="check"></div>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>状态</th>
                                <td>
                                    <asp:RadioButtonList ID="rblActive" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="在册" Value="1" Selected="True"></asp:ListItem>
                                        <%--<asp:ListItem Text="锁定" Value="-1"></asp:ListItem>--%>
                                        <asp:ListItem Text="删除" Value="-444"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                                <th>密码错误次数</th>
                                <td><asp:TextBox ID="txtErrNum" runat="server" MaxLength="4" ToolTip="密码错误次数" Text="0"></asp:TextBox>
                                    <i> (大于10时，用户自动锁定)</i>
                                </td>
                            </tr>
                            <tr>
                                <th rowspan="2">备注</th>
                                <td rowspan="2"><asp:TextBox ID="txtRemark" runat="server" TextMode="MultiLine" ToolTip="备注" CssClass="long" Rows="3"></asp:TextBox></td>
                                <th>最后登录时间</th>
                                <td><asp:TextBox ID="txtLastTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>最后登录IP</th>
                                <td><asp:TextBox ID="txtLastIp" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加时间</th>
                                <td><asp:TextBox ID="txtAddTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th rowspan="6">待验证字段</th>
                                <td rowspan="6"><asp:TextBox ID="txtCheckText" runat="server" TextMode="MultiLine" Rows="10" CssClass="readonly long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加IP</th>
                                <td><asp:TextBox ID="txtAddIp" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加人</th>
                                <td><asp:TextBox ID="txtAddUser" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>修改时间</th>
                                <td><asp:TextBox ID="txtUpTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>修改IP</th>
                                <td><asp:TextBox ID="txtUpIp" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>修改人</th>
                                <td><asp:TextBox ID="txtUpUser" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnEdit" runat="server" Text="新增" OnClick="btnEdit_Click" />
                        <asp:Button ID="btnDel" runat="server" Text="删除" Visible="false" OnClick="btnDel_Click" />
                        <input id="reset" type="reset" value="重填" />
                        <input id="back" type="button" value="返回" />
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plScore" runat="server" Visible="false">
                <div class="frm edit">
                    <strong>委员信息</strong>
                    <table>
                        <thead>
                            <tr>
                                <th>委员编号</th>
                                <td><asp:Literal ID="ltUserCode" runat="server"></asp:Literal></td>
                                <th>姓名</th>
                                <td><asp:Literal ID="ltTrueName" runat="server"></asp:Literal></td>
                                <th>累计积分</th>
                                <td><asp:Literal ID="ltScore" runat="server">0</asp:Literal></td>
                                <th>会议活动得分</th>
                                <td><asp:Literal ID="ltScore1" runat="server">0</asp:Literal></td>
                                <th>建言得分</th>
                                <td><asp:Literal ID="ltScore2" runat="server">0</asp:Literal></td>
                            </tr>
                        </thead>
                    </table>
                </div>
                <div class="cmd cmd2">
<script>
    $(function () {
        $('#ddlYear').change(function () {
            var url = window.location.href;
            if (url.indexOf("&time=") > 0) {
                url = url.substring(0, url.indexOf("&time="));
            }
            url += '&time=' + $(this).val();
            window.location.replace(url);
        });
    });
</script>
                    <asp:DropDownList ID="ddlYear" runat="server"></asp:DropDownList>
                </div>
                <div class="cmd">
                    <asp:PlaceHolder ID="plAddScore" runat="server" Visible="false">
<script>
    $(function () {
        $('#btnAddScore').click(function () {
            try {
                if (checkEmpty('#txtScore') || checkEmpty('#txtScoreTitle')) {
                    return false;
                }
            } catch (err) {
                alert("验证出错，请稍后重试！");
                return false;
            }
        });
    });
</script>
                        积分<asp:TextBox ID="txtScore" runat="server" CssClass="num" MaxLength="4" Text="0" ToolTip="积分"></asp:TextBox>
                        说明<asp:TextBox ID="txtScoreTitle" runat="server" CssClass="txt" MaxLength="50" ToolTip="积分说明"></asp:TextBox>
                        时间<asp:TextBox ID="txtScoreTime" runat="server" MaxLength="20" ToolTip="积分时间" CssClass="Wdate txt" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})"></asp:TextBox>
                        备注<asp:TextBox ID="txtScoreRemark" runat="server" CssClass="txt" MaxLength="200" ToolTip="备注"></asp:TextBox>
                        <asp:Button ID="btnAddScore" runat="server" Text="新增积分" OnClick="btnAddScore_Click" />
                    </asp:PlaceHolder>
                </div>
<script>
    $(function () {
        $('.list>table>tbody>tr>td>a').click(function () {
            if (!confirm('您确定要“' + $(this).text() + '”吗？')) {
                return false;
            }
        });
    });
</script>
                <div class="list">
                    <strong>委员积分情况
                        <span>符合条件的数据有：<b><asp:Literal ID="ltScoreTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>积分说明</th>
                                <th class="state">积分</th>
                                <th class="time">得分时间</th>
                                <th class="time">表名</th>
                                <th class="state">表Id</th>
                                <th class="state">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpScoreList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%>
                                            <p><%#DataBinder.Eval(Container.DataItem, "other")%></p>
                                        </td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ScoreText")%></td>
                                        <td align="center" title="添加时间：<%#DataBinder.Eval(Container.DataItem, "AddTime", "{0:yyyy/MM/dd HH:mm:ss}")%>"><%#DataBinder.Eval(Container.DataItem, "GetTime", "{0:yyyy/MM/dd HH:mm:ss}")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "TableName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "TableId")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "BtnEdit")%></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltScoreNo" runat="server">
                                <tr><td class="no">暂时没有查询到信息！</td></tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblScoreNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plScoreAdd" runat="server" Visible="false">
<script>
    $(function () {
        $('form').submit(function () {
            try {
                if (checkEmpty('#sUsers') || checkEmpty('#sScore') || checkEmpty('#sScoreTitle')) {
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
                    <strong>新增委员积分</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th><b>*</b>委员</th>
                                <td colspan="3"><asp:TextBox ID="sUsers" runat="server" CssClass="long" ToolTip="委员姓名"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>积分</th>
                                <td><asp:TextBox ID="sScore" runat="server" CssClass="num" MaxLength="4" Text="0" ToolTip="积分"></asp:TextBox></td>
                                <th><b>*</b>说明</th>
                                <td><asp:TextBox ID="sScoreTitle" runat="server" CssClass="long" MaxLength="50" ToolTip="积分说明"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>时间</th>
                                <td><asp:TextBox ID="sScoreTime" runat="server" MaxLength="20" ToolTip="积分时间" CssClass="Wdate txt" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})"></asp:TextBox></td>
                                <th>备注</th>
                                <td><asp:TextBox ID="sScoreRemark" runat="server" CssClass="long" MaxLength="200" ToolTip="备注"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnScoreAdd" runat="server" Text="新增积分" OnClick="btnScoreAdd_Click" />
                    </div>
                </div>
                <asp:Label ID="lblScoreAdd" runat="server"></asp:Label>
            </asp:PlaceHolder>
        </div>
    </asp:Panel>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
