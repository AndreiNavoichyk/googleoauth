using System.Collections.Generic;

namespace GoogleOAuthExample.Services
{
    public class StorageService : IStorageService
    {
        private readonly SettingsData _settingsData;

        public StorageService()
        {
            _settingsData = new SettingsData();
        }

        public void SetValue<T>(string key, T value)
        {
            if (_settingsData.Settings.Contains(key))
            {
                _settingsData.Settings.Remove(key);
            }
            _settingsData.Settings[key] = value;
            _settingsData.Settings.Save();
        }

        public T GetValue<T>(string key)
        {
            if (_settingsData.Settings.Contains(key))
            {
                return (T)_settingsData.Settings[key];
            }
            throw new KeyNotFoundException();
        }

        public T GetValueWithDefault<T>(string key, T defaultValue)
        {
            if (_settingsData.Settings.Contains(key))
            {
                return (T)_settingsData.Settings[key];
            }
            return defaultValue;
        }

        public bool RemoveValue(string key)
        {
            return !_settingsData.Settings.Contains(key) || _settingsData.Settings.Remove(key);
        }
    }
}
