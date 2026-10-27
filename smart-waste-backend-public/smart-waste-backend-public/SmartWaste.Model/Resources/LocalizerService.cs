using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace SmartWaste.Model.Resources
{
    public class LocalizerService
    {
        private readonly IStringLocalizer _localizer;

        private readonly Dictionary<string, string> _localization = new()
        {
            { "en", "en-US" },
            { "de", "de-DE"},
            { "fr", "fr-FR" },
            { "hr", "hr"},
            { "es", "es-ES" },
            { "no", "nb"},
            { "pt", "pt-PT" },
            { "ro", "ro"},
            { "sv", "sv"},
            { "it", "it"}
        };

        public LocalizerService(IStringLocalizerFactory factory)
        {
            var type = typeof(SharedResources);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("SharedResources", assemblyName.Name);
        }

        public string GetLocalString(string key, string language)
        {
            var culture = new CultureInfo(_localization[language]);
            using (new CultureScope(culture))
            {
                string value;
                try
                {
                    value = _localizer[key].Value.ToString();
                }
                catch (Exception)
                {
                    value = "";
                }

                return value;
            }
        }
    }

    public class CultureScope : IDisposable
    {
        private readonly CultureInfo _originalCulture;
        private readonly CultureInfo _originalUiCulture;

        public CultureScope(CultureInfo culture)
        {
            _originalCulture = CultureInfo.CurrentCulture;
            _originalUiCulture = CultureInfo.CurrentUICulture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = _originalCulture;
            CultureInfo.CurrentUICulture = _originalUiCulture;
        }
    }
}
