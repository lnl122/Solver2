using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solver2
{
    class Picture
    {
        // распознание и вбивание картинок
        public bool Process(OneTab T)
        {
            // создаем массив путей из больших картинок
            string[] SmallImagePath = Image.GetSmallImagePathes(T, T.level.urls, T.iRows, T.iCols);

            // отрисуем их в ГУИ
            string html = "<!DOCTYPE HTML><html><head><meta charset=\"utf-8\"></head><body bgcolor=\"#111111\"><br/>";
            for(int i=0; i<SmallImagePath.Length; i++)
            {
                html = html + "<img src=\"" + SmallImagePath[i] + "\" width=\"150\" height=\"150\" alt=\"" + (i+1).ToString() + "\"><!-- "+ (i+1).ToString() + " -->";
            }
            html = html + "<br/></body></html>";
            T.wbPictures.Invoke(new Action(() => { T.wbPictures.DocumentText = html; }));

            // из путей к картинкам делаем коллекции слов
            List<Words> TextsFromPics = Image.GetAllDescriptions(SmallImagePath);

            // пробуем вбить ответы (то есть передать их во вбиватор, включая указание приоритета)
            for(int i=0; i<TextsFromPics.Count; i++)
            {
                Words W = TextsFromPics[i];
                if (W != null)
                {
                    Answer.Add(T, 4, W.all_base10, i);
                }
                //*** позже добавить более низкие приоритеты
            }
            T.tbSectors.Invoke(new Action(() => { T.btSolve.Enabled = true; }));
            
            return true;
        }

        // разпознать и вбить слова от картинок, создание потока
        public Picture(OneTab T)
        {
            if (T.cbImageCuttingMethod.SelectedItem.ToString() == "Указан в ручную, равные доли")
            {
                Log.Write("Pics Начали решать картинки\r\n.\r\n");
                Task<bool> t1 = Task<bool>.Factory.StartNew(() => Process(T));
            }
        }
    }
}