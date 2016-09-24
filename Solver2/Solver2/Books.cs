using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solver2
{
    class Books
    {
        // словари
        public static List<string> dict;
        public static List<string> plain;
        public static List<string> gapo;
        public static int WordsCount = 0;
        // путь к словарю
        private static string DictionaryPath = "";
        // первое создание объекта уже было?
        private static bool isObjectReady = false;
        // словарь был ли загружен?
        private static bool isDicionaryLoaded = false;

        // инит объекта
        public static void Init()
        {
            Log.Write("books Инициализация объекта Books");
            if (isObjectReady == false)
            {
                dict = new List<string>();
                plain = new List<string>();
                gapo = new List<string>();
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
                        string s = ClearBookName(s1);

                        if (s != "")
                        {
                            dict.Add(s);
                            plain.Add(s.Replace(" ", "").ToLower());
                            string[] ar1 = s.Split(' ');
                            string res = "";
                            foreach (string w in ar1)
                            {
                                if (w.Length == 1)
                                {
                                    res = res + w.ToUpper();
                                }
                                else
                                {
                                    res = res + w.Substring(0, 1).ToUpper();
                                    res = res + w.Substring(1, 1).ToLower();
                                }
                            }
                            gapo.Add(res);
                        }
                    }
                    WordsCount = dict.Count;

                    isDicionaryLoaded = true;
                    Log.Write("books Чтение внешнего словаря завершено");
                }
                else
                {
                    Log.Write("books ERROR: словаря по указанному пути нет", DictPath);
                }
            }
        }

        // убираем лишние лимволы из названия
        public static string ClearBookName(string s1)
        {
            string s = s1.Trim().Replace("  ", "").Replace("  ", "").Replace("  ", "").Replace("ё", "е").Replace("Ё", "Е");
            s = s.Replace(".", "").Replace(",", "").Replace("-", "").Replace("\"", "").Replace("!", "").Replace("?", "").Replace("#", "");
            s = s.Replace(":", "").Replace(";", "").Replace("%", "").Replace("(", "").Replace(")", "").Replace("+", "").Replace("/", "").Replace("\\", "");
            s = s.Trim().Replace("  ", "").Replace("  ", "").Replace("  ", "");
            return s;
        }

    }
}
