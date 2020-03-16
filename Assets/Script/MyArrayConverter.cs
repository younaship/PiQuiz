using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* MyArrayConverter.cs (19.04.04)
 * 
 * stringを独自文字で区切るだけのクラス 
 */

public class MyArrayConverter<T>
{
    public static string ToString(T[] array, char breakChar = '`') // 初期値「 ` 」
    {
        string s = "";
        for (int i = 0; i < array.Length; i++)
        {
            s += array[i].ToString();
            if (i - 1 != array.Length) s += breakChar;
        }
        return s;
    }

    public static T[] ToArray(string str, char breakChar = '`')
    {
        string[] s = str.Split(breakChar);
        T[] ts = new T[s.Length];
        for (int i = 0; i < ts.Length; i++)
        {
            if (typeof(T) == typeof(int)) ts[i] = (T)(object)int.Parse(s[i]);
            else if (typeof(T) == typeof(double)) ts[i] = (T)(object)double.Parse(s[i]);
            else if (typeof(T) == typeof(float)) ts[i] = (T)(object)float.Parse(s[i]);
            else ts[i] = (T)((object)s[i]);
        }
        return ts;
    }
}