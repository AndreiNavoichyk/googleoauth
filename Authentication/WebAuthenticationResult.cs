namespace Authentication
{
    public sealed class WebAuthenticationResult
    {
        public string ResponseData { get; private set; }

        public WebAuthenticationStatus ResponseStatus { get; private set; }

        public uint ResponseErrorDetail { get; private set; }

        public WebAuthenticationResult(string data, WebAuthenticationStatus status, uint error)
        {
            ResponseData = data;
            ResponseStatus = status;
            ResponseErrorDetail = error;
        }
    }
}
