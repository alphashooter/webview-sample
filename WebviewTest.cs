using System.IO;
using System.Text;
using UnityEngine;

public class WebviewTest : MonoBehaviour
{
    private static int _counter = 0;

    public bool visible = false;

    private GameObject _object;
    private WebViewObject _webview;
    private bool _visible;

    private void OnEnable()
    {
        _object = new GameObject($"webview-{++_counter:X4}");
        _webview = _object.AddComponent<WebViewObject>();
        _webview.Init(
            ld: OnWebviewLoaded,
            enableWKWebView: true
        );
        _webview.SetVisibility(_visible = visible);
        _webview.SetMargins(0, 0, 0, 0);
        Debug.Log("webview init");

        var path = Path.Combine(Application.streamingAssetsPath, "sample.html");
        var data = File.ReadAllBytes(path);
        Debug.Log($"html loaded: {data.Length} bytes");

        _webview.LoadHTML(Encoding.UTF8.GetString(data), null);
    }

    private void OnDisable()
    {
        Debug.Log("webview destroy");
        Destroy(_object);
        _object = null;
        _webview = null;
    }

    private void Update()
    {
        if (_visible != visible)
        {
            Debug.Log($"webview visibility: {visible}");
            _webview.SetVisibility(_visible = visible);
        }
    }

    private void OnWebviewLoaded(string msg)
    {
        Debug.Log("webview loaded");

        _webview.EvaluateJS(
            "window.Unity = {" +
            "    call: function (msg) {" +
            "        window.webkit.messageHandlers.unityControl.postMessage(msg);" +
            "    }" +
            "}"
        );
        _webview.EvaluateJS("window.IS_SOUND_ON = false; window.LANGUAGE = \"EN\";");

        visible = true;
    }
}