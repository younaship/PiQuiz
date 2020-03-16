using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 19.6.15
 * 
 * 文字列の変換を行う独自クラスです
 * ver 0.1.0
 * 
 */

namespace MyConverter
{
    public class KanaConverter
    {
        const string KEYMAP_KANA = "アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲン" +
            "ガギグゲゴザジズゼゾダヂヅデドバビブベボパピプペポ" +
            "ァィゥェォッヮャュョ";
        const string KEYMAP_HIRAGANA = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをん" +
            "がぎぐげござじずぜぞだぢづでどばびぶべぼぱぴぷぺぽ" +
            "ぁぃぅぇぉっゎゃゅょ";
        /// <summary>
        /// 入力文字列をすべてひらがなに変換します。
        /// </summary>
        public static string ToHiragana(string str)
        {
            int index;
            string s = "";
            for (int i = 0; i < str.Length; i++)
                if ((index = KEYMAP_KANA.IndexOf(str[i])) != -1)
                    s += KEYMAP_HIRAGANA[index];
                else
                    s += str[i];
            return s;
        }
    }
}
