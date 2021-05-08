using Loqui;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Mutagen.Bethesda.WPF.Plugins.Converters
{
    public class RecordTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Type type) return Binding.DoNothing;
            if (LoquiRegistration.TryGetRegister(type, out var register))
            {
                return register.ClassType.Name;
            }
            return type.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
