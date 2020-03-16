using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

class MyNetworks
{
    public static async Task<string> GetResponse(string reqUri)
    {
        WebResponse res;
        try
        {
            WebRequest req = WebRequest.Create(reqUri);
            res = await req.GetResponseAsync();
        }
        catch
        {
            return null;
        }
        Console.WriteLine("Res:" + ((HttpWebResponse)res).StatusCode);

        string s;
        using (Stream stm = res.GetResponseStream())
        using (StreamReader read = new StreamReader(stm))
        {
            s = read.ReadToEnd();
        }
        return s;
    }
}