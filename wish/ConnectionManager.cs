using System.Net;

namespace Wish
{
    public static class ConnectionManager
    {
        private static WebClient _client;

        public static WebClient GetWebClient()
        {
            return _client = new WebClient();
        }

        public static void ReleaseWebClient()
        {
            _client.Dispose();
        }
    }
}