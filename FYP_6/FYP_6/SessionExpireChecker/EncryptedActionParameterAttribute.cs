using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.IO;
using System.Web.Routing;
using System.Text;
using System.Web.Security;

namespace FYP_6.SessionExpireChecker
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EncryptedActionParameterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Dictionary<string, object> decryptedParameters = new Dictionary<string, object>();
            if (HttpContext.Current.Request.QueryString.Get("id") != null)
            {
                string encryptedQueryString = HttpContext.Current.Request.QueryString.Get("id");
                string decrptedString = Decrypt(encryptedQueryString.ToString());
                string[] paramsArrs = decrptedString.Split('?');

                for (int i = 0; i < paramsArrs.Length; i++)
                {
                    string[] paramArr = paramsArrs[i].Split('=');
                    decryptedParameters.Add(paramArr[0], Convert.ToInt32(paramArr[1]));
                }
            }
            else
            {
                HttpContext.Current.Session.Clear();
                FormsAuthentication.SignOut();
                filterContext.Result =
               new RedirectToRouteResult(new RouteValueDictionary   
	            {  
	             { "action", "login" },  
	            { "controller", "Home" },  
                //{ "returnUrl", filterContext.HttpContext.Request.RawUrl}  
	             });
                return;  
            }
            for (int i = 0; i < decryptedParameters.Count; i++)
            {
                filterContext.ActionParameters[decryptedParameters.Keys.ElementAt(i)] = decryptedParameters.Values.ElementAt(i);
            }
            base.OnActionExecuting(filterContext);

        }

        private string Decrypt(string encryptedText)
        {
            try
            {
                string key = "jdsg432387#";
                byte[] DecryptKey = { };
                byte[] IV = { 55, 34, 87, 64, 87, 195, 54, 21 };
                byte[] inputByte = new byte[encryptedText.Length];
                DecryptKey = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByte = Convert.FromBase64String(encryptedText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(DecryptKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByte, 0, inputByte.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception)
            {
                string key = "jdsg432387#";
                string encText = "8tvmcY9BKY4=";
                byte[] DecryptKey = { };
                byte[] IV = { 55, 34, 87, 64, 87, 195, 54, 21 };
                byte[] inputByte = new byte[encText.Length];
                DecryptKey = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByte = Convert.FromBase64String(encText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(DecryptKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByte, 0, inputByte.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
        }
    }
}
