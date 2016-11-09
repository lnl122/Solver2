// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System.IO;
using System.Net;

namespace Solver2
{
    class Engine
    {
        //
        // public static void SetId(string s1, string s2, string s3, string s4, string s5, int i1)  - устанавливаем параметры извне к себе
        // public static string GetPage(string url)                                                 - получает текст страницы по урлу
        // public static string Logon(string url1, string name, string pass)                        - выход - страница после попытки авторизации
        // public static string TryOne(int lvl, string val)                                         - пробуем вбить один ответ
        //

        private static string username = "";    // логин пользователя
        private static string password = "";    // пасс пользвоателя
        private static string userid = "";      // ид пользователя
        private static string gamedomain = "";  // домен игры
        private static string gameid = "";      // ид игры
        private static int levels = 0;          // колво уровней
        public static bool isReady = false;     // структура готова

        public static int lastlevel = -1;       // последний уровень, к которому было обращение
        public static string lastpage = "";     // последняя полученная страница

        public static string cHead;             // куки
        public static CookieContainer cCont;    // куки

        // установить полученные в форме параметры
        public static void SetId(string s1, string s2, string s3, string s4, string s5, int i1)
        {
            userid = s1;
            username = s2;
            password = s3;
            gameid = s4;
            gamedomain = s5;
            levels = i1;
            isReady = true;
        }

        // получает страницу по урлу
        // вход - урл
        // выход - текст страницы в нижнем регистре
        public static string GetPage(string url)
        {
            string ps = "";
            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(url);

            //getRequest.Headers.Add("Accept-Language", "ru-ru,ru");
            //getRequest.Headers.Add("Content-Language", "ru-ru,ru");
            //getRequest.Headers.Set("Accept-Charset", "utf-8");
            //getRequest.Headers.Set("Accept-Encoding", "utf-8");
            //getRequest.Headers.
            //Accept-Charset
            //Accept-Encoding
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
                Log.Write("en.cx ERROR: Не удалось прочитать страницу ", url);
                ps = "";
            }
            // если на странице встретили "<form ID=\"formMain\" method=\"post\" action=\"/Login.aspx?return=%2fgameengines%2fencounter%2fplay%2f24889%2f%3flevel%3d11"
            // надо переавторизоваться и, если успешно - вернуть страницу
            return ps;
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

        // пробует вбить один ответ
        // вход - уровень и слово
        // выход - страница от движка
        public static string TryOne(int lvl, string val)
        {
            if (lvl == 0)
            {
                return "";
            }
            val = val.Replace('ё','е');
            string url = "http://" + gamedomain + "/gameengines/encounter/play/" + gameid + "/?level=" + lvl.ToString();
            if (lvl != lastlevel)
            {
                lastlevel = lvl;
                lastpage = GetPage(url);
            }
            if ((lastpage == null) || (lastpage == ""))
            {
                lastlevel = lvl;
                lastpage = GetPage(url);
            }
            string t1 = lastpage;
            
            string t2 = t1;
            string tt1 = "name=\"LevelId\" value=\"";
            t1 = t1.Substring(t1.IndexOf(tt1) + tt1.Length);
            string LevelId = t1.Substring(0, t1.IndexOf("\""));
            string tt2 = "name=\"LevelNumber\" value=\"";
            t2 = t2.Substring(t2.IndexOf(tt2) + tt2.Length);
            string LevelNumber = t2.Substring(0, t2.IndexOf("\""));

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.Expect100Continue = false;
            req.Referer = url;
            req.KeepAlive = true;
            
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            req.CookieContainer = cCont;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            string formParams = string.Format("LevelId={0}&LevelNumber={1}&LevelAction.Answer={2}", LevelId, LevelNumber, val);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(formParams);
            req.ContentLength = bytes.Length;
            using (Stream os = req.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
            }
            string ps = "";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            {
                ps = sr.ReadToEnd();
            }

            lastpage = ps;
            return ps;
        }
    }
}
