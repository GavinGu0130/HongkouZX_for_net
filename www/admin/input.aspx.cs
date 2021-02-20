using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mod.main;
using hkzx.db;
using hkzx.user;

namespace hkzx.web.admin
{
    public partial class input : System.Web.UI.Page
    {
        DataAdmin myUser = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            myUser = HelperAdmin.GetUser();
            if (myUser == null || myUser.Id <= 0 || string.IsNullOrEmpty(myUser.AdminName))
            {
                //Response.Redirect("login.aspx?url=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                return;
            }
            if (myUser.Grade < 9 || (myUser.Powers.IndexOf("alls") < 0 && myUser.Powers.IndexOf("system") < 0))
            {
                Response.Redirect("./");
            }
            header1.UserName = myUser.TrueName;
            header1.LastTime = myUser.LastTime.ToString("yyyy-MM-dd HH:mm:ss");
            header1.Powers = myUser.Powers;
            plNav.Visible = true;
            if (myUser.TrueName == "Tony")
            {
                lnkTest.Visible = true;
            }
            if (!IsPostBack)
            {
                hfUser.Value = myUser.AdminName;
                string strTitle = "";
                switch (Request.QueryString["ac"])
                {
                    case "test":
                        plTest.Visible = true;
                        strTitle = "生成测试数据";
                        break;
                    case "perform":
                        plPerform.Visible = true;
                        strTitle = "导入委员活动情况";
                        break;
                    case "opinion":
                        plOpinion.Visible = true;
                        strTitle = "导入提案信息";
                        break;
                    case "pop":
                        plPop.Visible = true;
                        strTitle = "导入社情民意信息";
                        break;
                    default:
                        plUser.Visible = true;
                        strTitle = "导入委员信息";
                        break;
                }
                Header.Title += " - " + strTitle;
            }
        }
        //
        #region 导入委员信息
        //导入数据
        protected void btnUser_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            WebUser webUser = new WebUser();
            string strFile = txtUserFile.Text.Trim();
            if (string.IsNullOrEmpty(strFile))
            {
                return;
            }
            DataUser[] data = xls2User(strFile);
            if (data != null)
            {
                for (int i = 0; i < data.Count(); i++)
                {
                    DataUser[] ckData = webUser.GetDatas(config.PERIOD, data[i].TrueName, "Id");//重名检查
                    if (ckData != null)
                    {
                        //是否更新操作
                        data[i].Id = ckData[0].Id;
                        if (!string.IsNullOrEmpty(data[i].IdCard) && data[i].IdCard.Length == 18)
                        {
                            data[i].IdCard = HelperSecret.DESEncrypt(data[i].IdCard.ToUpper(), config.IDDESKEY, config.IDDESIV);//des加密
                        }
                        string strPwd = (!string.IsNullOrEmpty(data[i].Mobile)) ? data[i].Mobile.Substring(data[i].Mobile.Length - 6) : "123456";
                        data[i].UserPwd = HelperSecret.MD5Encrypt(strPwd);
                        data[i].Active = 1;
                        if (webUser.Update(data[i]) > 0)
                        {

                        }
                        else
                        {
                            data[i].rowClass = " class='save'";
                        }
                    }
                    else
                    {//没有查询到，直接导入操作
                        if (!string.IsNullOrEmpty(data[i].IdCard))// && data[i].IdCard.Length == 18
                        {
                            data[i].IdCard = HelperSecret.DESEncrypt(data[i].IdCard.ToUpper(), config.IDDESKEY, config.IDDESIV);//des加密
                        }
                        string strPwd = (!string.IsNullOrEmpty(data[i].Mobile)) ? data[i].Mobile.Substring(data[i].Mobile.Length - 6) : "123456";
                        data[i].UserPwd = HelperSecret.MD5Encrypt(strPwd);
                        data[i].Active = 1;
                        data[i].Id = webUser.Insert(data[i]);
                        if (data[i].Id <= 0)
                        {
                            data[i].rowClass = " class='cancel'";
                        }
                    }
                }
                plUserData.Visible = true;
                rpUserList.DataSource = data;
                rpUserList.DataBind();
            }
        }
        //解析xlsx
        private DataUser[] xls2User(string strFile)
        {
            string strThead = "";
            string strTbody = "";
            ArrayList dataList = new ArrayList();

            DataTable myT = getTable(strFile);
            string[] colName = new string[myT.Columns.Count];
            for (int i = 0; i < myT.Rows.Count; i++)
            {
                //string strTr = "";
                //foreach (System.Data.DataColumn column in myT.Columns)
                //{
                //    strTr += string.Format("<td>{0}</td>", myT.Rows[i][column]);
                //}
                //strTbody += string.Format("<tr>{0}</tr>", strTr);
                //if (i == 0)
                //{
                //    string strTmp = "";
                //    foreach (System.Data.DataColumn column in myT.Columns)
                //    {
                //        strTmp += myT.Rows[i][column].ToString();
                //    }
                //    strThead += "<tr><th colspan='" + myT.Columns.Count.ToString() + "'>" + strTmp + "</th></tr>";
                //}
                if (i == 0)
                {
                    for (int j = 0; j < colName.Count(); j++)
                    {
                        colName[j] = myT.Rows[i][j].ToString();
                    }
                    strThead += "<tr><th>" + string.Join("</th><th>", colName) + "</th></tr>";
                }
                else
                {
                    DataUser data = new DataUser();
                    data.UserType = "委员";
                    data.Period = ddlPeriod.SelectedValue.Trim();
                    string strTmp = "";
                    for (int j = 0; j < colName.Count(); j++)
                    {
                        strTmp += "<td>" + myT.Rows[i][j].ToString() + "</td>";
                        setUser(data, colName[j], myT.Rows[i][j].ToString());
                    }
                    strTbody += "<tr>" + strTmp + "</tr>";
                    if (!string.IsNullOrEmpty(data.TrueName))
                    {
                        data.num = i;
                        if (!string.IsNullOrEmpty(data.Role))
                        {
                            data.Role = "," + data.Role.Trim(',') + ",";
                        }
                        dataList.Add(data);
                    }
                }
                //
            }
            //plIn.Visible = true;
            //ltInData.Text = string.Format("<table><thead>{0}</thead><tbody>{1}</tbody></table>", strThead, strTbody);
            if (dataList.Count > 0)
            {
                return (DataUser[])dataList.ToArray(typeof(DataUser));
            }
            return null;
        }
        //匹配字段
        private void setUser(DataUser data, string colName, string strVal)
        {
            if (string.IsNullOrEmpty(strVal))
            {
                return;
            }
            switch (colName)
            {
                case "委员编号":
                    data.UserCode = strVal;
                    break;
                case "姓名":
                    data.TrueName = strVal;
                    break;
                case "性别":
                    data.UserSex = strVal;
                    break;
                case "单位名称":
                case "单位名称及职务":
                    data.OrgName = strVal;
                    break;
                case "单位职务":
                    data.OrgDuty = strVal;
                    break;
                case "委员生日":
                    data.Birthday = Convert.ToDateTime(strVal);
                    break;
                case "手机":
                    data.Mobile = strVal;
                    break;
                case "籍贯":
                    data.Native = strVal;
                    break;
                case "民族":
                    data.Nation = strVal;
                    break;
                case "文化程度":
                    data.Education = strVal;
                    break;
                case "微信号码":
                    data.WeChat = strVal;
                    break;
                case "电子邮件":
                    data.Email = strVal;
                    break;
                case "家庭地址":
                    data.HomeAddress = strVal;
                    break;
                case "单位地址":
                    data.OrgAddress = strVal;
                    break;
                case "证件号码":
                    data.IdCard = strVal;
                    break;
                case "所属专委会1":
                    data.Committee = strVal;
                    break;
                case "所属专委会2":
                    data.Committee2 = strVal;
                    break;
                case "专委会职务":
                    if (!string.IsNullOrEmpty(data.Role))
                    {
                        if (data.Role.IndexOf(strVal) >= 0)
                        {
                            return;
                        }
                        data.Role += ",";
                    }
                    if (strVal == "主任")
                    {
                        strVal = "专委会主任";
                    }
                    else if (strVal == "副主任")
                    {
                        strVal = "专委会副主任";
                    }
                    data.Role += strVal;
                    //data.CommitteeDuty = strVal;
                    break;
                case "界别":
                    data.Subsector = strVal;
                    break;
                case "界别职务":
                    if (!string.IsNullOrEmpty(data.Role))
                    {
                        if (data.Role.IndexOf(strVal) >= 0)
                        {
                            return;
                        }
                        data.Role += ",";
                    }
                    if (strVal == "召集人")
                    {
                        strVal = "界别召集人";
                    }
                    data.Role += strVal;
                    //data.SubsectorDuty = strVal;
                    break;
                case "界别活动组":
                    data.Subsector2 = strVal;
                    break;
                case "街道活动组":
                    data.StreetTeam = strVal;
                    break;
                case "街道活动组职务":
                    if (!string.IsNullOrEmpty(data.Role))
                    {
                        if (data.Role.IndexOf(strVal) >= 0)
                        {
                            return;
                        }
                        data.Role += ",";
                    }
                    if (strVal == "组长")
                    {
                        strVal = "街道活动组组长";
                    }
                    else if (strVal == "副组长")
                    {
                        strVal = "街道活动组副组长";
                    }
                    if (strVal.IndexOf("地区联络组") >= 0)
                    {
                        strVal = strVal.Replace("地区联络组", "街道活动组");
                    }
                    data.Role += strVal;
                    //data.StreetTeamDuty = strVal;
                    break;
                case "政治面貌1":
                case "政治面貌2":
                    if (!string.IsNullOrEmpty(data.Party))
                    {
                        data.Party += ",";
                    }
                    data.Party += strVal.Replace("\n", "");
                    break;
                case "政协职务":
                    if (!string.IsNullOrEmpty(data.Role))
                    {
                        data.Role += ",";
                    }
                    string[] arr = strVal.Split(' ');
                    //for (int i = 0; i < arr.Count(); i++)
                    //{
                    //    if (arr[i] == "副秘书长")
                    //    {
                    //        arr[i] = "秘书长";
                    //    }
                    //}
                    data.Role += string.Join(",", arr);
                    break;
                case "单位邮编":
                    data.OrgZip = HelperMain.SqlFilter(strVal, 6);
                    break;
                case "家庭邮编":
                    data.HomeZip = HelperMain.SqlFilter(strVal, 6);
                    break;
                case "单位电话":
                    data.OrgTel = strVal;
                    break;
                case "家庭电话":
                    data.HomeTel = strVal;
                    break;
                case "信封地址":
                case "信封地址及邮编":
                    data.ContactAddress = strVal;
                    break;
                case "宗教信仰":
                    data.Religion = strVal;
                    break;
                case "荣誉称号":
                    data.Honor = strVal;
                    break;
                case "社会职务":
                    data.SocietyDuty = strVal;
                    break;
                case "信封邮编":
                    data.ContactZip = HelperMain.SqlFilter(strVal, 6);
                    break;
                default:
                    break;
            }
        }
        #endregion
        //
        #region 导入委员活动数据
        //导入数据
        protected void btnPerform_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            string strFile = txtPerformFile.Text.Trim();
            if (string.IsNullOrEmpty(strFile))
            {
                return;
            }

            string xlsFilePath = HttpContext.Current.Server.MapPath(strFile);
            //源的定义
            string strConn = "";
            if (strFile.IndexOf(".xlsx") > 0 && strFile.EndsWith("xlsx"))
            {
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + xlsFilePath + "';Extended Properties='Excel 12.0;HDR=NO;IMEX=1'";//HDR=YES
            }
            else if (strFile.IndexOf(".xls") > 0 && strFile.EndsWith("xls"))
            {
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + xlsFilePath + ";" + "Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
            }
            else
            {
                return;
            }
            //string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + excFilePath + ";" + "Extended Properties=Excel 8.0;";
            //连接数据源  
            System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn);
            conn.Open();
            //对于EXCEL中的表即sheet([sheet1$])如果不是固定的可以使用下面的方法得到
            System.Data.DataTable schemaTable = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);

            WebUser webUser = new WebUser();
            WebPerform webPerform = new WebPerform();
            WebPerformFeed webFeed = new WebPerformFeed();
            string PerformActiveName = "导入";
            for (int x = 0; x < schemaTable.Rows.Count; x += 2)
            {
                string tableName = schemaTable.Rows[x][2].ToString().Trim();
                if (tableName.IndexOf("Sheet") < 0)
                {
                    int UserId = 0;
                    //string SignManType = "";
                    string TrueName = tableName.Replace("$", "");
                    TrueName = TrueName.Trim();
                    DataUser[] uData = webUser.GetDatas(config.PERIOD, TrueName, "Id, Role");
                    if (uData != null)
                    {
                        UserId = uData[0].Id;
                        //if (uData[0].Role.IndexOf("常委") >= 0)
                        //{
                        //    SignManType = "常委";
                        //}
                    }
                    ltInData.Text += TrueName + "[" + UserId.ToString() + "]<br/>";
                
                    //Sql语句  
                    string strExcel = "select * from [" + tableName + "]";
                    //适配到数据源  
                    System.Data.OleDb.OleDbDataAdapter myCommand = new System.Data.OleDb.OleDbDataAdapter(strExcel, strConn);
                    //定义存放的数据表  
                    System.Data.DataSet ds = new System.Data.DataSet();
                    myCommand.Fill(ds, tableName);

                    DataTable myT = ds.Tables[tableName];
                    string strThead = "";
                    string strTbody = "";
                    //ArrayList dataList = new ArrayList();
                    string[] colName = new string[myT.Columns.Count];
                    for (int i = 0; i < myT.Rows.Count; i++)
                    {
                        //string strTr = "";
                        //foreach (System.Data.DataColumn column in myT.Columns)
                        //{
                        //    strTr += string.Format("<td>{0}</td>", myT.Rows[i][column]);
                        //}
                        //strTbody += string.Format("<tr>{0}</tr>", strTr);
                        if (i < 2)
                        {

                        }
                        else if (i == 2)
                        {
                            for (int j = 0; j < colName.Count(); j++)
                            {
                                colName[j] = myT.Rows[i][j].ToString();
                            }
                            strThead += "<tr><th>" + string.Join("</th><th>", colName) + "</th></tr>";
                        }
                        else
                        {
                            string strNum = myT.Rows[i][0].ToString();
                            if (!string.IsNullOrEmpty(strNum) && strNum.Length < 4)
                            {
                                //string strTr = "";
                                //foreach (System.Data.DataColumn column in myT.Columns)
                                //{
                                //    strTr += string.Format("<td>{0}</td>", myT.Rows[i][column]);
                                //}
                                //strTbody += string.Format("<tr>{0}</tr>", strTr);

                                DataPerform pData = new DataPerform();
                                string strTmp = "";
                                for (int j = 0; j < colName.Count(); j++)
                                {
                                    strTmp += "<td>" + myT.Rows[i][j].ToString() + "</td>";
                                    setPerform(pData, colName[j], myT.Rows[i][j].ToString());
                                }
                                strTbody += "<tr>" + strTmp + "</tr>";
                                if (!string.IsNullOrEmpty(pData.Title))
                                {
                                    pData.num = i - 2;
                                    pData.ActiveName = PerformActiveName;
                                    int PerformId = inputPerform(webPerform, pData);//增加活动
                                    if (PerformId > 0)
                                    {
                                        DataPerformFeed fData = new DataPerformFeed();
                                        fData.PerformId = PerformId;
                                        fData.UserId = UserId;
                                        fData.SignMan = TrueName;
                                        //fData.SignManType = (pData.SubType == "常委会议") ? SignManType : "";
                                        fData.ActiveName = pData.FeedActiveName;
                                        inputFeed(webFeed, fData);//记录出席情况
                                    }
                                }
                            }
                        }
                    }
                    ltInData.Text += string.Format("<table><thead>{0}</thead><tbody>{1}</tbody></table>", strThead, strTbody);
                    //if (x > 10)
                    //{
                    //    break;
                    //}
                }

                //if (dataList.Count > 0)
                //{
                //    return (DataPerform[])dataList.ToArray(typeof(DataPerform));
                //}
            }
            //关闭数据源
            conn.Close();

            updataPerform(webPerform, PerformActiveName, webFeed, webUser);//更新活动类型、状态、出席人员

            plIn.Visible = true;
        }
        //匹配字段
        private void setPerform(DataPerform data, string colName, string strVal)
        {
            if (string.IsNullOrEmpty(strVal))
            {
                return;
            }
            switch (colName)
            {
                //case "序号":
                //    data.FeedNo = strVal;
                //    break;
                case "活动主题":
                    data.Title = strVal;
                    break;
                case "活动开始时间":
                    data.SignTime = Convert.ToDateTime(strVal);
                    data.StartTime = data.SignTime;
                    data.EndTime = data.SignTime;
                    data.OverTime = data.SignTime;
                    break;
                case "活动类别":
                    data.SubType = strVal;
                    break;
                case "活动地点":
                    data.PerformSite = strVal;
                    break;
                case "出席情况":
                    data.FeedActiveName = strVal;
                    break;
                default:
                    break;
            }
        }
        //增加活动
        private int inputPerform(WebPerform webPerform, DataPerform data)
        {
            //校正活动类型

            //新增、更新活动
            DataPerform qData = new DataPerform();
            qData.Title = data.Title;
            qData.SignTime = data.SignTime;
            DataPerform[] ckData = webPerform.GetDatas(qData, "Id");
            if (ckData != null)
            {
                data.Id = ckData[0].Id;
                if (webPerform.Update(data) > 0)
                {
                    data.rowClass = " class='save'";
                }
                else
                {
                    data.rowClass = " class='cancel'";
                    data.Id = 0;
                }
            }
            else
            {
                //没有查询到，直接导入操作
                data.Id = webPerform.Insert(data);
                if (data.Id <= 0)
                {
                    data.rowClass = " class='cancel'";
                }
            }
            return data.Id;
        }
        //增加出席情况
        private void inputFeed(WebPerformFeed webFeed, DataPerformFeed data)
        {
            DataPerformFeed[] ckData = webFeed.GetDatas("", data.PerformId, data.UserId, data.SignMan, "Id");
            if (ckData != null)
            {
                data.Id = ckData[0].Id;
                if (webFeed.Update(data) > 0)
                {
                    data.rowClass = " class='save'";
                }
                else
                {
                    data.rowClass = " class='cancel'";
                    data.Id = 0;
                }
            }
            else
            {
                //没有查询到，直接导入操作
                data.Id = webFeed.Insert(data);
                if (data.Id <= 0)
                {
                    data.rowClass = " class='cancel'";
                }
            }
        }
        //更新活动出席人员
        string[,] arrPerform = {
            { "文卫体委学习考察", "专委会会议及活动-学习考察", "专委会-文化卫生体育委员会" }
            ,{ "大力推进虹口国际创新港建设，提升科技创新能力“专题协商议政会", "专题协商议政会", "" }
            ,{ "市政协党组领导赴区政协参加习近平总书记关于加强和改进人民政协工作的重要思想座谈会方案", "其他会议", "" }
            ,{ "新委员培训会", "区政协组织的委员培训学习班", "" }
            ,{ "区政协委员列席区十六届人大常委会第十七次会议（扩大）", "政协全体会议", "" }
            ,{ "社情民意信息工作座谈会（8月）", "社情民意专题会", "" }
            ,{ "大会发言（前期准备）界别召集人会", "界别-全体会议", "" }
            ,{ "走基层、重民生、保安全——“食品安全”主题活动", "专项民主监督", "" }
            ,{ "游戏化的家庭教育怎么做", "其他活动", "" }
            ,{ "曲阳路街道活动组、广中路街道活动组“走基层、重民生、保安全”主题活动（“六无”创建）", "专项民主监督", "街道活动组-曲阳路街道活动组,街道活动组-广中路街道活动组" }
            ,{ "北外滩街道活动组、江湾镇街道活动组高温慰问交警主题活动", "专项民主监督", "街道活动组-北外滩街道活动组,街道活动组-江湾镇街道活动组" }
            ,{ "委员学习报告会", "全体委员学习会", "" }
            ,{ "嘉兴路街道活动组、凉城新村街道活动组“走基层、重民生、保安全”主题活动（河道整治视察）", "专项民主监督", "街道活动组-嘉兴路街道活动组,街道活动组-凉城新村街道活动组" }
            ,{ "“关于优化和提升虹口区产业园区功能”课题调研", "课题组视察", "" }
            ,{ "专题协商+主席促办重点专题提案（养老）-石、郭、符", "专题协商议政会,主席促办重点专题提案会、提案办理民主评议会", "" }
            ,{ "区政协2018年度教师节慰问活动", "界别-界别视察", "" }
            ,{ "和美虹口——随手拍虹口", "其他活动", "" }
            ,{ "委员培训班（3组）", "区政协组织的委员培训学习班", "" }
            ,{ "区十四届政协第三期委员培训班全体班委会议", "", "" }//不记分
            ,{ "足迹虹口——户外定向", "其他活动", "" }
            ,{ "社情民意信息工作座谈会（9月）", "社情民意专题会", "" }
            ,{ "2018庆中秋国庆暨改革开放40周年活动——记忆虹口（趣味运动）", "其他活动", "" }
            ,{ "城市精细化管理同心圆下午茶活动", "“同心”系列活动 同心圆下午茶", "界别-民建" }
            ,{ "区十四届政协第十次常委会", "常委会议", "" }
            ,{ "“提质增效新征程 改革开放再出发”2018虹口政协迎国庆主题活动", "其他活动", "" }
            ,{ "走访20", "其他活动", "" }
            ,{ "区政协环境和城市建设委员会同心桥活动", "“同心”系列活动 同心桥", "专委会-环境和城市建设委员会" }
            ,{ "邀请区政协委员对中国国际进口博览会市容环境综合整治情况开展巡察的函", "", "" }//不记分
            ,{ "同心桥活动（经济委员会）", "“同心”系列活动 同心桥", "专委会-经济委员会" }
            ,{ "政协提案办理民主评议（区人社局）", "主席促办重点专题提案会、提案办理民主评议会", "" }
            ,{ "提案委员会学习交流", "专委会会议及活动-学习考察", "" }
            ,{ "构建立体化治安防控体系，提升社会综合治理水平同心圆下午茶活动", "“同心”系列活动 同心圆下午茶", "界别-民进" }
            ,{ "社法委同心桥活动", "“同心”系列活动 同心桥", "专委会-社会和法制委员会" }
            ,{ "虹口足球场大型活动智慧安保视察活动", "委员年末集中视察", "" }
            ,{ "区政协社情民意信息工作推进会", "社情民意工作会", "" }
            ,{ "2018年区政府实事项目执行及2019年政府实事项目院派专题议政会", "专题协商议政会", "" }
            ,{ "同心桥活动（科技委员会）", "“同心”系列活动 同心桥", "专委会-科技委员会" }
            ,{ "区政协教育学习文史委同心桥活动", "“同心”系列活动 同心桥", "专委会-教育学习文史委员会" }
            ,{ "同心桥活动（文化卫生体育委员会）", "“同心”系列活动 同心桥", "专委会-文化卫生体育委员会" }
            ,{ "区政协提案委员会全体会议", "专委会会议及活动-全体会议", "" }
            ,{ "关于我区优化营商环境，推进一网通办工作情况的年终视察", "委员年末集中视察", "" }
            ,{ "关于我区社会事业发展情况的年终视察", "委员年末集中视察", "" }
            ,{ "关于我区城市精细化管理情况年终视察", "委员年末集中视察", "" }
            ,{ "发展社会事业，建设幸福虹口同心圆下午茶活动", "“同心”系列活动 同心圆下午茶", "界别-教育界" }
            ,{ "年终视察专题二：关于我区科创中心建设情况", "委员年末集中视察", "" }
            ,{ "年终视察专题四：关于我区养老工作和市民驿站建设运行情况", "委员年末集中视察", "" }
            ,{ "专委会主任会议", "专委会会议及活动-主任会议", "" }
            ,{ "政协委员话改革开放四十周年", "其他会议", "" }
            ,{ "虹口区第十四届政协第十一次常委会", "常委会议", "" }
            ,{ "区政协党员委员培训班", "区政协组织的委员培训学习班", "" }
            ,{ "区政协十四届三次会议临时党委第一次会议", "", "" }//不记分
        };
        private void updataPerform(WebPerform webPerform, string ActiveName, WebPerformFeed webFeed, WebUser webUser)
        {
            DataPerform qData = new DataPerform();
            qData.ActiveName = ActiveName;
            qData.AddTime = DateTime.Today;
            DataPerform[] data = webPerform.GetDatas(qData);
            if (data == null)
            {
                return;
            }
            for (int i = 0; i < data.Count(); i++)
            {
                DataPerformFeed qFeed = new DataPerformFeed();
                qFeed.PerformId = data[i].Id;
                DataPerformFeed[] fData = webFeed.GetDatas(qFeed);
                if (fData != null)
                {
                    string strAttendees = "";
                    for (int j = 0; j < fData.Count(); j++)
                    {
                        strAttendees += "," + fData[j].SignMan;
                    }
                    for (int j = 0; j < arrPerform.GetLength(0); j++)
                    {
                        if (data[i].Title == arrPerform[j, 0])
                        {
                            data[i].SubType = arrPerform[j, 1];//活动类型
                            data[i].OrgName = arrPerform[j, 2];//申请部门
                            break;
                        }
                    }
                    bool blActive = false;
                    for (int j = 0; j < fData.Count(); j++)
                    {
                        if (data[i].SubType == "常委会议")
                        {
                            DataUser[] uData = webUser.GetDatas(config.PERIOD, fData[j].SignMan, "Role");
                            if (uData != null && uData[0].Role.IndexOf("常委") >= 0)
                            {
                                fData[j].SignManType = "常委";
                                webFeed.Update(fData[j]);
                            }
                        }
                        if (fData[j].ActiveName == "已出席")
                        {
                            blActive = true;
                        }
                    }
                    if (blActive && !string.IsNullOrEmpty(data[i].SubType))
                    {
                        data[i].IsMust = (data[i].SubType == "政协全体会议" || data[i].SubType == "全体委员学习会") ? "必须参加" : "报名参加";
                        data[i].Attendees = (!string.IsNullOrEmpty(strAttendees)) ? strAttendees + "," : "";
                        webPerform.Update(data[i]);//更新活动
                        //计分
                        string strUser = HelperMain.SqlFilter(myUser.AdminName, 20);
                        new admin.perform().ClosePerform(strUser, data[i].Id);
                        data[i].ActiveName = "履职关闭";//"发布"
                        webPerform.Update(data[i]);//更新活动
                    }
                    //
                }
            }
        }
        #endregion
        //
        #region 导入提案数据
        //导入数据
        protected void btnOpinion_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            string strFile = txtOpFile.Text.Trim();
            if (string.IsNullOrEmpty(strFile))
            {
                return;
            }
            DataOpinion[] data = xls2Opinion(strFile);
            if (data != null)
            {
                inputOpinion(data);
            }
        }
        //解析xlsx
        private DataOpinion[] xls2Opinion(string strFile)
        {
            DataTable myT = getTable(strFile);

            string strThead = "";
            string strTbody = "";
            WebOp webOp = new WebOp();
            DataOp[] opData = webOp.GetDatas(0, "承办单位", "", "OpName,OpValue");
            ArrayList dataList = new ArrayList();

            string strPeriod = "";
            string strTimes = "";
            string[] colName = new string[myT.Columns.Count];
            for (int i = 0; i < myT.Rows.Count; i++)
            {
                //string strTr = "";
                //foreach (System.Data.DataColumn column in myT.Columns)
                //{
                //    strTr += string.Format("<td>{0}</td>", myT.Rows[i][column]);
                //}
                //strTbody += string.Format("<tr>{0}</tr>", strTr);
                if (i < 4)
                {
                    string strTmp = "";
                    foreach (System.Data.DataColumn column in myT.Columns)
                    {
                        strTmp += string.Format("<th>{0}</th>", myT.Rows[i][column]);
                    }
                    strThead += string.Format("<tr>{0}</tr>", strTmp);
                    if (i == 1)
                    {
                        string tmp = myT.Rows[1][7].ToString();
                        tmp = tmp.Replace("上海市虹口区政协", "");
                        strPeriod = tmp.Substring(0, tmp.IndexOf("届"));
                        tmp = tmp.Substring(tmp.IndexOf("届") + 1);
                        strTimes = tmp.Substring(0, tmp.IndexOf("次会议"));
                    }
                }
                else if (i == 4)
                {
                    for (int j = 0; j < colName.Count(); j++)
                    {
                        colName[j] = myT.Rows[i][j].ToString();
                    }
                    strThead += "<tr><th>" + string.Join("</th><th>", colName) + "</th></tr>";
                }
                else
                {
                    DataOpinion data = new DataOpinion();
                    data.Period = strPeriod;
                    data.Times = strTimes;
                    string strTmp = "";
                    for (int j = 0; j < colName.Count(); j++)
                    {
                        strTmp += "<td>" + myT.Rows[i][j].ToString() + "</td>";
                        setOpinion(data, colName[j], myT.Rows[i][j].ToString());
                    }
                    strTbody += "<tr>" + strTmp + "</tr>";
                    if (!string.IsNullOrEmpty(data.OpNo))
                    {
                        if (!string.IsNullOrEmpty(data.ExamHostOrg))
                        {
                            string[] arr = data.ExamHostOrg.Split(',');
                            for (int j = 0; j < arr.Count(); j++)
                            {
                                if (arr[j].IndexOf("|") < 0)
                                {
                                    for (int x = 0; x < opData.Count(); x++)
                                    {
                                        if (opData[x].OpName.Trim() == arr[j])
                                        {
                                            arr[j] = opData[x].OpValue;
                                            break;
                                        }
                                    }
                                }
                            }
                            data.ExamHostOrg = string.Join(",", arr);
                        }
                        if (!string.IsNullOrEmpty(data.ExamHelpOrg))
                        {
                            string[] arr = data.ExamHelpOrg.Split(',');
                            for (int j = 0; j < arr.Count(); j++)
                            {
                                if (arr[j].IndexOf("|") < 0)
                                {
                                    for (int x = 0; x < opData.Count(); x++)
                                    {
                                        if (opData[x].OpName.Trim() == arr[j])
                                        {
                                            arr[j] = opData[x].OpValue;
                                            break;
                                        }
                                    }
                                }
                            }
                            data.ExamHelpOrg = string.Join(",", arr);
                        }
                        data.num = i - 4;
                        dataList.Add(data);
                    }
                }

            }
            //plIn.Visible = true;
            //ltInData.Text = string.Format("<table><thead>{0}</thead><tbody>{1}</tbody></table>", strThead, strTbody);

            if (dataList.Count > 0)
            {
                return (DataOpinion[])dataList.ToArray(typeof(DataOpinion));
            }
            return null;
        }
        //匹配字段
        private void setOpinion(DataOpinion data, string colName, string strVal)
        {
            if (string.IsNullOrEmpty(strVal))
            {
                return;
            }
            switch (colName)
            {
                case "流水号":
                    data.OpNum = strVal;
                    break;
                case "提案序号":
                    data.OpNo = strVal;
                    break;
                case "提案者":
                    string[] arrSubMans = strVal.Split(',');
                    if (arrSubMans.Count() > 0)
                    {
                        data.SubMan = "," + arrSubMans[0] + ",";
                    }
                    //if (arrSubMans.Count() > 1)
                    //{
                    //    data.SubMan2 = "," + arrSubMans[1] + ",";
                    //}
                    if (arrSubMans.Count() > 1)
                    {
                        string strTmp = "";
                        for (int i = 2; i < arrSubMans.Count(); i++)
                        {
                            strTmp += "," + arrSubMans[i];
                        }
                        data.SubMans = strTmp + ",";
                    }
                    break;
                case "主办单位":
                    data.ExamHostOrg = strVal;
                    break;
                case "会办单位":
                    data.ExamHelpOrg = strVal;
                    break;
                case "办理结果":
                    data.ResultInfo = strVal;
                    break;
                case "意见反馈":
                    data.Remark = "意见反馈：" + strVal;
                    break;
                case "案由":
                    data.Summary = strVal;
                    break;
                case "性质":
                    data.ActiveName = strVal;
                    break;
                case "再办理":
                    data.ReApply = strVal;
                    break;
                //case "跟踪办理情况":
                //case "跟踪办理情况hidden":
                //    break;
                case "跟踪办理结果":
                    data.ResultInfo2 = strVal;
                    break;
                case "提案分类":
                    data.SubType = strVal;
                    if (data.SubType == "其它")
                    {
                        data.SubType = "其他";
                    }
                    break;
                default:
                    break;
            }
        }
        //写入数据库
        private void inputOpinion(DataOpinion[] data)
        {
            WebOpinion webOpin = new WebOpinion();
            WebUser webUser = new WebUser();
            decimal ScoreProperty = 0;
            decimal ScoreProperty2 = 0;
            string TitleProperty = "立案提案";
            WebScore webScore = new WebScore();
            DataScore[] sData = webScore.GetDatas(1, "提交提案", "", TitleProperty + "%", "score,score2");
            if (sData != null)
            {
                ScoreProperty = sData[0].Score;
                ScoreProperty2 = sData[0].Score2;
            }
            string TableName = "tb_Opinion";
            string strIp = HelperMain.GetIpPort();
            string strUser = (myUser != null) ? myUser.AdminName : hfUser.Value;
            for (int i = 0; i < data.Count(); i++)
            {
                DataUser[] uData = webUser.GetDatas(config.PERIOD, data[i].SubMan.Trim(','), "UserType");
                if (uData != null)
                {
                    data[i].SubManType = uData[0].UserType;
                }
                else
                {
                    data[i].SubManType = "团体";
                }
                string strUserId = "";
                string strUserIds = "";
                string strParty = "";
                string strCommittee = "";
                string strSubsector = "";
                string strStreetTeam = "";
                if (!string.IsNullOrEmpty(data[i].SubMan))
                {
                    string[] arr = data[i].SubMan.Split(',');
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
                if (!string.IsNullOrEmpty(data[i].SubMans))
                {
                    string[] arr = data[i].SubMans.Split(',');
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
                data[i].Party = strParty.Trim(',');
                data[i].Committee = strCommittee.Trim(',');
                data[i].Subsector = strSubsector.Trim(',');
                data[i].StreetTeam = strStreetTeam.Trim(',');
                DataOpinion qData = new DataOpinion();
                qData.OpNo = data[i].OpNo;
                DataOpinion[] ckData = webOpin.GetDatas(qData, "Id");//重复检查
                if (ckData != null)
                {
                    data[i].Id = ckData[0].Id;
                    if (webOpin.Update(data[i]) > 0)
                    {
                        data[i].rowClass = " class='save'";
                    }
                    else
                    {
                        data[i].rowClass = " class='cancel'";
                        data[i].Id = 0;
                    }
                }
                else
                {//没有查询到，直接导入操作
                    data[i].Id = webOpin.Insert(data[i]);
                    if (data[i].Id <= 0)
                    {
                        data[i].rowClass = " class='cancel'";
                    }
                }
                if (!string.IsNullOrEmpty(strUserId) && ScoreProperty2 > 0)
                {
                    string[] arr = strUserIds.Trim(',').Split(',');
                    for (int j = 0; j < arr.Count(); j++)
                    {
                        int UserId = Convert.ToInt32(arr[j]);
                        DateTime dtTime = (data[i].SubTime > DateTime.MinValue) ? data[i].SubTime : DateTime.Now;
                        PublicMod.AddScore(UserId, TitleProperty + "（第一提案人）", ScoreProperty, TableName, data[i].Id, strIp, strUser, dtTime);//立案
                    }
                }
                if (!string.IsNullOrEmpty(strUserIds) && ScoreProperty > 0)
                {
                    string[] arr = strUserIds.Trim(',').Split(',');
                    for (int j = 0; j < arr.Count(); j++)
                    {
                        int UserId = Convert.ToInt32(arr[j]);
                        DateTime dtTime = (data[i].SubTime > DateTime.MinValue) ? data[i].SubTime : DateTime.Now;
                        PublicMod.AddScore(UserId, TitleProperty + "（联名）", ScoreProperty, TableName, data[i].Id, strIp, strUser, dtTime);//立案
                    }
                }
                data[i].SubMan = data[i].SubMan.Trim(',');
                //if (!string.IsNullOrEmpty(data[i].SubMan2))
                //{
                //    data[i].SubMan2 = data[i].SubMan2.Trim(',');
                //}
                if (!string.IsNullOrEmpty(data[i].SubMans))
                {
                    data[i].SubMans = data[i].SubMans.Trim(',');
                }
            }
            plOpData.Visible = true;
            rpOpList.DataSource = data;
            rpOpList.DataBind();
        }
        #endregion
        //
        #region 导入社情民意数据
        //导入数据
        protected void btnPop_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            string strFile = txtPopFile.Text.Trim();
            if (string.IsNullOrEmpty(strFile))
            {
                return;
            }
            DataOpinionPop[] data = xls2Pop(strFile);
            if (data != null)
            {
                inputPop(data);
            }
        }
        //解析xlsx
        private DataOpinionPop[] xls2Pop(string strFile)
        {
            DataTable myT = getTable(strFile);

            string strThead = "";
            string strTbody = "";
            ArrayList dataList = new ArrayList();
            string[] colName = new string[myT.Columns.Count];
            for (int i = 0; i < myT.Rows.Count; i++)
            {
                //string strTr = "";
                //foreach (System.Data.DataColumn column in myT.Columns)
                //{
                //    strTr += string.Format("<td>{0}</td>", myT.Rows[i][column]);
                //}
                //strTbody += string.Format("<tr>{0}</tr>", strTr);
                if (i < 2)
                {

                }
                else if (i == 2)
                {
                    for (int j = 0; j < colName.Count(); j++)
                    {
                        colName[j] = myT.Rows[i][j].ToString();
                    }
                    strThead += "<tr><th>" + string.Join("</th><th>", colName) + "</th></tr>";
                }
                else
                {
                    DataOpinionPop data = new DataOpinionPop();
                    data.Adopt1 = "";
                    data.Give1 = "";
                    data.Employ1 = "";
                    data.Reply1 = "";
                    data.Adopt2 = "";
                    data.Send2 = "";
                    data.Give2 = "";
                    data.Employ2 = "";
                    data.Reply2 = "";
                    data.Give3 = "";
                    data.Employ3 = "";
                    data.Reply3 = "";
                    string strTmp = "";
                    for (int j = 0; j < colName.Count(); j++)
                    {
                        strTmp += "<td>" + myT.Rows[i][j].ToString() + "</td>";
                        setPop(data, colName[j], myT.Rows[i][j].ToString());
                    }
                    data.ActiveName = (!string.IsNullOrEmpty(data.Adopt1)) ? "已录用" : "留存";
                    strTbody += "<tr>" + strTmp + "</tr>";
                    if (!string.IsNullOrEmpty(data.OpNum))
                    {
                        data.num = i - 2;
                        dataList.Add(data);
                    }
                }
            }
            //plIn.Visible = true;
            //ltInData.Text = string.Format("<table><thead>{0}</thead><tbody>{1}</tbody></table>", strThead, strTbody);

            if (dataList.Count > 0)
            {
                return (DataOpinionPop[])dataList.ToArray(typeof(DataOpinionPop));
            }
            return null;
        }
        //匹配字段
        private void setPop(DataOpinionPop data, string colName, string strVal)
        {
            if (string.IsNullOrEmpty(strVal))
            {
                return;
            }
            switch (colName)
            {
                case "序号":
                    data.OpNum = strVal;
                    break;
                case "来稿时间":
                    if (strVal.IndexOf("月") > 0)
                    {
                        data.SubTime = new DateTime(2019, Convert.ToInt16(strVal.Replace("月", "")), 1);
                    }
                    else
                    {
                        data.SubTime = DateTime.Now;
                    }
                    break;
                case "姓名":
                    if (strVal.IndexOf("\n") >= 0)
                    {
                        strVal = strVal.Replace(" ", "").Replace("、", ",").Replace("\n", ",");
                    }
                    else
                    {
                        strVal = strVal.Replace(" ", ",").Replace("、", ",");
                    }
                    if (strVal.IndexOf(",") > 0)
                    {
                        data.SubMan = strVal.Substring(0, strVal.IndexOf(","));
                        data.SubMans = strVal.Substring(strVal.IndexOf(",")) + ",";
                        data.SubMans = data.SubMans.Replace(",,", ",");
                    }
                    else
                    {
                        data.SubMan = strVal;
                    }
                    break;
                case "界别":
                    data.Subsector = strVal;
                    break;
                case "反映单位":
                    data.Committee = strVal;
                    break;
                case "政治面貌":
                    data.Party = strVal;
                    break;
                case "期数":
                    data.SubType2 = strVal;
                    break;
                case "区政协采用情况":
                    if (strVal.IndexOf("采用") > 0)
                    {
                        data.Adopt1 = strVal;//.Replace("采用", "")
                    }
                    else
                    {
                        data.SubType2 = strVal;
                    }
                    break;
                case "报区":
                    data.Give1 = strVal;
                    break;
                case "区采纳落实情况":
                    if (strVal.IndexOf("批示") >= 0)
                    {
                        data.Reply1 = strVal;//领导批示
                    }
                    else
                    {
                        data.Employ1 = strVal;
                    }
                    break;
                case "报市":
                    data.Send2 = strVal;
                    break;
                case "市政协采用情况":
                    if (strVal.IndexOf("单篇") >= 0)
                    {
                        data.Adopt2 = "单篇";
                    }
                    else if (strVal.IndexOf("综合") >= 0)
                    {
                        data.Adopt2 = "综合";
                    }
                    if (strVal.IndexOf("全国政协") >= 0)
                    {
                        data.Give3 = strVal;
                    }
                    else
                    {
                        if (strVal.IndexOf("采用") >= 0 || strVal.IndexOf("批示") >= 0)
                        {
                            data.Employ2 = strVal;
                        }
                        else
                        {
                            data.Send2 = strVal;
                        }
                    }
                    break;
                case "全国政协采用/市（全国）领导批示":
                    if (strVal.IndexOf("全国") >= 0)
                    {
                        if (strVal.IndexOf("批示") >= 0)
                        {
                            //data.Reply3 = strVal;//中央领导批示
                            data.Employ3 = strVal;
                        }
                        else
                        {
                            data.Give3 = strVal;
                        }
                    }
                    else
                    {
                        data.Reply2 = strVal;//领导批示
                    }
                    break;
                default:
                    break;
            }
            if (colName.IndexOf("信息标题") >= 0)
            {
                data.Summary = strVal;
            }
        }
        //写入数据库
        private void inputPop(DataOpinionPop[] data)
        {
            WebOpinionPop webPop = new WebOpinionPop();
            WebUser webUser = new WebUser();
            //string TableName = "tb_Opinion_Pop";
            string strIp = HelperMain.GetIpPort();
            string strUser = (myUser != null) ? myUser.AdminName : hfUser.Value;
            for (int i = 0; i < data.Count(); i++)
            {
                //string strUserIds = "";
                DataUser[] uData = webUser.GetDatas(config.PERIOD, data[i].SubMan.Trim(','), "UserType");
                if (uData != null)
                {
                    data[i].SubManType = uData[0].UserType;
                    string strSubMans = data[i].SubMan;
                    if (!string.IsNullOrEmpty(data[i].SubMans))
                    {
                        strSubMans += "," + data[i].SubMans.Trim(',');
                    }
                    string strParty = "";
                    string strCommittee = "";
                    string strSubsector = "";
                    string strStreetTeam = "";
                    string[] arr = strSubMans.Split(',');
                    for (int j = 0; j < arr.Count(); j++)
                    {
                        if (!string.IsNullOrEmpty(arr[j]))
                        {
                            DataUser[] uData2 = webUser.GetDatas(config.PERIOD, arr[j], "Id,Party,Committee,Committee2,Subsector,StreetTeam");
                            if (uData2 != null)
                            {
                                //strUserIds += "," + uData2[0].Id.ToString();
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
                    data[i].Party = strParty.Trim(',');
                    data[i].Committee = strCommittee.Trim(',');
                    data[i].Subsector = strSubsector.Trim(',');
                    data[i].StreetTeam = strStreetTeam.Trim(',');
                }
                else
                {
                    data[i].SubManType = "团体";
                }
                DataOpinionPop qData = new DataOpinionPop();
                qData.OpNum = data[i].OpNum;
                DataOpinionPop[] ckData = webPop.GetDatas(qData, "Id");//重复检查
                if (ckData != null)
                {
                    data[i].Id = ckData[0].Id;
                    if (webPop.Update(data[i]) > 0)
                    {
                        data[i].rowClass = " class='save'";
                    }
                    else
                    {
                        data[i].rowClass = " class='cancel'";
                        data[i].Id = 0;
                    }
                }
                else
                {//没有查询到，直接导入操作
                    data[i].Id = webPop.Insert(data[i]);
                    if (data[i].Id <= 0)
                    {
                        data[i].rowClass = " class='cancel'";
                    }
                }
                //if (!string.IsNullOrEmpty(strUserIds))
                //{
                //    string[] arr = strUserIds.Trim(',').Split(',');
                //    for (int j = 0; j < arr.Count(); j++)
                //    {
                //        int UserId = Convert.ToInt32(arr[j]);
                //        //PublicMod.AddScore(UserId, TitleProperty, ScoreProperty, TableName, data[i].Id, strIp, strUser);//提交
                //    }
                //}
                new admin.opinion_pop().AddScore(data[i], strIp, strUser);
            }

            plPopData.Visible = true;
            rpPopList.DataSource = data;
            rpPopList.DataBind();
        }
        #endregion
        //
        //获取数据表
        private DataTable getTable(string strFile)
        {
            string xlsFilePath = HttpContext.Current.Server.MapPath(strFile);
            //源的定义
            string strConn = "";
            if (strFile.IndexOf(".xlsx") > 0 && strFile.EndsWith("xlsx"))
            {
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + xlsFilePath + "';Extended Properties='Excel 12.0;HDR=NO;IMEX=1'";//HDR=YES
            }
            else if (strFile.IndexOf(".xls") > 0 && strFile.EndsWith("xls"))
            {
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + xlsFilePath + ";" + "Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
            }
            else
            {
                return null;
            }
            //string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + excFilePath + ";" + "Extended Properties=Excel 8.0;";
            //连接数据源  
            System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn);
            conn.Open();
            //对于EXCEL中的表即sheet([sheet1$])如果不是固定的可以使用下面的方法得到
            System.Data.DataTable schemaTable = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
            string tableName = schemaTable.Rows[0][2].ToString().Trim();
            //Sql语句  
            string strExcel = "select * from [" + tableName + "]";
            //适配到数据源  
            System.Data.OleDb.OleDbDataAdapter myCommand = new System.Data.OleDb.OleDbDataAdapter(strExcel, strConn);
            //定义存放的数据表  
            System.Data.DataSet ds = new System.Data.DataSet();
            myCommand.Fill(ds, tableName);
            //关闭数据源
            conn.Close();

            return ds.Tables[tableName];
        }
        //
        #region 生成测试数据
        //调研报告
        protected void btnTestReport_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            WebReport webReport = new WebReport();
            DataReport[] qData = webReport.GetDatas(new DataReport(), "Id", 1, 1, "", "total");
            int n = 0;
            if (qData != null)
            {
                n = qData[0].total;
            }
            int m = 0;
            string strIp = HelperMain.GetIpPort();
            for (int i = 0; i < 103; i++)
            {
                m = i + 1;
                DateTime dtNow = DateTime.Now;
                DataReport data = new DataReport();
                data.OrgName = "专委会-提案委员会";
                data.IsPoint = "否";
                data.SubMan = "测试员";
                data.Title = "调研报告测试" + (n + m).ToString();
                data.Body = "测试内容" + (n + m).ToString("d7");
                data.Files = "../upload/test/test" + (n + m).ToString("d7") + ".jpg";
                data.ActiveName = "暂存";
                data.UserId = 277;
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = "测试员";
                data.SubTime = dtNow;
                data.SubIp = strIp;
                data.Id = webReport.Insert(data);
            }
            ltTest.Text = (m > 0) ? "已生成" + m.ToString() + "条数据！" : "";
        }
        //提案
        protected void btnTestOpinion_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            WebOpinion webOpin = new WebOpinion();
            DataOpinion[] qData = webOpin.GetDatas(new DataOpinion(), "Id", 1, 1, "", "total");
            int n = 0;
            if (qData != null)
            {
                n = qData[0].total;
            }
            int m = 0;
            string strIp = HelperMain.GetIpPort();
            for (int i = 0; i < 1000000; i++)
            {
                m = i + 1;
                DateTime dtNow = DateTime.Now;
                DataOpinion data = new DataOpinion();
                data.OpNo = (n + m).ToString("d7");
                data.Period = "十四";
                data.Times = "一";
                data.SubType = "其他";
                data.SubManType = "委员";
                data.SubMan = ",测试员,";
                data.Summary = "提案测试" + (n + m).ToString();
                data.Body = "测试内容" + (n + m).ToString("d7");
                data.SubTime = dtNow;
                data.SubIp = strIp;
                data.ActiveName = "暂存";
                data.UserId = 277;
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = "测试员";
                data.Id = webOpin.Insert(data);
            }
            ltTest.Text = (m > 0) ? "已生成" + m.ToString() + "条数据！" : "";
        }
        //社情民意
        protected void btnTestPop_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            WebOpinionPop webPop = new WebOpinionPop();
            DataOpinionPop[] qData = webPop.GetDatas(new DataOpinionPop(), "Id", 1, 1, "", "total");
            int n = 0;
            if (qData != null)
            {
                n = qData[0].total;
            }
            int m = 0;
            string strIp = HelperMain.GetIpPort();
            for (int i = 0; i < 1000000; i++)
            {
                m = i + 1;
                DateTime dtNow = DateTime.Now;
                DataOpinionPop data = new DataOpinionPop();
                data.SubType = "其他";
                data.SubMan = "测试员";
                data.Summary = "社情民意测试" + (n + m).ToString();
                data.Body = "测试内容" + (n + m).ToString("d7");
                data.SubTime = dtNow;
                data.SubIp = strIp;
                data.ActiveName = "暂存";
                data.UserId = 277;
                data.AddTime = dtNow;
                data.AddIp = strIp;
                data.AddUser = "测试员";
                data.Id = webPop.Insert(data);
            }
            ltTest.Text = (m > 0) ? "已生成" + m.ToString() + "条数据！" : "";
        }
        //图片
        protected void btnTestPic_Click(object sender, EventArgs e)
        {
            if (myUser == null)
            {
                return;
            }
            string strFile = Server.MapPath("../upload/test/test.jpg");
            for (int i = 1; i <= 1000000; i++)
            {
                string strNew = "test" + i.ToString("d7") + ".jpg";
                strNew = strFile.Replace("test.jpg", strNew);
                if (!System.IO.File.Exists(strNew))
                {
                    System.IO.File.Copy(strFile, strNew);
                }
            }
        }
        #endregion
        //
    }
}