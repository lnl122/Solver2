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

        /// <summary>
        /// конструктор основной формы
        /// </summary>
        public AppForm()
        {
            Program.D.F = new Form();
            Program.D.F.Size = new Size(SystemInformation.PrimaryMonitorSize.Width / 2, SystemInformation.PrimaryMonitorSize.Height / 2);
            Program.D.F.Text = mainform_caption;// + " / " + G.username + " / " + G.gameid;
            Program.D.F.StartPosition = FormStartPosition.CenterScreen;
            Program.D.F.AutoSizeMode = AutoSizeMode.GrowOnly;
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

            Event_MainFormChangeSize(null, null);
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
            Program.D.Tabs.Height = Program.D.F.Height - 9 * border;
        }

    }
}
