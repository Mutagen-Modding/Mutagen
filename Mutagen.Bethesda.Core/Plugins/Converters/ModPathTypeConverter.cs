using System.ComponentModel;
using System.Globalization;

namespace Mutagen.Bethesda.Plugins.Converters;

public sealed class ModPathTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        switch (value)
        {
            case string str:
                return ModPath.FromPath(str);
            default:
                return new NotImplementedException();
        }
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value == null) return null;
        var modPath = (ModPath)value;
        return modPath.ToString();
    }
}