﻿// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Diagnostics;
using System.IO;

namespace Solver2
{
    static class Program
    {
        // 
        public static Splash spl;

        // структыра основных данных
        public struct Data
        {
            public System.Windows.Forms.Form F;
            public System.Windows.Forms.TabControl Tabs;
            //public System.Windows.Forms.TabPage[] Tab;
            public System.Collections.Generic.List<OneTab> OneTab;
            public System.Collections.Generic.List<Level> Lvl;
            public GameSelect Game;
        }

        // основные данные программы
        public static Data D;

        [STAThread]
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

            //создаём сплеш
            spl = new Splash(Properties.Resources.version);

            // выполняем проверки окружения
            if (!Components.Check())
            {
                System.Windows.Forms.MessageBox.Show("Не все необхдимые компоненты установлены на ПК.\r\nПроверьте лог-файл.");
                return;
            }

            // инициализируем наши собственные компоненты
            Components.Init();

            // создаем основную форму
            spl.ChangeProgress(70, "Создаём форму приложения");
            AppForm AF = new AppForm();


            // создаём пользовательский таб
            spl.ChangeProgress(70, "Создаём пользовательский уровень");
            Level userlevel = new Level(D.Game, 0);
            D.Lvl.Add(userlevel);
            OneTab OT = new OneTab(D, userlevel);
            D.OneTab.Add(OT);

            // закрываем сплеш
            spl.ChangeProgress(100, "Готово!");
            System.Windows.Forms.Application.Run(D.F);

            //System.Windows.Forms.MessageBox.Show("Типа мы запустились и работаем.");
            /*
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
            */

            // закругляемся
            Components.Close();
            Log.Write("Выход из программы..");
            Log.Close();
        }
    }
}
