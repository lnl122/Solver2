// *** Program          для скрашивания одиночества в процессе запуска нужен сплешскрин на этап инициализации и проверок.
// *** CheckSpell       при необходимости проверки более 1000 слов за один прием - разбивать на несколько процессов по 1000 слов.
// *** Associations     добавить поиск ассоциаций по трем словам
// *** Upload           сделать привязку к настройкам
// *** Upload           добавить в проверки при старте полный цикл: картинка (чебурашка) распознавание + ассоциации = проверяемый результат (гена)
// *** Upload           добавить функционал инициализации - выбор исправно работающего постащика услуг, сохранение выбора в стат.переменной
// *** GameSelect       нарушен принцип атомарности, код надо разбить на более мелкие куски

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Solver2
{
    static class Program
    {

        // получаем строку с версиями установленных .net
        private static string GetVersionDotNetFromRegistry()
        {
            string res = "";
            using (RegistryKey ndpKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {
                        res = res + versionKeyName + " ";
                    }
                }
            }
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    int releaseKey = (int)ndpKey.GetValue("Release");
                    if (releaseKey >= 393295) { res = res + " v4.6"; }
                    else
                    {
                        if ((releaseKey >= 379893)) { res = res + " v4.5.2"; }
                        else
                        {
                            if ((releaseKey >= 378675)) { res = res + " v4.5.1"; }
                            else
                            {
                                if ((releaseKey >= 378389)) { res = res + " v4.5"; }
                            }
                        }
                    }
                }
            }
            return res.Trim();
        }
        // получаем строку с версией MS Word
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
        // проверяем наличие, настройки и также работу всех необходимых компонент, ведем лог
        private static bool CheckComponents()
        {
            // .NET
            string DotNetVersions = GetVersionDotNetFromRegistry();
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
            WebClient wc1 = null;
            try { wc1 = new WebClient(); } catch { Log.Write("check ERROR: Не удалось создать объект WebClient"); return false; }
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

        // инициализируем наши объекты
        public static void InitComponents()
        {
            string localpath = Environment.CurrentDirectory + @"\Data\";
            SpellChecker.Init();
            SpellChecker.LoadDictionary(localpath + "SpChDict.dat");
            Associations.Init();
            Associations.LoadDictionary(localpath + "AssocDict.dat");
            Associations.LoadDictionaryBad(localpath + "AssocDictBad.dat");
        }
        // завершаем работы наших объектов
        public static void CloseComponents()
        {
            SpellChecker.SaveDictionary();
            Associations.SaveDictionary();
            Associations.SaveDictionaryBad();
        }

        // код основной программы
        static void Main(string[] args)
        {
            // инитим лог
            Log.Init();
            Log.Write("________________________________________________________________________________");
            Log.Write("      Старт программы..");
            Log.Write("      Сборка от " + File.GetCreationTime(Process.GetCurrentProcess().MainModule.FileName).ToString());
            Log.Write("      ПК: " + Environment.MachineName);
            Log.Write("      " + System.Environment.OSVersion.VersionString + ", " + Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") + ", ver:" + Environment.Version.ToString() + ", CPU: " + Environment.ProcessorCount.ToString() + ", 64bit:" + Environment.Is64BitOperatingSystem.ToString());

            // выполняем проверки окружения
            if (!CheckComponents())
            {
                System.Windows.Forms.MessageBox.Show("Не все необхдимые компоненты установлены на ПК.\r\nПроверьте лог-файл.");
                return;
            }
            // инициализируем наши собственные компоненты
            InitComponents();
            // выполняем логон в системе
            Logon logonData = new Logon();
            if (!logonData.isSuccessful) { System.Windows.Forms.MessageBox.Show("Жаль, что не помните ник и пароль... :(\r\n\r\nМожет быть вам необходимо зарегистрироваться на ***.en.cx?"); }
            else
            {
                GameSelect GameSelectData = new GameSelect(logonData);
                if (!GameSelectData.isSuccessful) { System.Windows.Forms.MessageBox.Show("Жаль, что не можете выбрать игру... :(\r\n\r\nМожет быть вам необходимо освежить память открыв список игр в браузере?"); }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Едем!");
                    // *** тут нужно прочитать все уровни, заполнить инитные данные для формы, создать её и открыть.
                }
            }

            // создаём форму, передаём её управление
            //MainForm MF1 = new MainForm();
            //System.Windows.Forms.Application.Run(MainForm.MF);

            //var tt = Upload.UploadFile_saveimgru(@"C:\1\34\pics\g24889_l2_p1_n1.jpg");

            // закругляемся
            CloseComponents();
            Log.Write("Выход из программы..");
            Log.Close();
        }
    }
}
