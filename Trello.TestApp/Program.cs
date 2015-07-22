using EmailWatchers;
using SlackNotifier;
using System;
using TrelloManagement;
using TrelloManagement.Models.Models;

namespace Trello.TestApp
{
    class Program
    {        
        private static string _supportChannelId;

        static void Main(string[] args)
        {
            var settingsService = new SettingsService();
            _supportChannelId = settingsService.GetSettinngs().SlackSettings.SupportChannelId;

            var watcher = new EwsMailWatcher(settingsService.GetSettinngs().EwsSettings, settingsService.GetSettinngs().TrelloSettings.BoardForwardingEmailAddress);

            watcher.BeginListening(MailRecieved);


            Console.WriteLine("press any key to close app down");
            Console.ReadKey();

            watcher.EndListening();
        }

        private static void MailRecieved(MessageReceived[] messages)
        {
            var slack = new SlackNotificationService();
            foreach (var msg in messages)
            {
                var text =string.Format("Support email received FROM: {0} => TO: {1} -- RE: {2}", msg.From, string.Join(",", msg.To), msg.Subject);
                Console.WriteLine(text);
                
                slack.Notify(_supportChannelId, text + " -- please allow a litle while for Trello to find and process the email.");
            }
        }
    }
}
