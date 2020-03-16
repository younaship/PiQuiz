using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpDialog : MonoBehaviour
{
    const string PREFAB_PATH = "Prefab/PopUpCanvas";
    const string PREFAB2_PATH = "Prefab/ProgressImage";

    GameObject canvas,prog;
    Text text;

    // Start is called before the first frame update
    void Awake()
    {
        canvas = Instantiate(Resources.Load(PREFAB_PATH) as GameObject);
        prog = Instantiate(Resources.Load(PREFAB2_PATH) as GameObject, canvas.transform);
        text = canvas.transform.Find("Image/Text").GetComponent<Text>();
    }

    string str = "";
    public void Set(string message)
    {
        str = message;
    }
    public void SetAsync(string message)
    {
        text.text = message;
    }

    public void Remove(float dt = 0)
    {
        Destroy(this.gameObject.GetComponent<PopUpDialog>(), dt);
        Destroy(canvas, dt);
    }

    private void Start()
    {
        text.text = str;
    }
}
