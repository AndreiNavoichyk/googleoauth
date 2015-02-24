namespace GoogleOAuthExample.Services
{
    public interface IStorageService
    {
        void SetValue<T>(string key, T value);
        T GetValue<T>(string key);
        T GetValueWithDefault<T>(string key, T defaultValue);
        bool RemoveValue(string key);
    }
}
