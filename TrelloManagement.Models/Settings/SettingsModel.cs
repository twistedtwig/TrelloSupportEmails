using System;

namespace TrelloManagement.Models.Settings
{
    public class SettingsModel
    {
        public EwsSettings EwsSettings { get; set; }
        public TrelloSettings TrelloSettings { get; set; }
        public SlackSettings SlackSettings { get; set; }
    }

    public class EwsSettings
    {
        public string UserEmailAddress { get; set; }
        public string SharedEmailAddress { get; set; }
        public String Password { get; set; }
    }

    public class TrelloSettings
    {
        public string Key { get; set; }
        public string ApiAppToken { get; set; }
        public string ApiAppName { get; set; }

        public string BoardId { get; set; }
        public string BoardName { get; set; }

        public string ListId { get; set; }
        public string ListName { get; set; }

        public string BoardForwardingEmailAddress { get; set; }
    }

    public class SlackSettings
    {
        public string SupportChannelId { get; set; }
    }
}
