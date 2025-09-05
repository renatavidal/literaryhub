using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class BEPurchase
    {
        public int PurchaseId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public byte Status { get; set; } // 0=Pending,1=Paid,2=Failed
    }
}
