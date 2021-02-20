<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="notice.aspx.cs" Inherits="hkzx.web.admin.notice" %><%--Tony维护--%>
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
                $('#plNav>a:first').addClass('cur').removeAttr('href');
            } else if ($('#btnEdit').val() == '发布') {
                $('#plNav>a[href*="id=0"]').addClass('cur').removeAttr('href');
            } else if (url.indexOf('nid=') > 0) {
                $('#plNav>a#feed').show().addClass('cur');
            } else if (url.indexOf('id=') > 0) {
                $('#plNav>a#update').show().addClass('cur');
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
    <asp:HiddenField ID="hfBack" runat="server" Value="./" />
    <uc1:ucHeader ID="header1" runat="server" />
    <div class="content">
        <div class="main">
            <asp:Panel ID="plNav" runat="server" Visible="false" CssClass="btn">
                <a href="?ac=">检索信息</a>
                <a href="?id=0">发布信息</a>
                <a id="update" class="hide">更新信息</a>
                <a id="feed" class="hide">反馈情况</a>
            </asp:Panel>

            <asp:PlaceHolder ID="plList" runat="server" Visible="false">
<script>
    $(function () {
        $('#btnQToMans').click(function () {
            var title = '选取需要通知的委员';
            var obj = 'txtQToMans';
            showDialog(title, '../cn/dialog.aspx?ac=subman&obj=' + obj, '', 640, 400, 'no');
            return false;
        });
        $('#btnQuery').click(function () {
            try {
                var url = '';
                var tmp = getChecked('#cblQActive', ',');
                if (tmp != '') {
                    url += '&Active=' + tmp;
                }
                tmp = getChecked('#cblQSubType', ',');
                if (tmp != '') {
                    url += '&SubType=' + tmp;
                }
                if ($('#txtQToMans').val()) {
                    var tmp2 = $('#txtQToMans').val().replace(/，/g, ',');
                    $('#txtQToMans').val(tmp2);
                    url += '&ToMans=' + $('#txtQToMans').val();
                }
                if ($('#txtQShowTime1').val() || $('#txtQShowTime2').val()) {
                    url += '&ShowTime=' + $('#txtQShowTime1').val() + ',' + $('#txtQShowTime2').val();
                }
                if ($('#txtQOverTime1').val() || $('#txtQOverTime2').val()) {
                    url += '&OverTime=' + $('#txtQOverTime1').val() + ',' + $('#txtQOverTime2').val();
                }
                if ($('#txtQTitle').val()) {
                    url += '&Title=' + $('#txtQTitle').val();
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
        $('#btnImportant, #btnPass, #btnDels').click(function () {
            if (!$('.list>table>tbody>tr>td>input:checkbox').is(':checked')) {
                alert('请先选取要操作的数据！');
                return false;
            }
            if ($(this).attr('id') == 'btnDels') {
                var txt = prompt('请填写删除原因：');
                if (txt) {
                    $('#hfVerifyInfo').val(txt);
                } else {
                    return false;
                }
            } else if (!confirm('您确定要“' + $(this).val() + '”吗?')) {
                return false;
            }
        });
    });
</script>
                <div class="frm edit">
                    <strong>检索信息</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>类型</th>
                                <td>
                                    <asp:CheckBoxList ID="cblQSubType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="通知"></asp:ListItem>
                                        <asp:ListItem Text="公告"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                                <th>状态</th>
                                <td>
                                    <asp:CheckBoxList ID="cblQActive" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="重要" Value=">=10"></asp:ListItem>
                                        <asp:ListItem Text="正常" Value=">0"></asp:ListItem>
                                        <asp:ListItem Text="暂存" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="删除" Value="<=-400"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th>标题</th>
                                <td><asp:TextBox ID="txtQTitle" runat="server" MaxLength="50" CssClass="long"></asp:TextBox></td>
                                <th>需要通知的委员</th>
                                <td><asp:TextBox ID="txtQToMans" runat="server" CssClass="readonly long"></asp:TextBox>
                                    <a id="btnQToMans" href="#" class="btn"><u>选取</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th>发布时间</th>
                                <td><asp:TextBox ID="txtQShowTime1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox> - <asp:TextBox ID="txtQShowTime2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtQShowTime1\')}'})"></asp:TextBox></td>
                                <th>失效时间</th>
                                <td><asp:TextBox ID="txtQOverTime1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox> - <asp:TextBox ID="txtQOverTime2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtQOverTime1\')}'})"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <input id="btnQuery" type="button" value="查询" />
                        <input id="clean" type="reset" value="清空" />
                    </div>
                </div>
                <div class="cmd">
                    <asp:Button ID="btnImportant" runat="server" Text="重要" OnClick="btnImportant_Click" />
                    <asp:Button ID="btnPass" runat="server" Text="正常" OnClick="btnPass_Click" />
                    <asp:Button ID="btnDels" runat="server" Text="删除" OnClick="btnDels_Click" /><asp:HiddenField ID="hfVerifyInfo" runat="server" />
                </div>
                <div class="list hover">
                    <strong>结果展现
                        <span>符合条件的数据有：<b><asp:Literal ID="ltQueryTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="state">类型</th>
                                <th>标题</th>
                                <th class="state">反馈数</th>
                                <th class="time">失效时间</th>
                                <th class="state">浏览数</th>
                                <th class="state">状态</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpQueryList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><b><%#DataBinder.Eval(Container.DataItem, "Title")%></b>
                                            <p><%#DataBinder.Eval(Container.DataItem, "Body")%></p><%#DataBinder.Eval(Container.DataItem, "Files")%>
                                        </td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "FeedNum")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "OverTime", "{0:yyyy-MM-dd HH:mm:ss}")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ReadNum")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td>
                                            <asp:CheckBox ID="_ck" runat="server" /><asp:HiddenField ID="_id" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "Id") %>'/>
                                            <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>修改</u></a>
                                            <a href="?nid=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看反馈</u></a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltQueryNo" runat="server">
                                <tr>
                                    <td class="no">暂无通知公告！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblQueryNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plEdit" runat="server" Visible="false">
<script>
    $(function () {
        loadEditor('#txtBody', '#editorBody', true);
        upFile('#txtFiles', '', '#btnFiles');
        $('#btnToMans').click(function () {
            var title = '选取需要通知的委员';
            var obj = 'txtToMans';
            showDialog(title, '../cn/dialog.aspx?ac=subman&obj=' + obj, '', 640, 400, 'no');
            return false;
        });
        $('form').submit(function () {
            try {
                if ($('#txtToMans').val()) {
                    var tmp2 = $('#txtToMans').val().replace(/，/g, ',');
                    $('#txtToMans').val(tmp2);
                }
                if (checkRadio('#rblSubType') || checkEmpty('#txtOverTime') || checkEmpty('#txtTitle') || ($('#rblSubType>input:radio:checked').val() == '通知' && checkEmpty('#txtToMans'))) {
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
                    <strong><asp:Literal ID="ltEditTitle" runat="server" Text="发布信息"></asp:Literal></strong>
                    <table>
                        <tbody>
                            <tr>
                                <th><b>*</b>类型</th>
                                <td>
                                    <asp:RadioButtonList ID="rblSubType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="类型"></asp:RadioButtonList>
                                    <asp:CheckBoxList ID="cblDegree" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="重要"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                                <th>自编号</th>
                                <td><asp:TextBox ID="txtId" runat="server" ReadOnly="true" CssClass="readonly" Text="0"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>标题</th>
                                <td><asp:TextBox ID="txtTitle" runat="server" MaxLength="50" CssClass="long" ToolTip="标题"></asp:TextBox></td>
                                <th><b>*</b>失效时间</th>
                                <td><asp:TextBox ID="txtOverTime" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" ToolTip="失效时间"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>内容</th>
                                <td colspan="3">
                                    <div id="editorBody" class="editor"><%--<p>欢迎使用 <b>wangEditor</b> 富文本编辑器</p>--%></div>
                                    <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" Rows="9" CssClass="long"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th>需要通知的委员</th>
                                <td colspan="3"><asp:TextBox ID="txtToMans" runat="server" TextMode="MultiLine" Rows="5" CssClass="readonly long" ToolTip="需要通知的委员"></asp:TextBox>
                                    <a id="btnToMans" href="#" class="btn"><u>选取</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>发布时间</th>
                                <td><asp:TextBox ID="txtShowTime" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss'})" ToolTip="发布时间"></asp:TextBox></td>
                                <th rowspan="2">备注</th>
                                <td rowspan="2"><asp:TextBox ID="txtRemark" runat="server" TextMode="MultiLine" Rows="3" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>浏览数</th>
                                <td><asp:TextBox ID="txtReadNum" runat="server" ReadOnly="true" CssClass="readonly" Text="0"></asp:TextBox></td>
                                <%--<th>附件</th>
                                <td><asp:TextBox ID="txtFiles" runat="server" MaxLength="100" CssClass="long" ToolTip="附件"></asp:TextBox>
                                    <a id="btnFiles" href="#"><u>上传</u></a>
                                </td>--%>
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
                        <asp:Button ID="btnSave" runat="server" Text="暂存" OnClick="btnSave_Click" />
                        <asp:Button ID="btnDel" runat="server" Text="删除" Visible="false" OnClick="btnDel_Click" />
                        <%--<input type="reset" value="重填" />--%>
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plFeed" runat="server" Visible="false">
<script>
    $(function () {
        $('#btnFromMan').click(function () {
            var title = '选取反馈委员';
            var obj = 'txtFromMan';
            showDialog(title, '../cn/dialog.aspx?ac=subman&obj=' + obj, '', 640, 400, 'no');
            return false;
        });
        $('#btnFeed').click(function () {
            try {
                var url = '';
                if ($('#txtFromMan').val()) {
                    url += '&FromMan=' + $('#txtFromMan').val();
                }
                if ($('#ddlFeedActive').val()) {
                    url += '&ActiveName=' + $('#ddlFeedActive').val();
                }
                //alert(encodeURI(url)); return false;
                window.location.href = '?nid=' + <%=Request.QueryString["nid"]%> + encodeURI(url);
                return false;
            } catch (err) {
                alert("验证出错，请稍后重试！");
                return false;
            }
        });
        $('.list>table>thead>tr>th>input:checkbox').click(function () {
            $('.list>table>tbody>tr>td>input:checkbox').prop('checked', $(this).prop('checked'));
        });
        $('#btnWxMsg').click(function () {
            if (!$('.list>table>tbody>tr>td>input:checkbox').is(':checked')) {
                alert('请先选取要操作的人员！');
                return false;
            }
            if (!confirm('您确定要“' + $(this).val() + '”吗?')) {
                return false;
            }
        });
        $('.list>table>tbody>tr>td>a').each(function () {
            var url = $(this).attr('href');
            if (url.indexOf('/cn/') < 0) {
                url = '../cn/' + url;
            }
            $(this).click(function () {
                showDialog('已发送消息', url, '', 800, 400, 'yes');
                return false;
            });
        });
    });
</script>
                <div class="frm edit">
                    <strong>反馈情况查询</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>标题</th>
                                <td colspan="3"><asp:TextBox ID="txtNoticeTitle" runat="server" MaxLength="50" ReadOnly="true" CssClass="readonly long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>反馈委员</th>
                                <td><asp:TextBox ID="txtFromMan" runat="server" CssClass="readonly long"></asp:TextBox>
                                    <a id="btnFromMan" href="#" class="btn"><u>选取</u></a>
                                </td>
                                <th>状态</th>
                                <td>
                                    <asp:DropDownList ID="ddlFeedActive" runat="server">
                                        <asp:ListItem Value="">全部</asp:ListItem>
                                        <asp:ListItem Value=">0">已阅</asp:ListItem>
                                        <asp:ListItem Value="0">未阅</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <input id="btnFeed" type="button" value="查询" />
                        <input type="reset" value="清空" />
                    </div>
                </div>
                <div class="cmd">
                    <asp:Button ID="btnWxMsg" runat="server" Text="发送微信消息" OnClick="btnWxMsg_Click" />
                </div>
                <div class="list hover">
                    <strong>《<asp:Literal ID="ltNoticeTitle" runat="server"></asp:Literal>》反馈情况
                        <span>符合条件的数据有：<b><asp:Literal ID="ltFeedTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="state">状态</th>
                                <th class="state">反馈委员</th>
                                <th class="time">反馈时间</th>
                                <th>备注</th>
                                <th class="state">已发消息</th>
                                <th class="state"><input type="checkbox" title="全选" />操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpFeedList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "FeedUser")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "FeedTime")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Remark")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SendMsg")%></td>
                                        <td align="center">
                                            <asp:CheckBox ID="_ck" runat="server" /><asp:HiddenField ID="_id" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "Id") %>'/>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltFeedNo" runat="server">
                                <tr>
                                    <td class="no">暂无反馈情况！</td>
                                </tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblFeedNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
