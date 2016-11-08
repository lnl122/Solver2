// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System.Windows.Forms;

namespace Solver2
{
    class Splash
    {
        private int border = 5;

        private int value = 0;
        private string text = "";

        private Form F;
        private Label L;
        private Label ver;
        private ProgressBar PB;

        /// <summary>
        /// создаём формы сплеш-скрина и показываем её
        /// </summary>
        /// <param name="vv">номер версии строкой</param>
        public Splash(string vv)
        {
            F = new Form();
            F.StartPosition = FormStartPosition.CenterScreen;
            F.ShowIcon = false;
            F.ShowInTaskbar = false;
            F.FormBorderStyle = FormBorderStyle.None;
            F.Size = new System.Drawing.Size(SystemInformation.PrimaryMonitorSize.Width / 4, SystemInformation.PrimaryMonitorSize.Height / 4);
            F.BackgroundImageLayout = ImageLayout.Stretch;

            System.Windows.Forms.PictureBox PictBox = new System.Windows.Forms.PictureBox();
            PictBox.Load(@"\\alm-dc-02\users\ovsyannikov.anton\Desktop\ENS2.png");
            System.Drawing.Bitmap Bmp = new System.Drawing.Bitmap(PictBox.Image);

            F.BackgroundImage = Bmp;
            L = new Label();
            L.Text = text;
            L.Left = border;
            L.Top = F.Height - 5 * border;
            L.Width = F.Width - 2 * border;
            F.Controls.Add(L);
            ver = new Label();
            ver.Text = "ver " + vv;
            ver.Left = border;
            ver.Top = border;
            F.Controls.Add(ver);
            PB = new ProgressBar();
            PB.Left = border;
            PB.Width = F.Width - 2 * border;
            PB.Top = F.Height - 10 * border;
            PB.Maximum = 100;
            PB.Minimum = 0;
            PB.Value = value;
            F.Controls.Add(PB);
            F.Show();
        }

        /// <summary>
        /// меняет прогресс выполнения и текст
        /// </summary>
        /// <param name="v">прогресс выполнения</param>
        /// <param name="t">текст на сплеш-скрине</param>
        public void ChangeProgress(int v, string t = "")
        {
            if (t != "")
            {
                text = t;
                L.Text = t;
            }
            if (v > value )
            {
                value = v;
                PB.Value = v;
            }
            if (value == 100)
            {
                F.Close();
            }
        }

    }
}
