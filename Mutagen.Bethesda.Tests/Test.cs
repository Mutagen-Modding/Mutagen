using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
    public class Test
    {
        private readonly Subject<string> _output = new Subject<string>();
        private readonly Func<Subject<string>, Task> _toDo;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly TaskCompletionSource _tcs = new TaskCompletionSource();
        private readonly List<Test> _children = new List<Test>();
        private readonly bool _parallel;

        public string Name { get; }
        public IObservable<string> Output => _output;
        public IObservable<string> AllOutput => Observable.Merge(_children.Select(c => c.AllOutput).And(_output));
        public Task CompleteTask => _tcs.Task;
        public IReadOnlyList<Test> Children => _children;
        public int ChildCount => _children.Sum(c => c.ChildCount) + _children.Count;

        public Test(string name, bool parallel, Func<Subject<string>, Task> toDo)
        {
            Name = name;
            _toDo = toDo;
            _parallel = parallel;
        }

        public async Task Start()
        {
            try
            {
                _output.OnNext("========================================\\");
                _output.OnNext(Name);
                await _toDo(_output);
                _output.OnNext("Passed");
                _output.OnNext("========================================/");
                await Task.WhenAll(_children.Select(c => c.Start()));
                _disposables.Dispose();
            }
            catch (Exception ex)
            {
                _output.OnNext(ex.ToString());
                _output.OnNext("Failed");
                _output.OnNext("========================================/");
            }
            finally
            {
                _output.OnCompleted();
                _tcs.Complete();
            }
        }

        public void AddDisposeAction(IDisposable a)
        {
            _disposables.Add(a);
        }

        public void AddAsChild(Test test)
        {
            _children.Add(test);
        }
    }
}
