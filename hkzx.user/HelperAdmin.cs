using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using mod.main;
using hkzx.db;

namespace hkzx.user
{
    public class HelperAdmin
    {
        private const string strCookie = "hkzx_ad";
        private const string strDesKey = "hkzx-008";
        private const string strDesIv = "shhkzx-789";
        private const int intMinute = 30;
        private const int intHour = 4;
        //cookie 30分钟过期，4小时后必须重新登录
        //Id=AdminName=TrueName=Grade=Powers=LastTime，des加密处理
        #region 设置用户
        public static void SetUser(DataAdmin data)
        {
            string strUser = string.Format("{0}={1}={2}={3}={4}={5:yyyy-MM-dd HH:mm:ss}", data.Id, data.AdminName, data.TrueName, data.Grade, data.Powers, data.LastTime);
            HttpContext.Current.Response.Cookies[strCookie].Value = HelperSecret.DESEncrypt(strUser, strDesKey, strDesIv);
            HttpContext.Current.Response.Cookies[strCookie].Expires = DateTime.Now.AddMinutes(intMinute);
        }
        #endregion

        #region 获取用户
        public static DataAdmin GetUser()
        {
            if (HttpContext.Current.Request.Cookies[strCookie] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Cookies[strCookie].Value))
            {
                string strUser = HelperSecret.DESDecrypt(HttpContext.Current.Request.Cookies[strCookie].Value, strDesKey, strDesIv);
                if (!string.IsNullOrEmpty(strUser))
                {
                    string[] arr = strUser.Split('=');
                    if (arr.Count() == 6)
                    {
                        DateTime dtLastTime = Convert.ToDateTime(arr[5]);
                        if (dtLastTime.AddHours(intHour) > DateTime.Now)
                        {
                            DataAdmin data = new DataAdmin();
                            data.Id = Convert.ToInt32(arr[0]);
                            data.AdminName = arr[1];
                            data.TrueName = arr[2];
                            data.Grade = Convert.ToInt16(arr[3]);
                            data.Powers = arr[4];
                            data.LastTime = dtLastTime;
                            WebAdmin webAdmin = new WebAdmin();
                            DataAdmin[] qData = webAdmin.GetData(data.Id, "LastTime");
                            if (qData != null && qData[0].LastTime.ToString("yyyy-MM-dd HH:mm:ss") == arr[5])
                            {
                                SetUser(data);
                                return data;
                            }
                        }
                    }
                }
            }
            return null;
        }
        #endregion

        #region 登出
        public static void Logout()
        {
            HttpContext.Current.Response.Cookies[strCookie].Value = "";
            HttpContext.Current.Response.Cookies[strCookie].Expires = DateTime.Now.AddDays(-1);
        }
        #endregion
    }
}