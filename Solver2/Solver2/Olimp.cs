using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solver2
{
    class Olimp
    {
        OneTab OT;
        private int[] links;
        private List<string> sended;

        public struct OneOlimp
        {
            public int num;
            public List<int> nums;
            public string id;
        }
        private List<OneOlimp> tasks;

        // решение олимпиек
        public bool Process(OneTab T)
        {
            OT = T;
            int PicsCount = Image.GetSmallImageCount(T.level.urls, T.iRows, T.iCols);
            CreateLinksTable(PicsCount);
            sended = new List<string>();

            int oldsecbon = T.level.secbon;
            tasks = FindBlanks(T.level);
            foreach(OneOlimp oo in tasks) { SolveOne(T.level, oo); }

            bool isTrue = true;
            while (isTrue == true)
            {
                while(T.level.secbon == oldsecbon) { System.Threading.Thread.Sleep(5000); }
                oldsecbon = T.level.secbon;
                tasks = FindBlanks(T.level);
                foreach (OneOlimp oo in tasks) { SolveOne(T.level, oo); }
            }

            //T.tbSectors.Invoke(new Action(() => { T.btSolve.Enabled = true; }));
            return true;
        }

        // получает ответы из структуры игры по номеру сектора
        // вход - Уровень, номер сектора
        // выход - коллекция слов
        private List<string> GetSec(Level lvl, int i)
        {
            string[] ar = lvl.sector[i-1].Split(' ');
            List<string> res = new List<string>();
            foreach (string s in ar) { if (s != "") { res.Add(s); } }
            return res;
        }

        // решает один сектор
        // вход - Уровень, структура стыковки
        private void SolveOne(Level lvl, OneOlimp oo)
        {
            List<string> wrd3 = new List<string>();
            List<string> wrd2 = new List<string>();
            List<string> wrd1 = new List<string>();
            List<List<string>> wrds = new List<List<string>>();
            foreach(int i in oo.nums) { wrds.Add(GetSec(lvl, i)); }
            if(oo.nums.Count == 4)
            {
                wrd3.AddRange(Get3(oo.num, wrds, oo.nums, 0, 1, 2));
                wrd3.AddRange(Get3(oo.num, wrds, oo.nums, 0, 1, 3));
                wrd3.AddRange(Get3(oo.num, wrds, oo.nums, 0, 2, 3));
                wrd3.AddRange(Get3(oo.num, wrds, oo.nums, 1, 2, 3));
                wrd2.AddRange(Get2(oo.num, wrds, oo.nums, 0, 1));
                wrd2.AddRange(Get2(oo.num, wrds, oo.nums, 0, 2));
                wrd2.AddRange(Get2(oo.num, wrds, oo.nums, 0, 3));
                wrd2.AddRange(Get2(oo.num, wrds, oo.nums, 1, 2));
                wrd2.AddRange(Get2(oo.num, wrds, oo.nums, 1, 3));
                wrd2.AddRange(Get2(oo.num, wrds, oo.nums, 2, 3));
                wrd1.AddRange(Get1(oo.num, wrds, oo.nums, 0));
                wrd1.AddRange(Get1(oo.num, wrds, oo.nums, 1));
                wrd1.AddRange(Get1(oo.num, wrds, oo.nums, 2));
                wrd1.AddRange(Get1(oo.num, wrds, oo.nums, 3));
            }
            if (oo.nums.Count == 3)
            {
                wrd3.AddRange(Get3(oo.num, wrds, oo.nums, 0, 1, 2));
                wrd2.AddRange(Get2(oo.num, wrds, oo.nums, 0, 1));
                wrd2.AddRange(Get2(oo.num, wrds, oo.nums, 0, 2));
                wrd2.AddRange(Get2(oo.num, wrds, oo.nums, 1, 2));
                wrd1.AddRange(Get1(oo.num, wrds, oo.nums, 0));
                wrd1.AddRange(Get1(oo.num, wrds, oo.nums, 1));
                wrd1.AddRange(Get1(oo.num, wrds, oo.nums, 2));
            }
            if (oo.nums.Count == 2)
            {
                wrd2.AddRange(Get2(oo.num, wrds, oo.nums, 0, 1));
                wrd1.AddRange(Get1(oo.num, wrds, oo.nums, 0));
                wrd1.AddRange(Get1(oo.num, wrds, oo.nums, 1));
            }
            if (oo.nums.Count == 1)
            {
                wrd1.AddRange(Get1(oo.num, wrds, oo.nums, 0));
            }
            //public static void Add(OneTab T, int priority, List<string> WordsList, int i1, int i2 = -1, int i3 = -1)
            Answer.Add(OT, 3, wrd3, oo.num);
            Answer.Add(OT, 4, wrd2, oo.num);
            Answer.Add(OT, 5, wrd1, oo.num);
        }

        // решение при трех ассоциациях
        private List<string> Get3(int num, List<List<string>> wrds, List<int> nums, int v1, int v2, int v3)
        {
            List<string> res = new List<string>();
            string id = num.ToString() + "=" + nums[v1].ToString() + "," + nums[v2].ToString() + "," + nums[v3].ToString();
            if (!sended.Contains(id))
            {
                sended.Add(id);
                res.AddRange(Associations.Get3(wrds[v1], wrds[v2], wrds[v3]));
            }
            return Associations.GetFirstItems(res, 10);
        }

        // решение при двух ассоциациях
        private List<string> Get2(int num, List<List<string>> wrds, List<int> nums, int v1, int v2)
        {
            List<string> res = new List<string>();
            string id = num.ToString() + "=" + nums[v1].ToString() + "," + nums[v2].ToString();
            if (!sended.Contains(id))
            {
                sended.Add(id);
                res.AddRange(Associations.Get2(wrds[v1], wrds[v2]));
            }
            return Associations.GetFirstItems(res,10);
        }

        // решение при одной ассоциации
        private List<string> Get1(int num, List<List<string>> wrds, List<int> nums, int v)
        {
            List<string> res = new List<string>();
            return res;
            string id = num.ToString() + "=" + nums[v].ToString();
            if (!sended.Contains(id))
            {
                sended.Add(id);
                res.AddRange(Associations.Get(wrds[v]));
            }
            return Associations.GetFirstItems(res,10);
        }

        // для нерешенных секторов ищет решенные стыковки, которые можно решить
        // вход - объект уровня
        // выход - набор задач по стыковке
        private List<OneOlimp> FindBlanks(Level lvl)
        {
            List<OneOlimp> res = new List<OneOlimp>();
            for (int i = 0; i < lvl.sectors; i++)
            {
                if (lvl.sector[i].Replace(" ", "") == "") {
                    List<int> nums = GetLinks(i + 1);
                    List<int> bad = new List<int>();
                    foreach (int j in nums) { if (lvl.sector[j - 1].Replace(" ", "") == "") { bad.Add(j); } }
                    foreach (int j in bad) { nums.Remove(j); }
                    if (nums.Count < 2)
                    {
                        foreach (int j in bad) { nums.AddRange(GetLinks(j)); }
                        List<int> bad2 = new List<int>();
                        foreach (int j in nums) { if (lvl.sector[j - 1].Replace(" ", "") == "") { bad2.Add(j); } }
                        foreach (int j in bad2) { nums.Remove(j); }
                    }
                    nums.Sort();
                    string id = (i+1).ToString();
                    foreach(int j in nums) { id = id + "," + j.ToString(); }
                    // i - номер сектора, который будем искать 0..max-1
                    // nums - сектора (1-4), от которых зависит решение
                    // id - строка идентификатор
                    if (nums.Count > 0)
                    {
                        OneOlimp temp = new OneOlimp();
                        temp.num = i;
                        temp.nums = nums;
                        temp.id = id;
                        res.Add(temp);
                    }
                }
            }
            return res;
        }

        // создание таблицы связей
        // вход = количество картинок
        // выхода нет, links заполнен
        private void CreateLinksTable(int PicsCount)
        {
            // расчет таблицы связей ячеек олимпийки
            int SizeOlimp = PicsCount * 2;
            links = new int[SizeOlimp];
            for (int i = 0; i < SizeOlimp; i++)
            {
                links[i] = SizeOlimp - (int)Math.Floor((SizeOlimp - i) / 2.0);
            }
            links[0] = -1;
            links[SizeOlimp - 1] = -1;
        }

        // находит связи в таблице
        // вход - номер сектора
        // выход - набор номеров, прямо связанных с искомым
        private List<int> GetLinks(int num)
        {
            List<int> res = new List<int>();
            if ((num !=0) && (num != links.Length-1))
            {
                res.Add(links[num]);
            }
            for(int i = 1; i<links.Length-1; i++)
            {
                if(links[i] == num)
                {
                    res.Add(i);
                }
            }
            return res;
        }

        // создание потока для ассоциатора олимпиек
        public Olimp(OneTab T)
        {
            Log.Write("Oli Начали решать олимпийки\r\n.\r\n");
            Task<bool> t1 = Task<bool>.Factory.StartNew(() => Process(T));
        }
    }
}
