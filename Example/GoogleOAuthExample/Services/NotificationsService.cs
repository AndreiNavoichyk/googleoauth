using System.Windows;
using Caliburn.Micro;

namespace GoogleOAuthExample.Services
{
    public class NotificationsService : INotificationsService
    {
        public void ShowAlert(string caption, string text)
        {
            Execute.OnUIThread(() => MessageBox.Show(text, caption, MessageBoxButton.OK));
        }

        public bool ShowMessage(string caption, string text)
        {
            return MessageBox.Show(text, caption, MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        }
    }
}
