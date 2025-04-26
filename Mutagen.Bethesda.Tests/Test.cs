using Noggog;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Noggog.WorkEngine;

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
    private readonly Subject<string> _output = new();
    private readonly IWorkDropoff _dropoff;
    private readonly Func<Subject<string>, Task> _toDo;
    private readonly CompositeDisposable _disposables = new();
    private readonly BehaviorSubject<TestState> _stateSignal = new(TestState.NotStarted);
    private readonly List<Test> _children = new();

    public string Name { get; }
    public GameRelease? GameRelease { get; }
    public FilePath? FilePath { get; }
    public IObservable<string> Output => _output;
    public IObservable<string> AllOutput => Observable.Merge(_children.Select(c => c.AllOutput).And(_output));
    public IObservable<TestState> StateSignal => _stateSignal;
    public IReadOnlyList<Test> Children => _children;
    public int ChildCount => _children.Sum(c => c.ChildCount) + _children.Count;

    public Test(string name, IWorkDropoff workDropoff, Func<Subject<string>, Task> toDo, GameRelease? release = null, FilePath? filePath = null)
    {
        Name = name;
        GameRelease = release;
        FilePath = filePath;
        _dropoff = workDropoff;
        _toDo = toDo;
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
            await _dropoff.EnqueueAndWait(_children, c =>
            {
                Debug.WriteLine($"Enqueuing {c.Name}");
                return c.Start();
            });
            _disposables.Dispose();
        }
        catch (Exception ex)
        {
            _stateSignal.OnError(ex);
            while (ex != null)
            {
                _output.OnNext(ex.ToString());
                if (ex.StackTrace != null)
                {
                    _output.OnNext(ex.StackTrace);
                }
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