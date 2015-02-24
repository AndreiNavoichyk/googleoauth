using System;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using GoogleOAuth.Models;
using GoogleOAuthExample.Services;
using GoogleOAuthExample.ViewModels.Base;
using Localization;
using ILog = GoogleOAuthExample.Services.ILog;

namespace GoogleOAuthExample.ViewModels
{
    public class UserProfilePageViewModel : BusyViewModel
    {
        #region Properties

        private UserInfo _userInfo;
        private ObservableCollection<PicasaItem> _albums;

        protected ISocialNetworkService SocialNetworkService;
        protected ILog Log;
        protected INavigationService NavigationService;
        protected readonly INotificationsService NotificationService;
        protected readonly IInputService InputService;

        public UserInfo UserInfo
        {
            get { return _userInfo; }
            set
            {
                _userInfo = value;
                NotifyOfPropertyChange(() => UserInfo);
            }
        }

        public ObservableCollection<PicasaItem> Albums
        {
            get { return _albums; }
            set
            {
                _albums = value;
                NotifyOfPropertyChange(() => Albums);
            }
        }

        private PicasaItem _selectedItem;

        public PicasaItem SelectedAlbum
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedAlbum);
            }
        }

        #endregion

        #region Initialize

        public UserProfilePageViewModel(
            ISocialNetworkService socialNetworkService,
            ILog log,
            INavigationService navigationService,
            INotificationsService notificationService,
            IInputService inputService)
        {
            Log = log;
            SocialNetworkService = socialNetworkService;
            NavigationService = navigationService;
            NotificationService = notificationService;
            InputService = inputService;
            LoadData();
        }

        #endregion

        #region Methods

        private async void LoadData()
        {
            try
            {
                StartBusiness();
                UserInfo = await SocialNetworkService.GetInfoAboutMeAsync();
                var albums = await SocialNetworkService.GetAlbumsAsync(UserInfo.Id);
                Albums = new ObservableCollection<PicasaItem>(albums);
            }
            catch (Exception ex)
            {
                Log.Write(string.Format("LoadData Error: {0}", ex.Message));
            }
            finally
            {
                StopBusiness();
            }
        }

        public void Logout()
        {
            if (SocialNetworkService.Logout())
            {
                NavigationService.UriFor<MainPageViewModel>().Navigate();
            }
        }

        public async void AddAlbum()
        {
            var albumName = await InputService.GetString(UINotifications.AddAlbum, UINotifications.EnterNewAlbumName);
            if (string.IsNullOrEmpty(albumName))
            {
                NotificationService.ShowAlert(UINotifications.Error, UINotifications.AlbumsNameCanNotBeEmpty);
                return;
            }
            try
            {
                StartBusiness();
                var result = await SocialNetworkService.AddAlbumAsync(UserInfo.Id, albumName);
                if (result)
                {
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                Log.Write(string.Format("AddAlbumAsync Error: {0}", ex.Message));
            }
            finally
            {
                StopBusiness();
            }
        }

        public async void RemoveAlbum(PicasaItem album)
        {
            if (album == null) return;
            try
            {
                StartBusiness();
                var result = await SocialNetworkService.RemoveAlbumAsync(UserInfo.Id, album.Id, album.Etag);
                if (result)
                {
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                Log.Write(string.Format("RemoveAlbum Error: {0}", ex.Message));
                throw;
            }
            finally
            {
                StopBusiness();
            }
        }

        public void RefreshData()
        {
            UserInfo = null;
            Albums = null;
            LoadData();
        }

        #endregion

        #region EventHandlers

        public void OnAlbumChanged()
        {
            if (SelectedAlbum == null) return;
            NavigationService.UriFor<PhotosPageViewModel>()
                .WithParam(vm => vm.AlbumId, SelectedAlbum.Id)
                .WithParam(vm => vm.UserId, UserInfo.Id)
                .WithParam(vm => vm.AlbumTitle, SelectedAlbum.Title)
                .Navigate();
            SelectedAlbum = null;
        }

        #endregion
    }
}
