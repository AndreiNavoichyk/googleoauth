using System.Collections.Generic;
using System.Threading.Tasks;
using GoogleOAuth.Models;

namespace GoogleOAuthExample.Services
{
    public interface ISocialNetworkService
    {
        bool IsLogged();
        Task<bool> LoginAsync();
        Task<UserInfo> GetInfoAboutMeAsync();
        Task<UserInfo> GetUserInfoAsync(string id);
        Task<IEnumerable<PicasaItem>> GetAlbumsAsync(string userId);
        Task<IEnumerable<PicasaItem>> GetPhotosAsync(string userId, string albumId);
        Task<bool> PostImageAsync(string userId, string albumId, byte[] data);
        Task<bool> RemoveImageAsync(string userId, string albumId, string photoId, string etag);
        Task<bool> AddAlbumAsync(string userId, string albumName);
        Task<bool> RemoveAlbumAsync(string userId, string albumId, string etag);
        bool Logout();
    }
}
