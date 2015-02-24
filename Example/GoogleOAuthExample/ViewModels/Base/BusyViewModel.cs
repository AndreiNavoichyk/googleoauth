using Caliburn.Micro;

namespace GoogleOAuthExample.ViewModels.Base
{
    public class BusyViewModel : Screen
    {
        private bool _isBusy;
        private int _busyCounter;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
                NotifyOfPropertyChange(() => IsNotBusy);
            }
        }

        public bool IsNotBusy
        {
            get { return !IsBusy; }
        }

        protected virtual void StartBusiness()
        {
            IsBusy = true;
            ++_busyCounter;
        }

        protected virtual void StopBusiness()
        {
            if (_busyCounter <= 0)
            {
                return;
            }

            --_busyCounter;
            if (_busyCounter == 0)
            {
                Execute.OnUIThread(() => IsBusy = false);
            }
        }
    }
}