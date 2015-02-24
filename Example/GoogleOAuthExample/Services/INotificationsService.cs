namespace GoogleOAuthExample.Services
{
    public interface INotificationsService
    {
        void ShowAlert(string caption, string text);
        bool ShowMessage(string caption, string text);
    }
}
