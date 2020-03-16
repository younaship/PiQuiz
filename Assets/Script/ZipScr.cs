using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.IO;
using System.Linq;

public class ZipScr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FileScaner scan = new FileScaner();

        SyncManager sy = new SyncManager(@"S:\TEST\After");
        string[] ss;
        foreach (string s in sy.GetNoSyncList(@"S:\TEST\Before", out ss)) Debug.Log(s);
        foreach(string s in ss) Debug.Log("Over:" + s);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

/* JSON Style

public Folder GetFolders(string json)
{
    Folder folder;
    DataContractJsonSerializer data = new DataContractJsonSerializer(typeof(Folder));
    using (var stm = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
    {
        folder = (Folder)data.ReadObject(stm);
    }
    return folder;
}

public string GetJson(Folder folders)
{
    string s;
    using (var stm = new MemoryStream())
    using (StreamReader reader = new StreamReader(stm))
    {
        DataContractJsonSerializer data = new DataContractJsonSerializer(typeof(Folder));
        data.WriteObject(stm, folders);
        s = reader.ReadToEnd();
    }
    return s;
}


*/