using Suncheon.UI;
using UnityEngine;
using Vuplex.WebView;

namespace Suncheon
{
    public class UI_WebView : MonoBehaviour
    {
        [SerializeField] GameObject canvas_WebView;

        public void ShowWebView(string showUrl)
        {
            UTILS.Log($"ÇöÀç URL : {showUrl}");
            canvas_WebView.GetComponent<CanvasWebViewPrefab>().InitialUrl = showUrl;
        }

        public void HideWebView()
        {
            canvas_WebView.GetComponent<CanvasWebViewPrefab>().InitialUrl = "";
            gameObject.SetActive(false);
        }
    }
}