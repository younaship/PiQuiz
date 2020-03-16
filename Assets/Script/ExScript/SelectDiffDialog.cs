using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SelectDiffDialog : MonoBehaviour
{ 

    GameObject canvas;
    Button backBtn;
    Button[] selBtn = new Button[4];

    Action<int> action;
    public void Set(Action<int> selectCallBack)
    {
        action = selectCallBack;
    }

    public void Awake()
    {
        canvas = Instantiate(Resources.Load("Prefab/SelectDiffCanvas") as GameObject);
        backBtn = canvas.transform.Find("Button").GetComponent<Button>();
        for (int i = 0; i < 3; i++) selBtn[i] = canvas.transform.Find("Image/Layout/Button_" + i).GetComponent<Button>();
        selBtn[3] = canvas.transform.Find("Image/Button_3").GetComponent<Button>();
    }

    public void Start()
    {
        backBtn.onClick.AddListener(Remove);
        for (int i = 0; i < 4; i++)
        {
            int x = i;
            selBtn[i].onClick.AddListener(() => {
                action(x);
            });
        }
    }

    private void Remove()
    {
        Destroy(canvas);
        Destroy(this.gameObject.GetComponent<SelectDiffDialog>());
    }
}
