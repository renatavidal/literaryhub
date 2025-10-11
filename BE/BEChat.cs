// BE/BEChat.cs
using System;
using System.Collections.Generic;

namespace BE
{
    public class BEChatThread
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public byte Status { get; set; }  // 1=Open,2=Closed
        public DateTime CreatedUtc { get; set; }
        public DateTime LastMsgUtc { get; set; }
        public string LastMsg { get; set; }
    }

    public class BEChatMessage
    {
        public long Id { get; set; }
        public int ThreadId { get; set; }
        public int SenderId { get; set; }
        public bool IsAdmin { get; set; }
        public string Body { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}
