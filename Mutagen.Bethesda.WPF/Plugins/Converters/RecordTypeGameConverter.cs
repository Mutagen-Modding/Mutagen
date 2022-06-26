using Loqui;
using System.Globalization;
using System.Windows.Data;

namespace Mutagen.Bethesda.WPF.Plugins.Converters;

public class RecordTypeGameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Type type) return Binding.DoNothing;
        if (LoquiRegistration.TryGetRegister(type, out var registration))
        {
            return registration.ProtocolKey.Namespace;
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}