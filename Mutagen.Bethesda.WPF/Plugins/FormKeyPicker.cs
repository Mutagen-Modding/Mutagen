using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
using System.Windows;

namespace Mutagen.Bethesda.WPF.Plugins
{
    public class FormKeyPicker : AFormKeyPicker
    {
        public double MaxSearchBoxHeight
        {
            get => (double)GetValue(MaxSearchBoxHeightProperty);
            set => SetValue(MaxSearchBoxHeightProperty, value);
        }
        public static readonly DependencyProperty MaxSearchBoxHeightProperty = DependencyProperty.Register(nameof(MaxSearchBoxHeight), typeof(double), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(double.PositiveInfinity));

        public double SearchBoxHeight
        {
            get => (double)GetValue(SearchBoxHeightProperty);
            set => SetValue(SearchBoxHeightProperty, value);
        }
        public static readonly DependencyProperty SearchBoxHeightProperty = DependencyProperty.Register(nameof(SearchBoxHeight), typeof(double), typeof(FormKeyPicker),
             new FrameworkPropertyMetadata(double.NaN));

        static FormKeyPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormKeyPicker), new FrameworkPropertyMetadata(typeof(FormKeyPicker)));
        }

        public FormKeyPicker()
        {
            PickerClickCommand = ReactiveCommand.Create((object o) =>
            {
                switch (o)
                {
                    case IMajorRecordIdentifier identifier:
                        FormKey = identifier.FormKey;
                        InSearchMode = false;
                        break;
                    default:
                        break;
                }
            });
        }
    }
}
