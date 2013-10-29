using System;
using System.Collections.Generic;
using System.Data;

#if ANDROID
using Mono.Data.Sqlite;
#endif

namespace RSSReader.DataAccess
{
	public class CategoryRepository
	{
		readonly string dbPath;

		public CategoryRepository()
		{
			dbPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "rssReader.sqlite");
		}

        /// <summary>
        /// Returns the category for the specified id.
        /// </summary>
        /// <param name="categoryId">Category identifier.</param>
        public Category GetCategory(int categoryId)
        {
            Category category = null;

            using (var connection = new SqliteConnection("Data Source=" + dbPath))
            using (var query = new SqliteCommand("SELECT * FROM Categories WHERE id = @categoryId", connection))
            {
                query.Parameters.AddWithValue("@categoryId", categoryId);

                connection.Open();

                var reader = query.ExecuteReader(CommandBehavior.CloseConnection);

                while (reader.Read())
                {
                    category = new Category();
                    category.Id = int.Parse(reader["id"].ToString());
                    category.Name = reader ["name"].ToString();
                }

                reader.Close();
            }

            return category;
        }

        /// <summary>
        /// Returns all categories.
        /// </summary>
        public IEnumerable<Category> GetAllCategories()
        {
            var categories = new List<Category>();

            using (var connection = new SqliteConnection("Data Source=" + dbPath))
            using (var query = new SqliteCommand("SELECT * FROM Categories", connection))
            {
                connection.Open();

                var reader = query.ExecuteReader(CommandBehavior.CloseConnection);

                while (reader.Read())
                {
                    var category = new Category();
                    category.Id = int.Parse(reader["id"].ToString());
                    category.Name = reader ["name"].ToString();

                    categories.Add(category);
                }

                reader.Close();
            }

            return categories;
        }

        /// <summary>
        /// Saves the specified category.
        /// </summary>
        /// <param name="category">Category to save</param>
        public void Save(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category", "Category should not be null");

            if (category.Id == 0)
            {
                using (var connection = new SqliteConnection("Data Source=" + dbPath))
                using (var query = new SqliteCommand("INSERT INTO Categories VALUES (NULL, @categoryName)", connection))
                {
                    query.Parameters.AddWithValue("@categoryName", category.Name);

                    connection.Open();
                    query.ExecuteNonQuery();
                    connection.Close();
                }
            }
            else
            {
                using (var connection = new SqliteConnection("Data Source=" + dbPath))
                using (var query = new SqliteCommand("UPDATE Categories SET Name = @categoryName WHERE Id = @id", connection))
                {
                    query.Parameters.AddWithValue("@id", category.Id);
                    query.Parameters.AddWithValue("@categoryName", category.Name);

                    connection.Open();
                    query.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Deletes the category with the specified id.
        /// </summary>
        /// <param name="categoryId">Category identifier.</param>
        public void Delete(int categoryId)
        {
            using (var connection = new SqliteConnection("Data Source=" + dbPath))
            using (var query = new SqliteCommand("DELETE FROM Categories WHERE Id = @id", connection))
            {
                query.Parameters.AddWithValue("@id", categoryId);

                connection.Open();
                query.ExecuteNonQuery();
                connection.Close();
            }
        }
	}
}