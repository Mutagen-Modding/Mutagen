using System.ComponentModel;
using System.Globalization;

namespace Mutagen.Bethesda.Plugins.Converters;

public sealed class ModKeyTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string)
               || sourceType == typeof(ModPath);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        switch (value)
        {
            case string str:
                return ModKey.FromNameAndExtension(str);
            case ModPath modPath:
                return modPath.ModKey;
            default:
                throw new NotImplementedException();
        }
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value == null) return null;
        var modKey = (ModKey)value;
        return modKey.ToString();
    }
}