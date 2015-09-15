using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace Useful_Classes
{
    public static class ImageController
    {
        public static System.Drawing.Image ResizeMyImage(System.Drawing.Image image, Size newSize)
        {
            if (!Licencing.validate())
                return null;

            Bitmap newImage = new Bitmap(newSize.Width, newSize.Height);
            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(image, new Rectangle(0, 0, newSize.Width, newSize.Height));
            }

            return newImage;
        }

        public static byte[] ResizeMyImage(byte[] image, Size newSize)
        {
            if (!Licencing.validate())
                return null;

            Bitmap newImage = new Bitmap(newSize.Width, newSize.Height);
            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(GetImageFromByteArray(image), new Rectangle(0, 0, newSize.Width, newSize.Height));
            }

            return GetByteArrayFromImage(newImage);
        }

        public static byte[] GetByteArrayFromImage(System.Drawing.Image image)
        {
            if (!Licencing.validate())
                return null;

            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        public static System.Drawing.Image GetImageFromByteArray(byte[] image)
        {
            if (!Licencing.validate())
                return null;

            MemoryStream ms = new MemoryStream(image);
            Image returnImage = Bitmap.FromStream(ms);
            return returnImage;
        }

        public static System.Drawing.Image PerformASPImageUpload(HttpPostedFile file, HttpServerUtility server, string folder)
        {
            if (!Licencing.validate())
                return null;

            string FileName = file.FileName;
            string saveFile = server.MapPath(folder + ((folder.Replace("/", "\\").EndsWith("\\")) ? "" : "\\") + Useful_Classes.StringManipulation.GenerateRandomString(25) + "." + FileName.Split('.')[FileName.Split('.').Length - 1]);

            if (validateFolderExistance(saveFile))
            {
                file.SaveAs(saveFile);
                System.Drawing.Image IMG = System.Drawing.Image.FromFile(saveFile);

                try
                {
                    File.Delete(saveFile);
                }
                catch { }

                return IMG;
            }

            return null;
        }
        
        public static string PerformASPFileUpload(HttpPostedFile file, HttpServerUtility server, string folder)
        {
            if (!Licencing.validate())
                return string.Empty;

            string FileName = file.FileName;
            string newFIleName = Useful_Classes.StringManipulation.GenerateRandomString(25) + "." + FileName.Split('.')[FileName.Split('.').Length - 1];
            string saveFile = server.MapPath(folder + ((folder.Replace("/", "\\").EndsWith("\\")) ? "" : "\\") + newFIleName);

            if (validateFolderExistance(saveFile))
            {
                file.SaveAs(saveFile);

                return newFIleName;
            }

            return string.Empty;
        }

        private static bool validateFolderExistance(string file)
        {
            try
            {
                string newFile = file.Split('\\')[0] + "\\";

                for (int i = 1; i < file.Split('\\').Length - 1; i++)
                {
                    newFile += file.Split('\\')[i] + "\\";

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
