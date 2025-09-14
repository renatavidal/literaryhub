using System;

namespace BE
{
    [Serializable]
    public class BENewsletter
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedUtc { get; set; }
        public int? CreatedByUser { get; set; }
        public bool IsPublished { get; set; }
    }
}
