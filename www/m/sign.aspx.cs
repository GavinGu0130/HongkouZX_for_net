using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mod.main;
using hkzx.db;
using hkzx.user;
using WxAPI;

namespace hkzx.web.m
{
    public partial class sign : System.Web.UI.Page
    {
        protected string wxJsSdkParam = "";
        private const string APPID = config.APPID;
        private const string APPSECRET = config.APPSECRET;
        private const string TOKEN = config.TOKEN;
        private const string TICKET = config.TICKET;
        private const string SIGNDESKEY = config.SIGNDEKEY;
        private const string SIGNDESIV = config.SIGNDESIV;
        private DataAdmin myUser = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["ac"] == "perform")
            {
                if (JsApi.GetIsWx(5))
                {
                    JsApi wxApi = new JsApi();
                    wxJsSdkParam = wxApi.GetJssdkParameters(APPID, APPSECRET, TOKEN, TICKET);//获取H5调起JS API参数
                }
                else if (Request.IsLocal)
                {

                }
                else
                {
                    ltInfo.Text = "<script>$(function(){ alert('请使用“微信客户端”访问！'); });</script>";
                    return;
                }
                plHeader.Visible = true;
                myUser = HelperAdmin.GetUser();
                if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
                {
                    //Response.Redirect("../admin/login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                    return;
                }
                ltUser.Text = myUser.AdminName;
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    listSign(Convert.ToInt32(Request.QueryString["id"]));
                }
                else
                {
                    listPerform();
                }
            }
            else
            {
                form1.Visible = false;
                if (!string.IsNullOrEmpty(Request.QueryString["pid"]))
                {
                    loadSign();//文件签到
                }
                else
                {
                    deskSign();//人脸签到
                }
                Response.End();
            }
        }
        //
        #region 人脸签到
        //token：DeskId=UserCode=SignTime（DES加密）
        private void deskSign()
        {
            if (string.IsNullOrEmpty(Request.QueryString["token"]))
            {
                Response.Write("err");
                Response.End();
                return;
            }
            string strToken = HelperSecret.DESDecrypt(Request.QueryString["token"], SIGNDESKEY, SIGNDESIV);
            //Response.Write(strToken);
            if (strToken.IndexOf("=") < 0)
            {
                Response.Write("err");
                return;
            }
            string[] arr = strToken.Split('=');
            if (arr.Count() != 3)
            {
                Response.Write("err");
                return;
            }
            string strDeskCode = arr[0];
            string strUserCode = arr[1];
            string strSignTime = arr[2];
            if (string.IsNullOrEmpty(strDeskCode) || string.IsNullOrEmpty(strUserCode) || string.IsNullOrEmpty(strSignTime))
            {
                Response.Write("err");
                return;
            }
            int PerformId = 0;
            int UserId = 0;
            string TrueName = "";
            WebUser webUser = new WebUser();
            DataUser[] uData = webUser.GetData(strUserCode, "Id,TrueName");
            if (uData != null)
            {
                UserId = uData[0].Id;
                TrueName = uData[0].TrueName;
            }
            else
            {
                Response.Write("委员编号错误！");
                return;
            }
            WebPerform webPerform = new WebPerform();
            DataPerform qData = new DataPerform();
            qData.ActiveName = "发布";
            qData.Attendees = TrueName;
            qData.SignDesk = strDeskCode;
            //时间戳起始时间：1970-01-01 08:00:00
            DateTime SignTime = new DateTime(1970, 1, 1, 8, 0, 0).AddSeconds(Convert.ToInt32(strSignTime));//Convert.ToDateTime(strSignTime);//DateTime.Now;
            //Response.Write(SignTime.ToString("yyyy-MM-dd HH:mm:ss")); Response.End(); return;
            qData.SignTimeText = "," + SignTime.ToString("yyyy-MM-dd HH:mm:ss");
            qData.EndTimeText = SignTime.ToString("yyyy-MM-dd HH:mm:ss") + ",";
            DataPerform[] pData = webPerform.GetDatas(qData, "Id,SubType,SignDesk");
            if (pData != null)
            {
                PerformId = pData[0].Id;
            }
            else
            {
                Response.Write("未查找到签到的履职活动！");
                return;
            }
            WebPerformFeed webFeed = new WebPerformFeed();
            DataPerformFeed[] fData = webFeed.GetDatas("", PerformId, UserId, TrueName, "Id,SignManType,IsMust,ActiveName");
            if (fData == null)
            {
                Response.Write("没有查询到您的签到信息！");
                return;
            }
            switch (fData[0].ActiveName)
            {
                case "已签到":
                case "报名已签到":
                case "未报名已签到":
                    Response.Write("您已签到了！");
                    return;
                case "关闭":
                    Response.Write("您的状态，不能进行签到！");
                    return;
                default:
                    break;
            }
            string strIp = HelperMain.GetIpPort();
            string strActive = "";
            if (fData[0].IsMust == "必须参加" || fData[0].ActiveName == "参加")
            {
                strActive = "已签到";
            }
            else
            {
                strActive = "已签到(未报名)";
            }
            if (webFeed.UpdateSign(fData[0].Id, strActive, SignTime, strIp, strDeskCode) > 0)
            {
                addScore(pData[0].SubType, strIp, strDeskCode, fData[0].SignManType, UserId, fData[0].Id, SignTime, strActive);//增加积分
                Response.Write("签到成功");
            }
            else
            {
                Response.Write("签到出错！");
            }
        }
        //增加签到积分
        private void addScore(string PerformSubType, string strIp, string strUser, string SignManType, int UserId, int FeedId, DateTime SignTime, string strAcite)
        {
            WebScore webScore = new WebScore();
            decimal deAddScore = 0;
            decimal deAddScore2 = 0;
            string strSubType = PerformSubType;
            if (!string.IsNullOrEmpty(strSubType))
            {
                if (strSubType.IndexOf("-") > 0)
                {
                    strSubType = strSubType.Substring(0, strSubType.IndexOf("-"));
                }
                DataScore[] sData = webScore.GetDatas(1, "", "", strSubType, "Score,Score2,Title");//取活动加分项
                if (sData != null)
                {
                    deAddScore = sData[0].Score;
                    deAddScore2 = sData[0].Score2;
                }
            }else{
                return;
            }
            string strAddTitle = "出席-" + PerformSubType;
            string strTableName = "tb_Perform_Feed";
            //string strIp = HelperMain.GetIpPort();
            //string strUser = strDeskCode;
            if (PerformSubType == "常委会议")
            {
                if (SignManType == "常委")
                {
                    PublicMod.AddScore(UserId, strAddTitle + "-常委", deAddScore2, strTableName, FeedId, strIp, strUser, SignTime);
                }
                else
                {
                    PublicMod.AddScore(UserId, strAddTitle + "-列席", deAddScore, strTableName, FeedId, strIp, strUser, SignTime);
                }
            }
            else if (PerformSubType.IndexOf("讲堂") > 0)
            {
                if (SignManType == "主讲人")
                {
                    PublicMod.AddScore(UserId, strAddTitle + "-主讲人", deAddScore2, strTableName, FeedId, strIp, strUser, SignTime);
                }
                else
                {
                    PublicMod.AddScore(UserId, strAddTitle + "-出席", deAddScore, strTableName, FeedId, strIp, strUser, SignTime);
                }
            }
            else
            {
                if (strAcite == "已签到(未报名)")
                {
                    deAddScore = deAddScore / 2;
                }
                PublicMod.AddScore(UserId, strAddTitle, deAddScore, strTableName, FeedId, strIp, strUser, SignTime);//签到积分
            }
        }
        #endregion
        //
        #region 管理员扫用户二维码签到（暂不使用）
        //加载履职活动列表
        private void listPerform()
        {
            plPerform.Visible = true;
            WebPerform webPerform = new WebPerform();
            DataPerform qData = new DataPerform();
            qData.ActiveName = "审核通过";
            qData.EndTimeText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",";
            DataPerform[] data = webPerform.GetDatas(qData, "Id,SubType,Title,StartTime,EndTime,PerformSite");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = i + 1;
                    data[i].PerformTimeText = PublicMod.GetTimeText(data[i].StartTime, data[i].EndTime);
                }
                rpPerformList.DataSource = data;
                rpPerformList.DataBind();
            }
        }
        //加载履职签到信息
        private void listSign(int PerformId)
        {
            if (PerformId <= 0)
            {
                return;
            }
            WebPerform webPerform = new WebPerform();
            DataPerform[] pData = webPerform.GetData(PerformId, "SubType,Title,StartTime,EndTime,PerformSite");
            if (pData == null)
            {
                return;
            }
            ltPerformSubType.Text = pData[0].SubType;
            ltPerformTitle.Text = pData[0].Title;
            ltPerformTime.Text = PublicMod.GetTimeText(pData[0].StartTime, pData[0].EndTime);
            ltPerformSite.Text = pData[0].PerformSite;
            hfPerformId.Value = HelperSecret.DESEncrypt(PerformId.ToString(), config.IDDESKEY, config.IDDESIV);

            plSign.Visible = true;
            WebPerformFeed webFeed = new WebPerformFeed();
            DataPerformFeed[] data = webFeed.GetDatas("", PerformId, 0, "");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    if (data[i].SignTime > DateTime.MinValue)
                    {
                        data[i].SignTimeText = data[i].SignTime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
                rpSignList.DataSource = data;
                rpSignList.DataBind();
                ltSignNum.Text = data.Count().ToString();
            }
        }
        #endregion
        //
        #region 文件签到（暂不使用）
        //pid：PerformId（DES加密）
        //token：UserId=TrueName=datetime（DES加密）
        private void loadSign()
        {
            if (string.IsNullOrEmpty(Request.QueryString["pid"]) || Request.QueryString["pid"].Length < 16 || string.IsNullOrEmpty(Request.QueryString["token"]))
            {
                Response.Write("err");
                return;
            }
            string strPid = HelperSecret.DESDecrypt(Request.QueryString["pid"], SIGNDESKEY, SIGNDESIV);
            int PerformId = (!string.IsNullOrEmpty(strPid)) ? Convert.ToInt32(strPid) : 0;
            string strToken = HelperSecret.DESDecrypt(Request.QueryString["token"], SIGNDESKEY, SIGNDESIV);
            if (PerformId <= 0 || string.IsNullOrEmpty(strToken) || strToken.IndexOf("=") < 0)
            {
                Response.Write("err");
                return;
            }
            //Response.Write("debug: " + strToken);
            string[] arr = strToken.Split('=');
            if (arr.Count() != 3)
            {
                Response.Write("err");
                return;
            }
            int UserId = Convert.ToInt32(arr[0]);
            string TrueName = arr[1];
            string strTime = arr[2];
            if (UserId < 0 || TrueName == "" || strTime == "" || strTime.IndexOf(":") < 0)
            {
                Response.Write("err");// + strToken
                return;
            }
            DateTime dtTime = Convert.ToDateTime(strTime);
            if (dtTime < DateTime.Now)
            {
                Response.Write("二维码已过期！");
                return;
            }
            WebPerform webPerform = new WebPerform();
            DataPerform[] pData = webPerform.GetData(PerformId, "Id,SubType,EndTime,ActiveName");
            if (pData == null)
            {
                Response.Write("没有查询到履职活动！");
                return;
            }
            if (pData[0].ActiveName != "审核通过" || pData[0].EndTime < DateTime.Now)
            {
                Response.Write("活动已关闭，不能进行签到！");
                return;
            }
            //UserId = 0;
            WebPerformFeed webFeed = new WebPerformFeed();
            DataPerformFeed[] fData = webFeed.GetDatas("", PerformId, UserId, TrueName, "Id,SignManType,ActiveName");
            if (fData == null)
            {
                Response.Write("没有查询到您的签到信息！");
                return;
            }
            switch (fData[0].ActiveName)
            {
                case "已签到2":
                case "已签到":
                    Response.Write("您已签到了！");
                    return;
                case "关闭":
                    Response.Write("您的状态，不能进行签到！");
                    return;
                default:
                    break;
            }
            string strIp = HelperMain.GetIpPort();
            string strDesk = (!string.IsNullOrEmpty(Request.QueryString["desk"])) ? HelperMain.SqlFilter(Request.QueryString["desk"].Trim(), 20) : "";
            if (webFeed.UpdateSign(fData[0].Id, "已签到", DateTime.MinValue, strIp, strDesk) > 0)
            {
                addScore(pData[0].SubType, strIp, strDesk, fData[0].SignManType, UserId, fData[0].Id, DateTime.Now, "已签到");//新增积分
                Response.Write("签到成功");
            }
            else
            {
                Response.Write("签到出错！");
            }
        }
        #endregion
        //
    }
}