namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface ILiveLoadOrderTimings
{
    TimeSpan Throttle { get; }
    TimeSpan RetryInterval { get; }
    TimeSpan RetryIntervalMax { get; }
}

public class LiveLoadOrderTimings : ILiveLoadOrderTimings
{
    public TimeSpan Throttle { get; init; } = TimeSpan.FromMilliseconds(150);
    public TimeSpan RetryInterval { get; init; } = TimeSpan.FromMilliseconds(250);
    public TimeSpan RetryIntervalMax { get; init; } = TimeSpan.FromSeconds(5);
}