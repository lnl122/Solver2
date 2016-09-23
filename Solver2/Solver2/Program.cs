
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Solver2
{
    static class Program
    {
        // структыра основных данных
        public struct Data
        {
            public AppForm AF;
            public System.Windows.Forms.Form F;
            public System.Windows.Forms.TabControl Tabs;
            public System.Windows.Forms.TabPage[] Tab;
            public OneTab[] OneTab;
            public GameSelect Game;
            public Level[] Lvl;
        }

        // основные данные программы
        public static Data D;

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

            Books.Init();
            Books.LoadDictionary(localpath + "Books.dat");

            Answer.Init();

        }
        // завершаем работы наших объектов
        public static void CloseComponents()
        {
            SpellChecker.SaveDictionary();
            Associations.SaveDictionary();
            Associations.SaveDictionaryBad();
        }

        [STAThread]
        // код основной программы
        static void Main(string[] args)
        {

            /*
            int PicsCount = 16;
            int SizeOlimp = PicsCount * 2;
            int[] links = new int[SizeOlimp];
            for (int i = 0; i < SizeOlimp; i++) { links[i] = SizeOlimp - (int)Math.Floor((SizeOlimp - i) / 2.0); }
            links[0] = -1;
            links[SizeOlimp - 1] = -1;
            int num = 31;

            System.Collections.Generic.List<int> res = new System.Collections.Generic.List<int>();
            if ((num !=0) && (num != links.Length-1)) { res.Add(links[num]); }
            for(int i = 1; i<links.Length-1; i++)
            {
                if(links[i] == num) { res.Add(i); }
            }
            int iii = 0;
            */

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
                    D.Game = GameSelectData;

                    // весь код ниже пока относиться (08.09.16) только к штурмам
                    if (D.Game.isStorm == true) { D.Lvl = new Level[D.Game.gamelevels]; } else { D.Lvl = new Level[1]; }
                    for (int i = 0; i < D.Game.gamelevels; i++) { D.Lvl[i] = new Level(D.Game, i + 1); }

                    // создаём форму, передаём её управление
                    D.AF = new AppForm(D.Game);
                    System.Windows.Forms.Application.Run(D.F);
                }
            }

            // закругляемся
            CloseComponents();
            Log.Write("Выход из программы..");
            Log.Close();
        }
    }
}
