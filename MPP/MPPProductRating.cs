using System;
using System.Collections;
using System.Data;
using BE;
using DAL;

namespace MPP
{
    public class MPPProductRating
    {
        private readonly Acceso _datos = new Acceso();

        // Upsert del rating y devuelve agregados (promedio y cantidad)
        public ProductRatingAggregate UpsertAndGetAggregate(int productId, int userId, byte rating)
        {
            var p = new Hashtable {
                {"@ProductId", productId},
                {"@UserId",    userId},
                {"@Rating",    rating}
            };

            // IMPORTANTE: usamos Leer porque el SP devuelve un resultset con Avg/Count
            var dt = _datos.Leer("usp_ProductRating_Upsert", p);

            var agg = new ProductRatingAggregate { Average = 0m, Count = 0 };
            if (dt.Rows.Count > 0)
            {
                var r = dt.Rows[0];
                agg.Average = r.IsNull("AvgRating") ? 0m : Convert.ToDecimal(r["AvgRating"]);
                agg.Count = r.IsNull("RatingsCount") ? 0 : Convert.ToInt32(r["RatingsCount"]);
            }
            return agg;
        }

        // Rating del usuario para ese producto (0 si no existe)
        public int GetUserRating(int productId, int userId)
        {
            var p = new Hashtable {
                {"@ProductId", productId},
                {"@UserId",    userId}
            };
            var dt = _datos.Leer("sp_ProductRating_GetUserRating", p);

            if (dt.Rows.Count == 0 || dt.Rows[0].IsNull("Rating"))
                return 0;

            return Convert.ToInt32(dt.Rows[0]["Rating"]);
        }

        // Agregados actuales del producto (promedio y cantidad)
        public ProductRatingAggregate GetAggregate(int productId)
        {
            var p = new Hashtable { { "@ProductId", productId } };
            var dt = _datos.Leer("sp_ProductRating_GetAggregate", p);

            var agg = new ProductRatingAggregate { Average = 0m, Count = 0 };
            if (dt.Rows.Count > 0)
            {
                var r = dt.Rows[0];
                agg.Average = r.IsNull("AvgRating") ? 0m : Convert.ToDecimal(r["AvgRating"]);
                agg.Count = r.IsNull("RatingsCount") ? 0 : Convert.ToInt32(r["RatingsCount"]);
            }
            return agg;
        }
    }
}
