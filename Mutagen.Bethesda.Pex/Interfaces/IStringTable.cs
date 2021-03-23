using JetBrains.Annotations;

namespace Mutagen.Bethesda.Pex.Interfaces
{
    [PublicAPI]
    public interface IStringTable : IBinaryObject
    {
        public string GetFromIndex(ushort index);
    }
}
