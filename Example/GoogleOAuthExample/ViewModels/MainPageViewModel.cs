using System;
using Caliburn.Micro;
using GoogleOAuthExample.Services;
using GoogleOAuthExample.ViewModels.Base;
using ILog = GoogleOAuthExample.Services.ILog;

namespace GoogleOAuthExample.ViewModels
{
    public class MainPageViewModel : BusyViewModel
    {
        #region Properties

        protected readonly ILog Log;
        protected readonly INavigationService NavigationService;
        protected readonly ISocialNetworkService SocialNetworkService;

        #endregion

        #region Initialize

        public MainPageViewModel(
            INavigationService navigationService,
            ISocialNetworkService socialNetworkService,
            ILog log)
        {
            Log = log;
            NavigationService = navigationService;
            SocialNetworkService = socialNetworkService;

            if (SocialNetworkService.IsLogged())
            {
                Login();
            }
        }

        #endregion

        #region EventHandlers

        public async void Login()
        {
            try
            {
                StartBusiness();
                var isLoginSuccess = await SocialNetworkService.LoginAsync();
                if (isLoginSuccess)
                {
                    NavigationService.UriFor<UserProfilePageViewModel>().Navigate();
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
            }
            finally
            {
                StopBusiness();
            }
        }

        #endregion
    }
}
