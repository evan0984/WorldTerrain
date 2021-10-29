using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class ScriptUnityTest : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Hello();

    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    private Button sendToJavascriptButton;

    // Start is called before the first frame update
    void Start()
    {
        sendToJavascriptButton = GameObject.Find("SendToWebButton").GetComponent<Button>();
        sendToJavascriptButton.onClick.AddListener(delegate { SendToJavascriptClicked(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SendToJavascriptClicked()
    {
        Hello();
    }

    public void SetText(string text)
    {
        var jsText = GameObject.Find("JavascriptToUnityText").GetComponent<Text>();
        jsText.text = string.Format("JS to Unity Test = {0}", text);
    }
}
