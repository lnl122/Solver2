using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solver2
{
    class PicsGybrids
    {
        public OneTab OT;
        // ищем ответ по двум спискам слов
        // вход - список, список, номер, номер, минимальная длина пересечения, приоритет
        // выхода нет - передаются во вбиватор
        private static void FindGybrids(OneTab OT, List<string> w1, List<string> w2, int i, int j, int min, int prior)
        {
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
                        }
                    }
                }
            }
        }

        // распознание и решение гибридов картинок
        public bool Process(OneTab T)
        {
            OT = T;

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
                    FindGybrids(T, w1.all_base, w2.all_base, i, j, min, 3);

                    List<string> ls1 = new List<string>();
                    List<string> ls2 = new List<string>();
                    ls1.AddRange(w1.all_base); ls1.AddRange(w1.all_assoc25);
                    ls2.AddRange(w2.all_base); ls2.AddRange(w2.all_assoc25);
                    //ls1.AddRange(w1.all_base); ls1.AddRange(w1.all_assoc25); ls1.AddRange(w1.ru_check); ls1.AddRange(w1.en_trans); ls1.AddRange(w1.f_b_noun); ls1.AddRange(w1.f_b_adjective);
                    //ls2.AddRange(w2.all_base); ls2.AddRange(w2.all_assoc25); ls2.AddRange(w2.ru_check); ls2.AddRange(w2.en_trans); ls2.AddRange(w2.f_b_noun); ls2.AddRange(w2.f_b_adjective);
                    ls1 = Words.KillDupesAndRange(ls1);
                    ls2 = Words.KillDupesAndRange(ls2);
                    FindGybrids(T, ls1, ls2, i, j, min, 4);

                    //List<string> ass1 = Words.KillDupesAndRange(Associations.Get(ls1));
                    //List<string> ass2 = Words.KillDupesAndRange(Associations.Get(ls2));
                    //FindGybrids(T, ass1, ass2, i, j, min, 5);

                }
            }

            T.tbSectors.Invoke(new Action(() => { T.btSolve.Enabled = true; }));

            return true;
        }

        // создание потока
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
