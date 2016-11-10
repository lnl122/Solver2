// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using Microsoft.Win32;
using System.Windows.Forms;

namespace Solver2
{
    class Logon
    {
        private int border = 5; // расстояния между элементами форм, константа
        private string url_game_en_cx = "http://game.en.cx/Login.aspx";
        private TextBox tu;
        private TextBox tp;
        public bool isSuccessful = false;
        public string username = "";
        public string password = "";
        public string userid = "";

        /// <summary>
        /// получает из реестра ник
        /// </summary>
        /// <returns>ник</returns>
        private string GetRegistryUser()
        {
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser;
            RegistryKey rks = rk.OpenSubKey("Software", true); rk.Close();
            RegistryKey rksl = rks.OpenSubKey("lnl122", true); if (rksl == null) { rksl = rks.CreateSubKey("lnl122"); }
            rks.Close();
            RegistryKey rksls = rksl.OpenSubKey("Solver", true); if (rksls == null) { rksls = rksl.CreateSubKey("Solver"); }
            rksl.Close();
            string user = "";
            var r_user = rksls.GetValue("user");
            if (r_user == null) { rksls.SetValue("user", ""); user = ""; } else { user = r_user.ToString(); }
            rksls.Close();
            return user;
        }

        /// <summary>
        /// получает из реестра пароль
        /// </summary>
        /// <returns>пароль</returns>
        private string GetRegistryPass()
        {
            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser;
            RegistryKey rks = rk.OpenSubKey("Software", true); rk.Close();
            RegistryKey rksl = rks.OpenSubKey("lnl122", true); if (rksl == null) { rksl = rks.CreateSubKey("lnl122"); }
            rks.Close();
            RegistryKey rksls = rksl.OpenSubKey("Solver", true); if (rksls == null) { rksls = rksl.CreateSubKey("Solver"); }
            rksl.Close();
            string pass = "";
            var r_pass = rksls.GetValue("pass");
            if (r_pass == null) { rksls.SetValue("pass", ""); pass = ""; } else { pass = r_pass.ToString(); }
            rksls.Close();
            return pass;
        }

        /// <summary>
        /// сохраняет в реестре ник и пароль
        /// </summary>
        /// <param name="u">ник</param>
        /// <param name="p">пароль</param>
        private void SetRegistryUserPass(string u, string p)
        {
            RegistryKey rk2 = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\lnl122\\Solver", true);
            rk2.SetValue("user", u);
            rk2.SetValue("pass", p);
            rk2.Close();
        }

        /// <summary>
        /// выполняет логон
        /// </summary>
        public Logon()
        {
            // нужная ветка реестра д.б. в HKCU - //[HKEY_CURRENT_USER\Software\lnl122\solver] //"user"="username" //"pass"="userpassword"
            // обратимся к реестру, есть ли там записи о последнем юзере, если есть - прочтем их
            string user = GetRegistryUser();
            string pass = GetRegistryPass();

            // форма для ввода данных
            Form Login = CreateForm(user, pass);

            // предложим ввести юзера и пароль, дефолтные значения - то, что было в реестре, или же пусто
            bool fl = true;
            while (fl)
            {
                if (Login.ShowDialog() == DialogResult.OK)
                {
                    // попробуем авторизоваться на гейм.ен.цх с указанной УЗ
                    user = tu.Text;
                    pass = tp.Text;
                    Log.Write("Пробуем выполнить вход на сайт для пользвоателя " + user);
                    string pageSource = Engine.Logon(url_game_en_cx, user, pass);
                    // если авторизовались успешно
                    if (pageSource.IndexOf("action=logout") != -1)
                    {
                        // обновить в реестре 
                        SetRegistryUserPass(user, pass);
                        // запомним параметры игрока
                        pageSource = pageSource.ToLower();
                        try
                        {
                            pageSource = pageSource.Substring(pageSource.IndexOf(user.ToLower()));
                            pageSource = pageSource.Substring(pageSource.IndexOf("(id"));
                            pageSource = pageSource.Substring(pageSource.IndexOf(">") + 1);
                            userid = pageSource.Substring(0, pageSource.IndexOf("<"));
                            username = user;
                            password = pass;
                            isSuccessful = true;
                            // поставим флаг выхода
                            fl = false;
                            // в лог
                            Log.Write("Имя и пароль пользователя проверены, успешный логон для id=" + userid);
                            // отобразим на форме
                            Program.D.F.Text = Program.D.F.Text + " / " + username;
                        }
                        catch
                        {
                            // если была ошибка в парсинге
                            userid = "";
                            username = user;
                            password = pass;
                            isSuccessful = false;
                            // поставим флаг выхода
                            fl = false;
                            // в лог
                            Log.Write("Не получилось определить id польвзоателя, хотя логон прошел успешно. ник=" + user);
                        }
                    }
                    else
                    {
                        // если не успешно - вернемся в вводу пользователя
                        userid = "";
                        username = "";
                        password = "";
                        isSuccessful = false;
                        Log.Write("Неверные логин/пароль");
                        MessageBox.Show("Неверные логин/пароль");
                    }
                }
                else
                {
                    // если отказались вводить имя/пасс - выходим
                    fl = false;
                }
            } // выход только если fl = false -- это или отказ пользователя в диалоге, или если нажато ОК - корректная УЗ
        }

        /// <summary>
        /// создаем экранную форму логона
        /// </summary>
        /// <param name="user">ник</param>
        /// <param name="pass">пароль</param>
        /// <returns>форма</returns>
        private Form CreateForm(string user, string pass)
        {
            Form Login = new Form();
            Login.Text = "Авторизация..";
            Login.StartPosition = FormStartPosition.CenterScreen;
            Login.Width = 35 * border;
            Login.Height = 25 * border;
            Login.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Login.AutoSize = true;
            Login.Icon = Properties.Resources.icon2;
            Label lu = new Label();
            lu.Text = "ник:";
            lu.Top = 2 * border;
            lu.Left = border;
            lu.Width = 10 * border;
            Login.Controls.Add(lu);
            Label lp = new Label();
            lp.Text = "пароль:";
            lp.Top = lu.Bottom + border;
            lp.Left = border;
            lp.Width = lu.Width;
            Login.Controls.Add(lp);
            tu = new TextBox();
            tu.Text = user;
            tu.Top = lu.Top;
            tu.Left = lu.Right + border;
            tu.Width = 3 * lu.Width;
            Login.Controls.Add(tu);
            tp = new TextBox();
            tp.Text = pass;
            tp.Top = lp.Top;
            tp.Left = tu.Left;
            tp.Width = tu.Width;
            Login.Controls.Add(tp);
            Button blok = new Button();
            blok.Text = "выполнить вход";
            blok.Top = lp.Bottom + 2 * border;
            blok.Left = lu.Left;
            blok.Width = tu.Right - 1 * border;
            blok.DialogResult = DialogResult.OK;
            Login.AcceptButton = blok;
            Login.Controls.Add(blok);
            return Login;
        }
    }
}
