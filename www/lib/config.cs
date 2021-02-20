using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hkzx.web
{
    public class config
    {
        /// Tony的测试号
        /// 测试appID：wxb3aaa3da1393bbdd
        /// 测试appsecret：d0a6cffc75ac2080fe1486e540416001
        /// Tony的OpenId：oT0o805kBzSylcFmyvQNy92jzNOg
        /// 虹口政协订阅号：hkqzx2015
        /// 虹口政协服务号：hkqzxwylzxt
        /// 开发者ID(AppID)：wxc9f6bfba12c2fc61
        /// 开发者密码(AppSecret)：c32c716e7ef1c3b2b2cd21a2f3395f19
        /// Tony的OpenId：obzVv1XUZojotGFD1L_TH4p0wII8
        /// 一次性订阅消息，模版ID：dL_4aLpxg4IiLzXKRZhwfk7EzeekeFlpfwfYAzKuqyg
        public const string APPID = "wxc9f6bfba12c2fc61";//
        public const string APPSECRET = "c32c716e7ef1c3b2b2cd21a2f3395f19";//
        public const string TOKEN = "hpzx_token";
        public const string TICKET = "hpzx_ticket";
        public const string IDDESKEY = "hk807zx";
        public const string IDDESIV = "shz#008";
        public const string SIGNDEKEY = "hkqzxgov";
        public const string SIGNDESIV = "hkzx#123";
        public const string HOSTIP = "117.184.33.144";
        public const string HOSTNAME = "hkzx.quyou.net";
        public const string HOSTDEBUG = "localhost:8010";
        public const string SMSUSER = "shzxhk";
        public const string SMSPWD = "Sh123456";
        public const string SMSSIGN = "【虹口政协】";

        public const string PERIOD = "十四";//届次
        public const string TIMES = "五";//届次
        public static string[,] arrSignDesk = { { "01", "01" }, { "02", "02" }, { "03", "03" }, { "04", "04" }, { "05", "05" }, { "06", "06" }, { "07", "07" }, { "08", "08" }, { "09", "09" }, { "10", "10" }, { "11", "11" }, { "12", "12" }, { "13", "川北" }, { "14", "欧阳" }, { "15", "嘉兴" }, { "16", "北外滩" }, { "17", "曲阳" }, { "18", "广中" }, { "19", "凉城" }, { "20", "江湾" } };

        public const string TelTeam = "021-25657525";//提案科电话
    }
}