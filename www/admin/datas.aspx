<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="datas.aspx.cs" Inherits="hkzx.web.admin.datas" %><%--Tony维护--%>
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
                    <a href="?id=">新增资料文档类型</a>
                    <a class="hide">修改资料文档类型</a>
                </div>
                <div class="frm edit">
                    <strong><asp:Literal ID="ltTypeTitle" runat="server" Text="新增资料文档类型"></asp:Literal></strong>
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
                                    <i>(倒序)</i>
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
                <div class="list">
                    <strong>资料文档类型
                        <span>符合条件的数据有：<b><asp:Literal ID="ltTypeTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>类型名称</th>
                                <th class="state">文档数</th>
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
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "DatasNum")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Remark")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Active")%></td>
                                        <td align="center">
                                            <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>选取</u></a>
                                            <a href="?tid=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>资料文档</u></a>
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

            <asp:PlaceHolder ID="plDatas" runat="server" Visible="false">
<script>
    $(function () {
        upFile('#txtFiles', '', '#btnFiles');
        if ($('#btnOpEdit').val() == '修改') {
            $('#nav2>a.hide').show().addClass('cur');
        } else {
            $('#nav2>a[href*="tid="]').addClass('cur').removeAttr('href');
        }
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtType') || checkEmpty('#txtTitle')) {
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
                    <a href="?id=">返回资料文档类型</a>
                    <a href="?tid=<%=Request.QueryString["tid"] %>">新增资料文档</a>
                    <a class="hide">修改资料文档</a>
                </div>
                <div class="frm edit">
                    <strong><asp:Literal ID="ltDatasTitle" runat="server" Text="新增资料文档"></asp:Literal></strong>
                    <table>
                        <tbody>
                            <tr>
                                <th><b>*</b>类型</th><asp:HiddenField ID="hfTypeId" runat="server" Value="0" />
                                <td><asp:TextBox ID="txtType" runat="server" ToolTip="类型名称" MaxLength="20" CssClass="readonly" ReadOnly="true"></asp:TextBox></td>
                                <th>自编号</th>
                                <td><asp:TextBox ID="txtId" runat="server" Text="0" CssClass="readonly" ReadOnly="true"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>标题</th>
                                <td><asp:TextBox ID="txtTitle" runat="server" ToolTip="资料文档标题" MaxLength="50" CssClass="long"></asp:TextBox></td>
                                <th><b>*</b>排序状态</th>
                                <td>
                                    <asp:TextBox ID="txtActive" runat="server" Text="1" MaxLength="4" ToolTip="排序状态"></asp:TextBox>
                                    <i>(倒序)</i>
                                </td>
                            </tr>
                            <tr>
                                <th>内容</th>
                                <td colspan="3"><asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" CssClass="long" Rows="8"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>附件</th>
                                <td colspan="3">
                                    <asp:TextBox ID="txtFiles" runat="server" CssClass="readonly double"></asp:TextBox>
                                    <a id="btnFiles" href="#" class="btn"><u>上传</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th>备注</th>
                                <td colspan="3"><asp:TextBox ID="txtRemark" runat="server" TextMode="MultiLine" Rows="5" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
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
                        <asp:Button ID="btnDatasEdit" runat="server" Text="新增" OnClick="btnDatasEdit_Click" />
                        <asp:Button ID="btnDatasCancel" runat="server" Text="取消" Visible="false" OnClick="btnDatasCancel_Click" />
                    </div>
                </div>
                <div class="list">
                    <strong>资料文档列表
                        <span>符合条件的数据有：<b><asp:Literal ID="ltDatasTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>类型</th>
                                <th>标题</th>
                                <th>内容</th>
                                <th class="state">附件</th>
                                <th class="state">浏览数</th>
                                <th class="state">状态</th>
                                <th class="state">排序</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpDatasList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "TypeName")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Body")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Files")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ReadNum")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Active")%></td>
                                        <td align="center">
                                            <a href="?tid=<%=Request.QueryString["tid"]%>&id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>选取</u></a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltDatasNo" runat="server">
                                <tr><td class="no">暂时没有查询到信息！</td></tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblDatasNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
