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
    public partial class user : System.Web.UI.Page
    {
        private DataAdmin myUser = null;
        private WebUser webUser = new WebUser();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperAdmin.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
            {
                //Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            if (myUser.Powers.IndexOf("alls") < 0 && myUser.Powers.IndexOf("user") < 0)
            {
                Response.Redirect("./");
            }
            if (Request.QueryString["ac"] == "output")
            {
                Header.Title += " - 委员信息 导出选项";
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
                if (!IsPostBack)
                {
                    string strTitle = "";
                    if (myUser.Grade >= 9 && myUser.Powers.IndexOf("alls") >= 0)
                    {
                        lnkScore.Visible = true;
                    }
                    if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                    {
                        hfBack.Value = PublicMod.GetBackUrl();
                        if (Request.QueryString["ac"] == "score")
                        {
                            plScore.Visible = true;
                            listScore(Convert.ToInt32(Request.QueryString["id"]));
                            strTitle = "委员积分情况";
                        }
                        else
                        {
                            plEdit.Visible = true;
                            loadData(Convert.ToInt32(Request.QueryString["id"]), myUser.AdminName);
                            strTitle = "编辑委员信息";
                        }
                    }
                    else if (Request.QueryString["ac"] == "scoreadd")
                    {
                        plScoreAdd.Visible = true;
                    }
                    else if (Request.QueryString["ac"] == "check")
                    {
                        checkData();
                        strTitle = "待审核委员信息";
                    }
                    else
                    {
                        queryData();
                        strTitle = "委员信息管理";
                    }
                    Header.Title += " - " + strTitle;
                }
            }
        }
        //
        #region 查询列表
        //获取检索条件
        private DataUser getDataUser()
        {
            DataUser data = new DataUser();
            data.Period = config.PERIOD;
            //if (!string.IsNullOrEmpty(Request.QueryString["Period"]))
            //{
            //    data.Period = HelperMain.SqlFilter(Request.QueryString["Period"].Trim(), 20);
            //    HelperMain.SetDownSelected(ddlQPeriod, data.Period);
            //}
            if (!string.IsNullOrEmpty(Request.QueryString["UserType"]))
            {
                data.UserType2 = HelperMain.SqlFilter(Request.QueryString["UserType"].Trim(), 20);
            }
            else if (string.IsNullOrEmpty(Request.QueryString["ac"]))
            {
                data.UserType2 = "在册委员";
            }
            switch (data.UserType2)
            {
                case "在册委员":
                    data.UserType = "委员";
                    data.ActiveName = ">0";
                    data.UserCode = "14%";
                    break;
                case "回收站":
                    data.ActiveName = "<=0";
                    break;
                default:
                    data.UserType = data.UserType2;
                    //data.ActiveName = ">0";
                    break;
            }
            HelperMain.SetDownSelected(ddlQUserType, data.UserType2);
            if (!string.IsNullOrEmpty(Request.QueryString["UserCode"]))
            {
                data.UserCode = "%" + HelperMain.SqlFilter(Request.QueryString["UserCode"].Trim(), 20) + "%";
                txtQUserCode.Text = data.UserCode.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["TrueName"]))
            {
                data.TrueName = "%" + HelperMain.SqlFilter(Request.QueryString["TrueName"].Trim(), 20) + "%";
                txtQTrueName.Text = data.TrueName.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["UserSex"]))
            {
                data.UserSex = HelperMain.SqlFilter(Request.QueryString["UserSex"].Trim(), 2);
                HelperMain.SetDownSelected(ddlQUserSex, data.UserSex);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Native"]))
            {
                data.Native = "%" + HelperMain.SqlFilter(Request.QueryString["Native"].Trim(), 20) + "%";
                txtQNative.Text = data.Native.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Nation"]))
            {
                data.Nation = "%" + HelperMain.SqlFilter(Request.QueryString["Nation"].Trim(), 8) + "%";
                HelperMain.SetDownSelected(ddlQNation, data.Nation.Trim('%'));
                data.Nation = data.Nation.Replace("族", "");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Birthday"]) && Request.QueryString["Birthday"].IndexOf(",") >= 0)
            {
                string strBirthday1 = Request.QueryString["Birthday"].Substring(0, Request.QueryString["Birthday"].IndexOf(","));
                string strBirthday2 = Request.QueryString["Birthday"].Substring(Request.QueryString["Birthday"].IndexOf(",") + 1);
                txtQBirthday1.Text = HelperMain.SqlFilter(strBirthday1.Trim(), 10);
                txtQBirthday2.Text = HelperMain.SqlFilter(strBirthday2.Trim(), 10);
                if (txtQBirthday1.Text != "" || txtQBirthday2.Text != "")
                {
                    data.BirthdayText = txtQBirthday1.Text + "," + txtQBirthday2.Text;
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["PostDate"]) && Request.QueryString["PostDate"].IndexOf(",") >= 0)
            {
                string strPostDate1 = Request.QueryString["PostDate"].Substring(0, Request.QueryString["PostDate"].IndexOf(","));
                string strPostDate2 = Request.QueryString["PostDate"].Substring(Request.QueryString["PostDate"].IndexOf(",") + 1);
                txtQPostDate1.Text = HelperMain.SqlFilter(strPostDate1.Trim(), 10);
                txtQPostDate2.Text = HelperMain.SqlFilter(strPostDate2.Trim(), 10);
                if (txtQPostDate1.Text != "" || txtQPostDate2.Text != "")
                {
                    data.PostDateText = txtQPostDate1.Text + "," + txtQPostDate2.Text;
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Education"]))
            {
                data.Education = "%" + HelperMain.SqlFilter(Request.QueryString["Education"].Trim(), 20) + "%";
                HelperMain.SetDownSelected(ddlQEducation, data.Education.Trim('%'));
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Party"]))
            {
                data.Party = "%" + HelperMain.SqlFilter(Request.QueryString["Party"].Trim(), 10) + "%";
                HelperMain.SetDownSelected(ddlQParty, data.Party.Trim('%'));
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Committee"]))
            {
                data.Committee = HelperMain.SqlFilter(Request.QueryString["Committee"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQCommittee, data.Committee.Trim('%'));
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Subsector"]))
            {
                data.Subsector = HelperMain.SqlFilter(Request.QueryString["Subsector"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQSubsector, data.Subsector);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["StreetTeam"]))
            {
                data.StreetTeam = HelperMain.SqlFilter(Request.QueryString["StreetTeam"].Trim(), 20);
                HelperMain.SetDownSelected(ddlQStreetTeam, data.StreetTeam);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["OrgType"]))
            {
                data.OrgType = HelperMain.SqlFilter(Request.QueryString["OrgType"].Trim(), 10);
                HelperMain.SetDownSelected(ddlQOrgType, data.OrgType);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["OrgName"]))
            {
                data.OrgName = "%" + HelperMain.SqlFilter(Request.QueryString["OrgName"].Trim(), 20) + "%";
                txtQOrgName.Text = data.OrgName.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["ContactTel"]))
            {
                data.ContactTel = "%" + HelperMain.SqlFilter(Request.QueryString["ContactTel"].Trim(), 20) + "%";
                txtQContactTel.Text = data.ContactTel.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["role"]))
            {
                data.Role = HelperMain.SqlFilter(Request.QueryString["role"].Trim());
                HelperMain.SetCheckSelected(cblQRole, data.Role);
            }
            return data;
        }
        //搜索查询
        private void queryData()
        {
            plQuery.Visible = true;
            PublicMod.LoadNation(ddlQNation);
            PublicMod.LoadEducation(ddlQEducation);
            WebOp webOp = new WebOp();
            //PublicMod.LoadDropDownList(ddlQPeriod, webOp, "_届");
            PublicMod.LoadDropDownList(ddlQUserType, webOp, "用户类别");
            PublicMod.LoadDropDownList(ddlQParty, webOp, "政治面貌");
            PublicMod.LoadDropDownList(ddlQCommittee, webOp, "专委会");
            PublicMod.LoadDropDownList(ddlQSubsector, webOp, "界别");
            PublicMod.LoadDropDownList(ddlQStreetTeam, webOp, "街道活动组");
            PublicMod.LoadDropDownList(ddlQOrgType, webOp, "单位性质");
            PublicMod.LoadCheckBoxList(cblQRole, webOp, "政协职务");
            //PublicMod.LoadCheckBoxList(cblQFields, webOp, "委员导出选项");
            DataUser data = getDataUser();
            listData(data);
        }
        //待审核列表
        private void checkData()
        {
            plNav.Visible = false;
            plCheck.Visible = true;
            DataUser data = new DataUser();
            data.Period = config.PERIOD;
            data.UserType = "委员";
            data.CheckText = "yes";
            listData(data);
        }
        //加载列表
        private void listData(DataUser qData)
        {
            plList.Visible = true;
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            string strFields = "";//"Id,UserCode,TrueName,Birthday,Committee,Committee2,Subsector,Subsector2";//
            DataUser[] data = webUser.GetDatas(qData, strFields, pageCur, pageSize, "", "total");
            if (data != null)
            {
                WebUserWx webUserWx = new WebUserWx();
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    if (data[i].Active > 0)
                    {
                        data[i].ActiveName = "正常";
                    }
                    else if (data[i].ErrNum > 10)
                    {
                        data[i].ActiveName = "密码错误";
                        data[i].rowClass = " class='cancel' title='密码错误过多'";
                    }
                    else if (data[i].Active <= -444)
                    {
                        data[i].ActiveName = "删除";
                        data[i].rowClass = " class='del' title='删除'";
                    }
                    else
                    {
                        data[i].ActiveName = "锁定";
                        data[i].rowClass = " class='del' title='锁定'";
                    }
                    if (data[i].Birthday > DateTime.MinValue)
                    {
                        data[i].BirthdayText = data[i].Birthday.ToString("yyyy-MM-dd");
                    }
                    DataUserWx[] wx = webUserWx.GetDatas(config.APPID, data[i].Id, "", "WxOpenId");
                    if (wx != null)
                    {
                        data[i].WxOpenId = "<i class=\"wx\" title=\"" + wx[0].WxOpenId + "\"></i>";
                    }
                }
                rpList.DataSource = data;
                rpList.DataBind();
                ltNo.Visible = false;
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = Request.Url.ToString();
                lblNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
                ltTotal.Text = pageCount.ToString();
            }
        }
        #endregion
        //
        #region 导出//下载名单
        protected void btnOutput_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                ltInfo.Text = "<script>$(function(){ alert('请重新登录！'); });</script>";
                return;
            }
            DataUser qData = getDataUser();
            DataUser[] data = webUser.GetDatas(qData);
            if (data != null)
            {
                string strFields = HelperMain.GetCheckSelected(cblOutput);
                int intYear = DateTime.Today.Year;
                string strStart = intYear.ToString() + "-1-1";
                string strEnd = intYear.ToString() + "-12-31";
                if (strFields.IndexOf("UserScore") >= 0)
                {
                    WebUserScore webScore = new WebUserScore();
                    for (int i = 0; i < data.Count(); i++)
                    {
                        decimal deScoreTotal = webScore.GetTotalScore(data[i].Id, strStart, strEnd);//累计总积分
                        decimal deScore2 = webScore.GetTotalScore(data[i].Id, strStart, strEnd, "tb_Opinion,tb_Opinion_Pop");//建言得分
                        data[i].UserScore = deScoreTotal;//.ToString("n2")
                        data[i].UserScore1 = (deScoreTotal - deScore2);
                        data[i].UserScore2 = deScore2;
                    }
                }
                //PublicMod.UserScoreOrder(data);//排序
                //rpList.DataSource = data;
                //rpList.DataBind();
                DateTime dtNow = DateTime.Now;
                string virtualPath = string.Format("../download/{0:yyyy}/{0:MM}/", dtNow);
                string filepath = HttpContext.Current.Server.MapPath(virtualPath);
                if (!System.IO.Directory.Exists(filepath))
                {
                    System.IO.Directory.CreateDirectory(filepath);
                }
                string strTitle = qData.UserType2 + "用户检索";
                string strFileName = string.Format("{0}_{1:yyyyMMddHHmmss}.xls", qData.UserType2, dtNow);
                string fileName = virtualPath + "/" + strFileName;
                string path = Server.MapPath(fileName);
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
                workSheet.Rows["3:6"].RowHeight = 20;
                PublicMod.SetCells(workSheet, 3, 1, string.Format("统计时间：{0} - {1}", strStart, strEnd));
                workSheet.Cells[5, 1].Font.Size = 16;//设置字体大小
                PublicMod.SetCells(workSheet, 5, 1, strTitle, "center,bold,border");
                PublicMod.SetCells(workSheet, 6, 1, "序号", "fit,center,bold,border", "LightGray");
                int intRow = 6;
                int num = 1;
                if (strFields.IndexOf("UserType") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "用户类别", "fit,center,bold,border", "LightGray");
                }
                if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("UserCode") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "委员编号", "fit,center,bold,border", "LightGray");
                }
                if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("TrueName") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "姓名", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("UserSex") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "性别", "fit,center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("Party") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "政治面貌", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("IdCard") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 20;//列宽
                    PublicMod.SetCells(workSheet, intRow, num, "身份证号", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("Birthday") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 12;
                    PublicMod.SetCells(workSheet, intRow, num, "出生日期", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("Native") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "籍贯", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("Nation") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "民族", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("Education") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "文化程度", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("Mobile") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 12;
                    PublicMod.SetCells(workSheet, intRow, num, "手机号码", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("WeChat") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 12;
                    PublicMod.SetCells(workSheet, intRow, num, "微信号码", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("PostDate") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 12;
                    PublicMod.SetCells(workSheet, intRow, num, "任职日期", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("Subsector") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "界别", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("Committee") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 20;
                    PublicMod.SetCells(workSheet, intRow, num, "专委会", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("StreetTeam") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 15;
                    PublicMod.SetCells(workSheet, intRow, num, "街道活动组", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("Role") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "政协职务", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("HkMacaoTw") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "港澳台委员", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("OrgName") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 20;
                    PublicMod.SetCells(workSheet, intRow, num, "工作单位及职务", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("OrgPost") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "职称", "fit,center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("OrgType") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "单位性质", "fit,center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("OrgAddress") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 20;
                    PublicMod.SetCells(workSheet, intRow, num, "单位地址", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("OrgZip") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "单位邮编", "fit,center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("OrgTel") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 12;
                    PublicMod.SetCells(workSheet, intRow, num, "单位电话", "fit,center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("SocietyDuty") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 15;
                    PublicMod.SetCells(workSheet, intRow, num, "社会职务", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("HomeAddress") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 20;
                    PublicMod.SetCells(workSheet, intRow, num, "家庭地址", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("HomeZip") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "家庭邮编", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("HomeTel") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 12;
                    PublicMod.SetCells(workSheet, intRow, num, "家庭电话", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("ContactAddress") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 20;
                    PublicMod.SetCells(workSheet, intRow, num, "通讯地址及邮编", "center,bold,border", "LightGray");
                }
                int intCol = num;
                //workSheet.Range[workSheet.Cells[3, 1], workSheet.Cells[3, intCol]].MergeCells = true;//合并单元格
                workSheet.Range[workSheet.Cells[5, 1], workSheet.Cells[5, intCol]].MergeCells = true;//合并单元格
                if (strFields.IndexOf("UserScore") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "总积分", "center,bold,border", "LightGray");
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "会议活动得分", "center,bold,border", "LightGray");
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "建言得分", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("Active") >= 0)
                {
                    num++;
                    PublicMod.SetCells(workSheet, intRow, num, "状态", "center,bold,border", "LightGray");
                }
                if (strFields.IndexOf("Remark") >= 0)
                {
                    num++;
                    workSheet.Cells[intRow, num].ColumnWidth = 20;
                    PublicMod.SetCells(workSheet, intRow, num, "备注", "center,bold,border", "LightGray");
                }
                for (int i = 0; i < data.Count(); i++)
                {
                    intRow = i + 7;
                    num = 1;
                    PublicMod.SetCells(workSheet, intRow, num, (i + 1).ToString(), "center,border");//序号
                    if (strFields.IndexOf("UserType") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].UserType, "txt,center,border");//用户类别
                    }
                    if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("UserCode") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].UserCode, "txt,center,border");//委员编号
                    }
                    if (string.IsNullOrEmpty(strFields) || strFields.IndexOf("TrueName") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].TrueName, "txt,center,border");//姓名
                    }
                    if (strFields.IndexOf("UserSex") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].UserSex, "txt,center,border");//性别
                    }
                    if (strFields.IndexOf("Party") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].Party, "txt,center,border");//政治面貌
                    }
                    if (strFields.IndexOf("IdCard") >= 0)
                    {
                        num++;
                        string strIdCard = "";
                        if (!string.IsNullOrEmpty(data[i].IdCard))
                        {
                            if (data[0].IdCard.Length > 18)
                            {
                                strIdCard = HelperSecret.DESDecrypt(data[i].IdCard, config.IDDESKEY, config.IDDESIV);//des解密
                            }
                            else
                            {
                                strIdCard = data[i].IdCard;
                            }
                        }
                        PublicMod.SetCells(workSheet, intRow, num, strIdCard, "txt,center,border");//身份证号
                    }
                    if (strFields.IndexOf("Birthday") >= 0)
                    {
                        num++;
                        string strDate = (data[i].Birthday > DateTime.MinValue) ? data[i].Birthday.ToString("yyyy-MM-dd") : "";
                        PublicMod.SetCells(workSheet, intRow, num, strDate, "center,border");//出生日期
                    }
                    if (strFields.IndexOf("Native") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].Native, "txt,center,border");//籍贯
                    }
                    if (strFields.IndexOf("Nation") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].Nation, "txt,center,border");//民族
                    }
                    if (strFields.IndexOf("Education") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].Education, "txt,center,border");//文化程度
                    }
                    if (strFields.IndexOf("Mobile") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].Mobile, "txt,center,border");//手机号码
                    }
                    if (strFields.IndexOf("WeChat") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].WeChat, "txt,center,border");//微信号码
                    }
                    if (strFields.IndexOf("PostDate") >= 0)
                    {
                        num++;
                        string strDate = (data[i].PostDate > DateTime.MinValue) ? data[i].PostDate.ToString("yyyy-MM-dd") : "";
                        PublicMod.SetCells(workSheet, intRow, num, strDate, "center,border");//任职日期
                    }
                    if (strFields.IndexOf("Subsector") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].Subsector, "txt,center,border");//界别
                    }
                    if (strFields.IndexOf("Committee") >= 0)
                    {
                        num++;
                        string strComm = data[i].Committee;
                        if (!string.IsNullOrEmpty(data[i].Committee2))
                        {
                            strComm += "、" + data[i].Committee2;
                        }
                        PublicMod.SetCells(workSheet, intRow, num, strComm, "txt,wrap,border");//专委会
                    }
                    if (strFields.IndexOf("StreetTeam") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].StreetTeam, "txt,wrap,border");//街道活动组
                    }
                    if (strFields.IndexOf("Role") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].Role, "txt,wrap,border");//政协职务
                    }
                    if (strFields.IndexOf("HkMacaoTw") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].HkMacaoTw, "txt,border");//港澳台委员
                    }
                    if (strFields.IndexOf("OrgName") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].OrgName, "txt,wrap,border");//工作单位及职务
                    }
                    if (strFields.IndexOf("OrgPost") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].OrgPost, "txt,wrap,border");//职称
                    }
                    if (strFields.IndexOf("OrgType") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].OrgType, "txt,center,border");//单位性质
                    }
                    if (strFields.IndexOf("OrgAddress") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].OrgAddress, "txt,wrap,border");//单位地址
                    }
                    if (strFields.IndexOf("OrgZip") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].OrgZip, "txt,center,border");//单位邮编
                    }
                    if (strFields.IndexOf("OrgTel") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].OrgTel, "txt,border");//单位电话
                    }
                    if (strFields.IndexOf("SocietyDuty") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].SocietyDuty, "txt,wrap,border");//社会职务
                    }
                    if (strFields.IndexOf("HomeAddress") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].HomeAddress, "txt,wrap,border");//家庭地址
                    }
                    if (strFields.IndexOf("HomeZip") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].HomeZip, "txt,center,border");//家庭邮编
                    }
                    if (strFields.IndexOf("HomeTel") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].HomeTel, "txt,border");//家庭电话
                    }
                    if (strFields.IndexOf("ContactAddress") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].ContactAddress, "txt,wrap,border");//通讯地址及邮编
                    }
                    if (strFields.IndexOf("UserScore") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].UserScore.ToString("n2"), "txt,center,border");//总积分
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].UserScore1.ToString("n2"), "txt,center,border");//会议活动得分
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].UserScore2.ToString("n2"), "txt,center,border");//建言得分
                    }
                    if (strFields.IndexOf("Active") >= 0)
                    {
                        num++;
                        string strActive = "";
                        if (data[i].Active <= 0)
                        {

                        }
                        else if (data[i].ErrNum > 10)
                        {
                            strActive = "密码错误过多";
                        }
                        else
                        {
                            strActive = "在册";
                        }
                        PublicMod.SetCells(workSheet, intRow, num, strActive, "txt,center,border");//状态
                    }
                    if (strFields.IndexOf("Remark") >= 0)
                    {
                        num++;
                        PublicMod.SetCells(workSheet, intRow, num, data[i].Remark, "txt,wrap,border");//备注
                    }
                }
                //有两个选项可以设置，如下
                excelApp.Visible = false;//visable属性设置为true的话，excel程序会启动；false的话，excel只在后台运行
                excelApp.DisplayAlerts = false;//displayalert设置为true将会显示excel中的提示信息
                //保存文件，关闭workbook
                workBook.SaveAs(path, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
                //workBook.SaveAs(path, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                workBook.Close(false, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                //退出并清理objects，回收内存
                excelApp.Quit();
                workSheet = null;
                workBook = null;
                excelApp = null;
                GC.Collect();
                Response.Redirect(fileName);
            }
        }
        #endregion
        //
        #region 编辑
        //加载数据
        private void loadData(int Id, string strAddUser)
        {
            PublicMod.LoadNation(ddlNation);
            WebOp webOp = new WebOp();
            //PublicMod.LoadCheckBoxList(cblPeriod, webOp, "_届");
            PublicMod.LoadRadioButtonList(rblUserType, webOp, "用户类别");
            rblUserType.SelectedIndex = 0;
            PublicMod.LoadDropDownList(ddlEducation, webOp, "文化程度");
            PublicMod.LoadDropDownLists(hfEducation, ddlEducation, "OpName", null, webOp);
            PublicMod.LoadCheckBoxList(cblParty, webOp, "政治面貌");
            PublicMod.LoadCheckBoxList(cblRole, webOp, "政协职务");
            PublicMod.LoadDropDownList(ddlHkMacaoTw, webOp, "港澳台委员");
            PublicMod.LoadDropDownList(ddlCommittee, webOp, "专委会");
            PublicMod.LoadDropDownLists(hfCommittee, ddlCommittee, "OpName", null, webOp);
            //for (int i = 0; i < ddlCommittee.Items.Count; i++)
            //{
            //    ddlCommittee2.Items.Add(ddlCommittee.Items[i].Value);
            //}
            PublicMod.LoadDropDownList(ddlSubsector, webOp, "界别");
            //PublicMod.LoadDropDownList(ddlSubsector2, webOp, "界别活动组");
            PublicMod.LoadDropDownList(ddlStreetTeam, webOp, "街道活动组");
            PublicMod.LoadDropDownList(ddlOrgType, webOp, "单位性质");

            if (myUser.Grade >= 9)
            {
                txtOldId.ReadOnly = false;
            }
            if (Id <= 0)
            {
                txtAddTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                txtAddIp.Text = HelperMain.GetIp();
                txtAddUser.Text = strAddUser;
                return;
            }
            DataUser[] data = webUser.GetData(Id);
            if (data != null)
            {
                int intYear = DateTime.Today.Year;
                string strStart = intYear.ToString() + "-1-1";
                string strEnd = intYear.ToString() + "-12-31";
                WebUserScore webScore = new WebUserScore();
                if (data[0].OrderScore >= 0)
                {
                    decimal deScoreTotal = webScore.GetTotalScore(Id, strStart, strEnd);//累计总积分
                    decimal deScore2 = webScore.GetTotalScore(Id, strStart, strEnd, "tb_Opinion,tb_Opinion_Pop");//建言得分
                    txtUserScore.Text = deScoreTotal.ToString("n2");
                    txtUserScore1.Text = (deScoreTotal - deScore2).ToString("n2");
                    txtUserScore2.Text = deScore2.ToString("n2");
                }
                else
                {
                    HelperMain.SetCheckSelected(cblNoScore, "-1");
                }
                WebLog webLog = new WebLog();
                txtLoginNum.Text = webLog.GetCount(1, "user_Login", strStart + "," + strEnd).ToString();

                txtId.Text = data[0].Id.ToString();
                txtOldId.Text = data[0].OldId.ToString();
                //HelperMain.SetCheckSelected(cblPeriod, data[0].Period.Trim(','));
                //data[0].PostNum
                HelperMain.SetRadioSelected(rblUserType, data[0].UserType);
                ltUserName.Text = data[0].UserName;
                //data[0].UserPwd
                txtUserCode.Text = data[0].UserCode;
                txtTrueName.Text = data[0].TrueName;
                HelperMain.SetRadioSelected(rblUserSex, data[0].UserSex);
                HelperMain.SetDownSelected(ddlNation, data[0].Nation);
                txtNative.Text = data[0].Native;
                txtEducation.Text = data[0].Education;
                //HelperMain.SetDownSelected(ddlEducation, data[0].Education);
                if (!string.IsNullOrEmpty(data[0].IdCard))
                {
                    if (data[0].IdCard.Length > 18)
                    {
                        txtIdCard.Text = HelperSecret.DESDecrypt(data[0].IdCard, config.IDDESKEY, config.IDDESIV);//des解密
                    }
                    else
                    {
                        txtIdCard.Text = data[0].IdCard;
                    }
                }
                txtPhoto.Text = data[0].Photo;
                if (data[0].Birthday > DateTime.MinValue)
                {
                    txtBirthday.Text = data[0].Birthday.ToString("yyyy-MM-dd");
                }
                if (data[0].PostDate > DateTime.MinValue)
                {
                    txtPostDate.Text = data[0].PostDate.ToString("yyyy-MM-dd");
                }
                HelperMain.SetCheckSelected(cblParty, data[0].Party);
                string strRole = data[0].Role;
                if (!string.IsNullOrEmpty(strRole))
                {
                    HelperMain.SetCheckSelected(cblRole, strRole.Trim(','));
                }
                HelperMain.SetDownSelected(ddlHkMacaoTw, data[0].HkMacaoTw);
                txtCommittee.Text = data[0].Committee;
                txtCommittee2.Text = data[0].Committee2;
                //HelperMain.SetDownSelected(ddlCommittee, data[0].Committee);
                //HelperMain.SetDownSelected(ddlCommittee2, data[0].Committee2);
                HelperMain.SetDownSelected(ddlSubsector, data[0].Subsector);
                //HelperMain.SetDownSelected(ddlSubsector2, data[0].Subsector2);
                HelperMain.SetDownSelected(ddlStreetTeam, data[0].StreetTeam);
                txtOrgName.Text = data[0].OrgName;
                txtOrgPost.Text = data[0].OrgPost;
                HelperMain.SetDownSelected(ddlOrgType, data[0].OrgType);
                txtOrgAddress.Text = data[0].OrgAddress;
                txtOrgZip.Text = data[0].OrgZip;
                txtOrgTel.Text = data[0].OrgTel;
                txtSocietyDuty.Text = data[0].SocietyDuty;
                txtHomeAddress.Text = data[0].HomeAddress;
                txtHomeZip.Text = data[0].HomeZip;
                txtHomeTel.Text = data[0].HomeTel;
                txtContactAddress.Text = data[0].ContactAddress;
                txtMobile.Text = data[0].Mobile;
                //txtEmail.Text = data[0].Email;
                txtWeChat.Text = data[0].WeChat;
                txtCheckText.Text = data[0].CheckText;
                txtRemark.Text = data[0].Remark;
                HelperMain.SetRadioSelected(rblActive, data[0].Active.ToString());
                txtAddTime.Text = data[0].AddTime.ToString("yyyy-MM-dd HH:mm:ss");
                txtAddIp.Text = data[0].AddIp;
                txtAddUser.Text = data[0].AddUser;
                if (!string.IsNullOrEmpty(data[0].UpIp) || !string.IsNullOrEmpty(data[0].UpUser))
                {
                    txtUpTime.Text = data[0].UpTime.ToString("yyyy-MM-dd HH:mm:ss");
                    txtUpIp.Text = data[0].UpIp;
                    txtUpUser.Text = data[0].UpUser;
                }
                if (data[0].LastTime > DateTime.MinValue)
                {
                    txtLastTime.Text = data[0].LastTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                txtLastIp.Text = data[0].LastIp;
                txtErrNum.Text = data[0].ErrNum.ToString();
                if (Request.QueryString["ac"] == "edit")
                {
                    ltTitle.Text = ltTitle.Text.Replace("新增", "修改");
                    btnEdit.Text = "修改";
                    if (data[0].Active > -444)
                    {
                        btnDel.Visible = true;
                    }
                }
                else
                {
                    ltTitle.Text = ltTitle.Text.Replace("新增", "查看");
                    btnEdit.Visible = false;
                }
            }
        }
        //编辑数据
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            string strBack = hfBack.Value;
            string strEditUser = "";
            if (myUser == null)
            {
                //Response.Redirect("login.aspx");
                strBack = "login.aspx?url=" + HttpUtility.UrlEncode(strBack);
                strEditUser = HelperMain.SqlFilter(txtAddUser.Text, 20);
            }
            else
            {
                strEditUser = HelperMain.SqlFilter(myUser.AdminName, 20);
            }
            DataUser data = new DataUser();
            data.Id = Convert.ToInt32(txtId.Text);
            data.OldId = Convert.ToInt32(txtOldId.Text);
            data.UserType = HelperMain.SqlFilter(rblUserType.SelectedValue.Trim(), 20);
            //data.UserName = HelperMain.SqlFilter(txtUserName.Text.Trim(), 50);
            if (!string.IsNullOrEmpty(txtUserPwd.Text))
            {
                data.UserPwd = txtUserPwd.Text.Trim();
            }
            //string strPeriod = HelperMain.GetCheckSelected(cblPeriod);
            //if (!string.IsNullOrEmpty(strPeriod))
            //{
            //    data.Period = HelperMain.SqlFilter(strPeriod);
            //}
            data.Period = config.PERIOD;
            data.UserCode = HelperMain.SqlFilter(txtUserCode.Text.Trim(), 20);
            data.TrueName = HelperMain.SqlFilter(txtTrueName.Text.Trim(), 20);
            DataUser[] ckData = webUser.GetDatas(config.PERIOD, data.TrueName, "Id");//委员姓名，重名检查
            if (ckData != null && ckData[0].Id != data.Id)
            {
                ltInfo.Text = "<script>$(function(){ alert('“委员姓名”重复，不能添加！'); window.history.back(-1); });</script>";
                return;
            }
            data.UserSex = (data.UserType == "委员") ? HelperMain.SqlFilter(rblUserSex.SelectedValue, 2) : "";
            string strNation = HelperMain.SqlFilter(ddlNation.SelectedValue, 8);
            if (strNation.IndexOf("族") > 0)
            {
                strNation = strNation.Replace("族", "");
            }
            data.Nation = strNation;
            data.Native = HelperMain.SqlFilter(txtNative.Text.Trim(), 20);
            data.Education = HelperMain.SqlFilter(txtEducation.Text.Trim(), 20);
            //data.Education = HelperMain.SqlFilter(ddlEducation.SelectedValue.Trim(), 20);
            string strIdCard = HelperMain.SqlFilter(txtIdCard.Text.Replace(" ", ""), 18);
            if (strIdCard != "")
            {
                data.IdCard = HelperSecret.DESEncrypt(strIdCard, config.IDDESKEY, config.IDDESIV);//des加密
            }
            else
            {
                data.IdCard = "";
            }
            data.Photo = HelperMain.SqlFilter(txtPhoto.Text.Trim(), 200);
            if (!string.IsNullOrEmpty(txtBirthday.Text.Trim()))
            {
                data.Birthday = Convert.ToDateTime(txtBirthday.Text.Trim());
            }
            if (!string.IsNullOrEmpty(txtPostDate.Text.Trim()))
            {
                data.PostDate = Convert.ToDateTime(txtPostDate.Text.Trim());
            }
            //data.PostNum = 0;
            data.Party = HelperMain.SqlFilter(HelperMain.GetCheckSelected(cblParty), 50);
            data.HkMacaoTw = HelperMain.SqlFilter(ddlHkMacaoTw.SelectedValue, 20);
            string strRole = HelperMain.SqlFilter(HelperMain.GetCheckSelected(cblRole));
            if (!string.IsNullOrEmpty(strRole))
            {
                strRole = "," + strRole + ",";
            }
            data.Role = strRole;
            data.Committee = HelperMain.SqlFilter(txtCommittee.Text.Trim(), 20);
            data.Committee2 = HelperMain.SqlFilter(txtCommittee2.Text.Trim(), 20);
            //data.Committee = HelperMain.SqlFilter(ddlCommittee.SelectedValue, 20);
            //data.Committee2 = HelperMain.SqlFilter(ddlCommittee2.SelectedValue, 20);
            data.Subsector = HelperMain.SqlFilter(ddlSubsector.SelectedValue, 20);
            //data.Subsector2 = HelperMain.SqlFilter(ddlSubsector2.SelectedValue, 20);
            data.StreetTeam = HelperMain.SqlFilter(ddlStreetTeam.SelectedValue, 20);
            data.OrgName = HelperMain.SqlFilter(txtOrgName.Text.Trim());
            data.OrgPost = HelperMain.SqlFilter(txtOrgPost.Text.Trim(), 20);
            data.OrgType = HelperMain.SqlFilter(ddlOrgType.SelectedValue, 8);
            data.OrgAddress = HelperMain.SqlFilter(txtOrgAddress.Text.Trim());
            data.OrgZip = HelperMain.SqlFilter(txtOrgZip.Text.Trim(), 6);
            data.OrgTel = HelperMain.SqlFilter(txtOrgTel.Text.Trim(), 50);
            data.SocietyDuty = HelperMain.SqlFilter(txtSocietyDuty.Text.Trim(), 50);
            data.HomeAddress = HelperMain.SqlFilter(txtHomeAddress.Text.Trim());
            data.HomeZip = HelperMain.SqlFilter(txtHomeZip.Text.Trim(), 6);
            data.HomeTel = HelperMain.SqlFilter(txtHomeTel.Text.Trim(), 50);
            data.ContactAddress = HelperMain.SqlFilter(txtContactAddress.Text.Trim());
            data.Mobile = HelperMain.SqlFilter(txtMobile.Text.Replace(" ", ""), 50);
            //data.Email = HelperMain.SqlFilter(txtEmail.Text.Trim());
            data.WeChat = HelperMain.SqlFilter(txtWeChat.Text.Trim(), 50);
            string strCheckText = HelperMain.SqlFilter(txtCheckText.Text.Trim().Replace("\r", ""));
            if (!string.IsNullOrEmpty(strCheckText))
            {
                strCheckText = strCheckText.Replace("\r", "");
            }
            data.CheckText = strCheckText;
            data.Remark = HelperMain.SqlFilter(txtRemark.Text.Trim());
            data.Active = Convert.ToInt16(rblActive.SelectedValue);
            DateTime now = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            //data.LastTime = now;
            //data.LastIp = strIp;
            data.ErrNum = Convert.ToInt16(txtErrNum.Text);
            string strOrderScore = HelperMain.GetCheckSelected(cblNoScore);
            if (!string.IsNullOrEmpty(strOrderScore))
            {
                data.OrderScore = Convert.ToInt16(strOrderScore);
            }
            else
            {
                data.OrderScore = 0;
            }
            if (data.Id <= 0)
            {
                string strPwd = "";
                if (!string.IsNullOrEmpty(data.UserPwd))
                {
                    strPwd = data.UserPwd;//设置密码
                }
                else if (!string.IsNullOrEmpty(data.IdCard) && data.IdCard.Length > 6)
                {
                    strPwd = data.IdCard.Substring(data.IdCard.Length - 6);//密码为身份证后6位
                }
                else if (!string.IsNullOrEmpty(data.Mobile) && data.Mobile.Length > 6)
                {
                    strPwd = data.Mobile.Substring(data.Mobile.Length - 6);//密码为手机号后6位
                }
                else
                {
                    strPwd = "123456";//初始密码
                }
                data.UserPwd = HelperSecret.MD5Encrypt(strPwd);
                data.AddTime = now;
                data.AddIp = strIp;
                data.AddUser = strEditUser;
                data.Id = webUser.Insert(data);
            }
            else
            {
                if (!string.IsNullOrEmpty(data.UserPwd))
                {
                    data.UserPwd = HelperSecret.MD5Encrypt(data.UserPwd);//设置密码
                }
                data.UpTime = now;
                data.UpIp = strIp;
                data.UpUser = strEditUser;
                if (webUser.Update(data) <= 0)
                {
                    data.Id = -1;
                }
            }
            if (data.Id > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltTitle.Text + "”成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + ltTitle.Text + "”失败！'); window.history.back(-1); });</script>";
            }
        }
        //删除数据
        protected void btnDel_Click(object sender, EventArgs e)
        {
            int intId = Convert.ToInt32(txtId.Text);
            if (intId <= 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“删除信息”出错！'); window.history.back(-1); });</script>";
                return;
            }
            string strBack = hfBack.Value;
            string strEditUser = "";
            if (myUser == null)
            {
                //Response.Redirect("login.aspx");
                strBack = "login.aspx?url=" + HttpUtility.UrlEncode(strBack);
                strEditUser = HelperMain.SqlFilter(txtAddUser.Text, 20);
            }
            else
            {
                strEditUser = HelperMain.SqlFilter(myUser.AdminName, 20);
            }
            DataUser[] data = webUser.GetData(intId, "TrueName,Active");
            if (data == null)
            {
                ltInfo.Text = "<script>$(function(){ alert('未查到“委员信息”，无法删除！'); window.history.back(-1); });</script>";
                return;
            }
            else if (data[0].Active <= -444)
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + data[0].TrueName + "”的信息已经删除了！'); window.location.replace('" + strBack + "'); });</script>";
                return;
            }
            if (webUser.UpdateActive(intId, -444) > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + data[0].TrueName + "”的信息，删除成功！'); window.location.href='" + strBack + "'; });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“" + data[0].TrueName + "”的信息，删除失败！'); window.history.back(-1); });</script>";
            }
        }
        #endregion
        //
        #region 委员积分
        private void listScore(int UserId)
        {
            if (myUser.AdminName == "Tony")
            {
                plAddScore.Visible = true;
            }
            DataUser[] uData = webUser.GetData(UserId, "UserCode,TrueName,AddTime");
            if (uData == null)
            {
                return;
            }
            DateTime dtNow = DateTime.Now;
            string strGetTimeText = "";
            if (!string.IsNullOrEmpty(Request.QueryString["time"]))
            {
                strGetTimeText = Request.QueryString["time"];
            }
            else
            {
                strGetTimeText = string.Format("{0:yyyy}-01-01,{0:yyyy}-12-31", dtNow);
            }
            int addYear = uData[0].AddTime.Year;
            int nowYear = DateTime.Today.Year;
            for (int i = addYear; i <= nowYear; i++)
            {
                ListItem item = new ListItem(string.Format("{0}年度", i), string.Format("{0}-01-01,{0}-12-31", i));
                ddlYear.Items.Add(item);
            }
            HelperMain.SetDownSelected(ddlYear, strGetTimeText);
            WebUserScore webScore = new WebUserScore();
            int ScoreId = (!string.IsNullOrEmpty(Request.QueryString["sid"])) ? Convert.ToInt32(Request.QueryString["sid"]) : 0;
            if (ScoreId > 0 && !string.IsNullOrEmpty(Request.QueryString["edit"]))
            {
                int ScoreActive = Convert.ToInt16(Request.QueryString["edit"]);
                webScore.UpdateActive(ScoreId, ScoreActive);//修改积分状态
            }
            ltUserCode.Text = uData[0].UserCode;
            ltTrueName.Text = uData[0].TrueName;
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            //string strGetTimeText = string.Format("{0}-01-01,{0}-12-31", DateTime.Now.Year);
            DataUserScore[] data = webScore.GetDatas(0, UserId.ToString(), "", 0, "", strGetTimeText, "", pageCur, pageSize, "GetTime DESC", "total");//委员积分列表
            if (data != null)
            {
                string strStart = strGetTimeText.Substring(0, strGetTimeText.IndexOf(","));
                string strEnd = strGetTimeText.Substring(strGetTimeText.IndexOf(",") + 1);
                //int intYear = DateTime.Today.Year;
                //string strStart = intYear.ToString() + "-1-1";
                //string strEnd = intYear.ToString() + "-12-31";
                decimal deScoreTotal = webScore.GetTotalScore(UserId, strStart, strEnd);//累计积分
                decimal deScore2 = webScore.GetTotalScore(UserId, strStart, strEnd, "tb_Opinion,tb_Opinion_Pop");//建言得分
                ltScore.Text = deScoreTotal.ToString("n2");
                ltScore1.Text=(deScoreTotal-deScore2).ToString("n2");
                ltScore2.Text = deScore2.ToString("n2");
                WebOpinion webOpin = new WebOpinion();
                WebOpinionPop webPop = new WebOpinionPop();
                WebReport webReport = new WebReport();
                WebPerform webPerform = new WebPerform();
                WebPerformFeed webFeed = new WebPerformFeed();
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = (pageCur - 1) * pageSize + i + 1;
                    data[i].other = PublicMod.GetScoreOther(data[i], webOpin, webPop, webReport, webPerform, webFeed);
                    if (!string.IsNullOrEmpty(data[i].Remark))
                    {
                        if (!string.IsNullOrEmpty(data[i].other))
                        {
                            data[i].other += "<br/>";
                        }
                        data[i].other += data[i].Remark;
                    }
                    data[i].ScoreText = data[i].Score.ToString("n2");
                    if (data[i].Active > 0)
                    {
                        //data[i].ActiveName = "正常";
                        data[i].BtnEdit = string.Format("<a href='?ac=score&id={0}&page={1}&edit=-1&sid={2}' class='btn'><u>取消</u></a>", UserId, pageCur, data[i].Id);
                    }
                    else if (data[i].Active <= -444)
                    {
                        data[i].rowClass = " class='del' title='删除'";
                    }
                    else
                    {
                        data[i].rowClass = " class='cancel' title='取消'";
                        data[i].BtnEdit = string.Format("<a href='?ac=score&id={0}&page={1}&edit=1&sid={2}' class='btn'><u>启用</u></a>", UserId, pageCur, data[i].Id);
                    }
                    //if (myUser.Grade < 9)
                    //{
                    //    data[i].BtnEdit = "";
                    //}
                }
                rpScoreList.DataSource = data;
                rpScoreList.DataBind();
                ltScoreNo.Visible = false;
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = Request.Url.ToString();
                lblScoreNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
                ltScoreTotal.Text = pageCount.ToString();
            }
        }
        //新增积分
        protected void btnAddScore_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            int UserId = (!string.IsNullOrEmpty(Request.QueryString["id"])) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
            decimal deScore = (!string.IsNullOrEmpty(txtScore.Text)) ? Convert.ToDecimal(txtScore.Text) : 0;
            string strTitle = HelperMain.SqlFilter(txtScoreTitle.Text.Trim(), 50);
            if (UserId > 0 && deScore != 0 && !string.IsNullOrEmpty(strTitle))
            {
                string strRemark = HelperMain.SqlFilter(txtScoreRemark.Text.Trim());
                string strIp = HelperMain.GetIpPort();
                string strUser = myUser.AdminName;
                DateTime getTime = (!string.IsNullOrEmpty(txtScoreTime.Text.Trim())) ? Convert.ToDateTime(txtScoreTime.Text) : DateTime.Now;
                int intId = PublicMod.AddScore(UserId, strTitle, deScore, "other", 0, strIp, strUser, getTime, strRemark);
                if (intId > 0)
                {
                    ltInfo.Text = "<script> $(function(){ alert('“" + btnAddScore.Text + "”成功！');window.location.replace('" + Request.Url.ToString() + "'); });</script>";
                }
                else
                {
                    ltInfo.Text = "<script> $(function(){ alert('“" + btnAddScore.Text + "”失败！'); window.history.back(-1); });</script>";
                }
            }
        }
        //新增积分（批量）
        protected void btnScoreAdd_Click(object sender, EventArgs e)
        {
            if (myUser == null || myUser.Grade < 9 || myUser.Powers.IndexOf("alls") < 0)
            {
                return;
            }
            string strUsers = HelperMain.SqlFilter(sUsers.Text.Replace(" ", ""));
            decimal deScore = (!string.IsNullOrEmpty(sScore.Text)) ? Convert.ToDecimal(sScore.Text) : 0;
            string strTitle = HelperMain.SqlFilter(sScoreTitle.Text.Trim(), 50);
            if (string.IsNullOrEmpty(strUsers) || deScore == 0 || string.IsNullOrEmpty(strTitle))
            {
                return;
            }
            string strUserOk = "";
            string strUserErr = "";
            string strUserNot = "";
            string strRemark = HelperMain.SqlFilter(sScoreRemark.Text.Trim());
            string strIp = HelperMain.GetIpPort();
            string strUser = myUser.AdminName;
            DateTime getTime = (!string.IsNullOrEmpty(sScoreTime.Text.Trim())) ? Convert.ToDateTime(sScoreTime.Text) : DateTime.Now;
            string[] arr = strUsers.Split(',');
            for (int i = 0; i < arr.Count(); i++)
            {
                if (!string.IsNullOrEmpty(arr[i]))
                {
                    DataUser[] data = webUser.GetDatas(config.PERIOD, arr[i], "Id");
                    if (data != null)
                    {
                        int UserId = data[0].Id;
                        int intId = PublicMod.AddScore(UserId, strTitle, deScore, "other", 0, strIp, strUser, getTime, strRemark);
                        if (intId > 0)
                        {
                            if (!string.IsNullOrEmpty(strUserOk))
                            {
                                strUserOk += ",";
                            }
                            strUserOk += arr[i];
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(strUserErr))
                            {
                                strUserErr += ",";
                            }
                            strUserErr += arr[i];
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(strUserNot))
                        {
                            strUserNot += ",";
                        }
                        strUserNot += arr[i];
                    }
                }
            }
            lblScoreAdd.Text = "";
            if (!string.IsNullOrEmpty(strUserOk))
            {
                //ltInfo.Text = "<script> $(function(){ alert('“" + btnScoreAdd.Text + "”成功！');window.location.replace('" + Request.Url.ToString() + "'); });</script>";
                lblScoreAdd.Text += "新增积分委员：<br/>" + strUserOk + "<hr/>";
            }
            else
            {
                //ltInfo.Text = "<script> $(function(){ alert('“" + btnScoreAdd.Text + "”失败！'); window.history.back(-1); });</script>";
            }
            if (!string.IsNullOrEmpty(strUserErr))
            {
                lblScoreAdd.Text += "新增失败委员：<br/>" + strUserErr + "<hr/>";
            }
            if (!string.IsNullOrEmpty(strUserNot))
            {
                lblScoreAdd.Text += "非委员用户：<br/>" + strUserNot + "<hr/>";
            }
        }
        #endregion
        //
    }
}