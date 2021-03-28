using System;
using System.IO;

namespace SignalRChat.Domain
{
    public class ChatMessage
    {
        public string Name { get; set; }

        public string Message { get; set; }

        public string GroupFilesId { get; set; }
    }
}
