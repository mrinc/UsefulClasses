using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Useful_Classes
{
    public static class CaptchaControl
    {
        private static string captcahSalt = "w2tygh@$WtyhbG^hy24ywERJYh3vbg46huerbghvy6$HUWBEJVH";
        public static CaptchaMessage GetCaptcha()
        {
            string captchaText = generateRandomString(5).ToUpper();

            CaptchaMessage captchaMsg = GenerateImage(captchaText, 100, 20, "Arial");

            return captchaMsg;
        }

        public static CaptchaResponce validateCaptcha(string captchaAnswer, string publicKey)
        {
            CaptchaResponce responce = new CaptchaResponce();
            try
            {
                string decrypted1 = decryptStr(publicKey.Replace("~A", "="), captcahSalt, true);

                DateTime encryptedDate = DateTime.Parse(decrypted1.Split(new string[] { "$##$" }, StringSplitOptions.None)[1]);

                if ((DateTime.Now - encryptedDate).TotalMinutes < 5)
                {
                    string captchaAnswerDecrypted = decryptStr(decrypted1.Split(new string[] { "$##$" }, StringSplitOptions.None)[0], captcahSalt, true);

                    if (captchaAnswer.ToUpper() == captchaAnswerDecrypted.ToUpper())
                    {
                        responce.Correct = true;
                        responce.Text = "Answer is correct";
                    }
                    else
                    {
                        responce.Correct = false;
                        responce.Text = "Captcha answer does not match captcha";
                    }
                }
                else
                {
                    responce.Correct = false;
                    responce.Text = "Captcha timeout occured";
                }
            }
            catch
            {
                responce.Correct = false;
                responce.Text = "Captcha is not valid";
            }

            return responce;
        }

        private static string encryptStr(String str, String Salt, bool useHashing)
        {
            if (Salt == string.Empty)
                return string.Empty;

            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(str);

            string key = Salt;

            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private static string decryptStr(String str, String salt, bool useHashing)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(str);

            string key = salt;

            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        private static CaptchaMessage GenerateImage(string text, int width, int height, string fontFamily)
        {
            Random random = new Random();

            // Create a new 32-bit bitmap image.
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            // Create a graphics object for drawing.
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, width, height);

            // Fill in the background.
            HatchBrush hatchBrush = new HatchBrush(HatchStyle.Wave, Color.LightGray, Color.White);
            g.FillRectangle(hatchBrush, rect);

            // Set up the text font.
            SizeF size;
            float fontSize = rect.Height + 1;
            Font font;
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            // Adjust the font size until the text fits within the image.
            do
            {
                fontSize--;
                font = new Font(fontFamily, fontSize, FontStyle.Bold);
                size = g.MeasureString(text, font, new SizeF(width, height), format);
            } while (size.Width > rect.Width);

            // Create a path using the text and warp it randomly.
            GraphicsPath path = new GraphicsPath();
            path.AddString(text, font.FontFamily, (int)font.Style, font.Size, rect, format);
            float v = 4F;
            PointF[] points =
			{
				new PointF(random.Next(rect.Width) / v, random.Next(rect.Height) / v),
				new PointF(rect.Width - random.Next(rect.Width) / v, random.Next(rect.Height) / v),
				new PointF(random.Next(rect.Width) / v, rect.Height - random.Next(rect.Height) / v),
				new PointF(rect.Width - random.Next(rect.Width) / v, rect.Height - random.Next(rect.Height) / v)
			};
            Matrix matrix = new Matrix();
            matrix.Translate(0F, 0F);
            path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);

            // Draw the text.
            hatchBrush = new HatchBrush(HatchStyle.DashedUpwardDiagonal, Color.DarkGray, Color.Black);
            g.FillPath(hatchBrush, path);

            // Add some random noise.
            int m = Math.Max(rect.Width, rect.Height);
            for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
            {
                int x = random.Next(rect.Width);
                int y = random.Next(rect.Height);
                int w = random.Next(m / 50);
                int h = random.Next(m / 50);
                g.FillEllipse(hatchBrush, x, y, w, h);
            }

            // Clean up.
            font.Dispose();
            hatchBrush.Dispose();
            g.Dispose();

            CaptchaMessage message = new CaptchaMessage();
            message.RequiredAnswer = text;
            message.CaptchaImage = bitmap;
            message.CaptchaHash = encryptStr(encryptStr(text, captcahSalt, true) + "$##$" + DateTime.Now.ToString(), captcahSalt, true).Replace("=", "~A");

            return message;
        }

        private static string generateRandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random(DateTime.Now.Millisecond);
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }

    public class CaptchaMessage
    {
        public string CaptchaHash { get; set; }

        public Bitmap CaptchaImage { get; set; }

        public string RequiredAnswer { get; set; }
    }

    public class CaptchaResponce
    {
        public bool Correct { get; set; }

        public string Text { get; set; }
    }
}
