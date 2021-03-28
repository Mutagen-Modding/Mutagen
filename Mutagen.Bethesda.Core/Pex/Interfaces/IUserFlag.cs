using Noggog;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Pex
{
    public interface IUserFlag : IBinaryObject
    {
        public string? Name { get; set; }
        
        public byte FlagIndex { get; set; }
        
        public uint FlagMask { get; }
    }
    
    public interface IHasUserFlags
    {
        public ExtendedList<IUserFlag> UserFlags { get; set; }
    }
}
