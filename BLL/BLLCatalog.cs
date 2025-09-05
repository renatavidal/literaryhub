using BE;
using MPP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLLCatalog
    {
        private readonly MPPBook _mBook = new MPPBook();
        private readonly MPPUserBook _mUserBook = new MPPUserBook();
        private readonly MPPComment _mComment = new MPPComment();
        private readonly MPPPayment _mPay = new MPPPayment();

        /// Garantiza que el libro exista (lo crea con datos de Google si no).
        public int EnsureBook(BEBook bookFromGoogle)
        {
            return _mBook.AddIfNotExists(bookFromGoogle);
        }

        public void SetUserBookStatus(int userId, BEBook book, UserBookStatus status)
        {
            int bookId = EnsureBook(book);
            _mUserBook.UpsertStatus(userId, bookId, status);
        }

        public void AddComment(int userId, BEBook book, string body)
        {
            int bookId = EnsureBook(book);
            _mComment.Add(userId, bookId, body);
        }

        public int SavePaymentMethodAndPurchase(int userId, BEBook book, BEPaymentMethod pm, decimal price, string currency)
        {
            int bookId = EnsureBook(book);
            pm.UserId = userId;
            int pmId = _mPay.AddMethod(pm);
            return _mPay.CreatePurchase(userId, bookId, pmId, price, currency);
        }

        public void SetPurchaseStatus(int purchaseId, byte status) => _mPay.SetPurchaseStatus(purchaseId, status);
    }
}
