﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solver2
{
    class Picture
    {
        //private OneTab T;
        
        // распознание и вбивание картинок
        public bool Process(OneTab T)
        {
            // создаем массив путей из больших картинок
            string[] SmallImagePath = Image.GetSmallImagePathes(T, T.level.urls, T.iRows, T.iCols);

            // из путей к картинкам делаем коллекции слов
            List<Words> TextsFromPics = Image.GetAllDescriptions(SmallImagePath);

            // пробуем вбить ответы (то есть передать их во вбиватор, включая указание приоритета)
            for(int i=0; i<TextsFromPics.Count; i++)
            {
                Words W = TextsFromPics[i];
                Answer.Add(T, 4, W.all_base10, i);
            }

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