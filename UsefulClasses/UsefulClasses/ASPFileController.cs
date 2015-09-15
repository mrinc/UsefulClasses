using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Useful_Classes
{
    public class ASPFileController
    {
        private string savePath = "/";
        private int maxFileSize = 4000;
        private string[] allowedExtensions = new string[] { "jpg", "png", "jpeg", "bmp" };

        public string SavePath
        {
            set
            {
                savePath = value;
                if (!savePath.EndsWith("/"))
                    savePath += "/";
            }
            get
            {
                return savePath;
            }
        }

        public bool checkFileToUpload(HttpPostedFile file)
        {
            if (!Licencing.validate())
                return false;

            if (file != null & savePath != string.Empty)
            {
                string FileName = file.FileName;
                if (allowedExtensions.Contains(FileName.Split('.')[FileName.Split('.').Length - 1]))
                {
                    if (file.ContentLength < maxFileSize)
                        return true;
                }
            }
            return false;
        }

        public Image GetUploadImage(HttpPostedFile file, Page page)
        {
            if (!Licencing.validate())
                return null;

            if (file != null & savePath != string.Empty)
            {
                string FileName = file.FileName;
                string saveFile = savePath + FileName;
                if (checkFileToUpload(file))
                {
                    file.SaveAs(saveFile);
                    System.Drawing.Image IMG = System.Drawing.Image.FromFile(saveFile);
                    //IMG = AddWaterMark(IMG, System.Drawing.Image.FromFile(saveFile));
                    //390;195
                    //IMG.Save(pge.Server.MapPath("~/Styling/Uploads/2" + FileName));
                    try
                    {
                        File.Delete(saveFile);
                    }
                    catch { }

                    return IMG;
                }
            }
            return null;
        }

        public byte[] GetUploadFileIgnoreLimits(HttpPostedFile file, Page page)
        {
            if (!Licencing.validate())
                return null;

            if (file != null & savePath != string.Empty)
            {
                string FileName = file.FileName;
                string saveFile = savePath + FileName;
                //if (checkFileToUpload(file))
                {
                    file.SaveAs(saveFile);
                    byte[] bytes = null;
                    FileStream fs = null;
                    try
                    {
                        fs = File.OpenRead(saveFile);
                        bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                    }
                    finally
                    {
                        try
                        {
                            if (fs != null)
                            {
                                fs.Close();
                                fs.Dispose();
                            }
                            File.Delete(saveFile);
                        }
                        catch { }
                    }

                    return bytes;
                }
            }
            return null;
        }

        public byte[] GetUploadFile(HttpPostedFile file, Page page)
        {
            if (!Licencing.validate())
                return null;

            if (file != null & savePath != string.Empty)
            {
                string FileName = file.FileName;
                string saveFile = savePath + FileName;
                if (checkFileToUpload(file))
                {
                    file.SaveAs(saveFile);
                    byte[] bytes = null;
                    FileStream fs = null;
                    try
                    {
                        fs = File.OpenRead(saveFile);
                        bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                    }
                    finally
                    {
                        try
                        {
                            if (fs != null)
                            {
                                fs.Close();
                                fs.Dispose();
                            }
                            File.Delete(saveFile);
                        }
                        catch { }
                    }

                    return bytes;
                }
            }
            return null;
        }

        public void UploadFile(HttpPostedFile file, Page page)
        {
            if (!Licencing.validate())
                return;

            if (file != null & savePath != string.Empty)
            {
                string FileName = file.FileName;
                string saveFile = savePath + FileName;
                if (checkFileToUpload(file))
                {
                    file.SaveAs(saveFile);
                }
            }
        }
    }
}
