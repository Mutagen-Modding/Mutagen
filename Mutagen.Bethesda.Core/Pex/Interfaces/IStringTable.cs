using JetBrains.Annotations;

namespace Mutagen.Bethesda.Core.Pex.Interfaces
{
    [PublicAPI]
    public interface IStringTable : IBinaryObject
    {
        public string GetFromIndex(ushort index);
    }
}
