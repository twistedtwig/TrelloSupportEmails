using CustomConfigurations;
using TrelloManagement.Models.Settings;

namespace TrelloManagement
{
    public class SettingsService
    {
        private SettingsModel _settings;

        public SettingsModel GetSettinngs()
        {
            if (_settings == null)
            {
                _settings = new SettingsModel();

                var config = new Config().GetSection("settings");
                if (config.Collections.ContainsKey("EwsSettings"))
                {
                    _settings.EwsSettings = config.Collections.GetCollection("EwsSettings").Create<EwsSettings>();
                }

                if (config.Collections.ContainsKey("TrelloSettings"))
                {
                    _settings.TrelloSettings = config.Collections.GetCollection("TrelloSettings").Create<TrelloSettings>();
                }

                if (config.Collections.ContainsKey("SlackSettings"))
                {
                    _settings.SlackSettings = config.Collections.GetCollection("SlackSettings").Create<SlackSettings>();
                }
            }

            return _settings;
        }
    }
}
