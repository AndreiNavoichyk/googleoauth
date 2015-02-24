using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace GoogleOAuthExample.Services
{
    public class InputService : IInputService
    {
        public async Task<string> GetString(string title, string message)
        {
            var args = await RadInputPrompt.ShowAsync(title, MessageBoxButtons.OKCancel, message, vibrate: false);
            if (args.Result == DialogResult.Cancel || string.IsNullOrEmpty(args.Text)) return null;
            return args.Text;
        }
    }
}
