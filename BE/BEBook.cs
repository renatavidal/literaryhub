using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class BEBook
    {
        public int BookId { get; set; }
        public string GoogleVolumeId { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Isbn13 { get; set; }
        public string PublishedDate { get; set; }
        public enum UserBookStatus : byte { None = 0, WantToRead = 1, Read = 2 }
    }
    public enum UserBookStatus : byte { None = 0, WantToRead = 1, Read = 2 }
    public class BECompraListItem
    {
        public int PurchaseId { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedUtc { get; set; }
    }

}
