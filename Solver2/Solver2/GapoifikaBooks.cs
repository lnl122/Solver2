using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
