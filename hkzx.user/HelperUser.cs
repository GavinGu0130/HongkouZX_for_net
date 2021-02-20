using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using mod.main;
using hkzx.db;

namespace hkzx.user
{
    public class HelperUser
    {
        private const string strCookie = "hkzx";
        private const string strDesKey = "hkzx-807";
        private const string strDesIv = "shhkzx-123";
        private const int intMinute = 30;//cookie保存时间
        private const int intHour = 8;//最长强制重新登录时间
        //cookie 30分钟过期，8小时后必须重新登录
        //Id=UserType=UserName=UserCode=TrueName=UserSex=Photo=LastTime=Committee=Committee2=Subsector=Subsector2=StreetTeam=Party，des加密处理
        #region 用户
        //设置用户
        public static void SetUser(DataUser data)
        {
            string strUser = string.Format("{0}={1}={2}={3}={4}={5}={6}={7:yyyy-MM-dd HH:mm:ss}={8}={9}={10}={11}={12}={13}={14}={15:yyyy-MM-dd HH:mm:ss}", data.Id, data.UserType, data.UserName, data.UserCode, data.TrueName, data.UserSex, data.Photo, data.LastTime, data.Committee, data.Committee2, data.Subsector, data.Subsector2, data.StreetTeam, data.Party, data.OrderColor, data.OrderTime);
            HttpContext.Current.Response.Cookies[strCookie].Value = HelperSecret.DESEncrypt(strUser, strDesKey, strDesIv);
            HttpContext.Current.Response.Cookies[strCookie].Expires = DateTime.Now.AddMinutes(intMinute);
        }
        //获取用户
        public static DataUser GetUser()
        {
            if (HttpContext.Current.Request.Cookies[strCookie] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Cookies[strCookie].Value))
            {
                string strUser = HelperSecret.DESDecrypt(HttpContext.Current.Request.Cookies[strCookie].Value, strDesKey, strDesIv);
                if (!string.IsNullOrEmpty(strUser))
                {
                    string[] arr = strUser.Split('=');
                    if (arr.Count() == 16)
                    {
                        DateTime dtLastTime = Convert.ToDateTime(arr[7]);
                        if (dtLastTime.AddHours(intHour) > DateTime.Now)
                        {
                            DataUser data = new DataUser();
                            data.Id = Convert.ToInt32(arr[0]);
                            data.UserType = arr[1];
                            data.UserName = arr[2];
                            data.UserCode = arr[3];
                            data.TrueName = arr[4];
                            data.UserSex = arr[5];
                            data.Photo = arr[6];
                            data.LastTime = dtLastTime;
                            data.Committee = arr[8];
                            data.Committee2 = arr[9];
                            data.Subsector = arr[10];
                            data.Subsector2 = arr[11];
                            data.StreetTeam = arr[12];
                            data.Party = arr[13];
                            data.OrderColor = arr[14];
                            data.OrderTime = Convert.ToDateTime(arr[15]);
                            WebUser webUser = new WebUser();
                            DataUser[] qData = webUser.GetData(data.Id, "LastTime");
                            if (qData != null && (qData[0].LastTime.ToString("yyyy-MM-dd HH:mm:ss") == arr[7] || HttpContext.Current.Request.IsLocal))
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
        //
        #region 登出
        public static void Logout()
        {
            HttpContext.Current.Response.Cookies[strCookie].Value = "";
            HttpContext.Current.Response.Cookies[strCookie].Expires = DateTime.Now.AddDays(-1);
        }
        #endregion
        //
    }
}