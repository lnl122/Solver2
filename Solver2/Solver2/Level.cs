// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;

namespace Solver2
{
    //
    // public Level(GameSelect GameParams, int lvl_number)  - создает объект с данными об уровне
    // public static string GetPageLevel(int idx)           - получает текст уровня по номеру
    // public void ForceUpdateLevel()                       - обновляет содержимое уровня полностью принудительно
    // public void ForceUpdateAnswersLevel()                - обновляет только ответы и статус уровня принудительно
    // public void UpdateLevel(string page)                 - обновляет содержимое уровня полностью
    // public void UpdateAnswersLevel(string page)          - обновляет только ответы и статус уровня
    //

    class Level
    {
        public int number;
        public string name;
        public string page;
        public string text;
        public string html;
        public bool isClose;
        public List<string> answers_good;
        public List<string> answers_bad;
        public int sectors;
        public int bonuses;
        public int secbon;
        public string[] sector;
        public string[] bonus;
        public List<string> urls;
        public string formlevelid;
        public string formlevelnumber;
        public DateTime dt;
        public GameSelect G;

        public static GameSelect Game;

        // вход - параметры игры, номер уровня 1..99
        // выход - объект с данными уровня
        // получает сведения об уровне, парсит его код.
        public Level(GameSelect GameParams = null, int lvl_number = 0)
        {
            Game = GameParams;
            G = GameParams;
            //if (Game.isStorm == true) { L = new level[Game.gamelevels]; } else { L = new level[1]; }
            // *** доделать отдельную ветки для линейных МШ
            // весь код ниже пока относиться (08.09.16) только к штурмам

            number = lvl_number;
            page = "";
            if (GameParams != null)
            {
                page = GetPageLevel(lvl_number);
            } 
            Log.Store("level_clean_" + lvl_number.ToString(), page);
            name = GetLvlName(page);
            isClose = GetLvlClose(page);
            answers_bad = GetLvlAnsBad(page);
            answers_good = GetLvlAnsGood(page);
            sector = GetLvlSectors(page);
            sectors = sector.Length;
            bonus = GetLvlBonuses(page);
            bonuses = bonus.Length;
            secbon = GetSecBon();
            formlevelid = "";
            formlevelnumber = "";
            if (!isClose)
            {
                formlevelid = GetLvlFormlevelid(page);
                formlevelnumber = GetLvlFormlevelnumber(page);
            }
            text = GetLvlText(page);
            html = GetLvlHtml(page);
            Log.Store("level_parsed_" + lvl_number.ToString(), html);
            urls = GetLvlUrls(page);
            dt = DateTime.Now;
        }

        // считает общее количество ответов, необходимо для дальнейшего обпередения необходимости обновления формы
        // выход - количество
        private int GetSecBon()
        {
            int r = 0;
            foreach (string s1 in sector)
            {
                string ss = s1.Trim().Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
                if (ss != "") { r++; }
            }
            foreach (string s1 in bonus)
            {
                string ss = s1.Trim().Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
                if (ss != "") { r++; }
            }
            return r;
        }

        // обновляет содержимое уровня полностью принудительно
        public void ForceUpdateLevel()
        {
            page = GetPageLevel(number);
            isClose = GetLvlClose(page);
            answers_bad = GetLvlAnsBad(page);
            answers_good = GetLvlAnsGood(page);
            sector = GetLvlSectors(page);
            sectors = sector.Length;
            bonus = GetLvlBonuses(page);
            bonuses = bonus.Length;
            secbon = GetSecBon();
            formlevelid = "";
            formlevelnumber = "";
            if (!isClose)
            {
                formlevelid = GetLvlFormlevelid(page);
                formlevelnumber = GetLvlFormlevelnumber(page);
            }
            text = GetLvlText(page);
            html = GetLvlHtml(page);
            urls = GetLvlUrls(page);
            dt = DateTime.Now;
        }
        // обновляет только ответы и статус уровня принудительно
        public void ForceUpdateAnswersLevel()
        {
            page = GetPageLevel(number);
            isClose = GetLvlClose(page);
            answers_bad = GetLvlAnsBad(page);
            answers_good = GetLvlAnsGood(page);
            sector = GetLvlSectors(page);
            sectors = sector.Length;
            bonus = GetLvlBonuses(page);
            bonuses = bonus.Length;
            secbon = GetSecBon();
            dt = DateTime.Now;
        }
        // обновляет содержимое уровня полностью
        public void UpdateLevel(string page)
        {
            isClose = GetLvlClose(page);
            answers_bad = GetLvlAnsBad(page);
            answers_good = GetLvlAnsGood(page);
            sector = GetLvlSectors(page);
            sectors = sector.Length;
            bonus = GetLvlBonuses(page);
            bonuses = bonus.Length;
            secbon = GetSecBon();
            formlevelid = "";
            formlevelnumber = "";
            if (!isClose)
            {
                formlevelid = GetLvlFormlevelid(page);
                formlevelnumber = GetLvlFormlevelnumber(page);
            }
            text = GetLvlText(page);
            html = GetLvlHtml(page);
            urls = GetLvlUrls(page);
            dt = DateTime.Now;
        }
        // обновляет только ответы и статус уровня
        public void UpdateAnswersLevel(string page)
        {
            isClose = GetLvlClose(page);
            answers_bad = GetLvlAnsBad(page);
            answers_good = GetLvlAnsGood(page);
            sector = GetLvlSectors(page);
            sectors = sector.Length;
            bonus = GetLvlBonuses(page);
            bonuses = bonus.Length;
            secbon = GetSecBon();
            dt = DateTime.Now;
        }
        // получает страницу по номеру уровня
        public static string GetPageLevel(int idx)
        {
            string url = "http://" + Game.gamedomain + "/gameengines/encounter/play/" + Game.gameid + "/?level=" + idx.ToString();
            string page = Engine.GetPage(url);
            Engine.lastlevel = idx;
            Engine.lastpage = page;
            return page;
        }

        // возвращает наименование текущего уровня со страницы
        private static string GetLvlName(string g1)
        {
            string g = g1;//.ToLower();
            int i1 = g.IndexOf("<ul class=\"section level\">");
            if (i1 == -1) { return "не определен"; }
            g = g.Substring(i1);
            int i2 = g.IndexOf("</ul>");
            g = g.Substring(0, i2);
            i1 = g.IndexOf("<span>");
            if (i1 == -1) { return "не определен"; }
            g = g.Substring(i1 + 6);
            i2 = g.IndexOf("</span>");
            g = g.Substring(0, i2);
            return g;
        }
        // возвращает признак - уровень закрыт или нет
        private static bool GetLvlClose(string g1)
        {
            string g = g1.ToLower();
            int i1 = g.IndexOf("<label for=\"answer\">");
            if (i1 == -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // возвращает ид уровня
        private static string GetLvlFormlevelid(string g1)
        {
            string g = g1.ToLower();
            int i1 = g.IndexOf("<form method=\"post\">");
            if (i1 == -1) { return ""; }
            else
            {
                g = g.Substring(i1);
                i1 = g.IndexOf("</form>");
                g = g.Substring(0, i1);
                string[] ar1 = System.Text.RegularExpressions.Regex.Split(g, "<input");
                foreach (string s1 in ar1)
                {
                    if (s1.Contains("levelid"))
                    {
                        string s2 = s1.Substring(s1.IndexOf("value=\"") + 7);
                        s2 = s2.Substring(0, s2.IndexOf("\""));
                        return s2;
                    }
                }
            }
            return "";
        }
        // возвращает номер уровня для формы
        private static string GetLvlFormlevelnumber(string g1)
        {
            string g = g1.ToLower();
            int i1 = g.IndexOf("<form method=\"post\">");
            if (i1 == -1) { return ""; }
            else
            {
                g = g.Substring(i1);
                i1 = g.IndexOf("</form>");
                g = g.Substring(0, i1);
                string[] ar1 = System.Text.RegularExpressions.Regex.Split(g, "<input");
                foreach (string s1 in ar1)
                {
                    if (s1.Contains("levelnumber"))
                    {
                        string s2 = s1.Substring(s1.IndexOf("value=\"") + 7);
                        s2 = s2.Substring(0, s2.IndexOf("\""));
                        return s2;
                    }
                }
            }
            return "";
        }
        // возвращает список неудачных ответов
        private static List<string> GetLvlAnsBad(string g1)
        {
            string g = g1.ToLower();
            List<string> res = new List<string>();
            int i1 = g.IndexOf("<ul class=\"history\">");
            if (i1 == -1) { return res; }
            g = g.Substring(i1);
            i1 = g.IndexOf("</ul>");
            g = g.Substring(0, i1);
            string[] ar1 = System.Text.RegularExpressions.Regex.Split(g, "<i>");
            foreach (string s1 in ar1)
            {
                int i2 = s1.IndexOf("</i>");
                if (i2 == -1) { continue; }
                string s2 = s1.Substring(0, i2);
                res.Add(s2);
            }
            return res;
        }
        // возвращает список удачных ответов
        private static List<string> GetLvlAnsGood(string g1)
        {
            string g = g1.ToLower();
            List<string> res = new List<string>();
            int i1 = g.IndexOf("<ul class=\"history\">");
            if (i1 == -1) { return res; }
            g = g.Substring(i1 + 20);
            i1 = g.IndexOf("</ul>");
            g = g.Substring(0, i1);
            g = g.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("<i>", " ").Replace("</i>", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            string[] ar1 = System.Text.RegularExpressions.Regex.Split(g, "</li>");
            foreach (string s1 in ar1)
            {
                int i2 = s1.IndexOf("<li class=\"correct\">");
                if (i2 == -1) { continue; }
                i2 = s1.IndexOf("<span");
                string s2 = s1.Substring(i2);
                i2 = s2.IndexOf(">");
                s2 = s2.Substring(i2 + 1);
                i2 = s2.IndexOf("<");
                s2 = s2.Substring(0, i2).Trim();
                if ((s2 != "") && (s2 != "пройден по таймауту")) { res.Add(s2); }
            }
            return res;
        }
        // возвращает перечень секторов и ответы на них
        private static string[] GetLvlSectors(string g1)
        {
            string g = g1.ToLower();
            List<string> res2 = new List<string>();
            string[] res = new string[0];
            g = g.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("<i>", " ").Replace("</i>", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            int i1 = g.IndexOf("<div class=\"cols-wrapper\">");
            if (i1 == -1)
            {
                string[] res1 = new string[0];
                return res1;
            }
            g = g.Substring(i1 + ("<div class=\"cols-wrapper\">").Length);
            i1 = g.IndexOf("</div><!--end cols-wrapper -->");
            g = g.Substring(0, i1);
            g = g.Replace("<div class=\"cols\">", "").Replace("</div><!--end cols-->", "").Replace("<div class=\"cols w100per\">", "").Trim();
            string[] ar1 = System.Text.RegularExpressions.Regex.Split(g, "<p>");
            foreach (string s1 in ar1)
            {
                if (s1.Length < 5) { continue; }
                int i2 = s1.IndexOf("class");
                string s2 = s1.Substring(i2);
                if (s2.Contains("color_dis"))
                {
                    res2.Add("");
                }
                if (s2.Contains("color_correct"))
                {
                    i2 = s2.IndexOf(">");
                    s2 = s2.Substring(i2 + 1);
                    i2 = s2.IndexOf("<");
                    s2 = s2.Substring(0, i2);
                    res2.Add(s2);
                }
            }
            res = new string[res2.Count];
            for (int i = 0; i < res2.Count; i++)
            {
                res[i] = res2[i];
            }
            return res;
        }
        // возвращает перечень бонусов и ответы на них
        private static string[] GetLvlBonuses(string g1)
        {
            string g = g1.ToLower();
            List<string> res2 = new List<string>();
            string[] res = new string[0];
            g = g.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("<i>", " ").Replace("</i>", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            int i1 = g.IndexOf("<h3>задание</h3>");
            if (i1 == -1)
            {
                i1 = g.IndexOf("<h3>task</h3>");
                if (i1 == -1)
                {
                    return res;
                }
            }
            g = g.Substring(i1);
            string[] ar1 = System.Text.RegularExpressions.Regex.Split(g, "<div class=\"spacer\"></div>");
            foreach (string s1 in ar1)
            {
                if (s1.Contains("<h3 class=\"color_bonus\">"))
                {
                    res2.Add("");
                }
                if (s1.Contains("<h3 class=\"color_correct\">"))
                {
                    int i2 = s1.IndexOf("<p>");
                    if (i2 == -1)
                    {
                        res2.Add("");
                    }
                    else
                    {
                        string s2 = s1.Substring(i2);
                        i2 = s2.IndexOf("</p>");
                        s2 = s2.Substring(0, i2);
                        s2 = s2.Replace("<p>", "").Replace("</p>", "").Trim();
                        res2.Add(s2);
                    }
                }
            }
            res = new string[res2.Count];
            for (int i = 0; i < res2.Count; i++)
            {
                res[i] = res2[i];
            }
            return res;
        }
        // возвращает хтмл текст уровня
        private static string GetLvlHtml(string g)
        {
            string res = "";
            res = ParsePage.ParseTags(g, tags4textlvl);
            res = ParsePage.ParseTags(res, tags4textlvl2);
            res = res.Replace("\t", " ").Replace("\n", "\r").Replace("<div class=\"spacer\"></div>", "\r").Replace("<br>", "\r").Replace("<ul>", "\r");
            res = res.Replace("<div class=\"container\">", "\r").Replace("<div class=\"content\">", "\r");
            res = res.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            res = res.Replace("\r\r", "\r").Replace("\r\r", "\r").Replace("\r\r", "\r").Replace("\r\r", "\r").Replace("\r\r", "\r");
            res = res.Replace("\r", "\r\n").Replace(" \r\n", "\r\n").Replace("\r\n ", "\r\n");

            return res;
        }
        // возвращает текст уровня
        private static string GetLvlText(string g)
        {
            string res = "";
            g = g.Replace("\t", " ").Replace("&nbsp;", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            int i1 = g.ToLower().IndexOf("<h3>задание</h3>");
            if (i1 == -1)
            {
                i1 = g.ToLower().IndexOf("<h3>task</h3>");
                if (i1 == -1)
                {
                    return res;
                }
            }
            g = g.Substring(i1).Replace("<h3>задание</h3>", "").Replace("<h3>task</h3>", "").Replace("<h3>Задание</h3>", "").Replace("<h3>Task</h3>", "").Replace("<H3>задание</H3>", "").Replace("<H3>task</H3>", "").Replace("<H3>Задание</H3>", "").Replace("<H3>Task</H3>", "");
            i1 = g.ToLower().IndexOf("</h3>"); if (i1 != -1) { g = g.Substring(0, i1); }
            i1 = g.ToLower().IndexOf("</div>"); if (i1 != -1) { g = g.Substring(0, i1); }
            g = g.Replace("\n", "\r").Replace("<br/>", "\r").Replace("<p>", " ").Replace("</p>", " ").Replace("\n", "\r").Replace("\n", "\r").Replace("\n", "\r");
            g = g.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("\r ", "\r").Replace(" \r", "\r").Replace("\r\r", "\r").Replace("\r\r", "\r").Replace("\r\r", "\r").Replace("\r\r", "\r");
            string[] ar1 = System.Text.RegularExpressions.Regex.Split(g, "<div class=\"spacer\">");
            res = res + ParsePage.ParseTags(ar1[0], tags4bonus) + "\r\r";

            foreach (string s1 in ar1)
            {
                if (s1.Contains("<h3 class=\"color_bonus\">"))
                {
                    string s2 = ParsePage.ParseTags(s1, tags4bonus);
                    res = res + s1 + "\r\r"; // *** надо изменить, учесть обработку скриптов для картинок, выкинуть лишнее
                }
            }
            res = res.Replace("\r", "\r\n");
            return res;
        }

        // возвращает набор урлов
        private static List<string> GetLvlUrls(string g1)
        {
            string g = g1.ToLower();
            List<string> res = new List<string>();
            g = g.Replace("\t", " ").Replace("&nbsp;", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            int i1 = g.IndexOf("<h3>задание</h3>");
            if (i1 == -1)
            {
                i1 = g.IndexOf("<h3>task</h3>");
                if (i1 == -1)
                {
                    return res;
                }
            }
            g = g.Substring(i1).Replace("<h3>задание</h3>", "").Replace("<h3>task</h3>", "");

            string[] ar1 = System.Text.RegularExpressions.Regex.Split(g, "<img src=\"");
            foreach (string s1 in ar1)
            {
                if (s1.Substring(0, 4) == "http")
                {
                    string s2 = s1.Substring(0, s1.IndexOf("\""));
                    res.Add(s2);
                }
            }
            return res;
        }

        private static string[,] tags4bonus = {
                { "<span class=\"color_sec\">", "бонус ", "bonus ", "<img"   },
                { "</span>"                   , " "     , " "     , ">"      }
            };
        private static string[,] tags4textlvl = {
                { "<!--[if lte ie 7]>", "<form",   "<iframe",   "<li class=\"refresh\">", "<ul class=\"history\">", "<ul class=\"section level\">", "<p class=\"globalmess\">", "<div class=\"cols-wrapper\">", "<div class=\"cols\">" },
                { "<![endif]-->"      , "</form>", "</iframe>", "</ul>"                 , "</ul>"                 , "</ul>"                       , "</p>"                    , "<!--end cols-wrapper -->",     "<!--end cols-->"  }
            };
        private static string[,] tags4textlvl2 = {
                { "<h3 class=\"color_", "<div class=\"pane\"", "<div class=\"aside\">", "<div class=\"header\">", "<script"  , "<h3>"   },
                { "</h3>"             , "</div>"             , "</div>"               , "</div>"                , "</script>", "</h3>"  }
            };

    }
}
