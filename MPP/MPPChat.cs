// MPP/MPPChat.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DAL;
using BE;

namespace MPP
{
    public class MPPChat
    {
        private readonly Acceso _db = new Acceso();

        public int GetOrCreateOpenThread(int customerId)
        {
            var h = new Hashtable { { "@CustomerId", customerId } };
            var dt = _db.Leer("usp_Chat_GetOrCreateOpenThread", h);
            return Convert.ToInt32(dt.Rows[0]["ThreadId"]);
        }

        public long SendMessage(int threadId, int senderId, bool isAdmin, string body)
        {
            var h = new Hashtable {
        {"@ThreadId", threadId}, {"@SenderId", senderId}, {"@IsAdmin", isAdmin}, {"@Body", body}
      };
            var dt = _db.Leer("usp_Chat_SendMessage", h);
            return Convert.ToInt64(dt.Rows[0]["MessageId"]);
        }

        public List<BEChatMessage> GetMessagesSince(int threadId, long sinceId)
        {
            var h = new Hashtable { { "@ThreadId", threadId }, { "@SinceId", sinceId } };
            var dt = _db.Leer("usp_Chat_GetMessagesSince", h);
            var list = new List<BEChatMessage>();
            foreach (DataRow r in dt.Rows)
                list.Add(new BEChatMessage
                {
                    Id = Convert.ToInt64(r["Id"]),
                    ThreadId = Convert.ToInt32(r["ThreadId"]),
                    SenderId = Convert.ToInt32(r["SenderId"]),
                    IsAdmin = Convert.ToBoolean(r["IsAdmin"]),
                    Body = Convert.ToString(r["Body"]),
                    CreatedUtc = Convert.ToDateTime(r["CreatedUtc"])
                });
            return list;
        }

        public List<BEChatThread> ListThreads()
        {
            var dt = _db.Leer("usp_Chat_ListThreads", new Hashtable());
            var list = new List<BEChatThread>();
            foreach (DataRow r in dt.Rows)
                list.Add(new BEChatThread
                {
                    Id = Convert.ToInt32(r["Id"]),
                    CustomerId = Convert.ToInt32(r["CustomerId"]),
                    Status = Convert.ToByte(r["Status"]),
                    CreatedUtc = Convert.ToDateTime(r["CreatedUtc"]),
                    LastMsgUtc = Convert.ToDateTime(r["LastMsgUtc"]),
                    LastMsg = r["LastMsg"] as string
                });
            return list;
        }

        public void CloseThread(int threadId)
        {
            var h = new Hashtable { { "@ThreadId", threadId } };
            _db.Escribir("usp_Chat_CloseThread", h);
        }
    }
}
