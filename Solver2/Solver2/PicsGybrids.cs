// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solver2
{
    //
    // public bool Process(OneTab T)        - процесс
    // public PicsGybrids(OneTab T)         - конструктор
    //

    class PicsGybrids
    {
        public OneTab OT;

        /// <summary>
        /// ищем гибриды по двум спискам слов
        /// </summary>
        /// <param name="OT">таб</param>
        /// <param name="w1">список1</param>
        /// <param name="w2">список2</param>
        /// <param name="i">номер первого слова</param>
        /// <param name="j">номер второго слова</param>
        /// <param name="min">минимальное пересечение, букв</param>
        /// <param name="prior">приоритет</param>
        /// <returns>строка ответов с разделителем \r\n</returns>
        private static string FindGybrids(OneTab OT, List<string> w1, List<string> w2, int i, int j, int min, int prior)
        {
            string r2 = "";
            foreach (string s1 in w1)
            {
                foreach (string s2 in w2)
                {
                    int mi = min;
                    int ma = s1.Length - 1;
                    if (s1.Length > s2.Length) { ma = s2.Length - 1; }
                    if (ma < mi) { continue; }
                    for (int k = mi; k <= ma; k++)
                    {
                        // s1 - первые символы (второе слово), s2 - последние символы (первое слово)
                        if (s1.Substring(0, k) == s2.Substring(s2.Length - k, k))
                        {
                            string ans = s2.Substring(0, s2.Length - k) + s1;
                            Answer.Add(OT, prior, ans, i, j);
                            r2 = r2 + ans + "\r\n";
                        }
                    }
                }
            }
            return r2;
        }

        /// <summary>
        /// распознание и решение гибридов картинок
        /// </summary>
        /// <param name="T">таб</param>
        /// <returns>true</returns>
        public bool Process(OneTab T)
        {
            OT = T;
            string resout = "";

            // создаем массив путей из больших картинок
            string[] SmallImagePath = Image.GetSmallImagePathes(T, T.level.urls, T.iRows, T.iCols);

            // отрисуем их в ГУИ
            string html = Picture.GetPicsHtml(SmallImagePath);
            T.wbPictures.Invoke(new Action(() => { T.wbPictures.DocumentText = html; }));

            // из путей к картинкам делаем коллекции слов
            List<Words> TextsFromPics = Image.GetAllDescriptions(SmallImagePath);

            // решаем, собственно
            int min = T.iGybridMin;
            int cnt = TextsFromPics.Count;
            for(int i=0; i<cnt; i++)
            {
                for(int j=0; j<cnt; j++)
                {
                    if(i == j) { continue; }
                    // надо сравнить наборы слов для i и j
                    Words w1 = TextsFromPics[i];
                    Words w2 = TextsFromPics[j];

                    // приоритет 3 - для гибридов по базовым словам
                    // приоритет 4 - для остальных
                    // приоритет 5 - для ассоциаций ко всему найденному
                    if ((w1 != null) && (w2 != null))
                    {
                        if ((w1.all_base != null) && (w2.all_base != null))
                        {
                            string resout2 = FindGybrids(T, w1.all_base, w2.all_base, i, j, min, 3);
                            resout = resout + resout2 + "\r\n";
                            /*
                            List<string> ls1 = new List<string>();
                            List<string> ls2 = new List<string>();
                            ls1.AddRange(w1.all_base); ls1.AddRange(w1.all_assoc25);
                            ls2.AddRange(w2.all_base); ls2.AddRange(w2.all_assoc25);
                            //ls1.AddRange(w1.all_base); ls1.AddRange(w1.all_assoc25); ls1.AddRange(w1.ru_check); ls1.AddRange(w1.en_trans); ls1.AddRange(w1.f_b_noun); ls1.AddRange(w1.f_b_adjective);
                            //ls2.AddRange(w2.all_base); ls2.AddRange(w2.all_assoc25); ls2.AddRange(w2.ru_check); ls2.AddRange(w2.en_trans); ls2.AddRange(w2.f_b_noun); ls2.AddRange(w2.f_b_adjective);
                            ls1 = Words.KillDupesAndRange(ls1);
                            ls2 = Words.KillDupesAndRange(ls2);
                            FindGybrids(T, ls1, ls2, i, j, min, 4);

                            иначе получаем чрезмерно много вариантов
                            */
                        }
                    }
                    //List<string> ass1 = Words.KillDupesAndRange(Associations.Get(ls1));
                    //List<string> ass2 = Words.KillDupesAndRange(Associations.Get(ls2));
                    //FindGybrids(T, ass1, ass2, i, j, min, 5);
                    //иначе получаем чрезмерно много вариантов

                }
            }

            T.tbSectors.Invoke(new Action(() => { T.btSolve.Enabled = true; }));
            T.tbTextHints.Invoke(new Action(() => { T.tbTextHints.Text = resout; }));
            T.tcTabText.Invoke(new Action(() => { T.tcTabText.SelectTab(1); }));

            return true;
        }

        /// <summary>
        /// создание потока
        /// </summary>
        /// <param name="T">таб</param>
        public PicsGybrids(OneTab T)
        {
            if (T.cbImageCuttingMethod.SelectedItem.ToString() == "Указан в ручную, равные доли")
            {
                Log.Write("PiGy Начали решать картинки\r\n.\r\n");
                Task<bool> t1 = Task<bool>.Factory.StartNew(() => Process(T));
            }
        }
    }
}
