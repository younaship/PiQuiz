using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningCountDown : MonoBehaviour {

    System.Action action;
    public void SetCallBack(System.Action callBack)
    {
        action = callBack;
    }

    GameObject canvas;

    // Use this for initialization
    void Start () {
        canvas = Instantiate(Resources.Load("Prefab/OpeningCanvas") as GameObject);
        canvas.GetComponent<Att_OpeningCanvas>().SetFinishCallBack(()=> {
            if (action != null) action();
            Destroy(this.gameObject.GetComponent<OpeningCountDown>());
        });
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
