<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="report.aspx.cs" Inherits="hkzx.web.m.report" %><%--Tony维护--%>
<%@ Register src="../cn/ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="ucHeader.ascx" tagname="ucHeader" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" Client="wap" />
    <title>虹口政协履职通 - 调研报告</title>
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
                } else if (url.indexOf('id=') > 0 && !$('#btnSub').val()) {
                    $('#plNav>div>#view').show().addClass('cur');
                } else {
                    //$('#plNav>div>a:first').addClass('cur');
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
    <asp:HiddenField ID="hfBack" runat="server" /><asp:HiddenField ID="hfOrg" runat="server" />
    <uc1:ucHeader ID="header1" runat="server" Title="调研报告" />
    <div class="content main">
        <asp:Panel ID="plNav" runat="server" CssClass="btn">
            <div>
                <a href="?ac=">提交调研报告</a>
                <a href="?ac=my">我的调研报告</a>
                <a href="?ac=save">暂存<b><asp:Literal ID="ltSaveNum" runat="server">0</asp:Literal></b></a>
                <a href="?ac=query">检索调研报告</a>
                <a id="view" class="hide">查阅调研报告</a>
            </div>
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
            showDialog(title, '../cn/dialog.aspx?ac=subman&obj=' + obj, '', 320, 480, 'no');
            return false;
        });
        $('#txtSubMan, #txtSubMans').focus(function () {
            $('#' + $(this).attr('id').replace('txt', 'btn')).click();
        });
        loadSelMenu('#hfOrg', '#txtOrgName', '#OrgName', 'm');
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
            <div class="edit">
                <dl>
                    <dt>提交调研报告</dt>
                    <dd>
                        <strong><b>*</b>提交人</strong>
                        <asp:TextBox ID="txtAddUser" runat="server" MaxLength="20" ReadOnly="True" CssClass="readonly" ToolTip="提交人"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong><b>*</b>是否建议案</strong>
                        <asp:RadioButtonList ID="rblIsPoint" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="是否建议案">
                            <asp:ListItem Text="是"></asp:ListItem>
                            <asp:ListItem Text="否" Selected="True"></asp:ListItem>
                        </asp:RadioButtonList>
                    </dd>
                    <dd>
                        <strong><b>*</b>提交部门(单位)</strong>
                        <asp:DropDownList ID="ddlOrgType" runat="server" Visible="false" ToolTip="提交部门(单位)">
                            <asp:ListItem Text="专委会"></asp:ListItem>
                            <asp:ListItem Text="界别"></asp:ListItem>
                            <asp:ListItem Text="街道活动组"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:TextBox ID="txtOrgName" runat="server" CssClass="readonly" ToolTip="提交部门(单位)"></asp:TextBox>
                        <div id="OrgName" class="selmenu"></div>
                    </dd>
                    <dd>
                        <strong><b>*</b>执笔人</strong>
                        <asp:TextBox ID="txtSubMan" runat="server" MaxLength="20" CssClass="readonly" ToolTip="执笔人"></asp:TextBox>
                        <a id="btnSubMan" href="#" class="btn"><u>选取</u></a>
                    </dd>
                    <dd>
                        <strong><b>*</b>课题组成员</strong>
                        <asp:TextBox ID="txtSubMans" runat="server" TextMode="MultiLine" Rows="3" CssClass="readonly long" ToolTip="课题组成员"></asp:TextBox>
                        <a id="btnSubMans" href="#" class="btn"><u>选取</u></a>
                    </dd>
                    <dd>
                        <strong><b>*</b>标题</strong>
                        <asp:TextBox ID="txtTitle" runat="server" MaxLength="50" CssClass="long" ToolTip="标题"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong>内容</strong>
                        <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" Rows="8" CssClass="long" ToolTip="内容"></asp:TextBox>
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
                    <asp:Button ID="btnDel" runat="server" Text="删除" OnClick="btnDel_Click" Visible="false" />
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plMy" runat="server" Visible="false">
            <div class="list table">
                <dl>
                    <dt>我的调研报告</dt>
                    <asp:Repeater ID="rpMyList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                <p>
                                    <b>部门：</b><%#DataBinder.Eval(Container.DataItem, "OrgName")%><br />
                                    <b>标题：</b><%#DataBinder.Eval(Container.DataItem, "Title")%><br />
                                    <b>执笔人：</b><%#DataBinder.Eval(Container.DataItem, "SubMan")%><br />
                                    <b>提交时间：</b><%#DataBinder.Eval(Container.DataItem, "SubTimeText")%><br />
                                    <b>状态：</b><%#DataBinder.Eval(Container.DataItem, "ActiveName")%><br />
                                    <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u><%#DataBinder.Eval(Container.DataItem, "StateName")%></u></a>
                                </p>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltMyNo" runat="server">
                        <dd class="no">暂无调研报告！</dd>
                    </asp:Literal>
                </dl>
                <asp:Label ID="lblMyNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plSave" runat="server" Visible="false">
            <div class="list table">
                <dl>
                    <dt>我的调研报告</dt>
                    <asp:Repeater ID="rpSaveList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                <p>
                                    <b>提交部门：</b><%#DataBinder.Eval(Container.DataItem, "OrgName")%><br />
                                    <b>报告标题：</b><%#DataBinder.Eval(Container.DataItem, "Title")%><br />
                                    <b>执笔人：</b><%#DataBinder.Eval(Container.DataItem, "SubMan")%><br />
                                    <b>编辑日期：</b><%#DataBinder.Eval(Container.DataItem, "UpTime", "{0:yyyy-MM-dd HH:mm:ss}")%><br />
                                    <b>状态：</b><%#DataBinder.Eval(Container.DataItem, "ActiveName")%><br />
                                    <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>选取</u></a>
                                </p>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltSaveNo" runat="server">
                        <dd class="no">暂无调研报告！</dd>
                    </asp:Literal>
                </dl>
                <asp:Label ID="lblSaveNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plQuery" runat="server" Visible="false">
<script>
    $(function () {
        loadSelMenu('#hfOrg', '#txtQOrgName', '#QOrgName', 'm');
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
            showDialog(title, '../cn/dialog.aspx?ac=subman&obj=' + obj, '', 320, 480, 'no');
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
            <div class="edit query">
                <div class="label">查询条件</div>
                <dl>
                    <dd>
                        <strong>提交部门(单位)</strong>
                        <asp:DropDownList ID="ddlQOrgType" runat="server" Visible="false" ToolTip="提交部门(单位)">
                            <asp:ListItem Text="专委会"></asp:ListItem>
                            <asp:ListItem Text="界别"></asp:ListItem>
                            <asp:ListItem Text="街道活动组"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:TextBox ID="txtQOrgName" runat="server" CssClass="readonly long" ToolTip="提交部门(单位)"></asp:TextBox>
                        <div id="QOrgName" class="selmenu"></div>
                    </dd>
                    <dd>
                        <strong>标题</strong>
                        <asp:TextBox ID="txtQTitle" runat="server" MaxLength="50" CssClass="long"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong>是否建议案</strong>
                        <asp:DropDownList ID="ddlQIsPoint" runat="server">
                            <asp:ListItem Text="全部" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </dd>
                    <dd>
                        <strong>执笔人</strong>
                        <asp:TextBox ID="txtQSubMan" runat="server" MaxLength="20" CssClass="readonly" ToolTip="执笔人"></asp:TextBox>
                        <a id="btnQSubMan" href="#" class="btn"><u>选取</u></a>
                    </dd>
                    <dd>
                        <strong>课题组成员</strong>
                        <asp:TextBox ID="txtQSubMans" runat="server" TextMode="MultiLine" Rows="3" CssClass="readonly long" ToolTip="课题组成员"></asp:TextBox>
                        <a id="btnQSubMans" href="#" class="btn"><u>选取</u></a>
                    </dd>
                    <dd>
                        <strong>提交人</strong>
                        <asp:TextBox ID="txtQAddUser" runat="server" MaxLength="20"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong>提交日期</strong>
                        <asp:TextBox ID="txtQSubTime1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox>
                        <asp:TextBox ID="txtQSubTime2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox>
                    </dd>
                </dl>
                <div class="cmd">
                    <input id="btnQuery" type="submit" value="查询" />
                    <input id="clean" type="reset" value="清空" />
                </div>
            </div>
            <div class="list table">
                <dl>
                    <dt class="total">符合条件的数据有：<b><asp:Literal ID="ltQueryTotal" runat="server" Text="0"></asp:Literal></b>条</dt>
                    <asp:Repeater ID="rpQueryList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                <p>
                                    <b>提交部门：</b><%#DataBinder.Eval(Container.DataItem, "OrgName")%><br />
                                    <b>报告标题：</b><%#DataBinder.Eval(Container.DataItem, "Title")%><br />
                                    <b>执笔人：</b><%#DataBinder.Eval(Container.DataItem, "SubMan")%><br />
                                    <b>提交日期：</b><%#DataBinder.Eval(Container.DataItem, "SubTime", "{0:yyyy-MM-dd HH:mm:ss}")%><br />
                                    <b>状态：</b><%#DataBinder.Eval(Container.DataItem, "ActiveName")%><br />
                                    <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看</u></a>
                                </p>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltQueryNo" runat="server">
                        <dd class="no">暂时没有查询到调研报告！</dd>
                    </asp:Literal>
                </dl>
                <asp:Label ID="lblQueryNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plView" runat="server" Visible="false">
            <div class="edit">
                <div class="label">查阅调研报告</div>
                <table>
                    <tbody>
                        <tr>
                            <th>提交人</th>
                            <td><asp:Literal ID="ltAddUser" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>提交时间</th>
                            <td><asp:Literal ID="ltSubTime" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>提交部门<br />(单位)</th>
                            <td><asp:Literal ID="ltOrgName" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>是否建议案</th>
                            <td><asp:Literal ID="ltIsPoint" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>执笔人</th>
                            <td><asp:Literal ID="ltSubMan" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>课题组成员</th>
                            <td><asp:Literal ID="ltSubMans" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>标题</th>
                            <td><asp:Literal ID="ltTitle" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>内容</th>
                            <td><asp:Literal ID="ltBody" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>附件</th>
                            <td><asp:Literal ID="ltFiles" runat="server"></asp:Literal></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </asp:PlaceHolder>
    </div>
</form>
</body>
</html>
