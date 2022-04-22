using Noggog;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Pex;

internal record PexParseMeta(
    GameCategory Category,
    IBinaryReadStream Reader,
    Dictionary<ushort, string> Strings)
{
    public string ReadString()
    {
        var index = Reader.ReadUInt16();
        return Strings[index];
    }
}