using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class BEComment
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public string Body { get; set; }
    }
}
