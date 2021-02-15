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
