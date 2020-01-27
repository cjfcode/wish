namespace Wish
{
    public static class WishData
    {
        private static string _data = null;
        private static string[] _allData = null;
        private static string _path = null;

        public static void SetData(string data)
        {
            _data = data;
        }

        public static string GetData()
        {
            return _data;
        }

        public static void SetAllData(string[] allData)
        {
            _allData = allData;
        }

        public static string[] GetAllData()
        {
            return _allData;
        }

        public static void SetPath(string path)
        {
            _path = path == null ? "Wishlist.txt" : path;
        }

        public static string GetPath()
        {
            if (_path == null)
                SetPath("Wishlist.txt");

            return _path;
        }
    }
}