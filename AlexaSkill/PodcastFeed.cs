using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AlexaSkill
{

    public class Entry
    {
        public string title { get; set; }
        public string link { get; set; }
        public string author { get; set; }
        public string publishedDate { get; set; }
        public string contentSnippet { get; set; }
        public string content { get; set; }
        public List<object> categories { get; set; }
    }

    public class Feed
    {
        public string feedUrl { get; set; }
        public string title { get; set; }
        public string link { get; set; }
        public string author { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public List<Entry> entries { get; set; }
    }

    public class ResponseData
    {
        public Feed feed { get; set; }
    }

    public class PodcastFeed
    {
        public ResponseData responseData { get; set; }
        public object responseDetails { get; set; }
        public int responseStatus { get; set; }
    }
}