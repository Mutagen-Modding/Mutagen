using System.Collections.Generic;

namespace Mutagen.Bethesda.Core.Pex.Interfaces
{
    public interface IUserFlag : IBinaryObject
    {
        public string? Name { get; set; }
        
        public byte FlagIndex { get; set; }
        
        public uint FlagMask { get; }
    }
    
    public interface IHasUserFlags
    {
        public List<IUserFlag> UserFlags { get; set; }
    }
}
