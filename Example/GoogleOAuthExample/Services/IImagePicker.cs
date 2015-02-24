using System.Threading.Tasks;

namespace GoogleOAuthExample.Services
{
    public interface IImagePicker
    {
        Task<byte[]> FromLibrary();
        Task<byte[]> FromCamera();
    }
}
