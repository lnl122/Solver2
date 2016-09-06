using System.IO;
using System.Net;

namespace Solver2
{
    // public static string Logon(string url1, string name, string pass) - выход - страница после попытки авторизации
    //

    class Engine
    {
        private static string cHead;            // куки
        private static CookieContainer cCont;   // куки


        // получает страницу по урлу
        public static string get_page(string url)
        {
            string ps = "";
            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(url);
            //getRequest.Headers.Add("Accept-Language", "ru-ru,ru");
            //getRequest.Headers.Add("Content-Language", "ru-ru,ru");
            ////getRequest.Headers.Set("Accept-Charset", "utf-8");
            ////getRequest.Headers.Set("Accept-Encoding", "utf-8");
            ////getRequest.Headers.
            ////Accept-Charset
            ////Accept-Encoding
            //getRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1";
            try
            {
                getRequest.CookieContainer = cCont;
                WebResponse getResponse = getRequest.GetResponse();
                using (StreamReader sr = new StreamReader(getResponse.GetResponseStream()))
                {
                    ps = sr.ReadToEnd();
                }
            }
            catch
            {
                Log.Write("gpage ERROR: Не удалось прочитать страницу ", url);
                ps = "";
            }
            return ps.ToLower();
        }
        
        // выполняем логон в движке
        // вход - урл, логин, пасс
        // выход - страница с ответом
        public static string Logon(string url1, string name, string pass)
        {
            string formParams = string.Format("Login={0}&Password={1}", name, pass);
            string cookieHeader = "";
            CookieContainer cookies = new CookieContainer();
            cCont = cookies;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url1);
            req.CookieContainer = cookies;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(formParams);
            req.ContentLength = bytes.Length;
            using (Stream os = req.GetRequestStream()) { os.Write(bytes, 0, bytes.Length); }
            string pageSource = "";
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                cookieHeader = resp.Headers["Set-cookie"];
                cHead = cookieHeader;
                using (StreamReader sr = new StreamReader(resp.GetResponseStream())) { pageSource = sr.ReadToEnd(); }
            }
            catch
            {
                Log.Write("en.cx ERROR: не удалось получить ответ на авторизацию", url1 + " " + name + " " + pass);
            }
            return pageSource;
        }
    }
}
