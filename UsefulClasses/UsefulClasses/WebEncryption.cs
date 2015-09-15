using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Useful_Classes
{
    //public class WebEncryption
    //{
    //    private string EncrSalt = string.Empty;

    //    public string Salt
    //    {
    //        get
    //        {
    //            if (EncrSalt == string.Empty)
    //            {
    //                StringManipulation strMani = new StringManipulation();
    //                EncrSalt = strMani.GenerateRandomString();
    //            }
    //            return EncrSalt;
    //        }
    //        set
    //        {
    //            EncrSalt = value;
    //        }
    //    }

    //    protected string encryptStr(String str, String salt, bool useHashing)
    //    {

    //        if (EncrSalt == string.Empty)
    //            return string.Empty;

    //        byte[] keyArray;
    //        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(str);

    //        string key = salt;

    //        //System.Windows.Forms.MessageBox.Show(key);
    //        //If hashing use get hashcode regards to your key
    //        if (useHashing)
    //        {
    //            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
    //            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
    //            //Always release the resources and flush data
    //            // of the Cryptographic service provide. Best Practice

    //            hashmd5.Clear();
    //        }
    //        else
    //            keyArray = UTF8Encoding.UTF8.GetBytes(key);

    //        TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
    //        //set the secret key for the tripleDES algorithm
    //        tdes.Key = keyArray;
    //        //mode of operation. there are other 4 modes.
    //        //We choose ECB(Electronic code Book)
    //        tdes.Mode = CipherMode.ECB;
    //        //padding mode(if any extra byte added)

    //        tdes.Padding = PaddingMode.PKCS7;

    //        ICryptoTransform cTransform = tdes.CreateEncryptor();
    //        //transform the specified region of bytes array to resultArray
    //        byte[] resultArray =
    //          cTransform.TransformFinalBlock(toEncryptArray, 0,
    //          toEncryptArray.Length);
    //        //Release resources held by TripleDes Encryptor
    //        tdes.Clear();
    //        //Return the encrypted data into unreadable string format
    //        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    //    }

    //    protected string decryptStr(String str, String salt, bool useHashing)
    //    {
    //        if (EncrSalt == string.Empty)
    //            return string.Empty;

    //        byte[] keyArray;
    //        //get the byte code of the string

    //        byte[] toEncryptArray = Convert.FromBase64String(str);

    //        string key = salt;

    //        if (useHashing)
    //        {
    //            //if hashing was used get the hash code with regards to your key
    //            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
    //            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
    //            //release any resource held by the MD5CryptoServiceProvider

    //            hashmd5.Clear();
    //        }
    //        else
    //        {
    //            //if hashing was not implemented get the byte code of the key
    //            keyArray = UTF8Encoding.UTF8.GetBytes(key);
    //        }

    //        TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
    //        //set the secret key for the tripleDES algorithm
    //        tdes.Key = keyArray;
    //        //mode of operation. there are other 4 modes. 
    //        //We choose ECB(Electronic code Book)

    //        tdes.Mode = CipherMode.ECB;
    //        //padding mode(if any extra byte added)
    //        tdes.Padding = PaddingMode.PKCS7;

    //        ICryptoTransform cTransform = tdes.CreateDecryptor();
    //        byte[] resultArray = cTransform.TransformFinalBlock(
    //                             toEncryptArray, 0, toEncryptArray.Length);
    //        //Release resources held by TripleDes Encryptor                
    //        tdes.Clear();
    //        //return the Clear decrypted TEXT
    //        return UTF8Encoding.UTF8.GetString(resultArray);
    //    }

    //    public string Encrypt(string stringToEncrypt)
    //    {
    //        if (!Licencing.validate())
    //            return null;

    //        String salt = encryptStr(stringToEncrypt, EncrSalt, true);
    //        return encryptStr(stringToEncrypt, salt, true);
    //    }

    //    public string Decrypt(string stringToDecrypt)
    //    {
    //        if (!Licencing.validate())
    //            return null;

    //        String salt = encryptStr(stringToDecrypt, EncrSalt, true);
    //        return decryptStr(stringToDecrypt, salt, true);
    //    }
    //}

    public static class WebEncryption
    {
        private static string encryptStr(String str, String salt, bool useHashing)
        {
            if (salt == string.Empty)
                return string.Empty;

            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(str);

            string key = salt;

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
            if (salt == string.Empty)
                return string.Empty;

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

        public static string Encrypt(string StringToEncrypt, string Salt)
        {
            if (!Licencing.validate())
                return null;

            String salt = encryptStr(StringToEncrypt, Salt, true);
            return encryptStr(StringToEncrypt, Salt, true);
        }

        public static string Decrypt(string StringToDecrypt, string Salt)
        {
            if (!Licencing.validate())
                return null;

            String salt = encryptStr(StringToDecrypt, Salt, true);
            return decryptStr(StringToDecrypt, Salt, true);
        }
    }
}
