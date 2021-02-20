<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="input.aspx.cs" Inherits="hkzx.web.admin.input" %><%--Tony维护--%>
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
                if (url.indexOf('ac=perform') > 0) {
                    $('#plNav>a[href*="ac=perform"]').addClass('cur').removeAttr('href');
                } else if (url.indexOf('ac=opinion') > 0) {
                    $('#plNav>a[href*="ac=opinion"]').addClass('cur').removeAttr('href');
                } else if (url.indexOf('ac=pop') > 0) {
                    $('#plNav>a[href*="ac=pop"]').addClass('cur').removeAttr('href');
                } else if (url.indexOf('ac=test') > 0) {
                    $('#plNav>a[href*="ac=test"]').addClass('cur').removeAttr('href');
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
                <a href="?ac=user">导入委员信息</a>
                <a href="?ac=perform">导入委员活动情况</a>
                <a href="?ac=opinion">导入提案信息</a>
                <a href="?ac=pop">导入社情民意信息</a>
                <asp:HyperLink ID="lnkTest" runat="server" Visible="false" NavigateUrl="?ac=test">测试数据</asp:HyperLink>
            </asp:Panel>

            <asp:PlaceHolder ID="plUser" runat="server" Visible="false">
<script>
    $(function () {
        upFile('#txtUserFile', '', '#btnUserFile', 'xls');
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtUserFile')) {
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
                    <strong>导入委员信息到数据库</strong>
                    <table>
                        <tbody>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="ddlPeriod" runat="server">
                                        <asp:ListItem Text="十四届" Value="十四"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUserFile" runat="server" CssClass="double" ToolTip="文件名"></asp:TextBox>
                                    <a id="btnUserFile" href="#" class="btn"><u>上传文件</u></a>
                                    <i>请先上传xls文件，再导入到数据库。</i>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnUser" runat="server" Text="导入" OnClick="btnUser_Click" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plUserData" runat="server" Visible="false">
                <div class="list">
                    <strong>导入的数据</strong>
                    <table>
                        <thead>
                            <tr>
                                <th>委员编号</th>
                                <th>姓名</th>
                                <th>性别</th>
                                <th>籍贯</th>
                                <th>民族</th>
                                <th class="date">委员生日</th>
                                <th>政治面貌</th>
                                <th>手机</th>
                                <th>单位名称及职务</th>
                                <th>专委会</th>
                                <th>界别</th>
                                <th>界别活动组</th>
                                <th>社区活动组</th>
                                <th>政协职务</th>
                                <th>操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpUserList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "UserCode")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "TrueName")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "UserSex")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Native")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Nation")%></td>
                                        <td align="center"><%#DataBinder.Eval(Container.DataItem, "Birthday", "{0:yyyy-MM-dd}")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Party")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Mobile")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OrgName")%><br /><%#DataBinder.Eval(Container.DataItem, "OrgDuty")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Committee")%><br /><%#DataBinder.Eval(Container.DataItem, "Committee2")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Subsector")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Subsector2")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "StreetTeam")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Role")%></td>
                                        <td><a href="?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plPerform" runat="server" Visible="false">
<script>
    $(function () {
        upFile('#txtPerformFile', '', '#btnPerformFile', 'xls');
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtPerformFile')) {
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
                    <strong>导入委员活动情况到数据库</strong>
                    <table>
                        <tbody>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtPerformFile" runat="server" CssClass="double" ToolTip="文件名"><%--../upload/file/20181229/201812290234080875.xls--%></asp:TextBox>
                                    <a id="btnPerformFile" href="#" class="btn"><u>上传文件</u></a>
                                    <i>请先上传xls文件，再导入到数据库。</i>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnPerform" runat="server" Text="导入" OnClick="btnPerform_Click" />
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plOpinion" runat="server" Visible="false">
<script>
    $(function () {
        upFile('#txtOpFile', '', '#btnOpFile', 'xls');
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtOpFile')) {
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
                    <strong>导入提案信息到数据库</strong>
                    <table>
                        <tbody>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtOpFile" runat="server" CssClass="double" ToolTip="文件名"></asp:TextBox>
                                    <a id="btnOpFile" href="#" class="btn"><u>上传文件</u></a>
                                    <i>请先上传xls文件，再导入到数据库。</i>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnOpinion" runat="server" Text="导入" OnClick="btnOpinion_Click" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plOpData" runat="server" Visible="false">
                <div class="list hover">
                    <strong>导入的数据</strong>
                    <table>
                        <thead>
                            <tr>
                                <th>流水号</th>
                                <th>提案序号</th>
                                <th>提案者</th>
                                <th>主办单位</th>
                                <th>会办单位</th>
                                <th>办理结果</th>
                                <th>意见反馈</th>
                                <th>案由</th>
                                <th>性质</th>
                                <th>再办理</th>
                                <th>跟踪办理</th>
                                <th>跟踪办理结果</th>
                                <th>提案分类</th>
                                <th>操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpOpList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "OpNum")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "OpNo")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubMan")%><br /><%#DataBinder.Eval(Container.DataItem, "SubMan2")%><br /><%#DataBinder.Eval(Container.DataItem, "SubMans")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "ExamHostOrg")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "ExamHelpOrg")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "ResultInfo")%></td>
                                        <td></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Summary")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "ActiveName")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "ReApply")%></td>
                                        <td></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "ResultInfo2")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubType")%></td>
                                        <td><a href="opinion.aspx?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plPop" runat="server" Visible="false">
<script>
    $(function () {
        upFile('#txtPopFile', '', '#btnPopFile', 'xls');
        $('form').submit(function () {
            try {
                if (checkEmpty('#txtPopFile')) {
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
                    <strong>导入社情民意信息到数据库</strong>
                    <table>
                        <tbody>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtPopFile" runat="server" CssClass="double" ToolTip="文件名"><%--../upload/file/20181229/201812290040456278.xls--%></asp:TextBox>
                                    <a id="btnPopFile" href="#" class="btn"><u>上传文件</u></a>
                                    <i>请先上传xls文件，再导入到数据库。</i>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="cmd">
                        <asp:Button ID="btnPop" runat="server" Text="导入" OnClick="btnPop_Click" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plPopData" runat="server" Visible="false">
                <div class="list hover">
                    <strong>导入的数据</strong>
                    <table>
                        <thead>
                            <tr>
                                <th>序号</th>
                                <th>来稿时间</th>
                                <th>姓名</th>
                                <th>界别</th>
                                <th>反映单位</th>
                                <th>政治面貌</th>
                                <th>信息标题</th>
                                <th>期数</th>
                                <th>区政协采用情况</th>
                                <th>报区</th>
                                <th>区采纳落实情况</th>
                                <th>报市</th>
                                <th>市政协采用情况</th>
                                <th>全国政协采用/市(全国)领导批示</th>
                                <th>操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rpPopList" runat="server">
                                <ItemTemplate>
                                    <tr<%#DataBinder.Eval(Container.DataItem, "rowClass")%>>
                                        <th><%#DataBinder.Eval(Container.DataItem, "OpNum")%></th>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubTime", "{0:M月}")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubMan")%><br /><%#DataBinder.Eval(Container.DataItem, "SubMans")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Subsector")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Committee")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Party")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Summary")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "SubType2")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Adopt1")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Give1")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Employ1")%>
                                            <%#DataBinder.Eval(Container.DataItem, "Reply1")%>
                                        </td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Send2")%></td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Give2")%>
                                            <%#DataBinder.Eval(Container.DataItem, "Employ2")%>
                                            <%#DataBinder.Eval(Container.DataItem, "Reply2")%>
                                            <%#DataBinder.Eval(Container.DataItem, "Give3")%>
                                        </td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "Employ3")%>
                                            <%#DataBinder.Eval(Container.DataItem, "Reply3")%>
                                        </td>
                                        <td><a href="opinion_pop.aspx?id=<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn"><u>查看</u></a></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plIn" runat="server" Visible="false">
                <div class="list hover">
                    <strong>导入的数据</strong>
                    <asp:Literal ID="ltInData" runat="server"></asp:Literal>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plTest" runat="server" Visible="false">
                <div class="frm edit">
                    <div class="cmd">
                        <asp:Button ID="btnTestReport" runat="server" Text="生成100w条调研报告" OnClick="btnTestReport_Click" />
                        <asp:Button ID="btnTestOpinion" runat="server" Text="生成100w条提案" OnClick="btnTestOpinion_Click" />
                        <asp:Button ID="btnTestPop" runat="server" Text="生成100w条社情民意" OnClick="btnTestPop_Click" />
                        <asp:Button ID="btnTestPic" runat="server" Text="生成100G文件" OnClick="btnTestPic_Click" />
                    </div>
                    <asp:Literal ID="ltTest" runat="server"></asp:Literal>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
    <uc1:ucFooter ID="footer1" runat="server" />
</form>
</body>
</html>
