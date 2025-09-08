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
        public List<BEBook> GetByUserStatus(int userId, byte status)
        {
            var h = new Hashtable
            {
                {"@UserId", userId},
                {"@Status", status}
            };

            DataTable dt = _db.Leer("sp_UserBooks_GetByUserStatus", h); // o _db.Leer("...")
            var list = new List<BEBook>();
            foreach (DataRow r in dt.Rows)
            {
                var b = new BEBook();
                b.BookId = Convert.ToInt32(r["BookId"]);
                b.GoogleVolumeId = r["GoogleVolumeId"] as string;
                b.Title = r["Title"] as string;
                b.Authors = r["Authors"] as string;
                b.ThumbnailUrl = r["ThumbnailUrl"] as string;
                b.Isbn13 = r["Isbn13"] as string;
                b.PublishedDate = r["PublishedDate"] as string; // si fuera DateTime, castealo
                list.Add(b);
            }
            return list;
        }

        public void MoveToRead(int userId, int bookId)
        {
            var h = new Hashtable { { "@UserId", userId }, { "@BookId", bookId } };
            _db.LeerCantidad("sp_UserBooks_MoveToRead", h); 
        }

        public void Remove(int userId, int bookId, byte status)
        {
            var h = new Hashtable { { "@UserId", userId }, { "@BookId", bookId }, { "@Status", status } };
            _db.LeerCantidad("sp_UserBooks_Remove", h);
        }

        public void Add(int userId, int bookId, byte status)
        {
            var h = new Hashtable { { "@UserId", userId }, { "@BookId", bookId }, { "@Status", status } };
            _db.LeerCantidad("sp_UserBooks_Add", h);
        }
    }
}
