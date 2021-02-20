<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="news.aspx.cs" Inherits="hkzx.web.admin.news" %><%--Tony维护--%>
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
            var url = window.location.href;
            if (url.indexOf('id=') < 0) {
                $('#nav>a:first').addClass('cur').removeAttr('href');
            } else if (url.indexOf('id=0') > 0) {
                $('#nav>a[href*="id=0"]').addClass('cur').removeAttr('href');
            } else if (url.indexOf('id=') > 0) {
                $('#nav>a.hide').show().addClass('cur');
            }
            if ($('.list>table>tbody>tr>td.no').text()) {
                $('.list>table>tbody>tr>td.no').attr('colspan', $('.list>table>thead>tr>th').size());
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
            <div id="nav" class="btn">
                <a href="?ac=">查询动态</a>
                <a href="?id=0">发布动态</a>
                <a class="hide">更新动态</a>
            </div>
            <asp:PlaceHolder ID="plList" runat="server" Visible="false">
<script>
    $(function () {
        $('#btnQuery').click(function () {
            try {
                var url = '';
                if ($('#txtQTitle').val()) {
                    url += '&Title=' + $('#txtQTitle').val();
                }
                var tmp = getChecked('#cblQSubType', ',');
                if (tmp != '') {
                    url += '&SubType=' + tmp;
                }
                if ($('#txtQShowTime1').val() || $('#txtQShowTime2').val()) {
                    url += '&ShowTime=' + $('#txtQShowTime1').val() + ',' + $('#txtQShowTime2').val();
                }
                //tmp = getChecked('#cblQActive', ',');
                //if (tmp != '') {
                //    url += '&Active=' + tmp;
                //}
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
    });
</script>
                <div class="frm edit">
                    <strong>查询动态</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>标题</th>
                                <td><asp:TextBox ID="txtQTitle" runat="server" MaxLength="50" CssClass="long"></asp:TextBox></td>
                                <th>类型</th>
                                <td>
                                    <asp:CheckBoxList ID="cblQSubType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="政协要闻"></asp:ListItem>
                                        <asp:ListItem Text="视频新闻"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th>发布时间大于</th>
                                <td><asp:TextBox ID="txtQShowTime1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox></td>
                                <th>发布时间小于</th>
                                <td><asp:TextBox ID="txtQShowTime2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <input id="btnQuery" type="button" value="查询" />
                        <input id="clean" type="reset" value="清空" />
                    </div>
                </div>
                <div class="list hover">
                    <strong>结果展现
                        <span>符合条件的数据有：<b><asp:Literal ID="ltNewsTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="pic">图片</th>
                                <th class="state">类型</th>
                                <th>标题</th>
                                <th class="time">发布时间</th>
                                <th class="state">浏览数</th>
                                <th class="state">状态</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpNewsList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "PicUrl")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><b><%#DataBinder.Eval(Container.DataItem, "Title")%></b>
                                            <p><%#DataBinder.Eval(Container.DataItem, "Body")%></p>
                                        </td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ShowTime", "{0:yyyy-MM-dd HH:mm:ss}")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ReadNum")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center">
                                            <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><u>选取</u></a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltNewsNo" runat="server">
                                <tr>
                                    <td class="no">暂无新闻动态！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblNewsNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plEdit" runat="server" Visible="false">
<script>
    $(function () {
        upFile('#txtPicUrl', '#showPic', '#btnPic', 'img');
        upFile('#txtVideo', '#showVideo', '#btnVideo', 'video');
        $('form').submit(function () {
            try {
                if (checkRadio('#rblActive') || checkRadio('#rblSubType') || checkEmpty('#txtTitle') || ($('#rblSubType>input:radio:checked').val() == '视频新闻' && checkEmpty('#txtVideo'))) {
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
                    <strong><asp:Literal ID="ltEditTitle" runat="server" Text="发布动态"></asp:Literal></strong>
                    <table>
                        <tbody>
                            <tr>
                                <th><b>*</b>状态</th>
                                <td>
                                    <asp:RadioButtonList ID="rblActive" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="状态">
                                        <asp:ListItem Text="正常" Value="1" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="取消" Value="-1"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                                <th>自编号</th>
                                <td><asp:TextBox ID="txtId" runat="server" ReadOnly="true" CssClass="readonly" Text="0"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>类型</th>
                                <td>
                                    <asp:RadioButtonList ID="rblSubType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="新闻类型"></asp:RadioButtonList>
                                </td>
                                <th></th>
                                <td></td>
                            </tr>
                            <tr>
                                <th><b>*</b>标题</th>
                                <td><asp:TextBox ID="txtTitle" runat="server" MaxLength="50" CssClass="long" ToolTip="标题"></asp:TextBox></td>
                                <th>链接地址</th>
                                <td><asp:TextBox ID="txtLnkUrl" runat="server" MaxLength="200" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th rowspan="2">图片地址</th>
                                <td><asp:TextBox ID="txtPicUrl" runat="server" MaxLength="100" CssClass="long" ToolTip="图片地址"></asp:TextBox>
                                    <a id="btnPic" href="#"><u>上传</u></a>
                                </td>
                                <th rowspan="2">视频地址</th>
                                <td><asp:TextBox ID="txtVideo" runat="server" MaxLength="100" CssClass="long" ToolTip="视频地址"></asp:TextBox>
                                    <a id="btnVideo" href="#"><u>上传</u></a>
                                </td>
                            </tr>
                            <tr>
                                <td id="showPic" class="pic"></td>
                                <td id="showVideo" class="pic"></td>
                            </tr>
                            <tr>
                                <th>内容提要</th>
                                <td><asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" Rows="5" CssClass="long"></asp:TextBox></td>
                                <th>备注</th>
                                <td><asp:TextBox ID="txtRemark" runat="server" TextMode="MultiLine" Rows="5" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>发布时间</th>
                                <td><asp:TextBox ID="txtShowTime" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})" ToolTip="显示时间"></asp:TextBox></td>
                                <th>浏览数</th>
                                <td><asp:TextBox ID="txtReadNum" runat="server" ReadOnly="true" CssClass="readonly" Text="0"></asp:TextBox></td>
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
                        <asp:Button ID="btnEdit" runat="server" Text="发布" OnClick="btnEdit_Click" />
                        <input type="reset" value="重填" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
