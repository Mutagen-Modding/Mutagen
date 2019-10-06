using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace Mutagen.Bethesda.Examples
{
    public abstract class SimpleOutputVM : ExampleVM
    {
        public ObservableCollectionExtended<string> OutputDisplay { get; } = new ObservableCollectionExtended<string>();
        public SourceList<string> Output { get; } = new SourceList<string>();

        public IReactiveCommand RunCommand { get; }

        private readonly ObservableAsPropertyHelper<string> _LastTiming;
        public string LastTiming => _LastTiming.Value;

        public SimpleOutputVM(MainVM mvm)
            : base(mvm)
        {
            this.RunCommand = ReactiveCommand.CreateFromTask(execute: Run);
            this.Output.Connect()
                .Buffer(TimeSpan.FromMilliseconds(100))
                .Where(l => l.Count > 0)
                .ObserveOn(RxApp.MainThreadScheduler)
                .FlattenBufferResult()
                .Bind(this.OutputDisplay)
                .Subscribe()
                .DisposeWith(this.CompositeDisposable);

            this._LastTiming = this.RunCommand.IsExecuting
                .DistinctUntilChanged()
                .TimeInterval()
                .Select(timespan => timespan.Value ? "..." : timespan.Interval.ToString(@"mm\:ss\.fff"))
                .Skip(1)
                .StartWith(string.Empty)
                .ToProperty(this, nameof(LastTiming));
        }

        protected Task Run()
        {
            return Task.Run(async () =>
            {
                this.Output.Clear();
                await ToDo();
                GC.Collect();
            });
        }

        protected abstract Task ToDo();
    }
}
