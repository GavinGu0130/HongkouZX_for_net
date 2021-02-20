<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="forum.aspx.cs" Inherits="hkzx.web.m.forum" %><%--Tony维护--%>
<%@ Register Src="../cn/ucMeta.ascx" TagName="ucMeta" TagPrefix="uc1" %>
<%@ Register Src="ucHeader.ascx" TagName="ucHeader" TagPrefix="uc1" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <uc1:ucMeta ID="meta1" runat="server" Client="wap" />
    <title>虹口政协履职通 - 委员论坛</title>
</head>
<body>
<form id="form1" runat="server">
    <asp:Literal ID="ltInfo" runat="server"></asp:Literal>
    <asp:HiddenField ID="hfBack" runat="server" Value="./" />
    <uc1:ucHeader ID="header1" runat="server" Title="委员论坛" />
    <div class="content main">
        <asp:PlaceHolder ID="plBoard" runat="server" Visible="false">
            <div class="list bbs">
                <dl class="board">
                    <asp:Repeater ID="rpBoardList" runat="server">
                        <ItemTemplate>
                            <dd>
                                <a href="?bid=<%#DataBinder.Eval(Container.DataItem, "Id")%>">
                                    <b>[<%#DataBinder.Eval(Container.DataItem, "BoardName")%>]</b>
                                    <p><%#DataBinder.Eval(Container.DataItem, "BoardRule")%></p>
                                </a>
                                <div class="bottom">
                                    <p>版主：<%#DataBinder.Eval(Container.DataItem, "BoardMaster")%></p>
                                    <span>今日帖：</span><b><%#DataBinder.Eval(Container.DataItem, "TodayNum")%></b>
                                    <span>主题帖：</span><%#DataBinder.Eval(Container.DataItem, "TopicNum")%>
                                    <span>总帖数：</span><%#DataBinder.Eval(Container.DataItem, "PostNum")%>
                                </div>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltBoardNo" runat="server">
                    <dd class="no">暂时没有版块！</dd>
                    </asp:Literal>
                </dl>
            </div>
        </asp:PlaceHolder>

        <asp:Panel ID="plNav" runat="server" Visible="false" CssClass="btn">
            <div>
                <a href="?bid=">论坛首页</a>
                <asp:HyperLink ID="lnkBoard" runat="server" NavigateUrl="?bid=">返回版块</asp:HyperLink>
                <asp:HyperLink ID="lnkTopic" runat="server" NavigateUrl="?bid={0}&tid={1}" Visible="false">返回主题帖</asp:HyperLink>
                <asp:HyperLink ID="lnkEdit" runat="server" NavigateUrl="?bid={0}&id={1}" Visible="false">编辑帖子</asp:HyperLink>
            </div>
        </asp:Panel>

        <asp:PlaceHolder ID="plTopic" runat="server" Visible="false">
            <div class="list bbs">
                <div class="rule">
                    <b>版规说明：</b>
                    <p>
                        <asp:Literal ID="ltBoardRule" runat="server"></asp:Literal></p>
                </div>
                <div class="master">
                    <b>版主：</b><asp:Literal ID="ltBoardMaster" runat="server"></asp:Literal>
                </div>
                <asp:Panel ID="plTopicCmd" runat="server" Visible="false" CssClass="cmd">
                    <a href="?bid=<%=Request.QueryString["bid"] %>&id=0"><u>发表主题</u></a>
                </asp:Panel>

                <dl class="topic">
                    <dt>
                        <span class="state">状态</span>
                        <span class="title">主题</span>
                        <span class="author">作者</span>
                        <span class="num">回复</span>
                        <span class="num">人气</span>
                        <span class="last">最后回复</span>
                    </dt>
                    <asp:Repeater ID="rpTopicList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                            <span class="state"><i></i></span>
                            <span class="title"><a href="?bid=<%#DataBinder.Eval(Container.DataItem, "BoardId")%>&tid=<%#DataBinder.Eval(Container.DataItem, "Id")%>"><%#DataBinder.Eval(Container.DataItem, "Title")%></a></span>
                            <span class="author"><%#DataBinder.Eval(Container.DataItem, "AddUser")%></span>
                            <span class="num"><%#DataBinder.Eval(Container.DataItem, "ReplyNum")%></span>
                            <span class="num"><%#DataBinder.Eval(Container.DataItem, "ReadNum")%></span>
                            <span class="last"><%#DataBinder.Eval(Container.DataItem, "ReplyTime", "{0:yyyy-MM-dd HH:mm:ss}")%> <%#DataBinder.Eval(Container.DataItem, "ReplyUser")%></span>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Literal ID="ltTopicNo" runat="server">
                    <dd class="no">暂时还没有主题帖哦！</dd>
                    </asp:Literal>
                </dl>
                <asp:Label ID="lblTopicNav" runat="server" CssClass="nav"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plPost" runat="server" Visible="false">
            <script>
                $(function () {
                    $('.post>dd>.body').each(function () {
                        var txt = ubb2html($(this).html());
                        $(this).html(txt);
                    });
                });
            </script>
            <div class="list bbs">
                <div class="master">
                    阅读数：<b><asp:Literal ID="ltReadNum" runat="server">0</asp:Literal></b>
                    回复数：<b><asp:Literal ID="ltReplyNum" runat="server">0</asp:Literal></b>
                </div>
                <asp:Panel ID="plPostCmd" runat="server" Visible="false" CssClass="cmd">
                    <a href="?bid=<%=Request.QueryString["bid"] %>&rid=<%=Request.QueryString["tid"] %>"><u>回复帖子</u></a>
                </asp:Panel>
                <dl class="post">
                    <dt>
                        <asp:Literal ID="ltPostTitle" runat="server"></asp:Literal></dt>
                    <asp:Repeater ID="rpPostList" runat="server">
                        <ItemTemplate>
                            <dd<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                            <div class="user">
                                <%#DataBinder.Eval(Container.DataItem, "UserPhoto")%>
                                <b><%#DataBinder.Eval(Container.DataItem, "AddUser")%></b>
                            </div>
                            <div class="bar">
                                <b><%#DataBinder.Eval(Container.DataItem, "num")%>楼</b>
                                [发表于：<%#DataBinder.Eval(Container.DataItem, "AddTime", "{0:yyyy-MM-dd HH:mm:ss}")%>]
                                <div class="btn"><%#DataBinder.Eval(Container.DataItem, "BtnEdit")%></div>
                            </div>
                            <div class="body"><%#DataBinder.Eval(Container.DataItem, "Body")%></div>
                            <div class="update"><%#DataBinder.Eval(Container.DataItem, "UpTimeText")%></div>
                            </dd>
                        </ItemTemplate>
                    </asp:Repeater>
                </dl>
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
            editor.customConfig.uploadImgServer = '../cn/upload.aspx';
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
            $code.change(function () {
                editor.txt.html(ubb2html($(this).val()));
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
            <div class="edit">
                <dl>
                    <dt><asp:Literal ID="ltTitle" runat="server">发表帖子</asp:Literal></dt>
                    <dd>
                        <strong><b>*</b>主题</strong>
                        <asp:TextBox ID="txtTitle" runat="server" MaxLength="50" CssClass="long" ToolTip="主题"></asp:TextBox>
                    </dd>
                    <dd>
                        <strong><b>*</b>内容</strong>
                        <div id="editor" class="long"><%--<p>欢迎使用 <b>wangEditor</b> 富文本编辑器</p>欢迎使用 [b]wangEditor[/b]富文本编辑器--%></div>
                        <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" Rows="9" CssClass="long" ToolTip="帖子内容"></asp:TextBox>
                    </dd>
                </dl>
                <div class="cmd">
                    <asp:Button ID="btnEdit" runat="server" Text="发表" OnClick="btnEdit_Click" />
                    <input type="reset" value="重置" />
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
</form>
</body>
</html>
