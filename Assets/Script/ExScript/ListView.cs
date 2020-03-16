using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ListView : MonoBehaviour
{
    const string CANVAS_PATH = "Prefab/ItemSelecterCanvas";
    const string ITEM_PATH = "Prefab/ListItem";

    public Func<ListViewItem[],Task> callBack { private set; get; }
    bool isClose;

    GameObject canvas;
    ScrollRect scroll;
    ListViewItem[] items; // SET
    GameObject[] children;

    private void Awake()
    {
        canvas = Instantiate(Resources.Load(CANVAS_PATH) as GameObject);
        scroll = canvas.transform.Find("Scroll View").GetComponent<ScrollRect>();
        canvas.transform.Find("Panel").GetComponent<Button>().onClick.AddListener(() => Remove());
        canvas.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => PushDone());
    }

    public void SetCallBack(Func<ListViewItem[],Task> action)
    {
        this.callBack = action;
    }

    public void SetItems(ListViewItem[] items)
    {
        this.items = items;
    }

    // Start is called before the first frame update
    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (children != null) foreach (GameObject g in children) Destroy(g);
        if (items == null) return;
        string prof;
        if((prof = MyProfiles.GetProfiles(MyProfiles.StringProfile.SelectedPatches)) != null)
        {
            foreach (string w in prof.Split(','))
                foreach (var item in items)
                    if (item.name == w) item.isChk = true;
        }

        children = new GameObject[items.Length];
        int i = 0;
        foreach (ListViewItem item in items)
        {
            children[i] = Instantiate(Resources.Load(ITEM_PATH) as GameObject, scroll.content);
            children[i].transform.Find("Name").GetComponent<Text>().text = item.name;
            children[i].transform.Find("Summary").GetComponent<Text>().text = item.summary;

            if (!item.isDL) children[i].transform.Find("Dlicon").GetComponent<Image>().enabled = enabled;
            var chk = children[i].transform.Find("Toggle").GetComponent<Toggle>();
            if (item.isChk) chk.isOn = true;

            children[i].GetComponent<Button>().onClick.AddListener(() =>
            {
                chk.isOn = !chk.isOn;
            });
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PushDone()
    {
        if (isClose) return;
        else isClose = true;
        string str = "";
        for(int i = 0; i < children.Length; i++)
        {
            items[i].isChk = children[i].transform.Find("Toggle").GetComponent<Toggle>().isOn;
        }
        Close();
    }

    private async void Close()
    {
        await callBack?.Invoke(items);
        Remove();
    }

    public void Remove()
    {
        Destroy(canvas);
        Destroy(this.GetComponent<ListView>());
    }
}
public class ListViewItem
{
    public ListViewItem(string name, string summary, bool isDL)
    {
        this.name = name;
        this.isDL = isDL;
        this.summary = summary;
    }
    public string name;
    public string summary;
    public bool isDL;
    public bool isChk;
}