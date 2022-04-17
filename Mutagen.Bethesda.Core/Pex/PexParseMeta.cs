using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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