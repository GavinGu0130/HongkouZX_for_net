using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using mod.main;
using hkzx.db;

namespace hkzx.web
{
    public class PublicMod
    {
        //获取返回URL
        public static string GetBackUrl(string strBack = "")
        {
            if (!string.IsNullOrEmpty(strBack))
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["url"]))
                {
                    strBack = HttpContext.Current.Request.QueryString["url"];
                }
                else if (HttpContext.Current.Request.UrlReferrer != null)
                {
                    strBack = HttpContext.Current.Request.UrlReferrer.ToString();
                }
                if (strBack.IndexOf("login.aspx") >= 0 || strBack.IndexOf("user.aspx") >= 0)
                {
                    strBack = "./";
                }
            }
            else
            {
                strBack = HttpContext.Current.Request.Url.ToString();
                if (HttpContext.Current.Request.UrlReferrer != null)
                {
                    strBack = HttpContext.Current.Request.UrlReferrer.ToString();
                }
                if (strBack.IndexOf("?id=") >= 0)
                {
                    strBack = strBack.Substring(0, strBack.IndexOf("?id="));
                }
                else if (strBack.IndexOf("&id=") >= 0)
                {
                    strBack = strBack.Substring(0, strBack.IndexOf("&id="));
                }
            }
            return strBack;
        }
        //格式化时间
        public static string GetTimeText(DateTime StartTime, DateTime EndTime)
        {
            string strTime = StartTime.ToString("yyyy年M月d日 HH:mm");
            if (StartTime.Year != EndTime.Year)
            {
                strTime += " - " + EndTime.ToString("yyyy年M月d日 HH:mm");
            }
            else if (StartTime.Month != EndTime.Month)
            {
                strTime += " - " + EndTime.ToString("M月d日 HH:mm");
            }
            else if (StartTime.Day != EndTime.Day)
            {
                strTime += " - " + EndTime.ToString("d日 HH:mm");
            }
            else
            {
                strTime += " - " + EndTime.ToString("HH:mm");
            }
            return strTime;
        }
        //格式化主办、会办单位
        public static string GetOrgText(string strOrg)
        {
            if (strOrg.IndexOf("|") >= 0)
            {
                strOrg = strOrg.Substring(0, strOrg.IndexOf("|"));
            }
            return strOrg;
        }
        //添加CheckBoxList子项
        public static void LoadCheckBoxList(CheckBoxList checkList, WebOp webData, string OpType, string selVal = "")
        {
            DataOp[] data = webData.GetDatas(1, OpType, "", "OpName,OpValue,Selected");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    string strName = data[i].OpName;
                    string strVal = (!string.IsNullOrEmpty(data[i].OpValue)) ? data[i].OpValue : strName;
                    ListItem item = new ListItem(strName, strVal);
                    if (selVal == "*")
                    {
                        item.Selected = true;
                    }
                    else if (selVal == "_")
                    {
                    }
                    else if (selVal != "")
                    {
                        if (("," + selVal + ",").IndexOf("," + strVal + ",") >= 0)
                        {
                            item.Selected = true;
                        }
                    }
                    else if (data[i].Selected)
                    {
                        item.Selected = true;
                    }
                    checkList.Items.Add(item);
                }
            }
        }
        //添加RadioButtonList子项
        public static void LoadRadioButtonList(RadioButtonList radioList, WebOp webData, string OpType, string selVal = "")
        {
            DataOp[] data = webData.GetDatas(1, OpType, "", "OpName,OpValue,Selected");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    string strName = data[i].OpName;
                    string strVal = (!string.IsNullOrEmpty(data[i].OpValue)) ? data[i].OpValue : strName;
                    ListItem item = new ListItem(strName, strVal);
                    radioList.Items.Add(item);
                    if (selVal == "*")
                    {
                        //radioList.SelectedIndex = intSel - 1;
                        //intSel = -1;
                    }
                    else if (selVal == "_")
                    {
                    }
                    else if (selVal != "")
                    {
                        if (strVal == selVal)
                        {
                            radioList.SelectedIndex = radioList.Items.Count - 1;
                            selVal = "_";
                        }
                    }
                    else if (data[i].Selected)
                    {
                        radioList.SelectedIndex = radioList.Items.Count - 1;
                        selVal = "_";
                    }
                }
            }
        }
        //添加DropDownList子项
        public static void LoadDropDownList(DropDownList ddlList, WebOp webData, string OpType, string selVal = "")
        {
            DataOp[] data = webData.GetDatas(1, OpType, "", "OpName,OpValue,Selected");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    string strName = data[i].OpName;
                    string strVal = (!string.IsNullOrEmpty(data[i].OpValue)) ? data[i].OpValue : strName;
                    ListItem item = new ListItem(strName, strVal);
                    ddlList.Items.Add(item);
                    if (selVal == "*")
                    {
                        //ddlList.SelectedIndex = intSel - 1;
                        //intSel = -1;
                    }
                    else if (selVal == "_")
                    {
                    }
                    else if (selVal != "")
                    {
                        if (strVal == selVal)
                        {
                            ddlList.SelectedIndex = ddlList.Items.Count - 1;
                            selVal = "_";
                        }
                    }
                    else if (data[i].Selected)
                    {
                        ddlList.SelectedIndex = ddlList.Items.Count - 1;
                        selVal = "_";
                    }
                }
            }
        }
        //加载DropDownList用的二维数组字符串
        public static void LoadDropDownLists(HiddenField hf, DropDownList ddlList, string OpName, DataUser uData = null, WebOp webOp = null)
        {
            string strOrg = "";
            if (webOp == null)
            {
                webOp = new WebOp();
            }
            List<int> list = new List<int>();
            for (int i = 0; i < ddlList.Items.Count; i++)
            {
                string strVal = ddlList.Items[i].Text.Trim();
                if (!string.IsNullOrEmpty(strVal))
                {
                    if (uData != null)
                    {
                        switch (strVal)
                        {
                            case "专委会":
                                if (string.IsNullOrEmpty(uData.Committee) && string.IsNullOrEmpty(uData.Committee2))
                                {
                                    list.Add(i);
                                    continue;
                                }
                                break;
                            case "界别":
                                if (string.IsNullOrEmpty(uData.Subsector))
                                {
                                    list.Add(i);
                                    continue;
                                }
                                break;
                            case "界别活动组":
                                if (string.IsNullOrEmpty(uData.Subsector2))
                                {
                                    list.Add(i);
                                    continue;
                                }
                                break;
                            case "街道活动组":
                                if (string.IsNullOrEmpty(uData.StreetTeam))
                                {
                                    list.Add(i);
                                    continue;
                                }
                                break;
                            case "政治面貌":
                                if (string.IsNullOrEmpty(uData.Party))
                                {
                                    list.Add(i);
                                    continue;
                                }
                                break;
                            default:
                                list.Add(i);
                                continue;
                        }
                    }
                    DataOp[] opData = webOp.GetDatas(1, strVal, "", OpName);//加载子选项
                    strOrg += "['" + strVal + "'";
                    if (opData != null)
                    {
                        for (int j = 0; j < opData.Count(); j++)
                        {
                            if (uData != null)
                            {
                                switch (strVal)
                                {
                                    case "专委会":
                                        if (opData[j].OpName != uData.Committee && opData[j].OpName != uData.Committee2)
                                        {
                                            continue;
                                        }
                                        break;
                                    case "界别":
                                        if (opData[j].OpName != uData.Subsector)
                                        {
                                            continue;
                                        }
                                        break;
                                    case "界别活动组":
                                        if (opData[j].OpName != uData.Subsector2)
                                        {
                                            continue;
                                        }
                                        break;
                                    case "街道活动组":
                                        if (opData[j].OpName != uData.StreetTeam)
                                        {
                                            continue;
                                        }
                                        break;
                                    case "政治面貌":
                                        if (uData.Party.IndexOf(opData[j].OpName) < 0)
                                        {
                                            continue;
                                        }
                                        break;
                                    default:
                                        continue;
                                }
                            }
                            string strTmp = (!string.IsNullOrEmpty(opData[j].OpValue)) ? opData[j].OpValue : opData[j].OpName;
                            strOrg += ",'" + strTmp + "'";
                        }
                    }
                    strOrg += "],";
                }
            }
            //[['专委会', '宝山区', '崇明县'], ['界别活动组', '昌平区', '朝阳区'], ['街道活动组', '宝坻区', '北辰区']]
            if (strOrg != "")
            {
                strOrg = "[" + strOrg.Trim(',') + "]";
            }
            int[] item = list.ToArray();
            if (item.Count() > 0)
            {
                for (int i = item.Count() - 1; i >= 0; i--)
                {
                    ddlList.Items.Remove(ddlList.Items[item[i]]);
                }
            }
            hf.Value = strOrg;
        }
        //
        //加载履职活动类型
        public static void LoadPerformType(DropDownList ddlList, string ScoreType)
        {
            WebScore webScore = new WebScore();
            DataScore[] data = webScore.GetDatas(1, ScoreType, "", "", "Title");
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    ddlList.Items.Add(data[i].Title);
                }
            }
        }
        //加载民族
        public static void LoadNation(DropDownList ddlList)
        {
            string[] arr = { "汉族", "阿昌族", "白族", "保安族", "布朗族", "布依族", "藏族", "朝鲜族", "达斡尔族", "傣族", "德昂族", "东乡族", "侗族", "独龙族", "俄罗斯族", "鄂伦春族", "鄂温克族", "高山族", "哈尼族", "哈萨克族", "赫哲族", "回族", "基诺族", "京族", "景颇族", "柯尔克孜族", "拉祜族", "黎族", "珞巴族", "满族", "毛南族", "门巴族", "蒙古族", "苗族", "仫佬族", "纳西族", "怒族", "普米族", "羌族", "撒拉族", "畲族", "水族", "僳僳族", "塔吉克族", "塔塔尔族", "土家族", "土族", "佤族", "维吾尔族", "乌孜别克族", "锡伯族", "瑶族", "彝族", "仡佬族", "裕固族", "壮族" };
            for (int i = 0; i < arr.Count(); i++)
            {
                ddlList.Items.Add(arr[i]);
            }
        }
        //加载文化程度
        public static void LoadEducation(DropDownList ddlList)
        {
            string[] arr = { "研究生", "本科", "大专", "高中", "中专", "初中", "小学" };
            for (int i = 0; i < arr.Count(); i++)
            {
                ddlList.Items.Add(arr[i]);
            }
        }
        //增加浏览反馈
        public static void AddFeed(string TableName, int TableId, DataUser uData)
        {
            WebFeedback webFeed = new WebFeedback();
            DataFeedback data = new DataFeedback();
            data.TableName = TableName;
            data.TableId = TableId;
            data.UserId = uData.Id;
            data.Active = 1;

            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = HelperMain.SqlFilter(uData.TrueName, 20);
            DataFeedback[] ckData = webFeed.GetDatas("", data.TableName, data.TableId, data.UserId, "Id");//重复检查
            if (ckData != null)
            {
                data.Id = ckData[0].Id;
                data.UpTime = dtNow;
                data.UpIp = strIp;
                data.UpUser = strUser;
                data.Id = webFeed.Update(data);//更新浏览时间
            }
            else
            {
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = strUser;
                data.Id = webFeed.Insert(data);//新增浏览时间
            }
        }
        //添加积分
        public static int AddScore(int UserId, string Title, decimal Score, string TableName, int TableId, string AddIp, string AddUser, DateTime GetTime, string strRemark = "")
        {
            if (Score == 0)
            {
                return 0;//0积分不添加
            }
            WebUserScore webScore = new WebUserScore();
            if (!string.IsNullOrEmpty(TableName) && TableId > 0)
            {
                if (Title == "立案提案" || Title == "立案提案（第一提案人）" || Title == "立案提案（联名）")
                {
                    int intYear = DateTime.Today.Year;
                    DateTime dtStart = new DateTime(intYear - 1, 12, 1);
                    if (GetTime < dtStart)
                    {
                        return 0;//超过获得积分时间
                    }
                    else if (GetTime.Year < intYear)
                    {
                        GetTime = new DateTime(intYear, 1, 1);//前一年提案，积分获得时间修改为1月1日
                    }
                    string GetTimeText = dtStart.ToString("yyyy-MM-dd") + "," + intYear.ToString() + "-11-30 23:59:59";//提案积分从前一年12月1日起--当年11月30日止
                    string strTitle = "立案提案%";
                    DataUserScore[] ckData = webScore.GetDatas(0, UserId.ToString(), TableName, 0, strTitle, GetTimeText, "Id,Title,Score,TableId,Active", 1, 0, "Active DESC, UpTime DESC, AddTime DESC");
                    if (ckData != null)
                    {
                        int intScore = 0;
                        decimal deScore = 0;
                        decimal deScore2 = 0;
                        for (int i = 0; i < ckData.Count(); i++)
                        {
                            int ckId = 0;
                            if (ckData[i].TableId == TableId)
                            {
                                ArrayList arrList = new ArrayList();
                                for (int j = 0; j < ckData.Count(); j++)
                                {//检查重复积分
                                    if (ckData[j].TableId == TableId)
                                    {
                                        arrList.Add(ckData[j].Id);
                                    }
                                }
                                if (arrList.Count > 0)
                                {
                                    webScore.UpdateActive(arrList, -1);//取消重复积分
                                    ckData[i].Active = 0;
                                }
                                ckId = ckData[i].Id;
                            }
                            else if (ckData[i].Active > 0)
                            {
                                deScore += ckData[i].Score;
                                if (ckData[i].Title.IndexOf("联名") > 0)
                                {
                                    deScore2 += ckData[i].Score;
                                }
                            }
                            if (deScore >= 9)
                            {
                                intScore = -2;
                                //return -2;//“立案提案”此项每人最高9分
                            }
                            else if (deScore2 >= 3)
                            {
                                intScore = -3;
                                //return -3;//“联名提案”每人最多3分
                            }
                            else
                            {
                                if (deScore + Score > 9)
                                {
                                    Score = 9 - deScore;
                                    if (Title.IndexOf("（超过最高分限制）") < 0)
                                    {
                                        Title += "（超过最高分限制）";
                                    }
                                }
                                else if (Score < 3 && deScore2 + Score > 3)
                                {//联名提案
                                    Score = 3 - deScore2;
                                    if (Title.IndexOf("（超过最高分限制）") < 0)
                                    {
                                        Title += "（超过最高分限制）";
                                    }
                                }
                                if (ckId > 0)
                                {
                                    if (ckData[i].Title != Title || ckData[i].Score != Score)
                                    {
                                        webScore.UpdateScore(ckId, Score, Title);//更新立案提案得分
                                    }
                                    else if (ckData[i].Active <= 0)
                                    {
                                        webScore.UpdateActive(ckId, 1);//更新积分状态
                                    }
                                    intScore = -1;
                                    //return -1;//已添加过此项目积分
                                }
                            }
                        }
                        if (intScore < 0)
                        {
                            return intScore;
                        }
                    }
                    //HttpContext.Current.Response.Write(GetTimeText); HttpContext.Current.Response.End();
                }
                else
                {
                    if (TableName == "tb_Perform_Feed")
                    {
                        if (Title.IndexOf("大会发言") >= 0 || Title.IndexOf("专题会发言") >= 0 || Title.IndexOf("会议发言") >= 0 || Title.IndexOf("提供资源") >= 0)
                        {

                        }
                        else
                        {
                            DataUserScore[] ckData1 = webScore.GetDatas(1, UserId.ToString(), TableName, TableId, "出席-%", "", "");
                            if (ckData1 != null)
                            {
                                for (int i = 0; i < ckData1.Count(); i++)
                                {
                                    ckData1[i].Active = 0;
                                    webScore.Update(ckData1[i]);//取消，因会议名称变改，而计分的积分
                                }
                            }
                            DataUserScore[] ckData2 = webScore.GetDatas(1, UserId.ToString(), TableName, TableId, "%-未出席", "", "");
                            if (ckData2 != null)
                            {
                                for (int i = 0; i < ckData2.Count(); i++)
                                {
                                    ckData2[i].Active = 0;
                                    webScore.Update(ckData2[i]);//取消，因会议名称变改，而计分的积分
                                }
                            }
                        }
                    }
                    DataUserScore[] ckData = webScore.GetDatas(0, UserId.ToString(), TableName, TableId, Title, "", "");//Id,UserId,Title,Score,TableName,TableId,Active
                    if (ckData != null)
                    {
                        for (int i = 0; i < ckData.Count(); i++)
                        {
                            if (ckData[i].Active <= 0)
                            {
                                //webScore.UpdateActive(ckData[i].Id, 1);
                                ckData[i].Active = 1;
                            }
                            ckData[i].GetTime = GetTime;
                            ckData[i].Score = Score;
                            webScore.Update(ckData[i]);
                        }
                        return -1;//已添加过此项目积分
                    }
                }
            }
            DataUserScore data = new DataUserScore();
            data.UserId = UserId;
            data.Title = Title;
            data.Score = Score;
            data.TableName = TableName;
            data.TableId = TableId;
            data.Remark = strRemark;
            data.Active = 1;
            data.AddTime = DateTime.Now;
            data.AddIp = AddIp;
            data.AddUser = AddUser;
            data.GetTime = GetTime;
            return webScore.Insert(data);//添加成功返回Id值
        }
        //
        //积分记录查询
        public static string GetScoreOther(DataUserScore data, WebOpinion webOpin, WebOpinionPop webPop, WebReport webReport, WebPerform webPerform, WebPerformFeed webFeed)
        {
            string strOut = "";
            switch (data.TableName)
            {
                case "tb_Opinion":
                    DataOpinion[] opData = webOpin.GetData(data.TableId, "Summary,SubTime,AddTime");
                    if (opData != null)
                    {
                        if (opData[0].SubTime > DateTime.MinValue)
                        {
                            strOut = opData[0].SubTime.ToString("yyyy年M月d日 ");
                        }
                        strOut += string.Format("提案《{0}》", opData[0].Summary);
                    }
                    break;
                case "tb_Opinion_Pop":
                    DataOpinionPop[] popData = webPop.GetData(data.TableId, "Summary,SubTime,AddTime");
                    if (popData != null)
                    {
                        if (popData[0].SubTime > DateTime.MinValue)
                        {
                            strOut = popData[0].SubTime.ToString("yyyy年M月d日 ");
                        }
                        strOut += string.Format("社情民意《{0}》", popData[0].Summary);
                    }
                    break;
                case "tb_Report":
                    DataReport[] rData = webReport.GetData(data.TableId, "Title,SubTime,AddTime");
                    if (rData != null)
                    {
                        if (rData[0].SubTime > DateTime.MinValue)
                        {
                            strOut = rData[0].SubTime.ToString("yyyy年M月d日 ");
                        }
                        strOut += string.Format("调研报告《{0}》", rData[0].Title);
                    }
                    break;
                case "tb_Perform_Feed":
                    DataPerformFeed[] fData = webFeed.GetData(data.TableId, "PerformId");
                    if (fData != null)
                    {
                        DataPerform[] pData = webPerform.GetData(fData[0].PerformId, "OrgName,Title,StartTime");
                        if (pData != null)
                        {
                            string strOrgName = "";
                            if (!string.IsNullOrEmpty(pData[0].OrgName))
                            {
                                string[] arr = pData[0].OrgName.Split(',');
                                for (int i = 0; i < arr.Count(); i++)
                                {
                                    if (arr[i].IndexOf("-") > 0)
                                    {
                                        arr[i] = arr[i].Substring(arr[i].IndexOf("-") + 1);
                                    }
                                }
                                strOrgName = string.Join(",", arr);
                            }
                            strOut = string.Format("{0} {2:yyyy年M月d日}《{1}》", strOrgName, pData[0].Title, pData[0].StartTime);
                        }
                    }
                    break;
                default:
                    break;
            }
            return strOut;
        }
        //
        //发送微信消息
        public static string SendTemplateMsg(string openid, string strMode, string url, string first, string remark, string word1, string word2, string word3 = "", string word4 = "")
        {
            string str = "";
            string template_id = "";//报名状态通知
            string strData = "";
            switch (strMode)
            {
                case "信息发布":
                    //{{first.DATA}}
                    //提醒内容：{{keyword1.DATA}}
                    //时间：{{keyword2.DATA}}
                    //{{remark.DATA}}
                    template_id = "sdIBqT8tGGnYJHk3hJe0sjJOInqz6Z0m115ux1yCTmc";//工作任务提醒
                    strData = "\"first\": { \"value\":\"" + first + "\" }, \"keyword1\": { \"value\":\"" + word1 + "\", \"color\":\"#173177\" }, \"keyword2\": { \"value\":\"" + word2 + "\", \"color\":\"#173177\" }, \"remark\": { \"value\":\"" + remark + "\", \"color\":\"#666666\" }";
                    break;
                case "政协-会议通知":
                    //{{first.DATA}}
                    //名称：{{keyword1.DATA}}
                    //时间：{{keyword2.DATA}}
                    //地点：{{keyword3.DATA}}
                    //{{remark.DATA}}
                    template_id = "qdmXb0cFIxsFDDuKtlrNvp9ofKQol160gtRD4CPIoyU";//会议通知
                    strData = "\"first\": { \"value\":\"" + first + "\" }, \"keyword1\": { \"value\":\"" + word1 + "\", \"color\":\"#173177\" }, \"keyword2\": { \"value\":\"" + word2 + "\", \"color\":\"#173177\" }, \"keyword3\": { \"value\":\"" + word3 + "\", \"color\":\"#173177\" }, \"remark\": { \"value\":\"" + remark + "\", \"color\":\"#666666\" }";
                    break;
                case "会议通知":
                    //{{first.DATA}}
                    //会议名称：{{keyword1.DATA}}
                    //会议时间：{{keyword2.DATA}}
                    //会议地点：{{keyword3.DATA}}
                    //会议介绍：{{keyword4.DATA}}
                    //{{remark.DATA}}
                    template_id = "OA14vq8DWb14US6a7LVJS91zI4rYIt0u5xGKMxXfYLg";//会议通知
                    strData = "\"first\": { \"value\":\"" + first + "\" }, \"keyword1\": { \"value\":\"" + word1 + "\", \"color\":\"#173177\" }, \"keyword2\": { \"value\":\"" + word2 + "\", \"color\":\"#173177\" }, \"keyword3\": { \"value\":\"" + word3 + "\", \"color\":\"#173177\" }, \"keyword4\": { \"value\":\"" + word4 + "\", \"color\":\"#173177\" }, \"remark\": { \"value\":\"" + remark + "\", \"color\":\"#666666\" }";
                    break;
                default:
                    return "未发送模板消息！";
            }
            string json = "{ \"touser\":\"" + openid + "\", \"template_id\":\"" + template_id + "\", \"url\":\"" + url + "\", \"data\":{" + strData + "} }";
            WxAPI.JsApi wxApi = new WxAPI.JsApi();
            string access_token = wxApi.GetAccessToken(config.APPID, config.APPSECRET, config.TOKEN);//获取（基础支持）access_token
            int timeout = 30;
            WxAPI.WxData data = wxApi.SendTemplateMessage(access_token, json, timeout);
            if (data != null)
            {
                str = string.Format("errcode：{0}\nerrmsg：{1}\nmsgid：{2}", data.GetValue("errcode"), data.GetValue("errmsg"), data.GetValue("msgid"));
            }
            else
            {
                str = "发送失败！";
            }
            return str;
        }
        //发送短信
        //向数据库中新增消息
        public static bool AddSendMsg(WebSendMsg webSendMsg, string TableName, int TableId, int UserId, string Body, string Remark, string strIp, string strUser, string Label)
        {
            DataSendMsg data = new DataSendMsg();
            data.TableName = TableName;
            data.TableId = TableId;
            data.UserId = UserId;
            data.Body = Body;
            data.Remark = Remark;
            data.Active = 1;
            data.AddTime = DateTime.Now;
            data.AddIp = strIp;
            data.AddUser = strUser;
            data.Label = Label;
            webSendMsg.Insert(data);
            return true;
        }
        //格式化excel单元格
        public static void SetCells(Microsoft.Office.Interop.Excel.Worksheet workSheet, int row, int col, string txt, string attr = "", string backColor = "", string strUrl = "")
        {
            Microsoft.Office.Interop.Excel.Range range = workSheet.Cells[row, col];
            if (attr.IndexOf("txt") >= 0)
            {
                range.NumberFormatLocal = "@";//设置单元格格式为文本
            }
            else if (attr.IndexOf("date") >= 0)
            {
                range.NumberFormatLocal = @"yyyy-mm-dd";//日期型格式
            }
            range.Value = txt;//workSheet.Cells[row, col] = txt;
            if (attr.IndexOf("wrap") >= 0)
            {
                range.WrapText = true;
            }
            if (attr.IndexOf("fit") >= 0)
            {
                range.Columns.AutoFit();//设置单元格宽度为自适应
            }
            if (attr.IndexOf("center") >= 0)
            {
                range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            }
            if (attr.IndexOf("bold") >= 0)
            {
                range.Font.Bold = true;
            }
            if (attr.IndexOf("border") >= 0)
            {
                range.BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, null);
            }
            switch (backColor)
            {
                case "LightGray":
                    range.Interior.Color = System.Drawing.Color.LightGray;
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(strUrl))
            {
                workSheet.Hyperlinks.Add(range, strUrl);
            }
        }
        //获取提案联名人
        public static string GetSubMans(int OpId, string SubMans, int intMod = 0)
        {
            if (string.IsNullOrEmpty(SubMans))
            {
                return "";
            }
            WebOpinionSign webSign = new WebOpinionSign();
            string[] arr = SubMans.Split(',');
            SubMans = "";
            for (int i = 0; i < arr.Count(); i++)
            {
                DataOpinionSign qSign = new DataOpinionSign();
                qSign.OpType = "提案";
                qSign.OpId = OpId;
                qSign.SignUser = arr[i];
                DataOpinionSign[] data = webSign.GetDatas(qSign, "Overdue,Active");
                if (data != null)
                {
                    if (data[0].Active > 0)
                    {
                        SubMans += "," + arr[i];
                    }
                    else if (data[0].Active == 0 && intMod <= 0)
                    {
                        SubMans += "," + arr[i];
                        if (intMod == 0)
                        {//*待会签，~会签过期
                            if (data[0].Overdue > DateTime.Now)
                            {
                                SubMans += "*";
                            }
                            else
                            {
                                SubMans += "~";
                            }
                        }
                    }
                }
            }
            return SubMans.Trim(',');
        }
        //社情民意详细采用情况
        public static string GetPopFeed(DataOpinionPop data)
        {
            string strEmploy = "";
            if (!string.IsNullOrEmpty(data.Adopt1))
            {
                strEmploy += "<b>采用：</b>" + data.Adopt1 + "<br/>";
            }
            if (!string.IsNullOrEmpty(data.Give1))
            {
                strEmploy += "<b>主送：</b>" + data.Give1 + "<br/>";
            }
            if (!string.IsNullOrEmpty(data.Employ1))
            {
                strEmploy += data.Employ1 + "<br/>";
            }
            if (!string.IsNullOrEmpty(data.Reply1))
            {
                strEmploy += "<b>区领导批示：</b>" + data.Reply1.Replace("\n", "<br/>") + "<br/>";
            }
            if (!string.IsNullOrEmpty(data.Adopt2))
            {
                strEmploy += "<b>采用：</b>" + data.Adopt2 + "<br/>";
            }
            if (!string.IsNullOrEmpty(data.Send2))
            {
                strEmploy += data.Send2 + "<br/>";
            }
            if (!string.IsNullOrEmpty(data.Give2))
            {
                strEmploy += data.Give2 + "<br/>";
            }
            if (!string.IsNullOrEmpty(data.Employ2))
            {
                strEmploy += data.Employ2 + "<br/>";
            }
            if (!string.IsNullOrEmpty(data.Reply2))
            {
                strEmploy += "<b>市领导批示：</b>" + data.Reply2.Replace("\n", "<br/>") + "<br/>";
            }
            if (!string.IsNullOrEmpty(data.Send3))
            {
                strEmploy += "<b>" + data.Send3 + "</b>：";
                if (!string.IsNullOrEmpty(data.Give3))
                {
                    strEmploy += data.Give3;
                }
                if (!string.IsNullOrEmpty(data.Employ3))
                {
                    strEmploy += "，" + data.Employ3;
                }
                if (!string.IsNullOrEmpty(data.Reply3))
                {
                    strEmploy += "<br/><b>中央领导批示：</b>" + data.Reply3.Replace("\n", "<br/>");
                }
            }
            return strEmploy;
        }
        //用户积分排序
        public static void UserScoreOrder(DataUser[] data)
        {
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
        }
        //按积分重新排序
        public static void ReOrderScore(DataUser myUser)
        {
            if (myUser.OrderTime >= DateTime.Today)
            {
                return;
            }
            DataUser qUser = new DataUser();
            qUser.Period = config.PERIOD;
            qUser.UserType = "委员";
            qUser.ActiveName = ">0";
            qUser.OrderScore = 1;
            WebUser webUser = new WebUser();
            DataUser[] data = webUser.GetDatas(qUser, "Id");
            if (data != null)
            {
                WebUserScore webScore = new WebUserScore();
                int intYear = DateTime.Today.Year;
                string strStart = intYear.ToString() + "-1-1";
                string strEnd = intYear.ToString() + "-12-31";
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i].UserScore = webScore.GetTotalScore(data[i].Id, strStart, strEnd);
                }
                UserScoreOrder(data);//排序
                int intColor = data.Count() / 7;
                int Ranking = 0;
                for (int i = 0; i < data.Count(); i++)
                {
                    if (i == 0 || data[i - 1].UserScore > data[i].UserScore)
                    {//判断并列名次
                        Ranking = i + 1;
                    }
                    data[i].OrderScore = Ranking;
                    if (Ranking <= intColor)
                    {
                        data[i].OrderColor = "Red";
                    }
                    else if (Ranking <= intColor * 2)
                    {
                        data[i].OrderColor = "Orange";
                    }
                    else if (Ranking <= intColor * 3)
                    {
                        data[i].OrderColor = "Yellow";
                    }
                    else if (Ranking <= intColor * 4)
                    {
                        data[i].OrderColor = "Green";
                    }
                    else if (Ranking <= intColor * 5)
                    {
                        data[i].OrderColor = "Cyan";
                    }
                    else if (Ranking <= intColor * 6)
                    {
                        data[i].OrderColor = "Blue";
                    }
                    else
                    {
                        data[i].OrderColor = "Purple";
                    }
                    webUser.UpdateOrder(data[i].Id, data[i].OrderScore, data[i].OrderColor);//排名、颜色更新到库
                    if (data[i].Id == myUser.Id)
                    {
                        myUser.OrderColor = data[i].OrderColor;
                        myUser.OrderTime = DateTime.Now;
                        //hkzx.user.HelperUser.SetUser(myUser);
                    }
                }
            }
        }
        //写日志
        public static void WriteLog(string TableName, int TableId, string strLog, string strUser)
        {
            string strIp = HelperMain.GetIpPort();
            DateTime dtNow = DateTime.Now;
            DataLog data = new DataLog();
            data.TableName = TableName;
            data.TableId = TableId;
            data.Body = strLog;
            data.Active = 1;
            data.AddTime = dtNow;
            data.AddIp = strIp;
            data.AddUser = strUser;
            WebLog webLog = new WebLog();
            webLog.Insert(data);
        }
        //
    }
}