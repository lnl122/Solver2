using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using System.Drawing;

namespace Solver2
{
    class Image
    {
        private static int image_border_width = 5;          // отступ от краев нарезанных картинок

        // считает количество мелких картинок из больших, и, их параметров. только для нарезки равными долями
        // вход - список урлов, массив количество строк, массив количеств колонок
        // выход - количество
        public static int GetSmallImageCount(List<string> urls, int[] Rows, int[] Cols)
        {
            int SmallImageCount = 0;
            for (int i = 0; i < urls.Count; i++)
            {
                SmallImageCount += (Cols[i] * Rows[i]);
            }
            return SmallImageCount;
        }
        
        // режет большие картинки на мелкие, сохраняет, возвращает массив локальных путей
        // вход - список урлов, массив количество строк, массив количеств колонок
        // выход - массив локальных путей
        public static string[] GetSmallImagePathes(OneTab T, List<string> urls, int[] Rows, int[] Cols)
        {
            int SmallImageCount = GetSmallImageCount(urls, Rows, Cols);
            string[] SmallImagePath = new string[SmallImageCount];
            List<string> tmplist = new List<string>();

            string basename = Environment.CurrentDirectory + @"\Pics\g" + T.level.G.gameid + "_l" + T.level.number.ToString() + "_p{1}_n{2}.jpg";
            for (int part = 0; part < urls.Count; part++)
            {
                System.Windows.Forms.PictureBox PictBox = new System.Windows.Forms.PictureBox();
                PictBox.Load(urls[part]);
                Bitmap Bmp = new Bitmap(PictBox.Image);

                int total_parts = Rows[part] * Cols[part];
                int w = Bmp.Width;
                int h = Bmp.Height;
                int dw = w / Cols[part] - 2 * image_border_width; // ширина одного блока
                int dh = h / Rows[part] - 2 * image_border_width;
                int cnt = 0;
                for (int r = 0; r < Rows[part]; r++)
                {
                    for (int c = 0; c < Cols[part]; c++)
                    {
                        cnt++;
                        int sw = image_border_width + (w * c / Cols[part]);
                        int sh = image_border_width + (h * r / Rows[part]);
                        System.Drawing.Bitmap nb = new System.Drawing.Bitmap(dw, dh);
                        System.Drawing.Rectangle re = new System.Drawing.Rectangle(sw, sh, dw, dh);
                        nb = Bmp.Clone(re, System.Drawing.Imaging.PixelFormat.Undefined);
                        string fn = basename;
                        fn = fn.Replace("{1}", part.ToString());
                        fn = fn.Replace("{2}", cnt.ToString());
                        nb.Save(fn, System.Drawing.Imaging.ImageFormat.Jpeg);
                        tmplist.Add(fn);
                    }
                }
            }

            SmallImagePath = new string[tmplist.Count];
            for (int i=0; i<tmplist.Count; i++) { SmallImagePath[i] = tmplist[i]; }

            return SmallImagePath;
        }

        // для списка путей к файлам получает описания картинок с гугля
        // вход - массив путей к файлам
        // выход - список коллекций слов
        public static List<Words> GetAllDescriptions(string[] paths)
        {
            // создаем потоки и распознаем в них картинки
            List<Task<Words>> Tasks2 = new List<Task<Words>>();
            foreach (string OneSmallPic in paths)
            {
                Tasks2.Add(Task<Words>.Factory.StartNew(() => Google.GetImageDescription(OneSmallPic)));
            }
            Task.WaitAll(Tasks2.ToArray());
            List<Words> SolvedPics = new List<Words>();
            foreach (Task<Words> t8 in Tasks2)
            {
                Words r8 = t8.Result;
                SolvedPics.Add(r8);
            }
            return SolvedPics;
        }
    }
}
