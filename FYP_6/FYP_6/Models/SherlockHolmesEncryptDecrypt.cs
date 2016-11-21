using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Transactions;
using System.Text;
using System.Web;

namespace FYP_6.Models
{
    public static class SherlockHolmesEncryptDecrypt
    {
        public static TripleDES CreateDES(string key)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            TripleDES des = new TripleDESCryptoServiceProvider();
            des.Key = md5.ComputeHash(Encoding.Unicode.GetBytes(key));
            des.IV = new byte[des.BlockSize / 8];
            return des;
        }

        public static string Encrypt(string plainText, string pass = "asd120xc6vb2gh")
        {
            using (TransactionScope t=new TransactionScope())
            {
                try
                {
                    byte[] plainBytes = Encoding.Unicode.GetBytes(plainText);

                    MemoryStream m = new MemoryStream();
                    TripleDES des = CreateDES(pass);

                    CryptoStream cryptostream = new CryptoStream(m, des.CreateEncryptor(), CryptoStreamMode.Write);

                    cryptostream.Write(plainBytes, 0, plainBytes.Length);
                    cryptostream.FlushFinalBlock();
                    t.Complete();
                    return Convert.ToBase64String(m.ToArray());
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        public static string Decrypt(string encryptedText, string pass = "asd120xc6vb2gh")
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    byte[] plainBytes = Convert.FromBase64String(encryptedText);

                    MemoryStream m = new MemoryStream();
                    TripleDES des = CreateDES(pass);

                    CryptoStream cryptostream = new CryptoStream(m, des.CreateDecryptor(), CryptoStreamMode.Write);

                    cryptostream.Write(plainBytes, 0, plainBytes.Length);
                    cryptostream.FlushFinalBlock();
                    t.Complete();
                    return Encoding.Unicode.GetString(m.ToArray());
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        #region Old Encryption Code
        //public static string Encrypt(string inText, string key="asdfghzxcvbnm")
        //{
        //    using (TransactionScope t=new TransactionScope())
        //    {
        //        try
        //        {
        //            byte[] bytesBuff = Encoding.Unicode.GetBytes(inText);
        //            using (Aes aes = Aes.Create())
        //            {
        //                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        //                aes.Key = crypto.GetBytes(32);
        //                aes.IV = crypto.GetBytes(16);
        //                using (MemoryStream mStream = new MemoryStream())
        //                {
        //                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
        //                    {
        //                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
        //                        cStream.Close();
        //                    }
        //                    inText = Convert.ToBase64String(mStream.ToArray());
        //                }
        //            }
        //            t.Complete();
        //            return inText;
        //        }
        //        catch (Exception)
        //        {
        //            return null;
        //        }
        //    }
            
        //}
        ////Decrypting a string
        //public static string Decrypt(string cryptTxt, string key = "asdfghzxcvbnm")
        //{
        //    using (TransactionScope t=new TransactionScope())
        //    {
        //        try
        //        {
        //            cryptTxt = cryptTxt.Replace(" ", "+");
        //            byte[] bytesBuff = Convert.FromBase64String(cryptTxt);
        //            using (Aes aes = Aes.Create())
        //            {
        //                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        //                aes.Key = crypto.GetBytes(32);
        //                aes.IV = crypto.GetBytes(16);
        //                using (MemoryStream mStream = new MemoryStream())
        //                {
        //                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
        //                    {
        //                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
        //                        cStream.Close();
        //                    }
        //                    cryptTxt = Encoding.Unicode.GetString(mStream.ToArray());
        //                }
        //            }
        //            t.Complete();
        //            return cryptTxt;
        //        }
        //        catch (Exception)
        //        {
        //            return null;
        //        }
        //    }
        //}
        #endregion
    }
}