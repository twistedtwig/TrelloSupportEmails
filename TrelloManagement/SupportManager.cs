using EmailWatchers;
using SlackNotifier;
using TrelloManagement.Models.Models;

namespace TrelloManagement
{
    public class SupportManager
    {
        private EwsMailWatcher _ewsMailWatcher;
        private SettingsService _settingsService;
        private SlackNotificationService _slackService;

        public SupportManager(EwsMailWatcher ewsMailWatcher, SlackNotificationService slackService, SettingsService settingsService)
        {
            _ewsMailWatcher = ewsMailWatcher;
            _slackService = slackService;
            _settingsService = settingsService;
        }

        public void BeginListening()
        {
            _ewsMailWatcher.BeginListening(MailRecieved);
        }

        public void EndListening()
        {
            _ewsMailWatcher.EndListening();
            _ewsMailWatcher = null;

            //dispose of trello.
            _settingsService = null;
            _slackService = null;
        }

        private void MailRecieved(MessageReceived[] messages)
        {
            var supportChannelId = _settingsService.GetSettinngs().SlackSettings.SupportChannelId;

            foreach (var msg in messages)
            {
                var text = string.Format("Support email received FROM: {0} => TO: {1} -- RE: {2}", msg.From, string.Join(",", msg.To), msg.Subject);
                _slackService.Notify(supportChannelId, text + " -- please allow a litle while for Trello to find and process the email.");
            }
        }
    }
}
