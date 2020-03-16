using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using System.Runtime.Serialization.Json;

public class FileUpDater
{
    public const string PATCH_DIRECTRY = "Patch\\";
    const string TMP_DIRECTRY = "Tmp\\";
    const string PATCH_LIST_REQUEST_URL = "https://api.younaship.com/PiQuiz/update.json";

    public bool IsOffLine { private set; get; }
    public string LocalPath { get; private set; }
    public string PatchFolder { get { return LocalPath + PATCH_DIRECTRY; } }
    public string TempFolder { get { return LocalPath + TMP_DIRECTRY; } }
    public string Log { get; private set; }

    public int TasksCount { private set; get; }
    public int TasksNowRunning { private set; get; }

    public List<Patch> InstalledPatch { get; private set; }

    public Patch[] PatchOnServer { private set; get; }
    

    public FileUpDater(string localPath)
    {
        InstalledPatch = new List<Patch>();
        LocalPath = localPath;
        Log += "localPath:" + localPath + "\n";
    }

    private void LogAdd(string log)
    {
        Log += log + "\n";
    }

    /// <summary>
    /// アップデートがあるかを確認、及び情報を保存します。
    /// </summary>
    /// <returns>True:情報を取得、保存済みです。 False:宛先が存在しないかエラーです。</returns>
    public async Task<bool> ChackUpDateAvailable()
    {
        PatchOnServer = await GetPatchOnServer();
        if (PatchOnServer != null) return true;
        IsOffLine = true;
        return false;
    }

    public string LogGet()
    {
        string s = "Loaded Patches from Server.\n";
        foreach (Patch p in PatchOnServer) s += "---\ni[" + p.Importance + "] NAME:" + p.PatchName + "\nURL:" + p.Url + "\n---";
        return s;
    }

    /// <summary>
    /// パッチ名:string[]のパッチを取得します。
    /// </summary>
    public Patch[] GetPatch(string[] patchNames)
    {
        List<Patch> list = new List<Patch>();
        foreach (string name in patchNames)
            foreach (Patch p in InstalledPatch)
                if (p.PatchName == name) { list.Add(p); break; }
        return list.ToArray();
    }

    public void SetPatch(Patch[] patchs)
    {
        InstalledPatch = new List<Patch>(patchs);
    }

    /// <summary>
    /// アップデートを適用(重要度 3>)します。
    /// </summary>
    public async Task<bool> ApplyUpdate()
    {
        TasksCount = PatchOnServer.Length;
        TasksNowRunning = 0;

        bool isOk = true;
        foreach (Patch p in PatchOnServer)
        {
            TasksNowRunning++;

            Log += "Chacking Update [" + p.PatchName + "]\n";
            if (p.Importance >= 3) // 重要度3以上の為インストールします。
            {
                if (!await InstallPatch(p))
                {
                    Log += "Install Missing.\n";
                    isOk = false;
                }
                else
                {
                    Log += "Success.\n";
                    p.Installed = true;
                }
            }
            else if (p.Importance <= -3) // 重要度-3以下の為アンインストールします。
            {
                if (!UnInstallPatch(p))
                {
                    Log += "UnInstall Missing.\n";
                    isOk = false;
                }
                else Log += "Success.\n";
            }
            else // ダウンロード不要だがインストール済みか確認
            {
                if(ChackInstalledPatch(p)) p.Installed = true;
            }
        }
        return isOk; 
    }

    /// <summary>
    /// 指定したパッチは既にインストール済みかを確認します。
    /// </summary>
    private bool ChackInstalledPatch(Patch target)
    {
        if (!FileScaner.ChkFile(TempFolder + target.PatchName)) return false;
        int installedFileCount = FileScaner.GetCountInFolder(PatchFolder + target.PatchName);
        LogAdd("Skip unpack. Installed [" + installedFileCount + "] Files.");
        if (target.FileCount != 0 && target.FileCount != installedFileCount) return false; // インストール済みの数が異なる
        return true;
    }

    /// <summary>
    /// 指定したパッチ(名)をアンインストールします。
    /// </summary>
    /// <returns>True:成功 False:失敗。ファイルが存在しない場合があります。</returns>
    public bool UnInstallPatch(Patch target)
    {
        try
        {
            LogAdd("Uninstall Package [" + target.PatchName + "]");
            FileScaner.RemoveFolder(PatchFolder + target.PatchName);
            FileScaner.RemoveFile(TempFolder + target.PatchName);
        }
        catch { return false; }
        return true;
    }

    /// <summary>
    /// パッチをインストールします。名前で指定するオーバーロードです。
    /// </summary>
    public async Task<bool> InstallPatch(string patchName)
    {
        foreach(Patch p in PatchOnServer)
            if (p.PatchName == patchName) return await InstallPatch(p);
        return false;     
    }

    /// <summary>
    /// パッチをインストールします。インストール済みの場合スキップされ True が返されます。
    /// </summary>
    public async Task<bool> InstallPatch(Patch target)
    {
        int tryCount = 0;
        TryAgain:

        if (!await TryDownLoad(target.Url, target.PatchName))
            if (!await TryDownLoad(target.SubUrl, target.PatchName)) return false;

        LogAdd("Try UnZip [" + target.PatchName + "].");

        if (FileScaner.MakeDir(PatchFolder + target.PatchName))
        {
            try
            {
                ZipFile.ExtractToDirectory(TempFolder + target.PatchName, PatchFolder + target.PatchName, Encoding.UTF8);
                LogAdd("UnZip OK.");
            }
            catch(Exception e)
            {
                Debug.LogWarning("Error Exception : "+e);
                return false;
            }
        }
        else
        {
            int installedFileCount = FileScaner.GetCountInFolder(PatchFolder + target.PatchName);
            LogAdd("Skip unpack. Installed [" + installedFileCount + "] Files.");
            if (target.FileCount != 0 && target.FileCount != installedFileCount) // インストール済みの数が異なる
            {
                LogAdd("Different File Error. Correct ["+target.FileCount+"] Files. Try Again.");
                UnInstallPatch(target);

                if (tryCount == 0)
                {
                    tryCount++;
                    goto TryAgain;
                }
                else return false;
            }
        }

        InstalledPatch.Add(target);
        return true;
    }


    private async Task<bool> TryDownLoad(string url, string patchName)
    {
        WebClient wc = new WebClient();
        FileScaner.MakeDir(TempFolder);
        if (url == null || patchName == null) return false;

        LogAdd("Try DownLoad [" + url + " ].");
        if (FileScaner.ChkFile(TempFolder + patchName)) LogAdd("Archive Found. Skip Download.");
        else
        {
            try
            {
                await wc.DownloadFileTaskAsync(new Uri(url), TempFolder + patchName);
                LogAdd("Download OK.");
            }
            catch (WebException we)
            {
                Debug.LogWarning("Error WebException : " + we.Status);
                FileScaner.RemoveFile(TempFolder + patchName);
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// サーバーにあるパッチリストを取得します
    /// </summary>
    private async Task<Patch[]> GetPatchOnServer()
    {
        string url = PATCH_LIST_REQUEST_URL;
        string resJson = await MyNetworks.GetResponse(url);
        Debug.Log("Received Json.\n" + resJson);

        try
        {
            if (resJson != null) return JsonUtility.FromJson<PatchRoot>(resJson).Root;
            else return null;
        }
        catch { return null; }
    }

    /// <summary>
    /// インストール済みパッケージリストを取得します。
    /// </summary>
    public string[] GetInstalledPatches()
    {
        List<string> list = new List<string>();
        foreach (string s in FileScaner.GetFolders(LocalPath + PATCH_DIRECTRY)) list.Add(s.Replace(LocalPath + PATCH_DIRECTRY, "").Trim());
        return list.ToArray();
    }

    /// <summary>
    /// インストール済みパッケージリストを取得します。
    /// </summary>
    private Patch[] GetInstalledPatches_()
    {
        string savedJson = PlayerPrefs.GetString("Installed", ""); // 保存済みstring(インストール済みリストJson)取得
        Patch[] patches = JsonUtility.FromJson<Patch[]>(savedJson);
        return patches;
    }

    /// <summary>
    /// 比較し、インストールのされてないパッチを取得します。
    /// </summary>
    private Patch[] GetAbsPatches(Patch[] server,Patch[] local) 
    {
        Patch tmp;
        List<Patch> list = new List<Patch>(server);
        foreach (Patch p in local)
            if ((tmp = local.First((x) => x.PatchName == p.PatchName)) != null)
                list.Remove(tmp);
        return list.ToArray();
    }

    [Serializable]
    public class PatchRoot
    {
        public Patch[] Root;
    }

    [Serializable]
    public class Patch
    {
        public int Importance = 0; // 重要度 (>3 必須)(<3 削除必須)
        public string PatchName;
        public string Summary; // 概要
        public string Url; // Grobal = URI | local = PATH
        public string SubUrl; // If Request to Url Missing. To Get From this URLs.
        public bool Installed;
        public int FileCount;
        public int Difficulty = 1; // 難易度

    }

    public class Serializer<T>
    {
        public static T GetT(string json)
        {
            T t;
            try
            {
                DataContractJsonSerializer dc = new DataContractJsonSerializer(typeof(T));
                using (MemoryStream stm = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    t = (T)dc.ReadObject(stm);
                }
            }
            catch { return default(T); }
            return t;
        }

        public static string GetJson(T t)
        {
            string json;
            DataContractJsonSerializer dc = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream stm = new MemoryStream())
            {
                dc.WriteObject(stm, t);
                stm.Position = 0;
                using (StreamReader read = new StreamReader(stm)) json = read.ReadToEnd(); ;
            }
            return json;
        }
    }

    public class IO<T>
    {
        /// <summary>
        /// T を Json としてファイルを書き込みます。
        /// </summary>
        public static bool Save(string path, T data)
        {
            Debug.Log("Fs > " + path);
            try
            {
                FileStream f = new FileStream(path, FileMode.Create);
                DataContractJsonSerializer dc = new DataContractJsonSerializer(typeof(T));
                dc.WriteObject(f, data);
                f.Close();
                return true;
            }
            catch { return false; }
        }

        public static async Task<bool> SaveAsync(string path, T data)
        {
            Debug.Log("Fs > " + path);
            try
            {
                FileStream f = new FileStream(path, FileMode.Create);
                MemoryStream m = new MemoryStream();

                DataContractJsonSerializer dc = new DataContractJsonSerializer(typeof(T));
                dc.WriteObject(m, data);
                m.Position = 0;

                string json;
                using (StreamReader str = new StreamReader(m))
                    json = await str.ReadToEndAsync();
                using (StreamWriter stm = new StreamWriter(f))
                {
                    byte[] b = Encoding.UTF8.GetBytes(json);
                    await f.WriteAsync(b, 0, b.Length);
                }

                f.Close();
                return true;
            }
            catch (Exception e) { return false; }
        }

        /// <summary>
        /// Json を T としてファイルを読み込みます。
        /// </summary>
        public static T Load(string path)
        {
            Debug.Log("Fs > " + path);
            try
            {
                FileStream f = new FileStream(path, FileMode.Open);
                DataContractJsonSerializer dc = new DataContractJsonSerializer(typeof(T));
                T t = (T)dc.ReadObject(f);
                f.Close();
                return t;
            }
            catch
            {
                return default(T);
            }
        }

        public static async Task<T> LoadAsync(string path)
        {
            Debug.Log("Fs > " + path);
            try
            {
                string s;
                using (FileStream f = new FileStream(path, FileMode.Open))
                using (StreamReader read = new StreamReader(f))
                    s = await read.ReadToEndAsync();

                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(s));
                DataContractJsonSerializer dc = new DataContractJsonSerializer(typeof(T));
                T t = (T)dc.ReadObject(ms);
                ms.Close();
                return t;
            }
            catch
            {
                return default(T);
            }
        }
    }

}