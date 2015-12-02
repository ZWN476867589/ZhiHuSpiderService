using System.IO;
using System.Net;
using System.Xml.Linq;
using System;

namespace ZhiHuSpider.Business
{
    public static class BusinessUtils
    {
        static string cookiesXmlPath = System.Environment.CurrentDirectory + @"\ZhiHuCookies.xml";
        static string host = "www.zhihu.com";
        static CookieCollection CookieCollect
        {
            get
            {
                if (cookiecollect == null)
                {
                    LoadCookiesFormXml();
                }
                return cookiecollect;
            }
        }
        private static CookieCollection cookiecollect;
        public static HttpWebRequest WebRequest;
        public static HttpWebResponse WebResponse;
        public static void InitwebResques(string Url)
        {
            lock (host)
            {
                WebRequest = (HttpWebRequest)HttpWebRequest.Create(Url);
                WebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                WebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36";
                WebRequest.CookieContainer = new System.Net.CookieContainer();
                WebRequest.CookieContainer.Add(CookieCollect);
                WebRequest.Host = host;
                WebRequest.Method = "GET";
                WebRequest.KeepAlive = true;
            }
        }
        public static CookieCollection LoadCookiesFormXml()
        {
            cookiecollect = new System.Net.CookieCollection();
            if (File.Exists(cookiesXmlPath))
            {
                XElement doc = XElement.Load(cookiesXmlPath);
                if (doc != null)
                {
                    foreach (var i in doc.Elements())
                    {
                        Cookie cookie = new System.Net.Cookie(i.Attribute("name").Value, i.Value);
                        cookie.Domain = "zhihu.com";
                        cookiecollect.Add(cookie);
                    }
                }
            }
            return cookiecollect;
        }
        public static string GetByUrl(string Url)
        {
            string result = "";
            try
            {
                InitwebResques(Url);
                WebResponse = (HttpWebResponse)WebRequest.GetResponse();
                Stream st = WebResponse.GetResponseStream();
                StreamReader str = new StreamReader(st);
                result = str.ReadToEnd();
            }
            catch (Exception ex)
            {
            }
            return result;
        }
    }
}
