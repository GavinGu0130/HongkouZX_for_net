<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="survey.aspx.cs" Inherits="hkzx.web.admin.survey" %><%--Tony维护--%>
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
            } else if (url.indexOf('rid=') > 0) {
                $('#plNav>a#feed').show().addClass('cur');
                $('#plNav>a#update,#plNav>a#op').show();
            } else if (url.indexOf('oid=') > 0) {
                $('#plNav>a#op').show().addClass('cur');
                $('#plNav>a#update,#plNav>a#feed').show();
            } else if (url.indexOf('?id=0') > 0) {
                $('#plNav>a[href*="id=0"]').addClass('cur').removeAttr('href');
            } else if (url.indexOf('?id=') > 0) {
                $('#plNav>a#update').show().addClass('cur');
                if (url.indexOf('?id=0') < 0) {
                    $('#plNav>a#op,#plNav>a#feed').show();
                }
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
                <a href="?ac=">检索问卷调查</a>
                <a href="?id=0">新增问卷调查</a>
                <a id="update" href="?id=<%=Request.QueryString["id"] %>" class="hide">修改问卷调查</a>
                <a id="op" href="?id=<%=Request.QueryString["id"] %>&oid=0" class="hide">子选项</a>
                <a id="feed" href="?id=<%=Request.QueryString["id"] %>&rid=0" class="hide">反馈情况</a>
            </asp:Panel>

            <asp:PlaceHolder ID="plSurvey" runat="server" Visible="false">
<script>
    $(function () {
        $('#btnQToMans').click(function () {
            var title = '选取需要通知的委员';
            var obj = 'txtQToMans';
            showDialog(title, '../cn/dialog.aspx?ac=subman&obj=' + obj, '', 640, 400, 'no');
            return false;
        });
        $('form').submit(function () {
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
                if ($('#txtQTitle').val()) {
                    url += '&Title=' + $('#txtQTitle').val();
                }
                if ($('#txtQStartTime1').val() || $('#txtQStartTime2').val()) {
                    url += '&StartTime=' + $('#txtQStartTime1').val() + ',' + $('#txtQStartTime2').val();
                }
                if ($('#txtQEndTime1').val() || $('#txtQEndTime2').val()) {
                    url += '&EndTime=' + $('#txtQEndTime1').val() + ',' + $('#txtQEndTime2').val();
                }
                if ($('#txtQToMans').val()) {
                    var tmp2 = $('#txtQToMans').val().replace(/，/g, ',');
                    $('#txtQToMans').val(tmp2);
                    url += '&ToMans=' + $('#txtQToMans').val();
                }
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
                    <strong>检索问卷调查</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>类型</th>
                                <td>
                                    <asp:CheckBoxList ID="cblQSubType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="问卷"></asp:ListItem>
                                        <asp:ListItem Text="投票"></asp:ListItem>
                                        <asp:ListItem Text="答题"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                                <th>状态</th>
                                <td>
                                    <asp:CheckBoxList ID="cblQActive" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="正常" Value=">0"></asp:ListItem>
                                        <asp:ListItem Text="暂停" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="取消" Value="<0"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th>标题</th>
                                <td><asp:TextBox ID="txtQTitle" runat="server" MaxLength="50" CssClass="long"></asp:TextBox></td>
                                <th>需参与委员</th>
                                <td><asp:TextBox ID="txtQToMans" runat="server" CssClass="readonly long"></asp:TextBox>
                                    <a id="btnQToMans" href="#" class="btn"><u>选取</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th>开始时间</th>
                                <td><asp:TextBox ID="txtQStartTime1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox> - <asp:TextBox ID="txtQStartTime2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtQStartTime1\')}'})"></asp:TextBox></td>
                                <th>结束时间</th>
                                <td><asp:TextBox ID="txtQEndTime1" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox> - <asp:TextBox ID="txtQEndTime2" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\'txtQEndTime1\')}'})"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <input id="btnQuery" type="submit" value="查询" />
                        <input id="clean" type="reset" value="清空" />
                    </div>
                </div>
                <div class="list">
                    <strong>问卷调查
                        <span>符合条件的数据有：<b><asp:Literal ID="ltSurveyTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="state">类型</th>
                                <th>标题</th>
                                <th class="state">选项数</th>
                                <th class="state">频次</th>
                                <th class="date">开始时间</th>
                                <th class="date">结束时间</th>
                                <th class="state">投票限次</th>
                                <th>规则说明</th>
                                <th class="state">反馈数</th>
                                <th class="state">状态</th>
                                <th class="state">排序</th>
                                <th class="cmd">操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpSurveyList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "OpNum")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "SurveyNum")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "StartTime", "{0:yyyy-MM-dd}<br/>{0:HH:mm:ss}")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "EndTime", "{0:yyyy-MM-dd}<br/>{0:HH:mm:ss}")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "MinNum")%>/<%#DataBinder.Eval(Container.DataItem, "MaxNum")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Body")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ResultNum")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Active")%></td>
                                        <td align="center">
                                            <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>修改</u></a>
                                            <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>&oid=0" class="btn"><u>子选项</u></a>
                                            <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>&rid=0" class="btn"><u>查看反馈</u></a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltSurveyNo" runat="server">
                                <tr><td class="no">暂时没有查询到信息！</td></tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblSurveyNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plSurveyEdit" runat="server" Visible="false">
<script>
    $(function () {
        $('#btnToMans').click(function () {
            var title = '选取委员';
            var obj = 'txtToMans';
            showDialog(title, '../cn/dialog.aspx?ac=subman&obj=' + obj, '', 640, 400, 'no');
            return false;
        });
        $('form').submit(function () {
            try {
                if (checkRadio('#rblSubType') || checkEmpty('#txtTitle') || checkEmpty('#txtSurveyNum') || checkEmpty('#txtStartTime') || checkEmpty('#txtEndTime') || checkEmpty('#txtActive')) {
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
                    <strong><asp:Literal ID="ltSurveyTitle" runat="server" Text="新增问卷调查"></asp:Literal></strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>自编号</th>
                                <td><asp:TextBox ID="txtId" runat="server" ReadOnly="true" CssClass="readonly" Text="0"></asp:TextBox></td>
                                <th><b>*</b>排序状态</th>
                                <td>
                                    <asp:TextBox ID="txtActive" runat="server" MaxLength="4" ToolTip="排序状态" Text="1"></asp:TextBox>
                                    <i>(倒序，大于0时才显示)</i>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>标题</th>
                                <td><asp:TextBox ID="txtTitle" runat="server" MaxLength="50" CssClass="long" ToolTip="标题"></asp:TextBox></td>
                                <th rowspan="7">需参与委员</th>
                                <td rowspan="7"><asp:TextBox ID="txtToMans" runat="server" TextMode="MultiLine" Rows="11" CssClass="readonly long" ToolTip="需要通知的委员"></asp:TextBox><br />
                                    <a id="btnToMans" href="#" class="btn"><u>选取</u></a><i>(全部参与可为空)</i>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>调查类型</th>
                                <td>
                                    <asp:RadioButtonList ID="rblSubType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" ToolTip="调查类型">
                                        <asp:ListItem Text="问卷"></asp:ListItem>
                                        <asp:ListItem Text="投票"></asp:ListItem>
                                        <asp:ListItem Text="答题(计分)" Value="答题"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>调查频次</th>
                                <td><asp:TextBox ID="txtSurveyNum" runat="server" MaxLength="3" Text="0" ToolTip="调查频次"></asp:TextBox>
                                    <i>(-1为不限次数，0为只能提交一次，n为每天提交n次)</i>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>开始时间</th>
                                <td><asp:TextBox ID="txtStartTime" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" ToolTip="开始时间"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>结束时间</th>
                                <td><asp:TextBox ID="txtEndTime" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})" ToolTip="结束时间"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>投票选项下限</th>
                                <td><asp:TextBox ID="txtMinNum" runat="server" MaxLength="2" Text="0"></asp:TextBox>/次 <i>(0为不限)</i></td>
                            </tr>
                            <tr>
                                <th>投票选项上限</th>
                                <td><asp:TextBox ID="txtMaxNum" runat="server" MaxLength="2" Text="0"></asp:TextBox>/次 <i>(0为不限)</i></td>
                            </tr>
                            <tr>
                                <th>规则说明</th>
                                <td><asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" Rows="8" CssClass="long"></asp:TextBox></td>
                                <th>备注</th>
                                <td><asp:TextBox ID="txtRemark" runat="server" TextMode="MultiLine" Rows="8" CssClass="long"></asp:TextBox></td>
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
                        <asp:Button ID="btnSurveyEdit" runat="server" Text="新增" OnClick="btnSurveyEdit_Click" />
                        <asp:Button ID="btnSurveyStop" runat="server" Text="暂停" Visible="false" OnClick="btnSurveyStop_Click" />
                        <asp:Button ID="btnSurveyCancel" runat="server" Text="取消" Visible="false" OnClick="btnSurveyCancel_Click" />
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plOp" runat="server" Visible="false">
<script>
    $(function () {
        upFile('#txtOpPicUrl', '#showPic', '#btnPic', 'img');
        if ($('#btnOpEdit').val() == '修改') {
            $('#nav2>a.hide').show().addClass('cur');
        }
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtOpActive') || checkEmpty('#txtOpTitle')) {
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
                    <a href="?id=<%=Request.QueryString["id"] %>&oid=0">新增选项</a>
                    <a class="hide">修改选项</a>
                </div>
                <div class="frm edit">
                    <strong><asp:Literal ID="ltOpTitle" runat="server" Text="新增选项名称"></asp:Literal></strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>标题</th>
                                <td><asp:TextBox ID="txtSurveyTitle" runat="server" ToolTip="标题" MaxLength="50" ReadOnly="true" CssClass="readonly long"></asp:TextBox></td>
                                <th>自编号</th>
                                <td><asp:TextBox ID="txtOpId" runat="server" Text="0" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>排序状态</th>
                                <td>
                                    <asp:TextBox ID="txtOpActive" runat="server" Text="1" MaxLength="4" ToolTip="排序状态"></asp:TextBox>
                                    <i>(倒序，大于0时才显示)</i>
                                </td>
                                <th rowspan="5">图片地址</th>
                                <td><asp:TextBox ID="txtOpPicUrl" runat="server" ToolTip="图片地址" MaxLength="100" CssClass="long"></asp:TextBox>
                                    <a id="btnPic" href="#" class="btn"><u>上传</u></a>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>选项名称</th>
                                <td><asp:TextBox ID="txtOpTitle" runat="server" ToolTip="选项名称" MaxLength="20"></asp:TextBox></td>
                                <td rowspan="4" id="showPic" class="pic"></td>
                            </tr>
                            <tr>
                                <th>调查方式</th>
                                <td>
                                    <asp:RadioButtonList ID="rblOpMethod" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="单选" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="多选"></asp:ListItem>
                                        <asp:ListItem Text="填写"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <th>答题类型答案</th>
                                <td><asp:TextBox ID="txtOpAnswer" runat="server" MaxLength="20"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>答题类型分值</th>
                                <td><asp:TextBox ID="txtOpScore" runat="server" Text="0"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>调查选项</th>
                                <td><asp:TextBox ID="txtOpBody" runat="server" TextMode="MultiLine" Rows="5" CssClass="long"></asp:TextBox>
                                    <i>（每行一个选项）</i>
                                </td>
                                <th>备注</th>
                                <td><asp:TextBox ID="txtOpRemark" runat="server" TextMode="MultiLine" Rows="5" CssClass="long"></asp:TextBox></td>
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
                        <asp:Button ID="btnOpStop" runat="server" Text="暂停" Visible="false" OnClick="btnOpStop_Click" />
                        <asp:Button ID="btnOpCancel" runat="server" Text="取消" Visible="false" OnClick="btnOpCancel_Click" />
                    </div>
                </div>
                <div class="list">
                    <strong><asp:Literal ID="ltOpSurveySubType" runat="server"></asp:Literal>《<asp:Literal ID="ltOpSurveyTitle" runat="server"></asp:Literal>》选项
                        <span>符合条件的数据有：<b><asp:Literal ID="ltOpTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="pic">图片</th>
                                <th class="state">投票数</th>
                                <th>选项名称</th>
                                <th class="state">答题方式</th>
                                <th class="state">答案</th>
                                <th class="state">分值</th>
                                <th>答题选项</th>
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
                                        <td><%#DataBinder.Eval(Container.DataItem, "PicUrl")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "VoteNum")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Method")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Answer")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Score")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Body")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Active")%></td>
                                        <td align="center">
                                            <a href="?id=<%#DataBinder.Eval(Container.DataItem, "SurveyId")%>&oid=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>选取</u></a>
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

            <asp:PlaceHolder ID="plResult" runat="server" Visible="false">
                <div class="frm list">
                    <strong><asp:Literal ID="ltResultSubType" runat="server"></asp:Literal>《<asp:Literal ID="ltResultTitle" runat="server"></asp:Literal>》反馈情况
                        <span>符合条件的数据有：<b><asp:Literal ID="ltResultTotal" runat="server" Text="0"></asp:Literal></b>条</span>
                        <asp:HyperLink ID="lnkDown" runat="server" Visible="false" Target="_blank" CssClass="btn">下载反馈</asp:HyperLink>
                    </strong>
                    <table>
                        <thead>
                            <tr>
                                <th class="num">序号</th>
                                <th class="state">反馈委员</th>
                                <th>反馈内容</th>
                                <th class="state">得分</th>
                                <th class="time">提交时间</th>
                                <th class="time">提交IP</th>
                                <th class="state">状态</th>
                                <%--<th class="cmd">操作</th>--%>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpResultList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "AddUser")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Body")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ScoreText")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "AddTime", "{0:yyyy-MM-dd HH:mm:ss}")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "AddIp")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <%--<td align="center">
                                            <a href="?id=<%#DataBinder.Eval(Container.DataItem, "SurveyId")%>&rid=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看</u></a>
                                        </td>--%>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltResultNo" runat="server">
                                <tr><td class="no">暂时没有查询到信息！</td></tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblResultNav" runat="server" CssClass="nav"></asp:Label>
                </div>
                <asp:Panel ID="plResultInfo" runat="server" Visible="false" CssClass="edit">
                    <strong>反馈详情</strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>流水号</th>
                                <td><asp:Literal ID="ltResultId" runat="server" Text="0"></asp:Literal></td>
                                <th rowspan="7">内容</th>
                                <td rowspan="7"><asp:Literal ID="ltResultBody" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>反馈委员</th>
                                <td><asp:Literal ID="ltResultUser" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>得分</th>
                                <td><asp:Literal ID="ltResultScore" runat="server" Text="0"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>状态</th>
                                <td><asp:Literal ID="ltResultActive" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>添加时间</th>
                                <td><asp:Literal ID="ltResultAddTime" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>添加IP</th>
                                <td><asp:Literal ID="ltResultAddIp" runat="server"></asp:Literal></td>
                            </tr>
                            <tr>
                                <th>添加人</th>
                                <td><asp:Literal ID="ltResultAddUser" runat="server"></asp:Literal></td>
                            </tr>
                        </tbody>
                    </table>
                </asp:Panel>
            </asp:PlaceHolder>
        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
