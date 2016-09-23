using System;
using System.Threading.Tasks;

namespace Solver2
{
    class GapoifikaBooks
    {
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
                string s = s1.Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", "").Replace("\"", "").Replace("!", "").Replace("?", "").Replace("#", "");
                s = s.Replace(":", "").Replace(";", "").Replace("%", "").Replace("(", "").Replace(")", "").Replace("+", "").Replace("/", "").Replace("\\", "");
                if (s != "")
                {
                    for(int i=0; i<Books.WordsCount; i++)
                    {
                        if(s.ToLower() == Books.gapo[i].ToLower())
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
