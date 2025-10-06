using System;
using BE;
using MPP;

namespace BLL
{
    public class BLLProductRating
    {
        private readonly MPPProductRating _mpp = new MPPProductRating();

        public ProductRatingAggregate SaveRating(int productId, int userId, int rating)
        {
            if (productId <= 0) throw new ArgumentException("ProductId inválido.");
            if (userId <= 0) throw new ArgumentException("UserId inválido.");
            if (rating < 0 || rating > 5) throw new ArgumentException("Rating fuera de rango (0..5).");

            return _mpp.UpsertAndGetAggregate(productId, userId, (byte)rating);
        }

        public int GetUserRating(int productId, int userId)
        {
            if (productId <= 0 || userId <= 0) return 0;
            return _mpp.GetUserRating(productId, userId);
        }

        public ProductRatingAggregate GetAggregate(int productId)
        {
            if (productId <= 0) return new ProductRatingAggregate { Average = 0m, Count = 0 };
            return _mpp.GetAggregate(productId);
        }
    }
}
