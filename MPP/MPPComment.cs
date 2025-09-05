using DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPP
{
    public class MPPComment
    {
        private readonly Acceso _db = new Acceso();

        public void Add(int userId, int bookId, string body)
        {
            var h = new Hashtable {
                {"@UserId", userId},
                {"@BookId", bookId},
                {"@Body",   body}
            };
            _db.Escribir("sp_Comment_Add", h);
        }
    }
}
