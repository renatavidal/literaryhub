using System;
using System.Collections.Generic;
using MPP;
using BE;

namespace BLL
{
    public class BLLChat
    {
        private readonly MPPChat _mpp = new MPPChat();

        public int GetOrCreateOpenThread(int customerId) => _mpp.GetOrCreateOpenThread(customerId);
        public long SendMessage(int threadId, int senderId, bool isAdmin, string body)
        {
            if (string.IsNullOrWhiteSpace(body)) throw new ArgumentException("Mensaje vacío.");
            if (body.Length > 2000) body = body.Substring(0, 2000);
            return _mpp.SendMessage(threadId, senderId, isAdmin, body.Trim());
        }
        public List<BEChatMessage> GetMessagesSince(int threadId, long sinceId) => _mpp.GetMessagesSince(threadId, sinceId);
        public List<BEChatThread> ListThreads() => _mpp.ListThreads();
        public void CloseThread(int threadId) => _mpp.CloseThread(threadId);
    }
}
