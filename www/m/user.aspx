<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="user.aspx.cs" Inherits="hkzx.web.m.user" %><%--Tony维护--%>
<%@ Register src="../cn/ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="ucHeader.ascx" tagname="ucHeader" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" Client="wap" />
    <title>虹口政协履职通</title>
</head>
<body>
<form id="form1" runat="server">
    <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    <uc1:ucHeader ID="header1" runat="server" Title="虹口区政协委员履职系统" />
    <div class="content main">
        <asp:PlaceHolder ID="plUser" runat="server" Visible="false">
<script>
    $(function () {
        upFile('#hfPhoto', '#showPhoto', '', 'photo');
        //$('#lnkUserScore').click(function () {
        //    showDialog('我的积分明细', $(this).attr('href'), '', 320, 480, 'yes');
        //    return false;
        //});
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
            <div class="edit">
                <dl class="user">
                    <dt>
                        <asp:HiddenField ID="hfPhoto" runat="server" />
                        <span id="showPhoto" class="photo"></span>
                    </dt>
                    <dd>
                        <b>委员编号</b>
                        <asp:TextBox ID="txtUserCode" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox>
                    </dd>
                    <dd>
                        <b>用户类型</b>
                        <asp:TextBox ID="txtUserType" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox>
                    </dd>
                    <dd>
                        <b>姓名</b>
                        <asp:TextBox ID="txtTrueName" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox>
                    </dd>
                    <dd>
                        <b>年度积分</b>
                        <asp:TextBox ID="txtUserScore" runat="server" ReadOnly="true" CssClass="readonly" Text="0"></asp:TextBox>&nbsp;<asp:HyperLink ID="lnkUserScore" runat="server" NavigateUrl="../cn/dialog.aspx?ac=score&view=my&Titler=我的积分明细&UserId=" CssClass="btn"><u>明细</u></asp:HyperLink>
                    </dd>
                    <dd>
                        <b>性别</b>
                        <asp:RadioButtonList ID="rblUserSex" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="性别">
                            <asp:ListItem Text="男"></asp:ListItem>
                            <asp:ListItem Text="女"></asp:ListItem>
                        </asp:RadioButtonList>
                        <div id="verUserSex" class="check"></div>
                    </dd>
                    <dd>
                        <b>身份证号</b>
                        <asp:TextBox ID="txtIdCard" runat="server" MaxLength="25" ToolTip="身份证" Width="190px"></asp:TextBox>
                        <div id="verIdCard" class="check"></div>
                    </dd>
                    <dd>
                        <b>出生日期</b>
                        <asp:TextBox ID="txtBirthday" runat="server" MaxLength="10" CssClass="Wdate" ToolTip="出生日期" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox>
                        <div id="verBirthday" class="check"></div>
                    </dd>
                    <dd>
                        <b>籍贯</b>
                        <asp:TextBox ID="txtNative" runat="server" MaxLength="20" ToolTip="籍贯"></asp:TextBox>
                        <div id="verNative" class="check"></div>
                    </dd>
                    <dd>
                        <b>民族</b>
                        <asp:DropDownList ID="ddlNation" runat="server" ToolTip="民族">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                        <div id="verNation" class="check"></div>
                    </dd>
                    <dd>
                        <b>文化程度</b>
                        <asp:DropDownList ID="ddlEducation" runat="server" ToolTip="文化程度">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                        <div id="verEducation" class="check"></div>
                    </dd>
                    <dd>
                        <b>手机号码</b>
                        <asp:TextBox ID="txtMobile" runat="server" MaxLength="50" ToolTip="手机号码"></asp:TextBox>
                        <div id="verMobile" class="check"></div>
                    </dd>
                    <dd>
                        <b>微信号码</b>
                        <asp:TextBox ID="txtWeChat" runat="server" MaxLength="50" ToolTip="微信号码"></asp:TextBox>
                        <div id="verWeChat" class="check"></div>
                    </dd>
                    <dd>
                        <b>政治面貌</b>
                        <asp:DropDownList ID="ddlParty" runat="server" ToolTip="政治面貌">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                        <div id="verParty" class="check"></div>
                    </dd>
                    <dd>
                        <b>委员任职日期</b>
                        <asp:TextBox ID="txtPostDate" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox>
                    </dd>
                    <dd>
                        <b>政协职务</b>
                        <asp:TextBox ID="txtRole" runat="server" ReadOnly="true" CssClass="readonly long"></asp:TextBox>
                    </dd>
                    <dd>
                        <b>专委会</b>
                        <asp:TextBox ID="txtCommittee" runat="server" ReadOnly="true" CssClass="readonly long"></asp:TextBox>
                    </dd>
                    <dd>
                        <b>界别</b>
                        <asp:TextBox ID="txtSubsector" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox>
                    </dd>
                    <dd>
                        <b>街道活动组</b>
                        <asp:TextBox ID="txtStreetTeam" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox>
                    </dd>
                    <dd>
                        <b>港澳台委员</b>
                        <asp:TextBox ID="txtHkMacaoTw" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox>
                    </dd>
                    <dd>
                        <b>职称</b>
                        <asp:TextBox ID="txtOrgPost" runat="server" MaxLength="20" ToolTip="职称"></asp:TextBox>
                        <div id="verOrgPost" class="check"></div>
                    </dd>
                    <dd>
                        <b>工作单位及职务</b>
                        <asp:TextBox ID="txtOrgName" runat="server" MaxLength="100" ToolTip="工作单位及职务" CssClass="long"></asp:TextBox>
                        <div id="verOrgName" class="check"></div>
                    </dd>
                    <dd>
                        <b>单位性质</b>
                        <asp:DropDownList ID="ddlOrgType" runat="server" ToolTip="单位性质">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                        <div id="verOrgType" class="check"></div>
                    </dd>
                    <dd>
                        <b>单位地址</b>
                        <asp:TextBox ID="txtOrgAddress" runat="server" MaxLength="100" ToolTip="单位地址" CssClass="long"></asp:TextBox>
                        <div id="verOrgAddress" class="check"></div>
                    </dd>
                    <dd>
                        <b>单位邮编</b>
                        <asp:TextBox ID="txtOrgZip" runat="server" MaxLength="6" ToolTip="单位邮编"></asp:TextBox>
                        <div id="verOrgZip" class="check"></div>
                    </dd>
                    <dd>
                        <b>单位电话</b>
                        <asp:TextBox ID="txtOrgTel" runat="server" MaxLength="50" ToolTip="单位电话"></asp:TextBox>
                        <div id="verOrgTel" class="check"></div>
                    </dd>
                    <dd>
                        <b>社会职务</b>
                        <asp:TextBox ID="txtSocietyDuty" runat="server" MaxLength="50" ToolTip="社会职务" CssClass="long"></asp:TextBox>
                        <div id="verSocietyDuty" class="check"></div>
                    </dd>
                    <dd>
                        <b>家庭地址</b>
                        <asp:TextBox ID="txtHomeAddress" runat="server" MaxLength="100" ToolTip="家庭地址" CssClass="long"></asp:TextBox>
                        <div id="verHomeAddress" class="check"></div>
                    </dd>
                    <dd>
                        <b>家庭邮编</b>
                        <asp:TextBox ID="txtHomeZip" runat="server" MaxLength="6" ToolTip="家庭邮编"></asp:TextBox>
                        <div id="verHomeZip" class="check"></div>
                    </dd>
                    <dd>
                        <b>家庭电话</b>
                        <asp:TextBox ID="txtHomeTel" runat="server" MaxLength="50" ToolTip="家庭电话"></asp:TextBox>
                        <div id="verHomeTel" class="check"></div>
                    </dd>
                    <dd>
                        <b>通讯地址及邮编</b>
                        <asp:TextBox ID="txtContactAddress" runat="server" MaxLength="100" ToolTip="通讯地址及邮编" CssClass="long"></asp:TextBox>
                        <div id="verContactAddress" class="check"></div>
                    </dd>
                </dl>
                <asp:Panel ID="plCmd" runat="server" CssClass="cmd">
                    <asp:Button ID="btnEdit" runat="server" Text="修改" OnClick="btnEdit_Click" />
                    <input id="reset" type="reset" value="重填" />
                </asp:Panel>
                <asp:TextBox ID="txtCheckText" runat="server" TextMode="MultiLine" style="display:none;"></asp:TextBox>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plQrcode" runat="server" Visible="false">
            <div class="qrcode">
<script type="text/javascript" src="../inc/jquery.qrcode.min.js"></script>
<script>
    $(function () {
        var coder;
        $('#refresh').click(function () {
            clearTimeout(coder);
            var This = this;
            $(This).hide();
            $('#loading').show();
            $('#qrcode').text('');
            $.get('user.aspx?ac=token', {}, function () { })
                .success(function (data, textStatus, jqXHR) {
                    if (data == '') {
                        //$('#dynamic>dd').html('请稍后<a href="javascript:dynamicSignup();">重试</a>，谢谢！');
                    } else {
                        $('#qrcode').qrcode({
                            render: 'canvas',//也可以替换为table
                            width: 220,
                            height: 220,
                            text: data
                        });
                    }
                })
                .error(function () { })
                .complete(function () {
                    $('#loading').hide();
                    coder = setTimeout(function () { $(This).show(); }, 60000);//定时器
                });
        }).click();//
    });
</script>
                <div id="qrcode"></div>
                <asp:Label ID="lblUser" runat="server"></asp:Label>
                <a id="refresh" class="load">请刷新二维码</a>
                <div id="loading"><img src="../image/loading.gif" /></div>
            </div>
        </asp:PlaceHolder>
    </div>
</form>
</body>
</html>
