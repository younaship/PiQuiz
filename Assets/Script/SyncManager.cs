using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class SyncManager : FileScaner
{
    /// <param name="syncPath">同期先のファイルパスを指定します。(例 C:\Test)</param>
    public SyncManager(string syncPath) { SyncPath = syncPath; }

    public string SyncPath { private set; get; }

    /// <summary>同期されてないファイルリストを取得します。</summary>
    /// <param name="sourcePath">比較元のフォルダーパス</param>
    /// <param name="excessFiles">比較先に存在する余分なファイルリストを出力します。(!)元に無いフォルダ未探索</param>
    public string[] GetNoSyncList(string sourcePath, out string[] excessFiles)
    {
        List<string> toList = new List<string>(GetFilesRelPath(SyncPath));
        List<string> fromList = new List<string>(GetFilesRelPath(sourcePath));
        List<string> overList = new List<string>();

        foreach (string s in toList) if (fromList.Contains(s)) fromList.Remove(s);
            else overList.Add(s);
        excessFiles = overList.ToArray();

        return fromList.ToArray();
    }

}

public class FileScaner
{
    public static void RemoveFile(string path)
    {
        if (ChkFile(path)) System.IO.File.Delete(path);
    }

    public static void RemoveFolder(string path)
    {
        if (System.IO.Directory.Exists(path)) Directory.Delete(path, true);
    }

    ///<summary> 指定フォルダー内のファイルをすべて検索し相対パスの配列で受け取ります。</summary>
    public static string[] GetFilesRelPath(string path)
    {
        List<string> list = new List<string>();
        foreach (File f in GetFiles(path)) list.Add(f.RelPath);
        return list.ToArray();
    }

    ///<summary> 指定フォルダー内のファイル数を取得します。</summary>
    public static int GetCountInFolder(string path)
    {
        return GetFiles(path).Length;
    }

    ///<summary> 指定フォルダー内のファイルをすべて検索します。</summary>
    public static File[] GetFiles(string path)
    {
        List<File> files = new List<File>();
        ScanFolder(ref files, path);
        return files.ToArray();
    }

    private static void ScanFolder(ref List<File> files, string folderPath)
    {
        DirectoryInfo info = new DirectoryInfo(folderPath);
        FileInfo[] fInfo = info.GetFiles();
        foreach (FileInfo f in fInfo) files.Add(new File()
        {
            FilePath = f.DirectoryName,
            FileName = f.Name,
            Extension = f.Extension,
            Current = folderPath
        });

        string[] fols = System.IO.Directory.GetDirectories(folderPath, "*", System.IO.SearchOption.AllDirectories);
        foreach (var fol in fols) ScanFolder(ref files, fol);
    }

    public static string[] GetFolders(string path)
    {
        DirectoryInfo info = new DirectoryInfo(path);
        string[] s;
        try
        {
            s = System.IO.Directory.GetDirectories(path, "*", System.IO.SearchOption.AllDirectories);
            return s;
        }
        catch
        {
            return new string[1] { "Error"};
        }
    }

    public static bool ChkFile(string path)
    {
        return System.IO.File.Exists(path);
    }

    public static bool MakeDir(string path)
    {
        if (Directory.Exists(path))
        {
            return false;
        }
        else
        {
            Directory.CreateDirectory(path);
            return true;
        }
    }

    public class File
    {
        /*
            Ext:.MOV
            FName:IMG_0434.MOV
            FPath:S:\TEST
            Path:S:\TEST\IMG_0434.MOV
            Rel:IMG_0434.MOV
         */

        public string 
            FilePath, 
            FileName, // ファイル名
            Extension, // 拡張子
            Current; // 親フォルダパス as FilePath
        public string Path { get { return FilePath + "\\" + FileName; } }
        public string RelPath { get { return Path.Remove(0, Current.Length + 1); } } // \まで抜くため +1

        /// <summary>
        /// hostPathを取り除いたパスを取得します。
        /// </summary>
        /// <param name="hostPath">例："C:\"</param>
        /// <returns>例：TEST</returns>
        public string RemoveCurrentPath(string hostPath)
        {
            return FilePath.Replace(hostPath, "").Trim();
        }
    }
}