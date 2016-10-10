using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solver2
{
    class WordCompare
    {
        // для двух наборов слов пробует решить метаграмму/логогрив/букарик
        // вход - два набора слов, два числа - суть количества букв, которые нужно менять в каждом из слов из наборов
        // выход - набор слов, соответствующий критериям
        public List<string> Cmp(List<string> w1, List<string> w2, int i1, int i2)
        {
            // для метаграммы - и1 = 1, и2 = 1
            // для логогрифа - и1 = 0, и2 = 1
            // для букарика - и1 = 1, и2 = 2
            // проверять нужно и w1-w2 и w2-w1
            List<string> res = new List<string>();



            return res;
        }
    }
}
