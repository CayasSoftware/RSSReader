using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Linq;
using System.Collections.Generic;

namespace RSSReader.Droid
{
    [Activity (Label = "RSSReader.Droid", MainLauncher = true)]
    public class MainActivity : ListActivity, ActionMode.ICallback
    {
        List<DataAccess.Feed> feeds;
        ActionMode actionMode;
        int actionModePosition = -1;

        /// <summary>
        /// Raises the create event.
        /// </summary>
        /// <param name="savedInstanceState">Saved instance state.</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.ActionBar);

			var dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "rssReader.sqlite");

			if (!System.IO.File.Exists(dbPath))
				CopyDatabase("rssReader.sqlite");

            // Because it is a list activity, we can directly use the ListView property.
            ListView.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs e)
            {
                if (actionMode != null)
                {
                    actionMode.Finish();
                    actionMode = null;
                }

                // Intents are used to communicate between components such as activities.
                var intent = new Intent(this, typeof(FeedItemOverviewActivity));
                intent.PutExtra("FeedUrl", feeds[e.Position].Url);

                StartActivity(intent);
            };

            ListView.ItemLongClick+= delegate(object sender, AdapterView.ItemLongClickEventArgs e)
            {
                if (actionMode != null)
                    return;

                actionModePosition = e.Position;

                actionMode = StartActionMode(this);
            };
        }

		protected override void OnResume()
		{
			base.OnResume ();

			FillListView();
		}

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MainActionBarItems, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Intent intent;

            switch (item.ItemId)
            {
                case Resource.Id.MainActionBarMenu_AddFeed:
                    intent = new Intent(this, typeof(ManageFeedActivity));
                    StartActivity(intent);
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Fills the list view with feed items for a specific feed.
        /// </summary>
        void FillListView()
        {
            try
            {
                feeds = new DataAccess.FeedRepository().GetAllFeeds().ToList();
                ListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, feeds.Select(f=>f.Name).ToList());
            }
            catch(Exception)
            {
                // A toast is a small pop up das disappears automaticaly.
                Toast.MakeText(this, "Ops, something went wrong.", ToastLength.Long).Show();
            }
        }

		void CopyDatabase(string dataBaseName)
		{
			var dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dataBaseName);

			if (!System.IO.File.Exists(dbPath))
			{
				var dbAssetStream = Assets.Open(dataBaseName);
				var dbFileStream = new System.IO.FileStream(dbPath, System.IO.FileMode.OpenOrCreate);
				var buffer = new byte[1024];

				int b = buffer.Length;
				int length;

				while ((length = dbAssetStream.Read(buffer, 0, b)) > 0)
				{
					dbFileStream.Write(buffer, 0, length);
				}

				dbFileStream.Flush();
				dbFileStream.Close();
				dbAssetStream.Close();
			}
		}

        #region ICallback implementation

        public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
        {
            if (item.ItemId == Resource.Id.ActionModeDeleteItem)
            {
                if (actionModePosition == -1)
                    return false;

                var feed = feeds [actionModePosition];

                new DataAccess.FeedRepository().Delete(feed.Id);

                FillListView();

                actionModePosition = -1;

                mode.Finish();

                Toast.MakeText(this, String.Format("{0} deleted.", feed.Name), ToastLength.Short).Show();

                return true;
            }

            return false;
        }

        public bool OnCreateActionMode(ActionMode mode, IMenu menu)
        {
            mode.MenuInflater.Inflate(Resource.Menu.ActionModeDeleteItems, menu);
            return true;
        }

        public void OnDestroyActionMode(ActionMode mode)
        {
            actionMode = null;
        }

        public bool OnPrepareActionMode(ActionMode mode, IMenu menu)
        {
            return false;
        }

        #endregion
    }
}