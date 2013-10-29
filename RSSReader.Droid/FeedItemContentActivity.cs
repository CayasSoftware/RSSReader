using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Webkit;

namespace RSSReader.Droid
{
	[Activity (Label = "FeedItemContentActivity")]			
	public class FeedItemContentActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			//It is also possible to set your views and layouts by code instead of using XML files
			var webView = new WebView (this);
			webView.LoadUrl (Intent.GetStringExtra("ItemUrl"));
			webView.SetWebViewClient (new WebViewClient ());

			SetContentView (webView);
		}
	}
}