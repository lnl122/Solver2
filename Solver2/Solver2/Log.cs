﻿// Copyright © 2016 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;

namespace Solver2
{
    //
    // public void Init()
    // public void Close()
    // public void Write(string text, string text2="", string text3 = "")
    // public void Store(string modulename, string pagetext)
    //
    class Log
    {
        private static object threadLockWrite = new object();
        private static object threadLockStore = new object();

        private static string PathToPages = "";         // путь (без слеша в конце, к папке для сохраняемых страниц
        private static System.IO.StreamWriter logfile;  // поток лога
        public static bool isReady = false;             // инициализация проведена?
        private static int fileidx = 1;                 // индекс/номер сохраняемого файла

        /// <summary>
        /// записывает строку текста в лог-файл
        /// </summary>
        /// <param name="str">строка для лог файла</param>
        /// <param name="str2">вторая строка для лог файла</param>
        /// <param name="str3">третья строка для лог файла</param>
        public static void Write(string str, string str2 = "", string str3 = "")
        {
            if (isReady)
            {
                lock (threadLockWrite)
                {
                    logfile.WriteLine("{0} {1} {2}", DateTime.Today.ToShortDateString(), DateTime.Now.ToLongTimeString(), str);
                    if (str2 != "")
                    {
                        logfile.WriteLine("                          " + str2);
                        if (str3 != "")
                        {
                            logfile.WriteLine("                          " + str3);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// записывает текст в отдельный файл
        /// </summary>
        /// <param name="modulename">имя модуля</param>
        /// <param name="text">строка текста</param>
        public static void Store(string modulename, string text)
        {
            if (isReady)
            {
                lock (threadLockStore)
                {
                    fileidx++;
                    var dt = DateTime.Today;
                    var dn = DateTime.Now;
                    string path = PathToPages + "\\" + modulename + "_" + fileidx.ToString() + "_" +
                        dt.Year.ToString() + dt.Month.ToString() + dt.Day.ToString() +
                        dn.Hour.ToString() + dn.Minute.ToString() + dn.Second.ToString() + ".http";
                    System.IO.File.WriteAllText(path, text, System.Text.Encoding.UTF8);

                }
            }
        }

        /// <summary>
        /// выполняет принудительную запись лога на диск
        /// </summary>
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

        /// <summary>
        /// если папка есть, или если не было, но удалось создать - возвращает путь к ней, иначе - базовый путь
        /// </summary>
        /// <param name="basepath">базовый путь</param>
        /// <param name="folder">имя папки</param>
        /// <returns>путь к папке</returns>
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

        /// <summary>
        /// инициализирует лог файл, если нету его - создает. в т.ч. необходимые папки
        /// </summary>
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
