using System.Threading.Tasks;

namespace Mutagen.Bethesda.Plugins.Binary.Parameters;

public class ParallelWriteParameters
{
    public static readonly ParallelWriteParameters Default = new();

    /// <summary>
    /// Gets or sets the <see cref="System.Threading.Tasks.TaskScheduler">TaskScheduler</see>
    /// associated with this <see cref="ParallelWriteParameters"/> instance. Setting this property to null
    /// indicates that the current scheduler should be used.
    /// </summary>
    public TaskScheduler? TaskScheduler { get; init; }

    /// <summary>
    /// Gets or sets the maximum degree of parallelism enabled by this ParallelOptions instance.
    /// </summary>
    /// <remarks>
    /// The <see cref="MaxDegreeOfParallelism"/> limits the number of concurrent operations run by <see
    /// cref="System.Threading.Tasks.Parallel">Parallel</see> method calls that are passed this
    /// ParallelOptions instance to the set value, if it is positive. If <see
    /// cref="MaxDegreeOfParallelism"/> is -1, then there is no limit placed on the number of concurrently
    /// running operations.
    /// </remarks>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// The exception that is thrown when this <see cref="MaxDegreeOfParallelism"/> is set to 0 or some
    /// value less than -1.
    /// </exception>
    public int MaxDegreeOfParallelism { get; init; } = -1;

    /// <summary>
    /// How many records to include into a new parallel task
    /// </summary>
    public ushort CutCount { get; init; } = 100;

    public ParallelOptions ParallelOptions => new()
    {
        TaskScheduler = TaskScheduler,
        MaxDegreeOfParallelism = MaxDegreeOfParallelism,
    };
}