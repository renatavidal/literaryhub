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
    public class MPPUserBook
    {
        private readonly Acceso _db = new Acceso();

        public void UpsertStatus(int userId, int bookId, UserBookStatus status)
        {
            var h = new Hashtable {
                {"@UserId", userId},
                {"@BookId", bookId},
                {"@Status", (byte)status}
            };
            _db.Escribir("sp_UserBook_UpsertStatus", h);
        }
    }
}
