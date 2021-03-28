using System;
using System.Collections.Generic;
using Noggog;

namespace Mutagen.Bethesda.Pex
{
    public interface IPexFile : IBinaryObject
    {
        public uint Magic { get; set; }
        
        public byte MajorVersion { get; set; }
        
        public byte MinorVersion { get; set; }
        
        public ushort GameId { get; set; }
        
        public DateTime CompilationTime { get; set; }
        
        public string SourceFileName { get; set; }
        
        public string Username { get; set; }
        
        public string MachineName { get; set; }

        public IDebugInfo? DebugInfo { get; set; }

        public ExtendedList<IPexObject> Objects { get; set; }
    }
}
