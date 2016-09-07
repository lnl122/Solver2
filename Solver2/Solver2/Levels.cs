
using System;
using System.Collections.Generic;

namespace Solver2
{
    class Levels
    {
        GameSelect Game;
        public struct level
        {
            public int number;
            public string name;
            public string page;
            public string text;
            public bool isClose;
            public List<string> answers_good;
            public List<string> answers_bad;
            public int sectors;
            public int bonuses;
            public string[] sector;
            public string[] bonus;
            public List<string> urls;
            public string formlevelid;
            public string formlevelnumber;
            public DateTime dt;
        }

        public static level[] L;

        public Levels(GameSelect GameParams)
        {
            Game = GameParams;
        }

    }
}
