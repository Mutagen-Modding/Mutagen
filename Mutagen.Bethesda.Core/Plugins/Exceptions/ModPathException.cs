using Noggog;

namespace Mutagen.Bethesda.Plugins.Exceptions;

public abstract class ModPathException : Exception
{
    public ModPath ModPath => ModPaths[0];
    public IReadOnlyList<ModPath> ModPaths { get; }

    public ModPathException(ModPath path, string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
        ModPaths = path.AsEnumerable().ToArray();
    }
    public ModPathException(IEnumerable<ModPath> path, string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
        ModPaths = path.ToArray();
    }

    public ModPathException(ModKey key, string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
        ModPaths = new ModPath(key, string.Empty).AsEnumerable().ToArray();
    }

    public ModPathException(IEnumerable<ModKey> keys, string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
        ModPaths = keys.Select(key => new ModPath(key, string.Empty)).ToArray();
    }

    public override string ToString()
    {
        if (ModPaths.Count == 1)
        {
            return $"{GetType().Name} {ModPath}: {this.Message} {this.InnerException}{this.StackTrace}";
        }
        else
        {
            return
                $"{GetType().Name} {ModPath} (+{ModPaths.Count - 1}): {this.Message} {this.InnerException}{this.StackTrace}";
        }
    }
}