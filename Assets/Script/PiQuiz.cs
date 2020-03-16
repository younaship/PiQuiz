using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class PiQuiz 
{
    public PiQuiz() { }

    public Stopwatch stopWatch = new Stopwatch();

    public int Time;

    List<int> getedItemList = new List<int>();
    List<Items> items = new List<Items>();

    /// <summary>
    /// アイテムセット(Json)を読み込みます。Itemsの画像パスを絶対パスに書き換えます。
    /// </summary>
    /// <param name="path">Jsonファイルの保存ディレクトリパス</param>
    /// <param name="jsonFileName">Jsonのファイル名(例：config.json)</param>
    public void LoadFromJson(string path,string jsonFileName) // itemsに登録
    {
        UnityEngine.Debug.Log("Load Json as " + path+jsonFileName);
        if (!FileScaner.ChkFile(path + jsonFileName)) return;
        string json;
        try
        {
            using (Stream stm = new FileStream(path + jsonFileName, FileMode.Open))
            using (StreamReader read = new StreamReader(stm, System.Text.Encoding.UTF8))
            {
                json = read.ReadToEnd();
            }
        }
        catch { return; }
        json.Replace('/', '\\');

        var load = new List<Items>(JsonUtility.FromJson<RootItems>(json).Items);
        foreach (var v in load) v.ImagePath = path + v.ImagePath;
        items.AddRange(load);
        UnityEngine.Debug.Log("Loaded [" + load.Count + "] Items From Json.");
    }

    /// <summary>
    /// 新しい問題を取得します。
    /// </summary>
    public Items GetItem()
    {
        if (items.Count == 0) throw new System.Exception("No QuizItem in List.");
        int tryCount = 0;
        int n = -1;
        do
        {
            var rnd = new System.Random(System.Environment.TickCount + tryCount);
            n = rnd.Next(items.Count);
            if (getedItemList.Contains(n) && tryCount < 100) n = -1;//既にGet済みItemの場合
            else getedItemList.Add(n);// GetするためGetedリストに追加
            tryCount += 10;
        } while (n == -1);
        return items[n];
    }

    /*Use Classes*/

    [Serializable]
    public class Items
    {
        public string Question; // クイズ(文字体)
        public string[] ItemName; // 回答の答え
        public string ImagePath; // クイズの画像
        //public Texture texture; // クイズの画像
        public int Difficulty; // 難易度 (0-9,10-19,20-...)

        public SavedItemStyle GetSavedStyle()
        {
            return new SavedItemStyle()
            {
                Difficulty = Difficulty,
                ImagePath = ImagePath,
                ItemName = MyArrayConverter<string>.ToString(ItemName)
            };
        }

        public static Items Create(SavedItemStyle saved)
        {
            return new Items
            {
                ItemName = MyArrayConverter<string>.ToArray(saved.ItemName),
                ImagePath = saved.ImagePath,
                Difficulty = saved.Difficulty
            };
        }
        public bool ChackAnswer(string ans)
        {
            foreach (string s in ItemName) if (s == ans.Trim()) return true;
            return false;
        }
    }

    [Serializable]
    public class RootItems // Data/Patch/v_0/config.json
    {
        public int Version = 1;
        public int QuizType = 0; // 0 : PiQuiz 1: NormalQuiz ...
        public string ItemTag; // 例："Caractor"
        public Items[] Items;
        //public string Path; // (例)Data/Patch/v_0/
    }

    /// Not Use
    public class SavedItemStyle
    {
        public int Difficulty;
        public string ItemName;
        public string ImagePath;
    }
}