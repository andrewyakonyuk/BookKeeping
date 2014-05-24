using System.Resources;

namespace BookKeeping.UI.Localization
{
    public static class ResourceLocalizer
    {
        static ResourceManager _resourceManager;

        static ResourceLocalizer()
        {
        }

        public static void Initialize(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;

            _resourceManager.IgnoreCase = true;
            _instance = (format, args) => GetString(format, args);
        }

        private static string GetString(string format, params object[] args)
        {
            var formatedString = (args == null || args.Length == 0) ? format : string.Format(format, args);
            var localizedString = _resourceManager.GetString(formatedString);
            if (localizedString == null)
                return formatedString;
            return _resourceManager.GetString(formatedString);
        }

        private static Localizer _instance;

        public static Localizer Instance { get { return _instance; } }
    }
}