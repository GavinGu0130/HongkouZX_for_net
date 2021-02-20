using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mod.main;

namespace hkzx.web.cn
{
    public partial class code : System.Web.UI.Page
    {
        public const string strCookie = "code";
        protected void Page_Load(object sender, EventArgs e)
        {
            string strCode = HelperMain.GetRdString(5);
            Response.Cookies[strCookie].Value = strCode;
            Response.Cookies[strCookie].Expires = DateTime.Now.AddMinutes(10);
            HelperImg.CreateCode(strCode);
        }
    }
}