using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Deubg : MonoBehaviour
{

    FileUpDater.PatchRoot ROOT = new FileUpDater.PatchRoot()
    {
        Root = new FileUpDater.Patch[] { new FileUpDater.Patch { PatchName = "NAME" } }
    };

    // Start is called before the first frame update
    void Start()
    {
        Run();
    }

    const string DEVICE_PATH = "S:\\Device\\";

    async void Run()
    {
        /// string s = await MyNetworks.GetResponse("http://nu-arai.sakura.ne.jp/hello.html");

        FileUpDater up = new FileUpDater(DEVICE_PATH);
        Debug.Log(JsonUtility.ToJson(ROOT));

        Debug.Log(await up.ChackUpDateAvailable());
        Debug.Log(up.LogGet());

        Debug.Log(await up.ApplyUpdate());
        Debug.Log(up.Log);
    }


    void Update()
    {
        
    }
}
