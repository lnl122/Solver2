// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Solver2
{
    class Raschl
    {
        OneTab OT;
        string task;
        int slov;

        struct OneStr
        {
            public string[] str;
            public int[] num;
        }                                             // струкрура данных для одной строчки расчлененок

        /// <summary>
        /// решает одну расчлененку, это уже отдельный поток
        /// </summary>
        /// <param name="s1">строка в формате "строитель(3)блеф(2)картон(2)</param>
        /// <param name="slov">количество слов</param>
        /// <returns></returns>
        private List<string> ProcessOne(string s1, int slov)
        {
            List<string> res = new List<string>();
            if (s1.Trim().Length == 0) { return res; }
            OneStr os = Prepare(s1);
            string[] wrds = Transposition(os);
            List<string> wrds2 = Spaces(wrds, slov);
            SpellChecker sc = new SpellChecker();
            res = sc.Check(wrds2);
            Answer.Add(OT, 3, res, -1);
            return res;
        }

        /// <summary>
        /// добавляет необходимое число пробелов в получившиеся слова
        /// </summary>
        /// <param name="w">массив слов</param>
        /// <param name="s">кол-во слов (для пробелов)</param>
        /// <returns>список слов с пробелами, если они нужны</returns>
        private List<string> Spaces(string[] w, int s)
        {
            List<string> res = new List<string>();
            if (w.Length == 0) { return res; }
            // если слово одно - не паримся вообще
            if (s == 1)
            {
                foreach(string s1 in w) { res.Add(s1); }
                return res;
            }
            // если слов 2?
            if (s == 2)
            {
                // один пробел
                foreach (string s1 in w)
                {
                    int l1 = s1.Length;
                    for(int i = 1; i < l1 - 1; i++)
                    {
                        string s2 = s1.Substring(0, i) + ' ' + s1.Substring(i);
                        res.Add(s2);
                    }
                }
                return res;
            }
            // если слов 3?
            if (s == 3)
            {
                // два пробела
                foreach (string s1 in w)
                {
                    int l1 = s1.Length;
                    for (int i = 1; i < l1 - 2; i++)
                    {
                        string s2 = s1.Substring(0, i) + ' ';
                        string s3 = s1.Substring(i);
                        for(int j = 1; j < s3.Length - 1; j++)
                        {
                            string s4 = s3.Substring(0, j) + ' ' + s3.Substring(j);
                            string s5 = s2 + s4;
                            res.Add(s5);
                        }
                    }
                }
                return res;
            }
            //заглушка для прочих случаев - нужна или нет?
            //foreach (string s1 in w) { res.Add(s1); }
            return res;
        }

        /// <summary>
        /// готовит набор вероятных слов
        /// </summary>
        /// <param name="d">структура расчлененки</param>
        /// <returns>массив слов</returns>
        private string[] Transposition(OneStr d)
        {
            if (d.num.Length == 0) { return new string[0]; }

            int words = d.str.Length;
            // d. = .str[], .num[]
            // перебирать все варианты, проверять орфографию каждого
            int[] cur = new int[words]; // текущие координаты
            int[] sta = new int[words]; // длинна слов
            int total = 1;
            for (int i = 0; i < words; i++)
            {
                cur[i] = 0;
                sta[i] = d.str[i].Length - d.num[i]; // максимальное начало строки, с нуля 0..ххх
                total = total * (sta[i] + 1);
            }
            string[] allwrds = new string[total];
            int curwrd = 0;
            while (cur[words - 1] <= sta[words - 1])
            {
                string r2 = "";
                for (int i = 0; i < words; i++)
                {
                    r2 = r2 + d.str[i].Substring(cur[i], d.num[i]);
                }
                allwrds[curwrd] = r2;
                curwrd++;
                cur[0]++;
                for (int i = 0; i < words - 1; i++)
                {
                    if (cur[i] > sta[i])
                    {
                        cur[i] = 0;
                        cur[i + 1]++;
                    }
                }
            }//while

            return allwrds;
        }

        /// <summary>
        /// готовит структуру слов в массива для решения
        /// </summary>
        /// <param name="s1">строка, число слов</param>
        /// <returns>структура из двух массивов</returns>
        private OneStr Prepare(string s1)
        {
            OneStr res = new OneStr();

            string[] t3 = Regex.Split(s1, "\\)");
            string[] wrds = new string[t3.Length - 1];
            int[] nums = new int[t3.Length - 1];
            for (int i = 0; i < t3.Length - 1; i++)
            {
                string t4 = t3[i];
                string[] t5 = Regex.Split(t4, "\\(");
                wrds[i] = t5[0];
                int rr = 0;
                Int32.TryParse(t5[1], out rr);
                nums[i] = rr;
            }
            // собрать данные в структуры по одной на строку
            res.num = nums;
            res.str = wrds;
            return res;
        }

        /// <summary>
        /// нормализует входные данные в формат
        /// "строитель(3)блеф(2)картон(2)#жироприказ(4)слюда(2)чемодан(2)гарнир(2)лезвие(1)#житель(3)тепло(2)рогожа(3)мрак(2)мозг(1)карман(2)##"
        /// </summary>
        /// <param name="d">строка</param>
        /// <returns>нормализованная строка или пустая - если некорректный формат</returns>
        private string Normalize(string d)
        {
            string t0 = d.ToLower().Replace(" ", "").Replace(",", "").Replace("\r\n", "#").Replace("###", "##").Replace("###", "##").Replace("###", "##").Replace("###", "##");
            t0 = (t0 + "##").Replace("###", "##").Replace("###", "##");
            //t1 = строитель(3)блеф(2)картон(2)#жироприказ(4)слюда(2)чемодан(2)гарнир(2)лезвие(1)#житель(3)тепло(2)рогожа(3)мрак(2)мозг(1)карман(2)##
            //t1 = строитель(3)#блеф(2)#картон(2)##жироприказ(4)#слюда(2)#чемодан(2)#гарнир(2)#лезвие(1)##
            // определим тип входного данного
            int s1 = t0.Length - t0.Replace(")", "").Length;        // правые скобки
            int s12 = t0.Length - t0.Replace("(", "").Length;       // левые скобки
            int s2 = (t0.Length - t0.Replace(")#", "").Length) / 2; // после каждой правой скобки - новая строка - сколько раз
            if ((s1 == 0) || (s1 != s12)) { return ""; }            // если нет правых скобок вообще или их количество не равно числу левых скобок
            string[] t2 = System.Text.RegularExpressions.Regex.Split(t0, "\\(");
            int res;
            bool fl = true;
            for (int i = 1; i < t2.Length; i++)
            {
                string[] t4 = System.Text.RegularExpressions.Regex.Split(t2[i], "\\)");
                fl = fl && Int32.TryParse(t4[0], out res);
            }
            if (!fl) { return ""; }                                 // если внутри скобок есть не число
            if (s1 == s2)
            {
                // type строитель(3)#блеф(2)#картон(2)##жироприказ(4)#слюда(2)#чемодан(2)#гарнир(2)#лезвие(1)##
                t0 = t0.Replace("#", "$").Replace("$$", "#").Replace("$", "");
            }
            else
            {
                // type строитель(3)блеф(2)картон(2)#жироприказ(4)слюда(2)чемодан(2)гарнир(2)лезвие(1)#житель(3)тепло(2)рогожа(3)мрак(2)мозг(1)карман(2)##
            }
            t0 = t0.Replace("##", "#").Replace("##", "#");
            return t0; // or "" above by text
        }                   // нормализация вида задачи

        /// <summary>
        /// решение расчлененок
        /// </summary>
        /// <param name="T">таб</param>
        /// <returns>true</returns>
        public bool Process(OneTab T)
        {
            OT = T;
            task = OT.tbTextTask.Text;
            slov = OT.iRaschl;
            string resout = "";
            string norm = Normalize(task);
            string[] ar1 = norm.Split('#');

            // формируем таски, передаём им управление
            var Tasks2 = new List<Task<List<string>>>();
            foreach (string s1 in ar1)
            {
                if (s1.Trim() == "") { continue; }
                Tasks2.Add(Task<List<string>>.Factory.StartNew(() => ProcessOne(s1, slov)));
            }
            
            // дождаться выполнения потоков
            Task.WaitAll(Tasks2.ToArray());

            // собираем результаты
            List<string> SolvedWords = new List<string>();
            foreach (Task<List<string>> t8 in Tasks2)
            {
                List<string> r8 = t8.Result;
                SolvedWords.AddRange(r8);
                SolvedWords.Add("");
            }
            foreach (string s1 in SolvedWords)
            {
                resout = resout + s1 + "\r\n";
            }

            T.tbSectors.Invoke(new Action(() => { T.btSolve.Enabled = true; }));
            T.tbTextHints.Invoke(new Action(() => { T.tbTextHints.Text = resout; }));
            T.tcTabText.Invoke(new Action(() => { T.tcTabText.SelectTab(1); }));
            return true;
        }

        /// <summary>
        /// создание потока для расчлененок
        /// </summary>
        /// <param name="T">таб</param>
        public Raschl(OneTab T)
        {
            Log.Write("Rasch Начали решать расчленки\r\n.\r\n");
            Task<bool> t1 = Task<bool>.Factory.StartNew(() => Process(T));
        }
    }
}
