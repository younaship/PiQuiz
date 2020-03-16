using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

/*
 * 19/06/14
 * ver 0.1.0
 * 
 * インターネットの接続(GET/POST)を簡略化するクラス
 */
namespace MyNet
{
    public class Debug
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

    }

    public class MyConnection
    {
        public class Result
        {
            public enum Status { Success, Error }
            public string Message;
        }

        /// <summary>
        /// GETリクエスト。
        /// </summary>
        /// <param name="parameters">Param ("id","value(UTF-8)") </param>
        /// <returns></returns>
        public static async Task<string> Get(string uri, Dictionary<string, string> parameters = null)
        {
            try
            {
                if (parameters != null)
                {
                    uri += "?";
                    foreach (KeyValuePair<string, string> kvp in parameters) uri += "&" + kvp.Key + "=" + Uri.EscapeUriString(kvp.Value);
                }


                Debug.Log("[GET] " + uri);
                var req = (HttpWebRequest)WebRequest.Create(uri);
                WebResponse res = await req.GetResponseAsync();
                Stream stm = res.GetResponseStream();

                string json;
                using (StreamReader str = new StreamReader(stm))
                {
                    json = await str.ReadToEndAsync();
                }
                return json;
            }
            catch (Exception e)
            {
                Debug.LogError("Ex" + e.GetType());
                return null;
            }
        }

        /// <summary>
        /// GETリクエスト。OLD
        /// </summary>
        /// <param name="parameters">Param ("id","value(UTF-8)") </param>
        /// <returns></returns>
        public static async Task<string> Get_(string uri, Dictionary<string, string> parameters = null)
        {
            if (parameters != null)
            {
                uri += "?";
                foreach (KeyValuePair<string, string> kvp in parameters) uri += "&" + kvp.Key + "=" + Uri.EscapeUriString(kvp.Value);
            }

            Debug.Log("[GET] " + uri);
            var req = (HttpWebRequest)WebRequest.Create(uri);
            WebResponse res = await req.GetResponseAsync();
            Stream stm = res.GetResponseStream();

            string json;
            using (StreamReader str = new StreamReader(stm))
            {
                json = await str.ReadToEndAsync();
            }
            return json;
        }

        public static Dictionary<string, string> CreateParam()
        {
            return new Dictionary<string, string>();
        }

        public static async Task<string> Post(string uri, string json)
        {
            Debug.Log("[POST] " + uri);
            var req = WebRequest.Create(uri);
            var bytes = Encoding.UTF8.GetBytes(json);
            req.Method = "POST";
            req.ContentLength = bytes.Length;
            req.Timeout = 5000;
            req.ContentType = "application/json";

            using (var reqStm = await req.GetRequestStreamAsync())
            {
                await reqStm.WriteAsync(bytes, 0, bytes.Length);
            }
            var res = await req.GetResponseAsync();

            string rJson;
            using (Stream stm = res.GetResponseStream())
            using (StreamReader str = new StreamReader(stm))
            {
                rJson = await str.ReadToEndAsync();
            }
            return rJson;
        }

        /// <summary>
        /// POST送信リクエスト。例外は外部で行う。&!!
        /// </summary>
        public static async Task<string> Post(string uri, Dictionary<string, string> keys)
        {
            Debug.Log("[POST] " + uri);
            var req = WebRequest.Create(uri);
            string s = "";
            foreach (KeyValuePair<string, string> kvp in keys)
            {
                s += "" + kvp.Key + "=" + kvp.Value;
                s += "&";
            }

            var bytes = Encoding.UTF8.GetBytes(s);
            req.Method = "POST";
            req.ContentLength = bytes.Length;
            req.Timeout = 5000;
            req.ContentType = "application/x-www-form-urlencoded";

            using (var reqStm = await req.GetRequestStreamAsync())
            {
                await reqStm.WriteAsync(bytes, 0, bytes.Length);
            }
            var res = await req.GetResponseAsync();

            string rJson;
            using (Stream stm = res.GetResponseStream())
            using (StreamReader str = new StreamReader(stm))
            {
                rJson = await str.ReadToEndAsync();
            }
            Debug.Log("[Res]" + rJson + "[Data]" + s + "[" + bytes.Length + "]");
            return rJson;
        }

        /// <summary>
        /// ファイルのダウンロードを行います。(pathにはファイル名、拡張子を含む必要があります。)
        /// </summary>
        public static async Task<bool> DownLoadToLocal(string uri, string path)
        {
            Debug.Log("DL > Try Download task[" + uri + "]>[" + path + "]");
            try
            {
                var web = new WebClient();
                await web.DownloadFileTaskAsync(new Uri(uri), path);
            }
            catch (Exception e)
            {
                Debug.LogError("[Ex] Missing Download. " + e.GetType() + " : " + e.Message);
                FileInfo f = new FileInfo(path);
                if (f.Exists) f.Delete(); // 失敗したファイル削除
                return false;
            }
            Debug.Log("Success > Download OK.");
            return true;
        }

        public static string GetMD5(string baseStr)
        {
            MD5 md5 = MD5.Create();
            var mb = md5.ComputeHash(Encoding.UTF8.GetBytes(baseStr));

            string res = "";
            foreach (byte b in mb) res += b.ToString("x2");
            return res;
        }

        public static string GetHex(byte[] bytes)
        {
            string res = "";
            foreach (byte b in bytes) res += b.ToString("x2");
            return res;
        }
    }
}