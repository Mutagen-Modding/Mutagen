using DynamicData.Binding;
using Noggog.WPF;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Examples
{
    public class PrintContentVM : ViewModel, IExampleVM
    {
        public string Name => "Print Content";

        public ObservableCollectionExtended<string> Names { get; } = new ObservableCollectionExtended<string>();

        public IReactiveCommand RunCommand { get; }

        public PrintContentVM()
        {
            this.RunCommand = ReactiveCommand.Create(
                execute: () =>
                {
                });
        }
    }
}
