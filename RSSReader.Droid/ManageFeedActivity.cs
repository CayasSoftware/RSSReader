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
    [Activity(Label = "ManageFeedActivity")]
    public class ManageFeedActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ManageFeed);

            var feedCategories = new DataAccess.CategoryRepository().GetAllCategories().ToList();

			var txtName = FindViewById<EditText> (Resource.Id.ManageFeed_FeedName_EditText);
            var txtUrl = FindViewById<EditText>(Resource.Id.ManageFeed_Url_EditText);

            var lvCategory = FindViewById<ListView>(Resource.Id.ManageFeed_FeedCategories_ListView);
            lvCategory.ChoiceMode = ChoiceMode.Single;
            lvCategory.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItemSingleChoice, feedCategories.Select(c => c.Name).ToList());

            if (feedCategories.Count > 0)
                lvCategory.SetSelection(0);

            var btnSave = FindViewById<Button>(Resource.Id.ManageFeed_Save_Button);
            btnSave.Click += delegate
            {
                var feedRepo = new DataAccess.FeedRepository();
                feedRepo.Save(new DataAccess.Feed(){ Name = txtName.Text, Url = txtUrl.Text, CategoryId = lvCategory.SelectedItemPosition > -1 ? feedCategories[lvCategory.SelectedItemPosition].Id : 0});

                Toast.MakeText(this,"Feed saved",ToastLength.Short).Show();
                Finish();
            };
        }
    }
}