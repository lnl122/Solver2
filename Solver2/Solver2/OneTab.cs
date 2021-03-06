﻿// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Solver2
{
    class OneTab
    {
        private static string s_picture_only            = "Картинки(только решить)";
        private static string s_picture_olimp           = "Олимпийки картинками";
        private static string s_olimp_only              = "Ассоциации для олимпиек";
        private static string s_picture_gybrid          = "Гибриды картинками (буКВАс)";
        private static string s_picture_logogrif        = "* Логогрифы картинками (сон-слон)";
        private static string s_picture_metagram        = "* Метаграммы картинками (кот-кит)";
        private static string s_picture_brukva          = "* Брюквы картинками (брюква-буква)";
        private static string s_picture_matrix          = "* Матрицы картинками";
        private static string s_picture_slova_ss        = "* Словосочетания сущ+сущ картинками";
        private static string s_picture_slova_sp        = "* Словосочетания сущ+прил картинками";
        private static string s_picture_anagram_matrix  = "* Анаграмматрицы картинками";
        private static string s_picture_association     = "* Ассоциации картинками";
        private static string s_picture_bruteforce      = "* Брутфорс по картинке";
        private static string s_raschlenenki            = "Расчленёнки";
        private static string s_gapoifaka_books         = "ГаПоИФиКа книжная";
        private static string s_gapoifaka_films         = "ГаПоИФиКа фильмов";
        private static string s_ledida_books            = "ЛеДиДа книжная";
        private static string s_ledida_films            = "* ЛеДиДа фильмов";
        private static string s_text_gybrid             = "* Гибриды текстом (буКВАс)";
        private static string s_text_logogrif           = "* Логогрифы текстом (сон-слон)";
        private static string s_text_metagram           = "* Метаграммы текстом (кот-кит)";
        private static string s_kubray                  = "* Кубрая / Букаря / Яарбук";
        private static string s_kubray_olimp            = "* Куброолимпийки";
        private static string s_kubray_gybrid           = "* Куброгибриды (кубрая + гибриды)";
        private static string s_kubray_3gybrid          = "* Трикуброгибриды (кубрая + тригибрид)";

        // типы заданий
        public string[] TaskTypes = {s_picture_only, s_picture_olimp, s_olimp_only, s_picture_gybrid, s_picture_logogrif, s_picture_metagram,
            s_picture_brukva, s_picture_matrix, s_picture_slova_ss, s_picture_slova_sp, s_picture_anagram_matrix, s_picture_association,
            s_picture_bruteforce, s_raschlenenki, s_gapoifaka_books, s_gapoifaka_films, s_ledida_books, s_ledida_films, s_text_gybrid,
            s_text_logogrif, s_text_metagram, s_kubray, s_kubray_olimp, s_kubray_gybrid, s_kubray_3gybrid
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
        public Button btAnswerSpace;
        public Button btAnswerDot;
        public Button btAnswerDot2;

        public int SettingsPositions;
        // settings and choice
        public Label lbType;
        public ComboBox cbType;
        public int[] iCols;
        public int[] iRows;
        public int levelUrlsCount;

        public Label lbOlimpSize;
        public ComboBox cbOlimpSize;
        public int iOlimpSize = 0;

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

        public Label lbRaschl;
        public ComboBox cbRaschl;
        public int iRaschl; // для расчлененок
        public Label lbGybrid;
        public ComboBox cbGybrid;
        public int iGybridMin; // для гибридов/циклоидов
        //public int iGybridMax;

        public Label lbSolve;
        public Button btSolve;

        /*
        public enum kubr { kubray, bukari1, bukari2, yarbuk, kubr_gybrid2, kubr_gybrid3 };
        public kubr enKubray;
        public int iLetterInWord1; // для метаграмм/логогрифов/брюкв и прочих
        public int iLetterInWord2;
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
        public TabControl tcTabSecBon;
        public TabPage tpSectors;
        public TabPage tpBonuses;
        public TextBox tbSectors;
        public TextBox tbBonuses;

        // others
        public bool isPicsSect;

        /// <summary>
        /// создаёт объект закладки с элементами управления. располагает задания
        /// </summary>
        /// <param name="D">глобальная инфа о игре</param>
        /// <param name="lvl">объект уровня</param>
        public OneTab(Program.Data D, Level lvl)
        {
            level = lvl;
            levelUrlsCount = level.urls.Count;

            Tab = new TabPage();
            Tab.Text = level.number.ToString() + ": " + level.name;
            D.F.SizeChanged += new EventHandler(Event_ChangeSize);
            D.Tabs.Controls.Add(Tab);

            // 1
            lbAnswer = new Label();
            lbAnswer.Text = "Ответ:";
            Tab.Controls.Add(lbAnswer);
            tbAnswer = new TextBox();
            tbAnswer.Text = "";
            tbAnswer.KeyDown += new KeyEventHandler(Event_KeyDown_tbAnswer);

            Tab.Controls.Add(tbAnswer);
            btAnswer = new Button();
            btAnswer.Text = "===>";
            btAnswer.Click += new EventHandler(Event_Click_btAnswer);
            Tab.Controls.Add(btAnswer);
            lbAnswerMulti = new Label();
            lbAnswerMulti.Text = "Ответы:";
            Tab.Controls.Add(lbAnswerMulti);
            tbAnswerMulti = new TextBox();
            tbAnswerMulti.Multiline = true;
            tbAnswerMulti.ScrollBars = ScrollBars.Both;
            tbAnswerMulti.Text = "";

            Tab.Controls.Add(tbAnswerMulti);
            btAnswerSpace = new Button();
            btAnswerSpace.Text = "\" \" |->";
            btAnswerSpace.Click += new EventHandler(Event_Click_btAnswerSpace);
            Tab.Controls.Add(btAnswerSpace);
            btAnswerDot = new Button();
            btAnswerDot.Text = "\".\" |->";
            btAnswerDot.Click += new EventHandler(Event_Click_btAnswerDot);
            Tab.Controls.Add(btAnswerDot);
            btAnswerDot2 = new Button();
            btAnswerDot2.Text = "\":\" |->";
            btAnswerDot2.Click += new EventHandler(Event_Click_btAnswerDot2);
            Tab.Controls.Add(btAnswerDot2);

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
            cbType.MaxDropDownItems = 30;
            cbType.DropDownStyle = ComboBoxStyle.DropDownList;
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
            cbImageCuttingMethod.DropDownStyle = ComboBoxStyle.DropDownList;
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
            cbImageNumber.DropDownStyle = ComboBoxStyle.DropDownList;
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
            cbStrs.DropDownStyle = ComboBoxStyle.DropDownList;
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
            cbCols.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCols.SelectedIndexChanged += new EventHandler(Event_Change_cbCols);
            Tab.Controls.Add(cbCols);

            lbOlimpSize = new Label();
            lbOlimpSize.Text = "Размер олимпийки:";
            lbOlimpSize.Visible = false;
            Tab.Controls.Add(lbOlimpSize);

            cbOlimpSize = new ComboBox();
            cbOlimpSize.Items.Add("не выбрано"); cbOlimpSize.Items.Add("4 + 3 = 7"); cbOlimpSize.Items.Add("8 + 7 = 15"); cbOlimpSize.Items.Add("16 + 15 = 31"); cbOlimpSize.Items.Add("32 + 31 = 63"); cbOlimpSize.Items.Add("64 + 63 = 127"); cbOlimpSize.Items.Add("128 + 127 = 511");
            cbOlimpSize.SelectedIndex = 0;
            iOlimpSize = 0;
            cbOlimpSize.Visible = false;
            cbOlimpSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cbOlimpSize.SelectedIndexChanged += new EventHandler(Event_Change_cbOlimpSize);
            Tab.Controls.Add(cbOlimpSize);

            lbRaschl = new Label();
            lbRaschl.Text = "Количество слов:";
            lbRaschl.Visible = false;
            Tab.Controls.Add(lbRaschl);
            cbRaschl = new ComboBox();
            cbRaschl.Items.Add("1"); cbRaschl.Items.Add("2"); cbRaschl.Items.Add("3");
            cbRaschl.SelectedIndex = 0;
            iRaschl = 1;
            cbRaschl.Visible = false;
            cbRaschl.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRaschl.SelectedIndexChanged += new EventHandler(Event_Change_cbRaschl);
            Tab.Controls.Add(cbRaschl);

            lbGybrid = new Label();
            lbGybrid.Text = "Мин. пересечение, букв:";
            lbGybrid.Visible = false;
            Tab.Controls.Add(lbGybrid);
            cbGybrid = new ComboBox();
            cbGybrid.Items.Add("2"); cbGybrid.Items.Add("3"); cbGybrid.Items.Add("4"); cbGybrid.Items.Add("5");
            cbGybrid.SelectedIndex = 1;
            iGybridMin = 3;
            cbGybrid.Visible = false;
            cbGybrid.DropDownStyle = ComboBoxStyle.DropDownList;
            cbGybrid.SelectedIndexChanged += new EventHandler(Event_Change_cbGybrid);
            Tab.Controls.Add(cbGybrid);

            lbProtect = new Label();
            lbProtect.Text = "Защита:";
            lbProtect.Visible = false;
            Tab.Controls.Add(lbProtect);

            cbProtect = new ComboBox();
            cbProtect.Items.Add("нет"); cbProtect.Items.Add("1слово"); cbProtect.Items.Add("01слово"); cbProtect.Items.Add("слово1"); cbProtect.Items.Add("слово01");
            cbProtect.SelectedIndex = 0;
            Event_Change_cbProtect(null, null);
            cbProtect.Visible = false;
            cbProtect.DropDownStyle = ComboBoxStyle.DropDownList;
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
            tcTabSecBon = new TabControl();
            Tab.Controls.Add(tcTabSecBon);
            tpSectors = new TabPage();
            tpSectors.Text = "Сектора";
            tcTabSecBon.Controls.Add(tpSectors);
            tpBonuses = new TabPage();
            tpBonuses.Text = "Бонусы";
            tcTabSecBon.Controls.Add(tpBonuses);
            tbSectors = new TextBox();
            tbSectors.Multiline = true;
            tbSectors.ScrollBars = ScrollBars.Both;
            string sec1 = "";
            for(int i = 0; i < level.sectors; i++) { sec1 = sec1 + (i+1).ToString() + ": " + level.sector[i] + "\r\n"; }
            tbSectors.Text = sec1;
            tpSectors.Controls.Add(tbSectors);
            tbBonuses = new TextBox();
            tbBonuses.Multiline = true;
            tbBonuses.ScrollBars = ScrollBars.Both;
            string bon1 = "";
            for (int i = 0; i < level.bonuses; i++) { bon1 = bon1 + (i+1).ToString() + ": " + level.bonus[i] + "\r\n"; }
            tbBonuses.Text = bon1;
            tpBonuses.Controls.Add(tbBonuses);

            Event_ChangeSize(this, null);
            Event_Change_cbType(this, null);
        }

        private void Event_Change_cbRaschl(object sender, EventArgs e)
        {
            iRaschl = cbRaschl.SelectedIndex + 1;
        }

        private void Event_Change_cbOlimpSize(object sender, EventArgs e)
        {
            int i = cbOlimpSize.SelectedIndex;
            if (i == 0) { iOlimpSize = 0; }
            if (i == 1) { iOlimpSize = 4; }
            if (i == 2) { iOlimpSize = 8; }
            if (i == 3) { iOlimpSize = 16; }
            if (i == 4) { iOlimpSize = 32; }
            if (i == 5) { iOlimpSize = 64; }
            if (i == 6) { iOlimpSize = 128; }
            if (i == 7) { iOlimpSize = 256; }
        }

        private void Event_Change_cbGybrid(object sender, EventArgs e)
        {
            iGybridMin = cbGybrid.SelectedIndex + 2;
        }

        private void Event_Click_btAnswerDot2(object sender, EventArgs e)
        {
            tbAnswerMulti.Text = RemoveCharsBefore(tbAnswerMulti.Text, ":");
        }

        private void Event_Click_btAnswerDot(object sender, EventArgs e)
        {
            tbAnswerMulti.Text = RemoveCharsBefore(tbAnswerMulti.Text, ".");
        }

        private void Event_Click_btAnswerSpace(object sender, EventArgs e)
        {
            tbAnswerMulti.Text = RemoveCharsBefore(tbAnswerMulti.Text, " ");
        }

        private void Event_KeyDown_tbAnswer(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btAnswer.PerformClick();
            }
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
            }
            else
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
            }
            else
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
        
        /// <summary>
                 /// ивент по кнопке "начать решение"
                 /// </summary>
        private void Event_Click_btSolve(object sender, EventArgs e)
        {
            string type = cbType.SelectedItem.ToString();
            if (type == s_picture_only)
            {
                var R1 = new Picture(this);
            }
            if (type == s_picture_olimp)
            {
                var R1 = new Picture(this);
                var R2 = new Olimp(this);
            }
            if (type == s_olimp_only)
            {
                var R2 = new Olimp(this);
            }
            if (type == s_picture_gybrid)
            {
                var R1 = new PicsGybrids(this);
            }
            if (type == s_gapoifaka_books)
            {
                var R1 = new GapoifikaBooks(this);
            }
            if (type == s_ledida_books)
            {
                var R1 = new LedidaBooks(this);
            }
            if (type == s_gapoifaka_films)
            {
                var R1 = new GapoifikaFilms(this);
            }
            if (type == s_raschlenenki)
            {
                var R1 = new Raschl(this);
            }
            // 
            btSolve.Enabled = false;
        }

        /// <summary>
        /// ивент при изменении типа задания. перерисовывает элементы управления
        /// </summary>
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
            lbGybrid.Visible = false;
            cbGybrid.Visible = false;
            lbOlimpSize.Visible = false;
            cbOlimpSize.Visible = false;
            lbRaschl.Visible = false;
            cbRaschl.Visible = false;
            //

            string type = cbType.SelectedItem.ToString();

            //
            if (type == s_olimp_only)
            {
                List<object> objs = new List<object>();
                objs.Add(lbProtect);
                objs.Add(cbProtect);
                objs.Add(lbOlimpSize);
                objs.Add(cbOlimpSize);
                objs.Add(lbSolve);
                objs.Add(btSolve);
                ShowSettingsOnScreen(objs, SettingsPositions);
                //isPicsSect = true;
            }
            if ((type == s_picture_only) || (type == s_picture_olimp))
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
            if (type == s_picture_gybrid)
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
                objs.Add(lbGybrid);
                objs.Add(cbGybrid);
                objs.Add(lbSolve);
                objs.Add(btSolve);
                ShowSettingsOnScreen(objs, SettingsPositions);
                //isPicsSect = true;
            }
            if ((type == s_gapoifaka_books) || type == (s_gapoifaka_films) || (type == s_ledida_books))
            {
                List<object> objs = new List<object>();
                objs.Add(lbSolve);
                objs.Add(btSolve);
                ShowSettingsOnScreen(objs, SettingsPositions);
            }
            if (type == s_raschlenenki)
            {
                List<object> objs = new List<object>();
                objs.Add(lbRaschl);
                objs.Add(cbRaschl);
                objs.Add(lbSolve);
                objs.Add(btSolve);
                ShowSettingsOnScreen(objs, SettingsPositions);
            }
            //


        }

        /// <summary>
        /// установка видимости набора объектов управления, расположение их последовательно по вертикали
        /// </summary>
        /// <param name="objs">список элементов управления</param>
        /// <param name="sp2">начальная координата по высоте</param>
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

        /// <summary>
        /// ивент - изменение размера всех объектов ГУИ
        /// </summary>
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
            tbAnswer.Width = width1;// - 10 * border;

            lbAnswerMulti.Top = tbAnswer.Bottom + 2 * border;
            lbAnswerMulti.Left = left1;
            lbAnswerMulti.Width = width1 - 10 * border;
            tbAnswerMulti.Top = lbAnswerMulti.Bottom + border;
            tbAnswerMulti.Left = left1;
            tbAnswerMulti.Width = width1 - 10 * border;
            tbAnswerMulti.Height = 15 * border;

            btAnswer.Top = lbAnswerMulti.Top;
            btAnswer.Left = tbAnswerMulti.Right + border;
            btAnswer.Width = 8 * border;
            btAnswerSpace.Top = btAnswer.Bottom + 2 * border;
            btAnswerSpace.Left = btAnswer.Left;
            btAnswerSpace.Width = btAnswer.Width;
            btAnswerDot.Top = btAnswerSpace.Bottom + 1;
            btAnswerDot.Left = btAnswer.Left;
            btAnswerDot.Width = btAnswer.Width;
            btAnswerDot2.Top = btAnswerDot.Bottom + 1;
            btAnswerDot2.Left = btAnswer.Left;
            btAnswerDot2.Width = btAnswer.Width;

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
            lbGybrid.Left = left1;
            lbGybrid.Width = width1;
            cbGybrid.Left = left1;
            cbGybrid.Width = width1;
            lbRaschl.Left = left1;
            lbRaschl.Width = width1;
            cbRaschl.Left = left1;
            cbRaschl.Width = width1;
            lbOlimpSize.Left = left1;
            lbOlimpSize.Width = width1;
            cbOlimpSize.Left = left1;
            cbOlimpSize.Width = width1;

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
            tcTabSecBon.Top = top;
            tcTabSecBon.Left = left3;
            tcTabSecBon.Width = width3;
            tcTabSecBon.Height = height - (3 * border);
            tbSectors.Top = border;
            tbSectors.Left = border;
            tbSectors.Width = tcTabText.Width - 3 * border;
            tbSectors.Height = tcTabText.Width - 3 * border;
            tbBonuses.Top = border;
            tbBonuses.Left = border;
            tbBonuses.Width = tcTabText.Width - 3 * border;
            tbBonuses.Height = tcTabText.Width - 3 * border;
        }

        /// <summary>
        /// ивент на кнопки - подготовка мульти-ответа -- убираем все символы до указанного ( ":" / "." / " ")
        /// </summary>
        /// <param name="text">текст мультиответа</param>
        /// <param name="v">символ, до которого всё убирается ( ":" / "." / " ")</param>
        /// <returns>текст с разделителем \r\n</returns>
        private string RemoveCharsBefore(string text, string v)
        {
            string res = "";
            string[] ar1 = System.Text.RegularExpressions.Regex.Split(text, "\r\n");
            foreach (string s in ar1)
            {
                string s1 = s.Trim().Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                if (s1 != "")
                {
                    int ii1 = s1.IndexOf(v);
                    if (ii1 != -1)
                    {
                        s1 = s1.Substring(ii1 + 1);
                    }
                    res = res + s1.Trim() + "\r\n";
                }
            }
            return res;
        }


    }
}
