// MPP/MPPAdvert.cs
using System;
using System.Collections;
using System.Data;
using DAL;

namespace MPP
{
    public class MPPAdvert
    {
        private readonly Acceso _datos = new Acceso();

        public DataTable ListAll()
        {
            return _datos.Leer("usp_Advert_ListAll", new Hashtable());
        }

        public int Save(int? id, string title, string body, string imageUrl, string linkUrl,
                        bool isActive, int weight, DateTime? startUtc, DateTime? endUtc)
        {
            var h = new Hashtable {
                {"@Id", (object)id ?? DBNull.Value},
                {"@Title", title},
                {"@Body", (object)body ?? DBNull.Value},
                {"@ImageUrl", (object)imageUrl ?? DBNull.Value},
                {"@LinkUrl", (object)linkUrl ?? DBNull.Value},
                {"@IsActive", isActive},
                {"@Weight", weight},
                {"@StartUtc", (object)startUtc ?? DBNull.Value},
                {"@EndUtc", (object)endUtc ?? DBNull.Value}
            };
            var dt = _datos.Leer("usp_Advert_Save", h);
            return Convert.ToInt32(dt.Rows[0]["NewId"]);
        }

        public void Delete(int id)
        {
            var h = new Hashtable { { "@Id", id } };
            _datos.Escribir("usp_Advert_Delete", h);
        }

        public DataTable GetRandomActive()
        {
            return _datos.Leer("usp_Advert_GetRandomActive", new Hashtable());
        }
    }
}
