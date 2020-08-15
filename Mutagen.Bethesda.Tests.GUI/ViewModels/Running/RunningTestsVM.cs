using DynamicData.Binding;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests.GUI
{
    public class RunningTestsVM : ViewModel
    {
        public ObservableCollectionExtended<GroupTestVM> Groups { get; } = new ObservableCollectionExtended<GroupTestVM>();

        [Reactive]
        public Exception? Error { get; set; }

        [Reactive]
        public PassthroughTestVM? SelectedPassthrough { get; set; }

        private readonly ObservableAsPropertyHelper<TestVM?> _SelectedTest;
        public TestVM? SelectedTest => _SelectedTest.Value;

        public RunningTestsVM()
        {
            _SelectedTest = this.WhenAnyValue(x => x.SelectedPassthrough!.SelectedTest)
                .ToGuiProperty(this, nameof(SelectedTest));
        }

        public async Task Run(MainVM mvm)
        {
            Groups.AddRange(mvm.Groups
                .Where(g => g.Do)
                .Where(g => g.Passthroughs.Select(p => p.Do).Any())
                .Select(g => new GroupTestVM(this, g)));
            await Task.WhenAll(Groups
                .SelectMany(g => g.Passthroughs.Items)
                .Select(t => Task.Run(t.Run)));
        }
    }
}
