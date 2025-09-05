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
    public class MPPBook
    {
        private readonly Acceso _db = new Acceso();

        public int AddIfNotExists(BEBook b)
        {
            var h = new Hashtable {
                {"@GoogleVolumeId", b.GoogleVolumeId},
                {"@Title",          (object)b.Title ?? DBNull.Value},
                {"@Authors",        (object)b.Authors ?? DBNull.Value},
                {"@ThumbnailUrl",   (object)b.ThumbnailUrl ?? DBNull.Value},
                {"@Isbn13",         (object)b.Isbn13 ?? DBNull.Value},
                {"@PublishedDate",  (object)b.PublishedDate ?? DBNull.Value}
            };
            return _db.LeerCantidad("sp_Book_AddIfNotExists", h); // devuelve BookId
        }
    }
}
