using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solver2
{
    class Answer
    {
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

        }
        // заглушка для получения списка
        public static void Add(OneTab T, int priority, List<string> WordsList, int i1, int i2 = -1, int i3 = -1)
        {
            foreach(string word in WordsList) { Add(T, priority, word, i1, i2, i3); }
        }
    }
}
