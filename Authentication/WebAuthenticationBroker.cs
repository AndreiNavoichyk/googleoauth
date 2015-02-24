using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Phone.Controls;

namespace Authentication
{
    public sealed class WebAuthenticationBroker
    {
        private static string _responseData = "";
        private static uint _responseErrorDetail;
        private static WebAuthenticationStatus _responseStatus = WebAuthenticationStatus.UserCancel;
        private static readonly AutoResetEvent AuthenticateFinishedEvent = new AutoResetEvent(false);

        public static bool AuthenticationInProgress { get; private set; }

        public static Uri StartUri { get; private set; }

        static WebAuthenticationBroker()
        {
        }

        public static Task<WebAuthenticationResult> AuthenticateAsync(string navigationPageUri, WebAuthenticationOptions options, Uri startUri)
        {
            if (options != WebAuthenticationOptions.None)
                throw new NotImplementedException("This method does not support authentication options other than 'None'.");
            var applicationFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (applicationFrame == null)
                throw new InvalidOperationException();
            StartUri = startUri;
            AuthenticationInProgress = true;
            applicationFrame.Navigate(new Uri(navigationPageUri, UriKind.Relative));
            return Task<WebAuthenticationResult>.Factory.StartNew(() =>
            {
                AuthenticateFinishedEvent.WaitOne();
                return new WebAuthenticationResult(_responseData, _responseStatus, _responseErrorDetail);
            });
        }

        public static void OnAuthenticationFinished(string data, WebAuthenticationStatus status, uint error)
        {
            _responseData = data;
            _responseStatus = status;
            _responseErrorDetail = error;
            AuthenticationInProgress = false;
            AuthenticateFinishedEvent.Set();
        }
    }
}
