using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Att_OpeningCanvas : MonoBehaviour {

    public AudioClip sound;
    const int START_TIME = 3;

    Text text;
    Image image;
    float startTime;

    Action callBack;
    AudioSource audio;

	// Use this for initialization
	void Awake () {
        audio = this.gameObject.AddComponent<AudioSource>();
        audio.clip = sound;

        startTime = Time.time;
	}

    private void Start()
    {
        image = this.transform.Find("Image").GetComponent<Image>();
        text = this.transform.Find("OnText").GetComponent<Text>();
    }

    public void SetFinishCallBack(Action callBack)
    {
        this.callBack += callBack;
    }

    // Update is called once per frame
    bool isSound = false;
    void Update () {
        if (GetRem() <= 2.9f && !isSound)
        {
            audio.Play();
            isSound = true;
        }
        if (GetRem() < 0)
        {
            if (callBack != null) callBack.Invoke();
            Destroy(this.gameObject);
        }
        image.fillAmount = GetRem() / START_TIME;
        text.text = ""+((int)GetRem()+1);
	}

    float GetRem()
    {
        return START_TIME - (Time.time - startTime);
    }

}
