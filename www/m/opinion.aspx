<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="opinion.aspx.cs" Inherits="hkzx.web.m.opinion" %><%--Tony维护--%>
<%@ Register src="../cn/ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="ucHeader.ascx" tagname="ucHeader" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" Client="wap" />
    <title>虹口政协履职通 - 提案管理</title>
    <style>
        .content i.flag { display:inline-block; background-color:#F00; line-height:18px; padding:0 3px; margin-right:4px; border-radius:3px; color:#FFF; font-style:normal; font-size:12px;}
    </style>
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
                } else if (url.indexOf('ac=sign') > 0) {
                    $('#plNav>div>a[href*="ac=sign"]').addClass('cur');
                } else if (url.indexOf('ac=feed') > 0) {
                    $('#plNav>div>a[href*="ac=feed"]').addClass('cur');
                } else if (url.indexOf('ac=result') > 0) {
                    $('#plNav>div>a[href*="ac=result"]').addClass('cur');
                } else if (url.indexOf('id=') > 0 && !$('#btnSub').val()) {
                    $('#plNav>div>#view').show().addClass('cur');
                } else {
                    //$('#plNav>a:first').addClass('cur');
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
    <asp:HiddenField ID="hfBack" runat="server" />
    <uc1:ucHeader ID="header1" runat="server" Title="提案管理" />
    <div class="content main">
        <asp:Panel ID="plNav" runat="server" Visible="false" CssClass="btn">
            <div>
                <a href="?ac=">提交提案</a>
                <a href="?ac=my">我的提案</a>
                <a href="?ac=save">暂存<b><asp:Literal ID="ltSaveNum" runat="server">0</asp:Literal></b></a>
                <a href="?ac=sign">需会签提案<b><asp:Literal ID="ltSignNum" runat="server">0</asp:Literal></b></a>
                <a href="?ac=feed">意见征询<b><asp:Literal ID="ltFeedNum" runat="server">0</asp:Literal></b></a>
                <%--<a href="?ac=result">跟踪办理情况<b><asp:Literal ID="ltResultNum" runat="server">0</asp:Literal></b></a>--%>
                <a href="?ac=query">检索提案</a>
                <a id="view" class="hide">查阅提案</a>
            </div>
        </asp:Panel>

        <asp:PlaceHolder ID="plSub" runat="server" Visible="false">
<script>
    $(function () {
        loadEditor('#txtBody', '#editorBody');
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
            showDialog('邀请联名提案人', '../cn/dialog.aspx?ac=subman&obj=txtSubMans', '', 320, 480, 'no');//&type=all
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
                } else if (checkEmpty('#txtSubOrg') || checkEmpty('#txtLinkman') || checkEmpty('#txtLinkmanTel')) {
                    return false;
                }
                if ($('#rblIsOpen>input:radio:checked').val() == '否' && checkEmpty('#txtOpenInfo')) {
                    return false;
                }
                if (checkRadio('#rblIsOpen') || checkEmpty('#txtSummary')) {// || checkRadio('#rblSubType') || checkEmpty('#txtSubTime')
                    return false;
                }
                if ($('#txtBody').val() == '' && $('#hfFiles').val() == '') {
                    alert('请填写[内容]或上传[附件]');
                    return false;
                }
                if ($('#txtBody').val()) {
                    var strBody = $('#txtBody').val();
                    if (strBody.indexOf('[img]blob:') >= 0 && strBody.indexOf('[/img]') > 0) {
                        alert('您有未成功上传的图片，\\n建议以word文档附件方式上传，谢谢！');
                        return false;
                    }
                }
                //if ($('#txtBody').val() != '') {
                //    var strBody = $('#txtBody').val();
                //    strBody = encodeURI(strBody);
                //    $('#txtBody').val(strBody);
                //}
            } catch (err) {
                //alert("验证出错，请稍后重试！");
                return false;
            }
        });
    });
</script>
            <div class="edit">
                <dl>
                    <dt>
                        虹口区政协
                        <asp:DropDownList ID="ddlPeriod" runat="server"></asp:DropDownList>届
                        <asp:DropDownList ID="ddlTimes" runat="server"></asp:DropDownList>次会议<br />
                        提案信息
                    </dt>
                    <dd>
                        <asp:HiddenField ID="hfSubManType" runat="server" Value="" />
                        <strong><b>*</b>提交日期</strong>
                        <asp:TextBox ID="txtSubTime" runat="server" MaxLength="10" ReadOnly="true" CssClass="readonly" ToolTip="提交日期"></asp:TextBox>
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
                        <strong><b>*</b>案由</strong>
                        <asp:TextBox ID="txtSummary" runat="server" TextMode="MultiLine" CssClass="long" Rows="3" MaxLength="100" ToolTip="案由"></asp:TextBox>
                    </dd>
                    <dd class="mans">
                        <strong><b>*</b>提案人</strong>
                        <asp:TextBox ID="txtSubMan" runat="server" ReadOnly="true" CssClass="readonly" ToolTip="第一提案人"></asp:TextBox>
                    </dd>
                    <dd class="mans">
                        <strong>联名提案人</strong>
                        <asp:TextBox ID="txtSubMans" runat="server" TextMode="MultiLine" Rows="3" CssClass="readonly long"></asp:TextBox>
                        <a id="btnSubMans" href="#"><u>选取</u></a>
                    </dd>
                    <dd class="team">
                        <strong><b>*</b>组织名称</strong>
                        <asp:TextBox ID="txtSubOrg" runat="server" ReadOnly="true" CssClass="readonly long" ToolTip="组织名称"></asp:TextBox>
                    </dd>
                    <dd class="team">
                        <strong><b>*</b>联系人</strong>
                        <asp:TextBox ID="txtLinkman" runat="server" MaxLength="20" ToolTip="联系人"></asp:TextBox>
                    </dd>
                    <dd class="team">
                        <strong><b>*</b>联系电话</strong>
                        <asp:TextBox ID="txtLinkmanTel" runat="server" MaxLength="50" ToolTip="联系电话"></asp:TextBox>
                    </dd>
                    <dd class="team">
                        <strong>通讯地址</strong>
                        <asp:TextBox ID="txtLinkmanAddress" runat="server" MaxLength="100" CssClass="long"></asp:TextBox>
                    </dd>
                    <dd class="team">
                        <strong>邮政编码</strong>
                        <asp:TextBox ID="txtLinkmanZip" runat="server" MaxLength="6"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong>提案者意向主办单位</strong>
                        <asp:DropDownList ID="ddlAdviseHostOrg" runat="server">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </dd>
                    <dd>
                        <strong>提案者意向会办单位</strong>
                        <asp:DropDownList ID="ddlAdviseHelpOrg" runat="server">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </dd>
                    <dd>
                        <strong><b>*</b>提案内容</strong>
                        <div id="editorBody" class="editor"><%--<p>欢迎使用 <b>wangEditor</b> 富文本编辑器</p>--%></div>
                        <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" CssClass="long" Rows="8" ToolTip="提案内容"></asp:TextBox>
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
                    <dt>我的提案</dt>
                    <asp:Repeater ID="rpSubList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                <p>
                                    <b>提案号：</b><%#DataBinder.Eval(Container.DataItem, "OpNo")%><br />
                                    <b>类别：</b><%#DataBinder.Eval(Container.DataItem, "SubType")%><br />
                                    <b>案由：</b><%#DataBinder.Eval(Container.DataItem, "Summary")%><br />
                                    <b>提交日期：</b><%#DataBinder.Eval(Container.DataItem, "SubTime", "{0:yyyy-MM-dd HH:mm:ss}")%><br />
                                    <b>所处流程：</b><%#DataBinder.Eval(Container.DataItem, "ActiveName")%><br /><%#DataBinder.Eval(Container.DataItem, "ApplyState")%><br />
                                    <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u><%#DataBinder.Eval(Container.DataItem, "StateName")%></u></a><%#DataBinder.Eval(Container.DataItem, "other")%>
                                </p>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltSubNo" runat="server">
                        <dd class="no">暂无已提交的提案！</dd>
                    </asp:Literal>
                </dl>
                <asp:Label ID="lblSubNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plSign" runat="server" Visible="false">
            <div class="list table">
                <dl>
                    <dt>联名提案</dt>
                    <asp:Repeater ID="rpSignList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                <p>
                                    <b>类别：</b><%#DataBinder.Eval(Container.DataItem, "SubType")%><br />
                                    <b>案由：</b><%#DataBinder.Eval(Container.DataItem, "Summary")%><br />
                                    <b>提案人：</b><%#DataBinder.Eval(Container.DataItem, "SubMan")%><br />
                                    <b>提交日期：</b><%#DataBinder.Eval(Container.DataItem, "SubTime", "{0:yyyy-MM-dd HH:mm:ss}")%><br />
                                    <b>状态：</b><%#DataBinder.Eval(Container.DataItem, "ActiveName")%><br />
                                    <%#DataBinder.Eval(Container.DataItem, "other")%>
                                </p>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltSignNo" runat="server">
                        <dd class="no">暂无需会签的提案！</dd>
                    </asp:Literal>
                </dl>
                <asp:Label ID="lblSignNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plFeed" runat="server" Visible="false">
            <div class="list table">
                <dl>
                    <dt>意见征询</dt>
                    <asp:Repeater ID="rpFeedList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                <p>
                                    <b>提案号：</b><%#DataBinder.Eval(Container.DataItem, "OpNo")%><br />
                                    <b>类别：</b><%#DataBinder.Eval(Container.DataItem, "SubType")%><br />
                                    <b>案由：</b><%#DataBinder.Eval(Container.DataItem, "Summary")%><br />
                                    <b>主办单位：</b><%#DataBinder.Eval(Container.DataItem, "ExamHostOrg")%><br />
                                    <b>会办单位：</b><%#DataBinder.Eval(Container.DataItem, "ExamHelpOrg")%><br />
                                    <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u><%#DataBinder.Eval(Container.DataItem, "StateName")%></u></a>
                                    <%#DataBinder.Eval(Container.DataItem, "other")%>
                                </p>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltFeedNo" runat="server">
                        <dd class="no">暂无意见征询提案！</dd>
                    </asp:Literal>
                </dl>
                <asp:Label ID="lblFeedNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plFeedEdit" runat="server" Visible="false">
            <div class="edit">
<script>
$(function () {
    upFile('#txtFeedFiles', '', '#btnFeedFiles');
    $('form').submit(function () {
        try {
            if (checkRadio('#rblFeedInterview') || checkRadio('#rblFeedAttitude') || checkRadio('#rblFeedResult')) {// || checkRadio('#rblFeedTakeWay') || checkRadio('#rblFeedPertinence')
                return false;
            }
        } catch (err) {
            //alert("验证出错，请稍后重试！");
            return false;
        }
    });
});
</script>
                <dl>
                    <dt>提案办理情况征询意见表</dt>
                    <dd>
                        <strong>提案号</strong>
                        <asp:Literal ID="ltFeedOpNo" runat="server"></asp:Literal>
                    </dd>
                    <dd>
                        <strong>承办单位</strong>
                        <asp:Literal ID="ltFeedOpOrg" runat="server"></asp:Literal>
                    </dd>
                    <dd>
                        <strong>办理结果</strong>
                        <asp:Literal ID="ltFeedOpResult" runat="server"></asp:Literal>
                    </dd>
                    <dd>
                        <strong>案由</strong>
                        <asp:Literal ID="ltFeedSummary" runat="server"></asp:Literal>
                    </dd>
                    <dd>
                        <strong><b>*</b>走访情况</strong>
                        <asp:Literal ID="ltFeedInterview" runat="server"></asp:Literal>
                        <asp:RadioButtonList ID="rblFeedInterview" runat="server" Visible="false" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="走访情况"></asp:RadioButtonList>
                    </dd>
                    <dd>
                        <strong><b>*</b>办理(走访) 人员态度</strong>
                        <asp:Literal ID="ltFeedAttitude" runat="server"></asp:Literal>
                        <asp:RadioButtonList ID="rblFeedAttitude" runat="server" Visible="false" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="办理态度"></asp:RadioButtonList>
                    </dd>
                    <dd>
                        <strong><b>*</b>是否同意 办理结果</strong>
                        <asp:Literal ID="ltFeedResult" runat="server"></asp:Literal>
                        <asp:RadioButtonList ID="rblFeedResult" runat="server" Visible="false" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="办理结果"></asp:RadioButtonList>
                    </dd>
                    <dd style="display:none;">
                        <strong><b>*</b>答复前听取 意见方式</strong>
                        <asp:Literal ID="ltFeedTakeWay" runat="server"></asp:Literal>
                        <asp:RadioButtonList ID="rblFeedTakeWay" runat="server" Visible="false" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="听取意见方式"></asp:RadioButtonList>
                    </dd>
                    <dd style="display:none;">
                        <strong>答复是否 针对提案</strong>
                        <asp:Literal ID="ltFeedPertinence" runat="server"></asp:Literal>
                        <asp:RadioButtonList ID="rblFeedPertinence" runat="server" Visible="false" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="答复是否针对提案"></asp:RadioButtonList>
                    </dd>
                    <dd style="display:none;">
                        <strong>(团体)分管领导答复</strong>
                        <asp:Literal ID="ltFeedLeaderReply" runat="server"></asp:Literal>
                        <asp:RadioButtonList ID="rblFeedLeaderReply" runat="server" Visible="false" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="分管领导答复">
                            <asp:ListItem Text="是"></asp:ListItem>
                            <asp:ListItem Text="否"></asp:ListItem>
                        </asp:RadioButtonList>
                    </dd>
                    <dd>
                        <strong>对提案办理的 意见或建议</strong>
                        <asp:Literal ID="ltFeedBody" runat="server"></asp:Literal>
                        <asp:TextBox ID="txtFeedBody" runat="server" Visible="false" TextMode="MultiLine" Rows="9" CssClass="long"></asp:TextBox>
                    </dd>
                </dl>
                <asp:Panel ID="plFeedCmd" runat="server" Visible="false" CssClass="cmd">
                    <asp:Button ID="btnFeed" runat="server" Text="提交" OnClick="btnFeed_Click" />
                    <input type="reset" value="重填" />
                </asp:Panel>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plSave" runat="server" Visible="false">
            <div class="list table">
                <dl>
                    <dt>暂存的提案</dt>
                    <asp:Repeater ID="rpSaveList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                <p>
                                    <b>类别：</b><%#DataBinder.Eval(Container.DataItem, "SubType")%><br />
                                    <b>案由：</b><%#DataBinder.Eval(Container.DataItem, "Summary")%><br />
                                    <b>编辑时间：</b><%#DataBinder.Eval(Container.DataItem, "UpTime", "{0:yyyy-MM-dd HH:mm:ss}")%><br />
                                    <b>状态：</b><%#DataBinder.Eval(Container.DataItem, "ActiveName")%><br />
                                    <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>选取</u></a>
                                </p>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltSaveNo" runat="server">
                        <dd class="no">暂无暂存的提案！</dd>
                    </asp:Literal>
                </dl>
                <asp:Label ID="lblSaveNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plResult" runat="server" Visible="false">
            <div class="list table">
                <dl>
                    <dt>跟踪办理情况</dt>
                    <asp:Repeater ID="rpResultList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                <p>
                                    <b>提案号：</b><%#DataBinder.Eval(Container.DataItem, "OpNo")%><br />
                                    <b>类别：</b><%#DataBinder.Eval(Container.DataItem, "SubType")%><br />
                                    <b>案由：</b><%#DataBinder.Eval(Container.DataItem, "Summary")%><br />
                                    <b>主办单位：</b><%#DataBinder.Eval(Container.DataItem, "ExamHostOrg")%><br />
                                    <b>会办单位：</b><%#DataBinder.Eval(Container.DataItem, "ExamHelpOrg")%><br />
                                    <b>办理状态：</b><%#DataBinder.Eval(Container.DataItem, "ApplyState")%><br />
                                    <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看</u></a>
                                    <%#DataBinder.Eval(Container.DataItem, "other")%>
                                </p>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltResultNo" runat="server">
                        <dd class="no">暂无跟踪办理的提案！</dd>
                    </asp:Literal>
                </dl>
                <asp:Label ID="lblResultNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plQuery" runat="server" Visible="false">
<script>
    $(function () {
        $('#btnQSubMan').click(function () {
            var title = '选取委员';
            var obj = 'txtQSubMan';
            showDialog(title, '../cn/dialog.aspx?ac=subman&type=all&obj=' + obj, '', 320, 480, 'no');
            return false;
        });
        $('#txtQSubMan').focus(function () {
            $('#btnQSubMan').click();
        });
        $('#btnQuery').click(function () {
            try {
                var url = '';
                if ($('#txtQOpNo').val()) {
                    url += '&OpNo=' + $('#txtQOpNo').val();
                }
                if ($('#txtQSubMan').val()) {
                    url += '&SubMan=' + $('#txtQSubMan').val();
                }
                if ($('#ddlQActiveName').val()) {
                    url += '&ActiveName=' + $('#ddlQActiveName').val();
                }
                if ($('#ddlQPeriod').val()) {
                    url += '&Period=' + $('#ddlQPeriod').val();
                }
                if ($('#ddlQTimes').val()) {
                    url += '&Times=' + $('#ddlQTimes').val();
                }
                if ($('#ddlQSubType').val()) {
                    url += '&SubType=' + $('#ddlQSubType').val();
                }
                if ($('#ddlQTimeMark').val()) {
                    url += '&TimeMark=' + $('#ddlQTimeMark').val();
                }
                if ($('#ddlQIsGood').val()) {
                    url += '&IsGood=' + $('#ddlQIsGood').val();
                }
                if ($('#ddlQIsPoint').val()) {
                    url += '&IsPoint=' + $('#ddlQIsPoint').val();
                }
                if ($('#ddlQParty').val()) {
                    url += '&Party=' + $('#ddlQParty').val();
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
                if ($('#txtQSummary').val()) {
                    url += '&Summary=' + $('#txtQSummary').val();
                }
                if ($('#txtQBody').val()) {
                    url += '&Body=' + $('#txtQBody').val();
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
                        <strong>提案号</strong>
                        <asp:TextBox ID="txtQOpNo" runat="server"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong>提案人</strong>
                        <asp:TextBox ID="txtQSubMan" runat="server" MaxLength="20" CssClass="readonly" ToolTip="提案人"></asp:TextBox>
                        <a id="btnQSubMan" href="#" class="btn"><u>选取</u></a>
                    </dd>
                    <dd>
                        <strong>提案性质</strong>
                        <asp:DropDownList ID="ddlQActiveName" runat="server">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                            <asp:ListItem Text="归并,立案"></asp:ListItem>
                            <asp:ListItem Text="不立案"></asp:ListItem>
                        </asp:DropDownList>
                    </dd>
                    <dd>
                        <strong>提案类别</strong>
                        <asp:DropDownList ID="ddlQSubType" runat="server">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                        </asp:DropDownList>
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
                        <strong>时间标识</strong>
                        <asp:DropDownList ID="ddlQTimeMark" runat="server">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                            <asp:ListItem Text="会间"></asp:ListItem>
                            <asp:ListItem Text="会后"></asp:ListItem>
                        </asp:DropDownList>
                    </dd>
                    <dd>
                        <strong>届次</strong>
                        <asp:DropDownList ID="ddlQPeriod" runat="server" Width="92">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                        </asp:DropDownList>届
                        <asp:DropDownList ID="ddlQTimes" runat="server" Width="92">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                        </asp:DropDownList>次
                    </dd>
                    <dd>
                        <strong>是否优秀提案</strong>
                        <asp:DropDownList ID="ddlQIsGood" runat="server">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                            <asp:ListItem Text="是"></asp:ListItem>
                            <asp:ListItem Text="否"></asp:ListItem>
                        </asp:DropDownList>
                    </dd>
                    <dd>
                        <strong>是否重点提案</strong>
                        <asp:DropDownList ID="ddlQIsPoint" runat="server" Width="92">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                            <asp:ListItem Text="是"></asp:ListItem>
                            <asp:ListItem Text="否"></asp:ListItem>
                        </asp:DropDownList>
                    </dd>
                    <dd>
                        <strong>案由</strong>
                        <asp:TextBox ID="txtQSummary" runat="server" CssClass="long"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong>正文</strong>
                        <asp:TextBox ID="txtQBody" runat="server" CssClass="long"></asp:TextBox>
                    </dd>
                </dl>
                <div class="cmd">
                    <input id="btnQuery" type="button" value="查询" />
                    <input id="clean" type="reset" value="清空" />
                </div>
            </div>
            <div class="list table">
                <dl>
                    <dt>符合条件的数据有：<b><asp:Literal ID="ltQueryTotal" runat="server" Text="0"></asp:Literal></b>条</dt>
                    <asp:Repeater ID="rpQueryList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                <p>
                                    <b>提案号：</b><%#DataBinder.Eval(Container.DataItem, "OpNo")%><br />
                                    <b>提案人：</b><%#DataBinder.Eval(Container.DataItem, "SubMan")%><br />
                                    <b>类别：</b><%#DataBinder.Eval(Container.DataItem, "SubType")%><br />
                                    <b>案由：</b><%#DataBinder.Eval(Container.DataItem, "Summary")%><br />
                                    <b>主办单位：</b><%#DataBinder.Eval(Container.DataItem, "ExamHostOrg")%><br />
                                    <b>会办单位：</b><%#DataBinder.Eval(Container.DataItem, "ExamHelpOrg")%><br />
                                    <b>办理状态：</b><%#DataBinder.Eval(Container.DataItem, "ApplyState")%><br />
                                    <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看</u></a>
                                </p>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltQueryNo" runat="server">
                        <dd class="no">暂无提案！</dd>
                    </asp:Literal>
                </dl>
                <asp:Label ID="lblQueryNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plView" runat="server" Visible="false">
<script>
    $(function () {
        if ($('div.edit>table>tbody>tr>td.editor').text()) {
            $('div.edit>table>tbody>tr>td.editor').html(ubb2html($('div.edit>table>tbody>tr>td.editor').text()));
        }
        switch ($('#SubManType').text()) {
            case '委员':
                $('.edit>table>tbody>tr.man').show();
                $('.edit>table>tbody>tr.mans').hide();
                $('.edit>table>tbody>tr.team').hide();
                break;
            case '联名':
                $('.edit>table>tbody>tr.man').hide();
                $('.edit>table>tbody>tr.mans').show();
                $('.edit>table>tbody>tr.team').hide();
                break;
            default:
                $('.edit>table>tbody>tr.man').hide();
                $('.edit>table>tbody>tr.mans').hide();
                $('.edit>table>tbody>tr.team').show();
                break;
        }
        if ($('#hfUserId').val() == '0') {
            $('div.edit>table>tbody .my').hide();
        }
    });
</script>
                <div class="edit">
                    <div class="label" style="display:block;text-align:center;">
                        虹口区政协
                        <u><asp:Literal ID="ltPeriod" runat="server" Text="十四"></asp:Literal></u>届
                        <u><asp:Literal ID="ltTimes" runat="server" Text="二"></asp:Literal></u>次会议
                        <asp:PlaceHolder ID="plTeamNum" runat="server" Visible="false">第<u><asp:Literal ID="ltTeamNum" runat="server"></asp:Literal></u>组</asp:PlaceHolder>
                        提案信息
                    </div>
                    <asp:HiddenField ID="hfUserId" runat="server" Value="0" />
                    <table>
                        <tbody>
                            <tr>
                                <th>提案号</th>
                                <td><asp:Literal ID="ltOpNo" runat="server"></asp:Literal></td>
                            </tr>
                            <tr class="my">
                                <th>提案流水号</th>
                                <td><asp:Literal ID="ltId" runat="server" Text="0"></asp:Literal></td>
                            </tr>
                            <tr class="my">
                                <th>提交日期</th>
                                <td><asp:Literal ID="ltSubTime" runat="server"></asp:Literal></td>
                            </tr>
                            <tr class="my">
                                <th>办理状态</th>
                                <td><asp:Literal ID="ltApplyState" runat="server"></asp:Literal></td>
                            </tr>
                            <tr class="my">
                                <th>提案性质</th>
                                <td><asp:Literal ID="ltActiveName" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>提案类别</th>
                                <td><asp:Literal ID="ltSubType" runat="server"></asp:Literal></td>
                            </tr>
                            <tr class="my">
                                <th>提案者性质</th>
                                <td id="SubManType"><asp:Literal ID="ltSubManType" runat="server"></asp:Literal></td>
                            </tr>
                            <tr class="my">
                                <th>是否同意公开</th>
                                <td><asp:Literal ID="ltIsOpen" runat="server"></asp:Literal></td>
                            </tr>
                            <tr class="my">
                                <th>是否重点提案</th>
                                <td><asp:Literal ID="ltIsPoint" runat="server"></asp:Literal></td>
                            </tr>
                            <tr class="man">
                                <th>提案人</th>
                                <td><asp:Literal ID="ltSubMan" runat="server"></asp:Literal></td>
                            </tr>
                            <tr class="mans">
                                <th>第一提案人</th>
                                <td><asp:Literal ID="ltSubMan1" runat="server"></asp:Literal></td>
                            </tr>
                            <tr class="mans">
                                <th>联名提案人</th>
                                <td><asp:Literal ID="ltSubMans" runat="server"></asp:Literal></td>
                            </tr>
                            <tr class="team">
                                <th>提案团体</th>
                                <td><asp:Literal ID="ltSubOrg" runat="server"></asp:Literal></td>
                            </tr>
                            <tr class="team my">
                                <th>联系人姓名</th>
                                <td><asp:Literal ID="ltLinkman" runat="server"></asp:Literal></td>
                            </tr>
                            <tr class="team my">
                                <th>联系电话</th>
                                <td><asp:Literal ID="ltLinkmanTel" runat="server"></asp:Literal></td>
                            </tr>
                            <tr class="team my">
                                <th>通讯地址</th>
                                <td><asp:Literal ID="ltLinkmanAddress" runat="server"></asp:Literal></td>
                            </tr>
                            <tr class="team my">
                                <th>邮政编码</th>
                                <td><asp:Literal ID="ltLinkmanZip" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>案由</th>
                                <td><asp:Literal ID="ltSummary" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>提案内容</th>
                                <td class="editor"><asp:Literal ID="ltBody" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>附件</th>
                                <td><asp:Literal ID="ltFiles" runat="server"></asp:Literal></td>
                            </tr>
                            <tr class="my">
                                <th>提案者意向主办单位</th>
                                <td><asp:Literal ID="ltAdviseHostOrg" runat="server"></asp:Literal></td>
                            </tr>
                            <tr class="my">
                                <th>提案者意向会办单位</th>
                                <td><asp:Literal ID="ltAdviseHelpOrg" runat="server"></asp:Literal></td>
                            </tr>
                            <asp:PlaceHolder ID="plViewResult" runat="server" Visible="false">
                                <tr class="my">
                                    <th>审查意向主办单位</th>
                                    <td><asp:Literal ID="ltExamHostOrg" runat="server"></asp:Literal></td>
                                </tr>
                                <tr class="my">
                                    <th>审查意向会办单位</th>
                                    <td><asp:Literal ID="ltExamHelpOrg" runat="server"></asp:Literal></td>
                                </tr>
                                <tr>
                                    <th>办理结果</th>
                                    <td><asp:Literal ID="ltResult" runat="server"></asp:Literal></td>
                                </tr>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plSignBody" runat="server" Visible="false">
                                <tr>
                                    <th>联名意见<br /><b>(必填)</b></th>
                                    <td><asp:Literal ID="ltSignBody" runat="server"></asp:Literal>
                                        <asp:TextBox ID="txtSignBody" runat="server" Visible="false" TextMode="MultiLine" Rows="3" CssClass="long" MaxLength="100" ToolTip="联名意见"></asp:TextBox>
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                        </tbody>
                    </table>
                    <asp:Panel ID="plSignEdit" runat="server" Visible="false" CssClass="cmd">
<script>
    var isSub = true;
    $(function () {
        $('#btnSign').click(function () {
            isSub = true;
        });
        $('#btnNoSign').click(function () {
            if (!confirm('您确定要“' + $(this).val() + '”吗?')) {
                return false;
            }
            isSub = false;
        });
        $('form').submit(function () {
            try {
                if (isSub && checkEmpty('#txtSignBody', 30, 100)) {
                    return false;
                }
            } catch (err) {
                //alert("验证出错，请稍后重试！");
                return false;
            }
        });
    })
</script>
                        <asp:Button ID="btnSign" runat="server" Text="同意会签" OnClick="btnSign_Click" />
                        <asp:Button ID="btnNoSign" runat="server" Text="谢绝会签" OnClick="btnNoSign_Click" />
                        <%--<input type="reset" value="重填" />--%><asp:HiddenField ID="hfSignId" runat="server" Value="0" />
                    </asp:Panel>
                </div>
        </asp:PlaceHolder>
    </div>
</form>
</body>
</html>
