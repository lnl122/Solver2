using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Solver2
{
    class Films
    {
        // http://www.hdkinoteatr.com/
        // и далее только через форму, она не простая
        // надо бы тестировать в отдельном приложении с отдельным браузером в окне

        private static string baseurl = "http://www.hdkinoteatr.com/index.php";
        public static string cHead;             // куки
        public static CookieContainer cCont;    // куки

        // по строке с гапоификой
        // вход - ГаПоиФиКа
        // выход - Список строк - названия фильмов, соответствующих гапоифике
        public static List<string> GetNameList(string name)
        {
            int current = 1;
            int idx = 1;
            List<string> res = new List<string>();
            string str = PrepareSearchString(name.Trim());
            string page = GetPage(str, idx, 1);

            int total = DetectCount(page);
            if (total == 0)
            {
                return res;
            }

            res.AddRange(GetNamesFromPage(page));

            current += 10;
            idx++;
            while (current <= total)
            {
                page = GetPage(str, idx, current);
                res.AddRange(GetNamesFromPage(page));
                current += 10;
                idx++;
            }

            return res;
        }

        // выполняет парсинг страницы, получает названия фильмов
        // вход - страница
        // выход - список названий
        private static List<string> GetNamesFromPage(string page)
        {
            List<string> res = new List<string>();

            string[] ar1 = System.Text.RegularExpressions.Regex.Split(page, "<div class=\"dpad searchitem\">");
            int cnt = ar1.Length;
            if (cnt == 1)
            {
                return res;
            }
            for (int i=1; i<cnt; i++)
            {
                string item = ar1[i];
                int ii1 = item.IndexOf("</div>");
                if (ii1 == -1) { continue; }
                item = item.Substring(0, ii1).Replace("<span class=\"hilight\">", "").Replace("</span>", "");
                ii1 = item.IndexOf("</a>");
                if (ii1 == -1) { continue; }
                item = item.Substring(0, ii1); //.Replace("<h3>", "").Replace("\n", "");
                ii1 = item.IndexOf("href=\"");
                if (ii1 == -1) { continue; }
                item = item.Substring(ii1 + 6);
                ii1 = item.IndexOf("\">");
                if (ii1 == -1) { continue; }
                item = item.Substring(ii1 + 2);
                ii1 = item.IndexOf("/");
                if (ii1 != -1)
                {
                    item = item.Substring(0, ii1);
                }
                item = item.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
                res.Add(item);
            }

            return res;
        }

        // считает количество найденных итемов
        // вход - строка страницы
        // выход - количество имеющихся фильмов
        private static int DetectCount(string page)
        {
            int res = 0;

            int ii1 = page.IndexOf("class=\"srchmsg\"");
            if (ii1 == -1)
            {
                return res;
            }

            string p = page;
            p = p.Substring(ii1);
            ii1 = p.IndexOf("</div>");
            p = p.Substring(0, ii1);

            string[] ar1 = p.Split(' ');
            foreach(string s in ar1)
            {
                int number;
                bool result = Int32.TryParse(s, out number);
                if (result)
                {
                    res = number;
                    break;
                }
            }
            return res;
        }

        // получает страницу с поиском, первую
        // вход - строка, которую ищем
        // выход - текст страницы
        private static string GetPage(string str, int idx, int from)
        {
            string formParams = string.Format("do=search&story={0}&titleonly=3&search_start={1}&subaction=search&result_from={2}&showposts=1", str, idx, from);
            string cookieHeader = "";
            CookieContainer cookies = new CookieContainer();
            cCont = cookies;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(baseurl);
            req.CookieContainer = cookies;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(formParams);
            req.ContentLength = bytes.Length;
            using (Stream os = req.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
            }
            string pageSource = "";
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                cookieHeader = resp.Headers["Set-cookie"];
                cHead = cookieHeader;
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    pageSource = sr.ReadToEnd();
                }
            }
            catch
            {
                Log.Write("Films ERROR: не удалось получить первую страницу", str);
            }
            return pageSource;
        }

        // преобразует ГаПоиФиКу в строку для поиска
        // вход - строка "ГаПоиФиКа"
        // выход - строка "га по и фи ка"
        public static string PrepareSearchString(string name)
        {
            // пробелы по заглавным буквам
            string res = "";
            for (int i=0; i<name.Length; i++)
            {
                string c = name.Substring(i,1);
                if (c[0] == c.ToUpper()[0])
                {
                    res = res + " ";
                }
                res = res + c;
            }
            // пробелы, если длина слова больше 2-х
            string res2 = "";
            int cnt = 0;
            for (int i=0; i<res.Length; i++)
            {
                string c = res.Substring(i, 1);
                if (c == " ")
                {
                    cnt = 0;
                }
                else
                {
                    cnt++;
                }
                if (cnt == 2)
                {
                    cnt = 0;
                    res2 = res2 + c + " ";
                }
                else
                {
                    res2 = res2 + c;
                }
            }
            // нормализуем ответ
            res2 = res2.ToLower();
            res2 = res2.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
            return res2;
        }
    }
}
