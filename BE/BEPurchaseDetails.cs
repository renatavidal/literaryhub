
using System;

namespace BE
{
    public class BEPurchaseDetails
    {
        public int PurchaseId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public byte Status { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
    }
}
