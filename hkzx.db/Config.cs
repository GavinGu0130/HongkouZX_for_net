using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hkzx.db
{
    public class Config
    {
        public static string ConnString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnHkZx"].ConnectionString;
    }
}