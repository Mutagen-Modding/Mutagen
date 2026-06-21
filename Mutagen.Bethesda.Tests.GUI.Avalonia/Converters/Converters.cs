using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Mutagen.Bethesda.Tests;

namespace Mutagen.Bethesda.Tests.GUI.Converters;

public class BoolToBrushConverter : IValueConverter
{
    public IBrush? True { get; set; }
    public IBrush? False { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? True : False;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class TestStateToBrushConverter : IValueConverter
{
    public static readonly IBrush NotStarted = new SolidColorBrush(Color.Parse("#303030"));
    public static readonly IBrush Running = new SolidColorBrush(Color.Parse("#2D6E8A"));
    public static readonly IBrush Complete = new SolidColorBrush(Color.Parse("#2E7D32"));
    public static readonly IBrush Error = new SolidColorBrush(Color.Parse("#7A2E2E"));

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            TestState.Running => Running,
            TestState.Complete => Complete,
            TestState.Error => Error,
            _ => NotStarted,
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class SelectedToBorderBrushConverter : IValueConverter
{
    public static readonly IBrush Selected = new SolidColorBrush(Color.Parse("#1BA1E2"));
    public static readonly IBrush Unselected = Brushes.Transparent;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? Selected : Unselected;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
