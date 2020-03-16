using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultWindow : MonoBehaviour {

    const string RESOURCES_PATH = "Prefab/ResultCanvas";

    Action tryAction;
    GameObject canvas;
    Text text;
    Button buttonTry, buttonExit;

    string message;
    int[] multiScore;
    string[] userNames;
    string title = "";

    public void SetAction(Action tryButtonAction)
    {
        tryAction = tryButtonAction;
    }

    public void SetTitle(string title)
    {
        this.title = title;
    }

    public void SetMessage(string message)
    {
        this.message = message;
    }

    public void SetResult(int[] multiScore,string[] userNames)
    {
        this.multiScore = multiScore;
        this.userNames = userNames;
    }

    // Use this for initialization
    void Start () {
        canvas = Instantiate(Resources.Load(RESOURCES_PATH) as GameObject);
        text = canvas.transform.Find("MessageText").GetComponent<Text>();
        buttonExit = canvas.transform.Find("Button_Exit").GetComponent<Button>();
        buttonTry = canvas.transform.Find("Button_Try").GetComponent<Button>();
        canvas.transform.Find("InfoText").GetComponent<Text>().text = title;

        if (multiScore != null)
        {
            string s = "Score\n";
            for (int i = 0; i < userNames.Length; i++) s += "[" + userNames[i] + "] " + multiScore[i] + "\n";
            text.text = s;
        }
        else text.text = message;

        buttonExit.onClick.AddListener(Remove);
        buttonTry.onClick.AddListener(()=>tryAction());
	}
	
    void Remove()
    {
        Destroy(canvas);
        Destroy(this.gameObject.GetComponent<ResultWindow>());
    }

	// Update is called once per frame
	void Update () {
		
	}
}
