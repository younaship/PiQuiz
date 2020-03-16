using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class AnswerResultDialog : MonoBehaviour
{
    public enum Result { Success, Failure , FailureMulti ,TimeOutFailure }
    public Result result;

    public Action NextAction;

    public void Set(Result result)
    {
        this.result = result;
    }

    public void SetCallBack(Action act)
    {
        NextAction += act;
    }

    Texture Texture; // Answer Image
    public void SetTex(Texture texture)
    {
        Texture = texture;
    }

    string Message;
    public void SetMessage(string message)
    {
        Message = message;
    }

    public void OnPushOK()
    {
        Remove();
        if (NextAction != null) NextAction();
    }

    GameObject canvas;
    Transform imageF, imageS;
    Slider slider;
    Text text;

    private void Start()
    {
        canvas = Instantiate(Resources.Load("Prefab/AResultCanvas")as GameObject);
        imageS = canvas.transform.Find("ImageS");
        imageF = canvas.transform.Find("ImageF");
        slider = canvas.transform.Find("Slider").GetComponent<Slider>();
        text = canvas.transform.Find("Text").GetComponent<Text>();
        if (Message != null) text.text = Message;

        switch (result)
        {
            case Result.Success:
                Destroy(text.gameObject);
                Destroy(imageF.gameObject);
                StartCoroutine(SuccessUpdate());
                StartCoroutine(RunSlider(2));
                break;
            case Result.Failure:
                Destroy(text.gameObject);
                Destroy(imageS.gameObject);
                StartCoroutine(FailureUpdate());
                StartCoroutine(RunSlider(1));
                break;
            case Result.FailureMulti:
                Destroy(text.gameObject);
                Destroy(imageS.gameObject);
                Destroy(slider.gameObject);
                StartCoroutine(FailureMultiUpdate());
                break;
            case Result.TimeOutFailure:
                Destroy(imageS.gameObject);
                StartCoroutine(RunSlider(2));
                StartCoroutine(TimeOut());
                break;
        }
    }

    IEnumerator TimeOut()
    {
        var rect = imageF.GetComponent<RectTransform>();
        var img = imageF.GetComponent<Image>();
        var c = img.color;

        for (float t = Time.time; t > Time.time - 0.5;)
        {
            rect.localPosition += Vector3.up * 12;
            img.color = new Color(c.r, c.g, c.b, img.color.a - 0.03f);
            yield return null;
        }

        Destroy(rect.gameObject);
        yield return new WaitForSeconds(1.5f);
        OnPushOK();

    }

    IEnumerator FailureUpdate()
    {
        yield return new WaitForSeconds(1);
        Remove();
    }

    IEnumerator FailureMultiUpdate()
    {
        var rect = imageF.GetComponent<RectTransform>();
        var img = imageF.GetComponent<Image>();
        var c = img.color;

        for (float t = Time.time; t > Time.time - 0.5;)
        {
            rect.localPosition += Vector3.up * 12;
            img.color = new Color(c.r, c.g, c.b, img.color.a - 0.03f);
            yield return null;
        }
        Remove();
    }

    IEnumerator SuccessUpdate()
    {
        var rect = imageS.GetComponent<RectTransform>();
        var img = imageS.GetComponent<Image>();
        var c = img.color;

        for (float t = Time.time; t > Time.time - 0.5;)
        {
            rect.localPosition += Vector3.up * 12;
            img.color = new Color(c.r, c.g, c.b, img.color.a - 0.03f);
            yield return null;
        }
        Destroy(rect.gameObject);
        yield return new WaitForSeconds(1.5f);
        OnPushOK();
    }

    IEnumerator RunSlider(int time)
    {
        for(float t = Time.time; t > Time.time - time;)
        {
            slider.value = 1 - ((Time.time - t) / time);
            yield return null;
        }
    }

    private void Remove()
    {
        Debug.Log("Removed 'AnswerResDialog.'"+this.gameObject);
        Destroy(canvas);
        Destroy(this.gameObject.GetComponent<AnswerResultDialog>());
    }
}