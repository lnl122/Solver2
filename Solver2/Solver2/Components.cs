// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;

namespace Solver2
{
    class Components
    {
        /// <summary>
        /// получаем строку с версией MS Word
        /// </summary>
        /// <returns></returns>
        private static string GetVersionMicrosoftWord()
        {
            try
            {
                var WordApp = new Microsoft.Office.Interop.Word.Application();
                string s1 = WordApp.Version;
                WordApp.Quit();
                return s1;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// удаляет файлы *.http из папки
        /// </summary>
        /// <param name="files">массив имён файлов</param>
        /// <returns>true - при успешном удалении</returns>
        private static bool DeleteOldPages(string[] files)
        {
            try
            {
                foreach (string f in files)
                {
                    System.IO.File.Delete(f);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// завершаем работы наших объектов
        /// </summary>
        public static void Close()
        {
            SpellChecker.SaveDictionary();
            Associations.SaveDictionary();
            Associations.SaveDictionaryBad();
        }

        /// <summary>
        /// проверяем наличие, настройки и также работу всех необходимых компонент, ведем лог
        /// </summary>
        /// <returns>признак успешной проверки</returns>
        public static bool Check()
        {
            // .NET
            string DotNetVersions = Registry.GetVersionDotNet();
            Log.Write("check Найденные версии .NET: " + DotNetVersions);
            if (DotNetVersions.IndexOf("v2.0") == -1) { Log.Write("check ERROR: Отсутствует .NET v2.0"); return false; }
            if (DotNetVersions.IndexOf("v3.0") == -1) { Log.Write("check ERROR: Отсутствует .NET v3.0"); return false; }
            if (DotNetVersions.IndexOf("v4.0") == -1) { Log.Write("check ERROR: Отсутствует .NET v4.0"); return false; }
            if ((DotNetVersions.IndexOf("v4.5") == -1) && (DotNetVersions.IndexOf("v4.6") == -1)) { Log.Write("check ERROR: Отсутствует .NET v4.5 или v4.6"); return false; }

            // MS Word
            string WordVersion = GetVersionMicrosoftWord();
            if (WordVersion == "") { Log.Write("check ERROR: Отсутствует установленный Microsoft Word"); return false; }
            int ii1 = 0;
            if (Int32.TryParse(WordVersion.Substring(0, WordVersion.IndexOf(".")), out ii1))
            {
                if (ii1 <= 11) { Log.Write("check ERROR: Версия Microsoft Word ниже 11.0, необходим Microsoft Word 2007 или более новый"); return false; }
            }
            else
            {
                Log.Write("check ERROR: Не удалось определить версию Microsoft Word"); return false;
            }
            Log.Write("check Найден Microsoft Word версии " + WordVersion);
            try
            {
                var testSC = new SpellChecker();
                if (testSC.CheckOne("мама") && testSC.CheckOne("мыла") && testSC.CheckOne("раму"))
                {
                    Log.Write("check Проверка орфографии установлена");
                }
            }
            catch
            {
                Log.Write("ERROR: Не удалось запустить проверку орфографии, или же проверка русского языка не установлена.."); return false;
            }

            // проверка открытия web-ресурсов
            System.Net.WebClient wc1 = null;
            try { wc1 = new System.Net.WebClient(); } catch { Log.Write("check ERROR: Не удалось создать объект WebClient"); return false; }
            string re1 = "";
            try { re1 = wc1.DownloadString("http://image.google.com/"); } catch { Log.Write("check ERROR: http://image.google.com/ не открывается"); return false; }
            try { re1 = wc1.DownloadString("http://game.en.cx/"); } catch { Log.Write("check ERROR: http://game.en.cx/ не открывается"); return false; }
            //try { re1 = wc1.DownloadString("http://jpegshare.net/"); }      catch { Log.Write("check ERROR: http://jpegshare.net/ не открывается");     return false; }
            //try { re1 = wc1.DownloadString("http://ipic.su/"); }            catch { Log.Write("check ERROR: http://ipic.su/ не открывается");           return false; }
            try { re1 = wc1.DownloadString("http://goldlit.ru/"); } catch { Log.Write("check ERROR: http://goldlit.ru/ не открывается"); return false; }
            try { re1 = wc1.DownloadString("http://sociation.org/"); } catch { Log.Write("check ERROR: http://sociation.org/ не открывается"); return false; }
            try { re1 = wc1.DownloadString("https://ru.wiktionary.org/"); } catch { Log.Write("check ERROR: https://ru.wiktionary.org/ не открывается"); return false; }
            Log.Write("check Все необходимые web-ресурсы открываются успешно");

            // все проверки пройдены
            return true;
        }

        /// <summary>
        /// инициализация объектов
        /// </summary>
        public static void Init()
        {
            string DataLocalPath = Environment.CurrentDirectory + @"\Data\";
            string PageLocalPath = Environment.CurrentDirectory + @"\Pages\";
            System.Threading.Tasks.Task<bool> t1 = System.Threading.Tasks.Task<bool>.Factory.StartNew(() => DeleteOldPages(System.IO.Directory.GetFiles(PageLocalPath, "*.http")));

            SpellChecker.Init();
            SpellChecker.LoadDictionary(DataLocalPath + "SpChDict.dat");

            Associations.Init();
            Associations.LoadDictionary(DataLocalPath + "AssocDict.dat");
            Associations.LoadDictionaryBad(DataLocalPath + "AssocDictBad.dat");

            Books.Init();
            Books.LoadDictionary(DataLocalPath + "Books.dat");

            Answer.Init();
        }
    }
}
