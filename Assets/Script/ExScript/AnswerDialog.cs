using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerDialog : MonoBehaviour {

    GameObject canvas;
    InputField input;
    Text text;
    Action<string,int> AnswerCallback;

    int PushUser;
    string str = "";

    /// <summary>
    /// string : Answer String  int : Answer User
    /// </summary>
    /// <param name="answerCallback"></param>
    public void Set(Action<string,int> answerCallback , int pushUser = -1)
    {
        AnswerCallback = answerCallback;
        PushUser = pushUser;
    }

    public void SetText(string str)
    {
        this.str = str;
    }

    private void Awake()
    {
        canvas = Instantiate(Resources.Load("Prefab/AnswerCanvas")as GameObject);
        input = canvas.transform.Find("InputField").GetComponent<InputField>();
        text = canvas.transform.Find("Text").GetComponent<Text>();
        Button button = canvas.transform.Find("Button").GetComponent<Button>();
        button.onClick.AddListener(() => PushOK());
    }

    private void Start()
    {
        text.text = this.str;
        input.ActivateInputField();
        TouchScreenKeyboard.Open(input.text, TouchScreenKeyboardType.Default);
    }

    public void PushOK()
    {
        string ans = input.text;
        ans = MyConverter.KanaConverter.ToHiragana(ans);

        if (AnswerCallback!=null) AnswerCallback(ans,PushUser);
        Destroy(canvas);
        Destroy(this.gameObject.GetComponent<AnswerDialog>());
    }
}
