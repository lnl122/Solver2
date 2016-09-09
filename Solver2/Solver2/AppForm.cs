using System;
using System.Drawing;
using System.Windows.Forms;

namespace Solver2
{
    class AppForm
    {
        public static string mainform_caption = "Solver2";

        public Form MF;
        public TabControl Tabs;
        public TabPage[] Tab;

        public AppForm(GameSelect G, Level[] L)
        {
            MF = new Form();
            MF.Size = new Size(SystemInformation.PrimaryMonitorSize.Width / 2, SystemInformation.PrimaryMonitorSize.Height / 2);
            MF.Text = mainform_caption;
            MF.StartPosition = FormStartPosition.CenterScreen;
            MF.AutoSizeMode = AutoSizeMode.GrowOnly;
            //MF.SizeChanged += new EventHandler(Event_MainFormChangeSize);
            Tabs = new TabControl();
            MF.Controls.Add(Tabs);

            Tab = new TabPage[G.gamelevels];
            for(int i=1; i<= G.gamelevels; i++)
            {
                TabPage TP = new TabPage();
                TP.Text = i.ToString();
                Tabs.Controls.Add(TP);
                TextBox TB = new TextBox();
                TB.Text = L[i-1].name;
                TP.Controls.Add(TB);
            }
        }

    }
}
