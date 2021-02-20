using System;
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
    public partial class ads : System.Web.UI.Page
    {
        private DataAdmin myUser = null;
        private WebAdmin webAdmin = new WebAdmin();
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperAdmin.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
            {
                //Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.Powers = myUser.Powers;
            if (!IsPostBack)
            {
                string strTitle = "";
                switch (Request.QueryString["ac"])
                {
                    case "manage":
                        if (myUser.Grade < 9 || (myUser.Powers.IndexOf("alls") < 0 && myUser.Powers.IndexOf("system") < 0))
                        {
                            Response.Redirect("./");
                        }
                        plNav.Visible = true;
                        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                        {
                            hfBack.Value = PublicMod.GetBackUrl();
                            strTitle = "编辑后台用户";
                            plEdit.Visible = true;
                            loadData(Convert.ToInt32(Convert.ToInt32(Request.QueryString["id"])));
                        }
                        else
                        {
                            strTitle = "后台用户管理";
                            plList.Visible = true;
                            listData();
                        }
                        break;
                    case "pwd":
                        strTitle = "修改密码";
                        hfBack.Value = PublicMod.GetBackUrl("./");
                        plPwd.Visible = true;
                        txtName.Text = myUser.AdminName;
                        break;
                    default:
                        strTitle = "我的信息";
                        hfBack.Value = PublicMod.GetBackUrl("./");
                        plInfo.Visible = true;
                        loadMy();
                        break;
                }
                Header.Title += " - " + strTitle;
            }
        }
        //
        #region 用户自行管理
        //修改密码
        protected void btnPwd_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            string strOld = txtOld.Text.Trim();
            string strNew = txtNew.Text.Trim();
            if (string.IsNullOrEmpty(strOld) || string.IsNullOrEmpty(strNew))
            {
                ltInfo.Text = "<script>$(function(){ alert('“新密码”和“旧密码”都不能为空！'); });</script>";
                return;
            }
            else if (strOld == strNew)
            {
                ltInfo.Text = "<script>$(function(){ alert('“新密码”和“旧密码”不能一样！'); });</script>";
                return;
            }
            string strErr = HelperMain.ChkPwd(strNew, 6, 20, 2);
            if (!string.IsNullOrEmpty(strErr))
            {
                ltInfo.Text = "<script>$(function(){ alert('" + strErr + "'); });</script>";
                return;
            }
            string strOldMd5 = strOld;//HelperSecret.MD5Encrypt(strOld);
            string strNewMd5 = HelperSecret.MD5Encrypt(strNew);
            if (webAdmin.SetUserPwd(myUser.Id, strNewMd5, strOldMd5) > 0)
            {
                ltInfo.Text = "<script>$(function(){ alert('“密码”修改成功！'); window.location.replace('" + hfBack.Value + "'); });</script>";
            }
            else
            {
                ltInfo.Text = "<script>$(function(){ alert('“旧密码”错误！'); });</script>";
            }
        }
        //用户信息
        private void loadMy()
        {
            DataAdmin[] data = webAdmin.GetData(myUser.Id);
            if (data != null)
            {
                ltId.Text = data[0].Id.ToString();
                ltAdminName.Text = data[0].AdminName;
                ltTrueName.Text = data[0].TrueName;
                ltUserSex.Text = data[0].UserSex;
                if (!string.IsNullOrEmpty(data[0].IdCard))
                {
                    ltIdCode.Text = HelperSecret.DESDecrypt(data[0].IdCard, config.IDDESKEY, config.IDDESIV);//des解密
                }
                if (!string.IsNullOrEmpty(data[0].Photo))
                {
                    ltPhoto.Text = string.Format("<img src='{0}' />", data[0].Photo);
                }
                ltGrade.Text = HelperMain.GetRadioText(rblGrade, data[0].Grade.ToString());
                ltPowers.Text = HelperMain.GetCheckText(cblPowers, data[0].Powers);
                ltDepPost.Text = data[0].DepPost;
                ltOfficeTel.Text = data[0].OfficeTel;
                ltMobile.Text = data[0].Mobile;
                ltEmail.Text = data[0].Email;
                //data[0].WeChat
                ltRemark.Text = data[0].Remark;
                ltActive.Text = HelperMain.GetRadioText(rblActive, data[0].Active.ToString());
                if (data[0].LastTime > DateTime.MinValue)
                {
                    ltLastTime.Text = data[0].LastTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                ltLastIp.Text = data[0].LastIp;
                ltErrNum.Text = data[0].ErrNum.ToString();
            }
        }
        #endregion
        //
        #region 后台管理
        //加载搜索查询条件
        private DataAdmin getQData()
        {
            DataAdmin data = new DataAdmin();
            if (!string.IsNullOrEmpty(Request.QueryString["UserName"]))
            {
                data.AdminName = "%" + HelperMain.SqlFilter(Request.QueryString["UserName"].Trim(), 20) + "%";
                txtQUserName.Text = data.AdminName.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["TrueName"]))
            {
                data.TrueName = "%" + HelperMain.SqlFilter(Request.QueryString["TrueName"].Trim(), 20) + "%";
                txtQTrueName.Text = data.TrueName.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Mobile"]))
            {
                data.Mobile = "%" + HelperMain.SqlFilter(Request.QueryString["Mobile"].Trim(), 20) + "%";
                txtQMobile.Text = data.Mobile.Trim('%');
            }
            if (!string.IsNullOrEmpty(Request.QueryString["DepPost"]))
            {
                data.DepPost = "%" + HelperMain.SqlFilter(Request.QueryString["DepPost"].Trim(), 20) + "%";
                txtQDepPost.Text = data.DepPost.Trim('%');
            }
            return data;
        }
        //加载列表
        private void listData()
        {
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = 10;
            DataAdmin qData = getQData();
            string strFields = "";//"Id,UserCode,TrueName,Birthday,Committee,Committee2,Subsector,Subsector2";//
            DataAdmin[] data = webAdmin.GetDatas(qData, strFields, pageCur, pageSize, "", "total");
            if (data != null)
            {
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
                        data[i].rowClass = " class='cancel'";
                    }
                    else
                    {
                        data[i].ActiveName = "锁定";
                        data[i].rowClass = " class='save'";
                    }
                    if (data[i].LastTime > DateTime.Today)
                    {
                        data[i].LastTimeText = data[i].LastTime.ToString("HH:mm:ss");
                    }
                    else if (data[i].LastTime > DateTime.MinValue)
                    {
                        data[i].LastTimeText = data[i].LastTime.ToString("yyyy-MM-dd");
                    }
                }
                rpList.DataSource = data;
                rpList.DataBind();
                int pageCount = data[0].total;
                int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                string lnk = Request.Url.ToString();
                lblNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
                ltTotal.Text = pageCount.ToString();
                ltNo.Visible = false;
            }
        }
        //加载数据
        private void loadData(int Id)
        {
            if (Id <= 0)
            {
                txtAddTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                txtAddIp.Text = HelperMain.GetIp();
                txtAddUser.Text = myUser.AdminName;
                return;
            }
            DataAdmin[] data = webAdmin.GetData(Id);
            if (data != null)
            {
                txtId.Text = data[0].Id.ToString();
                txtAdminName.Text = data[0].AdminName;
                txtTrueName.Text = data[0].TrueName;
                HelperMain.SetRadioSelected(rblUserSex, data[0].UserSex);
                if (!string.IsNullOrEmpty(data[0].IdCard))
                {
                    txtIdCode.Text = HelperSecret.DESDecrypt(data[0].IdCard, config.IDDESKEY, config.IDDESIV);//des解密
                }
                txtPhoto.Text = data[0].Photo;
                HelperMain.SetRadioSelected(rblGrade, data[0].Grade.ToString());
                HelperMain.SetCheckSelected(cblPowers, data[0].Powers);
                txtDepPost.Text = data[0].DepPost;
                txtOfficeTel.Text = data[0].OfficeTel;
                txtMobile.Text = data[0].Mobile;
                txtEmail.Text = data[0].Email;
                //data[0].WeChat
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
                btnEdit.Text = "修改";
                ltTitle.Text = ltTitle.Text.Replace("新增", "修改");
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
            DataAdmin data = new DataAdmin();
            data.Id = Convert.ToInt32(txtId.Text);
            data.AdminName = HelperMain.SqlFilter(txtAdminName.Text.Trim(), 20);
            data.TrueName = HelperMain.SqlFilter(txtTrueName.Text.Trim(), 20);
            DataAdmin[] ckData = webAdmin.GetDatas(data.AdminName, "Id");//登录名，重名检查
            if (ckData != null && ckData[0].Id != data.Id)
            {
                ltInfo.Text = "<script>$(function(){ alert('“登录名”重复，不能添加！'); window.history.back(-1); });</script>";
                return;
            }
            if (!string.IsNullOrEmpty(txtAdminPwd.Text))
            {
                data.AdminPwd = txtAdminPwd.Text.Trim();
            }
            data.UserSex = HelperMain.SqlFilter(rblUserSex.SelectedValue, 2);
            string strIdCard = HelperMain.SqlFilter(txtIdCode.Text.Replace(" ", ""), 18);
            if (strIdCard != "")
            {
                data.IdCard = HelperSecret.DESEncrypt(strIdCard, config.IDDESKEY, config.IDDESIV);//des加密
            }
            data.Photo = HelperMain.SqlFilter(txtPhoto.Text.Trim(), 200);
            data.Grade = Convert.ToInt16(rblGrade.SelectedValue);
            data.Powers = HelperMain.SqlFilter(HelperMain.GetCheckSelected(cblPowers));
            if (data.Powers.StartsWith("alls") && data.Powers.IndexOf(",") > 0)
            {
                data.Powers = "alls";
            }
            data.DepPost = HelperMain.SqlFilter(txtDepPost.Text.Trim(), 20);
            data.OfficeTel = HelperMain.SqlFilter(txtOfficeTel.Text.Trim(), 50);
            data.Mobile = HelperMain.SqlFilter(txtMobile.Text.Replace(" ", ""), 50);
            data.Email = HelperMain.SqlFilter(txtEmail.Text.Trim(), 200);
            //data.WeChat = "";
            data.Remark = HelperMain.SqlFilter(txtRemark.Text.Trim());
            data.Active = Convert.ToInt16(rblActive.SelectedValue);
            DateTime now = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            //data.LastTime = now;
            //data.LastIp = strIp;
            data.ErrNum = Convert.ToInt16(txtErrNum.Text);
            if (data.Id <= 0)
            {
                string strPwd = "";
                if (!string.IsNullOrEmpty(data.AdminPwd))
                {
                    strPwd = data.AdminPwd;//设置密码
                }
                else if (!string.IsNullOrEmpty(data.IdCard) && data.IdCard.Length > 6)
                {
                    strPwd = data.IdCard.Substring(data.IdCard.Length - 6);//密码为身份证后6位
                }
                else if (!string.IsNullOrEmpty(data.Mobile) && data.Mobile.Length > 6)
                {
                    strPwd = data.IdCard.Substring(data.Mobile.Length - 6);//密码为手机号后6位
                }
                else
                {
                    strPwd = "123456";//初始密码
                }
                data.AdminPwd = HelperSecret.MD5Encrypt(strPwd);
                data.AddTime = now;
                data.AddIp = strIp;
                data.AddUser = strEditUser;
                data.Id = webAdmin.Insert(data);
            }
            else
            {
                if (!string.IsNullOrEmpty(data.AdminPwd))
                {
                    data.AdminPwd = HelperSecret.MD5Encrypt(data.AdminPwd);//设置密码
                }
                data.UpTime = now;
                data.UpIp = strIp;
                data.UpUser = strEditUser;
                if (webAdmin.Update(data) <= 0)
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
        #endregion
        //
    }
}