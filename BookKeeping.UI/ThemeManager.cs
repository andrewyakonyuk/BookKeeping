using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using MahApps.Metro;

namespace BookKeeping.UI
{
    public static class ThemeManager
    {
        private static readonly ResourceDictionary LightResource = new ResourceDictionary { Source = new Uri("/Assets/Accents/BaseLight.xaml",UriKind.Relative) };
        private static readonly ResourceDictionary DarkResource = new ResourceDictionary { Source = new Uri("/Assets/Accents/BaseDark.xaml", UriKind.Relative) };

        private static IEnumerable<Accent> _accents;
        public static IEnumerable<Accent> DefaultAccents
        {
            get
            {
                return _accents ?? (_accents =
                    new List<Accent>{
                        new Accent("Red", new Uri("/Assets/Accents/Red.xaml",UriKind.Relative)),
                        new Accent("Green", new Uri("/Assets/Accents/Green.xaml", UriKind.Relative)),
                        new Accent("Blue", new Uri("/Assets/Accents/Blue.xaml", UriKind.Relative)),
                        new Accent("Purple", new Uri("/Assets/Accents/Purple.xaml", UriKind.Relative)),
                        new Accent("Orange", new Uri("/Assets/Accents/Orange.xaml", UriKind.Relative)),        
                        new Accent("VS", new Uri("/Assets/Accents/VS.xaml", UriKind.Relative)),
                    });
            }
        }

        public static void ChangeTheme(Application app, Accent accent, Theme theme)
        {
            ChangeTheme(app.Resources, accent, theme);
        }

        public static void ChangeTheme(Window window, Accent accent, Theme theme)
        {
            ChangeTheme(window.Resources, accent, theme);
        }

        public static void ChangeTheme(ResourceDictionary r, Accent accent, Theme theme)
        {
            ThemeIsDark = (theme == Theme.Dark);
            var themeResource = (theme == Theme.Light) ? LightResource : DarkResource;
            ApplyResourceDictionary(themeResource, r);
            ApplyResourceDictionary(accent.Resources, r);
        }

        public static bool ThemeIsDark { get; private set; }

        private static void ApplyResourceDictionary(ResourceDictionary newRd, ResourceDictionary oldRd)
        {
            foreach (DictionaryEntry r in newRd)
            {
                if (oldRd.Contains(r.Key))
                    oldRd.Remove(r.Key);

                oldRd.Add(r.Key, r.Value);
            }
        }
    }
}