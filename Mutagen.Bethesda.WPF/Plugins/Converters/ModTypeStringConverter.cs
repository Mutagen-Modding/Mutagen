using System.Globalization;
using System.Windows.Data;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.WPF.Plugins.Converters;

public class ModTypeStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not ModType modType) return Binding.DoNothing;
        return modType.GetFileExtension();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string str) return Binding.DoNothing;
        if (!ModKey.TryConvertExtensionToType(str, out var modType))
        {
            throw new ArgumentException($"String could not be converted to ModType: {str}");
        }
        return modType;
    }
}