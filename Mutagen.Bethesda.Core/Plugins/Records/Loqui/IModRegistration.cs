using Loqui;

namespace Mutagen.Bethesda.Plugins.Records.Loqui;

public interface IModRegistration : ILoquiRegistration
{
    GameCategory GameCategory { get; }
}