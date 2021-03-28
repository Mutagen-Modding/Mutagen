using System.IO;

namespace Mutagen.Bethesda.Core.Pex.Interfaces
{
    public interface IBinaryObject
    {
        public void Read(BinaryReader br);
        public void Write(BinaryWriter bw);
    }
}
