// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Threading.Tasks;

namespace Solver2
{
    class GapoifikaBooks
    {
        //
        // public bool Process(OneTab T)    - поток
        // public GapoifikaBooks(OneTab T)  - конструктор
        //

        OneTab OT;
        string task;

        // решение гапоифики
        public bool Process(OneTab T)
        {
            OT = T;
            task = OT.tbTextTask.Text;
            string[] ar1 = System.Text.RegularExpressions.Regex.Split(task, "\r\n");
            foreach(string s1 in ar1)
            {
                string s = Books.ClearBookName(s1).ToLower();
                if (s != "")
                {
                    for(int i=0; i<Books.WordsCount; i++)
                    {
                        if(s == Books.gapo[i].ToLower())
                        {
                            Answer.Add(T, 3, Books.dict[i], -1);
                        }
                    }
                }
            }

            T.tbSectors.Invoke(new Action(() => { T.btSolve.Enabled = true; }));
            return true;
        }

        // создание потока для гапоифики книг
        public GapoifikaBooks(OneTab T)
        {
            Log.Write("gaB Начали решать книжную гапоифику\r\n.\r\n");
            Task<bool> t1 = Task<bool>.Factory.StartNew(() => Process(T));
        }
    }
}
