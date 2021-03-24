using JetBrains.Annotations;

namespace Mutagen.Bethesda.Core.Pex.Enums
{
    [PublicAPI]
    public enum DebugFunctionType : byte
    {
        Method = 0,
        Getter = 1,
        Setter = 2
    }
}
