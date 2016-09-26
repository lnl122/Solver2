using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solver2
{
    class GapoifikaFilms
    {
        OneTab OT;
        string task;

        // решение гапоифики фильмов
        public bool Process(OneTab T)
        {
            OT = T;
            task = OT.tbTextTask.Text;
            string[] ar1 = System.Text.RegularExpressions.Regex.Split(task, "\r\n");
            foreach (string s1 in ar1)
            {
                if (s1 != "")
                {
                    // получим возможные наименвоания фильмов
                    List<string> names = Films.GetNameList(s1);
                    // и маску
                    string mask = Films.PrepareSearchString(s1);

                    foreach(string name in names)
                    {
                        string n = name;
                        // проверка каждого из названий
                        // предварительно убрать цифры в скобках

                        // ???

                        // затем спецсимволы !"№;%:?*() и 

                        n = Books.ClearBookName(n);

                        // проверяем, если соответствует - передаем во вбиватор

                    }
                }
            }

            T.tbSectors.Invoke(new Action(() => { T.btSolve.Enabled = true; }));
            return true;
        }

        // создание потока для гапоифики фильмов
        public GapoifikaFilms(OneTab T)
        {
            Log.Write("gaF Начали решать гапоифику по фильмам\r\n.\r\n");
            Task<bool> t1 = Task<bool>.Factory.StartNew(() => Process(T));
        }
    }
}
