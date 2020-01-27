namespace Wish
{
    public static class SteamApiAccess
    {
        private static string _url = null;
        private static string _steamID = null;
        private static int _page = 0;

        public static void SetSteamID(string id)
        {
            _steamID = id;
            _url = $"https://store.steampowered.com/wishlist/profiles/{GetSteamID()}/wishlistdata/?p=0";
        }

        public static string GetSteamID()
        {
            return _steamID;
        }

        public static string GetApiUrl()
        {
            return _url;
        }

        public static void SetApiUrl(int page)
        {
            _url = $"https://store.steampowered.com/wishlist/profiles/{GetSteamID()}/wishlistdata/?p={page}";
        }

        public static int GetNextPage()
        {
            return ++_page;
        }
    }
}