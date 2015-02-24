using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Localization.Annotations;

namespace Localization
{
    public class LocalizationManager : INotifyPropertyChanged, ILocalizationManager
    {
        // ReSharper disable InconsistentNaming
        public UIStrings UI { get; private set; }
        // ReSharper restore InconsistentNaming
        public UINotifications Notifications { get; private set; }

        public LocalizationManager()
        {
            UI = new UIStrings();
            Notifications = new UINotifications();
        }

        public void Reset(CultureInfo cultureInfo)
        {
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            UIStrings.Culture = cultureInfo;

            NotifyOfPropertyChange("UI");
            NotifyOfPropertyChange("Notifications");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyOfPropertyChange(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}