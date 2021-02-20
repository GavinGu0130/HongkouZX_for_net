using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WxAPI;
using mod.main;
using hkzx.db;
using hkzx.user;

namespace hkzx.web.m
{
    public partial class wx : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (JsApi.GetIsWx(5))
            {
                loadWx();
            }
            else
            {
                ltInfo.Text = "<div>请用微信访问本页面！<br/><a href='./'>返回</a></div>";
            }
        }
        //检查用户微信信息
        private void loadWx()
        {
            string strUrl = Request.QueryString["url"];
            if (string.IsNullOrEmpty(strUrl) || strUrl.IndexOf("login.aspx") >= 0)
            {
                strUrl = "../m/";
            }
            string state = Request.QueryString["ac"];
            if (state == "bind")
            {
                DataUser myUser = HelperUser.GetUser();
                if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.TrueName))
                {
                    Response.Redirect("../cn/login.aspx?url=" + strUrl);
                    return;
                }
                WebUserWx webUserWx = new WebUserWx();
                DataUserWx[] user = webUserWx.GetDatas(config.APPID, myUser.Id);//取已绑定微信openid
                if (user != null && !string.IsNullOrEmpty(user[0].WxOpenId))
                {
                    ltInfo.Text = "<script>$(function(){ alert('当前用户已绑定微信！\\n如需更改，请先解绑微信，再重新绑定。'); window.history.back(-1); });</script>";
                    return;
                }
            }

            string openid = getWx(state, strUrl);//获取微信信息
            if (!string.IsNullOrEmpty(openid))
            {
                if (state == "login")
                {
                    if (!string.IsNullOrEmpty(openid))
                    {
                        wxLogin(openid, strUrl);
                    }
                }
                else if (state == "bind")
                {
                    DataUser myUser = HelperUser.GetUser();
                    if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.TrueName))
                    {
                        Response.Redirect("../cn/login.aspx?url=" + strUrl);
                        return;
                    }
                    WebUserWx webUserWx = new WebUserWx();
                    DataUserWx[] user = webUserWx.GetDatas(config.APPID, 0, openid);//取已绑定微信openid的用户Id
                    if (user != null && user[0].UserId > 0)
                    {
                        ltInfo.Text = "<script>$(function(){ alert('当前微信已绑定其他用户！\\n如需更改，请先解绑微信，再重新绑定。'); window.location.replace('" + strUrl + "'); });</script>";
                    }
                    else if (webUserWx.UpdateUserId(myUser.Id, config.APPID, openid) > 0)
                    {
                        ltInfo.Text = "<script>$(function(){ alert('绑定微信成功！'); window.location.replace('" + strUrl + "'); });</script>";
                    }
                    //string strUrl = "../cn/login.aspx?ac=wx";
                    //if (!string.IsNullOrEmpty(Request.QueryString["url"]))
                    //{
                    //    strUrl += "&url=" + Request.QueryString["url"];
                    //}
                    //Response.Redirect(strUrl);
                }
                else if (state == "unbinded")
                {
                    WebUserWx webUserWx = new WebUserWx();
                    if (webUserWx.UpdateUserId(0, config.APPID, openid) > 0)
                    {
                    }
                    Response.Redirect(strUrl);
                }
            }
        }
        //微信登录
        private void wxLogin(string openid, string strUrl)
        {
            if (!string.IsNullOrEmpty(openid))
            {
                WebUserWx webUserWx = new WebUserWx();
                DataUserWx[] user = webUserWx.GetDatas(config.APPID, 0, openid);
                if (user != null && user[0].WxAccess_expires_in.AddSeconds(7000) >= DateTime.Now && user[0].UserId > 0)
                {
                    WebUser webUser = new WebUser();
                    DataUser[] data = webUser.GetData(user[0].UserId);
                    if (data == null)
                    {

                    }
                    else if (data[0].ErrNum > 10 || data[0].Active <= 0)
                    {
                        ltInfo.Text = "<script>$(function(){ alert('“密码”错误次数过多，\\n或“帐户”已被锁定！'); window.location.replace('../cn/login.aspx?url=" + strUrl + "'); });</script>";
                        return;
                    }
                    else
                    {
                        data[0].LastIp = HelperMain.GetIpPort();
                        data[0].LastTime = DateTime.Now;
                        webUser.UpdateLogin(data[0].Id, data[0].LastIp, data[0].LastTime);//更新登录信息
                        HelperUser.SetUser(data[0]);//设置用户cookie
                        PublicMod.WriteLog("user_Login", data[0].Id, "微信登录", "u_" + data[0].Id.ToString());//登录日志
                        Response.Redirect(strUrl);
                        return;
                    }
                }
            }
            ltInfo.Text = "<script>$(function(){ alert('请先绑定微信用户后，才能使用微信登录！'); window.location.replace('../cn/login.aspx?url=" + strUrl + "'); });</script>";
        }
        //获取用户微信信息
        private string getWx(string state, string strUrl)
        {
            try
            {
                JsApi wxApi = new JsApi();
                WxData wxData = new WxData();
                //调用【网页授权获取用户信息】接口获取用户的openid和access_token
                //snsapi_base （不弹出授权页面，直接跳转，只能获取用户openid）
                string scope = (state == "user") ? "snsapi_userinfo" : "snsapi_base";//"snsapi_base"//授权获取"snsapi_userinfo"
                wxApi.GetOpenidAndAccessToken(config.APPID, config.APPSECRET, scope);//获取用户的openid和access_token

                if (state == "openid")
                {
                    ltInfo.Text = "openid：" + wxApi.openid;
                    return "";
                }
                else if (scope == "snsapi_userinfo")
                {//采用授权获取用户信息
                    if (!wxApi.CheckRefreshToken(wxApi.access_token, wxApi.openid))//根据access_token判断access_token是否过期
                    {
                        wxApi.GetRefreshToken(config.APPID, wxApi.refresh_token);//若access_token已过期则根据refresh_token取得新的access_token
                    }
                    wxData = wxApi.GetUserInfo(wxApi.access_token, wxApi.openid);//采用授权获取用户信息
                }
                else
                {//采用静默高级接口获取用户信息
                    string access_token = wxApi.GetAccessToken(config.APPID, config.APPSECRET, config.TOKEN);//获取（基础支持）access_token
                    wxData = wxApi.GetUserInfo2(access_token, wxApi.openid);//高级接口获取用户信息
                }

                string info = "";
                //info += "openid：" + wxApi.openid + "<hr/>";
                //info += "access_token：" + wxApi.access_token + "<hr/>";
                //Response.Write(info); Response.End(); return;
                if (wxData != null)
                {
                    wxData.SetValue("WxAccess_token", wxApi.access_token);
                    wxData.SetValue("WxRefresh_token", wxApi.refresh_token);

                    //info += wxData.ToPrintStr() + "<hr/>";
                    if (wxData.IsSet("subscribe"))
                    {
                        if (wxData.GetValue("subscribe").ToString() == "0")
                        {
                            plSubscribe.Visible = true;
                            //info = "<div style=\"text-align:center;\">未关注本微信号！<b>趣游网（qyw-sh）</b><br/><img src=\"../images/weixin_qyw-sh.jpg\" width=\"200px\" /></div>";//<a href=\"?a=bind\" data-role=\"button\" rel=\"external\">继续授权绑定</a>
                            return "";
                        }
                        else
                        {
                            DateTime dtStart = new DateTime(1970, 1, 1);
                            double doNum = Convert.ToDouble(wxData.GetValue("subscribe_time").ToString());
                            //dtStart.ToUniversalTime()
                            string subscribe_time = dtStart.ToLocalTime().AddSeconds(doNum).ToString("yyyy-MM-dd HH:mm:ss");
                            //subscribe_time += " (" + dtStart.AddSeconds(doNum).ToString("yyyy-MM-dd HH:mm:ss") + ")";
                            info += "关注：" + wxData.GetValue("subscribe").ToString() + "<br/>时间：" + subscribe_time + "<br/>";
                        }
                    }
                    string sex = "";
                    if (wxData.IsSet("sex"))
                    {
                        switch (wxData.GetValue("sex").ToString())
                        {
                            case "1":
                                sex = "男";
                                break;
                            case "2":
                                sex = "女";
                                break;
                            default:
                                break;
                        }
                    }
                    info += string.Format("昵称：{0}<br/>性别：{1}<br/>国家：{2}<br/>省市：{3}<br/>城市：{4}<br/>头像：<img src=\"{5}\" width=\"64px\" /><br/>", wxData.GetValue("nickname"), sex, wxData.GetValue("country"), wxData.GetValue("province"), wxData.GetValue("city"), wxData.GetValue("headimgurl"));

                    if (state == "unbind")
                    {
                        DataUser myUser = HelperUser.GetUser();
                        if (myUser != null && myUser.Id > 0 && !string.IsNullOrEmpty(myUser.TrueName))
                        {
                            info += "<div><a href=\"?ac=unbinded&url=" + strUrl + "\" onclick=\"if (!confirm('解绑微信后，将收不到微信通知哦！\\n您确定要“解绑微信”吗？')) {return false;}\">解绑微信</a></div>";
                        }
                        //else
                        //{
                        //    info = "";
                        //}
                    }
                    else
                    {
                        saveWx(wxData);//保存用户微信信息
                        //info = "";
                    }
                }
                else
                {
                    info += "未取得微信信息！";
                    return "";

                }
                ltInfo.Text = info;
                return wxApi.openid;
            }
            catch (Exception ex)
            {
                //Response.Write("<span style='color:#FF0000;font-size:20px'>" + "页面加载出错，请重试" + "</span>");
                ltInfo.Text = "页面加载出错，请重试！<br/>" + ex.ToString();
            }
            return "";
        }
        //新增、更新用户微信信息
        private void saveWx(WxData wxData)
        {
            string info = "";
            DataUserWx data = new DataUserWx();
            data.WxAppId = config.APPID;
            data.WxOpenId = wxData.GetValue("openid").ToString();
            data.WxNickName = wxData.GetValue("nickname").ToString();
            data.WxSex = Convert.ToInt16(wxData.GetValue("sex").ToString());
            data.WxCountry = wxData.GetValue("country").ToString();
            data.WxProvince = wxData.GetValue("province").ToString();
            data.WxCity = wxData.GetValue("city").ToString();
            data.WxHeadImgUrl = wxData.GetValue("headimgurl").ToString();
            if (wxData.IsSet("subscribe"))
            {
                data.WxSubscribe = Convert.ToInt16(wxData.GetValue("subscribe").ToString());
            }
            if (wxData.IsSet("language"))
            {
                data.WxLanguage = wxData.GetValue("language").ToString();
            }
            if (wxData.IsSet("subscribe_time"))
            {
                data.WxSubscribe_time = Convert.ToInt32(wxData.GetValue("subscribe_time").ToString());
            }
            if (wxData.IsSet("privilege"))
            {
                data.WxPrivilege = wxData.GetValue("privilege").ToString();
            }
            if (wxData.IsSet("unionid"))
            {
                data.WxUnionid = wxData.GetValue("unionid").ToString();
            }
            if (wxData.IsSet("remark"))
            {
                data.WxRemark = wxData.GetValue("remark").ToString();
            }
            if (wxData.IsSet("groupid"))
            {
                data.WxGroupid = Convert.ToInt32(wxData.GetValue("groupid").ToString());
            }
            if (wxData.IsSet("tagid_list"))
            {
                data.WxTagid_list = wxData.GetValue("tagid_list").ToString();
            }
            if (wxData.IsSet("WxAccess_token"))
            {
                data.WxAccess_token = wxData.GetValue("WxAccess_token").ToString();
                data.WxAccess_expires_in = DateTime.Now;
            }
            if (wxData.IsSet("WxRefresh_token"))
            {
                data.WxRefresh_token = wxData.GetValue("WxRefresh_token").ToString();
                data.WxRefresh_expires_in = DateTime.Now;
            }
            string strIp = HelperMain.GetIpPort();
            DateTime dtTime = DateTime.Now;
            WebUserWx webUserWx = new WebUserWx();
            DataUserWx[] user = webUserWx.GetDatas(config.APPID, 0, data.WxOpenId);
            if (user != null)
            {
                if (user[0].Active <= 0)
                {
                    info = "您的状态异常" + user[0].Active.ToString() + "，请与管理员联系！";
                }
                else
                {
                    //更新信息，跳转页面
                    data.Id = user[0].Id;
                    bool blUpdate = false;
                    if (user[0].WxSubscribe != data.WxSubscribe)
                    {
                        user[0].WxSubscribe = data.WxSubscribe;
                        blUpdate = true;
                    }
                    if (user[0].WxNickName != data.WxNickName)
                    {
                        user[0].WxNickName = data.WxNickName;
                        blUpdate = true;
                    }
                    if (user[0].WxSex != data.WxSex)
                    {
                        user[0].WxSex = data.WxSex;
                        blUpdate = true;
                    }
                    if (user[0].WxLanguage != data.WxLanguage)
                    {
                        user[0].WxLanguage = data.WxLanguage;
                        blUpdate = true;
                    }
                    if (user[0].WxProvince != data.WxProvince)
                    {
                        user[0].WxProvince = data.WxProvince;
                        blUpdate = true;
                    }
                    if (user[0].WxCity != data.WxCity)
                    {
                        user[0].WxCity = data.WxCity;
                        blUpdate = true;
                    }
                    if (user[0].WxCountry != data.WxCountry)
                    {
                        user[0].WxCountry = data.WxCountry;
                        blUpdate = true;
                    }
                    if (user[0].WxHeadImgUrl != data.WxHeadImgUrl)
                    {
                        user[0].WxHeadImgUrl = data.WxHeadImgUrl;
                        blUpdate = true;
                    }
                    if (user[0].WxSubscribe_time != data.WxSubscribe_time)
                    {
                        user[0].WxSubscribe_time = data.WxSubscribe_time;
                        blUpdate = true;
                    }
                    if (user[0].WxPrivilege != data.WxPrivilege)
                    {
                        user[0].WxPrivilege = data.WxPrivilege;
                        blUpdate = true;
                    }
                    if (user[0].WxUnionid != data.WxUnionid)
                    {
                        user[0].WxUnionid = data.WxUnionid;
                        blUpdate = true;
                    }
                    if (user[0].WxRemark != data.WxRemark)
                    {
                        user[0].WxRemark = data.WxRemark;
                        blUpdate = true;
                    }
                    if (user[0].WxGroupid != data.WxGroupid)
                    {
                        user[0].WxGroupid = data.WxGroupid;
                        blUpdate = true;
                    }
                    if (user[0].WxTagid_list != data.WxTagid_list)
                    {
                        user[0].WxTagid_list = data.WxTagid_list;
                        blUpdate = true;
                    }
                    if (user[0].WxAccess_token != data.WxAccess_token)
                    {
                        user[0].WxAccess_token = data.WxAccess_token;
                        user[0].WxAccess_expires_in = data.WxAccess_expires_in;
                        blUpdate = true;
                    }
                    if (user[0].WxRefresh_token != data.WxRefresh_token)
                    {
                        user[0].WxRefresh_token = data.WxRefresh_token;
                        user[0].WxRefresh_expires_in = data.WxRefresh_expires_in;
                        blUpdate = true;
                    }
                    user[0].UpIp = strIp;
                    user[0].UpTime = dtTime;

                    if (blUpdate)
                    {
                        if (webUserWx.Update(user[0]) > 0)
                        {
                            info = "信息更新成功！";
                        }
                    }
                    else
                    {
                        info = "信息未修改。";
                    }
                }
            }
            else
            {//新增信息，跳转页面
                data.Active = 1;
                data.AddIp = strIp;
                data.AddTime = dtTime;
                data.Id = webUserWx.Insert(data);
                if (data.Id > 0)
                {
                    info = "信息新增成功！";
                }
                else
                {
                    info = "签到出错，请稍后重试！";
                    //info = "<script> $(function(){ alert('签到出错，请稍后重试！'); });</script>";
                }
            }
            ltInfo.Text = info;
        }
        //
    }
}