// *** OneTab           в html на экран выводиться все строчными буквами. надо переделать парсинг страницы аккуратнее

using System;
using System.Windows.Forms;

namespace Solver2
{
    class OneTab
    {
        // constants
        private int border = 5;
        private int part1 = 20;
        private int part2 = 80;
        private string blank_html = "<!DOCTYPE html><html lang=\"ru\"><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"><title>Сеть городских игр Encounter</title><meta name=\"description\" content=\"\"></head><body class=\"\">решение ещё не началось, или в задании нет картинок</body></html>";

        // main vars
        public TabPage Tab;
        public Level level;

        // 1 column
        public Label lbAnswer;
        public TextBox tbAnswer;
        public Button btAnswer;
        public Label lbAnswerMulti;
        public TextBox tbAnswerMulti;
        public Button btAnswerMulti;

        // settings and choice
        public Label lbType;
        public ComboBox cbType;
        public int[] iCols;
        public int[] iRows;
        public int[] iBeginNum;
        public enum prot { none, begin1, begin2, begin3, end1, end2, end3 };
        public prot enProtect;
        public enum kubr { kubray, bukari1, bukari2, yarbuk, kubr_gybrid2, kubr_gybrid3 };
        public prot enKubray;
        public int iLetterInWord1; // для метаграмм/логогрифов/брюкв и прочих
        public int iLetterInWord2;
        public int iGybridMin; // для гибридов/циклоидов
        public int iGybridMax;

        // 2 column
        public TabControl tcTabWeb;
        public TabPage tpTask;
        public WebBrowser wbTask;
        public TabPage tpPictures;
        public WebBrowser wbPictures;

        public TabControl tcTabText;
        public TabPage tpTextTask;
        public TextBox tbTextTask;
        public TabPage tpTextHints;
        public TextBox tbTextHints;

        //3 column
        public Label lbSectors;
        public TextBox tbSectors;
        public Label lbBonuses;
        public TextBox tbBonuses;

        public OneTab(Program.Data D, Level lvl)
        {
            level = lvl;
            Tab = new TabPage();
            Tab.Text = level.number.ToString() + " : " + level.name;
            D.F.SizeChanged += new EventHandler(Event_ChangeSize);
            D.Tabs.Controls.Add(Tab);

            // 1
            lbAnswer = new Label();
            lbAnswer.Text = "Ответ:";
            Tab.Controls.Add(lbAnswer);
            tbAnswer = new TextBox();
            tbAnswer.Text = "";
            Tab.Controls.Add(tbAnswer);
            btAnswer = new Button();
            btAnswer.Text = "->";
            Tab.Controls.Add(btAnswer);
            lbAnswerMulti = new Label();
            lbAnswerMulti.Text = "Ответы:";
            Tab.Controls.Add(lbAnswerMulti);
            tbAnswerMulti = new TextBox();
            tbAnswerMulti.Multiline = true;
            tbAnswerMulti.ScrollBars = ScrollBars.Both;
            tbAnswerMulti.Text = "";
            Tab.Controls.Add(tbAnswerMulti);
            btAnswerMulti = new Button();
            btAnswerMulti.Text = "->";
            Tab.Controls.Add(btAnswerMulti);

            // settings


            // 2 column
            tcTabWeb = new TabControl();
            Tab.Controls.Add(tcTabWeb);
            tpTask = new TabPage();
            tpTask.Text = "Задача";
            tcTabWeb.Controls.Add(tpTask);
            wbTask = new WebBrowser();
            wbTask.Navigate("about:blank");
            HtmlDocument doc1 = wbTask.Document;
            doc1.Write(level.html);
            tpTask.Controls.Add(wbTask);

            tpPictures = new TabPage();
            tpPictures.Text = "Картинки";
            tcTabWeb.Controls.Add(tpPictures);
            wbPictures = new WebBrowser();
            wbPictures.Navigate("about:blank");
            HtmlDocument doc2 = wbPictures.Document;
            doc2.Write(blank_html);
            tpPictures.Controls.Add(wbPictures);
            tcTabText = new TabControl();
            Tab.Controls.Add(tcTabText);
            tpTextTask = new TabPage();
            tpTextTask.Text = "Текст задачи";
            tcTabText.Controls.Add(tpTextTask);
            tbTextTask = new TextBox();
            tbTextTask.Multiline = true;
            tbTextTask.Text = level.text;
            tpTextTask.Controls.Add(tbTextTask);
            tpTextHints = new TabPage();
            tpTextHints.Text = "Хинты";
            tcTabText.Controls.Add(tpTextHints);
            tbTextHints = new TextBox();
            tbTextHints.Multiline = true;
            tbTextHints.Text = "решение ещё не началось, или нет смысла сюда выводить варианты";
            tpTextHints.Controls.Add(tbTextHints);

            Event_ChangeSize(this, null);
        }

        public void Event_ChangeSize(object sender, EventArgs e)
        {
            int width = Program.D.Tabs.Width - 2 * border;
            int height = Program.D.Tabs.Height - 4 * border;
            int top = border;
            int left1 = border;
            int left2 = border + width * part1 / 100;
            int left3 = border + width * part2 / 100;
            int width1 = left2 - left1 - border;
            int width2 = left3 - left2 - border;
            int width3 = width - left3;

            lbAnswer.Top = top;
            lbAnswer.Left = left1;
            lbAnswer.Width = width1;
            tbAnswer.Top = lbAnswer.Bottom + border;
            tbAnswer.Left = left1;
            tbAnswer.Width = width1 - 10 * border;
            btAnswer.Top = tbAnswer.Top;
            btAnswer.Left = tbAnswer.Right + border;
            btAnswer.Width = 8 * border;

            lbAnswerMulti.Top = tbAnswer.Bottom + 2 * border;
            lbAnswerMulti.Left = left1;
            lbAnswerMulti.Width = width1;
            tbAnswerMulti.Top = lbAnswerMulti.Bottom + border;
            tbAnswerMulti.Left = left1;
            tbAnswerMulti.Width = tbAnswer.Width;
            tbAnswerMulti.Height = 10 * border;
            btAnswerMulti.Top = tbAnswerMulti.Top;
            btAnswerMulti.Left = tbAnswerMulti.Right + border;
            btAnswerMulti.Width = btAnswer.Width;

            // 2
            tcTabWeb.Top = top;
            tcTabWeb.Left = left2;
            tcTabWeb.Width = width2;
            tcTabWeb.Height = (height / 3 * 2) - (2 * border);
            wbTask.Top = border;
            wbTask.Left = border;
            wbTask.Width = tcTabWeb.Width - 3 * border;
            wbTask.Height = tcTabWeb.Height - 3 * border;
            wbPictures.Top = wbTask.Top;
            wbPictures.Left = wbTask.Left;
            wbPictures.Width = wbTask.Width;
            wbPictures.Height = wbTask.Height;

            tcTabText.Top = tcTabWeb.Bottom + border;
            tcTabText.Left = left2;
            tcTabText.Width = width2;
            tcTabText.Height = (height / 3 * 1) - (2 * border);
            tbTextTask.Top = border;
            tbTextTask.Left = border;
            tbTextTask.Width = tcTabText.Width - 3 * border;
            tbTextTask.Height = tcTabText.Height - 3 * border;
            tbTextHints.Top = tbTextTask.Top;
            tbTextHints.Left = tbTextTask.Left;
            tbTextHints.Width = tbTextTask.Width;
            tbTextHints.Height = tbTextTask.Height;

        }
    }
}
