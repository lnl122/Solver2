using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solver2
{
    class Olimp
    {
        // решение олимпиек
        public bool Process(OneTab T)
        {
            bool isTrue = true;
            while (isTrue == true)
            {

            }

            //T.tbSectors.Invoke(new Action(() => { T.btSolve.Enabled = true; }));
            return true;
        }

        // разпознать и вбить слова от картинок, создание потока
        public Olimp(OneTab T)
        {
            Log.Write("Oli Начали решать олимпийки\r\n.\r\n");
            Task<bool> t1 = Task<bool>.Factory.StartNew(() => Process(T));
        }
    }
}
