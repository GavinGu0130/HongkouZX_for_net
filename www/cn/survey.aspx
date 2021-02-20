<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="survey.aspx.cs" Inherits="hkzx.web.cn.survey" %><%--Tony维护--%>
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
    <asp:HiddenField ID="hfBack" runat="server" />
    <uc1:ucHeader ID="header1" runat="server" />
    <div class="content">
        <div class="main">
            <asp:PlaceHolder ID="plList" runat="server" Visible="false">
                <div class="frm list">
                    <strong>问卷调查
                        <span>符合条件的数据有：<b><asp:Literal ID="ltTotal" runat="server" Text="0"></asp:Literal></b>条</span></strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="num">类型</th>
                                <th>标题</th>
                                <th class="time">截止时间</th>
                                <th class="state">可参与</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "EndTimeText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SurveyNumText")%></td>
                                        <td><a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>选取</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltNo" runat="server">
                                <tr>
                                    <td class="no">暂无问卷调查！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plEdit" runat="server" Visible="false">
<script>
    $(function () {
        $('.sub').each(function () {
            var $txt = $(this).find('input:text');
            $txt.hide();
            var $rd = $(this).find('input:radio');
            var $ck = $(this).find('input:checkbox');
            if ($rd.length > 0) {
                $rd.click(function () {
                    $txt.val($rd.filter(':checked').val());
                });
            } else if ($ck.length > 0) {
                $ck.click(function () {
                    var str = '';
                    $ck.each(function () {
                        var tmp = $(this).filter(':checked').val();
                        if (tmp) {
                            if (str != '') {
                                str += '|';
                            }
                            str += tmp;
                        }
                    });
                    $txt.val(str);
                });
            } else {
                $txt.show();
            }
        });
        var max = parseInt($('#hfMaxNum').val(), 10);
        var min = parseInt($('#hfMinNum').val(), 10);
        if (max >= 0 && min >= 0) {//投票
            $('.sub>input:checkbox').click(function () {
                $txt = $(this).parent().find('input:text');
                var n = $('.sub>input:checkbox:checked').length;
                if (max > 0 && n > max) {
                    $txt.val('');
                    alert('最多只能选取' + max + '个选项！');
                    return false;
                } else {
                    $txt.val($(this).val());
                }
            });
        }
        $('form').submit(function () {
            try {
                if (max >= 0 && min >= 0) {//投票
                    var n = $('.sub>input:checkbox:checked').length;
                    if (max > 0 && n > max) {
                        alert('最多只能选取' + max + '个选项！');
                        return false;
                    }
                    if (min > 0 && n < min) {
                        alert('至少要选取' + min + '个选项！');
                        return false;
                    }
                } else {//问卷、答题
                    var pass = true;
                    $('.sub>input:text').each(function () {
                        if ($(this).val() == '') {
                            pass = false;
                            alert('还有[选项]未完成！');
                            $(this).parent().find('input:first').focus();
                            return false;
                        }
                    });
                    if (!pass) {
                        return false;
                    }
                }
                return true;
            } catch (err) {
                //alert("验证出错，请稍后重试！");
                return false;
            }
        });
    });
</script>
                <div class="frm list">
                    <asp:HiddenField ID="hfMaxNum" runat="server" Value="-1" /><asp:HiddenField ID="hfMinNum" runat="server" Value="-1" />
                    <strong>《<asp:Literal ID="ltTitle" runat="server"></asp:Literal>》</strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th>调查内容</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpOpList" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td class="sub">
                                            <b><%#DataBinder.Eval(Container.DataItem, "Title")%></b>
                                            <%#DataBinder.Eval(Container.DataItem, "Body")%>
                                            <asp:TextBox ID="_txt" runat="server" MaxLength="20"></asp:TextBox>
                                            <asp:HiddenField ID="_id" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "Id") %>'/>
                                            <asp:HiddenField ID="_title" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "Title") %>'/>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltOpNo" runat="server">
                                <tr>
                                    <td class="no">暂无选项，请稍后重试！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Panel ID="plSub" runat="server" Visible="false" CssClass="cmd">
                        <asp:Button ID="btnSub" runat="server" Text="提交" OnClick="btnSub_Click" />
                        <input type="reset" value="重填" />
                    </asp:Panel>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
