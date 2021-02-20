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
    public partial class count : System.Web.UI.Page
    {
        private DataAdmin myUser = null;
        private string strDialog = "../cn/dialog.aspx";
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperAdmin.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
            {
                //Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            if (myUser.Powers.IndexOf("alls") < 0 && myUser.Powers.IndexOf("count") < 0)
            {
                Response.Redirect("./");
            }
            if (myUser.AdminName == "Tony")
            {
                lnkOther.Visible = true;
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.Powers = myUser.Powers;
            plNav.Visible = true;
            if (!IsPostBack)
            {
                string strTile = "";
                switch (Request.QueryString["ac"])
                {
                    case "committee":
                        plCount.Visible = true;
                        loadYearList();
                        listCommittee();
                        strTile = ltCountTitle.Text;
                        break;
                    case "street":
                        plCount.Visible = true;
                        loadYearList();
                        listStreetTeam();
                        strTile = ltCountTitle.Text;
                        break;
                    case "subsector":
                        plCount.Visible = true;
                        loadYearList();
                        listSubsector("界别");
                        strTile = ltCountTitle.Text;
                        break;
                    case "subsector2":
                        plCount.Visible = true;
                        loadYearList();
                        listSubsector("界别活动组");
                        strTile = ltCountTitle.Text;
                        break;
                    case "speak":
                        strTile = "大会发言";
                        plSpeak.Visible = true;
                        listSpeak();
                        break;
                    case "invited":
                        strTile = "特邀监督员工作";
                        plInvited.Visible = true;
                        listInivted();
                        break;
                    case "appraise":
                        strTile = "遴选评优";
                        plAppraise.Visible = true;
                        string strYear = DateTime.Today.ToString("yyyy");
                        txtAppDate1.Text = strYear + "-01-01";
                        txtAppDate2.Text = strYear + "-12-31";
                        break;
                    case "other":
                        strTile = "其它统计";
                        plOther.Visible = true;
                        break;
                    default:
                        plUser.Visible = true;
                        strTile = listQuery() + "统计表";
                        break;
                }
                Header.Title += " - " + strTile;
            }
        }
        //
        #region 模块
        //加载年度选项
        private void loadYearList()
        {
            int addYear = 2019;
            int nowYear = DateTime.Today.Year;
            for (int i = addYear; i <= nowYear; i++)
            {
                ListItem item = new ListItem(string.Format("{0}年度", i), i.ToString());
                ddlYear.Items.Add(item);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["year"]))
            {
                for (int i = 0; i < ddlYear.Items.Count; i++)
                {
                    if (ddlYear.Items[i].Value == Request.QueryString["year"])
                    {
                        ddlYear.SelectedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                ddlYear.SelectedIndex = ddlYear.Items.Count - 1;
            }
        }
        //计算委员数
        private int countManNum(string strTeamMans, string strSubMans)
        {
            int intNum = 0;
            if (!string.IsNullOrEmpty(strTeamMans) && !string.IsNullOrEmpty(strSubMans))
            {
                strSubMans = "," + strSubMans.Trim(',') + ",";
                string[] arr = strTeamMans.Trim(',').Split(',');
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(arr[i]))
                    {
                        if (strSubMans.IndexOf(arr[i]) >= 0)
                        {
                            intNum++;
                        }
                    }
                }
            }
            return intNum;
        }
        //界别活动组to界别
        private string getSubsector(string strSub, string strSub2)
        {
            string strOut = "";
            if (!string.IsNullOrEmpty(strSub))
            {//界别to活动组
                switch (strSub)
                {
                    case "农工":
                    case "致公":
                        strOut = strSub + "党";
                        break;
                    case "九三":
                        strOut = strSub + "学社";
                        break;
                    case "共青团":
                    case "青联":
                        strOut = "共青团、青联";
                        break;
                    case "少数民族":
                    case "宗教界":
                        strOut = "少数民族、宗教";
                        break;
                    case "医药卫生界":
                        strOut = "医卫";
                        break;
                    case "文艺界":
                    case "科技界":
                    case "经济界":
                    case "教育界":
                    case "体育界":
                        strOut = strSub.Replace("界", "");
                        break;
                    default:
                        strOut = strSub;
                        break;
                }
            }
            else
            {//活动组to界别
                switch (strSub2)
                {
                    case "农工党":
                    case "致公党":
                        strOut = strSub2.Replace("党", "");
                        break;
                    case "九三学社":
                        strOut = strSub2.Replace("学社", "");
                        break;
                    case "共青团、青联":
                        strOut = "共青团,青联";
                        break;
                    case "少数民族、宗教":
                        strOut = "少数民族,宗教界";
                        break;
                    case "医卫":
                        strOut = "医药卫生界";
                        break;
                    case "文艺":
                    case "科技":
                    case "经济":
                    case "教育":
                    case "体育":
                        strOut = strSub2 + "界";
                        break;
                    default:
                        strOut = strSub2;
                        break;
                }
            }
            return strOut;
        }
        //获取活动次数
        private string getPerform(WebPerform webPerform, string strOrgName, string strSubType)
        {
            int intYear = (!string.IsNullOrEmpty(ddlYear.SelectedValue)) ? Convert.ToInt16(ddlYear.SelectedValue) : DateTime.Today.Year;
            string strOut = "";
            DataPerform qData = new DataPerform();
            qData.StartTimeText = string.Format("{0}-01-01,{0}-12-31", intYear);
            qData.ActiveName = "发布,履职关闭";
            qData.OrgName = strOrgName;
            qData.SubType = strSubType;
            DataPerform[] data = webPerform.GetDatas(qData, "Id");
            if (data != null)
            {
                string SubType = (!string.IsNullOrEmpty(strSubType) && strSubType.IndexOf("-") > 0) ? strSubType.Substring(strSubType.IndexOf("-") + 1) : strSubType;
                strOut = string.Format("<td align='center'><a href='dialog.aspx?ac=perform&ActiveName={1}&OrgName={2}&SubType={3}&time={4}' title='{4}' target='_blank'>{0}</a></td>", data.Count(), qData.ActiveName, qData.OrgName, qData.SubType, SubType, qData.StartTimeText);
            }
            else
            {
                strOut = "<td align='center'>0</td>";
            }
            return strOut;
        }
        //获取提案次数
        private string getOpinion(WebOpinion webOpin, string strUser, string Party, string Committee, string Subsector, string StreetTeam, int intManNum = 0, string strTeamMans = "")
        {
            int intYear = (!string.IsNullOrEmpty(ddlYear.SelectedValue)) ? Convert.ToInt16(ddlYear.SelectedValue) : DateTime.Today.Year;
            string strOut = "";
            DataOpinion qData = new DataOpinion();
            qData.SubTimeText = string.Format("{0}-12-01,{1}-11-30", intYear - 1, intYear);
            qData.ActiveName = "待立案,立案,不立案";
            if (intManNum > 0)
            {
                //第一提案人、联名人
                if (!string.IsNullOrEmpty(Party))
                {
                    qData.Party = Party;
                }
                if (!string.IsNullOrEmpty(Committee))
                {
                    qData.Committee = Committee;
                }
                if (!string.IsNullOrEmpty(Subsector))
                {
                    qData.Subsector = Subsector;
                }
                if (!string.IsNullOrEmpty(StreetTeam))
                {
                    qData.StreetTeam = StreetTeam;
                }
                DataOpinion[] data = webOpin.GetDatas(qData, "Id,SubMan,SubMan2,SubMans");//获取提案
                if (data != null)
                {
                    strOut += string.Format("<td align='center'><a href='dialog.aspx?ac=opinion&ActiveName={1}&Party={2}&Committee={3}&Subsector={4}&StreetTeam={5}&time={6}' target='_blank'>{0}</a></td>", data.Count(), qData.ActiveName, qData.Party, qData.Committee, qData.Subsector, qData.StreetTeam, qData.SubTimeText);
                    string strSubMans = "";
                    for (int j = 0; j < data.Count(); j++)
                    {
                        strSubMans += "," + data[j].SubMan + "," + data[j].SubMans;// + "," + data[j].SubMan2
                    }
                    int intNum = countManNum(strTeamMans, strSubMans);
                    if (intManNum > 0 && intManNum > 0)
                    {
                        strOut += string.Format("<td align='center'>{0:N2} %</td>", Convert.ToDecimal(intNum) * 100 / intManNum);
                    }
                    else
                    {
                        strOut += "<td><br/></td>";
                    }
                    //data.OpinNum = oData.Count();
                    //data.OpinScale = (data.OpinNum * 100 / data.UserNum).ToString() + "%";
                }
                else
                {
                    strOut += "<td align='center'>0</td><td><br/></td>";
                }
                //第一提案人
                qData.SubMan = strTeamMans;
                qData.IsSubMan1 = true;
                data = webOpin.GetDatas(qData, "Id,SubMan");//获取提案
                if (data != null)
                {
                    strOut += string.Format("<td align='center'><a href='dialog.aspx?ac=opinion&ActiveName={1}&Party={2}&Committee={3}&Subsector={4}&StreetTeam={5}&time={6}&SubMan1={7}' target='_blank'>{0}</a></td>", data.Count(), qData.ActiveName, qData.Party, qData.Committee, qData.Subsector, qData.StreetTeam, qData.SubTimeText, qData.SubMan);
                    string strSubMans = "";
                    for (int j = 0; j < data.Count(); j++)
                    {
                        strSubMans += "," + data[j].SubMan;
                    }
                    int intNum = countManNum(strTeamMans, strSubMans);
                    if (intManNum > 0 && intManNum > 0)
                    {
                        strOut += string.Format("<td align='center'>{0:N2} %</td>", Convert.ToDecimal(intNum) * 100 / intManNum);
                    }
                    else
                    {
                        strOut += "<td><br/></td>";
                    }
                    //data.OpinNum = oData.Count();
                    //data.OpinScale = (data.OpinNum * 100 / data.UserNum).ToString() + "%";
                }
                else
                {
                    strOut += "<td align='center'>0</td><td><br/></td>";
                }
            }
            else
            {
                strOut = "<td align='center'>0</td><td><br/></td><td align='center'>0</td><td><br/></td>";
            }
            return strOut;
        }
        //获取社情民意次数
        private string getPop(WebOpinionPop webPop, string strUser, string Party, string Committee, string Subsector, string StreetTeam, int intManNum = 0, string strTeamMans = "")
        {
            int intYear = (!string.IsNullOrEmpty(ddlYear.SelectedValue)) ? Convert.ToInt16(ddlYear.SelectedValue) : DateTime.Today.Year;
            string strOut = "";
            DataOpinionPop qData = new DataOpinionPop();
            qData.SubTimeText = string.Format("{0}-01-01,{0}-12-31", intYear);
            qData.ActiveName = "已录用,留存";//待审核,未录用,
            if (intManNum > 0)
            {
                //第一反映人、联名人
                if (!string.IsNullOrEmpty(Party))
                {
                    qData.Party = Party;
                }
                if (!string.IsNullOrEmpty(Committee))
                {
                    qData.Committee = Committee;
                }
                if (!string.IsNullOrEmpty(Subsector))
                {
                    qData.Subsector = Subsector;
                }
                if (!string.IsNullOrEmpty(StreetTeam))
                {
                    qData.StreetTeam = StreetTeam;
                }
                DataOpinionPop[] data = webPop.GetDatas(qData, "Id,SubMan,SubMans");//获取社情民意
                if (data != null)
                {
                    strOut += string.Format("<td align='center'><a href='dialog.aspx?ac=pop&ActiveName={1}&Party={2}&Committee={3}&Subsector={4}&StreetTeam={5}&time={6}' target='_blank'>{0}</a></td>", data.Count(), qData.ActiveName, qData.Party, qData.Committee, qData.Subsector, qData.StreetTeam, qData.SubTimeText);
                    string strSubMans = "";
                    for (int i = 0; i < data.Count(); i++)
                    {
                        strSubMans += "," + data[i].SubMan + "," + data[i].SubMans;
                    }
                    int intNum = countManNum(strTeamMans, strSubMans);
                    if (intNum > 0 && intManNum > 0)
                    {
                        strOut += string.Format("<td align='center'>{0:N2} %</td>", Convert.ToDecimal(intNum) * 100 / intManNum);
                    }
                    else
                    {
                        strOut += "<td><br/></td>";
                    }
                    //data.PopNum = pData.Count();
                    //data.PopScale = (data.PopNum * 100 / data.UserNum).ToString() + "%";
                }
                else
                {
                    strOut += "<td align='center'>0</td><td><br/></td>";
                }
                //第一反映人
                qData.SubMan1 = strTeamMans;
                data = webPop.GetDatas(qData, "Id,SubMan");//获取社情民意
                if (data != null)
                {
                    strOut += string.Format("<td align='center'><a href='dialog.aspx?ac=pop&ActiveName={1}&Party={2}&Committee={3}&Subsector={4}&StreetTeam={5}&time={6}&SubMan1={7}' target='_blank'>{0}</a></td>", data.Count(), qData.ActiveName, qData.Party, qData.Committee, qData.Subsector, qData.StreetTeam, qData.SubTimeText, qData.SubMan1);
                    string strSubMans = "";
                    for (int i = 0; i < data.Count(); i++)
                    {
                        strSubMans += "," + data[i].SubMan;
                    }
                    int intNum = countManNum(strTeamMans, strSubMans);
                    if (intNum > 0 && intManNum > 0)
                    {
                        strOut += string.Format("<td align='center'>{0:N2} %</td>", Convert.ToDecimal(intNum) * 100 / intManNum);
                    }
                    else
                    {
                        strOut += "<td><br/></td>";
                    }
                }
                else
                {
                    strOut += "<td align='center'>0</td><td><br/></td>";
                }
            }
            else
            {
                strOut = "<td align='center'>0</td><td><br/></td><td align='center'>0</td><td><br/></td>";
            }
            return strOut;
        }
        //获取发言次数
        private string getSpeak(WebPerformFeed webFeed, string Committee, string Subsector)
        {
            string strOut = "";
            string strSignTimeText = string.Format("{0:yyyy}-01-01,{0:yyyy}-12-31", DateTime.Today);
            DataPerformFeed[] data = webFeed.GetSpeaks(Committee, Subsector, strSignTimeText, "f.SignManSpeak");
            if (data != null)
            {
                strOut = string.Format("<td align='center'><a href='dialog.aspx?ac=speak&Committee={1}&Subsector={2}&time={3}' target='_blank'>{0}</a></td>", data.Count(), Committee, Subsector, strSignTimeText);
                //int SpeakNum = 0;
                //int WriteNum = 0;
                for (int i = 0; i < data.Count(); i++)
                {
                    //if (fData[j].SignManSpeak.IndexOf("上台") >= 0)
                    //{
                    //    SpeakNum++;
                    //}
                    //if (fData[j].SignManSpeak.IndexOf("书面") >= 0)
                    //{
                    //    WriteNum++;
                    //}
                }
            }
            else
            {
                strOut = "<td align='center'>0</td>";
            }
            return strOut;
        }
        #endregion
        //
        #region 专委会、界别、界别活动组、街道活动组 统计
        //专委会
        private void listCommittee()
        {
            WebOp webOp = new WebOp();
            DataOp[] opData = webOp.GetDatas(1, "专委会", "", "OpName");
            if (opData == null)
            {
                return;
            }
            ltCountTitle.Text = "专委会活动统计表";
            string strThead = "<tr><th>专委会</th><th>人数</th><th>全体会议</th><th>主任会议</th><th>视察</th><th>学习考察</th><th>对口协商<br/>(同心桥活动)</th><th>提案</th><th>参与比例</th><th>提案<br/>(第一提案人)</th><th>参与比例</th><th>社情民意</th><th>参与比例</th><th>社情民意<br/>(第一反映人)</th><th>参与比例</th></tr>";
            WebUser webUser = new WebUser();
            WebPerform webPerform = new WebPerform();
            WebOpinion webOpin = new WebOpinion();
            WebOpinionPop webPop = new WebOpinionPop();
            string strTbody = "";
            for (int i = 0; i < opData.Count(); i++)
            {
                string strTeamName = opData[i].OpName;
                string strTeamMans = "";
                int intTeamManNum = 0;
                //DataTable data = new DataTable();
                string strTr = "<td>" + strTeamName + "</td>";

                DataUser qUser = new DataUser();
                qUser.Period = config.PERIOD;
                qUser.UserType = "委员";
                qUser.ActiveName = ">0";
                qUser.OrderScore = 1;
                qUser.Committee = strTeamName;
                if (qUser.Committee == "教科卫体委员会")
                {
                    qUser.Committee += "%";
                }
                DataUser[] uData = webUser.GetDatas(qUser, "Id,TrueName");
                if (uData != null)
                {
                    for (int j = 0; j < uData.Count(); j++)
                    {
                        strTeamMans += "," + uData[j].TrueName;
                    }
                    intTeamManNum = uData.Count();
                }
                strTr += string.Format("<td align='center'>{0}</td>", intTeamManNum);
                strTr += getPerform(webPerform, "专委会-" + strTeamName, "专委会会议及活动-全体会议");
                strTr += getPerform(webPerform, "专委会-" + strTeamName, "专委会会议及活动-主任会议");
                strTr += getPerform(webPerform, "专委会-" + strTeamName, "专委会会议及活动-视察");
                strTr += getPerform(webPerform, "专委会-" + strTeamName, "专委会会议及活动-学习考察");
                strTr += getPerform(webPerform, "专委会-" + strTeamName, "同心桥");
                //strTr += getPerform(webPerform, "专委会-" + strTeamName, "其他调研课题%");
                //strTr += getPerform(webPerform, "专委会-" + strTeamName, "建议案调研课题%");
                strTr += getOpinion(webOpin, null, "", strTeamName, "", "", intTeamManNum, strTeamMans);
                strTr += getPop(webPop, null, "", strTeamName, "", "", intTeamManNum, strTeamMans);

                strTbody += "<tr>" + strTr + "</tr>";
            }
            ltCountTable.Text = string.Format("<table><thead>{0}</thead><tbody>{1}</tbody></table>", strThead, strTbody);
        }
        //界别、界别活动组
        private void listSubsector(string strSubName)
        {
            WebOp webOp = new WebOp();
            DataOp[] opData = webOp.GetDatas(1, strSubName, "", "OpName");
            if (opData == null)
            {
                return;
            }
            ltCountTitle.Text = strSubName + "统计表";
            string strThead = "<tr><th>" + strSubName + "</th><th>人数</th><th>全体会议</th><th>界别视察</th><th>界别协商<br/>(同心圆下午茶)</th><th>会议发言</th><th>提案</th><th>参与比例</th><th>提案<br/>(第一提案人)</th><th>参与比例</th><th>社情民意</th><th>参与比例</th><th>社情民意<br/>(第一反映人)</th><th>参与比例</th></tr>";
            WebUser webUser = new WebUser();
            WebPerform webPerform = new WebPerform();
            WebPerformFeed webFeed = new WebPerformFeed();
            WebOpinion webOpin = new WebOpinion();
            WebOpinionPop webPop = new WebOpinionPop();
            string strTbody = "";
            for (int i = 0; i < opData.Count(); i++)
            {
                string strTeamName = opData[i].OpName;
                string strTeamMans = "";
                int intTeamManNum = 0;
                string strSubsector = (strSubName == "界别活动组") ? getSubsector("", strTeamName) : strTeamName;
                string strTr = "<td>" + strTeamName + "</td>";

                DataUser qUser = new DataUser();
                qUser.Period = config.PERIOD;
                qUser.UserType = "委员";
                qUser.ActiveName = ">0";
                qUser.OrderScore = 1;
                qUser.Subsector = strSubsector;
                DataUser[] uData = webUser.GetDatas(qUser, "Id,TrueName");
                if (uData != null)
                {
                    for (int j = 0; j < uData.Count(); j++)
                    {
                        strTeamMans += "," + uData[j].TrueName;
                    }
                    intTeamManNum = uData.Count();
                }
                strTr += string.Format("<td align='center'>{0}</td>", intTeamManNum);
                strTr += getPerform(webPerform, strSubName + "-" + strTeamName, "界别活动组会议及活动-全体会议");
                strTr += getPerform(webPerform, strSubName + "-" + strTeamName, "界别活动组会议及活动-界别视察");
                strTr += getPerform(webPerform, strSubName + "-" + strTeamName, "同心圆下午茶");
                strTr += getSpeak(webFeed, "", strTeamName);
                strTr += getOpinion(webOpin, null, "", "", strSubsector, "", intTeamManNum, strTeamMans);
                strTr += getPop(webPop, null, "", "", strSubsector, "", intTeamManNum, strTeamMans);

                strTbody += "<tr>" + strTr + "</tr>";
            }
            ltCountTable.Text = string.Format("<table><thead>{0}</thead><tbody>{1}</tbody></table>", strThead, strTbody);
        }
        //街道活动组
        private void listStreetTeam()
        {
            WebOp webOp = new WebOp();
            DataOp[] opData = webOp.GetDatas(1, "街道活动组", "", "OpName");
            if (opData == null)
            {
                return;
            }
            ltCountTitle.Text = "街道活动组统计表";
            string strThead = "<tr><th>街道活动组</th><th>人数</th><th>计划总结</th><th>会议组织</th><th>视察调研</th><th>联系服务居民区<br/>及居民群众</th><th>主题活动</th><th>特色工作</th><th>专项民主监督</th></tr>";
            WebUser webUser = new WebUser();
            WebPerform webPerform = new WebPerform();
            string strTbody = "";
            for (int i = 0; i < opData.Count(); i++)
            {
                string strTeamName = opData[i].OpName;
                string strTeamMans = "";
                int intTeamManNum = 0;
                string strTr = "<td>" + strTeamName + "</td>";

                DataUser qUser = new DataUser();
                qUser.Period = config.PERIOD;
                qUser.UserType = "委员";
                qUser.ActiveName = ">0";
                qUser.OrderScore = 1;
                qUser.StreetTeam = strTeamName;
                DataUser[] uData = webUser.GetDatas(qUser, "Id,TrueName");
                if (uData != null)
                {
                    for (int j = 0; j < uData.Count(); j++)
                    {
                        strTeamMans += "," + uData[j].TrueName;
                    }
                    intTeamManNum = uData.Count();
                }
                strTr += string.Format("<td align='center'>{0}</td>", intTeamManNum);
                strTr += getPerform(webPerform, "街道活动组-" + strTeamName, "街道活动组会议及活动-计划总结");
                strTr += getPerform(webPerform, "街道活动组-" + strTeamName, "街道活动组会议及活动-会议组织");
                strTr += getPerform(webPerform, "街道活动组-" + strTeamName, "街道活动组会议及活动-视察调研");
                strTr += getPerform(webPerform, "街道活动组-" + strTeamName, "街道活动组会议及活动-联系服务居民区及居民群众");
                strTr += getPerform(webPerform, "街道活动组-" + strTeamName, "街道活动组会议及活动-主题活动");
                strTr += getPerform(webPerform, "街道活动组-" + strTeamName, "街道活动组会议及活动-特色工作");
                strTr += getPerform(webPerform, "街道活动组-" + strTeamName, "专项民主监督");

                strTbody += "<tr>" + strTr + "</tr>";
            }
            ltCountTable.Text = string.Format("<table><thead>{0}</thead><tbody>{1}</tbody></table>", strThead, strTbody);
        }
        #endregion
        //
        #region 会议发言、特邀监督员工作 统计
        //会议发言
        private void listSpeak()
        {
            WebPerformFeed webFeed = new WebPerformFeed();
            DataPerformFeed qData = new DataPerformFeed();
            qData.ActiveName = "已签到";
            qData.SignTimeText = string.Format("{0:yyyy}-01-01,{0:yyyy}-12-31", DateTime.Today);
            DataPerformFeed[] fData = webFeed.GetSpeaks(qData);
            if (fData != null)
            {
                List<DataUser> list = new List<DataUser>();
                WebUser webUser = new WebUser();
                for (int i = 0; i < fData.Count(); i++)
                {
                    DataUser data = new DataUser();
                    if (fData[i].UserId > 0)
                    {
                        DataUser[] uData = webUser.GetData(fData[i].UserId);
                        if (uData != null)
                        {
                            if (uData[0].Birthday > DateTime.MinValue)
                            {
                                uData[0].BirthdayText = uData[0].Birthday.ToString("yyyy-MM-dd");
                            }
                            data = uData[0];
                        }
                    }
                    else
                    {
                        DataUser uData = new DataUser();
                        uData.Id = fData[i].UserId;
                        uData.TrueName = fData[i].SignMan;
                    }
                    data.PlatformNum = fData[i].PlatformNum;
                    data.WriteNum = fData[i].WriteNum;
                    data.SpeakNum = fData[i].SpeakNum;
                    list.Add(data);
                }
                if (list.Count > 0)
                {
                    rpSpeakList.DataSource = list;
                    rpSpeakList.DataBind();
                    ltSpeakNo.Visible = false;
                    ltSpeakTotal.Text = list.Count.ToString();
                }
            }
        }
        //特邀监督员工作
        private void listInivted()
        {
            WebUser webUser = new WebUser();
            DataUser qData = new DataUser();
            qData.ActiveName = ">0";
            qData.Period = config.PERIOD;
            qData.Role = "特邀监督员";
            DataUser[] data = webUser.GetDatas(qData);
            if (data != null)
            {
                WebPerformFeed webFeed = new WebPerformFeed();
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = i + 1;
                    if (data[i].Birthday > DateTime.MinValue)
                    {
                        data[i].BirthdayText = data[i].Birthday.ToString("yyyy-MM-dd");
                    }
                    DataPerformFeed qData2 = new DataPerformFeed();
                    qData2.ActiveName = "已签到";
                    qData2.UserId = data[i].Id;
                    qData2.SignTimeText = string.Format("{0:yyyy}-01-01,{0:yyyy}-12-31", DateTime.Today);
                    DataPerformFeed[] fData = webFeed.GetDatas(qData2, "Id");
                    if (fData != null)
                    {
                        data[i].PerformNum = fData.Count();
                    }
                }
                rpInvitedList.DataSource = data;
                rpInvitedList.DataBind();
                ltInvitedNo.Visible = false;
                ltInvitedTotal.Text = data.Count().ToString();
            }
        }
        #endregion
        //
        #region 委员/团体统计
        //加载搜索查询条件
        private DataUser getQData()
        {
            WebOp webOp = new WebOp();
            //PublicMod.LoadDropDownList(ddlQUserType, webOp, "用户类别");
            PublicMod.LoadDropDownList(ddlQParty, webOp, "政治面貌");
            PublicMod.LoadDropDownList(ddlQCommittee, webOp, "专委会");
            PublicMod.LoadDropDownList(ddlQSubsector, webOp, "界别");
            PublicMod.LoadDropDownList(ddlQStreetTeam, webOp, "街道活动组");
            PublicMod.LoadDropDownList(ddlQOrgType, webOp, "单位性质");
            PublicMod.LoadCheckBoxList(cblQRole, webOp, "政协职务");

            DataUser data = new DataUser();
            //if (!string.IsNullOrEmpty(Request.QueryString["UserType"]))
            //{
            //    data.UserType = HelperMain.SqlFilter(Request.QueryString["UserType"].Trim(), 20);
            //    HelperMain.SetDownSelected(ddlQUserType, data.UserType.ToString());
            //}
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
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Birthday"]) && Request.QueryString["Birthday"].IndexOf(",") >= 0)
            {
                string strDate = Request.QueryString["Birthday"];
                string strDate1 = strDate.Substring(0, strDate.IndexOf(","));
                string strDate2 = strDate.Substring(strDate.IndexOf(",") + 1);
                txtQBirthday1.Text = HelperMain.SqlFilter(strDate1.Trim(), 10);
                txtQBirthday2.Text = HelperMain.SqlFilter(strDate2.Trim(), 10);
                if (txtQBirthday1.Text != "" || txtQBirthday2.Text != "")
                {
                    data.BirthdayText = txtQBirthday1.Text + "," + txtQBirthday2.Text;
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["PostDate"]) && Request.QueryString["PostDate"].IndexOf(",") >= 0)
            {
                string strDate = Request.QueryString["PostDate"];
                string strDate1 = strDate.Substring(0, strDate.IndexOf(","));
                string strDate2 = strDate.Substring(strDate.IndexOf(",") + 1);
                txtQPostDate1.Text = HelperMain.SqlFilter(strDate1.Trim(), 10);
                txtQPostDate2.Text = HelperMain.SqlFilter(strDate2.Trim(), 10);
                if (txtQPostDate1.Text != "" || txtQPostDate2.Text != "")
                {
                    data.PostDateText = txtQPostDate1.Text + "," + txtQPostDate2.Text;
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Education"]))
            {
                data.Education = "%" + HelperMain.SqlFilter(Request.QueryString["Education"].Trim(), 20) + "%";
                txtQEducation.Text = data.Education.Trim('%');
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
        //加载列表
        private string listQuery()
        {
            string strQFields = Request.QueryString["Fields"];
            if (!string.IsNullOrEmpty(strQFields))
            {
                strQFields = "," + strQFields + ",";
            }
            string[] arrPlay = { "政协全体会议", "全体委员学习会", "常委会议", "专题协商议政会", "主席促办重点专题提案会|提案办理民主评议会", "专委会会议及活动", "街道活动组会议及活动", "界别会议及活动", "提案办理工作会议|提案协调会|提案答复会", "社情民意工作会", "社情民意专题会", "其他会议", "课题组视察", "专项民主监督", "委员年末集中视察", "“虹石榴”系列活动", "“同心”系列活动", "区政协组织的委员培训学习班", "参加其他培训班", "其他活动" };
            string[] arrSpeak = { "全会大会发言", "全会专题会发言", "其他会议发言" };
            string[] arrReport = { "列入常委会议和主席|会议建议案调研课题", "其他调研课题" };
            string[] arrOther = { "活动及发言统计", "提供资源", "提案", "社情民意", "扣分项", "其他得分" };
            int intPlay = 0;
            int intSpeak = 0;
            int intReport = 0;
            for (int i = 0; i < arrPlay.Count(); i++)
            {
                ListItem item = new ListItem(arrPlay[i]);
                if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrPlay[i] + ",") >= 0)
                {
                    intPlay++;
                    item.Selected = true;
                }
                cblFPlay.Items.Add(item);
            }
            if (cblFPlay.Items.Count == intPlay)
            {
                allPlay.Checked = true;
            }
            for (int i = 0; i < arrSpeak.Count(); i++)
            {
                ListItem item = new ListItem(arrSpeak[i]);
                if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrSpeak[i] + ",") >= 0)
                {
                    intSpeak++;
                    item.Selected = true;
                }
                cblFSpeak.Items.Add(item);
            }
            if (cblFSpeak.Items.Count == intSpeak)
            {
                allSpeak.Checked = true;
            }
            for (int i = 0; i < arrReport.Count(); i++)
            {
                ListItem item = new ListItem(arrReport[i]);
                if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrReport[i] + ",") >= 0)
                {
                    intReport++;
                    item.Selected = true;
                }
                cblFReport.Items.Add(item);
            }
            if (cblFReport.Items.Count == intReport)
            {
                allReport.Checked = true;
            }
            for (int i = 0; i < arrOther.Count(); i++)
            {
                ListItem item = new ListItem(arrOther[i]);
                if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrOther[i] + ",") >= 0)
                {
                    item.Selected = true;
                }
                cblFOther.Items.Add(item);
            }
            int pageCur = (!string.IsNullOrEmpty(Request.QueryString["page"])) ? Convert.ToInt32(Request.QueryString["page"]) : 1;
            if (pageCur < 1)
            {
                pageCur = 1;
            }
            int pageSize = (Request.QueryString["down"] == "xls") ? 0 : 10;
            if (!string.IsNullOrEmpty(Request.QueryString["CountDate"]) && Request.QueryString["CountDate"].IndexOf(",") >= 0)
            {
                string strDate = Request.QueryString["CountDate"];
                string strDate1 = strDate.Substring(0, strDate.IndexOf(","));
                string strDate2 = strDate.Substring(strDate.IndexOf(",") + 1);
                txtQCountDate1.Text = HelperMain.SqlFilter(strDate1.Trim(), 10);
                txtQCountDate2.Text = HelperMain.SqlFilter(strDate2.Trim(), 10);
            }
            else if (string.IsNullOrEmpty(Request.QueryString["CountType"]))
            {
                string strYear = DateTime.Today.ToString("yyyy");
                txtQCountDate1.Text = strYear + "-01-01";
                txtQCountDate2.Text = strYear + "-12-31";
            }
            string strGetTimeText = txtQCountDate1.Text + "," + txtQCountDate2.Text;
            HelperMain.SetRadioSelected(rblCountType, Request.QueryString["CountType"]);
            string strUserType = Request.QueryString["UserType"];
            HelperMain.SetRadioSelected(rblQUserType, strUserType);
            string strOut = "";
            if (!string.IsNullOrEmpty(strUserType) && strUserType != "委员")
            {
                List<string> list = new List<string>();
                WebOp webOp = new WebOp();
                DataOp[] opData = webOp.GetDatas(1, strUserType, "", "OpName");
                if (opData != null)
                {
                    for (int i = 0; i < opData.Count(); i++)
                    {
                        list.Add(opData[i].OpName);
                    }
                    string[] arrTeam = list.ToArray();
                    listTeam(strGetTimeText, strUserType, arrTeam, strQFields, arrPlay, arrSpeak, arrReport, arrOther);
                    strOut = "团体";
                }
                lnkDownXls.Visible = false;
            }
            else
            {
                DataUser qData = getQData();
                listUser(qData, strGetTimeText, pageCur, pageSize, strQFields, arrPlay, arrSpeak, arrReport, arrOther);
                strOut = "委员";
            }
            string strUrl = Request.Url.ToString();
            if (strUrl.IndexOf("?") > 0)
            {
                strUrl += "&down=xls";
            }
            else
            {
                strUrl += "?down=xls";
            }
            lnkDownXls.NavigateUrl = strUrl;
            return strOut;
        }
        //委员统计
        private void listUser(DataUser qData, string strGetTimeText, int pageCur, int pageSize, string strQFields, string[] arrPlay, string[] arrSpeak, string[] arrReport, string[] arrOther)
        {
            string strFields = "Id,UserCode,TrueName";//"Id,UserCode,TrueName,Birthday,Committee,Committee2,Subsector,Subsector2";//
            string strFilter = "";
            if (pageSize > 0)
            {//分布查询
                strFilter = "total";
                if (string.IsNullOrEmpty(qData.UserCode) && string.IsNullOrEmpty(qData.UserName))
                {
                    qData.OrderScore = 1;//记分人员
                }
            }
            else
            {//全部导出
                strFields += ",UserSex,Party,Mobile,Subsector,Committee,Committee2,StreetTeam,OrgType,OrgName";
                if (Request.IsLocal)
                {
                    strDialog = strDialog.Replace("../cn/", "http://" + config.HOSTDEBUG + "/cn/");
                }
                else
                {
                    if (string.IsNullOrEmpty(qData.UserCode) && string.IsNullOrEmpty(qData.UserName))
                    {
                        qData.UserCode = "14%";
                    }
                    strDialog = strDialog.Replace("../cn/", "http://" + config.HOSTIP + "/cn/");
                }
            }
            qData.Period = config.PERIOD;
            qData.UserType = "委员";
            qData.ActiveName = ">0";
            WebUser webUser = new WebUser();
            DataUser[] data = webUser.GetDatas(qData, strFields, pageCur, pageSize, "", strFilter);
            if (data != null)
            {
                string strThead = "委员编号\n姓名";
                for (int i = 0; i < arrPlay.Count(); i++)
                {
                    if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrPlay[i] + ",") >= 0)
                    {
                        strThead += "\n" + arrPlay[i];
                    }
                }
                for (int i = 0; i < arrSpeak.Count(); i++)
                {
                    if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrSpeak[i] + ",") >= 0)
                    {
                        strThead += "\n" + arrSpeak[i];
                    }
                }
                for (int i = 0; i < arrReport.Count(); i++)
                {
                    if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrReport[i] + ",") >= 0)
                    {
                        strThead += "\n" + arrReport[i];
                    }
                }
                for (int i = 0; i < arrOther.Count(); i++)
                {
                    if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrOther[i] + ",") >= 0)
                    {
                        strThead += "\n" + arrOther[i];
                    }
                }
                string strType = rblCountType.SelectedValue;
                strThead += (strType == "num") ? "\n统计次数" : "\n总分";
                WebUserScore webScore = new WebUserScore();
                for (int i = 0; i < data.Count(); i++)
                {
                    int[] arrPlayNum = new int[arrPlay.Count()];
                    decimal[] arrPlayScore = new decimal[arrPlay.Count()];
                    int[] arrSpeakNum = new int[arrSpeak.Count()];
                    decimal[] arrSpeakScore = new decimal[arrSpeak.Count()];
                    int[] arrReportNum = new int[arrPlay.Count()];
                    decimal[] arrReportScore = new decimal[arrPlay.Count()];
                    int countNum = 0;//活动及发言统计次数
                    decimal countScore = 0;//活动及发言统计分数
                    int resNum = 0;//提供资源次数
                    decimal resScore = 0;//提供资源分数
                    int opinNum = 0;//提案次数
                    decimal opinScore = 0;//提案分数
                    int popNum = 0;//社情民意次数
                    decimal popScore = 0;//社情民意数
                    int deScoreNum = 0;//扣分次数
                    decimal deScore = 0;//扣分
                    int otherScoreNum = 0;//其他得分次数
                    decimal otherScore = 0;//其他得分
                    int totalNum = 0;//总次数
                    decimal totalScore = 0;//总分

                    DataUserScore[] scoreData = webScore.GetDatas(1, data[i].Id.ToString(), "", 0, "", strGetTimeText, "Title,Score,TableName,TableId", 1, 0, "TableName ASC, TableId ASC, GetTime ASC");//(统计表)委员积分列表
                    if (scoreData != null)
                    {
                        for (int j = 0; j < scoreData.Count(); j++)
                        {
                            switch (scoreData[j].TableName)
                            {
                                case "tb_Perform_Feed"://会议/活动、发言、提供资源、扣分
                                    if (scoreData[j].Score < 0)
                                    {
                                        deScoreNum++;
                                        deScore += scoreData[j].Score;
                                    }
                                    else if (scoreData[j].Title.IndexOf("提供资源") >= 0)
                                    {
                                        resNum++;
                                        resScore += scoreData[j].Score;
                                    }
                                    else if (scoreData[j].Title.IndexOf("发言") >= 0)
                                    {
                                        for (int m = 0; m < arrSpeak.Count(); m++)
                                        {
                                            if (scoreData[j].Title.IndexOf(arrSpeak[m]) >= 0)
                                            {
                                                arrSpeakNum[m]++;
                                                arrSpeakScore[m] += scoreData[j].Score;
                                                countNum++;
                                                countScore += scoreData[j].Score;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int m = 0; m < arrPlay.Count(); m++)
                                        {
                                            if (scoreData[j].Title.IndexOf(arrPlay[m].Replace("|", "、")) >= 0)
                                            {
                                                arrPlayNum[m]++;
                                                arrPlayScore[m] += scoreData[j].Score;
                                                countNum++;
                                                countScore += scoreData[j].Score;
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                case "tb_Report"://调研报告
                                    for (int m = 0; m < arrReport.Count(); m++)
                                    {
                                        if (scoreData[j].Title.IndexOf(arrReport[m].Replace("|", "")) >= 0)
                                        {
                                            arrReportNum[m]++;
                                            arrReportScore[m] += scoreData[j].Score;
                                            countNum++;
                                            countScore += scoreData[j].Score;
                                            break;
                                        }
                                    }
                                    break;
                                case "tb_Opinion"://提案
                                    opinNum++;
                                    opinScore += scoreData[j].Score;
                                    break;
                                case "tb_Opinion_Pop"://社情民意
                                    popNum++;
                                    popScore += scoreData[j].Score;
                                    break;
                                default:
                                    otherScoreNum++;
                                    otherScore += scoreData[j].Score;
                                    break;
                            }
                        }
                    }

                    string strOther = string.Format("<a href='{0}?ac=score&UserId={2}&time={3}' target='_blank'>{1}</a>\n", strDialog, data[i].TrueName, data[i].Id, strGetTimeText);
                    for (int j = 0; j < arrPlay.Count(); j++)
                    {
                        if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrPlay[j] + ",") >= 0)
                        {
                            if (arrPlayNum[j] > 0)
                            {
                                string strNum = (strType == "num") ? arrPlayNum[j].ToString() : arrPlayScore[j].ToString("n2");//会议活动得分
                                strOther += string.Format("<a href='{0}?ac=score&UserId={2}&Title={3}&TableName=tb_Perform_Feed&time={4}' target='_blank'>{1}</a>\n", strDialog, strNum, data[i].Id, arrPlay[j], strGetTimeText);
                                totalNum += arrPlayNum[j];
                                totalScore += arrPlayScore[j];
                            }
                            else
                            {
                                strOther += "0\n";
                            }
                        }
                    }
                    for (int j = 0; j < arrSpeak.Count(); j++)
                    {
                        if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrSpeak[j] + ",") >= 0)
                        {
                            if (arrSpeakNum[j] > 0)
                            {
                                string strNum = (strType == "num") ? arrSpeakNum[j].ToString() : arrSpeakScore[j].ToString("n0");//会议发言得分
                                strOther += string.Format("<a href='{0}?ac=score&UserId={2}&Title={3}&TableName=tb_Perform_Speak&time={4}' target='_blank'>{1}</a>\n", strDialog, strNum, data[i].Id, arrSpeak[j], strGetTimeText);
                                totalNum += arrSpeakNum[j];
                                totalScore += arrSpeakScore[j];
                            }
                            else
                            {
                                strOther += "0\n";
                            }
                        }
                    }
                    for (int j = 0; j < arrReport.Count(); j++)
                    {
                        if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrReport[j] + ",") >= 0)
                        {
                            if (arrReportNum[j] > 0)
                            {
                                string strNum = (strType == "num") ? arrReportNum[j].ToString() : arrReportScore[j].ToString("n0");//调研得分
                                strOther += string.Format("<a href='{0}?ac=score&UserId={2}&Title={3}&TableName=tb_Report&time={4}' target='_blank'>{1}</a>\n", strDialog, strNum, data[i].Id, arrReport[j], strGetTimeText);
                                totalNum += arrReportNum[j];
                                totalScore += arrReportScore[j];
                            }
                            else
                            {
                                strOther += "0\n";
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf(",活动及发言统计,") >= 0)
                    {
                        string strNum = (strType == "num") ? countNum.ToString() : countScore.ToString("n2");//活动及发言
                        strOther += strNum + "\n";
                    }
                    if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf(",提供资源,") >= 0)
                    {
                        if (resNum > 0)
                        {
                            string strNum = (strType == "num") ? resNum.ToString() : resScore.ToString("n0");//提供资源得分
                            strOther += string.Format("<a href='{0}?ac=score&UserId={2}&TableName=tb_Perform_Res&time={3}' target='_blank'>{1}</a>\n", strDialog, strNum, data[i].Id, strGetTimeText);
                            totalNum += resNum;
                            totalScore += resScore;
                        }
                        else
                        {
                            strOther += "0\n";
                        }
                    }
                    if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf(",提案,") >= 0)
                    {
                        if (opinNum > 0)
                        {
                            string strNum = (strType == "num") ? opinNum.ToString() : opinScore.ToString("n2");//提案得分
                            strOther += string.Format("<a href='{0}?ac=score&UserId={2}&TableName=tb_Opinion&time={3}' target='_blank'>{1}</a>\n", strDialog, strNum, data[i].Id, strGetTimeText);
                            totalNum += opinNum;
                            totalScore += opinScore;
                        }
                        else
                        {
                            strOther += "0\n";
                        }
                    }
                    if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf(",社情民意,") >= 0)
                    {
                        if (popNum > 0)
                        {
                            string strNum = (strType == "num") ? popNum.ToString() : popScore.ToString("n2");//社情民意得分
                            strOther += string.Format("<a href='{0}?ac=score&UserId={2}&TableName=tb_Opinion_Pop&time={3}' target='_blank'>{1}</a>\n", strDialog, strNum, data[i].Id, strGetTimeText);
                            totalNum += popNum;
                            totalScore += popScore;
                        }
                        else
                        {
                            strOther += "0\n";
                        }
                    }
                    if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf(",扣分项,") >= 0)
                    {
                        if (deScoreNum > 0)
                        {
                            string strNum = (strType == "num") ? deScoreNum.ToString() : deScore.ToString("n0");//扣分项
                            totalNum += deScoreNum;
                            totalScore += deScore;
                            strOther += string.Format("<a href='{0}?ac=score&UserId={2}&TableName=tb_Perform_De&time={3}' target='_blank'>{1}</a>\n", strDialog, strNum, data[i].Id, strGetTimeText);
                        }
                        else
                        {
                            strOther += "0\n";
                        }
                    }
                    if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf(",其他得分,") >= 0)
                    {
                        if (otherScoreNum > 0)
                        {
                            string strNum = (strType == "num") ? otherScoreNum.ToString() : otherScore.ToString("n2");//其他得分
                            totalNum += otherScoreNum;
                            totalScore += otherScore;
                            strOther += string.Format("<a href='{0}?ac=score&UserId={2}&TableName=other&time={3}' target='_blank'>{1}</a>\n", strDialog, strNum, data[i].Id, strGetTimeText);
                        }
                        else
                        {
                            strOther += "0\n";
                        }
                    }
                    if (strType == "num")
                    {
                        data[i].UserScore = totalNum;
                        strOther += totalNum.ToString("n0");
                    }
                    else
                    {
                        data[i].UserScore = totalScore;
                        strOther += totalScore.ToString("n2");
                    }
                    data[i].other = strOther;
                }
                if (pageSize > 0)
                {
                    showUser(strThead, data);
                    int pageCount = data[0].total;
                    int pageLast = (pageCount % pageSize > 0) ? (pageCount / pageSize + 1) : (pageCount / pageSize);
                    string lnk = Request.Url.ToString();
                    lblUserNav.Text = HelperMain.LoadPageNav(lnk, pageCur, pageLast);
                    ltUserTotal.Text = pageCount.ToString();
                }
                else
                {
                    //排序
                    for (int i = 1; i < data.Count(); i++)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            //总积分降序
                            if (data[i].UserScore > data[j].UserScore)
                            {
                                DataUser tmp = data[j];
                                data[j] = data[i];
                                data[i] = tmp;
                            }
                        }
                    }
                    //showUser(strThead, data); return;
                    //ltUserTotal.Text = data.Count().ToString();
                    downXls("委员积分", strThead, data);
                }
            }
        }
        //显示委员积分情况
        private void showUser(string strThead, DataUser[] data)
        {
            string strTable = "<table>";
            if (!string.IsNullOrEmpty(strThead))
            {
                string[] arr = strThead.Split('\n');
                strTable += "<thead><tr>";
                for (int i = 0; i < arr.Count(); i++)
                {
                    strTable += "<th>" + arr[i].Replace("|", "<br/>") + "</th>";
                }
                strTable += "</tr></thead>";
            }
            strTable += "<tbody>";
            for (int i = 0; i < data.Count(); i++)
            {
                strTable += string.Format("<tr><th>{0}</th>", data[i].UserCode);
                if (!string.IsNullOrEmpty(data[i].other))
                {
                    string[] arr = data[i].other.Split('\n');
                    for (int j = 0; j < arr.Count(); j++)
                    {
                        string strTd = (j < arr.Count() - 1) ? arr[j] : "<b>" + arr[j] + "</b>";
                        strTable += string.Format("<td align='center'>{0}</td>", strTd);
                    }
                }
                strTable += "</tr>";
            }
            strTable += "</tbody></table>";
            ltUserTable.Text = strTable;
        }
        //下载委员/团体积分情况
        private void downXls(string strTitle, string strThead, DataUser[] data)
        {
            if (myUser == null)
            {
                return;
            }
            DateTime dtNow = DateTime.Now;
            string virtualPath = string.Format("../download/{0:yyyy}/{0:MM}/", dtNow);
            string filepath = HttpContext.Current.Server.MapPath(virtualPath);
            if (!System.IO.Directory.Exists(filepath))
            {
                System.IO.Directory.CreateDirectory(filepath);
            }
            string strFileName = string.Format("{0}_{1:yyyyMMddHHmmss}.xls", strTitle, dtNow);
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
            workSheet.Rows["3"].RowHeight = 20;
            workSheet.Cells[3, 1].Font.Size = 16;//设置字体大小
            workSheet.Range[workSheet.Cells[3, 1], workSheet.Cells[3, 11]].MergeCells = true;//合并单元格
            PublicMod.SetCells(workSheet, 3, 1, strTitle, "center,bold");
            workSheet.Rows["4"].RowHeight = 40;
            PublicMod.SetCells(workSheet, 4, 1, "序号", "fit,center,bold,border", "LightGray");
            PublicMod.SetCells(workSheet, 4, 2, "委员编号", "fit,center,bold,border", "LightGray");
            PublicMod.SetCells(workSheet, 4, 3, "姓名", "center,bold,border", "LightGray");
            PublicMod.SetCells(workSheet, 4, 4, "性别", "fit,center,bold,border", "LightGray");
            PublicMod.SetCells(workSheet, 4, 5, "政治面貌", "center,bold,border", "LightGray");
            PublicMod.SetCells(workSheet, 4, 6, "电话", "center,bold,border", "LightGray");
            PublicMod.SetCells(workSheet, 4, 7, "界别", "center,bold,border", "LightGray");
            workSheet.Cells[4, 7].ColumnWidth = 10;
            PublicMod.SetCells(workSheet, 4, 8, "专委会", "center,bold,border", "LightGray");
            workSheet.Cells[4, 8].ColumnWidth = 20;
            PublicMod.SetCells(workSheet, 4, 9, "街道活动组", "center,bold,border", "LightGray");
            workSheet.Cells[4, 9].ColumnWidth = 20;
            PublicMod.SetCells(workSheet, 4, 10, "单位性质", "fit,center,bold,border", "LightGray");
            PublicMod.SetCells(workSheet, 4, 11, "单位及职务", "fit,center,bold,border", "LightGray");
            if (!string.IsNullOrEmpty(strThead))
            {
                string[] arr = strThead.Split('\n');
                for (int i = 2; i < arr.Count(); i++)
                {
                    PublicMod.SetCells(workSheet, 4, i + 10, arr[i].Replace("|", "\n"), "fit,center,bold,border", "LightGray");
                }
                workSheet.Cells[4, 9 + arr.Count()].ColumnWidth = 10;
            }
            //PublicMod.SetCells(workSheet, 4, 12, "积分", "center,bold,border", "LightGray");
            int lenOrgName = "单位及职务".Length;
            for (int i = 0; i < data.Count(); i++)
            {
                string[] arr = data[i].other.Split('\n');
                PublicMod.SetCells(workSheet, i + 5, 1, (i + 1).ToString(), "txt,center,border");
                PublicMod.SetCells(workSheet, i + 5, 2, data[i].UserCode, "txt,center,border");
                PublicMod.SetCells(workSheet, i + 5, 3, data[i].TrueName, "txt,center,border");//arr[0]
                PublicMod.SetCells(workSheet, i + 5, 4, data[i].UserSex, "txt,center,border");
                PublicMod.SetCells(workSheet, i + 5, 5, data[i].Party, "txt,center,border");
                PublicMod.SetCells(workSheet, i + 5, 6, data[i].Mobile, "fit,txt,center,border");
                PublicMod.SetCells(workSheet, i + 5, 7, data[i].Subsector, "txt,center,border");
                string strComm = data[i].Committee;
                if (!string.IsNullOrEmpty(data[i].Committee2))
                {
                    strComm += "、" + data[i].Committee2;
                }
                PublicMod.SetCells(workSheet, i + 5, 8, strComm, "txt,border");
                PublicMod.SetCells(workSheet, i + 5, 9, data[i].StreetTeam, "txt,border");
                PublicMod.SetCells(workSheet, i + 5, 10, data[i].OrgType, "txt,center,border");
                string attrOrgName = "txt,border";
                if (data[i].OrgName.Length > lenOrgName)
                {
                    lenOrgName = data[i].OrgName.Length;
                    attrOrgName += ",fit";
                }
                PublicMod.SetCells(workSheet, i + 5, 11, data[i].OrgName, attrOrgName);
                for (int j = 1; j < arr.Count(); j++)
                {
                    string strTmp = arr[j];
                    string strUrl = "";
                    string strTxt = "";
                    if (strTmp.IndexOf("<a") >= 0)
                    {
                        strTmp = strTmp.Substring(strTmp.IndexOf("href='") + 6);
                        strUrl = strTmp.Substring(0, strTmp.IndexOf("'"));
                        strTmp = strTmp.Substring(strTmp.IndexOf(">") + 1);
                        strTxt = strTmp.Substring(0, strTmp.IndexOf("</"));
                    }
                    else
                    {
                        strTxt = strTmp;
                    }
                    PublicMod.SetCells(workSheet, i + 5, 11 + j, strTxt, "txt,center,border", "", strUrl);//积分
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
        //团组统计
        private void listTeam(string strGetTimeText, string strUserType, string[] arrTeam, string strQFields, string[] arrPlay, string[] arrSpeak, string[] arrReport, string[] arrOther)
        {
            string strThead = "<tr><th>" + strUserType + "</th><th>人数</th>";
            for (int i = 0; i < arrPlay.Count(); i++)
            {
                if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrPlay[i] + ",") >= 0)
                {
                    strThead += "<th>" + arrPlay[i].Replace("|", "<br/>") + "</th>";
                }
            }
            for (int i = 0; i < arrSpeak.Count(); i++)
            {
                if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrSpeak[i] + ",") >= 0)
                {
                    strThead += "<th>" + arrSpeak[i] + "</th>";
                }
            }
            for (int i = 0; i < arrReport.Count(); i++)
            {
                if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrReport[i] + ",") >= 0)
                {
                    strThead += "<th>" + arrReport[i].Replace("|", "<br/>") + "</th>";
                }
            }
            for (int i = 0; i < arrOther.Count(); i++)
            {
                if (arrOther[i] == "活动及发言统计")
                {

                }
                else if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrOther[i] + ",") >= 0)
                {
                    strThead += "<th>" + arrOther[i] + "</th>";
                }
            }
            string strType = rblCountType.SelectedValue;
            strThead += (strType == "num") ? "<th>统计次数</th>" : "<th>总分</th>";
            strThead += "</tr>";
            string strTbody = "";
            WebUser webUser = new WebUser();
            WebUserScore webScore = new WebUserScore();
            for (int i = 0; i < arrTeam.Count(); i++)
            {
                int intNum = 0;
                int[] arrPlayNum = new int[arrPlay.Count()];
                decimal[] arrPlayScore = new decimal[arrPlay.Count()];
                int[] arrSpeakNum = new int[arrSpeak.Count()];
                decimal[] arrSpeakScore = new decimal[arrSpeak.Count()];
                int[] arrReportNum = new int[arrPlay.Count()];
                decimal[] arrReportScore = new decimal[arrPlay.Count()];
                int countNum = 0;//活动及发言统计次数
                decimal countScore = 0;//活动及发言统计分数
                int resNum = 0;//提供资源次数
                decimal resScore = 0;//提供资源分数
                int opinNum = 0;//提案次数
                decimal opinScore = 0;//提案分数
                int popNum = 0;//社情民意次数
                decimal popScore = 0;//社情民意数
                int deScoreNum = 0;//扣分次数
                decimal deScore = 0;//扣分
                int otherScoreNum = 0;//其他得分次数
                decimal otherScore = 0;//其他得分
                int totalNum = 0;//总次数
                decimal totalScore = 0;//总分

                DataUser qUser = new DataUser();
                qUser.ActiveName = ">0";
                qUser.Period = config.PERIOD;
                qUser.OrderScore = 1;
                switch (strUserType)
                {
                    case "专委会":
                        qUser.Committee = arrTeam[i];
                        break;
                    case "界别":
                        qUser.Subsector = arrTeam[i];
                        break;
                    case "街道活动组":
                        qUser.StreetTeam = arrTeam[i];
                        break;
                    case "党派团体":
                        if (arrTeam[i] == "工商联" || arrTeam[i] == "侨联")
                        {
                            qUser.Subsector = arrTeam[i];
                        }
                        else
                        {
                            qUser.Party = arrTeam[i];
                        }
                        break;
                    default:
                        qUser.UserType = "委员";
                        qUser.UserCode = "14%";
                        return;
                }
                DataUser[] userData = webUser.GetDatas(qUser, "Id,TrueName");
                if (userData != null)
                {
                    intNum = userData.Count() - 1;
                    //string strDebug = "";
                    string strUserId = "";
                    for (int j = 0; j < userData.Count(); j++)
                    {
                        if (!string.IsNullOrEmpty(strUserId))
                        {
                            strUserId += ",";
                        }
                        strUserId += userData[j].Id.ToString();
                        //strDebug += "," + userData[j].TrueName;
                    }
                    //if (myUser.AdminName == "Tony" && (arrTeam[i] == "民盟" || arrTeam[i] == "民建" || arrTeam[i] == "工商联"))
                    //{
                    //    arrTeam[i] += "<br/>" + strDebug.Trim(',');
                    //}
                    DataUserScore[] scoreData = webScore.GetDatas(1, strUserId, "", 0, "", strGetTimeText, "Title,Score,TableName,TableId", 1, 0, "TableName ASC, TableId ASC, GetTime ASC");//(统计表)委员积分列表
                    if (scoreData != null)
                    {
                        for (int j = 0; j < scoreData.Count(); j++)
                        {
                            switch (scoreData[j].TableName)
                            {
                                case "tb_Perform_Feed"://会议/活动、发言、提供资源、扣分
                                    if (scoreData[j].Score < 0)
                                    {
                                        deScoreNum++;
                                        deScore += scoreData[j].Score;
                                    }
                                    else if (scoreData[j].Title.IndexOf("提供资源") >= 0)
                                    {
                                        resNum++;
                                        resScore += scoreData[j].Score;
                                    }
                                    else if (scoreData[j].Title.IndexOf("发言") >= 0)
                                    {
                                        for (int m = 0; m < arrSpeak.Count(); m++)
                                        {
                                            if (scoreData[j].Title.IndexOf(arrSpeak[m]) >= 0)
                                            {
                                                arrSpeakNum[m]++;
                                                arrSpeakScore[m] += scoreData[j].Score;
                                                countNum++;
                                                countScore += scoreData[j].Score;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int m = 0; m < arrPlay.Count(); m++)
                                        {
                                            if (scoreData[j].Title.IndexOf(arrPlay[m].Replace("|", "、")) >= 0)
                                            {
                                                arrPlayNum[m]++;
                                                arrPlayScore[m] += scoreData[j].Score;
                                                countNum++;
                                                countScore += scoreData[j].Score;
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                case "tb_Report"://调研报告
                                    for (int m = 0; m < arrReport.Count(); m++)
                                    {
                                        if (scoreData[j].Title.IndexOf(arrReport[m].Replace("|", "")) >= 0)
                                        {
                                            arrReportNum[m]++;
                                            arrReportScore[m] += scoreData[j].Score;
                                            countNum++;
                                            countScore += scoreData[j].Score;
                                            break;
                                        }
                                    }
                                    break;
                                case "tb_Opinion"://提案
                                    opinNum++;
                                    opinScore += scoreData[j].Score;
                                    break;
                                case "tb_Opinion_Pop"://社情民意
                                    popNum++;
                                    popScore += scoreData[j].Score;
                                    break;
                                default:
                                    otherScoreNum++;
                                    otherScore += scoreData[j].Score;
                                    break;
                            }
                        }
                    }
                }

                string strTr = string.Format("<td><a href='{0}?ac=score&UserId={1}&UserType={2}&time={4}' target='_blank'>{1}</a></td><td>{3}</td>", strDialog, arrTeam[i], strUserType, intNum, strGetTimeText);

                for (int j = 0; j < arrPlay.Count(); j++)
                {
                    if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrPlay[j] + ",") >= 0)
                    {
                        if (arrPlayNum[j] > 0)
                        {
                            string strNum = (strType == "num") ? arrPlayNum[j].ToString() : arrPlayScore[j].ToString("n0");
                            strTr += string.Format("<td align='center'><a href='{0}?ac=score&UserId={2}&UserType={3}&Title={4}&TableName=tb_Perform_Feed&time={5}' target='_blank'>{1}</a></td>", strDialog, strNum, arrTeam[i], strUserType, arrPlay[j], strGetTimeText);
                            totalNum += arrPlayNum[j];
                            totalScore += arrPlayScore[j];
                        }
                        else
                        {
                            strTr += "<td align='center'>0</td>";
                        }
                    }
                }
                for (int j = 0; j < arrSpeak.Count(); j++)
                {
                    if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrSpeak[j] + ",") >= 0)
                    {
                        if (arrSpeakNum[j] > 0)
                        {
                            string strNum = (strType == "num") ? arrSpeakNum[j].ToString() : arrSpeakScore[j].ToString("n0");
                            strTr += string.Format("<td align='center'><a href='{0}?ac=score&UserId={2}&UserType={3}&Title={4}&TableName=tb_Perform_Speak&time={5}' target='_blank'>{1}</a></td>", strDialog, strNum, arrTeam[i], strUserType, arrSpeak[j], strGetTimeText);
                            totalNum += arrSpeakNum[j];
                            totalScore += arrSpeakScore[j];
                        }
                        else
                        {
                            strTr += "<td align='center'>0</td>";
                        }
                    }
                }
                for (int j = 0; j < arrReport.Count(); j++)
                {
                    if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf("," + arrReport[j] + ",") >= 0)
                    {
                        if (arrReportNum[j] > 0)
                        {
                            string strNum = (strType == "num") ? arrReportNum[j].ToString() : arrReportScore[j].ToString("n0");
                            strTr += string.Format("<td align='center'><a href='{0}?ac=score&UserId={2}&UserType={3}&Title={4}&TableName=tb_Report&time={5}' target='_blank'>{1}</a></td>", strDialog, strNum, arrTeam[i], strUserType, arrReport[j], strGetTimeText);
                            totalNum += arrReportNum[j];
                            totalScore += arrReportScore[j];
                        }
                        else
                        {
                            strTr += "<td align='center'>0</td>";
                        }
                    }
                }
                //if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf(",活动及发言统计,") >= 0)
                //{
                //    string strNum = (strType == "num") ? countNum.ToString() : countScore.ToString("n2");//活动及发言
                //    strTr += "<td align='center'>" + strNum + "</td>";
                //}
                if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf(",提供资源,") >= 0)
                {
                    if (resNum > 0)
                    {
                        string strNum = (strType == "num") ? resNum.ToString() : resScore.ToString("n0");
                        strTr += string.Format("<td align='center'><a href='{0}?ac=score&UserId={2}&UserType={3}&TableName=tb_Perform_Res&time={4}' target='_blank'>{1}</a></td>", strDialog, strNum, arrTeam[i], strUserType, strGetTimeText);
                        totalNum += resNum;
                        totalScore += resScore;
                    }
                    else
                    {
                        strTr += "<td align='center'>0</td>";
                    }
                }
                if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf(",提案,") >= 0)
                {
                    if (opinNum > 0)
                    {
                        string strNum = (strType == "num") ? opinNum.ToString() : opinScore.ToString("n0");
                        strTr += string.Format("<td align='center'><a href='{0}?ac=score&UserId={2}&UserType={3}&TableName=tb_Opinion&time={4}' target='_blank'>{1}</a></td>", strDialog, strNum, arrTeam[i], strUserType, strGetTimeText);
                        totalNum += opinNum;
                        totalScore += opinScore;
                    }
                    else
                    {
                        strTr += "<td align='center'>0</td>";
                    }
                }
                if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf(",社情民意,") >= 0)
                {
                    if (popNum > 0)
                    {
                        string strNum = (strType == "num") ? popNum.ToString() : popScore.ToString("n2");
                        strTr += string.Format("<td align='center'><a href='{0}?ac=score&UserId={2}&UserType={3}&TableName=tb_Opinion_Pop&time={4}' target='_blank'>{1}</a></td>", strDialog, strNum, arrTeam[i], strUserType, strGetTimeText);
                        totalNum += popNum;
                        totalScore += popScore;
                    }
                    else
                    {
                        strTr += "<td align='center'>0</td>";
                    }
                }
                if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf(",扣分项,") >= 0)
                {
                    if (deScoreNum > 0)
                    {
                        string strNum = (strType == "num") ? deScoreNum.ToString() : deScore.ToString("n0");
                        totalNum += deScoreNum;
                        totalScore += deScore;
                        strTr += string.Format("<td align='center'><a href='{0}?ac=score&UserId={2}&UserType={3}&TableName=tb_Perform_De&time={4}' target='_blank'>{1}</a></td>", strDialog, strNum, arrTeam[i], strUserType, strGetTimeText);
                    }
                    else
                    {
                        strTr += "<td align='center'>0</td>";
                    }
                }
                if (string.IsNullOrEmpty(strQFields) || strQFields.IndexOf(",其他得分,") >= 0)
                {
                    if (otherScoreNum > 0)
                    {
                        string strNum = (strType == "num") ? otherScoreNum.ToString() : otherScore.ToString("n0");
                        totalNum += otherScoreNum;
                        totalScore += otherScore;
                        strTr += string.Format("<td align='center'><a href='{0}?ac=score&UserId={2}&UserType={3}&TableName=other&time={4}' target='_blank'>{1}</a></td>", strDialog, strNum, arrTeam[i], strUserType, strGetTimeText);
                    }
                    else
                    {
                        strTr += "<td align='center'>0</td>";
                    }
                }

                string strTotal = (strType == "num") ? totalNum.ToString() : totalScore.ToString("n2");

                strTbody += string.Format("<tr>{0}<td align='center'><b>{1}</b></td></tr>", strTr, strTotal);
            }
            ltUserTable.Text = string.Format("<table><thead>{0}</thead><tbody>{1}</tbody></table>", strThead, strTbody);
            ltUserTotal.Text = arrTeam.Count().ToString();
        }
        #endregion
        //
        #region 遴选评优
        //评选
        protected void btnApp_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            List<DataApp> data = new List<DataApp>();
            WebUser webUser = new WebUser();
            WebUserScore webScore = new WebUserScore();
            ltAppScore.Text = (rblAppCountType.SelectedValue == "次数") ? "总次数" : "总分";
            string strGetTimeText = txtAppDate1.Text + "," + txtAppDate2.Text;
            DataUser qUser = new DataUser();
            qUser.ActiveName = ">0";
            qUser.OrderScore = 1;//记分人员
            qUser.Period = config.PERIOD;
            if (rblAppUserType.SelectedValue == "委员")
            {
                ltAppUserName.Text = "姓名";
                ltAppUserCode.Text = "委员编号";
                qUser.UserType = "委员";
                string strFields = "Id,UserCode,TrueName";//"Id,UserCode,TrueName,Birthday,Committee,Committee2,Subsector,Subsector2";//
                DataUser[] userData = webUser.GetDatas(qUser, strFields);
                if (userData != null)
                {
                    for (int i = 0; i < userData.Count(); i++)
                    {
                        DataApp app = totalApp(webScore, userData[i].Id.ToString(), strGetTimeText);
                        app.UserName = userData[i].TrueName;
                        app.UserCode = userData[i].UserCode;
                        data.Add(app);
                    }
                }
            }
            else
            {
                ltAppUserName.Text = "名称";
                ltAppUserCode.Text = "人数";
                WebOp webOp = new WebOp();
                string strUserType = (rblAppUserType.SelectedValue == "党派") ? "政治面貌" : rblAppUserType.SelectedValue;
                DataOp[] opData = webOp.GetDatas(1, strUserType, "", "OpName");
                if (opData != null)
                {
                    for (int i = 0; i < opData.Count(); i++)
                    {
                        string strTeam = opData[i].OpName;
                        switch (rblAppUserType.SelectedValue)
                        {
                            case "专委会":
                                qUser.Committee = strTeam;
                                if (qUser.Committee == "教科卫体委员会")
                                {
                                    qUser.Committee += "%";
                                }
                                break;
                            case "界别":
                                qUser.Subsector = strTeam;
                                break;
                            case "街道活动组":
                                qUser.StreetTeam = strTeam;
                                break;
                            case "党派":
                                qUser.Party = strTeam;
                                break;
                            default:
                                return;
                        }
                        DataApp app = new DataApp();
                        DataUser[] userData = webUser.GetDatas(qUser, "Id,UserType");
                        if (userData != null)
                        {
                            int intUserNum = 0;
                            string strUserId = "";
                            for (int j = 0; j < userData.Count(); j++)
                            {
                                if (!string.IsNullOrEmpty(strUserId))
                                {
                                    strUserId += ",";
                                }
                                strUserId += userData[j].Id.ToString();
                                if (userData[j].UserType == "委员")
                                {
                                    intUserNum++;
                                }
                            }
                            app = totalApp(webScore, strUserId, strGetTimeText);
                            app.UserName = strTeam;
                            app.UserCode = intUserNum.ToString();//userData.Count().ToString();//团体人数
                        }
                        data.Add(app);
                    }
                }
            }
            if (data.Count > 0)
            {
                for (int i = 1; i < data.Count(); i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        switch (rblAppType.SelectedValue)
                        {
                            case "会议/活动":
                                if (rblAppOrder.SelectedValue == "升序")
                                {
                                    if ((data[i].ScorePlay < data[j].ScorePlay) || (data[i].ScorePlay == data[j].ScorePlay && data[i].ScoreTotal < data[j].ScoreTotal))
                                    {
                                        DataApp tmp = data[j];
                                        data[j] = data[i];
                                        data[i] = tmp;
                                    }
                                }
                                else
                                {
                                    if ((data[i].ScorePlay > data[j].ScorePlay) || (data[i].ScorePlay == data[j].ScorePlay && data[i].ScoreTotal > data[j].ScoreTotal))
                                    {
                                        DataApp tmp = data[j];
                                        data[j] = data[i];
                                        data[i] = tmp;
                                    }
                                }
                                break;
                            case "会议发言":
                                if (rblAppOrder.SelectedValue == "升序")
                                {
                                    if ((data[i].ScoreSpeak < data[j].ScoreSpeak) || (data[i].ScoreSpeak == data[j].ScoreSpeak && data[i].ScoreTotal < data[j].ScoreTotal))
                                    {
                                        DataApp tmp = data[j];
                                        data[j] = data[i];
                                        data[i] = tmp;
                                    }
                                }
                                else
                                {
                                    if ((data[i].ScoreSpeak > data[j].ScoreSpeak) || (data[i].ScoreSpeak == data[j].ScoreSpeak && data[i].ScoreTotal > data[j].ScoreTotal))
                                    {
                                        DataApp tmp = data[j];
                                        data[j] = data[i];
                                        data[i] = tmp;
                                    }
                                }
                                break;
                            case "调研报告":
                                if (rblAppOrder.SelectedValue == "升序")
                                {
                                    if ((data[i].ScoreReport < data[j].ScoreReport) || (data[i].ScoreReport == data[j].ScoreReport && data[i].ScoreTotal < data[j].ScoreTotal))
                                    {
                                        DataApp tmp = data[j];
                                        data[j] = data[i];
                                        data[i] = tmp;
                                    }
                                }
                                else
                                {
                                    if ((data[i].ScoreReport > data[j].ScoreReport) || (data[i].ScoreReport == data[j].ScoreReport && data[i].ScoreTotal > data[j].ScoreTotal))
                                    {
                                        DataApp tmp = data[j];
                                        data[j] = data[i];
                                        data[i] = tmp;
                                    }
                                }
                                break;
                            case "提供资源":
                                if (rblAppOrder.SelectedValue == "升序")
                                {
                                    if ((data[i].ScoreResource < data[j].ScoreResource) || (data[i].ScoreResource == data[j].ScoreResource && data[i].ScoreTotal < data[j].ScoreTotal))
                                    {
                                        DataApp tmp = data[j];
                                        data[j] = data[i];
                                        data[i] = tmp;
                                    }
                                }
                                else
                                {
                                    if ((data[i].ScoreResource > data[j].ScoreResource) || (data[i].ScoreResource == data[j].ScoreResource && data[i].ScoreTotal > data[j].ScoreTotal))
                                    {
                                        DataApp tmp = data[j];
                                        data[j] = data[i];
                                        data[i] = tmp;
                                    }
                                }
                                break;
                            case "提案":
                                if (rblAppOrder.SelectedValue == "升序")
                                {
                                    if ((data[i].ScoreOpinion < data[j].ScoreOpinion) || (data[i].ScoreOpinion == data[j].ScoreOpinion && data[i].ScoreTotal < data[j].ScoreTotal))
                                    {
                                        DataApp tmp = data[j];
                                        data[j] = data[i];
                                        data[i] = tmp;
                                    }
                                }
                                else
                                {
                                    if ((data[i].ScoreOpinion > data[j].ScoreOpinion) || (data[i].ScoreOpinion == data[j].ScoreOpinion && data[i].ScoreTotal > data[j].ScoreTotal))
                                    {
                                        DataApp tmp = data[j];
                                        data[j] = data[i];
                                        data[i] = tmp;
                                    }
                                }
                                break;
                            case "社情民意":
                                if (rblAppOrder.SelectedValue == "升序")
                                {
                                    if ((data[i].ScorePop < data[j].ScorePop) || (data[i].ScorePop == data[j].ScorePop && data[i].ScoreTotal < data[j].ScoreTotal))
                                    {
                                        DataApp tmp = data[j];
                                        data[j] = data[i];
                                        data[i] = tmp;
                                    }
                                }
                                else
                                {
                                    if ((data[i].ScorePop > data[j].ScorePop) || (data[i].ScorePop == data[j].ScorePop && data[i].ScoreTotal > data[j].ScoreTotal))
                                    {
                                        DataApp tmp = data[j];
                                        data[j] = data[i];
                                        data[i] = tmp;
                                    }
                                }
                                break;
                            case "扣分项":
                                if (rblAppOrder.SelectedValue == "升序")
                                {
                                    if ((data[i].ScoreDe < data[j].ScoreDe) || (data[i].ScoreDe == data[j].ScoreDe && data[i].ScoreTotal < data[j].ScoreTotal))
                                    {
                                        DataApp tmp = data[j];
                                        data[j] = data[i];
                                        data[i] = tmp;
                                    }
                                }
                                else
                                {
                                    if ((data[i].ScoreDe > data[j].ScoreDe) || (data[i].ScoreDe == data[j].ScoreDe && data[i].ScoreTotal > data[j].ScoreTotal))
                                    {
                                        DataApp tmp = data[j];
                                        data[j] = data[i];
                                        data[i] = tmp;
                                    }
                                }
                                break;
                            default://按总分
                                if (rblAppOrder.SelectedValue == "升序")
                                {
                                    if ((data[i].ScoreTotal < data[j].ScoreTotal) || (data[i].ScoreTotal == data[j].ScoreTotal && data[i].ScoreDe > data[j].ScoreDe))
                                    {
                                        DataApp tmp = data[j];
                                        data[j] = data[i];
                                        data[i] = tmp;
                                    }
                                }
                                else
                                {
                                    if ((data[i].ScoreTotal > data[j].ScoreTotal) || (data[i].ScoreTotal == data[j].ScoreTotal && data[i].ScoreDe < data[j].ScoreDe))
                                    {
                                        DataApp tmp = data[j];
                                        data[j] = data[i];
                                        data[i] = tmp;
                                    }
                                }
                                break;
                        }
                    }
                }
                plApp.Visible = true;
                ltAppTotal.Text = data.Count.ToString();
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].num = i + 1;
                    if (rblAppCountType.SelectedValue == "次数")
                    {
                        data[i].ScorePlayText = data[i].ScorePlay.ToString("n0");
                        data[i].ScoreSpeakText = data[i].ScoreSpeak.ToString("n0");
                        data[i].ScoreReportText = data[i].ScoreReport.ToString("n0");
                        data[i].ScoreResourceText = data[i].ScoreResource.ToString("n0");
                        data[i].ScoreOpinionText = data[i].ScoreOpinion.ToString("n0");
                        data[i].ScorePopText = data[i].ScorePop.ToString("n0");
                        data[i].ScoreDeText = data[i].ScoreDe.ToString("n0");
                        data[i].ScoreTotalText = data[i].ScoreTotal.ToString("n0");
                    }
                    else
                    {
                        data[i].ScorePlayText = data[i].ScorePlay.ToString("n2");
                        data[i].ScoreSpeakText = data[i].ScoreSpeak.ToString("n0");
                        data[i].ScoreReportText = data[i].ScoreReport.ToString("n0");
                        data[i].ScoreResourceText = data[i].ScoreResource.ToString("n0");
                        data[i].ScoreOpinionText = data[i].ScoreOpinion.ToString("n2");
                        data[i].ScorePopText = data[i].ScorePop.ToString("n2");
                        data[i].ScoreDeText = data[i].ScoreDe.ToString("n0");
                        data[i].ScoreTotalText = data[i].ScoreTotal.ToString("n2");
                    }
                }
                rpApp.DataSource = data;
                rpApp.DataBind();
            }
            else
            {
                plApp.Visible = false;
            }
        }
        //评选数据
        private class DataApp
        {
            public int num { get; set; }
            public string UserName { get; set; }
            public string UserCode { get; set; }
            public decimal ScorePlay { get; set; }
            public decimal ScoreSpeak { get; set; }
            public decimal ScoreReport { get; set; }
            public decimal ScoreResource { get; set; }
            public decimal ScoreOpinion { get; set; }
            public decimal ScorePop { get; set; }
            public decimal ScoreDe { get; set; }
            public decimal ScoreTotal { get; set; }
            public string ScorePlayText { get; set; }
            public string ScoreSpeakText { get; set; }
            public string ScoreReportText { get; set; }
            public string ScoreResourceText { get; set; }
            public string ScoreOpinionText { get; set; }
            public string ScorePopText { get; set; }
            public string ScoreDeText { get; set; }
            public string ScoreTotalText { get; set; }
        }
        //计分
        private DataApp totalApp(WebUserScore webScore, string strUserIds, string strGetTimeText)
        {
            DataApp app = new DataApp();
            DataUserScore[] scoreData = webScore.GetDatas(1, strUserIds, "", 0, "", strGetTimeText, "Title,Score,TableName,TableId", 1, 0, "TableName ASC, TableId ASC, GetTime ASC");//(统计表)委员积分列表
            if (scoreData != null)
            {
                for (int j = 0; j < scoreData.Count(); j++)
                {
                    switch (scoreData[j].TableName)
                    {
                        case "tb_Perform_Feed"://会议/活动、发言、提供资源、扣分
                            if (scoreData[j].Score < 0)
                            {
                                app.ScoreDe += (rblAppCountType.SelectedValue == "次数") ? 1 : scoreData[j].Score;
                            }
                            else if (scoreData[j].Title.IndexOf("提供资源") >= 0)
                            {
                                app.ScoreResource += (rblAppCountType.SelectedValue == "次数") ? 1 : scoreData[j].Score;
                            }
                            else if (scoreData[j].Title.IndexOf("发言") >= 0)
                            {
                                app.ScoreSpeak += (rblAppCountType.SelectedValue == "次数") ? 1 : scoreData[j].Score;
                            }
                            else
                            {
                                app.ScorePlay += (rblAppCountType.SelectedValue == "次数") ? 1 : scoreData[j].Score;
                            }
                            break;
                        case "tb_Report"://调研报告
                            app.ScoreReport += (rblAppCountType.SelectedValue == "次数") ? 1 : scoreData[j].Score;
                            break;
                        case "tb_Opinion"://提案
                            app.ScoreOpinion += (rblAppCountType.SelectedValue == "次数") ? 1 : scoreData[j].Score;
                            break;
                        case "tb_Opinion_Pop"://社情民意
                            app.ScorePop += (rblAppCountType.SelectedValue == "次数") ? 1 : scoreData[j].Score;
                            break;
                        default:
                            break;
                    }
                    app.ScoreTotal += (rblAppCountType.SelectedValue == "次数") ? 1 : scoreData[j].Score;
                }
            }
            return app;
        }
        #endregion
        //
        #region 其它统计
        //社情民意
        protected void btnPop_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            int intYear = DateTime.Today.Year;
            DateTime dtStart = new DateTime(intYear, 1, 1);
            DateTime dtEnd = new DateTime(intYear, 12, 31);
            DataOpinionPop qPop = new DataOpinionPop();
            qPop.ActiveName = "待审核,已录用,留存";
            qPop.SubTimeText = string.Format("{0:yyyy-MM-dd},{1:yyyy-MM-dd}", dtStart, dtEnd);
            WebOpinionPop webPop = new WebOpinionPop();
            DataOpinionPop[] pData = webPop.GetDatas(qPop);
            if (pData == null)
            {
                return;
            }
            string strOut = "";
            strOut += "<br/>社情民意：" + pData.Count().ToString() + "篇";
            DataUser qUser = new DataUser();
            qUser.Period = config.PERIOD;
            qUser.UserType = "委员";
            qUser.ActiveName = ">0";
            qUser.UserCode = "14%";
            WebUser webUser = new WebUser();
            DataUser[] uData = webUser.GetDatas(qUser, "TrueName");
            if (uData != null)
            {
                strOut += "<br/>委员：" + uData.Count().ToString() + "人";
                for (int i = 0; i < uData.Count(); i++)
                {
                    for (int j = 0; j < pData.Count(); j++)
                    {
                        if (pData[j].SubMan == uData[i].TrueName || pData[j].SubMans.IndexOf("," + uData[i].TrueName + ",") >= 0)
                        {
                            uData[i].UserScore++;
                        }
                    }
                }
                //排序
                for (int i = 1; i < uData.Count(); i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        //总积分降序
                        if (uData[i].UserScore > uData[j].UserScore)
                        {
                            DataUser tmp = uData[j];
                            uData[j] = uData[i];
                            uData[i] = tmp;
                        }
                    }
                }
                int noPop = 0;
                for (int i = 0; i < uData.Count(); i++)
                {
                    if (uData[i].UserScore <= 0)
                    {
                        noPop++;
                    }
                }
                int n = 10;
                if (uData.Count() - noPop < n)
                {
                    n = uData.Count() - noPop;
                }
                strOut += "<br/><br/>前10名名单：";
                decimal deScore = 0;
                int num = 0;
                for (int i = 0; i < uData.Count(); i++)
                {
                    if (uData[i].UserScore < uData[n - 1].UserScore)
                    {
                        break;
                    }
                    if (deScore == 0 || uData[i].UserScore < deScore)
                    {
                        deScore = uData[i].UserScore;
                        num = i + 1;
                    }
                    strOut += "<br/>" + num.ToString() + "、" + uData[i].TrueName + "，" + uData[i].UserScore.ToString() + "篇";
                }
                strOut += "<br/><br/>未参与人数：" + noPop.ToString();
                if (noPop > 0)
                {
                    strOut += "，百分比：" + (Convert.ToDecimal(noPop) / Convert.ToDecimal(uData.Count()) * 100).ToString("n2") + "%";
                }
            }

            lblResult.Text = strOut;
        }
        #endregion
        //
    }
}