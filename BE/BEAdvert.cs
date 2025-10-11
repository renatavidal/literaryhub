
using System;

namespace BE
{
    public class BEAdvert
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ImageUrl { get; set; }
        public string LinkUrl { get; set; }
        public bool IsActive { get; set; }
        public int Weight { get; set; }
        public DateTime? StartUtc { get; set; }
        public DateTime? EndUtc { get; set; }
    }
}

