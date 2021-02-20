using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using mod.main;
using hkzx.db;
using hkzx.user;

namespace hkzx.web.admin
{
    public partial class output : System.Web.UI.Page
    {
        DataAdmin myUser = null;
        private string[,] arrBsort = { { "00", "" }, { "01", "财政经济" }, { "02", "城市建设与管理" }, { "03", "科教文卫体" }, { "04", "公安、法制、廉政建设" }, { "05", "劳动人事、体制编制" }, { "06", "其他" }, { "07", "经济" }, { "08", "城建、城管" }, { "09", "科教文卫体" }, { "10", "其他" } };//, { "10", "其它" }
        private string[,] arrNum = { { "10", "十" }, { "1", "一" }, { "2", "二" }, { "3", "三" }, { "4", "四" }, { "5", "五" }, { "6", "六" }, { "7", "七" }, { "8", "八" }, { "9", "九" } };
        private string[,] arrFlow = { { "1", "暂存" }, { "2", "外网提交" }, { "3", "提交" }, { "4", "预审通过" }, { "-4", "预审退回" }, { "5", "立案" }, { "-5", "不立案" }, { "6", "已分理" }, { "-6", "分理退回" }, { "7", "待反馈" }, { "8", "已反馈" }, { "9", "归档" }, { "-9", "重新办理" }, { "98", "转用" }, { "99", "已转用" }, { "100", "不转用" } };
        private string[,] arrSubManType = { { "0", "其他" }, { "1", "委员" }, { "2", "党派团体" }, { "3", "专委会" }, { "4", "界别" } };
        //
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
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.Powers = myUser.Powers;
            plNav.Visible = true;
            if (!IsPostBack)
            {
                hfUser.Value = myUser.AdminName;
                string strTitle = "";
                switch (Request.QueryString["ac"])
                {
                    case "first":
                        plFirst.Visible = true;
                        strTitle = "本地数据库导入一次提案数据";
                        break;
                    case "in":
                        plIn.Visible = true;
                        strTitle = "导入提案办理情况";
                        break;
                    default:
                        plOut.Visible = true;
                        string strYear = DateTime.Today.ToString("yyyy");
                        txtOutDate1.Text = strYear + "-01-01";
                        txtOutDate2.Text = strYear + "-12-31";
                        strTitle = "导出提案数据及委员反馈";
                        break;
                }
                Header.Title += " - " + strTitle;
            }
        }
        //
        #region 导出一次提案
        protected void btnFirst_Click(object sender, EventArgs e)
        {
            string ConnString = "Data Source=(local);Initial Catalog=" + txtData.Text.Trim() + ";Persist Security Info=True;User ID=" + txtUserId.Text.Trim() + ";Password=" + txtPwd.Text + ";";
            System.Data.SqlClient.SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(ConnString);//获取并打开数据库连接
            sqlConn.Open();//打开数据库
            System.Data.SqlClient.SqlCommand sqlCmd = sqlConn.CreateCommand();//创建数据库命令
            sqlCmd.CommandText = "SELECT * FROM aaabbb";//创建查询语句
            System.Data.SqlClient.SqlDataReader reader = sqlCmd.ExecuteReader();//从数据库中读取数据流存入reader中
            //从reader中读取下一行数据,如果没有数据,reader.Read()返回flase
            WebOpinion webOpinion = new WebOpinion();
            WebOp webOp = new WebOp();
            string strIp = HelperMain.GetIpPort();
            string strUser = myUser.AdminName;
            string strOut = "<b>导入提案：</b><hr/>";
            while (reader.Read())
            {
                //reader.GetOrdinal("id")是得到ID所在列的index,  
                //reader.GetInt32(int n)这是将第n列的数据以Int32的格式返回  
                //reader.GetString(int n)这是将第n列的数据以string 格式返回
                string op_num = reader.GetDouble(reader.GetOrdinal("提案序号")).ToString();
                string subMan = reader.GetString(reader.GetOrdinal("提案者"));
                string HostOrg = reader.GetString(reader.GetOrdinal("主办单位"));
                string HelpOrg = (reader.IsDBNull(reader.GetOrdinal("会办单位"))) ? "" : reader.GetString(reader.GetOrdinal("会办单位"));
                string dept_result = reader.GetString(reader.GetOrdinal("办理结果"));
                string OpinionFeed = (reader.IsDBNull(reader.GetOrdinal("意见反馈"))) ? "" : reader.GetString(reader.GetOrdinal("意见反馈"));
                string op_summary = reader.GetString(reader.GetOrdinal("案由"));
                string op_flow = reader.GetString(reader.GetOrdinal("性质"));
                string op_bsort = reader.GetString(reader.GetOrdinal("提案分类"));
                string op_Text = reader.GetString(reader.GetOrdinal("内容"));
                DateTime op_date = reader.GetDateTime(reader.GetOrdinal("提交日期"));
                string dept_opinion = reader.GetString(reader.GetOrdinal("主办部门答复"));
                //strOut += string.Format("提案序号：{0}<br/>提案者：{1}<br/>主办单位：{2}<br/>会办单位：{3}<br/>办理结果：{4}<br/>意见反馈：{5}<br/>案由：{6}<br/>性质：{7}<br/>提案分类：{8}<br/>内容：{9}<br/>提交日期：{10:yyyy-MM-dd HH:mm:ss.fff}<br/>主办部门答复：{11}<hr/>", op_num, subMan, HostOrg, HelpOrg, dept_result, OpinionFeed, op_summary, op_flow, op_bsort, "...", op_date, "...");

                string subMans = "";
                string SubManType = "委员";
                if (subMan.IndexOf(",") > 0)
                {
                    subMans = "," + subMan.Substring(subMan.IndexOf(",") + 1) + ",";
                    subMan = subMan.Substring(0, subMan.IndexOf(","));
                }
                else if (subMan.IndexOf("委员会") > 0 || subMan.IndexOf("工商业联合会") > 0)
                {
                    SubManType = "党派团体";
                }
                else if (subMan.IndexOf("小组提案") >= 0)
                {
                    SubManType = "小组、联组";
                }
                else if (subMan.IndexOf("青年联合会") > 0)
                {
                    SubManType = "界别";
                }
                subMan = "," + subMan + ",";
                HostOrg = getOrg(webOp, HostOrg);
                HelpOrg = getOrg(webOp, HelpOrg);
                string strBody = op_Text;
                while (strBody.IndexOf("<!--[if gte mso 9]>") >= 0 && strBody.IndexOf("<![endif]-->") > 0)
                {
                    string strTmp = strBody.Substring(strBody.IndexOf("<!--[if gte mso 9]>"), strBody.IndexOf("<![endif]-->") + "<![endif]-->".Length - strBody.IndexOf("<!--[if gte mso 9]>"));
                    strBody = strBody.Replace(strTmp, "");
                }
                op_Text = HelperMain.Html2Ubb(strBody);
                if (!string.IsNullOrEmpty(dept_opinion))
                {
                    dept_opinion = dept_opinion.Trim();
                }
                if (!string.IsNullOrEmpty(OpinionFeed))
                {
                    OpinionFeed = "意见反馈：" + OpinionFeed;
                }

                DataOpinion data = new DataOpinion();
                if (!string.IsNullOrEmpty(op_num))
                {
                    DataOpinion[] qData = webOpinion.GetDatas(op_num);//按提案号
                    if (qData != null)
                    {
                        data = qData[0];//获取提案
                    }
                }
                
                data.OpNo = op_num;
                data.SubMan = subMan;
                data.SubMans = subMans;
                data.SubManType = SubManType;
                data.ExamHostOrg = HostOrg;
                data.ExamHelpOrg = HelpOrg;
                data.ResultInfo = dept_result;
                if (!string.IsNullOrEmpty(OpinionFeed) && (string.IsNullOrEmpty(data.Remark) || data.Remark.IndexOf(OpinionFeed) < 0))
                {
                    if (!string.IsNullOrEmpty(data.Remark))
                    {
                        data.Remark += "\n";
                    }
                    data.Remark += OpinionFeed;
                }
                data.Summary = op_summary;
                data.ActiveName = op_flow;
                data.SubType = op_bsort;
                data.Body = op_Text;
                data.SubTime = op_date;
                data.ResultBody = dept_opinion;

                data.Period = "十四";
                data.Times = "一";
                data.IsSign = "";
                data.TimeMark = "";
                data.IsOpen = "是";
                data.ApplyState = "已办复";
                //data.UserId = 0;
                //data.AddUser = "";
                data.VerifyTime = DateTime.Now;
                data.VerifyIp = strIp;
                data.VerifyUser = strUser;

                if (data.Id <= 0)
                {
                    strOut += "新增";
                    data.Id = webOpinion.Insert(data);
                }
                else
                {
                    strOut += "更新";
                    if (webOpinion.Update(data) <= 0)
                    {
                        data.Id = -1;
                    }
                }

                if (data.Id > 0)
                {
                    strOut += " 成功";
                }
                else
                {
                    strOut += " 失败";
                }
                strOut += string.Format("：(流水号){0}，(提案号){1}，(案由){2}<br/>", data.Id, data.OpNo, data.Summary);
            }
            ltFirst.Text = strOut;
        }
        //承办单位
        private string getOrg(WebOp webOp, string strOrg)
        {
            string[] arr = strOrg.Split(',');
            for (int i = 0; i < arr.Count(); i++)
            {
                DataOp qOp = new DataOp();
                qOp.OpType = "承办单位";
                qOp.OpName = arr[i];
                DataOp[] opData = webOp.GetDatas(qOp, "OpValue");
                if (opData != null)
                {
                    arr[i] = opData[0].OpValue;
                }
            }
            return string.Join(",", arr);
        }
        #endregion
        //
        #region 导出
        //导出数据
        protected void btnOut2_Click(object sender, EventArgs e)
        {
            outOpinion(2);
        }
        protected void btnOut_Click(object sender, EventArgs e)
        {
            outOpinion();
        }
        private void outOpinion(int intMode = 0)
        {
            if (myUser == null || string.IsNullOrEmpty(txtOutDate1.Text) || string.IsNullOrEmpty(txtOutDate2.Text))
            {
                return;
            }
            DateTime dtNow = DateTime.Now;
            string strPath = "../upload/opinion/" + dtNow.ToString("yyyyMMdd") + "/";
            string strFolder = Server.MapPath(strPath);//保存文件的物理目录
            if (!System.IO.Directory.Exists(strFolder))
            {
                System.IO.Directory.CreateDirectory(strFolder);//新建保存文件的目录
            }
            ltOut.Text = "";

            DataOpinion qData = new DataOpinion();
            qData.Period = config.PERIOD;
            qData.Times = config.TIMES;
            qData.ActiveName = "待立案";//,不立案,立案
            qData.ApplyState = "<>'已办复'";
            //DateTime dtOutDate2 = (!string.IsNullOrEmpty(txtOutDate2.Text)) ? Convert.ToDateTime(txtOutDate2.Text) : dtNow;
            //if (dtOutDate2 > dtNow.AddHours(-3))
            //{
            //    txtOutDate2.Text = dtNow.AddHours(-3).ToString("yyyy-MM-dd HH:mm:ss");
            //}
            qData.SubTimeText = txtOutDate1.Text.Trim() + "," + txtOutDate2.Text.Trim();
            WebOpinion webOpinion = new WebOpinion();
            DataOpinion[] data = webOpinion.GetDatas(qData, "", 1, 0, "o.OpNo ASC, o.SubTime ASC, o.UpTime ASC, o.AddTime ASC");
            if (data != null)
            {
                string strFile = strPath + "out" + dtNow.ToString("yyyyMMddHHmmssffff") + ".xml";
                string strOut = getOpinion2(strPath, data);//提案数据
                if (intMode == 2)
                {
                    strOut = getOpinion2(strPath, data);//提案数据
                }
                else
                {
                    strOut = getOpinion(strPath, data);//提案数据
                }
                HelperFile.WriteFile(strFile, strOut);
                //HelperFile.DownloadXml(strFile, strOut);
                ltOut.Text += string.Format("<br /><b>xml数据文件链接：</b><a href='{0}' download='{0}' target='_blank'>{0}</a>", strFile);
            }
        }
        //提案数据（新）
        private string getOpinion2(string strPath, DataOpinion[] data)
        {
            int intNum = 0;
            string strIds = "";
            string noIds = "";
            string strOpinion = "";
            List<DataFiles> files = new List<DataFiles>();
            for (int i = 0; i < data.Count(); i++)
            {
                string strOutNot = txtOutNot.Text.Trim();
                if (!string.IsNullOrEmpty(strOutNot))
                {
                    strOutNot = "," + strOutNot.Trim(',') + ",";
                }
                if (!string.IsNullOrEmpty(strOutNot) && strOutNot.IndexOf("," + data[i].Id.ToString() + ",") >= 0)
                {//排除Id，不导出。
                    continue;
                }
                string strOpId = (!string.IsNullOrEmpty(data[i].OpNum)) ? data[i].OpNum : "w" + data[i].Id.ToString();
                string strOpNo = data[i].OpNo;
                string strSummary = data[i].Summary;
                string strSubType = data[i].SubType;
                string strSubManType = data[i].SubManType;
                string strSubMan = data[i].SubMan.Trim(',');
                if (!string.IsNullOrEmpty(strSubMan))
                {
                    strSubMan = strSubMan.Replace(",", "、");
                }
                string strSubMans = data[i].SubMans.Trim(',');
                if (!string.IsNullOrEmpty(strSubMans))
                {
                    strSubMans = PublicMod.GetSubMans(data[i].Id, strSubMans, 0);//联名会签检查//*待会签，~会签过期
                    if (!string.IsNullOrEmpty(strSubMans) && strSubMans.IndexOf("*") >= 0)
                    {//未完成会签，不导出。
                        noIds += "," + data[i].Id.ToString();
                        continue;
                    }
                    else if (!string.IsNullOrEmpty(strSubMans) && strSubMans.IndexOf("~") >= 0)
                    {//过滤会签过期委员
                        string strTmp = "";
                        string[] arr = strSubMans.Split(',');
                        for (int j = 0; j < arr.Count(); j++)
                        {
                            if (!string.IsNullOrEmpty(arr[j]) && arr[j].IndexOf("~") < 0)
                            {
                                strTmp += "," + arr[j];
                            }
                        }
                        strSubMans = strTmp.Trim(',');
                    }
                }
                if (!string.IsNullOrEmpty(strSubMans))
                {
                    strSubMans = strSubMans.Replace(",", "、");
                }
                string strBody = data[i].Body;
                strBody = HelperMain.DelUbbFont(strBody);//删除字体
                strBody = HelperMain.Ubb2Html(strBody);//ubb转html
                //strBody = HelperMain.ReplaceImg(strBody);//去除图片
                string strRemark = data[i].Remark;
                if (!string.IsNullOrEmpty(strRemark))
                {
                    strRemark = "<![CDATA[" + strRemark + "]]>";
                }
                string strFilesPath = "";
                string[] arrFiles = data[i].Files.Split('|');
                for (int j = 0; j < arrFiles.Count(); j++)
                {
                    if (!string.IsNullOrEmpty(arrFiles[j]))
                    {
                        string strOld = arrFiles[j];
                        string strExt = "";
                        int intExt = strOld.LastIndexOf(".");
                        if (intExt > 0 && intExt > strOld.LastIndexOf("/"))
                        {
                            strExt = strOld.Substring(intExt);
                        }
                        string strTitle = "w" + data[i].Id.ToString();//重新按流水号命名
                        if (j > 0)
                        {
                            strTitle += "_" + j.ToString();//多附件加后缀
                        }
                        strTitle += strExt;//扩展名
                        string strUrl = strPath + strTitle;//新的链接
                        string strFile = Server.MapPath(strUrl);//物理地址
                        if (!System.IO.File.Exists(strFile))
                        {
                            string strTmp = Server.MapPath(arrFiles[j]);
                            if (System.IO.File.Exists(strTmp))
                            {
                                System.IO.File.Copy(strTmp, strFile);//复制新文件
                            }
                        }
                        DataFiles file = new DataFiles();
                        file.Title = strTitle;
                        file.Url = strUrl;
                        files.Add(file);
                        strFilesPath += "\n\t\t\t\t<附件><![CDATA[" + strUrl + "]]></附件>";
                    }
                }
                string strIsOpen = data[i].IsOpen;
                string strTimeMark = data[i].TimeMark;
                string strSubTime = (data[i].SubTime > DateTime.MinValue) ? data[i].SubTime.ToString("yyyy/MM/dd HH:mm:ss") : "";
                string strAdviseHostOrg = data[i].AdviseHostOrg;
                string strAdviseHelpOrg = data[i].AdviseHelpOrg;

                strOpinion += "\n\t\t\t<Row>\n\t\t\t\t<流水号>" + strOpId + "</流水号>\n\t\t\t\t<提案序号>" + strOpNo + "</提案序号>\n\t\t\t\t<案由><![CDATA[" + strSummary + "]]></案由>\n\t\t\t\t<提案分类>" + strSubType + "</提案分类>\n\t\t\t\t<提案者性质>" + strSubManType + "提案</提案者性质>\n\t\t\t\t<第一提案人>" + strSubMan + "</第一提案人>\n\t\t\t\t<联名提案人>" + strSubMans + "</联名提案人>\n\t\t\t\t<提案内容><![CDATA[" + strBody + "]]></提案内容>\n\t\t\t\t<备注>" + strRemark + "</备注>" + strFilesPath + "\n\t\t\t\t<是否同意公开>" + strIsOpen + "</是否同意公开>\n\t\t\t\t<时间标识>" + strTimeMark + "</时间标识>\n\t\t\t\t<提交时间>" + strSubTime + "</提交时间>\n\t\t\t\t<提案者意向主办单位>" + strAdviseHostOrg + "</提案者意向会办单位>\n\t\t\t\t<提案者意向主办单位>" + strAdviseHelpOrg + "</提案者意向会办单位>\n\t\t\t</Row>";
                strIds += "," + data[i].Id.ToString();
                intNum++;
            }
            if (!string.IsNullOrEmpty(strIds))
            {
                strIds = strIds.Trim(',');
            }
            if (!string.IsNullOrEmpty(noIds))
            {
                noIds = noIds.Trim(',');
            }
            ltOut.Text = "<b>导出成功！</b>共" + intNum.ToString() + "篇提案。<br/>导出Id：[" + strIds + "]<br/>未会签Id：[" + noIds + "]";
            if (files.Count() > 0)
            {
                ltOut.Text += "<br/><b>附件文件目录：</b>" + strPath + "，请从服务器上下载。";
            }
            string strOut = "\n\t\t<TableOpinion>" + strOpinion + "\n\t\t</TableOpinion>";
            strOut = "<Data>\n\t<OpinionInfo>" + strOut + "\n\t</OpinionInfo>\n</Data>";
            return strOut;
        }
        //获取提案者性质（old）
        private string getSubManType(string strSubManType, string strSubMan)
        {
            string strOut = "";
            switch (strSubManType)
            {
                case "委员":
                    strOut = "1";
                    break;
                case "党派":
                case "党派团体":
                    strOut = "2";
                    break;
                case "专委会":
                    strOut = "3";
                    break;
                case "界别":
                    strOut = "4";
                    break;
                case "街道活动组":
                case "小组、联组":
                case "其他":
                    strOut = "5";
                    break;
                default:
                    //if (strSubMan.IndexOf("街道") >= 0)
                    //{
                    //    strOut = "5";
                    //}
                    //else if (strSubMan.IndexOf("委员会") >= 0)
                    //{
                    //    strOut = "3";
                    //}
                    //else if (strSubMan.IndexOf("界") >= 0 || strSubMan.IndexOf("共青团") >= 0 || strSubMan.IndexOf("工会") >= 0 || strSubMan.IndexOf("妇联") >= 0 || strSubMan.IndexOf("青联") >= 0 || strSubMan.IndexOf("工商联") >= 0 || strSubMan.IndexOf("台胞台属") >= 0 || strSubMan.IndexOf("侨联") >= 0 || strSubMan.IndexOf("少数民族") >= 0)
                    //{
                    //    strOut = "4";
                    //}
                    //else if (strSubMan.IndexOf("中共") >= 0 || strSubMan.IndexOf("民革") >= 0 || strSubMan.IndexOf("民盟") >= 0 || strSubMan.IndexOf("民建") >= 0 || strSubMan.IndexOf("民进") >= 0 || strSubMan.IndexOf("农工党") >= 0 || strSubMan.IndexOf("致公党") >= 0 || strSubMan.IndexOf("九三学社") >= 0 || strSubMan.IndexOf("台盟") >= 0 || strSubMan.IndexOf("无党派") >= 0 || strSubMan.IndexOf("群众") >= 0)
                    //{
                    //    strOut = "2";
                    //}
                    //else
                    //{
                    //    strOut = "5";
                    //}
                    break;
            }
            return strOut;
        }
        //获取建议承办单位（old）
        private string getOrgVal(string strOrgName)
        {
            string strOut = "";
            if (strOrgName.IndexOf("|") > 0)
            {
                strOrgName = strOrgName.Substring(0, strOrgName.IndexOf("|"));
            }
            WebOp webOp = new WebOp();
            DataOp[] opData = webOp.GetDatas(0, "承办单位", strOrgName, "OpValue2");
            if (opData != null)
            {
                strOut = opData[0].OpValue2;
            }
            return strOut;
        }
        //提案数据（old）
        private string getOpinion(string strPath, DataOpinion[] data)
        {
            int intNum = 0;
            string strIds = "";
            string noIds = "";
            string strOpinion = "";
            string strFeed = "";
            List<DataFiles> files = new List<DataFiles>();
            WebFiles webFiles = new WebFiles();
            for (int i = 0; i < data.Count(); i++)
            {
                string strOutNot = txtOutNot.Text.Trim();
                if (!string.IsNullOrEmpty(strOutNot))
                {
                    strOutNot = "," + strOutNot.Trim(',') + ",";
                }
                if (!string.IsNullOrEmpty(strOutNot) && strOutNot.IndexOf("," + data[i].Id.ToString() + ",") >= 0)
                {//排除Id，不导出。
                    continue;
                }
                string strOpId = (!string.IsNullOrEmpty(data[i].OpNum)) ? data[i].OpNum : "w" + data[i].Id.ToString();
                string strBsort = "";
                string strSsort = "";
                //string strOpValue2 = "";
                //if (!string.IsNullOrEmpty(data[i].AdviseHostOrg))
                //{
                //    strOpValue2 = getOrgVal(data[i].AdviseHostOrg);
                //}
                //else if (!string.IsNullOrEmpty(data[i].AdviseHelpOrg))
                //{
                //    strOpValue2 = getOrgVal(data[i].AdviseHelpOrg);
                //}
                //if (!string.IsNullOrEmpty(strOpValue2) && strOpValue2.IndexOf("|") > 0)
                //{
                //    strBsort = strOpValue2.Substring(0, strOpValue2.IndexOf("|"));
                //    strSsort = strOpValue2.Substring(strOpValue2.IndexOf("|") + 1);
                //}
                string strSummary = data[i].Summary;
                string strDate = (data[i].SubTime > DateTime.MinValue) ? data[i].SubTime.ToString("yyyy/MM/dd HH:mm:ss") : "";
                string strModdate = "";//(data[i].VerifyTime > DateTime.MinValue) ? data[i].VerifyTime.ToString("yyyy/MM/dd HH:mm:ss") : "";
                string strFlow = "2";
                for (int j = 0; j < arrFlow.GetLength(0); j++)
                {//流程
                    if (arrFlow[j, 1] == data[i].ActiveName)
                    {
                        strFlow = arrFlow[j, 0];
                        break;
                    }
                }
                int intPeriod = 0;
                for (int x = 0; x < data[i].Period.Length; x++)
                {//届次
                    string tmp = data[i].Period.Substring(x, 1);
                    for (int j = 0; j < arrNum.GetLength(0); j++)
                    {
                        if (arrNum[j, 1] == tmp)
                        {
                            intPeriod += Convert.ToInt16(arrNum[j, 0]);
                            break;
                        }
                    }
                }
                string strPeriod = (intPeriod > 0) ? intPeriod.ToString() : "";
                string strTimes = "";
                for (int j = 0; j < arrNum.GetLength(0); j++)
                {//届次
                    if (arrNum[j, 1] == data[i].Times)
                    {
                        strTimes = arrNum[j, 0];
                        break;
                    }
                }
                string strTimeMark = data[i].TimeMark;
                string strOpNo = data[i].OpNo;
                string strBody = data[i].Body;
                strBody = HelperMain.DelUbbFont(strBody);//删除字体
                strBody = HelperMain.Ubb2Html(strBody);//ubb转html
                strBody = HelperMain.ReplaceImg(strBody);//去除图片
                string strSubManType = getSubManType(data[i].SubManType, data[i].SubMan);
                string strFilesPath = "";
                string strAffixName = "";
                string strAffixPath = "";
                string[] arrFiles = data[i].Files.Split('|');
                for (int j = 0; j < arrFiles.Count(); j++)
                {
                    if (!string.IsNullOrEmpty(arrFiles[j]))
                    {
                        string strOld = arrFiles[j];
                        string strExt = "";
                        int intExt = strOld.LastIndexOf(".");
                        if (intExt > 0 && intExt > strOld.LastIndexOf("/"))
                        {
                            strExt = strOld.Substring(intExt);
                        }
                        string strTitle = "w" + data[i].Id.ToString();//重新按流水号命名
                        if (j > 0)
                        {
                            strTitle += "_" + j.ToString();//多附件加后缀
                        }
                        strTitle += strExt;//扩展名
                        string strUrl = strPath + strTitle;//新的链接
                        string strFile = Server.MapPath(strUrl);//物理地址
                        if (!System.IO.File.Exists(strFile))
                        {
                            string strTmp = Server.MapPath(arrFiles[j]);
                            if (System.IO.File.Exists(strTmp))
                            {
                                System.IO.File.Copy(strTmp, strFile);//复制新文件
                            }
                        }
                        DataFiles file = new DataFiles();
                        file.Title = strTitle;
                        file.Url = strUrl;
                        //DataFiles[] fileData = webFiles.GetDataTitle(arrFiles[j], "Title,Url");
                        //if (fileData != null)
                        //{
                        //    file.Title = fileData[0].Title;
                        //    file.Url = fileData[0].Url;
                        //}
                        //else
                        //{
                        //    file.Title = arrFiles[j];
                        //    file.Url = arrFiles[j];
                        //}
                        files.Add(file);
                        strFilesPath += "\n\t\t\t\t<op_attachmentPath><![CDATA[" + strUrl + "]]></op_attachmentPath>";
                        //string strName = (arrFiles[j].IndexOf("/") >= 0) ? arrFiles[j].Substring(arrFiles[j].LastIndexOf("/") + 1) : arrFiles[j];
                        strAffixName += "\n\t\t\t\t<op_AffixName>" + strTitle + "</op_AffixName>";
                        strAffixPath += "\n\t\t\t\t<op_AffixPath><![CDATA[" + strFile + "]]></op_AffixPath>";
                    }
                }
                string strIsSign = data[i].IsSign;
                string strIsFeed = data[i].IsFeed;
                string strMajor = (data[i].IsPoint == "是") ? "1" : "0";

                strOpinion += "\n\t\t\t<Row>\n\t\t\t\t<op_id>" + strOpId + "</op_id>\n\t\t\t\t<op_title></op_title>\n\t\t\t\t<op_bsort>" + strBsort + "</op_bsort>\n\t\t\t\t<op_ssort>" + strSsort + "</op_ssort>\n\t\t\t\t<op_summary><![CDATA[" + strSummary + "]]></op_summary>\n\t\t\t\t<op_remark></op_remark>\n\t\t\t\t<op_date>" + strDate + "</op_date>\n\t\t\t\t<op_moddate>" + strModdate + "</op_moddate>\n\t\t\t\t<op_flow>" + strFlow + "</op_flow>\n\t\t\t\t<op_j>" + strPeriod + "</op_j>\n\t\t\t\t<op_duban_attitude></op_duban_attitude>\n\t\t\t\t<op_c>" + strTimes + "</op_c>\n\t\t\t\t<op_duban_idea></op_duban_idea>\n\t\t\t\t<op_end_date></op_end_date>\n\t\t\t\t<op_cy>" + strTimeMark + "</op_cy>\n\t\t\t\t<op_num>" + strOpNo + "</op_num>\n\t\t\t\t<op_input></op_input>\n\t\t\t\t<op_Text><![CDATA[" + strBody + "]]></op_Text>\n\t\t\t\t<op_comPry>" + strSubManType + "</op_comPry>" + strFilesPath + "\n\t\t\t\t<op_ifSign>" + strIsSign + "</op_ifSign>\n\t\t\t\t<op_ifNeedFeedBack>" +strIsFeed  + "</op_ifNeedFeedBack>\n\t\t\t\t<op_ifMajor>" + strMajor + "</op_ifMajor>\n\t\t\t\t<op_ManageDept></op_ManageDept>" + strAffixName + strAffixPath + "\n\t\t\t</Row>";

                string strSubMan = "";
                if (!string.IsNullOrEmpty(data[i].SubMan))
                {
                    strSubMan += "," + data[i].SubMan.Trim(',');
                }
                string strSubMans = data[i].SubMans.Trim(',');
                if (!string.IsNullOrEmpty(data[i].SubMans))
                {
                    strSubMans = PublicMod.GetSubMans(data[i].Id, strSubMans, 0);//联名会签检查//*待会签，~会签过期
                    if (!string.IsNullOrEmpty(strSubMans) && strSubMans.IndexOf("*") >= 0)
                    {//未完成会签，不导出。
                        noIds += "," + data[i].Id.ToString();
                        continue;
                    }
                    else if (!string.IsNullOrEmpty(strSubMans) && strSubMans.IndexOf("~") >= 0)
                    {//过滤会签过期委员
                        string strTmp = "";
                        string[] arr = strSubMans.Split(',');
                        for (int j = 0; j < arr.Count(); j++)
                        {
                            if (!string.IsNullOrEmpty(arr[j]) && arr[j].IndexOf("~") < 0)
                            {
                                strTmp += "," + arr[j];
                            }
                        }
                        strSubMans = strTmp.Trim(',');
                    }
                    if (!string.IsNullOrEmpty(strSubMans))
                    {
                        strSubMan += "," + strSubMans;
                    }
                }
                strSubMan = strSubMan.Trim(',');

                if (!string.IsNullOrEmpty(strSubMan))
                {
                    strFeed += getOpinionFeed(strSubMan, strSubManType, data[i]);//委员反馈
                }

                strIds += "," + data[i].Id.ToString();
                intNum++;
            }
            if (!string.IsNullOrEmpty(strIds))
            {
                strIds = strIds.Trim(',');
            }
            if (!string.IsNullOrEmpty(noIds))
            {
                noIds = noIds.Trim(',');
            }
            ltOut.Text = "<b>导出成功！</b>共" + intNum.ToString() + "篇提案。<br/>导出Id：[" + strIds + "]<br/>未会签Id：[" + noIds + "]";
            if (files.Count() > 0)
            {
                //plFiles.Visible = true;
                //for (int i = 0; i < files.Count(); i++)
                //{
                //    files[i].num = i + 1;
                //}
                //rpFiles.DataSource = files;
                //rpFiles.DataBind();
                ltOut.Text += "<br/><b>附件文件目录：</b>" + strPath + "，请从服务器上下载。";
            }
            string strOut = "\n\t\t<TableOpinion>" + strOpinion + "\n\t\t</TableOpinion>\n\t\t<TableCommissary>" + strFeed + "\n\t\t</TableCommissary>";
            strOut = "<Data>\n\t<PopInfo/>\n\t<OpinionInfo>" + strOut + "\n\t</OpinionInfo>\n</Data>";
            return strOut;
        }
        //委员反馈（old）
        private string getOpinionFeed(string strSubMan, string strSubManType, DataOpinion opinionData)
        {
            string strOut = "";
            WebUser webUser = new WebUser();
            WebOpinionFeed webFeed = new WebOpinionFeed();
            WebOpinionSign webSign = new WebOpinionSign();

            string[] arr = strSubMan.Split(',');
            for (int i = 0; i < arr.Count(); i++)
            {
                string strUser = arr[i];
                if (!string.IsNullOrEmpty(strUser))
                {
                    string strType = "";
                    string strComId = "0";
                    int intUserId = 0;
                    string strUserCode = "";
                    string strTrueName = "";
                    string strPwd = "";
                    DataUser[] userData = webUser.GetDatas(config.PERIOD, strUser, "Id,UserCode,TrueName,OldId");
                    if (userData != null)
                    {
                        intUserId = userData[0].Id;
                        if (opinionData.SubManType == "委员")
                        {
                            strType = "0";
                            strComId = userData[0].OldId.ToString();//intUserId.ToString();
                            strUserCode = userData[0].UserCode;
                            strTrueName = userData[0].TrueName;
                        }
                        else
                        {
                            strType = "1";
                            strComId = (userData[0].OldId > 0) ? userData[0].OldId.ToString() : userData[0].UserCode;
                        }
                    }
                    string strOpId = "w" + opinionData.Id.ToString();
                    string strStreet = "";
                    string strMem = "";
                    string strMainEnd = "";
                    string strMain = (opinionData.SubMan.IndexOf("," + strUser + ",") >= 0) ? "是" : "";
                    string strXZ = strSubManType;
                    string strEstimate = "";
                    string strHistory = "";
                    string strSignOk = "是";

                    DataOpinionFeed[] fData = webFeed.GetDatas(1, "提案", opinionData.Id, intUserId);
                    if (fData != null)
                    {
                        for (int j = 0; j < fData.Count(); j++)
                        {
                            string strAutoNum = fData[j].Id.ToString();
                            string strPertinence = fData[j].Pertinence;
                            string strAttitude = fData[j].Attitude;
                            string strResult = fData[j].Result;
                            string strTakeWay = fData[j].TakeWay;
                            strOut += "\n\t\t\t<Row>\n\t\t\t\t<autonum>" + strAutoNum + "</autonum>\n\t\t\t\t<com_id>" + strComId + "</com_id>\n\t\t\t\t<op_id>" + strOpId + "</op_id>\n\t\t\t\t<com_street>" + strStreet + "</com_street>\n\t\t\t\t<if_main>" + strMain + "</if_main>\n\t\t\t\t<com_idea>" + strPertinence + "</com_idea>\n\t\t\t\t<com_attitude>" + strAttitude + "</com_attitude>\n\t\t\t\t<com_mem>" + strMem + "</com_mem>\n\t\t\t\t<com_main_end>" + strMainEnd + "</com_main_end>\n\t\t\t\t<com_result>" + strResult + "</com_result>\n\t\t\t\t<com_xz>" + strXZ + "</com_xz>\n\t\t\t\t<com_mode>" + strTakeWay + "</com_mode>\n\t\t\t\t<com_estimate>" + strEstimate + "</com_estimate>\n\t\t\t\t<com_type>" + strType + "</com_type>\n\t\t\t\t<com_csnNum>" + strUserCode + "</com_csnNum>\n\t\t\t\t<IsHistoryFeedBack>" + strHistory + "</IsHistoryFeedBack>\n\t\t\t\t<if_SignOk>" + strSignOk + "</if_SignOk>\n\t\t\t\t<com_login_name>" + strTrueName + "</com_login_name>\n\t\t\t\t<com_login_pass>" + strPwd + "</com_login_pass>\n\t\t\t</Row>";
                        }
                    }
                    else
                    {
                        string strAutoNum = "";
                        if (strMain == "是")
                        {
                            strAutoNum = strOpId;
                        }
                        else
                        {
                            DataOpinionSign qSign = new DataOpinionSign();
                            qSign.OpType = "提案";
                            qSign.OpId = opinionData.Id;
                            qSign.SignUser = strUser;
                            qSign.ActiveName = ">0";
                            DataOpinionSign[] sData = webSign.GetDatas(qSign, "Id");
                            if (sData != null)
                            {
                                strAutoNum = sData[0].Id.ToString();
                            }
                        }
                        if (!string.IsNullOrEmpty(strAutoNum))
                        {
                            string strPertinence = "";
                            string strAttitude = "";
                            string strResult = "";
                            string strTakeWay = "";
                            strOut += "\n\t\t\t<Row>\n\t\t\t\t<autonum>" + strAutoNum + "</autonum>\n\t\t\t\t<com_id>" + strComId + "</com_id>\n\t\t\t\t<op_id>" + strOpId + "</op_id>\n\t\t\t\t<com_street>" + strStreet + "</com_street>\n\t\t\t\t<if_main>" + strMain + "</if_main>\n\t\t\t\t<com_idea>" + strPertinence + "</com_idea>\n\t\t\t\t<com_attitude>" + strAttitude + "</com_attitude>\n\t\t\t\t<com_mem>" + strMem + "</com_mem>\n\t\t\t\t<com_main_end>" + strMainEnd + "</com_main_end>\n\t\t\t\t<com_result>" + strResult + "</com_result>\n\t\t\t\t<com_xz>" + strXZ + "</com_xz>\n\t\t\t\t<com_mode>" + strTakeWay + "</com_mode>\n\t\t\t\t<com_estimate>" + strEstimate + "</com_estimate>\n\t\t\t\t<com_type>" + strType + "</com_type>\n\t\t\t\t<com_csnNum>" + strUserCode + "</com_csnNum>\n\t\t\t\t<IsHistoryFeedBack>" + strHistory + "</IsHistoryFeedBack>\n\t\t\t\t<if_SignOk>" + strSignOk + "</if_SignOk>\n\t\t\t\t<com_login_name>" + strTrueName + "</com_login_name>\n\t\t\t\t<com_login_pass>" + strPwd + "</com_login_pass>\n\t\t\t</Row>";
                        }
                    }
                }
            }
            return strOut;
        }
        #endregion
        //
        #region 导入
        //导入数据
        protected void btnIn_Click(object sender, EventArgs e)
        {
            if (myUser == null || string.IsNullOrEmpty(txtInFile.Text))
            {
                return;
            }
            //string strText = HelperFile.ReadFile(txtInFile.Text);
            //if (string.IsNullOrEmpty(strText))
            //{
            //    return;
            //}
            string fileName = Server.MapPath(txtInFile.Text);
            XmlDocument xml = new XmlDocument();
            xml.Load(fileName);
            //XmlNodeList PopInfo = xml.DocumentElement.SelectNodes("Data/PopInfo");
            lblInfo.Text = "";
            DateTime dtNow = DateTime.Now;
            string strIp = HelperMain.GetIpPort();
            string strUser = (myUser != null) ? myUser.AdminName : hfUser.Value;
            WebOpinion webOpinion = new WebOpinion();
            List<int> list = inOpinion(xml.DocumentElement, dtNow, strIp, strUser, webOpinion);//提案数据
            inOpinionDept(xml.DocumentElement, dtNow, strIp, strUser, webOpinion);//单位处理
            inOpinionFeed(xml.DocumentElement, dtNow, strIp, strUser, webOpinion);//委员反馈

            if (list.Count() > 0)
            {
                upOpinion(list, strIp, strUser, webOpinion);
            }
        }
        //提案数据
        private List<int> inOpinion(XmlNode root, DateTime dtNow, string strIp, string strUser, WebOpinion webOpinion)
        {
            string strOut = "<hr/><b>导入提案：</b><br/>";
            List<int> list = new List<int>();

            XmlNodeList nodes = root.SelectNodes("OpinionInfo/TableOpinion/Row");//提案信息列表
            for (int i = 0; i < nodes.Count; i++)
            {
                DataOpinion data = null;
                DataOpinion[] qData = null;
                string strOpNo = nodes[i].SelectSingleNode("op_num").InnerText;
                string strOpNum = nodes[i].SelectSingleNode("op_id").InnerText;//内部流水号（对接数据使用）
                //int intId = 0;
                if (!string.IsNullOrEmpty(strOpNo))
                {
                    qData = webOpinion.GetDatas(strOpNo);//按提案号
                }
                if (!string.IsNullOrEmpty(strOpNum) && qData == null)
                {
                    qData = webOpinion.GetDatas("", strOpNum);//按内部流水号
                }
                if (qData != null)
                {
                    data = qData[0];//获取提案
                }
                else
                {
                    data = new DataOpinion();//新建提案
                }

                //data.IsOpen = "是";//默认公开
                data.OpNo = strOpNo;
                data.OpNum = strOpNum;
                //nodes[i].SelectSingleNode("op_title").InnerText;
                string strBsort = nodes[i].SelectSingleNode("op_bsort").InnerText;
                if (!string.IsNullOrEmpty(strBsort))
                {//提案分类
                    for (int j = 0; j < arrBsort.GetLength(0); j++)
                    {
                        if (arrBsort[j, 0] == strBsort)
                        {
                            strBsort = arrBsort[j, 1];
                            break;
                        }
                    }
                }
                data.SubType = strBsort;
                //nodes[i].SelectSingleNode("op_ssort").InnerText;
                data.Summary = nodes[i].SelectSingleNode("op_summary").InnerText;
                string strRemark = nodes[i].SelectSingleNode("op_remark").InnerText;
                if (!string.IsNullOrEmpty(data.Remark) && !string.IsNullOrEmpty(strRemark))
                {
                    data.Remark = data.Remark.Replace(strRemark, "").Trim();
                }
                if (!string.IsNullOrEmpty(data.Remark))
                {
                    data.Remark += "\n";
                }
                data.Remark += strRemark;
                string strSubTime = nodes[i].SelectSingleNode("op_date").InnerText;
                if (data.SubTime <= DateTime.MinValue)
                {
                    data.SubTime = (!string.IsNullOrEmpty(strSubTime)) ? Convert.ToDateTime(strSubTime) : DateTime.Now;
                }
                string strVerifyTime = nodes[i].SelectSingleNode("op_moddate").InnerText;
                if (!string.IsNullOrEmpty(strVerifyTime))
                {
                    data.VerifyTime = Convert.ToDateTime(strVerifyTime);
                }
                string strFlow = nodes[i].SelectSingleNode("op_flow").InnerText;
                if (!string.IsNullOrEmpty(strFlow))
                {//流程
                    for (int j = 0; j < arrFlow.GetLength(0); j++)
                    {
                        if (arrFlow[j, 0] == strFlow)
                        {
                            strFlow = arrFlow[j, 1];
                            break;
                        }
                    }
                }
                if (data.ActiveName != "归并")
                {
                    switch (strFlow)
                    {
                        case "外网提交":
                        case "提交":
                            data.ActiveName = "待立案";
                            data.ApplyState = "";
                            break;
                        case "预审通过":
                            data.ActiveName = "待立案";
                            data.ApplyState = "部门处理";
                            break;
                        case "预审退回":
                        case "分理退回":
                            data.ActiveName = "退回";
                            data.ApplyState = "";
                            break;
                        case "已分理":
                            data.ActiveName = "立案";
                            data.ApplyState = "提案分理";
                            break;
                        case "待反馈":
                        case "已反馈":
                        case "归档":
                        case "转用":
                        case "已转用":
                        case "不转用":
                            data.ActiveName = "立案";
                            data.ApplyState = "已办复";
                            break;
                        case "立案":
                        case "重新办理":
                            data.ActiveName = "立案";
                            data.ApplyState = "提案立案";
                            break;
                        default:
                            data.ActiveName = strFlow;//暂存，不立案
                            data.ApplyState = "";
                            break;
                    }
                }
                string strPeriod = nodes[i].SelectSingleNode("op_j").InnerText;
                if (!string.IsNullOrEmpty(strPeriod))
                {//届次
                    int intPeriod = Convert.ToInt16(strPeriod);
                    strPeriod = "";
                    if (intPeriod > 10)
                    {
                        strPeriod += arrNum[0, 1]; //十";
                        intPeriod -= 10;
                    }
                    for (int j = 0; j < arrNum.GetLength(0); j++)
                    {
                        if (arrNum[j, 0] == intPeriod.ToString())
                        {
                            strPeriod += arrNum[j, 1];
                            break;
                        }
                    }
                }
                data.Period = strPeriod;
                //nodes[i].SelectSingleNode("op_duban_attitude").InnerText;
                string strTimes = nodes[i].SelectSingleNode("op_c").InnerText;
                if (!string.IsNullOrEmpty(strTimes))
                {//届次
                    for (int j = 0; j < arrNum.GetLength(0); j++)
                    {
                        if (arrNum[j, 0] == strTimes)
                        {
                            strTimes = arrNum[j, 1];
                            break;
                        }
                    }
                }
                data.Times = strTimes;
                //nodes[i].SelectSingleNode("op_duban_idea").InnerText;
                //nodes[i].SelectSingleNode("op_end_date").InnerText;
                data.TimeMark = nodes[i].SelectSingleNode("op_cy").InnerText;
                //nodes[i].SelectSingleNode("op_input").InnerText;
                string strBody = nodes[i].SelectSingleNode("op_Text").InnerText;
                while (strBody.IndexOf("<!--[if gte mso 9]>") >= 0 && strBody.IndexOf("<![endif]-->") > 0)
                {
                    string strTmp = strBody.Substring(strBody.IndexOf("<!--[if gte mso 9]>"), strBody.IndexOf("<![endif]-->") + "<![endif]-->".Length - strBody.IndexOf("<!--[if gte mso 9]>"));
                    strBody = strBody.Replace(strTmp, "");
                }
                data.Body = HelperMain.Html2Ubb(strBody);
                bool blSubManType = false;
                string strSubManType = nodes[i].SelectSingleNode("op_comPry").InnerText;
                if (!string.IsNullOrEmpty(strSubManType))
                {
                    for (int j = 0; j < arrSubManType.GetLength(0); j++)
                    {
                        if (arrSubManType[j, 0] == strSubManType)
                        {
                            strSubManType = arrSubManType[j, 1];
                            blSubManType = true;
                            break;
                        }
                    }
                }
                if (!blSubManType)
                {
                    strSubManType = "其他";
                }
                data.SubManType = strSubManType;
                //nodes[i].SelectSingleNode("op_attachmentPath").InnerText;
                data.IsSign = nodes[i].SelectSingleNode("op_ifSign").InnerText;
                string strIsFeed = nodes[i].SelectSingleNode("op_ifNeedFeedBack").InnerText;
                if (!string.IsNullOrEmpty(strIsFeed) && HelperMain.IsNumeric(strIsFeed))
                {
                    data.IsFeed = (Convert.ToInt16(strIsFeed) > 0) ? "是" : "否";
                }
                //else
                //{//默认为需要征询意见
                //    data.IsFeed = "是";
                //}
                string strIsPoint = nodes[i].SelectSingleNode("op_ifMajor").InnerText.Trim();
                data.IsPoint = (!string.IsNullOrEmpty(strIsPoint) && Convert.ToInt16(strIsPoint) > 0) ? "是" : "否";
                //nodes[i].SelectSingleNode("op_ManageDept").InnerText;
                string strFiles = "";
                XmlNodeList files = nodes[i].SelectNodes("op_AffixName");//附件名称
                for (int j = 0; j < files.Count; j++)
                {
                    if (!string.IsNullOrEmpty(files[j].InnerText))
                    {
                        if (!string.IsNullOrEmpty(strFiles))
                        {
                            strFiles += "|";
                        }
                        string strFile = "../upload/opinion/" + dtNow.ToString("yyyy") + "/" + files[j].InnerText;//按年份导入目录
                        if (!string.IsNullOrEmpty(data.Files))
                        {
                            string[] arrFiles = data.Files.Split('|');
                            for (int x = 0; x < arrFiles.Count(); x++)
                            {
                                if (arrFiles[x].IndexOf(files[j].InnerText) >= 0)
                                {
                                    strFile = arrFiles[x];
                                    break;
                                }
                            }
                        }
                        strFiles += strFile;
                    }
                }
                data.Files = strFiles;
                //nodes[i].SelectSingleNode("op_AffixPath").InnerText;

                //下面提案人数据先清空，后面再更新
                data.UserId = 0;
                data.AddUser = "";
                data.SubManType = "";
                if (data.ActiveName != "归并")
                {
                    data.SubMan = "";
                    data.SubMans = "";
                }
                data.Party = "";
                data.Committee = "";
                data.Subsector = "";
                data.StreetTeam = "";
                //下面办理数据先清空，后面再更新
                data.ExamHostOrg = "";
                data.ExamHelpOrg = "";
                data.ResultInfo = "";
                data.ResultBody = "";
                data.ResultInfo2 = "";
                data.ResultBody2 = "";
                data.ResultTime = DateTime.MinValue;

                if (data.Id <= 0)
                {
                    data.AddTime = dtNow;
                    data.AddIp = strIp;
                    data.AddUser = strUser;

                    data.Id = webOpinion.Insert(data);//新增
                    strOut += "新增";
                }
                else
                {
                    data.UpTime = dtNow;
                    data.UpIp = strIp;
                    data.UpUser = strUser;

                    if (webOpinion.Update(data) <= 0)//更新
                    {
                        data.Id = -1;
                    }
                    strOut += "更新";
                }

                if (data.Id > 0)
                {
                    strOut += " 成功";
                    list.Add(data.Id);
                }
                else
                {
                    strOut += " 失败";
                }

                strOut += string.Format("：(流水号){0}，(提案号){1}，(案由){2}<br/>", data.OpNum, data.OpNo, data.Summary);
            }
            lblInfo.Text += strOut;

            return list;
        }
        //更新提案 党派、专委会、界别、街道活动组
        private void upOpinion(List<int> list, string strIp, string strUser, WebOpinion webOpinion)
        {
            decimal ScoreProperty = 0;
            decimal ScoreProperty2 = 0;
            string TitleProperty = "立案提案";
            WebScore webScore = new WebScore();
            DataScore[] sData = webScore.GetDatas(1, "提交提案", "", TitleProperty + "%", "Score,Score2");
            if (sData != null)
            {
                ScoreProperty = sData[0].Score;
                ScoreProperty2 = sData[0].Score2;
            }
            string TableName = "tb_Opinion";

            WebUser webUser = new WebUser();
            for (int i = 0; i < list.Count(); i++)
            {
                DataOpinion[] data = webOpinion.GetData(list[i]);
                if (data != null)
                {
                    string strUserId = "";
                    string strUserIds = "";
                    string strParty = "";
                    string strCommittee = "";
                    string strSubsector = "";
                    string strStreetTeam = "";
                    if (!string.IsNullOrEmpty(data[0].SubMan))
                    {
                        string[] arr = data[0].SubMan.Split(',');
                        for (int j = 0; j < arr.Count(); j++)
                        {
                            if (!string.IsNullOrEmpty(arr[j]))
                            {
                                DataUser[] uData2 = webUser.GetDatas(config.PERIOD, arr[j], "Id,Party,Committee,Committee2,Subsector,StreetTeam");
                                if (uData2 != null)
                                {
                                    strUserId += "," + uData2[0].Id.ToString();
                                    if (!string.IsNullOrEmpty(uData2[0].Party) && (strParty + ",").IndexOf(uData2[0].Party) < 0)
                                    {
                                        strParty += "," + uData2[0].Party;
                                    }
                                    if (!string.IsNullOrEmpty(uData2[0].Committee) && (strCommittee + ",").IndexOf(uData2[0].Committee) < 0)
                                    {
                                        strCommittee += "," + uData2[0].Committee;
                                    }
                                    if (!string.IsNullOrEmpty(uData2[0].Committee2) && (strCommittee + ",").IndexOf(uData2[0].Committee2) < 0)
                                    {
                                        strCommittee += "," + uData2[0].Committee2;
                                    }
                                    if (!string.IsNullOrEmpty(uData2[0].Subsector) && (strSubsector + ",").IndexOf(uData2[0].Subsector) < 0)
                                    {
                                        strSubsector += "," + uData2[0].Subsector;
                                    }
                                    if (!string.IsNullOrEmpty(uData2[0].StreetTeam) && (strStreetTeam + ",").IndexOf(uData2[0].StreetTeam) < 0)
                                    {
                                        strStreetTeam += "," + uData2[0].StreetTeam;
                                    }
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(data[0].SubMans))
                    {
                        string[] arr = data[0].SubMans.Split(',');
                        for (int j = 0; j < arr.Count(); j++)
                        {
                            if (!string.IsNullOrEmpty(arr[j]))
                            {
                                DataUser[] uData2 = webUser.GetDatas(config.PERIOD, arr[j], "Id,Party,Committee,Committee2,Subsector,StreetTeam");
                                if (uData2 != null)
                                {
                                    strUserIds += "," + uData2[0].Id.ToString();
                                    if (!string.IsNullOrEmpty(uData2[0].Party) && (strParty + ",").IndexOf(uData2[0].Party) < 0)
                                    {
                                        strParty += "," + uData2[0].Party;
                                    }
                                    if (!string.IsNullOrEmpty(uData2[0].Committee) && (strCommittee + ",").IndexOf(uData2[0].Committee) < 0)
                                    {
                                        strCommittee += "," + uData2[0].Committee;
                                    }
                                    if (!string.IsNullOrEmpty(uData2[0].Committee2) && (strCommittee + ",").IndexOf(uData2[0].Committee2) < 0)
                                    {
                                        strCommittee += "," + uData2[0].Committee2;
                                    }
                                    if (!string.IsNullOrEmpty(uData2[0].Subsector) && (strSubsector + ",").IndexOf(uData2[0].Subsector) < 0)
                                    {
                                        strSubsector += "," + uData2[0].Subsector;
                                    }
                                    if (!string.IsNullOrEmpty(uData2[0].StreetTeam) && (strStreetTeam + ",").IndexOf(uData2[0].StreetTeam) < 0)
                                    {
                                        strStreetTeam += "," + uData2[0].StreetTeam;
                                    }
                                }
                            }
                        }
                    }
                    data[0].Party = strParty.Trim(',');
                    data[0].Committee = strCommittee.Trim(',');
                    data[0].Subsector = strSubsector.Trim(',');
                    data[0].StreetTeam = strStreetTeam.Trim(',');
                    webOpinion.Update(data[0]);

                    if (data[0].ActiveName == "立案" || data[0].ActiveName == "归并")
                    {
                        DateTime dtTime = (data[0].SubTime > DateTime.MinValue) ? data[0].SubTime : DateTime.Now;
                        if (!string.IsNullOrEmpty(strUserId) && ScoreProperty2 > 0)
                        {
                            string[] arr2 = strUserId.Trim(',').Split(',');
                            for (int j = 0; j < arr2.Count(); j++)
                            {
                                int UserId = Convert.ToInt32(arr2[j]);
                                PublicMod.AddScore(UserId, TitleProperty + "（第一提案人）", ScoreProperty2, TableName, data[0].Id, strIp, strUser, dtTime);//立案，添加积分
                            }
                        }
                        if (!string.IsNullOrEmpty(strUserIds) && ScoreProperty > 0)
                        {
                            string[] arr2 = strUserIds.Trim(',').Split(',');
                            for (int j = 0; j < arr2.Count(); j++)
                            {
                                int UserId = Convert.ToInt32(arr2[j]);
                                PublicMod.AddScore(UserId, TitleProperty + "（联名）", ScoreProperty, TableName, data[0].Id, strIp, strUser, dtTime);//立案，添加积分
                            }
                        }
                    }
                    else
                    {
                        WebUserScore webScore2 = new WebUserScore();
                        DataUserScore[] ckData = webScore2.GetDatas(0, "", TableName, data[0].Id, "", "", "Id");
                        if (ckData != null)
                        {
                            ArrayList arrList = new ArrayList();
                            for (int j = 0; j < ckData.Count(); j++)
                            {
                                arrList.Add(ckData[j].Id);
                            }
                            if (arrList.Count > 0)
                            {
                                webScore2.UpdateActive(arrList, -1);//取消积分
                            }
                        }
                    }

                }
            }
        }
        //委员反馈
        private void inOpinionFeed(XmlNode root, DateTime dtNow, string strIp, string strUser, WebOpinion webOpinion)
        {
            string strOut = "<hr/><b>导入委员反馈：</b><br/>";
            WebOpinionFeed webFeed = new WebOpinionFeed();
            WebUser webUser = new WebUser();
            WebOpinionSign webSign = new WebOpinionSign();

            XmlNodeList nodes = root.SelectNodes("OpinionInfo/TableCommissary/Row");//委员会签反馈列表
            for (int i = 0; i < nodes.Count; i++)
            {
                int intOpId = 0;
                string strActiveName = "";

                string strOpNum = nodes[i].SelectSingleNode("op_id").InnerText;
                int intId = (HelperMain.IsNumeric(strOpNum)) ? Convert.ToInt32(strOpNum) : 0;

                DataOpinion[] opinionData = webOpinion.GetDatas("", strOpNum, intId);
                if (opinionData != null)
                {
                    intOpId = opinionData[0].Id;
                    strActiveName = opinionData[0].ActiveName;
                }
                else
                {
                    continue;
                }
                DataOpinionFeed data = null;
                int intUserId = 0;
                string strTrueName = "";
                string strSubManType = "";
                string strUserCode = "";
                string strComType = nodes[i].SelectSingleNode("com_type").InnerText;
                if (strComType == "0")
                {//委员帐号
                    strUserCode = nodes[i].SelectSingleNode("com_csnNum").InnerText;//委员编号
                }
                else
                {//团体帐号
                    strUserCode = nodes[i].SelectSingleNode("com_id").InnerText;
                }
                if (!string.IsNullOrEmpty(strUserCode))
                {
                    DataUser[] userData = webUser.GetData(strUserCode, "Id,TrueName,UserType");
                    if (userData != null)
                    {
                        intUserId = userData[0].Id;
                        strTrueName = userData[0].TrueName;
                        strSubManType = userData[0].UserType;
                    }
                    else
                    {//当委员编号未知时
                        strTrueName = strUserCode;
                        strSubManType = "其他";
                    }
                }

                DataOpinionFeed[] qFeed = webFeed.GetDatas(0, "提案", intOpId, intUserId);
                if (qFeed != null)
                {
                    data = qFeed[0];
                }
                else
                {
                    data = new DataOpinionFeed();
                }
                data.OpId = intOpId;
                //nodes[i].SelectSingleNode("autonum").InnerText;
                //nodes[i].SelectSingleNode("com_id").InnerText;
                //nodes[i].SelectSingleNode("op_id").InnerText;
                //nodes[i].SelectSingleNode("com_street").InnerText;
                string strMain = nodes[i].SelectSingleNode("if_main").InnerText;//第一提案人
                if (!string.IsNullOrEmpty(strMain))
                {
                    strMain = strMain.Trim();
                }
                data.Pertinence = nodes[i].SelectSingleNode("com_idea").InnerText;
                data.Attitude = nodes[i].SelectSingleNode("com_attitude").InnerText;
                if (!string.IsNullOrEmpty(data.Remark))
                {
                    data.Remark += "\n" + nodes[i].SelectSingleNode("com_mem").InnerText;
                }
                else
                {
                    data.Remark = nodes[i].SelectSingleNode("com_mem").InnerText;
                }
                //nodes[i].SelectSingleNode("com_main_end").InnerText;
                data.Result = nodes[i].SelectSingleNode("com_result").InnerText;
                //nodes[i].SelectSingleNode("com_xz").InnerText;
                data.TakeWay = nodes[i].SelectSingleNode("com_mode").InnerText;
                //nodes[i].SelectSingleNode("com_estimate").InnerText;
                data.UserId = intUserId;
                bool upOpinion = false;
                if (!string.IsNullOrEmpty(strTrueName))
                {
                    string strSubMan = "," + strTrueName + ",";
                    if (!string.IsNullOrEmpty(strMain))
                    {//第一提案人
                        if (!string.IsNullOrEmpty(strMain) && intUserId > 0 && !string.IsNullOrEmpty(strTrueName))
                        {
                            opinionData[0].UserId = intUserId;
                            opinionData[0].AddUser = strTrueName;
                            opinionData[0].SubManType = strSubManType;
                        }
                        if (opinionData[0].SubMan.IndexOf(strSubMan) < 0)
                        {
                            if (!string.IsNullOrEmpty(opinionData[0].SubMan))
                            {
                                opinionData[0].SubMan += strTrueName + ",";
                            }
                            else
                            {
                                opinionData[0].SubMan = strSubMan;
                            }
                        }
                        upOpinion = true;
                    }
                    else if (strActiveName == "归并" && opinionData[0].SubMan.IndexOf(strSubMan) >= 0)
                    {//归并后，第一提案人
                        //upOpinion = true;
                    }
                    else if (opinionData[0].SubMans.IndexOf(strSubMan) < 0)
                    {//联名提案人
                        if (!string.IsNullOrEmpty(opinionData[0].SubMans))
                        {
                            opinionData[0].SubMans += strTrueName + ",";
                        }
                        else
                        {
                            opinionData[0].SubMans = strSubMan;
                        }
                        upOpinion = true;
                        //检查会签
                        DataOpinionSign qSign = new DataOpinionSign();
                        qSign.OpType = "提案";
                        qSign.OpId = intOpId;
                        qSign.SignUser = strTrueName;
                        DataOpinionSign[] ckData = webSign.GetDatas(qSign, "Overdue,Active");
                        if (ckData != null)
                        {
                            if (ckData[0].Active <= 0)
                            {
                                ckData[0].SignTime = dtNow;
                                ckData[0].Remark = "内网导入更新";
                                ckData[0].UpTime = dtNow;
                                ckData[0].UpIp = strIp;
                                ckData[0].UpUser = strUser;
                                //webSign.UpdateActive(ckData[0].Id, 1);
                                webSign.Update(ckData[0]);
                            }
                        }
                        else
                        {
                            DataOpinionSign sData = new DataOpinionSign();
                            sData.OpType = "提案";
                            sData.OpId = intOpId;
                            sData.UserId = intUserId;
                            sData.SignUser = strTrueName;
                            //sData.Overdue = DateTime.Now;
                            sData.SignTime = dtNow;
                            //sData.SignIp
                            //sData.SignMark = TimeMark;
                            //data.Body
                            sData.Remark = "内网导入新增";
                            sData.Active = 1;
                            sData.AddTime = dtNow;
                            sData.AddIp = strIp;
                            sData.AddUser = strUser;
                            webSign.Insert(sData);
                        }
                    }
                }
                strOut += string.Format("(提案流水号){0}：", data.OpId);
                if (upOpinion)
                {
                    if (webOpinion.Update(opinionData[0]) > 0)
                    {
                        strOut += string.Format("(更新提案人){0}({1})，", strTrueName, strSubManType);
                    }
                }
                //nodes[i].SelectSingleNode("IsHistoryFeedBack").InnerText;
                //nodes[i].SelectSingleNode("if_SignOk").InnerText;
                //nodes[i].SelectSingleNode("com_login_name").InnerText;
                //nodes[i].SelectSingleNode("com_login_pass").InnerText;

                data.UpTime = dtNow;
                data.UpIp = strIp;
                data.UpUser = strUser;

                if (!string.IsNullOrEmpty(data.Pertinence) || !string.IsNullOrEmpty(data.Attitude) || !string.IsNullOrEmpty(data.Result) || !string.IsNullOrEmpty(data.TakeWay))
                {
                    if (data.Id <= 0)
                    {
                        data.OpType = "提案";
                        data.Active = 1;
                        data.AddTime = dtNow;
                        data.AddIp = strIp;
                        data.AddUser = strTrueName;
                        //data.AddUser = strUser;

                        data.Id = webFeed.Insert(data);//新增
                        strOut += "新增";
                    }
                    else
                    {
                        if (webFeed.Update(data) <= 0)//更新
                        {
                            data.Id = -1;
                        }
                        strOut += "更新";
                    }

                    strOut += (data.Id > 0) ? " 成功" : " 失败";
                    strOut += string.Format("：(流水号){0}，(反馈人){1} {2}<br/>", data.Id, strTrueName, strUserCode);
                }
                else
                {
                    strOut += "<br/>";
                }
            }

            lblInfo.Text += strOut;
        }
        //单位处理
        private void inOpinionDept(XmlNode root, DateTime dtNow, string strIp, string strUser, WebOpinion webOpinion)
        {
            string strOut = "<hr/><b>导入单位处理：</b><br/>";
            WebOp webOp = new WebOp();

            XmlNodeList nodes = root.SelectNodes("OpinionInfo/TableDepartment/Row");//单位处理列表
            for (int i = 0; i < nodes.Count; i++)
            {
                DataOpinion data = null;
                string strOpNum = nodes[i].SelectSingleNode("op_id").InnerText;
                int intId = (HelperMain.IsNumeric(strOpNum)) ? Convert.ToInt32(strOpNum) : 0;

                DataOpinion[] qData = webOpinion.GetDatas("", strOpNum, intId);
                if (qData != null)
                {
                    data = qData[0];
                }
                else
                {
                    data = new DataOpinion();
                }

                //nodes[i].SelectSingleNode("autonum").InnerText;
                data.OpNum = nodes[i].SelectSingleNode("op_id").InnerText;
                string strOrgType = nodes[i].SelectSingleNode("dept_classify").InnerText;
                //nodes[i].SelectSingleNode("dept_bsort").InnerText;
                string strSsort = nodes[i].SelectSingleNode("dept_ssort").InnerText;
                DataOp qOp = new DataOp();
                qOp.OpType = "承办单位";
                qOp.OpValue2 = "%|" + strSsort;
                DataOp[] opData = webOp.GetDatas(qOp, "OpValue");
                if (opData != null)
                {
                    strSsort = opData[0].OpValue;
                }
                if (strOrgType == "主办单位")
                {
                    if (!string.IsNullOrEmpty(data.ExamHostOrg))
                    {
                        if (data.ExamHostOrg.IndexOf(strSsort) < 0)
                        {
                            data.ExamHostOrg += "," + strSsort;
                        }
                    }
                    else
                    {
                        data.ExamHostOrg = strSsort;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(data.ExamHelpOrg))
                    {
                        if (data.ExamHelpOrg.IndexOf(strSsort) < 0)
                        {
                            data.ExamHelpOrg += "," + strSsort;
                        }
                    }
                    else
                    {
                        data.ExamHelpOrg = strSsort;
                    }
                }
                string strFlow = nodes[i].SelectSingleNode("dept_onflow").InnerText;
                switch (strFlow)
                {
                    case "1":
                    case "2":
                        data.ApplyState = "部门处理";
                        break;
                    case "4":
                    case "5":
                        data.ApplyState = "已办复";
                        break;
                    default://不签收-2,退回认可3,退回不认可-3,退回不认可中文(退回不认可)
                        data.ApplyState = "部门处理";
                        //data.ActiveName = "退回";
                        break;
                }
                string strRegTime = nodes[i].SelectSingleNode("dept_signedDate").InnerText;
                if (!string.IsNullOrEmpty(strRegTime))
                {
                    data.RegTime = Convert.ToDateTime(strRegTime);
                }
                //nodes[i].SelectSingleNode("dept_main_end").InnerText;
                string strPlannedDate = nodes[i].SelectSingleNode("dept_plan_date").InnerText;
                if (!string.IsNullOrEmpty(strPlannedDate))
                {
                    data.PlannedDate = Convert.ToDateTime(strPlannedDate);
                }
                //nodes[i].SelectSingleNode("if_historydealResult").InnerText;
                string strResultTime = nodes[i].SelectSingleNode("dept_deal_date").InnerText;
                if (!string.IsNullOrEmpty(strResultTime) && strOrgType == "主办单位")
                {
                    data.ResultTime = Convert.ToDateTime(strResultTime);
                }
                //nodes[i].SelectSingleNode("dept_remark").InnerText;
                string strResultInfo = nodes[i].SelectSingleNode("dept_result").InnerText;
                if (!string.IsNullOrEmpty(strResultInfo) || string.IsNullOrEmpty(data.ResultInfo))
                {
                    switch (strResultInfo)
                    {
                        case "007":
                            strResultInfo = "解决或采纳";
                            break;
                        case "009":
                            strResultInfo = "留作参考";//"留作参考或暂难解决";//
                            break;
                        case "010":
                            strResultInfo = "列入计划拟解决";
                            break;
                        case "006":
                            strResultInfo = "其他";
                            break;
                        default:
                            strResultInfo = "处理中";
                            break;
                    }
                    if (string.IsNullOrEmpty(data.ResultInfo) || data.ResultInfo == "解决或采纳")
                    {
                        data.ResultInfo = strResultInfo;
                    }
                }
                string strResultInfo2 = nodes[i].SelectSingleNode("dept_final_result").InnerText;
                if (!string.IsNullOrEmpty(strResultInfo2))
                {
                    switch (strResultInfo2)
                    {
                        case "007":
                            strResultInfo2 = "采纳或解决";
                            break;
                        case "009":
                            strResultInfo2 = "留作参考";//"留作参考或暂难解决";//
                            break;
                        case "010":
                            strResultInfo2 = "列入计划拟解决";
                            break;
                        case "006":
                            strResultInfo2 = "其他";
                            break;
                        default:
                            strResultInfo2 = "处理中";
                            break;
                    }
                    if (string.IsNullOrEmpty(data.ResultInfo2) || data.ResultInfo2 == "解决或采纳")
                    {
                        data.ResultInfo2 = strResultInfo2;
                    }
                }
                string strResultBody = nodes[i].SelectSingleNode("dept_opinion").InnerText;
                if (!string.IsNullOrEmpty(strResultBody))
                {
                    string strOrg = "===" + strOrgType + " (" + strSsort + ") " + strResultTime + "===\n";
                    if (!string.IsNullOrEmpty(data.ResultInfo2))
                    {
                        if (!string.IsNullOrEmpty(data.ResultBody2))
                        {
                            if (data.ResultBody2.IndexOf(strResultBody) < 0)
                            {
                                data.ResultBody2 += "\n\n" + strOrg + strResultBody;
                            }
                        }
                        else
                        {
                            data.ResultBody2 = strOrg + strResultBody;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(data.ResultBody))
                        {
                            if (data.ResultBody.IndexOf(strResultBody) < 0)
                            {
                                data.ResultBody += "\n\n" + strOrg + strResultBody;
                            }
                        }
                        else
                        {
                            data.ResultBody = strOrg + strResultBody;
                        }
                    }
                }
                //nodes[i].SelectSingleNode("dept_final_date").InnerText;
                //nodes[i].SelectSingleNode("dept_DealLimitDate").InnerText;
                data.ResultUser = nodes[i].SelectSingleNode("dept_DealPerson").InnerText;
                data.ResultIp = nodes[i].SelectSingleNode("dept_DealPersonTel").InnerText;
                //nodes[i].SelectSingleNode("dept_ApplyDefer").InnerText;


                if (data.Id <= 0)
                {
                    data.AddTime = dtNow;
                    data.AddIp = strIp;
                    data.AddUser = strUser;

                    data.Id = webOpinion.Insert(data);//新增
                    strOut += "新增";
                }
                else
                {
                    data.UpTime = dtNow;
                    data.UpIp = strIp;
                    data.UpUser = strUser;

                    if (webOpinion.Update(data) <= 0)//更新
                    {
                        data.Id = -1;
                    }
                    strOut += "更新";
                }

                strOut += (data.Id > 0) ? " 成功" : " 失败";
                strOut += string.Format("：(流水号){0}，(提案号){1}，(案由){2}<br/>", data.OpNum, data.OpNo, data.Summary);
            }

            lblInfo.Text += strOut;
        }
        #endregion
        //
    }
}