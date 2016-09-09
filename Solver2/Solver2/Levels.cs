// нет смысла в модуле. вероятно предстоит убить его нафиг.

namespace Solver2
{
    class Levels
    {
        public static GameSelect Game;

        public Level[] L;

        public Levels(GameSelect GameParams)
        {
            Game = GameParams;
            if (Game.isStorm == true) { L = new Level[Game.gamelevels]; } else { L = new Level[1]; }
            // *** доделать отдельную ветки для линейных МШ
            // весь код ниже пока относиться (08.09.16) только к штурмам

            for (int i = 0; i < Game.gamelevels; i++)
            {
                L[i] = new Level(Game, i + 1);
            }
        }
    }
}
