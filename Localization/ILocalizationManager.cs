using System.Globalization;

namespace Localization
{
    public interface ILocalizationManager
    {
        void Reset(CultureInfo cultureInfo);
    }
}
