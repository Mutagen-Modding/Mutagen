using System.IO;
using JetBrains.Annotations;

namespace Mutagen.Bethesda.Pex.Interfaces
{
    [PublicAPI]
    public interface IBinaryObject
    {
        public void Read(BinaryReader br);
        public void Write(BinaryWriter bw);
    }
}
