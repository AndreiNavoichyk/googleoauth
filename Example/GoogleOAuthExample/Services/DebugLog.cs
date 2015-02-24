using System.Diagnostics;

namespace GoogleOAuthExample.Services
{
    public class DebugLog : ILog
    {
        public void Write(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
