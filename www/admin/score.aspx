<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="score.aspx.cs" Inherits="hkzx.web.admin.score" %><%--Tony维护--%>
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
            <asp:PlaceHolder ID="plScore" runat="server" Visible="false">
<script>
    $(function () {
        if ($('#btnEdit').val() == '修改') {
            $('#nav>a.hide').show().addClass('cur');
        } else {
            $('#nav>a:first').addClass('cur').removeAttr('href');
        }
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtTitle') || checkEmpty('#txtActive') || checkSelect('#ddlScoreType')) {
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
                <div id="nav" class="btn">
                    <a href="?id=">新增积分项</a>
                    <a class="hide">修改积分项</a>
                </div>

                <div class="frm edit">
                    <strong><asp:Literal ID="ltTitle" runat="server" Text="新增积分项"></asp:Literal></strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>自编号</th>
                                <td><asp:TextBox ID="txtId" runat="server" Text="0" CssClass="readonly" ReadOnly="true"></asp:TextBox></td>
                                <th><b>*</b>排序状态</th>
                                <td>
                                    <asp:TextBox ID="txtActive" runat="server" Text="1" MaxLength="4" ToolTip="排序状态"></asp:TextBox>
                                    <i>(倒序，大于0时才显示)</i>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>名称</th>
                                <td><asp:TextBox ID="txtTitle" runat="server" MaxLength="50" CssClass="long" ToolTip="名称"></asp:TextBox></td>
                                <th><b>*</b>类别</th>
                                <td>
                                    <asp:DropDownList ID="ddlScoreType" runat="server" ToolTip="类别">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>分值</th>
                                <td><asp:TextBox ID="txtScore" runat="server" MaxLength="6" Text="0"></asp:TextBox></td>
                                <th rowspan="3">备注</th>
                                <td rowspan="3"><asp:TextBox ID="txtRemark" runat="server" TextMode="MultiLine" Rows="5" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>最高分值</th>
                                <td><asp:TextBox ID="txtScore2" runat="server" MaxLength="4" Text="0"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>计量单位</th>
                                <td><asp:TextBox ID="txtUnit" runat="server" MaxLength="2"></asp:TextBox></td>
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
                    <div class="cmd">
                        <asp:Button ID="btnEdit" runat="server" Text="新增" OnClick="btnEdit_Click" />
                        <asp:Button ID="btnCancel" runat="server" Text="取消" Visible="false" OnClick="btnCancel_Click" />
                    </div>
                </div>

                <div class="list hover">
                    <strong>积分项
                        <span>符合条件的数据有：<b><asp:Literal ID="ltTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>类别</th>
                                <th>名称</th>
                                <th class="state">分值</th>
                                <th class="num">计量</th>
                                <th>备注说明</th>
                                <th class="state">状态</th>
                                <th class="state">排序</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "ScoreType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ScoreText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Unit")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Remark")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Active")%></td>
                                        <td align="center">
                                            <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>选取</u></a>
                                            <%--<a href="?tid=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>子选项</u></a>--%>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltNo" runat="server">
                                <tr><td class="no">暂时没有查询到积分项！</td></tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
