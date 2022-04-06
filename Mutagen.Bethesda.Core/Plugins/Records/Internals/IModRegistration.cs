using Loqui;

namespace Mutagen.Bethesda.Plugins.Records.Internals;

public interface IModRegistration : ILoquiRegistration
{
    GameCategory GameCategory { get; }
}