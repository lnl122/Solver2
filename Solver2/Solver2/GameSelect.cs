﻿// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Solver2
{
    class GameSelect
    {
        //
        // public GameSelect(Logon logonData)   - форма + получение и выбор игры из списка
        //

        private int border = 5; // расстояния между элементами форм, константа
        public static TextBox tbGname;

        public bool isSuccessful = true;
        public string username = "";
        public string password = "";
        public string userid = "";
        public string gamedomain = "";
        public string gameid = "";
        public bool isStorm = false;
        public bool isBrain = false;
        public int gamelevels = 0;

        private static string[] g_names;
        private static string[] g_urls;

        private string urlbeg = "http://game.en.cx/UserDetails.aspx?zone=1&tab=1&uid=";
        private string urlend = "&page=1";

        private static string[,] tags4list = {
                { "<script"  , "<noscript>" , "<style>" , "<!--", "bgcolor=\"", "align=\"", "nowrap=\"", "style=\"", "class=\"", "class='", "onclick=\"" , "id=\"", "height=\"" },
                { "</script>", "</noscript>", "</style>", "-->" , "\""        , "\""      , "\""       , "\""      , "\""      , "'"      , "\""         , "\""   , "\""        }
            };


        /// <summary>
        /// получает перечень игр
        /// </summary>
        /// <returns>текст страницы</returns>
        private string GetUserGames()
        {
            string url1 = urlbeg + userid + urlend;
            string cookieHeader = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url1);
            req.CookieContainer = Engine.cCont;
            req.ContentType = "application/x-www-form-urlencoded";
            string pageSource = "";
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                cookieHeader = resp.Headers["Set-cookie"];
                Engine.cHead = cookieHeader;
                using (StreamReader sr = new StreamReader(resp.GetResponseStream())) { pageSource = sr.ReadToEnd(); }
            }
            catch
            {
                Log.Write("en.cx ERROR: не удалось получить перечень игр");
                isSuccessful = false;
                return "";
            }
            int it1 = pageSource.IndexOf("VirtualGamesDescription");
            if (it1 == -1)
            {
                Log.Write("en.cx ERROR: не удалось выполнить парсинг страницы с играми пользвоателя, не нашли текст 'VirtualGamesDescription'");
                isSuccessful = false;
                return "";
            }
            pageSource = pageSource.Substring(it1);
            it1 = pageSource.IndexOf("QuizDescription");
            if (it1 == -1)
            {
                Log.Write("en.cx ERROR: не удалось выполнить парсинг страницы с играми пользвоателя, не нашли текст 'QuizDescription'");
                isSuccessful = false;
                return "";
            }
            pageSource = pageSource.Substring(0, it1);
            pageSource = RemoveTags(pageSource);
            if (pageSource.Length < 1)
            {
                Log.Write("en.cx ERROR: не удалось выполнить парсинг страницы с играми пользвоателя");
                isSuccessful = false;
                return "";
            }
            return pageSource;
        }

        /// <summary>
        /// из страницы получает необработанный перечень игр
        /// </summary>
        /// <param name="pageSource">текст страницы</param>
        /// <returns>список строк, по одной на игру</returns>
        private static List<string> GetDirtyListGames(string pageSource)
        {
            string[] ar1 = System.Text.RegularExpressions.Regex.Split(pageSource.Replace(" bg>", "").Replace("\r\n", " ").Replace("</tr> ", "").Replace("</td> ", ""), "<tr");
            List<string> l1 = new List<string>();

            foreach (string s1 in ar1)
            {
                if (s1.IndexOf("/Teams/TeamDetails.aspx") != -1)
                {
                    l1.Add(s1.Replace(" >", ">").Replace("<span>", " ").Replace("</span>", " ").Replace("<br />", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " "));
                }
            }
            return l1;
        }

        /// <summary>
        /// из строк со страницы с перечнем игр (грязный список) делает чистый список только неигранных игр
        /// </summary>
        /// <param name="DirtyList">список грязных строк</param>
        /// <returns>список списков строк, описывающих отдельные неигранные игры</returns>
        private List<List<string>> GetCleanListGames(List<string> DirtyList)
        {
            List<List<string>> res = new List<List<string>>();
            foreach (string s2 in DirtyList)
            {
                string r_url = "";
                string r_name = "";
                string r_num = "";
                bool r_flag = true;
                string[] ar2 = System.Text.RegularExpressions.Regex.Split(s2, "<td>");
                for (int i = 0; i < ar2.Length; i++)
                {
                    if (ar2[i].Length < 5) { continue; }
                    if (ar2[i].Trim().Substring(ar2[i].Trim().Length - 5, 5) == "Место") { r_flag = false; break; }
                    if (ar2[i].Trim()[0] == '#') { r_num = ar2[i]; }
                    if (ar2[i].IndexOf("<a href=\"") != -1)
                    {
                        string q1 = ar2[4].Substring(0, ar2[4].IndexOf("</a>")).Replace("<a href=\"", "");
                        r_url = q1.Substring(0, q1.IndexOf("\">"));
                        r_name = q1.Substring(q1.IndexOf("\">") + 2);
                    }
                }
                if (r_flag)
                {
                    List<string> l2 = new List<string>();
                    l2.Add(r_url.Trim());
                    l2.Add(r_num.Trim());
                    l2.Add(r_name.Trim());
                    res.Add(l2);
                }
            }
            return res;
        }

        /// <summary>
        /// создаём форму для выбора игры
        /// </summary>
        /// <param name="res">список списков строк с играми</param>
        /// <returns></returns>
        private Form CreateForm(List<List<string>> res)
        {
            // форма для ввода данных, создаем
            Form SelectGame = new Form();
            SelectGame.Text = "Выбор игры..";
            SelectGame.StartPosition = FormStartPosition.CenterScreen;
            SelectGame.Width = 35 * border;
            SelectGame.Height = 25 * border;
            SelectGame.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            SelectGame.AutoSize = true;
            SelectGame.Icon = Properties.Resources.icon2;
            Label la = new Label();
            la.Text = "Необходимо двойным кликом выбрать игру из списка\r\nили же ввести ссылку на игру в нижнем поле ввода\r\nи нажать 'Открыть игру'";
            la.Top = 2 * border;
            la.Left = border;
            la.Width = 100 * border;
            la.Height = 10 * border;
            SelectGame.Controls.Add(la);
            ListBox lb = new ListBox();
            lb.Top = la.Bottom + border;
            lb.Left = border;
            lb.Width = la.Width;
            lb.Height = 20 * border;

            g_urls = new string[res.Count];
            g_names = new string[res.Count];
            for (int i = 0; i < res.Count; i++)
            {
                List<string> l3 = res[i];
                g_names[i] = l3[1] + " | " + l3[2];
                g_urls[i] = l3[0];
                lb.Items.Add(g_names[i]);
            }
            lb.DoubleClick += new EventHandler(Event_SelectGameFromList);
            SelectGame.Controls.Add(lb);
            tbGname = new TextBox();
            tbGname.Text = "";
            if ((Environment.MachineName == "NBIT01") || (Environment.MachineName == "WS12")) { tbGname.Text = "http://demo.en.cx/gameengines/encounter/play/24889"; } // for TEST
            tbGname.Top = lb.Bottom + 2 * border;
            tbGname.Left = border;
            tbGname.Width = lb.Width - 24 * border;
            SelectGame.Controls.Add(tbGname);
            Button blok = new Button();
            blok.Text = "Открыть игру";
            blok.Top = tbGname.Top;
            blok.Left = tbGname.Right + 2 * border;
            blok.Width = 22 * border;
            blok.DialogResult = DialogResult.OK;
            SelectGame.AcceptButton = blok;
            SelectGame.Controls.Add(blok);
            return SelectGame;
        }

        /// <summary>
        /// получение ид игры и её домена из ссылки
        /// </summary>
        /// <param name="url">урл, указанный пользвоателем при выборе игры</param>
        private void GetDomainAndIdGame(string url)
        {
            // попробуем авторизоваться в игре - сначала разберем полученную строку
            if (url == "") { isSuccessful = false; MessageBox.Show("Не выбрана игра вообще.."); return; }
            string url2 = url;
            if (url2.Substring(0, 7) != "http://") { isSuccessful = false; MessageBox.Show("Указана не ссылка.."); return; }
            url2 = url.Replace("http://", "");
            int ii1 = url2.IndexOf("/"); if (ii1 == -1) { isSuccessful = false; MessageBox.Show("указан только хост.."); return; }
            gamedomain = url2.Substring(0, ii1);
            url2 = url2.Substring(ii1 + 1);
            if (url2.IndexOf("gameengines/encounter/play/") != -1)
            {
                ii1 = url2.IndexOf("/?level="); if (ii1 != -1) { url2 = url2.Substring(0, ii1); }
                gameid = url2.Substring(url2.LastIndexOf("/") + 1);
            }
            else
            {
                if (url2.IndexOf("GameDetails.aspx?gid=") != -1) { gameid = url2.Substring(url2.LastIndexOf("=") + 1); }
                else { isSuccessful = false; MessageBox.Show("Ссылку на игру не удалось понять.."); return; } // ни один из форматов ссылок не подошел
            }
        }

        /// <summary>
        /// получает игру по домену и ид, выполняет базовый парсинг страницы
        /// </summary>
        /// <param name="gamedomain">домен</param>
        /// <param name="gameid">ид игры</param>
        /// <returns>код страницы</returns>
        private string GetAndParseGamePage(string gamedomain, string gameid)
        {
            string ps3 = Engine.GetPage("http://" + gamedomain + "/GameDetails.aspx?gid=" + gameid);
            string ps4 = RemoveTags(ps3).ToLower().Replace("\r\n", "");
            ps4 = ps4.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            ps4 = ps4.Replace(" >", ">").Replace(" <", "<").Replace("< ", "<").Replace("> ", ">");
            string ps5 = ps4.Replace("<span>", "").Replace("</span>", "");
            return ps5;
        }

        /// <summary>
        /// получаем список игр МШ текущего игрока
        /// выход - список спиков из урл, номера, названия игр
        /// </summary>
        /// <param name="logonData">ид логона</param>
        public GameSelect(Logon logonData)
        {
            // перенесем авторизацию
            username = logonData.username;
            password = logonData.password;
            userid = logonData.userid;

            isSuccessful = true;
            string pageSource = GetUserGames();
            if (isSuccessful == false)
            {
                MessageBox.Show("Не удалось прочитать список игр, подробная причина указана в логе..");
                return;
            }
            List<string> DirtyList = GetDirtyListGames(pageSource);
            List<List<string>> res = GetCleanListGames(DirtyList);
            // в res сейчас перечень игр или пусто. Необходимо пользователю сделать выбор.

            // форма для ввода данных, создаем
            Form SelectGame = CreateForm(res);

            // выберем игру
            string page = "";
            bool fl = true;
            while (fl)
            {
                if (SelectGame.ShowDialog() == DialogResult.OK)
                {
                    isSuccessful = true;
                    GetDomainAndIdGame(tbGname.Text);
                    if (isSuccessful == false) { continue; }

                    // если авторизовались успешно - запоминаем игру
                    string ps2 = Engine.Logon("http://" + gamedomain + "/Login.aspx", username, password);
                    if (ps2.IndexOf("action=logout") != -1)
                    {
                        // прочесть игру и узнать её параметры
                        string ps5 = GetAndParseGamePage(gamedomain, gameid);
                        GetGameType(ps5);
                        if (!isBrain) { isSuccessful = false; MessageBox.Show("Это не МШ.."); continue; }

                        // *** потом убрать. заглушка для потенциальных линейных игр МШ
                        if (!isStorm) { isSuccessful = false; MessageBox.Show("Последовательность не штурмовая.."); continue; }

                        // прочитаем игру
                        string game_url = "http://" + gamedomain + "/gameengines/encounter/play/" + gameid;
                        page = Engine.GetPage(game_url);
                        page = page.ToLower();
                        if (page.IndexOf("class=\"gamecongratulation\"") != -1) { isSuccessful = false; MessageBox.Show("Эта игра уже закончилась.."); continue; }
                        if (page.IndexOf("<span id=\"animate\">поздравляем!!!</span>") != -1) { isSuccessful = false; MessageBox.Show("Эта игра уже закончилась.."); continue; }
                        if (page.IndexOf("капитан команды не включил вас в состав для участия в этой игре.") != -1) { isSuccessful = false; MessageBox.Show("Капитан команды не включил вас в состав для участия в этой игре.."); continue; }
                        if (page.IndexOf("<span id=\"panel_lblgameerror\">") != -1) { isSuccessful = false; MessageBox.Show("Эта игра ещё не началась.."); continue; }
                        if (page.IndexOf("вход в игру произойдет автоматически") != -1) { isSuccessful = false; MessageBox.Show("Эта игра ещё не началась.."); continue; }
                        if (page.IndexOf("ошибка. состав вашей команды превышает") != -1) { isSuccessful = false; MessageBox.Show("Состав вашей команды превышает установленный максимум.."); continue; }

                        //определим количтсво уровней
                        gamelevels = GetLevelsCount(page);
                        if ((isStorm == true) && (gamelevels == 0))
                        {
                            isSuccessful = false; MessageBox.Show("Выходит что штурмовая последовательность с 0 уровнями, фигня.."); continue;
                        }

                        // поставим флаг выхода
                        fl = false;

                        // в лог
                        Log.Write("en.cx Открыта игра " + gameid);
                        // отобразим на форме
                        Program.D.F.Text = Program.D.F.Text + " / " + game_url;
                        isSuccessful = true;
                    }
                    else
                    {
                        // если не успешно - вернемся в вводу пользователя
                        Log.Write("en.cx ERROR: Не удалось подключиться к " + gamedomain);
                        MessageBox.Show("Не удалось подключиться к " + gamedomain);
                        isSuccessful = false;
                    }
                }
                else
                {
                    // если отказались выбирать игру - выходим
                    fl = false;
                    isSuccessful = false;
                }
            } // выход только если fl = false -- это или отказ польователя в диалоге, или если нажато ОК - проверка пройдена

            // смотрим на page - если не пусто - то подключились
            if (isSuccessful)
            {
                Engine.SetId(userid, username, password, gameid, gamedomain, gamelevels);
            }
            
        }

        /// <summary>
        /// определяем количество уровней
        /// </summary>
        /// <param name="page">код страницы</param>
        /// <returns>количество уровней</returns>
        private int GetLevelsCount(string page)
        {
            int res = 0;
            if (isStorm)
            {
                string q_lvl = page.Substring(page.IndexOf("<body")).Replace("\r", "").Replace("\n", "").Replace("\t", "");
                string t1 = "<ul class=\"section level\">";
                string t2 = "</ul>";
                int i2 = q_lvl.IndexOf(t1);
                if (i2 != -1)
                {
                    q_lvl = q_lvl.Substring(i2 + t1.Length);
                    q_lvl = q_lvl.Substring(0, q_lvl.IndexOf(t2));
                    i2 = q_lvl.LastIndexOf("<i>");
                    q_lvl = q_lvl.Substring(i2 + 3);
                    q_lvl = q_lvl.Substring(0, q_lvl.IndexOf("</i>"));
                    if (Int32.TryParse(q_lvl, out i2)) { res = i2; }
                }
            }
            else
            {
                res = 0;
            }
            return res;
        }

        /// <summary>
        /// определяем тип игры и последовательность
        /// </summary>
        /// <param name="ps5">текст страницы</param>
        private void GetGameType(string ps5)
        {
            isBrain = true;
            int fr = ps5.IndexOf("игра:мозговой штурм");
            int fe = ps5.IndexOf("covering zone:brainstorm");
            if (fr + fe < 0) { isBrain = false; }
            isStorm = true;
            fr = ps5.IndexOf("<td>последовательность прохождения:штурмовая</td>");
            fe = ps5.IndexOf("<td>the levels passing sequence:storm</td>");
            if (fr + fe < 0) { isStorm = false; }
        }

        /// <summary>
        /// парсинг страницы со списком игр
        /// </summary>
        /// <param name="g">текст страницы</param>
        /// <returns>текст страницы</returns>
        private static string RemoveTags(string g)
        {
            g = ParsePage.ParseTags(g, tags4list);
            g = g.Replace("&nbsp;", " ").Replace("&quot;", "\"").Replace("\t", " ").Replace("\n", " ").Replace("\r", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            return g;
        }

        /// <summary>
        /// ивент по кнопке выбора игры
        /// </summary>
        public static void Event_SelectGameFromList(object sender, EventArgs e)
        {
            ListBox l4 = (ListBox)sender;
            tbGname.Text = g_urls[l4.SelectedIndex];
        }

    }
}
