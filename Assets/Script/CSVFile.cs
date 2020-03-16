using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

/* CSVFile.cs (19.04.04) Ver 0.1 
 * @ Based XFiler.cs (19.02.20) Ver 0.1
 * オリジナルのクラスの変数を保存するためにCSV形式にしたり復元したりできるクラス
 * 
 *  public T[] GetFromCSV(string csv)
 *  CSVファイルからクラスに変換します
 * 
 *  public GetFromT(MyClass[] , char entercode="\n" )
 *  クラスの変数をCSV形式に変換します。
 */

public class CSVFile<T> where T : new()
{
    FieldInfo[] info;

    public CSVFile()
    {
        info = typeof(T).GetFields();
    }

    public T[] GetFromCSV(string csv) // tagName : データーとして扱うタグ名 (例:<a>data</a> -> a)
    {
        StringReader reader = new StringReader(csv);
        List<T> Classes = new List<T>();
        string str;
        while ((str = reader.ReadLine()) != null)
        {
            string[] sp = str.Trim().Split(',');
            if (sp.Length != info.Length) throw new System.Exception("No Match Index Count. [" + sp.Length + "]!=[" + info.Length + "]");

            T Class = new T();

            for (int i = 0; i < info.Length; i++)
            {
                FieldInfo f = typeof(T).GetField(info[i].Name);
                if (info[i].FieldType == typeof(int)) f.SetValue(Class, int.Parse(sp[i]));
                else if (info[i].FieldType == typeof(float)) f.SetValue(Class, float.Parse(sp[i]));
                else if (info[i].FieldType == typeof(string)) f.SetValue(Class, sp[i]);
                else
                {
                    var v = Convert.ChangeType(sp[i], info[i].FieldType);
                    f.SetValue(Class, v);
                }
            }
            reader.Close();
            Classes.Add(Class);
        }

        return Classes.ToArray();
    }

    public string GetFromT(T[] type, char enterCode = '\n')
    {
        FieldInfo[] info = typeof(T).GetFields();
        string s = "";

        int r = 0;
        foreach (T t in type)
        {
            r++;
            for (int i = 0; i < info.Length; i++)
            {
                FieldInfo f = typeof(T).GetField(info[i].Name);
                s += f.GetValue(t).ToString();
                if (i < info.Length - 1) s += ",";
            }
            if (r != type.Length) s += enterCode;
        }
        return s;

    }
}