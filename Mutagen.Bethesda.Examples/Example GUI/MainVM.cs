using DynamicData.Binding;
using Newtonsoft.Json;
using Noggog.WPF;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Examples
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial class MainVM
    {
        public ObservableCollectionExtended<ViewModel> Examples { get; } = new ObservableCollectionExtended<ViewModel>();

        [Reactive]
        public ViewModel SelectedExample { get; set; }

        [JsonProperty]
        [Reactive]
        public string ModFilePath { get; set; }

        public MainVM()
        {
            this.Examples.Add(new PrintContentVM(this));
            this.Examples.Add(new RecordAccessThroughFormLinksVM(this));
            this.Examples.Add(new ImportComparisonVM(this));
            this.Examples.Add(new RecordOverrideOrDuplicationVM(this));
        }
    }
}
