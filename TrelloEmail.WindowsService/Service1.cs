using System.ServiceProcess;
using EmailWatchers;
using SlackNotifier;
using TrelloManagement;

namespace TrelloEmail.WindowsService
{
    public partial class Service1 : ServiceBase
    {
        private SupportManager _supportManager;

        public Service1()
        {
            InitializeComponent();

            var settingsService = new SettingsService();
            var settings = settingsService.GetSettinngs();

            var ewsMailWatcher = new EwsMailWatcher(settings.EwsSettings, settings.TrelloSettings.BoardForwardingEmailAddress);
            var slackService = new SlackNotificationService();

            _supportManager = new SupportManager(ewsMailWatcher, slackService, settingsService);
        }

        protected override void OnStart(string[] args)
        {
            _supportManager.BeginListening();
        }

        protected override void OnStop()
        {
            _supportManager.EndListening();
            _supportManager = null;
        }
    }
}
