// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solver2
{
    class Answer
    {
        //
        // public static bool Process()                                                                             - поток
        // public static void Init()                                                                                - инит потока
        // public static string SetProtect(string wrd, int i1, string protect)                                      - устанавливаем защиту
        // public static void Add(OneTab T, int priority, string wrd, int i1, int i2 = -1, int i3 = -1)             - добавляем в очередь
        // public static void Add(OneTab T, int priority, List<string> WordsList, int i1, int i2 = -1, int i3 = -1) - добавляем в очередь список
        //

        public struct Answ
        {
            public OneTab OT;
            public System.Windows.Forms.TabPage Tab;
            public System.Windows.Forms.TextBox Sec;
            public System.Windows.Forms.TextBox Bon;
            public System.Windows.Forms.WebBrowser Web;
            public System.Windows.Forms.TextBox Txt;
            public string wrd;
            public string wrd2;
            public int lvlnum;
            public int i1;
            public int i2;
            public int i3;
            public int priority;
        }
        public static List<Answ> Queue;
        public static int current_level;

        // процесс вбивания. единственный и неповторимый
        public static bool Process()
        {
            // здесь нужно в бесконечном цикле просматривать очередь, выбирать наиболее приоритетные ответы и вбивать их
            // после любых вбитий - перечитывать парсенные данные и обновлять ГУИ
            // после успешных - обновлять все прочее
            bool isTrue = true;
            while (isTrue == true)
            {
                while (Queue.Count < 1) { System.Threading.Thread.Sleep(1000); }
                // получим следующее
                Answ q1 = GetNext();
                // проверим, ранее вбивали или нет
                Level lvl = q1.OT.level;
                int oldsecbon = lvl.secbon;
                bool isAlreadyExist = checkAlreadyExist(q1, lvl);
                if (!isAlreadyExist)
                {
                    // попробуем вбить
                    string p1 = Engine.TryOne(q1.lvlnum, q1.wrd2);
                    lvl.UpdateAnswersLevel(p1);
                    q1.OT.level = lvl;
                    // проверим, наш ответ удачен или нет?
                    bool isYes = false;
                    if (lvl.answers_good.Contains(q1.wrd2)) { isYes = true; }
                    // были изменения секторов и бонусов?
                    if ((isYes) || (oldsecbon != lvl.secbon))
                    {
                        //очистим очередь от а) ответов с тем же ид б) от ответов с ид = номерам решенных секторов для картинок
                        ClearQueue(q1);
                        //надо обновить GUI
                        string sec1 = ""; for (int i = 0; i < lvl.sectors; i++) { sec1 = sec1 + (i + 1).ToString() + ": " + lvl.sector[i] + "\r\n"; }
                        q1.OT.tbSectors.Invoke(new Action(() => { q1.OT.tbSectors.Text = sec1; }));
                        string bon1 = ""; for (int i = 0; i < lvl.bonuses; i++) { bon1 = bon1 + (i + 1).ToString() + ": " + lvl.bonus[i] + "\r\n"; }
                        q1.OT.tbBonuses.Invoke(new Action(() => { q1.OT.tbBonuses.Text = bon1; }));
                        // подчистить картинки в ГУИ
                        if (q1.OT.isPicsSect)
                        {
                            string html = "";
                            q1.OT.wbPictures.Invoke(new Action(() => { html = q1.OT.wbPictures.DocumentText; }));
                            string html2 = UpdateHtmlPics(html, lvl);
                            q1.OT.wbPictures.Invoke(new Action(() => { q1.OT.wbPictures.DocumentText = html2; }));
                        }

                    }
                    //нужно выждать какое-то время
                    //System.Threading.Thread.Sleep(rnd1.Next(rnd_min, rnd_max));
                }
            }
            return true;
        }

        // обновляет html на экране
        private static string UpdateHtmlPics(string html, Level q)
        {
            string page = html;
            for(int i=0; i<q.sector.Length; i++) // поменять на сектора/бонусы/опцианально
            {
                string s = q.sector[i];
                string comment = "<!-- " + (i+1).ToString() + " -->";
                int ii1 = page.IndexOf(comment);
                if (s == "")
                { // сектор не решен, надо указать ответы, вбитые и те, которые будем вбивать, если такая картинка есть
                    if (ii1 != -1)
                    {/*
                        string p1 = page.Substring(0, ii1);
                        int ii2 = p1.LastIndexOf("alt=\"");
                        p1 = p1.Substring(0, ii2 + 5);
                        string p2 = page.Substring(p1.Length + 1);
                        int ii3 = p2.IndexOf("\"");
                        p2 = p2.Substring(ii3);
                        string ans = "";
                        foreach(Answ aa in Queue)
                        {
                            if ((aa.lvlnum == q.number) && ((aa.i1 == i) || (aa.i2 == i) || (aa.i3 == i)))
                            {
                                ans = ans + aa.wrd + " ";
                            }
                        }
                        page = p1 + ans + p2;*/
                    }
                }
                else
                { // сектор решен, убрать картинку, если она есть
                    if (ii1 != -1)
                    {
                        string p1 = page.Substring(0, ii1);
                        int ii2 = p1.LastIndexOf("<img");
                        p1 = p1.Substring(0, ii2);
                        string p2 = page.Substring(ii1 + comment.Length);
                        page = p1 + p2;
                    }
                }
            }
            return page;
        }

        // очищает очередь от всех ответов, где присутствуют ид из q1
        // вход - ответ, по ид которого очищается очередь
        private static void ClearQueue(Answ q1)
        {
            int i1 = q1.i1;
            int i2 = q1.i2;
            int i3 = q1.i3;
            int j1, j2, j3;
            int cnt = Queue.Count;
            for (int i = cnt-1; i >= 0; i--)
            {
                j1 = Queue[i].i1;
                j2 = Queue[i].i2;
                j3 = Queue[i].i3;
                bool fl = false;
                if ((i1 != -1) && (i1 == j1)) { fl = true; }
                if ((i1 != -1) && (i1 == j2)) { fl = true; }
                if ((i1 != -1) && (i1 == j3)) { fl = true; }
                if ((i2 != -1) && (i2 == j1)) { fl = true; }
                if ((i2 != -1) && (i2 == j2)) { fl = true; }
                if ((i2 != -1) && (i2 == j3)) { fl = true; }
                if ((i3 != -1) && (i3 == j1)) { fl = true; }
                if ((i3 != -1) && (i3 == j2)) { fl = true; }
                if ((i3 != -1) && (i3 == j3)) { fl = true; }
                if (fl) { Queue.RemoveAt(i); }
            }
            // почистим по номерам бонусов и секторов, если необходимо
            if (q1.OT.isPicsSect)
            {
                for (int i = 0; i < q1.OT.level.bonuses; i++)
                {
                    string s1 = q1.OT.level.bonus[i];
                    string[] ar1 = s1.Split(' ');
                    foreach(string s2 in ar1)
                    {
                        if((s2 == q1.wrd) || (s2 == q1.wrd2))
                        {
                            int cnt2 = Queue.Count;
                            int jj1, jj2, jj3;
                            for (int ii = cnt2-1; ii >= 0; ii--)
                            {
                                jj1 = Queue[ii].i1;
                                jj2 = Queue[ii].i2;
                                jj3 = Queue[ii].i3;
                                if (i == jj1) { Queue.RemoveAt(ii); }
                            }
                        }
                    }
                }
                for (int i = 0; i < q1.OT.level.sectors; i++)
                {
                    string s1 = q1.OT.level.sector[i];
                    string[] ar1 = s1.Split(' ');
                    foreach (string s2 in ar1)
                    {
                        if ((s2 == q1.wrd) || (s2 == q1.wrd2))
                        {
                            int cnt2 = Queue.Count;
                            int jj1, jj2, jj3;
                            for (int ii = cnt2 - 1; ii >= 0; ii--)
                            {
                                jj1 = Queue[ii].i1;
                                jj2 = Queue[ii].i2;
                                jj3 = Queue[ii].i3;
                                if (i == jj1) { Queue.RemoveAt(ii); }
                            }
                        }
                    }
                }
            }
        }

        // проверяет, ранее уже вбивали это слово или нет
        // вход - структура ответа, структура уровня
        // выход - флаг присутствия ответа в списках
        private static bool checkAlreadyExist(Answ q1, Level lvl)
        {
            List<string> old = new List<string>();
            old.AddRange(lvl.answers_bad);
            old.AddRange(lvl.answers_good);
            foreach (string s in lvl.bonus) { if (s != "") { old.Add(s); } }
            foreach (string s in lvl.sector) { if (s != "") { old.Add(s); } }
            bool f1 = old.Contains(q1.wrd);
            bool f2 = old.Contains(q1.wrd2);
            return (f1 || f2);
        }

        // возвращает наиболее приоритетный ответ
        private static Answ GetNext()
        {
            int prior = 999;
            int i_prior = -1;
            int i_level = -1;
            int i_choice = -1;
            int cnt = Queue.Count;
            for(int i=0; i<cnt; i++)
            {
                Answ q1 = Queue[i];
                if (q1.priority < prior) { prior = q1.priority; i_prior = i; i_level = -1; }
                if ((q1.lvlnum == current_level) && (q1.priority == prior)) { i_level = i; }
            }
            if (i_level == -1) { i_choice = i_prior; } else { i_choice = i_level; }
            Answ q2 = Queue[i_choice];
            Queue.Remove(q2);
            if (current_level != q2.lvlnum) { current_level = q2.lvlnum; }
            return q2;
        }

        // инициализация + старт процесса вбиватора
        public static void Init()
        {
            Queue = new List<Answ>();
            current_level = -1;
            Task<bool> t1 = Task<bool>.Factory.StartNew(() => Process());
        }

        // уствнавливает защиту слова, если это необходимо
        // вход - слово, сектор/номер, защита
        // выход - слово
        public static string SetProtect(string wrd, int i1, string protect)
        {
            string wrd2 = wrd;
            string istr = i1.ToString();
            if (protect == "1слово")
            {
                wrd2 = istr + wrd2;
            }
            else
            {
                if (protect == "слово1")
                {
                    wrd2 = wrd2 + istr;
                }
                else
                {
                    if (protect == "01слово")
                    {
                        if (istr.Length == 1)
                        {
                            wrd2 = "0" + istr + wrd2;
                        }
                        else
                        {
                            wrd2 = istr + wrd2;
                        }
                    }
                    else
                    {
                        if (protect == "слово01")
                        {
                            if (istr.Length == 1)
                            {
                                wrd2 = wrd2 + "0" + istr;
                            }
                            else
                            {
                                wrd2 = wrd2 + istr;
                            }
                        }
                    }
                }
            }
            return wrd2;
        }

        // постановка слова в очередь
        // вход - Таб, приоритет, слово, 3 номера картинок
        // выход - нет, слово ставиться в очередь
        public static void Add(OneTab T, int priority, string wrd, int i1, int i2 = -1, int i3 = -1)
        {
            if ((i1 != 0)&&(i2 != 0)&&(i3 != 0)) {
                int lvlnum = T.level.number;
                string wrd2 = SetProtect(wrd, i1, T.sProtect);
                Answ q1 = new Answ();
                q1.OT = T;
                q1.Tab = T.Tab;
                q1.Sec = T.tbSectors;
                q1.Bon = T.tbBonuses;
                q1.Web = T.wbPictures;
                q1.Txt = T.tbTextHints;
                q1.wrd = wrd;
                q1.wrd2 = wrd2;
                q1.lvlnum = T.level.number;
                q1.i1 = i1;
                q1.i2 = i2;
                q1.i3 = i3;
                q1.priority = priority;
                Queue.Add(q1);
            }
    }

        // заглушка для получения списка
        public static void Add(OneTab T, int priority, List<string> WordsList, int i1, int i2 = -1, int i3 = -1)
        {
            foreach(string word in WordsList) { Add(T, priority, word, i1, i2, i3); }
        }
    }
}
