using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Noggog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Mutagen.Bethesda.WPF.Plugins.Converters
{
    public class FormKeyLookupConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 3) return Binding.DoNothing;
            if (values[0] is not FormKey formKey) return Binding.DoNothing;
            if (values[1] is not ILinkCache linkCache) return Binding.DoNothing;
            if (values[2] is Type type)
            {
                if (linkCache.TryResolveIdentifier(formKey, type, out var edid))
                {
                    return edid.IsNullOrWhitespace() ? formKey.ToString() : edid;
                }
            }
            else if (values[2] is IEnumerable<Type> types)
            {
                if (linkCache.TryResolveIdentifier(formKey, types, out var edid))
                {
                    return edid.IsNullOrWhitespace() ? formKey.ToString() : edid;
                }
            }
            else
            {
                return Binding.DoNothing;
            }
            if (parameter is string failMessage) return failMessage;
            return formKey.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
