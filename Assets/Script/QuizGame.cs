using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PiQuizOpener
{
    public enum Mode {
        TimeAttack ,
        ScoreAttack
    } // TIME : 時間制(10s) スコア(10問)

    public static bool EnableMultiplayer = false;
    public static string[] PlayerNames;
    public static int QuestionSize = 5; // スコアアタックの問題数

    public static void SetGameMode(Mode mode) { GameMode = mode; }
    public static Mode GameMode { private set; get; }
    public static int ReturnResult = -1;
    public static int[] ReturnResultMulti = new int[4];
    public static int SelectDifficulty = -1;
    public static int SelectModeNumber = 0;
    public static int HighScoreThisMode = 0;
    public static List<FilePath> LoadJson = new List<FilePath>();

    public static int PlayerCount { get { return PlayerNames.Length; } }

    public class FilePath
    {
        public FilePath(string path,string name) {
            Path = path;
            Name = name;
        }
        public string Path;
        public string Name;
    }

}

public class QuizGame : MonoBehaviour {

    const int TIMEATTACK_TIME = 10;
    const int SCOREATTACK_QUESTION_SKIP_TIME = 8;

    public Canvas multiCanvas;
    public RawImage rawImage;
    public Slider slider;
    public Button answerButton;
    public Text scoreText, timebarText, HiSocre, modeText;

    public Texture DENUG_TEX1;

    public bool IsMulti { private set; get; }
    public static string[] PlayerNameOnMulti { private set; get; }

    PiQuiz piQuiz;
    SoundCenter sound;
    int quizCount = 0;
    int score = 0;
    int[] scoreMulti = new int[4];

    private void LoadGame() // <== ?
    {
        PiQuizOpener.ReturnResult = -1;
        IsMulti = PiQuizOpener.EnableMultiplayer;
        PlayerNameOnMulti = PiQuizOpener.PlayerNames;

        piQuiz = new PiQuiz();

        Debug.Log("Load Json Count " + PiQuizOpener.LoadJson.Count);
        foreach (var path in PiQuizOpener.LoadJson) piQuiz.LoadFromJson(path.Path, path.Name);

        if (IsMulti)
        {
            int playerNum = PlayerNameOnMulti.Length;
            Button[] multiAnsBtn = new Button[4];
            for (int i = 0; i < 4; i++)
            {
                multiAnsBtn[i] = multiCanvas.transform.Find("Button_" + i + "").GetComponent<Button>();
                int x = i;
                multiAnsBtn[i].onClick.AddListener(() => OnPushAnswer(x));
            }
            for (int i = playerNum; i < 4; i++) Destroy(multiAnsBtn[i].gameObject);

            Destroy(answerButton.gameObject);
        }
        else
        {
            Destroy(multiCanvas.gameObject);
        }
        
    }

    private void Awake()
    {
        sound = this.gameObject.GetComponent<SoundCenter>();
        modeText.text = "PiQuiz "+PiQuizTop.GetGameModeName(PiQuizOpener.SelectModeNumber);
    }

    void Start () {
        Debug.Log("Game Start.");

        LoadGame();
        this.gameObject.AddComponent<OpeningCountDown>().SetCallBack(StartGame);
        if (PiQuizOpener.HighScoreThisMode > 0)
            HiSocre.text = "High Socre : "+ PiQuizOpener.HighScoreThisMode;
        /*
         DEBUG
         */

        //PiQuizOpener.SetGameMode(PiQuizOpener.Mode.ScoreAttack);
        //piQuiz.LoadFromJson(@"D:\Dropbox\apk\v02\", "config.json");
        //IsMulti = true;
    }

    public void StartGame() //ゲームを開始します。
    {
        StartQuestion();
        answerButton.onClick.AddListener(() => OnPushAnswer());
    }

    int tryNullCount = 0;　//試行失敗回数
    PiQuiz.Items selectedItem; // 現在の問題
    long startQuestionTime;
    private void StartQuestion() // 問題を出力します。
    {
        quizCount++;
        if (selectedItem==null) selectedItem = piQuiz.GetItem();

        if (PiQuizOpener.GameMode == PiQuizOpener.Mode.ScoreAttack)
        {
            if (quizCount > PiQuizOpener.QuestionSize)
            {
                FinishGame();
                return;
            }
        }

        if(!FileScaner.ChkFile(selectedItem.ImagePath))
            if (tryNullCount < 10)
            {
                Debug.LogWarning("Error. No File." + selectedItem.ImagePath);
                selectedItem = piQuiz.GetItem();
                tryNullCount++;
            }

        rawImage.color = new Color(-0.2f, -0.2f, -0.2f);
        rawImage.texture = FileUniTexture.PNGToUniTex(selectedItem.ImagePath);
        startQuestionTime = piQuiz.stopWatch.ElapsedMilliseconds;
        piQuiz.stopWatch.Start();

        acceptAnswer = true;
        if (IsMulti)
        {
            if (acceptAnswerPlayer == null) acceptAnswerPlayer = new bool[PlayerNameOnMulti.Length];
            for (int i = 0; i < acceptAnswerPlayer.Length; i++) acceptAnswerPlayer[i] = true;
        }

        updateAction += GameRunning;

        if (IsMulti)
            if (PlayerNameOnMulti.Length >= 3) SetRundomRotate(rawImage.rectTransform);

    }

    private void SetRundomRotate(RectTransform rect) // (多数プレイヤー時)画像の不規則回転
    {
        int x = (int)UnityEngine.Random.Range(0, 4);
        rect.Rotate(Vector3.forward, 90 * x);
        Debug.Log("GET Random : "+x);
    }

    bool acceptAnswer;
    bool[] acceptAnswerPlayer;
    private void OnPushAnswer(int pushUser = 0) // 回答権を取得 (MultiPlay = 押した"User")
    {     
        if (!acceptAnswer) return;
        else if (!IsMulti) acceptAnswer = false;

        Debug.Log("Press Answer. " + pushUser);
        if (IsMulti)
            if (!acceptAnswerPlayer[pushUser]) return;
            else acceptAnswerPlayer[pushUser] = false;
        else Debug.Log("Single");

        piQuiz.stopWatch.Stop();
        updateAction -= GameRunning;

        
        AnswerDialog ans = this.gameObject.AddComponent<AnswerDialog>();
        ans.Set((x, y) => ChackAnswer(x, pushUser));
        if (IsMulti)
            ans.SetText(PlayerNameOnMulti[pushUser] + " が回答中...");
    }

    private void ChackAnswer(string str,int pushUser) // 回答を確認
    {
        var ans = this.gameObject.AddComponent<AnswerResultDialog>();
        if (selectedItem.ChackAnswer(str)) // Hit
        {
            sound.Sound(0);

            ans.Set(AnswerResultDialog.Result.Success);
            ans.SetTex(rawImage.texture);
            ans.NextAction = () => StartQuestion();
            rawImage.color = Color.white;
            if (IsMulti) GetScore(pushUser);
            else GetScore(); 

            selectedItem = piQuiz.GetItem(); // 正解後次の問題に備える
        }
        else
        {
            sound.Sound(1);

            if (IsMulti) ans.Set(AnswerResultDialog.Result.FailureMulti);
            else ans.Set(AnswerResultDialog.Result.Failure);

            piQuiz.stopWatch.Start();
            updateAction += GameRunning;
            acceptAnswer = true;

            if (IsMulti)
                if (acceptAnswerPlayer.All((x) => x == false)) QuestionTimeOut();
        }
    }

    void GetScore(int getScoreUser = 0) //正解した後
    {
        long dt = piQuiz.stopWatch.ElapsedMilliseconds - startQuestionTime; //経過時間
        int gScore = 0;

        switch (PiQuizOpener.GameMode)
        {
            case PiQuizOpener.Mode.ScoreAttack:
                gScore = (int)(110 - ( dt / 100));
                if (IsMulti) scoreMulti[getScoreUser] += gScore;
                else score += gScore;

                break;
            case PiQuizOpener.Mode.TimeAttack:
                gScore = 1;
                score += gScore;
                break;
        }
        Debug.Log("GET Score [" + gScore + "] To > " + getScoreUser +"\ndt"+dt);
    }

    void QuestionTimeOut() // 問題スキップ
    {
        rawImage.color = Color.white;
        Debug.Log("Question_TimeOut");
        updateAction -= GameRunning;
        acceptAnswer = false;

        var ans = this.gameObject.AddComponent<AnswerResultDialog>();
        ans.Set(AnswerResultDialog.Result.TimeOutFailure);
        ans.SetMessage("答え : "+selectedItem.ItemName[0]+" など...");
        ans.NextAction += () => StartQuestion();

        selectedItem = piQuiz.GetItem();
    }

    void FinishGame()
    {
        updateAction -= GameRunning;
        acceptAnswer = false;
        Instantiate(Resources.Load("FInishAnimation/Finish_anim") as GameObject);
        StartCoroutine(FinishThread());
    }
    // Other Thread
    IEnumerator FinishThread()
    {
        yield return new WaitForSeconds(1);
        PiQuizOpener.ReturnResult = score;
        if(IsMulti) PiQuizOpener.ReturnResultMulti = scoreMulti; 

        this.gameObject.AddComponent<MySceneManager>().Set("QuizTop",null);
    }

    // Update Thread
    void GameRunning()
    {
        float dx = 0.005f + 0.005f * MyEncodeForUni.GetDelta(Time.deltaTime);
        Color c = rawImage.color;
        rawImage.color = new Color(c.r + dx, c.g + dx, c.b + dx);

        ShowStat();
        switch (PiQuizOpener.GameMode) // ゲームモードごとに行う動作
        {
            case PiQuizOpener.Mode.TimeAttack:
                if (piQuiz.stopWatch.ElapsedMilliseconds > TIMEATTACK_TIME * 1000) FinishGame();
                break;
            case PiQuizOpener.Mode.ScoreAttack:
                if (piQuiz.stopWatch.ElapsedMilliseconds - startQuestionTime > SCOREATTACK_QUESTION_SKIP_TIME * 1000) QuestionTimeOut();
                break;
        }
    }

    float GetStatTimeBar(ref Text viewText) // タイムバーの値を取得します。(return 0-1) (ref:表示用テキスト)
    {
        long t = piQuiz.stopWatch.ElapsedMilliseconds;

        switch (PiQuizOpener.GameMode)
        {
            case PiQuizOpener.Mode.TimeAttack:
                viewText.text = "残り："+(int)((TIMEATTACK_TIME * 1000f - t)/100);
                return (TIMEATTACK_TIME * 1000f - t) / (TIMEATTACK_TIME * 1000f);

            case PiQuizOpener.Mode.ScoreAttack:
                long dt = t - startQuestionTime;
                viewText.text = "スキップまで\n"+ (int)((SCOREATTACK_QUESTION_SKIP_TIME * 1000f - dt) / 100);
                return (SCOREATTACK_QUESTION_SKIP_TIME * 1000f - dt) / (SCOREATTACK_QUESTION_SKIP_TIME * 1000f);
        }
        return 0.1f;
    }

    void ShowStat()
    {
        switch (PiQuizOpener.GameMode)
        {
            case PiQuizOpener.Mode.ScoreAttack:
                scoreText.text = "残り " + (PiQuizOpener.QuestionSize - quizCount + 1)+"問";
                break;
            case PiQuizOpener.Mode.TimeAttack:
                scoreText.text = "Score\n" + score;
                break;
        }
        slider.value = GetStatTimeBar(ref timebarText);
    }

    // Update is called once per frame
    Action updateAction;
	void Update () {
        if (updateAction != null) updateAction.Invoke();
	}

    public void OnPushExit()
    {
        this.gameObject.AddComponent<MySceneManager>().Set("QuizTop", null);
        //man.Back();
    }

}

public class UpdateThreadObj : MonoBehaviour
{
    Action action;
    public bool Enable = true;

    public void Set(Action action) { this.action = action; }

    public void Update()
    {
        if (action != null && Enable) action.Invoke();
    }
}

public class TextureController : FileUniTexture
{
    public enum From
    {
        Assets, // Load From "/Resoureces"
        Path // Load From Path
    }

    public static Texture GetTexture(From from,string path)
    {
        switch (from)
        {
            case From.Assets:
                return Resources.Load(path) as Texture;
            case From.Path:
                return FileUniTexture.PNGToUniTex(path);
        }
        return null;
    }
}