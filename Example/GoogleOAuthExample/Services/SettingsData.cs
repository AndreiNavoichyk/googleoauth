using System.IO.IsolatedStorage;

namespace GoogleOAuthExample.Services
{
    public class SettingsData
    {
        readonly object _locker = new object();
        private readonly IsolatedStorageSettings _settings;
        public IsolatedStorageSettings Settings
        {
            get
            {
                lock (_locker)
                {
                    return _settings;
                }
            }
        }

        public SettingsData()
        {
            _settings = IsolatedStorageSettings.ApplicationSettings;
        }
    }
}
