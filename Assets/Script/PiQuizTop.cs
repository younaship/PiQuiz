using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PiQuizTop : MonoBehaviour {

    static readonly string[] MODE_INFO = new string[3] { "[ソロ]タイムアタック", "[ソロ]スコアアタック", "[マルチ]スコアアタック" };
    static readonly string[] DIFFICULTY_MODE = new string[4] {"簡単","普通","難しい","ランダム" };
    static int viewCount = 0;

    private void Awake()
    {
        if (PiQuizOpener.ReturnResult != -1) //結果が返ってきている場合
        {
            Debug.Log("Return Result");
            if (PiQuizOpener.EnableMultiplayer) foreach (string s in PiQuizOpener.PlayerNames) Debug.Log("Name : " + s);

            var res = this.gameObject.AddComponent<ResultWindow>();
            res.SetAction(() =>
            {
                this.gameObject.AddComponent<MySceneManager>().Set("QuizMain", null);
            });

            string title = GetGameModeName(PiQuizOpener.SelectModeNumber);
            title += "\n難易度：" + DIFFICULTY_MODE[PiQuizOpener.SelectDifficulty];
            res.SetTitle(title);


            if (PiQuizOpener.EnableMultiplayer) res.SetResult(PiQuizOpener.ReturnResultMulti, PiQuizOpener.PlayerNames);
            else // ソロプレイヤー
            {
                string message = "";
                int highScore = PlayerPrefs.GetInt("hscore_" + PiQuizOpener.SelectModeNumber+PiQuizOpener.SelectDifficulty, 0);

                if (PiQuizOpener.ReturnResult > highScore)
                {
                    PlayerPrefs.SetInt("hscore_" + PiQuizOpener.SelectModeNumber+ PiQuizOpener.SelectDifficulty, PiQuizOpener.ReturnResult);
                    message += "ハイスコア更新！\n\n";
                    PiQuizOpener.HighScoreThisMode = highScore;
                }
                else message += "[ハイスコア：" + highScore + "]\n\n";
                message +="<今回の結果>\nスコア：" + PiQuizOpener.ReturnResult;
                res.SetMessage(message);
            }
        }

    }

    public static string GetGameModeName(int n)
    {
        switch (n)
        {
            case 1:
                return MODE_INFO[0];
            case 2:
                return MODE_INFO[1];
            case 11:
                return MODE_INFO[2];
        }
        return "";
    }

    // Use this for initialization
    void Start () {
        viewCount++;
        if (viewCount > 1) MyAdCenter.IAdShow();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPushShowList() //パッチのリストを表示します。
    {
        if (Title.FileUpdater == null) return;
        var ls = this.gameObject.AddComponent<ListView>();

        List<ListViewItem> list = new List<ListViewItem>();
        foreach (var v in Title.FileUpdater.PatchOnServer) list.Add(new ListViewItem(v.PatchName, v.Summary, v.Installed));
        ls.SetItems(list.ToArray());
        ls.SetCallBack((x) => ApplyFile(x));
        
    }

    private async Task ApplyFile(ListViewItem[] items) //選択されたファイルを適用します。
    {
        var dia = this.gameObject.AddComponent<PopUpDialog>();
        dia.Set("変更を適用中。");
        await Task.Delay(10);

        int i = 0;
        string str = "";
        var f = Title.FileUpdater;
        foreach (ListViewItem item in items)
        {
            dia.SetAsync("変更を適用中。\n[" + item.name + "]をダウンロードしています。");
            if (item.isChk)
                if (!await f.InstallPatch(item.name)) {
                    dia.Remove();
                    var d = this.gameObject.AddComponent<Dialog>();
                    d.Set("エラー", "不明なエラーが発生しました。",null,null);
                    return;
                };

            if (items[i].isChk) str += items[i].name;
            if (i < items.Length - 1) str += ",";
            i++;
        }

        MyProfiles.SetProfiles(MyProfiles.StringProfile.SelectedPatches, str);
        dia.Remove();
    }

    private void SetJsonSelected()
    {

    }

    public void OnPushPlayGame(int id)
    {
        switch (id)
        {
            case 1:
                PiQuizOpener.SetGameMode(PiQuizOpener.Mode.TimeAttack);
                PiQuizOpener.EnableMultiplayer = false;
                break;
            case 2:
                PiQuizOpener.SetGameMode(PiQuizOpener.Mode.ScoreAttack);
                PiQuizOpener.EnableMultiplayer = false;
                break;
            case 11:
                PiQuizOpener.SetGameMode(PiQuizOpener.Mode.ScoreAttack);
                PiQuizOpener.EnableMultiplayer = true;
                break;
            default: throw new System.Exception("Error.");
        }

        if (Title.FileUpdater == null) /// DEBUG 用
        {
            PiQuizOpener.LoadJson.Clear();
            PiQuizOpener.LoadJson.Add(
                new PiQuizOpener.FilePath("S:\\Device\\Patch\\v02\\", "config.json"));
        }
        else
        {

            /*読み込むファイルは指定*/
            PiQuizOpener.LoadJson.Clear();
            foreach (var p in Title.FileUpdater.InstalledPatch)
            {
                PiQuizOpener.LoadJson.Add(
                    new PiQuizOpener.FilePath(Title.FileUpdater.PatchFolder + p.PatchName + "/", "config.json"));
            }

        }

        var dia_d = this.gameObject.AddComponent<SelectDiffDialog>();
        dia_d.Set((n) => {

            if (PiQuizOpener.EnableMultiplayer) // マルチプレイ用の処理
            {
                var dia = this.gameObject.AddComponent<PlayerNumberDialog>();
                dia.Set((x) =>
                {
                    PiQuizOpener.PlayerNames = x;
                    StartGame(n,id);
                });
                return;
            }
            else StartGame(n,id);
        });

    }

    /// <summary>
    /// ゲームの設定を元にゲームを開始します。
    /// </summary>
    /// <param name="diff">難易度(0:easy,2:hard,3:all)</param>
    private void StartGame(int diff,int modenum)
    {
        PiQuizOpener.SelectDifficulty = diff;
        PiQuizOpener.SelectModeNumber = modenum;
        PiQuizOpener.HighScoreThisMode = PlayerPrefs.GetInt("hscore_" + PiQuizOpener.SelectModeNumber + PiQuizOpener.SelectDifficulty,0);
        /*読み込むファイルは指定*/
        PiQuizOpener.LoadJson.Clear();
        foreach (var p in Title.FileUpdater.InstalledPatch)
        {
            if (p.Difficulty == diff) // 選択された難易度を抽出
                PiQuizOpener.LoadJson.Add(
                    new PiQuizOpener.FilePath(Title.FileUpdater.PatchFolder + p.PatchName + "/", "config.json"));
        }

        this.gameObject.AddComponent<MySceneManager>().Set("QuizMain",null);
    }


    /* */
    bool isBisible = false;
    public void VisibleSetting()
    {
        isBisible = !isBisible;
        GameObject.Find("Canvas_Settings").GetComponent<Canvas>().enabled = isBisible;
    }

    public void ToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    /* TITLE同様 */
    public void ShowAppInfo()
    {
        this.gameObject.AddComponent<DialogLong>().Set("情報", AppConfig.APPINFO, () => {
            PlayerPrefs.SetInt("readme", 1);
        }, null);
    }

    public void ShowConnect()
    {
        this.gameObject.AddComponent<MyAccessFrom>();
    }

    public void ShowWebSite()
    {
        Application.OpenURL(Title.APPSITE_URL);
    }
}
