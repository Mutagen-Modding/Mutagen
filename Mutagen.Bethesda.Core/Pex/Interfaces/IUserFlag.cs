using System.Collections.Generic;
using JetBrains.Annotations;

namespace Mutagen.Bethesda.Core.Pex.Interfaces
{
    [PublicAPI]
    public interface IUserFlag : IBinaryObject
    {
        public string? Name { get; set; }
        
        public byte FlagIndex { get; set; }
        
        public uint FlagMask { get; }
    }
    
    [PublicAPI]
    public interface IHasUserFlags
    {
        public List<IUserFlag> UserFlags { get; set; }
    }
}
