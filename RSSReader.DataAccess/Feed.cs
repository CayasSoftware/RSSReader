using System;

namespace RSSReader.DataAccess
{
    public class Feed
    {
        public int Id { get; set; }
		public string Name { get; set; }
        public string Url { get; set; }
        public DateTime LastUpdated { get; set; }
		public int CategoryId { get; set; }
    }
}