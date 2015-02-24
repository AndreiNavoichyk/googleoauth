using System.Threading.Tasks;

namespace GoogleOAuthExample.Services
{
    public interface IInputService
    {
        Task<string> GetString(string title, string message);
    }
}
