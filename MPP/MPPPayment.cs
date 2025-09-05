using BE;
using DAL;
using System;
using System.Collections;
using System.Collections.Generic;
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
    }
    
}
