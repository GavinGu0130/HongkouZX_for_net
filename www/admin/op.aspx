<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="op.aspx.cs" Inherits="hkzx.web.admin.op" %><%--Tony维护--%>
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
    <asp:HiddenField ID="hfBack" runat="server" Value="./" />
    <uc1:ucHeader ID="header1" runat="server" />
    <div class="content">
        <div class="main">
            <asp:PlaceHolder ID="plType" runat="server" Visible="false">
<script>
    $(function () {
        if ($('#btnTypeEdit').val() == '修改') {
            $('#nav1>a.hide').show().addClass('cur');
        } else {
            $('#nav1>a:first').addClass('cur').removeAttr('href');
        }
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtTypeName')) {
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
                <div id="nav1" class="btn">
                    <a href="?id=">新增分类</a>
                    <a class="hide">修改分类</a>
                </div>
                <div class="frm edit">
                    <strong><asp:Literal ID="ltTypeTitle" runat="server" Text="新增选项分类"></asp:Literal></strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>自编号</th>
                                <td><asp:TextBox ID="txtTypeId" runat="server" Text="0" CssClass="readonly" ReadOnly="true"></asp:TextBox></td>
                                <th rowspan="3">备注</th>
                                <td rowspan="3"><asp:TextBox ID="txtTypeRemark" runat="server" TextMode="MultiLine" Rows="5" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>分类名称</th>
                                <td><asp:TextBox ID="txtTypeName" runat="server" ToolTip="分类名称" MaxLength="20"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>排序状态</th>
                                <td>
                                    <asp:TextBox ID="txtTypeActive" runat="server" Text="1" MaxLength="4" ToolTip="排序状态"></asp:TextBox>
                                    <i>(倒序，大于0时才显示)</i>
                                </td>
                            </tr>
                            <tr>
                                <th>添加时间</th>
                                <td><asp:TextBox ID="txtTypeAddTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改时间</th>
                                <td><asp:TextBox ID="txtTypeUpTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加IP</th>
                                <td><asp:TextBox ID="txtTypeAddIp" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改IP</th>
                                <td><asp:TextBox ID="txtTypeUpIp" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加人</th>
                                <td><asp:TextBox ID="txtTypeAddUser" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改人</th>
                                <td><asp:TextBox ID="txtTypeUpUser" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnTypeEdit" runat="server" Text="新增" OnClick="btnTypeEdit_Click" />
                        <asp:Button ID="btnTypeCancel" runat="server" Text="取消" Visible="false" OnClick="btnTypeCancel_Click" />
                    </div>
                </div>
                <div class="list hover">
                    <strong>选项分类
                        <span>符合条件的数据有：<b><asp:Literal ID="ltTypeTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>分类名称</th>
                                <th class="state">选项数</th>
                                <th>备注说明</th>
                                <th class="state">状态</th>
                                <th class="state">排序</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpTypeList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "TypeName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "OpNum")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Remark")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Active")%></td>
                                        <td align="center">
                                            <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>选取</u></a>
                                            <a href="?tid=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>子选项</u></a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltTypeNo" runat="server">
                                <tr><td class="no">暂时没有查询到信息！</td></tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblTypeNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plOp" runat="server" Visible="false">
<script>
    $(function () {
        if ($('#btnOpEdit').val() == '修改') {
            $('#nav2>a.hide').show().addClass('cur');
        } else {
            $('#nav2>a[href*="tid="]').addClass('cur').removeAttr('href');
        }
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtOpType') || checkEmpty('#txtOpName')) {
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
                <div id="nav2" class="btn">
                    <a href="?id=<%=Request.QueryString["tid"] %>">返回分类</a>
                    <a href="?tid=<%=Request.QueryString["tid"] %>">新增选项</a>
                    <a class="hide">修改选项</a>
                </div>
                <div class="frm edit">
                    <strong><asp:Literal ID="ltOpTitle" runat="server" Text="新增选项名称"></asp:Literal></strong>
                    <table>
                        <tbody>
                            <tr>
                                <th><b>*</b>选项分类</th>
                                <td><asp:TextBox ID="txtOpType" runat="server" ToolTip="分类名称" MaxLength="20" CssClass="readonly" ReadOnly="true"></asp:TextBox></td>
                                <th>自编号</th>
                                <td><asp:TextBox ID="txtOpId" runat="server" Text="0" CssClass="readonly" ReadOnly="true"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>选项名称</th>
                                <td><asp:TextBox ID="txtOpName" runat="server" CssClass="long" ToolTip="选项名称" MaxLength="50"></asp:TextBox></td>
                                <th>选项值</th>
                                <td><asp:TextBox ID="txtOpValue" runat="server" CssClass="long" ToolTip="选项值" MaxLength="50"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>默认选取</th>
                                <td>
                                    <asp:RadioButtonList ID="rblSelected" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="是" Value="True"></asp:ListItem>
                                        <asp:ListItem Text="否" Value="False" Selected="True"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                                <th rowspan="3">备注</th>
                                <td rowspan="3"><asp:TextBox ID="txtOpRemark" runat="server" TextMode="MultiLine" Rows="5" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>排序状态</th>
                                <td>
                                    <asp:TextBox ID="txtOpActive" runat="server" Text="1" MaxLength="4" ToolTip="排序状态"></asp:TextBox>
                                    <i>(倒序，大于0时才显示)</i>
                                </td>
                            </tr>
                            <tr>
                                <th>选项值2</th>
                                <td><asp:TextBox ID="txtOpValue2" runat="server" CssClass="long" ToolTip="选项值" MaxLength="50"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加时间</th>
                                <td><asp:TextBox ID="txtOpAddTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改时间</th>
                                <td><asp:TextBox ID="txtOpUpTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加IP</th>
                                <td><asp:TextBox ID="txtOpAddIp" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改IP</th>
                                <td><asp:TextBox ID="txtOpUpIp" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加人</th>
                                <td><asp:TextBox ID="txtOpAddUser" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改人</th>
                                <td><asp:TextBox ID="txtOpUpUser" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnOpEdit" runat="server" Text="新增" OnClick="btnOpEdit_Click" />
                        <asp:Button ID="btnOpCancel" runat="server" Text="取消" Visible="false" OnClick="btnOpCancel_Click" />
                    </div>
                </div>
                <div class="list hover">
                    <strong>选项名称
                        <span>符合条件的数据有：<b><asp:Literal ID="ltOpTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>选项分类</th>
                                <th>选项名称</th>
                                <th>选项值</th>
                                <th class="state">默认选取</th>
                                <th>备注说明</th>
                                <th class="state">状态</th>
                                <th class="state">排序</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpOpList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OpType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OpName")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OpValue")%><br /><%#DataBinder.Eval(Container.DataItem, "OpValue2")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SelectedName")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Remark")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Active")%></td>
                                        <td align="center">
                                            <a href="?tid=<%=Request.QueryString["tid"]%>&id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>选取</u></a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltOpNo" runat="server">
                                <tr><td class="no">暂时没有查询到信息！</td></tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblOpNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
