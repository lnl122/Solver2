using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solver2
{
    class Books
    {
        // словари - загружаемый и создающийся в процессе работы
        public static List<string> dict;
        // путь к словарю
        private static string DictionaryPath = "";
        // первое создание объекта уже было?
        private static bool isObjectReady = false;
        // словарь был ли загружен?
        private static bool isDicionaryLoaded = false;

        public static void Init()
        {
            Log.Write("books Инициализация объекта Books");
            if (isObjectReady == false)
            {
                dict = new List<string>();
                isObjectReady = true;
            }
        }

        // чтение словаря
        public static void LoadDictionary(string DictPath)
        {
            Log.Write("books Чтение внешнего словаря начато");
            if (isObjectReady == false) { return; }
            // если словарь не загружен
            if (isDicionaryLoaded == false)
            {
                // проверить путь на валидность
                if (System.IO.File.Exists(DictPath) == true)
                {
                    string[] dict1; // временный массив
                    dict1 = System.IO.File.ReadAllLines(DictPath);
                    DictionaryPath = DictPath;
                    // переносим в List
                    foreach (string s1 in dict1)
                    {
                        dict.Add(s1.ToLower().Trim().Replace("  ", "").Replace("  ", "").Replace("  ", ""));
                    }
                    isDicionaryLoaded = true;
                    Log.Write("books Чтение внешнего словаря завершено");
                }
                else
                {
                    Log.Write("books ERROR: словаря по указанному пути нет", DictPath);
                }
            }
        }

    }
}
