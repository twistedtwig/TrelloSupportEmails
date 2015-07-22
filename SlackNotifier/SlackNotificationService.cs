using System;
using System.Net;

namespace SlackNotifier
{
    public class SlackNotificationService
    {
        private const string SlackUrl = "https://hooks.slack.com/services/";
        public void Notify(string boardId, string message)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.UploadString(new Uri(SlackUrl + boardId), "POST", "{\"text\": \"" + message + "\"}");
            }
        }
    }
}
