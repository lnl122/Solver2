using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solver2
{
    class PicsGybrids
    {
        // распознание и решение гибридов картинок
        public bool Process(OneTab T)
        {
            // создаем массив путей из больших картинок
            string[] SmallImagePath = Image.GetSmallImagePathes(T, T.level.urls, T.iRows, T.iCols);

            // отрисуем их в ГУИ
            string html = Picture.GetPicsHtml(SmallImagePath);
            T.wbPictures.Invoke(new Action(() => { T.wbPictures.DocumentText = html; }));

            // из путей к картинкам делаем коллекции слов
            List<Words> TextsFromPics = Image.GetAllDescriptions(SmallImagePath);

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


                }
            }

            // пробуем вбить ответы (то есть передать их во вбиватор, включая указание приоритета)
            /*for (int i = 0; i < TextsFromPics.Count; i++)
            {
                Words W = TextsFromPics[i];
                if (W != null)
                {
                    Answer.Add(T, 4, W.all_base10, i);
                }
                //*** позже добавить более низкие приоритеты
            }
            */
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
