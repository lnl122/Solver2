﻿// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solver2
{
    class GapoifikaFilms
    {
        //
        // public GapoifikaFilms(OneTab T)  - конструктор
        // public bool Process(OneTab T)    - поток
        //

        OneTab OT;
        string task;

        /// <summary>
        /// решение гапоифики
        /// </summary>
        /// <param name="T">таб</param>
        /// <returns>true</returns>
        public bool Process(OneTab T)
        {
            OT = T;
            task = OT.tbTextTask.Text;
            string resout = "";
            string[] ar1 = System.Text.RegularExpressions.Regex.Split(task, "\r\n");
            foreach (string s1 in ar1)
            {
                if (s1 != "")
                {
                    // получим возможные наименвоания фильмов
                    List<string> names = Films.GetNameList(s1);
                    // и маску в формате "га по и фи ка"
                    string mask = Films.PrepareSearchString(s1);
                    mask = mask.ToLower().Replace("  ", " ").Replace("  ", " ").Trim();

                    foreach (string name in names)
                    {
                        // уберем цифры в скобках
                        string n = RemoveYearBrackets(name);
                        n = RemoveYearBrackets(n);
                        // уберем спецсимволы
                        n = n.Replace("`", " ").Replace("~", " ").Replace("!", " ").Replace("@", " ").Replace("#", " ").Replace("№", " ").Replace("\"", " ");
                        n = n.Replace(";", " ").Replace("$", " ").Replace("%", " ").Replace("^", " ").Replace(":", " ").Replace("&", " ").Replace("?", " ");
                        n = n.Replace("*", " ").Replace("(", " ").Replace(")", " ").Replace("-", " ").Replace("_", " ").Replace("=", " ").Replace("+", " ");
                        n = n.Replace("{", " ").Replace("}", " ").Replace("[", " ").Replace("]", " ").Replace("/", " ").Replace("|", " ").Replace("\\", " ");
                        n = n.Replace(";", " ").Replace(":", " ").Replace("'", " ").Replace(",", " ").Replace(".", " ").Replace("<", " ").Replace(">", " ");
                        n = n.Replace("«", " ").Replace("»", " ");
                        n = n.Replace("…", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
                        // составим из очередного найденного названия маску в стиле "га по и фи ка"
                        string[] ar2 = n.Split(' ');
                        string nn = "";
                        foreach (string s in ar2)
                        {
                            if (s == "") { continue; }
                            if (s.Length == 1)
                            {
                                nn = nn + s + " ";
                            }
                            else
                            {
                                nn = nn + s.Substring(0, 2) + " ";
                            }
                        }
                        nn = nn.ToLower().Replace("  ", " ").Replace("  ", " ").Trim();
                        // проверяем, если соответствует - передаем во вбиватор
                        if (nn == mask)
                        {
                            Answer.Add(T, 3, n, -1);
                            resout = resout + n + "\r\n";
                        }
                    }
                }
                resout = resout + "\r\n";
            }

            T.tbSectors.Invoke(new Action(() => { T.btSolve.Enabled = true; }));
            T.tbTextHints.Invoke(new Action(() => { T.tbTextHints.Text = resout; }));
            T.tcTabText.Invoke(new Action(() => { T.tcTabText.SelectTab(1); }));
            return true;
        }

        /// <summary>
        /// убирает цифры в скобках из строки
        /// например: Название(2016)Текст -> НазваниеТекст
        /// </summary>
        /// <param name="t2">название фильма</param>
        /// <returns>название фильма без скобок</returns>
        private string RemoveYearBrackets(string t2)
        {
            string t = "  " + t2 + "  ";
            string res = t2;
            int ii1 = t.IndexOf("(");
            int ii2 = t.IndexOf(")");
            if ((ii1 == -1) || (ii2 == -1)) { return res; }
            if (ii1 > ii2) { return res; }
            if (ii2 - ii1 - 1 <= 0) { return res; }

            string num = t.Substring(ii1+1, ii2-ii1-1);
            int number;
            bool result = Int32.TryParse(num, out number);
            if (result)
            {
                res = t.Substring(0, ii1) + t.Substring(ii2 + 1);
            }
            res = res.Trim();
            return res;
        }

        /// <summary>
        /// создание потока для гапоифики фильмов
        /// </summary>
        /// <param name="T">таб</param>
        public GapoifikaFilms(OneTab T)
        {
            Log.Write("gaF Начали решать гапоифику по фильмам\r\n.\r\n");
            Task<bool> t1 = Task<bool>.Factory.StartNew(() => Process(T));
        }
    }
}
