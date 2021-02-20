<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="perform.aspx.cs" Inherits="hkzx.web.m.perform" %><%--Tony维护--%>
<%@ Register src="../cn/ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="ucHeader.ascx" tagname="ucHeader" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" Client="wap" />
    <title>虹口政协履职通 - 履职专区</title>
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
                } else if (url.indexOf('ac=sub') > 0) {
                    //$('#plNav>a:first').addClass('cur');
                    $('#plNav>div>a[href$="ac=sub"]').addClass('cur');
                } else {
                    $('#plNav>div>a[href*="ac=query"]').addClass('cur');
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
    <asp:HiddenField ID="hfOrg" runat="server" /><asp:HiddenField ID="hfSubType" runat="server" />
    <uc1:ucHeader ID="header1" runat="server" Title="履职专区" />
    <div class="content main">
        <asp:Panel ID="plNav" runat="server" Visible="false" CssClass="btn">
            <div>
                <a href="?ac=sub">申请会议/活动</a>
                <a href="?ac=my">我提交的会议/活动</a>
                <a href="?ac=save">暂存<b><asp:Literal ID="ltSaveNum" runat="server">0</asp:Literal></b></a>
                <%--<a href="?ac=query">会议/活动通知</a>
                <a id="view" class="hide">履职活动祥情</a>--%>
            </div>
        </asp:Panel>

        <asp:PlaceHolder ID="plList" runat="server" Visible="false">
            <div class="list table">
                <dl>
                    <dt class="total">符合条件的数据有：<b><asp:Literal ID="ltQueryTotal" runat="server" Text="0"></asp:Literal></b>条</dt>
                    <asp:Repeater ID="rpQueryList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                <p>
                                    <b>参加方式：</b><%#DataBinder.Eval(Container.DataItem, "IsMust")%><br />
                                    <b>类型：</b><%#DataBinder.Eval(Container.DataItem, "SubType")%><br />
                                    <b>部门：</b><%#DataBinder.Eval(Container.DataItem, "OrgName")%><br />
                                    <b>主题：</b><%#DataBinder.Eval(Container.DataItem, "Title")%><br />
                                    <b>时间：</b><%#DataBinder.Eval(Container.DataItem, "PerformTimeText")%><br />
                                    <b>地点：</b><%#DataBinder.Eval(Container.DataItem, "PerformSite")%><br />
                                    <b>报名截止：</b><%#DataBinder.Eval(Container.DataItem, "OverTime", "{0:yyyy年M月d日 HH:mm}")%><br />
                                    <b>状态：</b><%#DataBinder.Eval(Container.DataItem, "ActiveName")%><br />
                                    <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看</u></a>
                                </p>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltQueryNo" runat="server">
                        <dd class="no">暂无履职活动！</dd>
                    </asp:Literal>
                </dl>
                <asp:Label ID="lblQueryNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plView" runat="server" Visible="false">
            <div class="edit">
                <div class="label">履职活动详情</div>
                <table>
                    <tbody>
                        <tr>
                            <th>活动状态</th>
                            <td><asp:Literal ID="ltActive" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>必须参加</th>
                            <td><asp:Literal ID="ltIsMust" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>类型</th>
                            <td><asp:Literal ID="ltSubType" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>申请部门</th>
                            <td><asp:Literal ID="ltOrgName" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>联系人</th>
                            <td><asp:Literal ID="ltLinkman" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>联系电话</th>
                            <td><asp:Literal ID="ltLinkmanTel" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>活动主题</th>
                            <td><asp:Literal ID="ltTitle" runat="server">名称</asp:Literal></td>
                        </tr>
                        <tr>
                            <th>地点</th>
                            <td><asp:Literal ID="ltPerformSite" runat="server">地点</asp:Literal></td>
                        </tr>
                        <tr>
                            <th>开始时间</th>
                            <td><asp:Literal ID="ltStartTime" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>结束时间</th>
                            <td><asp:Literal ID="ltEndTime" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>报名截止时间</th>
                            <td><asp:Literal ID="ltOverTime" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>签到开始时间</th>
                            <td><asp:Literal ID="ltSignTime" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>用车情况</th>
                            <td><asp:Literal ID="ltHaveBus" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>其他</th>
                            <td><asp:Literal ID="ltHaveDinner" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>出席领导</th>
                            <td><asp:Literal ID="ltLeaders" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>参加委员</th>
                            <td><asp:Literal ID="ltAttendees" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>活动内容</th>
                            <td><asp:Literal ID="ltBody" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>附件</th>
                            <td><asp:Literal ID="ltFiles" runat="server"></asp:Literal></td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <asp:Panel ID="plPlayFeed" runat="server" Visible="false" CssClass="list">
                <strong>履职情况
                    <span>符合条件的数据有：<b><asp:Literal ID="ltFeedTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                </strong>
                <table>
                    <thead>
                        <tr>
                            <th class="num">序号</th>
                            <th class="state">状态</th>
                            <th class="state">姓名</th>
                            <th class="time">签到时间</th>
                            <th class="time">反馈时间</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rpFeedList" runat="server">
                            <ItemTemplate>
                                <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                    <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "IsMust")%><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                    <td align="center" title="<%#DataBinder.Eval(Container.DataItem, "SignManCode")%>"><%#DataBinder.Eval(Container.DataItem, "SignMan")%></td>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "SignTimeText")%></td>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "FeedTimeText")%></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Literal ID="ltFeedNo" runat="server">
                            <tr>
                                <td class="no">暂无履职情况！</td>
                            </tr>
                        </asp:Literal>
                    </tbody>
                </table>
            </asp:Panel>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plFeed" runat="server" Visible="false">
<script>
$(function () {
    $('#leave').hide();
    $('#btnLeave').click(function () {
        try {
            if ($('#txtReply').val() == '') {
                $('#leave').slideDown(200);
                window.setTimeout(function () {
                    alert('请填写[' + $('#txtReply').attr('title') + ']');
                    $('#txtReply').focus();
                }, 300);
                return false;
            }
            return true;
        } catch (err) {
            //alert("验证出错，请稍后重试！");
            return false;
        }
    });
});
</script>
            <div class="edit">
                <div class="label">您的履职情况</div>
                <table>
                    <tbody>
                        <tr>
                            <th>履职状态</th>
                            <td><asp:Literal ID="ltFeed" runat="server"></asp:Literal></td>
                        </tr>
                        <tr id="leave">
                            <th>请假原因</th>
                            <td><asp:TextBox ID="txtReply" runat="server" TextMode="MultiLine" Rows="3" CssClass="long" ToolTip="请假原因"></asp:TextBox></td>
                        </tr>
                    </tbody>
                </table>
                <div class="cmd">
                    <asp:Button ID="btnAttend" runat="server" Visible="false" Text="参加" OnClick="btnAttend_Click" />
                    <asp:Button ID="btnNonAttend" runat="server" Visible="false" Text="不参加" OnClick="btnNonAttend_Click" />
                    <asp:Button ID="btnLeave" runat="server" Visible="false" Text="请假" OnClick="btnLeave_Click" />
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plSub" runat="server" Visible="false">
<script>
    $(function () {
        loadSelMenu('#hfOrg', '#txtOrgName', '#OrgName', 'm');
        loadSelMenu('#hfSubType', '#txtSubType', '#SubType', 'm');
        $('#txtSubType').change(function () {
            var txt = $(this).val();
            if (txt) {
                if (txt == '政协全体会议' || txt == '全体委员学习会' || txt == '常委会议') {
                    $('#txtIsMust').val('必须参加');
                } else {
                    $('#txtIsMust').val('报名参加');
                }
            }
        }).change();
        //upFile('#txtFiles', '', '#btnFiles');
        upFile('#hfFiles', '#files', '#btnFiles', 'doc');
        $('#btnAttendees').click(function () {
            var title = '选取参加委员';
            var obj = 'txtAttendees';
            showDialog(title, '../cn/dialog.aspx?ac=subman&obj=' + obj, '', 320, 480, 'no');
            return false;
        });
        $('#txtAttendees').focus(function () {
            $('#' + $(this).attr('id').replace('txt', 'btn')).click();
        });
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtOrgName') || checkEmpty('#txtSubType') || checkEmpty('#txtLinkman') || checkEmpty('#txtLinkmanTel') || checkEmpty('#txtTitle') || checkEmpty('#txtPerformSite') || checkEmpty('#txtStartTime') || checkEmpty('#txtEndTime') || checkEmpty('#txtOverTime') || checkEmpty('#txtSignTime') || checkRadio('#rblHaveBus') || checkEmpty('#txtAttendees')) {
                    return false;
                }
            } catch (err) {
                alert("验证出错，请稍后重试！");
                return false;
            }
        });
    });
</script>
            <div class="edit">
                <dl>
                    <dt>申请会议/活动</dt>
                    <dd>
                        <strong>活动状态</strong>
                        <asp:TextBox ID="txtActiveName" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong><b>*</b>申请部门(单位)</strong>
                        <asp:DropDownList ID="ddlOrgType" runat="server" Visible="false" ToolTip="部门">
                            <asp:ListItem Text="办公室"></asp:ListItem>
                            <asp:ListItem Text="专委办"></asp:ListItem>
                            <asp:ListItem Text="专委会"></asp:ListItem>
                            <asp:ListItem Text="界别"></asp:ListItem>
                            <asp:ListItem Text="街道活动组"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:TextBox ID="txtOrgName" runat="server" CssClass="readonly long"></asp:TextBox>
                        <div id="OrgName" class="selmenu"></div>
                    </dd>
                    <dd>
                        <strong><b>*</b>类型</strong>
                        <asp:DropDownList ID="ddlSubType" runat="server" Visible="false" ToolTip="类型"></asp:DropDownList>
                        <asp:TextBox ID="txtSubType" runat="server" CssClass="readonly long" ToolTip="类型"></asp:TextBox>
                        <div id="SubType" class="selmenu"></div>
                    </dd>
                    <dd>
                        <strong>参加方式</strong>
                        <asp:TextBox ID="txtIsMust" runat="server" CssClass="readonly"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong><b>*</b>联系人</strong>
                        <asp:TextBox ID="txtLinkman" runat="server" MaxLength="20" ToolTip="联系人"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong><b>*</b>联系电话</strong>
                        <asp:TextBox ID="txtLinkmanTel" runat="server" MaxLength="50" ToolTip="联系电话"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong><b>*</b>活动主题</strong>
                        <asp:TextBox ID="txtTitle" runat="server" MaxLength="50" CssClass="long" ToolTip="活动主题"></asp:TextBox>
                        <strong><b>*</b>地点</strong>
                        <asp:TextBox ID="txtPerformSite" runat="server" MaxLength="100" CssClass="long" ToolTip="地点"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong><b>*</b>会议开始时间</strong>
                        <asp:TextBox ID="txtStartTime" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" ToolTip="开始时间"></asp:TextBox>
                        <strong><b>*</b>会议结束时间</strong>
                        <asp:TextBox ID="txtEndTime" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm', minDate:'#F{$dp.$D(\'txtStartTime\')}'})" ToolTip="结束时间"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong><b>*</b>报名截止时间</strong>
                        <asp:TextBox ID="txtOverTime" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm',maxDate:'#F{$dp.$D(\'txtStartTime\')}'})" ToolTip="报名截止时间"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong><b>*</b>签到开始时间</strong>
                        <asp:TextBox ID="txtSignTime" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm',maxDate:'#F{$dp.$D(\'txtStartTime\')}'})" ToolTip="签到开始时间"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong><b>*</b>用车情况</strong>
                        <asp:RadioButtonList ID="rblHaveBus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="用车情况"></asp:RadioButtonList>
                    </dd>
                    <dd>
                        <strong>其他</strong>
                        <asp:CheckBoxList ID="cblHaveDinner" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
                    </dd>
                    <dd>
                        <strong>出席领导</strong>
                        <asp:TextBox ID="txtLeaders" runat="server" CssClass="long"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong><b>*</b>参加委员</strong>
                        <asp:TextBox ID="txtAttendees" runat="server" TextMode="MultiLine" Rows="3" CssClass="readonly long" ToolTip="参加委员"></asp:TextBox>
                        <a id="btnAttendees" href="#" class="btn"><u>选取</u></a>
                    </dd>
                    <dd>
                        <strong>活动内容</strong>
                        <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" Rows="9" CssClass="long"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong>附件</strong>
                        <asp:HiddenField ID="hfFiles" runat="server" />
                        <a id="btnFiles" href="#" class="btn"><u>上传</u></a>
                        <div id="files"></div>
                    </dd>
                    <asp:PlaceHolder ID="plVerify" runat="server" Visible="false">
                        <dd>
                            <strong>退回原因</strong>
                            <asp:Literal ID="ltVerifyInfo" runat="server"></asp:Literal>
                        </dd>
                    </asp:PlaceHolder>
                </dl>
                <div class="cmd">
                    <asp:Button ID="btnSub" runat="server" Text="申请" OnClick="btnSub_Click" />
                    <asp:Button ID="btnSave" runat="server" Text="暂存" OnClick="btnSave_Click" />
                    <asp:Button ID="btnDel" runat="server" Text="删除" Visible="false" OnClick="btnDel_Click" />
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plMy" runat="server" Visible="false">
<script>
    $(function () {
        $('.list>dl>dd>p>a').each(function () {
            if ($(this).text() == '修改') {
                var url = $(this).attr('href') + '&ac=sub';
                $(this).attr('href', url);
            }
        })
    });
</script>
            <div class="list table">
                <dl>
                    <dt>我申请的会议/活动</dt>
                    <asp:Repeater ID="rpMyList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                <p>
                                    <b>参加方式：</b><%#DataBinder.Eval(Container.DataItem, "IsMust")%><br />
                                    <b>类型：</b><%#DataBinder.Eval(Container.DataItem, "SubType")%><br />
                                    <b>部门：</b><%#DataBinder.Eval(Container.DataItem, "OrgName")%><br />
                                    <b>主题：</b><%#DataBinder.Eval(Container.DataItem, "Title")%><br />
                                    <b>时间：</b><%#DataBinder.Eval(Container.DataItem, "PerformTimeText")%><br />
                                    <b>地点：</b><%#DataBinder.Eval(Container.DataItem, "PerformSite")%><br />
                                    <b>报名截止：</b><%#DataBinder.Eval(Container.DataItem, "OverTime", "{0:yyyy年M月d日 HH:mm}")%><br />
                                    <b>状态：</b><%#DataBinder.Eval(Container.DataItem, "ActiveName")%><br />
                                    <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u><%#DataBinder.Eval(Container.DataItem, "StateName")%></u></a>
                                </p>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltMyNo" runat="server">
                        <dd class="no">暂无申请的会议/活动！</dd>
                    </asp:Literal>
                </dl>
                <asp:Label ID="lblMyNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plSave" runat="server" Visible="false">
            <div class="list table">
                <dl>
                    <dt>暂存的会议/活动</dt>
                    <asp:Repeater ID="rpSaveList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                <p>
                                    <b>参加方式：</b><%#DataBinder.Eval(Container.DataItem, "IsMust")%><br />
                                    <b>类型：</b><%#DataBinder.Eval(Container.DataItem, "SubType")%><br />
                                    <b>部门：</b><%#DataBinder.Eval(Container.DataItem, "OrgName")%><br />
                                    <b>主题：</b><%#DataBinder.Eval(Container.DataItem, "Title")%><br />
                                    <b>时间：</b><%#DataBinder.Eval(Container.DataItem, "PerformTimeText")%><br />
                                    <b>地点：</b><%#DataBinder.Eval(Container.DataItem, "PerformSite")%><br />
                                    <b>报名截止：</b><%#DataBinder.Eval(Container.DataItem, "OverTime", "{0:yyyy年M月d日 HH:mm}")%><br />
                                    <b>状态：</b><%#DataBinder.Eval(Container.DataItem, "ActiveName")%><br />
                                    <a href="?ac=sub&id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>修改</u></a>
                                </p>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltSaveNo" runat="server">
                        <dd class="no">暂无暂存的会议/活动！</dd>
                    </asp:Literal>
                </dl>
                <asp:Label ID="lblSaveNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

    </div>
</form>
</body>
</html>
