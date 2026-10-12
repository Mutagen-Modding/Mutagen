namespace Mutagen.Bethesda.Plugins.Binary.Overlay;

internal sealed class GroupCache<T>
    where T : class
{
    private T? _group;
    private object? _lock;

    public T Get(Func<T> read) => LazyInitializer.EnsureInitialized(ref _group, ref _lock, read);
}
