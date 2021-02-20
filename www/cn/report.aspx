<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="report.aspx.cs" Inherits="hkzx.web.cn.report" %><%--Tony维护--%>
<%@ Register src="ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="ucHeader.ascx" tagname="ucHeader" tagprefix="uc1" %>
<%@ Register src="ucFooter.ascx" tagname="ucFooter" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" />
    <title>虹口政协履职通</title>
    <script>
        $(function () {
            if ($('#plNav').text()) {
                var url = window.location.href;
                if (url.indexOf('ac=query') > 0) {
                    $('#plNav>a[href*="ac=query"]').addClass('cur');
                } else if (url.indexOf('ac=my') > 0) {
                    $('#plNav>a[href*="ac=my"]').addClass('cur');
                } else if (url.indexOf('ac=save') > 0) {
                    $('#plNav>a[href*="ac=save"]').addClass('cur');
                } else if (url.indexOf('id=') > 0 && !$('#btnSub').val()) {
                    $('#plNav>#view').show().addClass('cur');
                } else {
                    //$('#plNav>a:first').addClass('cur');
                    $('#plNav>a[href$="ac="]').addClass('cur');
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
    <asp:HiddenField ID="hfBack" runat="server" /><asp:HiddenField ID="hfOrg" runat="server" />
    <uc1:ucHeader ID="header1" runat="server" />
    <div class="content">
        <div class="main">
            <asp:Panel ID="plNav" runat="server" Visible="false" CssClass="btn">
                <a href="?ac=">提交调研报告</a>
                <a href="?ac=my">我的调研报告</a>
                <a href="?ac=save">暂存<b>(<asp:Literal ID="ltSaveNum" runat="server">0</asp:Literal>)</b></a>
                <a href="?ac=query">检索调研报告</a>
                <a id="view" class="hide">查阅调研报告</a>
            </asp:Panel>

            <asp:PlaceHolder ID="plSub" runat="server" Visible="false">
<script>
    $(function () {
        upFile('#hfFiles', '#files', '#btnFiles', 'doc');
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
            showDialog(title, 'dialog.aspx?ac=subman&obj=' + obj, '', 640, 400, 'no');
            return false;
        });
        $('#txtSubMan, #txtSubMans').focus(function () {
            $('#' + $(this).attr('id').replace('txt', 'btn')).click();
        });
        loadSelMenu('#hfOrg', '#txtOrgName', '#OrgName', '');
        $('form').submit(function () {
            try {
                if (checkRadio('#rblIsPoint') || checkEmpty('#txtSubMan') || checkEmpty('#txtSubMans') || checkEmpty('#txtTitle') || checkEmpty('#txtAddUser')) {
                    return false;
                }
                if ($('#txtBody').val() == '' && $('#hfFiles').val() == '') {
                    alert('请填写[内容]或上传[附件]');
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
                <div class="frm edit">
                    <strong>提交调研报告</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th><b>*</b>提交人</th>
                                <td><asp:TextBox ID="txtAddUser" runat="server" MaxLength="20" ReadOnly="True" CssClass="readonly" ToolTip="提交人"></asp:TextBox></td>
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
                                    <asp:TextBox ID="txtOrgName" runat="server" CssClass="readonly" ToolTip="提交部门(单位)"></asp:TextBox>
                                    <div id="OrgName" class="selmenu"></div>
                                </td>
                                <th rowspan="2"><b>*</b>课题组成员</th>
                                <td rowspan="2"><asp:TextBox ID="txtSubMans" runat="server" TextMode="MultiLine" Rows="3" CssClass="readonly long" ToolTip="课题组成员"></asp:TextBox>
                                    <a id="btnSubMans" href="#" class="btn"><u>选取</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>执笔人</th>
                                <td><asp:TextBox ID="txtSubMan" runat="server" MaxLength="20" CssClass="readonly" ToolTip="执笔人"></asp:TextBox>
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
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnSub" runat="server" Text="提交" OnClick="btnSub_Click" />
                        <asp:Button ID="btnSave" runat="server" Text="暂存" OnClick="btnSave_Click" />
                        <asp:Button ID="btnDel" runat="server" Text="删除" OnClick="btnDel_Click" Visible="false" />
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plMy" runat="server" Visible="false">
                <div class="frm list hover">
                    <strong>我的调研报告
                        <span>符合条件的数据有：<b><asp:Literal ID="ltMyTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>提交部门(单位)</th>
                                <th>标题</th>
                                <th class="state">执笔人</th>
                                <th class="date">提交日期</th>
                                <th class="state">状态</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpMyList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OrgName")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubMan")%></td>
                                        <td align="center" title="<%#DataBinder.Eval(Container.DataItem, "SubTime", "{0:yyyy-MM-dd HH:mm:ss}")%>"><%#DataBinder.Eval(Container.DataItem, "SubTimeText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td><a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u><%#DataBinder.Eval(Container.DataItem, "StateName")%></u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltMyNo" runat="server">
                                <tr>
                                    <td class="no">暂无调研报告！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblMyNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plSave" runat="server" Visible="false">
                <div class="frm list hover">
                    <strong>暂存的调研报告</strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>提交部门(单位)</th>
                                <th>标题</th>
                                <th class="state">执笔人</th>
                                <th class="time">编辑时间</th>
                                <th class="state">状态</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpSaveList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OrgName")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubMan")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "UpTime", "{0:yyyy-MM-dd HH:mm:ss}")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>选取</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltSaveNo" runat="server">
                                <tr>
                                    <td class="no">暂无调研报告！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblSaveNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

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
                    obj = 'txtQSubMan';
                    break;
                default:
                    title = '选取课题组成员';
                    obj = 'txtQSubMans';
                    break;
            }
            showDialog(title, 'dialog.aspx?ac=subman&obj=' + obj, '', 640, 400, 'no');
            return false;
        });
        $('#txtQSubMan, #txtQSubMans').focus(function () {
            $('#' + $(this).attr('id').replace('txt', 'btn')).click();
        });
        $('form').submit(function () {
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
                //alert(encodeURI(url)); return false;
                if (url != '') {
                    window.location.href = '?ac=query' + encodeURI(url);
                } else {
                    window.location.href = '?ac=query';
                }
                return false;
            } catch (err) {
                //alert("验证出错，请稍后重试！");
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
                                <th>提交部门(单位)</th>
                                <td>
                                    <asp:DropDownList ID="ddlQOrgType" runat="server" Visible="false" ToolTip="提交部门(单位)">
                                        <asp:ListItem Text="专委会"></asp:ListItem>
                                        <asp:ListItem Text="界别"></asp:ListItem>
                                        <asp:ListItem Text="街道活动组"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:TextBox ID="txtQOrgName" runat="server" CssClass="readonly long" ToolTip="提交部门(单位)"></asp:TextBox>
                                    <div id="QOrgName" class="selmenu"></div>
                                </td>
                                <th>提交日期</th>
                                <td><asp:TextBox ID="txtQSubTime1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox> - <asp:TextBox ID="txtQSubTime2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>标题</th>
                                <td><asp:TextBox ID="txtQTitle" runat="server" MaxLength="50" CssClass="long"></asp:TextBox></td>
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
                                <td><asp:TextBox ID="txtQSubMan" runat="server" MaxLength="20" CssClass="readonly" ToolTip="执笔人"></asp:TextBox>
                                    <a id="btnQSubMan" href="#" class="btn"><u>选取</u></a>
                                </td>
                                <th rowspan="2">课题组成员</th>
                                <td rowspan="2"><asp:TextBox ID="txtQSubMans" runat="server" TextMode="MultiLine" Rows="3" CssClass="readonly long" ToolTip="课题组成员"></asp:TextBox>
                                    <a id="btnQSubMans" href="#" class="btn"><u>选取</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th>提交人</th>
                                <td><asp:TextBox ID="txtQAddUser" runat="server" MaxLength="20"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <input id="btnQuery" type="submit" value="查询" />
                        <input id="clean" type="reset" value="清空" />
                    </div>
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
                                <th class="state">提交人</th>
                                <th class="date">提交日期</th>
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
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "AddUser")%></td>
                                        <td align="center" title="<%#DataBinder.Eval(Container.DataItem, "SubTime", "{0:yyyy-MM-dd HH:mm:ss}")%>"><%#DataBinder.Eval(Container.DataItem, "SubTimeText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看</u></a></td>
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

            <asp:PlaceHolder ID="plView" runat="server" Visible="false">
                <div class="frm edit">
                    <strong>查阅调研报告</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>提交人</th>
                                <td><asp:Literal ID="ltAddUser" runat="server"></asp:Literal></td>
                                <th>提交时间</th>
                                <td><asp:Literal ID="ltSubTime" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>提交部门(单位)</th>
                                <td><asp:Literal ID="ltOrgName" runat="server"></asp:Literal></td>
                                <th>是否建议案</th>
                                <td><asp:Literal ID="ltIsPoint" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>执笔人</th>
                                <td><asp:Literal ID="ltSubMan" runat="server"></asp:Literal></td>
                                <th>课题组成员</th>
                                <td><asp:Literal ID="ltSubMans" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>标题</th>
                                <td colspan="3"><asp:Literal ID="ltTitle" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>内容</th>
                                <td colspan="3"><asp:Literal ID="ltBody" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>附件</th>
                                <td colspan="3"><asp:Literal ID="ltFiles" runat="server"></asp:Literal></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>

