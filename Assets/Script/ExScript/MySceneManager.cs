using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour {

    public static string BeforeSceneName { get; private set; }
    const string PREFAB_PATH = "SceneManager\\SceneManagerCanvas";
    string nextScene;
    System.Action action;

    public void Set(string nextSceneName,System.Action finCallBack)
    {
        nextScene = nextSceneName;
        action = finCallBack;
    }

    public bool Back()
    {
        if (BeforeSceneName == null) return false;
        nextScene = BeforeSceneName;
        StartCoroutine(Run());
        return true;
    }

    // Animator anim;
    // Use this for initialization

    GameObject canvas;
	void Start () {
        canvas = Instantiate(Resources.Load(PREFAB_PATH) as GameObject);
       // anim = g.transform.Find("Image").GetComponent<Animator>();
        StartCoroutine(Run());
	}

    IEnumerator Run()
    {
        var befScene = SceneManager.GetActiveScene();
        BeforeSceneName = SceneManager.GetActiveScene().name;

        yield return new WaitForSeconds(1);
        var async = SceneManager.LoadSceneAsync(nextScene,LoadSceneMode.Additive);
        async.completed += (x)=> {
            Debug.Log("LOADED.CallBack.");
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextScene));
        };

        while (true)
        {
            if (async.isDone == true)
            {
                SceneManager.UnloadSceneAsync(befScene);
                Debug.Log("LOADED and Closed > " + befScene.name +" to "+nextScene);
                break;
            }
            Debug.Log("Loading.");
            yield return null;
        }

        GameObject g;
        Instantiate(g = Instantiate(Resources.Load(PREFAB_PATH+"2") as GameObject));

        yield return new WaitForSeconds(0.5f);
        
        if(action!=null) action();
    }
	
}
