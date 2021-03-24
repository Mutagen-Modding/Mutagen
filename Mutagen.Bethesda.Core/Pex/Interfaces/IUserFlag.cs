using System.Collections.Generic;
using JetBrains.Annotations;

namespace Mutagen.Bethesda.Core.Pex.Interfaces
{
    [PublicAPI]
    public interface IUserFlag : IBinaryObject
    {
        public ushort NameIndex { get; set; }
        
        public byte FlagIndex { get; set; }
        
        public uint FlagMask { get; }

        public string GetName(IStringTable stringTable);
    }
    
    [PublicAPI]
    public interface IHasUserFlags
    {
        public uint UserFlags { get; set; }

        public List<IUserFlag> GetUserFlags(IUserFlagsTable userFlagsTable);
    }

    [PublicAPI]
    public interface IUserFlagsTable : IBinaryObject
    {
        public List<IUserFlag> GetUserFlags(uint userFlags);
    }
}
