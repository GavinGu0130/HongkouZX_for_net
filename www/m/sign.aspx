<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="sign.aspx.cs" Inherits="hkzx.web.m.sign" %><%--Tony维护--%>
<%@ Register src="../cn/ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" Client="wap" />
    <title>签到页</title>
    <script src="http://res.wx.qq.com/open/js/jweixin-1.3.2.js"></script>
    <script>
        <% if (!string.IsNullOrEmpty(wxJsSdkParam)) { %>
        wx.config({
            //debug: true, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
            <%=wxJsSdkParam%>//josn串
        // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
        jsApiList: ['scanQRCode']
        });<%--
        wx.ready(function () {
            // config信息验证后会执行ready方法，所有接口调用都必须在config接口获得结果之后，config是一个客户端的异步操作，所以如果需要在页面加载时就调用相关接口，则须把相关接口放在ready函数中调用来确保正确执行。对于用户触发时才调用的接口，则可以直接调用，不需要放在ready函数中。
            wx.checkJsApi({
                jsApiList: ['scanQRCode'], // 需要检测的JS接口列表，所有JS接口列表见附录2,
                success: function (res) {
                    // 以键值对的形式返回，可用的api值true，不可用为false
                    // 如：{"checkResult":{"chooseImage":true},"errMsg":"checkJsApi:ok"}
                    // { "errMsg": "checkJsApi:ok", "checkResult": { "menu:share:appmessage": true } }
                    //alert(res);
                    alert('微信扫一扫接口状态：' + res.errMsg);

                }
            });
        });--%>
        <% } %>
        $(function () {
            if ($('#lblTitle').text() && !$('#header>dl.more>dd>b').text()) {
                $('#header>a.back').hide();
                $('#header>dl.more').hide();
                showDialog('', '../admin/login.aspx', '', 320, 280, 'no');
            } else {
                if (document.referrer != '') {
                    $('#header>a.back').click(function () {
                        window.history.back(-1);
                        return false;
                    });
                } else {
                    $('#header>a.back').hide();
                }
                $('#header>dl.more>dt').click(function () {
                    var $obj = $(this).parent().find('dd');
                    $obj.show();
                    setTimeout(function () {
                        $obj.hide();
                    }, 5000);
                });
                $('#logout').click(function () {
                    window.location.href = $(this).attr('href') + '&url=' + encodeURI(document.referrer);
                    return false;
                });
            }
        });
    </script>
</head>
<body>
<form id="form1" runat="server">
    <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    <asp:Panel ID="plHeader" runat="server" Visible="false" CssClass="header">
        <div id="header">
            <a href="#" class="back">&lt;</a>
            <asp:Label ID="lblTitle" runat="server"><span style="padding-right:5px;color:#F00;font-size:22px;">虹口区政协</span>签到系统</asp:Label>
            <dl class="more">
                <dt><i></i></dt>
                <dd>
                    <b><asp:Literal ID="ltUser" runat="server"></asp:Literal></b>
                    <a id="logout" href="../admin/login.aspx?ac=logout"><i></i><b>退出</b></a>
                </dd>
            </dl>
        </div>
    </asp:Panel>
    <div class="content main">
        <asp:Panel ID="plPerform" runat="server" Visible="false" CssClass="list">
            <dl class="sign">
                <asp:Repeater ID="rpPerformList" runat="server">
                    <ItemTemplate>
                        <dd>
                            <p>
                                <b>类型：</b><%#DataBinder.Eval(Container.DataItem, "SubType")%><br />
                                <b>主题：</b><%#DataBinder.Eval(Container.DataItem, "Title")%><br />
                                <b>时间：</b><%#DataBinder.Eval(Container.DataItem, "PerformTimeText")%><br />
                                <b>地点：</b><%#DataBinder.Eval(Container.DataItem, "PerformSite")%><br />
                            </p>
                            <a href="?ac=perform&id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看名单</u></a>
                        </dd>
                    </ItemTemplate>
                </asp:Repeater>
            </dl>
        </asp:Panel>

        <asp:Panel ID="plSign" runat="server" Visible="false" CssClass="list">
<script>
    $(function () {
        <% if (!string.IsNullOrEmpty(wxJsSdkParam)) { %>
        $('#sign').click(function () {
            //alert('调取扫码：' + $('#hfPerformId').val());
            wx.scanQRCode({
                needResult: 1, // 默认为0，扫描结果由微信处理，1则直接返回扫描结果，
                scanType: ["qrCode", "barCode"], // 可以指定扫二维码"qrCode"还是一维码"barCode"，默认二者都有
                success: function (res) {
                    //alert('扫码结果：' + res.resultStr); // 当needResult 为 1 时，扫码返回的结果
                    sign(res.resultStr);
                }
            });
            return false;
        });
        function sign (code) {
            var url = window.location.href;
            if (url.indexOf('?') >= 0) {
                url = url.substring(0, url.indexOf("?"));
            }
            url += '?pid=' + $('#hfPerformId').val() + '&token=' + code + '&desk=' + $('#header>dl>dd>b').text();
            $.get(url, {}, function () { }).success(function (data) {
                if (data != '签到成功') {
                    alert(data);
                } else {
                    alert(data);
                }
            });
        }
        <% } else { %>
        $('#sign').hide();
        <% } %>
    });
</script>
            <dl class="sign">
                <dd>
                    <p>
                        <b>类型：</b><asp:Literal ID="ltPerformSubType" runat="server"></asp:Literal><br />
                        <b>主题：</b><asp:Literal ID="ltPerformTitle" runat="server"></asp:Literal><br />
                        <b>时间：</b><asp:Literal ID="ltPerformTime" runat="server"></asp:Literal><br />
                        <b>地点：</b><asp:Literal ID="ltPerformSite" runat="server"></asp:Literal><br />
                        <b>人数：</b><asp:Literal ID="ltSignNum" runat="server"></asp:Literal>
                    </p>
                    <a id="sign" href="#" class="btn"><u>扫码签到</u></a>
                    <asp:HiddenField ID="hfPerformId" runat="server" /><asp:HiddenField ID="hfUser" runat="server" />
                </dd>
                <dt>
                    <span class="name">姓名</span>
                    <span class="time">签到时间</span>
                    <span class="state">状态</span>
                </dt>
                <asp:Repeater ID="rpSignList" runat="server">
                    <ItemTemplate>
                        <dd>
                            <span class="name"><%#DataBinder.Eval(Container.DataItem, "SignMan")%></span>
                            <span class="time"><%#DataBinder.Eval(Container.DataItem, "SignTimeText")%></span>
                            <span class="state"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></span>
                        </dd>
                    </ItemTemplate>
                </asp:Repeater>
            </dl>
        </asp:Panel>
    </div>
</form>
</body>
</html>
