// MPP/MPPCompras.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DAL;
using BE;

namespace MPP
{
    public class MPPCompras
    {
        private readonly Acceso _db = new Acceso();

        public List<BECompraListItem> ByUser(int userId)
        {
            var t = _db.Leer("usp_Purchases_ByUser", new Hashtable { { "@UserId", userId } });
            var list = new List<BECompraListItem>();
            foreach (DataRow r in t.Rows)
            {
                list.Add(new BECompraListItem
                {
                    PurchaseId = Convert.ToInt32(r["PurchaseId"]),
                    BookId = Convert.ToInt32(r["BookId"]),
                    Title = r["Title"] as string,
                    Price = Convert.ToDecimal(r["Price"]),
                    Currency = Convert.ToString(r["Currency"]),
                    CreatedUtc = Convert.ToDateTime(r["CreatedUtc"])
                });
            }
            return list;
        }
    }
}
