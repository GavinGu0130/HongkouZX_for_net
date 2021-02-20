<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="datas.aspx.cs" Inherits="hkzx.web.cn.datas" %><%--Tony维护--%>
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
            var url = window.location.href;
            if (url.indexOf('tid=') < 0) {
                $('#nav>a:first').addClass('cur');
                url += $('#nav>a:first').attr('href');
            } else {
                var tid = url.substring(url.indexOf('tid='));
                if (tid.indexOf('&') > 0) {
                    tid = tid.substring(0, tid.indexOf('&'));
                }
                $('#nav>a[href*="' + tid + '"]').addClass('cur');
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
    <uc1:ucHeader ID="header1" runat="server" />
    <div class="content">
        <div class="main">
            <div id="nav" class="btn">
                <asp:Repeater ID="rpType" runat="server">
                    <ItemTemplate>
                        <a href="?tid=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><%#DataBinder.Eval(Container.DataItem, "TypeName")%></a>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <asp:PlaceHolder ID="plList" runat="server" Visible="false">
                <div class="frm list hover">
                    <strong><asp:HyperLink ID="lnkMore" runat="server" NavigateUrl="?ac=query&TypeId=">查询&gt;&gt;</asp:HyperLink></strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>标题</th>
                                <th class="date">提交日期</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpDatasList" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%><br /><%#DataBinder.Eval(Container.DataItem, "Files")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "UpTime", "{0:yyyy-MM-dd}")%></td>
                                        <td align="center"><a href="?tid=<%#DataBinder.Eval(Container.DataItem, "TypeId")%>&id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltDatasNo" runat="server">
                                <tr><td class="no">暂时没有查询到资料文档！</td></tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblDatasNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:Repeater ID="rpView" runat="server">
                <ItemTemplate>
                    <div class="frm edit">
                        <strong>[资料文档] <%#DataBinder.Eval(Container.DataItem, "TypeName")%></strong>
                        <table>
                            <tbody>
                                <tr>
                                    <th>标题</th>
                                    <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                </tr>
                                <tr>
                                    <th>内容</th>
                                    <td>
                                        <%#DataBinder.Eval(Container.DataItem, "Files")%>
                                        <p><%#DataBinder.Eval(Container.DataItem, "Body")%></p>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <div class="cmd">
                            <input type="button" value="返回" onclick="window.history.back(-1);" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:PlaceHolder ID="plQuery" runat="server" Visible="false">
<script>
    $(function () {
        $('form').submit(function () {
            try {
                var url = '';
                if ($('#ddlQTypeId').val()) {
                    url += '&TypeId=' + $('#ddlQTypeId').val();
                }
                if ($('#txtQTitle').val()) {
                    url += '&Title=' + $('#txtQTitle').val();
                }
                if ($('#txtQBody').val()) {
                    url += '&Body=' + $('#txtQBody').val();
                }
                //alert(encodeURI(url)); return false;
                if (url != '') {
                    window.location.href = '?ac=query' + encodeURI(url);
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
                    <strong>查询条件</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>标题</th>
                                <td><asp:TextBox ID="txtQTitle" runat="server" MaxLength="50" CssClass="long"></asp:TextBox></td>
                                <th>类型</th>
                                <td>
                                    <asp:DropDownList ID="ddlQTypeId" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>正文内容</th>
                                <td colspan="3"><asp:TextBox ID="txtQBody" runat="server" MaxLength="200" CssClass="long"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <input id="btnQuery" type="submit" value="查询" />
                        <input id="clean" type="reset" value="清空" />
                    </div>
                </div>
                <div class="list">
                    <strong>结果展现
                        <span>符合条件的数据有：<b><asp:Literal ID="ltQueryTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>标题</th>
                                <th class="date">提交日期</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpQueryList" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "UpTime", "{0:yyyy-MM-dd}")%></td>
                                        <td><a href="?tid=<%#DataBinder.Eval(Container.DataItem, "TypeId")%>&id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltQueryNo" runat="server">
                                <tr><td class="no">暂时没有查询到信息！</td></tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblQueryNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
