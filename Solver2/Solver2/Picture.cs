using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solver2
{
    class Picture
    {
        /*private string qq(List<string> ww)
        {
            string res = "";
            foreach(string ss in ww) { res = res + ss + " "; }
            return (res + " | ");
        }*/

        public static string GetPicsHtml(string[] ar)
        {
            string html = "<!DOCTYPE HTML><html><head><meta charset=\"utf-8\"></head><body bgcolor=\"#111111\"><br/>";
            for (int i = 0; i < ar.Length; i++)
            {
                html = html + "<img src=\"" + ar[i] + "\" width=\"150\" height=\"150\" alt=\"" + (i + 1).ToString() + "\"><!-- " + (i + 1).ToString() + " -->";
            }
            html = html + "<br/></body></html>";
            return html;
        }

        // распознание и вбивание картинок
        public bool Process(OneTab T)
        {
            // создаем массив путей из больших картинок
            string[] SmallImagePath = Image.GetSmallImagePathes(T, T.level.urls, T.iRows, T.iCols);

            // отрисуем их в ГУИ
            string html = GetPicsHtml(SmallImagePath);
            T.wbPictures.Invoke(new Action(() => { T.wbPictures.DocumentText = html; }));

            // из путей к картинкам делаем коллекции слов
            List<Words> TextsFromPics = Image.GetAllDescriptions(SmallImagePath);

            //*** для теста
            /*
            for(int i=0; i<TextsFromPics.Count; i++)
            {
                Log.Write("_____________");
                Log.Write((i+1).ToString());
                Log.Write("_____________");
                Words ww = TextsFromPics[i];
                Log.Write(" | ww.ru | " + qq(ww.ru));
                Log.Write(" | ww.ru_check | " + qq(ww.ru_check));
                Log.Write(" | ww.en | " + qq(ww.en));
                Log.Write(" | ww.en_trans | " + qq(ww.en_trans));
                Log.Write(" | ww.f_b_noun | " + qq(ww.f_b_noun));
                Log.Write(" | ww.f_b_adjective | " + qq(ww.f_b_adjective));
                Log.Write(" | ww.f_b_verb | " + qq(ww.f_b_verb));
                Log.Write(" | ww.f_b_others | " + qq(ww.f_b_others));
                Log.Write(" | ww.all_base | " + qq(ww.all_base));
                Log.Write(" | ww.all_base10 | " + qq(ww.all_base10));
                Log.Write(" | ww.all_assoc | " + qq(ww.all_assoc));
                Log.Write(" | ww.all_assoc25 | " + qq(ww.all_assoc25));
                Log.Write(" | ww.all_find | " + qq(ww.all_find));
                */
            /*
    public List<string> ru;        // слова только из русских букв
    public List<string> ru_check;  // русские слова после орфографии
    public List<string> en;        // слова только из английских букв
    public List<string> en_trans;  // переведенные английские слова
    public List<string> all_find;  // собранные слова без дубликатов в оригинале (ворд_ру + енг_перевод), ранжированные по частоте
    public List<string> f_b_noun;      // сущ
    public List<string> f_b_adjective; // прил
    public List<string> f_b_verb;      // глагол
    public List<string> f_b_others;    // прочие
    public List<string> all_base;  // все слова из найденных, приведенную в базовую форму, ранжированные по частоте
    public List<string> all_base10;  // топовых 10 слов, из найденных, приведенную в базовую форму, ранжированные по частоте
    public List<string> all_assoc; // ассоциации к найденным словам, все подряд
    public List<string> all_assoc25; // ассоциации к найденным словам, все подряд

        }
    */
            // пробуем вбить ответы (то есть передать их во вбиватор, включая указание приоритета)
            for (int i=0; i<TextsFromPics.Count; i++)
            {
                Words W = TextsFromPics[i];
                if (W != null)
                {
                    Answer.Add(T, 4, Words.KillDupesAndRange(W.all_base10, 5), i);
                }
                //*** позже добавить более низкие приоритеты
            }
            T.tbSectors.Invoke(new Action(() => { T.btSolve.Enabled = true; }));
            
            return true;
        }

        // разпознать и вбить слова от картинок, создание потока
        public Picture(OneTab T)
        {
            if (T.cbImageCuttingMethod.SelectedItem.ToString() == "Указан в ручную, равные доли")
            {
                Log.Write("Pics Начали решать картинки\r\n.\r\n");
                Task<bool> t1 = Task<bool>.Factory.StartNew(() => Process(T));
            }
        }
    }
}