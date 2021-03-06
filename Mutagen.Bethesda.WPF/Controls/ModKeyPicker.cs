using Noggog.WPF;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mutagen.Bethesda.WPF
{
    public class ModKeyPicker : AModKeyPicker
    {
        public double MaxSearchBoxHeight
        {
            get => (double)GetValue(MaxSearchBoxHeightProperty);
            set => SetValue(MaxSearchBoxHeightProperty, value);
        }
        public static readonly DependencyProperty MaxSearchBoxHeightProperty = DependencyProperty.Register(nameof(MaxSearchBoxHeight), typeof(double), typeof(ModKeyPicker),
             new FrameworkPropertyMetadata(double.PositiveInfinity));

        public double SearchBoxHeight
        {
            get => (double)GetValue(SearchBoxHeightProperty);
            set => SetValue(SearchBoxHeightProperty, value);
        }
        public static readonly DependencyProperty SearchBoxHeightProperty = DependencyProperty.Register(nameof(SearchBoxHeight), typeof(double), typeof(ModKeyPicker),
             new FrameworkPropertyMetadata(double.NaN));

        public ModKeyPicker()
        {
            PickerClickCommand = ReactiveCommand.Create((object o) =>
            {
                switch (o)
                {
                    case ModKey modKey:
                        ModKey = modKey;
                        InSearchMode = false;
                        break;
                    default:
                        break;
                }
            });
        }
    }
}
