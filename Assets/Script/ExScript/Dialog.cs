using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    const string PREFAB_PATH = "Prefab/DialogCanvas";

    public Action actOK { private set; get; }
    public Action actCan { private set; get; }

    string titleStr, messageStr;

    GameObject canvas;
    Text title, text;
    Button btn_can, btn_ok;

    private void Awake()
    {
        canvas = Instantiate(Resources.Load(PREFAB_PATH) as GameObject);
        title = canvas.transform.Find("TextTitle").GetComponent<Text>();
        text = canvas.transform.Find("TextMessage").GetComponent<Text>();
        btn_can = canvas.transform.Find("Button_C").GetComponent<Button>();
        btn_ok = canvas.transform.Find("Button_OK").GetComponent<Button>();
    }

    public Dialog Set(string title,string message,Action actOK,Action actCan)
    {
        this.actOK = actOK;
        this.actCan = actCan;
        this.titleStr = title;
        this.messageStr = message;
        return this;
    }

    // Start is called before the first frame update
    void Start()
    {
        btn_ok.onClick.AddListener(() => this.actOK?.Invoke());
        btn_can.onClick.AddListener(() => this.actCan?.Invoke());
        title.text = titleStr;
        text.text = messageStr;

        actOK += Remove;
        if (actCan == null) Destroy(btn_can.gameObject);
        else actCan += Remove;
    }

    /// <summary>
    /// 削除します。しかし通常選択されれば削除されます。
    /// </summary>
    public void Remove()
    {
        Destroy(canvas);
        Destroy(this.gameObject.GetComponent<Dialog>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
