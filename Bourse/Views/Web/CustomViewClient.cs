using Android.Webkit;


namespace Bourse.Views.Web
{
    public class CustomViewClient : WebViewClient
    {
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            view.LoadUrl(url);
            return false;
        }
    }
}
