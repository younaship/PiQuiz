using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNumberDialog : MonoBehaviour
{
    string[] PlayerNames = new string[] { "Player 1", "Player 2", "Player 3", "Player 4" };

    GameObject canvas;
    Button backBtn;
    Button[] selBtn = new Button[3];

    Action<string[]> action;
    public void Set(Action<string[]> selectCallBack)
    {
        action = selectCallBack;
    }

    public void Awake()
    {
        canvas = Instantiate(Resources.Load("Prefab/PlayerNumberCanvas") as GameObject);
        backBtn = canvas.transform.Find("Button").GetComponent<Button>();
        for (int i = 0; i < 3; i++) selBtn[i] = canvas.transform.Find("Image/Layout/Button_" + i).GetComponent<Button>();
    }

    public void Start()
    {
        backBtn.onClick.AddListener(Remove);
        for(int i = 0; i < 3; i++)
        {
            int x = i;
            selBtn[i].onClick.AddListener(() => {
                string[] ss = new string[x+2];
                for (int r = 0; r < x+2; r++) ss[r] = PlayerNames[r];
                action(ss);
            });
        }
    }

    private void Remove()
    {
        Destroy(canvas);
        Destroy(this.gameObject.GetComponent<PlayerNumberDialog>());
    }
}