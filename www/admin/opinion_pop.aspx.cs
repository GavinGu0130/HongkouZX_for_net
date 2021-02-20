using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mod.main;
using hkzx.db;
using hkzx.user;

namespace hkzx.web.admin
{
    public partial class opinion_pop : System.Web.UI.Page
    {
        private DataAdmin myUser = null;
        WebOpinionPop webPop = new WebOpinionPop();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperAdmin.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
            {
                //Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            if (myUser.Powers.IndexOf("alls") < 0 && myUser.Powers.IndexOf("pop") < 0)
            {
                Response.Redirect("./");
            }
            if (Request.QueryString["ac"] == "output")
            {
                Header.Title += " - 社情民意 导出选项";
                plOutput.Visible = true;
                header1.Visible = false;
                footer1.Visible = false;
                plMain.Visible = false;
            }
            else
            {
                header1.UserName = myUser.TrueName;
                header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
                header1.Powers = myUser.Powers;
                plNav.Visible = true;
                if (myUser.TrueName == "Tony")
                {
                    lnkEdit.Visible = true;
                }
                if (!IsPostBack)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                    {
                        int intId = Convert.ToInt32(Request.QueryString["id"]);
                        if (Request.QueryString["ac"] == "verify")
                        {
                            Header.Title += " - 导出核稿单";
                            outVerify(intId);
                        }
                        else
                        {
                            Header.Title += " - 查阅社情民意";
                            plEdit.Visible = true;
                            loadData(intId);
                        }
                    }
                    else
                    {
                        Header.Title += " - 社情民意查询";
                        plQuery.Visible = true;
                        queryData();
                    }
                }
            }
        }
        //
        #region 查询
        //首页列表
        public void MyList(Repeater rpList, Literal ltNo)
        {
            DataOpinionPop qData = new DataOpinionPop();
            qData.ActiveName = "待审核";
            //qData.SubTimeText = DateTime.Now.ToString("yyyy-MM-dd") + ",";
            string strOrder = "o.SubTime ASC, o.UpTime DESC, o.AddTime DESC";
            listData(qData, strOrder, rpList, ltNo);
        }
        //查询列表
        protected void queryData()
        {
            WebOp webOp = new WebOp();
            PublicMod.LoadDropDownList(ddlQSubType, webOp, "社情民意类别");
            PublicMod.LoadCheckBoxList(cblQLinkmanInfo, webOp, "反映人身份");
            PublicMod.LoadDropDownList(ddlQCommittee, webOp, "专委会");
            PublicMod.LoadDropDownList(ddlQSubsector, webOp, "界别");
            PublicMod.LoadDropDownList(ddlQStreetTeam, webOp, "街道活动组");
            PublicMod.LoadDropDownList(ddlQParty, webOp, "政治面貌");
            PublicMod.LoadCheckBoxList(cblQActiveName, webOp, "社情民意录用状态");
            //PublicMod.LoadDropDownList(ddlQOrderBy, webOp, "社情民意查询信息");
            PublicMod.LoadCheckBoxList(cblQFields, webOp, "社情民意查询信息", "*");
            for (int i = 0; i < cblQFields.Items.Count; i++)
            {
                ListItem item = new ListItem(cblQFields.Items[i].Text, cblQFields.Items[i].Value);
                ddlQOrderBy.Items.Add(item);
            }
            //HelperMain.SetDownSelected(ddlQOrderBy, "Id");//流水号
            string strFields = "";
            if (!string.IsNullOrEmpty(Request.QueryString["Fields"]))
            {
                strFields = Request.QueryString["Fields"].Trim();
            }
            else
            {
                strFields = HelperMain.GetCheckSelected(cblQFields);
            }
            if (string.IsNullOrEmpty(strFields))
            {
                return;
            }
            HelperMain.SetCheckSelected(cblQFields, strFields);
            string strThead = "";
            string[] arrFields = strFields.Split(',');
            for (int i = 0; i < cblQFields.Items.Count; i++)
            {
                for (int j = 0; j < arrFields.Count(); j++)
                {
                    if (cblQFields.Items[i].Value == arrFields[j])
                    {
                        strThead += "<th>" + cblQFields.Items[i].Text + "</th>";
                        break;
                    }
                }
            }
            ltQueryThead.Text = strThead;
            DataOpinionPop qData = getData();
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            string[] tmpFields = new string[arrFields.Count()];
            for (int i = 0; i < arrFields.Count(); i++)
            {
                tmpFields[i] = "o." + arrFields[i];
                if (arrFields[i] == "SubMan")
                {
                    tmpFields[i] += ",o.Linkman";
                }
            }
            string sqlFields = string.Join(",", tmpFields);
            if (!string.IsNullOrEmpty(Request.QueryString["Order"]) && !string.IsNullOrEmpty(Request.QueryString["By"]))
            {
                HelperMain.SetDownSelected(ddlQOrderBy, Request.QueryString["Order"]);
                HelperMain.SetRadioSelected(rblQOrderBy, Request.QueryString["By"]);
            }
            string strOrderBy = ddlQOrderBy.SelectedValue + " " + rblQOrderBy.SelectedValue;
            string strJoin = "";
            //if (!string.IsNullOrEmpty(Request.QueryString["UserParty"]) || !string.IsNullOrEmpty(Request.QueryString["UserSex"]))
            //{
            //    strJoin += " LEFT JOIN tb_User AS u ON (u.Id=o.UserId)";
            //}
            
            DataOpinionPop[] data = webPop.GetDatas(qData, sqlFields, pageCur, pageSize, strOrderBy, "total", strJoin);
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    switch (data[i].ActiveName)
                    {
                        //case "未录用":
                        //    data[i].rowClass = " class='cancel' title='未录用'";
                        //    break;
                        //case "提交":
                        case "待审核":
                            data[i].rowClass = " class='save' title='待审核'";
                            break;
                        case "暂存":
                            data[i].rowClass = " class='save' title='暂存'";
                            break;
                        case "退回":
                            data[i].rowClass = " class='cancel' title='退回'";
                            break;
                        case "删除":
                            data[i].rowClass = " class='del' title='删除'";
                            break;
                        default://已录用、留存
                            break;
                    }
                    string strTr = "";
                    for (int j = 0; j < arrFields.Count(); j++)
                    {
                        strTr += "<td>" + loadFields(data[i], arrFields[j]) + "</td>";
                    }
                    data[i].tbody = strTr;
                }
                rpQueryTbody.DataSource = data;
                rpQueryTbody.DataBind();
                ltQueryNo.Visible = false;
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = Request.Url.ToString();
                lblQueryNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
                ltQueryTotal.Text = data[0].total.ToString();
            }
        }
        private string loadFields(DataOpinionPop data, string strField)
        {//Id,OpNo,SubMan,Summary,ActiveName,SubType,SubTime
            string strOut = "";
            switch (strField)
            {
                case "Id":
                    strOut = data.Id.ToString();
                    break;
                case "OpNum":
                    strOut = data.OpNum;
                    break;
                case "OrgName":
                    strOut = data.OrgName;
                    break;
                case "SubMan":
                    strOut = data.SubMan;
                    if (!string.IsNullOrEmpty(data.Linkman) && data.Linkman != data.SubMan)
                    {
                        strOut += "<br/>" + data.Linkman;
                    }
                    break;
                case "SubMans":
                    strOut = data.SubMans.Trim(',');
                    break;
                case "Committee":
                    strOut = data.Committee;
                    break;
                //case "Linkman":
                //    strOut = data.Linkman;
                //    break;
                case "LinkmanInfo":
                    strOut = data.LinkmanInfo;
                    break;
                case "LinkmanParty":
                    strOut = data.LinkmanParty;
                    break;
                case "LinkmanOrgName":
                    strOut = data.LinkmanOrgName;
                    break;
                case "LinkmanTel":
                    strOut = data.LinkmanTel;
                    break;
                case "Summary":
                    strOut = data.Summary;
                    break;
                case "ActiveName":
                    strOut = data.ActiveName;
                    break;
                case "SubType":
                    strOut = data.SubType;
                    break;
                case "SubTime":
                    strOut = (data.SubTime > DateTime.MinValue) ? data.SubTime.ToString("yyyy-MM-dd") : "";
                    break;
                default:
                    break;
            }
            if (strOut == "")
            {
                strOut = "&nbsp;";
            }
            return strOut;
        }
        private DataOpinionPop getData()
        {
            DataOpinionPop qData = new DataOpinionPop();
            if (!string.IsNullOrEmpty(Request.QueryString["ActiveName"]))
            {
                qData.ActiveName = HelperMain.SqlFilter(Request.QueryString["ActiveName"].Trim(), 20);
            }
            else
            {
                qData.ActiveName = "待审核,已录用,留存,退回";//<>'暂存'
            }
            HelperMain.SetCheckSelected(cblQActiveName, qData.ActiveName);
            if (!string.IsNullOrEmpty(Request.QueryString["Committee"]))
            {
                qData.Committee = HelperMain.SqlFilter(Request.QueryString["Committee"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQCommittee, qData.Committee);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Subsector"]))
            {
                qData.Subsector = HelperMain.SqlFilter(Request.QueryString["Subsector"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQSubsector, qData.Subsector);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["StreetTeam"]))
            {
                qData.StreetTeam = HelperMain.SqlFilter(Request.QueryString["StreetTeam"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQStreetTeam, qData.StreetTeam);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Party"]))
            {
                qData.Party = HelperMain.SqlFilter(Request.QueryString["Party"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQParty, qData.Party);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["SubType"]))
            {
                qData.SubType = HelperMain.SqlFilter(Request.QueryString["SubType"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQSubType, qData.SubType);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["SubMan"]))
            {
                qData.SubMan1 = HelperMain.SqlFilter(Request.QueryString["SubMan"].Trim());
                txtQSubMan.Text = qData.SubMan1;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["SubMans"]))
            {
                qData.SubMans = HelperMain.SqlFilter(Request.QueryString["SubMans"].Trim());
                txtQSubMans.Text = qData.SubMans;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["LinkmanInfo"]))
            {
                qData.LinkmanInfo = HelperMain.SqlFilter(Request.QueryString["LinkmanInfo"].Trim(), 50);
                HelperMain.SetCheckSelected(cblQLinkmanInfo, qData.LinkmanInfo);
            }
            //if (!string.IsNullOrEmpty(Request.QueryString["LinkmanParty"]))
            //{
            //    qData.LinkmanParty = HelperMain.SqlFilter(Request.QueryString["LinkmanParty"].Trim(), 20);
            //    HelperMain.SetDownSelected(ddlQLinkmanParty, qData.LinkmanParty);
            //}
            if (!string.IsNullOrEmpty(Request.QueryString["LinkmanOrgName"]))
            {
                qData.LinkmanOrgName = "%" + HelperMain.SqlFilter(Request.QueryString["LinkmanOrgName"].Trim(), 20) + "%";
                txtQLinkmanOrgName.Text = qData.LinkmanOrgName.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["LinkmanTel"]))
            {
                qData.LinkmanTel = "%" + HelperMain.SqlFilter(Request.QueryString["LinkmanTel"].Trim(), 20) + "%";
                txtQLinkmanTel.Text = qData.LinkmanTel.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["SubTime"]) && Request.QueryString["SubTime"].IndexOf(",") >= 0)
            {
                string strTime = Request.QueryString["SubTime"];
                string strTime1 = strTime.Substring(0, strTime.IndexOf(","));
                string strTime2 = strTime.Substring(strTime.IndexOf(",") + 1);
                txtQSubTime1.Text = HelperMain.SqlFilter(strTime1.Trim(), 10);
                txtQSubTime2.Text = HelperMain.SqlFilter(strTime2.Trim(), 10);
                if (txtQSubTime1.Text != "" || txtQSubTime2.Text != "")
                {
                    qData.SubTimeText = txtQSubTime1.Text + "," + txtQSubTime2.Text;
                }
            }
            else if (Request.QueryString["ac"] != "query")
            {
                txtQSubTime1.Text = DateTime.Today.ToString("yyyy-01-01");
                qData.SubTimeText = txtQSubTime1.Text + ",";
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Summary"]))
            {
                qData.Summary = "%" + HelperMain.SqlFilter(Request.QueryString["Summary"].Trim(), 100) + "%";
                txtQSummary.Text = qData.Summary.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Body"]))
            {
                qData.Body = "%" + HelperMain.SqlFilter(Request.QueryString["Body"].Trim(), 100) + "%";
                txtQBody.Text = qData.Body.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Advise"]))
            {
                qData.Advise = "%" + HelperMain.SqlFilter(Request.QueryString["Advise"].Trim(), 100) + "%";
                txtQAdvise.Text = qData.Advise.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["IsOpen"]))
            {
                qData.IsOpen = HelperMain.SqlFilter(Request.QueryString["IsOpen"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQIsOpen, qData.IsOpen);
            }
            return qData;
        }
        //加载列表
        private void listData(DataOpinionPop qData, string strOrder, Repeater rpList, Literal ltNo, Label lblNav = null, Literal ltTotal = null, Page page = null)
        {
            int pageCur = 1;
            if (lblNav != null && !string.IsNullOrEmpty(page.Request.QueryString["page"]))
            {
                pageCur = Convert.ToInt32(page.Request.QueryString["page"]);
            }
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            DataOpinionPop[] data = webPop.GetDatas(qData, "", pageCur, pageSize, strOrder, "total");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    switch (data[i].ActiveName)
                    {
                        case "暂存":
                            data[i].rowClass = " class='save' title='暂存'";
                            data[i].StateName = "选取";
                            break;
                        case "提交":
                            data[i].rowClass = " class='wait' title='提交'";
                            data[i].StateName = "查看";
                            break;
                        case "留存"://未录用
                            data[i].rowClass = " class='cancel' title='留存'";
                            data[i].StateName = "选取";
                            break;
                        case "退回":
                            data[i].rowClass = " class='cancel' title='退回'";
                            data[i].StateName = "选取";
                            break;
                        default://已录用
                            data[i].StateName = "查看";
                            break;
                    }
                }
                rpList.DataSource = data;
                rpList.DataBind();
                if (ltNo != null)
                {
                    ltNo.Visible = false;
                }
                if (lblNav != null)
                {
                    int pageCount = data[0].total;
                    int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                    string lnk = page.Request.Url.ToString();
                    lblNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
                }
                if (ltTotal != null)
                {
                    ltTotal.Text = data[0].total.ToString();
                }
            }
        }
        #endregion
        //
        #region 编辑
        //加载信息
        private void loadData(int Id)
        {
            hfBack.Value = PublicMod.GetBackUrl();
            WebOp webOp = new WebOp();
            PublicMod.LoadRadioButtonList(rblSubType, webOp, "社情民意类别");
            PublicMod.LoadCheckBoxList(cblLinkmanInfo, webOp, "反映人身份");
            PublicMod.LoadCheckBoxList(cblLinkmanParty, webOp, "政治面貌");
            PublicMod.LoadDropDownList(ddlActive, webOp, "社情民意录用状态");

            if (Id <= 0)
            {
                txtSubTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//DateTime.Today.Year.ToString() + "-01-01";
                if (rblSubType.Items.Count > 0)
                {
                    rblSubType.SelectedIndex = rblSubType.Items.Count - 1;
                }
                //HelperMain.SetCheckSelected(cblLinkmanInfo, "区政协委员");
                btnEdit.Text = "新增";
                return;
            }
            DataOpinionPop[] data = webPop.GetData(Id);
            if (data != null)
            {
                txtId.Text = data[0].Id.ToString();
                txtOpNum.Text = data[0].OpNum;
                txtSubTime.Text = data[0].SubTime.ToString("yyyy-MM-dd HH:mm:ss");
                HelperMain.SetRadioSelected(rblSubType, data[0].SubType);
                HelperMain.SetCheckSelected(cblIsGood, data[0].IsGood);
                HelperMain.SetRadioSelected(rblSubManType, data[0].SubManType);
                if (data[0].SubManType == "委员")
                {
                    txtSubMan.Text = data[0].SubMan;
                }
                else
                {
                    txtSubOrg.Text = data[0].SubMan;
                    txtLinkman.Text = data[0].Linkman;
                }
                HelperMain.SetCheckSelected(cblLinkmanInfo, data[0].LinkmanInfo);
                HelperMain.SetCheckSelected(cblLinkmanParty, data[0].LinkmanParty);
                txtLinkmanOrgName.Text = data[0].LinkmanOrgName;
                txtLinkmanTel.Text = data[0].LinkmanTel;
                if (!string.IsNullOrEmpty(data[0].SubMans))
                {
                    txtSubMans.Text = data[0].SubMans.Trim(',');
                }
                if (!string.IsNullOrEmpty(data[0].SubMan2))
                {
                    txtSubMan2.Text = data[0].SubMan2.Trim(',');
                }
                txtSummary.Text = data[0].Summary;
                txtBody.Text = data[0].Body;
                txtAdvise.Text = data[0].Advise;
                hfFiles.Value = data[0].Files;
                txtRemark.Text = data[0].Remark;
                HelperMain.SetDownSelected(ddlActive, data[0].ActiveName);
                txtVerifyTitle.Text = data[0].VerifyTitle;
                txtVerifyBody.Text = data[0].VerifyBody;
                txtVerifyAdvise.Text = data[0].VerifyAdvise;
                //if (!string.IsNullOrEmpty(data[0].VerifyTitle))
                //{
                //    btnEdit.Text = "修改";
                //} else {
                //    btnEdit.Text = "核稿";
                //}
                if (!string.IsNullOrEmpty(data[0].Adopt1))
                {
                    if (data[0].Adopt1.IndexOf("单篇") >= 0)
                    {
                        rblAdopt1.SelectedIndex = 0;
                    }
                    else
                    {
                        rblAdopt1.SelectedIndex = 1;
                        string strAdopt = data[0].Adopt1;
                        if (strAdopt.IndexOf("\n") > 0)
                        {
                            txtAdopt1.Text = strAdopt.Substring(0, strAdopt.IndexOf("\n"));
                            txtAdopt1_2.Text = strAdopt.Substring(strAdopt.IndexOf("\n") + 3);
                        }
                        else
                        {
                            txtAdopt1.Text = strAdopt;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(data[0].Give1))
                {
                    string[] arr = data[0].Give1.Split(',');
                    for (int i = 0; i < arr.Count(); i++)
                    {
                        if (arr[i].IndexOf("有关部门") >= 0)
                        {
                            cblGive1.Items[1].Selected = true;
                        }
                        else
                        {
                            cblGive1.Items[2].Selected = true;
                            txtGive1.Text = arr[i];
                        }
                    }
                }
                if (!string.IsNullOrEmpty(data[0].Employ1))
                {
                    if (data[0].Employ1.IndexOf("有关部门") >= 0)
                    {
                        cblEmploy1.Items[0].Selected = true;
                    }
                    if (data[0].Employ1.IndexOf("批示") >= 0)
                    {
                        cblEmploy1.Items[1].Selected = true;
                    }
                }
                if (!string.IsNullOrEmpty(data[0].Reply1))
                {
                    cblEmploy1.Items[1].Selected = true;
                    txtReply1.Text = data[0].Reply1;
                }
                if (!string.IsNullOrEmpty(data[0].Adopt2))
                {
                    HelperMain.SetRadioSelected(rblAdopt2, data[0].Adopt2);
                }
                if (!string.IsNullOrEmpty(data[0].Send2))
                {
                    cblGive1.Items[0].Selected = true;
                }
                if (!string.IsNullOrEmpty(data[0].Give2))
                {
                    if (data[0].Give2.IndexOf("有关部门") > 0)
                    {
                        cblGive2.Items[0].Selected = true;
                    }
                    if (data[0].Give2.IndexOf("市领导") > 0)
                    {
                        cblGive2.Items[1].Selected = true;
                    }
                }
                if (!string.IsNullOrEmpty(data[0].Employ2))
                {
                    if (data[0].Employ2.IndexOf("采用") >= 0)
                    {
                        cblEmploy2.Items[0].Selected = true;
                    }
                    else if (data[0].Employ2.IndexOf("有关部门") >= 0)
                    {
                        cblGive2.Items[0].Selected = true;
                    }
                    if (data[0].Employ2.IndexOf("批示") >= 0)
                    {
                        cblEmploy2.Items[1].Selected = true;
                    }
                    else if (data[0].Employ2.IndexOf("市领导") >= 0)
                    {
                        cblEmploy2.Items[1].Selected = true;
                    }
                }
                if (!string.IsNullOrEmpty(data[0].Reply2))
                {
                    cblEmploy2.Items[1].Selected = true;
                    txtReply2.Text = data[0].Reply2;
                }
                if (!string.IsNullOrEmpty(data[0].Send3))
                {
                    cblGive2.Items[2].Selected = true;
                }
                if (!string.IsNullOrEmpty(data[0].Give3))
                {
                    if (data[0].Give3.IndexOf("单篇采用") > 0)
                    {
                        cblGive3.Items[0].Selected = true;
                    }
                    if (data[0].Give3.IndexOf("综合采用") > 0)
                    {
                        cblGive3.Items[1].Selected = true;
                    }
                    if (data[0].Give3.IndexOf("国家有关部门") > 0)
                    {
                        cblGive3.Items[2].Selected = true;
                    }
                }
                if (!string.IsNullOrEmpty(data[0].Employ3))
                {
                    if (data[0].Employ3.IndexOf("单篇采用") > 0)
                    {
                        cblEmploy3.Items[0].Selected = true;
                    }
                    if (data[0].Employ3.IndexOf("综合采用") > 0)
                    {
                        cblEmploy3.Items[1].Selected = true;
                    }
                    if (data[0].Employ3.IndexOf("国家有关部门") > 0)
                    {
                        cblEmploy3.Items[2].Selected = true;
                    }
                }
                txtReply3.Text = data[0].Reply3;
                btnDel.Visible = true;
            }
        }
        //退回
        protected void btnBack_Click(object sender, EventArgs e)
        {
            UpdateActive("退回");
        }
        //删除数据
        protected void btnDel_Click(object sender, EventArgs e)
        {
            UpdateActive("删除");
        }
        //编辑审核
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            UpdateActive();
        }
        private void UpdateActive(string ActiveName = "")
        {
            if (myUser == null)
            {
                return;
            }
            DataOpinionPop data = new DataOpinionPop();
            data.Id = (!string.IsNullOrEmpty(txtId.Text)) ? Convert.ToInt32(txtId.Text.Trim()) : 0;
            //if (data.Id <= 0)
            //{
            //    return;
            //}
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(myUser.AdminName, 20);
            //data.OpNum = HelperMain.SqlFilter(txtOpNum.Text.Trim(), 20);
            //data.OrgName
            if (ActiveName == "删除" || ActiveName == "退回")
            {
                if (data.Id >= 0)
                {
                    if (webPop.UpdateActive(data.Id, ActiveName, strIp, strUser) <= 0)
                    {
                        data.Id = 0;
                    }
                }
            }
            else
            {
                data.SubTime = (!string.IsNullOrEmpty(txtSubTime.Text)) ? Convert.ToDateTime(txtSubTime.Text) : dtNow;
                data.OpNum = HelperMain.SqlFilter(txtOpNum.Text.Trim(), 20);
                ActiveName = HelperMain.SqlFilter(ddlActive.SelectedValue.Trim(), 20);
                data.ActiveName = ActiveName;
                //ActiveName = "修改";//btnEdit.Text;
                data.SubType = HelperMain.SqlFilter(rblSubType.SelectedValue, 20);
                //data.SubType2
                data.IsOpen = HelperMain.SqlFilter(rblIsOpen.SelectedValue, 2);
                data.IsGood = HelperMain.SqlFilter(cblIsGood.SelectedValue.Trim(), 2);
                if (data.IsOpen == "否")
                {
                    data.OpenInfo = HelperMain.SqlFilter(txtOpenInfo.Text.Trim(), 20);
                }
                data.SubManType = HelperMain.SqlFilter(rblSubManType.SelectedValue.Trim(), 8);
                if (data.SubManType == "委员")
                {
                    data.SubMan = HelperMain.SqlFilter(txtSubMan.Text.Trim(), 20);
                }
                else
                {
                    data.SubMan = HelperMain.SqlFilter(txtSubOrg.Text.Trim(), 20);
                    data.Linkman = HelperMain.SqlFilter(txtLinkman.Text.Trim(), 20);
                }
                data.LinkmanInfo = HelperMain.SqlFilter(HelperMain.GetCheckSelected(cblLinkmanInfo));
                data.LinkmanParty = HelperMain.SqlFilter(HelperMain.GetCheckSelected(cblLinkmanParty), 20);
                data.LinkmanOrgName = HelperMain.SqlFilter(txtLinkmanOrgName.Text.Trim(), 100);
                data.LinkmanTel = HelperMain.SqlFilter(txtLinkmanTel.Text.Trim(), 50);
                if (!string.IsNullOrEmpty(txtSubMans.Text))
                {
                    txtSubMans.Text = txtSubMans.Text.Replace("、", ",").Replace("，", ",").Replace("\n", ",").Replace("\r", "");
                    data.SubMans = "," + HelperMain.SqlFilter(txtSubMans.Text.Trim()) + ",";
                }
                else
                {
                    data.SubMans = "";
                }
                if (!string.IsNullOrEmpty(txtSubMan2.Text))
                {
                    txtSubMan2.Text = txtSubMan2.Text.Replace("、", ",").Replace("，", ",").Replace("\n", ",").Replace("\r", "");
                    data.SubMan2 = "," + HelperMain.SqlFilter(txtSubMan2.Text.Trim()) + ",";
                }
                else
                {
                    data.SubMan2 = "";
                }
                data.Summary = HelperMain.SqlFilter(txtSummary.Text.Trim(), 100);
                data.Body = HelperMain.SqlFilter(txtBody.Text.TrimEnd());
                data.Advise = HelperMain.SqlFilter(txtAdvise.Text.TrimEnd());
                data.Files = HelperMain.SqlFilter(hfFiles.Value.Trim('|'));
                data.Remark = HelperMain.SqlFilter(txtRemark.Text.Trim());
                data.Adopt1 = "";
                data.Give1 = "";
                data.Employ1 = "";
                data.Reply1 = "";
                data.Adopt2 = "";
                data.Send2 = "";
                data.Give2 = "";
                data.Employ2 = "";
                data.Reply2 = "";
                data.Send3 = "";
                data.Give3 = "";
                data.Employ3 = "";
                data.Reply3 = "";
                data.VerifyTitle = "";
                data.VerifyBody = "";
                data.VerifyAdvise = "";
                data.Employ1 = "";
                if (data.ActiveName == "已录用")
                {
                    data.VerifyTitle = HelperMain.SqlFilter(txtVerifyTitle.Text.Trim(), 100);
                    data.VerifyBody = HelperMain.SqlFilter(txtVerifyBody.Text.Trim(), 1500);
                    data.VerifyAdvise = HelperMain.SqlFilter(txtVerifyAdvise.Text.Trim(), 1500);
                    string strAdopt1 = "";
                    if (rblAdopt1.SelectedValue.IndexOf("单篇") >= 0)
                    {
                        strAdopt1 = "单篇，";
                        data.Adopt1 = HelperMain.SqlFilter(rblAdopt1.SelectedValue.Trim(), 20);
                    }
                    else
                    {
                        strAdopt1 = "综合，";
                        string strAdopt = HelperMain.SqlFilter(txtAdopt1.Text.Trim(), 100);
                        string strAdopt2 = HelperMain.SqlFilter(txtAdopt1_2.Text.Trim(), 100);
                        if (!string.IsNullOrEmpty(strAdopt) && !string.IsNullOrEmpty(strAdopt2))
                        {
                            strAdopt += "\n" + strAdopt2;
                        }
                        data.Adopt1 = strAdopt;
                    }
                    string strGive1 = HelperMain.SqlFilter(HelperMain.GetCheckSelected(cblGive1));
                    if (strGive1.IndexOf("有关部门") >= 0)
                    {
                        data.Give1 = strAdopt1 + "报送区领导及有关部门";
                    }
                    if (!string.IsNullOrEmpty(txtGive1.Text))
                    {
                        if (!string.IsNullOrEmpty(data.Give1))
                        {
                            data.Give1 += ",";
                        }
                        data.Give1 += HelperMain.SqlFilter(txtGive1.Text.Trim());
                    }
                    data.Employ1 = HelperMain.SqlFilter(HelperMain.GetCheckSelected(cblEmploy1));//区采用
                    data.Reply1 = HelperMain.SqlFilter(txtReply1.Text.Trim());//区领导批示
                    if (strGive1.IndexOf("市政协") >= 0)
                    {
                        data.Send2 = strAdopt1 + "报送市政协";
                        data.Adopt2 = HelperMain.SqlFilter(rblAdopt2.SelectedValue.Trim(), 20);//单篇、综合
                        string strAdopt2 = "";
                        if (!string.IsNullOrEmpty(data.Adopt2))
                        {
                            strAdopt2 = data.Adopt2 + "，";
                        }
                        string strGive2 = HelperMain.SqlFilter(HelperMain.GetCheckSelected(cblGive2));
                        if (!string.IsNullOrEmpty(strGive2))
                        {
                            if (strGive2.IndexOf("全国政协") >= 0)
                            {
                                data.Send3 = strAdopt2 + "报送全国政协";
                            }
                            else
                            {
                                data.Give2 = strAdopt2 + strGive2;//市政协报送
                            }
                        }
                        string strEmploy2 = HelperMain.SqlFilter(HelperMain.GetCheckSelected(cblEmploy2));
                        if (!string.IsNullOrEmpty(strEmploy2))
                        {
                            data.Employ2 = strAdopt2 + strEmploy2;//市政协采用
                        }
                    }
                    data.Reply2 = HelperMain.SqlFilter(txtReply2.Text.Trim());//市领导批示
                    data.Give3 = HelperMain.SqlFilter(HelperMain.GetCheckSelected(cblGive3));//全国政协报送
                    data.Employ3 = HelperMain.SqlFilter(HelperMain.GetCheckSelected(cblEmploy3));//全国政协采用
                    data.Reply3 = HelperMain.SqlFilter(txtReply3.Text.Trim());//中央领导批示
                }
                string strSubMans = data.SubMan;
                if (!string.IsNullOrEmpty(data.SubMans))
                {
                    strSubMans += "," + data.SubMans.Trim(',');
                }
                string strParty = "";
                string strCommittee = "";
                string strSubsector = "";
                string strStreetTeam = "";
                string[] arr = strSubMans.Split(',');
                WebUser webUser = new WebUser();
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(arr[i]))
                    {
                        DataUser[] uData2 = webUser.GetDatas(config.PERIOD, arr[i], "Id,Party,Committee,Committee2,Subsector,StreetTeam");
                        if (uData2 != null)
                        {
                            if (!string.IsNullOrEmpty(uData2[0].Party) && (strParty + ",").IndexOf(uData2[0].Party) < 0)
                            {
                                strParty += "," + uData2[0].Party;
                            }
                            if (!string.IsNullOrEmpty(uData2[0].Committee) && (strCommittee + ",").IndexOf(uData2[0].Committee) < 0)
                            {
                                strCommittee += "," + uData2[0].Committee;
                            }
                            if (!string.IsNullOrEmpty(uData2[0].Committee2) && (strCommittee + ",").IndexOf(uData2[0].Committee2) < 0)
                            {
                                strCommittee += "," + uData2[0].Committee2;
                            }
                            if (!string.IsNullOrEmpty(uData2[0].Subsector) && (strSubsector + ",").IndexOf(uData2[0].Subsector) < 0)
                            {
                                strSubsector += "," + uData2[0].Subsector;
                            }
                            if (!string.IsNullOrEmpty(uData2[0].StreetTeam) && (strStreetTeam + ",").IndexOf(uData2[0].StreetTeam) < 0)
                            {
                                strStreetTeam += "," + uData2[0].StreetTeam;
                            }
                        }
                    }
                }
                data.Party = strParty.Trim(',');
                data.Committee = strCommittee.Trim(',');
                data.Subsector = strSubsector.Trim(',');
                data.StreetTeam = strStreetTeam.Trim(',');
                if (data.Id <= 0)
                {
                    DataOpinionPop qData = new DataOpinionPop();
                    qData.Summary = data.Summary;
                    qData.SubMan1 = data.SubMan;
                    DataOpinionPop[] ckData = webPop.GetDatas(qData, "Id");//检查是否已添加
                    if (ckData == null)
                    {
                        if (!string.IsNullOrEmpty(data.SubMan))
                        {
                            DataUser[] uData = webUser.GetDatas(config.PERIOD, data.SubMan, "Id");
                            if (uData != null)
                            {
                                data.UserId = uData[0].Id;
                            }
                        }
                        data.AddTime = dtNow;
                        data.AddIp = strIp;
                        data.AddUser = strUser;
                        data.Id = webPop.Insert(data);
                        ActiveName = "新增";
                    }
                }
                else
                {
                    DataOpinionPop[] ckData = webPop.GetData(data.Id, "VerifyTitle");
                    //if (btnEdit.Text == "修改")
                    if (!string.IsNullOrEmpty(ckData[0].VerifyTitle))
                    {
                        data.EditTime = dtNow;
                        data.EditIp = strIp;
                        data.EditUser = strUser;
                    }
                    else
                    {
                        data.VerifyTime = dtNow;
                        data.VerifyIp = strIp;
                        data.VerifyUser = strUser;
                    }
                    data.UserId = -1;
                    if (webPop.Update(data) <= 0)
                    {
                        data.Id = -1;
                    }
                }
            }
            if (data.Id > 0)
            {
                string strBack = hfBack.Value;
                ltInfo.Text = "<script>$(function(){ alert('“" + ActiveName + "社情民意”成功！'); window.location.href='" + strBack + "'; });</script>";
                if (!string.IsNullOrEmpty(data.ActiveName))
                {
                    cancelScore(data, strIp, strUser);//取消积分
                    if (data.ActiveName == "已录用" || data.ActiveName == "留存")
                    {
                        AddScore(data, strIp, strUser);//增加积分
                    }
                }
            }
            else if (data.Id < 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ActiveName + "社情民意”失败！'); window.history.back(-1); });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“社情民意”的标题重复了！'); window.history.back(-1); });</script>";
            }
        }
        //增加积分
        public void AddScore(DataOpinionPop data, string strIp, string strUser)
        {
            string TitleGood = "";
            decimal ScoreGood = 0;

            string TitleSub = "";
            decimal ScoreSub = 0;

            string TitleEmploy1 = "";
            decimal ScoreEmploy1 = 0;
            string TitleReply1 = "";
            decimal ScoreReply1 = 0;

            string TitleGive2 = "";
            decimal ScoreGive2 = 0;
            string TitleEmploy2 = "";
            decimal ScoreEmploy2 = 0;

            string TitleGive3 = "";
            decimal ScoreGive3 = 0;
            string TitleEmploy3 = "";
            decimal ScoreEmploy3 = 0;

            WebScore webScore = new WebScore();
            if (data.IsGood == "是")
            {
                TitleGood = "优秀社情民意";
                DataScore[] sData = webScore.GetDatas(0, "社情民意", "", TitleGood, "score");
                if (sData != null)
                {
                    ScoreGood = sData[0].Score;
                }
            }
            if (data.ActiveName == "已录用" || data.ActiveName == "留存")
            {
                TitleSub = "委员提交社情民意";
                DataScore[] sData = webScore.GetDatas(0, "社情民意", "", TitleSub, "score");
                if (sData != null)
                {
                    ScoreSub = sData[0].Score;//委员单独提交、单篇采用
                    if (!string.IsNullOrEmpty(data.SubMans) && !string.IsNullOrEmpty(data.Adopt1) && data.Adopt1.IndexOf("单篇") < 0)
                    {//联名提交且综合采用，得分按1/4计。2020.4.3改为得分按1/2计
                        TitleSub += "-联名提交且综合采用";
                        ScoreSub = ScoreSub / 2;
                    }
                    else if (!string.IsNullOrEmpty(data.SubMans))
                    {//联名提交，得分按1/2计
                        TitleSub += "-联名提交";
                        ScoreSub = ScoreSub / 2;
                    }
                    else if (!string.IsNullOrEmpty(data.Adopt1) && data.Adopt1.IndexOf("单篇") < 0)
                    {//综合采用，得分按1/2计。2020.4.3改为综合采用同单编采用
                        TitleSub += "-综合采用";
                        //ScoreSub = ScoreSub / 2;
                    }
                }
            }

            if (!string.IsNullOrEmpty(data.Reply1))
            {
                TitleReply1 = "得到区领导批示";
                DataScore[] sData = webScore.GetDatas(0, "社情民意", "", TitleReply1, "score");
                if (sData != null)
                {
                    ScoreReply1 = sData[0].Score;//委员单独提交、单篇采用
                    if (!string.IsNullOrEmpty(data.SubMans) && data.Adopt1.IndexOf("单篇") < 0)
                    {//联名提交且综合采用，得分按1/4计
                        TitleReply1 += "-联名提交且综合采用";
                        ScoreReply1 = ScoreReply1 / 4;
                    }
                    else if (!string.IsNullOrEmpty(data.SubMans))
                    {//联名提交或综合采用，得分按1/2计
                        TitleReply1 += "-联名提交";
                        ScoreReply1 = ScoreReply1 / 2;
                    }
                    else if (data.Adopt1.IndexOf("单篇") < 0)
                    {//联名提交或综合采用，得分按1/2计
                        TitleReply1 += "-综合采用";
                        ScoreReply1 = ScoreReply1 / 2;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(data.Employ1))// && !string.IsNullOrEmpty(data.Give1)
            {
                TitleEmploy1 = "得到区有关部门采用";
                DataScore[] sData = webScore.GetDatas(0, "社情民意", "", TitleEmploy1, "score");
                if (sData != null)
                {
                    ScoreEmploy1 = sData[0].Score;//委员单独提交、单篇采用
                    if (!string.IsNullOrEmpty(data.SubMans) && data.Adopt1.IndexOf("单篇") < 0)
                    {//联名提交且综合采用，得分按1/4计
                        TitleEmploy1 += "-联名提交且综合采用";
                        ScoreEmploy1 = ScoreEmploy1 / 4;
                    }
                    else if (!string.IsNullOrEmpty(data.SubMans))
                    {//联名提交或综合采用，得分按1/2计
                        TitleEmploy1 += "-联名提交";
                        ScoreEmploy1 = ScoreEmploy1 / 2;
                    }
                    else if (data.Adopt1.IndexOf("单篇") < 0)
                    {//联名提交或综合采用，得分按1/2计
                        TitleEmploy1 += "-综合采用";
                        ScoreEmploy1 = ScoreEmploy1 / 2;
                    }
                }
            }

            if (!string.IsNullOrEmpty(data.Adopt2))
            {
                string strAdopt = "";
                if (data.Adopt2.IndexOf("单篇") >= 0)
                {//单篇报送市政协
                    strAdopt = "单篇";
                }
                else if (data.Adopt2.IndexOf("综合") >= 0)
                {//单篇报送市政协
                    strAdopt = "综合";
                }
                string strType = "市政协" + strAdopt + "采用";
                if (data.Give2.IndexOf("市有关部门") >= 0)
                {
                    TitleGive2 = strType + "-转送市有关部门";
                    DataScore[] sData = webScore.GetDatas(0, "社情民意", "", TitleGive2, "score");
                    if (sData != null)
                    {
                        ScoreGive2 = sData[0].Score;
                    }
                    if (data.Employ2.IndexOf("采用") >= 0)
                    {
                        TitleEmploy2 = strType + "-得到市有关部门采用";
                        DataScore[] sData2 = webScore.GetDatas(0, "社情民意", "", TitleEmploy2, "score");
                        if (sData2 != null)
                        {
                            ScoreEmploy2 = sData2[0].Score;
                        }
                    }
                }
                else if (data.Give2.IndexOf("市领导") >= 0)
                {
                    TitleGive2 = strType + "-报送市领导";
                    DataScore[] sData = webScore.GetDatas(0, "社情民意", "", TitleGive2, "score");
                    if (sData != null)
                    {
                        ScoreGive2 = sData[0].Score;
                    }
                    if (data.Employ2.IndexOf("批示") >= 0)
                    {
                        TitleEmploy2 = strType + "-得到市领导批示";
                        DataScore[] sData2 = webScore.GetDatas(0, "社情民意", "", TitleEmploy2, "score");
                        if (sData2 != null)
                        {
                            ScoreEmploy2 = sData2[0].Score;
                        }
                    }
                }
                else if (data.Send3.IndexOf("全国政协") >= 0)
                {
                    TitleGive2 = strType + "-报送全国政协";
                    DataScore[] sData = webScore.GetDatas(0, "社情民意", "", TitleGive2, "score");
                    if (sData != null)
                    {
                        ScoreGive2 = sData[0].Score;
                    }
                }

                if (!string.IsNullOrEmpty(data.Give3))
                {
                    string strType2 = strType + "报送全国政协";
                    if (data.Give3.IndexOf("单篇采用") >= 0)
                    {
                        TitleGive3 = strType2 + "-全国政协单篇采用";
                        DataScore[] sData = webScore.GetDatas(0, "社情民意", "", TitleGive3, "score");
                        if (sData != null)
                        {
                            ScoreGive3 = sData[0].Score;
                        }
                        if (data.Employ3.IndexOf("批示") >= 0)
                        {
                            TitleEmploy3 = TitleGive3 + "得到中央领导批示";
                            DataScore[] sData2 = webScore.GetDatas(0, "社情民意", "", TitleEmploy3, "score");
                            if (sData2 != null)
                            {
                                ScoreEmploy3 = sData2[0].Score;
                            }
                        }
                    }
                    else if (data.Give3.IndexOf("综合采用") >= 0)
                    {
                        TitleGive3 = strType2 + "-全国政协综合采用";
                        DataScore[] sData = webScore.GetDatas(0, "社情民意", "", TitleGive3, "score");
                        if (sData != null)
                        {
                            ScoreGive3 = sData[0].Score;
                        }
                        if (data.Employ3.IndexOf("批示") >= 0)
                        {
                            TitleEmploy3 = TitleGive3 + "得到中央领导批示";
                            DataScore[] sData2 = webScore.GetDatas(0, "社情民意", "", TitleEmploy3, "score");
                            if (sData2 != null)
                            {
                                ScoreEmploy3 = sData2[0].Score;
                            }
                        }
                    }
                    else if (data.Give3.IndexOf("有关部门") >= 0)
                    {
                        TitleGive3 = strType2 + "-全国政协转送国家有关部门";
                        DataScore[] sData = webScore.GetDatas(0, "社情民意", "", TitleGive3, "score");
                        if (sData != null)
                        {
                            ScoreGive3 = sData[0].Score;
                        }
                        if (data.Employ3.IndexOf("采用") >= 0)
                        {
                            TitleEmploy3 = strType2 + "得到国家有关部门采用";
                            DataScore[] sData2 = webScore.GetDatas(0, "社情民意", "", TitleEmploy3, "score");
                            if (sData2 != null)
                            {
                                ScoreEmploy3 = sData2[0].Score;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(data.SubMans))
                {//联名提交
                    if (!string.IsNullOrEmpty(TitleGive2))
                    {
                        TitleGive2 += "-联名提交";
                        ScoreGive2 = ScoreGive2 / 2;
                    }
                    if (!string.IsNullOrEmpty(TitleEmploy2))
                    {
                        TitleEmploy2 += "-联名提交";
                        ScoreEmploy2 = ScoreEmploy2 / 2;
                    }
                    if (!string.IsNullOrEmpty(TitleGive3))
                    {
                        TitleGive3 += "-联名提交";
                        ScoreGive3 = ScoreGive3 / 2;
                    }
                    if (!string.IsNullOrEmpty(TitleEmploy3))
                    {
                        TitleEmploy3 += "-联名提交";
                        ScoreEmploy3 = ScoreEmploy3 / 2;
                    }
                }
                //else
                //{

                //}
            }
            string TableName = "tb_Opinion_Pop";
            string strSubMans = data.SubMan;
            if (!string.IsNullOrEmpty(data.SubMans))
            {
                string[] tmp = data.SubMans.Trim(',').Split(',');
                for (int i = 0; i < tmp.Count(); i++)
                {
                    if (tmp[i] != data.SubMan)
                    {
                        strSubMans += "," + tmp[i];
                    }
                }
                //strSubMans += "," + data.SubMans;
            }
            string[] arr = strSubMans.Split(',');
            DateTime dtTime = (data.SubTime > DateTime.MinValue) ? data.SubTime : DateTime.Now;
            WebUser webUser = new WebUser();
            for (int i = 0; i < arr.Count(); i++)
            {
                if (!string.IsNullOrEmpty(arr[i]))
                {
                    DataUser[] uData2 = webUser.GetDatas(config.PERIOD, arr[i], "Id");
                    if (uData2 != null)
                    {
                        if (!string.IsNullOrEmpty(TitleGood))
                        {
                            PublicMod.AddScore(uData2[0].Id, TitleGood, ScoreGood, TableName, data.Id, strIp, strUser, dtTime);
                        }
                        if (!string.IsNullOrEmpty(TitleSub))
                        {
                            PublicMod.AddScore(uData2[0].Id, TitleSub, ScoreSub, TableName, data.Id, strIp, strUser, dtTime);
                        }
                        if (!string.IsNullOrEmpty(TitleEmploy1))
                        {
                            PublicMod.AddScore(uData2[0].Id, TitleEmploy1, ScoreEmploy1, TableName, data.Id, strIp, strUser, dtTime);
                        }
                        if (!string.IsNullOrEmpty(TitleReply1))
                        {
                            PublicMod.AddScore(uData2[0].Id, TitleReply1, ScoreReply1, TableName, data.Id, strIp, strUser, dtTime);
                        }
                        if (!string.IsNullOrEmpty(TitleGive2))
                        {
                            PublicMod.AddScore(uData2[0].Id, TitleGive2, ScoreGive2, TableName, data.Id, strIp, strUser, dtTime);
                        }
                        if (!string.IsNullOrEmpty(TitleEmploy2))
                        {
                            PublicMod.AddScore(uData2[0].Id, TitleEmploy2, ScoreEmploy2, TableName, data.Id, strIp, strUser, dtTime);
                        }
                        if (!string.IsNullOrEmpty(TitleGive3))
                        {
                            PublicMod.AddScore(uData2[0].Id, TitleGive3, ScoreGive3, TableName, data.Id, strIp, strUser, dtTime);
                        }
                        if (!string.IsNullOrEmpty(TitleEmploy3))
                        {
                            PublicMod.AddScore(uData2[0].Id, TitleEmploy3, ScoreEmploy3, TableName, data.Id, strIp, strUser, dtTime);
                        }
                    }
                }
            }
            //
        }
        //取消积分
        private void cancelScore(DataOpinionPop data, string strIp, string strUser)
        {
            string TableName = "tb_Opinion_Pop";
            WebUserScore webScore = new WebUserScore();
            DataUserScore[] sData = webScore.GetDatas(0, "", TableName, data.Id, "", "", "Id");
            if (sData != null)
            {
                ArrayList arrList = new ArrayList();
                for (int i = 0; i < sData.Count(); i++)
                {
                    arrList.Add(sData[i].Id);
                }
                if (arrList.Count > 0)
                {
                    webScore.UpdateActive(arrList, -1, strIp, strUser);//用户积分状态设为<0
                }
            }
        }
        #endregion
        //
        #region 导出
        //导出核稿单
        private void outVerify(int Id)
        {
            if (Id <= 0)
            {
                return;
            }
            DataOpinionPop[] data = webPop.GetData(Id);
            if (data != null)
            {
                DateTime dtNow = DateTime.Now;
                string virtualPath = string.Format("../download/{0:yyyy}/{0:MM}/", dtNow);
                string filepath = HttpContext.Current.Server.MapPath(virtualPath);
                if (!System.IO.Directory.Exists(filepath))
                {
                    System.IO.Directory.CreateDirectory(filepath);
                }
                string strFileName = string.Format("社情民意_核稿单{0}_{1:yyyyMMddHHmmss}.doc", Id, dtNow);
                string fileName = virtualPath + "/" + strFileName;

                Object Nothing = System.Reflection.Missing.Value;
                //创建Word文档
                Microsoft.Office.Interop.Word.Application WordApp = new Microsoft.Office.Interop.Word.Application();
                Microsoft.Office.Interop.Word.Document WordDoc = WordApp.Documents.Add(ref Nothing, ref Nothing, ref Nothing, ref Nothing);

                WordDoc.Paragraphs.Add(ref Nothing);//增加一段
                WordDoc.Paragraphs.Last.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;//设置居中
                //WordDoc.Paragraphs.Last.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;//设置左对齐
                //WordDoc.Paragraphs.Last.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;//设置右对齐
                WordDoc.Paragraphs.Last.Range.Text = string.Format("社情民意信息核稿单（{0:yyyy}）", dtNow);

                int intRow = (data[0].SubManType == "委员") ? 8 : 9;
                int intCol = 8;

                //文档中创建表格
                Microsoft.Office.Interop.Word.Table newTable = WordDoc.Tables.Add(WordApp.Selection.Range, intRow, intCol, ref Nothing, ref Nothing);
                //设置表格样式
                newTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleThickThinLargeGap;
                newTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                newTable.Columns[1].Width = 100f;
                newTable.Columns[2].Width = 220f;
                newTable.Columns[3].Width = 105f;

                WebUser webUser = new WebUser();
                int num = 0;
                string strSubMan = "";
                string strOrgName = "";
                string strTel = "";
                string strSubsector = "";
                string strCommittee = "";
                string strParty = "";
                if (data[0].SubManType == "委员")
                {
                    strSubMan = data[0].SubMan;
                    strOrgName = data[0].LinkmanOrgName;
                    strTel = data[0].LinkmanTel;
                    strSubsector = data[0].Subsector;
                    strCommittee = data[0].Committee;
                    strParty = data[0].Party;
                }
                else
                {
                    num++;
                    newTable.Cell(num, 1).Range.Text = "反映单位";//填充表格内容
                    newTable.Cell(num, 2).Range.Text = data[0].SubMan;
                    newTable.Cell(num, 2).Merge(newTable.Cell(num, intCol));//合并单元格
                    strSubMan = data[0].Linkman;
                    strOrgName = data[0].LinkmanOrgName;
                    strTel = data[0].LinkmanTel;
                    strSubsector = "";
                    strCommittee = "";
                    strParty = data[0].LinkmanParty;
                }
                string strSubMans = "";
                string strOrgNames = "";
                string strTels = "";
                string strSubsectors = "";
                string strCommittees = "";
                string strPartys = "";
                if (!string.IsNullOrEmpty(data[0].SubMans))
                {
                    string[] arr = data[0].SubMans.Split(',');
                    for (int i = 0; i < arr.Count(); i++)
                    {
                        if (!string.IsNullOrEmpty(arr[i]))
                        {
                            strSubMans += "、" + arr[i];
                        }
                    }
                    strSubMans = strSubMans.Trim('、');
                }
                num++;
                newTable.Cell(num, 1).Range.Text = "反映人";//填充表格内容
                newTable.Cell(num, 2).Range.Text = strSubMan;
                newTable.Cell(num, 3).Range.Text = "单位职务";//填充表格内容
                newTable.Cell(num, 4).Range.Text = strOrgName;
                newTable.Cell(num, 4).Merge(newTable.Cell(num, 6));//合并单元格
                newTable.Cell(num, 7).Range.Text = "联系电话";//填充表格内容
                newTable.Cell(num, 8).Range.Text = strTel;
                num++;
                newTable.Cell(num, 1).Range.Text = "界别";//填充表格内容
                newTable.Cell(num, 2).Range.Text = strSubsector;
                newTable.Cell(num, 3).Range.Text = "专委会";//填充表格内容
                newTable.Cell(num, 4).Range.Text = strCommittee;
                newTable.Cell(num, 4).Merge(newTable.Cell(num, 6));//合并单元格
                newTable.Cell(num, 7).Range.Text = "政治面貌";//填充表格内容
                newTable.Cell(num, 8).Range.Text = strParty;
                num++;
                newTable.Cell(num, 1).Range.Text = "联名人";//填充表格内容
                newTable.Cell(num, 2).Range.Text = strSubMans;
                newTable.Cell(num, 3).Range.Text = "单位职务";//填充表格内容
                newTable.Cell(num, 4).Range.Text = strOrgNames;
                newTable.Cell(num, 4).Merge(newTable.Cell(num, 6));//合并单元格
                newTable.Cell(num, 7).Range.Text = "联系电话";//填充表格内容
                newTable.Cell(num, 8).Range.Text = strTels;
                num++;
                newTable.Cell(num, 1).Range.Text = "界别";//填充表格内容
                newTable.Cell(num, 2).Range.Text = strSubsectors;
                newTable.Cell(num, 3).Range.Text = "专委会";//填充表格内容
                newTable.Cell(num, 4).Range.Text = strCommittees;
                newTable.Cell(num, 4).Merge(newTable.Cell(num, 6));//合并单元格
                newTable.Cell(num, 7).Range.Text = "政治面貌";//填充表格内容
                newTable.Cell(num, 8).Range.Text = strPartys;
                num++;
                newTable.Cell(num, 1).Range.Text = "采用：\n(请打√)";//填充表格内容
                newTable.Cell(num, 2).Range.Text = string.Format("（）单篇\n（）综合，原始篇目：(1)；\n　　　　　　　　　　(2)。");
                newTable.Cell(num, 2).Merge(newTable.Cell(num, intCol));//合并单元格
                num++;
                newTable.Cell(num, 1).Range.Text = "主送：\n(请打√)";//填充表格内容
                newTable.Cell(num, 2).Range.Text = string.Format("（）报送市政协综合信息处\n（）报送区委、区政府、区政协领导，区委办、区政府办及有关部门\n（）转送区委/办/局");
                newTable.Cell(num, 2).Merge(newTable.Cell(num, intCol));//合并单元格
                num++;
                newTable.Cell(num, 1).Range.Text = "编辑：";//填充表格内容
                newTable.Cell(num, 2).Range.Text = string.Format("（月日）");
                newTable.Cell(num, 2).Merge(newTable.Cell(num, 4));//合并单元格
                WordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;//设置右对齐
                newTable.Cell(num, 5).Range.Text = "核稿：";//填充表格内容
                newTable.Cell(num, 6).Range.Text = string.Format("（月日）");
                newTable.Cell(num, 6).Merge(newTable.Cell(num, 8));//合并单元格
                WordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;//设置右对齐
                num++;
                newTable.Cell(num, 1).Range.Text = string.Format("标题：\n\n反映的问题（分析）：\n\n\n建议：\n\n\n（正文篇幅通常不超过1500字）");//填充表格内容
                newTable.Cell(num, 2).Merge(newTable.Cell(num, intCol));//合并单元格

                //文件保存
                object objWordName = Server.MapPath(fileName);
                WordDoc.SaveAs(ref objWordName, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing);
                //WordDoc.Close(ref Nothing, ref Nothing, ref Nothing);
                //WordApp.Quit(ref Nothing, ref Nothing, ref Nothing);
                
                Response.Redirect(fileName);
            }
        }
        //导出清单
        protected void btnOutput_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                ltInfo.Text = "<script>$(function(){ alert('请重新登录！'); });</script>";
                return;
            }
            DataOpinionPop qData = getData();
            int pageCur = 1;
            int pageSize = 0;
            string strOrderBy = HelperMain.SqlFilter(Request.QueryString["Order"], 20) + " " + HelperMain.SqlFilter(Request.QueryString["By"], 4);
            string strFields = HelperMain.GetCheckSelected(cblOutput);
            if (string.IsNullOrEmpty(strFields))
            {
                return;
            }
            string strJoin = "";
            if (strFields.IndexOf("UserCode") >= 0)
            {
                strJoin += " LEFT JOIN tb_User AS u ON (u.Id=o.UserId)";
            }
            string[] arrFields = strFields.Split(',');
            string[] tmpFields = new string[arrFields.Count()];
            for (int i = 0; i < arrFields.Count(); i++)
            {
                switch (arrFields[i])
                {
                    case "SubMan":
                        tmpFields[i] = "o." + arrFields[i] + ",o.Linkman";
                        break;
                    case "UserCode":
                        tmpFields[i] = "u." + arrFields[i];
                        break;
                    case "ActiveName":
                        tmpFields[i] = "o." + arrFields[i] + ",o.Adopt1,o.Give1,o.Employ1,o.Reply1,o.Adopt2,o.Send2,o.Give2,o.Employ2,o.Reply2,o.Send3,o.Give3,o.Employ3,o.Reply3";
                        break;
                    default:
                        tmpFields[i] = "o." + arrFields[i];
                        break;
                }
            }
            string sqlFields = string.Join(",", tmpFields);
            DataOpinionPop[] data = webPop.GetDatas(qData, sqlFields, pageCur, pageSize, strOrderBy, "total", strJoin);
            if (data == null)
            {
                return;
            }
            DateTime dtNow = DateTime.Now;
            string virtualPath = string.Format("../download/{0:yyyy}/{0:MM}/", dtNow);
            string filepath = HttpContext.Current.Server.MapPath(virtualPath);
            if (!System.IO.Directory.Exists(filepath))
            {
                System.IO.Directory.CreateDirectory(filepath);
            }
            //首先初始化excel object
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            //在创建excel workbook之前，检查系统是否安装excel
            if (excelApp == null)
            {
                // if equal null means EXCEL is not installed.  
                //MessageBox.Show("Excel is not properly installed!");
                return;
            }
            //判断文件是否存在，如果存在就打开workbook，如果不存在就新建一个
            Microsoft.Office.Interop.Excel.Workbook workBook;
            string strFileName = string.Format("社情民意_{0:yyyyMMddHHmmss}.xls", dtNow);
            string fileName = virtualPath + "/" + strFileName;
            string path = Server.MapPath(fileName);
            if (System.IO.File.Exists(path))
            {
                workBook = excelApp.Application.Workbooks.Open(path, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                //workBook = excelApp.Application.Workbooks.Add(true);
            }
            else
            {
                workBook = excelApp.Application.Workbooks.Add(true);
            }
            //在创建完workbook之后，下一步就是新建worksheet并写入数据
            Microsoft.Office.Interop.Excel.Worksheet workSheet = workBook.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;
            workSheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.Worksheets.get_Item(1);//获得第i个sheet，准备写入
            //workSheet.Name = strFileName.Replace(".xls", "");//第1个表";
            workSheet.Cells[1, 1] = string.Format("下载时间：{0:yyyy-MM-dd HH:mm:ss}，下载人：{1}", dtNow, myUser.AdminName);
            //workSheet.Rows["3:6"].RowHeight = 20;
            workSheet.Range[workSheet.Cells[3, 1], workSheet.Cells[3, arrFields.Count()]].MergeCells = true;//合并单元格
            workSheet.Cells[3, 1].Font.Size = 16;//设置字体大小
            PublicMod.SetCells(workSheet, 3, 1, "社情民意", "center,bold");
            int[] intLen = new int[arrFields.Count()];
            workSheet.Rows[4].RowHeight = 20;
            for (int i = 0; i < arrFields.Count(); i++)
            {
                string strTitle = "";
                for (int j = 0; j < cblOutput.Items.Count; j++)
                {
                    if (cblOutput.Items[j].Value == arrFields[i])
                    {
                        strTitle = cblOutput.Items[j].Text;
                        intLen[i] = strTitle.Length;
                        break;
                    }
                }
                PublicMod.SetCells(workSheet, 4, i + 1, strTitle, "fit,center,bold,border", "LightGray");
                if (strTitle == "标题" || strTitle == "反馈结果" || strTitle == "内容" || strTitle == "备注")
                {
                    workSheet.Columns[i + 1].ColumnWidth = 30;
                }
            }
            for (int i = 0; i < data.Count(); i++)
            {
                workSheet.Rows[i + 5].RowHeight = 20;
                for (int j = 0; j < arrFields.Count(); j++)
                {
                    string strValue = "";
                    string attr = "";
                    switch (arrFields[j])
                    {
                        case "Id"://流水号
                            strValue = data[i].Id.ToString();
                            attr = "txt,center,border";
                            break;
                        case "SubMan"://反映人
                            if (!string.IsNullOrEmpty(data[i].SubMan))
                            {
                                strValue = data[i].SubMan;
                                attr = "txt,center,border";
                                if (!string.IsNullOrEmpty(data[i].Linkman) && data[i].Linkman != data[i].SubMan)
                                {
                                    workSheet.Rows[i + 5].RowHeight = 30;
                                    workSheet.Columns[j + 1].ColumnWidth = 2 * strValue.Length;
                                    strValue += "\n" + data[i].Linkman;
                                }
                                else if (strValue.Length > intLen[j])
                                {
                                    attr += ",fit";
                                    intLen[j] = strValue.Length;
                                }
                            }
                            break;
                        case "SubType"://信息类别
                            strValue = data[i].SubType;
                            attr = "txt,border";
                            if (strValue.Length > intLen[j])
                            {
                                attr += ",fit";
                                intLen[j] = strValue.Length;
                            }
                            break;
                        case "Summary"://标题
                            if (!string.IsNullOrEmpty(data[i].Summary))
                            {
                                strValue = data[i].Summary;
                                attr = "txt,wrap,border";
                                if (strValue.Length > 15)
                                {
                                    int row = strValue.Length / 15;
                                    if (strValue.Length % 15 > 0)
                                    {
                                        row++;
                                    }
                                    workSheet.Rows[i + 5].RowHeight = 15 * row;
                                }
                            }
                            break;
                        case "SubTime"://提交时间
                            strValue = data[i].SubTime.ToString("yyyy/MM/dd HH:mm:ss");
                            attr = "txt,center,border,fit";
                            break;
                        case "LinkmanOrgName"://工作单位与职位
                            strValue = data[i].LinkmanOrgName;
                            attr = "txt,border";
                            if (strValue.Length > intLen[j])
                            {
                                attr += ",fit";
                                intLen[j] = strValue.Length;
                            }
                            break;
                        case "LinkmanTel"://联系方式
                            strValue = data[i].LinkmanTel;
                            attr = "txt,border";
                            if (strValue.Length > intLen[j])
                            {
                                attr += ",fit";
                                intLen[j] = strValue.Length;
                            }
                            break;
                        case "LinkmanInfo"://委员类型
                            strValue = data[i].LinkmanInfo;
                            attr = "txt,border";
                            if (strValue.Length > intLen[j])
                            {
                                attr += ",fit";
                                intLen[j] = strValue.Length;
                            }
                            break;
                        case "UserCode"://委员证号
                            strValue = data[i].UserCode;
                            attr = "txt,center,border";
                            break;
                        case "OrgName"://来稿单位
                            strValue = data[i].OrgName;
                            attr = "txt,border";
                            if (strValue.Length > intLen[j])
                            {
                                attr += ",fit";
                                intLen[j] = strValue.Length;
                            }
                            break;
                        case "VerifyUser"://签发人
                            strValue = data[i].VerifyUser;
                            attr = "txt,center,border";
                            break;
                        case "UpTime"://编辑时间
                            strValue = data[i].UpTime.ToString("yyyy/MM/dd HH:mm:ss");
                            attr = "txt,center,border,fit";
                            break;
                        case "SubMans"://联名委员
                            if (!string.IsNullOrEmpty(data[i].SubMans))
                            {
                                strValue = data[i].SubMans.Trim(',');
                            }
                            attr = "txt,border";
                            if (strValue.Length > intLen[j])
                            {
                                attr += ",fit";
                                intLen[j] = strValue.Length;
                            }
                            break;
                        case "IsOpen"://是否公开
                            strValue = data[i].IsOpen;
                            attr = "txt,center,border";
                            break;
                        case "LinkmanParty"://党派
                            strValue = data[i].LinkmanParty;
                            attr = "txt,border";
                            if (strValue.Length > intLen[j])
                            {
                                attr += ",fit";
                                intLen[j] = strValue.Length;
                            }
                            break;
                        case "Subsector"://界别
                            strValue = data[i].Subsector;
                            attr = "txt,border";
                            if (strValue.Length > intLen[j])
                            {
                                attr += ",fit";
                                intLen[j] = strValue.Length;
                            }
                            break;
                        case "Committee"://专委会
                            strValue = data[i].Committee;
                            attr = "txt,border";
                            if (strValue.Length > intLen[j])
                            {
                                attr += ",fit";
                                intLen[j] = strValue.Length;
                            }
                            break;
                        case "ActiveName"://反馈结果
                            if (data[i].ActiveName == "已录用")
                            {
                                strValue = PublicMod.GetPopFeed(data[i]);
                                if (!string.IsNullOrEmpty(strValue))
                                {
                                    strValue = strValue.Replace("<b>", "").Replace("</b>", "").Replace("<br/>", "\n").Trim();
                                }
                            }
                            else
                            {
                                strValue = data[i].ActiveName;
                            }
                            attr = "txt,wrap,border";
                            break;
                        case "Body"://内容
                            strValue = data[i].Body;
                            attr = "txt,wrap,border";
                            break;
                        case "Remark"://备注
                            strValue = data[i].Remark;
                            attr = "txt,wrap,border";
                            break;
                        default:
                            break;
                    }
                    PublicMod.SetCells(workSheet, i + 5, j + 1, strValue, attr);
                }
            }

            //有两个选项可以设置，如下
            excelApp.Visible = false;//visable属性设置为true的话，excel程序会启动；false的话，excel只在后台运行
            excelApp.DisplayAlerts = false;//displayalert设置为true将会显示excel中的提示信息
            //保存文件，关闭workbook
            workBook.SaveAs(path, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
            workBook.Close(false, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
            //退出并清理objects，回收内存
            excelApp.Quit();
            workSheet = null;
            workBook = null;
            excelApp = null;
            GC.Collect();
            Response.Redirect(fileName);
        }
        #endregion
        //
    }
}