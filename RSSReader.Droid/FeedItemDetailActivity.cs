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

namespace RSSReader.Droid
{
    [Activity(Label = "FeedItemDetailActivity")]            
    public class FeedItemDetailActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.FeedItemDetail);

            var textView = FindViewById<TextView>(Resource.Id.FeedItemDetail_TextView);
            textView.Text = !String.IsNullOrWhiteSpace(Intent.GetStringExtra("FeedItemContent")) ? Intent.GetStringExtra("FeedItemContent") : "Sorry, no content";

			var btnViewOnWebsite = FindViewById<Button> (Resource.Id.FeedUtenDetail_ViewOnWebsite_Button);
			btnViewOnWebsite.Click+= delegate
			{
				var intent = new Intent(this, typeof(FeedItemContentActivity));
				intent.PutExtra("ItemUrl", Intent.GetStringExtra("FeedItemUrl"));

				StartActivity(intent);
			};
        }
    }
}