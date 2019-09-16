using DynamicData.Binding;
using Noggog.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Examples
{
    public class MainVM : ViewModel
    {
        public ObservableCollectionExtended<ViewModel> Examples { get; } = new ObservableCollectionExtended<ViewModel>();

        private ViewModel _SelectedExample;
        public ViewModel SelectedExample { get => _SelectedExample; set => this.RaiseAndSetIfChanged(ref _SelectedExample, value); }

        public MainVM()
        {
        }

        public MainVM(MainWindow window)
        {
            this.Examples.Add(new ImportComparisonVM());
            this.Examples.Add(new PrintContentVM());
        }
    }
}
