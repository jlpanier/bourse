using Android.Content;
using Android.Webkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bourse.Views.Web
{
    [Activity(Label = "@string/app_name", MainLauncher = false)]
    public class WebViewActivity : BaseActivity
    {
        #region Overall

        /// <summary>
        /// Layout de notre activité
        /// </summary>
        protected override int LayoutResourceId => Resource.Layout.WebView;

        #endregion


        private WebView web_view;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            web_view = FindViewById<WebView>(Resource.Id.webview);
            web_view.Settings.JavaScriptEnabled = true;
            web_view.SetWebViewClient(new CustomViewClient());
            web_view.LoadUrl(Intent.GetStringExtra(BaseActivity.ParamUrl));
        }
    }
}
