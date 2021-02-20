using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mod.main;
using hkzx.db;
using hkzx.user;

namespace hkzx.web.cn
{
    public partial class report : System.Web.UI.Page
    {
        private DataUser myUser = null;
        private WebReport webReport = new WebReport();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperUser.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.TrueName))
            {
                Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.UserType = myUser.UserType;
            LoadNav(myUser.TrueName, plNav, ltSaveNum);
            string strTitle = "调研报告";
            switch (Request.QueryString["ac"])
            {
                case "query":
                    strTitle = "检索调研报告";
                    plQuery.Visible = true;
                    QueryData(myUser, rpQueryList, ltQueryNo, lblQueryNav, ltQueryTotal, this, hfOrg, ddlQOrgType, txtQOrgName, ddlQIsPoint, txtSubMan, txtSubMans, txtQTitle, txtQAddUser, txtQSubTime1, txtQSubTime2);
                    break;
                case "my":
                    strTitle = "我的调研报告";
                    plMy.Visible = true;
                    MyList("<>'删除'", myUser.TrueName, rpMyList, ltMyNo, lblMyNav, ltMyTotal, this);//我的
                    break;
                case "save":
                    strTitle = "暂存的调研报告";
                    plSave.Visible = true;
                    MyList("暂存,退回", myUser.TrueName, rpSaveList, ltSaveNo, lblSaveNav, null, this);//暂存
                    break;
                default:
                    if (!IsPostBack)
                    {
                        int Id = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                        strTitle = LoadData(Id, myUser, this, plSub, hfOrg, ddlOrgType, txtOrgName, rblIsPoint, txtSubMan, txtSubMans, txtTitle, txtBody, hfFiles, txtAddUser, btnDel, plView, ltOrgName, ltIsPoint, ltSubMan, ltSubMans, ltTitle, ltBody, ltFiles, ltAddUser, ltSubTime);
                    }
                    break;
            }
            Header.Title += " - " + strTitle;
        }
        //页面nav
        public void LoadNav(string strUser, Panel plNav2, Literal ltSaveNum2)
        {
            plNav2.Visible = true;
            DataReport qData = new DataReport();
            qData.ActiveName = "暂存,退回";
            qData.AddUser = strUser;
            DataReport[] data = webReport.GetDatas(qData, "Id");
            if (data != null)
            {
                ltSaveNum2.Text = (data.Count() > 99) ? "99+" : data.Count().ToString();
            }
        }
        //
        #region 提交
        //初始化提交表
        private void initSub(PlaceHolder plSub2, HiddenField hfOrg2, DropDownList ddlOrgType2, DataUser uData, TextBox txtAddUser2)
        {
            plSub2.Visible = true;
            PublicMod.LoadDropDownLists(hfOrg2, ddlOrgType2, "OpName", uData);
            txtAddUser2.Text = uData.TrueName;
        }
        //加载提交表
        public string LoadData(int Id, DataUser uData, Page page, PlaceHolder plSub2, HiddenField hfOrg2, DropDownList ddlOrgType2, TextBox txtOrgName2, RadioButtonList rblIsPoint2, TextBox txtSubMan2, TextBox txtSubMans2, TextBox txtTitle2, TextBox txtBody2, HiddenField hfFiles2, TextBox txtAddUser2, Button btnDel2, PlaceHolder plView2, Literal ltOrgName2, Literal ltIsPoint2, Literal ltSubMan2, Literal ltSubMans2, Literal ltTitle2, Literal ltBody2, Literal ltFiles2, Literal ltAddUser2, Literal ltSubTime2)
        {
            string strTitle = "提交调研报告";
            if (Id <= 0)
            {
                initSub(plSub2, hfOrg2, ddlOrgType2, uData, txtAddUser2);
                return strTitle;
            }
            DataReport[] data = webReport.GetData(Id);
            if (data != null)
            {
                if ((data[0].ActiveName == "暂存" || data[0].ActiveName == "退回") && data[0].AddUser == uData.TrueName)
                {
                    initSub(plSub2, hfOrg2, ddlOrgType2, uData, txtAddUser2);
                    txtOrgName2.Text = data[0].OrgName;
                    HelperMain.SetRadioSelected(rblIsPoint2, data[0].IsPoint);
                    txtSubMan2.Text = data[0].SubMan;
                    txtSubMans2.Text = data[0].SubMans;
                    txtTitle2.Text = data[0].Title;
                    txtBody2.Text = data[0].Body;
                    hfFiles2.Value = data[0].Files;

                    btnDel2.Visible = true;
                }
                else if (data[0].ActiveName == "审核通过" || data[0].ActiveName == "提交")// && data[0].AddUser == uData.TrueName
                {
                    strTitle = "查阅调研报告";
                    plView2.Visible = true;
                    ltOrgName2.Text = data[0].OrgName;
                    ltIsPoint2.Text = data[0].IsPoint;
                    ltSubMan2.Text = data[0].SubMan;
                    ltSubMans2.Text = data[0].SubMans;
                    ltTitle2.Text = data[0].Title;
                    ltBody2.Text = data[0].Body;
                    if (!string.IsNullOrEmpty(data[0].Files))
                    {
                        string strFiles = "";
                        string[] arr = data[0].Files.Split('|');
                        for (int i = 0; i < arr.Count(); i++)
                        {
                            if (!string.IsNullOrEmpty(arr[i]))
                            {
                                strFiles += string.Format("<a href='{0}' target='_blank'>{0}</a><br/>", arr[i]);
                            }
                        }
                        ltFiles2.Text = strFiles;
                    }
                    ltSubTime2.Text = data[0].SubTime.ToString("yyyy-MM-dd HH:mm:ss");
                    ltAddUser2.Text = data[0].AddUser;
                }
                else
                {
                    page.Response.Redirect("report.aspx");
                }
            }
            return strTitle;
        }
        //提交数据
        protected void btnSub_Click(object sender, EventArgs e)
        {
            editData("提交");
        }
        //暂存数据
        protected void btnSave_Click(object sender, EventArgs e)
        {
            editData("暂存");
        }
        //删除数据
        protected void btnDel_Click(object sender, EventArgs e)
        {
            editData("删除");
        }
        //编辑数据
        private void editData(string ActiveName)
        {
            ltInfo.Text = EditData(ActiveName, this, myUser, txtOrgName, rblIsPoint, txtSubMan, txtSubMans, txtTitle, txtBody, hfFiles, txtSubMan);
        }
        public string EditData(string ActiveName, Page page, DataUser uData, TextBox txtOrgName2, RadioButtonList rblIsPoint2, TextBox txtSubMan2, TextBox txtSubMans2, TextBox txtTitle2, TextBox txtBody2, HiddenField hfFiles2, TextBox txtAddUser2)
        {
            if (uData == null)
            {
                return "<script>$(function(){ alert('请重新登录！'); });</script>";
            }
            string strOut = "";
            string strBack = PublicMod.GetBackUrl();
            DataReport data = new DataReport();
            data.Id = (!string.IsNullOrEmpty(page.Request.QueryString["id"])) ? Convert.ToInt32(page.Request.QueryString["id"]) : 0;
            if (ActiveName == "删除")
            {
                if (data.Id > 0)
                {
                    data.Id = webReport.UpdateActive(data.Id, ActiveName);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                DateTime dtNow = DateTime.Now;
                string strIp = HelperMain.GetIpPort();
                string strUser = HelperMain.SqlFilter(uData.TrueName, 20);
                if (ActiveName == "提交")
                {
                    data.SubTime = dtNow;
                    data.SubIp = strIp;
                    data.ActiveName = "提交";
                }
                else
                {
                    data.ActiveName = "暂存";
                }
                data.OrgName = HelperMain.SqlFilter(txtOrgName2.Text.Trim(), 50);
                data.IsPoint = HelperMain.SqlFilter(rblIsPoint2.SelectedValue.Trim(), 2);
                data.SubMan = HelperMain.SqlFilter(txtSubMan2.Text.Trim(), 20);
                data.SubMans = HelperMain.SqlFilter(txtSubMans2.Text.Trim());
                data.Title = HelperMain.SqlFilter(txtTitle2.Text.Trim(), 50);
                data.Body = HelperMain.SqlFilter(txtBody2.Text.Trim());
                data.Files = HelperMain.SqlFilter(hfFiles2.Value.Trim('|'));
                //data.Remark
                if (data.Id <= 0)
                {
                    DataReport qData = new DataReport();
                    qData.Title = data.Title;
                    qData.AddTime = DateTime.Today;
                    qData.AddUser = strUser;
                    DataReport[] ckData = webReport.GetDatas(qData, "Id", 1, 1);//重复检查
                    if (ckData != null)
                    {
                        //ltInfo.Text = "<script>$(function(){ alert('“标题”重复，不能添加！'); window.history.back(-1); });</script>";
                        strOut = "<script>$(function(){ alert('重复提交！'); });</script>";
                        page.Response.Redirect(strBack);
                        return strOut;
                    }
                    data.UserId = uData.Id;
                    data.AddTime = dtNow;
                    data.AddIp = strIp;
                    data.AddUser = strUser;
                    data.Id = webReport.Insert(data);
                }
                else
                {
                    data.UserId = -1;
                    data.UpTime = dtNow;
                    data.UpIp = strIp;
                    data.UpUser = strUser;
                    if (webReport.Update(data) <= 0)
                    {
                        data.Id = -1;
                    }
                }
            }
            if (data.Id > 0)
            {
                strOut = "<script>$(function(){ alert('“" + ActiveName + "调研报告”成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                strOut = "<script>$(function(){ alert('“" + ActiveName + "调研报告”失败！'); window.history.back(-1); });</script>";
            }
            return strOut;
        }
        #endregion
        //
        #region 查询
        //首页我的调研报告
        public void MyReport(string TrueName, Repeater rpList)
        {
            DataReport data = new DataReport();
            data.ActiveName = "审核通过,提交";
            data.AddUser = TrueName;
            listData(data, rpList);
        }
        //加载我的列表
        public void MyList(string ActiveName, string strUser, Repeater rpList, Literal ltNo = null, Label lblNav = null, Literal ltTotal = null, Page page = null)
        {
            DataReport data = new DataReport();
            data.ActiveName = ActiveName;
            data.AddUser = strUser;
            listData(data, rpList, ltNo, lblNav, ltTotal, page);
        }
        //查询信息
        public void QueryData(DataUser uData, Repeater rpList, Literal ltNo, Label lblNav, Literal ltTotal, Page page, HiddenField hfOrg2, DropDownList ddlQOrgType2, TextBox txtQOrgName2, DropDownList ddlQIsPoint2, TextBox txtQSubMan2, TextBox txtQSubMans2, TextBox txtQTitle2, TextBox txtQAddUser2, TextBox txtQSubTime01, TextBox txtQSubTime02)
        {
            PublicMod.LoadDropDownLists(hfOrg2, ddlQOrgType2, "OpName");
            DataReport data = new DataReport();
            data.ActiveName = "审核通过,提交";
            if (!string.IsNullOrEmpty(page.Request.QueryString["AddUser"]))
            {
                data.AddUser = HelperMain.SqlFilter(page.Request.QueryString["AddUser"].Trim(), 20);
                txtQAddUser2.Text = data.AddUser;
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["OrgName"]))
            {
                data.OrgName = HelperMain.SqlFilter(page.Request.QueryString["OrgName"].Trim(), 20);
                txtQOrgName2.Text = data.OrgName;
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["IsPoint"]))
            {
                data.IsPoint = HelperMain.SqlFilter(page.Request.QueryString["IsPoint"].Trim(), 2);
                HelperMain.SetDownSelected(ddlQIsPoint2, data.IsPoint);
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["SubMan"]))
            {
                data.SubMan = HelperMain.SqlFilter(page.Request.QueryString["SubMan"].Trim(), 20);
                txtQSubMan2.Text = data.SubMan;
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["SubMans"]))
            {
                data.SubMans = HelperMain.SqlFilter(page.Request.QueryString["SubMans"].Trim());
                txtQSubMans2.Text = data.SubMans;
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["Title"]))
            {
                data.Title = "%" + HelperMain.SqlFilter(page.Request.QueryString["Title"].Trim(), 50) + "%";
                txtQTitle2.Text = data.Title.Trim('%');
            }
            if (!string.IsNullOrEmpty(page.Request.QueryString["SubTime"]) && page.Request.QueryString["SubTime"].IndexOf(",") >= 0)
            {
                string strSubTime1 = page.Request.QueryString["SubTime"].Substring(0, page.Request.QueryString["SubTime"].IndexOf(","));
                string strSubTime2 = page.Request.QueryString["SubTime"].Substring(page.Request.QueryString["SubTime"].IndexOf(",") + 1);
                txtQSubTime01.Text = HelperMain.SqlFilter(strSubTime1.Trim(), 10);
                txtQSubTime02.Text = HelperMain.SqlFilter(strSubTime2.Trim(), 10);
                if (txtQSubTime01.Text != "" || txtQSubTime02.Text != "")
                {
                    data.SubTimeText = txtQSubTime01.Text + "," + txtQSubTime02.Text;
                }
            }
            listData(data, rpList, ltNo, lblNav, ltTotal, page);
        }
        //加载列表
        private void listData(DataReport qData, Repeater rpList, Literal ltNo = null, Label lblNav = null, Literal ltTotal = null, Page page = null)
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
            string strOrder = "";
            DataReport[] data = webReport.GetDatas(qData, "Id,OrgName,SubMan,Title,SubTime,AddUser,ActiveName,UpTime", pageCur, pageSize, strOrder, "total");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    if (!string.IsNullOrEmpty(data[i].OrgName))
                    {
                        string[] arr = data[i].OrgName.Split(',');
                        for (int j = 0; j < arr.Count(); j++)
                        {
                            if (arr[j].IndexOf("-") > 0)
                            {
                                arr[j] = arr[j].Substring(arr[j].IndexOf("-") + 1);
                            }
                        }
                        data[i].OrgName = string.Join("<br/>", arr);
                    }
                    switch (data[i].ActiveName)
                    {
                        case "暂存":
                            data[i].rowClass = " class='save' title='暂存'";
                            data[i].StateName = "选取";
                            break;
                        case "退回":
                            data[i].rowClass = " class='cancel' title='退回'";
                            data[i].StateName = "选取";
                            if (data[i].SubTime > DateTime.MinValue)
                            {
                                data[i].SubTimeText = data[i].SubTime.ToString("yyyy-MM-dd");
                            }
                            break;
                        default:
                            data[i].StateName = "查看";
                            if (data[i].SubTime > DateTime.MinValue)
                            {
                                data[i].SubTimeText = data[i].SubTime.ToString("yyyy-MM-dd");
                            }
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
    }
}