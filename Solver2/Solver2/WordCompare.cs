// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using Lstr = System.Collections.Generic.List<string>;
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
        public Lstr Cmp(Lstr w1, Lstr w2, int i1, int i2)
        {
            // для метаграммы - и1 = 1, и2 = 1
            // для логогрифа - и1 = 0, и2 = 1
            // для букарика - и1 = 1, и2 = 2
            // проверять нужно и w1-w2 и w2-w1
            Lstr res = new Lstr();



            return res;
        }
    }
}
