using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests;

public enum TestState
{
    NotStarted,
    Running,
    Complete,
    Error,
}

public class Test
{
    private readonly Subject<string> _output = new Subject<string>();
    private readonly Func<Subject<string>, Task> _toDo;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    private readonly BehaviorSubject<TestState> _stateSignal = new BehaviorSubject<TestState>(TestState.NotStarted);
    private readonly List<Test> _children = new List<Test>();
    private readonly bool _parallel;

    public string Name { get; }
    public GameRelease? GameRelease { get; }
    public FilePath? FilePath { get; }
    public IObservable<string> Output => _output;
    public IObservable<string> AllOutput => Observable.Merge(_children.Select(c => c.AllOutput).And(_output));
    public IObservable<TestState> StateSignal => _stateSignal;
    public IReadOnlyList<Test> Children => _children;
    public int ChildCount => _children.Sum(c => c.ChildCount) + _children.Count;

    public Test(string name, bool parallel, Func<Subject<string>, Task> toDo, GameRelease? release = null, FilePath? filePath = null)
    {
        Name = name;
        GameRelease = release;
        FilePath = filePath;
        _toDo = toDo;
        _parallel = parallel;
    }

    public async Task Start()
    {
        try
        {
            _stateSignal.OnNext(TestState.Running);
            _output.OnNext("========================================\\");
            _output.OnNext(Name);
            if (FilePath.HasValue)
            {
                _output.OnNext(FilePath.Value.ToString());
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            await _toDo(_output);
            sw.Stop();
            _output.OnNext("Passed");
            _output.OnNext($"{sw.ElapsedMilliseconds / 1000d}s");
            _output.OnNext("========================================/");
            _stateSignal.OnNext(TestState.Complete);
            if (_parallel)
            {
                await Task.WhenAll(_children.Select(c => Task.Run(c.Start)));
            }
            else
            {
                await Task.WhenAll(_children.Select(c => c.Start()));
            }
            _disposables.Dispose();
        }
        catch (Exception ex)
        {
            _stateSignal.OnError(ex);
            while (ex != null)
            {
                _output.OnNext(ex.ToString());
                ex = ex.InnerException;
            }
            _output.OnNext("Failed");
            _output.OnNext("========================================/");
            _stateSignal.OnNext(TestState.Error);
        }
        finally
        {
            _output.OnCompleted();
            _stateSignal.OnCompleted();
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