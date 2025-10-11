using System;
using System.Collections;
using System.Data;
using DAL;

namespace MPP
{
    public class MPPRevenue
    {
        private readonly Acceso _datos = new Acceso();

        public DataTable SumBy(string groupBy, DateTime? fromUtc, DateTime? toUtc, string currency)
        {
            var h = new Hashtable {
                {"@FromUtc",   (object)fromUtc ?? DBNull.Value},
                {"@ToUtc",     (object)toUtc   ?? DBNull.Value},
                {"@Currency",  (object)currency ?? DBNull.Value},
                {"@GroupBy",   (object)((groupBy ?? "DAY").ToUpperInvariant())}
            };
            return _datos.Leer("usp_Revenue_Sum_By", h);
        }
    }
}
