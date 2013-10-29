using System;
using System.Collections.Generic;
using System.Data;

#if ANDROID
using Mono.Data.Sqlite;
#endif

namespace RSSReader.DataAccess
{
    public class FeedRepository
    {
        readonly string dbPath;

        public FeedRepository()
        {
            dbPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "rssReader.sqlite");
        }

        /// <summary>
        /// Returns the feed with the specified id.
        /// </summary>
        /// <param name="feedId">Feed identifier.</param>
        public Feed GetFeed(int feedId)
        {
            Feed feed = null;

            using (var connection = new SqliteConnection("Data Source=" + dbPath))
            using (var query = new SqliteCommand("SELECT * FROM Feeds WHERE id = @feedId", connection))
            {
                query.Parameters.AddWithValue("@feedId", feedId);

                connection.Open();

                var reader = query.ExecuteReader(CommandBehavior.CloseConnection);

                while (reader.Read())
                {
                    feed = new Feed();
                    feed.Id = int.Parse(reader["id"].ToString());
					feed.Name = reader ["name"].ToString();
                    feed.Url = reader ["url"].ToString();
                    feed.LastUpdated = DateTime.Parse (reader ["LastUpdated"].ToString ());
                    feed.CategoryId = int.Parse(reader["categoryId"].ToString());
                }

                reader.Close();
            }

            return feed;
        }

        /// <summary>
        /// Returns all subscribed feeds.
        /// </summary>
        public IEnumerable<Feed> GetAllFeeds()
        {
			var feeds = new List<Feed>();

			using (var connection = new SqliteConnection("Data Source=" + dbPath))
			using (var query = new SqliteCommand("SELECT * FROM Feeds", connection))
			{
				connection.Open();

				var reader = query.ExecuteReader(CommandBehavior.CloseConnection);

				while (reader.Read())
				{
					var feed = new Feed();
					feed.Id = int.Parse(reader["id"].ToString());
					feed.Name = reader ["name"].ToString();
					feed.Url = reader ["url"].ToString();
					feed.LastUpdated = DateTime.Parse (reader ["LastUpdated"].ToString ());
					feed.CategoryId = int.Parse(reader["categoryId"].ToString());

					feeds.Add(feed);
				}

				reader.Close();
			}

			return feeds;
        }

		/// <summary>
		/// Returns all feeds for the specified category id.
		/// </summary>
		/// <param name="categoryId">The id from the category.</param>
		public IEnumerable<Feed> GetAllFeeds(int categoryId)
		{
			var feeds = new List<Feed>();

			using (var connection = new SqliteConnection("Data Source=" + dbPath))
			using (var query = new SqliteCommand("SELECT * FROM Feeds WHERE CategoryId = @categoryId", connection))
			{
				query.Parameters.AddWithValue ("@categoryId", categoryId);

				connection.Open();

				var reader = query.ExecuteReader(CommandBehavior.CloseConnection);

				while (reader.Read())
				{
					var feed = new Feed();
					feed.Id = int.Parse(reader["id"].ToString());
					feed.Name = reader ["name"].ToString();
					feed.Url = reader ["url"].ToString();
					feed.LastUpdated = DateTime.Parse (reader ["LastUpdated"].ToString ());
					feed.CategoryId = int.Parse(reader["categoryId"].ToString());

					feeds.Add(feed);
				}

				reader.Close();
			}

			return feeds;
		}

        /// <summary>
        /// Saves a feed.
        /// </summary>
        /// <param name="feed">Feed that should be saved.</param>
        public void Save(Feed feed)
        {
            if (feed == null)
                throw new ArgumentNullException("feed", "Feed should not be null");

            if (feed.Id == 0)
            {
                using (var connection = new SqliteConnection("Data Source=" + dbPath))
                using (var query = new SqliteCommand("INSERT INTO Feeds VALUES (NULL, @feedName, @url, @lastUpdated, @categoryId)", connection))
                {
					query.Parameters.AddWithValue("@feedName", feed.Name);
					query.Parameters.AddWithValue("@url", feed.Url);
					query.Parameters.AddWithValue("@lastUpdated", feed.LastUpdated);
					query.Parameters.AddWithValue("@categoryId", feed.CategoryId);

                    connection.Open();
                    query.ExecuteNonQuery();
                    connection.Close();
                }
            }
            else
            {
                using (var connection = new SqliteConnection("Data Source=" + dbPath))
					using (var query = new SqliteCommand("UPDATE Feeds SET Name = @feedName Url = @Url, LastUpdated = @lastUpdated, CategoryId = @categoryId WHERE Id = @id", connection))
                {
                    query.Parameters.AddWithValue("@id", feed.Id);
					query.Parameters.AddWithValue("@feedName", feed.Name);
					query.Parameters.AddWithValue("@url", feed.Url);
					query.Parameters.AddWithValue("@lastUpdated", feed.LastUpdated);
					query.Parameters.AddWithValue("@categoryId", feed.CategoryId);

                    connection.Open();
                    query.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Deletes the feed with the provided id.
        /// </summary>
        /// <param name="feedId">The id of the feed that should be deleted.</param>
        public void Delete(int feedId)
        {
            using (var connection = new SqliteConnection("Data Source=" + dbPath))
            using (var query = new SqliteCommand("DELETE FROM Feeds WHERE Id = @id", connection))
            {
                query.Parameters.AddWithValue("@id", feedId);

                connection.Open();
                query.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}