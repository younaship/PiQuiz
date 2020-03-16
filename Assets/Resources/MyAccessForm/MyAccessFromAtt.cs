using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyNet;
using System.Threading.Tasks;

public class MyAccessFromAtt : MonoBehaviour
{
    const string URI = "http://api.younaship.com/";
    public InputField inputMessage, inputMail;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool isRunning = false;
    public async void OnPushOK()
    {
        if (isRunning) return;
        else isRunning = true;

        string message = inputMessage.text;
        string mail = inputMail.text;

        if (message.Length <= 0 || message.Length > 300 || mail.Length > 100)
        {
            var d = this.gameObject.AddComponent<Dialog>();
            d.Set("Error", "入力された文字が不正です。文字数を確認してください。", () => {
                isRunning = false;
            }, null);
            return;
        }

        var pop = this.gameObject.AddComponent<PopUpDialog>();
        pop.Set("処理中");

        bool result = await Send();
        pop.Remove();

        var dia = this.gameObject.AddComponent<Dialog>();
        if (result)
            dia.Set("Success", "送信成功しました。", () => Close(), null);
        else
        {
            dia.Set("Error", "失敗しました。", () => { }, null);
            isRunning = false;
        }
    }

    private async Task<bool> Send()
    {
        string message = inputMessage.text + "(" + inputMail.text + ")";
        message.Replace('&', '@');
        message.Replace('=', '@');

        Dictionary<string, string> keys = new Dictionary<string, string>();
        keys.Add("value", message);

        try
        {
            string res = await MyConnection.Post(URI, keys);
            return true;
        }
        catch { return false; }
    }

    public void Close()
    {
        Destroy(this.gameObject.GetComponent<MyAccessFrom>());
        Destroy(this.gameObject); // Canvas
    }
}
