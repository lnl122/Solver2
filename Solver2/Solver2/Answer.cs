using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solver2
{
    class Answer
    {
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
                // попробуем вбить
                string p1 = Engine.TryOne(q1.lvlnum, q1.wrd2);
                Level lvl = q1.OT.level;
                int oldsecbon = lvl.secbon;
                lvl.UpdateAnswersLevel(p1);
                // проверим, наш ответ удачен или нет?
                bool isYes = false;
                if (lvl.answers_good.Contains(q1.wrd2)) { isYes = true; }
                // были изменения секторов и бонусов?
                if((isYes) || (oldsecbon != lvl.secbon))
                {
                    //надо обновить GUI
                    string sec1 = ""; for (int i = 0; i < lvl.sectors; i++) { sec1 = sec1 + (i + 1).ToString() + ": " + lvl.sector[i] + "\r\n"; }
                    q1.OT.tbSectors.Invoke(new Action(() => { q1.OT.tbSectors.Text = sec1; }));
                    string bon1 = ""; for (int i = 0; i < lvl.bonuses; i++) { bon1 = bon1 + (i + 1).ToString() + ": " + lvl.bonus[i] + "\r\n"; }
                    q1.OT.tbBonuses.Invoke(new Action(() => { q1.OT.tbBonuses.Text = bon1; }));

                }
                //нужно выждать какое-то время
                //System.Threading.Thread.Sleep(rnd1.Next(rnd_min, rnd_max));
            }
            return true;
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

        // заглушка для получения списка
        public static void Add(OneTab T, int priority, List<string> WordsList, int i1, int i2 = -1, int i3 = -1)
        {
            foreach(string word in WordsList) { Add(T, priority, word, i1, i2, i3); }
        }
    }
}
