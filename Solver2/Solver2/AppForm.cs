// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Solver2
{
    class AppForm
    {

        public string mainform_caption = Properties.Resources.ProgramCaption;
        private int border = 5;

        private Logon logonData;
        private GameSelect GameSelectData;

        // тексты
        private string strBadLogon = "Жаль, что не помните ник и пароль... :(\r\n\r\nМожет быть вам необходимо зарегистрироваться на ***.en.cx?";
        private string strBadSelect = "Жаль, что не можете выбрать игру... :(\r\n\r\nМожет быть вам необходимо освежить память открыв список игр в браузере?";
        private string mtGame = "Encounter";
        private string mtGameLogon = "Логон в движке";
        private string mtGameSelect = "Выбор и подключение к игре";
        private string mtGameExit = "Выход";
        private string mtAbout = "О программе";
        private string mtAboutPage = "Перейти на страницу проекта";
        private string mtAboutNewBug = "Я нашел баг! У меня новая идея! хочу сообщить!";
        private string mtAboutManual = "Документация";

        /// <summary>
        /// конструктор основной формы
        /// </summary>
        public AppForm()
        {
            Program.D.F = new Form();
            Program.D.F.Size = new Size(SystemInformation.PrimaryMonitorSize.Width / 4 * 3, SystemInformation.PrimaryMonitorSize.Height / 4 * 3);
            Program.D.F.Text = mainform_caption;
            Program.D.F.StartPosition = FormStartPosition.CenterScreen;
            Program.D.F.AutoSizeMode = AutoSizeMode.GrowOnly;
            Program.D.F.Icon = Properties.Resources.icon2;
            Program.D.F.SizeChanged += new EventHandler(Event_MainFormChangeSize);
            Program.D.Tabs = new TabControl();
            Program.D.F.Controls.Add(Program.D.Tabs);

            Program.D.OneTab = new System.Collections.Generic.List<OneTab>();
            Program.D.Lvl = new System.Collections.Generic.List<Level>();
            Program.D.Game = null;

            // создание меню
            Program.D.F.Menu = new MainMenu();
            MenuItem mGame = new MenuItem(mtGame);
            Program.D.F.Menu.MenuItems.Add(mGame);
            MenuItem mGameLogon = new MenuItem(mtGameLogon);
            mGameLogon.Click += new EventHandler(mGameLogonClick);
            mGame.MenuItems.Add(mGameLogon);
            MenuItem mGameSelect = new MenuItem(mtGameSelect);
            mGameSelect.Enabled = false;
            mGameSelect.Click += new EventHandler(mGameSelectClick);
            mGame.MenuItems.Add(mGameSelect);
            MenuItem mGameExit = new MenuItem(mtGameExit);
            mGameExit.Click += new EventHandler(mGameExitClick);
            mGame.MenuItems.Add(mGameExit);

            MenuItem mAbout = new MenuItem(mtAbout);
            Program.D.F.Menu.MenuItems.Add(mAbout);
            MenuItem mAboutPage = new MenuItem(mtAboutPage);
            mAboutPage.Click += new EventHandler(mAboutPageClick);
            mAbout.MenuItems.Add(mAboutPage);
            MenuItem mAboutNewBug = new MenuItem(mtAboutNewBug);
            mAboutNewBug.Click += new EventHandler(mAboutNewBugClick);
            mAbout.MenuItems.Add(mAboutNewBug);
            MenuItem mAboutManual = new MenuItem(mtAboutManual);
            mAboutManual.Click += new EventHandler(mAboutManualClick);
            mAbout.MenuItems.Add(mAboutManual);

            Event_MainFormChangeSize(null, null);
        }

        /// <summary>
        /// ивент - клик меню - Документация
        /// </summary>
        private void mAboutManualClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/lnl122/Solver2/blob/master/docs/index.md");
        }

        /// <summary>
        /// ивент - клик меню - Новый баг/жалоба
        /// </summary>
        private void mAboutNewBugClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/lnl122/Solver2/issues/new");
        }

        /// <summary>
        /// ивент - клик меню - О программе
        /// </summary>
        private void mAboutPageClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/lnl122/Solver2/blob/master/README.md");
        }

        /// <summary>
        /// ивент на закрытие основной формы и программы
        /// </summary>
        private void mGameExitClick(object sender, EventArgs e)
        {
            Program.D.F.Close();
        }

        /// <summary>
        /// ивент на выбор и подключение к игре
        /// </summary>
        private void mGameSelectClick(object sender, EventArgs e)
        {
            GameSelect GameSelectData = new GameSelect(logonData);
            if (!GameSelectData.isSuccessful)
            {
                MessageBox.Show(strBadSelect);
            }
            else
            {
                Program.D.Game = GameSelectData;
                var menu = Program.D.F.Menu.MenuItems[0].MenuItems;
                foreach (MenuItem m1 in menu)
                {
                    if (m1.Text == mtGameSelect) { m1.Enabled = false; }
                }
                // тут надо по открытой игре создать кучу табов
                CreateLevelsAndTabs();
            }
        }

        /// <summary>
        /// читает игру, создает уровни и табы на форме
        /// </summary>
        private void CreateLevelsAndTabs()
        {
            GameSelect G = Program.D.Game;
            for (int i=1; i <= G.gamelevels; i++)
            {
                Level lvl = new Level(G, i);
                Program.D.Lvl.Add(lvl);
                OneTab OT = new OneTab(Program.D, lvl);
                Program.D.OneTab.Add(OT);
            }

            Event_MainFormChangeSize(null, null);
        }

        /// <summary>
        /// ивент на логон в игре
        /// </summary>
        private void mGameLogonClick(object sender, EventArgs e)
        {
            logonData = new Logon();
            if (!logonData.isSuccessful)
            {
                MessageBox.Show(strBadLogon);
            }
            else
            {
                var menu = Program.D.F.Menu.MenuItems[0].MenuItems;
                foreach(MenuItem m1 in menu)
                {
                    if (m1.Text == mtGameLogon) { m1.Enabled = false; }
                    if (m1.Text == mtGameSelect) { m1.Enabled = true; }
                }
            }
        }

        /// <summary>
        ///  ивент на изменение размера основного окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Event_MainFormChangeSize(object sender, EventArgs e)
        {
            Program.D.Tabs.Left = border;
            Program.D.Tabs.Top = border;
            Program.D.Tabs.Width= Program.D.F.Width - 5 * border;
            Program.D.Tabs.Height = Program.D.F.Height - 13 * border;
        }

    }
}
