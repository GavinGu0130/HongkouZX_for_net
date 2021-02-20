<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="output.aspx.cs" Inherits="hkzx.web.admin.output" %><%--Tony维护--%>
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
                if (url.indexOf('ac=in') > 0) {
                    $('#plNav>a[href*="ac=in"]').addClass('cur').removeAttr('href');
                } else if (url.indexOf('ac=first') > 0) {
                    $('#plNav>a[href*="ac=first"]').addClass('cur').removeAttr('href');
                } else {
                    $('#plNav>a:first').addClass('cur').removeAttr('href');
                }
            }
            if ($('.list>table>tbody>tr>td.no').text()) {
                $('.list>table>tbody>tr>td.no').attr('colspan', $('.list>table>thead>tr>th').length);
                $('.list>table>thead').hide();
            } else {
                $('.list>table>thead>tr>th>input:checkbox').click(function () {
                    $('.list>table>tbody>tr>td>input:checkbox').prop('checked', $(this).prop('checked'));
                });
            }
        });
    </script>
</head>
<body>
<form id="form1" runat="server">
    <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    <asp:HiddenField ID="hfBack" runat="server" Value="./" />
    <asp:HiddenField ID="hfUser" runat="server" />
    <uc1:ucHeader ID="header1" runat="server" />
    <div class="content">
        <div class="main">
            <asp:Panel ID="plNav" runat="server" Visible="false" CssClass="btn">
                <a href="?ac=out">导出提案数据及委员反馈</a>
                <a href="?ac=in">导入提案办理情况</a>
                <asp:HyperLink ID="lnkFirst" runat="server" NavigateUrl="?ac=first">本地数据库导入一次提案数据</asp:HyperLink>
            </asp:Panel>

            <asp:PlaceHolder ID="plFirst" runat="server" Visible="false">
                <div class="frm edit">
                    <table>
                        <tbody>
                            <tr>
                                <th>数据库</th>
                                <td><asp:TextBox ID="txtData" runat="server">zx20200724</asp:TextBox></td>
                                <th>用户名</th>
                                <td><asp:TextBox ID="txtUserId" runat="server"></asp:TextBox></td>
                                <th>密码</th>
                                <td><asp:TextBox ID="txtPwd" runat="server" TextMode="Password"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnFirst" runat="server" Text="导入" OnClick="btnFirst_Click" />
                    </div>
                </div>
                <div class="info"><asp:Literal ID="ltFirst" runat="server"></asp:Literal></div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plOut" runat="server" Visible="false">
<script>
    $(function () {
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtOutDate1') || checkEmpty('#txtOutDate2')) {
                    return false;
                }
            } catch (err) {
                alert("验证出错，请稍后重试！");
                return false;
            }
        });
    });
</script>
                <div class="frm edit">
                    <table>
                        <tbody>
                            <tr>
                                <th>导出起止时间</th>
                                <td><asp:TextBox ID="txtOutDate1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" ToolTip="开始时间"></asp:TextBox> - <asp:TextBox ID="txtOutDate2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm',minDate:'#F{$dp.$D(\'txtOutDate1\')}'})" ToolTip="结束时间"></asp:TextBox></td>
                                <th>排除提案Id</th>
                                <td><asp:TextBox ID="txtOutNot" runat="server" CssClass="long"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnOut" runat="server" Text="导出" OnClick="btnOut_Click" /><asp:Button ID="btnOut2" runat="server" Text="导出2中文标识" OnClick="btnOut2_Click" />
                    </div>
                </div>
                <div class="info"><asp:Literal ID="ltOut" runat="server"></asp:Literal></div><%--<b>导出提案成功！</b><br />附件文件目录：" + strPath + "，请从服务器上下载。<br /><a href='{0}' download='{0}' target='_blank'>xml数据文件链接</a>--%>
            </asp:PlaceHolder>
            <asp:Panel ID="plFiles" runat="server" Visible="false" CssClass="list">
                <strong>请点击链接下载以下附件：</strong>
                <table>
                    <tbody>
                        <asp:Repeater ID="rpFiles" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                    <td><a href="<%#DataBinder.Eval(Container.DataItem, "Url")%>" download="<%#DataBinder.Eval(Container.DataItem, "Url")%>" target="_blank"><%#DataBinder.Eval(Container.DataItem, "Title")%></a></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </asp:Panel>

            <asp:PlaceHolder ID="plIn" runat="server" Visible="false">
<script>
    $(function () {
        upFile('#txtInFile', '', '#btnInFile', 'xml');
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtInFile')) {
                    return false;
                }
            } catch (err) {
                alert("验证出错，请稍后重试！");
                return false;
            }
        });
    });
</script>
                <div class="frm edit">
                    <table>
                        <tbody>
                            <tr>
                                <th>导入xml</th>
                                <td>
                                    <asp:TextBox ID="txtInFile" runat="server" CssClass="double" ToolTip="xml文件名"></asp:TextBox>
                                    <a id="btnInFile" href="#" class="btn"><u>上传文件</u></a>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnIn" runat="server" Text="导入" OnClick="btnIn_Click" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:Label ID="lblInfo" runat="server"></asp:Label>

        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
