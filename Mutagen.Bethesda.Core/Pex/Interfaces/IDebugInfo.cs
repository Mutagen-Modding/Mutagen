using System;
using System.Collections.Generic;
using Mutagen.Bethesda.Core.Pex.Enums;

namespace Mutagen.Bethesda.Core.Pex.Interfaces
{
    public interface IDebugInfo : IBinaryObject
    {
        public bool HasDebugInfo { get; set; }
        
        public DateTime ModificationTime { get; set; }
        
        public List<IDebugFunction> Functions { get; set; }
        
        public List<IDebugPropertyGroup> PropertyGroups { get; set; }
        
        public List<IDebugStructOrder> StructOrders { get; set; }
    }

    public interface IDebugFunction : IBinaryObject
    {
        public string? ObjectName { get; set; }
        public string? StateName { get; set; }
        public string? FunctionName { get; set; }
        
        public DebugFunctionType FunctionType { get; set; }
        
        public List<ushort> Instructions { get; set; }
    }

    public interface IDebugPropertyGroup : IBinaryObject
    {
        public string? ObjectName { get; set; }
        public string? GroupName { get; set; }
        public List<string> PropertyNames { get; set; }
    }

    public interface IDebugStructOrder : IBinaryObject
    {
        public string? ObjectName { get; set; }
        public string? OrderName { get; set; }
        public List<string> Names { get; set; }
    }
}
