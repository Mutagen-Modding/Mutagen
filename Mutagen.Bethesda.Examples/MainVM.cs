using DynamicData.Binding;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Examples
{
    public partial class MainVM
    {
        public ObservableCollectionExtended<ViewModel> Examples { get; } = new ObservableCollectionExtended<ViewModel>();

        private ViewModel _SelectedExample;
        public ViewModel SelectedExample { get => _SelectedExample; set => this.RaiseAndSetIfChanged(ref _SelectedExample, value); }

        public MainVM(MainWindow window)
        {
            this.Examples.Add(new PrintContentVM(this));
            this.Examples.Add(new RecordAccessThroughFormLinksVM(this));
        }
    }
}
