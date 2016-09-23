using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Solver2
{
    class OneTab
    {
        // типы заданий
        public string[] TaskTypes = {
            "Картинки (только решить)",
            "Олимпийки картинками",
            "* Гибриды картинками (буКВАс)",
            "* Логогрифы картинками (сон-слон)",
            "* Метаграммы картинками (кот-кит)",
            "* Брюквы картинками (брюква-буква)",
            "* Матрицы картинками",
            "* Словосочетания сущ+сущ картинками",
            "* Словосочетания сущ+прил картинками",
            "* Анаграмматрицы картинками",
            "* Ассоциации картинками",
            "* Брутфорс по картинке",
            "* Расчленёнки",
            "* ГаПоИФиКа книжная",
            "* ГаПоИФиКа фильмов",
            "* ЛеДиДа книжная",
            "* ЛеДиДа фильмов",
            "* Гибриды текстом (буКВАс)",
            "* Логогрифы текстом (сон-слон)",
            "* Метаграммы текстом (кот-кит)",
            "* Кубрая / Букаря / Яарбук",
            "* Куброолимпийки",
            "* Куброгибриды (кубрая + гибриды)",
            "* Трикуброгибриды (кубрая + тригибрид)"
        };

        // constants
        private int border = 5;
        private int part1 = 15;
        private int part2 = 85;
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

        public int SettingsPositions;
        // settings and choice
        public Label lbType;
        public ComboBox cbType;
        public int[] iCols;
        public int[] iRows;
        //public int[] iBeginNum;
        public int levelUrlsCount;

        public Label lbImageCuttingMethod;
        public ComboBox cbImageCuttingMethod;
        public CheckBox chImageSizeFlag;
        public Label lbImageNumber;
        public ComboBox cbImageNumber;
        public Label lbCols;
        public ComboBox cbCols;
        public Label lbStrs;
        public ComboBox cbStrs;
        public Label lbProtect;
        public ComboBox cbProtect;
        public string sProtect;

        public Label lbSolve;
        public Button btSolve;

        /*
        public enum kubr { kubray, bukari1, bukari2, yarbuk, kubr_gybrid2, kubr_gybrid3 };
        public kubr enKubray;
        public int iLetterInWord1; // для метаграмм/логогрифов/брюкв и прочих
        public int iLetterInWord2;
        public int iGybridMin; // для гибридов/циклоидов
        public int iGybridMax;
        */

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

        // others
        public bool isPicsSect;

        public OneTab(Program.Data D, Level lvl)
        {
            level = lvl;
            levelUrlsCount = level.urls.Count;

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
            btAnswer.Click += new EventHandler(Event_Click_btAnswer);
            Tab.Controls.Add(btAnswer);
            lbAnswerMulti = new Label();
            lbAnswerMulti.Text = "Ответы:";
            Tab.Controls.Add(lbAnswerMulti);
            tbAnswerMulti = new TextBox();
            tbAnswerMulti.Multiline = true;
            tbAnswerMulti.ScrollBars = ScrollBars.Both;
            tbAnswerMulti.Text = "";
            tbAnswerMulti.Click += new EventHandler(Event_Click_btAnswer);
            Tab.Controls.Add(tbAnswerMulti);
            btAnswerMulti = new Button();
            btAnswerMulti.Text = "->";
            Tab.Controls.Add(btAnswerMulti);

            // settings
            lbType = new Label();
            lbType.Text = "Вид задания:";
            Tab.Controls.Add(lbType);

            cbType = new ComboBox();
            foreach (string st1 in TaskTypes)
            {
                if ((levelUrlsCount > 0) || (st1.IndexOf("артин") == -1)) { cbType.Items.Add(st1); }
            }
            cbType.SelectedIndex = 0;
            cbType.SelectedIndexChanged += new EventHandler(Event_Change_cbType);
            Tab.Controls.Add(cbType);

            iCols = new int[levelUrlsCount];
            for (int i = 0; i < levelUrlsCount; i++) { iCols[i] = 4; }
            iRows = new int[levelUrlsCount];
            for (int i = 0; i < levelUrlsCount; i++) { iRows[i] = 4; }

            lbImageCuttingMethod = new Label();
            lbImageCuttingMethod.Text = "Метод нарезки картинок:";
            lbImageCuttingMethod.Visible = false;
            Tab.Controls.Add(lbImageCuttingMethod);

            cbImageCuttingMethod = new ComboBox();
            cbImageCuttingMethod.Items.Add("Указан в ручную, равные доли");
            cbImageCuttingMethod.SelectedIndex = 0;
            cbImageCuttingMethod.Visible = false;
            Tab.Controls.Add(cbImageCuttingMethod);

            chImageSizeFlag = new CheckBox();
            chImageSizeFlag.Text = "Одинаковые размеры у всех";
            chImageSizeFlag.Visible = false;
            if (levelUrlsCount >= 2) { chImageSizeFlag.Enabled = true; } else { chImageSizeFlag.Enabled = false; }
            chImageSizeFlag.CheckedChanged += new EventHandler(Event_Change_chImageSizeFlag);
            Tab.Controls.Add(chImageSizeFlag);

            lbImageNumber = new Label();
            lbImageNumber.Text = "Номер картинки:";
            lbImageNumber.Visible = false;
            Tab.Controls.Add(lbImageNumber);

            cbImageNumber = new ComboBox();
            cbImageNumber.Visible = false;
            if (levelUrlsCount > 0)
            {
                for (int i = 1; i <= levelUrlsCount; i++) { cbImageNumber.Items.Add(i.ToString()); }
                cbImageNumber.SelectedIndex = 0;
            }
            cbImageNumber.SelectedIndexChanged += new EventHandler(Event_Change_cbImageNumber);
            Tab.Controls.Add(cbImageNumber);

            lbStrs = new Label();
            lbStrs.Text = "Строк:";
            lbStrs.Visible = false;
            Tab.Controls.Add(lbStrs);

            cbStrs = new ComboBox();
            for(int i=1; i<=10; i++) { cbStrs.Items.Add(i.ToString()); }
            cbStrs.SelectedIndex = 3;
            cbStrs.Visible = false;
            cbStrs.SelectedIndexChanged += new EventHandler(Event_Change_cbStrs);
            Tab.Controls.Add(cbStrs);

            lbCols = new Label();
            lbCols.Text = "Колонок:";
            lbCols.Visible = false;
            Tab.Controls.Add(lbCols);

            cbCols = new ComboBox();
            for (int i = 1; i <= 10; i++) { cbCols.Items.Add(i.ToString()); }
            cbCols.SelectedIndex = 3;
            cbCols.Visible = false;
            cbCols.SelectedIndexChanged += new EventHandler(Event_Change_cbCols);
            Tab.Controls.Add(cbCols);

            lbProtect = new Label();
            lbProtect.Text = "Защита:";
            lbProtect.Visible = false;
            Tab.Controls.Add(lbProtect);

            cbProtect = new ComboBox();
            cbProtect.Items.Add("нет"); cbProtect.Items.Add("1слово"); cbProtect.Items.Add("01слово"); cbProtect.Items.Add("слово1"); cbProtect.Items.Add("слово01");
            cbProtect.SelectedIndex = 0;
            Event_Change_cbProtect(null, null);
            cbProtect.Visible = false;
            cbProtect.SelectedIndexChanged += new EventHandler(Event_Change_cbProtect);
            Tab.Controls.Add(cbProtect);

            lbSolve = new Label();
            lbSolve.Text = "__________";
            Tab.Controls.Add(btSolve);
            btSolve = new Button();
            btSolve.Text = "Начать решение";
            btSolve.Click += new EventHandler(Event_Click_btSolve);
            Tab.Controls.Add(btSolve);


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

            // 3 column
            lbSectors = new Label();
            lbSectors.Text = "Сектора:";
            Tab.Controls.Add(lbSectors);
            tbSectors = new TextBox();
            tbSectors.Multiline = true;
            tbSectors.ScrollBars = ScrollBars.Both;
            string sec1 = "";
            for(int i = 0; i < level.sectors; i++) { sec1 = sec1 + (i+1).ToString() + ": " + level.sector[i] + "\r\n"; }
            tbSectors.Text = sec1;
            Tab.Controls.Add(tbSectors);
            lbBonuses = new Label();
            lbBonuses.Text = "Бонусы:";
            Tab.Controls.Add(lbBonuses);
            tbBonuses = new TextBox();
            tbBonuses.Multiline = true;
            tbBonuses.ScrollBars = ScrollBars.Both;
            string bon1 = "";
            for (int i = 0; i < level.bonuses; i++) { bon1 = bon1 + (i+1).ToString() + ": " + level.bonus[i] + "\r\n"; }
            tbBonuses.Text = bon1;
            Tab.Controls.Add(tbBonuses);

            Event_ChangeSize(this, null);
            Event_Change_cbType(this, null);
        }

        private void Event_Click_btAnswer(object sender, EventArgs e)
        {
            if(tbAnswer.Text.Trim() != "")
            {
                Answer.Add(this, 1, tbAnswer.Text.Trim(), -1);
                tbAnswer.Text = "";
            }
            if (tbAnswerMulti.Text.Trim() != "")
            {
                string[] ansm = System.Text.RegularExpressions.Regex.Split(tbAnswerMulti.Text, "\r\n");
                foreach(string s1 in ansm)
                {
                    if(s1.Trim() != "")
                    {
                        Answer.Add(this, 2, s1, -1);
                    }
                }
                tbAnswerMulti.Text = "";
            }
        }

        private void Event_Change_cbProtect(object sender, EventArgs e)
        {
            sProtect = cbProtect.SelectedItem.ToString();
        }

        private void Event_Click_btSolve(object sender, EventArgs e)
        {
            string type = cbType.SelectedItem.ToString();
            if (type == "Картинки (только решить)")
            {
                var R1 = new Picture(this);
            }
            if (type == "Олимпийки картинками")
            {
                var R1 = new Picture(this);
                var R2 = new Olimp(this);
            }
            //
            btSolve.Enabled = false;
        }

        private void Event_Change_cbImageNumber(object sender, EventArgs e)
        {
            cbCols.SelectedIndex = iCols[cbImageNumber.SelectedIndex] - 1;
            cbStrs.SelectedIndex = iRows[cbImageNumber.SelectedIndex] - 1;
        }

        private void Event_Change_chImageSizeFlag(object sender, EventArgs e)
        {
            if (chImageSizeFlag.Checked == true)
            {
                for (int i = 0; i < iCols.Length; i++) { iCols[i] = cbCols.SelectedIndex + 1; }
                for (int i = 0; i < iRows.Length; i++) { iRows[i] = cbStrs.SelectedIndex + 1; }
                lbImageNumber.Enabled = false;
                cbImageNumber.Enabled = false;
            } else
            {
                lbImageNumber.Enabled = true;
                cbImageNumber.Enabled = true;
            }
        }

        private void Event_Change_cbCols(object sender, EventArgs e)
        {
            if (chImageSizeFlag.Checked == false)
            {
                iCols[cbImageNumber.SelectedIndex] = cbCols.SelectedIndex + 1;
            } else
            {
                for (int i = 0; i < iCols.Length; i++) { iCols[i] = cbCols.SelectedIndex + 1; }
            }
        }

        private void Event_Change_cbStrs(object sender, EventArgs e)
        {
            if (chImageSizeFlag.Checked == false)
            {
                iRows[cbImageNumber.SelectedIndex] = cbStrs.SelectedIndex + 1;
            }
            else
            {
                for (int i = 0; i < iRows.Length; i++) { iRows[i] = cbStrs.SelectedIndex + 1; }
            }
        }

        private void Event_Change_cbType(object sender, EventArgs e)
        {
            lbImageCuttingMethod.Visible = false;
            cbImageCuttingMethod.Visible = false;
            chImageSizeFlag.Visible = false;
            lbImageNumber.Visible = false;
            cbImageNumber.Visible = false;
            lbCols.Visible = false;
            cbCols.Visible = false;
            lbStrs.Visible = false;
            cbStrs.Visible = false;
            lbProtect.Visible = false;
            cbProtect.Visible = false;
            lbSolve.Visible = false;
            btSolve.Visible = false;
            isPicsSect = false;

            string type = cbType.SelectedItem.ToString();

            if ((type == "Картинки (только решить)") || (type == "Олимпийки картинками"))
            {
                List<object> objs = new List<object>();
                objs.Add(lbImageCuttingMethod);
                objs.Add(cbImageCuttingMethod);
                objs.Add(lbProtect);
                objs.Add(cbProtect);
                objs.Add(chImageSizeFlag);
                objs.Add(lbImageNumber);
                objs.Add(cbImageNumber);
                objs.Add(lbCols);
                objs.Add(cbCols);
                objs.Add(lbStrs);
                objs.Add(cbStrs);
                objs.Add(lbSolve);
                objs.Add(btSolve);
                ShowSettingsOnScreen(objs, SettingsPositions);
                isPicsSect = true;
            }
            //Олимпийки картинками

        }

        private void ShowSettingsOnScreen(List<object> objs, int sp2)
        {
            int sp = sp2;
            foreach(object o1 in objs)
            {
                string t1 = o1.GetType().Name;
                if (t1 == "Label") { ((Label)o1).Visible = true; ((Label)o1).Top = sp; sp = ((Label)o1).Bottom + border; }
                if (t1 == "Button") { ((Button)o1).Visible = true; ((Button)o1).Top = sp; sp = ((Button)o1).Bottom + border; }
                if (t1 == "ComboBox") { ((ComboBox)o1).Visible = true; ((ComboBox)o1).Top = sp; sp = ((ComboBox)o1).Bottom + border; }
                if (t1 == "CheckBox") { ((CheckBox)o1).Visible = true; ((CheckBox)o1).Top = sp; sp = ((CheckBox)o1).Bottom + border; }
                if (t1 == "NumericUpDown") { ((NumericUpDown)o1).Visible = true; ((NumericUpDown)o1).Top = sp; sp = ((NumericUpDown)o1).Bottom + border; }
                if (t1 == "TextBox") { ((TextBox)o1).Visible = true; ((TextBox)o1).Top = sp; sp = ((TextBox)o1).Bottom + border; }
            }
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

            // settings
            lbType.Top = tbAnswerMulti.Bottom + 2 * border;
            lbType.Left = left1;
            lbType.Width = width1;
            cbType.Top = lbType.Bottom + border;
            cbType.Left = left1;
            cbType.Width = width1;
            lbSolve.Left = left1;
            lbSolve.Width = width1;
            btSolve.Left = left1;
            btSolve.Width = width1;

            // left & width
            lbImageCuttingMethod.Left = left1;
            lbImageCuttingMethod.Width = width1;
            cbImageCuttingMethod.Left = left1;
            cbImageCuttingMethod.Width = width1;
            chImageSizeFlag.Left = left1;
            chImageSizeFlag.Width = width1;
            lbImageNumber.Left = left1;
            lbImageNumber.Width = width1;
            cbImageNumber.Left = left1;
            cbImageNumber.Width = width1;
            lbCols.Left = left1;
            lbCols.Width = width1;
            cbCols.Left = left1;
            cbCols.Width = width1;
            lbStrs.Left = left1;
            lbStrs.Width = width1;
            cbStrs.Left = left1;
            cbStrs.Width = width1;
            lbProtect.Left = left1;
            lbProtect.Width = width1;
            cbProtect.Left = left1;
            cbProtect.Width = width1;

            // for others settings
            SettingsPositions = cbType.Bottom + border;

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

            // 3
            lbSectors.Top = top;
            lbSectors.Left = left3;
            lbSectors.Width = width3;
            tbSectors.Top = lbSectors.Bottom + border;
            tbSectors.Left = left3;
            tbSectors.Width = width3;
            tbSectors.Height = (height / 2) - (8 * border);
            lbBonuses.Top = tbSectors.Bottom + border;
            lbBonuses.Left = left3;
            lbBonuses.Width = width3;
            tbBonuses.Top = lbBonuses.Bottom + border;
            tbBonuses.Left = left3;
            tbBonuses.Width = width3;
            tbBonuses.Height = tbSectors.Height;
        }
        /*
        public delegate void dUpdateTb(TextBox tb, string str);
        public static dUpdateTb fUpdateTb = new OneTab.dUpdateTb(OneTab.UpdateTb);
        public static void UpdateTb(System.Windows.Forms.TextBox tb, string str) // делегат, принимающий возвращенные потоком параметры. взаимодействует с ГУИ
        {
            tb.Text = str + "\r\n`r`n" + tb.Text;
        }
        */
    }
}
