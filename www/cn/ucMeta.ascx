<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucMeta.ascx.cs" Inherits="hkzx.web.cn.ucMeta" %><%--Tony维护--%>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/><meta http-equiv="Content-Language" content="zh-cn" />
<link rel="icon" href="../favicon.ico" type="image/x-icon"/><link rel="shortcut icon" href="../favicon.ico" type="image/x-icon"/>
<script>
    var ie = 0;
    if (new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})").exec(navigator.appVersion) != null) {
        ie = parseFloat(RegExp.$1);
    }<%--
    alert(navigator.appName);
    alert(navigator.userAgent);
    alert(navigator.userProfile);
    if (ie >= 6 && ie < 9) {
        alert('360浏览器请用“极速模式”访问！\nIE浏览器请升级到“9.0”以上版本！\n谢谢！');
    }--%>
    if (navigator.appName == 'Microsoft Internet Explorer' && (ie >= 6 && ie < 10)) {
        document.write('<script type="text/javascript" src="../inc/jquery-1.12.4.min.js">' + '</' + 'script>');
    } else {
        document.write('<script type="text/javascript" src="../inc/jquery-3.4.1.min.js">' + '</' + 'script>');
        document.write('<script type="text/javascript" src="../inc/wangEditor/wangEditor.js">' + '</' + 'script>');
    }
</script><%--
<script type="text/javascript" src="../inc/jquery-3.4.1.min.js"></script>
<script type="text/javascript" src="../inc/jquery-1.11.1.min.js"></script>
<script src="https://code.jquery.com/jquery-3.4.1.min.js"></script>
<script src="https://code.jquery.com/jquery-1.12.4.min.js"></script>
<script type="text/javascript" src="../inc/wangEditor/wangEditor.min.js"></%--script>
<script type="text/javascript" src="../inc/wangEditor/wangEditor.js"></script>--%>
<script type="text/javascript" src="../inc/ajaxfileupload.js"></script>
<script type="text/javascript" src="../inc/My97DatePicker/WdatePicker.js"></script>
<script charset="utf-8" src="../inc/ubb.js"></script>
<script type="text/javascript" src="../inc/kindeditor/kindeditor-min-ubb.js"></script>
<script type="text/javascript" src="../inc/check.js"></script>
<script type="text/javascript" src="../inc/default.js"></script>
<link rel="stylesheet" type="text/css" href="../inc/default.css" />
<asp:PlaceHolder ID="plM" runat="server" Visible="false">
    <link rel="stylesheet" type="text/css" href="../inc/m.css" />
    <meta name="viewport" content="width=device-width, user-scalable=no, maximum-scale=1.0, minimum-scale=1.0, initial-scale=1.0" />
</asp:PlaceHolder>
<asp:PlaceHolder ID="plPc" runat="server" Visible="false">
    <link rel="stylesheet" type="text/css" href="../inc/pc.css" />
    <meta name="viewport" content="width=device-width, user-scalable=yes, maximum-scale=1.0" />
</asp:PlaceHolder>
<asp:PlaceHolder ID="plAdmin" runat="server" Visible="false">
    <script type="text/javascript" src="../inc/admin.js"></script>
    <link rel="stylesheet" type="text/css" href="../inc/pc.css" />
    <link rel="stylesheet" type="text/css" href="../inc/admin.css" />
    <meta name="viewport" content="width=device-width, user-scalable=yes, maximum-scale=1.0" />
</asp:PlaceHolder>