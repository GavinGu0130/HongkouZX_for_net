<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="report.aspx.cs" Inherits="hkzx.web.admin.report" %><%--Tony维护--%>
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
                if (url.indexOf('id=') > 0) {
                    $('#plNav>a#view').show().addClass('cur');
                } else {
                    $('#plNav>a:first').addClass('cur').removeAttr('href');
                }
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
    <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    <asp:HiddenField ID="hfBack" runat="server" Value="./" /><asp:HiddenField ID="hfOrg" runat="server" />
    <uc1:ucHeader ID="header1" runat="server" />
    <div class="content">
        <div class="main">
            <asp:Panel ID="plNav" runat="server" Visible="false" CssClass="btn">
                <a href="?ac=">检索调研报告</a>
                <a id="view" class="hide">审核调研报告</a>
            </asp:Panel>

            <asp:PlaceHolder ID="plQuery" runat="server" Visible="false">
<script>
    $(function () {
        loadSelMenu('#hfOrg', '#txtQOrgName', '#QOrgName', '');
        $('#btnQSubMan, #btnQSubMans').click(function () {
            var title = '';
            var obj = '';
            switch ($(this).attr('id')) {
                case 'btnQSubMan':
                    title = '选取执笔人';
                    obj = 'txtQPenner';
                    break;
                default:
                    title = '选取课题组成员';
                    obj = 'txtQSubMans';
                    break;
            }
            showDialog(title, '../cn/dialog.aspx?ac=subman&obj=' + obj, '', 640, 400, 'no');
            return false;
        });
        $('#btnQuery').click(function () {
            try {
                var url = '';
                if ($('#QOrgName').val()) {
                    url += '&OrgName=' + $('#QOrgName').val();
                }
                if ($('#txtQIsPoint').val()) {
                    url += '&IsPoint=' + $('#txtQIsPoint').val();
                }
                if ($('#txtQSubMan').val()) {
                    url += '&SubMan=' + $('#txtQSubMan').val();
                }
                if ($('#txtQSubMans').val()) {
                    url += '&SubMans=' + $('#txtQSubMans').val();
                }
                if ($('#txtQTitle').val()) {
                    url += '&Title=' + $('#txtQTitle').val();
                }
                if ($('#txtQAddUser').val()) {
                    url += '&AddUser=' + $('#txtQAddUser').val();
                }
                if ($('#txtQSubTime1').val() || $('#txtQSubTime2').val()) {
                    url += '&SubTime=' + $('#txtQSubTime1').val() + ',' + $('#txtQSubTime2').val();
                }
                var tmp = getChecked('#cblQActive', ',');
                if (tmp != '') {
                    url += '&Active=' + tmp;
                }
                //alert(encodeURI(url)); return false;
                if (url != '') {
                    window.location.href = '?ac=query' + encodeURI(url);
                } else {
                    window.location.href = '?ac=query';
                }
                return false;
            } catch (err) {
                alert("验证出错，请稍后重试！");
                return false;
            }
        });
        $('#btnPass, #btnBack, #btnDel').click(function () {
            if (!$('.list>table>tbody>tr>td>input:checkbox').is(':checked')) {
                alert('请先选取要操作的数据！');
                return false;
            }
            if ($(this).attr('id') == 'btnBack') {
                var txt = prompt('请填写退回原因：');
                if (txt) {
                    $('#hfVerifyInfo').val(txt);
                } else {
                    return false;
                }
            } else if (!confirm('您确定要“' + $(this).val() + '”吗?')) {
                return false;
            }
        });
    });
</script>
                <div class="frm edit">
                    <strong>检索调研报告</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>标题</th>
                                <td><asp:TextBox ID="txtQTitle" runat="server" MaxLength="50" CssClass="long"></asp:TextBox></td>
                                <th>状态</th>
                                <td>
                                    <asp:CheckBoxList ID="cblQActive" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="审核通过"></asp:ListItem>
                                        <asp:ListItem Text="提交"></asp:ListItem>
                                        <asp:ListItem Text="退回"></asp:ListItem>
                                        <asp:ListItem Text="删除"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th>提交部门(单位)</th>
                                <td>
                                    <asp:TextBox ID="txtQOrgName" runat="server" CssClass="readonly long" ToolTip="提交部门(单位)"></asp:TextBox>
                                    <div id="QOrgName" class="selmenu"></div>
                                </td>
                                <th>是否建议案</th>
                                <td>
                                    <asp:DropDownList ID="ddlQIsPoint" runat="server">
                                        <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                        <asp:ListItem Text="是"></asp:ListItem>
                                        <asp:ListItem Text="否"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>执笔人</th>
                                <td><asp:TextBox ID="txtQSubMan" runat="server" MaxLength="20"></asp:TextBox>
                                    <a id="btnQSubMan" href="#" class="btn"><u>选取</u></a>
                                </td>
                                <th>课题组成员</th>
                                <td><asp:TextBox ID="txtQSubMans" runat="server" CssClass="long"></asp:TextBox>
                                    <a id="btnQSubMans" href="#" class="btn"><u>选取</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th>提交时间</th>
                                <td><asp:TextBox ID="txtQSubTime1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox> - <asp:TextBox ID="txtQSubTime2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtQSubTime1\')}'})"></asp:TextBox></td>
                                <th>提交人</th>
                                <td><asp:TextBox ID="txtQAddUser" runat="server" MaxLength="20"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <input id="btnQuery" type="button" value="查询" />
                        <input id="clean" type="reset" value="清空" />
                    </div>
                </div>
                <div class="cmd">
                    <asp:Button ID="btnPass" runat="server" Text="审核通过" OnClick="btnPass_Click" />
                    <asp:Button ID="btnBack" runat="server" Text="退回" OnClick="btnBack_Click" /><asp:HiddenField ID="hfVerifyInfo" runat="server" />
                    <asp:Button ID="btnDel" runat="server" Text="删除" OnClick="btnDel_Click" />
                    <%--<asp:Button ID="btnSave" runat="server" Text="暂存" OnClick="btnSave_Click" />--%>
                </div>
                <div class="list hover">
                    <strong>结果展现
                        <span>符合条件的数据有：<b><asp:Literal ID="ltQueryTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>提交部门(单位)</th>
                                <th>标题</th>
                                <th class="state">执笔人</th>
                                <th class="date">提交时间</th>
                                <th class="state">提交人</th>
                                <th class="date">审核时间</th>
                                <th class="state">状态</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpQueryList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OrgName")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SubMan")%></td>
                                        <td align="center" title="<%#DataBinder.Eval(Container.DataItem, "SubTime", "{0:yyyy-MM-dd HH:mm:ss}")%>"><%#DataBinder.Eval(Container.DataItem, "SubTimeText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "AddUser")%></td>
                                        <td align="center" title="<%#DataBinder.Eval(Container.DataItem, "VerifyTime", "{0:yyyy-MM-dd HH:mm:ss}")%>"><%#DataBinder.Eval(Container.DataItem, "VerifyTimeText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td>
                                            <asp:CheckBox ID="_ck" runat="server" /><asp:HiddenField ID="_id" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "Id") %>'/>
                                            <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>选取</u></a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltQueryNo" runat="server">
                                <tr>
                                    <td class="no">暂无调研报告！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblQueryNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plEdit" runat="server" Visible="false">
<script>
    $(function () {
        upFile('#hfFiles', '#files', '#btnFiles', 'file');
        $('#btnSubMan, #btnSubMans').click(function () {
            var title = '';
            var obj = '';
            switch ($(this).attr('id')) {
                case 'btnSubMan':
                    title = '选取执笔人';
                    obj = 'txtSubMan';
                    break;
                default:
                    title = '选取课题组成员';
                    obj = 'txtSubMans';
                    break;
            }
            showDialog(title, '../cn/dialog.aspx?ac=subman&obj=' + obj, '', 640, 400, 'no');
            return false;
        });
        loadSelMenu('#hfOrg', '#txtOrgName', '#OrgName', '');
        $('form').submit(function () {
            try {
                if (checkRadio('#rblIsPoint') || checkEmpty('#txtSubMan') || checkEmpty('#txtSubMans') || checkEmpty('#txtTitle')) {
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
                    <strong>审核调研报告</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>提交人</th>
                                <td><asp:Literal ID="ltAddUser" runat="server"></asp:Literal></td>
                                <th>提交时间</th>
                                <td><asp:Literal ID="ltSubTime" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>状态</th>
                                <td>
                                    <asp:DropDownList ID="ddlActiveName" runat="server">
                                        <asp:ListItem Text="审核通过"></asp:ListItem>
                                        <asp:ListItem Text="提交"></asp:ListItem>
                                        <asp:ListItem Text="退回"></asp:ListItem>
                                        <asp:ListItem Text="删除"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <th><b>*</b>是否建议案</th>
                                <td>
                                    <asp:RadioButtonList ID="rblIsPoint" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="是否建议案">
                                        <asp:ListItem Text="是"></asp:ListItem>
                                        <asp:ListItem Text="否" Selected="True"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>提交部门(单位)</th>
                                <td>
                                    <asp:DropDownList ID="ddlOrgType" runat="server" Visible="false" ToolTip="提交部门(单位)">
                                        <asp:ListItem Text="专委会"></asp:ListItem>
                                        <asp:ListItem Text="界别"></asp:ListItem>
                                        <asp:ListItem Text="街道活动组"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:TextBox ID="txtOrgName" runat="server" CssClass="readonly long" ToolTip="提交部门(单位)"></asp:TextBox>
                                    <div id="OrgName" class="selmenu"></div>
                                </td>
                                <th rowspan="2"><b>*</b>课题组成员</th>
                                <td rowspan="2"><asp:TextBox ID="txtSubMans" runat="server" TextMode="MultiLine" Rows="3" CssClass="long" ToolTip="课题组成员"></asp:TextBox>
                                    <a id="btnSubMans" href="#" class="btn"><u>选取</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>执笔人</th>
                                <td><asp:TextBox ID="txtSubMan" runat="server" MaxLength="20" ToolTip="执笔人"></asp:TextBox>
                                    <a id="btnSubMan" href="#" class="btn"><u>选取</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>标题</th>
                                <td colspan="3"><asp:TextBox ID="txtTitle" runat="server" MaxLength="50" CssClass="long" ToolTip="标题"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>内容</th>
                                <td colspan="3"><asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" Rows="8" CssClass="long" ToolTip="内容"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>附件</th>
                                <td colspan="3">
                                    <asp:HiddenField ID="hfFiles" runat="server" />
                                    <a id="btnFiles" href="#" class="btn"><u>上传</u></a>
                                    <div id="files"></div>
                                </td>
                            </tr>
                            <tr>
                                <th>备注</th>
                                <td colspan="3"><asp:TextBox ID="txtRemark" runat="server" TextMode="MultiLine" Rows="8" CssClass="long" ToolTip="备注"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnSub" runat="server" Text="审核" OnClick="btnSub_Click" />
                        <asp:Button ID="btnDel2" runat="server" Text="删除" OnClick="btnDel2_Click" Visible="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
