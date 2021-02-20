<%@ webhandler Language="C#" class="Upload" %>
using System;
using System.Web;
using mod.main;

public class Upload : IHttpHandler
{
	public void ProcessRequest(HttpContext context)
    {
        if (context.Request.RequestType == "POST")
        {
            loadPost(context);
        }
    }
    //
    public bool IsReusable
    {
        get
        {
            return true;
        }
    }

    //处理保存上传图片
    private void loadPost(HttpContext context)
    {
        try
        {//{"errno": 0, "data": ["http://www.quyou.com/images/logo.jpg", "../images/logo_qyw-sh.jpg"]}
            string imgFiles = "";
            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                HttpPostedFile imgFile = context.Request.Files[i];
                string fileName = imgFile.FileName;
                string fileExt = System.IO.Path.GetExtension(fileName).ToLower();
                DateTime dtNow = DateTime.Now;
                string newName = dtNow.ToString("yyyyMMddHHmmssffff");//Guid.NewGuid().ToString()
                string tmpPath = string.Format("../upload/image/{0:yyyyMMdd}/", dtNow);
                string dirPath = context.Server.MapPath(tmpPath);
                string filePath = tmpPath + newName + fileExt;
                string savePath = context.Server.MapPath(filePath);
                if (!System.IO.Directory.Exists(dirPath))
                {
                    System.IO.Directory.CreateDirectory(dirPath);
                }
                string fileMd5 = HelperSecret.GetStreamMd5(imgFile.InputStream);//文件MD5值
                if (!HelperSecret.CheckFileMd5(fileMd5))//验证文件MD5值合法性
                {
                    context.Response.Write("{\"errno\": 1, \"msg\": \"上传了非法MD5文件\"}");
                    return;
                }
                imgFile.SaveAs(savePath);//保存图片
                //System.Drawing.Image originalImage = System.Drawing.Image.FromStream(imgFile.InputStream);
                //if (imgFile.ContentLength > (2 * 1024 * 1024))
                //{//将图片大小限制为 2M
                //    qy.main.HelperImg.GetThumbnail(originalImage, 1200, 1200, "max").Save(savePath);//保存图片
                //}
                //else
                //{
                //    imgFile.SaveAs(savePath);//保存图片
                //}
                //if (originalImage.Width > 200 || originalImage.Height > 200)
                //{
                //    string filePath2 = tmpPath + newName + "-x" + fileExt;
                //    string savePath2 = context.Server.MapPath(filePath2);
                //    qy.main.HelperImg.GetThumbnail(originalImage, 200, 200, "max").Save(savePath2);
                //    filePath = filePath2;
                //}
                if (i > 0)
                {
                    imgFiles += ", ";
                }
                //imgFiles += string.Format("\"{0}\"", filePath.Replace("../../", "../"));
                imgFiles += "\"" + filePath + "\"";
            }
            context.Response.Write("{\"errno\": 0, \"data\": [" + imgFiles + "]}");
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"errno\": 1, \"msg\": \"" + ex.ToString() + "\"}");
        }
    }
    //验证非法MD5文件
    
    //
}