using System.IO;

namespace Mutagen.Bethesda.Pex;

public interface IBinaryObject
{
    public void Read(BinaryReader br);
    public void Write(BinaryWriter bw);
}