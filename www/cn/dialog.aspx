<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dialog.aspx.cs" Inherits="hkzx.web.cn.dialog" %><%--Tony维护--%>
<%@ Register src="ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="../m/ucHeader.ascx" tagname="ucHeaderM" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" />
    <title>对话框</title>
    <style>
        html { background:#FFF;}
        body { background:#FFF;}
    </style>
</head>
<body>
<form id="form1" runat="server">
    <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    <asp:HiddenField ID="hfObj" runat="server" />
    <uc1:ucHeaderM ID="header2" runat="server" Visible="false" Cur="login" />
    <div class="dialog">
        <asp:PlaceHolder ID="plSelUser" runat="server" Visible="false">
<script>
    $(function () {
        function loadSelUser() {
            var obj = '#' + $('#hfObj').val();
            var str = $(obj, window.parent.document).val();
            if (str) {
                var arr = str.split(',');
                for (i = 0; i < arr.length; i++) {
                    $('#selUsered').append('<span>' + arr[i] + '</span>');
                    $('#selUsers>span').each(function () {
                        if (arr[i] == $(this).text()) {
                            $(this).remove();
                        }
                    });
                }
            }
        }
        loadSelUser();
        $('#btnSelUser').click(function () {
            var obj = '#' + $('#hfObj').val();
            var str = '';
            $('#selUsered>span').each(function () {
                if (str) {
                    str += ',';
                }
                str += $(this).text();
            });
            //if (str.indexOf(',') > 0 && obj.lastIndexOf('2') == obj.length - 1) {
            //    alert('只能选取1位委员');
            //    return;
            //}
            $(obj, window.parent.document).val(str);
            top.window.dialog.remove();
        });
        $('#btnAddUser, #btnAddUsers').click(function () {
            var isSel = ($(this).attr('id') == 'btnAddUser') ? true : false;
            $('#selUsers>span').each(function () {
                if (!isSel || $(this).hasClass('cur')) {
                    $('#selUsered').append('<span id="' + $(this).attr('id') + '">' + $(this).text() + '</span>');
                    $(this).remove();
                }
            });
            return false;
        });
        $('#btnDelUser, #btnDelUsers').click(function () {
            var isSel = ($(this).attr('id') == 'btnDelUser') ? true : false;
            $('#selUsered>span').each(function () {
                if (!isSel || $(this).hasClass('cur')) {
                    $('#selUsers').append('<span id="' + $(this).attr('id') + '">' + $(this).text() + '</span>');
                    $(this).remove();
                }
            });
            return false;
        });
        $(document).on('click', 'div.list>table>tbody>tr>td>div>span', function () {
            if ($(this).hasClass('cur')) {
                $(this).removeClass('cur');
            } else {
                $(this).addClass('cur');
            }
        });
    });
</script>
            <div class="list">
                <table>
                    <thead>
                        <tr>
                            <td colspan="3">
                                <asp:DropDownList ID="ddlQSelCommittee" runat="server">
                                    <asp:ListItem Text="==专委会==" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:DropDownList ID="ddlQSelSubsector" runat="server">
                                    <asp:ListItem Text="==界别==" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:DropDownList ID="ddlQSelStreetTeam" runat="server">
                                    <asp:ListItem Text="==街道活动组==" Value=""></asp:ListItem>
                                </asp:DropDownList><br />
                                <asp:DropDownList ID="ddlQSelParty" runat="server">
                                    <asp:ListItem Text="==政治面貌==" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:DropDownList ID="ddlQSelUserType" runat="server">
                                    <asp:ListItem Text="==在册委员==" Value="在册委员"></asp:ListItem>
                                </asp:DropDownList><br />
                                <asp:CheckBoxList ID="cblQSelRole" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList><br />
                                <%--<asp:DropDownList ID="ddlQSelPeriod" runat="server"></asp:DropDownList>届--%>
                                委员姓名<asp:TextBox ID="txtQSelUser" runat="server" CssClass="txt"></asp:TextBox>
                                &nbsp;<asp:Button ID="btnQSelUser" runat="server" Text="查找" OnClick="btnQSelUser_Click" />
                            </td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td class="sel">
                                <div id="selUsers">
                                    <asp:Repeater ID="rpListUser" runat="server">
                                        <ItemTemplate>
                                            <span id="<%#DataBinder.Eval(Container.DataItem, "UserCode")%>"><%#DataBinder.Eval(Container.DataItem, "TrueName")%></span>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </td>
                            <th class="cmd">
                                <a id="btnAddUsers" href="#"><u>全部&gt;&gt;</u></a>
                                <a id="btnAddUser" href="#"><u>添加&gt;&gt;</u></a>
                                <br />
                                <a id="btnDelUser" href="#"><u>&lt;&lt;删除</u></a>
                                <a id="btnDelUsers" href="#"><u>&lt;&lt;全部</u></a>
                            </th>
                            <td>
                                <div id="selUsered"></div>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <div class="cmd">
                    <input id="btnSelUser" type="button" value="确定" />
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plScore" runat="server" Visible="false">
            <asp:Panel ID="plYear" runat="server" Visible="false" CssClass="score">
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
                <b>总积分：</b><asp:Literal ID="ltScore" runat="server">0</asp:Literal>
                <b>会议活动：</b><asp:Literal ID="ltScore1" runat="server">0</asp:Literal>
                <b>建言得分：</b><asp:Literal ID="ltScore2" runat="server">0</asp:Literal>
            </asp:Panel>
            <div class="list hover">
                <table>
                    <thead>
                        <tr>
                            <th>序号</th>
                            <th>说明</th>
                            <th>得分</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rpScoreList" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                    <td><%#DataBinder.Eval(Container.DataItem, "Title")%>
                                        <p><%#DataBinder.Eval(Container.DataItem, "other")%></p>
                                    </td>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "ScoreText")%></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
                <asp:Label ID="lblScoreNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plSpeak" runat="server" Visible="false">
            <div class="list hover">
                <table>
                    <thead>
                        <tr>
                            <th>序号</th>
                            <th>活动类型</th>
                            <th>主题</th>
                            <th>会议发言</th>
                            <th>发言人</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rpSpeakList" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                    <td><%#DataBinder.Eval(Container.DataItem, "PerformSubType")%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "PerformTitle")%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "SignManSpeak")%></td>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "SignMan")%></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
                <asp:Label ID="lblSpeakNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plPerform" runat="server" Visible="false">
            <div class="list hover">
                <table>
                    <thead>
                        <tr>
                            <th>序号</th>
                            <th>活动类型</th>
                            <th>主题</th>
                            <th>起止时间</th>
                            <th>状态</th>
                            <th>参与人数</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rpPerformList" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                    <td><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "StartTime", "{0:yyyy年M月d日 HH:mm}")%><br /><%#DataBinder.Eval(Container.DataItem, "EndTime", "{0:yyyy年M月d日 HH:mm}")%></td>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "FeedNum")%></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
                <asp:Label ID="lblPerformNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plOpinion" runat="server" Visible="false">
            <div class="list hover">
                <table>
                    <thead>
                        <tr>
                            <th>序号</th>
                            <th>提案号</th>
                            <th>类别</th>
                            <th>案由</th>
                            <th>第一提案人</th>
                            <th>状态</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rpOpinionList" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "OpNo")%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "Summary")%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "SubMan")%></td>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
                <asp:Label ID="lblOpinionNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plPop" runat="server" Visible="false">
            <div class="list hover">
                <table>
                    <thead>
                        <tr>
                            <th>序号</th>
                            <th>分类</th>
                            <th>标题</th>
                            <th>第一反映人</th>
                            <th>状态</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rpPopList" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                    <td><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "Summary")%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "SubMan")%></td>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
                <asp:Label ID="lblPopNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plOpSign" runat="server" Visible="false">
            <asp:HiddenField ID="hfSignOpId" runat="server" Value="0" /><asp:HiddenField ID="hfSignUserId" runat="server" Value="0" />
            <div class="edit">
                <table>
                    <tbody>
                        <tr>
                            <th>流水号</th>
                            <td><asp:TextBox ID="txtSignId" runat="server" ReadOnly="true" CssClass="readonly" Text="0"></asp:TextBox></td>
                            <th>状态</th>
                            <td>
                                <asp:RadioButtonList ID="rblSignActive" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                    <asp:ListItem Text="已签" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="待签" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="谢绝会签" Value="-1"></asp:ListItem>
                                    <asp:ListItem Text="取消" Value="-100"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <th><b>*</b>委员姓名</th>
                            <td><asp:TextBox ID="txtSignUser" runat="server" ReadOnly="true" CssClass="readonly" MaxLength="20" ToolTip="委员姓名"></asp:TextBox></td>
                            <th><b>*</b>签名结束</th>
                            <td><asp:TextBox ID="txtSignOverdue" runat="server" MaxLength="20" ToolTip="签名结束" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})"></asp:TextBox></td>
<%--                            <th><b>*</b>时间标识</th>
                            <td>
                                <asp:RadioButtonList ID="rblSignActiveName" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                    <asp:ListItem Text="会间"></asp:ListItem>
                                    <asp:ListItem Text="会后"></asp:ListItem>
                                    <asp:ListItem Text="_" Value=""></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>--%>
                        </tr>
                        <tr>
                            <th>签名时间</th>
                            <td><asp:TextBox ID="txtSignTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            <th>签名IP</th>
                            <td><asp:TextBox ID="txtSignIp" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <th>签名意见</th>
                            <td colspan="3"><asp:TextBox ID="txtSignBody" runat="server" ReadOnly="true" CssClass="readonly long" TextMode="MultiLine" Rows="4"></asp:TextBox></td>
                        </tr>
                    </tbody>
                </table>
                <div class="cmd">
                    <asp:Button ID="btnSign" runat="server" Text="修改" OnClick="btnSign_Click" />
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plMsg" runat="server" Visible="false">
            <div class="list hover">
                <table>
                    <thead>
                        <tr>
                            <th>序号</th>
                            <th>内容/备注</th>
                            <th>发送人/时间</th>
                            <th>标识</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rpMsgList" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                    <td><%#DataBinder.Eval(Container.DataItem, "Body")%>
                                        <p><%#DataBinder.Eval(Container.DataItem, "Remark")%></p><%#DataBinder.Eval(Container.DataItem, "other")%>
                                    </td>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "AddUser")%><br /><%#DataBinder.Eval(Container.DataItem, "AddTime", "{0:yyyy-MM-dd HH:mm:ss}")%><br /><%#DataBinder.Eval(Container.DataItem, "AddIp")%></td>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "Label")%></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
                <asp:Label ID="lblMsgNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>
    </div>
</form>
</body>
</html>
