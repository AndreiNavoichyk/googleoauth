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
    public class PhotosPageViewModel : BusyViewModel
    {
        #region Properties

        private string _albumTitle;
        private ObservableCollection<PicasaItem> _photos;

        protected ILog Log;
        protected readonly INavigationService NavigationService;
        protected readonly ISocialNetworkService SocialNetworkService;
        protected readonly IImagePicker ImagePicker;
        protected readonly INotificationsService NotificationsService;

        public string AlbumTitle
        {
            get { return _albumTitle; }
            set
            {
                _albumTitle = value;
                NotifyOfPropertyChange(() => AlbumTitle);
            }
        }

        public ObservableCollection<PicasaItem> Photos
        {
            get { return _photos; }
            set
            {
                _photos = value;
                NotifyOfPropertyChange(() => Photos);
            }
        }

        public string UserId { get; set; }
        public string AlbumId { get; set; }

        #endregion

        #region Initialize

        public PhotosPageViewModel(
            ILog log,
            INavigationService navigationService,
            ISocialNetworkService socialNetworkService,
            IImagePicker imagePicker,
            INotificationsService notificationsService)
        {
            Log = log;
            NavigationService = navigationService;
            SocialNetworkService = socialNetworkService;
            ImagePicker = imagePicker;
            NotificationsService = notificationsService;
        }

        protected override void OnActivate()
        {
            GetPhotos();
            base.OnActivate();
        }

        #endregion

        #region Methods

        private async void GetPhotos()
        {
            try
            {
                StartBusiness();
                Photos = new ObservableCollection<PicasaItem>(await SocialNetworkService.GetPhotosAsync(UserId, AlbumId));
            }
            catch (Exception ex)
            {
                Log.Write(string.Format("GetPhotos Error: {0}", ex.Message));
                throw;
            }
            finally
            {
                StopBusiness();
            }
        }

        public async void AddFromCamera()
        {
            var image = await ImagePicker.FromCamera();
            if (image == null)
            {
                NotificationsService.ShowAlert(UINotifications.Error, UINotifications.NoOnePictureIsLoaded);
                return;
            }
            UploadImage(image);
        }

        public async void AddFromLibrary()
        {
            var image = await ImagePicker.FromLibrary();
            if (image == null)
            {
                NotificationsService.ShowAlert(UINotifications.Error, UINotifications.NoOnePictureIsLoaded);
                return;
            }
            UploadImage(image);
        }

        private async void UploadImage(byte[] data)
        {
            try
            {
                StartBusiness();
                var result = await SocialNetworkService.PostImageAsync(UserId, AlbumId, data);
                if (result)
                {
                    GetPhotos();
                }
            }
            catch (Exception ex)
            {
                Log.Write(string.Format("PostImageAsync Error: {0}", ex.Message));
            }
            finally
            {
                StopBusiness();
            }
        }

        public async void RemoveItem(PicasaItem item)
        {
            if (item == null) return;
            try
            {
                StartBusiness();
                var result = await SocialNetworkService.RemoveImageAsync(UserId, AlbumId, item.Id, item.Etag);
                if (result)
                {
                    GetPhotos();
                }
            }
            catch (Exception ex)
            {
                NotificationsService.ShowAlert(UINotifications.Error, UINotifications.SomeErrorWhileDeletingItem);
                Log.Write(string.Format("RemoveItem Error: {0}", ex.Message));
            }
            finally
            {
                StopBusiness();
            }
        }

        #endregion
    }
}
