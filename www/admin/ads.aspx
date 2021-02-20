<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ads.aspx.cs" Inherits="hkzx.web.admin.ads" %><%--Tony维护--%>
<%@ Register src="../cn/ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="ucHeader.ascx" tagname="ucHeader" tagprefix="uc1" %>
<%@ Register src="ucFooter.ascx" tagname="ucFooter" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" Client="admin" />
    <title>虹口政协履职通 - 管理系统</title>
    <script src="../inc/md5.js"></script>
</head>
<body>
<form id="form1" runat="server">
    <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    <asp:HiddenField ID="hfBack" runat="server" />
    <uc1:ucHeader ID="header1" runat="server" />
    <div class="content">
        <asp:PlaceHolder ID="plPwd" runat="server" Visible="false">
<script>
    $(function () {
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtOld') || checkEmpty('#txtNew', 6, 20, 2)) {
                    return false;
                }
                if ($('#txtNew').val() != $('#txtNew2').val()) {
                    alert('两次输入的[密码]不一致');
                    $('#txtNew2').focus();
                    return false;
                }
                $('#txtOld').val(CryptoJS.MD5(encodeURIComponent($('#txtOld').val())));
                return true;
            } catch (err) {
                //alert("验证出错，请稍后重试！");
                return false;
            }
        });
    });
</script>
            <div class="login login2">
                <dl>
                    <dt>修改密码</dt>
                    <dd class="txt"><b>用 户 名：</b><asp:TextBox ID="txtName" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></dd>
                    <dd class="txt"><b>旧 密 码：</b><asp:TextBox ID="txtOld" runat="server" MaxLength="20" TextMode="Password" ToolTip="旧密码" AutoPostBack="false"></asp:TextBox></dd>
                    <dd class="txt"><b>新 密 码：</b><asp:TextBox ID="txtNew" runat="server" MaxLength="20" TextMode="Password" ToolTip="新密码" AutoPostBack="false"></asp:TextBox></dd>
                    <dd class="txt"><b>再输一遍：</b><asp:TextBox ID="txtNew2" runat="server" MaxLength="20" TextMode="Password" ToolTip="再输一遍" AutoPostBack="false"></asp:TextBox></dd>
                    <dd class="btn"><asp:Button ID="btnPwd" runat="server" Text="修改" OnClick="btnPwd_Click" /><input type="button" value="返回" onclick="history.back(-1);" /></dd>
                </dl>
            </div>
        </asp:PlaceHolder>
        <div class="main">
            <asp:PlaceHolder ID="plInfo" runat="server" Visible="false">
<script>
    $(function () {
        formatCard('#IdCode');
        formatMobile('#Mobile');
    });
</script>
                <div class="frm edit">
                    <strong>我的信息</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th><b>*</b>登录名</th>
                                <td><asp:Literal ID="ltAdminName" runat="server"></asp:Literal></td>
                                <th>自编号</th>
                                <td><asp:Literal ID="ltId" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th><b>*</b>姓名</th>
                                <td><asp:Literal ID="ltTrueName" runat="server"></asp:Literal></td>
                                <th rowspan="7">照片</th>
                                <td rowspan="7" class="pic"><asp:Literal ID="ltPhoto" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th><b>*</b>性别</th>
                                <td><asp:Literal ID="ltUserSex" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>身份证号</th>
                                <td id="IdCode"><asp:Literal ID="ltIdCode" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>管理级别</th>
                                <td><asp:Literal ID="ltGrade" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>管理权限</th>
                                <td><asp:Literal ID="ltPowers" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>部门职(岗)位</th>
                                <td><asp:Literal ID="ltDepPost" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>办公室电话</th>
                                <td><asp:Literal ID="ltOfficeTel" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th><b>*</b>手机号码</th>
                                <td id="Mobile"><asp:Literal ID="ltMobile" runat="server"></asp:Literal></td>
                                <th>电子邮箱</th>
                                <td><asp:Literal ID="ltEmail" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th><b>*</b>状态</th>
                                <td><asp:Literal ID="ltActive" runat="server"></asp:Literal></td>
                                <th>密码错误次数</th>
                                <td><asp:Literal ID="ltErrNum" runat="server" Text="0"></asp:Literal>
                                    <i>(大于10时，用户自动锁定)</i>
                                </td>
                            </tr>
                            <tr>
                                <th rowspan="2">备注</th>
                                <td rowspan="2"><asp:Literal ID="ltRemark" runat="server"></asp:Literal></td>
                                <th>最后登录时间</th>
                                <td><asp:Literal ID="ltLastTime" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>最后登录IP</th>
                                <td><asp:Literal ID="ltLastIp" runat="server"></asp:Literal></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </asp:PlaceHolder>
            
            <asp:PlaceHolder ID="plNav" runat="server" Visible="false">
<script>
    $(function () {
        var url = window.location.href;
        if (url.indexOf('id=') < 0) {
            $('#nav>a:first').addClass('cur').removeAttr('href');
        } else if (url.indexOf('id=0') > 0) {
            $('#nav>a[href*="id=0"]').addClass('cur').removeAttr('href');
        } else {
            $('#nav>a.hide').show().addClass('cur');
        }
    });
</script>
                <div id="nav" class="btn">
                    <a href="?ac=manage">后台用户查询</a>
                    <a href="?ac=manage&id=0">新增后台用户</a>
                    <a class="hide">修改后台用户</a>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plList" runat="server" Visible="false">
<script>
    $(function () {
        if ($('.list>table>tbody>tr>td.no').text()) {
            $('.list>table>tbody>tr>td.no').attr('colspan', $('.list>table>thead>tr>th').length);
            $('.list>table>thead').hide();
        }
        $('#clean').click(function () {
            $('#txtQUserName').val('');
            $('#txtQTrueName').val('');
            $('#txtQMobile').val('');
            $('#txtQDepPost').val('');
        });
        $('form').submit(function () {
            try {
                var url = '';
                if ($('#txtQUserName').val()) {
                    url += '&UserName=' + $('#txtQUserName').val();
                }
                if ($('#txtQTrueName').val()) {
                    url += '&TrueName=' + $('#txtQTrueName').val();
                }
                if ($('#txtQMobile').val()) {
                    url += '&Mobile=' + $('#txtQMobile').val();
                }
                if ($('#txtQDepPost').val()) {
                    url += '&DepPost=' + $('#txtQDepPost').val();
                }
                //alert(encodeURI(url)); return false;
                window.location.href = '?ac=manage' + encodeURI(url);
                return false;
            } catch (err) {
                alert("验证出错，请稍后重试！");
                return false;
            }
        });
    });
</script>
                <div class="frm edit">
                    <strong>后台用户查询</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>用户名</th>
                                <td><asp:TextBox ID="txtQUserName" runat="server"></asp:TextBox></td>
                                <th>姓名</th>
                                <td><asp:TextBox ID="txtQTrueName" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>联系电话</th>
                                <td><asp:TextBox ID="txtQMobile" runat="server"></asp:TextBox></td>
                                <th>部门职(岗)位</th>
                                <td><asp:TextBox ID="txtQDepPost" runat="server"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <input id="btnQuery" type="submit" value="查询" />
                        <input id="clean" type="reset" value="清空" />
                    </div>
                </div>
                <div class="list">
                    <strong>结果展现
                        <span>符合条件的数据有：<b><asp:Literal ID="ltTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>登录名</th>
                                <th>姓名</th>
                                <th>性别</th>
                                <th>部门职(岗)位</th>
                                <th>办公室电话</th>
                                <th>手机</th>
                                <th>电子邮箱</th>
                                <th>权限</th>
                                <th class="num">状态</th>
                                <th class="date">登录时间</th>
                                <th class="cmd">登录Ip</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpList" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "AdminName")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "TrueName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "UserSex")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "DepPost")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OfficeTel")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Mobile")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Email")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Powers")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "LastTimeText")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "LastIp")%></td>
                                        <td><a href="?ac=manage&id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>修改</u></a></td>
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
    $(function () {
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
            formatMobile(this);
        }).change(function () {
            checkMobile(this);
        }).blur();
        $('#txtEmail').change(function () {
            if ($(this).val()) {
                checkEmail(this);
            }
        });
        $('#cblPowers').click(function () {
            if ($(this).find('input:checkbox:checked').val() == 'alls') {
                $(this).find('input:checkbox').not('input[type=checkbox]:first').prop('checked', false);
            }
        });
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtAdminName') || checkEmpty('#txtTrueName') || checkRadio('#rblUserSex') || checkEmpty('#txtMobile') || checkRadio('#rblActive')) {
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
                    <strong><asp:Literal ID="ltTitle" runat="server" Text="新增后台用户"></asp:Literal></strong>
                    <table>
                        <tbody>
                            <tr>
                                <th><b>*</b>登录名</th>
                                <td><asp:TextBox ID="txtAdminName" runat="server" MaxLength="20" ToolTip="登录名"></asp:TextBox></td>
                                <th>自编号</th>
                                <td><asp:TextBox ID="txtId" runat="server" ReadOnly="True" CssClass="readonly" Text="0"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>姓名</th>
                                <td><asp:TextBox ID="txtTrueName" runat="server" MaxLength="20" ToolTip="姓名"></asp:TextBox></td>
                                <th rowspan="8">照片</th>
                                <td>
                                    <asp:TextBox ID="txtPhoto" runat="server" CssClass="long"></asp:TextBox>
                                    <a id="btnPhoto" href="#" class="btn"><u>上传</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th>密码</th>
                                <td><asp:TextBox ID="txtAdminPwd" runat="server" MaxLength="20" TextMode="Password"></asp:TextBox>
                                    <i>(不修改时请留空)</i>
                                </td>
                                <td rowspan="7" id="showPhoto" class="pic"></td>
                            </tr>
                            <tr>
                                <th><b>*</b>性别</th>
                                <td>
                                    <asp:RadioButtonList ID="rblUserSex" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="性别">
                                        <asp:ListItem Text="男"></asp:ListItem>
                                        <asp:ListItem Text="女"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <th>身份证号</th>
                                <td><asp:TextBox ID="txtIdCode" runat="server" ToolTip="身份证"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>管理级别</th>
                                <td>
                                    <asp:RadioButtonList ID="rblGrade" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="待批准" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="管理员" Value="1" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="超级管理员" Value="9"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <th>管理权限</th>
                                <td>
                                    <asp:CheckBoxList ID="cblPowers" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="6">
                                        <asp:ListItem Text="全部" Value="alls"></asp:ListItem>
                                        <asp:ListItem Text="委员信息" Value="user"></asp:ListItem>
                                        <%--<asp:ListItem Text="政协动态" Value="news"></asp:ListItem>
                                        <asp:ListItem Text="委员走访" Value="view"></asp:ListItem>--%>
                                        <asp:ListItem Text="信息发布" Value="notice"></asp:ListItem>
                                        <asp:ListItem Text="会议/活动通知" Value="perform"></asp:ListItem>
                                        <asp:ListItem Text="提案管理" Value="opin"></asp:ListItem>
                                        <asp:ListItem Text="社情民意" Value="pop"></asp:ListItem>
                                        <asp:ListItem Text="调研报告" Value="report"></asp:ListItem>
                                        <asp:ListItem Text="问卷调查" Value="survey"></asp:ListItem>
                                        <asp:ListItem Text="资料方档" Value="datas"></asp:ListItem>
                                        <asp:ListItem Text="委员论坛" Value="forum"></asp:ListItem>
                                        <asp:ListItem Text="统计分析" Value="count"></asp:ListItem>
                                        <asp:ListItem Text="系统设置" Value="system"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th>部门职(岗)位</th>
                                <td><asp:TextBox ID="txtDepPost" runat="server" MaxLength="20"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>办公室电话</th>
                                <td><asp:TextBox ID="txtOfficeTel" runat="server" MaxLength="50" ToolTip="办公室电话"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>手机号码</th>
                                <td><asp:TextBox ID="txtMobile" runat="server" MaxLength="50" ToolTip="手机号"></asp:TextBox></td>
                                <th>电子邮箱</th>
                                <td><asp:TextBox ID="txtEmail" runat="server" MaxLength="200" CssClass="long" ToolTip="电子邮箱"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>状态</th>
                                <td>
                                    <asp:RadioButtonList ID="rblActive" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="正常" Value="1" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="锁定" Value="-1"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                                <th>密码错误次数</th>
                                <td><asp:TextBox ID="txtErrNum" runat="server" Text="0"></asp:TextBox>
                                    <i>(大于10时，用户自动锁定)</i>
                                </td>
                            </tr>
                            <tr>
                                <th rowspan="2">备注</th>
                                <td rowspan="2"><asp:TextBox ID="txtRemark" runat="server" TextMode="MultiLine" Rows="3" CssClass="long"></asp:TextBox></td>
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
                    <%--
	[Id] int IDENTITY(1,1) PRIMARY KEY,						--主键
	[AdminName] nvarChar(20) NOT NULL UNIQUE NONCLUSTERED,	--管理员名，非空，唯一
	[AdminPwd] varChar(32) NOT NULL DEFAULT '',				--密码，非空
	[TrueName] nvarChar(20) NOT NULL DEFAULT '',			--真实姓名
	[UserSex] nvarChar(2) NOT NULL DEFAULT '',				--性别
	[IdCard] varChar(50) NOT NULL DEFAULT '',				--身份证号（des加密保存）
	[Photo] text NOT NULL DEFAULT '',						--照片
	[Grade] int NOT NULL DEFAULT 0,							--级别状态（-1锁定，0待批准，1普通管理员，9超级管理员）
	[Powers] text NOT NULL DEFAULT '',						--管理权限
	[DepPost] nvarChar(20) NOT NULL DEFAULT '',				--部门职(岗)位
	[OfficeTel] varChar(50) NOT NULL DEFAULT '',			--办公室电话
	[Mobile] varChar(50) NOT NULL DEFAULT '',				--手机
	[Email] text NOT NULL DEFAULT '',						--邮箱
	[WeChat] varChar(50) NOT NULL DEFAULT '',				--微信号
	[Remark] ntext NOT NULL DEFAULT '',						--备注
	[Active] int NOT NULL DEFAULT 0,						--排序，锁定
	[LastTime] datetime NULL,								--最后登录时间
	[LastIp] varChar(30) NOT NULL DEFAULT '',				--最后登录IP:端口号
	[ErrNum] int NOT NULL DEFAULT 0,						--密码错误次数
                        --%>
                    <div class="cmd">
                        <asp:Button ID="btnEdit" runat="server" Text="新增" OnClick="btnEdit_Click" />
                        <input id="reset" type="reset" value="重填" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
