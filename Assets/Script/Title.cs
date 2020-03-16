using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MyConverter;

public class Title : MonoBehaviour
{
    public const string APPSITE_URL = "https://younaship.com/piquiz/";
    public Slider slider;

    public static FileUpDater FileUpdater { get; private set; }

    SoundCenter se;

    // Start is called before the first frame update
    void Start()
    {
        int fi = PlayerPrefs.GetInt("readme", 0);
        if (fi == 0) 
        {
            this.gameObject.AddComponent<DialogLong>().Set("利用規約", AppConfig.README, () => {
                PlayerPrefs.SetInt("readme", 1);
                MyAdCenter.Start();
            }, null);
        }
        else MyAdCenter.Start();
        se = this.GetComponent<SoundCenter>();
    }

    public void ShowReadme()
    {
        this.gameObject.AddComponent<DialogLong>().Set("利用規約", AppConfig.README, () => {
            PlayerPrefs.SetInt("readme", 1);
        }, null);
    }

    public void ShowConnect()
    {
        this.gameObject.AddComponent<MyAccessFrom>();
    }

    public void ShowWebSite()
    {
        Application.OpenURL(APPSITE_URL);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool isRun = false;

    public void OnPushStart()
    {
        Sound();
        int fi = PlayerPrefs.GetInt("isFirst", 0);
        if (fi == 0) //　初回
        {
            this.gameObject.AddComponent<Dialog>().Set("ようこそ！", "クイズデーターの初回ダウンロードを行います。", () => {
                LoadStart();
            }, null);
        }
        else
        {
            LoadStart();
        }
    }

    public void Sound()
    {
        se.Sound();
    }

    public async void LoadStart()
    {
        if (isRun) return;
        else isRun = true;

        var pop = this.gameObject.AddComponent<PopUpDialog>();
        pop.Set("通信中...\nアップデートのチェック中");
        await Task.Delay(100);

        if (!await ChackUpdate(pop))
            if (!RunOnOffline())
            {
                Debug.LogWarning("Missing. Wake up.");
                return; // オフライン実行にも失敗
            }

        SceneManager.LoadSceneAsync("QuizTop");
    }

    IEnumerator enu;
    private async Task<bool> ChackUpdate(PopUpDialog dialog)
    {
        Debug.Log("Chack Update Start.");
        FileUpdater = new FileUpDater(Application.persistentDataPath+"\\");
        if (!await FileUpdater.ChackUpDateAvailable())
        {
            Debug.Log("Missing.");
            dialog.SetAsync("接続失敗\nアップデートは中断されました。");
            return false;
        }
        Debug.Log("Connection OK.");

        enu = RunProgress();
        StartCoroutine(enu);
        dialog.SetAsync("通信中...\nアップデートを取得しています");

        if (!await FileUpdater.ApplyUpdate()) Debug.Log("Missing.\n"+ FileUpdater.Log);
        else Debug.Log("Chack Update OK.\n"+ FileUpdater.Log);
        dialog.SetAsync("OK.\nアップデートが完了しました。");

        StopCoroutine(enu); // スライダー
        slider.value = slider.maxValue; 

        Debug.Log("<Installed Patches List> ["+ FileUpdater.InstalledPatch.Count + "]");
        if(FileUpdater.InstalledPatch.Count>0) foreach (var p in FileUpdater.InstalledPatch) Debug.Log("Package Name : "+p.PatchName);

        var json = FileUpDater.Serializer<FileUpDater.Patch[]>.GetJson(FileUpdater.InstalledPatch.ToArray());
        PlayerPrefs.SetString("patches", json);
        PlayerPrefs.SetInt("isFirst", 1);
        return true;
    }

    IEnumerator RunProgress()
    {
        while (true)
        {
            slider.maxValue = FileUpdater.TasksCount - 1;
            slider.value = FileUpdater.TasksNowRunning;
            yield return null;
        }
    }

    private bool RunOnOffline()
    {
        Debug.Log("Wake up On OffLine");
        var json = PlayerPrefs.GetString("patches", null);
        if (json == null) return false;
        else FileUpdater.SetPatch(FileUpDater.Serializer<FileUpDater.Patch[]>.GetT(json));
        return true;
    }
}
