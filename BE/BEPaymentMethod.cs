using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class BEPaymentMethod
    {
        public int PaymentMethodId { get; set; }
        public int UserId { get; set; }
        public string CardholderName { get; set; }
        public string CardBrand { get; set; }
        public string Last4 { get; set; }
        public byte ExpMonth { get; set; }
        public short ExpYear { get; set; }
        public byte[] PanEncrypted { get; set; }
        public byte[] Iv { get; set; }
    }
}
