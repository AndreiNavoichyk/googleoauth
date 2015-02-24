using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Authentication;
using GoogleOAuth.Models;
using Newtonsoft.Json;
using WebClient;

namespace GoogleOAuth
{
    public class GooglePlusProxy
    {
        #region Properties

        private static string _clientId;
        private static string _clientSecret;
        private GoogleSession _session;
        private string _refreshToken;

        protected readonly WebClient.WebClient WebClient;

        public GoogleSession Session
        {
            get { return _session; }
            set { _session = value; }
        }

        #endregion

        #region Initialize

        public GooglePlusProxy()
        {
            WebClient = new WebClient.WebClient();
        }

        #endregion

        #region Methods

        public void RegisterApp(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<GoogleSession> LoginAsync()
        {
            if (string.IsNullOrEmpty(_clientId))
                throw new NullReferenceException("You must register app at first!");
            Session = await PromptOAuthDialog(_clientId, WebAuthenticationOptions.None);
            return Session;
        }

        private async Task<GoogleSession> PromptOAuthDialog(string clientId, WebAuthenticationOptions options)
        {
            var startUri = GetLoginUrl(clientId);
            var result = await WebAuthenticationBroker.AuthenticateAsync("/GoogleOAuth;component/LoginPage.xaml", options, startUri);
            if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                throw new InvalidOperationException();
            if (result.ResponseStatus == WebAuthenticationStatus.UserCancel)
                throw new InvalidOperationException();
            if (string.IsNullOrEmpty(result.ResponseData))
                throw new InvalidCastException();
            return await GetToken(result.ResponseData);
        }

        private Uri GetLoginUrl(string clientId)
        {
            var url = string.Format("{0}?response_type=code&redirect_uri={1}&scope={2}&client_id={3}",
                GooglePlusResources.GeneralOAuthUri + "/auth",
                GooglePlusResources.RedirectUri,
                GooglePlusResources.Scope,
                clientId);
            return new Uri(url, UriKind.Absolute);
        }

        public async Task<GoogleSession> GetToken(string code)
        {
            var parameters = new Dictionary<string, string>
            {
                {"code", code},
                {"client_id", _clientId},
                {"client_secret", _clientSecret},
                {"redirect_uri", GooglePlusResources.RedirectUri},
                {"grant_type", "authorization_code"}
            };
            Session = await DoPostRequest<GoogleSession>(GooglePlusResources.GeneralOAuthUri + "/token", parameters);
            return Session;
        }

        public async Task<GoogleSession> RefreshTokenAsync(GoogleSession session)
        {
            _refreshToken = session.RefreshToken;
            var parameters = new Dictionary<string, string>
            {
                {"client_id", _clientId},
                {"client_secret", _clientSecret},
                {"refresh_token", session.RefreshToken},
                {"grant_type", "refresh_token"}
            };
            Session = await DoPostRequest<GoogleSession>(GooglePlusResources.GeneralOAuthUri + "/token", parameters);
            Session.RefreshToken = _refreshToken;
            return Session;
        }

        public async Task<UserInfo> GetInfoAboutMeAsync()
        {
            return await DoGetRequest<UserInfo>(GooglePlusResources.GooglePlusUri + "/people/me", _session.AccessToken);
        }

        public async Task<UserInfo> GetUserInfoAsync(string id)
        {
            return await DoGetRequest<UserInfo>(GooglePlusResources.GooglePlusUri + "/people/" + id, _session.AccessToken);
        }

        public async Task<object> GetUsersAsync()
        {
            return
                await
                    DoGetRequest<object>(GooglePlusResources.GooglePlusUri + "people", _session.AccessToken);
        }

        public async Task<IEnumerable<PicasaItem>> GetAlbumsAsync(string userId)
        {
            try
            {
                var response =
                    await
                        WebClient.DoGetAsync(string.Format("{0}/{1}", GooglePlusResources.PicasaUrl, userId), _session.AccessToken);
                return ResponseParser.Parse(await response.Content.ReadAsStringAsync());
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<PicasaItem>> GetPhotosAsync(string userId, string albumId)
        {
            try
            {
                var response =
                    await
                        WebClient.DoGetAsync(string.Format("{0}/{1}/albumid/{2}", GooglePlusResources.PicasaUrl, userId, albumId), _session.AccessToken);
                return ResponseParser.Parse(await response.Content.ReadAsStringAsync());
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> PostImageAsync(string userId, string albumId, byte[] data)
        {
            try
            {
                await WebClient.DoPostAsync(string.Format("{0}/{1}/albumid/{2}", GooglePlusResources.PicasaUrl, userId, albumId), data, _session.AccessToken, PostData.ImageJpeg);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveImageAsync(string userId, string albumId, string photoId, string etag)
        {
            try
            {
                await WebClient.DoDeleteAsync(string.Format(GooglePlusResources.RemoveImageUrl, userId, albumId, photoId), _session.AccessToken, etag);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> AddAlbumAsync(string userId, string albumName)
        {
            try
            {
                await WebClient.DoPostAsync(string.Format("{0}/{1}", GooglePlusResources.PicasaUrl, userId),
                                            string.Format(GooglePlusResources.AddAlbumRequestContent, albumName),
                                            _session.AccessToken, PostData.String);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveAlbumAsync(string userId, string albumId, string etag)
        {
            try
            {
                await WebClient.DoDeleteAsync(string.Format(GooglePlusResources.RemoveAlbumUrl, userId, albumId), _session.AccessToken, etag);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Helpers

        private async Task<TModel> DoPostRequest<TModel>(string path, Dictionary<string, string> parameters, Action<Exception> exceptionHandler = null)
            where TModel : class
        {
            try
            {
                var response = await WebClient.DoPostAsync(path, parameters);

                var stream = await response.Content.ReadAsStreamAsync();

                var model = DeserializeData<TModel>(stream);

                return model;
            }
            catch (Exception exception)
            {
                if (exceptionHandler != null)
                {
                    exceptionHandler(exception);
                }
            }

            return null;
        }

        private async Task<TModel> DoGetRequest<TModel>(string path, string authorizationString, Action<Exception> exceptionHandler = null)
            where TModel : class
        {
            try
            {
                var response = await WebClient.DoGetAsync(path, authorizationString);

                var stream = await response.Content.ReadAsStreamAsync();

                var model = DeserializeData<TModel>(stream);

                return model;
            }
            catch (Exception exception)
            {
                if (exceptionHandler != null)
                {
                    exceptionHandler(exception);
                }
            }

            return null;
        }

        private static T DeserializeData<T>(Stream stream) where T : class
        {
            var validatedString = new StreamReader(stream).ReadToEnd();
            var dto = JsonConvert.DeserializeObject<T>(validatedString);
            return dto;
        }

        #endregion
    }
}
