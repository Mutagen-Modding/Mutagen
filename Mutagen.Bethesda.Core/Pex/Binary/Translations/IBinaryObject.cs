using System.IO;

namespace Mutagen.Bethesda.Pex.Binary.Translations
{
    public interface IBinaryObject
    {
        public void Read(BinaryReader br);
        public void Write(BinaryWriter bw);
    }
}
