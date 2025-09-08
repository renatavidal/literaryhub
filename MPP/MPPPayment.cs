using BE;
using DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPP
{
    public class MPPPayment
    {
        private readonly Acceso _db = new Acceso();

        public int AddMethod(BEPaymentMethod pm)
        {
            var h = new Hashtable {
                {"@UserId", pm.UserId},
                {"@CardholderName", pm.CardholderName},
                {"@CardBrand", (object)pm.CardBrand ?? DBNull.Value},
                {"@Last4", pm.Last4},
                {"@ExpMonth", pm.ExpMonth},
                {"@ExpYear", pm.ExpYear},
                {"@PanEncrypted", pm.PanEncrypted},
                {"@Iv", pm.Iv}
            };
            return _db.LeerCantidad("sp_PaymentMethod_Add", h); // PaymentMethodId
        }

        public int CreatePurchase(int userId, int bookId, int paymentMethodId, decimal price, string currency)
        {
            var h = new Hashtable {
                {"@UserId", userId},
                {"@BookId", bookId},
                {"@PaymentMethodId", paymentMethodId},
                {"@Price", price},
                {"@Currency", currency}
            };
            return _db.LeerCantidad("sp_Purchase_Create", h); // PurchaseId
        }

        public void SetPurchaseStatus(int purchaseId, byte status)
        {
            var h = new Hashtable {
                {"@PurchaseId", purchaseId},
                {"@Status", status}
            };
            _db.Escribir("sp_Purchase_SetStatus", h);
        }
        public BEPurchaseDetails GetPurchaseDetails(int purchaseId)
        {
            var h = new Hashtable { { "@PurchaseId", purchaseId } };
            DataTable dt = _db.Leer("sp_Purchase_GetDetails", h);
            if (dt == null || dt.Rows.Count == 0) return null;

            var r = dt.Rows[0];
            return new BEPurchaseDetails
            {
                PurchaseId = Convert.ToInt32(r["PurchaseId"]),
                UserId = Convert.ToInt32(r["UserId"]),
                BookId = Convert.ToInt32(r["BookId"]),
                PaymentMethodId = Convert.ToInt32(r["PaymentMethodId"]),
                Price = Convert.ToDecimal(r["Price"]),
                Currency = Convert.ToString(r["Currency"]),
                Status = Convert.ToByte(r["Status"]),
                CreatedUtc = Convert.ToDateTime(r["CreatedUtc"]),
                Title = Convert.ToString(r["Title"]),
                Authors = Convert.ToString(r["Authors"])
            };
        }
    }
    
}
