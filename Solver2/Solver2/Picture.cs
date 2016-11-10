// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solver2
{
    //
    // public static string GetPicsHtml(string[] ar)    - создаёт вэб-страницу из списка локальных путей к картинкам
    // public Picture(OneTab T)                         - конструктор
    // public bool Process(OneTab T)                    - поток
    //

    class Picture
    {
        /// <summary>
        /// создаёт вэб-страницу из списка локальных путей к картинкам
        /// </summary>
        /// <param name="ar">массив путей к локальным картинкам</param>
        /// <returns>текст html</returns>
        public static string GetPicsHtml(string[] ar)
        {
            string html = "<!DOCTYPE HTML><html><head><meta charset=\"utf-8\"></head><body bgcolor=\"#111111\"><br/>";
            for (int i = 0; i < ar.Length; i++)
            {
                html = html + "<img src=\"" + ar[i] + "\" width=\"150\" height=\"150\" alt=\"" + (i + 1).ToString() + "\"><!-- " + (i + 1).ToString() + " -->";
            }
            html = html + "<br/></body></html>";
            return html;
        }

        /// <summary>
        /// распознание и вбивание картинок
        /// </summary>
        /// <param name="T">таб</param>
        /// <returns>true</returns>
        public bool Process(OneTab T)
        {
            string resout = "";
            // создаем массив путей из больших картинок
            string[] SmallImagePath = Image.GetSmallImagePathes(T, T.level.urls, T.iRows, T.iCols);

            // отрисуем их в ГУИ
            string html = GetPicsHtml(SmallImagePath);
            T.wbPictures.Invoke(new Action(() => { T.wbPictures.DocumentText = html; }));
            T.wbPictures.Invoke(new Action(() => { T.tcTabWeb.SelectedIndex = 1; }));

            // из путей к картинкам делаем коллекции слов
            List<Words> TextsFromPics = Image.GetAllDescriptions(SmallImagePath);

            // пробуем вбить ответы (то есть передать их во вбиватор, включая указание приоритета)
            for (int i=0; i<TextsFromPics.Count; i++)
            {
                Words W = TextsFromPics[i];
                if (W != null)
                {
                    List<string> ww5 = Words.KillDupesAndRange(W.all_base10, 5);
                    List<string> ww100 = Words.KillDupesAndRange(W.all_base10, 100);
                    Answer.Add(T, 4, ww5, i);
                    foreach (string s1 in ww100)
                    {
                        resout = resout + s1 + " ";
                    }
                }
                resout = resout + "\r\n";
                //*** позже добавить более низкие приоритеты
            }

            T.tbSectors.Invoke(new Action(() => { T.btSolve.Enabled = true; }));
            T.tbTextHints.Invoke(new Action(() => { T.tbTextHints.Text = resout; }));
            T.tcTabText.Invoke(new Action(() => { T.tcTabText.SelectTab(1); }));
            return true;
        }

        /// <summary>
        /// разпознать и вбить слова от картинок, создание потока
        /// </summary>
        /// <param name="T">таб</param>
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