using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoogleOAuth;
using GoogleOAuth.Models;

namespace GoogleOAuthExample.Services
{
    public class GooglePlusService : ISocialNetworkService
    {
        #region Properties

        private const string ClientId = "467960915198-fjppvvt1lu3t3n16j84l02u4etopdn53.apps.googleusercontent.com";
        private const string ClientSecret = "lFcL3lRS4BP9X3qSiiknzlzo";
        // ReSharper disable once InconsistentNaming
        private const string ACCOUNT_STORAGE_KEY = "ACCOUNT_STORAGE_KEY";
        private GoogleSession _session;

        protected GooglePlusProxy Proxy;
        protected readonly ILog Log;
        protected readonly IStorageService StorageService;

        #endregion

        #region Initialize

        public GooglePlusService(
            IStorageService storageService,
            ILog log)
        {
            Proxy = new GooglePlusProxy();
            Log = log;
            StorageService = storageService;
        }

        #endregion

        public bool IsLogged()
        {
            Proxy.RegisterApp(ClientId, ClientSecret);
            try
            {
                _session = StorageService.GetValue<GoogleSession>(ACCOUNT_STORAGE_KEY);
                Proxy.Session = _session;
                return true;
            }
            catch (KeyNotFoundException)
            {
                Log.Write("Google session isn't in cache");
            }
            return false;
        }

        public async Task<bool> LoginAsync()
        {
            if (IsLogged()) return true;
            try
            {
                _session = await Proxy.LoginAsync();
                StorageService.SetValue(ACCOUNT_STORAGE_KEY, _session);
            }
            catch (Exception ex)
            {
                Log.Write(string.Format("LoginAsync Error: {0}", ex.Message));
                throw;
            }
            return true;
        }

        public async Task<UserInfo> GetUserInfoAsync(string id)
        {
            await CheckSession();
            try
            {
                return await Proxy.GetUserInfoAsync(id);
            }
            catch (Exception ex)
            {
                Log.Write(string.Format("GetUserInfoAsync Error: {0}", ex.Message));
                throw;
            }
        }

        public async Task<IEnumerable<PicasaItem>> GetAlbumsAsync(string userId)
        {
            await CheckSession();
            try
            {
                return await Proxy.GetAlbumsAsync(userId);
            }
            catch (Exception ex)
            {
                Log.Write(string.Format("GetAlbumsAsync Error: {0}", ex.Message));
                throw;
            }
        }

        public async Task<IEnumerable<PicasaItem>> GetPhotosAsync(string userId, string albumId)
        {
            await CheckSession();
            try
            {
                return await Proxy.GetPhotosAsync(userId, albumId);
            }
            catch (Exception ex)
            {
                Log.Write(string.Format("GetPhotosAsync Error: {0}", ex.Message));
                throw;
            }
        }

        public async Task<bool> PostImageAsync(string userId, string albumId, byte[] data)
        {
            await CheckSession();
            try
            {
                return await Proxy.PostImageAsync(userId, albumId, data);
            }
            catch (Exception ex)
            {
                Log.Write(string.Format("PostImageAsync Error: {0}", ex.Message));
                throw;
            }
        }

        public async Task<bool> RemoveImageAsync(string userId, string albumId, string photoId, string etag)
        {
            await CheckSession();
            try
            {
                return await Proxy.RemoveImageAsync(userId, albumId, photoId, etag);
            }
            catch (Exception ex)
            {
                Log.Write(string.Format("RemoveImageAsync Error: {0}", ex.Message));
                throw;
            }
        }

        public async Task<bool> AddAlbumAsync(string userId, string albumName)
        {
            await CheckSession();
            try
            {
                return await Proxy.AddAlbumAsync(userId, albumName);
            }
            catch (Exception ex)
            {
                Log.Write(string.Format("AddAlbumAsync Error: {0}", ex.Message));
                throw;
            }
        }

        public async Task<bool> RemoveAlbumAsync(string userId, string albumId, string etag)
        {
            await CheckSession();
            try
            {
                return await Proxy.RemoveAlbumAsync(userId, albumId, etag);
            }
            catch (Exception ex)
            {
                Log.Write(string.Format("RemoveAlbumAsync Error: {0}", ex.Message));
                throw;
            }
        }

        public bool Logout()
        {
            return StorageService.RemoveValue(ACCOUNT_STORAGE_KEY);
        }

        public async Task<UserInfo> GetInfoAboutMeAsync()
        {
            await CheckSession();
            try
            {
                return await Proxy.GetInfoAboutMeAsync();
            }
            catch (Exception ex)
            {
                Log.Write(string.Format("GetInfoAboutMeAsync Error: {0}", ex.Message));
                throw;
            }
        }

        #region Helpers

        private async Task CheckSession()
        {
            if (DateTime.Now < _session.TokenExpireDateTime)
                return;
            try
            {
                _session = await Proxy.RefreshTokenAsync(_session);
            }
            catch (Exception ex)
            {
                Log.Write(string.Format("Refresh token Error: {0}", ex.Message));
                throw;
            }
        }

        #endregion
    }
}
