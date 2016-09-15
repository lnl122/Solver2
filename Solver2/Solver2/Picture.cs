using System.Threading.Tasks;

namespace Solver2
{
    class Picture
    {
        private OneTab T;
        
        public bool Process(OneTab T)
        {
            // создаем массив путей из больших картинок
            string[] SmallImagePath = Image.GetSmallImagePathes(T, T.level.urls, T.iRows, T.iCols);



            return true;
        }

        // разпознать и вбить слова от картинок
        public Picture(OneTab oneTab)
        {
            T = oneTab;
            if (T.cbImageCuttingMethod.SelectedText == "Указан в ручную, равные доли")
            {
                Log.Write("Pics Начали решать картинки\r\n.\r\n");
                Task<bool> t1 = Task<bool>.Factory.StartNew(() => Process(T));
            }
        }
    }
}