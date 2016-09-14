using System;
using System.Drawing;
using System.Windows.Forms;

namespace Solver2
{
    class AppForm
    {
        public static string mainform_caption = "Solver2";
        private int border = 5;

        public AppForm(GameSelect G)
        {
            Program.D.F = new Form();
            Program.D.F.Size = new Size(SystemInformation.PrimaryMonitorSize.Width / 2, SystemInformation.PrimaryMonitorSize.Height / 2);
            Program.D.F.Text = mainform_caption + " / " + G.username + " / " + G.gameid;
            Program.D.F.StartPosition = FormStartPosition.CenterScreen;
            Program.D.F.AutoSizeMode = AutoSizeMode.GrowOnly;
            Program.D.F.SizeChanged += new EventHandler(Event_MainFormChangeSize);
            Program.D.Tabs = new TabControl();
            Program.D.F.Controls.Add(Program.D.Tabs);
            Event_MainFormChangeSize(null, null);

            Program.D.Lvl = new Level[G.gamelevels];
            Program.D.Tab = new TabPage[G.gamelevels];
            Program.D.OneTab = new OneTab[G.gamelevels];
            for (int i=1; i<= G.gamelevels; i++)
            {
                Level lvl = new Level(G, i);
                Program.D.Lvl[i - 1] = lvl;
                OneTab OT = new OneTab(Program.D, lvl);
                Program.D.Tab[i - 1] = OT.Tab;
                Program.D.OneTab[i - 1] = OT;
            }
            Event_MainFormChangeSize(null, null);
        }

        private void Event_MainFormChangeSize(object sender, EventArgs e)
        {
            Program.D.Tabs.Left = border;
            Program.D.Tabs.Top = border;
            Program.D.Tabs.Width= Program.D.F.Width - 5 * border;
            Program.D.Tabs.Height = Program.D.F.Height - 9 * border;
        }
    }
}
