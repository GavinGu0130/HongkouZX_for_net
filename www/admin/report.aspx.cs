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
    public partial class report : System.Web.UI.Page
    {
        private DataAdmin myUser = null;
        private WebReport webReport = new WebReport();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperAdmin.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
            {
                //Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            if (myUser.Powers.IndexOf("alls") < 0 && myUser.Powers.IndexOf("report") < 0)
            {
                Response.Redirect("./");
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.Powers = myUser.Powers;
            plNav.Visible = true;
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    Header.Title += " - 查阅调研报告";
                    plEdit.Visible = true;
                    loadData(Convert.ToInt32(Request.QueryString["id"]));
                }
                else
                {
                    Header.Title += " - 调研报告查询";
                    plQuery.Visible = true;
                    queryData();
                }
            }
        }
        //
        #region 查询
        //首页列表
        public void MyList(Repeater rpList, Literal ltNo)
        {
            DataReport qData = new DataReport();
            qData.ActiveName = "提交";
            //qData.OverTimeText = DateTime.Now.ToString("yyyy-MM-dd") + ",";
            string strOrder = "SubTime ASC, UpTime DESC, AddTime DESC";
            listData(qData, strOrder, rpList, ltNo);
        }
        //查询列表
        protected void queryData()
        {
            PublicMod.LoadDropDownLists(hfOrg, ddlOrgType, "OpName");
            DataReport data = new DataReport();
            if (!string.IsNullOrEmpty(Request.QueryString["OrgName"]))
            {
                data.OrgName = HelperMain.SqlFilter(Request.QueryString["OrgName"].Trim());
                txtQOrgName.Text = data.OrgName;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["IsPoint"]))
            {
                data.IsPoint = HelperMain.SqlFilter(Request.QueryString["IsPoint"].Trim(), 2);
                HelperMain.SetDownSelected(ddlQIsPoint, data.IsPoint);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["SubMan"]))
            {
                data.SubMan = HelperMain.SqlFilter(Request.QueryString["SubMan"].Trim(), 20);
                txtQSubMan.Text = data.SubMan;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["SubMans"]))
            {
                data.SubMans = HelperMain.SqlFilter(Request.QueryString["SubMans"].Trim());
                txtQSubMans.Text = data.SubMans;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Title"]))
            {
                data.Title = "%" + HelperMain.SqlFilter(Request.QueryString["Title"].Trim(), 50) + "%";
                txtQTitle.Text = data.Title.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["AddUser"]))
            {
                data.AddUser = HelperMain.SqlFilter(Request.QueryString["AddUser"].Trim(), 20);
                txtQAddUser.Text = data.AddUser;
            }
            if (!string.IsNullOrEmpty(Request.QueryString["SubTime"]) && Request.QueryString["SubTime"].IndexOf(",") >= 0)
            {
                string strTime = Request.QueryString["SubTime"].Trim();
                string strTime1 = strTime.Substring(0, strTime.IndexOf(","));
                string strTime2 = strTime.Substring(strTime.IndexOf(",") + 1);
                txtQSubTime1.Text = HelperMain.SqlFilter(strTime1.Trim(), 10);
                txtQSubTime2.Text = HelperMain.SqlFilter(strTime2.Trim(), 10);
                if (txtQSubTime1.Text != "" || txtQSubTime2.Text != "")
                {
                    data.SubTimeText = txtQSubTime1.Text + "," + txtQSubTime2.Text;
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Active"]))
            {
                data.ActiveName = HelperMain.SqlFilter(Request.QueryString["Active"].Trim(), 20);
                HelperMain.SetCheckSelected(cblQActive, data.ActiveName);
            }
            else
            {
                data.ActiveName = "<>'暂存'";
            }
            string strOrder = "ActiveName ASC, SubTime DESC, UpTime DESC, AddTime DESC";
            listData(data, strOrder, rpQueryList, ltQueryNo, lblQueryNav, ltQueryTotal, this);
        }
        //加载列表
        private void listData(DataReport qData, string strOrder, Repeater rpList, Literal ltNo, Label lblNav = null, Literal ltTotal = null, Page page = null)
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
            DataReport[] data = webReport.GetDatas(qData, "Id,OrgName,SubMan,Title,ActiveName,SubTime,AddUser,VerifyTime", pageCur, pageSize, strOrder, "total");
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
                    if (data[i].SubTime > DateTime.MinValue)
                    {
                        data[i].SubTimeText = (data[i].SubTime < DateTime.Today) ? data[i].SubTime.ToString("yyyy-MM-dd") : data[i].SubTime.ToString("HH:mm:ss");
                    }
                    if (data[i].VerifyTime > DateTime.MinValue)
                    {
                        data[i].VerifyTimeText = (data[i].VerifyTime < DateTime.Today) ? data[i].VerifyTime.ToString("yyyy-MM-dd") : data[i].VerifyTime.ToString("HH:mm:ss");
                    }
                    switch (data[i].ActiveName)
                    {
                        case "已提交":
                            data[i].rowClass = " class='save' title='已提交'";
                            data[i].StateName = "查看";
                            break;
                        case "暂存":
                            data[i].rowClass = " class='save' title='暂存'";
                            data[i].StateName = "选取";
                            break;
                        case "退回":
                            data[i].rowClass = " class='cancel' title='退回'";
                            data[i].StateName = "选取";
                            break;
                        case "删除":
                            data[i].rowClass = " class='del' title='删除'";
                            data[i].StateName = "查看";
                            break;
                        default:
                            data[i].StateName = "查看";
                            break;
                    }
                    //if (!string.IsNullOrEmpty(data[i].Files))
                    //{
                    //    data[i].Files = string.Format("<a href='{0}' target='_blank' class='btn'><u>附件下载</u></a>", data[i].Files);
                    //}
                    //else
                    //{
                    //    data[i].Files = "<br/>";
                    //}
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
        //加载信息
        private void loadData(int Id)
        {
            PublicMod.LoadDropDownLists(hfOrg, ddlOrgType, "OpName");
            if (Id <= 0)
            {
                return;
            }
            DataReport[] data = webReport.GetData(Id);
            if (data != null)
            {
                ltAddUser.Text = data[0].AddUser;
                ltSubTime.Text = data[0].SubTime.ToString("yyyy-MM-dd HH:mm:ss");
                txtOrgName.Text = data[0].OrgName;
                HelperMain.SetRadioSelected(rblIsPoint, data[0].IsPoint);
                txtSubMan.Text = data[0].SubMan;
                txtSubMans.Text = data[0].SubMans;
                txtTitle.Text = data[0].Title;
                txtBody.Text = data[0].Body;
                hfFiles.Value = data[0].Files;
                txtRemark.Text = data[0].Remark;
                HelperMain.SetDownSelected(ddlActiveName, data[0].ActiveName);
            }
        }
        //提交数据
        protected void btnSub_Click(object sender, EventArgs e)
        {
            string ActiveName = HelperMain.SqlFilter(ddlActiveName.SelectedValue.Trim(), 20);
            editData(ActiveName);//"审核通过"
        }
        //删除数据
        protected void btnDel2_Click(object sender, EventArgs e)
        {
            editData("删除");
        }
        //编辑数据
        private void editData(string ActiveName)
        {
            DataReport data = new DataReport();
            data.Id = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
            if (data.Id <= 0)
            {
                return;
            }
            if (ActiveName == "删除")
            {
                data.Id = webReport.UpdateActive(data.Id, ActiveName);
            }
            else
            {
                DateTime dtNow = DateTime.Now;
                string strIp = HelperMain.GetIpPort();
                string strUser = HelperMain.SqlFilter(myUser.AdminName, 20);
                data.OrgName = HelperMain.SqlFilter(txtOrgName.Text.Trim(), 50);
                data.IsPoint = HelperMain.SqlFilter(rblIsPoint.SelectedValue.Trim(), 2);
                data.SubMan = HelperMain.SqlFilter(txtSubMan.Text.Trim(), 20);
                data.SubMans = HelperMain.SqlFilter(txtSubMans.Text.Trim());
                data.Title = HelperMain.SqlFilter(txtTitle.Text.Trim(), 50);
                data.Body = HelperMain.SqlFilter(txtBody.Text.Trim());
                data.Files = HelperMain.SqlFilter(hfFiles.Value.Trim('|'));
                data.Remark = HelperMain.SqlFilter(txtRemark.Text.Trim());
                data.ActiveName = ActiveName;
                data.UserId = -1;
                data.VerifyTime = dtNow;
                data.VerifyIp = strIp;
                data.VerifyUser = strUser;
                if (webReport.Update(data) <= 0)
                {
                    data.Id = -1;
                }
            }
            if (data.Id > 0)
            {
                string strBack = PublicMod.GetBackUrl();
                ltInfo.Text = "<script>$(function(){ alert('“" + ActiveName.Replace("通过", "") + "调研报告”成功！'); window.location.href='" + strBack + "'; });</script>";
                if (data.ActiveName == "审核通过")
                {
                    WebScore webScore = new WebScore();
                    WebUser webUser = new WebUser();
                    addScore(data, webScore, webUser);//增加积分
                }
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ActiveName.Replace("通过", "") + "调研报告”失败！'); window.history.back(-1); });</script>";
            }
        }
        #endregion
        //增加积分
        private void addScore(ArrayList arrList)
        {
            WebScore webScore = new WebScore();
            WebUser webUser = new WebUser();
            for (int i = 0; i < arrList.Count; i++)
            {
                DataReport[] rData = webReport.GetData(Convert.ToInt32(arrList[i]));
                if (rData != null)
                {
                    addScore(rData[0], webScore, webUser);
                }
            }
        }
        private void addScore(DataReport data, WebScore webScore, WebUser webUser)
        {
            if (data == null)
            {
                return;
            }
            string strTitle = (data.IsPoint == "是") ? "%建议案调研课题" : "其他调研课题";
            DataScore[] sData = webScore.GetDatas(1, "调研课题", "", strTitle, "Score,Score2");
            if (sData == null)
            {
                return;
            }
            //Response.Write(sData[0].Score2.ToString()); Response.End();
            string TableName = "tb_Report";
            string[] arrSubMan = data.SubMan.Split(',');
            DateTime dtTime = (data.SubTime > DateTime.MinValue) ? data.SubTime : DateTime.Now;
            for (int i = 0; i < arrSubMan.Count(); i++)
            {
                if (!string.IsNullOrEmpty(arrSubMan[i]))
                {
                    DataUser[] uData = webUser.GetDatas(config.PERIOD, arrSubMan[i], "Id");
                    if (uData != null)
                    {
                        PublicMod.AddScore(uData[0].Id, strTitle.Trim('%') + "-执笔人", sData[0].Score2, TableName, data.Id, data.VerifyIp, data.VerifyUser, dtTime);
                    }
                }
            }
            string[] arrSubMans = data.SubMans.Split(',');
            for (int i = 0; i < arrSubMans.Count(); i++)
            {
                if (!string.IsNullOrEmpty(arrSubMans[i]))
                {
                    DataUser[] uData = webUser.GetDatas(config.PERIOD, arrSubMans[i], "Id");
                    if (uData != null)
                    {
                        PublicMod.AddScore(uData[0].Id, strTitle.Trim('%') + "-课题组", sData[0].Score, TableName, data.Id, data.VerifyIp, data.VerifyUser, dtTime);
                    }
                }
            }
        }
        //
        #region 操作
        protected void btnPass_Click(object sender, EventArgs e)
        {
            updateActive("审核通过");
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            string VerifyInfo = (!string.IsNullOrEmpty(hfVerifyInfo.Value)) ? HelperMain.SqlFilter(hfVerifyInfo.Value.Trim()) : "";
            if (!string.IsNullOrEmpty(VerifyInfo))
            {
                updateActive("退回", VerifyInfo);
            }
            //updateActive(0, "暂存");
            //updateActive(1, "提交");
        }
        protected void btnDel_Click(object sender, EventArgs e)
        {
            updateActive("删除");
        }
        //处理操作
        private void updateActive(string ActiveName, string VerifyInfo = "")
        {
            if (myUser == null)
            {
                return;
            }
            ArrayList arrList = new ArrayList();
            for (int i = 0; i < rpQueryList.Items.Count; i++)
            {
                CheckBox ck = (CheckBox)rpQueryList.Items[i].FindControl("_ck");
                HiddenField hf = (HiddenField)rpQueryList.Items[i].FindControl("_id");
                if (ck.Checked)
                {
                    arrList.Add(hf.Value);
                }
            }
            if (arrList.Count <= 0)
            {
                return;
            }
            string strIp = HelperMain.GetIpPort();
            string strUser = myUser.AdminName;
            if (webReport.UpdateActive(arrList, ActiveName, VerifyInfo, strIp, strUser) > 0)
            {
                string strBack = Request.Url.ToString();
                ltInfo.Text = "<script>$(function(){ alert('“" + ActiveName + "”操作成功！'); window.location.href='" + strBack + "'; });</script>";
                if (ActiveName == "审核通过")
                {
                    addScore(arrList);//增加积分
                }
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ActiveName + "”操作失败！'); window.history.back(-1); });</script>";
            }
        }
        #endregion
        //
    }
}