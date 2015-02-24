using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Navigation;
using Authentication;
using Microsoft.Phone.Controls;

namespace GoogleOAuth
{
    public partial class LoginPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        #region Properties

        private Uri _uri;
        private string _responseData;
        private WebAuthenticationStatus _responseStatus = WebAuthenticationStatus.UserCancel;
        private uint _responseErrorDetail;
        private bool _authenticationStarted;
        private bool _authenticationFinished;

        public Uri Uri
        {
            get { return _uri; }
            set
            {
                _uri = value;
                NotifyOfPropertyChanged("Uri");
            }
        }

        #endregion

        public LoginPage()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _authenticationStarted = true;
            _authenticationFinished = false;
            Uri = WebAuthenticationBroker.StartUri;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (!WebAuthenticationBroker.AuthenticationInProgress || !_authenticationFinished)
                return;
            _authenticationStarted = false;
            _authenticationFinished = false;
            WebAuthenticationBroker.OnAuthenticationFinished(_responseData, WebAuthenticationStatus.Success, _responseErrorDetail);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyOfPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void WebBrowserOnNavigating(object sender, NavigatingEventArgs e)
        {
            if (!e.Uri.Host.Equals("localhost")) return;
            e.Cancel = true;
            var pos = e.Uri.Query.IndexOf("=", StringComparison.Ordinal);
            var messageCode = pos > -1 ? e.Uri.Query.Substring(pos + 1) : null;
            if (messageCode == null) _responseErrorDetail = 1;
            _responseData = messageCode;
            _responseStatus = WebAuthenticationStatus.Success;
            _authenticationFinished = true;
            GoBack();
        }

        private void WebBrowserOnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            var navigationException = e.Exception as WebBrowserNavigationException;
            _responseErrorDetail = navigationException == null ? 0U : (uint)navigationException.StatusCode;
            _responseStatus = WebAuthenticationStatus.ErrorHttp;
            _authenticationFinished = true;
            e.Handled = true;
            GoBack();
        }

        private void LoginPageOnBackKeyPress(object sender, CancelEventArgs e)
        {
            _responseData = string.Empty;
            _responseStatus = WebAuthenticationStatus.UserCancel;
            _authenticationFinished = true;
        }

        private void GoBack()
        {
            if (NavigationService.BackStack.Any())
            {
                NavigationService.GoBack();
            }
        }
    }
}