using System.IO;

namespace TrelloManagement.Models.Models
{
    public class MessageReceived
    {
        public string From { get; set; }
        public string[] To { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public MemoryStream[] Attachments { get; set; }
    }
}
