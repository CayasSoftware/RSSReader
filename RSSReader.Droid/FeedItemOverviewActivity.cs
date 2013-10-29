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
    [Activity(Label = "FeedItemOverviewActivity")]            
    public class FeedItemOverviewActivity : ListActivity
    {
        QDFeedParser.IFeed feed;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.ActionBar);

            if(String.IsNullOrWhiteSpace(Intent.GetStringExtra("FeedUrl")))
            {
                Toast.MakeText(this, "Sorry no feed url found", ToastLength.Long).Show();
                Finish();
            }

            ListView.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e)
            {
                var intent = new Intent(this, typeof(FeedItemDetailActivity));
                intent.PutExtra("FeedItemContent", feed.Items[e.Position].Content);
				intent.PutExtra("FeedItemUrl", feed.Items[e.Position].Link);

                StartActivity(intent);
            };

            FillListView(Intent.GetStringExtra("FeedUrl"));
        }

        /// <summary>
        /// Fills the list view with feed items for a specific feed.
        /// </summary>
        void FillListView(string url)
        {
            try
            {
                feed = new QDFeedParser.HttpFeedFactory().CreateFeed(new Uri(url));
                var feedTitleList = feed.Items.Select(i=>i.Title).ToList();

                ListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, feedTitleList);
            }
            catch (Exception)
            {
                Toast.MakeText(this, "Ops, something went wrong on finding the feed", ToastLength.Long).Show();
                Finish();
            }
        }
    }
}