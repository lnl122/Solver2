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
        /// <summary>
        /// для двух наборов слов пробует решить метаграмму/логогрив/букарик
        /// </summary>
        /// <param name="w1">первый набор слов</param>
        /// <param name="w2">второй набор слов</param>
        /// <param name="i1">кол-во букв для замены в 1м слове</param>
        /// <param name="i2">кол-во букв для замены в 2м слове</param>
        /// <returns>набор слов, соответствующий критериям</returns>
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
