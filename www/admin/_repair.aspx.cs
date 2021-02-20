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
    public partial class _repair : System.Web.UI.Page
    {
        private DataAdmin myUser = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperAdmin.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
            {
                //Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            if (myUser.Powers.IndexOf("alls") < 0 && myUser.Powers.IndexOf("system") < 0)
            {
                Response.Redirect("./");
            }
            btnUserScore.Visible = true;
        }
        //修正用户积分时间
        protected void btnUserScore_Click(object sender, EventArgs e)
        {
            WebUserScore webScore2 = new WebUserScore();
            DataUserScore[] data = webScore2.GetDatas(0, "", "", 0, "", "", "");
            if (data == null)
            {
                lblInfo.Text = "没有数据被修复";
                return;
            }
            WebOpinion webOpinion = new WebOpinion();
            WebOpinionPop webPop = new WebOpinionPop();
            WebReport webReport = new WebReport();
            WebPerform webPerform = new WebPerform();
            WebPerformFeed webFeed = new WebPerformFeed();
            for (int i = 0; i < data.Count(); i++)
            {
                DateTime dtTime = DateTime.MinValue;
                switch (data[i].TableName)
                {
                    case "tb_Opinion":
                        DataOpinion[] opData = webOpinion.GetData(data[i].TableId, "SubTime,AddTime");
                        if (opData != null)
                        {
                            if (opData[0].SubTime > DateTime.MinValue)
                            {
                                dtTime = opData[0].SubTime;
                            }
                            else if (opData[0].AddTime > new DateTime(2019, 1, 1) && opData[0].AddTime < new DateTime(2019, 1, 10))
                            {
                                dtTime = new DateTime(2018, 12, 31);
                            }
                            else
                            {
                                dtTime = opData[0].AddTime;
                            }
                        }
                        break;
                    case "tb_Opinion_Pop":
                        DataOpinionPop[] opData2 = webPop.GetData(data[i].TableId, "SubTime,AddTime");
                        if (opData2 != null)
                        {
                            dtTime = (opData2[0].SubTime > DateTime.MinValue) ? opData2[0].SubTime : opData2[0].AddTime;
                        }
                        break;
                    case "tb_Report":
                        DataReport[] rData = webReport.GetData(data[i].TableId, "SubTime,AddTime");
                        if (rData != null)
                        {
                            dtTime = (rData[0].SubTime > DateTime.MinValue) ? rData[0].SubTime : rData[0].AddTime;
                        }
                        break;
                    case "tb_Perform_Feed":
                        DataPerformFeed[] feedData = webFeed.GetData(data[i].TableId, "ActiveName,SignTime,VerifyTime,AddTime,PerformId");
                        if (feedData != null)
                        {
                            if (feedData[0].ActiveName == "已签到")
                            {
                                if (feedData[0].SignTime > DateTime.MinValue)
                                {
                                    dtTime = feedData[0].SignTime;
                                }
                                else if (feedData[0].VerifyTime > DateTime.MinValue)
                                {
                                    dtTime = feedData[0].VerifyTime;
                                }
                                else
                                {
                                    dtTime = feedData[0].AddTime;
                                }
                            }
                            else
                            {
                                DataPerform[] pData = webPerform.GetData(feedData[0].PerformId, "StartTime,EndTime");
                                if (pData != null)
                                {
                                    dtTime = pData[0].EndTime;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
                if (dtTime > DateTime.MinValue)
                {
                    data[i].GetTime = dtTime;
                }
                else
                {
                    data[i].GetTime = data[i].AddTime;
                }
                webScore2.Update(data[i]);
            }
            lblInfo.Text = "完成";
        }
        //
    }
}