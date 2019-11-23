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
using System.Diagnostics;
using ReactiveUI.Fody.Helpers;

namespace Mutagen.Bethesda.Examples
{
    public abstract class SimpleOutputVM : ExampleVM
    {
        public ObservableCollectionExtended<string> OutputDisplay { get; } = new ObservableCollectionExtended<string>();
        public SourceList<string> Output { get; } = new SourceList<string>();
        public List<string> DelayedOutput { get; } = new List<string>();

        public IReactiveCommand RunCommand { get; }

        [Reactive]
        public string LastTiming { get; private set; }

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
        }

        protected Task Run()
        {
            LastTiming = "...";
            return Task.Run(async () =>
            {
                try
                {
                    Output.Clear();

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    await ToDo();
                    sw.Stop();
                    LastTiming = sw.Elapsed.ToString(@"mm\:ss\.fff");
                    this.Output.Add(DelayedOutput);
                    DelayedOutput.Clear();
                }
                catch (Exception ex)
                {
                    this.Output.Add($"Exception: {ex}");
                }
            });
        }

        protected abstract Task ToDo();
    }
}
