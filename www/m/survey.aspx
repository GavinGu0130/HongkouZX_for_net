<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="survey.aspx.cs" Inherits="hkzx.web.m.survey" %>
<%@ Register src="../cn/ucMeta.ascx" tagname="ucMeta" tagprefix="uc1" %>
<%@ Register src="ucHeader.ascx" tagname="ucHeader" tagprefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" Client="wap" />
    <title>虹口政协履职通 - 问卷调查</title>
</head>
<body>
<form id="form1" runat="server">
    <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    <uc1:ucHeader ID="header1" runat="server" Title="问卷调查" />
    <div class="content main">
        <asp:PlaceHolder ID="plList" runat="server" Visible="false">
            <div class="list table">
                <dl>
                    <dt class="total">符合条件的数据有：<b><asp:Literal ID="ltTotal" runat="server" Text="0"></asp:Literal></b>条</dt>
                    <asp:Repeater ID="rpList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                <p>
                                    <b>[<%#DataBinder.Eval(Container.DataItem, "SubType")%>]</b> <%#DataBinder.Eval(Container.DataItem, "Title")%><br />
                                    <b>截止时间：</b><%#DataBinder.Eval(Container.DataItem, "EndTimeText")%><br />
                                    <b>可参与：</b><%#DataBinder.Eval(Container.DataItem, "SurveyNumText")%><br />
                                    <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>选取</u></a>
                                </p>
                                <%--<p><%#DataBinder.Eval(Container.DataItem, "Body")%></p>--%>
                            </a>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltNo" runat="server">
                        <dd class="no">暂无问卷调查！</dd>
                    </asp:Literal>
                </dl>
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
            <div class="edit edit2">
                <dl>
                    <asp:HiddenField ID="hfMaxNum" runat="server" Value="-1" /><asp:HiddenField ID="hfMinNum" runat="server" Value="-1" />
                    <dt>《<asp:Literal ID="ltTitle" runat="server"></asp:Literal>》</dt>
                    <asp:Repeater ID="rpOpList" runat="server">
                        <ItemTemplate>
                            <dd>
                                <%#DataBinder.Eval(Container.DataItem, "num")%>、<b><%#DataBinder.Eval(Container.DataItem, "Title")%></b>
                                <p class="sub">
                                    <%#DataBinder.Eval(Container.DataItem, "Body")%>
                                    <asp:TextBox ID="_txt" runat="server" MaxLength="20"></asp:TextBox>
                                    <asp:HiddenField ID="_id" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "Id") %>'/>
                                    <asp:HiddenField ID="_title" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "Title") %>'/>
                                </p>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltOpNo" runat="server"><dd class="no">暂无选项，请稍后重试！</dd></asp:Literal>
                </dl>
                <asp:Panel ID="plSub" runat="server" Visible="false" CssClass="cmd">
                    <asp:Button ID="btnSub" runat="server" Text="提交" OnClick="btnSub_Click" />
                    <input type="reset" value="重填" />
                </asp:Panel>
            </div>
        </asp:PlaceHolder>
    </div>
</form>
</body>
</html>
