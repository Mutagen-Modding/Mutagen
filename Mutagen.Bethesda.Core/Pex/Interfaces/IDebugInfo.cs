using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mutagen.Bethesda.Core.Pex.Enums;

namespace Mutagen.Bethesda.Core.Pex.Interfaces
{
    [PublicAPI]
    public interface IDebugInfo : IBinaryObject
    {
        public bool HasDebugInfo { get; set; }
        
        public DateTime ModificationTime { get; set; }
        
        public List<IDebugFunction> Functions { get; set; }
        
        public List<IDebugPropertyGroup> PropertyGroups { get; set; }
        
        public List<IDebugStructOrder> StructOrders { get; set; }
    }

    [PublicAPI]
    public interface IDebugFunction : IBinaryObject
    {
        public ushort ObjectNameIndex { get; set; }
        
        public ushort StateNameIndex { get; set; }
        
        public ushort FunctionNameIndex { get; set; }
        
        public DebugFunctionType FunctionType { get; set; }
        
        public List<ushort> Instructions { get; set; }

        public string GetObjectName(IStringTable stringTable);
        public string GetStateName(IStringTable stringTable);
        public string GetFunctionName(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IDebugPropertyGroup : IBinaryObject
    {
        public ushort ObjectNameIndex { get; set; }
        
        public ushort GroupNameIndex { get; set; }

        public List<ushort> PropertyNameIndices { get; set; }
        
        public string GetObjectName(IStringTable stringTable);
        public string GetGroupName(IStringTable stringTable);
        public List<string> GetPropertyNames(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IDebugStructOrder : IBinaryObject
    {
        public ushort ObjectNameIndex { get; set; }
        
        public ushort OrderNameIndex { get; set; }
        
        public List<ushort> NameIndices { get; set; }
        
        public string GetObjectName(IStringTable stringTable);
        public string GetOrderName(IStringTable stringTable);
        public List<string> GetNames(IStringTable stringTable);
    }
}
