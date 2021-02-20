<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="forum.aspx.cs" Inherits="hkzx.web.admin.forum" %><%--Tony维护--%>
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
                if (url.indexOf('ac=set') > 0) {
                    $('#plNav>a[href*="ac=set"]').addClass('cur').removeAttr('href');
                } else if (url.indexOf('?id=0') > 0) {
                    $('#plNav>a[href*="id=0"]').addClass('cur').removeAttr('href');
                } else if (url.indexOf('?id=') > 0) {
                    $('#plNav>a#update').show().addClass('cur');
                } else {
                    $('#plNav>a:first').addClass('cur').removeAttr('href');
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
            <asp:PlaceHolder ID="plMeeting" runat="server" Visible="false">
                <div class="frm edit">
                    <strong><asp:Literal ID="ltMTitle" runat="server" Text="全会时间设置"></asp:Literal></strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>自编号</th>
                                <td><asp:TextBox ID="txtMId" runat="server" ReadOnly="true" CssClass="readonly" Text="0"></asp:TextBox></td>
                                <th>标签</th>
                                <td><asp:TextBox ID="txtMLabel" runat="server" ReadOnly="true" CssClass="readonly" Text="meeting"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>会议开始时间</th>
                                <td><asp:TextBox ID="txtMStartDate" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})"></asp:TextBox></td>
                                <th rowspan="2">备注</th>
                                <td rowspan="2"><asp:TextBox ID="txtMRemark" runat="server" TextMode="MultiLine" Rows="3" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>会议结束时间</th>
                                <td><asp:TextBox ID="txtMEndDate" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加时间</th>
                                <td><asp:TextBox ID="txtMAddTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改时间</th>
                                <td><asp:TextBox ID="txtMUpTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加IP</th>
                                <td><asp:TextBox ID="txtMAddIp" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改IP</th>
                                <td><asp:TextBox ID="txtMUpIp" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加人</th>
                                <td><asp:TextBox ID="txtMAddUser" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改人</th>
                                <td><asp:TextBox ID="txtMUpUser" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnMeeting" runat="server" Text="新增" OnClick="btnMeeting_Click" />
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:Panel ID="plNav" runat="server" Visible="false" CssClass="btn">
                <a href="?id=0">新增论坛版块</a>
                <a id="update" class="hide">修改论坛版块</a>
                <a href="?ac=set">论坛设置</a>
            </asp:Panel>

            <asp:PlaceHolder ID="plSet" runat="server" Visible="false">
                <div class="frm edit">
                    <strong><asp:Literal ID="ltSetTitle" runat="server" Text="论坛设置"></asp:Literal></strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>自编号</th>
                                <td><asp:TextBox ID="txtSetId" runat="server" ReadOnly="true" CssClass="readonly" Text="0"></asp:TextBox></td>
                                <th>标签</th>
                                <td><asp:TextBox ID="txtSetLabel" runat="server" ReadOnly="true" CssClass="readonly" Text="forum"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>发帖开始日期</th>
                                <td><asp:TextBox ID="txtSetStartDate" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox></td>
                                <th rowspan="3">禁止发帖日期<br /><br />
                                    (每行为一个日期)<br />例：2018-12-31
                                </th>
                                <td rowspan="3"><asp:TextBox ID="txtSetNoPostDate" runat="server" TextMode="MultiLine" Rows="5" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>发帖结束日期</th>
                                <td><asp:TextBox ID="txtSetEndDate" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd'})"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>发帖星期时间</th>
                                <td>
                                    <asp:CheckBoxList ID="cblSetPostWeek" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="星期日" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="星期一" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="星期二" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="星期三" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="星期四" Value="4"></asp:ListItem>
                                        <asp:ListItem Text="星期五" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="星期六" Value="6"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>当天开始时间</th>
                                <td><asp:TextBox ID="txtSetPostStart" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'HH:mm'})"></asp:TextBox></td>
                                <th rowspan="3">备注</th>
                                <td rowspan="3"><asp:TextBox ID="txtSetRemark" runat="server" TextMode="MultiLine" Rows="5" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>当天结束时间</th>
                                <td><asp:TextBox ID="txtSetPostEnd" runat="server" CssClass="Wdate" onfocus="WdatePicker({dateFmt:'HH:mm'})"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>发帖默认状态</th>
                                <td>
                                    <asp:RadioButtonList ID="rblSetPostActive" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="发帖需审核" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="发帖直接显示" Value="1" Selected="True"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <th>添加时间</th>
                                <td><asp:TextBox ID="txtSetAddTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改时间</th>
                                <td><asp:TextBox ID="txtSetUpTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加IP</th>
                                <td><asp:TextBox ID="txtSetAddIp" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改IP</th>
                                <td><asp:TextBox ID="txtSetUpIp" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加人</th>
                                <td><asp:TextBox ID="txtSetAddUser" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改人</th>
                                <td><asp:TextBox ID="txtSetUpUser" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnSetEdit" runat="server" Text="新增" OnClick="btnSetEdit_Click" />
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plBoard" runat="server" Visible="false">
<script>
    $(function () {
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtBoardName')) {
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
                    <strong><asp:Literal ID="ltBoardTitle" runat="server" Text="新增论坛版块"></asp:Literal></strong>
                    <table>
                        <tbody>
                            <tr>
                                <th>自编号</th>
                                <td><asp:TextBox ID="txtBoardId" runat="server" Text="0" CssClass="readonly" ReadOnly="true"></asp:TextBox></td>
                                <th><b>*</b>排序状态</th>
                                <td>
                                    <asp:TextBox ID="txtBoardActive" runat="server" Text="1" MaxLength="4" ToolTip="排序状态"></asp:TextBox>
                                    <i>(倒序)</i>
                                </td>
                            </tr>
                            <tr>
                                <th><b>*</b>版块名称</th>
                                <td><asp:TextBox ID="txtBoardName" runat="server" ToolTip="版块名称" MaxLength="20"></asp:TextBox></td>
                                <th>版主</th>
                                <td><asp:TextBox ID="txtBoardMaster" runat="server" ToolTip="版主" MaxLength="100" CssClass="long"></asp:TextBox>
                                    <i>(姓名|姓名)</i>
                                </td>
                            </tr>
                            <tr>
                                <th rowspan="2">版规</th>
                                <td rowspan="2"><asp:TextBox ID="txtBoardRule" runat="server" TextMode="MultiLine" Rows="5" CssClass="long"></asp:TextBox></td>
                                <th><b>*</b>发帖默认状态</th>
                                <td>
                                    <asp:RadioButtonList ID="rblBoardPostActive" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Text="发帖需审核" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="发帖直接显示" Value="1" Selected="True"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <th>备注</th>
                                <td><asp:TextBox ID="txtBoardRemark" runat="server" TextMode="MultiLine" Rows="3" CssClass="long"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加时间</th>
                                <td><asp:TextBox ID="txtBoardAddTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改时间</th>
                                <td><asp:TextBox ID="txtBoardUpTime" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加IP</th>
                                <td><asp:TextBox ID="txtBoardAddIp" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改IP</th>
                                <td><asp:TextBox ID="txtBoardUpIp" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th>添加人</th>
                                <td><asp:TextBox ID="txtBoardAddUser" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                                <th>修改人</th>
                                <td><asp:TextBox ID="txtBoardUpUser" runat="server" ReadOnly="true" CssClass="readonly"></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnBoardEdit" runat="server" Text="新增" OnClick="btnBoardEdit_Click" />
                        <asp:Button ID="btnBoardCancel" runat="server" Text="取消" Visible="false" OnClick="btnBoardCancel_Click" />
                    </div>
                </div>
                <div class="list bbs board">
                    <table>
                        <thead>
                            <tr>
                                <td colspan="2">委员论坛</td>
                                <td>状态/操作</td>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpBoardList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th class="num"><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td>
                                            <table>
                                                <tbody>
                                                    <tr>
                                                        <td><b><a href="?bid=<%#DataBinder.Eval(Container.DataItem, "Id")%>">[<%#DataBinder.Eval(Container.DataItem, "BoardName")%>]</a></b><br />
                                                            <i></i>
                                                            <p><%#DataBinder.Eval(Container.DataItem, "BoardRule")%></p>
                                                        </td>
                                                        <td class="less">
                                                            <b>主题：</b><span><%#DataBinder.Eval(Container.DataItem, "TopicTitle")%></span><br />
                                                            <b>发帖：</b><span><%#DataBinder.Eval(Container.DataItem, "TopicUser")%></span><br />
                                                            <b>日期：</b><span><%#DataBinder.Eval(Container.DataItem, "TopicTime")%></span><br />
                                                        </td>
                                                    </tr>
                                                    <tr class="bottom">
                                                        <td>版主：<%#DataBinder.Eval(Container.DataItem, "BoardMaster")%></td>
                                                        <td class="less">
                                                            <div><span>今日帖：</span><b><%#DataBinder.Eval(Container.DataItem, "TodayNum")%></b></div>
                                                            <div><span>主题帖：</span><%#DataBinder.Eval(Container.DataItem, "TopicNum")%></div>
                                                            <div><span>总帖数：</span><%#DataBinder.Eval(Container.DataItem, "PostNum")%></div>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                        </td>
                                        <td class="cmd">
                                            <%#DataBinder.Eval(Container.DataItem, "ActiveName")%><br />
                                            <a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>修改</u></a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltBoardNo" runat="server">
                                <tr><td class="no">暂时没有版块！</td></tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plBbs" runat="server" Visible="false">
                <div class="line"></div>
            </asp:PlaceHolder>

            <asp:Panel ID="plNav2" runat="server" Visible="false" CssClass="btn">
                <a href="?bid=">论坛首页</a>
                <asp:HyperLink ID="lnkBoard" runat="server" NavigateUrl="?bid=">返回版块</asp:HyperLink>
                <asp:HyperLink ID="lnkTopic" runat="server" NavigateUrl="?bid={0}&tid={1}" Visible="false">返回主题帖</asp:HyperLink>
                <asp:HyperLink ID="lnkEdit" runat="server" NavigateUrl="?bid={0}&id={1}" Visible="false">编辑帖子</asp:HyperLink>
            </asp:Panel>

            <asp:PlaceHolder ID="plTopic" runat="server" Visible="false">
                <div class="line"></div>
                <div class="list bbs topic">
                    <div class="rule">
                        <b>版规说明：</b>
                        <p><asp:Literal ID="ltBoardRule" runat="server"></asp:Literal></p>
                    </div>
                    <div class="master">
                        <b>版主：</b><asp:Literal ID="ltBoardMaster" runat="server"></asp:Literal>
                    </div>
                    <div class="cmd">
                        <a href="?bid=<%=Request.QueryString["bid"] %>&id=0"><u>发表主题</u></a>
                    </div>
                    <table>
                        <thead>
                            <tr>
                                <th class="num"></th>
                                <th>主题</th>
                                <th class="state">作者</th>
                                <th class="state">回复</th>
                                <th class="state">人气</th>
                                <th class="last">最后回复</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpTopicList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "num")%></th>
                                        <td><span><a href="?bid=<%#DataBinder.Eval(Container.DataItem, "BoardId")%>&tid=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><%#DataBinder.Eval(Container.DataItem, "Title")%></a></span></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "AddUser")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ReplyNum")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "ReadNum")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "ReplyTime", "{0:yyyy-MM-dd HH:mm:ss}")%> <%#DataBinder.Eval(Container.DataItem, "ReplyUser")%></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Literal ID="ltTopicNo" runat="server">
                                <tr><td class="no">暂时还没有主题帖哦！</td></tr>
                            </asp:Literal>
                        </tbody>
                    </table>
                    <asp:Label ID="lblTopicNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plPost" runat="server" Visible="false">
<script>
    $(function () {
        $('.post>table>tbody>tr>td>.body').each(function () {
            var txt = ubb2html($(this).html());
            $(this).html(txt);
        });
        $('.list>table>tbody>tr>td>.bar>.btn>a.cmd').click(function () {
            if (!confirm('您确定要“' + $(this).text() + '”吗？')) {
                return false;
            }
        });
    });
</script>
                <div class="line"></div>
                <div class="list bbs post">
                    <div class="master">
                        阅读数：<b><asp:Literal ID="ltReadNum" runat="server">0</asp:Literal></b>
                        回复数：<b><asp:Literal ID="ltReplyNum" runat="server">0</asp:Literal></b>
                    </div>
                    <div class="cmd">
                        <a href="?bid=<%=Request.QueryString["bid"] %>&rid=<%=Request.QueryString["tid"] %>"><u>回复帖子</u></a>
                    </div>
                    <table>
                        <thead>
                            <tr>
                                <th></th>
                                <td><asp:Literal ID="ltPostTitle" runat="server"></asp:Literal></td>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpPostList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th>
                                            <%#DataBinder.Eval(Container.DataItem, "AddUser")%>
                                            <%#DataBinder.Eval(Container.DataItem, "UserPhoto")%>
                                        </th>
                                        <td>
                                            <div class="bar">
                                                <b><%#DataBinder.Eval(Container.DataItem, "num")%>楼</b>
                                                [发表于：<%#DataBinder.Eval(Container.DataItem, "AddTime", "{0:yyyy-MM-dd HH:mm:ss}")%>]
                                                <div class="btn"><%#DataBinder.Eval(Container.DataItem, "BtnEdit")%></div>
                                            </div>
                                            <div class="body"><%#DataBinder.Eval(Container.DataItem, "Body")%></div>
                                            <div class="update"><%#DataBinder.Eval(Container.DataItem, "UpTimeText")%></div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <asp:Label ID="lblPostNav" runat="server" CssClass="nav"></asp:Label>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plEdit" runat="server" Visible="false">
<script>
    $(function () {
        var $code = $('#txtBody');
        var $editor = $('#editor');
        try {
            var E = window.wangEditor;
            var editor = new E('#editor');
            editor.customConfig.uploadImgMaxLength = 5;//限制一次最多上传 5 张图片
            editor.customConfig.uploadImgMaxSize = 10 * 1024 * 1024;//将图片大小限制为 10M
            //editor.customConfig.showLinkImg = false;//网络图片
            //editor.customConfig.debug = true;//打开debug模式
            //editor.customConfig.uploadImgShowBase64 = true;
            editor.customConfig.uploadImgServer = 'upload.aspx';
            editor.customConfig.onchange = function (html) {
                $code.val(html2ubb(html));// 监控变化，同步更新到 textarea
            }
            editor.customConfig.onblur = function (html) {
                $code.val(html2ubb(html));// 监控变化，同步更新到 textarea
            }
            //if ($('meta[name=viewport]').attr('content').indexOf('width=device-width') >= 0) {
            //    editor.customConfig.menus = [
            //        'bold', 'italic', 'underline', 'list', 'justify', 'emoticon', 'image', 'undo', 'redo'//, 'head', 'quote', 'table'
            //    ]
            //}
            editor.create();
            editor.txt.html('<p>' + ubb2html($code.val()) + '</p>');
            //loadImg();
            $code.change(function () {
                editor.txt.html(ubb2html($(this).val()));
                //loadImg(1);
            }).hide();
            //$('#btnFormat').click(function () {
            //    var txt = ubb2text($code.val());
            //    $code.val(txt).change();
            //    //alert(txt);
            //    //alert(($editor).html());
            //});
        } catch (err) {
        }
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtTitle') || checkEmpty('#txtBody')) {
                    return false;
                }
                return true;
            } catch (err) {
                //alert("验证出错，请稍后重试！");
                return false;
            }
        });
    });
</script>
                <asp:HiddenField ID="hfBoardId" runat="server" Value="0" />
                <asp:HiddenField ID="hfTopicId" runat="server" Value="0" />
                <asp:HiddenField ID="hfId" runat="server" Value="0" />
                <div class="frm edit">
                    <strong><asp:Literal ID="ltTitle" runat="server">发表帖子</asp:Literal></strong>
                    <table>
                        <tbody>
                            <tr>
                                <th><b>*</b>主题</th>
                                <td><asp:TextBox ID="txtTitle" runat="server" MaxLength="50" CssClass="long" ToolTip="主题"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th><b>*</b>内容</th>
                                <td>
                                    <div id="editor" class="long"><%--<p>欢迎使用 <b>wangEditor</b> 富文本编辑器</p>欢迎使用 [b]wangEditor[/b]富文本编辑器--%></div>
                                    <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" Rows="9" CssClass="long" ToolTip="帖子内容"></asp:TextBox>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnEdit" runat="server" Text="发表" OnClick="btnEdit_Click" />
                        <input type="reset" value="重置" />
                    </div>
                </div>
            </asp:PlaceHolder>

        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
