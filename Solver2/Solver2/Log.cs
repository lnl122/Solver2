using System;

namespace Solver2
{
    // public void Init()
    // public void Close()
    // public void Write(string text, string text2="", string text3 = "")
    // public void Store(string modulename, string pagetext)
    //
    class Log
    {
        private static string PathToPages = "";         // путь (без слеша в конце, к папке для сохраняемых страниц
        private static System.IO.StreamWriter logfile;  // поток лога
        public static bool isReady = false;             // инициализация проведена?
        private static bool isBusyWrite = false;             // счас заняты? чтоб подождать если необходимо. для устранения коллизий при активном логгировании
        private static bool isBusyStore = false;             // счас заняты? чтоб подождать если необходимо. для устранения коллизий при активном логгировании
        private static int fileidx = 1;                 // индекс/номер сохраняемого файла

        // записывает строку текста в лог-файл
        // вход     строка для лог файла
        // выход    -
        public static void Write(string str, string str2 = "", string str3 = "")
        {
            if (isReady)
            {
                while (isBusyWrite) { } // ожидание освобождения флага
                isBusyWrite = true;
                logfile.WriteLine("{0} {1} {2}", DateTime.Today.ToShortDateString(), DateTime.Now.ToLongTimeString(), str);
                if (str2 != "")
                {
                    logfile.WriteLine("                          " + str2);
                    if (str3 != "")
                    {
                        logfile.WriteLine("                          " + str3);
                    }
                }
                isBusyWrite = false;
            }
        }

        // записывает текст в отдельный файл
        // вход     имя модуля, строка текста
        // выход    -
        public static void Store(string modulename, string text)
        {
            fileidx++;
            if (isReady)
            {
                while (isBusyStore) { } // ожидание освобождения флага
                isBusyStore = true;
                var dt = DateTime.Today;
                var dn = DateTime.Now;
                string path = PathToPages + "\\" + modulename + "_" + fileidx.ToString() + "_" +
                    dt.Year.ToString() + dt.Month.ToString() + dt.Day.ToString() +
                    dn.Hour.ToString() + dn.Minute.ToString() + dn.Second.ToString() + ".http";
                try
                {
                    System.IO.File.WriteAllText(path, text, System.Text.Encoding.UTF8);
                }
                catch
                {
                    int i = 0;/// *** возникают коллизии при записи файлов.
                }
                isBusyStore = false;
            }
        }

        // выполняет принудительную запись лога на диск
        // вход     -
        // выход    -
        public static void Close()
        {
            if (isReady)
            {
                logfile.Flush();
                logfile.Close();
                logfile = null;
                isReady = false;
            }
        }

        // если папка есть, или если не было, но удалось создать - возвращает путь к ней, иначе - базовый путь
        // вход     базовый путь, имя папки
        // выход    путь к папке
        private static string CheckCreateFolder(string basepath, string folder)
        {
            string path = basepath + @"\" + folder;
            if (System.IO.Directory.Exists(path) == false)
            {
                try
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                catch
                {
                    path = basepath;
                }
            }
            return path;
        }

        // инициализирует лог файл, если нету его - создает. в т.ч. необходимые папки
        // вход     -
        // выход    -
        public static void Init()
        {
            string local_path = Environment.CurrentDirectory;
            string self_name = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
            string PathToLogs = CheckCreateFolder(local_path, "Log");
            string PathToData = CheckCreateFolder(local_path, "Data");
            string PathToPics = CheckCreateFolder(local_path, "Pics");
            PathToPages = CheckCreateFolder(local_path, "Pages");
            string pathfilename = PathToLogs + "\\" + self_name + ".log";
            logfile = new System.IO.StreamWriter(System.IO.File.AppendText(pathfilename).BaseStream);
            logfile.AutoFlush = true;
            isReady = true;
        }
    }
}
