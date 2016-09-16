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


            return true;
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
            string wrd2 = SetProtect(wrd, i1, T.cbProtect.SelectedItem.ToString());
            Answ q1 = new Answ();
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
