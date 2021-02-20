using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LitJson;
using mod.main;
using hkzx.db;
using hkzx.user;

namespace hkzx.web.cn
{
    public partial class upload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.RequestType == "POST")
            {
                saveFile();
            }
        }
        //处理保存上传文件
        private void saveFile()
        {
            string strFiles = "";
            WebFiles webFiles = new WebFiles();
            int intUserId = 0;
            string strUserName = "";
            if (Request.QueryString["ac"] == "admin")
            {
                DataAdmin myAdmin = HelperAdmin.GetUser();
                if (myAdmin != null)
                {
                    intUserId = -myAdmin.Id;
                    strUserName = myAdmin.AdminName;
                }
            }
            else
            {
                DataUser myUser = HelperUser.GetUser();
                if (myUser != null)
                {
                    intUserId = myUser.Id;
                    strUserName = myUser.TrueName;
                }
            }
            string strPath = "../upload/";//保存文件的目录
            switch (Request.QueryString["type"])
            {
                case "photo":
                    strPath += "photo/";
                    break;
                case "img":
                    strPath += "image/" + DateTime.Today.ToString("yyyyMMdd/");
                    break;
                case "video":
                    strPath += "video/" + DateTime.Today.ToString("yyyyMMdd/");
                    break;
                default:
                    strPath += "file/" + DateTime.Today.ToString("yyyyMMdd/");
                    break;
            }
            string strFolder = Server.MapPath(strPath);//保存文件的物理目录
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFile objFile = Request.Files[i];//获取文件流

                string strFileMd5 = HelperSecret.GetStreamMd5(objFile.InputStream);//文件MD5值
                if (!HelperSecret.CheckFileMd5(strFileMd5))//验证文件MD5值合法性
                {
                    outTxt("上传了非法MD5文件", 1);
                    //Response.Write("{\"errno\": 1, \"msg\": \"上传了非法MD5文件\"}");
                    //Response.End();
                }

                string strFile = objFile.FileName;
                string strFileExt = Path.GetExtension(strFile);//取文件扩展名
                string strFileName = strFile;
                if (strFile.IndexOf(strFileExt) > 0)
                {
                    strFileName = strFile.Substring(0, strFile.LastIndexOf(strFileExt));//取文件名（不带扩展名）
                }
                else if (strFile.IndexOf(".") > 0)
                {
                    strFileName = strFile.Substring(0, strFile.LastIndexOf("."));//取文件名（不带扩展名）
                }
                strFileExt = strFileExt.ToLower();
                string strNewName = string.Format("{0:yyyyMMddHHmmssfff}{1}", DateTime.Now, i);//Guid.NewGuid().ToString()//生成新文件名
                string strFileUrl = strPath + strNewName + strFileExt;//最后的文件链接

                DataFiles[] dFile = webFiles.GetData(strFileMd5);//比对数据库中是否已上传该文件
                //outTxt(strFileMd5, 1);
                if (dFile != null)
                {
                    //outTxt(dFile[0].Url, 1);
                    //Response.Write("{\"errno\": 1, \"msg\": \"" + dFile[0].Url + "\"}"); Response.End();
                    strFileUrl = dFile[0].Url;
                    dFile[0].UpTime = DateTime.Now;
                    dFile[0].UpIp = HelperMain.GetIpPort();
                    dFile[0].UpUser = strUserName;
                    webFiles.Update(dFile[0]);
                }
                else
                {
                    try
                    {
                        if (!Directory.Exists(strFolder))
                        {
                            Directory.CreateDirectory(strFolder);//新建保存文件的目录
                        }
                        string strSaveFile = Server.MapPath(strFileUrl);//保存文件的物理路径
                        //if (strFileExt == ".jpg" || strFileExt == ".gif" || strFileExt == ".png" || strFileExt == ".jpeg")
                        //{
                        //    System.Drawing.Image originalImage = System.Drawing.Image.FromStream(objFile.InputStream);
                        //    if (objFile.ContentLength > (2 * 1024 * 1024))
                        //    {//将图片大小限制为 2M
                        //        HelperImg.GetThumbnail(originalImage, 1200, 1200, "max").Save(strSaveFile);//保存图片
                        //    }
                        //    else
                        //    {
                        //        objFile.SaveAs(strSaveFile);//保存图片
                        //    }
                        //    //if (originalImage.Width > 200 || originalImage.Height > 200)
                        //    //{
                        //    //    string strImgUrl = strTmpPath + strNewName + "-x" + strFileExt;
                        //    //    string savePath2 = Server.MapPath(strImgUrl);
                        //    //    HelperImg.GetThumbnail(originalImage, 200, 200, "max").Save(savePath2);
                        //    //    strFileUrl = strImgUrl;
                        //    //}
                        //}
                        //else
                        //{
                        //    objFile.SaveAs(strSaveFile);//保存文件
                        //}
                        objFile.SaveAs(strSaveFile);//保存文件
                    }
                    catch (Exception ex)
                    {
                        outTxt(ex.ToString(), 1);
                        //Response.Write("{\"errno\": 1, \"msg\": \"" + ex.ToString() + "\"}");
                        //Response.End();
                    }
                    DataFiles data = new DataFiles();
                    data.Title = strFileName;
                    data.Url = strFileUrl;
                    data.MD5 = strFileMd5;
                    data.Active = 1;
                    data.UserId = intUserId;
                    data.AddTime = DateTime.Now;
                    data.AddIp = HelperMain.GetIpPort();
                    data.AddUser = strUserName;
                    webFiles.Insert(data);//新文件，插入数据值
                }
                if (i > 0)
                {
                    strFiles += ", ";
                }
                strFiles += "\"" + strFileUrl + "\"";
            }
            //Response.Write("{\"errno\": 0, \"data\": [" + strFiles + "]}");
            outTxt(strFiles);
            Response.End();
        }
        //返回处理结果
        private void outTxt(string msg, int errno = 0)
        {
            Response.ClearContent();
            //Response.Charset = "utf-8";
            //Response.ContentType = "text/html";
            Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
            if (Request.QueryString["editor"] == "kind")
            {
                System.Collections.Hashtable hash = new System.Collections.Hashtable();
                if (errno == 0)
                {
                    hash["error"] = 0;
                    hash["url"] = msg.Replace("\"", "");
                }
                else
                {
                    hash["error"] = 1;
                    hash["message"] = msg;
                }
                Response.Write(JsonMapper.ToJson(hash));
            }
            else
            {
                string strOut = "";
                if (errno == 0)
                {
                    strOut = "{\"errno\": 0, \"data\": [" + msg + "]}";
                }
                else
                {
                    strOut = "{\"errno\": 1, \"msg\": \"" + msg + "\"}";
                }
                Response.Write(strOut);
            }
            Response.End();
        }
        //
    }
}