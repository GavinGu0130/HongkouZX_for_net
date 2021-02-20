<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="user.aspx.cs" Inherits="hkzx.web.cn.user" %><%--Tony维护--%>
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
    <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    <asp:HiddenField ID="hfBack" runat="server" />
    <uc1:ucHeader ID="header1" runat="server" />
    <div class="content">
        <div class="main">
            <asp:PlaceHolder ID="plUser" runat="server" Visible="false">
<script>
    $(function () {
        upFile('#hfPhoto', '#showPhoto', '', 'photo');
        $('#lnkUserScore').click(function () {
            showDialog('我的积分明细', $(this).attr('href'), '', 640, 480, 'yes');
            return false;
        });
        $('#txtIdCard').bind('keyup', function () {
            formatCard(this);
        }).blur(function () {
            formatCard(this);
        }).change(function () {
            checkCardCode(this, '#txtBirthday', 'rblUserSex');
        }).blur();
        $('#txtMobile').bind('keyup', function () {
            formatMobile(this);
        }).blur(function () {
            formatMobile(this);
        }).change(function () {
            checkMobile(this);
        }).blur();
        $('.cmd>input:reset').click(function () {
            var str = $('#txtCheckText').text();
            if (str) {
                var arr = str.split('\n');
                for (i = 0; i < arr.length; i++) {
                    if (arr[i] && arr[i].indexOf('=') > 0) {
                        var txt = arr[i].substring(0, arr[i].indexOf('='));
                        var val = arr[i].substring(arr[i].indexOf('=') + 1);
                        $('#ver' + txt).html('<b>待验证：</b><span>' + val + '</span>');
                    }
                }
            }
        }).click();
        $('form').submit(function () {
            try {
                if (($('#txtUserType').val() == '委员' && checkRadio('#rblUserSex')) || ($('#txtIdCard').val() && checkCardCode('#txtIdCard')) || ($('#txtBirthday').val() && checkDate('#txtBirthday'))) {// || ($('#txtMobile').val() && checkMobile('#txtMobile'))
                    return false;
                }
                return true;
            } catch (err) {
                alert("验证出错，请稍后重试！");
                return false;
            }
        });
    });
</script>
                <div class="frm edit">
                    <strong>我的信息</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>委员编号</th>
                                <td><asp:TextBox ID="txtUserCode" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>用户类型</th>
                                <td><asp:TextBox ID="txtUserType" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>姓名</th>
                                <td><asp:TextBox ID="txtTrueName" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>年度积分</th>
                                <td><asp:TextBox ID="txtUserScore" runat="server" ReadOnly="true" CssClass="readonly" Text="0"></asp:TextBox>&nbsp;<asp:HyperLink ID="lnkUserScore" runat="server" NavigateUrl="dialog.aspx?ac=score&view=my&UserId=" CssClass="btn"><u>积分明细</u></asp:HyperLink>
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
                                <th rowspan="7">照片</th>
                                <asp:HiddenField ID="hfPhoto" runat="server" />
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
                                <td><asp:TextBox ID="txtBirthday" runat="server" MaxLength="10" ToolTip="出生日期" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox>
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
                                    <asp:DropDownList ID="ddlEducation" runat="server" ToolTip="文化程度">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
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
                                <td><asp:TextBox ID="txtPostDate" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>政治面貌</th>
                                <td>
                                    <asp:DropDownList ID="ddlParty" runat="server" ToolTip="政治面貌">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                    <div id="verParty" class="check"></div>
                                </td>
                                <th>政协职务</th>
                                <td><asp:TextBox ID="txtRole" runat="server" ReadOnly="true" CssClass="readonly long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>专委会</th>
                                <td><asp:TextBox ID="txtCommittee" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th></th>
                                <td></td>
                            </tr>
                            <tr>
                                <th>界别</th>
                                <td><asp:TextBox ID="txtSubsector" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>街道活动组</th>
                                <td><asp:TextBox ID="txtStreetTeam" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>港澳台委员</th>
                                <td><asp:TextBox ID="txtHkMacaoTw" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
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
                        </tbody>
                    </table>
                    <asp:Panel ID="plCmd" runat="server" CssClass="cmd">
                        <asp:Button ID="btnEdit" runat="server" Text="修改" OnClick="btnEdit_Click" />
                        <input id="reset" type="reset" value="重填" />
                    </asp:Panel>
                    <asp:TextBox ID="txtCheckText" runat="server" TextMode="MultiLine" style="display:none;"></asp:TextBox>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
