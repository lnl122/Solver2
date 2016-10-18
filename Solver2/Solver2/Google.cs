﻿// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System.Net;

namespace Solver2
{
    class Google
    {
        //
        // public static Words GetImageDescription(string filepath)     - получение объекта Words из локального пути картинки
        // public static string ParsingPage(string g)                   - парсинг страницы гугля
        // public static string GetPageByImageUrl(string imgurl)        - получение страницы по урлу
        //

        // пути
        private static string googleRU = "https://www.google.ru/searchbyimage?&hl=ru-ru&lr=lang_ru&image_url=";
        // максимальное количество попыток чтения
        private static int MaxTryToReadPage = 3;
        // на сколько миллисекунд засыпать при неудачном одном чтении
        private static int TimeToSleepMs = 1000;

        public static string UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1";

        private static string[,] tags = {
                { "<script"  , "<noscript>" , "<style>" , "onmousedown=\"", "value=\"", "data-jiis=\"", "data-ved=\"", "aria-label=\"", "jsl=\"", "id=\"", "data-jibp=\"", "role=\"", "jsaction=\"", "onload=\"", "alt=\"", "title=\"", "width=\"", "height=\"", "data-deferred=\"", "aria-haspopup=\"", "aria-expanded=\"", "<input", "tabindex=\"", "tag=\"", "aria-selected=\"", "name=\"", "type=\"", "action=\"", "method=\"", "autocomplete=\"", "aria-expanded=\"", "aria-grabbed=\"", "data-bucket=\"", "aria-level=\"", "aria-hidden=\"", "aria-dropeffect=\"", "topmargin=\"" , "margin=\"", "data-async-context=\"", "valign=\"", "data-async-context=\"", "unselectable=\"", "<!--", "ID=\"", "style=\"" , "class=\"" , "//<![CDATA[" , "border=\"" , "cellspacing=\"" , "cellpadding=\"" , "target=\"" , "colspan=\"" , "onclick=\"" , "align=\"" , "color=\"" , "nowrap=\"" , "vspace=\"" , "href=\"" , "src=\"", "<cite"  , "{\"", "<g-img"  , "<a data-"   },
                { "</script>", "</noscript>", "</style>", "\""            , "\""      , "\""          , "\""         , "\""           , "\""    , "\""   , "\""          , "\""     , "\""         , "\""       , "\""    , "\""      ,"\""       , "\""       , "\""              , "\""              , "\""              , ">"     , "\""         , "\""    , "\""              , "\""     , "\""     , "\""       , "\""       , "\""             , "\""              , "\""             , "\""            , "\""           , "\""            , "\""                , "\""           , "\""       , "\""                   , "\""       , "\""                   , "\""             , "-->" , "\""   , "\""       , "\""       , "//]]>"       , "\""        , "\""             , "\""             , "\""        , "\""         , "\""         , "\""       , "\""       , "\""        , "\""        , "\""      , "\""    , "</cite>", "}"  , "</g-img>", "</a>"       }
            };
        private static string[,] tags1 = {
                { "<span>"   },
                { "</span>"  }
            };

        // читаем страницу по урлу, отрезаем шапку
        // вход - урл картинки
        // выход - страница гугля
        public static string GetPageByImageUrl(string imgurl)
        {
            string gurl = googleRU + imgurl;
            WebClient wc = new WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;
            wc.Headers.Add("User-Agent", UserAgent);
            wc.Headers.Add("Accept-Language", "ru-ru");
            wc.Headers.Add("Content-Language", "ru-ru");
            string page = "";
            bool isNeedReadPage = true;
            int CountTry = 0;
            while (isNeedReadPage)
            {
                try
                {
                    page = wc.DownloadString(gurl);
                    isNeedReadPage = false;
                }
                catch
                {
                    System.Threading.Thread.Sleep(TimeToSleepMs);
                    CountTry++;
                    if (CountTry == MaxTryToReadPage)
                    {
                        Log.Write("g_img ERROR: не удалось получить страницу гугля для изображение по ссылке ", imgurl);
                        Log.Store("g_img", page);
                        page = "";
                        isNeedReadPage = false;
                    }
                }
            }
            wc.Dispose();
            wc = null;

            if (page.Length <= 0)
            {
                Log.Write("g_img ERROR: длина строки нулевая");
                return "";
            }
            page = page.ToLower().Replace("\t", " ").Replace("\n", " ");
            int body1 = page.IndexOf("<body");
            int body2 = page.IndexOf("</body>");
            if ((body1 == -1) || (body2 == -1))
            {
                Log.Write("g_img ERROR: нет тегов <body> у страницы");
                Log.Store("g_img", page);
                return "";
            }
            page = page.Substring(body1 + 5, body2 - body1 - 5);
            Log.Store("g_img", page);
            return page;
        }

        // парсим текст страницы
        // вход - страница
        // выход - текст со страницы после парсинга
        public static string ParsingPage(string g)
        {
            if (g.Length < 1) { return ""; }

            int ihr1 = g.IndexOf("<hr");
            int ihr2 = g.LastIndexOf("<hr");
            if ((ihr1 < 0) || (ihr2 < 0)) { return ""; }
            g = g.Substring(ihr1, ihr2 - ihr1);

            g = ParsePage.ParseTags(g, tags);

            g = g.Replace("&nbsp;", " ");
            g = g.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            g = g.Replace(" >", ">").Replace("data-hve", " ");
            g = g.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            g = g.Replace("<em>", " ").Replace("</em>", " ").Replace("data-hve", " ").Replace("<h2>", " ").Replace("<h3>", " ").Replace("</h2>", " ").Replace("</h3>", " ");
            g = g.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            g = g.Replace(" >", ">").Replace("data-hve", " ");
            g = g.Replace("<a></a>", "").Replace("<div></div>", "").Replace("<span></span>", "").Replace("<a></a>", "").Replace("<div></div>", "").Replace("<span></span>", "").Replace("<a></a>", "").Replace("<div></div>", "").Replace("<span></span>", "");

            g = ParsePage.ParseTags(g, tags);

            g = g.Replace("<a>&times;</a>", "");

            g = ParsePage.ParseTags(g, tags1);

            g = g.Replace("страницы с подходящими изображениями", " ");
            g = g.Replace("<a>похожие изображения</a>", " ");
            g = g.Replace("благодарим за замечания.", " ");
            g = g.Replace("пожаловаться на содержание картинки.", " ");
            g = g.Replace("результаты поиска", " ");
            g = g.Replace("<a>сохраненная копия</a>", " ");
            g = g.Replace("<a>похожие</a>", " ");
            g = g.Replace("<ol>", " ").Replace("</ol>", " ").Replace("<li>", " ").Replace("</li>", " ").Replace("data-rt", " ").Replace("&middot;", " ");
            g = g.Replace("<wbr>", " ").Replace("&quot;", " ").Replace("...", " ").Replace("»", " ").Replace("«", " ").Replace("&#39;", " ");
            g = g.Replace("<hr>", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            g = g.Replace(" >", ">").Replace("> ", ">").Replace(" <", "<").Replace("< ", "<");
            g = g.Replace("<div>", " ").Replace("</div>", " ").Replace("<span>", " ").Replace("</span>", " ").Replace("<a>", " ").Replace("</a>", " ");
            g = g.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("<div e>", " ").Replace("  ", " ").Replace("  ", " ");
            g = g.Replace(";", " ").Replace("+", " ").Replace("\"", " ").Replace("—", " ").Replace("|", " ").Replace(".", " ").Replace("%", " ").Replace("*", " ").Replace("/", " ").Replace(",", " ").Replace("!", " ").Replace("?", " ").Replace(":", " ").Replace("-", " ").Replace("(", " ").Replace(")", " ");
            g = g.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");

            return g;
        }

        // вход - локальный путь к файлу с изображением
        // выход - набор слов со страницы гугля
        public static Words GetImageDescription(string path)
        {
            string a = Upload.UploadFile(path);
            if ((a == "") || (a == null)) { return null; }
            string b = GetPageByImageUrl(a);
            if ((b == "") || (b == null)) { return null; }
            string c = ParsingPage(b);
            if ((c == "") || (c == null)) { return null; }
            Words res = new Words(c);
            return res;
        }
    }
}
