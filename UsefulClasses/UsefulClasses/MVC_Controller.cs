using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Useful_Classes
{
    public static class MVC_Controller
    {

        public static bool validateFileIsUploadable(string[] files)
        {
            if (!Licencing.validate())
                return false;

            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    if (file.Length < 5)
                        return false;
                }

                return true;
            }
            return false;
        }

        public static Image GetUploadedImage(HttpPostedFileBase uploadedFile, string saveFilePath)
        {
            Image IMG = null;

            try
            {
                string filename = uploadedFile.FileName;

                validateFolderExistance(filename);

                string savePath = Environment.CurrentDirectory + "\\" + ((saveFilePath.Length > 0) ? saveFilePath + ((saveFilePath.Replace("/", "\\").EndsWith("\\")) ? "" : "\\") : "") + Useful_Classes.StringManipulation.GenerateRandomString(27) + "." + filename.Split('.')[filename.Split('.').Length - 1];

                if (uploadedFile.ContentLength > 0)
                {
                    uploadedFile.SaveAs(savePath);

                    IMG = System.Drawing.Image.FromFile(savePath);

                    try
                    {
                        File.Delete(savePath);
                    }
                    catch { }
                }
            }
            catch
            { }

            return IMG;
        }

        public static Image[] GetUploadedImages(string saveFilePath, HttpRequestBase request)
        {
            List<string> files = new List<string>();

            foreach (string file in request.Files)
            {
                files.Add(file);
            }

            return GetUploadedImages(files.ToArray(), saveFilePath, request).ToArray();
        }

        public static Image GetUploadedImage(string file, string saveFilePath, HttpRequestBase request)
        {
            return GetUploadedImages(new string[] { file }, saveFilePath, request).FirstOrDefault();
        }

        public static Image[] GetUploadedImages(string[] files, string saveFilePath, HttpRequestBase request)
        {
            List<Image> images = new List<Image>();

            if (!Licencing.validate())
                return null;

            if (validateFileIsUploadable(files))
            {
                for (int i = 0; i < files.Length; i++)
                {
                    string filename = files[i];

                    validateFolderExistance(filename);

                    string savePath = Environment.CurrentDirectory + "\\" + ((saveFilePath.Length > 0) ? saveFilePath + ((saveFilePath.Replace("/", "\\").EndsWith("\\")) ? "" : "\\") : "") + Useful_Classes.StringManipulation.GenerateRandomString(27) + "." + filename.Split('.')[filename.Split('.').Length - 1];

                    HttpPostedFileBase file = request.Files[i];

                    if (file.ContentLength > 0)
                    {
                        file.SaveAs(savePath);

                        System.Drawing.Image IMG = System.Drawing.Image.FromFile(savePath);

                        images.Add(IMG);
                        try
                        {
                            File.Delete(savePath);
                        }
                        catch { }
                    }
                }
            }
            return images.ToArray();
        }

        public static byte[] GetUploadedImageBytes(HttpPostedFileBase uploadedFile, string saveFilePath)
        {
            byte[] IMG = null;

            try
            {
                string filename = uploadedFile.FileName;

                validateFolderExistance(filename);

                string savePath = Environment.CurrentDirectory + "\\" + ((saveFilePath.Length > 0) ? saveFilePath + ((saveFilePath.Replace("/", "\\").EndsWith("\\")) ? "" : "\\") : "") + Useful_Classes.StringManipulation.GenerateRandomString(27) + "." + filename.Split('.')[filename.Split('.').Length - 1];

                if (uploadedFile.ContentLength > 0)
                {
                    uploadedFile.SaveAs(savePath);

                    byte[] bytes = null;
                    FileStream fs = null;
                    try
                    {
                        fs = File.OpenRead(savePath);
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
                        }
                        catch { }
                    }

                    IMG = bytes;
                    try
                    {
                        File.Delete(savePath);
                    }
                    catch { }
                }
            }
            catch
            { }

            return IMG;
        }

        public static byte[][] GetUploadedImagesBytes(string saveFilePath, HttpRequestBase request)
        {
            List<string> files = new List<string>();

            foreach (string file in request.Files)
            {
                files.Add(file);
            }

            return GetUploadedImagesBytes(files.ToArray(), saveFilePath, request).ToArray();
        }

        public static byte[] GetUploadedImageBytes(string file, string saveFilePath, HttpRequestBase request)
        {
            return GetUploadedImagesBytes(new string[] { file }, saveFilePath, request).FirstOrDefault();
        }

        public static byte[][] GetUploadedImagesBytes(string[] files, string saveFilePath, HttpRequestBase request)
        {
            List<byte[]> images = new List<byte[]>();

            if (!Licencing.validate())
                return null;

            if (validateFileIsUploadable(files))
            {
                for (int i = 0; i < files.Length; i++)
                {
                    string filename = files[i];

                    validateFolderExistance(filename);

                    string savePath = Environment.CurrentDirectory + "\\" + ((saveFilePath.Length > 0) ? saveFilePath + ((saveFilePath.Replace("/", "\\").EndsWith("\\")) ? "" : "\\") : "") + Useful_Classes.StringManipulation.GenerateRandomString(27) + "." + filename.Split('.')[filename.Split('.').Length - 1];

                    HttpPostedFileBase file = request.Files[i];

                    if (file.ContentLength > 0)
                    {
                        file.SaveAs(savePath);

                        byte[] bytes = null;
                        FileStream fs = null;
                        try
                        {
                            fs = File.OpenRead(savePath);
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
                            }
                            catch { }
                        }

                        images.Add(bytes);
                        try
                        {
                            File.Delete(savePath);
                        }
                        catch { }
                    }
                }
            }
            return images.ToArray();
        }

        private static bool validateFolderExistance(string file)
        {
            try
            {
                string newFile = file.Split('\\')[0] + "\\";

                for (int i = 1; i < file.Split('\\').Length - 1; i++)
                {
                    newFile += file.Split('\\')[i];

                    if (!Directory.Exists(newFile))
                        Directory.CreateDirectory(newFile);
                }

                return true;
            }
            catch
            { }

            return false;
        }
    }
}
